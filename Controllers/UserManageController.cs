using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using HRMMVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HRMMVC.Controllers
{
    [RoutePrefix("UserManage")]
    public class UserManageController : Controller
    {
        private  ApplicationDbContext _context = new ApplicationDbContext();

        [Route(""), AjaxOnly]
        public ActionResult Index()
        {
            var me = User.Identity.Name;
            var user = _context.Users.Where(s => s.UserName != "coxismail.bd@gmail.com" && s.UserName != me).ToList();
            if (User.IsInRole("System Admin"))
            {
                user = _context.Users.Where(s => s.UserName != me).ToList();
            }
            return PartialView(user);
        }
        [Route("Permission"), AjaxOnly]
        public ActionResult Permission(string id)
        {
            var user = _context.Users.Where(s => s.Id == id).SingleOrDefault();
            var roles = user.Roles.Select(f => f.RoleId).ToList();
            var mroles = _context.Roles.Where(s => s.Name != "System Admin").ToList();
            if (User.IsInRole("Permission Manager"))
            {
                mroles = _context.Roles.Where(s => s.Name != "System Admin" && s.Name != "Permission Manager").ToList();
            }

            List<PermissionViewmOdel> pmv = new List<PermissionViewmOdel>();
            foreach (var item in mroles)
            {
                var pm = new PermissionViewmOdel();
                pm.RoleName = item.Name;
                pm.RoleId = item.Id;
                pm.IsSelected = false;
                if (roles.Contains(item.Id))
                {
                    pm.IsSelected = true;
                }
                pmv.Add(pm);
            }
            ViewData["user"] = user;
            return PartialView(pmv);
        }
        [HttpPost, ValidateAntiForgeryToken, Route("Permission"), AjaxOnly]
        public async Task<ActionResult> Permission(string id, string[] RoleId)
        {
            var _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_context));
            var jsonResult = new { Status  = "Faild", Message = "Something went wrong", url = "" };
            var remove = "";
            var assign = "";

            var user = _context.Users.Where(s => s.Id == id).SingleOrDefault();
            var RemoveAbleAlreadyAssign = user.Roles.ToList().Select(f => f.RoleId).ToList();
            if (RoleId !=null)
            {
                RemoveAbleAlreadyAssign = user.Roles.Where(s => !RoleId.Contains(s.RoleId)).ToList().Select(f => f.RoleId).ToList();
            }
                  
            var alrRole = _context.Roles.Where(s => RemoveAbleAlreadyAssign.Contains(s.Id)).ToList();



            foreach (var item in alrRole)
            { // Remove Role
                await _userManager.RemoveFromRoleAsync(user.Id, item.Name);
                remove += item.Name + ", ";
            }

            if (RoleId != null)
            {
                var skipable = user.Roles.Where(s => RoleId.Contains(s.RoleId)).Select(f => f.RoleId).ToList();
                var NewRole = _context.Roles.Where(s => RoleId.Contains(s.Id) && !skipable.Contains(s.Id)).ToList();

                foreach (var item in NewRole)
                {
                    await _userManager.AddToRoleAsync(user.Id, item.Name);
                    assign += item.Name + ", ";
                }

            }
            string message = "";
            if (remove.Length > 1)
            {
                message += "Remove from : " + remove;
            }
            if (assign.Length > 1)
            {
                message +=  "& Assign to : " + assign + " Roles";
            }
            jsonResult = new { Status = "OK", Message = message, url = "/UserManage/" };

            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }
        [Route("Lock"), AjaxOnly]
        public async Task<ActionResult> Lock(string id)
        {
            ApplicationUser user = _context.Users.Where(s => s.Id == id).SingleOrDefault();
            if (user.Lock == false)
            {
                user.Lock = true;
                _context.Entry(user).State = System.Data.Entity.EntityState.Modified ;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        [Route("UnLock"), AjaxOnly]
        public async Task<ActionResult> UnLock(string id)
        {
            ApplicationUser user = _context.Users.Where(s => s.Id == id).SingleOrDefault();
            if (user.Lock == true)
            {
                user.Lock = false;
                _context.Entry(user).State = System.Data.Entity.EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}