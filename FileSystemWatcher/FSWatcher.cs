using FileSystemWatcher.Interfaces;
using FileSystemWatcher.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FileSystemWatcher
{
    public class FSWatcher
    {
        private CancellationTokenSource cts;
        private readonly IWatcherLoggingService _watcherLoggingService;
        private readonly IErrorLoggingService _errorLoggingService;

        public FSWatcher(string folderPath, IWatcherLoggingService outputService, IErrorLoggingService errorLoggingService)
        {
            FolderPath = folderPath;
            _watcherLoggingService = outputService;
            _errorLoggingService = errorLoggingService;
        }

        public string FolderPath { get; set; }

        public async Task Start()
        {
            cts?.Dispose();
            cts = new CancellationTokenSource();

            using (System.IO.FileSystemWatcher fileSystemWatcher = new System.IO.FileSystemWatcher(FolderPath))
            {
                fileSystemWatcher.IncludeSubdirectories = true;

                fileSystemWatcher.NotifyFilter =
                    NotifyFilters.Attributes |
                    NotifyFilters.CreationTime |
                    NotifyFilters.DirectoryName |
                    NotifyFilters.FileName |
                    NotifyFilters.LastAccess |
                    NotifyFilters.LastWrite |
                    NotifyFilters.Security |
                    NotifyFilters.Size;              

                fileSystemWatcher.Changed += FileSystemWatcher_Changed;
                fileSystemWatcher.Created += FileSystemWatcher_Created;
                fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
                fileSystemWatcher.Error += FileSystemWatcher_Error;
                fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;

                fileSystemWatcher.EnableRaisingEvents = true;

                await Task.Run(() =>
                {                   
                    while (!cts.Token.IsCancellationRequested) ;
                });  
            }
        }

        public void Stop()
        {
            cts.Cancel();
        }

        private async void FileSystemWatcher_Renamed(object sender, System.IO.RenamedEventArgs e)
        {
            var eventInfo = $"Renamed from {e.OldName} to {e.Name}\nOld path: {e.OldFullPath}\nNew path: {e.FullPath}";

            var model = new FSWModel
            {
                Event = e.ChangeType,
                Path = e.FullPath,
                EventInfo = eventInfo
            };

            await _watcherLoggingService.Log(model);
        }

        private async void FileSystemWatcher_Deleted(object sender, System.IO.FileSystemEventArgs e)
        {
            var eventInfo = $"Deleted {e.Name} at path {e.FullPath}";

            var model = new FSWModel
            {
                Event = e.ChangeType,
                Path = e.FullPath,
                EventInfo = eventInfo
            };

            await _watcherLoggingService.Log(model);
        }

        private async void FileSystemWatcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            var eventInfo = $"Created {e.Name} at path {e.FullPath}";

            var model = new FSWModel
            {
                Event = e.ChangeType,
                Path = e.FullPath,
                EventInfo = eventInfo
            };

            await _watcherLoggingService.Log(model);
        }

        private async void FileSystemWatcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            var eventInfo = $"Changed {e.Name} at path {e.FullPath}";
            var model = new FSWModel
            {
                Event = e.ChangeType,
                Path = e.FullPath,
                EventInfo = eventInfo
            };

            await _watcherLoggingService.Log(model);
        }

        private void FileSystemWatcher_Error(object sender, System.IO.ErrorEventArgs e)
        {
            _errorLoggingService.Log(e.GetException().Message);
        }
    }
}
