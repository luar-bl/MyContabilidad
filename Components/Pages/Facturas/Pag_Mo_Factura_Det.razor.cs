using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using ProyectoCasa.Components.Modal;
using ProyectoCasa.Model.Casa;
using ProyectoCasa.Model.Factura;
using ProyectoCasa.Service.FacturaDet;

namespace ProyectoCasa.Components.Pages.Facturas
{
    public partial class Pag_Mo_Factura_Det
    {
        private readonly ServicioFacturaDet _servicioFacturaDet;

        public Pag_Mo_Factura_Det(ServicioFacturaDet servFactDet)
        {
            _servicioFacturaDet = servFactDet;
        }

        string errorMensaje { get; set; } = string.Empty;


        #region "Eventos de la página"

        //ESTA LÓGICA SE ENCARGA EL MÉTODO 'OnInitializedAsync' BASICAMENTE PARA CARGAR TODA LA INFORMACIÓN
        protected override async Task OnInitializedAsync()
        {
            #region -- Código Comentado --
            //await CargarDatos(esEdicion);
            //await CargarDatosCasas();
            //return base.OnInitializedAsync();
            #endregion

            LstCasas = await _servicioFacturaDet.ListaCasas();
            _facturaCab = await _servicioFacturaDet.CargarFacturaEdicion(esEdicion, _Id);
            //SIRVE PARA EL SELECT OPTION EN LA UI
        }
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
                _facturaCab = await _servicioFacturaDet.AgregarDetalle(_det, _facturaCab);
                #region -- Código Comentado --
                //if (SupabaseClient == null) { errorMensaje = "¡Error de conexión!"; return; }
                //if (_det == null || _det != null && string.IsNullOrWhiteSpace(_det.Producto)) { errorMensaje = "¡Datos vacios!"; return; }

                ////SI FACTURA ES NUEVA 
                //if (_facturaCab == null || _facturaCab.Id == 0)
                //{
                //    await GuardarFactura();
                //}

                //_det.FacturaCabId = _facturaCab.Id;
                //_det.Id_Interno = _facturaCab.LstFactDet.Count + 1;
                //_det.Total = _det.Cantidad * _det.Precio;
                //_facturaCab.LstFactDet.Add(_det);

                //_facturaCab.TotalGastado = _facturaCab.LstFactDet.Sum(x => x.Total);

                ////INSERTAR DETALLE Y OBTENER SU ID POR SI HAY QUE ELIMINAR.
                //var res = await SupabaseClient.From<Mo_Factura_Det>().Insert(_det);
                //var idDetalle = res.Models.FirstOrDefault();

                //if (idDetalle != null)
                //{
                //    _det.Id = idDetalle.Id;
                //}

                ////UPDATE SALDO CASA MIENTRAS NO SEA DE TIPO AHORRO
                //var casa = await SupabaseClient.From<Mo_Casa>().Where(x => x.Id == _facturaCab.CasaId).Single();
                //if (casa != null && _facturaCab.TipoFactura != TipoFactura.Ahorro)
                //{
                //    casa.Saldo -= _det.Total;

                //    await SupabaseClient.From<Mo_Casa>().Update(casa);
                //}

                //await GuardarFactura();

                #endregion
            }
            catch (Exception ex)
            {
                errorMensaje = ex.Message;
                throw;
            }


            _det = new Mo_Factura_Det();
            //await FocusInputProducto();
        }


        ////ESTE MÉTODO SE ENCARGA DE MOSTRAR DATOS EN CASO DE QUE ESTEMOS EDITANDO O SI ES UNA NUEVA FACTURA.
        //private async Task CargarDatos(bool _esEdicion)
        //{
        //    //SI NO ES UNA EDICIÓN
        //    if (!_esEdicion)
        //    {
        //        return;
        //    }

        //    //SI ES UNA EDICIÓN
        //    //BUSCAMOS LA FACTURA Y CARGAMOS SUS DATOS
        //    var facturaActual = await SupabaseClient.From<Mo_Factura_Cab>().Where(x => x.Id == _Id).Single();
        //    if (facturaActual != null)
        //    {
        //        _facturaCab.Fecha = DateTime.SpecifyKind(facturaActual.Fecha, DateTimeKind.Utc);
        //        _facturaCab = facturaActual;

        //        //COMPROBAMOS SI TIENE DETALLES.
        //        var detalleFactura = await SupabaseClient.From<Mo_Factura_Det>().Where(x => x.FacturaCabId == _facturaCab.Id).Get();

        //        if (detalleFactura.Models.Any())
        //        {
        //            _facturaCab.LstFactDet = detalleFactura.Models.ToList();
        //        }
        //    }
        //}

        #region -- Código Comentado
        ////ESTE MÉTODO SE ENCARGA DE CARGAR TODAS LAS CASAS QUE TENGAMOS EN LA BBDD YA QUE SE UTILIZA EN UN 'SELECT - HTML'
        //private async Task CargarDatosCasas()
        //{
        //    var listaCasas = await SupabaseClient.From<Mo_Casa>().Get();

        //    if (listaCasas.Models.Any())
        //    {
        //        LstCasas = listaCasas.Models.ToList();
        //    }
        //    else
        //    {
        //        LstCasas = new List<Mo_Casa>();
        //    }
        //}

        ////ESTE MÉTODO HACE FOCUS AL 'INPUT PRODUCTO - HTML'
        ////private async Task FocusInputProducto()
        ////{
        ////    await inputProducto.FocusAsync();
        ////}
        #endregion


        ////ESTE MÉTODO TE REDIRIGE A LA MISMA PÁGINA CREANDO UNA NUEVA FACTURA.
        private async Task NuevaFactura()
        {
            #region -- Código Comentado --
            //await GuardarFactura(); //GUARDAMOS LA FACTURA EN CASO DE QUE HAYA ESCRITO Y/O MODIFICADO ALGO.
            //_facturaCab = new Mo_Factura_Cab();
            //_det = new Mo_Factura_Det();
            //_Id = null;
            //Navigation.NavigateTo("/factura/Mo_Factura_Det"); //REDIRIGE A LA NUEVA PÁGINA DE FACTURA
            #endregion


            await _servicioFacturaDet.GuardarFacturaCabecera(_facturaCab);

            _facturaCab = new Mo_Factura_Cab();
            _det = new Mo_Factura_Det();
            _Id = null;
            await _servicioFacturaDet.NuevaFactura();

        }

        //ESTE MÉTODO GUARDA LA FACTURA Y OBTENEMOS EL ID QUE HA CREADO LA BBDD, YA QUE SE LO TENEMOS QUE ASIGNAR AL DETALLE 'MASTER OBJECT - DETAIL'
        //" -- TODO HIJO PERTENECE A UN PADRE -- ".
        private async Task GuardarFactura()
        {
            #region -- Código Comentado -- 
            //if (string.IsNullOrWhiteSpace(_facturaCab.Descripcion))
            //{
            //    return;
            //}

            ////CREAMOS LA CABECERA SI NO EXISTE
            //if (_facturaCab.Id == 0)
            //{
            //    _facturaCab.Fecha = DateTime.SpecifyKind(_facturaCab.Fecha, DateTimeKind.Utc);
            //    //HACEMOS UN INSERT EN LA CABECERA PARA LUEGO OBTENER EL ID QUE NOS GENERA LA BASE DE DATOS PARA PODER ASIGNAR EL VALOR A SUS DETALLES
            //    var res = await SupabaseClient.From<Mo_Factura_Cab>().Insert(_facturaCab);

            //    var fact = res.Models.FirstOrDefault();
            //    if (fact != null)
            //    {
            //        //_facturaCab.Fecha = _facturaCab.Fecha.AddDays(1);
            //        _facturaCab.Id = fact.Id;
            //        _facturaCab.Descripcion = fact.Descripcion;
            //        _facturaCab.Fecha = DateTime.SpecifyKind(_facturaCab.Fecha, DateTimeKind.Utc);
            //        //_facturaCab.Fecha = Convert.ToDateTime(fact.Fecha.ToString("dd/MM/yyyy"));
            //        _facturaCab.TipoFactura = fact.TipoFactura;
            //        _facturaCab.CasaId = fact.CasaId;
            //        _facturaCab.TotalGastado = fact.TotalGastado;
            //    }
            //}
            //else
            //{
            //    //_facturaCab.Fecha = _facturaCab.Fecha.AddDays(1);
            //    _facturaCab.Fecha = DateTime.SpecifyKind(_facturaCab.Fecha, DateTimeKind.Utc);
            //    await SupabaseClient.From<Mo_Factura_Cab>().Update(_facturaCab);
            //}
            #endregion

            _facturaCab = await _servicioFacturaDet.GuardarFacturaCabecera(_facturaCab);
            StateHasChanged();
        }

        //ESTE MÉTODO ELIMINA EL DETALLE Y DEVUELVE EL SALDO A LA CASA QUE PERTENECE.
        private async Task Eliminar(Mo_Factura_Det detFact)
        {
            //if (detFact == null || detFact?.Id <= 0) { errorMensaje = "¡No se ha podido eliminar el detalle!"; return; }

            try
            {

                var objDet = _facturaCab.LstFactDet.FirstOrDefault(x => x.Id == detFact.Id);
                await _servicioFacturaDet.EliminarDetalle(objDet, _facturaCab);

                _facturaCab.LstFactDet.Remove(objDet);
                _Visible = false;
                StateHasChanged();

                #region -- Código Comentado --
                //OBTENEMOS EL DETALLE QUE QUEREMOS ELIMINAR
                //var objDet = _facturaCab.LstFactDet.FirstOrDefault(x => x.Id == detFact.Id);


                //if (objDet == null) { return; }

                ////GUARDAMOS EL TOTAL GASTADO DE ESA LÍNEA
                ////decimal importeA_Borrar = _facturaCab.TotalGastado;
                //decimal importeA_Borrar = objDet.Total;

                ////RESTAMOS EL TOTALGASTADO DE ESTA FACTURA
                //_facturaCab.TotalGastado -= importeA_Borrar;

                ////BUSCAMOS LA CASA QUE PERTENECE EL DETALLE DE LA FACTURA Y LE SUMAMOS EL IMPORTE.
                //var casa = await SupabaseClient.From<Mo_Casa>().Where(x => x.Id == _facturaCab.CasaId).Single();
                //if (casa == null) { return; }

                //casa.Saldo += importeA_Borrar;

                ////BORRAMOS Y ACTUALIZAMOS LA INFORMACIÓN
                //await SupabaseClient.From<Mo_Factura_Det>().Delete(objDet);
                //await SupabaseClient.From<Mo_Factura_Cab>().Update(_facturaCab);
                //await SupabaseClient.From<Mo_Casa>().Update(casa);

                #endregion
            }
            catch (Exception ex)
            {

            }

        }
        #endregion

        #region "Propiedades de la página"

        //PROPIEDAD GLOBAL
        Mo_Factura_Det _det = new Mo_Factura_Det();

        //PROPIEDAD GLOBAL
        Mo_Factura_Cab _facturaCab = new Mo_Factura_Cab();

        //SIRVE COMO SI FUESE UN ID O UN CLASS EN HTML - @ref="inputProducto"
        ElementReference inputProducto;

        List<Mo_Casa> LstCasas { get; set; } = new List<Mo_Casa>();
        #endregion

        #region "Propiedades Parámetros"


        //SIRVE PARA SABER SI VAMOS A EDITAR UNA FACTURA O SI ES UNA NUEVA.
        [Parameter]
        public long? _Id { get; set; }

        //DEVUELVE TRUE O FALSE SI ES UNA EDICIÓN O UN NUEVO OBJETO 
        private bool esEdicion => _Id.HasValue && _Id.Value > 0;
        #endregion

        #region "Propiedades y Eventos para el Modal"

        private bool _Visible;
        private Mo_Factura_Det? _DetalleFactura_Modal;
        private decimal? _ValorAntiguo_Modal;

        private async Task Editar(Mo_Factura_Det det)
        {
            _DetalleFactura_Modal = det;

            _ValorAntiguo_Modal = det.Total;

            var parameter = new DialogParameters
            {
                ["DetalleFactura"] = _DetalleFactura_Modal,
                ["ValorAntiguo"] = _ValorAntiguo_Modal
            };

            var options = new DialogOptions { CloseOnEscapeKey = true, };

            var dialog = await DialogService.ShowAsync<Modal_Edicion_Detalle>("Simple Dialog", parameter, options);

            var res = await dialog.Result;
            if (!res.Canceled)
            {
                //await CargarDatos(true);

                await _servicioFacturaDet.CargarFacturaEdicion(true, _Id);
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
