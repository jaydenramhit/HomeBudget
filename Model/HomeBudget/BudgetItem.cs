using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{

    /// <summary>
    /// Represents a single item that will contribute to a budget, along with it's category and expense.
    /// </summary>
    public class BudgetItem
    {
        /// <summary>
        /// Gets or sets the category id of the budget item.
        /// </summary>
        public int CategoryID { get; set; }
        /// <summary>
        /// Gets or sets the expense id of the budget item.
        /// </summary>
        public int ExpenseID { get; set; }
        /// <summary>
        /// Gets or sets the date of the budget item.
        /// </summary>
        /// <value>Represents the date the BudgetItem was purchased.</value>
        public DateTime Date { get; set; }
        /// <summary>
        /// Gets or sets the category of the budget item.
        /// </summary>
        public String Category { get; set; }
        /// <summary>
        /// Gets or sets the short description of the budget item.
        /// </summary>
        public String ShortDescription { get; set; }
        /// <summary>
        /// Gets or sets the amount of the budget item.
        /// </summary>
        public Double Amount { get; set; }
        /// <summary>
        /// Gets or sets the balance when this budget item was created.
        /// </summary>
        public Double Balance { get; set; }

    }

    /// <summary>
    /// Represents all the BudgetItems for one month with their total price.
    /// </summary>
    public class BudgetItemsByMonth
    {
        /// <summary>
        /// Gets or sets the month encompassed by the budget items by month object.
        /// </summary>
        public String Month { get; set; }
        /// <summary>
        /// Gets or sets the details about the budget items in this month.
        /// </summary>
        public List<BudgetItem> Details { get; set; }
        /// <summary>
        /// Gets or sets the total of the budget items in the budget items by month object.
        /// </summary>
        public Double Total { get; set; }
    }

    /// <summary>
    /// Represents all the BudgetItems for a category with their total price.
    /// </summary>
    public class BudgetItemsByCategory
    {
        /// <summary>
        /// Gets or sets the category encompassed by the budget items by category object.
        /// </summary>
        public String Category { get; set; }
        /// <summary>
        /// Gets or sets the details about the budget items by category object.
        /// </summary>
        public List<BudgetItem> Details { get; set; }
        /// <summary>
        /// Gets or sets the total price of the budget items in the budget items by category object.
        /// </summary>
        public Double Total { get; set; }

    }


}
