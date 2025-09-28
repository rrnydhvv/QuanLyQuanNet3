using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace QuanLyQuanNetWinForms
{
    public partial class NguoiDungForm : MaterialForm
    {
        private int currentUserId;
        private DataGridView? dgvNguoiDung;
        private MaterialButton? btnAdd, btnEdit, btnDelete, btnRefresh;

        public NguoiDungForm(int userId)
        {
            currentUserId = userId;
            InitializeComponent();
            SetupMaterialTheme();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản Lý Người Dùng";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterParent;

            // DataGridView
            dgvNguoiDung = new DataGridView();
            dgvNguoiDung.Location = new Point(10, 10);
            dgvNguoiDung.Size = new Size(1160, 550);
            dgvNguoiDung.ReadOnly = true;
            dgvNguoiDung.AllowUserToAddRows = false;
            dgvNguoiDung.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvNguoiDung.BackgroundColor = Color.White;
            dgvNguoiDung.BorderStyle = BorderStyle.None;
            dgvNguoiDung.GridColor = Color.FromArgb(240, 240, 240);

            // Material Buttons
            btnAdd = new MaterialButton();
            btnAdd.Text = "Thêm Người Dùng";
            btnAdd.Location = new Point(10, 580);
            btnAdd.Size = new Size(140, 40);
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new MaterialButton();
            btnEdit.Text = "Sửa";
            btnEdit.Location = new Point(160, 580);
            btnEdit.Size = new Size(80, 40);
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new MaterialButton();
            btnDelete.Text = "Xóa";
            btnDelete.Location = new Point(250, 580);
            btnDelete.Size = new Size(80, 40);
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = new MaterialButton();
            btnRefresh.Text = "Làm Mới";
            btnRefresh.Location = new Point(340, 580);
            btnRefresh.Size = new Size(100, 40);
            btnRefresh.Click += (s, e) => LoadData();

            // Add controls
            this.Controls.Add(dgvNguoiDung);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
            this.Controls.Add(btnRefresh);
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
                ShowError("Lỗi tải dữ liệu người dùng", ex);
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            try
            {
                NguoiDungDialog dialog = new NguoiDungDialog(false);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                    ShowSuccess("Thêm người dùng thành công!");
                }
            }
            catch (Exception ex)
            {
                ShowError("Lỗi khi thêm người dùng", ex);
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvNguoiDung?.SelectedRows.Count > 0)
            {
                try
                {
                    var value = dgvNguoiDung.SelectedRows[0].Cells["TenDangNhap"].Value;
                    if (value != null)
                    {
                        string tenDangNhap = value.ToString()!;
                        NguoiDungDialog dialog = new NguoiDungDialog(true, tenDangNhap);
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            LoadData();
                            ShowSuccess("Cập nhật người dùng thành công!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowError("Lỗi khi sửa người dùng", ex);
                }
            }
            else
            {
                ShowWarning("Vui lòng chọn người dùng để sửa!");
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvNguoiDung?.SelectedRows.Count > 0)
            {
                try
                {
                    var value = dgvNguoiDung.SelectedRows[0].Cells["TenDangNhap"].Value;
                    var tenValue = dgvNguoiDung.SelectedRows[0].Cells["HoTen"].Value;
                    if (value != null && tenValue != null)
                    {
                        string tenDangNhap = value.ToString()!;
                        string hoTen = tenValue.ToString()!;

                        if (ShowConfirm($"Bạn có chắc muốn xóa người dùng '{hoTen}'?"))
                        {
                            DatabaseHelper.ExecuteStoredProcedure("sp_DeleteNguoiDung",
                                new SqlParameter[] { new SqlParameter("@TenDangNhap", tenDangNhap) });

                            LoadData();
                            ShowSuccess("Xóa người dùng thành công!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowError("Lỗi khi xóa người dùng", ex);
                }
            }
            else
            {
                ShowWarning("Vui lòng chọn người dùng để xóa!");
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

        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private bool ShowConfirm(string message)
        {
            return MessageBox.Show(message, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }
}