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
    /// Represents a collection of category items.
    /// </summary>
    public class Categories
    {

        private SQLiteConnection databaseConnection;

        // ====================================================================
        // Constructor
        // ====================================================================

        /// <summary>
        /// Creates a categories object with the connection to the database passed.
        /// The if the <paramref name="newDB"/> flag is true, the categories table in the database
        /// is reset.
        /// </summary>
        /// <param name="connection">A connection to a database.</param>
        /// <param name="newDB">Indicates that the connection is to a new database.</param>
        public Categories(SQLiteConnection connection, bool newDB)
        {
            this.databaseConnection = connection;

            if (newDB)
            {
                FillCategoryTypesTableWithEnumValues();
                SetCategoriesToDefaults();
            }
        }


        /// <summary>
        /// Finds a category with the id passed in <paramref name="i"/>.
        /// </summary>
        /// <param name="i">The id of the category to return.</param>
        /// <returns>A Category object with the id passed in <paramref name="i"/>.</returns>
        /// <example>
        /// <b>Create a list of categories with ids between and including 3 and 7.</b>
        /// <code>
        /// <![CDATA[
        /// try{
        ///     HomeBudget homeBudget = new HomeBudget(filename, false);
        ///     homeBudget.categories = defaultCategories;
        ///     List<Category> categoryList = new List<Category>();
        ///     
        ///     // Fill the new list with the desired categories
        ///     for(int i = 3; i < 8; i++){
        ///         categoryList.Add(categories.GetCategoryFromId(i));
        ///     }
        /// }
        /// catch (Exception e){
        ///     Console.WriteLine(e.Message);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        /// <exception cref="Exception">Thrown when the passed id was not found in any of the categories.</exception>
        public Category GetCategoryFromId(int i)
        {
            string stm = "SELECT Id, Description, TypeId FROM categories WHERE Id = @id;";
            SQLiteCommand cmd= new SQLiteCommand(stm, databaseConnection);
            cmd.Parameters.Add(new SQLiteParameter("@id", i));
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                Category c = new Category(reader.GetInt32(0), reader.GetString(1), (Category.CategoryType)reader.GetInt32(2));
                return c;

            }

            throw new Exception("Cannot find category with id " + i.ToString());
            
           
        }

        /// <summary>
        /// Finds a category and updates it in the database
        /// </summary>
        /// <param name="id">The Id of the catgeory to find</param>
        /// <param name="newDescr">the new  description that overwrites the current one </param>
        /// <param name="income"> the  category type thet  overwrites the current one</param>
        /// <example>
        /// <b> finds the category of id 3 and updates it's description and category type and it to a list</b>
        /// <code>
        /// <![CDATA[
        /// try{
        ///     HomeBudget homeBudget = new HomeBudget("databaseFile.db", false)
        ///     Categories categories = homeBudget.list;
        ///     int i=3;
        ///    
        ///     categories.UpdateProperties(i,"a lot of money",income);
        ///     
        /// }
        /// catch (Exception e){
        ///     Console.WriteLine(e.Message);
        /// }
        /// 
        /// ]]>
        /// </code>
        /// </example>
        public int UpdateProperties(int id, string newDescr, Category.CategoryType income)
        {
           
                string stm = "UPDATE categories " +
                                "SET  Description = @desc ," +
                                "TypeId = @TypeId " +
                                "WHERE Id=@id";

                SQLiteCommand cmd = new SQLiteCommand(stm, databaseConnection);
                cmd.Parameters.Add(new SQLiteParameter("@id", id));
                cmd.Parameters.Add(new SQLiteParameter("@desc", newDescr));
                cmd.Parameters.Add(new SQLiteParameter("@typeId", (int)income));
                int count = cmd.ExecuteNonQuery();

                cmd.Dispose();

            if(count>0)
                return count;
            throw new Exception("No row was updated");
          
           

           
           
        }


        /// <summary>
        /// Populates the list of categories with default values.
        /// </summary>
        /// <example>
        /// <b>Reset the categories of an already created HomeBudget</b>
        /// <code>
        /// <![CDATA[
        /// Categories defaultCategories = new Categories(connection, true);
        /// try{
        ///     HomeBudget homeBudget = new HomeBudget(filename, false);
        ///     homeBudget.categories.SetCategoriesToDefaults();
        /// }
        /// catch (Exception e){
        ///     Console.WriteLine(e.Message);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public void SetCategoriesToDefaults()
        {
            // ---------------------------------------------------------------
            // Delete rows from the categories table,
            // ---------------------------------------------------------------
            string stm = "DELETE FROM categories;";
            SQLiteCommand cmd = new SQLiteCommand(stm, databaseConnection);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            // ---------------------------------------------------------------
            // Add Defaults
            // ---------------------------------------------------------------
            Add("Utilities", Category.CategoryType.Expense);
            Add("Rent", Category.CategoryType.Expense);
            Add("Food", Category.CategoryType.Expense);
            Add("Entertainment", Category.CategoryType.Expense);
            Add("Education", Category.CategoryType.Expense);
            Add("Miscellaneous", Category.CategoryType.Expense);
            Add("Medical Expenses", Category.CategoryType.Expense);
            Add("Vacation", Category.CategoryType.Expense);
            Add("Credit Card", Category.CategoryType.Credit);
            Add("Clothes", Category.CategoryType.Expense);
            Add("Gifts", Category.CategoryType.Expense);
            Add("Insurance", Category.CategoryType.Expense);
            Add("Transportation", Category.CategoryType.Expense);
            Add("Eating Out", Category.CategoryType.Expense);
            Add("Savings", Category.CategoryType.Savings);
            Add("Income", Category.CategoryType.Income);

        }

        private void FillCategoryTypesTableWithEnumValues()
        {
            int[] enumIds = (int[])Enum.GetValues(typeof(Category.CategoryType));
            string[] enumNames = (string[])Enum.GetNames(typeof(Category.CategoryType));

            string stm = "INSERT INTO categoryTypes (id, description) values (@id, @name)";
            SQLiteCommand cmd = new SQLiteCommand(stm, databaseConnection);
            
            for (int i = 0; i < enumIds.Length; i++)
            {
                cmd.Parameters.AddWithValue("@id", enumIds[i]);
                cmd.Parameters.AddWithValue("@name", enumNames[i]);
                cmd.ExecuteNonQuery();
            }

            cmd.Dispose();

        }

        /// <summary>
        /// Creates a new category and with the type and description passed. The new category object's id will be one larger than the previous largest id.
        /// </summary>
        /// <param name="desc">The description of the new category.</param>
        /// <param name="type">The type of the new category.</param>
        /// <example>
        /// <b>Add a Category to a Categories object.</b>
        /// <code>
        /// <![CDATA[
        /// try{
        ///     HomeBudget homeBudget = new HomeBudget(filename, false);
        ///     Categories categories = homeBudget.categories;    
        /// 
        ///     // Add categories
        ///     categories.Add("Gas", Category.CategoryType.Expense);
        ///     categories.Add("Part Time Job", Category.CategoryType.Savings);
        ///     categories.Add("Full Time Job", Category.CategoryType.Savings);
        ///     categories.Add("MasterCard", Category.CategoryType.Credit);  
        /// }
        /// catch (Exception e){
        ///     Console.WriteLine(e.Message);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public void Add(String desc, Category.CategoryType type)
        {

            // Get the last highest primary key
            string stm = "SELECT Id from categories order by Id desc limit 1;";
            SQLiteCommand cmd = new SQLiteCommand(stm, databaseConnection);
            SQLiteDataReader reader = cmd.ExecuteReader();

            int newId = 1;
            if (reader.Read())
                newId = reader.GetInt32(0) + 1;
            reader.Close();

            stm = "INSERT INTO categories (Id, Description, TypeId)" +
                  "values (@id, @desc, @typeId);";
            cmd = new SQLiteCommand(stm, databaseConnection);
            cmd.Parameters.Add(new SQLiteParameter("@id", newId));
            cmd.Parameters.Add(new SQLiteParameter("@desc", desc));
            cmd.Parameters.Add(new SQLiteParameter("@typeId", (int)type));

            cmd.ExecuteNonQuery();
            cmd.Dispose();


        }


        /// <summary>
        /// Deletes a category with the passed <paramref name="Id"/> from the list of categories.
        /// </summary>
        /// <param name="Id">The id of the category to delete.</param>
        /// <example>
        /// <b>Remove Categories with ids between 5 and 10.</b>
        /// <code>
        /// <![CDATA[
        /// try{
        ///     HomeBudget homeBudget = new HomeBudget(connection, false);
        ///     
        ///     for (int i = 5; i < 11; i++){
        ///         homeBudget.categories.Delete(i);
        ///     }
        /// }
        /// catch (Exception e){
        ///     Console.WriteLine(e.Message); 
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public void Delete(int Id)
        {
            string stm = "DELETE FROM categories WHERE Id = @Id;";
            
            SQLiteCommand cmd = new SQLiteCommand(stm, databaseConnection);
            cmd.Parameters.Add(new SQLiteParameter("@Id", Id));

            cmd.ExecuteNonQuery();
            cmd.Dispose();

        }

        
        /// <summary>
        /// Creates a list with the categories stored in this object, and returns it.
        /// </summary>
        /// <returns>A copy of the list of categories list stored in this object.</returns>
        /// <example>
        /// <b>Display all the categories.</b>
        /// <code>
        /// <![CDATA[
        /// // Get the list of default categories
        /// Categories categories = new Categories(connection, true);
        /// List<Category> listOfCategories = categories.List();
        /// 
        /// // Write all the categories to the console.
        /// foreach(Category cat in listOfCategories){
        ///     Console.WriteLine(Category.ToString());
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public List<Category> List()
        {
            string stm = "SELECT Id, Description, TypeId FROM categories;";
            SQLiteCommand cmd = new SQLiteCommand(stm, databaseConnection);
            SQLiteDataReader reader = cmd.ExecuteReader();

            List<Category> newList = new List<Category>();
            while (reader.Read())
                newList.Add(new Category(reader.GetInt32(0), reader.GetString(1), (Category.CategoryType)reader.GetInt32(2)));

            cmd.Dispose();
            return newList;
        }

    }
}

