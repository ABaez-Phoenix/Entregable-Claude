# Skill: scaffold-cqrs-handler

## Descripción
Genera el scaffold completo de un Command o Query CQRS para este proyecto.
Crea el archivo de Command/Query y su Handler siguiendo las convenciones establecidas.

## Cuándo usar
Cuando necesitas agregar un nuevo caso de uso al sistema y quieres generar
la estructura base de Command + Handler o Query + Handler en la capa Application.

## Instrucciones para Claude

Cuando el usuario invoque este skill con `/scaffold-cqrs-handler`, solicita:
1. Tipo: ¿Command o Query?
2. Nombre del caso de uso (ej: `CloseServiceOrder`, `GetOrderById`)
3. Tipo de retorno del Handler

Luego genera los archivos siguiendo estas reglas:

### Para un Command:
- Archivo: `src/ServiceOrders.Application/Commands/{NombreCommand}/{NombreCommand}Command.cs`
- Archivo: `src/ServiceOrders.Application/Commands/{NombreCommand}/{NombreCommand}Handler.cs`

```csharp
// {NombreCommand}Command.cs
using MediatR;

namespace ServiceOrders.Application.Commands.{NombreCommand};

public record {NombreCommand}Command(/* parámetros */) : IRequest<{TipoRetorno}>;
```

```csharp
// {NombreCommand}Handler.cs
using MediatR;
using ServiceOrders.Domain.Interfaces;

namespace ServiceOrders.Application.Commands.{NombreCommand};

public class {NombreCommand}Handler : IRequestHandler<{NombreCommand}Command, {TipoRetorno}>
{
    private readonly IServiceOrderRepository _repository;

    public {NombreCommand}Handler(IServiceOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<{TipoRetorno}> Handle({NombreCommand}Command request, CancellationToken cancellationToken)
    {
        // TODO: implementar lógica
        throw new NotImplementedException();
    }
}
```

### Para una Query:
- Archivo: `src/ServiceOrders.Application/Queries/{NombreQuery}/{NombreQuery}Query.cs`
- Archivo: `src/ServiceOrders.Application/Queries/{NombreQuery}/{NombreQuery}Handler.cs`

```csharp
// {NombreQuery}Query.cs
using MediatR;
using ServiceOrders.Application.Common;

namespace ServiceOrders.Application.Queries.{NombreQuery};

public record {NombreQuery}Query(/* parámetros */) : IRequest<{TipoRetorno}>;
```

```csharp
// {NombreQuery}Handler.cs
using MediatR;
using ServiceOrders.Domain.Interfaces;

namespace ServiceOrders.Application.Queries.{NombreQuery};

public class {NombreQuery}Handler : IRequestHandler<{NombreQuery}Query, {TipoRetorno}>
{
    private readonly IServiceOrderRepository _repository;

    public {NombreQuery}Handler(IServiceOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<{TipoRetorno}> Handle({NombreQuery}Query request, CancellationToken cancellationToken)
    {
        // TODO: implementar lógica
        throw new NotImplementedException();
    }
}
```

### Después de generar los archivos:
- Recordar al usuario que el Handler se registra automáticamente con MediatR via `RegisterServicesFromAssembly`
- Sugerir crear el test unitario correspondiente en `tests/ServiceOrders.Tests/`
- Verificar que el nuevo endpoint se agregue en `ServiceOrdersController.cs` si aplica
