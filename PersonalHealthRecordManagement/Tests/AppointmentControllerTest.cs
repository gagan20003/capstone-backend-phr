using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PersonalHealthRecordManagement.Controllers;
using PersonalHealthRecordManagement.Models;
using PersonalHealthRecordManagement.Services;
using Xunit;

namespace PersonalHealthRecordManagement.Tests.Controllers
{
    public class AppointmentsControllerTests
    {
        private readonly Mock<IAppointmentService> _serviceMock;
        private readonly Mock<ILogger<AppointmentsController>> _loggerMock;

        public AppointmentsControllerTests()
        {
            _serviceMock = new Mock<IAppointmentService>();
            _loggerMock = new Mock<ILogger<AppointmentsController>>();
        }

        // Helper: create controller with a fake logged-in user
        private AppointmentsController CreateControllerWithUser(string userId)
        {
            var controller = new AppointmentsController(_serviceMock.Object, _loggerMock.Object);

            var user = new ClaimsPrincipal(
            new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, userId) },
            "TestAuth"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return controller;
        }

        [Fact]
        public async Task GetAppointments_ShouldReturnOk_WithList()
        {
            // ARRANGE
            const string userId = "user-123";
            var controller = CreateControllerWithUser(userId);

            var list = new List<Appointments>
{
new Appointments { AppointmentId = 1, UserId = userId },
new Appointments { AppointmentId = 2, UserId = userId }
};

            _serviceMock
            .Setup(s => s.GetForUserAsync(userId))
            .ReturnsAsync(list);

            // ACT
            var actionResult = await controller.GetAppointments();

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returned = Assert.IsAssignableFrom<List<Appointments>>(okResult.Value);
            Assert.Equal(2, returned.Count);
        }
    }
}