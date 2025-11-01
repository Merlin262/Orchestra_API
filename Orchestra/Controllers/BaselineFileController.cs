using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Models;
using Orchestra.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Orchestra.Application.BaselineFile.Query;
using Orchestra.Application.BaselineFile.Query.GetBaselineFileContent;
using Orchestra.Application.BaselineFile.Query.GetBaselineFilesByBaselineId;
using Microsoft.AspNetCore.Authorization;

namespace Orchestra.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BaselineFileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;

        public BaselineFileController(ApplicationDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
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
            var files = await _mediator.Send(
                new GetBaselineFilesByBaselineIdQuery(baselineId)
            );
            if (files == null || files.Count == 0)
                return NoContent();
            return Ok(files);
        }

        // GET: api/BaselineFile/content/{id}
        [HttpGet("content/{id}")]
        public async Task<IActionResult> GetFileContent([FromRoute] Guid id)
        {
            var result = await _mediator.Send(new GetBaselineFileContentQuery(id));
            if (result == null)
                return NoContent();

            return File(result.Content, result.ContentType, result.FileName);
        }
    }
}
