using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyQuanNetWinForms
{
    public partial class ThietBiDialog : Form
    {
        private TextBox? txtMaThietBi, txtTenThietBi, txtMoTa, txtGiaTri;
        private ComboBox? cmbTrangThai;
        private Button? btnLuu, btnHuy;
        private bool isEditMode;
        private string? maThietBi;

        public ThietBiDialog(bool editMode = false, string? maThietBi = null)
        {
            isEditMode = editMode;
            this.maThietBi = maThietBi;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = isEditMode ? "Chỉnh Sửa Thiết Bị" : "Thêm Thiết Bị";
            this.Size = new Size(400, 350);
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterParent;

            // Labels and TextBoxes
            Label lblMaThietBi = new Label();
            lblMaThietBi.Text = "Mã Thiết Bị:";
            lblMaThietBi.Location = new Point(20, 20);
            lblMaThietBi.Size = new Size(100, 20);

            txtMaThietBi = new TextBox();
            txtMaThietBi.Location = new Point(130, 20);
            txtMaThietBi.Size = new Size(200, 20);
            txtMaThietBi.Enabled = !isEditMode;

            Label lblTenThietBi = new Label();
            lblTenThietBi.Text = "Tên Thiết Bị:";
            lblTenThietBi.Location = new Point(20, 60);
            lblTenThietBi.Size = new Size(100, 20);

            txtTenThietBi = new TextBox();
            txtTenThietBi.Location = new Point(130, 60);
            txtTenThietBi.Size = new Size(200, 20);

            Label lblTrangThai = new Label();
            lblTrangThai.Text = "Trạng Thái:";
            lblTrangThai.Location = new Point(20, 100);
            lblTrangThai.Size = new Size(100, 20);

            cmbTrangThai = new ComboBox();
            cmbTrangThai.Location = new Point(130, 100);
            cmbTrangThai.Size = new Size(200, 20);
            cmbTrangThai.Items.AddRange(new string[] { "Tốt", "Hỏng", "Bảo Trì" });
            cmbTrangThai.SelectedIndex = 0;

            Label lblMoTa = new Label();
            lblMoTa.Text = "Mô Tả:";
            lblMoTa.Location = new Point(20, 140);
            lblMoTa.Size = new Size(100, 20);

            txtMoTa = new TextBox();
            txtMoTa.Location = new Point(130, 140);
            txtMoTa.Size = new Size(200, 60);
            txtMoTa.Multiline = true;

            Label lblGiaTri = new Label();
            lblGiaTri.Text = "Giá Trị:";
            lblGiaTri.Location = new Point(20, 210);
            lblGiaTri.Size = new Size(100, 20);

            txtGiaTri = new TextBox();
            txtGiaTri.Location = new Point(130, 210);
            txtGiaTri.Size = new Size(200, 20);

            // Buttons
            btnLuu = new Button();
            btnLuu.Text = "Lưu";
            btnLuu.Location = new Point(130, 250);
            btnLuu.Size = new Size(80, 30);
            btnLuu.BackColor = Color.FromArgb(76, 175, 80);
            btnLuu.ForeColor = Color.White;
            btnLuu.FlatStyle = FlatStyle.Flat;
            btnLuu.Click += BtnLuu_Click;

            btnHuy = new Button();
            btnHuy.Text = "Hủy";
            btnHuy.Location = new Point(220, 250);
            btnHuy.Size = new Size(80, 30);
            btnHuy.BackColor = Color.FromArgb(244, 67, 54);
            btnHuy.ForeColor = Color.White;
            btnHuy.FlatStyle = FlatStyle.Flat;
            btnHuy.Click += (s, e) => this.Close();

            // Add controls
            this.Controls.Add(lblMaThietBi);
            this.Controls.Add(txtMaThietBi);
            this.Controls.Add(lblTenThietBi);
            this.Controls.Add(txtTenThietBi);
            this.Controls.Add(lblTrangThai);
            this.Controls.Add(cmbTrangThai);
            this.Controls.Add(lblMoTa);
            this.Controls.Add(txtMoTa);
            this.Controls.Add(lblGiaTri);
            this.Controls.Add(txtGiaTri);
            this.Controls.Add(btnLuu);
            this.Controls.Add(btnHuy);

            if (isEditMode && !string.IsNullOrEmpty(maThietBi))
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
                    new SqlParameter("@MaThietBi", maThietBi)
                };

                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetThietBiById", parameters);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtMaThietBi!.Text = row["MaThietBi"].ToString();
                    txtTenThietBi!.Text = row["TenThietBi"].ToString();
                    cmbTrangThai!.Text = row["TrangThai"].ToString();
                    txtMoTa!.Text = row["MoTa"].ToString();
                    txtGiaTri!.Text = row["GiaTri"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLuu_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaThietBi!.Text) ||
                string.IsNullOrWhiteSpace(txtTenThietBi!.Text) ||
                string.IsNullOrWhiteSpace(txtGiaTri!.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaThietBi", txtMaThietBi.Text),
                    new SqlParameter("@TenThietBi", txtTenThietBi.Text),
                    new SqlParameter("@TrangThai", cmbTrangThai!.Text),
                    new SqlParameter("@MoTa", txtMoTa!.Text),
                    new SqlParameter("@GiaTri", decimal.Parse(txtGiaTri.Text))
                };

                string spName = isEditMode ? "sp_UpdateThietBi" : "sp_InsertThietBi";
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