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
using System.Xml.Linq;

namespace VisProje3
{
    public partial class AdminPage : Form
    {
        public AdminPage()
        {
            InitializeComponent();
        }
        OleDbConnection con = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\\datam.accdb");
        private void MainPage_Load(object sender, EventArgs e)
        {

            RefreshMemberList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void RefreshMemberList()
        {
            OleDbDataAdapter adp = new OleDbDataAdapter("select * from members", con);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            dataGridView1.DataSource = dt;
            dt.Columns.Add("kalan gün",typeof(int));
            if (dt.Rows.Count> 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    DateTime startDate = Convert.ToDateTime(row["başlangıç"]);
                    DateTime endDate = Convert.ToDateTime(row["bitiş"]);
                    int kalan = (endDate - DateTime.Now).Days;
                    row["kalan gün"] = kalan;
                }
            }
            else
            {
                MessageBox.Show("No user found in the database","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            adp.Dispose();

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
                {
                    DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                    textBoxId.Text = selectedRow.Cells["ID"].Value.ToString();
                    textBoxName.Text = selectedRow.Cells["isim"].Value.ToString();
                    textBoxSurname.Text = selectedRow.Cells["soyisim"].Value.ToString();
                    textBoxMail.Text = selectedRow.Cells["mail"].Value.ToString();
                    textBoxAge.Text = selectedRow.Cells["yaş"].Value.ToString();
                    if (selectedRow.Cells["cinsiyet"].Value.ToString() == "Man")
                        comboBox1.SelectedIndex = 0;
                    else 
                        comboBox1.SelectedIndex = 1;
                    
                }
        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxId.Text))
            {
                MessageBox.Show("Please select a member to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this member?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                OleDbCommand cmd = new OleDbCommand("delete from members where ID=@id", con);
                cmd.Parameters.AddWithValue("@id", textBoxId.Text);
                con.Open();

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Member deleted successfully");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting member: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    con.Close();
                    cmd.Dispose();
                    RefreshMemberList();
                }
            }
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxId.Text) || string.IsNullOrEmpty(textBoxName.Text) || string.IsNullOrEmpty(textBoxSurname.Text) || string.IsNullOrEmpty(textBoxMail.Text) || string.IsNullOrEmpty(textBoxAge.Text) || comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill in all the fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            con.Open();
            OleDbCommand cmd = new OleDbCommand("update members set isim = @isim, soyisim = @soyisim, mail=@mail, yaş = @age, cinsiyet = @gender where ID = @id", con);
            cmd.Parameters.AddWithValue("@isim", textBoxName.Text);
            cmd.Parameters.AddWithValue("@soyisim", textBoxSurname.Text);
            cmd.Parameters.AddWithValue("@mail", textBoxMail.Text);
            cmd.Parameters.AddWithValue("@age", textBoxAge.Text);
            cmd.Parameters.AddWithValue("@gender",comboBox1.SelectedItem.ToString());
            cmd.Parameters.AddWithValue("@id", textBoxId.Text);
            cmd.ExecuteNonQuery();
            con.Close();
            cmd.Dispose();
            MessageBox.Show("Member information successfully updated","Success",MessageBoxButtons.OK,MessageBoxIcon.Information);
            RefreshMemberList();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void addMonth(int a)
        {
            if (string.IsNullOrEmpty(textBoxId.Text))
            {
                MessageBox.Show("Please select a member to add months.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DateTime currentDate;
            OleDbCommand cmd = new OleDbCommand("select bitiş from members where ID = @id", con);
            cmd.Parameters.AddWithValue("@id", textBoxId.Text);
            con.Open();
            var result = cmd.ExecuteScalar();
            con.Close();
            currentDate = Convert.ToDateTime(result);
            DateTime newEnd = currentDate.AddMonths(a);
            OleDbCommand cmd2 = new OleDbCommand("update members set bitiş = @bitiş where ID = @id", con);
            cmd2.Parameters.AddWithValue("@bitiş", newEnd);
            cmd2.Parameters.AddWithValue("@id", textBoxId.Text);
            con.Open();
            cmd2.ExecuteNonQuery();
            con.Close();
            RefreshMemberList();
            MessageBox.Show(a + " months succesfully added to the member");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            addMonth(1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            addMonth(6);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            addMonth(3);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            addMonth(12);
        }

        private void btnAddMember_Click(object sender, EventArgs e)
        {
            MemberAdd memberAddForm = new MemberAdd();
            memberAddForm.ShowDialog();

            RefreshMemberList();
        }
        private void ageChartBtn_Click(object sender, EventArgs e)
        {
            try
            {
                
                if (pictureBox1.Visible == true)
                {
                    pictureBox1.BringToFront();
                    pictureBox1.Visible = false;
                }
                else if (pictureBox1.Visible == false && groupBox3.Visible == false)
                {
                    pictureBox1.SendToBack();
                    pictureBox1.Visible = true;
                }

                groupBox2.Visible = true;
                OleDbDataAdapter adp = new OleDbDataAdapter("select yaş from members ", con);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    float count = 0;
                    int sifirYirmi = 0;
                    int yirmiKirk = 0;
                    int KirkAltmis = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        int yaş = Convert.ToInt32(dr["yaş"]);
                        if (yaş >= 0 && yaş < 20)
                            sifirYirmi++;
                        if (yaş >= 20 && yaş < 40)
                            yirmiKirk++;
                        if (yaş >= 40 && yaş <= 60)
                            KirkAltmis++;
                        count++;
                    }
                    float sy, yk, ka;
                    sy = (sifirYirmi / count) * 360;
                    yk = (yirmiKirk / count) * 360;
                    ka = (KirkAltmis / count) * 360;
                    Pen p = new Pen(Color.MidnightBlue);
                    p.Width = 3;
                    Rectangle rec = new Rectangle(ageChartBtn.Location.X + ageChartBtn.Size.Width + 170, 50, 225, 225);
                    Brush b1 = new SolidBrush(Color.White);
                    Brush b2 = new SolidBrush(Color.LawnGreen);
                    Brush b3 = new SolidBrush(Color.Orchid);
                    Graphics g = this.CreateGraphics();
                    g.DrawPie(p, rec, 0, sy);
                    g.FillPie(b1, rec, 0, sy);
                    g.DrawPie(p, rec, sy, yk);
                    g.FillPie(b2, rec, sy, yk);
                    g.DrawPie(p, rec, sy + yk, ka);
                    g.FillPie(b3, rec, sy + yk, ka);
                    adp.Dispose();
            }catch(Exception ex)
            {
                MessageBox.Show("Please make sure all the ages are valid numbers");
            }
        }
        private void genderChart_Click(object sender, EventArgs e)
        {
            try
            {
                if (pictureBox1.Visible == true)
                {
                    pictureBox1.BringToFront();
                    pictureBox1.Visible = false;
                }
                else if(pictureBox1.Visible == false && groupBox2.Visible == false)
                {
                    pictureBox1.SendToBack();
                    pictureBox1.Visible = true;
                }
                
                    groupBox3.Visible = true;
                    OleDbDataAdapter adp = new OleDbDataAdapter("select cinsiyet from members ", con);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    float count = 0;
                    float man = 0;
                    float woman = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        string gender = dr["cinsiyet"].ToString();
                        if (gender == "Man")
                            man++;
                        else
                            woman++;
                        count++;
                    }
                    float manDeg = (man / count) * 360;
                    float womanDeg = (woman / count) * 360;
                    Pen p = new Pen(Color.MidnightBlue);
                    p.Width = 3;
                    Rectangle rec = new Rectangle(ageChartBtn.Location.X + ageChartBtn.Size.Width + 425, 50, 225, 225);
                    Brush b1 = new SolidBrush(Color.Aqua);
                    Brush b2 = new SolidBrush(Color.OrangeRed);
                    Graphics g = this.CreateGraphics();
                    g.DrawPie(p, rec, 0, manDeg);
                    g.FillPie(b1, rec, 0, manDeg);
                    g.DrawPie(p, rec, manDeg, womanDeg);
                    g.FillPie(b2, rec, manDeg, womanDeg);
                    adp.Dispose();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                searchTxt.Focus();
        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            con.Open();
            OleDbCommand com = new OleDbCommand("select * from members where isim = @search or soyisim = @search or mail = @search or yaş = @search or cinsiyet = @search", con);
            com.Parameters.AddWithValue("@search",searchTxt.Text);
            OleDbDataAdapter adapter = new OleDbDataAdapter(com);
            DataTable search = new DataTable();
            adapter.Fill(search);
            if (search.Rows.Count > 0)
                dataGridView1.DataSource = search;
            else
                MessageBox.Show("The searched entry was not found", "Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            con.Close();
        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            searchTxt.Text = string.Empty;
            RefreshMemberList();
        }
    }
}
