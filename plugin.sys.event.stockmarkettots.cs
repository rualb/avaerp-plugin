#line 2


 
   #region BODY
        //BEGIN

        const int VERSION = 5;
        const string FILE = "plugin.sys.event.stockmarkettots.pls";

        const string event_STOCKMARKETTOTS_ = "hadlericom_stockmarkettots_";
        const string event_STOCKMARKETTOTS_DAYEND = "hadlericom_stockmarkettots_dayend";
        const string event_STOCKMARKETTOTS_GENSTATE = "hadlericom_stockmarkettots_genstate";
        const string event_STOCKMARKETTOTS_COUNTS = "hadlericom_stockmarkettots_counts";


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

                    x.MY_STOCKMARKETTOTS_USER = s.MY_STOCKMARKETTOTS_USER;
                    x.MY_STOCKMARKETTOTS_PW = s.MY_STOCKMARKETTOTS_PW;



                    //

                    _SETTINGS.BUF = x;

                }

                public string MY_STOCKMARKETTOTS_USER;
                public string MY_STOCKMARKETTOTS_PW;


            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.text_DESC) //, "ava.production.config")
            {

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Active On User")]
            public string MY_STOCKMARKETTOTS_USER
            {
                get
                {
                    return (_GET("MY_STOCKMARKETTOTS_USER", ""));
                }
                set
                {
                    _SET("MY_STOCKMARKETTOTS_USER", value);
                }

            }

            [ECategory(TEXT.text_DESC)]
            [EDisplayName("Password")]
            public string MY_STOCKMARKETTOTS_PW
            {
                get
                {
                    return (_GET("MY_STOCKMARKETTOTS_PW", ""));
                }
                set
                {
                    _SET("MY_STOCKMARKETTOTS_PW", value);
                }

            }

            //

            public static bool ISUSEROK(_PLUGIN pPLUGIN)
            {
                return (BUF.MY_STOCKMARKETTOTS_USER == "") ||
                      (Array.IndexOf<string>(
                     EXPLODELIST(BUF.MY_STOCKMARKETTOTS_USER),
                     FORMAT(pPLUGIN.GETSYSPRM_USER())
                     ) >= 0);
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
                var tree = CONTROL_SEARCH(FORM, "cTreeReports");
                if (tree != null)
                {
                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
            // { "_root" ,event_EMAILREP_},
			//{ "CmdText" ,"event name::"+event_STOCKMARKETTOTS_},
			{ "Text" ,TEXT.L.SIMPLE_TOT},
			{ "ImageName" ,"battery_32x32"},
			{ "Name" ,event_STOCKMARKETTOTS_},
            };

                        RUNUIINTEGRATION(tree, args);

                    }

                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
            { "_root" ,event_STOCKMARKETTOTS_},
			{ "CmdText" ,"event name::"+event_STOCKMARKETTOTS_DAYEND},
			{ "Text" ,TEXT.L.DAY_END},
			{ "ImageName" ,"calendar_32x32"},
			{ "Name" ,event_STOCKMARKETTOTS_DAYEND},
            };

                        RUNUIINTEGRATION(tree, args);

                    }



                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_STOCKMARKETTOTS_},
			{ "CmdText" ,"event name::"+event_STOCKMARKETTOTS_GENSTATE},
			{ "Text" ,TEXT.L.GEN_STATE},
			{ "ImageName" ,"pin_32x32"},
			{ "Name" ,event_STOCKMARKETTOTS_GENSTATE},
            };

                        RUNUIINTEGRATION(tree, args);

                    }



                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
             { "_root" ,event_STOCKMARKETTOTS_},
			{ "CmdText" ,"event name::"+event_STOCKMARKETTOTS_COUNTS},
			{ "Text" ,TEXT.L.COUNTS_MAT},
			{ "ImageName" ,"struct_32x32"},
			{ "Name" ,event_STOCKMARKETTOTS_COUNTS},
            };

                        RUNUIINTEGRATION(tree, args);

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

                string[] list_ = EXPLODELISTPATH(EVENTCODE);
                var cmd = list_.Length > 1 ? list_[1].ToLowerInvariant() : "";

                switch (cmd)
                {

                    case event_STOCKMARKETTOTS_GENSTATE:
                    case event_STOCKMARKETTOTS_DAYEND:
                    case event_STOCKMARKETTOTS_COUNTS:
                        {
                            MY_REP_HTML(cmd);
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


        void MY_REP_HTML(string pCmd)
        {


            var res = new StringBuilder();

            res.AppendLine("<html>");
            res.AppendLine(@" 

<style>
            table, th, td {
                border: 1px solid #dbdbdb;
                border-collapse: collapse;
                white-space: nowrap;
                padding: 3px;
 
            }
            table, h1, h2, h3, h4, h5, h6 {
                font-family: Segoe UI, Verdana, Aral;
            }
            th {
                font-weight:bold;
                text-align: center;
            }

</style>
 <body style=''>
");


            switch (pCmd)
            {
                case event_STOCKMARKETTOTS_DAYEND:
                    if (!checkPassword(this))
                        return;

                    #region DAYEND
                    {

                        var date = CASTASDATE(SQLSCALAR(
MY_CHOOSE_SQL(
@"select getdate()"
,
@"select now()::timestamp(0)"
)
 ));

                        var df = date.Date;
                        var dt = df.AddDays(+1).AddSeconds(-1);

                        if (!MY_ASKDATETIME(this, "T_DATE_RANGE", ref df, ref dt))
                            return;



                        var tf = GETTIMETOTIMEINT(df);
                        var tt = GETTIMETOTIMEINT(dt);



                        var data = (SQL(
MY_CHOOSE_SQL(

@"
 
declare @df datetime = @P1
declare @dt datetime = @P2
declare @tf int = @P3
declare @tt int = @P4

 
SELECT TOP (10000) SUM(CASE 
			WHEN TRCODE = 8
				THEN NETTOTAL
			ELSE 0
			END) TOT_SLS
	,SUM(CASE 
			WHEN TRCODE = 3
				THEN NETTOTAL
			ELSE 0
			END) TOT_SLSRET
	,SUM(CASE 
			WHEN TRCODE = 8
				THEN 1
			ELSE 0
			END) COUNT_SLS
	,SUM(CASE 
			WHEN TRCODE = 3
				THEN 1
			ELSE 0
			END) COUNT_SLSRET
	,SUM(CASE 
			WHEN (TRCODE = 8 AND DOCTRACKINGNR LIKE '+[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]/[0-9]%')
				THEN (CAST( substring(DOCTRACKINGNR,1+15,10) as FLOAT))
			ELSE 0.0
			END) TOT_BONUS
	,(
		SELECT CODE
		FROM LG_$FIRM$_CLCARD CLCARD WITH (NOLOCK)
		WHERE INVOICE.CLIENTREF = CLCARD.LOGICALREF
		) CLCARD_CODE
	,(
		SELECT DEFINITION_
		FROM LG_$FIRM$_CLCARD CLCARD WITH (NOLOCK)
		WHERE INVOICE.CLIENTREF = CLCARD.LOGICALREF
		) CLCARD_DEFINITION_,
	MAX(FICHENO) FICHENO
FROM LG_$FIRM$_$PERIOD$_INVOICE INVOICE WITH (NOLOCK) WHERE (
			( DATE_ > @df OR (DATE_ = @df AND TIME_ > @tf) )
			AND 
			( DATE_ < @dt OR (DATE_ = @dt AND TIME_ <= @tt) )
		)
	AND INVOICE.CANCELLED = 0
	AND INVOICE.TRCODE IN (
		3
		,8
		)
	AND @df <= @dt
GROUP BY INVOICE.CLIENTREF
 
",


@"
 
 
SELECT SUM(CASE 
			WHEN TRCODE = 8
				THEN NETTOTAL
			ELSE 0
			END) TOT_SLS
	,SUM(CASE 
			WHEN TRCODE = 3
				THEN NETTOTAL
			ELSE 0
			END) TOT_SLSRET
	,SUM(CASE 
			WHEN TRCODE = 8
				THEN 1
			ELSE 0
			END) COUNT_SLS
	,SUM(CASE 
			WHEN TRCODE = 3
				THEN 1
			ELSE 0
			END) COUNT_SLSRET
	,SUM(CASE 
			WHEN (TRCODE = 8 AND DOCTRACKINGNR LIKE '+[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]/[0-9]%')
				THEN (CAST( substring(DOCTRACKINGNR,1+15,10) as FLOAT))
			ELSE 0.0
			END) TOT_BONUS
	,(
		SELECT CODE
		FROM LG_$FIRM$_CLCARD CLCARD 
		WHERE INVOICE.CLIENTREF = CLCARD.LOGICALREF
		) CLCARD_CODE
	,(
		SELECT DEFINITION_
		FROM LG_$FIRM$_CLCARD CLCARD 
		WHERE INVOICE.CLIENTREF = CLCARD.LOGICALREF
		) CLCARD_DEFINITION_,
	MAX(FICHENO) FICHENO
FROM LG_$FIRM$_$PERIOD$_INVOICE INVOICE WHERE (
			( DATE_ > @P1 OR (DATE_ = @P1 AND TIME_ > @P3) )
			AND 
			( DATE_ < @P2 OR (DATE_ = @P2 AND TIME_ <= @P4) )
		)
	AND INVOICE.CANCELLED = 0
	AND INVOICE.TRCODE IN (
		3
		,8
		)
	AND @P1 <= @P2
GROUP BY INVOICE.CLIENTREF
LIMIT 10000
"
 ), new object[] { df.Date, dt.Date, tf, tt }));





                        //name
                        res.AppendLine(string.Format("<h2 style='width:100%;text-align:center'>{0}</h2>",
                           HTMLENCODE(TEXT.L.DAY_END)
                              ));


                        //filter
                        {

                            var lines = new List<string[]>();

                            lines.Add(new string[] { LANG("T_DATE_FROM"), FORMAT(df, "yyyy-MM-dd HH:mm") });
                            lines.Add(new string[] { LANG("T_DATE_TO"), FORMAT(dt, "yyyy-MM-dd HH:mm") });

                            res.AppendLine("<table>");

                            foreach (var row in lines)
                            {

                                res.AppendLine("<tr>");
                                foreach (var cell in row)
                                {
                                    res.AppendLine(string.Format(
                                    "<td>{0}</td>",
                                     cell
                                   ));
                                }
                                res.AppendLine("</tr>");

                            }


                            res.AppendLine("</table>");
                        }


                        res.AppendLine("<br/>");

                        //body
                        {
                            res.AppendLine("<table style='width:100%;'>");

                            res.AppendLine("<tr>");
                            foreach (var cell in new string[] { 
                                LANG("T_CODE"), 
                                LANG("T_NAME"), 
                                LANG("T_SALE (T_NET) <br/> T_SYS_CURR1"),
                                LANG("T_SALE (T_GROSS) <br/> T_SYS_CURR1"),
                                LANG("T_RETURN <br/> T_SYS_CURR1"),
                                LANG("T_SALE <br/> T_COUNT"),
                                LANG("T_RETURN <br/> T_COUNT"),
                             
                            })
                            {
                                res.AppendLine(string.Format(
                                "<th>{0}</th>",
                                 cell
                               ));
                            }
                            res.AppendLine("</tr>");


                            var slsSUM = 0.0;
                            var slsCountSUM = 0.0;
                            var slsRetSUM = 0.0;
                            var slsRetCountSUM = 0.0;
                            var slsNetSUM = 0.0;
                            //

                            for (int i = 0; i <= data.Rows.Count; ++i)
                            {
                                var indx = i + 1;
                                //

                                var isDark = false;
                                var isBold = false;

                                var isSumRow = false;

                                //
                                var row = i < data.Rows.Count ? data.Rows[i] : null;
                                //
                                isSumRow = row == null;

                                string[] arrCell = null;

                                if (row != null)
                                {


                                    var clCode = CASTASSTRING(TAB_GETROW(row, "CLCARD_CODE"));
                                    var clName = CASTASSTRING(TAB_GETROW(row, "CLCARD_DEFINITION_"));
                                    var sls = ROUND(CASTASDOUBLE(TAB_GETROW(row, "TOT_SLS")), 2);
                                    var slsCount = ROUND(CASTASDOUBLE(TAB_GETROW(row, "COUNT_SLS")), 2);
                                    var slsRet = ROUND(CASTASDOUBLE(TAB_GETROW(row, "TOT_SLSRET")), 2);
                                    var slsRetCount = ROUND(CASTASDOUBLE(TAB_GETROW(row, "COUNT_SLSRET")), 2);
                                    var slsNet = (sls - slsRet);

                                    slsSUM += sls;
                                    slsCountSUM += slsCount;
                                    slsRetSUM += slsRet;
                                    slsRetCountSUM += slsRetCount;
                                    slsNetSUM += slsNet;

                                    arrCell = new string[] { 
                                        clCode,
                                        clName,
                                        FORMAT(slsNet,"N2"),
                                        FORMAT(sls,"N2"),
                                        FORMAT(slsRet,"N2"),
                                        FORMAT(slsCount,"N2"),
                                        FORMAT(slsRetCount,"N2" ) 

                            };

                                }
                                else
                                {
                                    isDark = true;
                                    isBold = true;

                                    arrCell = new string[] { 
                                        "",
                                        LANG("T_TOTAL"),
                                        FORMAT(slsNetSUM,"N2"),
                                        FORMAT(slsSUM,"N2"),
                                        FORMAT(slsRetSUM,"N2"),
                                        FORMAT(slsCountSUM,"N2"),
                                        FORMAT(slsRetCountSUM,"N2" ) 

                            };

                                }


                                var backColor = "#FFFFFF";

                                //if (indx % 2 == 1)
                                //    backColor = "#F2F2F2";

                                if (isDark)
                                    backColor = "#B0B0B0";


                                var fontWeight = isBold ? "bold" : "normal";

                                res.AppendLine("<tr style='background-color:" + backColor + ";font-weight:" + fontWeight + "'>");
                                // foreach (var cellVal in arrCell)
                                for (int c = 0; c < arrCell.Length; ++c)
                                {
                                    var cellVal = arrCell[c];
                                    res.AppendLine(string.Format(
                                        "<td style='background-color:" + (c == 2 ? "#74ba5d" : "") + ";text-align: " + (c >= 2 ? "right" : "") + ";'>{0}</td>",
                                     cellVal
                                   ));
                                }
                                res.AppendLine("</tr>");




                            }

                            res.AppendLine("</table>");
                        }
                    }
                    #endregion
                    break;
                case event_STOCKMARKETTOTS_GENSTATE:
                    #region GENSTATE
                    {

                        var dataWh = (SQL(
MY_CHOOSE_SQL(
@"
 
select ROUND(sum(ONHAND*PRICE_PURCHASE) ,0) TOT_P,ROUND(sum(ONHAND*PRICE_SALE) ,0) TOT_S 
from (
select T.STOCKREF,SUM(ONHAND) ONHAND,
            COALESCE(

            (
                            SELECT TOP (1) PRICE PRICE
			                FROM LG_$FIRM$_$PERIOD$_STLINE WITH (NOLOCK)
			                WHERE (STOCKREF =  T.STOCKREF AND VARIANTREF >= 0 AND DATE_ >= '19000101' AND 
                                FTIME >= 0 AND IOCODE = 1  AND SOURCEINDEX IN (0)) AND (
					                CANCELLED = 0 AND LINETYPE = 0 AND TRCODE IN (1  )  
					                )
			                ORDER BY STOCKREF DESC,
				                VARIANTREF DESC,
				                DATE_ DESC,
				                FTIME DESC,
				                IOCODE DESC,
				                SOURCEINDEX DESC,
				                LOGICALREF DESC
            ), 
            (
                            SELECT TOP(1) PRICE FROM LG_$FIRM$_PRCLIST (NOLOCK) 
                            WHERE (CARDREF=T.STOCKREF) AND (PTYPE=1) order by BEGDATE desc 
            ),
            0.0
                    )   PRICE_PURCHASE,
            COALESCE(

            (
                SELECT TOP (1) PRICE PRICE
			    FROM LG_$FIRM$_$PERIOD$_STLINE WITH (NOLOCK)
			    WHERE (STOCKREF =  T.STOCKREF AND VARIANTREF >= 0 AND DATE_ >= '19000101' AND 
                    FTIME >= 0 AND IOCODE =  4  AND SOURCEINDEX IN (0)) AND (
					    CANCELLED = 0 AND LINETYPE = 0 AND TRCODE IN ( 8 )  
					    )
			    ORDER BY STOCKREF DESC,
				    VARIANTREF DESC,
				    DATE_ DESC,
				    FTIME DESC,
				    IOCODE DESC,
				    SOURCEINDEX DESC,
				    LOGICALREF DESC
            ), 
            (
                            SELECT TOP(1) PRICE FROM LG_$FIRM$_PRCLIST (NOLOCK) 
                            WHERE (CARDREF=T.STOCKREF) AND (PTYPE=2) order by BEGDATE desc 
            ),
            0.0
                    )  PRICE_SALE
from 
LG_$FIRM$_$PERIOD$_STINVTOT T WITH(NOLOCK)
where T.INVENNO = -1
GROUP BY T.STOCKREF
) D WHERE D.ONHAND > 0

",
@"
 
select ROUND(sum(ONHAND*PRICE_PURCHASE)::numeric,0)::float TOT_P,ROUND(sum(ONHAND*PRICE_SALE)::numeric,0)::float TOT_S 
from (
select T.STOCKREF,SUM(ONHAND) ONHAND,
            COALESCE(

            (
                            SELECT PRICE PRICE
			                FROM LG_$FIRM$_$PERIOD$_STLINE 
			                WHERE (STOCKREF =  T.STOCKREF AND VARIANTREF >= 0 AND DATE_ >= '19000101' AND 
                                FTIME >= 0 AND IOCODE = 1  AND SOURCEINDEX IN (0)) AND (
					                CANCELLED = 0 AND LINETYPE = 0 AND TRCODE IN (1  )  
					                )
			                ORDER BY STOCKREF DESC,
				                VARIANTREF DESC,
				                DATE_ DESC,
				                FTIME DESC,
				                IOCODE DESC,
				                SOURCEINDEX DESC,
				                LOGICALREF DESC
                            LIMIT 1
            ), 
            (
                            SELECT PRICE FROM LG_$FIRM$_PRCLIST 
                            WHERE (CARDREF=T.STOCKREF) AND (PTYPE=1) order by BEGDATE desc 
                            LIMIT 1
            ),
            0.0
                    )   PRICE_PURCHASE,
            COALESCE(

            (
                SELECT PRICE PRICE
			    FROM LG_$FIRM$_$PERIOD$_STLINE 
			    WHERE (STOCKREF =  T.STOCKREF AND VARIANTREF >= 0 AND DATE_ >= '19000101' AND 
                    FTIME >= 0 AND IOCODE =  4  AND SOURCEINDEX IN (0)) AND (
					    CANCELLED = 0 AND LINETYPE = 0 AND TRCODE IN ( 8 )  
					    )
			    ORDER BY STOCKREF DESC,
				    VARIANTREF DESC,
				    DATE_ DESC,
				    FTIME DESC,
				    IOCODE DESC,
				    SOURCEINDEX DESC,
				    LOGICALREF DESC
                            LIMIT 1
            ), 
            (
                            SELECT PRICE FROM LG_$FIRM$_PRCLIST 
                            WHERE (CARDREF=T.STOCKREF) AND (PTYPE=2) order by BEGDATE desc 
                            LIMIT 1
            ),
            0.0
                    )  PRICE_SALE
from 
LG_$FIRM$_$PERIOD$_STINVTOT T 
where T.INVENNO = -1
GROUP BY T.STOCKREF
) D WHERE D.ONHAND > 0

")
, new object[] { }));

                        var dataCl = (SQL(
MY_CHOOSE_SQL(
@"
 
 select 
sum(case when DEBIT-CREDIT > 0 then DEBIT-CREDIT else 0 end) DEBIT,
sum(case when DEBIT-CREDIT < 0 then CREDIT-DEBIT else 0 end) CREDIT
from LG_$FIRM$_$PERIOD$_GNTOTCL T  WITH(NOLOCK)
inner join 
LG_$FIRM$_CLCARD C  WITH(NOLOCK)
on T.CARDREF =  C.LOGICALREF
where T.TOTTYP = 1 and C.CARDTYPE IN (1,2,3)

",
@"
 
 select 
sum(case when DEBIT-CREDIT > 0 then DEBIT-CREDIT else 0 end) DEBIT,
sum(case when DEBIT-CREDIT < 0 then CREDIT-DEBIT else 0 end) CREDIT
from LG_$FIRM$_$PERIOD$_GNTOTCL T 
inner join 
LG_$FIRM$_CLCARD C 
on T.CARDREF =  C.LOGICALREF
where T.TOTTYP = 1 and C.CARDTYPE IN (1,2,3)

")


, new object[] { }));


                        var dataCash = (SQL(
MY_CHOOSE_SQL(
@"
 
SELECT SUM(DEBIT-CREDIT) CASH 
FROM LG_$FIRM$_$PERIOD$_CSHTOTS WITH(NOLOCK) WHERE CARDREF > 0 AND TOTTYPE = 1 AND DAY_ > -1

",
@"
 
SELECT SUM(DEBIT-CREDIT) CASH 
FROM LG_$FIRM$_$PERIOD$_CSHTOTS WHERE CARDREF > 0 AND TOTTYPE = 1 AND DAY_ > -1

")
, new object[] { }));


                        //name
                        res.AppendLine(string.Format("<h2 style='width:100%;text-align:center'>{0}</h2>",
                           HTMLENCODE(TEXT.L.GEN_STATE)
                              ));


                        //filter
                        {

                            var lines = new List<string[]>();

                            var v1 = CASTASDOUBLE(ISNULL(TAB_GETROW(dataWh, "TOT_P"), 0.0));
                            var v2 = CASTASDOUBLE(ISNULL(TAB_GETROW(dataWh, "TOT_S"), 0.0));
                            var v3 = CASTASDOUBLE(ISNULL(TAB_GETROW(dataCl, "DEBIT"), 0.0));
                            var v4 = CASTASDOUBLE(ISNULL(TAB_GETROW(dataCl, "CREDIT"), 0.0));
                            var v5 = CASTASDOUBLE(ISNULL(TAB_GETROW(dataCash, "CASH"), 0.0));


                            var sufixCurr = " " + LANG("T_SYS_CURR1");

                            lines.Add(new string[] { TEXT.L.WH_TOT_PRCH, FORMAT(v1, "N0") + sufixCurr });
                            lines.Add(new string[] { TEXT.L.WH_TOT_SLS, FORMAT(v2, "N0") + sufixCurr });
                            lines.Add(new string[] { TEXT.L.CL_SUM_DEBIT, FORMAT(v3, "N0") + sufixCurr });
                            lines.Add(new string[] { TEXT.L.CL_SUM_CREDIT, FORMAT(v4, "N0") + sufixCurr });
                            lines.Add(new string[] { TEXT.L.CASH_TOT, FORMAT(v5, "N0") + sufixCurr });


                            res.AppendLine("<table>");

                            foreach (var row in lines)
                            {

                                res.AppendLine("<tr>");

                                for (int c = 0; c < row.Length; ++c)
                                {

                                    var textalign = "";

                                    if (c > 0)
                                        textalign = "right";

                                    var val = row[c];
                                    res.AppendLine(string.Format(
                                    "<td style='font-weight:bold;text-align:" + textalign + "'>{0}</td>",
                                     val
                                   ));
                                }
                                res.AppendLine("</tr>");

                            }


                            res.AppendLine("</table>");
                        }


                        res.AppendLine("<br/>");


                    }
                    #endregion
                    break;
                case event_STOCKMARKETTOTS_COUNTS:
                    #region COUNTS
                    {
                        var now = DateTime.Now.Date;
                        var before30 = now.AddDays(-30);


                        var data = (SQL(@"
 
select 
(select count(LOGICALREF) from LG_$FIRM$_ITEMS where ACTIVE IN (0)) MAT1,
(select count(distinct STOCKREF) from LG_$FIRM$_$PERIOD$_GNTOTST where ONHAND>0.01) MAT2,
(select count(distinct STOCKREF) from LG_$FIRM$_$PERIOD$_STINVTOT where (DATE_ between @P1  and @P2 ) AND (SALAMNT > 0.01)) MAT3,

(select count(LOGICALREF) from LG_$FIRM$_ITEMS where CARDTYPE IN (1) and ACTIVE IN (0)) MAT_T1,
(select count(LOGICALREF) from LG_$FIRM$_ITEMS where CARDTYPE IN (10) and ACTIVE IN (0)) MAT_T10,
(select count(LOGICALREF) from LG_$FIRM$_ITEMS where CARDTYPE IN (11) and ACTIVE IN (0)) MAT_T11,
(select count(LOGICALREF) from LG_$FIRM$_ITEMS where CARDTYPE IN (12) and ACTIVE IN (0)) MAT_T12,

(select count(LOGICALREF) from LG_$FIRM$_CLCARD where CARDTYPE IN (1) and ACTIVE IN (0)) CL1,
(select count(LOGICALREF) from LG_$FIRM$_CLCARD where CARDTYPE IN (2) and ACTIVE IN (0)) CL2,
(select count(LOGICALREF) from LG_$FIRM$_CLCARD where CARDTYPE IN (3) and ACTIVE IN (0)) CL3,



(select 1) TEST

", new object[] { now, before30 }));

                        TAB_FILLNULL(data);

                        //name
                        res.AppendLine(string.Format("<h2 style='width:100%;text-align:center'>{0}</h2>",
                           HTMLENCODE(TEXT.L.COUNTS_MAT)
                              ));


                        var colDesc = new Dictionary<string, string>() { 
                        {"MAT1",TEXT.L.COUNTS_MAT_ALL},
                        {"MAT2",TEXT.L.COUNTS_MAT_ONHAND},
                        {"MAT3",TEXT.L.COUNTS_MAT_SLS},

                        {"MAT_T1",LANG("T_MATERIAL (T_MAT_COMMERCIAL_GOOD)")},
                        {"MAT_T10",LANG("T_MATERIAL (T_MAT_RAW_MATERIAL)")},
                        {"MAT_T11",LANG("T_MATERIAL (T_MAT_SEMI_FINISHED_GOOD)")},
                        {"MAT_T12",LANG("T_MATERIAL (T_MAT_FINISHED_GOOD)")},
 
                        {"CL1",LANG("T_PERSONAL (T_PERSONAL_1)")},
                        {"CL2",LANG("T_PERSONAL (T_PERSONAL_2)")},
                        {"CL3",LANG("T_PERSONAL (T_PERSONAL_3)")},

                        };

                        //filter
                        {

                            var lines = new List<string[]>();

                            foreach (string key in colDesc.Keys)
                            {
                                var type = data.Columns[key].DataType;
                                var isFloat = (type == typeof(double));
                                var format = isFloat ? "N2" : "N0";


                                var desc = colDesc[key];
                                var valStr = FORMAT(ISNULL(TAB_GETROW(data, key), 0.0), format);

                                //
                                lines.Add(new string[] { desc, valStr });

                            }







                            res.AppendLine("<table>");

                            foreach (var row in lines)
                            {

                                res.AppendLine("<tr>");

                                for (int c = 0; c < row.Length; ++c)
                                {

                                    var textalign = "";

                                    if (c > 0)
                                        textalign = "right";

                                    var val = row[c];
                                    res.AppendLine(string.Format(
                                    "<td style='font-weight:bold;text-align:" + textalign + "'>{0}</td>",
                                     val
                                   ));
                                }
                                res.AppendLine("</tr>");

                            }


                            res.AppendLine("</table>");
                        }


                        res.AppendLine("<br/>");


                    }
                    #endregion
                    break;




            }

            res.AppendLine("</body></html>");

            MSGUSERINFO(res.ToString());





        }



        //END



        #region CLAZZ

        class TEXT
        {

            public const string text_DESC = "Simple Tots";


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

                SIMPLE_TOT = "Basit Rakamlar";
                WH_TOT = "Depo Toplamı";
                DAY_END = "Gün Sonu";
                GEN_STATE = "Genel Durum";

                WH_TOT_PRCH = "Depo Enson Alış Maliyeti";
                WH_TOT_SLS = "Depo Enson Satış Maliyeti";
                CL_SUM_DEBIT = "Cari H. Bakiye Borç";
                CL_SUM_CREDIT = "Cari H. Bakiye Alacak";

                CASH_TOT = "Kasa";

                COUNTS_MAT = "Kart Sayısı";
                COUNTS_MAT_ALL = "Tüm Malzeme Kartları";
                COUNTS_MAT_ONHAND = "Depodaki Malzemeler";
                COUNTS_MAT_SLS = "Satışa Kullanılmış Malzemeler (Son 30 Gün)";

            }


            public void lang_en()
            {

                SIMPLE_TOT = "Simple Tots";
                WH_TOT = "Wh Tots";
                DAY_END = "Day End";
                GEN_STATE = "General State";


                WH_TOT_PRCH = "Warehouse Cost By Last Purchase Price";
                WH_TOT_SLS = "Warehouse Cost By Last Sale Price";
                CL_SUM_DEBIT = "Acounts Debit";
                CL_SUM_CREDIT = "Acounts Credit";

                CASH_TOT = "Cash";

                COUNTS_MAT = "Counts";
                COUNTS_MAT_ALL = "Material Counts";
                COUNTS_MAT_ONHAND = "Material Onhand";
                COUNTS_MAT_SLS = "Material used in Sale (last 30 days)";

            }

            public void lang_az()
            {

                SIMPLE_TOT = "Sadə Cəmlər";
                WH_TOT = "Ambar Cəmləri";
                DAY_END = "Gün Sonu";
                GEN_STATE = "Genel Durum";


                WH_TOT_PRCH = "Ambar Dəyəri Son Alış Qiy.";
                WH_TOT_SLS = "Ambar Dəyəri Son Satış Qiy.";
                CL_SUM_DEBIT = "Bizə Borc (Kreditorlar)";
                CL_SUM_CREDIT = "Bizim Borcumuz";

                CASH_TOT = "Kassa";

                COUNTS_MAT = "Say";
                COUNTS_MAT_ALL = "Ceşid Sayı";
                COUNTS_MAT_ONHAND = "Ceşid Əldə olanlar";
                COUNTS_MAT_SLS = "Satışı Olan (son 30 gün)";


            }



            public void lang_ru()
            {

                SIMPLE_TOT = "Упрощенные Итоги";
                WH_TOT = "Итог По Складу";
                DAY_END = "Дневной Итог";
                GEN_STATE = "Общее Состояние";

                WH_TOT_PRCH = "Себистоимость ТМЗ по Последней Закупке";
                WH_TOT_SLS = "Себистоимость ТМЗ по Последней Реализации";
                CL_SUM_DEBIT = "Дебиторская задолжность (Нам Должны)";
                CL_SUM_CREDIT = "Кредиторская задолжность (Наши Долги)";

                CASH_TOT = "Касса";

                COUNTS_MAT = "Наменклатура";
                COUNTS_MAT_ALL = "Материалов Всего";
                COUNTS_MAT_ONHAND = "Материалы в Наличаи";
                COUNTS_MAT_SLS = "Материалы Проданные (за 30 дней)";

            }

            public string SIMPLE_TOT;

            public string WH_TOT;
            public string DAY_END;
            public string GEN_STATE;
            public string COUNTS_MAT;

            public string WH_TOT_PRCH;
            public string WH_TOT_SLS;
            public string CL_SUM_DEBIT;
            public string CL_SUM_CREDIT;
            public string CASH_TOT;

            public string COUNTS_MAT_ALL;
            public string COUNTS_MAT_ONHAND;
            public string COUNTS_MAT_SLS;

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


        static void beepErr()
        {

            System.Media.SystemSounds.Asterisk.Play();
            System.Threading.Thread.Sleep(600);
            System.Media.SystemSounds.Asterisk.Play();

        }


        static bool checkPassword(_PLUGIN pPLUGIN)
        {

            if (_SETTINGS.BUF.MY_STOCKMARKETTOTS_PW == "")
                return true;


            var p = _SETTINGS.BUF.MY_STOCKMARKETTOTS_PW.ToLowerInvariant();

            while (true)
            {
                DataRow[] rows_ = pPLUGIN.REF("ref.gen.string password::1 desc::" + _PLUGIN.STRENCODE("" + pPLUGIN.LANG("T_PASSWORD") + "") + "");
                if (rows_ != null && rows_.Length > 0)
                {
                    var val_ = _PLUGIN.CASTASSTRING(ISNULL(rows_[0]["VALUE"], ""));


                    if (val_.ToLowerInvariant() == p)
                        return true;
                    else
                        beepErr();

                }
                else
                    return false;
            }


        }

        static bool MY_ASKDATETIME(_PLUGIN pPLUGIN, string pMsg, ref DateTime pDf, ref DateTime pDt)
        {

            DataRow[] rows_ = pPLUGIN.REF("ref.gen.daterange showtime::1 desc::" + _PLUGIN.STRENCODE(pMsg)
                + " filter::"
                + "filter_DATE1," + _PLUGIN.FORMATSERIALIZE(pDf)
                 + ";filter_DATE2," + _PLUGIN.FORMATSERIALIZE(pDt)
            )
                ;

            if (rows_ != null && rows_.Length > 0)
            {
                pDf = CASTASDATE(rows_[0]["DATETIME1"]);
                pDt = CASTASDATE(rows_[0]["DATETIME2"]);

                if (pDf > pDt)
                    return false;

                return true;
            }

            return false;

        }
        #endregion

        #endregion