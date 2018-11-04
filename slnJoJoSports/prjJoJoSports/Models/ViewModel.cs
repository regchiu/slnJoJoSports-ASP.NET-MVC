using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjJoJoSports.Models
{
    public class ViewModel
    {
        public List<t開團> 開團 { get; set; }
        public List<ActivityViewModel> 進行中的團 { get; set; }
        public List<CommentViewModel> 開團留言板 { get; set; }
        public List<ReplyViewModel> 留言板回覆 { get; set; }
        public List<t運動項目> 運動項目 { get; set; }
        public List<t縣市名稱> 縣市名稱 { get; set; }
        public List<t會員狀態> 會員狀態 { get; set; }
        public List<OldTeamDetailViewModel> 開團評價 { get; set; }
        public List<t參加名單> 參加名單 { get; set; }
        public List<AttendeeListViewModel> 已出席名單 { get; set; }
        public List<AbsenceListViewModel> 未出席名單 { get; set; }
        public List<OldTeamListViewModel> 舊團 { get; set; }
        public List<HistoryViewModel> 歷史紀錄 { get; set; }
        public List<SubscribersListViewModel> 訂閱名單 { get; set; }
    }
}