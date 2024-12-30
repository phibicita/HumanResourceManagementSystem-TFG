<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CrearHorarios.aspx.cs" Inherits="EmpresaProyecto.CrearHorarios" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Crear Horarios</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f5f5f5;
            margin: 0;
            padding: 20px;
        }
        .container {
            max-width: 1200px;
            margin: auto;
            background: #fff7f0;
            padding: 20px;
            border-radius: 5px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        }
        h1 {
            color: #ffa726;
            text-align: center;
        }
        .filter-group {
            display: flex;
            flex-wrap: wrap;
            gap: 15px;
            margin-bottom: 20px;
        }
        label {
            font-weight: bold;
            color: #ff7043;
        }
        input, select {
            width: 100%;
            padding: 8px;
            margin-top: 5px;
            border: 1px solid #ccc;
            border-radius: 4px;
        }
        .btn-group {
            text-align: center;
            margin-top: 20px;
        }
        .btn {
          background-color: #ffa726;
          color: white;
          padding: 8px 16px; 
          margin: 5px;
          border: none;
          border-radius: 4px;
          cursor: pointer;
          font-weight: bold;
          width: auto; 
          display: inline-block;
         }

            .btn:hover {
                background-color: #fb8c00;
            }

        table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }

            table th, table td {
                border: 1px solid #ddd;
                padding: 8px;
                text-align: left;
            }

            table th {
                background-color: #ffa726;
                color: white;
            }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>Crear Horarios</h1>
            <div class="filter-group">
                <div>
                    <label for="ddlEmpleado">Seleccione Empleado:</label>
                    <asp:DropDownList ID="ddlEmpleado" runat="server"></asp:DropDownList>
                </div>
                <div>
                    <label for="ddlDiaLibre">Día Libre:</label>
                    <asp:DropDownList ID="ddlDiaLibre" runat="server" Visible="false">
                        <asp:ListItem Value="Lunes">Lunes</asp:ListItem>
                        <asp:ListItem Value="Martes">Martes</asp:ListItem>
                        <asp:ListItem Value="Miercoles">Miercoles</asp:ListItem>
                        <asp:ListItem Value="Jueves">Jueves</asp:ListItem>
                        <asp:ListItem Value="Viernes">Viernes</asp:ListItem>
                        <asp:ListItem Value="Sabado">Sabado</asp:ListItem>
                        <asp:ListItem Value="Domingo">Domingo</asp:ListItem>
                    </asp:DropDownList>

                </div>

            </div>
            <div class="btn-group">
                <asp:Button ID="btnConsultarHorario" runat="server" Text="Consultar Horario" OnClick="btnConsultarHorario_Click" CssClass="btn btn-primary" />
                <asp:Button ID="btnCrearHorario" runat="server" Text="Crear Horario" CssClass="btn" OnClick="btnCrearHorario_Click" />
                <asp:Button ID="btnCambiarHorario" runat="server" Text="Cambiar Horario" CssClass="btn" OnClick="btnCambiarHorario_Click" />
                <asp:Button ID="btnSalir" runat="server" Text="Salir" CssClass="btn" OnClick="btnSalir_Click" />
            </div>
            <asp:GridView ID="gvHorarios" runat="server" CssClass="table" AutoGenerateColumns="False" Visible="false">
                <Columns>
                    <asp:BoundField DataField="diaSemana" HeaderText="Día" />
                    <asp:TemplateField HeaderText="Hora Entrada">
                        <ItemTemplate>
                            <asp:TextBox ID="txtHoraEntrada" runat="server" Text='<%# Bind("horaEntrada") %>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Hora Salida">
                        <ItemTemplate>
                            <asp:TextBox ID="txtHoraSalida" runat="server" Text='<%# Bind("horaSalida") %>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <div class="btn-group" id="guardarGroup" runat="server" visible="false">
                <asp:Button ID="btnGuardarHorario" runat="server" Text="Guardar" CssClass="btn" OnClick="btnGuardarHorario_Click" />
            </div>
        </div>
    </form>
</body>
</html>
