using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Globalization;
using System.Text;

namespace EmpresaProyecto
{
    public partial class RegistroAsistencia : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] != null)
                {
                    int empleadoId = Convert.ToInt32(Session["UserID"]);
                    lblNombreEmpleado.Text = "Nombre: " + ObtenerNombreEmpleado(empleadoId);
                    lblEstado.Text = "Estado: " + ObtenerEstadoEmpleado(empleadoId);
                    CargarRegistros(empleadoId);
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }
            }
        }

        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        private string ObtenerNombreEmpleado(int empleadoId)
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT CONCAT(nombre, ' ', primer_apellido, ' ', segundo_apellido) FROM Empleados WHERE idEmpleado = @idEmpleado";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@idEmpleado", empleadoId);
                connection.Open();
                return command.ExecuteScalar()?.ToString() ?? "Desconocido";
            }
        }

        private string ObtenerEstadoEmpleado(int empleadoId)
        {
            
            return "Fuera de horario"; 
        }

        private void CargarRegistros(int empleadoId)
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT idAsistencia, fecha, hora_entrada, hora_salida FROM Asistencia WHERE idEmpleado = @idEmpleado ORDER BY idAsistencia ASC";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@idEmpleado", empleadoId);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                gvRegistros.DataSource = dt;
                gvRegistros.DataBind();
            }
        }

        protected void btnEntrada_Click(object sender, EventArgs e)
        {
            RegistrarAsistencia("Entrada");
        }

        protected void btnSalida_Click(object sender, EventArgs e)
        {
            RegistrarAsistencia("Salida");
        }

        private void RegistrarAsistencia(string tipo)
        {
            string connectionString = "Data Source=PHIBI\\SQLEXPRESSS;Initial Catalog=empresaBD;Integrated Security=True";
            int empleadoId = Convert.ToInt32(Session["UserID"]);
            DateTime fechaActual = DateTime.Now.Date;

            
            string diaActual = RemoveDiacritics(DateTime.Now.ToString("dddd", new CultureInfo("es-ES"))).ToUpper().Trim();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

               
                string queryHorario = @"
                  SELECT horaEntrada, horaSalida 
                  FROM Horarios 
                  WHERE idEmpleado = @idEmpleado AND UPPER(diaSemana) = @diaSemana";

                SqlCommand commandHorario = new SqlCommand(queryHorario, connection);
                commandHorario.Parameters.AddWithValue("@idEmpleado", empleadoId);
                commandHorario.Parameters.AddWithValue("@diaSemana", diaActual);

                SqlDataReader readerHorario = commandHorario.ExecuteReader();

                if (!readerHorario.Read())
                {
                    MostrarPopup($"No se encontró un horario asignado para el día: {diaActual}. Contacte al administrador.");
                    readerHorario.Close();
                    return;
                }

                TimeSpan horaEntradaHorario = (TimeSpan)readerHorario["horaEntrada"];
                TimeSpan horaSalidaHorario = (TimeSpan)readerHorario["horaSalida"];
                readerHorario.Close();

                
                string queryVerificar = "SELECT hora_entrada, hora_salida FROM Asistencia WHERE idEmpleado = @idEmpleado AND fecha = @fecha";
                SqlCommand verificarCommand = new SqlCommand(queryVerificar, connection);
                verificarCommand.Parameters.AddWithValue("@idEmpleado", empleadoId);
                verificarCommand.Parameters.AddWithValue("@fecha", fechaActual);

                SqlDataReader readerAsistencia = verificarCommand.ExecuteReader();

                TimeSpan? horaEntradaAsistencia = null;
                TimeSpan? horaSalidaAsistencia = null;

                if (readerAsistencia.Read())
                {
                    horaEntradaAsistencia = readerAsistencia["hora_entrada"] != DBNull.Value ? (TimeSpan?)readerAsistencia["hora_entrada"] : null;
                    horaSalidaAsistencia = readerAsistencia["hora_salida"] != DBNull.Value ? (TimeSpan?)readerAsistencia["hora_salida"] : null;
                }
                readerAsistencia.Close();

               
                if (tipo == "Entrada")
                {
                    if (horaEntradaAsistencia.HasValue)
                    {
                        MostrarPopup("Ya se ha registrado una entrada para el día de hoy.");
                        return;
                    }

                    string queryRegistrarEntrada = "INSERT INTO Asistencia (fecha, hora_entrada, idEmpleado) VALUES (@fecha, @hora, @idEmpleado)";
                    SqlCommand commandRegistrarEntrada = new SqlCommand(queryRegistrarEntrada, connection);
                    commandRegistrarEntrada.Parameters.AddWithValue("@fecha", fechaActual);
                    commandRegistrarEntrada.Parameters.AddWithValue("@hora", DateTime.Now.TimeOfDay);
                    commandRegistrarEntrada.Parameters.AddWithValue("@idEmpleado", empleadoId);
                    commandRegistrarEntrada.ExecuteNonQuery();

                    MostrarPopup("Entrada registrada exitosamente.");
                }
                else if (tipo == "Salida")
                {
                    if (!horaEntradaAsistencia.HasValue)
                    {
                        MostrarPopup("No se puede registrar una salida sin una entrada previa.");
                        return;
                    }

                    if (horaSalidaAsistencia.HasValue)
                    {
                        MostrarPopup("Ya se ha registrado una salida para el día de hoy.");
                        return;
                    }

                    string queryRegistrarSalida = "UPDATE Asistencia SET hora_salida = @hora WHERE idEmpleado = @idEmpleado AND fecha = @fecha";
                    SqlCommand commandRegistrarSalida = new SqlCommand(queryRegistrarSalida, connection);
                    commandRegistrarSalida.Parameters.AddWithValue("@hora", DateTime.Now.TimeOfDay);
                    commandRegistrarSalida.Parameters.AddWithValue("@idEmpleado", empleadoId);
                    commandRegistrarSalida.Parameters.AddWithValue("@fecha", fechaActual);
                    commandRegistrarSalida.ExecuteNonQuery();

                    MostrarPopup("Salida registrada exitosamente.");
                }

                CargarRegistros(empleadoId);
            }
        }

       
        private void MostrarPopup(string mensaje)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "Popup", $"alert('{mensaje}');", true);
        }

        protected void btnSalir_Click(object sender, EventArgs e)
        {
            Response.Redirect("MenuEmpleados.aspx");
        }
    }
}
