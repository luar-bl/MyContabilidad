using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using MudBlazor;
using ProyectoCasa.Components.Modal;
using ProyectoCasa.Components.Pages.Facturas;
using ProyectoCasa.Model.Ahorro;
using ProyectoCasa.Model.Casa;

namespace ProyectoCasa.Components.Pages.Casa
{
    public partial class Pag_Mo_Casa_Det
    {


        decimal? _ValorAntiguo;

        ElementReference focusInputDescrip;

        private async Task Agregar()
        {
            try
            {
                if (SupabaseClient == null)
                {
                    return;
                }

                if (_det == null || _det != null && string.IsNullOrWhiteSpace(_det.Descripcion))
                {
                    return;
                }


                //CREAMOS LA CABECERA SI NO EXISTE
                if (_casa.Id == 0)
                {
                    if (string.IsNullOrWhiteSpace(_casa.Descripcion))
                    {
                        return;
                    }

                    //HACEMOS UN INSERT EN LA CABECERA PARA LUEGO OBTENER EL ID QUE NOS GENERA LA BASE DE DATOS PARA PODER ASIGNAR EL VALOR A SUS DETALLES
                    var resInsert = await SupabaseClient.From<Mo_Casa>().Insert(_casa);
                    var insertada = resInsert.Models.FirstOrDefault();
                    if (insertada != null)
                    {
                        _casa.Id = insertada.Id;
                        //_casa.Descripcion = insertada.Descripcion;
                        //_casa.Saldo = insertada.Saldo;
                    }
                }

                _det.CasaId = _casa.Id;
                _casa.LstDetalle.Add(_det);
                _det.Fecha = Convert.ToDateTime(_det.Fecha.ToString("MM/dd/yyyy"));
                _casa.Saldo += _det.Cantidad;


                //INSERTAR DETALLE
                await SupabaseClient.From<Mo_Casa_Det>().Insert(_det);

                //ACTUALIZAR EL MASTER
                await SupabaseClient.From<Mo_Casa>().Update(_casa);
            }
            catch (Exception)
            {
                throw;
            }

            _det = new Mo_Casa_Det();
            //await FocusInputDescripcion();
        }

        private async Task CargarDatos(bool esEdicion)
        {
            if (esEdicion)
            {
                var casaEncontrada = await SupabaseClient.From<Mo_Casa>().Where(x => x.Id == _Id).Single();
                if (casaEncontrada != null)
                {
                    _casa.Id = casaEncontrada.Id;
                    _casa.Descripcion = casaEncontrada.Descripcion;
                    _casa.Saldo = casaEncontrada.Saldo;

                    var lstDetalle = await SupabaseClient.From<Mo_Casa_Det>().Where(x => x.CasaId == _casa.Id).Get();
                    if (lstDetalle.Models.Count > 0)
                    {
                        _casa.LstDetalle = lstDetalle.Models.ToList();
                    }
                    else
                    {
                        _casa.LstDetalle = new List<Mo_Casa_Det>();
                    }
                }
                else
                {
                    _casa = new();
                    _casa.LstDetalle = new();
                }

            }
        }

        //private async Task CerrarModal()
        //{
        //    Visible = false;

        //    var casaActualizada = await SupabaseClient.From<Mo_Casa>().Where(x => x.Id == _casa.Id).Single();

        //    if (casaActualizada != null)
        //    {
        //        _casa.Saldo = casaActualizada.Saldo;

        //        await SupabaseClient.From<Mo_Casa>().Update(_casa);
        //        //Le decimos a Blazor que "refresh" de nuevo la pantalla
        //        StateHasChanged();
        //    }

        //}

        private async Task Editar(Mo_Casa_Det detSelect)
        {
            _ValorAntiguo = detSelect.Cantidad;
            //Visible = true;

            var parameter = new DialogParameters
            {
                ["DetalleCasa"] = detSelect,
                ["ValorAntiguo"] = _ValorAntiguo
            };

            var options = new DialogOptions { CloseOnEscapeKey = true, };

            var dialog = await DialogService.ShowAsync<Modal_Edicion_Detalle>(string.Empty, parameter, options);

            var res = await dialog.Result;
            if (!res.Canceled)
            {
                await CargarDatos(true);
                StateHasChanged();
                //Snackbar.add("¡Datos actualizados con éxito!", Severity.Success);
            }
        }


        //private async Task FocusInputDescripcion()
        //{
        //    await focusInputDescrip.FocusAsync();
        //}

        private async Task GuardarCambiosCab()
        {
            //SI ES UN ALTA NUEVA GUARDAMOS EL OBJETO Y RECUPERAMOS EL ID QUE LE GENERA LA BBDD
            if (_casa.Id == 0)
            {
                var resInsert = await SupabaseClient.From<Mo_Casa>().Insert(_casa);

                if (resInsert != null &&
                    resInsert.Models != null &&
                    resInsert.Models.Any())
                {
                    var recuperarId = resInsert.Models.FirstOrDefault();
                    if (recuperarId != null)
                    {
                        _casa.Id = recuperarId.Id;
                    }
                }
            }
            else
            {
                //HACEMOS UN UPDATE CON LOS CAMBIOS.
                await SupabaseClient.From<Mo_Casa>().Update(_casa);
            }
        }

        private List<Mo_Ahorro> MostrarAhorro()
        {
            if (_casa?.LstAhorros == null)
            {
                return _casa?.LstAhorros = new List<Mo_Ahorro>();
            }

            return _casa.LstAhorros.OrderBy(x => x.Id).ToList();
        }

        private List<Mo_Casa_Det> MostrarDetalle()
        {
            if (_casa?.LstDetalle == null)
            {
                return _casa?.LstDetalle = new List<Mo_Casa_Det>();
            }

            return _casa.LstDetalle.OrderBy(x => x.Id).ToList();
        }

        protected override async Task OnInitializedAsync()
        {
            await CargarDatos(esEdicion);
            //return base.OnInitializedAsync();
        }


        [Parameter]
        public long? _Id { get; set; }

        //public bool Visible { get; set; }

        public Mo_Casa _casa { get; set; } = new Mo_Casa();

        public Mo_Casa_Det _det { get; set; } = new Mo_Casa_Det();

        //COMPROBAR SI TIENE VALOR Y SI ES MAYOR QUE 0
        public bool esEdicion => _Id.HasValue && _Id.Value > 0;
    }
}
