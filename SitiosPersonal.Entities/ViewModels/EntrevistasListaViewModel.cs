using System;
using System.Collections.Generic;

namespace SitiosPersonal.Entities.ViewModels
{
    public class EntrevistasListaViewModel
    {
        public EntrevistasListaViewModel()
        {
            Entrevistas = new List<EntrevistaListaItem>();
        }

        public List<EntrevistaListaItem> Entrevistas { get; set; }

        public int Pagina { get; set; }

        public int CantidadPorPagina { get; set; }

        public int TotalRegistros { get; set; }

        public int TotalPaginas
        {
            get
            {
                return (int)Math.Ceiling((double)TotalRegistros / CantidadPorPagina);
            }
        }
    }

    public class EntrevistaListaItem
    {
        public int id_entrevista { get; set; }

        public int id_oferente { get; set; }

        public string NombreOferente { get; set; }

        public string NombreEmpleado { get; set; }

        public string NumeroEmpleado { get; set; }

        public DateTime fecha_entrevista { get; set; }

        public TimeSpan hora_entrevista { get; set; }

        public string estado { get; set; }
    }
}