using CoopTienda.AccesoDatos.Repositorio.IRepositorio;
using CoopTienda.Modelo;
using CoopTienda.Modelo.ViewsModels;
using CoopTienda.Utilidades;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CoopTienda.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductoController : Controller
    {
        private readonly IUnidadTrabajo unidadTrabajo;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductoController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment webHostEnvironment)
        {
            this.unidadTrabajo = unidadTrabajo;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<Producto> listaProducto = await unidadTrabajo.Producto.ObtenerTodos();
            return View(listaProducto);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            ProductoVM productoVM = new ProductoVM()
            {
                Producto = new Producto()
                {
                    Estado = true
                },
                ListaCategoria = unidadTrabajo.Producto.ObtenerDropDownList("Categoria"),
                ListaMarca = unidadTrabajo.Producto.ObtenerDropDownList("Marca")
            };
            return View(productoVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(ProductoVM productoVM)
        {
            if (ModelState.IsValid)
            {
                // Verificar si ya existe un serial igual (ignora mayúsculas/minúsculas y espacios)
                var serialExiste = await unidadTrabajo.Producto.ObtenerPrimero(x => x.Serial.ToLower().Trim() == productoVM.Producto.Serial.ToLower().Trim());

                if (serialExiste is not null)
                {
                    // Ya existe un serial igual → mostrar mensaje de error
                    ModelState.AddModelError("Producto.Serial", "Ya existe un producto con en mismo serial.");
                    productoVM.ListaCategoria = unidadTrabajo.Producto.ObtenerDropDownList("Categoria");
                    productoVM.ListaMarca = unidadTrabajo.Producto.ObtenerDropDownList("Marca");
                    return View(productoVM);
                }

                
                var descripcionExiste = await unidadTrabajo.Producto.ObtenerPrimero(x => x.Descripcion.ToLower().Trim() == productoVM.Producto.Descripcion.ToLower().Trim());

                if (descripcionExiste is not null)
                {
                    
                    ModelState.AddModelError("Producto.Descripcion", "Ya existe un producto con la misma descripción.");
                    productoVM.ListaCategoria = unidadTrabajo.Producto.ObtenerDropDownList("Categoria");
                    productoVM.ListaMarca = unidadTrabajo.Producto.ObtenerDropDownList("Marca");
                    return View(productoVM);
                }

                string rutaPrincipal = webHostEnvironment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                if (productoVM.Producto.Id == 0)
                {
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subida = Path.Combine(rutaPrincipal, @"imagenes\producto");
                    var extencion = Path.GetExtension(archivos[0].FileName);

                    using (var fileStrems = new FileStream(Path.Combine(subida, nombreArchivo + extencion), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStrems);
                    }

                    productoVM.Producto.ImagenUrl = @"\imagenes\producto\" + nombreArchivo + extencion;

                    await unidadTrabajo.Producto.Agregar(productoVM.Producto);
                    TempData[DS.Exitoso] = "Producto Agregado Correctamente";
                    await unidadTrabajo.Guardar();
                    return RedirectToAction("Index", "Producto", new { Area = "Admin"});
                }
            }
            productoVM.ListaCategoria = unidadTrabajo.Producto.ObtenerDropDownList("Categoria");
            productoVM.ListaMarca = unidadTrabajo.Producto.ObtenerDropDownList("Marca");

            return View(productoVM);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            ProductoVM productoVM = new ProductoVM()
            {
                Producto = new Producto(),
                ListaCategoria = unidadTrabajo.Producto.ObtenerDropDownList("Categoria"),
                ListaMarca = unidadTrabajo.Producto.ObtenerDropDownList("Marca")
            };

            if (id == 0 || productoVM.Producto is null)
            {
                return NotFound();
            }

            productoVM.Producto = await unidadTrabajo.Producto.Obtener(id);

            if (productoVM.Producto is null)
            {
                return NotFound();
            }

            return View(productoVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(ProductoVM productoVM)
        {
            if (ModelState.IsValid)
            {
                // 🔍 Validar duplicados
                var codigoExiste = await unidadTrabajo.Producto.ObtenerPrimero(
                    x => x.Codigo == productoVM.Producto.Codigo && x.Id != productoVM.Producto.Id);

                if (codigoExiste is not null)
                {
                    ModelState.AddModelError("Producto.Codigo", "Ya existe un producto con este código.");
                    CargarListas(productoVM);
                    return View(productoVM);
                }

                var serialExiste = await unidadTrabajo.Producto.ObtenerPrimero(
                    x => (x.Serial ?? "").ToLower().Trim() == (productoVM.Producto.Serial ?? "").ToLower().Trim() && x.Id != productoVM.Producto.Id);

                if (serialExiste is not null)
                {
                    ModelState.AddModelError("Producto.Serial", "Ya existe un producto con este serial.");
                    CargarListas(productoVM);
                    return View(productoVM);
                }

                var descripcionExiste = await unidadTrabajo.Producto.ObtenerPrimero(
                    x => (x.Descripcion ?? "").ToLower().Trim() == (productoVM.Producto.Descripcion ?? "").ToLower().Trim() && x.Id != productoVM.Producto.Id);

                if (descripcionExiste is not null)
                {
                    ModelState.AddModelError("Producto.Descripcion", "Ya existe un producto con esta descripción.");
                    CargarListas(productoVM);
                    return View(productoVM);
                }

                // 🔄 Obtener el producto original desde la base de datos
                var productoDesdeDB = await unidadTrabajo.Producto.Obtener(productoVM.Producto.Id);
                if (productoDesdeDB == null)
                {
                    return NotFound();
                }

                string rutaPrincipal = webHostEnvironment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                // ✅ Si se sube una nueva imagen
                if (archivos.Count > 0)
                {
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subida = Path.Combine(rutaPrincipal, @"imagenes\producto");
                    var extension = Path.GetExtension(archivos[0].FileName);

                    // 🧹 Eliminar imagen anterior
                    var imagenDB = Path.Combine(rutaPrincipal, productoDesdeDB.ImagenUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(imagenDB))
                    {
                        System.IO.File.Delete(imagenDB);
                    }

                    // 💾 Guardar la nueva imagen
                    using (var fileStreams = new FileStream(Path.Combine(subida, nombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams);
                    }

                    productoVM.Producto.ImagenUrl = @"\imagenes\producto\" + nombreArchivo + extension;
                }
                else
                {
                    // Si no se sube imagen, conservar la existente
                    productoVM.Producto.ImagenUrl = productoDesdeDB.ImagenUrl;
                }

                await unidadTrabajo.Producto.Actualizar(productoVM.Producto);
                TempData[DS.Exitoso] = "Producto Acualizado Correctamente";
                await unidadTrabajo.Guardar();

                return RedirectToAction("Index", "Producto", new { Area = "Admin" });
            }

            // Si el modelo no es válido
            CargarListas(productoVM);
            return View(productoVM);
        }

        private void CargarListas(ProductoVM productoVM)
        {
            productoVM.ListaCategoria = unidadTrabajo.Producto.ObtenerDropDownList("Categoria");
            productoVM.ListaMarca = unidadTrabajo.Producto.ObtenerDropDownList("Marca");
        }
        #region API

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await unidadTrabajo.Producto.ObtenerTodos(incluirPropiedades:"Categoria,Marca");
            return Json(new { data = todos });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            string rutaPrincipal = webHostEnvironment.WebRootPath;
            var productoDesdeDB = await unidadTrabajo.Producto.Obtener(id);

            if (productoDesdeDB is null || id == 0)
            {
                return Json(new { success = false, message = "Error al eliminar el producto" });
            }

            var imagenDb = Path.Combine(rutaPrincipal, productoDesdeDB.ImagenUrl.TrimStart('\\'));

            if (System.IO.File.Exists(imagenDb))
            {
                System.IO.File.Delete(imagenDb);
            }

            unidadTrabajo.Producto.Remover(productoDesdeDB);
            await unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Producto eliminado correctamente"});
        }
        #endregion
    }
}
