#line 2



    #region PLUGIN_BODY
        const int VERSION = 5;

        const string FILE = "plugin.sys.event.pricing.fromprch.pls";



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

                    x.MY_PRICING_FROMPRCH_USER = s.MY_PRICING_FROMPRCH_USER;

                    //

                    _SETTINGS.BUF = x;

                }

                public string MY_PRICING_FROMPRCH_USER;



            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC) //, "ava.production.config")
            {

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active On User")]
            public string MY_PRICING_FROMPRCH_USER
            {
                get
                {
                    return (_GET("MY_PRICING_FROMPRCH_USER", "1,2"));
                }
                set
                {
                    _SET("MY_PRICING_FROMPRCH_USER", value);
                }

            }



            //

            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_PRICING_FROMPRCH_USER),
                     FORMAT(pPLUGIN.GETSYSPRM_USER())
                     ) >= 0;
            }

        }

        #endregion



        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "Pricing From Prch";

             
            public const string text_UPDATE = "Update price from Prch Invoice";

         
            public class L
            {

            }
        }



        const string event_PRICING_ = "_pricing_";
        const string event_PRICING_FROMPRCH_ = "_pricing_fromprch_";
        const string event_PRICING_FROMPRCH_UPDATE = "_pricing_fromprch_update";
        const string event_PRICING_FROMPRCH_UPDATE_OUTCALL = "_pricing_fromprch_update_outcall";



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



            public string UPDATE_PRICE = "Price Update";
            public   string  UPDATE_PRICE_FROM_PRCH_INV = "Update price from Prch Invoice";

  
            public void lang_az()
            {

                UPDATE_PRICE = "Qiymət Təzələmə";
                UPDATE_PRICE_FROM_PRCH_INV = "Qiymət Təzələmə Alışdan";
            }

            public void lang_ru()
            {

                UPDATE_PRICE = "Обновить Цены";
                UPDATE_PRICE_FROM_PRCH_INV = "Обновить Цены из Закупки";
            }

            public void lang_tr()
            {


                
            }
        }



        #endregion

        #region MAIN



        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {

            if (ISWEB())
                return;


            _SETTINGS._BUF.LOAD_SETTINGS(this);


            if (!_SETTINGS.ISUSEROK(this))
                return;

            CHECK_LOC_DS(this);

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
                var tree = CONTROL_SEARCH(FORM, "cTreeTools");
                if (tree != null)
                {

                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
			{ "Text" ,_LANG.L.UPDATE_PRICE},
			{ "ImageName" ,"money_32x32"},
			{ "Name" ,event_PRICING_},
            };

                        RUNUIINTEGRATION(tree, args);

                    }


                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_PRICING_},
			{ "CmdText" ,"event name::"+event_PRICING_FROMPRCH_UPDATE},
			{ "Text" ,_LANG.L.UPDATE_PRICE_FROM_PRCH_INV},
			{ "ImageName" ,"refresh_32x32"},
		 { "Name" , event_PRICING_FROMPRCH_UPDATE},
            };

                        RUNUIINTEGRATION(tree, args);

                    }













                }
                return;

            }
            var cPanelBtnSub = CONTROL_SEARCH(FORM, "cPanelBtnSub");




            if (cPanelBtnSub == null)
                return;







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


                    case event_PRICING_FROMPRCH_UPDATE:

                        MY_PRICING_FROMPRCH_UPDATE(false);

                        break;

                    case event_PRICING_FROMPRCH_UPDATE_OUTCALL:

                        MY_PRICING_FROMPRCH_UPDATE(true);

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



        //call from other plugin

        void MY_PRICING_FROMPRCH_UPDATE(bool pOutCall)
        {


            if (!MSGUSERASK("T_MSG_COMMIT_BEGIN - " + _LANG.L.UPDATE_PRICE_FROM_PRCH_INV))
                return;




            // PRM.UPDATE_PRCH_PRICE_IN_CARD = true;
            //  PRM.UPDATE_PRICE_COIF = false;

            var date = MY_ASK_DATE(this, "T_DATE", DateTime.Now.Date).Date;

            if (ISEMPTY(date))
                return;

            
            //last prices
            var sql = @"


SELECT LOGICALREF,
	PRICEDOC PRICE
FROM (
	SELECT LOGICALREF,
		COALESCE((
				SELECT
					--$MS$--TOP(1) 
					((VATMATRAH + VATAMNT + DISTEXP) / AMOUNT) PRICE
				FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK)
				WHERE (
						STOCKREF = ITEMS.LOGICALREF
						AND VARIANTREF >= 0
						AND (DATE_ >= '19000101' AND DATE_ <= @P1)
						AND FTIME >= 0
						AND IOCODE = 1
						AND SOURCEINDEX = 0
						)
					AND (
						CANCELLED = 0
						AND LINETYPE = 0
						AND TRCODE IN (1) -- 50 
						)
				ORDER BY STOCKREF DESC,
					VARIANTREF DESC,
					DATE_ DESC,
					FTIME DESC,
					IOCODE DESC,
					SOURCEINDEX DESC,
					LOGICALREF DESC
					
					--$PG$--LIMIT 1
					--$SL$--LIMIT 1
					
				), 0.0) PRICEDOC,
		COALESCE((
				SELECT
					--$MS$--TOP(1) 
					P.PRICE
				FROM LG_$FIRM$_PRCLIST P WITH(NOLOCK)
				WHERE P.CARDREF = ITEMS.LOGICALREF
					AND P.PTYPE = 1
				ORDER BY P.ENDDATE DESC
					--$PG$--LIMIT 1
					--$SL$--LIMIT 1
				), 0.0) PRICECARD
	FROM LG_$FIRM$_ITEMS ITEMS
	) T
WHERE ABS(T.PRICEDOC - T.PRICECARD) > 0.001
	AND T.PRICEDOC > 0.001


";


            var data = SQL(sql, new object[] { date });
            var me = this;

            INVOKEINBATCH((s, e) =>
            {

                foreach (DataRow r in data.Rows)
                {

                    var lref = TAB_GETROW(r, "LOGICALREF");
                    var price = CASTASDOUBLE(TAB_GETROW(r, "PRICE"));

                    (new MY_SAVEPRICE(me) { pricePrch = price, matRef = lref }).RUN();
                }

            }, null);





            if (!pOutCall)
                MSGUSERINFO("T_MSG_OPERATION_OK - " + _LANG.L.UPDATE_PRICE_FROM_PRCH_INV);

        }




        #region SQL


        #endregion
        #endregion












        #region Help


        public static DateTime MY_ASK_DATE(_PLUGIN pPLUGIN, string pMsg, DateTime pDef )
        {

            if (pDef == null)
                pDef = DateTime.Now;

            DataRow[] rows_ = pPLUGIN.REF("ref.gen.date desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
            if (rows_ != null && rows_.Length > 0)
            {
                return _PLUGIN.CASTASDATE(rows_[0]["DATE_"]);
            }
            return new DateTime(1900,1,1);
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
            return CASTASDATE(pPLUGIN.SQLSCALAR("select getdate()", null));
        }

        static bool CHECK_DATA_IS_OLD(_PLUGIN pPLUGIN)
        {
            var dateLastValid_ = GET_SERVER_DATE(pPLUGIN).Date.AddDays(-1);
            var dateLastDb_ = CASTASDATE(pPLUGIN.SQLSCALAR("select DATE_ from OLAP_$FIRM$_LAST_DATE where CODE = 'ALL'", null)).Date;

            return (dateLastDb_ < dateLastValid_);
        }
        static void CHECK_LOC_DS(_PLUGIN pPLUGIN)
        {




        }








        #endregion




        #endregion




        #region CLASS

        class MY_SAVEPRICE : IDisposable
        {

            _PLUGIN PLUGIN;




            public double priceSls;
            public double pricePrch;

            public object matRef;




            string adp_ = "adp.mm.rec.mat";


            public MY_SAVEPRICE(_PLUGIN pPLUGIN)
            {

                PLUGIN = pPLUGIN;

            }

            public void RUN()
            {

                if (!ISEMPTYLREF(matRef) && (!ISNUMZERO(priceSls) || !ISNUMZERO(pricePrch)))
                {

                    var suf_ = FORMAT(ISNULL(PLUGIN.SQLSCALAR("SELECT CARDTYPE FROM LG_$FIRM$_ITEMS I WITH(NOLOCK) WHERE LOGICALREF=@P1", new object[] { matRef }), 1));

                    PLUGIN.EXEADPCMD(new string[] { adp_ + "/" + suf_ + " cmd::edit lref::" + _PLUGIN.FORMAT(matRef) }, new DoWorkEventHandler[] { DONE }, true);//in global batch


                }
            }
            bool UPDATEPRCH()
            {
                return true;

            }


            public void DONE(object sender, System.ComponentModel.DoWorkEventArgs e)
            {

                e.Result = false;

                short pTypePrch_ = 1;
                short pTypeSls_ = 2;

                DataSet doc_ = ((DataSet)e.Argument);

                DataTable mats_ = TAB_GETTAB(doc_, "ITEMS");
                DataRow matRec_ = TAB_GETLASTROW(mats_);
                DataTable tabPrice_ = TAB_GETTAB(doc_, "PRCLIST");


                List<DataRow> lDelP = new List<DataRow>();
                List<DataRow> lDelS = new List<DataRow>();

                double priceSls_ = Math.Round(priceSls, 2);
                double pricePrch_ = Math.Round(pricePrch, 4);

                DataRow rowPrch = null;
                DataRow rowSls = null;

                foreach (DataRow row in tabPrice_.Rows)
                {
                    if (!TAB_ROWDELETED(row) && COMPARE(pTypeSls_, TAB_GETROW(row, "PTYPE")))
                        lDelS.Add(rowSls = row);


                    if (UPDATEPRCH())
                        if (!TAB_ROWDELETED(row) && COMPARE(pTypePrch_, TAB_GETROW(row, "PTYPE")))
                            lDelP.Add(rowPrch = row);
                }

                {
                    if (lDelP.Count > 0)
                        lDelP.RemoveAt(lDelP.Count - 1);
                    if (lDelS.Count > 0)
                        lDelS.RemoveAt(lDelS.Count - 1);

                    foreach (DataRow row in lDelP)
                        row.Delete();
                    foreach (DataRow row in lDelS)
                        row.Delete();
                }

                if (!ISNUMZERO(priceSls_))
                {
                    if (rowSls == null)
                    {
                        rowSls = tabPrice_.NewRow();
                        tabPrice_.Rows.Add(rowSls);
                    }
                    TAB_SETROW(rowSls, "PTYPE", pTypeSls_);
                    TAB_SETROW(rowSls, "PRICE", priceSls_);
                }

                if (UPDATEPRCH())
                {


                    if (ISNUMZERO(pricePrch_))
                    {
                        pricePrch_ = CASTASDOUBLE(ISNULL(PLUGIN.SQLSCALAR(@"

SELECT 
--$MS$--TOP(1)  
((VATMATRAH+VATAMNT+DISTEXP)/AMOUNT) PRICE 
FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK) 
WHERE 
STOCKREF = @P1 AND 
TRCODE = 1 AND 
CANCELLED=0 AND 
LINETYPE = 0 
ORDER BY STOCKREF DESC,DATE_ DESC
--$PG$--LIMIT 1
--$SL$--LIMIT 1
",

                            new object[] { matRef }), 0));
                    }

                    pricePrch_ = ROUND(pricePrch_, 4);


                    if (rowPrch == null)
                    {
                        rowPrch = tabPrice_.NewRow();
                        tabPrice_.Rows.Add(rowPrch);
                    }

                    TAB_SETROW(rowPrch, "PTYPE", pTypePrch_);
                    TAB_SETROW(rowPrch, "PRICE", pricePrch_);




                }




                e.Result = true;
            }




            public void Dispose()
            {


                PLUGIN = null;

            }

        }


        #endregion


        #endregion
