Inventory Management System - Clean Project Copy
=================================================

IMPORTANT STARTUP STEPS
1) Extract the ZIP to a NEW folder. Do not overwrite an old project copy.
2) Open InventorySystem.slnx (or InventorySystem.csproj) in Visual Studio.
3) Build > Rebuild Solution.
4) Make sure SQL Server is available as (local) and database InventoryManagementDB exists.
5) Run the project.

Fix included for the startup error:
System.IO.DirectoryNotFoundException ... wwwroot

Program.cs now creates the physical wwwroot directory before ASP.NET Core initializes static web assets.
The complete wwwroot folder (Bootstrap, jQuery, validation, CSS and JS) is also included in this ZIP.

GitHub cleanup:
- .vs and .csproj.user are intentionally excluded from this clean copy.
- .gitignore is included so local Visual Studio/build files are not committed.

If Visual Studio still shows a stale path from an older copy:
1) Close Visual Studio.
2) Delete bin and obj if they exist.
3) Reopen this NEW extracted copy.
4) Rebuild Solution.
