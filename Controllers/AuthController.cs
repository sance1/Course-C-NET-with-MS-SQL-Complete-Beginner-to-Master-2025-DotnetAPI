using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly IConfiguration _config;
    public AuthController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _config = config;
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
                byte[] passwordSalt = new byte[128 / 8];
                using(RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    rng.GetNonZeroBytes(passwordSalt);
                }

                    byte[] passwordHash = GetPasswordHash(userForRegistrationDto.Password, passwordSalt);
                    string sqlAddAuth = @"INSERT INTO TutorialAppSchema.Auth ([Email],
                    [PasswordHash],[PasswordSalt]) VALUES ('" + userForRegistrationDto.Email + "', @PasswordHash, @PasswordSalt)"; 
                
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", System.Data.SqlDbType.VarBinary);
                passwordSaltParameter.Value = passwordSalt;
                
                SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", System.Data.SqlDbType.VarBinary);
                passwordHashParameter.Value = passwordHash;

                sqlParameters.Add(passwordSaltParameter);
                sqlParameters.Add(passwordHashParameter);

                
                if(_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                {
                    string sqlAddUser = @"
                    INSERT INTO [TutorialAppSchema].[Users]
                            ([FirstName]
                            ,[LastName]
                            ,[Email]
                            ,[Gender]
                            ,[Active])
                        VALUES
                            ("+ "'" + userForRegistrationDto.FirstName +
                            "','" + userForRegistrationDto.LastName +
                            "','" + userForRegistrationDto.Email +
                            "','" + userForRegistrationDto.Gender +
                            "', 1)";
                    if(_dapper.ExecuteSql(sqlAddUser))
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

    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLogin)
    {
        string sqlForHashAndSalt = @"Select PasswordHash, PasswordSalt From TutorialAppSchema.Auth Where Email = '" + 
        userForLogin.Email + "'";
        
        UserForLoginConfirmationDto userForConfirmation = _dapper
            .LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);
        
        byte[] passwordHash = GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

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
            {"token", CreateToken(userId)}
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
            {"token", CreateToken(userIdFromDb)}
        });  
    }
    private byte[] GetPasswordHash(string password,byte[] passwordSalt)
    {
        string passwordSaltPlusString = _config.GetSection("AppSettings:PasswodKey").Value + 
                    Convert.ToBase64String(passwordSalt);

                    return KeyDerivation.Pbkdf2(
                        password: password,
                        salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 100000,
                        numBytesRequested: 256 / 8
                    );
    }

    private string CreateToken(int userId)
    {
        Claim[] clams = new Claim[]{
            new Claim("userId", userId.ToString())
        };
        
        string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;
        SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                tokenKeyString != null ? tokenKeyString : ""
            )
        );

        SigningCredentials credentials = new SigningCredentials(
            tokenKey, 
            SecurityAlgorithms.HmacSha512Signature
           );
        SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(clams),
            SigningCredentials = credentials,
            Expires = DateTime.Now.AddDays(1)
            
        };
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateToken(descriptor);
        return tokenHandler.WriteToken(token);
    }
}