using System;
using System.Collections.Generic;

namespace PartyProductCore.Models
{
    public partial class ProductRates
    {
        public int Id { get; set; }
        public decimal Rate { get; set; }
        public DateTime DateOfRate { get; set; }
        public int ProductId { get; set; }

        public virtual Products Product { get; set; }
    }
}
