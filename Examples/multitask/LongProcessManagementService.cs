using System.Collections.Generic;
using System.Threading;
using Core.CacheManagement;
using Core.Shared.Extensions;
using System.Net;
using System;
using Core.SRD;
using ObjectModel.Core.LongProcess;
using System.Linq;
using Core.ErrorManagment;
using System.Threading.Tasks;

namespace Core.Register.LongProcessManagment
{
    public class LongProcessManagementService
    {
        private long _applicationId;

        private readonly CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();

        private readonly List<Task> _workers = new List<Task>();
		
		public void Start()
        {
			try
            {
                _applicationId = Log();

                WorkerCommon workerData = new WorkerCommon(_applicationId);
				
				Task checkerTask = Task.Factory.StartNew(() =>
                {
					try
					{
						WorkerChecker statusChecker = new WorkerChecker(workerData, _cancelTokenSource.Token);
						statusChecker.Start();
					}
					catch(Exception ex)
					{
						ErrorManager.LogError(ex);
					}
                }, _cancelTokenSource.Token);
                _workers.Add(checkerTask);

                Task schedulerTask = Task.Factory.StartNew(() =>
                {
					try
					{
						WorkerScheduler scheduler = new WorkerScheduler(workerData, _cancelTokenSource.Token);
						scheduler.Start();
					}
					catch (Exception ex)
					{
						ErrorManager.LogError(ex);
					}
                }, _cancelTokenSource.Token);
                _workers.Add(schedulerTask);

                Task starterTask = Task.Factory.StartNew(() =>
                {
					try
					{
						WorkerStarter starter = new WorkerStarter(workerData, _cancelTokenSource.Token);
						starter.Start();
					}
					catch (Exception ex)
					{
						ErrorManager.LogError(ex);
					}
                }, _cancelTokenSource.Token);
                _workers.Add(starterTask);
            }
            catch (Exception ex)
            {
                ErrorManager.LogError(ex);
            }
        }

        public void Stop()
        {
            _cancelTokenSource.Cancel();
            Log(_applicationId, OMLogStatus.Stopped);

            Task.WaitAll(_workers.ToArray());
            CacheUpdateManager.Close();
        }

        private static string GetEnvironmentData()
        {
            List<string> environmentData = new List<string>();

            environmentData.Add("Базовый каталог домена: " + AppDomain.CurrentDomain.BaseDirectory);

            string strHostName = Dns.GetHostName();

            if (strHostName.IsNotEmpty())
            {
                IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);

                environmentData.Add("DNS-имя узла: " + ipEntry.HostName);

                if (ipEntry.AddressList != null)
                {
                    for (int i = 0; i < ipEntry.AddressList.Length; i++)
                    {
                        environmentData.Add(String.Format("IP-адрес {0}: {1} ", i, ipEntry.AddressList[i]));
                    }
                }
            }

            return String.Join(Environment.NewLine, environmentData);
        }

        private static long Log(long? applicationId = null, OMLogStatus status = OMLogStatus.Running)
        {
            string exeInfo = GetEnvironmentData();

            OMLog serviceLog = null;

            if (applicationId.HasValue)
            {
                serviceLog = OMLog.Where(x => x.Id == applicationId)
                    .SelectAll()
                    .Execute()
                    .FirstOrDefault();
            }

            if (serviceLog == null)
            {
                serviceLog = new OMLog();
            }

            serviceLog.LastCheckDate = DateTime.Now;
            serviceLog.ExeInfo = exeInfo;
            serviceLog.StartDate = DateTime.Now;
            serviceLog.Status = (long)status;
            serviceLog.UserId = SRDSession.GetCurrentUserId();

            serviceLog.Save();

            return serviceLog.Id;
        }
    }
}
