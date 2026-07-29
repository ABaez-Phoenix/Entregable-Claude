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
  commands/opsx/               # Slash commands de OpenSpec (propose, apply, archive, etc.)
openspec/
  config.yaml                  # Contexto del proyecto para OpenSpec
  specs/                       # Specs vivas (fuente de verdad del comportamiento actual)
  changes/                     # Propuestas de cambio en curso
  changes/archive/             # Cambios ya implementados y archivados
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

## Flujo de trabajo: OpenSpec (spec-driven development)
Todo cambio de comportamiento (endpoint nuevo, transición de estado nueva, regla de negocio nueva) arranca con una propuesta antes de tocar código:
1. `/opsx:propose <nombre-del-cambio>` — genera `proposal.md`, `specs/` (delta) y `tasks.md` en `openspec/changes/<nombre>/` para revisar y ajustar antes de implementar.
2. `/opsx:apply` — implementa las tareas del checklist, marcándolas como completadas.
3. `/opsx:archive` — sincroniza la spec delta contra `openspec/specs/` (la spec viva) y archiva el cambio en `openspec/changes/archive/`.

`openspec/specs/service-order-lifecycle/spec.md` es la spec base (retrofit del comportamiento ya implementado). Cualquier cambio a ese comportamiento (ej. agregar el estado `Closed`) debe generar un delta contra esa spec, no reemplazarla directamente.

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
- **No** implementar cambios de comportamiento directamente — pasan primero por `/opsx:propose` (ver sección "Flujo de trabajo: OpenSpec")
