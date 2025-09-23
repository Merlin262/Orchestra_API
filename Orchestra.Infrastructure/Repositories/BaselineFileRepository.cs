using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Orchestra.Domain.Repositories;
using Orchestra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Orchestra.Models;
using Orchestra.Application.BaselineFile.Query.GetBaselineFileContent;
using Orchestra.Application.BaselineFile.Query.GetBaselineFilesByBaselineId;
using System.Linq;

namespace Orchestra.Infrastructure.Repositories
{
    public class BaselineFileRepository : IBaselineFileRepository
    {
        private readonly ApplicationDbContext _context;
        public BaselineFileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BaselineFileContentResult> GetFileContentAsync(Guid id)
        {
            var file = await _context.BaselineFiles.FindAsync(id);
            if (file == null)
                return null;

            return new BaselineFileContentResult
            {
                Content = file.Content,
                ContentType = file.ContentType,
                FileName = file.FileName
            };
        }

        public async Task<List<GetBaselineFilesByBaselineIdHandlerResult>> GetFilesByBaselineIdAsync(int baselineId)
        {
            return await _context.BaselineFiles
                .Include(f => f.UploadedBy)
                .Where(f => f.BaselineId == baselineId)
                .Select(f => new GetBaselineFilesByBaselineIdHandlerResult
                {
                    Id = f.Id,
                    FileName = f.FileName,
                    ContentType = f.ContentType,
                    UploadedAt = f.UploadedAt,
                    UploadedByName = f.UploadedBy != null ? f.UploadedBy.FullName : null,
                    XmlTaskId = f.XmlTaskId
                })
                .ToListAsync();
        }

        public async Task<BaselineFile> GetBaselineFileAsync(Guid id)
        {
            return await _context.BaselineFiles
                .Include(f => f.UploadedBy)
                .Include(f => f.Baseline)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<List<BaselineFile>> GetBaselineFilesByBaselineIdAsync(int baselineId)
        {
            return await _context.BaselineFiles
                .Include(f => f.UploadedBy)
                .Include(f => f.Baseline)
                .Where(f => f.BaselineId == baselineId)
                .ToListAsync();
        }
    }
}
