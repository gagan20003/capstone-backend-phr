
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
    public class RecordsControllerTests
    {
        private readonly Mock<IMedicalRecordService> _serviceMock;
        private readonly Mock<ILogger<RecordsController>> _loggerMock;

        public RecordsControllerTests()
        {
            _serviceMock = new Mock<IMedicalRecordService>();
            _loggerMock = new Mock<ILogger<RecordsController>>();
        }

        /// <summary>
        /// Helper to create controller with a fake logged-in user
        /// </summary>
        private RecordsController CreateControllerWithUser(string userId)
        {
            var controller = new RecordsController(_serviceMock.Object, _loggerMock.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId) }, "TestAuth"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            return controller;
        }

        // ✅ GET: api/records
        [Fact]
        public async Task GetMyRecords_ShouldReturnOk_WithList()
        {
            const string userId = "user-123";
            var controller = CreateControllerWithUser(userId); // ensures HttpContext and User are set

            var list = new List<MedicalRecords>
            {
                new MedicalRecords { RecordId = 1, UserId = userId },
                new MedicalRecords { RecordId = 2, UserId = userId }
            };

            _serviceMock.Setup(s => s.GetForUserAsync(userId)).ReturnsAsync(list);

            var actionResult = await controller.GetMyRecords();

            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returned = Assert.IsAssignableFrom<List<MedicalRecords>>(okResult.Value);
            Assert.Equal(2, returned.Count);
        }

        [Fact]
        public async Task GetMyRecords_ShouldReturnOk_WhenEmpty()
        {
            const string userId = "user-123";
            var controller = CreateControllerWithUser(userId);

            _serviceMock.Setup(s => s.GetForUserAsync(userId)).ReturnsAsync(new List<MedicalRecords>());

            var actionResult = await controller.GetMyRecords();

            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returned = Assert.IsAssignableFrom<List<MedicalRecords>>(okResult.Value);
            Assert.Empty(returned);
        }

        [Fact]
        public async Task GetMyRecords_ShouldReturnUnauthorized_WhenNoUser()
        {
            var controller = new RecordsController(_serviceMock.Object, _loggerMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() // HttpContext initialized, but User is empty
            };

            var actionResult = await controller.GetMyRecords();

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }


    }
}
