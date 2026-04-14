
using MudBlazor;
using ProyectoCasa.Model.Factura;
using static MudBlazor.CategoryTypes;

namespace ProyectoCasa.Components.Pages.Facturas
{
    public partial class Pag_Mo_Factura_Cab
    {

        //public List<Mo_Factura_Cab> LstFacturasCab = new List<Mo_Factura_Cab>();
        public List<Mo_Factura_Cab> LstFacturasCab = new List<Mo_Factura_Cab>();

        public Pag_Mo_Factura_Cab() { }

        private async Task CargarDatos()
        {
            try
            {
                var nuevaFechaDesde = _dateRange.Start;
                var nuevaFechaHasta = _dateRange.End;

                //CERRAR EL SELECCIONADOR DE FECHAS
                if (_picker != null && _picker.DateRange != null)
                {
                    await _picker.CloseAsync();
                }

                var res = await SupabaseClient.From<Mo_Factura_Cab>().Get();
                if (res != null && res.Models != null && res.Models.Any())
                {
                    //LstFacturasCab = res.Models.OrderBy(x => x.Id).ToList();
                    LstFacturasCab = res.Models.Where(s => s.Fecha >= nuevaFechaDesde &&
                                                           s.Fecha <= nuevaFechaHasta)
                                                .OrderBy(x => x.Id)
                                                .ToList();
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

            Desde = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); //PRIMER DÍA DEL MES
            Hasta = Desde?.AddMonths(1).AddDays(-1); //ÚLTIMO DÍA DEL MES

            _picker = new MudDateRangePicker();
            _dateRange = new DateRange(Desde, Hasta); //FECHAS QUE LE DAMOS A LA ETIQUETA MUD_BLAZOR_DATE_PICKER

            await CargarDatos();
            //return base.OnInitializedAsync();
        }

        #region -- Propiedades Globales --

        private MudDateRangePicker _picker;

        private DateRange _dateRange { set; get; }

        private DateTime? Desde { get; set; }
        private DateTime? Hasta { get; set; }

        #endregion

    }
}
