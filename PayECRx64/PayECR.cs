/*
 * 
 * 
 * Application : ECR Communication interface.
 * Author : Paysys Communications SDN BHD
 * Developer: Sayapathy Lagumaran
 * Date Created : 02-Jan-2015
 * Last Update : 09-Jun-2016
 * Remark: added hex string log
 * Version 1.0.1
*/

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;
using System.IO;
using System.Threading;

namespace PayECRx64
{
    public class ECR
    {
        private static string gs_com;
        private static int gi_baudRate;
        private static int gs_databits;

        private static Parity g_parity;
        private static StopBits g_stopbits;

        private static int gi_timeout;
        private int gi_buff_size;
        private SerialPort _serialPort;

        private static String LogPath;
        private static String ErrorLogPath;
        private static bool Do_Log;
        private static bool Do_Error_Log;

        //for background worker
        private Byte[] Abort = System.Text.Encoding.ASCII.GetBytes("ABORT");
        public bool _stop = false;

        public ECR(string _com, int _baudrate, int _databits, string _parity, string _stopbits, int _timeout, int _receive_buffer_size, string _log_path, bool _log, bool _error_log)
        {

            if (_com != null && _com != "" && _com.ToUpper().Contains("COM")) { gs_com = _com; } else { gs_com = "COM1"; }

            if (_parity != null && _parity != "")
            {
                if (_parity == "NONE") { g_parity = Parity.None; }
                else if (_parity == "EVEN") { g_parity = Parity.Even; }
                else if (_parity == "MARK") { g_parity = Parity.Mark; }
                else if (_parity == "ODD") { g_parity = Parity.Odd; }
                else if (_parity == "SPACE") { g_parity = Parity.Space; }
                else { g_parity = Parity.None; }
            }
            else { g_parity = Parity.None; }

            if (_stopbits != null && _stopbits != "")
            {
                if (_stopbits == "NONE") { g_stopbits = StopBits.None; }
                else if (_stopbits == "ONE") { g_stopbits = StopBits.One; }
                else if (_stopbits == "ONEPOINTFIVE") { g_stopbits = StopBits.OnePointFive; }
                else if (_stopbits == "TWO") { g_stopbits = StopBits.Two; }
                else { g_stopbits = StopBits.None; }
            }
            else { g_stopbits = StopBits.None; }

            if (_databits > 0)
            {
                gs_databits = _databits;
            }
            else
                gs_databits = 8;

            if (_baudrate > 0)
            {
                gi_baudRate = _baudrate;
            }
            else
                gi_baudRate = 9600;

            if (_timeout > 0)
            {
                gi_timeout = _timeout;
            }
            else
                gi_timeout = 120000;

            if (_receive_buffer_size > 0)
            {
                gi_buff_size = _receive_buffer_size;
            }
            else
                gi_buff_size = 1000;

            if (_log_path != "")
            {
                if (IsWrite(_log_path))
                {
                    LogPath = _log_path;
                    ErrorLogPath = _log_path;
                    Do_Log = _log;
                    Do_Error_Log = _error_log;
                }
                else
                {
                    Do_Log = false;
                    Do_Error_Log = false;
                }
            }
            else
            {
                Do_Log = false;
                Do_Error_Log = false;
            }


        }

        public ECR(string _com, int _timeout, int _receive_buffer_size, string _log_path, bool _log, bool _error_log)
        {
            if (_com != null && _com != "" && _com.ToUpper().Contains("COM")) { gs_com = _com; } else { gs_com = "COM1"; }
            gi_baudRate = 9600;
            gs_databits = 8;
            g_parity = Parity.None;
            g_stopbits = StopBits.One;
            if (_timeout > 0)
            {
                gi_timeout = _timeout;
            }
            else
                gi_timeout = 120000;

            if (_receive_buffer_size > 0)
            {
                gi_buff_size = _receive_buffer_size;
            }
            else
                gi_buff_size = 1000;

            if (_log_path != "")
            {
                if (IsWrite(_log_path))
                {
                    LogPath = _log_path;
                    ErrorLogPath = _log_path;
                    Do_Log = _log;
                    Do_Error_Log = _error_log;
                }
                else
                {
                    Do_Log = false;
                    Do_Error_Log = false;
                }
            }
            else
            {
                Do_Log = false;
                Do_Error_Log = false;
            }
        }
        /*
         * status code 0 - success,1 - ENQ failed, 2 - Write timeout, 3 - Read timeout,4- Null COM,5 Receive Wrong LRC
         * view windows event log for more details
         * log path c:/ECR_LOG/         
         */
        public void SendReceive(string writedata, ref string readdata, ref int status_code, ref string status_remark, int receive_buffer_size)
        {
            try
            {
                using (_serialPort = new SerialPort(gs_com, gi_baudRate, g_parity, gs_databits, g_stopbits))
                {
                    int li_lrc = 0;
                    string ls_send = "";
                    ls_send = "\x02";
                    status_code = 0;
                    writedata = writedata + "\x03";
                    li_lrc = computeLRC(writedata);

                    ls_send = ls_send + writedata + Convert.ToChar(li_lrc);

                    try
                    {
                        if (!_serialPort.IsOpen) { _serialPort.Open(); }

                        //if (Enquiry(ref status_code, ref status_remark))
                        //{
                            if (WriteToCOM(ref status_code, ref status_remark, ls_send))
                            {
                                if (ReadFromCOM(ref status_code, ref status_remark, ref readdata))
                                {
                                    _serialPort.Close();
                                    //status_remark = "Success";
                                }
                                else
                                    _serialPort.Close();
                            }
                            else
                                _serialPort.Close();
                        //}
                        //else
                        //    _serialPort.Close();

                    }
                    catch (TimeoutException ex)
                    {
                        status_code = 4;
                        status_remark = ex.Message;
                        ErrorLog(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                status_code = 4;
                status_remark = ex.Message;
                ErrorLog(ex.Message);
            }
        }

        private bool Enquiry(ref int status_code, ref string status_remark)
        {
            bool lb_success = false;
            int li_retry = 0, li_response = 0;
            status_code = 0;
            _serialPort.WriteTimeout = 2000;
            _serialPort.ReadTimeout = 2000;
            while (li_retry < 4)
            {
                li_retry += 1;
                try
                {
                    //if third attempt send EOT;
                    if (li_retry == 4)
                    {
                        status_code = 1;
                        status_remark = "ENQ failed";
                        //Log("ENQ:-" + status_remark);
                        _serialPort.Write("\x04");
                        Log("ENQ:-Write <EOT>");
                        System.Threading.Thread.Sleep(1);
                        break;
                    }
                    //if timeout send EOT
                    if (status_code == 1)
                    {
                        _serialPort.Write("\x04");
                        Log("ENQ:-Write <EOT>");
                        Thread.Sleep(1);
                    }

                    //set write timeout
                    _serialPort.Write("\x05");
                    Log("ENQ:-Write <ENQ>");
                    Thread.Sleep(5*1000);
                    li_response = _serialPort.ReadChar();

                    if (_stop)
                    {
                        _serialPort.Write("ABORT");
                        Log("WriteToCOM:-Write <ABORT>");
                        break;
                    }
                    //if success then return
                    if (li_response == 6)
                    {
                        li_retry = 3;
                        lb_success = true;
                        status_code = 0;
                        status_remark = "ENQ Success";
                        Log("ENQ:-Read <ACK> " + status_remark);
                        break;
                    }//if megative ack
                    else if (li_response == 21)
                    {
                        //li_retry = 3;
                        //lb_success = false;
                        status_code = 1;
                        status_remark = "ENQ failed";
                        Log("ENQ:-Read <NAK> " + status_remark);
                        //break;
                    }
                    else
                        Log("ENQ:-Read " + asciiOctets2String(System.Text.Encoding.UTF8.GetBytes(li_response.ToString())));

                }
                catch (TimeoutException ex)
                {

                    status_code = 1;
                    status_remark = ex.Message;
                    ErrorLog("ENQ- " + ex.Message);
                    ErrorLog("ENQ:- ENQ Timeout...");
                }
            }
            return lb_success;
        }

        private bool WriteToCOM(ref int status_code, ref string status_remark, string writedata)
        {
            bool lb_success = false;
            int li_retry = 0, li_response = 0;
            status_code = 0;
            _serialPort.WriteTimeout = gi_timeout;
            _serialPort.ReadTimeout = gi_timeout;
            while (li_retry < 3)
            {
                li_retry += 1;
                try
                {
                    //if timeout send EOT
                    if (status_code == 2)
                    {
                        _serialPort.Write("\x04");
                        Log("WriteToCOM:-Write <EOT>");
                        System.Threading.Thread.Sleep(1);
                    }

                    _serialPort.Write(writedata);
                    Log("WriteToCOM:-Write " + asciiOctets2String(System.Text.Encoding.UTF8.GetBytes(writedata)));
                    li_response = _serialPort.ReadChar();

                    if (_stop)
                    {
                        _serialPort.Write("ABORT");
                        Log("WriteToCOM:-Write <ABORT>");
                        break;
                    }
                    //if success then return
                    if (li_response == 6)
                    {
                        li_retry = 3;
                        lb_success = true;
                        status_code = 0;
                        status_remark = "Write Sucess";
                        Log("WriteToCOM:-Read <ACK> " + status_remark);
                        break;
                    }//if megative ack
                    else if (li_response == 21)
                    {
                        //li_retry = 3;
                        lb_success = false;
                        status_code = 2;
                        status_remark = "Write failed";
                        Log("WriteToCOM:-Read <NAK> " + status_remark);
                        //break;
                    }
                    else
                        Log("WriteToCOM:-Read " + asciiOctets2String(System.Text.Encoding.UTF8.GetBytes(li_response.ToString())));

                }
                catch (TimeoutException ex)
                {
                    status_code = 2;
                    status_remark = ex.Message;
                    ErrorLog("WriteToCOM:- " + ex.Message);
                }
            }
            return lb_success;
        }

        private bool ReadFromCOM(ref int status_code, ref string status_remark, ref string readdata)
        {
            bool lb_success = false;
            int li_retry = 0, li_response = 0;
            status_code = 0;
            _serialPort.WriteTimeout = gi_timeout;
            _serialPort.ReadTimeout = gi_timeout;
            byte[] recebuff;
            if (gi_buff_size > 1000)
                recebuff = new byte[gi_buff_size];
            else
                recebuff = new byte[1000];

            //wait for terminal enq
            li_response = _serialPort.ReadChar();

            //if success then return
            if (li_response == 5)
            {
                Log("ReadFromCOM:-Read <ENQ>");
                //acknowledge 
                _serialPort.Write("\x06");
                Log("ReadFromCOM:-Write <ACK>");
                _serialPort.ReadTimeout = 2000;

                while (li_retry < 3)
                {
                    li_retry += 1;
                    try
                    {
                        while (_serialPort.BytesToRead < 1)

                        {
                            if (_stop)
                            {
                                _serialPort.Write("ABORT");
                                Log("WriteToCOM:-Write <ABORT>");
                                return lb_success;
                            }
                        }
                        li_response = RcvData(recebuff, 0, 1000);
                        //computeLRCforByte(recebuff);                       
                        readdata = System.Text.ASCIIEncoding.ASCII.GetString(recebuff);

                        ////added on 3-Jun-2016
                        //int li_start = 0;
                        //int li_end = 0;
                        //li_start = readdata.IndexOf("\x02");
                        //li_end = readdata.IndexOf("\x03");

                        try
                        {
                            Log("ReadFromCOM:-Read in Hex " + BitConverter.ToString(recebuff, 0, li_response).Replace("-", string.Empty));
                        }
                        catch (Exception exx)
                        {
                            ErrorLog(exx.Message);
                        }

                        Log("ReadFromCOM:-Read " + asciiOctets2String((System.Text.Encoding.UTF8.GetBytes(readdata.Replace("\0", String.Empty).Trim()))));

                        ////validate LRC
                        //string ls_rtn = readdata.Substring(1, li_response - 2);
                        ////string ls_rtn = readdata.Substring(li_start + 1, li_end - (li_start + 1));

                        ////if (Convert.ToChar(readdata.Substring(li_response - 1, 1)) == Convert.ToChar(computeLRC(ls_rtn)))
                        ////{
                        if (computeLRCforByte(recebuff))
                        {
                            status_remark = "LRC OK";
                            Log("ReadFromCOM:-" + status_remark);
                            _serialPort.Write("\x06");
                            Log("ReadFromCOM:-Write <ACK>");
                            readdata = readdata.Substring(1, li_response - 3);
                            lb_success = true;
                            li_retry = 3;
                            status_code = 0;
                            break;
                        }
                        else
                        {
                            status_remark = "LRC Error!";
                            Log("ReadFromCOM:-" + status_remark);
                            _serialPort.Write("\x15");
                            Log("ReadFromCOM:-Write <NAK>");
                            _serialPort.ReadTimeout = 2000;
                            status_code = 5;
                            lb_success = false;
                        }

                    }
                    catch (TimeoutException ex)
                    {
                        status_code = 1;
                        status_remark = ex.Message;
                        ErrorLog("ReadFromCOM:- " + ex.Message);
                    }
                }
                //wait for end of transmission
                try
                {
                    _serialPort.ReadTimeout = 2000;
                    int li_eot = _serialPort.ReadChar();
                    if (li_eot == 4)
                    {
                        status_remark = "EOT Sucess";
                        Log("ReadFromCOM:-Read <EOT>");
                    }

                }
                catch (Exception et)
                {

                    status_remark = "EOT Timeout- " + et.Message;
                    ErrorLog("ReadFromCOM:- " + status_remark); ;
                }

            }//if megative ack instead of enq something wrong




            else if (li_response == 21)
            {
                //retry already faild this would be the 3rd try so exit the loop
                li_retry = 3;
                lb_success = false;
                status_code = 3;
                status_remark = " Read NAK";
                Log("ReadFromCOM:- <NAK>" + status_remark);

            }
            else
                Log("ReadFromCOM:- Read " + asciiOctets2String(System.Text.Encoding.UTF8.GetBytes(li_response.ToString())));

            return lb_success;
        }

        private bool VerifyLRC(string orig_data, string rtn_data, int length)
        {
            bool lb_rtn = false;
            if (rtn_data.IndexOf("R200") >= 0 && rtn_data.IndexOf("12\u0003") >= 0)
            {
                Log("LRC bypassed!");
                lb_rtn = true;
            }
            else if (Convert.ToChar(orig_data.Substring(length - 1, 1)) == Convert.ToChar(computeLRC(rtn_data)))
            {
                lb_rtn = true;
            }
            else
                lb_rtn = false;

            return lb_rtn;
        }
        private int computeLRC(String str)
        {

            int LRC = 0;
            for (int i = 0; i < str.Length; i++)
                LRC = LRC ^ str[i];
            return LRC;
        }

        private bool computeLRCforByte(byte[] str)
        {
            char lc_stx = '\u0002';
            char lc_etx = '\u0003';

            int li_stx = -1;
            int li_etx = -1;

            int LRC = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == lc_stx)
                    li_stx = i;
                if (str[i] == lc_etx)
                    li_etx = i;
                if (i > li_stx && li_etx < 0)
                    LRC = LRC ^ str[i];

                if (i == li_etx)
                {
                    LRC = LRC ^ str[li_etx];
                    break;
                }
            }

            if (LRC == str[li_etx + 1])
                return true;
            else
                return false;

        }

        private int RcvData(byte[] rcvBuff, int offset, int MaxBytes)
        {
            int len = 0;
            int readbytes = 0;
            while (true)
            {
                if (len == _serialPort.BytesToRead && len >= 4)
                {
                    if (readbytes < MaxBytes)
                        readbytes = _serialPort.Read(rcvBuff, offset, len);
                    else
                        readbytes = _serialPort.Read(rcvBuff, offset, MaxBytes);

                    return readbytes;
                }
                len = _serialPort.BytesToRead;
                System.Threading.Thread.Sleep(100);
            }
            // return 0; GETTING WARNING WHILE COMPILE...

        }
        private void logreport(string message)
        {
            //callback(message);
        }

        private void logevent(string message)
        {
            //if (!EventLog.SourceExists("PayECR"))
            //    EventLog.CreateEventSource("PayECR", "Application");
            //EventLog.WriteEntry("PayECR", message, EventLogEntryType.Error);
        }

        //public void ExportToFile(string filename)
        //{
        //    var permissionSet = new PermissionSet(PermissionState.None);
        //    var writePermission = new FileIOPermission(FileIOPermissionAccess.Write, filename);
        //    permissionSet.AddPermission(writePermission);

        //    if (permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet))
        //    {
        //        using (FileStream fstream = new FileStream(filename, FileMode.Create))
        //        using (TextWriter writer = new StreamWriter(fstream))
        //        {
        //            // try catch block for write permissions 
        //            writer.WriteLine("sometext");
        //        }
        //    }
        //    else
        //    {
        //        //perform some recovery action here
        //    }

        //}


        private static void Log(String outStr)
        {
            if (Do_Log)
            {
                //StreamWriter writer;
                String FileName;
                String txtLog;
                String txtDate;
                if (LogPath.Length < 3)
                    return;
                try
                {
                    if (!Directory.Exists(LogPath))
                    {
                        Directory.CreateDirectory(LogPath);
                    }
                    DateTime dateToday = new DateTime();
                    dateToday = DateTime.Now;
                    txtDate = String.Format("{0:0000}{1:00}{2:00}", dateToday.Year, dateToday.Month, dateToday.Day);
                    FileName = LogPath + "/L_P_" + txtDate + ".txt";

                    txtLog = dateToday.ToLongTimeString() + " " + outStr;

                    using (StreamWriter writer = System.IO.File.AppendText(FileName))
                    {
                        writer.WriteLine(txtLog);
                        //Console.SetOut(writer);
                        //Console.WriteLine(txtLog);
                        //writer.Close();
                        //GC.Collect();
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private static void ErrorLog(String outStr)
        {
            if (Do_Error_Log)
            {
                //StreamWriter writer;
                String FileName;
                String txtLog;
                String txtDate;
                try
                {
                    if (!Directory.Exists(ErrorLogPath))
                    {
                        Directory.CreateDirectory(ErrorLogPath);
                    }
                    DateTime dateToday = new DateTime();
                    dateToday = DateTime.Now;
                    txtDate = String.Format("{0:0000}{1:00}{2:00}", dateToday.Year, dateToday.Month, dateToday.Day);
                    FileName = ErrorLogPath + "/E_P_" + txtDate + ".txt";

                    txtLog = dateToday.ToLongTimeString() + " " + outStr;

                    using (StreamWriter writer = System.IO.File.AppendText(FileName))
                    {
                        writer.WriteLine(txtLog);
                        //Console.SetOut(writer);
                        //Console.WriteLine(txtLog);
                        //writer.Close();
                        //GC.Collect();
                    }
                }
                catch (Exception)
                {
                    //Console.WriteLine("logging failed");
                    //Console.WriteLine(e.Message);
                }
            }
        }

        private bool IsWrite(string as_path)
        {
            FileIOPermission f2 = new FileIOPermission(FileIOPermissionAccess.Write, as_path);
            try
            {
                f2.Demand();
                return true;
            }
            catch (SecurityException s)
            {
                return false;
            }
        }

        static string asciiOctets2String(byte[] bytes)
        {
            try
            {
                StringBuilder sb = new StringBuilder(bytes.Length);
                foreach (char c in System.Text.Encoding.UTF8.GetString(bytes).ToCharArray())
                {
                    switch (c)
                    {
                        case '\u0000': sb.Append("<NUL>"); break;
                        case '\u0001': sb.Append("<SOH>"); break;
                        case '\u0002': sb.Append("<STX>"); break;
                        case '\u0003': sb.Append("<ETX>"); break;
                        case '\u0004': sb.Append("<EOT>"); break;
                        case '\u0005': sb.Append("<ENQ>"); break;
                        case '\u0006': sb.Append("<ACK>"); break;
                        case '\u0007': sb.Append("<BEL>"); break;
                        case '\u0008': sb.Append("<BS>"); break;
                        case '\u0009': sb.Append("<HT>"); break;
                        case '\u000A': sb.Append("<LF>"); break;
                        case '\u000B': sb.Append("<VT>"); break;
                        case '\u000C': sb.Append("<FF>"); break;
                        case '\u000D': sb.Append("<CR>"); break;
                        case '\u000E': sb.Append("<SO>"); break;
                        case '\u000F': sb.Append("<SI>"); break;
                        case '\u0010': sb.Append("<DLE>"); break;
                        case '\u0011': sb.Append("<DC1>"); break;
                        case '\u0012': sb.Append("<DC2>"); break;
                        case '\u0013': sb.Append("<DC3>"); break;
                        case '\u0014': sb.Append("<DC4>"); break;
                        case '\u0015': sb.Append("<NAK>"); break;
                        case '\u0016': sb.Append("<SYN>"); break;
                        case '\u0017': sb.Append("<ETB>"); break;
                        case '\u0018': sb.Append("<CAN>"); break;
                        case '\u0019': sb.Append("<EM>"); break;
                        case '\u001A': sb.Append("<SUB>"); break;
                        case '\u001B': sb.Append("<ESC>"); break;
                        case '\u001C': sb.Append("<FS>"); break;
                        case '\u001D': sb.Append("<GS>"); break;
                        case '\u001E': sb.Append("<RS>"); break;
                        case '\u001F': sb.Append("<US>"); break;
                        case '\u007F': sb.Append("<DEL>"); break;
                        default:
                            if (c > '\u007F')
                            {
                                sb.AppendFormat(@"\u{0:X4}", (ushort)c); // in ASCII, any octet in the range 0x80-0xFF doesn't have a character glyph associated with it
                            }
                            else
                            {
                                sb.Append(c);
                            }
                            break;
                    }
                }
                return sb.ToString();

            }
            catch (Exception e)
            {
                return "";
            }
        }

    }
}
