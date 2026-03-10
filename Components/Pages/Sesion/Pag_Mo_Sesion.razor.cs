using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using ProyectoCasa.Model.Sesion;
using ProyectoCasa.Service;

namespace ProyectoCasa.Components.Pages.Sesion
{
    public partial class Pag_Mo_Sesion
    {
        private string? returnUrl;

        private async Task OnSubmit()
        {
            errorMessage = string.Empty;

            errorMessage = await service.HandleLogin(loginModel, AuthStateProvider);

            if (string.IsNullOrEmpty(errorMessage))
            {
                // Refrescar UI
                if (AuthStateProvider is SupabaseAuthStateProvider customProvider)
                    customProvider.NotifyAuthStateChanged();

                var targetUrl = !string.IsNullOrEmpty(returnUrl)
                    ? Uri.UnescapeDataString(returnUrl)
                    : "/casa/Pag_Mo_Casa_Cab";

                Navigation.NavigateTo(targetUrl, true);
            }

            loginModel.Usuario = string.Empty;
            loginModel.Password = string.Empty;
        }

        protected override void OnInitialized()
        {
            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var query = QueryHelpers.ParseQuery(uri.Query);

            if (query.TryGetValue("ReturnUrl", out var url))
            {
                returnUrl = url;
            }
        }

        // Usamos [Inject] para que Blazor nos dé el servicio ya configurado
        [Inject] public ServicioBlanco service { get; set; }

        [Parameter]
        public string errorMessage { get; set; }


        public Mo_Sesion loginModel { get; set; } = new Mo_Sesion();
    }
}
