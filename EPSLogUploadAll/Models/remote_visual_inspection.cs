using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSLogUploadAll.Models
{
    class remote_visual_inspection
    {
        [DisplayName("紀錄編號")]
        public string id { get; set; }

        [DisplayName("建立日期")]
        //[Required(ErrorMessage = "建立日期不可空白")]
        //[DateAfter("2022/05/12", ErrorMessage = "your {0} should after 2022/05/12")]
        //[DataType(DataType.Date)]
        public Nullable<System.DateTime> tdate { get; set; }

        [DisplayName("備註1")]
        public string comment1 { get; set; }

        [DisplayName("備註2")]
        public string comment2 { get; set; }

        [DisplayName("鋼捲1")]
        public string coil1 { get; set; }

        [DisplayName("鋼捲2")]
        public string coil2 { get; set; }

        [DisplayName("鋼捲3")]
        public string coil3 { get; set; }

        [DisplayName("鋼捲4")]
        public string coil4 { get; set; }

        [DisplayName("鋼捲5")]
        public string coil5 { get; set; }

        [DisplayName("鋼捲6")]
        public string coil6 { get; set; }

        [DisplayName("鋼捲7")]
        public string coil7 { get; set; }

        [DisplayName("鋼捲8")]
        public string coil8 { get; set; }

        [DisplayName("鋼捲9")]
        public string coil9 { get; set; }

        [DisplayName("鋼捲10")]
        public string coil10 { get; set; }

        [DisplayName("鋼捲11")]
        public string coil11 { get; set; }

        [DisplayName("鋼捲12")]
        public string coil12 { get; set; }

        [DisplayName("鋼捲13")]
        public string coil13 { get; set; }

        [DisplayName("鋼捲14")]
        public string coil14 { get; set; }

        [DisplayName("鋼捲15")]
        public string coil15 { get; set; }

        [DisplayName("鋼捲16")]
        public string coil16 { get; set; }

        [DisplayName("載運車牌")]
        public string carId { get; set; }

        [DisplayName("輸入者")]
        public string creator { get; set; }

        [DisplayName("更新時間")]
        public Nullable<System.DateTime> updateTime { get; set; }

        [DisplayName("IP")]
        public string ip { get; set; }

        [DisplayName("位置")]
        public string location { get; set; }

        [DisplayName("查詢月份")]
        public string queryMonth { get; set; }

    }
}
