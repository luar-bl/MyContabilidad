using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MudBlazor;
using ProyectoCasa.Model.Ahorro;
using ProyectoCasa.Model.Factura;
using ProyectoCasa.Repositorio.FacturaCab;

namespace ProyectoCasa.Service.FacturaCab
{
    public class ServicioFacturaCab
    {
        NavigationManager _navigation;
        private readonly RepositorioFacturaCab _repositorioFactCab;

        public ServicioFacturaCab(NavigationManager nav, RepositorioFacturaCab repositorioFacturaCab)
        {
            _navigation = nav;
            _repositorioFactCab = repositorioFacturaCab;
        }

        public void EditarFactura(long id)
        {
            _navigation.NavigateTo($"/factura/Mo_Factura_Det/{id}");
        }

        public void IrANuevaFactura()
        {
            _navigation.NavigateTo("/factura/Mo_Factura_Det");
        }

        public async Task<List<Mo_Factura_Cab>> ListaFacturas(DateTime? Desde, DateTime? Hasta)
        {
            return await _repositorioFactCab.ListaFacturasFiltrada(Desde, Hasta);
        }
    }
}
