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
    public class ProductsController : ControllerBase
    {
        private readonly PartyProductCoreContext _context;
        private readonly IMapper _mapper;

        public ProductsController(PartyProductCoreContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await _context.Products.ToListAsync();

            List<ProductDTO> productList = _mapper.Map<List<ProductDTO>>(products);

            return productList;
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProducts(int id)
        {
            if (!ProductsExists(id))
            {
                return NotFound("Product Not Found");
            }
            var products = await _context.Products.FindAsync(id);

            var productDTO = _mapper.Map<ProductDTO>(products);
            return productDTO;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDTO>> PutProducts(int id, ProductDTO productDTO)
        {
            var IsValidProduct = ProductsExists(id);
            if (!IsValidProduct)
            {
                return BadRequest();
            }
            productDTO.Id = id;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (ProductNameExists(productDTO.ProductName))
            {
                return BadRequest("Modify ProductName Already Exixts...");
            }

            _context.Entry(_mapper.Map<Products>(productDTO)).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return productDTO;
        }

        // POST: api/Products
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ProductDTO>> PostProducts(ProductDTO productDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (ProductNameExists(productDTO.ProductName))
            {
                return BadRequest("Product Already Exixts...");
            }
            _context.Products.Add(_mapper.Map<Products>(productDTO));
            await _context.SaveChangesAsync();


            return StatusCode(202, $"Product Created Successfully");
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProducts(int id)
        {
            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }

            _context.Products.Remove(products);
            await _context.SaveChangesAsync();


            return StatusCode(202, $"Product Deleted Successfully");
        }

        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
        private bool ProductNameExists(string productName)
        {
            return _context.Products.Any(e => e.ProductName == productName);
        }
    }
}
