using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    class ConexionBD
    {
        public static string cadenaConexionBD = "Data Source=.;Initial Catalog=Hotel;Integrated Security=True;TrustServerCertificate=True";

        public static SqlConnection obtenerConexion()
        {
            try
            {
                // crear conexion
                SqlConnection conexionBD = new SqlConnection(cadenaConexionBD);

                // devolver conexion
                return conexionBD;
            }
            catch (SqlException)
            {
                throw;
            }

        }

    }
}
