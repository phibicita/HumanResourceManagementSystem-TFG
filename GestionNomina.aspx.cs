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
using System;
using System.Windows;
using System.Text.RegularExpressions;



namespace EmpresaProyecto
{
    public partial class GestionNomina : System.Web.UI.Page
    {
        private const int JORNADA_LABORAL_MENSUAL = 240; // horas laborales por mes

        public class NominaEmpleado
        {
            public int idEmpleado { get; set; }
            public string NombreCompleto { get; set; }
            public string Identificacion { get; set; }
            public int HorasExtra { get; set; }
            public decimal SalarioBase { get; set; }
            public decimal CCSS { get; set; }
            public decimal FCL { get; set; }
            public decimal IVM { get; set; }
            public decimal Renta { get; set; }
            public decimal TotalDeducciones { get; set; }
            public decimal SalarioNeto { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Administrador")
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                MostrarQuincenaActual(); 
                CargarNomina(); 
            }
        }


        private void MostrarQuincenaActual()
        {
            DateTime fechaActual = DateTime.Now;
            string quincenaActual = fechaActual.Day <= 15 ? "Primera Quincena" : "Segunda Quincena";

            
            lblQuincenaActual.Text = $"Quincena Actual: {quincenaActual} ({fechaActual.ToString("MMMM yyyy")})";
        }

        private void CargarNomina()
        {
           
            CargarNomina(new List<int>());
        }

        private void CargarNomina(List<int> employeeIds = null)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["empresaBD"].ConnectionString;
            List<NominaEmpleado> nominaData = new List<NominaEmpleado>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = (employeeIds != null && employeeIds.Count > 0)
                    ? $@"
            SELECT e.idEmpleado, e.identificacion, e.Nombre, e.Primer_apellido, e.Segundo_apellido, 
                   e.salario_base, ISNULL(SUM(he.cantidad_horas), 0) AS HorasExtra
            FROM Empleados e
            LEFT JOIN horas_extra he ON e.idEmpleado = he.idEmpleado AND he.idestado_horasExtra = 
                 (SELECT idestado_horasExtra FROM estado_horasExtra WHERE estado = 'APROBADA')
            WHERE e.idEmpleado IN ({string.Join(",", employeeIds)})
            GROUP BY e.idEmpleado, e.Nombre, e.identificacion, e.Primer_apellido, e.Segundo_apellido, e.salario_base"
                    : @"
            SELECT e.idEmpleado, e.identificacion, e.Nombre, e.Primer_apellido, e.Segundo_apellido, 
                   e.salario_base, ISNULL(SUM(he.cantidad_horas), 0) AS HorasExtra
            FROM Empleados e
            LEFT JOIN horas_extra he ON e.idEmpleado = he.idEmpleado AND he.idestado_horasExtra = 
                 (SELECT idestado_horasExtra FROM estado_horasExtra WHERE estado = 'APROBADA')
            GROUP BY e.idEmpleado, e.Nombre, e.identificacion, e.Primer_apellido, e.Segundo_apellido, e.salario_base";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    decimal salarioBaseMensual = Convert.ToDecimal(reader["salario_base"]);
                    decimal salarioBaseQuincenal = salarioBaseMensual / 2;

                    
                    int horasExtra = Convert.ToInt32(reader["HorasExtra"]);
                    decimal pagoHorasExtra = horasExtra * (salarioBaseQuincenal / JORNADA_LABORAL_MENSUAL); 
                    salarioBaseQuincenal += pagoHorasExtra;

                    
                    decimal ccssPercentage = 0.055m; 
                    decimal ivmPercentage = 0.0267m; 
                    decimal fclPercentage = 0.01m; 

                    if (!decimal.TryParse(txtCCSS.Text, out ccssPercentage))
                    {
                        ccssPercentage = 0.055m; 
                    }

                    if (!decimal.TryParse(txtIVM.Text, out ivmPercentage))
                    {
                        ivmPercentage = 0.0267m;
                    }

                    if (!decimal.TryParse(txtFCL.Text, out fclPercentage))
                    {
                        fclPercentage = 0.01m; 
                    }

                    //  deducciones
                    decimal ccss = salarioBaseQuincenal * ccssPercentage;
                    decimal ivm = salarioBaseQuincenal * ivmPercentage;
                    decimal fcl = salarioBaseQuincenal * fclPercentage;
                    decimal renta = CalcularRenta(salarioBaseQuincenal);

                    decimal totalDeducciones = ccss + ivm + fcl + renta;
                    decimal salarioNeto = salarioBaseQuincenal - totalDeducciones;

                    
                    nominaData.Add(new NominaEmpleado
                    {
                        idEmpleado = Convert.ToInt32(reader["idEmpleado"]),
                        Identificacion = reader["identificacion"].ToString(),
                        NombreCompleto = $"{reader["Nombre"]} {reader["Primer_apellido"]} {reader["Segundo_apellido"]}",
                        HorasExtra = horasExtra,
                        SalarioBase = salarioBaseQuincenal, 
                        CCSS = ccss,
                        FCL = fcl,
                        IVM = ivm,
                        Renta = renta,
                        TotalDeducciones = totalDeducciones,
                        SalarioNeto = salarioNeto
                    });
                }

                connection.Close();
            }

            repeaterNomina.DataSource = nominaData;
            repeaterNomina.DataBind();
            Session["NominaData"] = nominaData;
        }

        protected void btnCalcularNomina_Click(object sender, EventArgs e)
        {
            var selectedEmployees = new List<int>();

            
            CheckBox chkSelectAll = (CheckBox)FindControl("chkSelectAll"); 
            if (chkSelectAll != null && chkSelectAll.Checked)
            {
                foreach (RepeaterItem item in repeaterNomina.Items)
                {
                    HiddenField hfEmpleadoId = (HiddenField)item.FindControl("hfEmpleadoId");
                    if (hfEmpleadoId != null)
                    {
                        selectedEmployees.Add(int.Parse(hfEmpleadoId.Value));
                    }
                }
            }
            else
            {
                
                foreach (RepeaterItem item in repeaterNomina.Items)
                {
                    CheckBox chkEmployee = (CheckBox)item.FindControl("chkEmployee");
                    HiddenField hfEmpleadoId = (HiddenField)item.FindControl("hfEmpleadoId");

                    if (chkEmployee != null && chkEmployee.Checked && hfEmpleadoId != null)
                    {
                        selectedEmployees.Add(int.Parse(hfEmpleadoId.Value));
                    }
                }
            }

            if (selectedEmployees.Count > 0)
            {
                CargarNomina(selectedEmployees);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Por favor, seleccione al menos un empleado.');", true);
            }
        }

        protected void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedEmployees = new List<int>();

                foreach (RepeaterItem item in repeaterNomina.Items)
                {
                    CheckBox chkEmployee = (CheckBox)item.FindControl("chkEmployee");
                    HiddenField hfEmpleadoId = (HiddenField)item.FindControl("hfEmpleadoId");

                    if (chkEmployee != null && chkEmployee.Checked && hfEmpleadoId != null)
                    {
                        selectedEmployees.Add(int.Parse(hfEmpleadoId.Value));
                    }
                }

                var nominaData = Session["NominaData"] as List<NominaEmpleado>;

                if (nominaData == null || !nominaData.Any())
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('No se encontraron datos para generar el reporte.');", true);
                    return;
                }

                var dataToExport = selectedEmployees.Count > 0
                    ? nominaData.Where(n => selectedEmployees.Contains(n.idEmpleado)).ToList()
                    : nominaData;

                
                Document pdfDoc = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20);
                PdfWriter.GetInstance(pdfDoc, new FileStream(Server.MapPath("~/Reportes/NominaReporteQuincenal.pdf"), FileMode.Create));
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

                Paragraph reportTitle = new Paragraph("Reporte de Nómina Quincenal", FontFactory.GetFont("Arial", 14, Font.NORMAL));
                reportTitle.Alignment = Element.ALIGN_CENTER;
                pdfDoc.Add(reportTitle);

                Paragraph fecha = new Paragraph($"Fecha de Generación: {DateTime.Now:dd/MM/yyyy}", FontFactory.GetFont("Arial", 10, Font.ITALIC));
                fecha.Alignment = Element.ALIGN_CENTER;
                pdfDoc.Add(fecha);

                pdfDoc.Add(new Paragraph(" ")); 

                
                PdfPTable table = new PdfPTable(10);
                table.WidthPercentage = 100;

                
                float[] columnWidths = { 12f, 18f, 10f, 12f, 12f, 12f, 12f, 12f, 15f, 15f };
                table.SetWidths(columnWidths);

                
                string[] headers = {
            "Identificación", "Nombre Completo", "Horas Extra", "Salario Base",
            "CCSS", "FCL", "IVM", "Renta", "Total Deducciones", "Salario Neto"
        };

                foreach (string header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                }

                
                string formatoMoneda = "N2";
                var culture = new System.Globalization.CultureInfo("es-CR");

                foreach (var item in dataToExport)
                {
                    
                    table.AddCell(new PdfPCell(new Phrase(item.Identificacion, FontFactory.GetFont("Arial", 9))) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(item.NombreCompleto, FontFactory.GetFont("Arial", 9))) { HorizontalAlignment = Element.ALIGN_LEFT });
                    table.AddCell(new PdfPCell(new Phrase(item.HorasExtra.ToString(), FontFactory.GetFont("Arial", 9))) { HorizontalAlignment = Element.ALIGN_CENTER });

                    
                    table.AddCell(new PdfPCell(new Phrase(item.SalarioBase.ToString(formatoMoneda, culture), FontFactory.GetFont("Arial", 9))) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(item.CCSS.ToString(formatoMoneda, culture), FontFactory.GetFont("Arial", 9))) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(item.FCL.ToString(formatoMoneda, culture), FontFactory.GetFont("Arial", 9))) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(item.IVM.ToString(formatoMoneda, culture), FontFactory.GetFont("Arial", 9))) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(item.Renta.ToString(formatoMoneda, culture), FontFactory.GetFont("Arial", 9))) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(item.TotalDeducciones.ToString(formatoMoneda, culture), FontFactory.GetFont("Arial", 9))) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(item.SalarioNeto.ToString(formatoMoneda, culture), FontFactory.GetFont("Arial", 9))) { HorizontalAlignment = Element.ALIGN_CENTER });
                }


                pdfDoc.Add(table);
                pdfDoc.Close();

                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Reporte de nómina quincenal generado en PDF.');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Error al generar el reporte PDF: {ex.Message}');", true);
            }
        }


        protected void btnAgregarEmpleado_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string primerApellido = txtPrimerApellido.Text.Trim();
            string segundoApellido = txtSegundoApellido.Text.Trim();
            string identificacion = txtIdentificacion.Text.Trim();
            decimal salarioBase;
            int genero, tipoEmpleado, estado;
            DateTime fechaIngreso;

            
            if (!Regex.IsMatch(nombre, @"^[a-zA-Z\s]+$") ||
                !Regex.IsMatch(primerApellido, @"^[a-zA-Z\s]+$") ||
                !Regex.IsMatch(segundoApellido, @"^[a-zA-Z\s]+$"))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('El nombre y apellidos solo pueden contener letras y espacios.');", true);
                return;
            }

            
            if (!Regex.IsMatch(identificacion, @"^\d+$"))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('La identificación solo puede contener números.');", true);
                return;
            }

           
            if (decimal.TryParse(txtSalarioBase.Text, out salarioBase) &&
                int.TryParse(ddlGenero.SelectedValue, out genero) &&
                int.TryParse(ddlTipoEmpleado.SelectedValue, out tipoEmpleado) &&
                int.TryParse(ddlEstado.SelectedValue, out estado) &&
                DateTime.TryParse(txtFechaIngreso.Text, out fechaIngreso))
            {
                
                string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Empleados (identificacion, Nombre, Primer_apellido, Segundo_apellido, 
                             salario_base, idGenero, idtipo_empleado, idEstado, fecha_ingreso)
                             VALUES (@Identificacion, @Nombre, @PrimerApellido, @SegundoApellido, 
                             @SalarioBase, @Genero, @TipoEmpleado, @Estado, @FechaIngreso)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Identificacion", identificacion);
                    command.Parameters.AddWithValue("@Nombre", nombre);
                    command.Parameters.AddWithValue("@PrimerApellido", primerApellido);
                    command.Parameters.AddWithValue("@SegundoApellido", segundoApellido);
                    command.Parameters.AddWithValue("@SalarioBase", salarioBase);
                    command.Parameters.AddWithValue("@Genero", genero);
                    command.Parameters.AddWithValue("@TipoEmpleado", tipoEmpleado);
                    command.Parameters.AddWithValue("@Estado", estado);
                    command.Parameters.AddWithValue("@FechaIngreso", fechaIngreso);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                
                CargarNomina();

                
                LimpiarCampos();
                pnlAgregarEmpleado.Visible = false;

                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Empleado agregado exitosamente.');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Por favor, complete todos los campos correctamente.');", true);
            }
        }

        private void LimpiarCampos()
        {
            txtIdentificacion.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtPrimerApellido.Text = string.Empty;
            txtSegundoApellido.Text = string.Empty;
            txtSalarioBase.Text = string.Empty;
            ddlGenero.SelectedIndex = 0;
            ddlTipoEmpleado.SelectedIndex = 0;
            ddlEstado.SelectedIndex = 0;
            txtFechaIngreso.Text = string.Empty;
        }

        protected void btnMostrarFormulario_Click(object sender, EventArgs e)
        {
            pnlAgregarEmpleado.Visible = true;
        }
        protected void btnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                var nominaData = Session["NominaData"] as List<NominaEmpleado>;

                if (nominaData == null || !nominaData.Any())
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('No hay datos para exportar a Excel.');", true);
                    return;
                }

                using (XLWorkbook workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Nómina");

                   
                    worksheet.Cell(1, 1).Value = "Identificación";
                    worksheet.Cell(1, 2).Value = "Nombre Completo";
                    worksheet.Cell(1, 3).Value = "Horas Extra";
                    worksheet.Cell(1, 4).Value = "Salario Base";
                    worksheet.Cell(1, 5).Value = "CCSS";
                    worksheet.Cell(1, 6).Value = "FCL";
                    worksheet.Cell(1, 7).Value = "IVM";
                    worksheet.Cell(1, 8).Value = "Renta";
                    worksheet.Cell(1, 9).Value = "Total Deducciones";
                    worksheet.Cell(1, 10).Value = "Salario Neto";

                    // Agregar datos
                    int row = 2; 
                    foreach (var item in nominaData)
                    {
                        worksheet.Cell(row, 1).Value = item.Identificacion;
                        worksheet.Cell(row, 2).Value = item.NombreCompleto;
                        worksheet.Cell(row, 3).Value = item.HorasExtra;
                        worksheet.Cell(row, 4).Value = item.SalarioBase;
                        worksheet.Cell(row, 5).Value = item.CCSS;
                        worksheet.Cell(row, 6).Value = item.FCL;
                        worksheet.Cell(row, 7).Value = item.IVM;
                        worksheet.Cell(row, 8).Value = item.Renta;
                        worksheet.Cell(row, 9).Value = item.TotalDeducciones;
                        worksheet.Cell(row, 10).Value = item.SalarioNeto;
                        row++;
                    }

                    
                    using (MemoryStream stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Position = 0;

                        Response.Clear();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=NominaReporte.xlsx");
                        Response.BinaryWrite(stream.ToArray());
                        Response.Flush();
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Error al exportar a Excel: {ex.Message}');", true);
            }
        }


        protected void btnEliminarEmpleado_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedEmployees = new List<int>();

                
                foreach (RepeaterItem item in repeaterNomina.Items)
                {
                    CheckBox chkEmployee = (CheckBox)item.FindControl("chkEmployee");
                    HiddenField hfEmpleadoId = (HiddenField)item.FindControl("hfEmpleadoId");

                    if (chkEmployee != null && chkEmployee.Checked && hfEmpleadoId != null)
                    {
                        if (int.TryParse(hfEmpleadoId.Value, out int idEmpleado))
                        {
                            selectedEmployees.Add(idEmpleado);
                        }
                    }
                }

               
                if (selectedEmployees.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Por favor, seleccione al menos un empleado.');", true);
                    return;
                }

               
                EliminarEmpleados(selectedEmployees);
                CargarNomina(); 

                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Empleado(s) eliminado(s) exitosamente.');", true);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Error al eliminar empleados: {ex.Message}');", true);
            }
        }

        private void CargarParametros()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["empresaBD"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM parametros";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    txtCCSS.Text = reader["CCSS"].ToString();
                    txtIVM.Text = reader["IVM"].ToString();
                    txtFCL.Text = reader["FCL"].ToString();
                    txtRentaTramo1.Text = reader["RentaTramo1"].ToString();
                    txtRentaTramo2.Text = reader["RentaTramo2"].ToString();
                    txtRentaTramo3.Text = reader["RentaTramo3"].ToString();
                }

                connection.Close();
            }
        }

        
        protected void btnGuardarParametros_Click(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["empresaBD"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
            UPDATE parametros 
            SET CCSS = @CCSS, IVM = @IVM, FCL = @FCL, 
                RentaTramo1 = @RentaTramo1, RentaTramo2 = @RentaTramo2, RentaTramo3 = @RentaTramo3";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CCSS", txtCCSS.Text);
                command.Parameters.AddWithValue("@IVM", txtIVM.Text);
                command.Parameters.AddWithValue("@FCL", txtFCL.Text);
                command.Parameters.AddWithValue("@RentaTramo1", txtRentaTramo1.Text);
                command.Parameters.AddWithValue("@RentaTramo2", txtRentaTramo2.Text);
                command.Parameters.AddWithValue("@RentaTramo3", txtRentaTramo3.Text);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Parámetros guardados correctamente.');", true);
            }
        }

        
        protected void btnEditarParametros_Click(object sender, EventArgs e)
        {
            pnlEditarParametros.Visible = true;
            CargarParametros();
        }

        
        private decimal CalcularRenta(decimal salarioBase)
        {
            decimal renta = 0;

            
            decimal tramo1 = 929000m;
            decimal tramo2 = 1363000m;
            decimal tramo3 = 2392000m;

           
            decimal tasaTramo1 = 0.10m; // 10%
            decimal tasaTramo2 = 0.20m; // 20%
            decimal tasaTramo3 = 0.25m; // 25%

           
            if (!decimal.TryParse(txtRentaTramo1.Text, out tramo1))
            {
                tramo1 = 929000m; 
            }

            if (!decimal.TryParse(txtRentaTramo2.Text, out tramo2))
            {
                tramo2 = 1363000m; 
            }

            if (!decimal.TryParse(txtRentaTramo3.Text, out tramo3))
            {
                tramo3 = 2392000m; 
            }

            
            if (salarioBase > tramo3)
            {
                renta += (salarioBase - tramo3) * tasaTramo3;
                salarioBase = tramo3;
            }

            if (salarioBase > tramo2)
            {
                renta += (salarioBase - tramo2) * tasaTramo2;
                salarioBase = tramo2;
            }

            if (salarioBase > tramo1)
            {
                renta += (salarioBase - tramo1) * tasaTramo1;
            }

            return renta;
        }

        protected void btnGuardarNomina_Click(object sender, EventArgs e)
        {
            var nominaData = Session["NominaData"] as List<NominaEmpleado>;

            if (nominaData == null || !nominaData.Any())
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('No hay datos para guardar en la nómina.');", true);
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["empresaBD"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                
                string quincena = DateTime.Now.Day <= 15 ? "Primera Quincena" : "Segunda Quincena";
                string mesActual = DateTime.Now.ToString("yyyy-MM");

                
                string queryValidacion = @"
            SELECT COUNT(*) 
            FROM nomina 
            WHERE quincena = @quincena 
              AND FORMAT(fecha_nomina, 'yyyy-MM') = @mesActual";

                using (SqlCommand validarCommand = new SqlCommand(queryValidacion, connection))
                {
                    validarCommand.Parameters.AddWithValue("@quincena", quincena);
                    validarCommand.Parameters.AddWithValue("@mesActual", mesActual);

                    int registrosExistentes = (int)validarCommand.ExecuteScalar();
                    if (registrosExistentes > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                            $"alert('Ya existe una nómina guardada para la {quincena} de {mesActual}.');", true);
                        return;
                    }
                }

                foreach (var empleado in nominaData)
                {
                    string query = @"
                INSERT INTO nomina (idEmpleado, fecha_nomina, salario_bruto, deducciones, salario_neto, quincena)
                VALUES (@idEmpleado, @fechaNomina, @salarioBruto, @deducciones, @salarioNeto, @quincena);";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        
                        command.Parameters.AddWithValue("@idEmpleado", empleado.idEmpleado);
                        command.Parameters.AddWithValue("@fechaNomina", DateTime.Now);
                        command.Parameters.AddWithValue("@salarioBruto", empleado.SalarioBase);
                        command.Parameters.AddWithValue("@deducciones", empleado.TotalDeducciones);
                        command.Parameters.AddWithValue("@salarioNeto", empleado.SalarioNeto);
                        command.Parameters.AddWithValue("@quincena", quincena);

                        command.ExecuteNonQuery();
                    }
                }

               
                string resetHorasQuery = "UPDATE horas_extra SET cantidad_horas = 0 WHERE idEmpleado IN (SELECT idEmpleado FROM Empleados)";
                using (SqlCommand resetCommand = new SqlCommand(resetHorasQuery, connection))
                {
                    resetCommand.ExecuteNonQuery();
                }
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Nómina guardada exitosamente y horas extras reseteadas.');", true);
        }


        private void EliminarEmpleados(List<int> employeeIds)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["empresaBD"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var empleadosSinLiquidacion = new List<string>(); 
                        var empleadosEliminados = new List<string>();     

                        foreach (int idEmpleado in employeeIds)
                        {
                            
                            string nombreEmpleado = "";
                            string getNombreQuery = @"
                        SELECT CONCAT(Nombre, ' ', Primer_apellido, ' ', Segundo_apellido) AS NombreCompleto
                        FROM Empleados WHERE idEmpleado = @idEmpleado";

                            using (SqlCommand nombreCommand = new SqlCommand(getNombreQuery, connection, transaction))
                            {
                                nombreCommand.Parameters.AddWithValue("@idEmpleado", idEmpleado);
                                nombreEmpleado = nombreCommand.ExecuteScalar()?.ToString();
                            }

                            if (string.IsNullOrEmpty(nombreEmpleado))
                            {
                                
                                continue;
                            }

                            
                            string validateLiquidacion = @"
                        SELECT COUNT(*) 
                        FROM Liquidaciones 
                        WHERE idEmpleado = @idEmpleado AND montoTotal IS NOT NULL";

                            using (SqlCommand validateCommand = new SqlCommand(validateLiquidacion, connection, transaction))
                            {
                                validateCommand.Parameters.AddWithValue("@idEmpleado", idEmpleado);
                                int liquidacionCount = (int)validateCommand.ExecuteScalar();

                                if (liquidacionCount == 0)
                                {
                                    empleadosSinLiquidacion.Add(nombreEmpleado); 
                                    continue;
                                }
                            }

                            
                            string deleteEmpleadoQuery = "DELETE FROM Empleados WHERE idEmpleado = @idEmpleado";

                            using (SqlCommand cmd = new SqlCommand(deleteEmpleadoQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@idEmpleado", idEmpleado);
                                cmd.ExecuteNonQuery();
                            }

                            empleadosEliminados.Add(nombreEmpleado); 
                        }

                        transaction.Commit();

                        
                        if (empleadosEliminados.Count > 0)
                        {
                            string mensajeEliminados = $"Empleado(s) eliminado(s): {string.Join(", ", empleadosEliminados)}";
                            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{mensajeEliminados}');", true);
                        }

                        
                        if (empleadosSinLiquidacion.Count > 0)
                        {
                            string mensajeNoEliminados = $"No se eliminaron los empleados sin liquidación válida: {string.Join(", ", empleadosSinLiquidacion)}";
                            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('{mensajeNoEliminados}');", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                            $"alert('Error al eliminar empleados: {ex.Message}');", true);
                    }
                }
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