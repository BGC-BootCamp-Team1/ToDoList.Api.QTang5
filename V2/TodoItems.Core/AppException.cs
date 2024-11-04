using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoItems.Core.AppException
{
    public class ExceedMaxModificationException : Exception
    {
        private static readonly string DefaultMessage = "" +
            "You have reached the maximum number of modifications for today. " +
            "Please try again tomorrow.";
        public ExceedMaxModificationException() : base(DefaultMessage)
        {
        }
    }
    public class ExceedMaxTodoItemsPerDueDateException : Exception
    {
        private static readonly string DefaultMessage = "" +
            "You have reached the maximum number of todo items for one due date. " +
            "Please try again.";

        public ExceedMaxTodoItemsPerDueDateException() : base(DefaultMessage)
        {
        }
    }
    public class InvalidDueDateException : Exception
    {
        private static readonly string DefaultMessage = "" +
            "The due date is invalid. " +
            "Please try again.";

        public InvalidDueDateException() : base(DefaultMessage)
        {
        }
    }
    //havn't used
    public class InvalidCreateOption : Exception
    {
        private static readonly string DefaultMessage =  "" +
            "The create option is invalid." +
            "Please try again.";
        public InvalidCreateOption() : base(DefaultMessage)
        {
        }
    }

}
