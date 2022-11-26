using Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HomeBudgetWPF
{

   public interface MainViewInterface
    {

        /// <summary>
        /// Resets the category addition form.
        /// </summary>
        void ClearCategoryForm();
     
        /// <summary>
        /// Displays a generic error to the user.
        /// </summary>
        /// <param name="error">The error to display</param>
        void ShowError(string error);
     
        /// <summary>
        /// Displays an error indicating that the input used to create an expense was invalid, and shows which arguments were invalid.
        /// </summary>
        /// <param name="error">The error to be displayed.</param>
        /// <param name="invalidArgumentPosition">The position of the arguments passed to the presenter method that were invalid (0 index).</param>
        void ShowExpensesAddError(string error, int[] invalidArgumentPosition);


        /// <summary>
        /// Displays an error indicating that the input used to create a category was invalid, and shows which arguments were invalid.
        /// </summary>
        /// <param name="error">The error to be displayed.</param>
        /// <param name="invalidArgumentPosition">The position of the arguments passed to the presenter method that were invalid (0 index).</param>
        void ShowCategoriesAddError(string error, int[] invalidArgumentPosition);

        /// <summary>
        /// Clears the input used to add an expense.
        /// </summary>
        void ClearExpenseForm();

        /// <summary>
        /// Populates the combo box with categories.
        /// </summary>
        /// <param name="categories">The list of Category objects to give as options to the user.</param>
        void PopulateCategories(List<Category> categories);

        /// <summary>
        /// Writes a message to the label below submission button if 
        /// the category was successfully added
        /// </summary>
        /// <param name="message">the successfully added message</param>
        void SuccessfullyAddedCategory(string message);
        /// <summary>
        /// Writes a message to the label below submission button if 
        /// the expense was successfully added
        /// </summary>
        /// <param name="message">the successfully added message</param>
        void SuccessfullyAddedExpense(string message);

        /// <summary>
        /// Calls the display budget items method in the presenter with the latest filtering and grouping
        /// to display the type of budget items summary, and / or filtering that the user selected.
        /// </summary>
        void DisplayGroupedOrFilteredBudgetItems();

        /// <summary>
        /// Allows the user to update the provided budget item.
        /// </summary>
        /// <param name="budgetItem">The budget item to be updated.</param>
        void UpdateBudgetItem(BudgetItem budgetItem);

    }

}
