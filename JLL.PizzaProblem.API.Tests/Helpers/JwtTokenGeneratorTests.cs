using Xunit;
using JLL.PizzaProblem.API.Middleware;
using System;
using System.Collections.Generic;
using System.Text;

namespace JLL.PizzaProblem.API.Middleware.Tests
{
    public class JwtTokenGeneratorTests
    {
        [Fact]
        public void GenerateJwtToken_ShouldCreate_ValidToken()
        {
            var token = JwtTokenGenerator.GenerateJwtToken(1, "THIS IS MY VERY LONG TESTING SECRET THAT NO ONE SHOULD KNOW");
            Assert.IsType<string>(token);
            Assert.NotNull(token);
        }
    }
}