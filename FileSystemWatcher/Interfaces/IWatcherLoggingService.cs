using FileSystemWatcher.Models;
using System.Threading.Tasks;

namespace FileSystemWatcher.Interfaces
{
    public interface IWatcherLoggingService
    {
        Task Log(FSWModel fswModel);
    }
}
