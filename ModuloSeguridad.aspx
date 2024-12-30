<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ModuloSeguridad.aspx.cs" Inherits="EmpresaProyecto.ModuloSeguridad" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Módulo de Seguridad</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f5f5f5;
            margin: 0;
            padding: 20px;
        }
        .container {
            max-width: 600px;
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
        .form-group {
            margin-bottom: 15px;
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
        .btn {
            background-color: #ffa726;
            color: white;
            padding: 10px 20px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-weight: bold;
            width: 100%;
        }
        .btn:hover {
            background-color: #fb8c00;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>Módulo de Seguridad</h1>
            <div class="form-group">
                <label for="ddlEmpleados">Seleccione un empleado:</label>
                <asp:DropDownList ID="ddlEmpleados" runat="server"></asp:DropDownList>
            </div>
            <div class="form-group">
                <label for="txtUsuario">Nombre de Usuario:</label>
                <asp:TextBox ID="txtUsuario" runat="server" placeholder="Ingrese el nombre de usuario"></asp:TextBox>
            </div>
            <div class="form-group">
                <label for="txtContrasena">Contraseña:</label>
                <asp:TextBox ID="txtContrasena" runat="server" TextMode="Password" placeholder="Ingrese la contraseña"></asp:TextBox>
            </div>
            <asp:Button ID="btnGuardar" runat="server" Text="Asignar Usuario y Contraseña" CssClass="btn" OnClick="btnGuardar_Click" />
            <asp:Button ID="btnSalir" runat="server" Text="Salir" CssClass="btn btn-exit" OnClick="btnSalir_Click" />
        </div>
    </form>
</body>
</html>
