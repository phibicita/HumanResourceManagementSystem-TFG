<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestionHorasExtra.aspx.cs" Inherits="EmpresaProyecto.GestionHorasExtra" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Gestión de Horas Extra - Administrador</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f8f8f8;
        }
        
        .container {
            width: 90%;
            margin: 0 auto;
            background-color: #fff7f0;
            padding: 20px;
            border-radius: 15px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }

        .table {
            width: 100%;
            margin-top: 20px;
            border-collapse: collapse;
        }

        .table th, .table td {
            padding: 10px;
            text-align: left;
            border-bottom: 1px solid #ddd;
        }

        .table th {
            background-color: #f4b183;
        }

        .action-buttons {
            margin-top: 20px;
            display: flex;
            justify-content: flex-end;
        }

        .action-buttons button {
            padding: 10px 20px;
            margin-left: 10px;
            border-radius: 10px;
            border: none;
            cursor: pointer;
            background-color: #FFA726;
            color: white;
            font-size: 16px;
        }

        .action-buttons button:hover {
            background-color: #FB8C00;
        }

        .exit-button {
            margin-top: 20px;
            padding: 10px 30px;
            background-color: #d9534f;
            color: white;
            font-size: 16px;
            border-radius: 5px;
        }

        .exit-button:hover {
            background-color: #c9302c;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>Gestión de Horas Extra - Administrador</h2>
               
             <div class="filter-container">
                <asp:Label ID="lblFiltrarPorEstado" runat="server" Text="Filtrar por Estado:" />
                <asp:DropDownList ID="ddlEstadoHorasExtra" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlEstadoHorasExtra_SelectedIndexChanged">
                    <asp:ListItem Text="Todos" Value="0" />
                    <asp:ListItem Text="Pendiente" Value="1" />
                    <asp:ListItem Text="Aprobada" Value="2" />
                    <asp:ListItem Text="Rechazada" Value="3" />
                </asp:DropDownList>
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" OnClick="btnFiltrar_Click" />
            </div>

            
            <asp:Repeater ID="repeaterHorasExtra" runat="server">
                <HeaderTemplate>
                    <table class="table">
                        <thead>
                            <tr>
                                <th>Seleccionar</th>
                                <th>Empleado</th>
                                <th>Identificación</th>
                                <th>Fecha</th>
                                <th>Horas Solicitadas</th>
                                <th>Estado</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><asp:CheckBox ID="chkSelectHoraExtra" runat="server" /></td>
                        <td><%# Eval("NombreCompleto") %></td>
                        <td><%# Eval("Identificacion") %></td>
                        <td><%# Eval("Fecha") %></td>
                        <td><%# Eval("HorasSolicitadas") %></td>
                        <td><%# Eval("EstadoHoraExtra") %></td>
                        <asp:HiddenField ID="hfIdHoraExtra" runat="server" Value='<%# Eval("idhora_extra") %>' />
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>

          
            <div class="action-buttons">
             <asp:Button ID="btnAceptarSolicitud" runat="server" Text="Aceptar Solicitud" OnClick="btnAceptarHoraExtra_Click" />
<asp:Button ID="btnRechazarSolicitud" runat="server" Text="Rechazar Solicitud" OnClick="btnRechazarHoraExtra_Click" />

            </div>

          
            <div class="exit-container">
                <asp:Button ID="btnSalir" runat="server" Text="Salir" CssClass="exit-button" OnClick="btnSalir_Click" />
            </div>
        </div>
    </form>
</body>
</html>

