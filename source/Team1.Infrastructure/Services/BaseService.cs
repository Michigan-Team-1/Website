using Team1.Entities;
using Team1.Infrastructure.UserIdentity;
using System.Linq;
using System.Reflection;

namespace Team1.Infrastructure.Services
{
    public abstract class BaseService
    {
        protected DataContext db { get; set; }
        protected ILoggingContext LoggingDb { get; set; }
        protected UserPermissionService UserPermissionService { get; set; }

        /// <summary>
        /// Sets up the data context and UserPermissionService for the service and all services injected in the service
        /// </summary>
        /// <param name="spudContext">data context</param>
        /// <param name="userPermissionService">UserPermissionService</param>
        public void SetupService(DataContext spudContext, ILoggingContext loggingDb, UserPermissionService userPermissionService)
        {
            db = spudContext;
            LoggingDb = loggingDb;
            UserPermissionService = userPermissionService;
            
            // go through all fields, if any of them are a BaseService Setup DB and UPS
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                         BindingFlags.Static | BindingFlags.Instance |
                         BindingFlags.DeclaredOnly;
            var fields = this.GetType().UnderlyingSystemType.GetFields(flags).Where(w => w.FieldType.BaseType == typeof(BaseService)).ToList();
            foreach (var item in fields)
            {
                var baseService = (item.GetValue(this) as BaseService);
                baseService?.SetupService(db, LoggingDb, UserPermissionService);
            }
        }
    }
}
