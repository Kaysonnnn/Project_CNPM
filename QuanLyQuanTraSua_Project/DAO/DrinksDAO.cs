using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
  public class DrinksDAO
    {
    private static DrinksDAO instance;

    public static DrinksDAO Instance
    {
      get { if (instance == null) instance = new DrinksDAO(); return DrinksDAO.instance; }
      private set { DrinksDAO.instance = value; }
    }

    private DrinksDAO() { }

    public List<Drinks> GetFoodByCategoryID(int id)
    {
      List<Drinks> list = new List<Drinks>();

      string query = "SELECT * FROM dbo.Food WHERE idCategory = " + id;

      DataTable data = DataProvider.Instance.ExecuteQuery(query);

      foreach (DataRow item in data.Rows)
      {
        Drinks food = new Drinks(item);
        list.Add(food);
      }

      return list;
    }

    public List<Drinks> GetListFood()
    {
      List<Drinks> list = new List<Drinks>();

      string query = "SELECT * FROM dbo.Food";

      DataTable data = DataProvider.Instance.ExecuteQuery(query);

      foreach (DataRow item in data.Rows)
      {
        Drinks food = new Drinks(item);
        list.Add(food);
      }

      return list;
    }

    public List<Drinks> SearchFoodByName(string name)
    {
      List<Drinks> list = new List<Drinks>();

      string query = string.Format("select * from dbo.Food where dbo.fuConvertToUnsign1(name) like N'%' + dbo.fuConvertToUnsign1(N'{0}') + '%'", name);

      DataTable data = DataProvider.Instance.ExecuteQuery(query);

      foreach (DataRow item in data.Rows)
      {
        Drinks food = new Drinks(item);
        list.Add(food);
      }

      return list;
    }

    public bool InsertFood(string name, int id, float price)
    {
      string query = string.Format("insert dbo.Food (name, idCategory, price) values (N'{0}', {1}, {2})", name, id, price);
      int result = DataProvider.Instance.ExecuteNonQuery(query);

      return result > 0;
    }

    public bool UpdateFood(int idFood, string name, int id, float price)
    {
      string query = string.Format("update dbo.Food set name = N'{0}', idCategory = {1}, price = {2} where id = {3}", name, id, price, idFood);
      int result = DataProvider.Instance.ExecuteNonQuery(query);

      return result > 0;
    }

    public bool DeleteFood(int idFood)
    {
      BillInfoDAO.Instance.DeleteBillInfoByFoodID(idFood);

      string query = string.Format("delete Food where id = {0}", idFood);
      int result = DataProvider.Instance.ExecuteNonQuery(query);

      return result > 0;
    }

    public List<Drinks> GetFoodListByCategoryID(int id)
    {
      List<Drinks> list = new List<Drinks>();

      string query = "SELECT * FROM dbo.Food WHERE idCategory = " + id;

      DataTable data = DataProvider.Instance.ExecuteQuery(query);

      foreach (DataRow item in data.Rows)
      {
        Drinks food = new Drinks(item);
        list.Add(food);
      }

      return list;
    }
  }
}
