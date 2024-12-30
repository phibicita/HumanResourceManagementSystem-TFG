using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace EmpresaProyecto
{
    public partial class GestionHorasExtra : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Administrador")
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                CargarHorasExtra();
            }
        }

        private void CargarHorasExtra(int estadoFiltro = 0)
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                
                string query = @"SELECT h.idhoras_extra, e.Nombre, e.Primer_apellido, e.Segundo_apellido, e.identificacion,
                         h.fecha AS Fecha, h.cantidad_horas AS HorasSolicitadas, eh.estado AS EstadoHoraExtra
                         FROM horas_extra h
                         JOIN Empleados e ON h.idEmpleado = e.idEmpleado
                         JOIN estado_horasExtra eh ON h.idestado_horasExtra = eh.idestado_horasExtra";

                if (estadoFiltro > 0)
                {
                    query += " WHERE h.idestado_horasExtra = @estadoFiltro";
                }

                SqlCommand command = new SqlCommand(query, connection);
                if (estadoFiltro > 0)
                {
                    command.Parameters.AddWithValue("@estadoFiltro", estadoFiltro);
                }

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                var horasExtra = new List<dynamic>();

                while (reader.Read())
                {
                    horasExtra.Add(new
                    {
                        idhora_extra = reader["idhoras_extra"],
                        NombreCompleto = $"{reader["Nombre"]} {reader["Primer_apellido"]} {reader["Segundo_apellido"]}",
                        Identificacion = reader["identificacion"],
                        Fecha = reader["Fecha"],
                        HorasSolicitadas = reader["HorasSolicitadas"],
                        EstadoHoraExtra = reader["EstadoHoraExtra"]
                    });
                }

                repeaterHorasExtra.DataSource = horasExtra;
                repeaterHorasExtra.DataBind();
            }
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            int estadoFiltro = int.Parse(ddlEstadoHorasExtra.SelectedValue);
            CargarHorasExtra(estadoFiltro);
        }

        protected void ddlEstadoHorasExtra_SelectedIndexChanged(object sender, EventArgs e)
        {
            int estadoFiltro = int.Parse(ddlEstadoHorasExtra.SelectedValue);
            CargarHorasExtra(estadoFiltro);
        }

        protected void btnAceptarHoraExtra_Click(object sender, EventArgs e)
        {
            ActualizarEstadoHorasExtra("Aprobado");
        }

        protected void btnRechazarHoraExtra_Click(object sender, EventArgs e)
        {
            ActualizarEstadoHorasExtra("Rechazado");
        }

        private void ActualizarEstadoHorasExtra(string nuevoEstado)
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";
            bool solicitudSeleccionada = false;

            
            int estadoNumerico = 0;
            if (nuevoEstado == "Aprobado")
            {
                estadoNumerico = 2; //  "Aprobado"
            }
            else if (nuevoEstado == "Rechazado")
            {
                estadoNumerico = 3; //  "Rechazado"
            }

            foreach (RepeaterItem item in repeaterHorasExtra.Items)
            {
                CheckBox chkSelectHoraExtra = (CheckBox)item.FindControl("chkSelectHoraExtra");
                HiddenField hfIdHoraExtra = (HiddenField)item.FindControl("hfIdHoraExtra");

                if (chkSelectHoraExtra != null && chkSelectHoraExtra.Checked && hfIdHoraExtra != null)
                {
                    if (int.TryParse(hfIdHoraExtra.Value, out int idHoraExtra))
                    {
                        solicitudSeleccionada = true;

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            try
                            {
                               
                                string queryActualizar = @"
                                UPDATE horas_extra 
                                SET idestado_horasExtra = @nuevoEstado, 
                                notificacion = 0, 
                                notificacion_respuesta = 1 
                                WHERE idhoras_extra = @idHoraExtra";

                                SqlCommand actualizarCommand = new SqlCommand(queryActualizar, connection);
                                actualizarCommand.Parameters.AddWithValue("@nuevoEstado", estadoNumerico);
                                actualizarCommand.Parameters.AddWithValue("@idHoraExtra", idHoraExtra);

                                connection.Open();
                                actualizarCommand.ExecuteNonQuery();
                                connection.Close();
                            }
                            catch (Exception ex)
                            {
                                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Error al actualizar la solicitud de horas extra: {ex.Message}');", true);
                                return;
                            }
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('ID de la solicitud de horas extra inválido.');", true);
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
                
                CargarHorasExtra();
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Estado de las solicitudes de horas extra actualizado correctamente.');", true);
            }
        }



        protected void btnSalir_Click(object sender, EventArgs e)
        {
            
            Response.Redirect("MenuAdministrador.aspx");
        }
    }
}


