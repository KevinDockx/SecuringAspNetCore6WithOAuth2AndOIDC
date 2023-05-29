using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(configure => 
        configure.JsonSerializerOptions.PropertyNamingPolicy = null);

// create an HttpClient used for accessing the API
builder.Services.AddHttpClient("APIClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ImageGalleryAPIRoot"]);
    client.DefaultRequestHeaders.Clear();
    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
});

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme) //after authenticated will save encrypted token validation 
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, option =>
    {
        option.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        option.Authority = "https://localhost:5001/"; //address for identity provider that responsable for identity provider part of openid connect flow,
                                                     //the middleware use this value to read metadata and discover endpoints
        option.ClientId = "imagegallaryclient";
        option.ClientSecret = "secret";
        option.ResponseType = "code";
        //option.Scope.Add("openid"); //Default
        //option.Scope.Add("profile");
        //option.CallbackPath = "signein-oidc";
        option.SaveTokens = true;
        option.GetClaimsFromUserInfoEndpoint = true;
    }

    ); // register and configure open id connect handler,
       // enable our app to support the openid connect authentication workflow
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Gallery}/{action=Index}/{id?}");

app.Run();
