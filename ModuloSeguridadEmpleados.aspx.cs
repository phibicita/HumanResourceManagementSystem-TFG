using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;


namespace EmpresaProyecto
{
    public partial class ModuloSeguridadEmpleados : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Empleado")
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                
                if (Session["UserName"] != null)
                {
                    lblUsuario.Text = Session["UserName"].ToString();
                }
                else
                {
                    lblUsuario.Text = "Usuario no identificado";
                    lblMensaje.Text = "Hubo un problema al identificar al usuario. Por favor, inicie sesión nuevamente.";
                    lblMensaje.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string usuario = Session["UserName"]?.ToString();
            string contrasenaActual = txtContrasenaActual.Text.Trim();
            string contrasenaNueva = txtContrasenaNueva.Text.Trim();

            if (string.IsNullOrEmpty(contrasenaActual) || string.IsNullOrEmpty(contrasenaNueva))
            {
                lblMensaje.Text = "Debe ingresar todos los campos.";
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (!EsContrasenaValida(contrasenaNueva))
            {
                lblMensaje.Text = "La nueva contraseña debe tener máximo 15 caracteres, incluyendo números y letras, pero sin caracteres especiales.";
                lblMensaje.ForeColor = System.Drawing.Color.Red;
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["empresaBD"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string queryVerificar = "SELECT COUNT(*) FROM usuario WHERE nombreUsuario = @usuario AND contrasena = @contrasenaActual";
                SqlCommand commandVerificar = new SqlCommand(queryVerificar, connection);
                commandVerificar.Parameters.AddWithValue("@usuario", usuario);
                commandVerificar.Parameters.AddWithValue("@contrasenaActual", contrasenaActual);

                connection.Open();
                int existe = (int)commandVerificar.ExecuteScalar();

                if (existe == 0)
                {
                    lblMensaje.Text = "La contraseña actual es incorrecta.";
                    lblMensaje.ForeColor = System.Drawing.Color.Red;
                    return;
                }

               
                string queryActualizar = "UPDATE usuario SET contrasena = @contrasenaNueva WHERE nombreUsuario = @usuario";
                SqlCommand commandActualizar = new SqlCommand(queryActualizar, connection);
                commandActualizar.Parameters.AddWithValue("@contrasenaNueva", contrasenaNueva);
                commandActualizar.Parameters.AddWithValue("@usuario", usuario);

                int filasAfectadas = commandActualizar.ExecuteNonQuery();
                connection.Close();

                if (filasAfectadas > 0)
                {
                    lblMensaje.Text = "Contraseña actualizada correctamente.";
                    lblMensaje.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblMensaje.Text = "Error al actualizar la contraseña. Intente de nuevo.";
                    lblMensaje.ForeColor = System.Drawing.Color.Red;
                }
            }
        }

        protected void btnSalir_Click(object sender, EventArgs e)
        {
            Response.Redirect("MenuEmpleados.aspx");
        }

        private bool EsContrasenaValida(string contrasena)
        {
            
            string patron = @"^[a-zA-Z0-9]{1,15}$";
            return Regex.IsMatch(contrasena, patron);
        }
    }
}
