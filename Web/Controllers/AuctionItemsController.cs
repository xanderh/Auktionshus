using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Entities;
using Microsoft.AspNetCore.Identity;

namespace Web.Controllers
{
    public class AuctionItemsController : Controller
    {
        private readonly DataContext _context;

        public AuctionItemsController(DataContext context)
        {
            _context = context;
        }

        // GET: AuctionItems
        public async Task<IActionResult> Index()
        {
            return View(await _context.AuctionItems.ToListAsync());
        }

        public IActionResult MyBids()
        {
            return View(_context.AuctionItems.Where(item => item.User == (User.Identity as IdentityUser)).ToList());
        }

        // GET: AuctionItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auctionItem = await _context.AuctionItems
                .FirstOrDefaultAsync(m => m.ItemNumber == id);
            if (auctionItem == null)
            {
                return NotFound();
            }

            return View(auctionItem);
        }

        // GET: AuctionItems/Edit/5
        public async Task<IActionResult> Bid(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var auctionItem = _context.AuctionItems.FirstOrDefault(item => item.ItemNumber == id);
            if (auctionItem == null)
            {
                return RedirectToAction("Index");
            }
            return View(auctionItem);
        }

        // POST: AuctionItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bid(int id, [FromForm] AuctionItem bid)
        {
            var auctionItem = _context.AuctionItems.FirstOrDefault(item => item.ItemNumber == id);
            if (auctionItem is null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if(bid.BidPrice <= auctionItem.BidPrice)
                    {
                        return View(auctionItem);
                    }
                    auctionItem.BidPrice = bid.BidPrice;
                    auctionItem.BidCustomName = bid.BidCustomName;
                    auctionItem.BidCustomPhone = bid.BidCustomPhone;
                    auctionItem.BidTimeStamp = DateTime.Now;
                    auctionItem.User = User.Identity as IdentityUser;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuctionItemExists(auctionItem.ItemNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(auctionItem);
        }

        private bool AuctionItemExists(int id)
        {
            return _context.AuctionItems.Any(e => e.ItemNumber == id);
        }

        public IActionResult SeedDatabase()
        {
            if (_context.AuctionItems.CountAsync().Result == 0)
            {
                var item = new AuctionItem()
                {
                    ItemDescription = "Lille vase",
                    RatingPrice = 100
                };
                _context.AuctionItems.Add(item);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
