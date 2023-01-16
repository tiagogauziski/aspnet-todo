using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using TodoList.UI.MVC.Extentions;
using TodoList.UI.MVC.HealthChecks;
using TodoList.UI.MVC.Options;
using TodoList.UI.MVC.TodoApiClient;

namespace TodoList.UI.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.Configure<TodoApiOptions>(builder.Configuration.GetSection(TodoApiOptions.TodoApi));
            builder.Services.AddHttpClient<ITodoApiClient, TodoApiClient.TodoApiClient>();
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            builder.Services.AddControllersWithViews();
            builder.Services.AddHealthChecks()
                .AddCheck<TodoApiHealthCheck>("TodoApi");

            var application = builder.Build();
            application.UseForwardedHeaders();

            // Configure the HTTP request pipeline.
            if (!application.Environment.IsDevelopment())
            {
                application.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                application.UseHsts();
            }

            application.UseHttpsRedirection();
            application.UseStaticFiles();

            application.UseRouting();

            application.UseAuthorization();

            application.MapDebugPage("/Debug");

            application.MapControllerRoute(
                name: "default",
                pattern: "{controller=Todo}/{action=Index}/{id?}");

            application.MapHealthChecks("/healthz/ready", new HealthCheckOptions()
            {
                Predicate = _ => true
            });

            application.MapHealthChecks("/healthz/live", new HealthCheckOptions()
            {
                Predicate = _ => false
            });

            application.Run();
        }
    }
}