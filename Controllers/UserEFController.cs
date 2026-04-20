using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]

public class UserEFController : ControllerBase
{
    DataContextEF _entityFramework;
    Mapper _mapper;
    public UserEFController(IConfiguration config)
    {
        _entityFramework = new DataContextEF(config);

        _mapper = new Mapper(new MapperConfiguration(cfg =>{
            cfg.CreateMap<UserToAddDto,User>();
        },NullLoggerFactory.Instance));
    }
    // NullLoggerFactory.Instance => ini karna mapper baru/ yang individu klw perusahaan biasanya berbayar
    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = _entityFramework.Users.ToList();
        return users;
 
    }
    [HttpGet("GetSingleUsers/{userId}")]
    public User GetSingleUsers(int userId)
    {
        User? user = _entityFramework.Users
            .Where(u => u.UserId == userId )
            .FirstOrDefault();
        
        if(user != null)
        {
            return user; 
        }
        throw new Exception("Failed to get user");
    }
    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userDb = _entityFramework.Users
            .Where(u => u.UserId == user.UserId )
            .FirstOrDefault();
        
        if(userDb != null)
        {
            userDb.FirstName = user.FirstName;
            userDb.LastName = user.LastName;
            userDb.Email = user.Email;
            userDb.Active = user.Active;
            userDb.Gender = user.Gender;
            if(_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
             throw new Exception("Failed to update user");
        }
        throw new Exception("Failed to get user");

    }

    [HttpPost]
    public IActionResult AddUser(UserToAddDto user)
    {
        User? userDb = _mapper.Map<User>(user);
        // User? userDb = new User();
    
        // userDb.FirstName = user.FirstName;
        // userDb.LastName = user.LastName;
        // userDb.Email = user.Email;
        // userDb.Active = user.Active;
        // userDb.Gender = user.Gender;

        _entityFramework.Users.Add(userDb);
        if(_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        }
        throw new Exception("Failed to add user");
        
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        User? userDb = _entityFramework.Users
            .Where(u => u.UserId == userId )
            .FirstOrDefault();
        
        if(userDb != null)
        {
            _entityFramework.Users.Remove(userDb);
            if(_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
             throw new Exception("Failed to Delete user");
        }
        throw new Exception("Failed to get user");
    }

}
