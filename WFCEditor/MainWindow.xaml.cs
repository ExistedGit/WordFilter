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
        private bool isOpendFromFile;
        private string currentOpendPath;

        public ObservableCollection<string> OldElements { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> NewElements { get; set; } = new ObservableCollection<string>();
        public string CurrentOpendPath
        {
            get => currentOpendPath;
            set
            {
                currentOpendPath = value;
                OnPropertyChanged();
            }
        }
        public bool isNeedSave
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
        public bool IsOpendFromFile
        {
            get => isOpendFromFile;
            set
            {
                isOpendFromFile = value;
                OnPropertyChanged("IsNeedSave");
            }
        }
        public string NewElement { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;



        public MainWindow()
        {
            InitializeComponent();
            LB_Elements.ItemsSource = NewElements;
            DataContext = this;

        }

        

        private void MenuItemSaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (NewElements.Count != 0)
                SaveAs();
            else
                MessageBox.Show("Not have elements to save");
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

                    if(index != -1 && NewElements.Count(NewElem => NewElem.ToLower().Contains(window.NewElement.ToLower()) == true) == 0)
                    {
                        NewElements[index] = window.NewElement;
                        OnPropertyChanged("isNeedSave");
                    }     
                    else
                        MessageBox.Show("This element already added", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                }




            }
        }

        private void MenuItemOpenAs_Click(object sender, RoutedEventArgs e)
        {
           
            if(isNeedSave && MessageBox.Show("You have not saved information. Save?", "info", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (isOpendFromFile)
                {
                    Save();
                }
                else
                {

                }
               
            }
                

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "WordFilter Config Files|*.wfc";
            ofd.Title = "Open wfc file as...";
            ofd.InitialDirectory = CurrentOpendPath ?? AppDomain.CurrentDomain.BaseDirectory;

            if(ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK) 
            {
                List<string> tmpList;
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    tmpList = sr.ReadToEnd().Split(new char[] { '\n', '\r'}, StringSplitOptions.RemoveEmptyEntries).ToList();
                }

                OldElements.Clear();
                NewElements.Clear();

                foreach (string s in tmpList)
                {
                    string tmpElem = s.Trim(' ');
                    OldElements.Add(tmpElem);
                    NewElements.Add(tmpElem);

                }

                CurrentOpendPath = ofd.FileName;
                isOpendFromFile = true;


            }


        }

        private bool Save()
        {
            if(!isOpendFromFile || !isNeedSave)
                return false;

            try
            {
                using (StreamWriter writer = new StreamWriter(CurrentOpendPath))
                    foreach (string s in NewElements)
                        writer.WriteLine(s);
            }
            catch(Exception) { return false; }
         

            OldElements.Clear();

            foreach (string s in NewElements)
                OldElements.Add(s);

            return true;

        }

        public bool SaveAs()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "WordFilter Config Files|*.wfc";
            sfd.Title = "Save wfc file as...";
            sfd.InitialDirectory = CurrentOpendPath ?? AppDomain.CurrentDomain.BaseDirectory;

            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (StreamWriter writer = new StreamWriter(sfd.FileName))
                {
                    foreach (string s in NewElements)
                        writer.WriteLine(s);

                    CurrentOpendPath = sfd.FileName;
                    isOpendFromFile = true;
                    return true;
                }


            }
            else return false;
        }

        public void AddElement()
        {
            string tmp = NewElement.Trim(' ');
            if (NewElements.Count(elem => elem.ToLower() == tmp.ToLower()) == 0)
            {
                NewElements.Add(tmp);
                OnPropertyChanged("isNeedSave");

                TB_NewElement.Text = string.Empty;
            }
            else
                MessageBox.Show("This element already added", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BTN_DeleteElement_Click(object sender, RoutedEventArgs e)
        {
            NewElements.Remove((string)LB_Elements.SelectedItem);
            OnPropertyChanged("isNeedSave");

            if(LB_Elements.Items.Count > 0)
                LB_Elements.SelectedIndex = LB_Elements.Items.Count-1;
        }

        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {

            if (isOpendFromFile)
                Save();
            else
                SaveAs();
           
        }

        private void TB_NewElement_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
                AddElement();
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
