using Infrastructure.DataAccess;
using Infrastructure.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductAPI.DataAccess.Contexts;
using ProductAPI.DataAccess.Repositories;
using ProductAPI.Models;
using ProductAPI.RabbitMQ;

namespace ProductAPI
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
            services.AddDbContext<ProductContext>(options => options.UseSqlServer(Configuration["ConnectionString:ProductDatabase"]));
            services.AddScoped<Repository<Product>, ProductRepository>();
            services.AddSingleton((service) => new RabbitMQConfig("ProductAPI"));
            services.AddTransient<RabbitMQMessenger>();
            services.AddSingleton<IRabbitMQHandler, ProductHandler>();
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