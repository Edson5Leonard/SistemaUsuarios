using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SistemaUsuarios.Models;

namespace SistemaUsuarios.Controllers
{
    public class LoginController : Controller
    {
        UsuarioDAO dao = new UsuarioDAO();

        // Especificamos la ruta exacta a la carpeta 'Cuenta'
        public IActionResult Index() => View("~/Views/Cuenta/Login.cshtml");

        [HttpPost]
        public IActionResult Entrar(string email, string password)
        {
            var user = dao.Validar(email);
            if (user != null && user.PasswordHash == password)
            {
                return RedirectToAction("Dashboard");
            }
            ViewBag.Error = "Usuario o clave incorrecta";
            // Si falla el login, vuelve a cargar la vista desde la carpeta Cuenta
            return View("~/Views/Cuenta/Login.cshtml");
        }

        public IActionResult Registro() => View("~/Views/Cuenta/Registro.cshtml");

        [HttpPost]
        public IActionResult Registrarse(Usuario u)
        {
            if (dao.Registrar(u)) return RedirectToAction("Index");
            return View("~/Views/Cuenta/Registro.cshtml");
        }

        public IActionResult Olvide() => View("~/Views/Cuenta/OlviderPassword.cshtml");

        [HttpPost]
        public IActionResult Recuperar(string email)
        {
            string token = Guid.NewGuid().ToString().Substring(0, 8);
            dao.GuardarToken(email, token);
            ViewBag.Mensaje = "Token generado: " + token + " (Enviado al correo)";
            return View("~/Views/Cuenta/OlviderPassword.cshtml");
        }

        public IActionResult Dashboard()
        {
            //Obtenemos la lista de la base de datos 
            var listaUsuarios = dao.ListarTodos();
            // Se la pasamos a la vista
            return View("~/Views/Cuenta/Dashboard.cshtml", listaUsuarios);
        }


        //Acccion para eliminar 
        public IActionResult Eliminar(int id)
        {
            dao.Eliminar(id);
            return RedirectToAction("Dashboard");
        }

        

        //Accion para editar 
        public IActionResult Editar(int id)
        {
            // Necesitas un método en tu DAO que busque un usuario por ID
            var usuario = dao.ObtenerPorId(id);
            return View("~/Views/Cuenta/Editar.cshtml", usuario);
        }


        [HttpPost]
        public IActionResult Editar(Usuario u)
        {
            if (dao.Editar(u))
            {
                return RedirectToAction(nameof(Dashboard));
            }
            var lista = dao.ListarTodos(); 
           return View("~/Views/Cuenta/Dashboard.cshtml", lista);
        }

        // Muestra la pantalla para ingresar el token y la nueva clave
        public IActionResult ResetPassword() => View("~/Views/Cuenta/ResetPassword.cshtml");

        [HttpPost]
        public IActionResult ActualizarPassword(string email, string token, string nuevaPassword)
        {
            if (dao.RestablecerPassword(email, token, nuevaPassword))
            {
                ViewBag.Mensaje = "Contraseña actualizada con éxito. Ya puedes iniciar sesión.";
                return View("~/Views/Cuenta/Login.cshtml");
            }

            ViewBag.Error = "Token inválido, expirado o datos incorrectos.";
            return View("~/Views/Cuenta/ResetPassword.cshtml");
        }
    }
}