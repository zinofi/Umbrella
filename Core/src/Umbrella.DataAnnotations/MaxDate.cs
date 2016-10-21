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
        /// Maximum date based on the exact date specified
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        public MaxDateAttribute(int year, int month, int day)
        {
            m_MaxDate = new DateTime(year, month, day);
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientValidationParameters()
        {
            return base.GetClientValidationParameters()
                .Union(new[] { new KeyValuePair<string, object>("Max", GetMaxDate().ToShortDateString()) });
        }

        public override bool IsValid(object value, object container)
        {
            DateTime result;

            DateTime maxDate = GetMaxDate();

            if (value is DateTime)
            {
                result = (DateTime)value;

                if (result.Date > maxDate)
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
                    if (DateTime.TryParse(strDateTime, out result) && result.Date > maxDate)
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

            if (m_MaxDate.HasValue)
                maxDate = m_MaxDate.Value;
            else if (m_OffSetDays.HasValue)
                maxDate = DateTime.UtcNow.AddDays(m_OffSetDays.Value);
            else
                maxDate = DateTime.UtcNow.Date;

            return maxDate;
        }
    }
}