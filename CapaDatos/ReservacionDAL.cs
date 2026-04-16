using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;

namespace CapaDatos
{
    public class ReservacionDAL
    {
        private string cadenaConexionBD = "Data Source=.;Initial Catalog=Hotel;Integrated Security=True;TrustServerCertificate=True";
        private string query = "";

        public Reservacion Obtener(int reservacionID)
        {
            try
            {
                DataTable registros = new DataTable();
                Reservacion reservacion = new Reservacion();

                // crear conexion
                SqlConnection conexionBD = ConexionBD.obtenerConexion();
                //SqlConnection conexionBD = new SqlConnection(cadenaConexionBD);

                // abrir conexion
                conexionBD.Open();

                // llenar una DataTable
                    query = @"SELECT 
                              r.reservacion_id,
                              r.estatus,
                              r.fecha_entrada,
                              r.fecha_salida,
                              r.numero_personas,
                              LTRIM(RTRIM(CONCAT(h.nombre, ' ', h.apellido_1, ' ', ISNULL(h.apellido_2, '')))) AS nombre_huesped
                          FROM reservaciones r
                          INNER JOIN huespedes h ON h.huesped_id = r.huesped_id
                          WHERE r.reservacion_id = @reservacion_id";
                SqlCommand comando = new SqlCommand(query, conexionBD);
                comando.Parameters.AddWithValue("@reservacion_id", reservacionID);

                SqlDataAdapter adaptadorDatos = new SqlDataAdapter(comando);
                adaptadorDatos.Fill(registros);


                // revisar si trajo el registro
                if (registros.Rows.Count > 0)
                {
                    reservacion.reservacion_id = int.Parse(registros.Rows[0]["reservacion_id"].ToString());
                    reservacion.estatus = registros.Rows[0]["estatus"].ToString();
                    reservacion.fecha_entrada = DateTime.Parse(registros.Rows[0]["fecha_entrada"].ToString());
                    reservacion.fecha_salida = DateTime.Parse(registros.Rows[0]["fecha_salida"].ToString());
                    reservacion.nombre_huesped = registros.Rows[0]["nombre_huesped"].ToString();
                    reservacion.numero_personas = int.Parse(registros.Rows[0]["numero_personas"].ToString());
                }

                // cerrar conexion
                conexionBD.Close();

                // devolver el DataTable
                return reservacion;

            }
            catch (SqlException)
            {
                throw;
            }
        }

        // Método para obtener todas las reservaciones
        public DataTable ObtenerTodos()
        {
            try
            {
                DataTable registros = new DataTable();

                // crear conexion
                SqlConnection conexionBD = new SqlConnection(cadenaConexionBD);

                // abrir conexion
                conexionBD.Open();

                // llenar una DataTable
                query = @"select 
                                r.reservacion_id,
                                r.estatus,
                                r.fecha_entrada,
                                r.fecha_salida,
                                LTRIM(RTRIM(CONCAT(h.nombre, ' ', h.apellido_1, ' ', ISNULL(h.apellido_2, '')))) AS nombre_huesped,
                                r.numero_personas
                            FROM reservaciones r
                            INNER JOIN huespedes h ON h.huesped_id = r.huesped_id";

                SqlDataAdapter adaptadorDatos = new SqlDataAdapter(query, conexionBD);
                adaptadorDatos.Fill(registros);

                // cerrar conexion
                conexionBD.Close();

                // devolver el DataTable
                return registros;

            }
            catch (SqlException)
            {
                throw;
            }
        }

        // Método para agregar una reservacion
        public bool Agregar(Reservacion reservacion)
        {
            try
            {
                bool comandoEjecutado = false;

                // crear conexion
                SqlConnection conexionBD = new SqlConnection(cadenaConexionBD);

                // abrir conexion
                conexionBD.Open();

                int huespedId = reservacion.huesped_id > 0
                    ? reservacion.huesped_id
                    : ObtenerHuespedIdPorNombre(conexionBD, reservacion.nombre_huesped);

                // agregar el registro
                query = @"INSERT INTO reservaciones (estatus, fecha_entrada, fecha_salida, huesped_id, numero_personas) 
                                      VALUES (@estatus, @fecha_entrada, @fecha_salida, @huesped_id, @numero_personas)";
                
                SqlCommand comando = new SqlCommand(query, conexionBD);
                comando.Parameters.AddWithValue("@estatus", reservacion.estatus);
                comando.Parameters.AddWithValue("@fecha_entrada", reservacion.fecha_entrada);
                comando.Parameters.AddWithValue("@fecha_salida", reservacion.fecha_salida);
                comando.Parameters.AddWithValue("@huesped_id", huespedId);
                comando.Parameters.AddWithValue("@numero_personas", reservacion.numero_personas);


                // ejecutar el comando 
                comandoEjecutado = comando.ExecuteNonQuery() > 0;
                //comando.ExecuteScalar();

                // cerrar conexion
                conexionBD.Close();

                return comandoEjecutado;

            }
            catch (SqlException)
            {
                throw;
            }
        }

        // Método para actualizar una reservacion
        public bool Actualizar(Reservacion reservacion)
        {
            try
            {
                bool comandoEjecutado = false;

                // crear conexion
                SqlConnection conexionBD = new SqlConnection(cadenaConexionBD);

                // abrir conexion
                conexionBD.Open();

                int huespedId = reservacion.huesped_id > 0
                    ? reservacion.huesped_id
                    : ObtenerHuespedIdPorNombre(conexionBD, reservacion.nombre_huesped);

                // agregar el registro
                query = @"UPDATE reservaciones
                                 SET estatus           = @estatus,
                                 fecha_entrada     = @fecha_entrada,
                                 fecha_salida      = @fecha_salida,
                                 huesped_id        = @huesped_id,
                                 numero_personas   = @numero_personas
                                 WHERE reservacion_id = @reservacion_id";

                // crear el comando
                SqlCommand comando = new SqlCommand(query, conexionBD);
                comando.Parameters.AddWithValue("@reservacion_id", reservacion.reservacion_id);
                comando.Parameters.AddWithValue("@estatus", reservacion.estatus);
                comando.Parameters.AddWithValue("@fecha_entrada", reservacion.fecha_entrada);
                comando.Parameters.AddWithValue("@fecha_salida", reservacion.fecha_salida);
                comando.Parameters.AddWithValue("@huesped_id", huespedId);
                comando.Parameters.AddWithValue("@numero_personas", reservacion.numero_personas);

                // ejecutar el comando 
                comandoEjecutado = comando.ExecuteNonQuery() > 0;

                // cerrar conexion
                conexionBD.Close();

                return comandoEjecutado;
            }
            catch (SqlException)
            {
                throw;
            }
        }

        // Método para eliminar un huesped
        public bool Eliminar(int reservacionID)
        {
            try
            {
                bool comandoEjecutado = false;

                // crear conexion
                SqlConnection conexionBD = new SqlConnection(cadenaConexionBD);

                // abrir conexion
                conexionBD.Open();

                // agregar el registro
                query = @"delete from reservaciones where reservacion_id = @reservacion_id";
                SqlCommand comando = new SqlCommand(query, conexionBD);
                comando.Parameters.AddWithValue("@reservacion_id", reservacionID);

                // ejecutar el comando 
                comandoEjecutado = comando.ExecuteNonQuery() > 0;

                // cerrar conexion
                conexionBD.Close();

                return comandoEjecutado;
            }
            catch (SqlException)
            {
                throw;
            }
        }

        private int ObtenerHuespedIdPorNombre(SqlConnection conexionBD, string nombreHuesped)
        {
            query = @"SELECT TOP (1) huesped_id
                      FROM huespedes
                      WHERE LTRIM(RTRIM(CONCAT(nombre, ' ', apellido_1, ' ', ISNULL(apellido_2, '')))) = @nombre_huesped
                         OR nombre = @nombre_huesped
                      ORDER BY huesped_id";

            SqlCommand comando = new SqlCommand(query, conexionBD);
            comando.Parameters.AddWithValue("@nombre_huesped", nombreHuesped?.Trim() ?? string.Empty);

            object resultado = comando.ExecuteScalar();
            if (resultado == null)
            {
                throw new InvalidOperationException("No se encontró un huésped con el nombre proporcionado.");
            }

            return Convert.ToInt32(resultado);
        }
    }
}
