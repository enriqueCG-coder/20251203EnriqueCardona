---------------------------------------------------------------------------------------
-- CRUD USUARIO
---------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_Usuario_Insertar
    @Nombre NVARCHAR(150),
    @Pwd NVARCHAR(255),
    @Rol NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO Usuario (Nombre, Pwd, Rol, IsActive)
        VALUES (@Nombre, @Pwd, @Rol, 1);

        SELECT SCOPE_IDENTITY() AS NuevoId;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO
---------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_Usuario_Modificar
    @Id INT,
    @Nombre NVARCHAR(150),
    @Pwd NVARCHAR(255),
    @Rol NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Usuario
    SET Nombre = @Nombre,
        Pwd = @Pwd,
        Rol = @Rol
    WHERE Id = @Id;
END;
GO
---------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_Usuario_Eliminar
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Usuario
    WHERE Id = @Id;
END;
GO
---------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_Usuario_Inactivar
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Usuario
    SET IsActive = 0
    WHERE Id = @Id;
END;
GO
---------------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE sp_Usuario_Activar
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Usuario
    SET IsActive = 1
    WHERE Id = @Id;
END;
GO
---------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_Usuario_Login
    @Nombre NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 Id, Nombre, Pwd, Rol, IsActive
    FROM Usuario
    WHERE Nombre = @Nombre;
END;
GO
---------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_Usuario_ID
    @ID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Nombre, Pwd, Rol, IsActive
    FROM Usuario
    WHERE Id = @ID;
END;
GO
---------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_Usuarios
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Nombre, Pwd, Rol, IsActive
    FROM Usuario
END;
GO


---------------------------------------------------------------------------------------
-- CRUD PRODUCTO
---------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_Producto_Insertar
    @Nombre NVARCHAR(150),
    @Precio FLOAT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO Producto (Nombre, Precio)
        VALUES (@Nombre, @Precio);

        SELECT SCOPE_IDENTITY() AS NuevoId; -- Retorna el ID generado
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO
---------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_Producto_Modificar
    @Id INT,
    @Nombre NVARCHAR(150),
    @Precio FLOAT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Producto
    SET Nombre = @Nombre,
        Precio = @Precio
    WHERE Id = @Id;
END;
GO
---------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_Producto_Eliminar
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM Producto
    WHERE Id = @Id;
END;
GO
---------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_Producto_Listar
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Nombre, Precio
    FROM Producto
    ORDER BY Nombre;
END;
GO
---------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_Producto_BuscarPorId
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 Id, Nombre, Precio
    FROM Producto
    WHERE Id = @Id;
END;
GO

---------------------------------------------------------------------------------------
-- CRUD VENTA
---------------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE SP_InsertarVenta
    @Codigo NVARCHAR(100),
    @IdUsuario INT
AS
BEGIN
    INSERT INTO Venta (Codigo, IdUsuario, Total, Estado, Fecha)
    VALUES (@Codigo, @IdUsuario, 0.00, 'Pendiente', GETDATE());

    SELECT SCOPE_IDENTITY() AS Id;
END;
go
---------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE SP_ConfirmarVenta
    @Id INT
AS
BEGIN
    UPDATE Venta SET Estado = 'Finalizada'
    WHERE Id = @Id;
END;
GO

---------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE SP_ListarVentas
	@IdUsuario INT
AS
BEGIN
    SELECT 
        V.Id,
		V.Codigo,
        V.Fecha,
        V.IdUsuario,
        U.Nombre AS NombreUsuario,
        V.Total,
		V.Estado
    FROM Venta V
    INNER JOIN Usuario U ON V.IdUsuario = U.Id
	WHERE U.Id = @IdUsuario ORDER BY V.Estado DESC;
END;
GO
---------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE SP_BuscarVentaPorId
    @Id INT,
	@IdUsuario INT
AS
BEGIN
    SELECT 
        V.Id,
		V.Codigo,
        V.Fecha,
        V.IdUsuario,
        U.Nombre AS Usuario,
        V.Total,
		V.Estado
    FROM Venta V
    INNER JOIN Usuario U ON V.IdUsuario = U.Id
    WHERE V.Id = @Id AND V.IdUsuario = @IdUsuario;
END;
GO

---------------------------------------------------------------------------------------
-- CRUD DETALLE
---------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE SP_InsertarDetalleVenta
    @IdVenta INT,
    @IdProducto INT,
    @Cantidad INT,
    @Precio DECIMAL(18,2),
    @Iva DECIMAL(18,2),
    @Total DECIMAL(18,2)
AS
BEGIN
    INSERT INTO DetalleVenta (IdVenta, IdProducto, Cantidad, Precio, Iva, Total, Fecha)
    VALUES (@IdVenta, @IdProducto, @Cantidad, @Precio, @Iva, @Total, GETDATE());

	UPDATE Venta SET Total = @Total WHERE Id = @IdVenta;
    -- Retornar la fila insertada
    SELECT d.Id, d.Fecha, d.IdVenta, d.IdProducto, p.Nombre AS NombreProducto, d.Cantidad, d.Precio, d.Iva, d.Total
    FROM DetalleVenta d
    INNER JOIN Producto p ON p.Id = d.IdProducto
    WHERE d.Id = SCOPE_IDENTITY();
END
GO
---------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE SP_EliminarDetalleVenta
    @Id INT
	
AS
BEGIN
	DECLARE @Total FLOAT, @IdVenta INT;
	SELECT @IdVenta = (SELECT TOP 1 dt.IdVenta FROM DetalleVenta dt WHERE Id = @Id);
	SELECT @Total = (Select Total FROM DetalleVenta WHERE Id = @Id);
    

	UPDATE Venta
    SET Total = Total - @Total
    WHERE Id = @IdVenta;

	DELETE FROM DetalleVenta
    WHERE Id = @Id;
END;
GO
---------------------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE SP_ListarDetallesPorVenta
    @IdVenta INT
AS
BEGIN
    SELECT 
        D.Id,
        D.Fecha,
        D.IdVenta,
        D.IdProducto AS IdProducto,
        P.Nombre AS NombreProducto,
        D.Cantidad,
        D.Precio,
        D.Iva,
        D.Total
    FROM DetalleVenta D
    INNER JOIN Producto P ON D.IdProducto = P.Id
    WHERE D.IdVenta = @IdVenta;
END;
go