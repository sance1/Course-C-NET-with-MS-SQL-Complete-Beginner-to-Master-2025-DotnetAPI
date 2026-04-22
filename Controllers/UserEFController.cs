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
            cfg.CreateMap<UserSalary,UserSalary>();
            cfg.CreateMap<UserJobInfo,UserJobInfo>();
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

    
    [HttpGet("GetUserSalary/{userId}")]
    public IEnumerable<UserSalary> GetUserSalaryEf(int userId)
    {
        return _entityFramework.UserSalary
            .Where(us => us.UserId == userId)
            .ToList();
    }
    [HttpPost("UserSalary")]
    public IActionResult PostUserSalaryEf(UserSalary userSalaryForInsert)
    {
        _entityFramework.UserSalary.Add(userSalaryForInsert);
        if(_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        }
        throw new Exception("Adding user salary failed on save");
    }
    [HttpPut("UserSalary")]
    public IActionResult PutUserSalary(UserSalary userSalaryForUpdate)
    {
        UserSalary? userSalaryToUpdate = _entityFramework.UserSalary
            .Where(us => us.UserId == userSalaryForUpdate.UserId)
            .FirstOrDefault();

        if (userSalaryToUpdate != null)
        {
            
            _mapper.Map(userSalaryForUpdate,userSalaryToUpdate);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception("Updating user salary failed on save");
        }
        throw new Exception("Failed to get user salary");
    }
    [HttpDelete("UserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        UserSalary? userToDelete = _entityFramework.UserSalary
            .Where(us => us.UserId == userId)
            .FirstOrDefault();

        if (userToDelete != null)
        {
            _entityFramework.UserSalary.Remove(userToDelete);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception("Deleting user salary failed on save");
        }
        throw new Exception("Failed to find user salary to delete");
    }
    [HttpGet("UserJobInfo/{userId}")]
    public IEnumerable<UserJobInfo> GetUserJobInfoEf(int userId)
    {
        return _entityFramework.UserJobInfos
            .Where(uj => uj.UserId == userId)
            .ToList();
    }
    [HttpPost("UserJobInfo")]
    public IActionResult PostUserJobInfo(UserJobInfo userJobInfoForInsert)
    {
        _entityFramework.UserJobInfos.Add(userJobInfoForInsert  );
        if(_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        }
        throw new Exception("Adding user job info failed on save");
    }
    [HttpPut("UserJobInfo")]
    public IActionResult PutUserJobInfoEf(UserJobInfo userJobInfoForUpdate)
    {
        UserJobInfo? userToUpdate = _entityFramework.UserJobInfos
            .Where(uj => uj.UserId == userJobInfoForUpdate.UserId)
            .FirstOrDefault();

        if (userToUpdate != null)
        {
            _mapper.Map(userJobInfoForUpdate,userToUpdate);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception("Updating user job info failed on save");
        }
        throw new Exception("Failed to find user job info to update");
    }
    [HttpDelete("DeleteUserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfoEf(int userId)
    {
        UserJobInfo? userToDelete = _entityFramework.UserJobInfos
            .Where(uj => uj.UserId == userId)
            .FirstOrDefault();

        if (userToDelete != null)
        {
            _entityFramework.UserJobInfos.Remove(userToDelete);
            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception("Deleting user job info failed on save");
        }
        throw new Exception("Failed to find user job info to delete");
    }

}
