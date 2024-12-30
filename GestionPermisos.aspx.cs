using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;


namespace EmpresaProyecto
{
    public partial class GestionPermisos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Administrador")
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                CargarPermisos();
            }
        }
        private void ActualizarEstadoPermisos(string nuevoEstado)
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";
            bool solicitudSeleccionada = false; 

            foreach (RepeaterItem item in repeaterPermisos.Items)
            {
                
                CheckBox chkSelectPermiso = (CheckBox)item.FindControl("chkSelectPermiso");
                HiddenField hfIdPermiso = (HiddenField)item.FindControl("hfIdPermiso");

                
                if (chkSelectPermiso != null && chkSelectPermiso.Checked && hfIdPermiso != null)
                {
                    if (int.TryParse(hfIdPermiso.Value, out int idPermiso))
                    {
                        solicitudSeleccionada = true;

                        
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            try
                            {
                                string query = @"
                              UPDATE Permisos 
                              SET estado = @nuevoEstado, 
                              notificacion = 0, 
                              notificacion_respuesta = 1 
                              WHERE idPermiso = @idPermiso";

                                SqlCommand command = new SqlCommand(query, connection);
                                command.Parameters.AddWithValue("@nuevoEstado", nuevoEstado); 
                                command.Parameters.AddWithValue("@idPermiso", idPermiso);

                                connection.Open();
                                command.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                
                                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Error al actualizar el permiso: {ex.Message}');", true);
                                return;
                            }
                        }
                    }
                    else
                    {
                        
                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Error: ID del permiso inválido.');", true);
                        return;
                    }
                }
            }

            
            if (!solicitudSeleccionada)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Debe seleccionar una solicitud antes de proceder.');", true);
            }
            else
            {
                
                CargarPermisos();
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Estado de los permisos actualizado correctamente.');", true);
            }
        }

        private void CargarPermisos()
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT p.idPermiso, 
                                e.Nombre, 
                                e.Primer_apellido, 
                                e.Segundo_apellido, 
                                e.identificacion,
                                p.fecha_permiso AS FechaPermiso, 
                                p.hora_inicio AS HoraInicio, 
                                p.hora_fin AS HoraFin, 
                                p.estado AS EstadoPermiso,
                                tp.descripcion AS TipoPermiso
                         FROM Permisos p
                         JOIN Empleados e ON p.idEmpleado = e.idEmpleado
                         JOIN TipoPermiso tp ON p.idTipoPermiso = tp.idTipoPermiso";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                var permisos = new List<dynamic>();

                while (reader.Read())
                {
                    permisos.Add(new
                    {
                        idPermiso = reader["idPermiso"],
                        NombreCompleto = $"{reader["Nombre"]} {reader["Primer_apellido"]} {reader["Segundo_apellido"]}",
                        Identificacion = reader["identificacion"],
                        FechaPermiso = reader["FechaPermiso"],
                        HoraInicio = reader["HoraInicio"],
                        HoraFin = reader["HoraFin"],
                        EstadoPermiso = reader["EstadoPermiso"],
                        TipoPermiso = reader["TipoPermiso"]
                    });
                }

                repeaterPermisos.DataSource = permisos;
                repeaterPermisos.DataBind();
            }
        }


        protected void btnAceptarPermiso_Click(object sender, EventArgs e)
        {
            ActualizarEstadoPermisos("Aprobado");
        }

        protected void btnRechazarPermiso_Click(object sender, EventArgs e)
        {
            ActualizarEstadoPermisos("Rechazado");
        }

        

        protected void btnSalir_Click(object sender, EventArgs e)
        {
            
            Response.Redirect("MenuAdministrador.aspx");
        }
    }
}