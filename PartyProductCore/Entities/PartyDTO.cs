using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PartyProductCore.Entities
{
    public class PartyDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Party Name Is Required")]
        public string PartyName { get; set; }
    }
}
