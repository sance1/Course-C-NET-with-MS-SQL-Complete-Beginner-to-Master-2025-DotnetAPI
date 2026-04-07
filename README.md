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
VD 68 -AutoMapper
VD 69 -Database Connection
VD 70 -Http Launch
VD 71 -User Models
