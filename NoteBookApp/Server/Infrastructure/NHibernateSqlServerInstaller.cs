using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using NHibernate.AspNetCore.Identity;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using NHibernate.NetCore;
using NoteBookApp.Logic.Mappings;
using NoteBookApp.Logic.Models;

namespace NoteBookApp.Server.Infrastructure
{
    public static class NHibernateSqlServerInstaller
    {
        public static IServiceCollection AddNHibernateSqlServer(this IServiceCollection services, string cnString)
        {
            var cfg = new Configuration();

            cfg.DataBaseIntegration(db =>
            {
                db.Dialect<MsSql2012Dialect>();
                db.Driver<MicrosoftDataSqlClientDriver>();
                db.ConnectionProvider<DriverConnectionProvider>();
                db.BatchSize = 500;
                //db.IsolationLevel = System.Data.IsolationLevel.ReadCommitted;
                db.LogSqlInConsole = true;
                db.ConnectionString = cnString;
                db.Timeout = 30;/*seconds*/
                db.SchemaAction = SchemaAutoAction.Validate;
            });


            //cfg.Proxy(p => p.ProxyFactoryFactory<StaticProxyFactoryFactory>());
            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddDefaultTokenProviders()
                .AddHibernateStores();
            cfg.Cache(c => c.UseQueryCache = false);

            var mapping = new ModelMapper();
            mapping.AddMappings(typeof(ApplicationUserMapping).Assembly.GetTypes());
            mapping.AddMapping(typeof(NHibernate.AspNetCore.Identity.Mappings.IdentityUserMappingMsSql));
            mapping.AddMapping(typeof(NHibernate.AspNetCore.Identity.Mappings.IdentityUserLoginMappingMsSql));
            mapping.AddMapping(typeof(NHibernate.AspNetCore.Identity.Mappings.IdentityUserTokenMappingMsSql));
            var mappingDocument = mapping.CompileMappingForAllExplicitlyAddedEntities();
            cfg.AddMapping(mappingDocument);
            services.AddHibernate(cfg);
#if DEBUG
            cfg.BuildSessionFactory();
#endif

            services.AddScoped<ISession>(provider =>
            {
                //var companyResolution = provider.GetService<ICompanyResolutionStrategy>();
                //var companyId = companyResolution!.GetCompanyIdAsync().GetAwaiter().GetResult();
                return provider.GetService<ISessionFactory>()!.OpenSession();
            });
            return services;
        }
    }
}