using System;
using System.Collections.Generic;

namespace Valuation.Common
{
    public static class DateTimeExtensions
    {
        public static IEnumerable<DateTime> GetDateRange(this DateTime fromDate, DateTime toDate)
        {
            var minDate = fromDate > toDate ? toDate : fromDate;
            var maxDate = minDate == fromDate ? toDate : fromDate;

            while(minDate <= maxDate)
            {
                yield return minDate;
                minDate = minDate.AddDays(1);
            }
        }

        public static bool IsWeekend(this DateTime day)
        {
            return day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}
