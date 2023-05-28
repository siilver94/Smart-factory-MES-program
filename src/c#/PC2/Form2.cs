using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KB_Data
{
    public partial class Form2 : Form
    {
        Form1 frm1;

        public Form2()
        {
            InitializeComponent();
        }

        public Form2(Form1 _form)
        {
            InitializeComponent();
            frm1 = _form;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            string input = textBox1.Text;

            if (Form1.pass != null && Form1.pass != "")
            {
                if (Form1.pass == input)
                {
                    frm1.xtraTabControl1.SelectedTabPageIndex = 3;
                    this.Close();
                }
                else
                {
                    textBox1.Text = "";
                    MessageBox.Show("Password is wrong");
                }
            }

            else
            {
                if ("1111" == input)
                {
                    frm1.xtraTabControl1.SelectedTabPageIndex = 3;
                    this.Close();
                }
                else
                {
                    textBox1.Text = "";
                    MessageBox.Show("Password is wrong");
                }
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
