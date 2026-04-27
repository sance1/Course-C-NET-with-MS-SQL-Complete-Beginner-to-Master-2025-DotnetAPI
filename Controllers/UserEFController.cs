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
    // DataContextEF _entityFramework;
    IUserRepository _userRepository;
    Mapper _mapper;
    public UserEFController(IConfiguration config, IUserRepository userRepository)
    {
        // _entityFramework = new DataContextEF(config);
        _userRepository = userRepository;

        _mapper = new Mapper(new MapperConfiguration(cfg =>{
            cfg.CreateMap<UserToAddDto,User>();
            cfg.CreateMap<UserSalary,UserSalary>();
            cfg.CreateMap<UserJobInfo,UserJobInfo>();
        },NullLoggerFactory.Instance));
    }
    // NullLoggerFactory.Instance => ini karna mapper baru/ yang individu klw perusahaan biasanya berbayar
    //_entityFramework.SaveChanges(); => From DbContext
    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        //IEnumerable<User> users = _entityFramework.Users.ToList();
        IEnumerable<User> users = _userRepository.GetUsers();
        return users;
 
    }
    [HttpGet("GetSingleUsers/{userId}")]
    public User GetSingleUsers(int userId)
    {
        // User? user = _entityFramework.Users
        //     .Where(u => u.UserId == userId )
        //     .FirstOrDefault();
        
        // if(user != null)
        // {
        //     return user; 
        // }
        // throw new Exception("Failed to get user");
        return _userRepository.GetSingleUsers(userId);
    }
    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        // User? userDb = _entityFramework.Users
        //     .Where(u => u.UserId == user.UserId )
        //     .FirstOrDefault();
        User? userDb = _userRepository.GetSingleUsers(user.UserId);

        if(userDb != null)
        {
            userDb.FirstName = user.FirstName;
            userDb.LastName = user.LastName;
            userDb.Email = user.Email;
            userDb.Active = user.Active;
            userDb.Gender = user.Gender;
            if(_userRepository.SaveChanges())
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

        //_entityFramework.Users.Add(userDb);
        _userRepository.AddEntity<User>(userDb);
        if(_userRepository.SaveChanges())
        {
            return Ok();
        }
        throw new Exception("Failed to add user");
        
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        // User? userDb = _entityFramework.Users
        //     .Where(u => u.UserId == userId )
        //     .FirstOrDefault();
        
        User? userDb = _userRepository.GetSingleUsers(userId);
        if(userDb != null)
        {
            //_entityFramework.Users.Remove(userDb);
            _userRepository.RemoveEntity<User>(userDb);
            //_entityFramework.SaveChanges();
            if(_userRepository.SaveChanges())
            {
                return Ok();
            }
             throw new Exception("Failed to Delete user");
        }
        throw new Exception("Failed to get user");
    }

    
    [HttpGet("GetUserSalary/{userId}")]
    public UserSalary GetUserSalaryEf(int userId)
    {
        return _userRepository.GetSingleUsersSalary(userId);
        // return _entityFramework.UserSalary
        //     .Where(us => us.UserId == userId)
        //     .ToList();
    }
    [HttpPost("UserSalary")]
    public IActionResult PostUserSalaryEf(UserSalary userSalaryForInsert)
    {
        //_entityFramework.UserSalary.Add(userSalaryForInsert);
        _userRepository.AddEntity<UserSalary>(userSalaryForInsert); 
        if(_userRepository.SaveChanges())
        {
            return Ok();
        }
        throw new Exception("Adding user salary failed on save");
    }
    [HttpPut("UserSalary")]
    public IActionResult PutUserSalaryEf(UserSalary userSalaryForUpdate)
    {
        // UserSalary? userSalaryToUpdate = _entityFramework.UserSalary
        //     .Where(us => us.UserId == userSalaryForUpdate.UserId)
        //     .FirstOrDefault();
        UserSalary? userSalaryToUpdate = _userRepository.GetSingleUsersSalary(userSalaryForUpdate.UserId);
        if (userSalaryToUpdate != null)
        {
            
            _mapper.Map(userSalaryForUpdate,userSalaryToUpdate);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Updating user salary failed on save");
        }
        throw new Exception("Failed to get user salary");
    }
    [HttpDelete("UserSalary/{userId}")]
    public IActionResult DeleteUserSalaryEf(int userId)
    {
        // UserSalary? userToDelete = _entityFramework.UserSalary
        //     .Where(us => us.UserId == userId)
        //     .FirstOrDefault();
        UserSalary? userToDelete = _userRepository.GetSingleUsersSalary(userId);
        if (userToDelete != null)
        {
            //_entityFramework.UserSalary.Remove(userToDelete);
            _userRepository.RemoveEntity<UserSalary>(userToDelete);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Deleting user salary failed on save");
        }
        throw new Exception("Failed to find user salary to delete");
    }
    [HttpGet("UserJobInfo/{userId}")]
    public UserJobInfo GetUserJobInfoEf(int userId)
    {
        // return _entityFramework.UserJobInfo
        //     .Where(uj => uj.UserId == userId)
        //     .ToList();
        return _userRepository.GetSingleUsersJobInfo(userId);
    }
    [HttpPost("UserJobInfo")]
    public IActionResult PostUserJobInfo(UserJobInfo userJobInfoForInsert)
    {
        //_entityFramework.UserJobInfos.Add(userJobInfoForInsert  );
        _userRepository.AddEntity<UserJobInfo>(userJobInfoForInsert);
        if(_userRepository.SaveChanges())
        {
            return Ok();
        }
        throw new Exception("Adding user job info failed on save");
    }
    [HttpPut("UserJobInfo")]
    public IActionResult PutUserJobInfoEf(UserJobInfo userJobInfoForUpdate)
    {
        // UserJobInfo? userToUpdate = _entityFramework.UserJobInfo
        //     .Where(uj => uj.UserId == userJobInfoForUpdate.UserId)
        //     .FirstOrDefault();
        UserJobInfo? userToUpdate = _userRepository.GetSingleUsersJobInfo(userJobInfoForUpdate.UserId);
        if (userToUpdate != null)
        {
            _mapper.Map(userJobInfoForUpdate,userToUpdate);
            if (_userRepository.SaveChanges())
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
        // UserJobInfo? userToDelete = _entityFramework.UserJobInfo
        //     .Where(uj => uj.UserId == userId)
        //     .FirstOrDefault();

        UserJobInfo? userToDelete = _userRepository.GetSingleUsersJobInfo(userId);

        if (userToDelete != null)
        {
            //_entityFramework.UserJobInfos.Remove(userToDelete);
            _userRepository.RemoveEntity<UserJobInfo>(userToDelete);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Deleting user job info failed on save");
        }
        throw new Exception("Failed to find user job info to delete");
    }

}
