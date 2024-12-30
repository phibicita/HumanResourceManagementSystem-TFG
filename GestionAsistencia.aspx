<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestionAsistencia.aspx.cs" Inherits="EmpresaProyecto.GestionAsistencia" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Gestión de Asistencia</title>
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
            padding: 8px 12px;
            margin: 5px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-weight: bold;
            font-size: 14px;
            width: auto;
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
            background-color: #f4b183;
            color: white;
        }
        .error {
            color: red;
            font-weight: bold;
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>Gestión de Asistencia</h1>

            
            <asp:Label ID="lblError" runat="server" CssClass="error" Visible="False"></asp:Label>

            <div class="filter-group">
                <div>
                    <label for="txtFechaInicio">Fecha Inicio:</label>
                    <asp:TextBox ID="txtFechaInicio" runat="server" placeholder="dd/mm/yyyy"></asp:TextBox>
                </div>
                <div>
                    <label for="txtFechaFin">Fecha Fin:</label>
                    <asp:TextBox ID="txtFechaFin" runat="server" placeholder="dd/mm/yyyy"></asp:TextBox>
                </div>
                <div>
                    <label for="ddlEmpleado">Empleado:</label>
                    <asp:DropDownList ID="ddlEmpleado" runat="server">
                        <asp:ListItem Text="Seleccione un empleado" Value="" />
                    </asp:DropDownList>
                </div>
            </div>

            <div class="btn-group">
                <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn" OnClick="btnFiltrar_Click" />
                <asp:Button ID="btnExportarExcel" runat="server" Text="Exportar a Excel" CssClass="btn" OnClick="btnExportarExcel_Click" />
                <asp:Button ID="btnExportarPDF" runat="server" Text="Exportar a PDF" CssClass="btn" OnClick="btnExportarPDF_Click" />
                <asp:Button ID="btnSalir" runat="server" Text="Salir" CssClass="btn" OnClick="btnSalir_Click" />
            </div>

            <asp:GridView ID="gvAsistencia" runat="server" CssClass="table" AutoGenerateColumns="False">
                <Columns>
                    <asp:BoundField DataField="Fecha" HeaderText="Fecha" />
                    <asp:BoundField DataField="Empleado" HeaderText="Empleado" />
                    <asp:BoundField DataField="HoraEntrada" HeaderText="Hora Entrada" />
                    <asp:BoundField DataField="HoraSalida" HeaderText="Hora Salida" />
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
