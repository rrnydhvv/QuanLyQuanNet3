using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyQuanNetWinForms
{
    public partial class MayTinhDialog : Form
    {
        private TextBox? txtMaMay, txtTenMay, txtTrangThai, txtViTri;
        private Button? btnLuu, btnHuy;
        private bool isEditMode;
        private string? maMay;

        public MayTinhDialog(bool editMode = false, string? maMay = null)
        {
            isEditMode = editMode;
            this.maMay = maMay;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = isEditMode ? "Chỉnh Sửa Máy Tính" : "Thêm Máy Tính";
            this.Size = new Size(400, 300);
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterParent;

            // Labels and TextBoxes
            Label lblMaMay = new Label();
            lblMaMay.Text = "Mã Máy:";
            lblMaMay.Location = new Point(20, 20);
            lblMaMay.Size = new Size(100, 20);

            txtMaMay = new TextBox();
            txtMaMay.Location = new Point(130, 20);
            txtMaMay.Size = new Size(200, 20);
            txtMaMay.Enabled = !isEditMode;

            Label lblTenMay = new Label();
            lblTenMay.Text = "Tên Máy:";
            lblTenMay.Location = new Point(20, 60);
            lblTenMay.Size = new Size(100, 20);

            txtTenMay = new TextBox();
            txtTenMay.Location = new Point(130, 60);
            txtTenMay.Size = new Size(200, 20);

            Label lblTrangThai = new Label();
            lblTrangThai.Text = "Trạng Thái:";
            lblTrangThai.Location = new Point(20, 100);
            lblTrangThai.Size = new Size(100, 20);

            txtTrangThai = new TextBox();
            txtTrangThai.Location = new Point(130, 100);
            txtTrangThai.Size = new Size(200, 20);

            Label lblViTri = new Label();
            lblViTri.Text = "Vị Trí:";
            lblViTri.Location = new Point(20, 140);
            lblViTri.Size = new Size(100, 20);

            txtViTri = new TextBox();
            txtViTri.Location = new Point(130, 140);
            txtViTri.Size = new Size(200, 20);

            // Buttons
            btnLuu = new Button();
            btnLuu.Text = "Lưu";
            btnLuu.Location = new Point(130, 200);
            btnLuu.Size = new Size(80, 30);
            btnLuu.BackColor = Color.FromArgb(76, 175, 80);
            btnLuu.ForeColor = Color.White;
            btnLuu.FlatStyle = FlatStyle.Flat;
            btnLuu.Click += BtnLuu_Click;

            btnHuy = new Button();
            btnHuy.Text = "Hủy";
            btnHuy.Location = new Point(220, 200);
            btnHuy.Size = new Size(80, 30);
            btnHuy.BackColor = Color.FromArgb(244, 67, 54);
            btnHuy.ForeColor = Color.White;
            btnHuy.FlatStyle = FlatStyle.Flat;
            btnHuy.Click += (s, e) => this.Close();

            // Add controls
            this.Controls.Add(lblMaMay);
            this.Controls.Add(txtMaMay);
            this.Controls.Add(lblTenMay);
            this.Controls.Add(txtTenMay);
            this.Controls.Add(lblTrangThai);
            this.Controls.Add(txtTrangThai);
            this.Controls.Add(lblViTri);
            this.Controls.Add(txtViTri);
            this.Controls.Add(btnLuu);
            this.Controls.Add(btnHuy);

            if (isEditMode && !string.IsNullOrEmpty(maMay))
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
                    new SqlParameter("@MaMay", maMay)
                };

                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetMayTinhById", parameters);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtMaMay!.Text = row["MaMay"].ToString();
                    txtTenMay!.Text = row["TenMay"].ToString();
                    txtTrangThai!.Text = row["TrangThai"].ToString();
                    txtViTri!.Text = row["ViTri"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLuu_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaMay!.Text) ||
                string.IsNullOrWhiteSpace(txtTenMay!.Text) ||
                string.IsNullOrWhiteSpace(txtTrangThai!.Text) ||
                string.IsNullOrWhiteSpace(txtViTri!.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaMay", txtMaMay.Text),
                    new SqlParameter("@TenMay", txtTenMay.Text),
                    new SqlParameter("@TrangThai", txtTrangThai.Text),
                    new SqlParameter("@ViTri", txtViTri.Text)
                };

                string spName = isEditMode ? "sp_UpdateMayTinh" : "sp_InsertMayTinh";
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