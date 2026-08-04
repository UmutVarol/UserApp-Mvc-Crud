# Graph Report - /Users/muhammedumutvarol/Documents/UserApp  (2026-08-03)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 352 nodes · 506 edges · 46 communities (31 shown, 15 thin omitted)
- Extraction: 95% EXTRACTED · 5% INFERRED · 0% AMBIGUOUS · INFERRED: 23 edges (avg confidence: 0.8)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `f6751270`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- HomeController
- UserApp.Data
- UserService
- UserApp.Entities.Dtos
- AccountController
- KullaniciRepository
- http
- UserApp.Data.csproj
- .UploadProfileImageAsync
- UserApp.Data.Migrations
- AddValidationRules
- InitialCreate
- AddDepartmanColumn
- DepartmanTablosunuAyir
- DahaFazlaDepartman
- Migration
- AddKayitTarihi
- AddProfileImagePath
- AddIdentityTables
- ServiceResult
- AppDbContextModelSnapshot.cs
- .UploadImageAsync
- UserApp.Web.Models
- UserApp.Web.Models.LoginViewModel
- Create.cshtml
- Delete.cshtml
- Details.cshtml
- Edit.cshtml
- kullanici-datatable.js
- DbContext
- HttpGet
- HttpPost
- IActionResult
- SonEklenenAdSoyad
- IFormFile
- int
- ValidateAntiForgeryToken

## God Nodes (most connected - your core abstractions)
1. `UserApp.Data.Migrations` - 21 edges
2. `UserApp.Data` - 18 edges
3. `UserService` - 14 edges
4. `HomeController` - 14 edges
5. `UserApp.Entities` - 13 edges
6. `IUserService` - 12 edges
7. `UserApp.Entities.Dtos` - 11 edges
8. `IKullaniciRepository` - 11 edges
9. `KullaniciRepository` - 11 edges
10. `AppDbContext` - 9 edges

## Surprising Connections (you probably didn't know these)
- `KullaniciEditDtoValidator` --references--> `KullaniciEditDto`  [EXTRACTED]
  UserApp.Services/Validation/KullaniciEditDtoValidator.cs → UserApp.Entities/Dtos/KullaniciEditDto.cs
- `KullaniciListViewModel` --references--> `KullaniciListItemDto`  [EXTRACTED]
  UserApp.Data/UserApp.Web/Models/KullaniciListViewModel.cs → UserApp.Entities/Dtos/KullaniciListItemDto.cs
- `KullaniciCreateDtoValidator` --references--> `KullaniciCreateDto`  [EXTRACTED]
  UserApp.Services/Validation/KullaniciCreateDtoValidator.cs → UserApp.Entities/Dtos/KullaniciCreateDto.cs
- `UserApp.Services` --references--> `net10.0`  [EXTRACTED]
  UserApp.Services/UserApp.Services.csproj → UserApp.Data/UserApp.Data.csproj
- `UserApp.Web` --references--> `net10.0`  [EXTRACTED]
  UserApp.Web/UserApp.Web.csproj → UserApp.Data/UserApp.Data.csproj

## Import Cycles
- None detected.

## Communities (46 total, 15 thin omitted)

### Community 0 - "HomeController"
Cohesion: 0.10
Nodes (25): ActionName, IFormFile, int, IWebHostEnvironment, KullaniciEditDto, Departman, DepartmanSayisi, Items (+17 more)

### Community 1 - "UserApp.Data"
Cohesion: 0.07
Nodes (25): UserApp.Services, UserApp.Entities, UserApp.Data.Seed, UserApp.Data, DbSet, IConfiguration, IdentityDbContext, IdentityUser (+17 more)

### Community 2 - "UserService"
Cohesion: 0.09
Nodes (24): IDepartmanRepository, IValidator, DepartmanSayisi, Items, Kullanici, List, SonEklenen, Task (+16 more)

### Community 3 - "UserApp.Entities.Dtos"
Cohesion: 0.11
Nodes (14): AbstractValidator, UserApp.Entities.Dtos, UserApp.Services.Validation, List, KullaniciListViewModel, IFormFile, KullaniciCreateDto, DateTime (+6 more)

### Community 4 - "AccountController"
Cohesion: 0.14
Nodes (13): Controller, UserApp.Web.Controllers, UserApp.Web.Models, SignInManager, HttpGet, HttpPost, IActionResult, Task (+5 more)

### Community 5 - "KullaniciRepository"
Cohesion: 0.21
Nodes (9): DepartmanSayisi, Items, Kullanici, List, SonEklenen, Task, ToplamKullanici, TotalCount (+1 more)

### Community 6 - "http"
Cohesion: 0.13
Nodes (15): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, applicationUrl, commandName (+7 more)

### Community 7 - "UserApp.Data.csproj"
Cohesion: 0.23
Nodes (11): net10.0, FluentValidation (12.1.1), Microsoft.AspNetCore.Identity.EntityFrameworkCore (10.0.10), Microsoft.EntityFrameworkCore.Design (10.0.10), Microsoft.EntityFrameworkCore.SqlServer (10.0.10), Microsoft.EntityFrameworkCore.Tools (10.0.10), Microsoft.NET.Sdk, Microsoft.NET.Sdk.Web (+3 more)

### Community 8 - ".UploadProfileImageAsync"
Cohesion: 0.20
Nodes (8): ErrorMessage, FilePath, Success, IFormFile, int, string, Task, FileHelper

### Community 9 - "UserApp.Data.Migrations"
Cohesion: 0.28
Nodes (4): UserApp.Data.Migrations, MigrationBuilder, ModelBuilder, AddUserRoleLinkingFields

### Community 10 - "AddValidationRules"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, AddValidationRules

### Community 11 - "InitialCreate"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, InitialCreate

### Community 12 - "AddDepartmanColumn"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, AddDepartmanColumn

### Community 13 - "DepartmanTablosunuAyir"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, DepartmanTablosunuAyir

### Community 14 - "DahaFazlaDepartman"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, DahaFazlaDepartman

### Community 15 - "Migration"
Cohesion: 0.25
Nodes (4): Migration, MigrationBuilder, ModelBuilder, AddUserNewFeatures

### Community 16 - "AddKayitTarihi"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, AddKayitTarihi

### Community 17 - "AddProfileImagePath"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, AddProfileImagePath

### Community 18 - "AddIdentityTables"
Cohesion: 0.29
Nodes (3): MigrationBuilder, ModelBuilder, AddIdentityTables

### Community 19 - "ServiceResult"
Cohesion: 0.33
Nodes (3): IEnumerable, List, ServiceResult

### Community 20 - "AppDbContextModelSnapshot.cs"
Cohesion: 0.40
Nodes (3): ModelSnapshot, ModelBuilder, AppDbContextModelSnapshot

### Community 21 - ".UploadImageAsync"
Cohesion: 0.40
Nodes (3): IFormFile, Task, IFileHelper

## Knowledge Gaps
- **25 isolated node(s):** `FluentValidation (12.1.1)`, `ErrorViewModel`, `$schema`, `commandName`, `dotnetRunMessages` (+20 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **15 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `UserApp.Data` connect `UserApp.Data` to `UserApp.Data.Migrations`, `AddValidationRules`, `InitialCreate`, `AddDepartmanColumn`, `DepartmanTablosunuAyir`, `DahaFazlaDepartman`, `Migration`, `AddKayitTarihi`, `AddProfileImagePath`, `AddIdentityTables`, `AppDbContextModelSnapshot.cs`?**
  _High betweenness centrality (0.305) - this node is a cross-community bridge._
- **Why does `UserService` connect `UserService` to `HomeController`, `UserApp.Data`?**
  _High betweenness centrality (0.170) - this node is a cross-community bridge._
- **Why does `UserApp.Entities` connect `UserApp.Data` to `AccountController`?**
  _High betweenness centrality (0.135) - this node is a cross-community bridge._
- **What connects `FluentValidation (12.1.1)`, `ErrorViewModel`, `$schema` to the rest of the system?**
  _25 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `HomeController` be split into smaller, more focused modules?**
  _Cohesion score 0.09634551495016612 - nodes in this community are weakly interconnected._
- **Should `UserApp.Data` be split into smaller, more focused modules?**
  _Cohesion score 0.06707317073170732 - nodes in this community are weakly interconnected._
- **Should `UserService` be split into smaller, more focused modules?**
  _Cohesion score 0.09024390243902439 - nodes in this community are weakly interconnected._