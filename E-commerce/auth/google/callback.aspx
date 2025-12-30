<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="callback.aspx.cs" Inherits="Ecommerce.Auth.Google.Callback" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Google Callback</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel ID="pnlError" runat="server" Visible="false">
            <asp:Literal ID="litError" runat="server"></asp:Literal>
        </asp:Panel>
    </form>
</body>
</html>