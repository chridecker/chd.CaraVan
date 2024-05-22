using chd.CaraVan.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace chd.CaraVan.DataAccess.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection AddDAL(this IServiceCollection services, params Assembly[] assemblies)
        {
            var me = Assembly.GetExecutingAssembly();
            if (!assemblies.Contains(me))
            {
                services.AddRepositories(me, ServiceLifetime.Singleton);
            }
            foreach (var assembly in assemblies)
            {
                services.AddRepositories(me, ServiceLifetime.Singleton);
            }
            return services;
        }
        private static IServiceCollection AddRepositories(this IServiceCollection services, Assembly assembly, ServiceLifetime serviceLifetime)
        {
            foreach (var type in assembly.GetTypes().Where(x => x.IsInterface && x.IsAssignableTo(typeof(IRepository))))
            {
                try
                {
                    var concreteType = assembly.GetTypes().FirstOrDefault(x => !x.IsAbstract && !x.IsInterface && x.IsAssignableTo(type));
                    if (concreteType != null)
                    {
                        services.Add(new ServiceDescriptor(type, concreteType, serviceLifetime));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return services;
        }
    }
}
