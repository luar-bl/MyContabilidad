using ProyectoCasa.Model.Ahorro;
using ProyectoCasa.Model.Casa;
using ProyectoCasa.Model.Factura;

namespace ProyectoCasa.Repositorio.Chart
{
    public class RepositorioChart
    {
        private readonly Supabase.Client _supabaseClient;

        public RepositorioChart(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        public async Task<List<Mo_Casa>> ReposObtenerCasas()
        {
            List<Mo_Casa> lstCasa = new List<Mo_Casa>();

            var res = await _supabaseClient.From<Mo_Casa>().Get();
            if (res == null ||
               res.Models == null ||
               !res.Models.Any())
            {
                return null;
            }

            lstCasa = res.Models.OfType<Mo_Casa>().ToList();    

            return lstCasa;
        }

        public async Task<List<Mo_Casa_Det>> ReposObtenerDetallesIngresos()
        {
            List<Mo_Casa_Det> lstCasaDets = new List<Mo_Casa_Det>();

            var res = await _supabaseClient.From<Mo_Casa_Det>().Get();
            if(res == null || 
               res.Models == null ||
               !res.Models.Any())
            {
                return null;
            }

            lstCasaDets = res.Models.OfType<Mo_Casa_Det>().ToList();
            return lstCasaDets;
        }

        public async Task<List<Mo_Factura_Det>> ReposObtenerFacturaDetalleGastos()
        {
            List<Mo_Factura_Det> lstFacturaDets = new List<Mo_Factura_Det>();

            var res = await _supabaseClient.From<Mo_Factura_Det>().Get();
            if(res == null || 
               res.Models == null ||
               !res.Models.Any())
            {
                return null;
            }

            lstFacturaDets = res.Models.OfType<Mo_Factura_Det>().ToList();
            return lstFacturaDets;
        }

        public async Task<List<Mo_Factura_Cab>> ReposObtenerFacturaCabGastos()
        {
            List<Mo_Factura_Cab> lstFacturaCab = new List<Mo_Factura_Cab>();

            var res = await _supabaseClient.From<Mo_Factura_Cab>().Get();
            if (res == null ||
               res.Models == null ||
               !res.Models.Any())
            {
                return null;
            }

            lstFacturaCab = res.Models.OfType<Mo_Factura_Cab>().ToList();
            return lstFacturaCab;
        }

        public async Task<List<Mo_Ahorro>> ReposObtenerAhorros()
        {
            List<Mo_Ahorro> lstAhorro = new List<Mo_Ahorro>();

            var res = await _supabaseClient.From<Mo_Ahorro>().Get();
            if (res == null ||
               res.Models == null ||
               !res.Models.Any())
            {
                return null;
            }

            lstAhorro = res.Models.OfType<Mo_Ahorro>().ToList();
            return lstAhorro;
        }
    }
}
