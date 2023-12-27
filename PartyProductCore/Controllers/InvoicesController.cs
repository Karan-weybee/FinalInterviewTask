using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PartyProductCore.Entities;
using PartyProductCore.Models;

namespace PartyProductCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
        private SqlConnection _connection = new SqlConnection("Server=DESKTOP-9IJS7NM;Database=PartyProductCore;Trusted_Connection=True;");
        private readonly PartyProductCoreContext _context;
        private readonly IMapper _mapper;
        private ILogger<InvoicesController> _logger;

        public InvoicesController(PartyProductCoreContext context, IMapper mapper, ILogger<InvoicesController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/Invoices
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<invoiceData>>> GetInvoices()
        {
            //var invoices = await _context.InvoiceData.FromSqlRaw("GetAllInvoices").ToListAsync();

            List<invoiceData> data = new List<invoiceData>();

            using (SqlCommand command = new SqlCommand("SELECT Party_id as id,sum((Rate_Of_Product * Quantity)) as total,p.PartyName as partyName FROM(SELECT *,DENSE_RANK() OVER(ORDER BY Party_id) as rank FROM invoices) AS ranked_data inner join Parties p on p.id = ranked_data.Party_id group by rank, party_id, p.PartyName", _connection))
            {
                _connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //data.Add(reader.GetString(0));
                        data.Add(new invoiceData { id = reader.GetInt32(0), Total = reader.GetDecimal(1), partyName = reader.GetString(2) });
                    }
                }
                _connection.Close();
            }

            return data;
            //return invoices;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<invoiceProducts>>> GetInvoices(int id)
        {
            //var invoices = await _context.invoiceProducts.FromSqlRaw("GetInvoicesForProduct @partyid", new SqlParameter("@partyid", id)).ToListAsync();

            List<invoiceProducts> data = new List<invoiceProducts>();

            using (SqlCommand command = new SqlCommand("select Product_id as id, Rate_Of_Product as RateOfProduct, quantity,sum((Rate_Of_Product * Quantity)) as total,pr.ProductName,i.DateOfInvoice from invoices i inner join Products pr on pr.id = i.Product_id where i.party_id = @partyId group by i.Product_id, pr.ProductName, Rate_Of_Product, quantity, i.DateOfInvoice", _connection))
            {
                command.Parameters.AddWithValue("@partyId", id);
                _connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //data.Add(reader.GetString(0));
                        data.Add(new invoiceProducts { id = reader.GetInt32(0), RateOfProduct = reader.GetDecimal(1), Quantity = reader.GetInt32(2), Total = reader.GetDecimal(3), ProductName = reader.GetString(4), DateOfInvoice = reader.GetDateTime(5) });
                    }
                }
                _connection.Close();
            }

            return data;
            //return invoices;
        }

        [HttpGet("Search")]
        public async Task<ActionResult<List<Invoices>>> GetInvoices([FromQuery] int partyId, string productName, DateTime dateTime)
        {

            // return Ok(partyId + ":-" + productName + ":-" + dateTime);
            string formattedDateTime = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            List<Invoices> invoices = new List<Invoices>();
            formattedDateTime = formattedDateTime == "0001-01-01 00:00:00" ? "" : formattedDateTime;
            productName = productName == null ? "" : productName;

            invoices = await _context.Invoices.FromSqlRaw(
        "EXEC GetInvoiceFromProductNameAndDate @partyId, @productName, @DateOfInvoice",
        new SqlParameter("@partyId", partyId),
        new SqlParameter("@productName", productName),
        new SqlParameter("@DateOfInvoice", formattedDateTime)
    ).ToListAsync();


            return Ok(invoices);

        }

        // POST: api/Invoices
        [HttpPost]
        public async Task<ActionResult<InvoiceDTO>> PostInvoices(InvoiceDTO invoicesDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if (IsProductInInvoice(invoicesDTO.ProductId, invoicesDTO.PartyId))
                {
                    await _context.Database.ExecuteSqlRawAsync("EXEC editInvoiceProduct @Rate_Of_Product,@Quantity,@Party_id, @Product_id,@Date",
                         new SqlParameter("@Rate_Of_Product", invoicesDTO.RateOfProduct),
                        new SqlParameter("@Quantity", invoicesDTO.Quantity),
                        new SqlParameter("@Party_id", invoicesDTO.PartyId),
                        new SqlParameter("@Product_id", invoicesDTO.ProductId),
                        new SqlParameter("@Date", DateTime.Today.Date));
                    return StatusCode(201, $"invoice Created");
                }
                else
                {
                    await _context.Database.ExecuteSqlRawAsync("EXEC InsertInvoice @Rate_Of_Product, @Quantity, @Party_id, @Product_id,@Date",
                        new SqlParameter("@Rate_Of_Product", invoicesDTO.RateOfProduct),
                        new SqlParameter("@Quantity", invoicesDTO.Quantity),
                        new SqlParameter("@Party_id", invoicesDTO.PartyId),
                        new SqlParameter("@Product_id", invoicesDTO.ProductId),
                        new SqlParameter("@Date", DateTime.Today.Date));
                    return StatusCode(201, $"invoice Created Successfully");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //update invoice for perticular party 
        [HttpPut("{id}")]
        public async Task<ActionResult<invoiceProducts>> PutInvoices(int id, invoiceProducts invoiceProduct)
        {

            try
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC UpdateInvoice @partyid,@Rate_Of_Product, @Quantity,@Product_id,@Date",
                     new SqlParameter("@partyid", id),
                     new SqlParameter("@Rate_Of_Product", invoiceProduct.RateOfProduct),
                     new SqlParameter("@Quantity", invoiceProduct.Quantity),
                     new SqlParameter("@Product_id", invoiceProduct.id),
                     new SqlParameter("@Date", DateTime.Today.Date));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return StatusCode(201, $"invoice updated Successfully");
        }

        // DELETE: api/Invoices
        [HttpDelete]
        public async Task<ActionResult<Invoices>> DeleteInvoices([FromQuery] int partyId, int productId)
        {
            await _context.Database.ExecuteSqlRawAsync($"DeleteInvoice @partyId,@productId",
                 new SqlParameter("@partyid", partyId),
                  new SqlParameter("@productId", productId)
                );

            return StatusCode(202, $"invoice Deleted Successfully");
        }

        private bool InvoicesExists(int id)
        {
            return _context.Invoices.Any(e => e.Id == id);
        }
        private bool IsProductInInvoice(int productId, int partyId)
        {
            return _context.Invoices.Any(e => e.ProductId == productId && e.PartyId == partyId);
        }
        private bool ProductNameExists(string productName)
        {
            return _context.Products.Any(e => e.ProductName.Contains(productName));
        }
    }
}
