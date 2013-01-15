using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeroMQ;

namespace ZeroMQServer
{
    public partial class ServerForm : Form
    {
        public ServerForm()
        {
            InitializeComponent();
        }

        private void toggleServer_Click(object sender, EventArgs e)
        {
            toggleServer.Enabled = false;
            Task.Factory.StartNew(() => StartListening());
        }

        private void StartListening()
        {
            var listening = true;
            using (var context = ZmqContext.Create())
            using (var server = context.CreateSocket(SocketType.REP))
            {
                server.Bind("tcp://*:5555");
                while (listening)
                {
                    var msg = server.Receive(Encoding.UTF8);
                    if (msg == "stop")
                    {
                        listening = false;
                    }
                    this.BeginInvoke(new Action(() => UpdateText(msg)));
                    server.Send("OK:" + DateTime.Now.ToShortTimeString(), Encoding.UTF8);
                }
            }
            this.BeginInvoke((MethodInvoker)delegate { toggleServer.Enabled = true; });
        }

        private void UpdateText(string text)
        {
            textBox1.Text = text + "\r\n" + textBox1.Text;
        }
    }
}
