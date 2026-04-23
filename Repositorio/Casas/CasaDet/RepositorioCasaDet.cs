using MudBlazor.Extensions;
using ProyectoCasa.Model.Ahorro;
using ProyectoCasa.Model.Casa;
using Supabase.Interfaces;
using Supabase.Postgrest.Responses;

namespace ProyectoCasa.Repositorio.Casas.CasaDet
{
    public class RepositorioCasaDet
    {
        private readonly Supabase.Client _supabaseClient;

        public RepositorioCasaDet(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        public async Task ActualizarCasa(Mo_Casa casaCab)
        {
            await _supabaseClient.From<Mo_Casa>().Update(casaCab);
        }

        public async Task ActualizarLineaDetalle(Mo_Casa_Det detalle)
        {
            await _supabaseClient.From<Mo_Casa_Det>().Update(detalle);
        }



        public async Task<Mo_Casa> CargarDatosCasa(long? id)
        {
            Mo_Casa obtenerCasaActual = await _supabaseClient.From<Mo_Casa>()
                                                  .Where(x => x.Id == id)
                                                  .Single();

            if (obtenerCasaActual == null)
            {
                return null;
            }

            var lstDetCasas = await _supabaseClient.From<Mo_Casa_Det>()
                                                                 .Where(x => x.CasaId == id)
                                                                 .Get();
            if (lstDetCasas != null &&
                lstDetCasas.Models.Any())
            {
                obtenerCasaActual.LstDetalle = lstDetCasas.Models;
            }

            var lstDetAhorro = await _supabaseClient.From<Mo_Ahorro>()
                                                    .Where(x => x.CasaId == id)
                                                    .Get();

            if (lstDetAhorro != null &&
                lstDetAhorro.Models.Any())
            {
                obtenerCasaActual.LstAhorros = lstDetAhorro.Models;
            }

            return obtenerCasaActual;
        }

        public async Task<Mo_Casa> GuardarCasa(Mo_Casa casaCab)
        {
            var insertCasa = await _supabaseClient.From<Mo_Casa>().Insert(casaCab);
            var devolverCasaInsertada = insertCasa.Models.FirstOrDefault();

            if (devolverCasaInsertada != null &&
                insertCasa.Models.Any())
            {
                return devolverCasaInsertada;
            }
            else
            {
                return casaCab;
            }
        }

        public async Task<Mo_Casa_Det> GuardarDetalleCasa(Mo_Casa_Det detalle)
        {
            var res = await _supabaseClient.From<Mo_Casa_Det>().Insert(detalle);
            if (res == null &&
                !res.Models.Any())
            {
                return null;
            }

            var detCasa = res.Models.FirstOrDefault();
            return detCasa;
        }

    }
}
