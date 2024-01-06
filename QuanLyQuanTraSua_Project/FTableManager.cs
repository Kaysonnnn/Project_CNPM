using QuanLyQuanCafe.DAO;
using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyQuanCafe
{
  public partial class FTableManager : Form
  {
    private string FormName = "Phần mềm quản lý quán tra sua Balley - Lol Team";
    
    private Account loginAccount;

    public Account LoginAccount
    {
      get => loginAccount;
      set { loginAccount = value; ChangeAccount(loginAccount.Type); }
    }

    public FTableManager(Account acc)
    {
      InitializeComponent();

      this.Text = FormName + " - Chưa chọn bàn";

      this.LoginAccount = acc;

      LoadTable();
      LoadCategory();
      LoadComboboxTable(cbSwitchTable);
      LoadComboboxTable(cbMergeTable);
    }

    #region Methods
    void ChangeAccount(int type)
    {
      adminToolStripMenuItem.Visible = type == 1; 

      thôngTinTàiKhoảnToolStripMenuItem.Text += " (" + LoginAccount.DisplayName + ")";
    }

    void LoadCategory()
    {
      List<Category> listCategory = CategoryDAO.Instance.GetListCategory();
      cbCategory.DataSource = listCategory;
      cbCategory.DisplayMember = "Name";
    }

    void LoadFoodListByCategoryID(int id)
    {
      List<Drinks> listFood = DrinksDAO.Instance.GetFoodByCategoryID(id);
      cbFood.DataSource = listFood;
      cbFood.DisplayMember = "Name";
    }

    void LoadTable()
    {
      flpTable.Controls.Clear();

      List<Table> tableList = TableDAO.Instance.LoadTableList();

      foreach (Table item in tableList)
      {
        Button btn = new Button() { Width = TableDAO.TableWidth, Height = TableDAO.TableHeight };
        btn.Text = item.Name + Environment.NewLine + item.Status;
        btn.Click += btn_Click;
        btn.Tag = item;

        switch (item.Status)
        {
          case "Trống":
            btn.BackColor = Color.Aqua;
            break;
          default:
            btn.BackColor = Color.LightPink;
            break;
        }
        flpTable.Controls.Add(btn);
      }
    }

    void ShowBill(int id)
    {
      CultureInfo culture = new CultureInfo("vi-VN");

      lsvBill.Items.Clear();

      List<DTO.Menu> listBillInfo = MenuDAO.Instance.GetListMenuByTable(id);

      float totalPrice = 0;

      foreach (DTO.Menu item in listBillInfo)
      {
        ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());

        lsvItem.SubItems.Add(item.Count.ToString());
        lsvItem.SubItems.Add(item.Price.ToString("c0", culture));
        lsvItem.SubItems.Add(item.TotalPrice.ToString("c0", culture));

        totalPrice += item.TotalPrice;

        lsvBill.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        lsvBill.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

        lsvBill.Items.Add(lsvItem);
      }

      txbTotalPrice.Text = totalPrice.ToString("c0", culture);
    }

    void LoadComboboxTable(ComboBox cb)
    {
      cb.DataSource = TableDAO.Instance.LoadTableList();
      cb.DisplayMember = "Name";
    }
    #endregion

    #region Events
    private void btn_Click(object sender, EventArgs e)
    {
      int tableID = ((sender as Button).Tag as Table).ID;
      lsvBill.Tag = (sender as Button).Tag;
      this.Text = FormName + " - " + ((sender as Button).Tag as Table).Name;
      ShowBill(tableID);
    }

    private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void thôngTinCáNhânToolStripMenuItem_Click(object sender, EventArgs e)
    {
      FAccountProfile f = new FAccountProfile(LoginAccount);
      f.UpdateAccount += f_UpdateAccount;
      f.ShowDialog();
    }

    void f_UpdateAccount(object sender, AccountEvent e)
    {
      thôngTinTàiKhoảnToolStripMenuItem.Text = "Thông tin tài khoản (" + e.Acc.DisplayName + ")";
    }

    private void adminToolStripMenuItem_Click(object sender, EventArgs e)
    {
      FAdmin f = new FAdmin();
      
      f.loginAccount = LoginAccount;
      
      f.InsertFood += f_InsertFood;
      f.DeleteFood += f_DeleteFood;
      f.UpdateFood += f_UpdateFood;

      f.InsertCategory += f_InsertCategory;
      f.UpdateCategory += f_UpdateCategory;
      f.DeleteCategory += f_DeleteCategory;
      
      f.InsertTable += f_InsertTable;
      f.DeleteTable += f_DeleteTable;
      
      f.ShowDialog();
    }

    private void f_InsertCategory(object sender, EventArgs e)
    {
      LoadCategory();
      if (cbCategory.Items.Count > 0)
        LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
      if (lsvBill.Tag != null)
        ShowBill((lsvBill.Tag as Table).ID);
    }

    private void f_UpdateCategory(object sender, EventArgs e)
    {
      LoadCategory();
      if (lsvBill.Tag != null)
        ShowBill((lsvBill.Tag as Table).ID);
    }

    private void f_DeleteCategory(object sender, EventArgs e)
    {
      LoadCategory();
      if (cbCategory.Items.Count > 0)
        LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
      if (lsvBill.Tag != null)
        ShowBill((lsvBill.Tag as Table).ID);
      LoadTable();
    }

    private void f_DeleteTable(object sender, EventArgs e)
    {
      LoadTable();
      LoadComboboxTable(cbSwitchTable);
      LoadComboboxTable(cbMergeTable);
    }

    private void f_InsertTable(object sender, EventArgs e)
    {
      LoadTable();
      LoadComboboxTable(cbSwitchTable);
      LoadComboboxTable(cbMergeTable);
    }

    private void f_UpdateFood(object sender, EventArgs e)
    {
      LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
      if (lsvBill.Tag != null)
        ShowBill((lsvBill.Tag as Table).ID);
    }

    private void f_DeleteFood(object sender, EventArgs e)
    {
      LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
      if (lsvBill.Tag != null)
        ShowBill((lsvBill.Tag as Table).ID);
      LoadTable();
    }

    private void f_InsertFood(object sender, EventArgs e)
    {
      LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
      if (lsvBill.Tag != null)
        ShowBill((lsvBill.Tag as Table).ID);
    }

    private void cbCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
      LoadFoodListByCategoryID((cbCategory.SelectedItem as Category).ID);
    }

    private void btnAddFood_Click(object sender, EventArgs e)
    {
      Table table = lsvBill.Tag as Table;

      if (table == null)
      {
        MessageBox.Show("Hãy chọn bàn");
        return;
      }

      int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
      int foodID = (cbFood.SelectedItem as Drinks).ID;
      int count = (int)nmFoodCount.Value;

      if (idBill == -1)
      {
        BillDAO.Instance.InsertBill(table.ID);
        BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetMaxIDBill(), foodID, count);
      }
      else
      {
        BillInfoDAO.Instance.InsertBillInfo(idBill, foodID, count);
      }

      ShowBill(table.ID);

      LoadTable();
    }

    private void btnCheckOut_Click(object sender, EventArgs e)
    {
            try
            {
                CultureInfo culture = new CultureInfo("vi-VN");
                Table table = lsvBill.Tag as Table;

                int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
                int discount = (int)nmDiscount.Value;
                // split space, remove dot (.) and convert to double
                double totalPrice = double.Parse(txbTotalPrice.Text.Split(' ')[0].Replace(".", ""));
                double finalTotalPrice = totalPrice - (totalPrice / 100) * discount;

                if (idBill != -1)
                {
                    if (MessageBox.Show(string.Format("Bạn có chắc thanh toán hóa đơn cho bàn {0}\nTổng tiền - (Tổng tiền / 100) x Giảm giá\n= {1} - ({1} / 100) x {2}%\n= {3}", table.Name, totalPrice, discount, finalTotalPrice.ToString("c0", culture)), "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                    {
                        PrintBillByID(idBill);
                        BillDAO.Instance.CheckOut(idBill, discount, (float)finalTotalPrice);
                        ShowBill(table.ID);
                        LoadTable();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
     }

    private void btnSwitchTable_Click(object sender, EventArgs e)
    {
      int id1;
      if (lsvBill.Tag == null)
      {
        MessageBox.Show("Hãy chọn bàn cần chuyển");
        return;
      }
      else
      {
        id1 = (lsvBill.Tag as Table).ID;
      }

      int id2 = (cbSwitchTable.SelectedItem as Table).ID;

      if (id1 == id2)
      {
        MessageBox.Show("Hai bàn trùng nhau, hãy chọn lại");
        return;
      }

      if (MessageBox.Show(string.Format("Bạn có thật sự muốn chuyển/đổi từ {0} qua {1}", (lsvBill.Tag as Table).Name, (cbSwitchTable.SelectedItem as Table).Name), "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
      {
        TableDAO.Instance.SwitchTable(id1, id2);

        LoadTable();
      }
    }

    private void btnMergeTable_Click(object sender, EventArgs e)
    {
      int id1;
      if (lsvBill.Tag == null)
      {
        MessageBox.Show("Hãy chọn bàn cần gộp");
        return;
      }
      else
      {
        id1 = (lsvBill.Tag as Table).ID;
      }

      int id2 = (cbMergeTable.SelectedItem as Table).ID;

      if (id1 == id2)
      {
        MessageBox.Show("Hai bàn trùng nhau, hãy chọn lại");
        return;
      }

      if (MessageBox.Show(string.Format("Bạn có thật sự muốn gộp {0} qua {1}", (lsvBill.Tag as Table).Name, (cbMergeTable.SelectedItem as Table).Name), "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
      {
        TableDAO.Instance.MergeTable(id1, id2);

        LoadTable();
      }
    }

    private void thêmMónToolStripMenuItem_Click(object sender, EventArgs e)
    {
      btnAddFood_Click(this, new EventArgs());
    }

    private void thanhToánToolStripMenuItem_Click(object sender, EventArgs e)
    {
      btnCheckOut_Click(this, new EventArgs());
    }

    void PrintBillByID(int? id)
    {
      PrintDialog printDialog = new PrintDialog();
      PrintDocument printDocument = new PrintDocument();
      printDocument.PrintPage += PrintDocument_PrintPage;

      if (printDialog.ShowDialog() == DialogResult.OK)
      {
        printDocument.PrinterSettings.PrinterName =
        printDialog.PrinterSettings.PrinterName;

        printDocument.DefaultPageSettings.PaperSize =
        printDialog.PrinterSettings.PaperSizes[8]; //giấy A4(1169 - 827)

        printDocument.DocumentName =
        $"Bill_{id}_" + DateTime.Now.ToString("ddMMyyy_hhmmss_tt");

        printDocument.Print();
      }
    }

    private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
    {
      Table table = lsvBill.Tag as Table;
      CultureInfo culture = new CultureInfo("vi-VN");

      int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);

      List<BillInfo> billInfos = BillInfoDAO.Instance.GetListBillInfo(idBill);

      Graphics graphic = e.Graphics;
      Font font = new Font("Courier New", 12);

      float fontHeight = font.GetHeight();

      int startX = 10;
      int startY = 10;
      int offset = 40;

      graphic.DrawString("HÓA ĐƠN THANH TOÁN TRÀ SỮA BALLEY", new Font("Courier New", 18), new SolidBrush(Color.Black), startX, startY);
      graphic.DrawString("Mã hóa đơn: " + idBill, font, new SolidBrush(Color.Black), startX, startY + offset);
      graphic.DrawString("Bàn: " + table.Name, font, new SolidBrush(Color.Black), startX, startY + offset * 2);
      graphic.DrawString("Ngày: " + DateTime.Now.ToString(), font, new SolidBrush(Color.Black), startX, startY + offset * 3);
      graphic.DrawString("Thu ngân: " + AccountDAO.Instance.GetAccountByUserName(loginAccount.UserName).DisplayName, font, new SolidBrush(Color.Black), startX, startY + offset * 4);

      graphic.DrawString("-----------------------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offset * 5);

      graphic.DrawString("Tên món", font, new SolidBrush(Color.Black), startX, startY + offset * 6);
      graphic.DrawString("SL", font, new SolidBrush(Color.Black), startX + 250, startY + offset * 6);
      graphic.DrawString("Đơn giá", font, new SolidBrush(Color.Black), startX + 300, startY + offset * 6);
      graphic.DrawString("Thành tiền", font, new SolidBrush(Color.Black), startX + 400, startY + offset * 6);

      graphic.DrawString("-----------------------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offset * 7);

      // get list food by idBill in Menu
      List<DTO.Menu> menus = MenuDAO.Instance.GetListMenuByTable(table.ID);
      for (int i = 0; i < menus.Count; i++)
      {
        graphic.DrawString(menus[i].FoodName, font, new SolidBrush(Color.Black), startX, startY + offset * (i + 8));
        graphic.DrawString(billInfos[i].Count.ToString(), font, new SolidBrush(Color.Black), startX + 250, startY + offset * (i + 8));
        graphic.DrawString(menus[i].Price.ToString("c0", culture), font, new SolidBrush(Color.Black), startX + 300, startY + offset * (i + 8));
        graphic.DrawString(menus[i].TotalPrice.ToString("c0", culture), font, new SolidBrush(Color.Black), startX + 400, startY + offset * (i + 8));
      }

      graphic.DrawString("-----------------------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offset * (menus.Count + 8));

      float sum = menus.Sum(x => x.TotalPrice);
      float finalTotalPrice = sum - (sum / 100) * (int)nmDiscount.Value;
      graphic.DrawString("Tổng tiền: " + sum.ToString("c0", culture), font, new SolidBrush(Color.Black), startX, startY + offset * (menus.Count + 9));
      graphic.DrawString("Giảm giá: " + (int)nmDiscount.Value + "%", font, new SolidBrush(Color.Black), startX, startY + offset * (menus.Count + 10));
      graphic.DrawString("Thành tiền: " + finalTotalPrice.ToString("c0", culture), font, new SolidBrush(Color.Black), startX, startY + offset * (menus.Count + 11));

      graphic.DrawString("-----------------------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offset * (menus.Count + 12));

      graphic.DrawString("Trà Sữa BALLEY cảm ơn quý khách! Hẹn gặp lại!", font, new SolidBrush(Color.Black), startX, startY + offset * (menus.Count + 13));
    }
        #endregion

        private void label8_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
