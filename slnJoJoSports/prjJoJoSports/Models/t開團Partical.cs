using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace prjJoJoSports.Models
{
    [MetadataType(typeof(t開團Metadata))]
    public partial class t開團
    {
        public class t開團Metadata
        {
            public int f開團編號 { get; set; }
            public string f團長名稱 { get; set; }
            [DisplayName("運動項目")]
            [Required(ErrorMessage = "請選擇運動項目")]
            public string f運動項目名稱 { get; set; }
            [DisplayName("團名")]
            [Required(ErrorMessage = "請輸入團名")]
            public string f標題 { get; set; }
            [DisplayName("備註")]
            public string f備註 { get; set; }
            [DisplayName("活動開始時間")]
            [DataType(DataType.DateTime)]
            [Required(ErrorMessage = "請輸入活動開始時間")]
            public DateTime f活動開始時間 { get; set; }
            [DisplayName("活動結束時間")]
            [DataType(DataType.DateTime)]
            [Required(ErrorMessage = "請輸入活動結束時間")]
            public DateTime f活動結束時間 { get; set; }
            public Nullable<System.DateTime> f貼文發布時間 { get; set; }
            [DisplayName("報名截止時間")]
            [DataType(DataType.DateTime)]
            [Required(ErrorMessage = "請輸入報名截止時間")]
            public DateTime f報名截止時間 { get; set; }
            [DisplayName("人數上限")]
            [Required(ErrorMessage = "請設定人數上限")]
            [Range(1, 100, ErrorMessage = "人數至少1人，最多100人")]
            public Nullable<int> f人數上限 { get; set; }
            [DisplayName("活動地點")]
            [Required(ErrorMessage = "請輸入活動地點")]
            public string f活動地點 { get; set; }
            public Nullable<decimal> f緯度 { get; set; }
            public Nullable<decimal> f經度 { get; set; }
            public string f狀態 { get; set; }
        }
    }
}