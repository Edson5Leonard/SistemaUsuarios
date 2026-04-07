using Microsoft.Data.SqlClient;
using System.Data;

namespace SistemaUsuarios.Models
{
    public class UsuarioDAO
    {
        string cadena = "Server=8CGK1P111102A05;Database=SistemaUsuarios;Trusted_Connection=True;TrustServerCertificate=True;";

        public bool Registrar(Usuario u)
        {
            using (Microsoft.Data.SqlClient.SqlConnection cn = new Microsoft.Data.SqlClient.SqlConnection(cadena))
            {
                string sql = "INSERT INTO Usuarios(Nombre,Email,PasswordHash) VALUES(@n,@e,@p)";
                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@n", u.Nombre);
                cmd.Parameters.AddWithValue("@e", u.Email);
                cmd.Parameters.AddWithValue("@p", u.PasswordHash);
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public Usuario Validar(string email)
        {
            using (Microsoft.Data.SqlClient.SqlConnection cn = new Microsoft.Data.SqlClient.SqlConnection(cadena))
            {
                string sql = "SELECT * FROM Usuarios WHERE Email=@e";
                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@e", email);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return new Usuario
                    {
                        Id = (int)dr["Id"],
                        // .Trim() es vital porque usas CHAR en la BD
                        Nombre = dr["Nombre"].ToString().Trim(),
                        Email = dr["Email"].ToString().Trim(),
                        PasswordHash = dr["PasswordHash"].ToString().Trim()
                    };
                }
            }
            return null;
        }

        public void GuardarToken(string email, string token)
        {
            using (Microsoft.Data.SqlClient.SqlConnection cn = new Microsoft.Data.SqlClient.SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("GuardarTokenRecuperacion", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Token", token);
                cmd.Parameters.AddWithValue("@Expiracion", DateTime.Now.AddHours(1));
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public bool RestablecerPassword(string email, string token, string nuevaPassword)
        {
            using (Microsoft.Data.SqlClient.SqlConnection cn = new Microsoft.Data.SqlClient.SqlConnection(cadena))
            {
                string sql = @"UPDATE Usuarios
                               SET PasswordHash = @p, ResetToken = NULL, TokenExpiracion = NULL
                               WHERE Email = @e
                               AND RTRIM(ResetToken) = @t
                               AND TokenExpiracion > GETDATE()";
                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@p", nuevaPassword);
                cmd.Parameters.AddWithValue("@e", email);
                cmd.Parameters.AddWithValue("@t", token.Trim());

                cn.Open();
                int filasAfectadas = cmd.ExecuteNonQuery();

                // Si filasAfectadas es > 0, significa que el token era válido y se cambió la clave
                return filasAfectadas > 0;
            }
        }

       
    }
}