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
    /// Represents an individual category for the budget program. 
    /// </summary>
    public class Category
    {
        // ====================================================================
        // Properties
        // ====================================================================

        /// <summary>
        /// Gets the id of the category.
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// Gets the description of the category.
        /// </summary>
        public String Description { get;  }

        /// <summary>
        /// Gets the type of the category.
        /// </summary>
        public CategoryType Type { get; }

        /// <summary>
        /// Reresents the the type or purpose of a category.
        /// </summary>
        public enum CategoryType
        {
            /// <summary>
            /// Indicates that a category type is associated with income.
            /// </summary>
            Income = 1,
            /// <summary>
            /// Indicates that a category type is associated with expense.
            /// </summary>
            Expense,
            /// <summary>
            /// Indicates that a category type is associated with credit.
            /// </summary>
            Credit,
            /// <summary>
            /// Indicates that a category type is associated with savings.
            /// </summary>
            Savings
        };

        // ====================================================================
        // Constructors
        // ====================================================================

        /// <summary>
        /// Creates a Category object with the passed <paramref name="id"/>, <paramref name="description"/> and <paramref name="type"/>. 
        /// If no type is passed, the new object's type will be set to <see cref="CategoryType.Expense"/> by default.
        /// </summary>
        /// <param name="id">The id of the new Category object.</param>
        /// <param name="description">The description of the new Category object.</param>
        /// <param name="type">The type of the new Category object.</param>
        /// <example>
        /// <b>Create Category objects to add to a list</b>
        /// <code>
        /// <![CDATA[
        /// List<Category> categories = new List<Category>();
        /// categories.Add(new Category(1, "Gas"));
        /// categories.Add(new Category(2, "Credit Card", CategoryType.Credit));
        /// ]]>
        /// </code>
        /// </example>
        public Category(int id, String description, CategoryType type = CategoryType.Expense)
        {
            this.Id = id;
            this.Description = description;
            this.Type = type;
        }


        /// <summary>
        /// Creates a new category object with the same values as the Category object passed as an argument.
        /// </summary>
        /// <param name="category">The Category containing the values that will be copied to the new Category object.</param>
        /// <example>
        /// <b>Copy an existing category to make a change while keeping the original.</b>
        /// <code>
        /// <![CDATA[
        /// Category birthdayCategory = new Category(1, "Birthday Money", CategoryType.Savings);
        /// Category christmasCategory = new Category(birthdayCategory);
        /// 
        /// christmasCategory.Id++;
        /// christmasCategory.Description = "Christmas Money";
        /// 
        /// Console.WriteLine(birthdayCategory);
        /// Console.WriteLine(christmasCategory);
        /// ]]>
        /// </code>
        /// Sample Output
        /// <code>
        /// Birthday Money
        /// Christmas Money
        /// </code>
        /// </example>
        public Category(Category category)
        {
            this.Id = category.Id;
            this.Description = category.Description;
            this.Type = category.Type;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>The Category's description.</returns>
        /// <example>
        /// <b>Display the details of each Category in a list.</b>
        /// <code>
        /// <![CDATA[
        /// Categories cats = new Categories();
        /// 
        /// foreach (Category cat in cats.List()){
        ///     Console.WriteLine(cat.ToString());
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public override string ToString()
        {
            return Description;
        }

    }
}

