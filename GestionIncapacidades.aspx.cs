using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using iTextSharp.text.pdf.codec.wmf;
using System.Drawing;


namespace EmpresaProyecto
{
    public partial class GestionIncapacidades : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Administrador")
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                CargarIncapacidades();
            }
        }

        private void CargarIncapacidades()
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                
                string filtroEstado = ddlFiltroEstado.SelectedValue;

                string query = @"
                SELECT i.idincapacidades, e.idEmpleado, e.Nombre, e.Primer_apellido, e.Segundo_apellido, e.identificacion,
                i.fecha_inicio AS FechaInicio, i.fecha_fin AS FechaFin, es.estado AS EstadoIncapacidad,
                est.estado AS EstadoEmpleado, i.documento_incapacidad AS DocumentoIncapacidad
                FROM Incapacidades i
                JOIN Empleados e ON i.idEmpleado = e.idEmpleado
                JOIN estado_incapacidad es ON i.idestado_incapacidad = es.idestado_incapacidad
                JOIN Estado est ON e.idEstado = est.idEstado";



                if (filtroEstado != "0") 
                {
                    query += " WHERE i.idestado_incapacidad = @filtroEstado";
                }

                SqlCommand command = new SqlCommand(query, connection);

                if (filtroEstado != "0") 
                {
                    command.Parameters.AddWithValue("@filtroEstado", filtroEstado);
                }

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                var incapacidades = new List<dynamic>();

                while (reader.Read())
                {
                    incapacidades.Add(new
                    {
                        idincapacidades = reader["idincapacidades"],
                        idEmpleado = reader["idEmpleado"],
                        NombreCompleto = $"{reader["Nombre"]} {reader["Primer_apellido"]} {reader["Segundo_apellido"]}",
                        Identificacion = reader["identificacion"],
                        FechaInicio = reader["FechaInicio"],
                        FechaFin = reader["FechaFin"],
                        EstadoIncapacidad = reader["EstadoIncapacidad"],
                        EstadoEmpleado = reader["EstadoEmpleado"]
                    });
                }

                repeaterIncapacidades.DataSource = incapacidades;
                repeaterIncapacidades.DataBind();
            }
        }

        protected void btnActualizarEstado_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (RepeaterItem item in repeaterIncapacidades.Items)
                        {
                            CheckBox chkSelectIncapacidad = (CheckBox)item.FindControl("chkSelectIncapacidad");
                            HiddenField hfIdIncapacidad = (HiddenField)item.FindControl("hfIdIncapacidad");
                            HiddenField hfIdEmpleado = (HiddenField)item.FindControl("hfIdEmpleado");

                            if (chkSelectIncapacidad != null && chkSelectIncapacidad.Checked && hfIdIncapacidad != null && hfIdEmpleado != null)
                            {
                                int idIncapacidad = int.Parse(hfIdIncapacidad.Value);
                                int idEmpleado = int.Parse(hfIdEmpleado.Value);

                                System.Diagnostics.Debug.WriteLine($"Procesando incapacidad ID: {idIncapacidad}, Empleado ID: {idEmpleado}");

                                
                                string queryIncapacidad = @"
                                 UPDATE Incapacidades 
                                 SET idestado_incapacidad = @nuevoEstado, notificacion = 0 
                                 WHERE idincapacidades = @idIncapacidad";
                                SqlCommand commandIncapacidad = new SqlCommand(queryIncapacidad, connection, transaction);
                                commandIncapacidad.Parameters.AddWithValue("@nuevoEstado", 7); 
                                commandIncapacidad.Parameters.AddWithValue("@idIncapacidad", idIncapacidad);
                                commandIncapacidad.ExecuteNonQuery();

                                
                                string queryActualizarEstadoEmpleado = @"
                                 UPDATE Estado
                                 SET estado = CASE
                                  WHEN EXISTS (
                                    SELECT 1 
                                    FROM Incapacidades 
                                    WHERE idEmpleado = @idEmpleado AND idestado_incapacidad = 7
                                 ) THEN 'Incapacitado'
                                 ELSE 'Activo'
                                 END
                                 WHERE idEmpleado = @idEmpleado";
                                SqlCommand commandActualizarEstadoEmpleado = new SqlCommand(queryActualizarEstadoEmpleado, connection, transaction);
                                commandActualizarEstadoEmpleado.Parameters.AddWithValue("@idEmpleado", idEmpleado);
                                commandActualizarEstadoEmpleado.ExecuteNonQuery();

                                
                                System.Diagnostics.Debug.WriteLine($"Empleado ID: {idEmpleado} actualizado correctamente.");
                            }
                        }

                        transaction.Commit();
                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Estado actualizado correctamente.');", true);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Error al actualizar: {ex.Message}');", true);
                    }
                }
            }

          
            CargarIncapacidades();
        }


        protected void btnSalir_Click(object sender, EventArgs e)
        {
            Response.Redirect("MenuAdministrador.aspx");
        }

        protected void ddlFiltroEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarIncapacidades();
        }


    }
}
