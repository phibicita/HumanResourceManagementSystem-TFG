using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;


namespace EmpresaProyecto
{
    public partial class GestionVacaciones : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Administrador")
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                CargarVacaciones();
            }
        }

        private List<DateTime> ObtenerDiasFeriados(DateTime fechaInicio, DateTime fechaFin)
        {
            List<DateTime> diasFeriados = new List<DateTime>();

            
            for (int anio = fechaInicio.Year; anio <= fechaFin.Year; anio++)
            {
                diasFeriados.AddRange(new List<DateTime>
        {
            new DateTime(anio, 1, 1),  // año nuevo
            new DateTime(anio, 4, 11), // día de Juan Santamaría
            new DateTime(anio, 5, 1),  // día del trabajador
            new DateTime(anio, 7, 25), // anexión de Guanacaste
            new DateTime(anio, 8, 2),  // Virgen de los Ángeles
            new DateTime(anio, 8, 15), // día de la Madre
            new DateTime(anio, 9, 15), // día de la Independencia
            new DateTime(anio, 12, 25) // navidad
        });
            }

            return diasFeriados;
        }


        private void CargarVacaciones()
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                
                int filtroEstado = int.Parse(ddlFiltroEstado.SelectedValue);

                
                string query = @"
            SELECT v.idvacaciones, e.Nombre, e.Primer_apellido, e.Segundo_apellido, e.identificacion,
                   v.fecha_inicio AS FechaInicio, v.fecha_fin AS FechaFin, ev.descripcion AS EstadoVacacion
            FROM vacaciones v
            JOIN Empleados e ON v.idEmpleado = e.idEmpleado
            JOIN estado_vacaciones ev ON v.idEstadoVacaciones = ev.idEstadoVacaciones
            WHERE v.fecha_inicio IS NOT NULL AND v.fecha_fin IS NOT NULL";

                
                if (filtroEstado != 0) 
                {
                    query += " AND v.idEstadoVacaciones = @filtroEstado";
                }

                SqlCommand command = new SqlCommand(query, connection);

               
                if (filtroEstado != 0)
                {
                    command.Parameters.AddWithValue("@filtroEstado", filtroEstado);
                }

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                var vacaciones = new List<dynamic>();

                while (reader.Read())
                {
                    vacaciones.Add(new
                    {
                        idvacaciones = reader["idvacaciones"],
                        NombreCompleto = $"{reader["Nombre"]} {reader["Primer_apellido"]} {reader["Segundo_apellido"]}",
                        Identificacion = reader["identificacion"],
                        FechaInicio = reader["FechaInicio"],
                        FechaFin = reader["FechaFin"],
                        EstadoVacacion = reader["EstadoVacacion"]
                    });
                }

                repeaterVacaciones.DataSource = vacaciones;
                repeaterVacaciones.DataBind();
            }
        }


        protected void btnAceptarSolicitud_Click(object sender, EventArgs e)
        {
            ActualizarEstadoVacaciones(2); // Aprobado
        }

        protected void btnRechazarSolicitud_Click(object sender, EventArgs e)
        {
            ActualizarEstadoVacaciones(3); // Rechazado
        }
        private void MarcarNotificacionesVacacionesComoAtendidas(int idVacacion)
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Vacaciones SET notificacion = 0 WHERE idvacaciones = @idVacacion";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@idVacacion", idVacacion);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private void ActualizarEstadoVacaciones(int nuevoEstado)
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";
            bool solicitudSeleccionada = false;

            foreach (RepeaterItem item in repeaterVacaciones.Items)
            {
                CheckBox chkSelectVacacion = (CheckBox)item.FindControl("chkSelectVacacion");
                HiddenField hfIdVacacion = (HiddenField)item.FindControl("hfIdVacacion");

                if (chkSelectVacacion != null && chkSelectVacacion.Checked && hfIdVacacion != null)
                {
                    if (int.TryParse(hfIdVacacion.Value, out int idVacacion))
                    {
                        solicitudSeleccionada = true;

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            
                            string queryActualizarEstado = @"
                        UPDATE Vacaciones 
                        SET idEstadoVacaciones = @nuevoEstado, 
                            notificacion = 0, 
                            notificacion_respuesta = 1 
                        WHERE idvacaciones = @idVacacion";

                            SqlCommand commandActualizar = new SqlCommand(queryActualizarEstado, connection);
                            commandActualizar.Parameters.AddWithValue("@nuevoEstado", nuevoEstado);
                            commandActualizar.Parameters.AddWithValue("@idVacacion", idVacacion);
                            commandActualizar.ExecuteNonQuery();

                           
                            if (nuevoEstado == 2) 
                            {
                               
                                string queryObtenerFechas = "SELECT fecha_inicio, fecha_fin FROM Vacaciones WHERE idvacaciones = @idVacacion";
                                SqlCommand commandObtenerFechas = new SqlCommand(queryObtenerFechas, connection);
                                commandObtenerFechas.Parameters.AddWithValue("@idVacacion", idVacacion);

                                SqlDataReader reader = commandObtenerFechas.ExecuteReader();
                                DateTime fechaInicio = DateTime.MinValue;
                                DateTime fechaFin = DateTime.MinValue;

                                if (reader.Read())
                                {
                                    fechaInicio = Convert.ToDateTime(reader["fecha_inicio"]);
                                    fechaFin = Convert.ToDateTime(reader["fecha_fin"]);
                                }
                                reader.Close();

                                
                                List<DateTime> diasFeriadosEnRango = ObtenerDiasFeriados(fechaInicio, fechaFin)
                                    .Where(d => d >= fechaInicio && d <= fechaFin)
                                    .ToList();

                                int diasTotales = (fechaFin - fechaInicio).Days + 1; 
                                int diasLaborables = diasTotales - diasFeriadosEnRango.Count; 

                                
                                string queryActualizarDias = @"
                            UPDATE Vacaciones
                            SET dias_disponibles = dias_disponibles - @diasLaborables
                            WHERE idvacaciones = @idVacacion";

                                SqlCommand commandActualizarDias = new SqlCommand(queryActualizarDias, connection);
                                commandActualizarDias.Parameters.AddWithValue("@diasLaborables", diasLaborables);
                                commandActualizarDias.Parameters.AddWithValue("@idVacacion", idVacacion);
                                commandActualizarDias.ExecuteNonQuery();
                            }

                            connection.Close();
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('ID de la solicitud de vacaciones inválido.');", true);
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
                CargarVacaciones();
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Estado de las solicitudes de vacaciones actualizado correctamente.');", true);
            }
        }



        protected void ddlFiltroEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarVacaciones();
        }

        protected void btnSalir_Click(object sender, EventArgs e)
        {
            
            Response.Redirect("MenuAdministrador.aspx");
        }
    }
}