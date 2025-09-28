using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace QuanLyQuanNetWinForms
{
    public partial class ThietBiForm : MaterialForm
    {
        private int currentUserId;
        private DataGridView? dgvThietBi;
        private MaterialButton? btnAdd, btnEdit, btnDelete, btnRefresh;

        public ThietBiForm(int userId)
        {
            currentUserId = userId;
            InitializeComponent();
            SetupMaterialTheme();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản Lý Thiết Bị";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterParent;

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

            // Material Buttons
            btnAdd = new MaterialButton();
            btnAdd.Text = "Thêm Thiết Bị";
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
            this.Controls.Add(dgvThietBi);
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
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetThietBi", Array.Empty<SqlParameter>());

                if (dt != null && dgvThietBi != null)
                {
                    dgvThietBi.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                ShowError("Lỗi tải dữ liệu thiết bị", ex);
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            try
            {
                ThietBiDialog dialog = new ThietBiDialog(false);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                    ShowSuccess("Thêm thiết bị thành công!");
                }
            }
            catch (Exception ex)
            {
                ShowError("Lỗi khi thêm thiết bị", ex);
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvThietBi?.SelectedRows.Count > 0)
            {
                try
                {
                    var value = dgvThietBi.SelectedRows[0].Cells["MaThietBi"].Value;
                    if (value != null)
                    {
                        string maThietBi = value.ToString()!;
                        ThietBiDialog dialog = new ThietBiDialog(true, maThietBi);
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            LoadData();
                            ShowSuccess("Cập nhật thiết bị thành công!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowError("Lỗi khi sửa thiết bị", ex);
                }
            }
            else
            {
                ShowWarning("Vui lòng chọn thiết bị để sửa!");
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvThietBi?.SelectedRows.Count > 0)
            {
                try
                {
                    var value = dgvThietBi.SelectedRows[0].Cells["MaThietBi"].Value;
                    var tenValue = dgvThietBi.SelectedRows[0].Cells["TenThietBi"].Value;
                    if (value != null && tenValue != null)
                    {
                        string maThietBi = value.ToString()!;
                        string tenThietBi = tenValue.ToString()!;

                        if (ShowConfirm($"Bạn có chắc muốn xóa thiết bị '{tenThietBi}'?"))
                        {
                            DatabaseHelper.ExecuteStoredProcedure("sp_DeleteThietBi",
                                new SqlParameter[] { new SqlParameter("@MaThietBi", maThietBi) });

                            LoadData();
                            ShowSuccess("Xóa thiết bị thành công!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowError("Lỗi khi xóa thiết bị", ex);
                }
            }
            else
            {
                ShowWarning("Vui lòng chọn thiết bị để xóa!");
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