using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Tax_Reminder.DbContext;

namespace Tax_Reminder.App_Start
{
    public class Admin : AuthorizeAttribute
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var authorized = base.AuthorizeCore(httpContext);
            if (!authorized)
            {
                // The user is not authorized => no need to go any further
                return false;
            }

            // We have an authenticated user, let's get his username
            string userId = httpContext.User.Identity.GetUserId();
            var user = _db.Users.FirstOrDefault(x => x.Id == userId);
            if (user != null && user.Email == "nafeeur.kuet.cse6@gmail.com")
            {
                return true;
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Items.Contains("redirectToCompleteProfile"))
            {
                var routeValues = new RouteValueDictionary(new
                {
                    controller = "someController",
                    action = "someAction",
                });
                filterContext.Result = new RedirectToRouteResult(routeValues);
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }

    }
}