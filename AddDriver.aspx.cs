using System;
using System.Web.UI;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace CargoManagement
{
    public partial class AddDriver : Page
    {
        protected void btnAddDriver_Click(object sender, EventArgs e)
        {
            string fullname = txtFullname.Text.Trim();
            string contact = txtContact.Text.Trim();
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(contact) || string.IsNullOrEmpty(email))
            {
                lblMessage.Text = "All fields are required!";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                lblMessage.Text = "Database connection string is missing.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if the email already exists
                    string checkQuery = "SELECT COUNT(*) FROM driver WHERE email = @Email";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", email);
                        int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (exists > 0)
                        {
                            lblMessage.Text = "Email is already registered!";
                            lblMessage.ForeColor = System.Drawing.Color.Red;
                            return;
                        }
                    }

                    // Insert new driver
                    string insertQuery = "INSERT INTO driver (fullname, contact, email) VALUES (@Fullname, @Contact, @Email)";
                    using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Fullname", fullname);
                        cmd.Parameters.AddWithValue("@Contact", contact);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.ExecuteNonQuery();
                        // Show success message
                        lblSuccessMessage.CssClass = "ms-3 alert alert-success"; // Remove "d-none" to make it visible
                        lblSuccessMessage.Text = "Driver successfully added!";

                        // Call the JavaScript function
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "showMessage", "showSuccessMessage();", true);
                    }
                }
            }
            catch (MySqlException ex)
            {
                lblMessage.Text = "Database error: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Unexpected error: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}
