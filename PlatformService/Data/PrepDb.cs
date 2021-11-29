using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                // normally db context would come from ctor dep inj
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
            }
        }

        private static void SeedData(AppDbContext ctx)
        {
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