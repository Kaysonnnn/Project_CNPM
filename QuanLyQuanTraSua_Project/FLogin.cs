using QuanLyQuanCafe.DAO;
using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyQuanCafe
{
  public partial class FLogin : Form
  {
    public FLogin()
    {
      InitializeComponent();
    }

    private void btnLogin_Click(object sender, EventArgs e)
    {
      string username = txbUserName.Text;
      string password = txbPassWord.Text;

      if (Login(username, password))
      {
        Account loginAccount = AccountDAO.Instance.GetAccountByUserName(username);
        FTableManager f = new FTableManager(loginAccount);
        this.Hide();
        f.ShowDialog();
        this.Show();
      }
      else
      {
        MessageBox.Show("Sai tên tài khoản hoặc mật khẩu!");
      }
    }

    bool Login(string username, string password)
    {
      return AccountDAO.Instance.Login(username, password);
    }

    private void btnExit_Click(object sender, EventArgs e)
    {
      Application.Exit();
    }

    private void FLogin_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (MessageBox.Show("Bạn có chắc chắn muốn thoát không?", "Thông báo", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
      {
        e.Cancel = true;
      }
    }

        private void label1_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void txbUserName_Leave(object sender, EventArgs e)
        {
            if (txbUserName.Text == "")
            {
                txbUserName.Text = "Tên Đăng Nhập";
            }
        }

        private void txbUserName_TextChanged(object sender, EventArgs e)
        {
            pnlUser.ForeColor = Color.FromArgb(78, 184, 206);
            txbUserName.ForeColor = Color.FromArgb(0, 0, 0);
            if (txbPassWord.Text == "Mật Khẩu")
                txbPassWord.ForeColor = Color.White;
            else
                txbPassWord.ForeColor = Color.Black;
        }

        private void txbUserName_Click(object sender, EventArgs e)
        {
            if (txbUserName.Text == "Tên Đăng Nhập")
            {
                txbUserName.Clear();
            }
            pnlUser.ForeColor = Color.FromArgb(78, 184, 206);
            txbUserName.ForeColor = Color.FromArgb(0, 0, 0);

            pnlPass.ForeColor = Color.White;
            if (txbPassWord.Text == "Mật Khẩu")
                txbPassWord.ForeColor = Color.White;
            else
                txbPassWord.ForeColor = Color.Black;
        }

        private void txbPassWord_TextChanged(object sender, EventArgs e)
        {
            if (!(txbPassWord.Text == "Mật Khẩu" || txbPassWord.Text == ""))
            {
                txbPassWord.PasswordChar = '*';
            }
            pnlPass.ForeColor = Color.FromArgb(78, 184, 206);
            txbPassWord.ForeColor = Color.FromArgb(0, 0, 0);

            pnlUser.ForeColor = Color.White;
            if (txbUserName.Text == "Tên Đăng Nhập")
                txbUserName.ForeColor = Color.White;
            else
                txbUserName.ForeColor = Color.Black;
        }

        private void txbPassWord_Leave(object sender, EventArgs e)
        {
            if (txbPassWord.Text == "")
            {
                txbPassWord.Text = "Mật Khẩu";
                txbPassWord.PasswordChar = default;
            }
        }

        private void txbPassWord_Click(object sender, EventArgs e)
        {
            if (txbPassWord.Text == "Mật Khẩu" )
            {
                txbPassWord.Clear();
            }
            pnlPass.ForeColor = Color.FromArgb(78, 184, 206);
            txbPassWord.ForeColor = Color.FromArgb(0, 0, 0);
            pnlUser.ForeColor = Color.White;
            if (txbUserName.Text == "Tên Đăng Nhập")
                txbUserName.ForeColor = Color.White;
            else
                txbUserName.ForeColor = Color.Black;
        }
    }
}
