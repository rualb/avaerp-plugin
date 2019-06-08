#line 2
 
 
   #region BODY
        //BEGIN

        const int VERSION = 5;
        const string FILE = "plugin.sys.event.stockdoclineprofit.pls";

        const string event_MAT_LINE_PROFIT_SLS = "hadlericom_mat_line_profit_sls";



        public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
        {




            object arg1 = ARGS.Length > 0 ? ARGS[0] : null;
            object arg2 = ARGS.Length > 1 ? ARGS[1] : null;
            object arg3 = ARGS.Length > 2 ? ARGS[2] : null;

            string[] list_ = EXPLODELISTPATH(EVENTCODE);

            switch (list_.Length > 0 ? list_[0] : "")
            {

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


            var isSls = fn.StartsWith("adp.sls.doc.inv");

            var isSlsOrder = fn.StartsWith("adp.sls.doc.order");



            if (isSls || isSlsOrder)
            {

                var cPanelBtnSub = CONTROL_SEARCH(FORM, "cPanelBtnSub");

                if (cPanelBtnSub == null)
                    return;



                if (isSls)
                    _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_MAT_LINE_PROFIT_SLS, LANG("T_PROFIT"), "money_32x32");

                if (isSlsOrder)
                    _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(cPanelBtnSub, event_MAT_LINE_PROFIT_SLS, LANG("T_PROFIT"), "money_32x32");

            }






        }
        void _MY_SYS_NEWFORM_INTEGRATE_ADD_BTN(Control pCtrl, string pEvent, string pText, string pImg = null)
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
			{ "ImageName" ,"right_16x16"},
			{ "Width" ,80},
            };

                //   RUNUIINTEGRATION(pCtrl, args);

                var b = RUNUIINTEGRATION(pCtrl, args) as Button;
                if (b != null)
                {
                    b.AutoSize = true;
                    var w = (Math.Max(80, b.Width + (12 + 16 + 12)) / 20) * 20;
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



                    case event_MAT_LINE_PROFIT_SLS:
                        {
                            var f = arg1 as Form;
                            if (f != null) // if (ISADAPTERFORM(f))
                            {
                                var ds_ = GETDATASETFROMADPFORM(f);

                                if (ds_ != null)
                                {
                                    var header = ds_.Tables["INVOICE"];
                                    if (header == null)
                                        header = ds_.Tables["STFICHE"];//order

                                    var lines = ds_.Tables["STLINE"];
                                    if (lines != null && header != null)
                                    {

                                        MY_LINE_PROFIT_WIN_HTML(header, lines, cmd);

                                    }

                                }


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


        void MY_LINE_PROFIT_WIN_HTML(DataTable pHeader, DataTable pLines, string pCmd)
        {
            if (pHeader == null || pLines == null)
                return;



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
                font-weight:bold
            }

         tr:hover td {
            background-color: #BCCFD6;
            }

</style>
 <body style=''>
");


            switch (pCmd)
            {
                case event_MAT_LINE_PROFIT_SLS:
                    {

                        var isOrder = pHeader.TableName == "STFICHE";



                        var date = CASTASDATE(TAB_GETROW(pHeader, "DATE_"));
                        var clDesc = CASTASSTRING(TAB_GETROW(pHeader, "CLCARD_____DEFINITION_"));
                        var trcode = CASTASSHORT(TAB_GETROW(pHeader, "TRCODE"));


                        var trcodeDesc = RESOLVESTR("[lang::" + (isOrder ? "T_DOC_STOCK_ORDER_" : "T_DOC_STOCK_INV_") + FORMAT(trcode) + "]");

                        var sign = +1;

                        if (isOrder)
                        {
                            switch (trcode)
                            {
                                case 1:

                                    break;
                                default:
                                    return;

                            }

                        }
                        else
                        {

                            switch (trcode)
                            {
                                case 8:
                                case 3:
                                    sign = -1;
                                    break;
                                case 2:
                                    sign = -1;
                                    break;
                                case 7:
                                case 9:

                                    break;
                                default:
                                    return;

                            }


                        }




                        //name
                        res.AppendLine(string.Format("<h2 style='width:100%;text-align:center'>{0}</h2>",
                           HTMLENCODE(LANG("T_PROFIT"))
                              ));


                        //filter
                        {

                            var lines = new List<string[]>();

                            lines.Add(new string[] { LANG("T_TYPE"), HTMLENCODE(trcodeDesc) });
                            lines.Add(new string[] { LANG("T_DATE"), FORMAT(date, "yyyy-MM-dd") });
                            lines.Add(new string[] { LANG("T_PERSONAL"), HTMLENCODE(clDesc) });



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

                            var headers = new string[] { 
                                "", 
                                LANG("T_MATERIAL"),
                                LANG("T_QUANTITY"),
                                LANG("T_PRICE"),
                                LANG("T_TOTAL"),
                                LANG("T_COST<br/>T_PRICE"),
                                LANG("T_COST<br/>T_TOTAL"),
                                LANG("T_PROFIT"),
                            
                            };

                            foreach (var cell in headers)
                            {
                                res.AppendLine(string.Format(
                                "<th>{0}</th>",
                                 cell
                               ));
                            }
                            res.AppendLine("</tr>");


                            var totProfit = 0.0;
                            //
                            var indx = 0;
                            var indxVisible = 0;
                            for (int i = 0; i <= pLines.Rows.Count; ++i)
                            {
                                ++indx;
                                var row = i < pLines.Rows.Count ? pLines.Rows[i] : null;
                                //
                                var isDark = false;
                                var isBold = false;

                                string[] arrCell = null;

                                var tfooter = false;


                                var opt = new Dictionary<string, string>();

                                if (row != null)
                                {
                                    var lineType = CASTASSHORT(TAB_GETROW(row, "LINETYPE"));

                                    if (lineType != 0)
                                        continue;

                                    var metRef = (TAB_GETROW(row, "STOCKREF"));

                                    if (ISEMPTYLREF(metRef))
                                        continue;

                                    ++indxVisible;

                                    var amnt = ROUND(CASTASDOUBLE(TAB_GETROW(row, "AMOUNT")), 2);

                                    var costPrice = MY_GET_PRCH_AVG_IN(metRef, date);
                                    var costTotal = costPrice * amnt;

                                    var t1 = CASTASDOUBLE(TAB_GETROW(row, "VATMATRAH"));
                                    var t2 = CASTASDOUBLE(TAB_GETROW(row, "VATAMNT"));
                                    var t3 = CASTASDOUBLE(TAB_GETROW(row, "DISTEXP"));

                                    var total = t1 + t2 + t3;

                                    var price = ISNUMZERO(amnt) ? 0.0 : total / amnt;

                                    var profit = sign * (total - costTotal);

                                    totProfit += profit;

                                    var matDesc = CASTASSTRING(TAB_GETROW(row, "ITEMS_____NAME"));




                                    arrCell = new string[] { 
                                    FORMAT(indx),
                                    FORMAT(matDesc),  
                                    FORMAT(amnt, "0.#"), 
                                    FORMAT(price, "0.00"), 
                                    FORMAT(total, "0.00"), 

                                    FORMAT(costPrice, "0.00"), //5
                                    FORMAT(costTotal, "0.00"), //6

                                    FORMAT(profit, "0.00"), //7


                            };
                                    
                            
                                    var isErr = ISNUMZERO(amnt) || ISNUMZERO(price) || ISNUMZERO(costPrice)   ;

                                    opt["5.color"] =  "blue";
                                    opt["6.color"] =   "blue";
                                    opt["7.color"] = isErr  ? "red" : "indigo";

                                    opt["7.font-weight"] = "bold";

                                    opt["0.text-align"] = "left";
                                    opt["1.text-align"] = "left";

                                }
                                else
                                {

                                    tfooter = true;

                                    isDark = true;
                                    isBold = true;

                                    arrCell = new string[] { 
                             "",
                              "", "", "",  "", "",   


                                FORMAT( LANG("T_TOTAL") ), 
                                FORMAT(totProfit, "0.00"), 

                            };

                                }


                                var backColor = "#FFFFFF";

                                if (indxVisible % 2 == 1)
                                    backColor = "#F2F2F2";

                                if (isDark)
                                    backColor = "#B0B0B0";


                                var fontWeight = isBold ? "bold" : "normal";

                                res.AppendLine("<tr style='background-color:" + backColor + ";font-weight:" + fontWeight + "'>");
                                for (int c = 0; c < arrCell.Length; ++c)
                                {
                                

                                    var cellVal = arrCell[c];

                                    var td_color = "";
                                    var td_text_align = "";
                                    var td_font_weight = "";
                                  
                                     opt.TryGetValue(c+".color", out  td_color);
                                     opt.TryGetValue(c + ".text-align", out  td_text_align);
                                     opt.TryGetValue(c + ".font-weight", out  td_font_weight);

                                    res.AppendLine(string.Format(
                                        "<td style='color:" + (td_color ?? "") + ";text-align:" + (ISEMPTY(td_text_align) ? "right" : td_text_align) + ";font-weight:" + (td_font_weight??"") + "'>{0}</td>",
                                     cellVal
                                   ));
                                }
                                res.AppendLine("</tr>");




                            }

                            res.AppendLine("</table>");
                        }
                    }
                    break;
            }

            res.AppendLine("</body></html>");

            MSGUSERINFO(res.ToString());





        }


        double MY_GET_PRCH_AVG_IN(object pMatRef, DateTime pDt)
        {


            return CASTASDOUBLE(SQLSCALAR(@"
  
		
		SELECT SUM(TOTAL)/SUM(AMOUNT) FROM
		(
		SELECT 
		--$MS$--TOP 3
		AMOUNT AMOUNT,
		(VATMATRAH+VATAMNT+DISTEXP) TOTAL
		FROM LG_$FIRM$_$PERIOD$_STLINE 
		WHERE (
				STOCKREF = @P1 AND 
				VARIANTREF = 0 AND DATE_ <= @P2 AND FTIME < 987654321 AND IOCODE = 1 AND 
				SOURCEINDEX >= 0
				) AND (
				CANCELLED = 0 AND TRCODE IN ( 1,13,14) AND (AMOUNT > 0) AND LINETYPE IN (0)
				) 
		ORDER BY STOCKREF DESC,
			VARIANTREF DESC,
			DATE_ DESC,
			FTIME DESC,
			IOCODE DESC,
			SOURCEINDEX DESC,
			LOGICALREF DESC
		--$PG$--LIMIT 3
		--$SL$--LIMIT 3
		) T 
		 
	
	
	 

", new object[] { pMatRef, pDt.Date }));

        }

        //END


        enum DataType
        {
            sale,
            prchs,
            saleByCl,
            prchsByCl



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
