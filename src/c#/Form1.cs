using Ken2.Database;
using Ken2.DataManagement;
using Ken2.Util;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KB_Data_V2
{
    public partial class Form1 : Form
    {
        Ken2.UIControl.dgvManager dgvmanager;
        public delegate void dele();
        bool AdminMode = false;

        public static string pass = "";

        public TCPClient_PLC1 plc1;
        public TCPClient_PLC2 plc2;

        Mysql_K sql;
        TCPClient_LabelPrinter LabelPrinter;

        TCPClient_Monitor monitor_pc;

        TCPClient_HandyConverter handyconv;


        public int CurrentModelNum = -1;
        public int CurrentModelNum1 = -1;

        string LastSavedBarcode2 = "";//가장 최근 저장된 임펠러 바코드

        DataGridView[] dgvES = new DataGridView[2];

        //public string[] QuantityData = new string[8] { "0", "0", "0", "0", "0", "0", "0", "0" };
        public string[] QuantityData = new string[6] { "0", "0", "0", "0", "0", "0" };

        string Mainpath = "Log";

        //keyy
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Keys key = keyData & ~(Keys.Shift | Keys.Control);

            switch (key)
            {

                case Keys.Q:
                    if ((keyData & Keys.Control) != 0)
                    {
                        xtraTabControl1.ShowTabHeader = DevExpress.Utils.DefaultBoolean.True;
                        AdminMode = true;
                    }
                    break;
                case Keys.W:
                    if ((keyData & Keys.Control) != 0)
                    {
                        xtraTabControl1.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
                        AdminMode = false;

                    }
                    break;

            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #region 상단 버튼

        private void mini_kenb_Click(object sender, EventArgs e)    //  최소화 버튼
        {
            try
            {
                this.WindowState = FormWindowState.Minimized;
            }
            catch (Exception)
            {

            }
        }

        private void exit_kenb_Click(object sender, EventArgs e)    //  종료 버튼
        {
            try
            {
                Application.Exit();
            }
            catch (Exception)
            {

            }
        }

        private void simpleButton23_Click(object sender, EventArgs e)   //  메인화면 버튼
        {
            xtraTabControl1.SelectedTabPageIndex = 0;
        }

        private void simpleButton22_Click(object sender, EventArgs e)   //  모델 버튼
        {
            xtraTabControl1.SelectedTabPageIndex = 1;
        }

        private void simpleButton30_Click(object sender, EventArgs e)   //  이력조회 버튼
        {
            xtraTabControl1.SelectedTabPageIndex = 2;
            xtraTabControl1.TabPages[2].Controls.Add(timerange);

            Time0.Visible = true;
            Time1.Visible = true;
        }

        private void simpleButton19_Click(object sender, EventArgs e)   //  라벨설정1 버튼
        {
            Form2 frm = new Form2(this);
            frm.Show();
        }

        private void simpleButton05_Click(object sender, EventArgs e)   //  프로그램설정 버튼
        {
            xtraTabControl1.SelectedTabPageIndex = 9;
        }

        private void simpleButton6_Click(object sender, EventArgs e)   //  X_R Report 버튼
        {
            xtraTabControl1.SelectedTabPageIndex = 11;
            xtraTabControl1.TabPages[11].Controls.Add(timerange);

            Time0.Visible = false;
            Time1.Visible = false;

            SetToday();
        }
        #endregion

        public Form1()
        {
            InitializeComponent();

            dgvES[0] = dgvES0;
            dgvES[1] = dgvES1;
        }

        public void SaveTxt()
        {
            ControlData.Save(textBox10);    //  프린터1 비밀번호
        }
        public void LoadTxt()
        {
            ControlData.Load(textBox10);
        }


        #region DGV 관련
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
                        //---------------↑ 기본 ↑---------------┘

                        //---------------↓ 생성 ↓---------------┐
                        string[] ColumnsName = new string[] {
                            "","","","","","","",""
                        };
                        int rows = 28;//초기 생성 Row수
                        GridMaster.Init3(dgv, true, height, rows, ColumnsName);
                        //---------------↑ 생성 ↑---------------┘

                        //---------------↓ 사용자 데이터 추가 부분 ↓---------------┐
                        //GridMaster.LoadCSV_OnlyData( dgv , System.Windows.Forms.Application.StartupPath + "\\AAAA.csv" );//셀데이터로드

                        //---------------↓ 1번라인 ↓---------------┐

                        int l1 = 0;

                        dgv.Rows[0].Cells[l1].Value = "블로워 라벨";
                        dgv.Rows[1].Cells[l1].Value = "임펠러 바코드";
                        dgv.Rows[2].Cells[l1].Value = "어퍼 바코드";
                        dgv.Rows[3].Cells[l1].Value = "#C40 저항 검사";
                        dgv.Rows[4].Cells[l1].Value = "#30 UPPER CASE PCB 측정값 최대";
                        dgv.Rows[5].Cells[l1].Value = "#50 스페이서 측정값";
                        dgv.Rows[6].Cells[l1].Value = "#90 스페이서 측정값";
                        dgv.Rows[7].Cells[l1].Value = "#60 베어링압입 결과 거리";
                        dgv.Rows[8].Cells[l1].Value = "#60 베어링압입 결과 하중";
                        dgv.Rows[9].Cells[l1].Value = "#110 스토퍼 높이 측정값";

                        for (int i = 0; i < 10; i++)
                        {
                            dgv.Rows[i].Cells[l1].Style.BackColor = Color.Yellow;
                        }


                        //---------------↑ 1번라인 ↑---------------┘




                        //---------------↓ 2번라인 ↓---------------┐
                        int l2 = 2;

                        dgv.Rows[0].Cells[l2].Value = "블로워 라벨";
                        dgv.Rows[1].Cells[l2].Value = "밸런스 결과 판정";
                        dgv.Rows[2].Cells[l2].Value = "밸런스 1차 각도";
                        dgv.Rows[3].Cells[l2].Value = "밸런스 1차 밸런스량";
                        dgv.Rows[4].Cells[l2].Value = "밸런스 2차 각도";
                        dgv.Rows[5].Cells[l2].Value = "밸런스 2차 밸런스량";

                        dgv.Rows[6].Cells[l2].Value = "블로워 라벨";
                        dgv.Rows[7].Cells[l2].Value = "밸런스 결과 판정";
                        dgv.Rows[8].Cells[l2].Value = "밸런스 1차 각도";
                        dgv.Rows[9].Cells[l2].Value = "밸런스 1차 밸런스량";
                        dgv.Rows[10].Cells[l2].Value = "밸런스 2차 각도";
                        dgv.Rows[11].Cells[l2].Value = "밸런스 2차 밸런스량";

                        for (int i = 6; i < 12; i++)
                        {
                            dgv.Rows[i].Cells[l2].Style.BackColor = Color.Yellow;
                        }

                        dgv.Rows[12].Cells[l2].Value = "블로워 라벨";
                        dgv.Rows[13].Cells[l2].Value = "밸런스 결과 판정";
                        dgv.Rows[14].Cells[l2].Value = "밸런스 1차 각도";
                        dgv.Rows[15].Cells[l2].Value = "밸런스 1차 밸런스량";
                        dgv.Rows[16].Cells[l2].Value = "밸런스 2차 각도";
                        dgv.Rows[17].Cells[l2].Value = "밸런스 2차 밸런스량";




                        //---------------↑ 2번라인 ↑---------------┘




                        //---------------↓ 3번라인 ↓---------------┐

                        int l3 = 4;

                        dgv.Rows[0].Cells[l3].Value = "블로워 라벨";
                        dgv.Rows[1].Cells[l3].Value = "특성 검사 저항 판정";
                        dgv.Rows[2].Cells[l3].Value = "특성 저항 검사 측정값";
                        dgv.Rows[3].Cells[l3].Value = "특성 검사 RPM 판정";
                        dgv.Rows[4].Cells[l3].Value = "특성 검사 RPM 측정값";
                        dgv.Rows[5].Cells[l3].Value = "특성 검사 전류 판정";
                        dgv.Rows[6].Cells[l3].Value = "특성 검사 전류 측정값";

                        for (int i = 0; i < 7; i++)
                        {
                            dgv.Rows[i].Cells[l3].Style.BackColor = Color.Yellow;
                        }

                        dgv.Rows[7].Cells[l3].Value = "블로워 라벨";
                        dgv.Rows[8].Cells[l3].Value = "특성 검사 저항 판정";
                        dgv.Rows[9].Cells[l3].Value = "특성 저항 검사 측정값";
                        dgv.Rows[10].Cells[l3].Value = "특성 검사 RPM 판정";
                        dgv.Rows[11].Cells[l3].Value = "특성 검사 RPM 측정값";
                        dgv.Rows[12].Cells[l3].Value = "특성 검사 전류 판정";
                        dgv.Rows[13].Cells[l3].Value = "특성 검사 전류 측정값";

                        dgv.Rows[14].Cells[l3].Value = "블로워 라벨";
                        dgv.Rows[15].Cells[l3].Value = "특성 검사 저항 판정";
                        dgv.Rows[16].Cells[l3].Value = "특성 저항 검사 측정값";
                        dgv.Rows[17].Cells[l3].Value = "특성 검사 RPM 판정";
                        dgv.Rows[18].Cells[l3].Value = "특성 검사 RPM 측정값";
                        dgv.Rows[19].Cells[l3].Value = "특성 검사 전류 판정";
                        dgv.Rows[20].Cells[l3].Value = "특성 검사 전류 측정값";

                        for (int i = 14; i < 21; i++)
                        {
                            dgv.Rows[i].Cells[l3].Style.BackColor = Color.Yellow;
                        }

                        dgv.Rows[21].Cells[l3].Value = "블로워 라벨";
                        dgv.Rows[22].Cells[l3].Value = "특성 검사 저항 판정";
                        dgv.Rows[23].Cells[l3].Value = "특성 저항 검사 측정값";
                        dgv.Rows[24].Cells[l3].Value = "특성 검사 RPM 판정";
                        dgv.Rows[25].Cells[l3].Value = "특성 검사 RPM 측정값";
                        dgv.Rows[26].Cells[l3].Value = "특성 검사 전류 판정";
                        dgv.Rows[27].Cells[l3].Value = "특성 검사 전류 측정값";


                        //---------------↑ 3번라인 ↑---------------┘


                        //---------------↓ 4번라인 ↓---------------┐
                        int l4 = 6;

                        dgv.Rows[0].Cells[l4].Value = "블로워 라벨";
                        dgv.Rows[1].Cells[l4].Value = "성능 검사 판정";
                        dgv.Rows[2].Cells[l4].Value = "성능 검사 RPM 측정값";
                        dgv.Rows[3].Cells[l4].Value = "성능 검사 소음 측정값";
                        dgv.Rows[4].Cells[l4].Value = "성능 검사 진동 측정값";


                        dgv.Rows[5].Cells[l4].Value = "블로워 라벨";
                        dgv.Rows[6].Cells[l4].Value = "성능 검사 판정";
                        dgv.Rows[7].Cells[l4].Value = "성능 검사 RPM 측정값";
                        dgv.Rows[8].Cells[l4].Value = "성능 검사 소음 측정값";
                        dgv.Rows[9].Cells[l4].Value = "성능 검사 진동 측정값";

                        for (int i = 5; i < 10; i++)
                        {
                            dgv.Rows[i].Cells[l4].Style.BackColor = Color.Yellow;
                        }

                        //---------------↑ 4번라인 ↑---------------┘


                        //---------------↑ 사용자 데이터 추가 부분 ↑---------------┘

                        //---------------↓ 정렬 ↓---------------┐
                        //GridMaster.CenterAlign( dgv );
                        GridMaster.LeftAlign(dgv);
                        GridMaster.Align(dgv, 1, DataGridViewContentAlignment.MiddleLeft);//단일 Column 정렬
                        GridMaster.Align(dgv, 3, DataGridViewContentAlignment.MiddleLeft);//단일 Column 정렬
                        GridMaster.Align(dgv, 5, DataGridViewContentAlignment.MiddleLeft);//단일 Column 정렬
                        GridMaster.Align(dgv, 7, DataGridViewContentAlignment.MiddleLeft);//단일 Column 정렬


                        //---------------↑ 정렬 ↑---------------┘

                        //---------------↓ 설정 ↓---------------┐
                        dgv.ReadOnly = true;//읽기전용
                        GridMaster.DisableSortColumn(dgv);//오름차순 내림차순 정렬 막기
                        //dgv.Columns[ 0 ].ReadOnly = true;//읽기전용
                        //dgv.AllowUserToResizeColumns = false;//컬럼폭 수정불가

                        //dgv.Columns[ 1 ].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";//표시형식
                        dgv.ColumnHeadersVisible = false;//컬럼헤더 가리기                        
                        //dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//스페이스 시 줄바꿈
                        //dgv.DefaultCellStyle.BackColor = Color.Black;//색반전
                        //dgv.DefaultCellStyle.ForeColor = Color.White;//색반전

                        //---------------↑ 설정 ↑---------------┘


                    }
                    catch (Exception)
                    {

                    }

                    break;

                case "dgvD1":

                    try
                    {
                        //---------------↓ 기본 ↓---------------┐
                        DataGridView dgv = (DataGridView)Reflection_K.Get(this, name);//이름가져옴
                        string DGV_name = dgv.Name;//적용
                        int height = int.Parse(DataRW.Load_Simple(DGV_name + "H", "30"));//데이터가져옴
                        int fontheader = int.Parse(DataRW.Load_Simple(DGV_name + "FH", "12"));//데이터가져옴
                        int fontcell = int.Parse(DataRW.Load_Simple(DGV_name + "FC", "12"));//데이터가져옴
                        GridMaster.FontSize2(dgv, fontheader, fontcell);//적용
                        //---------------↑ 기본 ↑---------------┘

                        //---------------↓ 생성 ↓---------------┐
                        string[] ColumnsName = new string[] {
                            "A","B"
                        };
                        int rows = 0;//초기 생성 Row수
                        GridMaster.Init3(dgv, true, height, rows, ColumnsName);
                        //---------------↑ 생성 ↑---------------┘

                        //---------------↓ 사용자 데이터 추가 부분 ↓---------------┐
                        //GridMaster.LoadCSV_OnlyData( dgv , System.Windows.Forms.Application.StartupPath + "\\AAAA.csv" );//셀데이터로드
                        //dgv.Rows[ 0 ].Cells[ 0 ].Value = "CORE HEIGHT 1";


                        //---------------↑ 사용자 데이터 추가 부분 ↑---------------┘

                        //---------------↓ 정렬 ↓---------------┐
                        //GridMaster.CenterAlign( dgv );
                        GridMaster.LeftAlign(dgv);
                        //GridMaster.Align( dgv , 0 , DataGridViewContentAlignment.MiddleLeft );//단일 Column 정렬
                        //---------------↑ 정렬 ↑---------------┘

                        //---------------↓ 설정 ↓---------------┐
                        dgv.ReadOnly = true;//읽기전용
                        GridMaster.DisableSortColumn(dgv);//오름차순 내림차순 정렬 막기
                        //dgv.Columns[ 0 ].ReadOnly = true;//읽기전용
                        //dgv.AllowUserToResizeColumns = false;//컬럼폭 수정불가

                        //dgv.Columns[ 1 ].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";//표시형식
                        dgv.ColumnHeadersVisible = false;//컬럼헤더 가리기                        
                        //dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//스페이스 시 줄바꿈
                        //dgv.DefaultCellStyle.BackColor = Color.Black;//색반전
                        //dgv.DefaultCellStyle.ForeColor = Color.White;//색반전

                        //---------------↑ 설정 ↑---------------┘



                    }
                    catch (Exception)
                    {

                    }

                    break;

                case "dgvD2":

                    try
                    {
                        //---------------↓ 기본 ↓---------------┐
                        DataGridView dgv = (DataGridView)Reflection_K.Get(this, name);//이름가져옴
                        string DGV_name = dgv.Name;//적용
                        int height = int.Parse(DataRW.Load_Simple(DGV_name + "H", "30"));//데이터가져옴
                        int fontheader = int.Parse(DataRW.Load_Simple(DGV_name + "FH", "12"));//데이터가져옴
                        int fontcell = int.Parse(DataRW.Load_Simple(DGV_name + "FC", "12"));//데이터가져옴
                        GridMaster.FontSize2(dgv, fontheader, fontcell);//적용
                        //---------------↑ 기본 ↑---------------┘

                        //---------------↓ 생성 ↓---------------┐
                        string[] ColumnsName = new string[] {
                            "A"
                        };
                        int rows = 5;//초기 생성 Row수
                        GridMaster.Init3(dgv, true, height, rows, ColumnsName);
                        //---------------↑ 생성 ↑---------------┘

                        //---------------↓ 사용자 데이터 추가 부분 ↓---------------┐
                        //GridMaster.LoadCSV_OnlyData( dgv , System.Windows.Forms.Application.StartupPath + "\\AAAA.csv" );//셀데이터로드
                        //dgv.Rows[ 0 ].Cells[ 0 ].Value = "CORE HEIGHT 1";
                        dgv.Rows[0].Cells[0].Value = "PLC1 연결(192.168.13.10)";
                        dgv.Rows[1].Cells[0].Value = "PLC2 연결(192.168.13.110)";
                        //dgv.Rows[2].Cells[0].Value = "PLC3 연결(192.168.13.150)";
                        dgv.Rows[2].Cells[0].Value = "라벨 프린터1 연결(192.168.13.41)";
                        //dgv.Rows[4].Cells[0].Value = "라벨 프린터2 연결(192.168.13.172)";
                        dgv.Rows[3].Cells[0].Value = "라벨 설정1(어퍼케이스)";
                        //dgv.Rows[6].Cells[0].Value = "라벨 설정2(볼트체결기)";
                        //dgv.Rows[4].Cells[0].Value = "핸디 리더기(COM2)";
                        dgv.Rows[4].Cells[0].Value = "생산현황표시PC 연결(192.168.13.173)";


                        //---------------↑ 사용자 데이터 추가 부분 ↑---------------┘

                        //---------------↓ 정렬 ↓---------------┐
                        GridMaster.CenterAlign(dgv);
                        //GridMaster.LeftAlign( dgv );
                        GridMaster.Align(dgv, 0, DataGridViewContentAlignment.MiddleLeft);//단일 Column 정렬
                        //---------------↑ 정렬 ↑---------------┘

                        //---------------↓ 설정 ↓---------------┐
                        dgv.ReadOnly = true;//읽기전용
                        GridMaster.DisableSortColumn(dgv);//오름차순 내림차순 정렬 막기
                        //dgv.Columns[ 0 ].ReadOnly = true;//읽기전용
                        //dgv.AllowUserToResizeColumns = false;//컬럼폭 수정불가

                        //dgv.Columns[ 1 ].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";//표시형식
                        dgv.ColumnHeadersVisible = false;//컬럼헤더 가리기                        
                        //dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//스페이스 시 줄바꿈
                        //dgv.DefaultCellStyle.BackColor = Color.Black;//색반전
                        //dgv.DefaultCellStyle.ForeColor = Color.White;//색반전

                        //---------------↑ 설정 ↑---------------┘


                    }
                    catch (Exception)
                    {

                    }

                    break;

                case "dgvDE0":

                    try
                    {
                        //---------------↓ 기본 ↓---------------┐
                        DataGridView dgv = dgvDE0;
                        string DGV_name = dgv.Name;//적용
                        int height = int.Parse(DataRW.Load_Simple(DGV_name + "H", "30"));//데이터가져옴
                        int fontheader = int.Parse(DataRW.Load_Simple(DGV_name + "FH", "12"));//데이터가져옴
                        int fontcell = int.Parse(DataRW.Load_Simple(DGV_name + "FC", "12"));//데이터가져옴
                        GridMaster.FontSize2(dgv, fontheader, fontcell);//적용
                        //---------------↑ 기본 ↑---------------┘

                        //---------------↓ 생성 ↓---------------┐
                        string[] ColumnsName = new string[] {
                            "A","A","A","A"//,"A"
                        };
                        int rows = 2;//초기 생성 Row수

                        GridMaster.Init3(dgv, true, height, rows, ColumnsName);
                        //---------------↑ 생성 ↑---------------┘

                        //---------------↓ 사용자 데이터 추가 부분 ↓---------------┐
                        //GridMaster.LoadCSV_OnlyData( dgv , System.Windows.Forms.Application.StartupPath + "\\AAAA.csv" );//셀데이터로드
                        //GridMaster.LoadCSV( dgvD0 , @"C:\Users\kclip3\Desktop\CR0.csv" );//셀데이터로드
                        GridMaster.LoadCSV_OnlyData(dgv, System.Windows.Forms.Application.StartupPath + "\\quantity.csv");//셀데이터로드

                        dgv.Rows[0].Cells[0].Value = "공정";
                        dgv.Rows[1].Cells[0].Value = "목표수량";

                        dgv.Rows[0].Cells[1].Value = "U/Case Ass'y";
                        dgv.Rows[0].Cells[2].Value = "Balance";
                        dgv.Rows[0].Cells[3].Value = "성능검사";
                        //dgv.Rows[0].Cells[4].Value = "BKT 체결";


                        //---------------↑ 사용자 데이터 추가 부분 ↑---------------┘

                        //---------------↓ 정렬 ↓---------------┐
                        GridMaster.CenterAlign(dgv);
                        //GridMaster.LeftAlign( dgv );
                        //GridMaster.Align( dgv , 0 , DataGridViewContentAlignment.MiddleLeft );//단일 Column 정렬
                        //---------------↑ 정렬 ↑---------------┘

                        //---------------↓ 설정 ↓---------------┐
                        //dgv.ReadOnly = true;//읽기전용
                        //dgv.Columns[ 0 ].ReadOnly = true;//읽기전용

                        GridMaster.DisableSortColumn(dgv);//오름차순 내림차순 정렬 막기

                        //dgv.AllowUserToResizeColumns = false;//컬럼폭 수정불가
                        dgv.ColumnHeadersVisible = false;//컬럼헤더 가리기                        
                        //dgv.Columns[ 1 ].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";//표시형식

                        //dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//스페이스 시 줄바꿈
                        //dgv.DefaultCellStyle.BackColor = Color.Black;//색반전
                        //dgv.DefaultCellStyle.ForeColor = Color.White;//색반전
                        //dgv.DefaultCellStyle.SelectionBackColor = Color.Transparent;
                        //dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
                        //dgv.BackgroundColor = Color.Black;

                        //---------------↑ 설정 ↑---------------┘



                    }
                    catch (Exception)
                    {

                    }

                    break;

                case "dgvXR0":

                    try
                    {
                        //---------------↓ 기본 ↓---------------┐
                        DataGridView dgv = (DataGridView)Reflection_K.Get(this, name);//이름가져옴
                        string DGV_name = dgv.Name;//적용
                        int height = int.Parse(DataRW.Load_Simple(DGV_name + "H", "30"));//데이터가져옴
                        int fontheader = int.Parse(DataRW.Load_Simple(DGV_name + "FH", "12"));//데이터가져옴
                        int fontcell = int.Parse(DataRW.Load_Simple(DGV_name + "FC", "12"));//데이터가져옴
                        GridMaster.FontSize2(dgv, fontheader, fontcell);//적용
                        //---------------↑ 기본 ↑---------------┘

                        //---------------↓ 생성 ↓---------------┐
                        string[] ColumnsName = new string[] {
                            "A" , "B"
                        };
                        int rows = 16;//초기 생성 Row수
                        GridMaster.Init3(dgv, true, height, rows, ColumnsName);
                        //---------------↑ 생성 ↑---------------┘

                        //---------------↓ 사용자 데이터 추가 부분 ↓---------------┐
                        //GridMaster.LoadCSV_OnlyData( dgv , System.Windows.Forms.Application.StartupPath + "\\AAAA.csv" );//셀데이터로드
                        //dgv.Rows[ 0 ].Cells[ 0 ].Value = "CORE HEIGHT 1";
                        dgv.Rows[0].Cells[0].Value = "#30 UPPER CASE 공급부 PCB 측정값 최대";
                        dgv.Rows[1].Cells[0].Value = "#50 스페이서 측정값";
                        dgv.Rows[2].Cells[0].Value = "#90 스페이서 측정값";
                        dgv.Rows[3].Cells[0].Value = "#60 베어링압입 결과 거리";
                        dgv.Rows[4].Cells[0].Value = "#60 베어링압입 결과 하중";
                        dgv.Rows[5].Cells[0].Value = "#110 스토퍼 높이 측정값";

                        dgv.Rows[6].Cells[0].Value = "밸런스 1차 각도";
                        dgv.Rows[7].Cells[0].Value = "밸런스 1차 밸런스량";
                        dgv.Rows[8].Cells[0].Value = "밸런스 2차 각도";
                        dgv.Rows[9].Cells[0].Value = "밸런스 2차 밸런스량";

                        dgv.Rows[10].Cells[0].Value = "특성 저항 검사 측정값";
                        dgv.Rows[11].Cells[0].Value = "특성 검사 RPM 측정값";
                        dgv.Rows[12].Cells[0].Value = "특성 검사 전류 측정값";

                        dgv.Rows[13].Cells[0].Value = "성능 검사 RPM 측정값";
                        dgv.Rows[14].Cells[0].Value = "성능 검사 소음 측정값";
                        dgv.Rows[15].Cells[0].Value = "성능 검사 진동 측정값";

                        ///////////////##################################################################

                        dgv.Rows[0].Cells[1].Value = "c14";
                        dgv.Rows[1].Cells[1].Value = "c15";
                        dgv.Rows[2].Cells[1].Value = "c16";
                        dgv.Rows[3].Cells[1].Value = "c17";
                        dgv.Rows[4].Cells[1].Value = "c18";
                        dgv.Rows[5].Cells[1].Value = "c180";

                        dgv.Rows[6].Cells[1].Value = "c20";
                        dgv.Rows[7].Cells[1].Value = "c21";
                        dgv.Rows[8].Cells[1].Value = "c22";
                        dgv.Rows[9].Cells[1].Value = "c23";

                        dgv.Rows[10].Cells[1].Value = "c25";
                        dgv.Rows[11].Cells[1].Value = "c27";
                        dgv.Rows[12].Cells[1].Value = "c29";

                        dgv.Rows[13].Cells[1].Value = "c31";
                        dgv.Rows[14].Cells[1].Value = "c32";
                        dgv.Rows[15].Cells[1].Value = "c33";


                        //---------------↑ 사용자 데이터 추가 부분 ↑---------------┘

                        //---------------↓ 정렬 ↓---------------┐
                        GridMaster.CenterAlign(dgv);
                        //GridMaster.LeftAlign( dgv );
                        GridMaster.Align(dgv, 0, DataGridViewContentAlignment.MiddleLeft);//단일 Column 정렬
                        //---------------↑ 정렬 ↑---------------┘

                        //---------------↓ 설정 ↓---------------┐
                        dgv.ReadOnly = true;//읽기전용
                        GridMaster.DisableSortColumn(dgv);//오름차순 내림차순 정렬 막기
                        //dgv.Columns[ 0 ].ReadOnly = true;//읽기전용
                        //dgv.AllowUserToResizeColumns = false;//컬럼폭 수정불가

                        //dgv.Columns[ 1 ].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";//표시형식
                        dgv.ColumnHeadersVisible = false;//컬럼헤더 가리기                        
                        //dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//스페이스 시 줄바꿈
                        //dgv.DefaultCellStyle.BackColor = Color.Black;//색반전
                        //dgv.DefaultCellStyle.ForeColor = Color.White;//색반전

                        dgv.Columns[1].Visible = false;//1번컬럼 숨기기

                        //---------------↑ 설정 ↑---------------┘



                    }
                    catch (Exception)
                    {

                    }

                    break;


                case "dgvP0":

                    try
                    {
                        //---------------↓ 기본 ↓---------------┐
                        DataGridView dgv = (DataGridView)Reflection_K.Get(this, name);//이름가져옴
                        string DGV_name = dgv.Name;//적용
                        int height = int.Parse(DataRW.Load_Simple(DGV_name + "H", "30"));//데이터가져옴
                        int fontheader = int.Parse(DataRW.Load_Simple(DGV_name + "FH", "12"));//데이터가져옴
                        int fontcell = int.Parse(DataRW.Load_Simple(DGV_name + "FC", "12"));//데이터가져옴
                        GridMaster.FontSize2(dgv, fontheader, fontcell);//적용
                        //---------------↑ 기본 ↑---------------┘

                        //---------------↓ 생성 ↓---------------┐
                        string[] ColumnsName = new string[] {
                             "내용" , "데이터"
                        };
                        int rows = 11;//초기 생성 Row수

                        GridMaster.Init3(dgv, true, height, rows, ColumnsName);
                        //---------------↑ 생성 ↑---------------┘

                        //---------------↓ 사용자 데이터 추가 부분 ↓---------------┐
                        //GridMaster.LoadCSV_OnlyData( dgv , System.Windows.Forms.Application.StartupPath + "\\AAAA.csv" );//셀데이터로드
                        //dgv.Rows[ 0 ].Cells[ 0 ].Value = "CORE HEIGHT 1";

                        dgv.Rows[0].Cells[0].Value = "주야정보";
                        dgv.Rows[1].Cells[0].Value = "카운트정보";
                        dgv.Rows[2].Cells[0].Value = "카운트 숫자데이터";
                        dgv.Rows[3].Cells[0].Value = "프로젝트이름";
                        dgv.Rows[4].Cells[0].Value = "라인정보";

                        dgv.Rows[5].Cells[0].Value = "캡션1";
                        dgv.Rows[6].Cells[0].Value = "캡션2";
                        dgv.Rows[7].Cells[0].Value = "캡션3";
                        dgv.Rows[8].Cells[0].Value = "캡션4";

                        dgv.Rows[9].Cells[0].Value = "날짜";
                        dgv.Rows[10].Cells[0].Value = "고객코드";


                        //---------------↑ 사용자 데이터 추가 부분 ↑---------------┘

                        //---------------↓ 정렬 ↓---------------┐
                        GridMaster.CenterAlign(dgv);
                        //GridMaster.LeftAlign( dgv );
                        //GridMaster.Align( dgv , 0 , DataGridViewContentAlignment.MiddleLeft );//단일 Column 정렬
                        //---------------↑ 정렬 ↑---------------┘

                        //---------------↓ 설정 ↓---------------┐
                        //dgv.ReadOnly = true;//읽기전용
                        GridMaster.DisableSortColumn(dgv);//오름차순 내림차순 정렬 막기
                        //dgv.Columns[ 0 ].ReadOnly = true;//읽기전용
                        //dgv.AllowUserToResizeColumns = false;//컬럼폭 수정불가

                        //dgv.Columns[ 1 ].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";//표시형식
                        //dgv.ColumnHeadersVisible = false;//컬럼헤더 가리기                        
                        //dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//스페이스 시 줄바꿈
                        //dgv.DefaultCellStyle.BackColor = Color.Black;//색반전
                        //dgv.DefaultCellStyle.ForeColor = Color.White;//색반전

                        //---------------↑ 설정 ↑---------------┘



                    }
                    catch (Exception)
                    {

                    }

                    break;

                case "dgvC0":

                    try
                    {
                        //---------------↓ 기본 ↓---------------┐
                        DataGridView dgv = (DataGridView)Reflection_K.Get(this, name);//이름가져옴
                        string DGV_name = dgv.Name;//적용
                        int height = int.Parse(DataRW.Load_Simple(DGV_name + "H", "30"));//데이터가져옴
                        int fontheader = int.Parse(DataRW.Load_Simple(DGV_name + "FH", "12"));//데이터가져옴
                        int fontcell = int.Parse(DataRW.Load_Simple(DGV_name + "FC", "12"));//데이터가져옴
                        GridMaster.FontSize2(dgv, fontheader, fontcell);//적용
                        //---------------↑ 기본 ↑---------------┘

                        //---------------↓ 생성 ↓---------------┐
                        string[] ColumnsName = new string[] {
                            "번지" , "내용" , "데이터"
                        };
                        int rows = 1500;//초기 생성 Row수

                        GridMaster.Init3(dgv, true, height, rows, ColumnsName);
                        //---------------↑ 생성 ↑---------------┘

                        //---------------↓ 사용자 데이터 추가 부분 ↓---------------┐
                        //GridMaster.LoadCSV_OnlyData( dgv , System.Windows.Forms.Application.StartupPath + "\\AAAA.csv" );//셀데이터로드
                        //dgv.Rows[ 0 ].Cells[ 0 ].Value = "CORE HEIGHT 1";

                        for (int i = 0; i < rows; i++)
                        {
                            dgv.Rows[i].Cells[0].Value = "D" + (i + 5000);
                        }

                        //---------------↑ 사용자 데이터 추가 부분 ↑---------------┘

                        //---------------↓ 정렬 ↓---------------┐
                        GridMaster.CenterAlign(dgv);
                        //GridMaster.LeftAlign( dgv );
                        //GridMaster.Align( dgv , 0 , DataGridViewContentAlignment.MiddleLeft );//단일 Column 정렬
                        //---------------↑ 정렬 ↑---------------┘

                        //---------------↓ 설정 ↓---------------┐
                        dgv.ReadOnly = true;//읽기전용
                        GridMaster.DisableSortColumn(dgv);//오름차순 내림차순 정렬 막기
                        //dgv.Columns[ 0 ].ReadOnly = true;//읽기전용
                        //dgv.AllowUserToResizeColumns = false;//컬럼폭 수정불가

                        //dgv.Columns[ 1 ].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";//표시형식
                        //dgv.ColumnHeadersVisible = false;//컬럼헤더 가리기                        
                        //dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//스페이스 시 줄바꿈
                        //dgv.DefaultCellStyle.BackColor = Color.Black;//색반전
                        //dgv.DefaultCellStyle.ForeColor = Color.White;//색반전

                        //---------------↑ 설정 ↑---------------┘



                    }
                    catch (Exception)
                    {

                    }

                    break;


                case "dgvC1":

                    try
                    {
                        //---------------↓ 기본 ↓---------------┐
                        DataGridView dgv = (DataGridView)Reflection_K.Get(this, name);//이름가져옴
                        string DGV_name = dgv.Name;//적용
                        int height = int.Parse(DataRW.Load_Simple(DGV_name + "H", "30"));//데이터가져옴
                        int fontheader = int.Parse(DataRW.Load_Simple(DGV_name + "FH", "12"));//데이터가져옴
                        int fontcell = int.Parse(DataRW.Load_Simple(DGV_name + "FC", "12"));//데이터가져옴
                        GridMaster.FontSize2(dgv, fontheader, fontcell);//적용
                        //---------------↑ 기본 ↑---------------┘

                        //---------------↓ 생성 ↓---------------┐
                        string[] ColumnsName = new string[] {
                            "번지" , "내용" , "데이터"
                        };
                        int rows = 1500;//초기 생성 Row수

                        GridMaster.Init3(dgv, true, height, rows, ColumnsName);
                        //---------------↑ 생성 ↑---------------┘

                        //---------------↓ 사용자 데이터 추가 부분 ↓---------------┐
                        //GridMaster.LoadCSV_OnlyData( dgv , System.Windows.Forms.Application.StartupPath + "\\AAAA.csv" );//셀데이터로드
                        //dgv.Rows[ 0 ].Cells[ 0 ].Value = "CORE HEIGHT 1";

                        for (int i = 0; i < rows; i++)
                        {
                            dgv.Rows[i].Cells[0].Value = "D" + (i + 7000);
                        }

                        //---------------↑ 사용자 데이터 추가 부분 ↑---------------┘

                        //---------------↓ 정렬 ↓---------------┐
                        GridMaster.CenterAlign(dgv);
                        //GridMaster.LeftAlign( dgv );
                        //GridMaster.Align( dgv , 0 , DataGridViewContentAlignment.MiddleLeft );//단일 Column 정렬
                        //---------------↑ 정렬 ↑---------------┘

                        //---------------↓ 설정 ↓---------------┐
                        dgv.ReadOnly = true;//읽기전용
                        GridMaster.DisableSortColumn(dgv);//오름차순 내림차순 정렬 막기
                        //dgv.Columns[ 0 ].ReadOnly = true;//읽기전용
                        //dgv.AllowUserToResizeColumns = false;//컬럼폭 수정불가

                        //dgv.Columns[ 1 ].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";//표시형식
                        //dgv.ColumnHeadersVisible = false;//컬럼헤더 가리기                        
                        //dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//스페이스 시 줄바꿈
                        //dgv.DefaultCellStyle.BackColor = Color.Black;//색반전
                        //dgv.DefaultCellStyle.ForeColor = Color.White;//색반전

                        //---------------↑ 설정 ↑---------------┘



                    }
                    catch (Exception)
                    {

                    }

                    break;


                case "dgvH0":

                    try
                    {
                        //---------------↓ 기본 ↓---------------┐
                        DataGridView dgv = (DataGridView)Reflection_K.Get(this, name);//이름가져옴
                        string DGV_name = dgv.Name;//적용
                        int height = int.Parse(DataRW.Load_Simple(DGV_name + "H", "30"));//데이터가져옴
                        int fontheader = int.Parse(DataRW.Load_Simple(DGV_name + "FH", "12"));//데이터가져옴
                        int fontcell = int.Parse(DataRW.Load_Simple(DGV_name + "FC", "12"));//데이터가져옴
                        GridMaster.FontSize2(dgv, fontheader, fontcell);//적용
                        //---------------↑ 기본 ↑---------------┘

                        //---------------↓ 생성 ↓---------------┐
                        string[] ColumnsName = new string[] {

                        };
                        int rows = 0;//초기 생성 Row수

                        GridMaster.Init3(dgv, false, height, rows, ColumnsName);
                        //---------------↑ 생성 ↑---------------┘

                        //---------------↓ 사용자 데이터 추가 부분 ↓---------------┐
                        //GridMaster.LoadCSV_OnlyData( dgv , System.Windows.Forms.Application.StartupPath + "\\AAAA.csv" );//셀데이터로드
                        //dgv.Rows[ 0 ].Cells[ 0 ].Value = "CORE HEIGHT 1";

                        dgv.Columns[4].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";


                        dgv.Columns[0].HeaderText = "블로워 라벨";
                        dgv.Columns[1].HeaderText = "임펠러 바코드";
                        dgv.Columns[2].HeaderText = "어퍼 바코드";
                        dgv.Columns[3].HeaderText = "날짜";
                        dgv.Columns[4].HeaderText = "모델";

                        dgv.Columns[5].HeaderText = "#C40 저항 검사 이상 완료";

                        dgv.Columns[6].HeaderText = "#30 UPPER CASE 공급부 PCB 측정값 최대";
                        dgv.Columns[7].HeaderText = "#50 스페이서 측정값";
                        dgv.Columns[8].HeaderText = "#90 스페이서 측정값";
                        dgv.Columns[9].HeaderText = "#60 베어링압입 결과 거리";
                        dgv.Columns[10].HeaderText = "#60 베어링압입 결과 하중";
                        dgv.Columns[11].HeaderText = "#110 스토퍼 높이 측정값";

                        dgv.Columns[12].HeaderText = "밸런스 결과 판정";
                        dgv.Columns[13].HeaderText = "밸런스 1차 각도";
                        dgv.Columns[14].HeaderText = "밸런스 1차 밸런스량";
                        dgv.Columns[15].HeaderText = "밸런스 2차 각도";
                        dgv.Columns[16].HeaderText = "밸런스 2차 밸런스량";
                        dgv.Columns[17].HeaderText = "특성 검사 저항 판정";
                        dgv.Columns[18].HeaderText = "특성 저항 검사 측정값";
                        dgv.Columns[19].HeaderText = "특성 검사 RPM 판정";
                        dgv.Columns[20].HeaderText = "특성 검사 RPM 측정값";
                        dgv.Columns[21].HeaderText = "특성 검사 전류 판정";
                        dgv.Columns[22].HeaderText = "특성 검사 전류 측정값";
                        dgv.Columns[23].HeaderText = "성능 검사 판정";
                        dgv.Columns[24].HeaderText = "성능 검사 RPM 측정값";
                        dgv.Columns[25].HeaderText = "성능 검사 소음 측정값";
                        dgv.Columns[26].HeaderText = "성능 검사 진동 측정값";
                        dgv.Columns[27].HeaderText = "최종판정";


                        //---------------↓ OKNG 색칠 ↓---------------┐

                        GridMaster.Color_Painting(dgv, 5);
                        GridMaster.Color_Painting(dgv, 12);

                        GridMaster.Color_Painting(dgv, 17);
                        GridMaster.Color_Painting(dgv, 19);
                        GridMaster.Color_Painting(dgv, 21);
                        GridMaster.Color_Painting(dgv, 23);
                        GridMaster.Color_Painting(dgv, 27);

                        //---------------↑ OKNG 색칠 ↑---------------┘



                        //---------------↑ 사용자 데이터 추가 부분 ↑---------------┘

                        //---------------↓ 정렬 ↓---------------┐
                        GridMaster.CenterAlign(dgv);
                        //GridMaster.LeftAlign( dgv );
                        //GridMaster.Align( dgv , 0 , DataGridViewContentAlignment.MiddleLeft );//단일 Column 정렬
                        //---------------↑ 정렬 ↑---------------┘

                        //---------------↓ 설정 ↓---------------┐
                        dgv.ReadOnly = true;//읽기전용
                        //GridMaster.DisableSortColumn( dgv );//오름차순 내림차순 정렬 막기
                        //dgv.Columns[ 0 ].ReadOnly = true;//읽기전용
                        //dgv.AllowUserToResizeColumns = false;//컬럼폭 수정불가

                        //dgv.Columns[ 1 ].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";//표시형식
                        //dgv.ColumnHeadersVisible = false;//컬럼헤더 가리기                        
                        //dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//스페이스 시 줄바꿈
                        //dgv.DefaultCellStyle.BackColor = Color.Black;//색반전
                        //dgv.DefaultCellStyle.ForeColor = Color.White;//색반전

                        //---------------↑ 설정 ↑---------------┘

                    }
                    catch (Exception)
                    {

                    }

                    break;


                case "dgvHN0":

                    try
                    {
                        //---------------↓ 기본 ↓---------------┐
                        DataGridView dgv = (DataGridView)Reflection_K.Get(this, name);//이름가져옴
                        string DGV_name = dgv.Name;//적용
                        int height = int.Parse(DataRW.Load_Simple(DGV_name + "H", "30"));//데이터가져옴
                        int fontheader = int.Parse(DataRW.Load_Simple(DGV_name + "FH", "12"));//데이터가져옴
                        int fontcell = int.Parse(DataRW.Load_Simple(DGV_name + "FC", "12"));//데이터가져옴
                        GridMaster.FontSize2(dgv, fontheader, fontcell);//적용
                        //---------------↑ 기본 ↑---------------┘

                        //---------------↓ 생성 ↓---------------┐
                        string[] ColumnsName = new string[] {
                             "TOTAL" , "OK" , "NG" , "PER"
                        };
                        int rows = 1;//초기 생성 Row수

                        GridMaster.Init3(dgv, true, height, rows, ColumnsName);
                        //---------------↑ 생성 ↑---------------┘

                        //---------------↓ 사용자 데이터 추가 부분 ↓---------------┐
                        //GridMaster.LoadCSV_OnlyData( dgv , System.Windows.Forms.Application.StartupPath + "\\AAAA.csv" );//셀데이터로드
                        //dgv.Rows[ 0 ].Cells[ 0 ].Value = "CORE HEIGHT 1";


                        //---------------↑ 사용자 데이터 추가 부분 ↑---------------┘

                        //---------------↓ 정렬 ↓---------------┐
                        GridMaster.CenterAlign(dgv);
                        //GridMaster.LeftAlign( dgv );
                        //GridMaster.Align( dgv , 0 , DataGridViewContentAlignment.MiddleLeft );//단일 Column 정렬
                        //---------------↑ 정렬 ↑---------------┘

                        //---------------↓ 설정 ↓---------------┐
                        dgv.ReadOnly = true;//읽기전용
                        GridMaster.DisableSortColumn(dgv);//오름차순 내림차순 정렬 막기
                        //dgv.Columns[ 0 ].ReadOnly = true;//읽기전용
                        //dgv.AllowUserToResizeColumns = false;//컬럼폭 수정불가

                        //dgv.Columns[ 1 ].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";//표시형식
                        //dgv.ColumnHeadersVisible = false;//컬럼헤더 가리기                        
                        //dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//스페이스 시 줄바꿈
                        //dgv.DefaultCellStyle.BackColor = Color.Black;//색반전
                        //dgv.DefaultCellStyle.ForeColor = Color.White;//색반전

                        //---------------↑ 설정 ↑---------------┘



                    }
                    catch (Exception)
                    {

                    }

                    break;

                case "dgvM0":

                    try
                    {
                        //---------------↓ 기본 ↓---------------┐
                        DataGridView dgv = (DataGridView)Reflection_K.Get(this, name);//이름가져옴
                        string DGV_name = dgv.Name;//적용
                        int height = int.Parse(DataRW.Load_Simple(DGV_name + "H", "30"));//데이터가져옴
                        int fontheader = int.Parse(DataRW.Load_Simple(DGV_name + "FH", "12"));//데이터가져옴
                        int fontcell = int.Parse(DataRW.Load_Simple(DGV_name + "FC", "12"));//데이터가져옴
                        GridMaster.FontSize2(dgv, fontheader, fontcell);//적용
                        //---------------↑ 기본 ↑---------------┘

                        //---------------↓ 생성 ↓---------------┐
                        string[] ColumnsName = new string[] {
                             "모델번호" , "모델이름"
                        };
                        int rows = 19;//초기 생성 Row수
                        GridMaster.Init3(dgv, true, height, rows, ColumnsName);
                        //---------------↑ 생성 ↑---------------┘

                        //---------------↓ 사용자 데이터 추가 부분 ↓---------------┐
                        GridMaster.LoadCSV_OnlyData(dgv, System.Windows.Forms.Application.StartupPath + "\\Model1.csv");//셀데이터로드
                        //dgv.Rows[ 0 ].Cells[ 0 ].Value = "CORE HEIGHT 1";
                        for (int i = 0; i < 19; i++)
                        {
                            dgv.Rows[i].Cells[0].Value = (i + 1).ToString();

                        }

                        //---------------↑ 사용자 데이터 추가 부분 ↑---------------┘

                        //---------------↓ 정렬 ↓---------------┐
                        GridMaster.CenterAlign(dgv);
                        //GridMaster.LeftAlign( dgv );
                        //GridMaster.Align( dgv , 0 , DataGridViewContentAlignment.MiddleLeft );//단일 Column 정렬
                        //---------------↑ 정렬 ↑---------------┘

                        //---------------↓ 설정 ↓---------------┐
                        dgv.ReadOnly = true;//읽기전용
                        GridMaster.DisableSortColumn(dgv);//오름차순 내림차순 정렬 막기
                        //dgv.Columns[ 0 ].ReadOnly = true;//읽기전용
                        //dgv.AllowUserToResizeColumns = false;//컬럼폭 수정불가

                        //dgv.Columns[ 1 ].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";//표시형식
                        //dgv.ColumnHeadersVisible = false;//컬럼헤더 가리기                        
                        //dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//스페이스 시 줄바꿈
                        //dgv.DefaultCellStyle.BackColor = Color.Black;//색반전
                        //dgv.DefaultCellStyle.ForeColor = Color.White;//색반전

                        //---------------↑ 설정 ↑---------------┘



                    }
                    catch (Exception)
                    {

                    }

                    break;

                case "dgvM1":

                    try
                    {
                        //---------------↓ 기본 ↓---------------┐
                        DataGridView dgv = (DataGridView)Reflection_K.Get(this, name);//이름가져옴
                        string DGV_name = dgv.Name;//적용
                        int height = int.Parse(DataRW.Load_Simple(DGV_name + "H", "30"));//데이터가져옴
                        int fontheader = int.Parse(DataRW.Load_Simple(DGV_name + "FH", "12"));//데이터가져옴
                        int fontcell = int.Parse(DataRW.Load_Simple(DGV_name + "FC", "12"));//데이터가져옴
                        GridMaster.FontSize2(dgv, fontheader, fontcell);//적용
                        //---------------↑ 기본 ↑---------------┘

                        //---------------↓ 생성 ↓---------------┐
                        string[] ColumnsName = new string[] {
                             "모델번호" , "모델이름"
                        };
                        int rows = 19;//초기 생성 Row수
                        GridMaster.Init3(dgv, true, height, rows, ColumnsName);
                        //---------------↑ 생성 ↑---------------┘

                        //---------------↓ 사용자 데이터 추가 부분 ↓---------------┐
                        GridMaster.LoadCSV_OnlyData(dgv, System.Windows.Forms.Application.StartupPath + "\\Model2.csv");//셀데이터로드
                        //dgv.Rows[ 0 ].Cells[ 0 ].Value = "CORE HEIGHT 1";
                        for (int i = 0; i < 19; i++)
                        {
                            dgv.Rows[i].Cells[0].Value = (i + 1).ToString();

                        }

                        //---------------↑ 사용자 데이터 추가 부분 ↑---------------┘

                        //---------------↓ 정렬 ↓---------------┐
                        GridMaster.CenterAlign(dgv);
                        //GridMaster.LeftAlign( dgv );
                        //GridMaster.Align( dgv , 0 , DataGridViewContentAlignment.MiddleLeft );//단일 Column 정렬
                        //---------------↑ 정렬 ↑---------------┘

                        //---------------↓ 설정 ↓---------------┐
                        dgv.ReadOnly = true;//읽기전용
                        GridMaster.DisableSortColumn(dgv);//오름차순 내림차순 정렬 막기
                        //dgv.Columns[ 0 ].ReadOnly = true;//읽기전용
                        //dgv.AllowUserToResizeColumns = false;//컬럼폭 수정불가

                        //dgv.Columns[ 1 ].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";//표시형식
                        //dgv.ColumnHeadersVisible = false;//컬럼헤더 가리기                        
                        //dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//스페이스 시 줄바꿈
                        //dgv.DefaultCellStyle.BackColor = Color.Black;//색반전
                        //dgv.DefaultCellStyle.ForeColor = Color.White;//색반전

                        //---------------↑ 설정 ↑---------------┘



                    }
                    catch (Exception)
                    {

                    }

                    break;

                case "dgvES0":

                    try
                    {
                        //---------------↓ 기본 ↓---------------┐
                        DataGridView dgv = (DataGridView)Reflection_K.Get(this, name);//이름가져옴
                        string DGV_name = dgv.Name;//적용
                        int height = int.Parse(DataRW.Load_Simple(DGV_name + "H", "30"));//데이터가져옴
                        int fontheader = int.Parse(DataRW.Load_Simple(DGV_name + "FH", "12"));//데이터가져옴
                        int fontcell = int.Parse(DataRW.Load_Simple(DGV_name + "FC", "12"));//데이터가져옴
                        GridMaster.FontSize2(dgv, fontheader, fontcell);//적용
                        //---------------↑ 기본 ↑---------------┘

                        //---------------↓ 생성 ↓---------------┐
                        string[] ColumnsName = new string[] {
                            "A" , "B"
                        };
                        int rows = 0;//초기 생성 Row수
                        GridMaster.Init3(dgv, true, height, rows, ColumnsName);
                        //---------------↑ 생성 ↑---------------┘

                        //---------------↓ 사용자 데이터 추가 부분 ↓---------------┐
                        //GridMaster.LoadCSV_OnlyData( dgv , System.Windows.Forms.Application.StartupPath + "\\AAAA.csv" );//셀데이터로드
                        //dgv.Rows[ 0 ].Cells[ 0 ].Value = "CORE HEIGHT 1";




                        //---------------↑ 사용자 데이터 추가 부분 ↑---------------┘

                        //---------------↓ 정렬 ↓---------------┐
                        //GridMaster.CenterAlign( dgv );
                        GridMaster.LeftAlign(dgv);
                        //GridMaster.Align( dgv , 0 , DataGridViewContentAlignment.MiddleLeft );//단일 Column 정렬
                        //---------------↑ 정렬 ↑---------------┘

                        //---------------↓ 설정 ↓---------------┐
                        dgv.ReadOnly = true;//읽기전용
                        GridMaster.DisableSortColumn(dgv);//오름차순 내림차순 정렬 막기
                        //dgv.Columns[ 0 ].ReadOnly = true;//읽기전용
                        //dgv.AllowUserToResizeColumns = false;//컬럼폭 수정불가

                        //dgv.Columns[ 1 ].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";//표시형식
                        dgv.ColumnHeadersVisible = false;//컬럼헤더 가리기                        
                        //dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//스페이스 시 줄바꿈
                        //dgv.DefaultCellStyle.BackColor = Color.Black;//색반전
                        //dgv.DefaultCellStyle.ForeColor = Color.White;//색반전

                        //---------------↑ 설정 ↑---------------┘



                    }
                    catch (Exception)
                    {

                    }

                    break;
                case "dgvES1":

                    try
                    {
                        //---------------↓ 기본 ↓---------------┐
                        DataGridView dgv = (DataGridView)Reflection_K.Get(this, name);//이름가져옴
                        string DGV_name = dgv.Name;//적용
                        int height = int.Parse(DataRW.Load_Simple(DGV_name + "H", "30"));//데이터가져옴
                        int fontheader = int.Parse(DataRW.Load_Simple(DGV_name + "FH", "12"));//데이터가져옴
                        int fontcell = int.Parse(DataRW.Load_Simple(DGV_name + "FC", "12"));//데이터가져옴
                        GridMaster.FontSize2(dgv, fontheader, fontcell);//적용
                        //---------------↑ 기본 ↑---------------┘

                        //---------------↓ 생성 ↓---------------┐
                        string[] ColumnsName = new string[] {
                            "A" , "B"
                        };
                        int rows = 0;//초기 생성 Row수
                        GridMaster.Init3(dgv, true, height, rows, ColumnsName);
                        //---------------↑ 생성 ↑---------------┘

                        //---------------↓ 사용자 데이터 추가 부분 ↓---------------┐
                        //GridMaster.LoadCSV_OnlyData( dgv , System.Windows.Forms.Application.StartupPath + "\\AAAA.csv" );//셀데이터로드
                        //dgv.Rows[ 0 ].Cells[ 0 ].Value = "CORE HEIGHT 1";




                        //---------------↑ 사용자 데이터 추가 부분 ↑---------------┘

                        //---------------↓ 정렬 ↓---------------┐
                        //GridMaster.CenterAlign( dgv );
                        GridMaster.LeftAlign(dgv);
                        //GridMaster.Align( dgv , 0 , DataGridViewContentAlignment.MiddleLeft );//단일 Column 정렬
                        //---------------↑ 정렬 ↑---------------┘

                        //---------------↓ 설정 ↓---------------┐
                        dgv.ReadOnly = true;//읽기전용
                        GridMaster.DisableSortColumn(dgv);//오름차순 내림차순 정렬 막기
                        //dgv.Columns[ 0 ].ReadOnly = true;//읽기전용
                        //dgv.AllowUserToResizeColumns = false;//컬럼폭 수정불가

                        //dgv.Columns[ 1 ].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";//표시형식
                        dgv.ColumnHeadersVisible = false;//컬럼헤더 가리기                        
                        //dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//스페이스 시 줄바꿈
                        //dgv.DefaultCellStyle.BackColor = Color.Black;//색반전
                        //dgv.DefaultCellStyle.ForeColor = Color.White;//색반전

                        //---------------↑ 설정 ↑---------------┘



                    }
                    catch (Exception)
                    {

                    }

                    break;
                    //case "dgvES2":

                    //    try
                    //    {
                    //        //---------------↓ 기본 ↓---------------┐
                    //        DataGridView dgv = (DataGridView)Reflection_K.Get(this, name);//이름가져옴
                    //        string DGV_name = dgv.Name;//적용
                    //        int height = int.Parse(DataRW.Load_Simple(DGV_name + "H", "30"));//데이터가져옴
                    //        int fontheader = int.Parse(DataRW.Load_Simple(DGV_name + "FH", "12"));//데이터가져옴
                    //        int fontcell = int.Parse(DataRW.Load_Simple(DGV_name + "FC", "12"));//데이터가져옴
                    //        GridMaster.FontSize2(dgv, fontheader, fontcell);//적용
                    //        //---------------↑ 기본 ↑---------------┘

                    //        //---------------↓ 생성 ↓---------------┐
                    //        string[] ColumnsName = new string[] {
                    //            "A" , "B"
                    //        };
                    //        int rows = 0;//초기 생성 Row수
                    //        GridMaster.Init3(dgv, true, height, rows, ColumnsName);
                    //        //---------------↑ 생성 ↑---------------┘

                    //        //---------------↓ 사용자 데이터 추가 부분 ↓---------------┐
                    //        //GridMaster.LoadCSV_OnlyData( dgv , System.Windows.Forms.Application.StartupPath + "\\AAAA.csv" );//셀데이터로드
                    //        //dgv.Rows[ 0 ].Cells[ 0 ].Value = "CORE HEIGHT 1";




                    //        //---------------↑ 사용자 데이터 추가 부분 ↑---------------┘

                    //        //---------------↓ 정렬 ↓---------------┐
                    //        //GridMaster.CenterAlign( dgv );
                    //        GridMaster.LeftAlign(dgv);
                    //        //GridMaster.Align( dgv , 0 , DataGridViewContentAlignment.MiddleLeft );//단일 Column 정렬
                    //        //---------------↑ 정렬 ↑---------------┘

                    //        //---------------↓ 설정 ↓---------------┐
                    //        dgv.ReadOnly = true;//읽기전용
                    //        GridMaster.DisableSortColumn(dgv);//오름차순 내림차순 정렬 막기
                    //        //dgv.Columns[ 0 ].ReadOnly = true;//읽기전용
                    //        //dgv.AllowUserToResizeColumns = false;//컬럼폭 수정불가

                    //        //dgv.Columns[ 1 ].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";//표시형식
                    //        dgv.ColumnHeadersVisible = false;//컬럼헤더 가리기                        
                    //        //dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//스페이스 시 줄바꿈
                    //        //dgv.DefaultCellStyle.BackColor = Color.Black;//색반전
                    //        //dgv.DefaultCellStyle.ForeColor = Color.White;//색반전

                    //        //---------------↑ 설정 ↑---------------┘



                    //    }
                    //    catch (Exception)
                    //    {

                    //    }

                    //    break;
                    //case "dgvES3":

                    //    try
                    //    {
                    //        //---------------↓ 기본 ↓---------------┐
                    //        DataGridView dgv = (DataGridView)Reflection_K.Get(this, name);//이름가져옴
                    //        string DGV_name = dgv.Name;//적용
                    //        int height = int.Parse(DataRW.Load_Simple(DGV_name + "H", "30"));//데이터가져옴
                    //        int fontheader = int.Parse(DataRW.Load_Simple(DGV_name + "FH", "12"));//데이터가져옴
                    //        int fontcell = int.Parse(DataRW.Load_Simple(DGV_name + "FC", "12"));//데이터가져옴
                    //        GridMaster.FontSize2(dgv, fontheader, fontcell);//적용
                    //        //---------------↑ 기본 ↑---------------┘

                    //        //---------------↓ 생성 ↓---------------┐
                    //        string[] ColumnsName = new string[] {
                    //            "A" , "B"
                    //        };
                    //        int rows = 0;//초기 생성 Row수
                    //        GridMaster.Init3(dgv, true, height, rows, ColumnsName);
                    //        //---------------↑ 생성 ↑---------------┘

                    //        //---------------↓ 사용자 데이터 추가 부분 ↓---------------┐
                    //        //GridMaster.LoadCSV_OnlyData( dgv , System.Windows.Forms.Application.StartupPath + "\\AAAA.csv" );//셀데이터로드
                    //        //dgv.Rows[ 0 ].Cells[ 0 ].Value = "CORE HEIGHT 1";




                    //        //---------------↑ 사용자 데이터 추가 부분 ↑---------------┘

                    //        //---------------↓ 정렬 ↓---------------┐
                    //        //GridMaster.CenterAlign( dgv );
                    //        GridMaster.LeftAlign(dgv);
                    //        //GridMaster.Align( dgv , 0 , DataGridViewContentAlignment.MiddleLeft );//단일 Column 정렬
                    //        //---------------↑ 정렬 ↑---------------┘

                    //        //---------------↓ 설정 ↓---------------┐
                    //        dgv.ReadOnly = true;//읽기전용
                    //        GridMaster.DisableSortColumn(dgv);//오름차순 내림차순 정렬 막기
                    //        //dgv.Columns[ 0 ].ReadOnly = true;//읽기전용
                    //        //dgv.AllowUserToResizeColumns = false;//컬럼폭 수정불가

                    //        //dgv.Columns[ 1 ].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";//표시형식
                    //        dgv.ColumnHeadersVisible = false;//컬럼헤더 가리기                        
                    //        //dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//스페이스 시 줄바꿈
                    //        //dgv.DefaultCellStyle.BackColor = Color.Black;//색반전
                    //        //dgv.DefaultCellStyle.ForeColor = Color.White;//색반전

                    //        //---------------↑ 설정 ↑---------------┘



                    //    }
                    //    catch (Exception)
                    //    {

                    //    }

                    //    break;
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
            if (e.Button.ToString().Equals("Middle"))
            {
                DataGridView thisdgv = (DataGridView)sender;
                dgvmanager = new Ken2.UIControl.dgvManager(thisdgv);
                dgvmanager.Init += OnInit;
                dgvmanager.Show();
            }
        }

        private void dgvD1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString().Equals("Middle"))
            {
                DataGridView thisdgv = (DataGridView)sender;
                dgvmanager = new Ken2.UIControl.dgvManager(thisdgv);
                dgvmanager.Init += OnInit;
                dgvmanager.Show();
            }
        }

        private void dgvD2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString().Equals("Middle"))
            {
                DataGridView thisdgv = (DataGridView)sender;
                dgvmanager = new Ken2.UIControl.dgvManager(thisdgv);
                dgvmanager.Init += OnInit;
                dgvmanager.Show();
            }
        }

        private void dgvM0_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString().Equals("Middle"))
            {
                DataGridView thisdgv = (DataGridView)sender;
                dgvmanager = new Ken2.UIControl.dgvManager(thisdgv);
                dgvmanager.Init += OnInit;
                dgvmanager.Show();
            }
        }

        private void dgvM1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString().Equals("Middle"))
            {
                DataGridView thisdgv = (DataGridView)sender;
                dgvmanager = new Ken2.UIControl.dgvManager(thisdgv);
                dgvmanager.Init += OnInit;
                dgvmanager.Show();
            }
        }

        private void dgvH0_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString().Equals("Middle"))
            {
                DataGridView thisdgv = (DataGridView)sender;
                dgvmanager = new Ken2.UIControl.dgvManager(thisdgv);
                dgvmanager.Init += OnInit;
                dgvmanager.Show();
            }
        }

        private void dgvHN0_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString().Equals("Middle"))
            {
                DataGridView thisdgv = (DataGridView)sender;
                dgvmanager = new Ken2.UIControl.dgvManager(thisdgv);
                dgvmanager.Init += OnInit;
                dgvmanager.Show();
            }
        }

        private void dgvP0_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString().Equals("Middle"))
            {
                DataGridView thisdgv = (DataGridView)sender;
                dgvmanager = new Ken2.UIControl.dgvManager(thisdgv);
                dgvmanager.Init += OnInit;
                dgvmanager.Show();
            }
        }

        private void dgvC0_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString().Equals("Middle"))
            {
                DataGridView thisdgv = (DataGridView)sender;
                dgvmanager = new Ken2.UIControl.dgvManager(thisdgv);
                dgvmanager.Init += OnInit;
                dgvmanager.Show();
            }
        }

        private void dgvC1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString().Equals("Middle"))
            {
                DataGridView thisdgv = (DataGridView)sender;
                dgvmanager = new Ken2.UIControl.dgvManager(thisdgv);
                dgvmanager.Init += OnInit;
                dgvmanager.Show();
            }
        }

        private void dgvDE0_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString().Equals("Middle"))
            {
                DataGridView thisdgv = (DataGridView)sender;
                dgvmanager = new Ken2.UIControl.dgvManager(thisdgv);
                dgvmanager.Init += OnInit;
                dgvmanager.Show();
            }
        }

        private void dgvES0_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString().Equals("Middle"))
            {
                DataGridView thisdgv = (DataGridView)sender;
                dgvmanager = new Ken2.UIControl.dgvManager(thisdgv);
                dgvmanager.Init += OnInit;
                dgvmanager.Show();
            }
        }

        private void dgvES1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString().Equals("Middle"))
            {
                DataGridView thisdgv = (DataGridView)sender;
                dgvmanager = new Ken2.UIControl.dgvManager(thisdgv);
                dgvmanager.Init += OnInit;
                dgvmanager.Show();
            }
        }

        private void dgvXR0_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button.ToString().Equals("Middle"))
            {
                DataGridView thisdgv = (DataGridView)sender;
                dgvmanager = new Ken2.UIControl.dgvManager(thisdgv);
                dgvmanager.Init += OnInit;
                dgvmanager.Show();
            }
        } 
        #endregion


        bool BarcodeCheck(string bcr, string columnsname, int okcnt_cutline)
        {
            DataSet ds = sql.ExecuteQuery("SELECT * FROM table1 WHERE `" + columnsname + "`='" + bcr + "' ;");
            int okcnt = 0;


            if (ds.Tables[0].Rows.Count > 0)//해당 바코드 찾았다.
            {
                int cnt = ds.Tables[0].Columns.Count;//컬럼 몇개니

                for (int i = 0; i < cnt; i++)//그 컬럼 수 안에 OK수량 계산
                {
                    if (ds.Tables[0].Rows[0][i].ToString().Equals("NG"))//NG하나라도 있음 return false다
                        return false;
                    else if (ds.Tables[0].Rows[0][i].ToString().Equals("OK"))//OK 카운트해라.
                        okcnt++;

                }

                if (okcnt >= okcnt_cutline)//OK 수량 커트라인에 합격 했나?
                    return true;
                else//수량 안맞으면 NG다
                    return false;

            }
            else//그런 바코드 데이터 한개도 없다
                return false;
        }

        #region InputItem 관련
        private static DateTime Delay(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
            }

            return DateTime.Now;
        }

        /// <summary>
        /// 가장 아래 아이템을 선택하고 오래된 것을 지웁니다.
        /// Limit_Item 화면에 표시 될 MAX ITEM 수
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="Limit_Item"></param>
        void RefreshDGVdata(DataGridView dgv, int Limit_Item)
        {
            int i = dgv.RowCount;
            dgv.CurrentCell = dgv.Rows[i - 1].Cells[0];

            if (i > Limit_Item)
                dgv.Rows.RemoveAt(0);
        }

        void InputItem(DataGridView dgv, string str1)
        {
            dgv.Rows.Add(str1);//추가

            int End_Item = dgv.RowCount - 1;//마지막 아이템

            RefreshDGVdata(dgv, 100);//오래된 아이템 삭제 후 마지막 아이템 강제선택
        }

        void InputItem(DataGridView dgv, string str1, string str2)
        {
            int decision_arr = 1;//판정column번호
            dgv.Rows.Add(str1, str2);//추가

            int End_Item = dgv.RowCount - 1;//마지막 아이템
            string str = dgv.Rows[End_Item].Cells[decision_arr].Value.ToString();

            //---------------↓ 색칠 ↓---------------┐
            if (str.Equals("OK"))//OK
                dgv.Rows[End_Item].Cells[decision_arr].Style.BackColor = Color.LightGreen;
            else if (str.Equals("NG"))
                dgv.Rows[End_Item].Cells[decision_arr].Style.BackColor = Color.IndianRed;
            //---------------↑ 색칠 ↑---------------┘

            RefreshDGVdata(dgv, 100);//오래된 아이템 삭제 후 마지막 아이템 강제선택
        } 
        #endregion

        //fffffffff
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);
            xtraTabControl1.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
            title_kenlb.Controls.Add(title_lbc);
            title_kenlb.Controls.Add(title_piced);

            LoadTxt();
            pass = textBox10.Text;

            cpkdecision.Value = (decimal)double.Parse(RWdataFast.Load("cpk", 0));

#if Release
            string plc1_ip = "192.168.13.10";
            string plc2_ip = "192.168.13.110";
            string printer_ip = "192.168.13.41";
            string pc = "192.168.13.173";

#else
            string plc1_ip = "192.168.56.1";
            string plc2_ip = "192.168.56.1";
            string printer_ip = "192.168.56.1";
            string pc = "192.168.56.1";

#endif

            dgvInit( "dgvD0" );//메인화면
            dgvInit( "dgvD1" );
            dgvInit( "dgvD2" );
            
            dgvInit( "dgvXR0" );

            dgvInit( "dgvC0" );//PLC와 통신 데이터 보는 그리드
            dgvInit( "dgvC1" );
            //dgvInit( "dgvC2" );

            dgvInit( "dgvM0" );
            dgvInit( "dgvM1" );
            //dgvInit( "dgvM2" );

            dgvInit( "dgvH0" );
            dgvInit( "dgvHN0" );

            dgvInit( "dgvP0" );
            //dgvInit( "dgvP1" );
            
            dgvInit( "dgvES0" );
            dgvInit( "dgvES1" );
            //dgvInit( "dgvES2" );
            //dgvInit( "dgvES3" );

            dgvInit( "dgvDE0" );

            //---------------↓ PLC와 통신 ↓---------------┐


            //192.168.13.10
            //D5000 ~ D5259
            plc1 = new TCPClient_PLC1(plc1_ip, 12417, 1000, this);
            plc1.TalkingComm += plc1_TalkingComm;

            //192.168.13.110
            plc2 = new TCPClient_PLC2(plc2_ip, 12419, 1000, this);
            plc2.TalkingComm += plc2_TalkingComm;

            ////192.168.13.150
            //plc3 = new TCPClient_PLC3(plc3_ip, 12421, 1000, this);
            //plc3.TalkingComm += plc3_TalkingComm;


            //---------------↑ PLC와 통신 ↑---------------┘

            //불량테스트 핸디
            handyconv = new TCPClient_HandyConverter("192.168.100.2", 1470, 500);
            handyconv.TalkingComm += handyconv_TalkingComm;

            LabelPrinter = new TCPClient_LabelPrinter(printer_ip,9100, 1000);
            //LabelPrinter.TalkingComm += LabelPrinter_TalkingComm;
            //모니터 수량 PC
            monitor_pc = new TCPClient_Monitor(pc, 9100, 500, this);

            sql = new Mysql_K("127.0.0.1", "kb_metal_2", "table1", "a", "qwerasdf");


            MainThreadStart(0);//시계 및 모니터PC수량전송
            SetToday();

            DayNightLoad();
            //DayNightLoad2();

            TimeLoad(daynight0);
            TimeLoad(daynight1);
            TimeLoad(daynight2);
            TimeLoad(daynight3);

            Directory.CreateDirectory("D:\\" + Mainpath + "\\Log");
            Log_K.WriteLog(log_lst, Mainpath, "프로그램 시작");

#if !Release

            ModelLoad(1);
            ModelLoad1(1);
            //ModelLoad2(1);

#endif

        }

        //cccccccccc
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveTxt();
            MainThreadStop();

            try
            {
                plc1.Dispose();
            }
            catch (Exception)
            {

            }

            try
            {
                plc2.Dispose();
            }
            catch (Exception)
            {

            }

            //try
            //{
            //    plc3.Dispose();
            //}
            //catch (Exception)
            //{

            //}

            try
            {
                LabelPrinter.Dispose();
            }
            catch (Exception)
            {

            }

            //try
            //{
            //    LabelPrinter2.Dispose();
            //}
            //catch (Exception)
            //{

            //}

            try
            {
                handyconv.Dispose();
            }
            catch (Exception)
            {

            }


            Thread.Sleep(3000);

            try
            {
                Process.GetCurrentProcess().Kill();
            }
            catch (Exception)
            {


            }

        }

        void SetToday()
        {
            Date0.Value = DateTime.Now;

            Date1.Value = DateTime.Now;

            Time0.Time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

            Time1.Time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

        }

        private void plc1_TalkingComm(string name, object data, string data2, string data3, string data4, string data5, string data6, string data7, string data8, string data9, string data10, string data11, string data12)
        {
            if (name.Equals("CommData"))//데이터 보기
            {
                this.Invoke(new dele(() =>
                {
                    int[] CommData = (int[])data;

                    for (int i = 0; i < CommData.Length; i++)
                    {
                        dgvC0.Rows[i].Cells[2].Value = CommData[i];
                    }
                }));
            }

            if (name.Equals("LabelPrint"))//바코드 라벨 출력 labelprint
            {
                try
                {
                    //Delay( 100 );
                    if (LabelPrinter.PrinterStatus == 0)
                    {

                        string bcr = PrintOne();

                        string cmd = SQLCMD.MakeUpdateCmdSentence_where_equals(sql.table, "barcode2", LastSavedBarcode2, "",    //  임펠러 바코드 찾아서 블로어 바코드 업데이트
                          "barcode1", bcr
                        );

                        sql.ExecuteNonQuery(cmd);

                        this.Invoke(new dele(() =>
                        {
                            Log_K.WriteLog(log_lst, Mainpath, "라벨프린터 자동출력 LastBarcode : " + LastSavedBarcode2);
                            Log_K.WriteLog(log_lst, Mainpath, "라벨프린터 자동출력 만든 바코드 bcr : " + bcr);
                            //InputItem( dgvD0 , LastSavedBarcode2 , "바코드 연결" , bcr , "OK" );
                            InputItem(dgvD1, bcr + " / 블로워 라벨 출력 [2]", "OK");
                            //dgvD0.Rows[0].Cells[1].Value = bcr;   //  주석처리 0907
                            LastSavedBarcode2 = "";

                        }));
                        
                        plc1.MCWrite(6011, 1);//저장했습니다.

                    }

                }

                catch (Exception exc)
                {
                    InputItem(dgvD1, "블로워 라벨 출력 [2] - ERROR", "NG");
                    this.Invoke(new dele(() =>
                    {
                        Log_K.WriteLog(log_lst, Mainpath, "라벨프린터 자동출력 실패 - ERROR");
                    }));
                }

            }
            #region 수동출력 1번프린터

            if (name.Equals("LabelPrintManual"))//바코드 라벨 출력 수동 labelprint
            {
                try
                {
                    if (LabelPrinter.PrinterStatus == 0)
                    {

                        string bcr = PrintOne();


                        string cmd = SQLCMD.MakeUpdateCmdSentence_where_equals(sql.table, "barcode2", LastSavedBarcode2, "",
                          "barcode1", bcr
                        );

                        sql.ExecuteNonQuery(cmd);


                        this.Invoke(new dele(() =>
                        {
                            Log_K.WriteLog(log_lst, Mainpath, "라벨프린터 수동출력 LastBarcode : " + LastSavedBarcode2);
                            Log_K.WriteLog(log_lst, Mainpath, "라벨프린터 수동출력 만든 바코드 bcr : " + bcr);
                            //InputItem( dgvD0 , LastSavedBarcode2 , "바코드 연결" , bcr , "OK" );
                            InputItem(dgvD1, bcr + " / 블로워 라벨 출력 [2]", "OK");
                            //dgvD0.Rows [ 0 ].Cells [ 1 ].Value = bcr;
                            LastSavedBarcode2 = "";

                        }));


                        plc1.MCWrite(6012, 1);//저장했습니다.

                    }

                }
                catch (Exception exc)
                {
                    InputItem(dgvD1, "블로워 라벨 수동 출력 [2] - ERROR", "NG");
                    Log_K.WriteLog(log_lst, Mainpath, "라벨프린터 수동출력 에러 - ERROR");
                }

            }
            #endregion

            if (name.Equals("Save1"))//32개바이트   //  D5010 라벨 부착부 Data 읽기 요구시 DB에 임펠러 / 어퍼 바코드 저장함.
            {
                //Delay( 500 );
                MessageBox.Show("save1");
                string barcode2 = data2;  // 임펠라
                string barcode3 = data3;  // 어퍼
                string barcode4 = data7;  // 특성 저항 판정
                string barcode5 = data8;  // 특성 저항 검사 측정값


                //LastSavedBarcode2 = barcode2;

                //if ( barcode2 != null && barcode2 != "" && barcode3 != null && barcode3 != "" )
                //{
                try
                {
                    int rows = sql.ExecuteQuery_Select_Count("SELECT COUNT(*) FROM table1 WHERE `Barcode2`='" + barcode2 + "' ;");

                    char[] decision = data4.ToCharArray();
                    string[] decision_str = new string[16];
                    for (int i = 0; i < 16; i++)
                    {
                        if (decision[i] == '1')
                            decision_str[15 - i] = "OK";
                        else
                            decision_str[15 - i] = "NG";
                    }

                    if (rows == 0)//없을때
                    {
                        string cmd = Ken2.Database.SQLiteCMD_K.MakeInsertCmdSentence(sql.table,

                      "barcode2", barcode2,
                      "barcode3", barcode3,
                      "Datetime", Dtime.Now(Dtime.StringType.ForDatum),

                      "Model", ModelNamelbl.Text,

                        "c1", decision_str[15],

                        "c14", data5,
                        "c15", data6,
                        "c16", data7,
                        "c17", data8,
                        "c18", data9,

                        "c24", barcode4,
                        "c25", barcode5,

                        "c180", data10

                         );
                        sql.ExecuteNonQuery(cmd);

                        this.Invoke(new dele(() =>
                        {
                            Log_K.WriteLog(log_lst, Mainpath, "DB데이터 없음 인서트 완료");
                            Log_K.WriteLog(log_lst, Mainpath, "임펠라바코드 / 어퍼바코드 / 결과 / 측정값 / #50스페이서측정 / #90스페이서측정 / 베어링 거리 / 베어링 하중 / 스토퍼 높이 / 특성 저항 판정 / 특성저항 검사 측정값");
                            Log_K.WriteLog(log_lst, Mainpath, barcode2 + "/" + barcode3 + "/" + decision_str[15] + "/" + data5 + "/" + data6 + "/" + data7 + "/" + data8 + "/" + data9 + "/" + data10 + "/" + barcode4 + "/" +barcode5);
                        }));
                    }

                    else//데이터 있다.
                    {
                        string cmd = SQLCMD.MakeUpdateCmdSentence_where_equals(sql.table, "Barcode2", barcode2, "",

                            "barcode2", barcode2,
                            "barcode3", barcode3,
                            "Datetime", Dtime.Now(Dtime.StringType.ForDatum),

                      "Model", ModelNamelbl.Text,

                        "c1", decision_str[15],

                        "c14", data5,
                        "c15", data6,
                        "c16", data7,
                        "c17", data8,
                        "c18", data9,

                        "c24", barcode4,
                        "c25", barcode5,

                        "c180", data10

                        );
                        sql.ExecuteNonQuery(cmd);

                        this.Invoke(new dele(() =>
                        {
                            Log_K.WriteLog(log_lst, Mainpath, "DB데이터 있음 업데이트 완료");
                            Log_K.WriteLog(log_lst, Mainpath, "임펠라바코드 / 어퍼바코드 / 결과 / 측정값 / #50스페이서측정 / #90스페이서측정 / 베어링 거리 / 베어링 하중 / 스토퍼 높이 / 특성 저항 판정 / 특성저항 검사 측정값");
                            Log_K.WriteLog(log_lst, Mainpath, barcode2 + "/" + barcode3 + "/" + decision_str[15] + "/" + data5 + "/" + data6 + "/" + data7 + "/" + data8 + "/" + data9 + "/" + data10 + "/" + barcode4 + "/" + barcode5);
                        }));

                    }

                    this.Invoke(new dele(() =>
                    {
                        InputItem(dgvD1, barcode2 + " / #C40 저항 검사 [1]", decision_str[15]);

                        int line = 1;

                        dgvD0.Rows[1].Cells[line].Value = barcode2; //  임펠라
                        dgvD0.Rows[2].Cells[line].Value = barcode3; //  어퍼
                        dgvD0.Rows[3].Cells[line].Value = decision_str[15]; //  결과
                        dgvD0.Rows[4].Cells[line].Value = data5;    //  측정값
                        dgvD0.Rows[5].Cells[line].Value = data6;    //  #50
                        dgvD0.Rows[6].Cells[line].Value = data7;    //  #90
                        dgvD0.Rows[7].Cells[line].Value = data8;    //  거리
                        dgvD0.Rows[8].Cells[line].Value = data9;    //  하중
                        dgvD0.Rows[9].Cells[line].Value = data10;   //  높이

                        GridMaster.Color_Painting(dgvD0, line);

                        Log_K.WriteLog(log_lst, Mainpath, "Save1 저항검사완료후 라스트바코드 변경");

                    }));

                    LastSavedBarcode2 = barcode2;

                    this.Invoke(new dele(() =>
                    {

                        Log_K.WriteLog(log_lst, Mainpath, "최종 LastSaveBarcode2 : " + LastSavedBarcode2);
                    }));
                }

                catch (Exception exc)
                {
                    InputItem(dgvD1, barcode2 + " / #C40 저항 검사 [1] - ERROR", "NG");

                    this.Invoke(new dele(() =>
                    {
                        Log_K.WriteLog(log_lst, Mainpath, "Save1 실패 에러");
                    }));
                }

                plc1.MCWrite(6010, 1);//저장했습니다.

                //}

                //else
                //{
                //    this.Invoke( new dele( ( ) =>
                //    {
                //        Log_K.WriteLog( log_lst, Mainpath, "임펠러 or 어퍼 바코드 없음" + barcode2 + " / " + barcode3 );
                //    } ) );

                //    plc1.MCWrite( 6010, 2 ); //임펠러 바코드없음 재검사 요구.
                //}

            }

            if (name.Equals("BarcodeCheck"))//32개바이트
            {
                //#C150 조립 완성품 배출부 바코드 판정 결과 요구
                //Delay( 500 );
                string barcode1 = data2;

                try
                {
                    bool decision = BarcodeCheck(barcode1, "barcode1", 1);
                    string res = "";

                    if (decision)
                    {
                        plc1.MCWrite(6020, 1);
                        res = "OK";

                    }
                    else
                    {
                        plc1.MCWrite(6020, 2);
                        res = "NG";

                    }


                    this.Invoke(new dele(() =>
                    {
                        InputItem(dgvD1, barcode1 + " / #C150 조립 완성품 배출부 바코드 판정 결과 요구 [3]", res);
                    }));
                }
                catch (Exception)
                {
                    InputItem(dgvD1, barcode1 + " / #C150 조립 완성품 배출부 바코드 판정 결과 요구 [3] - ERROR", "NG");

                }

            }

            if (name.Equals("BarcodeCheck2"))//32개바이트
            {
                //#D20  조립 완성품 배출 로더 바코드 판정 결과 요구
                //Delay( 500 );
                string barcode1 = data2;
                string res = "";

                try
                {
                    bool decision = BarcodeCheck(barcode1, "barcode1", 1);

                    if (decision)
                    {
                        plc1.MCWrite(6030, 1);
                        res = "OK";

                    }
                    else
                    {
                        plc1.MCWrite(6030, 2);
                        res = "NG";

                    }


                    this.Invoke(new dele(() =>
                    {
                        InputItem(dgvD1, barcode1 + " / #D20  조립 완성품 배출 로더 바코드 판정 결과 요구 [4]", res);

                    }));

                }
                catch (Exception)
                {
                    InputItem(dgvD1, barcode1 + " / #D20  조립 완성품 배출 로더 바코드 판정 결과 요구 [4] - ERROR", "NG");

                }

            }

            if (name.Equals("Balance"))//32개바이트
            {
                //Delay( 500 );
                string barcode1 = data2;

                try
                {
                    int rows = sql.ExecuteQuery_Select_Count("SELECT COUNT(*) FROM table1 WHERE `Barcode1`='" + barcode1 + "' ;");


                    if (rows == 0)
                    {
                        string cmd = Ken2.Database.SQLiteCMD_K.MakeInsertCmdSentence(sql.table,

                      "barcode1", barcode1,
                      "Datetime", Dtime.Now(Dtime.StringType.ForDatum),
                      "Model", ModelNamelbl.Text,
                      "c19", data3,
                      "c20", data4,
                      "c21", data5,
                      "c22", data6,
                      "c23", data7


                      );

                        sql.ExecuteNonQuery(cmd);

                    }
                    else
                    {
                        string cmd = SQLCMD.MakeUpdateCmdSentence_where_equals(sql.table, "barcode1", barcode1, "",

                      "barcode1", barcode1,
                      "Datetime", Dtime.Now(Dtime.StringType.ForDatum),
                      "Model", ModelNamelbl.Text,
                      "c19", data3,
                      "c20", data4,
                      "c21", data5,
                      "c22", data6,
                      "c23", data7

                      );

                        sql.ExecuteNonQuery(cmd);

                    }

                    this.Invoke(new dele(() =>
                    {
                        InputItem(dgvD1, barcode1 + " / 밸런스1 저장 [5]", data3);
                        Log_K.WriteLog(log_lst, Mainpath, "밸런스1 저장" + barcode1 + "/c19/" + data3 + "/c20/" + data4 + "/c21/" + data5 + "/c22/" + data6 + "/c23/" + data7);
                        int line = 3;

                        dgvD0.Rows[0].Cells[line].Value = data2;    //  바코드
                        dgvD0.Rows[1].Cells[line].Value = data3;    //  판정
                        dgvD0.Rows[2].Cells[line].Value = data4;    //  1차각도
                        dgvD0.Rows[3].Cells[line].Value = data5;    //  1차밸런스양
                        dgvD0.Rows[4].Cells[line].Value = data6;    //  2차각도
                        dgvD0.Rows[5].Cells[line].Value = data7;    //  2차밸런스양

                        GridMaster.Color_Painting(dgvD0, line);

                    }));
                }
                catch (Exception exc)
                {
                    InputItem(dgvD1, barcode1 + " / 밸런스1 저장 [5] - ERROR", "NG");
                    Log_K.WriteLog(log_lst, Mainpath, "밸런스1 저장 실패 - ERROR");
                }

                plc1.MCWrite(6040, 1);//저장했습니다.

            }

            if (name.Equals("Balance2"))//32개바이트
            {
                //Delay( 500 );
                string barcode1 = data2;

                try
                {
                    int rows = sql.ExecuteQuery_Select_Count("SELECT COUNT(*) FROM table1 WHERE `Barcode1`='" + barcode1 + "' ;");


                    if (rows == 0)
                    {
                        string cmd = Ken2.Database.SQLiteCMD_K.MakeInsertCmdSentence(sql.table,

                      "barcode1", barcode1,
                      "Datetime", Dtime.Now(Dtime.StringType.ForDatum),
                      "Model", ModelNamelbl.Text,
                      "c19", data3,
                      "c20", data4,
                      "c21", data5,
                      "c22", data6,
                      "c23", data7


                      );

                        sql.ExecuteNonQuery(cmd);


                    }
                    else
                    {
                        string cmd = SQLCMD.MakeUpdateCmdSentence_where_equals(sql.table, "barcode1", barcode1, "",

                      "barcode1", barcode1,
                      "Datetime", Dtime.Now(Dtime.StringType.ForDatum),
                      "Model", ModelNamelbl.Text,
                      "c19", data3,
                      "c20", data4,
                      "c21", data5,
                      "c22", data6,
                      "c23", data7


                      );

                        sql.ExecuteNonQuery(cmd);


                    }

                    this.Invoke(new dele(() =>
                    {
                        InputItem(dgvD1, barcode1 + " / 밸런스2 저장 [5]", data3);
                        Log_K.WriteLog(log_lst, Mainpath, "밸런스2 저장" + barcode1 + "/c19/" + data3 + "/c20/" + data4 + "/c21/" + data5 + "/c22/" + data6 + "/c23/" + data7);
                        int line = 3;

                        dgvD0.Rows[6].Cells[line].Value = data2;   //  바코드
                        dgvD0.Rows[7].Cells[line].Value = data3;   //  판정
                        dgvD0.Rows[8].Cells[line].Value = data4;   //  1차각도
                        dgvD0.Rows[9].Cells[line].Value = data5;   //  1차밸런스양
                        dgvD0.Rows[10].Cells[line].Value = data6;   //  2차각도
                        dgvD0.Rows[11].Cells[line].Value = data7;   //  2차밸런스양

                        GridMaster.Color_Painting(dgvD0, line);

                    }));
                }
                catch (Exception exc)
                {
                    InputItem(dgvD1, barcode1 + " / 밸런스2 저장 [5] - ERROR", "NG");
                    Log_K.WriteLog(log_lst, Mainpath, "밸런스2 저장 실패 - ERROR");
                }



                plc1.MCWrite(6050, 1);//저장했습니다.

            }

            if (name.Equals("Balance3"))//32개바이트
            {
                //Delay( 500 );
                string barcode1 = data2;

                try
                {
                    int rows = sql.ExecuteQuery_Select_Count("SELECT COUNT(*) FROM table1 WHERE `Barcode1`='" + barcode1 + "' ;");


                    if (rows == 0)
                    {
                        string cmd = Ken2.Database.SQLiteCMD_K.MakeInsertCmdSentence(sql.table,

                      "barcode1", barcode1,
                      "Datetime", Dtime.Now(Dtime.StringType.ForDatum),
                      "Model", ModelNamelbl.Text,
                      "c19", data3,
                      "c20", data4,
                      "c21", data5,
                      "c22", data6,
                      "c23", data7


                      );

                        sql.ExecuteNonQuery(cmd);


                    }
                    else
                    {
                        string cmd = SQLCMD.MakeUpdateCmdSentence_where_equals(sql.table, "barcode1", barcode1, "",

                      "barcode1", barcode1,
                      "Datetime", Dtime.Now(Dtime.StringType.ForDatum),
                      "Model", ModelNamelbl.Text,
                      "c19", data3,
                      "c20", data4,
                      "c21", data5,
                      "c22", data6,
                      "c23", data7


                      );

                        sql.ExecuteNonQuery(cmd);


                    }

                    this.Invoke(new dele(() =>
                    {
                        InputItem(dgvD1, barcode1 + " / 밸런스3 저장 [5]", data3);
                        Log_K.WriteLog(log_lst, Mainpath, "밸런스3 저장" + barcode1 + "/c19/" + data3 + "/c20/" + data4 + "/c21/" + data5 + "/c22/" + data6 + "/c23/" + data7);
                        int line = 3;

                        dgvD0.Rows[12].Cells[line].Value = data2;    //  바코드
                        dgvD0.Rows[13].Cells[line].Value = data3;    //  판정
                        dgvD0.Rows[14].Cells[line].Value = data4;    //  1차각도
                        dgvD0.Rows[15].Cells[line].Value = data5;    //  1차밸런스양
                        dgvD0.Rows[16].Cells[line].Value = data6;    //  2차각도
                        dgvD0.Rows[17].Cells[line].Value = data7;    //  2차밸런스양

                        GridMaster.Color_Painting(dgvD0, line);

                    }));
                }
                catch (Exception exc)
                {
                    InputItem(dgvD1, barcode1 + " / 밸런스3 저장 [5] - ERROR", "NG");
                    Log_K.WriteLog(log_lst, Mainpath, "밸런스3 저장 실패 - ERROR");
                }


                plc1.MCWrite(6060, 1);//저장했습니다.

            }

            if (name.Equals("ModelChange"))//데이터 보기
            {
                this.Invoke(new dele(() =>
                {
                    if (!checkBox1.Checked)
                    {
                        int ModelNumber = int.Parse(data.ToString());
                        ModelLoad(ModelNumber);
                    }
                }));

            }
        }

        private void plc2_TalkingComm(string name, object data, string data2, string data3, string data4, string data5, string data6, string data7, string data8, string data9)
        {
            if (name.Equals("CommData"))//데이터 보기
            {
                this.Invoke(new dele(() =>
                {
                    int[] CommData = (int[])data;

                    for (int i = 0; i < CommData.Length; i++)
                    {
                        dgvC1.Rows[i].Cells[2].Value = CommData[i];
                    }
                }));
            }

            if (name.Equals("BarcodeCheck"))//32개바이트
            {
                Delay(500);
                //#E30 로더 바코드 DATA 요구

                string barcode1 = data2;
                string res = "";

                try
                {
                    bool decision = BarcodeCheck(barcode1, "barcode1", 2);

                    if (decision)
                    {
                        plc2.MCWrite(8010, 1);
                        res = "OK";
                    }
                    else
                    {
                        plc2.MCWrite(8010, 2);
                        res = "NG";
                    }


                    this.Invoke(new dele(() =>
                    {
                        //InputItem( dgvD0 , barcode1 , "#E30 로더 바코드 DATA 요구" , "바코드체크" , res );
                        InputItem(dgvD1, barcode1 + " / #E30 로더 바코드 DATA 요구 [6]", res);
                        Log_K.WriteLog(log_lst, Mainpath, " / #E30 로더 바코드  : " + barcode1);
                    }));

                }
                catch (Exception)
                {
                    InputItem(dgvD1, barcode1 + " / #E30 로더 바코드 DATA 요구 [6] - ERROR", "NG");
                    Log_K.WriteLog(log_lst, Mainpath, " / #E30 로더 바코드  ERROR" + barcode1);
                }

            }

            if (name.Equals("Save1"))//32개바이트
            {
                Delay(500);
                string barcode1 = data2;
                
                try
                {
                    int rows = sql.ExecuteQuery_Select_Count("SELECT COUNT(*) FROM table1 WHERE `Barcode1`='" + barcode1 + "' ;");


                    if (rows == 0)
                    {
                        string cmd = Ken2.Database.SQLiteCMD_K.MakeInsertCmdSentence(sql.table,

                      "barcode1", barcode1,
                      "Datetime", Dtime.Now(Dtime.StringType.ForDatum),
                      "Model", ModelNamelbl.Text,

                      "c24", data3,
                      "c25", data4,
                      "c26", data5,
                      "c27", data6,
                      "c28", data7,
                      "c29", data8


                      );
                        sql.ExecuteNonQuery(cmd);


                    }
                    else
                    {
                        string cmd = SQLCMD.MakeUpdateCmdSentence_where_equals(sql.table, "barcode1", barcode1, "",
                      "Datetime", Dtime.Now(Dtime.StringType.ForDatum),
                      "Model", ModelNamelbl.Text,
                      "c24", data3,
                      "c25", data4,
                      "c26", data5,
                      "c27", data6,
                      "c28", data7,
                      "c29", data8

                        );
                        sql.ExecuteNonQuery(cmd);

                    }


                    this.Invoke(new dele(() =>
                    {
                        Log_K.WriteLog(log_lst, Mainpath, " / 특성데이터1 저장  : " + barcode1 + " / " + data3 + " / " + data4 + " / " + data5 + " / " + data6 + " / " + data7 + " / " + data8);
                        InputItem(dgvD1, barcode1 + " / 특성데이터1 저장 [7]", data3);

                        int line = 5;

                        dgvD0.Rows[0].Cells[line].Value = data2;    //  바코드
                        dgvD0.Rows[1].Cells[line].Value = data3;    //  특성 검사 저항 판정
                        dgvD0.Rows[2].Cells[line].Value = data4;    //  특성 저항 검사 측정값
                        dgvD0.Rows[3].Cells[line].Value = data5;    //  특성 검사 RPM 판정
                        dgvD0.Rows[4].Cells[line].Value = data6;   //  특성 검사 RPM 측정값
                        dgvD0.Rows[5].Cells[line].Value = data7;   //  특성 검사 전류 판정
                        dgvD0.Rows[6].Cells[line].Value = data8;   //  특성 검사 전류 측정값

                        GridMaster.Color_Painting(dgvD0, line);

                    }));


                }
                catch (Exception exc)
                {
                    InputItem(dgvD1, barcode1 + " / 특성데이터1 저장 [7] - ERROR", "NG");
                    Log_K.WriteLog(log_lst, Mainpath, " / 특성데이터1 저장  ERROR");
                }


                plc2.MCWrite(8020, 1);//저장했습니다.



                //this.Invoke( new dele( ( ) =>
                //{

                //    //dgvD0.Rows[ 1 ].Cells[ 2 ].Value = BCR;
                //    //dgvD0.Rows[ 2 ].Cells[ 1 ].Value = str1;
                //    //dgvD0.Rows[ 2 ].Cells[ 2 ].Value = str2;

                //    //UpdateGridColor( dgvD0 );

                //} ) );
            }

            if (name.Equals("Save2"))//32개바이트
            {
                Delay(500);
                string barcode1 = data2;


                try
                {
                    int rows = sql.ExecuteQuery_Select_Count("SELECT COUNT(*) FROM table1 WHERE `Barcode1`='" + barcode1 + "' ;");


                    if (rows == 0)
                    {
                        string cmd = Ken2.Database.SQLiteCMD_K.MakeInsertCmdSentence(sql.table,

                      "barcode1", barcode1,
                      "Datetime", Dtime.Now(Dtime.StringType.ForDatum),
                      "Model", ModelNamelbl.Text,

                      "c24", data3,
                      "c25", data4,
                      "c26", data5,
                      "c27", data6,
                      "c28", data7,
                      "c29", data8


                      );
                        sql.ExecuteNonQuery(cmd);


                    }
                    else
                    {
                        string cmd = SQLCMD.MakeUpdateCmdSentence_where_equals(sql.table, "barcode1", barcode1, "",
                      "Datetime", Dtime.Now(Dtime.StringType.ForDatum),
                      "Model", ModelNamelbl.Text,
                      "c24", data3,
                      "c25", data4,
                      "c26", data5,
                      "c27", data6,
                      "c28", data7,
                      "c29", data8

                        );
                        sql.ExecuteNonQuery(cmd);

                    }


                    this.Invoke(new dele(() =>
                    {
                        Log_K.WriteLog(log_lst, Mainpath, " / 특성데이터2 저장  : " + barcode1 + " / " + data3 + " / " + data4 + " / " + data5 + " / " + data6 + " / " + data7 + " / " + data8);
                        InputItem(dgvD1, barcode1 + " / 특성데이터2 저장 [7]", data3);

                        int line = 5;

                        dgvD0.Rows[7].Cells[line].Value = data2;   //  바코드
                        dgvD0.Rows[8].Cells[line].Value = data3;   //  특성 검사 저항 판정
                        dgvD0.Rows[9].Cells[line].Value = data4;   //  특성 저항 검사 측정값
                        dgvD0.Rows[10].Cells[line].Value = data5;   //  특성 검사 RPM 판정
                        dgvD0.Rows[11].Cells[line].Value = data6;   //  특성 검사 RPM 측정값
                        dgvD0.Rows[12].Cells[line].Value = data7;   //  특성 검사 전류 판정
                        dgvD0.Rows[13].Cells[line].Value = data8;   //  특성 검사 전류 측정값

                        GridMaster.Color_Painting(dgvD0, line);

                    }));


                }
                catch (Exception exc)
                {
                    InputItem(dgvD1, barcode1 + " / 특성데이터2 저장 [7] - ERROR", "NG");
                    Log_K.WriteLog(log_lst, Mainpath, " / 특성데이터2 저장 ERROR");
                }


                plc2.MCWrite(8030, 1);//저장했습니다.


            }

            if (name.Equals("Save3"))//32개바이트
            {
                Delay(500);
                string barcode1 = data2;


                try
                {
                    int rows = sql.ExecuteQuery_Select_Count("SELECT COUNT(*) FROM table1 WHERE `Barcode1`='" + barcode1 + "' ;");


                    if (rows == 0)
                    {
                        string cmd = Ken2.Database.SQLiteCMD_K.MakeInsertCmdSentence(sql.table,

                      "barcode1", barcode1,
                      "Datetime", Dtime.Now(Dtime.StringType.ForDatum),
                      "Model", ModelNamelbl.Text,

                      "c24", data3,
                      "c25", data4,
                      "c26", data5,
                      "c27", data6,
                      "c28", data7,
                      "c29", data8


                      );
                        sql.ExecuteNonQuery(cmd);


                    }
                    else
                    {
                        string cmd = SQLCMD.MakeUpdateCmdSentence_where_equals(sql.table, "barcode1", barcode1, "",
                      "Datetime", Dtime.Now(Dtime.StringType.ForDatum),
                      "Model", ModelNamelbl.Text,
                      "c24", data3,
                      "c25", data4,
                      "c26", data5,
                      "c27", data6,
                      "c28", data7,
                      "c29", data8

                        );
                        sql.ExecuteNonQuery(cmd);

                    }


                    this.Invoke(new dele(() =>
                    {
                        Log_K.WriteLog(log_lst, Mainpath, " / 특성데이터3 저장  : " + barcode1 + " / " + data3 + " / " + data4 + " / " + data5 + " / " + data6 + " / " + data7 + " / " + data8);
                        InputItem(dgvD1, barcode1 + " / 특성데이터3 저장 [7]", data3);

                        int line = 5;

                        dgvD0.Rows[14].Cells[line].Value = data2;    //  바코드
                        dgvD0.Rows[15].Cells[line].Value = data3;    //  특성 검사 저항 판정
                        dgvD0.Rows[16].Cells[line].Value = data4;    //  특성 저항 검사 측정값
                        dgvD0.Rows[17].Cells[line].Value = data5;    //  특성 검사 RPM 판정
                        dgvD0.Rows[18].Cells[line].Value = data6;    //  특성 검사 RPM 측정값
                        dgvD0.Rows[19].Cells[line].Value = data7;    //  특성 검사 전류 판정
                        dgvD0.Rows[20].Cells[line].Value = data8;    //  특성 검사 전류 측정값

                        GridMaster.Color_Painting(dgvD0, line);

                    }));


                }
                catch (Exception exc)
                {
                    InputItem(dgvD1, barcode1 + " / 특성데이터3 저장 [7] - ERROR", "NG");
                    Log_K.WriteLog(log_lst, Mainpath, " / 특성데이터13 저장 ERROR");

                }




                plc2.MCWrite(8040, 1);//저장했습니다.


            }

            if (name.Equals("Save4"))//32개바이트
            {
                Delay(500);
                string barcode1 = data2;


                try
                {
                    int rows = sql.ExecuteQuery_Select_Count("SELECT COUNT(*) FROM table1 WHERE `Barcode1`='" + barcode1 + "' ;");


                    if (rows == 0)
                    {
                        string cmd = Ken2.Database.SQLiteCMD_K.MakeInsertCmdSentence(sql.table,

                      "barcode1", barcode1,
                      "Datetime", Dtime.Now(Dtime.StringType.ForDatum),
                      "Model", ModelNamelbl.Text,

                      "c24", data3,
                      "c25", data4,
                      "c26", data5,
                      "c27", data6,
                      "c28", data7,
                      "c29", data8


                      );
                        sql.ExecuteNonQuery(cmd);


                    }
                    else
                    {
                        string cmd = SQLCMD.MakeUpdateCmdSentence_where_equals(sql.table, "barcode1", barcode1, "",
                      "Datetime", Dtime.Now(Dtime.StringType.ForDatum),
                      "Model", ModelNamelbl.Text,
                      "c24", data3,
                      "c25", data4,
                      "c26", data5,
                      "c27", data6,
                      "c28", data7,
                      "c29", data8

                        );
                        sql.ExecuteNonQuery(cmd);

                    }


                    this.Invoke(new dele(() =>
                    {
                        InputItem(dgvD1, barcode1 + " / 특성데이터4 저장 [7]", data3);
                        Log_K.WriteLog(log_lst, Mainpath, " / 특성데이터4 저장  : " + barcode1 + " / " + data3 + " / " + data4 + " / " + data5 + " / " + data6 + " / " + data7 + " / " + data8);

                        int line = 5;

                        dgvD0.Rows[21].Cells[line].Value = data2;    //  바코드
                        dgvD0.Rows[22].Cells[line].Value = data3;    //  특성 검사 저항 판정
                        dgvD0.Rows[23].Cells[line].Value = data4;    //  특성 저항 검사 측정값
                        dgvD0.Rows[24].Cells[line].Value = data5;   //  특성 검사 RPM 판정
                        dgvD0.Rows[25].Cells[line].Value = data6;   //  특성 검사 RPM 측정값
                        dgvD0.Rows[26].Cells[line].Value = data7;   //  특성 검사 전류 판정
                        dgvD0.Rows[27].Cells[line].Value = data8;   //  특성 검사 전류 측정값

                        GridMaster.Color_Painting(dgvD0, line);

                    }));


                }
                catch (Exception exc)
                {
                    InputItem(dgvD1, barcode1 + " / 특성데이터4 저장 [7] - ERROR", "NG");
                    Log_K.WriteLog(log_lst, Mainpath, " / 특성데이터4 저장 ERROR");
                }

                plc2.MCWrite(8050, 1);//저장했습니다.


            }

            if (name.Equals("BarcodeCheck2"))//32개바이트
            {

                //#G11 컨베이어 바코드 DATA 요구
                Delay(500);

                string barcode1 = data2;
                string res = "";

                try
                {
                    bool decision = BarcodeCheck(barcode1, "barcode1", 5);

                    if (decision)
                    {
                        plc2.MCWrite(8060, 1);
                        res = "OK";
                    }
                    else
                    {
                        plc2.MCWrite(8060, 2);
                        res = "NG";
                    }


                    this.Invoke(new dele(() =>
                    {
                        InputItem(dgvD1, barcode1 + " / #G11 컨베이어 바코드 DATA 요구 [8]", res);
                        Log_K.WriteLog(log_lst, Mainpath, " / #G11 컨베이어 바코드  : " + barcode1);
                    }));

                }
                catch (Exception)
                {
                    InputItem(dgvD1, barcode1 + " / #G11 컨베이어 바코드 DATA 요구 [8] - ERROR", "NG");
                    Log_K.WriteLog(log_lst, Mainpath, " / #G11 컨베이어 바코드 ERROR");
                }



            }

            //성능
            if (name.Equals("Save5"))//32개바이트
            {
                Delay(500);
                string barcode1 = data2;


                try
                {
                    int rows = sql.ExecuteQuery_Select_Count("SELECT COUNT(*) FROM table1 WHERE `Barcode1`='" + barcode1 + "' ;");


                    if (rows == 0)
                    {
                        string cmd = Ken2.Database.SQLiteCMD_K.MakeInsertCmdSentence(sql.table,

                      "barcode1", barcode1,
                      "Datetime", Dtime.Now(Dtime.StringType.ForDatum),
                      "Model", ModelNamelbl.Text,

                      "c30", data3,
                      "c31", data4,
                      "c32", data5,
                      "c33", data6

                      );
                        sql.ExecuteNonQuery(cmd);



                    }
                    else
                    {
                        string cmd = SQLCMD.MakeUpdateCmdSentence_where_equals(sql.table, "barcode1", barcode1, "",
                      "Datetime", Dtime.Now(Dtime.StringType.ForDatum),
                      "Model", ModelNamelbl.Text,

                      "c30", data3,
                      "c31", data4,
                      "c32", data5,
                      "c33", data6

                        );
                        sql.ExecuteNonQuery(cmd);


                    }

                    this.Invoke(new dele(() =>
                    {
                        InputItem(dgvD1, barcode1 + " / 성능데이터1 저장 [9]", data3);
                        Log_K.WriteLog(log_lst, Mainpath, " / 성능데이터1 저장  : " + barcode1 + " / " + data3 + " / " + data4 + " / " + data5 + " / " + data6);
                        int line = 7;

                        dgvD0.Rows[0].Cells[line].Value = data2;   //  바코드
                        dgvD0.Rows[1].Cells[line].Value = data3;   //  성능 검사 판정
                        dgvD0.Rows[2].Cells[line].Value = data4;   //  성능 검사 RPM 측정값
                        dgvD0.Rows[3].Cells[line].Value = data5;   //  성능 검사 소음 측정값
                        dgvD0.Rows[4].Cells[line].Value = data6;   //  성능 검사 진동 측정값

                        GridMaster.Color_Painting(dgvD0, line);

                    }));


                }
                catch (Exception exc)
                {
                    InputItem(dgvD1, barcode1 + " / 성능데이터1 저장 [9] - ERROR", "NG");
                    Log_K.WriteLog(log_lst, Mainpath, " / 성능데이터1 저장 ERROR");
                }

                plc2.MCWrite(8070, 1);//저장했습니다.

            }

            if (name.Equals("BarcodeCheck3"))//32개바이트
            {
                Delay(500);
                //#G12 컨베이어 바코드 DATA 요구

                string barcode1 = data2;
                string res = "";

                try
                {
                    bool decision = BarcodeCheck(barcode1, "barcode1", 5);

                    if (decision)
                    {
                        plc2.MCWrite(8080, 1);
                        res = "OK";
                    }
                    else
                    {
                        plc2.MCWrite(8080, 2);
                        res = "NG";
                    }


                    this.Invoke(new dele(() =>
                    {
                        InputItem(dgvD1, barcode1 + " / #G12 컨베이어 바코드 DATA 요구 [8]", res);
                        Log_K.WriteLog(log_lst, Mainpath, " / #G12 컨베이어 바코드  : " + barcode1);

                    }));

                }
                catch (Exception)
                {
                    InputItem(dgvD1, barcode1 + " / #G12 컨베이어 바코드 DATA 요구 [8] - ERROR", "NG");
                    Log_K.WriteLog(log_lst, Mainpath, " / #G12 컨베이어 바코드 ERROR");
                }

            }

            if (name.Equals("Save6"))//32개바이트
            {
                Delay(500);
                string barcode1 = data2;


                try
                {
                    int rows = sql.ExecuteQuery_Select_Count("SELECT COUNT(*) FROM table1 WHERE `Barcode1`='" + barcode1 + "' ;");


                    if (rows == 0)
                    {
                        string cmd = Ken2.Database.SQLiteCMD_K.MakeInsertCmdSentence(sql.table,

                      "barcode1", barcode1,
                      "Datetime", Dtime.Now(Dtime.StringType.ForDatum),
                      "Model", ModelNamelbl.Text,

                      "c30", data3,
                      "c31", data4,
                      "c32", data5,
                      "c33", data6

                      );
                        sql.ExecuteNonQuery(cmd);



                    }
                    else
                    {
                        string cmd = SQLCMD.MakeUpdateCmdSentence_where_equals(sql.table, "barcode1", barcode1, "",
                      "Datetime", Dtime.Now(Dtime.StringType.ForDatum),
                      "Model", ModelNamelbl.Text,

                      "c30", data3,
                      "c31", data4,
                      "c32", data5,
                      "c33", data6

                        );
                        sql.ExecuteNonQuery(cmd);


                    }

                    this.Invoke(new dele(() =>
                    {
                        InputItem(dgvD1, barcode1 + " / 성능데이터2 저장 [9]", data3);
                        Log_K.WriteLog(log_lst, Mainpath, " / 성능데이터2 저장  : " + barcode1 + " / " + data3 + " / " + data4 + " / " + data5 + " / " + data6);
                        int line = 7;

                        dgvD0.Rows[5].Cells[line].Value = data2;    //  바코드
                        dgvD0.Rows[6].Cells[line].Value = data3;    //  성능 검사 판정
                        dgvD0.Rows[7].Cells[line].Value = data4;    //  성능 검사 RPM 측정값
                        dgvD0.Rows[8].Cells[line].Value = data5;    //  성능 검사 소음 측정값
                        dgvD0.Rows[9].Cells[line].Value = data6;    //  성능 검사 진동 측정값

                        GridMaster.Color_Painting(dgvD0, line);

                    }));


                }
                catch (Exception exc)
                {
                    InputItem(dgvD1, barcode1 + " / 성능데이터2 저장 [9] - ERROR", "NG");
                    Log_K.WriteLog(log_lst, Mainpath, " / 성능데이터2 저장 ERROR");
                }

                plc2.MCWrite(8090, 1);//저장했습니다.

            }

            if (name.Equals("BarcodeCheck4"))//32개바이트
            {
                Delay(500);
                //#I60 완성 로더 바코드 DATA 요구                최종판정바코드

                if (!LastCheck.Checked)
                {
                    string barcode1 = data2;
                    string res = "";

                    try
                    {
                        bool decision = BarcodeCheck(barcode1, "barcode1", 6);
                        if (decision)
                        {
                            plc2.MCWrite(8100, 1);
                            res = "OK";
                        }
                        else
                        {
                            plc2.MCWrite(8100, 2);
                            res = "NG";

                        }

                    }
                    catch (Exception)
                    {

                    }


                    //---------------↓ 최종판정 DB에저장 ↓---------------┐

                    try
                    {
                        int rows = sql.ExecuteQuery_Select_Count("SELECT COUNT(*) FROM table1 WHERE `Barcode1`='" + barcode1 + "' ;");

                        if (rows != 0)
                        {
                            string cmd = SQLCMD.MakeUpdateCmdSentence_where_equals(sql.table, "barcode1", barcode1, "",
                              "Decision", res

                            );
                            sql.ExecuteNonQuery(cmd);

                            this.Invoke(new dele(() =>
                            {
                                InputItem(dgvD1, barcode1 + " / #I60 완성 로더 바코드 DATA 요구 [10]", res);
                                InputItem(dgvD1, barcode1 + " / 최종판정 DB에 저장");
                                Log_K.WriteLog(log_lst, Mainpath, " / 최종판정 DB에 저장  : " + barcode1 + " / " + res);
                            }));
                        }

                    }
                    catch (Exception)
                    {
                        this.Invoke(new dele(() =>
                        {
                            InputItem(dgvD1, barcode1 + " / #I60 완성 로더 바코드 DATA 요구 [10] - ERROR", "NG");
                            Log_K.WriteLog(log_lst, Mainpath, " / #I60 완성 로더 바코드 DATA 요구 [10] - ERROR");
                        }));
                    }

                    //---------------↑ 최종판정 DB에저장 ↑---------------┘
                }

                else
                {
                    string barcode1 = data2;
                    string res = "";

                    try
                    {
                        //bool decision = BarcodeCheck( barcode1, "barcode1", 6 );
                        //if ( decision )
                        //{
                        plc2.MCWrite(8100, 1);
                        res = "OK";
                        //}
                        //else
                        //{
                        //    plc2.MCWrite( 8100, 1 );
                        //    res = "OK";

                        //}

                    }
                    catch (Exception)
                    {

                    }


                    //---------------↓ 최종판정 DB에저장 ↓---------------┐

                    try
                    {
                        int rows = sql.ExecuteQuery_Select_Count("SELECT COUNT(*) FROM table1 WHERE `Barcode1`='" + barcode1 + "' ;");

                        if (rows != 0)
                        {
                            string cmd = SQLCMD.MakeUpdateCmdSentence_where_equals(sql.table, "barcode1", barcode1, "",
                              "Decision", res

                            );
                            sql.ExecuteNonQuery(cmd);

                            this.Invoke(new dele(() =>
                            {
                                InputItem(dgvD1, barcode1 + " / #I60 완성 로더 바코드 DATA 요구 [10]-리워크모드", res);
                                InputItem(dgvD1, barcode1 + " / 최종판정 OK-리워크모드");
                                Log_K.WriteLog(log_lst, Mainpath, " / 최종판정 OK-리워크모드  : " + barcode1 + " / " + res);
                            }));
                        }

                    }
                    catch (Exception)
                    {
                        this.Invoke(new dele(() =>
                        {
                            InputItem(dgvD1, barcode1 + " / #I60 완성 로더 바코드 DATA 요구 [10]-리워크모드 - ERROR", "NG");
                            Log_K.WriteLog(log_lst, Mainpath, " /  #I60 완성 로더 바코드 DATA 요구 [10]-리워크모드 - ERROR");
                        }));
                    }

                    //---------------↑ 최종판정 DB에저장 ↑---------------┘
                }


            }

            if (name.Equals("ModelChange"))//데이터 보기
            {
                this.Invoke(new dele(() =>
                {
                    if (!checkBox1.Checked)
                    {
                        int ModelNumber = int.Parse(data.ToString());
                        ModelLoad1(ModelNumber);
                    }
                }));

            }
        }

        private void handyconv_TalkingComm(string name, object data)    //  불량 테스트 핸디
        {
            string bcr = data.ToString();

            Reading_barcode_NG_View_Handy(bcr);
        }

        void Reading_barcode_NG_View_Handy(string bcr) // ######################### 불량 테스트 핸디
        {
            try
            {
                if (bcr.Length == 9)//브라켓 현대바코드
                {
                    this.Invoke(new dele(() =>
                    {
                        SelectHistory("barcode4", bcr);

                    }));
                }
                else if (bcr.Length > 10)//그외 바코드
                {
                    this.Invoke(new dele(() =>
                    {
                        SelectHistory("barcode1", bcr);

                    }));
                }
                else
                    return;
                

                this.Invoke(new dele(() =>
                {
                    dgvES[0].Rows.Clear();
                    dgvES[1].Rows.Clear();
                    dgvInit("dgvH0");

                    xtraTabControl1.SelectedTabPageIndex = 11;
                    es_barcode.Text = bcr;

                    int colcnt = dgvH0.Columns.Count;

                    for (int i = 0; i < colcnt; i++)
                    {
                        if (i < 20)
                            InputItem(dgvES[0], dgvH0.Columns[i].HeaderText, dgvH0.Rows[0].Cells[i].Value.ToString());
                        else
                            InputItem(dgvES[1], dgvH0.Columns[i].HeaderText, dgvH0.Rows[0].Cells[i].Value.ToString());

                    }

                    dgvH0.Rows.Clear();
                    dgvES[0].CurrentCell = null;
                    dgvES[1].CurrentCell = null;
                }));


            }
            catch (Exception)
            {

            }
        }


        #region -----# MainThread #-----

        private Thread MainThread;
        bool MainThreadFlag = false;

        CountPlay desti_cnt = new CountPlay();

        //ttttttttttttttttttttttttttttttttttttttttttttt
        private void MainThreadMethod(object param)
        {
            int para = (int)param;
            while (MainThreadFlag)
            {
                Thread.Sleep(500);

                try
                {
                    if (plc1.Connected)
                        dgvD2.Rows[0].Cells[0].Style.BackColor = Color.Lime;
                    else
                        dgvD2.Rows[0].Cells[0].Style.BackColor = Color.Red;

                    if (plc2.Connected)
                        dgvD2.Rows[1].Cells[0].Style.BackColor = Color.Lime;
                    else
                        dgvD2.Rows[1].Cells[0].Style.BackColor = Color.Red;

                    //if (plc3.Connected)
                    //    dgvD2.Rows[2].Cells[0].Style.BackColor = Color.Lime;
                    //else
                    //    dgvD2.Rows[2].Cells[0].Style.BackColor = Color.Red;

                    
                    if (LabelPrinter.Connected)
                        dgvD2.Rows[2].Cells[0].Style.BackColor = Color.Lime;
                    else
                        dgvD2.Rows[2].Cells[0].Style.BackColor = Color.Red;


                    //if (LabelPrinter2.Connected)
                    //    dgvD2.Rows[4].Cells[0].Style.BackColor = Color.Lime;
                    //else
                    //    dgvD2.Rows[4].Cells[0].Style.BackColor = Color.Red;
                    

                    if (confirmPrintData())
                        dgvD2.Rows[3].Cells[0].Style.BackColor = Color.Lime;
                    else
                        dgvD2.Rows[3].Cells[0].Style.BackColor = Color.Red;

                    //if (confirmPrintData2())
                    //    dgvD2.Rows[6].Cells[0].Style.BackColor = Color.Lime;
                    //else
                    //    dgvD2.Rows[6].Cells[0].Style.BackColor = Color.Red;




                    //if (handy.Connected)
                    //    dgvD2.Rows[7].Cells[0].Style.BackColor = Color.Lime;
                    //else
                    //    dgvD2.Rows[7].Cells[0].Style.BackColor = Color.Red;


                    if (monitor_pc.Connected)
                        dgvD2.Rows[4].Cells[0].Style.BackColor = Color.Lime;
                    else
                        dgvD2.Rows[4].Cells[0].Style.BackColor = Color.Red;




                    //if ( Status1( ) )
                    //    IsRunningMode1 = true;
                    //else
                    //    IsRunningMode1 = false;

                    //if ( Status( ) )
                    //    AllReady = true;
                    //else
                    //    AllReady = false;




                    //delegate 폼 컨트롤에 표시할때 주의
                    this.Invoke(new dele(() =>
                    {
                        MainView();

                        //if ( xtraTabControl1.SelectedTabPageIndex == 8 )
                        //    Desti_cnt_view( );

                    }));
                    //delegate
                }
                catch (Exception)
                {

                }

            }
        }

        void Desti_cnt_view()
        {


#if Release


            //dgvS0.Rows[ 1 ].Cells[ 1 ].Value = TotalLabel1.Text;
            //dgvS0.Rows[ 2 ].Cells[ 1 ].Value = OKLabel1.Text;
            //dgvS0.Rows[ 3 ].Cells[ 1 ].Value = NGLabel1.Text;

            //dgvS0.Rows[ 1 ].Cells[ 2 ].Value = TotalLabel0.Text;
            //dgvS0.Rows[ 2 ].Cells[ 2 ].Value = OKLabel0.Text;
            //dgvS0.Rows[ 3 ].Cells[ 2 ].Value = NGLabel0.Text;

            //dgvS0.Rows[ 1 ].Cells[ 3 ].Value = Convert.ToInt16( balance0_lbl.Text ) + Convert.ToInt16( balance1_lbl.Text );
            //dgvS0.Rows[ 2 ].Cells[ 3 ].Value = dgvS0.Rows[ 1 ].Cells[ 3 ].Value;
            //dgvS0.Rows[ 3 ].Cells[ 3 ].Value = 0;


#endif



            if (desti_cnt.OnePlay(6))
            {
                //dgvS0.Rows[ 5 ].Cells[ 1 ].Value = percent( 1 ) + "%";
                //dgvS0.Rows[ 5 ].Cells[ 2 ].Value = percent( 2 ) + "%";
                //dgvS0.Rows[ 5 ].Cells[ 3 ].Value = percent( 3 ) + "%";
            }

        }

        int percent(int col)
        {
            int result = 0;

            //try
            //{
            //    //double desti = Convert.ToDouble( dgvS0.Rows[ 4 ].Cells[ col ].Value );
            //    //double ok = Convert.ToDouble( dgvS0.Rows[ 2 ].Cells[ col ].Value );
            //    //result = ( int ) ( ok / desti * 100 );
            //}
            //catch ( Exception )
            //{
            //    result = 0;
            //}

            return result;
        }


        void MainView()
        {
            try
            {
                //if ( plc1.Server_Connected )
                //    KenCon.OK( StatusLbl0 );
                //else
                //    KenCon.Twinkle( StatusLbl0 );

                //if ( plc2.Server_Connected )
                //    KenCon.OK( StatusLbl1 );
                //else
                //    KenCon.Twinkle( StatusLbl1 );

                //if ( confirmPrintData( ) )
                //{
                //    KenCon.OK( StatusLbl3 );
                //    Groupcon.ChangeColor( projectgroup , Color.Black );
                //}
                //else
                //{
                //    KenCon.Twinkle( StatusLbl3 );
                //    Groupcon.Twinkle( projectgroup );
                //}

                //if ( print.Server_Connected )
                //    KenCon.OK( StatusLbl2 );
                //else
                //    KenCon.Twinkle( StatusLbl2 );

                //if ( DmFix[ 0 ].Connected )
                //    KenCon.OK( StatusLbl4 );
                //else
                //    KenCon.Twinkle( StatusLbl4 );

                //if ( DmFix[ 1 ].Connected )
                //    KenCon.OK( StatusLbl5 );
                //else
                //    KenCon.Twinkle( StatusLbl5 );

                //if ( DmHandyConv[ 0 ].Connected )
                //    KenCon.OK( StatusLbl6 );
                //else
                //    KenCon.Twinkle( StatusLbl6 );

                //if ( DmHandyConv[ 1 ].Connected )
                //    KenCon.OK( bal_handy1_sta );
                //else
                //    KenCon.Twinkle( bal_handy1_sta );

                //if ( DmHandyConv[ 2 ].Connected )
                //    KenCon.OK( bal_handy2_sta );
                //else
                //    KenCon.Twinkle( bal_handy2_sta );

                //if ( print.print_ok )
                //    KenCon.OK( StatusLbl8 );
                //else
                //    KenCon.Twinkle( StatusLbl8 );

                //if ( inkjet.print_ok )
                //    KenCon.OK( StatusLbl9 );
                //else
                //    KenCon.Twinkle( StatusLbl9 );

                //if ( inkjet.Server_Connected )
                //    KenCon.OK( StatusLbl10 );
                //else
                //    KenCon.Twinkle( StatusLbl10 );




            }
            catch (Exception)
            {


            }

            //label4.Text = Dtime.Now( Dtime.StringType.CurrentTime );
        }

        //스레드함수
        public void MainThreadStart(int param)
        {
            //스레드스타트
            MainThreadFlag = true;
            MainThread = new Thread((new ParameterizedThreadStart(MainThreadMethod)));
            MainThread.Start(param);
            //스레드스타트
        }
        public void MainThreadStop()
        {
            //스레드종료
            MainThreadFlag = false;
            //스레드종료
        }
        #endregion


        #region 이력조회 관련
        /// <summary>
        /// 바코드종류 , 바코드데이터
        /// </summary>
        /// <param name="barcode_dis"></param>
        /// <param name="barcode_condition"></param>
        void SelectHistory(string barcode_dis, string barcode_condition)
        {
            dgvH0.Columns.Clear();

            dgvHN0.Rows[0].Cells[0].Value = "";
            dgvHN0.Rows[0].Cells[1].Value = "";
            dgvHN0.Rows[0].Cells[2].Value = "";
            dgvHN0.Rows[0].Cells[3].Value = "";

            string cmd = SQLiteCMD_K.Select_Equal("table1", barcode_dis, barcode_condition,

                   "barcode1",
                   "barcode2",
                   "barcode3",

                 "Datetime",
                 "Model",

                 "c1",

                 "c14",
                 "c15",
                 "c16",
                 "c17",
                 "c18",
                 "c180",

                 "c19",
                 "c20",
                 "c21",
                 "c22",
                 "c23",
                 "c24",
                 "c25",
                 "c26",
                 "c27",
                 "c28",
                 "c29",
                 "c30",

                 "c31",
                 "c32",
                 "c33",
                 //"c34",
                 //"c35",
                 //"c36",
                 //"c37",
                 //"c38",
                 //"c39",
                 //"c40",
                 //"c41",
                 //"c42",
                 //"c43",

                 //"barcode4",

                 "Decision"

                       );

            sql.Select(dgvH0, cmd, false);
        }

        void SelectHistory()
        {
            dgvH0.Columns.Clear();


            //특정 바코드 검색시
            if (NameSearchcheck.Checked)
            {
                string selected_bcr = "";

                if (radio_bcr1.Checked)
                    selected_bcr = "barcode1";
                else if (radio_bcr2.Checked)
                    selected_bcr = "barcode2";
                else if (radio_bcr3.Checked)
                    selected_bcr = "barcode3";
                //else if (radio_bcr4.Checked)
                //    selected_bcr = "barcode4";


                string cmd = SQLiteCMD_K.Select_Equal("table1", selected_bcr, NameSearchTB.Text,

                    "barcode1",
                    "barcode2",
                    "barcode3",

                  "Datetime",
                                  "Model",

                 "c1",
                 "c14",
                 "c15",
                 "c16",
                 "c17",
                 "c18",
                 "c180",

                 "c19",
                 "c20",
                 "c21",
                 "c22",
                 "c23",
                 "c24",
                 "c25",
                 "c26",
                 "c27",
                 "c28",
                 "c29",
                 "c30",

                 "c31",
                 "c32",
                 "c33",
                 //"c34",
                 //"c35",
                 //"c36",
                 //"c37",
                 //"c38",
                 //"c39",
                 //"c40",
                 //"c41",
                 //"c42",
                 //"c43",

                 //"barcode4",

                 "Decision"

                        );

                sql.Select(dgvH0, cmd, false);
            }
            else//기간검색시
            {
                string cmd = SQLiteCMD_K.Select_Datetime("table1", "Datetime", Dtime.GetDateTime_string(Date0, Time0), Dtime.GetDateTime_string(Date1, Time1), "",

                    "barcode1",
                    "barcode2",
                    "barcode3",

                  "Datetime",
                                  "Model",

                 "c1",

                 "c14",
                 "c15",
                 "c16",
                 "c17",
                 "c18",
                 "c180",

                 "c19",
                 "c20",
                 "c21",
                 "c22",
                 "c23",
                 "c24",
                 "c25",
                 "c26",
                 "c27",
                 "c28",
                 "c29",
                 "c30",

                 "c31",
                 "c32",
                 "c33",
                 //"c34",
                 //"c35",
                 //"c36",
                 //"c37",
                 //"c38",
                 //"c39",
                 //"c40",
                 //"c41",
                 //"c42",
                 //"c43",

                 //"barcode4",

                 "Decision"

                  );

                sql.Select(dgvH0, cmd, false);

            }

            dgvInit("dgvH0");


            //---------------↓ 수량 ↓---------------┐
            if (NameSearchcheck.Checked == false)
            {
                int allcnt = dgvH0.Rows.Count;
                int okcnt = 0;
                int ngcnt = 0;

                for (int i = 0; i < allcnt; i++)
                {
                    if (dgvH0.Rows[i].Cells[27].Value.Equals("OK"))
                        okcnt++;
                    if (dgvH0.Rows[i].Cells[27].Value.Equals("NG"))
                        ngcnt++;
                }

                int percent = (int)(((double)okcnt / (okcnt + ngcnt)) * 100);

                dgvHN0.Rows[0].Cells[0].Value = (okcnt + ngcnt);
                dgvHN0.Rows[0].Cells[1].Value = okcnt;
                dgvHN0.Rows[0].Cells[2].Value = ngcnt;

                if (allcnt != 0)
                    dgvHN0.Rows[0].Cells[3].Value = percent + "%";
                else
                    dgvHN0.Rows[0].Cells[3].Value = "";
            }
            else
            {
                dgvHN0.Rows[0].Cells[0].Value = "";
                dgvHN0.Rows[0].Cells[1].Value = "";
                dgvHN0.Rows[0].Cells[2].Value = "";
                dgvHN0.Rows[0].Cells[3].Value = "";
            }
            //---------------↑ 수량 ↑---------------┘
        } 
        #endregion


        #region 모델 관련
        string cnt = "A0A";
        bool Day_Time_Working = true;

        void DayNightLoad()
        {
            //---------------↓ 주야정보 및 카운트 로드 ↓---------------┐
            cnt = RWdataFast.Load("cnt", "A0A");
            //LastKBcnt = cnt;
            Day_Time_Working = bool.Parse(RWdataFast.Load("Day_Time_Working", "TRUE"));
            dgvP0.Rows[0].Cells[1].Value = Kabul_label_data.GetDTinfo(Day_Time_Working);
            dgvP0.Rows[1].Cells[1].Value = cnt;
            dgvP0.Rows[2].Cells[1].Value = KB_count.ViewCnt_KB(cnt);


            //---------------↑ 주야정보 및 카운트 로드 ↑---------------┘
        }

        void ModelLoad(int num)//impeller
        {
            try
            {
                CurrentModelNum = num;
                ModelNamelbl.Text = dgvM0.Rows[num - 1].Cells[1].Value.ToString();


                dgvP0.Rows[5].Cells[1].Value = RWdataFast.Load_DBfolder(0, CurrentModelNum, "cap1");
                dgvP0.Rows[6].Cells[1].Value = RWdataFast.Load_DBfolder(0, CurrentModelNum, "cap2");
                dgvP0.Rows[7].Cells[1].Value = RWdataFast.Load_DBfolder(0, CurrentModelNum, "cap3");
                dgvP0.Rows[8].Cells[1].Value = RWdataFast.Load_DBfolder(0, CurrentModelNum, "cap4");

                dgvP0.Rows[3].Cells[1].Value = RWdataFast.Load_DBfolder(0, CurrentModelNum, "d3");
                dgvP0.Rows[4].Cells[1].Value = RWdataFast.Load_DBfolder(0, CurrentModelNum, "d4");

                dgvP0.Rows[10].Cells[1].Value = RWdataFast.Load_DBfolder(0, CurrentModelNum, "ccode");

                DayNightLoad();

            }
            catch (Exception)
            {


            }
        }

        void ModelLoad1(int num)//upper
        {
            try
            {
                CurrentModelNum1 = num;
                ModelNamelbl1.Text = dgvM1.Rows[num - 1].Cells[1].Value.ToString();

            }
            catch (Exception)
            {


            }
        }

        private void simpleButton12_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvM0.CurrentCell == null)
                {
                    return;
                }

                if (textBox4.Text.Equals(""))
                {
                    MessageBox.Show("모델 이름을 적어주세요", "Error");
                    return;
                }

                if (POPUP.YesOrNo("INFO", "모델 이름을 변경할까요?"))
                {
                 dgvM0.Rows[dgvM0.CurrentCell.RowIndex].Cells[1].Value = textBox4.Text;

                    GridMaster.SaveCSV_OnlyData(dgvM0, System.Windows.Forms.Application.StartupPath + "\\Model1.csv");//셀데이터로드

                    textBox4.Text = "";
                    MessageBox.Show("변경 완료", "Messagebox");
                }
            }
            catch (Exception)
            {

            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvM1.CurrentCell == null)
                {
                    return;
                }

                if (textBox5.Text.Equals(""))
                {
                    MessageBox.Show("모델 이름을 적어주세요", "Error");

                    return;
                }

                if (POPUP.YesOrNo("INFO", "모델 이름을 변경할까요?"))
                {
                    dgvM1.Rows[dgvM1.CurrentCell.RowIndex].Cells[1].Value = textBox5.Text;

                    GridMaster.SaveCSV_OnlyData(dgvM1, System.Windows.Forms.Application.StartupPath + "\\Model2.csv");//셀데이터로드


                    textBox5.Text = "";
                    MessageBox.Show("변경 완료", "Messagebox");
                }
            }
            catch (Exception)
            {

            }
        }
        #endregion
        
        #region 프린터 관련
        public string PrintOne()
        {
            //---------------↓ 바코드종류설정 ↓---------------┐
            int dis = 0;

            if (dgvP0.Rows[8].Cells[1].Value == null)
                dis = 1;
            else if (dgvP0.Rows[8].Cells[1].Value.ToString().Length == 0)
                dis = 1;
            else
                dis = 2;
            //---------------↑ 바코드종류설정 ↑---------------┘



            if (dis == 1)//현대 바코드 ㅡ> TM 
            {

                int year = int.Parse(dgvP0.Rows[9].Cells[1].Value.ToString().Split('-')[0]);
                int month = int.Parse(dgvP0.Rows[9].Cells[1].Value.ToString().Split('-')[1]);
                int day = int.Parse(dgvP0.Rows[9].Cells[1].Value.ToString().Split('-')[2]);
                //, year,month,day

                //모델 앞 두자리만 가져와라.. (TM0) -> (TM)
                
                string barcode = Kabul_label_data.GetCode(ModelNamelbl.Text.Substring(0, 2), dgvP0.Rows[3].Cells[1].Value.ToString(), dgvP0.Rows[4].Cells[1].Value.ToString(), this.Day_Time_Working, cnt, year, month, day, dgvP0.Rows[10].Cells[1].Value.ToString());

                string Hyundai_Left = HYUNDAI_label_data_NEW.GetCode(this.Day_Time_Working, year, month, day);
                int Hyundai_cnt = int.Parse(dgvP0.Rows[2].Cells[1].Value.ToString());

                //프린트출력명령어
                string cmd = HyundaiLabel_CMD(
                    dgvP0.Rows[5].Cells[1].Value.ToString(),
                    dgvP0.Rows[6].Cells[1].Value.ToString(),
                    dgvP0.Rows[7].Cells[1].Value.ToString(),
                    Hyundai_Left,
                    Hyundai_cnt.ToString("D5"),
                    barcode
                    );

#if Release

                LabelPrinter.SendString( cmd );
#endif

                //LastKBcnt = cnt;
                cnt = KB_count.CountUp_KB(cnt);
                RWdataFast.Save("cnt", cnt);

                this.Invoke(new dele(() =>
                {

                    dgvP0.Rows[1].Cells[1].Value = cnt;
                    dgvP0.Rows[2].Cells[1].Value = KB_count.ViewCnt_KB(dgvP0.Rows[1].Cells[1].Value.ToString()).ToString();

                    Log_K.WriteLog(log_lst, Mainpath, "현대 바코드 : " + barcode);
                }));

                return barcode;
            }

            else if (dis == 2)//KBI 일반 바코드 ㅡ> TM 외 나머지
            {

                int year = int.Parse(dgvP0.Rows[9].Cells[1].Value.ToString().Split('-')[0]);
                int month = int.Parse(dgvP0.Rows[9].Cells[1].Value.ToString().Split('-')[1]);
                int day = int.Parse(dgvP0.Rows[9].Cells[1].Value.ToString().Split('-')[2]);
                //, year,month,day

                //string barcode = Kabul_label_data.GetCode(ModelNamelbl.Text.Substring(0, 2), dgvP0.Rows[3].Cells[1].Value.ToString(), dgvP0.Rows[4].Cells[1].Value.ToString(), this.Day_Time_Working, cnt, year, month, day);
                string barcode = Kabul_label_data.GetCode(ModelNamelbl.Text.Substring(0, 2), dgvP0.Rows[3].Cells[1].Value.ToString(), dgvP0.Rows[4].Cells[1].Value.ToString(), this.Day_Time_Working, cnt, year, month, day, dgvP0.Rows[10].Cells[1].Value.ToString());

                string cmd = KBILabel_CMD(dgvP0.Rows[5].Cells[1].Value.ToString(), dgvP0.Rows[6].Cells[1].Value.ToString(), dgvP0.Rows[7].Cells[1].Value.ToString(), dgvP0.Rows[8].Cells[1].Value.ToString(), barcode);

#if Release

                LabelPrinter.SendString( cmd );

#endif


                cnt = KB_count.CountUp_KB(cnt);
                RWdataFast.Save("cnt", cnt);



                this.Invoke(new dele(() =>
                {

                    dgvP0.Rows[1].Cells[1].Value = cnt;
                    dgvP0.Rows[2].Cells[1].Value = KB_count.ViewCnt_KB(dgvP0.Rows[1].Cells[1].Value.ToString()).ToString();

                    Log_K.WriteLog(log_lst, Mainpath, "갑을 바코드 : " + barcode);
                }));

                return barcode;

            }

            return "";
        }

        // ^LS0    랑    ^MMT 빼라..         LS0 아마도 0,0새로잡는거     mmt 피딩끝나면 센서신호 안줌   
        string HyundaiLabel_CMD(string cap1, string cap2, string cap3, string day_english, string count, string barcode)
        {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             //^FO400,80^GFA,00512,00512,00008,:Z64
            //  2020_0206 TM 제외 QR 코드 위치 이동 -> 현대 바코드수정필요 
            //  2020-0730 바코드 아래로 글자 위로 수정 day _ count 줄이랑 barcode 줄 좌표 변경 550 , 175 / 230 >> 550, 25 / 53

            string result =
            //"^XA^PW827^LL0213^FO64,0^GFA,04096,04096,00032,:Z64:eJztlE1u1DAUx5/lFm9gvKpggRKOwSJqZjEH6I5rdDkL1JgLwIUQGCHEtheoMOoFXFWoFk1j3rMdx0Mzs2FJ3oySsX/z/D78twEWW+xfjb35tZ4G3pu/uPT+dx603g+vdt09mk4DTgO3w1uaGtLgMw28Krn3PYg0Fdby3hZYDDUOW5tSsRRjKHh9WWPCx32MFV6iDNCpzpBPjBVBWWIPnabARNhDWnNKgNvgxIOLSJXxfkpPMz8mBvXYhovMpeLk3zpybXPSmVcxNe8afL4VthMUUOYCGqyGwtsKB47bgVksiY+BYIsNCuElpm9GznIBDiTybuQ3PXOAu5W5hdqHTRcKmOY3Dl2RnyfMDO4f/sUbroArfhu4wbAF1yQAhhz4nY38xch13H/kGgS1kTl2b+D1xLsoicQ1s+zWwsux/Rp85qvIecHZyOnXk8RvCm4Kvorx+U9X+JPkvqCiYvw7w6woueGkT+RYP/JbG/mYP1hO4u964lg/9U/ovuCC1No5EKF/1H+JfOwfbAUdl84C7Y8O+0c8708TlIW8yvsr1cXEK0nyQYk3IRnbMVurC5b1IT/QYcEdcVE/7TtXq1ZmLn6Qlqlt+DrlZ8/Z2QmcTPrkQdioSIOvVqXZLGSUjqYH1ISESbPFAaXTxYc4M54bWdwQdHr5wIa4WJorLgCSr+jTTLwZeL5ugg9JOB3sOizQeiis9S7fP4L+2O5eUMfl/RXFoktOq+fzLsvLbMygfci/Z+6/Xasfuf/Ptj5awwbWG/wcbTaPuVltxXt27a3va/99jjdyxb4iH/bwCvm12tqq4fO8Qn91bld7uDyN/vu4+Bj959f3A3eTv5rj9lD8+0/Eg387Xx8zKf4eDoHv9wdzuH788kP9AyM9+vvhIN/6fpbrp2CeXTF1dnXZsG+P+WKLLbbYYrP2B2qJ5/0=:21A^FO400,80^GFA,00512,00512,00008,:Z64:eJzF0LENwzAMBEASKlwmG2SFbOCV7M5dDGQxb5AVNILUsRDyEV80kAEChM0Vkig+RX5VUxvO2CmQQ6MPtPDtXGfAvd/AB9sFOHhcqaXKBiZUTVYUt4QWtvXZTd1+p2uLhIVKtc3VMIWg6rIPyukSavH/UVNmDLwy5xtz9vHTPlQ3YkjEkCnyK3zMr32c+zn39d/6AGMCf8I=:35BF^FO256,0^GFA,00768,00768,00012,:Z64:eJzVz7ENwjAQBVAjFzQoNwKLoGStFBEYUVBmFQaIZI+SDZzSFYcj5f5dmACueoV9979z/zpBeTA+Gt9e+oQnfcIRJo74cM4xibtxnMVXeizige5woVDEC4VeTlUP4pnCRY/t3CIC8xvR6mz2396C+roTtcz+3V3jNRty2sy2S0dP9LXdKXMyxmGfJXKdXNSU1I3SndxPzgddmEeB:C941" +

            "^XA^PW827^LL0213^FO64,0^GFA,05120,05120,00040,:Z64:eJztlTFvI0UUx99ktR4pSB6QiKhg+1R0NMhrvoGRsI4Ci/sIQTppUyDPJBENBfkIWNCcQkFFy2100l2ZKzjR7ukK6G6iOykTvLeP/9ucBLu21ylokPbJ9lrPv/3vf968Nybqo48+/uexczvsPebVpFrJvMXMldv+jAQc5+3soJ2IBOO/2ulfF62EZn56Z8Whqg5amZTZ0SkXradUZYtjL1pW0m8T7U1xxVs/mrft/SicqfDxoag73OrIfJe27JVRVeP4HZayzJG6IvpgoF2DS4IWzohBT1RZV9/z8TDKm1yuxVoi3ITUFzPozYgOB6rJWWeCrHoZKPIUBegZ6B0Oo2YFvsaTUS57HUgfkDmAvySD1YFqcoHSIMsIJZkCL+jJmoshNTjlyQbZFD+neEJ7+dTRVGo4kFX9q3wFcZCy+REZ3OOgx6mTTW9wOicuSTEXifi7osypV5kk2pziCivkwjjoVVCMSuuEK9sck0UDGhdP9Oc0wwX1K1r9Z9xN/7EzTnuUEvX0ab3eRZPTbzjt9IE5hD99d039/uHgrzAl9NIcaw7DDRxpFz/Z+42mbir9195fqNRYJd88y3orSlf7RecYN69rTn91lVF29Iqy1f7TucxHhGJB77JiZ0/EIyVD49p6KCjeCcXTyczNvkEN4edRs++jIpUOxR6n0gy2sKceX8m8qBqc8pbH6FZPc9LZ3Y/G2fdjuof8H6G5IaE+W6BZSh+nhX2Q0yHyL4omV9aTa3NVwh+9/2T6i8OcwHgTI5b2g14UoIcZsGhmzDENW1wqR5Vil3iMMumQQn5Mq5HIgCtxRjv38dq/vwYSH3JqKFaCd0V9YESVWXP0NkLJHOibVXeGTCt65s9tnObXY/TMeBsXvem/rVGf9347t/7/Y03oW7iT2Nlf3AbrYyV2j39Y7O/vHp8dny12z751m7jk/KW3qX7Mz7hM+CLfxJmTZeCRueBLrtIu7vyyrOZ6O3cSykr0lt1ccuS5tPrx79evbfplh95PgQP0Hl6HES6bOZxkHv7Or8N8pLu4kBait+zWS9gnBfxBL+30xyERPXCjzro89Sa/8Tey3fUzRfrg4vlyua1+JrcMf8ydzz0KBn8geC5Xo5+79rfWu+G66lf749OH2N+u+sVqEtM7n8WffDqdvBvH401cH3300Ucf/038DXNIH7w=:9A88^FO400,80^GFA,00512,00512,00008,:Z64:eJzF0LENwzAMBEASKlwmG2SFbOCV7M5dDGQxb5AVNILUsRDyEV80kAEChM0Vkig+RX5VUxvO2CmQQ6MPtPDtXGfAvd/AB9sFOHhcqaXKBiZUTVYUt4QWtvXZTd1+p2uLhIVKtc3VMIWg6rIPyukSavH/UVNmDLwy5xtz9vHTPlQ3YkjEkCnyK3zMr32c+zn39d/6AGMCf8I=:35BF" + //400,

            "^FT68,197^A0N,33,33^FH\\^FD" + cap3 + "^FS" +
            "^FT68,126^A0N,33,33^FH\\^FD" + cap1 + "^FS" +
            "^FT550,25^A0N,23,24^FH\\^FD" + day_english + " " + count + "^FS" +
            "^FT550,53^A0N,23,24^FH\\^FD" + barcode + "^FS" +
            /*
            "^FT550,175^A0N,23,24^FH\\^FD" + day_english + " " + count + "^FS" +
            "^FT550,203^A0N,23,24^FH\\^FD" + barcode + "^FS" +
            */
            //"^FT671,203^A0N,23,24^FH\\^FD" + barcode + "^FS" +
            "^FT67,159^A0N,25,24^FH\\^FD" + cap2 + "^FS" +
            "^FT275,83^A0N,17,16^FH\\^FDMADE IN KOREA^FS" +
            "^FT308,57^A0N,37,36^FH\\^FDKBAT^FS" +
            "^BY128,128^FT550,202^BXN,8,200,0,0,1,~" +
            //"^BY128,128^FT712,149^BXN,8,200,0,0,1,~" +
            //"^BY128,128^FT812,149^BXN,8,200,0,0,1,~" +
            "^FH\\^FD" + barcode + "^FS" +
            "^PQ1,0,1,Y^XZ";


            //"^XA^PW827^LL0213^FO64,0^GFA,04096,04096,00032,:Z64:eJztlE1u1DAUx5/lFm9gvKpggRKOwSJqZjEH6I5rdDkL1JgLwIUQGCHEtheoMOoFXFWoFk1j3rMdx0Mzs2FJ3oySsX/z/D78twEWW+xfjb35tZ4G3pu/uPT+dx603g+vdt09mk4DTgO3w1uaGtLgMw28Krn3PYg0Fdby3hZYDDUOW5tSsRRjKHh9WWPCx32MFV6iDNCpzpBPjBVBWWIPnabARNhDWnNKgNvgxIOLSJXxfkpPMz8mBvXYhovMpeLk3zpybXPSmVcxNe8afL4VthMUUOYCGqyGwtsKB47bgVksiY+BYIsNCuElpm9GznIBDiTybuQ3PXOAu5W5hdqHTRcKmOY3Dl2RnyfMDO4f/sUbroArfhu4wbAF1yQAhhz4nY38xch13H/kGgS1kTl2b+D1xLsoicQ1s+zWwsux/Rp85qvIecHZyOnXk8RvCm4Kvorx+U9X+JPkvqCiYvw7w6woueGkT+RYP/JbG/mYP1hO4u964lg/9U/ovuCC1No5EKF/1H+JfOwfbAUdl84C7Y8O+0c8708TlIW8yvsr1cXEK0nyQYk3IRnbMVurC5b1IT/QYcEdcVE/7TtXq1ZmLn6Qlqlt+DrlZ8/Z2QmcTPrkQdioSIOvVqXZLGSUjqYH1ISESbPFAaXTxYc4M54bWdwQdHr5wIa4WJorLgCSr+jTTLwZeL5ugg9JOB3sOizQeiis9S7fP4L+2O5eUMfl/RXFoktOq+fzLsvLbMygfci/Z+6/Xasfuf/Ptj5awwbWG/wcbTaPuVltxXt27a3va/99jjdyxb4iH/bwCvm12tqq4fO8Qn91bld7uDyN/vu4+Bj959f3A3eTv5rj9lD8+0/Eg387Xx8zKf4eDoHv9wdzuH788kP9AyM9+vvhIN/6fpbrp2CeXTF1dnXZsG+P+WKLLbbYYrP2B2qJ5/0=:21AB^FO400,80^GFA,00512,00512,00008,:Z64:eJzF0LENwzAMBEASKlwmG2SFbOCV7M5dDGQxb5AVNILUsRDyEV80kAEChM0Vkig+RX5VUxvO2CmQQ6MPtPDtXGfAvd/AB9sFOHhcqaXKBiZUTVYUt4QWtvXZTd1+p2uLhIVKtc3VMIWg6rIPyukSavH/UVNmDLwy5xtz9vHTPlQ3YkjEkCnyK3zMr32c+zn39d/6AGMCf8I=:35BF^FO256,0^GFA,00768,00768,00012,:Z64:eJzVz7ENwjAQBVAjFzQoNwKLoGStFBEYUVBmFQaIZI+SDZzSFYcj5f5dmACueoV9979z/zpBeTA+Gt9e+oQnfcIRJo74cM4xibtxnMVXeizige5woVDEC4VeTlUP4pnCRY/t3CIC8xvR6mz2396C+roTtcz+3V3jNRty2sy2S0dP9LXdKXMyxmGfJXKdXNSU1I3SndxPzgddmEeB:C941" +
            //"^FT68,197^A0N,33,33^FH\\^FD" + cap3 + "^FS" +
            //"^FT68,126^A0N,33,33^FH\\^FD" + cap1 + "^FS" +
            //"^FT550,175^A0N,23,24^FH\\^FD" + day_english + " " + count + "^FS" +
            //"^FT550,203^A0N,23,24^FH\\^FD" + barcode + "^FS" +
            //    //"^FT671,203^A0N,23,24^FH\\^FD" + barcode + "^FS" +
            //"^FT67,159^A0N,25,24^FH\\^FD" + cap2 + "^FS" +
            //"^FT295,73^A0N,17,16^FH\\^FDMADE IN KOREA^FS" +
            //"^FT328,47^A0N,37,36^FH\\^FDKBAT^FS" +
            //"^BY128,128^FT550,152^BXN,8,200,0,0,1,~" +
            //    //"^BY128,128^FT712,149^BXN,8,200,0,0,1,~" +
            //    //"^BY128,128^FT812,149^BXN,8,200,0,0,1,~" +
            //"^FH\\^FD" + barcode + "^FS" +
            //"^PQ1,0,1,Y^XZ";

            //string result =
            ////"^XA^PW827^LL0213^FO64,0^GFA,04096,04096,00032,:Z64:+eJztlE1u1DAUx5/lFm9gvKpggRKOwSJqZjEH6I5rdDkL1JgLwIUQGCHEtheoMOoFXFWoFk1j3rMdx0Mzs2FJ3oySsX/z/D78twEWW+xfjb35tZ4G3pu/uPT+dx603g+vdt09mk4DTgO3w1uaGtLgMw28Krn3PYg0Fdby3hZYDDUOW5tSsRRjKHh9WWPCx32MFV6iDNCpzpBPjBVBWWIPnabARNhDWnNKgNvgxIOLSJXxfkpPMz8mBvXYhovMpeLk3zpybXPSmVcxNe8afL4VthMUUOYCGqyGwtsKB47bgVksiY+BYIsNCuElpm9GznIBDiTybuQ3PXOAu5W5hdqHTRcKmOY3Dl2RnyfMDO4f/sUbroArfhu4wbAF1yQAhhz4nY38xch13H/kGgS1kTl2b+D1xLsoicQ1s+zWwsux/Rp85qvIecHZyOnXk8RvCm4Kvorx+U9X+JPkvqCiYvw7w6woueGkT+RYP/JbG/mYP1hO4u964lg/9U/ovuCC1No5EKF/1H+JfOwfbAUdl84C7Y8O+0c8708TlIW8yvsr1cXEK0nyQYk3IRnbMVurC5b1IT/QYcEdcVE/7TtXq1ZmLn6Qlqlt+DrlZ8/Z2QmcTPrkQdioSIOvVqXZLGSUjqYH1ISESbPFAaXTxYc4M54bWdwQdHr5wIa4WJorLgCSr+jTTLwZeL5ugg9JOB3sOizQeiis9S7fP4L+2O5eUMfl/RXFoktOq+fzLsvLbMygfci/Z+6/Xasfuf/Ptj5awwbWG/wcbTaPuVltxXt27a3va/99jjdyxb4iH/bwCvm12tqq4fO8Qn91bld7uDyN/vu4+Bj959f3A3eTv5rj9lD8+0/Eg387Xx8zKf4eDoHv9wdzuH788kP9AyM9+vvhIN/6fpbrp2CeXTF1dnXZsG+P+WKLLbbYYrP2B2qJ5/0=:21AB^FO288,160^GFA,00512,00512,00008,:Z64:eJzF0LENwzAMBEASKlwmG2SFbOCV7M5dDGQxb5AVNILUsRDyEV80kAEChM0Vkig+RX5VUxvO2CmQQ6MPtPDtXGfAvd/AB9sFOHhcqaXKBiZUTVYUt4QWtvXZTd1+p2uLhIVKtc3VMIWg6rIPyukSavH/UVNmDLwy5xtz9vHTPlQ3YkjEkCnyK3zMr32c+zn39d/6AGMCf8I=:35BF^FO256,0^GFA,00768,00768,00012,:Z64:eJzVz7ENwjAQBVAjFzQoNwKLoGStFBEYUVBmFQaIZI+SDZzSFYcj5f5dmACueoV9979z/zpBeTA+Gt9e+oQnfcIRJo74cM4xibtxnMVXeizige5woVDEC4VeTlUP4pnCRY/t3CIC8xvR6mz2396C+roTtcz+3V3jNRty2sy2S0dP9LXdKXMyxmGfJXKdXNSU1I3SndxPzgddmEeB:C941" +
            //"^XA^PW827^LL0213^FO64,0^GFA,04096,04096,00032,:Z64:eJztlE1u1DAUx5/lFm9gvKpggRKOwSJqZjEH6I5rdDkL1JgLwIUQGCHEtheoMOoFXFWoFk1j3rMdx0Mzs2FJ3oySsX/z/D78twEWW+xfjb35tZ4G3pu/uPT+dx603g+vdt09mk4DTgO3w1uaGtLgMw28Krn3PYg0Fdby3hZYDDUOW5tSsRRjKHh9WWPCx32MFV6iDNCpzpBPjBVBWWIPnabARNhDWnNKgNvgxIOLSJXxfkpPMz8mBvXYhovMpeLk3zpybXPSmVcxNe8afL4VthMUUOYCGqyGwtsKB47bgVksiY+BYIsNCuElpm9GznIBDiTybuQ3PXOAu5W5hdqHTRcKmOY3Dl2RnyfMDO4f/sUbroArfhu4wbAF1yQAhhz4nY38xch13H/kGgS1kTl2b+D1xLsoicQ1s+zWwsux/Rp85qvIecHZyOnXk8RvCm4Kvorx+U9X+JPkvqCiYvw7w6woueGkT+RYP/JbG/mYP1hO4u964lg/9U/ovuCC1No5EKF/1H+JfOwfbAUdl84C7Y8O+0c8708TlIW8yvsr1cXEK0nyQYk3IRnbMVurC5b1IT/QYcEdcVE/7TtXq1ZmLn6Qlqlt+DrlZ8/Z2QmcTPrkQdioSIOvVqXZLGSUjqYH1ISESbPFAaXTxYc4M54bWdwQdHr5wIa4WJorLgCSr+jTTLwZeL5ugg9JOB3sOizQeiis9S7fP4L+2O5eUMfl/RXFoktOq+fzLsvLbMygfci/Z+6/Xasfuf/Ptj5awwbWG/wcbTaPuVltxXt27a3va/99jjdyxb4iH/bwCvm12tqq4fO8Qn91bld7uDyN/vu4+Bj959f3A3eTv5rj9lD8+0/Eg387Xx8zKf4eDoHv9wdzuH788kP9AyM9+vvhIN/6fpbrp2CeXTF1dnXZsG+P+WKLLbbYYrP2B2qJ5/0=:21AB^FO400,80^GFA,00512,00512,00008,:Z64:eJzF0LENwzAMBEASKlwmG2SFbOCV7M5dDGQxb5AVNILUsRDyEV80kAEChM0Vkig+RX5VUxvO2CmQQ6MPtPDtXGfAvd/AB9sFOHhcqaXKBiZUTVYUt4QWtvXZTd1+p2uLhIVKtc3VMIWg6rIPyukSavH/UVNmDLwy5xtz9vHTPlQ3YkjEkCnyK3zMr32c+zn39d/6AGMCf8I=:35BF^FO256,0^GFA,00768,00768,00012,:Z64:eJzVz7ENwjAQBVAjFzQoNwKLoGStFBEYUVBmFQaIZI+SDZzSFYcj5f5dmACueoV9979z/zpBeTA+Gt9e+oQnfcIRJo74cM4xibtxnMVXeizige5woVDEC4VeTlUP4pnCRY/t3CIC8xvR6mz2396C+roTtcz+3V3jNRty2sy2S0dP9LXdKXMyxmGfJXKdXNSU1I3SndxPzgddmEeB:C941" +
            //"ngk9xy7DkJO8UggPGesDlRBbGcq++2B4YgKwNVc9I8i66rWzFFH72irhtV3i07PwWJ08E+kcGtzqDBFyWA3tszgbwSs4qcjcaHvGLjzxznDXve1H6lm89zx3krpd4nFfACg6Q6PafIkHKNTiE3Wf/DuwW49vdZayotvtwEFZz/9v679NTVOHV/2lKk/er8HTRMOsKFRnY9dEo/luGd4LM1OobzcO6vBQytL6b2/+WYcvH4likKfWfKEc8N9tHNTkzcAf698ayes2N/gAw/x113fA36m7XgMe90+N+gj9KTeTOda/U9ZaL91uHud/bJLRfNZmhhoA8+CzcXoUfw6Kn3WAc1Dwf38aNGM/PtrxwvlR4sU34G7qw8vkzDeiU7o4dZj58K92z3yjO6VWB9wvu0OUjHrl+pdGvqI9Lfxrg1308f9h2Ytv/vXiROiTZ60x4ePP97iXv1hjfvwee+E9L/+Gn/9jPaG8/NnEBQ+e7wVe/tyzPnyPefkz9H/Th9+Lm+/68Lifr/jwH8/OfuDDtwY/z9VVlIAXf2Wl73WehFvbw35MdEqsfJ/68NfA73xm0aEff8n5U9xYY4011lj19A9hMxdp:2F56" +
            //"^FT68,197^A0N,33,33^FH\\^FD" + cap3 + "^FS" +  //  NX4 , RH 등
            //"^FT68,126^A0N,33,33^FH\\^FD" + cap1 + "^FS" +  //  품번 882p0 등
            //"^FT671,175^A0N,23,24^FH\\^FD" + day_english + " " + count + "^FS" +    //  날짜 / 시리얼넘버(카운트)
            //"^FT671,203^A0N,23,24^FH\\^FD" + barcode + "^FS" +  //  바코드
            //"^FT67,159^A0N,25,24^FH\\^FD" + cap2 + "^FS" +  //  svhm ass'y
            //"^FT295,73^A0N,17,16^FH\\^FDMADE IN KOREA^FS" + //  made in korea
            //"^FT328,47^A0N,37,36^FH\\^FDKBAT^FS" +  //  KBAT
            //    ////"^BY128,128^FT412,149^BXN,8,200,0,0,1,~" +
            //"^BY128,128^FT712,149^BXN,8,200,0,0,1,~" +   //  코드위치
            //"^FH\\^FD" + barcode + "^FS" +
            //"^PQ1,0,1,Y^XZ";

            //"^XA" +
            //"^MMT" +
            //"^PW827" +
            //"^LL0213" +
            //"^LS0" +
            //"^FO0,0^GFA,03072,03072,00032,:Z64:" +
            //"eJztlM9r1EAUx18yXbPYH+lBtEI2KUXEk/TgYSlxowdBBEGK1R7EWorowcOyVKg0bWL3UhDqUU918eDBk/gHaJbeRLwVL9pONwhFFKMtOsXtPie77Watm1lFEIR+D5nsfub73szLmwHY1X8gWa4N9R/y1vvviSCuhiNiuSlXa0BCxHC2u5Pz/5ECWHwIAPb29qTRnXDoNpa42+HAwWqcVLfWj54dcXWDASnzaQVoRxcmj2jZ99Q+WtjmVp6bERT+BMMDv00L6JVLHfXNZ8gXmaCrlvr65Lsl8BW9ROmHrvrybJWCgp6RmbbVYiCRRMqjV15GnHGufqeZjG2rHoN8nnMaccnjSZ3P1NbZeKdng0RS4BUjTjwrIEgDZjFTpSbB5w9cvP3qJ34AaTYwAtug+8ji0IiLxQbuhoWhaWpR06I65Inh0ZW5OldcZ3MTvTTl/uOBBlJC8+joUuSfCevvDVAruGE91aUFWaelsUrEi4gKQjW/yf1jHVqWjt5r5IyXj7EMs22mgf+M13csH+2/iJ6EQIf326Hg4Dmtu5HDWgWgQnj9pk1mM2inWhr9/FydT/JvW1Y8Q8/YE1MB36+m4QKJuMVAYqqrriaTwD/FL53Ly08CB5Q3/H3qxE4KsloC4ld4F7thG9T8shyFIbx3sNp/35o3OFbq/cuacaPW+Ak+uAB7rh0rnj55YbAxQGgPA4R25drh/PVcbk8DV9ZrlbwZPhNXD0nXc4NtzTJV+btDKyx3+aOAS8OD507F8oeP8i/O5O64MbytkF45fXbwVpwfZmcLyeRMcsuvp9YqGi6Xcan5bN1cq5j8YMfzxXnu34zjRtmZ1/xlcyWOrzu65r+N5+WMbvpoxsfXuX/5ePz6MkbZF+1Pq/K4/DU/bvgCf+Avb8T5/15y7eb/kxv/X+iIGJPXYt4ZnjURn/KEfKB2JmPlhFeCQFnoEfLzLfh4b7qFv1vI11v4L7ri/F9B7B9vtX64L+RZ6BfygcdPhDw1QoW865Mr5IkhMScVIYYW5d9VqB+SyHXX:9B60" +
            //"^PQ1,0,1,Y^XZ";

            return result;
        }

        string KBILabel_CMD(string Motor, string Cap1, string Cap2, string Cap3, string barcode)
        {
            string result = "^XA" +

            "^FO256,0^GFA,01536,01536,00016,:Z64:" +
            "eJzt1EFyhCAQBdAmWOklR+AoHA0tL6Y34QguXVD+IDNC95hJZZuqsHuF" +
            "2F+gJfoLg5GVPTBKA0jqcagFrvgQDsXyBdWLej2wNX6c67E3W9Z" +
            "2M1QBX43uVTskqABxiyoA9hro+iKDrAIZPMZvbU9wD8wYDWzfoWKK3" +
            "O2Kw9w/yJVknqXH5Kz0lJ2anw43a7Nef7Cdm/05P1CEsCsRutfsSLrUp12" +
            "avC17JBxm7cjKFpydqFf2p8+702V9z7vWeZEfsLvwdPeYgnI5ZbVemM/5wbT9t7W" +
            "+FX6M6/zMG787f6pKvUPqbdvCcrnetj2Ml+tty4G0fXNth+OzubYDhmarr/czU" +
            "Cv3DPCDX/vztZ+Naj+69f/tf0Fxof/x3fgCpqNyqg==:728B" +

            "^FT293,188^A0N,29,26^FH\\^FD" + barcode + "^FS" +
            "^FT137,58^A0N,54,57^FH\\^FD" + Motor + "^FS" +
            "^FT137,187^A0N,29,28^FH\\^FD" + Cap3 + "^FS" +
            "^FT138,111^A0N,22,26^FH\\^FD" + Cap1 + "^FS" +
            "^FT138,138^A0N,22,26^FH\\^FD" + Cap2 + "^FS" +
            "^FO123,2^GB0,209,7^FS" +
            "^BY128,128^FT366,145^BXN,8,200,0,0,1,~" +  //  FT 366 클수록 오른쪽 작을수록 왼쪽
            "^FH\\^FD" + barcode + "^FS" +
            "^PQ1,0,1,Y^XZ";

            return result;
        }


        bool confirmPrintData() //  프린터 설정 완료
        {
            //프로젝트이름
            if (dgvP0.Rows[3].Cells[1].Value == null || !(dgvP0.Rows[3].Cells[1].Value.ToString().Length == 1))
            {
                return false;
            }

            //라인정보
            if (dgvP0.Rows[4].Cells[1].Value == null || !(dgvP0.Rows[4].Cells[1].Value.ToString().Length == 2))
            {
                return false;
            }

            //셀이 비면 안된다. 0~7 index
            for (int i = 0; i < 7; i++)
            {
                if (dgvP0.Rows[i].Cells[1].Value == null || dgvP0.Rows[i].Cells[1].Value.ToString().Length == 0)
                {
                    return false;
                }
            }

            //날짜선택
            if (dgvP0.Rows[9].Cells[1].Value == null || !(dgvP0.Rows[9].Cells[1].Value.ToString().Length == 10))
            {
                return false;
            }

            return true;
        }

        #endregion

        void TimeLoad(DevExpress.XtraEditors.TimeEdit te)
        {
            te.Time = new DateTime(
                1,
                1,
                1,
                int.Parse(RWdataFast.Load(te.Name + "H", 0)),
                int.Parse(RWdataFast.Load(te.Name + "M", 0)),
                int.Parse(RWdataFast.Load(te.Name + "S", 0))
                );
        }

        #region 이력조회 버튼
        private void simpleButton14_Click(object sender, EventArgs e)   //  이력조회에 조회 버튼
        {
            int daydiff = Ken2.Util.Dtime.DayDiff(Dtime.GetDateTime(Date0, Time0), Dtime.GetDateTime(Date1, Time1));
            if (daydiff > 32)
            {
                MessageBox.Show("한 달 이내의 이력만 조회해주세요", "Error");
                return;
            }

            SelectHistory();
        }

        private void simpleButton15_Click(object sender, EventArgs e)   //  이력조회 오늘 버튼
        {
            NameSearchcheck.Checked = false;
            SetToday();
        }

        private void simpleButton1_Click(object sender, EventArgs e)    //  이력조회 선택 버튼
        {
            try
            {
                string Item = dgvH0.Rows[dgvH0.CurrentCell.RowIndex].Cells[0].Value.ToString();
                NameSearchcheck.Checked = true;
                NameSearchTB.Text = Item;
                SelectHistory();
            }
            catch (Exception)
            {

            }
        }

        private void simpleButton18_Click(object sender, EventArgs e)   //  CSV파일로 저장
        {
            Directory.CreateDirectory(@"D:\Database\SavedData\");
            GridMaster.SaveCSV(dgvH0, @"D:\Database\SavedData\" + Dtime.Now(Dtime.StringType.ForFile) + ".csv");

            MessageBox.Show("데이터가 저장되었습니다.\n경로 : " + @"D:\Database\SavedData\", "Message");
        }

        private void simpleButton34_Click(object sender, EventArgs e)   //  CSV폴더 열기
        {
            Directory.CreateDirectory(@"D:\Database\SavedData\");
            System.Diagnostics.Process.Start("explorer.exe", @"D:\Database\SavedData\");
        }
        #endregion



        public bool CanPrint(bool Day_Time_Working, string cnt)
        {
            int year = int.Parse(dgvP0.Rows[9].Cells[1].Value.ToString().Split('-')[0]);
            int month = int.Parse(dgvP0.Rows[9].Cells[1].Value.ToString().Split('-')[1]);
            int day = int.Parse(dgvP0.Rows[9].Cells[1].Value.ToString().Split('-')[2]);
            //, year,month,day

            string barcode = Kabul_label_data.GetCode(ModelNamelbl1.Text.Substring(0, 2), dgvP0.Rows[3].Cells[1].Value.ToString(), dgvP0.Rows[4].Cells[1].Value.ToString(), Day_Time_Working, cnt, year, month, day, dgvP0.Rows[10].Cells[1].Value.ToString());
            string query = "select count(*) from table2 where `Barcode`='" + barcode + "';";
            //Console.WriteLine( query );
            int bcr_cnt = sql.ExecuteQuery_Select_Count(query);

            if (bcr_cnt == 0)
                return true;
            else
                return false;
        }

        private void kenButton1_Click(object sender, EventArgs e)   //  프린터 주간 버튼
        {
            if (!confirmPrintData())
            {
                MessageBox.Show("프로젝트와 날짜를 선택해주세요", "Error");
                return;
            }

            string num = LabelProcess_num.Text;

            if (CanPrint(true, "A0A"))
            {
                if (POPUP.YesOrNo("INFO", "주간으로 전환하면 Count가 A0A로 초기화 됩니다.\n계속 할까요?"))
                {

                    Day_Time_Working = true;
                    cnt = "A0A";

                    dgvP0.Rows[1].Cells[1].Value = cnt;
                    dgvP0.Rows[0].Cells[1].Value = Kabul_label_data.GetDTinfo(Day_Time_Working);
                    dgvP0.Rows[2].Cells[1].Value = KB_count.ViewCnt_KB(cnt);
                    RWdataFast.Save("cnt", cnt);
                    RWdataFast.Save("Day_Time_Working", "true");
                }
            }
            else if (CanPrint(true, num))
            {
                if (POPUP.YesOrNo("INFO", "주간으로 전환하고 Count가 " + num + "(으)로 초기화 됩니다.\n계속 할까요?"))
                {

                    Day_Time_Working = true;
                    cnt = num;

                    dgvP0.Rows[1].Cells[1].Value = cnt;
                    dgvP0.Rows[0].Cells[1].Value = Kabul_label_data.GetDTinfo(Day_Time_Working);
                    dgvP0.Rows[2].Cells[1].Value = KB_count.ViewCnt_KB(cnt);
                    RWdataFast.Save("cnt", cnt);
                    RWdataFast.Save("Day_Time_Working", "true");
                }
            }
            else
                MessageBox.Show("변경 할 수 없습니다.\n이미 저장된 카운트 데이터입니다", "Error");
        }

        private void kenButton2_Click(object sender, EventArgs e)   //  프린터 야간 버튼
        {
            if (!confirmPrintData())
            {
                MessageBox.Show("프로젝트와 날짜를 선택해주세요", "Error");
                return;
            }


            string num = LabelProcess_num.Text;

            if (CanPrint(false, "A0A"))
            {
                if (POPUP.YesOrNo("INFO", "야간으로 전환하면 Count가  A0A로 초기화 됩니다.\n계속 할까요?"))
                {
                    Day_Time_Working = false;
                    cnt = "A0A";

                    dgvP0.Rows[1].Cells[1].Value = cnt;
                    dgvP0.Rows[0].Cells[1].Value = Kabul_label_data.GetDTinfo(Day_Time_Working);
                    dgvP0.Rows[2].Cells[1].Value = KB_count.ViewCnt_KB(cnt);


                    RWdataFast.Save("cnt", cnt);
                    RWdataFast.Save("Day_Time_Working", "false");
                }
            }
            else if (CanPrint(false, num))
            {
                if (POPUP.YesOrNo("INFO", "야간으로 전환하고 Count가 " + num + "(으)로 초기화 됩니다.\n계속 할까요?"))
                {

                    Day_Time_Working = false;
                    cnt = num;

                    dgvP0.Rows[1].Cells[1].Value = cnt;
                    dgvP0.Rows[0].Cells[1].Value = Kabul_label_data.GetDTinfo(Day_Time_Working);
                    dgvP0.Rows[2].Cells[1].Value = KB_count.ViewCnt_KB(cnt);

                    RWdataFast.Save("cnt", cnt);
                    RWdataFast.Save("Day_Time_Working", "false");
                }
            }
            else
                MessageBox.Show("변경 할 수 없습니다.\n이미 저장된 카운트 데이터입니다", "Error");
        }

        private void kenButton3_Click(object sender, EventArgs e)   //  프린터 날짜 적용 버튼
        {
            if (dgvP0.Rows[9].Cells[1].Value == null || dgvP0.Rows[9].Cells[1].Value.ToString().Length == 0)
            {
                //dgvP0.Rows[9].Cells[1].Value = PrintDate0.Value.ToShortDateString();
                
                //작업자 실수로, 날짜 적용버튼 누르면 무조건 오늘으로 설정
              dgvP0.Rows[9].Cells[1].Value = DateTime.Now.ToString("yyyy-MM-dd");

            }
            else if (POPUP.YesOrNo("INFO", "날짜 정보를 변경할까요?"))
            {
                // dgvP0.Rows[9].Cells[1].Value = PrintDate0.Value.ToShortDateString();
                dgvP0.Rows[9].Cells[1].Value = DateTime.Now.ToString("yyyy-MM-dd");

            }
        }

        private void kenButton6_Click(object sender, EventArgs e)   //  프린터 라벨테스트 출력 버튼
        {
            try
            {
                if (LabelPrinter.PrinterStatus == 0)
                {

                    if (POPUP.YesOrNo("INFO", "현재 정보의 라벨을 한 장 출력하고 Count 1이 추가됩니다.\n계속 할까요?"))
                    {
                        PrintOne();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void kenButton4_Click(object sender, EventArgs e)   //  프린터 라벨테스트 저장버튼
        {
            string cap1 = "";
            string cap2 = "";
            string cap3 = "";
            string cap4 = "";
            string ccode = "";

            string d3 = "";
            string d4 = "";

            try
            {
                cap1 = dgvP0.Rows[5].Cells[1].Value.ToString();
                cap2 = dgvP0.Rows[6].Cells[1].Value.ToString();
                cap3 = dgvP0.Rows[7].Cells[1].Value.ToString();
                cap4 = dgvP0.Rows[8].Cells[1].Value.ToString();

                d3 = dgvP0.Rows[3].Cells[1].Value.ToString();
                d4 = dgvP0.Rows[4].Cells[1].Value.ToString();

                ccode = dgvP0.Rows[10].Cells[1].Value.ToString();

            }
            catch (Exception)
            {

            }

            RWdataFast.Save_DBfolder(0, CurrentModelNum, "cap1", cap1);
            RWdataFast.Save_DBfolder(0, CurrentModelNum, "cap2", cap2);
            RWdataFast.Save_DBfolder(0, CurrentModelNum, "cap3", cap3);
            RWdataFast.Save_DBfolder(0, CurrentModelNum, "cap4", cap4);

            RWdataFast.Save_DBfolder(0, CurrentModelNum, "d3", d3);
            RWdataFast.Save_DBfolder(0, CurrentModelNum, "d4", d4);

            RWdataFast.Save_DBfolder(0, CurrentModelNum, "ccode", ccode);

            MessageBox.Show("저장 되었습니다");
        }

        public bool InspectCnt(string str)
        {
            char[] ch = str.ToCharArray();

            if (str.Length != 3)
                return false;

            if (ch[0] > 90)
                return false;
            if (ch[0] < 65)
                return false;

            if (ch[2] > 90)
                return false;
            if (ch[2] < 65)
                return false;

            if (ch[1] > 57)
                return false;
            if (ch[1] < 48)
                return false;

            return true;
        }

        private void kenButton5_Click(object sender, EventArgs e)   //  프린터 다음 Data (시리얼번호) 강제수정 적용 버튼
        {
            if (!confirmPrintData())
            {
                MessageBox.Show("프로젝트를 선택해주세요", "Error");
                return;
            }


            bool bb = InspectCnt(LabelProcess_num.Text);
            if (bb)
            {
                if (CanPrint(this.Day_Time_Working, LabelProcess_num.Text))
                {
                    if (POPUP.YesOrNo("INFO", "설정된 값으로 다음 바코드의 Count가 수정됩니다.\n계속 할까요?"))
                    {
                        cnt = LabelProcess_num.Text;

                        dgvP0.Rows[1].Cells[1].Value = cnt;

                        RWdataFast.Save("cnt", cnt);

                        dgvP0.Rows[2].Cells[1].Value = KB_count.ViewCnt_KB(dgvP0.Rows[1].Cells[1].Value.ToString()).ToString();
                    }
                }
                else
                {
                    MessageBox.Show("변경 할 수 없습니다.\n이미 저장된 카운트 데이터입니다", "Error");
                }
            }
            else
                MessageBox.Show("수량 데이터를 확인해주세요. Ex) A0A", "Error");
        }

        private void kenButton13_Click(object sender, EventArgs e)  //  패스워드 변경 버튼
        {
            pass = textBox10.Text;
            MessageBox.Show("Success");
        }

        private void button1_Click(object sender, EventArgs e)  //  통신 클리어 버튼
        {
            listBox1.Items.Clear();
        }

        private void button9_Click(object sender, EventArgs e)  //  통신 상단 데이터 쓰기 버튼
        {
            plc1.MCWrite((int)numericUpDown3.Value, (int)numericUpDown5.Value);
        }

        private void button7_Click(object sender, EventArgs e)  //  통신 하단 데이터 쓰기 버튼
        {
            plc1.MCWriteString((int)numericUpDown3.Value, textBox2.Text);
        }

        private void button6_Click(object sender, EventArgs e)  //  admin 모델체인지 1
        {
            int a = (int)numericUpDown4.Value;
            ModelLoad(a);
        }

        private void button11_Click(object sender, EventArgs e) //  admin 모델체인지 2
        {
            int a = (int)numericUpDown6.Value;
            ModelLoad1(a);
        }

        private void button2_Click(object sender, EventArgs e)  //  admin 버튼 2  -   안씀
        {
            //DB_SAVE_OverWrite0(textBox21.Text, textBox22.Text, textBox23.Text, textBox24.Text, textBox25.Text);
        }

        private void button3_Click(object sender, EventArgs e)  //  admin 버튼 3  -   안씀
        {
            //DB_SAVE_OverWrite_table2_pcb(textBox51.Text, textBox52.Text);
            //DB_SAVE_OverWrite_table2_impeller(textBox51.Text, textBox53.Text);
        }

        private void button22_Click(object sender, EventArgs e) //  admin test 버튼
        {
            string str = textBox1.Text;

            bool bb = BarcodeCheck(str, "barcode2", 3);
        }

        private void button12_Click(object sender, EventArgs e) //  admin 프린터상태 버튼  -   콘솔 확인
        {
            Console.WriteLine(LabelPrinter.PrinterStatus);
        }

        private void simpleButton20_Click(object sender, EventArgs e)   //  보관 - 6010_0 보내기
        {
            plc1.MCWrite(6010, 0);//저장했습니다.
        }

        private void simpleButton17_Click(object sender, EventArgs e)   //  보관 - 6010_1 보내기
        {
            plc1.MCWrite(6010, 1);//저장했습니다.
        }

        private void simpleButton21_Click(object sender, EventArgs e)   //  보관 - 6010_2 보내기
        {
            plc1.MCWrite(6010, 2);//저장했습니다.
        }

        private void labelprinter_btn_Click(object sender, EventArgs e) //  프린터7 명령어 보내기
        {
            string cmd = labelprinter_txt.Text;

            LabelPrinter.SendString(cmd);
        }

        private void button4_Click(object sender, EventArgs e)  //  프린터7 리셋 (프린터1)
        {
            string str = " CT~~CD,~CC^~CT~" +
                "^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR6,6~SD15^JUS^LRN^CI0^XZ" +
                "^XA" +
                "^MMT" +
                "^PW639" +
                "^LL0240" +
                "^LS0" +
                "^XZ";

            LabelPrinter.SendString(str);
        }

        private void button5_Click(object sender, EventArgs e)  //  자동실행 설정
        {
            try
            {
                // 시작프로그램 등록하는 레지스트리
                string runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                RegistryKey strUpKey = Registry.LocalMachine.OpenSubKey(runKey);
                if (strUpKey.GetValue("VisionStartup") == null)
                {
                    strUpKey.Close();
                    strUpKey = Registry.LocalMachine.OpenSubKey(runKey, true);
                    // 시작프로그램 등록명과 exe경로를 레지스트리에 등록
                    strUpKey.SetValue("VisionStartup", Application.ExecutablePath);
                }
                MessageBox.Show("성공적으로 적용되었습니다.");
            }
            catch
            {
                MessageBox.Show("적용 실패했습니다.");
            }
        }

        private void button19_Click(object sender, EventArgs e) //  자동실행 해제
        {
            try
            {
                string runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                RegistryKey strUpKey = Registry.LocalMachine.OpenSubKey(runKey, true);
                // 레지스트리값 제거
                strUpKey.DeleteValue("VisionStartup");
                MessageBox.Show("해제 처리되었습니다.");
            }
            catch
            {
                MessageBox.Show("적용 실패했습니다.");
            }
        }

        private void kenButton16_Click(object sender, EventArgs e)  //  설정에 주/야 작업시간설정 저장버튼
        {
            TimeSave(daynight0);
            TimeSave(daynight1);
            TimeSave(daynight2);
            TimeSave(daynight3);

            MessageBox.Show("저장되었습니다.", "Message");
        }

        void TimeSave(DevExpress.XtraEditors.TimeEdit te)
        {
            RWdataFast.Save(te.Name + "H", te.Time.Hour.ToString());
            RWdataFast.Save(te.Name + "M", te.Time.Minute.ToString());
            RWdataFast.Save(te.Name + "S", te.Time.Second.ToString());
        }

        private void kenButton17_Click(object sender, EventArgs e)
        {
            GridMaster.SaveCSV_OnlyData(dgvDE0, System.Windows.Forms.Application.StartupPath + "\\quantity.csv");//셀데이터로드
            MessageBox.Show("저장 되었습니다");
        }


        string x_r_savePATH = "";

        //x-r 엑셀 파일이 주간과 야간 데이터를 비교하게 만들어져있음, 그래서 주/야구분이 필요
        //이 함수로 어떤 컬럼 하나의 하루치 데이터의 평균값을 구함 주간 평균 하나, 야간 평균 하나
        public bool X_Rdata(bool Daily, string model, DateTime StartDateTime, DateTime EndDateTime, string db_column, out int row, out double datum)
        {
            row = 0;
            datum = 0;

            if (db_column == null || db_column.Length == 0)
                return false;

            if (Daily)
            {
                //주간

                string query =
                    "SELECT COUNT(*) , AVG(first1." + db_column + ") FROM (SELECT `" + db_column + "` FROM table1 WHERE `Model`='" + model + "' AND `Datetime` > '"
                    + StartDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "' AND `Datetime` < '"
                    + EndDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "' AND `Decision`='OK') AS first1;";

                DataSet ds = sql.ExecuteQuery(query);
                if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                {
                    try
                    {
                        row = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                        datum = double.Parse(ds.Tables[0].Rows[0][1].ToString());
                    }
                    catch (Exception)
                    {

                    }
                }

                //Console.WriteLine( "주간 / " + query );
                //Console.WriteLine( query );
            }
            else
            {
                //야간
                DateTime EndDateTime1 = EndDateTime.AddDays(1);

                //string query =
                //    "SELECT COUNT(*) , AVG(first1.data) FROM (SELECT `data` FROM table1 WHERE `Model`='" + model + "' AND `Datetime` > '"
                //    + StartDateTime.ToString( "yyyy-MM-dd HH:mm:ss" ) + "' AND `Datetime` < '"
                //    + EndDateTime1.ToString( "yyyy-MM-dd HH:mm:ss" ) + "' AND `Decision`='OK' AND `Process`='"
                //    + db_column + "' AND `Inspection`='"
                //    + inspection + "') AS first1;";

                string query =
                    "SELECT COUNT(*) , AVG(first1." + db_column + ") FROM (SELECT `" + db_column + "` FROM table1 WHERE `Model`='" + model + "' AND `Datetime` > '"
                    + StartDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "' AND `Datetime` < '"
                    + EndDateTime1.ToString("yyyy-MM-dd HH:mm:ss") + "' AND `Decision`='OK') AS first1;";



                DataSet ds = sql.ExecuteQuery(query);
                if (ds.Tables.Count != 0 && ds.Tables[0].Rows.Count != 0)
                {
                    try
                    {
                        row = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                        datum = double.Parse(ds.Tables[0].Rows[0][1].ToString());
                    }
                    catch (Exception)
                    {

                    }
                }

                //Console.WriteLine("야간 / " +  query );

            }

            if (row == 0)
                return false;
            else
                return true;
        }


        private void simpleButton7_Click(object sender, EventArgs e)    //  X_R 조회 버튼
        {
            if (!POPUP.YesOrNo("INFO", "좌측 검색 조건으로 X-R 데이터를 수집합니다.\n작업 시간은 수초에서 수십분까지 걸릴 수 있으며, 열려있는 EXCEL 프로그램이 종료됩니다.\n검색 작업은 라인작업을 멈추고 조회하는 것을 권장합니다.\n계속 할까요?"))
            {
                return;
            }

            try
            {
                Process[] p = Process.GetProcessesByName("EXCEL");
                for (int i = 0; i < p.Length; i++)
                {
                    p[i].Kill();
                }


                Directory.CreateDirectory(@"D:\Database\etc\temp");

                try
                {

                    string[] files = Directory.GetFiles(@"D:\Database\etc\temp");

                    for (int i = 0; i < files.Length; i++)
                    {
                        File.Delete(files[i]);
                    }

                }
                catch (Exception)
                {

                }

                DateTime start0 = new DateTime(Date0.Value.Year, Date0.Value.Month, Date0.Value.Day);
                DateTime end0 = new DateTime(Date1.Value.Year, Date1.Value.Month, Date1.Value.Day);

                int cnt = 0;
                int ValueCount = 0;

                x_rDATAClass[] x_rdataclass = new x_rDATAClass[25];
                for (int i = 0; i < 25; i++)
                {
                    x_rdataclass[i] = new x_rDATAClass();
                }

                TimeSpan ts = end0 - start0;
                if (ts.Days < 0)
                    return;

                string selected_columns_name = GridMaster.CurrentCell(dgvXR0, 1);

                while (true)
                {
                    cnt++;

                    int row0 = 0;
                    double datum0 = 0;

                    int row1 = 0;
                    double datum1 = 0;

                    DateTime ins0 = new DateTime(start0.Year, start0.Month, start0.Day, daynight0.Time.Hour, daynight0.Time.Minute, daynight0.Time.Second);
                    DateTime ins1 = new DateTime(start0.Year, start0.Month, start0.Day, daynight1.Time.Hour, daynight1.Time.Minute, daynight1.Time.Second);
                    DateTime ins2 = new DateTime(start0.Year, start0.Month, start0.Day, daynight2.Time.Hour, daynight2.Time.Minute, daynight2.Time.Second);
                    DateTime ins3 = new DateTime(start0.Year, start0.Month, start0.Day, daynight3.Time.Hour, daynight3.Time.Minute, daynight3.Time.Second);


                    bool s1 = X_Rdata(true, ModelNamelbl.Text, ins0, ins1, selected_columns_name, out row0, out datum0);
                    bool s2 = X_Rdata(false, ModelNamelbl.Text, ins2, ins3, selected_columns_name, out row1, out datum1);

                    if (s1 || s2)
                    {
                        x_rdataclass[ValueCount].datetime = start0.ToShortDateString();
                        x_rdataclass[ValueCount].s1_data = datum0;
                        x_rdataclass[ValueCount].s2_data = datum1;
                        x_rdataclass[ValueCount].s1 = row0;
                        x_rdataclass[ValueCount].s2 = row1;

                        ValueCount++;

                        //Console.WriteLine( " - " + cnt + " - " );
                        //Console.WriteLine( start0.ToShortDateString( ) );
                        //Console.WriteLine( row0 );
                        //Console.WriteLine( row1 );
                        //Console.WriteLine( datum0 );
                        //Console.WriteLine( datum1 );

                    }

                    start0 = start0.AddDays(1);
                    //start1 = start1.AddDays( 1 );

                    TimeSpan ts1 = end0 - start0;
                    if (cnt >= 25 || ts1.Days < 0)
                        break;
                }


                string path = @"D:\Database\etc\sample.xlsx";

                Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                app.Visible = false;

                Microsoft.Office.Interop.Excel.Workbook wkb = app.Workbooks.Open(path);
                Microsoft.Office.Interop.Excel.Worksheet ws = wkb.Sheets[1];

                ws.Protect(Contents: false);

                for (int i = 0; i < ValueCount; i++)
                {
                    ws.Cells[33, 3 + i].Value = x_rdataclass[i].datetime;
                    ws.Cells[34, 3 + i].Value = x_rdataclass[i].s1_data.ToString("N2");
                    ws.Cells[35, 3 + i].Value = x_rdataclass[i].s2_data.ToString("N2");
                    ws.Cells[36, 3 + i].Value = x_rdataclass[i].s1;
                    ws.Cells[37, 3 + i].Value = x_rdataclass[i].s2;
                }

                ws.Cells[5, 3].Value = ModelNamelbl.Text;

                ws.Cells[5, 10].Value = GridMaster.CurrentCell(dgvXR0, 0);

                ws.Cells[5, 17].Value = " ";

                ws.Cells[6, 24].Value = Dtime.Now(Dtime.StringType.ForDatum);

                ws.Cells[7, 11].Value = numericUpDown21.Value.ToString();

                ws.Cells[7, 12].Value = numericUpDown22.Value.ToString();

                ws.Cells[7, 14].Value = numericUpDown23.Value.ToString();

                Microsoft.Office.Interop.Excel.Range r = ws.Range["A8:AA40"];

                r.Select();

                r.CopyPicture(Microsoft.Office.Interop.Excel.XlPictureAppearance.xlScreen,
                               Microsoft.Office.Interop.Excel.XlCopyPictureFormat.xlBitmap);

                if (Clipboard.GetDataObject() != null)
                {
                    IDataObject data = Clipboard.GetDataObject();

                    if (data.GetDataPresent(DataFormats.Bitmap))
                    {
                        Image image = (Image)data.GetData(DataFormats.Bitmap, true);
                        this.pictureBox1.Image = image;
                    }
                }

                try
                {
                    double cpkval = double.Parse(ws.Cells[63, 3].Value.ToString());

                    if (cpkval > 1000 || cpkval < -1000)
                        cpkval = 0;

                    cpk.Text = cpkval.ToString("N3");
                }
                catch (Exception)
                {
                    cpk.Text = "";
                }
                x_r_savePATH = @"D:\Database\etc\temp\" + Dtime.Now(Dtime.StringType.ForFile) + ".xlsx";

                wkb.SaveAs(x_r_savePATH);

                app.Quit();

                Process[] p1 = Process.GetProcessesByName("EXCEL");
                for (int i = 0; i < p1.Length; i++)
                {
                    p1[i].Kill();
                }

                if ((double)cpkdecision.Value < double.Parse(cpk.Text))
                {
                    LabelControl_K.OK(cpkOKNG);
                }
                else
                {
                    LabelControl_K.NG(cpkOKNG);
                }

            }
            catch (Exception)
            {


            }
        }

        private void kenButton15_Click(object sender, EventArgs e)  //  X_R CPK 기준 저장 버튼
        {
            RWdataFast.Save("cpk", cpkdecision.Value.ToString());
            MessageBox.Show("저장되었습니다.", "Message");
        }

        private void simpleButton9_Click(object sender, EventArgs e)    //  X_R CSV로 저장 버튼
        {
            try
            {
                if (x_r_savePATH.Length != 0)
                {
                    Directory.CreateDirectory(@"D:\Database\SavedData\");
                    File.Move(x_r_savePATH, @"D:\Database\SavedData\" + Path.GetFileName(x_r_savePATH));
                    MessageBox.Show("데이터가 저장되었습니다.\n경로 : " + @"D:\Database\SavedData\", "Message");
                }
                else
                {
                    MessageBox.Show("데이터가 없습니다.", "Error");
                }
            }
            catch (Exception)
            {


            }
        }

        private void simpleButton8_Click(object sender, EventArgs e)    //  X_R CSV 폴더 열기 버튼
        {
            Directory.CreateDirectory(@"D:\Database\SavedData\");
            System.Diagnostics.Process.Start("explorer.exe", @"D:\Database\SavedData\");
        }

    }
}
