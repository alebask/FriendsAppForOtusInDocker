using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FriendsAppNoORM.Data;
using FriendsAppNoORM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FriendsAppNoORM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDatabaseContext _db;
       
        public HomeController(ILogger<HomeController> logger, ApplicationDatabaseContext db)
        {
            _logger = logger;
            _db = db;
        }

        [AllowAnonymous]
        public IActionResult Index(string message)
        {
            return View(message);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
