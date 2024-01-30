using Cocona;

var app = CoconaApp.Create();

app.AddCommand("sample", Sample);
app.AddCommand("api", Api);
app.AddCommand("mvc", Mvc);
app.AddCommand("datatables", DataTables);

app.Run();
