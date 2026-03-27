using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
                                query = @"select  r.reservacion_id,
                                                                    r.estatus,
                                                                    r.fecha_entrada,
                                                                    r.fecha_salida,
                                                                    ISNULL(NULLIF(LTRIM(RTRIM(CONCAT(h.nombre, ' ', h.apellido_1, ' ', h.apellido_2))), ''), CONVERT(varchar(20), r.huesped_id)) as nombre_huesped,
                                                                    r.numero_personas
                                                     from reservaciones r
                                                     left join huespedes h on h.huesped_id = r.huesped_id
                                                     where r.reservacion_id = @reservacion_id";
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

        /*public Alumno ObtenerPorMatricula(int matricula)
        {
            try
            {
                DataTable registros = new DataTable();
                Alumno alumno = new Alumno();

                // crear conexion
                SqlConnection conexionBD = ConexionBD.obtenerConexion();
                //SqlConnection conexionBD = new SqlConnection(cadenaConexionBD);

                // abrir conexion
                conexionBD.Open();

                // llenar una DataTable
                query = @"select * from alumnos where matricula = @matricula";
                SqlCommand comando = new SqlCommand(query, conexionBD);
                comando.Parameters.AddWithValue("@matricula", matricula);

                SqlDataAdapter adaptadorDatos = new SqlDataAdapter(comando);
                adaptadorDatos.Fill(registros);


                // revisar si trajo el registro
                if (registros.Rows.Count > 0)
                {
                    alumno.alumno_id = int.Parse(registros.Rows[0]["alumno_id"].ToString());
                    alumno.matricula = int.Parse(registros.Rows[0]["matricula"].ToString());
                    alumno.nombre = registros.Rows[0]["nombre"].ToString();
                    alumno.apellido1 = registros.Rows[0]["apellido1"].ToString();
                    alumno.apellido2 = registros.Rows[0]["apellido2"].ToString();
                    alumno.fecha_nacimiento = DateTime.Parse(registros.Rows[0]["fecha_nacimiento"].ToString());
                    alumno.sexo_id = int.Parse(registros.Rows[0]["sexo_id"].ToString());

                }

                // cerrar conexion
                conexionBD.Close();

                // devolver el DataTable
                return alumno;

            }
            catch (SqlException)
            {
                throw;
            }
        }*/


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
                                ISNULL(NULLIF(LTRIM(RTRIM(CONCAT(h.nombre, ' ', h.apellido_1, ' ', h.apellido_2))), ''), CONVERT(varchar(20), r.huesped_id)) as nombre_huesped,
                                r.numero_personas,
                                r.huesped_id
                            from reservaciones r
                            left join huespedes h on h.huesped_id = r.huesped_id";

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

                int huespedID = ResolverHuespedId(conexionBD, reservacion.nombre_huesped);
                if (huespedID <= 0)
                {
                    throw new Exception("No se encontró un huésped válido. Captura el ID de huésped o un nombre completo existente.");
                }

                // agregar el registro
                query = @"INSERT INTO reservaciones (huesped_id, estatus, fecha_entrada, fecha_salida, numero_personas)
                          VALUES (@huesped_id, @estatus, @fecha_entrada, @fecha_salida, @numero_personas)";

                SqlCommand comando = new SqlCommand(query, conexionBD);
                comando.Parameters.AddWithValue("@huesped_id", huespedID);
                comando.Parameters.AddWithValue("@estatus", reservacion.estatus);
                comando.Parameters.AddWithValue("@fecha_entrada", reservacion.fecha_entrada);
                comando.Parameters.AddWithValue("@fecha_salida", reservacion.fecha_salida);
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

                int huespedID = ResolverHuespedId(conexionBD, reservacion.nombre_huesped);
                if (huespedID <= 0)
                {
                    throw new Exception("No se encontró un huésped válido. Captura el ID de huésped o un nombre completo existente.");
                }

                // agregar el registro
                query = @"UPDATE reservaciones
                            SET huesped_id = @huesped_id,
                                estatus = @estatus,
                                fecha_entrada = @fecha_entrada,
                                fecha_salida = @fecha_salida,
                                numero_personas = @numero_personas
                            WHERE reservacion_id = @reservacion_id";

                // crear el comando
                SqlCommand comando = new SqlCommand(query, conexionBD);
                comando.Parameters.AddWithValue("@reservacion_id", reservacion.reservacion_id);
                comando.Parameters.AddWithValue("@huesped_id", huespedID);
                comando.Parameters.AddWithValue("@estatus", reservacion.estatus);
                comando.Parameters.AddWithValue("@fecha_entrada", reservacion.fecha_entrada);
                comando.Parameters.AddWithValue("@fecha_salida", reservacion.fecha_salida);
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

        private static int ResolverHuespedId(SqlConnection conexionBD, string valorHuesped)
        {
            if (string.IsNullOrWhiteSpace(valorHuesped))
            {
                return 0;
            }

            string texto = valorHuesped.Trim();
            if (int.TryParse(texto, out int huespedID) && huespedID > 0)
            {
                return huespedID;
            }

            using (SqlCommand cmd = new SqlCommand(@"SELECT TOP 1 h.huesped_id
                                                   FROM huespedes h
                                                   WHERE LOWER(LTRIM(RTRIM(CONCAT(h.nombre, ' ', h.apellido_1, ' ', h.apellido_2)))) = @nombreCompleto
                                                      OR LOWER(LTRIM(RTRIM(h.nombre))) = @nombreSimple", conexionBD))
            {
                cmd.Parameters.AddWithValue("@nombreCompleto", texto.ToLower());
                cmd.Parameters.AddWithValue("@nombreSimple", texto.ToLower());

                object encontrado = cmd.ExecuteScalar();
                if (encontrado == null)
                {
                    return 0;
                }

                return Convert.ToInt32(encontrado);
            }
        }
    }
}
