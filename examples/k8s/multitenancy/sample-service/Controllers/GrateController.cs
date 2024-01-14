using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using grate.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SampleService.Extension;
namespace Controllers;

[Route("api/[controller]")]
[ApiController]
public class Grate : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Hello([FromServices] GrateConfiguration grateConfiguration, [FromQuery] string databaseName)
    {
        grateConfiguration.SwitchDatabase(databaseName);
        var dbConnection = new SqlConnection(grateConfiguration.ConnectionString);
        var query = "select id, name from grate_test";
        var result = await dbConnection.QueryAsync<(int, string)>(query);
        return new OkObjectResult(result.Select(r => new { id = r.Item1, name = r.Item2 }).ToArray());
    }
}
