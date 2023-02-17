using System.Reflection;
using APIGateway.Domain;
using Microsoft.OpenApi.Models;
using Serilog;

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

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private static void AddHttpClients(IServiceCollection services)
        {
            services.AddHttpClient<ICarsRepository, CarsRepository>();
            services.AddHttpClient<IPaymentsRepository, PaymentsRepository>();
            services.AddHttpClient<IRentalsRepository, RentalsRepository>();
        }

        private static void AddLogging(IServiceCollection services, IConfiguration config)
        {
            var log = new LoggerConfiguration()
                .WriteTo.File(config["Logger"])
                .CreateLogger();
            
            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(logger: log, dispose: true));
        }
    }
}
