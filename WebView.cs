using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Web;

namespace APRSWXUDPSender
{
    public class WebView : SimpleServersPBAuth.ThreadedHttpServer
    {
        private WevViewRes parent = null;
        public bool AnalyzeNotHTTP = true;

        public WebView(int Port, WevViewRes parent) : base(Port) 
        { 
            this.parent = parent;
        }
        ~WebView() { this.Dispose(); }

        protected override void GetClientRequest(ClientRequest Request)
        {
            if ((parent != null) && (parent.OnQuery(null, Request.Query, Request.BodyText, "HTTP")))
                HttpClientSendError(Request.Client, 200);
            else
                HttpClientSendError(Request.Client, 406);
        }

        protected override void onBadClient(TcpClient Client, ulong id, byte[] Request)        
        {
            if (!AnalyzeNotHTTP) return;

            string query = System.Text.Encoding.ASCII.GetString(Request);
            int bRead = -1;
            int receivedBytes = Request.Length;
            int posCRLF = -1;
            
            try
            {
                Client.GetStream().ReadTimeout = 3000;
                while ((bRead = Client.GetStream().ReadByte()) >= 0)
                {
                    receivedBytes++;
                    query += (char)bRead; // standard symbol
                    if (bRead == 0x0A) posCRLF = query.IndexOf("\r\n"); // get end
                    if (posCRLF >= 0 || Request.Length > _MaxHeaderSize) { break; };
                };
            }
            catch { };

            if ((parent != null) && (parent.OnQuery(query.Trim(), null, null, "TCP")))
            {
                Client.GetStream().Write(new byte[] {0x32, 0x30, 0x30}, 0, 3);
                Client.GetStream().Flush();
            }
            else
            {
                Client.GetStream().Write(new byte[] {0x34, 0x30, 0x36}, 0, 3);
                Client.GetStream().Flush();
            };
        }
    }

    public interface WevViewRes
    {
        bool OnQuery(string full, string get, string post, string path);
    }
}
