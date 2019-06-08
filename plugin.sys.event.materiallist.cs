#line 2


 #region BODY
        //BEGIN

        const int VERSION = 9;
        const string FILE = "plugin.sys.event.materiallist.pls";





        #region TEXT


        const string event_MATERIALLIST_ = "hadlericom_materiallist_";
        const string event_MATERIALLIST_PRICELIST = "hadlericom_materiallist_pricelist";


        #endregion

        #region MAIN




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

                    x.MY_MATERIALLIST_IMG_DIR = s.MY_MATERIALLIST_IMG_DIR;
                    x.MY_MATERIALLIST_USER = s.MY_MATERIALLIST_USER;
                    x.MY_MATERIALLIST_IMG_SIZE = s.MY_MATERIALLIST_IMG_SIZE;

                    x.GETSYSPRM_USER = PLUGIN.GETSYSPRM_USER();


                    _SETTINGS.BUF = x;

                }

                public string MY_MATERIALLIST_IMG_DIR;
                public int MY_MATERIALLIST_IMG_SIZE;
                public string MY_MATERIALLIST_USER;


                public short GETSYSPRM_USER;
            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC)
            {

            }


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active On User")]
            public string MY_MATERIALLIST_USER
            {
                get
                {
                    return (_GET("MY_MATERIALLIST_USER", ""));
                }
                set
                {
                    _SET("MY_MATERIALLIST_USER", value);
                }

            }



            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Material Image Path ($CODE$)")]
            public string MY_MATERIALLIST_IMG_DIR
            {
                get
                {
                    return (_GET("MY_MATERIALLIST_IMG_DIR", ""));
                }
                set
                {
                    _SET("MY_MATERIALLIST_IMG_DIR", value);
                }

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Material Image Size (mm)")]
            public int MY_MATERIALLIST_IMG_SIZE
            {
                get
                {
                    return PARSEINT(_GET("MY_MATERIALLIST_IMG_SIZE", "30"));
                }
                set
                {
                    _SET("MY_MATERIALLIST_IMG_SIZE", value);
                }

            }
            //

            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return BUF.MY_MATERIALLIST_USER == ""
                || Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_MATERIALLIST_USER),
                     FORMAT(BUF.GETSYSPRM_USER)
                     ) >= 0;
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
                    MY_SYS_NEWFORM_INTEGRATE(arg1 as Form);
                    break;
                case SysEvent.SYS_USEREVENT:
                    MY_SYS_USEREVENT_HANDLER(EVENTCODE, ARGS);
                    break;

            }



        }



        void MY_SYS_NEWFORM_INTEGRATE(Form FORM)
        {
            if (FORM == null)
                return;

            var fn = GETFORMNAME(FORM);

            if (fn == null)
                return;

            var isList = fn.StartsWith("ref.mm.rec.mat");


            if (isList)
            {

                var cPanelBtnSub = CONTROL_SEARCH(FORM, "cPanelBtnSub");

                if (cPanelBtnSub == null)
                    return;


                if (isList)
                {

                    _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_MATERIALLIST_PRICELIST, LANG("T_PRICELIST"), "invoice_16x16");

                }
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

                string[] list_ = EXPLODELISTPATH(EVENTCODE);
                var cmd = list_.Length > 1 ? list_[1].ToLowerInvariant() : "";

                switch (cmd)
                {


                    case event_MATERIALLIST_PRICELIST:
                        {
                            MY_REP_HTML(cmd);
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


        void MY_REP_HTML(string pCmd)
        {


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
            table, h1, h2, h3, h4, h5, h6, div {
                font-family: Segoe UI, Verdana, Aral;
            }
            th {
                font-weight:bold;
                text-align: center;
            }

</style>
 <body style=''>
");


            switch (pCmd)
            {
                case event_MATERIALLIST_PRICELIST:
                    #region PRICELIST
                    {

                        var list = new List<string>();

                        list.Add("ONHAND");
                        list.Add("T_ONHAND");

                        list.Add("NEGATIVE_LEVEL");
                        list.Add("T_NEGATIVE_LEVEL");

                        list.Add("ALL");
                        list.Add("T_ALL");

                        list.Add("CODE");
                        list.Add("T_CODE");

                        list.Add("LISTPRICE");
                        list.Add("T_PRICE (T_LIST)");

                        list.Add("DOCPRICE");
                        list.Add("T_PRICE (T_DOC)");

                        list.Add("SPECODE");
                        list.Add("T_SPECODE");

                        if (_SETTINGS.BUF.MY_MATERIALLIST_IMG_DIR != "")
                        {
                            list.Add("IMAGE");
                            list.Add("T_IMAGE");
                        }

                        var res_ = REF(
                            "ref.gen.definedlist multi::1 checkbox::1" + " " +
                            "[obj::" + JOINLIST(list.ToArray()) + "]" + " " +
                            "[desc::T_PRICELIST]" + " " +
                            "[filter::filter_VALUE," + FORMATSERIALIZE("ONHAND") + ";filter_VALUE," + FORMATSERIALIZE("LISTPRICE") + "]" + " " +
                            "type::string"
                            );

                        if (res_ == null || res_.Length == 0)
                            return;


                        var flag_ALL = false;
                        var flag_ONHAND = false;
                        var flag_NEGATIVE_LEVEL = false;
                        var flag_CODE = false;
                        var flag_DOCPRICE = false;
                        var flag_LISTPRICE = false;
                        var flag_SPECODE = false;
                        var flag_IMAGE = false;

                        foreach (var rec in res_)
                        {
                            var code_ = CASTASSTRING(TAB_GETROW(rec, "VALUE"));
                            switch (code_ as string)
                            {

                                case "ALL":
                                    flag_ALL = true;
                                    break;
                                case "ONHAND":
                                    flag_ONHAND = true;
                                    break;
                                case "NEGATIVE_LEVEL":
                                    flag_NEGATIVE_LEVEL = true;
                                    break;
                                //
                                case "CODE":
                                    flag_CODE = true;
                                    break;
                                case "DOCPRICE":
                                    flag_DOCPRICE = true;
                                    break;
                                case "LISTPRICE":
                                    flag_LISTPRICE = true;
                                    break;
                                case "SPECODE":
                                    flag_SPECODE = true;
                                    break;
                                case "IMAGE":
                                    flag_IMAGE = true;
                                    break;

                            }

                        }


                        var sqlText =
@"


select 
ITEMS.CODE,
ITEMS.NAME,
ITEMS.SPECODE,
ITEMS.SPECODE2,

(SELECT 
--$MS$--TOP(1) 
PRICE FROM LG_$FIRM$_PRCLIST PRCLIST WITH(NOLOCK) WHERE PRCLIST.CARDREF = ITEMS.LOGICALREF AND PRCLIST.PTYPE = 2 
--$PG$--LIMIT 1
) PRICE_SALE,



	(
		SELECT 
--$MS$--TOP(1) 
(case when AMOUNT > 0 then (VATMATRAH+VATAMNT+DISTEXP)/AMOUNT else 0 end) PRICE
		FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK) 
		WHERE (
				STOCKREF = ITEMS.LOGICALREF AND  
				VARIANTREF >= 0 AND DATE_ >= '19000101' AND FTIME >= 0 AND IOCODE = 1 AND  
				SOURCEINDEX = 0
				) AND (
				CANCELLED = 0 AND TRCODE = 1  
				)
		ORDER BY STOCKREF DESC,
			VARIANTREF DESC,
			DATE_ DESC,
			FTIME DESC,
			IOCODE DESC,
			SOURCEINDEX DESC,
			LOGICALREF DESC
--$PG$--LIMIT 1
		) PRICE_LAST_PRCH,
	(
		SELECT 
--$MS$--TOP(1) 
(case when AMOUNT > 0 then (VATMATRAH+VATAMNT+DISTEXP)/AMOUNT else 0 end) PRICE
		FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK)
		WHERE (
				STOCKREF = ITEMS.LOGICALREF AND  
				VARIANTREF >= 0 AND DATE_ >= '19000101' AND FTIME >= 0 AND IOCODE = 4 AND  
				SOURCEINDEX = 0
				) AND (
				CANCELLED = 0 AND TRCODE IN (7,8) 
				)
		ORDER BY STOCKREF DESC,
			VARIANTREF DESC,
			DATE_ DESC,
			FTIME DESC,
			IOCODE DESC,
			SOURCEINDEX DESC,
			LOGICALREF DESC 
--$PG$--LIMIT 1
		) PRICE_LAST_SLS,


GNTOTST.ONHAND ONHAND_ALL,

(SELECT 
--$MS$--TOP(1) 
CODE FROM LG_$FIRM$_UNITSETL UNITSETL WITH(NOLOCK) WHERE UNITSETL.UNITSETREF = ITEMS.UNITSETREF AND UNITSETL.MAINUNIT=1
--$PG$--LIMIT 1
) UNIT 

 
from LG_$FIRM$_ITEMS ITEMS WITH(NOLOCK) 
left join 
LG_$FIRM$_$PERIOD$_GNTOTST GNTOTST WITH(NOLOCK) 
on ITEMS.LOGICALREF = GNTOTST.STOCKREF AND GNTOTST.INVENNO = -1

$WHERE$

ORDER BY ITEMS.NAME ASC


  "
;

                        if (flag_ONHAND)
                        {
                            sqlText = sqlText.Replace("$WHERE$", "where GNTOTST.ONHAND > 0.001");

                        }
                        else
                            if (flag_NEGATIVE_LEVEL)
                            {

                                sqlText = sqlText.Replace("$WHERE$", "where GNTOTST.ONHAND < -0.001");
                            }
                            else
                                if (flag_ALL)
                                {

                                    sqlText = sqlText.Replace("$WHERE$", "");
                                }
                                else
                                {
                                    return;
                                }


                        var data = (SQL(
sqlText
, new object[] { }));

                        TAB_FILLNULL(data);

                        // var img = GETRESOURCE("firm_logo.png");

                        //name
                        res.AppendLine(string.Format("<h3 style='width:100%;text-align:center; '>{0}</br>{1}</h3>",
                           HTMLENCODE(LANG("T_PRICELIST (T_SYS_CURR1)")), HTMLENCODE(GETSYSPRM_FIRMNAME())
                              ));


                        res.AppendLine(string.Format("<h4 style='width:100%;text-align:center; '>{0}</h4>",
                           HTMLENCODE(LEFT(FORMAT(DateTime.Now), 10))
                              ));
                        //filter



                        res.AppendLine("<br/>");

                        //body
                        {
                            res.AppendLine("<table style='widht:auto'>");

                            res.AppendLine("<tr>");
                            foreach (var cell in new string[] { 
                                (""),

                               (flag_IMAGE ?  (""): null),

                                (flag_CODE ? LANG("T_CODE"): null),
                                LANG("T_NAME"), 

                               (flag_LISTPRICE?   LANG("T_PRICE (T_SYS_CURR1)<br/>(T_LIST)"): null),

                                (flag_DOCPRICE?  LANG("T_PRICE (T_SYS_CURR1)<br/>(T_PURCHASE)"): null),
                                (flag_DOCPRICE?  LANG("T_PRICE (T_SYS_CURR1)<br/>(T_SALE)"): null),
                                
                                LANG("T_ONHAND"),
                                LANG("T_UNIT"),

                                (flag_SPECODE ? LANG("T_SPECODE"): null),
                                (flag_SPECODE ? LANG("T_SPECODE (2)"): null),


                            })
                            {
                                if (cell != null)
                                {
                                    res.AppendLine(string.Format(
                                    "<th>{0}</th>",
                                     cell
                                   ));
                                }
                            }
                            res.AppendLine("</tr>");



                            //

                            for (int i = 0; i < data.Rows.Count; ++i)
                            {
                                var indx = i + 1;
                                //

                                var isDark = (i % 2 == 1);
                                var isBold = false;



                                //
                                var row = data.Rows[i];
                                //

                                string path = _SETTINGS.BUF.MY_MATERIALLIST_IMG_DIR.Replace("$CODE$", CASTASSTRING(TAB_GETROW(row, "CODE")));
                                string imageData = "";
                                if (System.IO.File.Exists(path))
                                {
                                  imageData =  TOBASE64STR(System.IO.File.ReadAllBytes(path));

                                }
                             


                                string[] arrCell = new string[] { 
                                        FORMAT(indx) ,

                                         (flag_IMAGE ?
                                        string.Format(
                                        ("<img style='height:{0}mm;width:{0}mm;' src='data:image/jpeg;base64,{1}' />"),
                                         _SETTINGS.BUF.MY_MATERIALLIST_IMG_SIZE,
                                        imageData                                       
                                        )
                                         : null),

                                         (flag_CODE ?  CASTASSTRING(TAB_GETROW(row, "CODE")): null),

                                        CASTASSTRING(TAB_GETROW(row, "NAME")),

                                       (flag_LISTPRICE ?   FORMAT(ROUND(CASTASDOUBLE(TAB_GETROW(row, "PRICE_SALE")), 2),"N2"): null),

                                         (flag_DOCPRICE ?  FORMAT(ROUND(CASTASDOUBLE(TAB_GETROW(row, "PRICE_LAST_PRCH")), 2),"N2"): null),
                                        (flag_DOCPRICE ? FORMAT(ROUND(CASTASDOUBLE(TAB_GETROW(row, "PRICE_LAST_SLS")), 2),"N2"): null),
                                        
                                         
                                        FORMAT(ROUND(CASTASDOUBLE(TAB_GETROW(row, "ONHAND_ALL")), 2),"#,##0.#") ,
                                        CASTASSTRING(TAB_GETROW(row, "UNIT")),
                                        
                                       (flag_SPECODE ?  CASTASSTRING(TAB_GETROW(row, "SPECODE")): null),
                                       (flag_SPECODE ?  CASTASSTRING(TAB_GETROW(row, "SPECODE2")): null),
                                    };

                                //string[] arrCellWidth = new string[] { 
                                //        "50px" ,
                                //        "360px",
                                //        "100px",
                                //        "100px",
                                //        "50px",

                                //    };









                                var backColor = "#FFFFFF";

                                //if (indx % 2 == 1)
                                //    backColor = "#F2F2F2";

                                if (isDark)
                                    backColor = "#F0F0F0";


                                var fontWeight = isBold ? "bold" : "normal";

                                res.AppendLine("<tr style='background-color:" + backColor + ";font-weight:" + fontWeight + "'>");
                                // foreach (var cellVal in arrCell)
                                for (int c = 0; c < arrCell.Length; ++c)
                                {
                                    var cellVal = arrCell[c];
                                    if (cellVal != null)
                                    {
                                        var width = "auto"; //arrCellWidth[c]
                                        res.AppendLine(string.Format(
                                            "<td style='width:" + (width) + ";text-align: " + (c >= 4 ? "right" : "") + ";'>{0}</td>",
                                         cellVal
                                       ));
                                    }
                                }
                                res.AppendLine("</tr>");




                            }

                            res.AppendLine("</table>");
                        }
                    }
                    #endregion
                    break;


            }

            res.AppendLine("</body></html>");

            //  MSGUSERINFO(res.ToString());

            var priceLisPath = PATHCOMBINE(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                GETSYSPRM_FIRMNAME() + " " + LANG("T_PRICELIST") + ".html");

            FILEWRITE(priceLisPath, res.ToString());

            PROCESS(priceLisPath, "");


        }



        //END



        #region CLAZZ

        class TEXT
        {

            public const string text_DESC = "Material List";

            static TEXT _L = null;

            public static TEXT L
            {
                get
                {
                    if (_L == null)
                    {
                        _L = new TEXT();


                    }

                    return _L;
                }
            }


            public TEXT()
            {

                lang_en();

                var m = this.GetType().GetMethod("lang_" + LANG_ACTIVE());
                if (m != null)
                    m.Invoke(this, null);

            }



            public void lang_tr()
            {

                A = "A";

            }


            public void lang_en()
            {

                A = "A";


            }

            public void lang_az()
            {

                A = "A";



            }



            public void lang_ru()
            {

                A = "A";


            }

            public string A;



        }





        #endregion


        #region TOOLS
        public static string MY_CHOOSE_SQL(string pSqlMs, string pSqlPg)
        {

            if (ISMSSQL())
                return pSqlMs;

            if (ISPOSTGRESQL())
                return pSqlPg;


            throw new Exception("Undefined datasource");
        }

        static bool MY_ASKDATETIME(_PLUGIN pPLUGIN, string pMsg, ref DateTime pDf, ref DateTime pDt)
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
        #endregion

        #endregion
        #endregion