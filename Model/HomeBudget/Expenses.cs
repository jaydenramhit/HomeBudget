using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Data.SQLite;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{
    /// <summary>
    /// Represents a collection of expense items inside the homebudget database
    /// </summary>
    public class Expenses
    {
        
        private SQLiteConnection databaseConnection;




        /// <summary>
        /// Creates an expenses object with a database connection
        /// </summary>
        /// <param name="connection">connection to the database</param>

        public Expenses(SQLiteConnection connection)
        {
            this.databaseConnection = connection;


        }


    
        /// <summary>
        /// Adds new expense to the list of expenses with the properties passed to the function.
        /// </summary>
        /// <param name="date">The date of the new expense.</param>
        /// <param name="category">The category of the new expense.</param>
        /// <param name="amount">The amount of the new expense.</param>
        /// <param name="description">The description of the new expense.</param>
        /// <example>
        /// <b>Add an Expense to an Expenses object.</b>
        /// <code>
        /// <![CDATA[
        /// try{
        ///     HomeBudget homeBudget = new HomeBudget(dbfilename,false);
        ///    
        ///     Expenses expenses = homeBudget.expenses;    
        /// 
        ///     // Add expenses
        ///     expenses.Add(DateTime.Now, (int)Category.CategoryType.Expenses, 50.76, "Gas");
        ///     expenses.Add(DateTime.Now, (int)Category.CategoryType.Savings, 160.32, "Part Time Job");
        ///     expenses.Add(DateTime.Now, (int)Category.CategoryType.Savings, 890.43, "Full Time Job");
        ///     expenses.Add(DateTime.Now, (int)Category.CategoryType.Credit, 15.21, "Domino's Pizza");
        ///     
        ///  
        /// }
        /// catch (Exception e){
        ///     Console.WriteLine(e.Message);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public void Add(DateTime date, int category, Double amount, String description)
        {
            // Get the last highest primary key
            string stm = "SELECT Id from expenses order by Id desc limit 1;";
            SQLiteCommand cmd = new SQLiteCommand(stm, databaseConnection);
            SQLiteDataReader reader = cmd.ExecuteReader();

            int newId = 1;
            if (reader.Read())
                newId = reader.GetInt32(0) + 1;
            reader.Close();

            stm = "INSERT INTO expenses (Id, Date, Description, Amount, CategoryId)" +
                  "values (@id, @date, @description, @amount, @categoryId);";

            string dateString = date.ToString("yyyy-MM-dd");
            cmd = new SQLiteCommand(stm, databaseConnection);
            cmd.Parameters.AddWithValue("@id", newId);
            cmd.Parameters.AddWithValue("@date", dateString);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@categoryId", category);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

        }

        /// <summary>
        /// Removes an expense with the matching <paramref name="Id"/> if it exists.
        /// </summary>
        /// <param name="Id">The id of the expense to delete from the list.</param>
        /// <example>
        /// <b>Remove the Expense with id 10.</b>
        /// <code>
        /// <![CDATA[
        /// try{
        ///    HomeBudget homeBudget = new HomeBudget(dbfilename,false);
        ///     
        ///     // Shipment was cancelled, so remove the expense.
        ///     homeBudget.expenses.Delete(10);
        ///     
        ///     
        ///     
        /// }
        /// catch (Exception e){
        ///     Console.WriteLine(e.Message); 
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public void Delete(int Id)
        {
            string stm = "DELETE FROM expenses WHERE Id = @Id;";

            SQLiteCommand cmd = new SQLiteCommand(stm, databaseConnection);
            cmd.Parameters.Add(new SQLiteParameter("@Id", Id));

            cmd.ExecuteNonQuery();
            cmd.Dispose();

        }

        /// <summary>
        /// Makes a copy of the list of expenses and returns it.
        /// </summary>
        /// <returns>A copy of the list of expenses.</returns>
        /// <example>
        /// <b>Display all the Expenses.</b>
        /// <code>
        /// <![CDATA[
        /// // Get expenses from a file
        /// try{
        ///     Expenses expenses = new Expenses(conn);
        ///
        ///     List<Expense> listOfExpenses = expenses.List();
        /// 
        ///     // Write all the expenses to the console
        ///     foreach(Expense exp in listOfExpenses){
        ///         Console.WriteLine(Expense.Description);
        ///     }
        /// }
        /// catch (Exception e){
        ///     Console.WriteLine(e.Message);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public List<Expense> List()
        {
            string stm = "SELECT Id,[Date],Description,Amount,CategoryId FROM expenses;";
            SQLiteCommand cmd = new SQLiteCommand(stm, databaseConnection);
            SQLiteDataReader reader = cmd.ExecuteReader();

            List<Expense> newList = new List<Expense>();
            while (reader.Read())
                newList.Add(new Expense(reader.GetInt32(0), reader.GetDateTime(1), reader.GetInt32(4), reader.GetDouble(3), reader.GetString(2)));

          
            return newList;
        }





        /// <summary>
        ///  Finds an expense then updates it 
        /// </summary>
        /// <param name="id"> the id of the expense to be updated</param>
        /// <param name="date">the new date  given to the expense</param>
        /// <param name="category"> the new catgery id given to the expense</param>
        /// <param name="amount">the new  amount given to the updated expense</param>
        /// <param name="description">the new description given to the expense</param>
        /// <returns> the number of rows updated</returns>
        /// <b> writes 1 to the console if the update was successful</b>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// 
        /// try{
        ///     Expenses expenses = new Expenses(conn);
        ///     
        ///       // seeing if the row was updated
        ///    int rowUpdated = expenses.UpdateExpense(3,DateTime.Now,4,23.97,"expensive stuff")
        ///       Console.WriteLine(rowUpdated);
        ///     }
        /// }
        ///  catch (Exception e){
        ///     Console.WriteLine(e.Message);
        /// }
        ///     /// ]]>
        /// 
        /// </code>
        /// </example>
      


        public int UpdateExpense(int id, DateTime date, int category, Double amount, String description)
        {
            string stm = "UPDATE expenses " +
                 "SET  [Date] = @date," +
                 " Description = @desc ," +
                 " Amount = @amount ," +
                 " CategoryId = @catid " +
                 " WHERE Id = @id";

            SQLiteCommand cmd = new SQLiteCommand(stm, databaseConnection);
            cmd.Parameters.Add(new SQLiteParameter("@id", id));
            cmd.Parameters.Add(new SQLiteParameter("@desc", description));
            cmd.Parameters.Add(new SQLiteParameter("@amount", amount));
            cmd.Parameters.Add(new SQLiteParameter("@date", date));
            cmd.Parameters.Add(new SQLiteParameter("@catid", category));
            int count = cmd.ExecuteNonQuery();

            cmd.Dispose();

            if (count > 0)
                return count;
            throw new Exception("No row was updated");
        }



    }
}

