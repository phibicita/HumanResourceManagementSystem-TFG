using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace EmpresaProyecto
{
    public partial class SolicitudVacaciones : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    int empleadoId = Convert.ToInt32(Session["UserID"]);
                    lblNombreEmpleado.Text = ObtenerNombreEmpleado(empleadoId);
                    MostrarDiasDisponibles(empleadoId);
                    CargarSolicitudesVacaciones(empleadoId);
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
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
            new DateTime(anio, 5, 1),  // día del Trabajador
            new DateTime(anio, 7, 25), // anexión de Guanacaste
            new DateTime(anio, 8, 2),  // Virgen de los Ángeles
            new DateTime(anio, 8, 15), // día de la Madre
            new DateTime(anio, 9, 15), // día Independencia
            new DateTime(anio, 12, 25) // navidad
        });
            }

            return diasFeriados;
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

        private void MostrarDiasDisponibles(int empleadoId)
        {
            try
            {
                int diasDisponibles = ObtenerDiasDisponibles(empleadoId); 

                
                if (diasDisponibles < 0)
                {
                    diasDisponibles = 0;
                }

                lblDiasDisponibles.Text = $"Días disponibles: {diasDisponibles}";
            }
            catch (Exception ex)
            {
                lblDiasDisponibles.Text = "Error al calcular los días disponibles.";
                
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        private int ObtenerDiasDisponibles(int empleadoId)
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";
            int diasDisponibles = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
               SELECT 
                -- Días acumulados por cada 50 semanas trabajadas
                ((DATEDIFF(WEEK, fecha_ingreso, GETDATE()) / 50) * 14) AS diasAcumulados,
                -- Restar días de solicitudes aprobadas
                ISNULL(SUM(DATEDIFF(DAY, fecha_inicio, fecha_fin) + 1), 0) AS diasAprobados
                FROM Empleados e
                LEFT JOIN Vacaciones v ON e.idEmpleado = v.idEmpleado AND v.idEstadoVacaciones = 2
                WHERE e.idEmpleado = @idEmpleado
                GROUP BY e.fecha_ingreso";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@idEmpleado", empleadoId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int diasAcumulados = Convert.ToInt32(reader["diasAcumulados"]);
                    int diasAprobados = Convert.ToInt32(reader["diasAprobados"]);

                    diasDisponibles = diasAcumulados - diasAprobados;
                }
                connection.Close();
            }

            return diasDisponibles;
        }

        private void CargarSolicitudesVacaciones(int empleadoId)
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
              SELECT v.idvacaciones, v.fecha_inicio, v.fecha_fin, v.dias_disponibles, e.descripcion AS estado_solicitud
              FROM Vacaciones v
              INNER JOIN estado_vacaciones e ON v.idEstadoVacaciones = e.idEstadoVacaciones
              WHERE v.idEmpleado = @idEmpleado AND v.fecha_inicio IS NOT NULL AND v.fecha_fin IS NOT NULL";


                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@idEmpleado", empleadoId);

                connection.Open();
                repeaterVacaciones.DataSource = command.ExecuteReader();
                repeaterVacaciones.DataBind();
                connection.Close();
            }
        }

        protected void btnSolicitarVacaciones_Click(object sender, EventArgs e)
        {
            try
            {
               
                DateTime fechaInicio, fechaFin;

                
                if (!DateTime.TryParse(txtFechaInicio.Text, out fechaInicio) || !DateTime.TryParse(txtFechaFin.Text, out fechaFin))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Por favor, ingrese fechas válidas de inicio y fin.');", true);
                    return;
                }

                
                if (fechaInicio < DateTime.Today || fechaFin < DateTime.Today)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                        "alert('No se puede solicitar vacaciones para fechas anteriores a hoy.');", true);
                    return;
                }

                if (fechaInicio == DateTime.MinValue || fechaFin == DateTime.MinValue)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Por favor, seleccione las fechas de inicio y fin.');", true);
                    return;
                }

                
                var diasFeriadosEnRango = ObtenerDiasFeriados(fechaInicio, fechaFin)
                    .Where(d => d >= fechaInicio && d <= fechaFin)
                    .ToList();

                int diasSeleccionados = (fechaFin - fechaInicio).Days + 1;
                int diasLaborables = diasSeleccionados - diasFeriadosEnRango.Count;

                if (diasLaborables != 14)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                        $"alert('Debe seleccionar exactamente 14 días laborables consecutivos. Actualmente seleccionó {diasLaborables} días laborables.');", true);
                    return;
                }

                int empleadoId = Convert.ToInt32(Session["UserID"]);
                int diasDisponibles = ObtenerDiasDisponibles(empleadoId);

                if (diasDisponibles < 14)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                        "alert('No tienes suficientes días acumulados para tomar vacaciones.');", true);
                    return;
                }

                string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string queryInsertarSolicitud = @"
                  INSERT INTO Vacaciones (idEmpleado, fecha_inicio, fecha_fin, dias_disponibles, idEstadoVacaciones)
                  VALUES (@idEmpleado, @FechaInicio, @FechaFin, @DiasRestantes, 1)";

                    SqlCommand commandInsertarSolicitud = new SqlCommand(queryInsertarSolicitud, connection);
                    commandInsertarSolicitud.Parameters.AddWithValue("@idEmpleado", empleadoId);
                    commandInsertarSolicitud.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    commandInsertarSolicitud.Parameters.AddWithValue("@FechaFin", fechaFin);
                    commandInsertarSolicitud.Parameters.AddWithValue("@DiasRestantes", diasDisponibles - diasLaborables);

                    connection.Open();
                    commandInsertarSolicitud.ExecuteNonQuery();
                    connection.Close();
                }

                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    "alert('Solicitud de vacaciones realizada con éxito.');", true);
                MostrarDiasDisponibles(empleadoId);
                CargarSolicitudesVacaciones(empleadoId);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert",
                    $"alert('Ocurrió un error: {ex.Message}');", true);
            }
        }


        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            Button btnEliminar = (Button)sender;
            int idVacaciones = Convert.ToInt32(btnEliminar.CommandArgument);
            EliminarSolicitud(idVacaciones);

            int empleadoId = Convert.ToInt32(Session["UserID"]);
            CargarSolicitudesVacaciones(empleadoId);
        }

        private void EliminarSolicitud(int idVacaciones)
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string queryVerificarEstado = "SELECT idEstadoVacaciones FROM Vacaciones WHERE idvacaciones = @idVacaciones";
                SqlCommand verificarCommand = new SqlCommand(queryVerificarEstado, connection);
                verificarCommand.Parameters.AddWithValue("@idVacaciones", idVacaciones);

                object estadoResult = verificarCommand.ExecuteScalar();
                int estadoSolicitud = estadoResult != null ? Convert.ToInt32(estadoResult) : -1;

                if (estadoSolicitud == 2 || estadoSolicitud == 3)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                        "alert('No se puede eliminar una solicitud que ya ha sido aprobada o rechazada.');", true);
                    return; 
                }

                string queryObtenerDias = "SELECT DATEDIFF(DAY, fecha_inicio, fecha_fin) + 1 AS diasUtilizados FROM Vacaciones WHERE idvacaciones = @idVacaciones";
                SqlCommand commandObtenerDias = new SqlCommand(queryObtenerDias, connection);
                commandObtenerDias.Parameters.AddWithValue("@idVacaciones", idVacaciones);

                object result = commandObtenerDias.ExecuteScalar();
                int diasUtilizados = result != DBNull.Value ? Convert.ToInt32(result) : 0;

                
                string queryRestablecerSolicitud = @"
                UPDATE Vacaciones
                SET fecha_inicio = NULL,
                fecha_fin = NULL,
                dias_disponibles = dias_disponibles + @DiasUtilizados,
                diasPendientes = NULL,
                idEstadoVacaciones = 1,
                notificacion = 0,
                notificacion_respuesta = 0
                WHERE idvacaciones = @idVacaciones";

                SqlCommand commandRestablecer = new SqlCommand(queryRestablecerSolicitud, connection);
                commandRestablecer.Parameters.AddWithValue("@DiasUtilizados", diasUtilizados);
                commandRestablecer.Parameters.AddWithValue("@idVacaciones", idVacaciones);

                commandRestablecer.ExecuteNonQuery();

                ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                    "alert('La solicitud de vacaciones ha sido eliminada y restablecida correctamente.');", true);
            }
        }



        protected void btnSalir_Click(object sender, EventArgs e)
        {
            Response.Redirect("MenuEmpleados.aspx");
        }
    }
}
