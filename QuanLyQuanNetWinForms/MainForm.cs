using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace QuanLyQuanNetWinForms
{
    public partial class MainForm : MaterialForm  // Thay Form bằng MaterialForm
    {
        private int currentUserId;
        private string currentUserName;
        private int currentRoleId;
        private string connectionString = "Server=.;Database=QuanLyQuanNet;Trusted_Connection=True;";

        private SplitContainer? splitContainer1;
        private Panel? panelSidebar;
        private Panel? panelMain;
        private MaterialCard? cardRanh, cardDangSuDung, cardBaoTri;  // Cards thống kê giống repo
        private MaterialLabel? lblRanh, lblDangSuDung, lblBaoTri;
        private Button? btnMayTinh, btnThietBi, btnBaoTri, btnNguoiDung, btnBaoCao, btnCaiDat;
        private Label? lblAppTitle;

        public MainForm(int userId, string userName, int roleId)
        {
            currentUserId = userId;
            currentUserName = userName;
            currentRoleId = roleId;
            DatabaseHelper.SetUser(userId, userName, roleId);

            InitializeComponent();
            SetupMaterialSkin();  // Thêm MaterialSkin
            SetupSidebar();
            LoadDashboardStatistics();  // Load thống kê giống repo
        }

        private void SetupMaterialSkin()
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.BlueGrey800, Primary.BlueGrey900,  // Màu chính giống repo
                Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }

        private void InitializeComponent()
        {
            this.Text = $"Quản Lý Quán Net - Xin chào {currentUserName}";
            this.Size = new System.Drawing.Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Load += MainForm_Load;
            this.Shown += MainForm_Shown;  // Sửa: Đặt SplitterDistance ở đây để tránh lỗi

            // SplitContainer
            splitContainer1 = new SplitContainer();
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Panel1MinSize = 250;
            splitContainer1.Panel2MinSize = 400;
            splitContainer1.SplitterWidth = 1;  // Thêm để giống repo

            // Sidebar Panel
            panelSidebar = new Panel();
            panelSidebar.BackColor = Color.FromArgb(240, 248, 255);  // Màu sidebar giống repo
            panelSidebar.Dock = DockStyle.Fill;
            splitContainer1.Panel1.Controls.Add(panelSidebar);

            // Main Panel
            panelMain = new Panel();
            panelMain.BackColor = Color.White;
            panelMain.Dock = DockStyle.Fill;
            splitContainer1.Panel2.Controls.Add(panelMain);

            this.Controls.Add(splitContainer1);

            // MenuStrip (giữ nguyên, nhưng thêm Material style nếu cần)
            MenuStrip menuStrip = new MenuStrip();
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("File");
            ToolStripMenuItem exitMenu = new ToolStripMenuItem("Thoát", null, (s, e) => Application.Exit());
            fileMenu.DropDownItems.Add(exitMenu);
            menuStrip.Items.Add(fileMenu);

            ToolStripMenuItem userMenu = new ToolStripMenuItem("Người Dùng");
            ToolStripMenuItem changePasswordMenu = new ToolStripMenuItem("Đổi Mật Khẩu", null, ChangePasswordMenu_Click);
            userMenu.DropDownItems.Add(changePasswordMenu);
            menuStrip.Items.Add(userMenu);

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private void MainForm_Load(object? sender, EventArgs e)
        {
            // Không đặt SplitterDistance ở đây nữa để tránh lỗi
        }

        private void MainForm_Shown(object? sender, EventArgs e)
        {
            // Sửa lỗi: Đặt SplitterDistance sau khi form hiển thị và có kích thước
            if (splitContainer1 != null)
            {
                int desiredSplitterDistance = 250;
                int minDistance = splitContainer1.Panel1MinSize;
                int maxDistance = splitContainer1.Width - splitContainer1.Panel2MinSize;

                if (desiredSplitterDistance < minDistance)
                    splitContainer1.SplitterDistance = minDistance;
                else if (desiredSplitterDistance > maxDistance)
                    splitContainer1.SplitterDistance = maxDistance;
                else
                    splitContainer1.SplitterDistance = desiredSplitterDistance;
            }
        }

        private void SetupSidebar()
        {
            // App Title (giữ nguyên)
            lblAppTitle = new Label();
            lblAppTitle.Text = "QUẢN LÝ\nQUÁN NET";
            lblAppTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblAppTitle.ForeColor = Color.FromArgb(25, 118, 210);
            lblAppTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblAppTitle.Size = new Size(230, 80);
            lblAppTitle.Location = new Point(10, 20);
            panelSidebar!.Controls.Add(lblAppTitle);

            // Buttons (sử dụng MaterialRaisedButton để giống repo)
            int buttonY = 120;
            int buttonHeight = 50;

            btnMayTinh = CreateSidebarButton("Máy Tính", buttonY);
            btnMayTinh.Click += (s, e) => OpenMayTinhForm();
            buttonY += buttonHeight + 5;

            btnThietBi = CreateSidebarButton("Thiết Bị", buttonY);
            btnThietBi.Click += (s, e) => OpenThietBiForm();
            buttonY += buttonHeight + 5;

            btnBaoTri = CreateSidebarButton("Bảo Trì", buttonY);
            btnBaoTri.Click += (s, e) => OpenBaoTriForm();
            buttonY += buttonHeight + 5;

            if (currentRoleId == 1) // Admin only
            {
                btnNguoiDung = CreateSidebarButton("Người Dùng", buttonY);
                btnNguoiDung.Click += (s, e) => OpenNguoiDungForm();
                buttonY += buttonHeight + 5;
            }

            btnBaoCao = CreateSidebarButton("Báo Cáo", buttonY);
            btnBaoCao.Click += (s, e) => OpenBaoCaoForm();
            buttonY += buttonHeight + 5;

            btnCaiDat = CreateSidebarButton("Cài Đặt", buttonY);
            btnCaiDat.Click += (s, e) => OpenCaiDatForm();
        }

        private MaterialButton CreateSidebarButton(string text, int y)  // Sửa: Sử dụng MaterialButton
        {
            MaterialButton btn = new MaterialButton();
            btn.Text = text;
            btn.Size = new Size(230, 45);
            btn.Location = new Point(10, y);
            btn.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(15, 0, 0, 0);

            panelSidebar!.Controls.Add(btn);
            return btn;
        }

        private void ApplyTheme()
        {
            this.BackColor = Color.FromArgb(250, 250, 250);
            panelMain!.BackColor = Color.White;
            panelSidebar!.BackColor = Color.FromArgb(240, 248, 255);
        }

        private void LoadDashboardStatistics()
        {
            // Thêm cards thống kê giống repo, sử dụng sp_GetMayTinhStatistics
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_GetMayTinhStatistics", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", currentUserId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string trangThai = reader["TrangThai"]?.ToString() ?? "Unknown";
                                int soLuong = reader.GetInt32("SoLuong");
                                double tyLe = reader.GetDouble("TyLePhanTram");

                                MaterialCard card = new MaterialCard
                                {
                                    Size = new Size(200, 100),
                                    Location = new Point(panelMain!.Controls.Count * 210, 20),  // Đặt cạnh nhau
                                    Dock = DockStyle.None
                                };
                                MaterialLabel lbl = new MaterialLabel
                                {
                                    Text = $"{trangThai}: {soLuong} ({tyLe}%)",
                                    Dock = DockStyle.Fill,
                                    TextAlign = ContentAlignment.MiddleCenter
                                };
                                card.Controls.Add(lbl);
                                panelMain.Controls.Add(card);
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Lỗi tải thống kê: {ex.Message}");  // Xử lý lỗi DB riêng
                }
            }
        }

        // Các phương thức Open...Form giữ nguyên, nhưng thêm xử lý lỗi
        private void OpenMayTinhForm()
        {
            try
            {
                panelMain!.Controls.Clear();
                MayTinhForm mayTinhForm = new MayTinhForm() { TopLevel = false, FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill };
                panelMain.Controls.Add(mayTinhForm);
                mayTinhForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi mở form Máy Tính: {ex.Message}");
            }
        }

        private void ChangePasswordMenu_Click(object? sender, EventArgs e)
        {
            // TODO: Mở form đổi mật khẩu sử dụng sp_ChangePassword
            MessageBox.Show("Tính năng đổi mật khẩu sẽ được triển khai sau!");
        }

        private void OpenThietBiForm()
        {
            try
            {
                panelMain!.Controls.Clear();
                ThietBiForm thietBiForm = new ThietBiForm(currentUserId) { TopLevel = false, FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill };
                panelMain.Controls.Add(thietBiForm);
                thietBiForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi mở form Thiết Bị: {ex.Message}");
            }
        }

        private void OpenBaoTriForm()
        {
            try
            {
                panelMain!.Controls.Clear();
                BaoTriForm baoTriForm = new BaoTriForm(currentUserId) { TopLevel = false, FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill };
                panelMain.Controls.Add(baoTriForm);
                baoTriForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi mở form Bảo Trì: {ex.Message}");
            }
        }

        private void OpenNguoiDungForm()
        {
            try
            {
                panelMain!.Controls.Clear();
                NguoiDungForm nguoiDungForm = new NguoiDungForm(currentUserId) { TopLevel = false, FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill };
                panelMain.Controls.Add(nguoiDungForm);
                nguoiDungForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi mở form Người Dùng: {ex.Message}");
            }
        }

        private void OpenBaoCaoForm()
        {
            try
            {
                panelMain!.Controls.Clear();
                BaoCaoForm baoCaoForm = new BaoCaoForm(currentUserId) { TopLevel = false, FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill };
                panelMain.Controls.Add(baoCaoForm);
                baoCaoForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi mở form Báo Cáo: {ex.Message}");
            }
        }

        private void OpenCaiDatForm()
        {
            try
            {
                CaiDatForm caiDatForm = new CaiDatForm();
                caiDatForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi mở form Cài Đặt: {ex.Message}");
            }
        }
    }
}