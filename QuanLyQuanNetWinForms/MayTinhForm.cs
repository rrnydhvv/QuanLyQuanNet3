using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace QuanLyQuanNetWinForms
{
    public partial class MayTinhForm : MaterialForm
    {
        private DataGridView? dgvMayTinh;
        private MaterialButton? btnAdd, btnEdit, btnDelete, btnRefresh;

        public MayTinhForm()
        {
            InitializeComponent();
            SetupMaterialTheme();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Quản Lý Máy Tính";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterParent;

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

            // Material Buttons
            btnAdd = new MaterialButton();
            btnAdd.Text = "Thêm Máy Tính";
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
            this.Controls.Add(dgvMayTinh);
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
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetMayTinh", Array.Empty<SqlParameter>());

                if (dt != null && dgvMayTinh != null)
                {
                    dgvMayTinh.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                ShowError("Lỗi tải dữ liệu máy tính", ex);
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            try
            {
                MayTinhDialog dialog = new MayTinhDialog(false);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                    ShowSuccess("Thêm máy tính thành công!");
                }
            }
            catch (Exception ex)
            {
                ShowError("Lỗi khi thêm máy tính", ex);
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvMayTinh?.SelectedRows.Count > 0)
            {
                try
                {
                    var value = dgvMayTinh.SelectedRows[0].Cells["MaMay"].Value;
                    if (value != null)
                    {
                        string maMay = value.ToString()!;
                        MayTinhDialog dialog = new MayTinhDialog(true, maMay);
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            LoadData();
                            ShowSuccess("Cập nhật máy tính thành công!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowError("Lỗi khi sửa máy tính", ex);
                }
            }
            else
            {
                ShowWarning("Vui lòng chọn máy tính để sửa!");
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvMayTinh?.SelectedRows.Count > 0)
            {
                try
                {
                    var value = dgvMayTinh.SelectedRows[0].Cells["MaMay"].Value;
                    var tenValue = dgvMayTinh.SelectedRows[0].Cells["TenMay"].Value;
                    if (value != null && tenValue != null)
                    {
                        string maMay = value.ToString()!;
                        string tenMay = tenValue.ToString()!;

                        if (ShowConfirm($"Bạn có chắc muốn xóa máy tính '{tenMay}'?"))
                        {
                            DatabaseHelper.ExecuteStoredProcedure("sp_DeleteMayTinh",
                                new SqlParameter[] { new SqlParameter("@MaMay", maMay) });

                            LoadData();
                            ShowSuccess("Xóa máy tính thành công!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowError("Lỗi khi xóa máy tính", ex);
                }
            }
            else
            {
                ShowWarning("Vui lòng chọn máy tính để xóa!");
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