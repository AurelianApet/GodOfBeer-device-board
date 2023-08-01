using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase.Config;
using System.Net;
using device.util;

namespace device.network
{
    public class MainServer
    {
        TcpListener tcpServer = null;
        int tcp_port = -1;
        Thread waitClient;
        CommonHandler CommonHan = new CommonHandler();
        List<TClient> tclients = new List<TClient>();

        public MainServer()
        {
        }

        public void CreateServer(int port)
        {
            try
            {
                tcp_port = port;
                tcpServer = new TcpListener(IPAddress.Any, port);
                tcpServer.Start();
                waitClient = new Thread(new ThreadStart(ReceiveWait));
                waitClient.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("board 장치 연결 실패!") + ex.ToString());
                Thread.Sleep(5000);
                CreateServer(port);
            }
            //finally
            //{
            //    tcpServer.Stop();
            //}
        }

        private void ReceiveWait()
        {
            while (true)
            {
                Console.WriteLine("Waiting for a connection... ");
                TClient client = new TClient(tcpServer.AcceptTcpClient());
                tclients.Add(client);
                Console.WriteLine("New Client Connected! ip:" + client._clientIP + ", no:" + client.board_no);
                //ThreadPool.QueueUserWorkItem(ThreadProc, client);
                //Thread checkStatus = new Thread(new ThreadStart(checkClientStatus));
                //checkStatus.Start();
            }
        }

        private void checkClientStatus()
        {
            TClient tc = null;
            try
            {
                tc = tclients[tclients.Count - 1];
            }
            catch(Exception ex)
            {
                return;
            }
            while (tc != null)
            {
                try
                {
                    if (!tc.status)
                    {
                        tclients.Remove(tc);
                        break;
                    }
                    Thread.Sleep(500);
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }

        public void Send_REQ_PING(int board_no)
        {
            for(int i = 0; i < tclients.Count; i++)
            {
                if(board_no == ConfigSetting.getBoradNo(tclients[i]._clientIP))
                {
                    CommonHan.REQ_PING(tclients[i].networkStream);
                    tclients[i].networkStream.Flush();
                }
            }
        }

        public void Send_REQ_SET_VALVE_CTRL(int board_no, int ch_value, int valve, int status)
        {
            Console.WriteLine("client count : " + tclients.Count);
            for (int i = 0; i < tclients.Count; i++)
            {
                Console.WriteLine("client Ip : " + tclients[i]._clientIP);
                int bNo = ConfigSetting.getBoradNo(tclients[i]._clientIP);
                Console.WriteLine("client board No : " + bNo);
                if (board_no == bNo)
                {
                    CommonHan.REQ_SET_VALVE_CTRL(tclients[i].networkStream, ch_value, valve, status);
                    tclients[i].networkStream.Flush();
                }
            }
        }

        public void Send_REQ_SET_FLOWMETER_START(int board_no, int ch_value, string data, long max_limit, long empty_level, long soldout_level, long standby_time, int valve_open)
        {
            for (int i = 0; i < tclients.Count; i++)
            {
                if (board_no == ConfigSetting.getBoradNo(tclients[i]._clientIP))
                {
                    CommonHan.REQ_SET_FLOWMETER_START(tclients[i].networkStream, ch_value, data, max_limit, empty_level, soldout_level, standby_time, valve_open);
                    tclients[i].networkStream.Flush();
                    //Send_REQ_PING(board_no);
                }
            }
        }

        public void Send_REQ_SET_FLOWMETER_STOP(int board_no, int ch_value)
        {
            for (int i = 0; i < tclients.Count; i++)
            {
                if (board_no == ConfigSetting.getBoradNo(tclients[i]._clientIP))
                {
                    CommonHan.REQ_SET_FLOWMETER_STOP(tclients[i].networkStream, ch_value);
                    tclients[i].networkStream.Flush();
                }
            }
        }
    }
}
