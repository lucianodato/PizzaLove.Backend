using Xunit;
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
        public void IncreasePizzaLoveForUser_ShouldIncreasePizzaLove_ForValidUser()
        {
            // Act
            _testingService.IncreasePizzaLoveForUser(1);

            // Assert
            Assert.Equal(2, _testingService.GetById(1).PizzaLove);
        }

        [Fact]
        public void IncreasePizzaLoveForUser_ShouldNotIncreasePizzaLove_ForInValidUser()
        {
            // Act
            _testingService.IncreasePizzaLoveForUser(0);

            // Assert
            Assert.Equal(1, _testingService.GetById(1).PizzaLove);
            Assert.Equal(3, _testingService.GetById(2).PizzaLove);
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