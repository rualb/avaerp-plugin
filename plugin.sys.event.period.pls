#line 2




 #region PLUGIN_BODY
        const int VERSION = 6;

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

                    x.ACTIVE = (s.MY_PERIOD_ACTIVE == "1");

                    //

                    _SETTINGS.BUF = x;

                }

                public bool ACTIVE;

            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC)
            {

            }




            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active")]
            [TypeConverter(typeof(EListConverterActiveType))]
            public string MY_PERIOD_ACTIVE
            {
                get { return (_GET("MY_PERIOD_ACTIVE", "1")); }
                set { _SET("MY_PERIOD_ACTIVE", value); }

            }


            class EListConverterActiveType : EListConverter
            {
                public EListConverterActiveType()
                    : base("1,Yes,0,No")
                {

                }

            }

        }

        #endregion
        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "Period";
        }
        #endregion

        #region MAIN
        const double convertRate = 1.0; //1.0/1.80;use if one period is USR next period is EUR 
        //

        const string EVENT_PERIOD_MAT = "com_hadleri_period_mat";
        const string EVENT_PERIOD_CL = "com_hadleri_period_cl";
        const string EVENT_PERIOD_CASH = "com_hadleri_period_cash";
        const string EVENT_PERIOD_PRICE = "com_hadleri_period_price";
        const string EVENT_PERIOD_ACC = "com_hadleri_period_acc"; //gl

        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {

            if (GETSYSPRM_USER() != 1)
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
                    _MY_SYS_NEWFORM_INTEGRATE_MAINFORM(arg1 as Form);
                    break;
                case SysEvent.SYS_USEREVENT:
                    MY_SYS_USEREVENT_HANDLER(EVENTCODE, ARGS);
                    break;

            }



        }



        void _MY_SYS_NEWFORM_INTEGRATE_MAINFORM(Form FORM)
        {
            if (FORM == null)
                return;

            var fn = GETFORMNAME(FORM);
            if (fn != "form.app")
                return;

            _SETTINGS._BUF.LOAD_SETTINGS(this);
            if (!_SETTINGS.BUF.ACTIVE)
                return;


            var tree = CONTROL_SEARCH(FORM, "cTreeTools");
            if (tree == null)
                return;


            
            string nodeCode_ = "comhadleri_period";

            

            var args = new Dictionary<string, object>() {            
			        { "_cmd" ,""},
                    { "_type" ,""},
			        { "CmdText" ,""},
			        { "Text" ,"T_PERIOD"},
			        { "ImageName" ,"calendar_32x32"},
			        { "Name" ,nodeCode_},
                };
 
            RUNUIINTEGRATION(tree, args);
            var arr1 = new string[] { 
                "event name::" + EVENT_PERIOD_MAT, 
                "event name::" + EVENT_PERIOD_CL, 
                "event name::" + EVENT_PERIOD_CASH ,
                "event name::" + EVENT_PERIOD_PRICE ,
                "event name::" + EVENT_PERIOD_ACC 
            
            };
            var arr2 = new string[] { "T_MATERIAL", "T_PERSONAL", "T_CASH", "T_PRICE", "T_GL" };
            var arr3 = new string[] { "mm_32x32", "user_32x32", "cash_register_32x32", "money_32x32", "gl_32x32 " };

            for (int i = 0; i < arr1.Length; ++i)
                {
                    args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
			{ "_root" ,nodeCode_},
			{ "CmdText" ,arr1[i]},
			{ "Text" ,arr2[i]},
			{ "ImageName" ,arr3[i]},
			{ "Name" ,nodeCode_+ "_"+arr1[i]},
            };

                    RUNUIINTEGRATION(tree, args);
                }
        }



        public void MY_SYS_USEREVENT_HANDLER(string EVENTCODE, object[] ARGS) //adapter start
        {
            //"SYS_USEREVENT/exchange_rates_tcmb"


            try
            {

                object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
                object arg2 = ARGS.Length > 1 ? ARGS[1] : null;

                string[] list_ = EXPLODELISTPATH(EVENTCODE);

                switch (list_.Length > 1 ? list_[1].ToLowerInvariant() : "")
                {

      
                    case EVENT_PERIOD_MAT:
                        new PERIOD_MAT().DONE(this);
                        break;
                    case EVENT_PERIOD_CL:
                        new PERIOD_CLIENT().DONE(this);
                        break;
                    case EVENT_PERIOD_CASH:
                        new PERIOD_CASH().DONE(this);
                        break;
                    case EVENT_PERIOD_PRICE:
                        new PERIOD_PRICE().DONE(this);
                        break;
                    case EVENT_PERIOD_ACC:
                        new PERIOD_GL().DONE(this);
                        break;

                }

               
            }
            catch (Exception exc)
            {
                LOG(exc);
                MSGUSERERROR("T_MSG_OPERATION_FAILED");
                MSGUSERERROR(exc.Message);
            }
        }





        public string askString(string pMsg, string pDef)
        {

            DataRow[] rows_ = REF("ref.gen.string desc::" + STRENCODE(pMsg) + " filter::filter_VALUE," + FORMATSERIALIZE(pDef));
            if (rows_ != null && rows_.Length > 0)
            {
                return CASTASSTRING(ISNULL(rows_[0]["VALUE"], ""));
            }
            return null;

        }

        public static double askNumber(_PLUGIN PLUGIN, string pMsg, double pDef, int pDecimals)
        {
            //  
            DataRow[] rows_ = PLUGIN.REF("ref.gen.double decimals::" + FORMAT(pDecimals) + " calc::1 desc::" + STRENCODE(pMsg) + " filter::filter_VALUE," + FORMATSERIALIZE(pDef));
            if (rows_ != null && rows_.Length > 0)
            {
                return CASTASDOUBLE(ISNULL(rows_[0]["VALUE"], 0.0));
            }
            return -1;
        }

        public static string askPeriod(_PLUGIN PLUGIN, bool pAllFirm, bool pCommit)
        {

            var tab_ = PLUGIN.SQL(
            @"SELECT 
				FIRMNR,NR 
				FROM 
				L_CAPIPERIOD 
				WHERE 
				(@P1 = 0 OR @P1 = FIRMNR) AND (NR != $PERIOD$)
				ORDER BY FIRMNR DESC,NR DESC", new object[]{
				pAllFirm ? 0 : PLUGIN.GETSYSPRM_FIRM()
				
				});
            var list = new List<string>();
            foreach (DataRow r in tab_.Rows)
            {
                var f = FORMAT(TAB_GETROW(r, "FIRMNR"), "000");
                var p = FORMAT(TAB_GETROW(r, "NR"), "00");


                list.AddRange(new string[] { f + "_" + p, f + "_" + p });

            }


            var res_ = PLUGIN.REF("ref.gen.definedlist [obj::" + JOINLIST(list.ToArray()) + "] [desc::] type::string");

            string code_ = (res_ != null && res_.Length > 0 ? CASTASSTRING(res_[0]["VALUE"]) : null);


            if (code_ != null && pCommit)
            {
                if (!PLUGIN.MSGUSERASK("Period [" + code_ + "] ?"))
                    return null;

            }

            return code_;

        }
				
				
				


        #endregion


        #region CLASS

        class PERIOD_MAT
        {

            string SPERIOD;
            string SFIRM;

            public void DONE(_PLUGIN PLUGIN)
            {
                if (!PLUGIN.MSGUSERASK("T_MSG_COMMIT_BEGIN" + " T_MATERIAL (T_PERIOD)"))
                    return;



                SPERIOD = askPeriod(PLUGIN, false, true);
                if (SPERIOD == null)
                    return;

                SFIRM = LEFT(SPERIOD, 3);

                this.EXPORT(PLUGIN);
                this.IMPORT(PLUGIN);

            }


            public void EXPORT(_PLUGIN PLUGIN)
            {
                string newLine = "\r\n";



                var dir_ = MY_GETDIR(PLUGIN);
                if (Directory.Exists(dir_))
                    Directory.Delete(dir_, true);
                Directory.CreateDirectory(dir_);
                //

                foreach (object wh_ in MY_GETWH(PLUGIN))
                {

                    DataTable tableData_ = MY_GETDATA(PLUGIN, wh_);

                    if (tableData_.Rows.Count > 0)
                    {

                        string fileName_ = MY_GETFILE(PLUGIN, wh_);
                        File.WriteAllText(fileName_, newLine);
                        File.AppendAllText(fileName_, JOINLISTTAB(new string[] { "$b" }) + newLine);
                        File.AppendAllText(fileName_, JOINLISTTAB(new string[] { "$p", "adp", "adp.mm.doc.slip.14" }) + newLine);
                        File.AppendAllText(fileName_, JOINLISTTAB(new string[] { "$t", "STFICHE", "FICHENO", "SOURCEINDEX" }) + newLine);
                        File.AppendAllText(fileName_, JOINLISTTAB(FORMAT(new object[] { "$", MY_DOCCODE(PLUGIN, wh_), wh_ })) + newLine);
                        File.AppendAllText(fileName_, JOINLISTTAB(new string[] { "$t", "STLINE", "ITEMS_____CODE", "AMOUNT", "PRICE" }) + newLine);
                        //
                        foreach (DataRow rowData_ in tableData_.Rows)
                        {
                            File.AppendAllText(fileName_,
                            JOINLISTTAB(FORMAT(new object[]{
				"$",
				TAB_GETROW(rowData_,"CODE"),
				TAB_GETROW(rowData_,"ONHAND"),
				TAB_GETROW(rowData_,"PRICE") 
				
				})) + newLine);
                        }
                        //
                        File.AppendAllText(fileName_, JOINLISTTAB(new string[] { "$e" }) + newLine);
                    }

                }
                //

                //if(!MY_HASDATA(PLUGIN))
                //	throw new Exception("T_MSG_DATA_NO");
                // else
                PLUGIN.MSGUSERINFO("T_MSG_OPERATION_FINISHED - T_EXPORT");

            }

            public void IMPORT(_PLUGIN PLUGIN)
            {

                PLUGIN.EXECMDTEXT("import [loc::" + MY_GETDIR(PLUGIN) + "\\data.*.exim]");



                PLUGIN.MSGUSERINFO("T_MSG_OPERATION_FINISHED");

            }


            string MY_GETFILE(_PLUGIN PLUGIN, object pWh)
            {
                return MY_GETDIR(PLUGIN) + "\\" + "data." + FORMAT(pWh, "000") + ".exim";

            }

            string MY_GETDIR(_PLUGIN PLUGIN)
            {
                return
                GETDYNDIR() +
                "\\_period\\stock\\" +
                FORMAT(PLUGIN.GETSYSPRM_FIRM(), "000")
                 + "_" +
                FORMAT(PLUGIN.GETSYSPRM_PERIOD(), "00");

            }

            object[] MY_GETWH(_PLUGIN PLUGIN)
            {

                var sql =
                string.Format(
                @"
SELECT DISTINCT INVENNO FROM LG_{0}_GNTOTST WHERE INVENNO >= 0 ORDER BY INVENNO
"
                , SPERIOD);

                List<object> list_ = new List<object>();
                DataTable table_ = PLUGIN.SQL(sql);
                foreach (DataRow row_ in table_.Rows)
                    list_.Add(row_["INVENNO"]);
                return list_.ToArray();

            }


            DataTable MY_GETDATA(_PLUGIN PLUGIN, object pWh)
            {

                // 1 Prch 2, FIFO Cost

                var sql =
                string.Format(
                @"

						 declare @rate float
						 select @rate = @P3
						 
						 
DECLARE @CG SMALLINT 
SELECT @CG = COSTGRP FROM L_CAPIWHOUSE WHERE NR = @P1 AND FIRMNR = {1}


SELECT  
(SELECT I.CODE FROM LG_$FIRM$_ITEMS I WHERE I.LOGICALREF = G.STOCKREF) CODE,
G.ONHAND,  
ISNULL((
	case 
	when @P2 = 1 then 
	(
			SELECT TOP 1 PRICE
			FROM LG_{1}_PRCLIST PRC
			WHERE PRC.CARDREF = G.STOCKREF AND PRC.PTYPE = 1 AND PRC.PAYPLANREF IN (0)                               
			ORDER BY ENDDATE DESC
	)
	when @P2 = 2 then 
	(
	SELECT 0.0
	-- t_COST... may be notexists
	--SELECT (case when OUTREMAMNT> 0 then OUTREMCOST/OUTREMAMNT else 0 end) 
	--FROM t_COST_{0}_VAL WHERE LOGICALREF = G.STOCKREF AND COSTGRP = @CG

	)
	else 
	1
	end
),0) PRICE
into #TMP 
FROM LG_{0}_GNTOTST G WHERE INVENNO = @P1 AND G.ONHAND > 0.01

 


select 
CODE,
ROUND(ONHAND,4) ONHAND,
ROUND(PRICE*(@rate),4) PRICE
from #TMP
"

                , SPERIOD, SFIRM);

                return PLUGIN.SQL(sql, new object[] { pWh, 1, convertRate });
            }

            string MY_DOCCODE(_PLUGIN PLUGIN, object pWh)
            {
                return
                "D" +
                FORMAT(PLUGIN.GETSYSPRM_PERIODEND().Year, "0000") +
                "P" +
                FORMAT(PLUGIN.GETSYSPRM_PERIOD(), "00") +
                "X" +
                FORMAT(pWh, "000");

            }
            bool MY_HASDATA(_PLUGIN PLUGIN)
            {
                if (Directory.Exists(MY_GETDIR(PLUGIN)))
                    return (Directory.GetFiles(MY_GETDIR(PLUGIN), "data.*.exim").Length > 0);
                return false;
            }

        }

        class PERIOD_CLIENT
        {

            string SPERIOD;
            string SFIRM;

            public void DONE(_PLUGIN PLUGIN)
            {
                if (!PLUGIN.MSGUSERASK("T_MSG_COMMIT_BEGIN" + " T_PERSONAL (T_PERIOD)"))
                    return;



                SPERIOD = askPeriod(PLUGIN, false, true);
                if (SPERIOD == null)
                    return;

                SFIRM = LEFT(SPERIOD, 3);

                this.EXPORT(PLUGIN);
                this.IMPORT(PLUGIN);

            }


            public void EXPORT(_PLUGIN PLUGIN)
            {
                string newLine = "\r\n";



                var dir_ = MY_GETDIR(PLUGIN);
                if (Directory.Exists(dir_))
                    Directory.Delete(dir_, true);
                Directory.CreateDirectory(dir_);
                //

                foreach (var type_ in new int[] { 0, 1 }) //0 debit 1 credit
                {

                    DataTable tableData_ = MY_GETDATA(PLUGIN, type_);

                    if (tableData_.Rows.Count > 0)
                    {

                        string fileName_ = MY_GETFILE(PLUGIN, type_);
                        File.WriteAllText(fileName_, newLine);
                        File.AppendAllText(fileName_, JOINLISTTAB(new string[] { "$b" }) + newLine);
                        File.AppendAllText(fileName_, JOINLISTTAB(new string[] { "$p", "adp", "adp.fin.doc.client.14" }) + newLine);
                        File.AppendAllText(fileName_, JOINLISTTAB(new string[] { "$t", "CLFICHE", "FICHENO" }) + newLine);
                        File.AppendAllText(fileName_, JOINLISTTAB(FORMAT(new object[] { "$", MY_DOCCODE(PLUGIN, type_), type_ })) + newLine);
                        File.AppendAllText(fileName_, JOINLISTTAB(new string[]{"$t","CLFLINE",
			  "CLCARD_____CODE",
			  (type_ == 0 ? "DUMMY_____DEBIT":"DUMMY_____CREDIT")
			  }) + newLine);
                        //
                        foreach (DataRow rowData_ in tableData_.Rows)
                        {
                            File.AppendAllText(fileName_,
                            JOINLISTTAB(FORMAT(new object[]{
				"$",
				TAB_GETROW(rowData_,"CODE"),
				TAB_GETROW(rowData_,"AMOUNT") 
				
				})) + newLine);
                        }
                        //
                        File.AppendAllText(fileName_, JOINLISTTAB(new string[] { "$e" }) + newLine);
                    }

                }
                //

                //if(!MY_HASDATA(PLUGIN))
                //	throw new Exception("T_MSG_DATA_NO");
                //else
                PLUGIN.MSGUSERINFO("T_MSG_OPERATION_FINISHED - T_EXPORT");

            }

            public void IMPORT(_PLUGIN PLUGIN)
            {

                PLUGIN.EXECMDTEXT("import [loc::" + MY_GETDIR(PLUGIN) + "\\data.*.exim]");



                PLUGIN.MSGUSERINFO("T_MSG_OPERATION_FINISHED");

            }


            string MY_GETFILE(_PLUGIN PLUGIN, int pType)
            {
                return MY_GETDIR(PLUGIN) + "\\" + "data." + (pType == 0 ? "D" : "C") + ".exim";

            }

            string MY_GETDIR(_PLUGIN PLUGIN)
            {
                return
                GETDYNDIR() +
                "\\_period\\client\\" +
                FORMAT(PLUGIN.GETSYSPRM_FIRM(), "000")
                 + "_" +
                FORMAT(PLUGIN.GETSYSPRM_PERIOD(), "00");

            }



            DataTable MY_GETDATA(_PLUGIN PLUGIN, int pType)
            {

                // 0 debit 1, credit

                var sql =
                string.Format(
                @"
 						 declare @rate float
						 select @rate = @P2

SELECT  
(SELECT C.CODE FROM LG_$FIRM$_CLCARD C WHERE C.LOGICALREF = G.CARDREF) CODE,
(DEBIT-CREDIT) AMOUNT
 
into #TMP 
FROM LG_{0}_GNTOTCL G WHERE TOTTYP=1 AND ABS(DEBIT-CREDIT) >=0.001

 
if @P1=0 

select 
CODE,
ABS(ROUND(AMOUNT*@rate,3)) AMOUNT
 
from #TMP where AMOUNT > 0

else

select 
CODE,
ABS(ROUND(AMOUNT*@rate,3)) AMOUNT
 
from #TMP where AMOUNT < 0

"

                , SPERIOD, SFIRM);

                return PLUGIN.SQL(sql, new object[] { pType, convertRate });
            }

            string MY_DOCCODE(_PLUGIN PLUGIN, int pType)
            {
                return
                "D" +
                FORMAT(PLUGIN.GETSYSPRM_PERIODEND().Year, "0000") +
                "P" +
                FORMAT(PLUGIN.GETSYSPRM_PERIOD(), "00") +
                "X" +
                FORMAT(pType, "000");
            }

            bool MY_HASDATA(_PLUGIN PLUGIN)
            {
                if (Directory.Exists(MY_GETDIR(PLUGIN)))
                    return (Directory.GetFiles(MY_GETDIR(PLUGIN), "data.*.exim").Length > 0);
                return false;
            }

        }

        class PERIOD_GL
        {

            string SPERIOD;
            string SFIRM;

            public void DONE(_PLUGIN PLUGIN)
            {
                if (!PLUGIN.MSGUSERASK("T_MSG_COMMIT_BEGIN" + " T_GL (T_PERIOD)"))
                    return;



                SPERIOD = askPeriod(PLUGIN, false, true);
                if (SPERIOD == null)
                    return;

                SFIRM = LEFT(SPERIOD, 3);

                this.EXPORT(PLUGIN);
                this.IMPORT(PLUGIN);

            }


            public void EXPORT(_PLUGIN PLUGIN)
            {
                string newLine = "\r\n";



                var dir_ = MY_GETDIR(PLUGIN);
                if (Directory.Exists(dir_))
                    Directory.Delete(dir_, true);
                Directory.CreateDirectory(dir_);
                //

                foreach (var type_ in new int[] { 0, 1 }) //0 debit 1 credit
                {

                    DataTable tableData_ = MY_GETDATA(PLUGIN, type_);

                    if (tableData_.Rows.Count > 0)
                    {

                        string fileName_ = MY_GETFILE(PLUGIN, type_);
                        File.WriteAllText(fileName_, newLine);
                        File.AppendAllText(fileName_, JOINLISTTAB(new string[] { "$b" }) + newLine);
                        File.AppendAllText(fileName_, JOINLISTTAB(new string[] { "$p", "adp", "adp.gl.doc.slip.1" }) + newLine);
                        File.AppendAllText(fileName_, JOINLISTTAB(new string[] { "$t", "EMFICHE", "FICHENO" }) + newLine);
                        File.AppendAllText(fileName_, JOINLISTTAB(FORMAT(new object[] { "$", MY_DOCCODE(PLUGIN, type_), type_ })) + newLine);
                        File.AppendAllText(fileName_, JOINLISTTAB(new string[]{"$t","EMFLINE",
			  "EMUHACC_____CODE",
			  (type_ == 0 ? "DEBIT":"CREDIT")
			  }) + newLine);
                        //
                        foreach (DataRow rowData_ in tableData_.Rows)
                        {
                            File.AppendAllText(fileName_,
                            JOINLISTTAB(FORMAT(new object[]{
				"$",
				TAB_GETROW(rowData_,"CODE"),
				TAB_GETROW(rowData_,"AMOUNT") 
				
				})) + newLine);
                        }
                        //
                        File.AppendAllText(fileName_, JOINLISTTAB(new string[] { "$e" }) + newLine);
                    }

                }
                //

                // if(!MY_HASDATA(PLUGIN))
                //	throw new Exception("T_MSG_DATA_NO");
                //else
                PLUGIN.MSGUSERINFO("T_MSG_OPERATION_FINISHED - T_EXPORT");

            }

            public void IMPORT(_PLUGIN PLUGIN)
            {

                PLUGIN.EXECMDTEXT("import [loc::" + MY_GETDIR(PLUGIN) + "\\data.*.exim]");



                PLUGIN.MSGUSERINFO("T_MSG_OPERATION_FINISHED");

            }


            string MY_GETFILE(_PLUGIN PLUGIN, int pType)
            {
                return MY_GETDIR(PLUGIN) + "\\" + "data." + (pType == 0 ? "D" : "C") + ".exim";

            }

            string MY_GETDIR(_PLUGIN PLUGIN)
            {
                return
                GETDYNDIR() +
                "\\_period\\gl\\" +
                FORMAT(PLUGIN.GETSYSPRM_FIRM(), "000")
                 + "_" +
                FORMAT(PLUGIN.GETSYSPRM_PERIOD(), "00");

            }



            DataTable MY_GETDATA(_PLUGIN PLUGIN, int pType)
            {

                // 0 debit 1, credit

                var sql =
                string.Format(
                @"
 						 declare @rate float
						 select @rate = @P2
 
SELECT  
(SELECT C.CODE FROM LG_$FIRM$_EMUHACC C WHERE C.LOGICALREF = G.ACCOUNTREF) CODE,
SUM(DEBIT-CREDIT) AMOUNT
 
into #TMP 
FROM LG_{0}_EMUHTOT G WHERE 
   (TOTTYPE = 1) AND (BRANCH = -1) AND (DEPARTMENT = -1)
GROUP BY ACCOUNTREF
 
if @P1=0 

select 
CODE,
ABS(ROUND(AMOUNT*@rate,3)) AMOUNT
 
from #TMP where AMOUNT > 0 AND ABS(AMOUNT) > 0.001

else

select 
CODE,
ABS(ROUND(AMOUNT*@rate,3)) AMOUNT
 
from #TMP where AMOUNT < 0 AND ABS(AMOUNT) > 0.001

"

                , SPERIOD, SFIRM);

                return PLUGIN.SQL(sql, new object[] { pType, convertRate });
            }

            string MY_DOCCODE(_PLUGIN PLUGIN, int pType)
            {
                return
                "D" +
                FORMAT(PLUGIN.GETSYSPRM_PERIODEND().Year, "0000") +
                "P" +
                FORMAT(PLUGIN.GETSYSPRM_PERIOD(), "00") +
                "X" +
                FORMAT(pType, "000");

            }
            bool MY_HASDATA(_PLUGIN PLUGIN)
            {
                if (System.IO.Directory.Exists(MY_GETDIR(PLUGIN)))
                    return (System.IO.Directory.GetFiles(MY_GETDIR(PLUGIN), "data.*.exim").Length > 0);
                return false;
            }

        }

        class PERIOD_CASH
        {

            string SPERIOD;
            string SFIRM;

            public void DONE(_PLUGIN PLUGIN)
            {
                if (!PLUGIN.MSGUSERASK("T_MSG_COMMIT_BEGIN" + " T_CASH (T_PERIOD)"))
                    return;



                SPERIOD = askPeriod(PLUGIN, false, true);
                if (SPERIOD == null)
                    return;

                SFIRM = LEFT(SPERIOD, 3);

                this.EXPORT(PLUGIN);
                this.IMPORT(PLUGIN);

            }


            public void EXPORT(_PLUGIN PLUGIN)
            {
                string newLine = "\r\n";



                var dir_ = MY_GETDIR(PLUGIN);
                if (System.IO.Directory.Exists(dir_))
                    System.IO.Directory.Delete(dir_, true);
                System.IO.Directory.CreateDirectory(dir_);
                //

                foreach (var type_ in new int[] { 0, 1 }) //0 debit 1 credit
                {

                    DataTable tableData_ = MY_GETDATA(PLUGIN, type_);

                    if (tableData_.Rows.Count > 0)
                    {

                        string fileName_ = MY_GETFILE(PLUGIN, type_);
                        System.IO.File.WriteAllText(fileName_, newLine);
                        //doc by line
                        foreach (DataRow rowData_ in tableData_.Rows)
                        {

                            File.AppendAllText(fileName_, JOINLISTTAB(new string[] { "$b" }) + newLine);
                            File.AppendAllText(fileName_, JOINLISTTAB(new string[] { "$p", "adp", (type_ == 0 ? "adp.fin.doc.cash.71" : "adp.fin.doc.cash.72") }) + newLine);
                            File.AppendAllText(fileName_, JOINLISTTAB(new string[]{"$t","KSLINES",
			  "FICHENO",
			  "KSCARD_____CODE",
			  "AMOUNT"
			  }) + newLine);
                            //

                            System.IO.File.AppendAllText(fileName_,
                            JOINLISTTAB(FORMAT(new object[]{
				"$",
				MY_DOCCODE(PLUGIN,type_),
				TAB_GETROW(rowData_,"CODE"),
				TAB_GETROW(rowData_,"AMOUNT") 
				
				})) + newLine);

                            //
                            System.IO.File.AppendAllText(fileName_, JOINLISTTAB(new string[] { "$e" }) + newLine);

                        }
                    }

                }
                //

                // if(!MY_HASDATA(PLUGIN))
                //	throw new Exception("T_MSG_DATA_NO");
                // else
                PLUGIN.MSGUSERINFO("T_MSG_OPERATION_FINISHED - T_EXPORT");

            }

            public void IMPORT(_PLUGIN PLUGIN)
            {

                PLUGIN.EXECMDTEXT("import [loc::" + MY_GETDIR(PLUGIN) + "\\data.*.exim]");



                PLUGIN.MSGUSERINFO("T_MSG_OPERATION_FINISHED");

            }


            string MY_GETFILE(_PLUGIN PLUGIN, int pType)
            {
                return MY_GETDIR(PLUGIN) + "\\" + "data." + (pType == 0 ? "D" : "C") + ".exim";

            }

            string MY_GETDIR(_PLUGIN PLUGIN)
            {
                return
                GETDYNDIR() +
                "\\_period\\cash\\" +
                FORMAT(PLUGIN.GETSYSPRM_FIRM(), "000")
                 + "_" +
                FORMAT(PLUGIN.GETSYSPRM_PERIOD(), "00");

            }



            DataTable MY_GETDATA(_PLUGIN PLUGIN, int pType)
            {

                // 0 debit 1, credit

                var sql =
                string.Format(
                @"
 
 						 declare @rate float
						 select @rate = @P2
 
SELECT  
(SELECT C.CODE FROM LG_$FIRM$_KSCARD C WHERE C.LOGICALREF = G.CARDREF) CODE,
SUM(DEBIT-CREDIT) AMOUNT
 
into #TMP 
FROM LG_{0}_CSHTOTS G WHERE 
   TOTTYPE = 1 AND DAY_ > -1
 GROUP BY CARDREF
 
if @P1=0 

select 
CODE,
ABS(ROUND(AMOUNT*@rate,3)) AMOUNT
 
from #TMP where AMOUNT > 0 AND ABS(AMOUNT) > 0.001

else

select 
CODE,
ABS(ROUND(AMOUNT*@rate,3)) AMOUNT
 
from #TMP where AMOUNT < 0 AND ABS(AMOUNT) > 0.001

"

                , SPERIOD, SFIRM);

                return PLUGIN.SQL(sql, new object[] { pType, convertRate });
            }

            string MY_DOCCODE(_PLUGIN PLUGIN, int pType)
            {
                return
                "D" +
                FORMAT(PLUGIN.GETSYSPRM_PERIODEND().Year, "0000") +
                "P" +
                FORMAT(PLUGIN.GETSYSPRM_PERIOD(), "00") +
                "X" +
                FORMAT(pType, "000");

            }
            bool MY_HASDATA(_PLUGIN PLUGIN)
            {
                if (System.IO.Directory.Exists(MY_GETDIR(PLUGIN)))
                    return (System.IO.Directory.GetFiles(MY_GETDIR(PLUGIN), "data.*.exim").Length > 0);
                return false;
            }

        }



        class PERIOD_PRICE
        {

           
            public void DONE(_PLUGIN PLUGIN)
            {

                if (!PLUGIN.MSGUSERASK("T_MSG_COMMIT_BEGIN" + " T_PRICE (T_PERIOD)"))
                    return;

                var sql = @"
						 
						 declare @rate float
						 select @rate = @P1
						 
 

declare @periodNr smallint,@firmNr smallint,@periodBeg datetime,@periodCurr smallint

select 
@periodNr =$PERIOD$,@firmNr=$FIRM$
select  
@periodBeg = (select BEGDATE from L_CAPIPERIOD where FIRMNR = @firmNr and NR = @periodNr),
@periodCurr = (select PERLOCALCTYPE from L_CAPIPERIOD where FIRMNR = @firmNr and NR = @periodNr)

 
select * into #PRC from LG_$FIRM$_PRCLIST where @periodBeg between BEGDATE and ENDDATE --and CARDREF = 19549

update LG_$FIRM$_PRCLIST set ENDDATE = @periodBeg -1 where @periodBeg between BEGDATE and ENDDATE --and CARDREF = 19549

update #PRC set BEGDATE = @periodBeg  ,CURRENCY = @periodCurr,PRICE = round( PRICE*@rate,2)

DECLARE @INDX INT,@LREF_OLD INT,@LREF_NEW INT
DECLARE @LREFS TABLE(INDX INT IDENTITY(1,1),LOGICALREF INT)
INSERT INTO @LREFS(LOGICALREF) SELECT LOGICALREF from #PRC
SELECT @INDX  = ISNULL((SELECT MAX(INDX) from @LREFS),0)

WHILE EXISTS(SELECT 1 FROM @LREFS WHERE INDX = @INDX)
BEGIN

SELECT @LREF_OLD = LOGICALREF FROM @LREFS WHERE INDX=@INDX

EXEC LREF_LG_$FIRM$_PRCLIST @LREF_NEW OUTPUT

UPDATE #PRC SET LOGICALREF = @LREF_NEW WHERE LOGICALREF = @LREF_OLD

SELECT @INDX = @INDX-1
END
update #PRC set CODE = 'P'+(RIGHT('00'+cast(@periodNr as nvarchar),2))+(RIGHT('00000000'+cast(LOGICALREF as nvarchar),8))
 insert into LG_$FIRM$_PRCLIST select * from #PRC

drop table #PRC
 
 

						 
						 ";
                PLUGIN.INVOKEINBATCH(new DoWorkEventHandler((s, a) =>
                {
                    PLUGIN.SQL(sql, new object[] { convertRate });
                }), null);


                

                PLUGIN.MSGUSERINFO("T_MSG_OPERATION_FINISHED");
            }


            

        }


        #endregion


        #endregion
