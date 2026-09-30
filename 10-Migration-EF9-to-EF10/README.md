# Parte 10 — Migración EF Core 9 → EF Core 10

## Objetivo
Migrar una API real de EF Core 9 a EF Core 10 y comparar compilación, tooling, migraciones y SQL.

## Stack final
Visual Studio 2026 · .NET 10 · EF Core 10.0.12 · SQL Server Express.

## Flujo
1. Restaurar el `.csproj` de `EF9-BASELINE`.
2. Build EF9 y generar/aplicar `InitialCreate` si el lab comienza desde cero.
3. Ejecutar el `.http` y guardar SQL EF9.
4. Cambiar a `net10.0` y paquetes 10.0.12.
5. Build.
6. Conservar migraciones históricas y generar `EfCore10UpgradeCheck`.
7. Inspeccionar esa migración antes de aplicarla.
8. Repetir exactamente las pruebas y comparar SQL.

## Importante
EF Core 10 requiere SDK/runtime .NET 10. No se incluyen migraciones inventadas: deben generarse en el entorno real.
