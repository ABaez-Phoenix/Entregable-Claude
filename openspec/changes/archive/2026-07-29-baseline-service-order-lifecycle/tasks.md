## 1. Verificación de creación de orden

- [x] 1.1 Confirmar que `CreateServiceOrderHandler` rechaza una orden duplicada (mismo cliente + mismo equipo) lanzando `InvalidOperationException`, mapeada por el middleware a `400 Bad Request`
- [x] 1.2 Confirmar que una creación válida persiste la orden en estado `Pending` y que el controller responde `201 Created` con el `id`

## 2. Verificación de asignación de técnico

- [x] 2.1 Confirmar que `AssignTechnicianHandler` lanza `KeyNotFoundException` cuando el `id` de la orden no existe, mapeada por el middleware a `404 Not Found`
- [x] 2.2 Confirmar que una asignación válida cambia el estado de la orden a `InProgress` y que el controller responde `204 No Content`

## 3. Verificación de consulta por estado

- [x] 3.1 Confirmar que `GET /api/serviceorders` sin parámetro `status` usa `Pending` como valor por defecto
- [x] 3.2 Confirmar que un valor de `status` inválido responde `400 Bad Request` con el mensaje de valores válidos

## 4. Verificación del manejo global de errores

- [x] 4.1 Confirmar que `ExceptionHandlerMiddleware` mapea `KeyNotFoundException` → 404, `InvalidOperationException`/`ArgumentException` → 400, y cualquier otra excepción → 500 con mensaje genérico
- [x] 4.2 Confirmar que toda excepción no controlada queda registrada en el log antes de responder

## 5. Cierre del baseline

- [x] 5.1 Ejecutar `dotnet build` y `dotnet test` para confirmar que no hubo regresiones durante este retrofit
- [x] 5.2 Correr `openspec validate --all` para confirmar que la spec generada es estructuralmente válida
- [x] 5.3 Archivar el cambio con `/opsx:archive` para que `openspec/specs/service-order-lifecycle/spec.md` quede como la spec viva del proyecto
