using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using CPEI_MFG.Communicate.InOutStream;
using StringWriter = CPEI_MFG.Communicate.InOutStream.StringWriter;

namespace CPEI_MFG.Communicate.Implement.Serial
{
    public class MySerialPort : BaseCommunicate
    {
        private readonly SerialPort serialPort;
        private readonly DynamicTextReader outPutReader;
        public MySerialPort(string name, int baudrate) : this(name, baudrate, null) { }
        public MySerialPort(string name, int baudrate, Action<MySerialPort, string> dataReceivedAction)
        {
            Name = name;
            DataReceivedAction = dataReceivedAction;
            serialPort = new SerialPort(name, baudrate)
            {
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                Encoding = Encoding.UTF8
            };
            outPutReader = new DynamicTextReader();
            OutPutReader = outPutReader;
            serialPort.DataReceived += (_, o) =>
            {
                try
                {
                    string line = serialPort.ReadLine()?.Trim();
                    DataReceivedAction?.Invoke(this, line);
                    outPutReader.AddLine(line);
                }
                catch (Exception)
                {
                    DataReceivedAction?.Invoke(this, null);
                    outPutReader.AddLine(null);
                }
            };
        }
        public bool TryGetLine(out string line, int msTimeout, CancellationToken token)
        {
            return outPutReader.TryGetLine(out line, msTimeout, token);
        }
        public bool TryGetLine(out string line, int msTimeout)
        {
            return outPutReader.TryGetLine(out line, msTimeout);
        }
        public bool TryGetLine(out string line)
        {
            return outPutReader.TryGetLine(out line);
        }
        public Action<MySerialPort, string> DataReceivedAction { private get; set; }
        public string Name { get; }
        protected override void Close()
        {
            serialPort?.Dispose();
        }

        public static string[] GetPortNames()
        {
            return SerialPort.GetPortNames();
        }

        public override bool Connect()
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    serialPort.Open();
                    Thread.Sleep(500);
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                    InputWriter = new StringWriter(new StreamWriter(serialPort.BaseStream, Encoding.UTF8, 2048, leaveOpen: true)
                    {
                        AutoFlush = true
                    });
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override bool IsConnect => serialPort.IsOpen;

        public override bool Disconnect()
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
