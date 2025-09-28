using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace QuanLyQuanNetWinForms
{
    public partial class BaoTriForm : MaterialForm
    {
        private int currentUserId;
        private DataGridView? dgvBaoTri;
        private MaterialButton? btnAdd, btnEdit, btnDelete, btnRefresh;

        public BaoTriForm(int userId)
        {
            currentUserId = userId;
            InitializeComponent();
            SetupMaterialTheme();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản Lý Bảo Trì";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterParent;

            // DataGridView
            dgvBaoTri = new DataGridView();
            dgvBaoTri.Location = new Point(10, 10);
            dgvBaoTri.Size = new Size(1160, 550);
            dgvBaoTri.ReadOnly = true;
            dgvBaoTri.AllowUserToAddRows = false;
            dgvBaoTri.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBaoTri.BackgroundColor = Color.White;
            dgvBaoTri.BorderStyle = BorderStyle.None;
            dgvBaoTri.GridColor = Color.FromArgb(240, 240, 240);

            // Material Buttons
            btnAdd = new MaterialButton();
            btnAdd.Text = "Thêm Bảo Trì";
            btnAdd.Location = new Point(10, 580);
            btnAdd.Size = new Size(120, 40);
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new MaterialButton();
            btnEdit.Text = "Sửa";
            btnEdit.Location = new Point(140, 580);
            btnEdit.Size = new Size(80, 40);
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new MaterialButton();
            btnDelete.Text = "Xóa";
            btnDelete.Location = new Point(230, 580);
            btnDelete.Size = new Size(80, 40);
            btnDelete.Click += BtnDelete_Click;

            btnRefresh = new MaterialButton();
            btnRefresh.Text = "Làm Mới";
            btnRefresh.Location = new Point(320, 580);
            btnRefresh.Size = new Size(100, 40);
            btnRefresh.Click += (s, e) => LoadData();

            // Add controls
            this.Controls.Add(dgvBaoTri);
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
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetBaoTri", Array.Empty<SqlParameter>());

                if (dt != null && dgvBaoTri != null)
                {
                    dgvBaoTri.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                ShowError("Lỗi tải dữ liệu bảo trì", ex);
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            try
            {
                BaoTriDialog dialog = new BaoTriDialog(false);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                    ShowSuccess("Thêm bảo trì thành công!");
                }
            }
            catch (Exception ex)
            {
                ShowError("Lỗi khi thêm bảo trì", ex);
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvBaoTri?.SelectedRows.Count > 0)
            {
                try
                {
                    var value = dgvBaoTri.SelectedRows[0].Cells["MaBaoTri"].Value;
                    if (value != null)
                    {
                        string maBaoTri = value.ToString()!;
                        BaoTriDialog dialog = new BaoTriDialog(true, maBaoTri);
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            LoadData();
                            ShowSuccess("Cập nhật bảo trì thành công!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowError("Lỗi khi sửa bảo trì", ex);
                }
            }
            else
            {
                ShowWarning("Vui lòng chọn bảo trì để sửa!");
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvBaoTri?.SelectedRows.Count > 0)
            {
                try
                {
                    var value = dgvBaoTri.SelectedRows[0].Cells["MaBaoTri"].Value;
                    var tenValue = dgvBaoTri.SelectedRows[0].Cells["MaMay"].Value;
                    if (value != null && tenValue != null)
                    {
                        string maBaoTri = value.ToString()!;
                        string maMay = tenValue.ToString()!;

                        if (ShowConfirm($"Bạn có chắc muốn xóa bảo trì '{maBaoTri}'?"))
                        {
                            DatabaseHelper.ExecuteStoredProcedure("sp_DeleteBaoTri",
                                new SqlParameter[] { new SqlParameter("@MaBaoTri", maBaoTri) });

                            LoadData();
                            ShowSuccess("Xóa bảo trì thành công!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowError("Lỗi khi xóa bảo trì", ex);
                }
            }
            else
            {
                ShowWarning("Vui lòng chọn bảo trì để xóa!");
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