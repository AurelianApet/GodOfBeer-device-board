using System;
using device.network;
using Quobject.SocketIoClientDotNet.Client;
using device.util;
using Newtonsoft.Json.Linq;
using SimpleJSON;
using device.restful;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;

namespace device
{
    class Program
    {
        public const int tcpPort = 22000;

        public static MainServer mainServer = ServerManager.Instance.mainServer;

        public static bool is_socket_open = false;

        private static JSONNode reqJson;

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static void Main(string[] args)
        {
            ConfigSetting.api_prefix = @"/m-api/device/";
            try
            {
                Console.WriteLine("Board Exe");
                string title = "Board Exe";
                Console.Title = title;
                IntPtr hWnd = FindWindow(null, title);
                if (hWnd != IntPtr.Zero)
                {
                    ShowWindow(hWnd, 2); // minimize the winodw  
                }
                ConfigSetting.server_address = args[0];
                Console.WriteLine("server_address :" + ConfigSetting.server_address);
                ConfigSetting.api_server_domain = @"http://" + ConfigSetting.server_address + ":3006";
                ConfigSetting.socketServerUrl = @"http://" + ConfigSetting.server_address + ":3006";
                ConfigSetting.devices = new DeivceInfo[(args.Length - 1) / 2];
                for(int i = 1; i < args.Length; i++)
                {
                    Console.WriteLine("ip : " + args[i]);
                    ConfigSetting.devices[(i - 1) / 2].ip = args[i];
                    Console.WriteLine("no : " + args[i + 1]);
                    ConfigSetting.devices[(i - 1) / 2].serial_number = int.Parse(args[i + 1]);
                    i++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception : " + ex.Message);
            }

            try
            {
                Socket socket = IO.Socket(ConfigSetting.socketServerUrl);

                socket.On(Socket.EVENT_CONNECT, () =>
                {
                    try
                    {
                        if (is_socket_open)
                        {
                            return;
                        }
                        Console.WriteLine("Socket Connected!");
                        var UserInfo = new JObject();
                        socket.Emit("boardSetInfo", UserInfo);
                        //mainServer.Send_REQ_PING();
                        is_socket_open = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception : " + ex);
                    }
                });

                socket.On(Socket.EVENT_CONNECT_ERROR, (data) =>
                {
                    try
                    {
                        Console.WriteLine("Socket Connect failed : " + data.ToString());
                        is_socket_open = false;
                        //socket.Close();
                        //socket = IO.Socket(ConfigSetting.socketServerUrl);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception : " + ex);
                    }
                });

                socket.On(Socket.EVENT_DISCONNECT, (data) =>
                {
                    try
                    {
                        Console.WriteLine("Socket Disconnect : " + data.ToString());
                        is_socket_open = false;
                        //socket.Close();
                        //socket = IO.Socket(ConfigSetting.socketServerUrl);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception : " + ex);
                    }
                });

                mainServer.CreateServer(tcpPort);

                socket.On("boardValveCtrl", (data) =>
                {
                    try
                    {
                        Console.WriteLine("boardValveCtrl : " + data.ToString());
                        JSONNode jsonNode = SimpleJSON.JSON.Parse(data.ToString());
                        int board_no = jsonNode["board_no"].AsInt;
                        int ch_value = jsonNode["ch_value"].AsInt;
                        int valve = jsonNode["valve"].AsInt;
                        int status = jsonNode["status"].AsInt;
                        mainServer.Send_REQ_SET_VALVE_CTRL(board_no, ch_value, valve, status);
                        //mainServer.Send_REQ_PING(board_no);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception : " + ex);
                    }
                });

                socket.On("boardFlowStart", (data) =>
                {
                    Console.WriteLine("boardFlowStart : " + data.ToString());
                    reqJson = SimpleJSON.JSON.Parse(data.ToString());
                    try
                    {
                        int board_no = reqJson["board_no"].AsInt;
                        int ch_value = reqJson["ch_value"].AsInt;
                        string tag_data = reqJson["tag_data"];
                        long max_limit = long.Parse(reqJson["max_limit"]);
                        long empty_level = long.Parse(reqJson["empty_level"]);
                        long soldout_level = long.Parse(reqJson["soldout_level"]);
                        long standby_time = long.Parse(reqJson["standby_time"]);
                        int valve_open = reqJson["valve_open"].AsInt;
                        mainServer.Send_REQ_SET_FLOWMETER_START(board_no, ch_value, tag_data, max_limit, empty_level, soldout_level, standby_time, valve_open);
                        mainServer.Send_REQ_PING(board_no);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("FLOWMETER_START Exception : " + ex);
                    }
                });

                socket.On("boardFlowStop", (data) =>
                {
                    try
                    {
                        Console.WriteLine("boardFlowStop : " + data.ToString());
                        JSONNode jsonNode = SimpleJSON.JSON.Parse(data.ToString());
                        int board_no = jsonNode["board_no"].AsInt;
                        int ch_value = jsonNode["ch_value"].AsInt;
                        mainServer.Send_REQ_SET_FLOWMETER_STOP(board_no, ch_value);
                        //mainServer.Send_REQ_PING(board_no);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("FLOWMETER_STOP Exception : " + ex);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception : " + ex.Message);
            }

            //manualResetEvent.WaitOne();
            Console.ReadLine();
        }
    }
}
