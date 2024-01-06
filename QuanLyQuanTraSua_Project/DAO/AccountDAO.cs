using QuanLyQuanCafe.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyQuanCafe.DAO
{
  public class AccountDAO
  {
    private static AccountDAO instance;

    public static AccountDAO Instance
    {
      get { if (instance == null) instance = new AccountDAO(); return instance; }
      private set { instance = value; }
    }

    private AccountDAO() { }

    public bool Login(string username, string password)
    {
      byte[] temp = ASCIIEncoding.ASCII.GetBytes(password);
      byte[] hasData = new MD5CryptoServiceProvider().ComputeHash(temp);

      string hasPass = "";

      foreach (byte item in hasData)
      {
        hasPass += item;
      }

      //var list = hasData.ToString();
      //list.Reverse();

      string query = "USP_Login @username , @password";
      
      DataTable result = DataProvider.Instance.ExecuteQuery(query, new object[] {username, hasPass /*list*/});
      
      return result.Rows.Count > 0;
    }

    public bool UpdateAccount(string userName, string displayName, string password, string newpass)
    {
      int result = DataProvider.Instance.ExecuteNonQuery("EXEC USP_UpdateAccount @userName , @displayName , @password , @newPassword", new object[] { userName, displayName, password, newpass });

      return result > 0;
    }

    public DataTable GetListAccount()
    {
      return DataProvider.Instance.ExecuteQuery("select Username, DisplayName, Type from dbo.Account");
    }

    public Account GetAccountByUserName(string userName)
    {
      DataTable data = DataProvider.Instance.ExecuteQuery("SELECT * FROM dbo.Account WHERE userName = '" + userName + "'");

      foreach (DataRow item in data.Rows)
      {
        return new Account(item);
      }

      return null;
    }

    public bool InsertAccount(string name, string displayName, int type)
    {
      string query = string.Format("insert dbo.Account (Username, DisplayName, Type) values (N'{0}', N'{1}', {2})", name, displayName, type);
      int result = DataProvider.Instance.ExecuteNonQuery(query);

      return result > 0;
    }

    public bool UpdateAccount(string username, string displayName, int type)
    {
      string query = string.Format("update dbo.Account set DisplayName = N'{1}', Type = {2} where Username = N'{0}'", username, displayName, type);
      int result = DataProvider.Instance.ExecuteNonQuery(query);

      return result > 0;
    }

    public bool DeleteAccount(string username)
    {
      string query = string.Format("delete Account where Username = N'{0}'", username);
      int result = DataProvider.Instance.ExecuteNonQuery(query);

      return result > 0;
    }

    public bool ResetPassword(string username)
    {
      string query = string.Format("update dbo.Account set Password = N'2251022057731868917119086224872421513662' where Username = N'{0}'", username);
      int result = DataProvider.Instance.ExecuteNonQuery(query);

      return result > 0;
    }
  }
}
