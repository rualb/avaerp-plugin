#line 2


      #region BODY
        //BEGIN

        const int VERSION = 35;

 
        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {

            if (ISWEB())
                return;





            object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
            object arg2 = ARGS.Length > 1 ? ARGS[1] : null;
            object arg3 = ARGS.Length > 2 ? ARGS[2] : null;

            string[] list_ = EXPLODELISTPATH(EVENTCODE);

            var cmdType = list_.Length > 0 ? list_[0] : "";
            var cmdExt = list_.Length > 1 ? list_[1] : "";



            switch (cmdType)
            {

                case SysEvent.SYS_USEREVENT:
                    if (cmdExt.StartsWith("_barcodeterm_"))
                    {
                        var dic = new Dictionary<string, string>();
                        dic["type"] = FORMAT((int)4);
                        dic["clcard"] = (GETARG("clcard"));
                        dic["trcode"] = (GETARG("trcode"));
                        dic["cancelled"] = (GETARG("cancelled"));
                        dic["mode"] = (GETARG("mode"));

                        switch (cmdExt)
                        {
                            case "_barcodeterm_":
                                {
                                    var prm = arg1 as Dictionary<string, string>;

                                    foreach (var key in prm.Keys)
                                        dic[key] = prm[key];

                                }
                                break;

                            default:

                                return;

                        }

                        SYS_BEGIN_PRM(dic);

                    }
                    break;



            }



        }

        public void SYS_BEGIN_PRM(Dictionary<string, string> pPrm)
        {


            //////////////////////////////////////////////////////////////////////////////////////////////////////////////
            TERMTYPE TERMTYPE_ = (TERMTYPE)PARSESHORT(_GETDIC(pPrm, "type"));
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (TERMTYPE_ == TERMTYPE.none)
                throw new Exception(_LANG.L.MSG_ERROR_TERMINCORECT);


            if (!BARCODETERM_BASE.PosTerminal.canStartTermType(TERMTYPE_))
            {
                MSGUSERERROR(_LANG.L.MSG_ERROR_WINBLOCKED);
                return;
            }

            var p = new BARCODETERM_BASE.PosTerminal(this);

            BARCODETERM_BASE.PRM = new PARAMETERS(TERMTYPE_, this, pPrm);
            // BARCODETERM_BASE.PRM.LOAD(pPrm);


            p._CLSUFIX = CASTASSTRING(_GETDIC(pPrm, "clcard"));
            p._DESIGN = CASTASSTRING(_GETDIC(pPrm, "design"));
            p._TRCODE = CASTASSHORT(_GETDIC(pPrm, "trcode"));
            p._CANCELLED = CASTASSHORT(_GETDIC(pPrm, "cancelled")) == 1;
            p._WH = CASTASSHORT(_GETDIC(pPrm, "wh", "-2"));

            p.parentFormIndex = CASTASINT(_GETDIC(pPrm, "_parentFormIndex", "-2"));

            p._SPECODE = CASTASSTRING(_GETDIC(pPrm, "SPECODE"));
            p._DOCODE = CASTASSTRING(_GETDIC(pPrm, "DOCODE"));
            p._GENEXP1 = CASTASSTRING(_GETDIC(pPrm, "GENEXP1"));


            if (p._TRCODE == 0)
                p._TRCODE = 8;

            if (BARCODETERM_BASE.PRM.TERM_TYPE == TERMTYPE.production)
                p._TRCODE = 13;

            if (BARCODETERM_BASE.PRM.TERM_TYPE == TERMTYPE.count)
                p._TRCODE = 50;

            if (BARCODETERM_BASE.PRM.TERM_TYPE == TERMTYPE.barcode)
                p._TRCODE = 101;


            if (BARCODETERM_BASE.PRM.TERM_TYPE == TERMTYPE.pricing)
                p._TRCODE = 102;

            string mode_ = CASTASSTRING(_GETDIC(pPrm, "mode"));
            switch (mode_)
            {
                case "":
                    p.BEGIN_TERMINAL(null);
                    break;
                case "slsman":
                    p.BEGIN_SLSMAN();
                    break;
                case "slsinv":
                    p.BEGIN_SLSINV();
                    break;

                case "print":
                    p.PRINTLAST(false);
                    break;

                case "prchpricetolist":
                    p.PRICEPRCHFROMDOC();
                    p.JOB_DONE();
                    break;
                //case "calccostbyprchprice":
                //    p.CALCCOST();
                //    p.JOB_DONE();
                //    break;
                //case "runqprodsimple":
                //    p.QPRODSIMPLE();
                //    p.JOB_DONE();
                //    break;

                default:
                    throw new Exception("Undefined mode [" + ISNULL(mode_, "") + "]");
            }

        }

        public DataSet MY_STOCKCONTENTFORFILL()
        {

            return _MY_STOCKCONTENTFORFILL(this);


        }
        static DataSet _MY_STOCKCONTENTFORFILL(_PLUGIN PLUGIN)
        {
            var list = new List<string>();





            list.AddRange(new string[] { "1", _LANG.L.SALE });
            list.AddRange(new string[] { "2", _LANG.L.PURCHASE });
            list.AddRange(new string[] { "3", _LANG.L.WHDOCS });

            if (BARCODETERM_BASE.PRM.TERM_TYPE != TERMTYPE.production)
            {
                list.AddRange(new string[] { "4", _LANG.L.HASPRCHPRCDIFF });
                list.AddRange(new string[] { "5", _LANG.L.CHANGEDMATS });
            }

            if (BARCODETERM_BASE.PRM.TERM_TYPE == TERMTYPE.production)
                list.AddRange(new string[] { "6", _LANG.L.DOPRODFORNEGATIVE });





            var res_ = PLUGIN.REF("ref.gen.definedlist [obj::" + JOINLIST(list.ToArray()) + "] [desc::" + _LANG.L.DOCTYPE + "] type::string");

            string code_ = (res_ != null && res_.Length > 0 ? CASTASSTRING(res_[0]["VALUE"]) : null);
            if (code_ != null)
            {
                string ref_ = null;
                string sql_ = null;
                var args_ = new List<object>();
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
                    case "4":
                        //TODO

                        args_.Add(DateTime.Now.Date.AddDays(-2));
                        args_.Add(DateTime.Now.Date);


                        sql_ =
                            MY_CHOOSE_SQL(
@"
					   
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
 
					   
					   ",
  @"
 
SELECT LOGICALREF, STOCKREF, DATE_, FTIME INTO TEMP _tabRaw
FROM LG_$FIRM$_$PERIOD$_STLINE L 
WHERE TRCODE = 1 AND CANCELLED = 0 AND LINETYPE = 0 AND DATE_ BETWEEN @P1 AND @P2 AND FTIME >= 0
;
 
SELECT (
		SELECT LOGICALREF
		FROM _tabRaw
		WHERE STOCKREF = D.STOCKREF
		ORDER BY DATE_ DESC, FTIME DESC LIMIT 1
		) LOGICALREF, STOCKREF
INTO TEMP _tabDocs
FROM (
	SELECT DISTINCT STOCKREF
	FROM _tabRaw
	) D
;
 
SELECT LOGICALREF
INTO TEMP _tabDocsFin
FROM (
	SELECT *, (
			SELECT ((VATMATRAH + VATAMNT + DISTEXP) / AMOUNT)
			FROM LG_$FIRM$_$PERIOD$_STLINE L 
			WHERE LOGICALREF = F.LOGICALREF
			) PL, 
			COALESCE((
				SELECT PRICE
				FROM LG_$FIRM$_PRCLIST 
				WHERE CARDREF = F.STOCKREF AND PTYPE = 1
				ORDER BY ENDDATE DESC LIMIT 1
				), 0) PP
	FROM _tabDocs F
	) D
WHERE ABS(D.PP - D.PL) > 0.001
;
SELECT L.*
FROM _tabDocsFin F
INNER JOIN LG_$FIRM$_$PERIOD$_STLINE L ON F.LOGICALREF = L.LOGICALREF
ORDER BY DATE_ ASC, FTIME ASC, INVOICEREF ASC, INVOICELNNO ASC
 ;
					   
					   ");
                        break;

                    case "5":

                        //TODO


                        args_.Add(DateTime.Now.Date.AddDays(0).AddSeconds(-1));
                        args_.Add(DateTime.Now.Date.AddDays(+1).AddSeconds(-1));



                        sql_ =

MY_CHOOSE_SQL(
@"
							DECLARE @df DATETIME, @dt DATETIME, @now DATETIME

							SELECT @now = getdate(),
							 @df = dateadd(dd, datediff(dd, 0, @now), 0), 
							 @dt = DATEADD(s, 86400-1, @df) 


							declare @tab_tmp table(LOGICALREF INT,DATE_ DATETIME)
					 
 
							insert into @tab_tmp select LOGICALREF,(case when CAPIBLOCK_EXTCREATEDDATE > CAPIBLOCK_EXTMODIFIEDDATE then CAPIBLOCK_EXTCREATEDDATE else CAPIBLOCK_EXTMODIFIEDDATE end) DATE_ from LG_$FIRM$_ITEMS where CAPIBLOCK_EXTCREATEDDATE between @df and @dt or CAPIBLOCK_EXTMODIFIEDDATE between @df and @dt 

							insert into @tab_tmp select CARDREF,(case when CAPIBLOCK_EXTCREATEDDATE > CAPIBLOCK_EXTMODIFIEDDATE then CAPIBLOCK_EXTCREATEDDATE else CAPIBLOCK_EXTMODIFIEDDATE end) from LG_$FIRM$_PRCLIST where CAPIBLOCK_EXTCREATEDDATE between @df and @dt or CAPIBLOCK_EXTMODIFIEDDATE between @df and @dt 

							select 0 LOGICALREF,0 LINETYPE,0 GLOBTRANS, LOGICALREF STOCKREF,0.0 AMOUNT,0.0 PRICE,DATE_,CONVERT(nvarchar,DATE_,120) LINEEXP from (
						    select LOGICALREF,MAX(DATE_) DATE_ from @tab_tmp group by LOGICALREF) D ORDER BY D.DATE_ ASC
						", @"
 
 select cast(0 as INT) LOGICALREF ,cast(NOW() as TIMESTAMP(0)) DATE_ into TEMP _tab_tmp
LIMIT 0
;
insert into _tab_tmp						 
select LOGICALREF,
(case when CAPIBLOCK_EXTCREATEDDATE > CAPIBLOCK_EXTMODIFIEDDATE then 
CAPIBLOCK_EXTCREATEDDATE else CAPIBLOCK_EXTMODIFIEDDATE end) DATE_ 
 
from LG_$FIRM$_ITEMS where 
CAPIBLOCK_EXTCREATEDDATE between @P1 and @P2 or CAPIBLOCK_EXTMODIFIEDDATE between @P1 and @P2
;
insert into _tab_tmp	
select CARDREF,
(case when CAPIBLOCK_EXTCREATEDDATE > CAPIBLOCK_EXTMODIFIEDDATE then 
CAPIBLOCK_EXTCREATEDDATE else CAPIBLOCK_EXTMODIFIEDDATE end) 
 
from LG_$FIRM$_PRCLIST where 
CAPIBLOCK_EXTCREATEDDATE between @P1 and @P2 or CAPIBLOCK_EXTMODIFIEDDATE between @P1 and @P2 
;
select 
cast(0 as INT) LOGICALREF,cast(0 as SMALLINT) LINETYPE,cast(0 as SMALLINT) GLOBTRANS, LOGICALREF STOCKREF,
cast(0.0 as FLOAT) AMOUNT,cast(0.0 as FLOAT) PRICE,DATE_,substr(cast(DATE_ as VARCHAR),1,10) LINEEXP 
from (select LOGICALREF,MAX(DATE_) DATE_ from _tab_tmp group by LOGICALREF) D ORDER BY D.DATE_ ASC
;
						");


                        break;


                    case "6":

                        //TODO

                        args_.Add(DateTime.Now.Date);


                        sql_ =
MY_CHOOSE_SQL(

@"
 
                            DECLARE @dt DATETIME

							SELECT 
							 @dt = dateadd(dd, datediff(dd, 0, getdate()), 0)

				 
							declare @tab_tmp  table(LOGICALREF INT,ONHAND FLOAT,LINEEXP  NVARCHAR (200))

                            insert into @tab_tmp select D.LOGICALREF,ABS(G.ONHAND),(I.SPECODE+'/'+I.SPECODE2) 

                            from (select distinct MAINCREF LOGICALREF from LG_$FIRM$_STCOMPLN WITH(NOLOCK)) D 

                            inner join LG_$FIRM$_$PERIOD$_GNTOTST G WITH(NOLOCK) 
                                        on D.LOGICALREF = G.STOCKREF and G.INVENNO=-1 and G.ONHAND < -0.01 

                            inner join LG_$FIRM$_ITEMS I WITH(NOLOCK) on D.LOGICALREF = I.LOGICALREF
							select 0 LOGICALREF,0 LINETYPE,0 GLOBTRANS, LOGICALREF STOCKREF,ONHAND AMOUNT,0.0 PRICE,DATE_,
                            LINEEXP LINEEXP 
                            from (select LOGICALREF,@dt DATE_,ONHAND,LINEEXP from @tab_tmp ) D ORDER BY D.DATE_ ASC


						",

@"
 
select  cast(0 as INT) LOGICALREF,cast(0 as FLOAT) ONHAND ,cast('' as VARCHAR) LINEEXP into temp _tab_tmp
;
 
			 
                            insert into _tab_tmp 
                            
                            select D.LOGICALREF,ABS(G.ONHAND),(I.SPECODE||'/'||I.SPECODE2) 

                            from (select distinct MAINCREF LOGICALREF from LG_$FIRM$_STCOMPLN ) D 

                            inner join LG_$FIRM$_$PERIOD$_GNTOTST G 
                                        on D.LOGICALREF = G.STOCKREF and G.INVENNO=-1 and G.ONHAND < -0.01 

                            inner join LG_$FIRM$_ITEMS I on D.LOGICALREF = I.LOGICALREF
;
							select 
cast(0 as INT) LOGICALREF,cast(0 as SMALLINT) LINETYPE,cast(0 as SMALLINT) GLOBTRANS, 
LOGICALREF STOCKREF,ONHAND AMOUNT,
cast(0.0 as FLOAT) PRICE,DATE_,
                            LINEEXP LINEEXP 
                            from (select LOGICALREF,cast(@P1 as TIMESTAMP(0)) DATE_,ONHAND,LINEEXP from _tab_tmp ) D ORDER BY D.DATE_ ASC


						");


                        break;
                }

                if (ref_ != null)
                {
                    res_ = PLUGIN.REF(ref_);

                    if (res_ != null && res_.Length > 0)
                    {
                        var rec_ = res_[0];

                        var docLRef_ = TAB_GETROW(rec_, "LOGICALREF");


                        var isInv_ = (rec_.Table.TableName == "INVOICE");
                        DataSet ds_ = new DataSet();

                        if (isInv_)
                        {
                            var h = PLUGIN.SQL(

                               MY_CHOOSE_SQL(
                               "SELECT TOP(1) * FROM LG_$FIRM$_$PERIOD$_INVOICE WITH(NOLOCK) WHERE LOGICALREF = @P1",
                               "SELECT * FROM LG_$FIRM$_$PERIOD$_INVOICE WHERE LOGICALREF = @P1 LIMIT 1"
                               )

                                , new object[] { docLRef_ });
                            h.TableName = "INVOICE";
                            ds_.Tables.Add(h);


                            var l = PLUGIN.SQL(
                                MY_CHOOSE_SQL(
                                "SELECT TOP(5000) * FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK)  WHERE INVOICEREF = @P1",
                                "SELECT * FROM LG_$FIRM$_$PERIOD$_STLINE WHERE INVOICEREF = @P1 LIMIT 5000")
                                , new object[] { docLRef_ });
                            l.TableName = "STLINE";
                            ds_.Tables.Add(l);

                            return ds_;
                        }
                        else
                        {
                            var h = PLUGIN.SQL(
                                MY_CHOOSE_SQL(
                                "SELECT TOP(1) * FROM LG_$FIRM$_$PERIOD$_STFICHE WITH(NOLOCK) WHERE LOGICALREF = @P1",
                                "SELECT * FROM LG_$FIRM$_$PERIOD$_STFICHE WHERE LOGICALREF = @P1 LIMIT 1"), new object[] { docLRef_ });
                            h.TableName = "STFICHE";
                            ds_.Tables.Add(h);

                            var l = PLUGIN.SQL(
                                MY_CHOOSE_SQL(
                                "SELECT TOP(5000) * FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK)  WHERE STFICHEREF = @P1 AND (TRCODE != 25 OR (TRCODE = 25 AND IOCODE = 3))",
                                "SELECT * FROM LG_$FIRM$_$PERIOD$_STLINE WHERE STFICHEREF = @P1 AND (TRCODE != 25 OR (TRCODE = 25 AND IOCODE = 3)) LIMIT 5000"
                                ), new object[] { docLRef_ });
                            l.TableName = "STLINE";
                            ds_.Tables.Add(l);


                            return ds_;
                        }




                    }

                }
                else
                    if (sql_ != null)
                    {
                        DataSet ds_ = new DataSet();
                        var l = PLUGIN.SQL(sql_, args_.ToArray());
                        l.TableName = "STLINE";
                        ds_.Tables.Add(l);
                        return ds_;

                    }
            }

            return null;





        }

        static string _GETDIC(Dictionary<string, string> pPrm, string pKey)
        {
            return _GETDIC(pPrm, pKey, null);
        }
        static string _GETDIC(Dictionary<string, string> pPrm, string pKey, string pDef)
        {
            string r_ = pDef;

            pPrm.TryGetValue(pKey, out r_);

            return r_ == null ? pDef : r_;
        }

        public enum TERMTYPE
        {
            none = 0,
            magazin = 1,
            magazinWholesale = 2,
            credit = 3,
            restoran = 4,
            aptek = 5,
            magazinWholesaleSlsman = 6,
            hotel = 7,
            count = 8,
            barcode = 9,//label print for mat and shelf
            pricing = 10,
            production = 11
        }

        public class PRICNGVARS
        {
            public const string PL = "@PL"; //last purch 
            public const string PP = "@PP"; //purch price list 
            public const string PG = "@PG"; //purch gross price 
            public const string PC = "@PC"; //purch cost 
            public const string PA = "@PA";//purch avg   
            public const string PD = "@PD"; //purch discount 
            public const string SP = "@SP";//sale pricelist 
            public const string FF = "@FF"; //float f1 


            public static readonly string[] LIST = new string[] {

           PL, //1 last purch 
           PP, //2 purch price list 
           PG, //3 purch gross price 
           PC , //4 purch cost 
           PA ,//5 purch avg   
           PD , //6 purch discount 
           SP,//7 sale pricelist 
           FF   //8 float f1 
            
            };

            public const string _PRICINGINITED = "__PRICING__INITED"; //float f1 
            public static bool isLoaded(DataRow pRow)
            {
                return pRow[_PRICINGINITED].ToString() == "1";
            }
            public static void setLoaded(DataRow pRow)
            {
                pRow[_PRICINGINITED] = "1";
            }


            public static void setFormulaInited(DataTable pTab)
            {
                pTab.ExtendedProperties[_PRICINGINITED] = "1";
            }

            public static bool isFormulaInited(DataTable pTab)
            {
                return (pTab.ExtendedProperties[_PRICINGINITED] as string) == "1"; ;
            }
        }
        public class PARAMETERS
        {

            public static string saleInvoicePrintReportDir = null;
            public static string barcodePrintReportDir = "config/report/mm.label";
            public PARAMETERS()
            {
                //= "mm.000029"
                // "rep001030010"

                if (saleInvoicePrintReportDir == null)
                {
                    foreach (var dir in new string[] { "config/report/mm.000029", "config/report/rep001030010" })
                    {
                        var z = PATHCOMBINE(GETHOMEDIR(), dir);
                        if (Directory.Exists(z))
                        {
                            saleInvoicePrintReportDir = dir;
                            break;
                        }
                    }
                }
            }

            Dictionary<string, string> prm = new Dictionary<string, string>();

            public Dictionary<string, string> COPY_PRM()
            {
                var t = new Dictionary<string, string>();
                foreach (string key in prm.Keys)
                {
                    var val = prm[key];
                    t[key] = val;
                }

                return t;

            }

            void LOAD(Dictionary<string, string> pPrm)
            {

                double UI_SCALE = 1;
                double UI_SCALE_H = 1;

                foreach (string key in pPrm.Keys)
                {


                    var val = pPrm[key];


                    prm[key] = val;


                    switch (key)
                    {
                        case "BARCODE_LEN10_CLIENT":
                            this.BARCODE_LEN10_CLIENT = (val == "1");
                            break;
                        case "ASK_PRINT":
                            this.ASK_PRINT = (val == "1");
                            break;
                        case "USE_ONHAND":
                            this.USE_ONHAND = (val == "1");
                            break;
                        case "USE_ONHAND_TOT":
                            this.USE_ONHAND_TOT = (val == "1");
                            break;
                        case "USE_ONHAND_ALL":
                            this.USE_ONHAND_ALL = (val == "1");
                            break;
                        case "USE_ONHAND_MAIN":
                            this.USE_ONHAND_MAIN = (val == "1");
                            break;
                        case "NEGATIVE_LIGHT":
                            this.NEGATIVE_LIGHT = (val == "1");
                            break;
                        case "SAVE_TO_FILE":
                            this.SAVE_TO_FILE = (val == "1");
                            break;
                        case "PRICE_TYPE":
                            this.PRICE_TYPE = int.Parse(val);
                            break;
                        case "UI_SCALE":
                            this.UI_SCALE = UI_SCALE = 1 + Math.Min(Math.Max(0, double.Parse(val)), 100) / 100.0;
                            break;
                        case "UI_SCALE_H":
                            this.UI_SCALE_H = UI_SCALE_H = 1 + Math.Min(Math.Max(0, double.Parse(val)), 100) / 100.0;
                            break;
                        case "INFO_SCALE":
                            this.INFO_SCALE = 1 + Math.Min(Math.Max(0, double.Parse(val)), 100) / 100.0;
                            break;
                        case "PRICE_DIFF_BY_PRCH":
                            this.PRICE_DIFF_BY_PRCH = (val == "1");
                            break;

                        case "SEARCH_MAT_BY_CODE":
                            this.SEARCH_MAT_BY_CODE = (val == "1");
                            break;
                        case "SEARCH_CL_BY_CODE":
                            this.SEARCH_CL_BY_CODE = (val == "1");
                            break;

                        case "USE_BONUS_MAQ":
                            this.USE_BONUS_MAQ = (val == "1");
                            break;

                        case "REMOTE_SQL_CONN":
                            this.REMOTE_SQL_CONN = (val);
                            break;
                        case "CASH_LIST":
                            this.CASH_LIST = (val);
                            break;

                        case "PRINT_ON_SAVE":
                            this.PRINT_ON_SAVE = (val == "1");
                            break;

                        case "USE_VAT":
                            this.USE_VAT = (val == "1");
                            break;
                        case "EDIT_VAT":
                            this.EDIT_VAT = (val == "1");
                            break;
                        case "DEF_VAT":
                            this.DEF_VAT = PARSEDOUBLE(val);
                            break;


                        case "UPDATE_PRCH_PRICE_IN_CARD":
                            this.UPDATE_PRCH_PRICE_IN_CARD = (val == "1");
                            break;
                        case "USE_PASSWORD":
                            this.USE_PASSWORD = (val == "1");
                            break;
                        case "USE_PASSWORD_FOR_AMOUNT":
                            this.USE_PASSWORD_FOR_AMOUNT = (val == "1");
                            break;
                        case "FULL_SCREEN":
                            this.FULL_SCREEN = (val == "1");
                            break;
                        case "MAKE_PAYMENT":
                            this.MAKE_PAYMENT = (val == "1");
                            break;
                        case "ASK_PAYMENT":
                            this.ASK_PAYMENT = (val == "1");
                            break;
                        case "PRICE_FORMULA":
                            this.PRICE_FORMULA = (val);
                            break;
                        case "MAT_FROM_LIST":
                            this.MAT_FROM_LIST = (val == "1");
                            break;
                        case "FULL_CLEAN":
                            this.FULL_CLEAN = (val == "1");
                            break;
                        case "STOP_CLOSE_FULL_DOC":
                            this.STOP_CLOSE_FULL_DOC = (val == "1");
                            break;
                        case "DOC_BACKUP":
                            this.DOC_BACKUP = (val == "1");
                            break;
                        case "LOG_AMOUNT_DEC":
                            this.LOG_AMOUNT_DEC = (val == "1");
                            break;
                        case "BARCODE_LEN_CLIENT":
                            this.BARCODE_LEN_CLIENT = int.Parse(val);
                            break;
                        case "PARENT_OFFLINE":
                            this.PARENT_OFFLINE = (val == "1");
                            break;
                        //case "RETURN_BY_PARENT_DOC":
                        //    this.RETURN_BY_PARENT_DOC = (val == "1");
                        //    break;
                        case "RETURN_EDIT_PRICE":
                            this.RETURN_EDIT_PRICE = (val == "1");
                            break;
                        case "INFO_TO_LEFT":
                            this.INFO_TO_LEFT = (val == "1");
                            break;
                        case "HIDE_DISC":
                            this.HIDE_DISC = (val == "1");
                            break;

                        case "CHANGE_PAYPLAN":
                            this.CHANGE_PAYPLAN = (val == "1");
                            break;



                        case "DOCUMENTS":
                            this.DOCUMENTS = (val == "1");
                            break;

                        case "APPLY_CAMPAGIN":
                            this.APPLY_CAMPAGIN = (val == "1");
                            break;
                        case "APPLY_CAMPAGIN_AUTO":
                            this.APPLY_CAMPAGIN_AUTO = (val == "1");
                            break;

                        case "CAMPAGIN_CLEAN_ON_PAYPLAN":
                            this.CAMPAGIN_CLEAN_ON_PAYPLAN = (val == "1");
                            break;

                        //case "LAST_DOC_BY_DATE_":
                        //    this.LAST_DOC_BY_DATE_ = (val == "1");
                        //    break;

                        case "JOB_DONE_INFORM":
                            this.JOB_DONE_INFORM = (val == "1");
                            break;

                        case "USE_MANUAL_DISCOUNT":
                            this.USE_MANUAL_DISCOUNT = (val == "1");
                            break;

                        case "USE_MANUAL_DISCOUNT_TOT":
                            this.USE_MANUAL_DISCOUNT_TOT = (val == "1");
                            break;
                        case "USE_MANUAL_DISCOUNT_PERC":
                            this.USE_MANUAL_DISCOUNT_PERC = (val == "1");
                            break;


                        case "USE_LINE_AMNT_ON_FILL":
                            this.USE_LINE_AMNT_ON_FILL = (val == "1");
                            break;

                        case "POS_USER_PREFIX":
                            this.POS_USER_PREFIX = (val );
                            break;

                        case "POS_TERM_PREFIX":
                            this.POS_TERM_PREFIX = (val);
                            break;



                    }


                }

                GRID_ROW_H = (int)(GRID_ROW_H * UI_SCALE * UI_SCALE_H);
                CMD_BTN_H = (int)(CMD_BTN_H * UI_SCALE);
                FONT_SIZE = (int)(FONT_SIZE * UI_SCALE * UI_SCALE_H);

            }

            public PARAMETERS(TERMTYPE TYPE, _PLUGIN pPlugin, Dictionary<string, string> pPrm)
                : this()
            {
                INIT_DEF(TYPE, pPlugin);

                LOAD(pPrm);


                INIT_BTNS(pPlugin);
            }




            void INIT_DEF(TERMTYPE TYPE, _PLUGIN pPlugin)
            {

                TERM_TYPE = TYPE;
                //
                GRID_ROW_H = 24;
                CMD_BTN_H = 60;
                FONT_SIZE = 9;



                {

                    var len_ = pPlugin.GETCOLUMNSIZE("KSLINES", "FICHENO", 9);
                    /*
                    var len_ = CASTASINT(ISNULL(pPlugin.SQLSCALAR(
                        MY_CHOOSE_SQL(
@"
 
select CHARACTER_MAXIMUM_LENGTH from [INFORMATION_SCHEMA].[COLUMNS] where 
TABLE_NAME = 'LG_$FIRM$_$PERIOD$_KSLINES' and COLUMN_NAME = 'FICHENO'
 
",

@"
 
SELECT
COALESCE(CHARACTER_MAXIMUM_LENGTH,0) LENGTH 
FROM 
$CATALOG$.INFORMATION_SCHEMA.COLUMNS 
WHERE 
TABLE_CATALOG = '$CATALOG$' and 
TABLE_SCHEMA = '$USER$' and
TABLE_NAME = lower('LG_$FIRM$_$PERIOD$_KSLINES') and
COLUMN_NAME = lower('FICHENO')
 
")

                        , null), 0));

                    */

                    if (len_ > 0)
                        CASH_DOC_NR_LEN_SHORT = (len_ < 10);
                }

                switch (TERM_TYPE)
                {
                    case TERMTYPE.credit:

                        SAME_MAT_SUM = true; //+1 same material

                        MAT_PRICE_EDIT = true;

                        MAT_SELECT_FORM_BASE = true;
                        CLIENT_IN_DB = false; //client in db not need new create
                        CLIENT_INIT_FROM_USER = false;
                        ASK_MONTH = true;
                        ASK_PAYMENT = true;
                        USE_IF_NO_SLS_PRICE = true;

                        DOCUMENTS = false;

                        USER_TRACK_NR = false;
                        PRINT_ON_SAVE = true;

                        PRICE_DIFF_BY_PRCH = true;
                        ////////
                        numberFormatGen = "0.0;-0.0; ";
                        numberFormatGen2 = "0.0;-0.0; ";
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////




                        break;
                    case TERMTYPE.magazinWholesaleSlsman:

                        CLIENT_SELECT = true;

                        CLIENT_TO_TRACK = true;


                        SAME_MAT_SUM = true; //+1 same material

                        MAT_PRICE_EDIT = false;

                        MAT_SELECT_FORM_BASE = true;
                        CLIENT_INIT_FROM_USER = true; //client in db not need new create

                        ASK_MONTH = false;
                        ASK_PAYMENT = true;
                        USE_IF_NO_SLS_PRICE = false;
                        USE_SLSMAN = false;
                        DOCUMENTS = false;

                        USER_TRACK_NR = true;
                        PRINT_ON_SAVE = true;

                        PRICE_DIFF_BY_PRCH = false;

                        STOP_CLOSE_FULL_DOC = true;

                        USE_MANUAL_DISCOUNT = true;

                        CHANGE_PAYPLAN = true;

                        APPLY_CAMPAGIN = true;

                        BEEP = true;


                        INFO_TO_LEFT = true;
                        HIDE_DISC = false;

                        INFO_SCALE = 2.5 * 0.9;

                        ////////
                        numberFormatGen = "0.00;-0.00; ";
                        numberFormatGen2 = "0.00;-0.00; ";
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////




                        break;
                    case TERMTYPE.magazinWholesale:

                        CLIENT_SELECT = true;

                        CLIENT_TO_TRACK = true;


                        SAME_MAT_SUM = true; //+1 same material

                        MAT_PRICE_EDIT = true;

                        MAT_SELECT_FORM_BASE = true;
                        CLIENT_INIT_FROM_USER = true; //client in db not need new create

                        ASK_MONTH = false;
                        ASK_PAYMENT = true;
                        USE_IF_NO_SLS_PRICE = false;
                        USE_SLSMAN = false;
                        DOCUMENTS = false;

                        USER_TRACK_NR = true;
                        PRINT_ON_SAVE = false;

                        STOP_CLOSE_FULL_DOC = true;

                        PRICE_DIFF_BY_PRCH = true;

                        USE_MANUAL_DISCOUNT = true;

                        CHANGE_PAYPLAN = true;

                        ////////
                        numberFormatGen = "0.00;-0.00; ";
                        numberFormatGen2 = "0.00;-0.00; ";
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////




                        break;
                    case TERMTYPE.magazin:

                        CLIENT_SELECT = false;

                        CLIENT_TO_TRACK = true;

                        SAME_MAT_SUM = true; //+1 same material

                        MAT_PRICE_EDIT = false;

                        MAT_SELECT_FORM_BASE = true;
                        CLIENT_INIT_FROM_USER = true; //client in db not need new create

                        ASK_MONTH = false;
                        ASK_PAYMENT = true;
                        USE_IF_NO_SLS_PRICE = false;
                        DOCUMENTS = false;
                        USER_TRACK_NR = true;
                        PRINT_ON_SAVE = true;

                        PRICE_DIFF_BY_PRCH = false;
                        USE_SLSMAN = false;



                        FULL_CLEAN = false;

                        STOP_CLOSE_FULL_DOC = true;

                        DOC_BACKUP = true;

                        LOG_AMOUNT_DEC = true;

                        MAKE_PAYMENT = false;


                        INFO_TO_LEFT = true;
                        HIDE_DISC = true;

                        USE_BONUS_MAQ = true;

                        INFO_SCALE = 2.5 * 0.9;
                        ////////
                        numberFormatGen = "0.00;-0.00; ";
                        numberFormatGen2 = "0.00;-0.00; ";
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////



                        break;
                    case TERMTYPE.aptek:




                        SAME_MAT_SUM = false; //+1 same material

                        MAT_PRICE_EDIT = false;

                        MAT_SELECT_FORM_BASE = true;
                        CLIENT_INIT_FROM_USER = true; //client in db not need new create

                        ASK_MONTH = false;
                        ASK_PAYMENT = false;
                        USE_IF_NO_SLS_PRICE = false;
                        DOCUMENTS = false;
                        USER_TRACK_NR = true;
                        PRINT_ON_SAVE = false;

                        PRICE_DIFF_BY_PRCH = false;
                        USE_SLSMAN = false;
                        SLS_TO_CASH_FORCE = true;

                        ////////
                        numberFormatGen = "0.00;-0.00; ";
                        numberFormatGen2 = "0.00;-0.00; ";
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        break;
                    case TERMTYPE.restoran:



                        ////////////////////////////////////////

                        GRID_ROW_H = (int)(GRID_ROW_H * 1.5);
                        CMD_BTN_H = (int)(CMD_BTN_H * 1.2);
                        FONT_SIZE = (int)(FONT_SIZE * 1.2);

                        ////////////////////////////////////////
                        CLIENT_SELECT = true;

                        BARCODE_LEN_CLIENT = 3;

                        DOCUMENTS = true;

                        NO_PAYMENT = true;

                        SAME_MAT_SUM = false; //+1 same material

                        MAT_PRICE_EDIT = false;

                        MAT_SELECT_FORM_BASE = false;

                        CLIENT_INIT_FROM_USER = true; //client in db not need new create

                        ASK_MONTH = false;
                        ASK_PAYMENT = false;
                        USE_IF_NO_SLS_PRICE = false;
                        USE_SLSMAN = true;


                        USER_TRACK_NR = true;
                        PRINT_ON_SAVE = true;

                        PRICE_DIFF_BY_PRCH = false;



                        CHANGE_PAYPLAN = false;

                        SLSMAN_CLOSE = true;

                        DOC_PREFIX = "R";


                        //  INFO_TO_LEFT = true;
                        HIDE_DISC = true;

                        //   INFO_SCALE = 1;


                        ////////
                        numberFormatGen = "0.0;-0.0; ";
                        numberFormatGen2 = "0.0;-0.0; ";
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////





                        break;
                    case TERMTYPE.hotel:

                        GRID_ROW_H = (int)(GRID_ROW_H * 1.2);
                        CMD_BTN_H = (int)(CMD_BTN_H * 1.2);
                        FONT_SIZE = (int)(FONT_SIZE * 1.2);

                        CLIENT_SELECT = true;

                        CLIENT_IN_DB = false;


                        SAME_MAT_SUM = false; //+1 same material

                        MAT_PRICE_EDIT = true;

                        NO_PAYMENT = false;

                        MAT_SELECT_FORM_BASE = false;
                        CLIENT_INIT_FROM_USER = false; //client in db not need new create

                        ASK_MONTH = false;
                        ASK_PAYMENT = true;
                        USE_IF_NO_SLS_PRICE = false;

                        USER_TRACK_NR = false;
                        PRINT_ON_SAVE = false;

                        PRICE_DIFF_BY_PRCH = false;

                        USE_SLSMAN = false;

                        USE_MANUAL_DISCOUNT = true;

                        DOC_PREFIX = "H";

                        ////////
                        numberFormatGen = "0.0;-0.0; ";
                        numberFormatGen2 = "0.0;-0.0; ";
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////





                        break;

                    case TERMTYPE.barcode:

                        FILL_DOC = true;

                        NO_PAYMENT = true;

                        BARCODE_LEN10_CLIENT = false;

                        CLIENT_SELECT = false;

                        CLIENT_TO_TRACK = false;

                        SAME_MAT_SUM = true; //+1 same material

                        MAT_PRICE_EDIT = false;

                        MAT_SELECT_FORM_BASE = true;

                        CLIENT_INIT_FROM_USER = false; //client in db not need new create

                        ASK_MONTH = false;
                        ASK_PAYMENT = false;
                        USE_IF_NO_SLS_PRICE = true;
                        DOCUMENTS = false;
                        USER_TRACK_NR = false;
                        PRINT_ON_SAVE = false;

                        PRICE_DIFF_BY_PRCH = false;
                        USE_SLSMAN = false;
                        ////////
                        numberFormatGen = "0.00;-0.00; ";
                        numberFormatGen2 = "0.00;-0.00; ";
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////



                        break;

                    case TERMTYPE.production:

                        FILL_DOC = true;

                        NO_PAYMENT = true;

                        BARCODE_LEN10_CLIENT = false;

                        CLIENT_SELECT = false;

                        CLIENT_TO_TRACK = false;

                        SAME_MAT_SUM = true; //+1 same material

                        MAT_PRICE_EDIT = false;

                        MAT_SELECT_FORM_BASE = true;

                        CLIENT_INIT_FROM_USER = false; //client in db not need new create

                        ASK_MONTH = false;
                        ASK_PAYMENT = false;
                        USE_IF_NO_SLS_PRICE = true;
                        DOCUMENTS = false;
                        USER_TRACK_NR = false;
                        PRINT_ON_SAVE = false;

                        PRICE_DIFF_BY_PRCH = false;
                        USE_SLSMAN = false;
                        ////////
                        numberFormatGen = "0.00;-0.00; ";
                        numberFormatGen2 = "0.00;-0.00; ";
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////



                        break;
                    case TERMTYPE.count:

                        CHANGE_DATE = true;

                        ASK_SAVE = true;

                        DOC_PREFIX = "C";

                        PRICE_TYPE = 1;

                        WAREHOUSE_SELECT = true;

                        FILL_DOC = true;

                        NO_PAYMENT = true;

                        BARCODE_LEN10_CLIENT = false;

                        CLIENT_SELECT = false;

                        CLIENT_TO_TRACK = false;

                        SAME_MAT_SUM = true; //+1 same material

                        MAT_PRICE_EDIT = false;

                        MAT_SELECT_FORM_BASE = true;

                        CLIENT_INIT_FROM_USER = false; //client in db not need new create

                        ASK_MONTH = false;
                        ASK_PAYMENT = false;
                        USE_IF_NO_SLS_PRICE = true;
                        DOCUMENTS = false;
                        USER_TRACK_NR = false;
                        PRINT_ON_SAVE = false;

                        PRICE_DIFF_BY_PRCH = false;
                        USE_SLSMAN = false;

                        USE_ONHAND = true;
                        USE_ONHAND_TOT = true;
                        USE_ONHAND_DIFF = true;

                        CHANGE_PAYPLAN = true;
                        ////////
                        numberFormatGen = "0.00;-0.00; ";
                        numberFormatGen2 = "0.00;-0.00; ";
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////



                        break;


                    case TERMTYPE.pricing:

                        ASK_SAVE = true;

                        FILL_DOC = true;

                        NO_PAYMENT = true;

                        BARCODE_LEN10_CLIENT = false;

                        CLIENT_SELECT = false;

                        CLIENT_TO_TRACK = false;

                        SAME_MAT_SUM = true; //+1 same material

                        MAT_PRICE_EDIT = true;

                        MAT_SELECT_FORM_BASE = true;

                        CLIENT_INIT_FROM_USER = false; //client in db not need new create

                        ASK_MONTH = false;
                        ASK_PAYMENT = false;
                        USE_IF_NO_SLS_PRICE = true;
                        DOCUMENTS = false;
                        USER_TRACK_NR = false;
                        PRINT_ON_SAVE = false;

                        PRICE_DIFF_BY_PRCH = false;
                        USE_SLSMAN = false;
                        ////////
                        numberFormatGen = "0.00;-0.00; ";
                        numberFormatGen2 = "0.00;-0.00; ";
                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////



                        break;
                }

            }

            void INIT_BTNS(_PLUGIN pPlugin)
            {


                switch (TERM_TYPE)
                {
                    case TERMTYPE.credit:


                        ButtonCmdInfoArr1 = new BARCODETERM_BASE.ButtonCmdInfo[]{

                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.ADDNEW + "\nF12","cmdadd",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.SALER+ "\nCtrl+M","cmdsetslsman",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.PAYMENT + "\nF4","cmdpayment",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.MONTHS+ "\nCtrl+A","cmdmonth",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.PRICE + "\nCtrl+X","cmdprice",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.QUANTITY+"\nCtrl+Z","cmdquantity",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.CUSTOMER2+ "\nCtrl+C","cmdclcard","")
                            };

                        ButtonCmdInfoArr2 = new BARCODETERM_BASE.ButtonCmdInfo[]{

                                new BARCODETERM_BASE.ButtonCmdInfo(">>\nCtrl+>","cmdnextwin",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.CLEAN+ "\nShift+Del","cmddel",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.RESTART+ "\nCtrl+N","cmdclear",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.QUIT+ "\nEsc","cmdcancel",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.SAVE+ "\nF2","cmdsave",""),
                             };

                        break;
                    case TERMTYPE.magazinWholesaleSlsman:


                        ButtonCmdInfoArr1 = new BARCODETERM_BASE.ButtonCmdInfo[]{

                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.ADDNEW + "\nF12","cmdadd",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.CUSTOMER + "\nF11","cmdsetclcard",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.DISCPERC + "\nCtrl+E","cmdsetdiscountperc",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.DISCTOT + "\nCtrl+D","cmdsetdiscountamnt",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.LASTPRINT + "\nCtrl+P","cmdprint",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.PAYMENT + "\nF4","cmdpayment",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.PRICEGRP + "\nCtrl+O","cmdpayplan",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.QUANTITY+"\nCtrl+Z","cmdquantity",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.PROMO+ "\nCtrl+F9","cmdcampaign",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.RETURN+ "\nCtrl+Q","cmdslsret",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.CODE+"\nCtrl+K","cmdsetdoccode",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.SPECODE+ "\nCtrl+J","cmdsetdocspecode",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.DESC+ "\nCtrl+L","cmdsetdesc",""),
                            };

                        ButtonCmdInfoArr2 = new BARCODETERM_BASE.ButtonCmdInfo[]{

                                new BARCODETERM_BASE.ButtonCmdInfo(">>\nCtrl+>","cmdnextwin",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.CLEAN+ "\nShift+Del","cmddel",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.RESTART+ "\nCtrl+N","cmdclear",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.QUIT+ "\nEsc","cmdcancel",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.SAVE+ "\nF2","cmdsave",""),
                             };

                        break;
                    case TERMTYPE.magazinWholesale:



                        ButtonCmdInfoArr1 = new BARCODETERM_BASE.ButtonCmdInfo[]{

                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.ADDNEW + "\nF12","cmdadd",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.CUSTOMER + "\nF11","cmdsetclcard",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.DISCPERC + "\nCtrl+E","cmdsetdiscountperc",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.DISCTOT + "\nCtrl+D","cmdsetdiscountamnt",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.LASTPRINT + "\nCtrl+P","cmdprint",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.PAYMENT + "\nF4","cmdpayment",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.PAYMENT_TYPE + "\nF5","cmdchangecash",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.VAT + "\nF6","cmdchangeallvat",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.RETURN+ "\nCtrl+Q","cmdslsret",""),
                               // new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.PRICE + "\nCtrl+X","cmdprice",""),
                               // new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.QUANTITY+"\nCtrl+Z","cmdquantity","")
                            };

                        ButtonCmdInfoArr2 = new BARCODETERM_BASE.ButtonCmdInfo[]{

                                new BARCODETERM_BASE.ButtonCmdInfo(">>\nCtrl+>","cmdnextwin",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.CLEAN+ "\nShift+Del","cmddel",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.RESTART+ "\nCtrl+N","cmdclear",""),
                               
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.QUIT+ "\nEsc","cmdcancel",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.SAVE+ "\nF2","cmdsave",""),
                             };

                        break;
                    case TERMTYPE.magazin:



                        ButtonCmdInfoArr1 = new BARCODETERM_BASE.ButtonCmdInfo[]{
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.ADDNEW + "\nF12","cmdadd",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.PAYMENT + "\nF4","cmdpayment",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.QUANTITY+"\nCtrl+Z","cmdquantity",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.TEL+"\nCtrl+T","cmdsettrackno",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.DESC+ "\nCtrl+L","cmdsetdesc",""),
								new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.RETURN+ "\nCtrl+Q","cmdslsret",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.BONUS+ "\nCtrl+B","cmdusebonus",""),
                                 new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.DISCPERC + "\nCtrl+E","cmdsetdiscountperc",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.DISCTOT + "\nCtrl+D","cmdsetdiscountamnt",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.LASTPRINT + "\nCtrl+P","cmdprint",""),
                            };

                        ButtonCmdInfoArr2 = new BARCODETERM_BASE.ButtonCmdInfo[]{
                                new BARCODETERM_BASE.ButtonCmdInfo(">>\nCtrl+>","cmdnextwin",""),
                                 new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.NEW+ "\nCtrl+Y","cmdnewwin",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.CLEAN+ "\nShift+Del","cmddel",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.RESTART+ "\nCtrl+N","cmdclear",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.QUIT+ "\nEsc","cmdcancel",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.SAVE+ "\nF2","cmdsave",""),
                             };
                        break;
                    case TERMTYPE.aptek:




                        ButtonCmdInfoArr1 = new BARCODETERM_BASE.ButtonCmdInfo[]{
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.ADDNEW + "\nF12","cmdadd",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.PAYMENT + "\nF4","cmdpayment",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.QUANTITY+"\nCtrl+Z","cmdquantity","")
                            };

                        ButtonCmdInfoArr2 = new BARCODETERM_BASE.ButtonCmdInfo[]{
                                new BARCODETERM_BASE.ButtonCmdInfo(">>\nCtrl+>","cmdnextwin",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.CLEAN+ "\nShift+Del","cmddel",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.RESTART+ "\nCtrl+N","cmdclear",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.QUIT+ "\nEsc","cmdcancel",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.SAVE+ "\nF2","cmdsave",""),
                             };
                        break;
                    case TERMTYPE.restoran:



                        ButtonCmdInfoArr1 = new BARCODETERM_BASE.ButtonCmdInfo[]{
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.ADDNEW + "\nF12","cmdadd",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.QUANTITY+"\nCtrl+Z","cmdquantity",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.TABLE+"\nCtrl+M","cmdsetslsman",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.DESC+ "\nCtrl+L","cmdsetdesc",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.DOCS+ "\nCtrl+I","cmdslsinv",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.LASTPRINT + "\nCtrl+P","cmdprint",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.PRINT+ "\nAlt+P","cmdprintcurr",""),
                            };

                        ButtonCmdInfoArr2 = new BARCODETERM_BASE.ButtonCmdInfo[]{
                                new BARCODETERM_BASE.ButtonCmdInfo(">>\nCtrl+>","cmdnextwin",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.CLEAN+ "\nShift+Del","cmddel",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.RESTART+ "\nCtrl+N","cmdclear",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.QUIT+ "\nEsc","cmdcancel",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.SAVE+ "\nF2","cmdsave",""),
                             };


                        break;
                    case TERMTYPE.hotel:


                        ButtonCmdInfoArr1 = new BARCODETERM_BASE.ButtonCmdInfo[]{
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.ADDNEW + "\nF12","cmdadd",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.CUSTOMERS + "\nF11","cmdsetclcard",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.CUSTOMEROPEN + "\nCtrl+C","cmdclcard",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.DISCPERC + "\nCtrl+E","cmdsetdiscountperc",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.DISCTOT + "\nCtrl+D","cmdsetdiscountamnt",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.LASTPRINT + "\nCtrl+P","cmdprint",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.PAYMENT + "\nF4","cmdpayment",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.DESC+ "\nCtrl+L","cmdsetdesc",""),
                            };

                        ButtonCmdInfoArr2 = new BARCODETERM_BASE.ButtonCmdInfo[]{
                                new BARCODETERM_BASE.ButtonCmdInfo(">>\nCtrl+>","cmdnextwin",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.CLEAN+ "\nShift+Del","cmddel",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.RESTART+ "\nCtrl+N","cmdclear",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.QUIT+ "\nEsc","cmdcancel",""),
                                 new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.SAVE+ "\nF2","cmdsave",""),
                             };


                        break;

                    case TERMTYPE.barcode:



                        ButtonCmdInfoArr1 = new BARCODETERM_BASE.ButtonCmdInfo[]{
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.ADDNEW + "\nF12","cmdadd",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.QUANTITY+"\nCtrl+Z","cmdquantity",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.FILL+ "\nCtrl+F","cmdfill",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.FILTERBYSPC+ "\nAlt+S","cmdfilterspecode",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.SEARCH+ "\nCtrl+Sp","cmdfindtext",""),
                                     new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.PRINT+ "\nCtrl+P","cmdprint",""),
                            };

                        ButtonCmdInfoArr2 = new BARCODETERM_BASE.ButtonCmdInfo[]{
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.CLEAN+ "\nShift+Del","cmddel",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.RESTART+ "\nCtrl+N","cmdclear",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.QUIT+ "\nEsc","cmdcancel",""),
                         
                             };
                        break;

                    case TERMTYPE.production:



                        ButtonCmdInfoArr1 = new BARCODETERM_BASE.ButtonCmdInfo[]{
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.ADDNEW + "\nF12","cmdadd",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.QUANTITY+"\nCtrl+Z","cmdquantity",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.FILL+ "\nCtrl+F","cmdfill",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.SEARCH+ "\nCtrl+Sp","cmdfindtext",""),
 
                            };

                        ButtonCmdInfoArr2 = new BARCODETERM_BASE.ButtonCmdInfo[]{
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.CLEAN+ "\nShift+Del","cmddel",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.RESTART+ "\nCtrl+N","cmdclear",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.QUIT+ "\nEsc","cmdcancel",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.SAVE+ "\nF2","cmdsave",""),
                             };
                        break;
                    case TERMTYPE.count:



                        ButtonCmdInfoArr1 = new BARCODETERM_BASE.ButtonCmdInfo[]{
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.ADDNEW + "\nF12","cmdadd",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.QUANTITY+"\nCtrl+Z","cmdquantity",""),

                                 SAVE_TO_FILE? null:  
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.FILL+ "\nCtrl+F","cmdfill",""),

                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.WH+ "\nCtrl+W","cmdsetwh",""),

                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.DESC+ "\nCtrl+L","cmdsetdesc",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.SEARCH+ "\nCtrl+Sp","cmdfindtext",""),

                                 SAVE_TO_FILE? null:
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.DATE+ "\nCtrl+D","cmdsetdate",""),
                                 SAVE_TO_FILE? null:
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.PRICEGRP + "\nCtrl+O","cmdpayplan",""), //cl required
                                 SAVE_TO_FILE? null:
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.FILTERBYGRP+ "\nAlt+G","cmdfiltergroup",""),
                                 SAVE_TO_FILE? null:
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.FILTERBYSPC+ "\nAlt+S","cmdfilterspecode",""),
                                 SAVE_TO_FILE? null:
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.FILTERBYSPC+ " 2\nAlt+D","cmdfilterspecode2",""),
                                
                            };

                        ButtonCmdInfoArr2 = new BARCODETERM_BASE.ButtonCmdInfo[]{
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.CLEAN+ "\nShift+Del","cmddel",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.RESTART+ "\nCtrl+N","cmdclear",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.QUIT+ "\nEsc","cmdcancel",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.SAVE+ "\nF2","cmdsave",""),
                             };
                        break;


                    case TERMTYPE.pricing:


                        ButtonCmdInfoArr1 = new BARCODETERM_BASE.ButtonCmdInfo[]{
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.ADDNEW + "\nF12","cmdadd",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.PRICE + "\nCtrl+X","cmdprice",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.FILL+ "\nCtrl+F","cmdfill",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.FILTERBYGRP+ "\nAlt+G","cmdfiltergroup",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.FILTERBYSPC+ "\nAlt+S","cmdfilterspecode",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.FILTERBYSPC+ " 2\nAlt+D","cmdfilterspecode2",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.SEARCH+ "\nCtrl+Sp","cmdfindtext",""),
                                new BARCODETERM_BASE.ButtonCmdInfo("%","cmdpricingperc2all",""),
                               // new BARCODETERM_BASE.ButtonCmdInfo("Hesabla\nCtrl+H","cmdexe",""),
                            };

                        ButtonCmdInfoArr2 = new BARCODETERM_BASE.ButtonCmdInfo[]{
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.CLEAN+ "\nShift+Del","cmddel",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.RESTART+ "\nCtrl+N","cmdclear",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.QUIT+ "\nEsc","cmdcancel",""),
                                new BARCODETERM_BASE.ButtonCmdInfo(_LANG.L.SAVE+ "\nF2","cmdsave",""),
                             };
                        break;
                }

            }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////

            public TERMTYPE TERM_TYPE;
            //
            public float FONT_SIZE;

            public bool CASH_DOC_NR_LEN_SHORT = false;


            [DefaultValue(24)]
            [Browsable(true)]
            [Category("Grid")]
            [DisplayName("Grid Row Height")]
            [Description("Grid Row height in pixel")]
            public int GRID_ROW_H { get; set; }
            public bool SAME_MAT_SUM = false; //+1 same material
            public string CYPHCODE = "POS";
            public string CYPHCODE_CASH = "POS/CASH";
            public string CYPHCODE_CREDIT = "POS/CRED";
            public string CYPHCODE_CLOSED = "POS/CLOS";
            [DefaultValue(60)]
            [Browsable(true)]
            [Category("Buttons")]
            [DisplayName("Buttons Height")]
            [Description("Buttons height in pixel")]
            public int CMD_BTN_H { get; set; }
            public bool MAT_PRICE_EDIT = true;
            public string DOC_PREFIX = "A";
            public string POS_TERM_PREFIX = "";
            public string POS_USER_PREFIX = "";


            public double UI_SCALE = 1;
            public double UI_SCALE_H = 1;
            public double INFO_SCALE = 1;



            public bool MAT_SELECT_FORM_BASE = true;
            public bool FILL_DOC = false;
            public bool WAREHOUSE_SELECT = false;
            public bool WAREHOUSE_SELECT_FORM_BASE = true;

            public bool CLIENT_SELECT = false;
            public bool CLIENT_TO_TRACK = false;
            public bool CLIENT_SELECT_FORM_BASE = true;
            public bool CLIENT_IN_DB = true; //client in db not need new create

            public bool CLIENT_INIT_FROM_USER = true;
            public bool BARCODE_LEN10_CLIENT = true;
            public int BARCODE_LEN_CLIENT = -1;

            public bool ASK_PRINT = false;

            public bool SLSMAN_CLOSE = false;

            public bool CHANGE_DATE = false;


            public bool ASK_SAVE = false;

            public bool ASK_MONTH = true;
            public bool ASK_PAYMENT = true;
            public bool NO_PAYMENT = false;
            public bool USE_IF_NO_SLS_PRICE = true;
            public bool USER_TRACK_NR = true;
            public bool DOCUMENTS = false;
            public bool DOCUMENTS_READONLY = true;
            public bool PRINT_ON_SAVE = true;
            public bool USE_VAT = false;
            public bool EDIT_VAT = false;
            public double DEF_VAT = 0;

            public bool UPDATE_PRCH_PRICE_IN_CARD = true;
            public bool UPDATE_PRICE_COIF = true;
            public bool PRICE_DIFF_BY_PRCH = true;
            public bool USE_PASSWORD = false;
            public bool USE_PASSWORD_FOR_AMOUNT = true;

            public bool FULL_SCREEN = false;
            public bool MAKE_PAYMENT = true;
            public bool MAT_FROM_LIST = true;

            public bool SEARCH_MAT_BY_CODE = true;
            public bool SEARCH_CL_BY_CODE = true;
            public bool USE_SLSMAN = true;
            public bool FULL_CLEAN = true;
            public bool STOP_CLOSE_FULL_DOC = false;
            public bool SLS_TO_CASH_FORCE = false;
            public bool DOC_BACKUP = false;
            public bool LOG_AMOUNT_DEC = true;
            public bool PARENT_OFFLINE = false;
            //public bool RETURN_BY_PARENT_DOC = false;
            public bool RETURN_EDIT_PRICE = true;
            public bool INFO_TO_LEFT = false;
            public bool HIDE_DISC = true;

            //public bool LAST_DOC_BY_DATE_ = false;

            public bool JOB_DONE_INFORM = false;

            public bool BEEP = true;

            public int PRICE_TYPE = 2;// 1 prch 2 sls

            public bool USE_ONHAND = false;
            public bool USE_ONHAND_MAIN = false;
            public bool USE_ONHAND_ALL = false;
            public bool USE_ONHAND_TOT = false;
            public bool USE_ONHAND_DIFF = false;
            public bool NEGATIVE_LIGHT = false;
            public bool SAVE_TO_FILE = false;
            public bool CHANGE_PAYPLAN = false;
            public bool APPLY_CAMPAGIN = false;

            public bool APPLY_CAMPAGIN_AUTO = false;

            public bool CAMPAGIN_CLEAN_ON_PAYPLAN = true;


            public bool USE_MANUAL_DISCOUNT = false;

            public bool USE_MANUAL_DISCOUNT_TOT = true;
            public bool USE_MANUAL_DISCOUNT_PERC = true;
            public bool USE_LINE_AMNT_ON_FILL = false;



            public string CAMPAGIN_CYPHCODE = "POS";

            public Color COLOR_MAIN = Color.FromArgb(255, 245, 245, 245);// Color.WhiteSmoke;

            public string PRICE_FORMULA = "";
            ////////
            public string numberFormatGen = "0.00;-0.00; ";
            public string numberFormatGen2 = "0.00;-0.00; ";
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////

            public string REMOTE_SQL_CONN = "";
            public string CASH_LIST = "";

            public bool USE_BONUS_MAQ = false;

            public BARCODETERM_BASE.ButtonCmdInfo[] ButtonCmdInfoArr1;

            public BARCODETERM_BASE.ButtonCmdInfo[] ButtonCmdInfoArr2;




        }


        public class BARCODETERM_BASE
        {


            public static PARAMETERS PRM;

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////
            public static DateTime MY_CONVERTDATE(DateTime date, int time)
            {
                DateTime date_ = date;
                DateTime time_ = GETINTTIMETOTIME(time);
                return new DateTime(date_.Year, date_.Month, date_.Day, time_.Hour, time_.Minute, time_.Second);



            }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////

            public class EDATAGRIDVIEW : DataGridView
            {
                public EDATAGRIDVIEW()
                {
                    this.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
                }

                protected override void OnMouseWheel(MouseEventArgs e)
                {
                    base.OnMouseWheel(e);
                    if (e.Button == MouseButtons.None)
                    {
                        if (e.Delta > 0)
                            ProcessUpKey(Keys.Up);
                        else
                            if (e.Delta < 0)
                                ProcessDownKey(Keys.Down);
                    }


                }
            }
            class TOOLREMOTESQL
            {
                public static DataTable SQL(string sql, object[] args)
                {

                    var connStr = PRM.REMOTE_SQL_CONN;


                    if (string.IsNullOrEmpty(connStr))
                        throw new Exception("Set remote server connection string");

                    using (var conn = new System.Data.SqlClient.SqlConnection(connStr))
                    using (var command = new System.Data.SqlClient.SqlCommand(sql, conn) { CommandTimeout = 4 })
                    {
                        if (args != null)
                            for (int i = 0; i < args.Length; ++i)
                                command.Parameters.AddWithValue("@P" + (i + 1), args[i]);
                        try
                        {
                            conn.Open();
                        }
                        catch (Exception exc)
                        {
                            throw new Exception(_LANG.L.MSG_ERROR_CONN);
                        }

                        using (var adp = new System.Data.SqlClient.SqlDataAdapter(command))
                        {
                            var res = new DataTable();
                            try
                            {
                                adp.Fill(res);

                            }
                            catch (Exception exc)
                            {
                                throw new Exception(_LANG.L.MSG_ERROR_SQL);
                            }
                            return res;
                        }
                    }

                }

                public static object SQLSCALAR(string sql, object[] args)
                {
                    var r = SQL(sql, args);

                    return (r != null && r.Rows.Count > 0 && r.Columns.Count > 0) ? ISNULL(r.Rows[0][0], null) : null;
                }

            }


            class TOOLSGEN
            {
                public static void fillNullOnNewRow(DataTable pTab)
                {

                    pTab.TableNewRow += t_TableNewRow;

                }

                static void t_TableNewRow(object sender, DataTableNewRowEventArgs e)
                {
                    TAB_FILLNULL(e.Row);
                }

            }
            class TOOLSGRID
            {
                public static DataGridViewRow MY_GET_GRID_ROW(DataGridView pGrid, int pIndx)
                {
                    try
                    {
                        pIndx = Math.Max(pIndx, 0);
                        if (pGrid == null)
                            return null;
                        pIndx = Math.Min(pGrid.Rows.Count, pIndx);

                        return pGrid.Rows[pIndx] as DataGridViewRow;

                    }
                    catch { }
                    return null;


                }
                public static DataRow MY_GET_GRID_ROW_DATA(DataGridView pGrid, int pIndx)
                {
                    return MY_GET_GRID_ROW_DATA(MY_GET_GRID_ROW(pGrid, pIndx));
                }
                public static DataRow MY_GET_GRID_ROW_DATA(DataGridViewRow pRow)
                {

                    try
                    {
                        if (
                            pRow != null &&
                            (pRow.DataGridView as DataGridView) != null &&
                            pRow.DataGridView.DataSource != null &&
                            (pRow.DataBoundItem as DataRowView) != null)
                            return ((DataRowView)pRow.DataBoundItem).Row;
                    }
                    catch { }
                    return null;
                }



                public static DataRow MY_GET_GRID_ROW_DATA(DataGridView pGrid)
                {
                    return MY_GET_GRID_ROW_DATA(MY_GET_GRID_ROW(pGrid));
                }
                public static DataGridViewRow MY_GET_GRID_ROW(DataGridView pGrid)
                {

                    DataGridViewRow res_ = null;
                    try
                    {
                        res_ = pGrid.CurrentRow as DataGridViewRow;
                    }
                    catch { }
                    return res_;
                }
                public static int MY_POS_CORRECT(DataGridView pGrid, int pPos)
                {


                    try
                    {
                        if (pPos >= pGrid.Rows.Count)
                            pPos = pGrid.Rows.Count - 1;

                        if (pPos < 0)
                            pPos = 0;
                    }
                    catch { }

                    return pPos;
                }
                public static DataGridViewRow MY_GET_GRID_ROW(DataGridView pGrid, DataRow pRow)
                {
                    foreach (DataGridViewRow row in pGrid.Rows)
                        if (object.ReferenceEquals(pRow, MY_GET_GRID_ROW_DATA(row)))
                            return row;

                    return null;

                }
                public static void MY_SET_GRID_POSITION(DataGridView pGrid, int pPos, string pColumnCode)
                {
                    pPos = MY_POS_CORRECT(pGrid, pPos);
                    MY_SET_GRID_POSITION(pGrid, MY_GET_GRID_ROW_DATA(pGrid, pPos), pColumnCode);
                }

                public static void MY_SET_GRID_POSITION_OFFSET(DataGridView pGrid, int pPosOffset, string pColumnCode)
                {
                    if (pPosOffset == int.MinValue || pPosOffset == int.MaxValue)
                        pPosOffset = (int)Math.Sign(pPosOffset) * pGrid.DisplayedRowCount(false);

                    var p = MY_GET_GRID_POS(pGrid);
                    if (p >= 0)
                        MY_SET_GRID_POSITION(pGrid, p + pPosOffset, pColumnCode);
                }


                public static void MY_SET_GRID_POSITION(DataGridView pGrid, DataGridViewRow pRow, string pColumnCode)
                {
                    if (pRow == null)
                        return;

                    try
                    {

                        if (pRow != null)
                        {

                            DataGridViewCell cellCurr_ = null;

                            if (cellCurr_ == null)
                                if (pColumnCode != null && pColumnCode != "")
                                {
                                    foreach (DataGridViewColumn col_ in pGrid.Columns)
                                        if (col_.DataPropertyName == pColumnCode)
                                        {
                                            cellCurr_ = pRow.Cells[col_.Name]; //Index
                                            break;
                                        }
                                }

                            if (cellCurr_ == null)
                            {
                                int currCol_ = MY_GET_GRID_CELL(pGrid) != null ? MY_GET_GRID_CELL(pGrid).ColumnIndex : 0;
                                cellCurr_ = pRow.Cells[currCol_];
                            }

                            pGrid.CurrentCell = cellCurr_;

                        }
                    }
                    catch { }
                }
                public static void MY_SET_GRID_POSITION(DataGridView pGrid, DataRow pRow, string pColumnCode)
                {
                    if (pRow == null)
                        return;

                    try
                    {
                        foreach (DataGridViewRow rowGrid_ in pGrid.Rows)
                        {
                            DataRow rowData_ = MY_GET_GRID_ROW_DATA(rowGrid_);
                            if (rowData_ != null && Object.ReferenceEquals(rowData_, pRow))
                            {
                                MY_SET_GRID_POSITION(pGrid, rowGrid_, pColumnCode);
                                return;
                            }
                        }
                    }
                    catch { }

                }

                public static DataGridViewCell MY_GET_GRID_CELL(DataGridView pGrid)
                {
                    DataGridViewCell res_ = null;
                    try
                    {
                        res_ = pGrid.CurrentCell as DataGridViewCell;
                    }
                    catch { }
                    return res_;
                }

                public static int MY_GET_GRID_POS(DataGridView pGrid)
                {

                    DataGridViewRow res_ = null;
                    try
                    {
                        res_ = pGrid.CurrentRow as DataGridViewRow;
                        if (res_ != null)
                            return res_.Index;
                    }
                    catch { }
                    return -1;
                }




                public static bool MY_DEL_ROW(_PLUGIN pPLUGIN, DataGridView pGrid, string pInfoCol)
                {

                    try
                    {
                        DataRow row_ = MY_GET_GRID_ROW_DATA(pGrid);
                        if (row_ != null && row_.Table != null)
                        {
                            string info_ = TAB_GETROW(row_, pInfoCol).ToString();
                            if (pPLUGIN.MSGUSERASK(string.Format(_LANG.L.MSG_ASK_DELETE + " [{0}]", info_)))
                            {
                                row_.Table.Rows.Remove(row_);
                                return true;
                            }
                        }
                    }
                    catch { }

                    return false;
                }

                public static bool MY_DEL_ROW(_PLUGIN pPLUGIN, DataRow pRow, string pInfoCol, bool pForce)
                {

                    try
                    {
                        DataRow row_ = pRow;
                        if (row_ != null && row_.Table != null)
                        {
                            string info_ = TAB_GETROW(row_, pInfoCol).ToString();
                            if (
                                pForce ||
                                (PRM.USE_PASSWORD && PRM.USE_PASSWORD_FOR_AMOUNT) ||
                                pPLUGIN.MSGUSERASK(string.Format(_LANG.L.MSG_ASK_DELETE + " [{0}]", info_)))
                            {
                                row_.Table.Rows.Remove(row_);
                                return true;
                            }
                        }
                    }
                    catch { }

                    return false;
                }

                public static void MY_SET_GRID_POSITION_SEARCH(DataGridView pGrid, string pDataCol, object pValue)
                {
                    try
                    {
                        if (pGrid != null)
                            foreach (DataGridViewRow rowGrid_ in pGrid.Rows)
                            {
                                DataRow rowData_ = MY_GET_GRID_ROW_DATA(rowGrid_);
                                if (rowData_ != null)
                                {
                                    if (_PLUGIN.COMPARE(_PLUGIN.TAB_GETROW(rowData_, pDataCol), pValue))
                                    {
                                        MY_SET_GRID_POSITION(pGrid, rowGrid_, "");
                                    }
                                }

                            }
                    }
                    catch
                    {

                    }
                }



                public static void SETSTYLE(DataGridView pGrid)
                {
                    pGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                    if (pGrid.ColumnHeadersDefaultCellStyle.Font != null && !pGrid.ColumnHeadersDefaultCellStyle.Font.Bold)
                        pGrid.ColumnHeadersDefaultCellStyle.Font = new Font(pGrid.ColumnHeadersDefaultCellStyle.Font, pGrid.ColumnHeadersDefaultCellStyle.Font.Style | FontStyle.Bold);
                    pGrid.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;
                    pGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

                    pGrid.EnableHeadersVisualStyles = false;


                    pGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(153, 204, 255);// SystemColors.ControlDark;
                    pGrid.DefaultCellStyle.SelectionForeColor = pGrid.DefaultCellStyle.ForeColor;
                    pGrid.GridColor = System.Drawing.SystemColors.ControlLight;
                }

                public static DataGridViewColumn MY_ADD_COLUMN(DataGridView pGrid, string pText, string pCode, DataGridViewContentAlignment pAlign, string pFormat)
                {
                    if (pGrid.Columns[pCode] != null)
                        return null;


                    DataGridViewTextBoxColumn column_ = new DataGridViewTextBoxColumn();
                    column_.DataPropertyName = pCode;
                    column_.Name = pCode;
                    column_.HeaderText = pText;
                    column_.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    column_.DefaultCellStyle.Alignment = pAlign;
                    column_.DefaultCellStyle.Format = pFormat;


                    column_.SortMode = DataGridViewColumnSortMode.NotSortable;


                    pGrid.Columns.Add(column_);

                    return column_;
                }

                public static void MY_SETSTYLECOLOR(DataGridViewCellStyle pStyle, Color pColor)
                {

                    if (pStyle.BackColor != pColor)
                        pStyle.BackColor = pColor;

                    if (pStyle.SelectionBackColor != pColor)
                        pStyle.SelectionBackColor = pColor;

                }


                public static void MY_SETSTYLECOLORTEXT(DataGridViewCellStyle pStyle, Color pColor)
                {

                    if (pStyle.ForeColor != pColor)
                        pStyle.ForeColor = pColor;

                    if (pStyle.SelectionForeColor != pColor)
                        pStyle.SelectionForeColor = pColor;

                }
            }

            public class ButtonCmdInfo
            {
                public string TEXT;
                public string CMD;
                public string IMAGE;

                public ButtonCmdInfo(string pText, string pCmd, string pImage)
                {
                    TEXT = pText;
                    CMD = pCmd;
                    IMAGE = pImage;
                }

                public Button getButton()
                {
                    ButtonCmd b = new ButtonCmd();
                    b.ForeColor = SystemColors.ControlText;
                    b.Margin = new Padding(2);
                    b.Width = (int)(PRM.CMD_BTN_H * 1.6);
                    b.Dock = DockStyle.Left;
                    b.INFO = this;
                    return b;

                }
                class ButtonCmd : Button
                {
                    ButtonCmdInfo _INFO;
                    public ButtonCmdInfo INFO
                    {
                        get { return _INFO; }
                        set
                        {
                            _INFO = value;
                            Text = value.TEXT;
                        }


                    }

                    protected override void OnClick(EventArgs e)
                    {

                        base.OnClick(e);
                        //

                        CmdProcessing f = this.FindForm() as CmdProcessing;
                        if (f == null)
                            return;

                        switch (INFO.CMD)
                        {
                            case "cmdadd":
                                f.KeyProcess(Keys.F12);
                                break;
                            case "cmdshowdocs":
                                f.KeyProcess(Keys.F8);
                                break;
                            case "cmdsetclcard":
                                f.KeyProcess(Keys.F11);
                                break;
                            case "cmdsetdiscountperc":
                                f.KeyProcess(Keys.Control | Keys.E);
                                break;
                            case "cmdsetdiscountamnt":
                                f.KeyProcess(Keys.Control | Keys.D);
                                break;
                            case "cmdsetdesc":
                                f.KeyProcess(Keys.Control | Keys.L);
                                break;


                            case "cmdsetwh":
                                f.KeyProcess(Keys.Control | Keys.W);
                                break;
                            case "cmdfill":
                                f.KeyProcess(Keys.Control | Keys.F);
                                break;

                            case "cmdfindtext":
                                f.KeyProcess(Keys.Control | Keys.Space);
                                break;
                            case "cmdexe":
                                f.KeyProcess(Keys.Control | Keys.H);
                                break;
                            case "cmdsetdate":
                                f.KeyProcess(Keys.Control | Keys.R);
                                break;

                            case "cmdsave":
                                f.KeyProcess(Keys.F2);
                                break;
                            case "cmdpayment":
                                f.KeyProcess(Keys.F4);
                                break;
                            case "cmdchangecash":
                                f.KeyProcess(Keys.F5);
                                break;
                            case "cmdchangeallvat":
                                f.KeyProcess(Keys.F6);
                                break;
                            case "cmdpayplan":
                                f.KeyProcess(Keys.Control | Keys.O);
                                break;
                            case "cmdmonth":
                                f.KeyProcess(Keys.Control | Keys.A);
                                break;
                            case "cmddel":
                                f.KeyProcess(Keys.Shift | Keys.Delete);
                                break;
                            case "cmdclear":
                                f.KeyProcess(Keys.Control | Keys.N);
                                break;
                            case "cmdsetslsman":
                                f.KeyProcess(Keys.Control | Keys.M);
                                break;
                            case "cmdsettrackno":
                                f.KeyProcess(Keys.Control | Keys.T);
                                break;

                            case "cmdcampaign":
                                f.KeyProcess(Keys.Control | Keys.F9);
                                break;

                            case "cmdcancel":
                                f.KeyProcess(Keys.Escape);
                                break;
                            case "cmdslsinv":
                                f.KeyProcess(Keys.Control | Keys.I);
                                break;
                            case "cmdnextwin":
                                f.KeyProcess(Keys.Control | Keys.Right);
                                break;
                            case "cmdquantity":
                                f.KeyProcess(Keys.Control | Keys.Z);
                                break;
                            case "cmdusebonus":
                                f.KeyProcess(Keys.Control | Keys.B);
                                break;
                            case "cmdnewwin":
                                f.KeyProcess(Keys.Control | Keys.Y);
                                break;
                            case "cmdprice":
                                f.KeyProcess(Keys.Control | Keys.X);
                                break;
                            case "cmdclcard":
                                f.KeyProcess(Keys.Control | Keys.C);
                                break;
                            case "cmdprint":
                                f.KeyProcess(Keys.Control | Keys.P);
                                break;
                            case "cmdprintcurr":
                                f.KeyProcess(Keys.Alt | Keys.P);
                                break;
                            case "cmdslsret":
                                f.KeyProcess(Keys.Control | Keys.Q);
                                break;

                            case "cmdsetdoccode":
                                f.KeyProcess(Keys.Control | Keys.K);
                                break;
                            case "cmdsetdocspecode":
                                f.KeyProcess(Keys.Control | Keys.J);
                                break;

                            case "cmdfiltergroup":
                                f.KeyProcess(Keys.Alt | Keys.G);
                                break;
                            case "cmdfilterspecode":
                                f.KeyProcess(Keys.Alt | Keys.S);
                                break;
                            case "cmdfilterspecode2":
                                f.KeyProcess(Keys.Alt | Keys.D);
                                break;

                            default:
                                f.CmdProcess(INFO.CMD);
                                break;
                        }





                    }




                }

            }

            interface CmdProcessing
            {
                void KeyProcess(Keys keys);
                void CmdProcess(string pCmd);

            }

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////


            class TABLE_QPRODUCT
            {
                public class COLS
                {
                    public const string ITEMREF = "ITEMREF";
                    public const string AMOUNT = "AMOUNT";
                    public const string FICHENO = "FICHENO";
                    public const string HIDDEN_____RECALC = "HIDDEN_____RECALC";
                }
            }
            class TABLE_STLINE
            {
                public class COLS
                {
                    public const string STOCKREF = "STOCKREF";
                    public const string ITEMS_CODE = "ITEMS_CODE";
                    public const string ITEMS_NAME = "ITEMS_NAME";
                    public const string ITEMS_INTF1 = "ITEMS_INTF1";
                    public const string UNITBARCODE_LOGICALREF = "UNITBARCODE_LOGICALREF";
                    public const string UNITSETL_CODE = "UNITSETL_CODE";

                    public const string UOMREF = "UOMREF";
                    public const string PRICE = "PRICE";
                    public const string AMOUNT = "AMOUNT";
                    public const string TOTAL = "TOTAL";

                    public const string SPECODE = "SPECODE";
                    public const string LINEEXP = "LINEEXP";

                    public const string LINETYPE = "LINETYPE";
                    public const string GLOBTRANS = "GLOBTRANS";
                    public const string LOGICALREF = "LOGICALREF";
                    public const string CANCELLED = "CANCELLED";
                    public const string INVOICEREF = "INVOICEREF";

                    public const string PRCLIST_PRICE1 = "PRCLIST_PRICE1";
                    public const string PRCLIST_PRICE2 = "PRCLIST_PRICE2";

                    public const string DUMMY_FLOATF1 = "DUMMY_FLOATF1";
                    public const string DUMMY_FLOATF2 = "DUMMY_FLOATF2";
                    public const string DUMMY_FLOATF3 = "DUMMY_FLOATF3";

                    public const string DUMMY_ONHAND = "DUMMY_ONHAND";
                    public const string DUMMY_ONHAND_TOT = "DUMMY_ONHAND_TOT";
                    public const string DUMMY_ONHAND_DIFF = "DUMMY_ONHAND_DIFF";
                    public const string DUMMY_ONHAND_DIFFTOT = "DUMMY_ONHAND_DIFFTOT";

                    public const string DUMMY_ONHANDMAIN = "DUMMY_ONHANDMAIN";
                    public const string DUMMY_ONHANDALL = "DUMMY_ONHANDALL";

                    public const string DISCPER = "DISCPER";
                    public const string DISTDISC = "DISTDISC";
                    public const string LINENET = "LINENET";

                    public const string DATEBEG = "DATEBEG";
                    public const string DATEEND = "DATEEND";

                    public const string VAT = "VAT";
                }
                public class TYPES
                {
                    public static readonly Type LOGICALREF = typeof(int);

                    public static readonly Type STOCKREF = typeof(int);
                    public static readonly Type UOMREF = typeof(int);
                    public static readonly Type ITEMS_CODE = typeof(string);
                    public static readonly Type ITEMS_NAME = typeof(string);
                    public static readonly Type ITEMS_INTF1 = typeof(int);
                    public static readonly Type UNITBARCODE_LOGICALREF = typeof(int);
                    public static readonly Type UNITSETL_CODE = typeof(string);
                    public static readonly Type PRICE = typeof(double);
                    public static readonly Type AMOUNT = typeof(double);
                    public static readonly Type TOTAL = typeof(double);

                    public static readonly Type LINETYPE = typeof(short);
                    public static readonly Type GLOBTRANS = typeof(short);
                    public static readonly Type STLINE_LOGICALREF = typeof(int);

                    public static readonly Type PRCLIST_PRICE1 = typeof(double);
                    public static readonly Type PRCLIST_PRICE2 = typeof(double);

                    public static readonly Type DUMMY_FLOATF1 = typeof(double);
                    public static readonly Type DUMMY_FLOATF2 = typeof(double);
                    public static readonly Type DUMMY_FLOATF3 = typeof(double);

                    public static readonly Type DUMMY_ONHAND = typeof(double);
                    public static readonly Type DUMMY_ONHANDMAIN = typeof(double);
                    public static readonly Type DUMMY_ONHANDALL = typeof(double);

                }

                public enum LINETYPE
                {
                    material = 0,
                    promotion = 1,
                    discount = 2,
                    surcharge = 3,
                    service = 4
                }


                public static readonly Dictionary<short, string> LINETYPE_DESC = new Dictionary<short, string>() { 
                  {(short)LINETYPE.material,""},
                  {(short)LINETYPE.promotion,"PR"},
                  {(short)LINETYPE.discount,"EN"},
                  {(short)LINETYPE.surcharge,"XR"},
                  {(short)LINETYPE.service,"SR"},
          };

                public static readonly Dictionary<short, string> GLOBTRANS_DESC = new Dictionary<short, string>() { 
                  {(short)0,""},
                  {(short)1,"G"}
          };

                public class TOOLS
                {
                    static DataRow[] _searchSameDataRecords(DataTable pData, object pMatLRef, object pUnitLRef, double pPrice, string pLineDesc, bool pAll)
                    {
                        List<DataRow> list = new List<DataRow>();

                        foreach (DataRow row_ in pData.Rows)
                            if (!TAB_ROWDELETED(row_))
                            {
                                short glob_ = CASTASSHORT(TAB_GETROW(row_, TABLE_STLINE.COLS.GLOBTRANS));
                                short lineType_ = CASTASSHORT(TAB_GETROW(row_, TABLE_STLINE.COLS.LINETYPE));
                                if (glob_ == 0 && lineType_ == 0)
                                {
                                    object m_ = TAB_GETROW(row_, TABLE_STLINE.COLS.STOCKREF);
                                    object u_ = TAB_GETROW(row_, TABLE_STLINE.COLS.UOMREF);
                                    double p_ = CASTASDOUBLE(ISNULL(TAB_GETROW(row_, TABLE_STLINE.COLS.PRICE), 0.0));
                                    string d_ = CASTASSTRING(ISNULL(TAB_GETROW(row_, TABLE_STLINE.COLS.LINEEXP), ""));

                                    if (
                                        (COMPARE(m_, pMatLRef)) &&
                                        (COMPARE(u_, pUnitLRef)) &&
                                        ((PRM.TERM_TYPE == TERMTYPE.magazin || PRM.TERM_TYPE == TERMTYPE.pricing) || ISNUMEQUAL(p_, pPrice)) &&
                                        (pLineDesc == "*" || COMPARE(d_, pLineDesc))
                                        )
                                        list.Add(row_);


                                    if (!pAll && list.Count > 0)
                                        return list.ToArray();

                                }
                            }

                        return list.ToArray();
                    }

                    public static DataRow searchSameDataRecord(DataTable pData, object pMatLRef, object pUnitLRef, double pPrice, string pLineDesc)
                    {
                        DataRow[] res_ = _searchSameDataRecords(pData, pMatLRef, pUnitLRef, pPrice, pLineDesc, false);
                        return res_.Length > 0 ? res_[0] : null;
                    }
                    public static DataRow[] searchSameDataRecords(DataTable pData, object pMatLRef, object pUnitLRef, double pPrice, string pLineDesc)
                    {
                        return _searchSameDataRecords(pData, pMatLRef, pUnitLRef, pPrice, pLineDesc, true);
                    }
                    public static void deleteLine(DataRow row)
                    {
                        if (row != null && row.Table != null)
                            row.Table.Rows.Remove(row);
                    }

                    static DataTable _template;
                    public static DataTable createTable(_PLUGIN pPlugin)
                    {
                        if (_template == null)
                        {

                            _template = pPlugin.SQL(

                                MY_CHOOSE_SQL(
                                "SELECT TOP(0) * FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK)",
                                "SELECT * FROM LG_$FIRM$_$PERIOD$_STLINE LIMIT 0"));
                            _template.TableName = "STLINE";
                        }



                        var res_ = _template.Clone();


                        // res_.Columns.Add(TABLE_STLINE.COLS.LOGICALREF, TABLE_STLINE.TYPES.LOGICALREF);
                        // res_.Columns.Add(TABLE_STLINE.COLS.GLOBTRANS, TABLE_STLINE.TYPES.GLOBTRANS);
                        //  res_.Columns.Add(TABLE_STLINE.COLS.LINETYPE, TABLE_STLINE.TYPES.LINETYPE);
                        //  res_.Columns.Add(TABLE_STLINE.COLS.STOCKREF, TABLE_STLINE.TYPES.STOCKREF);
                        //  res_.Columns.Add(TABLE_STLINE.COLS.UOMREF, TABLE_STLINE.TYPES.UOMREF);
                        res_.Columns.Add(TABLE_STLINE.COLS.ITEMS_CODE, TABLE_STLINE.TYPES.ITEMS_CODE);
                        res_.Columns.Add(TABLE_STLINE.COLS.ITEMS_NAME, TABLE_STLINE.TYPES.ITEMS_NAME);

                        res_.Columns.Add(TABLE_STLINE.COLS.UNITBARCODE_LOGICALREF, TABLE_STLINE.TYPES.UNITBARCODE_LOGICALREF);
                        res_.Columns.Add(TABLE_STLINE.COLS.UNITSETL_CODE, TABLE_STLINE.TYPES.UNITSETL_CODE);
                        // res_.Columns.Add(TABLE_STLINE.COLS.PRICE, TABLE_STLINE.TYPES.PRICE);
                        // res_.Columns.Add(TABLE_STLINE.COLS.AMOUNT, TABLE_STLINE.TYPES.AMOUNT);
                        // res_.Columns.Add(TABLE_STLINE.COLS.TOTAL, TABLE_STLINE.TYPES.TOTAL, string.Format("[{0}]*[{1}]", TABLE_STLINE.COLS.PRICE, TABLE_STLINE.COLS.AMOUNT));

                        //res_.Columns[TABLE_STLINE.COLS.TOTAL].Expression = string.Format("[{0}]*[{1}]", TABLE_STLINE.COLS.PRICE, TABLE_STLINE.COLS.AMOUNT);


                        // res_.Columns.Add(TABLE_STLINE.COLS.SPECODE, typeof(string));
                        // res_.Columns.Add(TABLE_STLINE.COLS.LINEEXP, typeof(string));

                        res_.Columns.Add(TABLE_DUMMY.COLS.INDX_COL, TABLE_DUMMY.TYPES.INDX_COL_TYPE);

                        if (PRM.PRICE_DIFF_BY_PRCH)
                            res_.Columns.Add(TABLE_STLINE.COLS.PRCLIST_PRICE1, TABLE_STLINE.TYPES.PRCLIST_PRICE1);
                        res_.Columns.Add(TABLE_STLINE.COLS.PRCLIST_PRICE2, TABLE_STLINE.TYPES.PRCLIST_PRICE2);

                        if (PRM.PRICE_DIFF_BY_PRCH)
                        {
                            res_.Columns.Add(TABLE_STLINE.COLS.DUMMY_FLOATF1, TABLE_STLINE.TYPES.DUMMY_FLOATF1, string.Format("[{0}]*[{1}]", TABLE_STLINE.COLS.PRCLIST_PRICE1, TABLE_STLINE.COLS.AMOUNT));
                            res_.Columns.Add(TABLE_STLINE.COLS.DUMMY_FLOATF2, TABLE_STLINE.TYPES.DUMMY_FLOATF2, string.Format("([{0}]*(1+[{4}]/100)-[{1}])*[{2}]-[{3}]", TABLE_STLINE.COLS.PRICE, TABLE_STLINE.COLS.PRCLIST_PRICE1, TABLE_STLINE.COLS.AMOUNT, TABLE_STLINE.COLS.DISTDISC, TABLE_STLINE.COLS.VAT));
                        }
                        res_.Columns.Add(TABLE_STLINE.COLS.DUMMY_FLOATF3, TABLE_STLINE.TYPES.DUMMY_FLOATF3);

                        if (PRM.USE_ONHAND)
                            res_.Columns.Add(TABLE_STLINE.COLS.DUMMY_ONHAND, TABLE_STLINE.TYPES.DUMMY_ONHAND);
                        if (PRM.USE_ONHAND_MAIN)
                            res_.Columns.Add(TABLE_STLINE.COLS.DUMMY_ONHANDMAIN, TABLE_STLINE.TYPES.DUMMY_ONHANDMAIN);
                        if (PRM.USE_ONHAND_ALL)
                            res_.Columns.Add(TABLE_STLINE.COLS.DUMMY_ONHANDALL, TABLE_STLINE.TYPES.DUMMY_ONHANDALL);

                        if (PRM.USE_ONHAND_TOT)
                            res_.Columns.Add(TABLE_STLINE.COLS.DUMMY_ONHAND_TOT, TABLE_STLINE.TYPES.DUMMY_ONHAND);
                        if (PRM.USE_ONHAND_DIFF)
                        {
                            res_.Columns.Add(TABLE_STLINE.COLS.DUMMY_ONHAND_DIFF, TABLE_STLINE.TYPES.DUMMY_ONHAND);
                            res_.Columns.Add(TABLE_STLINE.COLS.DUMMY_ONHAND_DIFFTOT, TABLE_STLINE.TYPES.DUMMY_ONHAND);
                        }



                        if (PRM.TERM_TYPE == TERMTYPE.pricing)
                        {
                            res_.Columns.Add(PRICNGVARS.PL, TABLE_STLINE.TYPES.AMOUNT);
                            res_.Columns.Add(PRICNGVARS.PG, TABLE_STLINE.TYPES.AMOUNT);
                            res_.Columns.Add(PRICNGVARS.PD, TABLE_STLINE.TYPES.AMOUNT);
                            res_.Columns.Add(PRICNGVARS.PC, TABLE_STLINE.TYPES.AMOUNT);
                            res_.Columns.Add(PRICNGVARS.PA, TABLE_STLINE.TYPES.AMOUNT);
                            res_.Columns.Add(PRICNGVARS.PP, TABLE_STLINE.TYPES.AMOUNT);
                            res_.Columns.Add(PRICNGVARS.SP, TABLE_STLINE.TYPES.AMOUNT);
                            res_.Columns.Add(PRICNGVARS.FF, TABLE_STLINE.TYPES.AMOUNT);

                            res_.Columns.Add(PRICNGVARS._PRICINGINITED, typeof(string));
                        }



                        TOOLSGEN.fillNullOnNewRow(res_);

                        return res_;
                    }

                    public static DataRow addTrans(DataTable pTab, int pIndx)
                    {
                        DataRow new_ = pTab.NewRow();
                        pTab.Rows.InsertAt(new_, pIndx);

                        TAB_FILLNULL(pTab);
                        return new_;
                    }

                    public static DataRow addTransSub(DataRow pRow)
                    {
                        int indx = pRow.Table.Rows.IndexOf(pRow) + 1;
                        return addTrans(pRow.Table, indx);

                    }



                    public static DataRow addTrans(DataTable pTab)
                    {
                        DataRow r = TAB_ADDROW(pTab);
                        TAB_FILLNULL(pTab);
                        return r;
                    }
                    public static bool isLocal(DataRow row, LINETYPE type)
                    {
                        short lineType_ = CASTASSHORT(TAB_GETROW(row, TABLE_STLINE.COLS.LINETYPE));
                        short global_ = CASTASSHORT(TAB_GETROW(row, TABLE_STLINE.COLS.GLOBTRANS));

                        return (lineType_ == (short)type && global_ == 0);
                    }
                    public static bool isGlobal(DataRow row, LINETYPE type)
                    {
                        short lineType_ = CASTASSHORT(TAB_GETROW(row, TABLE_STLINE.COLS.LINETYPE));
                        short global_ = CASTASSHORT(TAB_GETROW(row, TABLE_STLINE.COLS.GLOBTRANS));

                        return (lineType_ == (short)type && global_ == 1);
                    }
                    public static bool isScripted(DataRow row)
                    {
                        return (TAB_GETROW(row, TABLE_STLINE.COLS.LINEEXP)).ToString().StartsWith("SCRIPT");
                    }
                    public static bool isLocalMat(DataRow row)
                    {
                        return isLocal(row, LINETYPE.material);

                    }

                    public static bool isLocalDisc(DataRow row)
                    {
                        return isLocal(row, LINETYPE.discount);

                    }
                    public static bool isLocalPromo(DataRow row)
                    {
                        return isLocal(row, LINETYPE.promotion);

                    }
                    public static bool isPromo(DataRow row)
                    {
                        return isLocal(row, LINETYPE.promotion) || isGlobal(row, LINETYPE.promotion);

                    }
                    public static DataRow getParentLine(DataRow pRow)
                    {
                        if (pRow != null && !TAB_ROWDELETED(pRow))
                        {
                            if (isLocalMat(pRow))
                                return pRow;

                            int i = pRow.Table.Rows.IndexOf(pRow) - 1;
                            if (i >= 0)
                                return getParentLine(pRow.Table.Rows[i]);
                        }

                        return null;
                    }
                    public static DataRow[] getSubLines(DataRow pRowParent, LINETYPE pLineType)
                    {
                        List<DataRow> list = new List<DataRow>();

                        if (pRowParent != null && !TAB_ROWDELETED(pRowParent) && TOOLS.isLocalMat(pRowParent))
                        {
                            var t = pRowParent.Table;
                            int i = t.Rows.IndexOf(pRowParent);
                            if (i >= 0)
                            {
                                ++i;
                                for (; i < t.Rows.Count; ++i)
                                {
                                    var r = t.Rows[i];
                                    if (!TAB_ROWDELETED(pRowParent))
                                    {
                                        if (TOOLS.isLocal(r, pLineType))
                                            list.Add(r);
                                        else
                                            break;
                                    }
                                }

                            }
                        }

                        return list.ToArray();
                    }

                }
            }


            class TABLE_DUMMY
            {

                public class COLS
                {
                    public const string INDX_COL = "INDX";
                    public const string DUMMY = "DUMMY";
                }

                public class TYPES
                {
                    public static readonly Type INDX_COL_TYPE = typeof(int);
                }



            }
            class TABLE_INFO
            {
                public class COLS
                {
                    public const string CODE = "CODE";
                    public const string NAME = "NAME";
                    public const string VALUE = "VALUE";
                }
                public class TYPES
                {
                    public static readonly Type CODE = typeof(string);
                    public static readonly Type NAME = typeof(string);
                    public static readonly Type VALUE = typeof(string);
                }
                public class CODES
                {
                    public const string TOTALNET = "TOTALNET";
                    public const string DISCOUNT = "DISCOUNT";
                    public const string PAYMENT = "PAYMENT";
                    public const string CHANGE = "CHANGE";
                    public const string VAT = "VAT";
                    public const string DUMMY = "DUMMY";
                }
            }
            class TABLE_MSG
            {
                public class COLS
                {
                    public const string CODE = "CODE";
                    public const string VALUE = "VALUE";
                }
                public class TYPES
                {
                    public static readonly Type CODE = typeof(string);
                    public static readonly Type VALUE = typeof(string);
                }
                public class CODES
                {
                    public const string MSG1 = "MSG1";
                    public const string MSG2 = "MSG2";
                    public const string MSG3 = "MSG3";
                    public const string MSG4 = "MSG4";
                    public const string MSG5 = "MSG5";
                }
            }
            class TABLE_ITEMS
            {
                public class COLS
                {
                    public const string LOGICALREF = "LOGICALREF";
                    public const string CODE = "CODE";
                    public const string NAME = "NAME";
                    public const string TEXTF5 = "TEXTF5";
                    public const string INTF1 = "INTF1";

                    public const string PRCLIST_PRICE = "PRCLIST_PRICE";
                    public const string UNITSETL_LOGICALREF = "UNITSETL_LOGICALREF";
                    public const string UNITSETL_CODE = "UNITSETL_CODE";
                }
            }


            class TABLE_PRCLIST
            {
                public class COLS
                {
                    public const string PRICE = "PRICE";
                }
            }
            class TABLE_CLCARD
            {
                public const string TABLE = "CLCARD";

                public class COLS
                {
                    public const string LOGICALREF = "LOGICALREF";
                    public const string CODE = "CODE";
                    public const string DEFINITION_ = "DEFINITION_";

                    public const string STATECODE = "STATECODE";

                    public const string ADDR1 = "ADDR1";
                    public const string ADDR2 = "ADDR2";
                    public const string TELNRS1 = "TELNRS1";
                    public const string TELNRS2 = "TELNRS2";
                    public const string DATEF1 = "DATEF1";
                    public const string DATEF2 = "DATEF2";
                    public const string FLOATF1 = "FLOATF1";
                    public const string FAXNR = "FAXNR";
                    public const string EMAILADDR = "EMAILADDR";
                    public const string DISCRATE = "DISCRATE";
                    public const string PAYMENTREF = "PAYMENTREF";

                }

            }
            class TABLE_UNITBARCODE
            {
                public class COLS
                {
                    public const string LOGICALREF = "LOGICALREF";
                    public const string ITEMREF = "ITEMREF";
                    public const string UNITLINEREF = "UNITLINEREF";
                    public const string BARCODE = "BARCODE";
                }

                public class TOOLS
                {


                    public class PARSER
                    {

                        const char prmPriceNumber = 'P';
                        const char prmPriceDecimal = 'Q';
                        const char prmWeithNumber = 'W';
                        const char prmWeithDecimal = 'X';
                        const char prmCode1 = 'M';
                        const char prmCode2 = 'N';
                        const char prmCode3 = 'T';

                        public string BARCODE { get; private set; }
                        public string FORMAT { get; private set; }

                        public PARSER(string pBarcode)
                        {
                            BARCODE = pBarcode;
                            if (pBarcode.Length == 13)
                            {
                                switch (LEFT(pBarcode, 2))
                                {
                                    case "27":
                                        FORMAT = "270MMMMWWXXXC";
                                        break;
                                    case "22":
                                        FORMAT = "22MMMMMWWXXXC";
                                        break;
                                    case "20":
                                        FORMAT = "20MMMMMWWWWWC";
                                        break;
                                    case "23":
                                        FORMAT = "23MMMMMWWXXXC";
                                        break;
                                    default:

                                        break;
                                }
                                //99 
                                switch (LEFT(pBarcode, 3))
                                {
                                    case "290":
                                        FORMAT = "NNNNNNNNNNNNN";
                                        break;
                                    case "297":
                                    case "299":
                                        FORMAT = "TTTTTTTTTTTTT";
                                        break;
                                }


                            }
                            else
                                if (pBarcode.Length == 10) //NFC card reader
                                {
                                    if (PRM.BARCODE_LEN10_CLIENT)
                                    {
                                        FORMAT = "NNNNNNNNNN";
                                    }
                                }
                                else
                                    if (PRM.BARCODE_LEN_CLIENT > 0 && pBarcode.Length == PRM.BARCODE_LEN_CLIENT) //NFC card reader
                                    {
                                        FORMAT = "".PadLeft(PRM.BARCODE_LEN_CLIENT, 'N');
                                    }
                            if (string.IsNullOrEmpty(FORMAT))
                                FORMAT = "".PadLeft(pBarcode.Length, 'M');

                            init();
                        }
                        public PARSER(string pBarcode, string pFormat)
                        {
                            BARCODE = pBarcode;
                            FORMAT = pFormat;

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
                                StringBuilder sbCode3_ = new StringBuilder();

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
                                        case prmCode3:
                                            sbCode3_.Append(v);
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
                                _CODE3 = sbCode3_.ToString();//.TrimStart('0');
                            }
                            catch
                            {
                                throw new Exception("Incorrect barcode [" + BARCODE + "] format [" + FORMAT + "]");
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
                        string _CODE1;//mat
                        public string CODE1
                        {
                            get { return _CODE1 == null ? "" : _CODE1; }
                        }

                        string _CODE2;//cl
                        public string CODE2
                        {
                            get { return _CODE2 == null ? "" : _CODE2; }
                        }
                        string _CODE3;//track
                        public string CODE3
                        {
                            get { return _CODE3 == null ? "" : _CODE3; }
                        }
                        public bool ISEMPTY
                        {
                            get { return string.IsNullOrEmpty(CODE1) && string.IsNullOrEmpty(CODE2); }

                        }

                    }


                }



            }
            class TABLE_UNITSETL
            {
                public class COLS
                {
                    public const string LOGICALREF = "LOGICALREF";
                    public const string CODE = "CODE";
                }
            }


            class TABLE_INVOICE
            {
                public class COLS
                {
                    public const string LOGICALREF = "LOGICALREF";
                    public const string FICHENO = "FICHENO";
                    public const string DATE_ = "DATE_";
                    public const string TIME_ = "TIME_";
                    public const string DOCODE = "DOCODE";
                    public const string SPECODE = "SPECODE";
                    public const string CYPHCODE = "CYPHCODE";
                    public const string NETTOTAL = "NETTOTAL";
                    public const string GENEXP1 = "GENEXP1";
                    public const string GENEXP2 = "GENEXP2";
                    public const string GENEXP3 = "GENEXP3";
                    public const string GENEXP4 = "GENEXP4";
                    public const string DOCTRACKINGNR = "DOCTRACKINGNR";
                    public const string TRCODE = "TRCODE";
                    public const string DUMMY_DATE = "DUMMY_DATETIME";
                    public const string CANCELLED = "CANCELLED";

                    public const string CLIENTREF = "CLIENTREF";
                    public const string SALESMANREF = "SALESMANREF";
                    public const string SLSMAN_CODE = "SLSMAN_CODE";
                    public const string SLSMAN_DEFINITION_ = "SLSMAN_DEFINITION_";

                    public const string CLCARD_DEFINITION_ = "CLCARD_DEFINITION_";
                    public const string CLCARD_CODE = "CLCARD_CODE";
                }

                public class TOOLS
                {

                    public static string getObjectId(object pLRef)
                    {
                        return "INVOICE" + (ISEMPTYLREF(pLRef) ? "" : FORMAT(pLRef));
                    }

                    public static Form getObjectOpenedForm(object pLRef)
                    {
                        if (!ISEMPTYLREF(pLRef))
                            foreach (Form f in Application.OpenForms)
                            {
                                ObjectWithId o = f as ObjectWithId;

                                if (o != null && o.getObjectId() == getObjectId(pLRef))
                                    return f;

                            }

                        return null;
                    }



                    static DataTable _template;
                    public static DataTable createTable(_PLUGIN pPlugin)
                    {
                        if (_template == null)
                        {

                            _template = pPlugin.SQL(
                                MY_CHOOSE_SQL(
                                "SELECT TOP(0) * FROM LG_$FIRM$_$PERIOD$_INVOICE WITH(NOLOCK)",
                                "SELECT * FROM LG_$FIRM$_$PERIOD$_INVOICE LIMIT 0"));
                            _template.TableName = "INVOICE";


                        }

                        var r = _template.Clone();
                        r.Rows.Add(r.NewRow());
                        TAB_FILLNULL(r);
                        return r;
                    }

                }
            }


            class TABLE_SPECODES
            {
                public class COLS
                {
                    public const string NR = "NR";
                    public const string LOGICALREF = "LOGICALREF";
                    public const string SPECODE = "SPECODE";
                    public const string DEFINITION_ = "DEFINITION_";
                }

                public class TYPES
                {
                    public static readonly Type LOGICALREF = typeof(int);
                    public static readonly Type SPECODE = typeof(string);
                    public static readonly Type DEFINITION_ = typeof(string);
                    public static readonly Type NR = typeof(int);
                }
            }
            class TABLE_LIST
            {
                public class COLS
                {
                    public const string ID_ = "ID_";
                    public const string DEFINITION_ = "DEFINITION_";
                }

                public class TYPES
                {
                    public static readonly Type ID_ = typeof(object);
                    public static readonly Type DEFINITION_ = typeof(string);

                }
            }

            class TABLE_SLSMAN
            {
                public class COLS
                {

                    public const string LOGICALREF = "LOGICALREF";
                    public const string CODE = "CODE";
                    public const string DEFINITION_ = "DEFINITION_";
                    public const string CLCARD_DEFINITION_ = "CLCARD_DEFINITION_"; //last staff inv with this slsman
                    public const string CLCARD_LOGICALREF = "CLCARD_LOGICALREF";
                }

                public class TYPES
                {
                    public static readonly Type LOGICALREF = typeof(int);
                    public static readonly Type CODE = typeof(string);
                    public static readonly Type DEFINITION_ = typeof(string);

                }
            }
            class TABLE_ITMUNITA
            {
                public class COLS
                {
                    public const string LOGICALREF = "LOGICALREF";
                    public const string UNITLINEREF = "UNITLINEREF";
                    public const string ITEMREF = "ITEMREF";
                    public const string CODE = "CODE";
                    public const string NAME = "NAME";
                }
            }
            public class Registers
            {
                DataRow header;
                DataSet dataSet;



                public Registers(DataSet pDataSet)
                {
                    dataSet = pDataSet;


                    var t = pDataSet.Tables["INVOICE"];
                    t.Clear();
                    t.Rows.Add(t.NewRow());
                    TAB_FILLNULL(t);
                    header = TAB_GETLASTROW(t);

                    clcard = new CLCARD(this);
                }

                public string formText;
                public bool firstInputText = true;

                public object docLRef { get { return header[TABLE_INVOICE.COLS.LOGICALREF]; } set { header[TABLE_INVOICE.COLS.LOGICALREF] = value; } }
                public double userToCashAmount = 0.0;

                public double getPaymentWinAmount()
                {

                    double matAmount_ =
                    this.totalNet > this.userToCashAmount ?
                    this.userToCashAmount :
                    this.totalNet;

                    if (PRM.SLS_TO_CASH_FORCE)
                        matAmount_ = this.totalNet;

                    matAmount_ = Math.Max(0, matAmount_);

                    return matAmount_;
                }

                public double totalGross = 0.0;
                public double totalNet = 0.0;
                public double totalDiscountLocal = 0.0;
                public double totalDiscount = 0.0;
                public double totalVAT = 0.0;
                public double discountPerc = 0.0;

                public double totalQuantity = 0.0;
                public double totalQuantityOnhand = 0.0;
                public double totalQuantityOnhandMath = 0.0;
                public double totalOnhand = 0.0;
                public double totalOnhandMath = 0.0;
                public double totalOnhandDiff = 0.0;
                public double totalOnhandDiffTot = 0.0;


                public double discountPercManual = 0.0;
                public double discountAmountManual = 0.0;

                public double bonus = 0.0;
                public double useBonus = 0.0;

                public double profitByPurchPrice = 0.0;
                public double totalByPurchPrice = 0.0;
                public string inputText = string.Empty;
                public object slsMan;
                public string slsManCode = string.Empty;
                public string slsManDesc = string.Empty;

                public short warehouse = -2;
                public string warehouseDesc = "";

                public string filterGroup = null;
                public string filterSpeCode = null;
                public string filterSpeCode2 = null;




                public bool hasFilter()
                {
                    return !string.IsNullOrEmpty(filterGroup) || !string.IsNullOrEmpty(filterSpeCode) || !string.IsNullOrEmpty(filterSpeCode2);
                }
                // public object clientLRef;
                //   public string clientCode = string.Empty;

                public string trackno = string.Empty;
                string docNr = string.Empty;
                public bool generatedDocNr = false;
                public string speCode = string.Empty;
                public string cyhpCode = PRM.CYPHCODE;

                public string cashCode = "";
                public string cashDesc = "";

                DateTime date = new DateTime(1900, 1, 1);
                public string desc1 = string.Empty;
                public string desc2 = string.Empty;
                public string desc3 = string.Empty;
                public string desc4 = string.Empty;
                public string docCode = string.Empty;

                public string promoCode = string.Empty;

                public object parentDocLRef = null;

                public int month = 0;
                public double monthPayment = 0;

                public DateTime getDocDate()
                {
                    if (date.Year == 1900)
                        date = DateTime.Now;

                    return date;
                }
                public void setDocDate(DateTime dt)
                {
                    date = dt;
                }

                public void setDocCode(string pCode)
                {
                    this.docNr = pCode;
                }

                public void nextCash(_PLUGIN PLUGIN)
                {
                    var list = PRM.CASH_LIST;

                    if (string.IsNullOrEmpty(list))
                        return;

                    var cashCodeTmp = cashCode;
                    var cashDescTmp = cashDesc;

                    var items = new List<string>(list.Split(','));

                    var indx = string.IsNullOrEmpty(cashCodeTmp) ?
                        -1 : //default
                        items.IndexOf(cashCodeTmp);

                    ++indx;

                    if (indx >= items.Count)
                        indx = 0;




                    cashCodeTmp = items[indx];
                    cashDescTmp = CASTASSTRING(ISNULL(
                        PLUGIN.SQLSCALAR("SELECT NAME FROM LG_$FIRM$_KSCARD WHERE CODE = @P1", new object[] { cashCodeTmp }),
                        "Error: " + cashCode));


                    cashCode = cashCodeTmp;
                    cashDesc = cashDescTmp;
                }

                object MY_GET_NEXTSEQ(_PLUGIN PLUGIN, string pSEQ)
                {

                    int res_ =
                    CASTASINT(ISNULL(PLUGIN.SQLSCALAR(
                    MY_CHOOSE_SQL(
                    "declare @v float exec LREF_LG_$FIRM$_$PERIOD$_" + pSEQ + " @v OUTPUT select @v VAL_",
                    "select nextval('LREF_LG_$FIRM$_$PERIOD$_" + pSEQ + "') V"),
                    new object[] { }), 0));

                    return res_;

                }
                object MY_GET_NEXTSEQ(_PLUGIN PLUGIN)
                {
                    return MY_GET_NEXTSEQ(PLUGIN, "SOURCESEQPERIOD");
                    //int res_ =
                    //CASTASINT(ISNULL(PLUGIN.SQLSCALAR("declare @v float exec LREF_LG_$FIRM$_$PERIOD$_SOURCESEQPERIOD @v OUTPUT select @v VAL_", new object[] { }), 0));

                    //return res_;

                }

                public string getDocCodeCash(_PLUGIN PLUGIN)
                {





                    if (PRM.CASH_DOC_NR_LEN_SHORT)
                    {
                        //A 00 000 000
                        object nextSeq_ = MY_GET_NEXTSEQ(PLUGIN, "KSLINES");
                        // code_ =  RIGHT("00" + _PLUGIN.FORMAT(PLUGIN.GETSYSPRM_USER()), 2);
                        string code_ = "";// FORMAT(PLUGIN.GETSYSPRM_USER());

                        code_ = code_ + RIGHT("00000000" + _PLUGIN.FORMAT(nextSeq_), 8);
                        code_ = PRM.DOC_PREFIX + code_;

                        return code_;
                    }



                    return getDocCode(PLUGIN);


                }


                public static string generateDocCode(_PLUGIN PLUGIN, DateTime pDate)
                {
                    bool operationOk = false;
                    var dt = pDate;
                    string res = null;

                    var userNr_ = PLUGIN.GETSYSPRM_USER();
                    for (int i = 0; i < 100; ++i)
                    {
                        var code_ = "";

                        code_ = code_ + "";
                        code_ = code_ + RIGHT("00" + FORMAT(dt.Year), 2);
                        code_ = code_ + RIGHT("00" + FORMAT(dt.Month), 2);
                        code_ = code_ + RIGHT("00" + FORMAT(dt.Day), 2);
                        code_ = code_ + "";
                        code_ = code_ + RIGHT("00" + FORMAT(dt.Hour), 2);
                        code_ = code_ + RIGHT("00" + FORMAT(dt.Minute), 2);
                        code_ = code_ + RIGHT("00" + FORMAT(dt.Second), 2);
                        code_ =
                           PRM.DOC_PREFIX +
                           PRM.POS_TERM_PREFIX +
                           (string.IsNullOrEmpty(PRM.POS_USER_PREFIX) ? (RIGHT("00" + FORMAT(userNr_), 2)) : (PRM.POS_USER_PREFIX)) +
                           code_;


                        if (true) // isSlsOrPrchDoc( Registers ))
                        {
                            if (ISNULL(PLUGIN.SQLSCALAR(
                                MY_CHOOSE_SQL(
                                "SELECT '1' FROM LG_$FIRM$_$PERIOD$_INVOICE WITH(NOLOCK) WHERE FICHENO = @P1",
                                "SELECT '1' FROM LG_$FIRM$_$PERIOD$_INVOICE WHERE FICHENO = @P1 LIMIT 1"), new object[] { code_ })))
                            {
                                res = code_;
                                operationOk = true;
                                break;
                            }
                        }
                        else
                        {
                            operationOk = true;
                            break;
                        }

                        dt = dt.AddSeconds(1);
                    }
                    //A00 000000 000000


                    if (!operationOk)
                    {
                        throw new Exception(_LANG.L.MSG_ERROR_NUMER);
                    }

                    return res;
                }

                public string getDocCode(_PLUGIN PLUGIN)
                {

                    string code_ = this.docNr;

                    if (!string.IsNullOrEmpty(code_))
                        return code_;



                    generatedDocNr = true;

                    DateTime dt = this.getDocDate();


                    return this.docNr = generateDocCode(PLUGIN, dt);


                }

                public readonly CLCARD clcard;

                public class CLCARD
                {
                    Registers registers;
                    public object lastClLref;


                    Dictionary<string, object> cacheDummy = new Dictionary<string, object>();

                    _PLUGIN PLUGIN;

                    public CLCARD(Registers pReg)
                    {
                        registers = pReg;
                    }

                    bool reinit = true;
                    //
                    DataTable table;
                    //DataSet ds;
                    //

                    void bindCodeChange(_PLUGIN pPLUGIN, DataSet pDs)
                    {
                        var tab_ = pDs.Tables[TABLE_CLCARD.TABLE];
                        if (tab_ != null)
                        {

                            tab_.ColumnChanged += tab__ColumnChanged;
                            PLUGIN = pPLUGIN;


                        }

                    }

                    bool blockCh = false;

                    void tab__ColumnChanged(object sender, DataColumnChangeEventArgs e)
                    {
                        if (blockCh)
                            return;
                        if (e.Column.ColumnName == TABLE_CLCARD.COLS.CODE)
                        {
                            blockCh = true;
                            try
                            {

                                if (PLUGIN != null)
                                {
                                    var code_ = e.ProposedValue as string;
                                    if (!ISNULL(code_))
                                    {
                                        var lref_ = PLUGIN.SQLSCALAR(
                                            MY_CHOOSE_SQL(
                                            "SELECT LOGICALREF FROM LG_$FIRM$_CLCARD WITH(NOLOCK) WHERE CODE=@P1",
                                            "SELECT LOGICALREF FROM LG_$FIRM$_CLCARD WHERE CODE=@P1"),

                                            new object[] { code_.Trim() });
                                        //clean if new code

                                        setClientRef(lref_, PLUGIN);

                                        if (ISEMPTYLREF(lref_)) //restore
                                            setValue(TABLE_CLCARD.COLS.CODE, code_);
                                    }

                                }



                            }
                            finally
                            {
                                blockCh = false;
                            }
                        }
                    }


                    void unbindCodeChange(DataSet pDs)
                    {
                        var tab_ = pDs.Tables[TABLE_CLCARD.TABLE];
                        if (tab_ != null)
                            tab_.ColumnChanged -= tab__ColumnChanged;

                        PLUGIN = null;
                    }



                    public void edit(_PLUGIN PLUGIN)
                    {
                        init(0, PLUGIN);
                        try
                        {
                            bindCodeChange(PLUGIN, registers.dataSet);

                            if (PRM.TERM_TYPE == TERMTYPE.credit)
                                PLUGIN.EDITDATA("adp.fin.rec.client.credit", registers.dataSet, null);
                            else
                                if (PRM.TERM_TYPE == TERMTYPE.hotel)
                                    PLUGIN.EDITDATA("adp.fin.rec.client.hotel", registers.dataSet, null);


                        }
                        finally
                        {
                            unbindCodeChange(registers.dataSet);
                        }
                        checkValues(PLUGIN);
                    }
                    public void setClientRef(object pLref, _PLUGIN PLUGIN)
                    {
                        var z = this.getClientRef();
                        if (COMPARE(z, pLref))
                            return;

                        reinit = true;


                        init(pLref, PLUGIN);
                    }
                    public object getClientRef()
                    {
                        return getValue(TABLE_CLCARD.COLS.LOGICALREF);
                    }
                    public object getPaymentRef()
                    {
                        return getValue(TABLE_CLCARD.COLS.PAYMENTREF);
                    }

                    public double getClientDisc()
                    {
                        return Math.Min(100, Math.Max(0, CASTASDOUBLE(getValue(TABLE_CLCARD.COLS.DISCRATE))));
                    }

                    public void setClientDisc(double val)
                    {
                        val = MIN(MAX(val, 0), 100);
                        setValue(TABLE_CLCARD.COLS.DISCRATE, val);
                    }


                    public string getClientDesc()
                    {
                        return CASTASSTRING(getValue(TABLE_CLCARD.COLS.DEFINITION_));
                    }
                    public string getClientCode()
                    {
                        return CASTASSTRING(getValue(TABLE_CLCARD.COLS.CODE));
                    }
                    public string getClientCodeSearch()
                    {
                        return CASTASSTRING(getValue(TABLE_CLCARD.COLS.STATECODE));
                    }

                    public string getClientBalanceDesc()
                    {
                        return CASTASSTRING(getValue("INFO_BALANCEDESC"));
                    }

                    public void setClientPayplan(object pLref, _PLUGIN PLUGIN)
                    {


                        setValue("PAYMENTREF", pLref);
                        refreshPayPlanDesc(PLUGIN);

                    }

                    void refreshPayPlanDesc(_PLUGIN PLUGIN)
                    {
                        string desc_ = CASTASSTRING(PLUGIN.SQLSCALAR(
                            MY_CHOOSE_SQL(
                            "SELECT TOP(1) PP.DEFINITION_ FROM LG_$FIRM$_PAYPLANS PP WITH(NOLOCK) WHERE PP.LOGICALREF=@P1",
                              "SELECT PP.DEFINITION_ FROM LG_$FIRM$_PAYPLANS PP WHERE PP.LOGICALREF=@P1 LIMIT 1"),

                            new object[] { ISNULL(getPaymentRef(), 0) }));
                        setValue("INFO_PAYPLANS", desc_);
                    }

                    public string getClientPayplanDesc()
                    {
                        return CASTASSTRING(getValue("INFO_PAYPLANS"));
                    }

                    public string getClientLastPaymentDesc()
                    {
                        return CASTASSTRING(getValue("INFO_LASTPAYMENTDESC"));
                    }
                    public bool isClientRefEmpty()
                    {
                        return ISEMPTYLREF(getClientRef());
                    }

                    public bool hasDbSameCl(_PLUGIN PLUGIN)
                    {
                        if (isClientRefEmpty())
                            return false;

                        if (!isClientRefEmpty() && !ISNULL(PLUGIN.SQLSCALAR(
                            MY_CHOOSE_SQL(
                            "SELECT TOP(1) 1 FROM LG_$FIRM$_CLCARD WHERE CODE = @P1",
                             "SELECT 1 FROM LG_$FIRM$_CLCARD WHERE CODE = @P1 LIMIT 1"),
                            new object[] { 
                            getClientCode() })))
                            return true;

                        return false;
                    }

                    public bool hasData()
                    {
                        return
                            table != null && table.Rows.Count > 0 &&
                            CASTASSTRING(getValue(TABLE_CLCARD.COLS.CODE)) != "" &&
                            CASTASSTRING(getValue(TABLE_CLCARD.COLS.DEFINITION_)) != "";
                    }
                    public void checkValues(_PLUGIN PLUGIN)
                    {
                        {

                            DateTime date_ = CASTASDATE(getValue(TABLE_CLCARD.COLS.DATEF1));
                            if (date_.Year == 1900)
                            {
                                date_ = registers.getDocDate();
                                date_ = date_.AddMonths(1);
                            }
                            date_ = new DateTime(date_.Year, date_.Month, Math.Min(28, date_.Day));
                            setValue(TABLE_CLCARD.COLS.DATEF1, date_);

                        }
                        {
                            if (registers.month > 0)
                            {
                                DateTime date_ = CASTASDATE(getValue(TABLE_CLCARD.COLS.DATEF1));
                                setValue(TABLE_CLCARD.COLS.DATEF2, date_.AddMonths(Math.Max(0, registers.month)));
                            }
                        }
                        {
                            string code_ = CASTASSTRING(getValue(TABLE_CLCARD.COLS.CODE));
                            if (code_ == "")
                            {
                                setValue(TABLE_CLCARD.COLS.CODE, registers.getDocCode(PLUGIN));
                            }
                        }
                    }
                    public void init(object pLref, _PLUGIN PLUGIN)
                    {
                        if (reinit)
                        {
                            if (ISEMPTYLREF(pLref))
                                pLref = 0;

                            table = PLUGIN.SQL(
                                MY_CHOOSE_SQL(
@"
 
SELECT TOP(1) 
CLCARD.*,

'' INFO_PAYPLANS,
ISNULL((SELECT TOP(1) CONVERT(nvarchar,ROUND((DEBIT - CREDIT),2)) FROM LG_$FIRM$_$PERIOD$_GNTOTCL TOT WITH(NOLOCK) WHERE (TOT.CARDREF = @P1 AND TOT.TOTTYP = 1)),'') INFO_BALANCEDESC,
ISNULL((SELECT TOP(1) LEFT(CONVERT(nvarchar,DATE_,120 ),10) + ': ' + CONVERT(nvarchar,ROUND(AMOUNT,2)) FROM LG_$FIRM$_$PERIOD$_CLFLINE L WITH(NOLOCK,INDEX=I$FIRM$_$PERIOD$_CLFLINE_I4) WHERE L.CLIENTREF = @P1 AND L.DATE_ > '19000101' AND L.MODULENR= 10  AND L.TRCODE=1 AND L.CANCELLED = 0 ORDER BY CLIENTREF DESC,DATE_ DESC,MODULENR DESC,TRCODE DESC,LOGICALREF DESC),'') INFO_LASTPAYMENTDESC
FROM 
LG_$FIRM$_CLCARD CLCARD 
WHERE LOGICALREF = @P1",
@"
SELECT 
CLCARD.*,
'' INFO_PAYPLANS,
COALESCE((SELECT cast(ROUND(cast((DEBIT - CREDIT) as numeric),2) as varchar) FROM LG_$FIRM$_$PERIOD$_GNTOTCL TOT WHERE (TOT.CARDREF = @P1 AND TOT.TOTTYP = 1) LIMIT 1 ),'') INFO_BALANCEDESC, 
COALESCE((SELECT substr(cast(DATE_ as varchar),1,10) || ': ' || cast(ROUND(cast(AMOUNT as numeric),2) as varchar) FROM LG_$FIRM$_$PERIOD$_CLFLINE L WHERE L.CLIENTREF = @P1 AND L.DATE_ > '19000101' AND L.MODULENR= 10  AND L.TRCODE=1 AND L.CANCELLED = 0 ORDER BY CLIENTREF DESC,DATE_ DESC,MODULENR DESC,TRCODE DESC,LOGICALREF DESC LIMIT 1),'') INFO_LASTPAYMENTDESC
FROM 
LG_$FIRM$_CLCARD CLCARD 
WHERE LOGICALREF = @P1
LIMIT 1
"
                       ),
new object[] { pLref });
                            table.TableName = TABLE_CLCARD.TABLE;
                            //
                            if (table.Rows.Count == 0)
                                _PLUGIN.TAB_ADDROW(table);
                            //
                            TAB_FILLNULL(table);
                            //
                            //ds = new DataSet();
                            //ds.Tables.Add(table);
                            //

                            registers.header[TABLE_INVOICE.COLS.CLIENTREF] = pLref;

                            var orgnTab_ = registers.dataSet.Tables[TABLE_CLCARD.TABLE];
                            if (orgnTab_ == null)
                                registers.dataSet.Tables.Add(table);
                            else
                            {
                                orgnTab_.Clear();
                                orgnTab_.ImportRow(table.Rows[0]);
                                table = orgnTab_;
                            }
                            //
                            refreshPayPlanDesc(PLUGIN);
                            //
                            reinit = false;
                        }

                        checkValues(PLUGIN);
                    }



                    public object getValue(string pCol)
                    {
                        if (table == null)
                        {
                            if (cacheDummy.ContainsKey(pCol))
                                return cacheDummy[pCol];

                            return null;
                        }

                        return TAB_GETROW(table, pCol);
                    }
                    public void setValue(string pCol, object pVal)
                    {
                        if (table == null)
                        {
                            switch (pCol)
                            {

                                case "INFO_PAYPLANS":
                                case "PAYMENTREF":
                                    cacheDummy[pCol] = pVal;
                                    break;
                            }


                            return;
                        }

                        TAB_SETROW(table, pCol, pVal);
                    }
                }



                class CNTS
                {
                    public const string backUpName = "_REGISTERS";
                    public const string PARENTDOC = "PARENTDOC";
                    public const string TRACKNR = "TRACKNR";
                }
                public void readBackUp(DataSet pDs)
                {
                    var tab = pDs.Tables[CNTS.backUpName];
                    if (tab == null)
                        return;

                    var row_ = TAB_GETLASTROW(tab);

                    this.parentDocLRef = ISNULL(TAB_GETROW(row_, CNTS.PARENTDOC), 0);
                    this.trackno = ISNULL(TAB_GETROW(row_, CNTS.TRACKNR), "").ToString();
                }

                public void writeBackUp(DataSet pDs)
                {
                    DataTable tab = new DataTable(CNTS.backUpName);
                    tab.Columns.Add(CNTS.PARENTDOC, typeof(int));
                    tab.Columns.Add(CNTS.TRACKNR, typeof(string));

                    tab.Rows.Add(
                        ISEMPTYLREF(this.parentDocLRef) ? 0 : this.parentDocLRef,
                        string.IsNullOrEmpty(this.trackno) ? "" : this.trackno
                        );

                    pDs.Tables.Add(tab);
                }
            }

            class MathOper
            {
                public const string sum = "+";
                public const string sub = "-";
                public const string mult = "*";
                public const string div = "/";


                public static bool isValid(string pOper)
                {

                    return (pOper == sum || pOper == sub || pOper == mult || pOper == div);

                }
            }



            abstract class FormReference : Form
            {

                public enum FormType
                {
                    grid = 1,
                    box = 2
                }


                protected FormType formType = FormType.grid;


                DataGridView grid_;
                FlowLayoutPanel boxes_;

                Panel panelTop;
                Panel panelTopFilter;
                Panel panelContent;
                Panel panelBottom;



                public bool stripeRowBackColor = true;
                public Color stripeColor = SystemColors.ControlLight;

                protected string sortColumn;
                protected bool sortAsc;
                //
                protected string searchColumn = "";
                protected object searchValue;
                //
                protected bool btnExit = true;
                protected bool btnSelect = false;

                string searchedText = string.Empty;
                string oldText;
                //


                public FormReference()
                {
                    //
                    KeyPreview = true;
                    BackColor = PRM.COLOR_MAIN;
                    Icon = CTRL_FORM_ICON();
                    //if (Application.OpenForms.Count > 0)
                    //    Icon = Application.OpenForms[0].Icon;

                    // 
                    //FormBorderStyle = FormBorderStyle.None;
                    WindowState = FormWindowState.Maximized;
                    Size = new System.Drawing.Size(720, 450);
                    Font = new System.Drawing.Font(Font.FontFamily, PRM.FONT_SIZE, FontStyle.Bold);
                    //
                    KeyPreview = true;

                }



                protected abstract DataReference getDataReference();

                bool inited = false;
                protected override void OnLoad(EventArgs e)
                {
                    if (!inited)
                    {

                        try
                        {
                            inited = true;

                            this.SuspendLayout();

                            if (formType == FormType.grid)
                                initGrid();
                            else
                                if (formType == FormType.box)
                                    initBoxes();

                            initPanels();
                            //
                            initStruct();
                            //
                            SORT();
                            SEARCH();
                            //
                            oldText = oldText ?? Text;

                            if (formType == FormType.grid)
                            {
                                this.ActiveControl = grid_;
                                grid_.Focus();

                            }

                            initedForm();

                        }
                        catch (Exception exc)
                        {

                            getDataReference().exceptionHandler(exc);
                            throw new Exception("Form load error", exc);
                        }
                        finally
                        {
                            this.ResumeLayout();
                        }


                    }


                    base.OnLoad(e);
                }

                protected virtual void initedForm()
                {

                }

                void initPanels()
                {
                    panelTop = new Panel();
                    panelTopFilter = new Panel();
                    panelContent = new Panel();
                    panelBottom = new Panel();
                }

                protected DataGridView getGrid()
                {
                    return grid_;
                }

                protected FlowLayoutPanel getBoxes()
                {
                    return boxes_;
                }

                protected virtual string filterColumn()
                {
                    return null;


                }

                void searchData(string searchedText)
                {
                    if (formType != FormType.grid)
                        return;

                    try
                    {
                        var t = grid_.DataSource as DataTable;
                        if (t == null)
                            return;

                        var filter = string.Empty;
                        if (string.IsNullOrEmpty(searchedText))
                            filter = string.Empty;
                        else
                        {
                            var col_ = filterColumn();
                            if (col_ == null)
                                return;

                            var arr_ = EXPLODELISTSEP(searchedText, ' ');

                            List<string> list = new List<string>();

                            foreach (var x in arr_)
                            {
                                var f = x.Trim();
                                if (f != string.Empty)
                                    list.Add(string.Format("{0} LIKE '%{1}%'", col_, f));
                            }

                            filter = string.Join(" OR ", list.ToArray());

                        }


                        if (t.DefaultView.RowFilter != filter)
                        {
                            t.DefaultView.RowFilter = filter;

                            Text = string.Format(searchedText == "" ? "{0}" : "{0} [{1}]", oldText, searchedText);

                        }


                    }
                    catch (Exception exc)
                    {

                        this.getDataReference().exceptionHandler(exc);
                    }
                }


                protected virtual void initBoxes()
                {

                    boxes_ = new FlowLayoutPanel();
                    boxes_.AutoScroll = true;
                    boxes_.Dock = DockStyle.Fill;
                    boxes_.FlowDirection = FlowDirection.LeftToRight;
                    boxes_.WrapContents = true;

                }

                protected virtual void initGrid()
                {



                    grid_ = new EDATAGRIDVIEW();
                    grid_.DataError += new DataGridViewDataErrorEventHandler(this.gridError);
                    grid_.AllowUserToResizeRows = false;
                    grid_.AllowUserToAddRows = false;
                    grid_.AutoGenerateColumns = false;
                    // grid_.BackgroundColor = SystemColors.Window;
                    grid_.AllowUserToDeleteRows = false;
                    grid_.MultiSelect = false;
                    grid_.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    grid_.RowHeadersVisible = false;
                    grid_.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    grid_.BorderStyle = BorderStyle.None;
                    grid_.RowTemplate.Height = PRM.GRID_ROW_H;
                    grid_.EditMode = DataGridViewEditMode.EditProgrammatically;
                    grid_.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    grid_.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;

                    grid_.ReadOnly = true;
                    grid_.AllowUserToResizeColumns = false;
                    grid_.ColumnHeadersHeight = (int)(PRM.GRID_ROW_H * 1.6);
                    grid_.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                    // grid_.Enabled = false;
                    grid_.RowPrePaint += this.gridRowPrePaint;
                    grid_.CellPainting += gridCellPrePainting;
                    grid_.CellFormatting += this.gridCellFormatting;
                    grid_.CellMouseClick += this.gridCellMouseClick;

                    grid_.KeyDown += grid__KeyDown;
                    grid_.KeyPress += grid__KeyPress;
                    TOOLSGRID.SETSTYLE(grid_);

                    grid_.BackgroundColor = this.BackColor;
                    grid_.Dock = DockStyle.Fill;
                }


                protected void loadBoxes(DataTable pTab)
                {
                    var ctxt = getBoxes().Controls;

                    List<Button> list = new List<Button>();
                    foreach (DataRow r in pTab.Rows)
                    {
                        var b = createSingleBox(r);

                        list.Add(b);
                    }

                    ctxt.AddRange(list.ToArray());



                }



                protected virtual void formatBox(Button pBox)
                {

                    var b = pBox as ButtonExt;
                    if (b == null)
                        return;

                    var row = b.targetObject as DataRow;
                    if (row == null)
                        return;


                    DataColumn c = null;
                    var res = "";
                    foreach (var s in new string[] { "NAME", "DEFINITION_" })
                    {
                        c = row.Table.Columns[s];
                        if (c != null)
                        {
                            res = row[c].ToString();
                            break;
                        }
                    }

                    b.Text = res;

                }

                protected Button createSingleBox(DataRow pRow)
                {
                    var r = getButton("", "select") as ButtonExt;


                    r.Dock = DockStyle.None;
                    r.targetObject = pRow;
                    //  r.Width *= 2;

                    r.Width = (int)(Screen.PrimaryScreen.WorkingArea.Width / 5 - 10);
                    r.Height = (int)(r.Height * 0.7);
                    r.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
                    r.AutoSize = true;
                    r.Anchor = AnchorStyles.None;
                    r.Margin = Padding.Empty;
                    r.Padding = Padding.Empty;

                    formatBox(r);

                    return r;
                }

                private void grid__KeyPress(object sender, KeyPressEventArgs e)
                {
                    if (char.IsLetterOrDigit(e.KeyChar) || char.IsPunctuation(e.KeyChar) || char.IsWhiteSpace(e.KeyChar))
                    {
                        if (e.KeyChar.ToString().Trim() != string.Empty || (e.KeyChar == ' '))
                        {
                            searchedText += e.KeyChar;
                            searchData(searchedText);
                        }
                    }
                }

                void grid__KeyDown(object sender, KeyEventArgs e)
                {
                    if (!e.Shift && !e.Control && !e.Alt)
                    {
                        if (e.KeyCode == Keys.Back)
                        {
                            if (searchedText.Length > 0)
                            {
                                e.Handled = true;
                                searchedText = string.Empty;
                                searchData(searchedText);
                            }
                        }
                    }
                }

                protected virtual void gridCellPrePainting(object sender, DataGridViewCellPaintingEventArgs e)
                {

                }

                protected virtual void gridError(object sender, DataGridViewDataErrorEventArgs e)
                {

                }

                protected virtual void gridRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
                {
                    if (stripeRowBackColor)
                    {
                        DataGridView grid_ = sender as DataGridView;
                        if (grid_ == null)
                            return;

                        DataGridViewRow row_ = TOOLSGRID.MY_GET_GRID_ROW(grid_, e.RowIndex);
                        if (row_ == null)
                            return;


                        // row_.DefaultCellStyle.SelectionBackColor = 
                        Color c = (e.RowIndex % 2 == 0) ? this.BackColor : stripeColor;
                        if (row_.DefaultCellStyle.BackColor != c)
                            row_.DefaultCellStyle.BackColor = c;

                    }
                }
                protected virtual void gridCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
                {


                    DataGridView grid_ = sender as DataGridView;

                    if (grid_ == null)
                        return;

                    if (e.ColumnIndex >= 0 && grid_.Columns[e.ColumnIndex].DataPropertyName == "INDX")
                    {
                        e.Value = _PLUGIN.FORMAT(e.RowIndex + 1);
                    }

                }
                protected virtual void gridCellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
                {
                    DataGridView grid_ = sender as DataGridView;
                    if (grid_ == null)
                        return;

                    if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                    {
                        DataRow row_ = TOOLSGRID.MY_GET_GRID_ROW_DATA(grid_, e.RowIndex);
                        if (row_ != null)
                            activatedRecord(row_, grid_.Columns[e.ColumnIndex].DataPropertyName);

                    }
                }

                protected virtual void activatedRecord(DataRow pRow, string pClickedColumn)
                {
                    if (pRow == null)
                        return;

                    if (pClickedColumn == TABLE_DUMMY.COLS.INDX_COL)
                        return;

                    if (!getDataReference().isModeShow())
                    {
                        getDataReference().select(pRow);
                        if (getDataReference().isFinished())
                            finish();
                    }

                }
                public void select(object sender)
                {
                    if (formType == FormType.grid)
                        activatedRecord(TOOLSGRID.MY_GET_GRID_ROW_DATA(grid_), null);
                    else
                        if (formType == FormType.box)
                        {
                            var x = sender as ButtonExt;
                            if (x != null)
                            {
                                var r = x.targetObject as DataRow;
                                activatedRecord(r, null);
                                formatBox(x);
                            }
                        }
                }
                public void finish()
                {
                    Close();
                }
                public void cancel()
                {
                    getDataReference().clear();
                    finish();
                }
                public virtual void runCmd(string pCmd)
                {
                    switch (pCmd)
                    {
                        case "beg":
                            TOOLSGRID.MY_SET_GRID_POSITION(grid_, 0, null);
                            break;
                        case "next":
                            TOOLSGRID.MY_SET_GRID_POSITION_OFFSET(grid_, +1, null);
                            break;
                        case "prev":
                            TOOLSGRID.MY_SET_GRID_POSITION_OFFSET(grid_, -1, null);
                            break;
                        case "nextpage":
                            TOOLSGRID.MY_SET_GRID_POSITION_OFFSET(grid_, int.MaxValue, null);
                            break;
                        case "prevpage":
                            TOOLSGRID.MY_SET_GRID_POSITION_OFFSET(grid_, int.MinValue, null);
                            break;
                        case "end":
                            TOOLSGRID.MY_SET_GRID_POSITION(grid_, int.MaxValue, null);
                            break;
                    }
                }

                void initStruct()
                {


                    panelTop.Dock = DockStyle.Top;
                    panelTopFilter.Dock = DockStyle.Top;
                    panelContent.Dock = DockStyle.Fill;
                    panelBottom.Dock = DockStyle.Bottom;

                    panelTopFilter.Height = panelTop.Height = panelBottom.Height = PRM.CMD_BTN_H;
                    //
                    foreach (var b in getButtons())
                        panelTop.Controls.Add(b);

                    if (btnSelect)
                        panelTop.Controls.Add(getButton(_LANG.L.SELECT, "select"));
                    if (btnExit)
                        panelTop.Controls.Add(getButton(_LANG.L.CLOSE, "cancel"));


                    initPanelTopFilter(panelTopFilter);
                    //
                    if (formType == FormType.grid)
                        panelContent.Controls.Add(grid_);
                    else
                        panelContent.Controls.Add(boxes_);


                    List<Control> lPanels = new List<Control>();
                    lPanels.Add(panelContent);

                    if (panelTopFilter.HasChildren)
                        lPanels.Add(panelTopFilter);

                    lPanels.Add(panelTop);

                    if (formType == FormType.grid)
                    {
                        panelBottom.Controls.Add(getButton("|<<", "beg"));
                        panelBottom.Controls.Add(getButton("<<", "prevpage"));
                        panelBottom.Controls.Add(getButton("<", "prev"));
                        panelBottom.Controls.Add(getButton(">", "next"));
                        panelBottom.Controls.Add(getButton(">>", "nextpage"));
                        panelBottom.Controls.Add(getButton(">>|", "end"));

                        lPanels.Add(panelBottom);
                    }



                    Controls.AddRange(lPanels.ToArray());
                }





                protected virtual Button[] getButtons()
                {
                    return new Button[] { };

                }

                protected virtual void initPanelTopFilter(Panel pPanelTopFilter)
                {

                }



                public virtual void SORT(string pCol, bool pAsc)
                {
                    sortColumn = pCol;
                    sortAsc = pAsc;
                }

                void SORT()
                {

                }

                public virtual void SEARCH(string pCol, object pValue)
                {
                    searchColumn = pCol;
                    searchValue = pValue;
                }

                void SEARCH()
                {
                    if (formType != FormType.grid)
                        return;

                    if (_PLUGIN.ISNULL(searchColumn, "") as string == "")
                        return;

                    if (_PLUGIN.ISNULL(searchValue))
                        return;

                    TOOLSGRID.MY_SET_GRID_POSITION_SEARCH(getGrid(), searchColumn, searchValue);
                }

                protected override void Dispose(bool disposing)
                {
                    base.Dispose(disposing);

                    grid_ = null;

                    panelTop = null;
                    panelContent = null;
                    panelBottom = null;


                }



                protected static Button getButton(string pText, string pCmd)
                {
                    ButtonExt b = new ButtonExt();
                    b.Text = pText;
                    b.cmd = pCmd;
                    b.Dock = DockStyle.Right;
                    b.Width = (int)(PRM.CMD_BTN_H * 1.2);
                    b.Height = PRM.CMD_BTN_H;
                    return b;
                }

                public class ButtonExt : Button
                {
                    public string cmd;
                    public object targetObject;

                    protected override void OnClick(EventArgs e)
                    {
                        base.OnClick(e);



                        FormReference f = this.FindForm() as FormReference;
                        if (f != null)
                            invokeCmd(f);
                    }

                    void invokeCmd(FormReference pForm)
                    {
                        if (pForm == null)
                            return;

                        switch (cmd)
                        {
                            case "select":
                                pForm.select(this);
                                break;
                            case "finish":
                                pForm.finish();
                                break;
                            case "cancel":
                                pForm.cancel();
                                break;
                            default:
                                pForm.runCmd(cmd);
                                break;
                        }
                    }
                }

            }


            //only dialog mode
            abstract class DataReference : IDisposable
            {
                protected bool modeMultiSelect = false; //select more than one
                protected bool modeShow = false; //just show no select

                protected _PLUGIN PLUGIN;
                List<DataRow> result_ = new List<DataRow>();
                //

                protected string sortColumn;
                protected bool sortAsc;

                protected string searchColumn;
                protected object searchValue;
                //

                //
                public DataReference(_PLUGIN pPLUGIN)
                {
                    //
                    PLUGIN = pPLUGIN;
                }

                public void setModeShow(bool pVal)
                {
                    modeShow = pVal;
                }
                public void setModeMultiSelect(bool pVal)
                {
                    modeMultiSelect = pVal;
                }

                public bool isModeShow()
                {
                    return modeShow;
                }
                public bool isModeMultiSelect()
                {
                    return modeMultiSelect;
                }
                public virtual void SORT(string pCol, bool pAsc)
                {

                    sortColumn = pCol;
                    sortAsc = pAsc;
                }

                public virtual void SEARCH(string pCol, object pValue)
                {
                    searchColumn = pCol;
                    searchValue = pValue;
                }

                public virtual DataRow[] REF()
                {
                    result_.Clear();

                    interactUser();

                    return result_.ToArray();
                }

                //REF sub function
                protected virtual void interactUser()
                {

                }

                //select record
                public void select(DataRow pRecord)
                {
                    if (pRecord != null)
                        result_.Add(pRecord);
                }

                //is fully loaded
                public bool isFinished()
                {
                    return !modeMultiSelect && result_.Count > 0;
                }

                //delete selected
                public void clear()
                {
                    result_.Clear();
                }
                public virtual void Dispose()
                {
                    PLUGIN = null;

                }

                protected virtual void initForm(FormReference pForm)
                {
                    if (pForm == null)
                        return;

                    pForm.SORT(sortColumn, sortAsc);
                    pForm.SEARCH(searchColumn, searchValue);

                }
                public void exceptionHandler(Exception exc)
                {
                    PLUGIN.LOG(exc.ToString());
                    PLUGIN.MSGUSERERROR(exc.Message);
                }

            }




            class SalesManReference : DataReference
            {

                public enum Filter
                {
                    All,
                    CurrentClientAndNoClient,
                    CurrentClient,
                    NoClient,
                }

                Filter filter = Filter.All;

                PosTerminal posTerminal;
                //
                public SalesManReference(_PLUGIN pPLUGIN, PosTerminal pPosTerminal, Filter pFilter)
                    : base(pPLUGIN)
                {

                    posTerminal = pPosTerminal;

                    filter = pFilter;
                }


                public static object getCurrentSlsManClient(_PLUGIN PLUGIN, object pSlsManRef)
                {
                    var sql_ =
                        MY_CHOOSE_SQL(
@"
 
                    SELECT TOP(1) CLIENTREF  
                    FROM LG_$FIRM$_$PERIOD$_INVOICE I WITH(NOLOCK) 
                            WHERE 
                            I.SALESMANREF = @P1 AND I.CYPHCODE = @P2 AND I.CANCELLED=0 AND I.TRCODE IN (8)
                            ORDER BY
                            I.DATE_ DESC,I.TIME_ DESC,I.TRCODE DESC,I.LOGICALREF DESC
", @"
                    SELECT CLIENTREF  
                    FROM LG_$FIRM$_$PERIOD$_INVOICE I 
                            WHERE 
                            I.SALESMANREF = @P1 AND I.CYPHCODE = @P2 AND I.CANCELLED=0 AND I.TRCODE IN (8)
                            ORDER BY
                            I.DATE_ DESC,I.TIME_ DESC,I.TRCODE DESC,I.LOGICALREF DESC
                    LIMIT 1
 
                                ");


                    return ISNULL(PLUGIN.SQLSCALAR(sql_, new object[]{
                        pSlsManRef,PRM.CYPHCODE
                    }), 0);

                }


                DataTable getRecords()
                {



                    string sql_ = @"
 


                            SELECT 
                            ( SELECT TOP(1) (SELECT TOP(1) DEFINITION_ FROM LG_$FIRM$_CLCARD WHERE LOGICALREF = I.CLIENTREF) + ' ' + I.GENEXP1 DEFINITION_ FROM LG_$FIRM$_$PERIOD$_INVOICE I WITH(NOLOCK) WHERE I.LOGICALREF = T.INVLOGICALREF) CLCARD_DEFINITION_,
                            ( SELECT TOP(1) I.CLIENTREF FROM LG_$FIRM$_$PERIOD$_INVOICE I WITH(NOLOCK) WHERE I.LOGICALREF = T.INVLOGICALREF) CLCARD_LOGICALREF,
                           
                            * 
                            FROM

(
                            
                            SELECT TOP(150) 
                            LOGICALREF,CODE,DEFINITION_,SPECODE,
                            (
                            SELECT TOP(1) I.LOGICALREF FROM LG_$FIRM$_$PERIOD$_INVOICE I WITH(NOLOCK,INDEX=I$FIRM$_$PERIOD$_INVOICE_I5) 
                            WHERE 
                            I.SALESMANREF = M.LOGICALREF AND I.CYPHCODE = @P3 AND I.CANCELLED=0 AND I.TRCODE IN (8)
                            ORDER BY
                            I.DATE_ DESC,I.TIME_ DESC,I.TRCODE DESC,I.LOGICALREF DESC
                            ) INVLOGICALREF
                            FROM 
                            LG_SLSMAN M WITH(NOLOCK) 
                            WHERE
                            M.FIRMNR = @P1 AND
                            M.ACTIVE = @P2
                            ORDER BY
                            M.FIRMNR ASC,
                            M.CODE ASC

) T


                            ";

                    DataTable tab_ = PLUGIN.SQL(sql_,
                             new object[] { PLUGIN.GETSYSPRM_FIRM(), 0, PRM.CYPHCODE });


                    _PLUGIN.TAB_FILLNULL(tab_);



                    if (filter != Filter.All)
                    {
                        for (int i = 0; i < tab_.Rows.Count; ++i)
                        {
                            var row_ = tab_.Rows[i];
                            var recClref_ = TAB_GETROW(row_, TABLE_SLSMAN.COLS.CLCARD_LOGICALREF);
                            var currClref_ = ISNULL(posTerminal.registers.clcard.getClientRef(), 0);

                            bool rem = true;

                            if (filter == Filter.CurrentClient)
                            {
                                if (COMPARE(recClref_, currClref_) && !ISEMPTYLREF(currClref_))
                                    rem = false;
                            }
                            else
                                if (filter == Filter.CurrentClientAndNoClient)
                                {

                                    if (COMPARE(recClref_, currClref_) && !ISEMPTYLREF(currClref_))
                                        rem = false;

                                    if (!ISEMPTYLREF(currClref_) && ISEMPTYLREF(recClref_))
                                        rem = false;
                                }
                                else
                                    if (filter == Filter.NoClient)
                                    {
                                        if (ISEMPTYLREF(recClref_))
                                            rem = false;

                                    }


                            if (rem)
                            {
                                tab_.Rows.Remove(row_);
                                --i;
                            }
                        }


                    }


                    return tab_;

                }

                protected override void interactUser()
                {
                    Form f = null;

                    try
                    {
                        f = getForm();
                        f.ShowDialog();
                    }
                    finally
                    {
                        if (f != null) f.Dispose();
                    }

                }







                Form getForm()
                {
                    FormReference form = new ItemsForm(this);
                    initForm(form);
                    return form;
                }



                class ItemsForm : FormReference
                {

                    SalesManReference reference_;

                    protected override DataReference getDataReference()
                    {
                        return reference_;
                    }

                    public ItemsForm(SalesManReference pReference)
                    {
                        reference_ = pReference;
                        Text = PRM.TERM_TYPE == TERMTYPE.restoran ? _LANG.L.TABLE : _LANG.L.SLSPLACE;
                    }

                    protected override string filterColumn()
                    {
                        return TABLE_SLSMAN.COLS.DEFINITION_;
                    }


                    protected override void initGrid()
                    {
                        base.initGrid();

                        DataGridView grid_ = getGrid();

                        string[] arrHeaderText = new string[]{
                            _LANG.L.CODE,  
                            _LANG.L.NAME,  
                            ""
                        };

                        string[] arrDataPropertyName = new string[]{
                            TABLE_SLSMAN.COLS.CODE ,
                            TABLE_SLSMAN.COLS.DEFINITION_, 
                             TABLE_SLSMAN.COLS.CLCARD_DEFINITION_
                        };



                        //    };
                        DataGridViewContentAlignment[] arrAlignment = new DataGridViewContentAlignment[]{
                            DataGridViewContentAlignment.MiddleLeft,
                            DataGridViewContentAlignment.MiddleLeft,  
                            DataGridViewContentAlignment.MiddleLeft,
                        };
                        string[] arrFormat = new string[]{
                            "",
                            "", //1 
                            ""
                        };
                        for (int colIndx_ = 0; colIndx_ < arrHeaderText.Length; ++colIndx_)
                        {
                            DataGridViewTextBoxColumn column_ = new DataGridViewTextBoxColumn();
                            column_.DataPropertyName = arrDataPropertyName[colIndx_];
                            column_.HeaderText = arrHeaderText[colIndx_];
                            //  column_.Width = arrWidth[colIndx_];
                            column_.DefaultCellStyle.Alignment = arrAlignment[colIndx_];
                            column_.DefaultCellStyle.Format = arrFormat[colIndx_];

                            column_.SortMode = arrDataPropertyName[colIndx_] == TABLE_DUMMY.COLS.INDX_COL ? DataGridViewColumnSortMode.NotSortable : DataGridViewColumnSortMode.Automatic;


                            grid_.Columns.Add(column_);

                            if (arrDataPropertyName[colIndx_] == TABLE_SPECODES.COLS.DEFINITION_)
                                column_.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;




                        }


                        reloadData();

                    }

                    public void reloadData()
                    {


                        var grid_ = getGrid();

                        DataTable tableRecords = reference_.getRecords();

                        _PLUGIN.TAB_ADDCOL(tableRecords, TABLE_DUMMY.COLS.INDX_COL, TABLE_DUMMY.TYPES.INDX_COL_TYPE);
                        _PLUGIN.TAB_SETCOL(tableRecords, TABLE_DUMMY.COLS.INDX_COL, 0);


                        grid_.DataSource = tableRecords;
                    }

                    protected override Button[] getButtons()
                    {
                        if (PRM.TERM_TYPE == TERMTYPE.restoran)
                            return new Button[] { getButton("" + _LANG.L.MSG_DO_FREE_PLACE + "", "slsmanclean") };

                        return base.getButtons();
                    }

                    public override void runCmd(string pCmd)
                    {
                        switch (pCmd)
                        {
                            case "slsmanclean":
                                {
                                    var grid_ = getGrid();

                                    var row_ = TOOLSGRID.MY_GET_GRID_ROW_DATA(grid_);

                                    if (row_ == null)
                                        return;

                                    var slsRef_ = TAB_GETROW(row_, TABLE_SLSMAN.COLS.LOGICALREF);
                                    var desc_ = TAB_GETROW(row_, TABLE_SLSMAN.COLS.DEFINITION_).ToString();

                                    if (reference_.posTerminal.SLSMANCLEAN(slsRef_, desc_))
                                        reloadData();

                                }
                                break;

                        }

                        base.runCmd(pCmd);
                    }


                    protected override void gridCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
                    {
                        base.gridCellFormatting(sender, e);



                    }

                    protected override void gridCellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
                    {
                        base.gridCellMouseClick(sender, e);
                    }



                    protected override void gridRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
                    {
                        base.gridRowPrePaint(sender, e);


                        DataGridView grid_ = sender as DataGridView;
                        if (grid_ == null)
                            return;



                        DataGridViewRow row_ = TOOLSGRID.MY_GET_GRID_ROW(grid_, e.RowIndex);
                        if (row_ == null)
                            return;

                        var rowData_ = TOOLSGRID.MY_GET_GRID_ROW_DATA(row_);

                        string clDesc_ = TAB_GETROW(rowData_, TABLE_SLSMAN.COLS.CLCARD_DEFINITION_).ToString();

                        if (clDesc_ != "")
                        {
                            TOOLSGRID.MY_SETSTYLECOLORTEXT(row_.DefaultCellStyle, Color.Green);

                        }


                    }

                    protected override void gridError(object sender, DataGridViewDataErrorEventArgs e)
                    {
                        DataGridView grid_ = sender as DataGridView;
                        if (grid_ == null)
                            return;

                        base.gridError(sender, e);
                    }




                    protected override void activatedRecord(DataRow pRow, string pClickedColumn)
                    {

                        switch (pClickedColumn)
                        {
                            case TABLE_SLSMAN.COLS.DEFINITION_:
                                base.activatedRecord(pRow, pClickedColumn);
                                break;


                        }

                    }





                    protected override void Dispose(bool disposing)
                    {
                        base.Dispose(disposing);

                        reference_ = null;
                    }





                }




                public override void Dispose()
                {
                    base.Dispose();

                    posTerminal = null;
                }





            }


            class ListReference : DataReference
            {
                DataTable tab;
                PosTerminal posTerminal;
                //


                public ListReference(_PLUGIN pPLUGIN, PosTerminal pPosTerminal, object[] pCode, string[] pDesc)
                    : base(pPLUGIN)
                {

                    posTerminal = pPosTerminal;
                    //
                    tab = new DataTable();
                    tab.Columns.Add(TABLE_LIST.COLS.ID_, TABLE_LIST.TYPES.ID_);
                    tab.Columns.Add(TABLE_LIST.COLS.DEFINITION_, TABLE_LIST.TYPES.DEFINITION_);


                    for (int i = 0; i < Math.Min(pCode.Length, pDesc.Length); ++i)
                        tab.Rows.Add(pCode[i], pDesc[i]);

                }





                DataTable getRecords()
                {



                    return tab;

                }

                protected override void interactUser()
                {
                    Form f = null;

                    try
                    {
                        f = getForm();
                        f.ShowDialog();
                    }
                    finally
                    {
                        if (f != null) f.Dispose();
                    }

                }







                Form getForm()
                {
                    FormReference form = new ItemsForm(this);
                    initForm(form);
                    return form;
                }



                class ItemsForm : FormReference
                {

                    ListReference reference_;

                    protected override DataReference getDataReference()
                    {
                        return reference_;
                    }

                    public ItemsForm(ListReference pReference)
                    {
                        reference_ = pReference;

                        Text = _LANG.L.TAGS;
                    }
                    protected override void initGrid()
                    {
                        base.initGrid();

                        DataGridView grid_ = getGrid();

                        string[] arrHeaderText = new string[]{
                            _LANG.L.TAG
                        };

                        string[] arrDataPropertyName = new string[]{
                            TABLE_LIST.COLS.DEFINITION_

                        };



                        //    };
                        DataGridViewContentAlignment[] arrAlignment = new DataGridViewContentAlignment[]{
                            DataGridViewContentAlignment.MiddleLeft
                        };
                        string[] arrFormat = new string[]{
                            "",
                        };
                        for (int colIndx_ = 0; colIndx_ < arrHeaderText.Length; ++colIndx_)
                        {
                            DataGridViewTextBoxColumn column_ = new DataGridViewTextBoxColumn();
                            column_.DataPropertyName = arrDataPropertyName[colIndx_];
                            column_.HeaderText = arrHeaderText[colIndx_];
                            //  column_.Width = arrWidth[colIndx_];
                            column_.DefaultCellStyle.Alignment = arrAlignment[colIndx_];
                            column_.DefaultCellStyle.Format = arrFormat[colIndx_];

                            column_.SortMode = arrDataPropertyName[colIndx_] == TABLE_DUMMY.COLS.INDX_COL ? DataGridViewColumnSortMode.NotSortable : DataGridViewColumnSortMode.Automatic;


                            grid_.Columns.Add(column_);

                            if (arrDataPropertyName[colIndx_] == TABLE_LIST.COLS.DEFINITION_)
                                column_.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }



                        DataTable tableRecords = reference_.getRecords();

                        _PLUGIN.TAB_ADDCOL(tableRecords, TABLE_DUMMY.COLS.INDX_COL, TABLE_DUMMY.TYPES.INDX_COL_TYPE);
                        _PLUGIN.TAB_SETCOL(tableRecords, TABLE_DUMMY.COLS.INDX_COL, 0);


                        grid_.DataSource = tableRecords;
                    }

                    protected override void gridCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
                    {
                        base.gridCellFormatting(sender, e);


                    }

                    protected override void gridCellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
                    {
                        base.gridCellMouseClick(sender, e);



                    }



                    protected override void gridRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
                    {
                        base.gridRowPrePaint(sender, e);

                    }

                    protected override void gridError(object sender, DataGridViewDataErrorEventArgs e)
                    {
                        DataGridView grid_ = sender as DataGridView;
                        if (grid_ == null)
                            return;

                        base.gridError(sender, e);
                    }




                    protected override void activatedRecord(DataRow pRow, string pClickedColumn)
                    {
                        base.activatedRecord(pRow, pClickedColumn);
                    }





                    protected override void Dispose(bool disposing)
                    {
                        base.Dispose(disposing);

                        reference_ = null;
                    }





                }




                public override void Dispose()
                {
                    base.Dispose();

                    posTerminal = null;
                }





            }



            class InvoiceSlsReference : DataReference
            {

                PosTerminal posTerminal;
                object clientCard;
                //
                public InvoiceSlsReference(_PLUGIN pPLUGIN, PosTerminal pPosTerminal, object pClientCard)
                    : this(pPLUGIN, pPosTerminal, pClientCard, false)
                {


                }

                public InvoiceSlsReference(_PLUGIN pPLUGIN, PosTerminal pPosTerminal, object pClientCard, bool pShowClosed)
                    : base(pPLUGIN)
                {
                    clientCard = ISEMPTYLREF(pClientCard) ? 0 : pClientCard;
                    posTerminal = pPosTerminal;
                }





                DataTable getRecords()
                {

                    var sql1_ = @"SELECT TOP(15) 
                            I.LOGICALREF,I.FICHENO,I.DATE_,I.TIME_,I.DOCODE,I.SPECODE,I.NETTOTAL,I.GENEXP1,I.GENEXP2,I.GENEXP3,I.GENEXP4,I.DOCTRACKINGNR,I.TRCODE,
                            (SELECT TOP(1) CODE FROM LG_SLSMAN M WITH(NOLOCK) WHERE M.LOGICALREF = I.SALESMANREF AND M.FIRMNR = $FIRM$) SLSMAN_CODE,
                            (SELECT TOP(1) DEFINITION_ FROM LG_$FIRM$_CLCARD M WITH(NOLOCK) WHERE M.LOGICALREF = I.CLIENTREF) CLCARD_DEFINITION_
                            FROM 
                            LG_$FIRM$_$PERIOD$_INVOICE I WITH(NOLOCK,INDEX=I$FIRM$_$PERIOD$_INVOICE_I9)
                            WHERE
                            (GRPCODE = 2 AND (@P1= 0 OR CLIENTREF = @P1)) AND (CYPHCODE = @P2 AND CANCELLED = 0 AND TRCODE IN (8))
                            ORDER BY GRPCODE DESC,CLIENTREF DESC,DATE_ DESC,TIME_ DESC,LOGICALREF DESC";

                    var sql1args_ = new object[] { clientCard, PRM.CYPHCODE };

                    //                    var sql2_ = @"SELECT TOP(15) 
                    //                            I.LOGICALREF,I.FICHENO,I.DATE_,I.TIME_,I.DOCODE,I.SPECODE,I.NETTOTAL,I.GENEXP1,I.GENEXP2,I.GENEXP3,I.GENEXP4,I.DOCTRACKINGNR,I.TRCODE,
                    //                            (SELECT TOP(1) CODE FROM LG_SLSMAN M WITH(NOLOCK) WHERE M.LOGICALREF = I.SALESMANREF AND M.FIRMNR = $FIRM$) SLSMAN_CODE
                    //                            FROM 
                    //                            LG_$FIRM$_$PERIOD$_INVOICE I WITH(NOLOCK)
                    //                            WHERE
                    //                            (GRPCODE = 2 AND (@P1 =0 OR SALESMANREF = @P1)) AND (CYPHCODE = @P2 AND CANCELLED = 0 AND TRCODE IN (8))
                    //                            ORDER BY SALESMANREF DESC,DATE_ DESC,TIME_ DESC,LOGICALREF DESC";

                    //                    var sql2args_ = new object[] { slsMan, PRM.CYPHCODE };


                    DataTable tab_ = PLUGIN.SQL(sql1_,
                               sql1args_);

                    _PLUGIN.TAB_ADDCOL(tab_, TABLE_DUMMY.COLS.INDX_COL, TABLE_DUMMY.TYPES.INDX_COL_TYPE);
                    _PLUGIN.TAB_SETCOL(tab_, TABLE_DUMMY.COLS.INDX_COL, 0);

                    _PLUGIN.TAB_ADDCOL(tab_, TABLE_INVOICE.COLS.DUMMY_DATE, typeof(DateTime));

                    foreach (DataRow row in tab_.Rows)
                    {

                        TAB_SETROW(row, TABLE_INVOICE.COLS.DUMMY_DATE, MY_CONVERTDATE(CASTASDATE(TAB_GETROW(row, TABLE_INVOICE.COLS.DATE_)), CASTASINT(TAB_GETROW(row, TABLE_INVOICE.COLS.TIME_))));


                    }


                    _PLUGIN.TAB_FILLNULL(tab_);
                    return tab_;
                }

                protected override void interactUser()
                {
                    Form f = null;

                    try
                    {
                        f = getForm();
                        f.ShowDialog();
                    }
                    finally
                    {
                        if (f != null) f.Dispose();
                    }

                }







                Form getForm()
                {
                    FormReference form = new ItemsForm(this);
                    initForm(form);
                    return form;
                }



                class ItemsForm : FormReference
                {

                    InvoiceSlsReference reference_;

                    protected override DataReference getDataReference()
                    {
                        return reference_;
                    }

                    public ItemsForm(InvoiceSlsReference pReference)
                    {
                        reference_ = pReference;

                        Text = _LANG.L.SALES;
                    }
                    protected override void initGrid()
                    {
                        base.initGrid();

                        DataGridView grid_ = getGrid();

                        string[] arrHeaderText = new string[]{
                            _LANG.L.CODE,  
                          //  _LANG.L.DATE+ "",
                          (PRM.TERM_TYPE == TERMTYPE.restoran ?_LANG.L.TABLE:_LANG.L.SALER),
                            _LANG.L.TOTAL,
                            _LANG.L.TEL,
                            _LANG.L.CARD,
                            ""
                        };

                        string[] arrDataPropertyName = new string[]{
                            TABLE_INVOICE.COLS.FICHENO ,
                           // TABLE_INVOICE.COLS.DUMMY_DATE, 
                           TABLE_INVOICE.COLS.SLSMAN_CODE,
                            TABLE_INVOICE.COLS.NETTOTAL,
                            TABLE_INVOICE.COLS.DOCTRACKINGNR,
                             TABLE_INVOICE.COLS.CLCARD_DEFINITION_,
                            TABLE_INVOICE.COLS.GENEXP1
                        };



                        //    };
                        DataGridViewContentAlignment[] arrAlignment = new DataGridViewContentAlignment[]{
                            DataGridViewContentAlignment.MiddleLeft,
                          //  DataGridViewContentAlignment.MiddleLeft, 
                          DataGridViewContentAlignment.MiddleLeft,
                            DataGridViewContentAlignment.MiddleRight,
                            DataGridViewContentAlignment.MiddleLeft,
                            DataGridViewContentAlignment.MiddleLeft,
                             DataGridViewContentAlignment.MiddleLeft,
                        };
                        string[] arrFormat = new string[]{
                            "",
                          //  "HH:mm", //1 //HH:mm (MM/dd)
                            "",
                            "N2",
                            "",
                             "",
                            ""
                        };
                        for (int colIndx_ = 0; colIndx_ < arrHeaderText.Length; ++colIndx_)
                        {
                            DataGridViewTextBoxColumn column_ = new DataGridViewTextBoxColumn();
                            column_.DataPropertyName = arrDataPropertyName[colIndx_];
                            column_.HeaderText = arrHeaderText[colIndx_];
                            //  column_.Width = arrWidth[colIndx_];
                            column_.DefaultCellStyle.Alignment = arrAlignment[colIndx_];
                            column_.DefaultCellStyle.Format = arrFormat[colIndx_];

                            column_.SortMode = arrDataPropertyName[colIndx_] == TABLE_DUMMY.COLS.INDX_COL ? DataGridViewColumnSortMode.NotSortable : DataGridViewColumnSortMode.Automatic;

                            grid_.Columns.Add(column_);

                            if (arrDataPropertyName[colIndx_] == TABLE_INVOICE.COLS.GENEXP1)
                                column_.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }



                        DataTable tableRecords = reference_.getRecords();




                        grid_.DataSource = tableRecords;
                    }

                    protected override void gridCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
                    {
                        base.gridCellFormatting(sender, e);


                    }

                    protected override void gridCellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
                    {
                        base.gridCellMouseClick(sender, e);



                    }



                    protected override void gridRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
                    {
                        base.gridRowPrePaint(sender, e);

                    }

                    protected override void gridError(object sender, DataGridViewDataErrorEventArgs e)
                    {
                        DataGridView grid_ = sender as DataGridView;
                        if (grid_ == null)
                            return;

                        base.gridError(sender, e);
                    }




                    protected override void activatedRecord(DataRow pRow, string pClickedColumn)
                    {
                        base.activatedRecord(pRow, pClickedColumn);

                        if (pClickedColumn == TABLE_INVOICE.COLS.GENEXP1)
                            return;

                        if (getDataReference().isModeShow())
                        {


                            object lref_ = TAB_GETROW(pRow, TABLE_INVOICE.COLS.LOGICALREF);


                            if (!ISEMPTYLREF(lref_))
                            {
                                if (TABLE_INVOICE.TOOLS.getObjectOpenedForm(lref_) != null)
                                {
                                    reference_.PLUGIN.MSGUSERINFO(_LANG.L.MSG_INFO_OPENED);
                                }
                                else
                                {
                                    this.reference_.posTerminal.COPY().BEGIN_TERMINAL(lref_);
                                    finish();
                                }
                            }
                        }
                    }



                    protected override void Dispose(bool disposing)
                    {
                        base.Dispose(disposing);

                        reference_ = null;
                    }



                    protected override Button[] getButtons()
                    {
                        if (PRM.TERM_TYPE == TERMTYPE.restoran)
                            return new Button[] { getButton("Cap et", "invprint") };

                        return base.getButtons();
                    }



                    public override void runCmd(string pCmd)
                    {
                        switch (pCmd)
                        {
                            case "invprint":
                                {
                                    var grid_ = getGrid();

                                    var row_ = TOOLSGRID.MY_GET_GRID_ROW_DATA(grid_);

                                    if (row_ == null)
                                        return;

                                    var lref_ = TAB_GETROW(row_, TABLE_INVOICE.COLS.LOGICALREF);

                                    if (reference_.posTerminal != null && !ISEMPTYLREF(lref_))
                                        reference_.posTerminal._PRINTLAST(false, lref_);

                                }
                                break;

                        }

                        base.runCmd(pCmd);
                    }

                }





                public override void Dispose()
                {
                    base.Dispose();

                    posTerminal = null;
                }





            }


            class MatReference : DataReference
            {

                PosTerminal posTerminal;
                //
                public MatReference(_PLUGIN pPLUGIN, PosTerminal pPosTerminal)
                    : base(pPLUGIN)
                {

                    posTerminal = pPosTerminal;
                }



                void setAmount(DataRow pRefRecord)
                {
                    if (pRefRecord == null)
                        return;




                    object mref = _PLUGIN.TAB_GETROW(pRefRecord, TABLE_STLINE.COLS.STOCKREF);
                    object uref = _PLUGIN.TAB_GETROW(pRefRecord, TABLE_STLINE.COLS.UOMREF);
                    double price = _PLUGIN.CASTASDOUBLE(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pRefRecord, TABLE_STLINE.COLS.PRICE), 0));
                    if (price < 0.01)
                        if (!PRM.USE_IF_NO_SLS_PRICE)
                        {
                            PLUGIN.MSGUSERINFO(_LANG.L.MSG_ERROR_NO_PRICE);
                            return;
                        }

                    double quantity = _PLUGIN.CASTASDOUBLE(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pRefRecord, TABLE_STLINE.COLS.AMOUNT), 0));

                    string mcode = _PLUGIN.CASTASSTRING(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pRefRecord, TABLE_STLINE.COLS.ITEMS_CODE), ""));
                    string mdesc = _PLUGIN.CASTASSTRING(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pRefRecord, TABLE_STLINE.COLS.ITEMS_NAME), ""));

                    string ucode = _PLUGIN.CASTASSTRING(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pRefRecord, TABLE_STLINE.COLS.UNITSETL_CODE), ""));

                    if (quantity == 0)
                        quantity = 1;

                    quantity = posTerminal.askNumber("T_QUANTITY, " + mdesc, quantity, (PRM.TERM_TYPE == TERMTYPE.restoran ? 0 : 2));

                    if (quantity < 0)
                        return;

                    _PLUGIN.TAB_SETROW(pRefRecord, TABLE_STLINE.COLS.AMOUNT, quantity);
                    ////////////////////////////




                    ////////////////////////////

                    posTerminal.setAmount(mref, mcode, mdesc, uref, ucode, price, quantity);

                }


                DataTable getGroups()
                {
                    return PLUGIN.SQL(
                        MY_CHOOSE_SQL(
@"
                            SELECT TOP(400) 
                            SPECODE,DEFINITION_,TEXTF1,SPECODE2
                            FROM 
                            LG_$FIRM$_SPECODES WITH(NOLOCK) 
                            WHERE
                            CODETYPE = @P1 AND
                            SPECODETYPE = @P2
                            ORDER BY
                            CODETYPE ASC,
                            SPECODETYPE ASC,
                            SPECODE ASC,
                            LOGICALREF ASC",
@"
                            SELECT 
                            SPECODE,DEFINITION_,TEXTF1,SPECODE2
                            FROM 
                            LG_$FIRM$_SPECODES 
                            WHERE
                            CODETYPE = @P1 AND
                            SPECODETYPE = @P2
                            ORDER BY
                            CODETYPE ASC,
                            SPECODETYPE ASC,
                            SPECODE ASC,
                            LOGICALREF ASC
                            LIMIT 400
"),
                             new object[] { 4, 0 });

                }



                DataTable getMatItems(string pGroup)
                {

                    object payPlan1_ = 0;
                    object payPlan2_ = posTerminal.registers.clcard.getPaymentRef();

                    return getMats(PLUGIN, pGroup, null, null, 1, 0, payPlan1_, payPlan2_, PRM.PRICE_TYPE);

                }





                public static DataTable getMats(_PLUGIN PLUGIN, string pGroup, string pSpeCode, string pSpeCode2, short pCardType, short pActive, object pPayPlan1, object pPayPlan2, int pPType)
                {

                    pGroup = ISNULL(pGroup, "%").ToString();
                    pSpeCode = ISNULL(pSpeCode, "%").ToString();
                    pSpeCode2 = ISNULL(pSpeCode2, "%").ToString();

                    var sql_ =
                        MY_CHOOSE_SQL(
                           @"SELECT TOP(400) 
                            LOGICALREF,CODE,NAME,SPECODE,SPECODE2,TEXTF1,INTF1,
                            {0} PRCLIST_PRICE,
                            (select top(1) U.LOGICALREF from LG_$FIRM$_UNITSETL U with(nolock) where U.UNITSETREF = I.UNITSETREF AND MAINUNIT = 1) UNITSETL_LOGICALREF,
                            (select top(1)       U.CODE from LG_$FIRM$_UNITSETL U with(nolock) where U.UNITSETREF = I.UNITSETREF AND MAINUNIT = 1) UNITSETL_CODE
                            FROM 
                            LG_$FIRM$_ITEMS I WITH(NOLOCK)
                            WHERE
                            STGRPCODE LIKE @P1 AND
                            SPECODE LIKE @P7 AND
                            SPECODE2 LIKE @P8 AND
                            CARDTYPE = @P2 AND
                            ACTIVE = @P3
                            ORDER BY
                            NAME ASC,
                            LOGICALREF ASC",
                           @"SELECT 
                            LOGICALREF,CODE,NAME,SPECODE,SPECODE2,TEXTF1,INTF1,
                            {0} PRCLIST_PRICE,
                            (select  U.LOGICALREF from LG_$FIRM$_UNITSETL U where U.UNITSETREF = I.UNITSETREF AND MAINUNIT = 1 LIMIT 1) UNITSETL_LOGICALREF,
                            (select U.CODE from LG_$FIRM$_UNITSETL U where U.UNITSETREF = I.UNITSETREF AND MAINUNIT = 1 LIMIT 1) UNITSETL_CODE
                            FROM 
                            LG_$FIRM$_ITEMS I 
                            WHERE
                            STGRPCODE LIKE @P1 AND
                            SPECODE LIKE @P7 AND
                            SPECODE2 LIKE @P8 AND
                            CARDTYPE = @P2 AND
                            ACTIVE = @P3
                            ORDER BY
                            NAME ASC,
                            LOGICALREF ASC
                            LIMIT 400
")


                           ;



                    var sqlPrice_ =
  MY_CHOOSE_SQL(
@"
( SELECT TOP(1) P.PRICE FROM LG_$FIRM$_PRCLIST P with(nolock) WHERE P.CARDREF = I.LOGICALREF AND P.PTYPE = @P6 AND P.PAYPLANREF IN (@P4,@P5) ORDER BY P.ENDDATE DESC,P.PAYPLANREF DESC)
",
 @"
( SELECT P.PRICE FROM LG_$FIRM$_PRCLIST P WHERE P.CARDREF = I.LOGICALREF AND P.PTYPE = @P6 AND P.PAYPLANREF IN (@P4,@P5) ORDER BY P.ENDDATE DESC,P.PAYPLANREF DESC LIMIT 1)
")

;


                    switch (pPType)
                    {
                        case 1:
                            sql_ = string.Format(sql_, sqlPrice_);
                            break;
                        case 2:
                            sql_ = string.Format(sql_, sqlPrice_);
                            break;
                    }

                    return PLUGIN.SQL(sql_,
         new object[] { pGroup, pCardType, pActive, pPayPlan1, pPayPlan2, pPType, pSpeCode, pSpeCode2 });


                }

                protected override void interactUser()
                {
                    Form f = null;

                    try
                    {
                        f = getGroupForm();
                        f.ShowDialog();
                    }
                    finally
                    {
                        if (f != null) f.Dispose();
                    }
                }

                public void REF(string pGroupCode)
                {
                    Form f = null;

                    try
                    {
                        f = getMatItemsForm(pGroupCode);
                        f.ShowDialog();
                    }
                    finally
                    {
                        if (f != null) f.Dispose();
                    }
                }




                Form getGroupForm()
                {

                    FormReference form = new MatGroupForm(this);
                    initForm(form);
                    return form;
                }
                Form getMatItemsForm(string pGroupCode)
                {
                    FormReference form = new MatItemsForm(this, pGroupCode);
                    initForm(form);
                    return form;
                }

                protected override void initForm(FormReference pForm)
                {
                    //base.initForm(pForm);
                }


                class MatGroupForm : FormReference
                {

                    MatReference matReference;

                    public MatGroupForm(MatReference pMatReference, FormType pFormType)
                    {
                        matReference = pMatReference;

                        Text = "Qruplar";

                        formType = pFormType;

                    }
                    public MatGroupForm(MatReference pMatReference)
                        : this(pMatReference, (PRM.TERM_TYPE == TERMTYPE.restoran ? FormType.box : FormType.grid))
                    {

                    }
                    protected override DataReference getDataReference()
                    {
                        return matReference;
                    }

                    protected override void initGrid()
                    {
                        base.initGrid();

                        DataGridView grid_ = getGrid();

                        string[] arrHeaderText = new string[]{
                            "",
                            _LANG.L.NAME, //1
                        };

                        string[] arrDataPropertyName = new string[]{
                            TABLE_DUMMY.COLS.INDX_COL ,
                            TABLE_SPECODES.COLS.SPECODE, //1
                             
                        };


                        DataGridViewContentAlignment[] arrAlignment = new DataGridViewContentAlignment[]{
                            DataGridViewContentAlignment.MiddleRight,
                            DataGridViewContentAlignment.MiddleLeft, //1
                        };

                        string[] arrFormat = new string[]{
                            "",
                            "", //1 
                        };
                        for (int colIndx_ = 0; colIndx_ < arrHeaderText.Length; ++colIndx_)
                        {
                            DataGridViewTextBoxColumn column_ = new DataGridViewTextBoxColumn();
                            column_.DataPropertyName = arrDataPropertyName[colIndx_];
                            column_.HeaderText = arrHeaderText[colIndx_];
                            //  column_.Width = arrWidth[colIndx_];
                            column_.DefaultCellStyle.Alignment = arrAlignment[colIndx_];
                            column_.DefaultCellStyle.Format = arrFormat[colIndx_];

                            column_.SortMode = arrDataPropertyName[colIndx_] == TABLE_DUMMY.COLS.INDX_COL ? DataGridViewColumnSortMode.NotSortable : DataGridViewColumnSortMode.Automatic;


                            grid_.Columns.Add(column_);

                            if (arrDataPropertyName[colIndx_] == TABLE_SPECODES.COLS.SPECODE)
                                column_.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }



                        DataTable tableGroups = matReference.getGroups();

                        _PLUGIN.TAB_ADDCOL(tableGroups, TABLE_DUMMY.COLS.INDX_COL, TABLE_DUMMY.TYPES.INDX_COL_TYPE);
                        _PLUGIN.TAB_SETCOL(tableGroups, TABLE_DUMMY.COLS.INDX_COL, 0);


                        grid_.DataSource = tableGroups;
                    }


                    protected override void initBoxes()
                    {
                        base.initBoxes();

                        if (formType == FormType.box)
                        {
                            DataTable tableGroups = matReference.getGroups();
                            loadBoxes(tableGroups);
                        }

                    }


                    protected override void gridCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
                    {
                        base.gridCellFormatting(sender, e);


                    }

                    protected override void gridCellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
                    {
                        base.gridCellMouseClick(sender, e);

                    }

                    protected override void activatedRecord(DataRow pRow, string pClickedColumn)
                    {
                        // base.activatedRecord(pRow, pClickedColumn);
                        string g = null;


                        if (
                            (formType == FormType.grid && pClickedColumn == TABLE_SPECODES.COLS.SPECODE) ||
                            (formType == FormType.box)
                            )
                        {
                            if (pRow != null)
                                g = CASTASSTRING(_PLUGIN.TAB_GETROW(pRow, TABLE_SPECODES.COLS.SPECODE));
                        }



                        if (g != null)
                            this.REF(g);
                    }

                    protected override void gridRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
                    {
                        base.gridRowPrePaint(sender, e);

                    }

                    protected override void gridError(object sender, DataGridViewDataErrorEventArgs e)
                    {
                        DataGridView grid_ = sender as DataGridView;
                        if (grid_ == null)
                            return;

                        base.gridError(sender, e);
                    }





                    public void REF(string pGroupCode)
                    {
                        if (matReference != null)
                            matReference.REF(pGroupCode);
                    }


                    protected override string filterColumn()
                    {
                        return "SPECODE";
                    }


                    protected override void Dispose(bool disposing)
                    {
                        base.Dispose(disposing);

                        matReference = null;
                    }





                }



                class MatItemsForm : FormReference
                {

                    MatReference matReference;
                    string groupCode;

                    public MatItemsForm(MatReference pMatReference, string pGroupCode, FormType pFormType)
                    {
                        matReference = pMatReference;
                        groupCode = pGroupCode;
                        //
                        Text = _LANG.L.SET;

                        formType = pFormType;
                    }
                    public MatItemsForm(MatReference pMatReference, string pGroupCode)
                        : this(pMatReference, pGroupCode, (PRM.TERM_TYPE == TERMTYPE.restoran ? FormType.box : FormType.grid))
                    {

                    }
                    protected override DataReference getDataReference()
                    {
                        return matReference;
                    }

                    protected override void initBoxes()
                    {
                        base.initBoxes();

                        if (formType == FormType.box)
                        {
                            loadBoxes(getData());
                        }

                    }
                    protected override void formatBox(Button pBox)
                    {
                        var b = pBox as ButtonExt;
                        if (b == null)
                            return;

                        var row = b.targetObject as DataRow;
                        if (row == null)
                            return;

                        string res = "";

                        var n = TAB_GETROW(row, TABLE_STLINE.COLS.ITEMS_NAME);
                        var u = TAB_GETROW(row, TABLE_STLINE.COLS.UNITSETL_CODE);
                        var p = TAB_GETROW(row, TABLE_STLINE.COLS.PRICE);
                        var a = CASTASDOUBLE(TAB_GETROW(row, TABLE_STLINE.COLS.AMOUNT));

                        res =
                            (n) +


                         ("\n" + FORMAT(p, "0.##") + " ") +
                        (a > 0.01 ? " X " + a + " " + u : "");

                        b.Text = res;
                        //
                        Color colorBack = a > 0.01 ? Color.Green : Color.Transparent;
                        if (b.BackColor != colorBack)
                            b.BackColor = colorBack;
                        //
                    }

                    protected override void initGrid()
                    {
                        base.initGrid();

                        DataGridView grid_ = getGrid();

                        string[] arrHeaderText = new string[]{
                            "",
                            _LANG.L.NAME, //1
                            "", //2
                           _LANG.L.PRICE, //3
                            _LANG.L.QUANTITY, //4
                          //  _LANG.L.TOTAL //5
                        };

                        string[] arrDataPropertyName = new string[]{
                            TABLE_DUMMY.COLS.INDX_COL ,
                            TABLE_STLINE.COLS.ITEMS_NAME, //1
                            TABLE_STLINE.COLS.UNITSETL_CODE, //2
                            TABLE_STLINE.COLS.PRICE, //3
                            TABLE_STLINE.COLS.AMOUNT, //4
                         //   TABLE_DATA.COLS.TOTAL, //5
                             
                        };

                        //int[] arrWidth = new int[]{
                        //        30,
                        //        350, //1
                        //        80, //2
                        //        100, //3
                        //        100, //4
                        //     //   100  //5

                        //    };
                        DataGridViewContentAlignment[] arrAlignment = new DataGridViewContentAlignment[]{
                            DataGridViewContentAlignment.MiddleRight,
                            DataGridViewContentAlignment.MiddleLeft, //1
                            DataGridViewContentAlignment.MiddleLeft, //2
                            DataGridViewContentAlignment.MiddleRight, //3
                            DataGridViewContentAlignment.MiddleRight, //4
                           // DataGridViewContentAlignment.MiddleRight, //5
                        };
                        string[] arrFormat = new string[]{
                            "",
                            "", //1 
                            "", //2
                            PRM.numberFormatGen, //3
                            PRM.numberFormatGen2, //4
                           // PRM.numberFormatGen //5
                        };
                        for (int colIndx_ = 0; colIndx_ < arrHeaderText.Length; ++colIndx_)
                        {
                            DataGridViewTextBoxColumn column_ = new DataGridViewTextBoxColumn();
                            column_.DataPropertyName = arrDataPropertyName[colIndx_];
                            column_.HeaderText = arrHeaderText[colIndx_];
                            //  column_.Width = arrWidth[colIndx_];
                            column_.DefaultCellStyle.Alignment = arrAlignment[colIndx_];
                            column_.DefaultCellStyle.Format = arrFormat[colIndx_];

                            column_.SortMode = arrDataPropertyName[colIndx_] == TABLE_DUMMY.COLS.INDX_COL ? DataGridViewColumnSortMode.NotSortable : DataGridViewColumnSortMode.Automatic;


                            grid_.Columns.Add(column_);

                            if (arrDataPropertyName[colIndx_] == TABLE_STLINE.COLS.ITEMS_NAME)
                                column_.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }




                        grid_.DataSource = getData();
                    }


                    DataTable getData()
                    {

                        DataTable tableData = TABLE_STLINE.TOOLS.createTable(this.matReference.PLUGIN);

                        tableData.ColumnChanged += tableData_ColumnChanged;

                        DataTable tableItems = matReference.getMatItems(groupCode);

                        foreach (DataRow row in tableItems.Rows)
                        {
                            DataRow newRow = tableData.NewRow();

                            object mlref;
                            object ulref;
                            double price;

                            TAB_SETROW(newRow, TABLE_STLINE.COLS.STOCKREF, mlref = TAB_GETROW(row, TABLE_ITEMS.COLS.LOGICALREF));
                            TAB_SETROW(newRow, TABLE_STLINE.COLS.UOMREF, ulref = TAB_GETROW(row, TABLE_ITEMS.COLS.UNITSETL_LOGICALREF));
                            TAB_SETROW(newRow, TABLE_STLINE.COLS.ITEMS_CODE, TAB_GETROW(row, TABLE_ITEMS.COLS.CODE));
                            TAB_SETROW(newRow, TABLE_STLINE.COLS.ITEMS_NAME, TAB_GETROW(row, TABLE_ITEMS.COLS.NAME));

                            TAB_SETROW(newRow, TABLE_STLINE.COLS.UNITSETL_CODE, TAB_GETROW(row, TABLE_ITEMS.COLS.UNITSETL_CODE));
                            TAB_SETROW(newRow, TABLE_STLINE.COLS.PRICE, price = _PLUGIN.CASTASDOUBLE(_PLUGIN.ISNULL(TAB_GETROW(row, TABLE_ITEMS.COLS.PRCLIST_PRICE), 0)));

                            TAB_SETROW(newRow, TABLE_STLINE.COLS.AMOUNT, matReference.posTerminal.getAmount(mlref, ulref, price, false));


                            tableData.Rows.Add(newRow);
                        }

                        _PLUGIN.TAB_FILLNULL(tableData);

                        return tableData;

                    }

                    void tableData_ColumnChanged(object sender, DataColumnChangeEventArgs e)
                    {
                        if (!TAB_ROWDELETED(e.Row))
                            columnChanged(e.Column.ColumnName, e.Row);
                    }

                    private void columnChanged(string pCol, DataRow pRow)
                    {
                        if (matReference == null || matReference.PLUGIN == null || matReference.posTerminal == null)
                            return;

                        matReference.posTerminal.dataTableColumnChanged(pCol, pRow);

                    }

                    protected override void gridCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
                    {
                        base.gridCellFormatting(sender, e);



                    }

                    protected override void gridCellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
                    {
                        base.gridCellMouseClick(sender, e);


                    }
                    protected override void activatedRecord(DataRow pRow, string pClickedColumn)
                    {
                        // base.activatedRecord(pRow, pClickedColumn);
                        if (formType == FormType.grid)
                        {
                            if (pClickedColumn == TABLE_STLINE.COLS.AMOUNT)
                                if (pRow != null)
                                    matReference.setAmount(pRow);
                        }
                        else
                        {
                            if (pRow != null)
                                matReference.setAmount(pRow);
                        }
                    }

                    protected override void gridRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
                    {
                        base.gridRowPrePaint(sender, e);


                    }

                    protected override void gridCellPrePainting(object sender, DataGridViewCellPaintingEventArgs e)
                    {
                        base.gridCellPrePainting(sender, e);






                    }

                    protected override void gridError(object sender, DataGridViewDataErrorEventArgs e)
                    {
                        DataGridView grid_ = sender as DataGridView;
                        if (grid_ == null)
                            return;

                        base.gridError(sender, e);
                    }

                    protected override string filterColumn()
                    {
                        return "NAME";
                    }


                    protected override void Dispose(bool disposing)
                    {
                        base.Dispose(disposing);

                        matReference = null;
                    }

                }

                public override void Dispose()
                {
                    base.Dispose();

                    PLUGIN = null;

                    posTerminal = null;


                }



            }



            //hotel
            class MatReferenceByDate : DataReference
            {

                bool noGroups;
                PosTerminal posTerminal;

                const string colDate1 = "DATE1";
                const string colDate2 = "DATE2";
                const string colPercent = "BYDATEPERC";
                const string colStatus = "BYDATESTATUS";

                //
                public MatReferenceByDate(_PLUGIN pPLUGIN, PosTerminal pPosTerminal, bool pNoGroups)
                    : base(pPLUGIN)
                {
                    noGroups = pNoGroups;
                    posTerminal = pPosTerminal;
                }

                bool select(DataRow pRefRecord, DateTime pDf, DateTime pDt)
                {
                    if (pRefRecord == null)
                        return false;

                    int len_ = (int)MAX(getNights(pDf, pDt), 1);

                    var perc_ = CASTASINT(TAB_GETROW(pRefRecord, colPercent));

                    if (perc_ != 100)
                    {
                        PLUGIN.MSGUSERINFO(_LANG.L.MSG_INFO_NOTSELECTABLE);
                        return false;
                    }

                    object mref = _PLUGIN.TAB_GETROW(pRefRecord, TABLE_STLINE.COLS.STOCKREF);
                    object uref = _PLUGIN.TAB_GETROW(pRefRecord, TABLE_STLINE.COLS.UOMREF);
                    double price = _PLUGIN.CASTASDOUBLE(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pRefRecord, TABLE_STLINE.COLS.PRICE), 0));
                    if (price < 0.01)
                        if (!PRM.USE_IF_NO_SLS_PRICE)
                        {
                            PLUGIN.MSGUSERINFO(_LANG.L.MSG_ERROR_NO_PRICE);
                            return false;
                        }

                    double quantity = len_;

                    string mcode = _PLUGIN.CASTASSTRING(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pRefRecord, TABLE_STLINE.COLS.ITEMS_CODE), ""));
                    string mdesc = _PLUGIN.CASTASSTRING(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pRefRecord, TABLE_STLINE.COLS.ITEMS_NAME), ""));

                    string ucode = _PLUGIN.CASTASSTRING(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pRefRecord, TABLE_STLINE.COLS.UNITSETL_CODE), ""));


                    ////////////////////////////

                    var row_ = posTerminal.addRecord(mref, mcode, mdesc, uref, ucode, 0, price, quantity, true);
                    TAB_SETROW(row_, TABLE_STLINE.COLS.DATEBEG, pDf);
                    TAB_SETROW(row_, TABLE_STLINE.COLS.DATEEND, pDt);

                    return true;
                }

                public static int getNights(DateTime df, DateTime dt)
                {
                    var diff = dt - df;

                    if (diff.TotalHours > 3)
                    {
                        diff = roundHotelDateEnd(dt) - roundHotelDateBeg(df);
                        return (int)MAX(diff.Days, 1);
                    }

                    return 0;

                }

                public static string getHotelDateRangeInfo(DateTime df, DateTime dt)
                {



                    // filterDateInfo
                    var info = "";
                    if (df < dt)
                    {
                        var diff = dt - df;

                        var nights = getNights(df, dt);

                        info = string.Format(nights > 0 ? "{1} " + _LANG.L.DAY + "" : "{0} " + _LANG.L.HOUR + "", diff.Hours, nights);
                    }
                    else
                        info = (_LANG.L.MSG_ERROR_DATERANGE);

                    return info;

                }

                public static DateTime roundHotelDate(DateTime d)
                {
                    return new DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, 0);
                }
                public static DateTime roundHotelDateBeg(DateTime d)
                {
                    if (d.Hour < 12)
                        d = d.AddDays(-1);

                    return new DateTime(d.Year, d.Month, d.Day, 12, 0, 0);
                }
                public static DateTime roundHotelDateEnd(DateTime d)
                {

                    d = d.AddMinutes(-1);

                    if (d.Hour >= 12)
                        d = d.AddDays(+1);

                    return new DateTime(d.Year, d.Month, d.Day, 12, 0, 0);
                }

                public static string formatHoletDate(DateTime date)
                {



                    return FORMAT(date.ToString("yy-MM-dd HH:mm"));

                }


                DataTable getGroups()
                {
                    return PLUGIN.SQL(
                        MY_CHOOSE_SQL(
@"SELECT TOP(400) 
                            SPECODE,DEFINITION_,TEXTF1,SPECODE2
                            FROM 
                            LG_$FIRM$_SPECODES WITH(NOLOCK) 
                            WHERE
                            CODETYPE = @P1 AND
                            SPECODETYPE = @P2
                            ORDER BY
                            CODETYPE ASC,
                            SPECODETYPE ASC,
                            SPECODE ASC,
                            LOGICALREF ASC",
@"SELECT 
                            SPECODE,DEFINITION_,TEXTF1,SPECODE2
                            FROM 
                            LG_$FIRM$_SPECODES 
                            WHERE
                            CODETYPE = @P1 AND
                            SPECODETYPE = @P2
                            ORDER BY
                            CODETYPE ASC,
                            SPECODETYPE ASC,
                            SPECODE ASC,
                            LOGICALREF ASC
LIMIT 400
")



,
                             new object[] { 4, 0 });

                }
                DataTable getMatItems(string pGroup)
                {

                    object payPlan1_ = 0;
                    object payPlan2_ = posTerminal.registers.clcard.getPaymentRef();



                    return PLUGIN.SQL(
                        MY_CHOOSE_SQL(
@"SELECT TOP(1000) 
LOGICALREF,CODE,NAME,SPECODE,SPECODE2,TEXTF1,INTF1,
( SELECT TOP(1) P.PRICE FROM LG_$FIRM$_PRCLIST P with(nolock) WHERE P.CARDREF = I.LOGICALREF AND P.PTYPE = 2 AND P.PAYPLANREF IN (@P4,@P5)  ORDER BY P.ENDDATE DESC,P.PAYPLANREF DESC) PRCLIST_PRICE,
(select top(1) U.LOGICALREF from LG_$FIRM$_UNITSETL U with(nolock) where U.UNITSETREF = I.UNITSETREF AND MAINUNIT = 1) UNITSETL_LOGICALREF,
(select top(1)       U.CODE from LG_$FIRM$_UNITSETL U with(nolock) where U.UNITSETREF = I.UNITSETREF AND MAINUNIT = 1) UNITSETL_CODE
FROM 
LG_$FIRM$_ITEMS I WITH(NOLOCK)
WHERE
STGRPCODE LIKE @P1 AND
CARDTYPE = @P2 AND
ACTIVE = @P3
ORDER BY
NAME ASC,
LOGICALREF ASC
",
 @"SELECT 
LOGICALREF,CODE,NAME,SPECODE,SPECODE2,TEXTF1,INTF1,
( SELECT P.PRICE FROM LG_$FIRM$_PRCLIST P with(nolock) WHERE P.CARDREF = I.LOGICALREF AND P.PTYPE = 2 AND P.PAYPLANREF IN (@P4,@P5)  ORDER BY P.ENDDATE DESC,P.PAYPLANREF DESC LIMIT 1) PRCLIST_PRICE,
(select U.LOGICALREF from LG_$FIRM$_UNITSETL U with(nolock) where U.UNITSETREF = I.UNITSETREF AND MAINUNIT = 1 LIMIT 1) UNITSETL_LOGICALREF,
(select U.CODE from LG_$FIRM$_UNITSETL U with(nolock) where U.UNITSETREF = I.UNITSETREF AND MAINUNIT = 1 LIMIT 1) UNITSETL_CODE
FROM 
LG_$FIRM$_ITEMS I 
WHERE
STGRPCODE LIKE @P1 AND
CARDTYPE = @P2 AND
ACTIVE = @P3
ORDER BY
NAME ASC,
LOGICALREF ASC
LIMIT 1000
"),
                             new object[] { pGroup, 1, 0, payPlan1_, payPlan2_ });

                }


                protected override void interactUser()
                {


                    if (noGroups)
                    {
                        var grp = "%";
                        if (PRM.TERM_TYPE == TERMTYPE.hotel)
                            grp = "ROOM%";

                        this.REF(grp);
                        return;
                    }

                    Form f = null;

                    try
                    {
                        f = getGroupForm();
                        f.ShowDialog();
                    }
                    finally
                    {
                        if (f != null) f.Dispose();
                    }
                }

                public void REF(string pGroupCode)
                {
                    Form f = null;

                    try
                    {
                        f = getMatItemsForm(pGroupCode);
                        f.ShowDialog();
                    }
                    finally
                    {
                        if (f != null) f.Dispose();
                    }
                }



                Form getGroupForm()
                {
                    FormReference form = new MatGroupForm(this);
                    initForm(form);
                    return form;
                }
                Form getMatItemsForm(string pGroupCode)
                {
                    FormReference form = new MatItemsForm(this, pGroupCode);
                    initForm(form);
                    return form;
                }

                protected override void initForm(FormReference pForm)
                {
                    //base.initForm(pForm);
                }

                class MatGroupForm : FormReference
                {

                    MatReferenceByDate matReference;


                    public MatGroupForm(MatReferenceByDate pMatReference)
                    {
                        matReference = pMatReference;

                        Text = "Qruplar";
                    }
                    protected override DataReference getDataReference()
                    {
                        return matReference;
                    }

                    protected override void initGrid()
                    {
                        base.initGrid();

                        DataGridView grid_ = getGrid();

                        string[] arrHeaderText = new string[]{
                            "",
                            _LANG.L.NAME, //1
                        };

                        string[] arrDataPropertyName = new string[]{
                            TABLE_DUMMY.COLS.INDX_COL ,
                            TABLE_SPECODES.COLS.SPECODE, //1
                             
                        };

                        //int[] arrWidth = new int[]{
                        //        30,
                        //        350, //1
                        //        80, //2
                        //        100, //3
                        //        100, //4
                        //     //   100  //5

                        //    };
                        DataGridViewContentAlignment[] arrAlignment = new DataGridViewContentAlignment[]{
                            DataGridViewContentAlignment.MiddleRight,
                            DataGridViewContentAlignment.MiddleLeft, //1
                        };
                        string[] arrFormat = new string[]{
                            "",
                            "", //1 
                        };
                        for (int colIndx_ = 0; colIndx_ < arrHeaderText.Length; ++colIndx_)
                        {
                            DataGridViewTextBoxColumn column_ = new DataGridViewTextBoxColumn();
                            column_.DataPropertyName = arrDataPropertyName[colIndx_];
                            column_.HeaderText = arrHeaderText[colIndx_];
                            //  column_.Width = arrWidth[colIndx_];
                            column_.DefaultCellStyle.Alignment = arrAlignment[colIndx_];
                            column_.DefaultCellStyle.Format = arrFormat[colIndx_];

                            column_.SortMode = arrDataPropertyName[colIndx_] == TABLE_DUMMY.COLS.INDX_COL ? DataGridViewColumnSortMode.NotSortable : DataGridViewColumnSortMode.Automatic;


                            grid_.Columns.Add(column_);

                            if (arrDataPropertyName[colIndx_] == TABLE_SPECODES.COLS.SPECODE)
                                column_.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }



                        DataTable tableGroups = matReference.getGroups();

                        _PLUGIN.TAB_ADDCOL(tableGroups, TABLE_DUMMY.COLS.INDX_COL, TABLE_DUMMY.TYPES.INDX_COL_TYPE);
                        _PLUGIN.TAB_SETCOL(tableGroups, TABLE_DUMMY.COLS.INDX_COL, 0);


                        grid_.DataSource = tableGroups;
                    }

                    protected override void gridCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
                    {
                        base.gridCellFormatting(sender, e);


                    }

                    protected override void gridCellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
                    {
                        base.gridCellMouseClick(sender, e);

                    }

                    protected override void activatedRecord(DataRow pRow, string pClickedColumn)
                    {
                        // base.activatedRecord(pRow, pClickedColumn);

                        if (pClickedColumn == TABLE_SPECODES.COLS.SPECODE)
                        {
                            if (pRow != null)
                                this.REF(_PLUGIN.CASTASSTRING(_PLUGIN.TAB_GETROW(pRow, TABLE_SPECODES.COLS.SPECODE)));
                        }
                    }

                    protected override void gridRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
                    {
                        base.gridRowPrePaint(sender, e);

                    }

                    protected override void gridError(object sender, DataGridViewDataErrorEventArgs e)
                    {
                        DataGridView grid_ = sender as DataGridView;
                        if (grid_ == null)
                            return;

                        base.gridError(sender, e);
                    }





                    public void REF(string pGroupCode)
                    {
                        if (matReference != null)
                            matReference.REF(pGroupCode);
                    }





                    protected override void Dispose(bool disposing)
                    {
                        base.Dispose(disposing);

                        matReference = null;
                    }





                }

                class MatItemsForm : FormReference
                {

                    class MatByDateRange //: ICloneable
                    {
                        public MatByDateRange(object mat, DateTime df, DateTime dt)
                        {
                            mLRef = mat;
                            dateFrom = df;
                            dateTo = dt;

                        }

                        public object mLRef;
                        public DateTime dateFrom;
                        public DateTime dateTo;
                        public object transLRef = null;
                        public object invLRef = null;
                        public bool cancelled = false;

                        public bool isFree()
                        {
                            return ISEMPTYLREF(transLRef);
                        }


                        public bool isIntersect(DateTime df, DateTime dt)
                        {
                            if (df > dt)
                                return false;

                            return (
                                (df >= dateFrom && df <= dateTo) ||
                                (df <= dateFrom && dateTo <= dt) ||
                                (dt >= dateFrom && dt <= dateTo)
                                );
                        }

                    }

                    MatReferenceByDate matReference;
                    string groupCode;
                    DataTable tabFilter;

                    TextBox filterDateInfo;

                    public MatItemsForm(MatReferenceByDate pMatReference, string pGroupCode)
                    {
                        matReference = pMatReference;
                        groupCode = pGroupCode;
                        //
                        Text = "Otaq";
                        //

                        btnSelect = true;
                        //


                        tabFilter = new DataTable();
                        tabFilter.Columns.Add(colDate1, typeof(DateTime));
                        tabFilter.Columns.Add(colDate2, typeof(DateTime));
                        //

                        var date = DateTime.Now;
                        var df = roundHotelDate(date);
                        var dt = roundHotelDateEnd(df);

                        //
                        tabFilter.Rows.Add(df, dt);
                    }

                    protected override void initedForm()
                    {
                        base.initedForm();

                        updateFilterInfo();
                    }

                    protected override DataReference getDataReference()
                    {
                        return matReference;
                    }
                    protected override string filterColumn()
                    {
                        return TABLE_STLINE.COLS.ITEMS_NAME; ;
                    }
                    protected override void initPanelTopFilter(Panel pPanelTopFilter)
                    {
                        FlowLayoutPanel wrap = new FlowLayoutPanel();
                        wrap.Dock = DockStyle.Fill;
                        wrap.WrapContents = false;
                        wrap.FlowDirection = FlowDirection.LeftToRight;

                        Label lDate = new Label();
                        lDate.Text = _LANG.L.DATE + "";
                        lDate.Width = 60;
                        //
                        DateTimePicker date1 = new DateTimePicker();

                        date1.DataBindings.Add("Value", tabFilter, colDate1);

                        DateTimePicker date2 = new DateTimePicker();

                        date2.DataBindings.Add("Value", tabFilter, colDate2);
                        date1.Width = date2.Width = 200;

                        date1.Format = date2.Format = DateTimePickerFormat.Custom;
                        date1.CustomFormat = date2.CustomFormat = "yyyy-MM-dd  HH:mm";

                        //date1.ShowUpDown = date2.ShowUpDown = true;
                        // 
                        var lInfo = filterDateInfo = new TextBox() { Text = "", Width = 120, ReadOnly = true };


                        //
                        var dateWrap = new FlowLayoutPanel() { FlowDirection = FlowDirection.TopDown };
                        dateWrap.Controls.AddRange(new Control[] { date1, date2 });

                        wrap.Controls.AddRange(new Control[]{
                            lDate,
                            dateWrap,
                           // date1,
                           // date2,
                            lInfo,
                            new ButtonExt() { Text = "+H", AutoSize = false, Size = new Size(50, 30), cmd = "plush" },
                            new ButtonExt() { Text = "+D", AutoSize = false, Size = new Size(50, 30), cmd = "plusd" },
                            new ButtonExt() { Text = "-H", AutoSize = false, Size = new Size(50, 30), cmd = "minush" },
                            new ButtonExt() { Text = "-D", AutoSize = false, Size = new Size(50, 30), cmd = "minusd" },
                            new ButtonExt() { Text = "R", AutoSize = false, Size = new Size(50, 30), cmd = "round" },
                        });

                        pPanelTopFilter.Controls.Add(wrap);

                        tabFilter.ColumnChanged += tabFilter_ColumnChanged;
                    }

                    void tabFilter_ColumnChanged(object sender, DataColumnChangeEventArgs e)
                    {
                        var col = e.Column.ColumnName;
                        var val = e.ProposedValue;
                        var row = e.Row;

                        switch (col)
                        {
                            case colDate1:
                            case colDate2:
                                filterChanged(col);
                                break;
                        }

                        var cm = this.BindingContext[tabFilter] as CurrencyManager;
                        if (cm != null)
                        {
                            cm.Refresh();
                        }



                    }

                    void filterChanged(string code)
                    {

                        reloadData();
                    }

                    DateTime getFilterDate1()
                    {
                        return ((DateTime)TAB_GETROW(tabFilter, colDate1));
                    }
                    DateTime getFilterDate2()
                    {
                        return ((DateTime)TAB_GETROW(tabFilter, colDate2));
                    }

                    void setFilterDate1(DateTime pDate)
                    {
                        var x = getFilterDate1();
                        if (pDate < x)
                            pDate = x;
                        TAB_SETROW(tabFilter, colDate1, pDate);
                    }
                    void setFilterDate2(DateTime pDate)
                    {
                        var x = getFilterDate1();
                        if (pDate < x)
                            pDate = x;

                        TAB_SETROW(tabFilter, colDate2, pDate);
                    }

                    public override void runCmd(string pCmd)
                    {
                        base.runCmd(pCmd);

                        switch (pCmd)
                        {
                            case "plush":
                                {
                                    var d = getFilterDate2();
                                    d = d.AddHours(1);
                                    setFilterDate2(d);
                                }
                                break;

                            case "plusd":
                                {
                                    var d = getFilterDate2();
                                    d = d.AddDays(1);
                                    d = roundHotelDate(d);
                                    setFilterDate2(d);
                                }
                                break;
                            case "minush":
                                {
                                    var d = getFilterDate2();
                                    d = d.AddHours(-1);
                                    setFilterDate2(d);
                                }
                                break;

                            case "minusd":
                                {
                                    var d = getFilterDate2();
                                    d = d.AddDays(-1);
                                    d = roundHotelDate(d);
                                    setFilterDate2(d);
                                }
                                break;
                            case "round":
                                {
                                    var d = getFilterDate2();
                                    d = roundHotelDateEnd(d);
                                    setFilterDate2(d);
                                }
                                break;
                        }
                    }




                    protected override void initGrid()
                    {
                        base.initGrid();

                        DataGridView grid_ = getGrid();

                        grid_.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                        string[] arrHeaderText = new string[]{
                            "",
                            _LANG.L.NAME, //1
                            "  %  ",
                           // "", //2
                            _LANG.L.PRICE, //3
                           // _LANG.L.QUANTITY, //4
                          //  _LANG.L.TOTAL //5
                     
                        };

                        string[] arrDataPropertyName = new string[]{
                            TABLE_DUMMY.COLS.INDX_COL ,
                            TABLE_STLINE.COLS.ITEMS_NAME, //1
                            colPercent,
                   
                            TABLE_STLINE.COLS.PRICE, //3
                 
                    
                             
                        };

                        int[] arrWidth = new int[]{
                                40,
                                320, //1
                                60, //2
                                80, //3
                                
                             //   100  //5

                           };
                        DataGridViewContentAlignment[] arrAlignment = new DataGridViewContentAlignment[]{
                            DataGridViewContentAlignment.MiddleRight,
                            DataGridViewContentAlignment.MiddleLeft,
                            DataGridViewContentAlignment.MiddleLeft, //1
                         //   DataGridViewContentAlignment.MiddleLeft, //2
                            DataGridViewContentAlignment.MiddleRight, //3
                         //   DataGridViewContentAlignment.MiddleRight, //4
                           // DataGridViewContentAlignment.MiddleRight, //5
                           DataGridViewContentAlignment.MiddleRight, //3
                        };
                        string[] arrFormat = new string[]{
                            "",
                            "",
                            "", //1 
                            PRM.numberFormatGen, //3
                         //   PRM.numberFormatGen2, //4
                           // PRM.numberFormatGen //5
                         
                        };
                        for (int colIndx_ = 0; colIndx_ < arrHeaderText.Length; ++colIndx_)
                        {
                            DataGridViewTextBoxColumn column_ = new DataGridViewTextBoxColumn();
                            column_.DataPropertyName = arrDataPropertyName[colIndx_];
                            column_.HeaderText = arrHeaderText[colIndx_];
                            column_.Width = arrWidth[colIndx_];
                            column_.DefaultCellStyle.Alignment = arrAlignment[colIndx_];
                            column_.DefaultCellStyle.Format = arrFormat[colIndx_];
                            column_.MinimumWidth = 25;
                            //  column_.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            column_.SortMode = arrDataPropertyName[colIndx_] == TABLE_DUMMY.COLS.INDX_COL ? DataGridViewColumnSortMode.NotSortable : DataGridViewColumnSortMode.Automatic;

                            if (column_.DataPropertyName == TABLE_STLINE.COLS.ITEMS_NAME)
                            {
                                column_.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                                column_.MinimumWidth = 360;
                            }

                            grid_.Columns.Add(column_);




                        }



                        reloadData();




                    }


                    void updateFilterInfo()
                    {

                        if (filterDateInfo == null)
                            return;

                        DateTime df = getFilterDate1();
                        DateTime dt = getFilterDate2();


                        // filterDateInfo


                        filterDateInfo.Text = getHotelDateRangeInfo(df, dt);



                    }
                    void reloadData()
                    {

                        try
                        {

                            this.SuspendLayout();
                            DataGridView grid_ = getGrid();



                            DateTime df = getFilterDate1();
                            DateTime dt = getFilterDate2();

                            updateFilterInfo();

                            if ((dt - df).TotalMinutes < 30)
                            {


                                return;

                            }

                            {




                                DataTable tableData = null;

                                if (grid_.DataSource == null)
                                {
                                    tableData = TABLE_STLINE.TOOLS.createTable(this.matReference.PLUGIN);
                                    TAB_ADDCOL(tableData, TABLE_STLINE.COLS.ITEMS_INTF1, TABLE_STLINE.TYPES.ITEMS_INTF1);
                                    tableData.ColumnChanged += tableData_ColumnChanged;

                                    grid_.DataSource = tableData;
                                }

                                tableData = grid_.DataSource as DataTable;

                                tableData.Clear();

                                DataTable tableItems = matReference.getMatItems(groupCode);

                                foreach (DataRow row in tableItems.Rows)
                                {
                                    DataRow newRow = tableData.NewRow();

                                    object mlref;
                                    object ulref;
                                    double price;

                                    TAB_SETROW(newRow, TABLE_STLINE.COLS.STOCKREF, mlref = TAB_GETROW(row, TABLE_ITEMS.COLS.LOGICALREF));
                                    TAB_SETROW(newRow, TABLE_STLINE.COLS.UOMREF, ulref = TAB_GETROW(row, TABLE_ITEMS.COLS.UNITSETL_LOGICALREF));
                                    TAB_SETROW(newRow, TABLE_STLINE.COLS.ITEMS_CODE, TAB_GETROW(row, TABLE_ITEMS.COLS.CODE));
                                    TAB_SETROW(newRow, TABLE_STLINE.COLS.ITEMS_NAME, TAB_GETROW(row, TABLE_ITEMS.COLS.NAME));
                                    TAB_SETROW(newRow, TABLE_STLINE.COLS.ITEMS_INTF1, TAB_GETROW(row, TABLE_ITEMS.COLS.INTF1));

                                    TAB_SETROW(newRow, TABLE_STLINE.COLS.UNITSETL_CODE, TAB_GETROW(row, TABLE_ITEMS.COLS.UNITSETL_CODE));
                                    TAB_SETROW(newRow, TABLE_STLINE.COLS.PRICE, price = _PLUGIN.CASTASDOUBLE(_PLUGIN.ISNULL(TAB_GETROW(row, TABLE_ITEMS.COLS.PRCLIST_PRICE), 0)));

                                    TAB_SETROW(newRow, TABLE_STLINE.COLS.AMOUNT, matReference.posTerminal.getAmount(mlref, ulref, price, false));


                                    tableData.Rows.Add(newRow);
                                }

                                _PLUGIN.TAB_FILLNULL(tableData);



                            }


                            {


                                DataTable tableData = grid_.DataSource as DataTable;
                                var dic = new Dictionary<object, List<MatByDateRange>>();
                                //per material all dateranges
                                foreach (DataRow row in tableData.Rows)
                                {
                                    var lref_ = TAB_GETROW(row, TABLE_STLINE.COLS.STOCKREF);
                                    var data_ = getMatByDate(lref_, df, dt);
                                    var list = dic[lref_] = new List<MatByDateRange>();

                                    foreach (DataRow trans in data_.Rows)
                                    {


                                        var obj = new MatByDateRange(
                                            lref_,
                                            (DateTime)TAB_GETROW(trans, TABLE_STLINE.COLS.DATEBEG),
                                             (DateTime)TAB_GETROW(trans, TABLE_STLINE.COLS.DATEEND)
                                            )
                                        {
                                            transLRef = TAB_GETROW(trans, TABLE_STLINE.COLS.LOGICALREF),
                                            cancelled = (CASTASINT(TAB_GETROW(trans, TABLE_STLINE.COLS.CANCELLED)) == 1),
                                            invLRef = TAB_GETROW(trans, TABLE_STLINE.COLS.INVOICEREF),

                                        };

                                        if (obj.dateFrom < df)
                                            obj.dateFrom = df;

                                        if (obj.dateTo > dt)
                                            obj.dateTo = dt;


                                        list.Add(obj);

                                    }

                                    if (list.Count == 0)
                                        list.Add(new MatByDateRange(lref_, df, dt));
                                }
                                //get all distinct dates
                                List<DateTime> listDates = new List<DateTime>();
                                {
                                    if (!listDates.Contains(df))
                                        listDates.Add(df);
                                    foreach (var l in dic.Values)
                                        foreach (var o in l)
                                        {
                                            if (!listDates.Contains(o.dateFrom))
                                                listDates.Add(o.dateFrom);
                                            if (!listDates.Contains(o.dateTo))
                                                listDates.Add(o.dateTo);
                                        }

                                    if (!listDates.Contains(dt))
                                        listDates.Add(dt);

                                    listDates.Sort();
                                }


                                //
                                //clean grid
                                for (int i = 0; i < grid_.Columns.Count; ++i)
                                    if (grid_.Columns[i].DataPropertyName.StartsWith(colStatus))
                                    {
                                        grid_.Columns.Remove(grid_.Columns[i]);
                                        --i;
                                    }

                                //clean data
                                for (int i = 0; i < tableData.Columns.Count; ++i)
                                    if (tableData.Columns[i].ColumnName.StartsWith(colStatus))
                                    {
                                        tableData.Columns.Remove(tableData.Columns[i]);
                                        --i;
                                    }


                                TAB_ADDCOL(tableData, colPercent, typeof(int));
                                //len - 1 cols
                                for (int i = 0; i < listDates.Count - 1; ++i)
                                {
                                    string name_ = colStatus + FORMAT(i);
                                    //
                                    tableData.Columns.Add(name_, typeof(object));
                                    //

                                    DateTime date_ = df.AddDays(i);

                                    var x = listDates[i];
                                    var y = listDates[i + 1];
                                    string header_ =
                                        formatHoletDate(x) +
                                        "\n" +
                                        getHotelDateRangeInfo(x, y) +
                                        "\n" +
                                        formatHoletDate(y);
                                    //

                                    DataGridViewTextBoxColumn column_ = new DataGridViewTextBoxColumn();
                                    //
                                    column_.Tag = new MatByDateRange(0, x, y);
                                    //
                                    column_.DataPropertyName = name_;
                                    column_.HeaderText = header_;

                                    column_.DefaultCellStyle.Format = "";

                                    column_.SortMode = DataGridViewColumnSortMode.NotSortable;

                                    column_.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                                    grid_.Columns.Add(column_);
                                }

                                foreach (DataRow row in tableData.Rows)
                                {
                                    var lref_ = TAB_GETROW(row, TABLE_STLINE.COLS.STOCKREF);

                                    var transDates_ = dic[lref_];


                                    for (int i = 0; i < listDates.Count - 1; ++i)
                                    {
                                        string name_ = colStatus + FORMAT(i);
                                        var b = listDates[i].AddMinutes(+1);//sensivety
                                        var e = listDates[i + 1].AddMinutes(-1);//sensivety

                                        foreach (var x in transDates_)
                                        {
                                            if (x.isIntersect(b, e))
                                                row[name_] = x;
                                        }
                                    }

                                    var totUsed_ = 0.0;
                                    foreach (var x in transDates_)
                                    {
                                        if (!x.isFree())
                                            totUsed_ += (x.dateTo - x.dateFrom).TotalHours;

                                    }
                                    var maxFree_ = (dt - df).TotalHours;

                                    var emptyRate_ = 100.0;

                                    if (maxFree_ > 0.01)
                                    {
                                        if (totUsed_ > 0.1)
                                            emptyRate_ = Math.Floor(100.0 * (maxFree_ - totUsed_) / maxFree_);
                                    }
                                    else
                                        emptyRate_ = 0;

                                    TAB_SETROW(row, colPercent, emptyRate_);

                                }

                                tableData.DefaultView.Sort = colPercent + " DESC";

                                //

                                /*

                                int len_ = (dt - df).Days + 1;


                                //clean grid
                                for (int i = 0; i < grid_.Columns.Count; ++i)
                                    if (grid_.Columns[i].DataPropertyName.StartsWith(colStatus))
                                    {
                                        grid_.Columns.Remove(grid_.Columns[i]);
                                        --i;
                                    }

                                //clean data
                                for (int i = 0; i < tableData.Columns.Count; ++i)
                                    if (tableData.Columns[i].ColumnName.StartsWith(colStatus))
                                    {
                                        tableData.Columns.Remove(tableData.Columns[i]);
                                        --i;
                                    }

                                for (int i = 0; i < len_; ++i)
                                {
                                    string name_ = colStatus + FORMAT(i);
                                    //
                                    tableData.Columns.Add(name_, typeof(int));
                                }

                                TAB_ADDCOL(tableData, colPercent, typeof(int));

                                //fill values
                                foreach (DataRow row in tableData.Rows)
                                {
                                    var lref_ = TAB_GETROW(row, TABLE_STLINE.COLS.STOCKREF);
                                    var data_ = getMatByDate(lref_, df, dt);
                                    int activeCount = 0;

                                    foreach (DataRow rowSub in data_.Rows)
                                    {
                                        var indx_ = CASTASINT(TAB_GETROW(rowSub, "NR"));
                                        var flagC_ = CASTASINT(TAB_GETROW(rowSub, "HASCANCLLED"));
                                        var flagS_ = CASTASINT(TAB_GETROW(rowSub, "HASSALE"));

                                        int status_ = (int)MAT_STAT.active;

                                        if (flagC_ > 0)
                                            status_ = (int)MAT_STAT.order;

                                        if (flagS_ > 0)
                                            status_ = (int)MAT_STAT.sale;

                                        if (status_ == (int)MAT_STAT.active)
                                            ++activeCount;

                                        TAB_SETROW(row, colStatus + FORMAT(indx_), status_);
                                    }

                                    TAB_SETROW(row, colPercent, 100.0 * activeCount / len_);

                                }

                                tableData.DefaultView.Sort = colPercent + " DESC";

                                for (int i = 0; i < len_; ++i)
                                {
                                    string name_ = colStatus + FORMAT(i);
                                    DateTime date_ = df.AddDays(i);
                                    string header_ = date_.ToString("MM") + "\n" + date_.ToString("dd");
                                    //
                                    DataGridViewTextBoxColumn column_ = new DataGridViewTextBoxColumn();
                                    column_.DataPropertyName = name_;
                                    column_.HeaderText = header_;

                                    column_.DefaultCellStyle.Format = "";

                                    column_.SortMode = DataGridViewColumnSortMode.NotSortable;

                                    column_.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                                    grid_.Columns.Add(column_);

                                }

                                */

                            }


                        }
                        finally
                        {
                            this.ResumeLayout();
                        }
                    }


                    DataTable getMatByDate(object pLRef, DateTime pF, DateTime pT)
                    {
                        //sort bar beg,end date
                        string sql = @"
declare @df datetime,@dt datetime

select @df = DATEADD (minute , +1 ,  @P2 ), @dt = DATEADD (minute , -1 , @P3 )
 
select LOGICALREF,INVOICEREF,CANCELLED,DATEBEG,DATEEND from LG_$FIRM$_$PERIOD$_STLINE with(nolock) where 
(
STOCKREF = @P1 AND 
(
(@df between DATEBEG and DATEEND ) OR
(@df <= DATEBEG AND DATEEND <= @dt ) OR
(@dt between DATEBEG and DATEEND )
)
) AND 
LINETYPE = 0 AND TRCODE = 8 
ORDER BY STOCKREF,DATEBEG,DATEEND


                                    ";


                        return matReference.PLUGIN.SQL(sql, new object[] { pLRef, pF, pT });//, DateTime.Now.Date });


                    }


                    void tableData_ColumnChanged(object sender, DataColumnChangeEventArgs e)
                    {
                        if (!TAB_ROWDELETED(e.Row))
                            columnChanged(e.Column.ColumnName, e.Row);
                    }

                    private void columnChanged(string pCol, DataRow pRow)
                    {
                        if (matReference == null || matReference.PLUGIN == null || matReference.posTerminal == null)
                            return;

                        matReference.posTerminal.dataTableColumnChanged(pCol, pRow);

                    }

                    protected override void gridCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
                    {
                        base.gridCellFormatting(sender, e);

                        if (e.ColumnIndex < 0)
                            return;
                        if (e.RowIndex < 0)
                            return;

                        var grid_ = sender as DataGridView;
                        if (grid_ == null)
                            return;

                        var col_ = grid_.Columns[e.ColumnIndex];

                        if (col_.DataPropertyName != null && col_.DataPropertyName.StartsWith(colStatus))
                        {
                            e.Value = "";
                            e.FormattingApplied = true;


                        }

                    }

                    protected override void gridCellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
                    {
                        base.gridCellMouseClick(sender, e);


                    }



                    protected override void activatedRecord(DataRow pRow, string pClickedColumn)
                    {
                        // base.activatedRecord(pRow, pClickedColumn);
                        //  if (pClickedColumn == TABLE_STLINE.COLS.AMOUNT)
                        //     if (pRow != null)
                        //       matReference.setAmount(pRow);


                        if (pClickedColumn == null)//by button
                        {
                            if (pRow != null)
                            {
                                DateTime df = getFilterDate1();
                                DateTime dt = getFilterDate2();
                                if (matReference.select(pRow, df, dt))
                                    finish();
                            }

                        }

                        if (pClickedColumn != null && pClickedColumn.StartsWith(colStatus))//by button
                        {
                            var x = TAB_GETROW(pRow, pClickedColumn) as MatByDateRange;
                            if (x != null && !x.isFree())
                            {

                                var p = new BARCODETERM_BASE.PosTerminal(this.matReference.PLUGIN);

                                p.BEGIN_TERMINAL(x.invLRef);

                            }
                        }


                    }

                    protected override void gridRowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
                    {
                        base.gridRowPrePaint(sender, e);


                    }



                    enum MAT_STAT
                    {
                        active = 0,
                        sale = 1,
                        order = 2,



                    }
                    protected override void gridCellPrePainting(object sender, DataGridViewCellPaintingEventArgs e)
                    {
                        base.gridCellPrePainting(sender, e);

                        if (e.ColumnIndex < 0)
                            return;
                        if (e.RowIndex < 0)
                            return;

                        var grid_ = sender as DataGridView;
                        if (grid_ == null)
                            return;

                        var row_ = TOOLSGRID.MY_GET_GRID_ROW_DATA(grid_, e.RowIndex);
                        if (row_ == null)
                            return;

                        var col_ = grid_.Columns[e.ColumnIndex];

                        if (col_.DataPropertyName != null)
                        {
                            if (col_.DataPropertyName.StartsWith(colStatus))
                            {
                                var v = e.Value as MatByDateRange;

                                if (v == null)
                                    v = new MatByDateRange(null, DateTime.MinValue, DateTime.MinValue);


                                Color col = Color.Black;

                                if (v.isFree())
                                    col = Color.Green;
                                else
                                {
                                    if (v.cancelled)
                                        col = Color.Orange;
                                    else
                                        col = Color.Red;
                                }


                                TOOLSGRID.MY_SETSTYLECOLOR(e.CellStyle, col);



                            }



                            if (col_.DataPropertyName == "INDX")
                            {
                                var c = CASTASINT(ISNULL(TAB_GETROW(row_, TABLE_STLINE.COLS.ITEMS_INTF1), 0)) == 0 ? this.BackColor : Color.Red;

                                TOOLSGRID.MY_SETSTYLECOLOR(e.CellStyle, c);
                            }

                        }





                    }
                    protected override void gridError(object sender, DataGridViewDataErrorEventArgs e)
                    {
                        DataGridView grid_ = sender as DataGridView;
                        if (grid_ == null)
                            return;

                        base.gridError(sender, e);
                    }




                    protected override void Dispose(bool disposing)
                    {
                        base.Dispose(disposing);

                        matReference = null;
                    }

                }

                public override void Dispose()
                {
                    base.Dispose();

                    PLUGIN = null;

                    posTerminal = null;


                }



            }


            public interface ObjectWithId
            {

                string getObjectId();
            }

            public class PosTerminal
            {

                public PosTerminal(_PLUGIN pPlugin)
                {
                    PLUGIN = pPlugin;
                }
                public PosTerminal COPY()
                {
                    return new PosTerminal(PLUGIN);
                }


                public static bool canStartTermType(TERMTYPE pType)
                {
                    if (PRM == null)
                        return true;



                    foreach (Form f in Application.OpenForms)
                    {
                        var o = f as TerminalForm;

                        if (o != null && pType != PRM.TERM_TYPE)
                            return false;


                    }

                    return true;
                }



                public _PLUGIN PLUGIN;
                public string _CLSUFIX = "";
                public string _DESIGN = "";
                public short _TRCODE = 8;
                public bool _READONLY = false;


                public bool isSlsOrPrchDoc()
                {
                    return isSlsOrPrchDoc(_TRCODE);
                }
                public static bool isSlsOrPrchDoc(short _TRCODE)
                {
                    return (_TRCODE == 8 || _TRCODE == 3 || _TRCODE == 1 || _TRCODE == 6);
                }
                public bool _CANCELLED = false;

                public short _WH = -2;

                public string _SPECODE = "";
                public string _DOCODE = "";
                public string _GENEXP1 = "";

                DataTable tableData_;
                DataTable tableDataHeader_;

                public DataTable getTable() { return tableData_; }

                DataTable tableInfo_;
                DataTable tableMsg_;

                DataSet dataSet_;

                TerminalForm dataInputForm_;

                public int parentFormIndex;

                public Registers registers;



                class TerminalForm : Form, CmdProcessing, ObjectWithId
                {
                    public DataTable tableData;
                    public DataTable tableInfo;
                    public DataTable tableMsg;

                    static int index_ = 0;


                    public readonly int formIndex = ++index_;


                    DataGridView grid_;
                    DataGridView gridInfo_;
                    DataGridView gridMsg_;


                    Panel panelTop = new Panel();

                    Panel panelButton1 = new Panel();
                    Panel panelButton2 = new Panel();

                    Panel panelBottom = new Panel();
                    Panel panelBottomRight = new Panel();
                    Panel panelBottomCenter = new Panel();
                    Panel panelBottomLeft = new Panel();

                    PosTerminal posTerminal;

                    public string getObjectId()
                    {
                        return TABLE_INVOICE.TOOLS.getObjectId(posTerminal.registers.docLRef);

                    }

                    public const string _NAME = "BarcodeTerminalForm";
                    public override string ToString()
                    {
                        return _NAME + FORMAT(formIndex).PadLeft(10, '0');
                    }
                    public TerminalForm(PosTerminal pPosTerminal)
                    {//



                        //
                        posTerminal = pPosTerminal;
                        //
                        KeyPreview = true;
                        BackColor = PRM.COLOR_MAIN;
                        Icon = CTRL_FORM_ICON();
                        //if (Application.OpenForms.Count > 0)
                        //    Icon = Application.OpenForms[0].Icon;

                        Text = "POS";

                        {
                            string suf_ = posTerminal._CLSUFIX;
                            if (suf_ != string.Empty)
                                Text = Text + " [" + suf_ + "]";

                        }

                        {
                            string suf_ = posTerminal._DESIGN;
                            if (suf_ != string.Empty)
                                Text = Text + " [" + suf_ + "]";

                        }
                        
                        

                        {
                            string suf_ = string.Empty;

                            switch (posTerminal._TRCODE)
                            {
                                case 8:
                                    suf_ = _LANG.L.MSG_INFO_SALE;
                                    break;
                                case 3:
                                    suf_ = _LANG.L.MSG_INFO_RETURN;
                                    break;
                                case 1:
                                    suf_ = _LANG.L.MSG_INFO_PURCHASE;
                                    break;
                                case 6:
                                    suf_ = _LANG.L.MSG_INFO_PURCHASERET;
                                    break;
                                case 13:
                                    suf_ = _LANG.L.MSG_INFO_PRODUCTION;
                                    break;
                                case 50:
                                    suf_ = _LANG.L.MSG_INFO_COUNT;
                                    break;
                                case 101:
                                    suf_ = _LANG.L.MSG_INFO_LABEL;
                                    break;
                                case 102:
                                    suf_ = _LANG.L.MSG_INFO_PRICE;
                                    break;
                                default:
                                    suf_ = "UDEFINED !!!";
                                    break;


                            }
                            if (suf_ != string.Empty)
                                Text = Text + " [" + suf_ + "]";


                        }

                        if (PRM.FULL_SCREEN)
                            FormBorderStyle = FormBorderStyle.None;

                        WindowState = FormWindowState.Maximized;
                        Size = new System.Drawing.Size(720, 450);
                        //if (PRM.TERM_TYPE == TERMTYPE.pricing)
                        //{
                        //    Font = new System.Drawing.Font("Monospace", PRM.FONT_SIZE, FontStyle.Bold);

                        //}
                        //else
                        //    Font = new System.Drawing.Font("Verdana", PRM.FONT_SIZE, FontStyle.Bold);
                        //

                        var isWin7 = (Environment.OSVersion.Version >= new Version(6, 0));
                        Font = new System.Drawing.Font(isWin7 ? "Segoe UI" : "Arial", PRM.FONT_SIZE, FontStyle.Bold);

                        initDataGrid();
                        //
                        initInfoGrid();
                        initMsgGrid();

                        initStruct();







                    }

                    protected override void OnShown(EventArgs e)
                    {
                        base.OnShown(e);

                        this.Activate();
                    }

                    private void initStruct()
                    {

                        var panelGrid_ = new Panel();
                        panelGrid_.AutoSize = true;



                        grid_.BackgroundColor =
                            gridInfo_.BackgroundColor =
                            gridMsg_.BackgroundColor = this.BackColor;

                        gridInfo_.DefaultCellStyle.BackColor = gridInfo_.DefaultCellStyle.SelectionBackColor = this.BackColor;
                        gridMsg_.DefaultCellStyle.BackColor = gridMsg_.DefaultCellStyle.SelectionBackColor = this.BackColor;



                        panelTop.Dock = DockStyle.Fill;
                        panelBottom.Dock = DockStyle.Bottom;
                        panelBottom.Height = (int)(PRM.GRID_ROW_H * 5.2);

                        panelBottomRight.Dock = DockStyle.Right;
                        panelBottomRight.Width = 250;
                        panelBottomCenter.Dock = DockStyle.Fill;
                        panelBottomLeft.Dock = DockStyle.Left;
                        panelBottomLeft.Width = 400;

                        panelButton1.Height = panelButton2.Height = PRM.CMD_BTN_H;
                        panelButton1.Dock = panelButton2.Dock = DockStyle.Bottom;

                        //
                        //add btns

                        {
                            List<ButtonCmdInfo> l = new List<ButtonCmdInfo>(PRM.ButtonCmdInfoArr1);
                            l.Reverse();
                            foreach (ButtonCmdInfo inf in l)
                                if (inf != null && isAllowedButton(inf.CMD))
                                    panelButton1.Controls.Add(inf.getButton());
                        }
                        {
                            List<ButtonCmdInfo> l = new List<ButtonCmdInfo>(PRM.ButtonCmdInfoArr2);
                            l.Reverse();
                            foreach (ButtonCmdInfo inf in l)
                                if (inf != null && isAllowedButton(inf.CMD))
                                    panelButton2.Controls.Add(inf.getButton());
                        }

                        //
                        grid_.Dock = panelGrid_.Dock = DockStyle.Fill;
                        gridInfo_.Dock = PRM.INFO_TO_LEFT ? DockStyle.Left : DockStyle.Fill;
                        gridMsg_.Dock = DockStyle.Fill;

                        panelGrid_.Controls.Add(grid_);

                        panelTop.Controls.Add(panelGrid_);

                        if (PRM.INFO_TO_LEFT)
                        {
                            var panelGridInfo_ = new Panel();
                            panelGridInfo_.AutoSize = true;
                            panelGridInfo_.Dock = DockStyle.Left;




                            panelGridInfo_.Controls.Add(gridInfo_);



                            string fileImg_ = "res/avatez.png";
                            if (System.IO.File.Exists(fileImg_))
                            {

                                panelGridInfo_.Controls.Add(new Panel()
                                {

                                    Dock = DockStyle.Bottom,
                                    BackgroundImage = Image.FromStream(new System.IO.MemoryStream(System.IO.File.ReadAllBytes(fileImg_))),
                                    BackgroundImageLayout = ImageLayout.None,
                                    Height = 120

                                });

                                panelGridInfo_.Controls.Add(new Panel()
                                {

                                    Dock = DockStyle.Bottom,
                                    Height = 5

                                });
                            }


                            gridInfo_.Width = 150 + (int)(PRM.INFO_SCALE * 100);



                            panelTop.Controls.Add(panelGridInfo_);
                        }
                        else
                            panelBottomLeft.Controls.Add(gridInfo_);


                        panelBottomCenter.Controls.Add(gridMsg_);

                        if (!PRM.INFO_TO_LEFT)
                            panelBottom.Controls.Add(panelBottomLeft);
                        panelBottom.Controls.Add(panelBottomCenter);
                        // panelBottom.Controls.Add(panelBottomRight);

                        Controls.Add(panelTop);
                        if (panelButton1.Controls.Count > 0)
                            Controls.Add(panelButton1);
                        if (panelButton2.Controls.Count > 0)
                            Controls.Add(panelButton2);
                        Controls.Add(panelBottom);


                        panelBottom.Font = new System.Drawing.Font(panelBottom.Font.FontFamily, (float)(panelBottom.Font.Size * 1.2), panelBottom.Font.Style);

                        if (
                            PRM.TERM_TYPE == TERMTYPE.count ||
                            PRM.TERM_TYPE == TERMTYPE.barcode ||
                            PRM.TERM_TYPE == TERMTYPE.pricing ||
                            PRM.TERM_TYPE == TERMTYPE.production
                            )
                            panelBottomLeft.Visible = false;
                    }

                    bool isAllowedButton(string pCmd)
                    {
                        //1
                        //if (!ISEMPTYLREF(posTerminal.registers.docLRef))
                        //{

                        //    if (pCmd == "cmdprintcurr")
                        //        return true;
                        //}

                        //2
                        if (posTerminal._READONLY)
                        {
                            if (pCmd == "cmdcancel")
                                return true;

                            if (pCmd == "cmdprintcurr")
                                return true;

                            return false;
                        }
                        else
                        {

                            if (pCmd == "cmdprintcurr")
                                return false;

                        }


                        if (!PRM.MAT_FROM_LIST)
                        {

                            if (pCmd == "cmdadd")
                                return false;
                        }


                        if (!PRM.FULL_CLEAN)
                        {
                            if (pCmd == "cmdclear")
                                return false;

                        }

                        if (posTerminal._TRCODE == 3)
                        {

                            if (pCmd == "cmdslsret")
                                return false;
                        }

                        if (!PRM.USE_BONUS_MAQ)
                        {
                            if (pCmd == "cmdusebonus")
                                return false;
                        }

                        if (!PRM.USE_MANUAL_DISCOUNT)
                        {
                            if (pCmd == "cmdsetdiscountperc" || pCmd == "cmdsetdiscountamnt")
                                return false;
                        }
                        else
                        {
                            if (!PRM.USE_MANUAL_DISCOUNT_TOT)
                                if (pCmd == "cmdsetdiscountamnt")
                                    return false;

                            if (!PRM.USE_MANUAL_DISCOUNT_PERC)
                                if (pCmd == "cmdsetdiscountperc")
                                    return false;

                        }


                        if (string.IsNullOrEmpty(PRM.CASH_LIST) || (EXPLODELIST(PRM.CASH_LIST).Length == 1))
                        {
                            if (pCmd == "cmdchangecash")
                                return false;
                        }


                        if (!PRM.EDIT_VAT)
                        {
                            if (pCmd == "cmdchangeallvat")
                                return false;
                        }



                        return true;
                    }

                    void initDataGrid()
                    {
                        grid_ = new EDATAGRIDVIEW();
                        grid_.DataError += new DataGridViewDataErrorEventHandler(gridDataError);
                        grid_.AllowUserToResizeRows = false;
                        grid_.AllowUserToAddRows = false;
                        grid_.AutoGenerateColumns = false;
                        // grid_.BackgroundColor = SystemColors.Window;
                        grid_.AllowUserToDeleteRows = false;
                        grid_.MultiSelect = false;
                        grid_.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                        grid_.RowHeadersVisible = false;
                        grid_.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                        grid_.BorderStyle = BorderStyle.None;
                        grid_.RowTemplate.Height = PRM.GRID_ROW_H;
                        grid_.EditMode = DataGridViewEditMode.EditProgrammatically;
                        grid_.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        grid_.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;

                        grid_.ReadOnly = true;
                        grid_.AllowUserToResizeColumns = false;
                        grid_.ColumnHeadersHeight = (int)(PRM.GRID_ROW_H * 1.4);
                        grid_.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                        // grid_.Enabled = false;
                        grid_.RowPrePaint += grid__RowPrePaint;
                        grid_.CellPainting += grid__CellPainting;
                        grid_.CellFormatting += grid__CellFormatting;
                        grid_.CellMouseClick += grid__CellMouseClick;

                        TOOLSGRID.SETSTYLE(grid_);

                        string[] arrHeaderText = new string[]{
                            "     ",//10
                            "     ",//20
                            "     ",//30
                            _LANG.L.CODE, //40
                            _LANG.L.NAME, //40
                            _LANG.L.FROMDATE, //50
                            _LANG.L.TODATE, //60
                            _LANG.L.TEXT+"  ", //70
                            _LANG.L.SK,//80
                            "",//90
                            "%    ",//100
                            _LANG.L.QUANTITY, //110
                            _LANG.L.PRICE, //120
                            _LANG.L.TOTAL,//130
                            _LANG.L.VAT,//140

                            _LANG.L.PRICEPRCH,//150
                            _LANG.L.TOTALPRCH,//160
                            _LANG.L.DIFF,//170
                            
                          _LANG.L.REM, //180
                           _LANG.L.REMTOT, //190
                           _LANG.L.REMDIFF, //200

                           _LANG.L.REMTOTDIF, //210
                          _LANG.L.REMCENTER, //220
                          _LANG.L.REMALL, //230
                        };

                        string[] arrDataPropertyName = new string[]{
                            TABLE_DUMMY.COLS.INDX_COL ,
                            TABLE_STLINE.COLS.GLOBTRANS,
                            TABLE_STLINE.COLS.LINETYPE,
                            TABLE_STLINE.COLS.ITEMS_CODE, //1
                            TABLE_STLINE.COLS.ITEMS_NAME, //1
                            TABLE_STLINE.COLS.DATEBEG, //1
                            TABLE_STLINE.COLS.DATEEND, //1
                            TABLE_STLINE.COLS.LINEEXP, //1
                            TABLE_STLINE.COLS.SPECODE, //1
                            TABLE_STLINE.COLS.UNITSETL_CODE, //2
                            TABLE_STLINE.COLS.DISCPER, //100
                            TABLE_STLINE.COLS.AMOUNT, //110
                            TABLE_STLINE.COLS.PRICE, //120
                            TABLE_STLINE.COLS.TOTAL, //130
                            TABLE_STLINE.COLS.VAT, //140

                            TABLE_STLINE.COLS.PRCLIST_PRICE1,
                            TABLE_STLINE.COLS.DUMMY_FLOATF1,
                            TABLE_STLINE.COLS.DUMMY_FLOATF2,

                             TABLE_STLINE.COLS.DUMMY_ONHAND,
                             TABLE_STLINE.COLS.DUMMY_ONHAND_TOT,
                             TABLE_STLINE.COLS.DUMMY_ONHAND_DIFF,

                             TABLE_STLINE.COLS.DUMMY_ONHAND_DIFFTOT,
                              TABLE_STLINE.COLS.DUMMY_ONHANDMAIN,
                              TABLE_STLINE.COLS.DUMMY_ONHANDALL,
                        };

                        //int[] arrWidth = new int[]{
                        //        30,
                        //        350, //1
                        //        80, //2
                        //        100, //3
                        //        100, //4
                        //        100  //5

                        //    };
                        DataGridViewContentAlignment[] arrAlignment = new DataGridViewContentAlignment[]{
                            DataGridViewContentAlignment.MiddleRight,
                            DataGridViewContentAlignment.MiddleLeft,
                            DataGridViewContentAlignment.MiddleLeft,
                            DataGridViewContentAlignment.MiddleLeft, //1
                            DataGridViewContentAlignment.MiddleLeft, //1
                            DataGridViewContentAlignment.MiddleLeft, //1
                            DataGridViewContentAlignment.MiddleLeft, //1
                            DataGridViewContentAlignment.MiddleLeft, //1
                            DataGridViewContentAlignment.MiddleLeft, //2
                             DataGridViewContentAlignment.MiddleLeft, //2
                             DataGridViewContentAlignment.MiddleRight, //3
                            DataGridViewContentAlignment.MiddleRight, //3
                            DataGridViewContentAlignment.MiddleRight, //4
                            DataGridViewContentAlignment.MiddleRight, //5
                            DataGridViewContentAlignment.MiddleRight, //5
                            DataGridViewContentAlignment.MiddleRight, //5

                            DataGridViewContentAlignment.MiddleRight, //5
                            DataGridViewContentAlignment.MiddleRight, //5
                             DataGridViewContentAlignment.MiddleRight, //5

                            DataGridViewContentAlignment.MiddleRight, //5
                            DataGridViewContentAlignment.MiddleRight, //5
                            DataGridViewContentAlignment.MiddleRight, //5

                             DataGridViewContentAlignment.MiddleRight, //5
                             DataGridViewContentAlignment.MiddleRight, //5
                             DataGridViewContentAlignment.MiddleRight, //5
                        };
                        string[] arrFormat = new string[]{
                            "",
                            "",
                            "",
                            "", //1 
                            "", //1 
                            "yyyy-MM-dd HH:mm", //1 
                            "yyyy-MM-dd HH:mm", //1 
                            "", //1 
                            "", //2
                            "", //2
                            PRM.numberFormatGen2,  //100
                            PRM.numberFormatGen2,  //110
                            PRM.numberFormatGen,  //120
                            PRM.numberFormatGen,  //130
                            "0.##;-0.##; ",  //140
                            PRM.numberFormatGen, //5
                            
                            PRM.numberFormatGen, //5
                            PRM.numberFormatGen, //5
                            PRM.numberFormatGen2, //4

                            PRM.numberFormatGen2, //4
                            PRM.numberFormatGen2, //4
                            PRM.numberFormatGen2, //4

                             PRM.numberFormatGen2, //4
                              PRM.numberFormatGen2, //4
                             PRM.numberFormatGen2, //4
                        };
                        for (int colIndx_ = 0; colIndx_ < arrHeaderText.Length; ++colIndx_)
                        {

                            var colProp_ = arrDataPropertyName[colIndx_];

                            if (!PRM.USE_VAT)
                            {
                                if (colProp_ == TABLE_STLINE.COLS.VAT)
                                    continue;
                            }

                            if (!PRM.USE_ONHAND)
                            {
                                if (colProp_ == TABLE_STLINE.COLS.DUMMY_ONHAND)
                                    continue;
                            }
                            if (!PRM.USE_ONHAND_TOT)
                            {
                                if (colProp_ == TABLE_STLINE.COLS.DUMMY_ONHAND_TOT)
                                    continue;
                            }
                            if (!PRM.USE_ONHAND_DIFF)
                            {
                                if (colProp_ == TABLE_STLINE.COLS.DUMMY_ONHAND_DIFF)
                                    continue;
                                if (colProp_ == TABLE_STLINE.COLS.DUMMY_ONHAND_DIFFTOT)
                                    continue;
                            }
                            if (!PRM.USE_ONHAND_MAIN)
                            {
                                if (colProp_ == TABLE_STLINE.COLS.DUMMY_ONHANDMAIN)
                                    continue;
                            }
                            if (!PRM.USE_ONHAND_ALL)
                            {
                                if (colProp_ == TABLE_STLINE.COLS.DUMMY_ONHANDALL)
                                    continue;
                            }

                            if (!PRM.PRICE_DIFF_BY_PRCH)
                            {
                                if (colProp_ == TABLE_STLINE.COLS.DUMMY_FLOATF1)
                                    continue;
                                if (colProp_ == TABLE_STLINE.COLS.DUMMY_FLOATF2)
                                    continue;
                                if (colProp_ == TABLE_STLINE.COLS.PRCLIST_PRICE1)
                                    continue;
                            }

                            if (PRM.TERM_TYPE != TERMTYPE.hotel)
                            {
                                if (colProp_ == TABLE_STLINE.COLS.DATEBEG)
                                    continue;
                                if (colProp_ == TABLE_STLINE.COLS.DATEEND)
                                    continue;
                                if (colProp_ == TABLE_STLINE.COLS.SPECODE)
                                    continue;
                            }

                            if (PRM.TERM_TYPE == TERMTYPE.hotel)
                            {
                                if (colProp_ == TABLE_STLINE.COLS.UNITSETL_CODE)
                                    continue;
                                if (colProp_ == TABLE_STLINE.COLS.DISCPER)
                                    continue;
                            }

                            if (PRM.TERM_TYPE == TERMTYPE.count ||
                                PRM.TERM_TYPE == TERMTYPE.barcode ||
                                 PRM.TERM_TYPE == TERMTYPE.production ||
                                PRM.TERM_TYPE == TERMTYPE.pricing)
                            {
                                if (colProp_ == TABLE_STLINE.COLS.GLOBTRANS)
                                    continue;
                                if (colProp_ == TABLE_STLINE.COLS.LINETYPE)
                                    continue;

                                if (colProp_ == TABLE_STLINE.COLS.DATEBEG)
                                    continue;
                                if (colProp_ == TABLE_STLINE.COLS.DATEEND)
                                    continue;

                                if (colProp_ == TABLE_STLINE.COLS.DISCPER)
                                    continue;

                                if (colProp_ == TABLE_STLINE.COLS.UNITSETL_CODE)
                                    continue;
                            }


                            if (PRM.TERM_TYPE == TERMTYPE.pricing)
                            {
                                if (colProp_ == TABLE_STLINE.COLS.TOTAL)
                                    continue;
                                if (colProp_ == TABLE_STLINE.COLS.AMOUNT)
                                    continue;
                                if (colProp_ == TABLE_STLINE.COLS.UNITSETL_CODE)
                                    continue;
                            }

                            if (PRM.TERM_TYPE == TERMTYPE.restoran)
                            {
                                if (colProp_ == TABLE_STLINE.COLS.DISCPER)
                                    continue;
                            }

                            if (PRM.TERM_TYPE == TERMTYPE.magazin)
                            {
                                if (colProp_ == TABLE_STLINE.COLS.DISCPER)
                                    continue;

                                if (colProp_ == TABLE_STLINE.COLS.LINEEXP)
                                    continue;

                                if (colProp_ == TABLE_STLINE.COLS.UNITSETL_CODE)
                                    continue;
                            }


                            DataGridViewTextBoxColumn column_ = new DataGridViewTextBoxColumn();

                            column_.DataPropertyName = arrDataPropertyName[colIndx_];
                            column_.HeaderText = arrHeaderText[colIndx_];
                            //column_.Width = arrWidth[colIndx_];
                            column_.DefaultCellStyle.Alignment = arrAlignment[colIndx_];
                            column_.DefaultCellStyle.Format = arrFormat[colIndx_];
                            column_.SortMode = DataGridViewColumnSortMode.NotSortable;
                            column_.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                            if (column_.DataPropertyName == TABLE_STLINE.COLS.ITEMS_CODE)
                                column_.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                            if (column_.DataPropertyName == TABLE_STLINE.COLS.ITEMS_NAME)
                                column_.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;



                            switch (colProp_)
                            {
                                case TABLE_STLINE.COLS.GLOBTRANS:
                                case TABLE_STLINE.COLS.LINETYPE:
                                    column_.MinimumWidth = 25;
                                    break;
                                case TABLE_DUMMY.COLS.INDX_COL:
                                    column_.MinimumWidth = 35;
                                    break;
                                case TABLE_STLINE.COLS.VAT:
                                    column_.Width = 50;
                                    break;
                                default:
                                    column_.MinimumWidth = 80;
                                    break;

                            }


                            grid_.Columns.Add(column_);



                        }


                        tableData = TABLE_STLINE.TOOLS.createTable(posTerminal.PLUGIN);

                        if (PRM.TERM_TYPE == TERMTYPE.pricing)
                        {
                            var a = DataGridViewContentAlignment.MiddleRight;
                            var f = PRM.numberFormatGen;
                            TOOLSGRID.MY_ADD_COLUMN(grid_, "" + _LANG.L.NET + "\n" + _LANG.L.PRCHPRICE + "", PRICNGVARS.PL, a, f);
                            TOOLSGRID.MY_ADD_COLUMN(grid_, "" + _LANG.L.GROSS + "\n" + _LANG.L.PRCHPRICE + "", PRICNGVARS.PG, a, f);
                            TOOLSGRID.MY_ADD_COLUMN(grid_, "" + _LANG.L.PRCH + "\n" + _LANG.L.DISC + " %", PRICNGVARS.PD, a, f);
                            TOOLSGRID.MY_ADD_COLUMN(grid_, "+ " + _LANG.L.COST + "", PRICNGVARS.PC, a, f);
                            TOOLSGRID.MY_ADD_COLUMN(grid_, "" + _LANG.L.AVG + "\n" + _LANG.L.PRCHPRICE + "", PRICNGVARS.PA, a, f);
                            TOOLSGRID.MY_ADD_COLUMN(grid_, "" + _LANG.L.PRCHPRICE + "\n" + _LANG.L.LIST + "", PRICNGVARS.PP, a, f);
                            TOOLSGRID.MY_ADD_COLUMN(grid_, "" + _LANG.L.SLSPRICE + "\n" + _LANG.L.LIST + "", PRICNGVARS.SP, a, f);
                            TOOLSGRID.MY_ADD_COLUMN(grid_, _LANG.L.PRICE + " %", PRICNGVARS.FF, a, f);


                        }


                        _PLUGIN.TAB_FILLNULL(tableData);

                        tableData.ColumnChanged += tableData_ColumnChanged;

                        grid_.DataSource = tableData;

                    }



                    void tableData_ColumnChanged(object sender, DataColumnChangeEventArgs e)
                    {
                        if (!TAB_ROWDELETED(e.Row))
                            columnChanged(e.Column.ColumnName, e.Row);
                    }

                    private void columnChanged(string pCol, DataRow pRow)
                    {
                        if (this.posTerminal == null || this.posTerminal.PLUGIN == null)
                            return;

                        this.posTerminal.dataTableColumnChanged(pCol, pRow);

                    }

                    void grid__CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
                    {

                        if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                        {
                            if (grid_.Columns[e.ColumnIndex].DataPropertyName == TABLE_STLINE.COLS.AMOUNT)
                                posTerminal.changeAmount(TOOLSGRID.MY_GET_GRID_ROW_DATA(grid_, e.RowIndex));
                            else
                                if (grid_.Columns[e.ColumnIndex].DataPropertyName == TABLE_STLINE.COLS.PRICE)
                                    posTerminal.changePrice(TOOLSGRID.MY_GET_GRID_ROW_DATA(grid_, e.RowIndex));
                                else
                                    if (grid_.Columns[e.ColumnIndex].DataPropertyName == TABLE_STLINE.COLS.LINEEXP)
                                        posTerminal.changeLineExp(TOOLSGRID.MY_GET_GRID_ROW_DATA(grid_, e.RowIndex));
                                    else
                                        if (grid_.Columns[e.ColumnIndex].DataPropertyName == TABLE_STLINE.COLS.DISCPER)
                                            posTerminal.changeLineDisc(TOOLSGRID.MY_GET_GRID_ROW_DATA(grid_, e.RowIndex));
                                        else
                                            if (grid_.Columns[e.ColumnIndex].DataPropertyName == PRICNGVARS.FF)
                                                posTerminal.changeMatCoif1(TOOLSGRID.MY_GET_GRID_ROW_DATA(grid_, e.RowIndex), -1);
                                            else
                                                if (grid_.Columns[e.ColumnIndex].DataPropertyName == TABLE_STLINE.COLS.ITEMS_NAME)
                                                    posTerminal.activateRecord(TOOLSGRID.MY_GET_GRID_ROW_DATA(grid_, e.RowIndex));
                                                else
                                                    if (grid_.Columns[e.ColumnIndex].DataPropertyName == TABLE_STLINE.COLS.SPECODE)
                                                        posTerminal.changeLineSpecode(TOOLSGRID.MY_GET_GRID_ROW_DATA(grid_, e.RowIndex));
                                                    else
                                                        if (grid_.Columns[e.ColumnIndex].DataPropertyName == TABLE_STLINE.COLS.VAT)
                                                            posTerminal.changeLineVAT(TOOLSGRID.MY_GET_GRID_ROW_DATA(grid_, e.RowIndex));
                        }
                    }

                    void grid__CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
                    {
                        string prop_ = e.ColumnIndex >= 0 ? grid_.Columns[e.ColumnIndex].DataPropertyName : "";
                        DataRow row_ = e.RowIndex >= 0 ? TOOLSGRID.MY_GET_GRID_ROW_DATA(grid_, e.RowIndex) : null;

                        if (prop_ == TABLE_DUMMY.COLS.INDX_COL)
                        {
                            e.Value = _PLUGIN.FORMAT(e.RowIndex + 1);
                        }

                        if (prop_ == TABLE_STLINE.COLS.LINETYPE && row_ != null)
                        {
                            short s_ = _PLUGIN.CASTASSHORT(_PLUGIN.TAB_GETROW(row_, TABLE_STLINE.COLS.LINETYPE));

                            if (TABLE_STLINE.LINETYPE_DESC.ContainsKey(s_))
                                e.Value = TABLE_STLINE.LINETYPE_DESC[s_];
                        }

                        if (prop_ == TABLE_STLINE.COLS.GLOBTRANS && row_ != null)
                        {
                            short s_ = _PLUGIN.CASTASSHORT(_PLUGIN.TAB_GETROW(row_, TABLE_STLINE.COLS.GLOBTRANS));

                            if (TABLE_STLINE.GLOBTRANS_DESC.ContainsKey(s_))
                                e.Value = TABLE_STLINE.GLOBTRANS_DESC[s_];
                        }
                    }


                    void initInfoGrid()
                    {

                        gridInfo_ = new EDATAGRIDVIEW();
                        gridInfo_.DataError += new DataGridViewDataErrorEventHandler(gridDataError);
                        gridInfo_.AllowUserToResizeRows = false;
                        gridInfo_.AutoGenerateColumns = false;
                        gridInfo_.AllowUserToAddRows = false;
                        // gridInfo_.BackgroundColor = SystemColors.Window;
                        gridInfo_.AllowUserToDeleteRows = false;
                        gridInfo_.MultiSelect = false;
                        gridInfo_.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                        gridInfo_.ColumnHeadersVisible = gridInfo_.RowHeadersVisible = false;
                        // gridInfo_.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                        gridInfo_.BorderStyle = BorderStyle.None;
                        gridInfo_.RowTemplate.Height = PRM.GRID_ROW_H;
                        gridInfo_.EditMode = DataGridViewEditMode.EditProgrammatically;
                        gridInfo_.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        gridInfo_.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;

                        TOOLSGRID.SETSTYLE(gridInfo_);

                        gridInfo_.ReadOnly = true;
                        gridInfo_.DefaultCellStyle.SelectionBackColor = gridInfo_.DefaultCellStyle.BackColor;
                        gridInfo_.DefaultCellStyle.SelectionForeColor = gridInfo_.DefaultCellStyle.ForeColor;
                        gridInfo_.Enabled = false;


                        gridInfo_.RowTemplate.Height = (int)(PRM.GRID_ROW_H * PRM.INFO_SCALE);



                        gridInfo_.RowPrePaint += gridInfo__RowPrePaint;
                        string[] arrHeaderText = new string[]{
                            "", //1
                            "" //2
                        };

                        string[] arrDataPropertyName = new string[]{
                            TABLE_INFO.COLS.NAME, //1
                            TABLE_INFO.COLS.VALUE //2
                        };

                        int[] arrWidth = new int[]{
                            110, //1
                            210, //2
                        };
                        DataGridViewContentAlignment[] arrAlignment = new DataGridViewContentAlignment[]{
                            DataGridViewContentAlignment.MiddleLeft, //1
                            DataGridViewContentAlignment.MiddleRight //2
                        };
                        string[] arrFormat = new string[]{
                            "", //1 
                            "" //2
                        };
                        for (int colIndx_ = 0; colIndx_ < arrHeaderText.Length; ++colIndx_)
                        {
                            DataGridViewTextBoxColumn column_ = new DataGridViewTextBoxColumn();
                            column_.DataPropertyName = arrDataPropertyName[colIndx_];
                            column_.HeaderText = arrHeaderText[colIndx_];
                            column_.Width = arrWidth[colIndx_];
                            column_.DefaultCellStyle.Alignment = arrAlignment[colIndx_];
                            column_.DefaultCellStyle.Format = arrFormat[colIndx_];
                            column_.SortMode = DataGridViewColumnSortMode.NotSortable;
                            gridInfo_.Columns.Add(column_);

                            switch (column_.DataPropertyName)
                            {
                                case TABLE_INFO.COLS.CODE:

                                    break;
                                case TABLE_INFO.COLS.NAME:
                                    column_.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                                    break;
                                case TABLE_INFO.COLS.VALUE:
                                    column_.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                                    var f_ = column_.InheritedStyle.Font;
                                    column_.DefaultCellStyle.Font = new Font(f_.Name, f_.Size * (float)PRM.INFO_SCALE * 2.1F, f_.Style);
                                    break;


                            }
                        }
                        //
                        tableInfo = new DataTable();
                        tableInfo.Columns.Add(TABLE_INFO.COLS.CODE, TABLE_INFO.TYPES.CODE);
                        tableInfo.Columns.Add(TABLE_INFO.COLS.NAME, TABLE_INFO.TYPES.NAME);
                        tableInfo.Columns.Add(TABLE_INFO.COLS.VALUE, TABLE_INFO.TYPES.VALUE);
                        //

                        bool hideTot_ = false;
                        bool hideDisc_ = false;
                        bool hidePayment_ = false;
                        bool hideChange_ = false;
                        bool hideVAT_ = false;

                        if (PRM.TERM_TYPE == TERMTYPE.restoran)
                        {
                            hideTot_ = false;
                            hideDisc_ = true;
                            hidePayment_ = true;
                            hideChange_ = true;
                            hideVAT_ = true;
                        }

                        if (
                            PRM.TERM_TYPE == TERMTYPE.count ||
                            PRM.TERM_TYPE == TERMTYPE.barcode ||
                            PRM.TERM_TYPE == TERMTYPE.pricing ||
                            PRM.TERM_TYPE == TERMTYPE.production
                            )
                        {
                            hideTot_ = true;
                            hideDisc_ = true;
                            hidePayment_ = true;
                            hideChange_ = true;
                            hideVAT_ = true;
                        }


                        tableInfo.Rows.Add(new object[] { TABLE_INFO.CODES.TOTALNET, hideTot_ ? "" : "" + _LANG.L.TOTAL + "", "" });
                        if (!PRM.HIDE_DISC)
                            tableInfo.Rows.Add(new object[] { TABLE_INFO.CODES.DISCOUNT, hideDisc_ ? "" : _LANG.L.DISC + "", "" });
                        if (PRM.USE_VAT)
                            tableInfo.Rows.Add(new object[] { TABLE_INFO.CODES.VAT, hideVAT_ ? "" : _LANG.L.VAT + "", "" });

                        tableInfo.Rows.Add(new object[] { TABLE_INFO.CODES.PAYMENT, hidePayment_ ? "" : "" + _LANG.L.CASHMONEY + "", "" });
                        tableInfo.Rows.Add(new object[] { TABLE_INFO.CODES.CHANGE, hideChange_ ? "" : "" + _LANG.L.REM + "", "" });
                        // tableInfo.Rows.Add(new object[] { TABLE_INFO.CODES.DUMMY, "", "" });
                        //
                        gridInfo_.DataSource = tableInfo;
                        //



                    }

                    void initMsgGrid()
                    {

                        gridMsg_ = new EDATAGRIDVIEW();
                        gridMsg_.DataError += new DataGridViewDataErrorEventHandler(gridDataError);
                        gridMsg_.AllowUserToResizeRows = false;
                        gridMsg_.AutoGenerateColumns = false;
                        gridMsg_.AllowUserToAddRows = false;
                        //  gridMsg_.BackgroundColor = SystemColors.Window;
                        gridMsg_.AllowUserToDeleteRows = false;
                        gridMsg_.MultiSelect = false;
                        gridMsg_.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                        gridMsg_.ColumnHeadersVisible = gridMsg_.RowHeadersVisible = false;
                        // gridMsg_.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                        gridMsg_.BorderStyle = BorderStyle.None;
                        gridMsg_.RowTemplate.Height = PRM.GRID_ROW_H;
                        gridMsg_.EditMode = DataGridViewEditMode.EditProgrammatically;
                        gridMsg_.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        gridMsg_.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;

                        TOOLSGRID.SETSTYLE(gridMsg_);

                        gridMsg_.ReadOnly = true;
                        gridMsg_.DefaultCellStyle.SelectionBackColor = gridMsg_.DefaultCellStyle.BackColor;
                        gridMsg_.DefaultCellStyle.SelectionForeColor = gridMsg_.DefaultCellStyle.ForeColor;
                        gridMsg_.Enabled = false;

                        gridMsg_.RowPrePaint += gridMsg__RowPrePaint;
                        string[] arrHeaderText = new string[]{
                            "" //1
                        };

                        string[] arrDataPropertyName = new string[]{
                            TABLE_MSG.COLS.VALUE //1
                        };

                        int[] arrWidth = new int[]{
                            100, //2
                        };
                        DataGridViewContentAlignment[] arrAlignment = new DataGridViewContentAlignment[]{
                            DataGridViewContentAlignment.MiddleRight //2
                        };
                        string[] arrFormat = new string[]{
                            "" //1 
                        };
                        for (int colIndx_ = 0; colIndx_ < arrHeaderText.Length; ++colIndx_)
                        {
                            DataGridViewTextBoxColumn column_ = new DataGridViewTextBoxColumn();
                            column_.DataPropertyName = arrDataPropertyName[colIndx_];
                            column_.HeaderText = arrHeaderText[colIndx_];
                            column_.Width = arrWidth[colIndx_];
                            column_.DefaultCellStyle.Alignment = arrAlignment[colIndx_];
                            column_.DefaultCellStyle.Format = arrFormat[colIndx_];
                            column_.SortMode = DataGridViewColumnSortMode.NotSortable;
                            gridMsg_.Columns.Add(column_);

                            switch (column_.DataPropertyName)
                            {
                                case TABLE_MSG.COLS.CODE:

                                    break;
                                case TABLE_MSG.COLS.VALUE:
                                    column_.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                                    break;


                            }
                        }
                        //
                        tableMsg = new DataTable();
                        tableMsg.Columns.Add(TABLE_MSG.COLS.CODE, TABLE_MSG.TYPES.CODE);
                        tableMsg.Columns.Add(TABLE_MSG.COLS.VALUE, TABLE_MSG.TYPES.VALUE);
                        //
                        tableMsg.Rows.Add(new object[] { TABLE_MSG.CODES.MSG1, "" });
                        tableMsg.Rows.Add(new object[] { TABLE_MSG.CODES.MSG2, "" });
                        tableMsg.Rows.Add(new object[] { TABLE_MSG.CODES.MSG3, "" });
                        tableMsg.Rows.Add(new object[] { TABLE_MSG.CODES.MSG4, "" });
                        tableMsg.Rows.Add(new object[] { TABLE_MSG.CODES.MSG5, "" });
                        gridMsg_.DataSource = tableMsg;
                    }

                    void grid__RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
                    {
                        DataGridViewRow row_ = getDataRecordWrap(e.RowIndex);
                        if (row_ == null)
                            return;

                        DataRow rowData_ = TOOLSGRID.MY_GET_GRID_ROW_DATA(row_);


                        {
                            // row_.DefaultCellStyle.SelectionBackColor = 
                            Color c = (e.RowIndex % 2 == 0) ? this.BackColor : SystemColors.ControlLight;
                            if (row_.DefaultCellStyle.BackColor != c)
                                row_.DefaultCellStyle.BackColor = c;
                        }

                        if (posTerminal.isReadOnly(rowData_))
                            TOOLSGRID.MY_SETSTYLECOLORTEXT(row_.DefaultCellStyle, SystemColors.Highlight);

                    }

                    void grid__CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
                    {

                        if (e.ColumnIndex < 0)
                            return;
                        if (e.RowIndex < 0)
                            return;

                        var grid_ = sender as DataGridView;
                        if (grid_ == null)
                            return;

                        var col_ = grid_.Columns[e.ColumnIndex];
                        if (col_ == null)
                            return;
                        var row_ = getDataRecordWrap(e.RowIndex);
                        if (row_ == null)
                            return;

                        var rowData_ = TOOLSGRID.MY_GET_GRID_ROW_DATA(row_);

                        if (col_.DataPropertyName == TABLE_STLINE.COLS.AMOUNT)
                            TOOLSGRID.MY_SETSTYLECOLORTEXT(e.CellStyle, Color.Green);

                        if (col_.DataPropertyName == TABLE_STLINE.COLS.VAT)
                            TOOLSGRID.MY_SETSTYLECOLORTEXT(e.CellStyle, Color.Gray);

                        if (PRM.NEGATIVE_LIGHT && PRM.USE_ONHAND)
                        {
                            if (col_.DataPropertyName == TABLE_STLINE.COLS.DUMMY_ONHAND)
                            {


                                var onhand_ = CASTASDOUBLE(TAB_GETROW(rowData_, TABLE_STLINE.COLS.DUMMY_ONHAND));

                                if (onhand_ < 0)
                                    TOOLSGRID.MY_SETSTYLECOLOR(e.CellStyle, Color.Red);


                            }
                        }

                        if (PRM.TERM_TYPE == TERMTYPE.pricing)
                        {




                            if (col_.DataPropertyName == TABLE_STLINE.COLS.PRICE)//last purch 
                                TOOLSGRID.MY_SETSTYLECOLORTEXT(e.CellStyle, Color.Green);

                            if (col_.DataPropertyName == PRICNGVARS.PL)//last purch 
                                TOOLSGRID.MY_SETSTYLECOLORTEXT(e.CellStyle, Color.MediumOrchid);
                            if (col_.DataPropertyName == PRICNGVARS.FF)//float f1 
                                TOOLSGRID.MY_SETSTYLECOLORTEXT(e.CellStyle, Color.Teal);
                            if (col_.DataPropertyName == PRICNGVARS.PP)//purch price list 
                                TOOLSGRID.MY_SETSTYLECOLORTEXT(e.CellStyle, Color.Olive);
                            if (col_.DataPropertyName == PRICNGVARS.SP)//sale pricelist 
                                TOOLSGRID.MY_SETSTYLECOLORTEXT(e.CellStyle, Color.Firebrick);


                            if (col_.DataPropertyName == TABLE_STLINE.COLS.PRICE)
                            {
                                var val_ = CASTASDOUBLE(TAB_GETROW(rowData_, TABLE_STLINE.COLS.PRICE));
                                var valSp_ = CASTASDOUBLE(TAB_GETROW(rowData_, PRICNGVARS.SP));

                                if (ISNUMZERO(val_) || ISNUMLESS(val_, valSp_))
                                    TOOLSGRID.MY_SETSTYLECOLOR(e.CellStyle, Color.Red);

                            }
                        }

                    }


                    void gridMsg__RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
                    {
                        DataGridViewRow row_ = getMsgRecordWrap(e.RowIndex);
                        if (row_ == null)
                            return;

                        DataRow rowData_ = getBoundRow(row_);
                        if (rowData_ == null)
                            return;

                        string CODE_ = CASTASSTRING(ISNULL(TAB_GETROW(rowData_, TABLE_MSG.COLS.CODE), ""));
                        switch (CODE_)
                        {
                            case TABLE_MSG.CODES.MSG1:
                                if (row_.DefaultCellStyle.ForeColor != Color.Black)
                                    row_.DefaultCellStyle.SelectionForeColor = row_.DefaultCellStyle.ForeColor = Color.Black;
                                break;
                            case TABLE_MSG.CODES.MSG2:
                            case TABLE_MSG.CODES.MSG3:
                                if (row_.DefaultCellStyle.ForeColor != Color.Blue)
                                    row_.DefaultCellStyle.SelectionForeColor = row_.DefaultCellStyle.ForeColor = Color.Blue;
                                break;
                            case TABLE_MSG.CODES.MSG4:
                            case TABLE_MSG.CODES.MSG5:
                                if (row_.DefaultCellStyle.ForeColor != Color.Green)
                                    row_.DefaultCellStyle.SelectionForeColor = row_.DefaultCellStyle.ForeColor = Color.Green;
                                break;
                        }
                    }
                    void gridInfo__RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
                    {
                        DataGridViewRow row_ = getInfoRecordWrap(e.RowIndex);
                        if (row_ == null)
                            return;



                        DataRow rowData_ = getBoundRow(row_);
                        if (rowData_ == null)
                            return;

                        string CODE_ = CASTASSTRING(ISNULL(TAB_GETROW(rowData_, TABLE_INFO.COLS.CODE), ""));
                        switch (CODE_)
                        {
                            case TABLE_INFO.CODES.TOTALNET:
                                if (row_.DefaultCellStyle.ForeColor != Color.Blue)
                                    row_.DefaultCellStyle.SelectionForeColor = row_.DefaultCellStyle.ForeColor = Color.Blue;


                                break;
                            case TABLE_INFO.CODES.DISCOUNT:

                                break;

                        }
                    }



                    void gridDataError(object sender, DataGridViewDataErrorEventArgs e)
                    {

                    }


                    public DataGridViewRow getDataRecordWrap(int pIndx)
                    {
                        return TOOLSGRID.MY_GET_GRID_ROW(grid_, pIndx);
                    }

                    public DataGridViewRow getInfoRecordWrap(int pIndx)
                    {
                        return TOOLSGRID.MY_GET_GRID_ROW(gridInfo_, pIndx);
                    }

                    public DataGridViewRow getMsgRecordWrap(int pIndx)
                    {
                        return TOOLSGRID.MY_GET_GRID_ROW(gridMsg_, pIndx);
                    }

                    public DataRow getBoundRow(DataGridViewRow pRow)
                    {
                        return TOOLSGRID.MY_GET_GRID_ROW_DATA(pRow);

                    }
                    public DataRow getCurrentRecord()
                    {
                        return getBoundRow(getCurrentRecordWarp());
                    }
                    public DataGridViewRow getCurrentRecordWarp()
                    {
                        return TOOLSGRID.MY_GET_GRID_ROW(grid_);

                    }
                    public DataRow searchInfoRecord(string pDataCol, object pVal)
                    {
                        return getBoundRow(searchInfoRecordWrap(pDataCol, pVal));
                    }
                    public DataGridViewRow searchInfoRecordWrap(string pDataCol, object pVal)
                    {
                        foreach (DataGridViewRow row_ in gridInfo_.Rows)
                        {
                            DataRow dataRow_ = getBoundRow(row_);
                            if (dataRow_ != null)
                                if (COMPARE(pVal, TAB_GETROW(dataRow_, pDataCol)))
                                    return row_;
                        }
                        return null;
                    }




                    public void activateMainGrid()
                    {
                        this.ActiveControl = grid_;
                    }

                    public void activateMainGridRow(int pIndx)
                    {
                        TOOLSGRID.MY_SET_GRID_POSITION(grid_, pIndx, null);

                    }

                    public void activateMainGridRow(DataRow pRow)
                    {
                        TOOLSGRID.MY_SET_GRID_POSITION(grid_, pRow, null);
                    }



                    public void KeyProcess(Keys keys)
                    {
                        this.OnKeyDown(new KeyEventArgs(keys));
                    }
                    public void CmdProcess(string pCmd)
                    {
                        switch (pCmd)
                        {
                            case "cmdpricingperc2all":

                                if (PRM.TERM_TYPE == TERMTYPE.pricing)
                                {
                                    var val_ = posTerminal.askNumber("%", 0, 2);

                                    if (val_ >= 0)
                                    {
                                        foreach (DataRow r in tableData.Rows)
                                            posTerminal.changeMatCoif1(r, val_);
                                    }


                                }

                                break;


                        }
                    }


                    public bool deleteRecord(_PLUGIN pPLUGIN)
                    {
                        DataRow row_ = TOOLSGRID.MY_GET_GRID_ROW_DATA(grid_);
                        if (posTerminal.isReadOnly(row_))
                            return false;

                        List<DataRow> list = new List<DataRow>();
                        list.Add(row_);

                        if (TABLE_STLINE.TOOLS.isLocalMat(row_))
                        {
                            list.AddRange(TABLE_STLINE.TOOLS.getSubLines(row_, TABLE_STLINE.LINETYPE.discount));
                            list.AddRange(TABLE_STLINE.TOOLS.getSubLines(row_, TABLE_STLINE.LINETYPE.promotion));
                        }

                        foreach (var r in list)
                        {
                            var force_ = !TABLE_STLINE.TOOLS.isLocalMat(r);
                            TOOLSGRID.MY_DEL_ROW(pPLUGIN, r, TABLE_STLINE.COLS.ITEMS_NAME, force_);

                        }
                        return true;
                    }

                    public void gridPosition(int pPos)
                    {
                        TOOLSGRID.MY_SET_GRID_POSITION(grid_, pPos, string.Empty);
                    }
                    public void gridPosition(DataRow pRow)
                    {
                        TOOLSGRID.MY_SET_GRID_POSITION(grid_, pRow, string.Empty);
                    }
                    protected override void Dispose(bool disposing)
                    {
                        base.Dispose(disposing);

                        posTerminal = null;

                        tableData = null;
                        tableInfo = null;
                        tableMsg = null;

                        grid_ = null;
                        gridInfo_ = null;
                        gridMsg_ = null;
                    }
                }


                public void BEGIN_TERMINAL(object pDocLRef)
                {
                    if (dataInputForm_ != null)
                        return;


                    if (!ISEMPTYLREF(pDocLRef))
                    {
                        if (PRM.DOCUMENTS_READONLY)
                            this._READONLY = true;
                    }

                    object parentDocLRef = null;

                    if (ISEMPTYLREF(pDocLRef))
                        if (
                            (PRM.TERM_TYPE == TERMTYPE.magazin) ||
                             (PRM.TERM_TYPE == TERMTYPE.magazinWholesale) ||
                             (PRM.TERM_TYPE == TERMTYPE.magazinWholesaleSlsman)
                            )
                        {

                            if (_TRCODE == 3)
                            {

                                if (!askPassword())
                                    return;

                                //if (PRM.RETURN_BY_PARENT_DOC)
                                //{
                                //    while (true)
                                //    {

                                //        var docNr_ = askString("" + _LANG.L.DOCNR + "", "");
                                //        if (docNr_ == "" || docNr_ == null)
                                //            return;

                                //        if (PRM.PARENT_OFFLINE)
                                //        {
                                //            //load from main
                                //            SQL("exec  [dbo].[p_MARKET_$FIRM$_$PERIOD$_GET_PARENT_DOC] @P1 ", new object[] { docNr_ });


                                //        }

                                //        object docRef_ = SQLSCALAR("SELECT LOGICALREF FROM LG_$FIRM$_$PERIOD$_INVOICE WITH(NOLOCK) WHERE FICHENO = @P1 AND TRCODE = 8", new object[] { docNr_ });

                                //        if (ISEMPTYLREF(docRef_))
                                //        {
                                //            MSGUSERINFO(_LANG.L.MSG_ERROR_NO_DATA);
                                //        }
                                //        else
                                //        {
                                //            parentDocLRef = docRef_;
                                //            //   backUpDoc();
                                //            break;
                                //        }

                                //    }
                                //}
                            }

                        }

                    dataInputForm_ = new TerminalForm(this);
                    //
                    dataInputForm_.KeyPress += new KeyPressEventHandler(keyPress);
                    dataInputForm_.KeyDown += new KeyEventHandler(keyDown);
                    dataInputForm_.FormClosing += new FormClosingEventHandler(formClosing);
                    dataInputForm_.FormClosed += formClosed;
                    //
                    tableData_ = dataInputForm_.tableData;
                    dataSet_ = new DataSet();
                    //
                    dataSet_.Tables.Add(tableData_);
                    dataSet_.Tables.Add(tableDataHeader_ = TABLE_INVOICE.TOOLS.createTable(PLUGIN));
                    //
                    tableInfo_ = dataInputForm_.tableInfo;
                    tableMsg_ = dataInputForm_.tableMsg;
                    //
                    tableData_.ColumnChanged += tableData__ColumnChanged;
                    tableData_.RowChanged += tableData__RowChanged;
                    //

                    prepareForNew();
                    //
                    registers.parentDocLRef = parentDocLRef;
                    //
                    //
                    if (!ISEMPTYLREF(pDocLRef))
                    {
                        loadDoc(pDocLRef, registers, tableData_);

                    }


                    //
                    dataInputForm_.Show();
                    //
                    // dataInputForm_.Activate();
                    //  dataInputForm_.WindowState = FormWindowState.Normal;
                    // dataInputForm_.BringToFront();


                }

                void loadDoc(object pDocLRef, Registers pRegisters, DataTable pTable)
                {
                    try
                    {


                        DataTable H = SQL(
                            MY_CHOOSE_SQL(
        @"
select top(1) 
* ,
(SELECT TOP(1) CODE FROM LG_SLSMAN M WITH(NOLOCK) WHERE M.LOGICALREF = I.SALESMANREF AND M.FIRMNR = $FIRM$) SLSMAN_CODE,
(SELECT TOP(1) DEFINITION_ FROM LG_SLSMAN M WITH(NOLOCK) WHERE M.LOGICALREF = I.SALESMANREF AND M.FIRMNR = $FIRM$) SLSMAN_DEFINITION_,
(SELECT TOP(1) DEFINITION_ FROM LG_$FIRM$_CLCARD C WITH(NOLOCK) WHERE C.LOGICALREF = I.CLIENTREF) CLCARD_DEFINITION_,
(SELECT TOP(1) CODE FROM LG_$FIRM$_CLCARD C WITH(NOLOCK) WHERE C.LOGICALREF = I.CLIENTREF) CLCARD_CODE
from 
LG_$FIRM$_$PERIOD$_INVOICE I with(nolock) 
where LOGICALREF = @P1
",
         @"
select 
* ,
(SELECT CODE FROM LG_SLSMAN M WITH(NOLOCK) WHERE M.LOGICALREF = I.SALESMANREF AND M.FIRMNR = $FIRM$ LIMIT 1) SLSMAN_CODE,
(SELECT DEFINITION_ FROM LG_SLSMAN M WITH(NOLOCK) WHERE M.LOGICALREF = I.SALESMANREF AND M.FIRMNR = $FIRM$ LIMIT 1) SLSMAN_DEFINITION_,
(SELECT DEFINITION_ FROM LG_$FIRM$_CLCARD C WITH(NOLOCK) WHERE C.LOGICALREF = I.CLIENTREF LIMIT 1) CLCARD_DEFINITION_,
(SELECT CODE FROM LG_$FIRM$_CLCARD C WITH(NOLOCK) WHERE C.LOGICALREF = I.CLIENTREF LIMIT 1) CLCARD_CODE
from 
LG_$FIRM$_$PERIOD$_INVOICE I  
where LOGICALREF = @P1
LIMIT 1
"),
        new object[] { pDocLRef, PRM.CYPHCODE });
                        DataTable L = SQL(
                            MY_CHOOSE_SQL(
        @"select 
*,
(case when L.LINETYPE in (0,1) then (select top(1) C.CODE from LG_$FIRM$_ITEMS C with(nolock) where C.LOGICALREF = L.STOCKREF ) else '' end) ITEMS_CODE,
(case when L.LINETYPE in (0,1) then (select top(1) C.NAME from LG_$FIRM$_ITEMS C with(nolock) where C.LOGICALREF = L.STOCKREF ) else '' end) ITEMS_NAME,
(select top(1) U.CODE from LG_$FIRM$_UNITSETL U with(nolock) where U.LOGICALREF = L.UOMREF) UNITSETL_CODE
from LG_$FIRM$_$PERIOD$_STLINE L with(nolock) 
where INVOICEREF = @P1 
order by INVOICEREF asc,INVOICELNNO asc
",
    @"select 
*,
(case when L.LINETYPE in (0,1) then (select C.CODE from LG_$FIRM$_ITEMS C where C.LOGICALREF = L.STOCKREF LIMIT 1) else '' end) ITEMS_CODE,
(case when L.LINETYPE in (0,1) then (select C.NAME from LG_$FIRM$_ITEMS C  where C.LOGICALREF = L.STOCKREF LIMIT 1) else '' end) ITEMS_NAME,
(select U.CODE from LG_$FIRM$_UNITSETL U where U.LOGICALREF = L.UOMREF  LIMIT 1) UNITSETL_CODE
from LG_$FIRM$_$PERIOD$_STLINE L  
where INVOICEREF = @P1 
order by INVOICEREF asc,INVOICELNNO asc
"),
        new object[] { pDocLRef });


                        TAB_FILLNULL(H);
                        TAB_FILLNULL(L);

                        foreach (DataRow row in H.Rows)
                        {
                            pRegisters.docLRef = TAB_GETROW(row, TABLE_INVOICE.COLS.LOGICALREF);
                            pRegisters.slsMan = TAB_GETROW(row, TABLE_INVOICE.COLS.SALESMANREF);
                            //pRegisters.clientLRef = TAB_GETROW(row, TABLE_INVOICE.COLS.CLIENTREF);
                            pRegisters.clcard.setClientRef(TAB_GETROW(row, TABLE_INVOICE.COLS.CLIENTREF), PLUGIN);
                            pRegisters.trackno = CASTASSTRING(TAB_GETROW(row, TABLE_INVOICE.COLS.DOCTRACKINGNR));
                            pRegisters.setDocCode(CASTASSTRING(TAB_GETROW(row, TABLE_INVOICE.COLS.FICHENO)));
                            pRegisters.speCode = CASTASSTRING(TAB_GETROW(row, TABLE_INVOICE.COLS.SPECODE));
                            pRegisters.cyhpCode = CASTASSTRING(TAB_GETROW(row, TABLE_INVOICE.COLS.CYPHCODE));
                            pRegisters.setDocDate(MY_CONVERTDATE(CASTASDATE(TAB_GETROW(row, TABLE_INVOICE.COLS.DATE_)), CASTASINT(TAB_GETROW(row, TABLE_INVOICE.COLS.TIME_))));
                            pRegisters.desc1 = CASTASSTRING(TAB_GETROW(row, TABLE_INVOICE.COLS.GENEXP1));
                            pRegisters.desc2 = CASTASSTRING(TAB_GETROW(row, TABLE_INVOICE.COLS.GENEXP2));
                            pRegisters.desc3 = CASTASSTRING(TAB_GETROW(row, TABLE_INVOICE.COLS.GENEXP3));
                            pRegisters.desc4 = CASTASSTRING(TAB_GETROW(row, TABLE_INVOICE.COLS.GENEXP4));
                            pRegisters.docCode = CASTASSTRING(TAB_GETROW(row, TABLE_INVOICE.COLS.DOCODE));

                            pRegisters.slsManCode = CASTASSTRING(TAB_GETROW(row, TABLE_INVOICE.COLS.SLSMAN_CODE));
                            pRegisters.slsManDesc = CASTASSTRING(TAB_GETROW(row, TABLE_INVOICE.COLS.SLSMAN_DEFINITION_));

                            // pRegisters.clientCode = CASTASSTRING(TAB_GETROW(row, TABLE_INVOICE.COLS.CLCARD_CODE));


                            _TRCODE = CASTASSHORT(TAB_GETROW(row, TABLE_INVOICE.COLS.TRCODE));
                            _CANCELLED = (CASTASSHORT(TAB_GETROW(row, TABLE_INVOICE.COLS.CANCELLED)) != 0);
                        }

                        foreach (DataRow row in L.Rows)
                        {
                            if (TABLE_STLINE.TOOLS.isLocalMat(row))
                            {
                                TABLE_STLINE.TOOLS.addTrans(pTable);

                                TAB_SETROW(pTable, TABLE_STLINE.COLS.LOGICALREF, TAB_GETROW(row, TABLE_STLINE.COLS.LOGICALREF));
                                TAB_SETROW(pTable, TABLE_STLINE.COLS.LINETYPE, TAB_GETROW(row, TABLE_STLINE.COLS.LINETYPE));
                                TAB_SETROW(pTable, TABLE_STLINE.COLS.GLOBTRANS, TAB_GETROW(row, TABLE_STLINE.COLS.GLOBTRANS));

                                TAB_SETROW(pTable, TABLE_STLINE.COLS.STOCKREF, TAB_GETROW(row, TABLE_STLINE.COLS.STOCKREF));
                                TAB_SETROW(pTable, TABLE_STLINE.COLS.UOMREF, TAB_GETROW(row, TABLE_STLINE.COLS.UOMREF));
                                TAB_SETROW(pTable, TABLE_STLINE.COLS.AMOUNT, TAB_GETROW(row, TABLE_STLINE.COLS.AMOUNT));
                                TAB_SETROW(pTable, TABLE_STLINE.COLS.PRICE, TAB_GETROW(row, TABLE_STLINE.COLS.PRICE));
                                TAB_SETROW(pTable, TABLE_STLINE.COLS.SPECODE, TAB_GETROW(row, TABLE_STLINE.COLS.PRICE));
                                TAB_SETROW(pTable, TABLE_STLINE.COLS.LINEEXP, TAB_GETROW(row, TABLE_STLINE.COLS.LINEEXP));


                                if (PRM.TERM_TYPE == TERMTYPE.hotel)
                                {
                                    TAB_SETROW(pTable, TABLE_STLINE.COLS.DATEBEG, TAB_GETROW(row, TABLE_STLINE.COLS.DATEBEG));
                                    TAB_SETROW(pTable, TABLE_STLINE.COLS.DATEEND, TAB_GETROW(row, TABLE_STLINE.COLS.DATEEND));
                                }


                                TAB_SETROW(pTable, TABLE_STLINE.COLS.ITEMS_CODE, TAB_GETROW(row, TABLE_STLINE.COLS.ITEMS_CODE));
                                TAB_SETROW(pTable, TABLE_STLINE.COLS.ITEMS_NAME, TAB_GETROW(row, TABLE_STLINE.COLS.ITEMS_NAME));
                                TAB_SETROW(pTable, TABLE_STLINE.COLS.UNITSETL_CODE, TAB_GETROW(row, TABLE_STLINE.COLS.UNITSETL_CODE));


                            }



                        }


                    }
                    catch (Exception exc)
                    {
                        exceptionHandler(exc);
                        setMsgErr(exc.Message);
                    }




                }



                public void BEGIN_SLSMAN()
                {
                    //
                    DataReference ref_ = null;

                    try
                    {

                        var f = SalesManReference.Filter.All;



                        ref_ = new SalesManReference(PLUGIN, this, f);
                        ref_.setModeShow(true);
                        ref_.REF();
                    }
                    finally
                    {
                        if (ref_ != null) ref_.Dispose();

                    }



                }
                public void BEGIN_SLSINV()
                {
                    if (!PRM.DOCUMENTS)
                        return;
                    //
                    DataReference ref_ = null;
                    try
                    {

                        object lref_ = registers.clcard.getClientRef();
                        if (ISEMPTYLREF(lref_))
                            return;

                        ref_ = new InvoiceSlsReference(PLUGIN, this, lref_);
                        ref_.setModeShow(true);
                        ref_.REF();
                    }
                    finally
                    {
                        if (ref_ != null) ref_.Dispose();

                    }



                }



                void tableData__RowChanged(object sender, DataRowChangeEventArgs e)
                {
                    refreshTots();
                }

                void tableData__ColumnChanged(object sender, DataColumnChangeEventArgs e)
                {
                    // refreshInfo();
                }

                void formClosed(object sender, FormClosedEventArgs e)
                {
                    if (dataInputForm_ != null) { dataInputForm_.Dispose(); dataInputForm_ = null; };

                    tableData_ = null;
                    tableInfo_ = null;
                    tableMsg_ = null;

                    tableData_ = null;

                    dataSet_ = null;

                    registers = null;
                }

                void formClosing(object sender, FormClosingEventArgs e)
                {

                    if (_READONLY)
                    {
                        e.Cancel = false;

                        return;

                    }

                    var has_ = hasData();

                    if (has_ && PRM.STOP_CLOSE_FULL_DOC)
                    {
                        e.Cancel = true;
                        return;
                    }

                    if (has_)
                        e.Cancel = (!MSGUSERASK(_LANG.L.MSG_ASK_CLOSE));
                }


                void keyDown(object sender, KeyEventArgs e)
                {

                    Exception err = null;
                    try
                    {
                        setMsgErr("");
                        // clearMsgs();

                        if (!e.Handled)
                            if (!e.Control && !e.Alt && !e.Shift)
                            {
                                switch (e.KeyCode)
                                {



                                    case Keys.Back: //clear input
                                        e.Handled = true;
                                        setInputText("");
                                        break;
                                    case Keys.Enter: //enter input
                                        e.Handled = true;
                                        processInputText();
                                        break;
                                    //case Keys.Up: //move prev
                                    //    dataGridSelectedRow(-1);
                                    //    break;
                                    //case Keys.Down: //move next
                                    //    dataGridSelectedRow(+1);
                                    //    break;
                                    //case Keys.Home://move first
                                    //    setDataGridCurrRecPos(0);
                                    //    break;
                                    //case Keys.End://move last
                                    //    setDataGridCurrRecPos(int.MaxValue);
                                    //    break;
                                    case Keys.F11://save and new
                                        e.Handled = true;
                                        processClientFromReference();
                                        break;

                                    case Keys.F12://save and new
                                        e.Handled = true;
                                        processMatFromReference();
                                        break;
                                    case Keys.F8://save and new
                                        e.Handled = true;
                                        BEGIN_SLSINV();
                                        break;
                                    case Keys.F2://save and new
                                        e.Handled = true;
                                        save();
                                        break;

                                    case Keys.F4://save and new
                                        e.Handled = true;
                                        userToCash();
                                        break;
                                    case Keys.F5://save and new
                                        e.Handled = true;
                                        changeCash();
                                        break;
                                    case Keys.F6://save and new
                                        e.Handled = true;
                                        changeAllVAT();
                                        break;
                                    case Keys.Escape://save and new
                                        e.Handled = true;
                                        cancel();
                                        break;
                                    case Keys.Multiply:
                                        e.Handled = true;
                                        processMathOperQuantity(MathOper.mult);
                                        break;
                                    case Keys.Subtract:
                                        e.Handled = true;
                                        processMathOperQuantity(MathOper.sub);
                                        break;
                                    case Keys.Add:
                                        e.Handled = true;
                                        processMathOperQuantity(MathOper.sum);
                                        break;
                                    case Keys.Divide:
                                        e.Handled = true;
                                        processMathOperQuantity(MathOper.div);
                                        break;



                                }

                            }

                        if (!e.Handled)
                            if (e.Control && !e.Alt && !e.Shift)
                            {
                                switch (e.KeyCode)
                                {

                                    case Keys.U: //user to cash
                                        e.Handled = true;
                                        userToCash();
                                        break;
                                    case Keys.A: //user to cash
                                        e.Handled = true;
                                        setMonth();
                                        break;
                                    case Keys.P: //print last
                                        {
                                            e.Handled = true;

                                            var all = false;

                                            if (PRM.TERM_TYPE == TERMTYPE.restoran)
                                            {
                                                all = PLUGIN.MSGUSERASK("" + _LANG.L.MSG_PRINTFORTABLE + "");

                                            }

                                            PRINTLAST(all);

                                        }
                                        break;
                                    case Keys.N: //clear for new
                                        if (PRM.FULL_CLEAN && !_READONLY)
                                        {
                                            e.Handled = true;
                                            if (!hasData() || MSGUSERASK(_LANG.L.MSG_ASK_CLEAN))
                                                prepareForNew();
                                        }
                                        break;
                                    case Keys.M: //set sales man
                                        e.Handled = true;
                                        setSalesMan();
                                        break;
                                    case Keys.Space: //find text
                                        e.Handled = true;
                                        findText();
                                        break;
                                    //case Keys.H: //execute
                                    //    e.Handled = true;
                                    //    execute();
                                    //    break;
                                    case Keys.R: //find text
                                        e.Handled = true;
                                        setDate();
                                        break;
                                    case Keys.T: //set trackno
                                        e.Handled = true;
                                        setTrackno(null);
                                        break;
                                    case Keys.L: //set desc
                                        e.Handled = true;
                                        setDocDesc();
                                        break;
                                    case Keys.K: //set desc
                                        e.Handled = true;
                                        setDocCode();
                                        break;
                                    case Keys.J: //set desc
                                        e.Handled = true;
                                        setDocSpeCode();
                                        break;
                                    case Keys.Right: //next
                                        e.Handled = true;
                                        moveNext();
                                        break;
                                    case Keys.I://docs
                                        e.Handled = true;
                                        this.BEGIN_SLSINV();
                                        break;
                                    case Keys.Z:
                                        e.Handled = true;
                                        changeAmount();
                                        break;
                                    case Keys.B:
                                        e.Handled = true;
                                        useBonus();
                                        break;
                                    case Keys.Y:
                                        e.Handled = true;
                                        openSameNewWin();
                                        break;
                                    case Keys.X:
                                        e.Handled = true;
                                        changePrice();
                                        break;
                                    case Keys.C:
                                        e.Handled = true;
                                        changeClCard();
                                        break;
                                    case Keys.O:
                                        e.Handled = true;
                                        setPayPlan();
                                        break;
                                    case Keys.E:
                                        e.Handled = true;
                                        setManualDiscountPerc();
                                        break;

                                    case Keys.D:
                                        e.Handled = true;
                                        setManualDiscountAmount();
                                        break;

                                    case Keys.F9:
                                        e.Handled = true;
                                        applyCampagin(false, null);
                                        break;


                                    case Keys.W:
                                        e.Handled = true;
                                        processWarehouseFromReference();
                                        break;
                                    case Keys.F:
                                        e.Handled = true;
                                        fillDoc();
                                        break;
                                    case Keys.Q:
                                        e.Handled = true;
                                        openSalesReturn();
                                        break;
                                }
                            }


                        if (!e.Handled)
                            if (!e.Control && e.Alt && !e.Shift)
                            {
                                switch (e.KeyCode)
                                {

                                    case Keys.G: //user to cash
                                        e.Handled = true;
                                        setFilterGroup();
                                        break;
                                    case Keys.S: //user to cash
                                        e.Handled = true;
                                        setFilterSpeCode();
                                        break;
                                    case Keys.D: //user to cash
                                        e.Handled = true;
                                        setFilterSpeCode2();
                                        break;
                                    case Keys.P: //print last
                                        {
                                            e.Handled = true;
                                            _PRINTLAST(false, registers.docLRef);

                                        }
                                        break;
                                }
                            }


                        if (!e.Handled)
                            if (!e.Control && !e.Alt && e.Shift)
                            {
                                switch (e.KeyCode)
                                {
                                    case Keys.Delete:
                                        e.Handled = true;
                                        dataGridDeleteRec();
                                        break;
                                }
                            }

                        //  if (!e.Handled)
                        //     if (e.Control && e.Alt && !e.Shift)
                        //     {
                        //        switch (e.KeyCode)
                        //        {
                        //            case Keys.Right:  
                        //                e.Handled = true;
                        //                openNew();
                        //                break;
                        //        }
                        //   }



                        //  refreshTots();



                    }
                    catch (Exception exc)
                    {
                        err = exc;
                        // exceptionHandler(exc);
                        // setMsgErr(exc.Message);
                    }
                    finally
                    {
                        // e.SuppressKeyPress = true;
                    }

                    try
                    {
                        if (err == null)
                        {
                            clearMsgs();



                            refreshTots();
                        }
                        else
                        {
                            exceptionHandler(err);
                            setMsgErr(err.Message);
                        }
                    }
                    catch (Exception exc)
                    {
                        exceptionHandler(exc);
                        setMsgErr(exc.Message);
                    }
                    finally
                    {

                    }
                }

                void save()
                {
                    //!!!
                    if (_READONLY)
                        return;

                    if (isEmptyDoc())//!hasData())
                    {
                        setMsgErr(_LANG.L.MSG_ERROR_NO_DATA);
                        return;
                    }




                    if (saveToDb())
                    {
                        //1
                        if (PRM.PRINT_ON_SAVE)
                        {
                            bool doPrint = true;

                            var all = false;

                            if (PRM.TERM_TYPE == TERMTYPE.restoran)
                                all = false;

                            if (PRM.ASK_PRINT)
                            {
                                if (!MSGUSERASK(_LANG.L.MSG_ASK_PRINT))
                                    doPrint = false;
                            }

                            if (doPrint)
                                PRINTLAST(all);


                        }


                        //2
                        prepareForNew();

                        //
                        if (
                            (PRM.TERM_TYPE == TERMTYPE.magazin || PRM.TERM_TYPE == TERMTYPE.magazinWholesale || PRM.TERM_TYPE == TERMTYPE.magazinWholesaleSlsman) &&
                            _TRCODE == 3)
                        {
                            cancel();
                            return;
                        }

                    }
                }
                void cancel()
                {



                    this.dataInputForm_.Close();

                    try
                    {
                        //
                        var indx = this.parentFormIndex;
                        //
                        if (indx > 0)
                            foreach (var f in Application.OpenForms)
                            {
                                var t = f as TerminalForm;
                                if (t != null && t.formIndex == indx)
                                {
                                    t.Activate();
                                    t.BringToFront();

                                }
                            }
                        //

                    }
                    catch
                    {


                    }
                }

                public bool SLSMANCLEAN(object pSlsMan, string pDesc)
                {
                    try
                    {
                        if (PRM.TERM_TYPE == TERMTYPE.restoran)
                        {

                            if (ISEMPTYLREF(pSlsMan))
                                return false;


                            if (!MSGUSERASK(pDesc + " " + _LANG.L.MSG_ASK_DO_FREE_PLACE + ""))
                                return false;

                            var text_ = PRM.CYPHCODE_CLOSED + "," + DateTime.Now.ToString("yyyy-MM-dd HH-mm");

                            var cl_ = SalesManReference.getCurrentSlsManClient(PLUGIN, pSlsMan);
                            var clCurr_ = registers.clcard.getClientRef();
                            if (ISEMPTYLREF(cl_))
                                return false;


                            if (!COMPARE(cl_, clCurr_))
                            {
                                setMsgErr(_LANG.L.MSG_ERROR_CLIENT);
                                return false;
                            }



                            var ficheNo_ = CASTASSTRING(PLUGIN.SQLSCALAR(MY_CHOOSE_SQL(
                               @"
 
                            select min(FICHENO) from  LG_$FIRM$_$PERIOD$_INVOICE WITH(NOLOCK)
                            where 
                            CLIENTREF = @P2 and SALESMANREF = @P3 AND CANCELLED=0 AND TRCODE IN (8) AND CYPHCODE = @P5
  
                            ", @"
 
                            select min(FICHENO) from  LG_$FIRM$_$PERIOD$_INVOICE 
                            where 
                            CLIENTREF = @P2 and SALESMANREF = @P3 AND CANCELLED=0 AND TRCODE IN (8) AND CYPHCODE = @P5
  
                            "),

                         new object[] { PRM.CYPHCODE_CLOSED, cl_, pSlsMan, text_, PRM.CYPHCODE }));


                            var val_ = CASTASDOUBLE(PLUGIN.SQLSCALAR(MY_CHOOSE_SQL(
                               @"
                            declare @val float 
                            select @val = 0
 
                            update LG_$FIRM$_$PERIOD$_INVOICE set CYPHCODE = @P1,  @val =  @val + NETTOTAL, 
                            GENEXP3 = @P4,  DOCTRACKINGNR = @P6
                            where 
                            CLIENTREF = @P2 and SALESMANREF = @P3 AND CANCELLED=0 AND TRCODE IN (8) AND CYPHCODE = @P5

                            select  isnull(@val,0) VAL
                            ", @"
 
WITH T AS (
                            update LG_$FIRM$_$PERIOD$_INVOICE set CYPHCODE = @P1,  GENEXP3 = @P4,  DOCTRACKINGNR = @P6
                            where 
                            CLIENTREF = @P2 and SALESMANREF = @P3 AND CANCELLED=0 AND TRCODE IN (8) AND CYPHCODE = @P5
                            RETURNING NETTOTAL 
) 
SELECT SUM(NETTOTAL) FROM T

                            
                            "),

                         new object[] { PRM.CYPHCODE_CLOSED, cl_, pSlsMan, text_, PRM.CYPHCODE, ficheNo_ }));

                            val_ = ROUND(val_, 2);

                            MSGUSERINFO(string.Format("" + _LANG.L.MSG_TABLETOT + " {0:N2} " + _LANG.L.CURRENCY + "", val_));

                            return true;
                        }



                    }
                    catch (Exception exc)
                    {
                        exceptionHandler(exc);
                        setMsgErr(_LANG.L.MSG_ERROR_PRINT);
                    }

                    return false;
                }
                //                public void QPRODSIMPLE()
                //                {

                //                    CALCCOST();

                //                    var sqlRequired = MY_CHOOSE_SQL( @"
                // 
                //SELECT 
                //STOCKREF LOGICALREF,
                //ABS(ONHAND) AMOUNT,
                //(
                //		SELECT TOP (1) P.PRICE
                //		FROM LG_$FIRM$_PRCLIST P WITH(NOLOCK)
                //		WHERE P.CARDREF = H.STOCKREF
                //			AND P.PTYPE = 1
                //		ORDER BY P.ENDDATE DESC
                //) PRICE
                //FROM (
                //	SELECT STOCKREF, ISNULL((
                //				SELECT ONHAND
                //				FROM LG_$FIRM$_$PERIOD$_GNTOTST GNTOTST WITH(NOLOCK)
                //				WHERE INVENNO = - 1 AND STOCKREF = T.STOCKREF
                //				), 0.0) ONHAND
                //	FROM (
                //		SELECT DISTINCT MAINCREF STOCKREF
                //		FROM LG_$FIRM$_STCOMPLN
                //		) T
                //	) H
                //WHERE H.ONHAND < - 0.01
                // 
                //", @"
                // 
                //SELECT 
                //STOCKREF LOGICALREF,
                //ABS(ONHAND) AMOUNT,
                //(
                //		SELECT P.PRICE
                //		FROM LG_$FIRM$_PRCLIST P WITH 
                //		WHERE P.CARDREF = H.STOCKREF
                //			AND P.PTYPE = 1
                //		ORDER BY P.ENDDATE DESC 
                //        LIMIT 1
                //) PRICE
                //FROM (
                //	SELECT STOCKREF, COALESCE((
                //				SELECT ONHAND
                //				FROM LG_$FIRM$_$PERIOD$_GNTOTST GNTOTST 
                //				WHERE INVENNO = - 1 AND STOCKREF = T.STOCKREF
                //				), 0.0) ONHAND
                //	FROM (
                //		SELECT DISTINCT MAINCREF STOCKREF
                //		FROM LG_$FIRM$_STCOMPLN
                //		) T
                //	) H
                //WHERE H.ONHAND < - 0.01
                // 
                //")


                //                        ;

                //                    var itemsQProdRaw = SQL(sqlRequired, null);

                //                    var tabTempalte = new DataTable();

                //                    TAB_ADDCOL(tabTempalte, TABLE_STLINE.COLS.STOCKREF, TABLE_STLINE.TYPES.STOCKREF);
                //                    TAB_ADDCOL(tabTempalte, TABLE_STLINE.COLS.AMOUNT, TABLE_STLINE.TYPES.AMOUNT);
                //                    TAB_ADDCOL(tabTempalte, TABLE_STLINE.COLS.PRICE, TABLE_STLINE.TYPES.PRICE);
                //                    TAB_ADDCOL(tabTempalte, TABLE_STLINE.COLS.LINEEXP, TABLE_STLINE.TYPES.ITEMS_NAME);

                //                    var itemsQProd = tabTempalte.Clone();
                //                    var itemsUsage = tabTempalte.Clone();
                //                    var itemsScrap = tabTempalte.Clone();


                //                    foreach (DataRow r1 in itemsQProdRaw.Rows)
                //                    {
                //                        var lrefP = TAB_GETROW(r1, "LOGICALREF");
                //                        var descP = SQLSCALAR(
                //                            MY_CHOOSE_SQL(
                //                            "SELECT STGRPCODE+'/'+NAME FROM LG_$FIRM$_ITEMS WHERE LOGICALREF=@P1",
                //                             "SELECT STGRPCODE || '/' || NAME FROM LG_$FIRM$_ITEMS WHERE LOGICALREF=@P1"), 
                //                            new object[] { lrefP });
                //                        var amountP = CASTASDOUBLE(TAB_GETROW(r1, "AMOUNT"));
                //                        var priceP = CASTASDOUBLE(TAB_GETROW(r1, "PRICE"));
                //                        itemsQProd.Rows.Add(
                //                           lrefP,
                //                            amountP,
                //                            priceP,
                //                            descP);


                //                        //


                //                        var subItems = QPROD("ITEMS", lrefP);

                //                        foreach (DataRow r2 in subItems.Rows)
                //                        {
                //                            var lrefS = TAB_GETROW(r2, "LOGICALREF");
                //                            var amountS = amountP * CASTASDOUBLE(TAB_GETROW(r2, "AMOUNT"));
                //                            var priceS = CASTASDOUBLE(TAB_GETROW(r2, "PRICE"));
                //                            var lostS = CASTASDOUBLE(TAB_GETROW(r2, "LOSTFACTOR"));

                //                            itemsUsage.Rows.Add(
                //                       lrefS,
                //                        amountS * (100 / (100 + lostS)),
                //                        priceS,
                //                        descP);

                //                            itemsScrap.Rows.Add(
                //                       lrefS,
                //                        amountS * (lostS / (100 + lostS)),
                //                        priceS,
                //                        descP);


                //                        }

                //                        var date = DateTime.Now;
                //                        var docNr = Registers.generateDocCode(this.PLUGIN, date);

                //                        DoWorkEventHandler handler = new DoWorkEventHandler((sender, args) =>
                //                        {
                //                            args.Result = false;

                //                            DataSet doc_ = ((DataSet)args.Argument);

                //                            DataTable tabHeaderSlip_ = TAB_GETTAB(doc_, "STFICHE");
                //                            DataTable tabLine_ = TAB_GETTAB(doc_, "STLINE");

                //                            var trcode = CASTASSHORT(TAB_GETROW(tabHeaderSlip_, "TRCODE"));

                //                            DataTable lines = null;
                //                            switch (trcode)
                //                            {
                //                                case 11: lines = itemsScrap; break;
                //                                case 12: lines = itemsUsage; break;
                //                                case 13: lines = itemsQProd; break;
                //                            }


                //                            TAB_SETROW(tabHeaderSlip_, "FICHENO", docNr);

                //                            TAB_SETROW(tabHeaderSlip_, "DUMMY_____DATETIME", date.Date);
                //                            TAB_SETROW(tabHeaderSlip_, "SOURCEINDEX", 0);
                //                            TAB_SETROW(tabHeaderSlip_, "CYPHCODE", PRM.CYPHCODE);


                //                            foreach (DataRow rowLine in lines.Rows)
                //                            {
                //                                var newRow_ = TAB_ADDROW(tabLine_);

                //                                TAB_SETROW(newRow_, TABLE_STLINE.COLS.STOCKREF, TAB_GETROW(rowLine, TABLE_STLINE.COLS.STOCKREF));
                //                                TAB_SETROW(newRow_, TABLE_STLINE.COLS.AMOUNT, TAB_GETROW(rowLine, TABLE_STLINE.COLS.AMOUNT));
                //                                TAB_SETROW(newRow_, TABLE_STLINE.COLS.PRICE, TAB_GETROW(rowLine, TABLE_STLINE.COLS.PRICE));
                //                                TAB_SETROW(newRow_, TABLE_STLINE.COLS.LINEEXP, TAB_GETROW(rowLine, TABLE_STLINE.COLS.LINEEXP));

                //                            }


                //                            args.Result = (tabLine_.Rows.Count > 0); //save if has data
                //                        }
                //                                );


                //                        PLUGIN.INVOKEINBATCH((s, e) =>
                //                        {

                //                            PLUGIN.EXEADPCMD(
                //                                new string[] { 

                //                                    "adp.mm.doc.slip.11 cmd::add", //scrap
                //                                    "adp.mm.doc.slip.12 cmd::add",  //usage
                //                                    "adp.mm.doc.slip.13 cmd::add" //prod

                //                                },
                //                                new DoWorkEventHandler[] { 

                //                                 handler,
                //                                 handler,
                //                                 handler


                //                                }, true);//in global batch

                //                        }, null);




                //                    }
                //                }



                public void PRICEPRCHFROMDOC()
                {

                    PRM.UPDATE_PRCH_PRICE_IN_CARD = true;
                    PRM.UPDATE_PRICE_COIF = false;


                    //last prices
                    var sql = MY_CHOOSE_SQL(
 @"


select LOGICALREF,PRICEDOC PRICE
from (
select 

LOGICALREF, 
ISNULL((

SELECT TOP(1) ((VATMATRAH+VATAMNT+DISTEXP)/AMOUNT) PRICE 
FROM 
LG_$FIRM$_$PERIOD$_STLINE 
WITH(NOLOCK)
WHERE 
(
STOCKREF = ITEMS.LOGICALREF AND 
VARIANTREF >= 0 AND
DATE_ >= '19000101' AND
FTIME >= 0 AND
IOCODE = 1 AND 
SOURCEINDEX = 0
) AND
(
CANCELLED = 0 AND 
LINETYPE = 0 AND 
TRCODE IN (1) -- 50 
)
ORDER BY
STOCKREF DESC,
VARIANTREF DESC,
DATE_ DESC,
FTIME DESC,
IOCODE DESC,
SOURCEINDEX DESC,
LOGICALREF DESC
),0.0) PRICEDOC,
ISNULL((
		SELECT TOP (1) P.PRICE
		FROM LG_$FIRM$_PRCLIST P WITH(NOLOCK)
		WHERE P.CARDREF = ITEMS.LOGICALREF
			AND P.PTYPE = 1
		ORDER BY P.ENDDATE DESC
),0.0) PRICECARD 
from LG_$FIRM$_ITEMS ITEMS
) T where ABS(T.PRICEDOC-T.PRICECARD)>0.001 AND T.PRICEDOC > 0.001


",
 @"


select LOGICALREF,PRICEDOC PRICE
from (
select 

LOGICALREF, 
COALESCE((

SELECT ((VATMATRAH+VATAMNT+DISTEXP)/AMOUNT) PRICE 
FROM 
LG_$FIRM$_$PERIOD$_STLINE 
 
WHERE 
(
STOCKREF = ITEMS.LOGICALREF AND 
VARIANTREF >= 0 AND
DATE_ >= '19000101' AND
FTIME >= 0 AND
IOCODE = 1 AND 
SOURCEINDEX = 0
) AND
(
CANCELLED = 0 AND 
LINETYPE = 0 AND 
TRCODE IN (1) -- 50 
)
ORDER BY
STOCKREF DESC,
VARIANTREF DESC,
DATE_ DESC,
FTIME DESC,
IOCODE DESC,
SOURCEINDEX DESC,
LOGICALREF DESC
 LIMIT 1

),0.0) PRICEDOC,
COALESCE((
		SELECT P.PRICE
		FROM LG_$FIRM$_PRCLIST P 
		WHERE P.CARDREF = ITEMS.LOGICALREF
			AND P.PTYPE = 1
		ORDER BY P.ENDDATE DESC LIMIT 1
),0.0) PRICECARD 
from LG_$FIRM$_ITEMS ITEMS
) T where ABS(T.PRICEDOC-T.PRICECARD)>0.001 AND T.PRICEDOC > 0.001


");


                    var data = SQL(sql, new object[] { });
                    var me = this;

                    PLUGIN.INVOKEINBATCH((s, e) =>
                    {

                        foreach (DataRow r in data.Rows)
                        {

                            var lref = TAB_GETROW(r, "LOGICALREF");
                            var price = CASTASDOUBLE(TAB_GETROW(r, "PRICE"));

                            (new MY_SAVEPRICE(me.PLUGIN, null) { pricePrch = price, matRef = lref }).RUN();
                        }

                    }, null);



                }

                public void JOB_DONE()
                {
                    if (PRM.JOB_DONE_INFORM)
                        MSGUSERINFO("T_MSG_OPERATION_FINISHED");
                }
                //double QPROD_COST(object pMatLref)
                //{
                //    var cost = 0.0;

                //    var sub = QPROD("ITEMS", pMatLref);
                //    foreach (DataRow x in sub.Rows)
                //    {
                //        cost += (CASTASDOUBLE(TAB_GETROW(x, "AMOUNT")) * CASTASDOUBLE(TAB_GETROW(x, "PRICE")));
                //    }


                //    return cost;
                //}

                //                DataTable QPROD(string pFilter, object pLref)
                //                {
                //                    //TODO
                //                    #region sql


                //                    var sqlCost = @"
                // 
                //
                //  declare @maxDepth int
                // select @maxDepth = 15
                //
                //--  10   Raw Material,Сырье
                //--  11  Semi Finished Good,Полуфабрикат
                //--  12  Finished Good,Продукция
                //--  13  Consumer Goods,Расходные Материалы
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

                //                    #endregion

                //                    return SQL(sqlCost, new object[] { pLref });
                //                }


                //                public void CALCCOST()
                //                {



                //                    PRM.UPDATE_PRCH_PRICE_IN_CARD = true;
                //                    PRM.UPDATE_PRICE_COIF = false;

                //                    PRICEPRCHFROMDOC();
                //                    var sqlMats = MY_CHOOSE_SQL(
                //@"
                //
                // select distinct MAINCREF LOGICALREF from LG_$FIRM$_STCOMPLN
                //
                //",

                // @"
                //
                // select distinct MAINCREF LOGICALREF from LG_$FIRM$_STCOMPLN
                //
                //" 

                // );


                //                    var items = SQL(sqlMats, new object[] { });
                //                    var me = this;

                //                    PLUGIN.INVOKEINBATCH((s, e) =>
                //                    {

                //                        foreach (DataRow r in items.Rows)
                //                        {

                //                            var lref = TAB_GETROW(r, "LOGICALREF");

                //                            var cost = QPROD_COST(lref);

                //                            (new MY_SAVEPRICE(me.PLUGIN, null)
                //                            {
                //                                matRef = lref,
                //                                pricePrch = cost
                //                            }).RUN();
                //                        }

                //                    }, null);

                //                }

                public void PRINTLAST(bool pAll)
                {
                    _PRINTLAST(pAll, null);
                }

                public void _PRINTLAST(bool pAll, object pLRef)
                {

                    if (_READONLY)
                        return;

                    try
                    {

                        string marker = "";

                        if (PRM.TERM_TYPE == TERMTYPE.barcode)
                        {
                            marker = _DESIGN;
                            //config/report/mm.label
                            string cmd_ = "rep loc::" + PARAMETERS.barcodePrintReportDir + " filter::filter_LIST,{0} REP_QUICK_MODE_B::1 REP_DEF_FILTER_B::1 DSGN_OUTPUT_B::1 REP_DSG_KEY_WORDS_S::print" + marker;
                            List<string> list_ = new List<string>();


                            var curr_ = this.dataInputForm_.getCurrentRecord();

                            bool add_ = false;

                            foreach (DataRow row_ in this.tableData_.Rows)
                            {
                                if (!add_)
                                {
                                    if (curr_ == null)
                                        add_ = true;
                                    else
                                        if (object.ReferenceEquals(curr_, row_))
                                            add_ = true;


                                }

                                if (add_)
                                {
                                    object mref_ = TAB_GETROW(row_, TABLE_STLINE.COLS.STOCKREF);
                                    var code_ = CASTASSTRING(SEARCH_MAT_VAL(mref_, TABLE_ITEMS.COLS.CODE));
                                    var quantity_ = (int)CASTASDOUBLE(TAB_GETROW(row_, TABLE_STLINE.COLS.AMOUNT));

                                    list_.Add(code_);
                                    list_.Add(FORMAT(quantity_));
                                }
                            }

                            if (list_.Count == 0)
                                return;

                            cmd_ = string.Format(cmd_, FORMATSERIALIZE(JOINLIST(list_.ToArray())));

                            
                            RUNTIMELOG(cmd_);
                           
                            PLUGIN.EXECMDTEXT(cmd_);

                            return;
                        }
                        {
                            object[] docRef_ = new object[] { };

                            if (PRM.TERM_TYPE == TERMTYPE.restoran)
                            {


                                DataTable tab_ = null;


                                if (pAll) //by table in restoran
                                {
                                    marker = "restall";

                                    DataReference ref_ = null;

                                    try
                                    {
                                        ref_ = new SalesManReference(PLUGIN, this, SalesManReference.Filter.CurrentClient);
                                        ref_.setModeShow(false);
                                        DataRow[] rows_ = ref_.REF();
                                        if (rows_ != null && rows_.Length > 0)
                                        {
                                            var slsMan_ = TAB_GETROW(rows_[0], TABLE_SLSMAN.COLS.LOGICALREF);
                                            string sql_ =

                                                MY_CHOOSE_SQL(
                                                @"
                                                SELECT LOGICALREF FROM LG_$FIRM$_$PERIOD$_INVOICE I WITH(NOLOCK) 
                                                WHERE 
                                                I.SALESMANREF = @P1 AND I.CYPHCODE = @P2 AND I.CANCELLED=0 AND I.TRCODE IN (8)
                                                ORDER BY I.LOGICALREF ASC",
                                                 @"
                                                SELECT LOGICALREF FROM LG_$FIRM$_$PERIOD$_INVOICE I 
                                                WHERE 
                                                I.SALESMANREF = @P1 AND I.CYPHCODE = @P2 AND I.CANCELLED=0 AND I.TRCODE IN (8)
                                                ORDER BY I.LOGICALREF ASC")


                                                ;


                                            // (PRM.LAST_DOC_BY_DATE_ ? "I.DATE_ ASC  " : "I.LOGICALREF ASC  ")
                                            tab_ = PLUGIN.SQL(sql_,
                                                   new object[] { slsMan_, PRM.CYPHCODE });

                                            //
                                        }
                                        else
                                            return;
                                    }
                                    finally
                                    {
                                        if (ref_ != null) ref_.Dispose();
                                    }


                                }
                                else
                                {

                                    marker = "restone";

                                    if (ISEMPTYLREF(pLRef))
                                    {

                                        var clRef_ = registers.clcard.getClientRef();


                                        if (!ISEMPTYLREF(clRef_))
                                        {
                                            string sql_ =
                                                MY_CHOOSE_SQL(
                                                @"
                                                SELECT TOP(1) LOGICALREF FROM LG_$FIRM$_$PERIOD$_INVOICE I WITH(NOLOCK) 
                                                WHERE 
                                                I.CLIENTREF = @P1 AND I.CYPHCODE = @P2 AND I.CANCELLED=0 AND I.TRCODE IN (8)
                                                ORDER BY I.LOGICALREF DESC",
                                                @"
                                                SELECT LOGICALREF FROM LG_$FIRM$_$PERIOD$_INVOICE I 
                                                WHERE 
                                                I.CLIENTREF = @P1 AND I.CYPHCODE = @P2 AND I.CANCELLED=0 AND I.TRCODE IN (8)
                                                ORDER BY I.LOGICALREF DESC
                                                LIMIT 1")

                                                ;


                                            //(PRM.LAST_DOC_BY_DATE_ ? "I.DATE_ DESC,I.TIME_ DESC,I.TRCODE DESC,I.LOGICALREF DESC" : "I.LOGICALREF DESC  ");
                                            tab_ = PLUGIN.SQL(sql_,
                                                   new object[] { clRef_, PRM.CYPHCODE });

                                        }
                                        else
                                        {
                                            setMsgErr(_LANG.L.MSG_ERROR_CLIENT);
                                            return;
                                        }
                                    }

                                }


                                if (tab_ != null)
                                {

                                    docRef_ = new object[tab_.Rows.Count];
                                    for (int i = 0; i < docRef_.Length; ++i)
                                        docRef_[i] = TAB_GETROW(tab_.Rows[i], TABLE_INVOICE.COLS.LOGICALREF);
                                }


                            }
                            else
                            {
                                marker = FORMAT(_TRCODE);

                                //if (PRM.TERM_TYPE == TERMTYPE.magazin)
                                //    if (PLUGIN.GETSYSPRM_USER() == 1)
                                //        marker = "info";

                                if (ISEMPTYLREF(pLRef))
                                {
                                    var ref_ = PLUGIN.SQLSCALAR(
                                        MY_CHOOSE_SQL(
                                         @"SELECT TOP(1) LOGICALREF FROM 
                            LG_$FIRM$_$PERIOD$_INVOICE I WITH(NOLOCK,INDEX=I$FIRM$_$PERIOD$_INVOICE_I5) 
                            WHERE CAPIBLOCK_CREATEDBY = @P1 AND TRCODE in (@P2) and CANCELLED = 0 
                            ORDER BY I.LOGICALREF DESC",
                                         @"SELECT LOGICALREF FROM 
                            LG_$FIRM$_$PERIOD$_INVOICE I 
                            WHERE CAPIBLOCK_CREATEDBY = @P1 AND TRCODE in (@P2) and CANCELLED = 0 
                            ORDER BY I.LOGICALREF DESC LIMIT 1"),
                                         new object[] { PLUGIN.GETSYSPRM_USER(), _TRCODE });

                                    //(PRM.LAST_DOC_BY_DATE_ ? "I.DATE_ DESC,I.TIME_ DESC,I.TRCODE DESC,I.LOGICALREF DESC" : "I.LOGICALREF DESC  ")
                                    if (!ISEMPTYLREF(ref_))
                                        docRef_ = new object[] { ref_ };
                                }

                            }


                            if (!ISEMPTYLREF(pLRef))
                                docRef_ = new object[] { pLRef };

                            if (docRef_.Length == 0)
                            {
                                setMsgErr("" + _LANG.L.MSG_NO_DOC + "");
                                return;
                            }

                            //config/report/mm.000029
                            string cmd_ = "rep loc::" + PARAMETERS.saleInvoicePrintReportDir + " filter::{0} REP_QUICK_MODE_B::1 REP_DEF_FILTER_B::1 REP_DSG_KEY_WORDS_S::print" + marker + " DSGN_OUTPUT_B::1 DSGN_OUTPUT_DEF_DEV_B::1";


                            List<string> list = new List<string>();
                            foreach (var v in docRef_)
                                list.Add(string.Format("filter_INVOICE_LOGICALREF2,{0}", FORMATSERIALIZE(v)));

                            cmd_ = string.Format(cmd_, JOINGRP(list.ToArray()));

                            RUNTIMELOG(cmd_);

                            PLUGIN.EXECMDTEXT(cmd_);
                        }
                    }
                    catch (Exception exc)
                    {
                        exceptionHandler(exc);
                        setMsgErr(_LANG.L.MSG_ERROR_PRINT);
                    }
                }
                void prepareForNew()
                {
                    try
                    {

                        registers = new Registers(dataSet_);


                        registers.speCode = _SPECODE;
                        registers.docCode = _DOCODE;
                        registers.desc1 = _GENEXP1;

                        setWarehouse(_WH);

                        tableData_.Clear();
                        // dataSet_.ExtendedProperties.Clear();
                        // tableData_.ExtendedProperties.Clear();

                        setMsgErr("");
                        clearMsgs();

                        if (isClientPreDefined())
                            setClientByCodeDef();


                        registers.nextCash(PLUGIN);


                        refreshTots();

                        //   unBackUpDoc();
                    }
                    catch (Exception exc)
                    {
                        exceptionHandler(exc);
                        setMsgErr(_LANG.L.MSG_ERROR_CREATE);
                    }
                }


                void clearMsgs()
                {

                    setMsgInfo1("");
                    setMsgInfo2("");
                    setMsgInfo3("");
                    setMsgInfo4("");
                }

                bool hasData()
                {

                    return (tableData_.Rows.Count > 0);
                }

                bool isEmptyDoc()
                {
                    if (PRM.TERM_TYPE == TERMTYPE.count)
                        return !hasData();

                    if (PRM.TERM_TYPE == TERMTYPE.barcode)
                        return !hasData();

                    if (PRM.TERM_TYPE == TERMTYPE.pricing)
                        return !hasData();

                    if (PRM.TERM_TYPE == TERMTYPE.production)
                        return !hasData();

                    if (!hasData())
                        return true;


                    if (ISNUMZERO(registers.totalNet))
                    {
                        //my be discounted
                        if (ISNUMZERO(registers.totalQuantity))
                            return true;

                    }

                    return false;
                }


                void _saveToDb(object sender, System.ComponentModel.DoWorkEventArgs e)
                {

                    if (isSlsOrPrchDoc())
                    {
                        new MY_SAVECLCARD(PLUGIN, registers);

                        if (registers.clcard.isClientRefEmpty() || !registers.clcard.hasData())
                            throw new Exception(_LANG.L.MSG_ERROR_CLIENT);


                        new MY_SAVEINVOICE(PLUGIN, registers, tableData_, _TRCODE, _CANCELLED); //save invoice


                        if (PRM.MAKE_PAYMENT)
                        {
                            bool doPayment_ = true;
                            if (_CANCELLED)
                                doPayment_ = false;



                            if (doPayment_)
                                new MY_SAVEPAYMENT(PLUGIN, registers, _TRCODE); //make payment for date and docNr from registers
                        }
                    }
                    else
                    {
                        if (PRM.TERM_TYPE == TERMTYPE.count)
                        {

                            new MY_SAVESLIP_COUNT(PLUGIN, registers, tableData_, 50, _CANCELLED);
                            new MY_SAVESLIP_COUNT(PLUGIN, registers, tableData_, 51, _CANCELLED);
                        }
                        else
                            if (PRM.TERM_TYPE == TERMTYPE.pricing)
                            {
                                new MY_SAVEPRICE(PLUGIN, registers, tableData_, _TRCODE, _CANCELLED);

                            }
                            else
                                if (PRM.TERM_TYPE == TERMTYPE.production)
                                {
                                    new MY_SAVEPRODUCTION(PLUGIN, registers, tableData_, _TRCODE, _CANCELLED);

                                }
                    }
                }





                bool saveToDb()
                {



                    try
                    {

                        //
                        if (prepareFoSaveToDb())
                        {


                            if (PRM.TERM_TYPE == TERMTYPE.count)
                            {
                                if (PRM.SAVE_TO_FILE)
                                {
                                    //check if data is old
                                    MY_DIR.SAVE(PLUGIN,
                                        MY_DIR.PRM_DIR_COUNTING,
                                         registers.getDocCode(PLUGIN) + "." +
                                        FORMAT(registers.warehouse).PadLeft(3, '0') + ".counting.xml",
                                        tableData_,
                                        registers
                                        );

                                    return true;
                                }
                            }

                            PLUGIN.INVOKEINBATCH(new DoWorkEventHandler(_saveToDb), null);

                            return true;
                        }
                    }
                    catch (Exception exc)
                    {
                        exceptionHandler(exc);
                        setMsgErr(RESOLVESTR("[lang::" + exc.Message + "]"));
                    }
                    return false;


                }

                bool prepareFoSaveToDb()
                {
                    refreshTots();


                    if (PRM.TERM_TYPE == TERMTYPE.barcode)
                    {
                        return false;
                    }


                    if (PRM.TERM_TYPE == TERMTYPE.count)
                    {
                        if (registers.warehouse < 0)
                        {
                            //


                            //
                            setMsgErr(_LANG.L.MSG_ERROR_WH);
                            return false;
                        }


                    }

                    if (PRM.ASK_SAVE)
                        if (!MSGUSERASK(_LANG.L.MSG_ASK_SAVE))
                        {
                            return false;
                        }

                    if (PRM.USE_SLSMAN)
                    {
                        if (ISEMPTYLREF(registers.slsMan))
                        {
                            if (!setSalesMan())
                                return false;
                        }
                    }
                    if (registers.clcard.isClientRefEmpty())
                    {
                        if (!setClient())
                            return false;
                    }

                    if (PRM.ASK_PAYMENT)
                    {
                        var ask_ = true;

                        if (registers.userToCashAmount >= 0.01)
                            ask_ = false;

                        if (registers.speCode.ToLower() == "nisye")
                            ask_ = false;

                        if (ask_)
                            userToCash();

                    }

                    if (PRM.ASK_MONTH && registers.month <= 0)
                        setMonth();


                    if (PRM.NEGATIVE_LIGHT)
                        REFRESH_ONHAND();

                    if (PRM.APPLY_CAMPAGIN_AUTO)
                        applyCampagin(true, null);

                    refreshTots();

                    return true;
                }


                void dataGridSelectedRow(int pOffset)
                {
                    BindingManagerBase bm_ = dataInputForm_.BindingContext[tableData_];
                    if (bm_ != null)
                    {
                        int pos_ = Math.Max(0, Math.Min(bm_.Count - 1, bm_.Position + pOffset));
                        if (bm_.Count > 0)
                            bm_.Position = pos_;
                    }
                }

                void dataGridDeleteRec()
                {

                    if (PRM.USE_PASSWORD_FOR_AMOUNT)
                        if (!askPassword())
                            return;

                    DataRow row_ = dataInputForm_.getCurrentRecord();
                    if (row_ != null)
                    {
                        string name_ = CASTASSTRING(TAB_GETROW(row_, TABLE_STLINE.COLS.ITEMS_NAME));
                        double val_ = CASTASDOUBLE(TAB_GETROW(row_, TABLE_STLINE.COLS.AMOUNT));
                        double prc_ = CASTASDOUBLE(TAB_GETROW(row_, TABLE_STLINE.COLS.PRICE));

                        if (dataInputForm_.deleteRecord(PLUGIN))
                        {

                            logDbAmountChanged(name_, val_, 0, prc_);

                            refreshTots();
                        }

                    }

                    //  backUpDoc();
                }
                int getDataGridCurrRecPos()
                {
                    BindingManagerBase bm_ = dataInputForm_.BindingContext[tableData_];
                    if (bm_ != null)
                        return bm_.Position;
                    return -1;
                }
                void setDataGridCurrRecPos(int pPos)
                {
                    dataInputForm_.gridPosition(pPos);




                }
                void setDataGridCurrRecPos(DataRow pRow)
                {
                    dataInputForm_.gridPosition(pRow);




                }
                public object getUserCardLRef()
                {
                    string code_ = "STAFF";


                    if (string.IsNullOrEmpty(PRM.POS_USER_PREFIX))
                        code_ = code_ + _PLUGIN.RIGHT("000" + _PLUGIN.FORMAT(PLUGIN.GETSYSPRM_USER()), 3);
                    else
                        code_ = code_ + PRM.POS_USER_PREFIX;

                    code_ = code_ + _CLSUFIX;

                    return getUserCardLRefByCode(code_);
                }


                public object getUserCardLRefByCode(string pCode)
                {
                    return ISNULL(SQLSCALAR(
                        MY_CHOOSE_SQL(
                        "select LOGICALREF from LG_$FIRM$_CLCARD WITH(NOLOCK) where CODE = @P1",
                         "select LOGICALREF from LG_$FIRM$_CLCARD where CODE = @P1")

                        , new object[] { pCode }), 0);
                }
                public object getUserCardLRefByBarCode(string pBarCode)
                {
                    DataTable tab_ = null;


                    if (PRM.SEARCH_CL_BY_CODE)
                    {
                        tab_ = SQL(
                            MY_CHOOSE_SQL(
                            "select top(2) LOGICALREF from LG_$FIRM$_CLCARD with(nolock) where CODE = @P1 and ACTIVE = 0",
                              "select LOGICALREF from LG_$FIRM$_CLCARD where CODE = @P1 and ACTIVE = 0 LIMIT 2"),
                            new object[] { pBarCode });
                    }
                    else
                    {
                        tab_ = SQL(
                            MY_CHOOSE_SQL(
                            "select top(2) LOGICALREF from LG_$FIRM$_CLCARD with(nolock) where STATECODE = @P1 and ACTIVE = 0",
                             "select LOGICALREF from LG_$FIRM$_CLCARD where STATECODE = @P1 and ACTIVE = 0 LIMIT 2"
                             ),

                            new object[] { pBarCode });
                    }
                    if (tab_.Rows.Count > 1)
                    {
                        ERROR_INVALID_MAT_BARCODE("Multi client with same barcode [" + pBarCode + "]", false);
                        return 0;
                    }
                    if (tab_.Rows.Count == 1)
                        return ISNULL(TAB_GETROW(tab_, "LOGICALREF"), 0);

                    ERROR_INVALID_MAT_BARCODE(pBarCode, false);

                    return 0;
                }



                void setPayPlan()
                {
                    if (!PRM.CHANGE_PAYPLAN)
                        return;


                    try
                    {


                        var raw_ = PLUGIN.SQL(
                            MY_CHOOSE_SQL(
                            "SELECT PP.CODE,PP.DEFINITION_ FROM LG_$FIRM$_PAYPLANS PP WITH(NOLOCK) ORDER BY PP.CODE",
                            "SELECT PP.CODE,PP.DEFINITION_ FROM LG_$FIRM$_PAYPLANS PP ORDER BY PP.CODE")
                            , null);
                        List<string> list = new List<string>();
                        foreach (DataRow x in raw_.Rows)
                        {
                            list.Add(CASTASSTRING(x["CODE"]).Replace(',', '_').Replace(' ', '_'));
                            list.Add(CASTASSTRING(x["DEFINITION_"]).Replace(',', '_').Replace(' ', '_'));
                        }

                        var res_ = PLUGIN.REF("ref.gen.definedlist [obj::" + JOINLIST(list.ToArray()) + "] [desc::" + _LANG.L.PRICEGRP + "] type::string");

                        string code_ = (res_ != null && res_.Length > 0 ? CASTASSTRING(res_[0]["VALUE"]) : null);
                        if (code_ != null)
                        {

                            object lref_ = PLUGIN.SQLSCALAR(
                                MY_CHOOSE_SQL(
                                "SELECT PP.LOGICALREF FROM LG_$FIRM$_PAYPLANS PP WITH(NOLOCK) WHERE PP.CODE = @P1",
                                "SELECT PP.LOGICALREF FROM LG_$FIRM$_PAYPLANS PP WHERE PP.CODE = @P1"),
                                new object[] { code_ });

                            registers.clcard.setClientPayplan(lref_, PLUGIN);

                            if (PRM.CAMPAGIN_CLEAN_ON_PAYPLAN)
                                applyCampagin(false, "_clear");




                            refreshDoc();
                        }




                    }
                    catch (Exception exc)
                    {
                        exceptionHandler(exc);
                        setMsgErr(_LANG.L.MSG_ERROR_CREATE);
                    }
                    finally
                    {

                    }

                }

                void changeClCard()
                {
                    if (PRM.CLIENT_IN_DB)
                        return;

                    registers.clcard.edit(PLUGIN);

                    refreshDoc();
                }

                public bool isClientPreDefined()
                {
                    return PRM.CLIENT_INIT_FROM_USER;
                }




                void setClientByCodeDef()
                {
                    //  if (isClientPreDefined())
                    {
                        registers.clcard.setClientRef(getUserCardLRef(), PLUGIN);

                        //price will be chaged, dont select client at the end
                        refreshDoc();
                    }
                }
                void setClientByCode(string pCode)
                {
                    //   if (isClientPreDefined())
                    {
                        registers.clcard.setClientRef(getUserCardLRefByCode(pCode), PLUGIN);

                        refreshDoc();
                    }
                }
                bool setClientByBarCode(string pBarCode)
                {
                    //  if (isClientPreDefined())
                    {
                        var lref_ = getUserCardLRefByBarCode(pBarCode);

                        if (!ISEMPTYLREF(lref_))
                        {
                            registers.clcard.setClientRef(lref_, PLUGIN);
                            refreshDoc();
                            //
                            // backUpDoc();
                            return true;
                        }
                    }

                    return false;
                }

                bool setClient()
                {
                    if (!isSlsOrPrchDoc())
                        return true;

                    try
                    {
                        if (isClientPreDefined())
                        {
                            setClientByCodeDef();
                        }
                        else
                            if (!registers.clcard.hasData())
                                changeClCard();

                        if (!registers.clcard.hasData())
                            setMsgErr(_LANG.L.MSG_ERROR_NO_DATA + " " + _LANG.L.MSG_ERROR_CLIENT);
                        else
                            return true;
                    }
                    catch (Exception exc)
                    {
                        exceptionHandler(exc);
                        setMsgErr(_LANG.L.MSG_ERROR_CREATE);
                    }
                    finally
                    {
                        setInputText(string.Empty);  //registers.inputText = string.Empty;
                    }

                    return false;
                }

                void setDate()
                {
                    if (_READONLY)
                        return;

                    if (!PRM.CHANGE_DATE)
                        return;


                    DateTime var_ = MY_ASKDATE("T_DATE", registers.getDocDate());
                    registers.setDocDate(var_);

                    if (PRM.TERM_TYPE == TERMTYPE.count)
                        refreshTots();

                }



                DateTime MY_ASKDATE(string pMsg, DateTime pDef)
                {

                    DataRow[] rows_ = PLUGIN.REF("ref.gen.date desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
                    if (rows_ != null && rows_.Length > 0)
                    {
                        var n = DateTime.Now;
                        var dt = _PLUGIN.CASTASDATE(ISNULL(rows_[0]["DATETIME"], n));
                        return dt;
                    }
                    return pDef;

                }

                string getPriceCalcFormula()
                {

                    var script_ = PRM.PRICE_FORMULA;

                    if (string.IsNullOrEmpty(script_))
                        return "";

                    try
                    {
                        var d = PARSEDOUBLE(script_);
                        d = (100 + d) / 100;
                        script_ = string.Format("PL*{0}", FORMAT(ROUND(d, 2)));
                    }
                    catch
                    {


                    }

                    {
                        var tmp_ = script_.ToLowerInvariant();
                        if (tmp_.Contains("drop") || tmp_.Contains("delete") || tmp_.Contains("select") || tmp_.Contains("insert") || tmp_.Contains("update"))
                            throw new Exception("Incorrect script [" + script_ + "]");
                    }


                    script_ = string.Format(
MY_CHOOSE_SQL(
@"

SELECT {0} FROM 
(
SELECT           
           @P1 PL, --1 last purch 
           @P2 PP, --2 purch price list 
           @P3 PG, --3 purch gross price 
           @P4 PC , --4 purch cost 
           @P5 PA ,--5 purch avg   
           @P6 PD , --6 purch discount 
           @P7 SP,--7 sale pricelist 
           @P8 FF   --8 float f1 
) V


",
@"

SELECT {0} FROM 
(
SELECT     
           cast(@P1 as FLOAT) PL, --1 last purch 
           cast(@P2 as FLOAT) PP, --2 purch price list 
           cast(@P3 as FLOAT) PG, --3 purch gross price 
           cast(@P4 as FLOAT) PC , --4 purch cost 
           cast(@P5 as FLOAT) PA ,--5 purch avg   
           cast(@P6 as FLOAT) PD , --6 purch discount 
           cast(@P7 as FLOAT) SP,--7 sale pricelist 
           cast(@P8 as FLOAT) FF   --8 float f1 
) V 



"), script_);


                    return script_;

                }

                void execute()
                {

                    return;


                    //                    if (PRM.TERM_TYPE == TERMTYPE.pricing)
                    //                    {
                    //                        if (PRICNGVARS.isFormulaInited(tableData_))
                    //                            return;

                    //                        string func_ = "dbo.AVATEZ_$FIRM$_PRICING_" + FORMAT(PLUGIN.GETSYSPRM_USER());

                    //                        var script_ = "";
                    //                        {

                    //                            try
                    //                            {
                    //                                script_ = CASTASSTRING(SQLSCALAR("exec " + func_ + " @TYPE = @P1", new object[] { "S" }));

                    //                            }
                    //                            catch
                    //                            {
                    //                                SQLSCALAR("CREATE PROC " + func_ + " @TYPE NVARCHAR AS BEGIN SELECT ''; END", null);
                    //                            }
                    //                            script_ = string.IsNullOrEmpty(PRM.PRICE_FORMULA) ? askString("Formula", script_) : PRM.PRICE_FORMULA;


                    //                            if (string.IsNullOrEmpty(script_))
                    //                                return;

                    //                            try
                    //                            {
                    //                                var d = PARSEDOUBLE(script_);
                    //                                d = (100 + d) / 100;
                    //                                script_ = string.Format("@PL*{0}", FORMAT(ROUND(d, 2)));
                    //                            }
                    //                            catch
                    //                            {


                    //                            }

                    //                            {
                    //                                var tmp_ = script_.ToLowerInvariant();
                    //                                if (tmp_.Contains("drop") || tmp_.Contains("delete") || tmp_.Contains("select") || tmp_.Contains("insert") || tmp_.Contains("update"))
                    //                                    throw new Exception("Incorrect script [" + script_ + "]");
                    //                            }
                    //                        }

                    //                        {




                    //                            var sql_ = string.Format(@"
                    //
                    //
                    //
                    //ALTER PROC " + func_ + @" @TYPE NVARCHAR='',@PL FLOAT = 0.0,@PG FLOAT = 0.0,@PD FLOAT = 0.0,@PC FLOAT = 0.0,@PA FLOAT = 0.0,@PP FLOAT = 0.0,@SP FLOAT = 0.0,@FF FLOAT = 0.0
                    // AS 
                    //BEGIN
                    //IF @TYPE = 'S'
                    //SELECT '{0}';
                    //ELSE
                    //SELECT {0};
                    //
                    //END
                    //
                    //", script_

                    //     );
                    //                            SQLSCALAR(sql_, null);

                    //                            PRICNGVARS.setFormulaInited(tableData_);

                    //                        }


                    //                    }

                }

                //Dictionary<string, double> getPricingArguments(DataRow pRecord)
                //{
                //    var d = new Dictionary<string, double>();



                //    foreach (DataColumn c in pRecord.Table.Columns)
                //        if (c.ColumnName.StartsWith("@"))
                //            d[c.ColumnName] = CASTASDOUBLE(TAB_GETROW(pRecord, c.ColumnName));

                //    return d;
                //}


                void calcPricingValue(DataRow pRecord)
                {
                    bool inited_ = PRICNGVARS.isLoaded(pRecord);

                    string func_ = getPriceCalcFormula();// "dbo.AVATEZ_$FIRM$_PRICING_" + FORMAT(PLUGIN.GETSYSPRM_USER());

                    if (!inited_)
                        fillPricingArguments(pRecord);

                    var listArgs = new List<object>();

                    foreach (string key in PRICNGVARS.LIST)
                    {

                        var colObj = pRecord.Table.Columns[key];
                        if (colObj == null)
                            throw new Exception("Cant find value for price calc [" + key + "]");

                        listArgs.Add(CASTASDOUBLE(CASTASDOUBLE(TAB_GETROW(pRecord, colObj.ColumnName))));
                    }

                    var val_ = ROUND(CASTASDOUBLE(ISNULL(SQLSCALAR(func_, listArgs.ToArray()), 0)), 2);
                    TAB_SETROW(pRecord, TABLE_STLINE.COLS.PRICE, val_);

                }


                //                void calcPricingValue(DataRow pRecord)
                //                {
                //                    bool inited_ = PRICNGVARS.isLoaded(pRecord);

                //                    string func_ = "dbo.AVATEZ_$FIRM$_PRICING_" + FORMAT(PLUGIN.GETSYSPRM_USER());

                //                    if (!inited_)
                //                        fillPricingArguments(pRecord);

                //                    List<string> list = new List<string>();
                //                    var args_ = getPricingArguments(pRecord);
                //                    foreach (string key in args_.Keys)
                //                    {

                //                        list.Add(string.Format(key + "={0}", FORMAT(args_[key])));
                //                    }

                //                    var exp_ = JOINLIST(list.ToArray());

                //                    //TODO
                //                    var sql_ = string.Format(@"
                //EXECUTE sp_executesql 
                //N'exec " + func_ + @" @TYPE=defaults, @PL=@PL,@PG=@PG,@PD=@PD, @PC=@PC, @PA=@PA, @PP=@PP, @SP=@SP, @FF=@FF', 
                //N'@PL FLOAT,@PG FLOAT,@PD FLOAT,@PC FLOAT,@PA FLOAT,@PP FLOAT,@SP FLOAT,@FF FLOAT',
                //{0}", exp_);
                //                    var val_ = ROUND(CASTASDOUBLE(ISNULL(SQLSCALAR(sql_, null), 0)), 2);
                //                    TAB_SETROW(pRecord, TABLE_STLINE.COLS.PRICE, val_);

                //                }


                void fillPricingArguments(DataRow pRecord)
                {
                    // string func_ = "dbo.AVATEZ_$FIRM$_PRICING_" + FORMAT(PLUGIN.GETSYSPRM_USER());


                    if (TABLE_STLINE.TOOLS.isLocalMat(pRecord))
                    {


                        string[] arrSql_ = new string[] { 
PRICNGVARS.PL,
MY_CHOOSE_SQL(
"SELECT TOP(1) (VATMATRAH+VATAMNT+DISTEXP)/AMOUNT FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK) WHERE STOCKREF = @P1 AND TRCODE = 1 AND CANCELLED=0 AND LINETYPE = 0 ORDER BY STOCKREF DESC,DATE_ DESC",
"SELECT (VATMATRAH+VATAMNT+DISTEXP)/AMOUNT FROM LG_$FIRM$_$PERIOD$_STLINE WHERE STOCKREF = @P1 AND TRCODE = 1 AND CANCELLED=0 AND LINETYPE = 0 ORDER BY STOCKREF DESC,DATE_ DESC LIMIT 1"),

PRICNGVARS.PG,
MY_CHOOSE_SQL(
"SELECT TOP(1) PRICE FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK) WHERE STOCKREF = @P1 AND TRCODE = 1 AND CANCELLED=0 AND LINETYPE = 0 ORDER BY STOCKREF DESC,DATE_ DESC",
"SELECT PRICE FROM LG_$FIRM$_$PERIOD$_STLINE WHERE STOCKREF = @P1 AND TRCODE = 1 AND CANCELLED=0 AND LINETYPE = 0 ORDER BY STOCKREF DESC,DATE_ DESC LIMIT 1"),

PRICNGVARS.PD,
MY_CHOOSE_SQL(
"SELECT TOP(1) (case when PRICE> 0 then 100*(PRICE-((VATMATRAH+VATAMNT+DISTEXP)/AMOUNT))/PRICE else 0 end) FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK) WHERE STOCKREF = @P1 AND TRCODE = 1 AND CANCELLED=0 AND LINETYPE = 0 ORDER BY STOCKREF DESC,DATE_ DESC",
"SELECT (case when PRICE> 0 then 100*(PRICE-((VATMATRAH+VATAMNT+DISTEXP)/AMOUNT))/PRICE else 0 end) FROM LG_$FIRM$_$PERIOD$_STLINE WHERE STOCKREF = @P1 AND TRCODE = 1 AND CANCELLED=0 AND LINETYPE = 0 ORDER BY STOCKREF DESC,DATE_ DESC LIMIT 1"),

PRICNGVARS.PC,
MY_CHOOSE_SQL(
"SELECT TOP(1) DIFFPRICE/AMOUNT FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK) WHERE STOCKREF = @P1 AND TRCODE = 1 AND CANCELLED=0 AND LINETYPE = 0 ORDER BY STOCKREF DESC,DATE_ DESC",
"SELECT DIFFPRICE/AMOUNT FROM LG_$FIRM$_$PERIOD$_STLINE WHERE STOCKREF = @P1 AND TRCODE = 1 AND CANCELLED=0 AND LINETYPE = 0 ORDER BY STOCKREF DESC,DATE_ DESC LIMIT 1"),

PRICNGVARS.PA,
MY_CHOOSE_SQL(
"DECLARE @V1 FLOAT,@V2 FLOAT;SELECT @V1=SUM(VATMATRAH+VATAMNT+DISTEXP),@V2=SUM(AMOUNT) FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK) WHERE STOCKREF = @P1 AND TRCODE = 1 AND CANCELLED=0 AND LINETYPE = 0 AND DATE_>= @P3; IF @V2>0 SELECT @V1/@V2 ELSE SELECT 0;",
@"
SELECT 
(CASE WHEN  V2>0  THEN  V1/V2 ELSE 0 END) V
FROM (
SELECT 
SUM(VATMATRAH+VATAMNT+DISTEXP) V1,
SUM(AMOUNT) V2
FROM LG_$FIRM$_$PERIOD$_STLINE 
WHERE STOCKREF = @P1 AND TRCODE = 1 AND CANCELLED=0 AND LINETYPE = 0 AND DATE_>= @P3  
) T
 

"),

PRICNGVARS.PP,
MY_CHOOSE_SQL(
"SELECT TOP(1) PRICE FROM LG_$FIRM$_PRCLIST WITH(NOLOCK) WHERE CARDREF = @P1 AND PTYPE = 1 ORDER BY ENDDATE DESC",
"SELECT PRICE FROM LG_$FIRM$_PRCLIST WHERE CARDREF = @P1 AND PTYPE = 1 ORDER BY ENDDATE DESC LIMIT 1"),

PRICNGVARS.SP,
MY_CHOOSE_SQL(
"SELECT TOP(1) PRICE FROM LG_$FIRM$_PRCLIST WITH(NOLOCK) WHERE CARDREF = @P1 AND PTYPE = 2 ORDER BY ENDDATE DESC",
"SELECT PRICE FROM LG_$FIRM$_PRCLIST WHERE CARDREF = @P1 AND PTYPE = 2 ORDER BY ENDDATE DESC LIMIT 1"),
PRICNGVARS.FF,
MY_CHOOSE_SQL(
"SELECT TOP(1) FLOATF1 FROM LG_$FIRM$_ITEMS WITH(NOLOCK) WHERE LOGICALREF = @P1",
"SELECT FLOATF1 FROM LG_$FIRM$_ITEMS WHERE LOGICALREF = @P1 LIMIT 1")
                                    };


                        var mref_ = TAB_GETROW(pRecord, TABLE_STLINE.COLS.STOCKREF);
                        var dateNow_ = DateTime.Now.Date;
                        var dateOld_ = dateNow_.AddDays(-60);


                        for (int i = 0; i < arrSql_.Length; i += 2)
                        {
                            var price_ = ROUND(CASTASDOUBLE(ISNULL(SQLSCALAR(arrSql_[i + 1], new object[] { mref_, dateNow_, dateOld_ }), 0.0)), 4);
                            TAB_SETROW(pRecord, arrSql_[i], price_);
                        }


                        var desc_ = ISNULL(SQLSCALAR(
                            MY_CHOOSE_SQL(
                            "SELECT TOP(1) SPECODE+'/'+SPECODE2 FROM LG_$FIRM$_ITEMS WITH(NOLOCK) WHERE LOGICALREF = @P1",
                            "SELECT SPECODE||'/'||SPECODE2 FROM LG_$FIRM$_ITEMS WHERE LOGICALREF = @P1 LIMIT 1"),
                            new object[] { mref_ }), "");

                        TAB_SETROW(pRecord, TABLE_STLINE.COLS.LINEEXP, desc_);

                        PRICNGVARS.setLoaded(pRecord);
                    }


                }


                string findText_ = string.Empty;
                void findText()
                {

                    try
                    {
                        var res_ = askString("T_TEXT", findText_);
                        if (string.IsNullOrEmpty(res_))
                            return;

                        findText_ = res_;

                        var v1 = findText_;
                        var v2 = findText_.ToLowerInvariant();


                        var rec_ = dataInputForm_.getCurrentRecord();
                        if (rec_ == null)
                            return;

                        var indx_ = tableData_.Rows.IndexOf(rec_);

                        if (indx_ < 0)
                            return;

                        List<int> list = new List<int>();
                        for (int i = indx_; i < tableData_.Rows.Count; ++i)
                            list.Add(i);

                        for (int i = 0; i < indx_ && i < tableData_.Rows.Count; ++i)
                            list.Add(i);


                        foreach (int i in list)
                        {
                            var row_ = tableData_.Rows[i];
                            if (!TAB_ROWDELETED(row_))
                            {
                                var strV1_ = CASTASSTRING(TAB_GETROW(row_, TABLE_STLINE.COLS.ITEMS_NAME));

                                var strV2_ = strV1_.ToLower();

                                if (strV1_.Contains(v1) || strV2_.Contains(v2))
                                {
                                    dataInputForm_.activateMainGridRow(i);
                                    return;
                                }

                            }
                        }


                        //    System.Console.Beep(500, 2000);

                        beep();



                    }
                    catch (Exception exc)
                    {
                        exceptionHandler(exc);
                        setMsgErr(exc.Message);
                    }


                }

                static void beep()
                {
                    if (PRM.BEEP)
                        System.Media.SystemSounds.Asterisk.Play();
                }
                static void beepErr()
                {
                    if (PRM.BEEP)
                    {
                        System.Media.SystemSounds.Asterisk.Play();
                        System.Threading.Thread.Sleep(600);
                        System.Media.SystemSounds.Asterisk.Play();
                    }
                }
                bool setSalesMan()
                {
                    if (_READONLY)
                        return false;

                    try
                    {

                        SalesManReference hanler_ = null;
                        try
                        {
                            var f = SalesManReference.Filter.All;

                            if (PRM.TERM_TYPE == TERMTYPE.restoran)
                                f = SalesManReference.Filter.CurrentClientAndNoClient;

                            hanler_ = new SalesManReference(PLUGIN, this, f);

                            hanler_.SEARCH(TABLE_SLSMAN.COLS.LOGICALREF, registers.slsMan);
                            DataRow[] res_ = hanler_.REF();
                            if (res_ != null && res_.Length > 0)
                            {
                                object slsman_ = _PLUGIN.TAB_GETROW(res_[0], TABLE_SLSMAN.COLS.LOGICALREF);
                                string desc_ = _PLUGIN.CASTASSTRING(_PLUGIN.TAB_GETROW(res_[0], TABLE_SLSMAN.COLS.DEFINITION_));
                                string code_ = _PLUGIN.CASTASSTRING(_PLUGIN.TAB_GETROW(res_[0], TABLE_SLSMAN.COLS.CODE));

                                if (PRM.TERM_TYPE == TERMTYPE.restoran)
                                {
                                    var cl_ = SalesManReference.getCurrentSlsManClient(PLUGIN, slsman_);
                                    var clCurr_ = registers.clcard.getClientRef();
                                    if (!ISEMPTYLREF(cl_) && !COMPARE(cl_, clCurr_))
                                    {
                                        setMsgErr(_LANG.L.MSG_ERROR_BINDING);
                                        return false;
                                    }
                                }




                                registers.slsMan = slsman_;
                                registers.slsManDesc = desc_;
                                registers.slsManCode = code_;
                                return true;

                            }
                        }
                        finally
                        {
                            if (hanler_ != null)
                                hanler_.Dispose();
                        }

                        return false;


                    }
                    catch (Exception exc)
                    {
                        exceptionHandler(exc);
                        setMsgErr(_LANG.L.MSG_ERROR_CREATE);
                    }
                    finally
                    {
                        setInputText(string.Empty);  //registers.inputText = string.Empty;
                    }

                    return false;
                }
                string askList(string pList)
                {
                    object val_ = askList(EXPLODELIST(pList), EXPLODELIST(pList));
                    if (val_ == null)
                        return null;

                    return CASTASSTRING(val_);
                }
                object askList(object[] pId, string[] pDesc)
                {

                    try
                    {

                        ListReference hanler_ = null;
                        try
                        {
                            hanler_ = new ListReference(PLUGIN, this, pId, pDesc);
                            // hanler_.SEARCH(TABLE_LIST.COLS.ID_, "");
                            DataRow[] res_ = hanler_.REF();
                            if (res_ != null && res_.Length > 0)
                            {

                                return (_PLUGIN.TAB_GETROW(res_[0], TABLE_LIST.COLS.ID_));
                            }
                        }
                        finally
                        {
                            if (hanler_ != null)
                                hanler_.Dispose();
                        }


                    }
                    catch (Exception exc)
                    {
                        exceptionHandler(exc);
                        setMsgErr(_LANG.L.MSG_ERROR_CREATE);
                    }
                    finally
                    {
                        setInputText(string.Empty);  //registers.inputText = string.Empty;
                    }

                    return null;
                }


                void setTrackno(string pVal)
                {

                    if (_READONLY)
                        return;

                    if (!PRM.USER_TRACK_NR)
                        return;


                    string text_ = pVal;

                    if (string.IsNullOrEmpty(pVal))
                    {
                        long val_ = 0;
                        try
                        {
                            if (_PLUGIN.CASTASSTRING(registers.trackno) != "")
                            {


                                val_ = _PLUGIN.CASTASLONG(registers.trackno);
                            }
                        }
                        catch { }


                        val_ = (long)askNumber("T_TEL", val_, 0);
                        if (val_ < 0)
                            return;

                        long min = 0000000000000;
                        long max = 9999999999999;

                        if (val_ < min || val_ > max)
                        {
                            setMsgErr(_LANG.L.MSG_ERROR_INVALID_VAL + " " + val_);
                            return;
                        }

                        text_ = (val_ == 0 ? string.Empty : _PLUGIN.FORMAT(val_));

                    }

                    if (registers.trackno != text_)
                    {
                        registers.trackno = text_;




                        refreshTots();
                        //backUpDoc();
                        //





                    }

                    if (PRM.USE_BONUS_MAQ)
                    {

                        registers.bonus = registers.useBonus = 0;

                        if (!string.IsNullOrEmpty(registers.trackno))
                        {
                            var _this = this;


                            var act = new Action(() =>
                            {
                                try
                                {
                                    var x = CASTASDOUBLE(ISNULL(TOOLREMOTESQL.SQLSCALAR(
                                    @"SELECT AMOUNT FROM t_MARKET_$FIRM$_$PERIOD$_BONUS WHERE CODE = @P1",

                                    new object[] { registers.trackno }), 0.0));

                                    _this.registers.bonus = x;

                                    _this.dataInputForm_.Invoke(new Action(_this.refreshTots));
                                }
                                catch (Exception e)
                                {
                                    _this.dataInputForm_.Invoke(new Action<string>(_this.setMsgErr), e.Message);
                                }
                            });

                            new System.Threading.Tasks.Task(() =>
                            {
                                RUNWITHTIMEOUT(act, 5);
                            }).Start();

                        }
                    }

                }


                void setDocDesc()
                {
                    if (_READONLY)
                        return;

                    var str_ = askString("T_DESC", registers.desc1);
                    if (str_ == null)
                        return;

                    registers.desc1 = str_;

                    refreshTots();

                }
                void setDocCode()
                {
                    if (_READONLY)
                        return;

                    var str_ = askString("T_CODE", registers.docCode);
                    if (str_ == null)
                        return;

                    registers.docCode = str_;

                    refreshTots();

                }
                void setDocSpeCode()
                {
                    if (_READONLY)
                        return;



                    var str_ = getSpeCode(1, 23, registers.speCode);
                    if (str_ == null)
                        return;

                    registers.speCode = str_;

                    refreshTots();

                }



                // void openNew()
                //  {
                //      this.COPY().BEGIN_TERMINAL(null);
                //  }
                void moveNext()
                {
                    List<Form> l = new List<Form>();
                    //1
                    foreach (Form f in Application.OpenForms)
                        l.Add(f);
                    //2 cycle
                    foreach (Form f in Application.OpenForms)
                        l.Add(f);


                    bool found_ = false;
                    foreach (Form f in l)
                    {

                        if (f.ToString().StartsWith(TerminalForm._NAME))
                        {
                            if (found_)
                            {
                                f.WindowState = FormWindowState.Maximized;
                                f.BringToFront();
                                f.Activate();

                                return;
                            }
                            else
                                if (object.ReferenceEquals(dataInputForm_, f))
                                    found_ = true;

                        }
                    }


                }
                void userToCash()
                {
                    if (PRM.NO_PAYMENT)
                        return;

                    if (_CANCELLED)
                        return;

                    double val_ = askNumber(_LANG.L.PAYMENT, registers.totalNet);
                    if (val_ < 0)
                        return;

                    registers.userToCashAmount = val_;

                    refreshTots();

                }


                void changeCash()
                {
                    if (PRM.NO_PAYMENT)
                        return;

                    if (_CANCELLED)
                        return;

                    registers.nextCash(PLUGIN);

                    refreshTots();

                }

                void changeAllVAT()
                {
                    if (!PRM.EDIT_VAT)
                        return;

                    if (_READONLY)
                        return;

                    if (!askPassword())
                        return;

                    var vat = askNumber("T_VAT", 0, 0);

                    if (vat < 0)
                        return;

                    vat = MIN(99, vat);


                    var tab = this.dataInputForm_.tableData;

                    foreach (DataRow row in tab.Rows)
                    {
                        if (!TAB_ROWDELETED(row))
                            if (TABLE_STLINE.TOOLS.isLocalMat(row))
                            {
                                TAB_SETROW(row, TABLE_STLINE.COLS.VAT, vat);
                            }
                    }
                }



                void useBonus()
                {
                    if (!PRM.USE_BONUS_MAQ)
                        return;

                    if (_CANCELLED)
                        return;

                    if (string.IsNullOrEmpty(registers.trackno))
                        return;


                    var bonus = Math.Max(registers.bonus, 0.0);

                    var totalNet = Math.Max(registers.totalNet, 0.0);



                    if (ISNUMZERO(bonus))
                        return;


                    var def_ = ISNUMZERO(registers.useBonus) ? totalNet - Math.Floor(totalNet) : registers.useBonus;

                    double val_ = askNumber(_LANG.L.BONUS + " [Max: " + FORMAT(registers.bonus, "N2") + "]", def_, 2);
                    if (val_ < 0)
                        return;

                    val_ = Math.Min(bonus, val_);


                    registers.useBonus = val_;

                    refreshTots();

                }
                /*
                string getBackupDir()
                {
                    string dir_ = (PATHCOMBINE(GETWORKDIR(), "AVATEZ" + _TRCODE));
                    System.IO.Directory.CreateDirectory(dir_);
                    return dir_;
                }
                void backUpDoc()
                {
                    try
                    {
                        if (!PRM.DOC_BACKUP)
                            return;


                        if (_TRCODE == 3)
                            return;

                        string dir_ = getBackupDir();
                        string file_ = PATHCOMBINE(dir_, registers.getDocCode(PLUGIN));

                        DataSet ds_ = new DataSet();

                        ds_.Tables.Add(tableData_.Copy());
                        ds_.Tables.Add(tableDataHeader_.Copy());
                        registers.writeBackUp(ds_);

                        DSXMLWRITE(file_, ds_);
                    }
                    catch (Exception exc)
                    {
                        exceptionHandler(exc);
                    }

                }


                void cleanBackUp()
                {
                    try
                    {
                        string dir_ = getBackupDir();
                        string[] files = System.IO.Directory.GetFiles(dir_);
                        foreach (string f in files)
                            System.IO.File.Delete(f);
                    }
                    catch { }
                }

                void unBackUpDoc()
                {

                    if (!PRM.DOC_BACKUP)
                        return;

                    if (_TRCODE == 3)
                        return;

                    string path_ = null;
                    try
                    {
                        string dir_ = getBackupDir();
                        string[] files = System.IO.Directory.GetFiles(dir_);
                        if (files.Length == 0)
                            return;
                        path_ = files[0];

                        string fileName_ = System.IO.Path.GetFileName(path_);

                        if (ISNULL(SQLSCALAR("SELECT 'OK' FROM LG_$FIRM$_$PERIOD$_INVOICE WITH(NOLOCK) WHERE FICHENO = @P1", new object[] { fileName_ })))
                        {
                            var ds_ = DSXMLREAD(path_);
                            var data_ = ds_.Tables[tableData_.TableName];
                            registers.readBackUp(ds_);
                            tableData_.Load(data_.CreateDataReader());

                            refreshTots();
                        }


                    }
                    catch (Exception exc)
                    {
                        exceptionHandler(exc);
                    }
                    finally
                    {
                      //  cleanBackUp();

                    }

                 //   backUpDoc();
                }

                */
                void applyCampagin(bool pAuto, string pCode)
                {

                    if (!PRM.APPLY_CAMPAGIN)
                        return;

                    try
                    {
                        string code_ = null;

                        if (registers.clcard.isClientRefEmpty())
                            return;

                        string clPromoCode_ = ISNULL(registers.clcard.getValue("SPECODE4"), string.Empty) as string;

                        var payPlanLRef_ = registers.clcard.getPaymentRef();

                        var payPlanCode_ = ISEMPTYLREF(payPlanLRef_) ? "" :
                          ISNULL(PLUGIN.SQLSCALAR(
                          MY_CHOOSE_SQL(
                          "SELECT TOP(1) PP.CODE FROM LG_$FIRM$_PAYPLANS PP WITH(NOLOCK) WHERE PP.LOGICALREF=@P1",
                             "SELECT PP.CODE FROM LG_$FIRM$_PAYPLANS PP WHERE PP.LOGICALREF=@P1 LIMIT 1"),
                          new object[] { payPlanLRef_ }), "") as string;

                        if (string.IsNullOrEmpty(payPlanCode_))
                            payPlanCode_ = "";

                        var raw_ = PLUGIN.SQL(
                            MY_CHOOSE_SQL(
@"SELECT CODE,CODE+'/'+NAME NAME,SPECODE4 
FROM 
LG_$FIRM$_CAMPAIGN H WITH(NOLOCK) 
WHERE 
CARDTYPE = 2 AND
@P1 BETWEEN BEGDATE AND ENDDATE AND 
ACTIVE = 0 AND
CYPHCODE = @P2 AND
PAYPLANCODE IN (@P3,'','*')
ORDER BY H.CODE 

",
 @"SELECT CODE,CODE||'/'||NAME NAME,SPECODE4 
FROM 
LG_$FIRM$_CAMPAIGN H   
WHERE 
CARDTYPE = 2 AND
@P1 BETWEEN BEGDATE AND ENDDATE AND 
ACTIVE = 0 AND
CYPHCODE = @P2 AND
PAYPLANCODE IN (@P3,'','*')
ORDER BY H.CODE 

"),

 new object[] { DateTime.Now.Date, PRM.CAMPAGIN_CYPHCODE, payPlanCode_ });

                        if (pAuto)
                        {
                            List<string> list = new List<string>();

                            if (!string.IsNullOrEmpty(clPromoCode_))
                                foreach (DataRow x in raw_.Rows)
                                {
                                    var promoCode_ = CASTASSTRING(ISNULL(x["CODE"], ""));
                                    var promoFilter_ = CASTASSTRING(ISNULL(x["SPECODE4"], ""));
                                    foreach (var s in EXPLODELIST(promoFilter_))
                                        if (s == clPromoCode_)
                                            list.Add(promoCode_);

                                }

                            if (list.Count > 0)
                                code_ = list[0];// JOINLIST(list.ToArray());

                        }
                        else
                            if (string.IsNullOrEmpty(pCode))
                            {

                                //
                                List<string> list = new List<string>();
                                foreach (DataRow x in raw_.Rows)
                                {
                                    list.Add(CASTASSTRING(x["CODE"]).Replace(',', '_').Replace(' ', '_'));
                                    list.Add(CASTASSTRING(x["NAME"]).Replace(',', '_').Replace(' ', '_'));
                                }

                                list.Add("_clear");
                                list.Add(PLUGIN.RESOLVESTR("[lang::T_CLEAR]"));


                                // list.Add("_auto");
                                // list.Add(PLUGIN.RESOLVESTR("[lang::T_GENERAL]"));

                                var res_ = PLUGIN.REF("ref.gen.definedlist [obj::" + JOINLIST(list.ToArray()) + "] desc::Kampaniyalar type::string");

                                code_ = (res_ != null && res_.Length > 0 ? CASTASSTRING(res_[0]["VALUE"]) : null);
                                //
                            }
                            else
                                if (!string.IsNullOrEmpty(pCode))
                                    code_ = pCode;

                        if (code_ != null)
                        {
                            if (code_ == "_auto")
                            {
                                applyCampagin(pAuto, null);
                                return;
                            }
                            else
                            {

                                Dictionary<string, object> dic = new Dictionary<string, object>();

                                dic["promocardcode"] = code_;

                                registers.promoCode = code_;

                                //TODO Event with args
                                // PLUGIN.PLUGIN("plugin.sys.event.dochandler.pls", "MY_STOCK_PROMO2", new object[] { dataSet_, dic });

                            }

                        }




                    }
                    catch (Exception exc)
                    {
                        exceptionHandler(exc);
                        setMsgErr(_LANG.L.MSG_ERROR_PROMO);
                    }
                    finally
                    {

                    }

                }

                void setManualDiscountPerc()
                {
                    if (_READONLY)
                        return;


                    if (!PRM.USE_MANUAL_DISCOUNT)
                        return;


                    if (!PRM.USE_MANUAL_DISCOUNT_PERC)
                        return;

                    if (!askPassword())
                        return;

                    double val_ = askNumber("T_DISCOUNT %", 0);
                    if (val_ < 0)
                        return;

                    val_ = Math.Min(100, val_);
                    //
                    registers.discountAmountManual = 0;
                    registers.discountPercManual = val_;
                    registers.clcard.setClientDisc(0);
                    //
                    refreshTots();

                }




                void setManualDiscountAmount()
                {

                    if (_READONLY)
                        return;

                    if (!PRM.USE_MANUAL_DISCOUNT)
                        return;

                    if (!PRM.USE_MANUAL_DISCOUNT_TOT)
                        return;

                    if (!askPassword())
                        return;

                    double val_ = askNumber("T_DISCOUNT " + _LANG.L.CURRENCY + "", 0);
                    if (val_ < 0)
                        return;

                    registers.discountAmountManual = val_;
                    registers.discountPercManual = 0;
                    registers.clcard.setClientDisc(0);

                    refreshTots();

                }
                void setMonth()
                {
                    if (PRM.TERM_TYPE != TERMTYPE.credit)
                        return;

                    double val_ = askNumber("T_MONTH", 0);
                    if (val_ < 0)
                        return;

                    registers.month = (int)val_;

                    refreshTots();

                }

                void keyPress(object sender, KeyPressEventArgs e)
                {
                    if (Char.IsLetterOrDigit(e.KeyChar) || e.KeyChar == '_' || e.KeyChar == '.')
                    {
                        registers.inputText = registers.inputText + e.KeyChar;
                        onInputTextChanged();
                    }

                }



                void setInputText(string pStr)
                {
                    registers.inputText = pStr;
                    onInputTextChanged();
                }

                class MATRECORD
                {
                    public string BARCODE;
                    public object ITEMREF;
                    public object UNITREF;
                    public double PRICE;
                    public double AMOUNT = 1;

                    public DataRow ROW_ITEM;
                    public DataRow ROW_ITEMBARCODE;
                    public DataRow ROW_ITEMUNITEXT;
                    public DataRow ROW_ITEMPRICE;



                }
                bool fillMatData(object pMatLref, MATRECORD pObj)
                {



                    pObj.BARCODE = "";
                    pObj.ITEMREF = pMatLref;

                    pObj.ROW_ITEM = SEARCH_MAT_LINE(pObj.ITEMREF);
                    if (pObj.ROW_ITEM == null)
                    {
                        ERROR_INVALID_MAT_REF(pObj.ITEMREF);
                        return false;
                    }

                    pObj.ROW_ITEMBARCODE = null;
                    pObj.ROW_ITEMUNITEXT = SEARCH_MAT_UNIT_MAIN(pObj.ITEMREF);
                    if (pObj.ROW_ITEMUNITEXT == null)
                    {
                        ERROR_INVALID_MAT_UNIT(pObj.ITEMREF);
                        return false;
                    }

                    pObj.UNITREF = TAB_GETROW(pObj.ROW_ITEMUNITEXT, TABLE_ITMUNITA.COLS.UNITLINEREF);


                    pObj.ROW_ITEMPRICE = _SEARCH_MAT_PRICE(pObj.ITEMREF, pObj.UNITREF, PRM.PRICE_TYPE, registers.clcard.getPaymentRef());

                    if (pObj.ROW_ITEMPRICE == null)
                    {
                        if (!PRM.USE_IF_NO_SLS_PRICE)
                        {
                            ERROR_INVALID_MAT_PRICE(pObj.ITEMREF, pObj.UNITREF);
                            return false;
                        }
                    }

                    pObj.PRICE = pObj.ROW_ITEMPRICE == null ? 0 : _PLUGIN.CASTASDOUBLE(_PLUGIN.ISNULL(TAB_GETROW(pObj.ROW_ITEMPRICE, TABLE_PRCLIST.COLS.PRICE), 0.0));

                    return true;
                }
                bool fillMatData(TABLE_UNITBARCODE.TOOLS.PARSER pMatBarcode, MATRECORD pObj)
                {
                    pObj.PRICE = pMatBarcode.PRICE;
                    pObj.AMOUNT = ISNUMZERO(pMatBarcode.WEIGHT) ? pObj.AMOUNT : pMatBarcode.WEIGHT;


                    pObj.ROW_ITEMBARCODE = SEARCH_MAT_LINE_BARCODE(pMatBarcode.CODE1);
                    if (pObj.ROW_ITEMBARCODE == null)
                    {
                        beepErr();
                        ERROR_INVALID_MAT_BARCODE(pMatBarcode.BARCODE, false);
                        return false;
                    }

                    pObj.BARCODE = pMatBarcode.CODE1;
                    pObj.ITEMREF = TAB_GETROW(pObj.ROW_ITEMBARCODE, "ITEMREF");
                    pObj.UNITREF = TAB_GETROW(pObj.ROW_ITEMBARCODE, "UNITLINEREF");

                    pObj.ROW_ITEM = SEARCH_MAT_LINE(pObj.ITEMREF);
                    if (pObj.ROW_ITEM == null)
                    {
                        beepErr();
                        ERROR_INVALID_MAT_REF(pObj.ITEMREF);
                        return false;
                    }
                    //pObj.ROW_ITEMBARCODE = pObj.ROW_ITEMBARCODE;
                    pObj.ROW_ITEMUNITEXT = SEARCH_MAT_UNIT(pObj.ITEMREF, pObj.UNITREF);
                    if (pObj.ROW_ITEMUNITEXT == null)
                    {
                        beepErr();
                        ERROR_INVALID_MAT_UNIT(pObj.ITEMREF);
                        return false;
                    }


                    if (ISNUMZERO(pObj.PRICE))
                    {

                        pObj.ROW_ITEMPRICE = _SEARCH_MAT_PRICE(pObj.ITEMREF, pObj.UNITREF, PRM.PRICE_TYPE, registers.clcard.getPaymentRef());
                        if (pObj.ROW_ITEMPRICE == null)
                        {
                            if (!PRM.USE_IF_NO_SLS_PRICE)
                            {
                                beepErr();
                                ERROR_INVALID_MAT_PRICE(pObj.ITEMREF, pObj.UNITREF);
                                return false;
                            }
                        }

                        pObj.PRICE = pObj.ROW_ITEMPRICE == null ? 0 : _PLUGIN.CASTASDOUBLE(TAB_GETROW(pObj.ROW_ITEMPRICE, TABLE_PRCLIST.COLS.PRICE));
                    }

                    return true;
                }

                void processMatFromReference()
                {
                    if (!PRM.MAT_FROM_LIST)
                        return;
                    //
                    try
                    {
                        if (PRM.MAT_SELECT_FORM_BASE)
                        {

                            DataRow[] res_ = REF("ref.mm.rec.mat", "NAME", "");
                            if (res_ != null && res_.Length > 0)
                            {
                                object mLRef_ = TAB_GETROW(res_[0], "LOGICALREF");
                                MATRECORD obj_ = new MATRECORD();
                                if (fillMatData(mLRef_, obj_))
                                {

                                    if (PRM.TERM_TYPE == TERMTYPE.count)
                                        obj_.AMOUNT = 0;

                                    if (PRM.TERM_TYPE == TERMTYPE.pricing)
                                        obj_.AMOUNT = 0;

                                    addRecord(obj_);
                                }
                            }
                        }
                        else
                        {

                            if (PRM.TERM_TYPE == TERMTYPE.hotel)
                            {

                                MatReferenceByDate hanler_ = null;
                                try
                                {
                                    hanler_ = new MatReferenceByDate(PLUGIN, this, true);
                                    hanler_.REF();
                                }
                                finally
                                {
                                    if (hanler_ != null)
                                        hanler_.Dispose();
                                }
                            }
                            else
                            {
                                MatReference hanler_ = null;
                                try
                                {
                                    hanler_ = new MatReference(PLUGIN, this);
                                    hanler_.REF();
                                }
                                finally
                                {
                                    if (hanler_ != null)
                                        hanler_.Dispose();
                                }
                            }

                        }
                    }
                    catch (Exception exc)
                    {
                        exceptionHandler(exc);
                        setMsgErr(_LANG.L.MSG_ERROR_CREATE);
                    }
                    finally
                    {
                        setInputText(string.Empty);  //registers.inputText = string.Empty;
                    }
                }


                void processClientFromReference()
                {
                    if (!PRM.CLIENT_SELECT)
                        return;
                    //
                    try
                    {
                        if (PRM.CLIENT_SELECT_FORM_BASE)
                        {
                            string clCode_ = registers.clcard.getClientCode();

                            DataRow[] res_ = REF("ref.fin.rec.client", clCode_ != "" ? "CODE" : "DEFINITION_", clCode_);
                            if (res_ != null && res_.Length > 0)
                            {
                                if (this._READONLY)
                                    return;

                                string code_ = TAB_GETROW(res_[0], "CODE").ToString();
                                setClientByCode(code_);


                                //   object mLRef_ = TAB_GETROW(res_[0], "LOGICALREF");
                                //   registers.clcard.setClientRef(mLRef_, PLUGIN);

                            }
                        }

                    }
                    catch (Exception exc)
                    {
                        exceptionHandler(exc);
                        setMsgErr(_LANG.L.MSG_ERROR_CLIENT);
                    }
                    finally
                    {
                        setInputText(string.Empty);  //registers.inputText = string.Empty;
                    }
                }
                void setFilterGroup()
                {
                    registers.filterGroup = getSpeCode(4, 0, registers.filterGroup);
                    refreshTots();
                }
                void setFilterSpeCode()
                {
                    registers.filterSpeCode = getSpeCode(1, 1, registers.filterSpeCode);
                    refreshTots();
                }
                void setFilterSpeCode2()
                {
                    registers.filterSpeCode2 = getSpeCode(502, 1, registers.filterSpeCode2);
                    refreshTots();
                }

                string getSpeCode(short t1, short t2, string val)
                {

                    try
                    {


                        DataRow[] res_ = REF(string.Format("ref.gen.rec.mcodes/{0}/{1}", t1, t2), "SPECODE", val);
                        if (res_ != null && res_.Length > 0)
                        {
                            return TAB_GETROW(res_[0], "SPECODE").ToString();


                        }


                    }
                    catch (Exception exc)
                    {
                        exceptionHandler(exc);
                        setMsgErr(exc.Message);
                    }


                    return null;
                }

                void openSameNewWin()
                {
                    if (_READONLY)
                        return;


                    var prm = PRM.COPY_PRM();


                    prm["_parentFormIndex"] = FORMAT(this.dataInputForm_.formIndex);


                    PLUGIN.SYSUSEREVENT("_barcodeterm_", new object[] { prm });

                    //PLUGIN.PLUGIN("plugin.sys.event.barcodeterm.pls", "SYS_BEGIN_PRM", new object[]{
                    //    prm
                    //});





                }
                void openSalesReturn()
                {
                    if (_READONLY)
                        return;


                    var prm = PRM.COPY_PRM();

                    prm["trcode"] = "3"; //override
                    prm["_parentFormIndex"] = FORMAT(this.dataInputForm_.formIndex);

                    PLUGIN.SYSUSEREVENT("_barcodeterm_", new object[] { prm });

                    //PLUGIN.PLUGIN("plugin.sys.event.barcodeterm.pls", "SYS_BEGIN_PRM", new object[]{
                    //    prm
                    //});



                }

                void fillDoc()
                {
                    if (!PRM.FILL_DOC)
                        return;


                    short fillType_ = 0;

                    if (PRM.TERM_TYPE == TERMTYPE.count)
                        fillType_ = 1;
                    else
                        if (PRM.TERM_TYPE == TERMTYPE.production)
                            fillType_ = 2;
                        else
                            if (PRM.TERM_TYPE == TERMTYPE.pricing || PRM.TERM_TYPE == TERMTYPE.barcode)
                            {
                                if (registers.hasFilter() && MSGUSERASK("" + _LANG.L.MSG_FILL_BY_FILTER + ""))
                                    fillType_ = 1;
                                else
                                    fillType_ = 2;
                            }


                    switch (fillType_)
                    {
                        case 1:
                            {
                                DataTable tabData_ = tableData_;

                                if (PRM.TERM_TYPE == TERMTYPE.count)
                                    if (registers.warehouse < 0)
                                    {
                                        setMsgErr(_LANG.L.MSG_ERROR_WH);
                                        return;
                                    }


                                if (tabData_.Rows.Count > 0)
                                {
                                    if (MSGUSERASK(_LANG.L.MSG_ASK_CLEAN))
                                        tabData_.Clear();
                                    else
                                        return;
                                }

                                DataTable x = MatReference.getMats(PLUGIN, registers.filterGroup, registers.filterSpeCode, registers.filterSpeCode2, 1, 0, 0, 0, PRM.PRICE_TYPE);

                                foreach (DataRow row in x.Rows)
                                {

                                    var mlref = TAB_GETROW(row, TABLE_ITEMS.COLS.LOGICALREF);
                                    var ulref = TAB_GETROW(row, TABLE_ITEMS.COLS.UNITSETL_LOGICALREF);
                                    var code_ = CASTASSTRING(TAB_GETROW(row, TABLE_ITEMS.COLS.CODE));
                                    var name_ = CASTASSTRING(TAB_GETROW(row, TABLE_ITEMS.COLS.NAME));
                                    var unitLref_ = TAB_GETROW(row, TABLE_ITEMS.COLS.UNITSETL_LOGICALREF);
                                    var unitCode_ = CASTASSTRING(TAB_GETROW(row, TABLE_ITEMS.COLS.UNITSETL_CODE));
                                    var price_ = CASTASDOUBLE(_PLUGIN.ISNULL(TAB_GETROW(row, TABLE_ITEMS.COLS.PRCLIST_PRICE), 0));
                                    var amt = 0;
                                    if (PRM.TERM_TYPE == TERMTYPE.barcode)
                                        amt = 1;

                                    var rec_ = addRecord(mlref, code_, name_, unitLref_, unitCode_, 0, price_, amt, true);

                                }
                            }
                            break;
                        case 2:
                            {
                                DataSet x = _MY_STOCKCONTENTFORFILL(PLUGIN);

                                if (x == null)
                                    return;

                                DataTable lines_ = x.Tables["STLINE"];

                                foreach (DataRow row in lines_.Rows)
                                {
                                    if (!TAB_ROWDELETED(row))
                                        if (TABLE_STLINE.TOOLS.isLocalMat(row) || TABLE_STLINE.TOOLS.isLocalPromo(row))
                                        {
                                            var mat = new MATRECORD();
                                            var mlref = TAB_GETROW(row, TABLE_STLINE.COLS.STOCKREF);
                                            var amt = CASTASDOUBLE(TAB_GETROW(row, TABLE_STLINE.COLS.AMOUNT));



                                            if (!PRM.USE_LINE_AMNT_ON_FILL)
                                            {
                                                if (PRM.TERM_TYPE == TERMTYPE.barcode)
                                                    amt = 1;
                                            }

                                            if (PRM.TERM_TYPE == TERMTYPE.barcode)
                                                amt = ROUND(amt, 0);

                                            fillMatData(mlref, mat);
                                            mat.AMOUNT = amt;

                                            var rec_ = addRecord(mat);
                                            if (rec_ != null)
                                            {

                                                //LINEEXP

                                                //if (PRM.TERM_TYPE == TERMTYPE.barcode)
                                                {
                                                    var currDesc_ = TAB_GETROW(rec_, TABLE_STLINE.COLS.LINEEXP).ToString();
                                                    var dbDesc_ = TAB_GETROW(row, TABLE_STLINE.COLS.LINEEXP).ToString();
                                                    TAB_SETROW(rec_, TABLE_STLINE.COLS.LINEEXP, currDesc_ + (dbDesc_ != "" ? "/" + dbDesc_ : ""));
                                                }

                                            }
                                        }
                                }
                            }
                            break;
                    }





                }
                void processWarehouseFromReference()
                {
                    if (!PRM.WAREHOUSE_SELECT)
                        return;
                    //
                    try
                    {
                        if (PRM.WAREHOUSE_SELECT_FORM_BASE)
                        {
                            short wh_ = registers.warehouse;

                            DataRow[] res_ = REF("ref.gen.rec.wh", "NR", wh_);
                            if (res_ != null && res_.Length > 0)
                            {
                                registers.warehouse = CASTASSHORT(TAB_GETROW(res_[0], "NR"));
                                registers.warehouseDesc = CASTASSTRING(TAB_GETROW(res_[0], "NAME"));
                                registers.warehouseDesc = string.Format("{0}/{1}", registers.warehouse, registers.warehouseDesc);

                                REFRESH_ONHAND();

                                refreshTots();

                            }
                        }

                    }
                    catch (Exception exc)
                    {
                        exceptionHandler(exc);
                        setMsgErr(_LANG.L.MSG_ERROR_WH);
                    }
                    finally
                    {
                        setInputText(string.Empty);  //registers.inputText = string.Empty;
                    }
                }



                void setWarehouse(short pNr)
                {

                    registers.warehouse = pNr;
                    registers.warehouseDesc = (pNr >= 0 ?
                        CASTASSTRING(SQLSCALAR(
                        MY_CHOOSE_SQL(
                        "SELECT TOP(1) NAME FROM L_CAPIWHOUSE WITH(NOLOCK) WHERE NR = @P1 AND FIRMNR = $FIRM$",
                         "SELECT NAME FROM L_CAPIWHOUSE WHERE NR = @P1 AND FIRMNR = $FIRM$ LIMIT 1"),
                        new object[] { pNr })) : "");




                }
                void processInputText()
                {
                    //
                    try
                    {
                        processBarcode(registers.inputText);
                    }
                    finally
                    {
                        setInputText(string.Empty);  // registers.inputText = string.Empty;
                    }
                }
                DataRow getCurrentRecord()
                {
                    return dataInputForm_.getCurrentRecord();

                }
                void processMathOperQuantity(string pCmd)
                {

                    if (!MathOper.isValid(pCmd))
                    {
                        ERROR_INVALID_MATH(pCmd);
                        return;
                    }

                    DataRow record_ = getCurrentRecord();

                    if (isReadOnly(record_))
                        return;

                    double val_ = askNumber("T_QUANTITY " + pCmd + "", 0);
                    if (val_ < 0)
                        return;

                    double quantity_ = CASTASDOUBLE(ISNULL(TAB_GETROW(record_, TABLE_STLINE.COLS.AMOUNT), 0.0));
                    double newQuantity_ = 0;
                    //
                    try
                    {

                        switch (pCmd)
                        {
                            case MathOper.sum:
                                newQuantity_ = ABS(quantity_ + val_);
                                break;
                            case MathOper.sub:
                                newQuantity_ = MAX(0, (quantity_ - val_));
                                break;
                            case MathOper.mult:
                                newQuantity_ = ABS(quantity_ * val_);
                                break;
                            case MathOper.div:
                                newQuantity_ = ABS(quantity_ / val_);
                                break;

                        }



                        switch (pCmd)
                        {
                            case MathOper.sub:
                            case MathOper.div:
                                if (PRM.USE_PASSWORD_FOR_AMOUNT)
                                    if (!askPassword())
                                        return;
                                logDbAmountChanged(record_, quantity_, newQuantity_);
                                break;

                        }

                        TAB_SETROW(record_, TABLE_STLINE.COLS.AMOUNT, newQuantity_);


                        refreshTots();

                        // backUpDoc();
                    }
                    finally
                    {
                        setInputText(string.Empty);
                    }
                }
                void onInputTextChanged()
                {
                    if (this.dataInputForm_ != null)
                    {
                        if (registers.firstInputText)
                        {
                            registers.formText = this.dataInputForm_.Text;
                            registers.firstInputText = false;
                        }

                        if (registers.inputText == null)
                            registers.inputText = "";

                        //  if (PRM.FULL_SCREEN)
                        {
                            if (registers.inputText != "")
                                setMsgErr(registers.inputText);
                        }
                        //else
                        //  this.dataInputForm_.Text = registers.formText + (registers.inputText == "" ? "" : " [" + registers.inputText + "]");
                    }
                }



                bool isBarcodeCmd(string pBarcode)
                {
                    return pBarcode.StartsWith("!");
                }
                void processBarcodeCmd(string pBarcode)
                {

                    var cmdType_ = pBarcode[0];

                    var arr_ = EXPLODELISTSEP((RIGHT(pBarcode, pBarcode.Length - 1)), '!');

                    //!k!NEQD
                    //!s!EXPORT
                    switch (cmdType_)
                    {
                        case '!':
                            if (arr_.Length >= 2)
                            {
                                switch (arr_[0].ToLower())
                                {
                                    case "k":
                                        registers.docCode = arr_[1];
                                        break;
                                    case "h":
                                        registers.speCode = arr_[1];
                                        break;
                                    case "d":
                                        registers.desc1 = arr_[1];
                                        break;
                                }

                            }

                            break;


                    }



                    refreshTots();
                    // backUpDoc();


                }


                void processBarcode(string pBarcode)
                {
                    if (pBarcode == null)
                        return;
                    pBarcode = pBarcode.Trim(); //.TrimStart('0');

                    if (pBarcode == string.Empty)
                        return;

                    setMsgErr("");




                    try
                    {


                        //





                        //

                        if (isBarcodeCmd(pBarcode))
                        {
                            processBarcodeCmd(pBarcode);

                        }
                        else
                        {



                            var p = new TABLE_UNITBARCODE.TOOLS.PARSER(pBarcode);

                            if (p.CODE1 != string.Empty) //weigth barcode or normal
                            {
                                MATRECORD obj_ = new MATRECORD();
                                if (fillMatData(p, obj_))
                                {

                                    if (PRM.TERM_TYPE == TERMTYPE.count)
                                        obj_.AMOUNT = 0;

                                    addRecord(obj_);
                                }
                            }
                            else
                                if (p.CODE2 != string.Empty) //client
                                {

                                    if (PRM.CLIENT_SELECT)
                                    {
                                        if (setClientByBarCode(p.CODE2))
                                            if (PRM.CLIENT_TO_TRACK)
                                            {
                                                setTrackno(p.CODE2);

                                            }
                                    }
                                    else
                                        setMsgErr("Client barcode not allowed in this mode");
                                }
                                else
                                    if (p.CODE3 != string.Empty) //track
                                    {
                                        setTrackno(p.CODE3);




                                    }
                                    else
                                    {
                                        beepErr();
                                        setMsgErr("Empty barcode");
                                    }


                        }

                    }
                    catch (Exception exc)
                    {
                        exceptionHandler(exc);
                        ERROR_INVALID_MAT_BARCODE(pBarcode, false);
                    }





                }






                void exceptionHandler(Exception exc)
                {
                    LOG(exc.ToString());
                }
                void EXEADPCMD(string pCmd, System.ComponentModel.DoWorkEventHandler pEvent)
                {
                    PLUGIN.EXEADPCMD(new string[] { pCmd }, new System.ComponentModel.DoWorkEventHandler[] { pEvent });
                }
                string RESOLVESTR(string pStr)
                {
                    return PLUGIN.RESOLVESTR(pStr);
                }

                void MSGUSERINFO(string pMsg)
                {
                    PLUGIN.MSGUSERINFO(pMsg);
                }


                bool MSGUSERASK(string pMsg)
                {
                    return PLUGIN.MSGUSERASK(pMsg);
                }
                void LOG(string pMsg)
                {
                    PLUGIN.LOG(pMsg);
                }
                DataRow[] REF(string pStr)
                {
                    return PLUGIN.REF(pStr);
                }
                DataRow[] REF(string pStr, string pCol, object pVal)
                {
                    return PLUGIN.REF(pStr, pCol, pVal);
                }
                DataTable SQL(string pSql, object[] pArr)
                {
                    return PLUGIN.SQL(pSql, pArr);
                }
                object SQLSCALAR(string pSql, object[] pArr)
                {
                    return PLUGIN.SQLSCALAR(pSql, pArr);
                }




                DataRow addRecord(MATRECORD pRecord)
                {



                    var r_ = addRecord(
                              pRecord.ITEMREF,
                             _PLUGIN.CASTASSTRING(TAB_GETROW(pRecord.ROW_ITEM, TABLE_ITEMS.COLS.CODE)),
                               _PLUGIN.CASTASSTRING(TAB_GETROW(pRecord.ROW_ITEM, TABLE_ITEMS.COLS.NAME)),
                              pRecord.UNITREF,
                              _PLUGIN.CASTASSTRING(TAB_GETROW(pRecord.ROW_ITEMUNITEXT, TABLE_UNITSETL.COLS.CODE)),
                              pRecord.ROW_ITEMBARCODE != null ? TAB_GETROW(pRecord.ROW_ITEMBARCODE, TABLE_UNITBARCODE.COLS.LOGICALREF) : 0,
                              pRecord.PRICE,
                              pRecord.AMOUNT,
                              !PRM.SAME_MAT_SUM);

                    //
                    setDataGridCurrRecPos(r_); //last

                    return r_;
                }

                void refreshDoc()
                {

                    foreach (DataRow row in tableData_.Rows)
                        if (!TAB_ROWDELETED(row))
                        {
                            object mref_ = TAB_GETROW(row, TABLE_STLINE.COLS.STOCKREF);
                            object uref_ = TAB_GETROW(row, TABLE_STLINE.COLS.UOMREF);
                            if (!ISEMPTYLREF(mref_) && !ISEMPTYLREF(uref_))
                            {
                                {
                                    DataRow rowS_ = _SEARCH_MAT_PRICE(mref_, uref_, PRM.PRICE_TYPE, registers.clcard.getPaymentRef());
                                    double priceS_ = rowS_ != null ? CASTASDOUBLE(TAB_GETROW(rowS_, TABLE_PRCLIST.COLS.PRICE)) : 0;
                                    TAB_SETROW(row, TABLE_STLINE.COLS.PRICE, priceS_);

                                    if (rowS_ == null)
                                        ERROR_INVALID_MAT_PRICE(mref_, uref_);
                                }

                                if (PRM.PRICE_DIFF_BY_PRCH)
                                {
                                    DataRow rowP_ = SEARCH_MAT_PRICE_PRCH(mref_, uref_, registers.clcard.getPaymentRef());

                                    double priceP_ = rowP_ != null ? CASTASDOUBLE(TAB_GETROW(rowP_, TABLE_PRCLIST.COLS.PRICE)) : 0;
                                    TAB_SETROW(row, TABLE_STLINE.COLS.PRCLIST_PRICE1, priceP_);

                                    if (rowP_ == null)
                                        ERROR_INVALID_MAT_PRICE(mref_, uref_);
                                }
                            }

                        }

                    refreshTots();
                }

                bool isRefreshing = false;



                void refreshTotsLocalDisc(DataRow pRow)//row mat
                {

                    if (pRow != null && !TAB_ROWDELETED(pRow))
                    {
                        double q = CASTASDOUBLE(TAB_GETROW(pRow, TABLE_STLINE.COLS.AMOUNT));
                        double p = CASTASDOUBLE(TAB_GETROW(pRow, TABLE_STLINE.COLS.PRICE));
                        double total = q * p;
                        double totalDisc = 0;

                        var arr_ = TABLE_STLINE.TOOLS.getSubLines(pRow, TABLE_STLINE.LINETYPE.discount);

                        foreach (var rowDisc_ in arr_)
                            if (TABLE_STLINE.TOOLS.isLocalDisc(rowDisc_))
                            {
                                var discPerc_ = Math.Min(100, CASTASDOUBLE(TAB_GETROW(rowDisc_, TABLE_STLINE.COLS.DISCPER)));
                                var discTot_ = (total - totalDisc) * discPerc_ / 100;
                                totalDisc = totalDisc + discTot_;

                                TAB_SETROW(rowDisc_, TABLE_STLINE.COLS.DISCPER, discPerc_);
                                TAB_SETROW(rowDisc_, TABLE_STLINE.COLS.TOTAL, discTot_);
                            }


                        TAB_SETROW(pRow, TABLE_STLINE.COLS.DISTDISC, totalDisc);

                    }


                }

                void _refreshTotsCalc()
                {



                    List<DataRow> listDelete = new List<DataRow>();
                    //sum
                    registers.totalGross = 0;
                    registers.totalNet = 0;

                    registers.totalDiscountLocal = 0;
                    registers.totalDiscount = 0;

                    registers.discountPerc = 0;

                    registers.totalVAT = 0;

                    registers.profitByPurchPrice = 0;
                    registers.totalByPurchPrice = 0;


                    registers.totalQuantity = 0.0;
                    registers.totalQuantityOnhand = 0.0;
                    registers.totalQuantityOnhandMath = 0.0;
                    registers.totalOnhand = 0.0;
                    registers.totalOnhandMath = 0.0;
                    registers.totalOnhandDiff = 0.0;
                    registers.totalOnhandDiffTot = 0.0;



                    //calc tots
                    double totalGross = 0.0;
                    double totalDiscountLocal = 0.0;
                    double totalByPurchPrice = 0.0;
                    double totalVAT = 0.0;

                    double totalQuantity = 0.0;
                    double totalQuantityOnhand = 0.0;
                    double totalQuantityOnhandMath = 0.0;
                    double totalOnhand = 0.0;
                    double totalOnhandMath = 0.0;
                    double totalOnhandDiff = 0.0;
                    double totalOnhandDiffTot = 0.0;




                    foreach (DataRow row_ in tableData_.Rows)
                    {
                        if (TABLE_STLINE.TOOLS.isLocalMat(row_))
                        {
                            refreshTotsLocalDisc(row_);

                            double _quant = CASTASDOUBLE(TAB_GETROW(row_, TABLE_STLINE.COLS.AMOUNT));
                            totalQuantity += _quant;

                            double _price = CASTASDOUBLE(TAB_GETROW(row_, TABLE_STLINE.COLS.PRICE));
                            double _disc = CASTASDOUBLE(TAB_GETROW(row_, TABLE_STLINE.COLS.DISTDISC));
                            double _VAT = CASTASDOUBLE(TAB_GETROW(row_, TABLE_STLINE.COLS.VAT));

                            double total = _quant * _price;
                            double totalNet = _quant * _price - _disc;
                            double totalVATLine = totalNet * _VAT / 100;


                            TAB_SETROW(row_, TABLE_STLINE.COLS.TOTAL, total);
                            TAB_SETROW(row_, TABLE_STLINE.COLS.LINENET, totalNet);


                            if (PRM.USE_ONHAND)
                            {
                                double _onhand = CASTASDOUBLE(TAB_GETROW(row_, TABLE_STLINE.COLS.DUMMY_ONHAND));
                                totalQuantityOnhand += (_onhand < 0 ? 0 : _onhand);
                                totalQuantityOnhandMath += _onhand;

                                if (PRM.USE_ONHAND_TOT)
                                {
                                    var v = (_onhand < 0 ? 0 : _onhand) * _price;
                                    var vm = _onhand * _price;

                                    totalOnhand += v;
                                    totalOnhandMath += vm;

                                    TAB_SETROW(row_, TABLE_STLINE.COLS.DUMMY_ONHAND_TOT, vm);

                                }
                                if (PRM.USE_ONHAND_TOT && PRM.USE_ONHAND_DIFF)
                                {

                                    var v1 = -_quant + _onhand;
                                    var v2 = -total + _onhand * _price;

                                    totalOnhandDiff += v1;
                                    totalOnhandDiffTot += v2;

                                    TAB_SETROW(row_, TABLE_STLINE.COLS.DUMMY_ONHAND_DIFF, v1);
                                    TAB_SETROW(row_, TABLE_STLINE.COLS.DUMMY_ONHAND_DIFFTOT, v2);
                                }

                            }

                            totalGross = totalGross + total;
                            totalDiscountLocal = totalDiscountLocal + _disc;
                            totalVAT += totalVATLine;

                            //
                            if (PRM.PRICE_DIFF_BY_PRCH)
                            {
                                double pl1 = CASTASDOUBLE(TAB_GETROW(row_, TABLE_STLINE.COLS.PRCLIST_PRICE1));

                                totalByPurchPrice = totalByPurchPrice + (_quant * (pl1));

                            }
                        }
                        else
                            if (TABLE_STLINE.TOOLS.isLocalDisc(row_))
                            {
                                //calc in refreshTotsLocalDisc

                            }
                            else
                                if (TABLE_STLINE.TOOLS.isLocalPromo(row_))
                                {


                                }





                    }

                    registers.totalGross = totalGross;
                    registers.totalDiscountLocal = totalDiscountLocal;
                    registers.totalByPurchPrice = totalByPurchPrice;

                    registers.totalQuantity = totalQuantity;
                    registers.totalQuantityOnhand = totalQuantityOnhand;
                    registers.totalQuantityOnhandMath = totalQuantityOnhandMath;
                    registers.totalOnhand = totalOnhand;
                    registers.totalOnhandMath = totalOnhandMath;

                    registers.totalOnhandDiff = totalOnhandDiff;
                    registers.totalOnhandDiffTot = totalOnhandDiffTot;

                    registers.totalVAT = totalVAT;


                    if (
                        PRM.TERM_TYPE == TERMTYPE.count ||
                        PRM.TERM_TYPE == TERMTYPE.barcode ||
                        PRM.TERM_TYPE == TERMTYPE.production ||
                        PRM.TERM_TYPE == TERMTYPE.pricing
                        )
                    {
                        return;

                    }

                    {

                        //
                        bool hasManualDisc = (!ISNUMZERO(registers.discountPercManual) || !ISNUMZERO(registers.discountAmountManual));

                        if (hasManualDisc)
                        {
                            if (!ISNUMZERO(registers.discountPercManual))
                            {
                                registers.discountPerc = registers.discountPercManual;
                            }
                            else
                                if (!ISNUMZERO(registers.discountAmountManual))
                                {
                                    var tot = registers.totalGross - registers.totalDiscountLocal;
                                    //
                                    var discAmount = MIN(tot, registers.discountAmountManual);
                                    registers.discountPerc = DIV(discAmount, tot) * 100;
                                }

                        }
                        else
                            registers.discountPerc = registers.clcard.getClientDisc();


                        registers.totalDiscount = (registers.discountPerc / 100) * (registers.totalGross - registers.totalDiscountLocal);

                        registers.totalNet = (
                            registers.totalGross - (registers.totalDiscount + registers.totalDiscountLocal)
                            ) + registers.totalVAT;

                        if (PRM.PRICE_DIFF_BY_PRCH)
                            registers.profitByPurchPrice = registers.totalNet - registers.totalByPurchPrice;
                    }
                }
                void refreshTots()
                {
                    if (isRefreshing)
                        return;

                    if (tableData_ == null)
                        return;
                    try
                    {
                        isRefreshing = true;

                        _refreshTotsCalc();

                        double change_ = Math.Max(registers.userToCashAmount - registers.totalNet, 0);

                        setInfo(TABLE_INFO.CODES.TOTALNET, FORMAT(registers.totalNet, PRM.numberFormatGen));

                        //
                        //bool hasManualDiscAmount = !ISNUMZERO(registers.discountAmountManual);
                        //bool hasLocalDisc = !ISNUMZERO(registers.totalDiscountLocal);

                        //if (!ISNUMZERO(registers.discountPerc) || hasManualDiscAmount || !ISNUMZERO(registers.totalDiscountLocal))
                        //{
                        //    var d1 = registers.totalDiscount;//  (hasManualDiscAmount ? registers.discountAmountManual : registers.totalDiscount)
                        //    var d2 = registers.totalDiscountLocal;

                        //    setInfo(TABLE_INFO.CODES.DISCOUNT,
                        //        string.Format("{0:0.##}%{1:0.00}" + (hasLocalDisc ? "+{2:0.00}={3:0.00}" : ""),
                        //        registers.discountPerc,
                        //        d1,
                        //        d2,
                        //        d1 + d2));
                        //}


                        if (!ISNUMZERO(registers.discountPerc) || !ISNUMZERO(registers.totalDiscountLocal))
                        {
                            setInfo(TABLE_INFO.CODES.DISCOUNT,
                                string.Format("{0:0.00}",
                                registers.totalDiscount + registers.totalDiscountLocal));
                        }

                        else
                            setInfo(TABLE_INFO.CODES.DISCOUNT, "");

                        setInfo(TABLE_INFO.CODES.PAYMENT, FORMAT(registers.userToCashAmount, PRM.numberFormatGen));
                        setInfo(TABLE_INFO.CODES.CHANGE, FORMAT(change_, PRM.numberFormatGen));

                        setInfo(TABLE_INFO.CODES.VAT, FORMAT(registers.totalVAT, PRM.numberFormatGen));

                        if (dataInputForm_ != null)
                        {
                            //  dataInputForm_.Refresh();
                            dataInputForm_.activateMainGrid();
                        }

                        //  setDataGridCurrRecPos(int.MaxValue);
                        ///////

                        refreshMsg1(); //info doc
                        refreshMsg2(); //info numerik
                        refreshMsg3(); //info cl
                        refreshMsg4(); //info cl fin

                    }
                    finally
                    {
                        isRefreshing = false;
                    }
                }

                void refreshMsg1()
                {
                    List<string> list_ = new List<string>();


                    {
                        var v_ = registers.cashDesc;
                        if (!string.IsNullOrEmpty(v_))
                        {
                            list_.Add(_LANG.L.PAYMENT_TYPE + ": " + v_);
                        }
                    }


                    {
                        string v_ = registers.filterGroup;

                        if (v_ != null)
                            list_.Add("F.Qrup: " + v_);
                    }
                    {
                        string v_ = registers.filterSpeCode;

                        if (v_ != null)
                            list_.Add("F.H.Kod: " + v_);
                    }
                    {
                        string v_ = registers.filterSpeCode2;

                        if (v_ != null)
                            list_.Add("F.H.Kod 2: " + v_);
                    }



                    {

                        string v_ = registers.trackno;

                        if (!string.IsNullOrEmpty(v_))
                            list_.Add("Id: " + v_);
                    }


                    {

                        string v_ = registers.promoCode;

                        if (!string.IsNullOrEmpty(v_))
                            list_.Add("Promo: " + v_);
                    }


                    {//  
                        string v_ = registers.speCode;

                        if (v_ != "")
                            list_.Add(_LANG.L.SPECODE + ": " + v_);
                    }
                    {//  
                        string v_ = registers.docCode;

                        if (v_ != "")
                            list_.Add("Kod: " + v_);
                    }


                    {
                        // if (PRM.TERM_TYPE == TERMTYPE.magazin)
                        if (_TRCODE == 3)
                            list_.Add(_LANG.L.MSG_INFO_RETURN.ToUpper());
                    }

                    {
                        // if (PRM.TERM_TYPE == TERMTYPE.magazin)
                        if (_CANCELLED)
                            list_.Add(_LANG.L.MSG_INFO_ORDER.ToUpper());
                    }


                    {//warehouse 
                        string desc_ = registers.warehouseDesc;

                        if (desc_ != "")
                            list_.Add(_LANG.L.WH + ": " + desc_);
                    }
                    {//sales man
                        string slsManInfo_ = registers.slsManCode;
                        if (registers.slsManDesc != "")
                            slsManInfo_ = slsManInfo_ + (slsManInfo_ != "" ? "/" : "") + registers.slsManDesc;

                        if (slsManInfo_ != "")
                            list_.Add((PRM.TERM_TYPE == TERMTYPE.restoran ? _LANG.L.TABLE : _LANG.L.SALER) + ": " + slsManInfo_);
                    }


                    setMsgInfo1(string.Join(", ", list_.ToArray()));
                }


                void refreshMsg2()
                {
                    List<string> list_ = new List<string>();
                    {
                        if (!ISNUMZERO(registers.totalQuantity))
                        {
                            list_.Add(string.Format(_LANG.L.QUANTITY + ":{0}", Math.Round(registers.totalQuantity, 2)));
                        }

                    }






                    if (!ISNUMZERO(registers.discountPerc) || !ISNUMZERO(registers.totalDiscountLocal))
                    {
                        var d1 = registers.totalDiscount;
                        var d2 = registers.totalDiscountLocal;
                        var p = registers.discountPerc;

                        if (!ISNUMZERO(d1))
                        {
                            list_.Add(string.Format("" + _LANG.L.SUBDISCOUNT + ":{0:0.00} " + _LANG.L.CURRENCY + "" + (ISNUMZERO(p) ? "" : " {1:0.##} %"), d1, p));
                        }

                        if (!ISNUMZERO(d2))
                        {
                            list_.Add(string.Format("" + _LANG.L.LINEDISC + ":{0:0.00} " + _LANG.L.CURRENCY + "", d2));
                        }

                        if (!ISNUMZERO(d1) && !ISNUMZERO(d2))
                        {
                            list_.Add(string.Format("" + _LANG.L.DISCTOT + ":{0:0.00} " + _LANG.L.CURRENCY + "", d1 + d2));
                        }


                    }

                    if (!ISNUMZERO(registers.bonus))
                    {
                        var b = registers.bonus;


                        if (!ISNUMZERO(b))
                        {
                            list_.Add(string.Format(_LANG.L.BONUS + ": {0:0.00} " + _LANG.L.CURRENCY + "", b));
                        }




                    }
                    if (!ISNUMZERO(registers.useBonus))
                    {
                        var b = registers.useBonus;


                        if (!ISNUMZERO(b))
                        {
                            list_.Add(string.Format(_LANG.L.BONUSMINUS + ": {0:0.00} " + _LANG.L.CURRENCY + "", b));
                        }


                    }



                    if (PRM.TERM_TYPE == TERMTYPE.count)
                    {
                        // if (!ISNUMZERO(registers.totalQuantityOnhand))
                        {
                            list_.Add(string.Format("" + _LANG.L.TOTAL + ":{0}" + _LANG.L.CURRENCY + "", Math.Round(registers.totalGross, 2)));
                        }
                    }


                    if (PRM.USE_ONHAND)
                    {
                        if (PRM.TERM_TYPE == TERMTYPE.count)
                        {
                            // if (!ISNUMZERO(registers.totalQuantityOnhand))
                            {
                                list_.Add(string.Format("" + _LANG.L.MATHREM + ":{0}", Math.Round(registers.totalQuantityOnhandMath, 2)));
                            }
                        }

                    }

                    if (PRM.USE_ONHAND_TOT)
                    {
                        if (PRM.TERM_TYPE == TERMTYPE.count)
                        {
                            //if (!ISNUMZERO(registers.totalOnhand))
                            {
                                list_.Add(string.Format("" + _LANG.L.MATHREMTOT + ":{0}" + _LANG.L.CURRENCY, Math.Round(registers.totalOnhandMath, 2)));
                            }
                        }

                    }


                    if (PRM.USE_ONHAND)
                    {
                        if (PRM.TERM_TYPE == TERMTYPE.count)
                        {
                            // if (!ISNUMZERO(registers.totalQuantityOnhand))
                            {
                                list_.Add(string.Format(_LANG.L.REM + ":{0}", Math.Round(registers.totalQuantityOnhand, 2)));
                            }
                        }

                    }

                    if (PRM.USE_ONHAND_TOT)
                    {
                        if (PRM.TERM_TYPE == TERMTYPE.count)
                        {
                            //if (!ISNUMZERO(registers.totalOnhand))
                            {
                                list_.Add(string.Format("" + _LANG.L.REMTOT + ".:{0}" + _LANG.L.CURRENCY + "", Math.Round(registers.totalOnhand, 2)));
                            }
                        }

                    }

                    if (PRM.USE_ONHAND_DIFF)
                    {
                        if (PRM.TERM_TYPE == TERMTYPE.count)
                        {
                            //  if (!ISNUMZERO(registers.totalOnhandDiff) || !ISNUMZERO(registers.totalOnhandDiffTot))
                            {
                                list_.Add(string.Format("" + _LANG.L.REMDIFF + ":{0}", Math.Round(registers.totalOnhandDiff, 2)));
                                list_.Add(string.Format("" + _LANG.L.REMTOTDIF + ":{0}" + _LANG.L.CURRENCY + "", Math.Round(registers.totalOnhandDiffTot, 2)));
                            }
                        }
                    }
                    {
                        if (!ISNUMZERO(registers.profitByPurchPrice))
                        {
                            list_.Add(string.Format("" + _LANG.L.PROFIT + ":{0}" + _LANG.L.CURRENCY + "", Math.Round(registers.profitByPurchPrice, 2)));
                        }

                    }
                    {
                        if (registers.totalByPurchPrice > 0.01)
                        {
                            list_.Add(string.Format("" + _LANG.L.PURCHTOT + ":{0}" + _LANG.L.CURRENCY + "", Math.Round(registers.totalByPurchPrice, 2)));
                        }

                    }
                    {//sales man
                        registers.monthPayment = 0;
                        if (registers.month > 0)
                        {
                            double diff_ = registers.totalNet - registers.userToCashAmount;

                            if (diff_ > 0.05)
                            {
                                registers.monthPayment = Math.Round(diff_ / registers.month, 2);

                                list_.Add(string.Format("" + _LANG.L.CREDITSUM + " {0} " + _LANG.L.CURRENCY + ", " + _LANG.L.LIFE + " {1} " + _LANG.L.MONTH + ", " + _LANG.L.MOTHLYPAY + " {2} " + _LANG.L.CURRENCY + "", diff_, registers.month, registers.monthPayment));

                            }
                        }
                    }





                    setMsgInfo2(string.Join(", ", list_.ToArray()));
                }

                void refreshMsg3()
                {
                    List<string> list_ = new List<string>();





                    {
                        string clDesc_ = registers.clcard.getClientDesc();
                        string clCodeSearch_ = registers.clcard.getClientCodeSearch();

                        if (clDesc_ != "")
                        {

                            string v = "";
                            v = v + " " + _LANG.L.WHO + ": " + clDesc_; // +(clCodeSearch_ == "" ? "" : " " + clCodeSearch_);
                            list_.Add(v);

                        }
                    }







                    setMsgInfo3(string.Join(", ", list_.ToArray()));
                }

                void refreshMsg4()
                {
                    List<string> list_ = new List<string>();



                    {
                        string clDesc_ = registers.clcard.getClientDesc();
                        string clBal_ = registers.clcard.getClientBalanceDesc();
                        string clPay_ = registers.clcard.getClientLastPaymentDesc();


                        if (clDesc_ != "")
                        {

                            string v = "";

                            if (PRM.MAKE_PAYMENT)
                            {
                                v = v + "" + _LANG.L.DEBIT + ": " + clBal_ + "";
                                if (clPay_ != "")
                                    v = v + " / " + _LANG.L.LASTPAYMENT + ": " + clPay_ + " " + _LANG.L.CURRENCY;
                            }

                            list_.Add(v);

                        }
                    }

                    {

                        string clPayPlan_ = registers.clcard.getClientPayplanDesc();

                        if (clPayPlan_ != "")
                        {

                            string v = "";
                            v = v + (list_.Count == 0 ? "" : "/") + " " + _LANG.L.PRICEGRP + ": " + clPayPlan_;

                            list_.Add(v);

                        }
                    }
                    setMsgInfo4(string.Join(", ", list_.ToArray()));
                }


                void setInfo(string pInfo, string pVal)
                {
                    if (
                        PRM.TERM_TYPE == TERMTYPE.count ||
                        PRM.TERM_TYPE == TERMTYPE.barcode ||
                         PRM.TERM_TYPE == TERMTYPE.production ||
                        PRM.TERM_TYPE == TERMTYPE.pricing
                        )
                        return;

                    DataRow row_ = TAB_SEARCH(tableInfo_, TABLE_INFO.COLS.CODE, pInfo);
                    if (row_ != null)
                    {
                        row_[TABLE_INFO.COLS.VALUE] = pVal;
                    }
                    // else
                    //     setMsgErr("Undefined record code [" + pInfo + "," + pVal + "]");
                }

                void setMsgErr(string pVal)
                {
                    setMsg(TABLE_MSG.CODES.MSG1, pVal);
                }
                void setMsgInfo1(string pVal)
                {
                    setMsg(TABLE_MSG.CODES.MSG2, pVal);
                }
                void setMsgInfo2(string pVal)
                {
                    setMsg(TABLE_MSG.CODES.MSG3, pVal);
                }
                void setMsgInfo3(string pVal)
                {
                    setMsg(TABLE_MSG.CODES.MSG4, pVal);
                }
                void setMsgInfo4(string pVal)
                {
                    setMsg(TABLE_MSG.CODES.MSG5, pVal);
                }
                void setMsg(string pMsgIndx, string pVal)
                {
                    if (tableMsg_ == null)
                        return;

                    DataRow row_ = TAB_SEARCH(tableMsg_, TABLE_MSG.COLS.CODE, pMsgIndx);
                    if (row_ != null)
                    {
                        row_[TABLE_INFO.COLS.VALUE] = pVal;
                    }
                    else
                        setMsgErr("Undefined record code [" + pMsgIndx + "," + pVal + "]");
                }

                DataRow SEARCH_MAT_LINE_BARCODE(string pBarcode)
                {
                    if (PRM.SEARCH_MAT_BY_CODE)
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
                        LG_$FIRM$_ITEMS I with(nolock) WHERE CODE = @P1",
@"
                        SELECT 
                        LOGICALREF,
                        LOGICALREF ITEMREF,
                        (select LOGICALREF from LG_$FIRM$_UNITSETL where UNITSETREF = I.UNITSETREF AND MAINUNIT = 1 LIMIT 1) UNITLINEREF,
                        CODE BARCODE 
                        FROM 
                        LG_$FIRM$_ITEMS I WHERE CODE = @P1
LIMIT 1
"),

                         new object[] { pBarcode }));



                    }

                    return TAB_GETLASTROW(SQL(
                        MY_CHOOSE_SQL(
                        "SELECT TOP(1) LOGICALREF,ITEMREF,UNITLINEREF,BARCODE FROM LG_$FIRM$_UNITBARCODE with(nolock) WHERE BARCODE = @P1",
                         "SELECT LOGICALREF,ITEMREF,UNITLINEREF,BARCODE FROM LG_$FIRM$_UNITBARCODE WHERE BARCODE = @P1 LIMIT 1"),

                        new object[] { pBarcode }));
                }

                object SEARCH_MAT_VAL(object pMatLRef, string pCol)
                {
                    object resMat_ = (SQLSCALAR(string.Format(
                        MY_CHOOSE_SQL(
                    @"
                select top(1) {0} from LG_$FIRM$_ITEMS with(nolock) where LOGICALREF = @P1
                ", @"
                select {0} from LG_$FIRM$_ITEMS where LOGICALREF = @P1 LIMIT 1
                "), pCol), new object[] { pMatLRef }));
                    return resMat_;
                }
                DataRow SEARCH_MAT_LINE(object pMatLRef)
                {
                    DataRow resMat_ = TAB_GETLASTROW(SQL(
                     MY_CHOOSE_SQL(@"
                select top(1) LOGICALREF,CODE,NAME,UNITSETREF from LG_$FIRM$_ITEMS with(nolock) where LOGICALREF = @P1
                ",
                  @"
                select LOGICALREF,CODE,NAME,UNITSETREF from LG_$FIRM$_ITEMS where LOGICALREF = @P1 LIMIT 1
                "),
                 new object[] { pMatLRef }));
                    return resMat_;
                }
                DataRow SEARCH_MAT_PRICE_SLS(object pMatLRef, object pUnitLRef, object pPayPlan)
                {
                    return _SEARCH_MAT_PRICE(pMatLRef, pUnitLRef, 2, pPayPlan);
                }
                DataRow SEARCH_MAT_PRICE_PRCH(object pMatLRef, object pUnitLRef, object pPayPlan)
                {
                    return _SEARCH_MAT_PRICE(pMatLRef, pUnitLRef, 1, pPayPlan);
                }
                DataRow _SEARCH_MAT_PRICE(object pMatLRef, object pUnitLRef, int pPriceType, object pPayPlan)
                {
                    if (pPriceType <= 0)
                        pPriceType = 2;

                    object payPlan1_ = 0;
                    object payPlan2_ = ISNULL(pPayPlan, 0);

                    DataRow resPrice_ = TAB_GETLASTROW(SQL(
                        MY_CHOOSE_SQL(
                        @"
                SELECT TOP(1) P.PRICE --(case when P.INCVAT = 1 then P.PRICE else P.PRICE * (1+(I.VAT/100)) end) PRICE 
                FROM 
                LG_$FIRM$_PRCLIST P with(nolock) 
                inner join 
                LG_$FIRM$_ITEMS I with(nolock) ON P.CARDREF = I.LOGICALREF 
                WHERE 
                P.CARDREF = @P1 AND P.PTYPE = @P3 AND  P.UOMREF = @P2  AND P.PAYPLANREF IN (@P4,@P5)  
                ORDER BY 
                P.ENDDATE DESC,P.PAYPLANREF DESC
                ", @"
                SELECT P.PRICE --(case when P.INCVAT = 1 then P.PRICE else P.PRICE * (1+(I.VAT/100)) end) PRICE 
                FROM 
                LG_$FIRM$_PRCLIST P 
                inner join 
                LG_$FIRM$_ITEMS I ON P.CARDREF = I.LOGICALREF 
                WHERE 
                P.CARDREF = @P1 AND P.PTYPE = @P3 AND  P.UOMREF = @P2  AND P.PAYPLANREF IN (@P4,@P5)  
                ORDER BY 
                P.ENDDATE DESC,P.PAYPLANREF DESC
LIMIT 1
                "), new object[] { pMatLRef, pUnitLRef, pPriceType, payPlan1_, payPlan2_ }));

                    return resPrice_;
                }

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
                    setMsgErr(_LANG.L.MSG_ERROR_MATERIAL + " [" + pMatLRef + "]");

                }
                void ERROR_INVALID_MAT_UNIT(object pMatLRef)
                {
                    setMsgErr(_LANG.L.MSG_ERROR_MATERIAL_UNIT + " [" + pMatLRef + "]");

                }
                void ERROR_INVALID_MAT_PRICE(object pMatLRef, object pUnitLRef)
                {

                    string desc_ = ISNULL(SEARCH_MAT_VAL(pMatLRef,
                        MY_CHOOSE_SQL("CODE+'/'+NAME", "CODE||'/'||NAME")


                        ), "").ToString();

                    setMsgErr(_LANG.L.MSG_ERROR_MATERIAL_PRICE + " [" + desc_ + "/RecId=" + pMatLRef + "/UnitId=" + pUnitLRef + "]");

                }
                void ERROR_INVALID_MAT_BARCODE(object pBarcode, bool pAskFromList)
                {
                    setMsgErr(_LANG.L.MSG_ERROR_MATERIAL_BARCODE + " [" + pBarcode + "]");

                    if (pAskFromList)
                        processMatFromReference();

                }

                void ERROR_INVALID_MATH(object pCmd)
                {
                    setMsgErr("Undefined math cmd [" + pCmd + "]");

                }


                public DataRow addRecord(object pMatLref, string pMatCode, string pMatDesc, object pUnitLref, string pUnitCode, object pBarcodeLref, double pPrice, double pAmount, bool pNewLineCreate)
                {

                    if (pMatCode == "" || pMatDesc == "" || pUnitCode == "")
                    {

                    }


                    DataRow row_ = null;

                    if (!pNewLineCreate)
                    {
                        row_ = TABLE_STLINE.TOOLS.searchSameDataRecord(tableData_, pMatLref, pUnitLref, pPrice, "*");


                        if (PRM.TERM_TYPE == TERMTYPE.pricing)
                        {
                            if (row_ != null)
                                return null;

                        }
                        if (row_ != null)
                        {
                            if (!isReadOnly(row_))
                            {
                                pAmount = pAmount + _PLUGIN.CASTASDOUBLE(TAB_GETROW(row_, TABLE_STLINE.COLS.AMOUNT));
                            }
                            else
                                row_ = null;
                        }
                    }

                    if (row_ == null)
                    {


                        int indx = 0;
                        foreach (DataRow row in tableData_.Rows)
                        {
                            short glob_ = CASTASSHORT(TAB_GETROW(row, TABLE_STLINE.COLS.GLOBTRANS));
                            if (glob_ == 1)
                                break;

                            ++indx;
                        }


                        row_ = TABLE_STLINE.TOOLS.addTrans(tableData_, indx);

                    }

                    TAB_SETROW(row_, TABLE_STLINE.COLS.STOCKREF, pMatLref);
                    TAB_SETROW(row_, TABLE_STLINE.COLS.UOMREF, pUnitLref);
                    TAB_SETROW(row_, TABLE_STLINE.COLS.UNITBARCODE_LOGICALREF, _PLUGIN.ISNULL(pBarcodeLref, 0));
                    TAB_SETROW(row_, TABLE_STLINE.COLS.ITEMS_CODE, pMatCode);
                    TAB_SETROW(row_, TABLE_STLINE.COLS.ITEMS_NAME, pMatDesc);
                    TAB_SETROW(row_, TABLE_STLINE.COLS.UNITSETL_CODE, pUnitCode);
                    TAB_SETROW(row_, TABLE_STLINE.COLS.PRICE, pPrice);
                    TAB_SETROW(row_, TABLE_STLINE.COLS.AMOUNT, pAmount);

                    REFRESH_ONHAND(row_);

                    if (PRM.USE_VAT && !ISNUMZERO(PRM.DEF_VAT))
                        TAB_SETROW(row_, TABLE_STLINE.COLS.VAT, PRM.DEF_VAT);

                    if (PRM.TERM_TYPE == TERMTYPE.pricing)
                    {
                        execute();
                        calcPricingValue(row_);
                    }

                    if (PRM.TERM_TYPE == TERMTYPE.barcode || PRM.TERM_TYPE == TERMTYPE.production)
                    {
                        var dbdesc_ = ISNULL(SQLSCALAR(MY_CHOOSE_SQL(
                            "SELECT TOP(1) SPECODE+'/'+SPECODE2 FROM LG_$FIRM$_ITEMS WITH(NOLOCK) WHERE LOGICALREF = @P1",
                            "SELECT SPECODE||'/'||SPECODE2 FROM LG_$FIRM$_ITEMS WITH(NOLOCK) WHERE LOGICALREF = @P1 LIMIT 1"),
                            new object[] { pMatLref }), "");
                        TAB_SETROW(row_, TABLE_STLINE.COLS.LINEEXP, dbdesc_);
                    }

                    //auto promo refresh on new rec
                    if (!string.IsNullOrEmpty(registers.promoCode))
                        applyCampagin(false, registers.promoCode);

                    //   backUpDoc();

                    return row_;
                }
                public void REFRESH_ONHAND()
                {
                    REFRESH_ONHAND(tableData_);
                }
                public void REFRESH_ONHAND(DataTable pTab)
                {
                    foreach (DataRow row in pTab.Rows)
                        if (!TAB_ROWDELETED(row))
                            REFRESH_ONHAND(row);
                }
                public void REFRESH_ONHAND(DataRow pRow)
                {
                    if (TAB_ROWDELETED(pRow))
                        return;

                    if (PRM.USE_ONHAND || PRM.USE_ONHAND_MAIN || PRM.USE_ONHAND_ALL)
                    {

                        if (TABLE_STLINE.TOOLS.isLocalMat(pRow) || TABLE_STLINE.TOOLS.isPromo(pRow))
                        {
                            object mat_ = TAB_GETROW(pRow, TABLE_STLINE.COLS.STOCKREF);
                            var res_ = GET_ONHAND(mat_, registers.warehouse, registers.getDocDate());

                            if (PRM.USE_ONHAND)
                            {
                                TAB_SETROW(pRow, TABLE_STLINE.COLS.DUMMY_ONHAND, res_[0]);
                            }
                            if (PRM.USE_ONHAND_MAIN)
                            {
                                TAB_SETROW(pRow, TABLE_STLINE.COLS.DUMMY_ONHANDMAIN, res_[1]);
                            }
                            if (PRM.USE_ONHAND_ALL)
                            {
                                TAB_SETROW(pRow, TABLE_STLINE.COLS.DUMMY_ONHANDALL, res_[2]);
                            }
                        }
                    }
                }

                public double[] GET_ONHAND(object pMRef, short pWh, DateTime pDate)
                {
                    if (pDate.Year == 1900)
                        pDate = DateTime.Now;

                    pDate = pDate.Date;

                    var sql_ = MY_CHOOSE_SQL(@"
declare @v float,@vm float,@va float
SELECT @v = SUM(ONHAND) FROM LG_$FIRM$_$PERIOD$_STINVTOT WITH(NOLOCK)  WHERE STOCKREF = @P1 AND INVENNO = @P2 AND DATE_ <= @P3
SELECT @vm = SUM(ONHAND) FROM LG_$FIRM$_$PERIOD$_STINVTOT WITH(NOLOCK)  WHERE STOCKREF = @P1 AND INVENNO = 0 AND DATE_ <= @P3
SELECT @va = SUM(ONHAND) FROM LG_$FIRM$_$PERIOD$_STINVTOT WITH(NOLOCK)  WHERE STOCKREF = @P1 AND INVENNO = -1 AND DATE_ <= @P3

select isnull(@v,0.0) ONHAND, isnull(@vm,0.0) ONHANDMAIN, isnull(@va,0.0) ONHANDALL
", @"
 
SELECT
COALESCE((SELECT SUM(ONHAND) FROM LG_$FIRM$_$PERIOD$_STINVTOT WHERE STOCKREF = @P1 AND INVENNO = @P2 AND DATE_ <= @P3),0.0) ONHAND,
COALESCE((SELECT SUM(ONHAND) FROM LG_$FIRM$_$PERIOD$_STINVTOT WHERE STOCKREF = @P1 AND INVENNO = 0 AND DATE_ <= @P3),0.0) ONHANDMAIN,
COALESCE((SELECT SUM(ONHAND) FROM LG_$FIRM$_$PERIOD$_STINVTOT WHERE STOCKREF = @P1 AND INVENNO = -1 AND DATE_ <= @P3),0.0) ONHANDALL

 
");
                    var res_ = TAB_GETLASTROW((SQL(sql_, new object[] { pMRef, pWh, pDate })));

                    return new double[] { CASTASDOUBLE(res_[0]), CASTASDOUBLE(res_[1]), CASTASDOUBLE(res_[2]) };
                }

                public double askNumber(string pMsg, double pDef, int pDecimals)
                {
                    //  
                    DataRow[] rows_ = PLUGIN.REF("ref.gen.double decimals::" + FORMAT(pDecimals) + " calc::1 desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
                    if (rows_ != null && rows_.Length > 0)
                    {
                        return _PLUGIN.CASTASDOUBLE(ISNULL(rows_[0]["VALUE"], 0.0));
                    }
                    return -1;
                }
                public double askNumber(string pMsg, double pDef)
                {

                    return askNumber(pMsg, pDef, 2);
                }

                public string askString(string pMsg, string pDef)
                {

                    DataRow[] rows_ = PLUGIN.REF("ref.gen.string desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
                    if (rows_ != null && rows_.Length > 0)
                    {
                        return _PLUGIN.CASTASSTRING(ISNULL(rows_[0]["VALUE"], ""));
                    }
                    return null;

                }
                public void logDbAmountChanged(string pName, double pOld, double pNew, double pPrice)
                {
                    if (!PRM.LOG_AMOUNT_DEC)
                        return;

                    if (ISNUMEQUAL(pOld, pNew))
                        return;

                    string msg_ = string.Format("{2:0.##}/{3:0.##}/{0}", LEFT(pName, 15) + '~', registers.getDocCode(PLUGIN), (pOld - pNew), (pOld - pNew) * pPrice);
                    logDb("AMT", msg_);

                }
                public void logDbAmountChanged(DataRow pRec, double pOld, double pNew)
                {

                    if (!PRM.LOG_AMOUNT_DEC)
                        return;

                    var name_ = CASTASSTRING(TAB_GETROW(pRec, TABLE_STLINE.COLS.ITEMS_NAME));
                    var price_ = CASTASDOUBLE(TAB_GETROW(pRec, TABLE_STLINE.COLS.PRICE));

                    logDbAmountChanged(name_, pOld, pNew, price_);
                }

                public void logDb(string pCode, string pMsg)
                {
                    PLUGIN.LOGPERIOD(0, "POS/" + pCode, pMsg);
                    //TODO
                    //                    SQL(@"
                    // exec p_INSERT_LG_$FIRM$_$PERIOD$_LOGPERIOD @P1,@P2,@P3,@P4,@P5,@P6
                    // 
                    // ", new object[] { 0, PLUGIN.GETSYSPRM_USER(), DBNull.Value, 0, "POS/" + pCode, pMsg });


                }
                public bool askPassword()
                {

                    if (!PRM.USE_PASSWORD)
                        return true;

                    string p = CASTASSTRING(ISNULL(SQLSCALAR(MY_CHOOSE_SQL(
                        "select VALUE from L_FIRMPARAMS with(NOLOCK) WHERE CODE IN ('PASSWORD','POS_PASSWORD') and FIRMNR=$FIRM$",
                            "select VALUE from L_FIRMPARAMS WHERE CODE IN ('PASSWORD','POS_PASSWORD')  and FIRMNR=$FIRM$"),
                        null), ""));

                    if (p == "")
                        return true;

                    while (true)
                    {
                        DataRow[] rows_ = PLUGIN.REF("ref.gen.string password::1 desc::" + _PLUGIN.STRENCODE("" + _LANG.L.PASSWD + "") + "");
                        if (rows_ != null && rows_.Length > 0)
                        {
                            var val_ = _PLUGIN.CASTASSTRING(ISNULL(rows_[0]["VALUE"], ""));


                            if (val_ == p)
                                return true;
                            else
                                beepErr();

                        }
                        else
                            return false;
                    }

                    return false;
                }
                public double getAmount(object pMatLref, object pUnitLref, double pPrice, bool pWithReadonly)
                {
                    DataRow[] rows = TABLE_STLINE.TOOLS.searchSameDataRecords(tableData_, pMatLref, pUnitLref, pPrice, "");
                    return getAmount(rows, pWithReadonly);
                }
                double getAmount(DataRow[] pRows, bool pWithReadonly)
                {
                    double res = 0;
                    foreach (DataRow row in pRows)
                        if (!_PLUGIN.TAB_ROWDELETED(row))
                            if (pWithReadonly || !isReadOnly(row))
                                res += _PLUGIN.CASTASDOUBLE(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(row, TABLE_STLINE.COLS.AMOUNT), 0));

                    return res;
                }
                double setAmount(DataRow[] pRows, double pAmount)
                {
                    List<DataRow> listDelete = new List<DataRow>();

                    foreach (DataRow row in pRows)
                        if (!_PLUGIN.TAB_ROWDELETED(row) && !isReadOnly(row))
                        {
                            double amount = _PLUGIN.CASTASDOUBLE(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(row, TABLE_STLINE.COLS.AMOUNT), 0));

                            pAmount = pAmount - amount;

                            if (pAmount < 0)
                            {
                                amount = Math.Max(0, amount + pAmount);
                                _PLUGIN.TAB_SETROW(row, TABLE_STLINE.COLS.AMOUNT, amount);
                            }

                            if (amount <= 0)
                                TABLE_STLINE.TOOLS.deleteLine(row);



                            if (pAmount < 0)
                                pAmount = 0;
                        }


                    if (pAmount > 0)
                        return pAmount;

                    return 0;

                }
                public DataRow setAmount(object pMatLref, string pMatCode, string pMatDesc, object pUnitLref, string pUnitCode, double pPrice, double pAmount)
                {
                    DataRow[] rows = TABLE_STLINE.TOOLS.searchSameDataRecords(tableData_, pMatLref, pUnitLref, pPrice, "");
                    double rem_ = setAmount(rows, pAmount);

                    if (rem_ > 0.0001)
                    {
                        return addRecord(pMatLref, pMatCode, pMatDesc, pUnitLref, pUnitCode, null, pPrice, rem_, false);
                    }

                    return null;
                }

                void changeAmount()
                {

                    changeAmount(dataInputForm_.getCurrentRecord());
                }
                void changePrice()
                {
                    changePrice(dataInputForm_.getCurrentRecord());
                }

                public bool isReadOnly(DataRow pDataRecord, string pCol)
                {
                    if (_READONLY)
                        return true;

                    if (pDataRecord == null)
                        return true;

                    if (pDataRecord.RowState == DataRowState.Deleted || pDataRecord.RowState == DataRowState.Detached)
                        return true;

                    if (TABLE_STLINE.TOOLS.isLocalDisc(pDataRecord) && !TABLE_STLINE.TOOLS.isScripted(pDataRecord))
                    {
                        if (pCol == TABLE_STLINE.COLS.LINEEXP)
                            return false;
                        if (pCol == TABLE_STLINE.COLS.DISCPER)
                            return false;
                    }

                    if (PRM.TERM_TYPE == TERMTYPE.barcode)
                    {
                        if (pCol == TABLE_STLINE.COLS.LINEEXP)
                            return true;
                    }

                    return !TABLE_STLINE.TOOLS.isLocalMat(pDataRecord);
                }
                public bool isReadOnly(DataRow pDataRecord)
                {
                    if (_READONLY)
                        return true;

                    if (pDataRecord == null)
                        return true;

                    if (!ISEMPTYLREF(TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.LOGICALREF)))
                        return true;

                    return !TABLE_STLINE.TOOLS.isLocalMat(pDataRecord);
                }
                public void changeMatCoif1(DataRow pDataRecord, double pValue)
                {
                    if (PRM.TERM_TYPE != TERMTYPE.pricing)
                        return;

                    if (isReadOnly(pDataRecord, PRICNGVARS.FF))
                        return;

                    if (pValue < 0)
                    {
                        double coif_ = CASTASDOUBLE(TAB_GETROW(pDataRecord, PRICNGVARS.FF));

                        coif_ = askNumber("T_MATERIAL - T_RATE", coif_);

                        if (coif_ < 0)
                            return;

                        pValue = coif_;

                    }


                    TAB_SETROW(pDataRecord, PRICNGVARS.FF, pValue);

                    calcPricingValue(pDataRecord);
                }



                void changeLineDisc(DataRow pDataRecord)
                {
                    if (isReadOnly(pDataRecord, TABLE_STLINE.COLS.DISCPER))
                        return;

                    if (!TABLE_STLINE.TOOLS.isLocalMat(pDataRecord))
                        return;

                    if (!askPassword())
                        return;

                    var arr_ = TABLE_STLINE.TOOLS.getSubLines(pDataRecord, TABLE_STLINE.LINETYPE.discount);

                    double disc_ = arr_.Length > 0 ? CASTASDOUBLE((_PLUGIN.TAB_GETROW(arr_[0], TABLE_STLINE.COLS.AMOUNT))) : 0;



                    string mdesc = _PLUGIN.CASTASSTRING(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.ITEMS_NAME), ""));


                    disc_ = askNumber("T_DISCOUNT, " + mdesc, disc_);

                    if (disc_ < 0)
                        return;

                    disc_ = Math.Min(disc_, 100);

                    foreach (var r in arr_)
                        r.Delete();


                    DataRow rowDisc_ = TABLE_STLINE.TOOLS.addTransSub(pDataRecord);

                    TAB_SETROW(rowDisc_, TABLE_STLINE.COLS.LINETYPE, TABLE_STLINE.LINETYPE.discount);
                    TAB_SETROW(rowDisc_, TABLE_STLINE.COLS.DISCPER, disc_);

                }
                void changeAmount(DataRow pDataRecord)
                {

                    if (PRM.TERM_TYPE == TERMTYPE.pricing)
                        return;

                    //  if (PRM.TERM_TYPE == TERMTYPE.hotel)
                    //    return;

                    if (isReadOnly(pDataRecord))
                        return;

                    object mref = _PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.STOCKREF);
                    object uref = _PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.UOMREF);
                    double price = _PLUGIN.CASTASDOUBLE(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.PRICE), 0));
                    double quantity = _PLUGIN.CASTASDOUBLE(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.AMOUNT), 0));

                    string mcode = _PLUGIN.CASTASSTRING(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.ITEMS_CODE), ""));
                    string mdesc = _PLUGIN.CASTASSTRING(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.ITEMS_NAME), ""));
                    string ucode = _PLUGIN.CASTASSTRING(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.UNITSETL_CODE), ""));

                    if (quantity == 0)
                        quantity = 1;

                    double old_ = quantity;

                    quantity = askNumber("T_QUANTITY, " + mdesc, quantity);

                    if (quantity < 0)
                        return;

                    if (old_ > quantity)
                    {
                        if (PRM.USE_PASSWORD_FOR_AMOUNT)
                            if (!askPassword())
                                return;

                        logDbAmountChanged(pDataRecord, old_, quantity);
                    }


                    _PLUGIN.TAB_SETROW(pDataRecord, TABLE_STLINE.COLS.AMOUNT, quantity);

                    //   backUpDoc();
                }

                bool canEditPrice()
                {

                    if (PRM.TERM_TYPE == TERMTYPE.magazin)
                        if (_TRCODE == 3 && PRM.RETURN_EDIT_PRICE)
                            return true;

                    return PRM.MAT_PRICE_EDIT;
                }


                void changePrice(DataRow pDataRecord)
                {
                    if (isReadOnly(pDataRecord))
                        return;

                    if (!canEditPrice())
                        return;

                    if (pDataRecord == null)
                        return;




                    object mref = _PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.STOCKREF);
                    object uref = _PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.UOMREF);
                    double price = _PLUGIN.CASTASDOUBLE(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.PRICE), 0));
                    double quantity = _PLUGIN.CASTASDOUBLE(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.AMOUNT), 0));

                    string mcode = _PLUGIN.CASTASSTRING(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.ITEMS_CODE), ""));
                    string mdesc = _PLUGIN.CASTASSTRING(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.ITEMS_NAME), ""));
                    string ucode = _PLUGIN.CASTASSTRING(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.UNITSETL_CODE), ""));



                    price = askNumber("T_PRICE, " + mdesc, price);

                    if (price < 0)
                        return;

                    _PLUGIN.TAB_SETROW(pDataRecord, TABLE_STLINE.COLS.PRICE, price);


                }


                void changeLineVAT(DataRow pDataRecord)
                {
                    if (isReadOnly(pDataRecord))
                        return;

                    if (!PRM.EDIT_VAT)
                        return;

                    if (pDataRecord == null)
                        return;

                    double vat = _PLUGIN.CASTASDOUBLE(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.VAT), 0));

                    string mdesc = _PLUGIN.CASTASSTRING(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.ITEMS_NAME), ""));

                    vat = askNumber("T_VAT, " + mdesc, vat, 0);

                    if (vat < 0)
                        return;

                    vat = MIN(99, vat);

                    _PLUGIN.TAB_SETROW(pDataRecord, TABLE_STLINE.COLS.VAT, vat);

                }
                void changeLineSpecode(DataRow pDataRecord)
                {

                    if (PRM.TERM_TYPE != TERMTYPE.hotel)
                        return;

                    if (isReadOnly(pDataRecord, TABLE_STLINE.COLS.SPECODE))
                        return;







                    string sp = _PLUGIN.CASTASSTRING(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.SPECODE), ""));
                    sp = getSpeCode(1, 25, sp);



                    if (sp == null)
                        return;

                    _PLUGIN.TAB_SETROW(pDataRecord, TABLE_STLINE.COLS.SPECODE, sp);


                }

                void changeLineExp(DataRow pDataRecord)
                {

                    if (PRM.TERM_TYPE == TERMTYPE.pricing)
                        return;

                    if (isReadOnly(pDataRecord, TABLE_STLINE.COLS.LINEEXP))
                        return;




                    object mref = _PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.STOCKREF);
                    object uref = _PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.UOMREF);
                    double price = _PLUGIN.CASTASDOUBLE(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.PRICE), 0));
                    double quantity = _PLUGIN.CASTASDOUBLE(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.AMOUNT), 0));

                    string mcode = _PLUGIN.CASTASSTRING(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.ITEMS_CODE), ""));
                    string mdesc = _PLUGIN.CASTASSTRING(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.ITEMS_NAME), ""));
                    string ucode = _PLUGIN.CASTASSTRING(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.UNITSETL_CODE), ""));

                    string lineExp = _PLUGIN.CASTASSTRING(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(pDataRecord, TABLE_STLINE.COLS.LINEEXP), ""));


                    string inDbList_ = CASTASSTRING(SEARCH_MAT_VAL(mref, TABLE_ITEMS.COLS.TEXTF5));
                    if (inDbList_ != string.Empty)
                    {
                        lineExp = askList(inDbList_);
                    }
                    else
                    {
                        lineExp = askString("T_TEXT, " + mdesc, lineExp);
                    }

                    if (lineExp == null)
                        return;

                    _PLUGIN.TAB_SETROW(pDataRecord, TABLE_STLINE.COLS.LINEEXP, lineExp);


                }

                public void dataTableColumnChanged(string pCol, DataRow pRow)
                {

                    if (!PRM.PRICE_DIFF_BY_PRCH)
                        return;


                    if (pCol == TABLE_STLINE.COLS.STOCKREF || pCol == TABLE_STLINE.COLS.UOMREF)
                    {
                        if (pRow.Table != null)
                        {
                            object mref_ = TAB_GETROW(pRow, TABLE_STLINE.COLS.STOCKREF);
                            object uref_ = TAB_GETROW(pRow, TABLE_STLINE.COLS.UOMREF);

                            double val_ = 0;

                            if (!ISEMPTYLREF(mref_) && !ISEMPTYLREF(uref_))
                            {
                                DataRow row_ = SEARCH_MAT_PRICE_PRCH(mref_, uref_, registers.clcard.getPaymentRef());
                                if (row_ != null)
                                {
                                    val_ = CASTASDOUBLE(TAB_GETROW(row_, TABLE_PRCLIST.COLS.PRICE));
                                }
                            }

                            TAB_SETROW(pRow, TABLE_STLINE.COLS.PRCLIST_PRICE1, val_);
                        }

                    }



                }

                public void activateRecord(DataRow pRow)
                {

                    if (pRow == null)
                        return;

                    if (
                        PRM.TERM_TYPE == TERMTYPE.pricing ||
                        PRM.TERM_TYPE == TERMTYPE.barcode ||
                        PRM.TERM_TYPE == TERMTYPE.production ||
                        PRM.TERM_TYPE == TERMTYPE.count)
                    {
                        var lref_ = TAB_GETROW(pRow, TABLE_STLINE.COLS.STOCKREF);
                        var code_ = CASTASSTRING(SQLSCALAR(MY_CHOOSE_SQL(
                            "SELECT CODE FROM LG_$FIRM$_ITEMS WITH(NOLOCK) WHERE LOGICALREF = @P1",
                             "SELECT CODE FROM LG_$FIRM$_ITEMS WHERE LOGICALREF = @P1"),
                            new object[] { lref_ }));

                        PLUGIN.REFNORES("ref.mm.rec.mat", "CODE", code_);
                    }


                }
            }

            class MY_SAVECLCARD : IDisposable
            {
                Registers REGISTERS;
                _PLUGIN PLUGIN;





                public MY_SAVECLCARD(_PLUGIN pPLUGIN, Registers pREGISTERS)
                {

                    REGISTERS = pREGISTERS;
                    PLUGIN = pPLUGIN;

                    if (PRM.CLIENT_IN_DB)
                        return;


                    if (pREGISTERS.clcard.hasDbSameCl(PLUGIN))
                        return;

                    string cmd_ = "adp.fin.rec.client/1 cmd::add";

                    object[] lref_ = PLUGIN.EXEADPCMD(new string[] { cmd_ }, new DoWorkEventHandler[] { this.DONE }, true);//in global batch

                    if (lref_ != null && lref_.Length > 0)
                        REGISTERS.clcard.setClientRef(lref_[0], PLUGIN);

                }

                public void DONE(object sender, System.ComponentModel.DoWorkEventArgs e)
                {

                    e.Result = false;


                    //
                    DataSet doc_ = ((DataSet)e.Argument);

                    DataTable tabHeader_ = TAB_GETTAB(doc_, "CLCARD");


                    REGISTERS.clcard.checkValues(PLUGIN);

                    TAB_SETROW(tabHeader_, TABLE_CLCARD.COLS.CODE, REGISTERS.clcard.getValue(TABLE_CLCARD.COLS.CODE));
                    TAB_SETROW(tabHeader_, TABLE_CLCARD.COLS.DEFINITION_, REGISTERS.clcard.getValue(TABLE_CLCARD.COLS.DEFINITION_));
                    TAB_SETROW(tabHeader_, TABLE_CLCARD.COLS.FAXNR, REGISTERS.clcard.getValue(TABLE_CLCARD.COLS.FAXNR));
                    TAB_SETROW(tabHeader_, TABLE_CLCARD.COLS.ADDR1, REGISTERS.clcard.getValue(TABLE_CLCARD.COLS.ADDR1));
                    TAB_SETROW(tabHeader_, TABLE_CLCARD.COLS.ADDR2, REGISTERS.clcard.getValue(TABLE_CLCARD.COLS.ADDR2));
                    TAB_SETROW(tabHeader_, TABLE_CLCARD.COLS.EMAILADDR, REGISTERS.clcard.getValue(TABLE_CLCARD.COLS.EMAILADDR));
                    TAB_SETROW(tabHeader_, TABLE_CLCARD.COLS.TELNRS1, REGISTERS.clcard.getValue(TABLE_CLCARD.COLS.TELNRS1));
                    TAB_SETROW(tabHeader_, TABLE_CLCARD.COLS.TELNRS2, REGISTERS.clcard.getValue(TABLE_CLCARD.COLS.TELNRS2));
                    //
                    TAB_SETROW(tabHeader_, TABLE_CLCARD.COLS.DATEF1, REGISTERS.clcard.getValue(TABLE_CLCARD.COLS.DATEF1));
                    TAB_SETROW(tabHeader_, TABLE_CLCARD.COLS.DATEF2, REGISTERS.clcard.getValue(TABLE_CLCARD.COLS.DATEF2));
                    //
                    if (REGISTERS.monthPayment > 0.001)
                        TAB_SETROW(tabHeader_, TABLE_CLCARD.COLS.FLOATF1, REGISTERS.monthPayment);





                    e.Result = true;
                }





                public void Dispose()
                {

                    REGISTERS = null;
                    PLUGIN = null;

                }

            }


            class MY_SAVEINVOICE : IDisposable
            {
                Registers REGISTERS;
                _PLUGIN PLUGIN;
                DataTable LINES;




                public MY_SAVEINVOICE(_PLUGIN pPLUGIN, Registers pREGISTERS, DataTable pLines, short pTrCode, bool pCancelled)
                {

                    REGISTERS = pREGISTERS;
                    PLUGIN = pPLUGIN;
                    LINES = pLines;
                    string adp_ = "";
                    switch (pTrCode)
                    {
                        case 8:
                            adp_ = "adp.sls.doc.inv.8";
                            break;
                        case 3:
                            adp_ = "adp.sls.doc.inv.3";
                            break;
                        case 1:
                            adp_ = "adp.prch.doc.inv.1";
                            break;
                        case 6:
                            adp_ = "adp.prch.doc.inv.6";
                            break;
                        default:
                            throw new Exception("Undefined operation type [" + pTrCode + "]");
                    }

                    if (pCancelled)
                        adp_ = adp_ + " cancel::1";

                    if (!ISEMPTYLREF(REGISTERS.docLRef))
                        PLUGIN.EXEADPCMD(new string[] { adp_ + " cmd::edit lref::" + _PLUGIN.FORMAT(REGISTERS.docLRef) }, new DoWorkEventHandler[] { DONE }, true);//in global batch
                    else
                        PLUGIN.EXEADPCMD(new string[] { adp_ + " cmd::add" }, new DoWorkEventHandler[] { DONE }, true);//in global batch
                }

                public void DONE(object sender, System.ComponentModel.DoWorkEventArgs e)
                {

                    e.Result = false;


                    DataSet doc_ = ((DataSet)e.Argument);

                    DataTable tabHeaderInv_ = TAB_GETTAB(doc_, "INVOICE");
                    DataTable tabHeaderSlip_ = TAB_GETTAB(doc_, "STFICHE");
                    DataTable tabLine_ = TAB_GETTAB(doc_, "STLINE");
                    //////////////////////////////////////////////////////////////////////////////

                    //////////////////////////////////////////////////////////////////////////////
                    string invCode = REGISTERS.getDocCode(PLUGIN);
                    string slipCode = REGISTERS.getDocCode(PLUGIN);



                    TAB_SETROW(tabHeaderInv_, "FICHENO", invCode);
                    TAB_SETROW(tabHeaderSlip_, "FICHENO", slipCode);

                    TAB_SETROW(tabHeaderInv_, "DUMMY_____DATETIME", REGISTERS.getDocDate());
                    TAB_SETROW(tabHeaderInv_, "DOCDATE", REGISTERS.getDocDate().Date);

                    TAB_SETROW(tabHeaderInv_, "SOURCEINDEX", REGISTERS.warehouse >= 0 ? REGISTERS.warehouse : 0);
                    TAB_SETROW(tabHeaderInv_, "CLIENTREF", _PLUGIN.ISNULL(REGISTERS.clcard.getClientRef(), 0));
                    //
                    TAB_SETROW(tabHeaderInv_, "DOCODE", ISNULL(REGISTERS.docCode, ""));
                    TAB_SETROW(tabHeaderInv_, "SPECODE", ISNULL(REGISTERS.speCode, ""));

                    TAB_SETROW(tabHeaderInv_, "INTERESTAPP", REGISTERS.userToCashAmount);

                    List<string> listExp4 = new List<string>();
                    listExp4.Add(REGISTERS.userToCashAmount > 0.01 ? "CASH," + FORMAT(REGISTERS.userToCashAmount) : "");
                    listExp4.Add(REGISTERS.month > 0 ? "MONTH," + FORMAT(REGISTERS.month) : "");
                    listExp4.Add(REGISTERS.monthPayment > 0.01 ? "CREDIT," + FORMAT(REGISTERS.monthPayment) : "");

                    TAB_SETROW(tabHeaderInv_, "GENEXP4", JOINLIST(listExp4.ToArray()));

                    //
                    TAB_SETROW(tabHeaderInv_, "CYPHCODE", REGISTERS.cyhpCode);

                    TAB_SETROW(tabHeaderInv_, "DOCTRACKINGNR", _PLUGIN.ISNULL(REGISTERS.trackno, ""));

                    if (PRM.USE_BONUS_MAQ)
                    {
                        if (!ISNUMZERO(REGISTERS.useBonus) && !string.IsNullOrEmpty(REGISTERS.trackno))
                        {
                            var x = FORMAT(Math.Round(REGISTERS.useBonus, 2));
                            TAB_SETROW(tabHeaderInv_, "DOCTRACKINGNR", _PLUGIN.ISNULL(REGISTERS.trackno + "/" + REGISTERS.useBonus, ""));

                        }



                    }
                    TAB_SETROW(tabHeaderInv_, "GENEXP1", _PLUGIN.ISNULL(REGISTERS.desc1, ""));

                    TAB_SETROW(tabHeaderInv_, "SALESMANREF", _PLUGIN.ISNULL(REGISTERS.slsMan, 0));
                    //
                    if (!ISEMPTYLREF(REGISTERS.parentDocLRef))
                    {
                        var data_ = TAB_GETLASTROW(PLUGIN.SQL(MY_CHOOSE_SQL(
                            "select FICHENO,DOCTRACKINGNR from LG_$FIRM$_$PERIOD$_INVOICE with(nolock) WHERE LOGICALREF = @P1",
                              "select FICHENO,DOCTRACKINGNR from LG_$FIRM$_$PERIOD$_INVOICE WHERE LOGICALREF = @P1"),
                            new object[] { REGISTERS.parentDocLRef }));
                        if (data_ != null)
                        {
                            TAB_SETROW(tabHeaderInv_, "DOCTRACKINGNR", TAB_GETROW(data_, "DOCTRACKINGNR"));
                            TAB_SETROW(tabHeaderInv_, "GENEXP3", TAB_GETROW(data_, "FICHENO"));
                        }
                    }

                    //


                    DataTable dataCompact_ = LINES;// joinLines(LINES); ;

                    //delete rows
                    List<DataRow> listRowsDel_ = new List<DataRow>();
                    List<DataRow> listRowsTrans_ = new List<DataRow>();
                    List<DataRow> listRowsData_ = new List<DataRow>();

                    foreach (DataRow row_ in dataCompact_.Rows)
                    {
                        object lref_ = TAB_GETROW(row_, TABLE_STLINE.COLS.LOGICALREF);
                        DataRow rowTrans_ = null;

                        //if commented all rows will be new
                        if (!ISEMPTYLREF(lref_))
                        {
                            rowTrans_ = TAB_SEARCH(tabLine_, TABLE_STLINE.COLS.LOGICALREF, lref_);
                        }

                        if (rowTrans_ == null)
                            rowTrans_ = TABLE_STLINE.TOOLS.addTrans(tabLine_);



                        listRowsTrans_.Add(rowTrans_);
                        listRowsData_.Add(row_);
                    }

                    foreach (DataRow row in tabLine_.Rows)
                        if (!TAB_ROWDELETED(row) && !listRowsTrans_.Contains(row)) //if hasnt paired data row
                            listRowsDel_.Add(row);

                    foreach (DataRow row in listRowsDel_)
                        if (!TAB_ROWDELETED(row))
                            row.Delete();


                    for (int i = 0; i < listRowsData_.Count; ++i)
                        syncTransAndDataRows(listRowsData_[i], listRowsTrans_[i]);

                    if (!ISNUMZERO(REGISTERS.discountPerc))
                    {
                        DataRow rowGlobDisc_ = TABLE_STLINE.TOOLS.addTrans(tabLine_);

                        TAB_SETROW(rowGlobDisc_, TABLE_STLINE.COLS.GLOBTRANS, 1);
                        TAB_SETROW(rowGlobDisc_, TABLE_STLINE.COLS.LINETYPE, 2);
                        TAB_SETROW(rowGlobDisc_, TABLE_STLINE.COLS.DISCPER, REGISTERS.discountPerc);
                    }

                    e.Result = true;
                }


                static void syncTransAndDataRows(DataRow pDataRow, DataRow pTransRow)
                {
                    TAB_SETROW(pTransRow, TABLE_STLINE.COLS.GLOBTRANS, TAB_GETROW(pDataRow, TABLE_STLINE.COLS.GLOBTRANS));
                    TAB_SETROW(pTransRow, TABLE_STLINE.COLS.LINETYPE, TAB_GETROW(pDataRow, TABLE_STLINE.COLS.LINETYPE));
                    TAB_SETROW(pTransRow, TABLE_STLINE.COLS.STOCKREF, TAB_GETROW(pDataRow, TABLE_STLINE.COLS.STOCKREF));
                    TAB_SETROW(pTransRow, TABLE_STLINE.COLS.UOMREF, TAB_GETROW(pDataRow, TABLE_STLINE.COLS.UOMREF));
                    TAB_SETROW(pTransRow, TABLE_STLINE.COLS.AMOUNT, TAB_GETROW(pDataRow, TABLE_STLINE.COLS.AMOUNT));
                    TAB_SETROW(pTransRow, TABLE_STLINE.COLS.PRICE, TAB_GETROW(pDataRow, TABLE_STLINE.COLS.PRICE));
                    TAB_SETROW(pTransRow, TABLE_STLINE.COLS.DISCPER, TAB_GETROW(pDataRow, TABLE_STLINE.COLS.DISCPER));
                    TAB_SETROW(pTransRow, TABLE_STLINE.COLS.SPECODE, TAB_GETROW(pDataRow, TABLE_STLINE.COLS.SPECODE));
                    TAB_SETROW(pTransRow, TABLE_STLINE.COLS.LINEEXP, TAB_GETROW(pDataRow, TABLE_STLINE.COLS.LINEEXP));
                    TAB_SETROW(pTransRow, TABLE_STLINE.COLS.VAT, TAB_GETROW(pDataRow, TABLE_STLINE.COLS.VAT));

                    if (PRM.TERM_TYPE == TERMTYPE.hotel)
                    {
                        TAB_SETROW(pTransRow, TABLE_STLINE.COLS.DATEBEG, TAB_GETROW(pDataRow, TABLE_STLINE.COLS.DATEBEG));
                        TAB_SETROW(pTransRow, TABLE_STLINE.COLS.DATEEND, TAB_GETROW(pDataRow, TABLE_STLINE.COLS.DATEEND));
                    }
                }


                public void Dispose()
                {

                    REGISTERS = null;
                    PLUGIN = null;
                    LINES = null;
                }

            }


            class MY_SAVESLIP_COUNT : IDisposable
            {
                Registers REGISTERS;
                _PLUGIN PLUGIN;
                DataTable LINES;

                short TRCODE;


                public MY_SAVESLIP_COUNT(_PLUGIN pPLUGIN, Registers pREGISTERS, DataTable pLines, short pTrCode, bool pCancelled)
                {
                    if (!PRM.USE_ONHAND)
                        throw new Exception("Counting require warehouse ONHAND");

                    TRCODE = pTrCode;
                    REGISTERS = pREGISTERS;
                    PLUGIN = pPLUGIN;
                    LINES = pLines;
                    string adp_ = "";
                    switch (TRCODE)
                    {
                        case 50:
                            adp_ = "adp.mm.doc.slip.50";
                            break;
                        case 51:
                            adp_ = "adp.mm.doc.slip.51";
                            break;
                        default:
                            throw new Exception("Undefined operation type [" + pTrCode + "]");
                    }

                    if (pCancelled)
                        adp_ = adp_ + " cancel::1";

                    PLUGIN.EXEADPCMD(new string[] { adp_ + " cmd::add" }, new DoWorkEventHandler[] { DONE }, true);//in global batch
                }

                public void DONE(object sender, System.ComponentModel.DoWorkEventArgs e)
                {

                    e.Result = false;


                    DataSet doc_ = ((DataSet)e.Argument);

                    DataTable tabHeaderSlip_ = TAB_GETTAB(doc_, "STFICHE");
                    DataTable tabLine_ = TAB_GETTAB(doc_, "STLINE");
                    //////////////////////////////////////////////////////////////////////////////

                    //////////////////////////////////////////////////////////////////////////////

                    string slipCode = REGISTERS.getDocCode(PLUGIN);

                    TAB_SETROW(tabHeaderSlip_, "FICHENO", slipCode);

                    TAB_SETROW(tabHeaderSlip_, "DUMMY_____DATETIME", REGISTERS.getDocDate());

                    TAB_SETROW(tabHeaderSlip_, "SOURCEINDEX", REGISTERS.warehouse);

                    TAB_SETROW(tabHeaderSlip_, "DOCODE", ISNULL(REGISTERS.docCode, ""));
                    TAB_SETROW(tabHeaderSlip_, "SPECODE", ISNULL(REGISTERS.speCode, ""));

                    TAB_SETROW(tabHeaderSlip_, "CYPHCODE", REGISTERS.cyhpCode);

                    TAB_SETROW(tabHeaderSlip_, "DOCTRACKINGNR", _PLUGIN.ISNULL(REGISTERS.trackno, ""));

                    TAB_SETROW(tabHeaderSlip_, "GENEXP1", _PLUGIN.ISNULL(REGISTERS.desc1, ""));

                    foreach (DataRow row in LINES.Rows)
                    {
                        if (!TAB_ROWDELETED(row) && TABLE_STLINE.TOOLS.isLocalMat(row))
                        {
                            syncTransAndDataRows(row, tabLine_);
                        }
                    }

                    e.Result = (tabLine_.Rows.Count > 0); //save if has data
                }


                void syncTransAndDataRows(DataRow pDataRow, DataTable pTabTarget)
                {

                    var quantity_ = CASTASDOUBLE(TAB_GETROW(pDataRow, TABLE_STLINE.COLS.AMOUNT));
                    var onhand_ = CASTASDOUBLE(TAB_GETROW(pDataRow, TABLE_STLINE.COLS.DUMMY_ONHAND));

                    var diff_ = quantity_ - onhand_;

                    if (ISNUMZERO(diff_))
                        return;

                    if (diff_ > 0 && TRCODE != 50)
                        return;

                    if (diff_ < 0 && TRCODE != 51)
                        return;

                    var newRow_ = TABLE_STLINE.TOOLS.addTrans(pTabTarget);

                    TAB_SETROW(newRow_, TABLE_STLINE.COLS.STOCKREF, TAB_GETROW(pDataRow, TABLE_STLINE.COLS.STOCKREF));

                    TAB_SETROW(newRow_, TABLE_STLINE.COLS.AMOUNT, Math.Abs(diff_));

                    TAB_SETROW(newRow_, TABLE_STLINE.COLS.SPECODE, TAB_GETROW(pDataRow, TABLE_STLINE.COLS.SPECODE));
                    TAB_SETROW(newRow_, TABLE_STLINE.COLS.LINEEXP, TAB_GETROW(pDataRow, TABLE_STLINE.COLS.LINEEXP));


                }


                public void Dispose()
                {

                    REGISTERS = null;
                    PLUGIN = null;
                    LINES = null;
                }

            }



            class MY_SAVEPRODUCTION : IDisposable
            {
                Registers REGISTERS;
                _PLUGIN PLUGIN;
                DataTable LINES;
                DataRow LINE;
                short TRCODE;


                public MY_SAVEPRODUCTION(_PLUGIN pPLUGIN, Registers pREGISTERS, DataTable pLines, short pTrCode, bool pCancelled)
                {


                    TRCODE = pTrCode;
                    REGISTERS = pREGISTERS;
                    PLUGIN = pPLUGIN;
                    LINES = pLines;
                    string adp_ = "";
                    switch (TRCODE)
                    {
                        case 13:
                            adp_ = "adp.mm.doc.prod";
                            break;

                        default:
                            throw new Exception("Undefined operation type [" + pTrCode + "]");
                    }

                    foreach (DataRow l in pLines.Rows)
                    {
                        LINE = l;
                        PLUGIN.EXEADPCMD(new string[] { adp_ + " cmd::add" }, new DoWorkEventHandler[] { DONE }, true);//in global batch
                    }
                }

                public void DONE(object sender, System.ComponentModel.DoWorkEventArgs e)
                {

                    e.Result = false;


                    DataSet doc_ = ((DataSet)e.Argument);

                    DataTable tabHeaderSlip_ = TAB_GETTAB(doc_, "QPRODUCT");

                    //////////////////////////////////////////////////////////////////////////////

                    //////////////////////////////////////////////////////////////////////////////

                    //string slipCode = REGISTERS.getDocCode(PLUGIN);

                    //TAB_SETROW(tabHeaderSlip_, "FICHENO", slipCode);

                    TAB_SETROW(tabHeaderSlip_, "DUMMY_____DATETIME", REGISTERS.getDocDate());


                    //   TAB_SETROW(tabHeaderSlip_, "SOURCEINDEX", 0);
                    //  TAB_SETROW(tabHeaderSlip_, "DEPARTMENT", 0);

                    //  TAB_SETROW(tabHeaderSlip_, "DOCODE", "");

                    TAB_SETROW(tabHeaderSlip_, "CYPHCODE", REGISTERS.cyhpCode);

                    //  TAB_SETROW(tabHeaderSlip_, "DOCTRACKINGNR", _PLUGIN.ISNULL(REGISTERS.trackno, ""));

                    //   TAB_SETROW(tabHeaderSlip_, "GENEXP1", _PLUGIN.ISNULL(REGISTERS.desc1, ""));


                    var quantity_ = CASTASDOUBLE(TAB_GETROW(LINE, TABLE_STLINE.COLS.AMOUNT));

                    if (ISNUMZERO(quantity_))
                        return;

                    var stock_ = TAB_GETROW(LINE, TABLE_STLINE.COLS.STOCKREF);

                    if (ISNULL(PLUGIN.SQLSCALAR(MY_CHOOSE_SQL(
                        "SELECT TOP(1) 1 FROM LG_$FIRM$_STCOMPLN WITH(NOLOCK) WHERE MAINCREF = @P1",
                          "SELECT 1 FROM LG_$FIRM$_STCOMPLN WHERE MAINCREF = @P1 LIMIT 1"),
                        new object[] { stock_ })))
                        return;


                    TAB_SETROW(tabHeaderSlip_, TABLE_QPRODUCT.COLS.ITEMREF, stock_);

                    TAB_SETROW(tabHeaderSlip_, TABLE_QPRODUCT.COLS.AMOUNT, quantity_);

                    TAB_SETROW(tabHeaderSlip_, TABLE_QPRODUCT.COLS.HIDDEN_____RECALC, 1);






                    e.Result = true; //save if has data
                }





                public void Dispose()
                {

                    REGISTERS = null;
                    PLUGIN = null;
                    LINES = null;
                }

            }


            class MY_SAVEPRICE : IDisposable
            {
                Registers REGISTERS;
                _PLUGIN PLUGIN;




                public double priceSls;
                public double pricePrch;

                public object matRef;




                string adp_ = "adp.mm.rec.mat";

                public MY_SAVEPRICE(_PLUGIN pPLUGIN, Registers pREGISTERS, DataTable pLines, short pTrCode, bool pCancelled)
                {

                    REGISTERS = pREGISTERS;
                    PLUGIN = pPLUGIN;


                    foreach (DataRow row in pLines.Rows)
                        if (TABLE_STLINE.TOOLS.isLocalMat(row))
                        {
                            matRef = TAB_GETROW(row, TABLE_STLINE.COLS.STOCKREF);
                            priceSls = CASTASDOUBLE(ISNULL(TAB_GETROW(row, TABLE_STLINE.COLS.PRICE), 0.0));


                            RUN();
                        }
                }
                public MY_SAVEPRICE(_PLUGIN pPLUGIN, Registers pREGISTERS)
                {


                    REGISTERS = pREGISTERS;
                    PLUGIN = pPLUGIN;

                }

                public void RUN()
                {

                    if (!ISEMPTYLREF(matRef) && (!ISNUMZERO(priceSls) || !ISNUMZERO(pricePrch)))
                    {

                        var suf_ = FORMAT(ISNULL(PLUGIN.SQLSCALAR(MY_CHOOSE_SQL
                        (
                        "SELECT CARDTYPE FROM LG_$FIRM$_ITEMS I WITH(NOLOCK) WHERE LOGICALREF=@P1",
                        "SELECT CARDTYPE FROM LG_$FIRM$_ITEMS I WHERE LOGICALREF=@P1"
                        ),
                        new object[] { matRef }), 1));

                        PLUGIN.EXEADPCMD(new string[] { adp_ + "/" + suf_ + " cmd::edit lref::" + _PLUGIN.FORMAT(matRef) }, new DoWorkEventHandler[] { DONE }, true);//in global batch


                    }
                }
                bool UPDATEPRCH()
                {
                    return PRM.UPDATE_PRCH_PRICE_IN_CARD;

                }

                bool UPDATEPRICECOIF()
                {
                    return PRM.UPDATE_PRICE_COIF;

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
                            pricePrch_ = CASTASDOUBLE(ISNULL(PLUGIN.SQLSCALAR(
                                MY_CHOOSE_SQL(
@"

SELECT TOP(1) ((VATMATRAH+VATAMNT+DISTEXP)/AMOUNT) PRICE 
FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK) 
WHERE 
STOCKREF = @P1 AND 
TRCODE = 1 AND 
CANCELLED=0 AND 
LINETYPE = 0 
ORDER BY STOCKREF DESC,DATE_ DESC",
@"

SELECT ((VATMATRAH+VATAMNT+DISTEXP)/AMOUNT) PRICE 
FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK) 
WHERE 
STOCKREF = @P1 AND 
TRCODE = 1 AND 
CANCELLED=0 AND 
LINETYPE = 0 
ORDER BY STOCKREF DESC,DATE_ DESC
LIMIT 1"),
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

                    if (UPDATEPRICECOIF())
                    {

                        if (!ISNUMZERO(pricePrch_))
                        {
                            var c = 100 * (priceSls_ - pricePrch_) / pricePrch_;
                            TAB_SETROW(matRec_, "FLOATF1", c);
                        }
                    }


                    e.Result = true;
                }




                public void Dispose()
                {

                    REGISTERS = null;
                    PLUGIN = null;

                }

            }

            class MY_SAVEPAYMENT : IDisposable
            {
                Registers REGISTERS;
                _PLUGIN PLUGIN;
                bool cashInput;

                short invTrCode;


                public MY_SAVEPAYMENT(_PLUGIN pPLUGIN, Registers pREGISTERS, short pInvoiceTrcode)
                {
                    invTrCode = pInvoiceTrcode;
                    REGISTERS = pREGISTERS;
                    PLUGIN = pPLUGIN;
                    cashInput = true;

                    string adp_ = "";
                    switch (invTrCode)
                    {
                        case 8:
                        case 6:
                            adp_ = "adp.fin.doc.cash.11";
                            break;
                        case 3:
                        case 1:
                            adp_ = "adp.fin.doc.cash.12";
                            break;
                        default:
                            throw new Exception("Undefined operation type [" + invTrCode + "]");
                    }


                    string cmd_ = adp_ + " cmd::add";

                    PLUGIN.EXEADPCMD(new string[] { cmd_ }, new DoWorkEventHandler[] { this.DONE }, true);//in global batch
                }


                string getCashCode()
                {
                    var cashCode = REGISTERS.cashCode ?? "";

                    if (string.IsNullOrEmpty(cashCode))
                        cashCode = _LANG.L.CURRENCY;

                    cashCode = cashCode.Trim();

                    //string docCashCode_ = "" + _LANG.L.CURRENCY + ""; //MY_CURR_DESC(curr_);
                    //string docCash2Code_ = "" + _LANG.L.CURRENCY + "" + "STAFF" + _PLUGIN.RIGHT("000" + _PLUGIN.FORMAT(PLUGIN.GETSYSPRM_USER()), 3);

                    string code_ = CASTASSTRING(PLUGIN.SQLSCALAR(MY_CHOOSE_SQL(
                        "SELECT TOP(1) CODE FROM LG_$FIRM$_KSCARD WITH(NOLOCK) WHERE CODE IN (@P1 ) ",
                        "SELECT  CODE FROM LG_$FIRM$_KSCARD WHERE CODE IN (@P1 ) LIMIT 1"),
                        new object[] { cashCode }));

                    if (code_ == "")
                    {
                        string msg = _LANG.L.MSG_ERROR_CASH + " [" + cashCode + "]";
                        PLUGIN.MSGUSERERROR(msg);
                        throw new Exception("Cant find cash [" + cashCode + "]");
                    }

                    return code_;
                }

                public void DONE(object sender, System.ComponentModel.DoWorkEventArgs e)
                {

                    e.Result = false;




                    //short trcurr_ = 0;
                    object docClLRef_ = REGISTERS.clcard.getClientRef();

                    DateTime docDate_ = REGISTERS.getDocDate(); //MY_ASKDATE("T_DATE", DateTime.Now);

                    if (ISEMPTYLREF(docClLRef_))
                        return;

                    short curr_ = 0; //MY_CURR("T_CURRENCY",trcurr_);
                    string docCashCode_ = "" + _LANG.L.CURRENCY + ""; //MY_CURR_DESC(curr_);


                    double matAmount_ = REGISTERS.getPaymentWinAmount();



                    if (matAmount_ <= 0.01)
                        return;

                    var docNr_ = REGISTERS.getDocCodeCash(PLUGIN);

                    //
                    DataSet doc_ = ((DataSet)e.Argument);

                    DataTable tabHeader_ = TAB_GETTAB(doc_, "KSLINES");
                    DataTable tabLine_ = TAB_GETTAB(doc_, "CLFLINE");


                    TAB_SETROW(tabHeader_, "KSCARD_____CODE", getCashCode());
                    TAB_SETROW(tabHeader_, "CLIENTREF", docClLRef_);
                    TAB_SETROW(tabHeader_, "DUMMY_____DATETIME", docDate_);
                    TAB_SETROW(tabHeader_, "CYPHCODE", PRM.CYPHCODE);

                    TAB_SETROW(tabHeader_, "AMOUNT", matAmount_);
                    TAB_SETROW(tabHeader_, "TRCURR", curr_);
                    TAB_SETROW(tabHeader_, "TRNET", matAmount_);


                    TAB_SETROW(tabHeader_, "FICHENO", docNr_);
                    TAB_SETROW(tabHeader_, "FICHENO2", docNr_);

                    TAB_SETROW(tabHeader_, "LINEEXP", REGISTERS.getDocCode(PLUGIN));

                    double amountMainCurr_ = _PLUGIN.CASTASDOUBLE(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(tabHeader_, "AMOUNT"), 0));


                    _PLUGIN.TAB_SETROW(tabHeader_, "SPECODE", "");


                    _PLUGIN.TAB_SETROW(tabLine_, "TRANNO", docNr_);



                    if (_PLUGIN.ISEMPTYLREF(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(tabHeader_, "CARDREF"), 0)))
                    {
                        string msg = _LANG.L.MSG_ERROR_CASH + " [" + docCashCode_ + "]";
                        PLUGIN.MSGUSERERROR(msg);
                        throw new Exception("Cant find cash [" + docCashCode_ + "]");
                    }
                    if (_PLUGIN.ISEMPTYLREF(_PLUGIN.ISNULL(_PLUGIN.TAB_GETROW(tabHeader_, "CLIENTREF"), "")))
                    {
                        string msg = _LANG.L.MSG_ERROR_CLIENT + " " + _LANG.L.MSG_ERROR_CLIENT;
                        PLUGIN.MSGUSERERROR(msg);
                        throw new Exception("Cant find personal card");
                    }

                    //TRANNO


                    PLUGIN.SQL(MY_CHOOSE_SQL(
@"
update LG_$FIRM$_$PERIOD$_INVOICE set CYPHCODE = @P1 where FICHENO = @P2 and TRCODE in (@P3)
update LG_$FIRM$_$PERIOD$_STFICHE set CYPHCODE = @P1 where FICHENO = @P2 and TRCODE in (@P3)
", @"
update LG_$FIRM$_$PERIOD$_INVOICE set CYPHCODE = @P1 where FICHENO = @P2 and TRCODE in (@P3);
update LG_$FIRM$_$PERIOD$_STFICHE set CYPHCODE = @P1 where FICHENO = @P2 and TRCODE in (@P3);
"), new object[] { PRM.CYPHCODE_CASH, REGISTERS.getDocCode(PLUGIN), invTrCode });

                    e.Result = true;
                }

                double MY_ASKNUMBER(string pMsg, double pDef)
                {

                    DataRow[] rows_ = PLUGIN.REF("ref.gen.double desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
                    if (rows_ != null && rows_.Length > 0)
                    {
                        return _PLUGIN.CASTASDOUBLE(ISNULL(rows_[0]["VALUE"], 0.0));
                    }
                    return 0;

                }

                //DateTime MY_ASKDATE(string pMsg, DateTime pDef)
                //{

                //    DataRow[] rows_ = PLUGIN.REF("ref.gen.date desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDef));
                //    if (rows_ != null && rows_.Length > 0)
                //    {
                //        return _PLUGIN.CASTASDATE(ISNULL(rows_[0]["DATE_"], DateTime.Now));
                //    }
                //    return pDef;

                //}


                short MY_CURR(string pMsg, short pCurr)
                {

                    DataRow[] rows_ = PLUGIN.REF("ref.gen.definedlist obj::LIST_GEN_CURRENCY desc::" + _PLUGIN.STRENCODE(pMsg) + " filter::filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pCurr));
                    if (rows_ != null && rows_.Length > 0)
                    {
                        return (_PLUGIN.CASTASSHORT(ISNULL(rows_[0]["VALUE"], 0)));

                    }
                    return 0;

                }

                string MY_CURR_DESC(short pCurr)
                {
                    switch (pCurr)
                    {

                        case 0:
                        case 162:
                        case 21: return "AZN";
                        case 1: return "USD";
                        case 20: return "EUR";
                        case 160: return "TL";
                        default: return "";

                    }


                }
                public void Dispose()
                {

                    REGISTERS = null;
                    PLUGIN = null;

                }

            }


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



            public string QUANTITY = "Quantity";
            public string CODE = "Code";
            public string NAME = "Name";
            public string TAGS = "Tags";
            public string TAG = "Tag";
            public string SALES = "Sales";
            public string TABLE = "Table";
            public string SALER = "SalesMan";
            public string TOTAL = "Total";
            public string TEL = "Phone";
            public string CARD = "Card";
            public string FROMDATE = "From";
            public string TODATE = "To";
            public string TEXT = "Text";
            public string SK = "S.C.";
            public string PRICEPRCH = "Purch. Prc.";
            public string TOTALPRCH = "Purch. Tot.";
            public string DIFF = "Diff";
            public string REM = "Rem";
            public string REMTOT = "Rem Cost"; //+
            public string REMDIFF = "Rem Diff"; //+
            public string REMTOTDIF = "Rem Diff Tot"; //+
            public string REMCENTER = "Rem On Main";
            public string REMALL = "Rem All";

            public string ADDNEW = "Add";
            public string CUSTOMER = "Customer";

            public string DISCPERC = "Disc %";
            public string DISCTOT = "Disc Tot"; //+
            public string LASTPRINT = "Print";
            public string PAYMENT = "Paymet";
            public string PAYMENT_TYPE = "Paym. Type";
            public string PRICEGRP = "Price Grp";
            public string PROMO = "Promo";
            public string RETURN = "Return";
            public string SPECODE = "Spe. Code";
            public string DESC = "Desc";
            public string CUSTOMER2 = "Customer";



            public string PRICE = "Price";
            public string CUSTOMERS = "Customers";
            public string CUSTOMEROPEN = "New Cust.";
            public string BONUS = "Bonus";
            public string MONTHS = "Month";
            public string CLEAN = "Clean";
            public string RESTART = "Restart";
            public string QUIT = "Quit";
            public string SAVE = "Save";


            public string FILL = "Fill";
            public string SEARCH = "Search";
            public string WH = "WH";
            public string DATE = "Date";
            public string FILTERBYGRP = "Filter Grp";
            public string FILTERBYSPC = "Filter S.C.";
            public string PRINT = "Print";
            public string DOCS = "Docs";
            public string NEW = "New";
            public string BONUSMINUS = "Clean Bonus";



            public string SALE = "Sale";
            public string PURCHASE = "Purch";
            public string WHDOCS = "Warehouse Docs";
            public string HASPRCHPRCDIFF = "Purch Price Diff";
            public string CHANGEDMATS = "Changed Mats";
            public string DOPRODFORNEGATIVE = "Do For Neg.Level";

            public string DOCTYPE = "Doc Type";




            public string MATHREM = "Mat Rem";
            public string MATHREMTOT = "Rem Cost";

            public string PROFIT = "Profit";
            public string PURCHTOT = "Purch Tot";
            public string CREDITSUM = "Credit Tot";
            public string LIFE = "Pay. Period";
            public string MONTH = "Month";
            public string MOTHLYPAY = "Monthly Pay.";
            public string LASTPAYMENT = "Last Pay.";
            public string DEBIT = "Debt";

            public string SUBDISCOUNT = "Sub Disc";
            public string LINEDISC = "Line Disc";
            public string PRCHPRICE = "Purch Prc";
            public string SLSPRICE = "Sale Prc";

            public string VAT = "VAT";
            public string DISC = "Disc";
            public string AVG = "Avg.";

            public string NET = "Net";
            public string GROSS = "Gross";

            public string LIST = "List";
            public string PRCH = "Purch";
            public string SLS = "Sale";
            public string COST = "Cost";
            public string DOCNR = "Doc No";

            public string SET = "Set Of";
            public string PASSWD = "Password";
            public string CASHMONEY = "Cash Money";
            public string SLSPLACE = "Sale Place";



            public string SELECT = "Select";
            public string CLOSE = "Close";
            public string DAY = "Day";
            public string HOUR = "Hour";
            public string WHO = "Who";


            public string CURRENCY = "USD";



            public string MSG_ERROR_TERMINCORECT = "Terminal type is not inited";
            public string MSG_ERROR_WINBLOCKED = "Close other 'POS' windows ";
            public string MSG_ERROR_NUMER = "New doc add error";


            public string MSG_ERROR_MATERIAL = "Material not exist";
            public string MSG_ERROR_CLIENT = "Account R/P not exists";
            public string MSG_ERROR_WH = "Wh not exists";
            public string MSG_ERROR_BINDING = "Biinding not exists";
            public string MSG_ERROR_CASH = "Cash not exists";
            public string MSG_ERROR_MATERIAL_UNIT = "Unit not exists";
            public string MSG_ERROR_MATERIAL_PRICE = "Price not exists";
            public string MSG_ERROR_MATERIAL_BARCODE = "Barcode error";
            public string MSG_ERROR_CREATE = "New record adding error";
            public string MSG_ERROR_PRINT = "Print error";
            public string MSG_ERROR_NO_DATA = "No information";
            public string MSG_ERROR_DATERANGE = "Incorrect date";
            public string MSG_ERROR_INVALID_VAL = "Incorrect price";
            public string MSG_ERROR_PROMO = "Promo apply error";

            public string MSG_ERROR_CONN = "Connection error";
            public string MSG_ERROR_SQL = "Request error";

            public string MSG_ASK_CLEAN = "Clean ?";
            public string MSG_ASK_CLOSE = "Close window ?";
            public string MSG_ASK_DELETE = "Remove ?";
            public string MSG_ASK_PRINT = "Print ?";
            public string MSG_ASK_SAVE = "Save doc ?";

            public string MSG_INFO_OPENED = "Doc is open";
            public string MSG_INFO_RETURN = "Return";
            public string MSG_INFO_SALE = "Sale";
            public string MSG_INFO_ORDER = "Order";
            public string MSG_INFO_PURCHASE = "Purchase";
            public string MSG_INFO_PURCHASERET = "Purchase return";
            public string MSG_INFO_PRODUCTION = "Production";
            public string MSG_INFO_COUNT = "Inventory";
            public string MSG_INFO_BARCODE = "Barcode";
            public string MSG_INFO_LABEL = "Label"; 
            public string MSG_INFO_PRICE = "Price";
            public string MSG_INFO_NOTSELECTABLE = "Record select error";


            ///
            public string MSG_ERROR_NO_PRICE = "Noprice!";
            public string MSG_ERROR_NOPRICEINPARENTDOC = "Cant find price from parent doc";
            public string MSG_ERROR_SELECTPARENTFORRET = "Return require parent doc";

            public string MSG_PRINTFORTABLE = "Print for table ?";
            public string MSG_TABLETOT = "Table price";
            public string MSG_NO_DOC = "Doc not exists";

            public string MSG_FILL_BY_FILTER = "Fill by Filter ?";

            public string MSG_DO_FREE_PLACE = "Clean Table";
            public string MSG_ASK_DO_FREE_PLACE = "Do Table Free?";


            public void lang_az()
            {



                QUANTITY = "Miqdar";

                CODE = "Kod";
                NAME = "Ad";
                TAGS = "Təglər";
                TAG = "Təg";
                SALES = "Satışlar";
                TABLE = "Masa";
                SALER = "Satıcı";
                TOTAL = "Cəmi";
                TEL = "Tel";
                CARD = "Kart";
                FROMDATE = "Tarixdən";
                TODATE = "Tarixə";
                TEXT = "Mətn";
                SK = "S.K.";
                PRICEPRCH = "Qiymət Alış";
                TOTALPRCH = "Cəmi Alış";
                DIFF = "Fərg";
                REM = "Qalıq";
                REMTOT = "Qalıq Dəyəri"; //+
                REMDIFF = "Qalıq Fərqi"; //+
                REMTOTDIF = "Qalıq Dəyər Fərqi"; //+
                REMCENTER = "Qalıq Mərkəz.";
                REMALL = "Qalıq But.";

                ADDNEW = "Əlavə Et";
                CUSTOMER = "Müştəri";

                DISCPERC = "Endirim %";
                DISCTOT = "Endirim M."; //+
                LASTPRINT = "Çap Et S-cu";
                PAYMENT = "Ödəniş";
                PAYMENT_TYPE = "Öd. Növü";
                PRICEGRP = "Qiymət Qr.";
                PROMO = "Kampaniya";
                RETURN = "Qaytarma";
                SPECODE = "H.Kod";
                DESC = "Acıqlama";
                CUSTOMER2 = "Alıcı";



                PRICE = "Qiymət";
                CUSTOMERS = "Müştərilər";
                CUSTOMEROPEN = "Müştəri Aç";
                BONUS = "Bonus";
                MONTHS = "Aylar";
                CLEAN = "Sil";
                RESTART = "Yenidən";
                QUIT = "Çıx";
                SAVE = "Qeyd et";


                FILL = "Doldur";
                SEARCH = "Axtar";
                WH = "Ambar";
                DATE = "Tarix";
                FILTERBYGRP = "F. Qrup";
                FILTERBYSPC = "F. H.Kod";
                PRINT = "Çap Et";
                DOCS = "Sənədlər";
                NEW = "Yeni";
                BONUSMINUS = "Bonus silinsin";



                SALE = "Satış";
                PURCHASE = "Alış";
                WHDOCS = "Ambar qəbzləri";
                HASPRCHPRCDIFF = "Alış qiyməti fərqi olanlar";
                CHANGEDMATS = "Dəyiştirilmiş mallar";
                DOPRODFORNEGATIVE = "Mənfi Olanları İstehsalata";

                DOCTYPE = "Sənəd Növü";




                MATHREM = "Riy. Qaliq";
                MATHREMTOT = "Riy. Qaliq Dəy.";

                PROFIT = "Qazanc";
                PURCHTOT = "Cəmi Alış";
                CREDITSUM = "Kredit məbləqi";
                LIFE = "Müddət";
                MONTH = "Ay";
                MOTHLYPAY = "Aylıq Ödəniş";
                LASTPAYMENT = "Son Ödəniş";
                DEBIT = "Borc";

                SUBDISCOUNT = "Alt Endirim";
                LINEDISC = "Sətir Endirim";
                PRCHPRICE = "Alış Qiyməti";
                SLSPRICE = "Satış Qiyməti";

                VAT = "ƏDV";
                DISC = "Endirim";
                AVG = "Ortalama";

                NET = "Net";
                GROSS = "Qross";

                LIST = "Siyah";
                PRCH = "Alış";
                SLS = "Satış";
                COST = "Dəyər";
                DOCNR = "Sənəd nömrəsi";

                SET = "Çeşid";
                PASSWD = "Şifrə";
                CASHMONEY = "Nəğd";
                SLSPLACE = "Satış Yeri";



                SELECT = "Sec";
                CLOSE = "Bağla";
                DAY = "Gün";
                HOUR = "Saat";

                WHO = "Kim";

                CURRENCY = "AZN";


                MSG_ERROR_TERMINCORECT = "Terminal type is not inited";
                MSG_ERROR_WINBLOCKED = "Başka növ 'POS' pəncərəsi acıqdi";
                MSG_ERROR_NUMER = "Sənəd nömrəsi yaradilma səhfi";


                MSG_ERROR_MATERIAL = "Material tapılmadı";
                MSG_ERROR_CLIENT = "Cari tapılmadı";
                MSG_ERROR_WH = "Ambar tapılmadı";
                MSG_ERROR_BINDING = "Bağlantı tapılmadı";
                MSG_ERROR_CASH = "Kassa tapılmadı";
                MSG_ERROR_MATERIAL_UNIT = "Material birimi tapılmadı";
                MSG_ERROR_MATERIAL_PRICE = "Material qiyməti tapılmadı";
                MSG_ERROR_MATERIAL_BARCODE = "Yalnış barkod";
                MSG_ERROR_CREATE = "Təzə qeyd yaranma səhvi";
                MSG_ERROR_PRINT = "Cap səhvi";
                MSG_ERROR_NO_DATA = "Məlumat yoxdu";
                MSG_ERROR_DATERANGE = "Yalnış tarix";
                MSG_ERROR_INVALID_VAL = "Yalnış dəyər";
                MSG_ERROR_PROMO = "Kampaniya uyğulama səhvi";

                MSG_ERROR_CONN = "Bağlantı səhvi";
                MSG_ERROR_SQL = "Sorqulama səhvi";

                MSG_ASK_CLEAN = "Məlumat təmizlənsin ?";
                MSG_ASK_CLOSE = "Pəncərə bağlansın ?";
                MSG_ASK_DELETE = "Qeyd silinsin ?";
                MSG_ASK_PRINT = "Cap etmək ?";
                MSG_ASK_SAVE = "Qeyd olunsun sənəd ?";

                MSG_INFO_OPENED = "Sənəd acıqdı";
                MSG_INFO_RETURN = "Qaytarma";
                MSG_INFO_SALE = "Satış";
                MSG_INFO_ORDER = "Sifariş";
                MSG_INFO_PURCHASE = "Alış";
                MSG_INFO_PURCHASERET = "Alış Qaytarma";
                MSG_INFO_PRODUCTION = "İstehsalat";
                MSG_INFO_COUNT = "Sayım";
                MSG_INFO_BARCODE = "Barcode";
                MSG_INFO_LABEL = "Etiket";
                MSG_INFO_PRICE = "Qiymət";
                MSG_INFO_NOTSELECTABLE = "Qeyd secilə bilməz";


                ///
                MSG_ERROR_NO_PRICE = "Qiymət yoxdu !";
                MSG_ERROR_NOPRICEINPARENTDOC = "Qaytarma ücün ana sənəddə qiymət tapılmadi";
                MSG_ERROR_SELECTPARENTFORRET = "Qaytarma ücün ana sənəd secilməli";

                MSG_PRINTFORTABLE = "Masa ücun cap etmək ?";
                MSG_TABLETOT = "Masa dəyəri";
                MSG_NO_DOC = "Sənəd tapılmadı";

                MSG_FILL_BY_FILTER = "Filter üzrə doldurmaq ?";

                MSG_DO_FREE_PLACE = "Masanı Boşald";
                MSG_ASK_DO_FREE_PLACE = " Masanı Boşaldmaq ?";





            }

            public void lang_ru()
            {

                QUANTITY = "Количество";

                CODE = "Код";
                NAME = "Наименование";
                TAGS = "Разметки";
                TAG = "Разметка";
                SALES = "Продажи";
                TABLE = "Стол";
                SALER = "Продавец";
                TOTAL = "Сумма";
                TEL = "Тел";
                CARD = "Карт";
                FROMDATE = "С даты";
                TODATE = "До даты";
                TEXT = "Текст";
                SK = "С.К.";
                PRICEPRCH = "Закупочная цена";
                TOTALPRCH = "Закупочная сумма";
                DIFF = "Разница";
                REM = "Остаток";
                REMTOT = "Остаточная стоимость"; //+
                REMDIFF = "Разница остатка"; //+
                REMTOTDIF = "Разница остаточной стоимости"; //+
                REMCENTER = "Остаточк Центр.";
                REMALL = "Остаток.";

                ADDNEW = "Добавить";
                CUSTOMER = "Клиент";

                DISCPERC = "Скидка %";
                DISCTOT = "Скидка См."; //+
                LASTPRINT = "Печать псл.";
                PAYMENT = "Оплата";
                PAYMENT_TYPE = "Тип Оплаты";
                PRICEGRP = "Ценовая группа.";
                PROMO = "Промо";
                RETURN = "Возврат";
                SPECODE = "Спец код";
                DESC = "Описание";
                CUSTOMER2 = "Покупатель";



                PRICE = "Цена";
                CUSTOMERS = "Клиенты";
                CUSTOMEROPEN = "Новый клиент";
                BONUS = "Бонус";
                MONTHS = "Месяцы";
                CLEAN = "Очистить";
                RESTART = "Заново";
                QUIT = "Выйти";
                SAVE = "Сохранить";


                FILL = "Наполнить";
                SEARCH = "Искать";
                WH = "Склад";
                DATE = "Дата";
                FILTERBYGRP = "Ф. по Грп-е";
                FILTERBYSPC = "Ф. Спец Код";
                PRINT = "Печатать";
                DOCS = "Документы";
                NEW = "Новый";
                BONUSMINUS = "Очистить бонус";



                SALE = "Продажа";
                PURCHASE = "Закупка";
                WHDOCS = "Накладные по складу";
                HASPRCHPRCDIFF = "Разница закупочных цен";
                CHANGEDMATS = "Изменённые товары";
                DOPRODFORNEGATIVE = "Произв. для негатив.";

                DOCTYPE = "Тип документа";




                MATHREM = "Мат. остаток";
                MATHREMTOT = "Стоимость остатка";

                PROFIT = "Прибыль";
                PURCHTOT = "Общая закупка";
                CREDITSUM = "Сумма кредита";
                LIFE = "Период оплаты";
                MONTH = "Месяц";
                MOTHLYPAY = "Месячная оплата";
                LASTPAYMENT = "Последняя оплата";
                DEBIT = "Долг";

                SUBDISCOUNT = "Суб скидка";
                LINEDISC = "Скидка на линию";
                PRCHPRICE = "Закупочная цена";
                SLSPRICE = "Цена продажи";

                VAT = "НДС";
                DISC = "Скидка";
                AVG = "Усредн.";

                NET = "Нетто";
                GROSS = "Гросс";

                LIST = "Список";
                PRCH = "Закупка";
                SLS = "Продажа";
                COST = "Стоимость";
                DOCNR = "Номер документа";

                SET = "Артикуль";
                PASSWD = "Пароль";
                CASHMONEY = "Наличные";
                SLSPLACE = "Местопродажи";



                SELECT = "Выбрать";
                CLOSE = "Закрыть";
                DAY = "День";
                HOUR = "Часы";
                WHO = "Кто";


                CURRENCY = "РУБ.";


                MSG_ERROR_TERMINCORECT = "Terminal type is not inited";
                MSG_ERROR_WINBLOCKED = "Открыто другое окно 'POS' ";
                MSG_ERROR_NUMER = "Ошибка создания номера документа";


                MSG_ERROR_MATERIAL = "Материал не найден";
                MSG_ERROR_CLIENT = "Текущий Сч. не найден";
                MSG_ERROR_WH = "Склад не найден";
                MSG_ERROR_BINDING = "Привязка не найдена";
                MSG_ERROR_CASH = "Касса не найдена";
                MSG_ERROR_MATERIAL_UNIT = "Товарная единица не найдена";
                MSG_ERROR_MATERIAL_PRICE = "Цена товара не найдена";
                MSG_ERROR_MATERIAL_BARCODE = "Ошибочный штрих-код";
                MSG_ERROR_CREATE = "Ошибка создания нового записа";
                MSG_ERROR_PRINT = "Ошибка печати";
                MSG_ERROR_NO_DATA = "Информации нету";
                MSG_ERROR_DATERANGE = "Ошибочная дата";
                MSG_ERROR_INVALID_VAL = "Ошибочная стоимость";
                MSG_ERROR_PROMO = "Ошибка приложения кампании";

                MSG_ERROR_CONN = "Ошибка соединения";
                MSG_ERROR_SQL = "Ошибка запроса";

                MSG_ASK_CLEAN = "Регулировать информацию ?";
                MSG_ASK_CLOSE = "Закрыть окно ?";
                MSG_ASK_DELETE = "Удалить заметку ?";
                MSG_ASK_PRINT = "Печатать ?";
                MSG_ASK_SAVE = "Документ записать ?";

                MSG_INFO_OPENED = "Документ открыт";
                MSG_INFO_RETURN = "Возврат";
                MSG_INFO_SALE = "Продажа";
                MSG_INFO_ORDER = "Заказ";
                MSG_INFO_PURCHASE = "Закупка";
                MSG_INFO_PURCHASERET = "Возврат закупки";
                MSG_INFO_PRODUCTION = "Производство";
                MSG_INFO_COUNT = "Ревизия";
                MSG_INFO_BARCODE = "Штрих-код";
                MSG_INFO_LABEL = "Этикетка";
                MSG_INFO_PRICE = "Цена";
                MSG_INFO_NOTSELECTABLE = "Запись невозможно выбрать";


                ///
                MSG_ERROR_NO_PRICE = "Нету цены !";
                MSG_ERROR_NOPRICEINPARENTDOC = "На основном документе не найдена цена для возврата";
                MSG_ERROR_SELECTPARENTFORRET = "Для возврата нужно выбрать основной документ";

                MSG_PRINTFORTABLE = "Печатать для стола ?";
                MSG_TABLETOT = "Стоимость стола";
                MSG_NO_DOC = "Документ не найден";

                MSG_FILL_BY_FILTER = "Заполнить для фильтра ?";

                MSG_DO_FREE_PLACE = "Очистить стол";
                MSG_ASK_DO_FREE_PLACE = " Очистить стол ?";




            }

            public void lang_tr()
            {


                QUANTITY = "Miktar";

                CODE = "Kod";
                NAME = "İsim";
                TAGS = "Etiketler";
                TAG = "Etiket";
                SALES = "Satışlar";
                TABLE = "Masa";
                SALER = "Satıcı";
                TOTAL = "Toplam";
                TEL = "Tel";
                CARD = "Kart";
                FROMDATE = "Baş.Tarihi";
                TODATE = "Bit.Tarihi";
                TEXT = "Metin";
                SK = "S.K.";
                PRICEPRCH = "Alış Fiyatı";
                TOTALPRCH = "Toplam Alış";
                DIFF = "Fark";
                REM = "Kalan";
                REMTOT = "Kalan Değer"; //+
                REMDIFF = "Kalan Fark"; //+
                REMTOTDIF = "Kalan Değer"; //+
                REMCENTER = "Merkez Kalan";
                REMALL = "Tüm Kalan";

                ADDNEW = "Yeni Ekle";
                CUSTOMER = "Cari Hesap";

                DISCPERC = "İndirim %";
                DISCTOT = "İnd. Tplm."; //+
                LASTPRINT = "Print";//En Son
                PAYMENT = "Tahsilat";
                PAYMENT_TYPE = "Tahsilat Tür";
                PRICEGRP = "Fiyat Grup";
                PROMO = "Kampanya";
                RETURN = "İade";
                SPECODE = "Özel Kod";
                DESC = "Açıklama";
                CUSTOMER2 = "Alıcı";

                PRICE = "Fiyat";
                CUSTOMERS = "Müşteriler";
                CUSTOMEROPEN = "Müşteri aç";
                BONUS = "Ödül";
                MONTHS = "Aylar";
                CLEAN = "Sil";
                RESTART = "Yeniden";//Yeniden başlat
                QUIT = "Çık";
                SAVE = "Kaydet";

                FILL = "Doldur";
                SEARCH = "Ara";
                WH = "Ambar";
                DATE = "Tarih";
                FILTERBYGRP = "Fltr. Grup";
                FILTERBYSPC = "Fltr. Ö.K.";
                PRINT = "Print";//Yazdır
                DOCS = "Evraklar";
                NEW = "Yeni";
                BONUSMINUS = "Prim";

                SALE = "Satış";
                PURCHASE = "Alış";
                WHDOCS = "Depo Fiş";
                HASPRCHPRCDIFF = "Alış fiyat farkı olanlar";
                CHANGEDMATS = "Değiştilimiş Ürünler";
                DOPRODFORNEGATIVE = "Negatif düşünleri üret";

                DOCTYPE = "Evrak Türü";




                MATHREM = "Gerçek Kalan";
                MATHREMTOT = "Gerçek Kln. T."; //Toplam

                PROFIT = "Kar";
                PURCHTOT = "Toplam Alış";
                CREDITSUM = "Alacak Toplam";
                LIFE = "Süre";
                MONTH = "Ay";
                MOTHLYPAY = "Aylık Tahsilat";
                LASTPAYMENT = "Son Tahsilat";//Enson Tahsilat
                DEBIT = "Borç";

                SUBDISCOUNT = "Alt İndirim";
                LINEDISC = "Satır İndirim";
                PRCHPRICE = "Alış Fiyatı";
                SLSPRICE = "Satış Fiyatı";

                VAT = "KDV";
                DISC = "İndirim";
                AVG = "Ortalama";

                NET = "Net";
                GROSS = "Brüt";

                LIST = "Liste";
                PRCH = "Alış";
                SLS = "Satış";
                COST = "Maliyet";
                DOCNR = "Belge Num.";

                SET = "Kur";
                PASSWD = "Şifre";
                CASHMONEY = "Nakit";
                SLSPLACE = "Satış Yeri";



                SELECT = "Seç";
                CLOSE = "Kapat";
                DAY = "Gün";
                HOUR = "Saat";

                WHO = "Kim";

                CURRENCY = "TL";



                MSG_ERROR_TERMINCORECT = "Terminal Doğru Değil";
                MSG_ERROR_WINBLOCKED = "Ekran Bloke oldu";
                MSG_ERROR_NUMER = "Numara Hatası";


                MSG_ERROR_MATERIAL = "Malzeme Hatası";
                MSG_ERROR_CLIENT = "Cari Hesap Hatası";
                MSG_ERROR_WH = "Ambar Hatası";
                MSG_ERROR_BINDING = "Bağlantı Hatası";
                MSG_ERROR_CASH = "Kasa Hatası";
                MSG_ERROR_MATERIAL_UNIT = "Malzeme Birim Hatası";
                MSG_ERROR_MATERIAL_PRICE = "Malzeme Fiyat Hatası";
                MSG_ERROR_MATERIAL_BARCODE = "Hatalı Barkod ";
                MSG_ERROR_CREATE = "Oluşturma Hatası";
                MSG_ERROR_PRINT = "Yazıcı Hatası";
                MSG_ERROR_NO_DATA = "Veri Bulunamadı";
                MSG_ERROR_DATERANGE = "Tarih Aralığı Hatasıatası";
                MSG_ERROR_INVALID_VAL = "Değer Hatası";
                MSG_ERROR_PROMO = "Kampanya Uygulama Hatası";

                MSG_ERROR_CONN = "Bağlantı Hatası";
                MSG_ERROR_SQL = "SQL Hatası";

                MSG_ASK_CLEAN = "Temizlensinmi ?";
                MSG_ASK_CLOSE = "Pencere Kapansınmı?";
                MSG_ASK_DELETE = "Kayıt Silinsinmi ?";
                MSG_ASK_PRINT = "Yazdırsınmı ?";
                MSG_ASK_SAVE = "Kayıt edilsinmi?";

                MSG_INFO_OPENED = "Açık Bilgisi";
                MSG_INFO_RETURN = "İade";
                MSG_INFO_SALE = "Satış";
                MSG_INFO_ORDER = "Siparişler";
                MSG_INFO_PURCHASE = "Alış";
                MSG_INFO_PURCHASERET = "Alış İade";
                MSG_INFO_PRODUCTION = "Üretim";
                MSG_INFO_COUNT = "Sayım";
                MSG_INFO_BARCODE = "Barkod";
                MSG_INFO_LABEL = "Etiket";
                MSG_INFO_PRICE = "Fiyat";
                MSG_INFO_NOTSELECTABLE = "Tablo Seçilemez";


                ///
                MSG_ERROR_NO_PRICE = "Fiyatı yok !";
                MSG_ERROR_NOPRICEINPARENTDOC = "Fiş de iade fiyatı bulunmuyor";
                MSG_ERROR_SELECTPARENTFORRET = "İade için fiş seçilmedi";

                MSG_PRINTFORTABLE = "Tablo için yazdır?";
                MSG_TABLETOT = "Masa Toplamı";
                MSG_NO_DOC = "Fiş Bulunamadı";

                MSG_FILL_BY_FILTER = "Filitreyi Doldurdunmu ?";

                MSG_DO_FREE_PLACE = "Yer Boşaltın";
                MSG_ASK_DO_FREE_PLACE = "Masanı boşalt?";


            }
        }

        //END






        #region TOOL

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



        #region CLASS





        class MY_DIR
        {

            public static string PRM_DIR_ROOT = PATHCOMBINE(
                 Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "POS");

            public static string PRM_DIR_COUNTING = PATHCOMBINE(PRM_DIR_ROOT, "COUNT");


            public static void SAVE(string pDir, string pFile, string pData)
            {

                if (!Directory.Exists(pDir))
                    Directory.CreateDirectory(pDir);
                FILEWRITE(PATHCOMBINE(pDir, pFile), pData, true);

            }

            public static void SAVE(_PLUGIN pPLUGIN, string pDir, string pFile, DataTable pLines, BARCODETERM_BASE.Registers pRegs)
            {




                StringBuilder sb = new StringBuilder();

                var xmlDoc = new System.Xml.XmlDocument();
                // xmlDoc.AppendChild(xmlDoc.CreateProcessingInstruction("xml", "version='1.0' encoding='UTF-8'"));

                var docs = xmlDoc.CreateElement("DOCS");
                var doc = xmlDoc.CreateElement("STFICHE");

                doc.SetAttribute("SOURCEINDEX", FORMAT(pRegs.warehouse));
                doc.SetAttribute("GENEXP1", FORMAT(pRegs.desc1));
                doc.SetAttribute("TRCODE", FORMAT(50));
                doc.SetAttribute("FICHENO", pRegs.getDocCode(pPLUGIN));

                xmlDoc.AppendChild(docs);
                docs.AppendChild(doc);

                foreach (DataRow row in pLines.Rows)
                    if (!TAB_ROWDELETED(row))
                    {

                        var line = xmlDoc.CreateElement("STLINE");

                        line.SetAttribute("STOCKREF", FORMAT(CASTASINT(TAB_GETROW(row, "STOCKREF"))));
                        line.SetAttribute("AMOUNT", FORMAT(ROUND(CASTASDOUBLE(TAB_GETROW(row, "AMOUNT")), 6)));
                        line.SetAttribute("PRICE", FORMAT(ROUND(CASTASDOUBLE(TAB_GETROW(row, "PRICE")), 6)));
                        line.SetAttribute("ITEMS_____CODE", CASTASSTRING(TAB_GETROW(row, "ITEMS_CODE")));
                        line.SetAttribute("ITEMS_____NAME", CASTASSTRING(TAB_GETROW(row, "ITEMS_NAME")));

                        doc.AppendChild(line);

                    }


                SAVE(pDir, pFile, XMLDOCFORMAT(xmlDoc));



            }

        }


        #endregion





        #endregion
