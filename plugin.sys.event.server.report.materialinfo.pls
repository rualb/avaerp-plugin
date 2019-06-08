 #line 2
 
      #region PLUGIN_BODY
        const int VERSION = 9;



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

                    x._USER = PLUGIN.GETSYSPRM_USER();
                    x._FIRM = PLUGIN.GETSYSPRM_FIRM();
                    x._FIRMNAME = PLUGIN.GETSYSPRM_FIRMNAME();
                    x._PERIOD = PLUGIN.GETSYSPRM_PERIOD();



                    _SETTINGS.BUF = x;


                }


                public short _FIRM;
                public string _FIRMNAME;
                public short _PERIOD;
                public short _USER;



            }


            public _SETTINGS(_PLUGIN pPLUGIN)
                : base(pPLUGIN, TEXT.L.REPORT_NAME)
            {

            }





        }

        #endregion


        #region MAIN


        public void SYS_BEGIN(string CMD, object[] ARGS)
        {



            ARGS = (ARGS == null ? new object[0] : ARGS);

            object arg0 = ARGS.Length > 0 ? ARGS[0] : null;
            object arg1 = ARGS.Length > 1 ? ARGS[1] : null;
            object arg2 = ARGS.Length > 2 ? ARGS[2] : null;

            switch (CMD)
            {

                case SysEvent.SYS_LOGIN:

                    _SETTINGS._BUF.LOAD_SETTINGS(this);

                    break;
                case "DATAREP": //!!! return false in undefined state
                    {
                        var prop = arg0 as Dictionary<string, object>;
                        var cmd = arg1 as string;
                        var res = arg2 as List<string>;

                        var out_ = MY_DO_REPORT(prop, cmd);

                        if (out_ != null)
                            res.Add(out_);

                    }
                    break;

            }


        }




        string MY_DO_REPORT(Dictionary<string, object> pProp, string pCmd)
        {
            var res = new StringBuilder();
            //
            var agentNr = CASTASSHORT(pProp["AGENTNR"]);
            var agentName = CASTASSTRING(pProp["AGENTNAME"]);
            //var agentFolder = CASTASSTRING(pProp["AGENTFOLDER"]);
            var agentWh = CASTASSHORT(pProp["AGENTWH"]);

            //
            var repLoc = CMDLINEGETARG(pCmd, "loc");
            //


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
                font-family: Verdana, Aral;
            }
            th {
                font-weight:bold
            }

            tr:hover td {
            background-color: #BCCFD6;
            }

</style>
 <body style=''>
");


            switch (repLoc)
            {





                case "materialtrans":
                    {


                        var clLRef = CASTASINT(PARSEDESERIALIZE(CMDLINEGETARG(pCmd, "filter_cl")));
                        var matLRef = CASTASINT(PARSEDESERIALIZE(CMDLINEGETARG(pCmd, "filter_mat")));

                        var clDesc = "";
                        var matDesc = "";

                        var filterByMaterial = !ISEMPTYLREF(matLRef);
                        var filterByCl = !ISEMPTYLREF(clLRef);


                        if (filterByCl)
                            clDesc = CASTASSTRING(MY_SQLSCALAR("SELECT DEFINITION_ FROM LG_$FIRM$_CLCARD WITH(NOLOCK) WHERE LOGICALREF = @P1", new object[] { clLRef }));
                        if (filterByMaterial)
                            matDesc = CASTASSTRING(MY_SQLSCALAR("SELECT NAME FROM LG_$FIRM$_ITEMS WITH(NOLOCK) WHERE LOGICALREF = @P1", new object[] { matLRef }));



                        //


                        var dateTo = CASTASDATE(MY_SQLSCALAR(@"
--$MS$--select getdate()
--$PG$--select now()
",null)).Date;
                        var dateFrom = dateTo.AddDays(-3 * 30);

                        //



                        var sql = @"
 

SELECT CANCELLED,
	TRCODE,
	STOCKREF,
	(
		SELECT 
            --$MS$--I.CODE + '/' + I.NAME
            --$PG$--I.CODE || '/' || I.NAME
		FROM LG_$FIRM$_ITEMS I 
		WHERE I.LOGICALREF = T.STOCKREF
		) ITEMS_DESC,

 	(
		SELECT DEFINITION_ FROM LG_$FIRM$_CLCARD WHERE LOGICALREF = T.CLIENTREF
		) CLCARD_DESC,
	IOCODE,
    AMOUNT,
	(case when AMOUNT>0 then (VATMATRAH+DISTDISC)/AMOUNT else 0 end) PRICE,
    (case when (VATMATRAH+DISTDISC)>0 then 100.0*DISTDISC/(VATMATRAH+DISTDISC) else 0 end) DISCPER,
    (case when (VATMATRAH)>0 then 100.0*VATAMNT/(VATMATRAH) else 0 end) VAT,
    (VATMATRAH+VATAMNT) TOTAL,

	DATE_,
	FTIME,
    DESTINDEX,
    SOURCEINDEX

FROM LG_$FIRM$_$PERIOD$_STLINE T WITH(NOLOCK)
WHERE 
LINETYPE IN (0) AND DATE_ BETWEEN @P1 AND @P2
";
    var filterValues = new List<object>(new object[] { dateFrom, dateTo });


if(filterByMaterial){
sql+=  (" AND STOCKREF = @P"+(filterValues.Count+1)+" ") ;
    filterValues.Add(matLRef);
}

if(filterByCl){
sql+=  (" AND CLIENTREF = @P"+(filterValues.Count+1)+" ") ;
    filterValues.Add(clLRef);
}


 

sql+=  @"
ORDER BY LOGICALREF DESC
 
";
                    
                        
                     

                        var data = MY_SQL(sql, filterValues.ToArray());

                        TAB_FILLNULL(data);

                        //name
                        res.AppendLine(string.Format("<h2 style='width:100%;text-align:center'>{0} </h2>",
HTMLENCODE(TEXT.L.REPORT_NAME)
));



                        //filter
                        {

                            var lines = new List<string[]>();


                            if (!ISEMPTY(clDesc))
                                lines.Add(new string[] { TEXT.L.CLIENT, HTMLENCODE((clDesc)) });
                            if (!ISEMPTY(matDesc))
                                lines.Add(new string[] { TEXT.L.MATERIAL, HTMLENCODE((matDesc)) });

                            lines.Add(new string[] { TEXT.L.DATE, FORMAT(dateFrom, "yyyy-MM-dd") + " " + FORMAT(dateTo, "yyyy-MM-dd") });

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
                            res.AppendLine("<table style='width:100%'>");

                            res.AppendLine("<tr>");

                            var arrHeaders = new string[] { TEXT.L.QUANTITY, TEXT.L.PRICE, TEXT.L.DISCOUNT, TEXT.L.VAT, TEXT.L.TOTAL };

                            foreach (var cell in arrHeaders)
                            {
                                res.AppendLine(string.Format(
                                "<th>{0}</th>",
                                 cell
                               ));
                            }
                            res.AppendLine("</tr>");

                            var indx = 0;
                            //
                            foreach (DataRow row in data.Rows)
                            {
                                ++indx;
                                //

                                var dic = new Dictionary<string, string>();


                                var iocode = CASTASSHORT(TAB_GETROW(row, "IOCODE"));
                                var trcode = CASTASSHORT(TAB_GETROW(row, "TRCODE"));

                                var descMat = CASTASSTRING(TAB_GETROW(row, "ITEMS_DESC"));
                                var descCl = CASTASSTRING(TAB_GETROW(row, "CLCARD_DESC"));
                                var amount = ROUND(CASTASDOUBLE(TAB_GETROW(row, "AMOUNT")), 2);
                                var price = ROUND(CASTASDOUBLE(TAB_GETROW(row, "PRICE")), 2);
                                var disc = ROUND(CASTASDOUBLE(TAB_GETROW(row, "DISCPER")), 2);
                                var vat = ROUND(CASTASDOUBLE(TAB_GETROW(row, "VAT")), 2);
                                var total = ROUND(CASTASDOUBLE(TAB_GETROW(row, "TOTAL")), 2);






                                dic["AMOUNT"] = "<span style='color:#388E3C'>" + FORMAT(amount, "#,##0.##") + "</span>";
                                dic["PRICE"] = FORMAT(price, "N2");
                                dic["DISCPER"] = FORMAT(disc, "#,##0.##") + " %";
                                dic["VAT"] = FORMAT(vat, "#,##0.##") + " %";
                                dic["TOTAL"] = FORMAT(total, "N2");



                                var backColor = "";

                                //if (indx % 2 == 1)
                                //    backColor = "#F2F2F2";



                               
                                {


                                    var date = CASTASDATE(TAB_GETROW(row, "DATE_"));
                                    var timeDate = GETINTTIMETOTIME(CASTASINT(TAB_GETROW(row, "FTIME")));
                                    date = new DateTime(date.Year, date.Month, date.Day, timeDate.Hour, timeDate.Minute, timeDate.Second);

                                    var docDesc = FORMAT(trcode);
                                   
                                    switch (trcode)
                                    {
                                        case 1:
                                            docDesc = TEXT.L.OPTYPE_PURCH;
                                            break;
                                        case 3:
                                            docDesc = TEXT.L.OPTYPE_SALER;
                                            break;
                                        case 6:
                                            docDesc = TEXT.L.OPTYPE_PURCHR;
                                            break;
                                        case 8:
                                            docDesc = TEXT.L.OPTYPE_SALE;
                                            break;


                                    }




                                    res.AppendLine(string.Format("<tr style='color:" + "#0288D1" + "'><td colspan='" + arrHeaders.Length + "'>{0}, {1}</td></tr>",
                                  HTMLENCODE(FORMAT(date, "yyyy-MM-dd HH:mm")), HTMLENCODE(docDesc)
                                       ));

                                }



                                if (!filterByMaterial)
                                {
                                    res.AppendLine(string.Format("<tr style='color:" + "#0288D1" + "'><td colspan='" + arrHeaders.Length + "'>{0}</td></tr>",
                                  HTMLENCODE(descMat)
                                       ));

                                }


                                if (!filterByCl)
                                {
                                    res.AppendLine(string.Format("<tr style='color:" + "#0288D1" + "'><td colspan='" + arrHeaders.Length + "'>{0}</td></tr>",
                                  HTMLENCODE(descCl)
                                       ));

                                }


                                var foreColor = "";
                                var fontWeight = "";

                                res.AppendLine(
                                       "<tr style='" +
                                       (ISEMPTY(backColor) ? "" : "background-color:" + backColor + ";") +
                                        (ISEMPTY(foreColor) ? "" : "color:" + foreColor + ";") +
                                       (ISEMPTY(fontWeight) ? "" : "font-weight:" + fontWeight + ";") +
                                       "'>");


                                foreach (var key in dic)
                                {
                                    var alignNum = false;

                                    switch (key.Key)
                                    {
                                        case "AMOUNT":
                                        case "PRICE":
                                        case "DISCPER":
                                        case "VAT":
                                        case "TOTAL":
                                            alignNum = true;
                                            break;
                                    }

                                    res.AppendLine(string.Format(
                                        "<td style='" + ((!alignNum) ? "" : " text-align: right;") + "'>{0}</td>",
                                     key.Value
                                   ));
                                }
                                res.AppendLine("</tr>");




                            }

                            res.AppendLine("</table>");
                        }
                    }
                    break;
                default: //unhandled report
                    return null;
            }

            res.AppendLine("</body></html>");

            return res.ToString();
        }






        #endregion


        class TEXT
        {
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

            public string GET(string pCode)
            {

                var f = this.GetType().GetField(pCode);
                if (f != null)
                    return f.GetValue(this) as string;

                return pCode;
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



            public string REPORT_NAME = "Material Transactions";

            public string AMOUNT = "Amount";
            public string QUANTITY = "Quantity";
            public string PRICE = "Price";
            public string DISCOUNT = "Disc";
            public string VAT = "VAT";
            public string TOTAL = "Total";


            public string DEBIT = "Debit";
            public string CREDIT = "Credit";

            public string OPTYPE = "Type";



            public string OPTYPE_1 = "Collection";
            public string OPTYPE_2 = "Payment";
            public string OPTYPE_3 = "DebitDirect";
            public string OPTYPE_4 = "CreditDirect";
            public string OPTYPE_5 = "Remmitance";

            public string OPTYPE_14 = "Opening";
            public string OPTYPE_20 = "ToBankAcc";
            public string OPTYPE_21 = "FromBankAcc";
            public string OPTYPE_31 = "Purchase";
            public string OPTYPE_33 = "SaleReturn";
            public string OPTYPE_36 = "PurchaseReturn";
            public string OPTYPE_38 = "Sale";




            public string OPTYPE_SALE = "Sale";
            public string OPTYPE_PURCH = "Purchase";
            public string OPTYPE_SALER = "Sale Return";
            public string OPTYPE_PURCHR = "Purch Return";
            public string OPTYPE_CASHIN = "Cash In";
            public string OPTYPE_CASHOUT = "Cash Out";
            public string OPTYPE_UNDEF = "---";

            public string DATE = "Date";
            public string CLIENT = "Client";
            public string MATERIAL = "Material";

            public string WH = "Warehouse";
            public string GROSSTOTAL = "Total Gross";
            public string TOTALDISCOUNTS = "Total Discount";
            public string TOTALVAT = "Total VAT";
            public string NETTOTAL = "Total Net";


        }




        DataTable MY_SQL(string pSql, object[] pArgs)
        {

            pSql = pSql.
                Replace("$FIRM$", FORMAT(_SETTINGS.BUF._FIRM).PadLeft(3, '0')).
                Replace("$PERIOD$", FORMAT(_SETTINGS.BUF._FIRM).PadLeft(2, '0'));

            return XSQL(pSql, pArgs);
        }
        object MY_SQLSCALAR(string pSql, object[] pArgs)
        {
            pSql = pSql.
            Replace("$FIRM$", FORMAT(_SETTINGS.BUF._FIRM).PadLeft(3, '0')).
            Replace("$PERIOD$", FORMAT(_SETTINGS.BUF._FIRM).PadLeft(2, '0'));

            return XSQLSCALAR(pSql, pArgs);
        }


        #endregion
