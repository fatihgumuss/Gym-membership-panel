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
    public partial class MemberAdd : Form
    {
        public MemberAdd()
        {
            InitializeComponent();
        }
        OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\\datam.accdb");

        private void MemberAdd_Load(object sender, EventArgs e)
        {

        }

        private void btnAddMember_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtSurname.Text) || string.IsNullOrEmpty(txtMail.Text) || string.IsNullOrEmpty(txtAge.Text) || comboBox1.SelectedIndex == -1 || string.IsNullOrEmpty(txtMembershipMonths.Text))
            {
                MessageBox.Show("Please fill in all the fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (int.TryParse(txtMembershipMonths.Text, out int months))
            {
                AddMember(months);

                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter a valid numeric value for the month.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddMember(int months)
        {
            try
            {
                DateTime currentDate = DateTime.Today;
                DateTime endDate = currentDate.AddMonths(months);

                OleDbCommand cmd = new OleDbCommand("INSERT INTO members (isim, soyisim, mail, yaş, başlangıç, bitiş, cinsiyet) VALUES (@isim, @soyisim, @mail, @yaş, @başlangıç, @bitiş, @cinsiyet)", con);
                cmd.Parameters.AddWithValue("@isim", txtName.Text);
                cmd.Parameters.AddWithValue("@soyisim", txtSurname.Text);
                cmd.Parameters.AddWithValue("@mail", txtMail.Text);
                cmd.Parameters.AddWithValue("@yaş", txtAge.Text);
                cmd.Parameters.AddWithValue("@başlangıç", currentDate);
                cmd.Parameters.AddWithValue("@bitiş", endDate);
                cmd.Parameters.AddWithValue("@cinsiyet", comboBox1.Text);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("Member added successfully.","Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding member: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
