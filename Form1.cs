using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Runtime.InteropServices;

namespace Inventory_Management_System
{
    public partial class Form1 : Form
    {
        // WinAPI importları
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        public Form1()
        {
            InitializeComponent();
            btnClose.Click += BtnClose_Click;
            btnMinimize.Click += BtnMinimize_Click;
            this.AcceptButton = button1;
            panelTop.MouseDown += panelTop_MouseDown;
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnMinimize_Click(object? sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string kullaniciAdi = textBox1.Text.Trim();
            string sifre = textBox2.Text.Trim();

            if (string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrWhiteSpace(sifre))
            {
                MessageBox.Show("Lütfen kullanıcı adı ve şifre girin!",
                                "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string connectionString = "Server=localhost;Database=envanterdb;Uid=root;Pwd=;";
                using var conn = new MySqlConnection(connectionString);
                conn.Open();

                string query = "SELECT rol FROM kullanicilar WHERE kullaniciAdi=@kullanici AND sifre=@sifre";
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@kullanici", kullaniciAdi);
                cmd.Parameters.AddWithValue("@sifre", sifre);

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    string rol = result.ToString() ?? "";
                    MessageBox.Show($"Giriş başarılı! Hoş geldin {rol}",
                                    "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    MainForm anaForm = new MainForm();
                    anaForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Kullanıcı adı veya şifre hatalı!",
                                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    textBox1.Clear();
                    textBox2.Clear();
                    textBox1.Focus();
                }
            }
            catch (MySqlException sqlEx)
            {
                MessageBox.Show("Veritabanı hatası: " + sqlEx.Message,
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Beklenmeyen hata: " + ex.Message,
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panelTop_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }
    }
}