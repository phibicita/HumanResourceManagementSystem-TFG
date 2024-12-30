<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ModuloSeguridadEmpleados.aspx.cs" Inherits="EmpresaProyecto.ModuloSeguridadEmpleados" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Módulo de Seguridad</title>
    <style>
        body { font-family: Arial, sans-serif; background-color: #ffffff; }
        .container { width: 400px; margin: 50px auto; background-color: #fff7f0; padding: 20px; border-radius: 15px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1); }
        h1 { text-align: center; color: #E07B22; }
        label { font-weight: bold; color: #333; display: block; margin-top: 15px; }
        input[type="text"], input[type="password"] { width: 100%; padding: 10px; margin-top: 5px; border: 1px solid #ccc; border-radius: 5px; background-color: #fdf4eb; }
        .btn { width: 100%; padding: 10px; margin-top: 15px; background-color: #FFA726; color: white; border: none; border-radius: 5px; cursor: pointer; font-size: 16px; }
        .btn:hover { background-color: #FB8C00; }
        .mensaje { text-align: center; margin-top: 15px; font-weight: bold; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>Módulo de Seguridad</h1>

            <label>Nombre de Usuario:</label>
            <asp:Label ID="lblUsuario" runat="server" Text="Usuario"></asp:Label>

            <label>Contraseña Actual:</label>
            <asp:TextBox ID="txtContrasenaActual" runat="server" TextMode="Password" placeholder="Ingrese su contraseña actual"></asp:TextBox>

            <label>Nueva Contraseña:</label>
            <asp:TextBox ID="txtContrasenaNueva" runat="server" TextMode="Password" placeholder="Máximo 15 caracteres (letras y números)" MaxLength="15"></asp:TextBox>

            <asp:Label ID="lblMensaje" runat="server" CssClass="mensaje"></asp:Label>

            <asp:Button ID="btnGuardar" runat="server" CssClass="btn" Text="Guardar Contraseña" OnClick="btnGuardar_Click" />
            <asp:Button ID="btnSalir" runat="server" CssClass="btn" Text="Salir" OnClick="btnSalir_Click" />
        </div>
    </form>
</body>
</html>
