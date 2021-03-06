using AutoMapper;
using JLL.PizzaProblem.API.Profiles;
using JLL.PizzaProblem.API.Middleware;
using JLL.PizzaProblem.DataAccess.EF;
using JLL.PizzaProblem.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace JLL.PizzaProblem
{
    [ExcludeFromCodeCoverage]
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
            services.AddCors();
            services.AddControllers().AddNewtonsoftJson();
            services.AddAutoMapper(typeof(Startup));

            // configure appsettings object to get secrets and generate tokens
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            // DI for in memory database
            services.AddDbContext<PizzaProblemContext>(options => options.UseInMemoryDatabase(databaseName: "PizzaProblem"));

            // configure DI for services
            // As a singleton for testing simplicity but it should be scoped when persistence available
            services.AddTransient<JwtMiddleware>();
            services.AddTransient<IUserService, UserService>();
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

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            // load custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
