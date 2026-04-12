USE [Hotel]
GO

/****** Object:  Table [dbo].[habitaciones]    Script Date: 10/04/2026 10:58:28 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[habitaciones](
	[tipo_habitacion] [varchar](50) NOT NULL,
	[piso] [int] NOT NULL,
	[estatus] [varchar](50) NOT NULL,
	[numero_habitacion] [int] NOT NULL,
 CONSTRAINT [PK_habitaciones] PRIMARY KEY CLUSTERED 
(
	[numero_habitacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

USE [Hotel]
GO

/****** Object:  Table [dbo].[huespedes]    Script Date: 10/04/2026 10:59:29 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[huespedes](
	[nombre] [varchar](50) NOT NULL,
	[apellido_1] [varchar](50) NOT NULL,
	[apellido_2] [varchar](50) NULL,
	[calle] [varchar](50) NOT NULL,
	[colonia] [varchar](50) NOT NULL,
	[codigo_postal] [int] NOT NULL,
	[ciudad] [varchar](50) NOT NULL,
	[correo] [varchar](50) NOT NULL,
	[numero_celular] [varchar](50) NOT NULL,
	[contrasena] [varchar](50) NOT NULL,
	[huesped_id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_huespedes] PRIMARY KEY CLUSTERED 
(
	[huesped_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


USE [Hotel]
GO

/****** Object:  Table [dbo].[reservaciones]    Script Date: 10/04/2026 10:59:58 p. m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[reservaciones](
	[estatus] [varchar](50) NOT NULL,
	[fecha_entrada] [datetime] NOT NULL,
	[fecha_salida] [datetime] NOT NULL,
	[huesped_id] [int] NOT NULL,
	[reservacion_id] [int] IDENTITY(1,1) NOT NULL,
	[numero_personas] [int] NOT NULL,
 CONSTRAINT [PK_reservaciones] PRIMARY KEY CLUSTERED 
(
	[reservacion_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[reservaciones]  WITH CHECK ADD  CONSTRAINT [FK_reservaciones_huespedes] FOREIGN KEY([huesped_id])
REFERENCES [dbo].[huespedes] ([huesped_id])
GO

ALTER TABLE [dbo].[reservaciones] CHECK CONSTRAINT [FK_reservaciones_huespedes]
GO


