using Microsoft.AspNetCore.Components;
using ProyectoCasa.Model.Sesion;
using ProyectoCasa.Service;

namespace ProyectoCasa.Components.Pages.Sesion
{
    public partial class Pag_Mo_Sesion
    {

        private ServicioBlanco servicioBlanco = new ServicioBlanco();

        [Parameter]
        public string errorMessage { get; set; }

        [SupplyParameterFromForm(FormName = "MyUniqueFormName")]
        public Mo_Sesion loginModel { get; set; } = new Mo_Sesion();
    }
}
