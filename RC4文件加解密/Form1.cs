using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections;
using System.Diagnostics;

namespace RC4文件加解密
{
    public partial class Form1 : Form
    {

        delegate void AsynUpdateUI(int step);
        public int openAexitMark = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void GroupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "D:\\";
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = openFileDialog.FileName;
                this.textBox3.AppendText("[+] " + DateTime.Now.ToLocalTime().ToString() + " 选择了文件" + openFileDialog.FileName + "\r\n");
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            int checkresult;
            checkresult = check();
            if (checkresult == 1)
            {
                int flag;
                int mark = 0;
                flag = init();
                if(flag == 1 || flag == 3)
                {
                    mark = 1;
                }
                eadThread ead = new eadThread(this.textBox2.Text, this.textBox1.Text,mark);
                ead.UpdateUIDelegate += UpdataUIStatus;
                Thread thread1 = new Thread(new ThreadStart(ead.execEaD));
                thread1.Start();
            }
            else if(checkresult == 0)
            {
                MessageBox.Show("请选择待加密文件以及密钥！", "提示");
            }
            else if(checkresult == 2)
            {
                MessageBox.Show("请选择待加密文件！", "提示");
            }
            else
            {
                MessageBox.Show("请输入密钥！", "提示");
            }
        }

        public int init()
        {
            int flag = 0;
            this.progressBar1.Value = 0;
            this.label4.Text = this.progressBar1.Value.ToString() + "%";
            if(this.checkBox1.Checked == true && this.checkBox2.Checked == true)
            {
                flag = 3;
                this.openAexitMark = 1;
            }
            else if(this.checkBox1.Checked == true && this.checkBox2.Checked == false)
            {
                flag = 1;
            }
            else if(this.checkBox1.Checked == false && this.checkBox2.Checked == true)
            {
                flag = 2;
                this.openAexitMark = 1;
            }
            else
            {
                flag = 0;
            }
            return flag;
        }

        public int check()
        {
            int flag = 0;
            if(this.textBox1.Text != "" && this.textBox2.Text != "")
            {
                flag = 1;
            }
            else if(this.textBox1.Text == "" && this.textBox2.Text != "")
            {
                flag = 2;
            }
            else if(this.textBox1.Text != "" && this.textBox2.Text == "")
            {
                flag = 3;
            }
            else
            {
                flag = 0;
            }
            return flag;
        }

        private void UpdataUIStatus(int step)
        {
            if (InvokeRequired)
            {
                this.Invoke(new AsynUpdateUI(delegate (int s)
                {
                    this.progressBar1.Value = step;
                    this.label4.Text = this.progressBar1.Value.ToString() + "%";
                    if (step == 10)
                    {
                        this.textBox3.AppendText("[+] " + DateTime.Now.ToLocalTime().ToString() + " 进入子线程，开始执行加解密操作\r\n");
                    }
                    else if(step == 20)
                    {
                        this.textBox3.AppendText("[+] " + DateTime.Now.ToLocalTime().ToString() + " 密钥初始化完成\r\n");
                    }
                    else if(step == 40)
                    {
                        this.textBox3.AppendText("[+] " + DateTime.Now.ToLocalTime().ToString() + " 开始调用DLL导出函数\r\n");
                    }
                    else if(step == 90)
                    {
                        this.textBox3.AppendText("[+] " + DateTime.Now.ToLocalTime().ToString() + " 文件加解密操作完成\r\n");
                    }
                    else if(step == 100)
                    {
                        this.textBox3.AppendText("[+] " + DateTime.Now.ToLocalTime().ToString() + " 打开文件夹操作(退出程序操作)完成\r\n");
                        MessageBox.Show("文件加解密成功！", "提示");
                        if (this.openAexitMark == 1)
                        {
                            Process.GetCurrentProcess().Kill();
                        }
                    }
                }), step);
            }
            else
            {
                this.progressBar1.Value = step;
                this.label4.Text = this.progressBar1.Value.ToString() + "%";
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            int checkresult;
            checkresult = check();
            if (checkresult == 1)
            {
                int flag;
                int mark = 0;
                flag = init();
                if (flag == 1 || flag == 3)
                {
                    mark = 1;
                }
                eadThread ead = new eadThread(this.textBox2.Text, this.textBox1.Text, mark);
                ead.UpdateUIDelegate += UpdataUIStatus;
                Thread thread1 = new Thread(new ThreadStart(ead.execEaD));
                thread1.Start();
            }
            else if (checkresult == 0)
            {
                MessageBox.Show("请选择待加密文件以及密钥！", "提示");
            }
            else if (checkresult == 2)
            {
                MessageBox.Show("请选择待加密文件！", "提示");
            }
            else
            {
                MessageBox.Show("请输入密钥！", "提示");
            }
        }
    }


    class eadThread
    {
        public delegate void UpdateUI(int step);
        public UpdateUI UpdateUIDelegate;

        private string filename;
        private ArrayList arr = new ArrayList();
        private int flag;

        [DllImport("RC4.dll", EntryPoint = "rc4", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]

        public static extern int rc4(int[] key, string filename, int keylen);

        public eadThread(string KEY, string file, int mark)
        {
            char[] temp = KEY.ToCharArray();
            for (int i = 0; i < temp.Length; i++)
            {
                arr.Add((int)temp[i]);
            }
            this.filename = file;
            this.flag = mark;
        }

        public void execEaD()
        {
            UpdateUIDelegate(10);
            int[] key = new int[arr.Count];
            for (int i = 0; i < arr.Count; i++)
            {
                key[i] = (int)arr[i];
            }
            UpdateUIDelegate(20);
            int len = key.Length;
            UpdateUIDelegate(40);
            int result = rc4(key, this.filename, len);
            UpdateUIDelegate(90);
            if (this.flag == 1)
            {
                openFile(this.filename);
            }
            UpdateUIDelegate(100);
        }

        public void openFile(string filename)
        {
            Process p = new Process();
            p.StartInfo.FileName = "explorer.exe";
            p.StartInfo.Arguments = " /e,/select," + filename;
            p.Start();
        }

    }
}
