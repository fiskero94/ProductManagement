using Infrastructure.DataAccess;
using Infrastructure.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderAPI.DataAccess.Contexts;
using OrderAPI.DataAccess.Repositories;
using OrderAPI.Models;
using OrderAPI.RabbitMQ;

namespace OrderAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.EnableEndpointRouting = false).AddNewtonsoftJson();
            services.AddDbContext<OrderContext>(options => options.UseSqlServer(Configuration["ConnectionString:OrderDatabase"]));
            services.AddDbContext<ProductContext>(options => options.UseSqlServer(Configuration["ConnectionString:OrderDatabase"]));
            services.AddScoped<IRepository<Order>, OrderRepository>();
            services.AddScoped<IRepository<Product>, ProductRepository>();
            services.AddSingleton((service) => new RabbitMQConfig("OrderAPI"));
            services.AddTransient<IRabbitMQMessenger, RabbitMQMessenger>();
            services.AddSingleton<IRabbitMQHandler, OrderHandler>();
            services.AddHostedService<RabbitMQListener>();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}