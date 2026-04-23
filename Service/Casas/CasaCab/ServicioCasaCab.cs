using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProyectoCasa.Model.Casa;
using ProyectoCasa.Repositorio.Casas.CasaCab;

namespace ProyectoCasa.Service.Casas.CasaCab
{
    public class ServicioCasaCab
    {
        private readonly NavigationManager _navigation;
        private readonly RepositorioCasaCab _repositorioCasaCab;

        public ServicioCasaCab(NavigationManager nav, RepositorioCasaCab respostCasaCab)
        {
            _navigation = nav;
            _repositorioCasaCab = respostCasaCab;
        }

        public void EditarCasa(long id)
        {
            _navigation.NavigateTo($"/detalle-casa/{id}");
        }

        public void IrANuevaCasa()
        {
            _navigation.NavigateTo("/detalle-casa");
        }

        public async Task<List<Mo_Casa>> ListadoCasas()
        {
            return await _repositorioCasaCab.ListaCasas();
        }
    }
}
