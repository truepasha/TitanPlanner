using MissionPlanner.Comms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class ConnectionControl : UserControl
    {
        private const int LayoutPadding = 2;
        private const int LayoutSpacing = 6;
        private readonly Label lblGcsId = new Label();

        public ConnectionControl()
        {
            InitializeComponent();
            lblGcsId.AutoSize = true;
            lblGcsId.BackColor = Color.Transparent;
            lblGcsId.ForeColor = Color.White;
            Controls.Add(lblGcsId);
            this.linkLabel1.Click += (sender, e) =>
            {
                ShowLinkStats?.Invoke(this, EventArgs.Empty);
            };
            Resize += (sender, args) => LayoutLinkStatsRow();
            MinimumSize = new Size(300, 48);
            Size = new Size(300, 48);
            UpdateGcsIdLabel();
            LayoutLinkStatsRow();
        }

        public event EventHandler ShowLinkStats;

        public ComboBox CMB_baudrate
        {
            get { return this.cmb_Baud; }
        }

        public ComboBox CMB_serialport
        {
            get { return this.cmb_Connection; }
        }


        /// <summary>
        /// Called from the main form - set whether we are connected or not currently.
        /// UI will be updated accordingly
        /// </summary>
        /// <param name="isConnected">Whether we are connected</param>
        public void IsConnected(bool isConnected)
        {
            this.linkLabel1.Visible = isConnected;
            cmb_Baud.Enabled = !isConnected;
            cmb_Connection.Enabled = !isConnected;

            UpdateSysIDS();
            UpdateGcsIdLabel();
            LayoutLinkStatsRow();
        }

        private void ConnectionControl_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void LayoutLinkStatsRow()
        {
            var desiredSysFieldWidth = Math.Max(180, TextRenderer.MeasureText(cmb_sysid.Text ?? string.Empty, cmb_sysid.Font).Width + 30);
            var desiredControlWidth = Math.Max(MinimumSize.Width, desiredSysFieldWidth + 120);
            if (Width < desiredControlWidth)
            {
                Width = Math.Min(420, desiredControlWidth);
            }

            var rowHeight = Math.Max(cmb_Connection.Height, cmb_Baud.Height);
            var top = LayoutPadding;
            var contentWidth = Math.Max(200, Width - (LayoutPadding * 2));

            var col1Width = Math.Max(90, lblGcsId.GetPreferredSize(Size.Empty).Width + 6);
            var col3Width = Math.Max(90, cmb_Baud.Width);
            var col2Width = Math.Max(80, contentWidth - col1Width - col3Width - (LayoutSpacing * 2));

            lblGcsId.Left = LayoutPadding;
            lblGcsId.Top = top + (rowHeight - lblGcsId.Height) / 2;

            cmb_Connection.Left = lblGcsId.Right + LayoutSpacing;
            cmb_Connection.Top = top;
            cmb_Connection.Width = col2Width;

            cmb_Baud.Left = cmb_Connection.Right + LayoutSpacing;
            cmb_Baud.Top = top;
            cmb_Baud.Width = col3Width;

            var secondRowTop = cmb_Connection.Bottom + 2;
            linkLabel1.AutoSize = true;
            linkLabel1.Left = LayoutPadding;
            linkLabel1.Top = secondRowTop + (rowHeight - linkLabel1.Height) / 2;

            cmb_sysid.Left = lblGcsId.Right + LayoutSpacing;
            cmb_sysid.Top = secondRowTop;
            cmb_sysid.Width = Math.Max(80, (LayoutPadding + contentWidth) - cmb_sysid.Left);
            cmb_sysid.DropDownWidth = Math.Max(cmb_sysid.Width, desiredSysFieldWidth);
            Height = cmb_sysid.Bottom + LayoutPadding;
        }

        private void UpdateGcsIdLabel()
        {
            lblGcsId.Text = $"GCS ID: {MAVLinkInterface.gcssysid}";
        }

        private void cmb_Connection_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            ComboBox combo = sender as ComboBox;
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight),
                    e.Bounds);
            else
                e.Graphics.FillRectangle(new SolidBrush(combo.BackColor),
                    e.Bounds);

            string text = combo.Items[e.Index].ToString();
            if (!MainV2.MONO)
            {
                text = text + " " + SerialPort.GetNiceName(text);
            }

            e.Graphics.DrawString(text, e.Font,
                new SolidBrush(combo.ForeColor),
                new Point(e.Bounds.X, e.Bounds.Y));

            e.DrawFocusRectangle();
        }

        public void UpdateSysIDS()
        {
            cmb_sysid.SelectedIndexChanged -= CMB_sysid_SelectedIndexChanged;

            var oldidx = cmb_sysid.SelectedIndex;

            cmb_sysid.Items.Clear();

            int selectidx = -1;

            foreach (var port in MainV2.Comports.ToArray())
            {
                var list = port.MAVlist.GetRawIDS();

                foreach (int item in list)
                {
                    var temp = new port_sysid() { compid = (item % 256), sysid = (item / 256), port = port };

                    // exclude GCS's from the list
                    if (temp.compid == (int)MAVLink.MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER)
                        continue;

                    var idx = cmb_sysid.Items.Add(temp);

                    if (temp.port == MainV2.comPort && temp.sysid == MainV2.comPort.sysidcurrent && temp.compid == MainV2.comPort.compidcurrent)
                    {
                        selectidx = idx;
                    }
                }
            }

            if (/*oldidx == -1 && */ selectidx != -1)
            {
                cmb_sysid.SelectedIndex = selectidx;
            }

            cmb_sysid.SelectedIndexChanged += CMB_sysid_SelectedIndexChanged;
        }

        internal struct port_sysid
        {
            internal MAVLinkInterface port;
            internal int sysid;
            internal int compid;
        }

        private void CMB_sysid_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmb_sysid.SelectedItem == null)
                return;

            var temp = (port_sysid)cmb_sysid.SelectedItem;

            foreach (var port in MainV2.Comports)
            {
                if (port == temp.port)
                {
                    MainV2.comPort = port;
                    MainV2.comPort.sysidcurrent = temp.sysid;
                    MainV2.comPort.compidcurrent = temp.compid;

                    if (MainV2.comPort.MAV.param.TotalReceived < MainV2.comPort.MAV.param.TotalReported && 
                        /*MainV2.comPort.MAV.compid == (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_AUTOPILOT1 && */
                        !(Control.ModifierKeys == Keys.Control))
                        MainV2.comPort.getParamList();

                    MainV2.View.Reload();
                }
            }
        }

        private void cmb_sysid_Format(object sender, ListControlConvertEventArgs e)
        {
            var temp = (port_sysid)e.Value;
            MAVLink.MAV_COMPONENT compid = (MAVLink.MAV_COMPONENT)temp.compid;
            string mavComponentHeader = "MAV_COMP_ID_";
            string mavComponentString = null;

            foreach (var port in MainV2.Comports)
            {
                if (port == temp.port)
                {
                    if (compid == (MAVLink.MAV_COMPONENT)1)
                    {
                        //use Autopilot type as displaystring instead of "FCS1"
                        mavComponentString = port.MAVlist[temp.sysid, temp.compid].aptype.ToString();
                    }
                    else
                    {
                        //use name from enum if it exists, use the component ID otherwise
                        mavComponentString = compid.ToString();
                        if (mavComponentString.Length > mavComponentHeader.Length)
                        {
                            //remove "MAV_COMP_ID_" header
                            mavComponentString = mavComponentString.Remove(0, mavComponentHeader.Length);
                        }

                        if (temp.port.MAVlist[temp.sysid, temp.compid].CANNode)
                            mavComponentString =
                                temp.compid + " " + temp.port.MAVlist[temp.sysid, temp.compid].VersionString;
                    }
                    e.Value = temp.port.BaseStream.PortName + "-" + ((int)temp.sysid) + "-" + mavComponentString.Replace("_", " ");
                }
            }
        }
    }
}
