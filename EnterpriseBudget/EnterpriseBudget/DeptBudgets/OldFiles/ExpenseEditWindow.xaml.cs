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
using Budget;

namespace EnterpriseBudget.DeptBudgets.OldFiles
{
    /// <summary>
    /// Interaction logic for ExpenseEditWindow.xaml
    /// </summary>
    public partial class ExpenseEditWindow : Window, EditViewInterface
    {

        Presenter presenter;
        int expenseId;

        public ExpenseEditWindow(int expenseId, DateTime expenseDate, double expenseAmount, int categoryId, string description, List<Category> categories, Presenter presenter)
        {
            InitializeComponent();
            presenter.EditView = this;
            this.presenter = presenter;
            cboExpenseCategory.ItemsSource = categories;
            cboExpenseCategory.SelectedIndex = categoryId - 1;
            datExpenseDate.SelectedDate = expenseDate;
            txtExpenseAmount.Text = expenseAmount.ToString();
            txtExpenseDescription.Text = description;
            this.expenseId = expenseId;
        }

        public void ChangeColors(string filepath)
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
                        grdMain.Background = bgColor;
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
        }

            private void btnCancelExpenseEdit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnEditExpense_Click(object sender, RoutedEventArgs e)
        {

            int categoryId = (cboExpenseCategory.SelectedItem as Category).Id;
            double amount;
            string description = txtExpenseDescription.Text;

            if (double.TryParse(txtExpenseAmount.Text, out amount) && datExpenseDate.SelectedDate.HasValue)
            {
                DateTime date = (DateTime)datExpenseDate.SelectedDate;
                presenter.EditExpense(expenseId, date, categoryId, amount, description);
            }
            else
            {
                if(!datExpenseDate.SelectedDate.HasValue)
                    DisplayEditFail("-A date must be selected.");
                else
                    DisplayEditFail("-Amount must be a number.");
            }
        }

        public void DisplayEditFail(string message)
        {
            expensesSubmitInfo.Content = message;
        }

        public void IndicateSuccessfulEdit()
        {
            this.Close();
        }

    }
}
