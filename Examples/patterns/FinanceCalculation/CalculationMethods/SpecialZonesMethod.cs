using System;
using System.Collections.Generic;
using System.Text;
using Miomo.Dal.FinanceCalculation.Dto;
using ObjectModel.Directory;

namespace Miomo.Dal.FinanceCalculation.CalculationMethods
{
    public class SpecialZonesMethod : AbstractCalculationMethod
    {
        public override CalcMethod MethodType { get { return CalcMethod.SpecialZones; } }
        
        public override decimal GetCalculatedGap(CalculationDetails details)
        {
            var res = (details.SCoef ?? 0) * (details.BaseAP ?? 0) * GetValueForCoefType(CoefType.SpecialEco7, details.Coefs) *
                             GetValueForCoefType(CoefType.DopCoef, details.Coefs);
            return res;
        }

        public override string GetFormula(bool useMaxFunction = false)
        {
            return "S x Cp х Ni";
        }

        public override List<CoefType> GetUsedCoefType(MethodConditions conditions)
        {
            return new List<CoefType>() { CoefType.SpecialEco7 };
        }
    }
}
