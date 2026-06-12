using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Dapper;
using DotnetAPI.Models;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    // private readonly IConfiguration _config;
    private readonly AuthHelpers _authHelpers;
    private readonly ReusableSql _reusableSql;
    private readonly IMapper _mapper;
    public AuthController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        // _config = config;
        _authHelpers = new AuthHelpers(config);
        _reusableSql = new ReusableSql(config);
        _mapper = new Mapper(new MapperConfiguration(cfg => {
            cfg.CreateMap<UserForRegistrationDto, UserComplete>();
        }, NullLoggerFactory.Instance));      
    }
     
    [AllowAnonymous]
    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDto userForRegistrationDto)
    {
        if(userForRegistrationDto.Password == userForRegistrationDto.PasswordConfirm)
        {
            string sqlCheckUserExists = "Select Email From TutorialAppSchema.Auth Where Email = '" + 
                userForRegistrationDto.Email + "'";  
            IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
            if(existingUsers.Count() == 0)
            {
                // byte[] passwordSalt = new byte[128 / 8];
                // using(RandomNumberGenerator rng = RandomNumberGenerator.Create())
                // {
                //     rng.GetNonZeroBytes(passwordSalt);
                // }

                //     // byte[] passwordHash = GetPasswordHash(userForRegistrationDto.Password, passwordSalt);
                //     byte[] passwordHash = _authHelpers.GetPasswordHash(userForRegistrationDto.Password, passwordSalt);
                //     // string sqlAddAuth = @"INSERT INTO TutorialAppSchema.Auth ([Email],
                //     // [PasswordHash],[PasswordSalt]) VALUES ('" + userForRegistrationDto.Email + "', @PasswordHash, @PasswordSalt)"; 
                //     string sqlAddAuth = @"EXEC TutorialAppSchema.spRegistration_Upsert
                //                     @Email = @EmailParam, 
                //                     @PasswordHash = @PasswordHashParam, 
                //                     @PasswordSalt = @PasswordSaltParam";

                // List<SqlParameter> sqlParameters = new List<SqlParameter>();
                
                // // SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", System.Data.SqlDbType.VarBinary);
                // // passwordSaltParameter.Value = passwordSalt;
                // // sqlParameters.Add(passwordSaltParameter);
                // SqlParameter emailParameter = new SqlParameter("@EmailParam", System.Data.SqlDbType.VarChar);
                // emailParameter.Value = userForRegistrationDto.Email;
                // sqlParameters.Add(emailParameter);
                
                // SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSaltParam", System.Data.SqlDbType.VarBinary);
                // passwordSaltParameter.Value = passwordSalt;
                // sqlParameters.Add(passwordSaltParameter);

                // SqlParameter passwordHashParameter = new SqlParameter("@PasswordHashParam", System.Data.SqlDbType.VarBinary);
                // passwordHashParameter.Value = passwordHash;
                // sqlParameters.Add(passwordHashParameter);

                
                // if(_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                // {
                UserForLoginDto userForSetPassword = new UserForLoginDto()
                {
                    Email = userForRegistrationDto.Email,
                    Password = userForRegistrationDto.Password
                };
                if(_authHelpers.SetPassword(userForSetPassword)){
                    
                    DotnetAPI.Models.UserComplete userComplete = _mapper.Map<DotnetAPI.Models.UserComplete>(userForRegistrationDto);
                    userComplete.Active = true; 
                    // string sqlAddUser = @"EXEC TutorialAppSchema.spUser_Upsert 
                    // @FirstName = '" + userForRegistrationDto.FirstName + 
                    // "', @LastName = '" + userForRegistrationDto.LastName + 
                    // "', @Email = '" + userForRegistrationDto.Email +    
                    // "', @Gender = '" + userForRegistrationDto.Gender +                     
                    // "', @JobTitle = '" + userForRegistrationDto.JobTitle + 
                    // "', @Department = '" + userForRegistrationDto.Department +
                    // "', @Salary = " + userForRegistrationDto.Salary +
                    // ", @Active = 1";
                     
                     
                    
                    // string sqlAddUser = @"
                    // INSERT INTO [TutorialAppSchema].[Users]
                    //         ([FirstName]
                    //         ,[LastName]
                    //         ,[Email]
                    //         ,[Gender]
                    //         ,[Active])
                    //     VALUES
                    //         ("+ "'" + userForRegistrationDto.FirstName +
                    //         "','" + userForRegistrationDto.LastName +
                    //         "','" + userForRegistrationDto.Email +
                    //         "','" + userForRegistrationDto.Gender +
                    //         "', 1)";
                    // if(_dapper.ExecuteSql(sqlAddUser))
                    // {
                    //     return Ok();
                    // }
                    if(_reusableSql.UpsertUser(userComplete))
                    {
                        return Ok();
                    }
                    throw new Exception("Failed to Add User");
                }
                throw new Exception("Failed to register user.");
            }
            throw new Exception("User with this email already exists!");            
        }
        
        throw new Exception("Passwords do not match!");
    }

    [HttpPut("ResetPassword")]
    public IActionResult ResetPassword(UserForLoginDto userForSetPassword)
    {
        if(_authHelpers.SetPassword(userForSetPassword)){
            return Ok();
        }
        throw new Exception("Failed to update password!");
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLogin)
    {
        string sqlForHashAndSalt = @"EXEC TutorialAppSchema.spLoginConfirmation_Get  
        @Email = @EmailParam";

        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@EmailParam", userForLogin.Email, System.Data.DbType.String);

                // SqlParameter emailParameter = new SqlParameter("@EmailParam", System.Data.SqlDbType.VarChar);
                // emailParameter.Value = userForLogin.Email;
                // sqlParameters.Add(emailParameter);

        
        UserForLoginConfirmationDto userForConfirmation = _dapper
            .LoadDataSingleWithParameters<UserForLoginConfirmationDto>(sqlForHashAndSalt, sqlParameters);
        
        byte[] passwordHash = _authHelpers.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

        // if(passwordHash == userForConfirmation.PasswordHash)
        // {
            // if(passwordHash == userForConfimation.PasswordHash) / wont work
            for(int index = 0; index < passwordHash.Length; index++)
            {
                if(passwordHash[index] != userForConfirmation.PasswordHash[index]){
                    return StatusCode(401, "Incorrect password!");
                }
            }    
                
        // }
        string userIdSql = @"
        SELECT UserId FROM TutorialAppSchema.Users WHERE Email = '" + userForLogin.Email + "'";
        int userId = _dapper.LoadDataSingle<int>(userIdSql);
        return Ok(new Dictionary<string, string>{
            {"token", _authHelpers.CreateToken(userId)}
        });
    }

    [HttpGet("RefreshToken")]
    public IActionResult RefreshToken()
    {
        string userId = User.FindFirst("userId")?.Value + "";
        string userIdSql = "SELECT userId From TutorialAppSchema.Users Where UserId = " 
                + userId;
        int userIdFromDb = _dapper.LoadDataSingle<int>(userIdSql);
        //return CreateToken(userIdFromDb);  
        return Ok(new Dictionary<string, string>{
            {"token", _authHelpers.CreateToken(userIdFromDb)}
        });  
    }
    // private byte[] GetPasswordHash(string password,byte[] passwordSalt)
    // {
    //     string passwordSaltPlusString = _config.GetSection("AppSettings:PasswodKey").Value + 
    //                 Convert.ToBase64String(passwordSalt);

    //                 return KeyDerivation.Pbkdf2(
    //                     password: password,
    //                     salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
    //                     prf: KeyDerivationPrf.HMACSHA256,
    //                     iterationCount: 100000,
    //                     numBytesRequested: 256 / 8
    //                 );
    // }

    // private string CreateToken(int userId)
    // {
    //     Claim[] clams = new Claim[]{
    //         new Claim("userId", userId.ToString())
    //     };
        
    //     string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;
    //     SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
    //         Encoding.UTF8.GetBytes(
    //             tokenKeyString != null ? tokenKeyString : ""
    //         )
    //     );

    //     SigningCredentials credentials = new SigningCredentials(
    //         tokenKey, 
    //         SecurityAlgorithms.HmacSha512Signature
    //        );
    //     SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
    //     {
    //         Subject = new ClaimsIdentity(clams),
    //         SigningCredentials = credentials,
    //         Expires = DateTime.Now.AddDays(1)
            
    //     };
    //     JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
    //     SecurityToken token = tokenHandler.CreateToken(descriptor);
    //     return tokenHandler.WriteToken(token);
    // }
}