# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build the solution
dotnet build

# Run the web application
cd SitiosPersonal && dotnet run

# Build a specific project
dotnet build SitiosPersonal.Services/SitiosPersonal.Services.csproj
```

There are no test projects in this solution.

## Architecture

**SitiosPersonal** is an ASP.NET Core 8.0 Razor Pages HR/recruitment management system with a 4-layer clean architecture:

```
SitiosPersonal          ← Web app (Razor Pages, filters, DI config)
SitiosPersonal.Services ← Business logic (BitacoraService, EncryptionHelper)
SitiosPersonal.Repository ← Data access (Dapper + raw SQL, DbContext, repositories)
SitiosPersonal.Entities ← Models and ViewModels (no logic)
```

## Folder Structure

```
SitiosPersonal/
├── Filters/
│   ├── SessionPageFilter.cs       ← validates session for /Home/Index
│   └── PermisoPageFilter.cs       ← validates DB permissions for /Seguridad/* and /General/*
├── Pages/
│   ├── Shared/
│   │   └── _Layout.cshtml         ← global layout (sidebar menu, topbar, Bootstrap + FontAwesome)
│   ├── Login/
│   │   └── Index.cshtml(.cs)
│   ├── Home/
│   │   └── Index.cshtml(.cs)
│   ├── Seguridad/
│   │   ├── Roles/
│   │   │   ├── Index.cshtml(.cs)  ← paginated list + delete modal
│   │   │   ├── Crear.cshtml(.cs)
│   │   │   └── Editar.cshtml(.cs)
│   │   ├── Modulos/
│   │   │   ├── Index.cshtml(.cs)
│   │   │   ├── Crear.cshtml(.cs)
│   │   │   └── Editar.cshtml(.cs)
│   │   └── Usuarios/
│   │       ├── Index.cshtml(.cs)
│   │       ├── Crear.cshtml(.cs)
│   │       └── Editar.cshtml(.cs)
│   └── General/
│       └── Bitacora/
│           └── Index.cshtml(.cs)
├── Program.cs
└── appsettings.json

SitiosPersonal.Entities/
├── Models/          ← plain classes that map 1:1 to DB tables (snake_case properties)
│   ├── Rol.cs       ← { id_rol, nombre, activo }
│   ├── Usuario.cs
│   ├── Pantalla.cs
│   └── Bitacora.cs
└── ViewModels/      ← page-specific models with DataAnnotations; hold lists for dropdowns
    ├── RolViewModel.cs
    ├── RolesListaViewModel.cs  ← has TotalPaginas computed property
    └── ...

SitiosPersonal.Repository/
├── Data/
│   └── DbContext.cs             ← CreateConnection() returns MySqlConnection
└── Repositories/
    ├── BitacoraRepository.cs
    ├── RolesRepository.cs       ← reference implementation for full CRUD + pagination
    ├── UsuariosRepository.cs
    ├── PantallasRepository.cs   ← named PantallasRepository in DI, class is PantallaRepository
    ├── LoginRepository.cs
    ├── MenuRepository.cs
    └── PermisosRepository.cs

SitiosPersonal.Services/
├── Services/
│   └── BitacoraService.cs
└── Helpers/
    └── EncryptionHelper.cs
```

**New pages for OFE/EMP/GEN modules follow this same structure:**
```
Pages/
├── Oferentes/
│   ├── Index.cshtml(.cs)
│   ├── Crear.cshtml(.cs)
│   └── Editar.cshtml(.cs)
├── Concursos/
│   ├── Index.cshtml(.cs)
│   ├── Crear.cshtml(.cs)
│   └── Editar.cshtml(.cs)
└── Entrevistas/
    ├── Index.cshtml(.cs)
    ├── Crear.cshtml(.cs)
    └── Editar.cshtml(.cs)
```

Every new repository must be registered in `Program.cs` as `AddScoped<XRepository>()`.

## Key Technology Choices

- **Dapper** (not EF Core) — all SQL is written by hand with parameterized queries
- **MySQL** via `MySQL.Data` — connection string in `appsettings.json`
- **AES-GCM** password encryption (not hashing) via `EncryptionHelper`
- **Session-based auth** — no ASP.NET Core Identity; uses `HttpContext.Session` directly

## Core Patterns

### Dependency Injection (Program.cs)
- `DbContext` → Singleton
- All repositories and services → Scoped
- Page filters (`SessionPageFilter`, `PermisoPageFilter`) → Scoped, registered as `IAsyncPageFilter`

### Repository Pattern
Each repository gets a connection via `DbContext.CreateConnection()` and runs raw Dapper SQL. Transactions are handled manually with `connection.BeginTransaction()`.

```csharp
// Standard repository method structure
public List<Rol> ListarPaginado(int pagina, int cantidadPorPagina)
{
    using var connection = _context.CreateConnection();
    int offset = (pagina - 1) * cantidadPorPagina;
    string sql = @"SELECT id_rol, nombre FROM rol ORDER BY id_rol DESC
                   LIMIT @cantidadPorPagina OFFSET @offset;";
    return connection.Query<Rol>(sql, new { cantidadPorPagina, offset }).ToList();
}

// Transaction pattern for operations that touch multiple tables
public int Crear(Entidad entidad)
{
    using var connection = _context.CreateConnection();
    connection.Open();
    using var transaction = connection.BeginTransaction();
    try
    {
        string sql = @"INSERT INTO tabla(col) VALUES(@col); SELECT LAST_INSERT_ID();";
        int id = connection.ExecuteScalar<int>(sql, entidad, transaction);
        transaction.Commit();
        return id;
    }
    catch { transaction.Rollback(); throw; }
}

// Soft-delete guard (used in every repository)
public bool PuedeEliminar(int id)
{
    using var connection = _context.CreateConnection();
    string sql = "SELECT COUNT(*) FROM tabla_hija WHERE id_padre = @id;";
    return connection.ExecuteScalar<int>(sql, new { id }) == 0;
}
```

### PageModel Pattern
Every page injects its repository and `BitacoraService`. Session is read via `HttpContext.Session.GetInt32("IdUsuario")`.

```csharp
public class IndexModel : PageModel
{
    private readonly RolesRepository _repository;
    private readonly BitacoraService _bitacoraService;

    public IndexModel(RolesRepository repository, BitacoraService bitacoraService)
    {
        _repository = repository;
        _bitacoraService = bitacoraService;
    }

    public RolesListaViewModel Lista { get; set; } = new();

    public IActionResult OnGet(int pagina = 1)
    {
        int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
        if (idUsuario == null) return RedirectToPage("/Login/Index");

        Lista = new RolesListaViewModel
        {
            Pagina = pagina,
            CantidadPorPagina = 10,
            TotalRegistros = _repository.Contar(),
            Roles = _repository.ListarPaginado(pagina, 10)
        };

        _bitacoraService.RegistrarConsulta(idUsuario, "Roles");
        return Page();
    }

    public IActionResult OnPostEliminar(int id)
    {
        int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
        var rol = _repository.ObtenerPorId(id);

        if (!_repository.PuedeEliminar(id))
        {
            TempData["Error"] = "No se puede eliminar un registro con datos relacionados.";
            return RedirectToPage("Index");
        }

        _repository.Eliminar(id);
        _bitacoraService.RegistrarDelete(idUsuario, "Rol", new { rol.id_rol, rol.nombre });

        TempData["Exito"] = "Rol eliminado correctamente.";
        return RedirectToPage("Index");
    }
}
```

### Audit Logging (BitacoraService)
Every CUD operation must be logged through `BitacoraService`. The service serializes state as JSON internally — just pass anonymous objects with the relevant fields.

```csharp
// On SELECT / list
_bitacoraService.RegistrarConsulta(idUsuario, "Roles");

// On INSERT — pass the new object after saving (so you have the generated ID)
_bitacoraService.RegistrarInsert(
    idUsuario,
    "Rol",
    new { rol.id_rol, rol.nombre }
);

// On UPDATE — fetch the old record BEFORE updating, then pass both
var rolAnterior = _repository.ObtenerPorId(id);   // fetch before update
_repository.Actualizar(rolActual);
_bitacoraService.RegistrarUpdate(
    idUsuario,
    "Rol",
    new { rolAnterior.id_rol, rolAnterior.nombre },
    new { rolActual.id_rol, rolActual.nombre }
);

// On DELETE — fetch the record BEFORE deleting
var rol = _repository.ObtenerPorId(id);
_repository.Eliminar(id);
_bitacoraService.RegistrarDelete(idUsuario, "Rol", new { rol.id_rol, rol.nombre });

// On error (in catch blocks)
_bitacoraService.RegistrarError(idUsuario, "Roles", ex.Message);
```

### Razor Page conventions
- `@page` directive sets the route explicitly: `@page "/Seguridad/Roles"`
- `TempData["Exito"]` / `TempData["Error"]` show Bootstrap alerts on the next page
- Delete confirmation uses a Bootstrap modal (`data-bs-toggle="modal"`) with a hidden form
- Pagination loops over `Model.Lista.TotalPaginas` using `asp-route-pagina`
- CSS classes in use: `crud-card`, `crud-card-header`, `crud-card-title`, `form-card`, `form-action-panel`, `action-circle`, `form-section-title`

### Authentication Flow
Session stores: `IdUsuario`, `Usuario`, `NombreCompleto`. Login locks account after 3 failed attempts (`estado = BLOQUEADO`). Session idle timeout is 5 minutes; cookie expires after 8 hours.

## Naming & Language Conventions
- All entity names, database columns, UI labels, and method names are in **Spanish**
- Database tables use `snake_case` (e.g., `usuario_rol`, `rol_pantalla`)
- C# classes use `PascalCase`, properties use `snake_case` to match DB columns exactly
- Entity status values are string literals: `"ACTIVO"`, `"INACTIVO"`, `"BLOQUEADO"`
- ViewModel suffix: `XxxViewModel` for forms, `XxxListaViewModel` for paginated lists

## Database
- MySQL at `138.59.135.33:3306`, database `DB_PERSONAL_SITIOS`
- Full schema + seed data in `/SitiosDB.sql`
- Key junction tables: `usuario_rol`, `rol_pantalla`
- Audit table: `bitacora` (JSON columns for state snapshots)
