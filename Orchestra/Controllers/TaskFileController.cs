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
        public async Task<ActionResult<IEnumerable<TaskFile>>> GetFilesByTaskId([FromRoute] Guid taskId)
        {
            var files = await _context.TaskFiles
                .Where(tf => tf.TaskId == taskId)
                .ToListAsync();

            if (files == null || files.Count == 0)
                return NotFound();

            return Ok(files);
        }
    }
}
