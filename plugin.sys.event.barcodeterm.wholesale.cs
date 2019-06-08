#line 2


 
        #region PLUGIN_BODY
        const int VERSION = 11;

        const string FILE = "plugin.sys.event.barcodeterm.wholesale.pls";


        #region SETTINGS

        class _SETTINGS : TOOL_SETTINGS
        {

            public static _BUF BUF = null;
            public class _BUF
            {
               
                public bool MY_BARCODETERM_WHOLESALE_PRINT_ON_SAVE;
                public string MY_BARCODETERM_WHOLESALE_USER;
                public string MY_BARCODETERM_WHOLESALE_CASH_LIST;
                public bool MY_BARCODETERM_WHOLESALE_USE_VAT;
                public bool MY_BARCODETERM_WHOLESALE_EDIT_VAT;
                public double MY_BARCODETERM_WHOLESALE_DEF_VAT;

                public bool _ISUSEROK;
                public short _FIRM;
                public string _FIRMNAME;
                public short _PERIOD;
                public short _USER;

            }

            public static void LOAD_SETTINGS(_PLUGIN PLUGIN)
            {
                if (_SETTINGS.BUF != null)
                    return;

                var x = new _SETTINGS._BUF();

                var s = new _SETTINGS(PLUGIN);

                x.MY_BARCODETERM_WHOLESALE_PRINT_ON_SAVE = s.MY_BARCODETERM_WHOLESALE_PRINT_ON_SAVE;
                x.MY_BARCODETERM_WHOLESALE_USER = s.MY_BARCODETERM_WHOLESALE_USER;
                x.MY_BARCODETERM_WHOLESALE_CASH_LIST = s.MY_BARCODETERM_WHOLESALE_CASH_LIST;
                x.MY_BARCODETERM_WHOLESALE_USE_VAT = s.MY_BARCODETERM_WHOLESALE_USE_VAT;
                x.MY_BARCODETERM_WHOLESALE_EDIT_VAT = s.MY_BARCODETERM_WHOLESALE_EDIT_VAT;
                x.MY_BARCODETERM_WHOLESALE_DEF_VAT = s.MY_BARCODETERM_WHOLESALE_DEF_VAT;



                //

                x._USER = PLUGIN.GETSYSPRM_USER();
                x._FIRM = PLUGIN.GETSYSPRM_FIRM();
                x._FIRMNAME = PLUGIN.GETSYSPRM_FIRMNAME();
                x._PERIOD = PLUGIN.GETSYSPRM_PERIOD();
                //

                _SETTINGS.BUF = x;

                //
                var arr = new List<string>(EXPLODELIST(x.MY_BARCODETERM_WHOLESALE_USER.Trim()));
                x._ISUSEROK = x._USER == 1 || (arr.Count == 0 || arr.Contains(FORMAT(x._USER)));

            }

            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC) //, "ava.production.config")
            {

            }


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active If User (Nr List 1,2,3 or empty)")]
            public string MY_BARCODETERM_WHOLESALE_USER
            {
                get
                {
                    return (_GET("MY_BARCODETERM_WHOLESALE_USER", ""));
                }
                set
                {
                    _SET("MY_BARCODETERM_WHOLESALE_USER", value);
                }

            }




            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Print On Save")]
            public bool MY_BARCODETERM_WHOLESALE_PRINT_ON_SAVE
            {
                get
                {
                    //wholesale no print on save by def 
                    return (_GET("MY_BARCODETERM_WHOLESALE_PRINT_ON_SAVE", "0")) == "1";
                }
                set
                {
                    _SET("MY_BARCODETERM_WHOLESALE_PRINT_ON_SAVE", value);
                }

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Use VAT")]
            public bool MY_BARCODETERM_WHOLESALE_USE_VAT
            {
                get
                {
                    //wholesale no print on save by def 
                    return (_GET("MY_BARCODETERM_WHOLESALE_USE_VAT", "0")) == "1";
                }
                set
                {
                    _SET("MY_BARCODETERM_WHOLESALE_USE_VAT", value);
                }

            }


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Edit VAT")]
            public bool MY_BARCODETERM_WHOLESALE_EDIT_VAT
            {
                get
                {
                    //wholesale no print on save by def 
                    return (_GET("MY_BARCODETERM_WHOLESALE_EDIT_VAT", "0")) == "1";
                }
                set
                {
                    _SET("MY_BARCODETERM_WHOLESALE_EDIT_VAT", value);
                }

            }


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Default VAT")]
            public double MY_BARCODETERM_WHOLESALE_DEF_VAT
            {
                get
                {

                    var vat = PARSEDOUBLE(_GET("MY_BARCODETERM_WHOLESALE_DEF_VAT", "0"));

                    return MIN(99, MAX(0, vat));
                }
                set
                {
                    _SET("MY_BARCODETERM_WHOLESALE_DEF_VAT", value);
                }

            }




            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Cash List (Cash1,Cash2,...)")]
            public string MY_BARCODETERM_WHOLESALE_CASH_LIST
            {
                get
                {
                    return (_GET("MY_BARCODETERM_WHOLESALE_CASH_LIST", ""));
                }
                set
                {
                    _SET("MY_BARCODETERM_WHOLESALE_CASH_LIST", value);
                }

            }
            //



        }

        #endregion


        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "POS Wholesale";


            public class L
            {

            }
        }


        const string event_BARCODETERM_ = "_barcodeterm_";
        const string event_BARCODETERM_WHOLESALE_SALE = "_barcodeterm_wholesale_sale";
        const string event_BARCODETERM_WHOLESALE_SALERET = "_barcodeterm_wholesale_saleret";
        #endregion

        #region MAIN



        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {

            if (ISWEB())
                return;





            object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
            object arg2 = ARGS.Length > 1 ? ARGS[1] : null;
            object arg3 = ARGS.Length > 2 ? ARGS[2] : null;

            string[] list_ = EXPLODELISTPATH(EVENTCODE);

            var cmdType = list_.Length > 0 ? list_[0] : "";
            var cmdExt = list_.Length > 1 ? list_[1] : "";

            _SETTINGS.LOAD_SETTINGS(this);

            if (!_SETTINGS.BUF._ISUSEROK)
                return;

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
			{ "CmdText" ,"event name::"+event_BARCODETERM_WHOLESALE_SALE},
			{ "Text" ,"T_WHOLESALE"},
			{ "ImageName" ,"street_stall_32x32"},
		    { "Name" ,event_BARCODETERM_WHOLESALE_SALE},
            };

                        RUNUIINTEGRATION(tree, args);

                    }
                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_BARCODETERM_},
			{ "CmdText" ,"event name::"+event_BARCODETERM_WHOLESALE_SALERET},
			{ "Text" ,"T_WHOLESALE (T_RETURN)"},
			{ "ImageName" ,"street_stall_32x32"},
			{ "Name" ,event_BARCODETERM_WHOLESALE_SALERET},
            };

                        RUNUIINTEGRATION(tree, args);

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


                        case event_BARCODETERM_WHOLESALE_SALE:
                        case event_BARCODETERM_WHOLESALE_SALERET:
                            {

                                var dic = new Dictionary<string, string>();
                                //
                                dic["type"] = FORMAT((int)2);
                                dic["trcode"] = cmd.EndsWith("ret") ? "3" : "8";

                                dic["BARCODE_LEN10_CLIENT"] = "0";
                                dic["ASK_PRINT"] = "0";
                                dic["UI_SCALE"] = "35";
                                dic["UI_SCALE_H"] = "20";

                                dic["SEARCH_MAT_BY_CODE"] = "1";
                                dic["SEARCH_CL_BY_CODE"] = "1";
                                dic["PRINT_ON_SAVE"] = FORMAT(_SETTINGS.BUF.MY_BARCODETERM_WHOLESALE_PRINT_ON_SAVE); // "1";



                                dic["FULL_SCREEN"] = "0";
                                dic["PARENT_OFFLINE"] = "0";
                                dic["RETURN_EDIT_PRICE"] = "1";

                                dic["HIDE_DISC"] = "0";

                                dic["CHANGE_PAYPLAN"] = "0";

                                dic["PRICE_DIFF_BY_PRCH"] = "0";

                                dic["MAT_PRICE_EDIT"] = "0";

                                dic["APPLY_CAMPAGIN"] = "0";

                                dic["BEEP"] = "1";

                                dic["CASH_LIST"] = FORMAT(_SETTINGS.BUF.MY_BARCODETERM_WHOLESALE_CASH_LIST);

                                dic["USE_VAT"] = FORMAT(_SETTINGS.BUF.MY_BARCODETERM_WHOLESALE_USE_VAT);
                                dic["EDIT_VAT"] = FORMAT(_SETTINGS.BUF.MY_BARCODETERM_WHOLESALE_EDIT_VAT);
                                dic["DEF_VAT"] = FORMAT(_SETTINGS.BUF.MY_BARCODETERM_WHOLESALE_DEF_VAT);

                                
                                SYSUSEREVENT("_barcodeterm_", new object[]{
                         dic
                            });


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






        #endregion
