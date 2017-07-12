using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities
{
    public static class UmbrellaStatics
    {
        public delegate string SerializeJson(object value, bool useCamelCasingRules = false);
        private static SerializeJson s_JsonSerializer;

        public static SerializeJson JsonSerializer
        {
            internal get
            {
                if (s_JsonSerializer == null)
                    throw new Exception("The JsonSerializer has not been assigned. This should be done on application startup.");

                return s_JsonSerializer;
            }
            set => s_JsonSerializer = value;
        }
    }
}