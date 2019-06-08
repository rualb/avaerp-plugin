 #line 2
 
    #region PLUGIN_BODY
        const int VERSION = 9;


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


                    //

                    x._USER = PLUGIN.GETSYSPRM_USER();
                    x._FIRM = PLUGIN.GETSYSPRM_FIRM();
                    x._FIRMNAME = PLUGIN.GETSYSPRM_FIRMNAME();
                    x._PERIOD = PLUGIN.GETSYSPRM_PERIOD();



                    _SETTINGS.BUF = x;


                }


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
            [EDisplayName("Prevent Negative Stock")]
            public bool MY_MAGENT_SYNC_PREVENT_NEQATIV_STOCK
            {
                get
                {
                    return ISTRUE(_GET("MY_MAGENT_SYNC_PREVENT_NEQATIV_STOCK", "0"));
                }
                set
                {
                    _SET("MY_MAGENT_SYNC_PREVENT_NEQATIV_STOCK", value);
                }
            }


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Cancel Sale")]
            public bool MY_MAGENT_SYNC_IMP_SALE_CANCELLED
            {
                get
                {
                    return ISTRUE(_GET("MY_MAGENT_SYNC_IMP_SALE_CANCELLED", "0"));
                }
                set
                {
                    _SET("MY_MAGENT_SYNC_IMP_SALE_CANCELLED", value);
                }
            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Cancel Sale Return")]
            public bool MY_MAGENT_SYNC_IMP_RETURN_CANCELLED
            {
                get
                {
                    return ISTRUE(_GET("MY_MAGENT_SYNC_IMP_RETURN_CANCELLED", "0"));
                }
                set
                {
                    _SET("MY_MAGENT_SYNC_IMP_RETURN_CANCELLED", value);
                }
            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Cancel Cash In")]
            public bool MY_MAGENT_SYNC_IMP_CASHIN_CANCELLED
            {
                get
                {
                    return ISTRUE(_GET("MY_MAGENT_SYNC_IMP_CASHIN_CANCELLED", "0"));
                }
                set
                {
                    _SET("MY_MAGENT_SYNC_IMP_CASHIN_CANCELLED", value);
                }
            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Cancel Cash Out")]
            public bool MY_MAGENT_SYNC_IMP_CASHOUT_CANCELLED
            {
                get
                {
                    return ISTRUE(_GET("MY_MAGENT_SYNC_IMP_CASHOUT_CANCELLED", "0"));
                }
                set
                {
                    _SET("MY_MAGENT_SYNC_IMP_CASHOUT_CANCELLED", value);
                }
            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Sale Spe-Code")]
            public string MY_MAGENT_SYNC_IMP_SALE_SPECODE
            {
                get
                {
                    return CASTASSTRING(_GET("MY_MAGENT_SYNC_IMP_SALE_SPECODE", ""));
                }
                set
                {
                    _SET("MY_MAGENT_SYNC_IMP_SALE_SPECODE", value);
                }
            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Sale Return Spe-Code")]
            public string MY_MAGENT_SYNC_IMP_RETURN_SPECODE
            {
                get
                {
                    return CASTASSTRING(_GET("MY_MAGENT_SYNC_IMP_RETURN_SPECODE", ""));
                }
                set
                {
                    _SET("MY_MAGENT_SYNC_IMP_RETURN_SPECODE", value);
                }
            }


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Cash In Spe-Code")]
            public string MY_MAGENT_SYNC_IMP_CASHIN_SPECODE
            {
                get
                {
                    return CASTASSTRING(_GET("MY_MAGENT_SYNC_IMP_CASHIN_SPECODE", ""));
                }
                set
                {
                    _SET("MY_MAGENT_SYNC_IMP_CASHIN_SPECODE", value);
                }
            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Cash Out Spe-Code")]
            public string MY_MAGENT_SYNC_IMP_CASHOUT_SPECODE
            {
                get
                {
                    return CASTASSTRING(_GET("MY_MAGENT_SYNC_IMP_CASHOUT_SPECODE", ""));
                }
                set
                {
                    _SET("MY_MAGENT_SYNC_IMP_CASHOUT_SPECODE", value);
                }
            }


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Order For Input Spe-Code")]
            public string MY_MAGENT_SYNC_IMP_ORDERIN_SPECODE
            {
                get
                {
                    return CASTASSTRING(_GET("MY_MAGENT_SYNC_IMP_ORDERIN_SPECODE", ""));
                }
                set
                {
                    _SET("MY_MAGENT_SYNC_IMP_ORDERIN_SPECODE", value);
                }
            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Order For Output Spe-Code")]
            public string MY_MAGENT_SYNC_IMP_ORDEROUT_SPECODE
            {
                get
                {
                    return CASTASSTRING(_GET("MY_MAGENT_SYNC_IMP_ORDEROUT_SPECODE", ""));
                }
                set
                {
                    _SET("MY_MAGENT_SYNC_IMP_ORDEROUT_SPECODE", value);
                }
            }
        }

        #endregion


        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "Sync Export-Import";
        }
        #endregion




        #region MAIN


        bool PRINTINV = false;
        bool OFFLINE = true;
        List<object> PRINTINVLREF = new List<object>();
        public object SYS_DONE(string CMD, object[] ARGS)
        {

            SYS_BEGIN(CMD, ARGS);

            return null;
        }

        public void SYS_BEGIN(string CMD, object[] ARGS)
        {
            try
            {

                _SETTINGS._BUF.LOAD_SETTINGS(this);


                ARGS = (ARGS == null ? new object[0] : ARGS);

                object arg0 = ARGS.Length > 0 ? ARGS[0] : null;
                object arg1 = ARGS.Length > 1 ? ARGS[1] : null;
                object arg2 = ARGS.Length > 2 ? ARGS[2] : null;

                switch (CMD)
                {
                    case SysEvent.SYS_PLUGINSETTINGS:
                        (arg0 as List<object>).Add(new _SETTINGS(this));
                        break;
                    case "STARTED":
                        MY_STARTED();
                        break;
                    case "DATASTARTING":
                        MY_DATASTARTING(arg0 as string);
                        break;
                    case "DATAFINISHING":
                        MY_DATAFINISHING(arg0 as string);
                        break;
                    case "DATACOMMIT":
                        MY_DATACOMMIT(arg0 as string);
                        break;
                    case "DATAIMPORTED":
                        MY_DATAIMPORTED(arg0 as object, arg1 as XmlNode, arg2 as XmlNode);
                        break;

                    case SysEvent.SYS_LOGIN:

                        MY_DATASOURCE.UPDATE(this);
                        break;
                    case "DATAEXPORT":
                        {

                            var prop = arg0 as Dictionary<string, object>;
                            var dataset = arg1 as TOOL_WEB.DATASET;

                            var agent = new AGENT(prop);





                            //
                            dataset.TABLES.Add(MY_WRITESYSTEM(agent));
                            dataset.TABLES.Add(MY_WRITECLIENT(agent));
                            dataset.TABLES.Add(MY_WRITEITEMS(agent));
                            dataset.TABLES.Add(MY_WRITEMARKCODES(agent));
                            //MY_WRITEWAREHOUSE(agent, dataset);
                            ////MY_WRITEDOCS(agent, dataset);
                            ////
                            //MY_WRITEINFOFROMFIRM(agent, dataset);
                            //MY_WRITEINFOFROMPERIOD(agent, dataset);
                            //MY_WRITEINFODOCSAVE(agent, dataset);

                            ////
                            //MY_CHANGE(agent, dataset);

                        }
                        break;
                    case "DATADOCEND":
                        {

                        }
                        break;



                }
            }
            catch (Exception exc)
            {
                RUNTIMELOG(exc.ToString());

                EXCEPTIONFORUSER("T_MSG_ERROR_INNER_SERVER");
            }

        }






        void MY_STARTED()
        {



        }


        void MY_DATASTARTING(string file)
        {
            PRINTINVLREF.Clear();

        }
        void MY_DATAFINISHING(string file)
        {



        }
        void MY_DATACOMMIT(string file)
        {

            if (PRINTINV)
            {
                foreach (object l in PRINTINVLREF)
                {
                    string cmd_ = "rep loc::config/report/mm.000029 filter::{0} REP_QUICK_MODE_B::1 REP_DEF_FILTER_B::1 REP_DSG_KEY_WORDS_S::printsync DSGN_OUTPUT_B::1 DSGN_OUTPUT_DEF_DEV_B::1";
                    string filter_ = string.Format("filter_INVOICE_LOGICALREF2,{0}", FORMATSERIALIZE(l));
                    cmd_ = string.Format(cmd_, filter_);
                    EXECMDTEXT(cmd_);
                }
            }
            //FILEWRITE("log.txt","\n"+FORMAT(l),false);


            PRINTINVLREF.Clear();
        }
        void MY_DATAIMPORTED(object lref, XmlNode nodeRoot, XmlNode nodeDoc)
        {
            if (PRINTINV)
            {

                if (XMLNODEATTR(nodeDoc, "ei_code") == "ADP_INVOICE")
                {

                    PRINTINVLREF.Add(lref);

                }
            }


        }
        object MY_DATAIMPORT(XmlNode nodeRoot, XmlNode nodeDoc)
        {

            return null;

        }



        static short GETAGENT(XmlNode nodeRoot)
        {
            return PARSESHORT(XMLNODEATTR(nodeRoot, "MOB_SYS_AGENT_ID"));

        }

        static short GETWH(XmlNode nodeRoot)
        {
            return PARSESHORT(XMLNODEATTR(nodeRoot, "MOB_SYS_AGENT_WH"));
        }






        #endregion



        #region TOOLS

        static string _TRIM(object pObj)
        {

            if (pObj == null)
                pObj = "";
            string str = (pObj.GetType() == typeof(string) ? (string)(pObj) : _PLUGIN.FORMAT(pObj));

            if (str.Length > 100)
                str.Substring(0, 100);

            return str.Replace('\t', '_').Replace('\n', '_');
        }



        void _APPLY_FILTER(AGENT pAgent, string pParamName, string[] pFilters, StringBuilder pWhere, List<object> pArgs)
        {

            string filters = pAgent.GET_PRM(pParamName);
            if (!ISEMPTY(filters))
            {

                //TODO User auth code replace ,;[space]

                filters = filters.Replace("{authcode}", pAgent.AGENTAUTHCODE);
                var map = EXPLODEFORPARAMETERS(filters);

                bool firstCol = true;
                foreach (var filterColumn in pFilters)
                {
                    if (map.ContainsKey(filterColumn))
                    {
                        var valList = map[filterColumn];
                        if (!ISEMPTY(valList))
                        {

                            bool firstVal = true;
                            var valArr = EXPLODELIST(valList);

                            if (!firstCol)
                                pWhere.AppendLine(" and ");

                            pWhere.AppendLine("( ");

                            foreach (var val in valArr)
                            {

                                if (!firstVal)
                                    pWhere.AppendLine(" OR ");

                                pWhere.Append("(" + filterColumn + " LIKE @P" + (pArgs.Count + 1) + ")");
                                pArgs.Add(val);

                                firstVal = false;
                            }
                            pWhere.AppendLine(" )");


                            firstCol = false;
                        }

                    }

                }


            }

        }

        TOOL_WEB.TABLE MY_WRITESYSTEM(AGENT pAgent)
        {


            var t = new TOOL_WEB.TABLE();
            t.TABLENAME = "params";


            t.ADDCOL("code", "string");
            t.ADDCOL("value", "string");




            t.ADDROW(new object[] { "prm_magent_firm_name", _TRIM(GETSYSPRM_FIRMNAME()) });
            t.ADDROW(new object[] { "prm_magent_agent_title", _TRIM((pAgent.AGENTNAME)) });
            t.ADDROW(new object[] { "prm_magent_agent_id", _TRIM(pAgent.AGENTNR) });

            t.ADDROW(new object[] { "prm_magent_agent_wh", CASTASSHORT(pAgent.GET_PRM("prm_magent_agent_wh")) });
            t.ADDROW(new object[] { "prm_magent_agent_dep", CASTASSHORT(pAgent.GET_PRM("prm_magent_agent_dep")) });
            t.ADDROW(new object[] { "prm_magent_now", FORMAT(pAgent.AGENTDATE) });

            t.ADDROW(new object[] { "prm_magent_mng_mat", _TRIM(pAgent.GET_PRM("prm_magent_mng_mat", "")) });
            t.ADDROW(new object[] { "prm_magent_mng_client", _TRIM(pAgent.GET_PRM("prm_magent_mng_client")) });

            //
            t.ADDROW(new object[] { "prm_magent_allow_tran", _TRIM(pAgent.GET_PRM("prm_magent_allow_tran")) });
            t.ADDROW(new object[] { "prm_magent_allow_manage", _TRIM(pAgent.GET_PRM("prm_magent_allow_manage")) });
            t.ADDROW(new object[] { "prm_magent_allow_ext", _TRIM(pAgent.GET_PRM("prm_magent_allow_ext")) });
            t.ADDROW(new object[] { "prm_magent_allow_report", _TRIM(pAgent.GET_PRM("prm_magent_allow_report", "")) });
            //

            foreach (var prmName in pAgent.GET_PRM_ALL())
            {
                if (prmName.StartsWith("prm_magent_user_"))
                    t.ADDROW(new object[] { prmName, _TRIM(pAgent.GET_PRM(prmName, "")) });
            }


            return t;

            //DataTable tabTmp = new DataTable("FIRMPARAMS");
            //tabTmp.Columns.Add("CODE", typeof(string));
            //tabTmp.Columns.Add("VALUE", typeof(object));

            //var dic = new Dictionary<string, string>();

            //dic["MOB_SYS_CURR1"] = LANG("T_SYS_CURR1");

            //dic["MOB_SYS_AGENT_NAME"] = FORMAT(pAgent.AGENTNAME);
            //dic["MOB_SYS_AGENT_ID"] = FORMAT(pAgent.AGENTNR);
            //dic["MOB_SYS_CMD_DELETE_DOCS_BEFORE"] = "2085-01-01 00-00-00";// FORMAT(new DateTime(2085, 1, 1));// FORMAT(pAgent.AGENTDATE);
            //dic["MOB_SYS_AGENT_WH"] = MY_TOGUID(FORMAT(pAgent.AGENTWH));
            //dic["MOB_SYS_WEIGHT_UNIT"] = FORMAT("kq");
            //dic["MOB_USR_DATA_ID"] = FORMAT(pAgent.AGENTGUID);

            //dic["MOB_SYS_FIRM"] = FORMAT(GETSYSPRM_FIRM());
            //dic["MOB_SYS_FIRMNAME"] = FORMAT(GETSYSPRM_FIRMNAME());
            //dic["MOB_SYS_PERIOD"] = FORMAT(GETSYSPRM_PERIOD());


            ////dic["MOB_DESCS_INV_8"] = "NEGD,KREDIT";
            //// dic["MOB_REQCOLS_INV_8"] = "GENEXP1";
            //dic["MOB_DEFAULTVALS_INV_8"] = "INVOICE,CLCARD_____CODE,/MOB_SYS_AGENT_ID";
            //dic["MOB_DEFAULTVALS_INV_1"] = "INVOICE,CLCARD_____CODE,/MOB_SYS_AGENT_ID";
            //dic["MOB_DEFAULTVALS_INV_6"] = "INVOICE,CLCARD_____CODE,/MOB_SYS_AGENT_ID";
            //dic["MOB_DEFAULTVALS_INV_3"] = "INVOICE,CLCARD_____CODE,/MOB_SYS_AGENT_ID";
            ////dic["MOB_DESCS_INV_3"] = "TARIX,XARAB";
            ////dic["MOB_DESCS_ORDER_3"] = "DATE EXP.,CLOSING,OTHER";
            ////dic["MOB_LISTS_ORDER_8"] = "LIST01,T_GROUP,GENEXP2";
            ////dic["LIST01"] = "YES,T_YES;NO,T_NO";
            ////dic["MOB_PRCLISTS_INV_8"] = "1";
            //dic["MOB_SYS_DEF_PLIST"] = "1";
            ////dic["MOB_SYS_DOPAYMENTEXP"] = "NEGD";


            ////dic["MOB_PRCLISTS_INV_1"] = "1";
            ////dic["MOB_PRCLISTS_INV_6"] = "1";
            ////dic["MOB_PRCLISTS_INV_8"] = "1,2,3,4,5";

            //dic["MOB_SYS_CANPAYMENT"] = "0";
            //dic["MOB_SOURCEINDEX_CHANGE_INV_15"] = "1";




            //dic["MOB_SYS_AGENTSEQRESEED"] = MOB_SYS_AGENTSEQRESEED(this, pAgent.AGENTNR);



            ////dic["MOB_DESC_COLOR_MARKER"] = "(OLMAZ),#FF0000,(OLMAZ5),#00FF00,(OLMAZ10),#FFFF00" ;
            ////dic["MOB_MAX_DOC_COUNT"] = "3" ;
            ////dic["MOB_MAX_CLIENT_BY_PREFIX"] = "(OLMAZ),3";

            ////MOB_DISCOUNTS_INV_8
            ////MOB_DESCS_ORDER_8
            ////MOB_LISTS_ORDER_8

            //foreach (string key in dic.Keys)
            //    tabTmp.Rows.Add(new object[] { key, dic[key] });

            //pDataSet.Tables.Add(tabTmp);

        }


        TOOL_WEB.TABLE MY_WRITEMARKCODES(AGENT pAgent)
        {


            var args = new List<object>();


            var sbFilters = new StringBuilder();

            _APPLY_FILTER(pAgent, "prm_magent_mcode_filter", new String[] { "DEFINITION_" }, sbFilters, args);

            //IF FILTER EMPTY 
            if (sbFilters.Length == 0)
                return null;



            var sqlText = "SELECT " +
                            "X.LOGICALREF AS LREF,X.SPECODE AS CODE,X.DEFINITION_ AS TITLE,X.CODETYPE AS CODETYPE,X.SPECODETYPE AS SPECODETYPE" +


                            " FROM " +
                            "LG_$FIRM$_SPECODES AS X WHERE X.SPECODETYPE IN (14,15,22,23,44) AND X.CODETYPE IN (1,502,503) AND " +
                            (sbFilters.ToString()) +
                            " " +
                            " ";



            var tab_tmp = _PLUGIN.XSQL(SQLTEXT(sqlText), args.ToArray());
 
            var tab = new DataTable();
            TAB_ADDCOL(tab, "lref", typeof(int));
            TAB_ADDCOL(tab, "rectype", typeof(string));
            TAB_ADDCOL(tab, "code", typeof(string));
            TAB_ADDCOL(tab, "title", typeof(string));

            foreach (DataRow row in tab_tmp.Rows)
            {
                var cardType = CASTASSHORT(TAB_GETROW(row, "SPECODETYPE"));
                var codeType = CASTASSHORT(TAB_GETROW(row, "CODETYPE"));

                var code = CASTASSTRING(TAB_GETROW(row, "CODE"));
                var title = CASTASSTRING(TAB_GETROW(row, "TITLE"));
                var lref = CASTASINT(TAB_GETROW(row, "LREF"));

                
                var indx = codeType % 100;
                if (indx < 1 || indx > 3)
                    continue;

                
                var listRecTypes = new List<string>();

                switch (cardType)
                {
                    case 14:
                        listRecTypes.Add("adp.erp.ord.doc.8" + "," + "markcode" + indx); //out 14
                        break;
                    case 15:
                        listRecTypes.Add("adp.erp.ord.doc.1" + "," + "markcode" + indx); //in 15
                        break;
                    case 22:
                        listRecTypes.Add("adp.erp.prch.doc.inv.1" + "," + "markcode" + indx);
                        listRecTypes.Add("adp.erp.prch.doc.inv.6" + "," + "markcode" + indx);
                        break;
                    case 23:
                        listRecTypes.Add("adp.erp.sls.doc.inv.3" + "," + "markcode" + indx);
                        listRecTypes.Add("adp.erp.sls.doc.inv.8" + "," + "markcode" + indx);
                        break;
                    case 44:
                        listRecTypes.Add("adp.erp.fin.doc.cash.11" + "," + "markcode" + indx);
                        listRecTypes.Add("adp.erp.fin.doc.cash.12" + "," + "markcode" + indx);
                        break;
                    default:
                        continue;
                }

                for (var i = 0; i < listRecTypes.Count; ++i)
                {
                    //one sp has multi in m-agent
                    //lref shuld be different
                    var newLref = i * 1000000 + lref;

                    tab.Rows.Add(newLref, listRecTypes[i], code, title);

                }
                foreach (var rectype in listRecTypes)
                {
                
                    
                   
                }
            }

            var tab_web = TOOL_WEB.TABLE.CREATE(tab);
            tab_web.TABLENAME = "mcode";
            return tab_web;


        }


        TOOL_WEB.TABLE MY_WRITECLIENT(AGENT pAgent)
        {



            var prm_cl_balance = ISTRUE(pAgent.GET_PRM("prm_magent_cl_balance"));

            var args = new List<object>();


            var sbFilters = new StringBuilder();

            _APPLY_FILTER(pAgent, "prm_magent_cl_filter", new String[] { "CYPHCODE" }, sbFilters, args);

            //IF FILTER EMPTY 
            if (sbFilters.Length == 0)
                return null;


            String col_regbalance = "COALESCE((SELECT LOCGN.DEBIT - LOCGN.CREDIT FROM LG_$FIRM$_$PERIOD$_GNTOTCL LOCGN WITH(NOLOCK) WHERE X.LOGICALREF = LOCGN.CARDREF AND LOCGN.TOTTYP = 1),0) AS REGBALANCE";
            if (!prm_cl_balance) col_regbalance = "CAST(0 as float) as regbalance";

            var sqlText = "SELECT " +
                            "X.LOGICALREF AS LREF,X.CODE AS CODE,X.DEFINITION_ AS TITLE,X.SPECODE AS MARKCODE1,X.SPECODE2 AS MARKCODE2," +
                            "X.DISCRATE AS DISCPERC, " +
                            "\n" +
                            col_regbalance +
                            "\n" +
                            " FROM " +
                            "LG_$FIRM$_CLCARD AS X WHERE " +
                            (sbFilters.ToString()) +
                            " " +
                            " ";



            var tab = _PLUGIN.XSQL(SQLTEXT(sqlText), args.ToArray());

            for (var i = 0; i < tab.Columns.Count; ++i)
            {
                tab.Columns[i].ColumnName = tab.Columns[i].ColumnName.ToLowerInvariant();
            }

            var tab_web = TOOL_WEB.TABLE.CREATE(tab);
            tab_web.TABLENAME = "clcard";
            return tab_web;

        }





        TOOL_WEB.TABLE MY_WRITEITEMS(AGENT pAgent)
        {

            #region OLD
            //            var sql =
            //MY_CHOOSE_SQL(
            //@"
            //
            //declare @wh smallint = @P1
            //declare @dt datetime = @P2
            //declare @agent smallint = @P1
            //
            //
            //
            //SELECT cast(1 AS SMALLINT) PROMO, -- 1 use as promo
            //	(ITM.SPECODE) STGRPCODE,
            //	(ITM.SPECODE2) STGRPCODESUB,
            //	ITM.LOGICALREF LOGICALREF,
            //	ITM.CODE CODE,
            //	ITM.NAME NAME,
            //	isNull(USL1.CODE, '') UNIT1,
            //	isNull(USL2.CODE, '') UNIT2,
            //	isNull(USL3.CODE, '') UNIT3,
            //	isNull(cast(USL1.LOGICALREF AS VARCHAR), '') UNITREF1,
            //	isNull(cast(USL2.LOGICALREF AS VARCHAR), '') UNITREF2,
            //	isNull(cast(USL3.LOGICALREF AS VARCHAR), '') UNITREF3,
            //	isNull(CASE WHEN UINF1.CONVFACT1 > 0 THEN UINF1.CONVFACT2 / UINF1.CONVFACT1 ELSE 0 END, 0) UNITCF1,
            //	isNull(CASE WHEN UINF2.CONVFACT1 > 0 THEN UINF2.CONVFACT2 / UINF2.CONVFACT1 ELSE 0 END, 0) UNITCF2,
            //	isNull(CASE WHEN UINF3.CONVFACT1 > 0 THEN UINF3.CONVFACT2 / UINF3.CONVFACT1 ELSE 0 END, 0) UNITCF3,
            //	isNull(UINF1.WEIGHT, 0) WEIGHT,
            // 
            ///*
            //	ISNULL((
            //		SELECT TOP(1) B.BARCODE
            //		FROM LG_$FIRM$_UNITBARCODE B WITH(NOLOCK)
            //		WHERE B.TYP = 0 AND B.ITEMREF = ITM.LOGICALREF AND B.UNITLINEREF = USL1.LOGICALREF AND B.LINENR = 1
            //		),'') BARCODE1, */
            //
            //
            //	 (ITM.CODE) BARCODE1,
            //
            //            ISNULL((
            //                SELECT TOP (1) PRICE PRICE
            //			    FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK)
            //			    WHERE (STOCKREF = ITM.LOGICALREF AND VARIANTREF >= 0 AND DATE_ >= '19000101' AND 
            //                    FTIME >= 0 AND IOCODE = 1 /*4*/ AND SOURCEINDEX IN (@wh,0)) AND (
            //					    CANCELLED = 0 AND LINETYPE = 0 AND TRCODE IN (1 /*8*/) -- 50 
            //					    )
            //			    ORDER BY STOCKREF DESC,
            //				    VARIANTREF DESC,
            //				    DATE_ DESC,
            //				    FTIME DESC,
            //				    IOCODE DESC,
            //				    SOURCEINDEX DESC,
            //				    LOGICALREF DESC
            //			), 0.0) PRICE1,
            //
            ///*
            //            ISNULL((
            //			    --SELECT TOP (1) ((VATMATRAH + VATAMNT + DISTEXP) / AMOUNT) PRICE
            //                SELECT TOP (1) PRICE PRICE
            //			    FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK)
            //			    WHERE (STOCKREF = ITM.LOGICALREF AND VARIANTREF >= 0 AND DATE_ >= '19000101' AND FTIME >= 0 AND IOCODE = 1 AND SOURCEINDEX = 0) AND (
            //					    CANCELLED = 0 AND LINETYPE = 0 AND TRCODE IN (1) -- 50 
            //					    )
            //			    ORDER BY STOCKREF DESC,
            //				    VARIANTREF DESC,
            //				    DATE_ DESC,
            //				    FTIME DESC,
            //				    IOCODE DESC,
            //				    SOURCEINDEX DESC,
            //				    LOGICALREF DESC
            //			), 0.0) PRICE1,
            //*/
            //
            ///*
            //	ROUND(isNull((
            //			SELECT TOP 1 PL.PRICE
            //			FROM LG_$FIRM$_PRCLIST PL WITH(NOLOCK)
            //			WHERE PL.CARDREF = ITM.LOGICALREF
            //				AND PL.UOMREF = USL1.LOGICALREF
            //				AND @dt BETWEEN PL.BEGDATE AND PL.ENDDATE
            //				AND (PL.PTYPE=1)
            //				AND (PL.PAYPLANREF = 0)
            //			), 0),4) PRICE0,
            //
            //	ROUND(isNull((
            //			SELECT TOP 1 PL.PRICE
            //			FROM LG_$FIRM$_PRCLIST PL WITH(NOLOCK)
            //			WHERE PL.CARDREF = ITM.LOGICALREF
            //				AND PL.UOMREF = USL1.LOGICALREF
            //				AND @dt BETWEEN PL.BEGDATE AND PL.ENDDATE
            //				AND (PL.PTYPE=2)
            //				AND (PL.PAYPLANREF = 0 OR PL.PAYPLANREF IN (SELECT LOGICALREF from LG_$FIRM$_PAYPLANS where CODE in ('','1')))
            //			), 0),4) PRICE1,
            //
            //	ROUND(isNull((
            //			SELECT TOP 1 PL.PRICE
            //			FROM LG_$FIRM$_PRCLIST PL WITH(NOLOCK)
            //			WHERE PL.CARDREF = ITM.LOGICALREF
            //				AND PL.UOMREF = USL1.LOGICALREF
            //				AND @dt BETWEEN PL.BEGDATE AND PL.ENDDATE
            //				AND (PL.PTYPE=2)
            //				AND PL.PAYPLANREF IN (SELECT LOGICALREF from LG_$FIRM$_PAYPLANS where CODE in ('2','2'))
            //			), 0),4) PRICE2,
            //	ROUND(isNull((
            //			SELECT TOP 1 PL.PRICE
            //			FROM LG_$FIRM$_PRCLIST PL WITH(NOLOCK)
            //			WHERE PL.CARDREF = ITM.LOGICALREF
            //				AND PL.UOMREF = USL1.LOGICALREF
            //				AND @dt BETWEEN PL.BEGDATE AND PL.ENDDATE
            //				AND (PL.PTYPE=2)
            //				AND PL.PAYPLANREF IN (SELECT LOGICALREF from LG_$FIRM$_PAYPLANS where CODE in ('3','3'))
            //			), 0),4) PRICE3,
            //	ROUND(isNull((
            //			SELECT TOP 1 PL.PRICE
            //			FROM LG_$FIRM$_PRCLIST PL WITH(NOLOCK)
            //			WHERE PL.CARDREF = ITM.LOGICALREF
            //				AND PL.UOMREF = USL1.LOGICALREF
            //				AND @dt BETWEEN PL.BEGDATE AND PL.ENDDATE
            //				AND (PL.PTYPE=2)
            //				AND PL.PAYPLANREF IN (SELECT LOGICALREF from LG_$FIRM$_PAYPLANS where CODE in ('4','4'))
            //			), 0),4) PRICE4,
            //	ROUND(isNull((
            //			SELECT TOP 1 PL.PRICE
            //			FROM LG_$FIRM$_PRCLIST PL WITH(NOLOCK)
            //			WHERE PL.CARDREF = ITM.LOGICALREF
            //				AND PL.UOMREF = USL1.LOGICALREF
            //				AND @dt BETWEEN PL.BEGDATE AND PL.ENDDATE
            //				AND (PL.PTYPE=2)
            //				AND PL.PAYPLANREF IN (SELECT LOGICALREF from LG_$FIRM$_PAYPLANS where CODE in ('5','5'))
            //			), 0),4) PRICE5,
            //*/
            //
            //	ROUND(isNull(GN.ONHAND, 0),3) ONHAND
            // 
            //FROM LG_$FIRM$_ITEMS ITM WITH(NOLOCK)
            //LEFT OUTER JOIN LG_$FIRM$_$PERIOD$_GNTOTST GN WITH(NOLOCK) ON GN.STOCKREF = ITM.LOGICALREF
            //	AND GN.INVENNO = @wh
            //LEFT OUTER JOIN LG_$FIRM$_UNITSETL USL1 WITH(NOLOCK) ON USL1.UNITSETREF = ITM.UNITSETREF
            //	AND USL1.LINENR = 1
            //LEFT OUTER JOIN LG_$FIRM$_UNITSETL USL2 WITH(NOLOCK) ON USL2.UNITSETREF = ITM.UNITSETREF
            //	AND USL2.LINENR = 2
            //LEFT OUTER JOIN LG_$FIRM$_UNITSETL USL3 WITH(NOLOCK) ON USL3.UNITSETREF = ITM.UNITSETREF
            //	AND USL3.LINENR = 3
            //LEFT OUTER JOIN LG_$FIRM$_ITMUNITA UINF1 WITH(NOLOCK) ON UINF1.ITEMREF = ITM.LOGICALREF
            //	AND UINF1.LINENR = 1
            //LEFT OUTER JOIN LG_$FIRM$_ITMUNITA UINF2 WITH(NOLOCK) ON UINF2.ITEMREF = ITM.LOGICALREF
            //	AND UINF2.LINENR = 2
            //LEFT OUTER JOIN LG_$FIRM$_ITMUNITA UINF3 WITH(NOLOCK) ON UINF3.ITEMREF = ITM.LOGICALREF
            //	AND UINF3.LINENR = 3
            //WHERE 
            //(ACTIVE = 0) --AND
            //--( RTRIM(LTRIM(ITM.SPECODE3)) = '' OR ITM.SPECODE3 like ('%'+cast(@wh as nvarchar)+'%') ) --OR [dbo].[f_SYNC_IS_READER](@wh)=1)
            //
            //
            //--filter by grp and sub-grp
            //
            //",
            // @"
            //
            //--declare @wh smallint = @P1
            //--declare @dt datetime = @P2
            //--declare @agent smallint = @P1
            //
            //
            //
            //SELECT cast(1 AS SMALLINT) PROMO, -- 1 use as promo
            //	(ITM.SPECODE) STGRPCODE,
            //	(ITM.SPECODE2) STGRPCODESUB,
            //	ITM.LOGICALREF LOGICALREF,
            //	ITM.CODE CODE,
            //	ITM.NAME AS NAME,
            //	COALESCE(USL1.CODE, '') UNIT1,
            //	COALESCE(USL2.CODE, '') UNIT2,
            //	COALESCE(USL3.CODE, '') UNIT3,
            //	COALESCE(cast(USL1.LOGICALREF AS VARCHAR), '') UNITREF1,
            //	COALESCE(cast(USL2.LOGICALREF AS VARCHAR), '') UNITREF2,
            //	COALESCE(cast(USL3.LOGICALREF AS VARCHAR), '') UNITREF3,
            //	COALESCE(CASE WHEN UINF1.CONVFACT1 > 0 THEN UINF1.CONVFACT2 / UINF1.CONVFACT1 ELSE 0 END, 0) UNITCF1,
            //	COALESCE(CASE WHEN UINF2.CONVFACT1 > 0 THEN UINF2.CONVFACT2 / UINF2.CONVFACT1 ELSE 0 END, 0) UNITCF2,
            //	COALESCE(CASE WHEN UINF3.CONVFACT1 > 0 THEN UINF3.CONVFACT2 / UINF3.CONVFACT1 ELSE 0 END, 0) UNITCF3,
            //	COALESCE(UINF1.WEIGHT, 0) WEIGHT,
            // 
            ///*
            //	COALESCE((
            //		SELECT B.BARCODE
            //		FROM LG_$FIRM$_UNITBARCODE B 
            //		WHERE B.TYP = 0 AND B.ITEMREF = ITM.LOGICALREF AND B.UNITLINEREF = USL1.LOGICALREF AND B.LINENR = 1
            //		LIMIT 1),'') BARCODE1, */
            //
            //
            //	 (ITM.CODE) BARCODE1,
            //
            //            COALESCE((
            //                SELECT PRICE PRICE
            //			    FROM LG_$FIRM$_$PERIOD$_STLINE 
            //			    WHERE (STOCKREF = ITM.LOGICALREF AND VARIANTREF >= 0 AND DATE_ >= '19000101' AND 
            //                    FTIME >= 0 AND IOCODE = 1 /*4*/ AND SOURCEINDEX IN (@P1,0)) AND (
            //					    CANCELLED = 0 AND LINETYPE = 0 AND TRCODE IN (1 /*8*/) -- 50 
            //					     )
            //			    ORDER BY STOCKREF DESC,
            //				    VARIANTREF DESC,
            //				    DATE_ DESC,
            //				    FTIME DESC,
            //				    IOCODE DESC,
            //				    SOURCEINDEX DESC,
            //				    LOGICALREF DESC
            //                LIMIT 1
            //			), 0.0) PRICE1,
            //
            ///*
            //            COALESCE((
            //			 
            //                SELECT PRICE PRICE
            //			    FROM LG_$FIRM$_$PERIOD$_STLINE 
            //			    WHERE (STOCKREF = ITM.LOGICALREF AND VARIANTREF >= 0 AND DATE_ >= '19000101' AND FTIME >= 0 AND IOCODE = 1 AND SOURCEINDEX = 0) AND (
            //					    CANCELLED = 0 AND LINETYPE = 0 AND TRCODE IN (1) -- 50 
            //					   )
            //			    ORDER BY STOCKREF DESC,
            //				    VARIANTREF DESC,
            //				    DATE_ DESC,
            //				    FTIME DESC,
            //				    IOCODE DESC,
            //				    SOURCEINDEX DESC,
            //				    LOGICALREF DESC
            //                LIMIT 1 
            //			), 0.0) PRICE1,
            //*/
            //
            ///*
            //	ROUND(COALESCE((
            //			SELECT PL.PRICE
            //			FROM LG_$FIRM$_PRCLIST PL 
            //			WHERE PL.CARDREF = ITM.LOGICALREF
            //				AND PL.UOMREF = USL1.LOGICALREF
            //				AND @P2 BETWEEN PL.BEGDATE AND PL.ENDDATE
            //				AND (PL.PTYPE=1)
            //				AND (PL.PAYPLANREF = 0)
            //			LIMIT 1 ), 0)::NUMERIC,4)::FLOAT PRICE0,
            //
            //	ROUND(COALESCE((
            //			SELECT PL.PRICE
            //			FROM LG_$FIRM$_PRCLIST PL 
            //			WHERE PL.CARDREF = ITM.LOGICALREF
            //				AND PL.UOMREF = USL1.LOGICALREF
            //				AND @P2 BETWEEN PL.BEGDATE AND PL.ENDDATE
            //				AND (PL.PTYPE=2)
            //				AND (PL.PAYPLANREF = 0 OR PL.PAYPLANREF IN (SELECT LOGICALREF from LG_$FIRM$_PAYPLANS where CODE in ('','1')))
            //			LIMIT 1), 0)::NUMERIC,4)::FLOAT PRICE1,
            //
            //	ROUND(COALESCE((
            //			SELECT PL.PRICE
            //			FROM LG_$FIRM$_PRCLIST PL 
            //			WHERE PL.CARDREF = ITM.LOGICALREF
            //				AND PL.UOMREF = USL1.LOGICALREF
            //				AND @P2 BETWEEN PL.BEGDATE AND PL.ENDDATE
            //				AND (PL.PTYPE=2)
            //				AND PL.PAYPLANREF IN (SELECT LOGICALREF from LG_$FIRM$_PAYPLANS where CODE in ('2','2'))
            //			LIMIT 1), 0)::NUMERIC,4)::FLOAT PRICE2,
            //	ROUND(COALESCE((
            //			SELECT PL.PRICE
            //			FROM LG_$FIRM$_PRCLIST PL 
            //			WHERE PL.CARDREF = ITM.LOGICALREF
            //				AND PL.UOMREF = USL1.LOGICALREF
            //				AND @P2 BETWEEN PL.BEGDATE AND PL.ENDDATE
            //				AND (PL.PTYPE=2)
            //				AND PL.PAYPLANREF IN (SELECT LOGICALREF from LG_$FIRM$_PAYPLANS where CODE in ('3','3'))
            //			LIMIT 1), 0)::NUMERIC,4)::FLOAT PRICE3,
            //	ROUND(COALESCE((
            //			SELECT PL.PRICE
            //			FROM LG_$FIRM$_PRCLIST PL 
            //			WHERE PL.CARDREF = ITM.LOGICALREF
            //				AND PL.UOMREF = USL1.LOGICALREF
            //				AND @P2 BETWEEN PL.BEGDATE AND PL.ENDDATE
            //				AND (PL.PTYPE=2)
            //				AND PL.PAYPLANREF IN (SELECT LOGICALREF from LG_$FIRM$_PAYPLANS where CODE in ('4','4'))
            //			LIMIT 1), 0)::NUMERIC,4)::FLOAT PRICE4,
            //	ROUND(COALESCE((
            //			SELECT PL.PRICE
            //			FROM LG_$FIRM$_PRCLIST PL 
            //			WHERE PL.CARDREF = ITM.LOGICALREF
            //				AND PL.UOMREF = USL1.LOGICALREF
            //				AND @P2 BETWEEN PL.BEGDATE AND PL.ENDDATE
            //				AND (PL.PTYPE=2)
            //				AND PL.PAYPLANREF IN (SELECT LOGICALREF from LG_$FIRM$_PAYPLANS where CODE in ('5','5'))
            //			LIMIT 1), 0)::NUMERIC,4)::FLOAT PRICE5,
            //*/
            //
            //	ROUND(COALESCE(GN.ONHAND, 0)::NUMERIC,3)::FLOAT ONHAND
            // 
            //FROM LG_$FIRM$_ITEMS ITM  
            //LEFT OUTER JOIN LG_$FIRM$_$PERIOD$_GNTOTST GN ON GN.STOCKREF = ITM.LOGICALREF
            //	AND GN.INVENNO = @P1
            //LEFT OUTER JOIN LG_$FIRM$_UNITSETL USL1 ON USL1.UNITSETREF = ITM.UNITSETREF
            //	AND USL1.LINENR = 1
            //LEFT OUTER JOIN LG_$FIRM$_UNITSETL USL2 ON USL2.UNITSETREF = ITM.UNITSETREF
            //	AND USL2.LINENR = 2
            //LEFT OUTER JOIN LG_$FIRM$_UNITSETL USL3 ON USL3.UNITSETREF = ITM.UNITSETREF
            //	AND USL3.LINENR = 3
            //LEFT OUTER JOIN LG_$FIRM$_ITMUNITA UINF1 ON UINF1.ITEMREF = ITM.LOGICALREF
            //	AND UINF1.LINENR = 1
            //LEFT OUTER JOIN LG_$FIRM$_ITMUNITA UINF2 ON UINF2.ITEMREF = ITM.LOGICALREF
            //	AND UINF2.LINENR = 2
            //LEFT OUTER JOIN LG_$FIRM$_ITMUNITA UINF3 ON UINF3.ITEMREF = ITM.LOGICALREF
            //	AND UINF3.LINENR = 3
            //WHERE 
            //(ACTIVE = 0)  
            // 
            //
            //--filter by grp and sub-grp
            //
            //");

            //            if (OFFLINE)
            //            {
            //                sql += "AND ITM.CODE like '" + FORMAT(pAgent.AGENTNR) + ".%'";
            //                sql += "\n";
            //            }

            //            var list = new List<object>(new object[] { pAgent.AGENTWH, pAgent.AGENTDATE, pAgent.AGENTNR });


            //            var prmIndx = list.Count;
            //            var filters = new Dictionary<string, string[]>() { 
            //            {"ITM.SPECODE",pAgent.MATGROUPSTOP},
            //              {"ITM.SPECODE2",pAgent.MATGROUPSSUB},
            //            };

            //            foreach (string filterCol in filters.Keys)
            //            {

            //                var filterString = "";

            //                var fVals = filters[filterCol];

            //                if (fVals != null && fVals.Length > 0)
            //                {
            //                    filterString += "AND (";
            //                    var first = true;
            //                    var indx = list.Count;
            //                    foreach (var f in pAgent.MATGROUPSTOP)
            //                    {
            //                        ++prmIndx;


            //                        filterString += ((first ? "" : " OR ") + filterCol + " LIKE @P" + prmIndx);

            //                        first = false;
            //                    }
            //                    filterString += ")";

            //                    filterString += "\n";
            //                }


            //                if (!string.IsNullOrEmpty(filterString))
            //                    sql += filterString;


            //            }

            #endregion



            var args = new List<object>();
            var sbFilters = new StringBuilder();




            var prm_onhand_all = ISTRUE(pAgent.GET_PRM("prm_magent_onhand_all"));
            var prm_price0_src = CASTASSTRING(pAgent.GET_PRM("prm_magent_mat_price0_src"));
            var prm_price1_src = CASTASSTRING(pAgent.GET_PRM("prm_magent_mat_price1_src"));
            var prm_wh = CASTASSHORT(pAgent.GET_PRM("prm_magent_agent_wh"));
            //

            //1


            //2
            //sale price as last price from prch or sale

            //3
            //prch price as last price
            //4 get prch price if sale price zero


            var col_price0 = "CAST(0 AS FLOAT) AS PRICE0";
            var col_price1 = "CAST(0 AS FLOAT) AS PRICE1";

            switch (prm_price0_src)
            {
                case "card":
                    col_price0 = @" (COALESCE((
             			SELECT
--$MS$--TOP 1 
                        PL.PRICE
             			FROM LG_$FIRM$_PRCLIST PL WITH(NOLOCK)
             			WHERE PL.CARDREF = X.LOGICALREF  
            				AND (PL.PTYPE=1)
            				AND (PL.PAYPLANREF = 0)
                        ORDER BY PL.ENDDATE DESC
--$PG$--LIMIT 1 
--$SL$--LIMIT 1 
           			), 0) ) as PRICE0";
                    break;
            }

            switch (prm_price1_src)
            {
                case "card":
                    col_price1 = @" (COALESCE((
             			SELECT
--$MS$--TOP 1 
                        PL.PRICE
             			FROM LG_$FIRM$_PRCLIST PL WITH(NOLOCK)
             			WHERE PL.CARDREF = X.LOGICALREF  
            				AND (PL.PTYPE=2)
            				AND (PL.PAYPLANREF = 0)
                        ORDER BY PL.ENDDATE DESC
--$PG$--LIMIT 1 
--$SL$--LIMIT 1 
           			), 0) ) as PRICE1";
                    break;
            }

            var col_regonhand_agent =
                "COALESCE((SELECT GN.ONHAND FROM LG_$FIRM$_$PERIOD$_GNTOTST GN WITH(NOLOCK) WHERE GN.STOCKREF = X.LOGICALREF AND GN.INVENNO = " + prm_wh + "),0.0) AS REGONHANDAGENT";
            var col_regonhand_all = (prm_onhand_all ? "COALESCE((SELECT GN.ONHAND FROM LG_$FIRM$_$PERIOD$_GNTOTST GN WITH(NOLOCK) WHERE GN.STOCKREF = X.LOGICALREF AND GN.INVENNO = -1),0.0) AS REGONHANDALL" : "CAST(0 AS FLOAT) AS REGONHANDALL");


            //2
            _APPLY_FILTER(pAgent, "prm_magent_mat_filter", new string[] { "CYPHCODE" }, sbFilters, args);


            if (sbFilters.Length == 0)
                return null;

            var sqlText = "SELECT " +
                    "X.LOGICALREF AS LREF,X.CODE AS CODE,X.NAME AS TITLE,X.SPECODE AS MARKCODE1,X.SPECODE2 AS MARKCODE2,\n" +
                    col_price0
                    + ",\n" +
                    col_price1
                    + ",\n" +
                    col_regonhand_all
                     + ",\n" +
                    col_regonhand_agent +
                    ",\n" +
                    "X.VAT AS VAT,X.UNIVID AS UNIT " +
                    "from LG_$FIRM$_ITEMS AS X WHERE " +
                    (sbFilters.ToString()) + //rquired filter
                    "";

            var tab = _PLUGIN.XSQL(SQLTEXT(sqlText), args.ToArray());

            for (var i = 0; i < tab.Columns.Count; ++i)
            {
                tab.Columns[i].ColumnName = tab.Columns[i].ColumnName.ToLowerInvariant();
            }

            var tab_web = TOOL_WEB.TABLE.CREATE(tab);
            tab_web.TABLENAME = "mat";
            return tab_web;



        }
        //        void MY_WRITEWAREHOUSE(AGENT pAgent, DataSet pDataSet)
        //        {

        //            var sql =
        //                MY_CHOOSE_SQL(
        //@"
        //SELECT WH.NAME NAME,
        //	WH.NR LOGICALREF,
        //WH.NR 
        //FROM L_CAPIWHOUSE WH WITH(NOLOCK)
        //WHERE WH.FIRMNR = $FIRM$
        //",
        // @"
        //SELECT WH.NAME AS NAME,
        //	WH.NR AS LOGICALREF,
        //WH.NR 
        //FROM L_CAPIWHOUSE WH 
        //WHERE WH.FIRMNR = $FIRM$
        //");

        //            var tabTmp = SQL(sql, new object[] { pAgent.AGENTNR, pAgent.AGENTDATE });


        //            MY_TOGUID(tabTmp, "LOGICALREF");
        //            tabTmp.TableName = "WHOUSE";
        //            pDataSet.Tables.Add(tabTmp);
        //        }
        //void MY_WRITEDOCS(AGENT pAgent, DataSet pDataSet)
        //{

        //}

        //        void MY_WRITEINFOFROMFIRM(AGENT pAgent, DataSet pDataSet)
        //        {
        //            var sql = @"
        // SELECT '' LOGICALREF,
        //	'' CODE
        //";

        //            var tabTmp = SQL(sql, new object[] { pAgent.AGENTNR, pAgent.AGENTDATE });


        //            MY_TOGUID(tabTmp, "LOGICALREF");
        //            tabTmp.TableName = "INFOFIRM";
        //            pDataSet.Tables.Add(tabTmp);
        //        }
        //        void MY_WRITEINFOFROMPERIOD(AGENT pAgent, DataSet pDataSet)
        //        {
        //            var sql = @"
        //SELECT '' LOGICALREF,
        //	'' CODE
        //";

        //            var tabTmp = SQL(sql, new object[] { pAgent.AGENTNR, pAgent.AGENTDATE });


        //            MY_TOGUID(tabTmp, "LOGICALREF");
        //            tabTmp.TableName = "INFOPERIOD";
        //            pDataSet.Tables.Add(tabTmp);
        //        }
        //        void MY_WRITEINFODOCSAVE(AGENT pAgent, DataSet pDataSet)
        //        {
        //            var sql =
        //MY_CHOOSE_SQL(
        //@"
        //declare @dt datetime = @P2
        //
        //SELECT CODE LOGICALREF,
        //	LTRIM(RTRIM(SPECODE)) TYPE,
        //	SPECODE2 CODE,
        //	FLOATF1 CF1,
        //	FLOATF2 CF2,
        //	SPECODE3 PROMOREF,
        //	SPECODE4 FILTERPROMOCL,
        //	TEXTF1 TEXT1,
        //	TEXTF2 TEXT2
        //FROM LG_$FIRM$_CAMPAIGN
        //WHERE CARDTYPE = 2
        //	AND ACTIVE = 0
        //	AND @dt BETWEEN BEGDATE AND ENDDATE
        //	AND SPECODE IN ('coif', 'grp', 'grpMin', 'grpCoif', 'discGlobByCell', 'disc', 'del', 'price')
        //",

        //@"
        //--declare @dt datetime = @P2
        //
        //SELECT CODE LOGICALREF,
        //	LTRIM(RTRIM(SPECODE)) AS TYPE,
        //	SPECODE2 CODE,
        //	FLOATF1 CF1,
        //	FLOATF2 CF2,
        //	SPECODE3 PROMOREF,
        //	SPECODE4 FILTERPROMOCL,
        //	TEXTF1 TEXT1,
        //	TEXTF2 TEXT2
        //FROM LG_$FIRM$_CAMPAIGN
        //WHERE CARDTYPE = 2
        //	AND ACTIVE = 0
        //	AND @P2 BETWEEN BEGDATE AND ENDDATE
        //	AND SPECODE IN ('coif', 'grp', 'grpMin', 'grpCoif', 'discGlobByCell', 'disc', 'del', 'price')
        //");

        //            var tabTmp = SQL(sql, new object[] { pAgent.AGENTNR, pAgent.AGENTDATE });


        //            MY_TOGUID(tabTmp, "LOGICALREF");
        //            tabTmp.TableName = "INFODOCSAVE";
        //            pDataSet.Tables.Add(tabTmp);
        //        }
        //        string MY_TOGUID(string pGuid)
        //        {
        //            return pGuid;
        //        }

        //        void MY_TOGUID(DataTable table, string colName)
        //        {

        //        }

        //        void MY_CHANGE(AGENT pAgent, DataSet pDs)
        //        {

        //            foreach (DataTable table in pDs.Tables)
        //            {

        //                switch (table.TableName)
        //                {
        //                    case "ITEMS":

        //                        if (OFFLINE)
        //                        {
        //                            foreach (DataRow row in table.Rows)
        //                            {
        //                                var name = CASTASSTRING(TAB_GETROW(row, "NAME"));

        //                                var indxCode = name.IndexOf('/');

        //                                if (indxCode > 0)
        //                                {
        //                                    var code = name.Substring(0, indxCode);
        //                                    name = name.Substring(indxCode + 1);

        //                                    TAB_SETROW(row, "CODE", code);
        //                                    TAB_SETROW(row, "BARCODE1", code);


        //                                    //TAB_SETROW(row, "NAME", name);


        //                                }

        //                                var indxGrp = name.IndexOf(' ');
        //                                if (indxGrp > 0)
        //                                {

        //                                    var grp = name.Substring(0, indxGrp);
        //                                    TAB_SETROW(row, "STGRPCODESUB", grp);
        //                                }
        //                                else
        //                                {
        //                                    TAB_SETROW(row, "STGRPCODESUB", LEFT(name, 2));
        //                                }
        //                            }
        //                        }
        //                        break;
        //                }
        //            }
        //        }



        //        static void CHECK_LOC_DS(_PLUGIN pPLUGIN)
        //        {

        //            /*

        //           var date_=   CASTASDATE(pPLUGIN.SQLSCALAR("select getdate()",  null));



        //             */


        //            //10


        //            var currVersionNum = 8;
        //            var currVersionCode = "SYNC_$FIRM$_$PERIOD$";
        //            var dbVers = CASTASINT(pPLUGIN.SQLSCALAR(
        //                "select [dbo].[f_GETOBJVERS]('" + currVersionCode + "')", //has pattern
        //                new object[] { }));


        //            if (dbVers >= currVersionNum)
        //                return;


        //            pPLUGIN.SQL(@"
        //
        //IF ISNULL (
        //		  OBJECT_ID('[dbo].[SYNC_$FIRM$_$PERIOD$_IMPHIS]'),0
        //		) = 0
        //	CREATE TABLE [SYNC_$FIRM$_$PERIOD$_IMPHIS] (
        //		[GUID_] [varchar](50),
        //		[DATE_] [datetime] NULL,
        //		PRIMARY KEY CLUSTERED ([GUID_]) ON [PRIMARY]
        //		) ON [PRIMARY]
        //
        //
        //IF NOT EXISTS (
        //		SELECT *
        //		FROM sysobjects
        //		WHERE id = OBJECT_ID('SYNC_$FIRM$_$PERIOD$_DOCNR')
        //		)
        //	CREATE TABLE SYNC_$FIRM$_$PERIOD$_DOCNR ([LASTLREF] [int] IDENTITY(1, 1) NOT NULL) ON [PRIMARY]
        // 
        //  
        //");
        //            pPLUGIN.SQL(@"
        //IF EXISTS (
        //		SELECT *
        //		FROM sysobjects
        //		WHERE id = OBJECT_ID('dbo.f_SYNC_$FIRM$_AGENT')
        //		)
        //	DROP FUNCTION dbo.f_SYNC_$FIRM$_AGENT
        //
        //
        // 
        //IF EXISTS (
        //		SELECT *
        //		FROM sysobjects
        //		WHERE id = OBJECT_ID('dbo.f_SYNC_$FIRM$_$PERIOD$_CLCARDLASTTRANS')
        //		)
        //	DROP FUNCTION dbo.f_SYNC_$FIRM$_$PERIOD$_CLCARDLASTTRANS
        //
        //
        //IF EXISTS (
        //		SELECT *
        //		FROM sysobjects
        //		WHERE id = OBJECT_ID('dbo.f_SYNC_$FIRM$_CASH')
        //		)
        //	DROP FUNCTION dbo.f_SYNC_$FIRM$_CASH
        //
        //IF EXISTS (
        //		SELECT *
        //		FROM sysobjects
        //		WHERE id = OBJECT_ID('dbo.p_SYNC_$FIRM$_$PERIOD$_DOCNR')
        //		)
        //	DROP PROC dbo.p_SYNC_$FIRM$_$PERIOD$_DOCNR
        //");


        //            pPLUGIN.SQL(@"
        // 
        //CREATE FUNCTION dbo.f_SYNC_$FIRM$_$PERIOD$_CLCARDLASTTRANS (
        //	@clref INT,
        //	@agent INT,
        //	@dt DATETIME
        //	)
        //RETURNS VARCHAR(500)
        //BEGIN
        // 
        //DECLARE @TRANS VARCHAR(500)
        //SELECT TOP(10) @TRANS = ISNULL(@TRANS + ';','') + ('TR'+CAST(TRCODE AS VARCHAR)+',') + LEFT(CONVERT(nvarchar, DATE_,120),10) + ',' + CAST(ROUND(AMOUNT,2) AS VARCHAR) 
        //FROM LG_$FIRM$_$PERIOD$_CLFLINE T WITH(NOLOCK,INDEX=I$FIRM$_$PERIOD$_CLFLINE_I4) 
        //WHERE CLIENTREF = @clref AND DATE_ > '19000101' AND ((MODULENR = 10 AND TRCODE IN (1,2)) OR (MODULENR = 4 AND TRCODE IN (38,33)))
        //ORDER BY CLIENTREF DESC,DATE_ DESC,MODULENR DESC,TRCODE DESC,LOGICALREF DESC
        // 
        // RETURN ISNULL(@TRANS,'')
        //
        //END
        //
        //");


        //            pPLUGIN.SQL(@"
        // 
        //
        //CREATE FUNCTION dbo.f_SYNC_$FIRM$_AGENT ()
        //RETURNS TABLE
        //RETURN SELECT SPECODE AS LOGICALREF,
        //	SPECODE AS NR,
        //	DEFINITION_ AS NAME,
        //	TEXTF1 OPTSTR,
        //	TEXTF2 KEY_
        //FROM LG_$FIRM$_SPECODES3  WITH(NOLOCK)
        //WHERE CODETYPE = 502
        //	AND SPECODETYPE = 26
        //	AND SPECODE BETWEEN 101 AND 32000 
        //
        //");

        //            pPLUGIN.SQL(@"
        // 
        // CREATE FUNCTION dbo.f_SYNC_$FIRM$_CASH ()
        //RETURNS TABLE
        //RETURN
        //SELECT LOGICALREF AS LOGICALREF,
        //	CODE AS CODE,
        //	NAME AS NAME
        //FROM LG_$FIRM$_KSCARD WITH(NOLOCK)
        //
        //");

        //            pPLUGIN.SQL(@"
        // 
        //
        //CREATE PROCEDURE dbo.p_SYNC_$FIRM$_$PERIOD$_DOCNR @LREF INTEGER OUTPUT
        //AS
        //BEGIN
        //	INSERT INTO SYNC_$FIRM$_$PERIOD$_DOCNR DEFAULT
        //	VALUES
        //
        //	SELECT @LREF = SCOPE_IDENTITY()
        //END
        //
        //");




        //            //fix
        //            pPLUGIN.SQL("exec [dbo].[p_SETOBJVERS] '" + currVersionCode + "', @P1", //has pattern
        //                new object[] { currVersionNum });



        //        }




        #endregion

        #region CLASS

        class AGENT
        {
            public AGENT(Dictionary<string, object> pProp)
            {

                AGENTNR = CASTASINT(pProp["AGENTNR"]);
                AGENTWH = CASTASSHORT(pProp["AGENTWH"]);
                AGENTDEP = CASTASSHORT(pProp["AGENTDEP"]);
                //AGENTFOLDER = (string)pProp["AGENTFOLDER"];
                AGENTNAME = CASTASSTRING(pProp["AGENTNAME"]);
                AGENTOPTSTRING = CASTASSTRING(pProp["AGENTOPTSTRING"]);
                AGENTAUTHCODE = CASTASSTRING(pProp["AGENTAUTHCODE"]);
                _PRMS = (Dictionary<string, string>)pProp["AGENTPRMS"];
                //
                AGENTDATE = CASTASDATE(pProp["AGENTDATE"]);
                // AGENTGUID = CASTASSTRING(pProp["AGENTGUID"]);
                //

                MATGROUPSTOP = TOOL.getAgentMatCatGroupsArr(AGENTOPTSTRING);
                MATGROUPSSUB = TOOL.getAgentMatSubCatGroupsArr(AGENTOPTSTRING);

                //
            }

            public int AGENTNR;
            public short AGENTWH;
            public short AGENTDEP;
            // public string AGENTFOLDER;
            public string AGENTNAME;
            public string AGENTOPTSTRING;
            public string AGENTAUTHCODE;

            public DateTime AGENTDATE;
            //public string AGENTGUID;
            //
            public string[] MATGROUPSTOP;
            public string[] MATGROUPSSUB;

            Dictionary<string, string> _PRMS;

            class TOOL
            {

                const char leftCharGrpItem = '(';
                const char rightCharGrpItem = ')';

                const char leftCharSubGrpItem = '<';
                const char rightCharSubGrpItem = '>';

                const char leftCharWh = '[';
                const char rightCharWh = ']';

                const char leftCharMatNumberFilter = '{';
                const char rightCharMatNumberFilter = '}';



                static string getAgentMatCatSeq(string agentOptString)
                {
                    if (agentOptString.Contains(leftCharGrpItem.ToString()) && agentOptString.Contains(rightCharGrpItem.ToString()))
                    {
                        int indxL = agentOptString.IndexOf(leftCharGrpItem);
                        int indxR = agentOptString.IndexOf(rightCharGrpItem);
                        string seq = agentOptString.Substring(indxL, indxR - indxL + 1);
                        seq = seq.Replace(leftCharGrpItem.ToString(), string.Empty).Replace(rightCharGrpItem.ToString(), string.Empty).Trim().Trim(',', ' ').Trim();
                        return seq;
                    }
                    return string.Empty;
                }
                static string getAgentMatSubCatSeq(string agentOptString)
                {
                    if (agentOptString.Contains(leftCharSubGrpItem.ToString()) && agentOptString.Contains(rightCharSubGrpItem.ToString()))
                    {
                        int indxL = agentOptString.IndexOf(leftCharSubGrpItem);
                        int indxR = agentOptString.IndexOf(rightCharSubGrpItem);
                        string seq = agentOptString.Substring(indxL, indxR - indxL + 1);
                        seq = seq.Replace(leftCharSubGrpItem.ToString(), string.Empty).Replace(rightCharSubGrpItem.ToString(), string.Empty).Trim().Trim(',', ' ').Trim();
                        return seq;
                    }
                    return string.Empty;
                }
                public static string[] getAgentMatCatGroupsArr(string agentOptString)
                {
                    List<string> list = new List<string>();
                    string[] matGroups = REMOVEEMPTY(TRIM(EXPLODELIST(getAgentMatCatSeq(agentOptString))));
                    foreach (string cat in matGroups)
                    {
                        list.Add(cat);
                    }
                    return list.ToArray();
                }

                public static string[] getAgentMatSubCatGroupsArr(string agentOptString)
                {
                    List<string> list = new List<string>();
                    string[] matSubGroups = REMOVEEMPTY(TRIM(EXPLODELIST(getAgentMatSubCatSeq(agentOptString))));
                    foreach (string cat in matSubGroups)
                    {
                        list.Add(cat);
                    }


                    return list.ToArray();
                }

                public static string[] getAgentMatNumFilterArr(string agentOptString)
                {
                    const char seqChar_ = '-';
                    List<string> list = new List<string>();
                    string[] matSubGroups = REMOVEEMPTY(TRIM(EXPLODELIST(getAgentNumberFilter(agentOptString))));
                    foreach (string cat in matSubGroups)
                    {
                        if (cat.Contains(seqChar_.ToString()))
                        {
                            try
                            {
                                var arr_ = BREAKLIST(seqChar_, cat);
                                int from_ = PARSEINT(arr_[0]);
                                int to_ = PARSEINT(arr_[1]);
                                if (from_ <= to_)
                                    for (int i = from_; i <= to_; ++i)
                                        list.Add(FORMAT(i));
                            }
                            catch (Exception exc)
                            {
                                RUNTIMELOG(exc.ToString());
                            }
                        }
                        list.Add(cat);
                    }


                    return list.ToArray();
                }

                static string getAgentNumberFilter(string agentOptString)
                {
                    if (agentOptString.Contains(leftCharMatNumberFilter.ToString()) && agentOptString.Contains(rightCharMatNumberFilter.ToString()))
                    {
                        int indxL = agentOptString.IndexOf(leftCharMatNumberFilter);
                        int indxR = agentOptString.IndexOf(rightCharMatNumberFilter);
                        string seq = agentOptString.Substring(indxL, indxR - indxL + 1);
                        seq = seq.Replace(leftCharMatNumberFilter.ToString(), string.Empty).Replace(rightCharMatNumberFilter.ToString(), string.Empty).Trim().Trim(',', ' ').Trim();
                        return seq;
                    }
                    return string.Empty;
                }

                static string[] TRIM(string[] arr)
                {
                    for (int i = 0; i < arr.Length; ++i)
                        arr[i] = arr[i].Trim();
                    return arr;
                }
                static string[] REMOVEEMPTY(string[] arr)
                {
                    List<string> list = new List<string>(arr);
                    for (int i = 0; i < list.Count; ++i)
                    {
                        if (arr[i] == string.Empty)
                        {
                            list.RemoveAt(i);
                            --i;
                        }
                    }
                    return list.ToArray();
                }
                static string[] BREAKLIST(char sep, string str)
                {
                    if (str == null || str == string.Empty)
                        return new string[0];
                    return str.Split(new char[] { sep }, 2);
                }
            }


            public string GET_PRM(string pName, string pDef = "")
            {
                return GETDIC(_PRMS, pName, pDef);
            }

            public string[] GET_PRM_ALL()
            {
                return new List<string>(_PRMS.Keys).ToArray();
            }

        }


        class MY_DATASOURCE
        {

            static string VERSION_CODE = "SYNC_$FIRM$_$PERIOD$";
            static int VERSION_INDX = 8;


            public static void UPDATE(_PLUGIN pPLUGIN)
            {
                if (ISVERSONOK(pPLUGIN))
                    return;


                //                UPDATE(pPLUGIN,
                //                @"
                //IF ISNULL (
                //		  OBJECT_ID('[dbo].[SYNC_$FIRM$_$PERIOD$_IMPHIS]'),0
                //		) = 0
                //	CREATE TABLE [SYNC_$FIRM$_$PERIOD$_IMPHIS] (
                //		[GUID_] [varchar](50),
                //		[DATE_] [datetime] NULL,
                //		PRIMARY KEY CLUSTERED ([GUID_]) ON [PRIMARY]
                //		) ON [PRIMARY]
                //
                //",
                //                 @"
                // 
                //	CREATE TABLE IF NOT EXISTS SYNC_$FIRM$_$PERIOD$_IMPHIS (
                //		 GUID_ varchar(50),
                //		 DATE_ TIMESTAMP(0) NULL,
                //		PRIMARY KEY ( GUID_ ) 
                //		)  
                //");

                //                UPDATE(pPLUGIN,
                //                @"
                // 
                //IF NOT EXISTS (
                //		SELECT *
                //		FROM sysobjects
                //		WHERE id = OBJECT_ID('SYNC_$FIRM$_$PERIOD$_DOCNR')
                //		)
                //	CREATE TABLE SYNC_$FIRM$_$PERIOD$_DOCNR ([LASTLREF] [int] IDENTITY(1, 1) NOT NULL) ON [PRIMARY]
                // 
                //",
                //                 @"
                // 
                //	CREATE SEQUENCE  IF NOT EXISTS SYNC_$FIRM$_$PERIOD$_DOCNR 
                // 
                //");



                //                UPDATE(pPLUGIN,
                //                @"
                ///*
                //    IF EXISTS (
                //		SELECT *
                //		FROM sysobjects
                //		WHERE id = OBJECT_ID('dbo.f_SYNC_$FIRM$_AGENT')
                //		)
                //	DROP FUNCTION dbo.f_SYNC_$FIRM$_AGENT
                //*/
                //
                //
                // 
                //IF EXISTS (
                //		SELECT *
                //		FROM sysobjects
                //		WHERE id = OBJECT_ID('dbo.f_SYNC_$FIRM$_$PERIOD$_CLCARDLASTTRANS')
                //		)
                //	DROP FUNCTION dbo.f_SYNC_$FIRM$_$PERIOD$_CLCARDLASTTRANS
                //
                //
                //IF EXISTS (
                //		SELECT *
                //		FROM sysobjects
                //		WHERE id = OBJECT_ID('dbo.f_SYNC_$FIRM$_CASH')
                //		)
                //	DROP FUNCTION dbo.f_SYNC_$FIRM$_CASH
                //
                //IF EXISTS (
                //		SELECT *
                //		FROM sysobjects
                //		WHERE id = OBJECT_ID('dbo.p_SYNC_$FIRM$_$PERIOD$_DOCNR')
                //		)
                //	DROP PROC dbo.p_SYNC_$FIRM$_$PERIOD$_DOCNR
                //",
                //                 @"
                // 
                ///*
                //	DROP FUNCTION IF EXISTS f_SYNC_$FIRM$_AGENT();
                //
                //
                //	DROP FUNCTION IF EXISTS f_SYNC_$FIRM$_$PERIOD$_CLCARDLASTTRANS( INT, INT, TIMESTAMP(0) );
                //
                // 
                //	DROP FUNCTION IF EXISTS f_SYNC_$FIRM$_CASH();
                // */
                // 
                //");

                //                UPDATE(pPLUGIN,
                //                @"
                // 
                //CREATE FUNCTION dbo.f_SYNC_$FIRM$_$PERIOD$_CLCARDLASTTRANS (
                //	@clref INT,
                //	@agent INT,
                //	@dt DATETIME
                //	)
                //RETURNS VARCHAR(500)
                //BEGIN
                // 
                //DECLARE @TRANS VARCHAR(500)
                //SELECT TOP(10) @TRANS = ISNULL(@TRANS + ';','') + ('TR'+CAST(TRCODE AS VARCHAR)+',') + LEFT(CONVERT(nvarchar, DATE_,120),10) + ',' + CAST(ROUND(AMOUNT,2) AS VARCHAR) 
                //FROM LG_$FIRM$_$PERIOD$_CLFLINE T WITH(NOLOCK,INDEX=I$FIRM$_$PERIOD$_CLFLINE_I4) 
                //WHERE CLIENTREF = @clref AND DATE_ > '19000101' AND ((MODULENR = 10 AND TRCODE IN (1,2)) OR (MODULENR = 4 AND TRCODE IN (38,33)))
                //ORDER BY CLIENTREF DESC,DATE_ DESC,MODULENR DESC,TRCODE DESC,LOGICALREF DESC
                // 
                // RETURN ISNULL(@TRANS,'')
                //
                //END
                //",
                //                 @"
                //
                //CREATE OR REPLACE FUNCTION  f_SYNC_$FIRM$_$PERIOD$_CLCARDLASTTRANS (
                //	_clref INT,
                //	_agent INT,
                //	_dt TIMESTAMP(0)
                //	)
                //RETURNS VARCHAR(500)
                //as
                //$$ 
                //DECLARE 
                //_TRANS VARCHAR(500)=NULL;
                //
                //BEGIN
                // 
                //SELECT string_agg(TRAN,';') INTO _TRANS FROM
                //(
                //	SELECT  
                //	(
                //	('TR'||CAST(TRCODE AS VARCHAR)||',') || 
                //	LEFT(DATE_::VARCHAR,10) || 
                //	',' || 
                //	CAST(ROUND(AMOUNT::NUMERIC,2) AS VARCHAR) 
                //	)  TRAN
                //	 FROM LG_$FIRM$_$PERIOD$_CLFLINE T  
                //	WHERE CLIENTREF = _clref AND DATE_ > '19000101' AND ((MODULENR = 10 AND TRCODE IN (1,2)) OR (MODULENR = 4 AND TRCODE IN (38,33)))
                //	ORDER BY CLIENTREF DESC,DATE_ DESC,MODULENR DESC,TRCODE DESC,LOGICALREF DESC 
                //	LIMIT 10
                // ) TMP;
                // 
                // RETURN COALESCE(_TRANS,'');
                //
                //end $$ language plpgsql;
                //
                //");


                /*
                  
                 
                UPDATE(pPLUGIN,
                @"
 CREATE FUNCTION dbo.f_SYNC_$FIRM$_AGENT ()
RETURNS TABLE
RETURN SELECT SPECODE AS LOGICALREF,
	SPECODE AS NR,
	DEFINITION_ AS NAME,
	TEXTF1 OPTSTR,
	TEXTF2 KEY_
FROM LG_$FIRM$_SPECODES3  WITH(NOLOCK)
WHERE CODETYPE = 502
	AND SPECODETYPE = 26
	AND SPECODE BETWEEN 101 AND 32000 

",
                 @"

CREATE  OR REPLACE FUNCTION f_SYNC_$FIRM$_AGENT ()
RETURNS TABLE (
 LOGICALREF INT ,
 NR INT ,
 NAME VARCHAR(50) ,
 OPTSTR VARCHAR(50) ,
 KEY_ VARCHAR (50)

)
as
$$ 
SELECT SPECODE  AS LOGICALREF,
	SPECODE  AS NR,
	DEFINITION_  AS NAME,
	TEXTF1  OPTSTR,
	TEXTF2  KEY_
FROM LG_$FIRM$_SPECODES3 
WHERE CODETYPE = 502
	AND SPECODETYPE = 26
	AND SPECODE BETWEEN 101 AND 32000 ;

  $$ language sql;


");

                */

                //                UPDATE(pPLUGIN,
                //                @"
                //  CREATE FUNCTION dbo.f_SYNC_$FIRM$_CASH ()
                //RETURNS TABLE
                //RETURN
                //SELECT LOGICALREF AS LOGICALREF,
                //	CODE AS CODE,
                //	NAME AS NAME
                //FROM LG_$FIRM$_KSCARD WITH(NOLOCK)
                //
                //",
                //                 @"
                // CREATE  OR REPLACE FUNCTION f_SYNC_$FIRM$_CASH ()
                //RETURNS TABLE (
                // LOGICALREF INT ,
                // CODE VARCHAR(50) ,
                // NAME VARCHAR(50) 
                //
                //)
                //as
                //$$ 
                //SELECT LOGICALREF AS LOGICALREF,
                //	CODE AS CODE,
                //	NAME AS NAME
                //FROM LG_$FIRM$_KSCARD 
                // 
                //
                //  $$ language sql;
                //");

                //                UPDATE(pPLUGIN,
                //                @"
                // 
                //
                //CREATE PROCEDURE dbo.p_SYNC_$FIRM$_$PERIOD$_DOCNR @LREF INTEGER OUTPUT
                //AS
                //BEGIN
                //	INSERT INTO SYNC_$FIRM$_$PERIOD$_DOCNR DEFAULT
                //	VALUES
                //
                //	SELECT @LREF = SCOPE_IDENTITY()
                //END
                //
                //",
                //                 @"
                // 
                // 
                //
                //");


                SETVERSION(pPLUGIN);
            }

            static bool ISVERSONOK(_PLUGIN pPLUGIN)
            {
                var currVersionNum = VERSION_INDX;
                var currVersionCode = VERSION_CODE;
                var dbVers = CASTASINT(pPLUGIN.SQLSCALAR(
                    MY_CHOOSE_SQL(
                    "select dbo.f_GETOBJVERS('" + currVersionCode + "')", //has pattern
                     "select f_GETOBJVERS('" + currVersionCode + "')"  //has pattern
                     ),
                    new object[] { }));


                return (dbVers >= currVersionNum);

            }

            static void SETVERSION(_PLUGIN pPLUGIN)
            {
                pPLUGIN.SQL(
                  MY_CHOOSE_SQL(
                  ("exec dbo.p_SETOBJVERS '" + VERSION_CODE + "', @P1"),
                  ("select p_SETOBJVERS ('" + VERSION_CODE + "', @P1)")
                  ), //has pattern
              new object[] { VERSION_INDX });

            }


            static void UPDATE(_PLUGIN pPLUGIN, string pMsSql, string pPgSql)
            {

                pPLUGIN.SQL(MY_CHOOSE_SQL(pMsSql, pPgSql));

            }

        }

        #endregion


        #region TOOLS


        static string SQLTEXT(string pSqlText)
        {
            var dic = new Dictionary<string, string>(){
              {"$FIRM$",_SETTINGS.BUF._FIRM.ToString().PadLeft(3,'0')},
              {"$PERIOD$",_SETTINGS.BUF._PERIOD.ToString().PadLeft(2,'0')},
          };

            foreach (var itm in dic)
                pSqlText = pSqlText.Replace(itm.Key, itm.Value);

            return pSqlText;
        }

        public static string MY_CHOOSE_SQL(string pSqlMs, string pSqlPg)
        {

            if (ISMSSQL())
                return pSqlMs;

            if (ISPOSTGRESQL())
                return pSqlPg;


            throw new Exception("Undefined datasource");
        }


        public static string MOB_SYS_AGENTSEQRESEED(_PLUGIN pPLUGIN, short pAgent)
        {
            var arr = AGENT_LREF_MIN_MAX(pAgent);

            var data = pPLUGIN.SQL(
 @"
SELECT 'CLCARD' AS NAME, MAX(LOGICALREF) AS LREF FROM LG_$FIRM$_CLCARD WHERE LOGICALREF BETWEEN @P1 AND @P2
UNION
SELECT 'ITEMS' AS NAME, MAX(LOGICALREF) AS LREF FROM LG_$FIRM$_ITEMS WHERE LOGICALREF BETWEEN @P1 AND @P2
",
 new object[] { arr[0], arr[1] }
             );

            var list = new List<string>();

            foreach (DataRow r in data.Rows)
            {
                var name = CASTASSTRING(TAB_GETROW(r, "NAME"));
                var lref = CASTASINT(TAB_GETROW(r, "LREF"));

                list.Add(FORMAT(name));
                list.Add(FORMAT(lref));

            }

            return JOINLIST(list.ToArray());
        }

        public static int[] AGENT_LREF_MIN_MAX(short pAgent)
        {

            var _agent = (int)pAgent;

            _agent = _agent << (4 + 8 + 8);

            var min = _agent;
            var max = _agent | 0x000FFFFF;



            return new int[] { min, max };
        }


        #endregion
        #endregion
