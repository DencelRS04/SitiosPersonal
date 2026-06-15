using SitiosPersonal.Entities.ViewModels;
using SitiosPersonal.Repository.Repositories;

namespace SitiosPersonal.Services.Services
{
    public class BitacoraConsultaService
    {
        private readonly BitacoraRepository _repository;
        private readonly BitacoraService _bitacoraService;

        public BitacoraConsultaService(
            BitacoraRepository repository,
            BitacoraService bitacoraService)
        {
            _repository = repository;
            _bitacoraService = bitacoraService;
        }

        public BitacoraFiltroViewModel ObtenerBitacoras(
            string? usuarioFiltro,
            string? descripcionFiltro,
            string orden,
            int pagina,
            int cantidadPorPagina,
            int? idUsuarioEjecuta)
        {
            try
            {
                var filtro = new BitacoraFiltroViewModel
                {
                    UsuarioFiltro = usuarioFiltro,
                    DescripcionFiltro = descripcionFiltro,
                    Orden = orden,
                    Pagina = pagina,
                    CantidadPorPagina = cantidadPorPagina,
                    TotalRegistros = _repository.Contar(usuarioFiltro, descripcionFiltro),
                    Bitacoras = _repository.Listar(usuarioFiltro, descripcionFiltro, orden, pagina, cantidadPorPagina)
                };

                _bitacoraService.RegistrarConsulta(idUsuarioEjecuta, "Bitácora");

                return filtro;
            }
            catch (Exception ex)
            {
                _bitacoraService.RegistrarError(idUsuarioEjecuta, "Bitácora", ex.Message);
                throw;
            }
        }
    }
}
