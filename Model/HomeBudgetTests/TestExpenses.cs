using System;
using Xunit;
using System.IO;
using System.Collections.Generic;
using Budget;
using System.Data.SQLite;

namespace BudgetCodeTests
{
    [Collection("Sequential")]
    public class TestExpenses
    {
        int numberOfExpensesInFile = TestConstants.numberOfExpensesInFile;
        String testInputFile = TestConstants.testExpensesInputFile;
        int maxIDInExpenseFile = TestConstants.maxIDInExpenseFile;
        Expense firstExpenseInFile = new Expense(1, new DateTime(2021, 1, 10), 10, 12, "hat (on credit)");


        // ========================================================================

        [Fact]
        public void ExpensesObject_New()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);

            // Act
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn);


            // Assert 
            Assert.IsType<Expenses>(expenses);


     
        }  


        // ========================================================================

        [Fact]
        public void ExpensesMethod_List_ReturnsListOfExpenses()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);


            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn);


            // Act
            List<Expense> list = expenses.List();

            // Assert
            Assert.Equal(numberOfExpensesInFile, list.Count);

        }

        // ========================================================================

        [Fact]
        public void ExpensesMethod_List_ModifyListDoesNotModifyExpensesInstance()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn);


            //expenses.ReadFromFile(dir + "\\" + testInputFile);
            List<Expense> list = expenses.List();


            list[0] = new Expense(200, DateTime.Now, 5, 10, "desc");

            // Assert
            Assert.False(Object.ReferenceEquals(list[0], expenses.List()[0]));

        }

        // ========================================================================

        [Fact]
        public void ExpensesMethod_Add()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn);

            int category = 1;
            double amount = 98.1;

            // Act
            expenses.Add(DateTime.Now, category,amount,"new expense");
            List<Expense> expensesList = expenses.List();
            int sizeOfList = expenses.List().Count;

            // Assert
            Assert.Equal(numberOfExpensesInFile+1, sizeOfList);
            Assert.Equal(maxIDInExpenseFile + 1, expensesList[sizeOfList - 1].Id);
            Assert.Equal(amount, expensesList[sizeOfList - 1].Amount);
            Assert.Equal(DateTime.Now.Day, expensesList[sizeOfList - 1].Date.Day);
            Assert.Equal(DateTime.Now.Month, expensesList[sizeOfList - 1].Date.Month);
            Assert.Equal(DateTime.Now.Year, expensesList[sizeOfList - 1].Date.Year);

        }

        // ========================================================================

        [Fact]
        public void ExpensesMethod_Delete()
        {

            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn);

            int IdToDelete = 3;

            // Act
            expenses.Delete(IdToDelete);
            List<Expense> expensesList = expenses.List();
            int sizeOfList = expensesList.Count;

            // Assert
            Assert.Equal(numberOfExpensesInFile - 1, sizeOfList);
            Assert.False(expensesList.Exists(e => e.Id == IdToDelete), "correct Expense item deleted");

        }      


        // ========================================================================
        [Fact]
        public void ExpenseMethod_Delete_ForeignKeyConstraint_DoesNotThrow()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn);


            int sizeOfList = expenses.List().Count;

            // Act
            // Expenses columns are not used as a foreign key in any of the tables
            // Deleting a row should never throw a foregin key constraint error
            bool deleted = true;

            for (int i = 1; i <= expenses.List().Count; i++)
            {
                try
                {
                    expenses.Delete(i);
                }
                catch (System.Data.SQLite.SQLiteException)
                {
                    deleted = false;
                }
            }


            // Assert
            // There should have been no issues deleting from this table
            Assert.True(deleted);
        }

  
        [Fact]
        public void ExpenseMethod_UpdateExpense()
        {
            //arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.existingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn);

            String newDescr = "Presents";
            int id = 3;
            int catid = 2;
            double amount = 37.73;

            // act
            List<Expense> list = expenses.List();
            expenses.UpdateExpense(id, DateTime.Now, catid, amount, newDescr);
            Expense expense = expenses.List()[id - 1];

            //assert

            Assert.Equal(catid, expense.Category);
            Assert.Equal(amount, expense.Amount);
            Assert.Equal(id, expense.Id);
            Assert.Equal(newDescr, expense.Description);
            Assert.Equal(DateTime.Now.Day, expense.Date.Day);
            Assert.Equal(DateTime.Now.Month, expense.Date.Month);
            Assert.Equal(DateTime.Now.Year, expense.Date.Year);


        }

    }
}

