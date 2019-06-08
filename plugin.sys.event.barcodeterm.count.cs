#line 2



         #region PLUGIN_BODY
        const int VERSION = 11;

        const string FILE = "plugin.sys.event.barcodeterm.count.pls";




        #region SETTINGS

        class _SETTINGS : TOOL_SETTINGS
        {

            public static _BUF BUF = null;
            public class _BUF
            {
                public static void LOAD_SETTINGS(_PLUGIN PLUGIN)
                {
                    if (_SETTINGS.BUF == null)
                    {

                        var x = new _SETTINGS._BUF();

                        var s = new _SETTINGS(PLUGIN);

                        x.MY_BARCODETERM_COUNT_USER = s.MY_BARCODETERM_COUNT_USER;
                        x.MY_BARCODETERM_COUNT_DEF_WH = s.MY_BARCODETERM_COUNT_DEF_WH;
                        x.MY_BARCODETERM_COUNT_SIMPLE_MODE = s.MY_BARCODETERM_COUNT_SIMPLE_MODE;




                        x._USER = PLUGIN.GETSYSPRM_USER();
                        x._FIRM = PLUGIN.GETSYSPRM_FIRM();
                        x._FIRMNAME = PLUGIN.GETSYSPRM_FIRMNAME();
                        x._PERIOD = PLUGIN.GETSYSPRM_PERIOD();
                        //


                        _SETTINGS.BUF = x;

                        //
                        var arr = new List<string>(EXPLODELIST(x.MY_BARCODETERM_COUNT_USER.Trim()));
                        x._ISUSEROK = x._USER == 1 || (arr.Count == 0 || arr.Contains(FORMAT(x._USER)));
                        //

                    }

                }

                public string MY_BARCODETERM_COUNT_USER;
                public int MY_BARCODETERM_COUNT_DEF_WH;
                public bool MY_BARCODETERM_COUNT_SIMPLE_MODE;

                //

                //

                public bool _ISUSEROK;
                public short _FIRM;
                public string _FIRMNAME;
                public short _PERIOD;
                public short _USER;



            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC)
            {

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active If User (Nr List 1,2,3 or empty)")]
            public string MY_BARCODETERM_COUNT_USER
            {
                get
                {
                    return (_GET("MY_BARCODETERM_COUNT_USER", ""));
                }
                set
                {
                    _SET("MY_BARCODETERM_COUNT_USER", value);
                }

            }



            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Default Warehouse (-2 if select)")]
            public int MY_BARCODETERM_COUNT_DEF_WH
            {
                get
                {
                    var z = (int)MIN(999, MAX(-2, PARSEINT(_GET("MY_BARCODETERM_COUNT_DEF_WH", "-2"))));

                    if (z == -1)
                        z = -2;

                    return z;
                }
                set
                {
                    _SET("MY_BARCODETERM_COUNT_DEF_WH", value);
                }

            }


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Simple Mode")]
            public bool MY_BARCODETERM_COUNT_SIMPLE_MODE
            {
                get
                {
                    return (_GET("MY_BARCODETERM_COUNT_SIMPLE_MODE", "1")) == "1";
                }
                set
                {
                    _SET("MY_BARCODETERM_COUNT_SIMPLE_MODE", value);
                }

            }




        }

        #endregion

        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "POS Counting";


            public class L
            {

            }
        }

        const string event_BARCODETERM_ = "_barcodeterm_";
        const string event_BARCODETERM_COUNT_ = "_barcodeterm_counting_";
        const string event_BARCODETERM_COUNT_COUNTING_SINGLE = "_barcodeterm_counting_single";
        const string event_BARCODETERM_COUNT_COUNTING_FILE = "_barcodeterm_counting_file";
        const string event_BARCODETERM_COUNT_COUNTING_IMPORT = "_barcodeterm_counting_import";
        const string event_BARCODETERM_COUNT_COUNTING_CONVERT = "_barcodeterm_counting_convert";

        #endregion

        #region MAIN

        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {

            if (ISWEB())
                return;

            object arg0 = ARGS.Length > 0 ? ARGS[0] : null;
            object arg1 = ARGS.Length > 1 ? ARGS[1] : null;
            object arg2 = ARGS.Length > 2 ? ARGS[2] : null;

            string[] list_ = EXPLODELISTPATH(EVENTCODE);

            var cmdType = list_.Length > 0 ? list_[0] : "";
            var cmdExt = list_.Length > 1 ? list_[1] : "";

            _SETTINGS._BUF.LOAD_SETTINGS(this);

            if (!_SETTINGS.BUF._ISUSEROK)
                return;

            switch (cmdType)
            {
                case SysEvent.SYS_PLUGINSETTINGS:
                    (arg0 as List<object>).Add(new _SETTINGS(this));
                    break;
                case SysEvent.SYS_LOGIN:
                    {

                    }
                    break;
                case SysEvent.SYS_USEREVENT:
                    MY_SYS_USEREVENT_HANDLER(EVENTCODE, ARGS);
                    break;
                case SysEvent.SYS_NEWFORM:
                    MY_SYS_NEWFORM_INTEGRATE(arg0 as Form);
                    break;


            }



        }


        void MY_SYS_NEWFORM_INTEGRATE(Form FORM)
        {


            var fn = GETFORMNAME(FORM);



            if (fn == "form.app")
            {
                var tree = CONTROL_SEARCH(FORM, "cTreeTools");
                if (tree != null)
                {

                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
			{ "Text" ,"POS"},
			{ "ImageName" ,"barcode_32x32"},
			{ "Name" ,event_BARCODETERM_},
            };

                        RUNUIINTEGRATION(tree, args);

                    }

                    // if (_SETTINGS.BUF.MY_BARCODETERM_COUNT_SIMPLE_MODE)
                    {
                        {
                            var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_BARCODETERM_},
			{ "CmdText" ,"event name::"+event_BARCODETERM_COUNT_COUNTING_SINGLE },
			{ "Text" ,"T_COUNTING (T_SINGLE)"},
			{ "ImageName" ,"pin_32x32"},
		    { "Name" ,event_BARCODETERM_COUNT_COUNTING_SINGLE},
            };

                            RUNUIINTEGRATION(tree, args);

                        }

                    }

                    if (!_SETTINGS.BUF.MY_BARCODETERM_COUNT_SIMPLE_MODE)
                    {

                        {
                            var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_BARCODETERM_},
			{ "CmdText" ,"event name::"+event_BARCODETERM_COUNT_COUNTING_FILE },
			{ "Text" ,"T_COUNTING (T_FILE)"},
			{ "ImageName" ,"pin_32x32"},
			{ "Name" ,event_BARCODETERM_COUNT_COUNTING_FILE},
            };

                            RUNUIINTEGRATION(tree, args);

                        }



                        {
                            var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_BARCODETERM_},
			{ "CmdText" ,"event name::"+event_BARCODETERM_COUNT_COUNTING_IMPORT },
			{ "Text" ,"T_COUNTING (T_IMPORT)"},
			{ "ImageName" ,"import_32x32"},
		    { "Name" ,event_BARCODETERM_COUNT_COUNTING_IMPORT},
            };

                            RUNUIINTEGRATION(tree, args);

                        }


                        {
                            var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_BARCODETERM_},
			{ "CmdText" ,"event name::"+event_BARCODETERM_COUNT_COUNTING_CONVERT },
			{ "Text" ,"T_COUNTING (T_CONVERT)"},
			{ "ImageName" ,"lamp_32x32"},
			 { "Name" ,event_BARCODETERM_COUNT_COUNTING_CONVERT},
            };

                            RUNUIINTEGRATION(tree, args);

                        }

                    }
                }
                return;

            }
            var cPanelBtnSub = CONTROL_SEARCH(FORM, "cPanelBtnSub");




            if (cPanelBtnSub == null)
                return;


            if (fn.StartsWith("ref.mm.rec.mat"))
            {




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
                if (cmd.StartsWith(event_BARCODETERM_))
                    switch (cmd)
                    {
                        case event_BARCODETERM_COUNT_COUNTING_SINGLE:
                        case event_BARCODETERM_COUNT_COUNTING_FILE:
                            {
                                var dic = new Dictionary<string, string>();

                                var fileMode = cmd.EndsWith("_file");

                                dic["type"] = FORMAT((int)8);
                                //dic["UI_SCALE"] = "20";
                                dic["NEGATIVE_LIGHT"] = "1";
                                if (fileMode)
                                {
                                    dic["SAVE_TO_FILE"] = "1";
                                    // dic["DOC_PREFIX"] = "Z";

                                }
                                //
                                dic["wh"] = FORMAT(_SETTINGS.BUF.MY_BARCODETERM_COUNT_DEF_WH); // "-2";

                                SYSUSEREVENT("_barcodeterm_", new object[]{
                         dic
                            });
                            }
                            break;

                        case event_BARCODETERM_COUNT_COUNTING_IMPORT:

                            MY_BARCODETERM_COUNT_COUNTING_IMPORT();

                            break;
                        case event_BARCODETERM_COUNT_COUNTING_CONVERT:

                            MY_BARCODETERM_COUNT_COUNTING_CONVERT();

                            break;

                    }


            }
            catch (Exception exc)
            {
                LOG(exc);
                MSGUSERERROR(exc.Message);
            }
        }

        #endregion


        #region HANDLER


        void MY_BARCODETERM_COUNT_COUNTING_IMPORT()
        {


            if (!MSGUSERASK("T_MSG_COMMIT_BEGIN - T_COUNTING (T_IMPORT)"))
                return;


            var dateTmp_ = MY_TOOL.MY_ASK_DATE(this, "T_DATE");

            if (dateTmp_ == null)
                return;

            var date_ = dateTmp_.Value.Date;


            var files = MY_DIR.GETFILES(MY_DIR.PRM_DIR_COUNTING, "*.counting.xml");


            var listWh = new List<short>();
            var listHeaders = new List<Dictionary<string, object>>();
            var listLines = new List<List<Dictionary<string, object>>>();


            foreach (var file in files)
            {
                try
                {

                    var header = new Dictionary<string, object>();
                    var lines = new List<Dictionary<string, object>>();

                    MY_TOOL.LOAD_COUNTING(file, header, lines);


                    listWh.Add(CASTASSHORT(header["SOURCEINDEX"]));
                    listHeaders.Add(header);
                    listLines.Add(lines);

                    //C01171130222408.112.counting.xml
                    header["FICHENO"] = LEFT(Path.GetFileName(file), 19);

                }
                catch (Exception exc)
                {
                    throw new Exception("Error on file: " + file, exc);
                }

            }

            INVOKEINBATCH(() =>
            {


                for (int i = 0; i < listWh.Count; ++i)
                {

                    var wh = listWh[i];
                    var header = listHeaders[i];
                    var line = listLines[i];



                    ////

                    header["CANCELLED"] = (short)1;///!!!
                    header["TRCODE"] = (short)50;
                    header["DATE_"] = date_;



                    header["FICHENO"] = header["FICHENO"];
                    header["SPECODE"] = "COUNTING";

                    new MY_SAVESLIP(this, header, line, true);


                }

            });




            MSGUSERINFO("T_MSG_OPERATION_OK - T_COUNTING (T_IMPORT)");


        }
        void MY_BARCODETERM_COUNT_COUNTING_CONVERT()
        {


            if (!MSGUSERASK("T_MSG_COMMIT_BEGIN - T_COUNTING (T_CONVERT)"))
                return;
 
            DateTime df=DateTime.Now.Date;
            DateTime dt = df;


            if (!MY_TOOL.MY_ASK_DATE(this, "T_DATE", ref df, ref dt))
                return;





            var stockWh_ = SQL(@"

select 
distinct F.SOURCEINDEX
from 
LG_001_01_STFICHE F where F.TRCODE in (50) and F.CANCELLED = 1 and F.DATE_ BETWEEN @P1 AND @P2

", new object[] { df,dt });




            TAB_FILLNULL(stockWh_);


            var listWh = new List<short>();
            var listHeaders = new List<Dictionary<string, object>>();
            var listLines = new List<List<Dictionary<string, object>>>();

            foreach (DataRow rowWh in stockWh_.Rows)
            {
                var wh_ = CASTASSHORT(TAB_GETROW(rowWh, "SOURCEINDEX"));


                var stockData_ = SQL(@"

select TMP.* from
(
    select 
    L.STOCKREF,
    SUM(L.AMOUNT) AMOUNT,
    SUM(L.TOTAL) TOTAL ,
    (select sum(T.ONHAND) FROM LG_001_01_STINVTOT T where DATE_ < @P1 and T.INVENNO = @P3 and T.STOCKREF = L.STOCKREF) ONHAND
    from 
    LG_001_01_STLINE L 
    where L.TRCODE in (50) and L.CANCELLED = 1 and L.DATE_ BETWEEN @P1 AND @P2 and L.SOURCEINDEX = @P3 and L.LINETYPE in (0)
    group by L.STOCKREF
) TMP
inner join 
LG_001_ITEMS I on TMP.STOCKREF = I.LOGICALREF
order by I.NAME asc

", new object[] { df, dt, wh_ });




                foreach (var trcode_ in new short[] { 50, 51 })
                {
                    var fisheNo = "C" + FORMAT(df, "yyMMdd") + "." + FORMAT(wh_).PadLeft(3, '0');
                    if (!ISNULL(SQLSCALAR("SELECT '1' FROM LG_001_01_STFICHE WHERE FICHENO = @P1 AND TRCODE = @P2",
                        new object[] { fisheNo, trcode_ })))
                        continue;


                    var header = new Dictionary<string, object>();
                    var lines = new List<Dictionary<string, object>>();


                    header["FICHENO"] = fisheNo;
                    header["DATE_"] = df;
                    header["TRCODE"] = trcode_;
                    header["SOURCEINDEX"] = wh_;


                    foreach (DataRow rowStock_ in stockData_.Rows)
                    {
                        var newLine = new Dictionary<string, object>();


                        var amnt = CASTASDOUBLE(TAB_GETROW(rowStock_, "AMOUNT"));
                        var onhand = CASTASDOUBLE(TAB_GETROW(rowStock_, "ONHAND"));
                        var total = CASTASDOUBLE(TAB_GETROW(rowStock_, "TOTAL"));
                        double price = ISNUMZERO(amnt) ? 0.0 : total / amnt;
                        var stockRef = TAB_GETROW(rowStock_, "STOCKREF");

                        var diff_ = amnt - onhand;

                        if (ISNUMZERO(diff_))
                            continue;

                        if (diff_ > 0 && trcode_ != 50)
                            continue;

                        if (diff_ < 0 && trcode_ != 51)
                            continue;



                        newLine["STOCKREF"] = stockRef;
                        newLine["AMOUNT"] = ABS(diff_);
                        newLine["PRICE"] = price;

                        

                        lines.Add(newLine);
                    }

                    if (lines.Count > 0)
                    {
                        listWh.Add(wh_);
                        listHeaders.Add(header);
                        listLines.Add(lines);
                    }
                }








            }


            INVOKEINBATCH(() =>
            {


                for (int i = 0; i < listWh.Count; ++i)
                {

                    var wh = listWh[i];
                    var header = listHeaders[i];
                    var line = listLines[i];

                    new MY_SAVESLIP(this, header, line, true);


                }

            });



            MSGUSERINFO("T_MSG_OPERATION_OK - T_COUNTING (T_CONVERT)");

        }

        #endregion


        #region CLASS


        class MY_DIR
        {

            public static string PRM_DIR_ROOT = PATHCOMBINE(
                 Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "POS");

            public static string PRM_DIR_COUNTING = PATHCOMBINE(PRM_DIR_ROOT, "COUNT");


            public static string[] GETFILES(string pDir, string pFilter)
            {

                if (!Directory.Exists(pDir))
                    Directory.CreateDirectory(pDir);

                return Directory.GetFiles(pDir, pFilter);

            }


        }


        class MY_TOOL
        {




            public static void LOAD_COUNTING(string pFileFullPath,
                Dictionary<string, object> pHeader,
                List<Dictionary<string, object>> pLines)
            {







                var xmlDoc = new System.Xml.XmlDocument();

                var dataXml = FILEREADTEXT(pFileFullPath);

                xmlDoc.LoadXml(dataXml);

                var root = xmlDoc["DOCS"];

                if (root == null)
                    throw new NullReferenceException("Xml doc hast root node");

                var header = root.FirstChild;

                if (header == null)
                    throw new NullReferenceException("Xml doc hast doc header");
                var lines = header.ChildNodes;

                // if (lines == null || lines.Count ==0) 
                //    throw new NullReferenceException("Xml doc hast doc lines");

                //var header = xmlDoc.SelectSingleNode("DOCS/STFICHE");
                //var lines = header.ChildNodes;




                pHeader["SOURCEINDEX"] = PARSESHORT(XMLNODEATTR(header, "SOURCEINDEX"));
                pHeader["GENEXP1"] = (XMLNODEATTR(header, "GENEXP1"));
                pHeader["TRCODE"] = PARSESHORT(XMLNODEATTR(header, "TRCODE"));




                foreach (XmlNode lineNode in lines)
                {

                    var line = new Dictionary<string, object>();

                    line["ITEMS_____CODE"] = (XMLNODEATTR(lineNode, "ITEMS_____CODE"));
                    line["AMOUNT"] = PARSEDOUBLE(XMLNODEATTR(lineNode, "AMOUNT"));
                    line["PRICE"] = PARSEDOUBLE(XMLNODEATTR(lineNode, "PRICE"));

                    pLines.Add(line);


                }






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



        class MY_SAVESLIP : IDisposable
        {

            _PLUGIN PLUGIN;

            Dictionary<string, object> HEADER;
            List<Dictionary<string, object>> LINES;




            public MY_SAVESLIP(_PLUGIN pPLUGIN, Dictionary<string, object> pHEADER, List<Dictionary<string, object>> pLINES, bool pGlobalBatch = false)
            {


                const int VERSION = 2;

                PLUGIN = pPLUGIN;
                HEADER = pHEADER;
                LINES = pLINES;


                var trcode = HEADER.ContainsKey("TRCODE") ? CASTASSHORT(HEADER["TRCODE"]) : 0;
                var cancelled = HEADER.ContainsKey("CANCELLED") ? CASTASSHORT(HEADER["CANCELLED"]) : 0;

                string adp_ = "";
                switch (trcode)
                {
                    case 50:
                        adp_ = "adp.mm.doc.slip.50";
                        break;
                    case 51:
                        adp_ = "adp.mm.doc.slip.51";
                        break;
                    case 11:
                        adp_ = "adp.mm.doc.slip.11";
                        break;
                    case 12:
                        adp_ = "adp.mm.doc.slip.12";
                        break;
                    default:
                        throw new Exception("Undefined document trcode [" + trcode + "]");
                }

                if (cancelled == 1)
                    adp_ = adp_ + " cancel::1";

                PLUGIN.EXEADPCMD(new string[] { adp_ + " cmd::add" }, new DoWorkEventHandler[] { DONE }, pGlobalBatch);//in global batch
            }

            void DONE(object sender, System.ComponentModel.DoWorkEventArgs e)
            {

                e.Result = false;


                DataSet doc_ = ((DataSet)e.Argument);


                DataTable tabHeaderSlip_ = TAB_GETTAB(doc_, "STFICHE");
                DataTable tabLine_ = TAB_GETTAB(doc_, "STLINE");
                //////////////////////////////////////////////////////////////////////////////

                //////////////////////////////////////////////////////////////////////////////


                foreach (var key in HEADER.Keys)
                    TAB_SETROW(tabHeaderSlip_, key, HEADER[key]);

                foreach (var rec in LINES)
                {
                    var ROW = TAB_ADDROW(tabLine_);


                    foreach (var key in rec.Keys)
                    {

                        var val = rec[key];
                        TAB_SETROW(ROW, key, rec[key]);


                        switch (key)
                        {
                            case "ITEMS_____CODE":

                                if (ISEMPTYLREF(TAB_GETROW(ROW, "STOCKREF")))
                                    throw new Exception("Cant find material by code: " + FORMAT(val));

                                break;

                        }

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
