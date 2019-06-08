#line 2


        #region PLUGIN_BODY
        const int VERSION = 5;

        const string FILE = "plugin.sys.event.pricing.production.pls";



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

                    x.MY_PRICING_PRODUCTION_USER = s.MY_PRICING_PRODUCTION_USER;

                    //

                    _SETTINGS.BUF = x;

                }

                public string MY_PRICING_PRODUCTION_USER;



            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC) //, "ava.production.config")
            {

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active On User")]
            public string MY_PRICING_PRODUCTION_USER
            {
                get
                {
                    return (_GET("MY_PRICING_PRODUCTION_USER", "1,2"));
                }
                set
                {
                    _SET("MY_PRICING_PRODUCTION_USER", value);
                }

            }



            //

            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_PRICING_PRODUCTION_USER),
                     FORMAT(pPLUGIN.GETSYSPRM_USER())
                     ) >= 0;
            }

        }

        #endregion



        #region TEXT

        public class TEXT
        {
            public const string text_DESC = "Pricing For Production";



            public class L
            {

            }
        }


        const string event_PRICING_ = "_pricing_";
        const string event_PRICING_PRODUCTION_ = "_pricing_production_";
        const string event_PRICING_PRODUCTION_DUMPMAP_SINGLE = "_pricing_production_dumpmat_single";
        const string event_PRICING_PRODUCTION_DUMPMAP_ALL = "_pricing_production_dumpmat_all";
        const string event_PRICING_PRODUCTION_UPDATE = "_pricing_production_update";


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



            public string UPDATE_PRICE = "Price Update";
            public string UPDATE_PRICE_IN_PROD_BY_PRCLIST = "Update Production By Price List";


            public void lang_az()
            {

                UPDATE_PRICE = "Qiymət Təzələmə";
                UPDATE_PRICE_IN_PROD_BY_PRCLIST = "İsteh. Qiymət Təzələmə Aliş Qiy. Listəsi";
            }

            public void lang_ru()
            {

                UPDATE_PRICE = "Обновить Цены";
                UPDATE_PRICE_IN_PROD_BY_PRCLIST = "Обновить Цены Произ. по П.Листу Закупки";
            }

            public void lang_tr()
            {



            }
        }


        #endregion

        #region MAIN



        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {

            if (ISWEB())
                return;


            _SETTINGS._BUF.LOAD_SETTINGS(this);


            if (!_SETTINGS.ISUSEROK(this))
                return;

            CHECK_LOC_DS(this);

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
			{ "Text" ,_LANG.L.UPDATE_PRICE},
			{ "ImageName" ,"money_32x32"},
			{ "Name" ,event_PRICING_},
            };

                        RUNUIINTEGRATION(tree, args);

                    }


                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_PRICING_},
			{ "CmdText" ,"event name::"+event_PRICING_PRODUCTION_UPDATE},
			{ "Text" ,_LANG.L.UPDATE_PRICE_IN_PROD_BY_PRCLIST},
			{ "ImageName" ,"refresh_32x32"},
		 { "Name" ,event_PRICING_PRODUCTION_UPDATE},
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

                foreach (var ctrl in CONTROL_DESTRUCT(FORM))
                {
                    {
                        var menuItem = ctrl as ToolStripItem;
                        if (menuItem != null && menuItem.Name == "cMenuMainInfo")
                        {
                            {
                                var args = new Dictionary<string, object>() { 
 
            { "_cmd" ,"add"},
            { "_type" ,"item"},
            { "_name" , event_PRICING_PRODUCTION_DUMPMAP_SINGLE},

            { "_infoLocation" ,"event"},
            { "_infoArg" ,event_PRICING_PRODUCTION_DUMPMAP_SINGLE},

            { "Text" ,LANG("T_PRODUCTION (T_SELECTED)")},
            { "ImageName" ,"func_16x16"},
             { "Name" ,event_PRICING_PRODUCTION_DUMPMAP_SINGLE},
            };

                                RUNUIINTEGRATION(menuItem, args);

                            }

                            {
                                var args = new Dictionary<string, object>() { 
 
            { "_cmd" ,"add"},
            { "_type" ,"item"},
            { "_name" , event_PRICING_PRODUCTION_DUMPMAP_ALL},

            { "_infoLocation" ,"event"},
            { "_infoArg" ,event_PRICING_PRODUCTION_DUMPMAP_ALL},

            { "Text" ,LANG("T_PRODUCTION (T_ALL)")},
            { "ImageName" ,"func_16x16"},
             { "Name" ,event_PRICING_PRODUCTION_DUMPMAP_ALL},
            };

                                RUNUIINTEGRATION(menuItem, args);

                            }

                        }
                    }




                }


            }


        }


        Button _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(Control pCtrl, string pEvent, string pText, string pImg = null)
        {
            if (pCtrl == null)
                return null;
            try
            {


                var args = new Dictionary<string, object>() { 
            
			{ "_cmd" ,""},
            { "_type" ,""},
			{ "CmdText" ,"event name::"+pEvent},
			{ "Text" ,pText},
			{ "ImageName" , pImg??"right_16x16"},
			{ "Width" ,100},
            { "AutoSize",true}
            };

                var b = RUNUIINTEGRATION(pCtrl, args) as Button;
                if (b != null)
                {
                    var w = b.Width + (6 * 2) + 16;
                    b.AutoSize = false;
                    b.Width = w;
                }

            }
            catch (Exception exc)
            {
                MSGUSERERROR("Cant add button: " + exc.Message);
            }
            return null;
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


                    case event_PRICING_PRODUCTION_UPDATE:

                        MY_PRICING_PRODUCTION_UPDATE();

                        break;
                    case event_PRICING_PRODUCTION_DUMPMAP_SINGLE:
                        if (ISREFERENCEFORM(arg1 as Form))
                        {
                            var form = arg1 as Form;
                            if (form == null)
                                return;

                            var grid = CONTROL_SEARCH(form, "cGrid") as DataGridView;
                            if (grid == null)
                                return;

                            var dataRecord = RUNUIINTEGRATION(arg1, "_cmd", "record") as DataRow;
                            MY_PRICING_PRODUCTION_DUMPMAP_SINGLE(dataRecord);

                            //
                        }




                        break;

                    case event_PRICING_PRODUCTION_DUMPMAP_ALL:
                        MY_PRICING_PRODUCTION_DUMPMAP_ALL();

                        break;

                }


            }
            catch (Exception exc)
            {
                LOG(exc);
                MSGUSERERROR(exc.Message);
            }
        }





        #region HANDLERS
        void MY_PRICING_PRODUCTION_DUMPMAP_ALL()
        {
            var filter = MY_ASK_STRING(this, "T_FILTER", "");
            if (ISEMPTY(filter))
                return;

            var mats = SQL(@"


WITH __FILTER AS (
select  
distinct MAINCREF as LOGICALREF from LG_$FIRM$_STCOMPLN 
union
select distinct LOGICALREF from LG_$FIRM$_USERDOC4FFIRM where TRCODE = 1
)

SELECT LOGICALREF FROM LG_$FIRM$_ITEMS as I WHERE CODE LIKE @P1 AND ACTIVE = 0 AND
EXISTS(SELECT 1 FROM __FILTER AS F where F.LOGICALREF = I.LOGICALREF)
ORDER BY NAME ASC

", new object[] { filter });

            var excel = new MY_EXCEL();
            foreach (DataRow rec in mats.Rows)
            {
                var matRef = TAB_GETROW(rec, "LOGICALREF");

                var resOk = __MY_PRICING_PRODUCTION_DUMPMAP(matRef, excel);
            }

            excel.FINISH();
 
            var name = MAKENAME("all", "");

            var file = MY_DIR.SAVE(excel.content, name, ".xls");

            PROCESS(file, null);
        }
        void MY_PRICING_PRODUCTION_DUMPMAP_SINGLE(DataRow pRow)
        {
            if (pRow == null)
                return;

            var matRef = TAB_GETROW(pRow, "LOGICALREF");

            var excel = new MY_EXCEL();

            var resOk = __MY_PRICING_PRODUCTION_DUMPMAP(matRef, excel);

            if (!resOk)
            {
                MSGUSERINFO("T_MSG_DATA_NO - T_SCRIPT");
                return;
            }

            excel.FINISH();

            var matDescTop = GET_MAT_DESC(this, matRef);

            var name = MAKENAME(matDescTop, "");

            var file = MY_DIR.SAVE(excel.content, name, ".xls");

            PROCESS(file, null);

        }
        bool __MY_PRICING_PRODUCTION_DUMPMAP(object pLRef, MY_EXCEL excel)
        {

            var matRef = pLRef;

            //check template exists
            var tmpRef = MY_TOOLS_QPROD.MY_PROC_TEMPLATE(this, matRef, true);

            if (ISEMPTYLREF(tmpRef))
            {
                return false;
            }

            var matDescTop = GET_MAT_DESC(this, matRef);

            var prodSet = new MY_TOOLS_QPROD.PROD_SET();

            MY_TOOLS_QPROD.MY_PROD_UNWRAP_ITEM(this, matRef, 1.0, prodSet, 3, true);




            excel.APPEND_HEADER(new string[] { LANG("T_NR"), LANG("T_TITLE"), LANG("T_QUANTITY"), LANG("T_PRICE"), LANG("T_TOTAL (T_SYS_CURR1)") });

            double cost = 0.0;

            var indx = 0;

            foreach (var prodItem in prodSet.LIST)
            {
                if (!prodItem.HASSCRIPT)
                {
                    ++indx;

                    var amnt = prodItem.AMOUNT;

                    var price = GET_PRICE_LIST_PRCH(this, prodItem.LOGICALREF);

                    var tot = ROUND(price * prodItem.AMOUNT, 6);
                    cost += tot;

                    var t = new object[]{
                       FORMAT( indx),
                        prodItem.INFO_DESC,
                      ( ROUND(prodItem.AMOUNT, 6) ),
                       ( ROUND(price, 6) ),
                       (  tot ),
                    };

                    excel.APPEND_OBJ(t);


                }
            }


            excel.APPEND_OBJ(new object[] { "---", matDescTop, 1.0, cost, cost }, true);

            excel.APPEND_OBJ(new object[] { });

            return true;
        }


        void MY_PRICING_PRODUCTION_UPDATE()
        {


            if (!MSGUSERASK("T_MSG_COMMIT_BEGIN - " + _LANG.L.UPDATE_PRICE_IN_PROD_BY_PRCLIST))
                return;



            //   EXECMDTEXT("event name::_pricing_fromprch_update_outcall");

            var sqlMats = @"

 select distinct MAINCREF LOGICALREF from LG_$FIRM$_STCOMPLN

";


            var items = SQL(sqlMats, new object[] { });
            var me = this;

            INVOKEINBATCH((s, e) =>
            {

                foreach (DataRow r in items.Rows)
                {

                    var lref = TAB_GETROW(r, "LOGICALREF");

                    var cost = MY_CALC.QPROD_COST(me, lref);

                    (new MY_SAVEPRICE(me)
                    {
                        matRef = lref,
                        pricePrch = cost

                    }).RUN();
                }

            }, null);





            MSGUSERINFO("T_MSG_OPERATION_OK - " + _LANG.L.UPDATE_PRICE_IN_PROD_BY_PRCLIST);
        }




        #region SQL


        #endregion
        #endregion












        #region Help

        class MY_DIR
        {
            public static string PRM_DIR_ROOT = PATHCOMBINE(
     Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "PRODUCTION");

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


        class MY_EXCEL
        {
            public StringBuilder content = new StringBuilder();


            public MY_EXCEL()
            {
                content.AppendLine(@"<?xml version='1.0'?>
<?mso-application progid='Excel.Sheet'?>
<Workbook xmlns='urn:schemas-microsoft-com:office:spreadsheet'
 xmlns:o='urn:schemas-microsoft-com:office:office'
 xmlns:x='urn:schemas-microsoft-com:office:excel'
 xmlns:ss='urn:schemas-microsoft-com:office:spreadsheet'
 xmlns:html='http://www.w3.org/TR/REC-html40'>
 <Styles>
  <Style ss:ID='Bold'>
   <Font ss:Bold='1'/>
  </Style>
 </Styles>
<Worksheet ss:Name='DATA'>
<Table>");
            }

            public void FINISH()
            {
                content.AppendLine(@"
</Table>
</Worksheet>
</Workbook>");
            }



            public void APPEND_OBJ(object[] pData, bool pBold = false)
            {
                content.AppendLine("<Row " + (pBold ? "ss:StyleID='Bold'" : "") + ">");

                foreach (var itm in pData)
                {
                    var str = HTMLESC(FORMAT(itm));
                    var type = "String";

                    if (itm != null)
                    {
                        if (itm.GetType() == typeof(double))
                        {
                            type = "Number";
                        }
                    }

                    content.Append("<Cell><Data ss:Type='" + type + "'>").Append(str).Append("</Data></Cell>").AppendLine();
                }

                content.AppendLine("</Row>");
            }

            public void APPEND_STR(string[] pData, bool pBold = false)
            {
                content.AppendLine("<Row " + (pBold ? "ss:StyleID='Bold'" : "") + ">");

                foreach (var itm in pData)
                {
                    var val = HTMLESC(itm);
                    content.Append("<Cell><Data ss:Type='" + "String" + "'>").Append(val).Append("</Data></Cell>").AppendLine();
                }

                content.AppendLine("</Row>");
            }

            public void APPEND_HEADER(string[] pData)
            {
                content.AppendLine("<Row ss:StyleID='Bold'>");

                foreach (var itm in pData)
                {
                    var val = HTMLESC(itm);
                    content.Append("<Cell><Data ss:Type='" + "String" + "'>").Append(val).Append("</Data></Cell>").AppendLine();
                }

                content.AppendLine("</Row>");
            }

        }


        public static string MY_ASK_STRING(_PLUGIN pPLUGIN, string pMsg, string pDef)
        {

            DataRow[] rows_ = pPLUGIN.REF("ref.gen.string desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
            if (rows_ != null && rows_.Length > 0)
            {
                return _PLUGIN.CASTASSTRING(ISNULL(rows_[0]["VALUE"], ""));
            }
            return null;

        }


        public static double MY_ASKNUM(_PLUGIN pPLUGIN, string pMsg, double pDef, int pDecimals)
        {
            //  
            DataRow[] rows_ = pPLUGIN.REF("ref.gen.double decimals::" + FORMAT(pDecimals) + " calc::1 desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
            if (rows_ != null && rows_.Length > 0)
            {
                return _PLUGIN.CASTASDOUBLE(ISNULL(rows_[0]["VALUE"], 0.0));
            }
            return -1;
        }
        public static double MY_ASKNUM(_PLUGIN pPLUGIN, string pMsg, double pDef)
        {

            return MY_ASKNUM(pPLUGIN, pMsg, pDef, 2);
        }


        static DateTime MONTH_BEG(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }
        static DateTime MONTH_END(DateTime dt)
        {
            return MONTH_BEG(dt).AddMonths(+1).AddDays(-1);
        }

        static DateTime GET_SERVER_DATE(_PLUGIN pPLUGIN)
        {
            return CASTASDATE(pPLUGIN.SQLSCALAR("select getdate()", null));
        }

        static string GET_MAT_DESC(_PLUGIN pPLUGIN, object pMatRef)
        {

            var descLine_ = CASTASSTRING(pPLUGIN.SQLSCALAR(@"

 select CONCAT(CODE,'/',NAME) AS TITLE from LG_$FIRM$_ITEMS where LOGICALREF = @P1 

  
", new object[] { pMatRef }));

            return descLine_;

        }

        static double GET_PRICE_LIST_PRCH(_PLUGIN pPLUGIN, object pMatRef)
        {

            var price = CASTASDOUBLE(pPLUGIN.SQLSCALAR(@"

SELECT 
--$MS$--TOP(1) 
P.PRICE
FROM LG_$FIRM$_PRCLIST P WITH(NOLOCK)
WHERE P.CARDREF = @P1
AND P.PTYPE = 1
ORDER BY P.ENDDATE DESC

--$PG$--LIMIT 1
--$SL$--LIMIT 1
", new object[] { pMatRef }));

            return price;

        }

        static void CHECK_LOC_DS(_PLUGIN pPLUGIN)
        {




        }








        #endregion




        #endregion




        #region CLASS


        class MY_CALC
        {



            public static double QPROD_COST(_PLUGIN pPLUGIN, object pMatLref)
            {
                var cost = 0.0;

                //var sub = QPROD(pPLUGIN, pMatLref);
                //foreach (DataRow x in sub.Rows)
                //{
                //    cost += (CASTASDOUBLE(TAB_GETROW(x, "AMOUNT")) * CASTASDOUBLE(TAB_GETROW(x, "PRICE")));
                //}


                var matDescTop = GET_MAT_DESC(pPLUGIN, pMatLref);



                var prodSet = new MY_TOOLS_QPROD.PROD_SET();

                MY_TOOLS_QPROD.MY_PROD_UNWRAP_ITEM(pPLUGIN, pMatLref, 1.0, prodSet, 3, true);

                foreach (var prodItem in prodSet.LIST)
                {
                    if (!prodItem.HASSCRIPT)
                    {

                        var amnt = prodItem.AMOUNT;
                        var matDescSub = GET_MAT_DESC(pPLUGIN, prodItem.LOGICALREF);


                        var price = GET_PRICE_LIST_PRCH(pPLUGIN, prodItem.LOGICALREF);


                        if (ISNUMZERO(price))
                            throw new Exception("Material [" + matDescSub + "] is sub material for material [" + matDescTop + "] has no purchase price");


                        cost += (amnt * price);

                    }
                }

                //pPLUGIN

                return cost;
            }

            //            public static DataTable QPROD(_PLUGIN pPLUGIN, object pLref)
            //            {

            //                #region sql


            //                var sqlCost = @"
            // 
            //
            //  declare @maxDepth int
            // select @maxDepth = 15
            //
            //--  10  Raw Material
            //--  11  Semi Finished Good
            //--  12  Finished Good
            //--  13  Consumer Goods
            //
            // 
            //
            //declare @ITEMS_PROD TABLE(
            //LOGICALREF INT,
            //AMOUNT FLOAT)
            //declare @TMP TABLE(
            //LOGICALREF INT,
            //MAINCREF INT,
            //STCREF INT,
            //AMNT FLOAT,
            //PERC FLOAT,
            //LOSTFACTOR FLOAT)
            //
            //declare @TMP2 TABLE(
            //LOGICALREF INT,
            //MAINCREF INT,
            //STCREF INT,
            //AMNT FLOAT,
            //PERC FLOAT,
            //LOSTFACTOR FLOAT)
            //insert into @ITEMS_PROD select @P1 LOGICALREF, 1.0 AMOUNT  
            // 
            // 
            //declare @do int 
            //select @do = 1
            //
            //
            //insert into @TMP 
            //select
            //REL.LOGICALREF,
            //REL.MAINCREF,
            //REL.STCREF,
            //SOURCE.AMOUNT * REL.AMNT / ITMQAMT.QPRODAMNT,
            //REL.PERC,
            //REL.LOSTFACTOR
            //from
            //@ITEMS_PROD SOURCE
            //inner join
            //LG_$FIRM$_STCOMPLN REL
            //on SOURCE.LOGICALREF = REL.MAINCREF
            //inner join 
            //LG_$FIRM$_ITEMS ITMQAMT 
            //on ITMQAMT.LOGICALREF = REL.MAINCREF
            //  
            // 
            //while @do = 1
            //begin
            //
            //delete @TMP2
            //
            //insert into @TMP2 
            //select TMP.* from @TMP TMP
            //where 
            //EXISTS(select 1 from LG_$FIRM$_STCOMPLN REL where REL.MAINCREF = TMP.STCREF)
            //
            //if EXISTS(select 1 from @TMP2)
            //begin
            //	delete @TMP where LOGICALREF in (select LOGICALREF from @TMP2)
            //
            //	insert into @TMP 
            //	select 
            //		REL.LOGICALREF,
            //		--TMP2.MAINCREF,
            //		REL.MAINCREF,
            //		REL.STCREF,
            //		((TMP2.LOSTFACTOR+100)/100)*TMP2.AMNT*(REL.AMNT/ITMQAMT.QPRODAMNT),
            //		REL.PERC,
            //		REL.LOSTFACTOR
            //	from @TMP2 TMP2 
            //	inner join LG_$FIRM$_STCOMPLN REL on TMP2.STCREF = REL.MAINCREF
            //	inner join LG_$FIRM$_ITEMS ITMQAMT on ITMQAMT.LOGICALREF = REL.MAINCREF
            //end
            //	else
            //	select @do = 0
            //	
            //
            //
            //--------------------
            //
            //select @maxDepth = @maxDepth - 1
            //if @maxDepth < 0
            //begin
            //select @do = 0
            //declare @desc nvarchar(100)
            //select @desc = 'Incorrect including for material: ' + CODE + '/' + NAME from @TMP2 T inner join LG_$FIRM$_ITEMS I on T.MAINCREF = I.LOGICALREF
            //raiserror( @desc,16,1 )
            //
            //end
            //
            //--------------------
            //
            //end
            // 
            // 
            //
            // 
            // 
            //select
            //STCREF LOGICALREF,
            //((T.LOSTFACTOR+100)/100)*T.AMNT AMOUNT,
            //ISNULL((		
            //SELECT TOP (1) P.PRICE
            //FROM LG_$FIRM$_PRCLIST P WITH(NOLOCK)
            //WHERE P.CARDREF = ITEMS.LOGICALREF
            //AND P.PTYPE = 1
            //ORDER BY P.ENDDATE DESC),0.0) PRICE,
            //T.LOSTFACTOR
            //from @TMP T left join LG_$FIRM$_ITEMS ITEMS on T.STCREF = ITEMS.LOGICALREF
            //
            // 
            //
            //  
            //
            //";

            //                #endregion

            //                return pPLUGIN.SQL(sqlCost, new object[] { pLref });
            //            }








        }




        class MY_TOOLS_QPROD
        {

            public class PROD_SET
            {


                public List<SCRIPT_ITEM> LIST = new List<SCRIPT_ITEM>();

                public void ADD(object pLef, double pAmnt, bool pHasScript, int pLev)
                {
                    ADD(new SCRIPT_ITEM() { LOGICALREF = pLef, AMOUNT = pAmnt, HASSCRIPT = pHasScript, LEVEL = pLev });

                }
                public void ADD(SCRIPT_ITEM pItem)
                {

                    var indx = LIST.IndexOf(pItem);

                    if (indx < 0)
                        LIST.Add(pItem);
                    else
                    {
                        var z = LIST[indx];

                        z.AMOUNT += pItem.AMOUNT;

                        z.HASSCRIPT = z.HASSCRIPT || pItem.HASSCRIPT;
                    }
                }

                public void ADD(PROD_SET pOtherSet)
                {

                    foreach (var x in pOtherSet.LIST)

                        this.ADD(x);

                }

                public void REFRESH(_PLUGIN pPLUGIN)
                {

                    foreach (var x in LIST)
                    {
                        var rec = TAB_GETLASTROW(pPLUGIN.SQL(@"

 select CARDTYPE,CODE,NAME   from LG_$FIRM$_ITEMS WITH(NOLOCK) where LOGICALREF = @P1 

  
", new object[] { x.LOGICALREF }));


                        x.INFO_CARDTYPE = (rec != null) ? CASTASSHORT(TAB_GETROW(rec, "CARDTYPE")) : (short)0;
                        x.INFO_CODE = (rec != null) ? CASTASSTRING(TAB_GETROW(rec, "CODE")) : "";
                        x.INFO_NAME = (rec != null) ? CASTASSTRING(TAB_GETROW(rec, "NAME")) : "";
                        x.INFO_DESC = x.INFO_CODE + "/" + x.INFO_NAME;

                    }

                }

            }

            public class SCRIPT_ITEM : IEquatable<SCRIPT_ITEM>, IComparable<SCRIPT_ITEM>
            {

                public object LOGICALREF;
                public double AMOUNT;
                public int LEVEL;
                public bool HASSCRIPT = false;

                public string INFO_NAME = "";
                public string INFO_CODE = "";
                public string INFO_DESC = "";
                public short INFO_CARDTYPE = 0;


                public bool Equals(SCRIPT_ITEM other)
                {
                    return this.CompareTo(other) == 0;
                }

                public int CompareTo(SCRIPT_ITEM other)
                {
                    try
                    {
                        var meLref = (int)this.LOGICALREF;
                        var ooLref = (int)other.LOGICALREF;


                        var meLev = (int)this.LEVEL;
                        var ooLev = (int)other.LEVEL;


                        var cLref = meLref.CompareTo(ooLref);
                        var cLev = meLev.CompareTo(ooLev);

                        if (cLref == 0)
                            return cLev;

                        return cLref;


                    }
                    catch { }

                    return -1;
                }


                public override string ToString()
                {
                    return string.Format("{0:N2} of {1}", AMOUNT, INFO_DESC);
                }
            }

            public static void MY_PROD_UNWRAP_ITEM(
                             _PLUGIN pPLUGIN, object pMatRef, double pAmountReq,

                             PROD_SET pProdSet,
                              int pLevelMax, bool pMatSefTemp)
            {

                _MY_PROD_UNWRAP_ITEM(
                   pPLUGIN, pMatRef, pAmountReq,

                   pProdSet,
                   -1, pLevelMax, pMatSefTemp);

                pProdSet.REFRESH(pPLUGIN);

            }

            static void _MY_PROD_UNWRAP_ITEM(
               _PLUGIN pPLUGIN, object pMatRef, double pAmountReq,

               PROD_SET pProdSet,
               int pLevelNow, int pLevelMax, bool pMatSefTemp = false)
            {
                ++pLevelNow;




                if (pLevelNow > pLevelMax)
                    return;

                if (ISEMPTYLREF(pMatRef))
                    return;



                if (pAmountReq < 0.00000001)
                    return;



                var descLine_ = GET_MAT_DESC(pPLUGIN, pMatRef);


                if (pLevelNow > 5)
                    throw new Exception("Self-include not allowed in prod template [desc:" + descLine_ + "][level:" + pLevelNow + "]");



                //prodTmpRef is mat ref in case template from mat card

                var prodTmpRef = pPLUGIN.SQLSCALAR(
                   (pMatSefTemp ?
                    @"select 
--$MS$--TOP(1)  
MAINCREF from LG_$FIRM$_STCOMPLN where MAINCREF = @P1 order by LINENO_ asc
--$PG$--LIMIT 1
--$SL$--LIMIT 1
" :
                    @"select LOGICALREF from LG_$FIRM$_USERDOC4FFIRM where STOCKREF = @P1 and TRCODE = 1 order by LOGICALREF desc")
                    ,

new object[] { pMatRef });


                if (ISEMPTYLREF(prodTmpRef))
                {

                    pProdSet.ADD(pMatRef, pAmountReq, false, pLevelNow);
                    return;
                }
                else
                {
                    pProdSet.ADD(pMatRef, pAmountReq, true, pLevelNow);
                }


                var prodAmountScript = CASTASDOUBLE(pPLUGIN.SQLSCALAR(

                    (pMatSefTemp ? "select I.QPRODAMNT FLOATF1 from LG_$FIRM$_ITEMS I where LOGICALREF = @P1" :
                    "select FLOATF1 from LG_$FIRM$_USERDOC4FFIRM where LOGICALREF = @P1")

 , new object[] { prodTmpRef }));


                if (prodAmountScript < 0.00001)
                    throw new Exception("Incorrect prod template, amount is zero [docRef:" + prodTmpRef + "][desc:" + descLine_ + "]");

                var amountCoif = pAmountReq / prodAmountScript;

                var prodSunAmounts = pPLUGIN.SQL(

                    (pMatSefTemp ?
                    "select X.STCREF STOCKREF,X.AMNT FLOATF1 from LG_$FIRM$_STCOMPLN X where MAINCREF = @P1" :
                    "select STOCKREF,FLOATF1 from LG_$FIRM$_USERDOC4LFIRM where PARENTREF = @P1")
 , new object[] { prodTmpRef });



                var currMatProdSet = new PROD_SET();



                foreach (DataRow rec in prodSunAmounts.Rows)
                {
                    var lref_ = TAB_GETROW(rec, "STOCKREF");
                    var amntTmpl_ = CASTASDOUBLE(TAB_GETROW(rec, "FLOATF1"));

                    var amt_ = amountCoif * amntTmpl_;



                    {

                        var tmpProdSet = new PROD_SET();

                        _MY_PROD_UNWRAP_ITEM(pPLUGIN,
                         lref_, amt_,
                         tmpProdSet,
                         pLevelNow, pLevelMax, pMatSefTemp);

                        currMatProdSet.ADD(tmpProdSet);

                    }
                }

                pProdSet.ADD(currMatProdSet);




            }



            public static object MY_PROC_TEMPLATE(_PLUGIN pPLUGIN, object pMatRef, bool pMatSefTemp)
            {

                var prodTmpRef = pPLUGIN.SQLSCALAR(
              (pMatSefTemp ?
               @"select 
--$MS$--TOP(1) 
MAINCREF from LG_$FIRM$_STCOMPLN where MAINCREF = @P1 order by LINENO_ asc
--$PG$--LIMIT 1
--$SL$--LIMIT 1
" :
               @"select LOGICALREF from LG_$FIRM$_USERDOC4FFIRM where STOCKREF = @P1 and TRCODE = 1 order by LOGICALREF desc")
               ,

new object[] { pMatRef });

                return prodTmpRef;

            }

        }
        class MY_SAVEPRICE : IDisposable
        {

            _PLUGIN PLUGIN;




            public double priceSls;
            public double pricePrch;

            public object matRef;




            string adp_ = "adp.mm.rec.mat";


            public MY_SAVEPRICE(_PLUGIN pPLUGIN)
            {

                PLUGIN = pPLUGIN;

            }

            public void RUN()
            {

                if (!ISEMPTYLREF(matRef) && (!ISNUMZERO(priceSls) || !ISNUMZERO(pricePrch)))
                {

                    var suf_ = FORMAT(ISNULL(PLUGIN.SQLSCALAR("SELECT CARDTYPE FROM LG_$FIRM$_ITEMS I WITH(NOLOCK) WHERE LOGICALREF=@P1", new object[] { matRef }), 1));

                    PLUGIN.EXEADPCMD(new string[] { adp_ + "/" + suf_ + " cmd::edit lref::" + _PLUGIN.FORMAT(matRef) }, new DoWorkEventHandler[] { DONE }, true);//in global batch


                }
            }
            bool UPDATEPRCH()
            {
                return true;

            }


            public void DONE(object sender, System.ComponentModel.DoWorkEventArgs e)
            {

                e.Result = false;

                short pTypePrch_ = 1;
                short pTypeSls_ = 2;

                DataSet doc_ = ((DataSet)e.Argument);

                DataTable mats_ = TAB_GETTAB(doc_, "ITEMS");
                DataRow matRec_ = TAB_GETLASTROW(mats_);
                DataTable tabPrice_ = TAB_GETTAB(doc_, "PRCLIST");


                List<DataRow> lDelP = new List<DataRow>();
                List<DataRow> lDelS = new List<DataRow>();

                double priceSls_ = Math.Round(priceSls, 2);
                double pricePrch_ = Math.Round(pricePrch, 4);

                DataRow rowPrch = null;
                DataRow rowSls = null;

                foreach (DataRow row in tabPrice_.Rows)
                {
                    if (!TAB_ROWDELETED(row) && COMPARE(pTypeSls_, TAB_GETROW(row, "PTYPE")))
                        lDelS.Add(rowSls = row);


                    if (UPDATEPRCH())
                        if (!TAB_ROWDELETED(row) && COMPARE(pTypePrch_, TAB_GETROW(row, "PTYPE")))
                            lDelP.Add(rowPrch = row);
                }

                {
                    if (lDelP.Count > 0)
                        lDelP.RemoveAt(lDelP.Count - 1);
                    if (lDelS.Count > 0)
                        lDelS.RemoveAt(lDelS.Count - 1);

                    foreach (DataRow row in lDelP)
                        row.Delete();
                    foreach (DataRow row in lDelS)
                        row.Delete();
                }

                if (!ISNUMZERO(priceSls_))
                {
                    if (rowSls == null)
                    {
                        rowSls = tabPrice_.NewRow();
                        tabPrice_.Rows.Add(rowSls);
                    }
                    TAB_SETROW(rowSls, "PTYPE", pTypeSls_);
                    TAB_SETROW(rowSls, "PRICE", priceSls_);
                }

                if (UPDATEPRCH())
                {


                    if (ISNUMZERO(pricePrch_))
                    {
                        pricePrch_ = CASTASDOUBLE(ISNULL(PLUGIN.SQLSCALAR(@"

SELECT 
--$MS$--TOP(1) 
((VATMATRAH+VATAMNT+DISTEXP)/AMOUNT) PRICE 
FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK) 
WHERE 
STOCKREF = @P1 AND 
TRCODE = 1 AND 
CANCELLED=0 AND 
LINETYPE = 0 
ORDER BY STOCKREF DESC,DATE_ DESC
--$PG$--LIMIT 1
--$SL$--LIMIT 1
",

                            new object[] { matRef }), 0));
                    }

                    pricePrch_ = ROUND(pricePrch_, 4);


                    if (rowPrch == null)
                    {
                        rowPrch = tabPrice_.NewRow();
                        tabPrice_.Rows.Add(rowPrch);
                    }

                    TAB_SETROW(rowPrch, "PTYPE", pTypePrch_);
                    TAB_SETROW(rowPrch, "PRICE", pricePrch_);




                }




                e.Result = true;
            }




            public void Dispose()
            {


                PLUGIN = null;

            }

        }


        #endregion


        #endregion
