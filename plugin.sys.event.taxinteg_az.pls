#line 2

     #region BODY
        //BEGIN

        public const int VERSION = 25;
        public const string FILE = "plugin.sys.event.taxinteg_az.pls";



        const string event_TAXINTEGAZ_ = "_taxintegaz_";
        const string event_TAXINTEGAZ_INVOICE_ALL_SALES = "_taxintegaz_inv_all_sales";
        const string event_TAXINTEGAZ_INVOICE_SINGLE = "_taxintegaz_inv_single";

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

                    x.MY_TAXINTEGAZ_USER = s.MY_TAXINTEGAZ_USER;
                    x.MY_TAXINTEGAZ_MY_TAX_NR = s.MY_TAXINTEGAZ_MY_TAX_NR;
                    x.MY_TAXINTEGAZ_OVERWRITE_VAT = s.MY_TAXINTEGAZ_OVERWRITE_VAT;
                    x.MY_TAXINTEGAZ_MAT_TITLE_SRC = s.MY_TAXINTEGAZ_MAT_TITLE_SRC;
                    x.MY_TAXINTEGAZ_USE_REP_CURR = s.MY_TAXINTEGAZ_USE_REP_CURR;
                    x.MY_TAXINTEGAZ_CLIENT_CODE_PREFIX = s.MY_TAXINTEGAZ_CLIENT_CODE_PREFIX;
                    //   x.MY_TAXINTEGAZ_EACH_INV_DIFF_ZIP = s.MY_TAXINTEGAZ_EACH_INV_DIFF_ZIP;
                    //x.MY_TAXINTEGAZ_DO_ZIP = s.MY_TAXINTEGAZ_DO_ZIP;
                    _SETTINGS.BUF = x;

                }

                public string MY_TAXINTEGAZ_USER;
                public string MY_TAXINTEGAZ_MY_TAX_NR;
                public string MY_TAXINTEGAZ_CLIENT_CODE_PREFIX;
                public double MY_TAXINTEGAZ_OVERWRITE_VAT;
                public bool MY_TAXINTEGAZ_USE_REP_CURR;
                //  public bool MY_TAXINTEGAZ_EACH_INV_DIFF_ZIP;
                //   public bool MY_TAXINTEGAZ_DO_ZIP;

                public TaxInvMatTitleSource MY_TAXINTEGAZ_MAT_TITLE_SRC;

            }

            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC)
            {

            }


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active On User")]
            public string MY_TAXINTEGAZ_USER
            {
                get
                {
                    return (_GET("MY_TAXINTEGAZ_USER", "1,2"));
                }
                set
                {
                    _SET("MY_TAXINTEGAZ_USER", value);
                }

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("My Tax Nr")]
            public string MY_TAXINTEGAZ_MY_TAX_NR
            {
                get
                {
                    return (_GET("MY_TAXINTEGAZ_MY_TAX_NR", ""));
                }
                set
                {
                    _SET("MY_TAXINTEGAZ_MY_TAX_NR", value);
                }
            }


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Title Source Columns")]
            public TaxInvMatTitleSource MY_TAXINTEGAZ_MAT_TITLE_SRC
            {
                get
                {
                    return (TaxInvMatTitleSource)CASTASINT(_GET("MY_TAXINTEGAZ_MAT_TITLE_SRC", Convert.ToInt32(TaxInvMatTitleSource.Name).ToString()));
                }
                set
                {
                    _SET("MY_TAXINTEGAZ_MAT_TITLE_SRC", Convert.ToInt32(value));
                }

            }

            public enum TaxInvMatTitleSource : int
            {
                Name = 1,
                Name2 = 2,
                Code = 3
            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Overwrite VAT")]
            public double MY_TAXINTEGAZ_OVERWRITE_VAT
            {
                get
                {
                    return CASTASDOUBLE(_GET("MY_TAXINTEGAZ_OVERWRITE_VAT", "0"));
                }
                set
                {
                    _SET("MY_TAXINTEGAZ_OVERWRITE_VAT", FORMAT(value));
                }

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Use Reporting Currency")]
            public bool MY_TAXINTEGAZ_USE_REP_CURR
            {
                get
                {
                    return ISTRUE(_GET("MY_TAXINTEGAZ_USE_REP_CURR", "0"));
                }
                set
                {
                    _SET("MY_TAXINTEGAZ_USE_REP_CURR", FORMAT(value));
                }

            }


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Client Code Prefix")]
            public string MY_TAXINTEGAZ_CLIENT_CODE_PREFIX
            {
                get
                {
                    return (_GET("MY_TAXINTEGAZ_CLIENT_CODE_PREFIX", ""));
                }
                set
                {
                    _SET("MY_TAXINTEGAZ_CLIENT_CODE_PREFIX", value);
                }
            }

            //[ECategory(TEXT.text_DESC)]
            //[EDisplayName("Create Zip for Each Invoice")]
            //public bool MY_TAXINTEGAZ_EACH_INV_DIFF_ZIP
            //{
            //    get
            //    {
            //        return ISTRUE(_GET("MY_TAXINTEGAZ_EACH_INV_DIFF_ZIP", "1"));
            //    }
            //    set
            //    {
            //        _SET("MY_TAXINTEGAZ_EACH_INV_DIFF_ZIP", FORMAT(value));
            //    }

            //}
            //[ECategory(TEXT.text_DESC)]
            //[EDisplayName("Do Zip")]
            //public bool MY_TAXINTEGAZ_DO_ZIP
            //{
            //    get
            //    {
            //        return ISTRUE(_GET("MY_TAXINTEGAZ_DO_ZIP", "1"));
            //    }
            //    set
            //    {
            //        _SET("MY_TAXINTEGAZ_DO_ZIP", FORMAT(value));
            //    }

            //}

            //

            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_TAXINTEGAZ_USER),
                     FORMAT(pPLUGIN.GETSYSPRM_USER())
                     ) >= 0;
            }


        }



        public class TEXT
        {
            public const string text_DESC = "Tax Integration (Az)";

        }



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


            var isSlsInv = fn.StartsWith("ref.sls.doc.inv");
            var isPrchInv = fn.StartsWith("ref.prch.doc.inv");

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
            { "_root" ,event_TAXINTEGAZ_},
			{ "CmdText" ,"event name::"+event_TAXINTEGAZ_INVOICE_ALL_SALES},
			{ "Text" ,LANG("T_TAX - T_EXPORT - T_ADP_SLS_DOC_INV_8")},
			{ "ImageName" ,"percent_32x32"},
			{ "Name" ,event_TAXINTEGAZ_INVOICE_ALL_SALES},
            };

                            RUNUIINTEGRATION(tree, args);

                        }




                    }

                }


            }


            if (isSlsInv || isPrchInv)
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
            { "_name" , event_TAXINTEGAZ_INVOICE_SINGLE},

            { "_infoLocation" ,"event"},
            { "_infoArg" ,event_TAXINTEGAZ_INVOICE_SINGLE},

            { "Text" ,"T_TAX - T_EXPORT"},
            { "ImageName" ,"percent_16x16"},
         
            };

                                RUNUIINTEGRATION(menuItem, args);

                            }





                        }
                    }





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
			{ "CmdText" , "event name::"+pEvent },
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
                TOOL_UI.ERROR(this, "Cant add button: " + exc.Message);
            }

        }

        public void MY_SYS_USEREVENT_HANDLER(string EVENTCODE, object[] ARGS) //adapter start
        {


            //
            try
            {


                object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
                object arg2 = ARGS.Length > 1 ? ARGS[1] : null;
                var cmdLine = (((ARGS.Length > 2 ? ARGS[2] : null) as string) ?? "");

                string[] list_ = EXPLODELISTPATH(EVENTCODE);
                var cmd = list_.Length > 1 ? list_[1].ToLowerInvariant() : "";

                switch (cmd)
                {



                    case event_TAXINTEGAZ_INVOICE_ALL_SALES:
                        {

                            MY_EXPORT_TAX_ALL(8);

                        }
                        break;
                    case event_TAXINTEGAZ_INVOICE_SINGLE:
                        {

                            var rec = arg1 as DataRow;

                            if (rec == null)
                            {
                                var f = arg1 as Form;
                                if (f != null) // if (ISADAPTERFORM(f))
                                {
                                    var grid_ = CONTROL_SEARCH(f, "cGrid") as DataGridView;
                                    if (grid_ != null)
                                        rec = TOOL_GRID.GET_GRID_ROW_DATA(grid_);

                                }
                            }

                            MY_EXPORT_TAX_SINGLE(rec);

                        }
                        break;


                }


            }

            catch (Exception exc)
            {

                MSGUSERERROR(exc.Message);
                LOG(exc);
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
        public static bool MY_ASK_DATE(_PLUGIN pPLUGIN, string pMsg, ref DateTime pDf, ref DateTime pDt)
        {

            DataRow[] rows_ = pPLUGIN.REF("ref.gen.daterange showtime::0 desc::" + _PLUGIN.STRENCODE(pMsg)
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


        public void MY_EXPORT_TAX_ALL(short pTrcode)
        {
            DateTime dateFrom = DateTime.Now.Date;

            DateTime dateTo = DateTime.Now.Date;

            if (!MY_ASK_DATE(this, "T_DATE", ref dateFrom, ref dateTo))
                return;

            var clCodeFilter = MY_ASK_STRING(this, "T_CLIENT (T_CODE)", "");
            if (clCodeFilter == null)
                return;


            dateFrom = dateFrom.Date;
            dateTo = dateTo.Date;

            var reg = new REGTOTS();
            reg.dateFrom = dateFrom;
            reg.dateTo = dateTo;



            while (dateFrom <= dateTo)
            {

                _MY_EXPORT_TAX(0, dateFrom.Date, pTrcode, clCodeFilter, reg);

                dateFrom = dateFrom.AddDays(+1);
            }

            LOG_REGTOTS.LOG(this, reg);



        }
        public void MY_EXPORT_TAX_SINGLE(DataRow pRec)
        {

            if (pRec == null)
                return;
            if (pRec.Table.TableName != "INVOICE")
                return;

            var docLRef = CASTASINT(TAB_GETROW(pRec, "LOGICALREF"));


            var date = CASTASDATE(SQLSCALAR("select DATE_ from LG_$FIRM$_$PERIOD$_INVOICE where LOGICALREF = @P1", new object[] { docLRef }));
            var trcode = CASTASSHORT(SQLSCALAR("select TRCODE from LG_$FIRM$_$PERIOD$_INVOICE where LOGICALREF = @P1", new object[] { docLRef }));

            _MY_EXPORT_TAX(docLRef, date, trcode, "", new REGTOTS());

        }
        public void _MY_EXPORT_TAX(object pDocRef, DateTime pDate, short pTrcode, string pClCodeFilter, REGTOTS pReg)
        {
            var data = _MY_EXPORT_ZIP(pDocRef, pDate, pTrcode, pClCodeFilter, pReg);

            if (data != null)
                foreach (var itm in data)
                {
                    MY_DIR.SAVE(itm.Value, itm.Key);
                }



        }



        Dictionary<string, byte[]> _MY_EXPORT_ZIP(object pDocRef, DateTime pDate, short pTrcode, string pClCodeFilter, REGTOTS pReg)
        {
            string matTitleColumn = "";

            switch (_SETTINGS.BUF.MY_TAXINTEGAZ_MAT_TITLE_SRC)
            {
                case _SETTINGS.TaxInvMatTitleSource.Name:
                    matTitleColumn = "NAME";
                    break;
                case _SETTINGS.TaxInvMatTitleSource.Name2:
                    matTitleColumn = "STATENAME";
                    break;
                case _SETTINGS.TaxInvMatTitleSource.Code:
                    matTitleColumn = "CODE";
                    break;
                default:
                    throw new Exception("Tax material title setting not correct");
            }

            var listArgs = new List<object>();
            var listFilterWhere = new StringBuilder();

            var sql = @"SELECT  
                       T.LINETYPE, T.LOGICALREF AS LREF,T.TRCODE AS RECTYPE, 
                        T.INVOICEREF AS SRCLREF, 
                        COALESCE((SELECT DOC.FICHENO FROM LG_$FIRM$_$PERIOD$_INVOICE DOC WHERE DOC.LOGICALREF = T.INVOICEREF),'') AS SRCNR,
                        T.REPORTRATE, 
                        T.AMOUNT AS QTY, (T.VATMATRAH+T.VATAMNT) AS TOTALNET,T.VAT AS VAT,  
                         '' AS TRACKNR,T.CLIENTREF AS CARD2REF, 
                          MAT.{MAT_TITLE} AS MAT_TITLE,MAT.UNIVID AS MAT_UNIT,MAT.GTIPCODE AS MAT_EXTCODE, 
                          CL.CODE AS CL_CODE,
                          CL.DEFINITION_ AS CL_TITLE,
                          CL.TAXNR AS CL_TAXNR  
                        FROM  
                        LG_$FIRM$_$PERIOD$_STLINE AS T  
                        INNER JOIN  
                        LG_$FIRM$_CLCARD AS CL ON T.CLIENTREF = CL.LOGICALREF  
                        INNER JOIN  
                        LG_$FIRM$_ITEMS AS MAT ON T.STOCKREF = MAT.LOGICALREF  
                        WHERE 
 T.DATE_=@P1 AND T.TRCODE = @P2 AND T.CANCELLED=0 AND T.LINETYPE IN (0) 
{WHERE}
                        ORDER BY T.TRCODE ASC,T.INVOICEREF ASC,T.INVOICELNNO ASC";

            sql = sql.Replace("{MAT_TITLE}", matTitleColumn);

            listArgs.AddRange(new object[] { pDate, pTrcode });

            if (ISEMPTYLREF(pDocRef))
            {




                //filter client by code prefix _SETTINGS.BUF.MY_TAXINTEGAZ_CLIENT_CODE_PREFIX
                var listPrefx = _SETTINGS.BUF.MY_TAXINTEGAZ_CLIENT_CODE_PREFIX.Trim();

                // AND ( ... OR ...)
                if (!ISEMPTY(listPrefx))
                {
                    var prefixs = EXPLODELIST(listPrefx);

                    if (prefixs.Length > 0)
                    {
                        listFilterWhere.Append(" AND ").Append("(");
                        for (var i = 0; i < prefixs.Length; ++i)
                        {
                            listArgs.Add(prefixs[i] + "%");
                            var prmName = "@P" + listArgs.Count;
                            listFilterWhere.Append(i > 0 ? " OR " : "").Append("(CL.CODE LIKE " + prmName + ")");
                        }
                        listFilterWhere.Append(")");
                    }

                }

                //
                pClCodeFilter = pClCodeFilter.Trim();
                if (!ISEMPTY(pClCodeFilter))
                {
                    listFilterWhere.Append(" AND ").Append("(");
                    {
                        listArgs.Add(pClCodeFilter);
                        var prmName = "@P" + listArgs.Count;
                        listFilterWhere.Append("(CL.CODE LIKE " + prmName + ")");
                    }
                    listFilterWhere.Append(")");
                }





                ////filter client by code prefix _SETTINGS.BUF.MY_TAXINTEGAZ_CLIENT_CODE_PREFIX
                //var listPrefx = _SETTINGS.BUF.MY_TAXINTEGAZ_CLIENT_CODE_PREFIX.Trim();
                //if (!ISEMPTY(listPrefx))
                //{
                //    var prefixs = EXPLODELIST(listPrefx);
                //    if (prefixs.Length > 0)
                //    {
                //        listFilterWhere.Append(" AND ").Append("(");
                //        for (var i = 0; i < prefixs.Length; ++i)
                //        {
                //            listArgs.Add(prefixs[i] + "%");
                //            var prmName = "@P" + listArgs.Count;
                //            listFilterWhere.Append(i > 0 ? " OR " : "").Append("(CL.CODE LIKE " + prmName + ")");
                //        }
                //        listFilterWhere.Append(")");
                //    }

                //}



            }
            else
            {
                listArgs.Add(pDocRef);
                var prmName = "@P" + listArgs.Count;
                listFilterWhere.Append(" AND ").Append("( T.INVOICEREF = " + prmName + " )");

            }

            //if (listFilterWhere.Length > 0)
            //    listFilterWhere.Insert(0, " AND ");

            sql = sql.Replace("{WHERE}", listFilterWhere.ToString());

            var tab = SQL(sql, listArgs.ToArray());

            var pack = new INV_PACK();

            pack.prm_vat = _SETTINGS.BUF.MY_TAXINTEGAZ_OVERWRITE_VAT;
            pack.prm_mytaxnr = _SETTINGS.BUF.MY_TAXINTEGAZ_MY_TAX_NR;

            if (ISEMPTY(pack.prm_mytaxnr))
                throw new Exception("Set My Tax Nr");

            pack.date = pDate;
            pack.prm_unit = "ədəd";


            for (int i = 0; i < tab.Rows.Count; ++i)
            {

                var rec = tab.Rows[i];

                if (i == 0)
                    pack.begDoc(rec);//start new
                else if (!pack.isSameDoc(rec))
                {
                    pack.endDoc();//end prev
                    pack.begDoc(rec);//start new
                }


                pack.addRec(rec);

                if (i == tab.Rows.Count - 1)
                    pack.endDoc();


            }


            var prefix = LEFT(FORMAT(pDate), 10).Replace(":", "-").Replace(" ", "-");

            pReg.ADD(pack);

            return pack.zipPack(prefix);

        }
        class LOG_REGTOTS
        {

            public static void LOG(_PLUGIN pPLUGIN, REGTOTS pReg)
            {
                var sb = new StringBuilder();

                var currTitle = pPLUGIN.LANG("T_SYS_CURR1");

                sb.AppendLine("[EVENT::" + FORMAT(DateTime.Now) + "]");
                sb.AppendLine("Date From: " + LEFT(FORMAT(pReg.dateFrom), 10));
                sb.AppendLine("Date To: " + LEFT(FORMAT(pReg.dateTo), 10));
                sb.AppendLine("Tax Nr Count: " + FORMAT(pReg.totByVoen.Count));
                sb.AppendLine("Invoice Count: " + FORMAT(pReg.inv_count));
                sb.AppendLine("Total (" + currTitle + "): " + FORMAT(pReg.sum_tot, "#,0.00"));
                sb.AppendLine("******************************");
                sb.AppendLine("TaxNr\tTitle\tCount");
                foreach (var itm in pReg.countByVoen)
                {
                    var voen = itm.Key;
                    var count = itm.Value;
                    var title = GETDIC(pReg.voenTitle, voen, "");

                    sb.AppendLine(voen + "\t" + title + "\t" + FORMAT(count));
                }
                sb.AppendLine("******************************");
                sb.AppendLine("TaxNr\tTitle\tTotal(" + currTitle + ")");
                foreach (var itm in pReg.totByVoen)
                {
                    var voen = itm.Key;
                    var sum = itm.Value;
                    var title = GETDIC(pReg.voenTitle, voen, "");

                    sb.AppendLine(voen + "\t" + title + "\t" + FORMAT(sum, "#,0.00"));
                }
                sb.AppendLine("******************************");

                MY_DIR.LOG(sb.ToString());
            }




        }
        public class REGTOTS
        {
            public DateTime dateFrom;
            public DateTime dateTo;

            public double inv_count;
            public double sum_tot;
            public SortedDictionary<string, double> totByVoen = new SortedDictionary<string, double>();
            public SortedDictionary<string, double> countByVoen = new SortedDictionary<string, double>();
            public Dictionary<string, string> voenTitle = new Dictionary<string, string>();

            public void ADD(INV_PACK pPack)
            {

                if (pPack.isEmpty())
                    return;

                ++inv_count;

                sum_tot += pPack.header.sum_tot;

                if (totByVoen.ContainsKey(pPack.header.cl_taxnr))
                    totByVoen[pPack.header.cl_taxnr] += pPack.header.sum_tot;
                else
                    totByVoen[pPack.header.cl_taxnr] = pPack.header.sum_tot;

                if (countByVoen.ContainsKey(pPack.header.cl_taxnr))
                    countByVoen[pPack.header.cl_taxnr] += 1;
                else
                    countByVoen[pPack.header.cl_taxnr] = 1;

                voenTitle[pPack.header.cl_taxnr] = pPack.header.cl_title;

            }



        }


        public class INV_PACK
        {

            public String prm_mytaxnr;
            public double prm_vat;
            public DateTime date;
            public String prm_unit;


            public DocHeader header;


            String bodyPlaceHolder = "<!--(body)-->";


            Dictionary<string, byte[]> map = new Dictionary<string, byte[]>();

            public INV_PACK()
            {


            }

            public bool isSameDoc(DataRow pRec)
            {
                return (
                        header.rectype == CASTASSHORT(TAB_GETROW(pRec, "RECTYPE")) &&
                                header.srcDocLref == CASTASINT(TAB_GETROW(pRec, "SRCLREF")) &&
                                header.card2ref == CASTASINT(TAB_GETROW(pRec, "CARD2REF"))
                );
            }

            public void begDoc(DataRow pFirstRec)
            {
                header = new DocHeader();

                //header.firstlref = CASTASINT(TAB_GETROW(pFirstRec, "LREF"));
                header.rectype = CASTASSHORT(TAB_GETROW(pFirstRec, "RECTYPE"));
                header.srcDocLref = CASTASINT(TAB_GETROW(pFirstRec, "SRCLREF"));
                header.srcDocNr = CASTASSTRING(TAB_GETROW(pFirstRec, "SRCNR"));

                header.card2ref = CASTASINT(TAB_GETROW(pFirstRec, "CARD2REF"));
                header.rownr = 0;
                //
                header.cl_taxnr = CASTASSTRING(TAB_GETROW(pFirstRec, "CL_TAXNR"));
                header.cl_code = CASTASSTRING(TAB_GETROW(pFirstRec, "CL_CODE"));
                header.cl_title = CASTASSTRING(TAB_GETROW(pFirstRec, "CL_TITLE"));

                if (ISEMPTYLREF(header.card2ref))
                    throw new Exception("Select invoice client: " + header.srcDocLref);



                if (ISEMPTY(header.cl_taxnr))
                    throw new Exception("Set Client Tax Nr for: " + header.cl_title);

                if (ISEMPTY(header.srcDocNr))
                    header.srcDocNr = "_";//desc min len 1

                if (ISEMPTY(header.cl_code))
                    header.cl_code = "_";// 

                if (ISEMPTY(header.cl_title))
                    header.cl_title = "_";// 

                header.body.Append(
                        "<qaimeKime>" + HTMLESC(header.cl_taxnr) + "</qaimeKime>\n" +
                                "<qaimeKimden>" + HTMLESC(prm_mytaxnr) + "</qaimeKimden>\n" +
                                "<ds></ds>\n" +
                                "<dn></dn>\n" +
                                "<des>" + HTMLESC(LEFT(header.srcDocNr + "," + FORMAT(header.srcDocLref), 150)) + "</des>\n" +
                                "<des2>" + HTMLESC(LEFT(header.cl_code + "," + header.cl_title, 150)) + "</des2>\n" +
                                "<ma></ma>\n" +
                                "<mk></mk>\n");

                header.body.Append(
                        "<product>\n" +
                                "<qaimeTable>\n");

            }

            public void addRec(DataRow pRec)
            {

                var mat_extcode = CASTASSTRING(TAB_GETROW(pRec, "MAT_EXTCODE")).Trim();
                var mat_title = CASTASSTRING(TAB_GETROW(pRec, "MAT_TITLE")).Trim();


                if (ISEMPTY(mat_extcode))
                    throw new Exception("Set Material Tax Nr: " + mat_title);

                var mat_unit = CASTASSTRING(TAB_GETROW(pRec, "MAT_UNIT")).Trim();
                if (ISEMPTY(mat_unit))
                    mat_unit = prm_unit;

                double qty = ROUND(CASTASDOUBLE(TAB_GETROW(pRec, "QTY")), 4);
                double reprate = CASTASDOUBLE(TAB_GETROW(pRec, "REPORTRATE"));

                double _totalnet = CASTASDOUBLE(TAB_GETROW(pRec, "TOTALNET"));

                if (_SETTINGS.BUF.MY_TAXINTEGAZ_USE_REP_CURR)
                {
                    _totalnet = DIV(_totalnet, reprate);
                }

                if (ISNUMZERO(_totalnet))
                {
                    throw new Exception("Invoice has empty (price is zero): " + header.srcDocLref);
                }


                double _vat = (ISNUMZERO(prm_vat) ? CASTASDOUBLE(TAB_GETROW(pRec, "VAT")) : prm_vat);
                //
                var lineType = CASTASSHORT(TAB_GETROW(pRec, "LINETYPE"));
                if (lineType == 3)
                    _vat = 0;
                //
                double price = ROUND(DIV(_totalnet * 100.0 / (100.0 + _vat), qty), 4);
                double vatbase = ROUND(price * qty, 4);
                double vattot = ROUND(vatbase * _vat / 100.0, 4);
                double tot = ROUND(vattot + vatbase, 4);

                header.sum_vatbase += vatbase;
                header.sum_vatbase_other += (ISNUMZERO(vattot) ? 0 : vatbase);
                header.sum_vatbase_zero += (ISNUMZERO(vattot) ? vatbase : 0);

                header.sum_vattot += vattot;
                header.sum_tot += tot;

                header.body.Append(

                        "<row no = '" + header.rownr + "'>\n" +
                                "<c1>" + HTMLESC(mat_extcode) + "</c1>\n" +
                                "<c2>" + HTMLESC(mat_title) + "</c2>\n" +
                                "<c3>" + HTMLESC(mat_unit) + "</c3>\n" +
                                "<c4>" + HTMLESC(FORMAT(qty)) + "</c4>\n" +
                                "<c5>" + HTMLESC(FORMAT(price)) + "</c5>\n" +
                                "<c6>" + HTMLESC(FORMAT(vatbase)) + "</c6>\n" +
                                "<c7>0</c7>\n" +
                                "<c8>0</c8>\n" +
                                "<c9>" + HTMLESC(FORMAT(vatbase)) + "</c9>\n" +
                                "<c10>" + HTMLESC(FORMAT(ISNUMZERO(vattot) ? 0 : vatbase)) + "</c10>\n" + //VAT>0
                                "<c11>" + HTMLESC(FORMAT(ISNUMZERO(vattot) ? vatbase : 0)) + "</c11>\n" + //VAT==0
                                "<c12>0</c12>\n" +
                                "<c13>" + HTMLESC(FORMAT(vattot)) + "</c13>\n" +
                                "<c15>0</c15>\n" +
                                "<c14>" + HTMLESC(FORMAT(tot)) + "</c14>" +

                                "</row>\n"
                );


                ++header.rownr;
            }

            public void endDoc()
            {
                var roundEnd = 2;

                header.sum_vatbase = ROUND(header.sum_vatbase, roundEnd);
                header.sum_vatbase_zero = ROUND(header.sum_vatbase_zero, roundEnd);
                header.sum_vatbase_other = ROUND(header.sum_vatbase_other, roundEnd);

                header.sum_vattot = ROUND(header.sum_vattot, roundEnd);
                header.sum_tot = ROUND(header.sum_tot, roundEnd);

                header.body.Append(
                        "</qaimeTable>\n" +
                                "<qaimeYekunTable>\n" +
                                "<row>\n" +
                                "<c1>" + HTMLESC(FORMAT(header.sum_vatbase)) + "</c1>\n" +
                                "<c2>0</c2>\n" +
                                "<c3>" + HTMLESC(FORMAT(header.sum_vatbase)) + "</c3>\n" +
                                "<c4>" + HTMLESC(FORMAT(header.sum_vatbase_other)) + "</c4>\n" +  //ƏDV-yə cəlb edilən məbləğ
                                "<c5>" + HTMLESC(FORMAT(header.sum_vatbase_zero)) + "</c5>\n" +//ƏDV-yə cəlb edilməyən məbləğ
                                "<c6>0</c6>\n" +
                                "<c7>" + HTMLESC(FORMAT(header.sum_vattot)) + "</c7>\n" +
                                "<c8>" + HTMLESC(FORMAT(header.sum_tot)) + "</c8>\n" +
                                "</row>\n" +
                                "</qaimeYekunTable>\n" +
                                "</product>\n"
                );


                String docId = header.getDocId();
                byte[] rawBody = ARRFROMSTRUTF8(__docWrapXml.Replace(bodyPlaceHolder, header.body.ToString()));

                map[docId] = rawBody;

            }

            public bool isEmpty()
            {
                return (map.Count == 0);
            }
            public Dictionary<string, byte[]> zipPack(string pSufix)
            {

                if (isEmpty())
                    return null;


                //file name

                //  var zip = _SETTINGS.BUF.MY_TAXINTEGAZ_DO_ZIP;
                //var separate = _SETTINGS.BUF.MY_TAXINTEGAZ_EACH_INV_DIFF_ZIP;



                //if (separate)
                //{
                //    var dic = new Dictionary<string, byte[]>();

                //    foreach (var itm in map)
                //    {

                //        dic[itm.Key + ".zip"] = ZIP(new Dictionary<string, byte[]>(){
                //        {itm.Key,itm.Value}
                //        });
                //    }

                //    return dic;
                //}
                //else
                {
                    var data = ZIP(map);
                    var prefix = FORMAT(DateTime.Now).Replace(":", "-").Replace(" ", "-");
                    return new Dictionary<string, byte[]>(){
                    {"data."+prefix +"."+pSufix+".zip",data}
                    };
                }


            }

            public class DocHeader
            {


                public int rownr;
                public short rectype;
                // public int firstlref;
                public int srcDocLref;
                public string srcDocNr;

                public int card2ref;
                public string cl_taxnr;
                public string cl_code;
                public string cl_title;

                public double sum_vatbase;
                public double sum_vatbase_other;
                public double sum_vatbase_zero;//if vat 0 
                public double sum_vattot;
                public double sum_tot;


                public StringBuilder body = new StringBuilder();

                public String getDocId()
                {
                    var code = MAKENAME(LEFT(cl_code, 20));
                    var title = MAKENAME(LEFT(cl_title, 20));
                    //+ firstlref + "-" 
                    return "tax-inv" + rectype + "-" + cl_taxnr + "-" + code + "-" + title + "-nr-" + srcDocLref + "-tot-" + FORMAT(ROUND(sum_tot, 2)) + ".xml";
                }


            }
        }


        #region CLAZZ




        class TOOL_FS
        {

        }


        class MY_DIR
        {
            public static string PRM_DIR_ROOT = PATHCOMBINE(
     Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "TAX_AZ");

            public static void CHECK_DIR()
            {

                if (!System.IO.Directory.Exists(PRM_DIR_ROOT))
                    System.IO.Directory.CreateDirectory(PRM_DIR_ROOT);


            }


            public static void SAVE(byte[] pData, string pFileName)
            {


                CHECK_DIR();

                var fileName = pFileName;

                FILEWRITE(PRM_DIR_ROOT + "/" + fileName, pData);

            }

            public static void LOG(string pInfo)
            {

                CHECK_DIR();

                System.IO.File.AppendAllText(PRM_DIR_ROOT + "/" + "log.txt", pInfo + "\r\n");


            }


        }


        class TOOL_UI
        {

            public static void ERROR(_PLUGIN pPLUGIN, string pMsg)
            {
                pPLUGIN.MSGUSERERROR(pMsg);
            }

            public static void ERROR(_PLUGIN pPLUGIN, Exception pExc)
            {

                {
                    var err = pExc as System.Net.WebException;
                    if (err != null)
                    {
                        switch (err.Status)
                        {
                            case System.Net.WebExceptionStatus.Timeout:
                            case System.Net.WebExceptionStatus.ConnectFailure:
                                ERROR(pPLUGIN, "T_MSG_ERROR_CONNECTION");
                                return;
                            case System.Net.WebExceptionStatus.ProtocolError:

                                var respHttp = err.Response as System.Net.HttpWebResponse;
                                if (respHttp != null)
                                {
                                    switch (respHttp.StatusCode)
                                    {

                                        case System.Net.HttpStatusCode.Forbidden:
                                        case System.Net.HttpStatusCode.Unauthorized:
                                            ERROR(pPLUGIN, "T_MSG_ERROR_NOT_ALLOWED (" + err.Message + ")");
                                            return;

                                    }


                                }
                                break;
                        }
                    }
                }

                ERROR(pPLUGIN, pExc.Message);
            }


        }









        const string __docWrapXml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<?xml-stylesheet type=""text/xsl"" href=""#stylesheet""?>
<!DOCTYPE root [
        <!ATTLIST xsl:stylesheet
                id ID #REQUIRED>
        ]>
<root version =""205"" kod=""QAIME_1"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:noNamespaceSchemaLocation=""QAIME_1.xsd"" >
    <xsl:stylesheet id=""stylesheet"" version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"" >
        <xsl:template match=""xsl:stylesheet"" />
        <xsl:template match=""/root"">
            <html>
                <head>
                    <style>
                        body {background-color: white;font-family:  Arial, sans-serif;}.paper {padding:5px;}
                        table {width: 100%;font-size: 16px;}
                        table tr td {padding: 10px 15px;text-align: left;width:50%;}.products table {border-collapse: collapse;font-size: 14px;}
                        .products table th, #products table td  {border: 1px solid #000;padding: 10px;}
                        .products table td {width:auto;border: 1px solid #000;text-align:center;}
                        .products table th {text-align:center;}
                        .noPadding {padding: 40px 0px;}
                        .total tr :nth-child(odd) {width:40%;}
                        .total tr :nth-child(even) {width:10%;}
                    </style>
                </head>
                <body>
                    <table class=""paper"">
                        <tr>
                            <td>Alan tərəfin VÖENi:</td>
                            <td><xsl:value-of select=""qaimeKime""/></td>
                        </tr>
                        <tr>
                            <td>Alan tərəfin adı:</td>
                            <td><xsl:value-of select=""qaimeKimeAd""/></td>
                        </tr>
                        <tr>
                            <td>Satan tərəfin VÖENi:</td>
                            <td><xsl:value-of select=""qaimeKimden""/></td>
                        </tr>
                        <tr>
                            <td>Dəqiqləşdirilən qaimənin seriyası</td>
                            <td><xsl:value-of select=""ds""/></td>
                        </tr>
                        <tr>
                            <td>Dəqiqləşdirilən qaimənin nömrəsi</td>
                            <td><xsl:value-of select=""dn""/></td>
                        </tr>
                        <tr>
                            <td>Qeyd</td>
                            <td><xsl:value-of select=""des""/></td>
                        </tr>
                        <tr>
                            <td>Əlavə qeyd</td>
                            <td><xsl:value-of select=""des2""/></td>
                        </tr>
                        <tr>
                            <td>Obyektin adı</td>
                            <td><xsl:value-of select=""ma""/></td>
                        </tr>
                        <tr>
                            <td>Obyektin kodu</td>
                            <td><xsl:value-of select=""mk""/></td>
                        </tr>
                        <tr>
                            <td class=""products noPadding"" colspan=""2"">
                                <table>
                                    <thead>
                                        <th>Mal kodu</th>
                                        <th>Mal adı</th>
                                        <th>Ölçü vahidi</th>
                                        <th>Malın miqdarı</th>
                                        <th>Malın buraxılış qiyməti</th>
                                        <th>Cəmi qiyməti</th>
                                        <th>Aksiz dərəcəsi</th>
                                        <th>Aksiz məbləği</th>
                                        <th>Cəmi məbləğ</th>
                                        <th>ƏDV-yə cəlb edilən məbləğ</th>
                                        <th>ƏDV-yə cəlb edilməyən məbləğ</th>
                                        <th>ƏDV-yə 0 dərəcə ilə cəlb edilən məbləğ</th>
                                        <th>Ödənilməli ƏDV</th>
                                        <th>Yol vergisi məbləği</th>
                                        <th>Yekun məbləğ</th>
                                    </thead>
                                    <tbody class=""productTable"">
                                        <xsl:for-each select=""product/qaimeTable/row"">
                                            <tr>
                                                <td><xsl:value-of select=""c1""/></td>
                                                <td><xsl:value-of select=""c2""/></td>
                                                <td><xsl:value-of select=""c3""/></td>
                                                <td><xsl:value-of select=""c4""/></td>
                                                <td><xsl:value-of select=""c5""/></td>
                                                <td><xsl:value-of select=""c6""/></td>
                                                <td><xsl:value-of select=""c7""/></td>
                                                <td><xsl:value-of select=""c8""/></td>
                                                <td><xsl:value-of select=""c9""/></td>
                                                <td><xsl:value-of select=""c10""/></td>
                                                <td><xsl:value-of select=""c11""/></td>
                                                <td><xsl:value-of select=""c12""/></td>
                                                <td><xsl:value-of select=""c13""/></td>
                                                <td><xsl:value-of select=""c15""/></td>
                                                <td><xsl:value-of select=""c14""/></td>
                                            </tr>
                                        </xsl:for-each>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <table class=""total"">
                        <tr>
                            <td>Malların cəmi qiyməti</td>
                            <td><xsl:value-of select=""product/qaimeYekunTable/row/c1""/></td>
                            <td>Malların aksiz cəmi məbləği</td>
                            <td><xsl:value-of select=""product/qaimeYekunTable/row/c2""/></td>
                        </tr>
                        <tr>
                            <td>Malların cəmi məbləği</td>
                            <td><xsl:value-of select=""product/qaimeYekunTable/row/c3""/></td>
                            <td>Malların ƏDV-yə cəlb edilən cəmi məbləği</td>
                            <td><xsl:value-of select=""product/qaimeYekunTable/row/c4""/></td>
                        </tr>
                        <tr>
                            <td>Malların  ƏDV-yə cəlb edilməyən cəmi məbləği </td>
                            <td><xsl:value-of select=""product/qaimeYekunTable/row/c5""/></td>
                            <td>Malların  ƏDV-yə 0 dərəcə ilə cəlb edilən cəmi məbləği</td>
                            <td><xsl:value-of select=""product/qaimeYekunTable/row/c6""/></td>
                        </tr>
                        <tr>
                            <td>Malların cəmi ödənilməli ƏDV məbləği</td>
                            <td><xsl:value-of select=""product/qaimeYekunTable/row/c7""/></td>
                            <td>Yekun məbləğ</td>
                            <td><xsl:value-of select=""product/qaimeYekunTable/row/c8""/></td>
                        </tr>
                    </table>
                </body>
            </html>
        </xsl:template>
    </xsl:stylesheet>
    <!--(body)-->
</root>
";

        #endregion



        #endregion
