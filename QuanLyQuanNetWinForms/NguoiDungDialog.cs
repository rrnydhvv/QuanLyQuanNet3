using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyQuanNetWinForms
{
    public partial class NguoiDungDialog : Form
    {
        private TextBox? txtTenDangNhap, txtMatKhau, txtHoTen, txtEmail;
        private ComboBox? cmbVaiTro;
        private Button? btnLuu, btnHuy;
        private bool isEditMode;
        private string? tenDangNhap;

        public NguoiDungDialog(bool editMode = false, string? tenDangNhap = null)
        {
            isEditMode = editMode;
            this.tenDangNhap = tenDangNhap;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = isEditMode ? "Chỉnh Sửa Người Dùng" : "Thêm Người Dùng";
            this.Size = new Size(400, 350);
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterParent;

            // Labels and TextBoxes
            Label lblTenDangNhap = new Label();
            lblTenDangNhap.Text = "Tên Đăng Nhập:";
            lblTenDangNhap.Location = new Point(20, 20);
            lblTenDangNhap.Size = new Size(120, 20);

            txtTenDangNhap = new TextBox();
            txtTenDangNhap.Location = new Point(150, 20);
            txtTenDangNhap.Size = new Size(200, 20);
            txtTenDangNhap.Enabled = !isEditMode;

            Label lblMatKhau = new Label();
            lblMatKhau.Text = "Mật Khẩu:";
            lblMatKhau.Location = new Point(20, 60);
            lblMatKhau.Size = new Size(120, 20);

            txtMatKhau = new TextBox();
            txtMatKhau.Location = new Point(150, 60);
            txtMatKhau.Size = new Size(200, 20);
            txtMatKhau.PasswordChar = '*';

            Label lblHoTen = new Label();
            lblHoTen.Text = "Họ Tên:";
            lblHoTen.Location = new Point(20, 100);
            lblHoTen.Size = new Size(120, 20);

            txtHoTen = new TextBox();
            txtHoTen.Location = new Point(150, 100);
            txtHoTen.Size = new Size(200, 20);

            Label lblEmail = new Label();
            lblEmail.Text = "Email:";
            lblEmail.Location = new Point(20, 140);
            lblEmail.Size = new Size(120, 20);

            txtEmail = new TextBox();
            txtEmail.Location = new Point(150, 140);
            txtEmail.Size = new Size(200, 20);

            Label lblVaiTro = new Label();
            lblVaiTro.Text = "Vai Trò:";
            lblVaiTro.Location = new Point(20, 180);
            lblVaiTro.Size = new Size(120, 20);

            cmbVaiTro = new ComboBox();
            cmbVaiTro.Location = new Point(150, 180);
            cmbVaiTro.Size = new Size(200, 20);
            cmbVaiTro.Items.AddRange(new string[] { "Admin", "User" });
            cmbVaiTro.SelectedIndex = 1;

            // Buttons
            btnLuu = new Button();
            btnLuu.Text = "Lưu";
            btnLuu.Location = new Point(150, 230);
            btnLuu.Size = new Size(80, 30);
            btnLuu.BackColor = Color.FromArgb(76, 175, 80);
            btnLuu.ForeColor = Color.White;
            btnLuu.FlatStyle = FlatStyle.Flat;
            btnLuu.Click += BtnLuu_Click;

            btnHuy = new Button();
            btnHuy.Text = "Hủy";
            btnHuy.Location = new Point(240, 230);
            btnHuy.Size = new Size(80, 30);
            btnHuy.BackColor = Color.FromArgb(244, 67, 54);
            btnHuy.ForeColor = Color.White;
            btnHuy.FlatStyle = FlatStyle.Flat;
            btnHuy.Click += (s, e) => this.Close();

            // Add controls
            this.Controls.Add(lblTenDangNhap);
            this.Controls.Add(txtTenDangNhap);
            this.Controls.Add(lblMatKhau);
            this.Controls.Add(txtMatKhau);
            this.Controls.Add(lblHoTen);
            this.Controls.Add(txtHoTen);
            this.Controls.Add(lblEmail);
            this.Controls.Add(txtEmail);
            this.Controls.Add(lblVaiTro);
            this.Controls.Add(cmbVaiTro);
            this.Controls.Add(btnLuu);
            this.Controls.Add(btnHuy);

            if (isEditMode && !string.IsNullOrEmpty(tenDangNhap))
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@TenDangNhap", tenDangNhap)
                };

                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetNguoiDungById", parameters);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtTenDangNhap!.Text = row["TenDangNhap"].ToString();
                    txtMatKhau!.Text = ""; // Không hiển thị mật khẩu cũ
                    txtHoTen!.Text = row["HoTen"].ToString();
                    txtEmail!.Text = row["Email"].ToString();
                    cmbVaiTro!.Text = row["VaiTro"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLuu_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenDangNhap!.Text) ||
                string.IsNullOrWhiteSpace(txtHoTen!.Text) ||
                string.IsNullOrWhiteSpace(txtEmail!.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!isEditMode && string.IsNullOrWhiteSpace(txtMatKhau!.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@TenDangNhap", txtTenDangNhap.Text),
                    new SqlParameter("@MatKhau", txtMatKhau!.Text),
                    new SqlParameter("@HoTen", txtHoTen.Text),
                    new SqlParameter("@Email", txtEmail.Text),
                    new SqlParameter("@VaiTro", cmbVaiTro!.Text)
                };

                string spName = isEditMode ? "sp_UpdateNguoiDung" : "sp_InsertNguoiDung";
                DatabaseHelper.ExecuteStoredProcedure(spName, parameters);

                MessageBox.Show(isEditMode ? "Cập nhật thành công!" : "Thêm mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}