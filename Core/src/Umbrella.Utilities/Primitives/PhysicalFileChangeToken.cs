using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.Utilities.Primitives
{
    public class PhysicalFileChangeToken : IChangeToken
    {
        #region Constructors
        public PhysicalFileChangeToken(FileInfo fileInfo,
            bool expireOnChanged = true,
            bool expireOnError = true,
            bool expireOnDeleted = true,
            bool expireOnRenamed = true)
        {
            var fsw = new FileSystemWatcher(fileInfo.DirectoryName, fileInfo.Name);

            if (expireOnChanged)
                fsw.Changed += (sender, args) => HasChanged = true;

            if (expireOnError)
                fsw.Error += (sender, args) => HasChanged = true;

            if (expireOnDeleted)
                fsw.Deleted += (sender, args) => HasChanged = true;

            if (expireOnRenamed)
                fsw.Renamed += (sender, args) => HasChanged = true;

            fsw.EnableRaisingEvents = true;
        }
        #endregion

        #region IChangeToken Members
        public bool HasChanged { get; set; }
        public bool ActiveChangeCallbacks => false;
        public IDisposable RegisterChangeCallback(Action<object> callback, object state) => EmptyDisposable.Instance;
        #endregion
    }
}