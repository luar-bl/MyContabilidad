using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace ProyectoCasa.Model.Sesion
{
    [Table("mo_sesion")]
    public class Mo_Sesion : BaseModel
    {
        [Column("usuario")]
        public string Usuario { get; set; }

        [Column("password")]
        public string Password { get; set; }
    }
}
