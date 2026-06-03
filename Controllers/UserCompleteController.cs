using Dapper;
using DotnetApi.Models;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class UserComplete : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly ReusableSql _reusableSql;
    public UserComplete(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _reusableSql = new ReusableSql(config);
        //Console.WriteLine(config.GetConnectionString("DefaultConnection"));
    }
    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }
    [HttpGet("GetUsers/{userId}/{isActive}")]
    public IEnumerable<DotnetAPI.Models.UserComplete> GetUsers(int userId, bool isActive)
    {
        string sql = @"EXEC TutorialAppSchema.spUsers_Get"; 
        //string parameters = "";
        string stringParameters = "";
        DynamicParameters sqlParameters = new DynamicParameters();
        if(userId != 0) 
        {
            //parameters += ", @UserId = " + userId.ToString();
            stringParameters += ", @UserId=@UserIdParameter";
            sqlParameters.Add("@UserIdParameter", userId, System.Data.DbType.Int32);
        }
        if(isActive)
        {
            //parameters += ", @Active = " + isActive.ToString();
            stringParameters += ", @Active=@ActiveParameter";
            sqlParameters.Add("@ActiveParameter", isActive, System.Data.DbType.Boolean);
        }
        //sql += parameters.Substring(1); //, parameters.Length); 
        if(stringParameters.Length > 1)
        {
            sql += stringParameters.Substring(1);
        }

        //Console.WriteLine(sql);

        IEnumerable<DotnetAPI.Models.UserComplete> users = _dapper.LoadDataWithParameters<DotnetAPI.Models.UserComplete>(sql, sqlParameters);
        return users;
    }
    
    [HttpPut("UpsertUser")]
    public IActionResult UpsertUser(DotnetAPI.Models.UserComplete user)
    {        
        // string sql = @"EXEC TutorialAppSchema.spUser_Upsert 
        //     @FirstName = '" + user.FirstName + 
        //     "', @LastName = '" + user.LastName + 
        //     "', @Email = '" + user.Email + 
        //     "', @Gender = '" + user.Gender + 
        //     "', @JobTitle = '" + user.JobTitle + 
        //     "', @Department = '" + user.Department + 
        //     "', @Salary = " + user.Salary + 
        //     "', @Active = '" + user.Active + 
        //     "', @UserId =" + user.UserId;
        //Console.WriteLine(sql);
        //this code use because Dynamic parameters and the other side in dbusing store procedure
        // string sql = @"EXEC TutorialAppSchema.spUser_Upsert 
        //     @FirstName = @FirstNameParameter, 
        //     @LastName = @LastNameParameter, 
        //     @Email = @EmailParameter, 
        //     @Gender = @GenderParameter, 
        //     @JobTitle = @JobTitleParameter, 
        //     @Department = @DepartmentParameter, 
        //     @Salary = @SalaryParameter, 
        //      @Active = @ActiveParameter, 
        //      @UserId = @UserIdParameter";

        // DynamicParameters sqlParameters = new DynamicParameters();

        // sqlParameters.Add("@FirstNameParameter", user.FirstName, System.Data.DbType.String);
        // sqlParameters.Add("@LastNameParameter", user.LastName, System.Data.DbType.String);
        // sqlParameters.Add("@EmailParameter", user.Email, System.Data.DbType.String);
        // sqlParameters.Add("@GenderParameter", user.Gender, System.Data.DbType.String);
        // sqlParameters.Add("@JobTitleParameter", user.JobTitle, System.Data.DbType.String);
        // sqlParameters.Add("@DepartmentParameter", user.Department, System.Data.DbType.String);
        // sqlParameters.Add("@SalaryParameter", user.Salary, System.Data.DbType.Decimal);
        // sqlParameters.Add("@ActiveParameter", user.Active, System.Data.DbType.Boolean);
        // sqlParameters.Add("@UserIdParameter", user.UserId, System.Data.DbType.Int32);

        // if(_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
        // {
        //     return Ok();
        // }
        if(_reusableSql.UpsertUser(user))
        {
            return Ok();
        }
        throw new Exception("Failed to update user");
    }
    
    //Before Dynamic parameters
    // [HttpDelete("DeleteUser/{userId}")]
    // public IActionResult DeleteUser(int userId)
    // {
    //      string sql = @"EXEC TutorialAppSchema.spUser_Delete 
    //                     @UserId = " + userId.ToString();
    //     if(_dapper.ExecuteSql(sql))
    //     {
    //         return Ok();
    //     }
    //     throw new Exception("Failed to delete user");
    // }
    //After Dynamic parameters
    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
         string sql = @"EXEC TutorialAppSchema.spUser_Delete 
                        @UserId = @UserIdParameter" ;

                DynamicParameters sqlParameters = new DynamicParameters();
                sqlParameters.Add("@UserIdParameter", userId, System.Data.DbType.Int32);
        if(_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
        {
            return Ok();
        }
        throw new Exception("Failed to delete user");
    }

    [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
    public IEnumerable<Post> GetPosts(int postId, int userId, string searchParam)
    {
        string sql = @"EXEC TutorialAppSchema.spPosts_Get"; 
        string stringParameters = "";
        
        DynamicParameters sqlParameters = new DynamicParameters();
        
        if(postId != 0)
        {
            stringParameters += ", @PostId=@PostIdParameter";
            sqlParameters.Add("@PostIdParameter", postId, System.Data.DbType.Int32);
        }
        if(userId != 0) 
        {
            stringParameters += ", @UserId=@UserIdParameter";
            sqlParameters.Add("@UserIdParameter", userId, System.Data.DbType.Int32);
        }
        if(searchParam.ToLower() != "none")
        {
            stringParameters += ", @SearchValue=@SearchValueParameter";
            sqlParameters.Add("@SearchValueParameter", searchParam, System.Data.DbType.String);            
        }
        if(stringParameters.Length > 0)
        {
            sql += stringParameters.Substring(1);
        }
        return _dapper.LoadDataWithParameters<Post>(sql, sqlParameters);
    }

    [HttpGet("MyPosts")]
    public IEnumerable<Post> GetMyPosts()
    {
        string sql = @"EXEC TutorialAppSchema.spPosts_Get @UserId = @UserIdParameter";
        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, System.Data.DbType.Int32);
        return _dapper.LoadDataWithParameters<Post>(sql, sqlParameters);
    }
    [HttpPut("UpsertPost")]
    public IActionResult UpsertPost(Post postToUpsert)
    {
           string sql = @"EXEC TutorialAppSchema.spPosts_Upsert 
            @UserId = @UserIdParameter, 
            @PostTitle = @PostTitleParameter, 
            @PostContent = @PostContentParameter";

        DynamicParameters sqlParameters = new DynamicParameters();

        sqlParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, System.Data.DbType.Int32);
        sqlParameters.Add("@PostTitleParameter", postToUpsert.PostTitle, System.Data.DbType.String);
        sqlParameters.Add("@PostContentParameter", postToUpsert.PostContent, System.Data.DbType.String);
        
        if(postToUpsert.PostId > 0)
        {
            sql += ", @PostId = @PostIdParameter";
            sqlParameters.Add("@PostIdParameter", postToUpsert.PostId, System.Data.DbType.Int32);
        }
        if(_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
        {
            return Ok();
        }
        throw new Exception("Failed to upsert post");
    }

    [HttpDelete("Post/{postId}")]
    public IActionResult DeletePost(int postId)
    {
         string sql = @"EXEC TutorialAppSchema.spPosts_Delete 
                        @UserId = @UserIdParameter,
                        @PostId = @PostIdParameter" ;

                DynamicParameters sqlParameters = new DynamicParameters();
                sqlParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, System.Data.DbType.Int32);
                sqlParameters.Add("@PostIdParameter", postId, System.Data.DbType.Int32);                

        if(_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
        {
            return Ok();
        }
        throw new Exception("Failed to delete post");
    }

    
}
