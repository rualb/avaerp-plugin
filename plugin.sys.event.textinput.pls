#line 2


 #region BODY
        //BEGIN



        const int VERSION = 17;

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

                    x.MY_TEXT_INPUT_SERCH_BY_NAME = s.MY_TEXT_INPUT_SERCH_BY_NAME;
                    x.MY_TEXT_INPUT_GTIN14 = s.MY_TEXT_INPUT_GTIN14;

                    //

                    _SETTINGS.BUF = x;

                }

                public bool MY_TEXT_INPUT_SERCH_BY_NAME;
                public bool MY_TEXT_INPUT_GTIN14;
            }



            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC)
            {

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Search by Name")]
            public bool MY_TEXT_INPUT_SERCH_BY_NAME
            {
                get { return PARSEBOOL(_GET("MY_TEXT_INPUT_SERCH_BY_NAME", "0")); }
                set { _SET("MY_TEXT_INPUT_SERCH_BY_NAME", value); }

            }


            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Correct Barcode to GTIN 14")]
            public bool MY_TEXT_INPUT_GTIN14
            {
                get { return PARSEBOOL(_GET("MY_TEXT_INPUT_GTIN14", "0")); }
                set { _SET("MY_TEXT_INPUT_GTIN14", value); }

            }
        }

        #endregion


        public class TEXT
        {
            public const string text_DESC = "Text Input";


        }

        const string event_TEXT_INPUT = "hadlericom_text_input";



        const string ITM_NAME = "ITEMS_____NAME";
        const string ITM_CODE = "ITEMS_____CODE";
        const string ITM_AMOUNT = "STLINE_____AMOUNT";
        const string ITM_PRICE = "STLINE_____PRICE";



        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {

            _SETTINGS._BUF.LOAD_SETTINGS(this);

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


            _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_TEXT_INPUT, LANG("T_IMPORT - T_TEXT"), "import_16x16.png");
            // _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_ITS_SERIALNR, "Seri No.");
        }
        void _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(Control pCtrl, string pEvent, string pText, string pImg)
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

            //
            try
            {


                object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
                object arg2 = ARGS.Length > 1 ? ARGS[1] : null;

                string[] list_ = EXPLODELISTPATH(EVENTCODE);

                switch (list_.Length > 1 ? list_[1].ToLowerInvariant() : "")
                {


                    case event_TEXT_INPUT:
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

            var pTabLines = TAB_GETTAB(DATASET, "STLINE");



            string text_ = MY_GET_TEXT();
            if (text_ == null || text_ == "")
                return;

            var trcode = CASTASSHORT(TAB_GETROW(pTabLines, "TRCODE"));


            //MSGUSERINFO(text_);

            DataTable data_ = MY_GET_DATA(trcode, text_);

            for (int i_ = data_.Rows.Count - 1; i_ >= 0; --i_)
            {
                DataRow row_ = data_.Rows[i_];
                //
                string itemCode_ = CASTASSTRING(TAB_GETROW(row_, ITM_CODE));
                double itemAmount_ = PARSEDOUBLE(CASTASSTRING(TAB_GETROW(row_, ITM_AMOUNT)));
                double itemPrice_ = PARSEDOUBLE(CASTASSTRING(TAB_GETROW(row_, ITM_PRICE)));

                //
                DataRow newRow = pTabLines.NewRow();
                pTabLines.Rows.InsertAt(newRow, 0);
                //
                TAB_SETROW(newRow, "ITEMS_____CODE", itemCode_);
                TAB_SETROW(newRow, "AMOUNT", itemAmount_);
                if (!ISNUMZERO(itemPrice_))
                    TAB_SETROW(newRow, "PRICE", itemPrice_);


            }









        }



        string MY_GET_TEXT()
        {

            DataRow[] refData_ = REF("ref.gen.text desc::" + (STRENCODE(LANG("T_" + (_SETTINGS.BUF.MY_TEXT_INPUT_SERCH_BY_NAME ? "NAME" : "CODE") + " - T_AMOUNT - T_PRICE")))); //
            if (refData_ == null || refData_.Length <= 0)
                return null;

            return CASTASSTRING(refData_[0]["VALUE"]);

        }
        DataTable MY_GET_DATA(short pTrCode, string pText)
        {
            //MSGUSERINFO("OK");
            DataTable data_ = TAB_EXPLODE(pText, new string[] { ITM_NAME, ITM_AMOUNT, ITM_PRICE });
            TAB_ADDCOL(data_, ITM_CODE, typeof(string));

            MY_CHECK_TABLE(pTrCode, data_);



            return data_;
        }

        string MY_GET_ITEMCODE(short pTrCode, string pItemDesc)
        {
            string itemName_ = _SETTINGS.BUF.MY_TEXT_INPUT_SERCH_BY_NAME ? pItemDesc : pItemDesc.TrimStart(new char[] { '+' });
            string itemName2_ = CONVERTBARCODE(pItemDesc);

            DataTable data = null;

            switch (pTrCode)
            {
                case 4:
                case 9:
                    data = SQL(string.Format(@"
SELECT 
--$MS$--TOP(2) 
CODE FROM LG_$FIRM$_SRVCARD WHERE ( {0} IN (@P1,@P2)) AND CARDTYPE = @P3
--$PG$--LIMIT 2
--$SL$--LIMIT 2
", (_SETTINGS.BUF.MY_TEXT_INPUT_SERCH_BY_NAME ? "NAME" : "CODE")), new object[] { itemName_, itemName2_, pTrCode == 4 ? 1 : 2 });

                    break;

                default:
                    data = SQL(string.Format(@"
SELECT 
--$MS$--TOP(2) 
CODE FROM LG_$FIRM$_ITEMS WHERE ( {0} IN (@P1,@P2))
--$PG$--LIMIT 2
--$SL$--LIMIT 2
", (_SETTINGS.BUF.MY_TEXT_INPUT_SERCH_BY_NAME ? "NAME" : "CODE")), new object[] { itemName_, itemName2_ });

                    break;

            }



            if (data.Rows.Count == 0)
                throw new Exception("T_MSG_DATA_NO - T_RECORD [" + pItemDesc + "]");
            else
                if (data.Rows.Count > 1)
                    throw new Exception("T_MSG_ERROR_DUPLICATE_RECORD - T_RECORD [" + pItemDesc + "]");



            return CASTASSTRING(TAB_GETROW(data, "CODE"));

        }


        ///////////////////////////////////////////////////////////////////////////////////
        //CHECK DATA TYPES
        ///////////////////////////////////////////////////////////////////////////////////

        void MY_CHECK_TABLE(short pTrCode, DataTable pTable)
        {
            string tableName_ = pTable.TableName;

            foreach (DataColumn col_ in pTable.Columns)
                foreach (DataRow row_ in pTable.Rows)
                    row_[col_] = row_[col_].ToString().Trim();

            foreach (DataColumn col_ in pTable.Columns)
                if (MY_IS_FLOAT(tableName_, col_.ColumnName))
                    foreach (DataRow row_ in pTable.Rows)
                    {
                        string value_ = row_[col_].ToString();
                        value_ = value_.Replace(" ", "").Replace(",", ".").Replace(" ", "");

                        if (value_ == string.Empty)
                            value_ = "0";

                        try
                        {
                            PARSEDOUBLE(value_);
                        }
                        catch
                        {
                            throw new Exception(
                            string.Format("T_MSG_INVALID_RECODR, T_TABLE/T_COLUMN/T_VALUE, {0}/{1},{2}", tableName_, col_.ColumnName, value_)
                            );
                        }

                        row_[col_] = value_;
                    }

            foreach (DataColumn col_ in pTable.Columns)
                if (MY_IS_DATE(tableName_, col_.ColumnName))
                    foreach (DataRow row_ in pTable.Rows)
                    {
                        string value_ = row_[col_].ToString();
                        try
                        {
                            if (value_.Length < 10) throw new Exception();
                            value_ = MY_CHECKDATE(value_);
                            PARSEDATETIME(value_);
                        }
                        catch
                        {
                            throw new Exception(
                            string.Format("T_MSG_INVALID_RECODR, T_TABLE/T_COLUMN/T_VALUE, {0}/{1},{2}", tableName_, col_.ColumnName, value_)
                            );
                        }

                        row_[col_] = value_;
                    }

            MY_CHECK_DATA_VALID(pTrCode, pTable);


        }

        static string CONVERTBARCODE(string pStr)
        {

            if (!_SETTINGS.BUF.MY_TEXT_INPUT_SERCH_BY_NAME)
            {
                if (!_SETTINGS.BUF.MY_TEXT_INPUT_GTIN14)
                {
                    pStr = pStr.TrimStart(new char[] { '+', '0' });
                    if (pStr.Length == 13)
                        pStr = "0" + pStr;
                }
            }

            return pStr;
        }

        void MY_CHECK_DATA_VALID(short pTrCode, DataTable pTable)
        {
            foreach (DataRow row_ in pTable.Rows)
            {
                string itemName_ = CASTASSTRING(TAB_GETROW(row_, ITM_NAME));

                string itemCode_ = MY_GET_ITEMCODE(pTrCode, itemName_);
                TAB_SETROW(row_, ITM_CODE, itemCode_);

                string itemAmount_ = CASTASSTRING(TAB_GETROW(row_, ITM_AMOUNT));
                string itemPrice_ = CASTASSTRING(TAB_GETROW(row_, ITM_PRICE));



            }

        }


        bool MY_IS_DUMMY(string pCol)
        {
            try
            {
                return (int.Parse(pCol) >= 500);
            }
            catch { }
            return false;
        }

        bool MY_IS_FLOAT(string pTab, string pCol)
        {
            Type t_ = MY_GET_COL_TYPE(pTab, pCol);
            return ((t_ == typeof(double)) || (t_ == typeof(int)) || (t_ == typeof(short)) || (t_ == typeof(decimal)) || (t_ == typeof(float)));
        }

        bool MY_IS_DATE(string pTab, string pCol)
        {
            Type t_ = MY_GET_COL_TYPE(pTab, pCol);
            return (t_ == typeof(DateTime));

        }
        bool MY_IS_STRING(string pTab, string pCol)
        {
            Type t_ = MY_GET_COL_TYPE(pTab, pCol);
            return (t_ == typeof(string));
        }
        Type MY_GET_COL_TYPE(string pTab, string pCol)
        {
            return GETCOLUMNTYPE(pTab, pCol);
        }
        bool MY_IS_VALID_DS_COL(string pTab, string pCol)
        {
            try
            {
                return GETCOLUMNTYPE(pTab, pCol) != null;
            }
            catch { }

            return false;
        }
        string MY_CHECKDATE(string pDate)
        {

            string ptrn_ = "1900-01-01 00-00-00";
            string res_ = pDate;
            if (pDate.Length < ptrn_.Length)
                res_ = pDate + ptrn_.Substring(pDate.Length, ptrn_.Length - pDate.Length);
            return res_;

        }
        bool MY_IS_EMPTY(object pVal)
        {
            if (pVal == null)
                return true;

            if (pVal.ToString().Trim() == "")
                return true;

            if (pVal.ToString().Trim() == "0")
                return true;

            return false;
        }




        public static string MY_CHOOSE_SQL(string pSqlMs, string pSqlPg)
        {

            if (ISMSSQL())
                return pSqlMs;

            if (ISPOSTGRESQL())
                return pSqlPg;


            throw new Exception("Undefined datasource");
        }


        #endregion