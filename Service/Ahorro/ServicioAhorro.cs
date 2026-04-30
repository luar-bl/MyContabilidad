using ProyectoCasa.Model.Ahorro;
using ProyectoCasa.Model.Factura;
using ProyectoCasa.Repositorio.Ahorro;
using ProyectoCasa.Repositorio.Casas.CasaDet;
using ProyectoCasa.Repositorio.Facturas.FacturaDet;
using ProyectoCasa.Service.Casas.CasaDet;
using ProyectoCasa.Service.Facturas.FacturaDet;
using static ProyectoCasa.Model.Enumeraciones.Mo_Enumeracions;

namespace ProyectoCasa.Service.Ahorro
{
    public class ServicioAhorro
    {
        private readonly RepositorioFacturaDet _repositorFactDet;
        private readonly RepositorioAhorro _repositorioAhorro;
        private readonly RepositorioCasaDet _repositorioCasaDet;

        public ServicioAhorro(RepositorioAhorro repositorioAhorro, RepositorioCasaDet reposCasaDet, RepositorioFacturaDet repostFactDet)
        {
            _repositorioAhorro = repositorioAhorro;
            _repositorioCasaDet = reposCasaDet;
            _repositorFactDet = repostFactDet;
        }


        /// <summary>
        /// MODIFICAR UNA LINEA DE AHORRO
        /// </summary>
        /// <param name="ahorro"></param>
        /// <param name="valorAntiguo"></param>
        /// <returns></returns>
        public async Task EditarLineaAhorroDetalle(Mo_Ahorro ahorro, decimal? valorAntiguo)
        {
            if (ahorro.Cantidad <= 0)
            {
                return;
            }


            //OBTENEMOS LA CASA Y MODIFICAMOS SUS DATOS CORRESPONDIENTES
            var casa = await _repositorioCasaDet.CargarDatosCasa(ahorro.CasaId);
            if (casa == null) { return; }

            //GUARDAMOS LOS CAMBIOS DE AHORRO
            await _repositorioAhorro.GuardarEdicionAhorro(ahorro);

            decimal diferencia = Convert.ToDecimal(ahorro.Cantidad - valorAntiguo);
            casa.Ahorro += Convert.ToDecimal(diferencia); // - + = -$    // + + = +$
            casa.Saldo -= diferencia; // si la diferencia sale negativo - - = +$

            //ACTUALIZO LA CABECERA
            await _repositorioCasaDet.ActualizarCasa(casa);

            var facturaCab = await _repositorioAhorro.ObtenerFacturaDeAhorro(ahorro.Id);

            var det = facturaCab.LstFactDet.FirstOrDefault();
            det.Precio = Convert.ToDecimal(ahorro.Cantidad);
            det.Total = det.Precio * det.Cantidad;

            await _repositorFactDet.ActualizarFacturaDet(det);
        }

        //INGRESAR NUEVO AHORRO
        public async Task NuevoAhorroDetalle(Mo_Ahorro ahorro, long? idCasa)
        {
            if (string.IsNullOrWhiteSpace(ahorro.Descripcion) || ahorro.Cantidad <= 0) { return; }
            ahorro.CasaId = Convert.ToInt64(idCasa);

            var casaActual = await _repositorioCasaDet.CargarDatosCasa(idCasa);
            if (casaActual == null)
            {
                return;
            }

            casaActual.Ahorro += Convert.ToDecimal(ahorro.Cantidad);
            casaActual.Saldo -= Convert.ToDecimal(ahorro.Cantidad);

            Mo_Ahorro guardarAhorro = await _repositorioAhorro.AgregarAhorro(ahorro);

            await _repositorioCasaDet.ActualizarCasa(casaActual);


            //SE CREA UNA FACTURA NUEVA
            Mo_Factura_Cab facturaAhorro = new Mo_Factura_Cab();
            facturaAhorro.Descripcion = $"Ahorro casa {casaActual.Descripcion}";
            //facturaAhorro.Fecha = DateTime.Today;
            facturaAhorro.Fecha = DateTime.SpecifyKind(facturaAhorro.Fecha, DateTimeKind.Utc);
            facturaAhorro.CasaId = casaActual.Id;
            facturaAhorro.TipoFactura = TipoFactura.Ahorro;
            facturaAhorro.AhorroId = guardarAhorro.Id;

            //AL SER UNA FACTURA DE TIPO AHORRO NO ACTUALIZAMOS EL SALDO DE CASA.

            //GUARDAMOS LA FACTURA Y LA OBTENEMOS DE NUEVO. (SOLO PARA SABER EL ID QUE NOS DEVUELVE LA BBDD
            var guardarFactura = await _repositorFactDet.GuardarFacturaCab(facturaAhorro);
            if (guardarFactura == null) { return; }


            //CREACIÓN 
            Mo_Factura_Det detalleFactura = new Mo_Factura_Det();
            detalleFactura.FacturaCabId = guardarFactura.Id;
            detalleFactura.Cantidad = 1;
            detalleFactura.Producto = "Ahorro";
            detalleFactura.Precio = Convert.ToDecimal(ahorro.Cantidad);
            detalleFactura.Total = Convert.ToDecimal(ahorro.Cantidad * detalleFactura.Cantidad);
            //guardarFactura.TotalGastado = detalleFactura.Total;

            //AGREGAMOS EL DETALLE 
            await _repositorFactDet.AgregarDetalle(detalleFactura);
        }

    }
}
