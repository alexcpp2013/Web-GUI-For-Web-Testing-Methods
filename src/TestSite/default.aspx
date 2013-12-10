﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default1.aspx.cs" Inherits="UnitSite.TestPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
    <title>MSUnit Тест Сайтов КПИ</title>
</head>

<body style="height: 836px; width: 944px;">
    <form id="form1" runat="server" enctype="multipart/form-data">
    <div class="page" style="height: 63px; width: 1003px;">
        <asp:Label ID="Label1" runat="server" Font-Bold="True" 
            Text="Страница запуска тестов" Font-Size="Larger" ForeColor="#0066FF" 
            style="text-align: left"></asp:Label>
        <br />
        
        <asp:ScriptManager ID="ScriptManager1" runat="server" 
            onasyncpostbackerror="ScriptManager1_AsyncPostBackError" 
            AsyncPostBackTimeout="600">
        </asp:ScriptManager>

        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            
            <ContentTemplate>

                <div style="height: 755px; width: 927px;">                    
                    <br /> <br />
                    <asp:Button ID="Button1" runat="server" Height="25px" OnClick="Button1_Click" Text="Старт" Width="157px" />
                    <asp:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="150">
                        <ProgressTemplate>
                            &nbsp;Идет обработка...
                            <img src="/Content/load.gif" style="height: 27px; width: 33px" />
                        </ProgressTemplate>
                    </asp:UpdateProgress>
                    
                    <br /> <br />
                    <table border="0" align="left" cellpadding="0" cellspacing="0" style="margin: 0px">
                        <tr>
	                        <td>
                                  <asp:Label ID="Label3" runat="server" Font-Bold="True" 
                                   Text="Сайты/Директории для проверки:"></asp:Label>
                                  <br />
                                  <asp:FileUpload ID="FileUpload1" runat="server" 
                                   Height="23px" Width="210px" />
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" 
                                     ControlToValidate="FileUpload1" ErrorMessage="Только .xml" 
                                     ValidationExpression="^.*\.(xml)$" runat="server" />
                                    &nbsp;
                                  <asp:Button ID="Button2" runat="server" Height="23px" 
                                   OnClick="Button2_Click" Text="Загрузить" Width="81px" />
                                  <br />
                                  <asp:TextBox ID="tbSites" runat="server" Height="100px" TextMode="MultiLine" 
                                   Width="375px" BorderStyle="Inset"></asp:TextBox>
                            </td>
                            <td width="50"> </td>
	                        <td> 
                                  <!-- <br /> <br /> -->
                                  <asp:Label ID="Label4" runat="server" Font-Bold="True" 
                                   Text="Атрибуты для проверки:"></asp:Label>
                                  <br />
                                  <asp:FileUpload ID="FileUpload2" runat="server" 
                                   Height="23px" Width="210px" />
                                  <asp:RegularExpressionValidator ID="RegularExpressionValidator2" 
                                   runat="server" ControlToValidate="FileUpload2" 
                                   ErrorMessage="Только .xml" ValidationExpression="^.*\.(xml)$" />
                                  &nbsp;
                                  <asp:Button ID="Button3" runat="server" 
                                   Height="23px" OnClick="Button3_Click" Text="Загрузить" Width="81px" />
                                  <br />
                                  <asp:TextBox ID="tbTags" runat="server" BorderStyle="Inset" 
                                   Height="100px" TextMode="MultiLine" Width="375px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <br style="clear: both" />
                    
                    <br /> <br />
                    <asp:Label ID="Label5" runat="server" Text="Выбор метода:" Font-Bold="True"></asp:Label>
                    <br />   
                    <asp:DropDownList ID="ddList" runat="server" Height="25px" Width="425px">
                        <asp:ListItem Selected="True" Value="0">Тест &#39;Заблокировал ли Google указанный сайт&#39;</asp:ListItem>
                        <asp:ListItem Value="1">Тест &#39;Вывести все мета-теги сайта&#39;</asp:ListItem>
                        <asp:ListItem Value="2">Тест &#39;Какие из базовых мета-тегов присутствуют на сайте&#39;</asp:ListItem>
                        <asp:ListItem Value="3">Тест &#39;Какие из указанных мета-тегов присутствуют на сайте&#39;</asp:ListItem>
                        <asp:ListItem Value="4">Тест &#39;Дата прошлого изменения файлов сайта&#39;</asp:ListItem>
                        <asp:ListItem Value="5">Тест &#39;Дата прошлого изменения файлов на диске&#39;</asp:ListItem>
                        <asp:ListItem Value="6">Тест &#39;Данные с push2check.com&#39;</asp:ListItem>
                        <asp:ListItem Value="7">Тест &#39;Данные о количестве файлов на сайте от Google&#39;</asp:ListItem>
                        <asp:ListItem Value="8">Тест &#39;Данные из robots.txt&#39;</asp:ListItem>
                    </asp:DropDownList>

                    <br /> <br />
                    <asp:Button ID="Button4" runat="server" onclick="Button4_Click" 
                        Text="Сохранить результат" Width="160px" Height="26px" />
                    <br /> 
                    <asp:Button ID="Button5" runat="server" Height="26px" onclick="Button5_Click" 
                        Text="Отправить на почту" Width="160px" />
                    &nbsp; <asp:TextBox ID="tbMail" runat="server" Width="250px"></asp:TextBox>
                    <br /> <br />
                    <asp:Label ID="Label2" runat="server" Font-Bold="True" Text="Результат:"></asp:Label>
                    <br />
                    <asp:TextBox ID="tbResult" runat="server" BorderStyle="Inset" Height="265px" 
                        ReadOnly="True" TextMode="MultiLine" Width="910px"></asp:TextBox>
                </div>
            </ContentTemplate>
            
            <Triggers>
                <asp:PostBackTrigger ControlID="Button2" />
                <asp:PostBackTrigger ControlID="Button3" />
                <asp:PostBackTrigger ControlID="Button4" />
                <asp:PostBackTrigger ControlID="Button5" />
            </Triggers>

        </asp:UpdatePanel> 

    </div>
    </form>
</body>
</html>