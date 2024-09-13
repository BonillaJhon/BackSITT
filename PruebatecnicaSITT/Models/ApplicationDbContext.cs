using Microsoft.EntityFrameworkCore;
using PruebatecnicaSITT.Data;

namespace PruebatecnicaSITT.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Tarea> Tareas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de la relación Usuario-Tarea
            modelBuilder.Entity<Tarea>()
                .HasOne(t => t.Usuario) // Una tarea tiene un usuario
                .WithMany(u => u.Tareas) // Un usuario tiene muchas tareas
                .HasForeignKey(t => t.UserId) // La clave foránea es UserId en Tarea
                .OnDelete(DeleteBehavior.Cascade); // Si se elimina un usuario, también se eliminan sus tareas

            base.OnModelCreating(modelBuilder);
        }
    }
}
