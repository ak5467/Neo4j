<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NeoInfo.aspx.cs" Inherits="WebApplicationNeo4j.NeoInfo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Information Page</title>
    <link href="NeoInfo.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">    
        <asp:Button ID="backButton" class="button" runat="server" Text="Back" OnClick="backButton_Click" />  
            <br />
            <h2 class="headings">Here is the movie you searched for: </h2>
            <br />
            <div>
                <br />
                <asp:Panel ID="moviePanel1" runat="server" BackColor="#92C5C7" BorderStyle="Outset" Font-Bold="True" Font-Italic="False" Font-Size="X-Large" ></asp:Panel>
            </div>
            <br />
            <div>
                <asp:Panel ID="moviePanel2" runat="server" BackColor="#DEC9DC" Font-Size="Large" ForeColor="#003300"></asp:Panel>
            </div>
            <br />
            <div>
                <asp:Panel ID="moviePanel3" runat="server" BackColor="#92C5C7" Font-Size="Large" ForeColor="#660066"></asp:Panel>
            </div>
            <br />
            <div>
                <asp:Panel ID="moviePanel4" runat="server" BackColor="#DEC9DC" Font-Size="Large" ForeColor="#003300"></asp:Panel>
            </div>
            <br />
            <div>
                <asp:Panel ID="moviePanel5" runat="server" BackColor="#92C5C7" Font-Size="Large" ForeColor="#660066"></asp:Panel>
            </div>
            <br />        
    </form>
</body>
</html>
