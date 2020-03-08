using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RapidWeb.Models;

namespace RapidWeb.Controllers
{
    public class DashboardController : Controller
    {
        [CustomAuthorize(Role.SuperAdmin,Role.Admin,Role.DataOperator,Role.HR,Role.Inventory,Role.RTPUser,Role.AccountsManager,Role.Activity)]
        public ActionResult Index()
        {
            switch (((User)Session["User"]).RoleId)
            {
                case Role.SuperAdmin:
                {
                    return RedirectToAction("Master", "Dashboard");
                }

                case Role.Admin:
                {
                    return RedirectToAction("Admin", "Dashboard");
                }

                case Role.DataOperator:
                {
                    return RedirectToAction("DataOperator", "Dashboard");
                }

                case Role.HR:
                {
                    return RedirectToAction("HR", "Dashboard");
                }

                case Role.Inventory:
                {
                    return RedirectToAction("Inventory", "Dashboard");
                }

                case Role.RTPUser:
                {
                    return RedirectToAction("RTPUser", "Dashboard");
                }

                case Role.AccountsManager:
                {
                    return RedirectToAction("AccountsManager", "Dashboard");
                }

                case Role.Activity:
                {
                    return RedirectToAction("Activity", "Dashboard");
                }

                default:
                {
                    return RedirectToAction("Logout", "User");
                }
            }
        }

        [CustomAuthorize(Role.SuperAdmin)]
        public ActionResult Master()
        {
            return View();
        }

        [CustomAuthorize(Role.Admin)]
        public ActionResult Admin()
        {
            return View();
        }

        [CustomAuthorize(Role.DataOperator)]
        public ActionResult DataOperator()
        {
            return View();
        }

        [CustomAuthorize(Role.HR)]
        public ActionResult HR()
        {
            return View();
        }

        [CustomAuthorize(Role.Inventory)]
        public ActionResult Inventory()
        {
            return View();
        }

        [CustomAuthorize(Role.RTPUser)]
        public ActionResult RTPUser()
        {
            return View();
        }

        [CustomAuthorize(Role.AccountsManager)]
        public ActionResult AccountsManager()
        {
            return View();
        }

        [CustomAuthorize(Role.Activity)]
        public ActionResult Activity()
        {
            return View();
        }
    }
}