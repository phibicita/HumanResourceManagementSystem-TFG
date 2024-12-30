using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data.SqlClient;
using System.Configuration;


namespace EmpresaProyecto
{
    public partial class EvaluacionPersonal : System.Web.UI.Page
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

                ddlEmpleados.Items.Insert(0, new ListItem("Seleccione un empleado", "0")); 
                connection.Close();
            }
        }

        protected void btnEvaluarEmpleado_Click(object sender, EventArgs e)
        {
            int idEmpleadoSeleccionado = int.Parse(ddlEmpleados.SelectedValue);

            if (idEmpleadoSeleccionado == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Por favor, seleccione un empleado.');", true);
            }
            else
            {
                Session["EmpleadoSeleccionado"] = idEmpleadoSeleccionado;
                formEvaluacion.Visible = true;
            }
        }

        protected void btnGuardarEvaluacion_Click(object sender, EventArgs e)
        {
           
            if (string.IsNullOrEmpty(rblPuntualidad.SelectedValue) ||
                string.IsNullOrEmpty(rblResponsabilidad.SelectedValue) ||
                string.IsNullOrEmpty(rblTrabajoEquipo.SelectedValue) ||
                string.IsNullOrEmpty(rblDisposicionAprendizaje.SelectedValue) ||
                string.IsNullOrEmpty(rblManejoTiempo.SelectedValue) ||
                string.IsNullOrEmpty(rblComunicacion.SelectedValue) ||
                string.IsNullOrEmpty(rblResolucionProblemas.SelectedValue) ||
                string.IsNullOrEmpty(rblCompromiso.SelectedValue))
            {
                
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Debe responder todas las preguntas antes de guardar la evaluación.');", true);
                return; 
            }

            
            int idEmpleadoSeleccionado = (int)Session["EmpleadoSeleccionado"];
            DateTime fechaEvaluacion = DateTime.Now;

            string puntualidad = rblPuntualidad.SelectedValue;
            string responsabilidad = rblResponsabilidad.SelectedValue;
            string trabajoEquipo = rblTrabajoEquipo.SelectedValue;
            string disposicionAprendizaje = rblDisposicionAprendizaje.SelectedValue;
            string manejoTiempo = rblManejoTiempo.SelectedValue;
            string comunicacion = rblComunicacion.SelectedValue;
            string resolucionProblemas = rblResolucionProblemas.SelectedValue;
            string compromiso = rblCompromiso.SelectedValue;

            string connectionString = ConfigurationManager.ConnectionStrings["empresaBD"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Evaluacion 
                        (fecha_evaluacion, idEmpleado, puntualidad, responsabilidad, trabajo_equipo, 
                        disposicion_aprendizaje, manejo_tiempo, comunicacion, resolucion_problemas, compromiso) 
                        VALUES 
                        (@fecha_evaluacion, @idEmpleado, @puntualidad, @responsabilidad, @trabajo_equipo, 
                        @disposicion_aprendizaje, @manejo_tiempo, @comunicacion, @resolucion_problemas, @compromiso)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@fecha_evaluacion", fechaEvaluacion);
                    command.Parameters.AddWithValue("@idEmpleado", idEmpleadoSeleccionado);
                    command.Parameters.AddWithValue("@puntualidad", puntualidad);
                    command.Parameters.AddWithValue("@responsabilidad", responsabilidad);
                    command.Parameters.AddWithValue("@trabajo_equipo", trabajoEquipo);
                    command.Parameters.AddWithValue("@disposicion_aprendizaje", disposicionAprendizaje);
                    command.Parameters.AddWithValue("@manejo_tiempo", manejoTiempo);
                    command.Parameters.AddWithValue("@comunicacion", comunicacion);
                    command.Parameters.AddWithValue("@resolucion_problemas", resolucionProblemas);
                    command.Parameters.AddWithValue("@compromiso", compromiso);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Evaluación guardada exitosamente.');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Error al guardar la evaluación.');", true);
                    }
                }
            }
        }


        protected void btnSalir_Click(object sender, EventArgs e)
        {
            
            Response.Redirect("MenuAdministrador.aspx");
        }
    }
}
