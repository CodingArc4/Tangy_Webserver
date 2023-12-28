using Microsoft.EntityFrameworkCore;
using Tangy_Busines.Repository;
using Tangy_Busines.Repository.IRepository;
using Tangy_DataAccess.Data;
using Tangy_Webserver.Data;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Tangy_Webserver.Serivce.IService;
using TangyWeb_Server.Service;
using Tangy_DataAccess;
using System.Composition.Hosting.Core;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Tangy_Webserver.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
//using TangyWeb_Client.Service;

namespace Tangy_Webserver
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            //builder.Services.AddSyncfusionBlazor();
            
            builder.Services.AddServerSideBlazor();
            builder.Services.AddSingleton<WeatherForecastService>();
            
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddDefaultTokenProviders().AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddAuthorizationCore();
            
            builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}