using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoopTienda.Modelo
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El codigo es obligatorio")]
        [Range(1,99999, ErrorMessage = "El codigo debe contener maximo 5 numeros")]
        public int Codigo { get; set; }

        [Required(ErrorMessage = "El numero de serie es obligatorio")]
        public string Serial { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [Display(Name = "Producto")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripcion es obligatoria")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(1,double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero 0")]
        public double Precio { get; set; }

        [Required(ErrorMessage = "El costo es obligatorio")]
        [Range(1, double.MaxValue, ErrorMessage = "El costo debe ser mayor a cero 0")]
        public double Costo { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(Name = "Imagen")]
        public string ImagenUrl { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        public bool Estado { get; set; }

        [Required(ErrorMessage = "La categoria es obligatoria")]
        [Display(Name = "Categoria")]
        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
        public Categoria Categoria { get; set; }

        [Required(ErrorMessage = "La marca es obligatoria")]
        [Display(Name = "Marca")]
        public int MarcaId { get; set; }

        [ForeignKey("MarcaId")]
        public Marca Marca { get; set; }
    }
}
