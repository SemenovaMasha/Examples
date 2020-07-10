using ObjectModel.Directory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Miomo.Dal.FinanceCalculation.Dto
{
    public class MethodConditions
    {
        /// <summary>
        /// Тип метода расчета
        /// </summary>
        public CalcMethod MethodType { get; set; }

        /// <summary>
        /// Использовать ли max функцию 
        /// </summary>
        public bool UseMaxFunction { get; set; } = false;

        /// <summary>
        /// Использовать ли доп коэф
        /// </summary>
        public bool UseDopCoef { get; set; } = false;

        /// <summary>
        /// Вернуть ли все коэффициенты (без разделения на одинарные и двойные), ВСЕ коэффициенты возвращаются только в случае создания/редактирования метода. 
        /// Когда пользователь создает расчет - UseAll = false
        /// например если UseAll = true и пользователь поставил галочку использовать max, то добавляется дополнительный двойной коэффициент,
        /// </summary>
        public bool UseAll { get; set; } = true;

        /// <summary>
        /// Если использовать разделение (UseAll = false), указывает какой тип коэф вернуть
        /// При создании/редактировании метода, или расчета, нужно для разделения обычных и двойных коэффициентов на разные таблицы
        /// </summary>
        public bool IsDouble { get; set; } = false;

        /// <summary>
        /// id метода в бд
        /// </summary>
        public long? MethodId { get; set; }

        /// <summary>
        /// id расчета в бд
        /// </summary>
        public long? CalcId { get; set; }

        /// <summary>
        /// Если true - вернуть все коэффициенты, используемые в формуле (без разделения на одинарные и двойные).
        /// Так как в формах метода и расчета коэффициенты делятся на разные таблицы (одинарные и двойные), 
        /// этот признак позволяет получить список без разделения (например для вывода всей формулы готового расчета)
        /// </summary>
        public bool GetAllUsed { get; set; } = false;
    }
}
