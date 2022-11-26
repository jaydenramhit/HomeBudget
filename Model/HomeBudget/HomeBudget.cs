using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Dynamic;
using System.Data.SQLite;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================


namespace Budget
{

    /// <summary>
    /// Represents an entire home budget. Manages and stores the <see cref="Categories">Categories</see> and <see cref="Expenses">Expenses</see> classes
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// string filename = "budgetFile.txt";
    /// // Try catch in case of file reading errors
    /// try{
    ///     // Open an existing budget
    ///     HomeBudget budget = new HomeBudget(databaseFile, false);
    ///     
    ///     // Add an expense and category
    ///     budget.expenses.Add(DateTime.Now, (int)Category.CategoryType.Credit, 4, "Protein Bar");
    ///     budget.categories.Add("Gas", Category.CategoryType.Expense);
    ///     
    ///     // Save the file
    ///     budget.SaveToFile(filename)
    /// }
    /// catch (Exception e){
    ///     Console.WriteLine(e.Message)
    /// }
    /// 
    /// ]]>
    /// </code>
    /// </example>
    public class HomeBudget
    {
        private Categories _categories;
        private Expenses _expenses;
        private SQLiteConnection dbConnection;


        // Properties (categories and expenses object)

        /// <summary>
        /// Gets the budget categories.
        /// </summary>
        public Categories categories { get { return _categories; } }
        /// <summary>
        /// Gets the budget expenses.
        /// </summary>
        public Expenses expenses { get { return _expenses; } }

       

        /// <summary>
        /// Creates a new HomeBudget object with a connection to a database file. If the <paramref name="newDB"/> flag is set to true,
        /// a new database file will be created.
        /// </summary>
        /// <param name="databaseFile">The name of the database file to connect to or create.</param>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// try{
        ///     HomeBudget homeBudget = new HomeBudget(databaseFile, false);
        ///     List<BudgetItem> budgetItems = homeBudget.GetBudgetItems(null, null, false, 0);
        /// }
        /// catch(Exception e){
        ///     Console.WriteLine(e.Message);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public HomeBudget(String databaseFile, bool newDB = false)
        {
            // if database exists, and user doesn't want a new database, open existing DB
            if (!newDB && File.Exists(databaseFile))
            {
                Database.existingDatabase(databaseFile);
            }
            // file did not exist, or user wants a new database, so open NEW DB
            else
            {
                Database.newDatabase(databaseFile);
                newDB = true;
            }
            // create the category object
            _categories = new Categories(Database.dbConnection, newDB);
            // create the _expenses course
            _expenses = new Expenses(Database.dbConnection);
            // save the database connection
            dbConnection = Database.dbConnection;
        }


        #region GetList


        /// <summary>
        /// Creates a list of BudgetItems and returns it. The new list will only contain BudgetItems that 
        /// were created between or at the <paramref name="Start"/> and <paramref name="End"/> dates.
        /// If the <paramref name="FilterFlag"/> is true, the returned list of BudgetItems will all
        /// be in the Category that matches the <paramref name="CategoryID"/>.
        /// </summary>
        /// <param name="Start">The earliest possible date of a budget item in the returned list.</param>
        /// <param name="End">The latest possible date of a budget item in the returned list.</param>
        /// <param name="FilterFlag">Indicated whether the list should filter to only contain BudgetItems that have a category that match the <paramref name="CategoryID"/></param>
        /// <param name="CategoryID">The category id that the list will exclusively contain if the <paramref name="FilterFlag"/> is true.</param>
        /// <returns>A list of the BudgetItems that met the criteria indicated in the arguments passed.</returns>
        /// <example>
        /// 
        /// Note: The output contains two ambiguous columns. The second last column is the value of that specific
        /// item (Amount), the last column is the total amount of expenses up to that item (Balance).
        /// <br/>For all examples below, assume the budget file contains the
        /// following elements:
        /// 
        /// <code>
        /// Cat_ID  Expense_ID  Date                    Description                    Cost
        ///    10       1       1/10/2018 12:00:00 AM   Clothes hat (on credit)         10
        ///     9       2       1/11/2018 12:00:00 AM   Credit Card hat                -10
        ///    10       3       1/10/2019 12:00:00 AM   Clothes scarf(on credit)        15
        ///     9       4       1/10/2020 12:00:00 AM   Credit Card scarf              -15
        ///    14       5       1/11/2020 12:00:00 AM   Eating Out McDonalds            45
        ///    14       7       1/12/2020 12:00:00 AM   Eating Out Wendys               25
        ///    14      10       2/1/2020 12:00:00 AM    Eating Out Pizza                33.33
        ///     9      13       2/10/2020 12:00:00 AM   Credit Card mittens            -15
        ///     9      12       2/25/2020 12:00:00 AM   Credit Card Hat                -25
        ///    14      11       2/27/2020 12:00:00 AM   Eating Out Pizza                33.33
        ///    14       9       7/11/2020 12:00:00 AM   Eating Out Cafeteria            11.11
        /// </code>
        /// 
        /// <b>Getting a list of ALL budget items.</b>
        /// 
        /// <code>
        /// <![CDATA[
        ///  HomeBudget budget = new HomeBudget(databaseFile, false);
        ///  
        ///  // Get a list of all budget items
        ///  var budgetItems = budget.GetBudgetItems(null, null, false, 0);
        ///            
        ///  // print important information
        ///  foreach (var bi in budgetItems)
        ///  {
        ///      Console.WriteLine ( 
        ///          String.Format("{0} {1,-20}  {2,8:C} {3,12:C}", 
        ///             bi.Date.ToString("yyyy/MMM/dd"),
        ///             bi.ShortDescription,
        ///             bi.Amount, bi.Balance)
        ///       );
        ///  }
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// 2018/Jan/10 hat (on credit)       ($10.00)     ($10.00)
        /// 2018/Jan/11 hat                     $10.00        $0.00
        /// 2019/Jan/10 scarf(on credit)      ($15.00)     ($15.00)
        /// 2020/Jan/10 scarf                   $15.00        $0.00
        /// 2020/Jan/11 McDonalds             ($45.00)     ($45.00)
        /// 2020/Jan/12 Wendys                ($25.00)     ($70.00)
        /// 2020/Feb/01 Pizza                 ($33.33)    ($103.33)
        /// 2020/Feb/10 mittens                 $15.00     ($88.33)
        /// 2020/Feb/25 Hat                     $25.00     ($63.33)
        /// 2020/Feb/27 Pizza                 ($33.33)     ($96.66)
        /// 2020/Jul/11 Cafeteria             ($11.11)    ($107.77)
        /// </code>
        /// 
        /// <b>Gets a list of items which are only in the category with id 10.</b>
        /// <code>
        /// <![CDATA[
        ///  HomeBudget budget = new HomeBudget(databaseFile, false);
        ///  
        ///  // Get a list of all budget items
        ///  var budgetItems = budget.GetBudgetItems(null, null, true, 10);
        ///            
        ///  // print important information
        ///  foreach (var bi in budgetItems)
        ///  {
        ///      Console.WriteLine ( 
        ///          String.Format("{0} {1,-20}  {2,8:C} {3,12:C}", 
        ///             bi.Date.ToString("yyyy/MMM/dd"),
        ///             bi.ShortDescription,
        ///             bi.Amount, bi.Balance)
        ///       );
        ///  }
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// 2018/Jan/10 hat (on credit)       ($10.00)     ($10.00)
        /// 2019/Jan/10 scarf(on credit)      ($15.00)     ($25.00)
        /// </code>
        /// <b>Getting a list of budget items between January 11th 2020 and February 25th 2020.</b>
        /// 
        /// <code>
        /// <![CDATA[
        ///  HomeBudget budget = new HomeBudget(databaseFile, false);
        ///  
        ///  // Get a list of all budget items
        ///  var budgetItems = budget.GetBudgetItems(new DateTime(2020/1/11), new DateTime(2020,2,25), false, 0);
        ///            
        ///  // print important information
        ///  foreach (var bi in budgetItems)
        ///  {
        ///      Console.WriteLine ( 
        ///          String.Format("{0} {1,-20}  {2,8:C} {3,12:C}", 
        ///             bi.Date.ToString("yyyy/MMM/dd"),
        ///             bi.ShortDescription,
        ///             bi.Amount, bi.Balance)
        ///       );
        ///  }
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// 2020/Jan/11 McDonalds             ($45.00)     ($45.00)
        /// 2020/Jan/12 Wendys                ($25.00)     ($70.00)
        /// 2020/Feb/01 Pizza                 ($33.33)    ($103.33)
        /// 2020/Feb/10 mittens                 $15.00     ($88.33)
        /// 2020/Feb/25 Hat                     $25.00     ($63.33)
        /// </code>
        /// </example>
        /// 
        public List<BudgetItem> GetBudgetItems(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // ------------------------------------------------------------------------
            // return joined list within time frame
            // ------------------------------------------------------------------------
            Start = Start ?? new DateTime(1900, 1, 1);
            End = End ?? new DateTime(2500, 1, 1);
            string stm = " select e.amount,c.Id , e.id, e.date ,c.Description , e.Description " +
                "from  categories c INNER join expenses e on c.Id =e.categoryId " +
               "where e.date >= @Start and e.date <= @End";
            double balance = 0;
            List<BudgetItem> items = new List<BudgetItem>();
            if (FilterFlag)
             stm += " AND e.categoryId = @CategoryID " +
                 " order by e.date ";
            else
            {
                stm += " order by e.date ";
            }


            SQLiteCommand cmd  = new SQLiteCommand(stm,dbConnection);
            cmd.Parameters.Add(new SQLiteParameter("@Start", Start.Value.ToString("yyyy-MM-dd")));
            cmd.Parameters.Add(new SQLiteParameter("@End",End.Value.ToString("yyyy-MM-dd")));
            cmd.Parameters.Add(new SQLiteParameter("@CategoryID", CategoryID));
            SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                balance += reader.GetDouble(0);
                items.Add(new BudgetItem {
                    CategoryID = reader.GetInt32(1),
                    ExpenseID = reader.GetInt32(2),
                    ShortDescription = reader.GetString(5),
                    Date = reader.GetDateTime(3),
                    Amount = reader.GetDouble(0),
                    Category =reader.GetString(4),
                    Balance = balance

                }); 


            }
            reader.Close();
            cmd.Dispose();

            return items;
        }


        /// <summary>
        /// Creates a list of BudgetItemsByMonth, which contains BudgetItems grouped by month and returns it. 
        /// The new list will only contain BudgetItems that  were created between or at the <paramref name="Start"/> 
        /// and <paramref name="End"/> dates. If the <paramref name="FilterFlag"/> is true, the BudgetItems in the BudgetItemsByMonth 
        /// list will all be in the Category that matches the <paramref name="CategoryID"/>.
        /// </summary>
        /// <param name="Start">The earliest possible date of a budget item in the returned list.</param>
        /// <param name="End">The latest possible date of a budget item in the returned list.</param>
        /// <param name="FilterFlag">Indicated whether the list should filter to only contain BudgetItems that have a category that match the <paramref name="CategoryID"/></param>
        /// <param name="CategoryID">The category id that the BudgetItems in the BudgetItemsByMonth list will exclusively contain if the <paramref name="FilterFlag"/> is true.</param>
        /// <returns>A list of BudgetItemsByMonth, which groups together the BudgetItems, sorted in chronological order that meet the criteria indicated in the arguments passed.</returns>
        /// <example>
        /// For all examples below, assume the budget file contains the
        /// following elements:
        /// 
        /// <code>
        /// Cat_ID  Expense_ID  Date                    Description                    Cost
        ///    10       1       1/10/2018 12:00:00 AM   Clothes hat (on credit)         10
        ///     9       2       1/11/2018 12:00:00 AM   Credit Card hat                -10
        ///    10       3       1/10/2019 12:00:00 AM   Clothes scarf(on credit)        15
        ///     9       4       1/10/2020 12:00:00 AM   Credit Card scarf              -15
        ///    14       5       1/11/2020 12:00:00 AM   Eating Out McDonalds            45
        ///    14       7       1/12/2020 12:00:00 AM   Eating Out Wendys               25
        ///    14      10       2/1/2020 12:00:00 AM    Eating Out Pizza                33.33
        ///     9      13       2/10/2020 12:00:00 AM   Credit Card mittens            -15
        ///     9      12       2/25/2020 12:00:00 AM   Credit Card Hat                -25
        ///    14      11       2/27/2020 12:00:00 AM   Eating Out Pizza                33.33
        ///    14       9       7/11/2020 12:00:00 AM   Eating Out Cafeteria            11.11
        /// </code>
        /// 
        /// <b>Display information about each Month of BudgetItems</b>
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(databaseFile, false);
        ///
        /// // Get a list of all budget items
        /// var budgetItemsByMonth = budget.GetBudgetItemsByMonth(null, null, false, 0);
        ///
        /// // print important information
        /// foreach (var bi in budgetItemsByMonth)
        /// {
        ///    Console.WriteLine(
        ///    String.Format("{0} {1,-20}  {2,8} {3,12:C}",
        ///    bi.Month, // The year and month grouped (YYYY/MM)
        ///    bi.Details.Count, // Number of expenses in this category
        ///    bi.Total // Total amount of the grouped expenses
        ///     )
        ///    );
        /// }
        /// ]]>
        /// </code>
        /// Sample output:
        /// <code>
        /// 2018/01                     2       $0.00
        /// 2019/01                     1      $15.00
        /// 2020/01                     3     ($55.00) 
        /// 2020/02                     4     ($26.66)
        /// 2020/07                     1     ($11.11)
        /// </code>
        /// 
        /// <b>Display information about each month of BudgetItems in a specific category</b>
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(databaseFile, false);
        ///
        /// // Get a list of all budget items
        /// var budgetItemsByMonth = budget.GetBudgetItemsByMonth(null, null, true, 10);
        ///
        /// // print important information
        /// foreach (var bi in budgetItemsByMonth)
        /// {
        ///    Console.WriteLine(
        ///    String.Format("{0} {1,-20}  {2,8} {3,12:C}",
        ///    bi.Month, // The year and month grouped (YYYY/MM)
        ///    bi.Details.Count, // Number of expenses in this category
        ///    bi.Total // Total amount of the grouped expenses
        ///     )
        ///    );
        /// }
        /// ]]>
        /// </code>
        /// Sample output:
        /// <code>
        /// 2018/01                     1      $10.00
        /// 2019/01                     1      $15.00
        /// </code>
        /// 
        /// <b>Display information about each category of BudgetItems from February 25th 2020 onward.</b>
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(databaseFile, false);
        ///
        /// // Get a list of all budget items
        /// var budgetItemsByMonth = budget.GetBudgetItemsByMonth(new Date(2020, 2, 25), null, false, 0);
        ///
        /// // print important information
        /// foreach (var bi in budgetItemsByMonth)
        /// {
        ///    Console.WriteLine(
        ///    String.Format("{0} {1,-20}  {2,8} {3,12:C}",
        ///    bi.Month, // The year and month grouped (YYYY/MM)
        ///    bi.Details.Count, // Number of expenses in this category
        ///    bi.Total // Total amount of the grouped expenses
        ///     )
        ///    );
        /// }
        /// ]]>
        /// </code>
        /// Sample output:
        /// <code>
        /// 2020/02                     2      ($8.33)
        /// 2020/07                     1     ($11.11)
        /// </code>
        /// </example>
        public List<BudgetItemsByMonth> GetBudgetItemsByMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {

            //-----------------------------------------------------------------------
            //Group by year/ month
            //-----------------------------------------------------------------------
           StringBuilder commandBuilder = new StringBuilder($"select SUM(amount), strftime(\"%Y/%m\", Date) as monthYear from expenses ");

            // Build the where clause
            if (FilterFlag || Start.HasValue || End.HasValue) {
                bool firstAdded = false;
                commandBuilder.Append("WHERE ");

                if (FilterFlag)
                {
                    if (firstAdded)
                        commandBuilder.Append("AND ");
                    commandBuilder.Append("categoryId = @catId ");
                    firstAdded = true;
                }

                if (Start.HasValue)
                {
                    if (firstAdded)
                        commandBuilder.Append("AND ");
                    commandBuilder.Append("date >= date(@startDate) ");
                    firstAdded = true;
                }

                if (End.HasValue)
                {
                    if (firstAdded)
                        commandBuilder.Append("AND ");
                    commandBuilder.Append("date <= date(@endDate) ");
                }
            }

            commandBuilder.Append("group by monthYear;");

            // Insert values into parameters
            SQLiteCommand command = new SQLiteCommand(commandBuilder.ToString(), dbConnection);
            command.Parameters.AddWithValue("@catId", CategoryID);
            if (Start.HasValue)
                command.Parameters.AddWithValue("@startDate", Start.Value.ToString("yyyy-MM-dd"));
            if (End.HasValue)
                command.Parameters.AddWithValue("@endDate", End.Value.ToString("yyyy-MM-dd"));

            SQLiteDataReader reader = command.ExecuteReader();

            // Read the grouped rows
            List<double> totals = new List<double>();
            List<string> dates = new List<string>();
            while (reader.Read())
            {
                totals.Add(reader.GetFloat(0));
                dates.Add(reader.GetString(1));
            }
            reader.Close();
            command.Dispose();

            // -----------------------------------------------------------------------
            // create new list
            // -----------------------------------------------------------------------
            var summary = new List<BudgetItemsByMonth>();

            for (int i = 0; i < totals.Count; i++)
            {
                int month = int.Parse(dates[i].Substring(5,2));
                int year = int.Parse(dates[i].Substring(0,4));
                int daysInMonth = DateTime.DaysInMonth(year, month);

                DateTime startDate = new DateTime(year, month, 1);
                DateTime endDate = new DateTime(year, month, daysInMonth);

                summary.Add(new BudgetItemsByMonth
                {
                    Month = dates[i],
                    Details = GetBudgetItems(startDate, endDate, FilterFlag, CategoryID),
                    Total = totals[i]
                });
            }

            return summary;
        }


        /// <summary>
        /// Creates a list of BudgetItemsByCategory, which contains BudgetItems grouped by category and returns it. 
        /// The new list will only contain BudgetItems that  were created between or at the <paramref name="Start"/> 
        /// and <paramref name="End"/> dates. If the <paramref name="FilterFlag"/> is true, the BudgetItems in the BudgetItemsByMonth 
        /// list will all be in the Category that matches the <paramref name="CategoryID"/>.
        /// Since the <paramref name="FilterFlag"/> filters to only contain one category, the returned list of BudgetItems
        /// will only have one entry if the <paramref name="FilterFlag"/> is true.
        /// </summary>
        /// <param name="Start">The earliest possible date of a budget item in the returned list.</param>
        /// <param name="End">The latest possible date of a budget item in the returned list.</param>
        /// <param name="FilterFlag">Indicated whether the list should filter to only contain BudgetItems that have a category that match the <paramref name="CategoryID"/></param>
        /// <param name="CategoryID">The category id that the BudgetItems in the BudgetItemsByMonth list will exclusively contain if the <paramref name="FilterFlag"/> is true.</param>
        /// <returns>A list of BudgetItemsByCategory, which groups together the BudgetItems, ordered by category name, that meet the criteria indicated in the arguments passed.</returns>
        /// <example>
        /// For all examples below, assume the budget file contains the
        /// following elements:
        /// 
        /// <code>
        /// Cat_ID  Expense_ID  Date                    Description                    Cost
        ///    10       1       1/10/2018 12:00:00 AM   Clothes hat (on credit)         10
        ///     9       2       1/11/2018 12:00:00 AM   Credit Card hat                -10
        ///    10       3       1/10/2019 12:00:00 AM   Clothes scarf(on credit)        15
        ///     9       4       1/10/2020 12:00:00 AM   Credit Card scarf              -15
        ///    14       5       1/11/2020 12:00:00 AM   Eating Out McDonalds            45
        ///    14       7       1/12/2020 12:00:00 AM   Eating Out Wendys               25
        ///    14      10       2/1/2020 12:00:00 AM    Eating Out Pizza                33.33
        ///     9      13       2/10/2020 12:00:00 AM   Credit Card mittens            -15
        ///     9      12       2/25/2020 12:00:00 AM   Credit Card Hat                -25
        ///    14      11       2/27/2020 12:00:00 AM   Eating Out Pizza                33.33
        ///    14       9       7/11/2020 12:00:00 AM   Eating Out Cafeteria            11.11
        /// </code>
        /// 
        /// <b>Display information about each category of BudgetItems</b>
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(databaseFile, false);
        ///
        /// // Get a list of all budget items
        /// var budgetItemsByCat = budget.GeBudgetItemsByCategory(null, null, false, 0);
        /// 
        /// // print important information
        /// foreach (var bi in budgetItemsByCat)
        /// {
        ///     Console.WriteLine(
        ///     String.Format("{0} {1,-10}  {2,8} {3,12:C}",
        ///     bi.Category, // The name of the category grouped
        ///     bi.Details.Count, // Number of expenses in this category
        ///     bi.Total // Total amount of the grouped expenses
        ///         )
        ///     );
        /// }
        /// ]]>
        /// </code>
        /// Sample output:
        /// <code>
        /// Credit Card                     4      $65.00
        /// Clothes                         2     ($25.00)
        /// Eating Out                      5    ($147.77)
        /// </code>
        /// 
        /// <b>Display information about a specific category of BudgetItems</b>
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(databaseFile, false);
        ///
        /// // Get a list of all budget items
        /// var budgetItemsByCat = budget.GeBudgetItemsByCategory(null, null, true, 10);
        /// 
        /// // print important information
        /// foreach (var bi in budgetItemsByCat)
        /// {
        ///     Console.WriteLine(
        ///     String.Format("{0} {1,-10}  {2,8} {3,12:C}",
        ///     bi.Category, // The name of the category grouped
        ///     bi.Details.Count, // Number of expenses in this category
        ///     bi.Total // Total amount of the grouped expenses
        ///         )
        ///     );
        /// }
        /// ]]>
        /// </code>
        /// Sample output:
        /// <code>
        /// Clothes                         2     ($25.00)
        /// </code>
        /// 
        /// <b>Display information about each category of BudgetItems between January 10th 2020 and February 25th 2020</b>
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(databaseFile, false);
        ///
        /// // Get a list of all budget items
        /// var budgetItemsByCat = budget.GeBudgetItemsByCategory(new DateTime(2020, 1, 10), new DateTime(2020, 2, 25), false, 0);
        /// 
        /// // print important information
        /// foreach (var bi in budgetItemsByCat)
        /// {
        ///     Console.WriteLine(
        ///     String.Format("{0} {1,-10}  {2,8} {3,12:C}",
        ///     bi.Category, // The name of the category grouped
        ///     bi.Details.Count, // Number of expenses in this category
        ///     bi.Total // Total amount of the grouped expenses
        ///         )
        ///     );
        /// }
        /// ]]>
        /// </code>
        /// Sample output:
        /// <code>
        /// Credit Card                     3      $55.00
        /// Eating Out                      3    ($103.33)
        /// </code>
        /// </example>
        public List<BudgetItemsByCategory> GeBudgetItemsByCategory(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            Start = Start ?? new DateTime(1900, 1, 1);
            End = End ?? new DateTime(2500, 1, 1);

            //Retrieving the Category(Description) and Total, in order to create a BudgetItemsByCategory instance
            string stm;

            if (!FilterFlag)
                stm = "SELECT sum(E.amount), C.Description, E.CategoryId from expenses E, categories C WHERE E.CategoryID = C.Id AND E.Date >= @Start AND E.Date <= @End GROUP BY E.CategoryId ORDER BY E.Date;";
            else
                stm = "SELECT sum(E.amount), C.Description, E.CategoryId from expenses E, categories C WHERE E.CategoryID = C.Id AND E.Date >= @Start AND E.Date <= @End AND E.CategoryId = @CatId GROUP BY E.CategoryId ORDER BY E.Date;";
           
            SQLiteCommand cmd = new SQLiteCommand(stm, dbConnection);
            cmd.Parameters.Add(new SQLiteParameter("@Start", Start.Value.ToString("yyyy-MM-dd")));
            cmd.Parameters.Add(new SQLiteParameter("@End", End.Value.ToString("yyyy-MM-dd")));

            if (FilterFlag)
                cmd.Parameters.Add(new SQLiteParameter("@CatId", CategoryID));

            SQLiteDataReader reader = cmd.ExecuteReader();

            // Use reader in order to add 
            // Use standard GetBudgetItems with filter flag in order to retrieve the Details (List of BudgetItems) need for BudgetItemsByCategory

            List<BudgetItemsByCategory> newList = new List<BudgetItemsByCategory>();
            while (reader.Read())
            {
                newList.Add(new BudgetItemsByCategory
                {
                    Category = reader.GetString(1),
                    Details = GetBudgetItems(Start, End, true, reader.GetInt32(2)),
                    Total = reader.GetDouble(0)
                });
            }

            reader.Close();
            cmd.Dispose();
            reader.Close();
            return newList;
        }


        /// <summary>
        /// Creates a list of dictionaries that contain details on all expenses grouped by <see cref="Category"/> and month.
        /// There is also a dictionary representing the total, which encompasses all the expenses.
        /// Each Dictionary has info on the year/month of the expenses, the total amount of the grouped expenses,
        /// the details of each item in the grouping, and the amount cost by each item. If the <paramref name="FilterFlag"/> is 
        /// set to true, only items in teh <see cref="Category"/> matching the <paramref name="CategoryID"/> will be in the returned 
        /// list of dictionaries.
        /// </summary>
        /// <param name="Start">The earliest possible date of an expense in the returned list.</param>
        /// <param name="End">The latest possible date of an expense in the returned list.</param>
        /// <param name="FilterFlag">Indicated whether the returned list should only contain expenses whose categories match <paramref name="CategoryID"/>.</param>
        /// <param name="CategoryID">The id that will exclusively be contained in the return list if <paramref name="FilterFlag"/> is true.</param>
        /// <returns>
        /// A list of dictionaries containing details on the expenses grouped by month and category.
        /// Each dictionary contains several key value pairs, "Month" gets the year and month of the expenses grouped in the dictionary,
        /// and "Total" returns the total expense of the items grouped in the dictionary. There are also key value pairs of
        /// "details:CATEGORY NAME" (where CATEGORY NAME is a name of a <see cref="Category"/>) which returns a list of budget items that fell in that category, and 
        /// "CATEGORY NAME" (where CATEGORY NAME is a name of a <see cref="Category"/>) which returns the total amount of all the items in the category passed.
        /// </returns>
        /// <example>
        /// For all examples below, assume the budget file contains the
        /// following elements:
        /// 
        /// <code>
        /// Cat_ID  Expense_ID  Date                    Description                    Cost
        ///    10       1       1/10/2018 12:00:00 AM   Clothes hat (on credit)         10
        ///     9       2       1/11/2018 12:00:00 AM   Credit Card hat                -10
        ///    10       3       1/10/2019 12:00:00 AM   Clothes scarf(on credit)        15
        ///     9       4       1/10/2020 12:00:00 AM   Credit Card scarf              -15
        ///    14       5       1/11/2020 12:00:00 AM   Eating Out McDonalds            45
        ///    14       7       1/12/2020 12:00:00 AM   Eating Out Wendys               25
        ///    14      10       2/1/2020 12:00:00 AM    Eating Out Pizza                33.33
        ///     9      13       2/10/2020 12:00:00 AM   Credit Card mittens            -15
        ///     9      12       2/25/2020 12:00:00 AM   Credit Card Hat                -25
        ///    14      11       2/27/2020 12:00:00 AM   Eating Out Pizza                33.33
        ///    14       9       7/11/2020 12:00:00 AM   Eating Out Cafeteria            11.11
        /// </code>
        /// 
        /// <b>Display information, distributed monthly, about each category of BudgetItems</b>
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(databaseFile, false);
        ///
        /// // Get a list of all budget items
        /// var budgetItemsByCategoryAndMonth = budget.GetBudgetDictionaryByCategoryAndMonth(null, null, false, 0);
        ///
        /// // print important information
        /// foreach (Dictionary<string, object> item in budgetItemsByCategoryAndMonth) {
        ///     string date = "";
        ///     decimal total = 0;
        ///     List<string> categories = new List<string>();
        ///     List<decimal> categoryTotals = new List<decimal>();
        ///
        ///        foreach (KeyValuePair<string, object> pair in item)
        ///        {
        ///            if (pair.Key.StartsWith("detail")) continue; // Skip the details
        ///
        ///            if (pair.Key == "Month") date = (string) pair.Value;
        ///            else if (pair.Key == "Total") total = decimal.Parse((string) pair.Value);
        ///            else
        ///            {
        ///                categories.Add(pair.Key);
        ///                categoryTotals.Add(decimal.Parse((string) pair.Value));
        ///            }
        ///         } 
        ///     
        ///     // Show the month and total of that month
        ///     Console.WriteLine(
        ///     String.Format("{0} {1,-20}  {2,8:C}",
        ///     date, total
        ///     ));
        ///    
        ///     // Show the details of each category in that month
        ///     for (int i = 0; i < categories.Count; i++)
        ///     {
        ///         Console.WriteLine(
        ///         String.Format("{0} {1,-10}  {2,8:C}",
        ///         categories[i], categoryTotals[i]
        ///         ));
        ///     }
        ///     
        ///     // Add spacing
        ///     Console.WriteLine();
        /// }
        /// ]]>
        /// </code>
        /// Sample output:
        /// <code>
        /// 2018/01             $0.00
        /// Clothes             $10.00 
        /// Credit Card        ($10.00)
        /// 
        /// 2019/01             $15.00
        /// Clothes             $15.00
        /// 
        /// 2020/01             $55.00
        /// Credit Card        ($15.00)
        /// Eating Out          $70.00
        /// 
        /// 2020/02             $26.66
        /// Eating Out          $66.66
        /// Credit Card        ($40.00)
        /// 
        /// 2020/07             $11.11
        /// Eating Out          $11.11
        /// </code>
        /// 
        /// <b>Display information, distributed monthly, about each category of BudgetItems,
        /// where each BudgetItem is in category 14.
        /// Only include BudgetItems created on or before February 1st 2020</b>
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(databaseFile, false);
        ///
        /// // Get a list of all budget items
        /// var budgetItemsByCategoryAndMonth = budget.GetBudgetDictionaryByCategoryAndMonth(null, new DateTime(2020,2,1), true, 10);
        ///
        /// // print important information
        /// foreach (Dictionary<string, object> item in budgetItemsByCategoryAndMonth) {
        ///     string date = "";
        ///     decimal total = 0;
        ///     List<string> categories = new List<string>();
        ///     List<decimal> categoryTotals = new List<decimal>();
        ///
        ///        foreach (KeyValuePair<string, object> pair in item)
        ///        {
        ///            if (pair.Key.StartsWith("detail")) continue; // Skip the details
        ///
        ///            if (pair.Key == "Month") date = (string) pair.Value;
        ///            else if (pair.Key == "Total") total = decimal.Parse((string) pair.Value);
        ///            else
        ///            {
        ///                categories.Add(pair.Key);
        ///                categoryTotals.Add(decimal.Parse((string) pair.Value));
        ///            }
        ///         } 
        ///     
        ///     // Show the month and total of that month
        ///     Console.WriteLine(
        ///     String.Format("{0} {1,-20}  {2,8:C}",
        ///     date, total
        ///     ));
        ///    
        ///     // Show the details of each category in that month
        ///     for (int i = 0; i < categories.Count; i++)
        ///     {
        ///         Console.WriteLine(
        ///         String.Format("{0} {1,-10}  {2,8:C}",
        ///         categories[i], categoryTotals[i]
        ///         ));
        ///     }
        ///     
        ///     // Add spacing
        ///     Console.WriteLine();
        /// }
        /// ]]>
        /// </code>
        /// Sample output:
        /// <code> 
        /// 2020/01             $70.00
        /// Eating Out          $70.00
        /// 
        /// 2020/02             $33.33
        /// Eating Out          $33.33
        /// </code>
        /// 
        /// <b>Display information, distributed monthly, about each category of BudgetItems.
        /// Only include BudgetItems between January 10th 2020 and February 25th 2020</b>
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget(databaseFile, false);
        ///
        /// // Get a list of all budget items
        /// var budgetItemsByCategoryAndMonth = budget.GetBudgetDictionaryByCategoryAndMonth(new DateTime(2020,1,10), DateTime(2020,2,25), false, 0);
        ///
        /// // print important information
        /// foreach (Dictionary<string, object> item in budgetItemsByCategoryAndMonth) {
        ///     string date = "";
        ///     decimal total = 0;
        ///     List<string> categories = new List<string>();
        ///     List<decimal> categoryTotals = new List<decimal>();
        ///
        ///        foreach (KeyValuePair<string, object> pair in item)
        ///        {
        ///            if (pair.Key.StartsWith("detail")) continue; // Skip the details
        ///
        ///            if (pair.Key == "Month") date = (string) pair.Value;
        ///            else if (pair.Key == "Total") total = decimal.Parse((string) pair.Value);
        ///            else
        ///            {
        ///                categories.Add(pair.Key);
        ///                categoryTotals.Add(decimal.Parse((string) pair.Value));
        ///            }
        ///         } 
        ///     
        ///     // Show the month and total of that month
        ///     Console.WriteLine(
        ///     String.Format("{0} {1,-20}  {2,8:C}",
        ///     date, total
        ///     ));
        ///    
        ///     // Show the details of each category in that month
        ///     for (int i = 0; i < categories.Count; i++)
        ///     {
        ///         Console.WriteLine(
        ///         String.Format("{0} {1,-10}  {2,8:C}",
        ///         categories[i], categoryTotals[i]
        ///         ));
        ///     }
        ///     
        ///     // Add spacing
        ///     Console.WriteLine();
        /// }
        /// ]]>
        /// </code>
        /// Sample output:
        /// <code>
        /// 2020/01             $55.00
        /// Credit Card        ($15.00)
        /// Eating Out          $70.00
        /// 
        /// 2020/02            ($6.67)
        /// Eating Out          $33.33
        /// Credit Card        ($40.00)
        /// </code>
        /// </example>
        public List<Dictionary<string,object>> GetBudgetDictionaryByCategoryAndMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items by month 
            // -----------------------------------------------------------------------
            List<BudgetItemsByMonth> GroupedByMonth = GetBudgetItemsByMonth(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // loop over each month
            // -----------------------------------------------------------------------
            var summary = new List<Dictionary<string, object>>();
            var totalsPerCategory = new Dictionary<String, Double>();

            foreach (var MonthGroup in GroupedByMonth)
            {
                // create record object for this month
                Dictionary<string, object> record = new Dictionary<string, object>();
                record["Month"] = MonthGroup.Month;
                record["Total"] = MonthGroup.Total;

                // break up the month details into categories
                var GroupedByCategory = MonthGroup.Details.GroupBy(c => c.Category);

                // -----------------------------------------------------------------------
                // loop over each category
                // -----------------------------------------------------------------------
                foreach (var CategoryGroup in GroupedByCategory.OrderBy(g => g.Key))
                {

                    // calculate totals for the cat/month, and create list of details
                    double total = 0;
                    var details = new List<BudgetItem>();

                    foreach (var item in CategoryGroup)
                    {
                        total = total + item.Amount;
                        details.Add(item);
                    }

                    // add new properties and values to our record object
                    record["details:" + CategoryGroup.Key] =  details;
                    record[CategoryGroup.Key] = total;

                    // keep track of totals for each category
                    if (totalsPerCategory.TryGetValue(CategoryGroup.Key, out Double CurrentCatTotal))
                    {
                        totalsPerCategory[CategoryGroup.Key] = CurrentCatTotal + total;
                    }
                    else
                    {
                        totalsPerCategory[CategoryGroup.Key] = total;
                    }
                }

                // add record to collection
                summary.Add(record);
            }
            // ---------------------------------------------------------------------------
            // add final record which is the totals for each category
            // ---------------------------------------------------------------------------
            Dictionary<string, object> totalsRecord = new Dictionary<string, object>();
            totalsRecord["Month"] = "TOTALS";

            foreach (var cat in categories.List())
            {
                try
                {
                    totalsRecord.Add(cat.Description, totalsPerCategory[cat.Description]);
                }
                catch { }
            }
            summary.Add(totalsRecord);


            return summary;
        }




        #endregion GetList

    }
}
