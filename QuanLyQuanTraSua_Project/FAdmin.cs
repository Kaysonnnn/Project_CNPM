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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyQuanCafe
{
  public partial class FAdmin : Form
  {
    BindingSource foodList = new BindingSource();
    BindingSource accountList = new BindingSource();
    BindingSource billList = new BindingSource();
    BindingSource categoryList = new BindingSource();
    BindingSource tableList = new BindingSource();

    public Account loginAccount;

    public FAdmin()
    {
      InitializeComponent();

      LoadData();
    }

    #region Methods
    List<Drinks> SearchFoodByName(string name)
    {
      List<Drinks> listFood = DrinksDAO.Instance.SearchFoodByName(name);

      if (listFood.Count == 0)
      {
        MessageBox.Show("Không tìm thấy!");
        return DrinksDAO.Instance.SearchFoodByName("");
      }
      else
      {
        return listFood;
      }
    }

    void LoadData()
    {
      dtgvFood.DataSource = foodList;
      dtgvAccount.DataSource = accountList;
      dtgvBill.DataSource = billList;
      dtgvCategory.DataSource = categoryList;
      dtgvTable.DataSource = tableList;

      LoadDateTimePickerBill();
      LoadListBillByDateAndPage(dtpkFromDate.Value, dtpkToDate.Value, Convert.ToInt32(txbPageBill.Text));
      LoadListFood();
      LoadAccount();
      LoadCategoryIntoCombobox(cbFoodCategory);
      LoadListCategory();
      LoadListTable();
      AddFoodBinding();
      AddAccountBinding();
      AddCategoryBinding();
      AddTableBinding();
    }
    
    void ShowBillInfo(int id, int discount)
    {
      CultureInfo culture = new CultureInfo("vi-VN");

      lsvBillInfo.Items.Clear();

      List<DTO.Menu> listBillInfo = MenuDAO.Instance.GetListMenuByBillID(id);
      
      float totalPrice = 0;
      
      foreach (DTO.Menu item in listBillInfo)
      {
        ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
        lsvItem.SubItems.Add(item.Count.ToString());
        lsvItem.SubItems.Add(item.Price.ToString());
        lsvItem.SubItems.Add(item.TotalPrice.ToString());
        
        totalPrice += item.TotalPrice;
        
        lsvBillInfo.Items.Add(lsvItem);
      }

      nmBillDiscount.Value = discount;
      txbBillTotalPrice.Text = totalPrice.ToString("c0", culture);
    }

    void AddAccountBinding()
    {
      txbUsername.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "Username", true, DataSourceUpdateMode.Never));
      txbDisplayName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "DisplayName", true, DataSourceUpdateMode.Never));
      nmAccountType.DataBindings.Add(new Binding("Value", dtgvAccount.DataSource, "Type", true, DataSourceUpdateMode.Never));
    }

    void AddCategoryBinding()
    {
      txbCategoryID.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "ID", true, DataSourceUpdateMode.Never));
      txbCategoryName.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "Name", true, DataSourceUpdateMode.Never));
    }

    void AddTableBinding()
    {
      txbTableID.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "ID", true, DataSourceUpdateMode.Never));
      txbTableName.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "Name", true, DataSourceUpdateMode.Never));
      txbTableStatus.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "Status", true, DataSourceUpdateMode.Never));
    }

    void LoadAccount()
    {
      accountList.DataSource = AccountDAO.Instance.GetListAccount();
    }

    void LoadDateTimePickerBill()
    {
      DateTime today = DateTime.Now;
      dtpkFromDate.Value = new DateTime(today.Year, today.Month, 1);
      dtpkToDate.Value = dtpkFromDate.Value.AddMonths(1).AddDays(-1);
    }

    void LoadListBillByDate(DateTime checkIn, DateTime checkOut)
    {
      dtgvBill.DataSource = BillDAO.Instance.GetBillListByDate(checkIn, checkOut);
    }

    void LoadListBillByDateAndPage(DateTime checkIn, DateTime checkOut, int page)
    {
      dtgvBill.DataSource = BillDAO.Instance.GetBillListByDateAndPage(checkIn, checkOut, page);
    }

    void AddFoodBinding()
    {
      txbFoodID.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "ID", true, DataSourceUpdateMode.Never));
      txbFoodName.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "Name", true, DataSourceUpdateMode.Never));
      nmFoodPrice.DataBindings.Add(new Binding("Value", dtgvFood.DataSource, "Price", true, DataSourceUpdateMode.Never));
    }

    void LoadCategoryIntoCombobox(ComboBox cb)
    {
      cb.DataSource = CategoryDAO.Instance.GetListCategory();
      cb.DisplayMember = "Name";
    }

    void LoadListFood()
    {
      foodList.DataSource = DrinksDAO.Instance.GetListFood();
    }

    void LoadListCategory()
    {
      categoryList.DataSource = CategoryDAO.Instance.GetListCategory();
    }

    void LoadListTable()
    {
      tableList.DataSource = TableDAO.Instance.LoadTableList();
    }

    void AddAccount(string username, string displayName, int type)
    {
      if (AccountDAO.Instance.InsertAccount(username, displayName, type))
      {
        MessageBox.Show("Thêm tài khoản thành công");
      }
      else
      {
        MessageBox.Show("Có lỗi khi thêm tài khoản");
      }

      LoadAccount();
    }

    void EditAccount(string username, string displayName, int type)
    {
      if (AccountDAO.Instance.UpdateAccount(username, displayName, type))
      {
        MessageBox.Show("Cập nhật tài khoản thành công");
      }
      else
      {
        MessageBox.Show("Có lỗi khi cập nhật tài khoản");
      }

      LoadAccount();
    }

    void DeleteAccount(string username)
    {
      if (loginAccount.UserName.Equals(username))
      {
        MessageBox.Show("Vui lòng không xóa chính bạn!");
        return;
      }

      if (AccountDAO.Instance.DeleteAccount(username))
      {
        MessageBox.Show("Xóa tài khoản thành công");
      }
      else
      {
        MessageBox.Show("Có lỗi khi xóa tài khoản");
      }

      LoadAccount();
    }

    void ResetPass(string username)
    {
      if (AccountDAO.Instance.ResetPassword(username))
      {
        MessageBox.Show("Đặt lại mật khẩu thành công");
      }
      else
      {
        MessageBox.Show("Có lỗi khi đặt lại mật khẩu");
      }
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
        $"BillID_{id}_" + DateTime.Now.ToString("ddMMyyy_hhmmss_tt");

        printDocument.Print();
      }
    }

    private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
    {
      Table table = lsvBillInfo.Tag as Table;
      CultureInfo culture = new CultureInfo("vi-VN");

      int idBill = (int)dtgvBill.SelectedCells[0].OwningRow.Cells["Mã đơn"].Value;
      List<BillInfo> billInfos = BillInfoDAO.Instance.GetListBillInfo(idBill);

      // get table name from bill id
      string tableName = (string)TableDAO.Instance.GetTableNameByBillID(idBill);

      Graphics graphic = e.Graphics;
      Font font = new Font("Courier New", 12);

      float fontHeight = font.GetHeight();

      int startX = 10;
      int startY = 10;
      int offset = 40;

      graphic.DrawString("HÓA ĐƠN THANH TOÁN", new Font("Courier New", 18), new SolidBrush(Color.Black), startX, startY);
      graphic.DrawString("Mã hóa đơn: " + idBill, font, new SolidBrush(Color.Black), startX, startY + offset);
      graphic.DrawString("Bàn: " + tableName, font, new SolidBrush(Color.Black), startX, startY + offset * 2);
      graphic.DrawString("Ngày: " + DateTime.Now.ToString(), font, new SolidBrush(Color.Black), startX, startY + offset * 3);
      graphic.DrawString("Thu ngân: " + AccountDAO.Instance.GetAccountByUserName(loginAccount.UserName).DisplayName, font, new SolidBrush(Color.Black), startX, startY + offset * 4);

      graphic.DrawString("-----------------------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offset * 5);

      graphic.DrawString("Tên món", font, new SolidBrush(Color.Black), startX, startY + offset * 6);
      graphic.DrawString("SL", font, new SolidBrush(Color.Black), startX + 250, startY + offset * 6);
      graphic.DrawString("Đơn giá", font, new SolidBrush(Color.Black), startX + 300, startY + offset * 6);
      graphic.DrawString("Thành tiền", font, new SolidBrush(Color.Black), startX + 400, startY + offset * 6);

      graphic.DrawString("-----------------------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offset * 7);

      // get list food by idBill in Menu
      List<DTO.Menu> menus = MenuDAO.Instance.GetListMenuByBillID(idBill);
      for (int i = 0; i < menus.Count; i++)
      {
        graphic.DrawString(menus[i].FoodName, font, new SolidBrush(Color.Black), startX, startY + offset * (i + 8));
        graphic.DrawString(billInfos[i].Count.ToString(), font, new SolidBrush(Color.Black), startX + 250, startY + offset * (i + 8));
        graphic.DrawString(menus[i].Price.ToString("c0", culture), font, new SolidBrush(Color.Black), startX + 300, startY + offset * (i + 8));
        graphic.DrawString(menus[i].TotalPrice.ToString("c0", culture), font, new SolidBrush(Color.Black), startX + 400, startY + offset * (i + 8));
      }

      graphic.DrawString("-----------------------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offset * (menus.Count + 8));

      float sum = menus.Sum(x => x.TotalPrice);
      float finalTotalPrice = sum - (sum / 100) * (int)nmBillDiscount.Value;
      graphic.DrawString("Tổng tiền: " + sum.ToString("c0", culture), font, new SolidBrush(Color.Black), startX, startY + offset * (menus.Count + 9));
      graphic.DrawString("Giảm giá: " + (int)nmBillDiscount.Value + "%", font, new SolidBrush(Color.Black), startX, startY + offset * (menus.Count + 10));
      graphic.DrawString("Thành tiền: " + finalTotalPrice.ToString("c0", culture), font, new SolidBrush(Color.Black), startX, startY + offset * (menus.Count + 11));

      graphic.DrawString("-----------------------------------------------------", font, new SolidBrush(Color.Black), startX, startY + offset * (menus.Count + 12));

      graphic.DrawString("Cảm ơn quý khách!", font, new SolidBrush(Color.Black), startX, startY + offset * (menus.Count + 13));
    }

    void DeleteCategoryID(int id)
    {
      if (CategoryDAO.Instance.DeleteCategory(id))
      {
        MessageBox.Show("Xóa danh mục thành công");
        LoadListCategory();
        LoadCategoryIntoCombobox(cbFoodCategory);
        if (deleteCategory != null)
          deleteCategory(this, new EventArgs());
      }
      else
      {
        MessageBox.Show("Có lỗi khi xóa danh mục");
      }
    }
    #endregion

  #region Events
    private void btnViewBill_Click(object sender, EventArgs e)
    {
      LoadListBillByDateAndPage(dtpkFromDate.Value, dtpkToDate.Value, Convert.ToInt32(txbPageBill.Text));
    }

    private void btnShowFood_Click(object sender, EventArgs e)
    {
      LoadListFood();
    }

    private void txbFoodID_TextChanged(object sender, EventArgs e)
    {
      try
      {
        if (dtgvFood.SelectedCells.Count > 0)
        {
          int id = (int)dtgvFood.SelectedCells[0].OwningRow.Cells["IDCategory"].Value;

          Category category = CategoryDAO.Instance.GetCategoryByID(id);

          cbFoodCategory.SelectedItem = category;

          int index = -1;
          int i = 0;
          foreach (Category item in cbFoodCategory.Items)
          {
            if (item.ID == category.ID)
            {
              index = i;
              break;
            }
            i++;
          }

          cbFoodCategory.SelectedIndex = index;
        }
      }
      catch { }
    }

    private void btnAddFood_Click(object sender, EventArgs e)
    {
      string name = txbFoodName.Text;
      int categoryID = (cbFoodCategory.SelectedItem as Category).ID;
      float price = (float)nmFoodPrice.Value;

      if (DrinksDAO.Instance.InsertFood(name, categoryID, price))
      {
        MessageBox.Show("Thêm món thành công");
        LoadListFood();
        if (insertFood != null)
          insertFood(this, new EventArgs());
      }
      else
      {
        MessageBox.Show("Có lỗi khi thêm thức ăn");
      }
    }

    private void btnEditFood_Click(object sender, EventArgs e)
    {
      string name = txbFoodName.Text;
      int categoryID = (cbFoodCategory.SelectedItem as Category).ID;
      float price = (float)nmFoodPrice.Value;
      int id = Convert.ToInt32(txbFoodID.Text);

      if (DrinksDAO.Instance.UpdateFood(id, name, categoryID, price))
      {
        MessageBox.Show("Sửa món thành công");
        LoadListFood();
        if (updateFood != null)
          updateFood(this, new EventArgs());
      }
      else
      {
        MessageBox.Show("Có lỗi khi sửa thức ăn");
      }
    }

    private void btnDeleteFood_Click(object sender, EventArgs e)
    {
      int id = Convert.ToInt32(txbFoodID.Text);

      if (DrinksDAO.Instance.DeleteFood(id))
      {
        MessageBox.Show("Xóa món thành công");
        LoadListFood();
        if (deleteFood != null)
          deleteFood(this, new EventArgs());
      }
      else
      {
        MessageBox.Show("Có lỗi khi xóa thức ăn");
      }
    }

    private void btnSearchFood_Click(object sender, EventArgs e)
    {
      foodList.DataSource = SearchFoodByName(txbSearchFoodName.Text);
    }

    private void btnShowAccount_Click(object sender, EventArgs e)
    {
      LoadAccount();
    }

    private void btnAddAccount_Click(object sender, EventArgs e)
    {
      string username = txbUsername.Text;
      string displayName = txbDisplayName.Text;
      int type = (int)nmAccountType.Value;

      AddAccount(username, displayName, type);
    }

    private void btnDeleteAccount_Click(object sender, EventArgs e)
    {
      string username = txbUsername.Text;

      DeleteAccount(username);
    }

    private void btnEditAccount_Click(object sender, EventArgs e)
    {
      string username = txbUsername.Text;
      string displayName = txbDisplayName.Text;
      int type = (int)nmAccountType.Value;

      EditAccount(username, displayName, type);
    }

    private void btnResetPassword_Click(object sender, EventArgs e)
    {
      string username = txbUsername.Text;

      ResetPass(username);
    }

    private void btnFirstBillPage_Click(object sender, EventArgs e)
    {
      txbPageBill.Text = "1";
    }

    private void btnLastBillPage_Click(object sender, EventArgs e)
    {
      int sumRecord = BillDAO.Instance.GetNumBillListByDate(dtpkFromDate.Value, dtpkToDate.Value);

      int lastPage = sumRecord / 10;

      if (sumRecord % 10 != 0)
        lastPage++;

      txbPageBill.Text = lastPage.ToString();
    }

    private void txbPageBill_TextChanged(object sender, EventArgs e)
    {
      dtgvBill.DataSource = BillDAO.Instance.GetBillListByDateAndPage(dtpkFromDate.Value, dtpkToDate.Value, Convert.ToInt32(txbPageBill.Text));
    }

    private void btnPreviousBillPage_Click(object sender, EventArgs e)
    {
      int page = Convert.ToInt32(txbPageBill.Text);

      if (page > 1)
        page--;

      txbPageBill.Text = page.ToString();
    }

    private void btnNextBillPage_Click(object sender, EventArgs e)
    {
      int page = Convert.ToInt32(txbPageBill.Text);
      int sumRecord = BillDAO.Instance.GetNumBillListByDate(dtpkFromDate.Value, dtpkToDate.Value);

      if (page < sumRecord)
        page++;

      txbPageBill.Text = page.ToString();
    }

    private void dtgvBill_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      int id = (int)dtgvBill.SelectedCells[0].OwningRow.Cells["Mã đơn"].Value;
      // get discount
      int discount = (int)dtgvBill.SelectedCells[0].OwningRow.Cells["Giảm giá"].Value;

      ShowBillInfo(id, discount);
    }

    private void btnPrintBill_Click(object sender, EventArgs e)
    {
      int id = (int)dtgvBill.SelectedCells[0].OwningRow.Cells["Mã đơn"].Value;
      PrintBillByID(id);
    }

    private void btnShowCategory_Click(object sender, EventArgs e)
    {
      LoadListCategory();
    }

    private void btnAddCategory_Click(object sender, EventArgs e)
    {
      string name = txbCategoryName.Text;

      if (CategoryDAO.Instance.InsertCategory(name))
      {
        MessageBox.Show("Thêm danh mục thành công");
        LoadListCategory();
        LoadCategoryIntoCombobox(cbFoodCategory);
        if (insertCategory != null)
          insertCategory(this, new EventArgs());
      }
      else
      {
        MessageBox.Show("Có lỗi khi thêm danh mục");
      }
    }

    private void btnEditCategory_Click(object sender, EventArgs e)
    {
      string name = txbCategoryName.Text;
      int id = Convert.ToInt32(txbCategoryID.Text);

      if (CategoryDAO.Instance.UpdateCategory(id, name))
      {
        MessageBox.Show("Sửa danh mục thành công");
        LoadListCategory();
        LoadCategoryIntoCombobox(cbFoodCategory);
        if (updateCategory != null)
          updateCategory(this, new EventArgs());
      }
      else
      {
        MessageBox.Show("Có lỗi khi sửa danh mục");
      }
    }

    private void btnDeleteCategory_Click(object sender, EventArgs e)
    {
      int id = Convert.ToInt32(txbCategoryID.Text);

      List<Drinks> listFood = DrinksDAO.Instance.GetFoodListByCategoryID(id);

      // thông báo khi xóa danh mục có thức ăn thuộc danh mục này, nếu đồng ý thì xóa thức ăn đó trước rồi mới xóa danh mục
      if (listFood.Count > 0)
      {
        if (MessageBox.Show(string.Format("Danh mục {0} có chứa thức ăn, bạn có chắc muốn xóa danh mục này không?", txbCategoryName.Text), "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
        {
          foreach (Drinks item in listFood)
          {
                DrinksDAO.Instance.DeleteFood(item.ID);
          }
          DeleteCategoryID(id);
          LoadListFood();
        }
      }
      else
      {
        DeleteCategoryID(id);
      }
    }

    private void btnShowTable_Click(object sender, EventArgs e)
    {
      LoadListTable();
    }

    private void btnAddTable_Click(object sender, EventArgs e)
    {
      // thêm 1 bàn tiếp theo theo số lượng bàn hiện có + 1 và tên bàn là "Bàn " + số lượng bàn hiện có + 1
      string name = "Bàn " + (TableDAO.Instance.GetMaxIDTable() + 1).ToString();
      string status = "Trống";

      if (TableDAO.Instance.InsertTable(name, status))
      {
        MessageBox.Show("Thêm bàn thành công");
        LoadListTable();
        if (insertTable != null)
          insertTable(this, new EventArgs());
      }
      else
      {
        MessageBox.Show("Có lỗi khi thêm bàn");
      }
    }

    private void btnDeleteTable_Click(object sender, EventArgs e)
    {
      int id = Convert.ToInt32(txbTableID.Text);

      if (TableDAO.Instance.DeleteTable(id))
      {
        MessageBox.Show("Xóa bàn thành công");
        LoadListTable();
        if (deleteTable != null)
          deleteTable(this, new EventArgs());
      }
      else
      {
        MessageBox.Show("Có lỗi khi xóa bàn");
      }
    }

    private event EventHandler insertFood;
    public event EventHandler InsertFood
    {
      add { insertFood += value; }
      remove { insertFood -= value; }
    }

    private event EventHandler deleteFood;
    public event EventHandler DeleteFood
    {
      add { deleteFood += value; }
      remove { deleteFood -= value; }
    }

    private event EventHandler updateFood;
    public event EventHandler UpdateFood
    {
      add { updateFood += value; }
      remove { updateFood -= value; }
    }

    private event EventHandler insertCategory;
    public event EventHandler InsertCategory
    {
      add { insertCategory += value; }
      remove { insertCategory -= value; }
    }

    private event EventHandler deleteCategory;
    public event EventHandler DeleteCategory
    {
      add { deleteCategory += value; }
      remove { deleteCategory -= value; }
    }

    private event EventHandler updateCategory;
    public event EventHandler UpdateCategory
    {
      add { updateCategory += value; }
      remove { updateCategory -= value; }
    }

    private event EventHandler insertTable;
    public event EventHandler InsertTable
    {
      add { insertTable += value; }
      remove { insertTable -= value; }
    }

    private event EventHandler deleteTable;
    public event EventHandler DeleteTable
    {
      add { deleteTable += value; }
      remove { deleteTable -= value; }
    }
        #endregion

        private void label18_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void label17_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
