using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace APRSWXUDPSender
{
    public class WXConfig: XMLSaved<WXConfig>
    {
        public class WXParam
        {
            [XmlAttribute("id")]
            public int id;
            [XmlAttribute("type")]
            public string typ;
            [XmlAttribute("name")]
            public string name;
            [XmlAttribute("caption")]
            public string caption;
            [XmlText]
            public string value;
            [XmlIgnore]
            public string inc_data = null;
            [XmlIgnore]
            public DateTime inc_last = DateTime.MinValue;
            [XmlIgnore]
            public double inc_past { get { return DateTime.UtcNow.Subtract(inc_last).TotalMinutes; } }

            public override string ToString()
            {
                return String.Format("{0}: {1}", caption, value);
            }
        }

        [XmlIgnore]
        private static string cd = XMLSaved<int>.GetCurrentDir() + @"\PRESETS\";

        public static string[][] GetPresets()
        {
            try
            {
                if (!System.IO.Directory.Exists(cd))
                    System.IO.Directory.CreateDirectory(cd);
                if (!System.IO.Directory.Exists(cd))
                    return null;
            }
            catch { return null; };

            string[] files = System.IO.Directory.GetFiles(cd, "*.cfgx");
            if (files == null) return null;
            if (files.Length == 0) return null;

            Regex rx = new Regex(@"\<\!--\sNAME:\s(.*?)\s--\>");
            List<string[]> res = new List<string[]>();
            for (int i = 0; i < files.Length; i++)
            {
                string fn = System.IO.Path.GetFileNameWithoutExtension(files[i]);
                string pn = fn;
                try
                {
                    System.IO.FileStream fs = new System.IO.FileStream(files[i], System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    System.IO.StreamReader sr = new System.IO.StreamReader(fs, System.Text.Encoding.UTF8);
                    string config = sr.ReadToEnd();
                    sr.Close();
                    fs.Close();
                    Match mx = rx.Match(config);
                    if (mx.Success) pn = mx.Groups[1].Value;
                    res.Add(new string[] { files[i], fn, pn });
                }
                catch { };
            };
            return res.ToArray();
        }

        public static void SetPresets(WXConfig preset, string name)
        {
            while (name.IndexOf("--") >= 0) name = name.Replace("--", "-");
            try
            {
                if (!System.IO.Directory.Exists(cd))
                    System.IO.Directory.CreateDirectory(cd);
                if (!System.IO.Directory.Exists(cd))
                    return;
            }
            catch { return; };

            string file = String.Format("{1}{0:yyyyMMddHHmmss}UTC.cfgx", DateTime.UtcNow, cd);
            string text = WXConfig.Save(preset);
            text = text.Replace("</WXConfig>", "<!-- NAME: " + name + " -->\r\n</WXConfig>");
            try
            {
                System.IO.FileStream fs = new System.IO.FileStream(file, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.UTF8);
                sw.Write(text);
                sw.Close();
                fs.Close();
            }
            catch { };
        }

        public static void UpdatePresets(WXConfig preset, string name, string file)
        {
            while (name.IndexOf("--") >= 0) name = name.Replace("--", "-");
            string text = WXConfig.Save(preset);
            text = text.Replace("</WXConfig>", "<!-- NAME: " + name + " -->\r\n</WXConfig>");
            try
            {
                System.IO.FileStream fs = new System.IO.FileStream(file, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.UTF8);
                sw.Write(text);
                sw.Close();
                fs.Close();
            }
            catch { };
        }

        [XmlIgnore]
        private static string file = XMLSaved<WXConfig>.GetCurrentDir() + @"\wxconfig.xml";

        public static WXConfig Load(SLV parent)
        {
            WXConfig res = new WXConfig();
            try
            {
                res = XMLSaved<WXConfig>.Load(file);
            }
            catch { };
            res.LoadView(parent);
            return res;
        }

        public static WXConfig Load(SLV parent, string file)
        {
            WXConfig res = new WXConfig();
            try
            {
                res = XMLSaved<WXConfig>.Load(file);
            }
            catch { };
            res.LoadView(parent);
            return res;
        }

        public void Save()
        {
            try
            {
                XMLSaved<WXConfig>.Save(file, this);
            }
            catch { };
        }

        public void Save(string file)
        {
            try
            {
                XMLSaved<WXConfig>.Save(file, this);
            }
            catch { };
        }

        private void LoadView(SLV parent)
        {
            if (parent == null) return;
            if (parent.GetListView() == null) return;
            lock (parent.GetListView())
            {
                parent.GetListView().Items.Clear();
                for (int i = 0; i < PARAMS.Length; i++)
                {
                    System.Windows.Forms.ListViewItem lvi = parent.GetListView().Items.Add(PARAMS[i].id.ToString("00"));
                    lvi.SubItems.Add(PARAMS[i].caption + ":");
                    lvi.SubItems.Add(PARAMS[i].value);
                    lvi.SubItems.Add(GetHint(PARAMS[i].id));
                    lvi.SubItems.Add("");
                };
            };
        }

        [XmlElement("PARAM")]
        public WXParam[] PARAMS = new WXParam[0];

        [XmlIgnore]
        public string this[string name]
        {
            get
            {
                foreach (WXParam param in PARAMS)
                    if (param.name == name)
                        return param.value;
                return "";
            }
            set
            {
                foreach (WXParam param in PARAMS)
                    if (param.name == name)
                        param.value = value;
            }
        }

        [XmlIgnore]
        public string this[int id]
        {
            get
            {
                foreach (WXParam param in PARAMS)
                    if (param.id == id)
                        return param.value;
                return "";
            }
            set
            {
                foreach (WXParam param in PARAMS)
                    if (param.id == id)
                        param.value = value;
            }
        }

        public string GetType(int id)
        {
            foreach (WXParam param in PARAMS)
                if (param.id == id)
                    return param.typ;
            return "string";
        }

        public string GetCaption(int id)
        {
            foreach (WXParam param in PARAMS)
                if (param.id == id)
                    return param.caption;
            return "";
        }

        public string GetName(int id)
        {
            foreach (WXParam param in PARAMS)
                if (param.id == id)
                    return param.name;
            return "";
        }

        public string GetHint(int id)
        {
            foreach (WXParam param in PARAMS)
                if (param.id == id)
                {
                    if (param.typ == "loc") return String.Format("{0,-6}", param.value);
                    if (param.typ == "lat")
                    {
                        double rdb = KMZRebuilder.LatLonParser.Parse(param.value, true);
                        return  Math.Truncate(rdb).ToString("00") + ((rdb - Math.Truncate(rdb)) * 60).ToString("00.00").Replace(",", ".") + (rdb > 0 ? "N" : "S");
                    };
                    if (param.typ == "lon")
                    {
                        double rdb = KMZRebuilder.LatLonParser.Parse(param.value, false);
                        return Math.Truncate(rdb).ToString("000") + ((rdb - Math.Truncate(rdb)) * 60).ToString("00.00").Replace(",", ".") + (rdb > 0 ? "E" : "W");
                    };
                };
            return "";
        }

        public string GetHint(string name)
        {
            foreach (WXParam param in PARAMS)
                if (param.name == name)
                {
                    if (param.typ == "loc") return String.Format("{0,-6}", param.value);
                    if (param.typ == "lat")
                    {
                        double rdb = KMZRebuilder.LatLonParser.Parse(param.value, true);
                        return Math.Truncate(rdb).ToString("00") + ((rdb - Math.Truncate(rdb)) * 60).ToString("00.00").Replace(",", ".") + (rdb > 0 ? "N" : "S");
                    };
                    if (param.typ == "lon")
                    {
                        double rdb = KMZRebuilder.LatLonParser.Parse(param.value, false);
                        return Math.Truncate(rdb).ToString("000") + ((rdb - Math.Truncate(rdb)) * 60).ToString("00.00").Replace(",", ".") + (rdb > 0 ? "E" : "W");
                    };
                };
            return "";
        }

        public WXParam GetParam(int id)
        {
            foreach (WXParam param in PARAMS)
                if (param.id == id)
                    return param;
            return null;
        }

        public WXParam GetParam(string name)
        {
            foreach (WXParam param in PARAMS)
                if (param.name == name)
                    return param;
            return null;
        }

        public long GetIncTime()
        {
            try { return long.Parse(this["INC_TIME"]); }
            catch { return 60; };
        }
        public long GetIncLoop()
        {
            try { return long.Parse(this["INC_LOOP"]); }
            catch { return 15; };
        }
    }

    public interface SLV
    {
        System.Windows.Forms.ListView GetListView();
    }
}
