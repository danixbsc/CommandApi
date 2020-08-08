using System;
using Xunit;
using CommandAPI.Models;

namespace CommandAPI.Tests
{
    public class CommandTest : IDisposable
    {
        Command testCommand;

        public CommandTest()
        {
            testCommand = new Command 
            {
                HowTo = "Do something awesome",
                Platform = "xUnit",
                CommandLine = "dotnet test"
            };
        }

        public void Dispose()
        {
            testCommand = null;
        }

        [Fact]
        public void CanChangeHowTo()
        {
            // Act
            testCommand.HowTo = "Execute unit test";

            // Assert
            Assert.Equal("Execute unit test", testCommand.HowTo);
        }

        [Fact]
        public void CanChangePlatform()
        {
            // Act
            testCommand.Platform = "MSUnit";

            // Assert
            Assert.Equal("MSUnit", testCommand.Platform);
        }

        [Fact]
        public void CanChangeCommandLine()
        {
            // Act
            testCommand.CommandLine = "dotnet mstest";

            // Assert
            Assert.Equal("dotnet mstest", testCommand.CommandLine);
        }
    }
}