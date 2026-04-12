using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class HuespedDAL
    {
        private string cadenaConexionBD = "Data Source=.;Initial Catalog=Hotel;Integrated Security=True;TrustServerCertificate=True";

        private string query = "";

        public Huesped Obtener(int huespedID)
        {
            try
            {
                DataTable registros = new DataTable();
                Huesped huesped = new Huesped();

                // crear conexion
                SqlConnection conexionBD = ConexionBD.obtenerConexion();
                //SqlConnection conexionBD = new SqlConnection(cadenaConexionBD);

                // abrir conexion
                conexionBD.Open();

                // llenar una DataTable
                query = @"select * from huespedes where huesped_id = @huesped_id";
                SqlCommand comando = new SqlCommand(query, conexionBD);
                comando.Parameters.AddWithValue("@huesped_id", huespedID);

                SqlDataAdapter adaptadorDatos = new SqlDataAdapter(comando);
                adaptadorDatos.Fill(registros);


                // revisar si trajo el registro
                if (registros.Rows.Count > 0)
                {
                    huesped.huesped_id = int.Parse(registros.Rows[0]["huesped_id"].ToString());
                    huesped.nombre = registros.Rows[0]["nombre"].ToString();
                    huesped.apellido_1 = registros.Rows[0]["apellido_1"].ToString();
                    huesped.apellido_2 = registros.Rows[0]["apellido_2"].ToString();
                    huesped.calle = registros.Rows[0]["calle"].ToString();
                    huesped.colonia = registros.Rows[0]["colonia"].ToString();
                    huesped.codigo_postal = int.Parse(registros.Rows[0]["codigo_postal"].ToString());
                    huesped.ciudad = registros.Rows[0]["ciudad"].ToString();
                    huesped.correo = registros.Rows[0]["correo"].ToString();
                    huesped.numero_celular = registros.Rows[0]["numero_celular"].ToString();
                }

                // cerrar conexion
                conexionBD.Close();

                // devolver el DataTable
                return huesped;

            }
            catch (SqlException)
            {
                throw;
            }
        }

        public Huesped ObtenerPorCorreo(string correo, string contrasena)
        {
            try
            {
                Huesped huesped = null;

                using (SqlConnection conexionBD = new SqlConnection(cadenaConexionBD))
                {
                    conexionBD.Open();

                    string query = @"SELECT huesped_id, nombre, correo FROM huespedes 
                             WHERE correo = @correo AND contrasena = @contrasena";

                    using (SqlCommand comando = new SqlCommand(query, conexionBD))
                    {
                        comando.Parameters.AddWithValue("@correo", correo);
                        comando.Parameters.AddWithValue("@contrasena", contrasena);

                        using (SqlDataReader reader = comando.ExecuteReader())
                        {
                            if (reader.Read()) // Si hay un resultado
                            {
                                huesped = new Huesped
                                {
                                    huesped_id = Convert.ToInt32(reader["huesped_id"]),
                                    nombre = reader["nombre"].ToString(),
                                    correo = reader["correo"].ToString()
                                };
                            }
                        }
                    }
                }

                return huesped;
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al obtener el huésped por correo.", ex);
            }
        }

        public Huesped ObtenerPorCorreo(string correo)
        {
            try
            {
                Huesped huesped = null;

                using (SqlConnection conexionBD = new SqlConnection(cadenaConexionBD))
                {
                    conexionBD.Open();

                    string query = @"SELECT huesped_id, nombre, apellido_1, apellido_2, calle, colonia, codigo_postal,
                                    ciudad, correo, numero_celular, contrasena
                             FROM huespedes
                             WHERE correo = @correo";

                    using (SqlCommand comando = new SqlCommand(query, conexionBD))
                    {
                        comando.Parameters.AddWithValue("@correo", correo);

                        using (SqlDataReader reader = comando.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                huesped = new Huesped
                                {
                                    huesped_id = Convert.ToInt32(reader["huesped_id"]),
                                    nombre = reader["nombre"].ToString(),
                                    apellido_1 = reader["apellido_1"].ToString(),
                                    apellido_2 = reader["apellido_2"].ToString(),
                                    calle = reader["calle"].ToString(),
                                    colonia = reader["colonia"].ToString(),
                                    codigo_postal = Convert.ToInt32(reader["codigo_postal"]),
                                    ciudad = reader["ciudad"].ToString(),
                                    correo = reader["correo"].ToString(),
                                    numero_celular = reader["numero_celular"].ToString(),
                                    contrasena = reader["contrasena"].ToString()
                                };
                            }
                        }
                    }
                }

                return huesped;
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al obtener el huésped por correo.", ex);
            }
        }
        // Método para obtener todos los huespedes
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
                query = @"SELECT TOP (1000) [nombre]
                                            ,[apellido_1]
                                            ,[apellido_2]
                                            ,[calle]
                                            ,[colonia]
                                            ,[codigo_postal]
                                            ,[ciudad]
                                            ,[correo]
                                            ,[numero_celular]
                                            ,[huesped_id]
                                            FROM [Hotel].[dbo].[huespedes]";

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
        // Método para agregar un huesped
        public bool Agregar(Huesped huesped)
        {
            try
            {
                bool comandoEjecutado = false;

                // crear conexion
                SqlConnection conexionBD = new SqlConnection(cadenaConexionBD);

                // abrir conexion
                conexionBD.Open();

                // agregar el registro
                query = @"INSERT INTO huespedes (nombre, apellido_1, apellido_2, calle, colonia, codigo_postal, ciudad, correo, numero_celular, contrasena) 
                             VALUES (@nombre, @apellido_1, @apellido_2, @calle, @colonia, @codigo_postal, @ciudad, @correo, @numero_celular, @contrasena)";
                
                SqlCommand comando = new SqlCommand(query, conexionBD);
                //comando.Parameters.AddWithValue("@huesped_id", huesped.huesped_id);
                comando.Parameters.AddWithValue("@nombre", huesped.nombre);
                comando.Parameters.AddWithValue("@apellido_1", huesped.apellido_1);
                comando.Parameters.AddWithValue("@apellido_2", huesped.apellido_2);
                comando.Parameters.AddWithValue("@calle", huesped.calle);
                comando.Parameters.AddWithValue("@colonia", huesped.colonia);
                comando.Parameters.AddWithValue("@codigo_postal", huesped.codigo_postal);
                comando.Parameters.AddWithValue("@ciudad", huesped.ciudad);
                comando.Parameters.AddWithValue("@correo", huesped.correo);
                comando.Parameters.AddWithValue("@numero_celular", huesped.numero_celular);
                comando.Parameters.AddWithValue("@contrasena", huesped.contrasena);

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

        public bool Actualizar(Huesped huesped)
        {
            try
            {
                using (SqlConnection conexionBD = new SqlConnection(cadenaConexionBD))
                {
                    conexionBD.Open();

                    string query = @"UPDATE huespedes
                             SET nombre = @nombre,
                                 apellido_1 = @apellido_1,
                                 apellido_2 = @apellido_2,
                                 calle = @calle,
                                 colonia = @colonia,
                                 codigo_postal = @codigo_postal,
                                 ciudad = @ciudad,
                                 correo = @correo,
                                 numero_celular = @numero_celular,
                                 contrasena = @contrasena
                             WHERE huesped_id = @huesped_id;";

                    using (SqlCommand comando = new SqlCommand(query, conexionBD))
                    {
                        comando.Parameters.AddWithValue("@huesped_id", huesped.huesped_id);
                        comando.Parameters.AddWithValue("@nombre", huesped.nombre);
                        comando.Parameters.AddWithValue("@apellido_1", huesped.apellido_1);
                        comando.Parameters.AddWithValue("@apellido_2", huesped.apellido_2);
                        comando.Parameters.AddWithValue("@calle", huesped.calle);
                        comando.Parameters.AddWithValue("@colonia", huesped.colonia);
                        comando.Parameters.AddWithValue("@codigo_postal", huesped.codigo_postal);
                        comando.Parameters.AddWithValue("@ciudad", huesped.ciudad);
                        comando.Parameters.AddWithValue("@correo", huesped.correo);
                        comando.Parameters.AddWithValue("@numero_celular", huesped.numero_celular);
                        comando.Parameters.AddWithValue("@contrasena", huesped.contrasena);

                        int filasAfectadas = comando.ExecuteNonQuery();
                        return filasAfectadas > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                // Aquí puedes loggear el error o manejarlo mejor
                throw new Exception("Error al actualizar huésped.", ex);
            }
        }


        // Método para eliminar un huesped
        public bool Eliminar(int huespedID)
        {
            try
            {
                bool comandoEjecutado = false;

                // crear conexion
                SqlConnection conexionBD = new SqlConnection(cadenaConexionBD);

                // abrir conexion
                conexionBD.Open();

                // agregar el registro
                query = @"delete from huespedes where huesped_id = @huesped_id";
                SqlCommand comando = new SqlCommand(query, conexionBD);
                comando.Parameters.AddWithValue("@huesped_id", huespedID);

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
    }
}
