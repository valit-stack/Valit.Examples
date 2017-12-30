using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Valit.AspNetCore.Validators;

namespace Valit.AspNetCore
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
            services.Scan(s => s.FromAssemblyOf<Startup>().AddClasses(c => c.AssignableTo(typeof(IValitator<>))).AsImplementedInterfaces());
            services.AddTransient<ValitatorFactory, ValitatorFactory>();
            services.AddTransient<ValitatorProvider, ValitatorProvider>();
            services.AddTransient<Resolver, Resolver>();

            var provier = services.BuildServiceProvider();
            var vp = provier.GetService(typeof(ValitatorProvider)) as ValitatorProvider;

            services.AddMvc(options => options.ModelValidatorProviders.Insert(0, vp)).Services.BuildServiceProvider();
            services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
