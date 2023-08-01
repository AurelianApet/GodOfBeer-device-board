using device.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace device.network
{
    class TokenManager : GenericSingleton<TokenManager>
    {
        public class Token 
        {
            public int flowCnt;
            public Token()
            {
                flowCnt = 0;
            }
        }

        Token token = new Token();
        public int GetFlowCnt()//유량센서 미작동
        {
            lock (token)
            {
                return token.flowCnt;
            }
        }

        public void SetFlowCnt(int fcnt)
        {
            lock (token)
            {
                token.flowCnt = fcnt;
            }
        }
    }
}
