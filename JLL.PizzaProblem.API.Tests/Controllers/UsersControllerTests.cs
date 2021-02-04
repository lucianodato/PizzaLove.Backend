using AutoMapper;
using JLL.PizzaProblem.Domain;
using JLL.PizzaProblem.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using JLL.PizzaProblem.API.Dtos;
using JLL.PizzaProblem.API.Middleware;
using Microsoft.Extensions.Options;
using JLL.PizzaProblem.API.Profiles;

namespace JLL.PizzaProblem.API.Controllers.Tests
{
    public class UsersControllerTests
    {
        private readonly IMapper _mockMapper;
        private readonly Mock<IUserService> _mockUserService;
        private readonly AppSettings _appSettings;
        private readonly IOptions<AppSettings> _appOptions;
        private readonly List<User> _usersExample;
        private readonly UsersController _userController;

        public UsersControllerTests()
        {
            _appSettings = new AppSettings();
            _appSettings.Secret = "THIS IS MY VERY LONG TESTING SECRET THAT NO ONE SHOULD KNOW";
            // Arrange testing service for all the testing class
            _appOptions = Options.Create(_appSettings);

            var mapperConfig = new MapperConfiguration(cfg => {
                cfg.AddProfile<UsersProfile>();
                cfg.AddProfile<AuthenticateProfile>();
            }); _mockMapper = mapperConfig.CreateMapper();

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

            _userController = new UsersController(_appOptions, _mockUserService.Object, _mockMapper);
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
            List<UserDto> users = Assert.IsType<List<UserDto>>(okObjectResult.Value);
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
            UserDto returnedUser = Assert.IsType<UserDto>(okObjectResult.Value);
            Assert.Equal(1, returnedUser.Id);
        }

        [Fact]
        public async Task Post_WithValidUser_ReturnsCreatedAtRouteResult()
        {
            // Arrange
            var newUser = new UserForCreationDto
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
            var newUser = new UserForCreationDto
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
            Assert.IsType<UserDto>(result.Value);
        }

        [Fact]
        public async Task Authenticate_ShouldReturn_BadRequestWhenUserIsInvalid()
        {
            // Arrange
            var newAuthenticationRequest = new AuthenticateRequestDto
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
            var newAuthenticationRequest = new AuthenticateRequestDto
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
            var newAuthenticationRequest = new AuthenticateRequestDto
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
            List<UserDto> users = Assert.IsType<List<UserDto>>(okObjectResult.Value);
            Assert.True(users.Count <= 10);
            //TODO Assert that the list is indeed ordered
        }

        [Fact]
        public async Task Patch_ShouldReturn_NotFoundForNotFoundUser()
        {
            // Act
            var response = await _userController.PatchAsync(0, new JsonPatchDocument<UserForPatchDto>());

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
            var jsonObject = new JsonPatchDocument<UserForPatchDto>();
            var userToUpdate = new UserForPatchDto
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