﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AdvScreen.Models;

namespace AdvScreen.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //[HttpGet("download")]
        public IActionResult InstructionsDownload([FromQuery] string link)
        {
            var fileName = "Инструкция по работе с REDI.docx";
            var net = new System.Net.WebClient();
            //var data = net.DownloadData(fileName);
            //var content = new System.IO.MemoryStream(data);
            var contentType = "APPLICATION/octet-stream";
            
            var filePath = fileName;
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, contentType, fileName);
        }

        public IActionResult Locations()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        public IActionResult ErrorHandle()
        {
            var test = TempData["Message"];
            return View();
        }
    }
}
