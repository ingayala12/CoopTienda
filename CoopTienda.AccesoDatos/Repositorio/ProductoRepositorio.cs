using CoopTienda.AccesoDatos.Data;
using CoopTienda.AccesoDatos.Repositorio.IRepositorio;
using CoopTienda.Modelo;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoopTienda.AccesoDatos.Repositorio
{
    public class ProductoRepositorio : Repositorio<Producto>, IProductoRepositorio
    {
        private readonly ApplicationDbContext db;

        public ProductoRepositorio(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public async Task Actualizar(Producto producto)
        {
            var obj = await db.Producto.FirstOrDefaultAsync(x => x.Id == producto.Id);

            if (obj is not null)
            {
                obj.Codigo = producto.Codigo;
                obj.Serial = producto.Serial;
                obj.Nombre = producto.Nombre;
                obj.Descripcion = producto.Descripcion;
                obj.Precio = producto.Precio;
                obj.Costo = producto.Costo;
                obj.Estado = producto.Estado;
                obj.CategoriaId = producto.CategoriaId;
                obj.MarcaId = producto.MarcaId;

                // ✅ Solo actualiza la imagen si se pasó una nueva
                if (producto.ImagenUrl is not null)
                {
                    obj.ImagenUrl = producto.ImagenUrl;
                }
            }
        }


        public IEnumerable<SelectListItem> ObtenerDropDownList(string obj)
        {
            if (obj == "Categoria")
            {
                return db.Categorias.Select(x => new SelectListItem
                {
                    Text = x.Nombre,
                    Value = x.Id.ToString()
                });
            }

            if (obj == "Marca")
            {
                return db.Marca.Select(x => new SelectListItem
                {
                    Text = x.Nombre,
                    Value = x.Id.ToString()
                });
            }
            return null;
        }
    }
}
