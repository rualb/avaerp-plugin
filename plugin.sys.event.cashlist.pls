 #line 2
 
       #region BODY
        //BEGIN

        const int VERSION = 9;
        const string FILE = "plugin.sys.event.cashlist.pls";


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

                    x.MY_CASHLIST_USER = s.MY_CASHLIST_USER;

                    //
                    x._USER = PLUGIN.GETSYSPRM_USER();
                    x._FIRM = PLUGIN.GETSYSPRM_FIRM();
                    x._FIRMNAME = PLUGIN.GETSYSPRM_FIRMNAME();
                    x._PERIOD = PLUGIN.GETSYSPRM_PERIOD();


                    _SETTINGS.BUF = x;

                }

                public string MY_CASHLIST_USER;


                public short _FIRM;
                public string _FIRMNAME;
                public short _PERIOD;
                public short _USER;

            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC) //, "ava.production.config")
            {

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active On User")]
            public string MY_CASHLIST_USER
            {
                get
                {
                    return (_GET("MY_CASHLIST_USER", "1,2"));
                }
                set
                {
                    _SET("MY_CASHLIST_USER", value);
                }

            }



            //

            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_CASHLIST_USER),
                     FORMAT(pPLUGIN.GETSYSPRM_USER())
                     ) >= 0;
            }

        }

        #endregion

        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "Cash List";


            public class L
            {

            }
        }

        const string event_CASHLIST_ = "hadlericom_cashlist_";

        const string event_CASHLIST_CASH_LIST = "hadlericom_cashlist_cash_list";
        const string event_CASHLIST_CASH_BY_INVOICE = "hadlericom_cashlist_cash_by_invoice";
        const string event_CASHLIST_DOCS_INFO = "hadlericom_cashlist_docs_info";
        const string event_CASHLIST_DOCS_PACKINFO = "hadlericom_cashlist_packinfo_info";
        public class _LANG
        {


            static _LANG _L = null;

            public static _LANG L
            {
                get
                {
                    if (_L == null)
                        _L = new _LANG();
                    return _L;
                }
            }


            public _LANG()
            {

                var m = this.GetType().GetMethod("lang_" + LANG_ACTIVE());
                if (m != null)
                    m.Invoke(this, null);

            }




            public string COUNT_DOC = "Docs Count";
            public string CASH_LIST = "Cash List";

            public void lang_az()
            {
                CASH_LIST = "Kassa Listi";
                COUNT_DOC = "Sənəd Sayı";
            }

            public void lang_ru()
            {

                CASH_LIST = "Список по Кассе";
                COUNT_DOC = "Кол. Док.";
            }

            public void lang_tr()
            {

                CASH_LIST = "Kasa Listesi";
                COUNT_DOC = "Fiş Sayısı";
            }
        }



        #endregion



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

                var isFormMain = fn.StartsWith("form.app");

                if (isFormMain)
                {
                    {
                        var tree = CONTROL_SEARCH(FORM, "cTreeTools");
                        if (tree != null)
                        {

                            {
                                var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
            { "_root" , "" },
		 
			{ "Text" ,_LANG.L.CASH_LIST},
			{ "ImageName" ,"cash_register_32x32"},
			{ "Name" ,event_CASHLIST_},
            };

                                RUNUIINTEGRATION(tree, args);

                            }


                            {
                                var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
            { "_root" ,event_CASHLIST_},
			{ "CmdText" ,"event name::"+event_CASHLIST_CASH_LIST},
			{ "Text" ,LANG("T_EDIT")},
			{ "ImageName" ,"money_32x32"},
			{ "Name" ,event_CASHLIST_CASH_LIST},
            };

                                RUNUIINTEGRATION(tree, args);

                            }

                            {
                                var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
            { "_root" ,event_CASHLIST_},
			{ "CmdText" ,"event name::"+event_CASHLIST_CASH_BY_INVOICE},
			{ "Text" ,LANG("T_ADD (T_INVOICE)")},
			{ "ImageName" ,"barcode_32x32"},
			{ "Name" ,event_CASHLIST_CASH_BY_INVOICE},
            };

                                RUNUIINTEGRATION(tree, args);

                            }




                            {
                                var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
            { "_root" ,event_CASHLIST_},
			{ "CmdText" ,"event name::"+event_CASHLIST_DOCS_INFO},
			{ "Text" ,LANG("T_INFO - T_DOCS")},
			{ "ImageName" ,"info_32x32"},
			{ "Name" ,event_CASHLIST_DOCS_INFO},
            };

                                RUNUIINTEGRATION(tree, args);

                            }
                            {
                                var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
            { "_root" ,event_CASHLIST_},
			{ "CmdText" ,"event name::"+event_CASHLIST_DOCS_PACKINFO},
			{ "Text" ,LANG("T_INFO - T_PACKAGE")},
			{ "ImageName" ,"info_32x32"},
			{ "Name" ,event_CASHLIST_DOCS_PACKINFO},
            };

                                RUNUIINTEGRATION(tree, args);

                            }



                        }

                    }


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

                    case event_CASHLIST_CASH_LIST:
                        {
                            if (!MY_TOOL.MY_IS_FORM_OPEN(typeof(TY_FORMS.FormCashList)))
                            {
                                var f = new TY_FORMS.FormCashList(this, null);
                                f.Show();
                            }
                        }
                        break;
                    case event_CASHLIST_CASH_BY_INVOICE:
                        {
                            if (!MY_TOOL.MY_IS_FORM_OPEN(typeof(TY_FORMS.FormCashByInvoice)))
                            {
                                var f = new TY_FORMS.FormCashByInvoice(this, null);
                                f.Show();
                            }
                        }
                        break;

                    case event_CASHLIST_DOCS_INFO:
                        {
                            DateTime date = CASTASDATE("");
                            var cmdLine = arg3 as string;
                            if (cmdLine != null)
                            {
                                date = CASTASDATE(CMDLINEGETARG(cmdLine, "date")).Date;
                            }
                            MY_SHOW_DOCS_INFO(date);
                        }
                        break;
                    case event_CASHLIST_DOCS_PACKINFO:
                        {
                            DateTime date = CASTASDATE("");
                            var cmdLine = arg3 as string;
                            if (cmdLine != null)
                            {
                                date = CASTASDATE(CMDLINEGETARG(cmdLine, "date")).Date;
                            }
                            MY_SHOW_PACKAGE_INFO(date);
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



        private void MY_SHOW_PACKAGE_INFO(DateTime pDate)
        {
            if (ISEMPTY(pDate))
            {
                pDate = DateTime.Now.Date;

                var date = MY_TOOL.MY_ASK_DATE(this, "T_DATE", pDate);
                if (date == null || ISEMPTY(date.Value))
                    return;

                pDate = date.Value.Date;

            }



            var infoBySpCode = SQL(@"

SELECT 
COALESCE((SELECT CONCAT(P.CODE,'/',P.NAME) FROM LG_$FIRM$_PROJECT AS P WHERE P.LOGICALREF = KST.PROJECTREF),'') AS PACKAGE,
KST.DEPARTMENT AS DEP,
CL.SPECODE AS CODE,
COUNT(*) AS COUNT,
SUM(KST.AMOUNT) AS AMOUNT 

FROM LG_$FIRM$_$PERIOD$_KSLINES AS KST 
LEFT JOIN
LG_$FIRM$_$PERIOD$_CLFLINE CLT ON KST.LOGICALREF = CLT.SOURCEFREF AND CLT.MODULENR = 10 AND CLT.TRCODE IN (1,2)
LEFT JOIN
LG_$FIRM$_CLCARD AS CL ON CLT.CLIENTREF = CL.LOGICALREF 

WHERE KST.DATE_ = @P1 AND KST.TRCODE = 11 AND KST.CANCELLED = 0
GROUP BY KST.PROJECTREF,KST.DEPARTMENT,CL.SPECODE
ORDER BY KST.PROJECTREF ASC,KST.DEPARTMENT ASC,CL.SPECODE ASC

", new object[] { pDate });

            var infoGlob = SQL(@"

SELECT 
COALESCE((SELECT CONCAT(P.CODE,'/',P.NAME) FROM LG_$FIRM$_PROJECT AS P WHERE P.LOGICALREF = KST.PROJECTREF),'') AS PACKAGE, 
COUNT(*) AS COUNT,
SUM(KST.AMOUNT) AS AMOUNT  
from LG_$FIRM$_$PERIOD$_KSLINES AS KST 
WHERE KST.DATE_ = @P1 AND KST.TRCODE = 11 AND KST.CANCELLED = 0
GROUP BY KST.PROJECTREF 
ORDER BY KST.PROJECTREF ASC 

", new object[] { pDate });




            var res = new StringBuilder();

            res.AppendLine("<html>");
            res.AppendLine(@" 

<style>
            table, th, td {
                border: 1px solid #dbdbdb;
                border-collapse: collapse;
                white-space: nowrap;
                padding: 3px;
 
            }
            table, h1, h2, h3, h4, h5, h6 {
                font-family: Segoe UI, Verdana, Aral;
            }
            th {
                font-weight:bold
            }

         tr:hover td {
            background-color: #BCCFD6;
            }

</style>
 <body style=''>
");


            //name
            res.AppendLine(string.Format("<h2 style='width:100%;text-align:center'>{0}</h2>",
               HTMLENCODE(_LANG.L.CASH_LIST + " - " + LANG("T_DOCS"))
                  ));


            //filter
            {

                var lines = new List<string[]>();

                lines.Add(new string[] { LANG("T_DATE "), FORMAT(pDate, "yyyy-MM-dd") });


                res.AppendLine("<table>");

                foreach (var row in lines)
                {

                    res.AppendLine("<tr>");
                    foreach (var cell in row)
                    {
                        res.AppendLine(string.Format(
                        "<td>{0}</td>",
                         cell
                       ));
                    }
                    res.AppendLine("</tr>");

                }


                res.AppendLine("</table>");
            }



            //body
            {




                foreach (var data in new DataTable[] { infoGlob, infoBySpCode })
                {
                    var hasCode = data.Columns.Contains("CODE");
                    var hasDep = data.Columns.Contains("DEP");
                    res.AppendLine("<br/>");

                    res.AppendLine("<table style=';'>");

                    res.AppendLine("<tr>");
                    foreach (var cell in new string[] { 
                                LANG("T_PACKAGE"), 
                                (hasDep?  LANG("T_DEP"):null),
                                (hasCode?  LANG("T_CODE"):null), 
                                LANG("T_COUNT"),
                                LANG("T_TOTAL"),
                                
                            })
                        if (cell != null)
                        {
                            res.AppendLine(string.Format(
                            "<th>{0}</th>",
                             cell
                           ));
                        }
                    res.AppendLine("</tr>");


                    for (int i = 0; i < data.Rows.Count; ++i)
                    {





                        var indx = i + 1;
                        //

                        var isDark = (i % 2 == 1);
                        var isBold = false;
                        var row = data.Rows[i];
                        string[] arrCell = new string[] {  
                                        FORMAT(CASTASSTRING(TAB_GETROW(row, "PACKAGE"))),
                                        (hasDep?  CASTASSTRING(TAB_GETROW(row, "DEP")):null), 
                                       (hasCode?  CASTASSTRING(TAB_GETROW(row, "CODE")):null), 
                                        FORMAT(ROUND(CASTASDOUBLE(TAB_GETROW(row, "COUNT")), 2),"#,#0.#") ,
                                        FORMAT(ROUND(CASTASDOUBLE(TAB_GETROW(row, "AMOUNT")), 2),"#,#0.00") , 
                                    };

                        var backColor = "#FFFFFF";

                        if (isDark)
                            backColor = "#F0F0F0";


                        var fontWeight = isBold ? "bold" : "normal";
                        var numColsStartIndx = 0 + 1 + (hasCode ? 1 : 0) + (hasDep ? 1 : 0);

                        res.AppendLine("<tr style='background-color:" + backColor + ";font-weight:" + fontWeight + "'>");
                        // foreach (var cellVal in arrCell)
                        for (int c = 0; c < arrCell.Length; ++c)
                        {
                            var cellVal = arrCell[c];
                            if (cellVal != null)
                            {
                                var width = "auto"; //arrCellWidth[c]
                                res.AppendLine(string.Format(
                                    "<td style='width:" + (width) + ";text-align: " + (c >= numColsStartIndx ? "right" : "") + ";'>{0}</td>",
                                 HTMLESC(cellVal)
                               ));
                            }
                        }
                        res.AppendLine("</tr>");




                    }


                    res.AppendLine("</table>");


                }
            }


            MSGUSERINFO(res.ToString());

        }
        private void MY_SHOW_DOCS_INFO(DateTime pDate)
        {
            if (ISEMPTY(pDate))
            {
                pDate = DateTime.Now.Date;

                var date = MY_TOOL.MY_ASK_DATE(this, "T_DATE", pDate);
                if (date == null || ISEMPTY(date.Value))
                    return;

                pDate = date.Value.Date;

            }



            var infoBySpCode = SQL(@"

SELECT KST.CANCELLED AS STATUS,
KST.DEPARTMENT AS DEP,
CL.SPECODE AS CODE,
COUNT(*) AS COUNT,
SUM(KST.AMOUNT) AS AMOUNT 

FROM LG_$FIRM$_$PERIOD$_KSLINES AS KST 
LEFT JOIN
LG_$FIRM$_$PERIOD$_CLFLINE CLT ON KST.LOGICALREF = CLT.SOURCEFREF AND CLT.MODULENR = 10 AND CLT.TRCODE IN (1,2)
LEFT JOIN
LG_$FIRM$_CLCARD AS CL ON CLT.CLIENTREF = CL.LOGICALREF 

WHERE KST.DATE_ = @P1 AND KST.TRCODE = 11 
GROUP BY KST.CANCELLED,KST.DEPARTMENT,CL.SPECODE
ORDER BY KST.CANCELLED ASC,KST.DEPARTMENT ASC,CL.SPECODE ASC

", new object[] { pDate });

            var infoGlob = SQL(@"

SELECT KST.CANCELLED AS STATUS, 
COUNT(*) AS COUNT,
SUM(KST.AMOUNT) AS AMOUNT  
from LG_$FIRM$_$PERIOD$_KSLINES AS KST 
WHERE KST.DATE_ = @P1 AND KST.TRCODE = 11
GROUP BY KST.CANCELLED 
ORDER BY KST.CANCELLED ASC 

", new object[] { pDate });




            var res = new StringBuilder();

            res.AppendLine("<html>");
            res.AppendLine(@" 

<style>
            table, th, td {
                border: 1px solid #dbdbdb;
                border-collapse: collapse;
                white-space: nowrap;
                padding: 3px;
 
            }
            table, h1, h2, h3, h4, h5, h6 {
                font-family: Segoe UI, Verdana, Aral;
            }
            th {
                font-weight:bold
            }

         tr:hover td {
            background-color: #BCCFD6;
            }

</style>
 <body style=''>
");


            //name
            res.AppendLine(string.Format("<h2 style='width:100%;text-align:center'>{0}</h2>",
               HTMLENCODE(_LANG.L.CASH_LIST + " - " + LANG("T_DOCS"))
                  ));


            //filter
            {

                var lines = new List<string[]>();

                lines.Add(new string[] { LANG("T_DATE "), FORMAT(pDate, "yyyy-MM-dd") });


                res.AppendLine("<table>");

                foreach (var row in lines)
                {

                    res.AppendLine("<tr>");
                    foreach (var cell in row)
                    {
                        res.AppendLine(string.Format(
                        "<td>{0}</td>",
                         cell
                       ));
                    }
                    res.AppendLine("</tr>");

                }


                res.AppendLine("</table>");
            }



            //body
            {




                foreach (var data in new DataTable[] { infoGlob, infoBySpCode })
                {
                    var hasCode = data.Columns.Contains("CODE");
                    var hasDep = data.Columns.Contains("DEP");
                    res.AppendLine("<br/>");

                    res.AppendLine("<table style=';'>");

                    res.AppendLine("<tr>");
                    foreach (var cell in new string[] { 
                                LANG("T_STATUS"), 
                                (hasDep?  LANG("T_DEP"):null),
                                (hasCode?  LANG("T_CODE"):null), 
                                LANG("T_COUNT"),
                                LANG("T_TOTAL"),
                                
                            })
                        if (cell != null)
                        {
                            res.AppendLine(string.Format(
                            "<th>{0}</th>",
                             cell
                           ));
                        }
                    res.AppendLine("</tr>");


                    for (int i = 0; i < data.Rows.Count; ++i)
                    {





                        var indx = i + 1;
                        //

                        var isDark = (i % 2 == 1);
                        var isBold = false;
                        var row = data.Rows[i];
                        string[] arrCell = new string[] {  
                                        RESOLVESTR("[list::LIST_GEN_CANCELED/" + FORMAT(CASTASSHORT(TAB_GETROW(row, "STATUS"))) + "]"),
                                        (hasDep?  CASTASSTRING(TAB_GETROW(row, "DEP")):null), 
                                       (hasCode?  CASTASSTRING(TAB_GETROW(row, "CODE")):null), 
                                        FORMAT(ROUND(CASTASDOUBLE(TAB_GETROW(row, "COUNT")), 2),"#,#0.#") ,
                                        FORMAT(ROUND(CASTASDOUBLE(TAB_GETROW(row, "AMOUNT")), 2),"#,#0.00") , 
                                    };

                        var backColor = "#FFFFFF";

                        if (isDark)
                            backColor = "#F0F0F0";


                        var fontWeight = isBold ? "bold" : "normal";
                        var numColsStartIndx = 0 + 1 + (hasCode ? 1 : 0) + (hasDep ? 1 : 0);

                        res.AppendLine("<tr style='background-color:" + backColor + ";font-weight:" + fontWeight + "'>");
                        // foreach (var cellVal in arrCell)
                        for (int c = 0; c < arrCell.Length; ++c)
                        {
                            var cellVal = arrCell[c];
                            if (cellVal != null)
                            {
                                var width = "auto"; //arrCellWidth[c]
                                res.AppendLine(string.Format(
                                    "<td style='width:" + (width) + ";text-align: " + (c >= numColsStartIndx ? "right" : "") + ";'>{0}</td>",
                                 HTMLESC(cellVal)
                               ));
                            }
                        }
                        res.AppendLine("</tr>");




                    }


                    res.AppendLine("</table>");


                }
            }


            MSGUSERINFO(res.ToString());

        }

        static int[] SELECT_CASH_IN(_PLUGIN pPLUGIN, DateTime pDate, short pCancelled, short pDocDep)
        {


            pDate = pDate.Date;


            DataTable info = null;

            if (pDocDep > 0)
            {

                info = pPLUGIN.SQL(@"
SELECT KST.LOGICALREF 

FROM LG_$FIRM$_$PERIOD$_KSLINES AS KST 
LEFT JOIN 
LG_$FIRM$_$PERIOD$_CLFLINE CLT ON KST.LOGICALREF = CLT.SOURCEFREF AND CLT.MODULENR = 10 AND CLT.TRCODE IN (1,2)
LEFT JOIN
LG_$FIRM$_CLCARD AS CL ON CLT.CLIENTREF = CL.LOGICALREF 
WHERE KST.DATE_ = @P1 AND KST.TRCODE = 11 AND KST.CANCELLED = @P2 AND (KST.DEPARTMENT = @P3 )
 

", new object[] { pDate, pCancelled, pDocDep });

                TAB_FILLNULL(info);

            }


            var list = new List<int>();
            if (info != null)
            {
                foreach (DataRow r in info.Rows)
                    list.Add(CASTASINT(r["LOGICALREF"]));

                list.Sort();
            }
            return list.ToArray();


        }

        static DataTable SELECT_CASH_IN(_PLUGIN pPLUGIN, object pDocLref)
        {


            var tab = (pPLUGIN.SQL(@"

SELECT 
KST.LOGICALREF,
KST.FICHENO,
KST.DATE_,
KST.LINEEXP ,
KST.CUSTTITLE,
KST.TRANGRPNO,
KST.CYPHCODE,
KST.DEPARTMENT,
COALESCE(CONCAT(CL.CODE,'/',CL.DEFINITION_,'/',CL.SPECODE),'') AS CLCARD_TITLE,
KST.AMOUNT,
COALESCE((SELECT (DEBIT-CREDIT) FROM LG_$FIRM$_$PERIOD$_GNTOTCL WHERE CARDREF = CLT.CLIENTREF AND TOTTYP = 1),0) AS BALANCE

FROM LG_$FIRM$_$PERIOD$_KSLINES AS KST 
LEFT JOIN
LG_$FIRM$_$PERIOD$_CLFLINE CLT ON KST.LOGICALREF = CLT.SOURCEFREF AND CLT.MODULENR = 10 AND CLT.TRCODE IN (1,2)
LEFT JOIN
LG_$FIRM$_CLCARD AS CL ON CLT.CLIENTREF = CL.LOGICALREF 


WHERE KST.LOGICALREF = @P1
AND KST.TRCODE = 11 AND KST.CANCELLED = @P2 

", new object[] { pDocLref, 1 }));
            //slect cancelled
            TAB_FILLNULL(tab);

            return tab;


        }


        public static string MY_CHOOSE_SQL(string pSqlMs, string pSqlPg)
        {

            if (ISMSSQL())
                return pSqlMs;

            if (ISPOSTGRESQL())
                return pSqlPg;


            throw new Exception("Undefined datasource");
        }



        #region TOOLS

        class MY_TOOL
        {

            public static string MY_ASK_STRING(_PLUGIN pPLUGIN, string pMsg, string pDef, string pCmdAppend = null)
            {

                DataRow[] rows_ = pPLUGIN.REF("ref.gen.string " + (ISEMPTY(pCmdAppend) ? "" : pCmdAppend) + "desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
                if (rows_ != null && rows_.Length > 0)
                {
                    return _PLUGIN.CASTASSTRING(ISNULL(rows_[0]["VALUE"], ""));
                }
                return null;

            }
            public static double MY_ASK_NUM(_PLUGIN pPLUGIN, string pMsg, double pDef, int pDecimals)
            {
                //  
                DataRow[] rows_ = pPLUGIN.REF("ref.gen.double decimals::" + FORMAT(pDecimals) + " calc::1 desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
                if (rows_ != null && rows_.Length > 0)
                {
                    return _PLUGIN.CASTASDOUBLE(ISNULL(rows_[0]["VALUE"], 0.0));
                }
                return -1;
            }

            public static void BEEPERR()
            {
                var t = new System.Threading.Tasks.Task(() =>
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        System.Media.SystemSounds.Asterisk.Play();
                        System.Threading.Thread.Sleep(600);
                    }
                });

                t.Start();

            }

            public static bool MY_IS_FORM_OPEN(Type pFormType)
            {
                try
                {

                    foreach (var f in Application.OpenForms)
                        if (f.GetType() == pFormType)
                        {
                            var z = f as Form;
                            if (z != null)
                            {
                                z.WindowState = FormWindowState.Maximized;
                                z.Activate();
                                return true;
                            }
                        }


                }
                catch (Exception exc)
                {

                    RUNTIMELOG(exc.ToString());

                }

                return false;
            }


            public static string MY_ASK_STRING(_PLUGIN pPLUGIN, string pMsg, string pDef)
            {

                DataRow[] rows_ = pPLUGIN.REF("ref.gen.string desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
                if (rows_ != null && rows_.Length > 0)
                {
                    return _PLUGIN.CASTASSTRING(ISNULL(rows_[0]["VALUE"], ""));
                }
                return null;

            }



            public static DateTime? MY_ASK_DATE(_PLUGIN pPLUGIN, string pMsg, DateTime? pDef = null)
            {

                if (pDef == null)
                    pDef = DateTime.Now;

                DataRow[] rows_ = pPLUGIN.REF("ref.gen.date desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
                if (rows_ != null && rows_.Length > 0)
                {
                    return _PLUGIN.CASTASDATE(rows_[0]["DATE_"]);
                }
                return null;
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




        }


        class TY_FORMS
        {

            public class FormCashList : Form
            {
                _PLUGIN PLUGIN;

                Filter filter = new Filter();

                public FormCashList(_PLUGIN pPLUGIN, Filter pFilter)
                {
                    PLUGIN = pPLUGIN;
                    filter = pFilter ?? new Filter() { date = DateTime.Now.Date };

                    this.Text = string.Format("{0}", _LANG.L.CASH_LIST);

                    //filter.packageNr = filter.generatePackNr(PLUGIN);

                    init();

                    bindFilterDo();

                    refreshData();
                }


                public void init()
                {

                    this.Icon = CTRL_FORM_ICON();
                    this.Size = new Size(1000, 700);
                    this.AutoSize = true;
                    //form.BackColor = Color.White;
                    this.StartPosition = FormStartPosition.CenterScreen;

                    var panelMain = new Panel() { Dock = DockStyle.Fill };
                    var tabControl = new TabControl() { Dock = DockStyle.Fill, AutoSize = true };
                    var tabPageDocsAll = new TabPage() { Text = PLUGIN.LANG("T_DOC_FIN_CASH_11 (T_CANCELED)") };
                    var tabPageDocsPacked = new TabPage() { Text = PLUGIN.LANG("T_PAIDIN") };


                    tabControl.Controls.Add(tabPageDocsAll);
                    tabControl.Controls.Add(tabPageDocsPacked);



                    this.Controls.Add(panelMain);
                    panelMain.Controls.Add(tabControl);


                    this.WindowState = FormWindowState.Maximized;

                    var labelWidth = 170;

                    var btnClose = new Button() { Text = PLUGIN.LANG("T_CLOSE"), Image = RES_IMAGE("close_16x16"), ImageAlign = ContentAlignment.MiddleLeft, Width = labelWidth, Dock = DockStyle.Right };
                    btnClose.Click += (s, arg) => { this.Close(); };

                    var btnInfoDocs = new Button() { Text = PLUGIN.LANG("T_INFO - T_DOCS"), Image = RES_IMAGE("info_16x16"), ImageAlign = ContentAlignment.MiddleLeft, Width = labelWidth, Dock = DockStyle.Left };
                    btnInfoDocs.Click += (s, arg) => { this.showDocsInfo(); };




                    var btnLoadDocs = new Button()
                    {
                        Text = PLUGIN.LANG("T_FILL"),
                        Image = RES_IMAGE("gas_16x16"),
                        ImageAlign = ContentAlignment.MiddleLeft,
                        Width = labelWidth
                    };
                    btnLoadDocs.Click += (s, arg) => { this.loadAllDocs(); };

                    var btnAddDoc = new Button()
                    {
                        Text = PLUGIN.LANG("T_ADD"),
                        Image = RES_IMAGE("doc_16x16"),
                        ImageAlign = ContentAlignment.MiddleLeft,
                        Width = labelWidth
                    };
                    btnAddDoc.Click += (s, arg) => { this.addCashManual(); };

                    var btnCalcPack = new Button()
                    {
                        Text = PLUGIN.LANG("T_PAIDIN"),
                        Image = RES_IMAGE("done_16x16"),
                        ImageAlign = ContentAlignment.MiddleLeft,
                        Width = labelWidth
                    };
                    btnCalcPack.Click += (s, arg) => { this.moveAllToPack(); };



                    //var btnSetPrice = new Button() { Width = 80, Text = PLUGIN.LANG("T_SAVE"), Image = RES_IMAGE("checked_16x16"), ImageAlign = ContentAlignment.MiddleLeft };
                    //  btnSetPrice.Click += (s, arg) => { TAB_SETROW(pRow, "PRICE", (double)priceBox.Value); this.Close(); };



                    var panelFilterDateRange = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Bottom, AutoSize = true };

                    var panelPacketAction = new FlowLayoutPanel() { Dock = DockStyle.Bottom, AutoSize = true };
                    var panelFilterCl = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Bottom, AutoSize = true };
                    var panelBtn = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Bottom, AutoSize = true };
                    var panelBtnPack = new Panel() { Dock = DockStyle.Bottom, Height = 25 };
                    var panelBtnGen = new Panel() { Dock = DockStyle.Bottom, Height = 25 };
                    var panelGenInfoDocsAll = new FlowLayoutPanel() { Dock = DockStyle.Bottom, Height = 25 };
                    var panelGenInfoDocsPacked = new FlowLayoutPanel() { Dock = DockStyle.Bottom, Height = 25 };
                    //







                    // panelMain.Controls.Add(panelPacketAction);
                    panelMain.Controls.Add(panelGenInfoDocsAll);
                    panelMain.Controls.Add(panelGenInfoDocsPacked);
                    panelMain.Controls.Add(panelBtnGen);

                    panelBtn.Controls.AddRange(new Control[] { 
                        btnLoadDocs,
                    btnAddDoc,
                    btnCalcPack 
                    });




                    panelBtnGen.Controls.Add(btnClose);

                    panelBtnGen.Controls.Add(btnInfoDocs);

                    //



                    panelFilterDateRange.Controls.AddRange(
                        new Control[]{
                            new Label(){Text = PLUGIN.LANG("T_DATE"), Width = labelWidth/2, TextAlign = ContentAlignment.MiddleLeft,Dock = DockStyle.Left},
                            new DateTimePicker() { Value = filter.date , Name = "filter_date",Width=labelWidth },
                   

                            new Button(){Text=PLUGIN.LANG("T_TODAY"),Image = RES_IMAGE("run_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_filter_date_today",Width=labelWidth/2 }, 
                            new Button(){Text=PLUGIN.LANG("-1 T_DAY"),Image = RES_IMAGE("run_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_filter_date_day_dec",Width=labelWidth/2 }, 
                            new Button(){Text=PLUGIN.LANG("+1 T_DAY"),Image = RES_IMAGE("run_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_filter_date_day_inc",Width=labelWidth/2 }, 
                        
                            ////////////


                          

                            }
                    );

                    {


                        panelPacketAction.Controls.AddRange(
                          new Control[]{
                        
                          

                            }
                          );


                        panelBtnPack.Controls.AddRange(
                          new Control[]{
                          
                           new Button(){Text=PLUGIN.LANG("T_SAVE"),Image = RES_IMAGE("save_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_package_save",Width=labelWidth,Dock = DockStyle.Right  },
                            new Button(){Text=PLUGIN.LANG("T_PRINT"),Image = RES_IMAGE("print_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_package_print",Width=labelWidth,Dock = DockStyle.Right  },
                          

                            }
                          );

                    }

                    panelFilterCl.Controls.AddRange(
                      new Control[]{
                           
                           new Label(){Text = PLUGIN.LANG("T_DEP"), Width = labelWidth/2, TextAlign = ContentAlignment.MiddleLeft,Dock = DockStyle.Left},
                            new NumericUpDown() {   Value = filter.docDep, Name = "filter_docdep",Width=labelWidth/2,Minimum = 0, Maximum = short.MaxValue, DecimalPlaces = 0},
                           new Button(){Text="",Image = RES_IMAGE("find_16x16"),ImageAlign = ContentAlignment.MiddleCenter,Name = "do_filter_docdep",Width=30 },
                          // new TextBox(){Width=700,ReadOnly=true,Name="desc_filter_client"}
                            }
                      );

                    {


                        panelGenInfoDocsAll.Controls.AddRange(new Control[]{

                              
                              new Label(){Text = PLUGIN.LANG("T_TOTAL (T_ALL)"), Width = labelWidth/2, TextAlign = ContentAlignment.MiddleRight,ForeColor = Color.Green },
                            new TextBox() {   Text = "0", Name = "val_docs_all_amount",Width=labelWidth/2,ReadOnly=true, TextAlign = HorizontalAlignment.Right },


                             new Label(){Text = PLUGIN.LANG("T_COUNT"), Width = labelWidth/2, TextAlign = ContentAlignment.MiddleRight },
                            new TextBox() {   Text = "0", Name = "val_docs_all_count",Width=labelWidth/2,ReadOnly=true, TextAlign = HorizontalAlignment.Right  },
                        

                          
                            
                            });

                    }


                    {



                        panelGenInfoDocsPacked.Controls.AddRange(new Control[]{
 

                              new Label(){Text = PLUGIN.LANG("T_TOTAL (T_PAIDIN)"), Width = labelWidth/2, TextAlign = ContentAlignment.MiddleRight,ForeColor = Color.Blue },
                            new TextBox() {   Text = "0", Name = "val_docs_pack_amount",Width=labelWidth/2,ReadOnly=true, TextAlign = HorizontalAlignment.Right  },


                             new Label(){Text = PLUGIN.LANG("T_COUNT"), Width = labelWidth/2, TextAlign = ContentAlignment.MiddleRight },
                            new TextBox() {   Text = "0", Name = "val_docs_pack_count",Width=labelWidth/2,ReadOnly=true, TextAlign = HorizontalAlignment.Right },
                        
                         
                         
                         

                            
                            });

                    }



                    #region GRIDS

                    var gridDocsAll = createGrid("grid_docs_all");
                    var gridDocsPacked = createGrid("grid_docs_packed");

                    tabPageDocsPacked.Controls.Add(gridDocsPacked);
                    // tabPageDocsPacked.Controls.Add(panelPacketAction);
                    tabPageDocsPacked.Controls.Add(panelBtnPack);



                    tabPageDocsAll.Controls.Add(gridDocsAll);

                    tabPageDocsAll.Controls.Add(panelFilterDateRange);
                    tabPageDocsAll.Controls.Add(panelFilterCl);
                    tabPageDocsAll.Controls.Add(panelBtn);




                    foreach (var g in new DataGridView[] { gridDocsAll, gridDocsPacked })
                        g.Columns.AddRange(
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_INDEX"), DataPropertyName = "LOGICALREF", Width = 60, Frozen = true },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_DATE"), DataPropertyName = "DATE_", Width = 80, Frozen = true },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_NR"), DataPropertyName = "FICHENO", Width = 80, Frozen = false },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_GROUP"), DataPropertyName = "TRANGRPNO", Width = 100, Frozen = false },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_INFO"), DataPropertyName = "LINEEXP", Width = 120, Frozen = false },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_INFO 2"), DataPropertyName = "CUSTTITLE", Width = 120, Frozen = false },

                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_AUTHCODE"), DataPropertyName = "CYPHCODE", Width = 50, Frozen = false },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_DEP"), DataPropertyName = "DEPARTMENT", Width = 40, Frozen = false },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_CONTRACTOR"), DataPropertyName = "CLCARD_TITLE", Width = 260, Frozen = false },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_TOTAL"), DataPropertyName = "AMOUNT", Width = 80, Frozen = false }
                                           );

                    gridDocsAll.Columns.Add(new DataGridViewButtonColumn() { Width = 80, Text = PLUGIN.LANG("T_PAIDIN"), Tag = "cmd/addtopack", UseColumnTextForButtonValue = true });
                    gridDocsPacked.Columns.Add(new DataGridViewButtonColumn() { Width = 80, Text = PLUGIN.LANG("T_DELETE"), Tag = "cmd/delfrompack", UseColumnTextForButtonValue = true });

                    foreach (var x in new DataGridView[] { gridDocsAll, gridDocsPacked })
                        foreach (DataGridViewColumn colObj in x.Columns)
                        {
                            colObj.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            colObj.SortMode = DataGridViewColumnSortMode.Programmatic;

                            switch (colObj.DataPropertyName)
                            {
                                case "AMOUNT":
                                    colObj.DefaultCellStyle.Format = "#,0.00;-#,0.00;''";
                                    colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                                    break;

                                case "DATE_":
                                    colObj.DefaultCellStyle.Format = "d";
                                    colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                                    break;
                            }

                            var colCmd = colObj.Tag as string;

                            if (colCmd != null)
                            {
                                switch (colCmd)
                                {
                                    case "cmd/addtopack":
                                        colObj.DefaultCellStyle.ForeColor = Color.Green;
                                        break;
                                    case "cmd/delfrompack":
                                        colObj.DefaultCellStyle.ForeColor = Color.Blue;
                                        break;
                                }

                            }
                        }

                    foreach (var x in new DataGridView[] { gridDocsAll, gridDocsPacked })
                    {

                        x.KeyDown += (sender, e) =>
                        {
                            try
                            {
                                DataGridView grid_ = sender as DataGridView;
                                if (grid_ == null)
                                    return;

                                if (e.Shift && !e.Control && !e.Alt && e.KeyCode == Keys.Delete)
                                {
                                    var data = TOOL_GRID.GET_GRID_ROW_DATA(grid_);
                                    removeRecord(data);

                                    e.SuppressKeyPress = true;
                                    return;
                                }
                            }
                            catch (Exception exc)
                            {
                                /// PLUGIN.MSGUSERERROR(exc.Message);
                                PLUGIN.LOG(exc);

                            }
                        };


                        x.CellClick += (sender, e) =>
                        {

                            try
                            {
                                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                                    return;

                                DataGridView grid_ = sender as DataGridView;
                                if (grid_ == null)
                                    return;

                                var colObj_ = e.ColumnIndex >= 0 ? grid_.Columns[e.ColumnIndex] : null;
                                if (colObj_ == null)
                                    return;


                                var colCmd = colObj_.Tag as string;

                                if (colCmd != null)
                                {
                                    doCmd(colCmd);
                                }
                                var tab = grid_.DataSource as DataTable;

                                if (tab == null)
                                    return;


                            }
                            catch (Exception exc)
                            {
                                /// PLUGIN.MSGUSERERROR(exc.Message);
                                PLUGIN.LOG(exc);

                            }


                        };


                        ///////////
                        x.CellMouseDoubleClick += (sender, e) =>
                        {
                            try
                            {
                                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                                    return;


                                DataGridView grid_ = sender as DataGridView;
                                if (grid_ == null)
                                    return;

                                var tab_ = grid_.DataSource as DataTable;

                                var colObj_ = e.ColumnIndex >= 0 ? grid_.Columns[e.ColumnIndex] : null;
                                if (colObj_ == null)
                                    return;

                                if (colObj_.Tag != null)
                                {
                                    //cmd/button column
                                    return;
                                }

                                DataRow ROW = TOOL_GRID.GET_GRID_ROW_DATA(grid_);
                                if (ROW == null)
                                    return;

                                try
                                {
                                    var lref = FORMAT(TAB_GETROW(ROW, "LOGICALREF"));
                                    var cmdEdit = "adp.fin.doc.cash.11 cmd::edit lref::" + lref;
                                    var cmdView = "adp.fin.doc.cash.11 cmd::view lref::" + lref;

                                    foreach (var cmd in new string[] { /*cmdEdit,*/ cmdView })
                                        if (PLUGIN.EXEADPCMDALLOWED(cmd, null))
                                            PLUGIN.EXECMDTEXT(cmd);

                                }
                                catch (Exception exc)
                                {
                                    PLUGIN.LOG(exc);
                                }

                            }
                            catch (Exception exc)
                            {
                                // PLUGIN.MSGUSERERROR(exc.Message);
                                PLUGIN.LOG(exc);
                            }

                        };


                        x.CellFormatting += (sender, e) =>
                        {
                            try
                            {
                                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                                    return;

                                DataGridView grid_ = sender as DataGridView;
                                if (grid_ == null)
                                    return;

                                var rowObj_ = TOOL_GRID.GET_GRID_ROW(grid_, e.RowIndex);
                                if (rowObj_ == null)
                                    return;

                                var colObj_ = e.ColumnIndex >= 0 ? grid_.Columns[e.ColumnIndex] : null;
                                if (colObj_ == null)
                                    return;


                                var rowData_ = TOOL_GRID.GET_GRID_ROW_DATA(rowObj_);
                                if (rowData_ == null)
                                    return;


                            }
                            catch (Exception exc)
                            {
                                PLUGIN.LOG(exc);
                                ///  PLUGIN.MSGUSERERROR(exc.Message);
                            }
                        };



                        x.CellPainting += (sender, e) =>
                        {

                            try
                            {


                                if (e.ColumnIndex < 0 || e.RowIndex < 0)
                                    return;

                                DataGridView grid_ = sender as DataGridView;
                                if (grid_ == null)
                                    return;

                                var bgColor = grid_.DefaultCellStyle.BackColor;

                                var rowObj_ = TOOL_GRID.GET_GRID_ROW(grid_, e.RowIndex);
                                if (rowObj_ == null)
                                    return;

                                var colObj_ = e.ColumnIndex >= 0 ? grid_.Columns[e.ColumnIndex] : null;
                                if (colObj_ == null)
                                    return;



                                var rowData_ = TOOL_GRID.GET_GRID_ROW_DATA(rowObj_);
                                if (rowData_ == null)
                                    return;




                            }
                            catch (Exception exc)
                            {
                                PLUGIN.LOG(exc);
                                //  PLUGIN.MSGUSERERROR(exc.Message);
                            }
                        };

                        /////////
                    }

                    #endregion



                }



                DataGridView createGrid(string pName)
                {

                    var grid = new DataGridView();
                    grid.Name = pName;
                    grid.Dock = DockStyle.Fill;
                    grid.MultiSelect = false;
                    grid.ReadOnly = true;
                    grid.AllowUserToAddRows = false;
                    grid.AllowUserToDeleteRows = false;
                    grid.AllowUserToResizeColumns = false;
                    grid.AllowUserToResizeRows = false;

                    grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    grid.AutoGenerateColumns = false;
                    grid.BackgroundColor = Color.White;
                    grid.RowHeadersVisible = false;
                    grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    //grid.Height = 110;
                    // grid.AutoSize = true;
                    grid.BorderStyle = BorderStyle.FixedSingle;


                    // grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                    grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font(grid.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
                    grid.ColumnHeadersHeight *= 2;

                    grid.DataError += (s, e) =>
                    {

                        RUNTIMELOG(e.Exception.ToString());

                    };
                    return grid;
                }

                void bindFilterDo()
                {
                    foreach (var o in CONTROL_DESTRUCT(this))
                    {

                        var ctrl = o as Control;

                        if (ctrl == null)
                            continue;

                        switch (ctrl.Name)
                        {



                            case "do_filter_docdep":
                            case "do_package_save":
                            case "do_package_print":
                            //
                            case "do_filter_date_today":
                            case "do_filter_date_day_dec":
                            case "do_filter_date_day_inc":
                                {
                                    var btn = ctrl as Button;
                                    btn.Click += filterDoBtnClick;

                                }
                                break;
                        }


                    }
                }
                DataRow doAskPackage()
                {
                    try
                    {
                        filter.readFilter(this);

                        var packNrPrefix = filter.generatePackNr(PLUGIN, filter.date).Split('-')[0];
                        var docDate = filter.date;

                        DataRow[] res_ = PLUGIN.REF("ref.gen.rec.project", "CODE", packNrPrefix);

                        if (res_ != null && res_.Length > 0)
                        {
                            return res_[0];
                        }
                        return null;

                        //                        var packNrPrefix = filter.generatePackNr(PLUGIN, filter.date).Split('-')[0];
                        //                        var docDate = filter.date;

                        //                        var packs = PLUGIN.SQL(@"
                        //SELECT 
                        //(SELECT CODE||'/'||NAME FROM LG_$FIRM$_PROJECT AS PRJ WHERE INV.PROJECTREF = PRJ.LOGICALREF) AS PROJECT,
                        //COUNT(*) AS COUNT ,
                        //SUM(COALESCE (
                        //(
                        //SELECT  
                        //SUM(L.AMOUNT* L.UINFO1/L.UINFO2 * I.AMOUNT) AS AMOUNT 
                        //from LG_$FIRM$_$PERIOD$_STLINE AS L INNER JOIN LG_$FIRM$_ITMUNITA AS I ON L.STOCKREF = I.ITEMREF AND I.LINENR=1
                        //WHERE L.INVOICEREF = INV.LOGICALREF AND L.LINETYPE IN (0,1)
                        //),0
                        //)) AS AMOUNT 
                        //FROM LG_$FIRM$_$PERIOD$_INVOICE AS INV WHERE DATE_ = @P1 AND TRCODE = 8 AND TRACKNR LIKE @P2 
                        //GROUP BY INV.TRACKNR,INV.PROJECTREF
                        //ORDER BY INV.TRACKNR DESC,INV.PROJECTREF DESC",
                        //                            new object[] { docDate, packNrPrefix + "%" });

                        //                        packs.Columns["PACKCODE"].ExtendedProperties["title"] = "T_CODE";
                        //                        packs.Columns["FRG_TITLE"].ExtendedProperties["title"] = "T_VEHICLE";
                        //                        packs.Columns["COUNT"].ExtendedProperties["title"] = "T_COUNT";
                        //                        packs.Columns["AMOUNT"].ExtendedProperties["title"] = "T_TOTAL";
                        //                        //
                        //                        packs.Columns["COUNT"].ExtendedProperties["format"] = "#,#0";
                        //                        packs.Columns["AMOUNT"].ExtendedProperties["format"] = "#,#0.##";

                        //                        var form = new FormRef(PLUGIN, "T_PAIDIN", packs);

                        //                        form.ShowDialog();

                        //                        if (form.selected.Count > 0)
                        //                        {
                        //                            return form.selected[0];
                        //                        }




                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(exc.Message);
                    }

                    return null;
                }

                void doPrintPackage()
                {
                    try
                    {

                        filter.readFilter(this);

                        var date = filter.date;
                        var dep = filter.docDep;


                        //REP_QUICK_MODE_B::1 REP_DEF_FILTER_B::1 DSGN_OUTPUT_B::1 REP_DSG_KEY_WORDS_S::print
                        string cmd_ = "rep loc::rep.cashlist.package " +
                            "filter::" +
                            "filter_DATE_," + FORMATSERIALIZE(date) + ";" +
                            "filter_DEP," + FORMATSERIALIZE(dep) + ";" +
                              " REP_QUICK_MODE_B::1 REP_DEF_FILTER_B::1";


                        PLUGIN.EXECMDTEXT(cmd_);

                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(exc.Message);
                    }
                }


                void removeRecord(DataRow pRec)
                {
                    try
                    {
                        if (pRec == null || pRec.RowState == DataRowState.Detached)
                            return;

                        if (PLUGIN.MSGUSERASK("T_MSG_COMMIT_DELETE"))
                        {
                            pRec.Table.Rows.Remove(pRec);
                        }

                        refreshCalc();
                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(exc.Message);
                    }
                }

                DataRow _moveCash(DataRow pRec, DataTable pDest)
                {
                    var data = pRec.ItemArray;
                    var src = pRec.Table;
                    var dest = pDest;

                    src.Rows.Remove(pRec);
                    var newRec = dest.Rows.Add(data);

                    return newRec;
                }

                void doCmd(string pCmd)
                {
                    try
                    {

                        switch (pCmd)
                        {

                            case "cmd/addtopack":
                                {
                                    var gridSrc = getGridOfDocsAll();
                                    var gridDest = getGridOfDocsPacked();

                                    //
                                    var rec = TOOL_GRID.GET_GRID_ROW_DATA(gridSrc);
                                    if (rec != null)
                                    {
                                        var newRec = _moveCash(rec, getTableOfDocsPacked());

                                        TOOL_GRID.SET_GRID_POSITION(gridDest, newRec, null);
                                        //
                                        refreshCalc();

                                    }
                                }
                                break;
                            case "cmd/delfrompack":
                                {
                                    var gridSrc = getGridOfDocsPacked();
                                    var gridDest = getGridOfDocsAll();

                                    //
                                    var rec = TOOL_GRID.GET_GRID_ROW_DATA(gridSrc);
                                    if (rec != null)
                                    {
                                        var newRec = _moveCash(rec, getTableOfDocsAll());
                                        //
                                        TOOL_GRID.SET_GRID_POSITION(gridDest, newRec, null);
                                        //
                                        refreshCalc();


                                    }
                                }
                                break;

                        }


                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(exc.Message);
                    }
                }
                void filterDoBtnClick(object sender, EventArgs e)
                {
                    try
                    {

                        var btn = sender as Button;

                        if (btn == null || btn.Name == null)
                            return;

                        filter.readFilter(this);


                        if (btn.Name == "do_filter_docdep")
                        {

                            var valCtrl = CONTROL_SEARCH(this, "filter_docdep") as NumericUpDown;
                            if (valCtrl != null)
                            {
                                DataRow[] res_ = PLUGIN.REF("ref.gen.rec.dep", "NR", filter.docDep);
                                if (res_ != null && res_.Length > 0)
                                {
                                    valCtrl.Value = CASTASSHORT(TAB_GETROW(res_[0], "NR"));
                                }
                            }

                        }

                        if (btn.Name == "do_package_print")
                        {
                            doPrintPackage();
                        }


                        if (btn.Name == "do_package_save")
                        {

                            var tab = getTableOfDocsPacked();
                            if (tab.Rows.Count == 0)
                                return;

                            if (!PLUGIN.MSGUSERASK("T_MSG_COMMIT_BEGIN - T_PAIDIN"))
                                return;

                            var list = new List<DataRow>();
                            foreach (DataRow r in tab.Rows)
                            {
                                list.Add(r);
                            }

                            //save 
                            PLUGIN.INVOKEINBATCH(() =>
                            {


                                foreach (DataRow row in list)
                                {
                                    var docRef = TAB_GETROW(row, "LOGICALREF");
                                    var isValidCash = ISTRUE(PLUGIN.SQLSCALAR(@"
                            SELECT 1 FROM LG_$FIRM$_$PERIOD$_KSLINES WHERE LOGICALREF = @P1 AND CANCELLED = 1
                            ", new object[] { docRef }));

                                    if (!isValidCash)
                                        continue;

                                    PLUGIN.SQL(@"
                            UPDATE LG_$FIRM$_$PERIOD$_KSLINES SET CANCELLED = 0, RECSTATUS=RECSTATUS+1 WHERE LOGICALREF = @P1 AND CANCELLED = 1;
                            ", new object[] { docRef });

                                    PLUGIN.SQL(@"
                            UPDATE LG_$FIRM$_$PERIOD$_CLFLINE SET CANCELLED = 0, RECSTATUS=RECSTATUS+1 WHERE SOURCEFREF = @P1 AND MODULENR = 10 AND TRCODE IN (1,2) AND CANCELLED = 1;
                            ", new object[] { docRef });
                                }

                            });
                            //clean
                            tab.Clear();

                            //filter.packageNr = "";//dont reset
                            filter.writeFilter(this);


                            refreshCalc();

                            return;
                        }


                        if (btn.Name.StartsWith("do_filter_date_"))
                        {

                            var ctrlDate = CONTROL_SEARCH(this, "filter_date") as DateTimePicker;

                            if (ctrlDate != null)
                            {
                                var now = DateTime.Now.Date;

                                switch (btn.Name)
                                {
                                    case "do_filter_date_today":
                                        ctrlDate.Value = now;
                                        break;
                                    case "do_filter_date_day_inc":
                                        {
                                            var tmp = ctrlDate.Value.Date;
                                            ctrlDate.Value = tmp.AddDays(+1);
                                        }
                                        break;
                                    case "do_filter_date_day_dec":
                                        {
                                            var tmp = ctrlDate.Value.Date;
                                            ctrlDate.Value = tmp.AddDays(-1);
                                        }
                                        break;

                                }
                            }

                            return;
                        }
                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(exc.Message);
                    }
                }

                void showDocsInfo()
                {

                    try
                    {
                        filter.readFilter(this);
                        PLUGIN.EXECMDTEXT("event name::" + event_CASHLIST_DOCS_INFO + " date::" + LEFT(FORMAT(filter.date), 10));
                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(exc.Message);
                    }
                }

                bool _isCashAdded(object pDocRef)
                {
                    foreach (var t in new DataTable[] { getTableOfDocsAll(), getTableOfDocsPacked() })
                    {
                        if (TAB_SEARCH(t, "LOGICALREF", pDocRef) != null)
                            return true;

                    }
                    return false;
                }

                DataRow _addCashToDocsAll(object pDocRef, bool pValidate = true)
                {

                    if (ISEMPTYLREF(pDocRef))
                        return null;

                    var gridDocsAll = getGridOfDocsAll();
                    var tabAllDocs = getTableOfDocsAll();



                    if (_isCashAdded(pDocRef))
                        return null;

                    //if WFSTATUS != 0 rec will be null
                    var cashRec = TAB_GETLASTROW(SELECT_CASH_IN(PLUGIN, pDocRef));
                    if (cashRec != null)
                    {

                        var newRec = tabAllDocs.NewRow();


                        foreach (DataColumn c in tabAllDocs.Columns)
                            TAB_SETROW(newRec, c.ColumnName, TAB_GETROW(cashRec, c.ColumnName));

                        tabAllDocs.Rows.Add(newRec);

                        return newRec;



                    }

                    return null;
                }
                void loadAllDocs()
                {

                    try
                    {
                        filter.readFilter(this);

                        var docDep = filter.docDep;



                        var arr = SELECT_CASH_IN(PLUGIN, filter.date, 1, filter.docDep);

                        DataRow lastRec = null;


                        foreach (var docLRef in arr)
                        {
                            var newRec = _addCashToDocsAll(docLRef, false);
                            if (newRec != null)
                                lastRec = newRec;
                        }

                        if (lastRec != null)
                            TOOL_GRID.SET_GRID_POSITION(getGridOfDocsAll(), lastRec, null);
                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(exc.Message);
                    }
                    finally
                    {
                        //!!!
                        refreshCalc();
                    }
                }


                void moveAllToPack()
                {
                    try
                    {

                        var dataSrc = getTableOfDocsAll();

                        if (dataSrc.Rows.Count == 0)
                            return;

                        try
                        {
                            var list = new List<DataRow>();
                            foreach (DataRow row in dataSrc.Rows)
                            {
                                list.Add(row);
                            }

                            foreach (DataRow row in list)
                            {
                                _moveCash(row, getTableOfDocsPacked());

                            }

                        }
                        finally
                        {
                            refreshCalc();
                        }
                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(exc.Message);
                    }
                }
                void addCashManual()
                {

                    try
                    {
                        filter.readFilter(this);

                        var date = filter.date.Date;

                        DataRow[] res_ = PLUGIN.REF("ref.fin.doc.cash multi::1 filter::filter_CANCELLED," + _PLUGIN.FORMATSERIALIZE(1) + ";filter_DATE_," + _PLUGIN.FORMATSERIALIZE(date) + "," + _PLUGIN.FORMATSERIALIZE(date) + ";filter_TRCODE," + _PLUGIN.FORMATSERIALIZE(11));
                        if (res_ != null && res_.Length > 0)
                        {
                            try
                            {


                                DataRow lastRec = null;

                                foreach (var rec in res_)
                                {
                                    var docLRef = rec["LOGICALREF"];

                                    var newRec = _addCashToDocsAll(docLRef, false);
                                    if (newRec != null)
                                        lastRec = newRec;

                                }

                                if (lastRec != null)
                                    TOOL_GRID.SET_GRID_POSITION(getGridOfDocsAll(), lastRec, null);
                            }
                            finally
                            {
                                refreshCalc();
                            }

                        }
                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(exc.Message);
                    }
                }

                DataGridView getGridOfDocsAll()
                {
                    var name = "grid_docs_all";
                    var grid = CONTROL_SEARCH(this, name) as DataGridView;
                    return grid;
                }
                DataTable getTableOfDocsAll()
                {
                    var grid = getGridOfDocsAll();
                    if (grid.DataSource == null)
                    {
                        var tab = SELECT_CASH_IN(PLUGIN, 0);
                        tab.TableName = grid.Name;
                        grid.DataSource = tab;
                    }
                    return grid.DataSource as DataTable;
                }
                DataGridView getGridOfDocsPacked()
                {
                    var name = "grid_docs_packed";
                    var grid = CONTROL_SEARCH(this, name) as DataGridView;
                    return grid;
                }
                DataTable getTableOfDocsPacked()
                {
                    var grid = getGridOfDocsPacked();
                    if (grid.DataSource == null)
                    {
                        var tab = SELECT_CASH_IN(PLUGIN, 0);
                        tab.TableName = grid.Name;
                        grid.DataSource = tab;
                    }
                    return grid.DataSource as DataTable;
                }
                void refreshCalc()
                {
                    try
                    {
                        filter.readFilter(this);


                        {
                            var tab = getTableOfDocsAll();
                            var amount = 0.0;
                            var count = 0;

                            foreach (DataRow r in tab.Rows)
                            {
                                count += 1;
                                amount += CASTASDOUBLE(TAB_GETROW(r, "AMOUNT"));
                            }

                            filter.val_docs_all_count = count;
                            filter.val_docs_all_amount = amount;
                        }

                        {
                            var tab = getTableOfDocsPacked();
                            var amount = 0.0;
                            var count = 0;

                            foreach (DataRow r in tab.Rows)
                            {
                                count += 1;
                                amount += CASTASDOUBLE(TAB_GETROW(r, "AMOUNT"));
                            }

                            filter.val_docs_pack_count = count;
                            filter.val_docs_pack_amount = amount;
                        }

                        filter.writeFilter(this);

                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(exc.Message);
                    }
                }
                void refreshData()
                {
                    try
                    {
                        filter.readFilter(this);

                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(exc.Message);
                    }
                }




                public class Filter
                {
                    // public double maxWeight = 0;
                    //public string vechile = "";
                    // public string packageNr = "";


                    public short docDep = 0;
                    public DateTime date;


                    public double val_docs_all_amount = 0;
                    public int val_docs_all_count = 0;

                    public double val_docs_pack_amount = 0;
                    public int val_docs_pack_count = 0;





                    public void writeFilter(Form pForm)
                    {
                        {
                            var ctrl = CONTROL_SEARCH(pForm, "val_docs_all_amount") as TextBox;
                            ctrl.Text = FORMAT(val_docs_all_amount, "#,#0.##");
                        }
                        {
                            var ctrl = CONTROL_SEARCH(pForm, "val_docs_all_count") as TextBox;
                            ctrl.Text = FORMAT(val_docs_all_count, "#,#0.##");
                        }
                        {
                            var ctrl = CONTROL_SEARCH(pForm, "val_docs_pack_amount") as TextBox;
                            ctrl.Text = FORMAT(val_docs_pack_amount, "#,#0.##");
                        }
                        {
                            var ctrl = CONTROL_SEARCH(pForm, "val_docs_pack_count") as TextBox;
                            ctrl.Text = FORMAT(val_docs_pack_count, "#,#0.##");
                        }


                    }
                    public void readFilter(Form pForm)
                    {
                        foreach (var o in CONTROL_DESTRUCT(pForm))
                        {

                            var ctrl = o as Control;

                            if (ctrl == null)
                                continue;

                            var asNum = ctrl as NumericUpDown;

                            if (asNum != null && asNum.Text == "")
                            {
                                //!!! num has problem on delete text, Text empty but Value hidden
                                asNum.Value = 0;
                            }


                            var asText = ctrl as TextBox;

                            if (asText != null && asText.Text == "")
                            {

                                asText.Text = "";
                            }

                            //if (asText != null)
                            //{

                            //    switch (ctrl.Name)
                            //    {

                            //        case "filter_clientmarkcode":
                            //            this.clientMarkCode = asText.Text.Trim();
                            //            break;

                            //    }

                            //}

                            var asDate = ctrl as DateTimePicker;

                            if (asDate != null)
                            {
                                switch (ctrl.Name)
                                {

                                    case "filter_date":
                                        this.date = asDate.Value.Date;
                                        break;

                                }

                            }



                            if (asNum != null)
                            {
                                switch (asNum.Name)
                                {
                                    case "filter_docdep":
                                        this.docDep = (short)asNum.Value;
                                        break;
                                }
                            }


                        }



                    }

                    public string generatePackNr(_PLUGIN pPLUGIN, DateTime pDate)
                    {
                        return "P" + FORMAT(pPLUGIN.GETSYSPRM_USER()).PadLeft(3, '0') + FORMAT(pDate, "yyMMdd-hhmmss");
                    }

                    public string generatePackNr(_PLUGIN pPLUGIN)
                    {
                        return generatePackNr(pPLUGIN, DateTime.Now);
                    }
                }


                public class Values
                {
                    public short indx = 0;
                    public string title = "";
                    public string docnr = "";
                    public DateTime date;

                    public double tot = 0.0;
                    public double totRep = 0.0;

                    public double totNoVat = 0.0;
                    public double totNoVatRep = 0.0;

                    public double balance = 0.0;
                    public double balanceRep = 0.0;

                    public short sign = 0;

                    public void write(DataRow rec)
                    {

                        TAB_SETROW(rec, "INDX", indx);
                        TAB_SETROW(rec, "TITLE", title);
                        TAB_SETROW(rec, "DATE_", date);
                        TAB_SETROW(rec, "SIGN", sign);

                        TAB_SETROW(rec, "TOT", tot);
                        TAB_SETROW(rec, "TOTREP", totRep);

                        TAB_SETROW(rec, "TOTNOVAT", totNoVat);
                        TAB_SETROW(rec, "TOTNOVATREP", totNoVatRep);

                        TAB_SETROW(rec, "BALANCE", balance);
                        TAB_SETROW(rec, "BALANCEREP", balanceRep);

                        TAB_SETROW(rec, "DOCNR", docnr);
                    }



                    public DataRow write(DataTable tab, int pos = -1)
                    {
                        var rec = tab.NewRow();

                        write(rec);

                        if (pos < 0)
                            tab.Rows.Add(rec);
                        else
                            tab.Rows.InsertAt(rec, pos);

                        return rec;

                    }

                }

                public class ValuesFinBalance
                {

                    public double balance = 0.0;
                    public double balanceRep = 0.0;


                    public void read(DataRow rec)
                    {

                        balance = CASTASDOUBLE(TAB_GETROW(rec, "TOT"));
                        balanceRep = CASTASDOUBLE(TAB_GETROW(rec, "TOTREP"));


                    }


                }

                public class ValuesClTran
                {

                    public object LOGICALREF;
                    public double AMOUNT;
                    public double REPORTNET;
                    public DateTime DATE_;
                    public short MODULENR;
                    public short TRCODE;
                    public object SOURCEFREF;
                    public short SIGN;

                    public ValuesClTran(DataRow rec)
                    {
                        LOGICALREF = (TAB_GETROW(rec, "LOGICALREF"));
                        AMOUNT = CASTASDOUBLE(TAB_GETROW(rec, "AMOUNT"));
                        REPORTNET = CASTASDOUBLE(TAB_GETROW(rec, "REPORTNET"));
                        DATE_ = CASTASDATE(TAB_GETROW(rec, "DATE_"));
                        MODULENR = CASTASSHORT(TAB_GETROW(rec, "MODULENR"));
                        TRCODE = CASTASSHORT(TAB_GETROW(rec, "TRCODE"));
                        SOURCEFREF = (TAB_GETROW(rec, "SOURCEFREF"));
                        SIGN = CASTASSHORT(TAB_GETROW(rec, "SIGN"));
                    }


                }

            }


            public class FormCashByInvoice : Form
            {
                _PLUGIN PLUGIN;

                Filter filter = new Filter();

                public FormCashByInvoice(_PLUGIN pPLUGIN, Filter pFilter)
                {
                    PLUGIN = pPLUGIN;
                    filter = pFilter ?? new Filter() { date = DateTime.Now.Date };

                    this.Text = string.Format("{0}", _LANG.L.CASH_LIST);

                    //filter.packageNr = filter.generatePackNr(PLUGIN);

                    init();

                    bindFilterDo();

                    refreshData();
                }


                public void init()
                {

                    this.Icon = CTRL_FORM_ICON();
                    this.Size = new Size(1000, 700);
                    this.AutoSize = true;
                    //form.BackColor = Color.White;
                    this.StartPosition = FormStartPosition.CenterScreen;

                    var panelMain = new Panel() { Dock = DockStyle.Fill };
                    var tabControl = new TabControl() { Dock = DockStyle.Fill, AutoSize = true };
                    var tabPageDocsAll = new TabPage() { Text = PLUGIN.LANG("T_DOC_FIN_CASH_11") };
                    var tabPageDocsPacked = new TabPage() { Text = PLUGIN.LANG("T_PAIDIN") };


                    tabControl.Controls.Add(tabPageDocsAll);
                    tabControl.Controls.Add(tabPageDocsPacked);



                    this.Controls.Add(panelMain);
                    panelMain.Controls.Add(tabControl);


                    this.WindowState = FormWindowState.Maximized;

                    var labelWidth = 170;

                    var btnClose = new Button() { Text = PLUGIN.LANG("T_CLOSE"), Image = RES_IMAGE("close_16x16"), ImageAlign = ContentAlignment.MiddleLeft, Width = labelWidth, Dock = DockStyle.Right };
                    btnClose.Click += (s, arg) => { this.Close(); };

                    var btnInfoDocs = new Button() { Text = PLUGIN.LANG("T_INFO - T_PACKAGE"), Image = RES_IMAGE("info_16x16"), ImageAlign = ContentAlignment.MiddleLeft, Width = labelWidth, Dock = DockStyle.Left };
                    btnInfoDocs.Click += (s, arg) => { this.showPackageInfo(); };

                    var btnInfoCashTran = new Button() { Text = PLUGIN.LANG("T_REF_FIN_DOC_CASH"), Image = RES_IMAGE("doc_16x16"), ImageAlign = ContentAlignment.MiddleLeft, Width = labelWidth, Dock = DockStyle.Left };
                    btnInfoCashTran.Click += (s, arg) => { this.openCashTran(); };



                    var btnAddDoc = new Button()
                    {
                        Text = PLUGIN.LANG("T_ADD"),
                        Image = RES_IMAGE("barcode_16x16"),
                        ImageAlign = ContentAlignment.MiddleLeft,
                        Width = labelWidth
                    };
                    btnAddDoc.Click += (s, arg) => { this.payByInvoice(); };



                    var panelFilterDateRange = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Bottom, AutoSize = true };

                    var panelPacketAction = new FlowLayoutPanel() { Dock = DockStyle.Bottom, AutoSize = true };

                    var panelBtn = new FlowLayoutPanel() { FlowDirection = FlowDirection.LeftToRight, Dock = DockStyle.Bottom, AutoSize = true };
                    var panelBtnPack = new Panel() { Dock = DockStyle.Bottom, Height = 25 };
                    var panelBtnGen = new Panel() { Dock = DockStyle.Bottom, Height = 25 };
                    var panelGenInfoDocsAll = new FlowLayoutPanel() { Dock = DockStyle.Bottom, Height = 25 };
                    var panelGenInfoDocsPacked = new FlowLayoutPanel() { Dock = DockStyle.Bottom, Height = 25 };
                    //







                    // panelMain.Controls.Add(panelPacketAction);
                    panelMain.Controls.Add(panelGenInfoDocsAll);
                    panelMain.Controls.Add(panelGenInfoDocsPacked);
                    panelMain.Controls.Add(panelBtnGen);

                    panelBtn.Controls.AddRange(new Control[] { 
                      
                    btnAddDoc,
                
                    });




                    panelBtnGen.Controls.Add(btnClose);



                    panelBtnGen.Controls.AddRange(new Control[] { 
                        btnInfoCashTran,
                        btnInfoDocs 
                    });

                    //



                    panelFilterDateRange.Controls.AddRange(
                        new Control[]{
                            new Label(){Text = PLUGIN.LANG("T_DATE"), Width = labelWidth/2, TextAlign = ContentAlignment.MiddleLeft,Dock = DockStyle.Left},
                            new DateTimePicker() { Value = filter.date , Name = "filter_date",Width=labelWidth },
                   

                            new Button(){Text=PLUGIN.LANG("T_TODAY"),Image = RES_IMAGE("run_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_filter_date_today",Width=labelWidth/2 }, 
                            new Button(){Text=PLUGIN.LANG("-1 T_DAY"),Image = RES_IMAGE("run_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_filter_date_day_dec",Width=labelWidth/2 }, 
                            new Button(){Text=PLUGIN.LANG("+1 T_DAY"),Image = RES_IMAGE("run_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_filter_date_day_inc",Width=labelWidth/2 }, 
                        
                            ////////////


                          

                            }
                    );

                    {


                        panelPacketAction.Controls.AddRange(
                          new Control[]{
                        
                          

                            }
                          );


                        panelBtnPack.Controls.AddRange(
                          new Control[]{
                          
                           new Button(){Text=PLUGIN.LANG("T_SAVE"),Image = RES_IMAGE("save_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_package_save",Width=labelWidth,Dock = DockStyle.Right  },
                           // new Button(){Text=PLUGIN.LANG("T_PRINT"),Image = RES_IMAGE("print_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_package_print",Width=labelWidth,Dock = DockStyle.Right  },
                          

                            }
                          );

                    }


                    {


                        panelGenInfoDocsAll.Controls.AddRange(new Control[]{

                              
                              new Label(){Text = PLUGIN.LANG("T_TOTAL (T_ALL)"), Width = labelWidth/2, TextAlign = ContentAlignment.MiddleRight,ForeColor = Color.Green },
                            new TextBox() {   Text = "0", Name = "val_docs_all_amount",Width=labelWidth/2,ReadOnly=true, TextAlign = HorizontalAlignment.Right },


                             new Label(){Text = PLUGIN.LANG("T_COUNT"), Width = labelWidth/2, TextAlign = ContentAlignment.MiddleRight },
                            new TextBox() {   Text = "0", Name = "val_docs_all_count",Width=labelWidth/2,ReadOnly=true, TextAlign = HorizontalAlignment.Right  },
                        

                          
                            
                            });

                    }


                    {



                        panelGenInfoDocsPacked.Controls.AddRange(new Control[]{
 

                              new Label(){Text = PLUGIN.LANG("T_TOTAL (T_PAIDIN)"), Width = labelWidth/2, TextAlign = ContentAlignment.MiddleRight,ForeColor = Color.Blue },
                            new TextBox() {   Text = "0", Name = "val_docs_pack_amount",Width=labelWidth/2,ReadOnly=true, TextAlign = HorizontalAlignment.Right  },


                             new Label(){Text = PLUGIN.LANG("T_COUNT"), Width = labelWidth/2, TextAlign = ContentAlignment.MiddleRight },
                            new TextBox() {   Text = "0", Name = "val_docs_pack_count",Width=labelWidth/2,ReadOnly=true, TextAlign = HorizontalAlignment.Right },
                        
                         
                         
                         

                            
                            });

                    }



                    #region GRIDS

                    var gridDocsAll = createGrid("grid_docs_all");
                    var gridDocsPacked = createGrid("grid_docs_packed");

                    tabPageDocsPacked.Controls.Add(gridDocsPacked);
                    // tabPageDocsPacked.Controls.Add(panelPacketAction);
                    tabPageDocsPacked.Controls.Add(panelBtnPack);



                    tabPageDocsAll.Controls.Add(gridDocsAll);

                    tabPageDocsAll.Controls.Add(panelFilterDateRange);

                    tabPageDocsAll.Controls.Add(panelBtn);


                    foreach (var g in new DataGridView[] { gridDocsAll, gridDocsPacked })
                        g.Columns.AddRange(
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_INDEX"), DataPropertyName = "LOGICALREF", Width = 60, Frozen = true },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_DATE"), DataPropertyName = "DATE_", Width = 80, Frozen = true },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_NR"), DataPropertyName = "FICHENO", Width = 80, Frozen = false },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_GROUP"), DataPropertyName = "TRANGRPNO", Width = 100, Frozen = false },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_INFO"), DataPropertyName = "LINEEXP", Width = 120, Frozen = false },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_INFO 2"), DataPropertyName = "CUSTTITLE", Width = 120, Frozen = false },

                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_AUTHCODE"), DataPropertyName = "CYPHCODE", Width = 50, Frozen = false },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_DEP"), DataPropertyName = "DEPARTMENT", Width = 40, Frozen = false },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_CONTRACTOR"), DataPropertyName = "CLCARD_TITLE", Width = 260, Frozen = false },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_BALANCE"), DataPropertyName = "BALANCE", Width = 100, Frozen = false },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_TOTAL"), DataPropertyName = "AMOUNT", Width = 100, Frozen = false, Tag = "cmd/editamount" }
                                           );

                    gridDocsAll.Columns.Add(new DataGridViewButtonColumn() { Width = 80, Text = PLUGIN.LANG("T_PAIDIN"), Tag = "cmd/addtopack", UseColumnTextForButtonValue = true });
                    gridDocsPacked.Columns.Add(new DataGridViewButtonColumn() { Width = 80, Text = PLUGIN.LANG("T_DELETE"), Tag = "cmd/delfrompack", UseColumnTextForButtonValue = true });

                    foreach (var x in new DataGridView[] { gridDocsAll, gridDocsPacked })
                        foreach (DataGridViewColumn colObj in x.Columns)
                        {
                            colObj.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            colObj.SortMode = DataGridViewColumnSortMode.Programmatic;

                            switch (colObj.DataPropertyName)
                            {
                                case "BALANCE":
                                case "AMOUNT":
                                    colObj.DefaultCellStyle.Format = "#,0.00;-#,0.00;''";
                                    colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                                    break;

                                case "DATE_":
                                    colObj.DefaultCellStyle.Format = "d";
                                    colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                                    break;
                            }

                            var colCmd = colObj.Tag as string;

                            if (colCmd != null)
                            {
                                switch (colCmd)
                                {
                                    case "cmd/addtopack":
                                        colObj.DefaultCellStyle.ForeColor = Color.Green;
                                        break;
                                    case "cmd/delfrompack":
                                        colObj.DefaultCellStyle.ForeColor = Color.Blue;
                                        break;
                                }

                            }
                        }

                    foreach (var x in new DataGridView[] { gridDocsAll, gridDocsPacked })
                    {

                        x.KeyDown += (sender, e) =>
                        {
                            try
                            {
                                DataGridView grid_ = sender as DataGridView;
                                if (grid_ == null)
                                    return;

                                if (e.Shift && !e.Control && !e.Alt && e.KeyCode == Keys.Delete)
                                {
                                    var data = TOOL_GRID.GET_GRID_ROW_DATA(grid_);
                                    removeRecord(data);

                                    e.SuppressKeyPress = true;
                                    return;
                                }
                            }
                            catch (Exception exc)
                            {
                                /// PLUGIN.MSGUSERERROR(exc.Message);
                                PLUGIN.LOG(exc);

                            }
                        };


                        x.CellClick += (sender, e) =>
                        {

                            try
                            {
                                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                                    return;

                                DataGridView grid_ = sender as DataGridView;
                                if (grid_ == null)
                                    return;

                                var colObj_ = e.ColumnIndex >= 0 ? grid_.Columns[e.ColumnIndex] : null;
                                if (colObj_ == null)
                                    return;


                                var colCmd = colObj_.Tag as string;

                                if (colCmd != null)
                                {
                                    doCmd(grid_, colCmd);
                                }
                                var tab = grid_.DataSource as DataTable;

                                if (tab == null)
                                    return;


                            }
                            catch (Exception exc)
                            {
                                /// PLUGIN.MSGUSERERROR(exc.Message);
                                PLUGIN.LOG(exc);

                            }


                        };


                        ///////////
                        x.CellMouseDoubleClick += (sender, e) =>
                        {
                            try
                            {
                                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                                    return;


                                DataGridView grid_ = sender as DataGridView;
                                if (grid_ == null)
                                    return;

                                var tab_ = grid_.DataSource as DataTable;

                                var colObj_ = e.ColumnIndex >= 0 ? grid_.Columns[e.ColumnIndex] : null;
                                if (colObj_ == null)
                                    return;

                                if (colObj_.Tag != null)
                                {
                                    //cmd/button column
                                    return;
                                }

                                DataRow ROW = TOOL_GRID.GET_GRID_ROW_DATA(grid_);
                                if (ROW == null)
                                    return;

                                try
                                {
                                    var lref = FORMAT(TAB_GETROW(ROW, "LOGICALREF"));
                                    var cmdEdit = "adp.fin.doc.cash.11 cmd::edit lref::" + lref;
                                    var cmdView = "adp.fin.doc.cash.11 cmd::view lref::" + lref;

                                    foreach (var cmd in new string[] { /*cmdEdit,*/ cmdView })
                                        if (PLUGIN.EXEADPCMDALLOWED(cmd, null))
                                            PLUGIN.EXECMDTEXT(cmd);

                                }
                                catch (Exception exc)
                                {
                                    PLUGIN.LOG(exc);
                                }

                            }
                            catch (Exception exc)
                            {
                                // PLUGIN.MSGUSERERROR(exc.Message);
                                PLUGIN.LOG(exc);
                            }

                        };


                        x.CellFormatting += (sender, e) =>
                        {
                            try
                            {
                                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                                    return;

                                DataGridView grid_ = sender as DataGridView;
                                if (grid_ == null)
                                    return;

                                var rowObj_ = TOOL_GRID.GET_GRID_ROW(grid_, e.RowIndex);
                                if (rowObj_ == null)
                                    return;

                                var colObj_ = e.ColumnIndex >= 0 ? grid_.Columns[e.ColumnIndex] : null;
                                if (colObj_ == null)
                                    return;


                                var rowData_ = TOOL_GRID.GET_GRID_ROW_DATA(rowObj_);
                                if (rowData_ == null)
                                    return;


                            }
                            catch (Exception exc)
                            {
                                PLUGIN.LOG(exc);
                                ///  PLUGIN.MSGUSERERROR(exc.Message);
                            }
                        };



                        x.CellPainting += (sender, e) =>
                        {

                            try
                            {


                                if (e.ColumnIndex < 0 || e.RowIndex < 0)
                                    return;

                                DataGridView grid_ = sender as DataGridView;
                                if (grid_ == null)
                                    return;

                                var bgColor = grid_.DefaultCellStyle.BackColor;

                                var rowObj_ = TOOL_GRID.GET_GRID_ROW(grid_, e.RowIndex);
                                if (rowObj_ == null)
                                    return;

                                var colObj_ = e.ColumnIndex >= 0 ? grid_.Columns[e.ColumnIndex] : null;
                                if (colObj_ == null)
                                    return;



                                var rowData_ = TOOL_GRID.GET_GRID_ROW_DATA(rowObj_);
                                if (rowData_ == null)
                                    return;

                                if (colObj_.DataPropertyName == "BALANCE")
                                {
                                    e.CellStyle.ForeColor = Color.Green;
                                }


                            }
                            catch (Exception exc)
                            {
                                PLUGIN.LOG(exc);
                                //  PLUGIN.MSGUSERERROR(exc.Message);
                            }
                        };

                        /////////
                    }

                    #endregion



                }



                DataGridView createGrid(string pName)
                {

                    var grid = new DataGridView();
                    grid.Name = pName;
                    grid.Dock = DockStyle.Fill;
                    grid.MultiSelect = false;
                    grid.ReadOnly = true;
                    grid.AllowUserToAddRows = false;
                    grid.AllowUserToDeleteRows = false;
                    grid.AllowUserToResizeColumns = false;
                    grid.AllowUserToResizeRows = false;

                    grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    grid.AutoGenerateColumns = false;
                    grid.BackgroundColor = Color.White;
                    grid.RowHeadersVisible = false;
                    grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    //grid.Height = 110;
                    // grid.AutoSize = true;
                    grid.BorderStyle = BorderStyle.FixedSingle;


                    // grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                    grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font(grid.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
                    grid.ColumnHeadersHeight *= 2;

                    grid.DataError += (s, e) =>
                    {

                        RUNTIMELOG(e.Exception.ToString());

                    };
                    return grid;
                }

                void bindFilterDo()
                {
                    foreach (var o in CONTROL_DESTRUCT(this))
                    {

                        var ctrl = o as Control;

                        if (ctrl == null)
                            continue;

                        switch (ctrl.Name)
                        {



                            case "do_filter_docdep":
                            case "do_package_save":
                            // case "do_package_print":
                            //
                            case "do_filter_date_today":
                            case "do_filter_date_day_dec":
                            case "do_filter_date_day_inc":
                                {
                                    var btn = ctrl as Button;
                                    btn.Click += filterDoBtnClick;

                                }
                                break;
                        }


                    }
                }
                DataRow doAskPackage()
                {
                    try
                    {
                        filter.readFilter(this);

                        var packNrPrefix = filter.generatePackNr(PLUGIN, filter.date).Split('-')[0];
                        var docDate = filter.date;

                        DataRow[] res_ = PLUGIN.REF("ref.gen.rec.project", "CODE", packNrPrefix);

                        if (res_ != null && res_.Length > 0)
                        {
                            return res_[0];
                        }
                        return null;

                        //                        var packNrPrefix = filter.generatePackNr(PLUGIN, filter.date).Split('-')[0];
                        //                        var docDate = filter.date;

                        //                        var packs = PLUGIN.SQL(@"
                        //SELECT 
                        //(SELECT CODE||'/'||NAME FROM LG_$FIRM$_PROJECT AS PRJ WHERE INV.PROJECTREF = PRJ.LOGICALREF) AS PROJECT,
                        //COUNT(*) AS COUNT ,
                        //SUM(COALESCE (
                        //(
                        //SELECT  
                        //SUM(L.AMOUNT* L.UINFO1/L.UINFO2 * I.AMOUNT) AS AMOUNT 
                        //from LG_$FIRM$_$PERIOD$_STLINE AS L INNER JOIN LG_$FIRM$_ITMUNITA AS I ON L.STOCKREF = I.ITEMREF AND I.LINENR=1
                        //WHERE L.INVOICEREF = INV.LOGICALREF AND L.LINETYPE IN (0,1)
                        //),0
                        //)) AS AMOUNT 
                        //FROM LG_$FIRM$_$PERIOD$_INVOICE AS INV WHERE DATE_ = @P1 AND TRCODE = 8 AND TRACKNR LIKE @P2 
                        //GROUP BY INV.TRACKNR,INV.PROJECTREF
                        //ORDER BY INV.TRACKNR DESC,INV.PROJECTREF DESC",
                        //                            new object[] { docDate, packNrPrefix + "%" });

                        //                        packs.Columns["PACKCODE"].ExtendedProperties["title"] = "T_CODE";
                        //                        packs.Columns["FRG_TITLE"].ExtendedProperties["title"] = "T_VEHICLE";
                        //                        packs.Columns["COUNT"].ExtendedProperties["title"] = "T_COUNT";
                        //                        packs.Columns["AMOUNT"].ExtendedProperties["title"] = "T_TOTAL";
                        //                        //
                        //                        packs.Columns["COUNT"].ExtendedProperties["format"] = "#,#0";
                        //                        packs.Columns["AMOUNT"].ExtendedProperties["format"] = "#,#0.##";

                        //                        var form = new FormRef(PLUGIN, "T_PAIDIN", packs);

                        //                        form.ShowDialog();

                        //                        if (form.selected.Count > 0)
                        //                        {
                        //                            return form.selected[0];
                        //                        }




                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(exc.Message);
                    }

                    return null;
                }

                //void doPrintPackage()
                //{
                //    try
                //    {

                //        filter.readFilter(this);

                //        var date = filter.date;
                //        var dep = filter.docDep;


                //        //REP_QUICK_MODE_B::1 REP_DEF_FILTER_B::1 DSGN_OUTPUT_B::1 REP_DSG_KEY_WORDS_S::print
                //        string cmd_ = "rep loc::rep.cashlist.package " +
                //            "filter::" +
                //            "filter_DATE_," + FORMATSERIALIZE(date) + ";" +
                //            "filter_DEP," + FORMATSERIALIZE(dep) + ";" +
                //              " REP_QUICK_MODE_B::1 REP_DEF_FILTER_B::1";


                //        PLUGIN.EXECMDTEXT(cmd_);

                //    }
                //    catch (Exception exc)
                //    {
                //        PLUGIN.LOG(exc);
                //        PLUGIN.MSGUSERERROR(exc.Message);
                //    }
                //}


                void removeRecord(DataRow pRec)
                {
                    try
                    {
                        if (pRec == null || pRec.RowState == DataRowState.Detached)
                            return;

                        if (PLUGIN.MSGUSERASK("T_MSG_COMMIT_DELETE"))
                        {
                            pRec.Table.Rows.Remove(pRec);
                        }

                        refreshCalc();
                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(exc.Message);
                    }
                }

                DataRow _moveCash(DataRow pRec, DataTable pDest)
                {
                    var data = pRec.ItemArray;
                    var src = pRec.Table;
                    var dest = pDest;

                    src.Rows.Remove(pRec);
                    var newRec = dest.Rows.Add(data);

                    return newRec;
                }

                void doCmd(DataGridView pGrid, string pCmd)
                {
                    try
                    {

                        switch (pCmd)
                        {
                            case "cmd/editamount":
                                {
                                    var rec = TOOL_GRID.GET_GRID_ROW_DATA(pGrid);
                                    if (rec != null)
                                    {
                                        var amt = CASTASDOUBLE(TAB_GETROW(rec, "AMOUNT"));
                                        amt = MY_TOOL.MY_ASK_NUM(PLUGIN, "T_TOTAL", amt, 2);
                                        if (amt > 0.01)
                                            TAB_SETROW(rec, "AMOUNT", amt);

                                        refreshCalc();
                                    }
                                }
                                break;
                            case "cmd/addtopack":
                                {
                                    var gridSrc = getGridOfDocsAll();
                                    var gridDest = getGridOfDocsPacked();

                                    //
                                    var rec = TOOL_GRID.GET_GRID_ROW_DATA(gridSrc);
                                    if (rec != null)
                                    {
                                        var newRec = _moveCash(rec, getTableOfDocsPacked());

                                        TOOL_GRID.SET_GRID_POSITION(gridDest, newRec, null);
                                        //
                                        refreshCalc();

                                    }
                                }
                                break;
                            case "cmd/delfrompack":
                                {
                                    var gridSrc = getGridOfDocsPacked();
                                    var gridDest = getGridOfDocsAll();

                                    //
                                    var rec = TOOL_GRID.GET_GRID_ROW_DATA(gridSrc);
                                    if (rec != null)
                                    {
                                        var newRec = _moveCash(rec, getTableOfDocsAll());
                                        //
                                        TOOL_GRID.SET_GRID_POSITION(gridDest, newRec, null);
                                        //
                                        refreshCalc();


                                    }
                                }
                                break;

                        }


                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(exc.Message);
                    }
                }
                void filterDoBtnClick(object sender, EventArgs e)
                {
                    try
                    {

                        var btn = sender as Button;

                        if (btn == null || btn.Name == null)
                            return;

                        filter.readFilter(this);


                        if (btn.Name == "do_filter_docdep")
                        {

                            var valCtrl = CONTROL_SEARCH(this, "filter_docdep") as NumericUpDown;
                            if (valCtrl != null)
                            {
                                DataRow[] res_ = PLUGIN.REF("ref.gen.rec.dep", "NR", filter.docDep);
                                if (res_ != null && res_.Length > 0)
                                {
                                    valCtrl.Value = CASTASSHORT(TAB_GETROW(res_[0], "NR"));
                                }
                            }

                        }

                        //if (btn.Name == "do_package_print")
                        //{
                        //    doPrintPackage();
                        //}


                        if (btn.Name == "do_package_save")
                        {

                            var tab = getTableOfDocsPacked();
                            if (tab.Rows.Count == 0)
                                return;

                            if (!PLUGIN.MSGUSERASK("T_MSG_COMMIT_BEGIN - T_PAIDIN"))
                                return;

                            var list = new List<DataRow>();
                            foreach (DataRow r in tab.Rows)
                            {
                                list.Add(r);
                            }

                            //save 
                            PLUGIN.INVOKEINBATCH(() =>
                            {
                                var index = 0;
                                foreach (DataRow row in list)
                                {
                                    var invoiceRef = TAB_GETROW(row, "LOGICALREF");

                                    var date = CASTASDATE(TAB_GETROW(row, "DATE_")).Date;
                                    var amount = CASTASDOUBLE(TAB_GETROW(row, "AMOUNT"));
                                    if (amount < 0.001)
                                        continue;

                                    var invRec = TAB_GETLASTROW(PLUGIN.SQL("SELECT * FROM LG_$FIRM$_$PERIOD$_INVOICE WHERE LOGICALREF = @P1", new object[] { invoiceRef }));
                                    if (invRec == null)
                                        continue;

                                    var docCode = CASTASSTRING(TAB_GETROW(invRec, "FICHENO"));

                                    var dep = CASTASSHORT(TAB_GETROW(invRec, "DEPARTMENT"));
                                    var clLRef = TAB_GETROW(invRec, "CLIENTREF");
                                    var prjLRef = TAB_GETROW(invRec, "PROJECTREF");

                                    var trackNr = CASTASSTRING(TAB_GETROW(invRec, "DOCTRACKINGNR"));
                                    var cyphcode = CASTASSTRING(TAB_GETROW(invRec, "CYPHCODE"));
                                    var sp1 = CASTASSTRING(TAB_GETROW(invRec, "SPECODE"));
                                    var sp2 = CASTASSTRING(TAB_GETROW(invRec, "SPECODE2"));
                                    var sp3 = CASTASSTRING(TAB_GETROW(invRec, "SPECODE3"));

                                    var info1 = CASTASSTRING(TAB_GETROW(invRec, "GENEXP1"));
                                    var info2 = CASTASSTRING(TAB_GETROW(invRec, "GENEXP2"));


                                    PLUGIN.EXEADPCMD("adp.fin.doc.cash.11 cmd::add", new DoWorkEventHandler((sender2, args2) =>
                                    {

                                        args2.Result = false;

                                        var ds = args2.Argument as DataSet;

                                        var tran = TAB_GETLASTROW(TAB_GETTAB(ds, "KSLINES"));
                                        var now = DateTime.Now;
                                        TAB_SETROW(tran, "FICHENO", docCode);
                                        TAB_SETROW(tran, "DUMMY_____DATETIME", date.Date.AddHours(now.Hour).AddMinutes(now.Minute));
                                        TAB_SETROW(tran, "LINEEXP", info1);
                                        TAB_SETROW(tran, "CUSTTITLE", info2);
                                        TAB_SETROW(tran, "SPECODE", sp1);
                                        TAB_SETROW(tran, "SPECODE2", sp2);
                                        TAB_SETROW(tran, "SPECODE3", sp3);
                                        TAB_SETROW(tran, "TRANGRPNO", trackNr);
                                        TAB_SETROW(tran, "CYPHCODE", cyphcode);
                                        TAB_SETROW(tran, "DEPARTMENT", dep);
                                        TAB_SETROW(tran, "CLIENTREF", clLRef);
                                        TAB_SETROW(tran, "PROJECTREF", prjLRef);
                                        TAB_SETROW(tran, "AMOUNT", amount);

                                       // ++index;
                                        //TAB_SETROW(tran, "TRANGRPLINENO", ++index);
                                        

                                        args2.Result = true;

                                    }), true);

                                }

                            });
                            //clean
                            tab.Clear();

                            //filter.packageNr = "";//dont reset
                            filter.writeFilter(this);


                            refreshCalc();

                            return;
                        }


                        if (btn.Name.StartsWith("do_filter_date_"))
                        {

                            var ctrlDate = CONTROL_SEARCH(this, "filter_date") as DateTimePicker;

                            if (ctrlDate != null)
                            {
                                var now = DateTime.Now.Date;

                                switch (btn.Name)
                                {
                                    case "do_filter_date_today":
                                        ctrlDate.Value = now;
                                        break;
                                    case "do_filter_date_day_inc":
                                        {
                                            var tmp = ctrlDate.Value.Date;
                                            ctrlDate.Value = tmp.AddDays(+1);
                                        }
                                        break;
                                    case "do_filter_date_day_dec":
                                        {
                                            var tmp = ctrlDate.Value.Date;
                                            ctrlDate.Value = tmp.AddDays(-1);
                                        }
                                        break;

                                }
                            }

                            return;
                        }
                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(GETEXCEPTIONFORUSERMSG(exc) ?? exc.Message);
                    }
                }
                void openCashTran()
                {
                    try
                    {
                        filter.readFilter(this);

                        var cmd = "ref.fin.doc.cash modal::0 filter::" +
                            "filter_DATE_," + FORMATSERIALIZE(filter.date) + "," + FORMATSERIALIZE(filter.date) + ";" +
                            "filter_TRCODE," + FORMATSERIALIZE((short)11) + ";" +
                            "filter_CANCELLED," + FORMATSERIALIZE((short)0) + ";";


                        PLUGIN.REFNORES(cmd);

                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(exc.Message);
                    }
                }


                void showPackageInfo()
                {

                    try
                    {
                        filter.readFilter(this);
                        PLUGIN.EXECMDTEXT("event name::" + event_CASHLIST_DOCS_PACKINFO + " date::" + LEFT(FORMAT(filter.date), 10));
                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(exc.Message);
                    }
                }

                bool _isCashAdded(object pDocRef)
                {
                    foreach (var t in new DataTable[] { getTableOfDocsAll(), getTableOfDocsPacked() })
                    {
                        if (TAB_SEARCH(t, "LOGICALREF", pDocRef) != null)
                            return true;

                    }
                    return false;
                }

                void CHECK_CASH_11_DOC_EXISTS(_PLUGIN pPLUGIN, string pDocCode)
                {

                    //check dublicate
                    var cashLRef = PLUGIN.SQLSCALAR("SELECT LOGICALREF FROM LG_$FIRM$_$PERIOD$_KSLINES WHERE FICHENO = @P1 AND TRCODE = 11", new object[] { pDocCode });
                    if (!ISEMPTYLREF(cashLRef))
                    {
                        EXCEPTIONFORUSER("T_MSG_ERROR_DUPLICATE_RECORD - T_DOCNO: " + pDocCode);

                    }

                }

                DataRow _addCashToDocsAll(object pInvoiceRef, bool pValidate = true)
                {

                    if (ISEMPTYLREF(pInvoiceRef))
                        return null;

                    var gridDocsAll = getGridOfDocsAll();
                    var tabAllDocs = getTableOfDocsAll();



                    if (_isCashAdded(pInvoiceRef))
                        return null;


                    var invRec = TAB_GETLASTROW(PLUGIN.SQL("SELECT * FROM LG_$FIRM$_$PERIOD$_INVOICE WHERE LOGICALREF = @P1", new object[] { pInvoiceRef }));
                    if (invRec == null)
                        return null;

                    var docCode = CASTASSTRING(TAB_GETROW(invRec, "FICHENO"));

                    //exception
                    CHECK_CASH_11_DOC_EXISTS(PLUGIN, docCode);

                    var dep = CASTASSHORT(TAB_GETROW(invRec, "DEPARTMENT"));
                    var clLRef = TAB_GETROW(invRec, "CLIENTREF");
                    var pjLRef = TAB_GETROW(invRec, "PROJECTREF");
                    var amount = CASTASDOUBLE(TAB_GETROW(invRec, "NETTOTAL"));

                    var trackNr = CASTASSTRING(TAB_GETROW(invRec, "DOCTRACKINGNR"));
                    var cyphcode = CASTASSTRING(TAB_GETROW(invRec, "CYPHCODE"));
                    var sp1 = CASTASSTRING(TAB_GETROW(invRec, "SPECODE"));
                    var sp2 = CASTASSTRING(TAB_GETROW(invRec, "SPECODE2"));
                    var sp3 = CASTASSTRING(TAB_GETROW(invRec, "SPECODE3"));

                    var info1 = CASTASSTRING(TAB_GETROW(invRec, "GENEXP1"));
                    var info2 = CASTASSTRING(TAB_GETROW(invRec, "GENEXP2"));

                    var balance = MY_CLIENT_BALANCE(PLUGIN, clLRef);
                    var clTitle = CASTASSTRING(
                        PLUGIN.SQLSCALAR(
                        "SELECT COALESCE(CONCAT(CL.CODE,'/',CL.DEFINITION_,'/',CL.SPECODE),'') FROM LG_$FIRM$_CLCARD AS CL WHERE CL.LOGICALREF = @P1 ",
                        new object[] { clLRef }));

                    var newRec = tabAllDocs.NewRow();
                    TAB_FILLNULL(newRec);

                    TAB_SETROW(newRec, "LOGICALREF", pInvoiceRef);
                    TAB_SETROW(newRec, "FICHENO", docCode);
                    TAB_SETROW(newRec, "DATE_", filter.date);
                    TAB_SETROW(newRec, "LINEEXP", info1);
                    TAB_SETROW(newRec, "CUSTTITLE", info2);
                    TAB_SETROW(newRec, "TRANGRPNO", trackNr);
                    TAB_SETROW(newRec, "CYPHCODE", cyphcode);
                    TAB_SETROW(newRec, "DEPARTMENT", dep);
                    TAB_SETROW(newRec, "CLCARD_TITLE", clTitle);
                    TAB_SETROW(newRec, "AMOUNT", amount);
                    TAB_SETROW(newRec, "BALANCE", balance);

                    tabAllDocs.Rows.Add(newRec);

                    return newRec;

                }
                //void loadAllDocs()
                //{

                //    try
                //    {
                //        filter.readFilter(this);

                //        var docDep = filter.docDep;



                //        var arr = SELECT_CASH_IN(PLUGIN, filter.date, 1, filter.docDep);

                //        DataRow lastRec = null;


                //        foreach (var docLRef in arr)
                //        {
                //            var newRec = _addCashToDocsAll(docLRef, false);
                //            if (newRec != null)
                //                lastRec = newRec;
                //        }

                //        if (lastRec != null)
                //            TOOL_GRID.SET_GRID_POSITION(getGridOfDocsAll(), lastRec, null);
                //    }
                //    catch (Exception exc)
                //    {
                //        PLUGIN.LOG(exc);
                //        PLUGIN.MSGUSERERROR(exc.Message);
                //    }
                //    finally
                //    {
                //        //!!!
                //        refreshCalc();
                //    }
                //}


                void moveAllToPack()
                {
                    try
                    {

                        var dataSrc = getTableOfDocsAll();

                        if (dataSrc.Rows.Count == 0)
                            return;

                        try
                        {
                            var list = new List<DataRow>();
                            foreach (DataRow row in dataSrc.Rows)
                            {
                                list.Add(row);
                            }

                            foreach (DataRow row in list)
                            {
                                _moveCash(row, getTableOfDocsPacked());

                            }

                        }
                        finally
                        {
                            refreshCalc();
                        }
                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(exc.Message);
                    }
                }
                void payByInvoice()
                {

                    try
                    {
                        filter.readFilter(this);

                        var date = filter.date.Date;


                        while (true)
                        {
                            var invoiceRef = MY_ASK_INVOICE_LREF();

                            if (ISEMPTYLREF(invoiceRef))
                                return;


                            try
                            {



                                DataRow lastRec = null;

                                var newRec = _addCashToDocsAll(invoiceRef, false);
                                if (newRec != null)
                                    lastRec = newRec;


                                if (lastRec != null)
                                    TOOL_GRID.SET_GRID_POSITION(getGridOfDocsAll(), lastRec, null);
                            }
                            finally
                            {
                                refreshCalc();
                            }



                        }
                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(exc.Message);
                    }
                }

                static double MY_CLIENT_BALANCE(_PLUGIN pPLUGIN, object pClRef)
                {
                    return CASTASDOUBLE(pPLUGIN.SQLSCALAR("SELECT (DEBIT-CREDIT) FROM LG_$FIRM$_$PERIOD$_GNTOTCL WHERE CARDREF = @P1 AND TOTTYP = 1", new object[] { pClRef }));
                }
                object MY_ASK_INVOICE_LREF()
                {

                    var cmdAppend = "";
                    while (true)
                    {
                        var barcode = MY_TOOL.MY_ASK_STRING(PLUGIN, "T_INVOICE - T_BARCODE", "", cmdAppend);
                        if (ISEMPTY(barcode))
                            break;

                        //make upper
                        barcode = barcode.ToUpperInvariant().Trim();

                        PLUGIN.LOG("Search invoice by code:" + barcode);
 
                        var docLRef = PLUGIN.SQLSCALAR("SELECT LOGICALREF FROM LG_$FIRM$_$PERIOD$_INVOICE WHERE FICHENO = @P1 AND TRCODE = 8", new object[] { barcode });

                        if (ISEMPTYLREF(docLRef))
                        {
                            MY_TOOL.BEEPERR();
                            cmdAppend = "warning::1 ";
                        }
                        else
                        {
                            return docLRef;

                            //SQL("UPDATE LG_$FIRM$_$PERIOD$_INVOICE SET WFSTATUS = 5, RECSTATUS=RECSTATUS+1 WHERE LOGICALREF = @P1", new object[] { docLRef });
                            //cmdAppend = "success::1 ";
                        }
                    }

                    return null;
                }
                DataGridView getGridOfDocsAll()
                {
                    var name = "grid_docs_all";
                    var grid = CONTROL_SEARCH(this, name) as DataGridView;
                    return grid;
                }
                DataTable getTableOfDocsAll()
                {
                    var grid = getGridOfDocsAll();
                    if (grid.DataSource == null)
                    {
                        var tab = SELECT_CASH_IN(PLUGIN, 0);
                        tab.TableName = grid.Name;
                        grid.DataSource = tab;
                    }
                    return grid.DataSource as DataTable;
                }
                DataGridView getGridOfDocsPacked()
                {
                    var name = "grid_docs_packed";
                    var grid = CONTROL_SEARCH(this, name) as DataGridView;
                    return grid;
                }
                DataTable getTableOfDocsPacked()
                {
                    var grid = getGridOfDocsPacked();
                    if (grid.DataSource == null)
                    {
                        var tab = SELECT_CASH_IN(PLUGIN, 0);
                        tab.TableName = grid.Name;
                        grid.DataSource = tab;
                    }
                    return grid.DataSource as DataTable;
                }
                void refreshCalc()
                {
                    try
                    {
                        filter.readFilter(this);


                        {
                            var tab = getTableOfDocsAll();
                            var amount = 0.0;
                            var count = 0;

                            foreach (DataRow r in tab.Rows)
                            {
                                count += 1;
                                amount += CASTASDOUBLE(TAB_GETROW(r, "AMOUNT"));
                            }

                            filter.val_docs_all_count = count;
                            filter.val_docs_all_amount = amount;
                        }

                        {
                            var tab = getTableOfDocsPacked();
                            var amount = 0.0;
                            var count = 0;

                            foreach (DataRow r in tab.Rows)
                            {
                                count += 1;
                                amount += CASTASDOUBLE(TAB_GETROW(r, "AMOUNT"));
                            }

                            filter.val_docs_pack_count = count;
                            filter.val_docs_pack_amount = amount;
                        }

                        filter.writeFilter(this);

                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(exc.Message);
                    }
                }
                void refreshData()
                {
                    try
                    {
                        filter.readFilter(this);

                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(exc.Message);
                    }
                }




                public class Filter
                {
                    // public double maxWeight = 0;
                    //public string vechile = "";
                    // public string packageNr = "";


                    public short docDep = 0;
                    public DateTime date;


                    public double val_docs_all_amount = 0;
                    public int val_docs_all_count = 0;

                    public double val_docs_pack_amount = 0;
                    public int val_docs_pack_count = 0;





                    public void writeFilter(Form pForm)
                    {
                        {
                            var ctrl = CONTROL_SEARCH(pForm, "val_docs_all_amount") as TextBox;
                            ctrl.Text = FORMAT(val_docs_all_amount, "#,#0.##");
                        }
                        {
                            var ctrl = CONTROL_SEARCH(pForm, "val_docs_all_count") as TextBox;
                            ctrl.Text = FORMAT(val_docs_all_count, "#,#0.##");
                        }
                        {
                            var ctrl = CONTROL_SEARCH(pForm, "val_docs_pack_amount") as TextBox;
                            ctrl.Text = FORMAT(val_docs_pack_amount, "#,#0.##");
                        }
                        {
                            var ctrl = CONTROL_SEARCH(pForm, "val_docs_pack_count") as TextBox;
                            ctrl.Text = FORMAT(val_docs_pack_count, "#,#0.##");
                        }


                    }
                    public void readFilter(Form pForm)
                    {
                        foreach (var o in CONTROL_DESTRUCT(pForm))
                        {

                            var ctrl = o as Control;

                            if (ctrl == null)
                                continue;

                            var asNum = ctrl as NumericUpDown;

                            if (asNum != null && asNum.Text == "")
                            {
                                //!!! num has problem on delete text, Text empty but Value hidden
                                asNum.Value = 0;
                            }


                            var asText = ctrl as TextBox;

                            if (asText != null && asText.Text == "")
                            {

                                asText.Text = "";
                            }

                            //if (asText != null)
                            //{

                            //    switch (ctrl.Name)
                            //    {

                            //        case "filter_clientmarkcode":
                            //            this.clientMarkCode = asText.Text.Trim();
                            //            break;

                            //    }

                            //}

                            var asDate = ctrl as DateTimePicker;

                            if (asDate != null)
                            {
                                switch (ctrl.Name)
                                {

                                    case "filter_date":
                                        this.date = asDate.Value.Date;
                                        break;

                                }

                            }



                            if (asNum != null)
                            {
                                switch (asNum.Name)
                                {
                                    case "filter_docdep":
                                        this.docDep = (short)asNum.Value;
                                        break;
                                }
                            }


                        }



                    }

                    public string generatePackNr(_PLUGIN pPLUGIN, DateTime pDate)
                    {
                        return "P" + FORMAT(pPLUGIN.GETSYSPRM_USER()).PadLeft(3, '0') + FORMAT(pDate, "yyMMdd-hhmmss");
                    }

                    public string generatePackNr(_PLUGIN pPLUGIN)
                    {
                        return generatePackNr(pPLUGIN, DateTime.Now);
                    }
                }


                public class Values
                {
                    public short indx = 0;
                    public string title = "";
                    public string docnr = "";
                    public DateTime date;

                    public double tot = 0.0;
                    public double totRep = 0.0;

                    public double totNoVat = 0.0;
                    public double totNoVatRep = 0.0;

                    public double balance = 0.0;
                    public double balanceRep = 0.0;

                    public short sign = 0;

                    public void write(DataRow rec)
                    {

                        TAB_SETROW(rec, "INDX", indx);
                        TAB_SETROW(rec, "TITLE", title);
                        TAB_SETROW(rec, "DATE_", date);
                        TAB_SETROW(rec, "SIGN", sign);

                        TAB_SETROW(rec, "TOT", tot);
                        TAB_SETROW(rec, "TOTREP", totRep);

                        TAB_SETROW(rec, "TOTNOVAT", totNoVat);
                        TAB_SETROW(rec, "TOTNOVATREP", totNoVatRep);

                        TAB_SETROW(rec, "BALANCE", balance);
                        TAB_SETROW(rec, "BALANCEREP", balanceRep);

                        TAB_SETROW(rec, "DOCNR", docnr);
                    }



                    public DataRow write(DataTable tab, int pos = -1)
                    {
                        var rec = tab.NewRow();

                        write(rec);

                        if (pos < 0)
                            tab.Rows.Add(rec);
                        else
                            tab.Rows.InsertAt(rec, pos);

                        return rec;

                    }

                }

                public class ValuesFinBalance
                {

                    public double balance = 0.0;
                    public double balanceRep = 0.0;


                    public void read(DataRow rec)
                    {

                        balance = CASTASDOUBLE(TAB_GETROW(rec, "TOT"));
                        balanceRep = CASTASDOUBLE(TAB_GETROW(rec, "TOTREP"));


                    }


                }

                public class ValuesClTran
                {

                    public object LOGICALREF;
                    public double AMOUNT;
                    public double REPORTNET;
                    public DateTime DATE_;
                    public short MODULENR;
                    public short TRCODE;
                    public object SOURCEFREF;
                    public short SIGN;

                    public ValuesClTran(DataRow rec)
                    {
                        LOGICALREF = (TAB_GETROW(rec, "LOGICALREF"));
                        AMOUNT = CASTASDOUBLE(TAB_GETROW(rec, "AMOUNT"));
                        REPORTNET = CASTASDOUBLE(TAB_GETROW(rec, "REPORTNET"));
                        DATE_ = CASTASDATE(TAB_GETROW(rec, "DATE_"));
                        MODULENR = CASTASSHORT(TAB_GETROW(rec, "MODULENR"));
                        TRCODE = CASTASSHORT(TAB_GETROW(rec, "TRCODE"));
                        SOURCEFREF = (TAB_GETROW(rec, "SOURCEFREF"));
                        SIGN = CASTASSHORT(TAB_GETROW(rec, "SIGN"));
                    }


                }

            }

            public class FormRef : Form
            {
                _PLUGIN PLUGIN;


                DataTable data;

                public List<DataRow> selected = new List<DataRow>();

                public FormRef(_PLUGIN pPLUGIN, string pText, DataTable pTab)
                {
                    PLUGIN = pPLUGIN;
                    data = pTab;

                    this.Text = string.Format("{0}", PLUGIN.LANG(pText));


                    init();

                    Size = new System.Drawing.Size(600, 400);
                }


                public void init()
                {

                    this.Icon = CTRL_FORM_ICON();
                    this.Size = new Size(1000, 700);
                    this.AutoSize = true;
                    //form.BackColor = Color.White;
                    this.StartPosition = FormStartPosition.CenterScreen;

                    var panelMain = new Panel() { Dock = DockStyle.Fill };



                    this.Controls.Add(panelMain);


                    this.WindowState = FormWindowState.Normal;

                    var btnClose = new Button() { Text = PLUGIN.LANG("T_CLOSE"), Image = RES_IMAGE("close_16x16"), ImageAlign = ContentAlignment.MiddleLeft, Width = 160, Dock = DockStyle.Right };
                    btnClose.Click += (s, arg) => { this.Close(); };

                    var panelBtnGen = new Panel() { Dock = DockStyle.Bottom, Height = 25 };



                    panelMain.Controls.Add(panelBtnGen);


                    panelBtnGen.Controls.Add(btnClose);








                    #region GRIDS

                    var grid = createGrid("grid");



                    foreach (DataColumn col in data.Columns)
                    {

                        var gridCol = new DataGridViewTextBoxColumn()
                        {
                            HeaderText = PLUGIN.LANG(CASTASSTRING(col.ExtendedProperties["title"])),
                            DataPropertyName = col.ColumnName,
                            AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells

                        };

                        gridCol.DefaultCellStyle.Format = CASTASSTRING(col.ExtendedProperties["format"]);

                        grid.Columns.Add(gridCol);


                    }


                    foreach (var x in new DataGridView[] { grid })
                    {
                        x.CellClick += (sender, e) =>
                        {

                            try
                            {
                                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                                    return;

                                DataGridView grid_ = sender as DataGridView;
                                if (grid_ == null)
                                    return;

                                var colObj_ = e.ColumnIndex >= 0 ? grid_.Columns[e.ColumnIndex] : null;
                                if (colObj_ == null)
                                    return;

                                var tab = grid_.DataSource as DataTable;

                                if (tab == null)
                                    return;


                            }
                            catch (Exception exc)
                            {
                                /// PLUGIN.MSGUSERERROR(exc.Message);
                                PLUGIN.LOG(exc);

                            }


                        };


                        ///////////
                        x.CellMouseDoubleClick += (sender, e) =>
                        {
                            try
                            {
                                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                                    return;

                                DataGridView grid_ = sender as DataGridView;
                                if (grid_ == null)
                                    return;

                                var tab_ = grid_.DataSource as DataTable;

                                var colObj_ = e.ColumnIndex >= 0 ? grid_.Columns[e.ColumnIndex] : null;
                                if (colObj_ == null)
                                    return;

                                DataRow ROW = TOOL_GRID.GET_GRID_ROW_DATA(grid_);
                                if (ROW == null)
                                    return;

                                selected.Add(ROW);

                                Close();

                            }
                            catch (Exception exc)
                            {
                                // PLUGIN.MSGUSERERROR(exc.Message);
                                PLUGIN.LOG(exc);
                            }

                        };



                        /////////
                    }

                    #endregion

                    panelMain.Controls.Add(grid);


                }

                DataGridView createGrid(string pName)
                {

                    var grid = new DataGridView();
                    grid.Name = pName;
                    grid.Dock = DockStyle.Fill;
                    grid.MultiSelect = false;
                    grid.ReadOnly = true;
                    grid.AllowUserToAddRows = false;
                    grid.AllowUserToDeleteRows = false;
                    grid.AllowUserToResizeColumns = false;
                    grid.AllowUserToResizeRows = false;

                    grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    grid.AutoGenerateColumns = false;
                    grid.BackgroundColor = Color.White;
                    grid.RowHeadersVisible = false;
                    grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    //grid.Height = 110;
                    // grid.AutoSize = true;
                    grid.BorderStyle = BorderStyle.None;


                    // grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                    grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    grid.ColumnHeadersDefaultCellStyle.Font = new Font(grid.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
                    grid.ColumnHeadersHeight *= 2;

                    grid.DataError += (s, e) =>
                    {

                        RUNTIMELOG(e.Exception.ToString());

                    };
                    return grid;
                }



            }


        }

        #endregion

        #endregion
