# Move to the project path with the docker-compose.yml file
# Starts the container based on the docker-compose.yml file
docker-compose up -d

# If not installed already, install dotnet tool
dotnet tool install --global dotnet-ef

# If already installed, update it
dotnet tool update --global dotnet-ef

# Ensure the DataLayer .csproj file includes
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />

# IF DATALAYER WAS A SEPERATE PROJECT
# Generate migration files based on the current model
dotnet ef migrations add InitialCreate --project TourPlanner.DataLayer
# Apply the migration (Create the database)
dotnet ef database update --project TourPlanner.DataLayer

# IF DATALAYER IS NOT SEPERATE
# Generate migration files based on the current model
dotnet ef migrations add InitialCreate --project . --startup-project .
# Apply the migration (Create the database)
dotnet ef database update --project . --startup-project .

# To undo the changes
dotnet ef migrations remove

# To get into the database
docker exec -it tourplanner-db psql -U salma -d TourPlanner

# Check for tables
\dt

# To check a table
TourPlanner=# SELECT * FROM "Tours";

