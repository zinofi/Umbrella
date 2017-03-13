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
    public class MinDateAttribute : ModelAwareValidationAttribute
    {
        private DateTime? m_MinDate;
        private int? m_OffSetDays;

        /// <summary>
        /// Minimum date defaults to the current date
        /// </summary>
        public MinDateAttribute()
        {
        }

        /// <summary>
        /// Minimum date based on the offset days from the current date
        /// </summary>
        /// <param name="offsetDays"></param>
        public MinDateAttribute(int offsetDays)
        {
            m_OffSetDays = offsetDays;
        }

        /// <summary>
        /// Minimum date based on the exact date specified
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        public MinDateAttribute(int year, int month, int day)
        {
            m_MinDate = new DateTime(year, month, day);
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientValidationParameters()
        {
            return base.GetClientValidationParameters()
                .Union(new[] { new KeyValuePair<string, object>("Min", GetMinDate().ToString("d")) });
        }

        public override bool IsValid(object value, object container)
        {
            DateTime result;

            DateTime minDate = GetMinDate();

            if (value is DateTime)
            {
                result = (DateTime)value;

                if (result.Date < minDate)
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
                    if (DateTime.TryParse(strDateTime, out result) && result.Date < minDate)
                    {
                        return false;
                    }
                }
            }

            //Assume success
            return true;
        }

        private DateTime GetMinDate()
        {
            DateTime minDate;

            if (m_MinDate.HasValue)
                minDate = m_MinDate.Value;
            else if (m_OffSetDays.HasValue)
                minDate = DateTime.UtcNow.AddDays(m_OffSetDays.Value);
            else
                minDate = DateTime.UtcNow.Date;

            return minDate;
        }
    }
}
