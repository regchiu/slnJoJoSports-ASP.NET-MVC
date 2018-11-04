using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using prjJoJoSports.Models;

namespace prjJoJoSports.Controllers
{
    public class HomeController : Controller
    {
        //LocalDB
        //建立可存取dbJoJoSports資料庫的 dbJoJoSportsEntities類別物件db
        //dbJoJoSportsEntities db = new dbJoJoSportsEntities();

        //本地資料庫
        prjJoJoSports_dbEntities db = new prjJoJoSports_dbEntities();
  
        // GET: Home
        public ActionResult Index()
        {
            //將所有過了活動結束時間的新團變更為舊團(本地)
            foreach (var item in db.t開團.Where(m => m.f活動結束時間 < DateTime.Now))
            {
                item.f狀態 = "關團";
            }
            db.SaveChanges();

            ViewModel vm = new ViewModel()
            {
                //未過報名截止時間(本地)
                開團 = db.t開團.Where(m => m.f狀態 == "開團" && m.f報名截止時間 > DateTime.Now).OrderByDescending(m => m.f貼文發布時間).Take(6).ToList(),
                舊團 = db.t開團.Join(db.t開團評價, m => m.f開團編號, n => n.f開團編號, (m, n) => new OldTeamListViewModel
                {
                    開團編號 = m.f開團編號,
                    團長名稱 = m.f團長名稱,
                    運動項目名稱 = m.f運動項目名稱,
                    標題 = m.f標題,
                    活動結束時間 = m.f活動結束時間,
                    狀態 = m.f狀態,
                    活動地點 = m.f活動地點,
                    平均評分 = db.t開團評價.Where(t => t.f開團編號 == m.f開團編號 && t.f評分 != null).Select(t => t.f評分).Average()
                }).Where(mn => mn.狀態 == "關團").Distinct().OrderByDescending(m => m.活動結束時間).Take(6).ToList()
            };
            //會員未登入後顯示的畫面(Session)
            if (Session["Member"] == null)
            {
                return View("Index", vm);
            }
            //會員登入後顯示的畫面
            return View("Index", "_LayoutMember", vm);
        }

        public JsonResult GetNotificationContacts()
        {
            var notificationRegisterTime = Session["LastUpdated"] != null ? Convert.ToDateTime(Session["LastUpdated"]) : DateTime.Now;
            NotificationComponent NC = new NotificationComponent();
            var list = NC.GetContacts(notificationRegisterTime);
            //update session here for get only new added contacts (notification)
            Session["LastUpdate"] = DateTime.Now;
            return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //註冊會員
        //GET:Home/Register
        public ActionResult Register()
        {
            //會員未登入後顯示的畫面(Session)
            if (Session["Member"] == null)
            {
                return View("Register");
            }
            //會員登入後顯示的畫面
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Register(t會員資料 pMember)
        {

            //若沒有通過驗證則顯示目前的畫面
            if (ModelState.IsValid == false)
            {
                return View();
            }

            //依帳號(信箱)取得會員並指定給member
            var member = db.t會員資料.Where(m => m.f帳號 == pMember.f帳號).FirstOrDefault();

            //依用戶名稱取得會員並指定給membername
            var membername = db.t會員資料.Where(m => m.f名稱 == pMember.f名稱).FirstOrDefault();

            //會員帳號與用戶名稱均未註冊
            if (member == null && membername == null)
            {
                //將會員紀錄新增到t會員資料
                db.t會員資料.Add(pMember);
                db.SaveChanges();

                //在會員記錄下新增t會員狀態
                if (pMember.f性別 == "男")
                {
                    t會員狀態 memberstatus = new t會員狀態();
                    memberstatus.f帳號 = pMember.f帳號;
                    memberstatus.f名稱 = pMember.f名稱;
                    memberstatus.f個人簡介 = "這個人很懶什麼都沒留下";
                    memberstatus.f大頭貼 = "defaultmale.jpg";
                    db.t會員狀態.Add(memberstatus);
                    db.SaveChanges();
                }

                if (pMember.f性別 == "女")
                {
                    t會員狀態 memberstatus = new t會員狀態();
                    memberstatus.f帳號 = pMember.f帳號;
                    memberstatus.f名稱 = pMember.f名稱;
                    memberstatus.f個人簡介 = "這個人很懶什麼都沒留下";
                    memberstatus.f大頭貼 = "defaultfemale.jpg";
                    db.t會員狀態.Add(memberstatus);
                    db.SaveChanges();
                }
                //顯示註冊成功並跳轉頁面至登入畫面
                var alert = string.Format("<script>alert('註冊成功!');location.href='{0}'</script>", Url.Action("Login"));
                return Content(alert, "text/html");
            }
            if (member != null)
            {
                ViewBag.Message = "此帳號已有人使用,註冊失敗";
            }
            if (membername != null)
            {
                ViewBag.Message = "此用戶名稱已有人使用,註冊失敗";
            }

            return View();
        }

        //會員登入
        //GET:Home/Login
        public ActionResult Login()
        {
            //會員未登入後顯示的畫面(Session)
            if (Session["Member"] == null)
            {
                return View("Login");
            }
            //會員登入後顯示的畫面
            return RedirectToAction("Index");
        }
        //POST:Home/Login
        [HttpPost]
        public ActionResult Login(string f帳號, string f密碼)
        {

            var member = db.t會員資料.Where(m => m.f帳號 == f帳號 && m.f密碼 == f密碼).FirstOrDefault();

            var profilephoto = db.t會員狀態.Where(m => m.f帳號 == f帳號).FirstOrDefault();
            //帳號密碼驗證失敗
            if (member == null)
            {
                ViewBag.Message = "帳密錯誤，登入失敗";
                return View();
            }
            //使用Session變數紀錄
            Session["Name"] = member.f名稱;
            Session["Birthday"] = DateTime.Parse(member.f生日.ToString()).ToShortDateString();
            Session["Gender"] = member.f性別;
            Session["Email"] = member.f帳號;
            Session["Phone"] = member.f電話;
            Session["Photo"] = profilephoto.f大頭貼;
            //使用Session變數紀錄登入的會員物件
            Session["Member"] = member;

            //執行Home控制器Index動作方法
            return RedirectToAction("Index");
        }

        //會員登出
        //GET:Home/Logout
        public ActionResult Logout()
        {
            //清除Session
            Session.Clear();
            return RedirectToAction("Index");
        }

        //個人狀態頁面
        //GET:Home/Profile_Index
        public ActionResult Profile_Index()
        {
            //會員未登入後顯示的畫面(Session)
            if (Session["Member"] == null)
            {
                return RedirectToAction("Index");
            }
            //取得登入會員的帳號並指定給fUserId
            string f帳號 = (Session["Member"] as t會員資料).f帳號;
            var profile = db.t會員狀態.Where(m => m.f帳號 == f帳號).FirstOrDefault();
            //重新讀取登入會員的大頭貼
            var profilephoto = db.t會員狀態.Where(m => m.f帳號 == f帳號).FirstOrDefault();
            Session["Photo"] = profilephoto.f大頭貼;
            //會員登入後顯示的畫面
            return View("Profile_Index", "_LayoutMember", profile);
        }

        //編輯個人狀態頁面
        //GET:Home/Profile_Edit
        public ActionResult Profile_Edit()
        {

            //會員未登入後顯示的畫面(Session)
            if (Session["Member"] == null)
            {
                return RedirectToAction("Index");
            }
            string f帳號 = (Session["Member"] as t會員資料).f帳號;
            var profile = db.t會員狀態.Where(m => m.f帳號 == f帳號).FirstOrDefault();
            //會員登入後顯示的畫面
            return View("Profile_Edit", "_LayoutMember", profile);
        }
        [HttpPost]
        public ActionResult Profile_Edit(HttpPostedFileBase photo, string f個人簡介)
        {
            string f帳號 = (Session["Member"] as t會員資料).f帳號;
            var profile = db.t會員狀態.Where(m => m.f帳號 == f帳號).FirstOrDefault();
            profile.f個人簡介 = f個人簡介;
            //檔案上傳
            if (photo != null)
            {
                if (photo.ContentLength > 0)
                {
                    //取得圖檔名稱並在檔名前加上全域唯一識別項
                    //var fImg =Guid.NewGuid().ToString()+ Path.GetFileName(photo.FileName);
                    //取得圖檔名稱並在檔名前加上會員帳號
                    var fImg = profile.f帳號 + Path.GetFileName(photo.FileName);
                    var path = Path.Combine(Server.MapPath("~/Profile_picture/"), fImg);
                    photo.SaveAs(path);
                    profile.f大頭貼 = fImg;
                }
            }
            db.SaveChanges();
            return RedirectToAction("Profile_Index");
        }

        //查看他人頁面
        //GET:Home/MemberPage
        public ActionResult MemberPage(string f會員名稱)
        {
            //會員未登入後顯示的畫面(Session)
            if (Session["Member"] == null)
            {
                return RedirectToAction("Index");
            }

            ViewModel vm = new ViewModel()
            {
                會員狀態 = db.t會員狀態.Where(m => m.f名稱 == f會員名稱).ToList(),

                //本機
                開團 = db.t開團.Where(m => m.f團長名稱 == f會員名稱 && m.f狀態 == "開團" && m.f報名截止時間 > DateTime.Now).ToList(),

                舊團 = db.t開團.Join(db.t開團評價, m => m.f開團編號, n => n.f開團編號, (m, n) => new OldTeamListViewModel
                {
                    開團編號 = m.f開團編號,
                    團長名稱 = m.f團長名稱,
                    運動項目名稱 = m.f運動項目名稱,
                    標題 = m.f標題,
                    活動結束時間 = m.f活動結束時間,
                    狀態 = m.f狀態,
                    活動地點 = m.f活動地點,
                    是否出席 = db.t參加名單.Where(t => t.f會員名稱 == f會員名稱 && t.f開團編號 == m.f開團編號 && t.f狀態 == "參加" && t.f是否出席 == "出席").Count(),
                    平均評分 = db.t開團評價.Where(t => t.f開團編號 == m.f開團編號 && t.f評分 != null).Select(t => t.f評分).Average()
                }).Where(mn => mn.狀態 == "關團" && mn.團長名稱 == f會員名稱).Distinct().OrderByDescending(m => m.活動結束時間).ToList()
            };
            string f名稱 = (Session["Member"] as t會員資料).f名稱;
            var list = db.t訂閱名單.Where(m => m.f會員名稱 == f名稱 && m.f訂閱會員名稱 == f會員名稱).FirstOrDefault();
            if (list != null)
            {
                ViewBag.狀態 = "已訂閱";
                ViewBag.訂閱編號 = list.f訂閱名單編號;
            }
            else
            {
                ViewBag.狀態 = "未訂閱";
            }

            //會員登入後顯示的畫面
            return View("MemberPage", "_LayoutMember", vm);
        }



        //尋找新團
        //GET:Home/NewTeamList
        public ActionResult NewTeamList(string search縣市, string search運動)
        {
            //會員未登入後顯示的畫面(Session)
            if (Session["Member"] == null)
            {
                return RedirectToAction("Index");
            }
            //為了放入@Html.DropDownList
            var county = db.t縣市名稱.Select(m => m.f縣市名稱).ToList();
            ViewBag.CountyName = new SelectList(county);
            //為了放入@Html.DropDownList
            var sporstname = db.t運動項目.Select(m => m.f運動名稱).ToList();
            ViewBag.SportsName = new SelectList(sporstname);


            var newteam = db.t開團.Where(m => m.f狀態 == "開團" && m.f報名截止時間 > DateTime.Now).ToList();

            if (!String.IsNullOrEmpty(search縣市))
            {
                //本機
                newteam = db.t開團.Where(m => m.f狀態 == "開團" && m.f報名截止時間 > DateTime.Now && m.f活動地點.Contains(search縣市)).ToList();


            }
            if (!String.IsNullOrEmpty(search運動))
            {
                //本機
                newteam = db.t開團.Where(m => m.f狀態 == "開團" && m.f報名截止時間 > DateTime.Now && m.f運動項目名稱 == search運動).ToList();

            }
            if (!String.IsNullOrEmpty(search縣市) && !String.IsNullOrEmpty(search運動))
            {
                //本機
                newteam = db.t開團.Where(m => m.f狀態 == "開團" && m.f報名截止時間 > DateTime.Now && m.f活動地點.Contains(search縣市) && m.f運動項目名稱 == search運動).ToList();
            }

            //會員登入後顯示的畫面
            return View("NewTeamList", "_LayoutMember", newteam);
        }

        //創建新團
        //GET:Home/CreateNewTeam
        public ActionResult CreateNewTeam()
        {
            ViewModel vm = new ViewModel()
            {
                運動項目 = db.t運動項目.ToList()
            };
            //會員未登入後顯示的畫面(Session)
            if (Session["Member"] == null)
            {
                return RedirectToAction("Index");

            }
            //會員登入後顯示的畫面
            return View("CreateNewTeam", "_LayoutMember", vm);
        }
        [HttpPost]
        public ActionResult CreateNewTeam(string f運動項目名稱, string f標題, string f備註, DateTime f活動開始時間, DateTime f活動結束時間, DateTime f報名截止時間, int f人數上限, string f活動地點, decimal f緯度, decimal f經度)
        {
            string alert = string.Format("<script>alert('開團成功!');location.href='{0}'</script>", Url.Action("MyTeam"));//alert 預設為開團成功
            string f會員名稱 = (Session["Member"] as t會員資料).f名稱;
            //找出此會員參加團的開團編號
            var idlist = db.t參加名單.Where(m => m.f會員名稱 == f會員名稱 && m.f狀態 == "參加").Select(m => m.f開團編號).ToList();
            //判斷要創立的團活動時間是否與目前參加的團(創立的團)活動時間重疊
            var myteam = db.t開團.Where(m => idlist.Contains(m.f開團編號) && ((m.f活動開始時間 >= f活動開始時間 && m.f活動開始時間 <= f活動結束時間) || (m.f活動結束時間 >= f活動開始時間 && m.f活動結束時間 <= f活動結束時間) || (m.f活動開始時間 <= f活動開始時間 && f活動開始時間 <= m.f活動結束時間 && m.f活動開始時間 <= f活動結束時間 && f活動結束時間 <= m.f活動結束時間) || (f活動開始時間 <= m.f活動開始時間 && f活動結束時間 >= m.f活動結束時間))).FirstOrDefault();

            if (myteam == null)
            {
                t開團 newteam = new t開團();
                newteam.f團長名稱 = f會員名稱;
                newteam.f標題 = f標題;
                newteam.f備註 = f備註;
                newteam.f運動項目名稱 = f運動項目名稱;
                newteam.f貼文發布時間 = DateTime.Now;
                newteam.f活動開始時間 = f活動開始時間;
                newteam.f活動結束時間 = f活動結束時間;
                newteam.f報名截止時間 = f報名截止時間;
                newteam.f人數上限 = f人數上限;
                newteam.f活動地點 = f活動地點;
                newteam.f緯度 = f緯度;
                newteam.f經度 = f經度;
                newteam.f狀態 = "開團";
                db.t開團.Add(newteam);
                db.SaveChanges();

                //參加名單
                t參加名單 crewlist = new t參加名單();
                crewlist.f開團編號 = newteam.f開團編號;
                crewlist.f會員名稱 = newteam.f團長名稱;
                crewlist.f狀態 = "參加";
                db.t參加名單.Add(crewlist);
                db.SaveChanges();

                //開團評價
                t開團評價 review = new t開團評價();
                review.f開團編號 = newteam.f開團編號;
                db.t開團評價.Add(review);
                db.SaveChanges();
            }
            else
            {
                alert = string.Format("<script>alert('抱歉!您在這活動時間內已有創立其它項目!');location.href='{0}'</script>", Url.Action("MyTeam"));
            }
            return Content(alert, "text/html");
        }
        //關團
        //GET:Home/CloseNewTeam
        public ActionResult CloseNewTeam(int? f開團編號)
        {
            string alert = string.Format("<script>alert('關團成功!');location.href='{0}'</script>", Url.Action("MyTeam"));
            t開團 newteam = db.t開團.Where(m => m.f開團編號 == f開團編號).FirstOrDefault();
            db.t開團.Remove(newteam);
            db.SaveChanges();
            return Content(alert, "text/html");
            //return RedirectToAction("MyTeam");
        }


        //新團詳細內容
        //GET:Home/NewTeamDetail
        public ActionResult NewTeamDetail(int? f開團編號)
        {
            //會員未登入後顯示的畫面(Session)
            if (Session["Member"] == null)
            {
                return RedirectToAction("Index");              
            }
            ViewModel vm = new ViewModel()
            {
                開團 = db.t開團.Where(m => m.f開團編號 == f開團編號 && m.f狀態 == "開團").ToList(),
                開團留言板 = db.t開團留言板.Join(db.t會員狀態, m => m.f會員名稱, n => n.f名稱, (m, n) => new CommentViewModel
                {
                    開團留言板編號 = m.f開團留言板編號,
                    開團編號 = m.f開團編號,
                    會員大頭貼 = n.f大頭貼,
                    會員名稱 = m.f會員名稱,
                    留言內容 = m.f留言內容,
                    發布時間 = m.f發布時間,
                }).Where(mn => mn.開團編號 == f開團編號).OrderBy(mn=>mn.發布時間).ToList(),
                留言板回覆 = db.t留言板回覆.Join(db.t會員狀態, m => m.f會員名稱, n => n.f名稱, (m, n) => new ReplyViewModel
                {
                    留言板回覆編號 = m.f留言板回覆編號,
                    開團留言板編號 = m.f開團留言板編號,
                    會員大頭貼 = n.f大頭貼,
                    會員名稱 = m.f會員名稱,
                    回覆內容 = m.f回覆內容,
                    發布時間 = m.f發布時間
                }).OrderBy(mn=>mn.發布時間).ToList()
            };
            var newteam = db.t開團.Where(m => m.f開團編號 == f開團編號 && m.f狀態 == "開團").FirstOrDefault();

            if (newteam == null)
            {
                return RedirectToAction("NewTeamList");
            }

            //判斷是否參加
            string f會員名稱 = (Session["Member"] as t會員資料).f名稱;
            var member = db.t參加名單.Where(m => m.f開團編號 == f開團編號 && m.f會員名稱 == f會員名稱 && m.f狀態 == "參加").FirstOrDefault();
            if (member == null)
            {
                ViewBag.Status = "未參加";
            }
            if (member != null)
            {
                ViewBag.Status = "已參加";
            }


            //計算目前參加人數並判斷是否滿人
            var crewcount = db.t參加名單.Where(m => m.f開團編號 == f開團編號 && m.f狀態 == "參加").Count();
            ViewBag.CrewCount = crewcount;
            if (crewcount < newteam.f人數上限)
            {
                ViewBag.Full = "人數未滿";
            }
            if (crewcount == newteam.f人數上限)
            {
                ViewBag.Full = "人數已滿";
            }

            if (DateTime.Now >= newteam.f報名截止時間 && DateTime.Now < newteam.f活動開始時間)
            {
                ViewBag.End = "報名截止";
            }
            if (DateTime.Now >= newteam.f活動開始時間)
            {
                ViewBag.Clock = "活動進行中";
            }


            //參加名單編號
            var 參加名單編號 = db.t參加名單.Where(m => m.f開團編號 == f開團編號 && m.f會員名稱 == f會員名稱).FirstOrDefault();
            if (參加名單編號 != null)
            {
                ViewBag.參加名單編號 = 參加名單編號.f參加名單編號;
            }

            //會員登入後顯示的畫面
            return View("NewTeamDetail", "_LayoutMember", vm);
        }

        //參加
        //GET:Home/Join
        public ActionResult Join(int? f參加名單編號, int? f開團編號)
        {
            string f會員名稱 = (Session["Member"] as t會員資料).f名稱;
            var member = db.t參加名單.Where(m => m.f開團編號 == f開團編號 && m.f會員名稱 == f會員名稱).FirstOrDefault();

            string alert = string.Format("<script>alert('參加成功!');location.href='{0}'</script>", Url.Action("MyJoinTeam"));//alert 預設為參加成功
            var crewcount = db.t參加名單.Where(m => m.f開團編號 == f開團編號 && m.f狀態 == "參加").Count();//參加人數
            var newteam = db.t開團.Where(m => m.f開團編號 == f開團編號).FirstOrDefault();
            t參加名單 crewlist = new t參加名單();

            //判斷人數是否已滿
            if (crewcount < newteam.f人數上限)
            {

                //找出此會員參加團的開團編號
                var idlist = db.t參加名單.Where(m => m.f會員名稱 == f會員名稱 && m.f狀態 == "參加").Select(m => m.f開團編號).ToList();
                //判斷要參加的團活動時間是否與正在參加的團活動時間重疊
                var myjointeam = db.t開團.Where(m => idlist.Contains(m.f開團編號) && ((m.f活動開始時間 >= newteam.f活動開始時間 && m.f活動開始時間 <= newteam.f活動結束時間) || (m.f活動結束時間 >= newteam.f活動開始時間 && m.f活動結束時間 <= newteam.f活動結束時間) || (m.f活動開始時間 <= newteam.f活動開始時間 && newteam.f活動開始時間 <= m.f活動結束時間 && m.f活動開始時間 <= newteam.f活動結束時間 && newteam.f活動結束時間 <= m.f活動結束時間) || (newteam.f活動開始時間 <= m.f活動開始時間 && newteam.f活動結束時間 >= m.f活動結束時間))).FirstOrDefault();

                if (myjointeam == null)
                {
                    //第一次參加就新增資料
                    if (member == null)
                    {
                        crewlist.f開團編號 = f開團編號;
                        crewlist.f會員名稱 = f會員名稱;
                        crewlist.f狀態 = "參加";
                        db.t參加名單.Add(crewlist);
                        db.SaveChanges();

                    }
                    //已在名單內就修改狀態
                    if (member != null)
                    {
                        crewlist = db.t參加名單.Where(m => m.f參加名單編號 == f參加名單編號).FirstOrDefault();
                        crewlist.f狀態 = "參加";
                        db.SaveChanges();
                    }
                }
                else
                {
                    alert = string.Format("<script>alert('抱歉!您在這段活動時間內已有參與活動或創立活動!');location.href='{0}'</script>", Url.Action("NewTeamlist"));
                }

            }
            else
            {
                alert = string.Format("<script>alert('抱歉!人數已滿!');location.href='{0}'</script>", Url.Action("NewTeamlist"));
            }

            return Content(alert, "text/html");
        }
        //退出
        //GET:Home/Leave
        public ActionResult Leave(int? f參加名單編號, int? f開團編號)
        {
            string alert = string.Format("<script>alert('退出成功!');location.href='{0}'</script>", Url.Action("NewTeamlist"));
            t參加名單 crewlist = db.t參加名單.Where(m => m.f參加名單編號 == f參加名單編號).FirstOrDefault();
            crewlist.f狀態 = "退出";
            db.SaveChanges();
            return Content(alert, "text/html");
        }

        //留言板
        //POST:Home/Comment
        [HttpPost]
        public ActionResult Comment(int f開團編號, string f留言內容)
        {
            string f會員名稱 = (Session["Member"] as t會員資料).f名稱;
            t開團留言板 messageboard = new t開團留言板();
            messageboard.f開團編號 = f開團編號;
            messageboard.f會員名稱 = f會員名稱;
            messageboard.f留言內容 = f留言內容;
            messageboard.f發布時間 = DateTime.Now;
            db.t開團留言板.Add(messageboard);
            db.SaveChanges();
            return Redirect("NewTeamDetail?f開團編號=" + f開團編號);

        }

        //留言板回覆
        //GET:Home/Reply
        [HttpPost]
        public ActionResult Reply(int f開團編號, int f開團留言板編號, string f回覆內容)
        {
            string f會員名稱 = (Session["Member"] as t會員資料).f名稱;
            t留言板回覆 repalyInfo = new t留言板回覆();
            repalyInfo.f開團留言板編號 = f開團留言板編號;
            repalyInfo.f會員名稱 = f會員名稱;
            repalyInfo.f回覆內容 = f回覆內容;
            repalyInfo.f發布時間 = DateTime.Now;
            db.t留言板回覆.Add(repalyInfo);
            db.SaveChanges();

            return Redirect("NewTeamDetail?f開團編號=" + f開團編號);
        }

        //找舊團
        //GET:Home/OldTeamList
        public ActionResult OldTeamList(string search縣市, string search運動)
        {
            //會員未登入後顯示的畫面(Session)
            if (Session["Member"] == null)
            {
                return RedirectToAction("Index");
            }

            //為了放入@Html.DropDownList
            var county = db.t縣市名稱.Select(m => m.f縣市名稱).ToList();
            ViewBag.CountyName = new SelectList(county);
            //為了放入@Html.DropDownList
            var sporstname = db.t運動項目.Select(m => m.f運動名稱).ToList();
            ViewBag.SportsName = new SelectList(sporstname);



            var oldteam = db.t開團.Join(db.t開團評價, m => m.f開團編號, n => n.f開團編號, (m, n) => new OldTeamListViewModel
            {
                開團編號 = m.f開團編號,
                團長名稱 = m.f團長名稱,
                運動項目名稱 = m.f運動項目名稱,
                標題 = m.f標題,
                活動結束時間 = m.f活動結束時間,
                狀態 = m.f狀態,
                活動地點 = m.f活動地點,
                平均評分 = db.t開團評價.Where(t => t.f開團編號 == m.f開團編號).Select(t => t.f評分).Average()
            }).Where(mn => mn.狀態 == "關團").Distinct().ToList();



            if (!String.IsNullOrEmpty(search縣市))
            {
                oldteam = db.t開團.Join(db.t開團評價, m => m.f開團編號, n => n.f開團編號, (m, n) => new OldTeamListViewModel
                {
                    開團編號 = m.f開團編號,
                    團長名稱 = m.f團長名稱,
                    運動項目名稱 = m.f運動項目名稱,
                    標題 = m.f標題,
                    活動結束時間 = m.f活動結束時間,
                    狀態 = m.f狀態,
                    活動地點 = m.f活動地點,
                    平均評分 = db.t開團評價.Where(t => t.f開團編號 == m.f開團編號).Select(t => t.f評分).Average()
                }).Where(mn => mn.狀態 == "關團" && mn.活動地點.Contains(search縣市)).Distinct().ToList();
            }
            if (!String.IsNullOrEmpty(search運動))
            {
                oldteam = db.t開團.Join(db.t開團評價, m => m.f開團編號, n => n.f開團編號, (m, n) => new OldTeamListViewModel
                {
                    開團編號 = m.f開團編號,
                    團長名稱 = m.f團長名稱,
                    運動項目名稱 = m.f運動項目名稱,
                    標題 = m.f標題,
                    活動結束時間 = m.f活動結束時間,
                    狀態 = m.f狀態,
                    活動地點 = m.f活動地點,
                    平均評分 = db.t開團評價.Where(t => t.f開團編號 == m.f開團編號).Select(t => t.f評分).Average()
                }).Where(mn => mn.狀態 == "關團" && mn.運動項目名稱 == search運動).Distinct().ToList();
            }
            if (!String.IsNullOrEmpty(search縣市) && !String.IsNullOrEmpty(search運動))
            {
                oldteam = db.t開團.Join(db.t開團評價, m => m.f開團編號, n => n.f開團編號, (m, n) => new OldTeamListViewModel
                {
                    開團編號 = m.f開團編號,
                    團長名稱 = m.f團長名稱,
                    運動項目名稱 = m.f運動項目名稱,
                    標題 = m.f標題,
                    活動結束時間 = m.f活動結束時間,
                    狀態 = m.f狀態,
                    活動地點 = m.f活動地點,
                    平均評分 = db.t開團評價.Where(t => t.f開團編號 == m.f開團編號).Select(t => t.f評分).Average()
                }).Where(mn => mn.狀態 == "關團" && mn.活動地點.Contains(search縣市) && mn.運動項目名稱 == search運動).Distinct().ToList();
            }


            //會員登入後顯示的畫面
            return View("OldTeamList", "_LayoutMember", oldteam);
        }


        //舊團詳細(評價)
        //GET:Home/OldTeamDetail
        public ActionResult OldTeamDetail(int? f開團編號)
        {
            ////會員未登入後顯示的畫面(Session)
            if (Session["Member"] == null)
            {
                return RedirectToAction("Index");
            }
            //會員登入後顯示的畫面
            ViewModel vm = new ViewModel()
            {
                開團 = db.t開團.Where(m => m.f開團編號 == f開團編號).ToList(),
                開團評價 = db.t開團評價.Join(db.t會員狀態, m => m.f會員名稱, n => n.f名稱, (m, n) => new OldTeamDetailViewModel
                {
                    開團評價編號 = m.f開團評價編號,
                    開團編號 = m.f開團編號,
                    會員大頭貼 = n.f大頭貼,
                    會員名稱 = m.f會員名稱,
                    評論 = m.f評論,
                    評分 = m.f評分,
                    活動照片1 = m.f活動照片1,
                    活動照片2 = m.f活動照片2,
                    活動照片3 = m.f活動照片3,
                    活動照片4 = m.f活動照片4,
                    活動照片5 = m.f活動照片5,
                    發布時間 = m.f發布時間
                }).Where(mn => mn.開團編號 == f開團編號).ToList(),
                參加名單 = db.t參加名單.ToList()
            };

            //評價平均分數
            double? avgStar = db.t開團評價.Where(m => m.f開團編號 == f開團編號).Select(m => m.f評分).Average();
            ViewBag.avgStar = avgStar;
            //報名人數
            int sumPeople = db.t參加名單.Where(m => m.f開團編號 == f開團編號).Select(m => m.f會員名稱).Count();
            //出席人數
            int attendPeople = db.t參加名單.Where(m => m.f開團編號 == f開團編號 && m.f是否出席 == "出席").Count();
            //組合字串 出席/報名
            ViewBag.attend = attendPeople + "/" + sumPeople;

            return View("OldTeamDetail", "_LayoutMember", vm);
        }


        //找會員
        //GET:Home/SearchMember
        public ActionResult SearchMember()
        {
            //會員未登入後顯示的畫面(Session)
            if (Session["Member"] == null)
            {
                return RedirectToAction("Index");
            }
            var member = db.t會員狀態.ToList();
            //會員登入後顯示的畫面
            return View("SearchMember", "_LayoutMember", member);
        }
        //自動完成搜尋會員名稱
        public JsonResult GetSearchMemberName(string search)
        {
            List<MemberNameModel> allsearch = db.t會員狀態.Where(m => m.f名稱.Contains(search)).Select(m => new MemberNameModel
            {
                fId = m.f會員狀態編號,
                fName = m.f名稱
            }).ToList();
            return new JsonResult { Data = allsearch, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        //參團歷史紀錄
        //GET:Home/History
        public ActionResult History()
        {

            //會員未登入後顯示的畫面(Session)
            if (Session["Member"] == null)
            {
                return RedirectToAction("Index");
            }
            string f會員名稱 = (Session["Member"] as t會員資料).f名稱;


            var idlist = db.t參加名單.Where(m => m.f會員名稱 == f會員名稱 && m.f狀態 == "參加").Select(m => m.f開團編號).ToList();

            ViewModel vm = new ViewModel()
            {
                歷史紀錄 = db.t開團.Join(db.t開團評價, m => m.f開團編號, n => n.f開團編號, (m, n) => new HistoryViewModel
                {
                    開團編號 = m.f開團編號,
                    團長名稱 = m.f團長名稱,
                    運動項目名稱 = m.f運動項目名稱,
                    標題 = m.f標題,
                    活動結束時間 = m.f活動結束時間,
                    狀態 = m.f狀態,
                    是否出席 = db.t參加名單.Where(t => t.f會員名稱 == f會員名稱 && t.f開團編號 == m.f開團編號 && t.f狀態 == "參加" && t.f是否出席 == "出席").Count(),
                    是否評分 = db.t開團評價.Where(t => t.f會員名稱 == f會員名稱 && t.f開團編號 == m.f開團編號).Select(t => t.f會員名稱).Count(),
                    平均評分 = db.t開團評價.Where(t => t.f開團編號 == m.f開團編號 && t.f評分 != null).Select(t => t.f評分).Average()
                }).Where(mn => idlist.Contains(mn.開團編號) && mn.狀態 == "關團").Distinct().OrderBy(mn => mn.是否評分).ToList(),
            };
            //會員登入後顯示的畫面
            return View("History", "_LayoutMember", vm);
        }

        //新增評價
        //POST:Home/AddReview
        [HttpPost]
        public ActionResult AddReview(int f開團編號, string f評論, int f評分, HttpPostedFileBase[] photos)
        {
            string alert = string.Format("<script>alert('新增評價成功!');location.href='{0}'</script>", Url.Action("History"));
            string f會員名稱 = (Session["Member"] as t會員資料).f名稱;
            t開團評價 review = new t開團評價();
            review.f開團編號 = f開團編號;
            review.f會員名稱 = f會員名稱;
            review.f評論 = f評論;
            review.f評分 = f評分;
            review.f發布時間 = DateTime.Now;

            string f帳號 = (Session["Member"] as t會員資料).f帳號;
            var profile = db.t會員狀態.Where(m => m.f帳號 == f帳號).FirstOrDefault();
            string[] fname = new string[5];
            //使用for迴圈取得所有上傳的檔案
            for (int i = 0; i < photos.Length; i++)
            {
                if (photos[i] != null)
                {
                    //取得圖檔名稱並在檔名前加上"開團編號"與"會員帳號"
                    fname[i] = f開團編號 + profile.f帳號 + Path.GetFileName(photos[i].FileName);
                    //將檔案儲存到網站的files資料夾下
                    var path = Path.Combine(Server.MapPath("~/Review_picture/"), fname[i]);
                    photos[i].SaveAs(path);
                }
            }
            review.f活動照片1 = fname[0];
            review.f活動照片2 = fname[1];
            review.f活動照片3 = fname[2];
            review.f活動照片4 = fname[3];
            review.f活動照片5 = fname[4];
            db.t開團評價.Add(review);
            db.SaveChanges();

            return Content(alert, "text/html");

        }


        //訂閱追蹤名單
        //GET:Home/SubscribersList
        public ActionResult SubscribersList()
        {
            //會員未登入後顯示的畫面(Session)
            if (Session["Member"] == null)
            {
                return RedirectToAction("Index");
            }
            string f會員名稱 = (Session["Member"] as t會員資料).f名稱;
            ViewModel vm = new ViewModel()
            {
                訂閱名單 = db.t訂閱名單.Join(db.t會員狀態, m => m.f訂閱會員名稱, n => n.f名稱, (m, n) => new SubscribersListViewModel
                {
                    訂閱名單編號 = m.f訂閱名單編號,
                    會員名稱 = m.f會員名稱,
                    訂閱會員大頭貼 = n.f大頭貼,
                    訂閱會員名稱 = m.f訂閱會員名稱
                }).Where(mn => mn.會員名稱 == f會員名稱).ToList()
            };
            //會員登入後顯示的畫面
            return View("SubscribersList", "_LayoutMember", vm);
        }
        //訂閱
        //GET:Home/Subscribe
        public ActionResult Subscribe(string f訂閱會員大頭貼, string f訂閱會員名稱)
        {
            string f會員名稱 = (Session["Member"] as t會員資料).f名稱;
            t訂閱名單 list = new t訂閱名單();
            list.f會員名稱 = f會員名稱;
            list.f訂閱會員名稱 = f訂閱會員名稱;
            db.t訂閱名單.Add(list);
            db.SaveChanges();
            return Redirect("SubscribersList");
        }
        //取消訂閱
        //GET:Home/UnSubscribe
        public ActionResult UnSubscribe(int? f訂閱名單編號)
        {
            t訂閱名單 list = db.t訂閱名單.Where(m => m.f訂閱名單編號 == f訂閱名單編號).FirstOrDefault();
            db.t訂閱名單.Remove(list);
            db.SaveChanges();
            return Redirect("SubscribersList");
        }
        //目前創建的團
        //GET:Home/MyTeam
        public ActionResult MyTeam()
        {
            //會員未登入後顯示的畫面(Session)
            if (Session["Member"] == null)
            {
                return RedirectToAction("Index");
            }
            string f會員名稱 = (Session["Member"] as t會員資料).f名稱;
            //找出活動進行中的開團編號(本機)
            var idlist = db.t開團.Where(m => m.f團長名稱 == f會員名稱 && m.f狀態 == "開團" && m.f活動開始時間 <= DateTime.Now && m.f活動結束時間 >= DateTime.Now).Select(m => m.f開團編號).ToList();
            ViewModel vm = new ViewModel()
            {
                //尚未報名截止
                開團 = db.t開團.Where(m => m.f團長名稱 == f會員名稱 && m.f狀態 == "開團" && m.f活動開始時間 > DateTime.Now).ToList(),



                //活動進行中
                進行中的團 = db.t開團.Join(db.t參加名單, m => m.f開團編號, n => n.f開團編號, (m, n) => new ActivityViewModel
                {
                    開團編號 = m.f開團編號,
                    團長名稱 = m.f團長名稱,
                    運動項目名稱 = m.f運動項目名稱,
                    標題 = m.f標題,
                    備註 = m.f備註,
                    活動開始時間 = m.f活動開始時間,
                    活動結束時間 = m.f活動結束時間,
                    貼文發布時間 = m.f貼文發布時間,
                    報名截止時間 = m.f報名截止時間,
                    人數上限 = m.f人數上限,
                    活動地點 = m.f活動地點,
                    緯度 = m.f緯度,
                    經度 = m.f經度,
                    狀態 = m.f狀態,
                    是否打卡 = db.t參加名單.Where(t => t.f會員名稱 == f會員名稱 && t.f開團編號 == m.f開團編號 && t.f狀態 == "參加" && t.f是否出席 == "出席").Select(t => t.f會員名稱).Count()
                }).Where(mn => mn.團長名稱 == f會員名稱 && mn.狀態 == "開團" && mn.活動開始時間 <= DateTime.Now && mn.活動結束時間 >= DateTime.Now).Distinct().ToList(),



                已出席名單 = db.t參加名單.Join(db.t會員狀態, m => m.f會員名稱, n => n.f名稱, (m, n) => new AttendeeListViewModel
                {
                    參加名單編號 = m.f參加名單編號,
                    開團編號 = m.f開團編號,
                    會員大頭貼 = n.f大頭貼,
                    會員名稱 = m.f會員名稱,
                    狀態 = m.f狀態,
                    是否出席 = m.f是否出席
                }).Where(mn => idlist.Contains((int)mn.開團編號) && mn.狀態 == "參加" && mn.是否出席 == "出席").ToList(),

                未出席名單 = db.t參加名單.Join(db.t會員狀態, m => m.f會員名稱, n => n.f名稱, (m, n) => new AbsenceListViewModel
                {
                    參加名單編號 = m.f參加名單編號,
                    開團編號 = m.f開團編號,
                    會員大頭貼 = n.f大頭貼,
                    會員名稱 = m.f會員名稱,
                    狀態 = m.f狀態,
                    是否出席 = m.f是否出席
                }).Where(mn => idlist.Contains((int)mn.開團編號) && mn.狀態 == "參加" && mn.是否出席 == null).ToList(),
            };
            //會員登入後顯示的畫面
            return View("MyTeam", "_LayoutMember", vm);
        }
        //目前參加的團
        //GET:Home/MyJoinTeam
        public ActionResult MyJoinTeam()
        {
            //會員未登入後顯示的畫面(Session)
            if (Session["Member"] == null)
            {
                return RedirectToAction("Index");
            }
            string f會員名稱 = (Session["Member"] as t會員資料).f名稱;
            var idlist = db.t參加名單.Where(m => m.f會員名稱 == f會員名稱 && m.f狀態 == "參加").Select(m => m.f開團編號).ToList();
            ViewModel vm = new ViewModel()
            {
                //尚未報名截止(本機)
                開團 = db.t開團.Where(m => idlist.Contains(m.f開團編號) && m.f團長名稱 != f會員名稱 && m.f狀態 == "開團" && m.f活動開始時間 > DateTime.Now).ToList(),

                //活動進行中(本機)
                進行中的團 = db.t開團.Join(db.t參加名單, m => m.f開團編號, n => n.f開團編號, (m, n) => new ActivityViewModel
                {
                    開團編號 = m.f開團編號,
                    團長名稱 = m.f團長名稱,
                    運動項目名稱 = m.f運動項目名稱,
                    標題 = m.f標題,
                    備註 = m.f備註,
                    活動開始時間 = m.f活動開始時間,
                    活動結束時間 = m.f活動結束時間,
                    貼文發布時間 = m.f貼文發布時間,
                    報名截止時間 = m.f報名截止時間,
                    人數上限 = m.f人數上限,
                    活動地點 = m.f活動地點,
                    緯度 = m.f緯度,
                    經度 = m.f經度,
                    狀態 = m.f狀態,
                    是否打卡 = db.t參加名單.Where(t => t.f會員名稱 == f會員名稱 && t.f開團編號 == m.f開團編號 && t.f狀態 == "參加" && t.f是否出席 == "出席").Select(t => t.f會員名稱).Count()
                }).Where(mn => idlist.Contains(mn.開團編號) && mn.團長名稱 != f會員名稱 && mn.狀態 == "開團" && mn.活動開始時間 <= DateTime.Now && mn.活動結束時間 >= DateTime.Now).Distinct().ToList()

            };
            //會員登入後顯示的畫面
            return View("MyJoinTeam", "_LayoutMember", vm);
        }



        //團長打卡
        //POST:Home/MyTeamCheckIn
        [HttpPost]
        public ActionResult MyTeamCheckIn(int f開團編號)
        {
            string alert = string.Format("<script>alert('打卡成功!');location.href='{0}'</script>", Url.Action("MyTeam"));
            string f會員名稱 = (Session["Member"] as t會員資料).f名稱;
            t參加名單 crewlist = new t參加名單();
            crewlist = db.t參加名單.Where(m => m.f會員名稱 == f會員名稱 && m.f開團編號 == f開團編號).FirstOrDefault();
            crewlist.f是否出席 = "出席";
            db.SaveChanges();
            return Content(alert, "text/html");
        }

        //個人打卡
        //POST:Home/MyJoinTeamCheckIn
        [HttpPost]
        public ActionResult MyJoinTeamCheckIn(int f開團編號)
        {
            string alert = string.Format("<script>alert('打卡成功!');location.href='{0}'</script>", Url.Action("MyJoinTeam"));
            string f會員名稱 = (Session["Member"] as t會員資料).f名稱;
            t參加名單 crewlist = new t參加名單();
            crewlist = db.t參加名單.Where(m => m.f會員名稱 == f會員名稱 && m.f開團編號 == f開團編號).FirstOrDefault();
            crewlist.f是否出席 = "出席";
            db.SaveChanges();
            return Content(alert, "text/html");
        }
    }
}