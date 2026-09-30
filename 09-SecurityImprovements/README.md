# Parte 09 — Security Improvements en EF Core 10

## Objetivo

Demostrar dos mejoras de seguridad introducidas específicamente en EF Core 10:

1. Redacción por defecto de constantes inline en el SQL registrado en logs.
2. Analyzer que advierte concatenaciones realizadas dentro de invocaciones Raw SQL.

## Stack

- Visual Studio 2026
- .NET 10 / C# 14
- EF Core 10.0.12
- ASP.NET Core Web API / Controllers
- SQL Server Express

## Base

`SecurityImprovementsDb`

## Experimento 1 — logging

`EF.Constant(roles)` fuerza valores inline. EF Core 10 sigue enviando los valores reales a SQL Server, pero el texto destinado al logging los redacta como `?` por defecto. `EnableSensitiveDataLogging()` permite volver a mostrar los valores deliberadamente.

## Experimento 2 — Raw SQL

`FromSql` con `FormattableString` parametriza valores. `FromSqlRaw` existe para SQL dinámico real, pero EF Core 10 incorpora un analyzer que advierte si la concatenación se realiza directamente dentro de una invocación Raw SQL.

El endpoint `raw-validated` muestra un caso legítimo: el identificador se selecciona mediante una whitelist y el valor se envía con `SqlParameter`.

## Migración

Package Manager Console:

```powershell
Add-Migration InitialCreate
Update-Database
```

Las migraciones no se incluyen en el set generado: deben ser creadas y ejecutadas en el laboratorio real.

## Pruebas

Ejecutar `SecurityImprovements.Api.http` de arriba hacia abajo. Guardar como evidencia:

- warning exacto del analyzer al compilar `raw-warning`;
- log de `roles-inline` sin SensitiveDataLogging;
- log de la misma consulta con SensitiveDataLogging;
- SQL/parameters de `safe-email`;
- respuesta válida e inválida de `raw-validated`.

## Importante

No confundir lo nuevo con capacidades anteriores. `FromSql`, `FromSqlRaw`, parametrización y `EnableSensitiveDataLogging()` ya existían. Lo nuevo en EF10 que estudia este lab es la redacción de constantes inline y el nuevo analyzer de concatenación Raw SQL.
