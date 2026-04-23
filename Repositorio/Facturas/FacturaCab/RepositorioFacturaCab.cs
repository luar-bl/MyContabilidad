using ProyectoCasa.Model.Factura;

namespace ProyectoCasa.Repositorio.Facturas.FacturaCab
{
    public class RepositorioFacturaCab
    {
        private readonly Supabase.Client _supabaseClient;

        public RepositorioFacturaCab(Supabase.Client supabaseCliente)
        {
            _supabaseClient = supabaseCliente;
        }

        public async Task<List<Mo_Factura_Cab>> ListaFacturasFiltrada(DateTime? Desde, DateTime? Hasta)
        {
            var lstFacturas = await _supabaseClient.From<Mo_Factura_Cab>()
                                                                    .Where(x => x.Fecha >= Desde &&
                                                                                x.Fecha <= Hasta)
                                                                    .Order(x => x.Fecha, Supabase.Postgrest.Constants.Ordering.Descending)
                                                                    .Get();

            return lstFacturas.Models?.OfType<Mo_Factura_Cab>().ToList();
        }
    }
}
