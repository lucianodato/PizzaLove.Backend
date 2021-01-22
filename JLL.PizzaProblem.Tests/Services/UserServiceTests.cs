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

namespace JLL.PizzaProblem.API.Services.Tests
{
    public class UserServiceTests
    {
        private readonly AppSettings _testingSettings;
        private readonly IOptions<AppSettings> _testingOptions;
        private readonly IMapper _mockMapper;
        private readonly IUserService _testingService;

        public UserServiceTests()
        {
            _testingSettings = new AppSettings();
            _testingSettings.Secret = "THIS IS MY VERY LONG TESTING SECRET THAT NO ONE SHOULD KNOW";
            // Arrange testing service for all the testing class
            _testingOptions = Options.Create(_testingSettings);

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new UsersProfile()));
            _mockMapper = mapperConfig.CreateMapper();

            _testingService = new UserService(_testingOptions, _mockMapper);
        }

        [Fact]
        public void GetAll_ShouldReturn_NotNullCollectionInitially()
        {
            // Act
            var listOfUsers = _testingService.GetAll();

            // Assert
            Assert.NotNull(listOfUsers);
        }

        [Fact]
        public void GetAll_ShouldReturn_LenghtOfTwoInitially()
        {
            // Act
            var listOfUsers = _testingService.GetAll();

            // Assert
            Assert.Equal(2, listOfUsers.Count);
        }

        [Fact]
        public void GetById_ShouldReturn_ValidUser()
        {
            // Act
            var firstUser = _testingService.GetById(1);

            // Assert
            Assert.Equal(1, firstUser.Id);
        }

        [Fact]
        public void GetById_ShouldReturn_NullForNonExistentUser()
        {
            // Act
            var firstUser = _testingService.GetById(0);

            // Assert
            Assert.Null(firstUser);
        }

        [Fact]
        public void AddNewUser_ShouldCreate_ANewIdForTheNewUser()
        {
            // Arrange
            var newUser = new User { Id = 0, FirstName = "Test", LastName = "Test", Username = "test", Password = "test" };

            // Act
            var addedUser = _testingService.AddNewUser(newUser);

            // Assert
            Assert.Equal(3, addedUser.Id);
        }

        [Fact]
        public void AddNewUser_ShouldCreate_AValidUser()
        {
            // Arrange
            var newUser = new User { Id = 0, FirstName = "Test", LastName = "Test", Username = "test", Password = "test" };

            // Act
            var addedUser = _testingService.AddNewUser(newUser);

            // Assert
            Assert.IsType<User>(addedUser);
        }

        [Fact]
        public void Authenticate_ShouldReturn_NullForInvalidUser()
        {
            // Arrange
            var newAuthenticateRequest = new AuthenticateRequest
            {
                Username = "notFoundUser",
                Password = "test"
            };

            // Act
            var response = _testingService.Authenticate(newAuthenticateRequest);

            // Assert
            Assert.Null(response);
        }

        [Fact]
        public void Authenticate_ShouldReturn_NullForInvalidPassword()
        {
            // Arrange
            var newAuthenticateRequest = new AuthenticateRequest
            {
                Username = "test",
                Password = "notAValidPassword"
            };

            // Act
            var response = _testingService.Authenticate(newAuthenticateRequest);

            // Assert
            Assert.Null(response);
        }

        [Fact]
        public void Authenticate_ShouldReturn_AValidResponseForValidUser()
        {
            // Arrange
            var newAuthenticateRequest = new AuthenticateRequest
            {
                Username = "test",
                Password = "test"
            };

            // Act
            var response = _testingService.Authenticate(newAuthenticateRequest);

            // Assert
            Assert.Equal(1, response.Id);
        }

        [Fact]
        public void UpdateUser_Should_DoNothingForInvalidUser()
        {
            // Arrange
            var invalidUser = new User
            {
                Id = 0,
                Username = "test",
                Password = "test",
                FirstName = "",
                LastName = "",
                PizzaLove = 0
            };

            // Act
            _testingService.UpdateUser(invalidUser);

            // Assert
            Assert.True(_testingService.GetById(invalidUser.Id) == null);
        }

        [Fact]
        public void UpdateUser_Should_UpdateValidUser()
        {
            // Arrange
            var validUser = new User
            {
                Id = 1,
                Username = "Test",
                Password = "Test",
                FirstName = "test",
                LastName = "test",
                PizzaLove = 3
            };

            // Act
            _testingService.UpdateUser(validUser);

            // Assert
            Assert.Equal(3, _testingService.GetById(1).PizzaLove);
        }

        [Fact]
        public void GetTopTenPizzaLove_ShouldReturn_AListOfUsers()
        {
            // Act
            var list = _testingService.GetTopTenPizzaLove();

            // Assert
            Assert.Equal(2, list.Count);
        }
    }
}