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
using ColorPickerWPF;

namespace EnterpriseBudget.DeptBudgets.OldFiles
{
    /// <summary>
    /// Interaction logic for ColorPickerWindow.xaml
    /// </summary>
    public partial class ColorSchemeWindow : Window
    {
        private Color? color;

        //private MainWindow mainWindow;
        private string filepath;

        //public ColorSchemeWindow(MainWindow referenceToMainWindow, string filepath)
        //{
        //    InitializeComponent();
        //    mainWindow = referenceToMainWindow;
        //    this.filepath = filepath;
        //    ChangeColors();
        //}



        private void btnBackground_Click(object sender, RoutedEventArgs e)
        {
            color = GetColor();

            if (color.HasValue)
            {
                ColorFileManager.ChangeGridBackgroundColor(color.Value);
                MakeAndSaveColorSchemeChanges();
            }
        }

        private void btnButton_Click(object sender, RoutedEventArgs e)
        {
            color = GetColor();

            if (color.HasValue)
            {
                ColorFileManager.ChangeButtonsBackgroundColor(color.Value);
                MakeAndSaveColorSchemeChanges();
            }
        }

        private void btnLabel_Click(object sender, RoutedEventArgs e)
        {
            color = GetColor();

            if (color.HasValue)
            {
                ColorFileManager.ChangeLabelForegroundColor(color.Value);
                MakeAndSaveColorSchemeChanges();
            }
        }

        private void btnTextBlock_Click(object sender, RoutedEventArgs e)
        {
            color = GetColor();

            if (color.HasValue)
            {
                ColorFileManager.ChangeTextBlockForegroundColor(color.Value);
                MakeAndSaveColorSchemeChanges();
            }
        }
        private void MakeAndSaveColorSchemeChanges()
        {
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(filepath);
            string filePath = System.IO.Path.GetDirectoryName(filepath);
            string newFilePath = filePath + "\\" + fileNameWithoutExtension + "_ColorScheme.csv";

            ColorFileManager.SaveColorSchemeFile(newFilePath);
            //mainWindow.ChangeColors();
            ChangeColors();
        }
        private Color? GetColor()
        {

            if (ColorPickerWindow.ShowDialog(out Color color))
                return color;

            return null;
        }
        public void ChangeColors()
        {
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(filepath);
            string filePath = System.IO.Path.GetDirectoryName(filepath);
            string newFilePath = filePath + "\\" + fileNameWithoutExtension + "_ColorScheme.csv";
            Dictionary<string, Color> colors = ColorFileManager.LoadColorSchemeFromFile(newFilePath);

            foreach (KeyValuePair<string, Color> color in colors)
            {
                Brush bgColor = new SolidColorBrush(color.Value);

                switch (color.Key)
                {
                    case "Grid-Background":
                        grdBackground.Background = bgColor;
                        break;
                    case "Button-Background":
                        Resources["mainButtons"] = bgColor;
                        break;
                    case "Label-Foreground":
                        Resources["mainLabels"] = bgColor;
                        break;
                        /*                    case "TextBlock-Foreground":
                                                Resources["mainTextBlocks"] = bgColor;
                                                break;*/
                }
            }

        }
    }
}
