# 05 - Dynamic ExecuteUpdate

Laboratorio de la serie **EF Core 10 New Features**.

## Qué problema estudia

`ExecuteUpdate` ya permitía actualizar filas sin cargar entidades desde EF Core 7. El problema aparecía cuando **la lista de propiedades a actualizar se decidía dinámicamente en runtime**.

En EF Core 9 y anteriores, el argumento `setters` era un `Expression<Func<...>>`. Para agregar un `SetProperty` sólo cuando se cumplía una condición había que construir o modificar manualmente el Expression Tree.

EF Core 10 cambia ese argumento a una lambda regular. Ahora podemos usar `if` normales dentro del bloque de setters.

## Qué demuestra el laboratorio

- baseline con un `ExecuteUpdateAsync` de forma fija;
- update dinámico de `Name`, `Price`, `Stock` e `IsActive`;
- distintos requests producen distintas listas de setters;
- cantidad de filas afectadas;
- caso negativo sin propiedades;
- SQL logging preparado para observar el `UPDATE` real.

## Stack

- Visual Studio 2026
- .NET 10
- C# 14
- EF Core 10.0.12
- ASP.NET Core Web API
- SQL Server

## Migración

Este SET no contiene una migración fabricada. El entorno donde se generó no dispone del SDK .NET/SQL Server para crearla y ejecutarla.

En Package Manager Console:

```powershell
Add-Migration InitialCreate
Update-Database
```

Después de ejecutarla localmente, la migración real debe incorporarse al repositorio.

## Orden de prueba

Usar `DynamicExecuteUpdate.Api.http` en el orden numerado.

La evidencia principal está en el Output de Visual Studio. Con logging de `Microsoft.EntityFrameworkCore.Database.Command` en `Information`, comparar el SQL de:

1. update fijo de `Price`;
2. dinámico sólo `Price`;
3. dinámico `Name + Stock`;
4. dinámico sólo `IsActive`.

## Qué esperar

Cada llamada dinámica debe construir un `UPDATE` cuyo `SET` contenga únicamente las propiedades cuyo `if` fue ejecutado.

Estado: **ESPERADO**, pendiente de ejecución local. No se incluye SQL inventado.

## Importante

`ExecuteUpdate`:

- ejecuta inmediatamente;
- no sincroniza el Change Tracker;
- no agrupa automáticamente múltiples invocaciones en una única transacción;
- no aplica automáticamente control de concurrencia optimista como `SaveChanges`;
- devuelve la cantidad de filas afectadas, que puede utilizarse para implementar concurrencia manual.

## Fuente oficial

Microsoft Learn — What's New in EF Core 10, sección **ExecuteUpdateAsync now accepts a regular, non-expression lambda**.
