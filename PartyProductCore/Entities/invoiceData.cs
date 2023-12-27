using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PartyProductCore.Entities
{
    public class invoiceData
    {
        [Required]
        public int id { get; set; }
        public decimal Total { get; set; }
        public string partyName { get; set; }
    }
}
