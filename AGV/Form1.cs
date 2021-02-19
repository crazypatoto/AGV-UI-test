using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace AGV
{

    public partial class Form1 : Form
    {
        List<QRCode> QRCodes = new List<QRCode>();
        List<QRCode> Route = new List<QRCode>();
        Int32 OriginOffset_X = 400;
        Int32 OriginOffset_Y = 400;
        float positionScale = 0.1f;
        TcpListener serverTcpListener = null;
        TcpClient tcpClient;
        BinaryWriter binaryWriter;
        Int16 odmX = 0;
        Int16 odmY = 0;
        Int16 odmAngle = 0;
        bool isRunning = false;

        AGVHandler AGVhandler;
        int pickedAGVIndex = -1;
        int prevAGVCount = 0;
        QRCode pickedQR = null;

        public Form1()
        {
            loadQRCode();
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            listView_AGV.View = View.Details;
            listView_AGV.GridLines = true;
            listView_AGV.FullRowSelect = true;
            //listView_AGV.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

            //Thread t = new Thread(() => { AGVConnect(); });
            //t.IsBackground = true;
            //t.Start();

            TcpListener serverTcpListener = new TcpListener(IPAddress.Any, 6666);
            AGVhandler = new AGVHandler(serverTcpListener);

            panel1.Enabled = true;

            Thread t = new Thread(() =>
            {
                while (true)
                {
                    AGVhandler.AcceptNewConnection();
                }
            });
            t.IsBackground = true;
            t.Start();

        }

        public string GetHostName(string ipAddress)
        {
            try
            {
                Console.WriteLine(ipAddress);
                IPHostEntry entry = Dns.GetHostEntry(ipAddress);

                if (entry != null)
                {
                    Console.WriteLine(entry.HostName);
                    return entry.HostName;
                }
            }
            catch (SocketException ex)
            {
                //unknown host or
                //not every IP has a name
                //log exception (manage it)
            }

            return null;
        }

        private void AGVConnect()
        {
            BinaryReader binaryReader;
            try
            {
                serverTcpListener = new TcpListener(IPAddress.Any, 6666);
                Console.WriteLine($"Starting TCP Server with port {6666}");
                serverTcpListener.Start();
                tcpClient = serverTcpListener.AcceptTcpClient();
                serverTcpListener.Stop();
                var hostname = GetHostName(((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString());
                this.Invoke(new MethodInvoker(() =>
                {
                    this.Text += " - ";
                    this.Text += hostname;
                }));
                binaryReader = new BinaryReader(tcpClient.GetStream());
                binaryWriter = new BinaryWriter(tcpClient.GetStream());
                this.Invoke(new MethodInvoker(() =>
                {
                    panel1.Enabled = true;
                }));

                Thread t = new Thread(() =>
                {
                    while (true)
                    {
                        if (tcpClient.Connected)
                        {
                            try
                            {
                                odmX = binaryReader.ReadInt16();
                                odmY = binaryReader.ReadInt16();
                                odmAngle = binaryReader.ReadInt16();

                                Console.WriteLine($"X = {odmX}, Y = {odmY}, Angle = {odmAngle}");

                                if (isRunning)
                                {
                                    if (Route.Count > 1)
                                    {
                                        QRCode qr = Route[1];
                                        if (new Rectangle((int)(qr.X * positionScale + OriginOffset_X - 15), (int)(-qr.Y * positionScale + OriginOffset_Y - 15), 30, 30).Contains((int)(odmX * positionScale + OriginOffset_X), (int)(-odmY * positionScale + OriginOffset_Y)))
                                        {
                                            Route.RemoveAt(0);
                                            this.Invoke(new MethodInvoker(() =>
                                            {
                                                listView1.Items.RemoveAt(0);
                                            }));

                                        }
                                    }
                                    else
                                    {
                                        QRCode qr = Route[0];
                                        if (new Rectangle((int)(qr.X * positionScale + OriginOffset_X - 15), (int)(-qr.Y * positionScale + OriginOffset_Y - 15), 30, 30).Contains((int)(odmX * positionScale + OriginOffset_X), (int)(-odmY * positionScale + OriginOffset_Y)))
                                        {
                                            Route.RemoveAt(0);
                                            this.Invoke(new MethodInvoker(() =>
                                            {
                                                listView1.Items.RemoveAt(0);
                                                button_Send.Enabled = true;
                                            }));

                                        }
                                        isRunning = false;
                                    }

                                }

                                pictureBox.Invalidate();
                            }
                            catch (Exception)
                            {

                            }
                        }
                        else
                        {
                            Environment.Exit(0);
                        }

                    }
                });
                t.IsBackground = true;
                t.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(-1);
            }
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            drawQR(e.Graphics);
            drawRoute(e.Graphics);
            //drawAGV(e.Graphics);
            drawAGVs(e.Graphics);
            drawAGVRoutes(e.Graphics);
        }

        private void updateListView()
        {
            int count = 1;
            listView1.Items.Clear();
            foreach (QRCode qr in Route)
            {
                ListViewItem lvitem = new ListViewItem(count++.ToString());
                lvitem.SubItems.Add(qr.TagNum.ToString());
                lvitem.SubItems.Add(qr.X.ToString());
                lvitem.SubItems.Add(qr.Y.ToString());
                listView1.Items.Add(lvitem);
            }
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void drawQR(Graphics g)
        {
            Pen p = new Pen(Color.Black, 3);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            Font drawFont = new Font("Arial", 12);

            foreach (QRCode qr in QRCodes)
            {
                //g.DrawEllipse(p, qr.X/5, -qr.Y/5, 100, 100);
                if (pickedQR == qr && pickedAGVIndex >= 0)
                {
                    p.Color = AGVhandler.AGVList[pickedAGVIndex].color;
                    drawBrush.Color = AGVhandler.AGVList[pickedAGVIndex].color;
                }
                else
                {
                    p.Color = Color.Black;
                    drawBrush.Color = Color.Black;
                }

                g.DrawRectangle(p, (qr.X * positionScale) + OriginOffset_X - 15, (-qr.Y * positionScale) + OriginOffset_Y - 15, 30, 30);
                g.DrawEllipse(p, (qr.X * positionScale) + OriginOffset_X - 3, (-qr.Y * positionScale) + OriginOffset_Y - 3, 6, 6);
                g.DrawString($"Tag {qr.TagNum}", drawFont, drawBrush, (qr.X * positionScale) + OriginOffset_X - 16, (-qr.Y * positionScale) + OriginOffset_Y + 16);

            }

        }
        private void drawAGV(Graphics g)
        {
            Pen p = new Pen(Color.Red, 3);
            Pen p2 = new Pen(Color.Red, 3);
            p2.EndCap = LineCap.RoundAnchor;
            p2.EndCap = LineCap.ArrowAnchor;

            float x = (odmX * positionScale) + OriginOffset_X;
            float y = (-odmY * positionScale) + OriginOffset_Y;

            if (QRCodes.Count > 0)
            {
                g.DrawEllipse(p, x - 30, y - 30, 60, 60);
                g.DrawLine(p2, x, y, (float)(x + Math.Cos(odmAngle / 180.0 * Math.PI) * 60.0f), (float)(y - Math.Sin(odmAngle / 180.0 * Math.PI) * 60.0f));
            }
        }

        private void drawAGVs(Graphics g)
        {
            foreach (var agv in AGVhandler.AGVList)
            {
                Pen p = new Pen(agv.color, 3);
                Pen p2 = new Pen(agv.color, 3);
                p2.EndCap = LineCap.RoundAnchor;
                p2.EndCap = LineCap.ArrowAnchor;

                //Console.WriteLine($"{agv.hostName}: X = {agv.odmX}, Y = {agv.odmY}, Angle = {agv.odmAngle}");

                float x = (agv.odmX * positionScale) + OriginOffset_X;
                float y = (-agv.odmY * positionScale) + OriginOffset_Y;

                g.DrawEllipse(p, x - 30, y - 30, 60, 60);
                g.DrawLine(p2, x, y, (float)(x + Math.Cos(agv.odmAngle / 180.0 * Math.PI) * 60.0f), (float)(y - Math.Sin(agv.odmAngle / 180.0 * Math.PI) * 60.0f));
                g.DrawString(agv.hostName, new Font("Arial", 8), new SolidBrush(agv.color), new PointF(x, y - 50));
            }

        }

        private void drawRoute(Graphics g)
        {
            Pen p = new Pen(Color.Green, 5);
            p.EndCap = LineCap.RoundAnchor;
            p.EndCap = LineCap.ArrowAnchor;

            for (int i = 0; i < Route.Count - 1; i++)
            {
                g.DrawLine(p, Route[i].X * positionScale + OriginOffset_X, -Route[i].Y * positionScale + OriginOffset_Y, Route[i + 1].X * positionScale + OriginOffset_X, -Route[i + 1].Y * positionScale + OriginOffset_Y);
            }
        }

        private void drawAGVRoutes(Graphics g)
        {
            foreach (var agv in AGVhandler.AGVList)
            {
                Pen p = new Pen(agv.color, 5);
                p.EndCap = LineCap.RoundAnchor;
                p.EndCap = LineCap.ArrowAnchor;

                for (int i = 0; i < agv.Route.Count - 1; i++)
                {
                    g.DrawLine(p, agv.Route[i].X * positionScale + OriginOffset_X, -agv.Route[i].Y * positionScale + OriginOffset_Y, agv.Route[i + 1].X * positionScale + OriginOffset_X, -agv.Route[i + 1].Y * positionScale + OriginOffset_Y);
                }
            }
        }

        private void loadQRCode()
        {
            QRCodes.Clear();
            string[] lines = System.IO.File.ReadAllLines(@"QRCodes.txt");

            foreach (string ln in lines)
            {
                string[] elements = ln.Split(',');
                QRCode qRCode_new = new QRCode(Int16.Parse(elements[1]), Int16.Parse(elements[2]), UInt32.Parse(elements[0]));
                QRCodes.Add(qRCode_new);
            }

        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                OriginOffset_Y += 10;
                pictureBox.Invalidate();
                return;
            }
            foreach (QRCode qr in QRCodes)
            {
                if (new Rectangle((int)(qr.X * positionScale + OriginOffset_X - 15), (int)(-qr.Y * positionScale + OriginOffset_Y - 15), 30, 30).Contains(e.X, e.Y))
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        if (Route.Count == 0 || Route.Last() != qr)
                        {
                            if (pickedAGVIndex >= 0)
                            {
                                pickedQR = qr;
                                AGVhandler.AGVList[pickedAGVIndex].Route.Add(qr);
                            }
                            //Route.Add(qr);
                        }
                    }
                    else if (e.Button == MouseButtons.Right)
                    {

                        if (pickedAGVIndex >= 0)
                        {
                            if (AGVhandler.AGVList[pickedAGVIndex].Route.Contains(qr))
                            {
                                AGVhandler.AGVList[pickedAGVIndex].Route.RemoveAt(AGVhandler.AGVList[pickedAGVIndex].Route.FindLastIndex((QRCode _qr) => { return _qr.TagNum == qr.TagNum; }));
                                //Route.RemoveAt(Route.FindLastIndex((QRCode _qr) => { return _qr.TagNum == qr.TagNum; }));
                            }
                        }
                    }
                    break;
                }
            }
            pictureBox.Invalidate();
            updateListView();
        }

        private void button_Send_Click(object sender, EventArgs e)
        {
            //string str = "";

            //if (Route.Count > 0)
            //{
            //    str += $"{Route[0].TagNum},{Route[0].X},{Route[0].Y}";
            //}
            //for (int i = 1; i < Route.Count; i++)
            //{
            //    str += $";{Route[i].TagNum},{Route[i].X},{Route[i].Y}";
            //}
            ////MessageBox.Show(str);
            //binaryWriter.Write(Encoding.UTF8.GetBytes(str));
            //isRunning = true;
            //button_Send.Enabled = false;

            if (pickedAGVIndex >= 0)
            {
                pickedQR = null;                
                AGVhandler.AGVList[pickedAGVIndex].sendRoute();
                pickedAGVIndex = -1;
            }
        }

        private void button_Clear_Click(object sender, EventArgs e)
        {
            if(pickedAGVIndex >= 0)
            {
                AGVhandler.AGVList[pickedAGVIndex].Route.Clear();
            }            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox.Invalidate();
        }

        private void timer_Tick(object sender, EventArgs e)
        {

            if (AGVhandler.AGVList.Count != prevAGVCount)
            {
                prevAGVCount = AGVhandler.AGVList.Count;
                listView_AGV.Items.Clear();
                foreach (var agv in AGVhandler.AGVList)
                {
                    ListViewItem lvitem = new ListViewItem(agv.hostName);
                    lvitem.SubItems.Add(((IPEndPoint)agv.tcpClient.Client.RemoteEndPoint).Address.ToString());
                    if (agv.isRunning)
                    {
                        lvitem.SubItems.Add("Running...");
                    }
                    else
                    {
                        lvitem.SubItems.Add("Idle");
                    }
                    listView_AGV.Items.Add(lvitem);
                }
            }

            for (int i = 0; i < listView_AGV.Items.Count; i++)
            {
                ListViewItem item = listView_AGV.Items[i];
                if (AGVhandler.AGVList[i].isRunning)
                {
                    item.SubItems[2].Text = "Running...";
                }
                else
                {
                    item.SubItems[2].Text = "Idle";
                }
            }
            pictureBox.Invalidate();
        }

        private void listView_AGV_Click(object sender, EventArgs e)
        {
            pickedQR = null;
            if (listView_AGV.SelectedItems.Count > 0)
            {
                pickedAGVIndex = listView_AGV.SelectedItems[0].Index;
            }

        }
    }

    public class AGVHandler
    {
        private TcpListener tcpListener;

        public AGVHandler(TcpListener _listener)
        {
            tcpListener = _listener;
        }

        public List<AGV> AGVList = new List<AGV>();
        public int AGVCount { get; private set; } = 0;

        public void AcceptNewConnection()
        {
            TcpClient client;

            lock (this)
            {
                Console.WriteLine("Accepting new client!!");
                tcpListener.Start();
                tcpListener.Start();
                client = tcpListener.AcceptTcpClient();
                tcpListener.Stop();
            }

            AGV agv = new AGV(
                GetHostName(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString()),
                client,
                new BinaryReader(client.GetStream()),
                new BinaryWriter(client.GetStream())
                );
            AGVList.Add(agv);
            AGVCount++;
            Console.WriteLine($"{agv.hostName} is Connected!");

            HandleAGV(agv);
        }

        private void HandleAGV(AGV agv)
        {

            Thread t = new Thread(() =>
            {
                while (agv.tcpClient.Connected)
                {
                    try
                    {
                        agv.odmX = agv.clientBinaryReader.ReadInt16();
                        agv.odmY = agv.clientBinaryReader.ReadInt16();
                        agv.odmAngle = agv.clientBinaryReader.ReadInt16();

                        //Console.WriteLine($"X = {agv.odmX}, Y = {agv.odmY}, Angle = {agv.odmAngle}");
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine(ex);
                        break;
                    }

                    if (agv.isRunning)
                    {
                        if (agv.Route.Count > 1)
                        {
                            QRCode qr = agv.Route[1];
                            if (new Rectangle((int)(qr.X - 50), (int)(qr.Y - 50), 100, 100).Contains((int)(agv.odmX), (int)(agv.odmY)))
                            {
                                agv.Route.RemoveAt(0);
                            }
                        }
                        else
                        {
                            QRCode qr = agv.Route[0];
                            if (new Rectangle((int)(qr.X - 50), (int)(qr.Y - 50), 100, 100).Contains((int)(agv.odmX), (int)(agv.odmY)))
                            {
                                agv.Route.RemoveAt(0);
                            }
                            agv.isRunning = false;
                        }

                    }
                }
                Console.WriteLine($"\r\nClient {agv.hostName} is Disconnected!");
                AGVList.Remove(agv);
                AGVCount--;
            });

            t.IsBackground = true;
            t.Start();
        }

        private string GetHostName(string ipAddress)
        {
            try
            {
                Console.WriteLine(ipAddress);
                IPHostEntry entry = Dns.GetHostEntry(ipAddress);

                if (entry != null)
                {
                    Console.WriteLine(entry.HostName);
                    return entry.HostName;
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex);
            }
            return "";
        }
    }

    public class QRCode
    {
        public QRCode(Int16 x, Int16 y, UInt32 tagNum)
        {
            X = x;
            Y = y;
            TagNum = tagNum;
        }

        public Int16 X { get; }
        public Int16 Y { get; }
        public UInt32 TagNum { get; }
        public override string ToString() => $"{X},{Y},{TagNum}";
    }

    public class AGV
    {
        public AGV(string _hostName, TcpClient _tcpClient, BinaryReader _clientBinaryReader, BinaryWriter _clientBinaryWriter)
        {
            hostName = _hostName;
            hostName = hostName.Replace(".local", "");
            tcpClient = _tcpClient;
            clientBinaryReader = _clientBinaryReader;
            clientBinaryWriter = _clientBinaryWriter;

            switch (hostName)
            {
                case "AGV001":
                    color = Color.Red;
                    break;
                case "AGV002":
                    color = Color.Green;
                    break;
                case "AGV003":
                    color = Color.Blue;
                    break;
                default:
                    color = Color.Black;
                    break;
            }
        }
        public string hostName { get; private set; } = "";
        public TcpClient tcpClient { get; private set; }
        public BinaryReader clientBinaryReader { get; private set; }
        public BinaryWriter clientBinaryWriter { get; private set; }
        public List<QRCode> Route { get; set; } = new List<QRCode>();
        public Int16 odmX { get; set; }
        public Int16 odmY { get; set; }
        public Int16 odmAngle { get; set; }
        public Color color { get; set; } = Color.Black;
        public bool isRunning { get; set; } = false;

        public void sendRoute()
        {
            string str = "";

            if (Route.Count > 0)
            {
                str += $"{Route[0].TagNum},{Route[0].X},{Route[0].Y}";
            }
            for (int i = 1; i < Route.Count; i++)
            {
                str += $";{Route[i].TagNum},{Route[i].X},{Route[i].Y}";
            }
            //MessageBox.Show(str);
            clientBinaryWriter.Write(Encoding.UTF8.GetBytes(str));
            isRunning = true;
        }
    }

    class ListViewNF : System.Windows.Forms.ListView
    {
        public ListViewNF()
        {
            //Activate double buffering
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            //Enable the OnNotifyMessage event so we get a chance to filter out 
            // Windows messages before they get to the form's WndProc
            this.SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        protected override void OnNotifyMessage(Message m)
        {
            //Filter out the WM_ERASEBKGND message
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }
    }
}


