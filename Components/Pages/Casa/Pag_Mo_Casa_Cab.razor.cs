using ProyectoCasa.Model.Casa;
using ProyectoCasa.Service.Casas.CasaCab;

namespace ProyectoCasa.Components.Pages.Casa
{
    public partial class Pag_Mo_Casa_Cab
    {
        private readonly ServicioCasaCab _servicioCasaCab;

        public Pag_Mo_Casa_Cab(ServicioCasaCab servCasaCab)
        {
            _servicioCasaCab = servCasaCab;
        }

        #region -- Propiedades --

        public List<Mo_Casa> LstCabecera { get; set; } = new();

        #endregion

        #region -- Métodos --
        private void EditarCasa(long id)
        {
            //Navigation.NavigateTo($"/detalle-casa/{id}");
            _servicioCasaCab.EditarCasa(id);
        }

        private void IrANuevaCasa()
        {
            //Navigation.NavigateTo("/detalle-casa");
            _servicioCasaCab.IrANuevaCasa();
        }
        private async Task MostrarCasa()
        {
            LstCabecera = await _servicioCasaCab.ListadoCasas();
            #region -- Código Comentado --
            //var res = await SupabaseClient.From<Mo_Casa>().Get();

            //if (res != null && res.Models.Count > 0)
            //{
            //    LstCabecera = res.Models.ToList();
            //}

            //LstCabecera.ToList();
            #endregion
        }

        protected override async Task OnInitializedAsync()
        {
            await MostrarCasa();
            //return base.OnInitializedAsync();
        }
        #endregion

    }
}
