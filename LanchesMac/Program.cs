using FastReport.Data;
using LanchesMac.Areas.Admin.Services;
using LanchesMac.Context;
using LanchesMac.Models;
using LanchesMac.Repositories;
using LanchesMac.Repositories.Interfaces;
using LanchesMac.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connection));

FastReport.Utils.RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));

// Incluindo o serviço do identity 
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options => options.AccessDeniedPath = "/Account/AccessDenied");
builder.Services.Configure<ConfigurationImagens>(builder.Configuration.GetSection("ConfigurationPastaImagens"));

// Definir o serviço para poder usar a classe HttpContextAcessor
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // tempo de expiração
//    options.SlidingExpiration = true;
//});

// CÓDIGO PARA CONFIGURAÇÃO IDENTITY DO ASP.NET (Sobrescreve o padrão do Identity)
// PODE SER ENCONTRADO EM: https://github.com/aspnet/AspNetCore
//=============================================================
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});
//=============================================================

builder.Services.AddTransient<ILancheRepository, LancheRepository>();
builder.Services.AddTransient<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddTransient<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<RelatorioVendasService>();
builder.Services.AddScoped<GraficoVendasService>();
builder.Services.AddScoped<RelatorioLanchesService>();

// Registro de usuários e perfis como um serviço
builder.Services.AddScoped<ISeedUserRoleInitial, SeeduserRoleInitial>();

// Registrando a política de autorização baseada nos perfis (roles) - Informando a necessidade do perfil Admin
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin",
        politica =>
        {
            politica.RequireRole("Admin");
        });
});


// Obtendo uma instancia da classe Carrinho de compra com um contexto, id (guid) e a lista de itens
// AddScoped - instância do carrinho a cada request
builder.Services.AddScoped(sp => CarrinhoCompra.GetCarrinho(sp));

builder.Services.AddControllersWithViews();

// 1 Etapa Configuração para paginação
builder.Services.AddPaging(options =>
{
    options.ViewName = "Bootstrap4";
    options.PageParameterName = "pageindex";
});

// Serviço de Autorização necessário para UseAuthorization
builder.Services.AddAuthorization();

// -- Registrando os midlewares --
// habilitar o cache
builder.Services.AddMemoryCache();
// habilitar o Session
builder.Services.AddSession();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change
    app.UseHsts();
}
app.UseHttpsRedirection();

// PERMITE ACESSO A ARQUIVOS STATICOS DA APLICAÇÃO
// ESTÁ DEFINIDO ANTES DA AUTHENTICAÇÃO (OS ARQUIVOS FICARÃO COM ACESSO PÚBLICO)
app.UseStaticFiles();
app.UseFastReport();
app.UseRouting();

CriarPerfisUsuarios(app);
////Usando a instancia do serviço de perfís e usuários (UTILIZADO NA CLASSE STARTUP)
////Criar os perfis
//seedUserRoleInitial.SeedRoles();
////Cria os usuários e atribui aos perfis
//seedUserRoleInitial.SeedUsers();

app.UseSession();  // É necessário ativar o session

app.UseAuthentication();
//Ativar o midleware do FastReport 
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    //// TESTE DE ROTEAMENTO
    //endpoints.MapControllerRoute(
    //    name: "teste",
    //    pattern: "testeme",
    //    defaults: new { controller = "teste", action = "Index" });

    //endpoints.MapControllerRoute(
    //    name: "admin",
    //    pattern: "admin/{action=Index}/{id?}",
    //    defaults: new { controller = "admin" });

    ////TESTE DE ORDEM DE CONFIGURAÇÃO
    //endpoints.MapControllerRoute(
    //    name: "admin",
    //    pattern: "admin",
    //    defaults: new { controller = "Admin", Action = "Index" });

    //endpoints.MapControllerRoute(
    //    name: "home",
    //    pattern: "{home}",
    //    defaults: new { controller = "Home", action = "Index" });

    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}"
    );

    endpoints.MapControllerRoute(
        name: "categoriaFiltro",
        pattern: "Lanche/{action}/{categoria?}",
        defaults: new { Controller = "Lanche", Action = "List" });

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();

void CriarPerfisUsuarios(WebApplication app)
{
    // IServiceScopeFactory - cria instâncias do serviço no meu scopo
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using (var scope = scopedFactory.CreateScope())
    {
        // OBTÉM A INSTÃNCIA DO SERVIÇO
        var service = scope.ServiceProvider.GetService<ISeedUserRoleInitial>();
        // UTILIZA O SERVIÇO
        service.SeedUsers();
        service.SeedRoles();
    }
}