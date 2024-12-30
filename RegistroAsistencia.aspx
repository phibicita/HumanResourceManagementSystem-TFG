<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RegistroAsistencia.aspx.cs" Inherits="EmpresaProyecto.RegistroAsistencia" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Registro de Asistencia</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f4f4f4;
        }

        .container {
            margin: 20px auto;
            padding: 20px;
            background-color: #fff7f0;
            box-shadow: 0 2px 5px rgba(0, 0, 0, 0.2);
            width: 80%;
            border-radius: 8px;
        }

        h2 {
            color: #FFA500;
            text-align: center;
        }

        .info-container {
            margin-bottom: 20px;
            text-align: center;
            padding: 10px;
            background-color: #FFE5B4;
            border-radius: 8px;
        }

        .info-container label {
            font-size: 16px;
            display: block;
            color: #333;
        }

        .button {
            background-color: #FFA500;
            color: white;
            border: none;
            padding: 10px 20px;
            font-size: 16px;
            cursor: pointer;
            border-radius: 5px;
            margin: 10px;
        }

        .button:hover {
            background-color: #e69500;
        }

        .table-container {
            margin-top: 20px;
        }

        table {
            width: 100%;
            border-collapse: collapse;
        }

        th, td {
            padding: 10px;
            text-align: left;
            border-bottom: 1px solid #ddd;
        }

        th {
            background-color: #FFA500;
            color: white;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>Registro de Asistencia</h2>

           
            <div class="info-container">
                <asp:Label ID="lblNombreEmpleado" runat="server" Text="Nombre: -"></asp:Label>
                <asp:Label ID="lblEstado" runat="server" Text="Estado: -"></asp:Label>
                <asp:Label ID="lblUltimaMarcacion" runat="server" Text="Última Marcación: -"></asp:Label>
            </div>

            
            <div class="button-container" style="text-align: center;">
                <asp:Button ID="btnEntrada" runat="server" CssClass="button" Text="Registrar Entrada" OnClick="btnEntrada_Click" />
                <asp:Button ID="btnSalida" runat="server" CssClass="button" Text="Registrar Salida" OnClick="btnSalida_Click" />
                <asp:Button ID="btnSalir" runat="server" CssClass="button" Text="Salir" OnClick="btnSalir_Click" />
            </div>

          
            <div class="table-container">
                <asp:GridView ID="gvRegistros" runat="server" AutoGenerateColumns="False">
                    <Columns>
                        <asp:BoundField DataField="idAsistencia" HeaderText="ID" />
                        <asp:BoundField DataField="fecha" HeaderText="Fecha" />
                        <asp:BoundField DataField="hora_entrada" HeaderText="Hora Entrada" />
                        <asp:BoundField DataField="hora_salida" HeaderText="Hora Salida" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </form>
</body>
</html>
