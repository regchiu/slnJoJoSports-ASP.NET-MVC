using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjJoJoSports.Models
{
    public class OldTeamDetailViewModel
    {
        public int 開團評價編號 { get; set; }
        public Nullable<int> 開團編號 { get; set; }        
        public string 會員大頭貼 { get; set; }//join欄位
        public string 會員名稱 { get; set; }
        public string 評論 { get; set; }
        public Nullable<int> 評分 { get; set; }
        public string 活動照片1 { get; set; }
        public string 活動照片2 { get; set; }
        public string 活動照片3 { get; set; }
        public string 活動照片4 { get; set; }
        public string 活動照片5 { get; set; }
        public Nullable<System.DateTime> 發布時間 { get; set; }
    }
}