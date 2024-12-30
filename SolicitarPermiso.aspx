<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SolicitarPermiso.aspx.cs" Inherits="EmpresaProyecto.SolicitarPermiso" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Solicitar Permiso</title>
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

        .form-group {
            margin-bottom: 15px;
        }

        label {
            font-weight: bold;
            display: block;
            margin-bottom: 5px;
        }

        h2 {
            color: #333;
        }

        table {
            width: 100%;
            margin-top: 20px;
            border-collapse: collapse;
            background-color: #FFEFD5;
            border: none;
        }

        th, td {
            padding: 10px;
            text-align: left;
            border: none;
        }

        th {
            background-color: #f4b183;
        }

        .button-action {
            display: flex;
            justify-content: center;
        }

        .btn-action {
            background-color: #ccc;
            color: #333;
            padding: 6px 12px;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
        }

        .btn-action:hover {
            background-color: #b3b3b3;
        }

        .button-container {
            display: flex;
            justify-content: flex-end;
            gap: 10px;
            margin-top: 20px;
        }

        .btn {
            background-color: #FFA726;
            color: white;
            padding: 10px 20px;
            border-radius: 10px;
            border: none;
            cursor: pointer;
        }

        .btn:hover {
            background-color: #FB8C00;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>Solicitar Permiso</h2>
            <asp:Label ID="lblEmpleadoNombre" runat="server" CssClass="form-group" />
            <asp:HiddenField ID="hdnIdPermiso" runat="server" />

         <asp:GridView ID="gridPermisos" runat="server" AutoGenerateColumns="False" DataKeyNames="idPermiso">
    <Columns>
        <asp:TemplateField HeaderText="Seleccionar">
            <ItemTemplate>
                <asp:CheckBox ID="chkSeleccionar" runat="server" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="Identificacion" HeaderText="Identificación" />
        <asp:BoundField DataField="NombreCompleto" HeaderText="Nombre Completo" />
        <asp:BoundField DataField="TipoPermiso" HeaderText="Tipo de Permiso" />
        <asp:BoundField DataField="FechaPermiso" HeaderText="Fecha del Permiso" />
        <asp:BoundField DataField="HoraInicio" HeaderText="Hora de Inicio" />
        <asp:BoundField DataField="HoraFin" HeaderText="Hora de Fin" />
        <asp:BoundField DataField="Estado" HeaderText="Estado" />
    </Columns>
</asp:GridView>


            <div class="button-container">
                <asp:Button ID="btnEliminarSeleccionados" runat="server" Text="Eliminar Seleccionados" CssClass="btn" OnClick="btnEliminarSeleccionados_Click" />
                <asp:Button ID="btnSolicitarPermiso" runat="server" Text="Solicitar Permiso" CssClass="btn" OnClick="btnSolicitarPermiso_Click" />
                <asp:Button ID="btnSalir" runat="server" Text="Salir" CssClass="btn" OnClick="btnSalir_Click" />
            </div>

            <div class="form-group">
                <label for="ddlTipoPermiso">Tipo de Permiso</label>
                <asp:DropDownList ID="ddlTipoPermiso" runat="server">
                    <asp:ListItem Text="Seleccione un tipo de permiso" Value="" />
                    <asp:ListItem Text="Médico" Value="1" />
                    <asp:ListItem Text="Lactancia" Value="2" />
                    <asp:ListItem Text="Personal" Value="3" />
                    <asp:ListItem Text="Legal" Value="4" />
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <label for="txtFechaPermiso">Fecha del Permiso</label>
                <asp:TextBox ID="txtFechaPermiso" runat="server" TextMode="Date" />
            </div>
            <div class="form-group">
                <label for="txtHoraInicio">Hora de Inicio</label>
                <asp:TextBox ID="txtHoraInicio" runat="server" TextMode="Time" />
            </div>
            <div class="form-group">
                <label for="txtHoraFin">Hora de Fin</label>
                <asp:TextBox ID="txtHoraFin" runat="server" TextMode="Time" />
            </div>
        </div>
    </form>
</body>
</html>
