using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using WordFilter.Entities;

namespace WordFilter
{
    partial class MainWindow
    {
        private void CreateAnalyzers()
        {
            string[] debugDrives = new string[] { @"A:\", @"B:\" };
            Analyzers = new ObservableCollection<Analyzer>();
            foreach (var drive in DriveInfo.GetDrives())
            {
                
                if (drive.IsReady)
                {
                    if (DEBUG && !debugDrives.Contains(drive.Name))
                        continue;
                    var analyzer = new Analyzer(drive.RootDirectory.FullName);
                    analyzer.FilesCounted += Analyzer_FilesCounted;
                    analyzer.Completed += Analyzer_Completed;
                    Analyzers.Add(analyzer);
                }
            };
            foreach(var a in Analyzers)
            {
                a.CountFilesAsync();
            }
        }

        private void Analyzer_Completed(Analyzer sender)
        {
            Directory.Delete(reportFolderPath, true);
            Directory.CreateDirectory(reportFolderPath);
            DirectoryInfo dir = Directory.CreateDirectory(Path.Combine(reportFolderPath, sender.Root[0].ToString()));
            
            foreach(var report in sender.FileReports)
            {
                File.Copy(report.FullPath, Path.Combine(dir.FullName, report.Name),true);
                using (StreamWriter writer = new StreamWriter(Path.Combine(dir.FullName,report.Name + " REPORT.xml")))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(FileReport));
                    serializer.Serialize(writer, report);
                }
            }
        }

        private void Analyzer_FilesCounted(Analyzer sender)
        {
            if (sender.TotalFileCount == 0)
                if (!Analyzers.Remove(sender))
                {
                    Analyzers.CollectionChanged += (s, args) => (s as ObservableCollection<Analyzer>).Remove(sender);
                    return;
                }
            if (Analyzers.Where(a=>a.Ready).Count() == Analyzers.Count)
                AnalyzersLoaded = true;
        }
    }
}
