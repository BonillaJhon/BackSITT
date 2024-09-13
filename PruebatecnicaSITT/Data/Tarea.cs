using System.ComponentModel.DataAnnotations;

namespace PruebatecnicaSITT.Data
{
    public class Tarea
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public bool Completado { get; set; }

        [Required]
        // Relación con el usuario (no obligatorio en la creación de tareas)
        public int UserId { get; set; }

        // Este campo no debería tener la anotación [Required]
        public Usuario Usuario { get; set; }
    }


}
