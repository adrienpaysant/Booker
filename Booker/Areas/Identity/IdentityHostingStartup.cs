using System;
using Booker.Areas.Identity.Data;
using Booker.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Booker.Areas.Identity.IdentityHostingStartup))]
namespace Booker.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<BookerContextId>(options =>
                    options.UseSqlite(
                        context.Configuration.GetConnectionString("BookerContextIdConnection")));

                services.AddDefaultIdentity<BookerUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<BookerContextId>();
            });
        }
    }
}