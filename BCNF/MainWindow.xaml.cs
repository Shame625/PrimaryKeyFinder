using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BCNF
{
    public partial class MainWindow : Window
    {
        private static string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        Problem problem;
        bool debugMode = true;

        public MainWindow()
        {
            InitializeComponent();
            problem = new Problem();
        }

        private void unesiButton_Click(object sender, RoutedEventArgs e)
        {
            UnosFO(foBox.Text);
        }

        private void UnosAtributa(string str)
        {
            problem.veliko_R.skup.Clear();

            if (string.IsNullOrEmpty(str))
            {
                MessageBox.Show("Nepostoje atributi!", "Prazno!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string[] atributi = str.Split(',');
            atributi = atributi.Select(s => RemoveWhitespace(s.ToUpper())).ToArray();
            atributi = atributi.Distinct().ToArray();
            atributi = atributi.Where(s => !string.IsNullOrEmpty(s)).ToArray();

            foreach (string s in atributi)
            {
                Atribut novi = new Atribut(s);

                problem.veliko_R.skup.Add(novi);
            }

            UpdateAtributaListBox();
        }

        private void UnosFO(string st)
        {
            problem.relacije.Clear();
            if (string.IsNullOrWhiteSpace(st))
            {
                MessageBox.Show("Nepostoje relacije!", "Prazno!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string[] relacije = st.Split(',');
            relacije = relacije.Select(s => RemoveWhitespace(s.ToUpper())).ToArray();
            relacije = relacije.Distinct().ToArray();
            relacije = relacije.Where(s => !string.IsNullOrEmpty(s)).ToArray();

            foreach (string str in relacije)
            {
                string[] Strane = str.Split(new string[] { "->" }, StringSplitOptions.None);
                if(Strane.Count() == 2)
                {
                    string[] Lijevo = Strane[0].Split('|');
                    string[] Desno = Strane[1].Split('|');

                    FO nova = new FO();

                    foreach(string s in Lijevo)
                    {
                        nova.Lijevo.skup.Add(new Atribut(s));
                    }
                    foreach (string s in Desno)
                    {
                        nova.Desno.skup.Add(new Atribut(s));
                    }
                    problem.relacije.Add(nova);
                }
               
            }
            UpdateRelacijeListBox();
        }

        private void listBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                int prev = 0;
                if (relacije_listBox.SelectedIndex >= 0)
                {
                    prev = relacije_listBox.SelectedIndex;
                    problem.relacije.RemoveAt(prev);
                    UpdateRelacijeListBox();

                    relacije_listBox.SelectedIndex = prev;
                    relacije_listBox.Focus();
                }
            }
        }

        public void Append(string str)
        {
            if(debugMode)
                debugBox.Text += str + '\n';
        }

        private void UpdateRelacijeListBox()
        {
            relacije_listBox.ItemsSource = null;
            relacije_listBox.ItemsSource = problem.relacije;
        }

        private void UpdateAtributaListBox()
        {
            atributi_listBox.ItemsSource = null;
            atributi_listBox.ItemsSource = problem.veliko_R.skup;
        }

        private void UpdateKljuceviListBox()
        {
            kljucevi_listBox.Items.Clear();

            for (int i= problem.kljucevi.Count()-1; i>=0;i--)
            {
                string t = "";
                foreach (Atribut s in problem.kljucevi[i].skup)
                {
                    t += s.ToString();
                }
                kljucevi_listBox.Items.Add(t);
            }
        }

        private void foBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                UnosFO(foBox.Text);
            }
        }

        private void unesiAtributeBottun_Click(object sender, RoutedEventArgs e)
        {
            UnosAtributa(atributiBox.Text);
        }


        private void atributiBox_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                UnosAtributa(atributiBox.Text);
        }

        private void atributi_listBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                int prev = 0;
                if (atributi_listBox.SelectedIndex >= 0)
                {
                    prev = atributi_listBox.SelectedIndex;
                    problem.veliko_R.skup.RemoveAt(prev);
                    UpdateAtributaListBox();

                    atributi_listBox.SelectedIndex = prev;
                    atributi_listBox.Focus();
                }
            }
        }

        private void kljuceviButton_Click(object sender, RoutedEventArgs e)
        {
            problem.Pronadji_Kljuceve();
            UpdateKljuceviListBox();
        }


        string filepath = "";

        void LoadFile()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();


            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text|*.txt|All|*.*";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                filepath = dlg.FileName;
                loadFromFile.Text = filepath;
            }
        }

        private void loadFromFile_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LoadFile();
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                MessageBox.Show("Odaberite lokaciju fajla prvo!", "Odabir!");
                LoadFile();
                return;
            }
            if (File.Exists(filepath))
            {
            string readText = File.ReadAllText(filepath);
            readText = readText.Replace("\n", String.Empty).Replace("\t", String.Empty).Replace("\r", String.Empty);

            string[] temp = readText.Split(';');

                if (temp.Length >= 3)
                {
                    UnosAtributa(temp[0]);
                    UnosFO(temp[1]);

                    if (temp[2].Length > 0)
                    {
                        problem.Pronadji_Kljuceve();
                        UpdateKljuceviListBox();
                    }
                }
                else
                {
                    MessageBox.Show("Loš format učitane datoteke!");
                }
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            string fileText = "";

            //radimo string

            foreach (Atribut sa in problem.veliko_R.skup)
            {
                fileText += sa.ToString() + ',';
            }

            if (string.IsNullOrEmpty(fileText))
            {
                MessageBox.Show("Greška pri zapisivanju atributa!", "Greška");
                return;
            }

            fileText = fileText.Remove(fileText.Length - 1, 1) + ";" + System.Environment.NewLine;

            string FOstr = "";

            foreach (FO fo in problem.relacije)
            {
                FOstr += fo.MyStr() + ',';
            }

            if (string.IsNullOrEmpty(FOstr))
            {
                MessageBox.Show("Greška pri zapisivanju relacija!", "Greška");
                return;
            }

            FOstr = FOstr.Remove(FOstr.Length - 1, 1) + ";" + System.Environment.NewLine;



            fileText += FOstr;

            string Keys = "";
            foreach (Skup_Atributa pk in problem.kljucevi)
            {
                Keys += pk.MyStr() + ',';
            }

            if (string.IsNullOrEmpty(Keys))
            {
                MessageBox.Show("Greška pri zapisivanju ključeva!", "Greška");
                return;
            }

            Keys = Keys.Remove(Keys.Length - 1, 1) + ";";



            fileText += Keys;

            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog()
            {
                Filter = "Text Files(*.txt)|*.txt|All(*.*)|*"
            };


            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, fileText);
            }
        }

        private void debugCheckBox_Click(object sender, RoutedEventArgs e)
        {
            debugMode = debugCheckBox.IsChecked.Value;

            debugCanvas.IsEnabled = true;
            BrushConverter bc = new BrushConverter();
            Brush brush;

            if (debugMode)
            {
                debugCanvas.IsEnabled = true;
                brush = (Brush)bc.ConvertFrom("#FF42191D");
                brush.Freeze();
                debugCanvas.Background = brush;
            }
            else
            {
                debugCanvas.IsEnabled = false;
                brush = (Brush)bc.ConvertFrom("#FFA29C9D");
                brush.Freeze();
                debugCanvas.Background = brush;
            }
        }
    }
}
