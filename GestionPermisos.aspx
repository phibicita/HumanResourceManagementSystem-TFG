<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestionPermisos.aspx.cs" Inherits="EmpresaProyecto.GestionPermisos" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Gestión de Permisos</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f8f8f8;
        }

        .container {
            width: 90%;
            margin: auto;
            padding: 20px;
            background-color: #fff7f0;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }

        h2 {
            color: #333;
        }

        table {
            width: 100%;
            margin-top: 20px;
            border-collapse: collapse;
        }

        th, td {
            padding: 10px;
            text-align: left;
            border-bottom: 1px solid #ddd;
        }

        th {
            background-color: #f4b183;
        }

        .btn {
            padding: 10px 20px;
            margin: 5px;
            cursor: pointer;
            background-color: #4CAF50;
            color: white;
            border: none;
            border-radius: 5px;
        }

        .btn-reject {
            background-color: #f44336;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>Gestión de Permisos</h2>

            <asp:Repeater ID="repeaterPermisos" runat="server">
                <HeaderTemplate>
                    <table>
                        <tr>
                            <th>Seleccionar</th>
                            <th>Nombre Completo</th>
                            <th>Identificación</th>
                            <th>Tipo de Permiso</th>
                            <th>Fecha Permiso</th>
                            <th>Hora Inicio</th>
                            <th>Hora Fin</th>
                            <th>Estado</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkSelectPermiso" runat="server" /></td>
                        <td><%# Eval("NombreCompleto") %></td>
                        <td><%# Eval("Identificacion") %></td>
                        <td><%# Eval("TipoPermiso") %></td>
                        <td><%# Eval("FechaPermiso", "{0:dd/MM/yyyy}") %></td>
                        <td><%# Eval("HoraInicio") %></td>
                        <td><%# Eval("HoraFin") %></td>
                        <td><%# Eval("EstadoPermiso") %></td>
                        <asp:HiddenField ID="hfIdPermiso" runat="server" Value='<%# Eval("idPermiso") %>' />
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
            <div class="action-buttons">
                <asp:Button ID="btnAceptarPermiso" runat="server" Text="Aceptar Permiso" CssClass="btn" OnClick="btnAceptarPermiso_Click" />
                <asp:Button ID="btnRechazarPermiso" runat="server" Text="Rechazar Permiso" CssClass="btn" OnClick="btnRechazarPermiso_Click" />
            </div>

            <div class="exit-container">
                <asp:Button ID="btnSalir" runat="server" Text="Salir" CssClass="exit-button" OnClick="btnSalir_Click" />
            </div>

        </div>
    </form>
</body>
</html>
