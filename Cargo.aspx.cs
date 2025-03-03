using System;
using System.Data;
using System.Web.UI;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace CargoManagement
{
    public partial class Cargo : Page
    {
        // Distance dictionary (in km) for major Maharashtra locations
        private static readonly Dictionary<(string, string), int> cityDistances = new Dictionary<(string, string), int>
        {
            { ("Mumbai", "Pune"), 150 },
            { ("Mumbai", "Nagpur"), 800 },
            { ("Mumbai", "Nashik"), 180 },
            { ("Mumbai", "Thane"), 30 },
            { ("Mumbai", "Aurangabad"), 335 },
            { ("Pune", "Nagpur"), 720 },
            { ("Pune", "Nashik"), 210 },
            { ("Pune", "Thane"), 145 },
            { ("Pune", "Aurangabad"), 240 },
            { ("Nagpur", "Nashik"), 700 },
            { ("Nagpur", "Thane"), 770 },
            { ("Nagpur", "Aurangabad"), 500 },
            { ("Nashik", "Thane"), 120 },
            { ("Nashik", "Aurangabad"), 200 },
            { ("Thane", "Aurangabad"), 320 }
        };

        private static readonly Dictionary<int, string> containerTypeMapping = new Dictionary<int, string>
        {
            { 1, "Liquid" },
            { 2, "Ice" },
            { 3, "Gas" },
            { 0, "Normal" }
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user_login"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                LoadActiveDeliveries();
                LoadMaharashtraLocations();
                LoadContainerTypes();
                CalculateTotalCost();
            }
        }

        private void LoadActiveDeliveries()
        {
            // Ensure the user is logged in
            if (Session["user_id"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            int userId = Convert.ToInt32(Session["user_id"]);

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT tracking_id, sender_name, sender_city AS pickup, receiver_name, receiver_city AS destination, weight, volume, quantity, 
                            CASE 
                                WHEN status = 0 THEN 'Pending' 
                                WHEN status = 1 THEN 'In Transit' 
                                WHEN status = 2 THEN 'Delivered' 
                                ELSE 'Unknown' 
                            END AS status 
                            FROM cargo 
                            WHERE user_id = @user_id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@user_id", userId); // Bind user ID

                        using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            gridCargo.DataSource = dt;
                            gridCargo.DataBind();
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error loading deliveries: " + ex.Message;
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
        }


        private void LoadMaharashtraLocations()
        {
            string[] locations = { "Mumbai", "Pune", "Nagpur", "Nashik", "Thane", "Aurangabad" };
            ddlPickup.Items.Clear();
            ddlDestination.Items.Clear();

            foreach (string loc in locations)
            {
                ddlPickup.Items.Add(loc);
                ddlDestination.Items.Add(loc);
            }
        }

        private int GetDistance(string pickup, string destination)
        {
            if (cityDistances.TryGetValue((pickup, destination), out int distance) ||
                cityDistances.TryGetValue((destination, pickup), out distance))
            {
                return distance;
            }
            return 300; // Default if not found
        }

        protected void btnBookCargo_Click(object sender, EventArgs e)
        {
            string senderName = txtSenderName.Text.Trim();
            string senderEmail = txtSenderEmail.Text.Trim();
            string senderContact = txtSenderContact.Text.Trim();
            string senderAddress = txtSenderAddress.Text.Trim();
            string receiverName = txtReceiverName.Text.Trim();
            string receiverEmail = txtReceiverEmail.Text.Trim();
            string receiverContact = txtReceiverContact.Text.Trim();
            string receiverAddress = txtReceiverAddress.Text.Trim();
            string pickup = ddlPickup.SelectedValue;
            string destination = ddlDestination.SelectedValue;
            string weight = txtWeight.Text.Trim();
            string volume = txtVolume.Text.Trim();
            string quantity = txtQuantity.Text.Trim();
            string containerType = ddlContainerTypes.SelectedValue; // Added container type

            // **Ensure required fields are filled**
            if (new[] { senderName, senderContact, senderAddress, receiverName, receiverContact, receiverAddress, pickup, destination, weight, volume, quantity, containerType }.Any(string.IsNullOrWhiteSpace))
            {
                lblMessage.Text = "All fields except sender and receiver email must be filled!";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // **Ensure numeric values for weight, volume, and quantity**
            if (!decimal.TryParse(weight, out decimal parsedWeight) || parsedWeight <= 0 ||
                !decimal.TryParse(volume, out decimal parsedVolume) || parsedVolume <= 0 ||
                !int.TryParse(quantity, out int parsedQuantity) || parsedQuantity <= 0)
            {
                lblMessage.Text = "Weight, Volume, and Quantity must be valid positive numbers!";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            try
            {
                // **Store details in session (NO DATABASE INSERT YET)**
                Session["sender_name"] = senderName;
                Session["sender_email"] = senderEmail;
                Session["sender_contact"] = senderContact;
                Session["sender_address"] = senderAddress;
                Session["receiver_name"] = receiverName;
                Session["receiver_email"] = receiverEmail;
                Session["receiver_contact"] = receiverContact;
                Session["receiver_address"] = receiverAddress;
                Session["pickup"] = pickup;
                Session["destination"] = destination;
                Session["weight"] = parsedWeight;
                Session["volume"] = parsedVolume;
                Session["quantity"] = parsedQuantity;
                Session["container_type"] = containerType;

                // Generate tracking ID but do NOT store it in the database yet
                Session["tracking_id"] = Guid.NewGuid().ToString("N").Substring(0, 16).ToUpper();

                // Redirect to Payment page
                Response.Redirect("Payment.aspx");
            }
            catch (Exception ex)
            {
                lblMessage.Text = "An error occurred while processing your request. Please try again.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                // Log the error (if applicable)
            }
        }



        protected void gridCargo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CancelCargo")
            {
                string trackingId = e.CommandArgument.ToString();
                Response.Redirect("Cancel.aspx?tracking_id=" + trackingId);
            }
            else if (e.CommandName == "ViewInvoice")
            {
                string trackingId = e.CommandArgument.ToString();
                Response.Redirect("Invoice.aspx?tracking_id=" + trackingId);
            }
        }

        private void LoadContainerTypes()
        {
            ddlContainerTypes.Items.Clear();
            ddlContainerTypes.Items.Add(new ListItem("-- Select a Container Type --", "-1"));

            foreach (var entry in containerTypeMapping)
            {
                ddlContainerTypes.Items.Add(new ListItem(entry.Value, entry.Key.ToString()));
            }
        }


        protected void CalculateTotalCost()
        {
            if (!string.IsNullOrEmpty(txtWeight.Text) && !string.IsNullOrEmpty(txtVolume.Text) && !string.IsNullOrEmpty(txtQuantity.Text))
            {
                int price;
                double weight = Convert.ToDouble(txtWeight.Text);
                double volume = Convert.ToDouble(txtVolume.Text);
                int quantity = Convert.ToInt32(txtQuantity.Text);

                int distance = 300;
                int basePrice = distance * 5;
                int weightPrice = (int)weight * 30;
                int volumePrice = (int)volume * 40;
                int quantityPrice = quantity * 20;

                price = basePrice + weightPrice + volumePrice + quantityPrice;

                lblTotalCost.Text = price.ToString("F2");
            }
        }
    }
}
