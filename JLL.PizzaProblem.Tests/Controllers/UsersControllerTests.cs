using Xunit;
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
using Microsoft.AspNetCore.JsonPatch;
using System.Threading.Tasks;

namespace JLL.PizzaProblem.API.Controllers.Tests
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
                new User { Id = 1, FirstName = "Test", LastName = "Test", Username = "test", Password = "test", PizzaLove = 1 },
                new User { Id = 2, FirstName = "User", LastName = "User", Username = "user", Password = "user", PizzaLove = 3 }
            };
            _mockUserService.Setup(x => x.GetAllAsync()).ReturnsAsync(_usersExample);
            _mockUserService.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(_usersExample[0]);
            _mockUserService.Setup(x => x.GetByIdAsync(3)).ReturnsAsync(_usersExample[0]);
            _mockUserService.Setup(x => x.AddNewUserAsync(It.IsAny<User>())).ReturnsAsync(_usersExample[1]);
            _mockUserService
                .Setup(x => x.AuthenticateAsync(It.Is<AuthenticateRequest>(i => i.Username == "userrrr")))
                .ReturnsAsync((AuthenticateResponse)null);
            _mockUserService
                .Setup(x => x.AuthenticateAsync(It.Is<AuthenticateRequest>(i => i.Password == "userrrr")))
                .ReturnsAsync((AuthenticateResponse)null);
            _mockUserService
                .Setup(x => x.AuthenticateAsync(It.Is<AuthenticateRequest>(i => i.Username == "user" && i.Password == "user")))
                .ReturnsAsync(new AuthenticateResponse());
            _mockUserService.Setup(x => x.GetTopTenPizzaLoveAsync()).ReturnsAsync(_usersExample);

            _userController = new UsersController(_mockUserService.Object, _mockMapper);
        }


        [Fact]
        public async Task GetAll_WhenCalled_ReturnsOkResult()
        {
            // Act
            var okResult = await _userController.GetAllAsync();

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public async Task GetAll_WhenCalled_ReturnsAllUsers()
        {
            // Act
            var okObject = await _userController.GetAllAsync();

            // Assert
            var okObjectResult = okObject.Result as ObjectResult;
            List<User> users = Assert.IsType<List<User>>(okObjectResult.Value);
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public async Task GetUser_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var okResult = await _userController.GetUserAsync(0);

            // Assert
            Assert.IsType<NotFoundResult>(okResult.Result);
        }

        [Fact]
        public async Task GetUser_WithValidId_ReturnsOkResult()
        {
            // Act
            var okResult = await _userController.GetUserAsync(1);

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public async Task GetUser_WithValidId_ReturnsUser()
        {
            // Act
            var okObject = await _userController.GetUserAsync(1);

            // Assert
            var okObjectResult = okObject.Result as ObjectResult;
            User returnedUser = Assert.IsType<User>(okObjectResult.Value);
            Assert.Equal(1, returnedUser.Id);
        }

        [Fact]
        public async Task Post_WithValidUser_ReturnsCreatedAtRouteResult()
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
            var createdAtRouteResult = await _userController.PostUserAsync(newUser);

            // Assert
            Assert.IsType<CreatedAtRouteResult>(createdAtRouteResult.Result);
        }

        [Fact]
        public async Task Post_WithValidUser_ReturnsCreatedUser()
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
            var createdAtRouteResult = await _userController.PostUserAsync(newUser);

            // Assert
            var result = createdAtRouteResult.Result as CreatedAtRouteResult;
            Assert.IsType<User>(result.Value);
        }

        [Fact]
        public async Task Authenticate_ShouldReturn_BadRequestWhenUserIsInvalid()
        {
            // Arrange
            var newAuthenticationRequest = new AuthenticateRequest
            {
                Username = "userrrr",
                Password = "user"
            };

            // Act
            var response = await _userController.AuthenticateAsync(newAuthenticationRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async Task Authenticate_ShouldReturn_BadRequestWhenPasswordIsInvalid()
        {
            // Arrange
            var newAuthenticationRequest = new AuthenticateRequest
            {
                Username = "user",
                Password = "userrrr"
            };

            // Act
            var response = await _userController.AuthenticateAsync(newAuthenticationRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Fact]
        public async Task Authenticate_ShouldReturn_OkResultWhenUserIsValid()
        {
            // Arrange
            var newAuthenticationRequest = new AuthenticateRequest
            {
                Username = "user",
                Password = "user"
            };

            // Act
            var response = await _userController.AuthenticateAsync(newAuthenticationRequest);

            // Assert
            Assert.IsType<OkObjectResult>(response.Result);
        }

        [Fact]
        public async Task GetTopTenUser_ShouldReturn_AListOfTenUsers()
        {
            // Act
            var response = await _userController.GetTopTenUserAsync();

            // Assert
            var okObjectResult = response.Result as OkObjectResult;
            List<User> users = Assert.IsType<List<User>>(okObjectResult.Value);
            Assert.True(users.Count <= 10);
            //TODO Assert that the list is indeed ordered
        }

        [Fact]
        public async Task Patch_ShouldReturn_NotFoundForNotFoundUser()
        {
            // Act
            var response = await _userController.PatchAsync(0, new JsonPatchDocument<UserForPatch>());

            // Assert
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task Patch_ShouldReturn_BadRequestForNullChanges()
        {
            // Act
            var response = await _userController.PatchAsync(1, null);

            // Assert
            Assert.IsType<BadRequestResult>(response);
        }

        [Fact]
        public async Task Patch_ShouldReturn_NoContentForFoundUserAndValidUpdate()
        {
            // Arrange
            var jsonObject = new JsonPatchDocument<UserForPatch>();
            var userToUpdate = new UserForPatch
            {
                FirstName = "Test",
                LastName = "Test",
                Username = "test",
                Password = "test",
                PizzaLove = 19
            };
            jsonObject.Replace(i => i.PizzaLove, userToUpdate.PizzaLove);

            // Act
            var response = await _userController.PatchAsync(1, jsonObject);

            // Assert
            Assert.IsType<NoContentResult>(response);
        }
    }
}