using FileSystemWatcher.Interfaces;
using FileSystemWatcher.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FileSystemWatcher.Services
{
    internal class ObservableCollectionWatcherLoggingService : IWatcherLoggingService
    {
        private ObservableCollection<FSWModel> _observableCollection;

        public ObservableCollectionWatcherLoggingService(ObservableCollection<FSWModel> observableCollection)
        {
            _observableCollection = observableCollection;
        }

        public Task Log(FSWModel fswModel)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            new Action(() => 
            {
                _observableCollection.Add(fswModel);
            }));
            
            return new Task(() => { });
        }
    }
}
