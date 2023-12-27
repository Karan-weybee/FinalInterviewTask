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
    public class ProductRatesController : ControllerBase
    {
        private readonly PartyProductCoreContext _context;
        private readonly IMapper _mapper;

        public ProductRatesController(PartyProductCoreContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/ProductRates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductRate>>> GetProductRates()
        {
            //var ansignParty = await _context.AssignParties.ToListAsync();
            var productRates = await _context.ProductRates.Include(x => x.Product).ToListAsync();
            var ProductRate = new List<ProductRate>();
            foreach (var item in productRates)
            {
                ProductRate.Add(new ProductRate()
                {
                    Id = item.Id,
                    Rate = item.Rate,
                    DateOfRate = item.DateOfRate,
                    ProductName = item.Product.ProductName.ToString()
                });
            }

            //var assignDto = _mapper.Map<List<AssignPartyDTO>>(ansignParty);
            return ProductRate;
        }

        // GET: api/ProductRates/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductRate>> GetProductRates(int id)
        {

            if (!ProductRatesExists(id))
            {
                return NotFound("ProductRate Not Found");
            }
            var productRate = await _context.ProductRates.Include(x => x.Product).SingleOrDefaultAsync(x => x.Id == id);
            ProductRate productRates = new ProductRate()
            {
                Id = id,
                Rate = productRate.Rate,
                DateOfRate = productRate.DateOfRate,
                ProductName = productRate.Product.ProductName.ToString()
            };
            return productRates;
        }

        // PUT: api/ProductRates/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductRates(int id, ProductRateDTO productRateDTO)
        {
            var IsValidProductRate = ProductRatesExists(id);
            if (!IsValidProductRate)
            {
                return BadRequest();
            }
            productRateDTO.Id = id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Entry(_mapper.Map<ProductRates>(productRateDTO)).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok($"ProductRate Modify at id {id}");
        }

        // POST: api/ProductRates
        [HttpPost]
        public async Task<ActionResult<ProductRateDTO>> PostProductRates(ProductRateDTO productRateDTO)
        {

            try
            {
                _context.ProductRates.Add(_mapper.Map<ProductRates>(productRateDTO));
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return StatusCode(202, $"ProductRate Created Successfully");
        }

        // DELETE: api/ProductRates/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductRates>> DeleteProductRates(int id)
        {
            var productRates = await _context.ProductRates.FindAsync(id);
            if (productRates == null)
            {
                return NotFound();
            }

            _context.ProductRates.Remove(productRates);
            await _context.SaveChangesAsync();

            return StatusCode(202, $"ProductRate Deleted Successfully");
        }

        private bool ProductRatesExists(int id)
        {
            return _context.ProductRates.Any(e => e.Id == id);
        }
    }
}
