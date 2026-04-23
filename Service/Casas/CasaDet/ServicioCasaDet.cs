using Microsoft.AspNetCore.Components;
using ProyectoCasa.Model.Ahorro;
using ProyectoCasa.Model.Casa;
using ProyectoCasa.Model.Factura;
using ProyectoCasa.Repositorio.Casas.CasaDet;
using ProyectoCasa.Service.Facturas.FacturaDet;
using Supabase.Interfaces;

namespace ProyectoCasa.Service.Casas.CasaDet
{
    public class ServicioCasaDet
    {
        private readonly RepositorioCasaDet _repositorioCasaDet;

        public ServicioCasaDet(RepositorioCasaDet repositorioCasaDet)
        {
            _repositorioCasaDet = repositorioCasaDet;
        }

        public async Task AgregarDetalle(Mo_Casa_Det detalle, Mo_Casa casaCab)
        {
            //COMPROBACIÓN DE DATOS
            if (detalle == null ||
                string.IsNullOrWhiteSpace(detalle.Descripcion) ||
                detalle.Cantidad <= 0)
            {
                return;
            }


            //CASA NO CONTIENE ID PORQUE NO HA SIDO GUARDADA ANTERIORMENTE 
            //GUARDAMOS CASA Y OBTENEMOS EL ID GENERADO
            if (casaCab.Id == 0)
            {
                casaCab = await _repositorioCasaDet.GuardarCasa(casaCab);
            }



            detalle.CasaId = casaCab.Id;
            detalle.Fecha = DateTime.SpecifyKind(Convert.ToDateTime(detalle.Fecha), DateTimeKind.Utc);

            //GUARDAR DETALLE
            detalle = await _repositorioCasaDet.GuardarDetalleCasa(detalle);

            casaCab.LstDetalle.Add(detalle);
            casaCab.Saldo += detalle.Cantidad;
            //ACTUALIZAR LOS DATOS DE LA CABECERA
            await _repositorioCasaDet.ActualizarCasa(casaCab);
        }

        public async Task<Mo_Casa> CargarDatos(bool esEdicion, long? id)
        {
            if (!esEdicion)
            {
                return new Mo_Casa();
            }

            Mo_Casa editarCasa = await _repositorioCasaDet.CargarDatosCasa(id);
            return editarCasa;
        }


        //EDITAR LA LINEA SELECCIONADA.
        public async Task EditarLineaDetalleCasa(Mo_Casa_Det detalle, decimal? valorAntiguo)
        {
            if (detalle.Cantidad <= 0)
            {
                return;
            }

            detalle.Fecha = DateTime.SpecifyKind(Convert.ToDateTime(detalle.Fecha), DateTimeKind.Utc);
            await _repositorioCasaDet.ActualizarLineaDetalle(detalle);

            var casa = await _repositorioCasaDet.CargarDatosCasa(detalle.CasaId);
            if (casa != null)
            {
                decimal diferencia = Convert.ToDecimal(detalle.Cantidad - valorAntiguo);
                casa.Saldo += Convert.ToDecimal(diferencia);

                await _repositorioCasaDet.ActualizarCasa(casa);
            }
        }

        //PARA EL BOTÓN DE GUARDAR CAMBIOS!
        public async Task<Mo_Casa> GuardarDatosCasa(Mo_Casa casaCab)
        {
            if (casaCab.Id == 0)
            {
                casaCab = await _repositorioCasaDet.GuardarCasa(casaCab);
            }
            else
            {
                await _repositorioCasaDet.ActualizarCasa(casaCab);
            }

            return casaCab;
        }



    }
}
