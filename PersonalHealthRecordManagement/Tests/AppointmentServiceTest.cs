using Xunit;
using Moq;
using FluentAssertions;
using PersonalHealthRecordManagement.Services;
using PersonalHealthRecordManagement.Repositories;
using PersonalHealthRecordManagement.DTOs;
using PersonalHealthRecordManagement.Models;

namespace PersonalHealthRecordManagement.Tests.Services
{
    public class AppointmentServiceTests
    {
        private readonly Mock<IAppointmentRepository> _repoMock;
        private readonly AppointmentService _service;

        public AppointmentServiceTests()
        {
            _repoMock = new Mock<IAppointmentRepository>();
            _service = new AppointmentService(_repoMock.Object);
        }

        [Fact]
        public async Task CreateAppointment_ShouldCallRepository()
        {
            var dto = new CreateUpdateAppointmentDto
            {
                DoctorName = "Dr House",
                Purpose = "Checkup",
                AppointmentDate = DateTime.UtcNow.AddDays(3)
            };

            var result = await _service.CreateForUserAsync("USER1", dto);

            result.Should().NotBeNull();
            _repoMock.Verify(x => x.AddAsync(It.IsAny<Appointments>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAppointment_ShouldReturnTrue_WhenExists()
        {
            _repoMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Appointments { AppointmentId = 1, UserId = "USER1" });

            var result = await _service.DeleteForUserAsync("USER1", 1);

            result.Should().BeTrue();
        }
    }
}