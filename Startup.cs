using HRMMVC.Manager;
using HRMMVC.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System;
using System.Data.SqlClient;
using System.Web.Configuration;

[assembly: OwinStartupAttribute(typeof(HRMMVC.Startup))]
namespace HRMMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //ExtraFunctions.SetDefaultAccounting();
            //ExtraFunctions.CreateDefults();
        }

      
    }


}
