using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.Runtime.Serialization;


namespace ProyectoCasa.Model.Casa
{
    [Table("mo_casa")]
    public class Mo_Casa : BaseModel
    {

        public Mo_Casa() { }


        [IgnoreDataMember]
        public List<Mo_Casa_Det> LstDetalle { get; set; }

        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("descripcion")]
        public string Descripcion { get; set; }

        [Column("saldo")]
        public decimal Saldo { get; set; }

    }
}
