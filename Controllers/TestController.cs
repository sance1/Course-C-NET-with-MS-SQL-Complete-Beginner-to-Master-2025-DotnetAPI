using Dapper;
using DotnetApi.Models;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class Test : ControllerBase
{
    private readonly DataContextDapper _dapper;

    public Test(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        //Console.WriteLine(config.GetConnectionString("DefaultConnection"));
    }
    [HttpGet("Connection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }
    [HttpGet]
    public string TestOk()
    {
        return "Your Application is up and running!";
    }

    
}
