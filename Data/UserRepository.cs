using DotnetAPI.Models;

namespace DotnetAPI.Data;

public class UserRepository : IUserRepository
{
    DataContextEF _entityFramework;
    public UserRepository(IConfiguration config)
    {
        _entityFramework = new DataContextEF(config);
    }
    public bool SaveChanges()
    {
        return _entityFramework.SaveChanges() > 0 ;
    }
     public void AddEntity<T>(T entityToAdd)
    // public bool AddEntity<T>(T entityToAdd)
    {
        if(entityToAdd != null)
        {
            _entityFramework.Add(entityToAdd);
            // return true;
        }
        // return false;
    }
    public void RemoveEntity<T>(T entityToRemove)
    {
        if(entityToRemove != null)
        {
            _entityFramework.Remove(entityToRemove);
        }
    }


    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = _entityFramework.Users.ToList();
        return users;
 
    }

    public User GetSingleUsers(int userId)
    {
        User? user = _entityFramework.Users
            .Where(u => u.UserId == userId )
            .FirstOrDefault<User>();
        //FirstOrDefault(u => u.UserId == userId); terbaru      
        /// <User> untuk userlama 
        if(user != null)
        {
            return user; 
        }
        throw new Exception("Failed to get user");
    }

    public UserSalary GetSingleUsersSalary(int userId)
    {
        UserSalary? userSalary = _entityFramework.UserSalary
            .Where(us => us.UserId == userId)
            .FirstOrDefault<UserSalary>();
        
        if(userSalary != null)
        {
            return userSalary; 
        }
        throw new Exception("Failed to get user");
    }

    public UserJobInfo GetSingleUsersJobInfo(int userId)
    {
        UserJobInfo? userJobInfo = _entityFramework.UserJobInfo
            .Where(uj => uj.UserId == userId)
            .FirstOrDefault<UserJobInfo>();
        
        if(userJobInfo != null)
        {
            return userJobInfo; 
        }
        throw new Exception("Failed to get user");
    }

}