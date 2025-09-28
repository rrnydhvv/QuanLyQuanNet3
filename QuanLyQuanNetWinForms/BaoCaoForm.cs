using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace QuanLyQuanNetWinForms
{
    public partial class BaoCaoForm : MaterialForm
    {
        private int currentUserId;
        private DataGridView? dgvBaoCao;
        private MaterialButton? btnThongKeMayTinh, btnBaoCaoChiPhi;

        public BaoCaoForm(int userId)
        {
            currentUserId = userId;
            InitializeComponent();
            SetupMaterialTheme();
        }

        private void InitializeComponent()
        {
            this.Text = "Báo Cáo và Thống Kê";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterParent;

            // Material Buttons
            btnThongKeMayTinh = new MaterialButton();
            btnThongKeMayTinh.Text = "Thống Kê Máy Tính";
            btnThongKeMayTinh.Location = new Point(10, 10);
            btnThongKeMayTinh.Size = new Size(150, 40);
            btnThongKeMayTinh.Click += BtnThongKeMayTinh_Click;

            btnBaoCaoChiPhi = new MaterialButton();
            btnBaoCaoChiPhi.Text = "Báo Cáo Chi Phí Bảo Trì";
            btnBaoCaoChiPhi.Location = new Point(170, 10);
            btnBaoCaoChiPhi.Size = new Size(180, 40);
            btnBaoCaoChiPhi.Click += BtnBaoCaoChiPhi_Click;

            // DataGridView
            dgvBaoCao = new DataGridView();
            dgvBaoCao.Location = new Point(10, 60);
            dgvBaoCao.Size = new Size(1160, 600);
            dgvBaoCao.ReadOnly = true;
            dgvBaoCao.AllowUserToAddRows = false;
            dgvBaoCao.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBaoCao.BackgroundColor = Color.White;
            dgvBaoCao.BorderStyle = BorderStyle.None;
            dgvBaoCao.GridColor = Color.FromArgb(240, 240, 240);

            // Add controls
            this.Controls.Add(btnThongKeMayTinh);
            this.Controls.Add(btnBaoCaoChiPhi);
            this.Controls.Add(dgvBaoCao);
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

        private void BtnThongKeMayTinh_Click(object? sender, EventArgs e)
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetMayTinhStatistics",
                    new SqlParameter[] { new SqlParameter("@UserID", currentUserId) });

                if (dt != null && dgvBaoCao != null)
                {
                    dgvBaoCao.DataSource = dt;
                    ShowSuccess("Tải thống kê máy tính thành công!");
                }
            }
            catch (Exception ex)
            {
                ShowError("Lỗi tải thống kê máy tính", ex);
            }
        }

        private void BtnBaoCaoChiPhi_Click(object? sender, EventArgs e)
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetBaoTriCostReport",
                    new SqlParameter[] { new SqlParameter("@UserID", currentUserId) });

                if (dt != null && dgvBaoCao != null)
                {
                    dgvBaoCao.DataSource = dt;
                    ShowSuccess("Tải báo cáo chi phí bảo trì thành công!");
                }
            }
            catch (Exception ex)
            {
                ShowError("Lỗi tải báo cáo chi phí bảo trì", ex);
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
    }
}