using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
  public class TableDAO
  {
    private static TableDAO instance;

    public static TableDAO Instance
    {
      get { if (instance == null) instance = new TableDAO(); return TableDAO.instance; }
      private set { TableDAO.instance = value; }
    }

    public static int TableWidth = 95;
    public static int TableHeight = 95;

    private TableDAO() { }

    public void SwitchTable(int id1, int id2)
    {
      DataProvider.Instance.ExecuteQuery("USP_SwitchTable @idTable1 , @idTable2", new object[] { id1, id2 });
    }

    public void MergeTable(int id1, int id2)
    {
      DataProvider.Instance.ExecuteQuery("USP_MergeTable @idTable1 , @idTable2", new object[] { id1, id2 });
    }

    // GetMaxIDTable
    public int GetMaxIDTable()
    {
      try
      {
        return (int)DataProvider.Instance.ExecuteScalar("SELECT MAX(id) FROM dbo.TableFood");
      }
      catch
      {
        return 1;
      }
    }

    public List<Table> LoadTableList()
    {
      List<Table> tableList = new List<Table>();

      DataTable data = DataProvider.Instance.ExecuteQuery("USP_GetTableList");

      foreach (DataRow item in data.Rows)
      {
        Table table = new Table(item);
        tableList.Add(table);
      }

      return tableList;
    }

    public string GetTableNameByBillID(int id)
    {
      string query = "SELECT t.name FROM dbo.TableFood AS t, dbo.Bill AS b WHERE t.id = b.idTable AND b.id = " + id;
      return (string)DataProvider.Instance.ExecuteScalar(query);
    }

    public bool InsertTable(string name, string status)
    {
      string query = string.Format("INSERT dbo.TableFood ( name, status ) VALUES  ( N'{0}', N'{1}')", name, status);
      int result = DataProvider.Instance.ExecuteNonQuery(query);

      return result > 0;
    }

    public bool DeleteTable(int id)
    {
      BillInfoDAO.Instance.DeleteBillInfoByFoodID(id);
      string query = string.Format("DELETE dbo.TableFood WHERE id = {0}", id);
      int result = DataProvider.Instance.ExecuteNonQuery(query);

      return result > 0;
    }
  }
}
