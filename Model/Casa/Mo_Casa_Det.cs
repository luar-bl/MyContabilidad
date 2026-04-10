using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ProyectoCasa.Model.Casa
{
    [Table("mo_casa_det")]
    public class Mo_Casa_Det : BaseModel
    {
        public Mo_Casa_Det() { }


        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("descripcion")]
        public string Descripcion { get; set; }

        [Column("cantidad")]
        public decimal Cantidad { get; set; }

        [Column("fecha")]
        public DateTime? Fecha { get; set; } = DateTime.Today;

        [Column("casaid")]
        public long CasaId { get; set; }

    }
}
