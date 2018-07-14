<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="WebApplicationNeo4j.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="Index.css" rel="stylesheet" type="text/css" />
    <title>Neo4j Movie Recommendation system</title>
</head>
<body> 
    <div>
        <h1 id="logo"><a href="https://neo4j.com/"><img src="img/neo4j_logo.png" alt="logo" style="height: 71px; width: 87px" /></a> Welcome to</h1>
        <h1>Neo4j Powered Recommendations!!</h1>
    </div>
    <form id="form1" class="form-wrapper" runat="server">
        <div>
            <br />
            <asp:TextBox ID="TextBox1" class="textbox" runat="server"></asp:TextBox><br /><br />            
            <asp:Button ID="Button1" class="button" runat="server" OnClick="Button1_Click" Text="Search" />
            <br /><br />
            <br /><br />
            <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Italic="True" Font-Size="Medium" ForeColor="#003399"></asp:Label>
            <br /><br />
            <asp:Panel ID="Panel1" ScrollBars="Auto" runat="server"  HorizontalAlign="Center">
            </asp:Panel>
            <br />
        </div>
    </form> 

    
    
    
    
    </body>

</html>
