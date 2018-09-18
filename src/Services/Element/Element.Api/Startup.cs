using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Riverside.Cms.Services.Core.Domain;
using Riverside.Cms.Services.Core.Infrastructure;
using Riverside.Cms.Services.Element.Domain;
using Riverside.Cms.Services.Element.Infrastructure;
using Riverside.Cms.Services.Storage.Domain;
using Riverside.Cms.Services.Storage.Infrastructure;
using Riverside.Cms.Utilities.Text.Csv;
using Riverside.Cms.Utilities.Text.Formatting;
using Swashbuckle.AspNetCore.Swagger;

namespace Element.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private void ConfigureDependencyInjectionStorageServices(IServiceCollection services)
        {
            // Storage domain services
            services.AddTransient<IStorageService, StorageService>();

            // Storage infrastructure services
            services.AddTransient<IBlobService, AzureBlobService>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IStorageRepository, SqlStorageRepository>();
        }

        private void ConfigureDependencyInjectionCoreServices(IServiceCollection services)
        {
            // Core domain services
            services.AddTransient<IDomainService, DomainService>();
            services.AddTransient<IForumService, ForumService>();
            services.AddTransient<IMasterPageService, MasterPageService>();
            services.AddTransient<IPageService, PageService>();
            services.AddTransient<IPageViewService, PageViewService>();
            services.AddTransient<ITagService, TagService>();
            services.AddTransient<IUserService, UserService>();

            // Core infrastructure services
            services.AddTransient<IDomainRepository, SqlDomainRepository>();
            services.AddTransient<IForumRepository, SqlForumRepository>();
            services.AddTransient<IMasterPageRepository, SqlMasterPageRepository>();
            services.AddTransient<IPageRepository, SqlPageRepository>();
            services.AddTransient<ITagRepository, SqlTagRepository>();
            services.AddTransient<IUserRepository, SqlUserRepository>();
        }

        private void ConfigureDependencyInjectionServices(IServiceCollection services)
        {
            services.AddTransient<ICsvService, CsvService>();
            services.AddTransient<IStringUtilities, StringUtilities>();

            services.AddTransient<IForumService, ForumService>();
            services.AddTransient<IPageService, PageService>();
            services.AddTransient<ITagService, TagService>();
            services.AddTransient<IUserService, UserService>();

            services.AddTransient<IAlbumElementService, AlbumElementService>();
            services.AddTransient<ICarouselElementService, CarouselElementService>();
            services.AddTransient<ICodeSnippetElementService, CodeSnippetElementService>();
            services.AddTransient<IFooterElementService, FooterElementService>();
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

            services.AddTransient<IElementRepository<AlbumElementSettings>, SqlAlbumElementRepository>();
            services.AddTransient<IElementRepository<CarouselElementSettings>, SqlCarouselElementRepository>();
            services.AddTransient<IElementRepository<CodeSnippetElementSettings>, SqlCodeSnippetElementRepository>();
            services.AddTransient<IElementRepository<FooterElementSettings>, SqlFooterElementRepository>();
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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Element HTTP API", Version = "v1" });
            });

            ConfigureDependencyInjectionStorageServices(services);
            ConfigureDependencyInjectionCoreServices(services);
            ConfigureDependencyInjectionServices(services);
            ConfigureOptionServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseSwagger()
              .UseSwaggerUI(c =>
              {
                  c.SwaggerEndpoint("/swagger/v1/swagger.json", "Element HTTP API v1");
              });
        }
    }
}
