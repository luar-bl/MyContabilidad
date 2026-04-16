using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Supabase.Gotrue;
using System.Security.Claims;


namespace ProyectoCasa.Service
{
    public class SupabaseAuthStateProvider : AuthenticationStateProvider
    {
        //private readonly ILocalStorageService _localStorage;
        private readonly ISessionStorageService _sessionStorage;
        private readonly Supabase.Client _supabaseClient;

        //public SupabaseAuthStateProvider(Supabase.Client supabaseClient, ILocalStorageService localStorage)
        //{
        //    _supabaseClient = supabaseClient;
        //    _localStorage = localStorage;
        //}
        public SupabaseAuthStateProvider(Supabase.Client supabaseClient, ISessionStorageService sessionStorage)
        {
            _supabaseClient = supabaseClient;
            _sessionStorage = sessionStorage;
        }


        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // 1. Intentamos obtener la sesión actual del cliente
            var session = _supabaseClient.Auth.CurrentSession;


            // 2. Si no existe, intentamos recuperarla de LocalStorage
            if (session == null)
                session = await _sessionStorage.GetItemAsync<Session>("supabase_session");

            // 3. Si encontramos sesión en LocalStorage, la restauramos en Supabase
            if (session != null && _supabaseClient.Auth.CurrentSession == null)
            {
                //_supabaseClient.Auth.SetSession(session.AccessToken, session.RefreshToken);
                _supabaseClient.Auth.SetSession(session.AccessToken, null);
            }

            // 4. Si sigue sin haber usuario → no autenticado
            if (session?.User == null)
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            // 5. Crear Claims para Blazor
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, session.User.Email),
                new Claim(ClaimTypes.NameIdentifier, session.User.Id)
            };

            var identity = new ClaimsIdentity(claims, "SupabaseAuth");

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        // Este método lo llamaremos después de hacer SignIn o SignOut para refrescar la UI
        public void NotifyAuthStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
