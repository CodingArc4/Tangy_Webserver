using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Tangy_DataAccess.Data
{
    public class DatabaseContext
    {
        private static IServiceScopeFactory _serviceScopeFactory;
        public DatabaseContext(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        //public static ApplicationDbContext GetContext()
        //{
        //    IDbContextFactory<ApplicationDbContext> contextFactory = CoreServices.GetService<IDbContextFactory<ApplicationDbContext>>();
        //    return contextFactory?.CreateDbContext();
        //    //return _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
        //}
    }
}
