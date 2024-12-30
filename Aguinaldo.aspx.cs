using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Data.SqlClient;
using System.Configuration;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ClosedXML.Excel;
using System.IO;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using System.Xml.Linq;
using System.Web.UI.HtmlControls;

namespace EmpresaProyecto
{
    public partial class Aguinaldo : System.Web.UI.Page
    {
        public class EmpleadoAguinaldo
        {
            public int idEmpleado { get; set; }
            public string NombreCompleto { get; set; }
            public string Identificacion { get; set; }
            public decimal TotalSalarioAnual { get; set; }
            public decimal Aguinaldo { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Administrador")
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                CargarEmpleados();

                
                int añoAnterior = DateTime.Now.Year - 1;

                
                HtmlTableCell lblDiciembreControl = lblDiciembre; 
                if (lblDiciembreControl != null)
                {
                    lblDiciembreControl.InnerText = $"Diciembre ({añoAnterior}):"; 
                }
            }
        }


        private void CargarEmpleados()
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT idEmpleado, 
                           CONCAT(Nombre, ' ', Primer_apellido, ' ', Segundo_apellido) AS NombreCompleto, 
                           identificacion, 
                           salario_base AS SalarioBase,
                           fecha_ingreso AS FechaIngreso,
                           CASE WHEN idtipo_empleado = 1 THEN 'Administrador' ELSE 'Empleado' END AS Puesto
                    FROM Empleados";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                var empleados = new List<dynamic>();

                while (reader.Read())
                {
                    empleados.Add(new
                    {
                        idEmpleado = reader["idEmpleado"],
                        NombreCompleto = reader["NombreCompleto"].ToString(),
                        Identificacion = reader["identificacion"].ToString(),
                        SalarioBase = Convert.ToDecimal(reader["SalarioBase"]),
                        FechaIngreso = Convert.ToDateTime(reader["FechaIngreso"]),
                        Puesto = reader["Puesto"].ToString()
                    });
                }

                repeaterEmpleados.DataSource = empleados;
                repeaterEmpleados.DataBind();
            }
        }
        private void CargarDatosEmpleado(int idEmpleado)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["empresaBD"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT CONCAT(Nombre, ' ', Primer_apellido, ' ', Segundo_apellido) AS NombreCompleto,
                   identificacion AS Cedula,
                   salario_base AS SalarioBase
            FROM Empleados
            WHERE idEmpleado = @idEmpleado";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@idEmpleado", idEmpleado);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Session["NombreEmpleado"] = reader["NombreCompleto"].ToString();
                    Session["CedulaEmpleado"] = reader["Cedula"].ToString();
                    Session["SalarioBaseEmpleado"] = reader["SalarioBase"] != DBNull.Value ? Convert.ToDecimal(reader["SalarioBase"]) : 0;
                }

                connection.Close();
            }
        }
        protected void btnSeleccionarEmpleado_Click(object sender, EventArgs e)
        {
            int idEmpleadoSeleccionado = 0;

            foreach (RepeaterItem item in repeaterEmpleados.Items)
            {
                CheckBox chkSelectEmployee = (CheckBox)item.FindControl("chkSelectEmployee");
                HiddenField hfIdEmpleado = (HiddenField)item.FindControl("hfIdEmpleado");

                if (chkSelectEmployee != null && chkSelectEmployee.Checked && hfIdEmpleado != null)
                {
                    idEmpleadoSeleccionado = int.Parse(hfIdEmpleado.Value);
                    Session["EmpleadoSeleccionado"] = idEmpleadoSeleccionado;

                    
                    CargarDatosEmpleado(idEmpleadoSeleccionado);

                    break;
                }
            }

            if (idEmpleadoSeleccionado != 0)
            {
                CargarSalariosAguinaldo(idEmpleadoSeleccionado);
                formSalarioMensual.Visible = true;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Seleccione un empleado.');", true);
            }
        }
        private void CargarSalariosAguinaldo(int idEmpleado)
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                
                string query = @"
            SELECT 
                DATEPART(MONTH, fecha_nomina) AS Mes, 
                DATEPART(YEAR, fecha_nomina) AS Anno,
                ISNULL(SUM(salario_bruto), 0) AS SalarioBrutoMensual
            FROM nomina
            WHERE idEmpleado = @idEmpleado 
              AND (DATEPART(YEAR, fecha_nomina) = YEAR(GETDATE()) OR DATEPART(YEAR, fecha_nomina) = YEAR(GETDATE()) - 1)
            GROUP BY DATEPART(MONTH, fecha_nomina), DATEPART(YEAR, fecha_nomina)
            ORDER BY DATEPART(YEAR, fecha_nomina), DATEPART(MONTH, fecha_nomina)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@idEmpleado", idEmpleado);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                
                LimpiarCampos();

                while (reader.Read())
                {
                    int mes = int.Parse(reader["Mes"].ToString());
                    int anno = int.Parse(reader["Anno"].ToString());
                    decimal salario = Convert.ToDecimal(reader["SalarioBrutoMensual"]);

                    
                    if (anno == DateTime.Now.Year - 1 && mes == 12) // diciembre  año anterior
                    {
                        txtDiciembre.Text = salario.ToString("F2");
                    }
                    else if (anno == DateTime.Now.Year) // meses año actual
                    {
                        switch (mes)
                        {
                            case 1: txtEnero.Text = salario.ToString("F2"); break;
                            case 2: txtFebrero.Text = salario.ToString("F2"); break;
                            case 3: txtMarzo.Text = salario.ToString("F2"); break;
                            case 4: txtAbril.Text = salario.ToString("F2"); break;
                            case 5: txtMayo.Text = salario.ToString("F2"); break;
                            case 6: txtJunio.Text = salario.ToString("F2"); break;
                            case 7: txtJulio.Text = salario.ToString("F2"); break;
                            case 8: txtAgosto.Text = salario.ToString("F2"); break;
                            case 9: txtSeptiembre.Text = salario.ToString("F2"); break;
                            case 10: txtOctubre.Text = salario.ToString("F2"); break;
                            case 11: txtNoviembre.Text = salario.ToString("F2"); break;
                        }
                    }
                }
            }
        }

        private void LimpiarCampos()
        {
            txtEnero.Text = "";
            txtFebrero.Text = "";
            txtMarzo.Text = "";
            txtAbril.Text = "";
            txtMayo.Text = "";
            txtJunio.Text = "";
            txtJulio.Text = "";
            txtAgosto.Text = "";
            txtSeptiembre.Text = "";
            txtOctubre.Text = "";
            txtNoviembre.Text = "";
            txtDiciembre.Text = "";
        }

        protected void btnCalcularAguinaldo_Click(object sender, EventArgs e)
        {
            decimal totalSalarioAnual = 0;
            int mesesConDatos = 0;

            
            foreach (TextBox mesTextBox in new[] { txtEnero, txtFebrero, txtMarzo, txtAbril, txtMayo, txtJunio, txtJulio, txtAgosto, txtSeptiembre, txtOctubre, txtNoviembre, txtDiciembre })
            {
                decimal salarioMes = ObtenerValor(mesTextBox);
                if (salarioMes > 0)
                {
                    totalSalarioAnual += salarioMes;
                    mesesConDatos++;
                }
            }

            if (totalSalarioAnual == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('No hay datos suficientes para calcular el aguinaldo.');", true);
                return;
            }

            
            decimal aguinaldo = totalSalarioAnual / 12;

            
            lblResultadoAguinaldo.Text = $"Aguinaldo Calculado: {aguinaldo.ToString("C", new System.Globalization.CultureInfo("es-CR"))}";
            lblResultadoAguinaldo.Visible = true;

            Session["AguinaldoCalculado"] = aguinaldo;
            btnGuardarAguinaldo.Visible = true;
        }


        private decimal ObtenerValor(TextBox textBox)
        {
            
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("es-CR");

            
            if (decimal.TryParse(textBox.Text.Trim(), System.Globalization.NumberStyles.Number, culture, out decimal valor))
            {
                return valor;
            }
            return 0;
        }


        protected void btnGuardarAguinaldo_Click(object sender, EventArgs e)
        {
            decimal totalSalarioAnual = 0;

            totalSalarioAnual += ObtenerValor(txtDiciembre);
            totalSalarioAnual += ObtenerValor(txtEnero);
            totalSalarioAnual += ObtenerValor(txtFebrero);
            totalSalarioAnual += ObtenerValor(txtMarzo);
            totalSalarioAnual += ObtenerValor(txtAbril);
            totalSalarioAnual += ObtenerValor(txtMayo);
            totalSalarioAnual += ObtenerValor(txtJunio);
            totalSalarioAnual += ObtenerValor(txtJulio);
            totalSalarioAnual += ObtenerValor(txtAgosto);
            totalSalarioAnual += ObtenerValor(txtSeptiembre);
            totalSalarioAnual += ObtenerValor(txtOctubre);
            totalSalarioAnual += ObtenerValor(txtNoviembre);

            decimal aguinaldo = totalSalarioAnual / 12;

            int idEmpleadoSeleccionado = (int)Session["EmpleadoSeleccionado"];
            DateTime fechaCalculo = DateTime.Now;
            int anno = fechaCalculo.Year;

            string connectionString = ConfigurationManager.ConnectionStrings["empresaBD"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    INSERT INTO Aguinaldo (idEmpleado, anno, monto_total_ingresos, monto_aguinaldo, fecha_calculo) 
                    VALUES (@idEmpleado, @anno, @montoTotalIngresos, @montoAguinaldo, @fechaCalculo)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@idEmpleado", idEmpleadoSeleccionado);
                command.Parameters.AddWithValue("@anno", anno);
                command.Parameters.AddWithValue("@montoTotalIngresos", totalSalarioAnual);
                command.Parameters.AddWithValue("@montoAguinaldo", aguinaldo);
                command.Parameters.AddWithValue("@fechaCalculo", fechaCalculo);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();

                if (rowsAffected > 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Aguinaldo guardado exitosamente.');", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Error al guardar el aguinaldo.');", true);
                }
            }
        }

        protected void btnExportarAguinaldo_Click(object sender, EventArgs e)
        {
            try
            {
                
                int idEmpleadoSeleccionado = Session["EmpleadoSeleccionado"] != null ? (int)Session["EmpleadoSeleccionado"] : 0;
                decimal aguinaldo = Session["AguinaldoCalculado"] != null ? (decimal)Session["AguinaldoCalculado"] : 0;
                string nombreEmpleado = Session["NombreEmpleado"] != null ? Session["NombreEmpleado"].ToString() : "No disponible";
                string cedulaEmpleado = Session["CedulaEmpleado"] != null ? Session["CedulaEmpleado"].ToString() : "No disponible";
                decimal salarioBase = Session["SalarioBaseEmpleado"] != null ? (decimal)Session["SalarioBaseEmpleado"] : 0;

                
                Document pdfDoc = new Document(PageSize.A4, 20, 20, 20, 20);
                PdfWriter.GetInstance(pdfDoc, new FileStream(Server.MapPath("~/Reportes/AguinaldoEmpleado.pdf"), FileMode.Create));
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

                Paragraph reportTitle = new Paragraph("Reporte de Aguinaldo", FontFactory.GetFont("Arial", 14, Font.NORMAL));
                reportTitle.Alignment = Element.ALIGN_CENTER;
                pdfDoc.Add(reportTitle);

                Paragraph fecha = new Paragraph($"Fecha de Generación: {DateTime.Now:dd/MM/yyyy}", FontFactory.GetFont("Arial", 10, Font.ITALIC));
                fecha.Alignment = Element.ALIGN_CENTER;
                pdfDoc.Add(fecha);
                pdfDoc.Add(new Paragraph(" "));

                
                pdfDoc.Add(new Paragraph($"Nombre: {nombreEmpleado}", FontFactory.GetFont("Arial", 12)));
                pdfDoc.Add(new Paragraph($"Cédula: {cedulaEmpleado}", FontFactory.GetFont("Arial", 12)));
                pdfDoc.Add(new Paragraph(" "));

               
                PdfPTable table = new PdfPTable(2);
                table.WidthPercentage = 100;

                PdfPCell cell = new PdfPCell(new Phrase("Componentes salariales"));
                cell.Colspan = 2;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table.AddCell(cell);

                
                string formatoMoneda = "N2"; //  formato 0,000.00 

                table.AddCell("Salario Base:");
                table.AddCell(salarioBase.ToString(formatoMoneda, new System.Globalization.CultureInfo("es-CR")));

                table.AddCell("Aguinaldo:");
                table.AddCell(aguinaldo.ToString(formatoMoneda, new System.Globalization.CultureInfo("es-CR")));

                pdfDoc.Add(table);

               
                pdfDoc.Close();

                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Aguinaldo exportado en PDF con éxito.');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Error al exportar el PDF: {ex.Message}');", true);
            }
        }


        protected void btnSalir_Click(object sender, EventArgs e)
        {
            
            if (Session["UserID"] != null)
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