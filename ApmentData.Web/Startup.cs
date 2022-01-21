using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ApmentData.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc();
            //services.AddControllers().AddControllersAsServices();
            //services.AddRazorPages();
            services.AddMvc();

            var settings = new ConnectionSettings(new Uri("https://search-apmentdata-hcbcntnroztezesswhs6abkbzi.us-east-1.es.amazonaws.com/"));
            services.AddSingleton<IElasticClient>(new ElasticClient(settings));

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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();


            app.UseRouting();

            app.UseAuthorization();

            //app.UseEndpoints(endpoints => endpoints.MapRazorPages());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters*
                new
                {
                    controller = "Search",
                    action = "Index",
                    id = UrlParameter.Optional
                });
            });
        }


    }
}
