# EFCore-5 SamuraiApp

*Notes

We have directly added 
Microsoft.EntityFrameworkCore.SqlServer
from nuget package manger to the SamuraiApp.Data folder.

This brings in additional functionality which is required for this particular database (sql server in this case).

Had we installed just Microsoft.EntityFrameworkCore, we would have to look and add a provider as well on top of that.

--------------------------------------------------------------------------------------------------------------------------------------------------

For migrations, Add following reference to the SamuraiApp.Data project : Microsoft.EntityFrameworkCore.Tools (For migration commands)
Tools package will pull in Microsoft.EntityFrameworkCore.Design for you (For migration APIs)

In VS, goto tools -> Nuget Package Manager -> Package Manager Console
In console, select default project as SamuraiApp.Data

get-help entityframework

Add-Migrations & Update-database are most commonly used.

add-migration init_migration_1                     //init_migration_1 is name of migration, you can put whatever you want

update-database                                    //update-database -verbose

--for development database => update-database
--for production database => script-migration


For reverse engineering (generating models / code from existing database):
scaffold-dbcontext -provider Microsoft.EntityFrameworkCore.SqlServer -connection "Data Source = (LocalDB)\\MSSQLLocalDB; Initial Catalog=SamuraiAppData"

--------------------------------------------------------------------------------------------------------------------------------------------------


