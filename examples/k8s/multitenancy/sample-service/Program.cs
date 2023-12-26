using grate;
using grate.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddControllers();
builder.Services.AddGrate(grateBuilder =>
{
    grateBuilder.WithAdminConnectionString(configuration.GetConnectionString("AdminConnection")!);
    grateBuilder.WithConnectionString(configuration.GetConnectionString("DefaultConnection")!);
    grateBuilder.WithSqlFilesDirectory("/db");
    grateBuilder.UseSqlServer();
});
var app = builder.Build();
app.UseRouting();
app.MapControllers();
app.Run();
