<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="CargoManagement.Admin" %>

<%@ Register TagPrefix="uc" Src="Header.ascx" TagName="Header" %>
<%@ Register TagPrefix="uc" Src="Footer.ascx" TagName="Footer" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Admin Panel - Cargo Management</title>
    <link rel="stylesheet" href="css/bootstrap.min.css">
    <link rel="stylesheet" type="text/css" href="css/styles.css">
</head>
<body>
    <uc:Header runat="server" />

    <div class="container mt-5">
        <h2 class="text-center">Admin Panel - Manage Cargo</h2>

        <form id="form1" runat="server">
            <div class="table-responsive">
                <asp:GridView ID="gridAdminCargo" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"
                    DataKeyNames="tracking_id"
                    OnRowEditing="gridAdminCargo_RowEditing"
                    OnRowUpdating="gridAdminCargo_RowUpdating"
                    OnRowCancelingEdit="gridAdminCargo_RowCancelingEdit"
                    OnRowCommand="gridAdminCargo_RowCommand">

                    <Columns>
                        <asp:BoundField DataField="tracking_id" HeaderText="Tracking ID" ReadOnly="True" />
                        <asp:TemplateField HeaderText="Sender">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtSenderName" runat="server" CssClass="form-control" Text='<%# Bind("sender_name") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblSenderName" runat="server" Text='<%# Eval("sender_name") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Receiver">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtReceiverName" runat="server" CssClass="form-control" Text='<%# Bind("receiver_name") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblReceiverName" runat="server" Text='<%# Eval("receiver_name") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Pickup City">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtPickup" runat="server" CssClass="form-control" Text='<%# Bind("pickup") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblPickup" runat="server" Text='<%# Eval("pickup") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Destination City">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDestination" runat="server" CssClass="form-control" Text='<%# Bind("destination") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblDestination" runat="server" Text='<%# Eval("destination") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Driver">
                            <ItemTemplate>
                                <asp:Label ID="lblDriver" runat="server" Text='<%# Eval("driver") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlDriver" runat="server"></asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>


                        <asp:TemplateField HeaderText="Status">
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                                    <asp:ListItem Text="Pending" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="In Transit" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="Delivered" Value="2"></asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("status") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:CommandField ShowEditButton="True" HeaderText="Actions" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Button ID="btnDelete" runat="server" CssClass="btn btn-sm btn-danger" CommandName="DeleteCargo" CommandArgument='<%# Eval("tracking_id") %>' Text="Delete" OnClientClick="return confirm('Are you sure?');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>


            <div class="text-center mt-3">
                <asp:Button ID="btnLogout" runat="server" CssClass="btn btn-warning" Text="Logout" OnClick="btnLogout_Click" />
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="text-danger"></asp:Label>
        </form>
    </div>

    <uc:Footer runat="server" />
</body>
</html>
