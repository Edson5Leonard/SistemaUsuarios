CREATE DATABASE SistemaUsuarios;
GO

USE SistemaUsuarios;
GO

CREATE TABLE Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre CHAR(100) NOT NULL, 
    Email CHAR(150) UNIQUE NOT NULL, 
    PasswordHash CHAR(255) NOT NULL,
    ResetToken CHAR(255) NULL, 
    TokenExpiracion DATETIME NULL,
    FechaRegistro DATETIME DEFAULT GETDATE()
);
GO 

CREATE PROCEDURE GuardarTokenRecuperacion
    @Email CHAR(150),
    @Token CHAR(255),
    @Expiracion DATETIME
AS
BEGIN
    SET NOCOUNT ON; 
    UPDATE Usuarios
    SET ResetToken = @Token,
        TokenExpiracion = @Expiracion
    WHERE Email = @Email;
END
GO  