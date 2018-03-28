using System.Threading;
using System.Threading.Tasks;
using Umbrella.Unity.Logging.Models;
using static Umbrella.Unity.Logging.Services.RemoteClientLogService;

namespace Umbrella.Unity.Logging.Services
{
    public interface IRemoteClientLogService
    {
        Task<RemoteClientLogServiceResult> PostAsync(RemoteClientLogModel model, CancellationToken cancellationToken = default);
    }
}