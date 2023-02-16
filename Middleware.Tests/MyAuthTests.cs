using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using webx.Middleware;
namespace Middleware.Tests;
public class MyAuthTests : IAsyncLifetime
{
    IHost? host;
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
    public async Task InitializeAsync()
    {
        host = await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                    })
                    .Configure(app =>
                    {
                        app.UseMiddleware<MyAuth>();
                        app.Run(async context =>
                        {
                            await context.Response.WriteAsync("Authorized");
                        });
                    });
            })
            .StartAsync();
    }
    [Fact]
    public async Task MiddlewareTest_FailWhenNotAuthenticated()
    {
        var response = await host.GetTestClient().GetAsync("/");
        Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode);
        var result = await response.Content.ReadAsStringAsync();
        Assert.Equal("Not Authorized", result);
    }
    [Fact]
    public async Task MiddlewareTest_Authenticated()
    {
        var response = await host.GetTestClient().GetAsync("/?username=user1&password=password1");
        var result = await response.Content.ReadAsStringAsync();

        Assert.Equal("Authorized", result);
    }

        [Fact]
    public async Task MiddlewareTest_FailWhenNotPassword()
    {
        var response = await host.GetTestClient().GetAsync("/?username=user1");
        Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode);
        var result = await response.Content.ReadAsStringAsync();
        Assert.Equal("Not Authorized", result);
    }

        [Fact]
    public async Task MiddlewareTest_FailWhenNotCorrectCredentials()
    {
        var response = await host.GetTestClient().GetAsync("/?username=user1&password=password2");
        Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode);
        var result = await response.Content.ReadAsStringAsync();
        Assert.Equal("Not Authorized", result);
    }
}