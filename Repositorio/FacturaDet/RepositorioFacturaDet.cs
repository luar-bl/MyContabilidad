using ProyectoCasa.Model.Casa;
using ProyectoCasa.Model.Factura;
using Supabase.Interfaces;

namespace ProyectoCasa.Repositorio.FacturaDet
{
    public class RepositorioFacturaDet
    {
        private readonly Supabase.Client _supabaseClient;

        public RepositorioFacturaDet(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        public async Task ActualizarCasa(Mo_Casa casa)
        {
            await _supabaseClient.From<Mo_Casa>().Update(casa);
        }


        public async Task ActualizarFacturaCab(Mo_Factura_Cab facturaCab)
        {
            await _supabaseClient.From<Mo_Factura_Cab>().Update(facturaCab);

        }

        public async Task AgregarDetalle(Mo_Factura_Det detalle)
        {
            await _supabaseClient.From<Mo_Factura_Det>().Insert(detalle);
        }

        public async Task BorrarDetalleFactura(Mo_Factura_Det detalle)
        {
            await _supabaseClient.From<Mo_Factura_Det>().Delete(detalle);
        }

        public async Task<Mo_Factura_Cab> GuardarFacturaCab(Mo_Factura_Cab facturaCab)
        {
            var res = await _supabaseClient.From<Mo_Factura_Cab>().Insert(facturaCab);
            if (res != null)
            {
                var obtenerFacInsertada = res.Models.FirstOrDefault();
                if (obtenerFacInsertada != null)
                {
                    facturaCab.Id = obtenerFacInsertada.Id;
                }
            }
            return facturaCab;
        }


        public async Task<List<Mo_Casa>> ListaCasas()
        {
            var res = await _supabaseClient.From<Mo_Casa>().Get();
            if (res != null)
            {
                return res.Models;
            }
            else
            {
                return null;
            }
        }

        public async Task<Mo_Casa> ObtenerCasa(long idCasa)
        {
            Mo_Casa obtenerCasa = await _supabaseClient.From<Mo_Casa>()
                                                       .Where(x => x.Id == idCasa)
                                                       .Single() ?? null;

            return obtenerCasa;
        }

        public async Task<List<Mo_Factura_Det>> ObtenerDetallesFacturaCabPorId(long idFacturaCab)
        {
            var detallesFactura = await _supabaseClient.From<Mo_Factura_Det>().Where(x => x.FacturaCabId == idFacturaCab).Get();
            if (detallesFactura != null &&
                detallesFactura.Models.Count > 0)
            {
                return detallesFactura.Models;
            }
            return new List<Mo_Factura_Det>();
        }

        public async Task<Mo_Factura_Cab> ObtenerInfoFacturaCab(long idFacturaCab)
        {
            Mo_Factura_Cab facturaActual = await _supabaseClient.From<Mo_Factura_Cab>().Where(x => x.Id == idFacturaCab).Single();
            if (facturaActual != null)
            {
                return facturaActual;
            }
            return new Mo_Factura_Cab();
        }

    }
}
