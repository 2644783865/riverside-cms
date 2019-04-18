using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Riverside.Cms.Applications.Web.Mvc.Services;
using Riverside.Cms.Services.Auth.Domain;
using Riverside.Cms.Services.Auth.Infrastructure;
using Riverside.Cms.Services.Core.Domain;
using Riverside.Cms.Services.Core.Infrastructure;
using Riverside.Cms.Services.Element.Domain;
using Riverside.Cms.Services.Element.Infrastructure;
using Riverside.Cms.Services.Mortgage.Domain;
using Riverside.Cms.Services.Mortgage.Infrastructure;
using Riverside.Cms.Services.Storage.Domain;
using Riverside.Cms.Services.Storage.Infrastructure;
using Riverside.Cms.Utilities.Net.Mail;
using Riverside.Cms.Utilities.Security.Encryption;
using Riverside.Cms.Utilities.Text.Csv;
using Riverside.Cms.Utilities.Text.Formatting;
using Riverside.Cms.Utilities.UI.Forms;
using Riverside.Cms.Utilities.Validation.DataAnnotations;

namespace Riverside.Cms.Applications.Web.Mvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private void ConfigureDependencyInjectionUiServices(IServiceCollection services)
        {
            services.AddTransient<IFormService, FormService>();
        }

        private void ConfigureDependencyInjectionSharedServices(IServiceCollection services)
        {
            services.AddTransient<ICsvService, CsvService>();
            services.AddTransient<IEmailService, SmtpEmailService>();
            services.AddTransient<IModelValidator, ModelValidator>();
            services.AddTransient<IStringUtilities, StringUtilities>();
        }

        private void ConfigureDependencyInjectionMvcServices(IServiceCollection services)
        {
            services.AddTransient<IElementServiceFactory, ElementServiceFactory>();
            services.AddTransient<ISeoService, SeoService>();
        }

        private void ConfigureDependencyInjectionAuthenticationServices(IServiceCollection services)
        {
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IAuthenticationValidator, AuthenticationValidator>();
            services.AddTransient<IAuthenticationConfigurationService, AuthenticationConfigurationService>();
            services.AddTransient<IAuthenticationRepository, SqlAuthenticationRepository>();
            services.AddTransient<IEncryptionService, EncryptionService>();
            services.AddTransient<ISecurityTokenService, JwtSecurityTokenService>();
        }

        private void ConfigureDependencyInjectionCoreServices(IServiceCollection services)
        {
            // Core domain services
            services.AddTransient<IDomainService, DomainService>();
            services.AddTransient<IElementService, ElementService>();
            services.AddTransient<IForumService, ForumService>();
            services.AddTransient<IMasterPageService, MasterPageService>();
            services.AddTransient<IPageService, PageService>();
            services.AddTransient<IPageValidator, PageValidator>();
            services.AddTransient<IPageViewService, PageViewService>();
            services.AddTransient<ITagService, TagService>();
            services.AddTransient<ITenantService, TenantService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IWebService, WebService>();
            services.AddTransient<IWebValidator, WebValidator>();

            // Core infrastructure services
            services.AddTransient<IDomainRepository, SqlDomainRepository>();
            services.AddTransient<IElementRepository, SqlElementRepository>();
            services.AddTransient<IForumRepository, SqlForumRepository>();
            services.AddTransient<IMasterPageRepository, SqlMasterPageRepository>();
            services.AddTransient<IPageRepository, SqlPageRepository>();
            services.AddTransient<ITagRepository, SqlTagRepository>();
            services.AddTransient<ITenantRepository, SqlTenantRepository>();
            services.AddTransient<IUserRepository, SqlUserRepository>();
            services.AddTransient<IWebRepository, SqlWebRepository>();
        }

        private void ConfigureDependencyInjectionElementServices(IServiceCollection services)
        {
            // Element domain services
            services.AddTransient<IAlbumElementService, AlbumElementService>();
            services.AddTransient<ICarouselElementService, CarouselElementService>();
            services.AddTransient<ICodeSnippetElementService, CodeSnippetElementService>();
            services.AddTransient<IFooterElementService, FooterElementService>();
            services.AddTransient<IFormElementService, FormElementService>();
            services.AddTransient<IForumElementService, ForumElementService>();
            services.AddTransient<IHtmlElementService, HtmlElementService>();
            services.AddTransient<ILatestThreadsElementService, LatestThreadsElementService>();
            services.AddTransient<INavigationBarElementService, NavigationBarElementService>();
            services.AddTransient<IPageHeaderElementService, PageHeaderElementService>();
            services.AddTransient<IPageListElementService, PageListElementService>();
            services.AddTransient<IShareElementService, ShareElementService>();
            services.AddTransient<ISocialBarElementService, SocialBarElementService>();
            services.AddTransient<ITableElementService, TableElementService>();
            services.AddTransient<ITagCloudElementService, TagCloudElementService>();
            services.AddTransient<ITestimonialElementService, TestimonialElementService>();

            // Element infrastructure services
            services.AddTransient<IElementRepository<AlbumElementSettings>, SqlAlbumElementRepository>();
            services.AddTransient<IElementRepository<CarouselElementSettings>, SqlCarouselElementRepository>();
            services.AddTransient<IElementRepository<CodeSnippetElementSettings>, SqlCodeSnippetElementRepository>();
            services.AddTransient<IElementRepository<FooterElementSettings>, SqlFooterElementRepository>();
            services.AddTransient<IElementRepository<FormElementSettings>, SqlFormElementRepository>();
            services.AddTransient<IElementRepository<ForumElementSettings>, SqlForumElementRepository>();
            services.AddTransient<IElementRepository<HtmlElementSettings>, SqlHtmlElementRepository>();
            services.AddTransient<IElementRepository<LatestThreadsElementSettings>, SqlLatestThreadsElementRepository>();
            services.AddTransient<IElementRepository<NavigationBarElementSettings>, SqlNavigationBarElementRepository>();
            services.AddTransient<IElementRepository<PageHeaderElementSettings>, SqlPageHeaderElementRepository>();
            services.AddTransient<IElementRepository<PageListElementSettings>, SqlPageListElementRepository>();
            services.AddTransient<IElementRepository<ShareElementSettings>, SqlShareElementRepository>();
            services.AddTransient<IElementRepository<SocialBarElementSettings>, SqlSocialBarElementRepository>();
            services.AddTransient<IElementRepository<TableElementSettings>, SqlTableElementRepository>();
            services.AddTransient<IElementRepository<TagCloudElementSettings>, SqlTagCloudElementRepository>();
            services.AddTransient<IElementRepository<TestimonialElementSettings>, SqlTestimonialElementRepository>();

            // Mortgage domain services
            services.AddTransient<IAmortisationCalculatorElementService, AmortisationCalculatorElementService>();
            services.AddTransient<IBorrowCalculatorElementService, BorrowCalculatorElementService>();
            services.AddTransient<IPayCalculatorElementService, PayCalculatorElementService>();
            services.AddTransient<IRentalCalculatorElementService, RentalCalculatorElementService>();
            services.AddTransient<IStampDutyCalculatorElementService, StampDutyCalculatorElementService>();

            // Mortgage infrastructure services
            services.AddTransient<IElementRepository<AmortisationCalculatorElementSettings>, SqlAmortisationCalculatorElementRepository>();
            services.AddTransient<IElementRepository<BorrowCalculatorElementSettings>, SqlBorrowCalculatorElementRepository>();
            services.AddTransient<IElementRepository<PayCalculatorElementSettings>, SqlPayCalculatorElementRepository>();
            services.AddTransient<IElementRepository<RentalCalculatorElementSettings>, SqlRentalCalculatorElementRepository>();
            services.AddTransient<IElementRepository<StampDutyCalculatorElementSettings>, SqlStampDutyCalculatorElementRepository>();
        }

        private void ConfigureDependencyInjectionStorageServices(IServiceCollection services)
        {
            // Storage domain services
            services.AddTransient<IStorageService, StorageService>();

            // Storage infrastructure services
            services.AddTransient<IBlobService, AzureBlobService>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IStorageRepository, SqlStorageRepository>();
        }

        private void ConfigureOptionServices(IServiceCollection services)
        {
            services.Configure<Cms.Services.Auth.Infrastructure.SqlOptions>(Configuration);
            services.Configure<Cms.Services.Core.Infrastructure.SqlOptions>(Configuration);
            services.Configure<Cms.Services.Element.Infrastructure.SqlOptions>(Configuration);
            services.Configure<Cms.Services.Mortgage.Infrastructure.SqlOptions>(Configuration);
            services.Configure<Cms.Services.Storage.Infrastructure.SqlOptions>(Configuration);
            services.Configure<AzureBlobOptions>(Configuration);
            services.Configure<EmailOptions>(Configuration);
            services.Configure<JwtOptions>(Configuration);
        }

        private void ConfigureAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKeyResolver = (string token, SecurityToken securityToken, string kid, TokenValidationParameters validationParameters) =>
                    {
                        ISecurityTokenService tokenService = services.BuildServiceProvider().GetService<ISecurityTokenService>();
                        long tenantId = Convert.ToInt64(kid);
                        byte[] securityKey = tokenService.GetTenantSecurityKey(tenantId);
                        return new List<SecurityKey> { new SymmetricSecurityKey(securityKey) };
                    }
                };
            });
        }

        private void ConfigureAuthorisation(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("CreatePages", policy => { policy.RequireRole("Editor", "EditorInChief", "Administrator"); });
                options.AddPolicy("UpdatePages", policy => { policy.RequireRole("Editor", "EditorInChief", "Administrator"); });
                options.AddPolicy("DeletePages", policy => { policy.RequireRole("Editor", "EditorInChief", "Administrator"); });
                options.AddPolicy("UpdatePageElements", policy => { policy.RequireRole("Editor", "EditorInChief", "Administrator"); });
                options.AddPolicy("CreateMasterPages", policy => { policy.RequireRole("Administrator"); });
                options.AddPolicy("UpdateMasterPages", policy => { policy.RequireRole("Administrator"); });
                options.AddPolicy("DeleteMasterPages", policy => { policy.RequireRole("Administrator"); });
                options.AddPolicy("UpdateMasterPageElements", policy => { policy.RequireRole("EditorInChief", "Administrator"); });
                options.AddPolicy("UpdateWeb", policy => { policy.RequireRole("Administrator"); });
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc(options => { options.EnableEndpointRouting = false; }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddHttpContextAccessor();

            ConfigureDependencyInjectionUiServices(services);
            ConfigureDependencyInjectionSharedServices(services);
            ConfigureDependencyInjectionMvcServices(services);
            ConfigureDependencyInjectionAuthenticationServices(services);
            ConfigureDependencyInjectionCoreServices(services);
            ConfigureDependencyInjectionElementServices(services);
            ConfigureDependencyInjectionStorageServices(services);
            ConfigureOptionServices(services);

            ConfigureAuthentication(services);
            ConfigureAuthorisation(services);
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
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                // General CMS
                routes.MapRoute(RouteNames.Home, "", new { controller = "cms", action = "readhomeasync" });
                routes.MapRoute(RouteNames.HomeTagged, "tagged/{*tags}", new { controller = "cms", action = "readhometaggedasync" });
                routes.MapRoute(RouteNames.PageImage, "pages/{pageId}/images/{pageImageType}/{*description}", new { controller = "cms", action = "readpageimageasync" });
                routes.MapRoute(RouteNames.ElementBlobContent, "elementtypes/{elementTypeId}/elements/{elementId}/blobsets/{blobSetId}/content", new { controller = "cms", action = "readelementblobasync" });
                routes.MapRoute(RouteNames.ElementAction, "elementtypes/{elementTypeId}/elements/{elementId}/action", new { controller = "cms", action = "performelementactionasync" });
                routes.MapRoute(RouteNames.PageTagged, "pages/{pageId}/{description}/tagged/{*tags}", new { controller = "cms", action = "readpagetaggedasync" });
                routes.MapRoute(RouteNames.Page, "pages/{pageId}/{*description}", new { controller = "cms", action = "readpageasync" });
                routes.MapRoute(RouteNames.UserImage, "users/{userId}/images/{userImageType}/{*description}", new { controller = "cms", action = "readuserblobasync" });
                routes.MapRoute(RouteNames.Robots, "robots.txt", new { controller = "cms", action = "robotsasync" });
                routes.MapRoute(RouteNames.Sitemap, "sitemap.xml", new { controller = "cms", action = "sitemapasync" });

                // Administration
                routes.MapRoute("AdminHome", "admin", new { controller = "admin", action = "home" });
                routes.MapRoute("AdminLogin", "admin/login", new { controller = "admin", action = "login" });
                routes.MapRoute("AdminPageElementRead", "admin/pages/{pageId}/elementtypes/{elementTypeId}/elements/{elementId}", new { controller = "admin", action = "readpageelement" });
                routes.MapRoute("AdminPageElementUpdate", "admin/pages/{pageId}/elementtypes/{elementTypeId}/elements/{elementId}/update", new { controller = "admin", action = "updatepageelement" });
                routes.MapRoute("AdminPageRead", "admin/pages/{pageId}", new { controller = "admin", action = "readpage" });
                routes.MapRoute("AdminPageUpdate", "admin/pages/{pageId}/update", new { controller = "admin", action = "updatepage" });
                routes.MapRoute("AdminWebRead", "admin/web", new { controller = "admin", action = "readweb" });
                routes.MapRoute("AdminWebUpdate", "admin/web/update", new { controller = "admin", action = "updateweb" });

                // Default
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
