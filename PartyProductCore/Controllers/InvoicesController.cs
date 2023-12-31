﻿using System;
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
using Microsoft.Extensions.Configuration;
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
        private SqlConnection _connection;
        private string _connectionstring;
        private readonly PartyProductCoreContext _context;
        private readonly IMapper _mapper;
        private ILogger<InvoicesController> _logger;

        public InvoicesController(PartyProductCoreContext context, IMapper mapper, ILogger<InvoicesController> logger, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _connectionstring = configuration.GetConnectionString("defaultconnection");
            _connection = new SqlConnection(_connectionstring);
        }

        // GET: api/Invoices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<invoiceData>>> GetInvoices()
        {
            //var invoices = await _context.InvoiceData.FromSqlRaw("GetAllInvoices").ToListAsync();

            List<invoiceData> data = new List<invoiceData>();

            using (SqlCommand command = new SqlCommand("GetAllInvoices", _connection))
            {
                _connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(new invoiceData { Id = reader.GetInt32(0), Total = reader.GetDecimal(3), PartyName = reader.GetString(1), DateOfInvoice = reader.GetDateTime(2), PartyId = reader.GetInt64(4) });
                    }
                }
                _connection.Close();
            }

            return data;
            //return invoices;
        }

        [HttpGet("Range")]
        public ActionResult<IEnumerable<invoiceData>> GetInvoicesDateRange([FromQuery] DateTime StartDate, DateTime EndDate)
        {
            //var invoices = await _context.InvoiceData.FromSqlRaw("GetAllInvoices").ToListAsync();

            List<invoiceData> data = new List<invoiceData>();

            using (SqlCommand command = new SqlCommand("SELECT Party_id,p.PartyName,i.DateOfInvoice,sum(i.Rate_Of_Product*i.quantity) as total,row_number() OVER(order by DateOfInvoice ) as rank FROM invoices i inner join Parties p on p.id = Party_id where  DateOfInvoice between @StartDate and @EndDate group by Party_id,p.PartyName,i.DateOfInvoice", _connection))
            {
                command.Parameters.AddWithValue("@StartDate", StartDate);
                command.Parameters.AddWithValue("@EndDate", EndDate);
                _connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(new invoiceData { Id = reader.GetInt32(0), Total = reader.GetDecimal(3), PartyName = reader.GetString(1), DateOfInvoice = reader.GetDateTime(2), PartyId = reader.GetInt64(4) });
                    }
                }
                _connection.Close();
            }

            return data;
            //return invoices;
        }

        [EnableCors("AllowSpecificOrigin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<invoiceProducts>>> GetInvoices(int id, [FromQuery] DateTime date)
        {
            //var invoices = await _context.invoiceProducts.FromSqlRaw("GetInvoicesForProduct @partyid", new SqlParameter("@partyid", id)).ToListAsync();

            List<invoiceProducts> data = new List<invoiceProducts>();

            using (SqlCommand command = new SqlCommand("select Product_id as id, Rate_Of_Product as RateOfProduct, quantity,sum((Rate_Of_Product * Quantity)) as total,pr.ProductName,i.DateOfInvoice from invoices i inner join Products pr on pr.id = i.Product_id where i.party_id = @partyId and DateOfInvoice=@date group by i.Product_id, pr.ProductName, Rate_Of_Product, quantity, i.DateOfInvoice", _connection))
            {
                command.Parameters.AddWithValue("@partyId", id);
                command.Parameters.AddWithValue("@date", date);
                _connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //data.Add(reader.GetString(0));
                        data.Add(new invoiceProducts { Id = reader.GetInt32(0), RateOfProduct = reader.GetDecimal(1), Quantity = reader.GetInt32(2), Total = reader.GetDecimal(3), ProductName = reader.GetString(4), DateOfInvoice = reader.GetDateTime(5) });
                    }
                }
                _connection.Close();
            }

            return data;
            //return invoices;
        }

        [HttpGet("Search")]
        public async Task<ActionResult> GetInvoices([FromQuery] int partyId, string productName, DateTime date)
        {

            List<invoiceProducts> data = new List<invoiceProducts>();

            using (SqlCommand command = new SqlCommand("select Product_id as id, Rate_Of_Product as RateOfProduct, quantity,sum((Rate_Of_Product * Quantity)) as total,pr.ProductName,i.DateOfInvoice from invoices i inner join Products pr on pr.id = i.Product_id where i.party_id = @partyId and pr.ProductName in (@productName) and i.DateOfInvoice=@date group by i.Product_id, pr.ProductName, Rate_Of_Product, quantity, i.DateOfInvoice", _connection))
            {
                command.Parameters.AddWithValue("@partyId", partyId);
                command.Parameters.AddWithValue("@productName", productName);
                command.Parameters.AddWithValue("@date", date);
                _connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //data.Add(reader.GetString(0));
                        data.Add(new invoiceProducts { Id = reader.GetInt32(0), RateOfProduct = reader.GetDecimal(1), Quantity = reader.GetInt32(2), Total = reader.GetDecimal(3), ProductName = reader.GetString(4), DateOfInvoice = reader.GetDateTime(5) });
                    }
                }
                _connection.Close();
            }

            return Ok(data);

        }

        [HttpGet("Sort")]
        public async Task<ActionResult> GetInvoicesSortData([FromQuery] int partyId, string sortField, int toggle, DateTime date)
        {

            List<invoiceProducts> data = new List<invoiceProducts>();

            using (SqlCommand command = new SqlCommand("select Product_id as id, Rate_Of_Product as RateOfProduct, quantity,sum((Rate_Of_Product * Quantity)) as total,pr.ProductName,i.DateOfInvoice from invoices i inner join Products pr on pr.id = i.Product_id where i.party_id = @partyId and i.DateOfInvoice=@date group by i.Product_id, pr.ProductName, Rate_Of_Product, quantity, i.DateOfInvoice", _connection))
            {
                command.Parameters.AddWithValue("@partyId", partyId);
                command.Parameters.AddWithValue("@date", date);
                _connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //data.Add(reader.GetString(0));
                        data.Add(new invoiceProducts { Id = reader.GetInt32(0), RateOfProduct = reader.GetDecimal(1), Quantity = reader.GetInt32(2), Total = reader.GetDecimal(3), ProductName = reader.GetString(4), DateOfInvoice = reader.GetDateTime(5) });
                    }
                }
                _connection.Close();
            }

            if (sortField == "ProductName")
            {
                var SortData = data.OrderBy(x => x.ProductName);

                if (toggle == 0)
                {
                    SortData = data.OrderByDescending(x => x.ProductName);
                }

                return Ok(SortData);
            }
            else
            {
                var SortData = data.OrderBy(x => x.Id);

                if (toggle == 0)
                {
                    SortData = data.OrderByDescending(x => x.Id);
                }

                return Ok(SortData);
            }
            return Ok(data);

        }

        [HttpGet("Page/{id}")]
        public async Task<ActionResult> GetInvoices(int id, [FromQuery] int size, int pageIndex, DateTime date)
        {
            int StartIndex = size * (pageIndex - 1);
            int EndIndex = size * pageIndex;
            List<invoiceProducts> data = new List<invoiceProducts>();

            using (SqlCommand command = new SqlCommand("select Product_id as id, Rate_Of_Product as RateOfProduct, quantity,sum((Rate_Of_Product * Quantity)) as total,pr.ProductName,i.DateOfInvoice from invoices i inner join Products pr on pr.id = i.Product_id where i.party_id = @partyId and DateOfInvoice=@Date group by i.Product_id, pr.ProductName, Rate_Of_Product, quantity, i.DateOfInvoice ORDER BY Product_id OFFSET @start ROWS FETCH NEXT @end ROWS ONLY", _connection))
            {
                command.Parameters.AddWithValue("@partyId", id);
                command.Parameters.AddWithValue("@start", StartIndex);
                command.Parameters.AddWithValue("@end", EndIndex);
                command.Parameters.AddWithValue("@Date", date);

                _connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //data.Add(reader.GetString(0));
                        data.Add(new invoiceProducts { Id = reader.GetInt32(0), RateOfProduct = reader.GetDecimal(1), Quantity = reader.GetInt32(2), Total = reader.GetDecimal(3), ProductName = reader.GetString(4), DateOfInvoice = reader.GetDateTime(5) });
                    }
                }
                _connection.Close();
            }

            return Ok(data);

        }
        // POST: api/Invoices
        [HttpPost]
        public async Task<ActionResult<InvoiceDTO>> PostInvoices(InvoiceDTO invoicesDTO)
        {

            if (IsProductInInvoice(invoicesDTO.ProductId, invoicesDTO.PartyId, invoicesDTO.DateOfInvoice))
            {
                await EditProduct(invoicesDTO);

                return StatusCode(200, $"Product Edited");
            }
            else
            {
                await _context.Database.ExecuteSqlRawAsync("EXEC InsertInvoice @Rate_Of_Product, @Quantity, @Party_id, @Product_id,@Date",
                    new SqlParameter("@Rate_Of_Product", invoicesDTO.RateOfProduct),
                    new SqlParameter("@Quantity", invoicesDTO.Quantity),
                    new SqlParameter("@Party_id", invoicesDTO.PartyId),
                    new SqlParameter("@Product_id", invoicesDTO.ProductId),
                    new SqlParameter("@Date", invoicesDTO.DateOfInvoice));
                return StatusCode(201, $"invoice Created Successfully");
            }

        }

        //update invoice for perticular party 
        [HttpPut("{id}")]
        public async Task<ActionResult<invoiceProducts>> PutInvoices(int id, invoiceProducts invoiceProduct)
        {

            await _context.Database.ExecuteSqlRawAsync("EXEC UpdateInvoice @partyid,@Rate_Of_Product, @Quantity,@Product_id,@Date",
                 new SqlParameter("@partyid", id),
                 new SqlParameter("@Rate_Of_Product", invoiceProduct.RateOfProduct),
                 new SqlParameter("@Quantity", invoiceProduct.Quantity),
                 new SqlParameter("@Product_id", invoiceProduct.Id),
                 new SqlParameter("@Date", invoiceProduct.DateOfInvoice));

            return StatusCode(201, $"invoice updated Successfully");
        }

        // DELETE: api/Invoices
        [HttpDelete]
        public async Task<ActionResult<Invoices>> DeleteInvoices([FromQuery] int partyId, int productId, DateTime date)
        {
            await _context.Database.ExecuteSqlRawAsync($"DeleteInvoice @partyId,@productId,@dateOfInvoice",
                 new SqlParameter("@partyid", partyId),
                  new SqlParameter("@productId", productId),
                  new SqlParameter("@dateOfInvoice", date)
                );

            return StatusCode(202, $"invoice Deleted Successfully");
        }

        private bool InvoicesExists(int id)
        {
            return _context.Invoices.Any(e => e.Id == id);
        }
        private bool IsProductInInvoice(int productId, int partyId, DateTime date)
        {
            string d = date.Year + "/" + date.Month + "/" + date.Day;
            bool b = _context.Invoices.Any(e => e.ProductId == productId && e.PartyId == partyId && e.DateOfInvoice == date.Date);
            return b;
        }
        private bool ProductNameExists(string productName)
        {
            return _context.Products.Any(e => e.ProductName.Contains(productName));
        }

        public async Task<IActionResult> EditProduct(InvoiceDTO invoicesDTO)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC editInvoiceProduct @Rate_Of_Product,@Quantity,@Party_id, @Product_id,@Date",
                        new SqlParameter("@Rate_Of_Product", invoicesDTO.RateOfProduct),
                       new SqlParameter("@Quantity", invoicesDTO.Quantity),
                       new SqlParameter("@Party_id", invoicesDTO.PartyId),
                       new SqlParameter("@Product_id", invoicesDTO.ProductId),

                       new SqlParameter("@Date", invoicesDTO.DateOfInvoice));
            return Ok("Edit");
        }
    }
}
