using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace serialCommunicationECR
{
    public partial class Form1 : Form
    {
        ECRip f1 = new ECRip();
        public string comport = "COM1";
        public Form1()
        {
            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();
            comboBox1.Items.AddRange(ports);
            comboBox1.Text = comport;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //f1.comport = this.comboBox1.Text;
            comboBox1.Text = comport;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        //echo
        private void button13_Click(object sender, EventArgs e)
        {

            this.Hide();
            // ECRip f1 = new ECRip();
            f1.textBox2.Hide();
            f1.textBox3.Hide();
            f1.textBox4.Hide();
            f1.textBox5.Hide();
            f1.label1.Hide();
            f1.label2.Hide();
            f1.label3.Hide();
            f1.label7.Hide();
            f1.label10.Hide();
            f1.textBox6.Hide();
            f1.command = "C902";
            f1.label6.Text = "ECHO";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();

        }
        //void
        private void button4_Click(object sender, EventArgs e)
        {
            f1.command = "";
            this.Hide();
            // ECRip f1 = new ECRip();
            f1.label7.Hide();
            f1.label10.Hide();
            f1.textBox6.Hide();
            f1.textBox5.Hide();
            f1.label3.Text = "TRACE NUMBER";
            f1.command = "C201";
            f1.label6.Text = "VOID";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();
        }
        //adjust
        private void button5_Click(object sender, EventArgs e)
        {
            f1.command = "";
            this.Hide();
            f1.label2.Text = "TRACE NUMBER";
            f1.label3.Text = "TIP AMOUNT (RM)";
            f1.label7.Text = "ORG. AMOUNT (RM)";
            // ECRip f1 = new ECRip();
            f1.command = "C220";
            f1.label6.Text = "ADJUST";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();
        }
        //refund
        private void button6_Click(object sender, EventArgs e)
        {
            f1.command = "";
            this.Hide();

            f1.label10.Hide();
            f1.textBox6.Hide();
            f1.label7.Text = "Additional Data";
            f1.command = "C203";
            f1.label6.Text = "REFUND";
            f1.label3.Text = "Original Amount";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();
        }
        //settlement
        private void button2_Click_1(object sender, EventArgs e)
        {
            f1.command = "";
            this.Hide();

            f1.textBox3.Hide();
            f1.textBox4.Hide();
            f1.textBox5.Hide();
            f1.label2.Hide();
            f1.label3.Hide();
            f1.label7.Hide();
            f1.label10.Hide();
            f1.textBox6.Hide();
            f1.command = "C500";
            f1.label6.Text = "SETTLEMENT";
            f1.comport = comboBox1.Text;
            f1.Show();
        }
        //sale
        private void button3_Click(object sender, EventArgs e)
        {
            //ECRip f1 = new ECRip();
            f1.command = "";
            this.Hide();
            f1.label7.Hide();
            f1.label10.Hide();
            f1.textBox6.Hide();
            f1.textBox5.Hide();
            f1.command = "C200";
            f1.label6.Text = "SALE";
            f1.label3.Text = "Additional Data";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();
        }


        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        //aliPay sale
        private void button19_Click(object sender, EventArgs e)
        {

            ECRip f1 = new ECRip();
            f1.command = "";
            this.Hide();
            f1.label7.Hide();
            f1.textBox5.Hide();
            f1.command = "C290";
            f1.label6.Text = "SALE";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();

        }

        //AliPay Refund
        private void button16_Click(object sender, EventArgs e)
        {
            f1.command = "";
            this.Hide();
            f1.label7.Hide();
            f1.textBox5.Hide();
            f1.command = "C292";
            f1.label10.Text = "Partner TxnID";
            f1.label6.Text = "REFUND";
            f1.label3.Text = "Original Amount";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();

        }
        //CASH BACK
        private void button25_Click(object sender, EventArgs e)
        {

            ECRip f1 = new ECRip();
            f1.command = "";
            this.Hide();
            f1.label7.Hide();
            f1.textBox5.Hide();
            f1.command = "C205";
            f1.label6.Text = "CASH BACK";
            f1.label3.Text = "CASH BACK \nAmount (RM)";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();
        }

        private void button26_Click(object sender, EventArgs e)
        {

            ECRip f1 = new ECRip();
            f1.command = "";
            this.Hide();
            f1.label3.Hide();
            f1.label7.Hide();
            f1.label10.Hide();
            f1.textBox4.Hide();
            f1.textBox5.Hide();
            f1.textBox6.Hide();
            f1.label1.Text = "Card Number";
            f1.label2.Text = "Session Key";
            f1.command = "C700";
            f1.label6.Text = "PIN Entry";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();

        }
        //pre-Auth
        private void button27_Click(object sender, EventArgs e)
        {
            f1.command = "";
            this.Hide();
            f1.label7.Hide();
            f1.label10.Hide();
            f1.textBox6.Hide();
            f1.textBox5.Hide();
            f1.command = "C100";
            f1.label6.Text = "Pre Auth";
            f1.label3.Text = "Additional Data";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();
        }

        // Read Card
        private void button26_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            // ECRip f1 = new ECRip();
            f1.textBox2.Hide();
            f1.textBox3.Hide();
            f1.textBox4.Hide();
            f1.textBox5.Hide();
            f1.label1.Hide();
            f1.label2.Hide();
            f1.label3.Hide();
            f1.label7.Hide();
            f1.label10.Hide();
            f1.textBox6.Hide();
            f1.command = "C910";
            f1.label6.Text = "Read Card";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();
        }

        // Get TID and MID
        // C904
        private void button28_Click(object sender, EventArgs e)
        {
            f1.command = "";
            this.Hide();

            f1.textBox3.Hide();
            f1.textBox4.Hide();
            f1.textBox5.Hide();
            f1.label2.Hide();
            f1.label3.Hide();
            f1.label7.Hide();
            f1.label10.Hide();
            f1.textBox6.Hide();
            f1.command = "C904";
            f1.label6.Text = "GetIDs";
            f1.comport = comboBox1.Text;
            f1.Show();
        }

        // Sale Completion
        private void button31_Click(object sender, EventArgs e)
        {

            f1.command = "";
            this.Hide();

            f1.label10.Hide();
            f1.textBox6.Hide();
            f1.command = "C230";
            f1.label6.Text = "Sale Completion";
            f1.label3.Text = "Approval Code";
            f1.label7.Text = "Additional Data";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();
        }

        //instant reward
        private void button42_Click(object sender, EventArgs e)
        {
            f1.command = "";
            this.Hide();
            f1.label1.Text = "AMOUNT (RM)";
            f1.label2.Hide();
            f1.label3.Hide();
            f1.label7.Hide();
            f1.textBox3.Hide();
            f1.textBox4.Hide();
            f1.textBox5.Hide();

            f1.label10.Hide();
            f1.textBox6.Hide();

            f1.label6.Text = "INSTANT REWARD";
            f1.command = "C242";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();

        }

        // point enquiry
        private void button44_Click(object sender, EventArgs e)
        {
            f1.command = "";
            this.Hide();
            f1.textBox2.Hide();
            f1.textBox3.Hide();
            f1.textBox4.Hide();
            f1.textBox5.Hide();
            f1.label1.Hide();
            f1.label2.Hide();
            f1.label3.Hide();
            f1.label7.Hide();
            this.Refresh();
            f1.label6.Text = "POINT ENQURY";
            f1.command = "C241";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();

        }

        //point redemption
        private void button38_Click(object sender, EventArgs e)
        {
            f1.command = "";
            this.Hide();
            f1.label1.Text = "ENTER POINTS";
            f1.label2.Hide();
            f1.label3.Hide();
            f1.label7.Hide();
            f1.textBox3.Hide();
            f1.textBox4.Hide();
            f1.textBox5.Hide();
            f1.label10.Hide();
            f1.textBox6.Hide();
            f1.label6.Text = "POINT REDEMPTION";
            f1.command = "C243";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();

        }

        //value redemption
        private void button40_Click(object sender, EventArgs e)
        {
            f1.command = "";
            this.Hide();
            f1.label1.Text = "AMOUNT (RM)";
            f1.label2.Hide();
            f1.label3.Hide();
            f1.label7.Hide();
            f1.label10.Hide();
            f1.textBox3.Hide();
            f1.textBox4.Hide();
            f1.textBox5.Hide();
            f1.textBox6.Hide();
            f1.label6.Text = "VALUE REDEMPTION";
            f1.command = "C244";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();

        }

        //hot deal
        private void button32_Click(object sender, EventArgs e)
        {
            f1.command = "";
            this.Hide();
            f1.label1.Text = "ENTER GIFT CODE";
            f1.label2.Text = "ENTER QUANTITY";
            f1.label3.Hide();
            f1.label7.Hide();
            f1.textBox4.Hide();
            f1.textBox5.Hide();

            f1.label10.Hide();
            f1.textBox6.Hide();

            f1.label6.Text = "HOT DEAL";
            f1.command = "C246";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();

        }

        // gift redemption
        private void button36_Click(object sender, EventArgs e)
        {
            f1.command = "";
            this.Hide();
            f1.label1.Text = "ENTER GIFT CODE";
            f1.label2.Text = "ENTER QUANTITY";
            f1.label3.Hide();
            f1.label7.Hide();
            f1.label10.Hide();
            f1.textBox4.Hide();
            f1.textBox5.Hide();

            f1.label10.Hide();
            f1.textBox6.Hide();

            f1.label6.Text = "GIFT REDEMTION";
            f1.command = "C245";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();
        }

        //ORC VOid
        private void button29_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 f2 = new Form2();
            f2.comport = comboBox1.Text;
            f2.Show();
            this.Refresh();


        }
        // read MyKad
        private void button34_Click(object sender, EventArgs e)
        {
            this.Hide();
            // ECRip f1 = new ECRip();
            f1.textBox2.Hide();
            f1.textBox3.Hide();
            f1.textBox4.Hide();
            f1.textBox5.Hide();
            f1.label1.Hide();
            f1.label2.Hide();
            f1.label3.Hide();
            f1.label7.Hide();
            f1.label10.Hide();
            f1.textBox6.Hide();
            f1.command = "C610";
            f1.label6.Text = "Read MyKad";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();

        }


        // Get Token
        private void button7_Click(object sender, EventArgs e)
        {
            this.Hide();
            // ECRip f1 = new ECRip();
            f1.textBox2.Hide();
            f1.textBox3.Hide();
            f1.textBox4.Hide();
            f1.textBox5.Hide();
            f1.label1.Hide();
            f1.label2.Hide();
            f1.label3.Hide();
            f1.label7.Hide();
            f1.label10.Hide();
            f1.textBox6.Hide();
            f1.command = "C911";
            f1.label6.Text = "Get Token";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();
        }

        //parking-pre Auth
        private void button8_Click(object sender, EventArgs e)
        {           

            f1.command = "";
            this.Hide();
            f1.label7.Hide();
            f1.label10.Hide();
            f1.textBox6.Hide();
            f1.textBox5.Hide();
            f1.command = "C101";
            f1.label6.Text = "Pre Auth_Parking";
            f1.label3.Text = "Additional Data";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();

        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.Hide();
            // ECRip f1 = new ECRip();
            f1.textBox2.Hide();
            f1.textBox3.Hide();
            f1.textBox4.Hide();
            f1.textBox5.Hide();
            f1.label1.Hide();
            f1.label2.Hide();
            f1.label3.Hide();
            f1.label7.Hide();
            f1.label10.Hide();
            f1.textBox6.Hide();
            f1.command = "C906";
            f1.label6.Text = "Scan QR/Barcode";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();

        }

        private void button10_Click(object sender, EventArgs e)
        {

            this.Hide();

            f1.textBox2.Hide();
            f1.textBox3.Hide();
            //f1.textBox4.Hide();
            f1.textBox5.Hide();
            f1.label1.Hide();
            f1.label2.Hide();
           // f1.label3.Hide();
            f1.label7.Hide();
            f1.label10.Hide();
            f1.textBox6.Hide();
            f1.command = "C208";
            f1.label6.Text = "MBB QRPAY Requery";
            f1.label3.Text = "Trx ID";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();

        }

        private void button11_Click(object sender, EventArgs e)
        {
            this.Hide();
            // ECRip f1 = new ECRip();
            f1.textBox2.Hide();
            f1.textBox3.Hide();
            f1.textBox4.Hide();
            f1.textBox5.Hide();
            f1.label1.Hide();
            f1.label2.Hide();
            f1.label3.Hide();
            f1.label7.Hide();
            f1.label10.Hide();
            f1.textBox6.Hide();
            f1.command = "C921200727183003";
            f1.label6.Text = "Set Time";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();
        }
        // read mifare
        private void button12_Click(object sender, EventArgs e)
        {
            this.Hide();
            // ECRip f1 = new ECRip();
            f1.textBox2.Hide();
            f1.textBox3.Hide();
            f1.textBox4.Hide();
            f1.textBox5.Hide();
            f1.label1.Hide();
            f1.label2.Hide();
            f1.label3.Hide();
            f1.label7.Hide();
            f1.label10.Hide();
            f1.textBox6.Hide();
            f1.command = "C912";
            f1.label6.Text = "MiFare";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();
        }

        // read genting card
        private void button14_Click(object sender, EventArgs e)
        {
            this.Hide();
            // ECRip f1 = new ECRip();
            f1.textBox2.Hide();
            f1.textBox3.Hide();
            f1.textBox4.Hide();
            f1.textBox5.Hide();
            f1.label1.Hide();
            f1.label2.Hide();
            f1.label3.Hide();
            f1.label7.Hide();
            f1.label10.Hide();
            f1.textBox6.Hide();
            f1.command = "C800";
            f1.label6.Text = "Genting";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            f1.command = "";
            this.Hide();
            f1.label1.Text = "AMOUNT (RM)";
            f1.label2.Hide();
            f1.label3.Hide();
            f1.label7.Hide();
            f1.label10.Hide();
            f1.textBox3.Hide();
            f1.textBox4.Hide();
            f1.textBox5.Hide();
            f1.textBox6.Hide();
            f1.label6.Text = "OTSR REDEMPTION";
            f1.command = "C275";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            f1.command = "";
            this.Hide();
            f1.textBox2.Hide();
            f1.textBox3.Hide();
            f1.textBox4.Hide();
            f1.textBox5.Hide();
            f1.textBox6.Hide();
            f1.label10.Hide();
            f1.label1.Hide();
            f1.label2.Hide();
            f1.label3.Hide();
            f1.label7.Hide();
            this.Refresh();
            f1.label6.Text = "OTSR ENQUIRY";
            f1.command = "C276";
            f1.comport = comboBox1.Text;
            f1.Show();
            this.Refresh();

        }
    }
}