using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class DataContext : DbContext
    {
        public DbSet<AuctionItem> AuctionItems { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            Database.EnsureCreated();
            if(AuctionItems.CountAsync().Result == 0)
            {
                var item = new AuctionItem()
                {
                    ItemDescription = "Lille vase",
                    RatingPrice = 100
                };
                AuctionItems.Add(item);
                SaveChanges();
                item = new AuctionItem()
                {
                    ItemDescription = "Stor vase",
                    RatingPrice = 300
                };
                AuctionItems.Add(item);
                SaveChanges();
            }
        }
    }


}
