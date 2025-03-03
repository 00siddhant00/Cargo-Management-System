using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace CargoManagement
{
    public partial class Admin : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCargoData();
            }
        }

        private void LoadCargoData()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
            SELECT c.tracking_id, c.sender_name, c.receiver_name, 
                   c.sender_city AS pickup, c.receiver_city AS destination, 
                   c.status, d.fullname AS driver
            FROM cargo c
            LEFT JOIN driver d ON c.driver_id = d.id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        gridAdminCargo.DataSource = dt;
                        gridAdminCargo.DataBind();
                    }
                }
            }
        }


        private void LoadDrivers(DropDownList ddlDrivers)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT id, fullname FROM driver WHERE status = 0";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();
                    using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        ddlDrivers.DataSource = dt;
                        ddlDrivers.DataTextField = "fullname";
                        ddlDrivers.DataValueField = "id";
                        ddlDrivers.DataBind();
                        ddlDrivers.Items.Insert(0, new ListItem("-- Select a Driver --", "0"));

                    }
                }
            }

            ddlDrivers.Items.Insert(0, new ListItem("-- Select a Driver --", "0")); // ✅ Fixed typo
        }


        // ✅ Fix: Add Logout Method
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Abandon(); // Destroy session
            Response.Redirect("AdminLogin.aspx"); // Redirect to login page
        }

        // ✅ Allows Editing a Row
        protected void gridAdminCargo_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gridAdminCargo.EditIndex = e.NewEditIndex;
            LoadCargoData();

            GridViewRow row = gridAdminCargo.Rows[e.NewEditIndex];
            DropDownList ddlDriver = (DropDownList)row.FindControl("ddlDriver");

            if (ddlDriver != null)
            {
                LoadDrivers(ddlDriver);
                ddlDriver.SelectedValue = gridAdminCargo.DataKeys[e.NewEditIndex].Values["tracking_id"].ToString();
            }
        }


        // ✅ Cancels Editing
        protected void gridAdminCargo_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gridAdminCargo.EditIndex = -1;
            LoadCargoData();
        }

        // ✅ Saves Updated Row Data
        protected void gridAdminCargo_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = gridAdminCargo.Rows[e.RowIndex];
            string trackingId = gridAdminCargo.DataKeys[e.RowIndex].Value.ToString();

            string senderName = (row.FindControl("txtSenderName") as TextBox).Text.Trim();
            string receiverName = (row.FindControl("txtReceiverName") as TextBox).Text.Trim();
            string pickup = (row.FindControl("txtPickup") as TextBox).Text.Trim();
            string destination = (row.FindControl("txtDestination") as TextBox).Text.Trim();
            int status = Convert.ToInt32((row.FindControl("ddlStatus") as DropDownList).SelectedValue);
            DropDownList ddlDriver = (DropDownList)row.FindControl("ddlDriver");
            int driverId = Convert.ToInt32(ddlDriver.SelectedValue);

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE cargo SET sender_name=@Sender, receiver_name=@Receiver, sender_city=@Pickup, receiver_city=@Destination, status=@Status, driver_id=@DriverId WHERE tracking_id=@TrackingId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Sender", senderName);
                    cmd.Parameters.AddWithValue("@Receiver", receiverName);
                    cmd.Parameters.AddWithValue("@Pickup", pickup);
                    cmd.Parameters.AddWithValue("@Destination", destination);
                    cmd.Parameters.AddWithValue("@DriverId", driverId);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@TrackingId", trackingId);

                    cmd.ExecuteNonQuery();
                }
            }

            gridAdminCargo.EditIndex = -1;
            LoadCargoData();
        }

        // ✅ Fix: Add Missing `RowCommand` Method for Delete and Status Update
        protected void gridAdminCargo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string trackingId = e.CommandArgument.ToString();

            if (e.CommandName == "UpdateStatus")
            {
                UpdateCargoStatus(trackingId);
            }
            else if (e.CommandName == "DeleteCargo")
            {
                DeleteCargo(trackingId);
            }
        }

        // ✅ Update Status of Cargo
        private void UpdateCargoStatus(string trackingId)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE cargo SET status = CASE WHEN status = 0 THEN 1 WHEN status = 1 THEN 2 ELSE 2 END WHERE tracking_id = @TrackingId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TrackingId", trackingId);
                    cmd.ExecuteNonQuery();
                }
            }

            LoadCargoData();
        }

        // ✅ Delete Cargo
        private void DeleteCargo(string trackingId)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM cargo WHERE tracking_id = @TrackingId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TrackingId", trackingId);
                    cmd.ExecuteNonQuery();
                }
            }

            LoadCargoData();
        }
    }
}
