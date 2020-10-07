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
    public partial class Form2 : Form
    {
        ECRip f1 = new ECRip();
        public string comport = "COM1";
        public Form2()
        {
            InitializeComponent();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            f1.command = "";
            this.Hide();
            // ECRip f1 = new ECRip();
            f1.label7.Hide();
            f1.textBox5.Hide();
            f1.label3.Text = "TRACE NUMBER";
            f1.label1.Text = "ORS HOST NUMBER";
            f1.command = "C342";
            f1.label6.Text = "VOID INSTANT REWARD";
            f1.comport = comport;
            f1.Show();
            this.Refresh();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            f1.command = "";
            this.Hide();
            f1.label7.Hide();
            f1.textBox5.Hide();
            f1.command = "C342";
            f1.label6.Text = "VOID GIFT REDEMPTION";
            f1.label1.Text = "GIFT CODE";
            f1.label2.Text = "TRACE NUMBER";
            f1.label3.Text = "ORS HOST NUMBER";
            f1.comport = comport;
            f1.Show();
            this.Refresh();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            f1.command = "";
            this.Hide();
            f1.label7.Hide();
            f1.textBox5.Hide();
            f1.command = "C342";
            f1.label6.Text = "VOID HOT DEAL REDEMPTION";
            f1.label1.Text = "ORS HOST NUMBER";
            f1.label2.Text = "GIFT CODE";
            f1.label3.Text = "TRACE NUMBER";
            f1.comport = comport;
            f1.Show();
            this.Refresh();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            f1.command = "";
            this.Hide();
            f1.label7.Hide();
            f1.textBox5.Hide();
            f1.command = "C342";
            f1.label6.Text = "VOID VALUE REDEMPTION";
            f1.label1.Text = "AMOUNT (RM)";
            f1.label2.Text = "TRACE NUMBER";
            f1.label3.Text = "ORS HOST NUMBER";
            f1.comport = comport;
            f1.Show();
            this.Refresh();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            f1.command = "";
            this.Hide();
            f1.label7.Hide();
            f1.textBox5.Hide();
            f1.command = "C342";
            f1.label6.Text = "VOID POINT REDEMPTION";
            f1.label1.Text = "ENTER POINTS";
            f1.label2.Text = "TRACE NUMBER";
            f1.label3.Text = "ORS HOST NUMBER";
            f1.comport = comport;
            f1.Show();
            this.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Form1 f2 = new Form1();
            f1.command = "";
            f1.comport = comport;
            f2.Show();
            this.Refresh();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            lbl_portname.Text = comport;
        }
    }
}