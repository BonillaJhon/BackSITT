using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PruebatecnicaSITT.Data;
using PruebatecnicaSITT.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PruebatecnicaSITT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/Auth/Register
        [HttpPost("registro")]
        public async Task<IActionResult> Registrar([FromBody] Usuario usuario)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email))
            {
                return BadRequest("El correo ya está registrado.");
            }

            // Hashear la contraseña antes de guardarla
            usuario.Contraseña = BCrypt.Net.BCrypt.HashPassword(usuario.Contraseña);

            // No se necesitan tareas al crear el usuario
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok("Usuario registrado con éxito.");
        }


        // POST: api/Auth/Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] login Usuario)
        {
            var existingUsuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == Usuario.Email);
            if (existingUsuario == null || !BCrypt.Net.BCrypt.Verify(Usuario.Contraseña, existingUsuario.Contraseña))
                return BadRequest("Correo o contraseña incorrectos");

            // Generar token JWT
            var token = GenerateJwtToken(existingUsuario);
            return Ok(new { token });
        }

        private string GenerateJwtToken(Usuario Usuario)
        {
            var claims = new[]
            {
             new Claim(JwtRegisteredClaimNames.Sub, Usuario.Id.ToString()),
             new Claim(JwtRegisteredClaimNames.Email, Usuario.Email)
         };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}