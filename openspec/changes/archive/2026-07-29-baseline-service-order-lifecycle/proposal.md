## Why

Este proyecto ya tiene tres casos de uso implementados y funcionando, pero nunca se documentó su comportamiento como spec formal. Se adopta OpenSpec a partir de ahora, y como el proyecto es brownfield, el primer cambio no agrega funcionalidad nueva: establece la línea base ("baseline") de specs que describen el comportamiento actual del ciclo de vida de una orden de servicio, para que futuros cambios (por ejemplo, agregar el estado `Closed`) tengan un punto de partida documentado contra el cual generar deltas.

## What Changes

- Se documenta como spec el comportamiento ya implementado de creación, asignación de técnico y consulta de órdenes de servicio.
- Se documenta la validación de duplicados al crear una orden (mismo cliente + mismo equipo).
- Se documenta el manejo global de excepciones (middleware) que traduce excepciones de dominio/aplicación a códigos HTTP.
- No se modifica código, arquitectura ni comportamiento existente — es retrofit puro.

## Capabilities

### New Capabilities
- `service-order-lifecycle`: creación de órdenes (con validación de duplicados), asignación de técnico (transición Pending → InProgress) y consulta de órdenes por estado, incluyendo el mapeo de errores a respuestas HTTP vía el middleware global de excepciones.

### Modified Capabilities
(ninguna — es la primera spec del proyecto, no hay specs previas que modificar)

## Impact

- Código afectado (solo lectura/verificación, sin cambios): `ServiceOrders.API.Controllers.ServiceOrdersController`, `ServiceOrders.API.Middleware.ExceptionHandlerMiddleware`, `ServiceOrders.Application.Commands.CreateServiceOrder.CreateServiceOrderHandler`, `ServiceOrders.Application.Commands.AssignTechnician.AssignTechnicianHandler`, `ServiceOrders.Application.Queries.GetOrdersByStatus.GetOrdersByStatusHandler`.
- No hay impacto en dependencias, base de datos ni APIs externas.
- Resultado: `openspec/specs/service-order-lifecycle/spec.md` queda como fuente de verdad viva del comportamiento actual, para comparar contra ella en el próximo cambio real (ej. agregar `Closed`).
