# EVIDENCE.md — Evidencias de uso de Claude Code

Este archivo documenta el uso de cada uno de los 8 temas requeridos de Claude Code
aplicados durante el desarrollo del sistema de órdenes de servicio técnico.

---

## Tema 1: Plan Mode + Ask Mode

**Descripción:** Antes de implementar la arquitectura del proyecto, se utilizó Plan Mode
para definir la estructura de capas, las dependencias entre proyectos y la estrategia de CQRS.

**Plan generado:**
- Capa Domain: entidades con private setters, enum `OrderStatus`, interfaz `IServiceOrderRepository`
- Capa Application: Commands (`CreateServiceOrder`, `AssignTechnician`) y Query (`GetOrdersByStatus`) con MediatR
- Capa Infrastructure: `AppDbContext` con EF Core + SQLite, `ServiceOrderConfiguration`, `ServiceOrderRepository`
- Capa API: `ServiceOrdersController` con 3 endpoints REST

**Decisión clave tomada en Ask Mode:** Usar `IRequest` sin tipo de retorno para el Command `AssignTechnician`
(retorna `Unit` implícito), ya que la operación no produce un recurso nuevo sino una actualización.

> 📸 _Captura del plan en Plan Mode — ver `/evidence/plan-mode-screenshot.png`_

---

## Tema 2: /init y CLAUDE.md

**Descripción:** Se utilizó `/init` para generar el contexto inicial del proyecto y se configuró
el archivo `CLAUDE.md` en la raíz con:
- Stack tecnológico completo
- Estructura de carpetas del repositorio
- Convenciones de código del proyecto
- Comandos frecuentes de desarrollo
- Restricciones de arquitectura

**Archivo generado:** [`CLAUDE.md`](./CLAUDE.md)

---

## Tema 3: Test-Driven Iteration (TDD)

### Ciclo TDD — `ServiceOrder.AssignTechnician`

**Paso 1 — Prueba escrita (ROJO):**
Se escribió primero el test `AssignTechnician_WhenOrderIsPending_TransitionsToInProgress`
antes de implementar el método `AssignTechnician` en la entidad.

```
❌ FAILED: AssignTechnician_WhenOrderIsPending_TransitionsToInProgress
   Error: 'ServiceOrder' does not contain a definition for 'AssignTechnician'
```

**Paso 2 — Implementación mínima:**
Se implementó `AssignTechnician` en `ServiceOrder.cs` con la validación de estado y
la transición a `InProgress`.

**Paso 3 — Prueba pasando (VERDE):**
```
✅ PASSED: AssignTechnician_WhenOrderIsPending_TransitionsToInProgress (2ms)
✅ PASSED: AssignTechnician_WhenOrderIsAlreadyInProgress_ThrowsInvalidOperationException (1ms)
✅ PASSED: AssignTechnician_WithEmptyName_ThrowsArgumentException (1ms)
```

**Resultado final:**
```
Passed! - Failed: 0, Passed: 13, Skipped: 0, Total: 13
```

**Archivos de test:** [`tests/ServiceOrders.Tests/Domain/ServiceOrderTests.cs`](./tests/ServiceOrders.Tests/Domain/ServiceOrderTests.cs)

---

## Tema 4: Documentation Guidelines

**Descripción:** Se generó con Claude Code:

1. **README.md** — Documentación completa del proyecto con instrucciones de ejecución,
   descripción de endpoints y stack tecnológico. Ver [`README.md`](./README.md)

2. **XML Comments** — Comentarios XML completos en la clase de dominio `ServiceOrder`:
   - Documentación de cada propiedad pública
   - Documentación de métodos `Create` y `AssignTechnician` con parámetros y excepciones
   - Documentación del enum `OrderStatus` con cada valor

   Ver: [`src/ServiceOrders.Domain/Entities/ServiceOrder.cs`](./src/ServiceOrders.Domain/Entities/ServiceOrder.cs)

---

## Tema 5: Security Review

**Descripción:** Se solicitó a Claude una revisión de seguridad del endpoint más crítico:
`POST /api/serviceorders` (creación de órdenes).

**Hallazgos identificados y mitigados:**

| # | Hallazgo | Severidad | Estado |
|---|----------|-----------|--------|
| 1 | `SQLitePCLRaw.lib.e_sqlite3` 2.1.11 tiene vulnerabilidad conocida (GHSA-2m69-gcr7-jv3q) | Alta | ⚠️ Documentado — pendiente actualización de paquete |
| 2 | No hay validación de longitud máxima en el body del request antes de llegar al dominio | Media | ✅ Mitigado con `HasMaxLength` en EF Core Configuration |
| 3 | `ProblemDescription` acepta cualquier texto sin sanitización | Baja | ✅ Aceptable — API interna, no renderiza HTML |
| 4 | No hay rate limiting en los endpoints | Media | 📝 Registrado para implementación futura |
| 5 | `UseHttpsRedirection` activo — conexiones HTTP redirigen a HTTPS | Informativo | ✅ Correcto |

**Confirmaciones (no vulnerabilidades):**
- No se exponen stack traces en errores de producción
- Las excepciones de dominio retornan mensajes controlados (`KeyNotFoundException` → 404, `InvalidOperationException` → 400)
- No hay SQL injection posible — EF Core usa parámetros seguros

> 📸 _Ver `/evidence/security-review-session.png` para la conversación con Claude_

---

## Tema 6: GitHub MCP Integration

**Descripción:** Se utilizó la integración de GitHub CLI (gh) para ejecutar acciones reales sobre el repositorio desde Claude Code:

- ✅ Push del commit inicial al repositorio `ABaez-Phoenix/Entregable-Claude`
- ✅ Autenticación con `gh auth login` (cuenta: ABaez-Phoenix)
- ✅ Creación del issue [#1 — Security: Actualizar SQLitePCLRaw.lib.e_sqlite3 - vulnerabilidad GHSA-2m69-gcr7-jv3q](https://github.com/ABaez-Phoenix/Entregable-Claude/issues/1) directamente desde Claude Code, documentando el hallazgo del Security Review

---

## Tema 7: Custom Skill

**Descripción:** Se creó el skill `scaffold-cqrs-handler` que genera automáticamente
el scaffold de Commands y Queries CQRS siguiendo las convenciones del proyecto.

**Archivo:** [`.claude/skills/scaffold-cqrs-handler.md`](./.claude/skills/scaffold-cqrs-handler.md)

**Utilidad:** Evita escribir el boilerplate repetitivo de Command + Handler o Query + Handler
cada vez que se agrega un nuevo caso de uso, garantizando consistencia con las convenciones
del proyecto (namespace, inyección de repositorio, estructura de carpetas).

**Ejemplo de uso:** `/scaffold-cqrs-handler` → Claude solicita tipo (Command/Query) y nombre,
luego genera los dos archivos con la estructura correcta y recuerda agregar el endpoint al controller.

---

## Tema 8: Custom Hook

**Descripción:** Se implementó el hook `pre-build-validator` que se ejecuta antes de
comandos de compilación (`dotnet build`) para validar la integridad del proyecto.

**Archivo:** [`.claude/hooks/pre-build-validator.sh`](./.claude/hooks/pre-build-validator.sh)

**Validaciones que realiza:**
1. Verifica que `CLAUDE.md` existe en la raíz
2. Verifica que `appsettings.json` tiene la cadena de conexión configurada
3. Verifica que la capa Domain **no** tiene referencias a otros proyectos (Clean Architecture)
4. Verifica que existen migraciones de EF Core

**Valor agregado:** Captura errores de configuración antes de la compilación,
reduciendo el ciclo de detección de problemas de setup.
