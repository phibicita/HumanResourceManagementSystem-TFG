using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EmpresaProyecto
{
    public partial class MenuEmpleados : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
               
                if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Empleado")
                {
                    
                    Response.Redirect("Login.aspx");
                }
                else if (Session["UserID"] == null)
                {
                    
                    Response.Redirect("Login.aspx");
                }
                else
                {
                    
                    System.Diagnostics.Debug.WriteLine("ID de Usuario: " + Session["UserID"].ToString());
                }

                MostrarNotificaciones();

                
                MarcarNotificacionesComoLeidas();
            }
        }

        private void MostrarNotificaciones()
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

               
                string queryPermisos = "SELECT COUNT(*) FROM Permisos WHERE idEmpleado = @idEmpleado AND notificacion_respuesta = 1";
                SqlCommand commandPermisos = new SqlCommand(queryPermisos, connection);
                commandPermisos.Parameters.AddWithValue("@idEmpleado", Session["UserID"]);
                int notificacionesPermisos = Convert.ToInt32(commandPermisos.ExecuteScalar());

                
                string queryVacaciones = "SELECT COUNT(*) FROM vacaciones WHERE idEmpleado = @idEmpleado AND notificacion_respuesta = 1";
                SqlCommand commandVacaciones = new SqlCommand(queryVacaciones, connection);
                commandVacaciones.Parameters.AddWithValue("@idEmpleado", Session["UserID"]);
                int notificacionesVacaciones = Convert.ToInt32(commandVacaciones.ExecuteScalar());

               
                string queryHorasExtra = "SELECT COUNT(*) FROM horas_extra WHERE idEmpleado = @idEmpleado AND notificacion_respuesta = 1";
                SqlCommand commandHorasExtra = new SqlCommand(queryHorasExtra, connection);
                commandHorasExtra.Parameters.AddWithValue("@idEmpleado", Session["UserID"]);
                int notificacionesHorasExtra = Convert.ToInt32(commandHorasExtra.ExecuteScalar());

                connection.Close();

                
                if (notificacionesPermisos > 0)
                {
                    string mensaje = $"Tienes {notificacionesPermisos} solicitudes de permisos respondidas.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertPermisos",
                        $"setTimeout(function() {{ alert('{mensaje}'); }}, 500);", true);
                }

                if (notificacionesVacaciones > 0)
                {
                    string mensaje = $"Tienes {notificacionesVacaciones} solicitudes de vacaciones respondidas.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertVacaciones",
                        $"setTimeout(function() {{ alert('{mensaje}'); }}, 500);", true);
                }

                if (notificacionesHorasExtra > 0)
                {
                    string mensaje = $"Tienes {notificacionesHorasExtra} solicitudes de horas extra respondidas.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertHorasExtra",
                        $"setTimeout(function() {{ alert('{mensaje}'); }}, 500);", true);
                }
            }
        }

        private void MarcarNotificacionesComoLeidas()
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                
                string queryPermisos = "UPDATE Permisos SET notificacion_respuesta = 0 WHERE idEmpleado = @idEmpleado AND notificacion_respuesta = 1";
                SqlCommand commandPermisos = new SqlCommand(queryPermisos, connection);
                commandPermisos.Parameters.AddWithValue("@idEmpleado", Session["UserID"]);
                commandPermisos.ExecuteNonQuery();

                
                string queryVacaciones = "UPDATE Vacaciones SET notificacion_respuesta = 0 WHERE idEmpleado = @idEmpleado AND notificacion_respuesta = 1";
                SqlCommand commandVacaciones = new SqlCommand(queryVacaciones, connection);
                commandVacaciones.Parameters.AddWithValue("@idEmpleado", Session["UserID"]);
                commandVacaciones.ExecuteNonQuery();

                
                string queryHorasExtra = "UPDATE horas_extra SET notificacion_respuesta = 0 WHERE idEmpleado = @idEmpleado AND notificacion_respuesta = 1";
                SqlCommand commandHorasExtra = new SqlCommand(queryHorasExtra, connection);
                commandHorasExtra.Parameters.AddWithValue("@idEmpleado", Session["UserID"]);
                commandHorasExtra.ExecuteNonQuery();

                connection.Close();
            }
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            try
            {
                
                Session.Clear();

                
                Response.Redirect("Login.aspx", false);  
            }
            catch (Exception ex)
            {
                
                System.Diagnostics.Debug.WriteLine("Error al cerrar sesión: " + ex.Message);
            }
        }

        protected void btnRegistroAsistencia_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("~/RegistroAsistencia.aspx", false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en redirección a Registro Asistencia : " + ex.Message);
            }
        }

        
        protected void btnSolicitarHoras_Click(object sender, EventArgs e)
        {
            try
            {
               
                Response.Redirect("~/SolicitarHorasExtra.aspx", false);  
            }
            catch (Exception ex)
            {
                
                System.Diagnostics.Debug.WriteLine("Error en redirección a Solicitar Horas Extra: " + ex.Message);
            }
        }

        protected void btnEnviarIncapacidad_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("~/EnviarIncapacidad.aspx", false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en redirección a Enviar Incapacidad: " + ex.Message);
            }
        }

        protected void btnEnviarPermiso_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("~/SolicitarPermiso.aspx", false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en redirección a Solicitar permisos: " + ex.Message);
            }
        }

        protected void btnEmpleadosConsulta_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("~/EmpleadosConsulta.aspx", false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en redirección a Consultas y Reportes: " + ex.Message);
            }
        }
        protected void btnModuloSeguridadEmpleados_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Redirigiendo a ModuloSeguridadEmpleados.aspx");
            try
            {
                Response.Redirect("~/ModuloSeguridadEmpleados.aspx", false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en redirección: " + ex.Message);
            }
        }

        protected void btnSolicitarVacaciones_Click(object sender, EventArgs e)
        {
            try
            {
                
                Response.Redirect("~/SolicitudVacaciones.aspx", false);  
            }
            catch (Exception ex)
            {
               
                System.Diagnostics.Debug.WriteLine("Error en redirección a Solicitar Vacaciones: " + ex.Message);
            }
        }
    }
}
