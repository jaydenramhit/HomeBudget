using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using HomeBudgetWPF;
using System.Windows.Media;

namespace HomeBudgetWPFTests
{
    public class TestColourFileManager
    {
        [Fact]
        public void ChangeButtonsBackgroundColor_CorrectColorIsAdded()
        {
            // Arrange
            Color color = Color.FromRgb(255,100,25);
            ColorFileManager.ClearOldColors();

            // Act
            ColorFileManager.ChangeButtonsBackgroundColor(color);
            ColorFileManager.SaveColorSchemeFile("test.csv");
            Dictionary<string, Color> dictionary = ColorFileManager.LoadColorSchemeFromFile("test.csv");
            
            // Assert
            Assert.Equal(color, dictionary["Button-Background"]);
            Assert.Single(dictionary.Keys);
        }

        [Fact]
        public void ChangeGridsBackgroundColor_CorrectColorIsAdded()
        {
            // Arrange
            Color color = Color.FromRgb(255, 100, 25);
            ColorFileManager.ClearOldColors();

            // Act
            ColorFileManager.ChangeGridBackgroundColor(color);
            ColorFileManager.SaveColorSchemeFile("test.csv");
            Dictionary<string, Color> dictionary = ColorFileManager.LoadColorSchemeFromFile("test.csv");

            // Assert
            Assert.Equal(color, dictionary["Grid-Background"]);
            Assert.Single(dictionary.Keys);
        }

        [Fact]
        public void ChangeTextBlockForegroundColor_CorrectColorIsAdded()
        {
            // Arrange
            Color color = Color.FromRgb(255, 100, 25);
            ColorFileManager.ClearOldColors();

            // Act
            ColorFileManager.ChangeTextBlockForegroundColor(color);
            ColorFileManager.SaveColorSchemeFile("test.csv");
            Dictionary<string, Color> dictionary = ColorFileManager.LoadColorSchemeFromFile("test.csv");

            // Assert
            Assert.Equal(color, dictionary["TextBlock-Foreground"]);
            Assert.Single(dictionary.Keys);
        }

        [Fact]
        public void ChangeLabelForegroundColor_CorrectColorIsAdded()
        {
            // Arrange
            Color color = Color.FromRgb(255, 100, 25);
            ColorFileManager.ClearOldColors();

            // Act
            ColorFileManager.ChangeLabelForegroundColor(color);
            ColorFileManager.SaveColorSchemeFile("test.csv");
            Dictionary<string, Color> dictionary = ColorFileManager.LoadColorSchemeFromFile("test.csv");

            // Assert
            Assert.Equal(color, dictionary["Label-Foreground"]);
            Assert.Single(dictionary.Keys);
        }

        [Fact]
        public void SaveColorSchemeFile_NoData_SavesEmptyFile()
        {
            // Arrange
            ColorFileManager.ClearOldColors();
            
            // Act
            ColorFileManager.SaveColorSchemeFile("test.csv");
            Dictionary<string, Color> dictionary = ColorFileManager.LoadColorSchemeFromFile("test.csv");

            // Assert
            Assert.Empty(dictionary.Keys);
        }

        [Fact]
        public void SaveColorSchemeFile_SeveralColorsStored_SavesAndLoadsAllColors()
        {
            // Arrange
            Color buttonColor = Color.FromRgb(255, 100, 25);
            Color gridColor = Color.FromRgb(62, 0, 200);
            Color labelColor = Color.FromRgb(12, 0, 4);
            Color textBlockColor = Color.FromRgb(255, 255, 255);
            ColorFileManager.ClearOldColors();

            // Act
            ColorFileManager.ChangeButtonsBackgroundColor(buttonColor);
            ColorFileManager.ChangeGridBackgroundColor(gridColor);
            ColorFileManager.ChangeLabelForegroundColor(labelColor);
            ColorFileManager.ChangeTextBlockForegroundColor(textBlockColor);
            ColorFileManager.SaveColorSchemeFile("test.csv");
            Dictionary<string, Color> dictionary = ColorFileManager.LoadColorSchemeFromFile("test.csv");

            // Assert
            Assert.Equal(buttonColor, dictionary["Button-Background"]);
            Assert.Equal(gridColor, dictionary["Grid-Background"]);
            Assert.Equal(labelColor, dictionary["Label-Foreground"]);
            Assert.Equal(textBlockColor, dictionary["TextBlock-Foreground"]);
            Assert.Equal(4, dictionary.Keys.Count);
        }

    }
}
