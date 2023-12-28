using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PartyProductCore.Entities;
using PartyProductCore.Models;

namespace PartyProductCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class UsersController : ControllerBase
    {
        private readonly PartyProductCoreContext _context;

        public UsersController(PartyProductCoreContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Users>> GetUsers(int id)
        {
            var users = await _context.Users.FindAsync(id);

            if (users == null)
            {
                return NotFound();
            }

            return users;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Create(Users users)
        {
            try
            {
                var userAvailable = _context.Users.Where(x => x.Email == users.Email).FirstOrDefault();
                if (userAvailable != null)
                {
                    return BadRequest("user is available Please login");
                }

                _context.Users.Add(users);
                await _context.SaveChangesAsync();

                return Ok(new Token { tokenName = new JwtService().GenerateToken(users.Email) });
            }
            catch (Exception ex)
            {
                return BadRequest("give Proper data");
            }
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult> Login(Users users)
        {

            var userAvailable = _context.Users.Where(x => x.Email == users.Email && x.Password == users.Password).FirstOrDefault();
            if (userAvailable != null)
            {
                return Ok(new Token { tokenName = new JwtService().GenerateToken(userAvailable.Email) });
            }
            else
            {
                return BadRequest("failure");
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Users>> DeleteUsers(int id)
        {
            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }

            _context.Users.Remove(users);
            await _context.SaveChangesAsync();

            return users;
        }

        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
