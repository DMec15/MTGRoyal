using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MTGRoyal.Models;

namespace MTGRoyal.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}