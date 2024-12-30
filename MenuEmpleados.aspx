<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MenuEmpleados.aspx.cs" Inherits="EmpresaProyecto.MenuEmpleados" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Menú Empleado</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #ffffff;
            margin: 0;
            padding: 0;
        }

        .menu-container {
            background-color: #FDECE3;
            width: 250px;
            height: 100vh;
            position: fixed;
            top: 0;
            left: 0;
            padding: 20px;
            display: flex;
            flex-direction: column;
            align-items: flex-start;
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

        a, .menu-item button {
            text-decoration: none;
            color: #333;
        }

        .menu-item a:hover, .menu-item button:hover {
            text-decoration: underline;
        }

        .menu-button {
            background-color: transparent;
            border: none;
            text-align: left;
            font-size: 16px;
            padding: 0;
            cursor: pointer;
            color: #333;
        }

        .logout-button {
            background-color: #FFA726;
            border: none;
            padding: 10px 20px;
            margin-top: auto;
            border-radius: 10px;
            cursor: pointer;
            font-size: 18px;
            width: calc(100% - 40px);
            text-align: center;
            color: white;
        }

        .logout-button:hover {
            background-color: #FB8C00;
        }

        .content {
            margin-left: 270px;
            padding: 20px;
        }

        .content img {
            width: 100%;
            height: auto;
            object-fit: cover;
        }

    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="menu-container">
            <ul>
                <li class="menu-item">
                 <img src="/images/RegistroAsistencia.png" alt="Registro Asistencia" />
                  <asp:Button ID="btnRegistroAsistencia" runat="server" Text="Registrar asistencia" OnClick="btnRegistroAsistencia_Click" CssClass="menu-button" />
                   </li>

                <li class="menu-item">
                    <img src="/images/horasExtra.png" alt="Horas Extra" />
                    <asp:Button ID="btnSolicitarHoras" runat="server" Text="Solicitar horas extra" OnClick="btnSolicitarHoras_Click" CssClass="menu-button" />
                </li>
                <li class="menu-item">
                    <img src="/images/vacaciones.png" alt="Vacaciones" />
                    <asp:Button ID="btnSolicitarVacaciones" runat="server" Text="Solicitar vacaciones" OnClick="btnSolicitarVacaciones_Click" CssClass="menu-button" />
                </li>
                <li class="menu-item">
                    <img src="/images/incapacidad.png" alt="Incapacidad" />
                    <asp:Button ID="btnEnviarIncapacidad" runat="server" Text="Enviar incapacidad" OnClick="btnEnviarIncapacidad_Click" CssClass="menu-button" />
                </li>
               
                <li class="menu-item">
                   <img src="/images/Permisos.png" alt="Horas Extra" />
                    <asp:Button ID="btnEnviarPermiso" runat="server" Text="Solicitar Permisos" OnClick="btnEnviarPermiso_Click" CssClass="menu-button" />
                    </li>

                 <li class="menu-item">
     <img src="/images/consultasyreportes.png" alt="Consultas y Reportes" />
      <asp:Button ID="btnEmpleadosConsulta" runat="server" Text="Consultas y Reportes" OnClick="btnEmpleadosConsulta_Click" CssClass="menu-button" />
 </li>
                <li class="menu-item">
                    <img src="/images/Seguridad.png" alt="Horas Extra" />
                      <asp:Button ID="btnModuloSeguridadEmpleados" runat="server" Text="Módulo Seguridad" OnClick="btnModuloSeguridadEmpleados_Click" CssClass="menu-button" />
                         </li>
                <li class="menu-item">
                    <asp:Button ID="btnCerrarSesion" runat="server" Text="Cerrar Sesión" CssClass="logout-button" OnClick="btnCerrarSesion_Click" />
                </li>
            </ul>
        </div>

        <div class="content">
            <img src="/images/menuPrincipal.jpeg" alt="Fondo del menú" />
        </div>
    </form>
</body>
</html>

