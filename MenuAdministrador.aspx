<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MenuAdministrador.aspx.cs" Inherits="EmpresaProyecto.MenuAdministrador" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Menú Principal Administrador</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #ffffff;
            margin: 0;
            padding: 0;
            background-image: url('/images/menuPrincipal.jpeg'); 
            background-size: cover; 
            background-position: center; 
            background-attachment: fixed; 
        }

        
        .sidebar {
            background-color: #FDECE3; 
            width: 260px;
            height: 100vh; 
            position: fixed; 
            top: 0;
            left: 0;
            padding: 20px;
            display: flex;
            flex-direction: column;
            align-items: flex-start;
            overflow-y: auto; 
        }

        
        .menu-item {
            list-style-type: none;
            padding: 15px 10px;
            text-align: left;
            font-size: 16px;
            color: #333;
            cursor: pointer;
            display: flex;
            align-items: center;
            width: 100%;
            margin-bottom: 10px;
            transition: background-color 0.3s ease;
        }

        
        .menu-item img {
            width: 25px;
            height: 25px;
            margin-right: 15px;
        }

       
        .menu-item:hover {
            background-color: #e6c2a5;
            border-radius: 8px;
        }
        
        
        .logout-button {
            background-color: #F3DFCA;
            border: none;
            padding: 10px 20px;
            margin-top: auto; 
            border-radius: 10px;
            cursor: pointer;
            font-size: 18px;
            width: calc(100% - 40px);
            text-align: center;
        }

        
        .content {
            margin-left: 260px; 
            padding: 20px;
        }
        

    </style>
</head>
<body>
    <form id="form1" runat="server">
        
        <div class="sidebar">
            <ul>
                

                <li class="menu-item">
                   <img src="/images/Horarios.png" alt="Horarios" />
                   <asp:Button ID="btnGestionHorarios" runat="server" Text="Gestión Horarios" OnClick="btnGestionHorarios_Click" CssClass="menu-button" />
                </li>
                <li class="menu-item">
                    <img src="/images/gestionNomina.png" alt="Nómina" />
                    <asp:Button ID="btnGestionNomina" runat="server" Text="Gestión nómina" OnClick="btnGestionNomina_Click" CssClass="menu-button" />
                </li>
                <li class="menu-item">
                    <img class="menu-icons" src="/images/horasExtra.png" alt="Horas Extra" />
                    <asp:Button ID="btnGestionHorasExtra" runat="server" Text="Gestión horas extra" OnClick="btnGestionHorasExtra_Click" CssClass="menu-button" />
                </li>
                <li class="menu-item">
    <img class="menu-icons" src="/images/liquidacion.png" alt="Horas Extra" />
    <asp:Button ID="btnGestionLiquidaciones" runat="server" Text="Liquidaciones" OnClick="btnGestionLiquidaciones_Click" CssClass="menu-button" />
</li>
                <li class="menu-item">
                    <img class="menu-icons" src="/images/vacaciones.png" alt="Vacaciones" />
                    <asp:Button ID="btnGestionVacaciones" runat="server" Text="Gestión de vacaciones" OnClick="btnGestionVacaciones_Click" CssClass="menu-button" />
                </li>
                <li class="menu-item">
                    <img class="menu-icons" src="/images/asistencia.png" alt="Asistencia" />
                    <asp:Button ID="btnGestionAsistencia" runat="server" Text="Gestión de asistencia" OnClick="btnGestionAsistencia_Click" CssClass="menu-button" />
                </li>
               
                <li class="menu-item">
                    <img src="/images/Permisos.png" alt="Permisos" />
                    <asp:Button ID="btnGestionPermisos" runat="server" Text="Gestión de permisos" OnClick="btnGestionPermisos_Click" CssClass="menu-button" />
                </li>
                <li class="menu-item">
                    <img class="menu-icons" src="/images/incapacidad.png" alt="Incapacidades" />
                    <asp:Button ID="btnGestionIncapacidades" runat="server" Text="Gestión de incapacidades" OnClick="btnGestionIncapacidades_Click" CssClass="menu-button" />
                </li>
                <li class="menu-item">
                    <img class="menu-icons" src="/images/Aguinaldo.png" alt="Aguinaldo" />
                    <asp:Button ID="btnCalculoAguinaldo" runat="server" Text="Cálculo de Aguinaldo" OnClick="btnCalculoAguinaldo_Click" CssClass="menu-button" />
                </li>
                <li class="menu-item">
                    <img class="menu-icons" src="/images/evaluacionPersonal.png" alt="Evaluación" />
                    <asp:Button ID="btnEvaluacionPersonal" runat="server" Text="Evaluación del personal" OnClick="btnEvaluacionPersonal_Click" CssClass="menu-button" />
                </li>
                <li class="menu-item">
                    <img class="menu-icons" src="/images/consultasyreportes.png" alt="Consultas y Reportes" />
                    <asp:Button ID="btnConsultasAdmin" runat="server" Text="Consultas y Reportes" OnClick="btnConsultasAdmin_Click" CssClass="menu-button" />
                </li>
                <li class="menu-item">
                      <img class="menu-icons" src="/images/Seguridad.png" alt="Seguridad" />
                      <asp:Button ID="btnModuloSeguridad" runat="server" Text="Módulo Seguridad" OnClick="btnModuloSeguridad_Click" CssClass="menu-button" />
                </li>
            </ul>

          
            <asp:Button ID="btnCerrarSesion" runat="server" Text="Cerrar Sesión" CssClass="logout-button" OnClick="btnCerrarSesion_Click" />
        
        </div>
    </form>
  
</body>
</html>

