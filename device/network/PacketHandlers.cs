using device.util;
using device.restful;
using System;
using System.Net.Sockets;

namespace device.network
{
    #region CommonHandler
    public class CommonHandler
    {
        TokenManager tm = TokenManager.Instance;
        public void REQ_PING(NetworkStream stream)
        {
            try
            {
                //// uint64 TIMESTAMP 본 패킷의 전송 시간
                //int date = NetUtils.ToInt32(requestInfo.Body, 0);
                //int time = NetUtils.ToInt32(requestInfo.Body, 4);
                DateTime now = DateTime.Now;
                Int32 length = PacketInfo.HeaderSize + 8;
                Int32 opcode = (int)Opcode.RES_PING;
                byte[] packet = new byte[length];
                Array.Copy(NetUtils.GetBytes(length), 0, packet, 0, 4);
                Array.Copy(NetUtils.GetBytes(opcode), 0, packet, 4, 4);
                Array.Copy(NetUtils.GetBytes((long)0), 0, packet, 8, 8);
                Array.Copy(NetUtils.GetBytes((long)0), 0, packet, 16, 8);
                int pos = 0;
                Array.Copy(NetUtils.GetBytes(NetUtils.ConvertDateTimeToNetDate(now)), 0, packet, PacketInfo.HeaderSize + pos, 4); pos += 4;
                Array.Copy(NetUtils.GetBytes(NetUtils.ConvertDateTimeToNetTime(now)), 0, packet, PacketInfo.HeaderSize + pos, 4); pos += 4;
                stream.Write(packet, 0, packet.Length);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception : " + ex.ToString());
            }
        }

        public void RES_PING(PacketInfo requestInfo)
        {
            try
            {
                // uint64 TIMESTAMP REQ_PING 패킷의 TIMESTAMP 값
                int date = NetUtils.ToInt32(requestInfo.Body, 0);
                int time = NetUtils.ToInt32(requestInfo.Body, 4);
                Console.WriteLine("[RES_PING] date : " + date + ", time : " + time);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception : " + ex.ToString());
            }
        }

        public void REQ_SET_VALVE_CTRL(NetworkStream stream, int ch_value, int valve, int status)
        {
            try
            {
                Int32 length = PacketInfo.HeaderSize + 12;
                Int32 opcode = (int)Opcode.REQ_SET_VALVE_CTRL;
                byte[] packet = new byte[length];
                Array.Copy(NetUtils.GetBytes(length), 0, packet, 0, 4);
                Array.Copy(NetUtils.GetBytes(opcode), 0, packet, 4, 4);
                Array.Copy(NetUtils.GetBytes((long)0), 0, packet, 8, 8);
                Array.Copy(NetUtils.GetBytes((long)0), 0, packet, 16, 8);
                Array.Copy(NetUtils.GetBytes(ch_value), 0, packet, 24, 4);
                Array.Copy(NetUtils.GetBytes(valve), 0, packet, 28, 4);
                Array.Copy(NetUtils.GetBytes(status), 0, packet, 32, 4);
                stream.Write(packet, 0, packet.Length);
                Console.WriteLine("[REQ_SET_VALVE_CTRL] : ch_value : " + ch_value + ", Valve : " + valve + ", Status : " + status);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception : " + ex.ToString());
            }
        }

        public void RES_SET_VALVE_CTRL(NetworkStream stream, PacketInfo requestInfo)
        {
            try
            {
                int ch_value = NetUtils.ToInt32(requestInfo.Body, 0);
                int valve = NetUtils.ToInt32(requestInfo.Body, 4);
                int status = NetUtils.ToInt32(requestInfo.Body, 8);
                Console.WriteLine("[RES_SET_VALVE_CTRL] : ch_value : " + ch_value + ", valve : " + valve + ", status : " + status);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception : " + ex.ToString());
            }
        }
        
        public void REQ_GET_DEVICE_STATUS(NetworkStream stream, int ch_value)
        {
            try
            {
                Int32 length = PacketInfo.HeaderSize + 4;
                Int32 opcode = (int)Opcode.REQ_GET_DEVICE_STATUS;
                byte[] packet = new byte[length];
                Array.Copy(NetUtils.GetBytes(length), 0, packet, 0, 4);
                Array.Copy(NetUtils.GetBytes(opcode), 0, packet, 4, 4);
                Array.Copy(NetUtils.GetBytes((long)0), 0, packet, 8, 8);
                Array.Copy(NetUtils.GetBytes((long)0), 0, packet, 16, 8);
                Array.Copy(NetUtils.GetBytes(ch_value), 0, packet, 24, 4);
                stream.Write(packet, 0, packet.Length);
                Console.WriteLine("[REQ_GET_DEVICE_STATUS] : ch_value : " + ch_value);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception : " + ex.ToString());
            }
        }

        public void RES_GET_DEVICE_STATUS(NetworkStream stream, PacketInfo requestInfo, int board_no)
        {
            try
            {
                int ch_value = NetUtils.ToInt32(requestInfo.Body, 0);
                byte flowmeter_state = requestInfo.Body[4];
                byte soldout_sensor_state = requestInfo.Body[5];
                byte valve1_state = requestInfo.Body[6];
                byte valve2_state = requestInfo.Body[7];
                Console.WriteLine("[RES_GET_DEVICE_STATUS] : ch_value : " + ch_value + ", flowmeter_state : " + flowmeter_state + ", soldout_sensor_state : " + soldout_sensor_state + ", Valve1_state : " + valve1_state + ", Valve2_state : " + valve2_state);
                ApiClient.Instance.GetDeviceStatusResponseFunc(board_no, ch_value, valve1_state, valve2_state);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception : " + ex.ToString());
            }
        }

        public void REQ_SET_DEVICE_STATUS(NetworkStream stream, PacketInfo requestInfo, int board_no)
        {
            try
            {
                int ch_value = NetUtils.ToInt32(requestInfo.Body, 0);
                byte is_valve_error = requestInfo.Body[4];
                byte is_soldout = requestInfo.Body[5];
                ApiClient.Instance.SetDeviceStatusFunc(board_no, ch_value, (int)is_valve_error, (int)is_soldout);
                Console.WriteLine("[REQ_SET_DEVICE_STATUS] : ch_value : " + ch_value + ", is_valve_error : " + is_valve_error + ", is_soldout : " + is_soldout);
                Int32 length = PacketInfo.HeaderSize;
                Int32 opcode = (int)Opcode.RES_SET_DEVICE_STATUS;
                byte[] packet = new byte[length];
                Array.Copy(NetUtils.GetBytes(length), 0, packet, 0, 4);
                Array.Copy(NetUtils.GetBytes(opcode), 0, packet, 4, 4);
                Array.Copy(NetUtils.GetBytes((long)0), 0, packet, 8, 8);
                Array.Copy(NetUtils.GetBytes((long)0), 0, packet, 16, 8);
                stream.Write(packet, 0, packet.Length);
                Console.WriteLine("[RES_SET_DEVICE_STATUS] sended");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception : " + ex.ToString());
            }
        }

        public void REQ_SET_FLOWMETER_START(NetworkStream stream, int ch_value, string data, long max_limit, long empty_level, long soldout_level, long standby_time, int valve_open)
        {
            try
            {
                Int32 length = PacketInfo.HeaderSize + 296;
                Int32 opcode = (int)Opcode.REQ_SET_FLOWMETER_START;
                byte[] packet = new byte[length];
                Array.Copy(NetUtils.GetBytes(length), 0, packet, 0, 4);
                Array.Copy(NetUtils.GetBytes(opcode), 0, packet, 4, 4);
                Array.Copy(NetUtils.GetBytes((long)0), 0, packet, 8, 8);
                Array.Copy(NetUtils.GetBytes((long)0), 0, packet, 16, 8);
                byte[] tag_data = new byte[256];
                Console.WriteLine(tag_data.Length);
                tag_data = NetUtils.ConvertStringToByteArrayASCII(data);
                Console.WriteLine(tag_data.Length);
                Array.Copy(NetUtils.GetBytes(ch_value), 0, packet, 24, 4);
                Array.Copy(tag_data, 0, packet, 28, tag_data.Length);
                Array.Copy(NetUtils.GetBytes(max_limit), 0, packet, 284, 8);
                Array.Copy(NetUtils.GetBytes(empty_level), 0, packet, 292, 8);
                Array.Copy(NetUtils.GetBytes(soldout_level), 0, packet, 300, 8);
                Array.Copy(NetUtils.GetBytes(standby_time), 0, packet, 308, 8);
                Array.Copy(NetUtils.GetBytes(valve_open), 0, packet, 316, 4);
                stream.Write(packet, 0, packet.Length);
                Console.WriteLine("[REQ_SET_FLOWMETER_START] sended!");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception : " + ex.ToString());
            }
        }

        public void RES_SET_FLOWMETER_START(NetworkStream stream, PacketInfo requestInfo)
        {

        }

        public void REQ_SET_FLOWMETER_STOP(NetworkStream stream, int ch_value)
        {
            try
            {
                Int32 length = PacketInfo.HeaderSize + 4;
                Int32 opcode = (int)Opcode.REQ_SET_FLOWMETER_STOP;
                byte[] packet = new byte[length];
                Array.Copy(NetUtils.GetBytes(length), 0, packet, 0, 4);
                Array.Copy(NetUtils.GetBytes(opcode), 0, packet, 4, 4);
                Array.Copy(NetUtils.GetBytes((long)0), 0, packet, 8, 8);
                Array.Copy(NetUtils.GetBytes((long)0), 0, packet, 16, 8);
                Array.Copy(NetUtils.GetBytes(ch_value), 0, packet, 24, 4);
                stream.Write(packet, 0, packet.Length);
                Console.WriteLine("[REQ_SET_FLOWMETER_STOP] sended!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception : " + ex.ToString());
            }
        }

        public void RES_SET_FLOWMETER_STOP(NetworkStream stream, PacketInfo requestInfo)
        {

        }

        public void REQ_SET_FLOWMETER_VALUE(NetworkStream stream, PacketInfo requestInfo, int board_no)
        {
            try
            {
                Console.WriteLine("0001======================");
                int ch_value = NetUtils.ToInt32(requestInfo.Body, 0);
                byte[] tag_value = new byte[256];
                Array.Copy(requestInfo.Body, 4, tag_value, 0, 256);
                long flowmeter_value = NetUtils.ToInt64(requestInfo.Body, 260);
                string str_tag_value = NetUtils.ConvertByteArrayToStringASCII(tag_value).ToUpper();
                if (flowmeter_value == 0)
                {
                    Console.WriteLine("0002======================");
                    int flowCnt = tm.GetFlowCnt();
                    flowCnt++;
                    if (flowCnt == 3)
                    {
                        Console.WriteLine("0003======================");
                        Console.WriteLine("유량센서 미작동!!!");
                        ApiClient.Instance.SensorNotworkFunc(board_no, ch_value, str_tag_value);
                        tm.SetFlowCnt(0);
                    }
                    else
                    {
                        Console.WriteLine("0004======================");
                        ApiClient.Instance.FlowmeterValueFunc(board_no, ch_value, flowmeter_value, str_tag_value);
                    }
                }
                else
                {
                    Console.WriteLine("0005======================");
                    ApiClient.Instance.FlowmeterValueFunc(board_no, ch_value, flowmeter_value, str_tag_value);
                    tm.SetFlowCnt(0);
                }
                Console.WriteLine("0006======================");
                Console.WriteLine("[REQ_SET_FLOWMETER_VALUE] ch_value : " + ch_value + ", tag_value : " + str_tag_value + ", flowmeter_value : " + flowmeter_value);
                Int32 length = PacketInfo.HeaderSize;
                Int32 opcode = (int)Opcode.RES_SET_FLOWMETER_VALUE;
                byte[] packet = new byte[length];
                Array.Copy(NetUtils.GetBytes(length), 0, packet, 0, 4);
                Array.Copy(NetUtils.GetBytes(opcode), 0, packet, 4, 4);
                Array.Copy(NetUtils.GetBytes((long)0), 0, packet, 8, 8);
                Array.Copy(NetUtils.GetBytes((long)0), 0, packet, 16, 8);
                stream.Write(packet, 0, packet.Length);
                Console.WriteLine("[RES_SET_FLOWMETER_VALUE] sended");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception : " + ex.ToString());
            }
        }

        public void REQ_SET_FLOWMETER_FINISH(NetworkStream stream, PacketInfo requestInfo, int board_no)
        {
            try
            {
                int ch_value = NetUtils.ToInt32(requestInfo.Body, 0);
                byte[] tag_value = new byte[256];
                Array.Copy(requestInfo.Body, 4, tag_value, 0, 256);
                long flowmeter_value = NetUtils.ToInt64(requestInfo.Body, 260);
                byte status = requestInfo.Body[268];
                string str_tag_value = NetUtils.ConvertByteArrayToStringASCII(tag_value).ToUpper();
                ApiClient.Instance.FlowmeterFinishFunc(board_no, ch_value, flowmeter_value, (int)status, str_tag_value);
                Console.WriteLine("[REQ_SET_FLOWMETER_FINISH] ch_value : " + ch_value + ", tag_value : " + str_tag_value + ", flowmeter_value : " + flowmeter_value + ", Status :" + status);
                Int32 length = PacketInfo.HeaderSize;
                Int32 opcode = (int)Opcode.RES_SET_FLOWMETER_FINISH;
                byte[] packet = new byte[length];
                Array.Copy(NetUtils.GetBytes(length), 0, packet, 0, 4);
                Array.Copy(NetUtils.GetBytes(opcode), 0, packet, 4, 4);
                Array.Copy(NetUtils.GetBytes((long)0), 0, packet, 8, 8);
                Array.Copy(NetUtils.GetBytes((long)0), 0, packet, 16, 8);
                stream.Write(packet, 0, packet.Length);
                Console.WriteLine("[RES_SET_FLOWMETER_FINISH] sended");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception : " + ex.ToString());
            }
        }

        public void REQ_SET_FLOWMETER_CHANGE(NetworkStream stream, PacketInfo requestInfo, int board_no)
        {
            try
            {
                int ch_value = NetUtils.ToInt32(requestInfo.Body, 0);
                //byte[] tag_value = new byte[256];
                //Array.Copy(requestInfo.Body, 4, tag_value, 0, 256);
                long flowmeter_value = NetUtils.ToInt64(requestInfo.Body, 4);
                //string str_tag_value = NetUtils.ConvertByteArrayToStringASCII(tag_value).ToUpper();
                ApiClient.Instance.FlowmeterChangeFunc(board_no, ch_value, flowmeter_value);
                Console.WriteLine("[REQ_SET_FLOWMETER_CHANGE] ch_value : " + ch_value + ", flowmeter_value : " + flowmeter_value);
                Int32 length = PacketInfo.HeaderSize;
                Int32 opcode = (int)Opcode.RES_SET_FLOWMETER_CHANGE;
                byte[] packet = new byte[length];
                Array.Copy(NetUtils.GetBytes(length), 0, packet, 0, 4);
                Array.Copy(NetUtils.GetBytes(opcode), 0, packet, 4, 4);
                Array.Copy(NetUtils.GetBytes((long)0), 0, packet, 8, 8);
                Array.Copy(NetUtils.GetBytes((long)0), 0, packet, 16, 8);
                stream.Write(packet, 0, packet.Length);
                Console.WriteLine("[RES_SET_FLOWMETER_CHANGE] sended");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception : " + ex.ToString());
            }
        }

        public void REQ_CONNECT_FAIL(int board_no, int ch_value, string tag_data)
        {
            Console.WriteLine("Failed to receive response from start request.");
            ApiClient.Instance.ReceiveFailFunc(board_no, ch_value, tag_data);
        }

        public void FINISH_FAIL(int board_no, int ch_value, string tag_data, long flowmeter_value)
        {
            Console.WriteLine("Failed to finish response.");
            ApiClient.Instance.FinishFailFunc(board_no, ch_value, tag_data, flowmeter_value);
        }

        public void ERROR_MESSAGE(PacketInfo requestInfo)
        {
            // uint32 OPCODE 에러가 발생한 요청의 OPCODE
            // uint32 ERROR_CODE 에러 코드
            // char[256] ERROR_MESSAGE 에러 메시지
            UInt32 opcode = NetUtils.ToUInt32(requestInfo.Body, 0);
            UInt32 error_code = NetUtils.ToUInt32(requestInfo.Body, 4);
            byte[] error_message = new byte[256];
            Array.Copy(requestInfo.Body, 8, error_message, 0, 256);
            string str_error_message = NetUtils.ConvertByteArrayToStringASCII(error_message);
            Console.WriteLine("[ERROR_MESSAGE] " + opcode.ToString() + " : " + error_code.ToString());
            //session.Close();//disconnect
        }
    }
    #endregion
}
