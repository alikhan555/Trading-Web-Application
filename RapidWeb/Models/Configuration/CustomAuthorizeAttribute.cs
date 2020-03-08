using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using RapidWeb.Models;

namespace RapidWeb.Models
{
    public class CustomAuthorizeAttribute : ActionFilterAttribute
    {
        int[] _authorizedRoles;
        private User _user;
        
        public CustomAuthorizeAttribute(params int[] authorizedRoles)
        {
            this._authorizedRoles = authorizedRoles;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _user = ((User) HttpContext.Current.Session["User"]);

            if (_user == null)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary
                        {
                            {"Controller", "User"},
                            {"Action", "Login"}
                        });
                }
            }
            else if (!IsAuthorized(_user))
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }

            base.OnActionExecuting(filterContext);
        }

        private bool IsAuthorized(User user)
        {
            return _authorizedRoles.Contains(user.RoleId);
        }
    }
}