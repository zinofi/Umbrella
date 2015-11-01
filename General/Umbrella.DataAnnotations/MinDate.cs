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
        private DateTime m_MinDate;

        /// <summary>
        /// Minimum date defaults to the current date
        /// </summary>
        public MinDateAttribute()
        {
            m_MinDate = DateTime.Now.Date;
        }

        /// <summary>
        /// Minimum date based on the offset days from the current date
        /// </summary>
        /// <param name="offset"></param>
        public MinDateAttribute(int offset)
        {
            m_MinDate = DateTime.Now.AddDays(offset);
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
                .Union(new[] { new KeyValuePair<string, object>("Min", m_MinDate.ToShortDateString()) });
        }

        public override bool IsValid(object value, object container)
        {
            string strDateTime = value as string;

            if (!string.IsNullOrEmpty(strDateTime))
            {
                //We have a value - try and parse it to a datetime
                DateTime result;
                if (DateTime.TryParse(strDateTime, out result) && result.Date < m_MinDate)
                {
                    return false;
                }
            }

            //Assume success
            return true;
        }
    }
}
