using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using static ProyectoCasa.Model.Enumeraciones.Mo_Enumeracions;

namespace ProyectoCasa.Model.Factura
{
    [Table("mo_factura_cab")]
    public class Mo_Factura_Cab : BaseModel
    {
        public Mo_Factura_Cab()
        {
            LstFactDet = new List<Mo_Factura_Det>();
            Fecha = DateTime.Today;
        }

        [IgnoreDataMember]
        public List<Mo_Factura_Det> LstFactDet { get; set; }

        [Column("casaid")]
        public long CasaId { get; set; }

        [Column("ahorroid")]
        public long? AhorroId { get; set; }

        [PrimaryKey("id", false)]
        public long Id { get; set; }
        [Column("descripcion")]
        public string Descripcion { get; set; }
        [Column("fecha")]
        public DateTime Fecha { get; set; }
        [Column("totalgastado")]
        public decimal TotalGastado { get; set; }

        [Column("tipofactura")]
        public TipoFactura TipoFactura { get; set; }

    }
   
}
