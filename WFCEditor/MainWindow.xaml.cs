using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;



namespace WFCEditor
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool isOpenedFromFile;
        private string curPath;

        public ObservableCollection<string> OldElements { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> NewElements { get; set; } = new ObservableCollection<string>();
        public string CurrentPath
        {
            get => curPath;
            set
            {
                curPath = value;
                OnPropertyChanged();
            }
        }
        public bool SaveNeeded
        {
            get
            {
                if (NewElements.Count != OldElements.Count)
                {
                    MenuItemSave.IsChecked = false;
                    return true;
                }

                if(NewElements.Count == 0)
                {
                    MenuItemSave.IsChecked = true;
                    return false;
                }

              
                for(int i = 0; i < NewElements.Count; i++)
                {
                    bool isNew = true;

                    for (int j = 0; j < OldElements.Count; j++)
                    {
                        if (NewElements[i].ToLower().Equals(OldElements[j].ToLower()))
                        {
                            isNew = false;
                        }        
                     
                    }
                    if (isNew)
                    {
                        MenuItemSave.IsChecked = false;
                        return true;
                        
                    }
                }
                MenuItemSave.IsChecked = true;
                return false;
            }
        }
        public bool IsOpenedFromFile
        {
            get => isOpenedFromFile;
            set
            {
                isOpenedFromFile = value;
                OnPropertyChanged("SaveNeeded");
            }
        }
        public string NewElement { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;



        public MainWindow()
        {
            InitializeComponent();
            LB_Elements.ItemsSource = NewElements;
            DataContext = this;

            string[] args = Environment.GetCommandLineArgs();

            if(args.Length == 2)
            {

                if (File.Exists(args[1]) && Path.GetExtension(args[1]) == ".wfc")
                {
                    CurrentPath = args[1];
                    foreach (string arg in OpenAs(args[1]))
                    {
                        OldElements.Add(arg);
                        NewElements.Add(arg);
                    }
                        
                }
            }


            try
            {
                RegistryKey rk = Registry.ClassesRoot.OpenSubKey("SystemFileAssociations\\.wfc\\Shell\\Open in WFCEditor\\Command", true);

                if (rk == null)
                    rk = Registry.ClassesRoot.CreateSubKey("SystemFileAssociations\\.wfc\\Shell\\Open in WFCEditor\\Command", true);


                if (rk.GetValue("") == null)
                    rk.SetValue("", $"\"{args[0]}\" \"%1\"");
                else
                    if ((string)rk.GetValue("") != $"\"{args[0]}\" \"%1\"")
                    rk.SetValue("", $"\"{args[0]}\" \"%1\"");


                rk.Close();
            }
            catch (Exception) { }
         

        }

        

        private void MenuItemSaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (SaveNeeded)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "WordFilter Config Files|*.wfc";
                sfd.Title = "Save WFC file as...";
                sfd.InitialDirectory = CurrentPath ?? AppDomain.CurrentDomain.BaseDirectory;

                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Save(sfd.FileName);

                    OldElements.Clear();
                    foreach (var element in NewElements)
                        OldElements.Add(element);

                    OnPropertyChanged("SaveNeeded");
                }
             
            }
            else
                MessageBox.Show("You can't save empty file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BTN_AddElement_Click(object sender, RoutedEventArgs e)
        {
            AddElement();
        }

        private void TB_NewElement_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TB_NewElement.Text))
                BTN_AddElement.IsEnabled = false;
            else
                BTN_AddElement.IsEnabled = true;

        }

        private void LB_Elemnts_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            var clickedItem = LB_Elements.InputHitTest(e.GetPosition(LB_Elements));

            if (clickedItem != null)
            {

                string Element;

                switch (clickedItem.GetType().Name)
                {
                    case "Border":
                        Element = ((TextBlock)((Border)clickedItem).Child).Text;
                        break;
                    case "TextBlock":
                        Element = ((TextBlock)clickedItem).Text;
                        break;
                    default:
                        return;
                       
                }

                EditElementWindow window = new EditElementWindow(Element);

                if (window.ShowDialog() == true)
                {
                    int index = NewElements.IndexOf(Element);

                    if(index != -1 && NewElements.Count(NewElem => NewElem.ToLower().Equals(window.NewElement.ToLower()) == true) == 0)
                    {
                        NewElements[index] = window.NewElement;
                        OnPropertyChanged("SaveNeeded");
                    }     
                    else
                        MessageBox.Show("This element already added", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                }




            }
        }

        private void MenuItemOpenAs_Click(object sender, RoutedEventArgs e)
        {
           
            if(SaveNeeded &&  MessageBox.Show("You have not saved information. Save?", "info", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (isOpenedFromFile)
                    Save(CurrentPath);
                else
                    SaveAs();
    
            }
                   


            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "WordFilter Config Files|*.wfc";
            ofd.Title = "Open WFC file as...";
            ofd.InitialDirectory = CurrentPath ?? AppDomain.CurrentDomain.BaseDirectory;

            if(ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK) 
            {
                List<string> tmpList = OpenAs(ofd.FileName);

                CurrentPath = ofd.FileName;
                isOpenedFromFile = true;


                OldElements.Clear();
                NewElements.Clear();

                foreach (string s in tmpList) {
                    OldElements.Add(s);
                    NewElements.Add(s);
                }
                OnPropertyChanged("SaveNeeded");
            }


        }

        public bool Save(string FileName)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(FileName))
                {
                    foreach (string s in NewElements)
                        writer.WriteLine(s);
                }
            }
            catch(Exception) { return false; }
            return true;


        }

        public bool SaveAs()
        {
            if (SaveNeeded)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "WordFilter Config Files|*.wfc";
                sfd.Title = "Save WFC file as...";
                sfd.InitialDirectory = CurrentPath ?? AppDomain.CurrentDomain.BaseDirectory;

              
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    if(Save(sfd.FileName) == false)
                        return false;
                        

                OldElements.Clear();

                foreach (var element in NewElements)
                    OldElements.Add(element);

                OnPropertyChanged("SaveNeeded");
                return true;
            }
            else
                MessageBox.Show("You can't save empty file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        public List<string> OpenAs(string FileName)
        {
            List<string> tmpList;

            try
            {
                using (StreamReader sr = new StreamReader(FileName))
                {
                    tmpList = sr.ReadToEnd().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
            }catch(Exception) { return null; }
            
            return tmpList;




        }

        public void AddElement()
        {
            if(NewElement != null && !string.IsNullOrWhiteSpace(NewElement))
            {
                string tmp = NewElement.Trim(' ');
                if (NewElements.Count(elem => elem.ToLower() == tmp.ToLower()) == 0)
                {
                    NewElements.Add(tmp);
                    TB_NewElement.Text = String.Empty;
                    OnPropertyChanged("SaveNeeded");
                }
                else
                    MessageBox.Show("This element already added", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                MessageBox.Show("Invalid input", "Error", MessageBoxButton.OK, MessageBoxImage.Error);


        }

        private void BTN_DeleteElement_Click(object sender, RoutedEventArgs e)
        {
            NewElements.Remove((string)LB_Elements.SelectedItem);
            OnPropertyChanged("SaveNeeded");

            if(LB_Elements.Items.Count > 0)
                LB_Elements.SelectedIndex = LB_Elements.Items.Count-1;
        }

        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {

            if (!SaveNeeded)
                return;

            if (!isOpenedFromFile)
            {

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "WordFilter Config Files|*.wfc";
                sfd.Title = "Save WFC file as...";
                sfd.InitialDirectory = CurrentPath ?? AppDomain.CurrentDomain.BaseDirectory;

                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    Save(sfd.FileName);
            }
            else
            {
                Save(CurrentPath);
            }

            OldElements.Clear();
            foreach (var element in NewElements)
                OldElements.Add(element);

            OnPropertyChanged("SaveNeeded");



        }

        private void TB_NewElement_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
            {
                AddElement();
            }
             
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (SaveNeeded && MessageBox.Show("You have not saved information. Save?", "info", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (isOpenedFromFile)
                    Save(CurrentPath);
                else
                    SaveAs();
            }
        }
    }
}