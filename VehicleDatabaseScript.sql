USE [master]
GO

/****** Object:  Database [vehicle]    Script Date: 26/9/2023 01:52:53 ******/
CREATE DATABASE [vehicle] 
	ON ( NAME = N'vehicle', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\vehicle.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
LOG ON ( NAME = N'vehicle_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\vehicle_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
	
GO


-- change owner to sa
ALTER AUTHORIZATION ON DATABASE::[vehicle] TO [sa]
GO
 
-- set recovery model to simple 
ALTER DATABASE [vehicle] SET RECOVERY SIMPLE 
GO
 
-- change compatibility level
ALTER DATABASE [vehicle] SET COMPATIBILITY_LEVEL = 130
GO

USE [vehicle]
GO

CREATE TABLE Category
(
	Id INT PRIMARY KEY IDENTITY(1,1),
	Name NVARCHAR(255) NOT NULL,
	IconUrl NVARCHAR(255) NOT NULL,
	MinWeightKg DECIMAL(18,2) NOT NULL,
	MaxWeightKg DECIMAL(18,2) NOT NULL
);

CREATE TABLE Vehicle
(
	Id INT PRIMARY KEY IDENTITY(1,1),
	OwnerName NVARCHAR(255) NOT NULL,
	Manufacturer NVARCHAR(255) NOT NULL,
	YearOfManufacture INT NOT NULL,
	WeightKg DECIMAL(18,2) NOT NULL,
	CategoryId INT NOT NULL,
	CONSTRAINT FK_Vehicle_Category FOREIGN KEY (CategoryId) REFERENCES Category(Id)
);


INSERT INTO [dbo].[Category]([Name],[IconUrl],[MinWeightKg],[MaxWeightKg])
                     VALUES ('Light', 'directions_car', 0.00, 500.00),
							('Medium', 'local_shipping', 500.01, 2500.00),
							('Light', 'directions_bus', 2500.01, 9999999999999999.00)
GO

INSERT INTO [dbo].[Vehicle] ([OwnerName],[Manufacturer],[YearOfManufacture],[WeightKg],[CategoryId])
     VALUES('Test1', 'Toyota', 2015, 850.00, 2)
GO


 
