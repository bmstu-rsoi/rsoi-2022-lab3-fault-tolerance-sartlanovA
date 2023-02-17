using System.Reflection;
using APIGateway.Domain;
using MassTransit;
using Microsoft.OpenApi.Models;
using Serilog;
using Polly;
using Polly.Extensions.Http;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace APIGateway
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
                .AddUrlGroup(new Uri(Configuration["CarsService:Host"] + "/manage/health"), name: "car-service-check", failureStatus: HealthStatus.Degraded)
                .AddUrlGroup(new Uri(Configuration["RentalsService:Host"] + "/manage/health"), name: "rental-service-check", failureStatus: HealthStatus.Degraded)
                .AddUrlGroup(new Uri(Configuration["PaymentsService:Host"] + "/manage/health"), name: "payment-service-check", failureStatus: HealthStatus.Degraded);
            
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Gateway", Version = "v1"});
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                
            });
            services.AddSwaggerGenNewtonsoftSupport();

            services.AddScoped<IRentalsService, RentalsService>();

            services.Configure<CarsSettings>(Configuration.GetSection("CarsService"));
            services.Configure<PaymentsSettings>(Configuration.GetSection("PaymentsService"));
            services.Configure<RentalsSettings>(Configuration.GetSection("RentalsService"));
            
            AddHttpClients(services);
            AddLogging(services, Configuration);
            
            AddMassTransit(services, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway v1"));

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

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

        private static void AddHttpClients(IServiceCollection services)
        {
            services.AddHttpClient<ICarsRepository, CarsRepository>()
                .AddPolicyHandler(InitBreakPolicy());
            
            services.AddHttpClient<IPaymentsRepository, PaymentsRepository>()
                .AddPolicyHandler(InitBreakPolicy());
            
            services.AddHttpClient<IRentalsRepository, RentalsRepository>()
                .AddPolicyHandler(InitBreakPolicy());
        }

        private static IAsyncPolicy<HttpResponseMessage> InitBreakPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(15, TimeSpan.FromSeconds(10));
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
