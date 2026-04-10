using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Metadata;
using MudBlazor;
using MudBlazor.Extensions;
using ProyectoCasa.Components.Pages.Facturas;
using ProyectoCasa.Model.Ahorro;
using ProyectoCasa.Model.Casa;
using ProyectoCasa.Model.Factura;
using System.Security.Cryptography;

namespace ProyectoCasa.Components.Modal
{
    public partial class Modal_Edicion_Detalle
    {

        private async Task Cerrar()
        {
            MudDialog.Close(DialogResult.Ok(true));
        }

        private async Task GuardarDatos()
        {
            if (DetalleCasa != null)
            {
                //ACTUALIZO EL DETALLE
                await SupabaseClient.From<Mo_Casa_Det>().Update(DetalleCasa);

                //OBTENO LA CASA
                var casa = await SupabaseClient.From<Mo_Casa>().Where(x => x.Id == DetalleCasa.CasaId).Single();
                if (casa != null)
                {
                    decimal diferencia = Convert.ToDecimal(DetalleCasa.Cantidad - ValorAntiguo);

                    casa.Saldo += Convert.ToDecimal(diferencia);


                    //ACTUALIZO LA CABECERA
                    await SupabaseClient.From<Mo_Casa>().Update(casa);
                }
            }

            if (DetalleFactura != null)
            {
                DetalleFactura.Total = DetalleFactura.Precio * DetalleFactura.Cantidad;
                await SupabaseClient.From<Mo_Factura_Det>().Update(DetalleFactura);

                var factura = await SupabaseClient.From<Mo_Factura_Cab>().Where(x => x.Id == DetalleFactura.FacturaCabId).Single();
                if (factura != null)
                {
                    var listaDet = await SupabaseClient.From<Mo_Factura_Det>().Where(x => x.FacturaCabId == factura.Id).Get();
                    if (listaDet.Models.Any())
                    {
                        factura.LstFactDet = listaDet.Models.ToList();
                    }


                    var casa = await SupabaseClient.From<Mo_Casa>().Where(c => c.Id == factura.CasaId).Single();
                    if (casa == null) { return; }

                    casa.Saldo += factura.TotalGastado;

                    factura.TotalGastado = 0;
                    factura.TotalGastado += factura.LstFactDet.Sum(x => x.Total);

                    //decimal diferencia = Convert.ToDecimal(factura.TotalGastado - ValorAntiguo);

                    await SupabaseClient.From<Mo_Factura_Cab>().Update(factura);

                    casa.Saldo -= factura.TotalGastado;

                    await SupabaseClient.From<Mo_Casa>().Update(casa);
                }
            }

            if (NuevoAhorro == true)
            {
                if (string.IsNullOrWhiteSpace(DetalleAhorro.Descripcion) || DetalleAhorro.Cantidad <= 0) { return; }
                DetalleAhorro.CasaId = IdCasa;

                var casaActual = await SupabaseClient.From<Mo_Casa>().Where(c => c.Id == IdCasa).Single();
                if (casaActual == null)
                {
                    return;
                }

                casaActual.Ahorro += Convert.ToDecimal(DetalleAhorro.Cantidad);
                casaActual.Saldo -= Convert.ToDecimal(DetalleAhorro.Cantidad);


                await SupabaseClient.From<Mo_Ahorro>().Insert(DetalleAhorro);

                Mo_Factura_Cab facturaAhorro = new Mo_Factura_Cab();
                facturaAhorro.Descripcion = $"Ahorro casa {casaActual.Descripcion}";
                facturaAhorro.Fecha = DateTime.Today;
                facturaAhorro.CasaId = casaActual.Id;
                facturaAhorro.TipoFactura = TipoFactura.Ahorro;

                var guardarFactura = await SupabaseClient.From<Mo_Factura_Cab>().Insert(facturaAhorro);
                var obtenerFacturaNueva = guardarFactura.Models.FirstOrDefault() ?? null;
                if (obtenerFacturaNueva == null) { return; }

                Mo_Factura_Det detalleFactura = new Mo_Factura_Det();
                detalleFactura.FacturaCabId = obtenerFacturaNueva.Id;
                detalleFactura.Cantidad = 1;
                detalleFactura.Producto = "Ahorro";
                detalleFactura.Precio = Convert.ToDecimal(DetalleAhorro.Cantidad);
                detalleFactura.Total = Convert.ToDecimal(DetalleAhorro.Cantidad * detalleFactura.Cantidad);

                obtenerFacturaNueva.TotalGastado = detalleFactura.Total;

                await SupabaseClient.From<Mo_Factura_Det>().Insert(detalleFactura);
            }


            DetalleCasa = null;
            DetalleFactura = null;
            await Cerrar();
        }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }


        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (NuevoAhorro == true)
            {
                DetalleAhorro = new Mo_Ahorro();
                DetalleAhorro.Cantidad = 0;
                DetalleAhorro.Descripcion = string.Empty;
            }
        }

        [Parameter]
        public bool NuevoAhorro { get; set; }

        [Parameter]
        public Mo_Casa_Det? DetalleCasa { get; set; }

        [Parameter]
        public decimal? ValorAntiguo { get; set; }

        [Parameter]
        public Mo_Factura_Det? DetalleFactura { get; set; }

        [Parameter]
        public Mo_Ahorro? DetalleAhorro { get; set; }

        [Parameter]
        public long IdCasa { get; set; }

        //[Parameter]
        //public bool Visible { get; set; }

        //[Parameter]
        //public EventCallback OnClose { get; set; }
    }
}
