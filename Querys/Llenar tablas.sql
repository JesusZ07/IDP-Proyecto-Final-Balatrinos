USE [Hotel]
GO

/*
  Script de carga inicial
  - 10 huespedes
  - 40 habitaciones
	- 5 reservaciones
*/

/* Limpieza previa para evitar conflictos de PK/FK */
DELETE FROM [dbo].[reservaciones];
DELETE FROM [dbo].[huespedes];
DELETE FROM [dbo].[habitaciones];
GO

/* Reiniciar identity */
DBCC CHECKIDENT ('[dbo].[huespedes]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[reservaciones]', RESEED, 0);
GO

/* 10 Huespedes */
INSERT INTO [dbo].[huespedes]
([nombre], [apellido_1], [apellido_2], [calle], [colonia], [codigo_postal], [ciudad], [correo], [numero_celular], [contrasena])
VALUES
('Jesus Abraham', 'Zapata', 'Estrada', 'Av Reforma 120', 'Centro', 21000, 'Mexicali', 'jesus_abraham03@hotmail.com', '6861404265', 'pass123'),
('Carlos', 'Lopez', 'Martinez', 'Calle Hidalgo 45', 'Nueva', 21020, 'Mexicali', 'carlos.lopez@correo.com', '6861111111', 'pass123'),
('Ana', 'Garcia', 'Ruiz', 'Av Madero 78', 'Industrial', 21030, 'Mexicali', 'ana.garcia@correo.com', '6862222222', 'pass123'),
('Luis', 'Hernandez', 'Soto', 'Privada del Sol 9', 'Esperanza', 21040, 'Mexicali', 'luis.hernandez@correo.com', '6863333333', 'pass123'),
('Maria', 'Torres', 'Diaz', 'Av Universidad 321', 'Pueblo Nuevo', 21050, 'Mexicali', 'maria.torres@correo.com', '6864444444', 'pass123'),
('Jorge', 'Ramirez', 'Vega', 'Calle Morelos 78', 'Bellavista', 21060, 'Mexicali', 'jorge.ramirez@correo.com', '6865555555', 'pass123'),
('Daniela', 'Mendoza', 'Gil', 'Calle Rio Presidio 12', 'Orizaba', 21070, 'Mexicali', 'daniela.mendoza@correo.com', '6866666666', 'pass123'),
('Ricardo', 'Navarro', 'Cruz', 'Blvd Lazaro 100', 'Ex Ejido', 21080, 'Mexicali', 'ricardo.navarro@correo.com', '6867777777', 'pass123'),
('Fernanda', 'Castillo', 'Luna', 'Calle Cuarta 55', 'Centro Civico', 21090, 'Mexicali', 'fernanda.castillo@correo.com', '6868888888', 'pass123'),
('Sofia', 'Ortega', 'Pineda', 'Av Colon 250', 'Prohogar', 21100, 'Mexicali', 'sofia.ortega@correo.com', '6869999999', 'pass123');
GO

/* 40 Habitaciones (101-110, 201-210, 301-310, 401-410) */
;WITH N AS (
	SELECT TOP (40) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
	FROM sys.all_objects
)
INSERT INTO [dbo].[habitaciones] ([tipo_habitacion], [piso], [estatus], [numero_habitacion])
SELECT
	CASE ((n - 1) % 5)
		WHEN 0 THEN 'Sencilla'
		WHEN 1 THEN 'Doble'
		WHEN 2 THEN 'Suite'
		WHEN 3 THEN 'Familiar'
		ELSE 'Ejecutiva'
	END AS tipo_habitacion,
	((n - 1) / 10) + 1 AS piso,
	CASE
		WHEN n % 7 = 0 THEN 'Mantenimiento'
		WHEN n % 3 = 0 THEN 'Ocupado'
		ELSE 'Disponible'
	END AS estatus,
	((((n - 1) / 10) + 1) * 100) + (((n - 1) % 10) + 1) AS numero_habitacion
FROM N
ORDER BY n;
GO

/* 5 Reservaciones usando huespedes existentes */
;WITH H AS (
	SELECT TOP (5)
		huesped_id,
		ROW_NUMBER() OVER (ORDER BY huesped_id) AS n
	FROM [dbo].[huespedes]
	ORDER BY huesped_id
)
INSERT INTO [dbo].[reservaciones] ([estatus], [fecha_entrada], [fecha_salida], [huesped_id], [numero_personas])
SELECT
	CASE
		WHEN n % 5 = 0 THEN 'Cancelada'
		WHEN n % 4 = 0 THEN 'Pendiente'
		ELSE 'Confirmada'
	END AS estatus,
	DATEADD(DAY, n, CAST('2026-05-01' AS datetime)) AS fecha_entrada,
	DATEADD(DAY, n + ((n % 4) + 1), CAST('2026-05-01' AS datetime)) AS fecha_salida,
	huesped_id,
	((n - 1) % 4) + 1 AS numero_personas
FROM H
ORDER BY n;
GO

/* Verificacion rapida de cantidades */
SELECT COUNT(*) AS total_huespedes FROM [dbo].[huespedes];
SELECT COUNT(*) AS total_habitaciones FROM [dbo].[habitaciones];
SELECT COUNT(*) AS total_reservaciones FROM [dbo].[reservaciones];
GO
