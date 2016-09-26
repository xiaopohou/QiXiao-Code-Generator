using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSharp_FlowchartToCode_DG
{
    public partial class Config : Form
    {
        public Config()
        {
            InitializeComponent();
        }
        string FilePath = @"qixiaoSrc\QixiaoConfig.ini";                                            //获取配置文件的路径
        string LoginType = "";                                    //数据库的登录方式 分为window登录和sqlserver登录两种

        private void Config_Load(object sender, EventArgs e)
        {
            textBox2.Text = Iniclass.GetString("Codes", "namespace_Model", "Model", FilePath);
            textBox8.Text = Iniclass.GetString("Codes", "ModelClassEmbellish", "", FilePath);
            textBox6.Text = Iniclass.GetString("Codes", "namespace_DAL", "DAL", FilePath);
            textBox9.Text = Iniclass.GetString("Codes", "DALClassEmbellish", "", FilePath);
            textBox3.Text = Iniclass.GetString("Codes", "namespace_BLL", "BLL", FilePath);
            textBox10.Text = Iniclass.GetString("Codes", "BLLClassEmbellish", "", FilePath);

            textBox1.Text = Iniclass.GetString("SQL", "ServerName", "", FilePath);
            textBox4.Text = Iniclass.GetString("SQL", "SqlAccount", "", FilePath);
            textBox5.Text = Iniclass.GetString("SQL", "SqlPwd", "", FilePath);

            LoginType = Iniclass.GetString("SQL", "LoginType", "", FilePath);//先获取登录方式 然后根据登录方式判断哪个应该被选中
            if (LoginType == "windows")
            {
                radioButton1.Checked=true;
            }
            else if(LoginType == "sqlserver")
            {
                radioButton2.Checked = true;
            }
            else
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                Iniclass.WriteString("config", "IsConfig", "True", FilePath);                           //如果不存在，先写配置文件
                //普通三层的配置文件
                Iniclass.WriteString("Codes", "namespace_Model", textBox2.Text, FilePath);
                Iniclass.WriteString("Codes", "ModelClassEmbellish", textBox8.Text, FilePath);
                Iniclass.WriteString("Codes", "namespace_DAL", textBox6.Text, FilePath);
                Iniclass.WriteString("Codes", "DALClassEmbellish", textBox9.Text, FilePath);
                Iniclass.WriteString("Codes", "namespace_BLL", textBox3.Text, FilePath);
                Iniclass.WriteString("Codes", "BLLClassEmbellish", textBox10.Text, FilePath);
                //登录部分的配置文件
                if (radioButton1.Checked)
                {
                    LoginType = "windows";
                }
                else if (radioButton2.Checked)
                {
                    LoginType = "sqlserver";
                }

                Iniclass.WriteString("SQL", "LoginType", LoginType, FilePath);      //将数据库登录方式写入配置文件
                Iniclass.WriteString("SQL", "ServerName", textBox1.Text, FilePath);      //windows登录 服务器名称
                Iniclass.WriteString("SQL", "SqlAccount", textBox4.Text, FilePath);             //sqlserver登录 账号
                Iniclass.WriteString("SQL", "SqlPwd", textBox5.Text, FilePath);                 //sqlserver登录 密码



                MessageBox.Show("配置成功！");
                //this.Owner.Show();  //显示主窗体
                this.Dispose();     //关闭子窗体

            }
            catch (Exception)
            {
                throw;
            }           
        }


        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                LoginType = "windows";
            }
            else if(radioButton2.Checked)
            {
                LoginType = "sqlserver";
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                LoginType = "windows";
            }
            else if (radioButton2.Checked)
            {
                LoginType = "sqlserver";
            }
        }

        private void Config_FormClosed(object sender, FormClosedEventArgs e)
        {
            //this.Owner.Show();  //显示主窗体
            this.Dispose();     //关闭子窗体
        }
    }
}
