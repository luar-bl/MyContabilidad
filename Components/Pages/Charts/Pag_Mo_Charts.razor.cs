using ProyectoCasa.Service.Charts;

namespace ProyectoCasa.Components.Pages.Charts
{
    public partial class Pag_Mo_Charts
    {
        private readonly ServicioCharts _servicioCharts;
        public Pag_Mo_Charts(ServicioCharts servicioChart)
        {
            _servicioCharts = servicioChart;
        }

        #region -- Propiedades --
        private string[] LstNombreCasa;
        private decimal[] LstIngresos;

        private decimal[] LstGastos;
        private string[] LstTipoFactura;

        private decimal[] LstAhorros;

        #endregion

        private async Task CargarDatos()
        {
            var lstObtenerIngresos = await _servicioCharts.ServObtenerIngresos();
            var lstObtenerGastos = await _servicioCharts.ServObtenerGastos();
            var lstObtenerAhorros = await _servicioCharts.ServObtenerAhorros();

            LstNombreCasa = new string[lstObtenerIngresos.Count] ;
            LstIngresos = new decimal[lstObtenerIngresos.Count];

            LstGastos = new decimal[lstObtenerGastos.Count];
            LstTipoFactura = new string[lstObtenerGastos.Count];

            LstAhorros = new decimal[lstObtenerAhorros.Count];

            //INGRESOS
            for (int i = 0; i < lstObtenerIngresos.Count; i++)
            {
                LstIngresos[i] = lstObtenerIngresos[i].Cantidad;
                LstNombreCasa[i] = $"{Convert.ToString(lstObtenerIngresos[i].NombreCasa)}: {lstObtenerIngresos[i].Cantidad}€";
            }

            //GASTOS
            for (int i = 0; i < lstObtenerGastos.Count; i++)
            {
                LstGastos[i] = lstObtenerGastos[i].Cantidad;
                LstTipoFactura[i] = $"{Enum.GetName(lstObtenerGastos[i].TipoFact)}: {lstObtenerGastos[i].Cantidad}€";
            }

            //AHORROS
            if (!LstNombreCasa.Any())
            {
                for (int i = 0; i < lstObtenerAhorros.Count; i++)
                {
                    LstAhorros[i] = lstObtenerAhorros[i].Cantidad;
                    LstNombreCasa[i] = lstObtenerAhorros[i].NombreCasa;
                }
            }
            else
            {
                for (int i = 0; i < lstObtenerAhorros.Count; i++)
                {
                    LstAhorros[i] = lstObtenerAhorros[i].Cantidad;
                }
            }

            //ADDRANGE  GUARDA MAS DE UN VALOR.
            //ADD ESPERA UN UNICO VALOR.
            //LstIngresos.AddRange(result.Select(x => x.Cantidad));
            //LstNombreCasa.AddRange(result.Select(x => x.CasaId.ToString()));
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                LstNombreCasa = new string[0];
                LstIngresos = new decimal[0];
                LstGastos = new decimal[0];
                LstTipoFactura = new string[0];
                LstAhorros = new decimal[0];
                await CargarDatos();
            }
            catch (Exception)
            {

                throw;
            }
           
            //return base.OnInitializedAsync();
        }

    }
}
