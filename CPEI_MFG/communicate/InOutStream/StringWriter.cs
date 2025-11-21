using System;
using System.IO;
using System.Threading.Tasks;
using CPEI_MFG.Communicate.Interface;

namespace CPEI_MFG.Communicate.InOutStream
{
    public class StringWriter : IWriter
    {
        private readonly TextWriter _writer;
        public StringWriter(TextWriter writer)
        {
            _writer = TextWriter.Synchronized(writer);
            _writer.Flush();
        }
        public void Dispose()
        {
            try
            {
                _writer?.Dispose();
            }
            catch (Exception)
            {
            }
        }

        public bool Write(string mess)
        {
            try
            {
                _writer.Write(mess);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool WriteLine(string mess)
        {
            try
            {
                _writer.WriteLine(mess);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> WriteAsync(string mess)
        {
            try
            {
                await _writer.WriteAsync(mess);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> WriteLineAsync(string mess)
        {
            try
            {
                await _writer.WriteLineAsync(mess);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
