dotnet run --project ../CodeGen -- api \
    --class-file-path Models/SampleTable.cs \
    --class-name SampleTable \
    --output-path ../CodeGen.Result/Controllers/api/SampleTableController.cs \
    --db-context-path ../CodeGen.Result/Models/SampleContext.cs \
    --namespace-name CodeGen.Result.WebApi \
    --controller-name SampleTableApiController
