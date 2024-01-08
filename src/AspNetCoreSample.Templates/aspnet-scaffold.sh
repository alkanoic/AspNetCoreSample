dotnet aspnet-codegenerator controller -f -name NameController -async -m Name -dc SampleContext -outDir Controllers -udl
dotnet aspnet-codegenerator controller -f -name NameApiController -api -async -m Name -dc SampleContext -outDir Controllers -udl
dotnet aspnet-codegenerator controller -f -name MultiTableController -async -m MultiTable -dc SampleContext -outDir Controllers -udl
