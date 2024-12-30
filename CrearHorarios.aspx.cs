using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Globalization;
using System.Configuration;


namespace EmpresaProyecto
{
    public partial class CrearHorarios : System.Web.UI.Page
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
                ddlDiaLibre.Visible = false; 
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

        protected void btnCrearHorario_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlEmpleado.SelectedValue))
            {
                
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Por favor, seleccione un empleado.');", true);
                return;
            }

            
            ddlDiaLibre.Visible = true;

            
            List<string> diasSemana = new List<string> { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };
            string diaLibre = ddlDiaLibre.SelectedValue;
            diasSemana.Remove(diaLibre);

            DataTable horarios = new DataTable();
            horarios.Columns.Add("diaSemana");
            horarios.Columns.Add("horaEntrada");
            horarios.Columns.Add("horaSalida");

            foreach (string dia in diasSemana)
            {
                DataRow row = horarios.NewRow();
                row["diaSemana"] = dia;
                row["horaEntrada"] = "";
                row["horaSalida"] = "";
                horarios.Rows.Add(row);
            }

            gvHorarios.DataSource = horarios;
            gvHorarios.DataBind();
            gvHorarios.Visible = true;
            guardarGroup.Visible = true;
        

        }

        private void CargarTablaHorarios()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("diaSemana");
            dt.Columns.Add("horaEntrada");
            dt.Columns.Add("horaSalida");

            string[] dias = { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };
            foreach (string dia in dias)
            {
                DataRow row = dt.NewRow();
                row["diaSemana"] = dia;
                dt.Rows.Add(row);
            }

            gvHorarios.DataSource = dt;
            gvHorarios.DataBind();
        }

        protected void btnGuardarHorario_Click(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["empresaBD"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                
                string diaLibre = ddlDiaLibre.SelectedValue;

                
                foreach (GridViewRow row in gvHorarios.Rows)
                {
                    string diaSemana = row.Cells[0].Text;

                    
                    if (diaSemana == diaLibre)
                        continue;

                    string horaEntrada = ((TextBox)row.Cells[1].FindControl("txtHoraEntrada")).Text;
                    string horaSalida = ((TextBox)row.Cells[2].FindControl("txtHoraSalida")).Text;

                    string query = @"INSERT INTO Horarios (idEmpleado, diaSemana, horaEntrada, horaSalida)
                             VALUES (@idEmpleado, @diaSemana, @horaEntrada, @horaSalida)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@idEmpleado", ddlEmpleado.SelectedValue);
                    command.Parameters.AddWithValue("@diaSemana", diaSemana);
                    command.Parameters.AddWithValue("@horaEntrada", string.IsNullOrEmpty(horaEntrada) ? DBNull.Value : (object)horaEntrada);
                    command.Parameters.AddWithValue("@horaSalida", string.IsNullOrEmpty(horaSalida) ? DBNull.Value : (object)horaSalida);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Horario guardado exitosamente.');", true);
        }

        protected void btnConsultarHorario_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlEmpleado.SelectedValue))
            {
               
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Por favor, seleccione un empleado.');", true);
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["empresaBD"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT diaSemana, horaEntrada, horaSalida FROM Horarios WHERE idEmpleado = @idEmpleado";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@idEmpleado", ddlEmpleado.SelectedValue);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    
                    gvHorarios.DataSource = dt;
                    gvHorarios.DataBind();
                    gvHorarios.Visible = true;
                }
                else
                {
                    
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('El empleado seleccionado no tiene horarios registrados.');", true);
                    gvHorarios.Visible = false;
                }
                

            }
        }
        protected void btnCambiarHorario_Click(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["empresaBD"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT diaSemana, horaEntrada, horaSalida FROM Horarios WHERE idEmpleado = @idEmpleado";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@idEmpleado", ddlEmpleado.SelectedValue);

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                gvHorarios.DataSource = dt;
                gvHorarios.DataBind();
                gvHorarios.Visible = true;
                guardarGroup.Visible = true;
            }
        }

        protected void btnSalir_Click(object sender, EventArgs e)
        {
            Response.Redirect("MenuAdministrador.aspx");
        }
    }
}
