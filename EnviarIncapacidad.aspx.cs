using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace EmpresaProyecto
{
    public partial class EnviarIncapacidad : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    int empleadoId = Convert.ToInt32(Session["UserID"]);
                    lblNombreEmpleado.Text = ObtenerNombreEmpleado(empleadoId);

                    
                    CargarTipoIncapacidad();
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }

        private void CargarTipoIncapacidad()
        {
           
            ddlTipoIncapacidad.Items.Clear();

            
            ddlTipoIncapacidad.Items.Add(new ListItem("Enfermedad", "1"));
            ddlTipoIncapacidad.Items.Add(new ListItem("Maternidad", "2"));
        }

        private string ObtenerNombreEmpleado(int empleadoId)
        {
            string nombreCompleto = "";
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Nombre, Primer_apellido, Segundo_apellido FROM Empleados WHERE idEmpleado = @empleadoId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@empleadoId", empleadoId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    nombreCompleto = $"{reader["Nombre"]} {reader["Primer_apellido"]} {reader["Segundo_apellido"]}";
                }
                connection.Close();
            }
            return nombreCompleto;
        }

        protected void btnEnviarIncapacidad_Click(object sender, EventArgs e)
        {
            try
            {
                if (fuArchivoIncapacidad.HasFile &&
                    (fuArchivoIncapacidad.PostedFile.ContentType == "application/pdf" ||
                     fuArchivoIncapacidad.PostedFile.ContentType == "image/jpeg" ||
                     fuArchivoIncapacidad.PostedFile.ContentType == "image/png" ||
                     fuArchivoIncapacidad.PostedFile.ContentType == "image/gif"))
                {
                    int empleadoId = Convert.ToInt32(Session["UserID"]); 
                    string tipoIncapacidad = ddlTipoIncapacidad.SelectedValue;
                    DateTime fechaInicio = Convert.ToDateTime(txtFechaInicio.Text);
                    DateTime fechaFin = Convert.ToDateTime(txtFechaFin.Text);

                   
                    string nombreArchivo = Path.GetFileName(fuArchivoIncapacidad.PostedFile.FileName);
                    string rutaArchivo = "~/Uploads/" + nombreArchivo;
                    fuArchivoIncapacidad.SaveAs(Server.MapPath(rutaArchivo));

                    string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string queryIncapacidad = @"
                    INSERT INTO Incapacidades (idEmpleado, fecha_inicio, fecha_fin, documento_incapacidad, idtipo_incapacidad, idestado_incapacidad)
                    VALUES (@idEmpleado, @fechaInicio, @fechaFin, @documentoIncapacidad, @tipoIncapacidad, @estadoIncapacidad)";

                        SqlCommand command = new SqlCommand(queryIncapacidad, connection);
                        command.Parameters.AddWithValue("@idEmpleado", empleadoId);
                        command.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                        command.Parameters.AddWithValue("@fechaFin", fechaFin);
                        command.Parameters.AddWithValue("@documentoIncapacidad", rutaArchivo);
                        command.Parameters.AddWithValue("@tipoIncapacidad", tipoIncapacidad);
                        command.Parameters.AddWithValue("@estadoIncapacidad", 6);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Incapacidad enviada exitosamente.');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Sube un archivo válido.');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
            }
        }

        protected void btnSalir_Click(object sender, EventArgs e)
        {
            if (Session["UserID"] != null)
            {
                Response.Redirect("MenuEmpleados.aspx");
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
    }
}
