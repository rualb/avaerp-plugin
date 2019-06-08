#line 2



    #region PLUGIN_BODY
        const int VERSION = 11;

        const string FILE = "plugin.sys.event.barcodeterm.pricing.pls";



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

                    x.MY_BARCODETERM_USER = s.MY_BARCODETERM_USER;

                    //

                    _SETTINGS.BUF = x;

                }

                public string MY_BARCODETERM_USER;



            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC) //, "ava.production.config")
            {

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active On User")]
            public string MY_BARCODETERM_USER
            {
                get
                {
                    return (_GET("MY_BARCODETERM_USER", "1"));
                }
                set
                {
                    _SET("MY_BARCODETERM_USER", value);
                }

            }



            //

            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_BARCODETERM_USER),
                     FORMAT(pPLUGIN.GETSYSPRM_USER())
                     ) >= 0;
            }

        }

        #endregion



        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "POS Pricing";


            public class L
            {

            }
        }


        const string event_BARCODETERM_ = "_barcodeterm_";
        const string event_BARCODETERM_PRICING = "_barcodeterm_pricing_do";

        #endregion

        #region MAIN



        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {

            if (ISWEB())
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
                var tree = CONTROL_SEARCH(FORM, "cTreeTools");
                if (tree != null)
                {

                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
			{ "Text" ,"POS"},
			{ "ImageName" ,"folder_32x32"},
			{ "Name" ,event_BARCODETERM_},
            };

                        RUNUIINTEGRATION(tree, args);

                    }


                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_BARCODETERM_},
			{ "CmdText" ,"event name::"+event_BARCODETERM_PRICING},
			{ "Text" ,"T_PRICE"},
			{ "ImageName" ,"money_32x32"},
            { "Name" ,event_BARCODETERM_PRICING},
            
            };

                        RUNUIINTEGRATION(tree, args);

                    }











                }
                return;

            }


            if (fn == "adp.mm.rec.mat")
            {

                var f1 = CONTROL_SEARCH(FORM, "cFloatF1");
                if (f1 != null)
                    f1.Enabled = false;

                var dsMat = GETDATASETFROMADPFORM(FORM);


                new ADP_STOCK_CARD_FORM_HANDLER(this, FORM, dsMat, dsMat.Tables["ITEMS"], dsMat.Tables["PRCLIST"]);

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

                string cmd_ = list_.Length > 1 ? list_[1].ToLowerInvariant() : "";

                switch (cmd_)
                {
                    case "genplu":
                        {
                            var ctrl_ = arg2 as Control;
                            if (ctrl_ != null)
                            {
                                var f = ctrl_.FindForm();
                                if (f != null)
                                {
                                    var ds_ = GETDATASETFROMADPFORM(f);
                                    if (ds_ != null)
                                    {
                                        var plu_ = CASTASINT(ISNULL(TAB_GETROW(ds_.Tables["ITEMS"], "INTF2"), 0));
                                        if (plu_ != 0)
                                        {
                                            // MSGUSERINFO("PLU not empty !");
                                            return;
                                        }
                                        plu_ = (CASTASINT(ISNULL(SQLSCALAR(
                                        @"
		   declare @p int,@indx int
		   
		   SELECT @indx=0,@p = MAX(INTF2) FROM LG_$FIRM$_ITEMS WITH(NOLOCK) 
		   select @p = isnull(@p,0)
		   
		   while @indx < 100
		   begin
		   select @p = @p + @indx
		   if not exists(select 1 from  LG_$FIRM$_ITEMS WITH(NOLOCK) where INTF2 = @p )
		     select @indx=10000
		   select @indx = @indx+1
		   end
		   select @p PLU
		   "

                                        , new object[] { }), 0)));
                                        TAB_SETROW(ds_.Tables["ITEMS"], "INTF2", plu_);
                                        TAB_SETROW(ds_.Tables["ITEMS"], "INTF1", 1);

                                    }
                                }
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




        #endregion




        #region CLAZZ

        class ADP_STOCK_CARD_FORM_HANDLER : IDisposable
        {



            Form FORM;

            DataTable ITEMS;
            DataTable PRCLIST;

            _PLUGIN PLUGIN;
            DataSet DATASET;

            public ADP_STOCK_CARD_FORM_HANDLER(_PLUGIN pPLUGIN, Form pFORM, DataSet pDATASET, DataTable pITEMS, DataTable pPRCLIST)
            {
                if (pFORM == null || pDATASET == null || pITEMS == null || pPRCLIST == null)
                    throw new ArgumentNullException();

                PLUGIN = pPLUGIN;

                FORM = pFORM;


                ITEMS = pITEMS;
                PRCLIST = pPRCLIST;

                DATASET = pDATASET;



                FORM.Disposed += FORM_DISPOSED;


                new MY_RETAIL_MAT_PERCENT(PLUGIN, ITEMS, PRCLIST);

            }
            //

            class MY_RETAIL_MAT_PERCENT
            {


                string FLOATF1 = "FLOATF1"; //percent
                string PRICE = "PRICE"; //price

                short PRCSLSTYPE = 2;
                short PRCPRCHTYPE = 1;

                DataTable ITEMS;
                DataTable PRCLIST;
                _PLUGIN PLUGIN;

                public MY_RETAIL_MAT_PERCENT(_PLUGIN pPLUGIN, DataTable pITEMS, DataTable pPRCLIST)
                {

                    ITEMS = pITEMS;
                    PRCLIST = pPRCLIST;
                    PLUGIN = pPLUGIN;


                    EVENT_COLUMN_CHANGED(ITEMS, FLOATF1, HANDLER_ROW);
                    EVENT_COLUMN_CHANGED(PRCLIST, PRICE, HANDLER_ROW);

                    //

                    //
                }

                bool locked_ = false;

                void HANDLER_ROW(DataTable pTable, string pColumn, DataRow pRow, object pValue)
                {

                    if (locked_)
                        return;

                    try
                    {

                        locked_ = true;

                        if (pRow == null)
                            return;


                        var rm = GET_MAT_REC();
                        var rp = GET_MAT_PRICE_REC(PRCPRCHTYPE);
                        var rs = GET_MAT_PRICE_REC(PRCSLSTYPE);


                        var coif_ = CASTASDOUBLE(TAB_GETROW(rm, FLOATF1)) / 100;
                        var prch_ = CASTASDOUBLE(TAB_GETROW(rp, PRICE));
                        var sls_ = CASTASDOUBLE(TAB_GETROW(rs, PRICE));


                        if (pColumn == FLOATF1)
                        {
                            TAB_SETROW(rs, PRICE, prch_ * (1 + coif_));
                        }
                        else
                        {
                            var ptype_ = CASTASSHORT(TAB_GETROW(pRow, "PTYPE"));
                            if (ptype_ == PRCPRCHTYPE)
                            {
                                TAB_SETROW(rs, PRICE, prch_ * (1 + coif_));
                            }
                            else
                                if (ptype_ == PRCSLSTYPE)
                                {
                                    if (!ISNUMZERO(prch_))
                                        TAB_SETROW(rm, FLOATF1, Math.Round(100 * (sls_ - prch_) / prch_, 2));
                                }


                        }

                    }
                    finally
                    {
                        locked_ = false;
                    }
                }

                DataRow GET_MAT_REC()
                {

                    return TAB_GETLASTROW(ITEMS);
                }
                DataRow GET_MAT_PRICE_REC(short pType)
                {

                    if (pType != PRCSLSTYPE && pType != PRCPRCHTYPE)
                        throw new Exception("Price type incorrect [" + pType + "]");


                    var row_ = TAB_SEARCH(PRCLIST, "PTYPE", pType);
                    if (row_ == null)
                    {
                        row_ = PRCLIST.NewRow();
                        PRCLIST.Rows.Add(row_);

                        TAB_SETROW(row_, "PTYPE", pType);
                    }
                    return row_;
                }

            }




            void FORM_DISPOSED(object sender, EventArgs e)
            {
                Dispose();
            }





            public void Dispose()
            {

                if (FORM != null) FORM.Disposed -= FORM_DISPOSED;

                FORM = null;
                ITEMS = null;
                PRCLIST = null;

                DATASET = null;
            }









        }


        #endregion

        #endregion
