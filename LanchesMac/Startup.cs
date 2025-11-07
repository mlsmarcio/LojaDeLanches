using LanchesMac.Areas.Admin.Services;
using LanchesMac.Context;
using LanchesMac.Models;
using LanchesMac.Repositories;
using LanchesMac.Repositories.Interfaces;
using LanchesMac.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;

namespace LanchesMac
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add serv
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Incluindo o serviço do identity 
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options => options.AccessDeniedPath = "/Account/AccessDenied");
            services.Configure<ConfigurationImagens>(Configuration.GetSection("ConfigurationPastaImagens"));

            // Definir o serviço para poder usar a classe HttpContextAcessor
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            //services.ConfigureApplicationCookie(options =>
            //{
            //    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // tempo de expiração
            //    options.SlidingExpiration = true;
            //});

            // CÓDIGO PARA CONFIGURAÇÃO IDENTITY DO ASP.NET (Sobrescreve o padrão do Identity)
            // PODE SER ENCONTRADO EM: https://github.com/aspnet/AspNetCore
            //=============================================================
            services.Configure<IdentityOptions>(options =>
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

            services.AddTransient<ILancheRepository, LancheRepository>();
            services.AddTransient<ICategoriaRepository, CategoriaRepository>();
            services.AddTransient<IPedidoRepository, PedidoRepository>();
            services.AddScoped<RelatorioVendasService>();
            services.AddScoped<GraficoVendasService>();

            // Registro de usuários e perfis como um serviço
            services.AddScoped<ISeedUserRoleInitial, SeeduserRoleInitial>();

            // Registrando a política de autorização baseada nos perfis (roles) - Informando a necessidade do perfil Admin
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin",
                    politica =>
                    {
                        politica.RequireRole("Admin");
                    });
            });


            // Obtendo uma instancia da classe Carrinho de compra com um contexto, id (guid) e a lista de itens
            // AddScoped - instância do carrinho a cada request
            services.AddScoped(sp => CarrinhoCompra.GetCarrinho(sp));

            services.AddControllersWithViews();

            // 1 Etapa Configuração para paginação
            services.AddPaging(options =>
            {
                options.ViewName = "Bootstrap4";
                options.PageParameterName = "pageindex";
            });

            // Serviço de Autorização necessário para UseAuthorization
            services.AddAuthorization();

            // -- Registrando os midlewares --
            // habilitar o cache
            services.AddMemoryCache();
            // habilitar o Session
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configur
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ISeedUserRoleInitial seedUserRoleInitial)
        {
            if (env.IsDevelopment())
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

            app.UseRouting();

            //Usando a instancia d0 serviço de perfís e usuários
            //Criar os perfis
            seedUserRoleInitial.SeedRoles();
            //Cria os usuários e atribui aos perfis
            seedUserRoleInitial.SeedUsers();

            app.UseSession();  // É necessário ativar o session

            app.UseAuthentication();
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
        }
    }
}
