using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using PayECR;
using System.IO;
using System.Diagnostics;

namespace serialCommunicationECR
{
    public partial class ECRip : Form
    {
        //SerialPort _serialPort;
        PayECR.ECR ecr;
        public String command = "";
        public string comport = "COM1";
        string ReceiptPath = @"C:\ECR_Receipts\";
        string ls_receive = "", ls_status = "", ls_data;
        int li_status = 0;
        public ECRip()
        {
            InitializeComponent();

        }

        private void ECRip_Load(object sender, EventArgs e)
        {
            lbl_portname.Text = comport;
            ecr = new ECR(comport, 120000, 2000, "C:\\ECR_LOG", true, true);
            textBox1.Text = command;
            if (command != "C207")
            {
                textBox2.Text = "00";
                textBox3.Text = "100";
            }
           

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
               
                ecr._stop = true; // send stop signal to lib 
                backgroundWorker1.CancelAsync();

                button2.Enabled = false;
                richTextBox1.Text = " plz wait, canceling trns...";
            }


            button3.Show();
            this.Refresh();
            this.Dispose();
            Form1 f1 = new Form1();
            f1.comport = comport;
            f1.Show();
            this.Refresh();
        } //disconnect button

        private void button3_Click(object sender, EventArgs e)//send button
        {
            // Makes sure serial port is open before trying to write
            if (command == "C902" || command == "C912" || command == "C241" || command == "C910" || command == "C610" || command == "C906")
            { echo(); }

            else if (command == "C700")
            {
                textBox1.Text = command;
                command += textBox2.Text.PadLeft(19, '0');
                command += textBox3.Text;
                string ls_receive = "", ls_status = "";
                int li_status = 0;


                ecr.SendReceive(command, ref ls_receive, ref li_status, ref ls_status, 2000);
                richTextBox1.Text = MsgFormat(ls_receive);
                lbl_status.Text = ls_status;
                command = textBox1.Text;

            }
            else
            {               

                if (textBox2.TextLength < 2)
                {
                    MessageBox.Show("Command Not found!!");
                    return;
                }

                if (command == "C290")
                {
                    if (textBox4.TextLength < 2)
                    {
                        MessageBox.Show("Invalid QR ID!!");
                        return;
                    }
                }        


                if (!backgroundWorker1.IsBusy)
                {
                    ls_data = CommanAllTx();
                    backgroundWorker1.RunWorkerAsync();

                }

                else
                {
                    lbl_status.Text = "busy processing, plz wait";
                }

            }
        }


        public void echo()
        {
            textBox1.Text = command;
            string ls_receive = "", ls_status = "";
            int li_status = 0;
            ecr.SendReceive(command, ref ls_receive, ref li_status, ref ls_status, 2000);

            richTextBox1.Text = MsgFormat(ls_receive);
            lbl_status.Text = ls_status;

        }
        public string CommanAllTx()
        {
            textBox1.Text = command;


            // for requery command
            if (command.Contains("C208"))
            {
                command += textBox4.Text.PadRight(24, ' ');
                return command;
            }

            if (command.Contains("C242"))
            {
                command += textBox2.Text.PadLeft(12, '0');// amount for instant Reward
            }
            else
            {
                command += textBox2.Text;//host no
            }

            


            if (command.Contains("C500") || command.Contains("C242"))
            {
                return command;
            }

            if (command.Contains( "C245") || command.Contains( "C246"))

            {
                command += textBox3.Text;
                return command;
            }

           


            command += textBox3.Text.PadLeft(12, '0');

            if (textBox1.Text == "C203" || textBox1.Text == "C292")
            { command += textBox4.Text.PadLeft(12, '0'); }
            else if (textBox1.Text == "C201")
            {
                command += textBox4.Text.PadLeft(6, '0');
            }

            if (textBox1.Text == "C290")
            {
                string QRCode = textBox6.Text;

                 string totalL = QRCode.Length.ToString().PadLeft(4, '0');// to get total length of qrcode
               // string totalL = "0088";
                command += totalL + QRCode;

            }

            if (textBox1.Text == "C292")
            {
                if (textBox2.Text == "03")
                {
                    string QRCode = textBox6.Text;

                    string totalL = QRCode.Length.ToString().PadLeft(4, '0');// to get total length of qrcode

                    command += totalL + QRCode;
                }
                else 
                {
                    command += textBox6.Text.PadLeft(32, ' ');
                }
               
            }

            if (textBox1.Text != "C201")
            {
                if (textBox1.Text == "C203")
                {
                    command += textBox5.Text.PadRight(24, ' ');
                }
                else
                {
                    command += textBox4.Text.PadRight(24, ' ');
                }
            }

            //ecr.SendReceive(command, ref ls_receive, ref li_status, ref ls_status, 1000);

            return command;

        }

       

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (backgroundWorker1.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            else
            {
                try
                {
            
                  
                    ecr.SendReceive(ls_data, ref ls_receive, ref li_status, ref ls_status,2000);
                    if (ls_receive != "")
                    { ProcessReceiveString(ls_receive); }
                }
                catch (Exception ex)
                {
                    backgroundWorker1.CancelAsync();
                    MessageBox.Show(ex.Message, "Error!");

                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {

                ecr._stop = true; // send stop signal to lib 
                backgroundWorker1.CancelAsync();

                button2.Enabled = false;
                richTextBox1.Text = " plz wait, canceling trns...";
            }
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {

                MessageBox.Show("Process cancelled");
                button2.Enabled = true;

            }

            else if (e.Error != null)
            {

                MessageBox.Show("processing error =" + e.Error.Message);
                this.Controls.Clear();
                this.InitializeComponent();

            }
            else
            {
                richTextBox1.Text = MsgFormat(ls_receive);
                lbl_status.Text = ls_status;
               
                button2.Enabled = true;
                command = textBox1.Text;
            }
        }

        //for receipt
        private void ProcessReceiveString(string ReceivedMessege)
        {
            string ResponseMsg = "",
                CardNo = "", ExpiryDate = "", StatusCode = "", ApprovalCode = "", RRN = "", TransactionTrace = "",
                BatchNumber = "", HostNo = "", TID = "", MID = "", AID = "", TC = "", CardholderName = "", CardType = "",
                PartnerTrxID = "", AlipayTrxID = "", CustomerID = "", Amount = "", BatchCount = "", BatchAmount = "",AppTrxID= "",MAHTrxID = "";
            string FileName = "";

            ResponseMsg = ReceivedMessege.Substring(0, 4);
            string response = ResponseMsg;

            if (ResponseMsg == "R200")
            {
                ResponseMsg = "SALE";
                CardNo = ReceivedMessege.Substring(4, 19);
                ExpiryDate = ReceivedMessege.Substring(23, 4);
                StatusCode = ReceivedMessege.Substring(27, 2);
                ApprovalCode = ReceivedMessege.Substring(29, 6);
                RRN = ReceivedMessege.Substring(35, 12);
                TransactionTrace = ReceivedMessege.Substring(47, 6);
                BatchNumber = ReceivedMessege.Substring(53, 6);
                HostNo = ReceivedMessege.Substring(59, 2);
                TID = ReceivedMessege.Substring(61, 8);
                MID = ReceivedMessege.Substring(69, 15);
                AID = ReceivedMessege.Substring(84, 14);
                TC = ReceivedMessege.Substring(98, 16);
                CardholderName = ReceivedMessege.Substring(114, 26);
                CardType = ReceivedMessege.Substring(140, 2);

                if (ReceivedMessege.Length > 158)
                {
                    MAHTrxID = ReceivedMessege.Substring(142, 16);
                    AppTrxID = ReceivedMessege.Substring(158, 16);
                }
               
            }
            else if (ResponseMsg == "R201")
            {
                ResponseMsg = "VOID";
                Amount = ReceivedMessege.Substring(4, 12);
                StatusCode = ReceivedMessege.Substring(16, 2);
                ApprovalCode = ReceivedMessege.Substring(18, 6);
                RRN = ReceivedMessege.Substring(24, 12);
                TransactionTrace = ReceivedMessege.Substring(36, 6);
                BatchNumber = ReceivedMessege.Substring(42, 6);
                HostNo = ReceivedMessege.Substring(48, 2);
                if (ReceivedMessege.Length > 50)
                {
                    PartnerTrxID = ReceivedMessege.Substring(50, 32);
                    AlipayTrxID = ReceivedMessege.Substring(82, 64);
                    CustomerID = ReceivedMessege.Substring(146, 26);
                }
                
            }
            else if (ResponseMsg == "R500")
            {
                ResponseMsg = "Settlement";
                HostNo = ReceivedMessege.Substring(4, 2);
                StatusCode = ReceivedMessege.Substring(6, 2);
                BatchNumber = ReceivedMessege.Substring(8, 6);
                BatchCount = ReceivedMessege.Substring(14, 3);
                BatchAmount = ReceivedMessege.Substring(17, 12);

            }

            else if (ResponseMsg == "R290")
            {
                ResponseMsg = "SALE";
                StatusCode = ReceivedMessege.Substring(4, 2);
                ApprovalCode = ReceivedMessege.Substring(6, 6);
                TransactionTrace = ReceivedMessege.Substring(12, 6);
                BatchNumber = ReceivedMessege.Substring(18, 6);
                HostNo = ReceivedMessege.Substring(24, 2);
                TID = ReceivedMessege.Substring(26, 8);
                MID = ReceivedMessege.Substring(34, 15);

                PartnerTrxID = ReceivedMessege.Substring(49, 32);
                AlipayTrxID = ReceivedMessege.Substring(81, 64);
                if (ReceivedMessege.Length > 145)
                    CustomerID = ReceivedMessege.Substring(145, 26);


            }

            if (StatusCode == "00")
            {
                if (textBox7.Text != "")
                {
                    FileName = textBox7.Text + ".TXT"; ;
                }
                else
                {
                     FileName = ResponseMsg + "_" + DateTime.Now.ToString("ddMMyyHHmm") + ".TXT";
                }

                string NewFilePath = ReceiptPath + FileName;
                if (!Directory.Exists(ReceiptPath))

                {
                    Directory.CreateDirectory(ReceiptPath);

                }
                if (!File.Exists(NewFilePath))

                {
                    File.Create(NewFilePath).Close();


                }

                if (response == "R290")

                {
                    using (StreamWriter Receipt = File.AppendText(NewFilePath))
                    {
                        Receipt.WriteLine("HOST: " + HostNo + "\n");
                        Receipt.WriteLine("TID: " + TID + "\n");
                        Receipt.WriteLine("MID: " + MID + "\n");
                        Receipt.WriteLine("\t" + ResponseMsg + "\n");
                        Receipt.WriteLine("DATE/TIME: " + DateTime.Now.ToString() + "\n");
                        Receipt.WriteLine();
                        Receipt.WriteLine("APP CODE :        " + ApprovalCode + "\n");
                        Receipt.WriteLine("BATCH #  :        " + BatchNumber + "\n");
                        Receipt.WriteLine("TRACE #  :        " + TransactionTrace + "\n");
                        Receipt.WriteLine("PrtnrTrxID #  :   " + PartnerTrxID + "\n");
                        Receipt.WriteLine("AlpayTrxID #  :   " + AlipayTrxID + "\n");
                        Receipt.WriteLine("CustomerID #  :   " + CustomerID + "\n");
                       
                        Receipt.WriteLine("AMOUNT RM     " + textBox3.Text + "\n");

                    }


                }

                else if (response == "R201")

                {
                    using (StreamWriter Receipt = File.AppendText(NewFilePath))
                    {
                        Receipt.WriteLine("HOST: " + HostNo + "\n");
                        Receipt.WriteLine("TID: " + TID + "\n");
                        Receipt.WriteLine("MID: " + MID + "\n");
                        Receipt.WriteLine("\t" + ResponseMsg + "\n");
                        Receipt.WriteLine("BATCH #  :        " + BatchNumber + "\n");
                        Receipt.WriteLine("TRACE #  :        " + TransactionTrace + "\n");
                        Receipt.WriteLine("DATE/TIME: " + DateTime.Now.ToString() + "\n");
                        Receipt.WriteLine("RRN REF #:  " + RRN + "\n");
                        Receipt.WriteLine("APP CODE :        " + ApprovalCode + "\n");
                        Receipt.WriteLine(AID + "\n");
                        Receipt.WriteLine("AMOUNT RM     -" + textBox3.Text + "\n");
                        Receipt.WriteLine();
                    }


                }
                else if (response == "R500")
                {
                    
                    using (StreamWriter Receipt = File.AppendText(NewFilePath))
                    {
                        Receipt.WriteLine("HOST: " + HostNo + "\n");

                        Receipt.WriteLine("\t" + ResponseMsg + "\n");

                        Receipt.WriteLine("DATE/TIME: " + DateTime.Now.ToString() + "\n");
                        Receipt.WriteLine();
                        Receipt.WriteLine("BATCH #        :   " + BatchNumber + "\n");
                        Receipt.WriteLine("BatchCount #   :   " + BatchCount + "\n");
                        Receipt.WriteLine("BatchAmount #  :   " + BatchAmount + "\n");
                    }
                }
                else
                {
                    using (StreamWriter Receipt = File.AppendText(NewFilePath))
                    {
                        Receipt.WriteLine("HOST: " + HostNo + "\n");
                        Receipt.WriteLine("TID: " + TID + "\n");
                        Receipt.WriteLine("MID: " + MID + "\n");
                        Receipt.WriteLine("\t" + ResponseMsg + "\n");
                        Receipt.WriteLine("  " + CardNo + "\n");
                        Receipt.WriteLine("BATCH #  :        " + BatchNumber + "\n");
                        Receipt.WriteLine("TRACE #  :        " + TransactionTrace + "\n");
                        Receipt.WriteLine("DATE/TIME: " + DateTime.Now.ToString() + "\n");
                        Receipt.WriteLine("RRN REF #:  " + RRN + "\n");
                        Receipt.WriteLine("APP CODE :        " + ApprovalCode + "\n");
                        Receipt.WriteLine(AID + "\n");
                        Receipt.WriteLine("CRYPT    :" + TC + "\n");
                        Receipt.WriteLine("App Trx ID #:     " + AppTrxID + "\n");
                        Receipt.WriteLine("MAH Trx ID #:     " + MAHTrxID + "\n");
                        Receipt.WriteLine("AMOUNT RM     " + textBox3.Text + "\n");

                    }
                }

            }
        }

        private string MsgFormat(string ReceivedMessege)
        {
            string msgPacket = "";

            if (ReceivedMessege != "")
            {
                string ResponseMsg = "",
                  CardNo = "", ExpiryDate = "", StatusCode = "", ApprovalCode = "", RRN = "", TransactionTrace = "",
                  BatchNumber = "", HostNo = "", TID = "", MID = "", AID = "", TC = "", CardholderName = "", CardType = "",
                  PartnerTrxID = "", AlipayTrxID = "", CustomerID = "", MAHTxnID = "", AppTxnID = "", ProdBrand = "", TotalRMB = "", ExchangeRate = "", Amount = "", BatchCount = "", BatchAmount = "", StatusDesc = "",
                  AppId = "", Currency = "", TotalAmount = "", CashFeeType = "", CashFee = "", OrderNo = "", MBBTransId = "", EndTime = "",
                  UserAccount = "", CardTypeDesc = "", Recall = "";

                ResponseMsg = ReceivedMessege.Substring(0, 4);

                #region response msg
                if (ResponseMsg == "R200")
                {
                    ResponseMsg = "SALE";
                    CardNo = ReceivedMessege.Substring(4, 19);
                    ExpiryDate = ReceivedMessege.Substring(23, 4);
                    StatusCode = ReceivedMessege.Substring(27, 2);
                    ApprovalCode = ReceivedMessege.Substring(29, 6);
                    RRN = ReceivedMessege.Substring(35, 12);
                    TransactionTrace = ReceivedMessege.Substring(47, 6);
                    BatchNumber = ReceivedMessege.Substring(53, 6);
                    HostNo = ReceivedMessege.Substring(59, 2);
                    TID = ReceivedMessege.Substring(61, 8);
                    MID = ReceivedMessege.Substring(69, 15);
                    AID = ReceivedMessege.Substring(84, 14);
                    TC = ReceivedMessege.Substring(98, 16);
                    CardholderName = ReceivedMessege.Substring(114, 26);
                    CardType = ReceivedMessege.Substring(140, 2);


                    // display in window
                    msgPacket += "Response :" + ResponseMsg + "\n";
                    msgPacket += "CardNo :" + CardNo + "\n";
                    msgPacket += "ExpiryDate :" + ExpiryDate + "\n";
                    msgPacket += "StatusCode :" + StatusCode + "\n";
                    msgPacket += "ApprovalCode :" + ApprovalCode + "\n";
                    msgPacket += "RRN :" + RRN + "\n";
                    msgPacket += "TransactionTrace :" + TransactionTrace + "\n";
                    msgPacket += "BatchNumber :" + BatchNumber + "\n";
                    msgPacket += "HostNo :" + HostNo + "\n";
                    msgPacket += "TID :" + TID + "\n";
                    msgPacket += "MID :" + MID + "\n";
                    msgPacket += "AID :" + AID + "\n";
                    msgPacket += "TC :" + TC + "\n";
                    msgPacket += "CardholderName :" + CardholderName + "\n";
                    msgPacket += "CardType :" + CardType + "\n";

                    if (ReceivedMessege.Length > 142)
                    {
                        PartnerTrxID = ReceivedMessege.Substring(222, 32);
                        AlipayTrxID = ReceivedMessege.Substring(254, 64);
                        CustomerID = ReceivedMessege.Substring(280, 26);

                        // msg
                        msgPacket += "PartnerTrxID :" + PartnerTrxID + "\n";
                        msgPacket += "AlipayTrxID :" + AlipayTrxID + "\n";
                        msgPacket += "CustomerID :" + CustomerID + "\n";
                    }

                }
                else if (ResponseMsg == "R201")
                {
                    ResponseMsg = "VOID";
                    Amount = ReceivedMessege.Substring(4, 12);
                    StatusCode = ReceivedMessege.Substring(16, 2);
                    ApprovalCode = ReceivedMessege.Substring(18, 6);
                    RRN = ReceivedMessege.Substring(24, 12);
                    TransactionTrace = ReceivedMessege.Substring(36, 6);
                    BatchNumber = ReceivedMessege.Substring(42, 6);
                    HostNo = ReceivedMessege.Substring(48, 2);


                    // display in window
                    msgPacket += "Response :" + ResponseMsg + "\n";
                    msgPacket += "Amount :" + Amount + "\n";
                    msgPacket += "StatusCode :" + StatusCode + "\n";
                    msgPacket += "ApprovalCode :" + ApprovalCode + "\n";
                    msgPacket += "RRN :" + RRN + "\n";
                    msgPacket += "TransactionTrace :" + TransactionTrace + "\n";
                    msgPacket += "BatchNumber :" + BatchNumber + "\n";
                    msgPacket += "HostNo :" + HostNo + "\n";


                    if (ReceivedMessege.Length > 50)
                    {
                        PartnerTrxID = ReceivedMessege.Substring(50, 32);
                        AlipayTrxID = ReceivedMessege.Substring(82, 64);
                        CustomerID = ReceivedMessege.Substring(146, 26);

                        // msg
                        msgPacket += "PartnerTrxID :" + PartnerTrxID + "\n";
                        msgPacket += "AlipayTrxID :" + AlipayTrxID + "\n";
                        msgPacket += "CustomerID :" + CustomerID + "\n";
                    }

                }
                if (ResponseMsg == "R203")
                {
                    ResponseMsg = "REFUND";
                    CardNo = ReceivedMessege.Substring(4, 19);
                    ExpiryDate = ReceivedMessege.Substring(23, 4);
                    StatusCode = ReceivedMessege.Substring(27, 2);
                    ApprovalCode = ReceivedMessege.Substring(29, 6);
                    RRN = ReceivedMessege.Substring(35, 12);
                    TransactionTrace = ReceivedMessege.Substring(47, 6);
                    BatchNumber = ReceivedMessege.Substring(53, 6);
                    HostNo = ReceivedMessege.Substring(59, 2);
                    TID = ReceivedMessege.Substring(61, 8);
                    MID = ReceivedMessege.Substring(69, 15);
                    AID = ReceivedMessege.Substring(84, 14);
                    TC = ReceivedMessege.Substring(98, 16);
                    CardholderName = ReceivedMessege.Substring(114, 26);
                    CardType = ReceivedMessege.Substring(140, 2);


                    // display in window
                    msgPacket += "Response :" + ResponseMsg + "\n";
                    msgPacket += "CardNo :" + CardNo + "\n";
                    msgPacket += "ExpiryDate :" + ExpiryDate + "\n";
                    msgPacket += "StatusCode :" + StatusCode + "\n";
                    msgPacket += "ApprovalCode :" + ApprovalCode + "\n";
                    msgPacket += "RRN :" + RRN + "\n";
                    msgPacket += "TransactionTrace :" + TransactionTrace + "\n";
                    msgPacket += "BatchNumber :" + BatchNumber + "\n";
                    msgPacket += "HostNo :" + HostNo + "\n";
                    msgPacket += "TID :" + TID + "\n";
                    msgPacket += "MID :" + MID + "\n";
                    msgPacket += "AID :" + AID + "\n";
                    msgPacket += "TC :" + TC + "\n";
                    msgPacket += "CardholderName :" + CardholderName + "\n";
                    msgPacket += "CardType :" + CardType + "\n";

                    if (ReceivedMessege.Length > 142)
                    {
                        string amount = ReceivedMessege.Substring(222, 12);
                        string originalAmount = ReceivedMessege.Substring(234, 12);
                        PartnerTrxID = ReceivedMessege.Substring(246, 32);
                        AlipayTrxID = ReceivedMessege.Substring(278, 64);
                        CustomerID = ReceivedMessege.Substring(342, 26);

                        // msg
                        msgPacket += "Amount :" + amount + "\n";
                        msgPacket += "Original Amount :" + amount + "\n";
                        msgPacket += "PartnerTrxID :" + PartnerTrxID + "\n";
                        msgPacket += "AlipayTrxID :" + AlipayTrxID + "\n";
                        msgPacket += "CustomerID :" + CustomerID + "\n";
                    }
                }
                else if (ResponseMsg == "R500")
                {
                    ResponseMsg = "Settlement";
                    string allHostMessage = ReceivedMessege.Substring(4);
                    for (int i = 0; i < allHostMessage.Length; i = i + 25)
                    {
                        HostNo = allHostMessage.Substring(i, 2);
                        StatusCode = allHostMessage.Substring(i + 2, 2);
                        BatchNumber = allHostMessage.Substring(i + 4, 6);
                        BatchCount = allHostMessage.Substring(i + 10, 3);
                        BatchAmount = allHostMessage.Substring(i + 13, 12);

                        // display in window
                        if (i == 0)
                        {
                            msgPacket += "\n";
                        }
                        msgPacket += "Response :" + ResponseMsg + "\n";
                        msgPacket += "HostNo :" + HostNo + "\n";
                        msgPacket += "StatusCode :" + StatusCode + "\n";
                        msgPacket += "BatchNumber :" + BatchNumber + "\n";
                        msgPacket += "BatchCount :" + BatchCount + "\n";
                        msgPacket += "BatchAmount :" + BatchAmount + "\n";
                    }
                }

                else if (ResponseMsg == "R290")
                {
                    ResponseMsg = "SALE";
                    StatusCode = ReceivedMessege.Substring(4, 2);
                    ApprovalCode = ReceivedMessege.Substring(6, 6);
                    TransactionTrace = ReceivedMessege.Substring(12, 6);
                    BatchNumber = ReceivedMessege.Substring(18, 6);
                    HostNo = ReceivedMessege.Substring(24, 2);
                    TID = ReceivedMessege.Substring(26, 8);
                    MID = ReceivedMessege.Substring(34, 15);

                    PartnerTrxID = ReceivedMessege.Substring(49, 32);
                    AlipayTrxID = ReceivedMessege.Substring(81, 64);
                    if (ReceivedMessege.Length > 145)
                        CustomerID = ReceivedMessege.Substring(145, 26);


                }

                else if (ResponseMsg == "Q200")
                {
                    ResponseMsg = "QR SALE";
                    StatusCode = ReceivedMessege.Substring(4, 2);
                    StatusDesc = ReceivedMessege.Substring(6, 32);
                    TransactionTrace = ReceivedMessege.Substring(38, 6);
                    BatchNumber = ReceivedMessege.Substring(44, 6);
                    HostNo = ReceivedMessege.Substring(50, 2);
                    TID = ReceivedMessege.Substring(52, 8);
                    MID = ReceivedMessege.Substring(60, 15);


                    msgPacket += "Response :" + ResponseMsg + "\n";
                    msgPacket += "StatusCode :" + StatusCode + "\n";
                    msgPacket += "StatusDesc :" + StatusDesc + "\n";
                    msgPacket += "TransactionTrace :" + TransactionTrace + "\n";
                    msgPacket += "BatchNumber :" + BatchNumber + "\n";
                    msgPacket += "HostNo :" + HostNo + "\n";
                    msgPacket += "TID :" + TID + "\n";
                    msgPacket += "MID :" + MID + "\n";

                    if (ReceivedMessege.Length > 75)
                    {
                        AppId = ReceivedMessege.Substring(75, 8);
                        Currency = ReceivedMessege.Substring(83, 3);
                        TotalAmount = ReceivedMessege.Substring(86, 12);
                        CashFeeType = ReceivedMessege.Substring(98, 3);
                        CashFee = ReceivedMessege.Substring(101, 12);
                        OrderNo = ReceivedMessege.Substring(113, 14);
                        int mbbTransIdLength = Convert.ToInt16(ReceivedMessege.Substring(127, 4));
                        MBBTransId = ReceivedMessege.Substring(131, mbbTransIdLength);
                        EndTime = ReceivedMessege.Substring(131 + mbbTransIdLength, 14);
                        UserAccount = ReceivedMessege.Substring(145 + mbbTransIdLength, 128);
                        CardTypeDesc = ReceivedMessege.Substring(273 + mbbTransIdLength, 100);

                        // display in window
                        msgPacket += "AppId :" + AppId + "\n";
                        msgPacket += "Currency :" + Currency + "\n";
                        msgPacket += "TotalAmount :" + TotalAmount + "\n";
                        msgPacket += "CashFeeType :" + CashFeeType + "\n";
                        msgPacket += "CashFee :" + CashFee + "\n";
                        msgPacket += "OrderNo :" + OrderNo + "\n";
                        msgPacket += "MBBTransId :" + MBBTransId + "\n";
                        msgPacket += "EndTime :" + EndTime + "\n";
                        msgPacket += "UserAccount :" + UserAccount + "\n";
                        msgPacket += "CardTypeDesc :" + CardTypeDesc + "\n";
                    }

                }
                else if (ResponseMsg == "Q203")
                {
                    ResponseMsg = "QR SALE";
                    StatusCode = ReceivedMessege.Substring(4, 2);
                    StatusDesc = ReceivedMessege.Substring(6, 32);
                    TransactionTrace = ReceivedMessege.Substring(38, 6);
                    BatchNumber = ReceivedMessege.Substring(44, 6);
                    HostNo = ReceivedMessege.Substring(50, 2);
                    TID = ReceivedMessege.Substring(52, 8);
                    MID = ReceivedMessege.Substring(60, 15);


                    msgPacket += "Response :" + ResponseMsg + "\n";
                    msgPacket += "StatusCode :" + StatusCode + "\n";
                    msgPacket += "StatusDesc :" + StatusDesc + "\n";
                    msgPacket += "TransactionTrace :" + TransactionTrace + "\n";
                    msgPacket += "BatchNumber :" + BatchNumber + "\n";
                    msgPacket += "HostNo :" + HostNo + "\n";
                    msgPacket += "TID :" + TID + "\n";
                    msgPacket += "MID :" + MID + "\n";

                    if (ReceivedMessege.Length > 75)
                    {
                        AppId = ReceivedMessege.Substring(75, 8);
                        Currency = ReceivedMessege.Substring(83, 3);
                        TotalAmount = ReceivedMessege.Substring(86, 12);
                        string refundNo = ReceivedMessege.Substring(98, 32);
                        int mbbTransIdLength = Convert.ToInt16(ReceivedMessege.Substring(130, 4));
                        MBBTransId = ReceivedMessege.Substring(134, mbbTransIdLength);
                        int refundIdLength = Convert.ToInt16(ReceivedMessege.Substring(134 + mbbTransIdLength, 4));
                        string refundId = ReceivedMessege.Substring(138 + mbbTransIdLength, refundIdLength);
                        EndTime = ReceivedMessege.Substring(138 + mbbTransIdLength + refundIdLength, 14);
                        UserAccount = ReceivedMessege.Substring(152 + mbbTransIdLength + refundIdLength, 128);
                        CardTypeDesc = ReceivedMessege.Substring(280 + mbbTransIdLength + refundIdLength, 100);

                        // display in window
                        msgPacket += "AppId :" + AppId + "\n";
                        msgPacket += "Currency :" + Currency + "\n";
                        msgPacket += "TotalAmount :" + TotalAmount + "\n";
                        msgPacket += "Refund No :" + refundNo + "\n";
                        msgPacket += "MBBTransId :" + MBBTransId + "\n";
                        msgPacket += "Refund ID :" + refundId + "\n";
                        msgPacket += "EndTime :" + EndTime + "\n";
                        msgPacket += "UserAccount :" + UserAccount + "\n";
                        msgPacket += "CardTypeDesc :" + CardTypeDesc + "\n";
                    }
                }
                else if (ResponseMsg == "G200")
                {
                    ResponseMsg = "QR SALE";
                    StatusCode = ReceivedMessege.Substring(4, 2);
                    TransactionTrace = ReceivedMessege.Substring(6, 6);
                    BatchNumber = ReceivedMessege.Substring(12, 6);
                    HostNo = ReceivedMessege.Substring(18, 2);
                    TID = ReceivedMessege.Substring(20, 8);
                    MID = ReceivedMessege.Substring(28, 15);

                    msgPacket += "Response :" + ResponseMsg + "\n";
                    msgPacket += "StatusCode :" + StatusCode + "\n";
                    msgPacket += "TransactionTrace :" + TransactionTrace + "\n";
                    msgPacket += "BatchNumber :" + BatchNumber + "\n";
                    msgPacket += "HostNo :" + HostNo + "\n";
                    msgPacket += "TID :" + TID + "\n";
                    msgPacket += "MID :" + MID + "\n";

                    if (ReceivedMessege.Length > 43)
                    {
                        MAHTxnID = ReceivedMessege.Substring(43, 40);
                        AppTxnID = ReceivedMessege.Substring(83, 40);
                        ProdBrand = ReceivedMessege.Substring(123, 40);
                        TotalRMB = ReceivedMessege.Substring(163, 16);
                        ExchangeRate = ReceivedMessege.Substring(179, 16);

                        // msg
                        msgPacket += "MAHTxnID :" + MAHTxnID + "\n";
                        msgPacket += "AppTxnID :" + AppTxnID + "\n";
                        msgPacket += "ProdBrand :" + ProdBrand + "\n";
                        msgPacket += "TotalRMB :" + TotalRMB + "\n";
                        msgPacket += "ExchangeRate :" + ExchangeRate + "\n";
                    }
                }
                else if (ResponseMsg == "R906")
                {
                    ResponseMsg = "QR SALE";
                    StatusCode = ReceivedMessege.Substring(4, 2);
                    int length = Convert.ToInt16(ReceivedMessege.Substring(6, 4));
                    string qrCode = ReceivedMessege.Substring(10, length);

                    msgPacket += "Response :" + ResponseMsg + "\n";
                    msgPacket += "StatusCode :" + StatusCode + "\n";
                    msgPacket += "QR Code Value :" + qrCode + "\n";
                }

                #endregion


            }

            if (msgPacket == "")
            {
                msgPacket = ReceivedMessege;
            }

            return msgPacket;

        }
    }
}