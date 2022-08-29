using ImageGallery.API.Authorization;
using ImageGallery.API.DbContexts;
using ImageGallery.API.Services;
using ImageGallery.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(configure => configure.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddDbContext<GalleryContext>(options =>
{
    options.UseSqlite(
        builder.Configuration["ConnectionStrings:ImageGalleryDBConnectionString"]);
});

// register the repository
builder.Services.AddScoped<IGalleryRepository, GalleryRepository>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthorizationHandler, MustOwnImageHandler>();

// register AutoMapper-related services
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      //.AddJwtBearer(options =>
      //{
      //    options.Authority = "https://localhost:5001";
      //    options.Audience = "imagegalleryapi";
      //    options.TokenValidationParameters = new ()
      //    {
      //        NameClaimType = "given_name",
      //        RoleClaimType = "role",
      //        ValidTypes = new[] { "at+jwt" }
      //    };
      //});
      .AddOAuth2Introspection(options =>
      {
          options.Authority = "https://localhost:44300";
          options.ClientId = "imagegalleryapi";
          options.ClientSecret = "apisecret";
          options.NameClaimType = "given_name";
          options.RoleClaimType = "role";
      });


builder.Services.AddAuthorization(authorizationOptions =>
{
    authorizationOptions.AddPolicy(
        "UserCanAddImage", AuthorizationPolicies.CanAddImage());
    authorizationOptions.AddPolicy(
        "ClientApplicationCanWrite", policyBuilder =>
        {
            policyBuilder.RequireClaim("scope", "imagegalleryapi.write");
        });
    authorizationOptions.AddPolicy(
        "MustOwnImage", policyBuilder =>
        {
            policyBuilder.RequireAuthenticatedUser();
            policyBuilder.AddRequirements(
                new MustOwnImageRequirement());

        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
