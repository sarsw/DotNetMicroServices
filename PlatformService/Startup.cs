using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

namespace PlatformService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private readonly IWebHostEnvironment _env;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)       // inject some useful items
        {
            Configuration = configuration;
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (_env.IsProduction()) // comment this out to force MSSQL when doing the initial migration with "dotnet ef"
            {
                Console.WriteLine("Uses MSSQL DB in prod mode");
                services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("PlatformsConn")));   // fine to use mem db for testing
            }
            else
            {
                Console.WriteLine("Uses memDB in dev mode");
                services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));   // fine to use mem db for testing
            }

            services.AddScoped<IPlatformRepo, PlatformRepo>();  // if asked for IPlatformRepo then provide a PlatformRepo

            services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();    // make use of http clienat factory, when asked for a ICommandDC, provide a HCDC

            services.AddSingleton<IMessageBusClient, MessageBusClient>();
            
            services.AddGrpc();

            services.AddControllers();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
            });

            Console.WriteLine($"Startup PlatformService using Command Service Endpoint: {Configuration["CommandService"]}");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<GrpcPlatformService>();

                // serve up the proto contract to clients
                string protoFile = "Protos/platforms.proto";
                endpoints.MapGet("/"+protoFile.ToLower(), async context =>
                {
                    await context.Response.WriteAsync(File.ReadAllText(protoFile));
                });
            });

            PrepDb.PrepPopulation(app, _env.IsProduction());      // comment out for DB migration
        }
    }
}
