using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PartyProductCore.Entities
{
    public class invoiceData
    {
        public int Id { get; set; }
        public decimal Total { get; set; }
        public string PartyName { get; set; }
        public DateTime DateOfInvoice { get; set; }
    }
}
