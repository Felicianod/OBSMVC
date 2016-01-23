<% Response.StatusCode = 404 %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="404.aspx.cs" Inherits="OBSMVC.ErrorPages._404" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <br />
        <div style="text-align: center">
            The Page you are requesting does not exist
        </div>
        <br />
        <div style="text-align: center">
            Please review your URL/Link and try your request again.<br />
            If you continue experiencing this problem,
        please contatc the Service Desk for assistance.<br />
        </div>
        <br />
        <br />
        <div style="text-align: center">
            - - - - REQUESTED PAGE NOT FOUND - - - - - 
        </div>
    </form>
</body>
</html>
