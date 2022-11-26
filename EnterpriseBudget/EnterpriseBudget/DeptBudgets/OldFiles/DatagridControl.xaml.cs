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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Budget;

namespace EnterpriseBudget.DeptBudgets.OldFiles
{
    /// <summary>
    /// Interaction logic for DatagridControl.xaml
    /// </summary>
    public partial class DatagridControl : UserControl, BudgetItemsVisualizationInterface
    {

        Presenter presenter;
        public Presenter Presenter { set { presenter = value; } }

        public DatagridControl()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

            string searchString = txtSearch.Text;

            // Get the currently selected item if any, index will be -1 if nothing is selected
            int selectedIndex = dgbudgetItemsData.SelectedIndex;
            var displayedItems = dgbudgetItemsData.Items;

            int timesSeenIndexZero = 0;

            // Only search if the table isn't empty
            if (displayedItems.Count != 0)
            {

                // Look for the substring from that index forward
                // Break if the selected index has been reached OR
                // if 0 has been reached twice when no item was selected to begin with
                for (int i = (selectedIndex + 1) % displayedItems.Count; i != selectedIndex && timesSeenIndexZero < 2; i = (i + 1) % displayedItems.Count)
                {
                    // If this budget item's amount contains the search string, select it
                    if ((displayedItems[i] as BudgetItem).Amount.ToString("F2").IndexOf(searchString, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                    {
                        dgbudgetItemsData.SelectedIndex = i;
                        break;
                    }
                    // Using an else if instead of an OR for readability
                    if ((displayedItems[i] as BudgetItem).ShortDescription.IndexOf(searchString, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                    {
                        dgbudgetItemsData.SelectedIndex = i;
                        break;
                    }

                    if (i == 0)
                        timesSeenIndexZero++;
                }
            }

            // Make sure the selected item contains the string to be sure a match was found
            if (dgbudgetItemsData.SelectedIndex == -1 ||
                ((dgbudgetItemsData.SelectedItem as BudgetItem).Amount.ToString("F2").IndexOf(searchString, 0, StringComparison.CurrentCultureIgnoreCase) == -1) &&
                (dgbudgetItemsData.SelectedItem as BudgetItem).ShortDescription.ToString().IndexOf(searchString, 0, StringComparison.CurrentCultureIgnoreCase) == -1)
                MessageBox.Show("No match was found!", "No Match!", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                dgbudgetItemsData.ScrollIntoView(dgbudgetItemsData.SelectedItem);
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Check if the text is empty, if it is, disable the search button
            if (string.IsNullOrEmpty((sender as TextBox).Text))
                btnSearch.IsEnabled = false;
            else
                btnSearch.IsEnabled = true;
        }

        public void ChangeSelectedItemAfterDelete(int rowIndex)
        {
            dgbudgetItemsData.SelectedIndex = rowIndex;
        }

        public int GetSelectedItemIndexBeforeDelete()
        {
            if (dgbudgetItemsData.Items.Count == 1)
                return -1;

            if (dgbudgetItemsData.SelectedIndex == dgbudgetItemsData.Items.Count - 1)
                return dgbudgetItemsData.SelectedIndex - 1;

            return dgbudgetItemsData.SelectedIndex;


        }

        private void HideSearch()
        {
            txtSearch.Visibility = Visibility.Hidden;
            btnSearch.Visibility = Visibility.Hidden;
            lblSearch.Visibility = Visibility.Hidden;
        }

        private void ShowSearch()
        {
            txtSearch.Visibility = Visibility.Visible;
            btnSearch.Visibility = Visibility.Visible;
            lblSearch.Visibility = Visibility.Visible;
        }


        public void ShowBudgetItems(List<BudgetItem> budgetItems)
        {
            dgbudgetItemsData.ItemsSource = budgetItems;
            dgbudgetItemsData.Columns.Clear();
     
            ShowSearch();

            Dictionary<string, string> headerToBindingDictionary = new Dictionary<string, string>() {
                { "Date", "Date" },
                {"Category", "Category"},
                { "Description", "ShortDescription"},
                {"Amount", "Amount" },
                {"Balance", "Balance"},
            };
            SetDatagridTextColumnHeadersAndBinding(headerToBindingDictionary);



        }

        public void ShowBudgetItemsGroupedByMonth(List<BudgetItemsByMonth> budgetItemsByMonths)
        {
            dgbudgetItemsData.ItemsSource = budgetItemsByMonths;
            dgbudgetItemsData.Columns.Clear();
         


            Dictionary<string, string> headerToBindingDictionary = new Dictionary<string, string>()
            {
                { "Month", "Month" },

                { "Total", "Total" }
            };

            SetDatagridTextColumnHeadersAndBinding(headerToBindingDictionary);
            HideSearch();

        }

        public void ShowBudgetItemsGroupedByCategoryByMonth(List<Dictionary<string, object>> budgetItemsByCategoryByMonth)
        {
            dgbudgetItemsData.ItemsSource = budgetItemsByCategoryByMonth;
            dgbudgetItemsData.Columns.Clear();
            HideSearch();


            Dictionary<string, string> headerToBindingDictionary = new Dictionary<string, string>()
            {
                { "Month", "[Month]" }
            };


            List<string> categories = presenter.GetAllCategoryNames();
            for (int i = 0; i < categories.Count; i++)
            {
                try
                {
                    headerToBindingDictionary.Add(categories[i], $"[{categories[i]}]");
                }
                catch (Exception e)
                {
                    // Prevent expenses
                }
            }


            headerToBindingDictionary.Add("Total", "[Total]");

            SetDatagridTextColumnHeadersAndBinding(headerToBindingDictionary);

        }

        public void ShowBudgetItemsByCategory(List<BudgetItemsByCategory> budgetItemsByCategories)
        {
            dgbudgetItemsData.ItemsSource = budgetItemsByCategories;
            dgbudgetItemsData.Columns.Clear();
            HideSearch();

            Dictionary<string, string> headerToBindingDictionary = new Dictionary<string, string>() {
                { "Category", "Category" },
                {"Total", "Total"},
            };

            SetDatagridTextColumnHeadersAndBinding(headerToBindingDictionary);
        }

        private void SetDatagridTextColumnHeadersAndBinding(Dictionary<string, string> headerToBindingDictionary)
        {
            foreach (KeyValuePair<string, string> keyValuePair in headerToBindingDictionary)
            {
                DataGridTextColumn column = new DataGridTextColumn();
                column.Width = new DataGridLength(10, DataGridLengthUnitType.Auto);
                column.Header = keyValuePair.Key;
                column.Binding = new Binding(keyValuePair.Value);

                Style style = resourceDictionary["styleDatagridCell"] as Style;

                if (keyValuePair.Key == "Date")
                    column.Binding.StringFormat = "dd/MM/yyyy";
                else
                {
                    column.Binding.StringFormat = "F2";
                    try
                    {
                        style.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));
                    }
                    catch (Exception e)
                    {
                        // Try catch because this should only happen the first time the columns are made, throws an error because it's sealed
                    }
                }

                column.CellStyle = style;
                dgbudgetItemsData.Columns.Add(column);
            }
        }

        private void OpenEditExpenseWindow(object sender, RoutedEventArgs e)
        {
            BudgetItem itemToSelect = (sender as Button).DataContext as BudgetItem;
            int idToSelect = itemToSelect.ExpenseID;
            presenter.TriggerUpdate(itemToSelect);
            SelectEditedRowButton(idToSelect);
        }

        private void SelectEditedRowButton(int id)
        {
            for(int row = 0; row < dgbudgetItemsData.Items.Count; row++)
            {
                if ((dgbudgetItemsData.Items[row] as BudgetItem).ExpenseID == id)
                {
                    dgbudgetItemsData.SelectedIndex = row;
                    break;
                }
            }
        }

        private void ContextMenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            int deletedItemIndex = GetSelectedItemIndexBeforeDelete();
            presenter.DeleteExpense(((sender as MenuItem).DataContext as BudgetItem).ExpenseID);
            ChangeSelectedItemAfterDelete(deletedItemIndex);
        }

        private void DeleteButton_DeleteExpense(object sender, RoutedEventArgs e)
        {
            int deletedItemIndex = GetSelectedItemIndexBeforeDelete();
            presenter.DeleteExpense(((sender as Button).DataContext as BudgetItem).ExpenseID);
            ChangeSelectedItemAfterDelete(deletedItemIndex);
        }


        private void contextMenuEditItem_Click(object sender, RoutedEventArgs e)
        {
            BudgetItem budgetItem = (sender as MenuItem).DataContext as BudgetItem;
            presenter.TriggerUpdate(budgetItem);
            SelectEditedRowButton(budgetItem.ExpenseID);
        }


        public void ChangeColorsToMatchColourScheme(string filePath)
        {
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(filePath);
            string noFileNameFilePath = System.IO.Path.GetDirectoryName(filePath);
            string newFilePath = noFileNameFilePath + "\\" + fileNameWithoutExtension + "_ColorScheme.csv";
            Dictionary<string, Color> colors = ColorFileManager.LoadColorSchemeFromFile(newFilePath);

            foreach (KeyValuePair<string, Color> color in colors)
            {
                Brush bgColor = new SolidColorBrush(color.Value);

                switch (color.Key)
                {
                    case "Grid-Background":
                        Resources["budgetItemsBackground"] = bgColor;
                        Resources["cellBackground"] = bgColor;
                        Resources["rowBackground"] = bgColor;
                        break;
                    case "Button-Background":
                        Resources["mainButtons"] = bgColor;
                        break;
                    case "Label-Foreground":
                        Resources["mainLabels"] = bgColor;
                        Resources["borderColor"] = bgColor;
                        Resources["dataGridHeader"] = bgColor;
                        break;
                    case "TextBlock-Foreground":
                        Resources["mainTextBlocks"] = bgColor;
                        Resources["mainTextBoxes"] = bgColor;
                        Resources["mainComboBoxes"] = bgColor;
                        Resources["mainDatePicker"] = bgColor;
                        Resources["rowForeground"] = bgColor;
                        break;
                }
            }


        }

    }
}
