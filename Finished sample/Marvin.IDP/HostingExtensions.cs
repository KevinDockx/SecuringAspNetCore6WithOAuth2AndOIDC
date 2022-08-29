using Duende.IdentityServer;
using Marvin.IDP.DbContexts;
using Marvin.IDP.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Marvin.IDP;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(
        this WebApplicationBuilder builder)
    {
        // configures IIS out-of-proc settings 
        builder.Services.Configure<IISOptions>(iis =>
        {
            iis.AuthenticationDisplayName = "Windows";
            iis.AutomaticAuthentication = false;
        });
        // ..or configures IIS in-proc settings
        builder.Services.Configure<IISServerOptions>(iis =>
        {
            iis.AuthenticationDisplayName = "Windows";
            iis.AutomaticAuthentication = false;
        });


        // uncomment if you want to add a UI
        builder.Services.AddRazorPages();

        builder.Services.AddScoped<IPasswordHasher<Entities.User>, 
            PasswordHasher<Entities.User>>();

        builder.Services.AddScoped<ILocalUserService, LocalUserService>();

        builder.Services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseSqlite(
                builder.Configuration
                .GetConnectionString("MarvinIdentityDBConnectionString"));
        });


        builder.Services.AddIdentityServer(options =>
            {
                // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
                options.EmitStaticAudienceClaim = true;
            })
            .AddProfileService<LocalUserProfileService>()
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryApiResources(Config.ApiResources)
            .AddInMemoryClients(Config.Clients);

       builder.Services
            .AddAuthentication()
            .AddOpenIdConnect("AAD", "Azure Active Directory", options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.Authority = "https://login.microsoftonline.com/621cb4b2-eeb8-4699-913d-a651c392babd/v2.0";
                options.ClientId = "df55658d-e228-4f72-9f11-b60334edb0e2";
                options.ClientSecret = "Tkt8Q~gJ9yez1EGA6BqFJfCVWIzM_B.bCAUx8bkB";
                options.ResponseType = "code";
                options.CallbackPath = new PathString("/signin-aad/");
                options.SignedOutCallbackPath = new PathString("/signout-aad/");
                options.Scope.Add("email");
                options.Scope.Add("offline_access");
                options.SaveTokens = true;
            });

        builder.Services.AddAuthentication()
            .AddFacebook("Facebook",
               options =>
               {
                   options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                   options.AppId = "864396097871039";
                   options.AppSecret = "11015f9e340b0990b0e50f39dd8a4e9a";
               }); 

        return builder.Build();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    { 
        app.UseSerilogRequestLogging();
    
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // uncomment if you want to add a UI
        app.UseStaticFiles();
        app.UseRouting();

        app.UseIdentityServer();

        // uncomment if you want to add a UI
        app.UseAuthorization();
        app.MapRazorPages().RequireAuthorization();

        return app;
    }
}
