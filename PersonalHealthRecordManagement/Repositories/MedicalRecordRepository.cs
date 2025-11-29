using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalHealthRecordManagement.Data;
using PersonalHealthRecordManagement.Models;

namespace PersonalHealthRecordManagement.Repositories
{
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly AppDbContext _context;

        public MedicalRecordRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MedicalRecords>> GetByUserIdAsync(string userId)
        {
            return await _context.MedicalRecords
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task<MedicalRecords?> GetByIdAsync(int recordId)
        {
            return await _context.MedicalRecords
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.RecordId == recordId);
        }

        public async Task AddAsync(MedicalRecords record)
        {
            await _context.MedicalRecords.AddAsync(record);
        }

        public Task UpdateAsync(MedicalRecords record)
        {
            _context.MedicalRecords.Update(record);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(MedicalRecords record)
        {
            _context.MedicalRecords.Remove(record);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}