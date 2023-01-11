using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System;
using UVisa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace UVisa.Controllers
{
    public class MainController : Controller
    {
        private readonly nurlans1_ucgContext _sql;
        public MainController(nurlans1_ucgContext sql)
        {
            _sql = sql;
        }
        [Authorize(Roles = "Admin")]

        public IActionResult AdminIndex(int page = 0)
        {
            ViewBag.Count = Math.Ceiling((decimal)_sql.UserInfos.Count() / 20);
            return View(_sql.Orders.Include(x=>x.OrderUserInfo).Where(x=>x.OrderUserInfo.UserInfoId != 29).Skip(page * 20).Take(20).ToList());
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AdminContact(int page = 0)
        {
            ViewBag.Count = Math.Ceiling((decimal)_sql.Contacts.Count() / 20);
            return View(_sql.Contacts.Skip(page * 20).Take(20).ToList());
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult RusIndex()
        {
            return View();
        }
        public IActionResult Application()
        {
            return View();
        }
        public IActionResult RusApplication()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddContact(Contact contact)
        {
            _sql.Contacts.Add(contact);
            _sql.SaveChanges();
            return RedirectToAction("Index", "Main");
        }
        public class MainData
        {
            public int OrderId { get; set; }
            public string Name { get; set; }
            public decimal? Money { get; set; }
            public string? Passport { get; set; }
            public string TypeVisa { get; set; }
            public DateTime? Date { get; set; }
            public string Photo { get; set; }
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var getUser = _sql.UserInfos.Include(x => x.UserInfoStatus).SingleOrDefault(x => x.UserInfoName == username && x.UserInfoPassportId == password);

            if (getUser != null)
            {
                ClaimsIdentity identity = new ClaimsIdentity("Cookie");
                identity.AddClaims(new[]
                {
                        new Claim(ClaimTypes.Sid, "Id"),
                        new Claim(ClaimTypes.Role, getUser.UserInfoStatus.StatusName),
                });

                var principal = new ClaimsPrincipal(identity);
                HttpContext.SignInAsync(principal, new AuthenticationProperties()
                {
                    ExpiresUtc = DateTime.UtcNow.AddDays(7)
                });


                return RedirectToAction("AdminIndex", "Main");
            }
            else
            {
                ViewBag.Error = "İstifadəçi adı və ya şifrə yanlışdır";
                return View("Login");
            }
        }
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync().Wait();
            return RedirectToAction("Login", "Main");
        }
        public JsonResult Search(string query)
        {
            var a = _sql.UserInfos.Where(x => x.UserInfoName.ToLower().Contains(query.ToLower()));
            return Json(a);
        }
        public JsonResult SearchContact(string query)
        {
            var a = _sql.Contacts.Where(x => x.ContactName.ToLower().Contains(query.ToLower()));
            return Json(a);
        }
        //odenis sistemi
        [HttpPost]
        public IActionResult Odenikec(UserInfo userInfo, IFormFile Photo)
        {
            string filename = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetExtension(Photo.FileName);
            using (Stream stream = new FileStream("wwwroot/images/" + filename, FileMode.Create))
            {
                Photo.CopyTo(stream);
            }
            userInfo.UserInfoFile = filename;
            userInfo.UserInfoStatusId = 2;
            _sql.UserInfos.Add(userInfo);
            _sql.SaveChanges();
            var claims = new List<Claim>
                {
                    new Claim("Id", userInfo.UserInfoId.ToString()),
                };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var princitial = new ClaimsPrincipal(identity);
            var props = new AuthenticationProperties();
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, princitial, props).Wait();
            Order o = new Order();
            o.OrderUserInfoId = userInfo.UserInfoId;
            o.OrderMoney = 1;
            o.OrderDate = DateTime.Now;
            o.OrderStatus = false;
            _sql.Add(o);
            _sql.SaveChanges();
            return RedirectToAction("Odeniset", "Main", new { orderid = o.OrderId });
        }
        [HttpGet]
        public IActionResult Odeniset(int orderid)
        {
            var order = _sql.Orders.SingleOrDefault(x => x.OrderId == orderid);
            var user = _sql.UserInfos.SingleOrDefault(x => x.UserInfoId == order.OrderUserInfoId);
            MainData md = new MainData();
            md.OrderId = orderid;
            md.Name = user.UserInfoName;
            md.Money = order.OrderMoney;
            md.Photo = user.UserInfoFile;
            md.Date = order.OrderDate;
            md.Passport = user.UserInfoPassportId;
            md.TypeVisa = user.UserInfoTypeVisa;
            string json = "{\"public_key\":\"i000200187\",\"amount\":\"" + order.OrderMoney + "\",\"currency\":\"AZN\",\"description\":\"" + user.UserInfoName + "\",\"order_id\":\"" + orderid + "\", \"language\":\"az\"}";
            ViewBag.Data = Base64Encode(json);
            string signature = "P8GBCzexgrRcqDkhC496tVjm" + ViewBag.Data + "P8GBCzexgrRcqDkhC496tVjm";
            ViewBag.Signature = Hash(signature);
            return View(md);
        }
        [HttpGet]
        public IActionResult result()
        {
            var usid = int.Parse(User.FindFirstValue("Id"));
            var orderid = _sql.Orders.OrderByDescending(x => x.OrderDate).LastOrDefault(x => x.OrderUserInfoId == usid).OrderId;
            var order = _sql.Orders.SingleOrDefault(x => x.OrderId == orderid);
            var user = _sql.UserInfos.SingleOrDefault(x => x.UserInfoId == order.OrderUserInfoId);
            MainData md = new MainData();
            md.OrderId = orderid;
            md.Name = user.UserInfoName;
            md.Money = order.OrderMoney;
            md.Photo = user.UserInfoFile;
            md.Date = order.OrderDate;
            md.Passport = user.UserInfoPassportId;
            md.TypeVisa = user.UserInfoTypeVisa;
            string json = "{\"public_key\":\"i000200187\",\"order_id\":\"" + orderid + "\"}";
            ViewBag.Data = Base64Encode(json);
            string signature = "P8GBCzexgrRcqDkhC496tVjm" + ViewBag.Data + "P8GBCzexgrRcqDkhC496tVjm";
            ViewBag.Signature = Hash(signature);
            return View(md);
        }

        public IActionResult result_s(int id)
        {
            Order order = _sql.Orders.SingleOrDefault(x => x.OrderId == id);
            order.OrderStatus = true;
            _sql.SaveChanges();
            return Ok("success");
        }
        static string Hash(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] shabytes = SHA1.Create().ComputeHash(bytes);
            return Convert.ToBase64String(shabytes);
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
