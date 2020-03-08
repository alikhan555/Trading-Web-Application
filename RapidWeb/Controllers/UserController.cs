using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RapidWeb.Models;
using RapidWeb.ViewModel;

namespace RapidWeb.Controllers
{
    public class UserController : Controller
    {
        private AppDbContext db;

        public UserController()
        {
            db = new AppDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        
        public ActionResult Login()
        {
            if (Session["User"] != null)
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        [CustomAuthorize(Role.SuperAdmin, Role.AccountsManager, Role.Admin, Role.DataOperator, Role.HR, Role.Inventory, Role.RTPUser)]
        public ActionResult ChangePassword()
        {
            ChangePasswordViewModel changePasswordViewModel = new ChangePasswordViewModel()
            {
                UserId = ((Models.User)Session["User"]).Id,
            };

            return View(changePasswordViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.AccountsManager, Role.Admin, Role.DataOperator, Role.HR, Role.Inventory, Role.RTPUser)]
        public ActionResult ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(changePasswordViewModel);
            }

            if (changePasswordViewModel.NewPassword != changePasswordViewModel.ConfirmNewPassword)
            {
                ModelState.AddModelError(String.Empty, "New Passwords does not matched.");
                return View(changePasswordViewModel);
            }

            User user = db.Users.SingleOrDefault(x => x.Id == changePasswordViewModel.UserId );

            if (user == null)
                return HttpNotFound();

            if (user.Password != changePasswordViewModel.CurrentPassword)
            {
                ModelState.AddModelError(String.Empty, "Wrong current password.");
                return View(changePasswordViewModel);
            }

            user.Password = changePasswordViewModel.NewPassword;
            db.SaveChanges();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User loginUser)
        {
            if (!ModelState.IsValid)
            {
                return View(loginUser);
            }

            User user = db.Users.SingleOrDefault(x =>
                x.UserName == loginUser.UserName && x.Password == loginUser.Password);

            if (user != null && user.Password != loginUser.Password)
                user = null;

            if (user != null)
            {
                Session["User"] = user;
                Session.Timeout = 20;
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ModelState.AddModelError(String.Empty, "UserName or Password is incorrect.");
                return View(loginUser);
            }
        }

        public ActionResult Logout()
        {
            Session["User"] = null;
            return RedirectToAction("Login");
        }

        [CustomAuthorize(Role.SuperAdmin)]
        public ActionResult Index()
        {
            IEnumerable<User> users = db.Users.Include(x => x.Role).Where(x => x.RoleId != Role.SuperAdmin).ToList();

            return View(users);
        }

        [CustomAuthorize(Role.SuperAdmin)]
        public ActionResult Create()
        {
            UserViewModel userViewModel = new UserViewModel()
            {
                User = new User(),
                Roles = db.Roles.Where(x => x.Id != Role.SuperAdmin).ToList()
            };
            return View(userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin)]
        public ActionResult Create(User user)
        {
            if (!ModelState.IsValid)
            {
                UserViewModel userViewModel = new UserViewModel()
                {
                    User = new User(),
                    Roles = db.Roles.Where(x => x.Id != Role.SuperAdmin).ToList()
                };
                return View(userViewModel);
            }

            if(db.Users.Any(x => x.UserName == user.UserName))
            {
                ModelState.AddModelError(String.Empty, "Username must be unique, this has already been used.");

                UserViewModel userViewModel = new UserViewModel()
                {
                    User = new User(),
                    Roles = db.Roles.Where(x => x.Id != Role.SuperAdmin).ToList()
                };
                return View(userViewModel);
            }

            db.Users.Add(user);

            db.SaveChanges();

            return RedirectToAction("Index","User");
        }

        [CustomAuthorize(Role.SuperAdmin)]
        public ActionResult Edit(int id)
        {
            User user = db.Users.SingleOrDefault(x => x.Id == id);

            if (user == null)
                return HttpNotFound();

            UserViewModel userViewModel = new UserViewModel()
            {
                User = user,
                Roles = db.Roles.Where(x => x.Id != Role.SuperAdmin).ToList()
            };
            return View(userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin)]
        public ActionResult Edit(User user)
        {
            if (!ModelState.IsValid)
            {
                UserViewModel userViewModel = new UserViewModel()
                {
                    User = user,
                    Roles = db.Roles.Where(x => x.Id != Role.SuperAdmin).ToList()
                };
                return View(userViewModel);
            }

            if (db.Users.Any(x => x.Id != user.Id && x.UserName == user.UserName))
            {
                ModelState.AddModelError(String.Empty, "Username must be unique, this has already been used.");

                UserViewModel userViewModel = new UserViewModel()
                {
                    User = user,
                    Roles = db.Roles.Where(x => x.Id != Role.SuperAdmin).ToList()
                };

                return View(userViewModel);
            }

            User userInDb = db.Users.SingleOrDefault(x => x.Id == user.Id);

            if (user == null)
                return HttpNotFound();

            userInDb.UserName = user.UserName;
            userInDb.Password = user.Password;
            userInDb.RoleId = user.RoleId;

            db.SaveChanges();

            return RedirectToAction("Index","User");
        }

        [CustomAuthorize(Role.SuperAdmin)]
        public ActionResult Delete(int id)
        {
            User user = db.Users.SingleOrDefault(x => x.Id == id);

            if (user == null)
                return HttpNotFound();

            db.Users.Remove(user);

            db.SaveChanges();

            return RedirectToAction("index","User");
        }
    }
}