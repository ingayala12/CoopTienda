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
    public class MarcaRepositorio : Repositorio<Marca>, IMarcaRepositorio
    {
        private readonly ApplicationDbContext db;

        public MarcaRepositorio(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public async Task Actualizar(Marca marca)
        {
            var obj = await db.Marca.FirstOrDefaultAsync(x => x.Id == marca.Id);

            if (obj is not null)
            {
                obj.Nombre = marca.Nombre;
                obj.Estado = marca.Estado;
            }
        }
    }
}
