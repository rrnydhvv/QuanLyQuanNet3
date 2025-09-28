using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuanLyQuanNetWinForms
{
    public partial class LoginForm : Form
    {
        private string connectionString = "Server=.;Database=QuanLyQuanNet;Trusted_Connection=True;";

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Đăng Nhập - Quản Lý Quán Net";
            this.Size = new System.Drawing.Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Label Username
            Label lblUsername = new Label();
            lblUsername.Text = "Tên đăng nhập:";
            lblUsername.Location = new System.Drawing.Point(50, 50);
            lblUsername.Size = new System.Drawing.Size(100, 20);

            // TextBox Username
            TextBox txtUsername = new TextBox();
            txtUsername.Name = "txtUsername";
            txtUsername.Location = new System.Drawing.Point(160, 50);
            txtUsername.Size = new System.Drawing.Size(180, 20);

            // Label Password
            Label lblPassword = new Label();
            lblPassword.Text = "Mật khẩu:";
            lblPassword.Location = new System.Drawing.Point(50, 100);
            lblPassword.Size = new System.Drawing.Size(100, 20);

            // TextBox Password
            TextBox txtPassword = new TextBox();
            txtPassword.Name = "txtPassword";
            txtPassword.Location = new System.Drawing.Point(160, 100);
            txtPassword.Size = new System.Drawing.Size(180, 20);
            txtPassword.PasswordChar = '*';

            // Button Login
            Button btnLogin = new Button();
            btnLogin.Text = "Đăng Nhập";
            btnLogin.Location = new System.Drawing.Point(100, 150);
            btnLogin.Size = new System.Drawing.Size(80, 30);
            btnLogin.Click += BtnLogin_Click;

            // Button Exit
            Button btnExit = new Button();
            btnExit.Text = "Thoát";
            btnExit.Location = new System.Drawing.Point(200, 150);
            btnExit.Size = new System.Drawing.Size(80, 30);
            btnExit.Click += (s, e) => Application.Exit();

            // Add controls
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnExit);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = ((TextBox)this.Controls["txtUsername"]).Text;
            string password = ((TextBox)this.Controls["txtPassword"]).Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_LoginNguoiDung", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int userId = (int)reader["UserID"];
                                string fullName = reader["FullName"].ToString();
                                int roleId = (int)reader["RoleID"];

                                // Mở MainForm
                                MainForm mainForm = new MainForm(userId, fullName, roleId);
                                this.Hide();
                                mainForm.ShowDialog();
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối database: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}