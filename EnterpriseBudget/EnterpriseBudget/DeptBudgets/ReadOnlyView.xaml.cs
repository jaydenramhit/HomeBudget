﻿using Budget;
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

namespace EnterpriseBudget.DeptBudgets
{
    /// <summary>
    /// View for DeptBudgets.ReadOnlyView
    /// </summary>
    public partial class ReadOnlyView : Window, InterfaceView, OldFiles.MainViewInterface
    {
        /// <summary>
        /// Presenter logic for DeptBudgets.ReadOnlyView
        /// </summary>
        public Presenter presenter { get; set; }

        /// <summary>
        /// view for the mainControl (starting point for the app)
        /// </summary>
        public MainControl.InterfaceView mainControl { get; set; }

        /// <summary>
        /// Standard windows constructor
        /// </summary>
        public ReadOnlyView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The window is about to close, cleans up anything
        /// that needs to be done before closing
        /// </summary>
        public void TidyUpAndClose()
        {
            this.Close();
        }

        // User has asked to return to main page
        private void ReturnButton_Clicked(object sender, RoutedEventArgs e)
        {
            TidyUpAndClose();
        }

        // the window is closing
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mainControl != null)
            {
                mainControl.ComeBackToForeground();
            }
            else
            {
                MessageBox.Show("You did not set mainControl - You need to fix this bug", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ClearCategoryForm()
        {
            throw new NotImplementedException();
        }

        public void ShowError(string error)
        {
            throw new NotImplementedException();
        }

        public void ShowExpensesAddError(string error, int[] invalidArgumentPosition)
        {
            throw new NotImplementedException();
        }

        public void ShowCategoriesAddError(string error, int[] invalidArgumentPosition)
        {
            throw new NotImplementedException();
        }

        public void ClearExpenseForm()
        {
            throw new NotImplementedException();
        }

        public void PopulateCategories(List<Category> categories)
        {
            throw new NotImplementedException();
        }

        public void SuccessfullyAddedCategory(string message)
        {
            throw new NotImplementedException();
        }

        public void SuccessfullyAddedExpense(string message)
        {
            throw new NotImplementedException();
        }

        public void DisplayGroupedOrFilteredBudgetItems()
        {
            throw new NotImplementedException();
        }

        public void UpdateBudgetItem(BudgetItem budgetItem)
        {
            throw new NotImplementedException();
        }
    }
}
