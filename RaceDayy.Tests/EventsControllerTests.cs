using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayy.Controllers;
using RaceDayy.Data;
using RaceDayy.Models;
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

        [Fact]
        public async Task GetAll_ReturnsOkWithEmptyList()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new EventsController(context);

            // Act
            var result = await controller.GetAll();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var controller = new EventsController(context);

            // Act
            var result = await controller.GetById(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetById_WithValidId_ReturnsOk()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Venues.Add(new Venue { VenueID = 1, VenueName = "Test Venue", Address = "Test", City = "Test", Country = "SA" });
            context.Events.Add(new Event { EventID = 1, EventName = "Comrades", EventDate = DateTime.Now, VenueID = 1, OrganizerID = 1 });
            await context.SaveChangesAsync();

            var controller = new EventsController(context);

            // Act
            var result = await controller.GetById(1);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}