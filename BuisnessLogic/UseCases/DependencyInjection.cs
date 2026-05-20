using Microsoft.Extensions.DependencyInjection;

namespace BuisnessLogic.UseCases
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUseCassess(this
            IServiceCollection services)
        {
            services.AddMediatR(conf =>

                conf.RegisterServicesFromAssembly(typeof(DependencyInjection)
                    .Assembly));
            return services;
        }

    }
}
