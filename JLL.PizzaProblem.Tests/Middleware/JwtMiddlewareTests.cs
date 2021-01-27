using Xunit;
using JLL.PizzaProblem.API.Middleware;
using System;
using System.Collections.Generic;
using System.Text;
using JLL.PizzaProblem.API.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.AspNetCore.Http.Features;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using JLL.PizzaProblem.API.Services;
using AutoMapper;
using JLL.PizzaProblem.API.Profiles;
using JLL.PizzaProblem.API.Data;
using Microsoft.EntityFrameworkCore;

namespace JLL.PizzaProblem.API.Middleware.Tests
{
    public class JwtMiddlewareTests
    {
        private readonly AppSettings _testingSettings;
        private readonly IOptions<AppSettings> _testingOptions;
        private readonly IMapper _mockMapper;
        private readonly IUserService _testingService;
        private readonly PizzaProblemContext _context;
        private readonly HttpContext _mockContext;
        private readonly RequestDelegate _next;
        private readonly JwtMiddleware _authenticationMiddleware;


        public JwtMiddlewareTests()
        {
            _testingSettings = new AppSettings();
            _testingSettings.Secret = "THIS IS MY VERY LONG TESTING SECRET THAT NO ONE SHOULD KNOW";
            // Arrange testing service for all the testing class
            _testingOptions = Options.Create(_testingSettings);

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new UsersProfile()));
            _mockMapper = mapperConfig.CreateMapper();

            _context = new PizzaProblemContext(
                new DbContextOptionsBuilder<PizzaProblemContext>()
                            .UseInMemoryDatabase(databaseName: "MiddlewareTests")
                            .Options);
            _context.Database.EnsureCreated();

            _testingService = new UserService(_testingOptions, _mockMapper, _context);

            _mockContext = new DefaultHttpContext();
            _next = async (HttpContext hc) => await Task.CompletedTask;

            _authenticationMiddleware = new JwtMiddleware(_testingService, _testingOptions);
        }

        [Fact]
        public async Task Invoke_AttachsUserToContext_WhenTokenNotNull()
        {
            // Arrange
            var newAuthenticateRequest = new AuthenticateRequest
            {
                Username = "user",
                Password = "user"
            };
            var response = _testingService.Authenticate(newAuthenticateRequest);
            _mockContext.Request.Headers.Add("Authorization", response.Token);

            // Act
            await _authenticationMiddleware.InvokeAsync(_mockContext, _next);

            // Assert
            Assert.True(_mockContext.Items.TryGetValue("User", out var header));
            var user = header as User;
            Assert.Equal(2, user.Id);
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