using FormApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace FormApp.Controllers
{
	//[Route("")]
	//[Route("/Login")]
	public class AccountController : Controller
	{

		private const string SessionKeyUsername = "Username";

		private readonly IConfiguration ConnectionString;

		public AccountController(IConfiguration _configuration)
		{
			ConnectionString = _configuration;
		}
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]

		public IActionResult Login(LoginModel model)
		{
			if (ModelState.IsValid)
			{
				if (IsValidUser(model.Username, model.Password))
				{
					HttpContext.Session.SetString(SessionKeyUsername, model.Username);
					return RedirectToAction("Index", "Home");
				}
				else
				{
					ViewData["ErrorMessage"] = "Invalid Username or Password";
				}
				ModelState.AddModelError("", "Invalid credentials.");
			}

			return View(model);
		}

		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Login", "Account");
		}

		private bool IsValidUser(string username, string password)
		{
			string connectionStr = ConnectionString.GetConnectionString("sql");
			SqlConnection conn1 = new SqlConnection(connectionStr);
			conn1.Open();
			SqlCommand cmd = conn1.CreateCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "PR_Login_Check";
			cmd.Parameters.AddWithValue("@Username", username);
			cmd.Parameters.AddWithValue("@Password", password);
			bool check = Convert.ToBoolean(cmd.ExecuteScalar());
			Console.WriteLine(check);
			conn1.Close();
			return (check!=null && check);
		}
	}
}
