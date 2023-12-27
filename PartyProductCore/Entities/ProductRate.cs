using System;

namespace PartyProductCore.Entities
{
    public class ProductRate
    {
        public int Id { get; set; }

        public decimal Rate { get; set; }
        public DateTime DateOfRate { get; set; }
        public string ProductName { get; set; }
    }
}
