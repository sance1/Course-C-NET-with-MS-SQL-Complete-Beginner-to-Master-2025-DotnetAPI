using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
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
    public IActionResult AddUser(UserToAddDto user)
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

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"DELETE FROM [TutorialAppSchema].[Users]
                    WHERE UserId = " + userId.ToString();
        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed to delete user");
    }

    [HttpGet("UserSalary/{userId}")]
    public IEnumerable<UserSalary> GetUserSalary(int userId)
    {
        return _dapper.LoadData<UserSalary>(@"
            SELECT [UserSalary].[UserId], [UserSalary].[Salary]
            FROM [DotNetCourseDatabase].[TutorialAppSchema].[UserSalary]
            WHERE UserId = " + userId.ToString());
    }
    
    [HttpPost("UserSalary")]
    public IActionResult PostUserSalary(UserSalary userSalaryForInsert)
    {
        string sql = @"
                    INSERT INTO [TutorialAppSchema].[UserSalary]
                            ([UserId], 
                            [Salary])
                        VALUES
                            (" + userSalaryForInsert.UserId 
                            + ", " + userSalaryForInsert.Salary 
                            + ")";
        if(_dapper.ExecuteSqlWithRowCount(sql) > 0)
        {
            return Ok(userSalaryForInsert);

        }
        throw new Exception("Adding user salary failed on save");                    
    }

    [HttpPut("UserSalary")] 
    public IActionResult PutUserSalary(UserSalary userSalaryForUpdate)
    {
        string sql = "UPDATE [TutorialAppSchema].[UserSalary] SET [Salary] = " 
                      + userSalaryForUpdate.Salary 
                      + " WHERE UserId = " + userSalaryForUpdate.UserId.ToString();
        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Updating user salary failed on save");        
    }

    [HttpDelete("UserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        string sql = "DELETE FROM [TutorialAppSchema].[UserSalary] WHERE UserId = " + userId.ToString();
        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("deleting user salary failed on save");
    }

    [HttpGet("UserJobInfo/{userId}")]
    public IEnumerable<UserJobInfo> GetUserJobInfo(int userId)
    {
        return _dapper.LoadData<UserJobInfo>(@"
            SELECT [UserJobInfo].[UserId], [UserJobInfo].[JobTitle], [UserJobInfo].[Department]
            FROM [DotNetCourseDatabase].[TutorialAppSchema].[UserJobInfo]
            WHERE UserId = " + userId.ToString());
    }   

    [HttpPost("UserJobInfo")]
    public IActionResult PostUserJobInfo(UserJobInfo userJobInfoForInsert)
    {
        string sql = @"
                    INSERT INTO [TutorialAppSchema].[UserJobInfo]
                            ([UserId], 
                            [JobTitle], 
                            [Department])
                        VALUES
                            (" + userJobInfoForInsert.UserId 
                            + ", '" + userJobInfoForInsert.JobTitle + 
                            "', '" + userJobInfoForInsert.Department + "')";
        if(_dapper.ExecuteSql(sql))
        {
            return Ok(userJobInfoForInsert);
        }
        throw new Exception("Adding user job info failed on save");                    
    }

    [HttpPut("UserJobInfo")]
    public IActionResult PutUserJobInfo(UserJobInfo userJobInfoForUpdate)
    {
        string sql = "UPDATE [TutorialAppSchema].[UserJobInfo] SET [JobTitle] = '" 
                      + userJobInfoForUpdate.JobTitle 
                      + "', [Department] = '" + userJobInfoForUpdate.Department 
                      + "' WHERE UserId = " + userJobInfoForUpdate.UserId.ToString();
        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Updating user job info failed on save");        
    }
    [HttpDelete("UserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfo(int userId)
    {
        string sql = "DELETE FROM [TutorialAppSchema].[UserJobInfo] WHERE UserId = " + userId.ToString();
        Console.WriteLine(sql);
        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Deleting user job info failed on save");
    }
}
