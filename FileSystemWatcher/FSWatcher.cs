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
        private bool _isWatching;
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
            _isWatching = true;

            using (System.IO.FileSystemWatcher fileSystemWatcher = new System.IO.FileSystemWatcher(FolderPath))
            {
                fileSystemWatcher.IncludeSubdirectories = true;

                fileSystemWatcher.NotifyFilter =
                    NotifyFilters.Attributes |
                    NotifyFilters.DirectoryName;             

                fileSystemWatcher.Changed += FileSystemWatcher_Changed;
                fileSystemWatcher.Created += FileSystemWatcher_Created;
                fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
                fileSystemWatcher.Error += FileSystemWatcher_Error;
                fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;

                fileSystemWatcher.EnableRaisingEvents = true;
                              
                await Task.Run(()=> 
                {
                    while (_isWatching);
                });    
            }
        }

        public void Stop()
        {
            _isWatching = false;
        }

        private async void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
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

        private async void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
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

        private async void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
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

        private async void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            var eventInfo = $"Attributes changed for {e.Name} at path {e.FullPath}";
            var model = new FSWModel
            {
                Event = e.ChangeType,
                Path = e.FullPath,
                EventInfo = eventInfo
            };

            await _watcherLoggingService.Log(model);
        }

        private void FileSystemWatcher_Error(object sender, ErrorEventArgs e)
        {
            _errorLoggingService.Log(e.GetException().Message);
        }
    }
}
