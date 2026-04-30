using ProyectoCasa.Model.Factura;
using static ProyectoCasa.Model.Enumeraciones.Mo_Enumeracions;

namespace ProyectoCasa.DTos.Chart
{
    public class DTo_Chart_CasaIngreso
    {

        public long CasaId { get; set; }
        public long FacturaCab { get; set; }
        public string NombreCasa { get; set; }
        public decimal Cantidad { get; set; }
        public TipoFactura TipoFact { get; set; }
    }
}
