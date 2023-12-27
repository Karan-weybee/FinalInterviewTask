using System;
using System.Collections.Generic;

namespace PartyProductCore.Models
{
    public partial class Invoices
    {
        public int Id { get; set; }
        public decimal RateOfProduct { get; set; }
        public int Quantity { get; set; }
        public int PartyId { get; set; }
        public int ProductId { get; set; }
        public DateTime DateOfInvoice { get; set; }

        public virtual Parties Party { get; set; }
        public virtual Products Product { get; set; }
    }
}
