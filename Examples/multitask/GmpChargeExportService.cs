using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Core.ErrorManagment;
using Core.Shared.Extensions;
using ObjectModel.Directory;
using ObjectModel.Finance.Gmp;
using ObjectModel.Cld;
using GmpIntegration.Helpers;
using ObjectModel.Finance.Flses;
using Core.SRD.DAL;
using ObjectModel.SRD;

namespace GmpIntegration.Services
{
    public abstract class GmpChargeExportService : GmpBaseService
    {
        private const string GMP_RESULT_OK = "0";
        private CancellationToken _cancellationToken;

        protected override string ServiceName { get { return "Экспорт начислений"; } }

        protected GmpChargeExportService(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
        }

        public void ChargeExport()
        {
            try
            {
                //цикл для новых начислений, аннулирований
                while (true)
                {
                    List<OMRegister> registerToGmpList = GetGmpRegisters();
                    
                    if (registerToGmpList.Count == 0)
                        break;

                    CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
                    ParallelOptions options = new ParallelOptions { CancellationToken = cancelTokenSource.Token, MaxDegreeOfParallelism = 1 };

                    Parallel.ForEach(registerToGmpList, options, registerToGmp =>
                    {
                        using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required))
                        {
                            registerToGmp.LockDbRow();

                            OMRegister registerToGmpToCheck = null;
                            OMSendProgress sendProgress = null;
                            string gmpMessageId = null;
                            try
                            {
                                registerToGmpToCheck = OMRegister.Where(x => x.EmpId == registerToGmp.EmpId)
                                    .Execute()
                                    .FirstOrDefault();

                                if (registerToGmpToCheck == null)
                                {
                                    return;
                                }
                                

                                GetJournalAndSendGmp(registerToGmp, ref gmpMessageId);

                                if (!gmpMessageId.IsNullOrEmpty())
                                {
                                    sendProgress = OMSendProgress.Where(x => x.GmpPackageId == gmpMessageId).ExecuteFirstOrDefault();
                                }
                                
                                registerToGmp.DateToGmp = DateTime.Now;
                                registerToGmp.ProcessStatus_Code = ProcessStatus.SentToGmp;
                                registerToGmp.Save();
                                ChangeBlockedByGmp(registerToGmp.EmpId);
                                
                                SRDAudit.Add(SRDCoreFunctions.MIOMO_GISGMP_FORCED_ACKNOW, true, "Передача начисления", OMRegister.GetRegisterId(), 
                                    registerToGmpToCheck.EmpId, sendProgress?.EmpId, OMRegister.GetRegisterId(), registerToGmpToCheck.EmpId );
                            }
                            catch (Exception ex)
                            {
                                int errorId = ErrorManager.LogError(ex);

                                registerToGmp.ProcessStatus_Code = ProcessStatus.SendError;
                                registerToGmp.ErrorId = errorId;
                                registerToGmp.Message = ex.Message.Truncate(1024);
                                registerToGmp.Save();
                                ChangeBlockedByGmp(registerToGmp.EmpId);
                                
                                if (!gmpMessageId.IsNullOrEmpty())
                                {
                                    sendProgress = OMSendProgress.Where(x => x.GmpPackageId == gmpMessageId).ExecuteFirstOrDefault();
                                    sendProgress.GmpPackageResultCode = "-100";
                                    sendProgress.GmpPackageResultDescription = "Техническая ошибка.";
                                    sendProgress.Save();
                                }

                                SRDAudit.Add(SRDCoreFunctions.MIOMO_GISGMP_FORCED_ACKNOW, false, $"Передача начисления (ErrorId = {errorId})", 
                                    OMRegister.GetRegisterId(), registerToGmpToCheck?.EmpId, sendProgress?.EmpId, OMRegister.GetRegisterId(), registerToGmpToCheck?.EmpId);

                            }

                            ts.Complete();
                        }
                    });
                }

            }
            catch (Exception ex)
            {
                ErrorManager.LogError(ex);
                throw;
            }
        }

        private List<OMRegister> GetGmpRegisters()
        {
            List<OMRegister> result = new List<OMRegister>();

            List<OMRegister> newItems = OMRegister.Where(x => x.ProcessStatus_Code ==  ProcessStatus.Signed  && x.NachStatus_Code ==NachStatus.New)
                .SelectAll(false)
                .SetPackageSize(1000)
                .Execute();

            result.AddRange(newItems);

            List<OMRegister> canceledItems = OMRegister.Where(x => x.ProcessStatus_Code == ProcessStatus.Signed && x.NachStatus_Code == NachStatus.Cancel)
                .SelectAll(false)
                .SetPackageSize(1000)
                .Execute();

            result.AddRange(canceledItems);
            
            return result;
        }

        #region Методы создания и отправки в сервис
        protected abstract string GeneratePayerIdentifier(OMConrtactor conrtactor);

        protected abstract string SendToGmpService(OMJournalNach oMGmpJournalNach, ref string messageId);

        protected abstract string SendRefundToGmpService(OMJournalNach oMGmpJournalNach, ref string messageId);

        private string GetJournalAndSendGmp(OMRegister registerToGmp, ref string messageId)
        {
            OMJournalNach oMGmpJournalNach = OMJournalNach.Where(x => x.FmRegistrToGmp == registerToGmp.EmpId).SelectAll().ExecuteFirstOrDefault();
            if (oMGmpJournalNach == null)
            {
                oMGmpJournalNach = CreateJournalNachGmp(registerToGmp);
            }

            //Отправление в ИС ГМП
            string GmpPackageId = SendToGmpService(oMGmpJournalNach, ref messageId);
            return GmpPackageId;

        }

        #endregion    
    }
}
