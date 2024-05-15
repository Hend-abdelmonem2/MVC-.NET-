using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using Resturant.Data;
using Resturant.Models;

namespace Resturant.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
              return _context.Users != null ? 
                          View(await _context.Users.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Users'  is null.");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost, ActionName("Login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string name, string password)
        {
            SqlConnection con = new SqlConnection("Data Source=DESKTOP-VT17DDD\\SQL2022;Initial Catalog=Resturant;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            string sql;
            sql = "SELECT * FROM Users where Name ='" + name + "' and  Password ='" + password + "' ";
            SqlCommand comm = new SqlCommand(sql, con);
            con.Open();
            SqlDataReader reader = comm.ExecuteReader();

            if (reader.Read())
            {
                string role = (string)reader["role"];
                string id = Convert.ToString((int)reader["UserId"]);
                HttpContext.Session.SetString("Name", name);
                HttpContext.Session.SetString("Role", role);
                HttpContext.Session.SetString("userid", id);
                reader.Close();
                con.Close();
                if (role == "customer")
                    return RedirectToAction("Menu", "FoodItems");

                else
                    return RedirectToAction("Index", "FoodItems");

            }
            else
            {
                ViewData["Message"] = "wrong user name password";
                return View();
            }
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Name,Password,Email")] User user)
        {
            user.Role = "customer";
            
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Login));
            
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit()
        {
            int id = Convert.ToInt32(HttpContext.Session.GetString("userid"));
            var user = await _context.Users.FindAsync(id);
            return View();

        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Name,Role,Password,Email")] User user)
        {
            user.Role = "customer";
            _context.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Login));
           
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("userid");
            HttpContext.Session.Remove("Name");
            HttpContext.Session.Remove("Role");
            return RedirectToAction("Login", "Users");
        }
        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
