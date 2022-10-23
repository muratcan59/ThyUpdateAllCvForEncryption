using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Owin;
using System;

namespace ThyUpdateAllCvForEncryption
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(config =>
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseDefaultTypeSerializer()
                    .UseMemoryStorage());

            services.AddHangfireServer();
        }

        public void Configure(
            IApplicationBuilder app,
            IRecurringJobManager recurringJobManager)
        {
            app.UseRouting();

            app.UseWelcomePage("/");
            app.UseHangfireDashboard();
         
            var operationService = new OperationService();
            operationService.UpdateAllCv();
            RecurringJob.AddOrUpdate(nameof(OperationService.UpdateAllCv), () => operationService.UpdateAllCv(), "0 */1 * * *");
          
        }
    }
}
