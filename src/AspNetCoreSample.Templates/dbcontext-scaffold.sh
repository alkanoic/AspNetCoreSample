dotnet ef dbcontext scaffold "Server=postgresql;Username=root;Password=postgres;Database=sample" Npgsql.EntityFrameworkCore.PostgreSQL -o Models -f --no-onconfiguring

# dotnet ef dbcontext scaffold "Server=localhost;User=docker;Password=docker;Database=sample" Pomelo.EntityFrameworkCore.MySql -o Models --data-annotations -f --no-onconfiguring
