#line 2



 
 
     #region PLUGIN_BODY
        const int VERSION = 11;

        const string FILE = "plugin.sys.event.carparts.crm.pls";



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

                    x.MY_WEBLIGHTCRM_USER = s.MY_WEBLIGHTCRM_USER;
                    x.MY_WEBLIGHTCRM_AUTH_STRING = s.MY_WEBLIGHTCRM_AUTH_STRING;
                    x.MY_WEBLIGHTCRM_SERVER_URL = s.MY_WEBLIGHTCRM_SERVER_URL;
       
                    //

                    _SETTINGS.BUF = x;

                }

                public string MY_WEBLIGHTCRM_USER;
                public string MY_WEBLIGHTCRM_AUTH_STRING;
                public string MY_WEBLIGHTCRM_SERVER_URL;
                 


            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC) //, "ava.production.config")
            {

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active On User")]
            public string MY_WEBLIGHTCRM_USER
            {
                get
                {
                    return (_GET("MY_WEBLIGHTCRM_USER", "1"));
                }
                set
                {
                    _SET("MY_WEBLIGHTCRM_USER", value);
                }

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Auth String (User:Password)")]
            public string MY_WEBLIGHTCRM_AUTH_STRING
            {
                get
                {
                    return (_GET("MY_WEBLIGHTCRM_AUTH_STRING", ""));
                }
                set
                {
                    _SET("MY_WEBLIGHTCRM_AUTH_STRING", value);
                }

            }
            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Server URL Base (http://localhost:8000)")]
            public string MY_WEBLIGHTCRM_SERVER_URL
            {
                get
                {
                    return (_GET("MY_WEBLIGHTCRM_SERVER_URL", "http://localhost:8000"));
                }
                set
                {
                    _SET("MY_WEBLIGHTCRM_SERVER_URL", value);
                }

            }

 

            //

            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_WEBLIGHTCRM_USER),
                     FORMAT(pPLUGIN.GETSYSPRM_USER())
                     ) >= 0;
            }

        }

        #endregion



        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "Web Light CRM";


            public class L
            {

            }
        }


        const string event_WEBLIGHTCRM_ = "_weblightcrm_";
        const string event_WEBLIGHTCRM_EXPORTMATS = "_weblightcrm_export_mat";
        const string event_WEBLIGHTCRM_EXPORTMATS_ALL = "_weblightcrm_export_mat_all";
        const string event_WEBLIGHTCRM_EXPORTTRANS = "_weblightcrm_export_trans";
        const string event_WEBLIGHTCRM_GEN_TASK = "_weblightcrm_gen_task";
        const string event_WEBLIGHTCRM_EXPORTUSERS = "_weblightcrm_export_users";
        const string event_WEBLIGHTCRM_IMPORT_ORDERS = "_weblightcrm_import_orders";
        #endregion

        #region MAIN



        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {

            if (ISWEB())
                return;


            if (GETSYSPRM_FIRM() == 0)
                return;

            if (GETSYSPRM_USER() != 1)
                return;


            _SETTINGS._BUF.LOAD_SETTINGS(this);


            if (!_SETTINGS.ISUSEROK(this))
                return;



            object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
            object arg2 = ARGS.Length > 1 ? ARGS[1] : null;
            object arg3 = ARGS.Length > 2 ? ARGS[2] : null;

            string[] list_ = EXPLODELISTPATH(EVENTCODE);

            var cmdType = list_.Length > 0 ? list_[0] : "";
            var cmdExt = list_.Length > 1 ? list_[1] : "";



            switch (cmdType)
            {
                case SysEvent.SYS_PLUGINSETTINGS:
                    (arg1 as List<object>).Add(new _SETTINGS(this));
                    break;
                case SysEvent.SYS_LOGIN:
                    {

                    }
                    break;
                case SysEvent.SYS_USEREVENT:
                    MY_SYS_USEREVENT_HANDLER(EVENTCODE, ARGS);
                    break;
                case SysEvent.SYS_NEWFORM:
                    MY_SYS_NEWFORM_INTEGRATE(arg1 as Form);
                    break;


            }



        }



        void MY_SYS_NEWFORM_INTEGRATE(Form FORM)
        {


            var fn = GETFORMNAME(FORM);



            if (fn == "form.app")
            {
                {
                    var tree = CONTROL_SEARCH(FORM, "cTreeTools");
                    if (tree != null)
                    {

                        {
                            var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
			{ "Text" ,"Web CRM"},
			{ "ImageName" ,"earth_32x32"},
			{ "Name" ,event_WEBLIGHTCRM_},
            };

                            RUNUIINTEGRATION(tree, args);

                        }


                        {
                            var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_WEBLIGHTCRM_},
			{ "CmdText" ,"event name::"+event_WEBLIGHTCRM_GEN_TASK},
			{ "Text" ,"T_EXPORT - T_GENERAL"},
			{ "ImageName" ,"calendar_32x32"},
			//{ "Name" ,event_WEBLIGHTCRM_LOADPARTCODESTECDOC},
            };

                            RUNUIINTEGRATION(tree, args);
                        }

                        {
                            var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_WEBLIGHTCRM_},
			{ "CmdText" ,"event name::"+event_WEBLIGHTCRM_EXPORTMATS},
			{ "Text" ,"T_EXPORT - T_MATERIAL"},
			{ "ImageName" ,"mm_32x32"},
			//{ "Name" ,event_WEBLIGHTCRM_LOADPARTCODESTECDOC},
            };

                            RUNUIINTEGRATION(tree, args);

                        }

                        {
                            var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_WEBLIGHTCRM_},
			{ "CmdText" ,"event name::"+event_WEBLIGHTCRM_EXPORTMATS_ALL},
			{ "Text" ,"T_EXPORT - T_MATERIAL - T_ALL"},
			{ "ImageName" ,"mm_32x32"},
			//{ "Name" ,event_WEBLIGHTCRM_LOADPARTCODESTECDOC},
            };

                            RUNUIINTEGRATION(tree, args);

                        }




                        {
                            var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_WEBLIGHTCRM_},
			{ "CmdText" ,"event name::"+event_WEBLIGHTCRM_EXPORTTRANS},
			{ "Text" ,"T_EXPORT - T_DOC"},
			{ "ImageName" ,"run_32x32"},
			//{ "Name" ,event_WEBLIGHTCRM_LOADPARTCODESTECDOC},
            };

                            RUNUIINTEGRATION(tree, args);

                        }

                        {
                            var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_WEBLIGHTCRM_},
			{ "CmdText" ,"event name::"+event_WEBLIGHTCRM_EXPORTUSERS},
			{ "Text" ,"T_EXPORT - T_USER"},
			{ "ImageName" ,"user_32x32"},
			//{ "Name" ,event_WEBLIGHTCRM_LOADPARTCODESTECDOC},
            };

                            RUNUIINTEGRATION(tree, args);

                        }

                        {
                            var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_WEBLIGHTCRM_},
			{ "CmdText" ,"event name::"+event_WEBLIGHTCRM_IMPORT_ORDERS},
			{ "Text" ,"T_IMPORT - T_ORDER"},
			{ "ImageName" ,"import_32x32"},
			//{ "Name" ,event_WEBLIGHTCRM_LOADPARTCODESTECDOC},
            };

                            RUNUIINTEGRATION(tree, args);

                        }






                    }
                }
                {

                    var tree = CONTROL_SEARCH(FORM, "cTree");
                    if (tree != null)
                    {

                        if (ACCESSALLOWED("_sys"))
                        {
                            var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
            { "_root" ,"cTree_006010000"},//general records
			{ "CmdText" ,"ref.gen.rec.userext"},
			{ "Text" ,"T_USER (T_EXT)"},
			{ "ImageName" ,"user_32x32"},
            { "AccessCode" ,"_admin"},
			{ "Name" ,"ref.gen.rec.userext"},
            };

                            RUNUIINTEGRATION(tree, args);

                        }

                    }

                }
                return;

            }






        }

        public void MY_SYS_USEREVENT_HANDLER(string EVENTCODE, object[] ARGS) //adapter start
        {


            //
            try
            {


                object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
                object arg2 = ARGS.Length > 1 ? ARGS[1] : null;

                string[] list_ = EXPLODELISTPATH(EVENTCODE);

                switch (list_.Length > 1 ? list_[1].ToLowerInvariant() : "")
                {
                    case event_WEBLIGHTCRM_EXPORTMATS_ALL:

                        MY_SYS_EXPORT_MATS(true);

                        break;

                    case event_WEBLIGHTCRM_EXPORTMATS:

                        MY_SYS_EXPORT_MATS(false);

                        break;
                    case event_WEBLIGHTCRM_EXPORTTRANS:

                        MY_SYS_EXPORT_TRANS();

                        break;
                    case event_WEBLIGHTCRM_GEN_TASK:

                        MY_SYS_GEN_TASK();

                        break;

                    case event_WEBLIGHTCRM_EXPORTUSERS:

                        MY_SYS_EXPORTUSERS();

                        break;
                    case event_WEBLIGHTCRM_IMPORT_ORDERS:

                        MY_SYS_IMPORT_ORDER();

                        break;


                }


            }
            catch (Exception exc)
            {
                LOG(exc);
                MSGUSERERROR(exc.Message);
            }
        }





        #region HANDLERS


        void MY_SYS_EXPORT_MATS(bool pAll)
        {


            if (!MSGUSERASK("T_MSG_COMMIT_BEGIN - T_EXPORT - T_MATERIAL" + (pAll ? " - T_ALL" : "")))
                return;

            var now = DateTime.Now.Date;

            DataTable data = null;


            var sqlData = @"


SELECT 
 
ITEMS.LOGICALREF,ITEMS.CODE,ITEMS.NAME,ITEMS.SPECODE,ITEMS.SPECODE2, 
COALESCE((		SELECT (ONHAND)
		FROM LG_$FIRM$_$PERIOD$_GNTOTST WITH(NOLOCK)
		WHERE STOCKREF = ITEMS.LOGICALREF AND 
			INVENNO = 0 ),0.0) ONHAND, 
COALESCE((
		SELECT 
--$MS$--TOP 1 
        PRICE
		FROM LG_$FIRM$_PRCLIST PRC WITH(NOLOCK)
		WHERE PRC.CARDREF = ITEMS.LOGICALREF AND PRC.PTYPE = 2 
		ORDER BY ENDDATE DESC
--$PG$--LIMIT 1
),0.0) PRICE 
FROM 
(

{0}

) FILTER 
INNER JOIN
LG_$FIRM$_ITEMS ITEMS
ON FILTER.LOGICALREF = ITEMS.LOGICALREF
ORDER BY FILTER.LOGICALREF DESC
 
 
";

            var sqlFilterChanged = @"
    SELECT LOGICALREF FROM LG_$FIRM$_ITEMS
    WHERE
    ((CAPIBLOCK_EXTCREATEDDATE BETWEEN @P1 AND @P2) OR
    (CAPIBLOCK_EXTMODIFIEDDATE BETWEEN @P1 AND @P2))
    UNION
    SELECT CARDREF FROM LG_$FIRM$_PRCLIST
    WHERE
    ((CAPIBLOCK_EXTCREATEDDATE BETWEEN @P1 AND @P2) OR
    (CAPIBLOCK_EXTMODIFIEDDATE BETWEEN @P1 AND @P2)) AND PTYPE IN (1,2)
    UNION
    SELECT STOCKREF FROM LG_$FIRM$_$PERIOD$_STLINE
    WHERE
    ((DATE_ BETWEEN @P1 AND @P2) AND (LINETYPE= 0))
";
            var sqlFilterAll = @"
SELECT LOGICALREF FROM LG_$FIRM$_ITEMS
";
            if (pAll)
            {

                var sql = string.Format(sqlData, sqlFilterAll);

                data = SQL(sql, new object[] { });

            }
            else
            {


                var pDt = now;
                var pDf = now.AddDays(-1);


                if (!MY_ASK_DATE(this, "T_DATE_RANGE", ref   pDf, ref  pDt))
                    return;

                var sql = string.Format(sqlData, sqlFilterChanged);

                data = SQL(sql, new object[] { pDf, pDt });


            }

           // var convert = _SETTINGS.BUF.MY_WEBLIGHTCRM_CURR_REP_AS_MAIN;
            var coif = 1.0;

            //if (convert)
            //{
            //    var curr2 = GETSYSPRM_CURRENCYREP();
            //    var curr2_rate = ROUND(CASTASDOUBLE(
            //             SQLSCALAR("SELECT RATES1 from L_DAILYEXCHANGES WHERE CRTYPE = @P1 AND DATE_ <= @P2 ORDER BY DATE_ DESC LIMIT 1",
            //             new object[] { curr2, GETDATETODATEINT(now) })
            //             ), 4);

            //    if (ISNUMZERO(curr2_rate))
            //        throw new Exception("Set currency rate");

            //    coif = 1 / curr2_rate;
            //}

            var countSend = data.Rows.Count;
            var sendStep = 4;
            var index = 0;


            while (index < countSend)
            {

                var list = new List<Dictionary<string, string>>();

                list.Add(new Dictionary<string, string>() { 
                {"TARGET","WEBITEMS"}
                });

                for (int r = 0; r < sendStep && index < countSend; ++r, ++index)
                {

                    DataRow row = data.Rows[index];

                    var oh = CASTASDOUBLE(TAB_GETROW(row, "ONHAND"));
                    TAB_SETROW(row, "ONHAND", (oh > 0.1 ? 1.0 : 0.0));

                    var price = CASTASDOUBLE(TAB_GETROW(row, "PRICE"));
                    TAB_SETROW(row, "PRICE", ROUND(price * coif, 2));

                    {
                        var recAdd = new Dictionary<string, string>();
                        foreach (DataColumn col in data.Columns)
                            recAdd[col.ColumnName] = MY_FORMAT(TAB_GETROW(row, col.ColumnName));
                        list.Add(recAdd);
                    }

                }


                var frommain = Encoding.UTF8.GetBytes(JSONFORMAT(list));

                var zipData = ZIP(new Dictionary<string, byte[]>() { { "frommain.json", frommain } });

                var res = TOOL_WEB.connect(zipData, "SRV_FUNC_CRM_ORD_MAT_FROM_MAIN");

                var resObj = JSONPARSE<Dictionary<string, string>>(Encoding.UTF8.GetString(res)) as Dictionary<string, string>;

                if (resObj["OK"] != "1")
                    MSGUSERERROR("T_MSG_OPERATION_FAILED\nT_MATERIAL(T_CODE) = " + resObj["ERROR"]);
            }



            MSGUSERINFO("T_MSG_OPERATION_OK, T_COUNT: " + (countSend));

        }
        void MY_SYS_GEN_TASK()
        {


            if (!MSGUSERASK("T_MSG_COMMIT_BEGIN - T_GENERAL"))
                return;

            var date = DateTime.Now;

            var list = new List<Dictionary<string, string>>();

            list.Add(new Dictionary<string, string>() { 
                {"TARGET","TASK"}
                });

            list.Add(new Dictionary<string, string>() { 
                {"CMD",MY_FORMAT("DELETEOLD")},
                {"AGE",MY_FORMAT("41")},
                });

            list.Add(new Dictionary<string, string>() { 
                {"CMD",MY_FORMAT("CURRRATE")},
                {"CURR",MY_FORMAT(GETSYSPRM_CURRENCYREP())},
                {"DATE_",MY_FORMAT(date)},
                {"RATE",MY_FORMAT(ROUND(CASTASDOUBLE(
                     SQLSCALAR("SELECT RATES1 from L_DAILYEXCHANGES WHERE CRTYPE = @P1 AND DATE_ <= @P2 ORDER BY DATE_ DESC LIMIT 1",
                     new object[] { GETSYSPRM_CURRENCYREP(), GETDATETODATEINT(GET_SERVER_DATE(this).Date) })
                     ), 4))},
                });

            var frommain = Encoding.UTF8.GetBytes(JSONFORMAT(list));

            var zipData = ZIP(new Dictionary<string, byte[]>() { { "frommain.json", frommain } });

            var res = TOOL_WEB.connect(zipData, "SRV_FUNC_CRM_ORD_TASK_FROM_MAIN");

            var resObj = JSONPARSE<Dictionary<string, string>>(Encoding.UTF8.GetString(res)) as Dictionary<string, string>;

            if (resObj["OK"] == "1")
                MSGUSERINFO("T_MSG_OPERATION_OK");
            else
                MSGUSERERROR("T_MSG_OPERATION_FAILED\nT_CODE = " + resObj["ERROR"]);

        }
        void MY_SYS_IMPORT_ORDER()
        {
            if (!MSGUSERASK("T_MSG_COMMIT_BEGIN - T_IMPORT T_ORDER"))
                return;


            var now = DateTime.Now.Date;
            var pDt = now;
            var pDf = now.AddDays(-1);


            if (!MY_ASK_DATE(this, "T_DATE_RANGE", ref   pDf, ref  pDt))
                return;



            var countOurdersAllReg = 0;
            var countOurdersImportedReg = 0;

            var date = pDf.Date;

            while (date <= pDt)
            {




                var list = new List<Dictionary<string, string>>();

                //header
                list.Add(new Dictionary<string, string>() { 
                {"DATE_",MY_FORMAT(date)},
                {"TARGET","ORDERSFORMAIN"}
                });


                var frommain = Encoding.UTF8.GetBytes(JSONFORMAT(list));

                var zipData = ZIP(new Dictionary<string, byte[]>() { { "frommain.json", frommain } });


                var res = TOOL_WEB.connect(zipData, "SRV_FUNC_CRM_ORD_GET_FORMAIN");

                var resObj = JSONPARSE<List<Dictionary<string, string>>>(Encoding.UTF8.GetString(res)) as List<Dictionary<string, string>>;

                if (resObj.Count > 0)
                {
                    if (resObj[0]["OK"] != "1")
                    {
                        MSGUSERERROR("T_MSG_OPERATION_FAILED\nT_CODE = " + resObj[0]["ERROR"]);
                        return;
                    }
                }

                //
                var countOurdersAll = 0;
                var countOurdersImported = 0;
                MY_SYS_IMPORT_ORDER(resObj, out countOurdersAll, out countOurdersImported);

                countOurdersAllReg += countOurdersAll;
                countOurdersImportedReg += countOurdersImported;
                //


                date = date.AddDays(+1);
            }

            MSGUSERINFO("T_MSG_OPERATION_OK, T_ALL: " + countOurdersAllReg + ", T_IMPORT: " + countOurdersImportedReg + "");

        }
        void MY_SYS_IMPORT_ORDER(List<Dictionary<string, string>> pRawData, out int pCountAll, out int pCountImported)
        {
            pCountAll = 0;
            pCountImported = 0;

            var docs = new Dictionary<object, List<Dictionary<string, object>>>();

            for (int i = 1; i < pRawData.Count; ++i)
            {
                ++pCountAll;

                var resObjLine = pRawData[i];


                var parentref = PARSEINT(resObjLine["PARENTLREF"]);

                var lref = PARSEINT(resObjLine["LOGICALREF"]);


                var lineGlobId = "WEB" + FORMAT(lref).PadLeft(10, '0');

                var importLRef = CASTASINT(SQLSCALAR(@"
 
SELECT
--$MS$--TOP(1)
LOGICALREF 
FROM LG_$FIRM$_$PERIOD$_ORFLINE WHERE GLOBID = @P1 
--$PG$--LIMIT 1

", new object[] { lineGlobId }));

                if (importLRef > 0)
                    continue;


                var modnr = PARSESHORT(resObjLine["MODULENR"]);
                var trcode = PARSESHORT(resObjLine["TRCODE"]);

                if (modnr != 5 && trcode != 1)
                    continue;

                var docDate = PARSEDATETIME(resObjLine["DATE_"]);
                var matRef = PARSEINT(resObjLine["CARDREF"]);
                var amount = PARSEDOUBLE(resObjLine["AMOUNT"]);


                var newLine = new Dictionary<string, object>() { 
                     {"STOCKREF",matRef},
                     {"AMOUNT",amount} ,
                     {"GLOBID",lineGlobId} 
                    };

                ++pCountImported;

                if (docs.ContainsKey(parentref))
                {

                    docs[parentref].Add(newLine);
                }

                else
                {
                    var clientRef = SQLSCALAR("SELECT CLIENTREF FROM L_CAPIUSEREXT WHERE LOGICALREF = @P1", new object[] { parentref });

                    var headLine = new Dictionary<string, object>() { 
                     {"CLIENTREF",clientRef},
                     {"DUMMY_____DATETIME",docDate} ,
                     {"TRCODE",(short)1} ,


                    };

                    var listDoc = docs[parentref] = new List<Dictionary<string, object>>();
                    listDoc.Add(headLine);
                    listDoc.Add(newLine);


                }


            }


            foreach (var key in docs.Keys)
            {

                var parentref = key;
                var lines = docs[key];

                var header = lines[0];

                lines.RemoveAt(0);

                new MY_SAVEORDER(this, header, lines);

            }

        }
        void MY_SYS_EXPORTUSERS()
        {
            if (!MSGUSERASK("T_MSG_COMMIT_BEGIN - T_EXPORT - T_USER"))
                return;




            var now = DateTime.Now.Date;
            var pDt = now;
            var pDf = now.AddDays(-1);


            if (!MY_ASK_DATE(this, "T_DATE_RANGE", ref   pDf, ref  pDt))
                return;

            pDf = pDf.Date;
            pDt = pDt.Date.AddDays(+1).AddSeconds(-1);


            var data = SQL(@"

SELECT 

LOGICALREF,
NAME,
DEFINITION_,
PASSWD,
BLOCKED
FROM
L_CAPIUSEREXT 
WHERE
(CAPIBLOCK_EXTCREATEDDATE BETWEEN @P1 AND @P2) OR
(CAPIBLOCK_EXTMODIFIEDDATE BETWEEN @P1 AND @P2)
 
", new object[] { pDf, pDt });



            var list = new List<Dictionary<string, string>>();

            //header
            list.Add(new Dictionary<string, string>() { 
                {"TARGET","USER"}
                });

            foreach (DataRow row in data.Rows)
            {
                {
                    var recAdd = new Dictionary<string, string>();
                    foreach (DataColumn col in data.Columns)
                        recAdd[col.ColumnName] = MY_FORMAT(TAB_GETROW(row, col.ColumnName));
                    list.Add(recAdd);
                }
            }


            var frommain = Encoding.UTF8.GetBytes(JSONFORMAT(list));

            var zipData = ZIP(new Dictionary<string, byte[]>() { { "frommain.json", frommain } });


            var res = TOOL_WEB.connect(zipData, "SRV_FUNC_CRM_ORD_USER_FROM_MAIN");

            var resObj = JSONPARSE<Dictionary<string, string>>(Encoding.UTF8.GetString(res)) as Dictionary<string, string>;

            if (resObj["OK"] != "1")
            {
                MSGUSERERROR("T_MSG_OPERATION_FAILED\nT_CODE = " + resObj["ERROR"]);
                return;
            }


            MSGUSERINFO("T_MSG_OPERATION_OK");

        }
        void MY_SYS_EXPORT_TRANS()
        {


            if (!MSGUSERASK("T_MSG_COMMIT_BEGIN - T_EXPORT - T_DOC"))
                return;


            var now = DateTime.Now.Date;
            var pDt = now;
            var pDf = now.AddDays(-1);


            if (!MY_ASK_DATE(this, "T_DATE_RANGE", ref   pDf, ref  pDt))
                return;

            var date = pDf.Date;

            while (date <= pDt)
            {

                var data = SQL(@"
SELECT 


    CLFLINE.LOGICALREF ,
	USEREXT.LOGICALREF AS PARENTLREF ,
	CLFLINE.MODULENR AS MODULENR  ,
	CLFLINE.TRCODE AS TRCODE  ,
	CAST(0 AS SMALLINT) TRCODE2  ,
	CAST(0 AS SMALLINT) SIGN  ,
	CLFLINE.DATE_  ,
	CAST(0 AS SMALLINT) CARDTYPE  ,
	CAST(0 AS SMALLINT) CARDREF  ,
	CAST(0 AS SMALLINT) CARD2TYPE  ,
	CAST(0 AS SMALLINT) CARD2REF  ,
	CAST(0 AS FLOAT) AMOUNT  ,
	CAST(0 AS FLOAT) PRICE  ,
	CLFLINE.AMOUNT AS TOTAL  ,
	CLFLINE.REPORTRATE AS REPORTRATE ,
	CLFLINE.REPORTNET AS REPORTNET ,
	CAST(0 AS FLOAT) VAT ,
	CAST(0 AS SMALLINT) STATUS 


FROM 
LG_$FIRM$_$PERIOD$_CLFLINE CLFLINE WITH(NOLOCK) 
INNER JOIN
LG_$FIRM$_CLCARD CLCARD WITH(NOLOCK)
ON CLFLINE.CLIENTREF = CLCARD.LOGICALREF
INNER JOIN
L_CAPIUSEREXT USEREXT WITH(NOLOCK)
ON CLCARD.LOGICALREF = USEREXT.CLIENTREF AND USEREXT.FIRMNR = $FIRM$
WHERE 
CLFLINE.DATE_ = @P1 AND
CLFLINE.CLIENTREF > 0 AND
(
(CLFLINE.MODULENR = 4 AND CLFLINE.TRCODE = 38) OR
(CLFLINE.MODULENR = 4 AND CLFLINE.TRCODE = 33) OR
(CLFLINE.MODULENR = 10 AND CLFLINE.TRCODE = 1)  
) AND 
CLFLINE.CANCELLED = 0

ORDER BY CLFLINE.DATE_

", new object[] { date });




                var list = new List<Dictionary<string, string>>();

                //header
                list.Add(new Dictionary<string, string>() { 
                {"DATE_",MY_FORMAT(date)},
                {"TARGET","SHORTTRANSFROMMAIN"}
                });

                foreach (DataRow row in data.Rows)
                {
                    //var repRateByLine = CASTASDOUBLE(TAB_GETROW(row, "REPORTRATE"));

                    //if (ISNUMZERO(repRateByLine))
                    //    repRateByLine = 1.0;

                    //var price = CASTASDOUBLE(TAB_GETROW(row, "PRICE"));
                    //TAB_SETROW(row, "PRICE", ROUND(price / repRateByLine, 2));

                    //var tot = CASTASDOUBLE(TAB_GETROW(row, "TOTAL"));
                    //TAB_SETROW(row, "TOTAL", ROUND(tot / repRateByLine, 2));

                    //var tot_rep = CASTASDOUBLE(TAB_GETROW(row, "REPORTNET"));
                    //TAB_SETROW(row, "REPORTNET", ROUND(tot_rep * repRateByLine, 2));


                    var modNr = CASTASSHORT(TAB_GETROW(row, "MODULENR"));
                    var trCode = CASTASSHORT(TAB_GETROW(row, "TRCODE"));

                    //4,38 4,33
                    //10,1

                    switch (modNr * 100 + trCode)
                    {
                        case 438:
                            TAB_SETROW(row, "MODULENR", 7);
                            TAB_SETROW(row, "TRCODE", 8);
                            break;
                        case 433:
                            TAB_SETROW(row, "MODULENR", 7);
                            TAB_SETROW(row, "TRCODE", 3);
                            break;
                        case 1001:
                            TAB_SETROW(row, "MODULENR", 11);
                            TAB_SETROW(row, "TRCODE", 11);
                            break;
                        default:
                            throw new Exception("Undefined operation type and module");
                    }
                    {
                        var recAdd = new Dictionary<string, string>();
                        foreach (DataColumn col in data.Columns)
                            recAdd[col.ColumnName] = MY_FORMAT(TAB_GETROW(row, col.ColumnName));
                        list.Add(recAdd);
                    }


                }


                var frommain = Encoding.UTF8.GetBytes(JSONFORMAT(list));

                var zipData = ZIP(new Dictionary<string, byte[]>() { { "frommain.json", frommain } });


                var res = TOOL_WEB.connect(zipData, "SRV_FUNC_CRM_ORD_TRAN_FROM_MAIN");

                var resObj = JSONPARSE<Dictionary<string, string>>(Encoding.UTF8.GetString(res)) as Dictionary<string, string>;

                if (resObj["OK"] != "1")
                {
                    MSGUSERERROR("T_MSG_OPERATION_FAILED\nT_CODE = " + resObj["ERROR"]);
                    return;
                }

                date = date.AddDays(+1);
            }

            MSGUSERINFO("T_MSG_OPERATION_OK");
        }


        #endregion












        #region Help


        public static string MY_ASK_STRING(_PLUGIN pPLUGIN, string pMsg, string pDef)
        {

            DataRow[] rows_ = pPLUGIN.REF("ref.gen.string desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
            if (rows_ != null && rows_.Length > 0)
            {
                return _PLUGIN.CASTASSTRING(ISNULL(rows_[0]["VALUE"], ""));
            }
            return null;

        }


        public static double MY_ASKNUM(_PLUGIN pPLUGIN, string pMsg, double pDef, int pDecimals)
        {
            //  
            DataRow[] rows_ = pPLUGIN.REF("ref.gen.double decimals::" + FORMAT(pDecimals) + " calc::1 desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
            if (rows_ != null && rows_.Length > 0)
            {
                return _PLUGIN.CASTASDOUBLE(ISNULL(rows_[0]["VALUE"], 0.0));
            }
            return -1;
        }
        public static double MY_ASKNUM(_PLUGIN pPLUGIN, string pMsg, double pDef)
        {

            return MY_ASKNUM(pPLUGIN, pMsg, pDef, 2);
        }


        static DateTime MONTH_BEG(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }
        static DateTime MONTH_END(DateTime dt)
        {
            return MONTH_BEG(dt).AddMonths(+1).AddDays(-1);
        }

        static DateTime GET_SERVER_DATE(_PLUGIN pPLUGIN)
        {
            return CASTASDATE(pPLUGIN.SQLSCALAR(@"
--$MS$--select getdate()
--$PG$--select now()

", null));
        }




        public static bool MY_ASK_DATE(_PLUGIN pPLUGIN, string pMsg, ref DateTime pDf, ref DateTime pDt)
        {

            DataRow[] rows_ = pPLUGIN.REF("ref.gen.daterange showtime::1 desc::" + _PLUGIN.STRENCODE(pMsg)
                + " filter::"
                + "filter_DATE1," + _PLUGIN.FORMATSERIALIZE(pDf)
                 + ";filter_DATE2," + _PLUGIN.FORMATSERIALIZE(pDt)
            )
                ;

            if (rows_ != null && rows_.Length > 0)
            {
                pDf = CASTASDATE(rows_[0]["DATETIME1"]);
                pDt = CASTASDATE(rows_[0]["DATETIME2"]);

                if (pDf > pDt)
                    return false;

                return true;
            }

            return false;

        }




        public static bool MY_ASK_DATE(_PLUGIN pPLUGIN, string pMsg, ref DateTime pDate)
        {

            DataRow[] rows_ = pPLUGIN.REF("ref.gen.date desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDate));
            if (rows_ != null && rows_.Length > 0)
            {
                pDate = _PLUGIN.CASTASDATE(ISNULL(rows_[0]["DATETIME"], pDate));
                return true;
            }
            return false;

        }

        static string MY_FORMAT(object pVal)
        {

            var str = FORMAT(pVal);
            //foreach (var c in new char[] { 
            //    '\t' ,'\'','^',';','!','@','"','$','%','?','*','#', '<', '>'
            //})
            str = HTMLENCODE(str.Replace('\t', '_'));

            return str;
        }

        class TOOL_WEB
        {




            static public byte[] connect(byte[] pPostData, string pFunc)
            {



                var URL = _SETTINGS.BUF.MY_WEBLIGHTCRM_SERVER_URL;


                pPostData = pPostData ?? new byte[] { };

                using (var client = new _WebClient())
                {

                    client.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(_SETTINGS.BUF.MY_WEBLIGHTCRM_AUTH_STRING)));
                    //System.Net.WebException
                    byte[] pageData = client.UploadData(URL + "/callfunction?__func=" + pFunc, "POST", pPostData);
                    return pageData;
                }

            }




            private class _WebClient : System.Net.WebClient
            {

                protected override System.Net.WebRequest GetWebRequest(Uri uri)
                {
                    var request = base.GetWebRequest(uri);
                    request.Timeout = 5 * 1000;
                    ((System.Net.HttpWebRequest)request).ReadWriteTimeout = 20 * 1000;
                    return request;
                }
            }

        }

        #endregion




        #endregion




        #region CLASS

        class MY_SAVEORDER : IDisposable
        {

            _PLUGIN PLUGIN;

            Dictionary<string, object> HEADER;
            List<Dictionary<string, object>> LINES;


            public MY_SAVEORDER(_PLUGIN pPLUGIN, Dictionary<string, object> pHEADER, List<Dictionary<string, object>> pLINES)
            {


                PLUGIN = pPLUGIN;
                HEADER = pHEADER;
                LINES = pLINES;


                var trcode = HEADER.ContainsKey("TRCODE") ? CASTASSHORT(HEADER["TRCODE"]) : 0;

                string adp_ = "";
                switch (trcode)
                {
                    case 2:
                        adp_ = "adp.prch.doc.order.2";
                        break;
                    case 1:
                        adp_ = "adp.sls.doc.order.1";
                        break;

                    default:
                        throw new Exception("Undefined document trcode [" + trcode + "]");
                }


                PLUGIN.EXEADPCMD(new string[] { adp_ + " cmd::add" }, new DoWorkEventHandler[] { DONE }, false);//in global batch
            }

            void DONE(object sender, System.ComponentModel.DoWorkEventArgs e)
            {

                e.Result = false;


                DataSet doc_ = ((DataSet)e.Argument);

                DataTable tabHeaderInv_ = TAB_GETTAB(doc_, "STFICHE");
                DataTable tabLine_ = TAB_GETTAB(doc_, "STLINE");

                var docCode = HEADER.ContainsKey("FICHENO") ? CASTASSTRING(HEADER["FICHENO"]) : "";


                foreach (var key in HEADER.Keys)
                    TAB_SETROW(tabHeaderInv_, key, HEADER[key]);



                foreach (var rec in LINES)
                {
                    var ROW = TAB_ADDROW(tabLine_);
                    // TAB_FILLNULL(ROW);
                    foreach (var key in rec.Keys)
                    {
                        TAB_SETROW(ROW, key, rec[key]);
                    }

                }



                e.Result = true;
            }





            public void Dispose()
            {

                PLUGIN = null;
                HEADER = null;
                LINES = null;
            }

        }

        #endregion


        #endregion
