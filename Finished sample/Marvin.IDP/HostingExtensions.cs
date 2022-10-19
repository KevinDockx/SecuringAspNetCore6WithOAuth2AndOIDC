using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Duende.IdentityServer;
using Marvin.IDP.DbContexts;
using Marvin.IDP.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

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

        var azureCredential = new DefaultAzureCredential();

        builder.Services.AddDataProtection()
            .PersistKeysToAzureBlobStorage(
                new Uri(builder.Configuration["DataProtection:Keys"]),
                azureCredential)
            .ProtectKeysWithAzureKeyVault(
                new Uri(builder.Configuration["DataProtection:ProtectionKeyForKeys"]),
                azureCredential);

        var secretClient = new SecretClient(
               new Uri(builder.Configuration["KeyVault:RootUri"]),
               azureCredential);

        var secretResponse = secretClient.GetSecret(
            builder.Configuration["KeyVault:CertificateName"]);

        var signingCertificate = new X509Certificate2(
            Convert.FromBase64String(secretResponse.Value.Value),
            (string)null,
            X509KeyStorageFlags.MachineKeySet);


        // certificate in KeyVault: 
        //  - certificate resource (public key, metadata)
        //  - key resource (private key)
        //  - secret resource (full certificate)

        // uncomment if you want to add a UI
        builder.Services.AddRazorPages();

        builder.Services.AddScoped<IPasswordHasher<Entities.User>, 
            PasswordHasher<Entities.User>>();

        builder.Services.AddScoped<ILocalUserService, LocalUserService>();

        builder.Services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseSqlServer(
               builder.Configuration
               .GetConnectionString("MarvinIdentityDBConnectionString"));
        });

        var migrationsAssembly = typeof(Program).GetTypeInfo()
            .Assembly.GetName().Name;

        builder.Services.AddIdentityServer(options =>
            {
                // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
                options.EmitStaticAudienceClaim = true;               
            })
            .AddProfileService<LocalUserProfileService>()
            //.AddInMemoryIdentityResources(Config.IdentityResources)
            //.AddInMemoryApiScopes(Config.ApiScopes)
            //.AddInMemoryApiResources(Config.ApiResources)
            //.AddInMemoryClients(Config.Clients)
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = optionsBuilder =>
                optionsBuilder.UseSqlServer(
                    builder.Configuration
                    .GetConnectionString("IdentityServerDBConnectionString"),
                            sqlOptions => sqlOptions
                            .MigrationsAssembly(migrationsAssembly));
            })
            .AddConfigurationStoreCache()
            .AddOperationalStore(options =>
             {
                  options.ConfigureDbContext = optionsBuilder =>
                     optionsBuilder.UseSqlServer(builder.Configuration
                     .GetConnectionString("IdentityServerDBConnectionString"),
                           sqlOptions => sqlOptions
                           .MigrationsAssembly(migrationsAssembly));
                 options.EnableTokenCleanup = true;
             })
            .AddSigningCredential(signingCertificate); 

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

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor
              | ForwardedHeaders.XForwardedProto;
        });

        return builder.Build();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseForwardedHeaders();

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
