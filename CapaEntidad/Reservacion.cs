using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Reservacion
    {
        public string estatus { get; set; }
        public DateTime fecha_entrada { get; set; }
        public DateTime fecha_salida { get; set; }
        public string nombre_huesped { get; set; }
        public int reservacion_id { get; set; }
        public int numero_personas { get; set; }
    }
}
