using CoopTienda.AccesoDatos.Data;
using CoopTienda.AccesoDatos.Repositorio.IRepositorio;
using CoopTienda.Modelo;
using CoopTienda.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CoopTienda.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize( Roles = DS.Role_Admin)]
    public class UsuarioController : Controller
    {
        private readonly IUnidadTrabajo unidadTrabajo;
        private readonly ApplicationDbContext db;

        public UsuarioController(IUnidadTrabajo unidadTrabajo, ApplicationDbContext db)
        {
            this.unidadTrabajo = unidadTrabajo;
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<UsuarioAplicacion> listaUsuario = await unidadTrabajo.UsuarioAplicacion.ObtenerTodos();
            return View(listaUsuario);
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var usuarioLista = await unidadTrabajo.UsuarioAplicacion.ObtenerTodos();
            var userRole = await db.UserRoles.ToListAsync();
            var roles = await db.Roles.ToListAsync();

            foreach (var usuario in usuarioLista)
            {
                var roleId = userRole.FirstOrDefault(x => x.UserId == usuario.Id).RoleId;
                usuario.Role = roles.FirstOrDefault(x => x.Id == roleId).Name;
            }
            return Json(new { data = usuarioLista });
        }

        [HttpPost]
        public async Task<IActionResult> BloquearDesbloquear([FromBody] string id)
        {
            var usuario = await unidadTrabajo.UsuarioAplicacion.ObtenerPrimero(x => x.Id == id);
            if (usuario is null)
            {
                return Json(new { success = false, message = "Error de Usuario" });
            }

            if (usuario.LockoutEnd is not null && usuario.LockoutEnd > DateTime.Now)
            {
                usuario.LockoutEnd = DateTime.Now;
            }
            else
            {
                usuario.LockoutEnd = DateTime.Now.AddYears(1);
            }

            await unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Operacion Exitosa" });
        }

        #endregion
    }
}
