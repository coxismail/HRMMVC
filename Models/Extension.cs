using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace HRMMVC.Models
{
    public static class IdentityExtensions
    {
        public static string DisplayName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("DisplayName");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string ProfilePicture(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("ProfilePicture");
            return (claim != null) ? claim.Value : "~/Assets/images/person.jpg";
        }

        public static string GetPhoneNumber(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("PhoneNumber");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static bool IsApproved(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("IsApproved");
            return (claim != null) ? Convert.ToBoolean(claim.Value) : false;
        }
        public static bool IsMember(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("IsMember");
            return (claim != null) ? Convert.ToBoolean(claim.Value) : false;
        }
        public static TimeZoneInfo TimeZone(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("TimeZoneId")?.Value ?? TimeZoneInfo.Local.Id;
            return TimeZoneInfo.FindSystemTimeZoneById(claim) ?? TimeZoneInfo.Local;


        }
    }


    public class AjaxOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
                filterContext.HttpContext.Response.Redirect("/directAccessNotAllowed");
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }
    }
}