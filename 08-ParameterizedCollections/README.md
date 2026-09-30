# Parte 08 — Parameterized Collections en EF Core 10

## Objetivo

Comparar cómo EF Core 10 traduce una colección usada con `Contains` y aislar el cambio real de versión: **MultipleParameters es ahora la estrategia predeterminada**.

## Stack

- Visual Studio 2026
- .NET 10 / C# 14
- EF Core 10.0.12
- ASP.NET Core Web API / Controllers
- SQL Server

## Base

`ParameterizedCollectionsDb`

## Qué vamos a comparar

1. Default EF10: múltiples parámetros escalares.
2. `EF.Constant(ids)`: constantes inline.
3. `EF.Parameter(ids)`: un parámetro de colección; SQL Server usa JSON/OPENJSON.
4. `EF.MultipleParameters(ids)`: fuerza explícitamente múltiples parámetros.
5. Una colección de 8 elementos para observar el padding documentado por EF10.

## Historia de versiones

- Antes de EF8: valores inline como constantes.
- EF8: JSON parameter se convirtió en default para colecciones parametrizadas.
- EF9: control explícito de estrategia por consulta/configuración.
- EF10: MultipleParameters pasa a ser el default.

## Migración

Package Manager Console:

```powershell
Add-Migration InitialCreate
Update-Database
```

Las migraciones no están incluidas porque deben ser generadas y ejecutadas en el laboratorio real.

## Evidencia

Los logs SQL de EF están habilitados. Ejecutar `ParameterizedCollections.Api.http` de arriba hacia abajo y guardar el SQL real de cada estrategia.

No asumir que una estrategia es universalmente más rápida. Microsoft advierte que la elección óptima depende del tamaño de las colecciones, patrones de consulta y datos.
