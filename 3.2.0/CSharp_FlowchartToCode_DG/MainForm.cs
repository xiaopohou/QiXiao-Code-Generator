using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;


namespace CSharp_FlowchartToCode_DG
{
    public partial class MainForm : Form
    {
        #region 全局变量
        //全局变量

        public List<object> CreateInfo = new List<object>();          //存储全部信息的List

        public List<string> FeildName = new List<string>();          //表字段名称
        public List<string> FeildType = new List<string>();          //表字段类型
        public List<string> FeildIsNullable = new List<string>();    //表字段可空
        public List<string> FeildLength = new List<string>();        //表字段长度
        public List<string> FeildMark = new List<string>();              //表字段说明
        public List<string> FeildIsPK = new List<string>();              //表字段是否主键
        public List<string> FeildIsIdentity = new List<string>();        //表字段是否自增


        string CodeTxt = "";//代码字符串，用于输出到文件
        string CodeTxtModel = "";   //Model层的代码
        string CodeTxtBLL = "";     //BLL层的代码
        string CodeTxtDAL = "";     //DAL层的代码
        string dir = "";            //获取路径

        string FilePath = @"qixiaoSrc\QixiaoConfig.ini";          //获取配置文件的路径

        string LoginType = "";                                    //数据库的登录方式 分为window登录和sqlserver登录两种

        #endregion

        public MainForm()
        {
            InitializeComponent();

        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                label_Author.Text += Info.Author;
                label_Version.Text += Info.VersionNum;


                string IsConfig = Iniclass.GetString("config", "IsConfig", "", FilePath);                   //检测是否存在配置文件，如果存在继续执行，如果不存在，先配置配置文件；

                if (!IsConfig.Equals("True"))
                {
                    MessageBox.Show("请先点击配置按钮来配置必要的参数！");
                    //跳转到配置Form来执行配置
                    Config con = new Config();
                    con.Owner = this;
                    //this.Hide();
                    con.ShowDialog();//打开配置窗进行配置文件的写入

                }

                textBox3.Text = Iniclass.GetString("Codes", "namespace_Model", "Model", FilePath);
                textBox8.Text = Iniclass.GetString("Codes", "ModelClassEmbellish", "", FilePath);
                textBox2.Text = Iniclass.GetString("Codes", "namespace_DAL", "DAL", FilePath);
                textBox9.Text = Iniclass.GetString("Codes", "DALClassEmbellish", "", FilePath);
                textBox6.Text = Iniclass.GetString("Codes", "namespace_BLL", "BLL", FilePath);
                textBox10.Text = Iniclass.GetString("Codes", "BLLClassEmbellish", "", FilePath);

                LoginType = Iniclass.GetString("SQL", "LoginType", "", FilePath);//先获取登录方式 然后根据登录方式判断哪个应该被选中
                if (LoginType == "windows")
                {
                    label1.Text = "(windows登录)Data Source=";
                    textBox1.Text = Iniclass.GetString("SQL", "ServerName", "", FilePath);
                }
                else if (LoginType == "sqlserver")
                {
                    label1.Text = "(Sql登录) Data Source=";
                    textBox1.Text = Iniclass.GetString("SQL", "ServerName", "", FilePath);
                }
                else
                {
                    label1.Text = "请先进行配置数据库登录";
                    textBox1.Text = "先进行配置--ERROR！！！";
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region 获取数据库结构的代码
        //获取数据库信息
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "select name from sys.databases where database_id > 4";//查询sqlserver中的非系统库
                string ConnectionStr = "";
                if (LoginType.Equals("windows"))
                {
                    ConnectionStr = "Data Source=" + textBox1.Text.Trim() + ";Initial Catalog=master;Integrated Security=True";
                }
                else if (LoginType.Equals("sqlserver"))
                {
                    //从配置文件获取到数据库登录的信息
                    string userId = Iniclass.GetString("SQL", "SqlAccount", "--", FilePath);
                    string Pwd = Iniclass.GetString("SQL", "SqlPwd", "--", FilePath);
                    //将数据库登录信息填充到链接字符串中以实现sqlserver方式登录
                    ConnectionStr = "Data Source=" + textBox1.Text.Trim() + ";Initial Catalog=master;User Id=" + userId + ";Password=" + Pwd + ";";
                }
                DataSet ds = SqlHelper.ExecuteDataSet(ConnectionStr, sql);
                DataTable dataTable = ds.Tables[0];

                TreeNode grand = new TreeNode(textBox1.Text.Trim());//添加节点服务器地址
                grand.ImageIndex = 1;
                treeView1.Nodes[0].Nodes.Add(grand);

                foreach (DataRow row in dataTable.Rows)
                {
                    TreeNode root = new TreeNode(row["name"].ToString());//创建节点
                    root.Name = row["name"].ToString();
                    root.ImageIndex = 2;
                    grand.Nodes.Add(root);
                    TreeNode biao = new TreeNode("表");
                    biao.Name = "表";
                    biao.ImageIndex = 3;
                    root.Nodes.Add(biao);


                    //获取表名
                    string sqltable = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
                    DataSet ds2 = SqlHelper.ExecuteDataSet("Data Source=" + textBox1.Text.Trim() + ";Initial Catalog=" + row["name"] + ";Integrated Security=True", sqltable);
                    DataTable dt2 = ds2.Tables[0];
                    foreach (DataRow row2 in dt2.Rows)
                    {
                        TreeNode biaovalue = new TreeNode(row2[0].ToString());
                        biaovalue.Name = row2[0].ToString();
                        biaovalue.ImageIndex = 4;
                        biao.Nodes.Add(biaovalue);
                    }
                }

            }
            catch (Exception)
            {
                //throw;
                if (LoginType.Equals("windows"))
                {
                    MessageBox.Show("windows身份登录 获取数据库信息失败，请重试！");
                }
                else if (LoginType.Equals("sqlserver"))
                {
                    MessageBox.Show("sqlserver身份登录 获取数据库信息失败，请重试！");
                }
                else
                {
                    MessageBox.Show("第一次运行程序请先配置 请检查登录方式及登录信息！");
                }

            }
        }
        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            getTableInfo();
        }
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            getTableInfo();
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            getTableInfo();
        }
        //获取数据库表信息
        public void getTableInfo()
        {
            try
            {
                string database = this.treeView1.SelectedNode.Parent.Parent.Name;
                string table = this.treeView1.SelectedNode.Name;

                textBox5.Text = table;//将table的表名赋值给TableName变量，方便后续传值; Model
                textBox4.Text = table;//将table的表名赋值给TableName变量，方便后续传值; DAL
                textBox7.Text = table;//将table的表名赋值给TableName变量，方便后续传值; BLL

                string connStr = "Data Source=" + textBox1.Text.Trim() + ";Initial Catalog=" + database + ";Integrated Security=True";

                string sql = @"select syscolumns.name as 字段名 ,systypes.name as 字段类型 , syscolumns.length as 长度,syscolumns.isnullable as 允许为空, sys.extended_properties.value as 说明  ,IsPK = Case  when exists ( select 1 from sysobjects  inner join sysindexes  on sysindexes.name = sysobjects.name  inner join sysindexkeys  on sysindexes.id = sysindexkeys.id  and  sysindexes.indid = sysindexkeys.indid  where xtype='PK'  and parent_obj = syscolumns.id and sysindexkeys.colid = syscolumns.colid ) then 1 else 0 end ,IsIdentity = Case syscolumns.status when 128 then 1 else 0 end  from syscolumns inner join systypes on(  syscolumns.xtype = systypes.xtype and systypes.name <>'_default_' and systypes.name<>'sysname'  ) left outer join sys.extended_properties on  ( sys.extended_properties.major_id=syscolumns.id and minor_id=syscolumns.colid  ) where syscolumns.id = (select id from sysobjects where name='" + table + @"') order by syscolumns.colid ";

                DataSet ds = SqlHelper.ExecuteDataSet(connStr, sql);

                DataTable dt = ds.Tables[0];
                this.dataGridView1.DataSource = dt.DefaultView;
                //设置初始值为全选中
                for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                {
                    this.dataGridView1.Rows[i].Cells[0].Value = true;
                }

            }
            catch (Exception)
            {
                //throw;
                //MessageBox.Show("无法检索到字段！");
            }
        }
        #endregion

        #region 操作栏按钮点击事件 全选和清空
        //全选按钮
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                int count = Convert.ToInt32(this.dataGridView1.Rows.Count.ToString());
                for (int i = 0; i < count; i++)
                {
                    //如果DataGridView是可编辑的，将数据提交，否则处于编辑状态的行无法取到 
                    this.dataGridView1.EndEdit();
                    DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)this.dataGridView1.Rows[i].Cells["选择"];
                    checkCell.Value = true;
                }
            }
            catch (Exception)
            {

            }
        }
        //清空按钮
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                int count = Convert.ToInt32(this.dataGridView1.Rows.Count.ToString());
                for (int i = 0; i < count; i++)
                {
                    //如果DataGridView是可编辑的，将数据提交，否则处于编辑状态的行无法取到 
                    this.dataGridView1.EndEdit();
                    DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)this.dataGridView1.Rows[i].Cells["选择"];
                    checkCell.Value = false;
                }
            }
            catch (Exception)
            {
            }
        }
        //初始化数据按钮
        private void button10_Click(object sender, EventArgs e)
        {
            textBox3.Text = Iniclass.GetString("Codes", "namespace_Model", "Model", FilePath);
            textBox8.Text = Iniclass.GetString("Codes", "ModelClassEmbellish", "", FilePath);
            textBox2.Text = Iniclass.GetString("Codes", "namespace_DAL", "DAL", FilePath);
            textBox9.Text = Iniclass.GetString("Codes", "DALClassEmbellish", "", FilePath);
            textBox6.Text = Iniclass.GetString("Codes", "namespace_BLL", "BLL", FilePath);
            textBox10.Text = Iniclass.GetString("Codes", "BLLClassEmbellish", "", FilePath);

            LoginType = Iniclass.GetString("SQL", "LoginType", "", FilePath);//先获取登录方式 然后根据登录方式判断哪个应该被选中
            if (LoginType == "windows")
            {
                label1.Text = "(windows登录)Data Source=";
                textBox1.Text = Iniclass.GetString("SQL", "ServerName", "", FilePath);
            }
            else if (LoginType == "sqlserver")
            {
                label1.Text = "(Sql登录) Data Source=";
                textBox1.Text = Iniclass.GetString("SQL", "ServerName", "", FilePath);
            }
            else
            {
                label1.Text = "请先进行配置数据库登录";
                textBox1.Text = "先进行配置--ERROR！！！";
            }

            textBox5.Text = "Class1";
            textBox4.Text = "Class1";
            textBox7.Text = "Class1";


            checkBox1.Checked = true;
            checkBox2.Checked = true;
            checkBox3.Checked = true;
            checkBox4.Checked = true;
            checkBox5.Checked = true;
            checkBox6.Checked = true;
            checkBox7.Checked = true;
            checkBox8.Checked = true;
            checkBox9.Checked = true;
            checkBox10.Checked = true;

        }
        #endregion


        //通用的设置信息的方法需要时候调用(从gridview中获取到数据)
        private void SetInfo()
        {
            //先将字段列表全部清空
            CreateInfo.Clear();

            FeildName.Clear();
            FeildType.Clear();
            FeildLength.Clear();
            FeildIsNullable.Clear();
            FeildMark.Clear();
            FeildIsPK.Clear();
            FeildIsIdentity.Clear();

            int count = Convert.ToInt32(this.dataGridView1.Rows.Count.ToString());
            for (int i = 0; i < count; i++)
            {
                //如果DataGridView是可编辑的，将数据提交，否则处于编辑状态的行无法取到 
                this.dataGridView1.EndEdit();
                DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)this.dataGridView1.Rows[i].Cells["选择"];
                Boolean flag = Convert.ToBoolean(checkCell.Value);
                if (flag == true)     //查找被选择的数据行 
                {
                    //从 DATAGRIDVIEW 中获取数据项 
                    string FName = this.dataGridView1.Rows[i].Cells[1].Value.ToString().Trim();
                    FeildName.Add(FName);
                    string FType = this.dataGridView1.Rows[i].Cells[2].Value.ToString().Trim();
                    FeildType.Add(FType);
                    string FLength = this.dataGridView1.Rows[i].Cells[3].Value.ToString().Trim();
                    FeildLength.Add(FLength);
                    string FIsNullable = this.dataGridView1.Rows[i].Cells[4].Value.ToString().Trim();
                    FeildIsNullable.Add(FIsNullable);
                    string FMark = this.dataGridView1.Rows[i].Cells[5].Value.ToString().Trim();
                    FeildMark.Add(FMark);
                    string FIsPK = this.dataGridView1.Rows[i].Cells[6].Value.ToString().Trim();
                    FeildIsPK.Add(FIsPK);
                    string FIsIdentity = this.dataGridView1.Rows[i].Cells[7].Value.ToString().Trim();
                    FeildIsIdentity.Add(FIsIdentity);
                }

            }
        }


        #region 生成三层的按钮点击事件
        //生成Model层代码
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ModelCreate();
                CodeTxt = CodeTxtModel;
                richTextBox1.Text = CodeTxt;    //获取model层的代码
                this.tabControl1.SelectedTab = tabPage2;//转换到代码页
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }

        }
        public void ModelCreate()
        {
            try
            {
                //将表的信息以及生成信息封装成List类型参数传递到生成代码的方法中
                SetInfo();//设置信息

                CreateInfo.Add(textBox3.Text.Trim());        //传递命名空间

                CreateInfo.Add(textBox5.Text.Trim());       //string类型的表名--后续的类名

                CreateInfo.Add(FeildName);
                CreateInfo.Add(FeildType);
                CreateInfo.Add(FeildLength);
                CreateInfo.Add(FeildIsNullable);
                CreateInfo.Add(FeildMark);
                CreateInfo.Add(FeildIsPK);
                CreateInfo.Add(FeildIsIdentity);

                CreateInfo.Add(textBox5.Text.Trim() + textBox8.Text.Trim());//添加类名修饰

                richTextBox1.Text = null;
                CodeTxtModel = ThreelayeToModel.CreateModelsCode(CreateInfo);

            }
            catch (Exception)
            {

            }
        }
        //生成DAL层代码
        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                DALCreate();
                CodeTxt = CodeTxtDAL;
                richTextBox1.Text = CodeTxt;   //获取DAL层的代码
                this.tabControl1.SelectedTab = tabPage2;//转换到代码页
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }
        public void DALCreate()
        {
            try
            {

                //将表的信息以及生成信息封装成List类型参数传递到生成代码的方法中
                SetInfo();//设置信息

                CreateInfo.Add(textBox2.Text.Trim());        //传递命名空间

                CreateInfo.Add(textBox4.Text.Trim());       //后续的类名

                CreateInfo.Add(FeildName);
                CreateInfo.Add(FeildType);
                CreateInfo.Add(FeildLength);
                CreateInfo.Add(FeildIsNullable);
                CreateInfo.Add(FeildMark);
                CreateInfo.Add(FeildIsPK);
                CreateInfo.Add(FeildIsIdentity);

                //方法的生成说明 依次代表 Add Delete Updata Select
                Boolean[] MethodInfo = new Boolean[] { checkBox1.Checked, checkBox2.Checked, checkBox3.Checked, checkBox4.Checked, checkBox9.Checked, checkBox24.Checked };
                CreateInfo.Add(MethodInfo);

                CreateInfo.Add(textBox4.Text.Trim() + textBox9.Text.Trim());//添加类名修饰

                richTextBox1.Text = null;

                CodeTxtDAL = ThreelayeToDAL.CreateDALCode(CreateInfo);
            }
            catch (Exception)
            {
            }
        }
        //生成BLL层代码
        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                BLLCreate();
                CodeTxt = CodeTxtBLL;
                richTextBox1.Text = CodeTxt;   //获取BLL层的代码
                this.tabControl1.SelectedTab = tabPage2;//转换到代码页
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }

        }
        public void BLLCreate()
        {
            try
            {
                //将表的信息以及生成信息封装成List类型参数传递到生成代码的方法中
                SetInfo();//设置信息

                CreateInfo.Add(textBox6.Text.Trim());        //传递命名空间

                CreateInfo.Add(textBox7.Text.Trim());       //string类型的表名--后续的类名

                CreateInfo.Add(FeildName);
                CreateInfo.Add(FeildType);
                CreateInfo.Add(FeildLength);
                CreateInfo.Add(FeildIsNullable);
                CreateInfo.Add(FeildMark);
                CreateInfo.Add(FeildIsPK);
                CreateInfo.Add(FeildIsIdentity);

                //方法的生成说明 依次代表 Add Delete Updata Select
                Boolean[] MethodInfo = new Boolean[] { checkBox5.Checked, checkBox6.Checked, checkBox7.Checked, checkBox8.Checked, checkBox10.Checked, checkBox12.Checked };
                CreateInfo.Add(MethodInfo);

                CreateInfo.Add(textBox7.Text.Trim() + textBox10.Text.Trim());//添加类名
                CreateInfo.Add(textBox4.Text.Trim() + textBox9.Text.Trim());//获取DAL 层类名修饰

                richTextBox1.Text = null;
                CodeTxtBLL = ThreelayeToBLL.CreateBLLCode(CreateInfo);
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region 页3的代码生成事件
        //生成controller代码
        private void button16_Click(object sender, EventArgs e)
        {
            try
            {
                //将表的信息以及生成信息封装成List类型参数传递到生成代码的方法中
                SetInfo();//设置信息

                CreateInfo.Add(textBox5.Text.Trim());       //string类型的表名--后续的类名

                CreateInfo.Add(FeildName);
                CreateInfo.Add(FeildType);
                CreateInfo.Add(FeildLength);
                CreateInfo.Add(FeildIsNullable);
                CreateInfo.Add(FeildMark);
                CreateInfo.Add(FeildIsPK);
                CreateInfo.Add(FeildIsIdentity);

                //方法的生成说明 依次代表 实例化对象 实例化自定义  jsonresult
                Boolean[] MethodInfo = new Boolean[] { checkBox11.Checked, checkBox15.Checked, checkBox13.Checked, checkBox19.Checked };
                CreateInfo.Add(MethodInfo);

                CreateInfo.Add(textBox7.Text.Trim() + textBox10.Text.Trim());//添加BLL类名

                richTextBox1.Text = null;
                CodeTxt = CodeToControllers.CreateControllersCode(CreateInfo);
                richTextBox1.Text = CodeTxt;   //获取BLL层的代码

                this.tabControl1.SelectedTab = tabPage2;//转换到代码页
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }
        //生成HTML代码
        private void button15_Click(object sender, EventArgs e)
        {
            try
            {
                //将表的信息以及生成信息封装成List类型参数传递到生成代码的方法中
                SetInfo();//设置信息

                CreateInfo.Add(textBox5.Text.Trim());       //string类型的表名--后续的类名

                CreateInfo.Add(FeildName);
                CreateInfo.Add(FeildType);
                CreateInfo.Add(FeildLength);
                CreateInfo.Add(FeildIsNullable);
                CreateInfo.Add(FeildMark);
                CreateInfo.Add(FeildIsPK);
                CreateInfo.Add(FeildIsIdentity);

                //方法的生成说明 依次代表 实例化对象 实例化自定义  jsonresult
                Boolean[] MethodInfo = new Boolean[] { checkBox14.Checked };
                CreateInfo.Add(MethodInfo);

                richTextBox1.Text = null;
                CodeTxt = CodeToHTML.CreateHTMLCode(CreateInfo);
                richTextBox1.Text = CodeTxt;   //获取BLL层的代码

                this.tabControl1.SelectedTab = tabPage2;//转换到代码页
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }
        //生成Javascript代码
        private void button7_Click_1(object sender, EventArgs e)
        {
            try
            {
                //将表的信息以及生成信息封装成List类型参数传递到生成代码的方法中
                SetInfo();//设置信息

                CreateInfo.Add(textBox5.Text.Trim());       //string类型的表名--后续的类名

                CreateInfo.Add(FeildName);
                CreateInfo.Add(FeildType);
                CreateInfo.Add(FeildLength);
                CreateInfo.Add(FeildIsNullable);
                CreateInfo.Add(FeildMark);
                CreateInfo.Add(FeildIsPK);
                CreateInfo.Add(FeildIsIdentity);

                //方法的生成说明 依次代表 实例化对象 实例化自定义  jsonresult
                Boolean[] MethodInfo = new Boolean[] { checkBox16.Checked, checkBox17.Checked, checkBox18.Checked, checkBox22.Checked, checkBox23.Checked, checkBox20.Checked };
                CreateInfo.Add(MethodInfo);

                richTextBox1.Text = null;
                CodeTxt = CodeToJavascript.CreateJavaScriptCode(CreateInfo);
                richTextBox1.Text = CodeTxt;   //获取BLL层的代码

                this.tabControl1.SelectedTab = tabPage2;//转换到代码页
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }
        #endregion

        #region 生成代码到桌面
        //生成到桌面
        private void button17_Click(object sender, EventArgs e)
        {
            ModelCreate();
            using (FileStream fs = new FileStream(dir + @"\" + textBox5.Text.Trim() + ".cs", FileMode.Create))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(CodeTxtModel);
                sw.Close();
            }
            MessageBox.Show("已输出到桌面 [" + textBox5.Text.Trim() + ".cs]");
        }

        private void button18_Click(object sender, EventArgs e)
        {
            DALCreate();
            using (FileStream fs = new FileStream(dir + @"\" + textBox4.Text.Trim() + textBox9.Text.Trim() + ".cs", FileMode.Create))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(CodeTxtDAL);
                sw.Close();
            }
            MessageBox.Show("已输出到桌面 [" + textBox4.Text.Trim() + textBox9.Text.Trim() + ".cs]");
        }

        private void button19_Click(object sender, EventArgs e)
        {
            BLLCreate();
            using (FileStream fs = new FileStream(dir + @"\" + textBox7.Text.Trim() + textBox10.Text.Trim() + ".cs", FileMode.Create))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(CodeTxtBLL);
                sw.Close();
            }
            MessageBox.Show("已输出到桌面 [" + textBox7.Text.Trim() + textBox10.Text.Trim() + ".cs]");
        }
        //将文本框文件保存到桌面
        private void button6_Click(object sender, EventArgs e)
        {
            using (FileStream fs = new FileStream(dir + @"\Temp.cs", FileMode.Create))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(CodeTxt);
                sw.Close();
            }
            MessageBox.Show("已输出到桌面[Temp.cs]");
        }
        //输出三层代码到桌面
        private void button11_Click(object sender, EventArgs e)
        {
            ModelCreate();
            BLLCreate();
            DALCreate();
            using (FileStream fs = new FileStream(dir + @"\" + textBox5.Text.Trim() + ".cs", FileMode.Create))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(CodeTxtModel);
                sw.Close();
            }
            using (FileStream fs = new FileStream(dir + @"\" + textBox4.Text.Trim() + textBox9.Text.Trim() + ".cs", FileMode.Create))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(CodeTxtDAL);
                sw.Close();
            }
            using (FileStream fs = new FileStream(dir + @"\" + textBox7.Text.Trim() + textBox10.Text.Trim() + ".cs", FileMode.Create))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(CodeTxtBLL);
                sw.Close();
            }
            MessageBox.Show("已输出到桌面[" + textBox5.Text.Trim() + ".cs] [" + textBox4.Text.Trim() + textBox9.Text.Trim() + ".cs] [" + textBox7.Text.Trim() + textBox10.Text.Trim() + ".cs]");
        }
        #endregion

        //返回代码生成设置页面 也就是首页
        private void button14_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = tabPage1;//转换到首页
        }

        //全选的按钮 的事件
        private void button12_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();//全选
        }
        //复制按钮的事件
        private void button13_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();//复制选择文本
        }
        //输出sqlhelper_DG到桌面
        private void button20_Click(object sender, EventArgs e)
        {
            try
            {
                System.IO.File.Copy(@"qixiaoSrc\SqlHelper_Framework4_5_DG.dll", dir + @"\SqlHelper_Framework4_5_DG.dll", true);//允许覆盖同名文件
                MessageBox.Show("复制成功，请查看桌面文件 SqlHelper_Framework4_5_DG.dll ！");
            }
            catch (Exception)
            {
                MessageBox.Show("复制文件失败，请检查文件是否在和本程序相同的路径下！");
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Config con = new Config();
            //con.Owner = this;
            //this.Hide();
            con.ShowDialog();//显示方式为模式窗体 不关闭不能操作其他窗体
        }


    }
}
