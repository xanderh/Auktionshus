using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Entities;
using Microsoft.AspNetCore.Identity;
using System.Net.Http;
using Newtonsoft.Json;

namespace Web.Controllers
{
    public class AuctionItemsController : Controller
    {
        private readonly DataContext _context;
        private readonly HttpClient _httpClient;
        private Uri BaseEndPoint { get; set; }

        public AuctionItemsController(DataContext context)
        {
            BaseEndPoint = new Uri("https://localhost:44324/api/auctionItems");
            _context = context;
            _httpClient = new HttpClient();
        }

        // GET: AuctionItems
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync(BaseEndPoint, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return View(JsonConvert.DeserializeObject<List<AuctionItem>>(data));
        }

        public async Task<IActionResult> MyBids()
        {
            var response = await _httpClient.GetAsync(BaseEndPoint, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var dataDeserialized = JsonConvert.DeserializeObject<List<AuctionItem>>(data);
            return View(dataDeserialized.Where(item => item.User == (User.Identity as IdentityUser)).ToList());
        }

        // GET: AuctionItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _httpClient.GetAsync(BaseEndPoint.ToString() + $"/{id}", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var auctionItem = JsonConvert.DeserializeObject<AuctionItem>(data);

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

            var response = await _httpClient.GetAsync(BaseEndPoint.ToString() + $"/{id}", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var auctionItem = JsonConvert.DeserializeObject<AuctionItem>(data);

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
        public async Task<IActionResult> Bid(int id, [FromForm] AuctionItem NewAuctionItem)
        {
            var response = await _httpClient.GetAsync(BaseEndPoint.ToString() + $"/{id}", HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            var auctionItem = JsonConvert.DeserializeObject<AuctionItem>(data);

            if (auctionItem is null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (NewAuctionItem.BidPrice <= auctionItem.BidPrice)
                    {
                        ViewBag.PriceTooLow = true;
                        return View(auctionItem);
                    }
                    Bid bid = new Bid(auctionItem.ItemNumber, NewAuctionItem.BidPrice, NewAuctionItem.BidCustomName, NewAuctionItem.BidCustomPhone);
                    var postResponse = _httpClient.PostAsJsonAsync<Bid>(BaseEndPoint, bid);
                    postResponse.Result.EnsureSuccessStatusCode();
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
