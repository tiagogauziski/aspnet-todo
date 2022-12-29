using Microsoft.EntityFrameworkCore;
using TodoList.API.Infrastructure.Database;

namespace TodoList.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<TodoContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("TodoListDatabase")));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var application = builder.Build();

            // Ensure database gets created
            using (var scope = application.Services.CreateScope())
            {
                var databaseContext = scope.ServiceProvider.GetRequiredService<TodoContext>();
                databaseContext.Database.EnsureCreated();
            };

            // Configure the HTTP request pipeline.
            if (application.Environment.IsDevelopment())
            {
                application.UseSwagger();
                application.UseSwaggerUI();
            }

            application.UseHttpsRedirection();

            application.UseAuthorization();

            application.MapControllers();

            application.Run();
        }
    }
}