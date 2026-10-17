using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RaceDayy.Controllers;
using RaceDayy.Data;
using Xunit;

namespace RaceDayy.Tests
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Register_WithMissingEmail_ReturnsBadRequest()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object, null!, null!, null!, null!, null!, null!, null!, null!);
            var mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                mockUserManager.Object, new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object, null!, null!, null!, null!);

            var controller = new AuthController(mockUserManager.Object, mockSignInManager.Object);
            var dto = new RegisterDto { Email = "", Password = "Test@123", Role = "Participant" };

            // Act
            var result = await controller.Register(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_WithInvalidRole_ReturnsBadRequest()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object, null!, null!, null!, null!, null!, null!, null!, null!);
            var mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                mockUserManager.Object, new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object, null!, null!, null!, null!);

            var controller = new AuthController(mockUserManager.Object, mockSignInManager.Object);
            var dto = new RegisterDto { Email = "test@test.com", Password = "Test@123", Role = "SuperHero" };

            // Act
            var result = await controller.Register(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object, null!, null!, null!, null!, null!, null!, null!, null!);

            mockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);

            var mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                mockUserManager.Object, new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object, null!, null!, null!, null!);

            var controller = new AuthController(mockUserManager.Object, mockSignInManager.Object);
            var dto = new LoginDto { Email = "wrong@test.com", Password = "wrong" };

            // Act
            var result = await controller.Login(dto);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}