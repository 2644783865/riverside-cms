﻿using System;
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
using Riverside.Cms.Services.Storage.Domain;
using Swashbuckle.AspNetCore.Swagger;

namespace Core.API
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
            services.AddTransient<IStorageService, StorageService>();

            services.AddTransient<IDomainService, DomainService>();
            services.AddTransient<IForumService, ForumService>();
            services.AddTransient<IMasterPageService, MasterPageService>();
            services.AddTransient<IPageService, PageService>();
            services.AddTransient<IPageViewService, PageViewService>();
            services.AddTransient<ITagService, TagService>();
            services.AddTransient<IUserService, UserService>();

            services.AddTransient<IDomainRepository, SqlDomainRepository>();
            services.AddTransient<IForumRepository, SqlForumRepository>();
            services.AddTransient<IMasterPageRepository, SqlMasterPageRepository>();
            services.AddTransient<IPageRepository, SqlPageRepository>();
            services.AddTransient<ITagRepository, SqlTagRepository>();
            services.AddTransient<IUserRepository, SqlUserRepository>();
        }

        private void ConfigureOptionServices(IServiceCollection services)
        {
            services.Configure<SqlOptions>(Configuration);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Core HTTP API", Version = "v1" });
            });

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
                  c.SwaggerEndpoint("/swagger/v1/swagger.json", "Core HTTP API v1");
              });
        }
    }
}
