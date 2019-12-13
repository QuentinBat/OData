using System.Linq;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OData.Models;

namespace OData
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddOData();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            var builder = new ODataConventionModelBuilder(app.ApplicationServices);

            builder.EntitySet<Product>("Products");

            app.UseMvc(routeBuilder =>
            {
                routeBuilder.EnableDependencyInjection();

                // and this line to enable OData query option, for example $filter
                routeBuilder.Select().Expand().Filter().OrderBy().Count().MaxTop(null);

                routeBuilder.MapODataServiceRoute("ODataRoute", "odata", builder.GetEdmModel());
            });
        }
    }
}
