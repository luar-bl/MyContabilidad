using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using ProyectoCasa.Components.Modal;
using ProyectoCasa.Model.Casa;
using ProyectoCasa.Model.Factura;

namespace ProyectoCasa.Components.Pages.Facturas
{
    public partial class Pag_Mo_Factura_Det
    {
        string errorMensaje { get; set; } = string.Empty;

        #region "Eventos de la página"
        protected override async Task OnInitializedAsync()
        {
            await CargarDatos(esEdicion);
            await CargarDatosCasas();
            //return base.OnInitializedAsync();
        }
        #endregion

        #region "Propiedades de la página"

        //PROPIEDAD GLOBAL
        Mo_Factura_Det _det = new Mo_Factura_Det();

        //PROPIEDAD GLOBAL
        Mo_Factura_Cab _facturaCab = new Mo_Factura_Cab();

        //SIRVE COMO SI FUESE UN ID O UN CLASS EN HTML - @ref="inputProducto"
        ElementReference inputProducto;

        public List<Mo_Casa> LstCasas = new List<Mo_Casa>();
        #endregion

        #region "Propiedades Parámetros"
        //ESTA LÓGICA SE ENCARGA EL MÉTODO 'OnInitializedAsync' BASICAMENTE PARA CARGAR TODA LA INFORMACIÓN

        //SIRVE PARA SABER SI VAMOS A EDITAR UNA FACTURA O SI ES UNA NUEVA.
        [Parameter]
        public long? _Id { get; set; }

        //DEVUELVE TRUE O FALSE SI ES UNA EDICIÓN O UN NUEVO OBJETO 
        private bool esEdicion => _Id.HasValue && _Id.Value > 0;
        #endregion

        #region "Métodos"
        //MUESTRA LOS DETALLES QUE TIENE EL MASTER-OBJECT
        private List<Mo_Factura_Det> MostrarDetalle()
        {
            return _facturaCab.LstFactDet.ToList();
        }

        //AGREGA UN NUEVO DETALLE AL MASTER-OBJECT
        private async Task AgregarDetalle()
        {
            try
            {
                if (SupabaseClient == null) { errorMensaje = "¡Error de conexión!"; return; }

                if (_det == null || _det != null && string.IsNullOrWhiteSpace(_det.Producto)) { errorMensaje = "¡Datos vacios!"; return; }

                await GuardarFactura();

                _det.FacturaCabId = _facturaCab.Id;
                _det.Id_Interno = _facturaCab.LstFactDet.Count + 1;
                _det.Total = _det.Cantidad * _det.Precio;
                _facturaCab.LstFactDet.Add(_det);

                _facturaCab.TotalGastado = _facturaCab.LstFactDet.Sum(x => x.Total);

                //INSERTAR DETALLE Y OBTENER SU ID POR SI HAY QUE ELIMINAR.
                var res = await SupabaseClient.From<Mo_Factura_Det>().Insert(_det);
                var idDetalle = res.Models.FirstOrDefault();

                if (idDetalle != null)
                {
                    _det.Id = idDetalle.Id;
                }

                //UPDATE SALDO CASA
                var casa = await SupabaseClient.From<Mo_Casa>().Where(x => x.Id == _facturaCab.CasaId).Single();
                if (casa != null)
                {
                    casa.Saldo -= _det.Total;

                    await SupabaseClient.From<Mo_Casa>().Update(casa);
                }

                //UPDATE FACTURA CAB
                await GuardarFactura();


            }
            catch (Exception ex)
            {
                errorMensaje = ex.Message;
                throw;
            }


            _det = new Mo_Factura_Det();
            await FocusInputProducto();
        }


        //ESTE MÉTODO SE ENCARGA DE MOSTRAR DATOS EN CASO DE QUE ESTEMOS EDITANDO O SI ES UNA NUEVA FACTURA.
        private async Task CargarDatos(bool _esEdicion)
        {
            //SI ES UNA EDICIÓN
            if (_esEdicion)
            {
                //BUSCAMOS LA FACTURA Y CARGAMOS SUS DATOS
                var facturaActual = await SupabaseClient.From<Mo_Factura_Cab>().Where(x => x.Id == _Id).Single();
                if (facturaActual != null)
                {
                    _facturaCab.Id = facturaActual.Id;
                    _facturaCab.Descripcion = facturaActual.Descripcion;
                    _facturaCab.Fecha = facturaActual.Fecha;
                    _facturaCab.CasaId = facturaActual.CasaId;
                    _facturaCab.TotalGastado = facturaActual.TotalGastado;
                    _facturaCab.TipoFactura = facturaActual.TipoFactura;

                    //COMPROBAMOS SI TIENE DETALLES.
                    var detalleFactura = await SupabaseClient.From<Mo_Factura_Det>().Where(x => x.FacturaCabId == _facturaCab.Id).Get();

                    if (detalleFactura.Models.Any())
                    {
                        _facturaCab.LstFactDet = detalleFactura.Models.ToList();
                    }
                    else
                    {
                        _facturaCab.LstFactDet = new List<Mo_Factura_Det>();
                    }
                }
                else
                {
                    _facturaCab = new Mo_Factura_Cab();
                    _facturaCab.LstFactDet = new List<Mo_Factura_Det>();
                }
            }
        }

        //ESTE MÉTODO SE ENCARGA DE CARGAR TODAS LAS CASAS QUE TENGAMOS EN LA BBDD YA QUE SE UTILIZA EN UN 'SELECT - HTML'
        private async Task CargarDatosCasas()
        {
            var listaCasas = await SupabaseClient.From<Mo_Casa>().Get();

            if (listaCasas.Models.Any())
            {
                LstCasas = listaCasas.Models.ToList();
            }
            else
            {
                LstCasas = new List<Mo_Casa>();
            }
        }

        //ESTE MÉTODO HACE FOCUS AL 'INPUT PRODUCTO - HTML'
        private async Task FocusInputProducto()
        {
            await inputProducto.FocusAsync();
        }

        //SIRVE PARA EL SELECT OPTION EN LA UI
        private List<Mo_Casa> MostrarDatosCasas()
        {
            return LstCasas?.ToList();
        }

        //ESTE MÉTODO TE REDIRIGE A LA MISMA PÁGINA CREANDO UNA NUEVA FACTURA.
        private async Task NuevaFactura()
        {
            await GuardarFactura(); //GUARDAMOS LA FACTURA EN CASO DE QUE HAYA ESCRITO Y/O MODIFICADO ALGO.
            _facturaCab = new Mo_Factura_Cab();
            _det = new Mo_Factura_Det();
            _Id = null;
            Navigation.NavigateTo("/factura/Mo_Factura_Det"); //REDIRIGE A LA NUEVA PÁGINA DE FACTURA
        }

        //ESTE MÉTODO GUARDA LA FACTURA Y OBTENEMOS EL ID QUE HA CREADO LA BBDD, YA QUE SE LO TENEMOS QUE ASIGNAR AL DETALLE 'MASTER OBJECT - DETAIL'
        //" -- TODO HIJO PERTENECE A UN PADRE -- ".
        private async Task GuardarFactura()
        {

            if (string.IsNullOrWhiteSpace(_facturaCab.Descripcion))
            {
                return;
            }

            //CREAMOS LA CABECERA SI NO EXISTE
            if (_facturaCab.Id == 0)
            {
                //HACEMOS UN INSERT EN LA CABECERA PARA LUEGO OBTENER EL ID QUE NOS GENERA LA BASE DE DATOS PARA PODER ASIGNAR EL VALOR A SUS DETALLES
                var res = await SupabaseClient.From<Mo_Factura_Cab>().Insert(_facturaCab);

                var fact = res.Models.FirstOrDefault();
                if (fact != null)
                {
                    _facturaCab.Id = fact.Id;
                    _facturaCab.Descripcion = fact.Descripcion;
                    _facturaCab.Fecha = Convert.ToDateTime(fact.Fecha.ToString("dd/MM/yyyy"));
                    _facturaCab.TipoFactura = fact.TipoFactura;
                    _facturaCab.CasaId = fact.CasaId;
                    _facturaCab.TotalGastado = fact.TotalGastado;
                }
            }
            else
            {
                await SupabaseClient.From<Mo_Factura_Cab>().Update(_facturaCab);
            }

            StateHasChanged();
        }

        //ESTE MÉTODO ELIMINA EL DETALLE Y DEVUELVE EL SALDO A LA CASA QUE PERTENECE.
        private async Task Eliminar(Mo_Factura_Det detFact)
        {
            if (detFact == null || detFact?.Id <= 0) { errorMensaje = "!No se ha podido eliminar el detalle!"; return; }

            try
            {
                //OBTENEMOS EL DETALLE QUE QUEREMOS ELIMINAR
                var objDet = _facturaCab.LstFactDet.FirstOrDefault(x => x.Id == detFact.Id);
                if (objDet == null) { return; }

                //GUARDAMOS EL TOTAL GASTADO DE ESA LÍNEA
                decimal importeA_Borrar = _facturaCab.TotalGastado;

                //RESTAMOS EL TOTALGASTADO DE ESTA FACTURA
                _facturaCab.TotalGastado -= objDet.Total;

                //BUSCAMOS LA CASA QUE PERTENECE EL DETALLE DE LA FACTURA Y LE SUMAMOS EL IMPORTE.
                var casa = await SupabaseClient.From<Mo_Casa>().Where(x => x.Id == _facturaCab.CasaId).Single();
                if (casa != null)
                {
                    casa.Saldo += importeA_Borrar - _facturaCab.TotalGastado;
                }
                else
                {
                    return;
                }

                //BORRAMOS Y ACTUALIZAMOS LA INFORMACIÓN
                await SupabaseClient.From<Mo_Factura_Det>().Delete(objDet);
                await SupabaseClient.From<Mo_Factura_Cab>().Update(_facturaCab);
                await SupabaseClient.From<Mo_Casa>().Update(casa);

                _facturaCab.LstFactDet.Remove(objDet);

                _Visible = false;
                StateHasChanged();

            }
            catch (Exception ex)
            {

            }

        }
        #endregion

        #region "Propiedades y Eventos para el Modal"

        private bool _Visible;
        private Mo_Factura_Det? _DetalleFactura;
        private decimal? _ValorAntiguo;

        private async Task Editar(Mo_Factura_Det det)
        {
            _DetalleFactura = det;

            _ValorAntiguo = det.Total;

            var parameter = new DialogParameters
            {
                ["DetalleFactura"] = _DetalleFactura,
                ["ValorAntiguo"] = _ValorAntiguo
            };

            var options = new DialogOptions { CloseOnEscapeKey = true, };

            var dialog = await DialogService.ShowAsync<Modal_Edicion_Detalle>("Simple Dialog", parameter, options);

            var res = await dialog.Result;
            if (!res.Canceled)
            {
                await CargarDatos(true);
                StateHasChanged();
            }
        }

        //private async Task CerrarModal()
        //{
        //    _Visible = false;

        //    var res = await SupabaseClient.From<Mo_Factura_Cab>().Where(x => x.Id == _facturaCab.Id).Single();
        //    if (res != null)
        //    {
        //        _facturaCab.TotalGastado = res.TotalGastado;

        //        StateHasChanged();
        //    }
        //}

        #endregion

    }
}
