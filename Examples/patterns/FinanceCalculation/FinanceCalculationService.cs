using Core.Shared.Extensions;
using GemBox.Spreadsheet;
using Miomo.Dal.FinanceCalculation.CalculationMethods;
using Miomo.Dal.FinanceCalculation.Dto;
using Miomo.Dal.PaymentsImport;
using ObjectModel.Directory;
using ObjectModel.Finance;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Miomo.Dal.FinanceCalculation
{
    public class FinanceCalculationService
    {
        /// <summary>
        /// Список двойных коэфициентов
        /// </summary>
        public List<CoefType> DoubleCoefs = new List<CoefType>() { CoefType.K1 };

        /// <summary>
        /// Список реализаций методов расчета
        /// </summary>
        public List<AbstractCalculationMethod> CalcMethods = new List<AbstractCalculationMethod>() {
            new BaseapMethod(),
            new CadastrMethod(),
            new BaseAp1Method(),
            new BaseAp2CasesMethod(),
            new BaseAp2Method(),
            new CultureObjectMethod(),
            new SpecialZonesMethod(),
            new SpecialZonesZUMethod(),
        };

        /// <summary>
        /// Возвращает рассчитанную ГАП
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        public decimal GetCalculatedGap(CalculationDetails details)
        {
            decimal res = 0;
            OMCalcMethod method = OMCalcMethod.Where(x => x.Id == details.MethodId).SelectAll().ExecuteFirstOrDefault();
            if (method != null)
            {
                var implement = GetImplementation(method.MethodType_Code);
                if (implement == null) return res;
                return implement.GetCalculatedGap(details);
            }
            return res;
        }

        /// <summary>
        /// Возвращает формулу
        /// </summary>
        /// <param name="methodId"></param>
        /// <param name="method"></param>
        /// <param name="useMax"></param>
        /// <param name="useDopCoef"></param>
        /// <returns></returns>
        public string GetFormula(MethodConditions conditions)
        {
            if (conditions.MethodId.HasValue)
            {
                OMCalcMethod met = OMCalcMethod.Where(x => x.Id == conditions.MethodId).SelectAll().ExecuteFirstOrDefault();
                conditions.MethodType = met.MethodType_Code;
                conditions.UseDopCoef = met.UseDopCoef??false;
            }

            string res = "";
            var implement = GetImplementation(conditions.MethodType);
            if (implement == null) return res;

            res = implement.GetFormula(conditions.UseMaxFunction);
            if (conditions.UseDopCoef)
            {
                res += " x Kдоп";
            }
            return res;
        }

        /// <summary>
        /// Возвращает реализацию алгоритма по заданному методу расчета
        /// </summary>
        /// <param name="methodType"></param>
        /// <returns></returns>
        private AbstractCalculationMethod GetImplementation(CalcMethod methodType)
        {
            return CalcMethods.Where(x => x.MethodType == methodType).FirstOrDefault();
        }

        /// <summary>
        /// Возвращает используемые в формуле типы коэффициентов
        /// </summary>
        /// <returns></returns>
        public List<CoefType> GetUsedCoefType(MethodConditions conditions)
        {
            var allTypes = new List<CoefType>();

            var implement = GetImplementation(conditions.MethodType);
            if (implement == null) return allTypes;

            allTypes = implement.GetUsedCoefType(conditions);

            if (conditions.UseAll || conditions.UseDopCoef)
            {
                allTypes.Add(CoefType.DopCoef);
            }

            if (!conditions.GetAllUsed)
            {
                if (conditions.IsDouble)
                {
                    allTypes = allTypes.Where(x => DoubleCoefs.Contains(x)).ToList();
                }
                else
                {
                    allTypes = allTypes.Where(x => !DoubleCoefs.Contains(x)).ToList();
                }
            }

            return allTypes;
        }

        /// <summary>
        /// Возвращает список методов расчета по типу объекта
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public List<CalcMethod> GetCalcMethods(MethodObjectType objectType)
        {
            var allTypes = Enum.GetValues(typeof(CalcMethod)).Cast<CalcMethod>().ToList();
            switch (objectType)
            {
                case MethodObjectType.ZU:
                    allTypes = new List<CalcMethod>() { CalcMethod.BaseAp, CalcMethod.CadastrCost, CalcMethod.SpecialZonesZU };
                    break;
                case MethodObjectType.Property:
                    allTypes = new List<CalcMethod>() { CalcMethod.BaseAp, CalcMethod.CadastrCost, CalcMethod.CultureObject, CalcMethod.SpecialZones };
                    break;
            }
            return allTypes;
        }
        
        /// <summary>
        /// Возвращает обозначение коэффициента
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetCoefTypeMark(CoefType type)
        {
            return type.GetShortTitle();
        }

        /// <summary>
        /// Возвращает список используемых в формуле конкретного расчета коэффициентов
        /// </summary>
        /// <param name="calcId"></param>
        /// <param name="methodId"></param>
        /// <returns></returns>
        public List<ChosenCoefModel> GetChosenCoefsList(MethodConditions conditions)
        {
            OMCalcMethod method = OMCalcMethod.Where(x => x.Id == conditions.MethodId).SelectAll().ExecuteFirstOrDefault();
            //var coefTypes = GetCoefTypes(method.MethodType_Code, useMax, method.UseDopCoef ?? false, isDouble, false);
            conditions.MethodType = method.MethodType_Code;
            conditions.UseDopCoef = method.UseDopCoef ?? false;
            var coefTypes = GetUsedCoefType(conditions);

            var chosenCoefGrid = new List<ChosenCoefModel>();
            var coefIds = OMCalcCoefConnection.Where(x => x.CalcId == conditions.CalcId).SelectAll().Execute().Select(x => x.CoefId).ToList();
            foreach (var coefType in coefTypes)
            {
                OMCalcCoefs coef = null;
                if (coefIds.Count > 0)
                    coef = OMCalcCoefs.Where(x => x.CoefType_Code == coefType && coefIds.Contains(x.Id)).SelectAll().ExecuteFirstOrDefault();
                if (conditions.CalcId == -1 || coef == null)
                {
                    chosenCoefGrid.Add(new ChosenCoefModel()
                    {
                        Id = -1,
                        Name = "",
                        Value = 1,
                        Value2 = 1,
                        CoefType_Code = coefType,
                        CoefType = coefType.GetEnumDescription(),
                        Mark = GetCoefTypeMark(coefType)
                    });
                }
                else
                {
                    chosenCoefGrid.Add(new ChosenCoefModel()
                    {
                        Id = coef.Id,
                        Name = coef.Name,
                        Value = coef.Value,
                        Value2 = coef.Value2,
                        CoefType_Code = coefType,
                        CoefType = coefType.GetEnumDescription(),
                        Mark = GetCoefTypeMark(coefType),
                        ActualS= coef.ActualS,
                        ActualPo = coef.ActualPo
                    });
                }
            }
            return chosenCoefGrid;
        }
    }
}
