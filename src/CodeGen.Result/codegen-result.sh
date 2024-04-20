dotnet run --project ../CodeGen -- api \
    --class-file-path Models/SampleTable.cs \
    --class-name SampleTable \
    --output-path ../CodeGen.Result/Controllers/api/SampleTableController.cs \
    --db-context-path ../CodeGen.Result/Models/SampleContext.cs \
    --namespace-name CodeGen.Result.WebApi \
    --controller-name SampleTableApiController

dotnet run --project ../CodeGen -- api \
    --class-file-path Models/MultiTable.cs \
    --class-name MultiTable \
    --output-path ../CodeGen.Result/Controllers/api/MultiTableController.cs \
    --db-context-path ../CodeGen.Result/Models/SampleContext.cs \
    --namespace-name CodeGen.Result.WebApi \
    --controller-name MultiTableApiController

dotnet run --project ../CodeGen -- mvc \
    --class-file-path Models/SampleTable.cs \
    --class-name SampleTable \
    --output-directory ../CodeGen.Result/ \
    --db-context-path ../CodeGen.Result/Models/SampleContext.cs \
    --namespace-name CodeGen.Result.Controllers

dotnet run --project ../CodeGen -- mvc \
    --class-file-path Models/MultiTable.cs \
    --class-name MultiTable \
    --output-directory ../CodeGen.Result/ \
    --db-context-path ../CodeGen.Result/Models/SampleContext.cs \
    --namespace-name CodeGen.Result.Controllers
