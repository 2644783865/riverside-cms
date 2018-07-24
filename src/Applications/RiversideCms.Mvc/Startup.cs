﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Riverside.Cms.Services.Core.Client;
using Riverside.Cms.Services.Element.Client;
using Riverside.Cms.Services.Storage.Client;
using RiversideCms.Mvc.Services;

namespace RiversideCms.Mvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private void ConfigureDependencyInjectionServices(IServiceCollection services)
        {
            // Core services
            services.AddTransient<IPageService, PageService>();
            services.AddTransient<IPageViewService, PageViewService>();

            // Element services
            services.AddTransient<ICodeSnippetElementService, CodeSnippetElementService>();
            services.AddTransient<IFooterElementService, FooterElementService>();
            services.AddTransient<IHtmlElementService, HtmlElementService>();
            services.AddTransient<INavigationBarElementService, NavigationBarElementService>();
            services.AddTransient<IPageHeaderElementService, PageHeaderElementService>();
            services.AddTransient<IShareElementService, ShareElementService>();

            // Element factory
            services.AddTransient<IElementServiceFactory, ElementServiceFactory>();
        }

        private void ConfigureOptionServices(IServiceCollection services)
        {
            services.Configure<CoreApiOptions>(Configuration);
            services.Configure<ElementApiOptions>(Configuration);
            services.Configure<StorageApiOptions>(Configuration);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            ConfigureDependencyInjectionServices(services);
            ConfigureOptionServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute("Home", "", new { controller = "pages", action = "home" });
                routes.MapRoute("PageImage", "pages/{pageId}/images/{pageImageType}/{*description}", new { controller = "pages", action = "readimage" });
                routes.MapRoute("Page", "pages/{pageId}/{*description}", new { controller = "pages", action = "read" });
                routes.MapRoute("Login", "account/login", new { controller = "account", action = "login" });
                routes.MapRoute("Logout", "account/logout", new { controller = "account", action = "logout" });
                routes.MapRoute("Register", "account/register", new { controller = "account", action = "register" });
                routes.MapRoute("UpdateProfile", "account/updateprofile", new { controller = "account", action = "updateprofile" });
                routes.MapRoute("ChangePassword", "account/changepassword", new { controller = "account", action = "changepassword" });
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
