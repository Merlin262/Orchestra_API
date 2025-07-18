﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestra.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<object>> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var totalUsers = await _context.Users.CountAsync();
            var users = await _context.Users
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                TotalItems = totalUsers,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = users
            };

            return Ok(result);
        }

        // GET: api/Users/Roles
        [HttpGet("Roles")]
        public async Task<ActionResult<IEnumerable<object>>> GetRolesCount()
        {
            var users = await _context.Users.ToListAsync();

            var rolesCount = users
                .SelectMany(u => u.Roles ?? new List<string>())
                .GroupBy(role => role)
                .Select(g => new
                {
                    Role = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(r => r.Count)
                .ToList();

            return Ok(rolesCount);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //[HttpGet("ByGroups")]
        //public async Task<ActionResult<IEnumerable<User>>> GetUsersByGroupNames([FromQuery] List<string> groupNames)
        //{
        //    if (groupNames == null || !groupNames.Any())
        //        return BadRequest("A lista de nomes de grupos não pode ser vazia.");

        //    var users = await _context.Users
        //        .Where(u => u.UserGroup != null && groupNames.Contains(u.UserGroup.Name))
        //        .ToListAsync();

        //    return Ok(users);
        //}
    }
}
