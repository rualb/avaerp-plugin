#line 2


        #region BODY
        //BEGIN

        const int VERSION = 9;
        const string FILE = "plugin.sys.event.magent.users.pls";


        #region TEXT


        const string event_MAGENT_USERS_ = "hadlericom_magent_users_";
        const string event_MAGENT_USERS_QRCODE_SHOW = "hadlericom_magent_users_qrcode_show";


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


                    x.MY_MAGENT_USERS_QRCODE_ENV = s.MY_MAGENT_USERS_QRCODE_ENV;
                    x.MY_MAGENT_USERS_QRCODE_URL = s.MY_MAGENT_USERS_QRCODE_URL;
                    x.MY_MAGENT_USERS_USER = s.MY_MAGENT_USERS_USER;


                    _SETTINGS.BUF = x;

                }

                public string MY_MAGENT_USERS_QRCODE_ENV;
                public string MY_MAGENT_USERS_QRCODE_URL;
                public string MY_MAGENT_USERS_USER;
            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC)
            {

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active On User")]
            public string MY_MAGENT_USERS_USER
            {
                get
                {
                    return (_GET("MY_MAGENT_USERS_USER", "1,2"));
                }
                set
                {
                    _SET("MY_MAGENT_USERS_USER", value);
                }

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("M-Agent Server URL (http://192.168.1.100:8000)")]
            public string MY_MAGENT_USERS_QRCODE_URL
            {
                get
                {
                    return (_GET("MY_MAGENT_USERS_QRCODE_URL", "http://192.168.1.200:8000"));
                }
                set
                {
                    _SET("MY_MAGENT_USERS_QRCODE_URL", value);
                }

            }


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("M-Agent Server Env (s1.main)")]
            public string MY_MAGENT_USERS_QRCODE_ENV
            {
                get
                {
                    return (_GET("MY_MAGENT_USERS_QRCODE_ENV", "s1.main"));
                }
                set
                {
                    _SET("MY_MAGENT_USERS_QRCODE_ENV", value);
                }

            }
            //

            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_MAGENT_USERS_USER),
                     FORMAT(pPLUGIN.GETSYSPRM_USER())
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

            if (fn.StartsWith("ref.gen.rec.userext"))
            {

                var cPanelBtnSub = CONTROL_SEARCH(FORM, "cPanelBtnSub");

                if (cPanelBtnSub == null)
                    return;



                _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_MAGENT_USERS_QRCODE_SHOW, LANG("QRCode"), "barcode_16x16");


            }


            if (fn == "form.app")
            {
                  var tree = CONTROL_SEARCH(FORM, "cTreeTools");
                  if (tree != null)
                  {
                      //some times not admin manage m-users(ext.)
                     // if (ACCESSALLOWED("_sys"))
                      {
                          var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
            { "_root" ,""},//general records
			{ "CmdText" ,"ref.gen.rec.userext"},
			{ "Text" ,"T_USER (T_EXT)"},
			{ "ImageName" ,"user_32x32"},
           // { "AccessCode" ,"_admin"},
			{ "Name" ,"ref.gen.rec.userext"},
            };

                          RUNUIINTEGRATION(tree, args);

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


                    case event_MAGENT_USERS_QRCODE_SHOW:
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

                            MY_GEN_QRCODE(rec);
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


        void MY_GEN_QRCODE(DataRow pRow)
        {
            if (pRow == null)
                return;

            if (pRow.Table.TableName != "USEREXT")
                return;


            var lref = CASTASINT(pRow["LOGICALREF"]);

            var rec = TAB_GETLASTROW(XSQL("select NAME,PASSWD,DEFINITION_ from L_CAPIUSEREXT where LOGICALREF = @P1 and FIRMNR=@P2", new object[] { lref, GETSYSPRM_FIRM() }));

            var title = TAB_GETROW(rec, "DEFINITION_");
            var name = TAB_GETROW(rec, "NAME");
            var pw = TAB_GETROW(rec, "PASSWD");

            var url = _SETTINGS.BUF.MY_MAGENT_USERS_QRCODE_URL;
            var env = _SETTINGS.BUF.MY_MAGENT_USERS_QRCODE_ENV;

            var sb = new StringBuilder();

            sb.Append("cmd,").Append("setuser").AppendLine();
            sb.Append("url,").Append(url).AppendLine();
            sb.Append("ident,").Append(env + ":" + name).AppendLine();
            sb.Append("token,").Append("pw " + pw);

            var qrText = sb.ToString();



            using (var form = new Form())
            {

                form.Icon = CTRL_FORM_ICON();
                form.Text = "QRCode: " + title;
                // form.Size = new Size(300, 300);
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ClientSize = new Size(250, 250);
                form.Padding = new Padding(5);
                var pic = new PictureBox() { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.CenterImage };

                form.BackColor = Color.White;
                pic.BackColor = Color.White;

                form.Controls.Add(pic);

                var size = Math.Min(pic.Size.Width, pic.Size.Height);

                var data = _PLUGIN.TOOL_IMAGE.QRCODE(qrText, size);

                var img = Image.FromStream(new System.IO.MemoryStream(data));
                pic.Image = img;

                form.ShowDialog();

            }
        }


        //END



        #region CLAZZ

        class TEXT
        {

            public const string text_DESC = "Users (Ext.)";

            static TEXT _L = null;

            public static TEXT L
            {
                get
                {
                    if (_L == null) 
                        _L = new TEXT(); 
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


            }


            public void lang_en()
            {




            }

            public void lang_az()
            {



            }



            public void lang_ru()
            {


            }

            public string A;



        }





        #endregion


        #region TOOLS



        #endregion

        #endregion


        #endregion