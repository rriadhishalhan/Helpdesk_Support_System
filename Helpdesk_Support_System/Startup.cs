using API.Context;
using API.Repository.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helpdesk_Support_System
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
            services.AddScoped<AccountRepository>();
            services.AddScoped<CategoryRepository>();
            services.AddScoped<EmployeeRepository>();
            services.AddScoped<RoleRepository>();
            services.AddScoped<PositionRepository>();
            services.AddScoped<CustomerRepository>();
            services.AddScoped<PriorityRepository>();
            services.AddScoped<TicketRepository>();
            services.AddScoped<TicketHistoryRepository>();
            services.AddScoped<TicketHistoryRepository>();
            services.AddDbContext<MyContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Helpdesk")));
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
