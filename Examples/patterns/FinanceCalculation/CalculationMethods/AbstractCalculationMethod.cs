using Miomo.Dal.FinanceCalculation.Dto;
using ObjectModel.Directory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miomo.Dal.FinanceCalculation.CalculationMethods
{
    public abstract class AbstractCalculationMethod
    {
        /// <summary>
        /// Тип метода
        /// </summary>
        public abstract CalcMethod MethodType { get; }

        /// <summary>
        /// Возвращает используемые в формуле типы коэффициентов
        /// </summary>
        /// <returns></returns>
        public abstract List<CoefType> GetUsedCoefType(MethodConditions conditions);
        
        /// <summary>
        /// Возвращает формулу
        /// </summary>
        /// <param name="methodId"></param>
        /// <param name="useDopCoef"></param>
        /// <param name="useMaxFunction"></param>
        /// <returns></returns>
        public abstract string GetFormula(bool useMaxFunction = false);
        
        /// <summary>
        /// Возвращает рассчитанный по формуле ГАП
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        public abstract decimal GetCalculatedGap(CalculationDetails details);
        
        /// <summary>
        /// Возвращает значение коэф нужного типа из списка
        /// </summary>
        /// <param name="type"></param>
        /// <param name="coefs"></param>
        /// <returns></returns>
        protected decimal GetValueForCoefType(CoefType type, List<ChosenCoefModel> coefs, bool value2 = false)
        {
            if (coefs == null) return 1;
            var coef = coefs.FirstOrDefault(x => x.CoefType_Code == type);
            if (coef == null || coef.Id == -1)
                return 1;
            else return value2 ? (coef.Value2 ?? 1) : (coef.Value ?? 1);
        }
    }
}
