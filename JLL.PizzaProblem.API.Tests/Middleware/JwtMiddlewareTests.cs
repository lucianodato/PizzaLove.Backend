using Xunit;
using JLL.PizzaProblem.API.Middleware;
using System;
using System.Collections.Generic;
using System.Text;
using JLL.PizzaProblem.Domain;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using JLL.PizzaProblem.Services;
using AutoMapper;
using JLL.PizzaProblem.DataAccess.EF;
using Microsoft.EntityFrameworkCore;
using JLL.PizzaProblem.API.Profiles;
using JLL.PizzaProblem.API.Dtos;
using Moq;
using JLL.PizzaProblem.API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace JLL.PizzaProblem.API.Middleware.Tests
{
    public class JwtMiddlewareTests
    {
        private readonly AppSettings _testingSettings;
        private readonly IMapper _mockMapper;
        private readonly HttpContext _mockContext;
        private readonly RequestDelegate _next;
        private readonly JwtMiddleware _authenticationMiddleware;
        private readonly Mock<IUserService> _mockUserService;
        private readonly IOptions<AppSettings> _appOptions;
        private readonly AuthenticateResponse _authenticateExample;
        private readonly UsersController _userController;


        public JwtMiddlewareTests()
        {
            _testingSettings = new AppSettings();
            _testingSettings.Secret = "THIS IS MY VERY LONG TESTING SECRET THAT NO ONE SHOULD KNOW";
            // Arrange testing service for all the testing class
            _appOptions = Options.Create(_testingSettings);

            var mapperConfig = new MapperConfiguration(cfg => {
                cfg.AddProfile<UsersProfile>();
                cfg.AddProfile<AuthenticateProfile>();
            });
            _mockMapper = mapperConfig.CreateMapper();

            _authenticateExample = new AuthenticateResponse { 
                Id = 2,
                FirstName = "User",
                LastName = "User",
                Username = "user",
                Token = "My Token" 
            };

            var user = new User
            {
                Id = 2,
                FirstName = "User",
                LastName = "User",
                Username = "user",
                Password = "user",
                PizzaLove = 3
            };
            _mockUserService = new Mock<IUserService>();
            _mockUserService
                .Setup(x => x.AuthenticateAsync(It.Is<AuthenticateRequest>(i => i.Username == "user")))
                .ReturnsAsync(_authenticateExample);
            _mockUserService.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(user);


            _userController = new UsersController(_appOptions, _mockUserService.Object, _mockMapper);

            _mockContext = new DefaultHttpContext();
            _next = async (HttpContext hc) => await Task.CompletedTask;

            _authenticationMiddleware = new JwtMiddleware(_mockUserService.Object, _appOptions);
        }

        [Fact]
        public async Task Invoke_AttachsUserToContext_WhenTokenNotNull()
        {
            // Arrange
            var newAuthenticateRequest = new AuthenticateRequestDto
            {
                Username = "user",
                Password = "user"
            };
            var response = await _userController.AuthenticateAsync(newAuthenticateRequest);
            var result = response.Result as OkObjectResult;
            _mockContext.Request.Headers.Add("Authorization",
                _mockMapper.Map<AuthenticateResponseDto>(result.Value).Token);

            // Act
            await _authenticationMiddleware.InvokeAsync(_mockContext, _next);

            // Assert
            Assert.True(_mockContext.Items.TryGetValue("User", out var header));
            var user = header as Task<User>;
            Assert.Equal(2, user.Result.Id);
        }

        [Fact]
        public async Task Invoke_ShouldDoNothing_WhenTokenIsInvalid()
        {
            // Arrange
            _mockContext.Request.Headers.Add("Authorization", "ThisIsAnInvalidToken");

            // Act
            await _authenticationMiddleware.InvokeAsync(_mockContext, _next);

            // Assert
            Assert.False(_mockContext.Items.TryGetValue("User", out var header));
        }
    }
}