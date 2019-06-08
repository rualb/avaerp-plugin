#line 2


 #region BODY
        //BEGIN

        const int VERSION = 7;
        const string FILE = "plugin.sys.event.admin.pls";


        #region TEXT

        const string event_ADMIN_ = "hadlericom_admin_";
        const string event_ADMIN_EXPORT_FIRM = "hadlericom_admin_firm";

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

                    x.MY_ADMIN_USER = s.MY_ADMIN_USER;

                    x.GETSYSPRM_USER = PLUGIN.GETSYSPRM_USER();

                    _SETTINGS.BUF = x;

                }


                public string MY_ADMIN_USER;


                public short GETSYSPRM_USER;
            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC)
            {

            }


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active On User")]
            public string MY_ADMIN_USER
            {
                get
                {
                    return (_GET("MY_ADMIN_USER", "1"));
                }
                set
                {
                    _SET("MY_ADMIN_USER", value);
                }

            }


            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return BUF.MY_ADMIN_USER == ""
                || Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_ADMIN_USER),
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


            if (fn == "form.app")
            {
                var tree = CONTROL_SEARCH(FORM, "cTreeTools");
                if (tree != null)
                {

                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
			{ "Text" ,"Admin"},
            { "AccessCode" ,"_sys"  },
			{ "ImageName" ,"role_32x32"},
			{ "Name" ,event_ADMIN_},
            };

                        RUNUIINTEGRATION(tree, args);

                    }




                    {

                        var _cmd = new string[] { "ref.gen.rec.user", "ref.gen.rec.firm", "ref.gen.rec.fs", "ref.gen.rec.currlist", "ref.gen.rec.parameter", "ref.gen.rec.fssecurity", "ref.gen.rec.refsecurity" };
                        var _text = new string[] { "T_USER", "T_FIRM", "T_FILE", "T_CURRENCY", "T_PARAMETER (T_DATASOURCE)", "T_AUTH", "T_CYPHCODE (T_REFERENCE)" };
                        var _img = new string[] { "user_32x32", "firm_32x32", "save_32x32", "fin_32x32", "tools_32x32", "cert_16x16", "filter_32x32" };

                        for (var i = 0; i < _cmd.Length; ++i)
                        {
                            var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_ADMIN_},
             { "AccessCode" ,"_sys"  },
			{ "CmdText" ,_cmd[i]  },
			{ "Text" ,_text[i]},
			{ "ImageName" ,_img[i]},
		    { "Name" ,_cmd[i]},
            };

                            RUNUIINTEGRATION(tree, args);

                        }

                    }


                }
                return;

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


                    case event_ADMIN_EXPORT_FIRM:
                        {


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





        #region CLAZZ

        class TEXT
        {

            public const string text_DESC = "Admin";

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


        class MY_DIR
        {
            public static string PRM_DIR_ROOT = PATHCOMBINE(
     Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ADMIN");

            static string filePrefix = "exp";

            public static void CHECK_DIR()
            {

                if (!System.IO.Directory.Exists(PRM_DIR_ROOT))
                    System.IO.Directory.CreateDirectory(PRM_DIR_ROOT);


            }




            public static string SAVE(StringBuilder pSb, string pSufix, string pExt = ".csv")
            {
                CHECK_DIR();


                var data = Encoding.UTF8.GetBytes(pSb.ToString());

                var fileName = filePrefix + "." + pSufix + "." + (FORMAT(DateTime.Now).Replace(" ", "-").Replace(":", "-")) + pExt;

                var fileNameFull = PRM_DIR_ROOT + "/" + fileName;

                FILEWRITE(fileNameFull, data);

                return fileNameFull;
            }
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


        #endregion

        #endregion
        #endregion




  