using FileSystemWatcher.Interfaces;
using FileSystemWatcher.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FileSystemWatcher.Services
{
    internal class WPFGridWatcherLoggingService : IWatcherLoggingService
    {
        private ObservableCollection<FSWModel> _gridSource;

        public WPFGridWatcherLoggingService(ObservableCollection<FSWModel> gridSource)
        {
            _gridSource = gridSource;
        }

        public Task Log(FSWModel fswModel)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            new Action(() => 
            {
                _gridSource.Add(fswModel);
            }));
            
            return new Task(() => { });
        }
    }
}
