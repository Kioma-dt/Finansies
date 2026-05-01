using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuisnessLogic.Enums;

namespace BuisnessLogic.Shared
{
    public static class DateTimeExtender
    {
        public static DateTime AddTransactionPeriodicity(this DateTime date, TransactionPeriodicity period)
        {
            var tempDate = date;
            tempDate = period switch
            {
                TransactionPeriodicity.Daily => tempDate.AddDays(1),
                TransactionPeriodicity.Monthly => tempDate.AddMonths(1),
                TransactionPeriodicity.Yearly => tempDate.AddYears(1),
                _ => tempDate,
            };

            return tempDate;
        }
    }
}
