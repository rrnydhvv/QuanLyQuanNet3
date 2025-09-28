using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuanLyQuanNetWinForms
{
    public static class DatabaseHelper
    {
        private static string connectionString = @"Server=.;Database=QuanLyQuanNet;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False;";

        public static string CurrentUsername { get; private set; } = string.Empty;
        public static int CurrentUserId { get; private set; } = 0;
        public static int CurrentRoleId { get; private set; } = 0;

        public enum UserRole
        {
            Unknown = 0,
            Admin = 1,
            Tech = 2,
            Staff = 3
        }

        public static UserRole CurrentRole { get; private set; } = UserRole.Unknown;

        public static void SetUser(int userId, string username, int roleId)
        {
            CurrentUserId = userId;
            CurrentUsername = username;
            CurrentRoleId = roleId;
            CurrentRole = (UserRole)roleId;
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
                catch (SqlException sqlEx) when (sqlEx.Message.Contains("permission was denied"))
                {
                    ShowPermissionDenied("Truy vấn dữ liệu", query);
                    return null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                    return null;
                }
            }
        }

        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteNonQuery();
                }
                catch (SqlException sqlEx) when (sqlEx.Message.Contains("permission was denied"))
                {
                    ShowPermissionDenied("Thực hiện thao tác", query);
                    return -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                    return -1;
                }
            }
        }

        public static object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteScalar();
                }
                catch (SqlException sqlEx) when (sqlEx.Message.Contains("permission was denied"))
                {
                    ShowPermissionDenied("Thực hiện truy vấn", query);
                    return null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                    return null;
                }
            }
        }

        public static int ExecuteStoredProcedureNonQuery(string spName, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(spName, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    return cmd.ExecuteNonQuery();
                }
                catch (SqlException sqlEx) when (sqlEx.Message.Contains("permission was denied"))
                {
                    ShowPermissionDenied("Gọi stored procedure", spName);
                    return -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                    return -1;
                }
            }
        }

        public static DataTable ExecuteStoredProcedure(string spName, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(spName, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
                catch (SqlException sqlEx) when (sqlEx.Message.Contains("permission was denied"))
                {
                    ShowPermissionDenied("Gọi stored procedure", spName);
                    return null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                    return null;
                }
            }
        }

        public static void ShowPermissionDenied(string action = "", string objectName = "", string sqlError = "")
        {
            string roleText = CurrentRole switch
            {
                UserRole.Admin => "Admin",
                UserRole.Tech => "Kỹ thuật viên",
                UserRole.Staff => "Nhân viên",
                _ => "Không xác định"
            };

            string message = $"🚫 Bạn không có quyền {action.ToLower()}!\n\n";
            if (!string.IsNullOrEmpty(objectName))
            {
                message += $"🎯 Đối tượng: {objectName}\n";
            }
            message += $"👤 Người dùng: {CurrentUsername}\n" +
                      $"🔑 Vai trò hiện tại: {roleText}\n";

            if (!string.IsNullOrEmpty(sqlError))
            {
                message += $"\n🔍 Chi tiết lỗi:\n{sqlError}\n";
            }

            message += "\n💡 Vui lòng liên hệ quản trị viên để được cấp quyền!";
            MessageBox.Show(message, "Cảnh báo phân quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}