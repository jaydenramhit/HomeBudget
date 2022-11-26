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
    /// Represence an individual expense for the budget program.
    /// </summary>
    public class Expense
    {
        // ====================================================================
        // Properties
        // ====================================================================

        /// <summary>
        /// Gets the expense's id.
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// Gets the date of the expense.
        /// </summary>
        public DateTime Date { get;  }
        /// <summary>
        /// Gets the amount of the expense.
        /// </summary>
        public Double Amount { get;}
        /// <summary>
        /// Gets a description of the expense.
        /// </summary>
        public String Description { get;}
        /// <summary>
        /// Gets the category of the expense.
        /// </summary>
        public int Category { get;}

        /// <summary>
        /// Instantiates a new Expense object with the values passed.
        /// </summary>
        /// <param name="id">The id of the new Expense.</param>
        /// <param name="date">The date of the new Expense.</param>
        /// <param name="category">The category of the new Expense.</param>
        /// <param name="amount">The amount of the new Expense.</param>
        /// <param name="description">The description of the new Expense.</param>
        /// <example>
        /// <b>Create a list of expenses.</b>
        /// <code>
        /// <![CDATA[
        /// List<Expense> expenses = new List<Expenses>();
        /// expenses.Add(new Expense(1, DateTime.Now, Category.CategoryType.Expense, 14.99, "Subway"))
        /// expenses.Add(new Expense(2, DateTime.Now, Category.CategoryType.Credit, 20.17, "Pizza Hut"))
        /// 
        /// foreach (Expense e in expenses){
        ///     Console.WriteLine(e.Description);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public Expense(int id, DateTime date, int category, Double amount, String description)
        {
            this.Id = id;
            this.Date = date;
            this.Category = category;
            this.Amount = amount;
            this.Description = description;
        }


        /// <summary>
        /// Creates a new Expense object with the same properties as the Expense object passed (<paramref name="obj"/>)
        /// </summary>
        /// <param name="obj">The Expense object to use to create the new Expense.</param>
        /// <example>
        /// <b>Copy and change an expense, leaving the original unchanged.</b>
        /// <code>
        /// <![CDATA[
        /// Expense pizzaExpense = new Expense(1, DateTime.Now, Category.CategoryType.Credit, 20.17, "Pizza Hut")
        /// Expense iceCreamExpense = new Expense(pizzaExpense);
        /// 
        /// iceCreamExpense.Id++;
        /// iceCreamExpense.Description = "Ice Cream";
        /// iceCreamExpense.Amount = 7.98;
        /// 
        /// Console.Log(pizzaExpense.Description);
        /// Console.Log(iceCreamExpense.Description);
        /// ]]>
        /// </code>
        /// Sample Output
        /// <code>
        /// Pizza Hut
        /// Ice Cream
        /// </code>
        /// </example>
        public Expense (Expense obj)
        {
            this.Id = obj.Id;
            this.Date = obj.Date;
            this.Category = obj.Category;
            this.Amount = obj.Amount;
            this.Description = obj.Description;
           
        }
    }
}
