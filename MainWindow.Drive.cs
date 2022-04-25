using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordFilter.Entities;

namespace WordFilter
{
    partial class MainWindow
    {
        private List<Analyzer> CreateAnalyzers()
        {
            List<Analyzer> result = new List<Analyzer>();
            foreach (var drive in DriveInfo.GetDrives()) {
                if (drive.IsReady)
                    result.Add(new Analyzer(drive.RootDirectory.FullName));
            };
            return result;
        }
    }
}
