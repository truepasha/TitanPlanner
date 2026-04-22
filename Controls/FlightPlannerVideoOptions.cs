using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
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
        private Label labelCodecValue;
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
        private bool startup = true;

        public FlightPlannerVideoOptions()
        {
            InitializeComponent();

            // Wire up event handlers (done here to prevent linter from removing them)
            this.cmbVideoSources.Click += CmbVideoSources_Click;
            this.cmbVideoSources.SelectedIndexChanged += CmbVideoSources_SelectedIndexChanged;
            this.cmbOsdColor.DrawItem += CmbOsdColor_DrawItem;
            this.cmbOsdColor.SelectedIndexChanged += CmbOsdColor_SelectedIndexChanged;
            this.btnVideoStart.Click += BtnVideoStart_Click;
            this.btnVideoStop.Click += BtnVideoStop_Click;
            this.chkHudShow.CheckedChanged += ChkHudShow_CheckedChanged;
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
            if (this.Visible)
            {
                // Sync HUD overlay checkbox with saved setting when tab becomes visible
                chkHudShow.Checked = Settings.Instance.GetBoolean("CHK_hudshow", GCSViews.FlightData.myhud.hudon);
                AdjustVideoLayout();
            }
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
            this.labelCodecValue = new System.Windows.Forms.Label();
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
            this.labelStreamUrl.Text = "Stream URL";
            // 
            // labelCodecValue
            // 
            this.labelCodecValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCodecValue.AutoEllipsis = true;
            this.labelCodecValue.Location = new System.Drawing.Point(520, 110);
            this.labelCodecValue.Name = "labelCodecValue";
            this.labelCodecValue.Size = new System.Drawing.Size(114, 18);
            this.labelCodecValue.TabIndex = 13;
            this.labelCodecValue.Text = "Codec: -";
            this.labelCodecValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelGStreamer
            // 
            this.labelGStreamer.AutoSize = true;
            this.labelGStreamer.Location = new System.Drawing.Point(10, 140);
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
            this.txtGStreamerSource.Location = new System.Drawing.Point(120, 140);
            this.txtGStreamerSource.Multiline = true;
            this.txtGStreamerSource.Name = "txtGStreamerSource";
            this.txtGStreamerSource.Size = new System.Drawing.Size(514, 32);
            this.txtGStreamerSource.TabIndex = 15;
            // 
            // btnGStreamerStart
            // 
            this.btnGStreamerStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGStreamerStart.Location = new System.Drawing.Point(640, 140);
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
            this.btnGStreamerStop.Location = new System.Drawing.Point(699, 140);
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
            this.Controls.Add(this.labelCodecValue);
            this.Controls.Add(this.txtStreamUrl);
            this.Controls.Add(this.btnStreamStart);
            this.Controls.Add(this.btnStreamStop);
            this.Controls.Add(this.labelGStreamer);
            this.Controls.Add(this.txtGStreamerSource);
            this.Controls.Add(this.btnGStreamerStart);
            this.Controls.Add(this.btnGStreamerStop);
            this.Name = "FlightPlannerVideoOptions";
            this.Size = new System.Drawing.Size(760, 200);
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

            // Populate OSD color dropdown
            cmbOsdColor.DataSource = Enum.GetNames(typeof(KnownColor));

            // Set OSD color from settings
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

            // Pre-fill GStreamer source from settings
            txtGStreamerSource.Text = Settings.Instance["gstreamer_url"] != null
                ? Settings.Instance["gstreamer_url"]
                : @"videotestsrc ! video/x-raw, width=1280, height=720, framerate=30/1 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink";
            txtStreamUrl.Text = Settings.Instance["video_stream_url"] ?? string.Empty;
            labelCodecValue.Text = "Codec: -";

            // Setup video start/stop button states
            if (MainV2.cam != null)
            {
                btnVideoStart.Enabled = false;
            }
            else
            {
                btnVideoStart.Enabled = true;
            }

            // Sync HUD overlay checkbox with saved setting
            chkHudShow.Checked = Settings.Instance.GetBoolean("CHK_hudshow", GCSViews.FlightData.myhud.hudon);

            // Setup GStreamer start button state
            btnGStreamerStart.Enabled = !GCSViews.FlightData.IsHudGStreamerRunning;
            btnStreamStart.Enabled = !GCSViews.FlightData.IsHudGStreamerRunning;

            // Try to load saved video device
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

            // Vertical layout (DPI/font-safe)
            var streamY = cmbOsdColor.Bottom + 14;
            txtStreamUrl.Top = streamY;
            labelStreamUrl.Top = streamY + 3;
            btnStreamStart.Top = streamY;
            btnStreamStop.Top = streamY;
            labelCodecValue.Top = streamY + 3;

            var gstY = txtStreamUrl.Bottom + 16;
            txtGStreamerSource.Top = gstY;
            labelGStreamer.Top = gstY + 3;
            btnGStreamerStart.Top = gstY;
            btnGStreamerStop.Top = gstY;
            txtGStreamerSource.Height = 38;

            btnStreamStart.Left = startX;
            btnStreamStop.Left = stopX;
            btnGStreamerStart.Left = startX;
            btnGStreamerStop.Left = stopX;

            var codecWidth = 110;
            var codecX = startX - gap - codecWidth;
            if (codecX < left + 150)
            {
                codecWidth = Math.Max(70, startX - gap - (left + 150));
                codecX = startX - gap - codecWidth;
            }

            labelCodecValue.Left = codecX;
            labelCodecValue.Width = Math.Max(70, codecWidth);

            txtStreamUrl.Left = left;
            txtStreamUrl.Width = Math.Max(150, codecX - gap - txtStreamUrl.Left);

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
                var TaskMemPointer = Marshal.AllocCoTaskMem(size);
                for (var i = 0; i < count; i++)
                {
                    hr = videoStreamConfig.GetStreamCaps(i, out media, TaskMemPointer);
                    v = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
                    c = (VideoStreamConfigCaps)Marshal.PtrToStructure(TaskMemPointer, typeof(VideoStreamConfigCaps));
                    modes.Add(new GCSBitmapInfo(v.BmiHeader.Width, v.BmiHeader.Height, c.MaxFrameInterval,
                        c.VideoStandard.ToString(), media));
                }
                Marshal.FreeCoTaskMem(TaskMemPointer);
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

                // Save selected device
                if (!startup)
                {
                    Settings.Instance["video_device"] = cmbVideoSources.SelectedIndex.ToString();
                }
            }
            catch { }
        }

        private void BtnVideoStart_Click(object sender, EventArgs e)
        {
            if (MainV2.MONO)
                return;

            // Stop first
            BtnVideoStop_Click(sender, e);

            var bmp = cmbVideoResolutions.SelectedItem as GCSBitmapInfo;
            if (bmp == null)
                return;

            try
            {
                MainV2.cam = new Capture(cmbVideoSources.SelectedIndex, bmp.Media);
                MainV2.cam.Start();

                // Apply HUD overlay setting
                GCSViews.FlightData.myhud.hudon = chkHudShow.Checked;

                // Hook up the camera image event to display on HUD
                MainV2.cam.camimage += GCSViews.FlightData.instance.cam_camimage;

                Settings.Instance["video_device"] = cmbVideoSources.SelectedIndex.ToString();
                Settings.Instance["video_options"] = cmbVideoResolutions.SelectedIndex.ToString();

                btnVideoStart.Enabled = false;
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Camera Fail: " + ex.Message);
            }
        }

        private void BtnVideoStop_Click(object sender, EventArgs e)
        {
            btnVideoStart.Enabled = true;
            if (MainV2.cam != null)
            {
                MainV2.cam.Dispose();
                MainV2.cam = null;
            }

            // Clear the HUD background image to prevent stale frames
            GCSViews.FlightData.myhud.bgimage = null;
        }

        private void ChkHudShow_CheckedChanged(object sender, EventArgs e)
        {
            GCSViews.FlightData.myhud.hudon = chkHudShow.Checked;
            Settings.Instance["CHK_hudshow"] = chkHudShow.Checked.ToString();
        }

        private void BtnGStreamerStart_Click(object sender, EventArgs e)
        {
            // Disable immediately to prevent spam clicking
            btnGStreamerStart.Enabled = false;

            var url = txtGStreamerSource.Text;
            if (string.IsNullOrWhiteSpace(url))
            {
                btnGStreamerStart.Enabled = true;
                return;
            }

            Settings.Instance["gstreamer_url"] = url;

            GStreamer.GstLaunch = GStreamer.LookForGstreamer();

            if (!GStreamer.GstLaunchExists)
            {
                GStreamerUI.DownloadGStreamer();

                if (!GStreamer.GstLaunchExists)
                {
                    btnGStreamerStart.Enabled = true;
                    return;
                }
            }

            try
            {
                // Apply HUD overlay setting
                GCSViews.FlightData.myhud.hudon = chkHudShow.Checked;
                GCSViews.FlightData.StopHudWebViewOverlay();

                GCSViews.FlightData.StartHudGStreamer(url);
                labelCodecValue.Text = "Codec: " + DetectCodec(url);
            }
            catch (Exception ex)
            {
                btnGStreamerStart.Enabled = true;
                CustomMessageBox.Show(ex.ToString(), Strings.ERROR);
            }
        }

        private void BtnGStreamerStop_Click(object sender, EventArgs e)
        {
            GCSViews.FlightData.StopHudGStreamer();
            GCSViews.FlightData.StopHudWebViewOverlay();
            btnGStreamerStart.Enabled = true;
            btnStreamStart.Enabled = true;
            labelCodecValue.Text = "Codec: -";
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

            btnStreamStart.Enabled = false;

            if (!Uri.TryCreate(streamUrl, UriKind.Absolute, out var parsedUri))
            {
                btnStreamStart.Enabled = true;
                CustomMessageBox.Show("Invalid stream URL", Strings.ERROR);
                return;
            }

            if (parsedUri.Scheme != Uri.UriSchemeHttp &&
                parsedUri.Scheme != Uri.UriSchemeHttps &&
                parsedUri.Scheme != "rtsp")
            {
                btnStreamStart.Enabled = true;
                CustomMessageBox.Show("Supported URL schemes: http, https, rtsp", Strings.ERROR);
                return;
            }

            Settings.Instance["video_stream_url"] = streamUrl;

            // Browser-render mode for http(s): render WebView2 frames into HUD background
            if (parsedUri.Scheme == Uri.UriSchemeHttp || parsedUri.Scheme == Uri.UriSchemeHttps)
            {
                try
                {
                    GCSViews.FlightData.myhud.hudon = chkHudShow.Checked;
                    GCSViews.FlightData.StopHudGStreamer();
                    GCSViews.FlightData.StartHudWebViewOverlay(streamUrl);
                    labelCodecValue.Text = "Codec: Browser/WebRTC";
                }

                catch (Exception ex)
                {
                    btnStreamStart.Enabled = true;
                    CustomMessageBox.Show(ex.ToString(), Strings.ERROR);
                }

                return;
            }

            var pipeline = BuildPipelineFromUrl(parsedUri);
            txtGStreamerSource.Text = pipeline;
            Settings.Instance["gstreamer_url"] = pipeline;

            GStreamer.GstLaunch = GStreamer.LookForGstreamer();
            if (!GStreamer.GstLaunchExists)
            {
                GStreamerUI.DownloadGStreamer();

                if (!GStreamer.GstLaunchExists)
                {
                    btnStreamStart.Enabled = true;
                    return;
                }
            }

            try
            {
                GCSViews.FlightData.myhud.hudon = chkHudShow.Checked;
                GCSViews.FlightData.StopHudWebViewOverlay();
                GCSViews.FlightData.StartHudGStreamer(pipeline);
                labelCodecValue.Text = "Codec: " + DetectCodec(streamUrl);
            }
            catch (Exception ex)
            {
                btnStreamStart.Enabled = true;
                CustomMessageBox.Show(ex.ToString(), Strings.ERROR);
            }
        }

        private void BtnStreamStop_Click(object sender, EventArgs e)
        {
            BtnGStreamerStop_Click(sender, e);
        }

        private static string BuildPipelineFromUrl(Uri streamUri)
        {
            var location = streamUri.AbsoluteUri.Replace("\\", "\\\\").Replace("\"", "\\\"");
            if (streamUri.Scheme == "rtsp")
            {
                return $"rtspsrc location=\"{location}\" latency=1 udp-reconnect=1 timeout=0 do-retransmission=false ! application/x-rtp ! decodebin ! queue max-size-buffers=1 leaky=2 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink sync=false";
            }

            return $"uridecodebin uri=\"{location}\" ! queue max-size-buffers=1 leaky=2 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink sync=false";
        }

        private static string DetectCodec(string source)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(GStreamer.GstLaunch))
                    return DetectCodecHeuristic(source);

                var gstBinPath = Path.GetDirectoryName(GStreamer.GstLaunch);
                if (string.IsNullOrEmpty(gstBinPath))
                    return DetectCodecHeuristic(source);

                var discoverer = Path.Combine(gstBinPath, "gst-discoverer-1.0.exe");
                if (!File.Exists(discoverer))
                    return DetectCodecHeuristic(source);

                var psi = new ProcessStartInfo
                {
                    FileName = discoverer,
                    Arguments = $"\"{source}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = Process.Start(psi))
                {
                    if (process == null)
                        return DetectCodecHeuristic(source);

                    if (!process.WaitForExit(2500))
                    {
                        try { process.Kill(); } catch { }
                        return DetectCodecHeuristic(source);
                    }

                    var output = (process.StandardOutput.ReadToEnd() + "\n" + process.StandardError.ReadToEnd()).Trim();
                    var match = System.Text.RegularExpressions.Regex.Match(output, @"Codec:\s*(.+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (match.Success)
                        return match.Groups[1].Value.Trim();
                }
            }
            catch { }

            return DetectCodecHeuristic(source);
        }

        private static string DetectCodecHeuristic(string source)
        {
            var lower = (source ?? "").ToLowerInvariant();
            if (lower.Contains("h265") || lower.Contains("hevc")) return "H.265/HEVC";
            if (lower.Contains("h264") || lower.Contains("avc")) return "H.264/AVC";
            if (lower.Contains("vp9")) return "VP9";
            if (lower.Contains("av1")) return "AV1";
            if (lower.Contains("mjpeg") || lower.Contains("jpeg")) return "MJPEG";
            return "Unknown";
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
            Brush brush = null;

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
