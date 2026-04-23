using ProyectoCasa.Model.Casa;

namespace ProyectoCasa.Repositorio.Casas.CasaCab
{
    public class RepositorioCasaCab
    {

        private readonly Supabase.Client _supabaseClient;

        public RepositorioCasaCab(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        public async Task<List<Mo_Casa>> ListaCasas()
        {
            var res = await _supabaseClient.From<Mo_Casa>().Get();
            if (res != null)
            {
                return res.Models.OfType<Mo_Casa>().ToList();
            }
            else
            {
                return new List<Mo_Casa>();
            }
        }
    }
}
