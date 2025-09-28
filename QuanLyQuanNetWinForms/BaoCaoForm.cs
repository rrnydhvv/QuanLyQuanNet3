using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyQuanNetWinForms
{
    public partial class BaoCaoForm : Form
    {
        private int currentUserId;
        private DataGridView? dgvBaoCao;
        private Button? btnThongKeMayTinh, btnBaoCaoChiPhi;

        public BaoCaoForm(int userId)
        {
            currentUserId = userId;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Báo Cáo và Thống Kê";
            this.Size = new Size(1200, 700);
            this.BackColor = Color.White;

            // Buttons
            btnThongKeMayTinh = new Button();
            btnThongKeMayTinh.Text = "Thống Kê Máy Tính";
            btnThongKeMayTinh.Location = new Point(10, 10);
            btnThongKeMayTinh.Size = new Size(150, 40);
            btnThongKeMayTinh.BackColor = Color.FromArgb(33, 150, 243);
            btnThongKeMayTinh.ForeColor = Color.White;
            btnThongKeMayTinh.FlatStyle = FlatStyle.Flat;
            btnThongKeMayTinh.Click += BtnThongKeMayTinh_Click;

            btnBaoCaoChiPhi = new Button();
            btnBaoCaoChiPhi.Text = "Báo Cáo Chi Phí Bảo Trì";
            btnBaoCaoChiPhi.Location = new Point(170, 10);
            btnBaoCaoChiPhi.Size = new Size(180, 40);
            btnBaoCaoChiPhi.BackColor = Color.FromArgb(156, 39, 176);
            btnBaoCaoChiPhi.ForeColor = Color.White;
            btnBaoCaoChiPhi.FlatStyle = FlatStyle.Flat;
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

        private void BtnThongKeMayTinh_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetMayTinhStatistics",
                    new SqlParameter[] { new SqlParameter("@UserID", currentUserId) });

                if (dt != null && dgvBaoCao != null)
                {
                    dgvBaoCao.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải thống kê máy tính: " + ex.Message);
            }
        }

        private void BtnBaoCaoChiPhi_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteStoredProcedure("sp_GetBaoTriCostReport",
                    new SqlParameter[] { new SqlParameter("@UserID", currentUserId) });

                if (dt != null && dgvBaoCao != null)
                {
                    dgvBaoCao.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải báo cáo chi phí bảo trì: " + ex.Message);
            }
        }
    }
}