 #line 2
 
 
      #region BODY
        //BEGIN

        const int VERSION = 9;
        const string FILE = "plugin.sys.event.loadlist.inv.statusto5.pls";


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

                    _SETTINGS.BUF = x;

                }

                public string MY_LOADLIST_USER;



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
            public const string text_DESC = "Load List Invoice Status to 5";


            public class L
            {

            }
        }

        const string event_LOADLIST_ = "hadlericom_loadlist_";
        const string event_LOADLIST_INV_STATUS_FROM_3_TO_5 = "hadlericom_loadlist_inv_status_from_3_to_5";



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
			{ "CmdText" ,"event name::"+event_LOADLIST_INV_STATUS_FROM_3_TO_5},
			{ "Text" ,LANG("T_INVOICE (T_WORKFLOW_STATUS_5)")},
			{ "ImageName" ,"barcode_32x32"},
			{ "Name" ,event_LOADLIST_INV_STATUS_FROM_3_TO_5},
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


                    case event_LOADLIST_INV_STATUS_FROM_3_TO_5:
                        {
                            MY_INV_FROM_3_TO_5();
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



        void MY_INV_FROM_3_TO_5()
        {

            var cmdAppend = "";
            while (true)
            {
                var barcode = MY_TOOL.MY_ASK_STRING(this, "T_INVOICE", "", cmdAppend);
                if (ISEMPTY(barcode))
                    break;

                //make upper
                barcode = barcode.ToUpperInvariant().Trim(); ;
 

                var docLRef = SQLSCALAR("SELECT LOGICALREF FROM LG_$FIRM$_$PERIOD$_INVOICE WHERE FICHENO = @P1 AND TRCODE = 8", new object[] { barcode });

                if (ISEMPTYLREF(docLRef))
                {
                    MY_TOOL.BEEPERR();
                    cmdAppend = "warning::1 ";//red
                }
                else
                {
                    SQL("UPDATE LG_$FIRM$_$PERIOD$_INVOICE SET WFSTATUS = 5, RECSTATUS=RECSTATUS+1 WHERE LOGICALREF = @P1", new object[] { docLRef });
                    cmdAppend = "success::1 ";//green
                }
            }


        }

        #region TOOLS

        class MY_TOOL
        {

            public static string MY_ASK_STRING(_PLUGIN pPLUGIN, string pMsg, string pDef, string pCmdAppend=null)
            {

                DataRow[] rows_ = pPLUGIN.REF("ref.gen.string " + (ISEMPTY(pCmdAppend) ? "" : pCmdAppend) + "desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
                if (rows_ != null && rows_.Length > 0)
                {
                    return _PLUGIN.CASTASSTRING(ISNULL(rows_[0]["VALUE"], ""));
                }
                return null;

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
        }


        #endregion

        #endregion
