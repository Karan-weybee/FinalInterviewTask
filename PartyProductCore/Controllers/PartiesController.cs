using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PartyProductCore.Entities;
using PartyProductCore.Models;

namespace PartyProductCore.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    [Authorize]
    public class PartiesController : ControllerBase
    {
        private readonly PartyProductCoreContext _context;
        private readonly IMapper _mapper;

        public PartiesController(PartyProductCoreContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Parties
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PartyDTO>>> GetParties()
        {
            var parties = await _context.Parties.FromSqlRaw("GetAllParties").ToListAsync();

            var partyList = _mapper.Map<List<PartyDTO>>(parties);

            return partyList;
        }

        // GET: api/Parties/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PartyDTO>> GetParties(int id)
        {

            if (!PartiesExists(id))
            {
                return NotFound("Party Not Found");
            }
            var parties = await _context.Parties.FromSqlRaw($"GetParty {id}").ToListAsync();

            var partyDTO = _mapper.Map<List<PartyDTO>>(parties);
            return partyDTO.First();
        }

        // PUT: api/Parties/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParties(int id, PartyDTO partyDTO)
        {
            var IsValidParty = PartiesExists(id);
            if (!IsValidParty)
            {
                return BadRequest("Party Not Found");
            }
            partyDTO.Id = id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (PartiesNameExists(partyDTO.PartyName))
            {
                return BadRequest("Modifyed Party already Exist");
            }

            _context.Entry(_mapper.Map<Parties>(partyDTO)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PartiesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetParties", new { id = partyDTO.Id }, partyDTO);
        }

        // POST: api/Parties
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<PartyDTO>> PostParties(PartyDTO partyDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (PartiesNameExists(partyDTO.PartyName))
            {
                return BadRequest("Party Already Exists...");
            }
            await _context.Database.ExecuteSqlRawAsync("EXEC InsertParty @PartyName", new SqlParameter("@PartyName", partyDTO.PartyName));

            return CreatedAtAction("GetParties", new { id = partyDTO.Id }, partyDTO);

        }

        // DELETE: api/Parties/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PartyDTO>> DeleteParties(int id)
        {

            try
            {
                var party = await _context.Parties.FindAsync(id);

                if (party == null)
                {
                    return NotFound();
                }

                // Call the stored procedure to delete the partyDTO
                await _context.Database.ExecuteSqlRawAsync($"DeleteParty {id}");

                return StatusCode(202, $"Party Deleted Successfully");
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately for your application
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool PartiesExists(int id)
        {
            return _context.Parties.Any(e => e.Id == id);
        }
        private bool PartiesNameExists(string partyName)
        {
            return _context.Parties.Any(e => e.PartyName == partyName);
        }
    }
}
