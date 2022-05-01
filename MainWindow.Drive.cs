using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using WordFilter.Entities;

namespace WordFilter
{
    partial class MainWindow
    {
        public void CreateAnalyzers()
        {
            totalFileCount = 0;
            string[] debugDrives = new string[] { @"A:\", @"B:\" };
            Analyzers = new ObservableCollection<Analyzer>();
            foreach (var drive in DriveInfo.GetDrives())
            {
                
                if (drive.IsReady)
                {
                    if (!debugDrives.Contains(drive.Name))
                        continue;
                    var analyzer = new Analyzer(drive.RootDirectory.FullName);
                    if(BannedStrings!= null)
                        analyzer.SetBannedStrings(BannedStrings);

                    analyzer.FilesCounted += Analyzer_FilesCounted;
                    analyzer.Completed += Analyzer_Completed;
                    analyzer.PropertyChanged += Analyzer_PropertyChanged;
                    Analyzers.Add(analyzer);
                }
            };
            LB_Drives.ItemsSource = Analyzers;
            foreach(var a in Analyzers)
            {
                a.CountFilesAsync();
            }
        }

        private void Analyzer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Analyzer analyzer = sender as Analyzer;
            if(e.PropertyName == "AnalyzedFileCount")
            {
                AnalyzedFileCount = Analyzers.Select(a => a.AnalyzedFileCount).Sum();
            }
        }

        private void Analyzer_Completed(Analyzer sender)
        {
            DirectoryInfo dir = Directory.CreateDirectory(Path.Combine(reportFolderPath, sender.Root[0].ToString()));
            dir.Delete(true);
            dir.Create();
            foreach(var report in sender.FileReports)
            {
                try
                {
                    File.Copy(report.FullPath, Path.Combine(dir.FullName, report.Name), true);
                }
                catch (Exception)
                {
                    continue;
                }
                using(StreamWriter writer = new StreamWriter(Path.Combine(dir.FullName, report.Name + " FIXED.txt")))
                {
                    using (StreamReader reader = new StreamReader(report.FullPath))
                    {
                        string text = reader.ReadToEnd();
                        foreach(var counter in report.WordOccurenceAttribute)
                            text = Regex.Replace(text, $@"\b{counter.Word}\b", "*******");
                        writer.Write(text);
                    }
                }
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
            TotalFileCount += sender.TotalFileCount;
            if (Analyzers.Where(a => a.Ready).Count() == Analyzers.Count)
            {
                AnalyzersLoaded = true;
                SilentAllFilesCounted?.Invoke(this);
            }
        }
    }
}
