using CoopTienda.AccesoDatos.Repositorio.IRepositorio;
using CoopTienda.Modelo;
using CoopTienda.Utilidades;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoopTienda.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AlmacenController : Controller
    {
        private readonly IUnidadTrabajo unidadTrabajo;

        public AlmacenController(IUnidadTrabajo unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Almacen> listaAlmacen = await unidadTrabajo.Almacen.ObtenerTodos();
            return View(listaAlmacen);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Almacen almacen)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(almacen.Nombre))
                {
                    ModelState.AddModelError("Nombre", "El nombre del almacén es obligatorio");
                    return View(almacen);
                }

                var existe = await unidadTrabajo.Almacen.ObtenerPrimero(
                    x => x.Nombre.ToLower().Trim() == almacen.Nombre.ToLower().Trim()
                );

                if (existe is not null)
                {
                    ModelState.AddModelError("Nombre", "El almacén ya existe");
                    return View(almacen);
                }

                await unidadTrabajo.Almacen.Agregar(almacen);
                TempData[DS.Exitoso] = "Almacen Agregado Correctamente";
                await unidadTrabajo.Guardar();

                return RedirectToAction(nameof(Index), "Almacen", new { Area = "Admin"});
            }

            return View(almacen);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var existe = await unidadTrabajo.Almacen.Obtener(id);

            if (existe is null)
            {
                return NotFound();
            }
            return View(existe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Almacen almacen)
        {
            if (ModelState.IsValid)
            {
                // Verificar si existe otro almacén con el mismo nombre
                var existe = await unidadTrabajo.Almacen.ObtenerPrimero(
                    x => x.Nombre.ToLower().Trim() == almacen.Nombre.ToLower().Trim()
                      && x.Id != almacen.Id
                );

                if (existe is not null)
                {
                    ModelState.AddModelError("Nombre", "Ya existe un almacén con este nombre");
                    return View(almacen);
                }

                // Actualizar el almacén
                await unidadTrabajo.Almacen.Actualizar(almacen);
                await unidadTrabajo.Guardar();

                TempData["Exitoso"] = "Almacén actualizado correctamente";

                return RedirectToAction("Index", "Almacen", new { Area = "Admin"});
            }

            return View(almacen);
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            return Json(new { data = await unidadTrabajo.Almacen.ObtenerTodos() });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var obj = await unidadTrabajo.Almacen.Obtener(id);

            if (obj is null)
            {
                return Json(new { success = false, error = "Error al eliminar" });
            }

            unidadTrabajo.Almacen.Remover(obj);
            await unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Almacen eliminado correctamente" });
        }
        #endregion

    }
}
