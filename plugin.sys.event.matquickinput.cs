#line 2


    #region BODY
        //BEGIN


        const int VERSION = 19;
        const string FILE = "plugin.sys.event.matquickinput.pls";



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



                    x.ASK_QUANTITY = s.MY_MATQUICKINPUT_ASK_QUANTITY;
                    x.ASK_PRICE = s.MY_MATQUICKINPUT_ASK_PRICE;
                    x.DEFAULT_QUANTITY = s.MY_MATQUICKINPUT_DEFAULT_QUANTITY;

                    x.MY_MATQUICKINPUT_USER = s.MY_MATQUICKINPUT_USER;

                    //

                    _SETTINGS.BUF = x;

                }

                public bool ASK_QUANTITY;
                public bool ASK_PRICE;
                public int DEFAULT_QUANTITY;

                public string MY_MATQUICKINPUT_USER;
            }

            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_MATQUICKINPUT_USER),
                     FORMAT(pPLUGIN.GETSYSPRM_USER())
                     ) >= 0;
            }

            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_MATQUICKINPUT)
            {

            }



            [ECategory(TEXT.text_MATQUICKINPUT)]
            [EDisplayName("Ask Quantity")]
            public bool MY_MATQUICKINPUT_ASK_QUANTITY
            {
                get { return PARSEBOOL(_GET("MY_MATQUICKINPUT_ASK_QUANTITY", "1")); }
                set { _SET("MY_MATQUICKINPUT_ASK_QUANTITY", value); }

            }

            [ECategory(TEXT.text_MATQUICKINPUT)]
            [EDisplayName("Ask Price")]
            public bool MY_MATQUICKINPUT_ASK_PRICE
            {
                get { return PARSEBOOL(_GET("MY_MATQUICKINPUT_ASK_PRICE", "1")); }
                set { _SET("MY_MATQUICKINPUT_ASK_PRICE", value); }

            }

            [ECategory(TEXT.text_MATQUICKINPUT)]
            [EDisplayName("Default Quantity")]
            public int MY_MATQUICKINPUT_DEFAULT_QUANTITY
            {
                get { return PARSEINT(_GET("MY_MATQUICKINPUT_DEFAULT_QUANTITY", "1")); }
                set { _SET("MY_MATQUICKINPUT_DEFAULT_QUANTITY", value); }

            }

            [ECategory(TEXT.text_MATQUICKINPUT)]
            [EDisplayName("Active On User")]
            public string MY_MATQUICKINPUT_USER
            {
                get
                {
                    return (_GET("MY_MATQUICKINPUT_USER", "1,2"));
                }
                set
                {
                    _SET("MY_MATQUICKINPUT_USER", value);
                }

            }
        }

        #endregion


        public class TEXT
        {
            public const string text_MATQUICKINPUT = "Material Quick Input for Stock Doc";


        }
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



            public string ADD_REC = "Add";


            public void lang_az()
            {

                ADD_REC = "Əlavə et";

            }

            public void lang_ru()
            {

                ADD_REC = "Добавить";

            }

            public void lang_tr()
            {


                ADD_REC = "Ekle";

            }
        }



        const string event_MATQUICKINPUT_INPUT = "hadlericom_matquickinput_input";



        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {

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
                case SysEvent.SYS_ADPDONE:
                    //  MY_SYS_ADPDONE_HANDLER(EVENTCODE, ARGS);
                    break;
            }



        }



        void MY_SYS_NEWFORM_INTEGRATE(Form FORM)
        {
            if (FORM == null)
                return;
            try
            {
                _MY_SYS_NEWFORM_INTEGRATE_STOCKADP(FORM);
            }
            catch (Exception exc)
            {
                MSGUSERERROR("Cant add button: " + exc.Message);
            }

        }


        void _MY_SYS_NEWFORM_INTEGRATE_STOCKADP(Form FORM)
        {


            var fn = GETFORMNAME(FORM);
            var isStock_ = (
                fn.StartsWith("adp.mm.doc.slip") ||
                fn.StartsWith("adp.sls.doc.inv") ||
                fn.StartsWith("adp.prch.doc.inv") ||
                fn.StartsWith("adp.prch.doc.order") ||
                fn.StartsWith("adp.sls.doc.order")
                );


            if (!isStock_)
                return;

            var taskcmd = RUNUIINTEGRATION(FORM, "_cmd", "taskcmd") as string;

            if (taskcmd == null)
                return;

            var cPanelBtnSub = CONTROL_SEARCH(FORM, "cPanelBtnSub");

            if (cPanelBtnSub == null)
                return;


            _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_MATQUICKINPUT_INPUT, _LANG.L.ADD_REC, "mm_16x16");

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

            _SETTINGS._BUF.LOAD_SETTINGS(this);

            if (!_SETTINGS.ISUSEROK(this))
                return;
            //
            try
            {


                object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
                object arg2 = ARGS.Length > 1 ? ARGS[1] : null;

                string[] list_ = EXPLODELISTPATH(EVENTCODE);

                switch (list_.Length > 1 ? list_[1].ToLowerInvariant() : "")
                {


                    case event_MATQUICKINPUT_INPUT:
                        {

                            if (ISADAPTERFORM(arg1 as Form))
                            {
                                var ds = RUNUIINTEGRATION(arg1, "_cmd", "dataset") as DataSet;
                                MY_IMPORT(ds);
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

        //END



        //export item info.
        public void MY_IMPORT(DataSet DATASET)
        {
            if (DATASET == null)
                return;

            DataTable pTabLines = TAB_GETTAB(DATASET, "STLINE");

            //doc has atleast one row
            var trcode = CASTASSHORT(TAB_GETROW(pTabLines, "TRCODE"));

            

            try
            {
                var prevMatName = "";
                while (true)
                {


                    var matRec = MY_GET_MAT_REC(trcode, ref prevMatName);
                    if (matRec == null)
                        return;


                    DataRow row_ = addRecord(trcode, pTabLines, matRec);
                    //positionOnRow(row_);

                }

            }
            catch (Exception exc)
            {
                MSGUSERINFO(exc.Message);
                LOG(exc);

            }

        }

        DataRow MY_GET_MAT_REC(short pTrCode,ref string pName)
        {
            pName =  pName??"" ;

            var refcode = "ref.mm.rec.mat";
            var col = "NAME";
            switch (pTrCode)
            {
                case 4:
                    refcode = "ref.prch.rec.srv";
                    col = "DEFINITION_";
                    break;
                case 9:
                    refcode = "ref.sls.rec.srv";
                    col = "DEFINITION_";
                    break;
            }

            DataRow[] refData_ = REF(refcode, col, pName); //
            if (refData_ == null || refData_.Length <= 0)
                return null;

            var res = refData_[0];

            pName = CASTASSTRING(TAB_GETROW(res, col));
            return res;

        }


        void positionOnRow(DataRow pRow)
        {
            if (pRow == null)
                return;
            Form form_ = Form.ActiveForm as Form;
            if (form_ == null)
                return;
            if (!object.ReferenceEquals(pRow.Table.DataSet, _PLUGIN.GETDATASETFROMADPFORM(form_)))
                return;

            DataGridView GRID = _PLUGIN.CONTROL_SEARCH(form_, "cGrid") as DataGridView;
            if (GRID == null)
                return;


            TOOL_GRID.SET_GRID_POSITION(GRID, pRow, null);

        }









        DataRow addRecord(short pTrCode, DataTable pTabLines, DataRow pMaterialRec)
        {
            if (pMaterialRec == null)
                return null;

            int indx_ = pTabLines.Rows.Count - 1;
            for (; indx_ >= 0; --indx_)
            {
                var r_ = pTabLines.Rows[indx_];
                if (!TAB_ROWDELETED(r_))
                {
                    var g_ = CASTASSHORT(ISNULL(TAB_GETROW(r_, "GLOBTRANS"), 0));
                    if (g_ == 0)
                        break;
                }
            }






            DataRow row_ = pTabLines.NewRow();
            pTabLines.Rows.InsertAt(row_, ++indx_);

            positionOnRow(row_);

            //pTabLines.Rows.InsertAt(row_, 0);
            //
            TAB_SETROW(row_, "STOCKREF", TAB_GETROW(pMaterialRec, "LOGICALREF"));


            if (_SETTINGS.BUF.DEFAULT_QUANTITY > 0)
                TAB_SETROW(row_, "AMOUNT", _SETTINGS.BUF.DEFAULT_QUANTITY);

            if (_SETTINGS.BUF.ASK_QUANTITY)
            {
                double quantity_ = askNumber("T_QUANTITY", 1);
                if (quantity_ >= 0)
                    TAB_SETROW(row_, "AMOUNT", quantity_);
            }


            if (_SETTINGS.BUF.ASK_PRICE)
            {
                double price_ = askNumber("T_PRICE", CASTASDOUBLE(ISNULL(TAB_GETROW(row_, "PRICE"), 0.0)));
                if (price_ >= 0)
                    TAB_SETROW(row_, "PRICE", price_);

            }



            TAB_SETROW(row_, "SPECODE", "");



            return row_;


        }



        double askNumber(string pMsg, double pDef)
        {

            DataRow[] rows_ = REF("ref.gen.double desc::" + STRENCODE(pMsg) + " filter::filter_VALUE," + FORMATSERIALIZE(pDef));
            if (rows_ != null && rows_.Length > 0)
            {
                return CASTASDOUBLE(ISNULL(rows_[0]["VALUE"], 0.0));
            }
            return -1.0;

        }





        public static string MY_CHOOSE_SQL(string pSqlMs, string pSqlPg, string pSqlSl = null)
        {

            if (ISMSSQL())
                return pSqlMs;

            if (ISPOSTGRESQL())
                return pSqlPg;

            if (ISSQLITE())
                return pSqlSl ?? pSqlPg;

            throw new Exception("Undefined datasource");
        }


        #endregion