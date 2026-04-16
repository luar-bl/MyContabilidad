using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ProyectoCasa.Model.Sesion;
using System.Security.Claims;


namespace ProyectoCasa.Service
{
    public class ServicioBlanco
    {
        //private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalStorageService _localStorage;
        private readonly ISessionStorageService _sessionStorage;
        private readonly Supabase.Client _supabaseClient;

        //public ServicioBlanco(Supabase.Client supabaseClient, ILocalStorageService localStorage)
        //{
        //    _supabaseClient = supabaseClient;
        //    _localStorage = localStorage;
        //}
        public ServicioBlanco(Supabase.Client supabaseClient, ISessionStorageService sessionStorage)
        {
            _supabaseClient = supabaseClient;
            _sessionStorage = sessionStorage;
        }

        public async Task<string> HandleLogin(Mo_Sesion login, AuthenticationStateProvider authProvider)
        {
            try
            {
                var response = await _supabaseClient.Auth.SignIn(login.Usuario, login.Password);

                if (response?.User == null)
                    return "Usuario o contraseña incorrectos";

                // Guardamos la sesión en LocalStorage
                await _sessionStorage.SetItemAsync("supabase_session", response);

                // Notificamos a Blazor que el estado de autenticación cambió
                if (authProvider is SupabaseAuthStateProvider custom)
                {
                    custom.NotifyAuthStateChanged();
                }
            }
            catch (Exception ex)
            {
                return "Usuario o contraseña incorrectos";
            }

            return string.Empty;
        }
    }
}
