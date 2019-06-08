#line 2


     #region BODY
        //BEGIN

        const int VERSION = 6;
        const string FILE = "plugin.sys.event.tools.pls";





        #region TEXT


        const string event_TOOLS_ = "hadlericom_tools_";
        const string event_TOOLS_CARDTYPE = "hadlericom_tools_cardtype";
        const string event_TOOLS_ZEROWH = "hadlericom_tools_zerowh";
 


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

                    x.MY_TOOLS_USER = s.MY_TOOLS_USER;

                    x.GETSYSPRM_USER = PLUGIN.GETSYSPRM_USER();


                    _SETTINGS.BUF = x;

                }


                public string MY_TOOLS_USER;


                public short GETSYSPRM_USER;
            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC)
            {

            }


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active On User")]
            public string MY_TOOLS_USER
            {
                get
                {
                    return (_GET("MY_TOOLS_USER", "1"));
                }
                set
                {
                    _SET("MY_TOOLS_USER", value);
                }

            }



            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_TOOLS_USER),
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
            // { "_root" ,""},
			 
			{ "Text" ,"T_TOOL"},
			{ "ImageName" ,"tools_32x32"},
			 { "Name" ,event_TOOLS_},
            };

                        RUNUIINTEGRATION(tree, args);

                    }
                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
            { "_root" ,event_TOOLS_},
			{ "CmdText" ,"event name::"+event_TOOLS_CARDTYPE},
			{ "Text" ,"Change Card Type"},
			{ "ImageName" ,"run_32x32"},
			 { "Name" ,event_TOOLS_CARDTYPE},
            };

                        RUNUIINTEGRATION(tree, args);

                    }


            //        {
            //            var args = new Dictionary<string, object>() { 
 
            //{ "_cmd" ,""},
            //{ "_type" ,""},
            //{ "_root" ,event_TOOLS_},
            //{ "CmdText" ,"event name::"+event_TOOLS_ZEROWH},
            //{ "Text" ,"Zero Warehouse"},
            //{ "ImageName" ,"num0_32x32"},
            // { "Name" ,event_TOOLS_ZEROWH},
            //};

            //            RUNUIINTEGRATION(tree, args);

            //        }

                }
                return;

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
                    case event_TOOLS_CARDTYPE:
                        {
                            TOOL_CHANGE_CARD_TYPE();
                        }
                        break;
                    case event_TOOLS_ZEROWH:
                        {
                            TOOL_ZEROWH();
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

        void TOOL_ZEROWH()
        {
            if (GETSYSPRM_USER() != 1)
                return;

            if (!MSGUSERASK("Zero all warehouses ?"))
                return;
            var PLUGIN = this;

            INVOKEINBATCH((s, a) =>
            {

                var docNr = 50;
                var docWh = 0;

                var handler = new DoWorkEventHandler((s2, e) =>
                {
                    e.Result = false;
                    DataSet doc_ = ((DataSet)e.Argument);

                    DataTable tabHeaderSlip_ = TAB_GETTAB(doc_, "STFICHE");
                    DataTable tabLine_ = TAB_GETTAB(doc_, "STLINE");
                    TAB_SETROW(tabHeaderSlip_, "FICHENO", "WH" + docWh + "ZERO" + docNr + "_" + LEFT(FORMAT(DateTime.Now), 10));
                    TAB_SETROW(tabHeaderSlip_, "SOURCEINDEX", docWh);
                    TAB_SETROW(tabHeaderSlip_, "DOCODE", "ZERO");


                    var sign_ = (docNr == 50 ? -1 : +1);

                    var lines = SQL(@"
						SELECT  
						G.STOCKREF,
						0.0 PRICE,
						ABS(G.ONHAND) DIFF
						FROM LG_$FIRM$_$PERIOD$_GNTOTST G WHERE INVENNO = @P1 AND ABS(G.ONHAND) > 0.01 
						AND SIGN(G.ONHAND) = @P2
						
						
						", new object[] { docWh, sign_ });


                    foreach (DataRow mRow in lines.Rows)
                    {

                        DataRow r = TAB_ADDROW(tabLine_);

                        TAB_SETROW(r, "STOCKREF", TAB_GETROW(mRow, "STOCKREF"));
                        //TAB_SETROW(r,"PRICE", TAB_GETROW(mRow,"PRICE"));
                        TAB_SETROW(r, "AMOUNT", TAB_GETROW(mRow, "DIFF"));

                    }


                    e.Result = (tabLine_.Rows.Count > 0); //save if has data
                });

                var whTab_ = SQL("SELECT NR FROM L_CAPIWHOUSE WHERE FIRMNR = $FIRM$ ", null);

                foreach (DataRow whRow in whTab_.Rows)
                {
                    docWh = CASTASSHORT(TAB_GETROW(whRow, "NR"));

                    docNr = 50;
                    PLUGIN.EXEADPCMD(new string[] { "adp.mm.doc.slip.50 cmd::add" }, new DoWorkEventHandler[] { handler }, true);//in global batch
                    docNr = 51;
                    PLUGIN.EXEADPCMD(new string[] { "adp.mm.doc.slip.51 cmd::add" }, new DoWorkEventHandler[] { handler }, true);//in global batch
                }

            }, null);


            MSGUSERINFO("Warehouses set zero");
        }

        void TOOL_CHANGE_CARD_TYPE()
        {
            var list = new List<string>();

            list.Add("client");
            list.Add(LANG("T_PERSONAL"));
            list.Add("material");
            list.Add(LANG("T_MATERIAL"));


            var resCard_ = REF("ref.gen.definedlist [obj::" + JOINLIST(list.ToArray()) + "] desc::T_CARD type::string");
            var card_ = (resCard_ != null && resCard_.Length > 0 ? CASTASSTRING(resCard_[0]["VALUE"]) : null);


            var refCode = "";
            var dbTable = "";
            var cardTypeList = "";

            if (card_ != null)
            {
                switch (card_)
                {
                    case "client":
                        refCode = "ref.fin.rec.client";
                        dbTable = "LG_$FIRM$_CLCARD";
                        cardTypeList = "LIST_FIN_PERSONAL_TYPE";
                        break;
                    case "material":
                        refCode = "ref.mm.rec.mat";
                        dbTable = "LG_$FIRM$_ITEMS";
                        cardTypeList = "LIST_MM_MAT_TYPE";
                        break;

                }


                if (!ISEMPTY(refCode))
                {


                    var resCardType_ = REF("ref.gen.definedlist [obj::" + cardTypeList + "] desc::T_TYPE type::int");
                    var cardType_ = (resCardType_ != null && resCardType_.Length > 0 ? CASTASINT(resCardType_[0]["VALUE"]) : 0);


                    if (cardType_ > 0)
                    {
                        DataRow[] resLRef_ = REF(refCode +" multi::1", "LOGICALREF", "");
                        if (resLRef_ != null && resLRef_.Length > 0)
                        {
                            foreach (var rec in resLRef_)
                            {
                                var lref = TAB_GETROW(rec, "LOGICALREF");

                                SQL("UPDATE " + dbTable + " SET CARDTYPE = @P1 WHERE LOGICALREF = @P2", new object[] { cardType_, lref });

                            }


                            MSGUSERINFO("T_MSG_OPERATION_OK");

                        }
                    }
                }
            }
        }




        //END



        #region CLAZZ

        class TEXT
        {

            public const string text_DESC = "Material List";

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

