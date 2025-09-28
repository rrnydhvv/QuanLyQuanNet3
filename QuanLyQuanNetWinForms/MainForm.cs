using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyQuanNetWinForms
{
    public partial class MainForm : Form
    {
        private int currentUserId;
        private string currentUserName;
        private int currentRoleId;
        private string connectionString = "Server=.;Database=QuanLyQuanNet;Trusted_Connection=True;";

        private SplitContainer? splitContainer1;
        private Panel? panelSidebar;
        private Panel? panelMain;
        private Button? btnMayTinh, btnThietBi, btnBaoTri, btnNguoiDung, btnBaoCao, btnCaiDat;
        private Label? lblAppTitle;

        public MainForm(int userId, string userName, int roleId)
        {
            currentUserId = userId;
            currentUserName = userName;
            currentRoleId = roleId;
            DatabaseHelper.SetUser(userId, userName, roleId);

            InitializeComponent();
            SetupSidebar();
            ApplyTheme();
        }

        private void InitializeComponent()
        {
            this.Text = $"Quản Lý Quán Net - Xin chào {currentUserName}";
            this.Size = new System.Drawing.Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Load += MainForm_Load;

            // SplitContainer
            splitContainer1 = new SplitContainer();
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Panel1MinSize = 250;
            splitContainer1.Panel2MinSize = 400;

            // Sidebar Panel
            panelSidebar = new Panel();
            panelSidebar.BackColor = Color.FromArgb(240, 248, 255);
            panelSidebar.Dock = DockStyle.Fill;
            splitContainer1.Panel1.Controls.Add(panelSidebar);

            // Main Panel
            panelMain = new Panel();
            panelMain.BackColor = Color.White;
            panelMain.Dock = DockStyle.Fill;
            splitContainer1.Panel2.Controls.Add(panelMain);

            this.Controls.Add(splitContainer1);

            // MenuStrip
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
            // Đặt SplitterDistance sau khi form đã được load và có kích thước chính xác
            if (splitContainer1 != null)
            {
                // Đảm bảo form đã được layout đúng cách
                this.PerformLayout();

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
            // App Title
            lblAppTitle = new Label();
            lblAppTitle.Text = "QUẢN LÝ\nQUÁN NET";
            lblAppTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblAppTitle.ForeColor = Color.FromArgb(25, 118, 210);
            lblAppTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblAppTitle.Size = new Size(230, 80);
            lblAppTitle.Location = new Point(10, 20);
            panelSidebar!.Controls.Add(lblAppTitle);

            // Buttons
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

        private Button CreateSidebarButton(string text, int y)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Size = new Size(230, 45);
            btn.Location = new Point(10, y);
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = Color.FromArgb(240, 248, 255);
            btn.ForeColor = Color.FromArgb(33, 33, 33);
            btn.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(15, 0, 0, 0);
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;

            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(189, 229, 252);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(240, 248, 255);

            panelSidebar!.Controls.Add(btn);
            return btn;
        }

        private void ApplyTheme()
        {
            this.BackColor = Color.FromArgb(250, 250, 250);
            panelMain!.BackColor = Color.White;
            panelSidebar!.BackColor = Color.FromArgb(240, 248, 255);
        }

        private void OpenMayTinhForm()
        {
            panelMain!.Controls.Clear();
            MayTinhForm mayTinhForm = new MayTinhForm(currentUserId) { TopLevel = false, FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill };
            panelMain.Controls.Add(mayTinhForm);
            mayTinhForm.Show();
        }

        private void OpenThietBiForm()
        {
            panelMain!.Controls.Clear();
            ThietBiForm thietBiForm = new ThietBiForm(currentUserId) { TopLevel = false, FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill };
            panelMain.Controls.Add(thietBiForm);
            thietBiForm.Show();
        }

        private void OpenBaoTriForm()
        {
            panelMain!.Controls.Clear();
            BaoTriForm baoTriForm = new BaoTriForm(currentUserId) { TopLevel = false, FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill };
            panelMain.Controls.Add(baoTriForm);
            baoTriForm.Show();
        }

        private void OpenNguoiDungForm()
        {
            if (currentRoleId != 1) return;
            panelMain!.Controls.Clear();
            NguoiDungForm nguoiDungForm = new NguoiDungForm(currentUserId) { TopLevel = false, FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill };
            panelMain.Controls.Add(nguoiDungForm);
            nguoiDungForm.Show();
        }

        private void OpenBaoCaoForm()
        {
            panelMain!.Controls.Clear();
            BaoCaoForm baoCaoForm = new BaoCaoForm(currentUserId) { TopLevel = false, FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill };
            panelMain.Controls.Add(baoCaoForm);
            baoCaoForm.Show();
        }

        private void OpenCaiDatForm()
        {
            panelMain!.Controls.Clear();
            CaiDatForm caiDatForm = new CaiDatForm() { TopLevel = false, FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill };
            panelMain.Controls.Add(caiDatForm);
            caiDatForm.Show();
        }

        private void ChangePasswordMenu_Click(object? sender, EventArgs e)
        {
            // TODO: Implement change password dialog
            MessageBox.Show("Tính năng đổi mật khẩu sẽ được triển khai sau!");
        }
    }
}