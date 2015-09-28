using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Umbrella.Legacy.WebUtilities.Extensions
{
    public static class ControlExtensions
    {
        #region Control Finders
        /// <summary>
        /// Recursively searches Control hierarchy for matches for Predicate and returns it as T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="from"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T FindFirstControl<T>(this Control from, Predicate<T> predicate) where T : Control
        {
            if (from is T && predicate(from as T))
                return from as T;

            foreach (Control idx in from.Controls)
            {
                T tmpRetVal = idx.FindFirstControl<T>(predicate);
                if (tmpRetVal != null)
                    return tmpRetVal;
            }

            return null;
        }

        /// <summary>
        /// Recursively searches Control hierarchy for the first control that
        /// matches the type of T and returns it as T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="from"></param>
        /// <returns></returns>
        public static T FindFirstControl<T>(this Control from) where T : Control
        {
            return from.FindFirstControl<T>(x => x is T);
        }

        /// <summary>
        /// Recursively search Control hierarcy for ALL controls that
        /// matches the given Predicate and returns them as T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="from"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindControls<T>(this Control from, Predicate<T> predicate) where T : Control
        {
            if (from is T)
            {
                if (predicate(from as T))
                    yield return from as T;
            }

            foreach (Control idx in from.Controls)
            {
                foreach (T idxInner in idx.FindControls<T>(predicate))
                {
                    yield return idxInner;
                }
            }
        }

        /// <summary>
        /// Recursively search Control hierarcy for ALL controls that
        /// matches the type of T and returns them as T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="from"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindControls<T>(this Control from) where T : Control
        {
            return from.FindControls<T>(x => x is T);
        }
        #endregion

        #region Utilities
        public static string ToHtmlString(this Control ctrl)
        {
            return ctrl.ToStringBuilder().ToString();
        }

        public static StringBuilder ToStringBuilder(this Control ctrl)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.IO.StringWriter tw = new System.IO.StringWriter(sb);
            System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);
            ctrl.RenderControl(hw);
            return sb;
        }
        #endregion
    }
}
