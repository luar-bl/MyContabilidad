using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using ProyectoCasa.Components;
using ProyectoCasa.Service;
using Supabase;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazorBootstrap();

//SERVICIO DE MUDBLAZOR
builder.Services.AddMudServices();

#region "-- Eventos SUPABASE -- "
// Configurar Supabase OBTENER BBDD
var supabaseUrl = builder.Configuration["Supabase:Url"];
var supabaseKey = builder.Configuration["Supabase:Key"];


// 1. Herramientas base
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddCascadingAuthenticationState();

// 2. Configuración de Supabase (Primero las opciones)
var supabaseOptions = new SupabaseOptions
{
    AutoRefreshToken = false,
    AutoConnectRealtime = true,
    // Nota: Si tu versión no tiene PersistSession, lo gestionaremos con el Provider
};

// 3. Cliente de Supabase
builder.Services.AddScoped<Supabase.Client>(sp =>
    new Supabase.Client(supabaseUrl, supabaseKey, supabaseOptions));

// 4. Tu lógica de negocio y seguridad
builder.Services.AddScoped<SupabaseAuthStateProvider>(); // Lo registramos como su clase
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<SupabaseAuthStateProvider>());
builder.Services.AddScoped<ServicioBlanco>();

// 5. Autenticación y Cookies (Antes de los componentes)
builder.Services.AddAuthentication("SupabaseAuth")
    .AddCookie("SupabaseAuth", options =>
    {
        options.LoginPath = "/login";
        options.Cookie.Name = "auth_token";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);

        // 🛡️ AÑADE ESTO PARA EVITAR EL BLOQUEO DEL NAVEGADOR
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddAuthorization();

// 6. Componentes de UI
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
#endregion


// --- FORZAR FORMATO ESPAÑOL ---
var supportedCultures = new[] { "es-ES" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("es-ES");
    options.SupportedCultures = supportedCultures.Select(c => new System.Globalization.CultureInfo(c)).ToList();
    options.SupportedUICultures = supportedCultures.Select(c => new System.Globalization.CultureInfo(c)).ToList();
});
// ------------------------------

var app = builder.Build();
app.UseRequestLocalization(supportedCultures[0]);
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.MapStaticAssets();

app.UseAntiforgery();

app.UseAuthentication(); // ¿Quién eres?
app.UseAuthorization();  // ¿Tienes permiso?


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();