using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Entities;

namespace Auktionshus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionItemsController : ControllerBase
    {
        private readonly DataContext _context;

        public AuctionItemsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/AuctionItems
        [HttpGet]
        public IEnumerable<AuctionItem> GetAllAuctionItems()
        {
            return _context.AuctionItems;
        }

        // GET: api/AuctionItems/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuctionItem([FromRoute] int itemNumber)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var auctionItem = await _context.AuctionItems.FindAsync(itemNumber);

            //if (auctionItem == null)
            //{
            //    return NotFound();
            //}

            return Ok(auctionItem);
        }

        [HttpPost]
        public async Task<IActionResult> ProvideBid([FromBody] Bid bid)
        {
            var auctionItem = _context.AuctionItems.FirstOrDefault(item => item.ItemNumber == bid.ItemNumber);
            if(auctionItem is null)
            {
                return NotFound("Vare findes ikke");
            }
            if(bid.Price <= auctionItem.BidPrice)
            {
                return BadRequest("Bud for lavt");
            } else
            {
                auctionItem.BidPrice = bid.Price;
                auctionItem.BidCustomName = bid.CustomName;
                auctionItem.BidCustomPhone = bid.CustomPhone;
                auctionItem.BidTimeStamp = DateTime.Now;
                await _context.SaveChangesAsync();
                return Ok();
            }

        }

        private bool AuctionItemExists(int id)
        {
            return _context.AuctionItems.Any(e => e.ItemNumber == id);
        }
    }
}