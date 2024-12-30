<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestionIncapacidades.aspx.cs" Inherits="EmpresaProyecto.GestionIncapacidades" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Gestión de Incapacidades</title>
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

        .header-title {
            font-size: 24px;
            font-weight: bold;
            margin-bottom: 20px;
            color: #333;
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

        .btn {
            background-color: #FFA726;
            color: white;
            padding: 10px;
            border-radius: 10px;
            border: none;
            cursor: pointer;
            margin-top: 20px;
        }

            .btn:hover {
                background-color: #FB8C00;
            }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header-title">Gestión de Incapacidades</div>
            <div>
                <asp:DropDownList ID="ddlFiltroEstado" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlFiltroEstado_SelectedIndexChanged">
                    <asp:ListItem Text="Todas" Value="0"></asp:ListItem>
                    <asp:ListItem Text="Pendiente" Value="6"></asp:ListItem>
                    <asp:ListItem Text="Aprobado" Value="7"></asp:ListItem>
                </asp:DropDownList>

            </div>
            <asp:Repeater ID="repeaterIncapacidades" runat="server">
                <HeaderTemplate>
                    <table class="table">
                        <tr>
                            <th>Seleccionar</th>
                            <th>Nombre</th>
                            <th>Identificación</th>
                            <th>Fecha de Inicio</th>
                            <th>Fecha de Fin</th>
                            <th>Estado de Incapacidad</th>
                            <th>Estado del Empleado</th>
                            <th>Documento</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkSelectIncapacidad" runat="server" />
                            <asp:HiddenField ID="hfIdIncapacidad" runat="server" Value='<%# Eval("idincapacidades") %>' />
                            <asp:HiddenField ID="hfIdEmpleado" runat="server" Value='<%# Eval("idEmpleado") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblNombre" runat="server" Text='<%# Eval("NombreCompleto") %>' /></td>
                        <td>
                            <asp:Label ID="lblCedula" runat="server" Text='<%# Eval("Identificacion") %>' /></td>
                        <td>
                            <asp:Label ID="lblFechaInicio" runat="server" Text='<%# Eval("FechaInicio", "{0:dd/MM/yyyy}") %>' /></td>
                        <td>
                            <asp:Label ID="lblFechaFin" runat="server" Text='<%# Eval("FechaFin", "{0:dd/MM/yyyy}") %>' /></td>
                        <td>
                            <asp:Label ID="lblEstado" runat="server" Text='<%# Eval("EstadoIncapacidad") %>' /></td>
                        <td>
                            <asp:Label ID="lblEstadoEmpleado" runat="server" Text='<%# Eval("EstadoEmpleado") %>' /></td>
                        <td>
                            <asp:HyperLink ID="lnkDocumento" runat="server"
                                NavigateUrl='<%# Eval("DocumentoIncapacidad") %>'
                                Text="Abrir Documento"
                                Target="_blank" />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>

            <asp:Button ID="btnActualizarEstado" runat="server" Text="Actualizar Estado de Empleado" CssClass="btn" OnClick="btnActualizarEstado_Click" />

            <asp:Button ID="btnSalir" runat="server" Text="Salir" CssClass="btn" OnClick="btnSalir_Click" />

        </div>
    </form>
</body>
</html>
