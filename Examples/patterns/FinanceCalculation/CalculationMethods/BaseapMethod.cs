using Miomo.Dal.FinanceCalculation.Dto;
using ObjectModel.Directory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miomo.Dal.FinanceCalculation.CalculationMethods
{
    public class BaseapMethod : AbstractCalculationMethod
    {
        public override CalcMethod MethodType { get { return CalcMethod.BaseAp; } }
        
        public override decimal GetCalculatedGap(CalculationDetails details)
        {
            decimal res = GetValueForCoefType(CoefType.Vri, details.Coefs) * GetValueForCoefType(CoefType.Correct, details.Coefs) * GetValueForCoefType(CoefType.Location, details.Coefs)
                           * (details.SCoef ?? 0) * GetValueForCoefType(CoefType.DopCoef, details.Coefs);
            if (details.UseMax ?? false)
            {
                res *= Math.Max(GetValueForCoefType(CoefType.BaseAp, details.Coefs) * GetValueForCoefType(CoefType.K1, details.Coefs),
                    GetValueForCoefType(CoefType.K1, details.Coefs,true));
            }
            else
            {
                res *= GetValueForCoefType(CoefType.BaseAp, details.Coefs);
            }
            return res;
        }

        public override string GetFormula(bool useMaxFunction = false)
        {
            if (useMaxFunction)
                return "MAX(Аб*K1,К2) x Кд x Пкд x Км x S";
            else
                return "К1 х Аб x Кд x Пкд x Км x S";
        }

        public override List<CoefType> GetUsedCoefType(MethodConditions conditions)
        {
            var allTypes = new List<CoefType>() { CoefType.BaseAp, CoefType.Vri, CoefType.Correct, CoefType.Location, CoefType.K1 };

            if (!conditions.UseAll)
            {
                if (!conditions.UseMaxFunction)
                {
                    allTypes.Remove(CoefType.K1);
                }
            }
            return allTypes;
        }
    }
}
