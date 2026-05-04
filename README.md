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


      





