using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Huesped
    {
        public int huesped_id { get; set; }
        public string nombre { get; set; }
        public string apellido_1 { get; set; }
        public string apellido_2 { get; set; }
        public string calle { get; set; }
        public string colonia { get; set; }
        public int codigo_postal { get; set; }
        public string ciudad { get; set; }
        public string correo { get; set; }
        public string numero_celular { get; set; }
        public string contrasena { get; set; }
    }
}
