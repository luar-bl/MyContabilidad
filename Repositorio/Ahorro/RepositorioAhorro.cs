using ProyectoCasa.Model.Ahorro;
using ProyectoCasa.Model.Factura;
using ProyectoCasa.Repositorio.Casas.CasaDet;

namespace ProyectoCasa.Repositorio.Ahorro
{
    public class RepositorioAhorro
    {
        private readonly Supabase.Client _supabaseClient;

        public RepositorioAhorro(Supabase.Client supabasClient)
        {
            _supabaseClient = supabasClient;
        }

        public async Task<Mo_Ahorro> AgregarAhorro(Mo_Ahorro ahorro)
        {
            var ahorroAdd = await _supabaseClient.From<Mo_Ahorro>().Insert(ahorro);
            Mo_Ahorro devolverAhorro = ahorroAdd.Models?.FirstOrDefault();

            return devolverAhorro;
        }

        public async Task GuardarEdicionAhorro(Mo_Ahorro ahorro)
        {
            await _supabaseClient.From<Mo_Ahorro>().Update(ahorro);
        }


        public async Task<Mo_Factura_Cab> ObtenerFacturaDeAhorro(long ahorroId)
        {
            if (ahorroId == 0) { return null; }

            var factura = await _supabaseClient.From<Mo_Factura_Cab>()
                                         .Where(x => x.AhorroId == ahorroId).Single();

            if (factura == null) { return null; }


            var buscarDetsFactura = await _supabaseClient.From<Mo_Factura_Det>()
                                         .Where(x => x.FacturaCabId == factura.Id).Get();
            if (buscarDetsFactura != null &&
                !buscarDetsFactura.Models.Any())
            {
                return null;
            }

            List<Mo_Factura_Det> lstDetallesFact = buscarDetsFactura.Models.OfType<Mo_Factura_Det>().ToList();
            factura.LstFactDet = lstDetallesFact;

            return factura;

        }
    }
}
