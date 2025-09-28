using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyQuanNetWinForms
{
    public partial class NguoiDungForm : Form
    {
        private int currentUserId;
        private DataGridView? dgvNguoiDung;
        private Button? btnAdd, btnEdit, btnDelete, btnRefresh;

        public NguoiDungForm(int userId)
        {
            currentUserId = userId;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản Lý Người Dùng";
            this.Size = new Size(1200, 700);
            this.BackColor = Color.White;

            // DataGridView
            dgvNguoiDung = new DataGridView();
            dgvNguoiDung.Location = new Point(10, 10);
            dgvNguoiDung.Size = new Size(1160, 550);
            dgvNguoiDung.ReadOnly = true;
            dgvNguoiDung.AllowUserToAddRows = false;
            dgvNguoiDung.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Buttons
            btnAdd = new Button();
            btnAdd.Text = "Thêm Người Dùng";
            btnAdd.Location = new Point(10, 580);
            btnAdd.Size = new Size(140, 40);
            btnAdd.BackColor = Color.FromArgb(76, 175, 80);
            btnAdd.ForeColor = Color.White;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button();
            btnEdit.Text = "Sửa";
            btnEdit.Location = new Point(160, 580);
            btnEdit.Size = new Size(80, 40);
            btnEdit.BackColor = Color.FromArgb(255, 193, 7);
            btnEdit.ForeColor = Color.White;
            btnEdit.FlatStyle = FlatStyle.Flat;
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button();
            btnDelete.Text = "Xóa";
            btnDelete.Location = new Point(250, 580);
            btnDelete.Size = new Size(80, 40);
            btnDelete.BackColor = Color.FromArgb(244, 67, 54);
            btnDelete.ForeColor = Color.White;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = new Button();
            btnRefresh.Text = "Làm Mới";
            btnRefresh.Location = new Point(340, 580);
            btnRefresh.Size = new Size(100, 40);
            btnRefresh.BackColor = Color.FromArgb(33, 150, 243);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Click += (s, e) => LoadData();

            // Add controls
            this.Controls.Add(dgvNguoiDung);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnRefresh);
        }

        private void LoadData()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetNguoiDung", Array.Empty<SqlParameter>());

                if (dt != null && dgvNguoiDung != null)
                {
                    dgvNguoiDung.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu người dùng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            NguoiDungDialog dialog = new NguoiDungDialog(false);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvNguoiDung?.SelectedRows.Count > 0)
            {
                var value = dgvNguoiDung.SelectedRows[0].Cells["TenDangNhap"].Value;
                if (value != null)
                {
                    string tenDangNhap = value.ToString()!;
                    NguoiDungDialog dialog = new NguoiDungDialog(true, tenDangNhap);
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn người dùng để sửa!");
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvNguoiDung?.SelectedRows.Count > 0)
            {
                var value = dgvNguoiDung.SelectedRows[0].Cells["TenDangNhap"].Value;
                var tenValue = dgvNguoiDung.SelectedRows[0].Cells["HoTen"].Value;
                if (value != null && tenValue != null)
                {
                    string tenDangNhap = value.ToString()!;
                    string hoTen = tenValue.ToString()!;

                    DialogResult result = MessageBox.Show($"Bạn có chắc muốn xóa người dùng '{hoTen}'?",
                        "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            DatabaseHelper.ExecuteStoredProcedure("sp_DeleteNguoiDung",
                                new SqlParameter[] { new SqlParameter("@TenDangNhap", tenDangNhap) });

                            MessageBox.Show("Xóa người dùng thành công!");
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn người dùng để xóa!");
            }
        }
    }
}