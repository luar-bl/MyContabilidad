using ProyectoCasa.Model.Casa;

namespace ProyectoCasa.Components.Pages.Casa
{
    public partial class Pag_Mo_Casa_Cab
    {
        private void EditarCasa(long id)
        {
            Navigation.NavigateTo($"/detalle-casa/{id}");
        }

        private void IrANuevaCasa()
        {
            Navigation.NavigateTo("/detalle-casa");
        }

        private async Task MostrarCasa()
        {
            var res = await SupabaseClient.From<Mo_Casa>().Get();

            if (res != null && res.Models.Count > 0)
            {
                LstCabecera = res.Models.ToList();
            }

            LstCabecera.ToList();
        }

        protected override async Task OnInitializedAsync()
        {
            await MostrarCasa();
            //return base.OnInitializedAsync();
        }

        public List<Mo_Casa> LstCabecera { get; set; } = new();

    }
}
