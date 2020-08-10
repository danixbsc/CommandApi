using System;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using CommandAPI.Controllers;
using CommandAPI.Models;

namespace CommandAPI.Tests
{
    public class CommandsControllerTests: IDisposable
    {
        DbContextOptionsBuilder<CommandContext> options;
        CommandContext dbContext;
        CommandsController controller;

        public CommandsControllerTests()
        {
            // ARRANGE
            options = new DbContextOptionsBuilder<CommandContext>();
            options.UseInMemoryDatabase("UnitTestInMemDb");
            dbContext = new CommandContext(options.Options);

            controller = new CommandsController(dbContext);
        }

        public void Dispose()
        {
            options = null;
            foreach(var item in dbContext.CommandItems)
            {
                dbContext.CommandItems.Remove(item);
            }
            dbContext.SaveChanges();
            dbContext.Dispose();
            controller = null;
        }

        [Fact]
        public void GetCommanditems_ReturnZeroItems_WhenDbIsEmpty()
        {
            // ACT
            var items = controller.GetCommandsItem();

            // ASSERT
            Assert.Empty(items.Value);
        }

        [Fact]
        public void GetCommanditems_ReturnOneItem_WhenDbAsOneObject()
        {
            // ARRANGE
            Command command = new Command 
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = "Some Commandline"
            };

            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            // ACT
            var items = controller.GetCommandsItem();

            // ASSERT
            Assert.Single(items.Value);
        }

        [Fact]
        public void GetCommanditems_ReturnNItem_WhenDbAsNObjects()
        {
            // ARRANGE
            for(int i = 0; i < 4; i++)
            {
                Command command = new Command 
                {
                    HowTo = "Do Something" + i,
                    Platform = "Some Platform" + i,
                    CommandLine = "Some Commandline" + i
                };

                dbContext.CommandItems.Add(command);
            }
            
            dbContext.SaveChanges();

            // ACT
            var items = controller.GetCommandsItem();

            // ASSERT
            Assert.Equal(4, items.Value.Count());
        }

        [Fact]
        public void GetCommandsItems_ReturnTheCorrectType() 
        {
            // ACT
            var items = controller.GetCommandsItem();
            
            // ASSERT
            Assert.IsType<ActionResult<IEnumerable<Command>>>(items);
        }

        [Fact]
        public void GetCommandItem_ReturnsNullResults_WhenInvalidId()
        {
            // ACT
            var command = controller.GetCommandItem(0);

            // ASSERT
            Assert.Null(command.Value);
        }

        [Fact]
        public void GetCommandItem_Return404NotFound_WhenInvalidId()
        {
            // ACT
            var command = controller.GetCommandItem(0);

            // ASSERT
            Assert.IsType<NotFoundResult>(command.Result);
        }

        [Fact]
        public void GetCommandItem_ReturnCorrectType()
        {
            // ARRANGE
            var command = new Command()
            {
                HowTo = "Do something",
                Platform = "Some platform",
                CommandLine = "Soma command line"
            };

            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            var id = command.Id;

            // ACT
            var commandReturned = controller.GetCommandItem(id);

            // ASSERT
            Assert.IsType<ActionResult<Command>>(commandReturned);
        }
    }
}