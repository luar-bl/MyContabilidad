using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProyectoCasa.Service;

namespace ProyectoCasa.Components
{
    public partial class Routes
    {
        private bool sessionLoaded = false;

        [Inject] SupabaseAuthStateProvider test { get; set; }

        protected override async Task OnInitializedAsync()
        {

            // Esperar a que SupabaseAuthStateProvider cargue sesión
            var authState = await test.GetAuthenticationStateAsync();
            // sessionLoaded = true;
            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

            string returnUrl = null;
            if (query.TryGetValue("ReturnUrl", out var url))
            {
                returnUrl = Uri.UnescapeDataString(url);
            }

            if (authState.User.Identity != null && authState.User.Identity.IsAuthenticated)
            {
                sessionLoaded = true;
                Navigation.NavigateTo(returnUrl ?? "/casa/Pag_Mo_Casa_Cab");
            }
            else
            {
                Navigation.NavigateTo(uri.ToString());
            }
        }
    }
}
