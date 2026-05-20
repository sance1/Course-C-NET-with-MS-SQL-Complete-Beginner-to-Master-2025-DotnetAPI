Lecture Course : https://www.udemy.com/course/net-core-with-ms-sql-beginner-to-expert/learn/lecture/34625324#overview  
See : https://www.youtube.com/watch?v=ZVGutgqBMUM&t=182s  
Getting Started .net  
https://dotnet.microsoft.com/  
https://code.visualstudio.com/  
for extension  
C# Dev Kit  
C#  
dont forget to instal github  

create webapi dan webapp in one folder  
1Buat Folder Utama & Solution:  
mkdir MyProject  
cd MyProject  
dotnet new sln -n MySolution  
2.Buat Proyek Web API:  
dotnet new webapi -o MyApi  
dotnet sln add MyApi/MyApi.csproj  
3.Buat Proyek Web App (UI):  
dotnet new webapp -o MyWeb  
dotnet sln add MyWeb/MyWeb.csproj  

how to running   
dotnet watch run --this for runing codee   
dotnet run --project MyApi/MyApi.csproj   
dotnet run --project MyWeb/MyWeb.csproj   
running from folder   
cd MyWeb   
dotnet run    
Buka dua Terminal/Tab di VS Code:    
Terminal 1: dotnet run --project MyApi    
Terminal 2: dotnet run --project MyWeb    
see the respon cmd for run in website   
******  
VD-68 -AutoMapper   
VD-69 -Database Connection   
VD-70 -Http Launch    
VD-71 -User Models, VD-72,    
VD-73
Berdasarkan dokumentasi Microsoft Learn, berikut perbedaannya: 
Microsoft Learn
Microsoft Learn
[FromBody]: Data diambil dari isi pesan (body) (biasanya JSON).
[FromQuery]: Data diambil dari URL query string (contoh: ?id=123).
[FromRoute]: Data diambil dari route parameter (contoh: /api/user/{id}).
[FromForm]: Data diambil dari postingan form HTML
apakah boleh jika tidak menggunakan from body?
Boleh saja, tetapi ada dua kondisi utama yang akan menentukan apakah kode kamu tetap jalan atau tidak:
1. Jika Kamu Menggunakan Atribut [ApiController]
Di ASP.NET Core modern, jika di atas class controller kamu ada tulisan [ApiController], kamu tidak wajib menuliskan [FromBody].
Sistem sudah punya aturan otomatis (Inference Rules):
Jika parameter kamu adalah Class/Objek (misal: User model), ASP.NET otomatis menganggapnya sebagai [FromBody].
Jika parameter kamu adalah tipe dasar (misal: string nama, int id), ASP.NET otomatis menganggapnya sebagai [FromQuery].
2. Jika Kamu TIDAK Menggunakan [ApiController]
Jika kamu menggunakan controller biasa (seperti di MVC lama), maka kamu wajib menuliskan [FromBody] jika ingin mengambil data JSON. Tanpa itu, ASP.NET akan mencoba mencari data tersebut di URL (Query String) atau Form Data, dan variabel kamu akan berakhir null karena data JSON di dalam body tidak diperiksa.   
IactionResult->return ok()->return ok(user)->untuk fungsi banwaan
actionRestul->return user()
gunakan ActionResult di kontroller, sedangkan direpository/serives gunakan langsung variable user ->class user nama_klas    
VD-75,VD-77 EF Setup   
dotnet add package Microsoft.EntityFrameworkCore 
for fix an error HasDefaultSchema add this
dotnet add package Microsoft.EntityFrameworkCore.Relational --version 8.0.13   
*you need also match entityframeworkcore with type this  
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.13     
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.13
VD-78.EF User Controller VD-81.Beginner AssignmentSolutionDapper  
1.create fungction GetUserSalary will return IEnumerable with parameter int userId   
First = "Beri saya yang pertama, saya yakin ada."
FirstOrDefault = "Beri saya yang pertama, kalau tidak ada ya sudah (null)."
Single = "Beri saya satu-satunya, tidak boleh kosong, tidak boleh dua."
SingleOrDefault = "Beri saya satu-satunya, kalau kosong boleh (null), tapi kalau ada dua itu salah."   

try {
    var user = _context.Users.SingleOrDefault(u => u.Email == email);
} 
catch (InvalidOperationException ex) {
    // Oh, ini pasti karena datanya duplikat!
}  
public IEnumerable<User> GetActiveUsers(List<User> allUsers)
{
    // LINQ secara otomatis mengembalikan IEnumerable
    return allUsers.Where(u => u.IsActive); 
}   

dari 83  itu sudah entity dan bisa crud stelehnya itu lanjut repository  
VD-85|VD-89 Password Management(AuthController/Register)   
VD-90 Login, VD-91,VD-92, VD-93       
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer  
VD-96

CREATE TABLE TutorialAppShema.Post (
    PostId INT IDENTITY(1,1),
    UserId INT,
    PostTitle NVARCHAR(255),
    PostContent NVARCHAR(MAX),
    PostCreated DATETIME,
    LastUpdated DATETIME
)

CREATE CLUSTERED INDEX cix_Post_UserId_PostId on TutorialAppSchema.Post(UserId, PostId)
VD-97, VD-98,VD-100,  
USE [DotNetCourseDatabase]
GO
/****** Object:  StoredProcedure [TutorialAppSchema].[spUsers_Get]    Script Date: 5/9/2026 6:42:14 AM ******/

ALTER PROCEDURE [TutorialAppSchema].[spUsers_Get]
--EXEC TutorialAppSchema.spUsers_Get @UserId=1, @RunFilter=1
	@RunFilter BIT
	, @UserId INT
AS
BEGIN
	SELECT UserId, FirstName,
	LastName, Email, Gender, Active
FROM TutorialAppSchema.Users As Users
WHERE Users.UserId = @UserId
ENDUSE [DotNetCourseDatabase]
GO
/****** Object:  StoredProcedure [TutorialAppSchema].[spUsers_Get]    Script Date: 5/9/2026 6:42:14 AM ******/

ALTER PROCEDURE [TutorialAppSchema].[spUsers_Get]
--EXEC TutorialAppSchema.spUsers_Get @UserId=1, @RunFilter=1
	@RunFilter BIT
	, @UserId INT
AS
BEGIN
	SELECT UserId, FirstName,
	LastName, Email, Gender, Active
FROM TutorialAppSchema.Users As Users
WHERE Users.UserId = @UserId
END   
-- WHERE Users.UserId = ISNULL(@UserId,Users.Userid) -> the result same with not useing where statement  
Filter doest have value,,
VD-102 Outer Applay  
USE [DotNetCourseDatabase]
GO

ALTER PROCEDURE [TutorialAppSchema].[spUsers_Get]
--EXEC TutorialAppSchema.spUsers_Get 1004
-- @RunFilter BIT
	 @UserId INT = null	
AS
BEGIN
	Select
		[Users].[UserId],
		[Users].[FirstName],
		[Users].[LastName],
		[Users].[Email],
		[Users].[Gender],
		[Users].Active,
		UserSalary.Salary,
		UserJobInfo.Department,
		UserJobInfo.JobTitle,
		AvgSalary.AvgSalary
	from TutorialAppSchema.Users as Users
	Left join TutorialAppSchema.UserSalary as UserSalary
	on UserSalary.UserId = Users.UserId
	Left join TutorialAppSchema.UserJobInfo as UserJobInfo
	on UserJobInfo.UserId = Users.UserId
	Outer Apply(
		Select AVG(UserSalary.Salary) as AvgSalary
		from TutorialAppSchema.Users as Users
		Left join TutorialAppSchema.UserSalary as UserSalary
		on UserSalary.UserId = Users.UserId
		Left join TutorialAppSchema.UserJobInfo as UserJobInfo2
		on UserJobInfo.UserId = Users.UserId
		Where UserJobInfo2.Department = UserJobInfo.Department
		Group by Department
	) as AvgSalary
	Where Users.UserId = ISNULL(@UserId, Users.UserId)
END    
VD-103  
USE [DotNetCourseDatabase]
GO

ALTER PROCEDURE [TutorialAppSchema].[spUsers_Get]
--EXEC TutorialAppSchema.spUsers_Get @UserId = 1004
-- @RunFilter BIT
	 @UserId INT = NULL
	 , @Active BIT = NULL
AS
BEGIN
	--IF OBJECT_ID('tempdb..#AverageDeptSalary', 'U') IS NOT NULL
	--BEGIN
	--	DROP TABLE #AverageDeptSalary
	--END
	
	DROP TABLE IF EXISTS #AverageDeptSalary
	
	Select AVG(UserSalary.Salary) as AvgSalary
		, UserJobInfo.Department
		Into #AverageDeptSalary
		from TutorialAppSchema.Users as Users
		Left join TutorialAppSchema.UserSalary as UserSalary
		on UserSalary.UserId = Users.UserId
		Left join TutorialAppSchema.UserJobInfo as UserJobInfo
		on UserJobInfo.UserId = Users.UserId
		Group by Department
	
	Select
		[Users].[UserId],
		[Users].[FirstName],
		[Users].[LastName],
		[Users].[Email],
		[Users].[Gender],
		[Users].Active,
		UserSalary.Salary,
		UserJobInfo.Department,
		UserJobInfo.JobTitle,
		AvgSalary.AvgSalary
	from TutorialAppSchema.Users as Users
	Left join TutorialAppSchema.UserSalary as UserSalary
	on UserSalary.UserId = Users.UserId
	Left join TutorialAppSchema.UserJobInfo as UserJobInfo
	on UserJobInfo.UserId = Users.UserId
	left join #AverageDeptSalary as AvgSalary
	on AvgSalary.Department = UserJobInfo.Department
	--Outer Apply(
	--	Select AVG(UserSalary.Salary) as AvgSalary
	--	from TutorialAppSchema.Users as Users
	--	Left join TutorialAppSchema.UserSalary as UserSalary2
	--	on UserSalary2.UserId = Users.UserId
	--	Left join TutorialAppSchema.UserJobInfo as UserJobInfo2
	--	on UserJobInfo.UserId = Users.UserId
	--	Where UserJobInfo2.Department = UserJobInfo.Department
	--	Group by Department
	--) as AvgSalary
	Where Users.UserId = ISNULL(@UserId, Users.UserId)
		And ISNULL(Users.Active, 0) = COALESCE(@Active, Users.Active, 0)
END

--SELECT CASE WHEN NULL = NULL then 1 Else 0 End
--	,Case When Null <> NUll then 1 Else 0 End;

VD-104  
go
CREATE OR ALTER PROCEDURE tutorialAppschema.spUser_Upset
	@FirstName NVARCHAR(50) ,
	@LastName NVARCHAR(50) ,
	@Email NVARCHAR(50) ,
	@Gender NVARCHAR(50) ,
	@Active BIT,
	@UserId INT = NULL
AS
BEGIN
	--SELECT GETDATE();
	IF NOT EXISTS(SELECT * FROM TutorialAppSchema.Users WHERE UserId = @UserId)
		BEGIN
			IF NOT EXISTS(SELECT * FROM TutorialAppSchema.Users WHERE Email = @UserId)
			BEGIN
				INSERT INTO TutorialAppSchema.Users(
				FirstName,
				LastName,
				Email,
				Gender,
				Active
			) VALUES (
				@FirstName,
				@LastName,
				@Email,
				@gender,
				@Active
			)
			END
		END
	ELSE
		BEGIN
			UPDATE TutorialAppSchema.Users
				SET FirstName = @FirstName,
					LastName = @LastName,
					Email = @Email,
					gender = @Gender,
					Active = @Active
			WHERE UserId = @UserId
		END
END    
VD-105

USE [DotNetCourseDatabase]
GO

ALTER   PROCEDURE [TutorialAppSchema].[spUser_Upset]
	@FirstName NVARCHAR(50) ,
	@LastName NVARCHAR(50) ,
	@Email NVARCHAR(50) ,
	@Gender NVARCHAR(50) ,
	@JobTitle NVARCHAR(50),
	@Department NVARCHAR(50),
	@Salary DECIMAL(18, 4),
	@Active BIT,
	@UserId INT = NULL
AS
BEGIN
	--SELECT GETDATE();
	IF NOT EXISTS(SELECT * FROM TutorialAppSchema.Users WHERE UserId = @UserId)
		BEGIN
			IF NOT EXISTS(SELECT * FROM TutorialAppSchema.Users WHERE Email = @Email)
			BEGIN
				DECLARE @OutputUserId INT
				INSERT INTO TutorialAppSchema.Users(
				FirstName,
				LastName,
				Email,
				Gender,
				Active
			) VALUES (
				@FirstName,
				@LastName,
				@Email,
				@gender,
				@Active
			)

			SET @OutputUserId = @@IDENTITY	
			
			INSERT INTO TutorialAppSchema.UserSalary(
				UserId,
				Salary			
			) VALUES (
				@OutputUserId,
				@Salary
			)
			INSERT INTO TutorialAppSchema.UserJobInfo(
				UserId,
				Department,
				JobTitle
			) VALUES (
				@OutputUserId,
				@Department,
				@JobTitle
			)
			END
		END
	ELSE
		BEGIN
			UPDATE TutorialAppSchema.Users
				SET FirstName = @FirstName,
					LastName = @LastName,
					Email = @Email,
					gender = @Gender,
					Active = @Active
			WHERE UserId = @UserId

			UPDATE TutorialAppSchema.UserSalary
				SET Salary = @Salary
				WHERE UserId = @UserId
			UPDATE TutorialAppSchema.UserJobInfo
				SET JobTitle = @JobTitle,
					Department = @Department
				WHERE UserId = @UserId
		END
END

--select @@TRANCOUNT
--commit

--Rollback
--exec TutorialAppSchema.spUsers_Get    

VD-106  
USE [DotNetCourseDatabase]
GO

ALTER PROCEDURE [TutorialAppSchema].[spUser_Delete]
	@UserId INT
AS
BEGIN
	DELETE FROM TutorialAppSchema.Users
		WHERE UserId = @UserId
	DELETE FROM TutorialAppSchema.UserSalary
		WHERE UserId = @UserId
	DELETE FROM TutorialAppSchema.UserJobInfo
		WHERE UserId = @UserId
END   
VD-107  








      





