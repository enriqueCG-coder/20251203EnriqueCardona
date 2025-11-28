# Prueba Técnica desarrollada en .NET 8 utilizando EF y C#.
La solución está compuesta por 2 proyectos configurados para arrancar dualmente (al mismo tiempo). Un proyecto API y un proyecto web MVC.

El modelado de la DB fue creado a partir de los modelos utilizando EF.

PASOS PARA EJECUTAR LA SOLUCIÓN:
1. Colocar la ruta de base de datos en el archivo appsettings.json y en la clase InventarioContextFactory.cs
2. Ejecutar el comando: dotnet ef migrations add init --project .\PruebaTec.API\
3. Ejecutar el comando: dotnet ef database update --project .\PruebaTec.API\
4. Ejecutar el archivo de Procedimientos almacenados
5. F5 a la solución
