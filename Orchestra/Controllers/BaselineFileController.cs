using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Models;
using Orchestra.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestra.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaselineFileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BaselineFileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/BaselineFile/upload/{baselineId}
        [HttpPost("upload/{baselineId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadBaselineFile([FromRoute] int baselineId, [FromForm] UploadBaselineFileDto dto)
        {
            var baseline = await _context.BpmnProcess.FindAsync(baselineId);
            if (baseline == null)
                return NotFound("Baseline não encontrada.");

            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("Arquivo inválido.");

            using var ms = new MemoryStream();
            await dto.File.CopyToAsync(ms);
            var fileBytes = ms.ToArray();

            var baselineFile = new BaselineFile
            {
                Id = Guid.NewGuid(),
                BaselineId = baselineId,
                FileName = dto.File.FileName,
                ContentType = dto.File.ContentType,
                Content = fileBytes,
                UploadedAt = DateTime.UtcNow,
                UploadedByUserId = dto.UploadedByUserId,
                XmlTaskId = dto.XmlDataObjectId
            };

            _context.BaselineFiles.Add(baselineFile);
            await _context.SaveChangesAsync();
            return Ok(new { baselineFile.Id, baselineFile.FileName, baselineFile.UploadedAt });
        }

        // GET: api/BaselineFile/by-baseline/{baselineId}
        [HttpGet("by-baseline/{baselineId}")]
        public async Task<IActionResult> GetFilesByBaselineId([FromRoute] int baselineId)
        {
            var files = await _context.BaselineFiles
                .Include(f => f.UploadedBy)
                .Where(f => f.BaselineId == baselineId)
                .Select(f => new {
                    f.Id,
                    f.FileName,
                    f.ContentType,
                    f.UploadedAt,
                    UploadedByName = f.UploadedBy != null ? f.UploadedBy.FullName : null,
                    f.XmlTaskId
                })
                .ToListAsync();

            if (files == null || files.Count == 0)
                return NotFound("Nenhum documento encontrado para esta baseline.");

            return Ok(files);
        }

        // GET: api/BaselineFile/content/{id}
        [HttpGet("content/{id}")]
        public async Task<IActionResult> GetFileContent([FromRoute] Guid id)
        {
            var file = await _context.BaselineFiles.FindAsync(id);
            if (file == null)
                return NotFound("Arquivo não encontrado.");

            return File(file.Content, file.ContentType, file.FileName);
        }
    }
}
