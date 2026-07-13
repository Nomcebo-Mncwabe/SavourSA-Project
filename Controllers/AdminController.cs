using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SavourSA_Project.Controllers
{
    
        public class AdminController : Controller
        {
            public ActionResult Dashboard()
            {
                return View();
            }

            public ActionResult Recipes()
            {
                return View();
            }

            public ActionResult Categories()
            {
                return View();
            }

            public ActionResult Users()
            {
                return View();
            }

            public ActionResult Reports()
            {
                return View();
            }

            public ActionResult Comments()
            {
                return View();
            }

            public ActionResult Settings()
            {
                return View();
            }
        }
    }