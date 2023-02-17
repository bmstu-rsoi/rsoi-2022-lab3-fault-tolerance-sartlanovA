using System.Reflection;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Payments.ModelsDB;
using Payments.Repositories;
using Payments.Controllers;
using Serilog;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using ModelsDTO.Payments.Cancel;

namespace Payments
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
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddDbContextCheck<PaymentContext>();
            
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Payments", Version = "v1"});
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                
            });
            services.AddSwaggerGenNewtonsoftSupport();

            AddDbContext(services, Configuration);
            AddLogging(services, Configuration);
            
            AddMassTransit(services, Configuration);
            
            services.AddScoped<IPaymentsRepository, PaymentsRepository>();
            services.AddScoped<PaymentsWebController>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payments v1"));

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/manage/health");
                endpoints.MapHealthChecks("/manage/health/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
        }
        
        private static void AddDbContext(IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<PaymentContext>(opt => 
                opt.UseNpgsql(config.GetConnectionString("Local")));
        }
        
        private static void AddLogging(IServiceCollection services, IConfiguration config)
        {
            var log = new LoggerConfiguration()
                .WriteTo.File(config["Logger"])
                .CreateLogger();
            
            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(logger: log, dispose: true));
        }
        
        private static void AddMassTransit(IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<RabbitMqTransportOptions>()
                .BindConfiguration(nameof(RabbitMqTransportOptions));

            services.AddMassTransit(cfg =>
            {
                cfg.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(configuration.GetValue<string>("EndpointPrefix"), false));
                cfg.AddConsumer<PaymentConsumer>();
         
                cfg.ConfigureHealthCheckOptions(x =>
                {
                    x.FailureStatus = HealthStatus.Degraded;
                });
            
                cfg.UsingRabbitMq((context, config) =>
                {
                    config.UseBsonSerializer();
                    config.ConfigureEndpoints(context);
                });
            });
        }
    }
}
