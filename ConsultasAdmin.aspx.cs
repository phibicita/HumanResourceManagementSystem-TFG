using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace EmpresaProyecto
{
    public partial class ConsultasAdmin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Administrador")
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                CargarEmpleados();
                CargarTipoReporte();
                gvResultados.DataBound += gvResultados_DataBound; 
            }
        }


        private void CargarEmpleados()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["empresaBD"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT idEmpleado, CONCAT(Nombre, ' ', Primer_apellido, ' ', Segundo_apellido) AS NombreCompleto FROM Empleados";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                ddlEmpleado.DataSource = reader;
                ddlEmpleado.DataTextField = "NombreCompleto";
                ddlEmpleado.DataValueField = "idEmpleado";
                ddlEmpleado.DataBind();

                ddlEmpleado.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Todos", "0"));
            }
        }

        protected void gvResultados_DataBound(object sender, EventArgs e)
        {
            if (gvResultados.HeaderRow != null)
            {
                foreach (TableCell cell in gvResultados.HeaderRow.Cells)
                {
                    
                    string cleanHeader = System.Text.RegularExpressions.Regex.Replace(cell.Text, "([a-z])([A-Z])", "$1 $2"); // div mayus
                    cleanHeader = cleanHeader.Replace("_", " "); //  guiones bajos con espacio
                    cell.Text = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cleanHeader.ToLower()); 
                }
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
            ddlTipoReporte.Items.Add(new System.Web.UI.WebControls.ListItem("Evaluación del Personal", "Evaluacion"));
            ddlTipoReporte.Items.Add(new System.Web.UI.WebControls.ListItem("Horarios", "Horarios"));
            ddlTipoReporte.Items.Add(new System.Web.UI.WebControls.ListItem("Liquidaciones", "Liquidaciones"));
            ddlTipoReporte.Items.Add(new System.Web.UI.WebControls.ListItem("Aguinaldo", "Aguinaldo"));
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

                    gvResultados.DataSource = dt;
                    gvResultados.DataBind();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Error: {ex.Message}');", true);
            }
        }
        private string BuildQuery()
        {
            string baseQuery = "";
            string table = "";
            string fechaColumna = "";
            string extraSelect = "";
            string joinEmpleado = "JOIN Empleados e ON t.idEmpleado = e.idEmpleado";
            string selectEmpleado = "e.identificacion, CONCAT(e.Nombre, ' ', e.Primer_apellido, ' ', e.Segundo_apellido) AS NombreCompleto,";

            
            switch (ddlTipoReporte.SelectedValue)
            {
                case "Nomina":
                    table = "nomina";
                    fechaColumna = "fecha_nomina";
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
                    table = "asistencia";
                    fechaColumna = "fecha";
                    extraSelect = "t.fecha, t.hora_entrada, t.hora_salida"; 
                    break;

                case "Horas Extra":
                    table = "horas_extra";
                    fechaColumna = "fecha";
                    extraSelect = @"
                     t.cantidad_horas, 
                     t.fecha, 
                     eh.estado AS EstadoHorasExtra"; 
                    joinEmpleado += " JOIN estado_horasExtra eh ON t.idestado_horasExtra = eh.idestado_horasExtra"; 
                    break;


                case "Vacaciones":
                    table = "vacaciones";
                    fechaColumna = "fecha_inicio";
                    extraSelect = @"
                t.fecha_inicio, 
                t.fecha_fin, 
                DATEDIFF(DAY, t.fecha_inicio, t.fecha_fin) + 1 AS Dias_Solicitados,
                t.dias_disponibles,  
                ev.descripcion AS EstadoVacaciones";
                    joinEmpleado += " JOIN estado_vacaciones ev ON t.idEstadoVacaciones = ev.idEstadoVacaciones";
                    break;

                case "Incapacidades":
                    table = "incapacidades";
                    fechaColumna = "fecha_inicio";
                    extraSelect = @"
                     t.fecha_inicio, 
                     t.fecha_fin,
                     t.documento_incapacidad,
                     ti.descripcion AS TipoIncapacidad,
                     ei.estado AS EstadoIncapacidad";
                    joinEmpleado += @"
                    JOIN tipo_incapacidad ti ON t.idtipo_incapacidad = ti.idtipo_incapacidad
                    JOIN estado_incapacidad ei ON t.idestado_incapacidad = ei.idestado_incapacidad";
                    break;



                case "Permisos":
                    table = "permisos";
                    fechaColumna = "fecha_permiso";
                    extraSelect = "t.fecha_permiso,t.hora_inicio, t.hora_fin, t.estado";
                    break;

                case "Evaluacion":
                    table = "evaluacion";
                    fechaColumna = "fecha_evaluacion";
                    extraSelect = "t.puntualidad, t.responsabilidad, t.trabajo_equipo, t.disposicion_aprendizaje, t.manejo_tiempo, t.comunicacion, t.resolucion_problemas, t.compromiso";
                    break;

                case "Horarios":
                    table = "horarios";
                    extraSelect = "t.diaSemana, t.horaEntrada, t.horaSalida";
                    fechaColumna = ""; 
                    break;

                case "Liquidaciones":
                    table = "Liquidaciones";
                    fechaColumna = "fechaLiquidacion";
                    extraSelect = @"
                  t.fechaLiquidacion, 
                   t.montoTotal, 
                   t.cesantia, 
                   t.preaviso, 
                   t.vacacionesPendientes";
                    break;


                case "Aguinaldo":
                    table = "aguinaldo";
                    fechaColumna = "fecha_calculo";
                    extraSelect = " t.fecha_calculo, t.monto_aguinaldo, t.monto_total_ingresos";
                    break;

                default:
                    throw new Exception("Tipo de reporte no válido.");
            }

            
            baseQuery = $@"
             SELECT {selectEmpleado} {extraSelect} 
              FROM {table} t
              {joinEmpleado} 
              WHERE 1=1";

            
            if (!string.IsNullOrEmpty(fechaColumna))
            {
                if (!string.IsNullOrEmpty(txtFechaInicio.Text))
                {
                    if (DateTime.TryParse(txtFechaInicio.Text, out DateTime fechaInicio))
                    {
                        baseQuery += $" AND t.{fechaColumna} >= '{fechaInicio:yyyy-MM-dd}'";
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
                        baseQuery += $" AND t.{fechaColumna} <= '{fechaFin:yyyy-MM-dd}'";
                    }
                    else
                    {
                        throw new Exception("El formato de la Fecha Fin no es válido.");
                    }
                }
            }

           
            if (!string.IsNullOrEmpty(ddlEmpleado.SelectedValue) && ddlEmpleado.SelectedValue != "0")
            {
                baseQuery += $" AND t.idEmpleado = {ddlEmpleado.SelectedValue}";
            }

            return baseQuery;
        }

        protected void btnExportarPDF_Click(object sender, EventArgs e)
        {
            try
            {
               
                bool isPagingEnabled = gvResultados.AllowPaging;
                gvResultados.AllowPaging = false;
                btnConsultar_Click(sender, e); 

                if (gvResultados == null || gvResultados.Rows.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('No hay datos para exportar a PDF.');", true);
                    gvResultados.AllowPaging = isPagingEnabled; 
                    return;
                }

                string pdfPath = Server.MapPath("~/Reportes/ConsultaReporteAdmin.pdf");

                
                Font headerFont = FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK);
                Font cellFont = FontFactory.GetFont("Arial", 9, Font.NORMAL, BaseColor.BLACK);
                Font titleFont = FontFactory.GetFont("Arial", 14, Font.BOLD, new BaseColor(255, 167, 38));
                Font companyFont = FontFactory.GetFont("Arial", 16, Font.BOLD, BaseColor.BLACK);

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

                        PdfPCell logoCell = new PdfPCell(logo) { Border = PdfPCell.NO_BORDER, HorizontalAlignment = Element.ALIGN_CENTER };
                        headerTable.AddCell(logoCell);
                    }

                    PdfPCell companyCell = new PdfPCell(new Phrase("Corporación Krasinski S.A", companyFont))
                    {
                        Border = PdfPCell.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    headerTable.AddCell(companyCell);
                    pdfDoc.Add(headerTable);

                    
                    Paragraph title = new Paragraph($"Reporte de {ddlTipoReporte.SelectedItem.Text}", titleFont)
                    {
                        Alignment = Element.ALIGN_CENTER
                    };
                    pdfDoc.Add(title);

                    pdfDoc.Add(new Paragraph($"Fecha de Generación: {DateTime.Now:dd/MM/yyyy}", new Font(headerFont)) { Alignment = Element.ALIGN_CENTER });
                    pdfDoc.Add(new Paragraph(" "));

                    
                    int numColumns = gvResultados.HeaderRow.Cells.Count;
                    PdfPTable table = new PdfPTable(numColumns) { WidthPercentage = 100 };

                    table.SetWidths(Enumerable.Repeat(1f, numColumns).ToArray());

                    
                    foreach (TableCell headerCell in gvResultados.HeaderRow.Cells)
                    {
                        string cleanHeader = headerCell.Text
                            .Replace("NombreCompleto", "Nombre Completo")
                            .Replace("TotalDeducciones", "Total Deducciones")
                            .Replace("MesCalculado", "Mes Calculado")
                            .Replace("_", " ");

                        PdfPCell pdfHeaderCell = new PdfPCell(new Phrase(cleanHeader, headerFont))
                        {
                            BackgroundColor = new BaseColor(230, 230, 230),
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            PaddingTop = 5,
                            PaddingBottom = 5
                        };
                        table.AddCell(pdfHeaderCell);
                    }

                    
                    foreach (GridViewRow row in gvResultados.Rows)
                    {
                        for (int i = 0; i < row.Cells.Count; i++)
                        {
                            string cellText = row.Cells[i].Text.Trim().Replace("&nbsp;", "");

                            
                            if (gvResultados.HeaderRow.Cells[i].Text == "identificacion")
                            {
                                cellText = row.Cells[i].Text;
                            }
                            else if (gvResultados.HeaderRow.Cells[i].Text.Contains("salario") ||
                                     gvResultados.HeaderRow.Cells[i].Text.Contains("Deduccion") ||
                                     gvResultados.HeaderRow.Cells[i].Text.Contains("monto") ||
                                     gvResultados.HeaderRow.Cells[i].Text.Contains("Total Deducciones"))
                            {
                                if (decimal.TryParse(cellText, out decimal value))
                                {
                                    cellText = $"₡ {value:N2}";
                                }
                            }

                            PdfPCell pdfCell = new PdfPCell(new Phrase(cellText, cellFont))
                            {
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                PaddingTop = 5,
                                PaddingBottom = 5
                            };
                            table.AddCell(pdfCell);
                        }
                    }

                    pdfDoc.Add(table);
                    pdfDoc.Close();
                }

                gvResultados.AllowPaging = isPagingEnabled; 
                btnConsultar_Click(sender, e);

                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Reporte exportado a PDF correctamente.');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Error al exportar PDF: {ex.Message}');", true);
            }
        }

        protected void btnSalir_Click(object sender, EventArgs e)
        {
            
            if (Session["UserRole"] != null && Session["UserRole"].ToString() == "Administrador")
            {
                Response.Redirect("MenuAdministrador.aspx");
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

       
    }
}
