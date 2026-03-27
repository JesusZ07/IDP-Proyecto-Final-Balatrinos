using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocios
{
    public class ReservacionBLL
    {
        private ReservacionDAL reservacionDAL = new ReservacionDAL();

        public Reservacion Obtener(int reservacionID)
        {
            return reservacionDAL.Obtener(reservacionID);
        }

        public DataTable ObtenerTodos()
        {
            return reservacionDAL.ObtenerTodos();
        }

        public bool Agregar(Reservacion reservacion)
        {
            return reservacionDAL.Agregar(reservacion);
        }

        public bool Actualizar(Reservacion reservacion)
        {
            return reservacionDAL.Actualizar(reservacion);
        }

        public bool Eliminar(int reservacionID)
        {
            return reservacionDAL.Eliminar(reservacionID);
        }
    }
}
