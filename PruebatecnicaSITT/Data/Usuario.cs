using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace PruebatecnicaSITT.Data
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Contraseña { get; set; }

        public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
    }


}
