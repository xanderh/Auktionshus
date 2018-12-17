using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities
{
    public class Bid
    {
        public int Id { get; set; }
        public int ItemNumber { get; set; }
        public decimal Price { get; set; }
        [Required]
        [MinLength(4)]
        public string CustomName { get; set; }
        [Required]
        [MinLength(8)]
        [MaxLength(8)]
        public string CustomPhone { get; set; }

        public Bid(int itemNumber, decimal price, string customName, string customPhone)
        {
            ItemNumber = itemNumber;
            Price = price;
            CustomName = customName;
            CustomPhone = customPhone;
        }
    }
}
