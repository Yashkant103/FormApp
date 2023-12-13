using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using FormApp.Areas.LOC_Country.Models;
using FormApp.Areas.LOC_State.Models;

namespace FormApp.Areas.LOC_State.Controllers
{
	[Area("LOC_State")]
	[Route("LOC_State/[Controller]/[Action]")]
	public class LOC_StateController : Controller
    {
        private readonly IConfiguration Configuration;

        public LOC_StateController(IConfiguration _Configuration)
        {
            Configuration = _Configuration;
        }
        public IActionResult Index(string? StateName,string? StateCode, int? CountryID,bool filter = false)
        {
            Country_DD();

			string connectionStr = Configuration.GetConnectionString("sql");
            DataTable dt = new DataTable();
            SqlConnection conn = new SqlConnection(connectionStr);
            conn.Open();
            SqlCommand objCmd = conn.CreateCommand();
            objCmd.CommandType = CommandType.StoredProcedure;
            if (Convert.ToBoolean(filter))
            {
                objCmd.CommandText = "PR_State_Search";
                objCmd.Parameters.AddWithValue("@StateName", StateName);
                objCmd.Parameters.AddWithValue("@StateCode", StateCode);
                objCmd.Parameters.AddWithValue("@CountryID", CountryID);
			}
            else
            {
                objCmd.CommandText = "PR_State_SelectAll";
            }
            SqlDataReader objSDR = objCmd.ExecuteReader();
            dt.Load(objSDR);
            conn.Close();
            return View(dt);
        }
        public IActionResult AddEdit(int? StateID)
        {
            Country_DD();

			if (StateID == null)
            {
                @ViewData["addEdit"] = "ADD";
                return View("LOC_State_AddEdit");
            }
            else
            {
                @ViewData["addEdit"] = "EDIT";
                string connectionStr2 = Configuration.GetConnectionString("sql");
                SqlConnection conn1 = new SqlConnection(connectionStr2);
                conn1.Open();
                SqlCommand cmd = conn1.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "PR_State_SelectByPK";
                cmd.Parameters.AddWithValue("@StateID", StateID);
                SqlDataReader objSDR = cmd.ExecuteReader();
                LOC_StateModel sModel = new LOC_StateModel();
                if (objSDR.HasRows)
                {
                    while (objSDR.Read())
                    {
                        sModel.StateID = Convert.ToInt32(objSDR["StateID"]);
                        sModel.StateName = objSDR["StateName"].ToString();
                        sModel.CountryID = Convert.ToInt32(objSDR["CountryID"]);
                        sModel.StateCode = objSDR["StateCode"].ToString();
                    }
                }
                conn1.Close();
                return View("LOC_State_AddEdit", sModel);
            }
        }


        public IActionResult AddEditSave(LOC_StateModel sModel)
        {
            string connectionStr = Configuration.GetConnectionString("sql");
            SqlConnection conn = new SqlConnection(connectionStr);
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            if (sModel.StateID == null)
            {
                cmd.CommandText = "PR_State_Insert";
            }
            else
            {
                cmd.CommandText = "PR_State_Update";
                cmd.Parameters.AddWithValue("@StateID", sModel.StateID);
            }
            cmd.Parameters.AddWithValue("@StateName", sModel.StateName);
            cmd.Parameters.AddWithValue("@CountryID", sModel.CountryID);
            cmd.Parameters.AddWithValue("@StateCode", sModel.StateCode);
            cmd.ExecuteNonQuery();
            conn.Close();
            return RedirectToAction("Index");
        }
        public IActionResult DeleteState(int StateID)
        {
            string connectionStr = Configuration.GetConnectionString("sql");
            SqlConnection conn = new SqlConnection(connectionStr);
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_State_Delete";
            cmd.Parameters.AddWithValue("@StateID", StateID);
            cmd.ExecuteNonQuery();
            conn.Close();
            return RedirectToAction("Index");
        }


		#region Country_DD
        public void Country_DD()
        {

			List<LOC_Country_DropDown> list = new List<LOC_Country_DropDown>();
			string connectionStr = Configuration.GetConnectionString("sql");
			SqlConnection conn = new SqlConnection(connectionStr);
			conn.Open();
			SqlCommand ddcmd = conn.CreateCommand();
			ddcmd.CommandType = CommandType.StoredProcedure;
			ddcmd.CommandText = "PR_Country_DropDown";
			SqlDataReader obj = ddcmd.ExecuteReader();
			if (obj.HasRows)
			{
				while (obj.Read())
				{
					LOC_Country_DropDown cModel = new LOC_Country_DropDown();
					cModel.CountryID = Convert.ToInt32(obj["CountryID"]);
					cModel.CountryName = obj["CountryName"].ToString();
					list.Add(cModel);
				}
				ViewBag.CountryList = list;
			}
			conn.Close();

		}
		#endregion
	}
}
