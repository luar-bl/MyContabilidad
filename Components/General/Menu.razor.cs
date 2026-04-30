namespace ProyectoCasa.Components.General
{
    public partial class Menu
    {
        private void NavegarAListadoCasas()
        {
            Navigation.NavigateTo("/gestion-casas");
            //Navigation.NavigateTo("/casa/Pag_Mo_Casa_Cab");
        }

        private void NavegarAListadoFacturas()
        {
            Navigation.NavigateTo("/factura/Mo_Factura_Cab");
        }
        private void NavegarAGraficos()
        {
            Navigation.NavigateTo("/resumen-graficos");
        }
    }
}
