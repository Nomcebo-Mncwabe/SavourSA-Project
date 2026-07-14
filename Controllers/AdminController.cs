using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SavourSA_Project.Models.ViewModels;
using static SavourSA_Project.Models.ViewModels.DashboardViewModel;

namespace SavourSA_Project.Controllers
{
    
        public class AdminController : Controller
        {
            public ActionResult Dashboard()
            {
                DashboardViewModel model = new DashboardViewModel
                {
                    TotalUsers = 0,
                    TotalRecipes = 0,
                    TotalCategories = 0,
                    TotalComments = 0
                };

                return View(model);
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