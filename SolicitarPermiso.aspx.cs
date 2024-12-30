using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace EmpresaProyecto
{
    public partial class SolicitarPermiso : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    int empleadoId = Convert.ToInt32(Session["UserID"]);
                    lblEmpleadoNombre.Text = ObtenerNombreEmpleado(empleadoId);
                    CargarPermisosEmpleado(empleadoId);
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }
        

        private void CargarPermisosEmpleado(int empleadoId)
        {
            string connectionString = "Data Source=phibi\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT 
                            p.idPermiso,
                            e.identificacion AS Identificacion,
                            (e.Nombre + ' ' + e.Primer_apellido + ' ' + e.Segundo_apellido) AS NombreCompleto,
                            tp.descripcion AS TipoPermiso,
                            p.fecha_permiso AS FechaPermiso,
                            p.hora_inicio AS HoraInicio,
                            p.hora_fin AS HoraFin,
                            p.estado AS Estado
                            FROM Permisos p
                            JOIN Empleados e ON p.idEmpleado = e.idEmpleado
                            JOIN TipoPermiso tp ON p.idTipoPermiso = tp.idTipoPermiso
                            WHERE p.idEmpleado = @idEmpleado";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@idEmpleado", empleadoId);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                gridPermisos.DataSource = reader;
                gridPermisos.DataBind();
            }
        }
         
        private bool EmpleadoEstaIncapacitado(int empleadoId)
        {
            string connectionString = "Data Source=phibi\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT COUNT(1) 
            FROM Estado est
            WHERE est.idEmpleado = @idEmpleado AND LTRIM(RTRIM(UPPER(est.estado))) = 'INCAPACITADO'";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@idEmpleado", empleadoId);

                connection.Open();
                int result = Convert.ToInt32(command.ExecuteScalar());
                return result > 0; 
            }
        }

        protected void btnSolicitarPermiso_Click(object sender, EventArgs e)
        {
           
            int empleadoId = Convert.ToInt32(Session["UserID"]);

            
            if (EmpleadoEstaIncapacitado(empleadoId))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('No puedes solicitar permisos porque estás en estado incapacitado.');", true);
                return;
            }

           
            DateTime fechaPermiso;
            TimeSpan horaInicio, horaFin;

            if (!DateTime.TryParse(txtFechaPermiso.Text, out fechaPermiso) ||
                !TimeSpan.TryParse(txtHoraInicio.Text, out horaInicio) ||
                !TimeSpan.TryParse(txtHoraFin.Text, out horaFin))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Por favor, ingrese valores válidos para la fecha y las horas.');", true);
                return;
            }

            DateTime fechaActual = DateTime.Now.Date;
            TimeSpan horaActual = DateTime.Now.TimeOfDay;

            
            if (fechaPermiso < fechaActual)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('No se puede solicitar un permiso para una fecha pasada.');", true);
                return;
            }

            
            if (fechaPermiso == fechaActual && horaInicio < horaActual)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('No se puede solicitar un permiso para una hora anterior a la actual.');", true);
                return;
            }

           
            if (horaFin <= horaInicio)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('La hora de fin debe ser mayor que la hora de inicio.');", true);
                return;
            }

            
            try
            {
                string connectionString = "Data Source=phibi\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Permisos (idEmpleado, idTipoPermiso, fecha_permiso, hora_inicio, hora_fin, estado)
                     VALUES (@idEmpleado, @idTipoPermiso, @fechaPermiso, @horaInicio, @horaFin, @estado)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@idEmpleado", empleadoId);
                    command.Parameters.AddWithValue("@idTipoPermiso", ddlTipoPermiso.SelectedValue);
                    command.Parameters.AddWithValue("@fechaPermiso", fechaPermiso);
                    command.Parameters.AddWithValue("@horaInicio", horaInicio);
                    command.Parameters.AddWithValue("@horaFin", horaFin);
                    command.Parameters.AddWithValue("@estado", "Pendiente");

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Permiso solicitado exitosamente.');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Error al registrar el permiso: {ex.Message}');", true);
            }

            
            CargarPermisosEmpleado(empleadoId);
        }

       
        protected void btnEliminarSeleccionados_Click(object sender, EventArgs e)
        {
            
            List<int> permisosSeleccionados = new List<int>();

            
            foreach (GridViewRow row in gridPermisos.Rows)
            {
                CheckBox chkSeleccionar = (CheckBox)row.FindControl("chkSeleccionar");
                if (chkSeleccionar != null && chkSeleccionar.Checked)
                {
                   
                    int permisoId = Convert.ToInt32(gridPermisos.DataKeys[row.RowIndex].Value);
                    permisosSeleccionados.Add(permisoId);
                }
            }

          
            if (permisosSeleccionados.Count > 0)
            {
                string connectionString = "Data Source=phibi\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    foreach (int permisoId in permisosSeleccionados)
                    {
                        string query = "DELETE FROM Permisos WHERE idPermiso = @idPermiso";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@idPermiso", permisoId);
                        command.ExecuteNonQuery();
                    }
                }

                
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Permisos eliminados exitosamente.');", true);

                
                int empleadoId = Convert.ToInt32(Session["UserID"]);
                CargarPermisosEmpleado(empleadoId);
            }
            else
            {
                
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Por favor, selecciona al menos un permiso para eliminar.');", true);
            }
        }

        private string ObtenerNombreEmpleado(int empleadoId)
        {
            string nombreCompleto = "";
            string connectionString = "Data Source=phibi\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";

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

        protected void gridPermisos_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
