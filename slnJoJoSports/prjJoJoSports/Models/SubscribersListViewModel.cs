using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjJoJoSports.Models
{
    public class SubscribersListViewModel
    {
        public int 訂閱名單編號 { get; set; }
        public string 會員名稱 { get; set; }
        public string 訂閱會員大頭貼 { get; set; }//Join欄位
        public string 訂閱會員名稱 { get; set; }
    }
}