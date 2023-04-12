
using Ken2.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KB_Monitor_V2
{
    public partial class Form1 : Form
    {
        TCPServer_K server1;
        TCPServer_K server2;

        Ken2.UIControl.dgvManager dgvmanager;
        private delegate void dele();//delegate

        public Form1()
        {
            InitializeComponent();
        }

        //fffffffffff
        private void Form1_Load(object sender, EventArgs e)
        {

#if Release
            string ip = "192.168.13.173";
 
#else
            //string ip = "192.168.56.1";
            string ip1 = "192.168.13.173";
            string ip2 = "192.168.13.174";

#endif
            server1 = new TCPServer_K(ip1, 5000);
            server1.TalkingComm += server_TalkingComm;

            

            dgvInit("dgvD0");

            StartmainThread(0);

        }


        #region ////////////////// mainThread //////////////////
        private Thread mainThread;
        bool mainThreadFlag = false;
        private void mainThreadMethod(object param)
        {
            int para = (int)param;
            while (mainThreadFlag)
            {
                try
                {
                    this.Invoke(new dele(() =>
                    {
                        label1.Text = Dtime.Now(Dtime.StringType.CurrentTime);
                        dgvD0.CurrentCell = null;
                    }));
                }
                catch (Exception exc)
                {
                }
                //Thread.Sleep(1000);
            }
        }
        public void StartmainThread(int param)
        {
            mainThreadFlag = true;
            mainThread = new Thread((new ParameterizedThreadStart(mainThreadMethod)));
            mainThread.Start(param);
        }
        public void StopmainThread(int None)
        {
            mainThreadFlag = false;
        }
        public void KillmainThread(int None)
        {
            mainThread.Abort();
        }
        #endregion ////////////////// mainThread //////////////////


        //evvvvvvvvvvvvvv
        private void server_TalkingComm(string name, object data, int length)
        {
            if (name.Equals("Data"))
            {

                byte[] bt = (byte[])data;
                string data_str = Encoding.ASCII.GetString(bt, 0, length);

                string[] comm_data = new string[12];

                try
                {
                    comm_data = data_str.Split('@')[0].Split('~');
                }
                catch (Exception)
                {

                }

                this.Invoke(new dele(() =>
                {
                    try
                    {
                        //모델
                        dgvD0.Rows[0].Cells[1].Value = comm_data[0];
                        dgvD0.Rows[0].Cells[2].Value = comm_data[1];
                        dgvD0.Rows[0].Cells[3].Value = comm_data[2];
                        //dgvD0.Rows[0].Cells[4].Value = comm_data[3];

                        //합계
                        //dgvD0.Rows[2].Cells[1].Value = comm_data[4];
                        //dgvD0.Rows[2].Cells[2].Value = comm_data[6];
                        //dgvD0.Rows[2].Cells[3].Value = comm_data[8];
                        ////dgvD0.Rows[2].Cells[4].Value = comm_data[10];
                        dgvD0.Rows[2].Cells[1].Value = comm_data[3];
                        dgvD0.Rows[2].Cells[2].Value = comm_data[5];
                        dgvD0.Rows[2].Cells[3].Value = comm_data[7];
                        //dgvD0.Rows[2].Cells[4].Value = comm_data[10];

                        //NG
                        //dgvD0.Rows[4].Cells[1].Value = comm_data[5];
                        //dgvD0.Rows[4].Cells[2].Value = comm_data[7];
                        //dgvD0.Rows[4].Cells[3].Value = comm_data[9];
                        ////dgvD0.Rows[4].Cells[4].Value = comm_data[11];
                        dgvD0.Rows[4].Cells[1].Value = comm_data[4];
                        dgvD0.Rows[4].Cells[2].Value = comm_data[6];
                        dgvD0.Rows[4].Cells[3].Value = comm_data[8];
                        //dgvD0.Rows[4].Cells[4].Value = comm_data[11];

                        //OK
                        //dgvD0.Rows[3].Cells[1].Value = int.Parse(comm_data[4]) - int.Parse(comm_data[5]);
                        //dgvD0.Rows[3].Cells[2].Value = int.Parse(comm_data[6]) - int.Parse(comm_data[7]);
                        //dgvD0.Rows[3].Cells[3].Value = int.Parse(comm_data[8]) - int.Parse(comm_data[9]);
                        ////dgvD0.Rows[3].Cells[4].Value = int.Parse(comm_data[10]) - int.Parse(comm_data[11]);
                        dgvD0.Rows[3].Cells[1].Value = int.Parse(comm_data[3]) - int.Parse(comm_data[4]);
                        dgvD0.Rows[3].Cells[2].Value = int.Parse(comm_data[5]) - int.Parse(comm_data[6]);
                        dgvD0.Rows[3].Cells[3].Value = int.Parse(comm_data[7]) - int.Parse(comm_data[8]);
                        //dgvD0.Rows[3].Cells[4].Value = int.Parse(comm_data[10]) - int.Parse(comm_data[11]);

                        //목표
                        //dgvD0.Rows[5].Cells[1].Value = comm_data[12];
                        //dgvD0.Rows[5].Cells[2].Value = comm_data[13];
                        //dgvD0.Rows[5].Cells[3].Value = comm_data[14];
                        ////dgvD0.Rows[5].Cells[4].Value = comm_data[15];
                        dgvD0.Rows[5].Cells[1].Value = comm_data[9];
                        dgvD0.Rows[5].Cells[2].Value = comm_data[10];
                        dgvD0.Rows[5].Cells[3].Value = comm_data[11];
                        //dgvD0.Rows[5].Cells[4].Value = comm_data[15];

                        //진도율
                        int[] var = new int[4];

                        //for (int i = 0; i < 4; i++)
                        //{
                        //    var[i] = (int)((double.Parse(comm_data[4 + (i * 2)]) / double.Parse(comm_data[12 + i])) * 100);
                        //    if (!(var[i] >= 0 && var[i] <= 1000))
                        //        var[i] = 0;

                        //    dgvD0.Rows[6].Cells[1 + i].Value = var[i] + "%";
                        //}

                        for (int i = 0; i < 3; i++)
                        {
                            var[i] = (int)((double.Parse(comm_data[3 + (i * 2)]) / double.Parse(comm_data[9 + i])) * 100);
                            if (!(var[i] >= 0 && var[i] <= 1000))
                                var[i] = 0;

                            dgvD0.Rows[6].Cells[1 + i].Value = var[i] + "%";
                        }


                    }
                    catch (Exception)
                    {

                    }

                }));
            }

        }


        public void dgvInit(string name)
        {
            switch (name)
            {

                case "dgvD0":

                    try
                    {
                        //---------------↓ 기본 ↓---------------┐
                        DataGridView dgv = (DataGridView)Reflection_K.Get(this, name);//이름가져옴
                        string DGV_name = dgv.Name;//적용
                        int height = int.Parse(DataRW.Load_Simple(DGV_name + "H", "30"));//데이터가져옴
                        int fontheader = int.Parse(DataRW.Load_Simple(DGV_name + "FH", "12"));//데이터가져옴
                        int fontcell = int.Parse(DataRW.Load_Simple(DGV_name + "FC", "12"));//데이터가져옴
                        GridMaster.FontSize2(dgv, fontheader, fontcell);//적용
                        //GridMaster.FontSize2( dgv , "New Gulim" , fontheader , fontcell );//한자나 글자 깨질 때 이걸로 사용하세요.
                        //---------------↑ 기본 ↑---------------┘

                        //---------------↓ 생성 ↓---------------┐
                        string[] ColumnsName = new string[] {
                            "A","A","A","A"//,"A"
                        };
                        int rows = 7;//초기 생성 Row수

                        GridMaster.Init3(dgv, true, height, rows, ColumnsName);
                        //---------------↑ 생성 ↑---------------┘

                        //---------------↓ 사용자 데이터 추가 부분 ↓---------------┐
                        //GridMaster.LoadCSV_OnlyData( dgv , System.Windows.Forms.Application.StartupPath + "\\AAAA.csv" );//셀데이터로드
                        //GridMaster.LoadCSV( dgvD0 , @"C:\Users\kclip3\Desktop\CR0.csv" );//셀데이터로드
                        dgv.Rows[0].Cells[0].Value = "모델";
                        dgv.Rows[1].Cells[0].Value = "공정";
                        dgv.Rows[2].Cells[0].Value = "합계";
                        dgv.Rows[3].Cells[0].Value = "OK";
                        dgv.Rows[4].Cells[0].Value = "NG";
                        dgv.Rows[5].Cells[0].Value = "목표수량";
                        dgv.Rows[6].Cells[0].Value = "진도율";

                        dgv.Rows[0].Cells[1].Value = "..";
                        dgv.Rows[0].Cells[2].Value = "..";
                        dgv.Rows[0].Cells[3].Value = "..";
                        //dgv.Rows[0].Cells[4].Value = "..";


                        dgv.Rows[1].Cells[1].Value = "U/Case Ass'y";
                        dgv.Rows[1].Cells[2].Value = "Balance";
                        dgv.Rows[1].Cells[3].Value = "성능검사";
                        //dgv.Rows[1].Cells[4].Value = "BRKT 체결";

                        for (int i = 0; i < 4; i++)
                        {
                            dgv.Rows[2 + i].Cells[1].Value = 0;
                            dgv.Rows[2 + i].Cells[2].Value = 0;
                            dgv.Rows[2 + i].Cells[3].Value = 0;
                            //dgv.Rows[2 + i].Cells[4].Value = 0;
                        }

                        dgv.Rows[6].Cells[1].Value = "0%";
                        dgv.Rows[6].Cells[2].Value = "0%";
                        dgv.Rows[6].Cells[3].Value = "0%";
                        //dgv.Rows[6].Cells[4].Value = "0%";

                        dgv.Rows[0].Cells[1].Style.ForeColor = Color.Yellow;
                        dgv.Rows[0].Cells[2].Style.ForeColor = Color.Yellow;
                        dgv.Rows[0].Cells[3].Style.ForeColor = Color.Yellow;
                        //dgv.Rows[0].Cells[4].Style.ForeColor = Color.Yellow;

                        dgv.Rows[2].Cells[1].Style.ForeColor = Color.Cyan;
                        dgv.Rows[2].Cells[2].Style.ForeColor = Color.Cyan;
                        dgv.Rows[2].Cells[3].Style.ForeColor = Color.Cyan;
                        //dgv.Rows[2].Cells[4].Style.ForeColor = Color.Cyan;

                        dgv.Rows[3].Cells[1].Style.ForeColor = Color.Lime;
                        dgv.Rows[3].Cells[2].Style.ForeColor = Color.Lime;
                        dgv.Rows[3].Cells[3].Style.ForeColor = Color.Lime;
                        //dgv.Rows[3].Cells[4].Style.ForeColor = Color.Lime;

                        dgv.Rows[4].Cells[1].Style.ForeColor = Color.Red;
                        dgv.Rows[4].Cells[2].Style.ForeColor = Color.Red;
                        dgv.Rows[4].Cells[3].Style.ForeColor = Color.Red;
                        //dgv.Rows[4].Cells[4].Style.ForeColor = Color.Red;

                        //---------------↑ 사용자 데이터 추가 부분 ↑---------------┘

                        //---------------↓ 정렬 ↓---------------┐
                        GridMaster.CenterAlign(dgv);
                        //GridMaster.LeftAlign( dgv );
                        //GridMaster.Align( dgv , 0 , DataGridViewContentAlignment.MiddleLeft );//단일 Column 정렬
                        //---------------↑ 정렬 ↑---------------┘

                        //---------------↓ 설정 ↓---------------┐
                        dgv.ReadOnly = true;//읽기전용
                        //dgv.Columns[ 0 ].ReadOnly = true;//읽기전용

                        GridMaster.DisableSortColumn(dgv);//오름차순 내림차순 정렬 막기

                        //dgv.AllowUserToResizeColumns = false;//컬럼폭 수정불가
                        dgv.ColumnHeadersVisible = false;//컬럼헤더 가리기                        
                        //dgv.Columns[ 1 ].DefaultCellStyle.Format = "yyyy-MM-dd 
                        dgv.DefaultCellStyle.BackColor = Color.Black;//색반전
                        dgv.DefaultCellStyle.ForeColor = Color.White;//색반전
                        //dgv.DefaultCellStyle.SelectionBackColor = Color.Transparent;
                        //dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
                        dgv.BackgroundColor = Color.Black;

                        //---------------↑ 설정 ↑---------------┘



                    }
                    catch (Exception)
                    {

                    }

                    break;


            }


        }
        

        //ccccccccccccc
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopmainThread(0);
            try
            {
                server1.Dispose();
            }
            catch (Exception)
            {

            }
        }

        void OnInit(string name, object data)
        {
            this.Invoke(new dele(() =>
            {
                dgvInit(name);
            }));
        }

        private void dgvD0_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString().Equals("Right"))
            {
                DataGridView thisdgv = (DataGridView)sender;
                dgvmanager = new Ken2.UIControl.dgvManager(thisdgv);
                dgvmanager.Init += OnInit;
                dgvmanager.Show();
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            try
            {
                Application.Exit();
            }
            catch (Exception)
            {

            }
        }

        private Point mousePoint; // 현재 마우스 포인터의 좌표저장 변수 선언

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            mousePoint = new Point(e.X, e.Y); //현재 마우스 좌표 저장
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left) //마우스 왼쪽 클릭 시에만 실행
            {
                //폼의 위치를 드래그중인 마우스의 좌표로 이동 
                Location = new Point(Left - (mousePoint.X - e.X), Top - (mousePoint.Y - e.Y));
            }
        }
    }
}
