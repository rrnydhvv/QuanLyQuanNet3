using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyQuanNetWinForms
{
    public partial class MayTinhForm : Form
    {
        private int currentUserId;
        private DataGridView? dgvMayTinh;
        private Button? btnAdd, btnEdit, btnDelete, btnRefresh;

        public MayTinhForm(int userId)
        {
            currentUserId = userId;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản Lý Máy Tính";
            this.Size = new Size(1200, 700);
            this.BackColor = Color.White;

            // DataGridView
            dgvMayTinh = new DataGridView();
            dgvMayTinh.Location = new Point(10, 10);
            dgvMayTinh.Size = new Size(1160, 550);
            dgvMayTinh.ReadOnly = true;
            dgvMayTinh.AllowUserToAddRows = false;
            dgvMayTinh.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMayTinh.BackgroundColor = Color.White;
            dgvMayTinh.BorderStyle = BorderStyle.None;
            dgvMayTinh.GridColor = Color.FromArgb(240, 240, 240);

            // Buttons
            btnAdd = new Button();
            btnAdd.Text = "Thêm Máy Tính";
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
            this.Controls.Add(dgvMayTinh);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnRefresh);
        }

        private void LoadData()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetMayTinh", Array.Empty<SqlParameter>());

                if (dt != null && dgvMayTinh != null)
                {
                    dgvMayTinh.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu máy tính: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            MayTinhDialog dialog = new MayTinhDialog(false);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvMayTinh?.SelectedRows.Count > 0)
            {
                var value = dgvMayTinh.SelectedRows[0].Cells["MaMay"].Value;
                if (value != null)
                {
                    string maMay = value.ToString()!;
                    MayTinhDialog dialog = new MayTinhDialog(true, maMay);
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn máy tính để sửa!");
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvMayTinh?.SelectedRows.Count > 0)
            {
                var value = dgvMayTinh.SelectedRows[0].Cells["MaMay"].Value;
                var tenValue = dgvMayTinh.SelectedRows[0].Cells["TenMay"].Value;
                if (value != null && tenValue != null)
                {
                    string maMay = value.ToString()!;
                    string tenMay = tenValue.ToString()!;

                    DialogResult result = MessageBox.Show($"Bạn có chắc muốn xóa máy tính '{tenMay}'?",
                        "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            DatabaseHelper.ExecuteStoredProcedure("sp_DeleteMayTinh",
                                new SqlParameter[] { new SqlParameter("@MaMay", maMay) });

                            MessageBox.Show("Xóa máy tính thành công!");
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
                MessageBox.Show("Vui lòng chọn máy tính để xóa!");
            }
        }
    }
}