using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using RaceDayy.Controllers;
using RaceDayy.Data;
using RaceDayy.Models;
using RaceDayy.Services;
using Xunit;

namespace RaceDayy.Tests
{
    public class EventsControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private (UserManager<ApplicationUser>, IBlobStorageService) GetMockDependencies()
        {
            var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                mockUserStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            var mockBlobs = new Mock<IBlobStorageService>();

            return (mockUserManager.Object, mockBlobs.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewWithEvents()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var (mockUserManager, mockBlobs) = GetMockDependencies();

            var controller = new EventsController(context, mockUserManager, mockBlobs);

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var (mockUserManager, mockBlobs) = GetMockDependencies();
            var controller = new EventsController(context, mockUserManager, mockBlobs);

            // Act
            var result = await controller.Details(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsView()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var (mockUserManager, mockBlobs) = GetMockDependencies();

            context.Venues.Add(new Venue { VenueID = 1, VenueName = "Test Venue", Address = "Test", City = "Test", Country = "SA" });
            context.Events.Add(new Event { EventID = 1, EventName = "Comrades", EventDate = DateTime.Now, VenueID = 1, OrganizerID = 1 });
            await context.SaveChangesAsync();

            var controller = new EventsController(context, mockUserManager, mockBlobs);

            // Act
            var result = await controller.Details(1);

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}