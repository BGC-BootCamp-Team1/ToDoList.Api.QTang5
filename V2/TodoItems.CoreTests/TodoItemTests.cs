using Microsoft.VisualStudio.TestTools.UnitTesting;
using TodoItems.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoItems.Core.AppException;

namespace TodoItems.Core.Tests
{
    [TestClass]
    public class TodoItemTests
    {
        [TestMethod]
        public void ModifyItem_ShouldUpdateDescription_WhenUnderLimit()
        {
            // Arrange
            var todoItem = new TodoItem("Initial Description", DateOnly.FromDateTime(DateTime.Now.AddDays(1)));

            // Act
            todoItem.ModifyItem("Modified Description");

            // Assert
            Assert.AreEqual("Modified Description", todoItem.Description);
            Assert.AreEqual(1, todoItem.ModificationHistory.Count);
        }

        [TestMethod]
        public void ModifyItem_ShouldThrowException_WhenOverLimit()
        {
            // Arrange
            var todoItem = new TodoItem("Initial Description", DateOnly.FromDateTime(DateTime.Now.AddDays(1)));

            // Add three modifications for today
            todoItem.ModifyItem("Modified Description 1");
            todoItem.ModifyItem("Modified Description 2");
            todoItem.ModifyItem("Modified Description 3");

            // Assert is handled by ExpectedException
            Assert.ThrowsException<ExceedMaxModificationException>(() => todoItem.ModifyItem("Modified Description 4"));
        }

        [TestMethod]
        public void ModifyItem_ShouldAllowModification_WhenNewDay()
        {
            // Arrange
            var todoItem = new TodoItem("Initial Description", DateOnly.FromDateTime(DateTime.Now.AddDays(1)));

            // Add three modifications for today
            todoItem.ModifyItem("Modified Description 1");
            todoItem.ModifyItem("Modified Description 2");
            todoItem.ModifyItem("Modified Description 3");

            // Simulate a new day
            foreach (var mod in todoItem.ModificationHistory)
            {
                mod.TimesStamp = mod.TimesStamp.AddDays(-1);
            }

            // Act
            todoItem.ModifyItem("Modified Description New Day");

            // Assert
            Assert.AreEqual("Modified Description New Day", todoItem.Description);
            Assert.AreEqual(4, todoItem.ModificationHistory.Count);
        }


    }
}
