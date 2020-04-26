using System;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using ProjectArena.Api.Filters;
using ProjectArena.Application;
using ProjectArena.Domain;
using ProjectArena.Domain.ArenaHub;
using ProjectArena.Domain.BattleService;
using ProjectArena.Domain.Email;
using ProjectArena.Domain.Mongo;
using ProjectArena.Domain.QueueService;
using ProjectArena.Domain.Registry;
using ProjectArena.Infrastructure;
using Serilog.Events;

namespace ProjectArena.Api
{
  public class Startup
    {
        public static LogEventLevel LogEventLevel { get; set; }

        public Startup(IWebHostEnvironment environment)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables()
                .Build();
            LogEventLevel = (LogEventLevel)Enum.Parse(LogEventLevel.GetType(), Configuration["Logging:Level"]);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers(options =>
                {
                    options.Filters.Add(typeof(ValidationFilter));
                    options.Filters.Add(typeof(ExceptionFilter));
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver =
                        new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                .AddFluentValidation(options =>
                {
                    options.RegisterValidatorsFromAssembly(ApplicationRegistry.GetAssembly());
                });
            services.Configure<ServerSettings>(
                Configuration.GetSection("Server"));
            services.Configure<MongoConnectionSettings>(
                Configuration.GetSection("MongoConnection"));
            services.Configure<MongoContextSettings<RegistryContext>>(
                Configuration.GetSection("MongoConnection:Registry"));
            services.Configure<EmailSenderSettings>(
                Configuration.GetSection("SmtpServer"));
            services.RegisterDomainLayer($"{Configuration["MongoConnection:ServerName"]}/{Configuration["MongoConnection:Identity:DatabaseName"]}");
            services.RegisterApplicationLayer();
        }

        private bool IsFrontendRoute(HttpContext context)
        {
            var path = context.Request.Path;
            return path.HasValue &&
                !path.Value.StartsWith("/api", StringComparison.OrdinalIgnoreCase) &&
                !path.Value.StartsWith("/hub", StringComparison.OrdinalIgnoreCase);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ApplicationServices.GetRequiredService<IMongoConnection>();

            var registry = app.ApplicationServices.GetRequiredService<RegistryContext>();
            registry.LoadMigrations();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ArenaHub>("/hub");
            });
        }
    }
}
