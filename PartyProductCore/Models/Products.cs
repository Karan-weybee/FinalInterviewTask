using System;
using System.Collections.Generic;

namespace PartyProductCore.Models
{
    public partial class Products
    {
        public Products()
        {
            AssignParties = new HashSet<AssignParties>();
            Invoices = new HashSet<Invoices>();
            ProductRates = new HashSet<ProductRates>();
        }

        public int Id { get; set; }
        public string ProductName { get; set; }

        public virtual ICollection<AssignParties> AssignParties { get; set; }
        public virtual ICollection<Invoices> Invoices { get; set; }
        public virtual ICollection<ProductRates> ProductRates { get; set; }
    }
}
