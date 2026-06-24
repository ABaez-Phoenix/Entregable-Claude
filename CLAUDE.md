# CLAUDE.md — Service Orders API

## Proyecto
Web API REST para gestión de órdenes de servicio técnico.  
Asignación interna de Phoenix Calibration DR — Claude Code in Practice.

## Stack
- **Runtime:** .NET 10
- **Arquitectura:** Clean Architecture + CQRS con MediatR
- **ORM:** Entity Framework Core 10 con SQLite
- **Tests:** xUnit + NSubstitute
- **Documentación API:** Swagger / Swashbuckle

## Estructura del repositorio
```
src/
  ServiceOrders.Domain/        # Entidades, enums e interfaces (sin dependencias externas)
  ServiceOrders.Application/   # Handlers CQRS, DTOs, DI registration
  ServiceOrders.Infrastructure/ # EF Core, repositorios, migraciones
  ServiceOrders.API/           # Controllers, Program.cs, appsettings
tests/
  ServiceOrders.Tests/         # Tests unitarios con xUnit + NSubstitute
.claude/
  skills/                      # Custom skills de Claude Code
  hooks/                       # Custom hooks de Claude Code
```

## Convenciones de código
- Clases de dominio con **private setters** — mutación solo a través de métodos de negocio
- Handlers siempre retornan `Task` o `Task<T>` — no bloquear con `.Result`
- Registrar dependencias exclusivamente en `DependencyInjection.cs` por capa
- No usar `var` cuando el tipo no es evidente en la misma línea
- Nombres en **inglés**, comentarios XML en español para clases de dominio

## Casos de uso implementados
| Endpoint | Método | Descripción |
|----------|--------|-------------|
| `POST /api/serviceorders` | Command | Crear nueva orden |
| `PATCH /api/serviceorders/{id}/assign` | Command | Asignar técnico |
| `GET /api/serviceorders?status={status}` | Query | Consultar por estado |

## Estados válidos de una orden
`Pending` → `InProgress` (al asignar técnico) → `Closed` (futuro)

## Comandos frecuentes
```bash
# Compilar
dotnet build

# Ejecutar tests
dotnet test

# Aplicar migraciones manualmente
dotnet ef database update --project src/ServiceOrders.Infrastructure --startup-project src/ServiceOrders.API

# Agregar migración nueva
dotnet ef migrations add <NombreMigracion> --project src/ServiceOrders.Infrastructure --startup-project src/ServiceOrders.API

# Ejecutar API
dotnet run --project src/ServiceOrders.API
```

## Restricciones
- **No** modificar entidades del dominio desde fuera de la capa Domain
- **No** inyectar `AppDbContext` directamente en la capa Application — usar solo `IServiceOrderRepository`
- **No** agregar lógica de negocio en los Controllers
- La base de datos SQLite (`serviceorders.db`) se crea automáticamente al iniciar la API
