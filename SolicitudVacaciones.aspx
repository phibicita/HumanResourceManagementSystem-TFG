<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SolicitudVacaciones.aspx.cs" Inherits="EmpresaProyecto.SolicitudVacaciones" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Solicitar Vacaciones</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f8f8f8;
        }
        .container {
            width: 78%;
            margin: 0 auto;
            background-color: #fff7f0;
            padding: 20px;
            border-radius: 15px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }
        .header {
            display: flex;
            justify-content: space-between;
            margin-bottom: 20px;
        }

        .date-picker {
            width: 250px;
            padding: 8px;
            font-size: 14px;
            border: 1px solid #ccc;
            border-radius: 5px;
        }

        .employee-name, .available-days {
            font-size: 18px;
            font-weight: bold;
        }

        .request-vacation-btn {
            background-color: #FFA726;
            color: white;
            padding: 10px;
            border-radius: 10px;
            border: none;
            cursor: pointer;
        }

            .request-vacation-btn:hover {
                background-color: #FB8C00;
            }

        .vacation-table {
            width: 100%;
            margin-top: 20px;
            border-collapse: collapse;
            background-color: #FFEFD5;
        }

            .vacation-table th, .vacation-table td {
                padding: 10px;
                text-align: left;
                border-bottom: 1px solid #ddd;
            }

            .vacation-table th {
                background-color: #f4b183;
            }

        .button-container {
            display: flex;
            justify-content: right;
            margin-top: 20px;
        }

        .calendar-container {
            display: flex;
            flex-direction: column;
            gap: 20px;
        }

        .date-group {
            display: flex;
            flex-direction: column;
            margin-bottom: 10px;
        }

        .action-buttons {
            display: flex;
            justify-content: space-between;
            margin-top: 20px;
        }

            .action-buttons button {
                background-color: #FFA726;
                color: white;
                padding: 10px;
                border-radius: 10px;
                border: none;
                cursor: pointer;
            }

                .action-buttons button:hover {
                    background-color: #FB8C00;
                }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header">
                <div>
                    <div class="employee-name">
                        <asp:Label ID="lblNombreEmpleado" runat="server" Text="Nombre del empleado"></asp:Label>
                    </div>
                    <div class="available-days">
                        <asp:Label ID="lblDiasDisponibles" runat="server" Text="Días disponibles: 0"></asp:Label>
                    </div>
                </div>
                <asp:Button ID="btnSolicitarVacaciones" runat="server" CssClass="request-vacation-btn" Text="Solicitar vacaciones" OnClick="btnSolicitarVacaciones_Click" />
            </div>
            <div class="button-container">
                <asp:Button ID="btnSalir" runat="server" CssClass="exit-button" Text="Salir" OnClick="btnSalir_Click" />
            </div>

            <table class="vacation-table">
                <tr>
                    <th>Fecha Inicio</th>
                    <th>Fecha Fin</th>
                    <th>Estado Solicitud</th>
                    <th>Eliminar</th>
                </tr>

                <asp:Repeater ID="repeaterVacaciones" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:Label ID="lblFechaInicio" runat="server" Text='<%# Eval("fecha_inicio", "{0:yyyy-MM-dd}") %>'></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblFechaFin" runat="server" Text='<%# Eval("fecha_fin", "{0:yyyy-MM-dd}") %>'></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lblEstadoSolicitud" runat="server" Text='<%# Eval("estado_solicitud") %>'></asp:Label>
                            </td>
                            <td>
                                <asp:Button ID="btnEliminar" runat="server" Text="Eliminar" CommandArgument='<%# Eval("idvacaciones") %>' OnClick="btnEliminar_Click" />
                            </td>

                        </tr>
                    </ItemTemplate>
                </asp:Repeater>

            </table>

            <div class="calendar-container">
                <div class="date-group">
                    <label for="txtFechaInicio">Fecha de Inicio:</label>
                    <asp:TextBox ID="txtFechaInicio" runat="server" TextMode="Date" CssClass="date-picker"></asp:TextBox>
                </div>
                <div class="date-group">
                    <label for="txtFechaFin">Fecha de Fin:</label>
                    <asp:TextBox ID="txtFechaFin" runat="server" TextMode="Date" CssClass="date-picker"></asp:TextBox>
                </div>
            </div>

        </div>
    </form>
</body>
</html>
