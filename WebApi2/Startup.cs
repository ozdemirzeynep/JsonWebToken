using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace WebApi2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)  //burada bütün setting dosyaları dependency olarak gelir.
        {
            Configuration = configuration;  //kapsülleme yapmış
        }

        public IConfiguration Configuration { get; }
        public string JwtBearerDefault { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.Configure<TokenManagement>(Configuration.GetSection("tokenManagement"));
            TokenManagement token = Configuration.GetSection("tokenManagement").Get<TokenManagement>();

            byte[] secret = Encoding.ASCII.GetBytes(token.Secret);

            services.AddScoped<IAuthenticationService, TokenAuthenticationService>();
            services.AddScoped<IUserManagementService, UserManagementService>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })

        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secret),
                ValidateIssuer = false,
                ValidateAudience = false
            
            
            };

        });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(x => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());  // api ı herkes tüketebilir.
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
