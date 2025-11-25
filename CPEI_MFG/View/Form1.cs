using CPEI_MFG.Common;
using CPEI_MFG.Communicate;
using CPEI_MFG.Config;
using CPEI_MFG.Model;
using CPEI_MFG.Service.Condition;
using CPEI_MFG.Services;
using CPEI_MFG.Services.DHCP;
using CPEI_MFG.Services.ErrorCode;
using CPEI_MFG.Services.FTU;
using CPEI_MFG.Services.SerialControl;
using CPEI_MFG.Services.SerialControl.CCD;
using CPEI_MFG.Services.Worker;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CPEI_MFG.View
{
    public partial class Form1 : Form
    {
        private readonly TestModel testModel;
        private readonly Dhcp dhcp;
        private readonly GoldenVerify goldenVerify;
        private readonly CheckTestFailedCondition checkTestFailed;
        private readonly FtuService ftuService;
        private readonly LogAnalysis logAnalysis;
        private readonly Sfis sfis;
        private readonly SerialControlService serialControlService;
        private Thread workThread;
        enum STATUSFLAG
        {
            RUN = 0, PASS = 1, FAIL = 2, ERROR = 3, STANDBY = 4, CHECKING = 5
        }
        public Form1()
        {
            InitializeComponent();
            testModel = new TestModel();

            dhcp = new Dhcp();
            dhcp.WriteInfoLog += WriteDebugLog;

            sfis = new Sfis(testModel);
            sfis.WriteErrorLog += WriteDebugLog;
            sfis.WriteInfoLog += WriteInfoLog;

            goldenVerify = CheckConditionFactory.GetGoldenVerifyInstanceOf(0);
            checkTestFailed = CheckConditionFactory.GetCheckTestFailedConditionInstanceOf(0);

            var ccdChecker = new CcdChecker(testModel);
            ccdChecker.WriteLog += WriteDebugLog;

            ftuService = new FtuService(testModel);
            serialControlService = new SerialControlService();
            serialControlService.WriteLog += WriteDebugLog;

            logAnalysis = new LogAnalysis();
            logAnalysis.OnNewLine += WriteInfoLog;
            logAnalysis.AddService(ccdChecker);
            logAnalysis.AddService(ftuService);
            logAnalysis.AddService(new WorkerChecker(testModel));
            logAnalysis.AddService(serialControlService);
            logAnalysis.AddService(new ErrorcodeChecker());
        }

        private ProgramConfig ProgramConfig => ConfigLoader.ProgramConfig;
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                testModel.AppVer = "V2025.11.24";
                IPHostEntry host;
                host = Dns.GetHostEntry(Dns.GetHostName());
                for (int i = 0; i < host.AddressList.Count(); i++)
                {
                    if (host.AddressList[i].IsIPv6LinkLocal) continue;
                    if (host.AddressList[i].IsIPv6Multicast) continue;
                    if (host.AddressList[i].IsIPv6SiteLocal) continue;
                    if (host.AddressList[i].IsIPv6Teredo) continue;
                    if (string.IsNullOrWhiteSpace(testModel.IpAddr))
                    {
                        testModel.IpAddr = host.AddressList[i].ToString();
                        break;
                    }
                }
                testModel.PcName = Environment.MachineName.Trim();
                if (!sfis.Init()
                    || !dhcp.Init()
                    || !logAnalysis.Init()
                    )
                {
                    Application.Exit();
                }
                InitGolden();
                InitTestCondition();
                InitializeFormUI();
                InitializeFormSetting();
                InitListView1();
                if (!testModel.PcName.ToUpper().Contains(ProgramConfig.Station.ToUpper().Trim()))
                {
                    MessageBox.Show("Vui lòng gọi TE online để kiểm tra lại tên máy!");
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }

        }

        private void InitTestCondition()
        {
            var testConditionConfig = ProgramConfig.TestCondition;
            if (testConditionConfig == null)
            {
                MessageBox.Show("testConditionConfig == null");
                Application.Exit();
            }
            checkTestFailed.EnableFailCheck = testConditionConfig.IsEnableContinueFail;
            checkTestFailed.EnableOldMac = testConditionConfig.IsEnableOldMacFail;
            checkTestFailed.Spec = testConditionConfig.MaxContinueFailSpec;
        }

        private void InitGolden()
        {
            var goldenConfig = ProgramConfig.GoldenConfig;
            if (goldenConfig == null)
            {
                MessageBox.Show("goldenConfig == null");
                Application.Exit();
            }
            goldenVerify.GoodGoldenEnable = goldenConfig.IsGoodGdEnable;
            goldenVerify.BadGoldenEnable = goldenConfig.IsBadGdEnable;
            goldenVerify.Spec = goldenConfig.Time;
            goldenVerify.GoodGoldens.Clear();
            goldenVerify.BadGoldens.Clear();
            foreach (var item in goldenConfig.GoodGoldens)
            {
                goldenVerify.GoodGoldens.Add(item);
            }
            foreach (var item in goldenConfig.BadGoldens)
            {
                goldenVerify.BadGoldens.Add(item);
            }
        }



        //--------------------------Write Setup.ini-------------------------------
        private void InitializeFormUI()
        {
            this.tabPage3.Controls.Add(dhcp.DhcpControl);
            this.Text = testModel.MainWindowTitle = $"{testModel.PcName} - Ver: {testModel.AppVer}";
            this.lbVer.Text = testModel.AppVer;
            this.label_Model.Text = ProgramConfig.Model;
            this.label_Station.Text = ProgramConfig.Station;
            this.label_StationNO.Text = testModel.PcName;
            this.label_Result.Text = "Standby";
            this.label_Result.ForeColor = Color.Black;
            this.rb_SfisOFF.Visible = ProgramConfig.IsDebugModeEnable;
            SetStatusFlag(STATUSFLAG.STANDBY);
            ActiveControl = txtInput;
        }
        private void InitializeFormSetting()
        {
            this.S_Model.Text = ProgramConfig.Model;
            this.S_Station.Text = ProgramConfig.Station;
            this.S_StationNO.Text = testModel.PcName;
            this.S_LocalLogPath.Text = ProgramConfig.LocalLog;
            var ftuconfig = ProgramConfig.FtuConfig;
            this.txtFtuExecuteName.Text = ftuconfig.DirPath;
            this.txtFtuWindowTitle.Text = ftuconfig.FTUWindowsTitle;
            this.txtFtuConfig.Text = ftuconfig.ConfigPath;
            var sfisConfig = ProgramConfig.SfisConfig;
            this.S_SFISCom.Text = sfisConfig.Com;
            var verConfig = ProgramConfig.VersionConfig;
            this.S_FWVersion.Text = verConfig.FWVer;
            this.S_FCDVersion.Text = verConfig.FCDVer;
            this.S_BOMVersion.Text = verConfig.BOMVer;
            this.txtFtuVersion.Text = verConfig.FTUVer;
            this.txtRegionVersion.Text = verConfig.RegionVer;
        }
        //------------------------Initial SFIS COM-----------------------------

        private void UpdatePassFailNum()
        {

            this.Label_Pass.Text = Convert.ToString(testModel.PassNum);
            this.Label_Fail.Text = Convert.ToString(testModel.FailNum);
            if (Convert.ToDecimal(testModel.FailNum) == 0)
            {
                this.Label_Retest.Text = "0.00%";
            }
            else
            {
                Decimal nRetestRate = Convert.ToDecimal(testModel.FailNum) / Convert.ToDecimal(testModel.FailNum + testModel.PassNum) * 100;
                if (nRetestRate > 5)
                {
                    this.Label_Retest.ForeColor = Color.Red;
                }
                else
                {
                    this.Label_Retest.ForeColor = Color.Black;
                }
                this.Label_Retest.Text = Convert.ToString(nRetestRate);
                if (Label_Retest.Text.Length > 7)
                    this.Label_Retest.Text = Label_Retest.Text.Substring(0, 5) + "%";
                else
                    this.Label_Retest.Text += "%";
            }

        }
        private void InitListView1()
        {
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.GridLines = true;
            this.listView1.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.listView1.Clear();
            this.listView1.Columns.Add("ItemList", 100, HorizontalAlignment.Left);
            this.listView1.Columns.Add("Value", 150, HorizontalAlignment.Center);
            this.listView1.Items.Add("IPAddress", 1);
            this.listView1.Items.Add("SFIS_COM", 2);
            this.listView1.Items.Add("FWVersion", 3);
            this.listView1.Items.Add("FCDVersion", 4);
            this.listView1.Items.Add("FTUVersion", 5);
            this.listView1.Items.Add("BOMVersion", 6);
            this.listView1.Items.Add("RegionVersion", 7);
            this.listView1.Items.Add("Continue_Fail", 8);
            this.listView1.Items.Add("PASS_Num", 9);
            this.listView1.Items.Add("FAIL_Num", 10);


            this.listView1.Items[0].SubItems.Add(testModel.IpAddr);
            this.listView1.Items[1].SubItems.Add(ProgramConfig.SfisConfig.Com);
            var verConfig = ProgramConfig.VersionConfig;
            this.listView1.Items[2].SubItems.Add(verConfig.FWVer);
            this.listView1.Items[3].SubItems.Add(verConfig.FCDVer);
            this.listView1.Items[4].SubItems.Add(verConfig.FTUVer);
            this.listView1.Items[5].SubItems.Add(verConfig.BOMVer);
            this.listView1.Items[6].SubItems.Add(verConfig.RegionVer);
            this.listView1.Items[7].SubItems.Add(checkTestFailed.Count.ToString());
            this.listView1.Items[8].SubItems.Add(testModel.PassNum.ToString());
            this.listView1.Items[9].SubItems.Add(testModel.FailNum.ToString());
            this.listView1.Visible = true;
        }
        private void SetStatusFlag(STATUSFLAG flag, string errorCode = "", string mess = "")
        {
            if (InvokeRequired)
            {
                Invoke(new Action<STATUSFLAG, string, string>(SetStatusFlag), flag, errorCode, mess);
            }
            else
            {
                InitializeFormSetting();
                InitListView1();
                switch (flag)
                {
                    case STATUSFLAG.CHECKING:
                        this.groupBox3.Enabled = false;
                        this.tabControl1.SelectedIndex = 0;
                        this.label_Result.Text = "Checking";
                        this.label_Result.ForeColor = Color.Orange;
                        this.txtAreaLog.Text = null;
                        this.LbStatus.Text = "Checking...";
                        this.label_Error.Visible = false;
                        this.LbStatus.BackColor = Color.Orange;
                        WriteInfoLog($"{ProgramConfig.Station}  Checking...");
                        ActiveControl = txtInput;
                        break;
                    case STATUSFLAG.RUN:
                        WriteInfoLog($"{ProgramConfig.Station}  Test Pass");
                        this.groupBox3.Enabled = false;
                        this.tabControl1.SelectedIndex = 1;
                        this.testModel.CycleTime = 0;
                        this.label_Result.Text = "Testing";
                        this.label_Result.ForeColor = Color.Yellow;
                        this.txtAreaLog.Clear();
                        timer_Count.Enabled = true;
                        this.label_Error.Visible = false;
                        this.LbStatus.Text = "Testing...";
                        this.LbStatus.BackColor = Color.Yellow;
                        LbInput.Text = testModel.Input;
                        ActiveControl = txtInput;
                        break;
                    case STATUSFLAG.PASS:
                        this.groupBox3.Enabled = true;
                        txtInput.Focus();
                        txtInput.Text = string.Empty;
                        this.tabControl1.SelectedIndex = 0;
                        this.timer_Count.Enabled = false;
                        this.label_Result.Text = "PASS";
                        this.label_Result.ForeColor = Color.Green;
                        this.label_Error.Visible = false;
                        this.txtInput.Text = "";
                        this.testModel.PassNum++;
                        LbInput.Text = string.Empty;
                        this.btClear.Enabled = true;
                        this.LbStatus.Text = string.IsNullOrWhiteSpace(mess) ? "PASS" : $"PASS\r\n{mess}";
                        this.LbStatus.BackColor = Color.Green;
                        ActiveControl = txtInput;
                        break;

                    case STATUSFLAG.FAIL:
                        WriteInfoLog($"{ProgramConfig.Station}  Test Failed");
                        this.groupBox3.Enabled = true;
                        txtInput.Focus();
                        txtInput.Text = string.Empty;
                        this.tabControl1.SelectedIndex = 0;
                        this.label_Result.Text = "FAIL";
                        this.label_Result.ForeColor = Color.Red;
                        this.label_Error.Text = $"ErrorCode: \n {errorCode}";
                        this.label_Error.Visible = true;
                        this.timer_Count.Enabled = false;
                        this.testModel.FailNum++;
                        LbInput.Text = string.Empty;
                        this.btClear.Enabled = true;
                        this.LbStatus.Text = string.IsNullOrWhiteSpace(mess) ? "FAIL" : $"FAIL\r\n{errorCode}\r\n{mess}";
                        this.LbStatus.BackColor = Color.Red;
                        ActiveControl = txtInput;
                        break;
                    case STATUSFLAG.ERROR:
                        this.groupBox3.Enabled = true;
                        txtInput.Focus();
                        txtInput.Text = string.Empty;
                        this.label_Result.Text = "ERROR";
                        this.LbStatus.Text = $"ERROR: \r\n{errorCode}\r\n{mess}";
                        this.LbStatus.BackColor = Color.Brown;
                        this.label_Error.Visible = true;
                        this.label_Error.Text = $"Error: \n {errorCode}";
                        this.label_Result.ForeColor = Color.Brown;
                        this.timer_Count.Enabled = false;
                        LbInput.Text = string.Empty;
                        this.btClear.Enabled = true;
                        ActiveControl = txtInput;
                        break;
                    case STATUSFLAG.STANDBY:
                        this.groupBox3.Enabled = true;
                        txtInput.Focus();
                        txtInput.Text = string.Empty;
                        this.tabControl1.SelectedIndex = 0;
                        this.label_Result.Text = "Standby";
                        label_Error.Text = string.Empty;
                        this.txtInput.Enabled = true;
                        this.label_Result.ForeColor = Color.Black;
                        this.timer_Count.Enabled = false;
                        LbInput.Text = string.Empty;
                        this.btClear.Enabled = true;
                        this.LbStatus.Text = "Standby";
                        this.LbStatus.BackColor = Color.DarkCyan;
                        ActiveControl = txtInput;
                        break;

                }
                UpdatePassFailNum();
            }
        }

        public bool IsRuning => workThread?.IsAlive == true;
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Return)
            {
                return;
            }
            if (IsRuning)
            {
                txtInput.Focus();
                txtInput.Text = string.Empty;
                return;
            }
            testModel.Input = txtInput.Text.Trim();
            testModel.DutMO = "debugMO";
            if (testModel.Input.Length != ProgramConfig.InputMaxLength)
            {
                this.SetStatusFlag(STATUSFLAG.ERROR, $"Input length != {ProgramConfig.InputMaxLength}");
                return;
            }
            txtInput.Focus();
            if (ProgramConfig.InputMaxLength - ProgramConfig.MacIndex < 12)
            {
                MessageBox.Show($"InputMaxLength - MacIndex < 12 !!");
                SetStatusFlag(STATUSFLAG.STANDBY);
                return;
            }
            testModel.SfcPSN = testModel.Input.ToUpper();
            testModel.ScanMAC = testModel.Input.Substring(ProgramConfig.MacIndex, 12).ToUpper();
            if (!ftuService.CheckFtuConfig())
            {
                this.SetStatusFlag(STATUSFLAG.ERROR, "FTU Config");
                return;
            }
            if (goldenVerify.IsGoodGoldenMac(testModel.Input))
            {
                MessageBox.Show("This is Good Golden");
                StartTest();
                return;
            }
            if (goldenVerify.IsBadGoldenMac(testModel.Input))
            {
                MessageBox.Show("This is Bad Golden");
                StartTest();
                return;
            }
            if (rb_SfisON.Checked)
            {
                if (goldenVerify.IsTimeOutTestGoodGolden)
                {
                    MessageBox.Show("Pls! Test Good Golden");
                    SetStatusFlag(STATUSFLAG.STANDBY);
                    return;
                }
                if (goldenVerify.IsTimeOutTestBadGolden)
                {
                    MessageBox.Show("Pls! Test Bad Golden");
                    SetStatusFlag(STATUSFLAG.STANDBY);
                    return;
                }
                if (checkTestFailed.IsFailedTimeOutOfSpec)
                {
                    MessageBox.Show($"The Fail count out of Spec: '{checkTestFailed.Count >= checkTestFailed.Spec}'");
                    this.SetStatusFlag(STATUSFLAG.STANDBY);
                    return;
                }
                if (checkTestFailed.IsOldMac(testModel.ScanMAC))
                {
                    MessageBox.Show($"MAC:'{testModel.ScanMAC}' has been failed!\r\n pls! change other Unit.");
                    this.SetStatusFlag(STATUSFLAG.STANDBY);
                    return;
                }
                Task.Factory.StartNew(() =>
                {
                    SetStatusFlag(STATUSFLAG.CHECKING);
                    switch (sfis.CheckMacSfis(testModel.ScanMAC))
                    {
                        case Sfis.SfisResult.PASS:
                            StartTest();
                            break;
                        case Sfis.SfisResult.FAIL:
                            SetStatusFlag(STATUSFLAG.ERROR, "SFC_00", "Check MAC failed!");
                            break;
                        case Sfis.SfisResult.TIME_OUT:
                            SetStatusFlag(STATUSFLAG.ERROR, "TERMINAL", "Terminal time out!");
                            break;
                    }
                });
            }
            else
            {
                StartTest();
            }
        }
        private void StartTest()
        {
            SetStatusFlag(STATUSFLAG.RUN);
            if (!IsRuning)
            {
                workThread = new Thread(StationTest);
                workThread.Start();
            }
        }

        private void rb_SfisON_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_SfisON.Checked)
            {
                this.BackColor = Color.LightSkyBlue;
            }
            else
            {

                this.BackColor = Color.Yellow;
            }
        }
        private void timer_Count_Tick(object sender, EventArgs e)
        {
            testModel.CycleTime++;
            this.Label_Time.Text = $"{testModel.CycleTime} s";
        }



        private void WriteDebugLog(string msg)
        {
            if (msg == null)
            {
                SetText("\n\r");
            }
            else
            {
                string fullmsg = $"{DateTime.Now:HH:mm:ss.fff}\t{msg}";
                SetText(fullmsg, 1);
            }
        }
        private void WriteInfoLog(string msg)
        {
            if (msg == null)
            {
                SetText("\n\r");
            }
            else
            {
                string fullmsg = $"{DateTime.Now:HH:mm:ss.fff}\t{msg}";
                SetText(fullmsg, 0);
            }
        }
        private void WriteErrorLog(string msg)
        {
            if (msg == null)
            {
                SetText("\n\r");
            }
            else
            {
                string fullmsg = $"{DateTime.Now:HH:mm:ss.fff}\t{msg}";
                SetText(fullmsg, 2);
            }
        }
        private void StationTest()
        {
            DateTime startTime = DateTime.Now;
            try
            {
                WriteDebugLog($"-----{ProgramConfig.Station} Test start ------");
                Kill_testprogram();
                DeleteOldLogs();
                ResultModel result;
                if (!serialControlService.StartTest())
                {
                    result = new ResultModel("");
                    result.SetFail("RJ45IN");
                }
                else
                {
                    try
                    {
                        if (!PingDUT())
                        {
                            result = new ResultModel("");
                            result.SetFail("PINGFF");
                        }
                        else
                        {
                            ftuService.StartTest();
                            result = AnalysisLog();
                            if (!SaveLogSFTP(result.LogPath, result.IsPass, result.ErrorCode))
                            {
                                SetStatusFlag(STATUSFLAG.ERROR, "SAVE LOG FAILED", "Save test log to server failed!");
                                return;
                            }
                        }

                    }
                    finally
                    {
                        var a = Task.Run(() => serialControlService.EndTest());
                    }
                }
                EndStep(result);
            }
            catch (Exception ex)
            {
                WriteDebugLog($"{ex.Message}");
                WriteDebugLog($"{ProgramConfig.Station}  Test Failed");
                SetStatusFlag(STATUSFLAG.FAIL, "EXCEPTION", ex.Message);
            }
            finally
            {
                DeleteOldLogs();
                double CycleTime = (DateTime.Now - startTime).TotalSeconds;
                WindowControl.RaiseWindowProcess(this.Text);
                WriteDebugLog($"Total Test time : {CycleTime}");

            }
        }

        private void EndStep(ResultModel result)
        {
            bool isGolden = goldenVerify.IsGoldenMacResult(this.testModel.Input, result.IsPass);
            if (isGolden || !rb_SfisON.Checked)
            {
                if (result.IsPass)
                {
                    checkTestFailed.SetPass();
                    SetStatusFlag(STATUSFLAG.PASS);
                }
                else
                {
                    SetStatusFlag(STATUSFLAG.FAIL, result.ErrorCode);
                }
            }
            else
            {
                if (result.IsPass)
                {
                    if (sfis.SendTestResultToSFC(true, "") == Sfis.SfisResult.PASS)
                    {
                        checkTestFailed.SetPass();
                        SetStatusFlag(STATUSFLAG.PASS);
                    }
                    else
                    {
                        SetStatusFlag(STATUSFLAG.FAIL, "SFC_FF");
                    }
                }
                else
                {
                    sfis.SendTestResultToSFC(false, result.ErrorCode);
                    checkTestFailed.SetFailed(result.ErrorCode);
                    SetStatusFlag(STATUSFLAG.FAIL, result.ErrorCode);
                }
            }
        }

        private bool PingDUT()
        {
            string dutIP = ProgramConfig.DUT_IP;
            WriteDebugLog($"-----PING To '{dutIP}'------");
            if (string.IsNullOrEmpty(dutIP) || !Util.Ping(dutIP, 120000))
            {
                WriteDebugLog($"-----PING To '{dutIP}' Failed!------");
                return false;
            }
            WriteDebugLog($"-----PING To '{dutIP}' Ok------");
            return true;
        }

        private void Kill_testprogram()
        {
            Process[] myProcesses = Process.GetProcesses();
            string ftuName = ProgramConfig.FtuConfig.Name;
            foreach (Process myProcess in myProcesses)
            {
                if (!string.IsNullOrWhiteSpace(ftuName) && myProcess.ProcessName.Contains(ftuName))
                {
                    try
                    {
                        myProcess.Kill();
                    }
                    catch
                    {
                    }
                }
            }
            Process[] myProcesses1 = Process.GetProcesses();
            foreach (Process myProcess in myProcesses1)
            {
                if (myProcess.ProcessName.Contains("adb"))
                {
                    try
                    {
                        myProcess.Kill();
                    }
                    catch
                    {
                    }
                }
            }
        }

        private bool SaveLogSFTP(string file, bool result, string errorCode)
        {
            string logDir;
            string ServerlogDir;
            string dirElem = Path.Combine(ProgramConfig.Model, ProgramConfig.Station, testModel.DutMO, testModel.PcName, DateTime.Now.ToString("yyyy-MM-dd"));
            if (goldenVerify.IsGoldenMac(testModel.Input))
            {
                logDir = Path.Combine(ProgramConfig.LocalLogGolden, dirElem);
                ServerlogDir = Path.Combine(ProgramConfig.ServerLogGolden, dirElem);
            }
            else
            if (rb_SfisON.Checked)
            {
                logDir = Path.Combine(ProgramConfig.LocalLog, dirElem);
                ServerlogDir = Path.Combine(ProgramConfig.ServerlogDir, dirElem);
            }
            else
            {
                logDir = Path.Combine(ProgramConfig.LocalLogNoSFC, dirElem);
                ServerlogDir = Path.Combine(ProgramConfig.ServerLogNoSFC, dirElem);
            }
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            string strTime_log = DateTime.Now.ToString("yyyyMMddhhmmss");
            string fileName;
            if (result)
            {
                fileName = $"PASS_{testModel.ScanMAC}_{ProgramConfig.Model}_{ProgramConfig.Station}_{testModel.PcName}_{testModel.DutMO}_{strTime_log}.log";
            }
            else
            {
                fileName = $"FAIL_{testModel.ScanMAC}_{ProgramConfig.Model}_{ProgramConfig.Station}_{testModel.PcName}__{testModel.DutMO}_{strTime_log}_{errorCode}.log";
            }
            string localFilePath = Path.Combine(logDir, fileName);
            File.Copy(file, localFilePath);
            var sftpConfig = ProgramConfig.LoggerConfig;
            MySftp sftp = new MySftp(sftpConfig.Host, sftpConfig.Port, sftpConfig.User, sftpConfig.Password);
            string sftpFilePath = Path.Combine(ServerlogDir, fileName);
            while (!sftp.UploadFile(sftpFilePath, localFilePath))
            {
                WriteDebugLog($"Save the log to server: '{sftpFilePath}' failed!");
                WriteDebugLog($"Save the log to server failed, Please check the connnection to the server!\r\nIP:'{sftpConfig.Host}'");
            }
            WriteDebugLog($"Save the log to server: '{sftpFilePath}' ok");
            return true;
        }
        private void DeleteOldLogs()
        {
            try
            {
                if (Directory.Exists(ProgramConfig.FtuConfig.LogDir))
                {
                    Directory.Delete(ProgramConfig.FtuConfig.LogDir, true);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex.ToString());
                MessageBox.Show($"Delete FTU logs: {ex.Message}");
                Application.Exit();
            }
        }


        private ResultModel AnalysisLog()
        {
            try
            {
                Directory.CreateDirectory(ProgramConfig.FtuConfig.LogDir);
                logAnalysis.Reset();
                string path = FindLogPathAt(ProgramConfig.FtuConfig.LogDir);
                return logAnalysis.Analysis(path);
            }
            finally
            {
                Kill_testprogram();
            }
        }

        private string FindLogPathAt(string dir)
        {
            while (true)
            {
                FileInfo logFile = new DirectoryInfo(dir)
                    .GetFiles($"{testModel.ScanMAC}_*.log", SearchOption.TopDirectoryOnly)
                    .OrderByDescending(f => f.LastWriteTime).FirstOrDefault();
                if (logFile != null)
                {
                    return logFile.FullName;
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                sfis.SendSYANTE5();
            }
            catch (Exception ex)
            {
                WriteDebugLog(ex.Message);
            }
        }
        private void SetText(string msg, int iColor = 0)
        {
            try
            {
                if (this.txtAreaLog.InvokeRequired)
                {
                    txtAreaLog.Invoke(new Action<string, int>(SetText), new object[] { msg, iColor });
                }
                else
                {
                    if (iColor == 1)
                    {
                        this.txtAreaLog.SelectionColor = Color.Blue;
                        this.txtAreaLog.SelectionStart = txtAreaLog.Text.Length;
                        this.txtAreaLog.SelectedText = msg + Environment.NewLine;
                    }
                    else if (iColor == 2)
                    {
                        this.txtAreaLog.SelectionColor = Color.Red;
                        this.txtAreaLog.SelectionStart = txtAreaLog.Text.Length;
                        this.txtAreaLog.SelectedText = msg + Environment.NewLine;
                    }
                    else
                    {
                        this.txtAreaLog.SelectionColor = Color.Black;
                        this.txtAreaLog.SelectionStart = txtAreaLog.Text.Length;
                        this.txtAreaLog.SelectedText = msg + Environment.NewLine;
                    }
                    this.txtAreaLog.ScrollToCaret();
                }
            }
            catch (Exception)
            {
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Kill_testprogram();
                if (IsRuning)
                {
                    workThread.Abort();
                }
                dhcp?.Stop();
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}