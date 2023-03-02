using EPSLogUploadAll.Models;
using prjTCP_ChatRoomClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace EPSLogUploadAll
{
    public partial class Form1 : Form
    {
        int i = 0;
        int lastTimeReadLineIndex = 0;
        int allLinesCount = 0;
        string carId = "";
        string creatorId = "";
        List<string> coilList = new List<string>();
        List<remote_visual_inspection> rviList = new List<remote_visual_inspection>();
        DateTime CrossDay;
        string Location;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ImportAllLogToWebsite();
            MessageBox.Show("上傳完成");
        }
        void ImportAllLogToWebsite()
        {
            string myLogFolder;
            using (ReadINI oTINI = new ReadINI(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.ini")))
            {
                myLogFolder = oTINI.getKeyValue("WorklogPath", "Value"); //Section name=Worklog；Key name=Value
                Location = oTINI.getKeyValue("Location", "Value");
            }
            string[] PathEntries = GetAllFileInDirectory(myLogFolder);

            List<string> PathEntriesList = PathEntries.ToList<string>();


            for (int i = 0; i < PathEntriesList.Count; i++)
            {

                if (PathEntriesList[i].IndexOf("WorkLog") == -1)
                {
                    PathEntriesList.Remove(PathEntriesList[i]);
                    i--;
                }
            }

            for (int i = 0; i < PathEntriesList.Count; i++)
            {
                string[] dateStringParse = PathEntriesList[i].Substring(myLogFolder.Length + 1, 10).Split('-');
                var ParsedDate = DateTime.Parse($"{dateStringParse[0]}/{dateStringParse[1]}/{dateStringParse[2]}");
                if (ParsedDate < DateTime.Parse("2022/05/13"))
                {
                    //移除遠端檢放上線前的log
                    PathEntriesList.Remove(PathEntriesList[i]);
                    i--;
                }
            }

            for (int i = 0; i < PathEntriesList.Count; i++)
            {
                readLogOneDay(PathEntriesList[i]);
                lastTimeReadLineIndex = 0;
            }
        }

        void readLogOneDay(string myLogPath)
        {
            var appLocation = AppDomain.CurrentDomain.BaseDirectory;
            List<string> allLines = new List<string>();
            using (var fs = new FileStream(myLogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var logFileReader = new StreamReader(fs, ASCIIEncoding.Default))
            {
                while (!logFileReader.EndOfStream)
                {
                    allLines.Add(logFileReader.ReadLine());
                    allLinesCount++;
                    // Your code here
                }
                // read the stream
                //...
            }

            //上次讀取log檔行數與這次讀取log檔行數相同，直接結束
            if (allLinesCount == lastTimeReadLineIndex) return;

            int thisTimeReadLineIndex = 0;
            foreach (var item in allLines)
            {
                if (thisTimeReadLineIndex >= lastTimeReadLineIndex)
                {
                    if (item.IndexOf("車輛報到") != -1)
                    {
                        var tt = item.IndexOf("車輛報到");
                        int commaPos1 = item.IndexOf(",");
                        int car_checkin_pos = commaPos1 + 7;
                        int car_Num_start_pos = car_checkin_pos + 4;

                        carId = item.Substring(car_Num_start_pos, 8);
                    }
                    else if (item.IndexOf("掃瞄車上鋼品") != -1)
                    {
                        var tt = item.IndexOf("掃瞄車上鋼品");

                        var commaPos1 = item.IndexOf(",");
                        var commaPos2 = item.IndexOf("顆", commaPos1 + 1);
                        var commaPos3 = item.IndexOf(",", commaPos1 + 1);
                        //line += item.Substring(commaPos1 + 1, commaPos2 - commaPos1 - 1) + "\n";
                        coilList.Add(item.Substring(commaPos2 + 1, commaPos3 - commaPos2 - 1));
                    }
                    else if (item.IndexOf("放行") != -1)
                    {
                        var tt = item.IndexOf("放行");
                        var commaPos1 = item.IndexOf(",");
                        //line += item.Substring(commaPos1 + 1) + "\n" + "等待車輛報到中........\n";

                        //keep_Queue.Clear();
                        remote_visual_inspection rvi = new remote_visual_inspection();
                        rvi.carId = carId;
                        rvi.creator = creatorId;
                        foreach (var coil in coilList)
                        {
                            if (rvi.coil1 is null)
                                rvi.coil1 = coil;
                            else if (rvi.coil2 is null)
                                rvi.coil2 = coil;
                            else if (rvi.coil3 is null)
                                rvi.coil3 = coil;
                            else if (rvi.coil4 is null)
                                rvi.coil4 = coil;
                            else if (rvi.coil5 is null)
                                rvi.coil5 = coil;
                            else if (rvi.coil6 is null)
                                rvi.coil6 = coil;
                            else if (rvi.coil7 is null)
                                rvi.coil7 = coil;
                            else if (rvi.coil8 is null)
                                rvi.coil8 = coil;
                        }
                        rvi.location = Location;
                        rvi.tdate = DateTime.Parse(item.Substring(0, commaPos1));
                        rviList.Add(rvi);
                        carId = "";
                        coilList = new List<string>();
                    }
                    else if (item.IndexOf("已退車") != -1)
                    {
                        var tt = item.IndexOf("已退車");
                        carId = "";
                        coilList = new List<string>();
                    }
                    else if (item.IndexOf("無內銷車籍記錄") != -1)
                    {
                        carId = "";
                    }
                    else if (item.IndexOf("無外銷車籍記錄") != -1)
                    {
                        carId = "";
                    }
                    else if (item.IndexOf("登入") != -1)
                    {
                        var creatorId_Num_start_pos = item.IndexOf("檢核員");
                        creatorId = item.Substring(creatorId_Num_start_pos + 3 , 6);

                    }
                    else if (item.IndexOf("登出") != -1)
                    {
                        var tt = item.IndexOf("登出");
                        creatorId = "";
                    }
                }
                thisTimeReadLineIndex++;
            }
            //將本次讀取到的行數紀錄起來，下次讀取時可知道上次讀取到哪一行，避免重複讀取
            lastTimeReadLineIndex = thisTimeReadLineIndex;
            uploadList(rviList);
            rviList = new List<remote_visual_inspection>();
            allLinesCount = 0;
        }

        private void uploadList(List<remote_visual_inspection> rviList)
        {
            if (rviList.Count == 0) return;
            try
            {
                string url = "http://c34web.csc.com.tw/C349WebMVC/api/RVI_API";

                foreach (var rvi in rviList)
                {
                    upload(rvi);
                }


            }
            catch (Exception ex)
            {

            }
        }

        static Boolean upload(remote_visual_inspection rvi)
        {
            try
            {
                string url = "http://c34web.csc.com.tw/C349WebMVC/api/RVI_API";
                //string url = "http://c34web.csc.com.tw/C349WebMVC/api/RVI_API";
                //string url = "http://localhost:1954/RVI_CreateWithEPSLOG/Create";
                //string url = "http://localhost:1954/api/RVI_API";

                Boolean log_exist = false;  //此flag用來檢查server端是否存在相同log，若有則取消此次上傳
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url + $"?tdate={rvi.tdate.Value.ToString("yyyy-MM-dd HH:mm:ss")}&carId={rvi.carId}");
                //set the cookie container object
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                //set method POST and content type application/x-www-form-urlencoded
                request.Method = "GET";
                request.ContentType = "application/json";

                //看到.GetResponse()才代表真正把 request 送到 伺服器
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default))
                    {
                        var z = sr.ReadToEnd().Split(',')[0].Split(':')[1];
                        if (z.IndexOf("null", 0) >= 0)
                        {
                            log_exist = true;
                        }
                    }
                }
                if (!log_exist) return false;

                string Param = $"carId={rvi.carId}&tdate={HttpUtility.UrlEncode(rvi.tdate.Value.ToString("yyyy-MM-dd HH:mm:ss"), Encoding.UTF8)}" +
                    $"&coil1={rvi.coil1}" +
                    $"&coil2={rvi.coil2}" +
                    $"&coil3={rvi.coil3}" +
                    $"&coil4={rvi.coil4}" +
                    $"&coil5={rvi.coil5}" +
                    $"&coil6={rvi.coil6}" +
                    $"&coil7={rvi.coil7}" +
                    $"&coil8={rvi.coil8}" +
                    $"&coil9={rvi.coil9}" +
                    $"&coil10={rvi.coil10}" +
                    $"&coil11={rvi.coil11}" +
                    $"&coil12={rvi.coil12}" +
                    $"&coil13={rvi.coil13}" +
                    $"&coil14={rvi.coil14}" +
                    $"&coil15={rvi.coil15}" +
                    $"&coil16={rvi.coil16}" +
                    $"&location={rvi.location}" +
                    $"&creator={rvi.creator}";
                request = (HttpWebRequest)HttpWebRequest.Create(url + $"/create?{Param}");

                request.Method = "GET";
                request.ContentType = "application/json";
                string EIPLoginPostData = "";
                EIPLoginPostData = Param;

                Boolean UpLoadAgain = false;

                //看到.GetResponse()才代表真正把 request 送到 伺服器
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            UpLoadAgain = true;
                        }
                        var yy = sr.ReadToEnd();
                        // sr 就是伺服器回覆的資料
                        //Response.Write(sr.ReadToEnd()); //將 sr 寫入到 html中，呈現給客戶端看
                    }
                }
                while (UpLoadAgain)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"{rvi.tdate}/{rvi.carId}/{rvi.coil1}/{rvi.coil2}/{rvi.coil3}/{rvi.coil4}/{rvi.coil5}/{rvi.coil6}/{rvi.coil7}/{rvi.coil8}/{rvi.coil9}/{rvi.coil10}/{rvi.coil11}/{rvi.coil12}/{rvi.coil13}/{rvi.coil14}/{rvi.coil15}/{rvi.coil16} UpLoad Fail.");
                    sb.Clear();
                    System.Threading.Thread.Sleep(1000);
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            if (response.StatusCode != HttpStatusCode.OK)
                            {
                                UpLoadAgain = true;
                            }
                        }
                    }
                }

                return true;


            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string[] GetAllFileInDirectory(string PPath)
        {
            try
            {
                string[] entries = Directory.GetFiles(PPath);
                return entries;
            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "upload_err_log.txt"), PPath);
                File.AppendAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "upload_err_log.txt"), ex.ToString());
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string myLogFolder;
            using (ReadINI oTINI = new ReadINI(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.ini")))
            {
                myLogFolder = oTINI.getKeyValue("WorklogPath", "Value"); //Section name=Worklog；Key name=Value              

            }
            string[] PathEntries = GetAllFileInDirectory(myLogFolder);
            string yy = "";
            foreach (var item in PathEntries)
            {
                yy += item.ToString() +"\n";
            }            
        }
    }
}
