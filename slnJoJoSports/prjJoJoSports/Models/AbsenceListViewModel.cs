using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjJoJoSports.Models
{
    public class AbsenceListViewModel
    {
        public int 參加名單編號 { get; set; }
        public Nullable<int> 開團編號 { get; set; }
        public string 會員大頭貼 { get; set; }//Join欄位
        public string 會員名稱 { get; set; }
        public string 狀態 { get; set; }
        public string 是否出席 { get; set; }
    }
}