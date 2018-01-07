using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Extensions.Logging.Azure
{
   public class LocationInformation
    {
        public string ClassName { get; }
        public string FileName { get; }
        public string LineNumber { get; }
        public string MethodName { get; }
        public string FullInfo { get; }

        public LocationInformation(string className, string fileName, string lineNumber, string methodName, string fullInfo)
        {
            ClassName = className;
            FileName = fileName;
            LineNumber = lineNumber;
            MethodName = methodName;
            FullInfo = fullInfo;
        }
    }
}