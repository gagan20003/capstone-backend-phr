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

        // Helper: controller with fake logged-in user
        private RecordsController CreateControllerWithUser(string userId)
        {
            var controller = new RecordsController(medicalRecordService: _serviceMock.Object,
            logger: _loggerMock.Object);

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
        public async Task GetMyRecords_ShouldReturnOk_WithList()
        {
            // ARRANGE
            const string userId = "user-123";
            var controller = CreateControllerWithUser(userId);

            var list = new List<MedicalRecords>
{
new MedicalRecords { RecordId = 1, UserId = userId },
new MedicalRecords { RecordId = 2, UserId = userId }
};

            _serviceMock
            .Setup(s => s.GetForUserAsync(userId))
            .ReturnsAsync(list);

            // ACT
            var actionResult = await controller.GetMyRecords();

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returned = Assert.IsAssignableFrom<List<MedicalRecords>>(okResult.Value);
            Assert.Equal(2, returned.Count);
        }
    }
}