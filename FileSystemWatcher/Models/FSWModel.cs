using System;
using System.IO;

namespace FileSystemWatcher.Models
{
    public class FSWModel
    {
        public DateTime DateTime { get; private set; } = DateTime.Now;
        public string Path { get; set; }
        public WatcherChangeTypes Event { get; set; }
        public string EventInfo { get; set; }
    }
}
