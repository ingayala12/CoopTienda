using CoopTienda.AccesoDatos.Repositorio.IRepositorio;
using CoopTienda.Modelo;
using CoopTienda.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoopTienda.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin)]
    public class CategoriaController : Controller
    {
        private readonly IUnidadTrabajo unidadTrabajo;

        public CategoriaController(IUnidadTrabajo unidadTrabajo)
        {
            this.unidadTrabajo = unidadTrabajo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Categoria> listaCategoria = await unidadTrabajo.Categoria.ObtenerTodos();
            return View(listaCategoria);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(categoria.Nombre))
                {
                    ModelState.AddModelError("Nombre", "El nombre de la categoria es obligatoria");
                    return View(categoria);
                }

                var existe = await unidadTrabajo.Categoria.ObtenerPrimero(
                    x => x.Nombre.ToLower().Trim() == categoria.Nombre.ToLower().Trim()
                );

                if (existe is not null)
                {
                    ModelState.AddModelError("Nombre", "La categoria ya existe");
                    return View(categoria);
                }

                await unidadTrabajo.Categoria.Agregar(categoria);
                TempData[DS.Exitoso] = "Categoria Agregada Correctamente";
                await unidadTrabajo.Guardar();

                return RedirectToAction(nameof(Index), "Categoria", new { Area = "Admin"});
            }

            return View(categoria);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var existe = await unidadTrabajo.Categoria.Obtener(id);

            if (existe is null)
            {
                return NotFound();
            }
            return View(existe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                // Verificar si existe otro almacén con el mismo nombre
                var existe = await unidadTrabajo.Categoria.ObtenerPrimero(
                    x => x.Nombre.ToLower().Trim() == categoria.Nombre.ToLower().Trim()
                      && x.Id != categoria.Id
                );

                if (existe is not null)
                {
                    ModelState.AddModelError("Nombre", "Ya existe una categoria con este nombre");
                    return View(categoria);
                }

                // Actualizar el almacén
                await unidadTrabajo.Categoria.Actualizar(categoria);
                await unidadTrabajo.Guardar();

                TempData["Exitoso"] = "Categoria actualizada correctamente";

                return RedirectToAction("Index", "Categoria", new { Area = "Admin"});
            }

            return View(categoria);
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            return Json(new { data = await unidadTrabajo.Categoria.ObtenerTodos() });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var obj = await unidadTrabajo.Categoria.Obtener(id);

            if (obj is null)
            {
                return Json(new { success = false, error = "Error al eliminar" });
            }

            unidadTrabajo.Categoria.Remover(obj);
            await unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Categoria eliminada correctamente" });
        }
        #endregion

    }
}
