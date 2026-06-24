# Service Orders API

Web API REST para gestión de órdenes de servicio técnico, construida con .NET 10, Clean Architecture y CQRS.

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- No requiere instalación de base de datos (SQLite embebido)

## Ejecutar el proyecto

```bash
# Clonar el repositorio
git clone https://github.com/ABaez-Phoenix/Entregable-Claude.git
cd Entregable-Claude

# Restaurar dependencias
dotnet restore

# Ejecutar la API (las migraciones se aplican automáticamente al iniciar)
dotnet run --project src/ServiceOrders.API
```

La API estará disponible en `https://localhost:7xxx` o `http://localhost:5xxx`.  
Swagger UI: `https://localhost:{puerto}/swagger`

## Ejecutar los tests

```bash
dotnet test
```

## Endpoints

### 1. Crear orden de servicio
```http
POST /api/serviceorders
Content-Type: application/json

{
  "customerName": "Juan Pérez",
  "equipmentName": "Laptop Dell XPS 15",
  "problemDescription": "La pantalla parpadea al encender"
}
```
**Respuesta:** `201 Created` con `{ "id": "guid" }`

---

### 2. Asignar técnico a una orden
```http
PATCH /api/serviceorders/{id}/assign
Content-Type: application/json

{
  "technicianName": "Carlos Méndez"
}
```
**Respuesta:** `204 No Content`  
**Errores:** `404` si no existe la orden · `400` si el estado no permite asignación

---

### 3. Consultar órdenes por estado
```http
GET /api/serviceorders?status=Pending
GET /api/serviceorders?status=InProgress
GET /api/serviceorders?status=Closed
```
**Respuesta:** `200 OK` con lista de órdenes

---

## Arquitectura

```
Domain      → Entidades + reglas de negocio (sin dependencias externas)
Application → Handlers CQRS + MediatR (depende solo de Domain)
Infrastructure → EF Core + SQLite + repositorios (implementa interfaces de Domain)
API         → Controllers HTTP (orquesta Application)
```

## Stack

| Tecnología | Uso |
|------------|-----|
| .NET 10 | Runtime |
| ASP.NET Core | Web API |
| MediatR 14 | CQRS |
| EF Core 10 + SQLite | Persistencia |
| xUnit + NSubstitute | Tests unitarios |
| Swashbuckle | Documentación Swagger |
