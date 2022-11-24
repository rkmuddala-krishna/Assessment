using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using DSO.ServiceLibrary;
using DSO.OTP.API.DB;
using Microsoft.EntityFrameworkCore;
using DSO.OTP.API.Infrastructure;
using DSO.OTP.API.Services;
using DSO.OTP.API.Services.Interface;
using OtpNet;
namespace DSO.OTP.API
{
    public class Startup
    {

        protected string Rootpath { get; set; }
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Rootpath = env.ContentRootPath;
            var builder = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    //.SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                    .AddEnvironmentVariables();

            Configuration = builder.Build();

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var sqlLiteConnection = Configuration.GetValue<string>("ConnectionStrings:sqlLiteConnection");
            var DbPath = @$"{Rootpath}{sqlLiteConnection}";
            Console.WriteLine($"DB path:{DbPath}");
            services.AddSingleton<IConfiguration>(Configuration);
            Console.WriteLine($"DbPath : {DbPath}");
            services.AddDbContext<OTPDBContext>(options =>
                options.UseSqlite($"Data Source={DbPath}"));
            services.AddControllers();

            services
                            .AddScoped<IUnitOfWork, UnitOfWork>()
                            .AddScoped(typeof(IRepository<>), typeof(Repository<>))
                            .AddScoped<IOTPDBService, OTPDBService>();

            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IOTPService, OTPService>();
           
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DSO.OTP.API", Version = "v1" });
            });
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DSO.OTP.API v1"));
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();


            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
                                   {
                                       endpoints.MapRazorPages();
                                   });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            loggerFactory.AddFile("Logs/OTPGenerator-{Date}.txt");

        }
    }
}
