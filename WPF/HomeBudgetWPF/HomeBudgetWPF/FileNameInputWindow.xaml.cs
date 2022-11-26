using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.IO;

namespace HomeBudgetWPF
{
    /// <summary>
    /// Interaction logic for FileNameInput.xaml
    /// </summary>
    public partial class FileNameInputWindow : Window
    {

        private string fileName;
        private string folder;

        public string NewFilePath
        {
            get { return $"{folder}\\{fileName}"; }
        }

        public bool HasFileName
        {
            get { return !string.IsNullOrEmpty(fileName); }
        }

        public FileNameInputWindow(string folder)
        {
            InitializeComponent();
            this.folder = folder;
            txtFolderDisplay.Text = $"{folder}\\";
        }

        private void btnCreateFile_Click(object sender, RoutedEventArgs e)
        {
            
            bool cancelCreateFile = false;
            if (File.Exists($"{folder}\\{txtFileInput.Text}"))
            {
                MessageBoxResult result = MessageBox.Show($"{folder}\\{txtFileInput.Text} already exists. Do you want to overwrite it?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                cancelCreateFile = result != MessageBoxResult.Yes;
            }

            // Only make the file if the user wants to
            if (!cancelCreateFile)
            {
                fileName = txtFileInput.Text;
                this.Close();
            }
        }

        private void txtFileInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnCreateFile.IsEnabled = !string.IsNullOrEmpty(txtFileInput.Text);
        }
    }
}
