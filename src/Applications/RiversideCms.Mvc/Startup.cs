using System;
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
using Riverside.Cms.Services.Storage.Infrastructure;
using Riverside.Cms.Utilities.Text.Csv;
using Riverside.Cms.Utilities.Text.Formatting;
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

        private void ConfigureDependencyInjectionSharedServices(IServiceCollection services)
        {
            // Utilities
            services.AddTransient<ICsvService, CsvService>();
            services.AddTransient<IStringUtilities, StringUtilities>();
        }

        private void ConfigureDependencyInjectionClientServices(IServiceCollection services)
        {
            // Core services
            services.AddTransient<Riverside.Cms.Services.Core.Client.IDomainService, Riverside.Cms.Services.Core.Client.DomainService>();
            services.AddTransient<Riverside.Cms.Services.Core.Client.IPageService, Riverside.Cms.Services.Core.Client.PageService>();
            services.AddTransient<Riverside.Cms.Services.Core.Client.IPageViewService, Riverside.Cms.Services.Core.Client.PageViewService>();
            services.AddTransient<Riverside.Cms.Services.Core.Client.ITagService, Riverside.Cms.Services.Core.Client.TagService>();
            services.AddTransient<Riverside.Cms.Services.Core.Client.IUserService, Riverside.Cms.Services.Core.Client.UserService>();

            // Element services
            services.AddTransient<Riverside.Cms.Services.Element.Client.IAlbumElementService, Riverside.Cms.Services.Element.Client.AlbumElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Client.ICarouselElementService, Riverside.Cms.Services.Element.Client.CarouselElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Client.ICodeSnippetElementService, Riverside.Cms.Services.Element.Client.CodeSnippetElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Client.IFooterElementService, Riverside.Cms.Services.Element.Client.FooterElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Client.IHtmlElementService, Riverside.Cms.Services.Element.Client.HtmlElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Client.ILatestThreadsElementService, Riverside.Cms.Services.Element.Client.LatestThreadsElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Client.INavigationBarElementService, Riverside.Cms.Services.Element.Client.NavigationBarElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Client.IPageHeaderElementService, Riverside.Cms.Services.Element.Client.PageHeaderElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Client.IPageListElementService, Riverside.Cms.Services.Element.Client.PageListElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Client.IShareElementService, Riverside.Cms.Services.Element.Client.ShareElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Client.ISocialBarElementService, Riverside.Cms.Services.Element.Client.SocialBarElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Client.ITableElementService, Riverside.Cms.Services.Element.Client.TableElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Client.ITagCloudElementService, Riverside.Cms.Services.Element.Client.TagCloudElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Client.ITestimonialElementService, Riverside.Cms.Services.Element.Client.TestimonialElementService>();

            // Element service factory
            services.AddTransient<IElementServiceFactory, ElementServiceFactory>();
        }

        private void ConfigureDependencyInjectionCoreServices(IServiceCollection services)
        {
            // Core domain services
            services.AddTransient<Riverside.Cms.Services.Core.Domain.IDomainService, Riverside.Cms.Services.Core.Domain.DomainService>();
            services.AddTransient<Riverside.Cms.Services.Core.Domain.IForumService, Riverside.Cms.Services.Core.Domain.ForumService>();
            services.AddTransient<Riverside.Cms.Services.Core.Domain.IMasterPageService, Riverside.Cms.Services.Core.Domain.MasterPageService>();
            services.AddTransient<Riverside.Cms.Services.Core.Domain.IPageService, Riverside.Cms.Services.Core.Domain.PageService>();
            services.AddTransient<Riverside.Cms.Services.Core.Domain.IPageViewService, Riverside.Cms.Services.Core.Domain.PageViewService>();
            services.AddTransient<Riverside.Cms.Services.Core.Domain.ITagService, Riverside.Cms.Services.Core.Domain.TagService>();
            services.AddTransient<Riverside.Cms.Services.Core.Domain.IUserService, Riverside.Cms.Services.Core.Domain.UserService>();

            // Core infrastructure services
            services.AddTransient<Riverside.Cms.Services.Core.Domain.IDomainRepository, Riverside.Cms.Services.Core.Infrastructure.SqlDomainRepository>();
            services.AddTransient<Riverside.Cms.Services.Core.Domain.IForumRepository, Riverside.Cms.Services.Core.Infrastructure.SqlForumRepository>();
            services.AddTransient<Riverside.Cms.Services.Core.Domain.IMasterPageRepository, Riverside.Cms.Services.Core.Infrastructure.SqlMasterPageRepository>();
            services.AddTransient<Riverside.Cms.Services.Core.Domain.IPageRepository, Riverside.Cms.Services.Core.Infrastructure.SqlPageRepository>();
            services.AddTransient<Riverside.Cms.Services.Core.Domain.ITagRepository, Riverside.Cms.Services.Core.Infrastructure.SqlTagRepository>();
            services.AddTransient<Riverside.Cms.Services.Core.Domain.IUserRepository, Riverside.Cms.Services.Core.Infrastructure.SqlUserRepository>();
        }

        private void ConfigureDependencyInjectionElementServices(IServiceCollection services)
        {
            // Element domain services
            services.AddTransient<Riverside.Cms.Services.Element.Domain.IAlbumElementService, Riverside.Cms.Services.Element.Domain.AlbumElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.ICarouselElementService, Riverside.Cms.Services.Element.Domain.CarouselElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.ICodeSnippetElementService, Riverside.Cms.Services.Element.Domain.CodeSnippetElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.IFooterElementService, Riverside.Cms.Services.Element.Domain.FooterElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.IHtmlElementService, Riverside.Cms.Services.Element.Domain.HtmlElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.ILatestThreadsElementService, Riverside.Cms.Services.Element.Domain.LatestThreadsElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.INavigationBarElementService, Riverside.Cms.Services.Element.Domain.NavigationBarElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.IPageHeaderElementService, Riverside.Cms.Services.Element.Domain.PageHeaderElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.IPageListElementService, Riverside.Cms.Services.Element.Domain.PageListElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.IShareElementService, Riverside.Cms.Services.Element.Domain.ShareElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.ISocialBarElementService, Riverside.Cms.Services.Element.Domain.SocialBarElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.ITableElementService, Riverside.Cms.Services.Element.Domain.TableElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.ITagCloudElementService, Riverside.Cms.Services.Element.Domain.TagCloudElementService>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.ITestimonialElementService, Riverside.Cms.Services.Element.Domain.TestimonialElementService>();

            // Element infrastructure services
            services.AddTransient<Riverside.Cms.Services.Element.Domain.IElementRepository<Riverside.Cms.Services.Element.Domain.AlbumElementSettings>, Riverside.Cms.Services.Element.Infrastructure.SqlAlbumElementRepository>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.IElementRepository<Riverside.Cms.Services.Element.Domain.CarouselElementSettings>, Riverside.Cms.Services.Element.Infrastructure.SqlCarouselElementRepository>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.IElementRepository<Riverside.Cms.Services.Element.Domain.CodeSnippetElementSettings>, Riverside.Cms.Services.Element.Infrastructure.SqlCodeSnippetElementRepository>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.IElementRepository<Riverside.Cms.Services.Element.Domain.FooterElementSettings>, Riverside.Cms.Services.Element.Infrastructure.SqlFooterElementRepository>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.IElementRepository<Riverside.Cms.Services.Element.Domain.HtmlElementSettings>, Riverside.Cms.Services.Element.Infrastructure.SqlHtmlElementRepository>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.IElementRepository<Riverside.Cms.Services.Element.Domain.LatestThreadsElementSettings>, Riverside.Cms.Services.Element.Infrastructure.SqlLatestThreadsElementRepository>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.IElementRepository<Riverside.Cms.Services.Element.Domain.NavigationBarElementSettings>, Riverside.Cms.Services.Element.Infrastructure.SqlNavigationBarElementRepository>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.IElementRepository<Riverside.Cms.Services.Element.Domain.PageHeaderElementSettings>, Riverside.Cms.Services.Element.Infrastructure.SqlPageHeaderElementRepository>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.IElementRepository<Riverside.Cms.Services.Element.Domain.PageListElementSettings>, Riverside.Cms.Services.Element.Infrastructure.SqlPageListElementRepository>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.IElementRepository<Riverside.Cms.Services.Element.Domain.ShareElementSettings>, Riverside.Cms.Services.Element.Infrastructure.SqlShareElementRepository>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.IElementRepository<Riverside.Cms.Services.Element.Domain.SocialBarElementSettings>, Riverside.Cms.Services.Element.Infrastructure.SqlSocialBarElementRepository>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.IElementRepository<Riverside.Cms.Services.Element.Domain.TableElementSettings>, Riverside.Cms.Services.Element.Infrastructure.SqlTableElementRepository>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.IElementRepository<Riverside.Cms.Services.Element.Domain.TagCloudElementSettings>, Riverside.Cms.Services.Element.Infrastructure.SqlTagCloudElementRepository>();
            services.AddTransient<Riverside.Cms.Services.Element.Domain.IElementRepository<Riverside.Cms.Services.Element.Domain.TestimonialElementSettings>, Riverside.Cms.Services.Element.Infrastructure.SqlTestimonialElementRepository>();
        }

        private void ConfigureDependencyInjectionStorageServices(IServiceCollection services)
        {
            // Storage domain services
            services.AddTransient<Riverside.Cms.Services.Storage.Domain.IStorageService, Riverside.Cms.Services.Storage.Domain.StorageService>();

            // Storage infrastructure services
            services.AddTransient<Riverside.Cms.Services.Storage.Domain.IBlobService, AzureBlobService>();
            services.AddTransient<Riverside.Cms.Services.Storage.Domain.IImageService, ImageService>();
            services.AddTransient<Riverside.Cms.Services.Storage.Domain.IStorageRepository, SqlStorageRepository>();
        }

        private void ConfigureOptionClientServices(IServiceCollection services)
        {
            services.Configure<Riverside.Cms.Services.Core.Client.CoreApiOptions>(Configuration);
            services.Configure<Riverside.Cms.Services.Element.Client.ElementApiOptions>(Configuration);
            services.Configure<Riverside.Cms.Services.Storage.Client.StorageApiOptions>(Configuration);
        }

        private void ConfigureOptionServices(IServiceCollection services)
        {
            services.Configure<Riverside.Cms.Services.Core.Infrastructure.SqlOptions>(Configuration);
            services.Configure<Riverside.Cms.Services.Element.Infrastructure.SqlOptions>(Configuration);
            services.Configure<Riverside.Cms.Services.Storage.Infrastructure.SqlOptions>(Configuration);
            services.Configure<AzureBlobOptions>(Configuration);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            ConfigureDependencyInjectionSharedServices(services);
            ConfigureDependencyInjectionClientServices(services);
            ConfigureDependencyInjectionCoreServices(services);
            ConfigureDependencyInjectionElementServices(services);
            ConfigureDependencyInjectionStorageServices(services);
            ConfigureOptionClientServices(services);
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
                routes.MapRoute("Home", "", new { controller = "cms", action = "home" });
                routes.MapRoute("HomeTagged", "tagged/{*tags}", new { controller = "cms", action = "hometagged" });
                routes.MapRoute("PageImage", "pages/{pageId}/images/{pageImageType}/{*description}", new { controller = "cms", action = "readpageimageasync" });
                routes.MapRoute(RouteNames.ElementBlobContent, "elementtypes/{elementTypeId}/elements/{elementId}/blobsets/{blobSetId}/content", new { controller = "cms", action = "readelementblobasync" });
                routes.MapRoute("PageTagged", "pages/{pageId}/{description}/tagged/{*tags}", new { controller = "cms", action = "readtaggedasync" });
                routes.MapRoute("Page", "pages/{pageId}/{*description}", new { controller = "cms", action = "readasync" });
                routes.MapRoute("UserImage", "users/{userId}/images/{userImageType}/{*description}", new { controller = "cms", action = "readuserblobasync" });
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
