using System;
using System.Collections.Generic;

namespace PartyProductCore.Models
{
    public partial class Parties
    {
        public Parties()
        {
            AssignParties = new HashSet<AssignParties>();
            Invoices = new HashSet<Invoices>();
        }

        public int Id { get; set; }
        public string PartyName { get; set; }

        public virtual ICollection<AssignParties> AssignParties { get; set; }
        public virtual ICollection<Invoices> Invoices { get; set; }
    }
}
