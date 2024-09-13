using System.ComponentModel.DataAnnotations;

namespace PruebatecnicaSITT.Data
{
    public class login
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Contraseña { get; set; }
    }
}
