using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace APRSWXUDPSender
{
    public partial class MainForm : Form, SLV, WevViewRes
    {
        private WXConfig config = null;
        private WebView webserver = null;
        private string mainCap = null;
        private byte mode = 0; // 0 - not run, 1 - single, 2 - multi
        private List<KeyValuePair<string[], WXConfig>> mult = null;

        public static string Software = "APRSWXUDPSender";
        public static string Version
        {
            get
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.FileVersion;
            }
        }

        public MainForm(string[] args)
        {            
            InitializeComponent();
            this.Text += " (v" + MainForm.Version+")";
            this.mainCap = this.Text;
            config = WXConfig.Load(this);
            bool minimized = config["Minimized"] == "Да";
            bool runselector = false;
            foreach (string arg in args)
            {
                string varg = arg.ToLower();
                if (varg == "/minimized") minimized = true;
                if (varg == "/min") minimized = true;
                if (varg == "/runselector") runselector = true;
            };
            AfterLoad(!runselector);
            if (runselector) runmultiToolStripMenuItem_Click(null, null);
            if (minimized) this.WindowState = FormWindowState.Minimized;
        }

        public void AfterLoad(bool allowstart)
        {
            textBox1.ReadOnly = config["ALLOW_EDIT"] != "Да";
            if(allowstart)
                if (config["START"] == "Да")
                    Start();
        }

        public ListView GetListView()
        {
            return txprops;
        }
        
        public static void SendUDP(string host, int port, string data)
        {
            UdpClient udp = new UdpClient();
            udp.Connect(host, port);
            byte[] dt = System.Text.Encoding.GetEncoding(1251).GetBytes(data);
            udp.Send(dt, dt.Length);
            udp.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(((e.CloseReason == CloseReason.FormOwnerClosing) || (e.CloseReason == CloseReason.MdiFormClosing) || (e.CloseReason == CloseReason.UserClosing)) && (mode > 0))
            {
                DialogResult dr = MessageBox.Show("Сервер запущен!\r\nОстановить сервер и закрыть приложение?", "Закрытие приложения", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (dr != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                };
            };
            Stop();
            config.Save();
        }

        private void txprops_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 39) SetProperty(1);
            if (e.KeyValue == 37) SetProperty(-1);
        }

        private void txprops_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r') SetProperty();
            if (e.KeyChar == ' ') SetProperty();
        }

        private void txprops_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            SetProperty();
        }

        private void SetProperty() { SetProperty(0); }
        private void SetProperty(int dir)
        {
            if (mode > 0) return;
            if (txprops.SelectedItems.Count == 0) return;
            int index = txprops.SelectedIndices[0];
            if (index < 0) return;

            int id = int.Parse(txprops.Items[index].SubItems[0].Text);
            string caption = config.GetCaption(id);
            string reftext = config[id];
            string mask = "";
            string typ = config.GetType(id);
            string[] yno = new string[] { "Да", "Нет" };

            if (dir == 0)
            {
                if (typ == "int") mask = @"R^\d*$";
                if (typ == "ico") mask = @"R^.{0,2}$";
                if (typ == "cal") mask = @"R^[a-zA-Z0-9\-]{0,9}$";
                if (typ == "pas") mask = @"R^[\d\-]{0,5}$";
                if (typ == "loc") mask = @"R^[a-zA-Z0-9]{0,6}$";
                if (typ == "lat") mask = @"R^((?:\d{1,2}\.\d*°?\s*[NnSs])|(?:\d{1,2}°?\s+\d{1,2}\.\d*\'?\s*[NnSs])|(?:\d{1,2}°?\s+\d{1,2}\'?\s+\d{1,2}\""?\s*[NnSs])|([\s\d\.]*))$";
                if (typ == "lon") mask = @"R^((?:\d{1,3}\.\d*°?\s*[EeWw])|(?:\d{1,3}°?\s+\d{1,2}\.\d*\'?\s*[EeWw])|(?:\d{1,3}°?\s+\d{1,2}\'?\s+\d{1,2}\""?\s*[EeWw])|([\s\d\.]*))$";

                if (typ == "yno")
                {
                    if (System.Windows.Forms.InputBox.Show("Изменение параметров:", caption + ":", yno, ref reftext, false) == DialogResult.OK)
                        txprops.Items[index].SubItems[2].Text = config[id] = reftext;
                }
                else
                {
                    if (System.Windows.Forms.InputBox.Show("Изменение параметров:", caption + ":", ref reftext, mask) == DialogResult.OK)
                    {
                        txprops.Items[index].SubItems[2].Text = config[id] = reftext;
                        txprops.Items[index].SubItems[3].Text = config.GetHint(id);
                    };
                };
            }
            else
            {
                if (typ == "yno")
                {
                    txprops.Items[index].SubItems[2].Text = config[id] = yno[reftext == "Да" ? 1 : 0];
                };
            };
            if ((config.GetName(id) == "TCP") && (webserver != null)) Stop();
            AfterLoad(false);
            if (config["START"] == "Да") Start(); else Stop();
        }

        private void Start() { Start(false); }
        private void Start(bool multi)
        {
            if ((webserver != null) && (webserver.Running)) return;
            int port = int.Parse(config["INC_HTTPPORT"]);
            if((port <= 0)  || (port > 65536)) return;

            webserver = new WebView(port, this);
            if (csl.Text.Length > 0) csl.Text = "\r\n" + csl.Text;
            if (multi)
            {
                string[][] pres = WXConfig.GetPresets();
                if (pres == null) multi = false;
                else
                {
                    webserver.AnalyzeNotHTTP = true;
                    AddToLog("Запуск HTTP сервера на порту " + port.ToString() + " - селекторный режим");
                    AddToLog(" - APRSWXUDPSender.exe /runselector [/minimized] -- для запуска в селекторном режиме из командной строки");                    
                    mult = new List<KeyValuePair<string[], WXConfig>>();
                    foreach (string[] pr in pres)
                    {                        
                        WXConfig wxc = WXConfig.Load(null, pr[0]);
                        string ssel = wxc["APRS_SELECTOR"];
                        if (String.IsNullOrEmpty(ssel))
                        {
                            AddToLog(String.Format(" - Игнорирование селектора {0} --> Нет условия", pr[2]));
                        }
                        else
                        {
                            AddToLog(String.Format(" - Инициализация селектора {0} --> Условие: {1} --> Режим {3} --> Отправлять: {2}", pr[2], ssel, wxc["SEND_WX"], wxc["TCP"] == "Да" ? "HTTP/TCP" : "HTTP"));
                            mult.Add(new KeyValuePair<string[], WXConfig>(pr, wxc));
                        };                        
                    };                    
                };
            };            
            if (!multi)
            {
                webserver.AnalyzeNotHTTP = config["TCP"] == "Да";
                AddToLog("Запуск HTTP сервера на порту " + port.ToString());
            };
            try
            {
                webserver.Start();
                if (multi)
                {
                    AddToLog(String.Format("Запущен: http://127.0.0.1:{0}/ - селекторный режим: {1} станций", port, mult == null ? 0 : mult.Count));
                    this.Text = this.mainCap + " ~ Listen: " + webserver.ServerPort.ToString() + String.Format(" Multi {0}", mult == null ? 0 : mult.Count);                    
                }
                else
                {
                    AddToLog(String.Format("Запущен: http://127.0.0.1:{0}/", port));
                    this.Text = this.mainCap + " ~ Listen: " + webserver.ServerPort.ToString();
                };
                
                conmnu.Enabled = false;
                setdefsToolStripMenuItem.Enabled = false;
                if (!multi)
                {
                    runmultiToolStripMenuItem.Enabled = false;
                    runsingleToolStripMenuItem.Checked = true;
                };
                if (multi) runsingleToolStripMenuItem.Enabled = false;
                mode = multi ? (byte)2 : (byte)1;
            }
            catch (Exception ex)
            {
                AddToLog("Ошибка: " + ex.Message);
                webserver.Stop();
                webserver.Dispose();
                webserver = null;
            };
        }

        private void Stop()
        {
            this.Text = this.mainCap;
            conmnu.Enabled = true;
            setdefsToolStripMenuItem.Enabled = true;
            runmultiToolStripMenuItem.Enabled = true;
            runsingleToolStripMenuItem.Checked = false;
            runsingleToolStripMenuItem.Enabled = true;
            mode = 0;
            mult = null;
            if (webserver == null) return;
            if ((webserver != null) && (!webserver.Running)) return;            
            AddToLog("Остановка HTTP сервера");
            webserver.Stop();
            webserver.Dispose();
            webserver = null;
            AddToLog("Остановлен");            
        }

        private void AddToLog(string text)
        {
            csl.Text = DateTime.Now.ToString("dd.MM.yyyy ddd HH:mm:ss") + ": " + text + "\r\n" + csl.Text;
            if (csl.Text.Length > 65536) csl.Text = csl.Text.Substring(0,65536);
        }

        public bool OnQuery(string full, string get, string post, string path)
        {
            if (String.IsNullOrEmpty(full) && String.IsNullOrEmpty(get) && String.IsNullOrEmpty(post)) return false;            
            if (mode == 1)
            {
                bool res = false;
                if (!String.IsNullOrEmpty(full)) res = res || Process(full, path);
                if (!String.IsNullOrEmpty(get)) res = res || Process(get, path + " GET");
                if (!String.IsNullOrEmpty(post)) res = res || Process(post, path + " POST");
                if (res) this.Invoke(new ProcLoop(ProcessLoop), new object[] { config });
                return res;
            };
            if (mode == 2)
            {
                bool res = false;
                WXConfig config = null;
                if (!String.IsNullOrEmpty(full)) res = res || ProcessMulti(full, path, ref config);
                if (!String.IsNullOrEmpty(get)) res = res || ProcessMulti(get, path + " GET", ref config);
                if (!String.IsNullOrEmpty(post)) res = res || ProcessMulti(post, path + " POST", ref config);
                if (res) this.Invoke(new ProcLoop(ProcessLoop), new object[] { config });
                return res;
            };
            return false;
        }

        public bool Process(string query, string path)
        {
            this.Invoke(new ProcLog(AddToLog), new object[] { String.Format("Принято {1}: {0}", query, path) });
            bool res = false;
            if(!String.IsNullOrEmpty(config["APRS_SELECTOR"]))
            {
                Regex rx = new Regex(config["APRS_SELECTOR"], RegexOptions.None);
                Match mx = rx.Match(query);
                if (mx.Success)
                    this.Invoke(new ProcLog(AddToLog), new object[] { String.Format(" ? Найден селектор: {0}", mx.Groups[0].Value) });
                else
                    return false;
            };
            foreach (WXConfig.WXParam wxp in config.PARAMS)
            {
                if (wxp.typ != "par") continue;
                Regex rx = new Regex(wxp.value, RegexOptions.None);
                Match mx = rx.Match(query);
                if (mx.Success && (mx.Groups.Count > 1))
                {
                    res = true;
                    wxp.inc_data = mx.Groups[1].Value;
                    if (mx.Groups[wxp.name] != null) wxp.inc_data = mx.Groups[wxp.name].Value;
                    wxp.inc_last = DateTime.UtcNow;
                    this.Invoke(new ProcElement(Process), new object[] { wxp, path });
                };
            };
            return res;            
        }

        public bool ProcessMulti(string query, string path, ref WXConfig config)
        {
            this.Invoke(new ProcLog(AddToLog), new object[] { String.Format("Принято {1}: {0}", query, path) });
            if (config != null)
                return ProcessMultiSub(query, path, config);

            lock (mult)
            {
                foreach (KeyValuePair<string[], WXConfig> kvp in mult)
                {
                    WXConfig cfg = kvp.Value;
                    if ((path == "TCP") && (cfg["TCP"] == "Нет")) continue;
                    if (!String.IsNullOrEmpty(cfg["APRS_SELECTOR"]))
                    {                        
                        Regex rx = new Regex(cfg["APRS_SELECTOR"], RegexOptions.None);
                        Match mx = rx.Match(query);                        
                        if (mx.Success)
                        {                            
                            this.Invoke(new ProcLog(AddToLog), new object[] { String.Format(" ? Найден селектор: {0} --> {1}", mx.Groups[0].Value, kvp.Key[2]) });
                            return ProcessMultiSub(query, path, config = cfg);
                        };
                    };
                };
            };

            return false;
        }

        public bool ProcessMultiSub(string query, string path, WXConfig config)
        {
            bool res = false;
            foreach (WXConfig.WXParam wxp in config.PARAMS)
            {
                if (wxp.typ != "par") continue;
                Regex rx = new Regex(wxp.value, RegexOptions.None);
                Match mx = rx.Match(query);
                if (mx.Success && (mx.Groups.Count > 1))
                {
                    res = true;
                    wxp.inc_data = mx.Groups[1].Value;
                    if (mx.Groups[wxp.name] != null) wxp.inc_data = mx.Groups[wxp.name].Value;
                    wxp.inc_last = DateTime.UtcNow;
                    this.Invoke(new ProcLog(AddToLog), new object[] { String.Format(" - Разобрано от {2}: {0} = {1}", wxp.name, wxp.inc_data, path) });
                };
            };
            return res;
        }

        public delegate void ProcElement(WXConfig.WXParam wxp, string path);
        public delegate void ProcLog(string value);
        public delegate void ProcLoop(WXConfig wxp);

        public void Process(WXConfig.WXParam wxp, string path)
        {
            AddToLog(String.Format(" - Разобрано от {2}: {0} = {1}", wxp.name, wxp.inc_data, path)); 
            for (int i = 0; i < txprops.Items.Count; i++)
                if (int.Parse(txprops.Items[i].SubItems[0].Text) == wxp.id)
                {
                    txprops.Items[i].SubItems[3].Text = wxp.inc_data;
                    txprops.Items[i].SubItems[4].Text = wxp.inc_last.ToString("HH:mm:ss dd.MM.yyyy UTC");
                };
        }

        public void ProcessLoop(WXConfig config)
        {
            string data = PrepareText(config, false);
            if (config["SEND_WX"] == "Да")
            {
                Upload(config, data);
                AddToLog("Обработка входящих данных с отправкой погоды на сервер завершена");
            }
            else
                AddToLog("Обработка входящих данных без отправки погоды на сервер завершена");
        }

        public void Upload(WXConfig config, string data)
        {
            if (String.IsNullOrEmpty(data)) return;            
            if (String.IsNullOrEmpty(config["APRS_CALLSIGN"])) return;
            if (String.IsNullOrEmpty(config["APRS_SERVER"])) return;
            if (String.IsNullOrEmpty(config["APRS_PORT"])) return;
            if ((String.IsNullOrEmpty(config["APRS_PASSWORD"])) || (config["APRS_PASSWORD"] == "") || (config["APRS_PASSWORD"] == "-1")) return;

            string login = String.Format("user {0} pass {1} vers {3} {2}\r\n", config["APRS_CALLSIGN"], config["APRS_PASSWORD"], MainForm.Version, MainForm.Software);
            string[] lines2send = data.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if ((lines2send == null) || (lines2send.Length == 0)) return;
            for (int i = 0; i < lines2send.Length; i++)
            {
                if (lines2send[i].IndexOf("#") == 0) continue;
                string msg = login + lines2send[i] + "\r\n";
                SendUDP(config["APRS_SERVER"], int.Parse(config["APRS_PORT"]), msg);
                AddToLog(" > Отправлено udp://" + config["APRS_SERVER"] + ":" + config["APRS_PORT"] + "/" + lines2send[i]);
            };
        }

        public void UploadManual(WXConfig config)
        {
            string login = String.Format("user {0} pass {1} vers {3} {2}\r\n", config["APRS_CALLSIGN"], config["APRS_PASSWORD"], MainForm.Version, MainForm.Software);
            string[] lines2send = textBox1.Text.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if ((lines2send == null) || (lines2send.Length == 0))
            {
                AddToLog("Нечего отправлять");
                return;
            };
            if (String.IsNullOrEmpty(config["APRS_CALLSIGN"]))
            {
                AddToLog("Позывной не указан");
                return;
            };
            if ((String.IsNullOrEmpty(config["APRS_PASSWORD"])) || (config["APRS_PASSWORD"] == "") || (config["APRS_PASSWORD"] == "-1"))
            {
                AddToLog("Пароль не указан");
                return;
            };


            string msg = "";
            for (int i = 0; i < lines2send.Length; i++)
            {
                lines2send[i] = lines2send[i].Trim();
                if (lines2send[i].IndexOf("#") == 0) continue;
                msg += lines2send[i] + "\r\n";
            };
            if (msg == "")
            {
                AddToLog("Нечего отправлять");
                return;
            };

            if (MessageBox.Show(msg, "Отправить пакет?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            for (int i = 0; i < lines2send.Length; i++)
            {
                if (lines2send[i].IndexOf("#") == 0) continue;
                msg = login + lines2send[i] + "\r\n";
                SendUDP(config["APRS_SERVER"], int.Parse(config["APRS_PORT"]), msg);
                AddToLog(" > Отправлено udp://" + config["APRS_SERVER"] + ":" + config["APRS_PORT"] + "/" + lines2send[i]);
            };
        }

        public string PrepareText(WXConfig config, bool manual)
        {
            string data = "";
            System.Globalization.CultureInfo cu = System.Globalization.CultureInfo.InvariantCulture;

            
            // location
            if (!String.IsNullOrEmpty(config["APRS_LOCATION"]))
                data += String.Format("{0}>APRS,TCPIP*:[{1:-6}]\r\n", new object[] { config["APRS_CALLSIGN"], config["APRS_LOCATION"] });
            // status
            if (!String.IsNullOrEmpty(config["APRS_STATUS"]))
                data += String.Format("{0}>APRS,TCPIP*:>{1}\r\n", new object[] { config["APRS_CALLSIGN"], config["APRS_STATUS"] });
            // packet
            {
                DateTime dt = DateTime.UtcNow;
                WXConfig.WXParam param;
                {
                    // date time
                    if (((param = config.GetParam("time_utc")) != null) && (param.inc_past <= config.GetIncTime())) DateTime.TryParse(param.inc_data, out dt);
                    if (((param = config.GetParam("time_loc")) != null) && (param.inc_past <= config.GetIncTime()) && (DateTime.TryParse(param.inc_data, out dt))) dt = dt.AddHours(-1 * int.Parse(config["TIME_ZONE"]));
                    if (((param = config.GetParam("date_utc")) != null) && (param.inc_past <= config.GetIncTime())) DateTime.TryParse(param.inc_data, out dt);
                    if (((param = config.GetParam("date_loc")) != null) && (param.inc_past <= config.GetIncTime()) && (DateTime.TryParse(param.inc_data, out dt))) dt = dt.AddHours(-1 * int.Parse(config["TIME_ZONE"]));
                };
                // UB3APB-W1&gt;APRS,TCPIP*:!5531.97N/03730.22E_.../...g...t{T}r...p...P...h{H}b{B}Uzhnoe Butovo WX {TEXT}
                string packet = String.Format("{0}>APRS,TCPIP*:/{5:HHmmss}z{1}{2}{3}{4}", new object[] { config["APRS_CALLSIGN"], config.GetHint("APRS_LATITUDE"), config["APRS_ICON"][0], config.GetHint("APRS_LONGITUDE"), config["APRS_ICON"][1], dt });
                {
                    // wind
                    double wd = 0, ws = 0;
                    if (((param = config.GetParam("wind_dir")) != null) && (param.inc_past <= config.GetIncTime())) double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out wd);
                    if (((param = config.GetParam("wind_mph")) != null) && (param.inc_past <= config.GetIncTime())) double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out ws);
                    if (((param = config.GetParam("wind_kph")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out ws))) ws = ws / 1.61;
                    if (((param = config.GetParam("wind_mps")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out ws))) ws = ws * 3.6 / 1.61;
                    packet += String.Format("{0:000.}/{1:000.}", wd, ws);
                };
                {
                    // all other
                    double x = 0;
                    // gust
                    string g = "g...";
                    if (((param = config.GetParam("gust_mph")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out x))) { g = String.Format("g{0:000.}", x); };
                    if (((param = config.GetParam("gust_kph")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out x))) { x = x / 1.61; g = String.Format("g{0:000.}", x); };
                    if (((param = config.GetParam("gust_mps")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out x))) { x = x * 3.6 / 1.61; g = String.Format("g{0:000.}", x); };
                    packet += g;
                    // temp
                    string t = "t...";
                    if (((param = config.GetParam("temp_fah")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out x))) { t = String.Format("t{0:000.}", x); };
                    if (((param = config.GetParam("temp_cel")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out x))) { x = x * 1.8 + 32; t = String.Format("t{0:000.}", x); };
                    packet += t;
                    // rain
                    if (((param = config.GetParam("rain_h100")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out x))) { packet += String.Format("r{0:000.}", x); };
                    if (((param = config.GetParam("rain_hinc")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out x))) { x = x * 100.0; packet += String.Format("r{0:000.}", x); };
                    if (((param = config.GetParam("rain_hmm")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out x))) { x = x / 25.4 * 100; packet += String.Format("r{0:000.}", x); };
                    if (((param = config.GetParam("rain_d100")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out x))) { packet += String.Format("P{0:000.}", x); };
                    if (((param = config.GetParam("rain_dinc")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out x))) { x = x * 100.0; packet += String.Format("P{0:000.}", x); };
                    if (((param = config.GetParam("rain_dmm")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out x))) { x = x / 25.4 * 100; packet += String.Format("P{0:000.}", x); };
                    if (((param = config.GetParam("rain_n100")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out x))) { packet += String.Format("p{0:000.}", x); };
                    if (((param = config.GetParam("rain_ninc")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out x))) { x = x * 100.0; packet += String.Format("p{0:000.}", x); };
                    if (((param = config.GetParam("rain_nmm")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out x))) { x = x / 25.4 * 100; packet += String.Format("p{0:000.}", x); };
                    // hum
                    if (((param = config.GetParam("humi_per")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out x))) { packet += String.Format("h{0:00.}", x > 99 ? 0 : x); };
                    // press
                    if (((param = config.GetParam("bar_m10")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out x))) { packet += String.Format("b{0:00000.}", x); };
                    if (((param = config.GetParam("bar_mil")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out x))) { x = x * 10; packet += String.Format("b{0:00000.}", x); };
                    if (((param = config.GetParam("bar_bar")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out x))) { x = x * 10000; packet += String.Format("b{0:00000.}", x); };
                    if (((param = config.GetParam("bar_mmh")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out x))) { x = x * 1.33322 * 10; packet += String.Format("b{0:00000.}", x); };
                    //// snow
                    if (((param = config.GetParam("snow_in")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out x))) { packet += String.Format("s{0:000.}", x); };
                    if (((param = config.GetParam("snow_mm")) != null) && (param.inc_past <= config.GetIncTime()) && (double.TryParse(param.inc_data, System.Globalization.NumberStyles.AllowDecimalPoint, cu, out x))) { x = x / 25.4; packet += String.Format("s{0:000.}", x); };
                };
                if (!String.IsNullOrEmpty(config["APRS_COMMENT"]))
                    packet += config["APRS_COMMENT"];
                AddToLog(String.Format("{1}Сформирован пакет: {0}", packet, manual ? "" : " - "));
                data = packet + "\r\n" + data;
            };
            return data;
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Text = PrepareText(config, true);
        }

        private void sendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UploadManual(config);
        }

        private void updateSendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Text = PrepareText(config, true);
            UploadManual(config);
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = ".cfgx";
            ofd.Filter = "XML config (*.cfgx)|*.cfgx";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                config = WXConfig.Load(this, ofd.FileName);
                AfterLoad(false);
                if (config["START"] == "Да")
                {
                    if (MessageBox.Show("Настройки загружены!\r\nЗапустить HTTP сервер?", "Настройки", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        Start();
                };
            };
            ofd.Dispose();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = ".cfgx";
            sfd.Filter = "XML config (*.cfgx)|*.cfgx";
            if (sfd.ShowDialog() == DialogResult.OK)
                config.Save(sfd.FileName);
            sfd.Dispose();
        }

        private void conmnu_DropDownOpening(object sender, EventArgs e)
        {
            int c = conmnu.DropDownItems.Count;
            if (c > 5) for (int i = c - 1; i >= 5; i--) conmnu.DropDownItems.RemoveAt(i);
            string[][] presets = WXConfig.GetPresets();
            if (presets == null) return;

            foreach (string[] preset in presets)
            {
                ToolStripItem tsi = conmnu.DropDownItems.Add(preset[2] + " ...");
                tsi.Tag = preset;
                tsi.Click += new EventHandler(tsi_Click);
            };
        }

        private void tsi_Click(object sender, EventArgs e)
        {
            if (sender == null) return;
            if (!(sender is ToolStripItem)) return;
            ToolStripItem tsi = (ToolStripItem)sender;
            if (tsi.Tag == null) return;
            string[] preset = (string[])tsi.Tag;
            int si = 0;
            if (System.Windows.Forms.InputBox.Show("Селекторные настройки", preset[2] + ":", new string[] { "Загрузить найстройки", "Обновить настройки", "Удалить настройки" }, ref si) != DialogResult.OK) return;
            if (si == 0)
            {
                config = WXConfig.Load(this, preset[0]);
                AfterLoad(false);
                if (config["START"] == "Да")
                {
                    if(MessageBox.Show("Настройки загружены!\r\nЗапустить HTTP сервер?", "Настройки", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        Start();
                }
                else
                    MessageBox.Show("Настройки загружены!", "Настройки", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            }
            else if (si == 1)
            {
                WXConfig.UpdatePresets(config, preset[2], preset[0]);
                MessageBox.Show("Настройки обновлены!", "Настройки", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                try
                {
                    File.Delete(preset[0]);
                    MessageBox.Show("Настройки удалены!", "Настройки", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch { };
            };
        }

        private void savepresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = String.Format("Настройки от {0:HH:mm:ss ddd dd.MM.yyyy}", DateTime.Now);
            if (System.Windows.Forms.InputBox.Show("Сохранение настроек", "Название:", ref text) == DialogResult.OK)
            {
                WXConfig.SetPresets(config, text);
                MessageBox.Show("Настройки сохранены!", "Настройки", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
        }

        private void savelogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = ".txt";
            sfd.Filter = "Text Files(*.txt)|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding(1251));
                sw.Write(csl.Text);
                sw.Close();
                fs.Close();
            };
            sfd.Dispose();
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            csl.Clear();
        }

        private void setdefsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Установить настройки по умолчанию?", "Настройки", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                config = WXConfig.Load(this, XMLSaved<int>.GetCurrentDir()+@"\wxdefault.xml");
                AfterLoad(false);                
            };
        }

        private void runmultiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            runmultiToolStripMenuItem.Checked = !runmultiToolStripMenuItem.Checked;
            if (runmultiToolStripMenuItem.Checked)
            {                
                Start(true);
                runmultiToolStripMenuItem.Text = String.Format("Запущено в селекторном режиме: {0}", mult == null ? 0 : mult.Count);
            }
            else
            {
                runmultiToolStripMenuItem.Text = "Запуск в селекторном режиме";
                Stop();
            };
        }

        private void runsingleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mode > 0)
            {
                Stop();
                int id = config.GetParam("START").id;
                for (int i = 0; i < txprops.Items.Count; i++)
                    if (int.Parse(txprops.Items[i].SubItems[0].Text) == id)
                        txprops.Items[i].SubItems[2].Text = config.GetParam("START").value = "Нет";
            }
            else
            {
                Start();
                int id = config.GetParam("START").id;
                for (int i = 0; i < txprops.Items.Count; i++)
                    if (int.Parse(txprops.Items[i].SubItems[0].Text) == id)
                        txprops.Items[i].SubItems[2].Text = config.GetParam("START").value = "Да";
            };
        }
    }
}