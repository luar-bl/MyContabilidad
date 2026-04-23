using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MudBlazor;
using ProyectoCasa.Model.Casa;
using ProyectoCasa.Model.Factura;
using ProyectoCasa.Repositorio.Facturas.FacturaDet;

namespace ProyectoCasa.Service.Facturas.FacturaDet
{
    public class ServicioFacturaDet
    {
        private readonly RepositorioFacturaDet _facturaDetRepositorio;
        public List<Mo_Casa> LstCasas = new List<Mo_Casa>();

        public ServicioFacturaDet(RepositorioFacturaDet repositorioFacDet, NavigationManager nav)
        {
            _facturaDetRepositorio = repositorioFacDet;
            Navigation = nav;
        }

        [Inject]
        NavigationManager Navigation { get; set; }

        public async Task<Mo_Factura_Cab> AgregarDetalle(Mo_Factura_Det detalle, Mo_Factura_Cab facturaCab)
        {
            if (detalle == null || detalle != null && string.IsNullOrWhiteSpace(detalle.Producto)) { return null; }

            //SI FACTURA ES NUEVA 
            if (facturaCab.Id == 0)
            {
                facturaCab.Fecha = DateTime.SpecifyKind(facturaCab.Fecha, DateTimeKind.Utc);
                facturaCab = await _facturaDetRepositorio.GuardarFacturaCab(facturaCab);
            }

            detalle.FacturaCabId = facturaCab.Id;
            detalle.Id_Interno = facturaCab.LstFactDet.Count + 1;
            detalle.Total = detalle.Cantidad * detalle.Precio;
            facturaCab.LstFactDet.Add(detalle);

            facturaCab.TotalGastado = facturaCab.LstFactDet.Sum(x => x.Total);

            //INSERTAR DETALLE Y OBTENER SU ID POR SI HAY QUE ELIMINAR.
            await _facturaDetRepositorio.AgregarDetalle(detalle);

            Mo_Casa casa = await _facturaDetRepositorio.ObtenerCasa(facturaCab.CasaId);
            if (casa != null && facturaCab.TipoFactura != TipoFactura.Ahorro)
            {
                casa.Saldo -= detalle.Total;

                await _facturaDetRepositorio.ActualizarCasa(casa);
            }


            //ACTUALIZAMOS LA FACTURA CAB POR SI HA HABIDO ALGÚN CAMBIO
            facturaCab.Fecha = DateTime.SpecifyKind(facturaCab.Fecha, DateTimeKind.Utc);
            await _facturaDetRepositorio.ActualizarFacturaCab(facturaCab);

            return facturaCab;
        }

        public async Task<Mo_Factura_Cab> CargarFacturaEdicion(bool esEdicion, long? id)
        {
            Mo_Factura_Cab facturaCab = new Mo_Factura_Cab();
            if (!esEdicion)
            {
                return facturaCab;
            }

            facturaCab = await _facturaDetRepositorio.ObtenerInfoFacturaCab(id.Value);
            if (facturaCab == null) { return null; }

            facturaCab.Fecha = DateTime.SpecifyKind(facturaCab.Fecha, DateTimeKind.Utc);

            var lstDetalleFac = await _facturaDetRepositorio.ObtenerDetallesFacturaCabPorId(id.Value);
            facturaCab.LstFactDet = lstDetalleFac;

            return facturaCab;
        }

        //DEVUELVE TRUE O FALSE SI ES UNA EDICIÓN O UN NUEVO OBJETO 
        public bool EditarFactura(long? _Id)
        {
            return _Id.HasValue && _Id.Value > 0;
        }


        //ESTE MÉTODO ELIMINA EL DETALLE Y DEVUELVE EL SALDO A LA CASA QUE PERTENECE.
        public async Task EliminarDetalle(Mo_Factura_Det detFact, Mo_Factura_Cab facturaCab)
        {
            if (detFact == null || detFact?.Id <= 0) { /*errorMensaje = "¡No se ha podido eliminar el detalle!";*/ return; }

            try
            {
                //OBTENEMOS EL DETALLE QUE QUEREMOS ELIMINAR
                var objDet = facturaCab.LstFactDet.FirstOrDefault(x => x.Id == detFact.Id);
                if (objDet == null) { return; }

                //GUARDAMOS EL TOTAL GASTADO DE ESA LÍNEA
                //decimal importeA_Borrar = facturaCab.TotalGastado;
                decimal importeA_Borrar = objDet.Total;

                //RESTAMOS EL TOTALGASTADO DE ESTA FACTURA
                facturaCab.TotalGastado -= importeA_Borrar;

                //BUSCAMOS LA CASA QUE PERTENECE EL DETALLE DE LA FACTURA Y LE SUMAMOS EL IMPORTE.
                var casa = await _facturaDetRepositorio.ObtenerCasa(facturaCab.CasaId);
                if (casa == null) { return; }
                casa.Saldo += importeA_Borrar;

                //BORRAMOS Y ACTUALIZAMOS LA INFORMACIÓN

                await _facturaDetRepositorio.BorrarDetalleFactura(objDet);
                await _facturaDetRepositorio.ActualizarFacturaCab(facturaCab);
                await _facturaDetRepositorio.ActualizarCasa(casa);

                //facturaCab.LstFactDet.Remove(objDet);
                //_Visible = false;
                //StateHasChanged();

            }
            catch (Exception ex)
            {

            }
        }


        /// <summary>
        /// SE UTILIZA CUANDO CREAMOS HACEMOS CLICK EN EL BOTÓN EN NUEVA FACTURA
        /// SE UTILIZA PARA EL BOTÓN DE GUARDAR CAMBIOS
        /// </summary>
        /// <param name="facturaCab"></param>
        /// <returns></returns>
        public async Task<Mo_Factura_Cab> GuardarFacturaCabecera(Mo_Factura_Cab facturaCab)
        {
            if (string.IsNullOrWhiteSpace(facturaCab.Descripcion))
            {
                return null;
            }

            facturaCab.Fecha = DateTime.SpecifyKind(facturaCab.Fecha, DateTimeKind.Utc);

            Mo_Factura_Cab factCab = null;

            if (facturaCab.Id == 0)
            {
                factCab = await _facturaDetRepositorio.GuardarFacturaCab(facturaCab);
                facturaCab.Id = factCab.Id;
            }
            else
            {
                await _facturaDetRepositorio.ActualizarFacturaCab(facturaCab);
            }

            return facturaCab;
        }

        public async Task<List<Mo_Casa>> ListaCasas()
        {
            return await _facturaDetRepositorio.ListaCasas() ?? new List<Mo_Casa>();
        }

        public async Task NuevaFactura()
        {
            Navigation.NavigateTo("/factura/Mo_Factura_Det"); //REDIRIGE A LA NUEVA PÁGINA DE FACTURA
        }
    }
}
