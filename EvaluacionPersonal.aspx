<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EvaluacionPersonal.aspx.cs" Inherits="EmpresaProyecto.EvaluacionPersonal" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Evaluación del Personal</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #ffffff;
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
            color: #333;
        }

        .dropdown {
            width: 100%;
            padding: 10px;
            margin-top: 10px;
            border-radius: 5px;
            border: 1px solid #ccc;
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

        .question {
            margin-top: 15px;
            font-weight: bold;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="header-title">Evaluación del Personal</div>


            <label for="ddlEmpleados">Seleccione un empleado:</label>
            <asp:DropDownList ID="ddlEmpleados" runat="server" CssClass="dropdown">
                <asp:ListItem Text="Seleccione un empleado" Value="0" />
            </asp:DropDownList>


            <asp:Button ID="btnEvaluarEmpleado" runat="server" Text="Evaluar Empleado" CssClass="btn" OnClick="btnEvaluarEmpleado_Click" />


            <div id="formEvaluacion" runat="server" visible="false">
                <div class="question">1. Puntualidad del empleado</div>
                <asp:RadioButtonList ID="rblPuntualidad" runat="server">
                    <asp:ListItem Text="Excelente" Value="Excelente" />
                    <asp:ListItem Text="Deficiente" Value="Deficiente" />
                </asp:RadioButtonList>

                <div class="question">2. ¿Cumple el empleado con sus responsabilidades y tareas asignadas?</div>
                <asp:RadioButtonList ID="rblResponsabilidad" runat="server">
                    <asp:ListItem Text="Excelente" Value="Excelente" />
                    <asp:ListItem Text="Deficiente" Value="Deficiente" />
                </asp:RadioButtonList>

                <div class="question">3. ¿Cómo se desarrolla el empleado en trabajo en equipo?</div>
                <asp:RadioButtonList ID="rblTrabajoEquipo" runat="server">
                    <asp:ListItem Text="Excelente" Value="Excelente" />
                    <asp:ListItem Text="Deficiente" Value="Deficiente" />
                </asp:RadioButtonList>


                <div class="question">4. ¿Cómo es la disposición del empleado para aprender y mejorar?</div>
                <asp:RadioButtonList ID="rblDisposicionAprendizaje" runat="server">
                    <asp:ListItem Text="Excelente" Value="Excelente" />
                    <asp:ListItem Text="Deficiente" Value="Deficiente" />
                </asp:RadioButtonList>

                <div class="question">5. ¿Qué tan bien maneja el empleado el tiempo para completar sus tareas?</div>
                <asp:RadioButtonList ID="rblManejoTiempo" runat="server">
                    <asp:ListItem Text="Excelente" Value="Excelente" />
                    <asp:ListItem Text="Deficiente" Value="Deficiente" />
                </asp:RadioButtonList>

                <div class="question">6. ¿Cómo es la comunicación del empleado con sus compañeros y supervisores?</div>
                <asp:RadioButtonList ID="rblComunicacion" runat="server">
                    <asp:ListItem Text="Excelente" Value="Excelente" />
                    <asp:ListItem Text="Deficiente" Value="Deficiente" />
                </asp:RadioButtonList>

                <div class="question">7. ¿Qué tan eficiente es el empleado al resolver problemas o enfrentar situaciones imprevistas?</div>
                <asp:RadioButtonList ID="rblResolucionProblemas" runat="server">
                    <asp:ListItem Text="Excelente" Value="Excelente" />
                    <asp:ListItem Text="Deficiente" Value="Deficiente" />
                </asp:RadioButtonList>

                <div class="question">8. ¿Cómo calificaría el compromiso del empleado con los valores y objetivos de la empresa?</div>
                <asp:RadioButtonList ID="rblCompromiso" runat="server">
                    <asp:ListItem Text="Excelente" Value="Excelente" />
                    <asp:ListItem Text="Deficiente" Value="Deficiente" />
                </asp:RadioButtonList>

                <asp:Button ID="btnGuardarEvaluacion" runat="server" Text="Guardar Evaluación" CssClass="btn" OnClick="btnGuardarEvaluacion_Click" />
                <asp:Button ID="btnSalir" runat="server" Text="Salir" CssClass="btn" OnClick="btnSalir_Click" />
            </div>
        </div>
    </form>
</body>
</html>


