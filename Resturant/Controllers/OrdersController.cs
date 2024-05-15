using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Resturant.Data;
using Resturant.Models;
using static NuGet.Packaging.PackagingConstants;

namespace Resturant.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Orders.Include(o => o.FoodItem).Include(o => o.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.FoodItem)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create(int? id)
        {
            
            var Items = await _context.FoodItems.FindAsync(id);
            return View(Items);
        }


        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int fooditemId, int quantity)
        {
            // Get the user ID from the session
            int userid = Convert.ToInt32(HttpContext.Session.GetString("userid"));

            // Check if the user exists in the database
            User user = await _context.Users.FindAsync(userid);
            if (user == null)
            {
                // Return an error message if the user does not exist
                ModelState.AddModelError("", "User not found");
                return View();
            }

            // Create a new order object and set its properties
            Order order = new Order
            {
                fooditemId = fooditemId,
                quantity = quantity,
                userid = userid,
                orderdate = DateTime.Today
            };

            // Add the new order to the database and save changes
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Cart));
        }
        public async Task<IActionResult> Cart()// for customer
        {
            ViewData["FoodItems"] = _context.FoodItems.ToList();
            int id = Convert.ToInt32(HttpContext.Session.GetString("userid")); ;
            var items = await _context.Orders.FromSqlRaw("select * from Orders where userid = '" + id + "' ").ToListAsync();
            return View(items);
        }
       

        public async Task<IActionResult> customerOrders(int? id)// for Admin
        {


            var Items = await _context.Orders.FromSqlRaw("select *  from Orders where  userid = '" + id + "'  ").ToListAsync();
            return View(Items);

        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
            {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["fooditemId"] = new SelectList(_context.FoodItems, "FoodItemId", "Category", order.fooditemId);
            ViewData["userid"] = new SelectList(_context.Users, "UserId", "Email", order.userid);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,fooditemId,userid,quantity,orderdate")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Cart));
            }
            ViewData["fooditemId"] = new SelectList(_context.FoodItems, "FoodItemId", "Category", order.fooditemId);
            ViewData["userid"] = new SelectList(_context.Users, "UserId", "Email", order.userid);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.FoodItem)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Cart));
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
    }
}
