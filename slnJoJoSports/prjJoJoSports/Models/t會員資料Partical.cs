using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace prjJoJoSports.Models
{
    [MetadataType(typeof(t會員資料Metadata))]
    public partial class t會員資料
    {
        public class t會員資料Metadata
        {
            public int f會員資料編號 { get; set; }
            [DisplayName("電子郵件")]
            [EmailAddress(ErrorMessage = "電子郵件格式有誤")]
            [Required(ErrorMessage = "請輸入電子郵件")]
            public string f帳號 { get; set; }
            [DisplayName("密碼")]
            [StringLength(12, ErrorMessage = "密碼長度至少8個字元至多12個字元", MinimumLength = 8)]
            [Required(ErrorMessage = "請輸入密碼")]
            public string f密碼 { get; set; }
            //非DB的欄位
            [DisplayName("確認密碼")]
            [Compare("f密碼", ErrorMessage = "兩組密碼必須相同")]
            public string f確認密碼 { get; set; }
            [DisplayName("姓名")]
            [Required(ErrorMessage = "請輸入姓名")]
            public string f姓名 { get; set; }
            [DisplayName("用戶名稱")]
            [Required(ErrorMessage = "請輸入用戶名稱")]
            public string f名稱 { get; set; }
            [DisplayName("性別")]
            [Required(ErrorMessage = "請選擇您的性別")]
            public string f性別 { get; set; }
            [DisplayName("聯絡電話")]
            [Phone(ErrorMessage = "電話格式有誤")]
            [Required(ErrorMessage = "請輸入聯絡電話")]
            public string f電話 { get; set; }
            [DisplayName("出生日期")]
            [Required(ErrorMessage = "請輸入出生日期")]
            [DataType(DataType.Date)]
            public Nullable<System.DateTime> f生日 { get; set; }
        }
    }
}