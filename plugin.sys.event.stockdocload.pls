#line 2
 

   #region BODY
        //BEGIN



        const int VERSION = 7;

        const string FILE = "plugin.sys.event.stockdocload.pls";

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



                    //

                    _SETTINGS.BUF = x;

                }



            }



            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC)
            {

            }


        }

        #endregion


        public class TEXT
        {
            public const string text_DESC = "Stock Doc Load";
        }



        const string event_STOCKDOCLOAD_DO = "hadlericom_stockdocload_do";



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

            var cmd_ = CMDLINEGETARG(taskcmd, "cmd");

            var cPanelBtnSub = CONTROL_SEARCH(FORM, "cPanelBtnSub");

            if (cPanelBtnSub == null)
                return;


            if (isStock_ && (cmd_ == "add" || cmd_ == "edit" || cmd_ == ""))
                _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_STOCKDOCLOAD_DO, LANG("T_LOAD"));
            // _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_ITS_SERIALNR, "Seri No.");
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

            _SETTINGS._BUF.LOAD_SETTINGS(this);

            //
            try
            {


                object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
                object arg2 = ARGS.Length > 1 ? ARGS[1] : null;

                string[] list_ = EXPLODELISTPATH(EVENTCODE);

                switch (list_.Length > 1 ? list_[1].ToLowerInvariant() : "")
                {


                    case event_STOCKDOCLOAD_DO:
                        {

                            if (ISADAPTERFORM(arg1 as Form))
                            {
                                var ds = RUNUIINTEGRATION(arg1, "_cmd", "dataset") as DataSet;
                                MY_STOCKDOCLOAD_DO(ds);
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




        void MY_STOCKDOCLOAD_DO(DataSet pDsDest)
        {
            var dsSource_ = MY_SELECTDOC(this);


            MY_LOAD_TO_DOC(this, dsSource_, pDsDest);

        }







        ///////////////////////////////////////////////////////////////////////////////////



        ///////////////////////////////////////////////////////////////////////////////////		


        #region TOOLS

        static void MY_LOAD_TO_DOC(_PLUGIN PLUGIN, DataSet[] pDsSources, DataSet pDsTarget)
        {

            if (pDsSources == null || pDsSources.Length == 0)
                return;


            //INVOICE
            //STFICHE
            //STLINE


            var tabInvDest_ = pDsTarget.Tables["INVOICE"];
            var tabSlipDest_ = pDsTarget.Tables["STFICHE"];
            var tabStlineDest_ = pDsTarget.Tables["STLINE"];


            foreach (var dataSetSource in pDsSources)
            {

                var tabInvSrc_ = dataSetSource.Tables["INVOICE"];
                var tabSlipSrc_ = dataSetSource.Tables["STFICHE"];
                var tabStlineSrc_ = dataSetSource.Tables["STLINE"];

                var isSInv = tabInvSrc_ != null;
                var isSSlip = !isSInv;


                var isDInv = tabInvDest_ != null;
                var isDSlip = !isDInv;


                var rowS_ = isSInv ? TAB_GETLASTROW(tabInvSrc_) : TAB_GETLASTROW(tabSlipSrc_);
                var rowD_ = isDInv ? TAB_GETLASTROW(tabInvDest_) : TAB_GETLASTROW(tabSlipDest_);

                //header
                if (rowS_ != null && rowD_ != null)
                    foreach (var c in new string[] { "GENEXP1", "CLIENTREF", "PAYDEFREF", "SOURCEINDEX", "FICHENO" })
                    {

                        var val_ = TAB_GETROW(rowS_, c);
                        if (c == "FICHENO")
                        {
                            var tr_ = CASTASSHORT(TAB_GETROW(rowS_, "TRCODE"));
                            val_ = val_.ToString() + "_L" + tr_;
                        }
                        TAB_SETROW(rowD_, c, val_);


                    }


                ////if loadint to slip then all promo make as real move
                //if (isDSlip)
                //    foreach (DataRow row in tabStlineSrc_.Rows)
                //    {
                //        //make glob and loc promo as mat
                //        var g_ = CASTASSHORT(TAB_GETROW(row, "GLOBTRANS"));
                //        var l_ = CASTASSHORT(TAB_GETROW(row, "LINETYPE"));

                //        if (l_ == 1)
                //        {
                //            TAB_SETROW(row, "GLOBTRANS", 0);
                //            TAB_SETROW(row, "LINETYPE", 0);
                //            TAB_SETROW(row, "LINEEXP", TAB_GETROW(row, "LINEEXP").ToString() + (g_ == 0 ? "L" : "G") + "PROMO");
                //        }

                //    }

                int localIndx_ = -1;
                foreach (DataRow rowS in tabStlineSrc_.Rows)
                {



                    //if (isDSlip)
                    //{

                    //    if (g_ == 1)
                    //        continue;
                    //    if (l_ != 0)
                    //        continue;

                    //}

                    //DataRow rowT_ = tabStlineDest_.NewRow();

                    //if (g_ == 1)
                    //    tabStlineDest_.Rows.Add(rowT_);
                    //else
                    //    tabStlineDest_.Rows.InsertAt(rowT_, ++localIndx_);

                    //foreach (var c in new string[] { "GLOBTRANS", "LINETYPE", "STOCKREF", "UOMREF", "AMOUNT", "PRICE", "SPECODE", "LINEEXP", })
                    //{

                    //    var val_ = TAB_GETROW(row, c);
                    //    TAB_SETROW(rowT_, c, val_);
                    //}


                    {


                        var s_GLOBTRANS = CASTASSHORT(TAB_GETROW(rowS, "GLOBTRANS"));
                        var s_LINETYPE = CASTASSHORT(TAB_GETROW(rowS, "LINETYPE"));
                        var s_STOCKREF = CASTASINT(TAB_GETROW(rowS, "STOCKREF"));
                        var s_UOMREF = CASTASINT(TAB_GETROW(rowS, "UOMREF"));
                        var s_AMOUNT = CASTASDOUBLE(TAB_GETROW(rowS, "AMOUNT"));
                        var s_PRICE = CASTASDOUBLE(TAB_GETROW(rowS, "PRICE"));
                        var s_SPECODE = CASTASSTRING(TAB_GETROW(rowS, "SPECODE"));
                        var s_LINEEXP = CASTASSTRING(TAB_GETROW(rowS, "LINEEXP"));
                        var s_VAT = CASTASDOUBLE(TAB_GETROW(rowS, "VAT"));
                        var s_VATINC = CASTASSHORT(TAB_GETROW(rowS, "VATINC"));

                        if (s_GLOBTRANS != 0 || s_LINETYPE != 0)
                            continue; //copy only real mat lines

                        DataRow targetRow = null;

                        foreach (DataRow rowD in tabStlineDest_.Rows)
                            if (!TAB_ROWDELETED(rowD))
                            {


                                var d_GLOBTRANS = CASTASSHORT(TAB_GETROW(rowD, "GLOBTRANS"));
                                var d_LINETYPE = CASTASSHORT(TAB_GETROW(rowD, "LINETYPE"));
                                if (d_GLOBTRANS != 0 || d_LINETYPE != 0)
                                    continue; //copy only real mat lines

                                var d_STOCKREF = CASTASINT(TAB_GETROW(rowD, "STOCKREF"));
                                var d_UOMREF = CASTASINT(TAB_GETROW(rowD, "UOMREF"));
                                var d_AMOUNT = CASTASDOUBLE(TAB_GETROW(rowD, "AMOUNT"));
                                var d_PRICE = CASTASDOUBLE(TAB_GETROW(rowD, "PRICE"));

                                if (
                                    s_STOCKREF == d_STOCKREF &&
                                    s_UOMREF == d_UOMREF &&
                                   ISNUMEQUAL(s_PRICE, d_PRICE)
                                    )
                                {

                                    targetRow = rowD;
                                    break;
                                }
                                else
                                {

                                    //continue
                                }

                            }


                        if (targetRow == null)
                        {
                            targetRow = tabStlineDest_.NewRow();
                            tabStlineDest_.Rows.InsertAt(targetRow, ++localIndx_);

                        }


                        TAB_SETROW(targetRow, "STOCKREF", s_STOCKREF);
                        TAB_SETROW(targetRow, "UOMREF", s_UOMREF);
                        TAB_SETROW(targetRow, "AMOUNT", s_AMOUNT + CASTASDOUBLE(TAB_GETROW(targetRow, "AMOUNT")));
                        TAB_SETROW(targetRow, "PRICE", s_PRICE);
                        TAB_SETROW(targetRow, "VAT", s_VAT);
                        TAB_SETROW(targetRow, "VATINC", s_VATINC);

                        if (!ISEMPTY(s_SPECODE))
                            TAB_SETROW(targetRow, "SPECODE", s_SPECODE);
                        if (!ISEMPTY(s_LINEEXP))
                            TAB_SETROW(targetRow, "LINEEXP", s_LINEEXP);
                    }
                }
            }

        }




        static DataSet[] MY_SELECTDOC(_PLUGIN PLUGIN)
        {
            var list = new List<string>();





            list.AddRange(new string[] { "1", PLUGIN.LANG("T_SALE") });
            list.AddRange(new string[] { "2", PLUGIN.LANG("T_PURCHASE") });
            list.AddRange(new string[] { "3", PLUGIN.LANG("T_WH") });
            //  list.AddRange(new string[] { "4", PLUGIN.LANG("T_HASPRCHPRCDIFF") });
            //  list.AddRange(new string[] { "5", PLUGIN.LANG("T_CHANGEDMATS") });
            //  list.AddRange(new string[] { "6", PLUGIN.LANG("T_DOPRODFORNEGATIVE") });



            var res_ = PLUGIN.REF("ref.gen.definedlist [obj::" + JOINLIST(list.ToArray()) + "] [desc::" +
                "T_TYPE" + "] type::string");

            string code_ = (res_ != null && res_.Length > 0 ? CASTASSTRING(res_[0]["VALUE"]) : null);
            if (code_ != null)
            {
                string ref_ = null;
                string sql_ = null;
                switch (code_)
                {
                    case "1":
                        ref_ = "ref.sls.doc.inv";
                        break;
                    case "2":
                        ref_ = "ref.prch.doc.inv";
                        break;
                    case "3":
                        ref_ = "ref.mm.doc.slip";
                        break;

                    /*
                case "4":
                    sql_ = @"
					   
DECLARE @df DATETIME, @dt DATETIME, @now DATETIME

SELECT @now = getdate(), @df = dateadd(dd, datediff(dd, 0, @now), 0), @dt = dateadd(dd, datediff(dd, 0, @now), 0)

SELECT @df = DATEADD(day, - 2, @df)

DECLARE @tabRaw TABLE (LOGICALREF INT, STOCKREF INT, DATE_ DATETIME, FTIME INT)
DECLARE @tabDocs TABLE (LOGICALREF INT, STOCKREF INT)
DECLARE @tabDocsFin TABLE (LOGICALREF INT)

INSERT @tabRaw
SELECT LOGICALREF, STOCKREF, DATE_, FTIME
FROM LG_$FIRM$_$PERIOD$_STLINE L WITH (INDEX = I$FIRM$_$PERIOD$_STLINE_I19)
WHERE TRCODE = 1 AND CANCELLED = 0 AND LINETYPE = 0 AND DATE_ BETWEEN @df AND @dt AND FTIME >= 0

INSERT INTO @tabDocs
SELECT (
    SELECT TOP (1) LOGICALREF
    FROM @tabRaw
    WHERE STOCKREF = D.STOCKREF
    ORDER BY DATE_ DESC, FTIME DESC
    ) LOGICALREF, STOCKREF
FROM (
SELECT DISTINCT STOCKREF
FROM @tabRaw
) D

INSERT INTO @tabDocsFin
SELECT LOGICALREF
FROM (
SELECT *, (
        SELECT ((VATMATRAH + VATAMNT + DISTEXP) / AMOUNT)
        FROM LG_$FIRM$_$PERIOD$_STLINE L WITH(NOLOCK)
        WHERE LOGICALREF = F.LOGICALREF
        ) PL, 
        ISNULL((
            SELECT TOP (1) PRICE
            FROM LG_$FIRM$_PRCLIST WITH(NOLOCK)
            WHERE CARDREF = F.STOCKREF AND PTYPE = 1
            ORDER BY ENDDATE DESC
            ), 0) PP
FROM @tabDocs F
) D
WHERE ABS(D.PP - D.PL) > 0.001

SELECT L.*
FROM @tabDocsFin F
INNER JOIN LG_$FIRM$_$PERIOD$_STLINE L WITH(NOLOCK) ON F.LOGICALREF = L.LOGICALREF
ORDER BY DATE_ ASC, FTIME ASC, INVOICEREF ASC, INVOICELNNO ASC
 
					   
                   ";
                    break;

                case "5":

                    sql_ = @"
                        DECLARE @df DATETIME, @dt DATETIME, @now DATETIME

                        SELECT @now = getdate(),
                         @df = dateadd(dd, datediff(dd, 0, @now), 0), 
                         @dt = DATEADD(s, 86400-1, @df) 


                        declare @tab_tmp table(LOGICALREF INT,DATE_ DATETIME)
					 
 
                        insert into @tab_tmp select LOGICALREF,(case when CAPIBLOCK_EXTCREATEDDATE > CAPIBLOCK_EXTMODIFIEDDATE then CAPIBLOCK_EXTCREATEDDATE else CAPIBLOCK_EXTMODIFIEDDATE end) DATE_ from LG_$FIRM$_ITEMS where CAPIBLOCK_EXTCREATEDDATE between @df and @dt or CAPIBLOCK_EXTMODIFIEDDATE between @df and @dt 

                        insert into @tab_tmp select CARDREF,(case when CAPIBLOCK_EXTCREATEDDATE > CAPIBLOCK_EXTMODIFIEDDATE then CAPIBLOCK_EXTCREATEDDATE else CAPIBLOCK_EXTMODIFIEDDATE end) from LG_$FIRM$_PRCLIST where CAPIBLOCK_EXTCREATEDDATE between @df and @dt or CAPIBLOCK_EXTMODIFIEDDATE between @df and @dt 

                        select 0 LOGICALREF,0 LINETYPE,0 GLOBTRANS, LOGICALREF STOCKREF,0.0 AMOUNT,0.0 PRICE,DATE_,CONVERT(nvarchar,DATE_,120) LINEEXP from (
                        select LOGICALREF,MAX(DATE_) DATE_ from @tab_tmp group by LOGICALREF) D ORDER BY D.DATE_ ASC
                    ";


                    break;


                case "6":

                    sql_ = @"
 
                        DECLARE @dt DATETIME

                        SELECT 
                         @dt = dateadd(dd, datediff(dd, 0, getdate()), 0)

				 
                        declare @tab_tmp  table(LOGICALREF INT,ONHAND FLOAT,LINEEXP  NVARCHAR (200))

                        insert into @tab_tmp select D.LOGICALREF,ABS(G.ONHAND),(I.SPECODE+'/'+I.SPECODE2) from (select distinct MAINCREF LOGICALREF from LG_$FIRM$_STCOMPLN WITH(NOLOCK)) D 
                        inner join LG_$FIRM$_$PERIOD$_GNTOTST G WITH(NOLOCK) on D.LOGICALREF = G.STOCKREF and G.INVENNO=0 and G.ONHAND < -0.01 
                        inner join LG_$FIRM$_ITEMS I WITH(NOLOCK) on D.LOGICALREF = I.LOGICALREF
                        select 0 LOGICALREF,0 LINETYPE,0 GLOBTRANS, LOGICALREF STOCKREF,ONHAND AMOUNT,0.0 PRICE,DATE_,
                        LINEEXP LINEEXP 
                        from (select LOGICALREF,@dt DATE_,ONHAND,LINEEXP from @tab_tmp ) D ORDER BY D.DATE_ ASC
                    ";


                    break;


                    */
                }

                if (ref_ != null)
                {
                    res_ = PLUGIN.REF(ref_ + "  multi::1 ");

                    if (res_ != null && res_.Length > 0)
                    {

                        var listDs = new List<DataSet>();

                        foreach (var rec_ in res_)
                        {


                            var docLRef_ = TAB_GETROW(rec_, "LOGICALREF");


                            var isInv_ = (rec_.Table.TableName == "INVOICE");
                            var ds_ = new DataSet();

                            if (isInv_)
                            {
                                var h = PLUGIN.SQL(@"
SELECT 
--$MS$--TOP(1) 
* FROM LG_$FIRM$_$PERIOD$_INVOICE 
--$MS$--WITH(NOLOCK) 
WHERE LOGICALREF = @P1
--$PG$--LIMIT 1
", new object[] { docLRef_ });
                                h.TableName = "INVOICE";
                                ds_.Tables.Add(h);


                                var l = PLUGIN.SQL(@"
SELECT 
--$MS$--TOP(5000) 
* 
FROM LG_$FIRM$_$PERIOD$_STLINE 
--$MS$--WITH(NOLOCK)  
WHERE INVOICEREF = @P1 
ORDER BY INVOICELNNO
--$PG$--LIMIT 5000
", new object[] { docLRef_ });
                                l.TableName = "STLINE";
                                ds_.Tables.Add(l);

                                listDs.Add(ds_);
                            }
                            else
                            {
                                var h = PLUGIN.SQL(@"
SELECT 
--$MS$--TOP(1)
* FROM LG_$FIRM$_$PERIOD$_STFICHE 
--$MS$--WITH(NOLOCK) 
WHERE LOGICALREF = @P1
--$PG$--LIMIT 1
", new object[] { docLRef_ });
                                h.TableName = "STFICHE";
                                ds_.Tables.Add(h);

                                var l = PLUGIN.SQL(@"
SELECT 
--$MS$--TOP(5000) 
* FROM LG_$FIRM$_$PERIOD$_STLINE 
--$MS$--WITH(NOLOCK)  
WHERE STFICHEREF = @P1 AND (TRCODE != 25 OR (TRCODE = 25 AND IOCODE = 3))
ORDER BY STFICHELNNO
--$PG$--LIMIT 5000
", new object[] { docLRef_ });
                                l.TableName = "STLINE";
                                ds_.Tables.Add(l);


                                listDs.Add(ds_);
                            }


                        }





                        return listDs.ToArray();

                    }

                }
                else
                    if (sql_ != null)
                    {
                        DataSet ds_ = new DataSet();
                        var l = PLUGIN.SQL(sql_, new object[] { });
                        l.TableName = "STLINE";
                        ds_.Tables.Add(l);
                        return new DataSet[] { ds_ };

                    }
            }

            return null;





        }

        #endregion



        #endregion
