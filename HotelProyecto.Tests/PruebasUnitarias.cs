using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Xunit;
using CapaNegocios;
using CapaDatos;

namespace HotelProyecto.Tests
{
    public class PruebasUnitarias
    {
        private const string CadenaConexion = "Data Source=.;Initial Catalog=Hotel;Integrated Security=True;TrustServerCertificate=True";

        [Fact, Trait("Category", "Database")]
        public void CadenaConexion_ContieneCatalogoHotel()
        {
            Assert.Contains("Initial Catalog=Hotel", CadenaConexion, StringComparison.OrdinalIgnoreCase);
        }

        //Pruebas de las funciones y metodos de la CapaDatos
        [Fact, Trait("Category", "Database")]
        public void Conexion_BD()
        {
            using SqlConnection conexion = new SqlConnection(CadenaConexion);

            conexion.Open();

            Assert.Equal(System.Data.ConnectionState.Open, conexion.State);
        }

        [Fact, Trait("Category", "Database")]
        public void ObtenerHuespedes()
        {
            HuespedDAL huespedDAL = new HuespedDAL();

            DataTable huespedes = huespedDAL.ObtenerTodos();

            Assert.NotNull(huespedes);
            Assert.True(huespedes.Columns.Count > 0, "HuespedDAL.ObtenerTodos no devolvió columnas.");
        }

        [Fact, Trait("Category", "Database")]
        public void ObtenerHabitaciones()
        {
            HabitacionDAL habitacionDAL = new HabitacionDAL();

            DataTable habitaciones = habitacionDAL.ObtenerTodos();

            Assert.NotNull(habitaciones);
            Assert.True(habitaciones.Columns.Count > 0, "HabitacionDAL.ObtenerTodos no devolvió columnas.");
        }

        [Fact, Trait("Category", "Database")]
        public void ObtenerReservaciones()
        {
            ReservacionDAL reservacionDAL = new ReservacionDAL();

            DataTable reservaciones = reservacionDAL.ObtenerTodos();

            Assert.NotNull(reservaciones);
            Assert.True(reservaciones.Columns.Count > 0, "ReservacionDAL.ObtenerTodos no devolvió columnas.");
        }

        [Fact, Trait("Category", "Database")]
        public void ObtenerHuespedPorID()
        {
            HuespedDAL huespedDAL = new HuespedDAL();
            DataTable huespedes = huespedDAL.ObtenerTodos();

            Assert.NotNull(huespedes);

            if (huespedes.Rows.Count == 0)
            {
                return;
            }

            int huespedId = Convert.ToInt32(huespedes.Rows[0]["huesped_id"]);
            var huesped = huespedDAL.Obtener(huespedId);

            Assert.NotNull(huesped);
            Assert.Equal(huespedId, huesped.huesped_id);
        }

        [Fact, Trait("Category", "Database")]
        public void ObtenerHabitacionesPorNumero()
        {
            HabitacionDAL habitacionDAL = new HabitacionDAL();
            DataTable habitaciones = habitacionDAL.ObtenerTodos();

            Assert.NotNull(habitaciones);

            if (habitaciones.Rows.Count == 0)
            {
                return;
            }

            int numeroHabitacion = Convert.ToInt32(habitaciones.Rows[0]["numero_habitacion"]);
            var habitacion = habitacionDAL.Obtener(numeroHabitacion);

            Assert.NotNull(habitacion);
            Assert.Equal(numeroHabitacion, habitacion.numero_habitacion);
        }

        [Fact, Trait("Category", "Database")]
        public void ObtenerReservacionesPorID()
        {
            ReservacionDAL reservacionDAL = new ReservacionDAL();
            DataTable reservaciones = reservacionDAL.ObtenerTodos();

            Assert.NotNull(reservaciones);

            if (reservaciones.Rows.Count == 0)
            {
                return;
            }

            int reservacionId = Convert.ToInt32(reservaciones.Rows[0]["reservacion_id"]);
            var reservacion = reservacionDAL.Obtener(reservacionId);

            Assert.NotNull(reservacion);
            Assert.Equal(reservacionId, reservacion.reservacion_id);
        }

        //Pruebas de las funciones y metodos de la CapaNegocios
        [Fact, Trait("Category", "Database")]
        public void Cargar_TablasBD()
        {
            HuespedBLL huespedBLL = new HuespedBLL();
            HabitacionBLL habitacionBLL = new HabitacionBLL();
            ReservacionBLL reservacionBLL = new ReservacionBLL();

            DataTable huespedes = huespedBLL.ObtenerTodos();
            DataTable habitaciones = habitacionBLL.ObtenerTodos();
            DataTable reservaciones = reservacionBLL.ObtenerTodos();

            Assert.NotNull(huespedes);
            Assert.NotNull(habitaciones);
            Assert.NotNull(reservaciones);

            Assert.True(huespedes.Columns.Count > 0, "La tabla de huéspedes no cargó columnas.");
            Assert.True(habitaciones.Columns.Count > 0, "La tabla de habitaciones no cargó columnas.");
            Assert.True(reservaciones.Columns.Count > 0, "La tabla de reservaciones no cargó columnas.");
        }

        [Fact, Trait("Category", "Database")]
        public void VerificacionCorreo()
        {
            HuespedBLL huespedBLL = new HuespedBLL();
            DataTable huespedes = huespedBLL.ObtenerTodos();

            Assert.NotNull(huespedes);

            if (huespedes.Rows.Count == 0)
            {
                return;
            }

            int huespedId = Convert.ToInt32(huespedes.Rows[0]["huesped_id"]);
            string correo = Convert.ToString(huespedes.Rows[0]["correo"]) ?? string.Empty;

            var huesped = huespedBLL.Obtener(huespedId);
            bool correoExiste = huespedBLL.VerificarCorreoExistente(correo);

            Assert.NotNull(huesped);
            Assert.Equal(huespedId, huesped.huesped_id);
            Assert.True(correoExiste, "La validación de correo existente debe ser verdadera para un correo real.");
        }

        [Fact, Trait("Category", "Database")]
        public void ObtenerHabitacionPorNumero()
        {
            HabitacionBLL habitacionBLL = new HabitacionBLL();
            DataTable habitaciones = habitacionBLL.ObtenerTodos();

            Assert.NotNull(habitaciones);

            if (habitaciones.Rows.Count == 0)
            {
                return;
            }

            int numeroHabitacion = Convert.ToInt32(habitaciones.Rows[0]["numero_habitacion"]);
            var habitacion = habitacionBLL.Obtener(numeroHabitacion);

            Assert.NotNull(habitacion);
            Assert.Equal(numeroHabitacion, habitacion.numero_habitacion);
        }

        [Fact, Trait("Category", "Database")]
        public void ObtenerReservacionPorID()
        {
            ReservacionBLL reservacionBLL = new ReservacionBLL();
            DataTable reservaciones = reservacionBLL.ObtenerTodos();

            Assert.NotNull(reservaciones);

            if (reservaciones.Rows.Count == 0)
            {
                return;
            }

            int reservacionId = Convert.ToInt32(reservaciones.Rows[0]["reservacion_id"]);
            var reservacion = reservacionBLL.Obtener(reservacionId);

            Assert.NotNull(reservacion);
            Assert.Equal(reservacionId, reservacion.reservacion_id);
        }
    }
}