using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget;
using EnterpriseBudget.DeptBudgets.OldFiles;


namespace EnterpriseBudget.DeptBudgets
{
    /// <summary>
    /// Presenter logic for the DeptBudgets.View
    /// </summary>
    public class Presenter
    {
        Model.DepartmentBudgets budget;
        InterfaceView view;
        int deptId;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_view">View associated with this presenter</param>
        /// <param name="deptId">This presenter is for a specific department</param>
        public Presenter(MainViewInterface mainView, InterfaceView _view,int deptId)
        {
            view = _view;
            this.deptId = deptId;
            this.mainView = mainView;
        }
        
        /// <summary>
        /// Get the data from the database, etc
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        public bool LoadData()
        {
            budget = new Model.DepartmentBudgets();

            bool successfullyLoadedData = budget.DownLoadAndOpenDepartmentBudgetFile(deptId);

            if (successfullyLoadedData) {
                model = budget.HomeBudget;
                expenses = model.expenses;
                categories = model.categories;
                mainView.PopulateCategories(categories.List());
            }

            return successfullyLoadedData;
        }

        /// <summary>
        /// The view is closing, and needs to tidy-up by calling
        /// this routine.
        /// </summary>
        public void onClose()
        {
            if (budget != null)
            {
                budget.Close();
            }
        }

        /// <summary>
        /// Saves the local sqlite file to the database.
        /// </summary>
        /// <returns>True if the save was successful, false if it failed.</returns>
        public bool SaveToServer()
        {
            bool saved = budget.SaveToDatabase();
            model = budget.HomeBudget;
            expenses = model.expenses;
            categories = model.categories;
            return saved;
        }

        /// <summary>
        /// Gets the departments limit for the selected category
        /// </summary>
        /// <param name="catId">The id of the selected category</param>
        /// <returns>The limit<returns>
        public double getLimit(int catId)
        {
            SqlDataReader rdr = null;
            double limit = 0;

            try
            {
                SqlCommand getLimit = Model.Connection.cnn.CreateCommand();

                int departmentId = budget.CurrentUserDepartmentId;

                getLimit.CommandText = "SELECT limit FROM budgetCategoryLimits WHERE catId = @catId AND deptId = @departmentId";
                getLimit.Parameters.AddWithValue("@catId", catId);
                getLimit.Parameters.AddWithValue("@departmentId", departmentId);

                rdr = getLimit.ExecuteReader();

                if (rdr.Read())
                {
                    limit = rdr.GetDouble(0);
                }
                
            }
            catch (Exception e)
            {
                string error = "Failed to get the limit for the selected category.";
                mainView.ShowError(error);
            }
            finally
            {
                rdr.Close();
            }
            if (rdr != null)
            {
                try { rdr.Close(); } catch { }
            }

            return limit;
        }

        public double GetCurrentTotal(int catId)
        {
            List<BudgetItemsByCategory> items = model.GetBudgetItemsByCategory(null, null, true, catId);

            if (items.Count == 0)
                return 0;

            return items[0].Total;
        }

        #region OUR PRESENTER

        MainViewInterface mainView;
        EditViewInterface editView;
        BudgetItemsVisualizationInterface dataVisualizationView;
        HomeBudget model;
        Expenses expenses;
        Categories categories;

        /// <summary>
        /// Sets the view used to edit an expense.
        /// </summary>
        public EditViewInterface EditView { set { editView = value; } }
        /// <summary>
        /// Sets the view used to display the budget items to the user.
        /// </summary>
        public BudgetItemsVisualizationInterface DataVisualizationView { set { dataVisualizationView = value; } }


        /// <summary>
        /// Edits an expense in the database to contain the new values passed.
        /// </summary>
        /// <param name="id">The id of the expense to edit.</param>
        /// <param name="date">The new date of the expense.</param>
        /// <param name="categoryId">The id of the new category of the expense.</param>
        /// <param name="amount">The new amount of the expense.</param>
        /// <param name="description">The new description of the expense.</param>
        public void EditExpense(int id, DateTime date, int categoryId, double amount, string description)
        {
            bool validDescription = !String.IsNullOrWhiteSpace(description);

            if (!validDescription)
            {
                editView.DisplayEditFail("-Description can not be empty.");
            }
            else
            {
                try
                {
                    model.expenses.UpdateProperties(id, date, categoryId, amount, description);
                    editView.IndicateSuccessfulEdit();
                }
                catch (Exception)
                {
                    string error = "Error editing the expense. You may have opened the wrong type of file.";
                    editView.DisplayEditFail(error);
                }
            }
        }

        /// <summary>
        /// Validates the arguments and adds an expense to the database made of the values passed.
        /// </summary>
        /// <param name="amount">The amount of the expense to be added.</param>
        /// <param name="category">The ID of the category in the expense to be added.</param>
        /// <param name="date">The date the expense was created.</param>
        /// <param name="description">The description of the expense to be created</param>
        public void AddExpense(double amount, int category, DateTime date, string description)
        {

            bool validDescription = !String.IsNullOrWhiteSpace(description);
            

            // Add validation for description
            if (!validDescription)
            {
                mainView.ShowExpensesAddError("-Description can not be empty.", new int[] { 3 });
            }
         else if(getLimit(category)<GetCurrentTotal(category)+amount)
                mainView.ShowError("The current total spent for this category has exceeded its limit. This expense can not be added");
            else
            {
                try
                {
                    expenses.Add(date, category, amount, description);
                    mainView.SuccessfullyAddedExpense("Successfully added expense");
                    mainView.ClearExpenseForm();
                    mainView.DisplayGroupedOrFilteredBudgetItems();
                }
                catch (Exception e)
                {
                    string error = "There was an error executing the add expense. You may have opened the wrong type of file.";
                    mainView.ShowError(error);
                }
            }

        }

        /// <summary>
        /// Gets all the budget items from the model and calls the necessary functions in the view to display them.
        /// </summary>
        /// <param name="startDate">The minimum start date of the budget items included.</param>
        /// <param name="endDate">The maximum end date of the budget items included.</param>
        /// <param name="filterFlag">Indicated whether the budget items should only include expenses with the passed <paramref name="categoryID"/>.</param>
        /// <param name="categoryID">The category id to filter budget items on if <paramref name="filterFlag"/> is true.</param>
        public void DisplayAllBudgetItems(DateTime? start, DateTime? end, bool filterFlag, int categoryID)
        {
            List<BudgetItem> budgetItems = model.GetBudgetItems(start, end, filterFlag, categoryID);
            dataVisualizationView.ShowBudgetItems(budgetItems);
        }

        /// <summary>
        /// Displays the totals for budget items grouped by month.
        /// </summary>
        /// <param name="startDate">The minimum start date of the budget items included.</param>
        /// <param name="endDate">The maximum end date of the budget items included.</param>
        /// <param name="filterFlag">Indicated whether the budget items should only include expenses with the passed <paramref name="categoryID"/>.</param>
        /// <param name="categoryID">The category id to filter budget items on if <paramref name="filterFlag"/> is true.</param>
        public void DisplayBudgetItemsByCategory(DateTime? start, DateTime? end, bool filterFlag, int categoryID)
        {
            List<BudgetItemsByCategory> budgetItemsByCategory = model.GetBudgetItemsByCategory(start, end, filterFlag, categoryID);
            dataVisualizationView.ShowBudgetItemsByCategory(budgetItemsByCategory);
        }

        /// <summary>
        /// Deletes an expense with the provided <paramref name="id"/>
        /// </summary>
        /// <param name="id">The id of the expense to delete.</param>
        public void DeleteExpense(int id)
        {
            // Delete the expense
            model.expenses.Delete(id);

            // Display the new Budget Items
            mainView.DisplayGroupedOrFilteredBudgetItems();
        }

        /// <summary>
        /// Displays the totals for budget items grouped by month.
        /// </summary>
        /// <param name="startDate">The minimum start date of the budget items included.</param>
        /// <param name="endDate">The maximum end date of the budget items included.</param>
        /// <param name="filterFlag">Indicated whether the budget items should only include expenses with the passed <paramref name="categoryID"/>.</param>
        /// <param name="categoryID">The category id to filter budget items on if <paramref name="filterFlag"/> is true.</param>
        public void DisplayBudgetItemsGroupByMonth(DateTime? startDate, DateTime? endDate, bool filterFlag, int categoryID)
        {
            List<BudgetItemsByMonth> budgetItemsGroupedByMonth = model.GetBudgetItemsByMonth(startDate, endDate, filterFlag, categoryID);

            dataVisualizationView.ShowBudgetItemsGroupedByMonth(budgetItemsGroupedByMonth);
        }


        /// <summary>
        /// Display the totals for budget items grouped by category and by month
        /// </summary>
        /// <param name="startDate">The minimum start date of the budget items included.</param>
        /// <param name="endDate">The maximum end date of the budget items included</param>
        /// <param name="filterFlag">Indicated whether the budget items should only include expenses with the passed <paramref name="categoryID"/>.</param>
        /// <param name="categoryID">The category id to filter budget items on if <paramref name="filterFlag"/> is true.</param>
        public void DisplayBudgetItemsGroupedByCategoryBymonth(DateTime? startDate, DateTime? endDate, bool filterFlag, int categoryID)
        {

            List<Dictionary<string, object>> budgetItemsByCategoryByMonth = model.GetBudgetDictionaryByCategoryAndMonth(startDate, endDate, filterFlag, categoryID);


            dataVisualizationView.ShowBudgetItemsGroupedByCategoryByMonth(budgetItemsByCategoryByMonth);
        }

        /// <summary>
        /// Gets a list containing the name of each category.
        /// </summary>
        /// <returns>A list containing the name of each category.</returns>
        public List<string> GetAllCategoryNames()
        {
            List<Category> categories = model.categories.List();
            List<string> categoryStrings = new List<string>();

            foreach (Category category in categories)
            {
                categoryStrings.Add(category.Description);
            }

            return categoryStrings;
        }

        /// <summary>
        /// Triggers an update in the main view.
        /// </summary>
        /// <param name="budgetItem">The budget item to be updated.</param>
        public void TriggerUpdate(BudgetItem budgetItem)
        {
            mainView.UpdateBudgetItem(budgetItem);
        }

        /// <summary>
        /// Displays the correctly formatted budget items by calling the appropriate method in the 
        /// data visualization view.
        /// </summary>
        /// <param name="groupByMonth">Indicates whether the budget items should be grouped by month.</param>
        /// <param name="groupByCategory">Indicates whether the budget items should be grouped by category.</param>
        /// <param name="startDate">The earliest date of an expense to be displayed.</param>
        /// <param name="endDate">The latest data of an expense to be displayed.</param>
        /// <param name="filterByCategoryFlag">Indicates whether to filter by category.</param>
        /// <param name="categoryId">The id of the category to filter by if the <paramref name="filterByCategoryFlag"/> is true.</param>
        public void DisplayGroupedOrFilteredBudgetItems(bool groupByMonth, bool groupByCategory, DateTime? startDate, DateTime? endDate, bool filterByCategoryFlag, int categoryId)
        {
            if (groupByCategory && groupByMonth)
            {
                DisplayBudgetItemsGroupedByCategoryBymonth(startDate, endDate, filterByCategoryFlag, categoryId);
            }
            else if (groupByCategory)
            {

                DisplayBudgetItemsByCategory(startDate, endDate, filterByCategoryFlag, categoryId);
            }
            else if (groupByMonth)
            {
                DisplayBudgetItemsGroupByMonth(startDate, endDate, filterByCategoryFlag, categoryId);
            }
            else
            {
                DisplayAllBudgetItems(startDate, endDate, filterByCategoryFlag, categoryId);
            }
        }

        #endregion

    }


}
