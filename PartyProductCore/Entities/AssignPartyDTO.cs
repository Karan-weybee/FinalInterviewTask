using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PartyProductCore.Entities
{
    public class AssignPartyDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Party Is Required")]
        public int PartyId { get; set; }

        [Required(ErrorMessage = "product Is Required")]
        public int ProductId { get; set; }

    }
}
