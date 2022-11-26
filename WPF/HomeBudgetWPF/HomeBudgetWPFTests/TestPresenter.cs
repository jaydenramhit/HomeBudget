using System;
using Xunit;
using HomeBudgetWPF;
using System.Collections.Generic;
using Budget;


namespace HomeBudgetWPFTests
{
    public class TestPresenter
    {

        [Fact]
        public void AddCategory_ValidInput_UpdatesCategoriesWithNoError()
        {
            //arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);

            //act
            presenter.AddCategory("car payment", Category.CategoryType.Credit);

            //assert
            Assert.True(mainView.clearCategoryFormWasCalled);
            Assert.False(mainView.showCategoriesAddErrorWasCalled);
            Assert.Equal(2, mainView.numberOfTimesPopulateCategoriesWasCalled);
            Assert.True(mainView.successfullyAddedCategoryWasCalled);

        }

        [Fact]
        public void AddCategory_EmptyDescription_OnlyShowCategoriesAddErrorWasCalled()
        {
            //arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);

            //act
            presenter.AddCategory("", Category.CategoryType.Savings);

            //assert
            Assert.False(mainView.clearCategoryFormWasCalled);
            Assert.True(mainView.showCategoriesAddErrorWasCalled);
            Assert.Equal(new int[] { 0 }, mainView.arrayPassedToShowCategoriesAddError);
            Assert.Equal(1, mainView.numberOfTimesPopulateCategoriesWasCalled);

        }

        [Fact]
        public void AddExpense_ValidInput_ClearsExpenseFormWithoutError()
        {
            // Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);

            // Act
            presenter.AddExpense(10, 1, DateTime.Today, "Description");

            // Assert
            Assert.True(mainView.clearExpenseFormWasCalled);
            Assert.False(mainView.showErrorWasCalled);
            Assert.False(mainView.showExpensesAddErrorWasCalled);
            Assert.True(mainView.successfullyAddedExpenseWasCalled);
        }

        [Fact]
        public void AddExpense_EmptyDescription_OnlyCallsShowExpensesAddError()
        {
            // Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);

            // Act
            presenter.AddExpense(10, 1, DateTime.Today, "");

            // Assert
            Assert.True(mainView.showExpensesAddErrorWasCalled);
            Assert.Single(mainView.arrayPassedToShowExpensesAddError);
            Assert.Equal(3, mainView.arrayPassedToShowExpensesAddError[0]);
            Assert.False(mainView.clearExpenseFormWasCalled);
            Assert.False(mainView.showErrorWasCalled);
        }

        [Fact]
        public void PopulateCategories_IsCalledOnInitialization()
        {
            // Arrange/Act
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);

            // Assert
            Assert.Equal(1, mainView.numberOfTimesPopulateCategoriesWasCalled);
        }

        [Fact]
        public void AddExpense_ValidData_DisplaysBudgetItems()
        {
            // Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);

            // Act
            presenter.AddExpense(10, 1, DateTime.Now, "desc");

            // Assert
            Assert.True(mainView.displayGroupedOrFilteredBudgetItemsCalled);
        }

        [Fact]
        public void AddExpense_InalidData_DoesntDisplaysBudgetItems()
        {
            // Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);

            // Act
            presenter.AddExpense(10, 1, DateTime.Now, "");

            // Assert
            Assert.False(mainView.displayGroupedOrFilteredBudgetItemsCalled);
        }

        [Fact]
        public void DisplayAllBudgetItems_NoFilter_ShowsAllBudgetItems()
        {
            // Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);
            presenter.AddExpense(10, 1, DateTime.Now.AddDays(-1), "Description");
            presenter.AddExpense(10, 2, DateTime.Today, "Description");
            presenter.AddExpense(10, 1, DateTime.Today.AddDays(1), "Description");

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            // Act
            presenter.DisplayAllBudgetItems(null, null, false, 0);

            // Assert
            Assert.Equal(1, dataView.numberOfTimesShowBudgetItemsWasCalled);
            Assert.Equal(3, dataView.budgetItemsShowBudgetItemsWasCalledWith.Count);

        }

        [Fact]
        public void DisplayAllBudgetItems_FilterStartDate_CorrectBudgetItemsShown()
        {
            // Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);
            presenter.AddExpense(10, 1, DateTime.Now.AddDays(-1), "Description 1");
            presenter.AddExpense(10, 2, DateTime.Today, "Description 2");
            presenter.AddExpense(10, 1, DateTime.Today.AddDays(1), "Description 3");

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            // Act
            presenter.DisplayAllBudgetItems(DateTime.Today, null, false, 0);

            // Assert
            Assert.Equal(1, dataView.numberOfTimesShowBudgetItemsWasCalled);
            Assert.Equal(2, dataView.budgetItemsShowBudgetItemsWasCalledWith.Count);
            Assert.Equal("Description 2", dataView.budgetItemsShowBudgetItemsWasCalledWith[0].ShortDescription);
            Assert.Equal("Description 3", dataView.budgetItemsShowBudgetItemsWasCalledWith[1].ShortDescription);

        }

        [Fact]
        public void DisplayAllBudgetItems_FilterEndDate_CorrectBudgetItemsShown()
        {
            // Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);
            presenter.AddExpense(10, 1, DateTime.Now.AddDays(-1), "Description 1");
            presenter.AddExpense(10, 2, DateTime.Today, "Description 2");
            presenter.AddExpense(10, 1, DateTime.Today.AddDays(1), "Description 3");

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            // Act
            presenter.DisplayAllBudgetItems(null, DateTime.Now, false, 0);

            // Assert
            Assert.Equal(1, dataView.numberOfTimesShowBudgetItemsWasCalled);
            Assert.Equal(2, dataView.budgetItemsShowBudgetItemsWasCalledWith.Count);
            Assert.Equal("Description 1", dataView.budgetItemsShowBudgetItemsWasCalledWith[0].ShortDescription);
            Assert.Equal("Description 2", dataView.budgetItemsShowBudgetItemsWasCalledWith[1].ShortDescription);

        }

        [Fact]
        public void DisplayAllBudgetItems_FilterStartAndEndDate_CorrectBudgetItemsShown()
        {
            // Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);
            presenter.AddExpense(10, 1, DateTime.Now.AddDays(-1), "Description 1");
            presenter.AddExpense(10, 2, DateTime.Today, "Description 2");
            presenter.AddExpense(10, 1, DateTime.Today.AddDays(1), "Description 3");

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            // Act
            presenter.DisplayAllBudgetItems(DateTime.Now, DateTime.Now, false, 0);

            // Assert
            Assert.Equal(1, dataView.numberOfTimesShowBudgetItemsWasCalled);
            Assert.Single(dataView.budgetItemsShowBudgetItemsWasCalledWith);
            Assert.Equal("Description 2", dataView.budgetItemsShowBudgetItemsWasCalledWith[0].ShortDescription);

        }

        [Fact]
        public void DisplayAllBudgetItems_FilterCategory_CorrectBudgetItemsShown()
        {
            // Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);
            presenter.AddExpense(10, 1, DateTime.Now.AddDays(-1), "Description 1");
            presenter.AddExpense(10, 2, DateTime.Today, "Description 2");
            presenter.AddExpense(10, 1, DateTime.Today.AddDays(1), "Description 3");

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            // Act
            presenter.DisplayAllBudgetItems(null, null, true, 1);

            // Assert
            Assert.Equal(1, dataView.numberOfTimesShowBudgetItemsWasCalled);
            Assert.Equal(2, dataView.budgetItemsShowBudgetItemsWasCalledWith.Count);
            Assert.Equal("Description 1", dataView.budgetItemsShowBudgetItemsWasCalledWith[0].ShortDescription);
            Assert.Equal("Description 3", dataView.budgetItemsShowBudgetItemsWasCalledWith[1].ShortDescription);

        }

        [Fact]
        public void DisplayAllBudgetItems_FilterAll_CorrectBudgetItemsShown()
        {
            // Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);
            presenter.AddExpense(10, 1, DateTime.Now.AddDays(-1), "Description 1");
            presenter.AddExpense(10, 2, DateTime.Today, "Description 2");
            presenter.AddExpense(10, 1, DateTime.Today.AddDays(1), "Description 3");

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            // Act
            presenter.DisplayAllBudgetItems(DateTime.Now, DateTime.Now.AddDays(5), true, 1);

            // Assert
            Assert.Equal(1, dataView.numberOfTimesShowBudgetItemsWasCalled);
            Assert.Single(dataView.budgetItemsShowBudgetItemsWasCalledWith);
            Assert.Equal("Description 3", dataView.budgetItemsShowBudgetItemsWasCalledWith[0].ShortDescription);

        }

        [Fact]
        public void DisplayAllBudgetItems_FilterAllNoMatches_NoBudgetItemsShown()
        {
            // Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);
            presenter.AddExpense(10, 1, DateTime.Now.AddDays(-1), "Description 1");
            presenter.AddExpense(10, 2, DateTime.Today, "Description 2");
            presenter.AddExpense(10, 1, DateTime.Today.AddDays(1), "Description 3");

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            // Act
            presenter.DisplayAllBudgetItems(DateTime.Now, DateTime.Now.AddDays(5), true, 8);

            // Assert
            Assert.Equal(1, dataView.numberOfTimesShowBudgetItemsWasCalled);
            Assert.Empty(dataView.budgetItemsShowBudgetItemsWasCalledWith);

        }
        [Fact]
        public void DisplayBudgetItemsByCategory_NoFilter_ShowsBudgetItemsByCategory()
        {
            // Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);
            presenter.AddExpense(10, 1, DateTime.Now.AddDays(-1), "Description");
            presenter.AddExpense(10, 2, DateTime.Today, "Description");
            presenter.AddExpense(10, 1, DateTime.Today.AddDays(1), "Description");

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            // Act
            presenter.DisplayBudgetItemsByCategory(null, null, false, 0);

            // Assert
            Assert.True(dataView.ShowBudgetItemsByCategoryWasCalled);
            Assert.Equal(2, dataView.budgetItemsByCategoryShowBudgetItemsByCategoryWasCalledWith.Count);
            Assert.Equal(20, dataView.budgetItemsByCategoryShowBudgetItemsByCategoryWasCalledWith[0].Total);
            Assert.Equal(10, dataView.budgetItemsByCategoryShowBudgetItemsByCategoryWasCalledWith[1].Total);
        }

        [Fact]
        public void DisplayBudgetItemsByCategory_FilterStartDate_CorrectBudgetItemsByCategoryShown()
        {
            // Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);
            presenter.AddExpense(10, 1, DateTime.Today.AddDays(-1), "Description 1");
            presenter.AddExpense(10, 1, DateTime.Today, "Description 2");
            presenter.AddExpense(10, 1, DateTime.Today, "Description 3");
            presenter.AddExpense(10, 2, DateTime.Today.AddDays(1), "Description 4");

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            // Act
            presenter.DisplayBudgetItemsByCategory(DateTime.Today, null, false, 0);

            // Assert
            Assert.True(dataView.ShowBudgetItemsByCategoryWasCalled);
            Assert.Equal(2, dataView.budgetItemsByCategoryShowBudgetItemsByCategoryWasCalledWith.Count);
            Assert.Equal(20, dataView.budgetItemsByCategoryShowBudgetItemsByCategoryWasCalledWith[0].Total);
            Assert.Equal(10, dataView.budgetItemsByCategoryShowBudgetItemsByCategoryWasCalledWith[1].Total);
        }

        [Fact]
        public void DisplayBudgetItemsByCategory_FilterEndDate_CorrectBudgetItemsByCategoryShown()
        {
            // Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);
            presenter.AddExpense(10, 1, DateTime.Today.AddDays(-1), "Description 1");
            presenter.AddExpense(10, 1, DateTime.Today.AddDays(-1), "Description 1");
            presenter.AddExpense(10, 1, DateTime.Today.AddDays(1), "Description 1");
            presenter.AddExpense(10, 2, DateTime.Today.AddDays(1), "Description 1");

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            // Act
            presenter.DisplayBudgetItemsByCategory(null, DateTime.Today, false, 0);

            // Assert
            Assert.True(dataView.ShowBudgetItemsByCategoryWasCalled);
            Assert.Single(dataView.budgetItemsByCategoryShowBudgetItemsByCategoryWasCalledWith);
            Assert.Equal(20, dataView.budgetItemsByCategoryShowBudgetItemsByCategoryWasCalledWith[0].Total);
        }
        [Fact]
        public void DisplayBudgetItemsByCategory_FilterStartAndEndDate_CorrectBudgetItemsByCategoryShown()
        {
            // Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);
            presenter.AddExpense(10, 1, DateTime.Today.AddDays(-1), "Description 1");
            presenter.AddExpense(10, 1, DateTime.Today, "Description 2");
            presenter.AddExpense(10, 1, DateTime.Today, "Description 3");
            presenter.AddExpense(10, 2, DateTime.Today.AddDays(1), "Description 4");

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            // Act
            presenter.DisplayBudgetItemsByCategory(DateTime.Today, DateTime.Today, false, 0);

            // Assert
            Assert.True(dataView.ShowBudgetItemsByCategoryWasCalled);
            Assert.Single(dataView.budgetItemsByCategoryShowBudgetItemsByCategoryWasCalledWith);
            Assert.Equal(20, dataView.budgetItemsByCategoryShowBudgetItemsByCategoryWasCalledWith[0].Total);
        }
        [Fact]
        public void DisplayBudgetItemsByCategory_FilterCategory_CorrectBudgetItemsByCategoryShown()
        {
            // Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);
            presenter.AddExpense(10, 1, DateTime.Today.AddDays(-1), "Description 1");
            presenter.AddExpense(10, 1, DateTime.Today, "Description 2");
            presenter.AddExpense(10, 1, DateTime.Today, "Description 3");
            presenter.AddExpense(10, 2, DateTime.Today.AddDays(1), "Description 4");

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            // Act
            presenter.DisplayBudgetItemsByCategory(null, null, true, 1);

            // Assert
            Assert.True(dataView.ShowBudgetItemsByCategoryWasCalled);
            Assert.Single(dataView.budgetItemsByCategoryShowBudgetItemsByCategoryWasCalledWith);
            Assert.Equal(30, dataView.budgetItemsByCategoryShowBudgetItemsByCategoryWasCalledWith[0].Total);
        }
        [Fact]
        public void DisplayBudgetItemsByCategory_FilterAll_CorrectBudgetItemsByCategoryShown()
        {
            // Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);
            presenter.AddExpense(10, 1, DateTime.Today.AddDays(-1), "Description 1");
            presenter.AddExpense(10, 1, DateTime.Today, "Description 2");
            presenter.AddExpense(10, 1, DateTime.Today.AddDays(2), "Description 3");
            presenter.AddExpense(10, 2, DateTime.Today.AddDays(2), "Description 4");
            presenter.AddExpense(10, 1, DateTime.Today.AddDays(4), "Description 5");

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            // Act
            presenter.DisplayBudgetItemsByCategory(DateTime.Today, DateTime.Today.AddDays(3), true, 1);

            // Assert
            Assert.True(dataView.ShowBudgetItemsByCategoryWasCalled);
            Assert.Single(dataView.budgetItemsByCategoryShowBudgetItemsByCategoryWasCalledWith);
            Assert.Equal(20, dataView.budgetItemsByCategoryShowBudgetItemsByCategoryWasCalledWith[0].Total);
        }
        [Fact]
        public void DisplayBudgetItemsByCategory_FilterAllNoMatches_NoBudgetItemsByCategoryShown()
        {
            // Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);
            presenter.AddExpense(10, 1, DateTime.Today.AddDays(-1), "Description 1");
            presenter.AddExpense(10, 1, DateTime.Today, "Description 2");
            presenter.AddExpense(10, 1, DateTime.Today.AddDays(2), "Description 3");
            presenter.AddExpense(10, 2, DateTime.Today.AddDays(2), "Description 4");
            presenter.AddExpense(10, 1, DateTime.Today.AddDays(4), "Description 5");

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            // Act
            presenter.DisplayBudgetItemsByCategory(DateTime.Today, DateTime.Today.AddDays(3), true, 3);

            // Assert
            Assert.True(dataView.ShowBudgetItemsByCategoryWasCalled);
            Assert.Empty(dataView.budgetItemsByCategoryShowBudgetItemsByCategoryWasCalledWith);
        }

        [Fact]
        public void DeleteExpense_ValidId_CallsDisplayGroupedOrFilteredBudgetItems()
        {
            // Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);
            presenter.AddExpense(10, 1, DateTime.Now.AddDays(-1), "Description 1");
            presenter.AddExpense(10, 2, DateTime.Today, "Description 2");

            // Act
            presenter.DeleteExpense(2);

            // Assert
            Assert.True(mainView.displayGroupedOrFilteredBudgetItemsCalled);

        }

        [Fact]
        public void DeleteExpense_InvalidId_CallsDisplayGroupedOrFilteredBudgetItems()
        {
            // Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);
            presenter.AddExpense(10, 1, DateTime.Now.AddDays(-1), "Description 1");
            presenter.AddExpense(10, 2, DateTime.Today, "Description 2");

            // Act
            presenter.DeleteExpense(3);

            // Assert
            Assert.True(mainView.displayGroupedOrFilteredBudgetItemsCalled);

        }

        [Fact]

        public void DisplayBudgetItemsGroupedByMonth_NoFilter_NoDateSpecified_AllMonthsShown()
        {
            //Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);

            presenter.AddExpense(23, 2, DateTime.Now.AddDays(-56), "Description 1");
            presenter.AddExpense(13, 4, DateTime.Now, "Description 2");

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            //Act
            presenter.DisplayBudgetItemsGroupByMonth(null, null, false, 0);

            //Assert
            Assert.Equal(23, dataView.budgetItemsShowBudgetItemsByMonthWasCalledWith[0].Total);
            Assert.Equal(2, dataView.budgetItemsShowBudgetItemsByMonthWasCalledWith.Count);

        }



        [Fact]

        public void DisplayBudgetItemsGroupedByMonth_NoFilter_FilteredDate_OnlyOneItemIsShown()
        {
            //Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);

            presenter.AddExpense(23, 2, DateTime.Now.AddDays(-56), "Description 1");
            presenter.AddExpense(13, 4, DateTime.Now, "Description 2");

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            //Act
            presenter.DisplayBudgetItemsGroupByMonth(DateTime.Now.AddDays(-78), DateTime.Now.AddDays(-21), false, 0);

            //Assert
            Assert.Equal(23, dataView.budgetItemsShowBudgetItemsByMonthWasCalledWith[0].Total);
            Assert.Single(dataView.budgetItemsShowBudgetItemsByMonthWasCalledWith);


        }


        [Fact]

        public void DisplayBudgetItemsGroupedByMonth_FilterById4_NoDateFilter_ItemTotalIs50()
        {
            //Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);

            presenter.AddExpense(23, 2, DateTime.Now.AddDays(-56), "Description 1");
            presenter.AddExpense(13, 4, DateTime.Now, "Description 2");
            presenter.AddExpense(37, 4, DateTime.Now, "Description 3");

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            //Act
            presenter.DisplayBudgetItemsGroupByMonth(null,null, true, 4);

            //Assert
            Assert.Equal(50, dataView.budgetItemsShowBudgetItemsByMonthWasCalledWith[0].Total);
            Assert.Single(dataView.budgetItemsShowBudgetItemsByMonthWasCalledWith);
        }


        [Fact]

        public void DisplayBudgetItemsGroupedByMonth_FilterById4_FilteredDate__ItemTotalIs37()
        {
            //Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);

            presenter.AddExpense(23, 2, DateTime.Now.AddDays(-56), "Description 1");
            presenter.AddExpense(13, 4, DateTime.Now, "Description 2");
            presenter.AddExpense(37, 4, DateTime.Now.AddDays(-2), "Description 3");

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            //Act
            presenter.DisplayBudgetItemsGroupByMonth(null, DateTime.Now.AddDays(-2), true, 4);

            //Assert
            Assert.Equal(37, dataView.budgetItemsShowBudgetItemsByMonthWasCalledWith[0].Total);
            Assert.Single(dataView.budgetItemsShowBudgetItemsByMonthWasCalledWith);
        }

        // Edit View

        [Fact]
        public void EditExpense_ValidData_CallsIndicateSuccessfulEditAndEditsExpense()
        {
            // Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            TestEditView editView = new TestEditView();
            Presenter presenter = new Presenter(mainView, "test.db", true);
            presenter.AddExpense(10, 1, DateTime.Now.AddDays(-1), "Description 1");
            presenter.AddExpense(10, 2, DateTime.Today, "Description 2");
            presenter.EditView = editView;

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;


            // Act
            presenter.EditExpense(1, DateTime.Now.AddDays(100), 4, -1000, "New Description");
            presenter.DisplayAllBudgetItems(null, null, false, 0);

            // Assert
            Assert.True(editView.indicateSuccessfulEditCalled);
            Assert.Equal(-1000, dataView.budgetItemsShowBudgetItemsWasCalledWith[1].Amount);
            Assert.Equal(4, dataView.budgetItemsShowBudgetItemsWasCalledWith[1].CategoryID);
            Assert.Equal(DateTime.Now.AddDays(100).Date, dataView.budgetItemsShowBudgetItemsWasCalledWith[1].Date.Date);
            Assert.Equal("New Description", dataView.budgetItemsShowBudgetItemsWasCalledWith[1].ShortDescription);

        }

        [Fact]
        public void EditExpense_InvalidDescription_CallsDisplayEditFailWasCalled()
        {
            // Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            TestEditView editView = new TestEditView();
            Presenter presenter = new Presenter(mainView, "test.db", true);
            presenter.AddExpense(10, 1, DateTime.Now.AddDays(-1), "Description 1");
            presenter.AddExpense(10, 2, DateTime.Today, "Description 2");
            presenter.EditView = editView;

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            // Act
            presenter.EditExpense(1, DateTime.Now.AddDays(100), 4, -1000, "");
            presenter.DisplayAllBudgetItems(null, null, false, 0);

            // Assert
            Assert.True(editView.displayEditFailWasCalled);
            Assert.Equal(10, dataView.budgetItemsShowBudgetItemsWasCalledWith[0].Amount);
            Assert.Equal(1, dataView.budgetItemsShowBudgetItemsWasCalledWith[0].CategoryID);
            Assert.Equal(DateTime.Now.AddDays(-1).Date, dataView.budgetItemsShowBudgetItemsWasCalledWith[0].Date.Date);
            Assert.Equal("Description 1", dataView.budgetItemsShowBudgetItemsWasCalledWith[0].ShortDescription);

        }



        [Fact]
        public void  DisplayBudgetItemsByCategoryByMonth_NoFilter_NoFilteredDate_AllItems_AreShown()
        {
            //arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);

            presenter.AddExpense(23, 2, DateTime.Now.AddDays(-56), "Description 1");
            presenter.AddExpense(13, 4, DateTime.Now, "Description 2");
            presenter.AddExpense(37, 4, DateTime.Now, "Description 3");

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            //Act
            presenter.DisplayBudgetItemsGroupedByCategoryBymonth(null, null, false, 0);

            //Assert
            Assert.Equal(3, dataView.budgetItemsShowBudgetItemsByCategoryByMonthWasCalledWith.Count);
            Assert.True(dataView.budgetItemsShowBudgetItemsByCategoryByMonthWasCalledWith[0].ContainsKey("Rent"));
            Assert.True(dataView.budgetItemsShowBudgetItemsByCategoryByMonthWasCalledWith[1].ContainsKey("Entertainment"));

        }

        [Fact]
        public void DisplayBudgetItemsByCategoryByMonth_NoExpenseAdded_DictionaryEmpty()
        {
            //Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            //Act
            presenter.DisplayBudgetItemsGroupedByCategoryBymonth(null, null, false, 0);

            //Assert
            Assert.Single(dataView.budgetItemsShowBudgetItemsByCategoryByMonthWasCalledWith);

       

        }

        [Fact]
        public void DisplayBudgetItemsByCategoryByMonth_FilterDateOnly_CorrectItemsAreShown()
        {
            //Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);

            presenter.AddExpense(23, 2, DateTime.Now.AddDays(-56), "Description 1");
            presenter.AddExpense(13, 4, DateTime.Now, "Description 2");
            presenter.AddExpense(37, 4, DateTime.Now.AddDays(-2), "Description 3");
            presenter.AddExpense(41, 7, DateTime.Now.AddDays(-93), "Description 3");

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            //Act
            presenter.DisplayBudgetItemsGroupedByCategoryBymonth(null, DateTime.Now.AddDays(-42), false, 0);

            //Assert
            Assert.Equal(3, dataView.budgetItemsShowBudgetItemsByCategoryByMonthWasCalledWith.Count);
            Assert.True(dataView.budgetItemsShowBudgetItemsByCategoryByMonthWasCalledWith[0].ContainsKey("Medical Expenses"));
            Assert.True(dataView.budgetItemsShowBudgetItemsByCategoryByMonthWasCalledWith[1].ContainsKey("Rent"));

        }
            
        [Fact]

        public void DisplayBudgetItemsByCategoryByMonth_FilterById4_CorrectItemsAreShown()
        {

            //Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);

            presenter.AddExpense(23, 2, DateTime.Now.AddDays(-56), "Description 1");
            presenter.AddExpense(13, 4, DateTime.Now, "Description 2");
            presenter.AddExpense(37, 4, DateTime.Now, "Description 3");

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            //Act
            presenter.DisplayBudgetItemsGroupedByCategoryBymonth(null, null,true, 4);

            //Assert
            Assert.Equal(2, dataView.budgetItemsShowBudgetItemsByCategoryByMonthWasCalledWith.Count);
            Assert.True(dataView.budgetItemsShowBudgetItemsByCategoryByMonthWasCalledWith[0].ContainsKey("Entertainment"));

        }


        [Fact]
        public void DisplayBudgetItemsByCategoryByMonthFilterById3AndByDate_CorrectItemsAreShown()
        {
            //Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);

            presenter.AddExpense(23, 2, DateTime.Now.AddDays(-96), "Description 1");
            presenter.AddExpense(13, 4, DateTime.Now, "Description 2");
            presenter.AddExpense(37, 4, DateTime.Now.AddDays(-2), "Description 3");
            presenter.AddExpense(91, 3, DateTime.Now.AddDays(-34), "Description 4");
            presenter.AddExpense(11, 3, DateTime.Now.AddDays(-79), "Description 5");
            presenter.AddExpense(19, 3, DateTime.Now, "Description 6");

            TestDataVisualizationView dataView = new TestDataVisualizationView();
            presenter.DataVisualizationView = dataView;

            //Act
            presenter.DisplayBudgetItemsGroupedByCategoryBymonth(DateTime.Now.AddDays(-100), DateTime.Now.AddDays(-17), true, 3);


            //Assert
            Assert.Equal(3, dataView.budgetItemsShowBudgetItemsByCategoryByMonthWasCalledWith.Count);
            Assert.True(dataView.budgetItemsShowBudgetItemsByCategoryByMonthWasCalledWith[0].ContainsKey("Food"));
            Assert.True(dataView.budgetItemsShowBudgetItemsByCategoryByMonthWasCalledWith[1].ContainsKey("Food"));
       
        }

        [Fact]

        public void DeleteTwoExpenses_BothGetSelectedIndexAndChangeSelectedItemsAreCalled()
        {
            //Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);

            presenter.AddExpense(23, 2, DateTime.Now.AddDays(-96), "Description 1");
            presenter.AddExpense(13, 4, DateTime.Now, "Description 2");
            presenter.AddExpense(37, 4, DateTime.Now.AddDays(-2), "Description 3");

            //Act
            presenter.DeleteExpense(2);
            presenter.DeleteExpense(1);

            //Assert
            Assert.True(mainView.displayGroupedOrFilteredBudgetItemsCalled);

        }

        [Fact]
        public void NoExpensesDeleted_NoneOfTheSelectedIndexAndChangeSelectedItemArecalled()
        {

            //Arrange
            MainViewTestClass mainView = new MainViewTestClass();
            Presenter presenter = new Presenter(mainView, "test.db", true);

            presenter.AddExpense(23, 2, DateTime.Now.AddDays(-96), "Description 1");
            presenter.AddExpense(13, 4, DateTime.Now, "Description 2");
            presenter.AddExpense(37, 4, DateTime.Now.AddDays(-2), "Description 3");

            //Act

            //Assert
            Assert.True(mainView.displayGroupedOrFilteredBudgetItemsCalled);
        }



        private class MainViewTestClass : MainViewInterface
        {
            public bool clearExpenseFormWasCalled;
            public bool showErrorWasCalled;
            public bool showExpensesAddErrorWasCalled;
            public bool showCategoriesAddErrorWasCalled;
            public int numberOfTimesPopulateCategoriesWasCalled;
            public int[] arrayPassedToShowExpensesAddError;
            public int[] arrayPassedToShowCategoriesAddError;
            public bool clearCategoryFormWasCalled;
            public bool successfullyAddedCategoryWasCalled;
            public bool successfullyAddedExpenseWasCalled;
            public bool displayGroupedOrFilteredBudgetItemsCalled;

            public MainViewTestClass()
            {
                clearExpenseFormWasCalled = false;
                showErrorWasCalled = false;
                showExpensesAddErrorWasCalled = false;
                showCategoriesAddErrorWasCalled = false;
                arrayPassedToShowExpensesAddError = null;
                clearCategoryFormWasCalled = false;
                successfullyAddedCategoryWasCalled = false;
                successfullyAddedExpenseWasCalled = false;
                displayGroupedOrFilteredBudgetItemsCalled = false;
            }

            public void ClearExpenseForm()
            {
                clearExpenseFormWasCalled = true;
            }

            public void ShowError(string error)
            {
                showErrorWasCalled = true;
            }

            public void ShowExpensesAddError(string error, int[] invalidArguments)
            {
                showExpensesAddErrorWasCalled = true;
                arrayPassedToShowExpensesAddError = invalidArguments;
            }

            public void ClearCategoryForm()
            {
                clearCategoryFormWasCalled = true;
            }

            public void PopulateCategories(List<Category> category)
            {
                numberOfTimesPopulateCategoriesWasCalled++;
            }

            public void SuccessfullyAddedCategory(string message)
            {
                successfullyAddedCategoryWasCalled = true;

            }

            public void SuccessfullyAddedExpense(string message)
            {
                successfullyAddedExpenseWasCalled = true;

            }

            public void ShowCategoriesAddError(string error, int[] invalidArgumentPosition)
            {
                showCategoriesAddErrorWasCalled = true;
                arrayPassedToShowCategoriesAddError = invalidArgumentPosition;
            }

            public void DisplayGroupedOrFilteredBudgetItems()
            {
                displayGroupedOrFilteredBudgetItemsCalled = true;
            }


            public void UpdateBudgetItem(BudgetItem budgetItem)
            {
                throw new NotImplementedException();
            }
        }

        private class TestEditView : EditViewInterface
            {

                public bool displayEditFailWasCalled;
                public bool indicateSuccessfulEditCalled;

                public TestEditView()
                {
                    displayEditFailWasCalled = false;
                    indicateSuccessfulEditCalled = false;
                }

                public void DisplayEditFail(string message)
                {
                    displayEditFailWasCalled = true;
                }

                public void IndicateSuccessfulEdit()
                {
                    indicateSuccessfulEditCalled = true;
                }

            }

        private class TestDataVisualizationView: BudgetItemsVisualizationInterface
        {

            public int numberOfTimesShowBudgetItemsWasCalled;
            public List<BudgetItem> budgetItemsShowBudgetItemsWasCalledWith;
            public List<BudgetItemsByMonth> budgetItemsShowBudgetItemsByMonthWasCalledWith;
            public List<Dictionary<string, object>> budgetItemsShowBudgetItemsByCategoryByMonthWasCalledWith;
            public int numberOfTimesShowBudgetItemsByCategoryWasCalled;
            public bool ShowBudgetItemsByCategoryWasCalled;
            public List<BudgetItemsByCategory> budgetItemsByCategoryShowBudgetItemsByCategoryWasCalledWith;
            public int numberOfTimesChangeSelectedItemAfterDeleteWasCalled;
            public int numberOfTimesGetSelectedItemIndexBeforeDeleteWasCalled;
            public bool changeColoursToMatchColourSchemeCalled;

            public TestDataVisualizationView()
            {
                numberOfTimesShowBudgetItemsWasCalled = 0;
                budgetItemsShowBudgetItemsWasCalledWith = null;
                budgetItemsShowBudgetItemsByMonthWasCalledWith = null;
                budgetItemsShowBudgetItemsByCategoryByMonthWasCalledWith = null;
                numberOfTimesShowBudgetItemsByCategoryWasCalled = 0;
                ShowBudgetItemsByCategoryWasCalled = false;
                budgetItemsByCategoryShowBudgetItemsByCategoryWasCalledWith = null;
                numberOfTimesChangeSelectedItemAfterDeleteWasCalled = 0;
                numberOfTimesGetSelectedItemIndexBeforeDeleteWasCalled = 0;
                changeColoursToMatchColourSchemeCalled = false;
            }

            public void ShowBudgetItems(List<BudgetItem> budgetItems)
            {
                numberOfTimesShowBudgetItemsWasCalled++;
                budgetItemsShowBudgetItemsWasCalledWith = budgetItems;
            }


            public void ShowBudgetItemsGroupedByMonth(List<BudgetItemsByMonth> budgetItemsByMonths)
            {
                budgetItemsShowBudgetItemsByMonthWasCalledWith = budgetItemsByMonths;

            }

            public void ShowBudgetItemsGroupedByCategoryByMonth(List<Dictionary<string, object>> budgetItemsByCategoryByMonth)
            {

                budgetItemsShowBudgetItemsByCategoryByMonthWasCalledWith = budgetItemsByCategoryByMonth;
            }
            public void ShowBudgetItemsByCategory(List<BudgetItemsByCategory> budgetItemsByCategories)
            {
                numberOfTimesShowBudgetItemsByCategoryWasCalled++;
                ShowBudgetItemsByCategoryWasCalled = true;
                budgetItemsByCategoryShowBudgetItemsByCategoryWasCalledWith = budgetItemsByCategories;
            }
            public void ChangeSelectedItemAfterDelete(int rowIndex)
            {
                numberOfTimesChangeSelectedItemAfterDeleteWasCalled = rowIndex;
            }

            public int GetSelectedItemIndexBeforeDelete()
            {
                return ++numberOfTimesGetSelectedItemIndexBeforeDeleteWasCalled;
            }

            public void ChangeColorsToMatchColourScheme(string filePath)
            {
                changeColoursToMatchColourSchemeCalled = true;
            }
        }

    }
        
    



}
