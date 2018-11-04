using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjJoJoSports.Models
{
    public class OldTeamListViewModel
    {
        public int 開團編號 { get; set; }
        public string 團長名稱 { get; set; }
        public string 運動項目名稱 { get; set; }
        public string 標題 { get; set; }
        public Nullable<System.DateTime> 活動結束時間 { get; set; }
        public string 活動地點 { get; set; }
        public int 是否出席 { get; set; }
        public string 狀態 { get; set; }
        public Nullable<double> 平均評分 { get; set; }
    }
}