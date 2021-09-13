using IPGVolume.Api.Hubs;
using IPGVolume.Api.Models.Database;
using IPGVolume.Api.Workers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace IPGVolume.Api
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "IPGVolume.Api", Version = "v1" });
            });
            services.AddSignalR();
            services.AddCors();

            services.AddDbContext<IPGContext>(options =>
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                options.UseMySQL(Configuration.GetConnectionString("IPGContext"));
            });

            services.AddCors(options =>
            {
                options.AddPolicy(name: "Dev", builder =>
                {
                    builder.WithOrigins("http://localhost:4200", "http://localhost:4201", "http://localhost:7226");
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                    builder.AllowCredentials();
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy(name: "Prod", builder =>
                {
                    builder.WithOrigins("Add origins here");
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                    builder.AllowCredentials();
                });
            });

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ng-volume/dist";
            });

            services.AddHostedService<StartUpWorker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IPGVolume.Api v1"));
                app.UseCors("Dev");
            }

            if(env.IsProduction())
            {
                app.UseCors("Prod");
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<AudioHub>("/AudioHub");
            });


            if (!env.IsDevelopment())
            {
                app.UseSpa(spa =>
                {
                    // To learn more about options for serving an Angular SPA from ASP.NET Core,
                    // see https://go.microsoft.com/fwlink/?linkid=864501

                    spa.Options.SourcePath = "ng-volume";
                    spa.UseAngularCliServer(npmScript: "start");
                });
            }
        }
    }
}
