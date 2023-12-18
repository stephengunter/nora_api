# nora_api
dotnet build ./nora_api.sln && dotnet run --project ./Web 


dotnet ef migrations add init -s ../Web
dotnet ef migrations remove -s ../Web
dotnet ef database update -s ../Web