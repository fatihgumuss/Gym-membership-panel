using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VisProje3
{
    public partial class LoginPage : Form
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\\datam.accdb");
        private void LoginPage_Load(object sender, EventArgs e)
        {
            usernameText.Focus();
        }
        private void loginCheck()
        {
            string username = usernameText.Text;
            string password = passwordText.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            OleDbCommand command = new OleDbCommand("Select * from admin Where username = @username and password = @password", con);
            command.Parameters.AddWithValue("username", username);
            command.Parameters.AddWithValue("password", password);

            con.Open();
            OleDbDataReader reader = command.ExecuteReader();

            try
            {
                if (reader.Read())
                {
                    MessageBox.Show("Welcome " + username.ToUpper());
                    this.Hide();
                    AdminPage mainPage = new AdminPage();
                    mainPage.Show();
                }
                else
                {
                    MessageBox.Show("Incorrect username or password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    usernameText.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }

        }
        private void loginBtn_Click(object sender, EventArgs e)
        {
            loginCheck();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                passwordText.Focus();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                loginBtn.PerformClick();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
