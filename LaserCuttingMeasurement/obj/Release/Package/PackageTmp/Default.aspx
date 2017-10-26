<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LaserCuttingMeasurement._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <br />
    If anything goes wrong, email Jonathan with the .dxf file, screenshot of the website, steps how to produce it, and any other anomalies.<br />
    <br />
    Upload a .dxf file for measurement.
    <asp:FileUpload ID="FileUpload1" runat="server" Height="23px" Width="250px" />
    <br />
    Team:
    <asp:DropDownList ID="teamDDL" runat="server">
        <asp:ListItem>TA</asp:ListItem>
        <asp:ListItem>A1</asp:ListItem>
        <asp:ListItem>A2</asp:ListItem>
        <asp:ListItem>A3</asp:ListItem>
        <asp:ListItem>A4</asp:ListItem>
        <asp:ListItem>A5</asp:ListItem>
        <asp:ListItem>A6</asp:ListItem>
        <asp:ListItem>A7</asp:ListItem>
        <asp:ListItem>A8</asp:ListItem>
        <asp:ListItem>A9</asp:ListItem>
        <asp:ListItem>B1</asp:ListItem>
        <asp:ListItem>B2</asp:ListItem>
        <asp:ListItem>B3</asp:ListItem>
        <asp:ListItem>B4</asp:ListItem>
        <asp:ListItem>C1</asp:ListItem>
    </asp:DropDownList>
    <br />
    Quantity: <asp:TextBox ID="quantityText" runat="server" Width="84px"></asp:TextBox>
    <br />
    Material:
    <asp:DropDownList ID="materialDDL" runat="server">
        <asp:ListItem Value="1">Acrylic 1/8&quot;</asp:ListItem>
        <asp:ListItem Value="2">Acrylic 1/4&quot;</asp:ListItem>
        <asp:ListItem Value="3">MDF 1/4&quot;</asp:ListItem>
    </asp:DropDownList>
    <br />
    <asp:Button ID="submitBtn" runat="server" OnClick="submitBtn_Click" Text="Calculate" />
    <br />
    <asp:Literal ID="Literal1" runat="server"></asp:Literal>

</asp:Content>
