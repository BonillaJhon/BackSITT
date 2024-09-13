using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebatecnicaSITT.Data;
using PruebatecnicaSITT.Models;
using System.IdentityModel.Tokens.Jwt;

namespace PruebatecnicaSITT.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TareasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TareasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obtener todas las tareas del usuario autenticado
        [HttpGet("get-tareas")]
        public async Task<IActionResult> GetTareas()
        {
            var tareas = await _context.Tareas.ToListAsync();

            if (tareas == null || tareas.Count == 0)
            {
                return NotFound("No se encontraron tareas.");
            }

            return Ok(tareas);
        }


        // Crear nueva tarea
        [HttpPost("crear-tarea")]
        public async Task<IActionResult> CrearTarea([FromBody] Tarea tarea)
        {
            if (tarea == null)
            {
                return BadRequest("La tarea no puede ser nula.");
            }

            // Verificar que el usuario existe en la base de datos sin rastreo para evitar conflictos
            var usuario = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == tarea.UserId);
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            // Asignar el UserId a la tarea, no es necesario adjuntar el objeto Usuario
            tarea.Usuario = null;

            // Agregar la tarea a la base de datos sin intentar rastrear el usuario
            _context.Tareas.Add(tarea);
            await _context.SaveChangesAsync();

            return Ok(tarea);
        }




        // Actualizar tarea
        [HttpPut("actualizar-tarea/{id}")]
        public async Task<IActionResult> ActualizarTarea(int id, [FromBody] Tarea tareaActualizada)
        {
            // Verifica si el ID de la tarea coincide
            if (id != tareaActualizada.Id)
            {
                return BadRequest("El ID de la tarea no coincide.");
            }

            // Intentar obtener la tarea desde la base de datos
            var tareaExistente = await _context.Tareas.FindAsync(id);

            if (tareaExistente == null)
            {
                return NotFound("No se encontró la tarea.");
            }

            // Actualizar las propiedades de la tarea existente con los nuevos datos
            tareaExistente.Nombre = tareaActualizada.Nombre;
            tareaExistente.Descripcion = tareaActualizada.Descripcion;
            tareaExistente.Completado = tareaActualizada.Completado;

            try
            {
                // Intentar guardar los cambios en la base de datos
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Verificar si la tarea sigue existiendo en la base de datos
                if (!TareaExiste(id))
                {
                    return NotFound("No se pudo actualizar, tarea no encontrada.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Método auxiliar para verificar si la tarea existe
        private bool TareaExiste(int id)
        {
            return _context.Tareas.Any(t => t.Id == id);
        }


        // Eliminar tarea
        [HttpDelete("eliminar-tarea/{id}")]
        public async Task<IActionResult> EliminarTarea(int id)
        {
            // Intentar obtener la tarea desde la base de datos
            var tarea = await _context.Tareas.FindAsync(id);

            if (tarea == null)
            {
                return NotFound("No se encontró la tarea.");
            }

            // Si la tarea existe, proceder a eliminarla
            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();

            return Ok("Tarea eliminada exitosamente.");
        }

    }
}