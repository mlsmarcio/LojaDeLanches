﻿using System.Diagnostics;
using LanchesMac.Models;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            TempData["Nome"] = "Márcio";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
