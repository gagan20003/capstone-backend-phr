using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PersonalHealthRecordManagement.DTOs;
using PersonalHealthRecordManagement.Models;
using PersonalHealthRecordManagement.Repositories;

namespace PersonalHealthRecordManagement.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;

        public MedicalRecordService(IMedicalRecordRepository medicalRecordRepository)
        {
            _medicalRecordRepository = medicalRecordRepository;
        }

        public async Task<List<MedicalRecords>> GetForUserAsync(string userId)
        {
            return await _medicalRecordRepository.GetByUserIdAsync(userId);
        }

        public async Task<MedicalRecords?> GetByIdForUserAsync(string userId, int recordId)
        {
            var record = await _medicalRecordRepository.GetByIdAsync(recordId);
            if (record == null || record.UserId != userId)
            {
                return null;
            }

            return record;
        }

        public async Task<MedicalRecords> CreateForUserAsync(string userId, CreateUpdateMedicalRecordDto dto)
        {
            var record = new MedicalRecords
            {
                UserId = userId,
                RecordType = dto.RecordType,
                Provider = dto.Provider,
                Description = dto.Description,
                RecordDate = DateOnly.FromDateTime(dto.RecordDate),
                FileUrl = dto.FileUrl,
                CreatedAt = DateTime.UtcNow
            };

            await _medicalRecordRepository.AddAsync(record);
            await _medicalRecordRepository.SaveChangesAsync();

            return record;
        }

        public async Task<MedicalRecords?> UpdateForUserAsync(string userId, int recordId, CreateUpdateMedicalRecordDto dto)
        {
            var record = await _medicalRecordRepository.GetByIdAsync(recordId);
            if (record == null || record.UserId != userId)
            {
                return null;
            }

            record.RecordType = dto.RecordType;
            record.Provider = dto.Provider;
            record.Description = dto.Description;
            record.RecordDate = DateOnly.FromDateTime(dto.RecordDate);
            record.FileUrl = dto.FileUrl;

            await _medicalRecordRepository.UpdateAsync(record);
            await _medicalRecordRepository.SaveChangesAsync();

            return record;
        }

        public async Task<bool> DeleteForUserAsync(string userId, int recordId)
        {
            var record = await _medicalRecordRepository.GetByIdAsync(recordId);
            if (record == null || record.UserId != userId)
            {
                return false;
            }

            await _medicalRecordRepository.DeleteAsync(record);
            await _medicalRecordRepository.SaveChangesAsync();
            return true;
        }
    }
}