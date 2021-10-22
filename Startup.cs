using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RedisDemo.Data;
using RedisDemo.Models;
using RedisDemo.Infra;

namespace RedisDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RedisDemo", Version = "v1" });
            });

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379";
            });

            services.AddDbContext<ApplicationContext>((provider, options) =>
            {
                options.UseNpgsql("Host=localhost;Database=RedisDemo;Username=postgres;Password=123");
                options.LogTo(Console.WriteLine);
                options.EnableSensitiveDataLogging();
            });

            services.AddScoped<ICacheProvider, CacheProvider>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RedisDemo v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            InitDatabase(app);
        }

        public void InitDatabase(IApplicationBuilder app)
        {
            using var db = app
                .ApplicationServices
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<ApplicationContext>();

            if (db.Database.EnsureCreated())
            {
                db.Countries.AddRange(new Country[]
                {
                    new Country
                    {
                        Name = "Brazil",
                        Capital = "Brasilia",
                    },
                    new Country
                    {
                        Name = "Japan",
                        Capital = "Tokyo",
                    },
                    new Country
                    {
                        Name = "Finland",
                        Capital = "Helsinki"
                    },
                    new Country
                    {
                        Name = "Italy",
                        Capital = "Rome"
                    },
                    new Country
                    {
                        Name = "Romania",
                        Capital = "Bucharest"
                    },
                    new Country
                    {
                        Name = "Thailand",
                        Capital = "Bangkok"
                    },
                });

                db.SaveChanges();
            }
        }
    }
}
