<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmpleadosConsulta.aspx.cs" Inherits="EmpresaProyecto.EmpleadosConsulta" %>


<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Empleados Consulta</title>
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
        .empleado-nombre {
            font-size: 1.2em;
            color: #333;
            margin-top: 10px;
            text-align: center;
        }
        .filter-group {
            display: flex;
            flex-wrap: wrap;
            gap: 15px;
            margin-bottom: 20px;
        }
        .filter-group div {
            flex: 1;
            min-width: 200px;
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
    padding: 10px 15px; 
    margin: 5px;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-weight: bold;
    width: auto; 
    max-width: 200px; 
    text-align: center; 
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
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>Consulta Reportes Empleados</h1>
           
            <asp:Label ID="lblEmpleadoNombre" runat="server" CssClass="empleado-nombre" Text=""></asp:Label>
            
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
                    <label for="ddlTipoReporte">Tipo de Reporte:</label>
                    <asp:DropDownList ID="ddlTipoReporte" runat="server"></asp:DropDownList>
                </div>
            </div>
            <div class="btn-group">
                <asp:Button ID="btnConsultar" runat="server" Text="Consultar" CssClass="btn" OnClick="btnConsultar_Click" />
                <asp:Button ID="btnExportarPDF" runat="server" Text="Exportar a PDF" CssClass="btn" OnClick="btnExportarPDF_Click" />
                <asp:Button ID="btnSalir" runat="server" Text="Salir" CssClass="btn" OnClick="btnSalir_Click" />
            </div>
<asp:GridView ID="gvResultados" runat="server" CssClass="table" AutoGenerateColumns="true" OnDataBound="gvResultados_DataBound"></asp:GridView>
        </div>
    </form>
</body>
</html>
