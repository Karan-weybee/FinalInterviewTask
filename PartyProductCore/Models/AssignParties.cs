using System;
using System.Collections.Generic;

namespace PartyProductCore.Models
{
    public partial class AssignParties
    {
        public int Id { get; set; }
        public int PartyId { get; set; }
        public int ProductId { get; set; }

        public virtual Parties Party { get; set; }
        public virtual Products Product { get; set; }
    }
}
