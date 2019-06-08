 #line 2
 
 
 
 
    #region PLUGIN_BODY
        const int VERSION = 19;
        const string FILE = "plugin.sys.event.distributebalance.pls";


        #region SETTINGS

        class _SETTINGS : TOOL_SETTINGS
        {

            public static _BUF BUF = null;
            public class _BUF
            {
                public static void LOAD_SETTINGS(_PLUGIN PLUGIN)
                {
                    if (_SETTINGS.BUF != null)
                        return;

                    var x = new _SETTINGS._BUF();

                    var s = new _SETTINGS(PLUGIN);


                    x.MY_DISTRIBUTEBALANCE_USER = s.MY_DISTRIBUTEBALANCE_USER;

                    //
                    x._USER = PLUGIN.GETSYSPRM_USER();
                    x._FIRM = PLUGIN.GETSYSPRM_FIRM();
                    x._FIRMNAME = PLUGIN.GETSYSPRM_FIRMNAME();
                    x._PERIOD = PLUGIN.GETSYSPRM_PERIOD();




                    _SETTINGS.BUF = x;


                }

                public string MY_DISTRIBUTEBALANCE_USER;


                public short _FIRM;
                public string _FIRMNAME;
                public short _PERIOD;
                public short _USER;


            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC)//, "ava.dbbackup.config")
            {

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active On User")]
            public string MY_DISTRIBUTEBALANCE_USER
            {
                get
                {
                    return (_GET("MY_DISTRIBUTEBALANCE_USER", "1,2"));
                }
                set
                {
                    _SET("MY_DISTRIBUTEBALANCE_USER", value);
                }

            }

            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_DISTRIBUTEBALANCE_USER),
                     FORMAT(pPLUGIN.GETSYSPRM_USER())
                     ) >= 0;
            }

        }

        #endregion
        #region TEXT


        const string event_DISTRIBUTEBALANCE_ = "hadlericom_distributebalance_";
        const string event_DISTRIBUTEBALANCE_APPLY = "hadlericom_distributebalance_apply";

        public class TEXT
        {
            public const string text_DESC = "Distribute Contractor Balance";

        }
        #endregion

        #region MAIN






        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {


            _SETTINGS._BUF.LOAD_SETTINGS(this);

            if (!_SETTINGS.ISUSEROK(this))
                return;


            object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
            object arg2 = ARGS.Length > 1 ? ARGS[1] : null;
            object arg3 = ARGS.Length > 2 ? ARGS[2] : null;

            string[] list_ = EXPLODELISTPATH(EVENTCODE);

            switch (list_.Length > 0 ? list_[0] : "")
            {
                case SysEvent.SYS_PLUGINSETTINGS:
                    (arg1 as List<object>).Add(new _SETTINGS(this));
                    break;
                case SysEvent.SYS_NEWFORM:
                    MY_SYS_NEWFORM_INTEGRATE_REF(arg1 as Form);
                    break;
                case SysEvent.SYS_USEREVENT:
                    MY_SYS_USEREVENT_HANDLER(EVENTCODE, ARGS);
                    break;

            }



        }



        void MY_SYS_NEWFORM_INTEGRATE_REF(Form FORM)
        {
            if (FORM == null)
                return;
            try
            {

                var fn = GETFORMNAME(FORM);



                var isTarget = fn.StartsWith("adp.fin.doc.client.5");


                if (isTarget)
                {

                    var cPanelBtnSub = CONTROL_SEARCH(FORM, "cPanelBtnSub");

                    if (cPanelBtnSub == null)
                        return;




                    _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_DISTRIBUTEBALANCE_APPLY, LANG("T_DISTRIBUTE"), "tools_16x16");



                }

            }
            catch (Exception exc)
            {
                MSGUSERERROR("Cant add button: " + exc.Message);
            }

        }





        void _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(Control pCtrl, string pEvent, string pText, string pImg = null)
        {
            if (pCtrl == null)
                return;
            pImg = pImg ?? "info_16x16";
            try
            {


                var args = new Dictionary<string, object>() { 
            
			{ "_cmd" ,""},
            { "_type" ,""},
			{ "CmdText" ,"event name::"+pEvent},
			{ "Text" ,pText},
			{ "ImageName" ,pImg},
			{"AutoSize", true},
			//{ "Width" ,100},
            };

                var b = RUNUIINTEGRATION(pCtrl, args) as Button;
                if (b != null)
                {

                    var w = (Math.Max(100, b.Width + 32) / 20) * 20;
                    b.AutoSize = false;
                    b.Width = w;

                }

            }
            catch (Exception exc)
            {
                MSGUSERERROR("Cant add button: " + exc.Message);
            }

        }

        public void MY_SYS_USEREVENT_HANDLER(string EVENTCODE, object[] ARGS) //adapter start
        {


            //
            try
            {


                object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
                object arg2 = ARGS.Length > 1 ? ARGS[1] : null;
                object arg3 = ARGS.Length > 2 ? ARGS[2] : null;

                string[] list_ = EXPLODELISTPATH(EVENTCODE);
                var cmd = list_.Length > 1 ? list_[1].ToLowerInvariant() : "";

                switch (cmd)
                {

                    case event_DISTRIBUTEBALANCE_APPLY:
                        {

                            var form = arg1 as Form;
                            if (form != null && ISADAPTERFORM(form))
                            {
                                var ds = GETDATASETFROMADPFORM(form);
                                MY_DISTRIBUTEBALANCE_APPLY(ds);

                            }

                        }
                        break;

                }


            }
            catch (Exception exc)
            {
                LOG(exc);
                MSGUSERERROR(exc.Message);
            }
        }

        void MY_DISTRIBUTEBALANCE_APPLY(DataSet pDs)
        {
            var tool = MY_ASK_TOOL(this);

            if (ISEMPTY(tool))
                return;

            switch (tool)
            {
                case tool_balance_distribute:
                    _MY_DISTRIBUTEBALANCE_APPLY(pDs);
                    break;

                case tool_sum_debit:
                    _MY_DISTRIBUTEBALANCE_APPLY_SUM_DEBIT_OR_CREDIT(pDs, true);
                    break;
                case tool_sum_credit:
                    _MY_DISTRIBUTEBALANCE_APPLY_SUM_DEBIT_OR_CREDIT(pDs, false);
                    break;

            }


        }



        void _MY_DISTRIBUTEBALANCE_APPLY_SUM_DEBIT_OR_CREDIT(DataSet pDs, bool pDebit)
        {
            if (pDs == null)
                return;

            var HEADER = pDs.Tables["CLFICHE"];
            var LINES = pDs.Tables["CLFLINE"];

            if (HEADER == null || LINES == null)
                return;


            string colSource = pDebit ? "DEBIT" : "CREDIT";
            string colDest = !pDebit ? "DUMMY_____DEBIT" : "DUMMY_____CREDIT";

            double sum = CASTASDOUBLE(TAB_GETROW(HEADER, colSource));



            if (!ISNUMZERO(sum))
            {
                var rowNew = TAB_ADDROW(LINES);
                TAB_SETROW(rowNew, colDest, sum);

            }
        }


        void _MY_DISTRIBUTEBALANCE_APPLY(DataSet pDs)
        {
            if (pDs == null)
                return;

            var HEADER = pDs.Tables["CLFICHE"];
            var LINES = pDs.Tables["CLFLINE"];

            if (HEADER == null || LINES == null)
                return;

            var date = CASTASDATE(TAB_GETROW(HEADER, "DATE_")).Date;

            date = MY_ASK_DATE(this, "T_DATE", date);

            if (ISEMPTY(date))
                return;


            object clientTopRef = null;
            string clientTopCode = null;
            {
                DataRow[] res_ = REF("ref.fin.rec.client desc::" + _PLUGIN.STRENCODE("Contractor Top Card"));
                if (res_ != null && res_.Length > 0)
                {
                    clientTopRef = TAB_GETROW(res_[0], "LOGICALREF");
                }
                if (ISEMPTYLREF(clientTopRef))
                    return;
                clientTopCode = MY_CLIENT_CODE(this, clientTopRef);

            }

            if (ISEMPTY(clientTopCode))
                return;

            object clientSubRef = null;
            string clientSubCode = null;
            {
                DataRow[] res_ = REF("ref.fin.rec.client desc::" + _PLUGIN.STRENCODE("One of Contractor Sub Card e.g. [00.000.0000]"));
                if (res_ != null && res_.Length > 0)
                {
                    clientSubRef = TAB_GETROW(res_[0], "LOGICALREF");
                }
                if (ISEMPTYLREF(clientSubRef))
                    return;
                clientSubCode = MY_CLIENT_CODE(this, clientSubRef);

            }

            if (ISEMPTY(clientSubCode))
                return;

            {
                var arr = clientSubCode.Split('.');
                if (arr.Length <= 1)
                    throw new Exception("Client code shuld has more than one group");

                arr[arr.Length - 1] = "%";
                clientSubCode = string.Join(".", arr);
            }

            var subClients = (SQL(@"

SELECT 
--$MS$--TOP(10000)
LOGICALREF FROM LG_$FIRM$_CLCARD WHERE CODE LIKE @P1 ORDER BY CODE ASC
--$PG$--LIMIT 10000
--$SL$--LIMIT 10000
", new object[] { clientSubCode }));

            if (subClients.Rows.Count == 0)
                throw new Exception("T_MSG_ERROR_NO_DATA [" + clientSubCode + ".*]");


            var topClBalance = MY_CLIENT_BALANCE(this, clientTopRef, date);


            TAB_ADDCOL(subClients, "BALANCE", typeof(double));
            TAB_ADDCOL(subClients, "RATE", typeof(double));
            double totalSubBalance = 0.0;
            foreach (DataRow row in subClients.Rows)
            {
                var subClLRef = TAB_GETROW(row, "LOGICALREF");
                var subClBalance = MY_CLIENT_BALANCE(this, subClLRef, date);

                if (Math.Sign(subClBalance) == Math.Sign(topClBalance))
                {
                    subClBalance = 0;
                }

                totalSubBalance += subClBalance;

                TAB_SETROW(row, "BALANCE", subClBalance);
            }

            foreach (DataRow row in subClients.Rows)
            {
                var rate = ABS(DIV(TAB_GETROW(row, "BALANCE"), totalSubBalance));
                TAB_SETROW(row, "RATE", rate);
            }

            double totDistribute = MIN(ABS(topClBalance), ABS(totalSubBalance));

            totDistribute = MY_ASKNUMBER(this, "T_VALUE", totDistribute, 2);

            if (totDistribute < 0)
                return;

            var toBalanceIsDebit = topClBalance > 0;
            //debit=>credit

          

            var insertPos = 0;
            {
                var recFrom = LINES.NewRow();
                LINES.Rows.InsertAt(recFrom, insertPos);

                TAB_SETROW(recFrom, "CLIENTREF", clientTopRef);
              
                TAB_SETROW(recFrom, toBalanceIsDebit ? "DUMMY_____CREDIT" : "DUMMY_____DEBIT", totDistribute);
            }

            foreach (DataRow row in subClients.Rows)
            {
                var subClLRef = TAB_GETROW(row, "LOGICALREF");
                var balanceRate = CASTASDOUBLE(TAB_GETROW(row, "RATE"));
                var clBalancePart = balanceRate * totDistribute;

                if (ISNUMZERO(balanceRate))
                    continue;
                
                {
                    ++insertPos;

                    var recFrom = LINES.NewRow();
                    LINES.Rows.InsertAt(recFrom, insertPos);

                    TAB_SETROW(recFrom, "CLIENTREF", subClLRef);

                    //if tot debit sub is credit so set debit
                    //debit=>debit

                    TAB_SETROW(recFrom, toBalanceIsDebit ? "DUMMY_____DEBIT" : "DUMMY_____CREDIT", clBalancePart);
                }
            }



        }



        #endregion

        #region TOOLS


        const string tool_balance_distribute = "tool_balance_distribute";

        const string tool_sum_debit = "tool_sum_debit";
        const string tool_sum_credit = "tool_sum_credit";


        static string MY_ASK_TOOL(_PLUGIN pPLUGIN)
        {
            var list = new List<string>();

            //
            //distribute
            //sum debit
            //sum credit

            list.AddRange(new string[] { tool_sum_debit, pPLUGIN.LANG("T_SUM (T_DEBIT) => T_CREDIT") });
            list.AddRange(new string[] { tool_sum_credit, pPLUGIN.LANG("T_SUM (T_CREDIT) => T_DEBIT") });
            list.AddRange(new string[] { tool_balance_distribute, pPLUGIN.LANG("T_DISTRIBUTE (T_BALANCE)") });



            var res_ = pPLUGIN.REF("ref.gen.definedlist [obj::" + JOINLIST(list.ToArray()) + "] [desc::T_TOOL] type::string");

            string exportCode_ = (res_ != null && res_.Length > 0 ? CASTASSTRING(res_[0]["VALUE"]) : null);

            return exportCode_;

        }

        public static DateTime MY_ASK_DATE(_PLUGIN pPLUGIN, string pMsg, DateTime? pDef = null)
        {

            if (pDef == null)
                pDef = DateTime.Now;

            DataRow[] rows_ = pPLUGIN.REF("ref.gen.date desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
            if (rows_ != null && rows_.Length > 0)
            {
                return _PLUGIN.CASTASDATE(rows_[0]["DATE_"]);
            }
            return new DateTime(1900, 1, 1);
        }

        public double MY_CLIENT_BALANCE(_PLUGIN pPLUGIN, object pClRef)
        {
            return CASTASDOUBLE(SQLSCALAR("SELECT (DEBIT-CREDIT) FROM LG_$FIRM$_$PERIOD$_GNTOTCL WHERE CARDREF = @P1 AND TOTTYP = 1", new object[] { pClRef }));
        }

        public double MY_CLIENT_BALANCE(_PLUGIN pPLUGIN, object pClRef, DateTime pDate)
        {

            var val = CASTASDOUBLE(SQLSCALAR(@"

SELECT
SUM(COALESCE((CASE WHEN SIGN = 0 THEN +AMOUNT ELSE -AMOUNT END),0.0))
FROM 
LG_$FIRM$_$PERIOD$_CLFLINE 
WHERE CLIENTREF= @P1 AND DATE_ <= @P2 AND CANCELLED = 0

", new object[] { pClRef, pDate }));

            return val;

        }
        public string MY_CLIENT_CODE(_PLUGIN pPLUGIN, object pClRef)
        {
            return CASTASSTRING(SQLSCALAR("SELECT CODE FROM LG_$FIRM$_CLCARD WHERE LOGICALREF = @P1", new object[] { pClRef }));
        }
        public double MY_ASKNUMBER(_PLUGIN pPLUGIN, string pMsg, double pDef, int pDecimals)
        {
            //  
            DataRow[] rows_ = pPLUGIN.REF("ref.gen.double decimals::" + FORMAT(pDecimals) + " calc::1 desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
            if (rows_ != null && rows_.Length > 0)
            {
                return _PLUGIN.CASTASDOUBLE(ISNULL(rows_[0]["VALUE"], 0.0));
            }
            return -1;
        }

        #endregion

        #endregion
