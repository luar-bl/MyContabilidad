using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ProyectoCasa.Model.Factura
{
    [Table("mo_factura_det")]
    public class Mo_Factura_Det : BaseModel
    {
        public Mo_Factura_Det()
        {

        }

        [Column("facturacabid")]
        public long FacturaCabId { get; set; }

        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [IgnoreDataMember]
        public int Id_Interno { get; set; }

        [Column("producto")]
        public string Producto { get; set; }
        [Column("precio")]
        public decimal Precio { get; set; }
        [Column("cantidad")]
        public int Cantidad { get; set; } = 1;
        [Column("total")]
        public decimal Total { get; set; }
    }
}
