﻿using Xunit;
using JLL.PizzaProblem.API.Services;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using JLL.PizzaProblem.API.Models;
using Microsoft.Extensions.Options;
using Moq;
using JLL.PizzaProblem.API.Profiles;
using JLL.PizzaProblem.DataAccess.EF;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using JLL.PizzaProblem.API.Middleware;
using JLL.PizzaProblem.Services;

namespace JLL.PizzaProblem.API.Services.Tests
{
    public class UserServiceTests
    {
        private readonly AppSettings _testingSettings;
        private readonly IOptions<AppSettings> _testingOptions;
        private readonly IMapper _mockMapper;
        private IUserService _testingService;
        private PizzaProblemContext _context;

        public UserServiceTests()
        {
            _testingSettings = new AppSettings();
            _testingSettings.Secret = "THIS IS MY VERY LONG TESTING SECRET THAT NO ONE SHOULD KNOW";
            // Arrange testing service for all the testing class
            _testingOptions = Options.Create(_testingSettings);

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new UsersProfile()));
            _mockMapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public async Task GetAll_ShouldReturn_NotNullCollectionInitially()
        {
            // Arrange
            _context = new PizzaProblemContext(
               new DbContextOptionsBuilder<PizzaProblemContext>()
                           .UseInMemoryDatabase(databaseName: "UserServiceTests")
                           .Options);
            _context.Database.EnsureCreated();
            _testingService = new UserService(_testingOptions, _mockMapper, _context);

            // Act
            var listOfUsers = await _testingService.GetAllAsync();

            // Assert
            Assert.NotNull(listOfUsers);
        }

        [Fact]
        public async Task GetAll_ShouldReturn_LenghtOfTwoInitially()
        {
            // Arrange
            _context = new PizzaProblemContext(
               new DbContextOptionsBuilder<PizzaProblemContext>()
                           .UseInMemoryDatabase(databaseName: "GetAll")
                           .Options);
            _context.Database.EnsureCreated();
            _testingService = new UserService(_testingOptions, _mockMapper, _context);

            // Act
            var listOfUsers = await _testingService.GetAllAsync();

            // Assert
            Assert.Equal(2, listOfUsers.Count);
        }

        [Fact]
        public async Task GetById_ShouldReturn_ValidUser()
        {
            // Arrange
            _context = new PizzaProblemContext(
               new DbContextOptionsBuilder<PizzaProblemContext>()
                           .UseInMemoryDatabase(databaseName: "UserServiceTests")
                           .Options);
            _context.Database.EnsureCreated();
            _testingService = new UserService(_testingOptions, _mockMapper, _context);

            // Act
            var firstUser = await _testingService.GetByIdAsync(1);

            // Assert
            Assert.Equal(1, firstUser.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturn_NullForNonExistentUser()
        {
            // Arrange
            _context = new PizzaProblemContext(
               new DbContextOptionsBuilder<PizzaProblemContext>()
                           .UseInMemoryDatabase(databaseName: "UserServiceTests")
                           .Options);
            _context.Database.EnsureCreated();
            _testingService = new UserService(_testingOptions, _mockMapper, _context);

            // Act
            var firstUser = await _testingService.GetByIdAsync(0);

            // Assert
            Assert.Null(firstUser);
        }

        [Fact]
        public async Task AddNewUser_ShouldCreate_ANewIdForTheNewUser()
        {
            // Arrange
            _context = new PizzaProblemContext(
               new DbContextOptionsBuilder<PizzaProblemContext>()
                           .UseInMemoryDatabase(databaseName: "AddNewUser")
                           .Options);
            _context.Database.EnsureCreated();
            _testingService = new UserService(_testingOptions, _mockMapper, _context);
            var newUser = new UserDto { Id = 0, FirstName = "Test", LastName = "Test", Username = "test", Password = "test" };

            // Act
            var addedUser = await _testingService.AddNewUserAsync(newUser);

            // Assert
            Assert.Equal(3, addedUser.Id);
        }

        [Fact]
        public async Task AddNewUser_ShouldCreate_AValidUser()
        {
            // Arrange
            _context = new PizzaProblemContext(
               new DbContextOptionsBuilder<PizzaProblemContext>()
                           .UseInMemoryDatabase(databaseName: "UserServiceTests")
                           .Options);
            _context.Database.EnsureCreated();
            _testingService = new UserService(_testingOptions, _mockMapper, _context);
            var newUser = new UserDto { Id = 0, FirstName = "Test", LastName = "Test", Username = "test", Password = "test" };

            // Act
            var addedUser = await _testingService.AddNewUserAsync(newUser);

            // Assert
            Assert.IsType<UserDto>(addedUser);
        }

        [Fact]
        public async Task Authenticate_ShouldReturn_NullForInvalidUser()
        {
            // Arrange
            _context = new PizzaProblemContext(
               new DbContextOptionsBuilder<PizzaProblemContext>()
                           .UseInMemoryDatabase(databaseName: "UserServiceTests")
                           .Options);
            _context.Database.EnsureCreated();
            _testingService = new UserService(_testingOptions, _mockMapper, _context);
            var newAuthenticateRequest = new AuthenticateRequestDto
            {
                Username = "notFoundUser",
                Password = "test"
            };

            // Act
            var response = await _testingService.AuthenticateAsync(newAuthenticateRequest);

            // Assert
            Assert.Null(response);
        }

        [Fact]
        public async Task Authenticate_ShouldReturn_NullForInvalidPassword()
        {
            // Arrange
            _context = new PizzaProblemContext(
               new DbContextOptionsBuilder<PizzaProblemContext>()
                           .UseInMemoryDatabase(databaseName: "UserServiceTests")
                           .Options);
            _context.Database.EnsureCreated();
            _testingService = new UserService(_testingOptions, _mockMapper, _context);
            var newAuthenticateRequest = new AuthenticateRequestDto
            {
                Username = "test",
                Password = "notAValidPassword"
            };

            // Act
            var response = await _testingService.AuthenticateAsync(newAuthenticateRequest);

            // Assert
            Assert.Null(response);
        }

        [Fact]
        public async Task Authenticate_ShouldReturn_AValidResponseForValidUser()
        {
            // Arrange
            _context = new PizzaProblemContext(
               new DbContextOptionsBuilder<PizzaProblemContext>()
                           .UseInMemoryDatabase(databaseName: "Authenticate")
                           .Options);
            _context.Database.EnsureCreated();
            _testingService = new UserService(_testingOptions, _mockMapper, _context);
            var newAuthenticateRequest = new AuthenticateRequestDto
            {
                Username = "test",
                Password = "test"
            };

            // Act
            var response = await _testingService.AuthenticateAsync(newAuthenticateRequest);

            // Assert
            Assert.Equal(1, response.Id);
        }

        [Fact]
        public async Task UpdateUser_Should_DoNothingForInvalidUser()
        {
            // Arrange
            _context = new PizzaProblemContext(
               new DbContextOptionsBuilder<PizzaProblemContext>()
                           .UseInMemoryDatabase(databaseName: "UpdateUser")
                           .Options);
            _context.Database.EnsureCreated();
            _testingService = new UserService(_testingOptions, _mockMapper, _context);
            var invalidUser = new UserDto
            {
                Id = 10,
                Username = "test",
                Password = "test",
                FirstName = "",
                LastName = "",
                PizzaLove = 0
            };

            // Act
            await _testingService.UpdateUserAsync(invalidUser);

            // Assert
            Assert.True(await _testingService.GetByIdAsync(invalidUser.Id) == null);
        }

        [Fact]
        public async Task UpdateUser_Should_UpdateValidUser()
        {
            // Arrange
            _context = new PizzaProblemContext(
               new DbContextOptionsBuilder<PizzaProblemContext>()
                           .UseInMemoryDatabase(databaseName: "UserServiceTests")
                           .Options);
            _context.Database.EnsureCreated();
            _testingService = new UserService(_testingOptions, _mockMapper, _context);
            var validUser = new UserDto
            {
                Id = 1,
                Username = "Test",
                Password = "Test",
                FirstName = "test",
                LastName = "test",
                PizzaLove = 3
            };

            // Act
            await _testingService.UpdateUserAsync(validUser);

            // Assert
            var user = await _testingService.GetByIdAsync(1);
            Assert.Equal(3, user.PizzaLove);
        }

        [Fact]
        public async Task GetTopTenPizzaLove_ShouldReturn_AListOfUsers()
        {
            // Arrange
            _context = new PizzaProblemContext(
               new DbContextOptionsBuilder<PizzaProblemContext>()
                           .UseInMemoryDatabase(databaseName: "GetTopTenPizzaLove")
                           .Options);
            _context.Database.EnsureCreated();
            _testingService = new UserService(_testingOptions, _mockMapper, _context);

            // Act
            var list = await _testingService.GetTopTenPizzaLoveAsync();

            // Assert
            Assert.Equal(2, list.Count);
        }
    }
}