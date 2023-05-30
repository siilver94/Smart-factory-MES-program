using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KB_Data
{
    public class x_rDATAClass
    {
        public string datetime;
        public double s1_data;
        public double s2_data;
        public double s3_data;
        public double s4_data;
        public double s5_data;

        public int s1;
        public int s2;
        public int s3;
        public int s4;
        public int s5;
    }

    public class Product_info
    {
        public string CurrentModel;
        public int Total;
        public int OK;
        public int NG;
    }

    public class KB_count
    {
        public static string CountUp_KB( string cnt )
        {
            string result = "";
            char[ ] split = cnt.ToCharArray( );

            if ( split[ 2 ] == 'Z' )
            {
                if ( split[ 1 ] == '9' )
                {
                    if ( split[ 0 ] == 'Z' )
                    {
                        return "Z9Z";
                    }
                    else
                    {
                        result = ( ++split[ 0 ] ).ToString( ) + "0A";
                    }
                }
                else
                {
                    result = split[ 0 ].ToString( ) + ( ++split[ 1 ] ).ToString( ) + "A";
                }
            }
            else
            {
                result = split[ 0 ].ToString( ) + split[ 1 ].ToString( ) + ( ++split[ 2 ] ).ToString( );

            }

            return result;
        }

        public static int ViewCnt_KB( string data )
        {
            try
            {
                char[ ] split = data.ToCharArray( );

                int result =
                ( ( ( byte ) split[ 0 ] ) - 65 ) * 260 + int.Parse( split[ 1 ].ToString( ) ) * 26 + ( ( ( byte ) split[ 2 ] ) - 64 );

                return result;
            }
            catch ( Exception )
            {

                return 0;
            }
        }
    }
    
    public class Kabul_label_data
    {
        public static string GetYear( int Value )
        {
            switch ( Value )
            {
                case 2017:
                    return "A6";
                case 2018:
                    return "A7";
                case 2019:
                    return "A8";
                case 2020:
                    return "A9";
                case 2021:
                    return "B1";
                case 2022:
                    return "B2";
                case 2023:
                    return "B3";
                case 2024:
                    return "B4";
                case 2025:
                    return "B5";
                case 2026:
                    return "B6";
                case 2027:
                    return "B7";
                case 2028:
                    return "B8";
                case 2029:
                    return "B9";

            }

            return "#";

        }

        public static char GetMonth( int Value )
        {
            switch ( Value )
            {
                case 1:
                    return 'A';
                case 2:
                    return 'B';
                case 3:
                    return 'C';
                case 4:
                    return 'D';
                case 5:
                    return 'E';
                case 6:
                    return 'F';
                case 7:
                    return 'G';
                case 8:
                    return 'H';
                case 9:
                    return 'J';
                case 10:
                    return 'K';
                case 11:
                    return 'L';
                case 12:
                    return 'M';

            }

            return '#';

        }

        public static string GetCode( string Modelname , string Project , string Line , bool DayTimeWorking , string Count )
        {
            string result = Modelname + Project + Line;

            result += GetYear( DateTime.Now.Year );
            result += GetMonth( DateTime.Now.Month );
            result += DateTime.Now.Day.ToString( "D2" );

            if ( DayTimeWorking )
                result += "D";
            else
                result += "N";

            return result + Count;
        }

        public static string GetCode( string Modelname , string Project , string Line , bool DayTimeWorking , string Count , int year , int month , int day, string cp1 )
        {
            string result = cp1 + Modelname + Project + Line;

            result += GetYear( year );
            result += GetMonth( month );
            result += day.ToString( "D2" );

            if ( DayTimeWorking )
                result += "D";
            else
                result += "N";

            return result + Count;
        }

        public static string GetDTinfo( bool DayTimeWorking )
        {
            if ( DayTimeWorking )
                return "주간";
            else
                return "야간";
        }
    }

    public sealed class HYUNDAI_label_data_NEW
    {
        public static char GetYear( int Value )
        {
            switch ( Value )
            {
                case 2005:
                    return 'A';
                case 2006:
                    return 'B';
                case 2007:
                    return 'C';
                case 2008:
                    return 'D';
                case 2009:
                    return 'E';
                case 2010:
                    return 'F';
                case 2011:
                    return 'G';
                case 2012:
                    return 'H';
                case 2013:
                    return 'I';
                case 2014:
                    return 'J';
                case 2015:
                    return 'K';
                case 2016:
                    return 'L';
                case 2017:
                    return 'M';
                case 2018:
                    return 'N';
                case 2019:
                    return 'O';
                case 2020:
                    return 'P';
                case 2021:
                    return 'Q';
                case 2022:
                    return 'R';
                case 2023:
                    return 'S';
                case 2024:
                    return 'T';
                case 2025:
                    return 'U';
                case 2026:
                    return 'V';
                case 2027:
                    return 'W';
                case 2028:
                    return 'X';
                case 2029:
                    return 'Y';
                case 2030:
                    return 'Z';

            }

            return '#';

        }

        public static char GetMonth( int Value )
        {
            switch ( Value )
            {
                case 1:
                    return 'A';
                case 2:
                    return 'B';
                case 3:
                    return 'C';
                case 4:
                    return 'D';
                case 5:
                    return 'E';
                case 6:
                    return 'F';
                case 7:
                    return 'G';
                case 8:
                    return 'H';
                case 9:
                    return 'I';
                case 10:
                    return 'J';
                case 11:
                    return 'K';
                case 12:
                    return 'L';
                case 13:
                    return 'M';
                case 14:
                    return 'N';
                case 15:
                    return 'O';
                case 16:
                    return 'P';
                case 17:
                    return 'Q';
                case 18:
                    return 'R';
                case 19:
                    return 'S';
                case 20:
                    return 'T';
                case 21:
                    return 'U';
                case 22:
                    return 'V';
                case 23:
                    return 'W';
                case 24:
                    return 'X';
                case 25:
                    return 'Y';
                case 26:
                    return 'Z';

            }

            return '#';

        }

        public static char GetDay( int Value )
        {
            switch ( Value )
            {
                case 1:
                    return 'A';
                case 2:
                    return 'B';
                case 3:
                    return 'C';
                case 4:
                    return 'D';
                case 5:
                    return 'E';
                case 6:
                    return 'F';
                case 7:
                    return 'G';
                case 8:
                    return 'H';
                case 9:
                    return 'I';
                case 10:
                    return 'J';
                case 11:
                    return 'K';
                case 12:
                    return 'L';
                case 13:
                    return 'M';
                case 14:
                    return 'N';
                case 15:
                    return 'O';
                case 16:
                    return 'P';
                case 17:
                    return 'Q';
                case 18:
                    return 'R';
                case 19:
                    return 'S';
                case 20:
                    return 'T';
                case 21:
                    return 'U';
                case 22:
                    return 'V';
                case 23:
                    return 'W';
                case 24:
                    return 'X';
                case 25:
                    return 'Y';
                case 26:
                    return 'Z';
                case 27:
                    return '1';
                case 28:
                    return '2';
                case 29:
                    return '3';
                case 30:
                    return '4';
                case 31:
                    return '5';

            }

            return '#';

        }

        public static string GetCode( bool DayTimeWorking )
        {
            string result = "";

            result += GetYear( DateTime.Now.Year );
            result += GetMonth( DateTime.Now.Month );
            result += GetDay( DateTime.Now.Day );

            if ( DayTimeWorking )
                result += "A";
            else
                result += "B";

            return result;
        }

        public static string GetCode( bool DayTimeWorking , int year , int month , int day )
        {
            string result = "";

            result += GetYear( year );
            result += GetMonth( month );
            result += GetDay( day );

            if ( DayTimeWorking )
                result += "A";
            else
                result += "B";

            return result;
        }

        public static string GetDTinfo( bool DayTimeWorking )
        {
            if ( DayTimeWorking )
                return "A (주간)";
            else
                return "B (야간)";
        }
    }
    
    /// <summary>
    /// 갑을메탈에서 쓰는 바란싱 장비에서
    /// 최근 바란싱 데이터를 FTP로 긁어옵니다.
    /// </summary>
    public sealed class Kabul_Balancing
    {
        string ServerIP = "";
        string FolderName = "";
        string ID = "";
        string PW = "";
        int TIMEOUT = 0;

        public Kabul_Balancing( string ServerIP , string FolderName , string ID , string PW , int TIMEOUT )
        {
            this.ServerIP = ServerIP;
            this.FolderName = FolderName;
            this.ID = ID;
            this.PW = PW;
            this.TIMEOUT = TIMEOUT;
        }

        /// <summary>
        /// array[0] 이 null인지 검사하십시오.
        /// null인 경우 => 에러
        /// 1이 날짜 2는 시간 7이 무게 8이 각도
        /// 리턴값은 0 1 2 3 가져가셈
        /// </summary>
        /// <returns></returns>
        public string[ ] GetData( )
        {
            string[ ] result = new string[ 4 ];

            FtpWebRequest req = ( FtpWebRequest ) WebRequest.Create( "ftp://" + ServerIP + "/" + FolderName + "/" + DateTime.Now.ToString( "d-M-yyyy" ) + ".TXT" );
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            req.Credentials = new NetworkCredential( ID , PW );
            req.KeepAlive = false;//할짓다하고 연결끊음 중요함.
            req.Timeout = TIMEOUT;

            try
            {
                FtpWebResponse response = ( FtpWebResponse ) req.GetResponse( );

                Stream responseStream = response.GetResponseStream( );
                StreamReader reader = new StreamReader( responseStream );

                string buff = reader.ReadToEnd( );

                reader.Close( );
                response.Close( );

                string[ ] LinData = buff.Split( '\n' );
                string[ ] Data = LinData[ LinData.Length - 2 ].Split( '\t' );
                string[ ] timesplit = Data[ 0 ].Split( '/' );
                string time = timesplit[ 2 ] + "-" + timesplit[ 1 ] + "-" + timesplit[ 0 ];

                result = new string[ ] { 
                time.Trim(),
                Data[1].Trim(),
                Data[7].Trim(),
                Data[8].Trim()
                };

            }
            catch ( Exception )
            {

            }

            return result;

        }

    }

    public class TCPClient_HandyConverter
    {
        string ServerIP = "";
        int ServerPort = 0;
        int ReceiveTimeOut = 0;

        string ClientIP = "";
        int ClientPort = 0;
        LingerOption lingeroption = new LingerOption( true , 0 );

        public delegate void EveHandler( string name , object data );
        public event EveHandler TalkingComm;

        public bool Connected = false;


        public NetworkStream _stream = null;
        private TcpClient mClient;

        public TCPClient_HandyConverter( string ServerIP , int ServerPort , int ReceiveTimeOut )
        {

            this.ServerIP = ServerIP;
            this.ServerPort = ServerPort;
            this.ReceiveTimeOut = ReceiveTimeOut;
            //this.TwoWordComm = TwoWordComm;
            ConnectStart( 0 );
        }

        public TCPClient_HandyConverter( string ServerIP , int ServerPort , int ReceiveTimeOut , string ClientIP , int ClientPort )
        {

            this.ServerIP = ServerIP;
            this.ServerPort = ServerPort;
            this.ReceiveTimeOut = ReceiveTimeOut;
            this.ClientIP = ClientIP;
            this.ClientPort = ClientPort;
            //this.TwoWordComm = TwoWordComm;

            ConnectStart( 0 );
        }

        #region -----# Connect #-----
        //스레드변수 (스레드구성요소 3개)
        //[ FLAG ] [ METHOD ] [ THREAD ]
        private Thread Connect;//스레드
        bool ConnectFlag = false;//Bool Flag
        //스레드함수
        private void ConnectMethod( object param )
        {
            int para = ( int ) param;

            while ( true )
            {
                Thread.Sleep( 1000 );
                if ( ConnectFlag == false )
                    break;

                try
                {

                    if ( Connected == false )//연결끊어졌을때만 함
                    {

                        if ( ClientPort == 0 )
                        {
                            mClient = new TcpClient( );
                            mClient.ReceiveTimeout = ReceiveTimeOut;
                            mClient.Connect( ServerIP , ServerPort );
                            _stream = mClient.GetStream( );
                            Connected = true;

                            CommStart( );//연결되었으니 통신스레드 시작함.
                        }
                        else
                        {
                            System.Net.IPAddress ip = System.Net.IPAddress.Parse( ClientIP );
                            IPEndPoint ipLocalEndPoint = new IPEndPoint( ip , 0 );
                            mClient = new TcpClient( ipLocalEndPoint );

                            mClient.Client.SetSocketOption( SocketOptionLevel.Socket , SocketOptionName.DontLinger , false );
                            mClient.Client.SetSocketOption( SocketOptionLevel.Socket , SocketOptionName.Linger , lingeroption );
                            mClient.Client.SetSocketOption( SocketOptionLevel.Socket , SocketOptionName.KeepAlive , 0 );

                            mClient.ReceiveTimeout = ReceiveTimeOut;
                            mClient.Connect( ServerIP , ServerPort );
                            _stream = mClient.GetStream( );
                            _stream.ReadTimeout = 1000;
                            Connected = true;

                            CommStart( );//연결되었으니 통신스레드 시작함.

                        }


                        TalkingComm( "Connected" , Connected );
                    }



                }
                catch ( Exception )
                {

                }
            }


        }
        //스레드함수
        public void ConnectStart( int param )
        {
            //스레드스타트
            ConnectFlag = true;
            Connect = new Thread( ( new ParameterizedThreadStart( ConnectMethod ) ) );
            Connect.Start( param );
            //스레드스타트
        }
        public void ConnectStop( )
        {

            ConnectFlag = false;

        }
        #endregion

        public void Reboot( )
        {
            List<byte> RebootSignal = new List<byte>( );

            RebootSignal.Add( 0x7C );
            RebootSignal.Add( 0x7C );
            RebootSignal.Add( 0x3E );

            RebootSignal.Add( 0x72 );
            RebootSignal.Add( 0x65 );
            RebootSignal.Add( 0x62 );
            RebootSignal.Add( 0x6F );
            RebootSignal.Add( 0x6F );
            RebootSignal.Add( 0x74 );

            RebootSignal.Add( 0x0D );
            RebootSignal.Add( 0x0A );

            byte[ ] bt = RebootSignal.ToArray( );
            _stream.Write( bt , 0 , bt.Length );
        }

        #region -----# Comm #-----

        private Thread Comm;//스레드
        bool CommFlag = false;//Bool Flag

        byte SignalFlag = 0;

        private void CommMethod( )
        {
            byte[ ] buff = new byte[ 1024 ];


            while ( true )
            {
                Thread.Sleep( 200 );
                if ( CommFlag == false )
                    break;
                try
                {

                    SignalFlag++;
                    if ( SignalFlag >= 30 )
                    {
                        SignalFlag = 0;
                        byte[ ] LiveSignal = new byte[ 1 ] { 0x31 };
                        _stream.Write( LiveSignal , 0 , LiveSignal.Length );
                        //주기적으로 1보냄
                    }

                    if ( _stream.DataAvailable )//왔는지 확인
                    {
                        if ( _stream.ReadByte( ) == 0x02 )//선두확인
                        {
                            string result = "";//초기화

                            while ( _stream.DataAvailable )//읽을수 있는지 확인
                            {
                                int datum = _stream.ReadByte( );//읽음

                                if ( datum == 0x03 )//끝인지확인
                                    break;//끝
                                else
                                    result += ( char ) datum;//추가
                            }

                            TalkingComm( "Data" , result );
                        }
                    }



                }
                catch ( System.IO.IOException )
                {
                    Pause( );
                }
                catch ( Exception )
                {

                }
            }
        }

        //스레드함수
        public void CommStart( )
        {
            //스레드스타트
            CommFlag = true;
            Comm = new Thread( CommMethod );
            Comm.Start( );
            //스레드스타트
        }

        public void CommStop( )
        {
            CommFlag = false;
        }

        private void Pause( )
        {
            try
            {
                Connected = false;

                if ( _stream != null )
                {
                    _stream.Close( );
                }

                if ( mClient != null )
                {
                    mClient.Close( );
                }

                CommStop( );

            }
            catch ( Exception exc )
            {

            }

            TalkingComm( "DisConnected" , Connected );
        }

        public void Dispose( )
        {
            try
            {
                Pause( );

                ConnectStop( );
                Connect.Abort( );
                Comm.Abort( );
            }
            catch ( Exception )
            {

            }
        }

        public void Disconnect( )
        {
            try
            {
                Pause( );

                ConnectStop( );
            }
            catch ( Exception )
            {

            }
        }
        #endregion

    }

    public class HYUNDAI_label_data
    {
        public static char GetYear( int Value )
        {
            switch ( Value )
            {
                case 2005:
                    return 'A';
                case 2006:
                    return 'B';
                case 2007:
                    return 'C';
                case 2008:
                    return 'D';
                case 2009:
                    return 'E';
                case 2010:
                    return 'F';
                case 2011:
                    return 'G';
                case 2012:
                    return 'H';
                case 2013:
                    return 'I';
                case 2014:
                    return 'J';
                case 2015:
                    return 'K';
                case 2016:
                    return 'L';
                case 2017:
                    return 'M';
                case 2018:
                    return 'N';
                case 2019:
                    return 'O';
                case 2020:
                    return 'P';
                case 2021:
                    return 'Q';
                case 2022:
                    return 'R';
                case 2023:
                    return 'S';
                case 2024:
                    return 'T';
                case 2025:
                    return 'U';
                case 2026:
                    return 'V';
                case 2027:
                    return 'W';
                case 2028:
                    return 'X';
                case 2029:
                    return 'Y';
                case 2030:
                    return 'Z';

            }

            return '#';

        }

        public static char GetMonth( int Value )
        {
            switch ( Value )
            {
                case 1:
                    return 'A';
                case 2:
                    return 'B';
                case 3:
                    return 'C';
                case 4:
                    return 'D';
                case 5:
                    return 'E';
                case 6:
                    return 'F';
                case 7:
                    return 'G';
                case 8:
                    return 'H';
                case 9:
                    return 'I';
                case 10:
                    return 'J';
                case 11:
                    return 'K';
                case 12:
                    return 'L';
                case 13:
                    return 'M';
                case 14:
                    return 'N';
                case 15:
                    return 'O';
                case 16:
                    return 'P';
                case 17:
                    return 'Q';
                case 18:
                    return 'R';
                case 19:
                    return 'S';
                case 20:
                    return 'T';
                case 21:
                    return 'U';
                case 22:
                    return 'V';
                case 23:
                    return 'W';
                case 24:
                    return 'X';
                case 25:
                    return 'Y';
                case 26:
                    return 'Z';

            }

            return '#';

        }

        public static char GetDay( int Value )
        {
            switch ( Value )
            {
                case 1:
                    return 'A';
                case 2:
                    return 'B';
                case 3:
                    return 'C';
                case 4:
                    return 'D';
                case 5:
                    return 'E';
                case 6:
                    return 'F';
                case 7:
                    return 'G';
                case 8:
                    return 'H';
                case 9:
                    return 'I';
                case 10:
                    return 'J';
                case 11:
                    return 'K';
                case 12:
                    return 'L';
                case 13:
                    return 'M';
                case 14:
                    return 'N';
                case 15:
                    return 'O';
                case 16:
                    return 'P';
                case 17:
                    return 'Q';
                case 18:
                    return 'R';
                case 19:
                    return 'S';
                case 20:
                    return 'T';
                case 21:
                    return 'U';
                case 22:
                    return 'V';
                case 23:
                    return 'W';
                case 24:
                    return 'X';
                case 25:
                    return 'Y';
                case 26:
                    return 'Z';
                case 27:
                    return '1';
                case 28:
                    return '2';
                case 29:
                    return '3';
                case 30:
                    return '4';
                case 31:
                    return '5';

            }

            return '#';

        }

        public static string GetCode( bool DayTimeWorking )
        {
            string result = "";

            result += GetYear( DateTime.Now.Year );
            result += GetMonth( DateTime.Now.Month );
            result += GetDay( DateTime.Now.Day );

            if ( DayTimeWorking )
                result += "A";
            else
                result += "D";

            return result;
        }

        public static string GetCode( bool DayTimeWorking , int year , int month , int day )
        {
            string result = "";

            result += GetYear( year );
            result += GetMonth( month );
            result += GetDay( day );

            if ( DayTimeWorking )
                result += "A";
            else
                result += "D";

            return result;
        }

        public static string GetDTinfo( bool DayTimeWorking )
        {
            if ( DayTimeWorking )
                return "A (주간)";
            else
                return "D (야간)";
        }
    }
}
