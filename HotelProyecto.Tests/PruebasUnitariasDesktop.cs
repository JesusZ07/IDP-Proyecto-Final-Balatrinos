using System;
using System.Data;
// removed unused using: System.Linq
using Microsoft.Data.SqlClient;
using Xunit;
using CapaDatos;
using CapaNegocios;
using CapaEntidad;

namespace HotelProyecto.Tests
{
    public class PruebasUnitariasDesktop
    {
        private static int createdHuespedId = 0;
        private static string createdHuespedCorreo = null;
        private static string createdHuespedNombreCompleto = null;
        private static int createdHabitacionNumero = 0;
        private static int createdReservacionId = 0;

        private void EnsureHuespedExists()
        {
            if (createdHuespedId > 0) return;
            var huespedBLL = new HuespedBLL();
            var huesped = new Huesped
            {
                nombre = "Manuel Antonio",
                apellido_1 = "Ramirez",
                apellido_2 = "Estrada",
                calle = "Churrubusco",
                colonia = "Condesa",
                codigo_postal = 21467,
                ciudad = "Tangamandapio",
                correo = $"manuel.antonio{Guid.NewGuid().ToString().Substring(0,8)}@example.com",
                numero_celular = "6861404265",
                contrasena = "password"
            };

            bool creado = huespedBLL.Agregar(huesped);
            Assert.True(creado, "No se pudo crear el huésped de prueba en EnsureHuespedExists.");

            var tabla = huespedBLL.ObtenerTodos();
            foreach (System.Data.DataRow row in tabla.Rows)
            {
                if (row["numero_celular"].ToString() == "6861404265" && row["nombre"].ToString() == "Manuel Antonio")
                {
                    createdHuespedId = Convert.ToInt32(row["huesped_id"]);
                    createdHuespedCorreo = row["correo"].ToString();
                    createdHuespedNombreCompleto = string.Concat(row["nombre"].ToString().Trim(), " ", row["apellido_1"].ToString().Trim(), " ", row["apellido_2"].ToString().Trim()).Trim();
                    break;
                }
            }
            Assert.True(createdHuespedId > 0, "EnsureHuespedExists no capturó el ID del huésped creado.");
        }

        private void EnsureReservacionExists()
        {
            if (createdReservacionId > 0) return;
            EnsureHuespedExists();

            var reservacionBLL = new ReservacionBLL();
            var reservacion = new CapaEntidad.Reservacion
            {
                estatus = "Confirmada",
                fecha_entrada = DateTime.Today.AddDays(7),
                fecha_salida = DateTime.Today.AddDays(10),
                nombre_huesped = createdHuespedNombreCompleto,
                numero_personas = 2
            };

            bool creado = reservacionBLL.Agregar(reservacion);
            Assert.True(creado, "No se pudo crear la reservación de prueba en EnsureReservacionExists.");

            var tabla = reservacionBLL.ObtenerTodos();
            foreach (System.Data.DataRow row in tabla.Rows)
            {
                if (row["nombre_huesped"].ToString().Trim() == createdHuespedNombreCompleto &&
                    Convert.ToDateTime(row["fecha_entrada"]).Date == DateTime.Today.AddDays(7) &&
                    Convert.ToInt32(row["numero_personas"]) == 2)
                {
                    createdReservacionId = Convert.ToInt32(row["reservacion_id"]);
                    break;
                }
            }
            Assert.True(createdReservacionId > 0, "EnsureReservacionExists no capturó el ID de la reservación creada.");
        }

        private void EnsureHabitacionExists()
        {
            if (createdHabitacionNumero > 0) return;
            var habitacionBLL = new HabitacionBLL();
            var rnd = new Random();
            int numero = rnd.Next(1000, 9999);
            var habitacion = new CapaEntidad.Habitacion
            {
                numero_habitacion = numero,
                tipo_habitacion = "Doble",
                piso = 2,
                estatus = "Disponible"
            };

            bool creado = habitacionBLL.Agregar(habitacion);
            Assert.True(creado, "No se pudo crear la habitación de prueba en EnsureHabitacionExists.");
            createdHabitacionNumero = numero;
        }
        [Fact, Trait("Category", "Database")]
        public void ObtenerConexion()
        {
            using SqlConnection conexion = ConexionBD.obtenerConexion();

            conexion.Open();
            Assert.Equal(ConnectionState.Open, conexion.State);

            conexion.Close();
            Assert.Equal(ConnectionState.Closed, conexion.State);
        }

        [Fact, Trait("Category", "Database")]
        public void Huesped_Create()
        {
            string correo = $"manuel.antonio{Guid.NewGuid().ToString().Substring(0,8)}@example.com";
            var huespedBLL = new HuespedBLL();
            var huesped = new Huesped
            {
                nombre = "Manuel Antonio",
                apellido_1 = "Ramirez",
                apellido_2 = "Estrada",
                calle = "Churrubusco",
                colonia = "Condesa",
                codigo_postal = 21467,
                ciudad = "Tangamandapio",
                correo = correo,
                numero_celular = "6861404265",
                contrasena = "password"
            };

            bool creado = huespedBLL.Agregar(huesped);
            Assert.True(creado, "La creación del huésped debería retornar true.");

            // Capturar ID del huésped creado para uso en otras pruebas
            var tabla = huespedBLL.ObtenerTodos();
            foreach (System.Data.DataRow row in tabla.Rows)
            {
                if (row["numero_celular"].ToString() == "6861404265" && row["nombre"].ToString() == "Manuel Antonio")
                {
                    createdHuespedId = Convert.ToInt32(row["huesped_id"]);
                    createdHuespedCorreo = row["correo"].ToString();
                    createdHuespedNombreCompleto = string.Concat(row["nombre"].ToString().Trim(), " ", row["apellido_1"].ToString().Trim(), " ", row["apellido_2"].ToString().Trim()).Trim();
                    break;
                }
            }
            Assert.True(createdHuespedId > 0, "No se pudo capturar el ID del huésped creado.");
        }

        [Fact, Trait("Category", "Database")]
        public void Huesped_Read()
        {
            var huespedBLL = new HuespedBLL();

            var tabla = huespedBLL.ObtenerTodos();
            Assert.NotNull(tabla);

            Huesped encontrado = null;
            foreach (System.Data.DataRow row in tabla.Rows)
            {
                if (row["nombre"].ToString() == "Manuel Antonio" &&
                    row["apellido_1"].ToString() == "Ramirez" &&
                    row["apellido_2"].ToString() == "Estrada" &&
                    row["numero_celular"].ToString() == "6861404265")
                {
                    encontrado = new Huesped
                    {
                        huesped_id = Convert.ToInt32(row["huesped_id"]),
                        nombre = row["nombre"].ToString(),
                        apellido_1 = row["apellido_1"].ToString()
                    };
                    break;
                }
            }

            Assert.NotNull(encontrado);
            Assert.Equal("Manuel Antonio", encontrado.nombre);
        }

        [Fact, Trait("Category", "Database")]
        public void Huesped_Update()
        {
            // Use the captured guest to perform update so deletion targets the latest version
            EnsureHuespedExists();
            var huespedBLL = new HuespedBLL();
            var existente = huespedBLL.Obtener(createdHuespedId);
            Assert.NotNull(existente);

            existente.ciudad = "Ensenada";
            existente.contrasena = "password123456";

            bool actualizado = huespedBLL.Actualizar(existente);
            Assert.True(actualizado, "La actualización debería retornar true.");

            var obtenido = huespedBLL.Obtener(createdHuespedId);
            Assert.NotNull(obtenido);
            Assert.Equal("Ensenada", obtenido.ciudad);
        }

        [Fact, Trait("Category", "Database")]
        public void Habitaciones_Create()
        {
            var habitacionBLL = new HabitacionBLL();
            var rnd = new Random();
            int numero = rnd.Next(1000, 9999);
            var habitacion = new CapaEntidad.Habitacion
            {
                numero_habitacion = numero,
                tipo_habitacion = "Doble",
                piso = 2,
                estatus = "Disponible"
            };

            bool creado = habitacionBLL.Agregar(habitacion);
            Assert.True(creado, "La creación de la habitación debería retornar true.");

            // Capturar número de habitación creada
            createdHabitacionNumero = numero;
        }

        [Fact, Trait("Category", "Database")]
        public void Habitaciones_Read()
        {
            var habitacionBLL = new HabitacionBLL();
            var tabla = habitacionBLL.ObtenerTodos();
            Assert.NotNull(tabla);

            int numeroEncontrado = 0;
            foreach (System.Data.DataRow row in tabla.Rows)
            {
                if (row["tipo_habitacion"].ToString() == "Doble" &&
                    Convert.ToInt32(row["piso"]) == 2 &&
                    row["estatus"].ToString() == "Disponible")
                {
                    numeroEncontrado = Convert.ToInt32(row["numero_habitacion"]);
                    break;
                }
            }

            Assert.True(numeroEncontrado > 0, "No se encontró la habitación creada anteriormente.");

            var habitacion = habitacionBLL.Obtener(numeroEncontrado);
            Assert.NotNull(habitacion);
            Assert.Equal("Doble", habitacion.tipo_habitacion);
        }

        [Fact, Trait("Category", "Database")]
        public void Habitaciones_Update()
        {
            // Update the room created earlier to keep tests consistent
            EnsureHabitacionExists();
            var habitacionBLL = new HabitacionBLL();
            var existente = habitacionBLL.Obtener(createdHabitacionNumero);
            Assert.NotNull(existente);

            existente.piso = 4;
            existente.estatus = "Ocupada";

            bool actualizado = habitacionBLL.Actualizar(existente);
            Assert.True(actualizado, "La actualización debería retornar true.");

            var obtenido = habitacionBLL.Obtener(createdHabitacionNumero);
            Assert.NotNull(obtenido);
            Assert.Equal(4, obtenido.piso);
            Assert.Equal("Ocupada", obtenido.estatus);
        }

        [Fact, Trait("Category", "Database")]
        public void Reservaciones_Create()
        {
            EnsureHuespedExists();
            var reservacionBLL = new ReservacionBLL();

            var reservacion = new CapaEntidad.Reservacion
            {
                estatus = "Confirmada",
                fecha_entrada = DateTime.Today.AddDays(7),
                fecha_salida = DateTime.Today.AddDays(10),
                nombre_huesped = createdHuespedNombreCompleto,
                numero_personas = 2
            };

            bool creado = reservacionBLL.Agregar(reservacion);
            Assert.True(creado, "La creación de la reservación debería retornar true.");

            // Capturar ID de la reservación creada
            var tabla = reservacionBLL.ObtenerTodos();
            foreach (System.Data.DataRow row in tabla.Rows)
            {
                if (createdHuespedNombreCompleto != null && row["nombre_huesped"].ToString().Trim() == createdHuespedNombreCompleto &&
                    Convert.ToDateTime(row["fecha_entrada"]).Date == DateTime.Today.AddDays(7) &&
                    Convert.ToInt32(row["numero_personas"]) == 2)
                {
                    createdReservacionId = Convert.ToInt32(row["reservacion_id"]);
                    break;
                }
            }
            Assert.True(createdReservacionId > 0, "No se pudo capturar el ID de la reservación creada.");
        }

        [Fact, Trait("Category", "Database")]
        public void Reservaciones_Read()
        {
            EnsureReservacionExists();
            var reservacionBLL = new ReservacionBLL();
            // Prefer using the captured ID for deterministic lookup
            int idEncontrado = createdReservacionId;
            if (idEncontrado == 0)
            {
                var tabla = reservacionBLL.ObtenerTodos();
                Assert.NotNull(tabla);
                foreach (System.Data.DataRow row in tabla.Rows)
                {
                    if (createdHuespedNombreCompleto != null && row["nombre_huesped"].ToString().Trim() == createdHuespedNombreCompleto &&
                        row["estatus"].ToString() == "Confirmada" &&
                        row["numero_personas"] != DBNull.Value &&
                        Convert.ToInt32(row["numero_personas"]) == 2)
                    {
                        idEncontrado = Convert.ToInt32(row["reservacion_id"]);
                        break;
                    }
                }
            }

            Assert.True(idEncontrado > 0, "No se encontró la reservación creada anteriormente.");

            var reserv = reservacionBLL.Obtener(idEncontrado);
            Assert.NotNull(reserv);
            Assert.Equal(createdHuespedNombreCompleto, reserv.nombre_huesped);
        }

        [Fact, Trait("Category", "Database")]
        public void Reservaciones_Update()
        {
            var reservacionBLL = new ReservacionBLL();
            // Update the reservation created earlier to ensure delete removes last version
            EnsureReservacionExists();
            int idEncontrado = createdReservacionId;
            Assert.True(idEncontrado > 0, "No existe reservación capturada para actualizar.");

            var existente = reservacionBLL.Obtener(idEncontrado);
            Assert.NotNull(existente);

            existente.numero_personas = 5;

            bool actualizado = reservacionBLL.Actualizar(existente);
            Assert.True(actualizado, "La actualización debería retornar true.");

            var obtenido = reservacionBLL.Obtener(idEncontrado);
            Assert.NotNull(obtenido);
            Assert.Equal(5, obtenido.numero_personas);
        }

        // DELETE tests: moved to the end to avoid removing data needed by other tests
        [Fact, Trait("Category", "Database")]
        public void Reservaciones_Delete()
        {
            EnsureReservacionExists();
            var reservacionBLL = new ReservacionBLL();
            int idAEliminar = createdReservacionId;
            if (idAEliminar == 0)
            {
                var tabla = reservacionBLL.ObtenerTodos();
                Assert.NotNull(tabla);
                foreach (System.Data.DataRow row in tabla.Rows)
                {
                    if (createdHuespedNombreCompleto != null && row["nombre_huesped"].ToString().Trim() == createdHuespedNombreCompleto &&
                        row["estatus"].ToString() == "Confirmada")
                    {
                        int id = Convert.ToInt32(row["reservacion_id"]);
                        if (id > idAEliminar) idAEliminar = id;
                    }
                }
            }

            Assert.True(idAEliminar > 0, "No se encontró una reservación de prueba para eliminar.");

            bool eliminado = reservacionBLL.Eliminar(idAEliminar);
            Assert.True(eliminado, "La eliminación debería retornar true.");

            var obtenido = reservacionBLL.Obtener(idAEliminar);
            Assert.NotNull(obtenido);
            Assert.Equal(0, obtenido.reservacion_id);

            // marcar que ya fue eliminada
            if (createdReservacionId == idAEliminar) createdReservacionId = 0;
        }

        [Fact, Trait("Category", "Database")]
        public void Habitaciones_Delete()
        {
            var habitacionBLL = new HabitacionBLL();
            int numeroAEliminar = createdHabitacionNumero;
            if (numeroAEliminar == 0)
            {
                var tabla = habitacionBLL.ObtenerTodos();
                Assert.NotNull(tabla);
                foreach (System.Data.DataRow row in tabla.Rows)
                {
                    if (row["tipo_habitacion"].ToString() == "Doble" &&
                        Convert.ToInt32(row["piso"]) == 4 &&
                        row["estatus"].ToString() == "Ocupada")
                    {
                        numeroAEliminar = Convert.ToInt32(row["numero_habitacion"]);
                        break;
                    }
                }
            }

            Assert.True(numeroAEliminar > 0, "No se encontró una habitación de prueba para eliminar.");

            bool eliminado = habitacionBLL.Eliminar(numeroAEliminar);
            Assert.True(eliminado, "La eliminación debería retornar true.");

            var obtenido = habitacionBLL.Obtener(numeroAEliminar);
            Assert.NotNull(obtenido);
            Assert.Equal(0, obtenido.numero_habitacion);

            if (createdHabitacionNumero == numeroAEliminar) createdHabitacionNumero = 0;
        }

        [Fact, Trait("Category", "Database")]
        public void Huesped_Delete()
        {
            var huespedBLL = new HuespedBLL();
            int idAEliminar = createdHuespedId;
            string correo = createdHuespedCorreo;
            string nombreCompleto = createdHuespedNombreCompleto;

            if (idAEliminar == 0)
            {
                var tabla = huespedBLL.ObtenerTodos();
                Assert.NotNull(tabla);
                foreach (System.Data.DataRow row in tabla.Rows)
                {
                    if (row["nombre"].ToString() == "Manuel Antonio" &&
                        row["apellido_1"].ToString() == "Ramirez" &&
                        row["apellido_2"].ToString() == "Estrada" &&
                        row["numero_celular"].ToString() == "6861404265")
                    {
                        idAEliminar = Convert.ToInt32(row["huesped_id"]);
                        correo = row["correo"].ToString();
                        nombreCompleto = string.Concat(row["nombre"].ToString().Trim(), " ", row["apellido_1"].ToString().Trim(), " ", row["apellido_2"].ToString().Trim()).Trim();
                        break;
                    }
                }
            }

            Assert.True(idAEliminar > 0, "No se encontró un huésped de prueba para eliminar.");

            // eliminar reservaciones asociadas primero (evitar FK)
            if (!string.IsNullOrEmpty(nombreCompleto))
            {
                var reservacionBLL = new ReservacionBLL();
                var tablaRes = reservacionBLL.ObtenerTodos();
                foreach (System.Data.DataRow r in tablaRes.Rows)
                {
                    // eliminar todas las reservaciones que apunten al huésped (por nombre completo)
                    if (r["nombre_huesped"].ToString().Trim() == nombreCompleto)
                    {
                        int idRes = Convert.ToInt32(r["reservacion_id"]);
                        reservacionBLL.Eliminar(idRes);
                        if (createdReservacionId == idRes) createdReservacionId = 0;
                    }
                }
            }

            bool eliminado = huespedBLL.Eliminar(idAEliminar);
            Assert.True(eliminado, "La eliminación debería retornar true.");

            var obtenido = huespedBLL.Obtener(idAEliminar);
            Assert.NotNull(obtenido);
            Assert.Equal(0, obtenido.huesped_id);

            if (createdHuespedId == idAEliminar) createdHuespedId = 0;
            if (createdHuespedCorreo == correo) createdHuespedCorreo = null;
            if (createdHuespedNombreCompleto == nombreCompleto) createdHuespedNombreCompleto = null;
        }
    }
}
