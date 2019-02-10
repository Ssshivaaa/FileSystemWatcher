using FileSystemWatcher.Interfaces;
using FileSystemWatcher.Models;
using FileSystemWatcher.Services;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace FileSystemWatcher
{
    public partial class FileSystemWatcherWindow : Window
    {
        private readonly IErrorLoggingService _errorLoggingService; 
        private FSWatcher _fileSystemWatcher;

        public FileSystemWatcherWindow()
        {
            InitializeComponent();
            DataContext = this;
            _errorLoggingService = new DialogErrorLoggingService();
            Events = new ObservableCollection<FSWModel>();
        }

        public ObservableCollection<FSWModel> Events { get; set; }

        private void btnFolderSelect_Click(object sender, RoutedEventArgs e)
        {
            Execute(() =>
            {
                using (CommonOpenFileDialog dialog = new CommonOpenFileDialog()
                {
                    IsFolderPicker = true
                })
                {
                    var result = dialog.ShowDialog();

                    if (result != CommonFileDialogResult.Ok)
                    {
                        return;
                    }

                    string selectedFolder = dialog.FileName;

                    if (!string.IsNullOrEmpty(selectedFolder) && tbSelectedPath.Text != selectedFolder)
                    {
                        Events.Clear();
                    }

                    btnStart.IsEnabled = !string.IsNullOrEmpty(selectedFolder);

                    tbSelectedPath.Text = selectedFolder;
                }
            });       
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Execute(() =>
            {
                if (_fileSystemWatcher == null)
                {
                    _fileSystemWatcher = new FSWatcher(tbSelectedPath.Text, new ObservableCollectionWatcherLoggingService(Events), _errorLoggingService);
                }
                else
                {
                    _fileSystemWatcher.FolderPath = tbSelectedPath.Text;
                }

                SwitchButtonsState();

                _fileSystemWatcher.Start();
            });         
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            Execute(() =>
            {
                SwitchButtonsState();
                _fileSystemWatcher.Stop();
            });       
        }

        private void SwitchButtonsState()
        {
            Execute(() =>
            {
                btnStop.IsEnabled = !btnStop.IsEnabled;
                btnStart.IsEnabled = !btnStart.IsEnabled;
                btnFolderSelect.IsEnabled = !btnFolderSelect.IsEnabled;
            });        
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Execute(()=> Events.Clear());
        }

        private void Execute(Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                _errorLoggingService.Log(ex.Message);
            }
        }
    }
}
