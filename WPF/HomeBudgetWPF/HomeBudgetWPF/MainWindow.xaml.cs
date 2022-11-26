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

namespace HomeBudgetWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, MainViewInterface
    {
        Presenter presenter;
        private string filePath;

        public MainWindow(string filePath, bool newFile)
        {
            InitializeComponent();
            CatDropDownInitialization();
            this.filePath = filePath;
            presenter = new Presenter(this, filePath, newFile);
            ClearExpenseForm();
            ChangeColors();
        }


        // This method is called when the MainWindow is closed.
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Unsaved changes for expense form

            bool unsavedChanges = !String.IsNullOrEmpty(txtExpenseAmount.Text) ||
                (datExpenseDate.SelectedDate.HasValue &&
                datExpenseDate.SelectedDate != DateTime.Today) || 
                !String.IsNullOrEmpty(txtExpenseDescription.Text);

            if (unsavedChanges)
            {
                System.Windows.MessageBoxResult input = MessageBox.Show("There may be unsaved changes, would you like to quit anyways?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (input == MessageBoxResult.No)
                    e.Cancel = true;
            }
        }

        private void CatDropDownInitialization()
        {
            
            foreach (var val in Enum.GetValues(typeof(Category.CategoryType)))
            {
                cmbCatTypes.Items.Add(val);
            }

            cmbCatTypes.SelectedItem = Category.CategoryType.Expense;
        }

        private void btnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            string categoryDescription = txtCatInput.Text;
            Category.CategoryType categoryType = (Category.CategoryType)cmbCatTypes.SelectedItem;

            presenter.AddCategory(categoryDescription, categoryType);
        }

        public void ClearCategoryForm()
        {
            cmbCatTypes.SelectedItem = Category.CategoryType.Expense;
            txtCatInput.Clear();
           
        }
        public void SuccessfullyAddedCategory(string message)
        {
            categoriesSubmitInfo.Content = message;
        }

        public void SuccessfullyAddedExpense(string message)
        {
            expensesSubmitInfo.Content = message;
        }

        public void ShowError(string error)
        {
            MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowExpensesAddError(string error, int[] invalidArguments)
        {
            expensesSubmitInfo.Content = error;
        }

        public void ShowCategoriesAddError(string error, int[] invalidArgumentPosition)
        {
            categoriesSubmitInfo.Content = error;
        }


        private void btnResetExpenseForm_Click(object sender, RoutedEventArgs e)
        {
            ClearExpenseForm();
        }

        private void btnAddExpense_Click(object sender, RoutedEventArgs e)
        {
            
            int categoryId = (cboExpenseCategory.SelectedItem as Category).Id;
            double amount;
            
            string description = txtExpenseDescription.Text;

            if (double.TryParse(txtExpenseAmount.Text, out amount) && datExpenseDate.SelectedDate.HasValue)
            {
                DateTime date = (DateTime)datExpenseDate.SelectedDate;
                presenter.AddExpense(amount, categoryId, date, description);
            }
            else
            {
                if(!datExpenseDate.SelectedDate.HasValue)
                    ShowExpensesAddError("-A valid date must be selected.", new int[] { 2 });
                else
                    ShowExpensesAddError("-Amount must be a number.", new int[] { 0 });
            }
        }

        public void ClearExpenseForm()
        {
            txtExpenseAmount.Clear();
            cboExpenseCategory.SelectedIndex = 0;
            datExpenseDate.SelectedDate = DateTime.Today;
            txtExpenseDescription.Clear();
            //expensesSubmitInfo.Content = string.Empty;
        }
        public void PopulateCategories(List<Category> categories)
        {
            cboExpenseCategory.DisplayMemberPath = "Description";
            cboExpenseCategory.ItemsSource = categories;
            cboExpenseCategory.SelectedIndex = 0;
            cboFilterCategories.DisplayMemberPath = "Description";
            cboFilterCategories.ItemsSource = categories;
            cboFilterCategories.SelectedIndex = 0;
        }

        private void miColorScheme_Click(object sender, RoutedEventArgs e)
        {
            ColorSchemeWindow colorSchemeWindow = new ColorSchemeWindow(this, filePath);
            colorSchemeWindow.Show();
        }


        public void DisplayGroupedOrFilteredBudgetItems()
        {

            // Check the grouping
            bool groupByMonth = false;
            bool groupByCategory = false;
            if (chkGroupByMonth.IsChecked.HasValue)
                groupByMonth = chkGroupByMonth.IsChecked.Value;
            if (chkGroupByCategory.IsChecked.HasValue)
                groupByCategory = chkGroupByCategory.IsChecked.Value;

            // Get filtering values
            DateTime? startDate = datFilterStart.SelectedDate;
            DateTime? endDate = datFilterEnd.SelectedDate;
            bool filterByCategory = false;
            int filteredCategoryId = cboFilterCategories.SelectedIndex + 1;

            if (chkFilterCategory.IsChecked.HasValue)
                filterByCategory = chkFilterCategory.IsChecked.Value;

            SetDataVisualizationViewInPresenter();
            presenter.DisplayGroupedOrFilteredBudgetItems(groupByMonth, groupByCategory, startDate, endDate, filterByCategory, filteredCategoryId);

        }

        public void ChangeColors()
        {
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(filePath);
            string noFileNameFilePath = System.IO.Path.GetDirectoryName(filePath);
            string newFilePath = noFileNameFilePath + "\\" + fileNameWithoutExtension + "_ColorScheme.csv";
            Dictionary<string, Color> colors = ColorFileManager.LoadColorSchemeFromFile(newFilePath);
            
            foreach (KeyValuePair<string,Color> color in colors)
            {
                Brush bgColor = new SolidColorBrush(color.Value);

                switch (color.Key)
                {
                    case "Grid-Background":
                        grdBackground.Background = bgColor;
                        Resources["budgetItemsBackground"] = bgColor;
                        //Resources["cellBackground"] = bgColor;
                        //Resources["rowBackground"] = bgColor;
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

            // Set the colours for both so they have the colour when they come into view
            dataVisualisationElementPieChart.ChangeColorsToMatchColourScheme(filePath);
            dataVisualisationElementDatagrid.ChangeColorsToMatchColourScheme(filePath);

        }

        private void FilterElement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if presenter is null because this method can be called right when the window starts
            // and the presenter may not be instantiated yet
            if(presenter != null)
                DisplayGroupedOrFilteredBudgetItems();
        }

        private void chkFilterCategory_CheckedOrUnchecked(object sender, RoutedEventArgs e)
        {
            // Check if presenter is null because this method can be called right when the window starts
            // and the presenter may not be instantiated yet
            DisplayGroupedOrFilteredBudgetItems();
        }
        

        private void chkGroupByMonth_Click(object sender, RoutedEventArgs e)
        {
            DisplayGroupedOrFilteredBudgetItems();
        }

        private void chkGroupByCategory_Click(object sender, RoutedEventArgs e)
        {
            DisplayGroupedOrFilteredBudgetItems();
        }


        public void UpdateBudgetItem(BudgetItem budgetItem)
        {
            ExpenseEditWindow editWindow = new ExpenseEditWindow(
                budgetItem.ExpenseID,
                budgetItem.Date,
                budgetItem.Amount,
                budgetItem.CategoryID,
                budgetItem.ShortDescription,
                cboExpenseCategory.ItemsSource as List<Category>,
                presenter,
                filePath
            );
            editWindow.ShowDialog();
            DisplayGroupedOrFilteredBudgetItems();
        }

        private void dataVisualisationElement_Loaded(object sender, RoutedEventArgs e)
        {
            ShowPieChartOrDataGrid();
        }

        private void chkPieChart_Click(object sender, RoutedEventArgs e)
        {
            ShowPieChartOrDataGrid();
        }

        private void ShowPieChartOrDataGrid()
        {
            // If the pie chart checkbox is not checked or has no value
            if (chkPieChart.IsChecked.HasValue && chkPieChart.IsChecked.Value)
            {
                dataVisualisationElementPieChart.Presenter = presenter;
                dataVisualisationElementPieChart.ChangeColorsToMatchColourScheme(filePath);
                scrollbarViewerPieChart.Visibility = Visibility.Visible;
                dataVisualisationElementDatagrid.Visibility = Visibility.Hidden;
            }
            else
            {
                dataVisualisationElementDatagrid.Presenter = presenter;
                dataVisualisationElementDatagrid.ChangeColorsToMatchColourScheme(filePath);
                scrollbarViewerPieChart.Visibility = Visibility.Hidden;
                dataVisualisationElementDatagrid.Visibility = Visibility.Visible;
            }
            SetDataVisualizationViewInPresenter();

            // This is the line that shows the budget items on startup
            DisplayGroupedOrFilteredBudgetItems();
            
        }

        private void SetDataVisualizationViewInPresenter()
        {
            if (chkPieChart.IsChecked.HasValue && chkPieChart.IsChecked.Value)
                presenter.DataVisualizationView = dataVisualisationElementPieChart;
            else
                presenter.DataVisualizationView = dataVisualisationElementDatagrid;
        }

    }
}
