<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="EmpresaProyecto.Login" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Iniciar Sesión</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: white;
            text-align: center;
            height: 100vh;
            margin: 0;
            display: flex;
            justify-content: center;
            align-items: center;
        }
        .login-container {
            text-align: center;
            padding: 50px;
            box-shadow: 0 0 20px rgba(0, 0, 0, 0.1);
            border-radius: 20px;
            background-color: #FFFFFF;
            width: 400px;
        }
        h2 {
            color: #E07B22;
        }
        input[type="text"], input[type="password"] {
            width: 300px;
            padding: 10px;
            margin: 10px 0;
            background-color: #FDECE3;
            border: none;
            border-radius: 15px;
            box-shadow: 2px 2px 5px rgba(0, 0, 0, 0.1);
        }
        .password-container {
            position: relative;
            display: inline-block;
        }
        .password-toggle {
            position: absolute;
            right: 10px;
            top: 50%;
            transform: translateY(-50%);
            cursor: pointer;
        }
        .login-button {
            padding: 10px 30px;
            background-color: #F3DFCA;
            border: none;
            border-radius: 15px;
            cursor: pointer;
            box-shadow: 2px 2px 5px rgba(0, 0, 0, 0.2);
            font-size: 18px;
        }
        .login-button:hover {
            background-color: #e6c2a5;
        }
        .forgot-password {
            margin-top: 10px;
            color: red;
            text-decoration: none;
        }
        .forgot-password:hover {
            text-decoration: underline;
        }
        .logo {
            margin-bottom: 20px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">
            <h2>Corporación Krasinski S.A</h2>
            <img class="logo" src="/images/LOGO.png" alt="Logo" width="200" />
            <br />
            <asp:Label ID="Label1" runat="server" Text="Usuario"></asp:Label>
            <br />
            <asp:TextBox ID="txtUsuario" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="Label2" runat="server" Text="Contraseña"></asp:Label>
            <br />
            <div class="password-container">
                <asp:TextBox ID="txtContrasena" runat="server" TextMode="Password" placeholder="Contraseña"></asp:TextBox>
                <span class="password-toggle" onclick="togglePassword()">
              <img id="ojoIcon" src="/images/eye-closed.png" alt="Mostrar/Ocultar" width="20" />
                </span>
            </div>
            <br />
            <asp:LinkButton ID="lnkOlvideContrasena" runat="server" OnClick="lnkOlvideContrasena_Click" ForeColor="Red">Olvidé contraseña</asp:LinkButton>

            <br /><br />
            <asp:Button ID="btnIniciarSesion" runat="server" CssClass="login-button" Text="Iniciar Sesión" OnClick="btnIniciarSesion_Click" />
            
            <br />
            <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
        </div>
    </form>

    <script type="text/javascript">
        function togglePassword() {
            var passwordField = document.getElementById('<%= txtContrasena.ClientID %>');
            var ojoIcon = document.getElementById('ojoIcon');

            if (passwordField.type === "password") {
                passwordField.type = "text";
                ojoIcon.src = "/images/eye-open.png"; 
            } else {
                passwordField.type = "password";
                ojoIcon.src = "/images/eye-closed.png"; 
            }
        }
    </script>
</body>
</html>
