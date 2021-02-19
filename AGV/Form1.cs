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

            Thread t = new Thread(() => { AGVConnect(); });
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
            drawAGV(e.Graphics);
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
                if (Route.Contains(qr))
                {
                    p.Color = Color.Green;
                    drawBrush.Color = Color.Green;
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
                            Route.Add(qr);
                        }
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        if (Route.Contains(qr))
                        {
                            Route.RemoveAt(Route.FindLastIndex((QRCode _qr) => { return _qr.TagNum == qr.TagNum; }));
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
            binaryWriter.Write(Encoding.UTF8.GetBytes(str));
            isRunning = true;
            button_Send.Enabled = false;
        }

        private void button_Clear_Click(object sender, EventArgs e)
        {
            Route.Clear();
            pictureBox.Invalidate();
            updateListView();
        }
    }
}


