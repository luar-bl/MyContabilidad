
using ProyectoCasa.Model.Factura;

namespace ProyectoCasa.Components.Pages.Facturas
{
    public partial class Pag_Mo_Factura_Cab
    {

        //public List<Mo_Factura_Cab> LstFacturasCab = new List<Mo_Factura_Cab>();
        public IEnumerable<Mo_Factura_Cab> LstFacturasCab = new List<Mo_Factura_Cab>();

        public Pag_Mo_Factura_Cab() { }

        private async Task CargarDatos()
        {
            try
            {
                var res = await SupabaseClient.From<Mo_Factura_Cab>().Get();

                if (res != null && res.Models != null && res.Models.Count > 0)
                {
                    //LstFacturasCab = res.Models.OrderBy(x => x.Id).ToList();
                    LstFacturasCab = res.Models.OrderBy(x => x.Id).ToList();
                }
            }
            catch (Exception)
            {
                throw;
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
