using System;
using AutoMapper;
using Boilerplate.Api;
using Boilerplate.EF;
using Boilerplate.IntegrationTests.Mocks;
using Boilerplate.Services.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Boilerplate.IntegrationTests
{
    public class Factory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryAppDb");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                services.AddAutoMapper(typeof(Startup));

                var sp = services.BuildServiceProvider();
                var mapper = sp.GetService<IMapper>();
                mapper.ConfigurationProvider.AssertConfigurationIsValid();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<Factory>>();

                    db.Database.EnsureCreated();

                    try
                    {
                        // Seed the database with test data.
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"An error occurred seeding the " +
                                            "database with test messages. ErrorModel: {ex.Message}");
                    }
                }
            });

            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IPermissionsService, MockedPermissionsService>();
            });
        }
    }
}