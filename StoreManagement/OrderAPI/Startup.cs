using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.DataAccess;
using Infrastructure.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderAPI.DataAccess.Contexts;
using OrderAPI.DataAccess.Repositories;
using OrderAPI.Models;
using OrderAPI.RabbitMQ;

namespace OrderAPI
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
            services.AddMvc(options => options.EnableEndpointRouting = false).AddNewtonsoftJson();
            services.AddDbContext<OrderContext>(options => options.UseSqlServer(Configuration["ConnectionString:OrderDatabase"]));
            services.AddDbContext<ProductContext>(options => options.UseSqlServer(Configuration["ConnectionString:OrderDatabase"]));
            services.AddScoped<Repository<Order>, OrderRepository>();
            services.AddScoped<Repository<Product>, ProductRepository>();
            services.AddSingleton((service) => new RabbitMQConfig("OrderAPI"));
            services.AddTransient<RabbitMQMessenger>();
            services.AddSingleton<IRabbitMQHandler, OrderHandler>();
            services.AddHostedService<RabbitMQListener>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
