using CoopTienda.Modelo;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CoopTienda.AccesoDatos.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Almacen> Almacen { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Marca> Marca { get; set; }
        public DbSet<Producto> Producto { get; set; }
        public DbSet<UsuarioAplicacion> UsuarioAplicacion { get; set; }
    }
}
