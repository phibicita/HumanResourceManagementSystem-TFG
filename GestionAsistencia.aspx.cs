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
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace EmpresaProyecto
{
    public partial class GestionAsistencia : System.Web.UI.Page
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
                CargarAsistencia();
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

                ddlEmpleado.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Seleccione un empleado", ""));
            }
        }

        private void CargarAsistencia()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["empresaBD"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT 
                            a.fecha AS Fecha, 
                            CONCAT(e.Nombre, ' ', e.Primer_apellido, ' ', e.Segundo_apellido) AS Empleado, 
                            a.hora_entrada AS HoraEntrada, 
                            a.hora_salida AS HoraSalida
                        FROM Asistencia a
                        INNER JOIN Empleados e ON a.idEmpleado = e.idEmpleado";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                gvAsistencia.DataSource = dt;
                gvAsistencia.DataBind();
            }
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["empresaBD"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"SELECT 
                            a.fecha AS Fecha, 
                            CONCAT(e.Nombre, ' ', e.Primer_apellido, ' ', e.Segundo_apellido) AS Empleado, 
                            a.hora_entrada AS HoraEntrada, 
                            a.hora_salida AS HoraSalida
                        FROM Asistencia a
                        INNER JOIN Empleados e ON a.idEmpleado = e.idEmpleado
                        WHERE 1=1";

                if (!string.IsNullOrEmpty(txtFechaInicio.Text))
                {
                    query += " AND a.fecha >= @FechaInicio";
                }
                if (!string.IsNullOrEmpty(txtFechaFin.Text))
                {
                    query += " AND a.fecha <= @FechaFin";
                }
                if (!string.IsNullOrEmpty(ddlEmpleado.SelectedValue))
                {
                    query += " AND e.idEmpleado = @EmpleadoId";
                }

                SqlCommand command = new SqlCommand(query, connection);

                if (!string.IsNullOrEmpty(txtFechaInicio.Text))
                {
                    DateTime fechaInicio;
                    if (DateTime.TryParse(txtFechaInicio.Text, out fechaInicio))
                    {
                        command.Parameters.AddWithValue("@FechaInicio", fechaInicio.ToString("yyyy-MM-dd"));
                    }
                }

                if (!string.IsNullOrEmpty(txtFechaFin.Text))
                {
                    DateTime fechaFin;
                    if (DateTime.TryParse(txtFechaFin.Text, out fechaFin))
                    {
                        command.Parameters.AddWithValue("@FechaFin", fechaFin.ToString("yyyy-MM-dd"));
                    }
                }

                if (!string.IsNullOrEmpty(ddlEmpleado.SelectedValue))
                {
                    command.Parameters.AddWithValue("@EmpleadoId", ddlEmpleado.SelectedValue);
                }

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                gvAsistencia.DataSource = dt;
                gvAsistencia.DataBind();
            }
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            DataTable dt = ObtenerDatosGridView();
            if (dt.Rows.Count > 0)
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add(dt, "Asistencia");
                    using (MemoryStream stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        Response.Clear();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=Asistencia.xlsx");
                        Response.BinaryWrite(stream.ToArray());
                        Response.End();
                    }
                }
            }
        }
        protected void btnExportarPDF_Click(object sender, EventArgs e)
        {
            DataTable dt = ObtenerDatosGridView();
            if (dt.Rows.Count > 0)
            {
                try
                {
                    
                    Document pdfDoc = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20);
                    PdfWriter.GetInstance(pdfDoc, new FileStream(Server.MapPath("~/Reportes/AsistenciaReporte.pdf"), FileMode.Create));
                    pdfDoc.Open();

                    
                    string imagePath = Server.MapPath("~/Images/logo.png");
                    if (File.Exists(imagePath))
                    {
                        iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(imagePath);
                        logo.ScaleToFit(80f, 80f);
                        logo.Alignment = Element.ALIGN_CENTER;
                        pdfDoc.Add(logo);
                    }

                    
                    Paragraph companyName = new Paragraph("Corporación Krasinski S.A", FontFactory.GetFont("Arial", 16, Font.BOLD));
                    companyName.Alignment = Element.ALIGN_CENTER;
                    pdfDoc.Add(companyName);

                    Paragraph reportTitle = new Paragraph("Reporte de Asistencia", FontFactory.GetFont("Arial", 14, Font.NORMAL));
                    reportTitle.Alignment = Element.ALIGN_CENTER;
                    pdfDoc.Add(reportTitle);

                    Paragraph fecha = new Paragraph($"Fecha de Generación: {DateTime.Now:dd/MM/yyyy}", FontFactory.GetFont("Arial", 10, Font.ITALIC));
                    fecha.Alignment = Element.ALIGN_CENTER;
                    pdfDoc.Add(fecha);

                    pdfDoc.Add(new Paragraph(" ")); 

                    
                    PdfPTable pdfTable = new PdfPTable(dt.Columns.Count);
                    pdfTable.WidthPercentage = 100;

                    
                    foreach (DataColumn column in dt.Columns)
                    {
                        PdfPCell headerCell = new PdfPCell(new Phrase(column.ColumnName, FontFactory.GetFont("Arial", 10, Font.BOLD)));
                        headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                        headerCell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        pdfTable.AddCell(headerCell);
                    }

                    
                    foreach (DataRow row in dt.Rows)
                    {
                        foreach (var cell in row.ItemArray)
                        {
                            PdfPCell dataCell = new PdfPCell(new Phrase(cell.ToString(), FontFactory.GetFont("Arial", 9)));
                            dataCell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pdfTable.AddCell(dataCell);
                        }
                    }

                    pdfDoc.Add(pdfTable);
                    pdfDoc.Close();

                   
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Reporte de asistencia generado en PDF.');", true);
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Error al generar el reporte PDF: {ex.Message}');", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('No hay datos disponibles para generar el reporte.');", true);
            }
        }


        private DataTable ObtenerDatosGridView()
        {
            DataTable dt = new DataTable();
            foreach (TableCell cell in gvAsistencia.HeaderRow.Cells)
            {
                dt.Columns.Add(cell.Text);
            }
            foreach (GridViewRow row in gvAsistencia.Rows)
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    dr[i] = row.Cells[i].Text;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        protected void btnSalir_Click(object sender, EventArgs e)
        {
            Response.Redirect("MenuAdministrador.aspx");
        }
    }
}
