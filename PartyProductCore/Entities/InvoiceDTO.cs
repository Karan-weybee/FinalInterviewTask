using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PartyProductCore.Entities
{
    public class InvoiceDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Rate Is Required")]
        public decimal RateOfProduct { get; set; }
        [Required(ErrorMessage = "Quantity Is Required")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Party Is Required")]
        public int PartyId { get; set; }
        [Required(ErrorMessage = "Product Is Required")]
        public int ProductId { get; set; }
        public DateTime DateOfInvoice { get; set; }
    }
}
