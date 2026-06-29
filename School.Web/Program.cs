using Blazored.SessionStorage;
using School.Web.Components;
using School.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("SchoolApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7181/");
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ToastService>();

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddBlazoredSessionStorage();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();