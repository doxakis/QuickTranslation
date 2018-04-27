using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAppDemo.Models;
using QuickTranslationPack;

namespace WebAppDemo.Controllers
{
    public class HomeController : Controller
    {
		static QuickTranslation Translations = QuickTranslation.Prepare(@"
            MESSAGE_001: This is a message from the controller.
				- fr: Ceci est un message du contrôleur.
        ");

		public IActionResult Index()
		{
			ViewBag.Message = Translations["MESSAGE_001"];
			return View();
		}
		
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
