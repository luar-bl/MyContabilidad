using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Metadata;
using ProyectoCasa.Components.Pages.Facturas;
using ProyectoCasa.Model.Casa;
using ProyectoCasa.Model.Factura;

namespace ProyectoCasa.Components.Modal
{
    public partial class Modal_Edicion_Detalle
    {

        [Parameter]
        public Mo_Casa_Det? DetalleCasa { get; set; }

        [Parameter]
        public decimal? ValorAntiguo { get; set; }


        [Parameter]
        public Mo_Factura_Det? DetalleFactura { get; set; }

        [Parameter]
        public bool Visible { get; set; }

        [Parameter]
        public EventCallback OnClose { get; set; }

        private async Task Cerrar()
        {
            await OnClose.InvokeAsync();
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
                    decimal diferencia = Convert.ToDecimal(DetalleCasa.Cantidad -ValorAntiguo);

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
                    var listaDet =  await SupabaseClient.From<Mo_Factura_Det>().Where(x => x.FacturaCabId == factura.Id).Get();
                    if (listaDet.Models.Any())
                    {
                        factura.LstFactDet = listaDet.Models.ToList();
                    }

                    factura.TotalGastado = 0;
                    factura.TotalGastado += factura.LstFactDet.Sum(x => x.Total);

                    decimal diferencia = Convert.ToDecimal(factura.TotalGastado - ValorAntiguo);

                    await SupabaseClient.From<Mo_Factura_Cab>().Update(factura);

                    var casa = await SupabaseClient.From<Mo_Casa>().Where(c => c.Id == factura.CasaId).Single();
                    if (casa == null) { return; }

                    casa.Saldo += diferencia;


                    await SupabaseClient.From<Mo_Casa>().Update(casa);
                }
              
            }

            DetalleCasa = null;
            DetalleFactura = null;

            await Cerrar();
        }
    }
}
