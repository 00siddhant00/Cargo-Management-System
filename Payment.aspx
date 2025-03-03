<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Payment.aspx.cs" Inherits="CargoManagement.Payment" %>

<%@ Register TagPrefix="uc" Src="Header.ascx" TagName="Header" %>
<%@ Register TagPrefix="uc" Src="Footer.ascx" TagName="Footer" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Payment - Cargo Management</title>
    <link rel="stylesheet" href="css/bootstrap.min.css">
    <link rel="stylesheet" href="css/styles.css">
</head>
<body>

    <form id="form1" runat="server">
        <!-- 🔹 Load Header -->
        <uc:Header runat="server" />

        <div class="container mt-5">
            <div class="card shadow-lg p-4">
                <h2 class="text-center mb-4">Confirm Payment</h2>

                <!-- Sender & Receiver Details -->
                <div class="row">
                    <div class="col-md-6">
                        <div class="card p-3 mb-3">
                            <h4 class="text-primary">Sender Details</h4>
                            <div class="mb-2"><strong>Name:</strong>
                                <asp:Label ID="lblSenderName" runat="server" CssClass="text-dark"></asp:Label></div>
                            <div class="mb-2"><strong>Email:</strong>
                                <asp:Label ID="lblSenderEmail" runat="server" CssClass="text-dark"></asp:Label></div>
                            <div class="mb-2"><strong>Contact:</strong>
                                <asp:Label ID="lblSenderContact" runat="server" CssClass="text-dark"></asp:Label></div>
                            <div><strong>Address:</strong>
                                <asp:Label ID="lblSenderAddress" runat="server" CssClass="text-dark"></asp:Label></div>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="card p-3 mb-3">
                            <h4 class="text-primary">Receiver Details</h4>
                            <div class="mb-2"><strong>Name:</strong>
                                <asp:Label ID="lblReceiverName" runat="server" CssClass="text-dark"></asp:Label></div>
                            <div class="mb-2"><strong>Email:</strong>
                                <asp:Label ID="lblReceiverEmail" runat="server" CssClass="text-dark"></asp:Label></div>
                            <div class="mb-2"><strong>Contact:</strong>
                                <asp:Label ID="lblReceiverContact" runat="server" CssClass="text-dark"></asp:Label></div>
                            <div><strong>Address:</strong>
                                <asp:Label ID="lblReceiverAddress" runat="server" CssClass="text-dark"></asp:Label></div>
                        </div>
                    </div>
                </div>

                <!-- Cargo Details -->
                <div class="card p-3 mb-3">
                    <h4 class="text-primary">Cargo Details</h4>
                    <div class="row">
                        <div class="col-md-6">
                            <div class="mb-2"><strong>Pickup Location:</strong>
                                <asp:Label ID="lblPickup" runat="server"></asp:Label></div>
                            <div class="mb-2"><strong>Destination:</strong>
                                <asp:Label ID="lblDestination" runat="server"></asp:Label></div>
                            <div class="mb-2"><strong>Weight:</strong>
                                <asp:Label ID="lblWeight" runat="server"></asp:Label>
                                kg</div>
                            <div class="mb-2"><strong>Container Type:</strong>
                                <asp:Label ID="lblContainerType" runat="server"></asp:Label></div>
                        </div>
                        <div class="col-md-6">
                            <div class="mb-2"><strong>Volume:</strong>
                                <asp:Label ID="lblVolume" runat="server"></asp:Label>
                                m³</div>
                            <div class="mb-2"><strong>Quantity:</strong>
                                <asp:Label ID="lblQuantity" runat="server"></asp:Label></div>
                        </div>
                    </div>
                </div>

                <!-- Payment Section -->
                <div class="card p-3 mb-4">
                    <h4 class="text-danger">Payment Details</h4>
                    <div class="mb-3">
                        <strong>Amount:</strong>
                        <asp:Label ID="lblAmount" runat="server" CssClass="fw-bold text-success fs-5"></asp:Label>
                    </div>
                    <asp:Button ID="btnConfirmPayment" runat="server" CssClass="btn btn-success btn-lg w-100" Text="Confirm Payment" OnClick="btnConfirmPayment_Click" />
                </div>
            </div>
        </div>

        <!-- 🔹 Load Footer -->
        <uc:Footer runat="server" />
    </form>

</body>
</html>
