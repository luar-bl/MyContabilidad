using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProyectoCasa.Model.Sesion;

namespace ProyectoCasa.Service
{
    public class ServicioBlanco
    {
        public ServicioBlanco() { }

        public async Task HandleLogin(Mo_Sesion loginModel, string errorMessage, NavigationManager Navigation, Supabase.Client SupabaseClient)
        {
            // Lógica de autenticación aquí
            // Si es correcto, redirigir:

            var session = await SupabaseClient.Auth.SignUp(loginModel.Usuario, loginModel.Password);
            if (session == null)
            {
                errorMessage = "Error al iniciar session";
                Navigation.NavigateTo("/");
            }
            Navigation.NavigateTo("/casa/Pag_Mo_Casa_Cab");
        }
    }
}
