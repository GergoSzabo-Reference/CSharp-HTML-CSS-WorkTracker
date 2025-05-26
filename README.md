# CSharp-HTML-CSS-WorkTracker

# model-validations
data validations

# ef-core-setup
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Design - migrations
Microsoft.EntityFrameworkCore.Tools - db
Swashbuckle.AspNetCore

nullables
dbcontext.cs
project reference

appsettings.js -> ConnectionString for db

program.cs -> dependency injection container: db config + swagger

# database-migrations
Add-Migration InitialCreate -> nuget manager console
Update-Database

# customer-crud-api
UgyfelekController.cs -> CRUD api, get/getall/post/put/delete

# work-crud-api
MunkakController.cs -> CRUD api, get/getall/post/put/delete

# predict-workhours
Services -> WorkHourEstimationService.cs
Program.cs -> register new service in order for the controllers get the DI
Munka.cs -> double BecsultMunkaorak
MunkakController.cs -> DI

# listing-client-jobs
UgyfelekController.cs -> Get + DI

# unit-tests
Autoszerelo_UnitTests project
    -> WorkHourEstimationTests.cs