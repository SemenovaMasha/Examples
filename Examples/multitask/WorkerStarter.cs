using Core.ErrorManagment;
using Core.Shared.Extensions;
using Core.SRD;
using ObjectModel.Core.LongProcess;
using ObjectModel.Directory.Core.LongProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Register.LongProcessManagment
{
    public class WorkerStarter
    {
        private readonly WorkerCommon _workerData;

        private CancellationToken _cancelToken;

        public WorkerStarter(WorkerCommon workerData, CancellationToken cancelToken)
        {
            _workerData = workerData;
            _cancelToken = cancelToken;
        }

        public void Start()
        {
            while (true)
            {
                try
                {
                    if (_cancelToken.IsCancellationRequested)
                    {
                        break;
                    }

                    if (LongProcessesConfiguration.GetConfig().AllConfiguredProcessNames.Count == 0)
                    {
                        LogError(addedQueue, $"[{DateTime.Now:dd.MM.yyyy HH:mm:ss}]: WorkerStarter: Не сконфигурирован ни один процесс");
                        return;
                    }

                    List<long> addedProcessTypeIds = new List<long>();

                    // Первый добавлен - первый отправлен на обработку
                    List<OMQueue> addedQueueList = OMQueue.Where(x => x.Status_Code == Status.Added &&
                            LongProcessesConfiguration.GetConfig().AllConfiguredProcessNames.Contains(x.ParentProcessType.ProcessName))
                       .SelectAll()
                       .OrderBy(x => x.CreateDate)
                       .Execute();

					// Отмечаем задачи в очереди - на запуск
                    foreach (OMQueue addedQueue in addedQueueList)
                    {
                        if (!WorkerCommon.ProcessTypeCache.ContainsKey(addedQueue.ProcessTypeId))
                        {
                            LogError(addedQueue, "Не найден тип процесса с ИД: " + addedQueue.ProcessTypeId);
                            continue;
                        }

                        OMProcessType processType = WorkerCommon.ProcessTypeCache[addedQueue.ProcessTypeId];

                        //RSMSUPPORT-1097 проверка на максимальное количество запущенных потоков для типа процесса
                        if (IsMaxRunningTasks(processType, addedProcessTypeIds))
                        {
                            continue;
                        }

                        addedQueue.Status_Code = Status.PrepareToRun;
                        addedQueue.ServiceLogId = _workerData.ApplicationId;
                        addedQueue.LastCheckDate = DateTime.Now;
                        addedQueue.Save();

                        addedProcessTypeIds.Add(processType.Id);
                    }

					// Запускаем отмеченные задачи
                    List<OMQueue> queueList =
                        OMQueue.Where(
                            x =>
                                x.Status_Code == Status.PrepareToRun &&
                                x.ServiceLogId == _workerData.ApplicationId).SelectAll().Execute();

                    foreach (OMQueue queue in queueList)
                    {
                        StartTask(queue);
                    }
                }
                catch (Exception ex)
                {
                    int errorId = ErrorManager.LogError(ex);
                }

				Thread.Sleep(TimeSpan.FromMilliseconds(_workerData.TaskStartTimerInterval));
			}
        }
        
        /// <summary>
        /// Создает новую таску
        /// </summary>
        /// <param name="queue"></param>
        private void StartTask(OMQueue queue)
        {
            if (!WorkerCommon.ProcessTypeCache.ContainsKey(queue.ProcessTypeId))
            {
                LogError(queue, "Не найден тип процесса с ИД: " + queue.ProcessTypeId);
                return;
            }

            OMProcessType processType = WorkerCommon.ProcessTypeCache[queue.ProcessTypeId];

            ILongProcess longProcess;

            try
            {
                longProcess = WorkerCommon.GetLongProcessObject(queue.ProcessTypeId);
            }
            catch (Exception ex)
            {
                LogError(queue, ex.Message);
                return;
            }
            
            if (longProcess == null)
            {
                LogError(queue, "Не удалось создать объект типа: " + processType.ClassName);
                return;
            }

            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

            queue.StartDate = DateTime.Now;
            queue.LastCheckDate = DateTime.Now;
            queue.Status_Code = Status.Running;
            queue.Save();

            Task task = new Task(() =>
            {
                try
                {
                    _workerData.СurrentTasks[queue.Id].CurrentThread = Thread.CurrentThread;

                    // Так как данный процесс запускается в отдельном потоке в него необходимо передать данные об авторизованном пользователе,
                    // иначе работа будет выполняться под учетной записью пула приложений
                    if (queue.UserId > 0 && SRDCache.Users.ContainsKey((int)queue.UserId.Value))
                    {
                        SRDSession.SetThreadCurrentPrincipal(queue.UserId.Value);
                    }

					queue.CleanPropertyChangedList();

					longProcess.StartProcess(processType, queue, cancelTokenSource.Token);

                    processType.LastStartDate = queue.StartDate;
                    processType.Save();

                    queue.EndDate = DateTime.Now;
                    queue.LastCheckDate = DateTime.Now;
                    queue.Status_Code = Status.Completed;
                    queue.Save();

                    _workerData.СurrentTasks.Remove(queue.Id);
                }
                catch (Exception ex)
                {
                    _workerData.СurrentTasks.Remove(queue.Id);

                    int errorId = ErrorManager.LogError(ex);

                    queue.EndDate = DateTime.Now;
                    queue.Status_Code = Status.Faulted;
                    queue.ErrorId = errorId;

                    var message = ex.Message;
                    if (message.Length >= 4000) message = message.Substring(0, 4000);

                    queue.Message = message;
                    queue.Save();

                    try
                    {
                        longProcess.LogError(queue.ObjectId, ex, errorId);
                    }
                    catch (Exception) { }
                }
            }, cancelTokenSource.Token);

            TaskData taskData = new TaskData
            {
                Task = task,
                CancellationTokenSource = cancelTokenSource,
                LongProcess = longProcess,
                ProcessTypeId = processType.Id
            };

            _workerData.СurrentTasks.Add(queue.Id, taskData);

            taskData.Task.Start();
        }

        private static void LogError(OMQueue queue, string message)
        {
            queue.StartDate = DateTime.Now;
            queue.EndDate = DateTime.Now;
            queue.Status_Code = Status.Faulted;
            queue.Message = message;
            queue.Save();
        }

		/// <summary>
		/// Проверяет возможность запуска для типа процесса
		/// </summary>
		/// <param name="processType"></param>
		/// <returns></returns>
		private bool IsMaxRunningTasks(OMProcessType processType, List<long> currentAddedProcessTypes = null)
		{
			if (_workerData.Configuration.ProcessTypes == null ||
				_workerData.Configuration.ProcessTypes.Count == 0)
            {
                return _workerData.СurrentTasks.Count >= _workerData.MaxRunningTasks;
            }

            if (processType == null
                || processType.ProcessName.IsNullOrEmpty())
            {
				throw new Exception("Не передано описание типа процесса или имя типа процесса не заполнено");
            }
			
			ProcessTypeConfiguration processTypeConfiguration = _workerData.Configuration.GetProcessType(processType.ProcessName);

			if (processTypeConfiguration == null)
			{
				// Конфигурация для оставшихся типов процессов
				processTypeConfiguration = _workerData.Configuration.GetProcessType("*");
			}

			if (processTypeConfiguration == null)
            {
				// Если в конфигурации нет данного процесса, то игнорируем его
				return true;
            }

            List<string> keys;

			if (processTypeConfiguration.Name.Contains(','))
            {
				keys = processTypeConfiguration.Name.Split(',').Where(x => x.IsNotEmpty()).Distinct().ToList();
            }
			else if(processTypeConfiguration.Name == "*")
			{
				//Получаем все несконфигурированые процессы
				List<string> allConfiguredProcesses = new List<string>();

				foreach(var processConfig in _workerData.Configuration.ProcessTypes)
				{
					allConfiguredProcesses.AddRange(processConfig.Name.Split(",").Distinct());
				}

				List<string> keysToExclude = new List<string>();

				if (processTypeConfiguration.ExcludeName.IsNotEmpty())
				{
					keysToExclude = processTypeConfiguration.ExcludeName.Split(",").Distinct().Where(x => x.IsNotEmpty()).ToList();
					
					if(keysToExclude.Contains(processType.ProcessName))
					{
						return true;
					}
				}
				
				keys = WorkerCommon.ProcessTypeCache.Values.Where(x => !allConfiguredProcesses.Contains(x.ProcessName) && !keysToExclude.Contains(x.ProcessName))
					.Select(x => x.ProcessName).ToList();
			}
            else
            {
                keys = new List<string>() { processType.ProcessName };
            }

            List<long> processTypeIds = WorkerCommon.ProcessTypeCache.Values
                .Where(x => keys.Contains(x.ProcessName))
                .Select(x => x.Id)
                .Distinct()
                .ToList();

            int currentTasksCount = (currentAddedProcessTypes != null ? currentAddedProcessTypes.Where(x => processTypeIds.Contains(x)).Count() : 0) +
                _workerData.СurrentTasks.Values.Where(x => processTypeIds.Contains(x.ProcessTypeId)).Count();

            return currentTasksCount >= processTypeConfiguration.MaxRunningTasks;
        }
    }
}
