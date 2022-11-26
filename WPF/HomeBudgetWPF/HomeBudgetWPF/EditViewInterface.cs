using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgetWPF
{
    public interface EditViewInterface
    {
        /// <summary>
        /// Displays a message for the user when there was a failure to edit an expense.
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        void DisplayEditFail(string message);

        /// <summary>
        /// Indicates that the expense was successfully updated.
        /// </summary>
        void IndicateSuccessfulEdit();
    }
}
