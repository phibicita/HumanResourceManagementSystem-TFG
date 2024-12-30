<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Liquidacion.aspx.cs" Inherits="EmpresaProyecto.Liquidacion" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Gestión de Liquidaciones</title>
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
        .section-title {
            font-size: 18px;
            font-weight: bold;
            margin-top: 20px;
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
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header-title">Gestión de Liquidaciones</div>

            
            <div class="section-title">Lista de Empleados</div>
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
        <td><%# Eval("NombreCompleto") %></td>
        <td><%# Eval("Identificacion") %></td>
        <td><%# Eval("Puesto") %></td> 
        <td><%# Eval("SalarioBase", "{0:C}") %></td>
        <td><%# Eval("FechaIngreso", "{0:dd/MM/yyyy}") %></td>
    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>

            
            <div class="section-title">Tipo de Liquidación</div>
            <asp:DropDownList ID="ddlTipoLiquidacion" runat="server" CssClass="form-control">
                <asp:ListItem Text="Seleccionar" Value="" />
                <asp:ListItem Text="Despido con Causa" Value="1" />
                <asp:ListItem Text="Despido sin Causa" Value="2" />
                <asp:ListItem Text="Renuncia Voluntaria" Value="3" />
            </asp:DropDownList>

            
            <div class="section-title">Cálculo de Beneficios y Deducciones</div>
            <asp:Label ID="lblCesantia" runat="server" Text="Cesantía:" />
            <asp:TextBox ID="txtCesantia" runat="server" ReadOnly="true" CssClass="form-control" />

            <asp:Label ID="lblPreaviso" runat="server" Text="Preaviso:" />
            <asp:TextBox ID="txtPreaviso" runat="server" ReadOnly="true" CssClass="form-control" />


            <asp:Label ID="lblVacacionesPendientes" runat="server" Text="Vacaciones Pendientes:" />
            <asp:TextBox ID="txtVacacionesPendientes" runat="server" ReadOnly="true" CssClass="form-control" />

            <asp:Label ID="lblAguinaldoProporcional" runat="server" Text="Aguinaldo Proporcional:" />
            <asp:TextBox ID="txtAguinaldoProporcional" runat="server" ReadOnly="true" CssClass="form-control" />

            <asp:Label ID="lblSalariosPendientes" runat="server" Text="Salarios Pendientes:" />
            <asp:TextBox ID="txtSalariosPendientes" runat="server" ReadOnly="true" CssClass="form-control" />

            
            <div class="section-title">Monto Total de Liquidación</div>
            <asp:TextBox ID="txtMontoTotal" runat="server" ReadOnly="true" CssClass="form-control" />

            
            <asp:Button ID="btnCalcularLiquidacion" runat="server" Text="Calcular Liquidación" CssClass="btn" OnClick="btnCalcularLiquidacion_Click" />
            <asp:Button ID="btnGuardarLiquidacion" runat="server" Text="Guardar Liquidación" CssClass="btn" OnClick="btnGuardarLiquidacion_Click" />
            <asp:Button ID="btnGenerarReporteLiquidacion" runat="server" Text="Generar Reporte" CssClass="btn" OnClick="btnGenerarReporteLiquidacion_Click" />
            <asp:Button ID="btnLimpiar" runat="server" Text="Cancelar/Limpiar" CssClass="btn" OnClick="btnLimpiar_Click" />
            <asp:Button ID="btnSalir" runat="server" CssClass="button" Text="Salir" OnClick="btnSalir_Click" />
        </div>
    </form>
</body>
</html>


