using CoopTienda.AccesoDatos.Data;
using CoopTienda.AccesoDatos.Repositorio.IRepositorio;
using CoopTienda.Modelo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoopTienda.AccesoDatos.Repositorio
{
    public class AlmacenRepositorio : Repositorio<Almacen>, IAlmacenRepositorio
    {
        private readonly ApplicationDbContext db;

        public AlmacenRepositorio(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public async Task Actualizar(Almacen almacen)
        {
            var obj = await db.Almacen.FirstOrDefaultAsync(x => x.Id == almacen.Id);

            if (obj is not null)
            {
                obj.Nombre = almacen.Nombre;
                obj.Descripcion = almacen.Descripcion;
            }
        }
    }
}
