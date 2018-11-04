using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjJoJoSports.Models
{
    public class CommentViewModel
    {
        public int 開團留言板編號 { get; set; }
        public Nullable<int> 開團編號 { get; set; }
        public string 會員大頭貼 { get; set; }//join欄位
        public string 會員名稱 { get; set; }
        public string 留言內容 { get; set; }
        public Nullable<System.DateTime> 發布時間 { get; set; }
    }
}