using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProd)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                // normally db context would come from ctor dep inj
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
            }
        }

        private static void SeedData(AppDbContext ctx, bool isProd)
        {
            if (isProd) // determine runtime mode
            {
                Console.WriteLine("Trying to do DB migration....");
                try {
                    ctx.Database.Migrate();
                } catch (Exception e) {
                    Console.WriteLine($"DB Migration failed {e.Message}");
                }
            }

            if (!ctx.Platforms.Any())   // if empty
            {
                Console.WriteLine("Populating DB");
                ctx.Platforms.AddRange(         // add several items
                    new Platform() {Name="NodeJS", Publisher="Ryan Dahl", Cost="£0"},
                    new Platform() {Name="Dot net", Publisher="MS", Cost="£0"},
                    new Platform() {Name="Docker", Publisher="Solomon Hykes", Cost="£0"}
                );

                ctx.SaveChanges();      //save the actual data
            }
            else
            {
                Console.WriteLine("DB not empty");  // never going to see this with in-memory DB!
            }
        }
    }
}