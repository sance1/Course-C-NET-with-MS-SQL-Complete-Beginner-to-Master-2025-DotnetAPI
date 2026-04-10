using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class UserController : ControllerBase
{
    DataContextDapper _dapper;

    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        //Console.WriteLine(config.GetConnectionString("DefaultConnection"));
    }
    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }
    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        string sql = @"SELECT [UserId]
                    ,[FirstName]
                    ,[LastName]
                    ,[Email]
                    ,[Gender]
                    ,[Active]
                    FROM [DotNetCourseDatabase].[TutorialAppSchema].[Users]";
        IEnumerable<User> users = _dapper.LoadData<User>(sql);
        return users;
    }
    [HttpGet("GetSingleUsers/{userId}")]
    public User GetSingleUsers(int userId)
    {
         string sql = @"SELECT [UserId]
                    ,[FirstName]
                    ,[LastName]
                    ,[Email]
                    ,[Gender]
                    ,[Active]
                    FROM [DotNetCourseDatabase].[TutorialAppSchema].[Users]
                    WHERE UserId = " + userId.ToString();
        User user = _dapper.LoadDataSingle<User>(sql);
        return user;    
    }
    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string sql = @"UPDATE TutorialAppSchema.Users
                SET [FirstName] = '" + user.FirstName + 
                "',[LastName] = '" + user.LastName + 
                "',[Email] = '" + user.Email + 
                 "',[Gender] = '" + user.Gender + 
                 "',[Active] = '" + user.Active + 
                 "' where [UserId] =" + user.UserId;
        Console.WriteLine(sql);
        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed to update user");
    }

    [HttpPost]
    public IActionResult AddUser(User user)
    {
        string sql = @"
                    INSERT INTO [TutorialAppSchema].[Users]
                            ([FirstName]
                            ,[LastName]
                            ,[Email]
                            ,[Gender]
                            ,[Active])
                        VALUES
                            ("+ "'" + user.FirstName +
                            "','" + user.LastName +
                            "','" + user.Email +
                            "','" + user.Gender +
                            "','" + user.Active + 
                            "')";
        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed to add user");
    }

}
