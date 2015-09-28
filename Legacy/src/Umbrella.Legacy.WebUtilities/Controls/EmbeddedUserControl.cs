using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.IO;
using System.Reflection;

namespace Umbrella.Legacy.WebUtilities.Controls
{
    public class EmbeddedUserControl : UserControl
    {
        #region Private Static Members
        private static Dictionary<string, List<FieldInfo>> s_CachedControlFieldsDictionary;
        #endregion

        #region Delegates
        protected Action OnFrameworkInitialized;
        #endregion

        #region Static Constructor
        static EmbeddedUserControl()
        {
            s_CachedControlFieldsDictionary = new Dictionary<string, List<FieldInfo>>();
        }
        #endregion

        #region Overridden Methods
        protected override void FrameworkInitialize()
        {
            base.FrameworkInitialize();

            string content = String.Empty;

            //TODO: Need to implement some kind of caching for this stuff - we cant be doing this everytime for the same bloody controls
            Type type = this.GetType();

            Stream stream = Assembly.GetAssembly(type).GetManifestResourceStream(GetType(), GetType().Name + ".ascx");

            using (StreamReader reader = new StreamReader(stream))
            {
                content = reader.ReadToEnd();
            }

            Control userControl = Page.ParseControl(content, true);

            EmbeddedUserControlOptionsAttribute options = type.GetCustomAttributes(typeof(EmbeddedUserControlOptionsAttribute), false).OfType<EmbeddedUserControlOptionsAttribute>().FirstOrDefault();
            if (options != null)
            {
                this.EnableViewState = options.EnableViewState;
                this.ClientIDMode = options.ClientIDMode;
            }

            this.Controls.Add(userControl);

            EnsureControls();

            if (OnFrameworkInitialized != null)
                OnFrameworkInitialized();
        }
        #endregion

        #region Private Methods
        private void EnsureControls()
        {
            Type type = this.GetType();

            List<FieldInfo> fields = s_CachedControlFieldsDictionary.ContainsKey(type.FullName)
                ? s_CachedControlFieldsDictionary[type.FullName]
                : null;

            if (fields == null)
            {
                lock (s_CachedControlFieldsDictionary)
                {
                    if (fields == null)
                    {
                        fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).Where(x => typeof(Control).IsAssignableFrom(x.FieldType)).ToList();
                        s_CachedControlFieldsDictionary.Add(type.FullName, fields);
                    }
                }
            }

            //We should have the reflection information we need from the cache now
            foreach (FieldInfo field in fields)
            {
                if (field.GetValue(this) == null)
                {
                    Control value = FindControl(field.Name);
                    field.SetValue(this, value);
                }
            }
        }
        #endregion
    }
}
