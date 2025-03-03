using System;
using System.Web.UI;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace CargoManagement
{
    public partial class Login : Page
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // **Fetch user details based on email**
                    string query = "SELECT id, fullname, password FROM users WHERE email = @Email";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHashedPassword = reader["password"].ToString();

                                // **Verify the entered password with the stored hashed password**
                                if (BCrypt.Net.BCrypt.Verify(password, storedHashedPassword))
                                {
                                    // **Set session variables**
                                    Session["user_login"] = email;
                                    Session["user_id"] = reader["id"].ToString();
                                    Session["user_name"] = reader["fullname"].ToString();

                                    // **Redirect to homepage**
                                    Response.Redirect("Cargo.aspx");
                                }
                                else
                                {
                                    lblMessage.Text = "Invalid email or password.";
                                    lblMessage.ForeColor = System.Drawing.Color.Red;
                                }
                            }
                            else
                            {
                                lblMessage.Text = "User not found!";
                                lblMessage.ForeColor = System.Drawing.Color.Red;
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                lblMessage.Text = "Database error: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}
