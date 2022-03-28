
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Design;
using StarApp1.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Google;
using StarApp1.Services;


// Add services to the container.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<StarUserDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MYConnector")));

builder.Services.ConfigureApplicationCookie(config => config.LoginPath = "/SignIn") ;

builder.Services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();


builder.Services.AddScoped<ILogin, Login>();

builder.Services.AddScoped<IDbConnection,DbConnection>();

builder.Services.AddScoped<IRegistration, Registration>();

builder.Services.AddScoped<IAuthHandler, AuthHandler>();
builder.Services.AddScoped<IUserAdmin, User>();
builder.Services.AddScoped<IDashboard, UserDashboard>();
builder.Services.AddScoped<IPopulate, Populate>();
builder.Services.AddScoped<IButton, Button>();
builder.Services.AddScoped<IDataTransfer, DataTransfer>();
builder.Services.AddScoped<ITriggerMail, TriggerMail>();
builder.Services.AddScoped<IResetPassword, ResetPassword>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(15);
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.Name = "star_cookie";
    //options.Cookie.Expiration = TimeSpan.FromMinutes(15);
    options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.LoginPath = "/User/SignIn";
    
});


//builder.Services
//    .AddAuthentication(options =>
//    {
//options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
//})
//    .AddCookie();
  ////  .AddGoogle(options =>
///    {
//        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
//    });
    
builder.Services.AddRazorPages();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseSession();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
