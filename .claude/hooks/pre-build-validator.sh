#!/bin/bash
# =============================================================================
# Hook: pre-build-validator
# Propósito: Validar la integridad del proyecto antes de ejecutar dotnet build.
#            Se ejecuta automáticamente antes de comandos de compilación para
#            detectar problemas comunes de Clean Architecture y configuración.
# Evento: PreToolUse — intercepta llamadas de Bash que contengan "dotnet build"
# =============================================================================

PROJECT_ROOT="$(git rev-parse --show-toplevel 2>/dev/null || pwd)"
ERRORS=0

echo "🔍 [pre-build-validator] Validando proyecto..."

# 1. Verificar que CLAUDE.md existe
if [ ! -f "$PROJECT_ROOT/CLAUDE.md" ]; then
  echo "⚠️  CLAUDE.md no encontrado en la raíz del proyecto."
  ERRORS=$((ERRORS + 1))
fi

# 2. Verificar que el archivo de conexión tiene la cadena de conexión configurada
APPSETTINGS="$PROJECT_ROOT/src/ServiceOrders.API/appsettings.json"
if [ -f "$APPSETTINGS" ]; then
  if ! grep -q "DefaultConnection" "$APPSETTINGS"; then
    echo "❌ appsettings.json no contiene 'DefaultConnection'. La API no podrá conectarse a SQLite."
    ERRORS=$((ERRORS + 1))
  fi
else
  echo "❌ appsettings.json no encontrado en src/ServiceOrders.API/"
  ERRORS=$((ERRORS + 1))
fi

# 3. Verificar que la capa Domain no referencia proyectos externos (Clean Architecture)
DOMAIN_CSPROJ="$PROJECT_ROOT/src/ServiceOrders.Domain/ServiceOrders.Domain.csproj"
if [ -f "$DOMAIN_CSPROJ" ]; then
  if grep -q "ProjectReference" "$DOMAIN_CSPROJ"; then
    echo "❌ ServiceOrders.Domain contiene ProjectReferences — viola Clean Architecture (Domain debe ser independiente)."
    ERRORS=$((ERRORS + 1))
  fi
fi

# 4. Verificar que existen migraciones de EF Core
MIGRATIONS_DIR="$PROJECT_ROOT/src/ServiceOrders.Infrastructure/Migrations"
if [ ! -d "$MIGRATIONS_DIR" ] || [ -z "$(ls -A "$MIGRATIONS_DIR" 2>/dev/null)" ]; then
  echo "⚠️  No se encontraron migraciones en Infrastructure/Migrations. Ejecuta: dotnet ef migrations add InitialCreate"
  ERRORS=$((ERRORS + 1))
fi

# Resultado final
if [ $ERRORS -eq 0 ]; then
  echo "✅ [pre-build-validator] Todas las validaciones pasaron. Procediendo con el build."
  exit 0
else
  echo "🚫 [pre-build-validator] Se encontraron $ERRORS problema(s). Revisa los errores antes de compilar."
  exit 1
fi
