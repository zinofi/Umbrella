using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.DataAnnotations.BaseClasses;

namespace Umbrella.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxDateAttribute : ModelAwareValidationAttribute
    {
        private DateTime? m_MaxDate;
        private int? m_OffSetDays;

        /// <summary>
        /// Maximum date defaults to the current date
        /// </summary>
        public MaxDateAttribute()
        {
        }

        /// <summary>
        /// Maximum date based on the offset days from the current date
        /// </summary>
        /// <param name="offsetDays"></param>
        public MaxDateAttribute(int offsetDays)
        {
            m_OffSetDays = offsetDays;
        }

        /// <summary>
        /// Maximum date based on the exact date specified at the end of the day, i.e. 23:59:59
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        public MaxDateAttribute(int year, int month, int day)
        {
            m_MaxDate = new DateTime(year, month, day, 23, 59, 59, DateTimeKind.Utc);
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientValidationParameters()
        {
            return base.GetClientValidationParameters()
                .Union(new[] { new KeyValuePair<string, object>("Max", GetMaxDate().ToString("d")) });
        }

        public override bool IsValid(object value, object container)
        {
            DateTime result;

            DateTime maxDate = GetMaxDate();

            if (value is DateTime)
            {
                result = (DateTime)value;

                if (result.ToUniversalTime() > maxDate)
                {
                    return false;
                }
            }
            else
            {
                string strDateTime = value as string;

                if (!string.IsNullOrEmpty(strDateTime))
                {
                    //We have a value - try and parse it to a datetime
                    if (DateTime.TryParse(strDateTime, out result) && result.ToUniversalTime() > maxDate)
                    {
                        return false;
                    }
                }
            }

            //Assume success
            return true;
        }

        private DateTime GetMaxDate()
        {
            DateTime maxDate;

            var now = DateTime.UtcNow;

            if (m_MaxDate.HasValue)
                maxDate = m_MaxDate.Value;
            else if (m_OffSetDays.HasValue)
                maxDate = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, DateTimeKind.Utc).AddDays(m_OffSetDays.Value);
            else
                maxDate = now;

            return maxDate;
        }
    }
}