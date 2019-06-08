#line 2


         #region BarcodeTool
        //BEGIN



        const int VERSION = 14;

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

                    x.ADD_IF_NOT_EXISTS = s.MY_BARCODE_ADD_IF_NOT_EXISTS;
                    x.USE_CODE = s.MY_BARCODE_USE_CODE;
                    x.WEIGHT_PAT = s.MY_BARCODE_WEIGHT_PAT;

                    x.ASK_QUANTITY = s.MY_BARCODE_ASK_QUANTITY;
                    x.ASK_PRICE = s.MY_BARCODE_ASK_PRICE;
                    x.DEFAULT_QUANTITY = s.MY_BARCODE_DEFAULT_QUANTITY;
                    x.MY_BARCODE_GTIN14 = s.MY_BARCODE_GTIN14;

                    
                    //

                    _SETTINGS.BUF = x;

                }

                public bool ADD_IF_NOT_EXISTS;
                public bool USE_CODE;
                public bool ASK_QUANTITY;
                public bool ASK_PRICE;
                public int DEFAULT_QUANTITY;
                public string WEIGHT_PAT;

                public bool MY_BARCODE_GTIN14;
                
            }



            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_BARCODE)
            {

            }

            [ECategory(TEXT.text_BARCODE)]
            [EDisplayName("Add if barcode not exists")]
            public bool MY_BARCODE_ADD_IF_NOT_EXISTS
            {
                get { return PARSEBOOL(_GET("MY_BARCODE_ADD_IF_NOT_EXISTS", "1")); }
                set { _SET("MY_BARCODE_ADD_IF_NOT_EXISTS", value); }

            }

            [ECategory(TEXT.text_BARCODE)]
            [EDisplayName("Search By Material Code")]
            public bool MY_BARCODE_USE_CODE
            {
                get { return PARSEBOOL(_GET("MY_BARCODE_USE_CODE", "1")); }
                set { _SET("MY_BARCODE_USE_CODE", value); }

            }

            [ECategory(TEXT.text_BARCODE)]
            [EDisplayName("Weight Barcode")]
            public string MY_BARCODE_WEIGHT_PAT
            {
                get { return PARSESTRING(_GET("MY_BARCODE_WEIGHT_PAT", "22MMMMMWWXXXC")); }
                set { _SET("MY_BARCODE_WEIGHT_PAT", value); }

            }

            [ECategory(TEXT.text_BARCODE)]
            [EDisplayName("Ask Quantity")]
            public bool MY_BARCODE_ASK_QUANTITY
            {
                get { return PARSEBOOL(_GET("MY_BARCODE_ASK_QUANTITY", "1")); }
                set { _SET("MY_BARCODE_ASK_QUANTITY", value); }

            }

            [ECategory(TEXT.text_BARCODE)]
            [EDisplayName("Ask Price")]
            public bool MY_BARCODE_ASK_PRICE
            {
                get { return PARSEBOOL(_GET("MY_BARCODE_ASK_PRICE", "1")); }
                set { _SET("MY_BARCODE_ASK_PRICE", value); }

            }

            [ECategory(TEXT.text_BARCODE)]
            [EDisplayName("Default Quantity")]
            public int MY_BARCODE_DEFAULT_QUANTITY
            {
                get { return PARSEINT(_GET("MY_BARCODE_DEFAULT_QUANTITY", "1")); }
                set { _SET("MY_BARCODE_DEFAULT_QUANTITY", value); }

            }

            [ECategory(TEXT.text_BARCODE)]
            [EDisplayName("Correct Barcode to GTIN 14")]
            public bool MY_BARCODE_GTIN14
            {
                get { return PARSEBOOL(_GET("MY_BARCODE_GTIN14", "0")); }
                set { _SET("MY_BARCODE_GTIN14", value); }

            }
        }

        #endregion


        public class TEXT
        {
            public const string text_BARCODE = "Barcode Input for Stock Doc";
            public const string MSG_ERROR_MATERIAL = "T_MSG_INVALID_MATERIAL";
            public const string MSG_ERROR_MATERIAL_UNIT = "T_MSG_INVALID_RECODR (T_UNIT)";
            public const string MSG_ERROR_MATERIAL_PRICE = "T_MSG_INVALID_RECODR (T_PRICE)";
            public const string MSG_ERROR_MATERIAL_BARCODE = "T_MSG_INVALID_BARCODE";
            public const string MSG_ASK_CREATE_MAT = "T_MSG_INVALID_BARCODE. T_MSG_COMMIT_ADD_NEW";

        }



        const string event_BARCODE_INPUT = "hadlericom_barcode_input";
        const string event_BARCODE_GENERATE = "hadlericom_barcode_gen";


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


            _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_BARCODE_INPUT, TEXT.text_BARCODE);
            // _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_ITS_SERIALNR, "Seri No.");
        }
        void _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(Control pCtrl, string pEvent, string pText)
        {
            if (pCtrl == null)
                return;
            try
            {


                var args = new Dictionary<string, object>() { 
            
			{ "_cmd" ,""},
            { "_type" ,""},
			{ "CmdText" ,"event name::"+pEvent},
			{ "Text" ,pText},
			{ "ImageName" ,"barcode_16x16"},
			{ "Width" ,100},
            };

                RUNUIINTEGRATION(pCtrl, args);


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


                    case event_BARCODE_INPUT:
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


            try
            {
                while (true)
                {


                    string barcode_ = MY_GET_TEXT();
                    if (barcode_ == null)
                        return;
                    barcode_ = barcode_.Trim();
                    if (barcode_ == "")
                        return;

                    var barcodeObj_ = new BARCODE_ITEM(barcode_, this);
                    var materialObj_ = new MATRECORD();

                    if (!fillMatData(barcodeObj_.GTIN, barcodeObj_.PLU, materialObj_))
                        return;


                    DataRow row_ = addRecord(pTabLines, barcodeObj_, materialObj_);
                    //positionOnRow(row_);

                }

            }
            catch (Exception exc)
            {
                setMsgErr(exc.Message);
                //exceptionHandler(exc);

            }







        }

        string MY_GET_TEXT()
        {

            DataRow[] refData_ = REF("ref.gen.string desc::" + (STRENCODE("BARCODE"))); //
            if (refData_ == null || refData_.Length <= 0)
                return null;

            return CASTASSTRING(refData_[0]["VALUE"]);

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





        string MY_GET_ITEMCODE(string pItemDesc)
        {

            string itemCode_ = CASTASSTRING(ISNULL(SQLSCALAR(
                MY_CHOOSE_SQL(
                "SELECT TOP(1) CODE FROM LG_$FIRM$_ITEMS WHERE NAME LIKE @P1",
                 "SELECT CODE FROM LG_$FIRM$_ITEMS WHERE NAME LIKE @P1 LIMIT 1"),
                 new object[] { pItemDesc }), "null"));
            if (itemCode_ == "null")
                throw new Exception(TEXT.MSG_ERROR_MATERIAL + " [" + pItemDesc + "]");
            return itemCode_;

        }


        class ADD_MATERIAL_CLASS : _PLUGIN.PLUGIN_INTERFACE_METHODS, IDisposable
        {
            //MSGUSERINFO((string)DATASET.ExtendedProperties[PRM_CONST.PRM_ADP_CMD]);
            public string CMD;
            public string GUID;
            public string BARCODE;
            _PLUGIN PLUGIN;

            public ADD_MATERIAL_CLASS(_PLUGIN pPlugin)
            {
                PLUGIN = pPlugin;

            }
            public const string PRM_ADP_CMD = "_SYS_PRM_CMD_";
            public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
            {
                object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
                switch (EVENTCODE)
                {

                    case SysEvent.SYS_NEWFORM:
                        MY_SYS_NEWFORM(arg1 as Form);
                        break;
                    case SysEvent.SYS_ADPBEGIN:
                        MY_SYS_ADPBEGIN(arg1 as DataSet);
                        break;

                }
            }

            bool IS_MY_DS(DataSet pDs)
            {
                return (pDs != null && pDs.ExtendedProperties[PRM_ADP_CMD] == CMD);// && pDs.Tables["ITEMS"] != null);
            }

            public void MY_SYS_NEWFORM(Form FORM)
            {

                if (!IS_MY_DS(_PLUGIN.GETDATASETFROMADPFORM(FORM)))
                    return;

                _PLUGIN.BINDTOFORMLIFECYCLE(FORM, this);


            }
            public void MY_SYS_ADPBEGIN(DataSet DATASET) //adapter start
            {
                if (!IS_MY_DS(DATASET))
                    return;

                /*
               DataTable UNITBARCODE = DATASET.Tables["UNITBARCODE"];
               if(UNITBARCODE == null)
               return;
	 
                DataRow row_ = TAB_GETLASTROW(UNITBARCODE);
                if(row_ == null) row_ = TAB_ADDROW(UNITBARCODE);
                TAB_SETROW(row_,"BARCODE",BARCODE);
                */

                DataTable ITEMS = DATASET.Tables["ITEMS"];
                if (ITEMS == null)
                    return;

                DataRow row_ = TAB_GETLASTROW(ITEMS);
                TAB_SETROW(row_, "CODE", BARCODE);

            }
            public void Dispose()
            {

                if (PLUGIN != null)
                    PLUGIN.DELETESYSEVENTHANDLER(GUID);

                PLUGIN = null;

            }

        }
        object MY_ADD_MATERIAL(string pMatBarcode, int pPLU)
        {
            //ADDSYSEVENTHANDLER
            //DELETESYSEVENTHANDLER

            string guid_ = null;
            try
            {
                ADD_MATERIAL_CLASS c_ = new ADD_MATERIAL_CLASS(this);
                guid_ = ADDSYSEVENTHANDLER(c_);
                c_.CMD = "adp.mm.rec.mat/1 cmd::add  dialog::1 guid::" + guid_;
                c_.GUID = guid_;
                c_.BARCODE = pPLU>0 ? "" : pMatBarcode;
                return EXECMDTEXT(c_.CMD);
            }
            finally
            {
                DELETESYSEVENTHANDLER(guid_);
            }

        }
        ///////////////////////////////////////////////////////////////////////////////////
        //CHECK DATA TYPES
        ///////////////////////////////////////////////////////////////////////////////////



        static bool ISINNERBARCODE(string pBarcode)
        {
            return pBarcode.Length == 13 && LEFT(pBarcode, 1) == "2";



        }

        bool fillMatData(string pBarcode, int pPLU, MATRECORD pObj)
        {
            pObj.ROW_ITEMBARCODE = SEARCH_MAT_LINE_BARCODE(pBarcode, pPLU);

            if (pObj.ROW_ITEMBARCODE == null)
            {
                if (_SETTINGS.BUF.ADD_IF_NOT_EXISTS)
                    if (ASK_ADD_MATERIAL(pBarcode))
                    {
                        object lref_ = MY_ADD_MATERIAL(pBarcode, pPLU);
                        if (!ISEMPTYLREF(lref_))
                        {

                            pObj.ROW_ITEMBARCODE = SEARCH_BARCODE(lref_);
                            if (pObj.ROW_ITEMBARCODE == null)
                            {
                                ERROR_INVALID_MAT_REF(lref_);
                                return false;
                            }



                        }


                    }
                    else
                        return false;
            }

            if (pObj.ROW_ITEMBARCODE == null)
            {
                ERROR_INVALID_MAT_BARCODE(pBarcode);
                return false;
            }

            //pObj.BARCODE = pMatBarcode;
            pObj.ITEMREF = TAB_GETROW(pObj.ROW_ITEMBARCODE, "ITEMREF");
            pObj.UNITREF = TAB_GETROW(pObj.ROW_ITEMBARCODE, "UNITLINEREF");

            pObj.ROW_ITEM = SEARCH_MAT_LINE(pObj.ITEMREF);
            if (pObj.ROW_ITEM == null)
            {
                ERROR_INVALID_MAT_REF(pObj.ITEMREF);
                return false;
            }
            //pObj.ROW_ITEMBARCODE = pObj.ROW_ITEMBARCODE;
            pObj.ROW_ITEMUNITEXT = SEARCH_MAT_UNIT(pObj.ITEMREF, pObj.UNITREF);
            if (pObj.ROW_ITEMUNITEXT == null)
            {
                ERROR_INVALID_MAT_UNIT(pObj.ITEMREF);
                return false;
            }


            return true;
        }

        DataRow _addRecordRaw(DataTable pTabLines, BARCODE_ITEM pBarcode, MATRECORD pMaterial)
        {

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
            TAB_SETROW(row_, "STOCKREF", pMaterial.ITEMREF);
            TAB_SETROW(row_, "UOMREF", pMaterial.UNITREF);



            if (_SETTINGS.BUF.DEFAULT_QUANTITY > 0)
                TAB_SETROW(row_, "AMOUNT", _SETTINGS.BUF.DEFAULT_QUANTITY);

            if (_SETTINGS.BUF.ASK_QUANTITY)
            {
                double quantity_ = askNumber("T_QUANTITY", pBarcode.AMOUNT > 0 ? pBarcode.AMOUNT : 1);
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
        DataRow addRecord(DataTable pTabLines, BARCODE_ITEM pBarcode, MATRECORD pMaterial)
        {


            return _addRecordRaw(pTabLines, pBarcode, pMaterial);
        }

        static int GETPLU(string pBarcode)
        {
            try
            {
                if (pBarcode.StartsWith("+") && pBarcode.Length < 6)
                    return PARSEINT(pBarcode.TrimStart('+'));
            }
            catch
            {

            }

            return 0;
        }
        DataRow SEARCH_MAT_LINE_BARCODE(string pBarcode, int pPLU)
        {

            if (pBarcode == "")
                return null;

            if (pPLU > 0)//plu
            {

                try
                {

                    int plu_ = pPLU;
                    if (plu_ > 0)
                    {
                        return TAB_GETLASTROW(SQL(
                            MY_CHOOSE_SQL(
@"
                        SELECT TOP(1) 
                        LOGICALREF,
                        LOGICALREF ITEMREF,
                        (select top(1) LOGICALREF from LG_$FIRM$_UNITSETL with(nolock) where UNITSETREF = I.UNITSETREF AND MAINUNIT = 1) UNITLINEREF,
                        CODE BARCODE 
                        FROM 
                        LG_$FIRM$_ITEMS I with(nolock) WHERE INTF2 = @P1",
@"
                        SELECT 
                        LOGICALREF,
                        LOGICALREF ITEMREF,
                        (select LOGICALREF from LG_$FIRM$_UNITSETL 
                        where UNITSETREF = I.UNITSETREF AND MAINUNIT = 1 LIMIT 1) UNITLINEREF,
                        CODE BARCODE 
                        FROM 
                        LG_$FIRM$_ITEMS I WHERE INTF2 = @P1
                        LIMIT 1
"),

                         new object[] { plu_ }));
                    }
                    else
                    {
                        return null;
                    }

                }
                catch
                {


                }

                return null;
            }

            if (_SETTINGS.BUF.USE_CODE)
            {
                var barcode1 = pBarcode;
                var barcode2 = pBarcode;

                if (_SETTINGS.BUF.MY_BARCODE_GTIN14)
                {
                    if (barcode2.Length == 13)
                        barcode2 = "0" + barcode2; //gtin
                }

                return TAB_GETLASTROW(SQL(
                    MY_CHOOSE_SQL(
@"
                        SELECT TOP(1) 
                        LOGICALREF,
                        LOGICALREF ITEMREF,
                        (select top(1) LOGICALREF from LG_$FIRM$_UNITSETL with(nolock) where UNITSETREF = I.UNITSETREF AND MAINUNIT = 1) UNITLINEREF,
                        CODE BARCODE 
                        FROM 
                        LG_$FIRM$_ITEMS I with(nolock) WHERE CODE IN (@P1,@P2)",
@"
                        SELECT 
                        LOGICALREF,
                        LOGICALREF ITEMREF,
                        (select LOGICALREF from LG_$FIRM$_UNITSETL 
                        where UNITSETREF = I.UNITSETREF AND MAINUNIT = 1 LIMIT 1) UNITLINEREF,
                        CODE BARCODE 
                        FROM 
                        LG_$FIRM$_ITEMS I WHERE CODE IN (@P1,@P2)
                        LIMIT 1"),
                 new object[] { barcode1, barcode2 }));



            }



            return TAB_GETLASTROW(SQL(
                MY_CHOOSE_SQL(
                @"
SELECT TOP(1) LOGICALREF,ITEMREF,UNITLINEREF,BARCODE FROM LG_$FIRM$_UNITBARCODE with(nolock) 
WHERE BARCODE= @P1",
                    @"
SELECT LOGICALREF,ITEMREF,UNITLINEREF,BARCODE FROM LG_$FIRM$_UNITBARCODE  
WHERE BARCODE= @P1 LIMIT 1"),
                   new object[] { pBarcode }));
        }
        DataRow SEARCH_BARCODE(object pMatLref)
        {
            string br_ = "";
            if (_SETTINGS.BUF.USE_CODE)
            {
                br_ = CASTASSTRING(ISNULL(SQLSCALAR(
                    MY_CHOOSE_SQL(
@"
                        SELECT TOP(1) CODE
                        FROM 
                        LG_$FIRM$_ITEMS I with(nolock) WHERE LOGICALREF = @P1",
@"
                        SELECT CODE
                        FROM 
                        LG_$FIRM$_ITEMS I WHERE LOGICALREF = @P1 LIMIT 1"),

                new object[] { pMatLref }), ""));



            }
            else
                br_ = CASTASSTRING(SQLSCALAR(
                    MY_CHOOSE_SQL(
                    @"
SELECT TOP(1) BARCODE FROM LG_$FIRM$_UNITBARCODE with(nolock) 
WHERE ITEMREF =@P1 ORDER BY ITEMREF DESC,UNITLINEREF DESC,TYP DESC,LINENR DESC",
                       @"
SELECT BARCODE FROM LG_$FIRM$_UNITBARCODE 
WHERE ITEMREF =@P1 ORDER BY ITEMREF DESC,UNITLINEREF DESC,TYP DESC,LINENR DESC LIMIT 1"),

                                                                               new object[] { pMatLref }));

            return SEARCH_MAT_LINE_BARCODE(br_,0);
        }
        DataRow SEARCH_MAT_LINE(object pMatLRef)
        {
            DataRow resMat_ = TAB_GETLASTROW(SQL(
                MY_CHOOSE_SQL(
                @"
                select top(1) LOGICALREF,CODE,NAME,UNITSETREF from LG_$FIRM$_ITEMS with(nolock) 
where LOGICALREF = @P1
                ", @"
                select LOGICALREF,CODE,NAME,UNITSETREF from LG_$FIRM$_ITEMS 
where LOGICALREF = @P1 LIMIT 1
                "), new object[] { pMatLRef }));
            return resMat_;
        }
        DataRow SEARCH_MAT_PRICE(object pMatLRef, object pUnitLRef)
        {
            DataRow resPrice_ = TAB_GETLASTROW(SQL(
                MY_CHOOSE_SQL(
                @"
                SELECT TOP(1) (case when P.INCVAT = 1 then P.PRICE else P.PRICE * (1+(I.VAT/100)) end) PRICE FROM LG_$FIRM$_PRCLIST P with(nolock) inner join LG_$FIRM$_ITEMS I with(nolock) ON P.CARDREF = I.LOGICALREF WHERE P.CARDREF = @P1 AND P.PTYPE = 2 AND P.UOMREF = @P2 ORDER BY P.ENDDATE DESC
                ", @"
                SELECT (case when P.INCVAT = 1 then P.PRICE else P.PRICE * (1+(I.VAT/100)) end) PRICE 
FROM LG_$FIRM$_PRCLIST P inner join LG_$FIRM$_ITEMS I 
ON P.CARDREF = I.LOGICALREF WHERE P.CARDREF = @P1 AND P.PTYPE = 2 AND P.UOMREF = @P2 ORDER BY P.ENDDATE DESC
LIMIT 1
                "), new object[] { pMatLRef, pUnitLRef }));

            return resPrice_;
        }

        //        DataRow SEARCH_MAT_UNIT(object pMatLRef, object pUnitLRef)
        //        {
        //            DataRow resUnit_ = TAB_GETLASTROW(SQL(
        //                @"
        //                declare @unitSet int,@unitMainRef int,@CODE nvarchar(100),@NAME nvarchar(100)
        //
        //                select top(1) @unitMainRef = LOGICALREF,@CODE = CODE,@NAME = NAME from LG_$FIRM$_UNITSETL with(nolock) where LOGICALREF = @P2
        //                select top(1) ISNULL(@CODE,'') CODE,ISNULL(@NAME,'') NAME,* from LG_$FIRM$_ITMUNITA with(nolock) where ITEMREF = @P1 AND VARIANTREF >=0 AND UNITLINEREF = @unitMainRef
        //                ", new object[] { pMatLRef, pUnitLRef }));

        //            return resUnit_;

        //        }

        //        DataRow SEARCH_MAT_UNIT_MAIN(object pMatLRef)
        //        {
        //            DataRow resUnit_ = TAB_GETLASTROW(SQL(
        //                @"
        //                declare @unitSet int,@unitMainRef int,@CODE nvarchar(100),@NAME nvarchar(100)
        //                select top(1) @unitSet = UNITSETREF from LG_$FIRM$_ITEMS with(nolock) where LOGICALREF = @P1
        //                select top(1) @unitMainRef = LOGICALREF,@CODE = CODE,@NAME = NAME from LG_$FIRM$_UNITSETL with(nolock) where UNITSETREF = @unitSet AND MAINUNIT = 1
        //                select top(1) ISNULL(@CODE,'') CODE,ISNULL(@NAME,'') NAME,* from LG_$FIRM$_ITMUNITA with(nolock) where ITEMREF = @P1 AND VARIANTREF >=0 AND UNITLINEREF = @unitMainRef
        //                ", new object[] { pMatLRef }));

        //            return resUnit_;

        //        }


        DataRow SEARCH_MAT_UNIT(object pMatLRef, object pUnitLRef)
        {




            DataRow resUnit_ = TAB_GETLASTROW(SQL(MY_CHOOSE_SQL(
                @"
                declare @unitSet int,@unitMainRef int,@CODE nvarchar(100),@NAME nvarchar(100)

                select top(1) @unitMainRef = LOGICALREF,@CODE = CODE,@NAME = NAME from LG_$FIRM$_UNITSETL with(nolock) where LOGICALREF = @P2
                select top(1) ISNULL(@CODE,'') CODE,ISNULL(@NAME,'') NAME,* from LG_$FIRM$_ITMUNITA with(nolock) where ITEMREF = @P1 AND VARIANTREF >=0 AND UNITLINEREF = @unitMainRef
                
                ",


@"
select 
U.CODE,
U.NAME,
A.*
from LG_$FIRM$_ITMUNITA A 
inner join 
LG_$FIRM$_UNITSETL U 
on 
A.UNITLINEREF = U.LOGICALREF 
where 
A.ITEMREF = @P1 AND A.UNITLINEREF = @P2

 

                "),

         new object[] { pMatLRef, pUnitLRef }));

            return resUnit_;

        }

        DataRow SEARCH_MAT_UNIT_MAIN(object pMatLRef)
        {
            //TODO
            DataRow resUnit_ = TAB_GETLASTROW(SQL(MY_CHOOSE_SQL(
                @"
                declare @unitSet int,@unitMainRef int,@CODE nvarchar(100),@NAME nvarchar(100)
                select top(1) @unitSet = UNITSETREF from LG_$FIRM$_ITEMS with(nolock) where LOGICALREF = @P1
                select top(1) @unitMainRef = LOGICALREF,@CODE = CODE,@NAME = NAME from LG_$FIRM$_UNITSETL with(nolock) where UNITSETREF = @unitSet AND MAINUNIT = 1
                select top(1) ISNULL(@CODE,'') CODE,ISNULL(@NAME,'') NAME,* from LG_$FIRM$_ITMUNITA with(nolock) where ITEMREF = @P1 AND VARIANTREF >=0 AND UNITLINEREF = @unitMainRef
                ",
          @"

select 
U.CODE,
U.NAME,
A.*
from LG_$FIRM$_ITEMS I 
inner join 
LG_$FIRM$_UNITSETL U on U.UNITSETREF = I.UNITSETREF AND U.MAINUNIT = 1
inner join  
LG_$FIRM$_ITMUNITA A on A.ITEMREF = I.LOGICALREF AND A.UNITLINEREF = U.LOGICALREF
where 
I.LOGICALREF = @P1 


                "),

         new object[] { pMatLRef }));

            return resUnit_;

        }


        void ERROR_INVALID_MAT_REF(object pMatLRef)
        {
            setMsgErr(TEXT.MSG_ERROR_MATERIAL + " [" + pMatLRef + "]");

        }
        void ERROR_INVALID_MAT_UNIT(object pMatLRef)
        {
            setMsgErr(TEXT.MSG_ERROR_MATERIAL_UNIT + " [" + pMatLRef + "]");

        }
        void ERROR_INVALID_MAT_PRICE(object pMatLRef, object pUnitLRef)
        {
            setMsgErr(TEXT.MSG_ERROR_MATERIAL_PRICE + " [" + pMatLRef + "/" + pUnitLRef + "]");

        }
        void ERROR_INVALID_MAT_BARCODE(object pBarcode)
        {
            setMsgErr(TEXT.MSG_ERROR_MATERIAL_BARCODE + " [" + pBarcode + "]");

        }
        bool ASK_ADD_MATERIAL(object pBarcode)
        {
            if (ISINNERBARCODE(pBarcode.ToString()))
                return false;

            return askMsg(TEXT.MSG_ASK_CREATE_MAT + " [" + pBarcode + "]");

        }

        void setMsgErr(string pVal)
        {
            MSGUSERINFO(pVal);
        }
        bool askMsg(string pVal)
        {
            return MSGUSERASK(pVal);
        }
        void exceptionHandler(Exception exc)
        {
            LOG(exc.ToString());
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

        ///////////////////////////////////////////////////////////////////////////////////
        class BARCODE_ITEM
        {

            public BARCODE_ITEM(string pBarcode, _PLUGIN pPLUGIN)
            {

                try
                {
                    PLU = GETPLU(pBarcode);

                    if (PLU > 0)
                        return;

                    pBarcode = pBarcode.Trim().Trim('+');


                    var isIts = (
                        pBarcode.Length > (2 + 14 + 2) &&
                        pBarcode.Substring(0, 2) == ("01") && //GTIN
                        pBarcode.Substring((2 + 14), 2) == ("21") && //SN
                        pBarcode.IndexOf("17", 2 + 14) > 0 && //XD
                         pBarcode.IndexOf("10", 2 + 14) > 0 //BN
                        );


                    if (isIts)
                    {
                        pBarcode = pBarcode.Substring(2, 14);

                        pBarcode = ISNULL(
                            pPLUGIN.SQLSCALAR(
                            MY_CHOOSE_SQL(
                            "SELECT CODE FROM LG_$FIRM$_ITEMS WITH(NOLOCK) WHERE CODE IN (@P1,@P2)",
                            "SELECT CODE FROM LG_$FIRM$_ITEMS WHERE CODE IN (@P1,@P2)"),
                            new object[] { pBarcode, pBarcode.TrimStart('0') }),
                            pBarcode) as string;
                    }

                    var p = new PARSER(pBarcode, _SETTINGS.BUF.WEIGHT_PAT);

                    GTIN = p.CODE1;
                    AMOUNT = p.WEIGHT;





                }
                catch (Exception exc)
                {
                    throw new Exception(TEXT.MSG_ERROR_MATERIAL_BARCODE + " [" + pBarcode + "] " + exc.Message);
                }

            }

            public string GTIN;

            public int PLU = 0;

            public double AMOUNT = 0;
        }

        public class PARSER
        {

            const char prmPriceNumber = 'P';
            const char prmPriceDecimal = 'Q';
            const char prmWeithNumber = 'W';
            const char prmWeithDecimal = 'X';
            const char prmCode1 = 'M';
            const char prmCode2 = 'N';

            public string BARCODE { get; private set; }
            public string FORMAT { get; private set; }




            public static bool ISMACH(string pBarcode, string pFormat)
            {
                if (pBarcode == null || pFormat == null)
                    return false;

                if (pBarcode.Length != pFormat.Length || pBarcode.Length == 0 || pFormat.Length == 0)
                    return false;

                for (int i = 0; i < pBarcode.Length; ++i)
                {
                    var b = pBarcode[i];
                    var p = pFormat[i];
                    if (char.IsDigit(b) && char.IsDigit(p))
                    {
                        if (b != p)
                            return false;

                    }

                }

                return true;

            }

            public PARSER(string pBarcode, string pFormat)
            {
                BARCODE = pBarcode;

                if (ISMACH(pBarcode, pFormat))
                    FORMAT = pFormat;
                else
                    FORMAT = "".PadLeft(pBarcode.Length, 'M');

                init();
            }




            void init()
            {
                try
                {
                    if (BARCODE == null || FORMAT == null || BARCODE.Length != FORMAT.Length || BARCODE.Length == 0)
                        throw new Exception();


                    StringBuilder sbCode1_ = new StringBuilder();
                    StringBuilder sbCode2_ = new StringBuilder();

                    StringBuilder sbWeightN_ = new StringBuilder();
                    StringBuilder sbWeightD_ = new StringBuilder();
                    StringBuilder sbPriceN_ = new StringBuilder();
                    StringBuilder sbPriceD_ = new StringBuilder();
                    for (int i = 0; i < FORMAT.Length; ++i)
                    {
                        char f = FORMAT[i];
                        char v = BARCODE[i];
                        switch (f)
                        {
                            case prmCode1:
                                sbCode1_.Append(v);
                                break;
                            case prmCode2:
                                sbCode2_.Append(v);
                                break;
                            case prmWeithDecimal:
                                sbWeightD_.Append(v);
                                break;
                            case prmWeithNumber:
                                sbWeightN_.Append(v);
                                break;
                            case prmPriceDecimal:
                                sbPriceD_.Append(v);
                                break;
                            case prmPriceNumber:
                                sbPriceN_.Append(v);
                                break;
                        }

                    }

                    _WEIGHT = PARSEDOUBLE((sbWeightN_.Length > 0 ? sbWeightN_.ToString() : "0") + "." + (sbWeightD_.Length > 0 ? sbWeightD_.ToString() : "0"));
                    _PRICE = PARSEDOUBLE((sbPriceN_.Length > 0 ? sbPriceN_.ToString() : "0") + "." + (sbPriceD_.Length > 0 ? sbPriceD_.ToString() : "0"));
                    _CODE1 = sbCode1_.ToString();//.TrimStart('0');
                    _CODE2 = sbCode2_.ToString();//.TrimStart('0');
                }
                catch
                {
                    throw new Exception(TEXT.MSG_ERROR_MATERIAL_BARCODE + " [" + BARCODE + "] != [" + FORMAT + "]");
                }

            }
            double _WEIGHT;
            public double WEIGHT
            {
                get { return _WEIGHT; }
            }

            double _PRICE;
            public double PRICE
            {
                get { return _PRICE; }
            }
            string _CODE1;
            public string CODE1
            {
                get { return _CODE1 == null ? "" : _CODE1; }
            }

            string _CODE2;
            public string CODE2
            {
                get { return _CODE2 == null ? "" : _CODE2; }
            }

            public bool ISEMPTY
            {
                get { return string.IsNullOrEmpty(CODE1) && string.IsNullOrEmpty(CODE2); }

            }

        }
        class MATRECORD
        {
            public string BARCODE;
            public object ITEMREF;
            public object UNITREF;

            public DataRow ROW_ITEM;
            public DataRow ROW_ITEMBARCODE;
            public DataRow ROW_ITEMUNITEXT;
            public DataRow ROW_ITEMPRICE;
        }


        ///////////////////////////////////////////////////////////////////////////////////		




        public static string MY_CHOOSE_SQL(string pSqlMs, string pSqlPg)
        {

            if (ISMSSQL())
                return pSqlMs;

            if (ISPOSTGRESQL())
                return pSqlPg;


            throw new Exception("Undefined datasource");
        }


        #endregion
