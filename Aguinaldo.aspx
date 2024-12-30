<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Aguinaldo.aspx.cs" Inherits="EmpresaProyecto.Aguinaldo" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Gestión de Aguinaldo</title>
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
        }

        .table {
            width: 100%;
            margin-top: 20px;
            border-collapse: collapse;
            background-color: #FFEFD5;
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

        .result-text {
            font-size: 18px;
            font-weight: bold;
            color: #FFA726;
            margin-top: 20px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header-title">Cálculo de Aguinaldo</div>


            <asp:Repeater ID="repeaterEmpleados" runat="server">
                <HeaderTemplate>
                    <table class="table">
                        <tr>
                            <th>Seleccionar</th>
                            <th>Nombre</th>
                            <th>Identificación</th>
                            <th>Puesto</th>
                            <th>Salario Base</th>
                            <th>Fecha de Ingreso</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkSelectEmployee" runat="server" />
                            <asp:HiddenField ID="hfIdEmpleado" runat="server" Value='<%# Eval("idEmpleado") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblNombreCompleto" runat="server" Text='<%# Eval("NombreCompleto") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblCedula" runat="server" Text='<%# Eval("Identificacion") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblPuesto" runat="server" Text='<%# Eval("Puesto") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblSalarioBase" runat="server" Text='<%# Eval("SalarioBase", "{0:C}") %>' />
                        </td>
                        <td>
                            <asp:Label ID="lblFechaIngreso" runat="server" Text='<%# Eval("FechaIngreso", "{0:dd/MM/yyyy}") %>' />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>


            <div id="formSalarioMensual" runat="server" visible="false">
                <h2>Salario bruto de los últimos meses</h2>
                <p>En cada una de las siguientes casillas se debe digitar el salario bruto devengado en cada mes según corresponda.</p>
                <table>
                    <tr>
                        <td id="lblDiciembre" runat="server">Diciembre:</td>
                        <td>
                            <asp:TextBox ID="txtDiciembre" runat="server" /></td>

                        <td>Junio:</td>
                        <td>
                            <asp:TextBox ID="txtJunio" runat="server" /></td>
                    </tr>
                    <tr>
                        <td>Enero:</td>
                        <td>
                            <asp:TextBox ID="txtEnero" runat="server" /></td>
                        <td>Julio:</td>
                        <td>
                            <asp:TextBox ID="txtJulio" runat="server" /></td>
                    </tr>
                    <tr>
                        <td>Febrero:</td>
                        <td>
                            <asp:TextBox ID="txtFebrero" runat="server" /></td>
                        <td>Agosto:</td>
                        <td>
                            <asp:TextBox ID="txtAgosto" runat="server" /></td>
                    </tr>
                    <tr>
                        <td>Marzo:</td>
                        <td>
                            <asp:TextBox ID="txtMarzo" runat="server" /></td>
                        <td>Septiembre:</td>
                        <td>
                            <asp:TextBox ID="txtSeptiembre" runat="server" /></td>
                    </tr>
                    <tr>
                        <td>Abril:</td>
                        <td>
                            <asp:TextBox ID="txtAbril" runat="server" /></td>
                        <td>Octubre:</td>
                        <td>
                            <asp:TextBox ID="txtOctubre" runat="server" /></td>
                    </tr>
                    <tr>
                        <td>Mayo:</td>
                        <td>
                            <asp:TextBox ID="txtMayo" runat="server" /></td>
                        <td>Noviembre:</td>
                        <td>
                            <asp:TextBox ID="txtNoviembre" runat="server" /></td>
                    </tr>
                </table>

                <asp:Button ID="btnCalcularAguinaldoMeses" runat="server" Text="Calcular Aguinaldo" OnClick="btnCalcularAguinaldo_Click" CssClass="btn" />
            </div>


            <asp:Label ID="lblResultadoAguinaldo" runat="server" CssClass="result-text" Visible="false" />


            <asp:Button ID="btnCalcularAguinaldo" runat="server" Text="Seleccionar y Calcular Aguinaldo" CssClass="btn" OnClick="btnSeleccionarEmpleado_Click" />
            <asp:Button ID="btnGuardarAguinaldo" runat="server" Text="Guardar Aguinaldo" CssClass="btn" OnClick="btnGuardarAguinaldo_Click" Visible="false" />



            <asp:Button ID="btnExportarAguinaldo" runat="server" Text="Exportar a PDF/Excel" OnClick="btnExportarAguinaldo_Click" CssClass="btn btn-secondary" />
            <asp:Button ID="btnSalir" runat="server" Text="Salir" CssClass="btn" OnClick="btnSalir_Click" />
        </div>
    </form>
</body>
</html>

