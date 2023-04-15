
using Ken2.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KB_Monitor_V2
{
    public class TCPServer_K
    {
        string IP = "";
        int port = 0;

        public TcpListener mServer;
        public TcpClient mClient;
        public NetworkStream _stream;
        public bool Connected = false;


        private delegate void dele( );//delegate

        //이벤트 발생시키는 클래스에 선언
        public delegate void EveHandler( string name , object data , int length );
        public event EveHandler TalkingComm;


        public TCPServer_K( string ip , int port )
        {
            this.IP = ip;
            this.port = port;

            mServer = new TcpListener( IPAddress.Parse( IP ) , port );
            mServer.Start( );

            ListenThreadStart( 0 );

        }

        public void Pause( )
        {
            ReceiveThreadStop( );

            if ( _stream != null )
            {
                _stream.Close( );
            }

            if ( mClient != null )
            {
                mClient.Close( );
            }

            TalkingComm( "DisConnected" , 0 , 0 );
            Connected = false;
        }

        public void Dispose( )
        {
            ListenThreadStop( );
            ReceiveThreadStop( );
        }


        #region -----# ListenThread #-----
        private Thread ListenThread;//스레드
        bool ListenThreadFlag = false;//Bool Flag
        //스레드함수

        //tttttttttttttttttt
        private void ListenThreadMethod( object param )
        {
            int para = ( int ) param;


            while ( ListenThreadFlag )
            {

                try
                {
                    mServer.BeginAcceptTcpClient( HandleAsyncConnection , mServer );
                    //여기서 무한정기다리지않고 번호표뽑고 연결할라고 대기하는놈들 몇마리인지
                    //확인 후 있으면 HandleAsyncConnection 콜백함수 호출해줌.
                    //없으면 넘어가기(비동기)
                    //AcceptTcpClient = 동기 신호로 무한정기다리면서 연결하는놈

                }
                catch ( Exception )
                {

                }

                Thread.Sleep( 1000 );

            }

            try
            {
                mServer.Stop( );
                mClient.Close( );
                _stream.Close( );
            }
            catch ( Exception )
            {

            }

        }
        //스레드함수
        public void ListenThreadStart( int param )
        {
            //스레드스타트
            ListenThreadFlag = true;
            ListenThread = new Thread( ( new ParameterizedThreadStart( ListenThreadMethod ) ) );
            ListenThread.Start( param );

            //스레드스타트
        }
        public void ListenThreadStop( )
        {
            //스레드종료
            ListenThreadFlag = false;

        }
        #endregion


        private void HandleAsyncConnection( IAsyncResult res )
        {
            try
            {
                mClient = mServer.EndAcceptTcpClient( res );
                _stream = mClient.GetStream( );
                _stream.ReadTimeout = 1000;

                ReceiveThreadStart( 0 );

            }
            catch
            {

            }
        }


        #region -----# ReceiveThread #-----

        private Thread ReceiveThread;//스레드
        bool ReceiveThreadFlag = false;//Bool Flag
        //스레드함수
        //ttttttttttttttttttttttttt
        private void ReceiveThreadMethod( object param )
        {
            int length = 0;

            int para = ( int ) param;
            TalkingComm( "Connected" , 0 , 0 );
            Connected = true;

            while ( ReceiveThreadFlag )
            {
                try
                {
                    byte[ ] buff = new byte[ 6000 ];

                    length = _stream.Read( buff , 0 , buff.Length );


                    if ( length == 0 )
                    {
                        Pause( );
                        break;
                    }

                    if ( length < 5 )
                        return;


                    if ( TalkingComm != null ) TalkingComm( "Data" , buff , length );

                    byte[ ] bt = new byte[ 1 ] { 0x31 };
                    _stream.Write( bt , 0 , 1 );

                }
                catch ( Exception )
                {

                }
            }
        }
        //스레드함수
        public void ReceiveThreadStart( int param )
        {
            //스레드스타트
            ReceiveThreadFlag = true;
            ReceiveThread = new Thread( ( new ParameterizedThreadStart( ReceiveThreadMethod ) ) );
            ReceiveThread.Start( param );

        }
        public void ReceiveThreadStop( )
        {
            //스레드종료
            ReceiveThreadFlag = false;
            //ReceiveThread = null;

        }
        #endregion


        public void SendString( string str )
        {
            byte[ ] buff = DataChange_K.StringToByteArr( str );

            try
            {
                _stream.Write( buff , 0 , buff.Length );
            }
            catch ( Exception )
            {
                Pause( );
            }
        }

        public void Send( string str )
        {

            //try
            //{
            //    string SendData = Parsing.DeleteSpace( str );

            //    //int SendDataLength = textBox2.TextLength;//4

            //    char[ ] CharArray = SendData.ToCharArray( );// 0 0 0 0

            //    string[ ] NewSendData = new string[ CharArray.Length / 2 ];// 2

            //    for ( int i = 0 ; i < NewSendData.Length ; i++ )
            //    {
            //        NewSendData[ i ] = CharArray[ i * 2 ].ToString( ) + CharArray[ i * 2 + 1 ].ToString( );
            //    }

            //    byte[ ] SendBuffer = new byte[ NewSendData.Length ];

            //    for ( int i = 0 ; i < SendBuffer.Length ; i++ )
            //    {
            //        SendBuffer[ i ] = byte.Parse( NewSendData[ i ] , System.Globalization.NumberStyles.HexNumber );
            //    }

            //    _stream.Write( SendBuffer , 0 , SendBuffer.Length );


            //}
            //catch ( Exception eee )
            //{
            //    Pause( );

            //}

        }
    }

}
