using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Resturant.Data;
using Resturant.Models;

namespace Resturant.Controllers
{
    public class FoodItemsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment _environment;
        public FoodItemsController(ApplicationDbContext db, IWebHostEnvironment environment)
        {
            this.db = db;
            _environment = environment;

        }

        // GET: FoodItems
        public async Task<IActionResult> Index()
        {
            IEnumerable<FoodItem> items = db.FoodItems.ToList();
            return View(items);
            //return db.FoodItems != null ? 
            //            View(await db.FoodItems.ToListAsync()) :
            //            Problem("Entity set 'ApplicationDbContext.FoodItems'  is null.");
        }

        public async Task<IActionResult> Menu()
        {
            return View(await db.FoodItems.ToListAsync());
        }



        // GET: FoodItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || db.FoodItems == null)
            {
                return NotFound();
            }

            var foodItem = await db.FoodItems
                .FirstOrDefaultAsync(m => m.FoodItemId == id);
            if (foodItem == null)
            {
                return NotFound();
            }

            return View(foodItem);
        }

        // GET: FoodItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FoodItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]  // to more security
        public async Task<IActionResult> Create([Bind("FoodItemId,Name,Price,Quantity,Category,Foodpic")] FoodItem Item, IFormFile img_file)
        {
            // to create Images folder in the project Path.
            string path = Path.Combine(_environment.WebRootPath, "images"); // wwwroot/Img/
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (img_file != null)
            {
                path = Path.Combine(path, img_file.FileName); // for exmple : /Img/Photoname.png
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await img_file.CopyToAsync(stream);
                    ViewBag.Message = string.Format("<b>{0}</b> uploaded.</br>", img_file.FileName.ToString());
                    Item.Foodpic = img_file.FileName;
                }
            }
            else
            {
                Item.Foodpic = "default.jpg";//to save the default image path in database;
            }
            try
            {
                db.Add(Item);
                db.SaveChanges();
                return RedirectToAction("Index");


            }
            catch (Exception ex) { ViewBag.exc = ex.Message; }
            return View();
        }


        // GET: FoodItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || db.FoodItems == null)
            {
                return NotFound();
            }

            var foodItem = await db.FoodItems.FindAsync(id);
            if (foodItem == null)
            {
                return NotFound();
            }
            return View(foodItem);
        }

        // POST: FoodItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FoodItemId,Name,Price,Quantity,Category,Foodpic")] FoodItem foodItem)
        {
            if (id != foodItem.FoodItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(foodItem);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FoodItemExists(foodItem.FoodItemId))
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
            return View(foodItem);
        }

        // GET: FoodItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || db.FoodItems == null)
            {
                return NotFound();
            }

            var foodItem = await db.FoodItems
                .FirstOrDefaultAsync(m => m.FoodItemId == id);
            if (foodItem == null)
            {
                return NotFound();
            }

            return View(foodItem);
        }

        // POST: FoodItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (db.FoodItems == null)
            {
                return Problem("Entity set 'ApplicationDbContext.FoodItems'  is null.");
            }
            var foodItem = await db.FoodItems.FindAsync(id);
            if (foodItem != null)
            {
                db.FoodItems.Remove(foodItem);
            }
            
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FoodItemExists(int id)
        {
          return (db.FoodItems?.Any(e => e.FoodItemId == id)).GetValueOrDefault();
        }
    }
}
