﻿using Xunit;
using JLL.PizzaProblem.API.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using JLL.PizzaProblem.API.Profiles;
using JLL.PizzaProblem.API.Services;
using JLL.PizzaProblem.API.Models;
using Microsoft.Extensions.Options;
using Moq;
using Microsoft.AspNetCore.Mvc;

namespace JLL.PizzaProblem.Controllers.Tests
{
    public class UsersControllerTests
    {
        private readonly IMapper _mockMapper;
        private readonly Mock<IUserService> _mockUserService;
        private readonly List<User> _usersExample;
        private readonly UsersController _userController;

        public UsersControllerTests()
        {
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new UsersProfile()));
            _mockMapper = mapperConfig.CreateMapper();

            _mockUserService = new Mock<IUserService>();
            _usersExample = new List<User>
            {
                new User { Id = 1, FirstName = "Test", LastName = "Test", Username = "test", Password = "test" },
                new User { Id = 2, FirstName = "User", LastName = "User", Username = "user", Password = "user" }
            };
            _mockUserService.Setup(x => x.GetAll()).Returns(_usersExample);
            _mockUserService.Setup(x => x.GetById(1)).Returns(_usersExample[0]);
            _mockUserService.Setup(x => x.GetById(3)).Returns(_usersExample[0]);
            _mockUserService.Setup(x => x.AddNewUser(It.IsAny<User>())).Returns(_usersExample[1]);
            _mockUserService
                .Setup(x => x.Authenticate(It.Is<AuthenticateRequest>(i => i.Username == "userrrr")))
                .Returns((AuthenticateResponse)null);
            _mockUserService
                .Setup(x => x.Authenticate(It.Is<AuthenticateRequest>(i => i.Password == "userrrr")))
                .Returns((AuthenticateResponse)null);
            _mockUserService
                .Setup(x => x.Authenticate(It.Is<AuthenticateRequest>(i => i.Username == "user" && i.Password == "user")))
                .Returns(new AuthenticateResponse());

            _userController = new UsersController(_mockUserService.Object, _mockMapper);
        }


        [Fact]
        public void GetAll_WhenCalled_ReturnsOkResult()
        {
            // Act
            var okResult = _userController.GetAll();

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public void GetAll_WhenCalled_ReturnsAllUsers()
        {
            // Act
            var okObject = _userController.GetAll();

            // Assert
            var okObjectResult = okObject.Result as ObjectResult;
            List<User> users = Assert.IsType<List<User>>(okObjectResult.Value);
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public void GetUser_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var okResult = _userController.GetUser(0);

            // Assert
            Assert.IsType<NotFoundResult>(okResult.Result);
        }

        [Fact]
        public void GetUser_WithValidId_ReturnsOkResult()
        {
            // Act
            var okResult = _userController.GetUser(1);

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public void GetUser_WithValidId_ReturnsUser()
        {
            // Act
            var okObject = _userController.GetUser(1);

            // Assert
            var okObjectResult = okObject.Result as ObjectResult;
            User returnedUser = Assert.IsType<User>(okObjectResult.Value);
            Assert.Equal(1, returnedUser.Id);
        }

        [Fact]
        public void Post_WithValidUser_ReturnsCreatedAtRouteResult()
        {
            // Arrange
            var newUser = new UserForCreation
            {
                FirstName = "User",
                LastName = "User",
                Username = "user",
                Password = "user"
            };

            // Act
            var createdAtRouteResult = _userController.Post(newUser);

            // Assert
            Assert.IsType<CreatedAtRouteResult>(createdAtRouteResult.Result);
        }

        [Fact]
        public void Post_WithValidUser_ReturnsCreatedUser()
        {
            // Arrange
            var newUser = new UserForCreation
            {
                FirstName = "User",
                LastName = "User",
                Username = "user",
                Password = "user"
            };

            // Act
            var createdAtRouteResult = _userController.Post(newUser);
            
            // Assert
            var result = createdAtRouteResult.Result as CreatedAtRouteResult;
            Assert.IsType<User>(result.Value);
        }

        [Fact]
        public void Authenticate_ShouldReturn_BadRequestWhenUserIsInvalid()
        {
            // Arrange
            var newAuthenticationRequest = new AuthenticateRequest
            {
                Username = "userrrr",
                Password = "user"
            };

            // Act
            var response = _userController.Authenticate(newAuthenticationRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public void Authenticate_ShouldReturn_BadRequestWhenPasswordIsInvalid()
        {
            // Arrange
            var newAuthenticationRequest = new AuthenticateRequest
            {
                Username = "user",
                Password = "userrrr"
            };

            // Act
            var response = _userController.Authenticate(newAuthenticationRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public void Authenticate_ShouldReturn_OkResultWhenUserIsValid()
        {
            // Arrange
            var newAuthenticationRequest = new AuthenticateRequest
            {
                Username = "user",
                Password = "user"
            };

            // Act
            var response = _userController.Authenticate(newAuthenticationRequest);

            // Assert
            Assert.IsType<OkObjectResult>(response.Result);
        }
    }
}