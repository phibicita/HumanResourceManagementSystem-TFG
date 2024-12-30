<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestionNomina.aspx.cs" Inherits="EmpresaProyecto.GestionNomina" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Gestión de Nómina</title>
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
    .header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 20px;
    }
    .header-title {
        font-size: 24px;
        font-weight: bold;
    }
    .action-buttons button {
        background-color: #FFA726;
        color: white;
        padding: 10px;
        border-radius: 10px;
        border: none;
        cursor: pointer;
        margin-right: 10px;
    }
    .action-buttons button:hover {
        background-color: #FB8C00;
    }
    .payroll-table {
        width: 100%;
        margin-top: 20px;
        border-collapse: collapse;
        background-color: #FFEFD5;
        table-layout: fixed; 
    }
    .payroll-table th, .payroll-table td {
        padding: 15px;
        text-align: left;
        border-bottom: 1px solid #ddd;
        width: auto; 
    }
    .payroll-table th {
        background-color: #f4b183;
        color: #000;
        font-weight: bold;
        text-align: center;
    }
    .payroll-table th:first-child, .payroll-table td:first-child {
        width: 60px; 
    }
    
    .form-group {
        display: flex;
        flex-direction: row;
        align-items: center;
        margin-bottom: 10px;
    }
    .form-group label {
        width: 150px;
        font-weight: bold;
    }
    .form-group input, .form-group select {
        flex: 1;
        padding: 5px;
        border-radius: 5px;
        border: 1px solid #ddd;
    }
    .form-buttons {
        display: flex;
        justify-content: flex-end;
        margin-top: 20px;
    }
    .form-buttons .btn {
        padding: 10px 20px;
        border-radius: 5px;
        border: none;
        cursor: pointer;
        margin-left: 10px;
    }
    .form-buttons .btn-primary {
        background-color: #4CAF50;
        color: white;
    }
    .form-buttons .btn-primary:hover {
        background-color: #45a049;
    }
    .btn-secondary {
        background-color: #FFA726;
        color: white;
    }
</style>

    <script type="text/javascript">
        
        function toggleSelectAll(source) {
            var checkboxes = document.querySelectorAll('.employeeCheckbox');
            for (var i = 0; i < checkboxes.length; i++) {
                checkboxes[i].checked = source.checked;
                checkboxes[i].dispatchEvent(new Event('change')); 
            }
        }

    </script>

</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header">
                <div class="header-title">Gestión de Nómina</div>
                
                <div class="quincena-info">
                    <asp:Label ID="lblQuincenaActual" runat="server"></asp:Label>
                </div>

                <div class="action-buttons">
                    <asp:Button ID="btnCalcularNomina" runat="server" Text="Calcular Nómina" OnClick="btnCalcularNomina_Click" />
                    <asp:Button ID="btnGenerarReporte" runat="server" Text="Generar Reporte" OnClick="btnGenerarReporte_Click" />
                    <asp:Button ID="btnExportar" runat="server" Text="Exportar a Excel" OnClick="btnExportar_Click" />
                    <asp:Button ID="btnGuardarNomina" runat="server" Text="Guardar Nómina" OnClick="btnGuardarNomina_Click" CssClass="btn btn-primary" />
                    <asp:Button ID="btnMostrarFormulario" runat="server" Text="Agregar Nuevo Empleado" OnClick="btnMostrarFormulario_Click" CssClass="btn btn-secondary" />
                     <asp:Button ID="btnEliminarEmpleado" runat="server" Text="Eliminar Empleado(s)" OnClick="btnEliminarEmpleado_Click" CssClass="btn btn-danger" />
                    <asp:Button ID="btnEditarParametros" runat="server" Text="Editar Parámetros" OnClick="btnEditarParametros_Click" />

                </div>
                
<asp:Panel ID="pnlEditarParametros" runat="server" Visible="false" CssClass="container">
    <h3>Editar Parámetros de Deducción</h3>
    <div class="form-group">
        <asp:Label ID="lblCCSS" runat="server" Text="Porcentaje CCSS:" AssociatedControlID="txtCCSS" />
        <asp:TextBox ID="txtCCSS" runat="server" CssClass="form-control" />
    </div>
    <div class="form-group">
        <asp:Label ID="lblIVM" runat="server" Text="Porcentaje IVM:" AssociatedControlID="txtIVM" />
        <asp:TextBox ID="txtIVM" runat="server" CssClass="form-control" />
    </div>
    <div class="form-group">
        <asp:Label ID="lblFCL" runat="server" Text="Porcentaje FCL:" AssociatedControlID="txtFCL" />
        <asp:TextBox ID="txtFCL" runat="server" CssClass="form-control" />
    </div>
    <div class="form-group">
        <asp:Label ID="lblRentaTramo1" runat="server" Text="Límite Tramo 1 (0%):" AssociatedControlID="txtRentaTramo1" />
        <asp:TextBox ID="txtRentaTramo1" runat="server" CssClass="form-control" />
    </div>
    <div class="form-group">
        <asp:Label ID="lblRentaTramo2" runat="server" Text="Límite Tramo 2 (10%):" AssociatedControlID="txtRentaTramo2" />
        <asp:TextBox ID="txtRentaTramo2" runat="server" CssClass="form-control" />
    </div>
    <div class="form-group">
        <asp:Label ID="lblRentaTramo3" runat="server" Text="Límite Tramo 3 (15%):" AssociatedControlID="txtRentaTramo3" />
        <asp:TextBox ID="txtRentaTramo3" runat="server" CssClass="form-control" />
    </div>
    <div class="form-buttons">
        <asp:Button ID="btnGuardarParametros" runat="server" Text="Guardar Parámetros" OnClick="btnGuardarParametros_Click" CssClass="btn btn-primary" />
    </div>
</asp:Panel>
            </div>

           
            <asp:Panel ID="pnlAgregarEmpleado" runat="server" Visible="false" CssClass="container">
                <h3>Agregar Nuevo Empleado</h3>
                
                 <div class="form-group">
 <asp:Label ID="lblIdentificacion" runat="server" Text="Identificación:" AssociatedControlID="txtIdentificacion" />
 <asp:TextBox ID="txtIdentificacion" runat="server" CssClass="form-control" />
      </div>


                <div class="form-group">
                    <asp:Label ID="lblNombre" runat="server" Text="Nombre:" AssociatedControlID="txtNombre" />
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" />
                </div>

                <div class="form-group">
                    <asp:Label ID="lblPrimerApellido" runat="server" Text="Primer Apellido:" AssociatedControlID="txtPrimerApellido" />
                    <asp:TextBox ID="txtPrimerApellido" runat="server" CssClass="form-control" />
                </div>

                <div class="form-group">
                    <asp:Label ID="lblSegundoApellido" runat="server" Text="Segundo Apellido:" AssociatedControlID="txtSegundoApellido" />
                    <asp:TextBox ID="txtSegundoApellido" runat="server" CssClass="form-control" />
                </div>

               
                <div class="form-group">
                    <asp:Label ID="lblSalarioBase" runat="server" Text="Salario Base:" AssociatedControlID="txtSalarioBase" />
                    <asp:TextBox ID="txtSalarioBase" runat="server" CssClass="form-control" />
                </div>

                                <div class="form-group">
                <asp:Label ID="lblFechaIngreso" runat="server" Text="Fecha de Ingreso:" AssociatedControlID="txtFechaIngreso" />
<asp:TextBox ID="txtFechaIngreso" runat="server" CssClass="form-control" placeholder="dd/mm/aaaa" />
                                    </div>

                <div class="form-group">
                    <asp:Label ID="lblGenero" runat="server" Text="Género:" AssociatedControlID="ddlGenero" />
                    <asp:DropDownList ID="ddlGenero" runat="server" CssClass="form-control">
                        <asp:ListItem Text="Seleccionar" Value="" />
                        <asp:ListItem Text="Masculino" Value="2" />
                        <asp:ListItem Text="Femenino" Value="1" />
                    </asp:DropDownList>
                </div>

                <div class="form-group">
                    <asp:Label ID="lblTipoEmpleado" runat="server" Text="Tipo de Empleado:" AssociatedControlID="ddlTipoEmpleado" />
                    <asp:DropDownList ID="ddlTipoEmpleado" runat="server" CssClass="form-control">
                        <asp:ListItem Text="Seleccionar" Value="" />
                        <asp:ListItem Text="Gerente" Value="1" />
                        <asp:ListItem Text="Empleado" Value="2" />
                    </asp:DropDownList>
                </div>

                <div class="form-group">
                    <asp:Label ID="lblEstado" runat="server" Text="Estado:" AssociatedControlID="ddlEstado" />
                    <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-control">
                        <asp:ListItem Text="Seleccionar" Value="" />
                        <asp:ListItem Text="Activo" Value="1" />
                        <asp:ListItem Text="Inactivo" Value="2" />
                    </asp:DropDownList>
                </div>

                <div class="form-buttons">
                    <asp:Button ID="btnAgregarEmpleado" runat="server" Text="Agregar Empleado" OnClick="btnAgregarEmpleado_Click" CssClass="btn btn-primary" />
                </div>
            </asp:Panel>
            

            <table class="payroll-table">
                <tr>
<th>
    <asp:CheckBox ID="chkSelectAll" runat="server" onclick="toggleSelectAll(this)" />
    Seleccionar Todos
</th>
                    <th>Nombre del Empleado</th>
                    <th>Horas Extra Trabajadas</th>
                    <th>Salario Base</th>
                    <th>CCSS (SEM)</th>
                    <th>FCL</th>
                    <th>IVM</th>
                    <th>Renta</th>
                    <th>Total Deducciones</th>
                    <th>Salario Neto</th>
                </tr>
                <asp:Repeater ID="repeaterNomina" runat="server">
    <ItemTemplate>
        <tr>
            <td>
                <asp:CheckBox ID="chkEmployee" runat="server" CssClass="employeeCheckbox" />
                <asp:HiddenField ID="hfEmpleadoId" runat="server" Value='<%# Eval("idEmpleado") %>' />
            </td>
            <td><%# Eval("NombreCompleto") %></td>
            <td><%# Eval("HorasExtra") %></td>
            <td><%# Eval("SalarioBase", "{0:C}") %></td>
            <td><%# Eval("CCSS", "{0:C}") %></td>
            <td><%# Eval("FCL", "{0:C}") %></td>
            <td><%# Eval("IVM", "{0:C}") %></td>
            <td><%# Eval("Renta", "{0:C}") %></td>
            <td><%# Eval("TotalDeducciones", "{0:C}") %></td>
            <td><%# Eval("SalarioNeto", "{0:C}") %></td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
            </table>
            <asp:Button ID="btnSalir" runat="server" CssClass="button" Text="Salir" OnClick="btnSalir_Click" />
        </div>
    </form>
</body>
</html>
