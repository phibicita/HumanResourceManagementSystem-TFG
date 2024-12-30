using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EmpresaProyecto
{
    public partial class SolicitarHorasExtra : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    int empleadoId = Convert.ToInt32(Session["UserID"]);
                    lblEmpleadoNombre.Text = ObtenerNombreEmpleado(empleadoId);

                    ddlEstado.Items.Add(new ListItem("Todas", "0"));
                    ddlEstado.Items.Add(new ListItem("Pendiente", "1"));
                    ddlEstado.Items.Add(new ListItem("Aprobada", "2"));
                    ddlEstado.Items.Add(new ListItem("Rechazada", "3"));
                    ddlEstado.SelectedIndex = 0;
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }

            if (Session["UserID"] != null)
            {
                int empleadoId = Convert.ToInt32(Session["UserID"]);
                CargarSolicitudesHorasExtra(empleadoId);
            }
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
        private void CargarSolicitudesHorasExtra(int empleadoId)
        {
            tablaSolicitudes.Rows.Clear();

            
            TableHeaderRow header = new TableHeaderRow();
            header.Cells.Add(new TableHeaderCell { Text = "Seleccionar" });
            header.Cells.Add(new TableHeaderCell { Text = "Fecha de solicitud" });
            header.Cells.Add(new TableHeaderCell { Text = "Horas solicitadas" });
            header.Cells.Add(new TableHeaderCell { Text = "Estado" });
            header.Cells.Add(new TableHeaderCell { Text = "Fecha de respuesta" });
            header.Cells.Add(new TableHeaderCell { Text = "Comentarios" });
            tablaSolicitudes.Rows.Add(header);

            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string estadoFiltro = ddlEstado.SelectedValue;
                string query = @"
                 SELECT idhoras_extra, fecha, cantidad_horas, idestado_horasExtra
                 FROM dbo.horas_extra
                 WHERE idEmpleado = @empleadoId";

                if (estadoFiltro != "0")
                {
                    query += " AND idestado_horasExtra = @estadoFiltro";
                }

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@empleadoId", empleadoId);
                if (estadoFiltro != "0")
                {
                    command.Parameters.AddWithValue("@estadoFiltro", estadoFiltro);
                }

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    TableRow row = new TableRow();

                    
                    TableCell checkCell = new TableCell();
                    CheckBox chk = new CheckBox();
                    chk.ID = "chk_" + reader["idhoras_extra"].ToString();
                    checkCell.Controls.Add(chk);
                    row.Cells.Add(checkCell);

                   
                    TableCell fechaCell = new TableCell();
                    fechaCell.Text = Convert.ToDateTime(reader["fecha"]).ToString("dd/MM/yyyy");
                    row.Cells.Add(fechaCell);

                    
                    TableCell horasCell = new TableCell();
                    horasCell.Text = reader["cantidad_horas"].ToString();
                    row.Cells.Add(horasCell);

                    
                    TableCell estadoCell = new TableCell();
                    int estadoId = Convert.ToInt32(reader["idestado_horasExtra"]);
                    estadoCell.Text = ObtenerEstadoSolicitud(estadoId);
                    row.Cells.Add(estadoCell);

                   
                    TableCell respuestaCell = new TableCell();
                    if (estadoId == 2 || estadoId == 3) 
                    {
                        respuestaCell.Text = DateTime.Now.ToString("dd/MM/yyyy"); //  fecha actual
                    }
                    else
                    {
                        respuestaCell.Text = "-";
                    }
                    row.Cells.Add(respuestaCell);

                    TableCell comentariosCell = new TableCell();
                    comentariosCell.Text = ObtenerComentarios(estadoId);
                    row.Cells.Add(comentariosCell);

                    tablaSolicitudes.Rows.Add(row);
                }
                connection.Close();
            }
        }

        private string ObtenerEstadoSolicitud(int estadoId)
        {
            switch (estadoId)
            {
                case 1:
                    return "Pendiente";
                case 2:
                    return "Aprobada";
                case 3:
                    return "Rechazada";
                default:
                    return "Desconocido";
            }
        }

        private string ObtenerComentarios(int estadoId)
        {
            switch (estadoId)
            {
                case 2:
                    return "Aprobada.";
                case 3:
                    return "No se aprobaron horas extra este día.";
                default:
                    return "-";
            }
        }

        protected void btnSolicitarHorasExtra_Click(object sender, EventArgs e)
        {
            // otra logica
        }

      
        protected void btnCancelarSolicitud_Click(object sender, EventArgs e)
        {
            List<TableRow> filasSeleccionadas = new List<TableRow>();

            foreach (TableRow row in tablaSolicitudes.Rows)
            {
                if (row.Cells[0].Controls.Count > 0 && row.Cells[0].Controls[0] is CheckBox chk && chk.Checked)
                {
                    string idHorasExtra = chk.ID.Split('_')[1];
                    string estado = row.Cells[3].Text;

                    if (estado != "Pendiente")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Solo puedes eliminar solicitudes pendientes.');", true);
                        return;
                    }

                    filasSeleccionadas.Add(row);
                }
            }

            foreach (TableRow row in filasSeleccionadas)
            {
                string idHorasExtra = ((CheckBox)row.Cells[0].Controls[0]).ID.Split('_')[1];
                string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM dbo.horas_extra WHERE idhoras_extra = @idHorasExtra";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@idHorasExtra", idHorasExtra);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            int empleadoId = Convert.ToInt32(Session["UserID"]);
            CargarSolicitudesHorasExtra(empleadoId);
        }

        protected void ddlEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            if (Session["UserID"] != null)
            {
                int empleadoId = Convert.ToInt32(Session["UserID"]);

               
                CargarSolicitudesHorasExtra(empleadoId);
            }
            else
            {
               
                Response.Redirect("Login.aspx");
            }
        }
        protected void btnEditar_Click(object sender, EventArgs e)
        {
            foreach (TableRow row in tablaSolicitudes.Rows)
            {
                if (row.Cells[0].Controls.Count > 0 && row.Cells[0].Controls[0] is CheckBox chk && chk.Checked)
                {
                    string idHorasExtra = chk.ID.Split('_')[1]; 
                    string horasActuales = row.Cells[2].Text; 

                    hdnIdHorasExtraModal.Value = idHorasExtra;
                    txtEditarHoras.Text = horasActuales;

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "showModal", "document.getElementById('modalCheckbox').checked = true;", true);
                    return;
                }
            }

            
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Por favor, selecciona una solicitud para editar.');", true);
        }

        protected void btnGuardarEdicion_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdnIdHorasExtraModal.Value) && !string.IsNullOrEmpty(txtEditarHoras.Text))
            {
                try
                {
                    int idHorasExtra = int.Parse(hdnIdHorasExtraModal.Value);
                    int nuevasHoras = int.Parse(txtEditarHoras.Text);

                    string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = @"UPDATE dbo.horas_extra 
                                 SET cantidad_horas = @nuevasHoras 
                                 WHERE idhoras_extra = @idHorasExtra";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@nuevasHoras", nuevasHoras);
                        command.Parameters.AddWithValue("@idHorasExtra", idHorasExtra);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }

                    
                    int empleadoId = Convert.ToInt32(Session["UserID"]);
                    CargarSolicitudesHorasExtra(empleadoId);

                    hdnIdHorasExtraModal.Value = string.Empty;
                    txtEditarHoras.Text = string.Empty;

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('La solicitud se ha editado correctamente.');", true);
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Por favor, selecciona una solicitud e ingresa el número de horas.');", true);
            }
        }

        protected void btnEnviarSolicitud_Click(object sender, EventArgs e)
        {
            if (Session["UserID"] != null)
            {
                int empleadoId = Convert.ToInt32(Session["UserID"]);
                int horasSolicitadas = Convert.ToInt32(txtHoras.Text);
                string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";

                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string queryValidarHoras = @"
                SELECT ISNULL(SUM(cantidad_horas), 0) 
                FROM dbo.horas_extra 
                WHERE idEmpleado = @idEmpleado AND CAST(fecha AS DATE) = CAST(GETDATE() AS DATE)";

                    SqlCommand commandValidarHoras = new SqlCommand(queryValidarHoras, connection);
                    commandValidarHoras.Parameters.AddWithValue("@idEmpleado", empleadoId);

                    connection.Open();
                    int horasTotalesHoy = Convert.ToInt32(commandValidarHoras.ExecuteScalar());
                    connection.Close();

                    if (horasTotalesHoy + horasSolicitadas > 4)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No puedes solicitar más de 4 horas extra por día.');", true);
                        return;
                    }
                }

                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                INSERT INTO dbo.horas_extra (fecha, cantidad_horas, monto_horas, idEmpleado, idestado_horasExtra, notificacion)
                VALUES (GETDATE(), @cantidad_horas, 0, @idEmpleado, 1, 1)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@cantidad_horas", horasSolicitadas);
                    command.Parameters.AddWithValue("@idEmpleado", empleadoId);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                
                CargarSolicitudesHorasExtra(empleadoId);
            }
        }

    }
}