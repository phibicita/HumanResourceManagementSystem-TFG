using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


namespace EmpresaProyecto
{
    public partial class ModuloSeguridad : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Administrador")
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                CargarEmpleados();
            }
        }
        private void CargarEmpleados()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["empresaBD"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT idEmpleado, CONCAT(Nombre, ' ', Primer_apellido, ' ', Segundo_apellido) AS NombreCompleto FROM Empleados";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                ddlEmpleados.DataSource = reader;
                ddlEmpleados.DataTextField = "NombreCompleto";
                ddlEmpleados.DataValueField = "idEmpleado";
                ddlEmpleados.DataBind();

                ddlEmpleados.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Seleccione un empleado", ""));
            }
        }

        protected void btnSalir_Click(object sender, EventArgs e)
        {
            
            Response.Redirect("MenuAdministrador.aspx");
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            if (ddlEmpleados.SelectedValue == "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Seleccione un empleado.');", true);
                return;
            }

            if (string.IsNullOrEmpty(txtUsuario.Text) || string.IsNullOrEmpty(txtContrasena.Text))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Complete el nombre de usuario y la contraseña.');", true);
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["empresaBD"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO usuario (idEmpleado, nombreUsuario, contrasena)
                                 VALUES (@idEmpleado, @usuario, @contrasena)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@idEmpleado", ddlEmpleados.SelectedValue);
                command.Parameters.AddWithValue("@usuario", txtUsuario.Text);
                command.Parameters.AddWithValue("@contrasena", txtContrasena.Text);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Usuario y contraseña asignados con éxito.');", true);

            txtUsuario.Text = "";
            txtContrasena.Text = "";
            ddlEmpleados.SelectedIndex = 0;
        }
    }
}
