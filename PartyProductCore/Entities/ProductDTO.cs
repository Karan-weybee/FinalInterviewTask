using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PartyProductCore.Entities
{
    public class ProductDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Product Name Is Required")]
        public string ProductName { get; set; }

    }
}
