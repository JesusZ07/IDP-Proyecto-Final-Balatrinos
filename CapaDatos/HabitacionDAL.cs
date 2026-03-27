using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;

namespace CapaDatos
{
    public class HabitacionDAL
    {
        private string cadenaConexionBD = "Data Source=.;Initial Catalog=Hotel;Integrated Security=True;TrustServerCertificate=True";
        private string query = "";

        public Habitacion Obtener(int numero_habitacion)
        {
            try
            {
                DataTable registros = new DataTable();
                Habitacion habitacion = new Habitacion();

                // crear conexion
                SqlConnection conexionBD = ConexionBD.obtenerConexion();
                //SqlConnection conexionBD = new SqlConnection(cadenaConexionBD);

                // abrir conexion
                conexionBD.Open();

                // llenar una DataTable
                query = @"select * from habitaciones where numero_habitacion = @numero_habitacion";
                SqlCommand comando = new SqlCommand(query, conexionBD);
                comando.Parameters.AddWithValue("@numero_habitacion", numero_habitacion);

                SqlDataAdapter adaptadorDatos = new SqlDataAdapter(comando);
                adaptadorDatos.Fill(registros);


                // revisar si trajo el registro
                if (registros.Rows.Count > 0)
                {
                    habitacion.numero_habitacion = int.Parse(registros.Rows[0]["numero_habitacion"].ToString());
                    habitacion.tipo_habitacion = registros.Rows[0]["tipo_habitacion"].ToString();
                    habitacion.piso = int.Parse(registros.Rows[0]["piso"].ToString());
                    habitacion.estatus = registros.Rows[0]["estatus"].ToString();
                }

                // cerrar conexion
                conexionBD.Close();

                // devolver el DataTable
                return habitacion;

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


        // Método para obtener todos los habitaciones
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
                query = @"SELECT habitacion.numero_habitacion, 
                                 habitacion.tipo_habitacion,   
                                 habitacion.piso,  
                                 habitacion.estatus 
                                 FROM habitaciones AS habitacion";

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

        // Método para agregar un habitacion
        public bool Agregar(Habitacion habitacion)
        {
            try
            {
                using (SqlConnection conexionBD = new SqlConnection(ConexionBD.cadenaConexionBD))
                {
                    conexionBD.Open();

                    string query = @"INSERT INTO habitaciones (numero_habitacion, tipo_habitacion, piso, estatus) 
                                 VALUES (@numero_habitacion, @tipo_habitacion, @piso, @estatus)";

                    using (SqlCommand comando = new SqlCommand(query, conexionBD))
                    {
                        comando.CommandType = System.Data.CommandType.Text;
                        comando.Parameters.AddWithValue("@numero_habitacion", habitacion.numero_habitacion);
                        comando.Parameters.AddWithValue("@tipo_habitacion", habitacion.tipo_habitacion);
                        comando.Parameters.AddWithValue("@piso", habitacion.piso);
                        comando.Parameters.AddWithValue("@estatus", habitacion.estatus);

                        return comando.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error SQL: " + ex.Message);
                return false;
            }
        }

        // Método para actualizar un habitacion
        public bool Actualizar(Habitacion habitacion)
        {
            try
            {
                bool comandoEjecutado = false;

                // crear conexion
                SqlConnection conexionBD = new SqlConnection(cadenaConexionBD);

                // abrir conexion
                conexionBD.Open();

                // agregar el registro
                query = @"UPDATE habitaciones
                          SET numero_habitacion = @numero_habitacion,
                          tipo_habitacion = @tipo_habitacion,
                          piso = @piso,
                          estatus = @estatus
                          WHERE numero_habitacion = @numero_habitacion";

                // crear el comando
                SqlCommand comando = new SqlCommand(query, conexionBD);
                comando.Parameters.AddWithValue("@numero_habitacion", habitacion.numero_habitacion);
                comando.Parameters.AddWithValue("@tipo_habitacion", habitacion.tipo_habitacion);
                comando.Parameters.AddWithValue("@piso", habitacion.piso);
                comando.Parameters.AddWithValue("@estatus", habitacion.estatus);

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

        // Método para eliminar un habitacion
        public bool Eliminar(int numero_habitacion)
        {
            try
            {
                bool comandoEjecutado = false;

                // crear conexion
                SqlConnection conexionBD = new SqlConnection(cadenaConexionBD);

                // abrir conexion
                conexionBD.Open();

                // agregar el registro
                query = @"delete from habitaciones where numero_habitacion = @numero_habitacion";
                SqlCommand comando = new SqlCommand(query, conexionBD);
                comando.Parameters.AddWithValue("@numero_habitacion", numero_habitacion);

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
