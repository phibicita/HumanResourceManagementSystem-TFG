using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;

namespace EmpresaProyecto
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserRole"] != null)
            {
                string userRole = Session["UserRole"].ToString();
                RedirectBasedOnRole(userRole);
            }
        }

        protected void btnIniciarSesion_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text;
            string contrasena = txtContrasena.Text;

            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"SELECT u.idEmpleado, e.idtipo_empleado, u.nombreUsuario 
                                     FROM Usuario u 
                                     INNER JOIN Empleados e ON u.idEmpleado = e.idEmpleado 
                                     WHERE u.nombreUsuario = @usuario 
                                     AND u.contrasena COLLATE SQL_Latin1_General_CP1_CS_AS = @contrasena";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@contrasena", contrasena);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        int tipoEmpleado = Convert.ToInt32(reader["idtipo_empleado"]);
                        int idEmpleado = Convert.ToInt32(reader["idEmpleado"]);
                        string nombreUsuario = reader["nombreUsuario"].ToString();

                        Session["UserID"] = idEmpleado;
                        Session["UserRole"] = tipoEmpleado == 1 ? "Administrador" : "Empleado";
                        Session["UserName"] = nombreUsuario;

                        if (tipoEmpleado == 1)
                        {
                            Response.Redirect("MenuAdministrador.aspx");
                        }
                        else if (tipoEmpleado == 2)
                        {
                            Response.Redirect("MenuEmpleados.aspx");
                        }
                    }
                    else
                    {
                        lblError.Text = "Usuario o contraseña incorrectos.";
                    }
                }
                catch (Exception ex)
                {
                    lblError.Text = "Error de conexión: " + ex.Message;
                }
            }
        }


        protected void lnkOlvideContrasena_Click(object sender, EventArgs e)
        {
            Response.Redirect("ResetPassword.aspx");
        }

        private void RedirectBasedOnRole(string role)
        {
            if (role == "Administrador")
            {
                Response.Redirect("MenuAdministrador.aspx");
            }
            else if (role == "Empleado")
            {
                Response.Redirect("MenuEmpleados.aspx");
            }
        }
    }
}