using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjJoJoSports.Models
{
    public class ActivityViewModel
    {
        public int 開團編號 { get; set; }
        public string 團長名稱 { get; set; }
        public string 運動項目名稱 { get; set; }
        public string 標題 { get; set; }
        public string 備註 { get; set; }
        public Nullable<System.DateTime> 活動開始時間 { get; set; }
        public Nullable<System.DateTime> 活動結束時間 { get; set; }
        public Nullable<System.DateTime> 貼文發布時間 { get; set; }
        public Nullable<System.DateTime> 報名截止時間 { get; set; }
        public Nullable<int> 人數上限 { get; set; }
        public string 活動地點 { get; set; }
        public Nullable<decimal> 緯度 { get; set; }
        public Nullable<decimal> 經度 { get; set; }
        public string 狀態 { get; set; }       
        public int 是否打卡 { get; set; }
    }
}