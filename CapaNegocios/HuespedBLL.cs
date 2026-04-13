using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CapaEntidad;
using CapaDatos;


namespace CapaNegocios
{
    public class HuespedBLL
    {
        private HuespedDAL huespedDAL = new HuespedDAL();

        public Huesped Obtener(int huespedID)
        {
            return huespedDAL.Obtener(huespedID);
        }


        public Huesped ValidarCredenciales(string correo, string contrasena)
        {
            return huespedDAL.ObtenerPorCorreo(correo, contrasena); // Retorna el huésped autenticado si existe
        }

        public Huesped ObtenerPorCorreo(string correo)
        {
            return huespedDAL.ObtenerPorCorreo(correo);
        }

        public DataTable ObtenerTodos()
        {
            return huespedDAL.ObtenerTodos();
        }

        public bool Agregar(Huesped huesped)
        {
            return huespedDAL.Agregar(huesped);
        }

        public bool Actualizar(Huesped huesped)
        {
            return huespedDAL.Actualizar(huesped);
        }

        public bool Eliminar(int huespedID)
        {
            return huespedDAL.Eliminar(huespedID);
        }

        public bool VerificarCorreoExistente(string correo)
        {
            return huespedDAL.VerificarCorreoExistente(correo);
        }
    }
}
