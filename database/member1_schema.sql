-- Optional reference schema for Member 1.
-- Preferred setup: use EF Core migrations from the README.

IF DB_ID(N'LogiFlowDb') IS NULL
BEGIN
    CREATE DATABASE LogiFlowDb;
END
GO
USE LogiFlowDb;
GO

IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
BEGIN
CREATE TABLE dbo.Roles (
    Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Roles PRIMARY KEY,
    Name nvarchar(100) NOT NULL CONSTRAINT UQ_Roles_Name UNIQUE
);
END
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Roles)
BEGIN
    SET IDENTITY_INSERT dbo.Roles ON;
    INSERT dbo.Roles(Id,Name) VALUES (1,N'Admin'),(2,N'Logistics Staff'),(3,N'Driver'),(4,N'Customer');
    SET IDENTITY_INSERT dbo.Roles OFF;
END
GO

-- EF Core migrations remain the source of truth for the complete schema.
