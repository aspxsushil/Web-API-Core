using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotificationApi.Services;

namespace NotificationApi
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
            services.Configure<DatabaseSettings>(Configuration.GetSection(nameof(DatabaseSettings)));

            services.AddSingleton<IDatabaseSettings>(sp => sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);

            services.AddSingleton<AuthService>();

            services.AddSingleton<NotificationService>();

            services.AddCors(options=>{
                options.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            //Register JWT Authentication Middleware and add authentication scheme

            /*Token is going to be valid if:
             * 1.Issuer is the actual server that created the token ValidateIssuer=true
             * 2.Receipient of the token is the actual recepient (ValidateAudience = true)
             * 3.Token has not expired
             * 4.The signing key is valid and trusted by the server (ValidateIssuerSigningKey = true)
             * Values for the issuer , audience and signing keys are also provided.
             */
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                                            .AddJwtBearer(options =>
                                            {
                                                options.TokenValidationParameters = new TokenValidationParameters
                                                {

                                                    ValidateIssuer = true,
                                                    ValidateAudience = true,
                                                    ValidateLifetime = true,
                                                    ValidateIssuerSigningKey = true,

                                                    ValidIssuer = "https://localhost:44316",
                                                    ValidAudience = "http://localhost:4200",
                                                    IssuerSigningKey =  new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"))

                                                };

                                            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseAuthentication();
            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
