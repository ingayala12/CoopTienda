using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoopTienda.Modelo.ViewsModels
{
    public class ProductoVM
    {
        public Producto Producto { get; set; }
        public IEnumerable<SelectListItem> ListaCategoria { get; set; }
        public IEnumerable<SelectListItem> ListaMarca { get; set; }
    }
}
