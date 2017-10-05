using System;
using System.Threading.Tasks;

namespace Umbrella.Unity.Utilities.Async
{
    public interface ITaskCompletionSourceProcessor
    {
        void Enqueue(TaskCompletionSource<object> source, Func<bool> completionTestFunc);
    }
}