using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyQuanNetWinForms
{
    public partial class CaiDatForm : Form
    {
        private Button? btnDangXuat, btnThoat;

        public CaiDatForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Cài Đặt";
            this.Size = new Size(400, 300);
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterParent;

            // Buttons
            btnDangXuat = new Button();
            btnDangXuat.Text = "Đăng Xuất";
            btnDangXuat.Location = new Point(100, 50);
            btnDangXuat.Size = new Size(200, 50);
            btnDangXuat.BackColor = Color.FromArgb(244, 67, 54);
            btnDangXuat.ForeColor = Color.White;
            btnDangXuat.FlatStyle = FlatStyle.Flat;
            btnDangXuat.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnDangXuat.Click += BtnDangXuat_Click;

            btnThoat = new Button();
            btnThoat.Text = "Thoát Ứng Dụng";
            btnThoat.Location = new Point(100, 120);
            btnThoat.Size = new Size(200, 50);
            btnThoat.BackColor = Color.FromArgb(33, 33, 33);
            btnThoat.ForeColor = Color.White;
            btnThoat.FlatStyle = FlatStyle.Flat;
            btnThoat.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnThoat.Click += (s, e) => Application.Exit();

            // Add controls
            this.Controls.Add(btnDangXuat);
            this.Controls.Add(btnThoat);
        }

        private void BtnDangXuat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?",
                "Đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Đóng tất cả form và quay về login
                foreach (Form form in Application.OpenForms)
                {
                    if (!(form is LoginForm))
                    {
                        form.Close();
                    }
                }

                LoginForm loginForm = new LoginForm();
                loginForm.Show();
            }
        }
    }
}