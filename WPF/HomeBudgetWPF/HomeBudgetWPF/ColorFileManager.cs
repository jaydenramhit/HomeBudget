using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.IO;

namespace HomeBudgetWPF
{
    public static class ColorFileManager
    {

        private static Color? textBlockForeground;
        private static Color? labelForeground;
        private static Color? buttonBackground;
        private static Color? gridBackground;


        public static void ClearOldColors()
        {
            textBlockForeground = null;
            labelForeground = null;
            buttonBackground = null;
            gridBackground = null;
        }

        /// <summary>
        /// Stores the value of the text block foregrounds to be saved later.
        /// </summary>
        /// <param name="color">The new color of the text block foregrounds.</param>
        public static void ChangeTextBlockForegroundColor(Color color)
        {
            textBlockForeground = color;
        }

        /// <summary>
        /// Stores the value of the label foregroundsto be saved later.
        /// </summary>
        /// <param name="color">The new color of the label foregrounds.</param>
        public static void ChangeLabelForegroundColor(Color color)
        {
            labelForeground = color;
        }

        /// <summary>
        /// Stores the value of the button backgrounds to be saved later.
        /// </summary>
        /// <param name="color">The new color of the button backgrounds.</param>
        public static void ChangeButtonsBackgroundColor(Color color)
        {
            buttonBackground = color;
        }

        /// <summary>
        /// Stores the value of the grid backgrounds to be saved later.
        /// </summary>
        /// <param name="color">The new color of the grid backgorunds.</param>
        public static void ChangeGridBackgroundColor(Color color)
        {
            gridBackground = color;
        }

        /// <summary>
        /// Saves the color scheme to a csv file.
        /// </summary>
        /// <param name="csvFilename">The name of the csv file to save the colors to.</param>
        /// <returns></returns>
        public static bool SaveColorSchemeFile(string csvFilename)
        {

            StringBuilder builder = new StringBuilder();

            // Write the TextBlock Foreground
            if (textBlockForeground.HasValue)
                builder.AppendLine(ColorToCsvString("TextBlock-Foreground", textBlockForeground.Value));
            // Write the Label Foreground
            if (labelForeground.HasValue)
                builder.AppendLine(ColorToCsvString("Label-Foreground", labelForeground.Value));
            // Write the Button Background
            if (buttonBackground.HasValue)
                builder.AppendLine(ColorToCsvString("Button-Background", buttonBackground.Value));
            // Write the foreground
            if (gridBackground.HasValue)
                builder.AppendLine(ColorToCsvString("Grid-Background", gridBackground.Value));

            try
            {
                File.WriteAllText(csvFilename, builder.ToString());
                return true;
            }
            catch
            {
                return false;
            }

            
        }


        /// <summary>
        /// Gets a dictionary of string-color key-value pairs which returns a color when given the name of the xaml element.
        /// </summary>
        /// <param name="csvFilename">The name of the color scheme file to load.</param>
        /// <exception cref="FileNotFoundException">Thrown when the <paramref name="csvFilename"/> could not be found.</exception>
        /// <exception cref="IOException">Thrown when the program fails to read from the <paramref name="csvFilename"/> or when the file's content is invalid.</exception>"
        /// <returns>dictionary of string, color key value pairs which returns a color when given the name of the xaml element.</returns>
        public static Dictionary<string, Color> LoadColorSchemeFromFile(string csvFilename)
        {
            // Make sure the file exists
            if (!File.Exists(csvFilename))
            {
                File.Create(csvFilename).Close();

                gridBackground = Color.FromRgb(34, 34, 34);
                SaveColorSchemeFile(csvFilename);
            }

            // Read the input file
            string[] colorFileLines;
            try
            {
                colorFileLines = File.ReadAllLines(csvFilename);
            }
            catch(Exception e)
            {
                throw new IOException($"Error while trying to open the file at {csvFilename}: {e}");
            }

            // Create the dictionary
            Dictionary<string, Color> nameColorPairs = new Dictionary<string, Color>();
            try
            {
                foreach (string colorLine in colorFileLines)
                {
                    string[] csvValues = colorLine.Split(',');
                    Color color = new Color();

                    color.R = byte.Parse(csvValues[1]);
                    color.G = byte.Parse(csvValues[2]);
                    color.B = byte.Parse(csvValues[3]);
                    color.A = byte.Parse(csvValues[4]);
                    nameColorPairs.Add(csvValues[0], color);

                    // Save previous colors if loading, as they are initially null
                    switch (csvValues[0])
                    {
                        case "Grid-Background":
                            gridBackground = color;
                            break;
                        case "Button-Background":
                            buttonBackground = color;
                            break;
                        case "Label-Foreground":
                            labelForeground = color;
                            break;
                        case "TextBlock-Foreground":
                            textBlockForeground = color;
                            break;
                    }
                }
            }
            catch(Exception)
            {
                throw new IOException($"Error trying to convert the values in the input file at {csvFilename} to bytes.");
            }

            return nameColorPairs;
        }
        

        private static string ColorToCsvString(string name, Color color)
        {
            return $"{name},{color.R},{color.G},{color.B},{color.A}";
        }


    }
}
