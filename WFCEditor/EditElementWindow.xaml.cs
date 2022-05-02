using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace WFCEditor
{

    public partial class EditElementWindow : Window, INotifyPropertyChanged
    {
        private string newElement;

        public bool CanSave
        {
            get => !NewElement.ToLower().Trim(' ').Equals(OldElement.ToLower());      
        }


        public string OldElement { get; private set; }

        public string NewElement 
        { 
            get { return newElement; }
            set
            {
                 
                newElement = value;
                OnPropertyChanged("CanSave");

            }
        }

        public string Result => NewElement.Trim(' ');
       

        public event PropertyChangedEventHandler PropertyChanged;

        public EditElementWindow(string Element)
        {
            InitializeComponent();

            OldElement = Element;
            NewElement = Element;
            
            DataContext = this;
        }

        private void BTN_Discard_Click(object sender, RoutedEventArgs e)
        {

            DialogResult = false;
            this.Close();
        }

        private void BTN_Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}