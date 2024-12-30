using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;


namespace EmpresaProyecto
{
    public partial class EmpleadosConsulta : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                if (Session["UserID"] != null)
                {
                    int empleadoId = Convert.ToInt32(Session["UserID"]);

                    lblEmpleadoNombre.Text = ObtenerNombreEmpleado(empleadoId);

                    CargarTipoReporte();

                    gvResultados.DataBound += gvResultados_DataBound;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Error: No hay una sesión activa. Por favor, inicie sesión nuevamente.');", true);
                    Response.Redirect("Login.aspx");
                }
            }
        }


        protected void gvResultados_DataBound(object sender, EventArgs e)
        {
            if (gvResultados.HeaderRow != null)
            {
                foreach (TableCell cell in gvResultados.HeaderRow.Cells)
                {
                   
                    string cleanHeader = System.Text.RegularExpressions.Regex.Replace(cell.Text, "([a-z])([A-Z])", "$1 $2");
                    cleanHeader = cleanHeader.Replace("_", " ");
                    cell.Text = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cleanHeader.ToLower()); 
                }
            }
        }

        private string ObtenerNombreEmpleado(int empleadoId)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["empresaBD"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT CONCAT(Nombre, ' ', Primer_apellido, ' ', Segundo_apellido) AS NombreCompleto FROM Empleados WHERE idEmpleado = @idEmpleado";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@idEmpleado", empleadoId);

                connection.Open();
                object result = command.ExecuteScalar();
                return result != null ? result.ToString() : "Empleado Desconocido";
            }
        }

        private void CargarTipoReporte()
        {
            ddlTipoReporte.Items.Clear();
            ddlTipoReporte.Items.Add(new System.Web.UI.WebControls.ListItem("Seleccione un tipo de reporte", ""));
            ddlTipoReporte.Items.Add(new System.Web.UI.WebControls.ListItem("Nómina", "Nomina"));
            ddlTipoReporte.Items.Add(new System.Web.UI.WebControls.ListItem("Asistencia", "Asistencia"));
            ddlTipoReporte.Items.Add(new System.Web.UI.WebControls.ListItem("Horas Extra", "Horas Extra"));
            ddlTipoReporte.Items.Add(new System.Web.UI.WebControls.ListItem("Vacaciones", "Vacaciones"));
            ddlTipoReporte.Items.Add(new System.Web.UI.WebControls.ListItem("Incapacidades", "Incapacidades"));
            ddlTipoReporte.Items.Add(new System.Web.UI.WebControls.ListItem("Permisos", "Permisos"));
        }

        protected void btnConsultar_Click(object sender, EventArgs e)
        {
            try
            {
                string query = BuildQuery();

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["empresaBD"].ConnectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        gvResultados.DataSource = dt;
                        gvResultados.DataBind();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('No se encontraron resultados para la consulta.');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Error: {ex.Message}');", true);
            }
        }
        private string BuildQuery()
        {
            string table = "";
            string extraSelect = "";
            string joinEmpleado = "";
            string fechaColumna = "";
            string idEmpleado = Session["UserID"] != null ? Session["UserID"].ToString() : null;

            if (string.IsNullOrEmpty(idEmpleado))
            {
                throw new Exception("El ID del empleado no está disponible en la sesión. Inicie sesión nuevamente.");
            }

            
            switch (ddlTipoReporte.SelectedValue)
            {
                case "Nomina":
                    table = "nomina t";
                    fechaColumna = "t.fecha_nomina";
                    extraSelect = @"
                t.salario_bruto, 
                t.salario_neto, 
                (t.salario_bruto * 0.055) AS Deduccion_CCSS,
                (t.salario_bruto * 0.0267) AS Deduccion_IVM,
                (t.salario_bruto * 0.01) AS Deduccion_SEC,
                ((t.salario_bruto * 0.055) + (t.salario_bruto * 0.0267) + (t.salario_bruto * 0.01)) AS TotalDeducciones,
                t.quincena, 
                t.MesCalculado";
                    break;

                case "Asistencia":
                    table = "asistencia t";
                    fechaColumna = "t.fecha";
                    extraSelect = "t.fecha, t.hora_entrada, t.hora_salida"; 
                    break;

                case "Horas Extra":
                    table = "horas_extra t";
                    fechaColumna = "t.fecha";
                    extraSelect = @"
                t.fecha AS Fecha,
                t.cantidad_horas AS CantidadHoras,
                eh.estado AS EstadoHorasExtra";
                    joinEmpleado = "INNER JOIN estado_horasExtra eh ON t.idestado_horasExtra = eh.idestado_horasExtra";
                    break;

                case "Vacaciones":
                    table = "vacaciones t";
                    extraSelect = @"
                  t.fecha_inicio, 
                  t.fecha_fin, 
                  DATEDIFF(DAY, t.fecha_inicio, t.fecha_fin) AS Dias_Solicitados,
                   CASE 
                  WHEN t.dias_disponibles < 0 THEN 0
                 ELSE t.dias_disponibles
                 END AS dias_disponibles,  
                  ev.descripcion AS EstadoVacaciones";
                    joinEmpleado = "INNER JOIN estado_vacaciones ev ON t.idEstadoVacaciones = ev.idEstadoVacaciones";
                    break;


                case "Incapacidades":
                    table = "incapacidades t";
                    fechaColumna = "t.fecha_inicio";
                    extraSelect = @"
                  t.fecha_inicio, 
                  t.fecha_fin,
                  t.documento_incapacidad, 
                  ti.descripcion AS TipoIncapacidad, 
                  ei.estado AS EstadoIncapacidad";
                    joinEmpleado = @"
                  INNER JOIN tipo_incapacidad ti ON t.idtipo_incapacidad = ti.idtipo_incapacidad
                   INNER JOIN estado_incapacidad ei ON t.idestado_incapacidad = ei.idestado_incapacidad";
                    break;


                case "Permisos":
                    table = "permisos t";
                    fechaColumna = "t.fecha_permiso";
                    extraSelect = "t.fecha_permiso, t.hora_inicio, t.hora_fin, t.estado";
                    break;

                default:
                    throw new Exception("Tipo de reporte no válido.");
            }

            
            string query = $@"
            SELECT {extraSelect}
            FROM {table}
           {joinEmpleado}
            WHERE t.idEmpleado = {idEmpleado}";

            
            if (!string.IsNullOrEmpty(fechaColumna))
            {
                if (!string.IsNullOrEmpty(txtFechaInicio.Text))
                {
                    if (DateTime.TryParse(txtFechaInicio.Text, out DateTime fechaInicio))
                    {
                        query += $" AND {fechaColumna} >= '{fechaInicio:yyyy-MM-dd}'";
                    }
                    else
                    {
                        throw new Exception("El formato de la Fecha Inicio no es válido.");
                    }
                }

                if (!string.IsNullOrEmpty(txtFechaFin.Text))
                {
                    if (DateTime.TryParse(txtFechaFin.Text, out DateTime fechaFin))
                    {
                        query += $" AND {fechaColumna} <= '{fechaFin:yyyy-MM-dd}'";
                    }
                    else
                    {
                        throw new Exception("El formato de la Fecha Fin no es válido.");
                    }
                }
            }

            
            Console.WriteLine($"Consulta generada: {query}");
            ScriptManager.RegisterStartupScript(this, GetType(), "log", $"console.log(`{query}`);", true);

            return query;
        }

        protected void btnSalir_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
        }

        protected void btnExportarPDF_Click(object sender, EventArgs e)
        {
            try
            {
                
                if (gvResultados == null || gvResultados.Rows.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('No hay datos para exportar a PDF.');", true);
                    return;
                }

                
                string pdfPath = Server.MapPath("~/Reportes/ConsultaReporte.pdf");

                
                string tituloReporte = ddlTipoReporte.SelectedItem != null ? ddlTipoReporte.SelectedItem.Text : "Reporte General";

                
                string nombreEmpleado = "No disponible";
                string identificacionEmpleado = "No disponible";

                int idEmpleado = Convert.ToInt32(Session["UserID"]); 
                string connectionString = ConfigurationManager.ConnectionStrings["empresaBD"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT identificacion, CONCAT(Nombre, ' ', Primer_apellido, ' ', Segundo_apellido) AS NombreCompleto
                             FROM Empleados WHERE idEmpleado = @idEmpleado";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@idEmpleado", idEmpleado);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        nombreEmpleado = reader["NombreCompleto"].ToString();
                        identificacionEmpleado = reader["identificacion"].ToString();
                    }
                }

                
                using (Document pdfDoc = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20))
                using (FileStream fs = new FileStream(pdfPath, FileMode.Create, FileAccess.Write))
                {
                    PdfWriter.GetInstance(pdfDoc, fs);
                    pdfDoc.Open();

                   
                    string imagePath = Server.MapPath("~/Images/LOGO.png");
                    PdfPTable headerTable = new PdfPTable(1) { WidthPercentage = 100 };

                    if (File.Exists(imagePath))
                    {
                        iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imagePath);
                        logo.ScaleAbsolute(120, 60);
                        logo.Alignment = Element.ALIGN_CENTER;
                        headerTable.AddCell(new PdfPCell(logo) { Border = PdfPCell.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER });
                    }

                    PdfPCell companyCell = new PdfPCell(new Phrase("Corporación Krasinski S.A", FontFactory.GetFont("Arial", 16, Font.BOLD)))
                    {
                        Border = PdfPCell.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    headerTable.AddCell(companyCell);
                    pdfDoc.Add(headerTable);

                    
                    Paragraph title = new Paragraph($"Reporte de {tituloReporte}",
                        FontFactory.GetFont("Arial", 14, Font.BOLD, new BaseColor(255, 167, 38)))
                    {
                        Alignment = Element.ALIGN_CENTER
                    };
                    pdfDoc.Add(title);

                    
                    Paragraph empleadoInfo = new Paragraph($"Empleado: {nombreEmpleado} | Identificación: {identificacionEmpleado}",
                        FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))
                    {
                        Alignment = Element.ALIGN_CENTER
                    };
                    pdfDoc.Add(empleadoInfo);

                    
                    Paragraph date = new Paragraph($"Fecha de Generación: {DateTime.Now:dd/MM/yyyy}",
                        FontFactory.GetFont("Arial", 10));
                    date.Alignment = Element.ALIGN_CENTER;
                    pdfDoc.Add(date);
                    pdfDoc.Add(new Paragraph(" "));

                    
                    int numColumns = gvResultados.HeaderRow.Cells.Count;
                    PdfPTable table = new PdfPTable(numColumns) { WidthPercentage = 100 };

                    
                    foreach (TableCell headerCell in gvResultados.HeaderRow.Cells)
                    {
                        string cleanHeader = headerCell.Text.Replace("_", " ");
                        PdfPCell pdfHeaderCell = new PdfPCell(new Phrase(cleanHeader, FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK)))
                        {
                            BackgroundColor = new BaseColor(230, 230, 230),
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Padding = 5
                        };
                        table.AddCell(pdfHeaderCell);
                    }

                    
                    foreach (GridViewRow row in gvResultados.Rows)
                    {
                        for (int i = 0; i < row.Cells.Count; i++)
                        {
                            string cellText = row.Cells[i].Text.Trim().Replace("&nbsp;", "");

                            PdfPCell pdfCell = new PdfPCell(new Phrase(cellText, FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK)))
                            {
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                Padding = 5
                            };
                            table.AddCell(pdfCell);
                        }
                    }

                    pdfDoc.Add(table);
                    pdfDoc.Close();
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Reporte exportado a PDF correctamente.');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Error al exportar PDF: {ex.Message}');", true);
            }
        }

    }
}
