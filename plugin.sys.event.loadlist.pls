 #line 2
 
     #region BODY
        //BEGIN

        const int VERSION = 9;
        const string FILE = "plugin.sys.event.loadlist.pls";


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

                    x.MY_LOADLIST_USER = s.MY_LOADLIST_USER;

                    //
                    x._USER = PLUGIN.GETSYSPRM_USER();
                    x._FIRM = PLUGIN.GETSYSPRM_FIRM();
                    x._FIRMNAME = PLUGIN.GETSYSPRM_FIRMNAME();
                    x._PERIOD = PLUGIN.GETSYSPRM_PERIOD();


                    _SETTINGS.BUF = x;

                }

                public string MY_LOADLIST_USER;


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
            public string MY_LOADLIST_USER
            {
                get
                {
                    return (_GET("MY_LOADLIST_USER", "1,2"));
                }
                set
                {
                    _SET("MY_LOADLIST_USER", value);
                }

            }



            //

            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_LOADLIST_USER),
                     FORMAT(pPLUGIN.GETSYSPRM_USER())
                     ) >= 0;
            }

        }

        #endregion

        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "Load List";


            public class L
            {

            }
        }

        const string event_LOADLIST_ = "hadlericom_loadlist_";
        const string event_LOADLIST_DOC_DIMENSIONS = "hadlericom_loadlist_doc_dimensions";
        const string event_LOADLIST_DOCS_INFO = "hadlericom_loadlist_docs_info";
        const string event_LOADLIST_PACK_INFO = "hadlericom_loadlist_pack_info";
        const string event_LOADLIST_LOAD_LIST = "hadlericom_loadlist_load_list";


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
            public string LOAD_LIST = "Load List";

            public void lang_az()
            {
                LOAD_LIST = "Yükləmə Listi";
                COUNT_DOC = "Sənəd Sayı";
            }

            public void lang_ru()
            {

                LOAD_LIST = "Список Погрузки";
                COUNT_DOC = "Кол. Док.";
            }

            public void lang_tr()
            {

                LOAD_LIST = "Yükleme Listesi";
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




                var isListClient = fn.StartsWith("ref.sls.doc.inv");


                if (isListClient)
                {
                    foreach (var ctrl in CONTROL_DESTRUCT(FORM))
                    {
                        {
                            var menuItem = ctrl as ToolStripItem;
                            if (menuItem != null && menuItem.Name == "cMenuGridInfoPlugin")
                            {
                                {
                                    var args = new Dictionary<string, object>() { 
 
            { "_cmd" ,"add"},
            { "_type" ,"item"},
            { "_name" , event_LOADLIST_DOC_DIMENSIONS},

            { "_infoLocation" ,"event"},
            { "_infoArg" ,event_LOADLIST_DOC_DIMENSIONS},

            { "Text" ,LANG("T_SIZE")},
            { "ImageName" ,"sum_16x16"},
             { "Name" ,event_LOADLIST_DOC_DIMENSIONS},
            };

                                    RUNUIINTEGRATION(menuItem, args);

                                }

                            }
                        }

                    }



                }



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
		 
			{ "Text" ,_LANG.L.LOAD_LIST},
			{ "ImageName" ,"car_truck_32x32"},
			{ "Name" ,event_LOADLIST_},
            };

                                RUNUIINTEGRATION(tree, args);

                            }

                            {
                                var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
            { "_root" ,event_LOADLIST_},
			{ "CmdText" ,"event name::"+event_LOADLIST_LOAD_LIST},
			{ "Text" ,LANG("T_CREATE")},
			{ "ImageName" ,"tools_32x32"},
			{ "Name" ,event_LOADLIST_LOAD_LIST},
            };

                                RUNUIINTEGRATION(tree, args);

                            }


                            {
                                var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
            { "_root" ,event_LOADLIST_},
			{ "CmdText" ,"event name::"+event_LOADLIST_DOCS_INFO},
			{ "Text" ,LANG("T_INFO - T_DOCS")},
			{ "ImageName" ,"info_32x32"},
			{ "Name" ,event_LOADLIST_DOCS_INFO},
            };

                                RUNUIINTEGRATION(tree, args);

                            }

                            {
                                var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
            { "_root" ,event_LOADLIST_},
			{ "CmdText" ,"event name::"+event_LOADLIST_PACK_INFO},
			{ "Text" ,LANG("T_INFO - T_PACKAGE")},
			{ "ImageName" ,"info_32x32"},
			{ "Name" ,event_LOADLIST_PACK_INFO},
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

                    case event_LOADLIST_DOC_DIMENSIONS:
                        {
                            DataRow row = arg1 as DataRow;

                            if (row == null)
                            {
                                var grid_ = CONTROL_SEARCH(arg1 as Form, "cGrid") as DataGridView;
                                if (grid_ != null)
                                    row = TOOL_GRID.GET_GRID_ROW_DATA(grid_);
                            }

                            MY_SHOW_DOC_DIMENSIONS(row);
                        }
                        break;
                    case event_LOADLIST_DOCS_INFO:
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
                    case event_LOADLIST_PACK_INFO:
                        {
                            DateTime date = CASTASDATE("");

                            var cmdLine = arg3 as string;
                            if (cmdLine != null)
                            {
                                date = CASTASDATE(CMDLINEGETARG(cmdLine, "date")).Date;

                            }

                            MY_SHOW_PACK_INFO(date);
                        }
                        break;
                    case event_LOADLIST_LOAD_LIST:
                        {
                            MY_LOAD_LIST();
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



        static DataRow MY_GET_STOCK_REC(_PLUGIN pPLUGIN, object pLRef)
        {
            if (ISEMPTYLREF(pLRef))
                return null;

            return TAB_GETLASTROW(pPLUGIN.SQL("SELECT * FROM LG_$FIRM$_ITEMS WHERE LOGICALREF = @P1", new object[] { pLRef }));
        }

        private void MY_SHOW_DOC_DIMENSIONS(DataRow pRecord)
        {

            if (pRecord == null)
                return;


            var docLRef = pRecord["LOGICALREF"];

            var weight = DOC_DIMENSIONS(this, docLRef, true);

            MSGUSERINFO("T_WEIGHT: " + FORMAT(ROUND(weight, 2)));

        }
        private void MY_LOAD_LIST()
        {

            if (!MY_TOOL.MY_IS_FORM_OPEN(typeof(TY_FORMS.FormLoadList)))
            {
                var f = new TY_FORMS.FormLoadList(this, null);
                f.Show();
            }
        }

        private void MY_SHOW_PACK_INFO(DateTime pDate)
        {
            if (ISEMPTY(pDate))
            {
                pDate = DateTime.Now.Date;

                var date = MY_TOOL.MY_ASK_DATE(this, "T_DATE", pDate);
                if (date == null || ISEMPTY(date.Value))
                    return;

                pDate = date.Value.Date;

            }

            DOC_VALIDATE(this, pDate);

            var info = SQL(@"

SELECT INV.WFSTATUS AS STATUS,
(SELECT CODE||'/'||NAME FROM LG_$FIRM$_PROJECT AS PRJ WHERE INV.PROJECTREF = PRJ.LOGICALREF) AS PACKAGE_TITLE,
COUNT(*) AS COUNT,
SUM(COALESCE (
(
SELECT  
SUM(L.AMOUNT* L.UINFO1/L.UINFO2 * I.WEIGHT) AS WEIGHT 
from LG_$FIRM$_$PERIOD$_STLINE AS L INNER JOIN LG_$FIRM$_ITMUNITA AS I ON L.STOCKREF = I.ITEMREF AND I.LINENR=1
WHERE L.INVOICEREF = INV.LOGICALREF AND L.LINETYPE IN (0,1)
),0
)) AS WEIGHT 
FROM LG_$FIRM$_$PERIOD$_INVOICE AS INV 
WHERE INV.DATE_ = @P1 AND INV.TRCODE = 8 AND INV.CANCELLED = 0 
GROUP BY INV.WFSTATUS,INV.PROJECTREF
ORDER BY INV.WFSTATUS ASC,INV.PROJECTREF ASC 

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
               HTMLENCODE(_LANG.L.LOAD_LIST + " - " + LANG("T_PACKAGE"))
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




                foreach (var data in new DataTable[] { info })
                {


                    res.AppendLine("<br/>");

                    res.AppendLine("<table style=';'>");

                    res.AppendLine("<tr>");
                    foreach (var cell in new string[] { 
                                LANG("T_STATUS"), 
                                LANG("T_PACKAGE") , 
                                LANG("T_COUNT"),
                                LANG("T_WEIGHT"), 
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
                                        RESOLVESTR("[list::LIST_GEN_WORKFLOW_STATUS/" + FORMAT(CASTASSHORT(TAB_GETROW(row, "STATUS"))) + "]"),
                                        CASTASSTRING(TAB_GETROW(row, "PACKAGE_TITLE")) ,  
                                        FORMAT(ROUND(CASTASDOUBLE(TAB_GETROW(row, "COUNT")), 2),"#,#0.#") ,
                                        FORMAT(ROUND(CASTASDOUBLE(TAB_GETROW(row, "WEIGHT")), 2),"#,#0.#") , 
                                    };

                        var backColor = "#FFFFFF";

                        if (isDark)
                            backColor = "#F0F0F0";


                        var fontWeight = isBold ? "bold" : "normal";
                        var numColsStartIndx = 3;

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

            DOC_VALIDATE(this, pDate);

            var infoBySpCode = SQL(@"

SELECT INV.WFSTATUS AS STATUS,INV.DEPARTMENT AS DEP,CL.SPECODE AS CODE,COUNT(*) AS COUNT,
SUM(COALESCE (
(
SELECT  
SUM(L.AMOUNT* L.UINFO1/L.UINFO2 * I.WEIGHT) AS WEIGHT 
from LG_$FIRM$_$PERIOD$_STLINE AS L INNER JOIN LG_$FIRM$_ITMUNITA AS I ON L.STOCKREF = I.ITEMREF AND I.LINENR=1
WHERE L.INVOICEREF = INV.LOGICALREF AND L.LINETYPE IN (0,1)
),0
)) AS WEIGHT 
FROM LG_$FIRM$_$PERIOD$_INVOICE AS INV LEFT JOIN LG_$FIRM$_CLCARD AS CL ON INV.CLIENTREF = CL.LOGICALREF 
WHERE INV.DATE_ = @P1 AND INV.TRCODE = 8 AND INV.CANCELLED = 0 
GROUP BY INV.WFSTATUS,INV.DEPARTMENT,CL.SPECODE
ORDER BY INV.WFSTATUS ASC,INV.DEPARTMENT ASC,CL.SPECODE ASC

", new object[] { pDate });

            var infoGlob = SQL(@"

SELECT INV.WFSTATUS AS STATUS, COUNT(*) AS COUNT,
SUM(COALESCE (
(SELECT  
SUM(L.AMOUNT* L.UINFO1/L.UINFO2 * I.WEIGHT) AS WEIGHT 
from LG_$FIRM$_$PERIOD$_STLINE AS L INNER JOIN LG_$FIRM$_ITMUNITA AS I ON L.STOCKREF = I.ITEMREF AND I.LINENR=1
WHERE L.INVOICEREF = INV.LOGICALREF AND L.LINETYPE IN (0,1)
),0
)) AS WEIGHT from LG_$FIRM$_$PERIOD$_INVOICE AS INV 
WHERE INV.DATE_ = @P1 AND INV.TRCODE = 8 AND INV.CANCELLED = 0 
GROUP BY INV.WFSTATUS 
ORDER BY INV.WFSTATUS ASC 

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
               HTMLENCODE(_LANG.L.LOAD_LIST + " - " + LANG("T_DOCS"))
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
                                LANG("T_WEIGHT"),
                                
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
                                        RESOLVESTR("[list::LIST_GEN_WORKFLOW_STATUS/" + FORMAT(CASTASSHORT(TAB_GETROW(row, "STATUS"))) + "]"),
                                        (hasDep?  CASTASSTRING(TAB_GETROW(row, "DEP")):null), 
                                       (hasCode?  CASTASSTRING(TAB_GETROW(row, "CODE")):null), 
                                        FORMAT(ROUND(CASTASDOUBLE(TAB_GETROW(row, "COUNT")), 2),"#,#0.#") ,
                                        FORMAT(ROUND(CASTASDOUBLE(TAB_GETROW(row, "WEIGHT")), 2),"#,#0.#") , 
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

        static void DOC_VALIDATE(_PLUGIN pPLUGIN, DateTime pDate)
        {

            var errRec = TAB_GETLASTROW(pPLUGIN.SQL(@"

select  
L.STOCKREF 
from LG_$FIRM$_$PERIOD$_STLINE AS L INNER JOIN LG_$FIRM$_ITMUNITA AS I ON L.STOCKREF = I.ITEMREF AND I.LINENR=1
WHERE L.DATE_ = @P1 AND L.LINETYPE IN (0,1) AND L.TRCODE = 8 AND I.WEIGHT < 0.01

", new object[] { pDate.Date }));

            if (errRec != null)
            {

                var recMat = MY_GET_STOCK_REC(pPLUGIN, errRec["STOCKREF"]);

                if (recMat != null)
                {

                    var title = CASTASSTRING(recMat["CODE"]) + "/" + CASTASSTRING(recMat["NAME"]);
                    throw new Exception("T_MSG_INVALID_RECODR - T_MATERIAL [" + title + "]");

                }

            }

        }

        static void DOC_VALIDATE_WEIGHT(_PLUGIN pPLUGIN, object pInvoiceRef )
        {
            var errRec = TAB_GETLASTROW(pPLUGIN.SQL(@"

select  
L.STOCKREF 
from LG_$FIRM$_$PERIOD$_STLINE AS L INNER JOIN LG_$FIRM$_ITMUNITA AS I ON L.STOCKREF = I.ITEMREF AND I.LINENR=1
WHERE L.INVOICEREF = @P1 AND L.LINETYPE IN (0,1) AND L.TRCODE = 8 AND I.WEIGHT < 0.01

", new object[] { pInvoiceRef }));

            if (errRec != null)
            {

                var recMat = MY_GET_STOCK_REC(pPLUGIN, errRec["STOCKREF"]);

                if (recMat != null)
                {

                    var title = CASTASSTRING(recMat["CODE"]) + "/" + CASTASSTRING(recMat["NAME"]);
                    throw new Exception("T_MSG_INVALID_RECODR - T_MATERIAL (T_WEIGHT) [" + title + "]");

                }

            }


        }
        static double DOC_DIMENSIONS(_PLUGIN pPLUGIN, object pInvoiceRef, bool pValidate = true)
        {



            if (pValidate)
            {
                DOC_VALIDATE_WEIGHT(pPLUGIN, pInvoiceRef);
            }


            var info = pPLUGIN.SQL(@"

select  
SUM(L.AMOUNT* L.UINFO1/L.UINFO2 * I.WEIGHT) AS WEIGHT 
from LG_$FIRM$_$PERIOD$_STLINE AS L INNER JOIN LG_$FIRM$_ITMUNITA AS I ON L.STOCKREF = I.ITEMREF AND I.LINENR=1
WHERE L.INVOICEREF = @P1 AND L.LINETYPE IN (0,1)

", new object[] { pInvoiceRef });

            var WEIGHT = CASTASDOUBLE(TAB_GETROW(info, "WEIGHT"));

            return WEIGHT;





        }


        static int[] SELECT_INVOICE_SLS(_PLUGIN pPLUGIN, DateTime pDate, short pWfStatus, string pClientSpeCode1, short pDocDep)
        {


            pDate = pDate.Date;
            pClientSpeCode1 = pClientSpeCode1.Trim();

            DataTable info = null;

            if (!ISEMPTY(pClientSpeCode1))
            {
                info = pPLUGIN.SQL(@"

SELECT INV.LOGICALREF from LG_$FIRM$_$PERIOD$_INVOICE AS INV INNER JOIN LG_$FIRM$_CLCARD AS CL ON INV.CLIENTREF = CL.LOGICALREF 
WHERE INV.DATE_ = @P1 AND INV.TRCODE = 8 AND INV.CANCELLED = 0 AND INV.WFSTATUS = @P2 AND (CL.SPECODE LIKE @P3 )

", new object[] { pDate, pWfStatus, pClientSpeCode1 });

            }
            else
                if (pDocDep > 0)
                {

                    info = pPLUGIN.SQL(@"

SELECT INV.LOGICALREF from LG_$FIRM$_$PERIOD$_INVOICE AS INV INNER JOIN LG_$FIRM$_CLCARD AS CL ON INV.CLIENTREF = CL.LOGICALREF 
WHERE INV.DATE_ = @P1 AND INV.TRCODE = 8 AND INV.CANCELLED = 0 AND INV.WFSTATUS = @P2 AND (INV.DEPARTMENT = @P3 )

", new object[] { pDate, pWfStatus, pDocDep });

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

        static DataTable SELECT_INVOICE_SLS(_PLUGIN pPLUGIN, object pDocLref)
        {





            var tab = (pPLUGIN.SQL(@"

select 
INV.LOGICALREF,
FICHENO,DATE_,DOCTRACKINGNR, GENEXP1 ,
CYPHCODE,
SOURCEINDEX,
DEPARTMENT,
(SELECT CODE||'/'||NAME FROM LG_$FIRM$_PROJECT AS PRJ WHERE INV.PROJECTREF = PRJ.LOGICALREF) AS PACKAGE_TITLE,
(SELECT CODE||'/'||DEFINITION_||'/'||SPECODE FROM LG_$FIRM$_CLCARD AS CL WHERE CL.LOGICALREF = INV.CLIENTREF ) AS CLCARD_TITLE,
NETTOTAL,
(cast (0 as float)) AS WEIGHT
from LG_$FIRM$_$PERIOD$_INVOICE AS INV where INV.LOGICALREF = @P1
AND INV.TRCODE = 8 AND INV.CANCELLED = 0 AND INV.WFSTATUS = 0

", new object[] { pDocLref }));





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


            public static string SQLTEXT(string pSqlText)
            {
                var dic = new Dictionary<string, string>(){
                  {"$FIRM$",_SETTINGS.BUF._FIRM.ToString().PadLeft(3,'0')},
                  {"$PERIOD$",_SETTINGS.BUF._PERIOD.ToString().PadLeft(2,'0')},
                  };

                foreach (var itm in dic)
                    pSqlText = pSqlText.Replace(itm.Key, itm.Value);

                return pSqlText;
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


        }


        class TY_FORMS
        {

            public class FormLoadList : Form
            {
                _PLUGIN PLUGIN;

                Filter filter = new Filter();

                public FormLoadList(_PLUGIN pPLUGIN, Filter pFilter)
                {
                    PLUGIN = pPLUGIN;
                    filter = pFilter ?? new Filter() { date = DateTime.Now.Date };

                    this.Text = string.Format("{0}", _LANG.L.LOAD_LIST);

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
                    var tabPageDocsAll = new TabPage() { Text = PLUGIN.LANG("T_DOC_STOCK_INV_8") };
                    var tabPageDocsPacked = new TabPage() { Text = PLUGIN.LANG("T_PACKAGE") };


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

                    var btnInfoPack = new Button() { Text = PLUGIN.LANG("T_INFO - T_PACKAGE"), Image = RES_IMAGE("info_16x16"), ImageAlign = ContentAlignment.MiddleLeft, Width = labelWidth, Dock = DockStyle.Left };
                    btnInfoPack.Click += (s, arg) => { this.showPackInfo(); };


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
                    btnAddDoc.Click += (s, arg) => { this.addInvoiceManual(); };

                    var btnCalcPack = new Button()
                    {
                        Text = PLUGIN.LANG("T_CALCULATE"),
                        Image = RES_IMAGE("func_16x16"),
                        ImageAlign = ContentAlignment.MiddleLeft,
                        Width = labelWidth
                    };
                    btnCalcPack.Click += (s, arg) => { this.calcPack(); };

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


                    //panelBtn.Controls.Add(btnInfoPack);
                    //panelBtn.Controls.Add(btnInfoDocs);




                    panelBtnGen.Controls.Add(btnClose);

                    panelBtnGen.Controls.Add(btnInfoPack);
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
                           new Button(){Text=PLUGIN.LANG("T_PRINT (T_HEADER)"),Image = RES_IMAGE("print_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_package_print_header",Width=labelWidth,Dock = DockStyle.Right  },
                           new Button(){Text=PLUGIN.LANG("T_PRINT (T_INVOICE)"),Image = RES_IMAGE("print_16x16"),ImageAlign = ContentAlignment.MiddleLeft,Name = "do_package_print_inv",Width=labelWidth,Dock = DockStyle.Right  },


                            }
                          );

                    }

                    panelFilterCl.Controls.AddRange(
                      new Control[]{
                            new Label(){Text = PLUGIN.LANG("T_SPECODE"), Width = labelWidth/2, TextAlign = ContentAlignment.MiddleLeft,Dock = DockStyle.Left},
                            new TextBox() {   Text = filter.clientMarkCode, Name = "filter_clientmarkcode",Width=labelWidth},
                           new Button(){Text="",Image = RES_IMAGE("find_16x16"),ImageAlign = ContentAlignment.MiddleCenter,Name = "do_filter_clientmarkcode",Width=30 },

                           new Label(){Text = PLUGIN.LANG("T_DEP"), Width = labelWidth/2, TextAlign = ContentAlignment.MiddleRight,Dock = DockStyle.Left},
                            new NumericUpDown() {   Value = filter.docDep, Name = "filter_docdep",Width=labelWidth/2,Minimum = 0, Maximum = short.MaxValue, DecimalPlaces = 0},
                           new Button(){Text="",Image = RES_IMAGE("find_16x16"),ImageAlign = ContentAlignment.MiddleCenter,Name = "do_filter_docdep",Width=30 },
                          // new TextBox(){Width=700,ReadOnly=true,Name="desc_filter_client"}
                            }
                      );

                    {


                        panelGenInfoDocsAll.Controls.AddRange(new Control[]{

                              
                              new Label(){Text = PLUGIN.LANG("T_WEIGHT (T_ALL)"), Width = labelWidth/2, TextAlign = ContentAlignment.MiddleRight,ForeColor = Color.Green },
                            new TextBox() {   Text = "0", Name = "val_docs_all_weight",Width=labelWidth/2,ReadOnly=true, TextAlign = HorizontalAlignment.Right },


                             new Label(){Text = PLUGIN.LANG("T_COUNT"), Width = labelWidth/2, TextAlign = ContentAlignment.MiddleRight },
                            new TextBox() {   Text = "0", Name = "val_docs_all_count",Width=labelWidth/2,ReadOnly=true, TextAlign = HorizontalAlignment.Right  },
                        

                          
                            
                            });

                    }


                    {



                        panelGenInfoDocsPacked.Controls.AddRange(new Control[]{
 

                              new Label(){Text = PLUGIN.LANG("T_WEIGHT (T_PACKAGE)"), Width = labelWidth/2, TextAlign = ContentAlignment.MiddleRight,ForeColor = Color.Blue },
                            new TextBox() {   Text = "0", Name = "val_docs_pack_weight",Width=labelWidth/2,ReadOnly=true, TextAlign = HorizontalAlignment.Right  },


                             new Label(){Text = PLUGIN.LANG("T_COUNT"), Width = labelWidth/2, TextAlign = ContentAlignment.MiddleRight },
                            new TextBox() {   Text = "0", Name = "val_docs_pack_count",Width=labelWidth/2,ReadOnly=true, TextAlign = HorizontalAlignment.Right },
                        
                         
                             new Label(){Text = PLUGIN.LANG("T_VEHICLE"), Width = labelWidth/2, TextAlign = ContentAlignment.MiddleLeft  },
                           new TextBox() {   Text = filter.vechile, Name = "filter_vehicle",Width=labelWidth , ReadOnly=true },

                           new Button(){Text="",Image = RES_IMAGE("find_16x16"),ImageAlign = ContentAlignment.MiddleCenter,Name = "do_filter_vehicle",Width=labelWidth/4   },
                            new Label(){Text = PLUGIN.LANG("T_WEIGHT (T_MAX)"), Width = labelWidth/2, TextAlign = ContentAlignment.MiddleRight  },
                           new TextBox() {   Text = FORMAT( filter.maxWeight,"#,#0.##"), Name = "val_weight_max",Width=labelWidth/2 , TextAlign = HorizontalAlignment.Right,
                               ReadOnly=true, Font = new Font(this.Font, FontStyle.Bold) },
                         
                                     new Label(){Text = PLUGIN.LANG("T_PACKAGE (T_NR)"), Width = labelWidth/2, TextAlign = ContentAlignment.MiddleLeft },
                            new TextBox() {   Text = filter.packageNr, Name = "filter_packagenr",Width=labelWidth , ReadOnly=true  },
                           

                            
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
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_TRACKING"), DataPropertyName = "DOCTRACKINGNR", Width = 100, Frozen = false },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_INFO"), DataPropertyName = "GENEXP1", Width = 120, Frozen = false },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_AUTHCODE"), DataPropertyName = "CYPHCODE", Width = 50, Frozen = false },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_WH"), DataPropertyName = "SOURCEINDEX", Width = 40, Frozen = false },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_DEP"), DataPropertyName = "DEPARTMENT", Width = 40, Frozen = false },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_PACKAGE"), DataPropertyName = "PACKCODE", Width = 100, Frozen = false },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_CONTRACTOR"), DataPropertyName = "CLCARD_TITLE", Width = 260, Frozen = false },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_TOTAL"), DataPropertyName = "NETTOTAL", Width = 80, Frozen = false },
                                                new DataGridViewTextBoxColumn() { HeaderText = PLUGIN.LANG("T_WEIGHT"), DataPropertyName = "WEIGHT", Width = 80, Frozen = false }
                                           );

                    gridDocsAll.Columns.Add(new DataGridViewButtonColumn() { Width = 80, Text = PLUGIN.LANG("T_ADD"), Tag = "cmd/addtopack", UseColumnTextForButtonValue = true });
                    gridDocsPacked.Columns.Add(new DataGridViewButtonColumn() { Width = 80, Text = PLUGIN.LANG("T_DELETE"), Tag = "cmd/delfrompack", UseColumnTextForButtonValue = true });

                    foreach (var x in new DataGridView[] { gridDocsAll, gridDocsPacked })
                        foreach (DataGridViewColumn colObj in x.Columns)
                        {
                            colObj.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                            colObj.SortMode = DataGridViewColumnSortMode.Programmatic;

                            switch (colObj.DataPropertyName)
                            {
                                case "NETTOTAL":
                                    colObj.DefaultCellStyle.Format = "#,0.00;-#,0.00;''";
                                    colObj.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                                    break;
                                case "WEIGHT":
                                    colObj.DefaultCellStyle.Format = "#,0.##;-#,0.##;''";
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
                                    var cmdEdit = "adp.sls.doc.inv.8 cmd::edit lref::" + lref;
                                    var cmdView = "adp.sls.doc.inv.8 cmd::view lref::" + lref;

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
                            case "do_filter_selectpackagenr":
                            case "do_filter_vehicle":
                            case "do_filter_clientmarkcode":
                            case "do_filter_docdep":
                            case "do_package_save":
                            case "do_package_print_header":
                            case "do_package_print_inv":
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
                        //SUM(L.AMOUNT* L.UINFO1/L.UINFO2 * I.WEIGHT) AS WEIGHT 
                        //from LG_$FIRM$_$PERIOD$_STLINE AS L INNER JOIN LG_$FIRM$_ITMUNITA AS I ON L.STOCKREF = I.ITEMREF AND I.LINENR=1
                        //WHERE L.INVOICEREF = INV.LOGICALREF AND L.LINETYPE IN (0,1)
                        //),0
                        //)) AS WEIGHT 
                        //FROM LG_$FIRM$_$PERIOD$_INVOICE AS INV WHERE DATE_ = @P1 AND TRCODE = 8 AND TRACKNR LIKE @P2 
                        //GROUP BY INV.TRACKNR,INV.PROJECTREF
                        //ORDER BY INV.TRACKNR DESC,INV.PROJECTREF DESC",
                        //                            new object[] { docDate, packNrPrefix + "%" });

                        //                        packs.Columns["PACKCODE"].ExtendedProperties["title"] = "T_CODE";
                        //                        packs.Columns["FRG_TITLE"].ExtendedProperties["title"] = "T_VEHICLE";
                        //                        packs.Columns["COUNT"].ExtendedProperties["title"] = "T_COUNT";
                        //                        packs.Columns["WEIGHT"].ExtendedProperties["title"] = "T_WEIGHT";
                        //                        //
                        //                        packs.Columns["COUNT"].ExtendedProperties["format"] = "#,#0";
                        //                        packs.Columns["WEIGHT"].ExtendedProperties["format"] = "#,#0.##";

                        //                        var form = new FormRef(PLUGIN, "T_PACKAGE", packs);

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
                void doPrintPackageHeader(string pPackage)
                {
                    try
                    {
                        object prjLRef = null;

                        var packageNr = pPackage;
                        if (ISEMPTY(packageNr))
                        {
                            var rec = doAskPackage();
                            if (rec == null)
                                return;
                            prjLRef = CASTASINT(TAB_GETROW(rec, "LOGICALREF"));

                            if (ISEMPTYLREF(prjLRef))
                                return;
 
                        }
                        else
                        {
                            prjLRef = CASTASINT(PLUGIN.SQLSCALAR("SELECT LOGICALREF FROM LG_$FIRM$_PROJECT WHERE CODE = @P1", new object[] { packageNr }));
                            if (ISEMPTYLREF(prjLRef))
                                return;
                        }
                        //REP_QUICK_MODE_B::1 REP_DEF_FILTER_B::1 DSGN_OUTPUT_B::1 REP_DSG_KEY_WORDS_S::print
                        string cmd_ = "rep loc::rep.loadlist.package.header filter::filter_PROJECTREF," +
                            FORMATSERIALIZE(prjLRef)
                            + " REP_QUICK_MODE_B::1 REP_DEF_FILTER_B::1";


                        PLUGIN.EXECMDTEXT(cmd_);

                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(exc.Message);
                    }
                }
                void doPrintPackageInv(string pPackage)
                {
                    try 
                    {
                        object prjLRef = null;

                        var packageNr = pPackage;
                        if (ISEMPTY(packageNr))
                        {
                            var rec = doAskPackage();
                            if (rec == null)
                                return;
                            prjLRef = CASTASINT(TAB_GETROW(rec, "LOGICALREF"));

                            if (ISEMPTYLREF(prjLRef))
                                return;

                        }
                        else
                        {
                            prjLRef = CASTASINT(PLUGIN.SQLSCALAR("SELECT LOGICALREF FROM LG_$FIRM$_PROJECT WHERE CODE = @P1", new object[] { packageNr }));
                            if (ISEMPTYLREF(prjLRef))
                                return;
                        }
                        //REP_QUICK_MODE_B::1 REP_DEF_FILTER_B::1 DSGN_OUTPUT_B::1 REP_DSG_KEY_WORDS_S::print
                        string cmd_ = "rep loc::rep.loadlist.package.inv filter::filter_PROJECTREF," +
                            FORMATSERIALIZE(prjLRef)
                            + " REP_QUICK_MODE_B::1 REP_DEF_FILTER_B::1";


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

                DataRow _moveInvoice(DataRow pRec, DataTable pDest)
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
                                        var newRec = _moveInvoice(rec, getTableOfDocsPacked());

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
                                        var newRec = _moveInvoice(rec, getTableOfDocsAll());
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

                        if (btn.Name == "do_filter_clientmarkcode")
                        {

                            var valCtrl = CONTROL_SEARCH(this, "filter_clientmarkcode") as TextBox;
                            if (valCtrl != null)
                            {
                                DataRow[] res_ = PLUGIN.REF("ref.gen.rec.mcodes/1/26", "SPECODE", (valCtrl.Text));
                                if (res_ != null && res_.Length > 0)
                                {
                                    valCtrl.Text = CASTASSTRING(TAB_GETROW(res_[0], "SPECODE"));
                                }
                            }

                            return;
                        }

                        if (btn.Name == "do_filter_vehicle")
                        {

                            var valCtrlMaxWeight = CONTROL_SEARCH(this, "val_weight_max") as TextBox;

                            var valCtrl = CONTROL_SEARCH(this, "filter_vehicle") as TextBox;
                            if (valCtrl != null)
                            {
                                DataRow[] res_ = PLUGIN.REF("ref.gen.rec.transporttype", "CODE", (valCtrl.Text));
                                if (res_ != null && res_.Length > 0)
                                {
                                    valCtrl.Text = CASTASSTRING(TAB_GETROW(res_[0], "CODE"));

                                    filter.maxWeight = CASTASDOUBLE(TAB_GETROW(res_[0], "FLOATF1"));
                                    if (valCtrlMaxWeight != null)
                                        valCtrlMaxWeight.Text = FORMAT(filter.maxWeight, "#,#0.##");
                                }
                            }

                            return;
                        }
                        if (btn.Name == "do_package_print_header")
                        {
                            doPrintPackageHeader(filter.packageNr);
                        }
                        if (btn.Name == "do_package_print_inv")
                        {
                            doPrintPackageInv(filter.packageNr);
                        }
                        if (btn.Name == "do_package_save")
                        {

                            var tab = getTableOfDocsPacked();
                            if (tab.Rows.Count == 0)
                                return;

                            var vechile = filter.vechile;

                            if (ISEMPTY(vechile))
                            {
                                PLUGIN.MSGUSERERROR("T_MSG_SET_REQFIELDS - T_VEHICLE");
                                return;
                            }

                            var vechileAndDriver = CASTASSTRING(PLUGIN.SQLSCALAR("SELECT CODE||'/'||NAME FROM L_FRGTYPES F WHERE F.CODE = @P1", new object[] { vechile }));
                            //TODO requre vehicle

                            var packageNr = filter.packageNr = filter.generatePackNr(PLUGIN);

                            //check if over vehicle max weight

                            //save 
                            PLUGIN.INVOKEINBATCH(() =>
                            {

                                var newProject = PLUGIN.GETSEQ("LG_$FIRM$_PROJECT");
                                var now = DateTime.Now.Date;
                                //create project/package

                                PLUGIN.SQL(
                                    "INSERT INTO LG_$FIRM$_PROJECT(LOGICALREF,CODE,NAME,BEGDATE,ENDDATE) VALUES(@P1,@P2,@P3,@P4,@P5)",
                                    new object[] { newProject, packageNr, vechileAndDriver, now, now });

                                foreach (DataRow r in tab.Rows)
                                {
                                    var docRef = TAB_GETROW(r, "LOGICALREF");
                                    var isValidInv = ISTRUE(PLUGIN.SQLSCALAR(@"
SELECT 1 FROM LG_$FIRM$_$PERIOD$_INVOICE WHERE LOGICALREF = @P1 AND WFSTATUS = 0
", new object[] { docRef }));

                                    if (!isValidInv)
                                        continue;

                                    PLUGIN.SQL(@"
UPDATE LG_$FIRM$_$PERIOD$_INVOICE SET PROJECTREF = @P1, WFSTATUS = 3, RECSTATUS=RECSTATUS+1  WHERE LOGICALREF = @P2 AND WFSTATUS = 0;
", new object[] { newProject, docRef });
                                    PLUGIN.SQL(@"
UPDATE LG_$FIRM$_$PERIOD$_STFICHE SET PROJECTREF = @P1, RECSTATUS=RECSTATUS+1 WHERE INVOICEREF = @P2 ;
", new object[] { newProject, docRef });
                                    PLUGIN.SQL(@"
UPDATE LG_$FIRM$_$PERIOD$_STLINE SET PROJECTREF = @P1, RECSTATUS=RECSTATUS+1 WHERE INVOICEREF = @P2 ;
", new object[] { newProject, docRef });
                                    PLUGIN.SQL(@"
UPDATE LG_$FIRM$_$PERIOD$_CLFLINE SET CLPRJREF = @P1, RECSTATUS=RECSTATUS+1 WHERE SOURCEFREF = @P2 AND MODULENR = 4 AND TRCODE = 38 ;
", new object[] { newProject, docRef });
                                }

                            });
                            //clean
                            tab.Clear();
                            filter.vechile = "";
                            //filter.packageNr = "";//dont reset
                            filter.writeFilter(this);
                            //
                            refreshCalc();

                            return;
                        }


                        if (btn.Name == "do_filter_selectpackagenr")
                        {
                            //doAskPackage(null);

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
                        PLUGIN.EXECMDTEXT("event name::" + event_LOADLIST_DOCS_INFO + " date::" + LEFT(FORMAT(filter.date), 10));
                    }
                    catch (Exception exc)
                    {
                        PLUGIN.LOG(exc);
                        PLUGIN.MSGUSERERROR(exc.Message);
                    }
                }

                bool _isInvoiceAdded(object pDocRef)
                {
                    foreach (var t in new DataTable[] { getTableOfDocsAll(), getTableOfDocsPacked() })
                    {
                        if (TAB_SEARCH(t, "LOGICALREF", pDocRef) != null)
                            return true;

                    }
                    return false;
                }

                DataRow _addInvoiceToDocsAll(object pDocRef, bool pValidate = true)
                {

                    if (ISEMPTYLREF(pDocRef))
                        return null;

                    var gridDocsAll = getGridOfDocsAll();
                    var tabAllDocs = getTableOfDocsAll();



                    if (_isInvoiceAdded(pDocRef))
                        return null;
                     
                    //if WFSTATUS != 0 rec will be null
                    var invRec = TAB_GETLASTROW(SELECT_INVOICE_SLS(PLUGIN, pDocRef));
                    if (invRec != null)
                    {
                        var weight = DOC_DIMENSIONS(PLUGIN, pDocRef, pValidate);
                        TAB_SETROW(invRec, "WEIGHT", weight);

                        var newRec = tabAllDocs.NewRow();


                        foreach (DataColumn c in tabAllDocs.Columns)
                            TAB_SETROW(newRec, c.ColumnName, TAB_GETROW(invRec, c.ColumnName));

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

                        var markCode = filter.clientMarkCode.Trim();

                        if (ISEMPTY(markCode) && filter.docDep <= 0)
                            return;

                        var arr = SELECT_INVOICE_SLS(PLUGIN, filter.date, 0, markCode, filter.docDep);

                        DataRow lastRec = null;
                        foreach (var docLRef in arr)
                        {
                            DOC_VALIDATE_WEIGHT(PLUGIN, docLRef);
                        }

                        foreach (var docLRef in arr)
                        {
                            var newRec = _addInvoiceToDocsAll(docLRef,false);
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

                void calcPack()
                {
                    try
                    {

                        var weight = MY_TOOL.MY_ASK_NUM(PLUGIN, "T_WEIGHT", filter.maxWeight, 0);
                        if (weight < 0.01)
                            return;

                        var dataSrc = getTableOfDocsAll();

                        if (dataSrc.Rows.Count == 0)
                            return;

                        try
                        {
                            var list = new List<DataRow>();

                            foreach (DataRow row in dataSrc.Rows)
                            {
                                var invWight = CASTASDOUBLE(row["WEIGHT"]);

                                if (invWight < weight + 0.01)
                                {
                                    list.Add(row);// ;
                                    weight -= invWight;
                                }
                                else
                                    continue;
                            }

                            foreach (var row in list)
                            {
                                _moveInvoice(row, getTableOfDocsPacked());

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
                void addInvoiceManual()
                {

                    try
                    {
                        filter.readFilter(this);

                        var date = filter.date.Date;

                        DataRow[] res_ = PLUGIN.REF("ref.sls.doc.inv multi::1 filter::filter_WFSTATUS," + _PLUGIN.FORMATSERIALIZE(0) + ";filter_DATE_," + _PLUGIN.FORMATSERIALIZE(date) + "," + _PLUGIN.FORMATSERIALIZE(date));
                        if (res_ != null && res_.Length > 0)
                        {
                            try
                            {


                                DataRow lastRec = null;
                                foreach (var rec in res_)
                                {
                                    var docLRef = rec["LOGICALREF"]; 
                                    DOC_VALIDATE_WEIGHT(PLUGIN, docLRef);
                                }

                                foreach (var rec in res_)
                                {
                                    var docLRef = rec["LOGICALREF"];

                                    var newRec = _addInvoiceToDocsAll(docLRef,false);
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
                void showPackInfo()
                {

                    try
                    {
                        filter.readFilter(this);
                        PLUGIN.EXECMDTEXT("event name::" + event_LOADLIST_PACK_INFO + " date::" + LEFT(FORMAT(filter.date), 10));
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
                        var tab = SELECT_INVOICE_SLS(PLUGIN, 0);
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
                        var tab = SELECT_INVOICE_SLS(PLUGIN, 0);
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
                            var weight = 0.0;
                            var count = 0;

                            foreach (DataRow r in tab.Rows)
                            {
                                count += 1;
                                weight += CASTASDOUBLE(TAB_GETROW(r, "WEIGHT"));
                            }

                            filter.val_docs_all_count = count;
                            filter.val_docs_all_weight = weight;
                        }

                        {
                            var tab = getTableOfDocsPacked();
                            var weight = 0.0;
                            var count = 0;

                            foreach (DataRow r in tab.Rows)
                            {
                                count += 1;
                                weight += CASTASDOUBLE(TAB_GETROW(r, "WEIGHT"));
                            }

                            filter.val_docs_pack_count = count;
                            filter.val_docs_pack_weight = weight;
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
                    public double maxWeight = 0;
                    public string vechile = "";
                    public string packageNr = "";
                    public string clientMarkCode = "";
                    public short docDep = 0;
                    public DateTime date;


                    public double val_docs_all_weight = 0;
                    public int val_docs_all_count = 0;

                    public double val_docs_pack_weight = 0;
                    public int val_docs_pack_count = 0;





                    public void writeFilter(Form pForm)
                    {
                        {
                            var ctrl = CONTROL_SEARCH(pForm, "val_docs_all_weight") as TextBox;
                            ctrl.Text = FORMAT(val_docs_all_weight, "#,#0.##");
                        }
                        {
                            var ctrl = CONTROL_SEARCH(pForm, "val_docs_all_count") as TextBox;
                            ctrl.Text = FORMAT(val_docs_all_count, "#,#0.##");
                        }
                        {
                            var ctrl = CONTROL_SEARCH(pForm, "val_docs_pack_weight") as TextBox;
                            ctrl.Text = FORMAT(val_docs_pack_weight, "#,#0.##");
                        }
                        {
                            var ctrl = CONTROL_SEARCH(pForm, "val_docs_pack_count") as TextBox;
                            ctrl.Text = FORMAT(val_docs_pack_count, "#,#0.##");
                        }

                        {
                            var ctrl = CONTROL_SEARCH(pForm, "filter_packagenr") as TextBox;
                            ctrl.Text = FORMAT(packageNr);
                        }

                        {
                            var ctrl = CONTROL_SEARCH(pForm, "filter_vehicle") as TextBox;
                            ctrl.Text = FORMAT(vechile);
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

                            if (asText != null)
                            {

                                switch (ctrl.Name)
                                {
                                    case "filter_packagenr":
                                        this.packageNr = asText.Text.Trim();
                                        break;
                                    case "filter_vehicle":
                                        this.vechile = asText.Text.Trim();
                                        break;
                                    case "filter_clientmarkcode":
                                        this.clientMarkCode = asText.Text.Trim();
                                        break;

                                }

                            }

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
                        return "P" + FORMAT(pPLUGIN.GETSYSPRM_USER()).PadLeft(3, '0') + FORMAT(pDate, "yyMMdd-HHmmss");
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
