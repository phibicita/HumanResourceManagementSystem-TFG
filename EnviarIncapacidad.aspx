<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EnviarIncapacidad.aspx.cs" Inherits="EmpresaProyecto.EnviarIncapacidad" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Enviar Incapacidad</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f8f8f8;
        }
        .container {
            width: 90%;
            max-width: 600px; 
            margin: 0 auto;
            background-color: #fff7f0;
            padding: 20px;
            border-radius: 15px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }
        .header {
            margin-bottom: 20px;
            font-size: 20px;
            font-weight: bold;
            text-align: center;
        }
        .form-group {
            margin-bottom: 15px;
        }
        .form-label {
            font-weight: bold;
            display: block;
            margin-bottom: 5px;
        }
        .form-input {
            width: 100%;
            max-width: 300px; 
            padding: 8px;
            border-radius: 5px;
            border: 1px solid #ccc;
        }
        .submit-btn, .exit-btn {
            background-color: #FFA726;
            color: white;
            padding: 10px 20px;
            border-radius: 10px;
            border: none;
            cursor: pointer;
            width: auto;
            max-width: 150px;
            text-align: center;
        }

            .submit-btn:hover, .exit-btn:hover {
                background-color: #FB8C00;
            }

        .exit-btn {
            margin-top: 10px;
            background-color: #d9534f;
        }

            .exit-btn:hover {
                background-color: #c9302c;
            }
    </style>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
        <div class="container">
            <div class="header">
                Enviar Incapacidad - <asp:Label ID="lblNombreEmpleado" runat="server" Text="Nombre del empleado"></asp:Label>
            </div>

            <div class="form-group">
                <asp:Label ID="lblTipoIncapacidad" runat="server" Text="Tipo de Incapacidad:" CssClass="form-label"></asp:Label>
                <asp:DropDownList ID="ddlTipoIncapacidad" runat="server" CssClass="form-input">
                </asp:DropDownList>
            </div>

            <div class="form-group">
                <asp:Label ID="lblFechaInicio" runat="server" Text="Fecha de Inicio:" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="txtFechaInicio" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox>
            </div>

            <div class="form-group">
                <asp:Label ID="lblFechaFin" runat="server" Text="Fecha de Fin:" CssClass="form-label"></asp:Label>
                <asp:TextBox ID="txtFechaFin" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox>
            </div>

            <div class="form-group">
                <asp:Label ID="lblArchivoIncapacidad" runat="server" Text="Subir Incapacidad (PDF o Imagen):" CssClass="form-label"></asp:Label>
                <asp:FileUpload ID="fuArchivoIncapacidad" runat="server" CssClass="form-input" />
            </div>

            <asp:Button ID="btnEnviarIncapacidad" runat="server" Text="Enviar Incapacidad" CssClass="submit-btn" OnClick="btnEnviarIncapacidad_Click" />
            <asp:Button ID="btnSalir" runat="server" Text="Salir" CssClass="btn" OnClick="btnSalir_Click" />
        </div>
    </form>
</body>
</html>

