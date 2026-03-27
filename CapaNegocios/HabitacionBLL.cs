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
    public class HabitacionBLL
    {
        private HabitacionDAL habitacionDAL = new HabitacionDAL();

        public Habitacion Obtener(int numero_habitacion)
        {
            return habitacionDAL.Obtener(numero_habitacion);
        }

        public DataTable ObtenerTodos()
        {
            return habitacionDAL.ObtenerTodos();
        }

        public bool Agregar(Habitacion habitacion)
        {
            return habitacionDAL.Agregar(habitacion);
        }

        public bool Actualizar(Habitacion habitacion)
        {
            return habitacionDAL.Actualizar(habitacion);
        }

        public bool Eliminar(int numero_habitacion)
        {
            return habitacionDAL.Eliminar(numero_habitacion);
        }
    }
}
