using System;
using System.Web.UI;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Web.DynamicData;
using System.Collections.Generic;

namespace CargoManagement
{
    public partial class Payment : Page
    {
        protected string trackingId;
        protected int price;

        private static readonly Dictionary<int, string> containerTypeMapping = new Dictionary<int, string>
        {
            { 0, "Liquid" },
            { 1, "Ice" },
            { 2, "Gas" },
            { 3, "Normal" }
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["user_login"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                // **Retrieve stored booking details from Session**
                lblSenderName.Text = Session["sender_name"]?.ToString();
                lblSenderEmail.Text = Session["sender_email"]?.ToString();
                lblSenderContact.Text = Session["sender_contact"]?.ToString();
                lblSenderAddress.Text = Session["sender_address"]?.ToString();
                lblReceiverName.Text = Session["receiver_name"]?.ToString();
                lblReceiverEmail.Text = Session["receiver_email"]?.ToString();
                lblReceiverContact.Text = Session["receiver_contact"]?.ToString();
                lblReceiverAddress.Text = Session["receiver_address"]?.ToString();
                lblPickup.Text = Session["pickup"]?.ToString();
                lblDestination.Text = Session["destination"]?.ToString();
                lblWeight.Text = Session["weight"]?.ToString();
                lblVolume.Text = Session["volume"]?.ToString();
                lblQuantity.Text = Session["quantity"]?.ToString();
                int containerType;
                if (int.TryParse(Session["container_type"]?.ToString(), out containerType) && containerTypeMapping.ContainsKey(containerType))
                {
                    lblContainerType.Text = containerTypeMapping[containerType - 1];
                }
                else
                {
                    lblContainerType.Text = "Unknown"; // Or any default value you want to show
                }


                // **Calculate Dynamic Price**
                CalculatePrice();
            }
        }

        private void CalculatePrice()
        {
            string pickup = Session["pickup"]?.ToString();
            string destination = Session["destination"]?.ToString();
            decimal weight = Convert.ToDecimal(Session["weight"] ?? "0");
            decimal volume = Convert.ToDecimal(Session["volume"] ?? "0");
            int quantity = Convert.ToInt32(Session["quantity"] ?? "0");
            int type = Convert.ToInt32(Session["container_type"] ?? "3"); // Default to 3 if null

            if (!string.IsNullOrEmpty(pickup) && !string.IsNullOrEmpty(destination))
            {
                int distance = 300;
                int basePrice = distance * 5;
                int weightPrice = (int)weight * 30;
                int volumePrice = (int)volume * 40;
                int quantityPrice = quantity * 20;
                int typePrice = 100; // Default for Normal

                if (type == 1)
                    typePrice = 200; // Liquid
                else if (type == 2)
                    typePrice = 300; // Ice
                else if (type == 3)
                    typePrice = 400; // Gas

                int price = basePrice + weightPrice + volumePrice + quantityPrice + typePrice;

                lblAmount.Text = "₹" + price.ToString();
                Session["payment_amount"] = price;
            }
        }


        protected void btnConfirmPayment_Click(object sender, EventArgs e)
        {
            string email = Session["user_login"]?.ToString();
            string paymentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            trackingId = Session["tracking_id"] as string;
            price = Convert.ToInt32(Session["payment_amount"] ?? 0);

            if (string.IsNullOrEmpty(trackingId) || price == 0)
            {
                lblAmount.Text = "Error: Missing payment details!";
                return;
            }

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            int userId = 0;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // **Fetch user_id explicitly before inserting payment**
                    string getUserIdQuery = "SELECT id FROM users WHERE email = @Email";
                    using (MySqlCommand cmd = new MySqlCommand(getUserIdQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            userId = Convert.ToInt32(result);
                        }
                        else
                        {
                            lblAmount.Text = "Error: User not found!";
                            return;
                        }
                    }

                    // **Insert Cargo details into DB only when Confirm Payment is clicked**
                    string cargoQuery = "INSERT INTO cargo (tracking_id, user_id, sender_name, sender_email, sender_contact, sender_address, sender_city, " +
                                        "receiver_name, receiver_email, receiver_contact, receiver_address, receiver_city, weight, volume, quantity, container_type, status) " +
                                        "VALUES (@TrackingId, @UserId, @SenderName, @SenderEmail, @SenderContact, @SenderAddress, @Pickup, " +
                                        "@ReceiverName, @ReceiverEmail, @ReceiverContact, @ReceiverAddress, @Destination, @Weight, @Volume, @Quantity, @ContainerType, 0)";

                    using (MySqlCommand cmd = new MySqlCommand(cargoQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@TrackingId", trackingId);
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@SenderName", Session["sender_name"]?.ToString() ?? DBNull.Value.ToString());
                        cmd.Parameters.AddWithValue("@SenderEmail", string.IsNullOrEmpty(Session["sender_email"]?.ToString()) ? (object)DBNull.Value : Session["sender_email"]);
                        cmd.Parameters.AddWithValue("@SenderContact", Session["sender_contact"]?.ToString() ?? DBNull.Value.ToString());
                        cmd.Parameters.AddWithValue("@SenderAddress", Session["sender_address"]?.ToString() ?? DBNull.Value.ToString());
                        cmd.Parameters.AddWithValue("@ReceiverName", Session["receiver_name"]?.ToString() ?? DBNull.Value.ToString());
                        cmd.Parameters.AddWithValue("@ReceiverEmail", string.IsNullOrEmpty(Session["receiver_email"]?.ToString()) ? (object)DBNull.Value : Session["receiver_email"]);
                        cmd.Parameters.AddWithValue("@ReceiverContact", Session["receiver_contact"]?.ToString() ?? DBNull.Value.ToString());
                        cmd.Parameters.AddWithValue("@ReceiverAddress", Session["receiver_address"]?.ToString() ?? DBNull.Value.ToString());
                        cmd.Parameters.AddWithValue("@Pickup", Session["pickup"]?.ToString() ?? DBNull.Value.ToString());
                        cmd.Parameters.AddWithValue("@Destination", Session["destination"]?.ToString() ?? DBNull.Value.ToString());
                        cmd.Parameters.AddWithValue("@Weight", Convert.ToDecimal(Session["weight"] ?? 0));
                        cmd.Parameters.AddWithValue("@Volume", Convert.ToDecimal(Session["volume"] ?? 0));
                        cmd.Parameters.AddWithValue("@Quantity", Convert.ToInt32(Session["quantity"] ?? 0));
                        cmd.Parameters.AddWithValue("@ContainerType", Session["container_type"]?.ToString() ?? DBNull.Value.ToString());
                        cmd.ExecuteNonQuery();
                    }


                    // **Insert payment details**
                    string paymentQuery = "INSERT INTO payments (tracking_id, user_id, amount, payment_date) " +
                                          "VALUES (@TrackingId, @UserId, @Amount, @PaymentDate)";
                    using (MySqlCommand cmd = new MySqlCommand(paymentQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@TrackingId", trackingId);
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@Amount", price);
                        cmd.Parameters.AddWithValue("@PaymentDate", paymentDate);
                        cmd.ExecuteNonQuery();
                    }
                }

                // **Redirect after successful payment**
                Response.Redirect("Cargo.aspx");
            }
            catch (Exception ex)
            {
                lblAmount.Text = "Error: " + ex.Message;
            }
        }
    }
}