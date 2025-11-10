using System.Diagnostics;
using System.Threading.Tasks;
using CoopTienda.AccesoDatos.Repositorio.IRepositorio;
using CoopTienda.Modelo;
using CoopTienda.Modelo.ViewsModels;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Index()
        {
            IEnumerable<Producto> listaProductos = await unidadTrabajo.Producto.ObtenerTodos();
            return View(listaProductos);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
