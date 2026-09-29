# 06 - LeftJoin y RightJoin

Laboratorio de la serie **EF Core 10 New Features**.

## Stack

- Visual Studio 2026
- .NET 10
- C# 14
- EF Core 10.0.12
- ASP.NET Core Web API
- SQL Server

## Qué problema estudiamos

Antes de .NET 10 no existían operadores LINQ de primera clase `LeftJoin` y `RightJoin`.

Un LEFT JOIN se expresaba normalmente mediante un patrón formado por:

```text
GroupJoin
+ SelectMany
+ DefaultIfEmpty
```

El patrón funcionaba, pero era más difícil de leer, escribir y reconocer.

## Qué cambia

La responsabilidad está dividida:

- **.NET 10** incorpora `LeftJoin` y `RightJoin` a LINQ.
- **EF Core 10** reconoce esos nuevos operadores cuando se usan sobre `IQueryable` y los traduce a SQL.
- En SQL Server, EF Core 10 traduce `LeftJoin` a `LEFT JOIN` y `RightJoin` a `RIGHT JOIN`.

Por lo tanto, no es correcto afirmar simplemente que "EF Core 10 inventó LeftJoin".

## Qué demuestra el laboratorio

1. El patrón anterior para LEFT JOIN.
2. El nuevo `LeftJoin`.
3. Que ambos LEFT JOIN deben representar la misma semántica.
4. El nuevo `RightJoin`.
5. Una fila izquierda sin coincidencia: Carla no tiene órdenes.
6. Una fila derecha sin coincidencia: la orden con `CustomerId = 99` no tiene cliente.
7. El SQL debe inspeccionarse en Output de Visual Studio.
8. El reset restaura datos e IDENTITY para mantener el laboratorio reproducible.

## Datos

Customers:

```text
1 Ana
2 Bruno
3 Carla
```

Orders:

```text
1 CustomerId=1 Total=120
2 CustomerId=1 Total=85
3 CustomerId=2 Total=200
4 CustomerId=99 Total=50
```

No configuramos una foreign key física entre ambas tablas de forma deliberada: necesitamos conservar la orden 4 para demostrar un `RightJoin` sin coincidencia.

## Migraciones

Package Manager Console:

```powershell
Add-Migration InitialCreate
Update-Database
```

Las migraciones no se incluyen en este SET porque no fueron ejecutadas en el entorno de generación. No se fabrican evidencias.

## Orden recomendado

Ejecutar `LeftRightJoin.Api.http` de arriba hacia abajo.

## Qué observar

En Visual Studio, revisar:

```text
Output
→ Show output from: Debug
```

Con el logging de `Microsoft.EntityFrameworkCore.Database.Command` en `Information`.

Esperado según la documentación oficial:

- `LeftJoin` → `LEFT JOIN`
- `RightJoin` → `RIGHT JOIN`

La captura concreta del SQL debe realizarse al ejecutar el laboratorio localmente.
