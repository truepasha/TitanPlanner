using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using DirectShowLib;
using MissionPlanner.Utilities;
using WebCamService;

namespace MissionPlanner.Controls
{
    public class FlightPlannerVideoOptions : UserControl
    {
        private Label labelVideoDevice;
        private Label labelVideoFormat;
        private Label labelOsdColor;
        private Label labelStreamUrl;
        private Label labelGStreamer;
        private ComboBox cmbVideoSources;
        private ComboBox cmbVideoResolutions;
        private ComboBox cmbOsdColor;
        private TextBox txtGStreamerSource;
        private TextBox txtStreamUrl;
        private MyButton btnStreamStart;
        private MyButton btnStreamStop;
        private MyButton btnGStreamerStart;
        private MyButton btnGStreamerStop;
        private MyButton btnVideoStart;
        private MyButton btnVideoStop;
        private CheckBox chkHudShow;
        private CheckBox chkStreamFillHud;
        private bool startup = true;
        private readonly object _videoDeviceLock = new object();

        public FlightPlannerVideoOptions()
        {
            InitializeComponent();

            this.cmbVideoSources.Click += CmbVideoSources_Click;
            this.cmbVideoSources.SelectedIndexChanged += CmbVideoSources_SelectedIndexChanged;
            this.cmbOsdColor.DrawItem += CmbOsdColor_DrawItem;
            this.cmbOsdColor.SelectedIndexChanged += CmbOsdColor_SelectedIndexChanged;
            this.btnVideoStart.Click += BtnVideoStart_Click;
            this.btnVideoStop.Click += BtnVideoStop_Click;
            this.chkHudShow.CheckedChanged += ChkHudShow_CheckedChanged;
            this.chkStreamFillHud.CheckedChanged += ChkStreamFillHud_CheckedChanged;
            this.btnGStreamerStart.Click += BtnGStreamerStart_Click;
            this.btnGStreamerStop.Click += BtnGStreamerStop_Click;
            this.btnStreamStart.Click += BtnStreamStart_Click;
            this.btnStreamStop.Click += BtnStreamStop_Click;
            this.txtStreamUrl.Leave += TxtStreamUrl_Leave;
            this.VisibleChanged += FlightPlannerVideoOptions_VisibleChanged;
            this.Resize += FlightPlannerVideoOptions_Resize;
        }

        private void FlightPlannerVideoOptions_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible)
                return;

            chkHudShow.Checked = Settings.Instance.GetBoolean("CHK_hudshow", GCSViews.FlightData.myhud.hudon);
            SyncStreamControlStates();
            AdjustVideoLayout();
        }

        private void FlightPlannerVideoOptions_Resize(object sender, EventArgs e)
        {
            AdjustVideoLayout();
        }

        private void InitializeComponent()
        {
            this.labelVideoDevice = new System.Windows.Forms.Label();
            this.labelVideoFormat = new System.Windows.Forms.Label();
            this.labelOsdColor = new System.Windows.Forms.Label();
            this.labelStreamUrl = new System.Windows.Forms.Label();
            this.labelGStreamer = new System.Windows.Forms.Label();
            this.cmbVideoSources = new System.Windows.Forms.ComboBox();
            this.cmbVideoResolutions = new System.Windows.Forms.ComboBox();
            this.cmbOsdColor = new System.Windows.Forms.ComboBox();
            this.txtGStreamerSource = new System.Windows.Forms.TextBox();
            this.txtStreamUrl = new System.Windows.Forms.TextBox();
            this.btnStreamStart = new MissionPlanner.Controls.MyButton();
            this.btnStreamStop = new MissionPlanner.Controls.MyButton();
            this.btnGStreamerStart = new MissionPlanner.Controls.MyButton();
            this.btnGStreamerStop = new MissionPlanner.Controls.MyButton();
            this.btnVideoStart = new MissionPlanner.Controls.MyButton();
            this.btnVideoStop = new MissionPlanner.Controls.MyButton();
            this.chkHudShow = new System.Windows.Forms.CheckBox();
            this.chkStreamFillHud = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // labelVideoDevice
            // 
            this.labelVideoDevice.AutoSize = true;
            this.labelVideoDevice.Location = new System.Drawing.Point(10, 15);
            this.labelVideoDevice.Name = "labelVideoDevice";
            this.labelVideoDevice.Size = new System.Drawing.Size(89, 16);
            this.labelVideoDevice.TabIndex = 0;
            this.labelVideoDevice.Text = "Video Device";
            // 
            // labelVideoFormat
            // 
            this.labelVideoFormat.AutoSize = true;
            this.labelVideoFormat.Location = new System.Drawing.Point(10, 45);
            this.labelVideoFormat.Name = "labelVideoFormat";
            this.labelVideoFormat.Size = new System.Drawing.Size(88, 16);
            this.labelVideoFormat.TabIndex = 2;
            this.labelVideoFormat.Text = "Video Format";
            // 
            // labelOsdColor
            // 
            this.labelOsdColor.AutoSize = true;
            this.labelOsdColor.Location = new System.Drawing.Point(10, 75);
            this.labelOsdColor.Name = "labelOsdColor";
            this.labelOsdColor.Size = new System.Drawing.Size(71, 16);
            this.labelOsdColor.TabIndex = 7;
            this.labelOsdColor.Text = "OSD Color";
            // 
            // labelStreamUrl
            // 
            this.labelStreamUrl.AutoSize = true;
            this.labelStreamUrl.Location = new System.Drawing.Point(10, 110);
            this.labelStreamUrl.Name = "labelStreamUrl";
            this.labelStreamUrl.Size = new System.Drawing.Size(78, 16);
            this.labelStreamUrl.TabIndex = 9;
            this.labelStreamUrl.Text = "Video URL";
            // 
            // labelGStreamer
            // 
            this.labelGStreamer.AutoSize = true;
            this.labelGStreamer.Location = new System.Drawing.Point(10, 160);
            this.labelGStreamer.Name = "labelGStreamer";
            this.labelGStreamer.Size = new System.Drawing.Size(72, 16);
            this.labelGStreamer.TabIndex = 14;
            this.labelGStreamer.Text = "GStreamer";
            // 
            // cmbVideoSources
            // 
            this.cmbVideoSources.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbVideoSources.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVideoSources.FormattingEnabled = true;
            this.cmbVideoSources.Location = new System.Drawing.Point(120, 12);
            this.cmbVideoSources.Name = "cmbVideoSources";
            this.cmbVideoSources.Size = new System.Drawing.Size(514, 24);
            this.cmbVideoSources.TabIndex = 1;
            // 
            // cmbVideoResolutions
            // 
            this.cmbVideoResolutions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbVideoResolutions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVideoResolutions.FormattingEnabled = true;
            this.cmbVideoResolutions.Location = new System.Drawing.Point(120, 42);
            this.cmbVideoResolutions.Name = "cmbVideoResolutions";
            this.cmbVideoResolutions.Size = new System.Drawing.Size(514, 24);
            this.cmbVideoResolutions.TabIndex = 3;
            // 
            // cmbOsdColor
            // 
            this.cmbOsdColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbOsdColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbOsdColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOsdColor.FormattingEnabled = true;
            this.cmbOsdColor.Location = new System.Drawing.Point(120, 72);
            this.cmbOsdColor.Name = "cmbOsdColor";
            this.cmbOsdColor.Size = new System.Drawing.Size(629, 23);
            this.cmbOsdColor.TabIndex = 8;
            // 
            // txtStreamUrl
            // 
            this.txtStreamUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStreamUrl.Location = new System.Drawing.Point(120, 107);
            this.txtStreamUrl.Name = "txtStreamUrl";
            this.txtStreamUrl.Size = new System.Drawing.Size(514, 22);
            this.txtStreamUrl.TabIndex = 10;
            // 
            // btnStreamStart
            // 
            this.btnStreamStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStreamStart.Location = new System.Drawing.Point(640, 106);
            this.btnStreamStart.Name = "btnStreamStart";
            this.btnStreamStart.Size = new System.Drawing.Size(50, 23);
            this.btnStreamStart.TabIndex = 11;
            this.btnStreamStart.Text = "Start";
            this.btnStreamStart.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.btnStreamStart.UseVisualStyleBackColor = true;
            // 
            // btnStreamStop
            // 
            this.btnStreamStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStreamStop.Location = new System.Drawing.Point(699, 106);
            this.btnStreamStop.Name = "btnStreamStop";
            this.btnStreamStop.Size = new System.Drawing.Size(50, 23);
            this.btnStreamStop.TabIndex = 12;
            this.btnStreamStop.Text = "Stop";
            this.btnStreamStop.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.btnStreamStop.UseVisualStyleBackColor = true;
            // 
            // txtGStreamerSource
            // 
            this.txtGStreamerSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGStreamerSource.Location = new System.Drawing.Point(120, 160);
            this.txtGStreamerSource.Multiline = true;
            this.txtGStreamerSource.Name = "txtGStreamerSource";
            this.txtGStreamerSource.Size = new System.Drawing.Size(514, 32);
            this.txtGStreamerSource.TabIndex = 15;
            // 
            // btnGStreamerStart
            // 
            this.btnGStreamerStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGStreamerStart.Location = new System.Drawing.Point(640, 160);
            this.btnGStreamerStart.Name = "btnGStreamerStart";
            this.btnGStreamerStart.Size = new System.Drawing.Size(50, 23);
            this.btnGStreamerStart.TabIndex = 16;
            this.btnGStreamerStart.Text = "Start";
            this.btnGStreamerStart.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.btnGStreamerStart.UseVisualStyleBackColor = true;
            // 
            // btnGStreamerStop
            // 
            this.btnGStreamerStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGStreamerStop.Location = new System.Drawing.Point(699, 160);
            this.btnGStreamerStop.Name = "btnGStreamerStop";
            this.btnGStreamerStop.Size = new System.Drawing.Size(50, 23);
            this.btnGStreamerStop.TabIndex = 17;
            this.btnGStreamerStop.Text = "Stop";
            this.btnGStreamerStop.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.btnGStreamerStop.UseVisualStyleBackColor = true;
            // 
            // btnVideoStart
            // 
            this.btnVideoStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVideoStart.Location = new System.Drawing.Point(640, 13);
            this.btnVideoStart.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnVideoStart.Name = "btnVideoStart";
            this.btnVideoStart.Size = new System.Drawing.Size(50, 23);
            this.btnVideoStart.TabIndex = 4;
            this.btnVideoStart.Text = "Start";
            this.btnVideoStart.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.btnVideoStart.UseVisualStyleBackColor = true;
            // 
            // btnVideoStop
            // 
            this.btnVideoStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVideoStop.Location = new System.Drawing.Point(699, 13);
            this.btnVideoStop.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnVideoStop.Name = "btnVideoStop";
            this.btnVideoStop.Size = new System.Drawing.Size(50, 23);
            this.btnVideoStop.TabIndex = 5;
            this.btnVideoStop.Text = "Stop";
            this.btnVideoStop.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.btnVideoStop.UseVisualStyleBackColor = true;
            // 
            // chkHudShow
            // 
            this.chkHudShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkHudShow.AutoSize = true;
            this.chkHudShow.Location = new System.Drawing.Point(640, 44);
            this.chkHudShow.Name = "chkHudShow";
            this.chkHudShow.Size = new System.Drawing.Size(109, 20);
            this.chkHudShow.TabIndex = 6;
            this.chkHudShow.Text = "HUD Overlay";
            this.chkHudShow.UseVisualStyleBackColor = true;
            // 
            // chkStreamFillHud
            // 
            this.chkStreamFillHud.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkStreamFillHud.AutoSize = true;
            this.chkStreamFillHud.Location = new System.Drawing.Point(540, 109);
            this.chkStreamFillHud.Name = "chkStreamFillHud";
            this.chkStreamFillHud.Size = new System.Drawing.Size(97, 20);
            this.chkStreamFillHud.TabIndex = 13;
            this.chkStreamFillHud.Text = "Fill vertically";
            this.chkStreamFillHud.UseVisualStyleBackColor = true;
            // 
            // FlightPlannerVideoOptions
            // 
            this.Controls.Add(this.labelVideoDevice);
            this.Controls.Add(this.cmbVideoSources);
            this.Controls.Add(this.labelVideoFormat);
            this.Controls.Add(this.cmbVideoResolutions);
            this.Controls.Add(this.btnVideoStart);
            this.Controls.Add(this.btnVideoStop);
            this.Controls.Add(this.chkHudShow);
            this.Controls.Add(this.labelOsdColor);
            this.Controls.Add(this.cmbOsdColor);
            this.Controls.Add(this.labelStreamUrl);
            this.Controls.Add(this.txtStreamUrl);
            this.Controls.Add(this.btnStreamStart);
            this.Controls.Add(this.btnStreamStop);
            this.Controls.Add(this.chkStreamFillHud);
            this.Controls.Add(this.labelGStreamer);
            this.Controls.Add(this.txtGStreamerSource);
            this.Controls.Add(this.btnGStreamerStart);
            this.Controls.Add(this.btnGStreamerStop);
            this.Name = "FlightPlannerVideoOptions";
            this.Size = new System.Drawing.Size(760, 210);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Activate();
        }

        public void Activate()
        {
            startup = true;

            cmbOsdColor.DataSource = Enum.GetNames(typeof(KnownColor));

            var hudcolor = Settings.Instance["hudcolor"];
            if (hudcolor != null)
            {
                var index = cmbOsdColor.Items.IndexOf(hudcolor ?? "White");
                try
                {
                    if (index >= 0)
                        cmbOsdColor.SelectedIndex = index;
                }
                catch { }
            }

            txtGStreamerSource.Text = Settings.Instance["gstreamer_url"] != null
                ? Settings.Instance["gstreamer_url"]
                : @"videotestsrc ! video/x-raw, width=1280, height=720, framerate=30/1 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink";
            txtStreamUrl.Text = Settings.Instance["video_stream_url"] ?? string.Empty;
            chkStreamFillHud.Checked = Settings.Instance.GetBoolean("video_stream_fill_hud", false);
            GCSViews.FlightData.SetHudWebViewStretchToHud(chkStreamFillHud.Checked);

            chkHudShow.Checked = Settings.Instance.GetBoolean("CHK_hudshow", GCSViews.FlightData.myhud.hudon);
            SyncStreamControlStates();

            try
            {
                if (Settings.Instance["video_device"] != null)
                {
                    CmbVideoSources_Click(this, null);
                    var device = Settings.Instance.GetInt32("video_device");
                    if (cmbVideoSources.Items.Count > device)
                        cmbVideoSources.SelectedIndex = device;

                    if (Settings.Instance["video_options"] != "" && cmbVideoSources.Text != "")
                    {
                        if (cmbVideoResolutions.Items.Count > Settings.Instance.GetInt32("video_options"))
                            cmbVideoResolutions.SelectedIndex = Settings.Instance.GetInt32("video_options");
                    }
                }
            }
            catch { }

            startup = false;
            AdjustVideoLayout();
        }

        private void AdjustVideoLayout()
        {
            if (ClientSize.Width <= 0)
                return;

            const int left = 120;
            const int right = 10;
            const int gap = 6;
            const int buttonWidth = 50;

            var stopX = ClientSize.Width - right - buttonWidth;
            var startX = stopX - gap - buttonWidth;

            var streamY = cmbOsdColor.Bottom + 14;
            txtStreamUrl.Left = left;
            txtStreamUrl.Top = streamY;
            labelStreamUrl.Top = streamY + 3;
            btnStreamStart.Top = streamY;
            btnStreamStop.Top = streamY;

            chkStreamFillHud.Top = streamY + 2;
            chkStreamFillHud.Left = Math.Max(left + 20, startX - chkStreamFillHud.Width - 8);
            txtStreamUrl.Width = Math.Max(150, chkStreamFillHud.Left - gap - txtStreamUrl.Left);

            var gstY = txtStreamUrl.Bottom + 16;
            txtGStreamerSource.Top = gstY;
            labelGStreamer.Top = gstY + 3;
            btnGStreamerStart.Top = gstY;
            btnGStreamerStop.Top = gstY;
            txtGStreamerSource.Height = 34;

            btnStreamStart.Left = startX;
            btnStreamStop.Left = stopX;
            btnGStreamerStart.Left = startX;
            btnGStreamerStop.Left = stopX;
            txtGStreamerSource.Left = left;
            txtGStreamerSource.Width = Math.Max(150, startX - gap - txtGStreamerSource.Left);
        }

        private void CmbVideoSources_Click(object sender, EventArgs e)
        {
            if (MainV2.MONO)
                return;

            try
            {
                var capt = new Capture();
                var devices = WebCamService.Capture.getDevices();
                cmbVideoSources.DataSource = devices;
                capt.Dispose();
            }
            catch { }
        }

        private void CmbVideoSources_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MainV2.MONO)
                return;

            try
            {
                int hr;
                int count;
                int size;
                object o;
                IBaseFilter capFilter = null;
                ICaptureGraphBuilder2 capGraph = null;
                AMMediaType media = null;
                VideoInfoHeader v;
                VideoStreamConfigCaps c;
                var modes = new List<GCSBitmapInfo>();

                capGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
                var m_FilterGraph = (IFilterGraph2)new FilterGraph();

                DsDevice[] capDevices;
                capDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

                if (cmbVideoSources.SelectedIndex < 0 || cmbVideoSources.SelectedIndex >= capDevices.Length)
                    return;

                hr = m_FilterGraph.AddSourceFilterForMoniker(capDevices[cmbVideoSources.SelectedIndex].Mon, null,
                    "Video input", out capFilter);
                try
                {
                    DsError.ThrowExceptionForHR(hr);
                }
                catch
                {
                    return;
                }

                hr = capGraph.FindInterface(PinCategory.Capture, MediaType.Video, capFilter, typeof(IAMStreamConfig).GUID, out o);
                DsError.ThrowExceptionForHR(hr);

                var videoStreamConfig = o as IAMStreamConfig;
                if (videoStreamConfig == null)
                    return;

                hr = videoStreamConfig.GetNumberOfCapabilities(out count, out size);
                DsError.ThrowExceptionForHR(hr);
                var taskMemPointer = Marshal.AllocCoTaskMem(size);
                for (var i = 0; i < count; i++)
                {
                    hr = videoStreamConfig.GetStreamCaps(i, out media, taskMemPointer);
                    v = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
                    c = (VideoStreamConfigCaps)Marshal.PtrToStructure(taskMemPointer, typeof(VideoStreamConfigCaps));
                    modes.Add(new GCSBitmapInfo(v.BmiHeader.Width, v.BmiHeader.Height, c.MaxFrameInterval,
                        c.VideoStandard.ToString(), media));
                }
                Marshal.FreeCoTaskMem(taskMemPointer);
                DsUtils.FreeAMMediaType(media);

                cmbVideoResolutions.DataSource = modes;

                if (Settings.Instance["video_options"] != "" && cmbVideoSources.Text != "")
                {
                    try
                    {
                        cmbVideoResolutions.SelectedIndex = Settings.Instance.GetInt32("video_options");
                    }
                    catch { }
                }

                if (!startup)
                    Settings.Instance["video_device"] = cmbVideoSources.SelectedIndex.ToString();
            }
            catch { }
        }

        private void BtnVideoStart_Click(object sender, EventArgs e)
        {
            if (MainV2.MONO)
                return;

            var bmp = cmbVideoResolutions.SelectedItem as GCSBitmapInfo;
            if (bmp == null)
                return;

            try
            {
                StopAllHudSources();
                lock (_videoDeviceLock)
                {
                    MainV2.cam = CreateVideoCaptureWithRetry(cmbVideoSources.SelectedIndex, bmp.Media);
                    MainV2.cam.Start();
                }

                GCSViews.FlightData.myhud.hudon = chkHudShow.Checked;
                MainV2.cam.camimage += GCSViews.FlightData.instance.cam_camimage;

                Settings.Instance["video_device"] = cmbVideoSources.SelectedIndex.ToString();
                Settings.Instance["video_options"] = cmbVideoResolutions.SelectedIndex.ToString();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Camera Fail: " + ex.Message);
            }
            finally
            {
                SyncStreamControlStates();
            }
        }

        private void BtnVideoStop_Click(object sender, EventArgs e)
        {
            StopVideoDeviceSource();
            SyncStreamControlStates();
        }

        private void ChkHudShow_CheckedChanged(object sender, EventArgs e)
        {
            GCSViews.FlightData.myhud.hudon = chkHudShow.Checked;
            Settings.Instance["CHK_hudshow"] = chkHudShow.Checked.ToString();
        }

        private void ChkStreamFillHud_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["video_stream_fill_hud"] = chkStreamFillHud.Checked.ToString();
            GCSViews.FlightData.SetHudWebViewStretchToHud(chkStreamFillHud.Checked);
        }

        private void BtnGStreamerStart_Click(object sender, EventArgs e)
        {
            var url = txtGStreamerSource.Text;
            if (string.IsNullOrWhiteSpace(url))
                return;

            Settings.Instance["gstreamer_url"] = url;

            GStreamer.GstLaunch = GStreamer.LookForGstreamer();
            if (!GStreamer.GstLaunchExists)
            {
                GStreamerUI.DownloadGStreamer();
                if (!GStreamer.GstLaunchExists)
                {
                    SyncStreamControlStates();
                    return;
                }
            }

            try
            {
                StopAllHudSources();
                GCSViews.FlightData.myhud.hudon = chkHudShow.Checked;
                GCSViews.FlightData.StartHudGStreamer(url);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString(), Strings.ERROR);
            }
            finally
            {
                SyncStreamControlStates();
            }
        }

        private void BtnGStreamerStop_Click(object sender, EventArgs e)
        {
            GCSViews.FlightData.StopHudGStreamer();
            SyncStreamControlStates();
        }

        private void TxtStreamUrl_Leave(object sender, EventArgs e)
        {
            var value = txtStreamUrl.Text?.Trim();
            if (!string.IsNullOrEmpty(value))
                Settings.Instance["video_stream_url"] = value;
        }

        private void BtnStreamStart_Click(object sender, EventArgs e)
        {
            var streamUrl = txtStreamUrl.Text?.Trim();
            if (string.IsNullOrWhiteSpace(streamUrl))
            {
                CustomMessageBox.Show("Please enter stream URL", Strings.ERROR);
                return;
            }

            if (!Uri.TryCreate(streamUrl, UriKind.Absolute, out var parsedUri))
            {
                CustomMessageBox.Show("Invalid stream URL", Strings.ERROR);
                SyncStreamControlStates();
                return;
            }

            if (parsedUri.Scheme != Uri.UriSchemeHttp && parsedUri.Scheme != Uri.UriSchemeHttps)
            {
                CustomMessageBox.Show("Stream URL supports only web streams: http, https", Strings.ERROR);
                SyncStreamControlStates();
                return;
            }

            Settings.Instance["video_stream_url"] = streamUrl;

            try
            {
                StopAllHudSources();
                GCSViews.FlightData.myhud.hudon = chkHudShow.Checked;
                GCSViews.FlightData.StartHudWebViewOverlay(streamUrl);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString(), Strings.ERROR);
            }
            finally
            {
                SyncStreamControlStates();
            }
        }

        private void BtnStreamStop_Click(object sender, EventArgs e)
        {
            GCSViews.FlightData.StopHudWebViewOverlay();
            SyncStreamControlStates();
        }

        private void StopAllHudSources()
        {
            StopVideoDeviceSource();
            GCSViews.FlightData.StopHudGStreamer();
            GCSViews.FlightData.StopHudWebViewOverlay();
        }

        private void StopVideoDeviceSource()
        {
            lock (_videoDeviceLock)
            {
                if (MainV2.cam != null)
                {
                    var previousCam = MainV2.cam;
                    MainV2.cam = null;

                    try
                    {
                        previousCam.camimage -= GCSViews.FlightData.instance.cam_camimage;
                    }
                    catch { }

                    try
                    {
                        previousCam.Dispose();
                    }
                    catch { }
                }
            }

            GCSViews.FlightData.myhud.bgimage = null;
        }

        private static Capture CreateVideoCaptureWithRetry(int selectedIndex, AMMediaType media)
        {
            Exception lastError = null;
            for (var attempt = 0; attempt < 3; attempt++)
            {
                try
                {
                    return new Capture(selectedIndex, media);
                }
                catch (Exception ex)
                {
                    lastError = ex;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    Thread.Sleep(200);
                }
            }

            throw lastError ?? new Exception("Unable to start video device");
        }

        private void SyncStreamControlStates()
        {
            var active = GetActiveVideoSource();
            var idle = active == ActiveVideoSource.None;

            btnVideoStart.Enabled = idle;
            btnVideoStop.Enabled = active == ActiveVideoSource.VideoDevice;
            btnStreamStart.Enabled = idle;
            btnStreamStop.Enabled = active == ActiveVideoSource.StreamUrl;
            btnGStreamerStart.Enabled = idle;
            btnGStreamerStop.Enabled = active == ActiveVideoSource.GStreamer;

            cmbVideoSources.Enabled = idle || active == ActiveVideoSource.VideoDevice;
            cmbVideoResolutions.Enabled = idle || active == ActiveVideoSource.VideoDevice;
            txtStreamUrl.Enabled = idle;
            txtGStreamerSource.Enabled = idle || active == ActiveVideoSource.GStreamer;
            chkStreamFillHud.Enabled = idle || active == ActiveVideoSource.StreamUrl;
        }

        private ActiveVideoSource GetActiveVideoSource()
        {
            if (MainV2.cam != null)
                return ActiveVideoSource.VideoDevice;
            if (GCSViews.FlightData.IsHudGStreamerRunning)
                return ActiveVideoSource.GStreamer;
            if (GCSViews.FlightData.IsHudWebViewRunning)
                return ActiveVideoSource.StreamUrl;

            return ActiveVideoSource.None;
        }

        private void CmbOsdColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            if (cmbOsdColor.Text != "")
            {
                Settings.Instance["hudcolor"] = cmbOsdColor.Text;
                GCSViews.FlightData.myhud.hudcolor =
                    Color.FromKnownColor((KnownColor)Enum.Parse(typeof(KnownColor), cmbOsdColor.Text));
            }
        }

        private void CmbOsdColor_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            var g = e.Graphics;
            var rect = e.Bounds;
            Brush brush;

            if ((e.State & DrawItemState.Selected) == 0)
                brush = new SolidBrush(cmbOsdColor.BackColor);
            else
                brush = SystemBrushes.Highlight;

            g.FillRectangle(brush, rect);

            brush = new SolidBrush(Color.FromName((string)cmbOsdColor.Items[e.Index]));

            g.FillRectangle(brush, rect.X + 2, rect.Y + 2, 30, rect.Height - 4);
            g.DrawRectangle(Pens.Black, rect.X + 2, rect.Y + 2, 30, rect.Height - 4);

            if ((e.State & DrawItemState.Selected) == 0)
                brush = new SolidBrush(cmbOsdColor.ForeColor);
            else
                brush = SystemBrushes.HighlightText;
            g.DrawString(cmbOsdColor.Items[e.Index].ToString(),
                cmbOsdColor.Font, brush, rect.X + 35, rect.Top + rect.Height - cmbOsdColor.Font.Height);
        }

        private enum ActiveVideoSource
        {
            None,
            VideoDevice,
            StreamUrl,
            GStreamer
        }

        public class GCSBitmapInfo
        {
            public GCSBitmapInfo(int width, int height, long fps, string standard, AMMediaType media)
            {
                Width = width;
                Height = height;
                Fps = fps;
                Standard = standard;
                Media = media;
            }

            public int Width { get; set; }
            public int Height { get; set; }
            public long Fps { get; set; }
            public string Standard { get; set; }
            public AMMediaType Media { get; set; }

            public override string ToString()
            {
                return Width + " x " + Height + string.Format(" {0:0.00} fps ", 10000000.0 / Fps) + Standard;
            }
        }
    }
}
