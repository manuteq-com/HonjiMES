using HonjiMES.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace HonjiMES
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
            var ConnectionStringMyDB = Configuration.GetConnectionString("MyDB");
            //services.AddMvc().AddNewtonsoftJson(options => { options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; });
            services.AddControllers().AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
                o.JsonSerializerOptions.DictionaryKeyPolicy = null;
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver() { NamingStrategy = new Newtonsoft.Json.Serialization.DefaultNamingStrategy() };
            });
            services.AddControllersWithViews() ;//.AddNewtonsoftJson();
            // In production, the Angular files will be served from this directory  
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            services.AddDbContext<HonjiContext>(options => options.UseLazyLoadingProxies().UseMySql(ConnectionStringMyDB, x => x.ServerVersion("8.0.19-mysql"))); //將原本ConnectString移到appsettings.json
            services.AddMvc().AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
                o.JsonSerializerOptions.DictionaryKeyPolicy = null;
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver() { NamingStrategy = new Newtonsoft.Json.Serialization.DefaultNamingStrategy() };
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0).ConfigureApiBehaviorOptions(options =>
            {
                // 關閉驗證失敗時自動 HTTP 400 回應
                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddMvc().AddXmlSerializerFormatters();
            services.AddSwaggerDocument();
            services.AddCors(options =>
            {
                // CorsPolicy 是自訂的 Policy 名稱
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.WithOrigins("http://localhost")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseOpenApi();
            app.UseSwaggerUi3();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
            app.UseCors("CorsPolicy");
        }
    }
}
