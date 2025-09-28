using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyQuanNetWinForms
{
    public partial class ThietBiForm : Form
    {
        private int currentUserId;
        private DataGridView? dgvThietBi;
        private Button? btnAdd, btnEdit, btnDelete, btnRefresh;

        public ThietBiForm(int userId)
        {
            currentUserId = userId;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản Lý Thiết Bị";
            this.Size = new Size(1200, 700);
            this.BackColor = Color.White;

            // DataGridView
            dgvThietBi = new DataGridView();
            dgvThietBi.Location = new Point(10, 10);
            dgvThietBi.Size = new Size(1160, 550);
            dgvThietBi.ReadOnly = true;
            dgvThietBi.AllowUserToAddRows = false;
            dgvThietBi.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvThietBi.BackgroundColor = Color.White;
            dgvThietBi.BorderStyle = BorderStyle.None;
            dgvThietBi.GridColor = Color.FromArgb(240, 240, 240);

            // Buttons
            btnAdd = new Button();
            btnAdd.Text = "Thêm Thiết Bị";
            btnAdd.Location = new Point(10, 580);
            btnAdd.Size = new Size(120, 40);
            btnAdd.BackColor = Color.FromArgb(76, 175, 80);
            btnAdd.ForeColor = Color.White;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button();
            btnEdit.Text = "Sửa";
            btnEdit.Location = new Point(140, 580);
            btnEdit.Size = new Size(80, 40);
            btnEdit.BackColor = Color.FromArgb(255, 193, 7);
            btnEdit.ForeColor = Color.White;
            btnEdit.FlatStyle = FlatStyle.Flat;
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button();
            btnDelete.Text = "Xóa";
            btnDelete.Location = new Point(230, 580);
            btnDelete.Size = new Size(80, 40);
            btnDelete.BackColor = Color.FromArgb(244, 67, 54);
            btnDelete.ForeColor = Color.White;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = new Button();
            btnRefresh.Text = "Làm Mới";
            btnRefresh.Location = new Point(320, 580);
            btnRefresh.Size = new Size(100, 40);
            btnRefresh.BackColor = Color.FromArgb(33, 150, 243);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Click += (s, e) => LoadData();

            // Add controls
            this.Controls.Add(dgvThietBi);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnRefresh);
        }

        private void LoadData()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetThietBi", Array.Empty<SqlParameter>());

                if (dt != null && dgvThietBi != null)
                {
                    dgvThietBi.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu thiết bị: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            ThietBiDialog dialog = new ThietBiDialog(false);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvThietBi?.SelectedRows.Count > 0)
            {
                var value = dgvThietBi.SelectedRows[0].Cells["MaThietBi"].Value;
                if (value != null)
                {
                    string maThietBi = value.ToString()!;
                    ThietBiDialog dialog = new ThietBiDialog(true, maThietBi);
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn thiết bị để sửa!");
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvThietBi?.SelectedRows.Count > 0)
            {
                var value = dgvThietBi.SelectedRows[0].Cells["MaThietBi"].Value;
                var tenValue = dgvThietBi.SelectedRows[0].Cells["TenThietBi"].Value;
                if (value != null && tenValue != null)
                {
                    string maThietBi = value.ToString()!;
                    string tenThietBi = tenValue.ToString()!;

                    DialogResult result = MessageBox.Show($"Bạn có chắc muốn xóa thiết bị '{tenThietBi}'?",
                        "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            DatabaseHelper.ExecuteStoredProcedure("sp_DeleteThietBi",
                                new SqlParameter[] { new SqlParameter("@MaThietBi", maThietBi) });

                            MessageBox.Show("Xóa thiết bị thành công!");
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
                MessageBox.Show("Vui lòng chọn thiết bị để xóa!");
            }
        }
    }
}