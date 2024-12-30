using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ClosedXML.Excel;
using System.IO;

namespace EmpresaProyecto
{
    public partial class Liquidacion : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Administrador")
            {
                
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Acceso denegado. Solo los administradores pueden acceder a esta página.');", true);
                Response.Redirect("Login.aspx");
                return;
            }

          
            if (!IsPostBack)
            {
                CargarEmpleados();
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
                        NombreCompleto = reader["NombreCompleto"],
                        Identificacion = reader["identificacion"],
                        SalarioBase = reader["SalarioBase"],
                        FechaIngreso = reader["FechaIngreso"],
                        Puesto = reader["Puesto"]
                    });
                }

                repeaterEmpleados.DataSource = empleados;
                repeaterEmpleados.DataBind();
            }
        }

        protected void btnCalcularLiquidacion_Click(object sender, EventArgs e)
        {
            int idEmpleadoSeleccionado = 0;


            foreach (RepeaterItem item in repeaterEmpleados.Items)
            {
                CheckBox chkSelectEmployee = (CheckBox)item.FindControl("chkSelectEmployee");
                HiddenField hfIdEmpleado = (HiddenField)item.FindControl("hfIdEmpleado");

                if (chkSelectEmployee != null && chkSelectEmployee.Checked && hfIdEmpleado != null)
                {
                    idEmpleadoSeleccionado = int.Parse(hfIdEmpleado.Value);
                    break;
                }
            }
            int seleccionados = 0;

            foreach (RepeaterItem item in repeaterEmpleados.Items)
            {
                CheckBox chkSelectEmployee = (CheckBox)item.FindControl("chkSelectEmployee");

                if (chkSelectEmployee != null && chkSelectEmployee.Checked)
                {
                    seleccionados++;
                }

                if (seleccionados > 1)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Solo se puede realizar una liquidación por vez.');", true);
                    return;
                }
            }


            if (idEmpleadoSeleccionado == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Por favor, seleccione un empleado.');", true);
                return;
            }

            decimal salarioBase = 0;
            DateTime fechaIngreso = DateTime.Now;

            string connectionString = ConfigurationManager.ConnectionStrings["empresaBD"].ConnectionString;

            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string queryEmpleado = @"SELECT salario_base, fecha_ingreso FROM Empleados WHERE idEmpleado = @idEmpleado";
                SqlCommand commandEmpleado = new SqlCommand(queryEmpleado, connection);
                commandEmpleado.Parameters.AddWithValue("@idEmpleado", idEmpleadoSeleccionado);

                connection.Open();
                SqlDataReader reader = commandEmpleado.ExecuteReader();

                if (reader.Read())
                {
                    salarioBase = Convert.ToDecimal(reader["salario_base"]);
                    fechaIngreso = Convert.ToDateTime(reader["fecha_ingreso"]);
                }
                connection.Close();
            }

            
            int mesesTrabajados = ((DateTime.Now.Year - fechaIngreso.Year) * 12) + DateTime.Now.Month - fechaIngreso.Month;

            if (mesesTrabajados < 3)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('No se puede realizar la liquidación porque el empleado tiene menos de 3 meses de antigüedad.');", true);
                return;
            }


           
            int diasAcumulados = (int)((DateTime.Now - fechaIngreso).Days / 350.0 * 14); 
            int diasAprobados = 0;

            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string queryVacaciones = @"
                  SELECT ISNULL(SUM(dias_disponibles), 0) AS DiasAprobados 
                  FROM Vacaciones 
                  WHERE idEmpleado = @idEmpleado 
                  AND idEstadoVacaciones = 2 
                  AND fecha_inicio <= GETDATE()"; 

                SqlCommand commandVacaciones = new SqlCommand(queryVacaciones, connection);
                commandVacaciones.Parameters.AddWithValue("@idEmpleado", idEmpleadoSeleccionado);

                connection.Open();
                object result = commandVacaciones.ExecuteScalar();
                diasAprobados = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                connection.Close();
            }

            
            int diasPendientes = Math.Max(diasAcumulados - diasAprobados, 0);

            
            bool tieneVacacionesFuturas = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string queryFuturas = @"
                 SELECT COUNT(1) 
                 FROM Vacaciones 
                 WHERE idEmpleado = @idEmpleado 
                 AND idEstadoVacaciones = 2 
                 AND fecha_inicio > GETDATE()";

                SqlCommand commandFuturas = new SqlCommand(queryFuturas, connection);
                commandFuturas.Parameters.AddWithValue("@idEmpleado", idEmpleadoSeleccionado);

                connection.Open();
                tieneVacacionesFuturas = (int)commandFuturas.ExecuteScalar() > 0;
                connection.Close();
            }

            if (tieneVacacionesFuturas)
            {
                diasPendientes = 0; 
            }

            
            decimal vacacionesPendientes = 0;
            if (diasPendientes > 0)
            {
                vacacionesPendientes = salarioBase / 30 * diasPendientes;
            }

           
            int diasTrabajadosMesActual = DateTime.Now.Day; 
            decimal salariosPendientes = salarioBase / 30 * diasTrabajadosMesActual;

            
            decimal aguinaldoProporcional = salarioBase / 12; 
            decimal cesantia = 0;
            decimal preaviso = 0;
            decimal montoTotal = 0;

           
            txtVacacionesPendientes.Text = vacacionesPendientes.ToString("C");
            txtSalariosPendientes.Text = salariosPendientes.ToString("C");
            txtAguinaldoProporcional.Text = aguinaldoProporcional.ToString("C");

           
            string tipoLiquidacion = ddlTipoLiquidacion.SelectedValue;

            switch (tipoLiquidacion)
            {
                case "1": // despido con causa
                    montoTotal = salariosPendientes + vacacionesPendientes + aguinaldoProporcional;
                    break;

                case "2": // despido sin causa
                    preaviso = CalcularPreaviso(fechaIngreso, salarioBase);
                    cesantia = CalcularCesantia(fechaIngreso, salarioBase);
                    montoTotal = salariosPendientes + vacacionesPendientes + aguinaldoProporcional + preaviso + cesantia;
                    txtPreaviso.Text = preaviso.ToString("C");
                    txtCesantia.Text = cesantia.ToString("C");
                    break;

                case "3": // renuncia 
                    montoTotal = salariosPendientes + vacacionesPendientes + aguinaldoProporcional;
                    break;

                default:
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Seleccione un tipo de liquidación válido.');", true);
                    return;
            }

            txtMontoTotal.Text = montoTotal.ToString("C");
        }


        
        private decimal CalcularCesantia(DateTime fechaIngreso, decimal salarioBase)
        {
            int mesesTrabajados = ((DateTime.Now.Year - fechaIngreso.Year) * 12) + DateTime.Now.Month - fechaIngreso.Month;

            if (mesesTrabajados < 3)
            {
                return 0; // n/a cesantía si tiene menos de 3 meses de servicio
            }
            else if (mesesTrabajados < 6)
            {
                return salarioBase / 30 * 7; // cesantía de 7 días para 3 a 6 meses
            }
            else if (mesesTrabajados < 12)
            {
                return salarioBase / 30 * 14; // cesantía de 14 días para 6 a 12 meses
            }
            else
            {
                return salarioBase * Math.Min(mesesTrabajados / 12, 8) * 19.5m / 30; // 19.5 días por año, hasta 8 años
            }
        }

        
        private decimal CalcularPreaviso(DateTime fechaIngreso, decimal salarioBase)
        {
            int mesesTrabajados = ((DateTime.Now.Year - fechaIngreso.Year) * 12) + DateTime.Now.Month - fechaIngreso.Month;

            if (mesesTrabajados < 6)
            {
                return salarioBase / 4; 
            }
            else if (mesesTrabajados < 12)
            {
                return salarioBase / 2; 
            }
            else
            {
                return salarioBase; // preaviso de 1 mes para más de 1 año
            }
        }

        protected void btnGuardarLiquidacion_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";

            foreach (RepeaterItem item in repeaterEmpleados.Items)
            {
                CheckBox chkSelectEmployee = (CheckBox)item.FindControl("chkSelectEmployee");
                HiddenField hfIdEmpleado = (HiddenField)item.FindControl("hfIdEmpleado");

                
                if (chkSelectEmployee != null && chkSelectEmployee.Checked && hfIdEmpleado != null)
                {
                    int idEmpleadoSeleccionado = int.Parse(hfIdEmpleado.Value);

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = @"
                         INSERT INTO Liquidaciones 
                         (idEmpleado, idtipo_liquidacion, cesantia, preaviso, vacacionesPendientes, montoTotal, fechaLiquidacion) 
                         VALUES 
                         (@idEmpleado, @tipo_liquidacion, @cesantia, @preaviso, @vacacionesPendientes, @montoTotal,  @fechaLiquidacion)";

                        SqlCommand command = new SqlCommand(query, connection);

                        
                        command.Parameters.AddWithValue("@idEmpleado", idEmpleadoSeleccionado);
                        command.Parameters.AddWithValue("@tipo_liquidacion", ddlTipoLiquidacion.SelectedValue);
                        command.Parameters.AddWithValue("@cesantia", ParseDecimal(txtCesantia.Text));
                        command.Parameters.AddWithValue("@preaviso", ParseDecimal(txtPreaviso.Text));
                        command.Parameters.AddWithValue("@fechaLiquidacion", DateTime.Now); 
                        command.Parameters.AddWithValue("@vacacionesPendientes", ParseDecimal(txtVacacionesPendientes.Text));
                        command.Parameters.AddWithValue("@montoTotal", ParseDecimal(txtMontoTotal.Text));

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }

                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Liquidación guardada con éxito.');", true);
                    return;
                }
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Por favor, seleccione un empleado.');", true);
        }
        
        private decimal ParseDecimal(string input)
        {
            try
            {
               
                if (string.IsNullOrWhiteSpace(input))
                    return 0;

                
                input = input.Replace("¢", "")
                             .Replace(",", "")
                             .Replace(" ", "")
                             .Trim();

                
                if (decimal.TryParse(input, System.Globalization.NumberStyles.Any,
                                     System.Globalization.CultureInfo.CurrentCulture, out decimal result))
                {
                    return result;
                }

                throw new FormatException($"El valor '{input}' no tiene el formato correcto.");
            }
            catch (Exception ex)
            {
                
                throw new Exception($"Error al convertir el valor '{input}': {ex.Message}");
            }
        }

        protected void btnGenerarReporteLiquidacion_Click(object sender, EventArgs e)
        {
            
            Document pdfDoc = new Document(PageSize.A4, 10, 10, 40, 10); 
            string rutaLogo = Server.MapPath("~/Images/LOGO.png"); 
            PdfWriter.GetInstance(pdfDoc, new FileStream(Server.MapPath("~/Reportes/LiquidacionReporte.pdf"), FileMode.Create));
            pdfDoc.Open();

           
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(rutaLogo);
            logo.ScaleToFit(100f, 100f); 
            logo.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(logo);

          
            Paragraph tituloEmpresa = new Paragraph("Corporación Krasinski S.A", FontFactory.GetFont("Arial", 16, Font.BOLD));
            tituloEmpresa.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(tituloEmpresa);

            Paragraph tituloReporte = new Paragraph("Reporte de Liquidación", FontFactory.GetFont("Arial", 14, Font.BOLD));
            tituloReporte.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(tituloReporte);

            
            Paragraph fecha = new Paragraph("Fecha de Generación: " + DateTime.Now.ToString("dd/MM/yyyy"), FontFactory.GetFont("Arial", 10, Font.NORMAL));
            fecha.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(fecha);

            pdfDoc.Add(new Paragraph(" ")); 

           
            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 80;
            table.HorizontalAlignment = Element.ALIGN_CENTER;

            
            PdfPCell cell1 = new PdfPCell(new Phrase("Concepto", FontFactory.GetFont("Arial", 12, Font.BOLD)));
            PdfPCell cell2 = new PdfPCell(new Phrase("Monto", FontFactory.GetFont("Arial", 12, Font.BOLD)));
            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
            cell2.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell1);
            table.AddCell(cell2);

            
            table.AddCell("Cesantía");
            table.AddCell(txtCesantia.Text);
            table.AddCell("Preaviso");
            table.AddCell(txtPreaviso.Text);
            table.AddCell("Vacaciones Pendientes");
            table.AddCell(txtVacacionesPendientes.Text);
            table.AddCell("Monto Total de Liquidación");
            table.AddCell(txtMontoTotal.Text);

            pdfDoc.Add(table);
            pdfDoc.Close();

           
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Reporte de liquidación generado en PDF.');", true);
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            
            using (XLWorkbook workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Liquidación");
                worksheet.Cell(1, 1).Value = "Concepto";
                worksheet.Cell(1, 2).Value = "Monto";
                worksheet.Cell(2, 1).Value = "Cesantía";
                worksheet.Cell(2, 2).Value = txtCesantia.Text;
                worksheet.Cell(3, 1).Value = "Preaviso";
                worksheet.Cell(3, 2).Value = txtPreaviso.Text;
                worksheet.Cell(5, 1).Value = "Vacaciones Pendientes";
                worksheet.Cell(5, 2).Value = txtVacacionesPendientes.Text;
                worksheet.Cell(6, 1).Value = "Monto Total de Liquidación";
                worksheet.Cell(6, 2).Value = txtMontoTotal.Text;

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=LiquidacionReporte.xlsx");
                    Response.BinaryWrite(stream.ToArray());
                    Response.End();
                }
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
           
            txtCesantia.Text = "";
            txtPreaviso.Text = "";
            txtVacacionesPendientes.Text = "";
            txtMontoTotal.Text = "";
            txtAguinaldoProporcional.Text = "";
            txtSalariosPendientes.Text = "";
            ddlTipoLiquidacion.SelectedIndex = 0;
        }

        protected void btnSalir_Click(object sender, EventArgs e)
        {
            Response.Redirect("MenuAdministrador.aspx");
        }
    }
}