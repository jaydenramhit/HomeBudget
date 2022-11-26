using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget;

namespace EnterpriseBudget.DeptBudgets.OldFiles
{
    public interface BudgetItemsVisualizationInterface
    {

        /// <summary>
        /// Changes the display colours of the view to match the application's colour scheme.
        /// </summary>
        /// <param name="filePath">The filepath to the colour scheme file.</param>
        void ChangeColorsToMatchColourScheme(string filePath);

        /// <summary>
        /// Displays the <paramref name="budgetItems"/> to the user.
        /// </summary>
        /// <param name="budgetItems">The budget items to display.</param>
        void ShowBudgetItems(List<BudgetItem> budgetItems);

        /// <summary>
        /// Displays the <paramref name="budgetItemsByMonth"/> to the user./>
        /// </summary>
        /// <param name="budgetItemsByMonth"> The budget items grouped by month to display</param>
        void ShowBudgetItemsGroupedByMonth(List<BudgetItemsByMonth> budgetItemsByMonth);

        /// <summary>
        /// Display the <paramref name="budgetItemsByCategoryByMonth"/>  to the user./>
        /// </summary>
        /// <param name="budgetItemsByCategoryByMonth">The budget items grouped by month and category to display</param>
        void ShowBudgetItemsGroupedByCategoryByMonth(List<Dictionary<string, object>> budgetItemsByCategoryByMonth);

        /// <summary>
        /// Displays the <paramref name="budgetItemsByCategories"/> to the user.
        /// </summary>
        void ShowBudgetItemsByCategory(List<BudgetItemsByCategory> budgetItemsByCategories);

    }
}
