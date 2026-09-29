# Parte 07 — Vector Search con EF Core 10 y SQL Server 2025

Laboratorio reproducible de la serie **EF Core 10 New Features**.

## Objetivo

Demostrar el soporte introducido en EF Core 10 para el tipo `vector` de SQL Server 2025 y la traducción de `EF.Functions.VectorDistance()` a `VECTOR_DISTANCE()`.

## Stack

- Visual Studio 2026
- .NET 10 / C# 14
- EF Core 10.0.12
- ASP.NET Core Web API con Controllers
- SQL Server 2025 Express
- SQL Server compatibility level 170

## Importante

Los vectores del laboratorio tienen 3 dimensiones y son manuales. Sirven para estudiar EF Core y SQL Server sin depender de un proveedor de embeddings. En una aplicación real, el embedding normalmente se genera fuera de SQL Server mediante un modelo y todas las dimensiones deben coincidir.

## Base

La conexión preparada es:

`Server=.\SQLEXPRESS;Database=VectorSearchDb;Trusted_Connection=True;TrustServerCertificate=True`

Si tu instancia tiene otro nombre, cambiá solamente la connection string.

## Antes de migrar

En SSMS verificá:

```sql
SELECT
    SERVERPROPERTY('ProductVersion') AS ProductVersion,
    SERVERPROPERTY('Edition') AS Edition;
```

Esperamos SQL Server 2025 / versión 17.x.

Después de crear la base, verificá:

```sql
SELECT name, compatibility_level
FROM sys.databases
WHERE name = 'VectorSearchDb';
```

El objetivo es compatibility level 170.

## Migración

Desde Package Manager Console:

```powershell
Add-Migration InitialCreate
Update-Database
```

Las migraciones NO están incluidas en este SET porque todavía no fueron generadas ni ejecutadas en tu máquina.

## Pruebas

Ejecutar `VectorSearch.Api.http` de arriba hacia abajo.

La consulta central es:

```csharp
EF.Functions.VectorDistance(
    "cosine",
    document.Embedding,
    queryVector)
```

EF Core 10 debe traducirla a la función SQL Server:

```sql
VECTOR_DISTANCE('cosine', ...)
```

## Evidencia

Los logs de `Microsoft.EntityFrameworkCore.Database.Command` están habilitados para poder capturar el SQL real.

## Alcance

Este laboratorio usa búsqueda exacta. `VECTOR_DISTANCE()` calcula distancia contra las filas candidatas. No demuestra índices vectoriales ni ANN como si fueran parte estable del alcance original de EF Core 10.
