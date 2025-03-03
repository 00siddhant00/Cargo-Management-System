using System;
using System.Data;
using System.Web.UI;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;

namespace CargoManagement
{
    public partial class AdminLogin : Page
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = HashPassword(txtPassword.Text.Trim());

            if (ValidateAdmin(email, password))
            {
                Session["admin_login"] = email;
                Response.Redirect("Admin.aspx");
            }
            else
            {
                lblMessage.Text = "Invalid login credentials!";
            }
        }

        private bool ValidateAdmin(string email, string hashedPassword)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM admin WHERE email = @Email AND password = @Password";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
