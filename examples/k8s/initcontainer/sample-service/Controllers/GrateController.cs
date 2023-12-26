using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
namespace Controllers;

[Route("api/[controller]")]
[ApiController]
public class Grate : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Hello([FromServices] IConfiguration configuration)
    {
        var connectionString = configuration["ConnectionStrings:DefaultConnection"] ?? throw new Exception("No connection string found in appsettings");
        var dbConnection = new SqlConnection(connectionString);
        var query = "select id, name from grate_test";
        var result = await dbConnection.QueryAsync<(int, string)>(query);
        return new OkObjectResult(result.Select(r => new { id = r.Item1, name = r.Item2 }).ToArray());
    }

}
