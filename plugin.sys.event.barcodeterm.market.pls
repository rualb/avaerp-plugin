﻿#line 2



        #region PLUGIN_BODY
        const int VERSION = 11;

        const string FILE = "plugin.sys.event.barcodeterm.market.pls";



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
            public const string text_DESC = "POS Market";


            public class L
            {

            }
        }


        const string event_BARCODETERM_ = "_barcodeterm_";
        const string event_BARCODETERM_MARKET_SALE = "_barcodeterm_market_sale";
        const string event_BARCODETERM_MARKET_SALERET = "_barcodeterm_market_saleret";
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
			{ "CmdText" ,"event name::"+event_BARCODETERM_MARKET_SALE},
			{ "Text" ,"T_SALE"},
			{ "ImageName" ,"street_stall_32x32"},
		    { "Name" ,event_BARCODETERM_MARKET_SALE},
            };

                        RUNUIINTEGRATION(tree, args);

                    }
                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_BARCODETERM_},
			{ "CmdText" ,"event name::"+event_BARCODETERM_MARKET_SALERET},
			{ "Text" ,"T_SALE (T_RETURN)"},
			{ "ImageName" ,"street_stall_32x32"},
			{ "Name" ,event_BARCODETERM_MARKET_SALERET},
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

                switch (list_.Length > 1 ? list_[1].ToLowerInvariant() : "")
                {


                    case event_BARCODETERM_MARKET_SALE:
                    case event_BARCODETERM_MARKET_SALERET:
                        

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
