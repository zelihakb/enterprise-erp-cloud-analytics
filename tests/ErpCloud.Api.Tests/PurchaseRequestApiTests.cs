using System.Net;
using System.Net.Http.Json;
using ErpCloud.Api.Data;
using ErpCloud.Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Testcontainers.MsSql;

namespace ErpCloud.Api.Tests;

public class PurchaseRequestApiTests
{
    [Fact]
    public async Task CreateRequest_CanBeRetrievedThroughApi()
    {
        await using var sql = new MsSqlBuilder(
            "mcr.microsoft.com/mssql/server:2022-latest"
        ).Build();

        await sql.StartAsync();

        var connectionString = new SqlConnectionStringBuilder(
            sql.GetConnectionString()
        )
        {
            InitialCatalog = "ErpCloudApiTests"
        }.ConnectionString;

        using var factory = new TestApiFactory(connectionString);
        using var client = factory.CreateClient();

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider
                .GetRequiredService<ErpDbContext>();

            Assert.Equal(
                "ErpCloudApiTests",
                db.Database.GetDbConnection().Database
            );

            await db.Database.MigrateAsync();
        }

        using var response = await client.PostAsJsonAsync(
            "/api/purchase-requests",
            new PurchaseRequest(2001, "Mouse", 2)
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var saved = await client.GetFromJsonAsync<PurchaseRequest>(
            "/api/purchase-requests/2001"
        );

        Assert.NotNull(saved);
        Assert.Equal("Mouse", saved.ProductName);
        Assert.Equal(2, saved.Quantity);
    }

    private sealed class TestApiFactory : WebApplicationFactory<Program>
    {
        private readonly string _connectionString;

        public TestApiFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureHostConfiguration(config =>
                config.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:ErpDatabase"] = _connectionString
                    }
                )
            );

            return base.CreateHost(builder);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<
                    IDbContextOptionsConfiguration<ErpDbContext>>();

                services.RemoveAll<DbContextOptions<ErpDbContext>>();

                services.AddDbContext<ErpDbContext>(options =>
                    options.UseSqlServer(_connectionString));
            });
        }
    }
}