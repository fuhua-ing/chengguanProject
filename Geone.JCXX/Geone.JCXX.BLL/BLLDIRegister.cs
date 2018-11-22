using Microsoft.Extensions.DependencyInjection;

namespace Geone.JCXX.BLL
{
    public static class BLLDIRegister
    {
        public static void Register(IServiceCollection services)
        {
            services.AddTransient(typeof(IAppBLL), typeof(AppBLL));
            services.AddTransient(typeof(IAppMenuBLL), typeof(AppMenuBLL));
            services.AddTransient(typeof(IAppRoleBLL), typeof(AppRoleBLL));
            services.AddTransient(typeof(ICaseClassBLL), typeof(CaseClassBLL));
            services.AddTransient(typeof(IDeptBLL), typeof(DeptBLL));
            services.AddTransient(typeof(IDictCategoryBLL), typeof(DictCategoryBLL));
            services.AddTransient(typeof(IDictItemBLL), typeof(DictItemBLL));
            services.AddTransient(typeof(IGridBLL), typeof(GridBLL));
            services.AddTransient(typeof(IHolidayBLL), typeof(HolidayBLL));
            services.AddTransient(typeof(IPhoneGroupBLL), typeof(PhoneGroupBLL));
            services.AddTransient(typeof(IPhoneGroupItemBLL), typeof(PhoneGroupItemBLL));
            services.AddTransient(typeof(IQSRoleBLL), typeof(QSRoleBLL));
            services.AddTransient(typeof(IUserBLL), typeof(UserBLL));
            services.AddTransient(typeof(IVehicleBLL), typeof(VehicleBLL));
            services.AddTransient(typeof(IMonitorBLL), typeof(MonitorBLL));
        }
    }
}