using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class AuctionItem
    {
        [Key]
        public int ItemNumber { get; set; }
        public string ItemDescription { get; set; }
        public decimal RatingPrice { get; set; }
        public decimal BidPrice { get; set; }
        public string BidCustomName { get; set; }
        public string BidCustomPhone { get; set; }
        public DateTime BidTimeStamp { get; set; }
        public IdentityUser User { get; set; }
    }
}
