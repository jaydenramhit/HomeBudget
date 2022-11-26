using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Threading;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("HomeBudgetTests")]

// ===================================================================
// Very important notes:
// ... To keep everything working smoothly, you should always
//     dispose of EVERY SQLiteCommand even if you recycle a 
//     SQLiteCommand variable later on.
//     EXAMPLE:
//            Database.newDatabase(GetSolutionDir() + "\\" + filename);
//            var cmd = new SQLiteCommand(Database.dbConnection);
//            cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES('Whatever')";
//            cmd.ExecuteNonQuery();
//            cmd.Dispose();
//
// ... also dispose of reader objects
//
// ... by default, SQLite does not impose Foreign Key Restraints
//     so to add these constraints, connect to SQLite something like this:
//            string cs = $"Data Source=abc.sqlite; Foreign Keys=1";
//            var con = new SQLiteConnection(cs);
//
// ===================================================================


namespace Budget
{

    internal class Database
    {

        public static SQLiteConnection dbConnection { get { return _connection; } }
        private static SQLiteConnection _connection;

        // ===================================================================
        // create and open a new database
        // ===================================================================
        public static void newDatabase(string filename)
        {

            // If there was a database open before, close it and release the lock
            CloseDatabaseAndReleaseFile();

            SQLiteConnection.CreateFile(filename);
            string cs = $"Data Source={filename}; Foreign Keys=1";


            SQLiteConnection con = new SQLiteConnection(cs);
            _connection = con;
            con.Open();

            string stm = "CREATE TABLE categoryTypes(Id INTEGER PRIMARY KEY, Description TEXT);";
            SQLiteCommand cmd = new SQLiteCommand(stm, con);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            stm = "CREATE TABLE categories(" +
                "Id INTEGER PRIMARY KEY, " +
                "Description TEXT, " +
                "TypeId INTEGER, " +
                "FOREIGN KEY (TypeId) REFERENCES categoryTypes(Id));";
            cmd = new SQLiteCommand(stm, con);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            stm = "CREATE TABLE expenses(" +
                "Id INTEGER PRIMARY KEY, " +
                "Date TEXT, " +
                "Description TEXT, " +
                "Amount REAL, " +
                "CategoryId INTEGER, " +
                "FOREIGN KEY (CategoryId) REFERENCES categories(Id));";
            cmd = new SQLiteCommand(stm, con);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

        }

        // ===================================================================
        // open an existing database
        // ===================================================================
        public static void existingDatabase(string filename)
        {

            CloseDatabaseAndReleaseFile();

            // your code
            string cs = $"Data Source={filename}; Foreign Keys=1";


            SQLiteConnection con = new SQLiteConnection(cs);
            _connection = con;
            con.Open();
        }

        // ===================================================================
        // close existing database, wait for garbage collector to
        // release the lock before continuing
        // ===================================================================
        static public void CloseDatabaseAndReleaseFile()
        {
            if (Database.dbConnection != null)
            {
                // close the database connection
                Database.dbConnection.Close();


                // wait for the garbage collector to remove the
                // lock from the database file
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }

}
