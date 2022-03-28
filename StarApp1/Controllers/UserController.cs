
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarApp1.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using StarApp1.Services;

namespace StarApp1.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        static string tempPeriodStart = null;
        static string tempProjectId = null;
        public static List<AllowanceDashboardViewModel> listEmployee2 = new List<AllowanceDashboardViewModel>();
        

        public IConfiguration Configuration;
        public IHttpContextAccessor Session;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ILogin _login;
        private readonly IAuthHandler _authHandler;
        private readonly IRegistration _registration;
        private readonly IUserAdmin _userAdmin;
        private readonly IDashboard _dashboard;
        private readonly IPopulate _populate;
        private readonly IDataTransfer _dataTransfer;
        private readonly IButton _button;
        private readonly ITriggerMail _triggerMail;
        private readonly IResetPassword _resetPassword;
        public UserController(IResetPassword resetPassword ,IConfiguration _configuration,IHttpContextAccessor session, IHttpContextAccessor httpContext, IButton button,ILogin login,IAuthHandler authHandler,IRegistration registration,IUserAdmin userAdmin,IDashboard dashboard,IPopulate populate,IDataTransfer dataTransfer, ITriggerMail triggerMail)
        {
            Session = session;
            Configuration = _configuration;
            _httpContext = httpContext;
            _login = login;
            _authHandler = authHandler;
            _registration = registration;
            _userAdmin = userAdmin;
            _dashboard = dashboard;
            _populate = populate;
            _dataTransfer = dataTransfer;
            _button=button;
            _triggerMail=triggerMail;
            _resetPassword=resetPassword;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        
        public IActionResult SignIn()
        {

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(LoginViewModel model)
        {
            string msg = "";
            bool flag = false;
            var user = new LoginViewModel();
            if (!ModelState.IsValid)
            {
                user = _login.CheckLogin( model);

                if (user.UserName!=null /*&& model.UserName == user.UserName && string.Compare(_registration.HashPassword(model.Password), user.Password) == 0*/)
                {

                    HttpContext.Session.SetString("UserName", model.UserName); 
                    HttpContext.Session.SetString("Name", user.Name);
                    HttpContext.Session.SetInt32("RoleId", user.RoleId);
                     

                    int timeout = model.RememberMe ? 525600 : 10; // 525600 min = 1 year
                    AuthenticationProperties authProperties = _authHandler.GetAuthProp(timeout, model);
                    ClaimsIdentity claimsIdentity = _authHandler.GetClaimsIdentity(model);
                
    
                    await _httpContext.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(claimsIdentity),authProperties);

                
               
                    
                        if ( user.StatusId!=3 && user.StatusId!=2)
                        {
                            if (user.RoleId == 1)
                            {
                                flag = true;
                                HttpContext.Session.SetString("Flag", flag.ToString());
                                return RedirectToAction("UserAdmin");

                            }
                            else

                            {
                                flag = true;
                                HttpContext.Session.SetString("Flag", flag.ToString());
                                return RedirectToAction("Dashboard");
                            }
                        }
                        else
                        {
                        msg = "Your request is not granted please Visit later";
                        }
                    
                }
                else
                {
                    msg = "Invalid Credentials";

                }

            }
            HttpContext.Session.SetString("Flag", flag.ToString());
            ViewBag.Message = msg;
            return View();

        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> LogOut()
        {
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("Name");
            Response.Cookies.Delete("star_cookie");
            tempPeriodStart = null;
            tempProjectId = null;

            await _httpContext.HttpContext.SignOutAsync();
            return RedirectToAction("SignIn");
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult SignUp()
        {
            RegisterViewModel model = new RegisterViewModel();
            model.RoleName = _populate.PopulateRole();
            return View(model);

        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult SignUp(RegisterViewModel model)   //User Registration
        {
            bool status = false;
            string msg = "";
            var count = 0;

            // Model Validation 
            if (!ModelState.IsValid)
            {

                model.RoleName = _populate.PopulateRole();
                
                var selectedItem = model.RoleName.Find(p => p.Value == model.RoleId.ToString());
                if (selectedItem != null)
                {
                    selectedItem.Selected = true;

                    #region Check for Existing User
                    if ( _registration.IsEmailExist(model.UserName))
                    {
                        ModelState.AddModelError("EmailExist", "Email already requested for access");
                        return View(model);
                    }
                    #endregion
                    #region Password Hashing
                    model.Password=_registration.HashPassword(model.Password);
                    model.ConfirmPassword=_registration.HashPassword(model.ConfirmPassword);
                    #endregion
                    #region Inserting Data in Db
                    count=_registration.Register(model);
                    #endregion

                }

                if (count == 1)
                {
                    msg = "User registerd successfully :)";
                    status = true;
                }

            }
            
            else
            {
                msg = "Invalid Request";
            }
            ViewBag.Message = msg;
            ViewBag.Status = status;
            return View(model);
        }
        

        [HttpGet]
        public IActionResult UserAdmin(UserAdmin model)       // User Admin Dashboard
        {
            var msg = "";
            if (HttpContext.Session.GetInt32("RoleId") == 1)
            {
                List<UserAdmin> listEmployee = new List<UserAdmin>();
                listEmployee=_userAdmin.DisplayData();

                return View(listEmployee);
            }
            else
            {
                msg = "You are Not Authorised to View This Page";
                ViewBag.Message = msg;
                return View("UserAdmin");
            }
            

        }
        [HttpPost]
        
        public IActionResult UserAdmin()
        {
            List<UserAdmin> listEmployee = new List<UserAdmin>();
            listEmployee = _userAdmin.DisplayData();
        
                return View(listEmployee);
            


        }                   //Admin Dashboard
        [HttpGet]

        public IActionResult Dashboard(AllowanceDashboardViewModel model, string sortValue)
        {
            
            List<AllowanceDashboardViewModel> listEmployee = new List<AllowanceDashboardViewModel>();
           

            model.Project = _populate.PopulateProject();

            model.Period = _populate.PopulatePeriod();
            tempPeriodStart = null;
            tempProjectId = null;

            string querry = _dashboard.SortQuerrySelector(model.ProjectId,model.PeriodStart,sortValue);
            
            listEmployee = _dashboard.DisplayUsersData(querry,model,model.ProjectId,model.PeriodStart);

            ViewBag.listEmployee = listEmployee;
            return View(model);

        }                   //User Dashboard
        [HttpPost]


        public IActionResult Dashboard(AllowanceDashboardViewModel model, String sortValue,int a)
        {
            try
            {


                List<AllowanceDashboardViewModel> listEmployee1 = new List<AllowanceDashboardViewModel>();
                model.Period = _populate.PopulatePeriod();

                model.Project = _populate.PopulateProject();
                if (model.ProjectId != null)
                {
                    tempProjectId = model.ProjectId;
                    tempPeriodStart = model.PeriodStart;
                }


                String querry = _dashboard.SortQuerrySelector(tempProjectId, tempPeriodStart, sortValue);

                listEmployee1 = _dashboard.DisplayUsersData(querry, model, tempProjectId, tempPeriodStart);

                //#region
                //const int pageSize = 10;
                //if (pg < 1)
                //    pg = 1;

                //int recsCount = listEmployee1.Count();
                //var pager = new Pager(recsCount, pg, pageSize);
                //int recSkip = (pg - 1) * pageSize;

                //var data1 = listEmployee1.Skip(recSkip).Take(pager.PageSize).ToList();
                //this.ViewBag.Pager = pager;

                //ViewBag.Data = data1;
                //#endregion

                listEmployee2 = listEmployee1;
                //listEmployee2 = data1;
                ViewBag.listEmployee = listEmployee1;


            }
            catch (Exception ex)
            {

                return RedirectToAction("Dashboard");
            }



            return View(model);
        }

        [AllowAnonymous]
        
        public IActionResult getDecline(string UserName,string name)
        {
            
            try
            {
                _button.DeclineBtn(UserName);
                _triggerMail.SendMail("Your Request has not been Accepted..! <br/> Sorry", "Reg. STARAPP Access Request", UserName, name);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message.ToString();
            }

            return RedirectToAction("UserAdmin");

        }       //Decline of Access
        [AllowAnonymous]
        public IActionResult getApprove(string UserName,string name)
        {
            
            try
            {
                _button.ApproveBtn(UserName);
                _triggerMail.SendMail("Your Request has been APPROVED successfully..!  Thank You", "Reg. STARAPP Access Request", UserName, name);
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message.ToString();
            }
            
            return RedirectToAction("UserAdmin");
        }    //Approval of Access

        public IActionResult ExportToCSV()
        {
            
            StringBuilder builder = _dataTransfer.ExportCsv(listEmployee2);
            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "users.csv");

        }                  //Approval of reports and download csv
        [HttpPost]
        public IActionResult ImportToDb(IFormFile file)
        {
            try
            {
                _dataTransfer.ImportExcel(file);
            }
            catch(Exception ex)
            {
                
                return RedirectToAction("Dashboard");

            }
            return RedirectToAction("Dashboard");



        }//Import data to Db
        [HttpGet]
        public IActionResult UpdateShift(UpdateViewModel model)
        {
            TempData["Hours"] = Convert.ToInt32(model.Hours);
            return View(model);
        }
        [HttpPost]
        public IActionResult UpdateShift(UpdateViewModel model, int LogId )
        {
            int tsd = Convert.ToInt32(TempData["Hours"]) / 8;

            string userName = HttpContext.Session.GetString("UserName");
   
               int asd = Convert.ToInt32(model.updatedASD);
            

               int nsd = Convert.ToInt32(model.updatedNSD);
            
            
           int  utsd = asd + nsd;
            if (tsd!=utsd)
            {
                ViewBag.msg = "Total days Exceed Limit";
                TempData.Remove("Hours");
                return View(model);
            }
            else
            {
                int count = _dashboard.UpdateShiftsData(LogId, asd, nsd, utsd, userName);
                if (count != 0)
                {
                    ViewBag.msg = "Updated";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    ViewBag.msg = "Some Error Occured";
                    return View(model);
                }
            }
            
            
        }
        public IActionResult DeleteAccess(string userName)
        {
            int count = _userAdmin.DeleteAccess(userName);
            if (count!=0)
            { ViewBag.msg = "Deleted Successfully";
            
            }
            else
            {
                ViewBag.msg = "Some Error occured.";
            }
            return RedirectToAction("UserAdmin");
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ChangePassword(PasswordReset model)
        {
            flag = false;
            emailV = false;
            otpV = false;
            
            OTP = 0;
            userName = null;
            ViewBag.flag = flag;
            ViewBag.emailV = emailV;
            ViewBag.otpV = otpV;


            return View(model);
        }
        static int OTP;
        static string userName;
        static bool emailV = false;
        static bool otpV=false;
        static bool flag = false;
        [HttpPost]
        [AllowAnonymous]
        public IActionResult ChangePassword(PasswordReset model,int a)
        {
            ViewBag.flag=flag;
            ViewBag.emailV=emailV;
            ViewBag.msg = "";
            ViewBag.otpV = otpV;
            if (model.UserName!=null)
            {
               if (_registration.IsEmailExist(model.UserName))
                {
                    flag=true;
                    ViewBag.flag = flag;
                    userName =model.UserName;
                    Random random = new Random();
                    OTP = random.Next(100000, 999999);
                    _triggerMail.SendMail($"Your OTP :{OTP}", "Password Reset of StarApp Account", model.UserName, "StarApp User");
                    ViewBag.msg = "OTP is successfully sent..!";
                    emailV=true;
                    ViewBag.emailV = emailV;
                }
                else
                {
                    ViewBag.msg = "Email dosen't Exist..!";
                }
            }
           if (model.Otp!=0)
            {
                if (OTP == model.Otp)
                {
                    otpV = true;
                    ViewBag.otpV = otpV;
                }
                else
                    ViewBag.msg = " Wrong OTP..!";
            }
           if (model.Password!=null)
            {
                if(_resetPassword.ChangePass(userName, model.Password))
                {
                    ViewBag.msg = "Password Changed Successfully";
                    RedirectToAction("SignIn");
                }
                else
                {
                    ViewBag.msg = "Some Error Occured";
                }
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult Update(UserAdmin model)
        {
            
            model.RoleName = _populate.PopulateRole();
            TempData["username"] = model.UserName;
            return View(model);
            
        }
        [HttpPost]
        public IActionResult Update(UserAdmin model,int a)
        {
            model.RoleName = _populate.PopulateRole();

            int count = _userAdmin.UpdateAccess(TempData["username"].ToString(), model.RoleId);
            if (count != 0)
            {
                TempData.Clear();
                return RedirectToAction("UserAdmin");
            }
                
            else
            {
                ViewBag.msg = "Some Error Occured";
                return View();
            }
                
            

        }
        
    }

}
