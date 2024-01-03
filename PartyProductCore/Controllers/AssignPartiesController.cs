using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartyProductCore.Entities;
using PartyProductCore.Models;

namespace PartyProductCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    [Authorize]
    public class AssignPartiesController : ControllerBase
    {
        private readonly PartyProductCoreContext _context;
        private readonly IMapper _mapper;

        public AssignPartiesController(PartyProductCoreContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/AssignParties
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssignParty>>> GetAssignParties()
        {
            //var ansignParty = await _context.AssignParties.ToListAsync();
            var ansignParty = await _context.AssignParties.Include(x => x.Party).Include(x => x.Product).ToListAsync();
            var list = new List<AssignParty>();
            foreach (var item in ansignParty)
            {
                list.Add(new AssignParty()
                {
                    id = item.Id,
                    partyName = item.Party.PartyName.ToString(),
                    productName = item.Product.ProductName.ToString()
                });
            }

            //var assignDto = _mapper.Map<List<AssignPartyDTO>>(ansignParty);
            return list;
        }

        // GET: api/AssignParties/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AssignParty>> GetAssignParties(int id)
        {
            if (!AssignPartiesExists(id))
            {
                return NotFound("AssignParty Not Found");
            }
            var ansignParty = await _context.AssignParties.Include(x => x.Party).Include(x => x.Product).SingleOrDefaultAsync(x => x.Id == id);


            AssignParty assign = new AssignParty()
            {
                id = id,
                partyName = ansignParty.Party.PartyName.ToString(),
                productName = ansignParty.Product.ProductName.ToString()
            };
            return assign;
        }

        // PUT: api/AssignParties/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAssignParties(int id, AssignPartyDTO assignPartyDTO)
        {

            var IsValidAssign = AssignPartiesExists(id);
            if (!IsValidAssign)
            {
                return BadRequest();
            }
            assignPartyDTO.Id = id;
            try
            {
                _context.Entry(_mapper.Map<AssignParties>(assignPartyDTO)).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("Assign Modify");
        }

        // POST: api/AssignParties
        [HttpPost]
        public async Task<ActionResult<AssignPartyDTO>> PostAssignParties(AssignPartyDTO assignPartyDTO)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}
            if (AssignExists(assignPartyDTO.PartyId, assignPartyDTO.ProductId))
            {
                return BadRequest("Party Is already Assign with same Product");
            }

            try
            {

                _context.AssignParties.Add(_mapper.Map<AssignParties>(assignPartyDTO));
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return StatusCode(202, $"Assign Created Successfully");
        }

        // DELETE: api/AssignParties/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<AssignParties>> DeleteAssignParties(int id)
        {
            var assignParties = await _context.AssignParties.FindAsync(id);
            if (assignParties == null)
            {
                return NotFound();
            }

            _context.AssignParties.Remove(assignParties);
            await _context.SaveChangesAsync();

            return StatusCode(202, $"Assign Deleted Successfully");
        }

        private bool AssignPartiesExists(int id)
        {
            return _context.AssignParties.Any(e => e.Id == id);
        }
        private bool AssignExists(int partyId, int productId)
        {
            return _context.AssignParties.Any(e => e.PartyId == partyId && e.ProductId == productId);
        }

    }
}
