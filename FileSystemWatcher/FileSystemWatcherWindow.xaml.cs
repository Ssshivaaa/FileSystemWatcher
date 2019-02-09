using FileSystemWatcher.Models;
using FileSystemWatcher.Services;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.ObjectModel;
using System.Windows;

namespace FileSystemWatcher
{
    public partial class FileSystemWatcherWindow : Window
    {
        private FSWatcher _fileSystemWatcher;
        public ObservableCollection<FSWModel> Events { get; set; }

        public FileSystemWatcherWindow()
        {
            InitializeComponent();
            Events = new ObservableCollection<FSWModel>();
            DataContext = this;
        }

        private void btnFolderSelect_Click(object sender, RoutedEventArgs e)
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
              
                if(!string.IsNullOrEmpty(selectedFolder) && tbSelectedPath.Text != selectedFolder)
                {
                    Events.Clear();
                }

                btnStart.IsEnabled = !string.IsNullOrEmpty(selectedFolder);                

                tbSelectedPath.Text = selectedFolder;
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if(_fileSystemWatcher == null)
            {
                _fileSystemWatcher = new FSWatcher(tbSelectedPath.Text, new WPFGridWatcherLoggingService(Events), new DialogErrorLoggingService());
            }
            else
            {
                _fileSystemWatcher.FolderPath = tbSelectedPath.Text;
            }

            SwitchButtonsState();

            _fileSystemWatcher.Start();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            SwitchButtonsState();
            _fileSystemWatcher.Stop();
        }

        private void SwitchButtonsState()
        {
            btnStop.IsEnabled = !btnStop.IsEnabled;
            btnStart.IsEnabled = !btnStart.IsEnabled;
            btnFolderSelect.IsEnabled = !btnFolderSelect.IsEnabled;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Events.Clear();
        }
    }
}
