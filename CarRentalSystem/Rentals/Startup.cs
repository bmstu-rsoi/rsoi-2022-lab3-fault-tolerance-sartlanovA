using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Rentals.ModelsDB;
using Rentals.Repositories;
using Rentals.Controllers;
using Serilog;

namespace Rentals
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
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Rentals", Version = "v1"});
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                
            });
            services.AddSwaggerGenNewtonsoftSupport();

            AddDbContext(services, Configuration);
            AddScoped(services);
            AddLogging(services, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rentals v1"));

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
        
        private static void AddDbContext(IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<RentalContext>(opt => 
                opt.UseNpgsql(config.GetConnectionString("Postgres")));
        }

        private static void AddScoped(IServiceCollection services)
        {
            services.AddScoped<IRentalsRepository, RentalsRepository>();
            services.AddScoped<RentalsWebController>();
        }
        
        private static void AddLogging(IServiceCollection services, IConfiguration config)
        {
            var logger = new LoggerConfiguration()
                .WriteTo.File(config["Logger"])
                .CreateLogger();
            
            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(logger, true));
        }
    }
}
