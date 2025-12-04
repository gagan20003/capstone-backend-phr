
using Xunit;
using Moq;
using FluentAssertions;
using PersonalHealthRecordManagement.Services;
using PersonalHealthRecordManagement.Repositories;
using PersonalHealthRecordManagement.Models;
using PersonalHealthRecordManagement.DTOs;

namespace PersonalHealthRecordManagement.Tests.Services
{
    public class MedicalRecordServiceTests
    {
        private readonly Mock<IMedicalRecordRepository> _repoMock;
        private readonly MedicalRecordService _service;

        public MedicalRecordServiceTests()
        {
            _repoMock = new Mock<IMedicalRecordRepository>();
            _service = new MedicalRecordService(_repoMock.Object);
        }

        [Fact]
        public async Task GetAllRecords_ShouldReturnList()
        {
            var userId = "USER1";
            var records = new List<MedicalRecords>
            {
                new MedicalRecords { RecordId = 1, UserId = userId },
                new MedicalRecords { RecordId = 2, UserId = userId }
            };

            _repoMock.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(records);

            var result = await _service.GetForUserAsync(userId);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllRecords_ShouldReturnEmpty_WhenNoRecords()
        {
            var userId = "USER1";
            _repoMock.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(new List<MedicalRecords>());

            var result = await _service.GetForUserAsync(userId);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task CreateRecord_ShouldCallRepository()
        {
            var userId = "USER1";
            var dto = new CreateUpdateMedicalRecordDto
            {
                Provider = "AIIMS",
                RecordType = "Test",
                FileUrl = "file.pdf",
                RecordDate = DateTime.UtcNow
            };

            var result = await _service.CreateForUserAsync(userId, dto);

            result.Should().NotBeNull();
            _repoMock.Verify(x => x.AddAsync(It.IsAny<MedicalRecords>()), Times.Once);
            _repoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateRecord_ShouldThrow_WhenFileUrlMissing()
        {
            var userId = "USER1";
            var dto = new CreateUpdateMedicalRecordDto
            {
                Provider = "AIIMS",
                RecordType = "Test",
                FileUrl = "",
                RecordDate = DateTime.UtcNow
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateForUserAsync(userId, dto));
        }

        [Fact]
        public async Task CreateRecord_ShouldThrow_WhenRepositoryFails()
        {
            var userId = "USER1";
            var dto = new CreateUpdateMedicalRecordDto
            {
                Provider = "AIIMS",
                RecordType = "Test",
                FileUrl = "file.pdf",
                RecordDate = DateTime.UtcNow
            };

            _repoMock.Setup(x => x.AddAsync(It.IsAny<MedicalRecords>()))
                .ThrowsAsync(new Exception("DB error"));

            await Assert.ThrowsAsync<Exception>(() => _service.CreateForUserAsync(userId, dto));
        }
    }
}
