using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestra.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskFileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaskFileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TaskFile/by-task/{taskId}
        [HttpGet("by-task/{taskId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetFilesByTaskId([FromRoute] Guid taskId)
        {
            var files = await _context.TaskFiles
                .Include(tf => tf.UploadedBy)
                .Where(tf => tf.TaskId == taskId)
                .ToListAsync();

            if (files == null || files.Count == 0)
                return NotFound();

            var result = files.Select(tf => new {
                tf.Id,
                tf.TaskId,
                tf.FileName,
                tf.ContentType,
                tf.Content,
                tf.UploadedAt,
                tf.UploadedByUserId,
                FullName = tf.UploadedBy != null ? tf.UploadedBy.FullName : null
            });

            return Ok(result);
        }
    }
}
