using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;



namespace EmpresaProyecto
{
    public partial class MenuAdministrador : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Administrador")
                {
                    Response.Redirect("Login.aspx");
                }

                
                MostrarNotificacionesHorasExtra();
                MostrarNotificacionesVacaciones();
                MostrarNotificacionesPermisos();
                MostrarNotificacionesIncapacidades();
            }
        }

        private void MostrarNotificacionesHorasExtra()
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM dbo.horas_extra WHERE notificacion = 1";
                SqlCommand command = new SqlCommand(query, connection);

                int notificacionesPendientes = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();

                if (notificacionesPendientes > 0)
                {
                    string mensaje = $"Tienes {notificacionesPendientes} solicitudes de horas extra pendientes.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertHorasExtra",
                        $"setTimeout(function() {{ alert('{mensaje}'); }}, 500);", true);
                }
            }
        }

        private void MostrarNotificacionesPermisos()
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

               
                string query = "SELECT COUNT(*) FROM dbo.Permisos WHERE notificacion = 1 AND estado = 'Pendiente'"; 
                SqlCommand command = new SqlCommand(query, connection);

                int notificacionesPendientes = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();

                if (notificacionesPendientes > 0)
                {
                    
                    string mensaje = $"Tienes {notificacionesPendientes} solicitudes de permisos pendientes.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertPermisos",
                        $"setTimeout(function() {{ alert('{mensaje}'); }}, 500);", true);
                }
            }
        }

        private void MostrarNotificacionesIncapacidades()
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM Incapacidades WHERE notificacion = 1 AND idestado_incapacidad = 6"; // 6 pendiente en bd
                SqlCommand command = new SqlCommand(query, connection);

                int notificacionesPendientes = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();

                if (notificacionesPendientes > 0)
                {
                    string mensaje = $"Hay {notificacionesPendientes} incapacidades pendientes.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertIncapacidades",
                        $"setTimeout(function() {{ alert('{mensaje}'); }}, 500);", true);
                }
            }
        }

        private void MostrarNotificacionesVacaciones()
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM dbo.Vacaciones WHERE notificacion = 1 AND idEstadoVacaciones = 1";
                SqlCommand command = new SqlCommand(query, connection);

                int notificacionesPendientes = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();

                if (notificacionesPendientes > 0)
                {
                    string mensaje = $"Tienes {notificacionesPendientes} solicitudes de vacaciones pendientes.";
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertVacaciones",
                        $"setTimeout(function() {{ alert('{mensaje}'); }}, 500);", true);
                }
            }
        }
        public void MarcarNotificacionesVacacionesComoAtendidas()
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE dbo.Vacaciones SET notificacion = 0 WHERE notificacion = 1";
                SqlCommand command = new SqlCommand(query, connection);

                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        public void MarcarNotificacionesPermisosComoAtendidas()
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                
                string query = "UPDATE dbo.Permisos SET notificacion = 0 WHERE notificacion = 1";
                SqlCommand command = new SqlCommand(query, connection);

                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        private void MarcarNotificacionesIncapacidadesComoAtendidas()
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE Incapacidades SET notificacion = 0 WHERE notificacion = 1";
                SqlCommand command = new SqlCommand(query, connection);

                command.ExecuteNonQuery();
                connection.Close();
            }
        }


        public void MarcarNotificacionesComoAtendidas()
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE dbo.horas_extra SET notificacion = 0 WHERE notificacion = 1";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        
        protected void AtenderSolicitudes()
        {
            
            MarcarNotificacionesComoAtendidas();

            
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Solicitudes atendidas correctamente.');", true);
        }


        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
           
            Session.Clear();

            
            Response.Redirect("Login.aspx");
        }

        protected void btnGestionHorarios_Click(object sender, EventArgs e)
        {
            Response.Redirect("CrearHorarios.aspx");
        }
        protected void btnGestionNomina_Click(object sender, EventArgs e)
        {
            Response.Redirect("GestionNomina.aspx");
        }
        protected void btnGestionLiquidaciones_Click(object sender, EventArgs e)
        {
            
            Response.Redirect("Liquidacion.aspx");
        }

        protected void btnCalculoAguinaldo_Click(object sender, EventArgs e)
        {

            Response.Redirect("Aguinaldo.aspx");
        }

        protected void btnEvaluacionPersonal_Click(object sender, EventArgs e)
        {
            
            Response.Redirect("EvaluacionPersonal.aspx");
        }

        protected void btnGestionPermisos_Click(object sender, EventArgs e)
        {
            Response.Redirect("GestionPermisos.aspx");
        }
        protected void btnGestionIncapacidades_Click(object sender, EventArgs e)
        {
            
            Response.Redirect("GestionIncapacidades.aspx");
        }

        protected void btnGestionVacaciones_Click(object sender, EventArgs e)
        {
            
            Response.Redirect("GestionVacaciones.aspx");
        }
       
        protected void btnGestionHorasExtra_Click(object sender, EventArgs e)
        {
         
            Response.Redirect("GestionHorasExtra.aspx");
        }

        protected void btnModuloSeguridad_Click(object sender, EventArgs e)
        {
            
            Response.Redirect("ModuloSeguridad.aspx");
        }

        protected void btnGestionAsistencia_Click(object sender, EventArgs e)
        {
            
            Response.Redirect("GestionAsistencia.aspx");
        }
        protected void btnConsultasAdmin_Click(object sender, EventArgs e)
        {
            Response.Redirect("ConsultasAdmin.aspx");
        }

    }
}
