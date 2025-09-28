using System;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace QuanLyQuanNetWinForms
{
    public partial class CaiDatForm : MaterialForm
    {
        private MaterialButton? btnDangXuat, btnThoat;

        public CaiDatForm()
        {
            InitializeComponent();
            SetupMaterialTheme();
        }

        private void InitializeComponent()
        {
            this.Text = "Cài Đặt";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;

            // Material Buttons
            btnDangXuat = new MaterialButton();
            btnDangXuat.Text = "Đăng Xuất";
            btnDangXuat.Location = new Point(100, 50);
            btnDangXuat.Size = new Size(200, 50);
            btnDangXuat.Click += BtnDangXuat_Click;

            btnThoat = new MaterialButton();
            btnThoat.Text = "Thoát Ứng Dụng";
            btnThoat.Location = new Point(100, 120);
            btnThoat.Size = new Size(200, 50);
            btnThoat.Click += (s, e) => Application.Exit();

            // Add controls
            this.Controls.Add(btnDangXuat);
            this.Controls.Add(btnThoat);
        }

        private void SetupMaterialTheme()
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Teal600, Primary.Teal700,
                Primary.Teal100, Accent.Orange200, TextShade.WHITE);
        }

        private void BtnDangXuat_Click(object? sender, EventArgs e)
        {
            try
            {
                if (ShowConfirm("Bạn có chắc chắn muốn đăng xuất?"))
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
                    ShowSuccess("Đăng xuất thành công!");
                }
            }
            catch (Exception ex)
            {
                ShowError("Lỗi khi đăng xuất", ex);
            }
        }

        private void ShowError(string message, Exception ex)
        {
            MessageBox.Show($"{message}: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowSuccess(string message)
        {
            MessageBox.Show(message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool ShowConfirm(string message)
        {
            return MessageBox.Show(message, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }
}