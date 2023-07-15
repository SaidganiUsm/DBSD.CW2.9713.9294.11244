using DBSD.CW2._9713._9294._11244.DAL;
using DBSD.CW2._9713._9294._11244.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DBSD.CW2._9713._9294._11244
{
    public class Startup
    {
        private const string DATA_DIRECTORY = @"C:\Users\Said\Desktop\DBSD.CW2.9713.9294.11244\DBSD.CW2.9713.9294.11244\AppData\DBSD.mdf";
        private string _dataDirectoryPath;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IEmployeeRepository, EmployeeRepositoryDapper>(
                    x=> new EmployeeRepositoryDapper(Configuration.GetConnectionString("DBSD")
                    .Replace(DATA_DIRECTORY, _dataDirectoryPath)));
            
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Employee}/{action=Index}/{id?}");
            });
        }
    }
}
