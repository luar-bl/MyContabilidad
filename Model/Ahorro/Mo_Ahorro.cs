using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ProyectoCasa.Model.Ahorro
{
    [Table("mo_ahorro")]
    public class Mo_Ahorro : BaseModel
    {
        public Mo_Ahorro() { }


        [Column("casaid")]
        public long CasaId { get; set; }

        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("descripcion")]
        public string Descripcion { get; set; }

        [Column("cantidad")]
        public decimal Cantidad { get; set; }
    }
}
