using Xunit;
using JLL.PizzaProblem.API.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using JLL.PizzaProblem.API.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using JLL.PizzaProblem.API.Services;
using AutoMapper;
using JLL.PizzaProblem.API.Profiles;
using Microsoft.AspNetCore.Mvc;
using JLL.PizzaProblem.API.Data;
using Microsoft.EntityFrameworkCore;

namespace JLL.PizzaProblem.API.Filters.Tests
{
    public class AuthorizeAttributeTests
    {
        private readonly AppSettings _testingSettings;
        private readonly IOptions<AppSettings> _testingOptions;
        private readonly HttpContext _mockContext;
        private readonly IUserService _testingService;
        private readonly IMapper _mockMapper;
        private readonly ActionContext _actionContext;
        private readonly AuthorizationFilterContext _authorizationFilterContext;
        private readonly AuthorizeAttribute _authorizeAttribute;
        private readonly PizzaProblemContext _context;

        public AuthorizeAttributeTests()
        {
            _testingSettings = new AppSettings();
            _testingSettings.Secret = "THIS IS MY VERY LONG TESTING SECRET THAT NO ONE SHOULD KNOW";
            // Arrange testing service for all the testing class
            _testingOptions = Options.Create(_testingSettings);

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new UsersProfile()));
            _mockMapper = mapperConfig.CreateMapper();

            _context = new PizzaProblemContext(
                new DbContextOptionsBuilder<PizzaProblemContext>()
                            .UseInMemoryDatabase(databaseName: "AuthorizeAttributeTests")
                            .Options);
            _context.Database.EnsureCreated();

            _testingService = new UserService(_testingOptions, _mockMapper, _context);

            _mockContext = new DefaultHttpContext();

            _actionContext =
                new ActionContext(_mockContext,
                  new Microsoft.AspNetCore.Routing.RouteData(),
                  new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

            _authorizationFilterContext = new AuthorizationFilterContext(_actionContext,
                    new List<IFilterMetadata>());

            _authorizeAttribute = new AuthorizeAttribute();
        }

        [Fact]
        public void OnAuthorization_ShouldReturn_NotNullForExistentUser()
        {
            // Arrange
            var newAuthenticateRequest = new AuthenticateRequest
            {
                Username = "user",
                Password = "user"
            };

            // Act
            _testingService.Authenticate(newAuthenticateRequest);
            _authorizeAttribute.OnAuthorization(_authorizationFilterContext);

            // Assert
            Assert.NotNull(_authorizationFilterContext.Result);
        }

        [Fact]
        public void OnAuthorization_ShouldReturn_UnauthorizedForNonExistentUser()
        {
            // Arrange
            var newAuthenticateRequest = new AuthenticateRequest
            {
                Username = "NonExistentUser",
                Password = "user"
            };

            // Act
            _testingService.Authenticate(newAuthenticateRequest);
            _authorizeAttribute.OnAuthorization(_authorizationFilterContext);

            // Assert
            var result = _authorizationFilterContext.Result as JsonResult;
            Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
        }
    }
}