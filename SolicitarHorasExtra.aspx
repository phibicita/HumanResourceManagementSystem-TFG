<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SolicitarHorasExtra.aspx.cs" Inherits="EmpresaProyecto.SolicitarHorasExtra" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Solicitud de Horas Extra</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f8f8f8;
        }
        .container {
            width: 80%;
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
        .header .left {
            font-size: 24px;
        }
        .header .right {
            display: flex;
            justify-content: space-around;
        }
        .right button, .right input[type="submit"] {
            padding: 10px 20px;
            margin: 5px;
            border-radius: 10px;
            border: none;
            cursor: pointer;
            background-color: #FFA726;
            color: white;
            font-size: 16px;
        }
        .right button:hover, .right input[type="submit"]:hover {
            background-color: #FB8C00;
        }
        .table-container {
            margin-bottom: 20px;
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
        .exit-container {
            display: flex;
            justify-content: flex-end;
        }
        #modalCheckbox {
            display: none;
        }
        .modal {
            display: none;
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            background-color: white;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.2);
            z-index: 1000;
            width: 300px;
        }
        #modalCheckbox:checked + .modal {
            display: block;
        }
        .overlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0, 0, 0, 0.5);
            z-index: 999;
        }
        #modalCheckbox:checked ~ .overlay {
            display: block;
        }
        .modal h2 {
            margin-bottom: 15px;
        }
        .modal input[type="text"], .modal button {
            margin-top: 10px;
        }
        .close-modal {
            cursor: pointer;
            color: red;
            text-decoration: underline;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header">
                <div class="left">
                    <asp:Label ID="lblEmpleadoNombre" runat="server" Text="Nombre del Empleado"></asp:Label>
                </div>
                <div class="right">
                    <asp:TextBox ID="txtHoras" runat="server" placeholder="Ingrese horas solicitadas" Width="200px"></asp:TextBox>
                    <asp:DropDownList ID="ddlEstado" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlEstado_SelectedIndexChanged" />
                    <asp:Button ID="btnEnviarSolicitud" runat="server" Text="Enviar Solicitud" CssClass="button" OnClick="btnEnviarSolicitud_Click" />
                  <asp:Button ID="btnEditar" runat="server" Text="Editar Solicitud" CssClass="button" OnClick="btnEditar_Click" />



                    <asp:Button ID="btnCancelarSolicitud" runat="server" Text="Cancelar Solicitud" CssClass="button" OnClick="btnCancelarSolicitud_Click" />
                </div>
            </div>
            <div class="table-container">
                <asp:Table ID="tablaSolicitudes" runat="server" CssClass="table">
                    <asp:TableHeaderRow>
                        <asp:TableHeaderCell>Seleccionar</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Fecha de solicitud</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Horas solicitadas</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Estado</asp:TableHeaderCell>
                        <asp:TableHeaderCell>Fecha de respuesta</asp:TableHeaderCell>
                    </asp:TableHeaderRow>
                </asp:Table>
            </div>
            <div class="exit-container">
                <asp:Button ID="btnSalir" runat="server" Text="Salir" CssClass="exit-button" PostBackUrl="~/MenuEmpleados.aspx" />
            </div>
        </div>
        <asp:HiddenField ID="hdnIdHorasExtra" runat="server" />
        <input type="checkbox" id="modalCheckbox" />
        <div class="modal">
            <h2>Editar Solicitud de Horas Extra</h2>
            <asp:HiddenField ID="hdnIdHorasExtraModal" runat="server" />
            <label for="txtEditarHoras">Horas solicitadas:</label>
            <asp:TextBox ID="txtEditarHoras" runat="server" Width="200px"></asp:TextBox>
            <br /><br />
            <asp:Button ID="btnGuardarEdicion" runat="server" Text="Guardar Cambios" OnClick="btnGuardarEdicion_Click" />
            <label for="modalCheckbox" class="close-modal">Cerrar</label>
        </div>
        <div class="overlay"></div>
    </form>
</body>
</html>
