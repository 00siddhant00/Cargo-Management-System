using System;
using System.Web.UI;

namespace CargoManagement
{
    public partial class Header : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadNavLinks();
            }
        }

        private void LoadNavLinks()
        {
            string currentPage = System.IO.Path.GetFileName(Request.Url.AbsolutePath).ToLower();
            string links = "<li class='nav-item'><a class='nav-link' href='Default.aspx'>Home</a></li>"; // Always show Home

            if (Session["user_login"] != null) // User is logged in
            {
                links += @"
                        <li class='nav-item'><a class='nav-link' href='Cargo.aspx'>Cargos</a></li>
                        <li class='nav-item'><a class='nav-link' href='profile.aspx'>My Profile</a></li>
                        <li class='nav-item'><a class='nav-link' href='logout.aspx'>Logout</a></li>";
            }
            else // User is not logged in
            {
                if (currentPage.Contains("admin") || currentPage.Contains("driver")) // Admin/Driver login pages
                {
                    links += @"
                        <li class='nav-item'><a class='nav-link' href='AdminLogin.aspx'>Admin Login</a></li>
                        <li class='nav-item'><a class='nav-link' href='ManageAdmins.aspx'>Admin Register</a></li>
                        <li class='nav-item'><a class='nav-link' href='AddDriver.aspx'>Add Driver</a></li>";
                }
                else // Other pages (general users)
                {
                    links += @"
                        <li class='nav-item'><a class='nav-link' href='Login.aspx'>Login</a></li>
                        <li class='nav-item'><a class='nav-link' href='Register.aspx'>Register</a></li>";
                }
            }

            navLinks.Text = links;
        }
    }
}
