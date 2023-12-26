using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dapper;
using grate.Configuration;
using grate.Migration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SampleService.Extension;
namespace Controllers;

[Route("api/[controller]")]
[ApiController]
public class TenantController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromServices] GrateConfiguration grateConfiguration, [FromServices] IGrateMigrator grateMigrator)
    {
        var newDatabaseId = $"tenant_{Guid.NewGuid():N}";
        // swith to new connectionstring
        grateConfiguration.SwitchDatabase(newDatabaseId);

        // consider to use hosted service to run this in background if you care about the performance
        await grateMigrator.Migrate();
        return new OkObjectResult(new { id = newDatabaseId });
    }
}
