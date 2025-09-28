using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyQuanNetWinForms
{
    public partial class BaoTriDialog : Form
    {
        private TextBox? txtMaBaoTri, txtMaMay, txtMoTa, txtChiPhi;
        private ComboBox? cmbTrangThai;
        private DateTimePicker? dtpNgayBaoTri;
        private Button? btnLuu, btnHuy;
        private bool isEditMode;
        private string? maBaoTri;

        public BaoTriDialog(bool editMode = false, string? maBaoTri = null)
        {
            isEditMode = editMode;
            this.maBaoTri = maBaoTri;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = isEditMode ? "Chỉnh Sửa Bảo Trì" : "Thêm Bảo Trì";
            this.Size = new Size(400, 400);
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterParent;

            // Labels and TextBoxes
            Label lblMaBaoTri = new Label();
            lblMaBaoTri.Text = "Mã Bảo Trì:";
            lblMaBaoTri.Location = new Point(20, 20);
            lblMaBaoTri.Size = new Size(100, 20);

            txtMaBaoTri = new TextBox();
            txtMaBaoTri.Location = new Point(130, 20);
            txtMaBaoTri.Size = new Size(200, 20);
            txtMaBaoTri.Enabled = !isEditMode;

            Label lblMaMay = new Label();
            lblMaMay.Text = "Mã Máy:";
            lblMaMay.Location = new Point(20, 60);
            lblMaMay.Size = new Size(100, 20);

            txtMaMay = new TextBox();
            txtMaMay.Location = new Point(130, 60);
            txtMaMay.Size = new Size(200, 20);

            Label lblNgayBaoTri = new Label();
            lblNgayBaoTri.Text = "Ngày Bảo Trì:";
            lblNgayBaoTri.Location = new Point(20, 100);
            lblNgayBaoTri.Size = new Size(100, 20);

            dtpNgayBaoTri = new DateTimePicker();
            dtpNgayBaoTri.Location = new Point(130, 100);
            dtpNgayBaoTri.Size = new Size(200, 20);
            dtpNgayBaoTri.Format = DateTimePickerFormat.Short;

            Label lblTrangThai = new Label();
            lblTrangThai.Text = "Trạng Thái:";
            lblTrangThai.Location = new Point(20, 140);
            lblTrangThai.Size = new Size(100, 20);

            cmbTrangThai = new ComboBox();
            cmbTrangThai.Location = new Point(130, 140);
            cmbTrangThai.Size = new Size(200, 20);
            cmbTrangThai.Items.AddRange(new string[] { "Đang Bảo Trì", "Hoàn Thành", "Hủy" });
            cmbTrangThai.SelectedIndex = 0;

            Label lblMoTa = new Label();
            lblMoTa.Text = "Mô Tả:";
            lblMoTa.Location = new Point(20, 180);
            lblMoTa.Size = new Size(100, 20);

            txtMoTa = new TextBox();
            txtMoTa.Location = new Point(130, 180);
            txtMoTa.Size = new Size(200, 60);
            txtMoTa.Multiline = true;

            Label lblChiPhi = new Label();
            lblChiPhi.Text = "Chi Phí:";
            lblChiPhi.Location = new Point(20, 250);
            lblChiPhi.Size = new Size(100, 20);

            txtChiPhi = new TextBox();
            txtChiPhi.Location = new Point(130, 250);
            txtChiPhi.Size = new Size(200, 20);

            // Buttons
            btnLuu = new Button();
            btnLuu.Text = "Lưu";
            btnLuu.Location = new Point(130, 290);
            btnLuu.Size = new Size(80, 30);
            btnLuu.BackColor = Color.FromArgb(76, 175, 80);
            btnLuu.ForeColor = Color.White;
            btnLuu.FlatStyle = FlatStyle.Flat;
            btnLuu.Click += BtnLuu_Click;

            btnHuy = new Button();
            btnHuy.Text = "Hủy";
            btnHuy.Location = new Point(220, 290);
            btnHuy.Size = new Size(80, 30);
            btnHuy.BackColor = Color.FromArgb(244, 67, 54);
            btnHuy.ForeColor = Color.White;
            btnHuy.FlatStyle = FlatStyle.Flat;
            btnHuy.Click += (s, e) => this.Close();

            // Add controls
            this.Controls.Add(lblMaBaoTri);
            this.Controls.Add(txtMaBaoTri);
            this.Controls.Add(lblMaMay);
            this.Controls.Add(txtMaMay);
            this.Controls.Add(lblNgayBaoTri);
            this.Controls.Add(dtpNgayBaoTri);
            this.Controls.Add(lblTrangThai);
            this.Controls.Add(cmbTrangThai);
            this.Controls.Add(lblMoTa);
            this.Controls.Add(txtMoTa);
            this.Controls.Add(lblChiPhi);
            this.Controls.Add(txtChiPhi);
            this.Controls.Add(btnLuu);
            this.Controls.Add(btnHuy);

            if (isEditMode && !string.IsNullOrEmpty(maBaoTri))
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
                    new SqlParameter("@MaBaoTri", maBaoTri)
                };

                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetBaoTriById", parameters);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtMaBaoTri!.Text = row["MaBaoTri"].ToString();
                    txtMaMay!.Text = row["MaMay"].ToString();
                    if (row["NgayBaoTri"] != DBNull.Value)
                        dtpNgayBaoTri!.Value = (DateTime)row["NgayBaoTri"];
                    cmbTrangThai!.Text = row["TrangThai"].ToString();
                    txtMoTa!.Text = row["MoTa"].ToString();
                    txtChiPhi!.Text = row["ChiPhi"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLuu_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaBaoTri!.Text) ||
                string.IsNullOrWhiteSpace(txtMaMay!.Text) ||
                string.IsNullOrWhiteSpace(txtChiPhi!.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@MaBaoTri", txtMaBaoTri.Text),
                    new SqlParameter("@MaMay", txtMaMay.Text),
                    new SqlParameter("@NgayBaoTri", dtpNgayBaoTri!.Value),
                    new SqlParameter("@TrangThai", cmbTrangThai!.Text),
                    new SqlParameter("@MoTa", txtMoTa!.Text),
                    new SqlParameter("@ChiPhi", decimal.Parse(txtChiPhi.Text))
                };

                string spName = isEditMode ? "sp_UpdateBaoTri" : "sp_InsertBaoTri";
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