using Ken2.Communication;
using Ken2.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace KB_Data_V2

{
    public class TCPClient_Monitor
    {
        LingerOption lingeroption = new LingerOption( true , 0 );

        //---------------↓ 통신관련 ↓---------------┐
        string ServerIP = "";
        int ServerPort = 0;
        int ReceiveTimeOut = 0;
        string ClientIP = "";
        int ClientPort = 0;
        //---------------↑ 통신관련 ↑---------------┘

        public delegate void EveHandler( string name , object data );
        public event EveHandler TalkingComm;

        public bool Connected = false;

        Form1 mainform;

        public NetworkStream _stream = null;
        private TcpClient mClient;

        public void Dispose( )
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

        public void SendString( string str )
        {
            try
            {
                byte[ ] buff = Ken2.Communication.DataChange_K.StringToByteArr( str );
                _stream.Write( buff , 0 , buff.Length );
            }
            catch ( Exception )
            {

            }
        }

        public TCPClient_Monitor( string ServerIP , int ServerPort , int ReceiveTimeOut , Form1 mainform )
        {

            this.ServerIP = ServerIP;
            this.ServerPort = ServerPort;
            this.ReceiveTimeOut = ReceiveTimeOut;
            this.mainform = mainform;
            ConnectStart( 0 );

        }

        public TCPClient_Monitor( string ServerIP , int ServerPort , int ReceiveTimeOut , string ClientIP , int ClientPort )
        {

            this.ServerIP = ServerIP;
            this.ServerPort = ServerPort;
            this.ReceiveTimeOut = ReceiveTimeOut;
            this.ClientIP = ClientIP;
            this.ClientPort = ClientPort;

            ConnectStart( 0 );

        }

        #region -----# Connect #-----
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
            Connect.Abort( );

            ConnectFlag = false;

        }
        #endregion

        /// <summary>
        /// 받은 데이터에서 상태에 해당하는 데이터만 추출해 옴
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public int ViewPrintStatus( string Data )
        {
            try
            {

                string[ ] split = Data.Split( '\n' );
                string[ ] buff0 = split[ 0 ].Split( ',' );
                string[ ] buff1 = split[ 1 ].Split( ',' );

                if ( buff0[ 2 ] == "1" )
                    return 2;
                if ( buff1[ 7 ] == "1" )
                    return 1;

            }
            catch ( Exception )
            {


            }
            return 0;
        }

        #region -----# Comm #-----

        private Thread Comm;//스레드
        bool CommFlag = false;//Bool Flag

        private void CommMethod( )
        {
            byte[ ] buff = new byte[ 1024 ];
            int length = 0;


            while ( CommFlag )
            {
                try
                {

                    SendString(
                        mainform.ModelNamelbl.Text + "~" + mainform.ModelNamelbl.Text + "~" + mainform.ModelNamelbl1.Text + "~" + 
                        mainform.QuantityData[0] + "~" + mainform.QuantityData[1] + "~" + mainform.QuantityData[2] + "~" + mainform.QuantityData[3] + "~" +
                        mainform.QuantityData[4] + "~" + mainform.QuantityData[5] + "~" + //mainform.QuantityData[6] + "~" + mainform.QuantityData[7] + "~" +
                        mainform.dgvDE0.Rows[1].Cells[1].Value.ToString() + "~" + mainform.dgvDE0.Rows[1].Cells[2].Value.ToString() + "~" + mainform.dgvDE0.Rows[1].Cells[3].Value.ToString() //+ "~" + mainform.dgvDE0.Rows[1].Cells[4].Value.ToString()
                        + "@0"
                        );

                    //SendString(
                    //    mainform.ModelNamelbl.Text + "~" + mainform.ModelNamelbl.Text + "~" + mainform.ModelNamelbl1.Text + "~" + mainform.ModelNamelbl2.Text + "~" +
                    //    mainform.QuantityData[ 0 ] + "~" + mainform.QuantityData[ 1 ] + "~" + mainform.QuantityData[ 2 ] + "~" + mainform.QuantityData[ 3 ] + "~" +
                    //    mainform.QuantityData[ 4 ] + "~" + mainform.QuantityData[ 5 ] + "~" + mainform.QuantityData[ 6 ] + "~" + mainform.QuantityData[ 7 ] + "~" +
                    //    mainform.dgvDE0.Rows[ 1 ].Cells[ 1 ].Value.ToString( ) + "~" + mainform.dgvDE0.Rows[ 1 ].Cells[ 2 ].Value.ToString( ) + "~" + mainform.dgvDE0.Rows[ 1 ].Cells[ 3 ].Value.ToString( ) + "~" + mainform.dgvDE0.Rows[ 1 ].Cells[ 4 ].Value.ToString( )
                    //    + "@0"
                    //    );

                    length = _stream.Read( buff , 0 , buff.Length );
                    
                }
                catch ( System.IO.IOException )
                {
                    Pause( );
                }
                catch ( Exception exc )
                {

                }

                Thread.Sleep( 2000 );//2초마다 한번씩
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

        /// <summary>
        /// 연결 상태유지 및 재 연결 시도.
        /// 통신은 중단.
        /// </summary>
        private void Pause( )
        {
            try
            {
                Connected = false;
                CommStop( );

                if ( _stream != null )
                {
                    _stream.Close( );
                }

                if ( mClient != null )
                {
                    mClient.Close( );
                }

                TalkingComm( "DisConnected" , Connected );

            }
            catch ( Exception exc )
            {

            }

        }

        #endregion

    }


    public class TCPClient_LabelPrinter
    {
        LingerOption lingeroption = new LingerOption( true , 0 );

        //---------------↓ 통신관련 ↓---------------┐
        string ServerIP = "";
        int ServerPort = 0;
        int ReceiveTimeOut = 0;
        string ClientIP = "";
        int ClientPort = 0;
        //---------------↑ 통신관련 ↑---------------┘

        public delegate void EveHandler( string name , object data );
        public event EveHandler TalkingComm;

        public bool Connected = false;
        public int PrinterStatus = 0;
        //프린터의 상태 0 = 출력된 라벨 없음
        //1 = 출력된 라벨 있음
        //2 = 에러

        public NetworkStream _stream = null;
        private TcpClient mClient;

        public void Dispose( )
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

        public void SendString( string str )
        {
            try
            {
                byte[ ] buff = Ken2.Communication.DataChange_K.StringToByteArr( str );
                _stream.Write( buff , 0 , buff.Length );
            }
            catch ( Exception )
            {

            }
        }

        public TCPClient_LabelPrinter( string ServerIP , int ServerPort , int ReceiveTimeOut )
        {

            this.ServerIP = ServerIP;
            this.ServerPort = ServerPort;
            this.ReceiveTimeOut = ReceiveTimeOut;

            ConnectStart( 0 );

        }

        public TCPClient_LabelPrinter( string ServerIP , int ServerPort , int ReceiveTimeOut , string ClientIP , int ClientPort )
        {

            this.ServerIP = ServerIP;
            this.ServerPort = ServerPort;
            this.ReceiveTimeOut = ReceiveTimeOut;
            this.ClientIP = ClientIP;
            this.ClientPort = ClientPort;

            ConnectStart( 0 );

        }

        #region -----# Connect #-----
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
            Connect.Abort( );

            ConnectFlag = false;

        }
        #endregion

        /// <summary>
        /// 받은 데이터에서 상태에 해당하는 데이터만 추출해 옴
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public int ViewPrintStatus( string Data )
        {
            try
            {

                string[ ] split = Data.Split( '\n' );
                string[ ] buff0 = split[ 0 ].Split( ',' );
                string[ ] buff1 = split[ 1 ].Split( ',' );

                if ( buff0[ 2 ] == "1" )
                    return 2;
                if ( buff1[ 7 ] == "1" )
                    return 1;

            }
            catch ( Exception )
            {


            }
            return 0;
        }

        #region -----# Comm #-----

        private Thread Comm;//스레드
        bool CommFlag = false;//Bool Flag

        private void CommMethod( )
        {
            byte[ ] buff = new byte[ 1024 ];
            int length = 0;

            string status_cmd = "^XA^MMP~HS^XZ";//상태물어보기 명령어

            while ( CommFlag )
            {
                try
                {
                    SendString( status_cmd );

                    while ( _stream.DataAvailable )
                    {
                        length = _stream.Read( buff , 0 , buff.Length );
                        string print_data = Encoding.ASCII.GetString( buff , 0 , length );

                        int print_sta = ViewPrintStatus( print_data );

                        switch ( print_sta )
                        {
                            case 0:
                                //없다.

                                PrinterStatus = 0;

                                break;
                            case 1:
                                //남아있다.

                                PrinterStatus = 1;

                                break;
                            case 2:
                                //에러났다.

                                PrinterStatus = 2;

                                break;
                        }

                    }

                }
                catch ( System.IO.IOException )
                {
                    Pause( );
                }
                catch ( Exception exc )
                {

                }

                Thread.Sleep( 2000 );//2초마다 한번씩
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

        /// <summary>
        /// 연결 상태유지 및 재 연결 시도.
        /// 통신은 중단.
        /// </summary>
        private void Pause( )
        {
            try
            {
                Connected = false;
                CommStop( );

                if ( _stream != null )
                {
                    _stream.Close( );
                }

                if ( mClient != null )
                {
                    mClient.Close( );
                }


            }
            catch ( Exception exc )
            {

            }

            TalkingComm( "DisConnected" , Connected );
        }

        #endregion

    }

    public class TCPClient_PLC1
    {
        string ServerIP = "";
        int ServerPort = 0;
        int ReceiveTimeOut = 0;
        LingerOption lingeroption = new LingerOption( true , 0 );

        public delegate void EveHandler( string name , object data , string data2 , string data3 , string data4 , string data5 , string data6 , string data7 , string data8 , string data9 , string data10 , string data11 , string data12 );
        public event EveHandler TalkingComm;

        public bool Connected = false;
        NetworkStream _stream = null;
        private TcpClient mClient;
        Form1 mainform;

        string ClientIP = "";
        int ClientPort = 0;


        public TCPClient_PLC1( string ServerIP , int ServerPort , int ReceiveTimeOut , Form1 mainform )
        {

            this.ServerIP = ServerIP;
            this.ServerPort = ServerPort;
            this.ReceiveTimeOut = ReceiveTimeOut;
            this.mainform = mainform;
            ConnectStart( );

        }

        public TCPClient_PLC1( string ServerIP , int ServerPort , int ReceiveTimeOut , string ClientIP , int ClientPort , Form1 mainform )
        {
            this.ServerIP = ServerIP;
            this.ServerPort = ServerPort;
            this.ReceiveTimeOut = ReceiveTimeOut;
            this.mainform = mainform;
            this.ClientIP = ClientIP;
            this.ClientPort = ClientPort;

            ConnectStart( );
        }


        object tcplock = new object( );



        public void MCWrite_Clear( int offset , int length )
        {

            lock ( tcplock )
            {

                byte[ ] ReceiveData = new byte[ 1000 ];//데이터받음

                try
                {
                    _stream.Write( Ken2.Communication.MCProtocolCmd_K.Write_W_Clear( offset , length ) , 0 , Ken2.Communication.MCProtocolCmd_K.Write_W_Clear( offset , length ).Length );

                }
                catch ( IOException )//데이터를전송할수가없어서 plc와 연결을 끊기. 연결이 끊어지면 계속 연결시도함.
                {
                    Pause( );

                }

                try
                {
                    _stream.Read( ReceiveData , 0 , ReceiveData.Length );//리시브데이터에 집어넣음
                    _stream.Flush( );

                }
                catch ( IOException )
                {

                }



            }
        }

        public void MCWriteString( int offset , string str )
        {
            lock ( tcplock )
            {
                byte[ ] ReceiveData = new byte[ 100 ];//데이터받음

                try
                {
                    _stream.Write( Ken2.Communication.MCProtocolCmd_K.Write_W_reg( offset , str ) , 0 , Ken2.Communication.MCProtocolCmd_K.Write_W_reg( offset , str ).Length );
                }
                catch ( IOException )//데이터를전송할수가없어서 plc와 연결을 끊기. 연결이 끊어지면 계속 연결시도함.
                {
                    Pause( );
                }

                try
                {
                    _stream.Read( ReceiveData , 0 , ReceiveData.Length );//리시브데이터에 집어넣음
                    _stream.Flush( );

                }
                catch ( IOException )
                {

                }
            }

        }

        object ReadLock = new object( );

        public int[ ] MCRead_By_Offsets( int offset , int num )
        {
            lock ( tcplock )
            {
                byte[ ] ReceiveData = new byte[ 2000 ];//데이터받음
                byte[ ] Command_Byte = Ken2.Communication.MCProtocolCmd_K.Read_Dreg( offset , num );
                try
                {
                    _stream.Write( Command_Byte , 0 , Command_Byte.Length );
                }
                catch ( IOException )//데이터를전송할수가없어서 plc와 연결을 끊기. 연결이 끊어지면 계속 연결시도함.
                {
                    Pause( );
                }

                try
                {
                    _stream.Read( ReceiveData , 0 , ReceiveData.Length );//리시브데이터에 집어넣음
                    _stream.Flush( );
                }
                catch ( IOException )
                {

                }

                return Ken2.Communication.MCProtocolCmd_K.View_MCData( ReceiveData );
            }
        }

        public byte[ ] MCRead( int offset , int num )
        {
            lock ( tcplock )
            {
                byte[ ] ReceiveData = new byte[ 2000 ];//데이터받음
                byte[ ] Command_Byte = Ken2.Communication.MCProtocolCmd_K.Read_Dreg( offset , num );
                try
                {
                    _stream.Write( Command_Byte , 0 , Command_Byte.Length );
                }
                catch ( IOException )//데이터를전송할수가없어서 plc와 연결을 끊기. 연결이 끊어지면 계속 연결시도함.
                {
                    Pause( );
                }

                try
                {
                    _stream.Read( ReceiveData , 0 , ReceiveData.Length );//리시브데이터에 집어넣음
                    _stream.Flush( );
                }
                catch ( IOException )
                {

                }

                return Ken2.Communication.MCProtocolCmd_K.View_MCData_Byte( ReceiveData );
            }
        }

        public void MCWrite( int offset , int data )
        {

            lock ( tcplock )
            {
                byte[ ] ReceiveData = new byte[ 2000 ];//데이터받음
                byte[ ] Command_Byte = Ken2.Communication.MCProtocolCmd_K.Write_Dreg( offset , data );


                try
                {
                    _stream.Write( Command_Byte , 0 , Command_Byte.Length );

                }
                catch ( IOException )//데이터를전송할수가없어서 plc와 연결을 끊기. 연결이 끊어지면 계속 연결시도함.
                {
                    Pause( );

                }

                try
                {
                    _stream.Read( ReceiveData , 0 , ReceiveData.Length );//리시브데이터에 집어넣음
                    _stream.Flush( );

                }
                catch ( IOException )
                {

                }



            }
        }

        int Start = 5000;

        int CalcByte( int Offset )
        {
            int result = Offset - Start;
            return result * 2;
        }

        string DecimalToBinary( int dec )
        {
            string s = Convert.ToString( dec , 2 ).PadLeft( 16 , '0' );
            return s;
        }

        #region -----# Connect #-----
        private Thread Connect;
        bool ConnectFlag = false;//Bool Flag

        private void ConnectMethod( )
        {
            while ( ConnectFlag )
            {

                try
                {

                    if ( Connected == false )//연결끊어졌을때만 함
                    {
                        if ( ClientIP.Equals( "" ) )
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

                        //( "ServerConnected" , Connected );
                    }

                }
                catch ( Exception )
                {

                }

                Thread.Sleep( 1000 );

            }

        }
        //스레드함수
        public void ConnectStart( )
        {
            //스레드스타트
            ConnectFlag = true;
            Connect = new Thread( ConnectMethod );
            Connect.Start( );
            //스레드스타트
        }
        public void ConnectStop( )
        {
            Connect.Abort( );
            //스레드종료
            ConnectFlag = false;

            //스레드종료
        }
        #endregion

        #region -----# Comm #-----

        private Thread Comm;//스레드
        bool CommFlag = false;//Bool Flag

        double RoundUp( string d_value , int n_point )
        {
            double bf = double.Parse( d_value );
            double res = Math.Round( bf , n_point );

            return res;
        }

        string ByteToDecision( byte bt )
        {
            if ( bt == 1 )
                return "OK";
            else if ( bt == 2 )
                return "NG";
            else
                return "";
        }

        public static string PLCValue( string data , int word_num )
        {
            try
            {
                long buff = long.Parse( data );


                if ( word_num == 1 )
                {

                    if ( buff > 32767 )
                        buff = buff - 65536;


                    return buff.ToString( );
                }
                else if ( word_num == 2 )
                {
                    long diff = 4294967296;

                    if ( buff > 2147483647 )
                        buff = buff - diff‬;

                    return buff.ToString( );

                }

            }
            catch ( Exception )
            {
                try
                {
                    if ( data.Equals( "OK" ) || data.Equals( "NG" ) )
                        return data;
                }
                catch ( Exception )
                {

                }
            }
            return "0";
        }

        public string DecimalPoint( string str , int point )
        {
            if ( point < 0 )
                return "0";

            int div = 10;

            for ( int i = 0 ; i < point - 1 ; i++ )
            {
                div *= 10;

            }

            string str_ = ( double.Parse( str ) / div ).ToString( "N" + point.ToString( ) );

            return str_;
        }


        //tttttttttttttttttttttttttttttttttttt
        private void CommMethod( )
        {
            PulseDetector Save1 = new PulseDetector( );
            PulseDetector Save2 = new PulseDetector( );
            PulseDetector Save3 = new PulseDetector( );
            PulseDetector LabelPrint = new PulseDetector( );

            PulseDetector BarcodeCheck = new PulseDetector( );
            PulseDetector BarcodeCheck2 = new PulseDetector( );

            PulseDetector Balance = new PulseDetector( );
            PulseDetector Balance2 = new PulseDetector( );
            PulseDetector Balance3 = new PulseDetector( );


            PulseDetector ManualPrint = new PulseDetector( );

            CountPlay flip = new CountPlay( );

            CountPlay quantity = new CountPlay( );
            Stopwatch stopwatch = new Stopwatch();

            string Mainpath = "Log";
            while ( CommFlag )
            {
                try
                {
                    stopwatch.Start(); // 시간 측정 시작

                    if ( mainform.Viewdatachk.Checked )
                    {
                        int[ ] commdata = MCRead_By_Offsets( 5000 , 300 );//5000번지 300워드

                        if ( TalkingComm != null ) TalkingComm( "CommData" , commdata , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" );
                    }

                    byte[ ] buff = MCRead( 5000 , 300 );//600개 바이트


                    //if ( mainform.AllReady )//Ready신호
                    //{

                    if (flip.OnePlay(1))
                        MCWrite(6000, 0);
                    else
                        MCWrite(6000, 1);
                    //}

                    if ( mainform.CurrentModelNum != buff[ CalcByte( 5000 ) ] )
                        if ( TalkingComm != null ) TalkingComm( "ModelChange" , buff[ CalcByte( 5000 ) ] , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" );

                    if ( quantity.OnePlay( 10 ) )
                    {
                        int offset1 = 5070;
                        string data1 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( offset1 ) ] , buff[ CalcByte( offset1 ) + 1 ] , buff[ CalcByte( offset1 ) + 2 ] , buff[ CalcByte( offset1 ) + 3 ] ).ToString( );
                        data1 = PLCValue( data1 , 2 );
                        mainform.QuantityData[ 0 ] = data1;

                        int offset2 = 5072;
                        string data2 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( offset2 ) ] , buff[ CalcByte( offset2 ) + 1 ] , buff[ CalcByte( offset2 ) + 2 ] , buff[ CalcByte( offset2 ) + 3 ] ).ToString( );
                        data2 = PLCValue( data2 , 2 );
                        mainform.QuantityData[ 1 ] = data2;

                        int offset3 = 5270;
                        string data3 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( offset3 ) ] , buff[ CalcByte( offset3 ) + 1 ] , buff[ CalcByte( offset3 ) + 2 ] , buff[ CalcByte( offset3 ) + 3 ] ).ToString( );
                        data3 = PLCValue( data3 , 2 );
                        mainform.QuantityData[ 2 ] = data3;

                        int offset4 = 5272;
                        string data4 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( offset4 ) ] , buff[ CalcByte( offset4 ) + 1 ] , buff[ CalcByte( offset4 ) + 2 ] , buff[ CalcByte( offset4 ) + 3 ] ).ToString( );
                        data4 = PLCValue( data4 , 2 );
                        mainform.QuantityData[ 3 ] = data4;
                    }

                    //#C140 라벨 부착부 DATA 읽기 요구
                    if ( Save1.Detect( buff[ CalcByte( 5010 ) ] , 1 ) )
                    {
                        string Impeller_bcr = DataChange_K.ByteToString_Clean_0x00( buff , CalcByte( 5021 ) , 18 ).Trim( );
                        string Upper_bcr = DataChange_K.ByteToString_Clean_0x00( buff , CalcByte( 5031 ) , 18 ).Trim( );
                        string bcr_decision = DecimalToBinary( buff[ CalcByte( 5012 ) ] + buff[ CalcByte( 5012 ) + 1 ] * 256 );//5012 바이너리 결과 c1

                        //string Impeller_bcr = DataChange_K.ByteToString_Clean_0x00(buff, CalcByte(5021), 18).Trim();

                        string data1 = ( buff[ CalcByte( 5044 ) ] + buff[ CalcByte( 5044 ) + 1 ] * 256 ).ToString( );
                        //#30 UPPER CASE 공급부 PCB 측정값 최대
                        //c14

                        string data2 = ( buff[ CalcByte( 5046 ) ] + buff[ CalcByte( 5046 ) + 1 ] * 256 ).ToString( );
                        //#50 스페이서 측정값
                        //c15

                        string data3 = ( buff[ CalcByte( 5047 ) ] + buff[ CalcByte( 5047 ) + 1 ] * 256 ).ToString( );
                        //#90 스페이서 측정값
                        //c16

                        string data4 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 5050 ) ] , buff[ CalcByte( 5050 ) + 1 ] , buff[ CalcByte( 5050 ) + 2 ] , buff[ CalcByte( 5050 ) + 3 ] ).ToString( );
                        //#60 베어링압입 결과 거리
                        //c17

                        string data5 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 5052 ) ] , buff[ CalcByte( 5052 ) + 1 ] , buff[ CalcByte( 5052 ) + 2 ] , buff[ CalcByte( 5052 ) + 3 ] ).ToString( );
                        //#60 베어링압입 결과 하중
                        //c18

                        string data6 = ( buff[ CalcByte( 5054 ) ] + buff[ CalcByte( 5054 ) + 1 ] * 256 ).ToString( );
                        //#110 스토퍼 높이 측정값
                        //c180

                        //data7 판정 data8 측정값 추가

                        string data7 = ByteToDecision( buff[ CalcByte( 5056 ) ] );
                        string data8 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 5058 ) ] , buff[ CalcByte( 5058 ) + 1 ] , buff[ CalcByte( 5058 ) + 2 ] , buff[ CalcByte( 5058 ) + 3 ] ).ToString( );


                        data1 = PLCValue( data1 , 1 );  
                        data2 = PLCValue( data2 , 1 );  
                        data3 = PLCValue( data3 , 1 );  

                        data4 = PLCValue( data4 , 2 );
                        data5 = PLCValue( data5 , 2 );
                        data6 = PLCValue( data6 , 1 );

                        //  추가
                        data7 = PLCValue(data7, 1);
                        data8 = PLCValue(data8, 2);

                        data1 = DecimalPoint( data1 , 3 );
                        data2 = DecimalPoint( data2 , 3 );
                        data3 = DecimalPoint( data3 , 3 );

                        data4 = DecimalPoint( data4 , 3 );
                        data5 = DecimalPoint( data5 , 1 );
                        data6 = DecimalPoint( data6 , 2 );

                        //  추가
                        data8 = DecimalPoint( data8 , 3 );                        

                        data1 = RoundUp( data1 , 2 ).ToString( );
                        data2 = RoundUp( data2 , 2 ).ToString( );
                        data3 = RoundUp( data3 , 2 ).ToString( );

                        data4 = RoundUp( data4 , 2 ).ToString( );
                        data5 = RoundUp( data5 , 2 ).ToString( );
                        data6 = RoundUp( data6 , 3 ).ToString( );

                        //  추가
                        data8 = RoundUp( data8 , 3 ).ToString( );

                        //          // data7= 특성저항 판정, data8= 특성 저항 검사 측정값
                        //          //  string data11 = ByteToDecision(buff[CalcByte(5058)]);
                        //          //string data12 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word(buff[CalcByte(5056)], buff[CalcByte(5056) + 1], buff[CalcByte(5056) + 2], buff[CalcByte(5056) + 3]).ToString();
                        //          string data11 = (buff[CalcByte(5054)] + buff[CalcByte(5054) + 1] * 256).ToString();
                        //          string data12 = (buff[CalcByte(5054)] + buff[CalcByte(5054) + 1] * 256).ToString();
                        //          data11 = PLCValue(data11, 1);
                        //          data12 = PLCValue(data12, 3);
                        //          
                        //          data12 = DecimalPoint(data12, 2);
                        //          data12 = RoundUp(data12, 2).ToString();

                        if ( TalkingComm != null ) TalkingComm( "Save1" , "" , Impeller_bcr , Upper_bcr , bcr_decision ,
                            data1, //5 UPPER CASE 공급부 PCB 측정값 최대
                            data2, //6 50 스페이서 측정값
                            data3 , //7 
                            data4 , //8                      
                            data5 , //9
                            data6 , //10
                            data7,  //11
                            data8  //12
                            );
                    }

                    //#C140 라벨 출력 요구
                    if ( LabelPrint.Detect( buff[ CalcByte( 5014 ) ] , 1 ) )
                    {
                        if ( TalkingComm != null ) TalkingComm( "LabelPrint" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" );
                    }

                    // 라벨 수동 출력
                    if ( ManualPrint.Detect( buff [ CalcByte( 5015 ) ], 1 ) )    //  1st데이터 저장 지령
                    {
                        if ( TalkingComm != null ) TalkingComm( "LabelPrintManual", "", "", "", "", "", "", "", "", "", "", "", "" );
                    }
                    else
                    {
                        //Save1Cp.ResetCount( );
                    }

                    //#C150 조립 완성품 배출부 바코드 판정 결과 요구
                    if ( BarcodeCheck.Detect( buff[ CalcByte( 5100 ) ] , 1 ) )
                    {
                        string bcr = DataChange_K.ByteToString_Clean_0x00( buff , CalcByte( 5103 ) , 18 ).Trim( );

                        if ( TalkingComm != null ) TalkingComm( "BarcodeCheck" , "" , bcr , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" );
                    }

                    //#D20  조립 완성품 배출 로더 바코드 판정 결과 요구
                    if ( BarcodeCheck2.Detect( buff[ CalcByte( 5120 ) ] , 1 ) )
                    {
                        string bcr = DataChange_K.ByteToString_Clean_0x00( buff , CalcByte( 5123 ) , 18 ).Trim( );

                        if ( TalkingComm != null ) TalkingComm( "BarcodeCheck2" , "" , bcr , "" , "" , "" , "" , "" , "" , "" , "" , "" , "" );
                    }

                    //#D31  밸런스 MC-1 DATA 읽기 요구
                    if ( Balance.Detect( buff[ CalcByte( 5140 ) ] , 1 ) )
                    {
                        string bcr = DataChange_K.ByteToString_Clean_0x00( buff , CalcByte( 5144 ) , 18 ).Trim( );

                        string data1 = ByteToDecision( buff[ CalcByte( 5153 ) ] );
                        string data2 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 5154 ) ] , buff[ CalcByte( 5154 ) + 1 ] , buff[ CalcByte( 5154 ) + 2 ] , buff[ CalcByte( 5154 ) + 3 ] ).ToString( );
                        string data3 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 5156 ) ] , buff[ CalcByte( 5156 ) + 1 ] , buff[ CalcByte( 5156 ) + 2 ] , buff[ CalcByte( 5156 ) + 3 ] ).ToString( );
                        string data4 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 5158 ) ] , buff[ CalcByte( 5158 ) + 1 ] , buff[ CalcByte( 5158 ) + 2 ] , buff[ CalcByte( 5158 ) + 3 ] ).ToString( );
                        string data5 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 5160 ) ] , buff[ CalcByte( 5160 ) + 1 ] , buff[ CalcByte( 5160 ) + 2 ] , buff[ CalcByte( 5160 ) + 3 ] ).ToString( );

                        data2 = PLCValue( data2 , 2 );
                        data3 = PLCValue( data3 , 2 );
                        data4 = PLCValue( data4 , 2 );
                        data5 = PLCValue( data5 , 2 );

                        data2 = DecimalPoint( data2 , 4 );
                        data3 = DecimalPoint( data3 , 4 );
                        data4 = DecimalPoint( data4 , 4 );
                        data5 = DecimalPoint( data5 , 4 );

                        data2 = RoundUp( data2 , 2 ).ToString( );
                        data3 = RoundUp( data3 , 2 ).ToString( );
                        data4 = RoundUp( data4 , 2 ).ToString( );
                        data5 = RoundUp( data5 , 2 ).ToString( );

                        if ( TalkingComm != null ) TalkingComm( "Balance" , "" , bcr , data1 , data2 , data3 , data4 , data5 , "" , "" , "" , "" , "" );
                    }

                    //#D32  밸런스 M2-2 DATA 읽기 요구
                    if ( Balance2.Detect( buff[ CalcByte( 5180 ) ] , 1 ) )
                    {
                        string bcr = DataChange_K.ByteToString_Clean_0x00( buff , CalcByte( 5184 ) , 18 ).Trim( );

                        string data1 = ByteToDecision( buff[ CalcByte( 5193 ) ] );
                        string data2 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 5194 ) ] , buff[ CalcByte( 5194 ) + 1 ] , buff[ CalcByte( 5194 ) + 2 ] , buff[ CalcByte( 5194 ) + 3 ] ).ToString( );
                        string data3 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 5196 ) ] , buff[ CalcByte( 5196 ) + 1 ] , buff[ CalcByte( 5196 ) + 2 ] , buff[ CalcByte( 5196 ) + 3 ] ).ToString( );
                        string data4 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 5198 ) ] , buff[ CalcByte( 5198 ) + 1 ] , buff[ CalcByte( 5198 ) + 2 ] , buff[ CalcByte( 5198 ) + 3 ] ).ToString( );
                        string data5 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 5200 ) ] , buff[ CalcByte( 5200 ) + 1 ] , buff[ CalcByte( 5200 ) + 2 ] , buff[ CalcByte( 5200 ) + 3 ] ).ToString( );

                        data2 = PLCValue( data2 , 2 );
                        data3 = PLCValue( data3 , 2 );
                        data4 = PLCValue( data4 , 2 );
                        data5 = PLCValue( data5 , 2 );

                        data2 = DecimalPoint( data2 , 4 );
                        data3 = DecimalPoint( data3 , 4 );
                        data4 = DecimalPoint( data4 , 4 );
                        data5 = DecimalPoint( data5 , 4 );

                        data2 = RoundUp( data2 , 2 ).ToString( );
                        data3 = RoundUp( data3 , 2 ).ToString( );
                        data4 = RoundUp( data4 , 2 ).ToString( );
                        data5 = RoundUp( data5 , 2 ).ToString( );

                        if ( TalkingComm != null ) TalkingComm( "Balance2" , "" , bcr , data1 , data2 , data3 , data4 , data5 , "" , "" , "" , "" , "" );
                    }

                    //#D33  밸런스 MC-3 DATA 읽기 요구 1차

                    if ( Balance3.Detect( buff[ CalcByte( 5220 ) ] , 1 ) )
                    {
                        string bcr = DataChange_K.ByteToString_Clean_0x00( buff , CalcByte( 5224 ) , 18 ).Trim( );

                        string data1 = ByteToDecision( buff[ CalcByte( 5233 ) ] );
                        string data2 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 5234 ) ] , buff[ CalcByte( 5234 ) + 1 ] , buff[ CalcByte( 5234 ) + 2 ] , buff[ CalcByte( 5234 ) + 3 ] ).ToString( );
                        string data3 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 5236 ) ] , buff[ CalcByte( 5236 ) + 1 ] , buff[ CalcByte( 5236 ) + 2 ] , buff[ CalcByte( 5236 ) + 3 ] ).ToString( );
                        string data4 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 5238 ) ] , buff[ CalcByte( 5238 ) + 1 ] , buff[ CalcByte( 5238 ) + 2 ] , buff[ CalcByte( 5238 ) + 3 ] ).ToString( );
                        string data5 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 5240 ) ] , buff[ CalcByte( 5240 ) + 1 ] , buff[ CalcByte( 5240 ) + 2 ] , buff[ CalcByte( 5240 ) + 3 ] ).ToString( );

                        data2 = PLCValue( data2 , 2 );
                        data3 = PLCValue( data3 , 2 );
                        data4 = PLCValue( data4 , 2 );
                        data5 = PLCValue( data5 , 2 );

                        data2 = DecimalPoint( data2 , 4 );
                        data3 = DecimalPoint( data3 , 4 );
                        data4 = DecimalPoint( data4 , 4 );
                        data5 = DecimalPoint( data5 , 4 );

                        data2 = RoundUp( data2 , 2 ).ToString( );
                        data3 = RoundUp( data3 , 2 ).ToString( );
                        data4 = RoundUp( data4 , 2 ).ToString( );
                        data5 = RoundUp( data5 , 2 ).ToString( );

                        if ( TalkingComm != null ) TalkingComm( "Balance3" , "" , bcr , data1 , data2 , data3 , data4 , data5 , "" , "" , "" , "" , "" );
                    }


                    
                }
                catch ( Exception )
                {

                }

                Thread.Sleep( 1000 );
                stopwatch.Stop(); // 시간 측정 끝

                //Directory.CreateDirectory("D:\\" + Mainpath + "\\Log");
               // Log_K.WriteLog(System.Windows.Forms.ListBox.li, Mainpath, "라벨프린터 자동출력 실패 - ERROR");

                System.Console.WriteLine("실행시간 :" + DateTime.Now.ToLongTimeString() + " / 경과시간 : "+ stopwatch.ElapsedMilliseconds);
                stopwatch.Reset();

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
            //스레드종료
            CommFlag = false;

            //스레드종료
        }

        private void Pause( )
        {
            try
            {
                Connected = false;

                if ( _stream != null )
                {
                    _stream.Close( );
                    _stream = null;
                }

                if ( mClient != null )
                {
                    mClient.Close( );
                    mClient = null;
                }

                CommStop( );

            }
            catch ( Exception )
            {

            }
        }
        public void Dispose( )
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
        public void Disconnection( )
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

    public class TCPClient_PLC2
    {
        string ServerIP = "";
        int ServerPort = 0;
        int ReceiveTimeOut = 0;

        LingerOption lingeroption = new LingerOption( true , 0 );

        string ClientIP = "";
        int ClientPort = 0;

        public delegate void EveHandler( string name , object data , string data2 , string data3 , string data4 , string data5 , string data6 , string data7 , string data8 , string data9 );
        public event EveHandler TalkingComm;

        public bool Connected = false;
        NetworkStream _stream = null;
        private TcpClient mClient;
        Form1 mainform;

        int CalcByte( int Offset )
        {
            int result = Offset - Start;
            return result * 2;
        }
        string ByteToDecision( byte bt )
        {
            if ( bt == 1 )
                return "OK";
            else if ( bt == 2 )
                return "NG";
            else
                return "";
        }

        public TCPClient_PLC2( string ServerIP , int ServerPort , int ReceiveTimeOut , string ClientIP , int ClientPort , Form1 mainform )
        {
            this.ServerIP = ServerIP;
            this.ServerPort = ServerPort;
            this.ReceiveTimeOut = ReceiveTimeOut;
            this.mainform = mainform;

            this.ClientIP = ClientIP;
            this.ClientPort = ClientPort;

            ConnectStart( );
        }

        public TCPClient_PLC2( string ServerIP , int ServerPort , int ReceiveTimeOut , Form1 mainform )
        {
            this.ServerIP = ServerIP;
            this.ServerPort = ServerPort;
            this.ReceiveTimeOut = ReceiveTimeOut;
            this.mainform = mainform;


            ConnectStart( );
        }

        public string DecimalPoint( string str , int point )
        {
            if ( point < 0 )
                return "0";

            int div = 10;

            for ( int i = 0 ; i < point - 1 ; i++ )
            {
                div *= 10;

            }

            string str_ = ( double.Parse( str ) / div ).ToString( "N" + point.ToString( ) );

            return str_;
        }



        public int[ ] MCRead_By_Offsets( int offset , int num )
        {
            lock ( tcplock )
            {
                byte[ ] ReceiveData = new byte[ 5000 ];//데이터받음
                byte[ ] Command_Byte = Ken2.Communication.MCProtocolCmd_K.Read_Dreg( offset , num );
                try
                {
                    _stream.Write( Command_Byte , 0 , Command_Byte.Length );
                }
                catch ( IOException )//데이터를전송할수가없어서 plc와 연결을 끊기. 연결이 끊어지면 계속 연결시도함.
                {
                    Pause( );
                }

                try
                {
                    _stream.Read( ReceiveData , 0 , ReceiveData.Length );//리시브데이터에 집어넣음
                    _stream.Flush( );
                }
                catch ( IOException )
                {

                }

                return Ken2.Communication.MCProtocolCmd_K.View_MCData( ReceiveData );
            }
        }

        object tcplock = new object( );

        public byte[ ] MCRead( int offset , int num )
        {
            lock ( tcplock )
            {
                byte[ ] ReceiveData = new byte[ 5000 ];//데이터받음
                byte[ ] Command_Byte = Ken2.Communication.MCProtocolCmd_K.Read_Dreg( offset , num );
                try
                {
                    _stream.Write( Command_Byte , 0 , Command_Byte.Length );
                }
                catch ( IOException )//데이터를전송할수가없어서 plc와 연결을 끊기. 연결이 끊어지면 계속 연결시도함.
                {
                    Pause( );
                }

                try
                {
                    _stream.Read( ReceiveData , 0 , ReceiveData.Length );//리시브데이터에 집어넣음
                    _stream.Flush( );
                }
                catch ( IOException )
                {

                }

                return Ken2.Communication.MCProtocolCmd_K.View_MCData_Byte( ReceiveData );
            }
        }

        public void MCWrite( int offset , int data )
        {

            lock ( tcplock )
            {
                byte[ ] ReceiveData = new byte[ 2000 ];//데이터받음
                byte[ ] Command_Byte = Ken2.Communication.MCProtocolCmd_K.Write_Dreg( offset , data );


                try
                {
                    _stream.Write( Command_Byte , 0 , Command_Byte.Length );

                }
                catch ( IOException )//데이터를전송할수가없어서 plc와 연결을 끊기. 연결이 끊어지면 계속 연결시도함.
                {
                    Pause( );

                }

                try
                {
                    _stream.Read( ReceiveData , 0 , ReceiveData.Length );//리시브데이터에 집어넣음
                    _stream.Flush( );

                }
                catch ( IOException )
                {

                }



            }
        }

        public void MCWrite2(int offset, uint data)
        {

            lock (tcplock)
            {
                byte[] ReceiveData = new byte[2000];//데이터받음
                //byte[] Command_Byte = Ken2.Communication.MCProtocolCmd_K.Write_Dreg(offset, data);
                byte[] Command_Byte = Ken2.Communication.MCProtocolCmd_K.Write_Dreg_2Word(offset, data);


                try
                {
                    _stream.Write(Command_Byte, 0, Command_Byte.Length);

                }
                catch (IOException)//데이터를전송할수가없어서 plc와 연결을 끊기. 연결이 끊어지면 계속 연결시도함.
                {
                    Pause();

                }

                try
                {
                    _stream.Read(ReceiveData, 0, ReceiveData.Length);//리시브데이터에 집어넣음
                    _stream.Flush();

                }
                catch (IOException)
                {

                }



            }
        }

        public static string PLCValue( string data , int word_num )
        {
            try
            {
                long buff = long.Parse( data );


                if ( word_num == 1 )
                {

                    if ( buff > 32767 )
                        buff = buff - 65536;


                    return buff.ToString( );
                }
                else if ( word_num == 2 )
                {
                    long diff = 4294967296;

                    if ( buff > 2147483647 )
                        buff = buff - diff‬;

                    return buff.ToString( );

                }

            }
            catch ( Exception )
            {
                try
                {
                    if ( data.Equals( "OK" ) || data.Equals( "NG" ) )
                        return data;
                }
                catch ( Exception )
                {

                }
            }
            return "0";
        }

        public void MCWriteString( int offset , string str )
        {
            lock ( tcplock )
            {
                byte[ ] ReceiveData = new byte[ 100 ];//데이터받음

                try
                {
                    _stream.Write( Ken2.Communication.MCProtocolCmd_K.Write_W_reg( offset , str ) , 0 , Ken2.Communication.MCProtocolCmd_K.Write_W_reg( offset , str ).Length );
                }
                catch ( IOException )//데이터를전송할수가없어서 plc와 연결을 끊기. 연결이 끊어지면 계속 연결시도함.
                {
                    Pause( );
                }

                try
                {
                    _stream.Read( ReceiveData , 0 , ReceiveData.Length );//리시브데이터에 집어넣음
                    _stream.Flush( );

                }
                catch ( IOException )
                {

                }

            }
        }

        public void MCWrite_Clear( int offset , int length )
        {

            lock ( tcplock )
            {

                byte[ ] ReceiveData = new byte[ 1000 ];//데이터받음

                try
                {
                    _stream.Write( Ken2.Communication.MCProtocolCmd_K.Write_W_Clear( offset , length ) , 0 , Ken2.Communication.MCProtocolCmd_K.Write_W_Clear( offset , length ).Length );

                }
                catch ( IOException )//데이터를전송할수가없어서 plc와 연결을 끊기. 연결이 끊어지면 계속 연결시도함.
                {
                    Pause( );

                }

                try
                {
                    _stream.Read( ReceiveData , 0 , ReceiveData.Length );//리시브데이터에 집어넣음
                    _stream.Flush( );

                }
                catch ( IOException )
                {

                }



            }
        }

        object ReadLock = new object( );

        double RoundUp( string d_value , int n_point )
        {
            double bf = double.Parse( d_value );
            double res = Math.Round( bf , n_point );

            return res;
        }

        int Start = 7000;

        int CalcOffset( int num )
        {
            int result = num - Start;
            return result * 2;
        }

        #region -----# Connect #-----

        private Thread Connect;
        bool ConnectFlag = false;//Bool Flag
        //스레드함수
        private void ConnectMethod( )
        {
            while ( ConnectFlag )
            {

                try
                {

                    if ( Connected == false )//연결끊어졌을때만 함
                    {
                        if ( ClientIP.Equals( "" ) )
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

                        //TalkingComm( "ServerConnected" , Connected );
                    }



                }
                catch ( Exception )
                {

                }

                Thread.Sleep( 1000 );

            }
        }
        //스레드함수
        public void ConnectStart( )
        {
            //스레드스타트
            ConnectFlag = true;
            Connect = new Thread( ConnectMethod );
            Connect.Start( );
            //스레드스타트
        }
        public void ConnectStop( )
        {
            Connect.Abort( );

            //스레드종료
            ConnectFlag = false;

            //스레드종료
        }
        #endregion

        #region -----# Comm #-----

        private Thread Comm;//스레드
        bool CommFlag = false;//Bool Flag

        //tttttttttttttttttttttttttttttttttttt
        private void CommMethod( )
        {
            PulseDetector Save1 = new PulseDetector( );
            PulseDetector Save2 = new PulseDetector( );
            PulseDetector Save3 = new PulseDetector( );
            PulseDetector Save4 = new PulseDetector( );
            PulseDetector Save5 = new PulseDetector( );
            PulseDetector Save6 = new PulseDetector( );

            PulseDetector LabelPrint = new PulseDetector( );

            PulseDetector BarcodeCheck = new PulseDetector( );
            PulseDetector BarcodeCheck2 = new PulseDetector( );
            PulseDetector BarcodeCheck3 = new PulseDetector( );
            PulseDetector BarcodeCheck4 = new PulseDetector( );

            PulseDetector Balance = new PulseDetector( );
            PulseDetector Balance2 = new PulseDetector( );
            PulseDetector Balance3 = new PulseDetector( );

            CountPlay flip = new CountPlay( );
            CountPlay quantity = new CountPlay( );


            while ( CommFlag )
            {

                try
                {

                    if ( mainform.Viewdatachk.Checked )
                    {
                        int[ ] commdata = MCRead_By_Offsets( 7000 , 500 );//7000번지 500워드

                        if ( TalkingComm != null ) TalkingComm( "CommData" , commdata , "" , "" , "" , "" , "" , "" , "" , "" );
                    }

                    byte[ ] buff = MCRead( 7000 , 500 );//1000개 바이트

                    if ( flip.OnePlay( 1 ) )
                        MCWrite( 8000 , 0 );
                    else
                        MCWrite( 8000 , 1 );
                    //}

                    if ( mainform.CurrentModelNum1 != buff[ CalcByte( 7000 ) ] )
                        if ( TalkingComm != null ) TalkingComm( "ModelChange" , buff[ CalcByte( 7000 ) ] , "" , "" , "" , "" , "" , "" , "" , "" );


                    //#E30 로더 바코드 DATA 요구                  
                    if ( BarcodeCheck.Detect( buff[ CalcOffset( 7010 ) ] , 1 ) )
                    {
                        string bcr = DataChange_K.ByteToString_Clean_0x00( buff , CalcByte( 7013 ) , 18 ).Trim( );

                        if ( TalkingComm != null ) TalkingComm( "BarcodeCheck" , "" , bcr , "" , "" , "" , "" , "" , "" , "" );
                    }

                    if ( quantity.OnePlay( 10 ) )
                    {
                        int offset1 = 7356;
                        string data1 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( offset1 ) ] , buff[ CalcByte( offset1 ) + 1 ] , buff[ CalcByte( offset1 ) + 2 ] , buff[ CalcByte( offset1 ) + 3 ] ).ToString( );
                        data1 = PLCValue( data1 , 2 );
                        mainform.QuantityData[ 4 ] = data1;

                        int offset2 = 7358;
                        string data2 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( offset2 ) ] , buff[ CalcByte( offset2 ) + 1 ] , buff[ CalcByte( offset2 ) + 2 ] , buff[ CalcByte( offset2 ) + 3 ] ).ToString( );
                        data2 = PLCValue( data2 , 2 );
                        mainform.QuantityData[ 5 ] = data2;

                    }


                    //#F21 특성 검사 DATA 읽기 요구
                    if ( Save1.Detect( buff[ CalcOffset( 7040 ) ] , 1 ) )
                    {
                        string bcr = DataChange_K.ByteToString_Clean_0x00( buff , CalcByte( 7043 ) , 18 ).Trim( );

                        //string data1 = ByteToDecision( buff[ CalcByte( 7060 ) ] );
                       // string data2 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 7061 ) ] , buff[ CalcByte( 7061 ) + 1 ] , buff[ CalcByte( 7061 ) + 2 ] , buff[ CalcByte( 7061 ) + 3 ] ).ToString( );
                        string data3 = ByteToDecision( buff[ CalcByte( 7064 ) ] );
                        string data4 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 7065 ) ] , buff[ CalcByte( 7065 ) + 1 ] , buff[ CalcByte( 7065 ) + 2 ] , buff[ CalcByte( 7065 ) + 3 ] ).ToString( );
                        string data5 = ByteToDecision( buff[ CalcByte( 7068 ) ] );
                        string data6 = ( buff[ CalcByte( 7069 ) ] + buff[ CalcByte( 7069 ) + 1 ] * 256 ).ToString( );

                       // data1 = PLCValue( data1 , 1 );
                       // data2 = PLCValue( data2 , 2 );
                        data3 = PLCValue( data3 , 1 );

                        data4 = PLCValue( data4 , 2 );
                        //data4 = PLCValue(data4, 2);

                        data5 = PLCValue( data5 , 1 );
                        data6 = PLCValue( data6 , 1 );

                       // data2 = DecimalPoint( data2 , 2 );
                        data6 = DecimalPoint( data6 , 2 );

                        //data2 = RoundUp( data2 , 2 ).ToString( );
                        data6 = RoundUp( data6 , 2 ).ToString( );
                        

                        if ( TalkingComm != null ) TalkingComm( "Save1" , "" , bcr , "" , "", data3 , data4 , data5 , data6 , "" );
                    }


                    //#F31 특성 검사 DATA 읽기 요구
                    if ( Save2.Detect( buff[ CalcOffset( 7080 ) ] , 1 ) )
                    {
                        string bcr = DataChange_K.ByteToString_Clean_0x00( buff , CalcByte( 7083 ) , 18 ).Trim( );

                        //string data1 = ByteToDecision( buff[ CalcByte( 7100 ) ] );
                       // string data2 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 7101 ) ] , buff[ CalcByte( 7101 ) + 1 ] , buff[ CalcByte( 7101 ) + 2 ] , buff[ CalcByte( 7101 ) + 3 ] ).ToString( );
                        string data3 = ByteToDecision( buff[ CalcByte( 7104 ) ] );
                        string data4 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 7105 ) ] , buff[ CalcByte( 7105 ) + 1 ] , buff[ CalcByte( 7105 ) + 2 ] , buff[ CalcByte( 7105 ) + 3 ] ).ToString( );
                        string data5 = ByteToDecision( buff[ CalcByte( 7108 ) ] );
                        string data6 = ( buff[ CalcByte( 7109 ) ] + buff[ CalcByte( 7109 ) + 1 ] * 256 ).ToString( );

                       // data1 = PLCValue( data1 , 1 );
                        //data2 = PLCValue( data2 , 2 );
                        data3 = PLCValue( data3 , 1 );
                        //data4 = PLCValue( data4 , 5 );
                        data4 = PLCValue(data4, 2);
                        data5 = PLCValue( data5 , 1 );
                        data6 = PLCValue( data6 , 1 );

                       // data2 = DecimalPoint( data2 , 2 );
                        data6 = DecimalPoint( data6 , 2 );

                        //data2 = RoundUp( data2 , 2 ).ToString( );
                        data6 = RoundUp( data6 , 2 ).ToString( );

                        if ( TalkingComm != null ) TalkingComm( "Save2" , "" , bcr , "" , "" , data3 , data4 , data5 , data6 , "" );

                    }


                    //#F22 특성 검사 DATA 읽기 요구
                    if ( Save3.Detect( buff[ CalcOffset( 7120 ) ] , 1 ) )
                    {
                        string bcr = DataChange_K.ByteToString_Clean_0x00( buff , CalcByte( 7123 ) , 18 ).Trim( );

                        //string data1 = ByteToDecision( buff[ CalcByte( 7140 ) ] );
                       // string data2 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 7141 ) ] , buff[ CalcByte( 7141 ) + 1 ] , buff[ CalcByte( 7141 ) + 2 ] , buff[ CalcByte( 7141 ) + 3 ] ).ToString( );
                        string data3 = ByteToDecision( buff[ CalcByte( 7144 ) ] );
                        string data4 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 7145 ) ] , buff[ CalcByte( 7145 ) + 1 ] , buff[ CalcByte( 7145 ) + 2 ] , buff[ CalcByte( 7145 ) + 3 ] ).ToString( );
                        string data5 = ByteToDecision( buff[ CalcByte( 7148 ) ] );
                        string data6 = ( buff[ CalcByte( 7149 ) ] + buff[ CalcByte( 7149 ) + 1 ] * 256 ).ToString( );

                       // data1 = PLCValue( data1 , 1 );
                       // data2 = PLCValue( data2 , 2 );
                        data3 = PLCValue( data3 , 1 );
                        // data4 = PLCValue( data4 , 5 );
                        data4 = PLCValue(data4, 2);
                        data5 = PLCValue( data5 , 1 );
                        data6 = PLCValue( data6 , 1 );

                       // data2 = DecimalPoint( data2 , 2 );
                        data6 = DecimalPoint( data6 , 2 );

                       // data2 = RoundUp( data2 , 2 ).ToString( );
                        data6 = RoundUp( data6 , 2 ).ToString( );

                        if ( TalkingComm != null ) TalkingComm( "Save3" , "" , bcr , "" , "" , data3 , data4 , data5 , data6 , "" );
                    }


                    //#F32 특성 검사 DATA 읽기 요구
                    if ( Save4.Detect( buff[ CalcOffset( 7160 ) ] , 1 ) )
                    {
                        string bcr = DataChange_K.ByteToString_Clean_0x00( buff , CalcByte( 7163 ) , 18 ).Trim( );

                        //string data1 = ByteToDecision( buff[ CalcByte( 7180 ) ] );
                       // string data2 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 7181 ) ] , buff[ CalcByte( 7181 ) + 1 ] , buff[ CalcByte( 7181 ) + 2 ] , buff[ CalcByte( 7181 ) + 3 ] ).ToString( );
                        string data3 = ByteToDecision( buff[ CalcByte( 7184 ) ] );
                        string data4 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 7185 ) ] , buff[ CalcByte( 7185 ) + 1 ] , buff[ CalcByte( 7185 ) + 2 ] , buff[ CalcByte( 7185 ) + 3 ] ).ToString( );
                        string data5 = ByteToDecision( buff[ CalcByte( 7188 ) ] );
                        string data6 = ( buff[ CalcByte( 7189 ) ] + buff[ CalcByte( 7189 ) + 1 ] * 256 ).ToString( );

                       // data1 = PLCValue( data1 , 1 );
                        //data2 = PLCValue( data2 , 2 );
                        data3 = PLCValue( data3 , 1 );
                        //data4 = PLCValue( data4 , 5 );
                        data4 = PLCValue(data4, 2);

                        data5 = PLCValue( data5 , 1 );
                        data6 = PLCValue( data6 , 1 );

                        //data2 = DecimalPoint( data2 , 2 );
                        data6 = DecimalPoint( data6 , 2 );

                        //data2 = RoundUp( data2 , 2 ).ToString( );
                        data6 = RoundUp( data6 , 2 ).ToString( );

                        if ( TalkingComm != null ) TalkingComm( "Save4" , "" , bcr , "" , "" , data3 , data4 , data5 , data6 , "" );
                    }



                    //#G11 컨베이어 바코드 DATA 요구
                    if ( BarcodeCheck2.Detect( buff[ CalcOffset( 7200 ) ] , 1 ) )
                    {
                        string bcr = DataChange_K.ByteToString_Clean_0x00( buff , CalcByte( 7203 ) , 18 ).Trim( );

                        if ( TalkingComm != null ) TalkingComm( "BarcodeCheck2" , "" , bcr , "" , "" , "" , "" , "" , "" , "" );

                    }

                    //#H11 성능 검사 DATA 읽기 요구
                    if ( Save5.Detect( buff[ CalcOffset( 7220 ) ] , 1 ) )
                    {
                        string bcr = DataChange_K.ByteToString_Clean_0x00( buff , CalcByte( 7223 ) , 18 ).Trim( );

                        string data1 = ByteToDecision( buff[ CalcByte( 7240 ) ] );
                        string data2 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 7242 ) ] , buff[ CalcByte( 7242 ) + 1 ] , buff[ CalcByte( 7242 ) + 2 ] , buff[ CalcByte( 7242 ) + 3 ] ).ToString( );
                        string data3 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 7244 ) ] , buff[ CalcByte( 7244 ) + 1 ] , buff[ CalcByte( 7244 ) + 2 ] , buff[ CalcByte( 7244 ) + 3 ] ).ToString( );
                        string data4 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 7246 ) ] , buff[ CalcByte( 7246 ) + 1 ] , buff[ CalcByte( 7246 ) + 2 ] , buff[ CalcByte( 7246 ) + 3 ] ).ToString( );

                        data1 = PLCValue( data1 , 1 );
                        data2 = PLCValue( data2 , 2 );
                        data3 = PLCValue( data3 , 2 );
                        data4 = PLCValue( data4 , 2 );

                        data3 = DecimalPoint( data3 , 1 );
                        data4 = DecimalPoint( data4 , 2 );

                        data3 = RoundUp( data3 , 2 ).ToString( );
                        data4 = RoundUp( data4 , 2 ).ToString( );

                        if (TalkingComm != null)
                        {
                            TalkingComm("Save5", "", bcr, data1, data2, data3, data4, "", "", "");
                            //최종판정
                            TalkingComm("BarcodeCheck4", "", bcr, "", "", "", "", "", "", "");
                        }

                        

                    }


                    //#G12 컨베이어 바코드 DATA 요구
                    if ( BarcodeCheck3.Detect( buff[ CalcOffset( 7300 ) ] , 1 ) )
                    {
                        string bcr = DataChange_K.ByteToString_Clean_0x00( buff , CalcByte( 7303 ) , 18 ).Trim( );

                        if ( TalkingComm != null ) TalkingComm( "BarcodeCheck3" , "" , bcr , "" , "" , "" , "" , "" , "" , "" );

                    }


                    //#H12 성능 검사 DATA 읽기 요구
                    if ( Save6.Detect( buff[ CalcOffset( 7320 ) ] , 1 ) )
                    {
                        string bcr = DataChange_K.ByteToString_Clean_0x00( buff , CalcByte( 7323 ) , 18 ).Trim( );

                        string data1 = ByteToDecision( buff[ CalcByte( 7340 ) ] );
                        string data2 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 7342 ) ] , buff[ CalcByte( 7342 ) + 1 ] , buff[ CalcByte( 7342 ) + 2 ] , buff[ CalcByte( 7342 ) + 3 ] ).ToString( );
                        string data3 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 7344 ) ] , buff[ CalcByte( 7344 ) + 1 ] , buff[ CalcByte( 7344 ) + 2 ] , buff[ CalcByte( 7344 ) + 3 ] ).ToString( );
                        string data4 = Ken2.Communication.MCProtocolCmd_K.View_MCData_2Word( buff[ CalcByte( 7346 ) ] , buff[ CalcByte( 7346 ) + 1 ] , buff[ CalcByte( 7346 ) + 2 ] , buff[ CalcByte( 7346 ) + 3 ] ).ToString( );

                        data1 = PLCValue( data1 , 1 );
                        data2 = PLCValue( data2 , 2 );
                        data3 = PLCValue( data3 , 2 );
                        data4 = PLCValue( data4 , 2 );

                        data3 = DecimalPoint( data3 , 1 );
                        data4 = DecimalPoint( data4 , 2 );

                        data3 = RoundUp( data3 , 2 ).ToString( );
                        data4 = RoundUp( data4 , 2 ).ToString( );

                        if (TalkingComm != null)
                        {
                            TalkingComm("Save6", "", bcr, data1, data2, data3, data4, "", "", "");

                            //최종판정 
                            TalkingComm("BarcodeCheck4", "", bcr, "", "", "", "", "", "", "");
                        }

                        

                    }

                    //최종판정 을 성능검사 를 하고 하는것으로 이동
                    //#I60 완성 로더 바코드 DATA 요구
                    if (BarcodeCheck4.Detect(buff[CalcOffset(7400)], 1))
                    {
                        string bcr = DataChange_K.ByteToString_Clean_0x00(buff, CalcByte(7403), 18).Trim();

                        //완성 로더 바코드만 체크하고 최종판정을 안하는 것으로 해서 4 -> 5로 변경
                        if (TalkingComm != null) TalkingComm("BarcodeCheck5", "", bcr, "", "", "", "", "", "", "");
                    }


                }
                catch ( Exception )
                {

                }


                Thread.Sleep( 200 );


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
            //스레드종료
            CommFlag = false;

            //스레드종료
        }







        private void Pause( )
        {
            try
            {
                Connected = false;

                if ( _stream != null )
                {
                    _stream.Close( );
                    _stream = null;
                }

                if ( mClient != null )
                {
                    mClient.Close( );
                    mClient = null;
                }

                CommStop( );

            }
            catch ( Exception )
            {

            }
        }
        public void Dispose( )
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
        public void Disconnection( )
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
    
}
