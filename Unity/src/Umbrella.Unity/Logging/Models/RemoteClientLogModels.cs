using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Unity.Logging.Models
{
    public class RemoteClientLogModel
    {
        public Guid Id { get; set; }
        public RemoteClientLogLevel Level { get; set; }
        public string Url { get; set; }
        public string Source { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }

    public enum RemoteClientLogLevel
    {
        Warning = 3,
        Error = 4,
        Critical = 5
    }
}