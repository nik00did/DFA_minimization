using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DfaMinimization
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string fileName = "";
        DfaReader myDfa = new DfaReader();
        DfaReader newDfa = new DfaReader();

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }


    

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            textBox3.Clear();
            textBox4.Clear();
            richTextBox1.Clear();
            newDfa = new DfaReader();
            opf.InitialDirectory = Assembly.GetExecutingAssembly().Location;
            opf.RestoreDirectory = true;
            opf.Filter = "txt files (*.txt)|*.txt";
            if (opf.ShowDialog() == DialogResult.OK)
            {
                fileName = opf.FileName;
            }
            try
            {
                
                myDfa.FillDfa(fileName);
                newDfa = myDfa.MiniDfa();
                richTextBox1.Clear();
                textBox4.Text = "Input word with " + newDfa.Alphabet[0] + "," + newDfa.Alphabet[1];
                richTextBox1.Text = newDfa.printDfa();
            }
            catch
            {
                MessageBox.Show("Something went wrong", "Attention");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(newDfa.CheckWord(textBox3.Text).ToString(), "Result");
                textBox3.Clear();
            }
            catch
            {
                MessageBox.Show("Something went wrong", "Attention");
            }
        }
    }
}
