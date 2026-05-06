using DotnetApi.Models;
using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class PostController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    public PostController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("Posts")]
    public IEnumerable<Post> GetPosts()
    {
        string sql = @"SELECT [PostId]
                    ,[UserId]
                    ,[PostTitle]
                    ,[PostContent]
                    ,[PostCreated]
                    ,[LastUpdated]
                    FROM [DotNetCourseDatabase].[TutorialAppSchema].[Post]";
        return _dapper.LoadData<Post>(sql);

    }
    [HttpGet("PostSingle/{postId}")]
    public Post GetPostSingle(int postId)
    {
        string sql = @"SELECT [PostId]
                    ,[UserId]
                    ,[PostTitle]
                    ,[PostContent]
                    ,[PostCreated]
                    ,[LastUpdated]
                    FROM [DotNetCourseDatabase].[TutorialAppSchema].[Post]
                    WHERE PostId = " + postId.ToString();
        return _dapper.LoadDataSingle<Post>(sql);
    }

    [HttpGet("PostsByUser/{userId}")]
    public IEnumerable<Post> GetPostsByUser(int userId)
    {
        string sql = @"SELECT [PostId]
                    ,[UserId]
                    ,[PostTitle]
                    ,[PostContent]
                    ,[PostCreated]
                    ,[LastUpdated]
                    FROM [DotNetCourseDatabase].[TutorialAppSchema].[Post]
                    WHERE UserId = " + userId.ToString();
        return _dapper.LoadData<Post>(sql);
    }
    [HttpGet("MyPosts")]
    public IEnumerable<Post> GetMyPosts()
    {
        string sql = @"SELECT [PostId]
                    ,[UserId]
                    ,[PostTitle]
                    ,[PostContent]
                    ,[PostCreated]
                    ,[LastUpdated]
                    FROM [DotNetCourseDatabase].[TutorialAppSchema].[Post]
                    WHERE UserId = " + this.User.FindFirst("userId")?.Value;
                    
        return _dapper.LoadData<Post>(sql);
    }

    [HttpGet("PostsBySearch/{searchParam}")]
    public IEnumerable<Post> PostsBysearch(string searchParam)
    {
        string sql = @"SELECT [PostId]
                    ,[UserId]
                    ,[PostTitle]
                    ,[PostContent]
                    ,[PostCreated]
                    ,[LastUpdated]
                    FROM [DotNetCourseDatabase].[TutorialAppSchema].[Post]
                    WHERE PostTitle like '%" + searchParam + 
                    "%' OR PostContent like '%" + searchParam + "%'";
                    
        return _dapper.LoadData<Post>(sql);
    }

    [HttpPost("Post")]
    public IActionResult AddPost(PostToAddDto postToAdd)
    {
        string sql = @"
                    INSERT INTO [DotNetCourseDatabase].[TutorialAppSchema].[Post](
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [LastUpdated]) VALUES(" + this.User.FindFirst("userId")?.Value 
                    + ",'" + postToAdd.PostTitle 
                    + "','" + postToAdd.PostContent 
                    + "', GETDATE(), GETDATE())";

        if (_dapper.ExecuteSql(sql)){
            return Ok();    
        }
        throw new Exception("Failed to create new post"); 
    }
    [HttpPut("Post")]
    public IActionResult EditPost(PostToEditDto postToEdit)
    {
        string sql = @"
                    UPDATE TutorialAppSchema.Post
                    Set PostContent = '" + postToEdit.PostContent + 
                    "',  PostTitle = '"+ postToEdit.PostTitle + 
                    @"', LastUpdated = GETDATE()  
                    Where PostId = "+ postToEdit.PostId.ToString() +
                    " AND UserId = " + this.User.FindFirst("userId")?.Value; 
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();    
        }
        throw new Exception("Failed to edit post");        
    }
    [HttpDelete("Post/{postId}")]
    public IActionResult DeletePost(int postId)
    {
        string sql = @"DELETE FROM TutorialAPPSchema.Post
        WHERE PostId = " + postId.ToString() + 
         " AND UserId = " + this.User.FindFirst("userId")?.Value;
        if(_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed to delete post!");
    }

}