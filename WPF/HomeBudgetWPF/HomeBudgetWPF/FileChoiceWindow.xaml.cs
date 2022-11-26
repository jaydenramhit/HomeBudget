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
using Microsoft.WindowsAPICodePack.Dialogs;

namespace HomeBudgetWPF
{
    /// <summary>
    /// Interaction logic for FileChoiceWindowxaml.xaml
    /// </summary>
    public partial class FileChoiceWindow : Window
    {
        public FileChoiceWindow()
        {
            InitializeComponent();
        }

        private void btnNewFile_Click(object sender, RoutedEventArgs e)
        {
            // https://csharp.hotexamples.com/examples/Microsoft.WindowsAPICodePack.Dialogs/CommonOpenFileDialog/-/php-commonopenfiledialog-class-examples.html
            CommonOpenFileDialog folderPickerDialog = new CommonOpenFileDialog();

            folderPickerDialog.IsFolderPicker = true;
            folderPickerDialog.Multiselect = false;
            folderPickerDialog.Title = "Select the save location of your Home Budget file.";
            folderPickerDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Pick the directory for the new file
            if (folderPickerDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                FileNameInputWindow choiceWindow = new FileNameInputWindow(folderPickerDialog.FileName);

                // Pick the name of the new file
                choiceWindow.Owner = this;
                var result = choiceWindow.ShowDialog();
                if (choiceWindow.HasFileName)
                {
                    try
                    {
                        MainWindow mainWindow = new MainWindow(choiceWindow.NewFilePath, true);
                        mainWindow.Show();
                        this.Close();
                    }
                    catch (Exception exception)
                    {
                        // If the file could not be opened, show an error
                        // This likely won't come up when it was written, but later when the MainWindow
                        // constructor reads all the home budgets it will be helpful.
                        MessageBox.Show($"The selected file could not be opened. {exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }

            }
        }

        private void btnExistingFile_Click(object sender, RoutedEventArgs e)
        {
            // https://csharp.hotexamples.com/examples/Microsoft.WindowsAPICodePack.Dialogs/CommonOpenFileDialog/-/php-commonopenfiledialog-class-examples.html
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.InitialDirectory = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\Documents";

            // If a file was selected, try to open the main window
            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {

                MainWindow mainWindow;
                // Make sure there's no issue creating the file
                try 
                {
                    mainWindow = new MainWindow(openFileDialog.FileName, false);
                    mainWindow.Show();
                    this.Close();
                }
                catch (Exception exception)
                {
                    // If the file could not be opened, show an error
                    // This likely won't come up when it was written, but later when the MainWindow
                    // constructor reads all the home budgets it will be helpful.
                    MessageBox.Show($"The selected file could not be opened. {exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

    }
}
