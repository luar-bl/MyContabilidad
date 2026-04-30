using ProyectoCasa.DTos.Chart;
using ProyectoCasa.Repositorio.Chart;
using static ProyectoCasa.Model.Enumeraciones.Mo_Enumeracions;

namespace ProyectoCasa.Service.Charts
{
    public class ServicioCharts
    {

        private readonly RepositorioChart _repositorioChart;

        public ServicioCharts(RepositorioChart repositorioChart)
        {
            _repositorioChart = repositorioChart;
        }

        public async Task<List<DTo_Chart_CasaIngreso>> ServObtenerIngresos()
        {
            var lstDetallesIngresos = await _repositorioChart.ReposObtenerDetallesIngresos();
            if (lstDetallesIngresos == null || !lstDetallesIngresos.Any()) { return new List<DTo_Chart_CasaIngreso>(); }

            var lstCasas = await _repositorioChart.ReposObtenerCasas();

            var resDetalle = lstDetallesIngresos.GroupBy(x => new
            {
                Casa = x.CasaId,
            }).Select(z => new DTo_Chart_CasaIngreso()
            {
                CasaId = z.Key.Casa,
                Cantidad = z.Sum(i => i.Cantidad)
            }
            ).ToList();

            var resultado = lstCasas.Join(resDetalle,
                a => a.Id,
                b => b.CasaId,
                (a, b) => new DTo_Chart_CasaIngreso()
                {
                    NombreCasa = a.Descripcion,
                    CasaId = b.CasaId,
                    Cantidad = b.Cantidad
                }).ToList();

            return resultado;
        }


        public async Task<List<DTo_Chart_CasaIngreso>> ServObtenerGastos()
        {
            var lstDetallesFactura = await _repositorioChart.ReposObtenerFacturaDetalleGastos();
            if (lstDetallesFactura == null ||
                !lstDetallesFactura.Any())
            {
                return new List<DTo_Chart_CasaIngreso>();
            }

            var lstFacturasCabs = await _repositorioChart.ReposObtenerFacturaCabGastos();
            if (lstFacturasCabs == null ||
                !lstFacturasCabs.Any())
            {
                return new List<DTo_Chart_CasaIngreso>();
            }

            //SOLO FACTURAS QUE NO SEAN DE TIPO AHORRO
            var lstFacturaSinTipoAhorro = lstFacturasCabs.Where(x => x.TipoFactura != TipoFactura.Ahorro).ToList();

            //EJEMPLO DEL RESULTADO OBTENIDO EN ESTA CONSULTA
            //TipoFact: Suministros  =>  Cantidad: 150
            //TipoFact: Suministros  =>  Cantidad: 200
            //TipoFact: Restaurantes =>  Cantidad: 80
            var lstFactDetalleAgrupado = lstDetallesFactura.GroupBy(x => new
            {
                FactId = x.FacturaCabId,
            }).Select(x => new DTo_Chart_CasaIngreso()
            {
                FacturaCab = x.Key.FactId,
                Cantidad = x.Sum(t => t.Total)
            }).ToList();

            //JOIN PARA COMBINAR VARIAS TABLAS (con id que comparten) (Mo_Factura_Cab y Mo_Factura_Det), detalles pertenece a Cabecera.
            //AGRUPAMOS X TIPO FACTURA
            //RESULTADO DEL SELECT:
            //TipoFact: Suministros  =>  Cantidad: 350
            //TipoFact: Restaurantes =>  Cantidad: 80
            var lstResultadoJoin = lstFactDetalleAgrupado.Join(lstFacturaSinTipoAhorro,
                                                              detalleFact => detalleFact.FacturaCab,
                                                              factCab => factCab.Id,
                                                           (detalleFact, factCab) => new DTo_Chart_CasaIngreso()
                                                           {
                                                               TipoFact = factCab.TipoFactura,
                                                               Cantidad = detalleFact.Cantidad
                                                           })
                                                        .GroupBy(f => f.TipoFact)
                                                        .Select(n => new DTo_Chart_CasaIngreso()
                                                        {
                                                            TipoFact = n.Key,
                                                            Cantidad = n.Sum(s => s.Cantidad)
                                                        })
                                                        .ToList();

            return lstResultadoJoin;

        }

        public async Task<List<DTo_Chart_CasaIngreso>> ServObtenerAhorros()
        {
            var lstAhorros = await _repositorioChart.ReposObtenerAhorros();
            if(lstAhorros == null ||
                !lstAhorros.Any())
            {
                return new List<DTo_Chart_CasaIngreso>();
            }

            var lstCasas = await _repositorioChart.ReposObtenerCasas();
            if (lstCasas == null ||
                !lstCasas.Any())
            {
                return new List<DTo_Chart_CasaIngreso>();
            }

            var lstResultadoAgrupado = lstAhorros.Join(lstCasas,
                                                        ahorro => ahorro.CasaId,
                                                        casa => casa.Id,
                                                        (ahorro, casa) => new DTo_Chart_CasaIngreso
                                                        {
                                                            CasaId = casa.Id,
                                                            NombreCasa = casa.Descripcion,
                                                            Cantidad = Convert.ToDecimal(ahorro.Cantidad)
                                                        })
                                                 .GroupBy(x => x.CasaId)
                                                 .Select(x => new DTo_Chart_CasaIngreso()
                                                 {
                                                     NombreCasa = x.FirstOrDefault()?.NombreCasa,
                                                     Cantidad =  x.Sum(c => c.Cantidad)
                                                 })
                                                 .ToList();

            return lstResultadoAgrupado;
        }


    }
}
