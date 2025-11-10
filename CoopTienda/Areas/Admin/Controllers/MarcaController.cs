using CoopTienda.AccesoDatos.Repositorio.IRepositorio;
using CoopTienda.Modelo;
using CoopTienda.Utilidades;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoopTienda.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MarcaController : Controller
    {
        private readonly IUnidadTrabajo unidadTrabajo;

        public MarcaController(IUnidadTrabajo unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Marca> listamarca = await unidadTrabajo.Marca.ObtenerTodos();
            return View(listamarca);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Marca marca)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(marca.Nombre))
                {
                    ModelState.AddModelError("Nombre", "El nombre de la marca es obligatoria");
                    return View(marca);
                }

                var existe = await unidadTrabajo.Marca.ObtenerPrimero(
                    x => x.Nombre.ToLower().Trim() == marca.Nombre.ToLower().Trim()
                );

                if (existe is not null)
                {
                    ModelState.AddModelError("Nombre", "La marca ya existe");
                    return View(marca);
                }

                await unidadTrabajo.Marca.Agregar(marca);
                TempData[DS.Exitoso] = "Marca Agregada Correctamente";
                await unidadTrabajo.Guardar();

                return RedirectToAction(nameof(Index), "Marca", new { Area = "Admin"});
            }

            return View(marca);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var existe = await unidadTrabajo.Marca.Obtener(id);

            if (existe is null)
            {
                return NotFound();
            }
            return View(existe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Marca marca)
        {
            if (ModelState.IsValid)
            {
                // Verificar si existe otro almacén con el mismo nombre
                var existe = await unidadTrabajo.Marca.ObtenerPrimero(
                    x => x.Nombre.ToLower().Trim() == marca.Nombre.ToLower().Trim()
                      && x.Id != marca.Id
                );

                if (existe is not null)
                {
                    ModelState.AddModelError("Nombre", "Ya existe una marca con este nombre");
                    return View(marca);
                }

                // Actualizar el almacén
                await unidadTrabajo.Marca.Actualizar(marca);
                await unidadTrabajo.Guardar();

                TempData["Exitoso"] = "Marca actualizada correctamente";

                return RedirectToAction("Index", "Marca", new { Area = "Admin"});
            }

            return View(marca);
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            return Json(new { data = await unidadTrabajo.Marca.ObtenerTodos() });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var obj = await unidadTrabajo.Marca.Obtener(id);

            if (obj is null)
            {
                return Json(new { success = false, error = "Error al eliminar" });
            }

            unidadTrabajo.Marca.Remover(obj);
            await unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Marca eliminada correctamente" });
        }
        #endregion

    }
}
