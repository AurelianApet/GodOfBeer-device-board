using device.util;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;

namespace device.restful
{
    public class ApiClient : GenericSingleton<ApiClient>
    {
        public class ApiInfo
        {
            public string api { get; set; }
            public object resObject { get; set; }
        }

        public class ApiResponse
        {
            public int? suc { get; set; }
            public string msg { get; set; }
            public Dictionary<string, object> dataMap { get; set; }
        }

        Dictionary<Type, ApiInfo> matchDic = null;

        JsonSerializer json = new JsonSerializer();

        public class SetDeviceStatusApi
        {
            public int? board_no { get; set; }
            public int? ch_value { get; set; }
            public int? is_valve_error { get; set; }
            public int? is_soldout { get; set; }
        }

        public class FlowmeterValueApi
        {
            public int? board_no { get; set; }
            public int? ch_value { get; set; }
            public long? flowmeter_value { get; set; }
            public string tag_data { get; set; }
        }

        public class FlowmeterChangeApi
        {
            public int? board_no
            {
                get; set;
            }
            public int? ch_value
            {
                get; set;
            }
            public long? flowmeter_value
            {
                get; set;
            }
        }

        public class FlowmeterFinishApi
        {
            public int? board_no { get; set; }
            public int? ch_value { get; set; }
            public long? flowmeter_value { get; set; }
            public int? status { get; set; }
            public string tag_data { get; set; }
        }

        public class SensorNotworkApi
        {
            public int? board_no { get; set; }
            public int? board_channel { get; set; }
            public string tag_data { get; set; }
        }

        public class GetDeviceStatusResponseApi
        {
            public int? board_no { get; set; }
            public int? ch_value { get; set; }
            public int? valve1_state { get; set; }
            public int? valve2_state { get; set; }
        }

        public class ReceiveFailResponseApi
        {
            public int? board_no { get; set; }
            public int? board_channel { get; set; }
            public string tag_data { get; set; }
        }

        public class FinishFailResponseApi
        {
            public int? board_no { get; set; }
            public int? board_channel { get; set; }
            public string tag_data { get; set; }
            public long? flowmeter_value { get; set; }
        }

        public ApiClient()
        {
            matchDic = new Dictionary<Type, ApiInfo>();
            matchDic.Add(typeof(SetDeviceStatusApi), new ApiInfo() { api = "set-device-status", resObject = new ApiResponse() });
            matchDic.Add(typeof(FlowmeterValueApi), new ApiInfo() { api = "flowmeter-value", resObject = new ApiResponse() });
            matchDic.Add(typeof(FlowmeterChangeApi), new ApiInfo() { api = "flowmeter-change", resObject = new ApiResponse() });
            matchDic.Add(typeof(FlowmeterFinishApi), new ApiInfo() { api = "flowmeter-finish", resObject = new ApiResponse() });
            matchDic.Add(typeof(SensorNotworkApi), new ApiInfo() { api = "sensor-notwork", resObject = new ApiResponse() });
            matchDic.Add(typeof(GetDeviceStatusResponseApi), new ApiInfo() { api = "get-device-response", resObject = new ApiResponse() });
            matchDic.Add(typeof(ReceiveFailResponseApi), new ApiInfo() { api = "receive-fail", resObject = new ApiResponse() });
            matchDic.Add(typeof(FinishFailResponseApi), new ApiInfo() { api = "finish-fail", resObject = new ApiResponse() });
        }

        public ApiResponse PostQuery(object postData)
        {
            ApiResponse result = null;
            try
            {
                Console.WriteLine("0008======================");
                var client = new RestClient(ConfigSetting.api_server_domain);
                var request = new RestRequest(ConfigSetting.api_prefix + matchDic[postData.GetType()].api, Method.POST);
                request.AddHeader("Content-Type", "application/json; charset=utf-8");
                request.AddJsonBody(postData);
                var response = client.Execute(request);
                result = json.Deserialize<ApiResponse>(response);
            }
            catch (Exception ex)
            {
                result = new ApiResponse();
                result.suc = 0;
                result.msg = ex.Message;
                result.dataMap = null;
            }
            return result;
        }

        public ApiResponse SetDeviceStatusFunc(int board_no, int ch_value, int is_valve_error, int is_soldout)
        {
            Console.WriteLine("[SetDeviceStatusFunc] ch_value : " + ch_value + ", board_no : " + board_no + ", is_valve_error : " + is_valve_error + ", is_soldout : " + is_soldout);
            SetDeviceStatusApi info = new SetDeviceStatusApi();
            info.board_no = board_no;
            info.ch_value = ch_value;
            info.is_valve_error = is_valve_error;
            info.is_soldout = is_soldout;
            return PostQuery(info);
        }

        public ApiResponse FlowmeterValueFunc(int board_no, int ch_value, long flowmeter_value, string tag_data)
        {
            Console.WriteLine("0007======================");
            Console.WriteLine("[FlowmeterValueFunc] ch_value : " + ch_value + ", board_no : " + board_no + ", tag_data : " + tag_data + ", flowmeter_value : " + flowmeter_value);
            FlowmeterValueApi info = new FlowmeterValueApi();
            info.board_no = board_no;
            info.ch_value = ch_value;
            info.flowmeter_value = flowmeter_value;
            info.tag_data = tag_data;
            return PostQuery(info);
        }

        public ApiResponse FlowmeterFinishFunc(int board_no, int ch_value, long flowmeter_value, int status, string tag_data)
        {
            Console.WriteLine("[FlowmeterFinishFunc] ch_value : " + ch_value + ", board_no : " + board_no + ", tag_data : " + tag_data + ", flowmeter_value : " + flowmeter_value);
            FlowmeterFinishApi info = new FlowmeterFinishApi();
            info.board_no = board_no;
            info.ch_value = ch_value;
            info.flowmeter_value = flowmeter_value;
            info.status = status;
            info.tag_data = tag_data;
            return PostQuery(info);
        }

        public ApiResponse SensorNotworkFunc(int board_no, int board_channel, string tag_data)
        {
            Console.WriteLine("[SensorNotworkFunc] ch_value : " + board_channel + ", board_no : " + board_no + ", tag_data : " + tag_data);
            SensorNotworkApi info = new SensorNotworkApi();
            info.board_no = board_no;
            info.board_channel = board_channel;
            info.tag_data = tag_data;
            return PostQuery(info);
        }

        public ApiResponse FlowmeterChangeFunc(int board_no, int ch_value, long flowmeter_value)
        {
            Console.WriteLine("[FlowmeterChangeFunc] ch_value : " + ch_value + ", board_no : " + board_no + ", flowmeter_value : " + flowmeter_value);
            FlowmeterChangeApi info = new FlowmeterChangeApi();
            info.board_no = board_no;
            info.ch_value = ch_value;
            info.flowmeter_value = flowmeter_value;
            return PostQuery(info);
        }

        public ApiResponse GetDeviceStatusResponseFunc(int board_no, int ch_value, int valve1, int valve2)
        {
            Console.WriteLine("[GetDeviceStatusResponseFunc] ch_value : " + ch_value + ", board_no : " + board_no + ", valve1 : " + valve1 + ", valve2 : " + valve2);
            GetDeviceStatusResponseApi info = new GetDeviceStatusResponseApi();
            info.board_no = board_no;
            info.ch_value = ch_value;
            info.valve1_state = valve1;
            info.valve2_state = valve2;
            return PostQuery(info);
        }

        public ApiResponse ReceiveFailFunc(int board_no, int ch_value, string tag_data)
        {
            Console.WriteLine("[ReceiveFailFunc] ch_value : " + ch_value + ", board_no : " + board_no + ", tag_data : " + tag_data);
            ReceiveFailResponseApi info = new ReceiveFailResponseApi();
            info.board_no = board_no;
            info.board_channel = ch_value;
            info.tag_data = tag_data;
            return PostQuery(info);
        }

        public ApiResponse FinishFailFunc(int board_no, int ch_value, string tag_data, long flowmeter_value)
        {
            Console.WriteLine("[FinishFailFunc] ch_value : " + ch_value + ", board_no : " + board_no + ", tag_data " + tag_data);
            FinishFailResponseApi info = new FinishFailResponseApi();
            info.board_no = board_no;
            info.board_channel = ch_value;
            info.tag_data = tag_data;
            info.flowmeter_value = flowmeter_value;
            return PostQuery(info);
        }
    }
}
