## Purpose

Define el ciclo de vida de una orden de servicio técnico: su creación, la asignación de un técnico y la consulta de órdenes existentes por estado, incluyendo cómo se comunican los errores al consumidor de la API.

## ADDED Requirements

### Requirement: Creación de orden de servicio
El sistema SHALL permitir crear una nueva orden de servicio a partir de nombre de cliente, nombre de equipo y descripción del problema, y SHALL rechazar la creación si ya existe una orden para el mismo cliente con el mismo equipo.

#### Scenario: Creación exitosa
- **WHEN** se envía `POST /api/serviceorders` con `customerName`, `equipmentName` y `problemDescription` válidos, y no existe una orden previa para ese cliente y equipo
- **THEN** el sistema crea la orden con estado `Pending`, la persiste y responde `201 Created` con el `id` de la orden creada

#### Scenario: Rechazo por orden duplicada
- **WHEN** se envía `POST /api/serviceorders` con `customerName` y `equipmentName` que coinciden con los de una orden ya existente
- **THEN** el sistema no crea una nueva orden y responde `400 Bad Request` con un mensaje indicando que ya existe una orden para ese cliente y equipo

### Requirement: Asignación de técnico
El sistema SHALL permitir asignar un técnico a una orden existente, transicionando su estado de `Pending` a `InProgress`, y SHALL rechazar la asignación si la orden no existe.

#### Scenario: Asignación exitosa
- **WHEN** se envía `PATCH /api/serviceorders/{id}/assign` con `technicianName` para una orden existente
- **THEN** el sistema asigna el técnico a la orden, cambia su estado a `InProgress` y responde `204 No Content`

#### Scenario: Asignación a orden inexistente
- **WHEN** se envía `PATCH /api/serviceorders/{id}/assign` con un `id` que no corresponde a ninguna orden
- **THEN** el sistema responde `404 Not Found` con un mensaje indicando que la orden no fue encontrada

### Requirement: Consulta de órdenes por estado
El sistema SHALL permitir consultar las órdenes de servicio filtradas por estado, y SHALL usar `Pending` como valor por defecto cuando no se especifica un estado.

#### Scenario: Consulta con estado válido
- **WHEN** se envía `GET /api/serviceorders?status=InProgress`
- **THEN** el sistema responde `200 OK` con la lista de órdenes cuyo estado es `InProgress`

#### Scenario: Consulta sin parámetro de estado
- **WHEN** se envía `GET /api/serviceorders` sin el parámetro `status`
- **THEN** el sistema interpreta el estado como `Pending` y responde `200 OK` con la lista de órdenes en ese estado

#### Scenario: Consulta con estado inválido
- **WHEN** se envía `GET /api/serviceorders?status=` con un valor que no corresponde a ningún estado válido (`Pending`, `InProgress`, `Closed`)
- **THEN** el sistema responde `400 Bad Request` con un mensaje indicando que el valor de estado no es válido

### Requirement: Manejo global de errores
El sistema SHALL traducir las excepciones no controladas producidas durante el procesamiento de una petición a una respuesta HTTP con código y cuerpo JSON consistentes, sin exponer detalles internos para errores no anticipados.

#### Scenario: Error de recurso no encontrado
- **WHEN** un handler lanza `KeyNotFoundException` durante el procesamiento de una petición
- **THEN** el sistema responde `404 Not Found` con un cuerpo JSON `{ "error": "<mensaje>", "statusCode": 404 }`

#### Scenario: Error de operación inválida o argumento inválido
- **WHEN** un handler lanza `InvalidOperationException` o `ArgumentException` durante el procesamiento de una petición
- **THEN** el sistema responde `400 Bad Request` con un cuerpo JSON `{ "error": "<mensaje>", "statusCode": 400 }`

#### Scenario: Error no anticipado
- **WHEN** ocurre cualquier otra excepción no controlada durante el procesamiento de una petición
- **THEN** el sistema responde `500 Internal Server Error` con un cuerpo JSON genérico que no revela el detalle interno de la excepción, y registra el error en el log
