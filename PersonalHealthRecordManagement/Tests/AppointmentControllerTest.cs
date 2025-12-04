
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PersonalHealthRecordManagement.Controllers;
using PersonalHealthRecordManagement.DTOs;
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

        private AppointmentsController CreateControllerWithUser(string userId)
        {
            var controller = new AppointmentsController(_serviceMock.Object, _loggerMock.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "TestAuth"));
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
            return controller;
        }

        // ✅ GET: api/appointments
        [Fact]
        public async Task GetAppointments_ShouldReturnOk_WithList()
        {
            const string userId = "user-123";
            var controller = CreateControllerWithUser(userId);

            var list = new List<Appointments>
            {
                new Appointments { AppointmentId = 1, UserId = userId },
                new Appointments { AppointmentId = 2, UserId = userId }
            };

            _serviceMock.Setup(s => s.GetForUserAsync(userId)).ReturnsAsync(list);

            var actionResult = await controller.GetAppointments();

            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returned = Assert.IsAssignableFrom<List<Appointments>>(okResult.Value);
            Assert.Equal(2, returned.Count);
        }

        

        // ✅ GET: api/appointments/{id}
        [Fact]
        public async Task GetAppointment_ShouldReturnOk_WhenFound()
        {
            const string userId = "user-123";
            var controller = CreateControllerWithUser(userId);

            var appointment = new Appointments { AppointmentId = 1, UserId = userId };
            _serviceMock.Setup(s => s.GetByIdForUserAsync(userId, 1)).ReturnsAsync(appointment);

            var actionResult = await controller.GetAppointment(1);

            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returned = Assert.IsType<Appointments>(okResult.Value);
            Assert.Equal(1, returned.AppointmentId);
        }

        [Fact]
        public async Task GetAppointment_ShouldReturnNotFound_WhenMissing()
        {
            const string userId = "user-123";
            var controller = CreateControllerWithUser(userId);

            _serviceMock.Setup(s => s.GetByIdForUserAsync(userId, 99)).ReturnsAsync((Appointments)null);

            var actionResult = await controller.GetAppointment(99);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Contains("Appointment not found", notFoundResult.Value.ToString());
        }

        // ✅ POST: api/appointments
        [Fact]
        public async Task CreateAppointment_ShouldReturnCreated_WhenValid()
        {
            const string userId = "user-123";
            var controller = CreateControllerWithUser(userId);

            var dto = new CreateUpdateAppointmentDto
            {
                DoctorName = "Dr House",
                Purpose = "Checkup",
                AppointmentDate = DateTime.UtcNow.AddDays(2)
            };

            var created = new Appointments { AppointmentId = 10, UserId = userId };
            _serviceMock.Setup(s => s.CreateForUserAsync(userId, dto)).ReturnsAsync(created);

            var actionResult = await controller.CreateAppointment(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returned = Assert.IsType<Appointments>(createdResult.Value);
            Assert.Equal(10, returned.AppointmentId);
        }

        [Fact]
        public async Task CreateAppointment_ShouldReturnBadRequest_WhenDateIsPast()
        {
            const string userId = "user-123";
            var controller = CreateControllerWithUser(userId);

            var dto = new CreateUpdateAppointmentDto
            {
                DoctorName = "Dr Strange",
                Purpose = "Consultation",
                AppointmentDate = DateTime.UtcNow.AddDays(-1)
            };

            var actionResult = await controller.CreateAppointment(dto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Contains("Appointment date cannot be in the past", badRequestResult.Value.ToString());
        }

        // ✅ PUT: api/appointments/{id}
        [Fact]
        public async Task UpdateAppointment_ShouldReturnOk_WhenUpdated()
        {
            const string userId = "user-123";
            var controller = CreateControllerWithUser(userId);

            var dto = new CreateUpdateAppointmentDto
            {
                DoctorName = "Dr Who",
                Purpose = "Follow-up",
                AppointmentDate = DateTime.UtcNow.AddDays(3)
            };

            var updated = new Appointments { AppointmentId = 1, UserId = userId };
            _serviceMock.Setup(s => s.UpdateForUserAsync(userId, 1, dto)).ReturnsAsync(updated);

            var actionResult = await controller.UpdateAppointment(1, dto);

            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returned = Assert.IsType<Appointments>(okResult.Value);
            Assert.Equal(1, returned.AppointmentId);
        }

        [Fact]
        public async Task UpdateAppointment_ShouldReturnNotFound_WhenMissing()
        {
            const string userId = "user-123";
            var controller = CreateControllerWithUser(userId);

            var dto = new CreateUpdateAppointmentDto
            {
                DoctorName = "Dr Who",
                Purpose = "Follow-up",
                AppointmentDate = DateTime.UtcNow.AddDays(3)
            };

            _serviceMock.Setup(s => s.UpdateForUserAsync(userId, 99, dto)).ReturnsAsync((Appointments)null);

            var actionResult = await controller.UpdateAppointment(99, dto);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Contains("Appointment not found", notFoundResult.Value.ToString());
        }

        // ✅ DELETE: api/appointments/{id}
        [Fact]
        public async Task DeleteAppointment_ShouldReturnNoContent_WhenDeleted()
        {
            const string userId = "user-123";
            var controller = CreateControllerWithUser(userId);

            _serviceMock.Setup(s => s.DeleteForUserAsync(userId, 1)).ReturnsAsync(true);

            var actionResult = await controller.DeleteAppointment(1);

            Assert.IsType<NoContentResult>(actionResult);
        }

        [Fact]
        public async Task DeleteAppointment_ShouldReturnNotFound_WhenMissing()
        {
            const string userId = "user-123";
            var controller = CreateControllerWithUser(userId);

            _serviceMock.Setup(s => s.DeleteForUserAsync(userId, 99)).ReturnsAsync(false);

            var actionResult = await controller.DeleteAppointment(99);

            Assert.IsType<NotFoundResult>(actionResult);
        }
    }
}