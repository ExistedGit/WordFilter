using System.Collections.ObjectModel;
using System.IO;
using WordFilter.Entities;

namespace WordFilter
{
    partial class MainWindow
    {
        private ObservableCollection<Analyzer> CreateAnalyzers()
        {
            ObservableCollection<Analyzer> result = new ObservableCollection<Analyzer>();
            foreach (var drive in DriveInfo.GetDrives()) {
                if (drive.IsReady)
                    result.Add(new Analyzer(drive.RootDirectory.FullName));
            };
            return result;
        }
    }
}
