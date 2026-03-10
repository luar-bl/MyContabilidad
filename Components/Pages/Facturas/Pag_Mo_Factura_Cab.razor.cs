
using ProyectoCasa.Model.Factura;

namespace ProyectoCasa.Components.Pages.Facturas
{
    public partial class Pag_Mo_Factura_Cab
    {


        public List<Mo_Factura_Cab> LstFacturasCab = new List<Mo_Factura_Cab>();

        public Pag_Mo_Factura_Cab() { }


        private async Task CargarDatos()
        {
            var res = await SupabaseClient.From<Mo_Factura_Cab>().Get();

            if (res.Models.Count > 0)
            {
                LstFacturasCab = res.Models.OrderBy(x => x.Id).ToList();
            }
        }

        private void EditarFactura(long id)
        {
            Navigation.NavigateTo($"/factura/Mo_Factura_Det/{id}");
        }

        private void IrANuevaFactura()
        {
            Navigation.NavigateTo("/factura/Mo_Factura_Det");
        }

        protected override async Task OnInitializedAsync()
        {
            await CargarDatos();
            //return base.OnInitializedAsync();
        }
    }
}
