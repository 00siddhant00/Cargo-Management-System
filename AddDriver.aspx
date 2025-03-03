<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddDriver.aspx.cs" Inherits="CargoManagement.AddDriver" %>

<%@ Register TagPrefix="uc" Src="~/Header.ascx" TagName="Header" %>
<%@ Register TagPrefix="uc" Src="~/Footer.ascx" TagName="Footer" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Add Driver - Cargo Management</title>
    <link rel="stylesheet" href="css/bootstrap.min.css">
    <link rel="stylesheet" href="css/styles.css">
</head>
<body>

    <form id="form1" runat="server">
        <uc:Header runat="server" />

        <div class="container mt-5">
            <h2 class="d-flex align-items-center">Add Driver
                <asp:Label ID="lblSuccessMessage" runat="server" CssClass="ms-3 alert alert-success d-none" role="alert"></asp:Label>
            </h2>

            <asp:Label ID="lblMessage" runat="server" ForeColor="Red"></asp:Label>

            <div class="mb-3">
                <label>Full Name:</label>
                <asp:TextBox ID="txtFullname" runat="server" CssClass="form-control"></asp:TextBox>
            </div>

            <div class="mb-3">
                <label>Contact Number:</label>
                <asp:TextBox ID="txtContact" runat="server" CssClass="form-control"></asp:TextBox>
            </div>

            <div class="mb-3">
                <label>Email:</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"></asp:TextBox>
            </div>

            <asp:Button ID="btnAddDriver" runat="server" CssClass="btn btn-primary" Text="Add Driver" OnClick="btnAddDriver_Click" />

        </div>

        <uc:Footer runat="server" />
    </form>

    <script>
        function showSuccessMessage() {
            var messageLabel = document.getElementById('<%= lblSuccessMessage.ClientID %>');
            if (messageLabel) {
                messageLabel.classList.remove("d-none");
                messageLabel.innerText = "Driver successfully added!";
            }
        }
    </script>

</body>
</html>
