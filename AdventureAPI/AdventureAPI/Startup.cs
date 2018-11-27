using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AdventureAPI
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddMemoryCache();
            services.AddMvc().AddSessionStateTempDataProvider();
            services.AddSession(options =>
            {
                options.Cookie.Name = "username";
                options.Cookie.Name = "score";
                options.IdleTimeout = TimeSpan.FromHours(1);
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseCors("AllowAllOrigins");
            app.UseMvc();
        }
    }
}
