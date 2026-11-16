using Entities;
using Entities.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ContactsManager.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            // Remove the existing ApplicationDbContext registration
            var descriptor = services.SingleOrDefault(service =>
                service.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            var providerConfigurations = services
                .Where(d => d.ServiceType.Name.StartsWith("IDbContextOptionsConfiguration"))
                .ToList();
            foreach (var config in providerConfigurations)
            {
                services.Remove(config);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            // Remove the Identity-added authentication schemes to avoid conflicts
            var identityAuthDescriptors = services
                .Where(s => s.ServiceType.Namespace?.StartsWith("Microsoft.AspNetCore.Identity") == true)
                .ToList();
            foreach (var desc in identityAuthDescriptors)
            {
                services.Remove(desc);
            }

            // Add Identity services for DI (controllers need UserManager, SignInManager, etc.)
            services
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
                .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

            // Replace authentication with test scheme
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = "TestScheme";
                    options.DefaultChallengeScheme = "TestScheme";
                    options.DefaultAuthenticateScheme = "TestScheme";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("TestScheme", options => { });
        });
    }
}

public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    // Use a constant GUID so the same user is returned for all requests
    // This is important for antiforgery token validation
    private static readonly string TestUserId = Guid.NewGuid().ToString();

    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Create an authenticated principal with test user 
        // Note: Use the same GUID for all requests so antiforgery tokens are valid
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, TestUserId),
            new Claim(ClaimTypes.Email, "testuser@example.com"),
            new Claim(ClaimTypes.Name, "Test User")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return await Task.FromResult(AuthenticateResult.Success(ticket));
    }
}