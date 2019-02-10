using FileSystemWatcher.Interfaces;
using System.Windows;

namespace FileSystemWatcher.Services
{
    public class DialogErrorLoggingService : IErrorLoggingService
    {
        public void Log(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
