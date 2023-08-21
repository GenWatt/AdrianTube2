using AdrianTube2.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Blazored.Toast;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using AdrianTube2.state;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<MovieService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<LikeService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<StateContainer>();
builder.Services.AddScoped<UserService>();
builder.Services.AddBlazoredToast();
builder.Services.AddHttpClient();

// configure multipart legnt limit
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1024 * 1024 * 15; // 15MB
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
