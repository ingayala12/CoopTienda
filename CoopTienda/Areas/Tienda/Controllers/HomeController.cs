using CoopTienda.AccesoDatos.Repositorio;
using CoopTienda.AccesoDatos.Repositorio.IRepositorio;
using CoopTienda.Modelo;
using CoopTienda.Modelo.Especificaciones;
using CoopTienda.Modelo.ViewsModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CoopTienda.Areas.Tienda.Controllers
{
    [Area("Tienda")]
    public class HomeController : Controller
    {
        private readonly IUnidadTrabajo unidadTrabajo;

        public HomeController(IUnidadTrabajo unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;
        }
        [HttpGet]
        public IActionResult Index(int pageNumber = 1, string busqueda = "", string busquedaActual = "")
        {
            if (!String.IsNullOrEmpty(busqueda))
            {
                pageNumber = 1;
            }
            else
            {
                busqueda = busquedaActual;
            }
            ViewData["BusquedaActual"] = busqueda;

            if (pageNumber < 1) { pageNumber = 1;}

            Parametros parametros = new Parametros()
            {
                PageNumber = pageNumber,
                PageSize = 4
            };

            var resultado = unidadTrabajo.Producto.ObtenerTodosPaginado(parametros);

            if (!String.IsNullOrEmpty(busqueda))
            {
                resultado = unidadTrabajo.Producto.ObtenerTodosPaginado(parametros, p => p.Descripcion.ToLower().Trim().Contains(busqueda) || p.Codigo.ToString().Contains(busqueda));
            }

            ViewData["TotalPaginas"] = resultado.MetaData.TotalPages;
            ViewData["TotalRegistros"] = resultado.MetaData.TotalCount;
            ViewData["PageSize"] = resultado.MetaData.PageSize;
            ViewData["PageNumber"] = pageNumber;
            ViewData["Previo"] = "disabled";  // clase css para desactivar el boton
            ViewData["Siguiente"] = "";

            if (pageNumber > 1) { ViewData["Previo"] = ""; }
            if (resultado.MetaData.TotalPages <= pageNumber) { ViewData["Siguiente"] = "disabled"; }

            return View(resultado);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
