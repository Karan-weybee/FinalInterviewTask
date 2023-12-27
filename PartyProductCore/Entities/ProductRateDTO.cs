using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PartyProductCore.Entities
{
    public class ProductRateDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Rate Is Required")]
        public decimal Rate { get; set; }

        [Required(ErrorMessage = "Date Is Required")]
        public DateTime DateOfRate { get; set; }

        [Required(ErrorMessage = "{Product Is Required")]
        public int ProductId { get; set; }
    }
}
