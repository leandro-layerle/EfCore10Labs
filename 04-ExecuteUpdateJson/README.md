# 04 - ExecuteUpdate sobre JSON

Laboratorio de la serie **EF Core 10 New Features**.

## Stack

- Visual Studio 2026
- .NET 10
- C# 14
- EF Core 10.0.12
- ASP.NET Core Web API
- SQL Server

## Qué demuestra

- Complex Type mapeado a JSON con `ToJson()`.
- Diferencia entre `SaveChangesAsync()` y `ExecuteUpdateAsync()`.
- Actualización masiva de una propiedad interna de JSON sin cargar entidades.
- Actualización de múltiples propiedades del documento.
- Filtrado por una propiedad interna del JSON.
- Cantidad de filas afectadas.
- `ExecuteUpdateAsync()` ejecuta inmediatamente y no sincroniza el Change Tracker.

## Importante

`ExecuteUpdate` existe desde EF Core 7.

La novedad de EF Core 10 que estudiamos acá es:

```text
ExecuteUpdate
+
propiedades dentro de JSON relacional
```

El soporte requiere que el objeto JSON esté modelado como **Complex Type**.

No funciona para tipos JSON modelados como **Owned Entity Types**.

## Compatibilidad SQL Server

La prueba:

```csharp
.SetProperty(
    blog => blog.Details.Views,
    100)
```

es la prueba base del laboratorio.

La variante:

```csharp
.SetProperty(
    blog => blog.Details.Views,
    blog => blog.Details.Views + 1)
```

utiliza el valor actual de otra expresión/columna. En SQL Server, esta forma requiere capacidades modernas del motor/provider; SQL Server 2022+ es el escenario recomendado.

Con SQL Server 2025 y compatibilidad 170, EF Core 10 puede mapear el documento al tipo nativo `json` y generar `modify()`.

## Migraciones

Package Manager Console:

```powershell
Add-Migration InitialCreate
Update-Database
```

## Orden recomendado para probar

```text
POST /api/blogs/seed
GET  /api/blogs

POST /api/blogs/reset
POST /api/blogs/savechanges/increment-views?category=.NET
GET  /api/blogs

POST /api/blogs/reset
POST /api/blogs/executeupdate/set-views?category=.NET&views=100
GET  /api/blogs

POST /api/blogs/reset
POST /api/blogs/executeupdate/increment-views?category=.NET
GET  /api/blogs

POST /api/blogs/reset
POST /api/blogs/change-tracker-demo/1
```

Revisar el Output de Visual Studio para observar el SQL generado.
