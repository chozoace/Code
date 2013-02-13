using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;



namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
       
        XmlTextWriter writer = new XmlTextWriter("PlayerSample.xml", Encoding.UTF8);
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            listBox1.Items.Add(textBox1.Text);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //XmlDocument xdoc= new XmlDocument();
           
            writer.Formatting = Formatting.Indented;

            
            writer.WriteStartElement("playerStats");
            


             writer.WriteAttributeString("health", textBox2.Text);
             writer.WriteAttributeString("height", textBox1.Text);
             writer.WriteAttributeString("width" , textBox3.Text);
             writer.WriteAttributeString("maxSpeed", textBox4.Text);
             writer.WriteAttributeString("accelRate", textBox5.Text);
             writer.WriteAttributeString("decelRate", textBox6.Text);
             writer.WriteAttributeString("jumpSpeedLimit", textBox7.Text);
             writer.WriteAttributeString("jumpSpeed", textBox8.Text);
             writer.WriteAttributeString("fallSpeed", textBox9.Text);
             writer.WriteAttributeString("gravity", textBox10.Text);
            
                    
            writer.WriteEndElement();//playerStats
          
            writer.Close();
           
             

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

     
        
    
    }
}
