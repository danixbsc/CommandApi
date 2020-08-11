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

        [Fact]
        public void GetCommandItem_ReturnCorrectResource_WhenIdIsCorrect()
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
            Assert.Equal(commandReturned.Value.Id, id);
        }

        [Fact]
        public void PostCommandItem_ItemCountIncrementBy1_WhenValidObject() 
        {
            // Arrange
            Command command = new Command() 
            {
                HowTo = "Do something",
                Platform = "Some platoforme",
                CommandLine = "Some command"
            };

            int oldCount = dbContext.CommandItems.Count();

            // ACT
            controller.PostCommandItem(command);
            
            // ASSERT
            Assert.Equal(oldCount + 1, dbContext.CommandItems.Count());
            
        }

        
        [Fact]
        public void PostCommandItem_Returns201_WhenValidObject() 
        {
            // Arrange
            Command command = new Command() 
            {
                HowTo = "Do something",
                Platform = "Some platoforme",
                CommandLine = "Some command"
            };

            // ACT
            var result = controller.PostCommandItem(command);
            
            // ASSERT
            Assert.IsType<CreatedAtActionResult>(result.Result); 
            
        }

        [Fact]
        public void PutCommandItem_AttributeUpdated_WhenValidObject()
        {
            // ARRANGE
            Command command = new Command {
                HowTo = "Do Somenthing",
                Platform = "Some platform",
                CommandLine = "Some command line"
            };

            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();
            
            // ACT
            command.HowTo = "Do something else";

            controller.PutCommandItem(command.Id, command);
            var result = dbContext.CommandItems.Find(command.Id);

            //ASSERT
            Assert.Equal("Do something else", result.HowTo);
        }

        [Fact]
        public void PutCommandItem_Returns204_WhenValidObject()
        {
            // ARRANGE
            Command command = new Command {
                HowTo = "Do Somenthing",
                Platform = "Some platform",
                CommandLine = "Some command line"
            };

            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();
            
            // ACT
            command.HowTo = "Do something else";

            var result = controller.PutCommandItem(command.Id, command);
            
            //ASSERT
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void PutCommandItem_Returns400BadRequest_WhenInvalidObjectId()
        {
            // ARRANGE
            Command command = new Command {
                HowTo = "Do something",
                Platform = "platform",
                CommandLine = "command"
            };

            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            // ACT
            int cmdId = command.Id + 1;
            command.HowTo = "Do something else";

            var result = controller.PutCommandItem(cmdId, command);

            // ASSERT
            Assert.IsType<BadRequestResult>(result);
        }
        
        [Fact]
        public void PutCommandItem_ItemUnchanged_WhenInvalidObjectId()
        {
            // ARRANGE
            Command command = new Command {
                HowTo = "Do something",
                Platform = "platform",
                CommandLine = "command"
            };

            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();
            // ACT
            Command command2 = new Command {
                Id = command.Id,
                HowTo = "Do something else",
                Platform = "platform",
                CommandLine = "command"
            };

            controller.PutCommandItem(command.Id + 1, command2);
            var result = dbContext.CommandItems.Find(command.Id);
            // ASSERT
            Assert.Equal("Do something", result.HowTo);
        }

        [Fact]
        public void DeleteCommandItem_Returns200Ok_WhenValidObjectId()
        {
            // ARRANGE
            Command command = new Command {
                HowTo = "Do something",
                Platform = "platform",
                CommandLine = "command"
            };
            
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            // ACT
            var result = controller.DeleteCommandItem(command.Id);
            
            // ASSERT
            Assert.IsType<OkResult>(result);
        }
        
        [Fact]
        public void DeleteCommandItem_Returns400BadRequest_WhenInvalidObjectId()
        {
            // ARRANGE
            Command command = new Command {
                HowTo = "Do something",
                Platform = "platform",
                CommandLine = "command"
            };
            
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            // ACT
            var result = controller.DeleteCommandItem(command.Id + 1);
            
            // ASSERT
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void DeleteCommandItem_ItemsCountNotChange_WhenInvalidObjectId()
        {
            // ARRANGE
            Command command = new Command {
                HowTo = "Do something",
                Platform = "platform",
                CommandLine = "command"
            };
            
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            int oldCount = dbContext.CommandItems.Count();

            // ACT
            var result = controller.DeleteCommandItem(command.Id + 1);
            int actualCount =dbContext.CommandItems.Count();

            // ASSERT
            Assert.Equal(oldCount, actualCount);
        }
        
    }
}