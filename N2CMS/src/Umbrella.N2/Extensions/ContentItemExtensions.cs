using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using N2;
using System.ComponentModel;
using Umbrella.N2.CustomProperties.LinkEditor;
using System.Diagnostics;
using System.Reflection;
using Umbrella.Utilities.Extensions;
using System.Runtime.CompilerServices;

namespace Umbrella.N2.Extensions
{
    public static class ContentItemExtensions
    {
        private static readonly Func<object, bool> IsNullOrEmpty = x => x == null || x is string && string.IsNullOrEmpty((string)(x));

        #region Public Static Methods
        public static TProperty GetPropertyValue<TContentItem, TProperty>(this TContentItem contentItem, Expression<Func<TContentItem, TProperty>> expression) where TContentItem : ContentItem
        {
            MemberExpression memberExpression = GetMemberExpression(expression);

            object value = contentItem.GetDetail(memberExpression.Member.Name);

            return ConvertToRequestedType<TProperty>(value);
        }

        public static LinkItemCollection GetPropertyValue<TContentItem>(this TContentItem contentItem, Expression<Func<TContentItem, LinkItemCollection>> expression)
            where TContentItem : ContentItem
        {
            MemberExpression memberExpression = GetMemberExpression(expression);

            return LinkItemCollection.FindByPageAndPropertyName(contentItem, memberExpression.Member.Name);
        }

        public static void SetPropertyValue<TContentItem>(this TContentItem contentItem, Expression<Func<TContentItem, object>> expression, object value) where TContentItem : ContentItem
        {
            MemberExpression memberExpression = null;
            if (expression.Body is MemberExpression)
                memberExpression = (MemberExpression)expression.Body;
            else if (expression.Body is UnaryExpression)
                memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            if (memberExpression == null)
                throw new Exception("The body of the expression must be either a MemberExpression or a UnaryExpression.");

            if (value is LinkItemCollection)
            {
                //LinkItemCollection needs to be saved as a json string
                LinkItemCollection links = value as LinkItemCollection;
                contentItem.SetDetail(memberExpression.Member.Name, links.ToJSONString(), typeof(string));
            }
            else
            {
                contentItem.SetDetail(memberExpression.Member.Name, value, ((PropertyInfo)memberExpression.Member).PropertyType);
            }
        }

        public static T FindClosestParent<T>(this ContentItem item) where T : ContentItem
        {
            ContentItem parent = item;

            while (parent != null)
            {
                if (parent is T)
                {
                    return parent as T;
                }

                parent = parent.Parent;
            }

            return null;
        }

        

        public static TProperty GetDynamicProperty<TContentItem, TProperty>(this TContentItem contentItem, Expression<Func<TContentItem, TProperty>> action)
            where TContentItem : ContentItem
        {
            TProperty value = contentItem.GetPropertyValue(action);

            //Check if we are in display mode or are editing the page - we dont want the editor to pick up inherited values
            if (!Context.Current.RequestContext.Url.Path.StartsWith(Context.Current.ManagementPaths.GetManagementInterfaceUrl()))
            {
                if (IsNullOrEmpty(value) && contentItem.Parent != null && contentItem.Parent is TContentItem)
                {
                    value = action.Compile()(contentItem.Parent as TContentItem);
                }
            }

            return value;
        }

        public static ContentItem HasInheritedDynamicValue(this ContentItem contentItem, string name)
        {
            ContentItem retVal = contentItem;

            object value = contentItem.GetDetail(name);

            //Check if we are in display mode or are editing the page - we dont want the editor to pick up inherited values
            if (IsNullOrEmpty(value) && contentItem.Parent != null)
            {
                retVal = HasInheritedDynamicValue(contentItem.Parent, name);
            }

            if (retVal != null && Context.UrlParser.IsRootOrStartPage(retVal) && retVal != Context.UrlParser.StartPage)
                return null;

            return retVal;
        }
        #endregion

        #region Private Static Members
        private static MemberExpression GetMemberExpression<TPageData, TProperty>(Expression<Func<TPageData, TProperty>> expression)
        {
            MemberExpression memberExpression = null;
            if (expression.Body is MemberExpression)
                memberExpression = (MemberExpression)expression.Body;
            else if (expression.Body is UnaryExpression)
                memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            else if (expression.Body is MethodCallExpression mce)
            {
                if (mce.Arguments.Count > 0 && mce.Arguments[0] is MemberExpression)
                    memberExpression = (MemberExpression)mce.Arguments[0];
                else
                    memberExpression = (MemberExpression)mce.Object;
            }

            if (memberExpression == null)
                throw new Exception("The body of the expression must be either a MemberExpression or a UnaryExpression.");
            else
                return memberExpression;
        }

        private static TProperty ConvertToRequestedType<TProperty>(object value)
        {
            if (value != null)
                return (TProperty)value;
            if (typeof(TProperty) == typeof(bool))
                return default;
            if (!TypeExtensions.CanBeNull(typeof(TProperty)))
                throw new Exception("The property value is null and the requested type is a value type. \r\n Consider using nullable as type or make the property mandatory.");
            else
                return (TProperty)value;
        }
        #endregion
    }
}