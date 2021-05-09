# Dotnet Core net5.0 , EFCore 5, SamuraiApp

> Notes

We have directly added 
Microsoft.EntityFrameworkCore.SqlServer
from nuget package manger to the SamuraiApp.Data folder.

This brings in additional functionality which is required for this particular database (sql server in this case).

Had we installed just Microsoft.EntityFrameworkCore, we would have to look and add a provider as well on top of that.

--------------------------------------------------------------------------------------------------------------------------------------------------

**For Migrations** 

Add following reference to the SamuraiApp.Data project : Microsoft.EntityFrameworkCore.Tools (For migration commands)
Tools package will pull in Microsoft.EntityFrameworkCore.Design for you (For migration APIs)

In VS, goto tools -> Nuget Package Manager -> Package Manager Console
In console, select default project as SamuraiApp.Data

get-help entityframework

get-migration                                      // used to get a list of all the migrations with respective status

                                                    //Add-Migrations & Update-database are most commonly used.

add-migration init_migration_1                     //init_migration_1 is name of migration, you can put whatever you want
      
update-database                                    //update-database -verbose

--for development database => update-database
--for production database => script-migration


For reverse engineering (generating models / code from existing database):
scaffold-dbcontext -provider Microsoft.EntityFrameworkCore.SqlServer -connection "Data Source = (LocalDB)\\MSSQLLocalDB; Initial Catalog=SamuraiAppData"

--------------------------------------------------------------------------------------------------------------------------------------------------

**To visualize the the models and relationships created by entity framework**

Use following tool :-

EF Core Power Tools

This is a visual studio extension which runs on DGML editor

Visual Studio -> Extensions -> Manage Extensions.

Search 'EF Core Power Tools' and click download.

Then, right click on SamuraiApp.Data project -> EF Core Power Tools -> Add DbContext Model Diagram

--------------------------------------------------------------------------------------------------------------------------------------------------

**To create sql functions / views using ef core:**

add-migration createViewAndSqlFunctionWithEfcore	//This is empth migration, without any changes in dbcontext

Go to createViewAndSqlFunctionWithEfcore.cs.

Add raw sql in Up() for creating, and Down() for dropping/deleting functions and views.

Save and update-database.

You can see the functions and views created in the database.
(SamuraiAppData -> Views -> dbo.SamuraiBattleStats)
(SamuraiAppData -> Programmability -> Functions -> Scalar-valued Functions -> dbo.EarliestBattleFoughtBySamurai)

_Note: Same process for stored procedures as well._

--------------------------------------------------------------------------------------------------------------------------------------------------


> Testing

```
 For normal app, we are using **SamuraiAppData** (check connection string in SamuraiContect.cs, OnConfiguring()).
 For integration test, we are using **SamuraiTestData**.
 This can be done just by changing the database name in connection string.
 ```

 For using InMemory database in out Data project, add nuget package - "Microsoft.EntityFrameworkCore.InMemory"