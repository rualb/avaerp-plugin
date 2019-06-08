#line 2

    #region BODY
        //BEGIN

        const int VERSION = 7;
        const string FILE = "plugin.sys.event.simplebalance.pls";

        const string event_SIMPLEBALANCE_ = "hadlericom_simplebalance_";
        const string event_SIMPLEBALANCE_DO = "hadlericom_simplebalance_do";


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
			//{ "CmdText" ,"event name::"+event_SIMPLEBALANCE_},
			{ "Text" ,TEXT.L.BALANCE},
			{ "ImageName" ,"battery_32x32"},
			{ "Name" ,event_SIMPLEBALANCE_},
            };

                        RUNUIINTEGRATION(tree, args);

                    }

                    {
                        var args = new Dictionary<string, object>() { 
 
			{ "_cmd" ,""},
            { "_type" ,""},
            { "_root" ,event_SIMPLEBALANCE_},
			{ "CmdText" ,"event name::"+event_SIMPLEBALANCE_DO},
			{ "Text" ,TEXT.L.FIN_STATUS},
			{ "ImageName" ,"money_32x32"},
			{ "Name" ,event_SIMPLEBALANCE_DO},
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

                    case event_SIMPLEBALANCE_DO:
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
                case event_SIMPLEBALANCE_DO:
                    #region SIMPLEBALANCE
                    {
                        var now = DateTime.Now.Date;

                        DateTime df = new DateTime(now.Year, now.Month, 1);
                        DateTime dt = df.AddMonths(+1).AddDays(-1);

                        if (!MY_ASKDATETIME(this, "T_DATE_RANGE", ref df, ref dt))
                            return;


                        df = new DateTime(df.Year, df.Month, 1);
                        dt = new DateTime(dt.Year, dt.Month, 1).AddMonths(+1).AddDays(-1);


                        var listData = new List<DataTable>();


                        listData.Add(TOTALS.GET_CASH_IO(this, df, dt));

                        listData.Add(TOTALS.GET_BANK_IO_LOC(this, df, dt));
  
                        //listData.Add(TOTALS.GET_CASH_OPER_BY_SPECODE(this, df, dt));

                        listData.Add(TOTALS.GET_FIN_OPER_BY_SPECODE(this, df, dt));
                        
                        listData.Add(TOTALS.GET_CLIENT_IO(this, df, dt));

                        listData.Add(TOTALS.GET_WH_OPER(this, df, dt));

                        listData.Add(TOTALS.GET_PRCH_SLS_OPER_ALL(this, df, dt));

                        listData.Add(TOTALS.GET_MAT_REM(this, df, dt));

                        listData.Add(TOTALS.GET_PRCH_SLS_OPER_MAT(this, df, dt));

                        listData.Add(TOTALS.GET_MAT_PROFIT(this, df, dt));


                        var values = new Dictionary<string, double>();

                        foreach (var tabData in listData)
                        {
                            foreach (DataColumn tabCol in tabData.Columns)
                            {
                                values[tabCol.ColumnName] = ROUND(CASTASDOUBLE(TAB_GETROW(tabData, tabCol.ColumnName)), 2);
                            }

                        }



                        values["VAL_MATTOT_PRCHNET"] =
                             values["VAL_MATTOT_PRCH"] - values["VAL_MATTOT_PRCHRET"];

                        values["VAL_MATTOT_SLSNET"] =
                                values["VAL_MATTOT_SLS"] - values["VAL_MATTOT_SLSRET"];


                        values["VAL_MATTOT_PROFIT_BY_PRCH"] =
                          values["VAL_MATTOT_SLSNET"] - values["VAL_MATTOT_SLS_COST_BY_PRCH"];


                        values["VAL_EXPENSE_TOT"] =
                            values["VAL_ACC728"] + values["VAL_ACC733"] + values["VAL_ACC735"] + values["VAL_ACC738"] + values["VAL_ACC742"] + values["VAL_ACC747"];

                        values["VAL_PROFIT_GROSS"] =
                                values["VAL_MATTOT_PROFIT_BY_PRCH"] + values["VAL_ACC642"] + values["VAL_ACC655"];


                        values["VAL_PROFIT_NET"] =
                              values["VAL_PROFIT_GROSS"] - values["VAL_EXPENSE_TOT"];


                        var keysOrder = new List<string>(new string[]{
                         LANG("T_CASH/T_BANK"),
                        "VAL_CASHEND",
                        "VAL_BANKEND",
                        "VAL_ACC411",
                        "VAL_ACC534",

                         LANG("T_PERSONAL"),
                        "VAL_CLCDEBIT",
                        "VAL_CLCCREDIT",
                        "VAL_CLVDEBIT",
                        "VAL_CLVCREDIT",
                       

                         LANG("T_MATERIAL"),
                        "VAL_MATREM_PRCHPRC",
                        "VAL_MATREM_SLSPRC",
                        "VAL_MATTOT_PRCHNET",  
                        "VAL_MATTOT_PRCHRET",
                        "VAL_MATTOT_SLSNET", 
                        "VAL_MATTOT_SLSRET",
                        "VAL_MATTOT_COUNT50",
                        "VAL_MATTOT_COUNT51",
                         
                        LANG("T_EXPENSE"),
                        "VAL_ACC728",
                        "VAL_ACC733",
                        "VAL_ACC735",
                        "VAL_ACC738",
                        "VAL_ACC742",
                        "VAL_ACC747",
                        "VAL_EXPENSE_TOT,[d]",
                       
                        LANG("T_PROFIT"),
                        "VAL_MATTOT_PROFIT_BY_PRCH",
                        "VAL_ACC642",
                        "VAL_ACC655",
                        "VAL_PROFIT_GROSS,[d]",
                        "VAL_PROFIT_NET,[b]", 
                       
                        
                        
                        });


                        //name
                        res.AppendLine(string.Format("<h2 style='width:100%;text-align:center'>{0}</h2>",
                           HTMLENCODE(TEXT.L.FIN_STATUS)
                              ));


                        //filter
                        {

                            var lines = new List<string[]>();

                            lines.Add(new string[] { LANG("T_DATE_FROM"), FORMAT(df, "yyyy-MM-dd") });
                            lines.Add(new string[] { LANG("T_DATE_TO"), FORMAT(dt, "yyyy-MM-dd") });

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
                            res.AppendLine("<table style=';'>");

                            res.AppendLine("<tr>");
                            foreach (var cell in new string[] { 
                                LANG("T_NAME"), 
                                LANG("T_VALUE (T_SYS_CURR1)"),
                             
                            })
                            {
                                res.AppendLine(string.Format(
                                "<th>{0}</th>",
                                 cell
                               ));
                            }
                            res.AppendLine("</tr>");




                            foreach (string key in keysOrder)
                            {
                                var isVal = key.StartsWith("VAL_");

                                var valParts = new List<string>(EXPLODELIST(key));

                                var val = isVal ? values[valParts[0]] : 0;

                                var desc = isVal ? TEXT.L.GET(valParts[0]) : key;

                                if (isVal)
                                    if (valParts[0].StartsWith("VAL_ACC") && valParts[0].Length == 10)
                                        desc = "(" + (RIGHT(valParts[0], 3)) + ") " + desc;


                                var isDark = isVal && valParts.Contains("[d]") ? true : false;
                                var isBold = !isVal || valParts.Contains("[b]") ? true : false;

                                string[] arrCell = null;


                                arrCell = new string[] { 
                                        desc,
                                        FORMAT(val,"N2"),
   
                                        };

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

                                    if (!isVal)
                                    {
                                        if (c > 0)
                                            break;

                                    }

                                    var cellVal = arrCell[c];
                                    res.AppendLine(string.Format(
                                        "<td colspan='" + (!isVal ? arrCell.Length : 0) + "' style='background-color:" + (c == 2 ? "#74ba5d" : "") + ";text-align: " + (c >= 1 ? "right" : "") + ";'>{0}</td>",
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





            }

            res.AppendLine("</body></html>");

            MSGUSERINFO(res.ToString());





        }



        //END



        #region CLAZZ

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

                BALANCE = "Mali Durum";
                FIN_STATUS = "Mali Durum Özet Raporu ";

                VAL_CASHBEG = "Kasa Başlangıç Bakiyesi";
                VAL_CASHIN = "Kasa Girişleri";
                VAL_CASHOUT = "Kasa Çıkışları";
                VAL_CASHEND = "Kasa Bakiyesi";

                VAL_BANKBEG = "Banka Başlangıç Bakiyesi";
                VAL_BANKIN = "Banka Girişleri";
                VAL_BANKOUT = "Banka Çıkışları";
                VAL_BANKEND = "Banka Bakiyesi";

                VAL_CLVDEBIT = "Borçlu Tedarikçiler";
                VAL_CLVCREDIT = "Alacaklı tedarikçiler";
                VAL_CLVIN = "Teadarikçilere Borç Artışı";
                VAL_CLVOUT = "Teadarikçilere Borç Azalışı";

                VAL_CLCDEBIT = "Alıcı Borcu";
                VAL_CLCCREDIT = "Alıcılara Borç";
                VAL_CLCIN = "Alıcıların Borç Artışı";
                VAL_CLCOUT = "Alıcıların Borc Azalışı";

                VAL_MATREM_SLSPRC = "Stok Durumu Satış Fiyatına Göre";
                VAL_MATREM_PRCHPRC = "Stok Durumu Alış Fiyatına Göre";
                VAL_MATREM_PROFIT = "Stok Durumu(Satış-Alış) Fark";

                VAL_MATTOT_PRCH = "Bürüt Alış";
                VAL_MATTOT_PRCHRET = "Alış İadesi";
                VAL_MATTOT_PRCHNET = "Alış Net";

                VAL_MATTOT_SLS = "Bürüt Satış";
                VAL_MATTOT_SLSRET = "Satış İade";
                VAL_MATTOT_SLSNET = "Satış Net";

                VAL_MATTOT_COUNT50 = "Sayım Fazlası";
                VAL_MATTOT_COUNT51 = "Sayım Eksiği";


                VAL_MATTOT_PRCH_TOT = "Mal Alışı";

                VAL_MATTOT_SLS_TOT = "Mal Satışı";

                VAL_MATTOT_SLS_COST = "Satılan Malın Maliyeti";
                VAL_MATTOT_SLS_COST_BY_PRCH = "Satılan Malın Maliyeti Alış Fiyatına Göre";
                VAL_MATTOT_PROFIT_BY_PRCH = "Kar Alış Fiyatına Göre";


                VAL_ACC411 = "Yatırımlar";
                VAL_ACC521 = "Tedarikçilere Ödemeler";
                VAL_ACC534 = "Ortaklara ödemeler";
                VAL_ACC642 = "Kira Gelirleri";
                VAL_ACC655 = "Diğer Gelirler";
                VAL_ACC728 = "Diğer Ödemeler";
                VAL_ACC733 = "Maaş Ödemeleri";
                VAL_ACC735 = "Vergi Giderleri";
                VAL_ACC738 = "Ofis Giderleri";
                VAL_ACC742 = "Kira Giderleri";
                VAL_ACC747 = "Zarar";

                VAL_EXPENSE_TOT = "Toplam Giderler";
                VAL_PROFIT_GROSS = "Bürüt Kar";
                VAL_PROFIT_NET = "Net Kar";
            }


            public void lang_en()
            {

                BALANCE = "Financial Status";
                FIN_STATUS = "Financial Status Simple";
                VAL_CASHBEG = "Cash Begin";
                VAL_CASHIN = "Cash In";
                VAL_CASHOUT = "Cash Out";
                VAL_CASHEND = "Cash End";

                VAL_BANKBEG = "Bank Begin";
                VAL_BANKIN = "Bank In";
                VAL_BANKOUT = "Bank Out";
                VAL_BANKEND = "Bank End";


                VAL_CLVDEBIT = "Vendor Debit";
                VAL_CLVCREDIT = "Vendor Credit";
                VAL_CLVIN = "Vendor Debt Decrement";
                VAL_CLVOUT = "Vendor Debt Increment";

                VAL_CLCDEBIT = "Customer Debit";
                VAL_CLCCREDIT = "Customer Credit";
                VAL_CLCIN = "Customer Debt Increment";
                VAL_CLCOUT = "Customer Debt Decrement";

                VAL_MATREM_SLSPRC = "Mat Remain By Sale Price";
                VAL_MATREM_PRCHPRC = "Mat Remain By Purchase Price";
                VAL_MATREM_PROFIT = "Mat Remain Include Profit";

                VAL_MATTOT_PRCH = "Mat Purchase Gross";
                VAL_MATTOT_PRCHRET = "Mat Purchase Return";
                VAL_MATTOT_PRCHNET = "Mat Purchase Net";

                VAL_MATTOT_SLS = "Mat Sale Gross";
                VAL_MATTOT_SLSRET = "Mat Sale Return";
                VAL_MATTOT_SLSNET = "Mat Sale Net";

                VAL_MATTOT_COUNT50 = "Mat Inventory Increment";
                VAL_MATTOT_COUNT51 = "Mat Inventory Decrement";

                VAL_MATTOT_PRCH_TOT = "Mat Purchase";
                VAL_MATTOT_SLS_TOT = "Mat Sales";
                VAL_MATTOT_SLS_COST = "Cost of Mat Sales";
                VAL_MATTOT_SLS_COST_BY_PRCH = "Cost of Mat Sales By Purchase";
                VAL_MATTOT_PROFIT_BY_PRCH = "Profit from Mat Sales By Purchase";


                VAL_ACC411 = "Investments";
                VAL_ACC521 = "Payments To Vendor";
                VAL_ACC534 = "Payments To Shareholders";
                VAL_ACC642 = "Rental Income Revenue";
                VAL_ACC655 = "Service Income Revenue";
                VAL_ACC728 = "Operating Expenses";
                VAL_ACC733 = "Salaries Expenses";
                VAL_ACC735 = "Taxes And Fees";
                VAL_ACC738 = "Office Expenses";
                VAL_ACC742 = "Leasing Expenses";
                VAL_ACC747 = "Losses";

                VAL_EXPENSE_TOT = "Expenses Total";
                VAL_PROFIT_GROSS = "Profit Gross";
                VAL_PROFIT_NET = "Profit Net";
            }

            public void lang_az()
            {

                BALANCE = "Maliyyə Vəziyyəti";
                FIN_STATUS = "Maliyyə Vəziyyəti Sadə";
                VAL_CASHBEG = "Kassa İlk Qalıq";
                VAL_CASHIN = "Kassaya Qələn Pul";
                VAL_CASHOUT = "Kassadan Qedən Pun";
                VAL_CASHEND = "Kassa Son Qalıq";

                VAL_BANKBEG = "Bank İlk Qalıq";
                VAL_BANKIN = "Bank Qələn Pul";
                VAL_BANKOUT = "Bank Qedən Pun";
                VAL_BANKEND = "Bank Son Qalıq";


                VAL_CLVDEBIT = "Tedarukculərin Borcu";
                VAL_CLVCREDIT = "Tedarukculərə Borc";
                VAL_CLVIN = "Tedarukculərə Borc Azalması";
                VAL_CLVOUT = "Tedarukculərə Borc Coxalması";

                VAL_CLCDEBIT = "Alıcı Borcu";
                VAL_CLCCREDIT = "Alıcılara Borc";
                VAL_CLCIN = "Alıcıların Borc Coxalması";
                VAL_CLCOUT = "Alıcıların Borc Azalması";

                VAL_MATREM_SLSPRC = "Mal Qalıqı Satış Qiymətnən";
                VAL_MATREM_PRCHPRC = "Mal Qalıqı Alış Qiymətnən";
                VAL_MATREM_PROFIT = "Mal Qalıqında (Satış-Alış) Fərq";

                VAL_MATTOT_PRCH = "Mal Alışı Qros";
                VAL_MATTOT_PRCHRET = "Mal Alışın Qaytarılması";
                VAL_MATTOT_PRCHNET = "Mal Alışın Net";

                VAL_MATTOT_SLS = "Mal Satışı Qros";
                VAL_MATTOT_SLSRET = "Mal Satışı Qaytarılması";
                VAL_MATTOT_SLSNET = "Mal Satışı Net";

                VAL_MATTOT_COUNT50 = "Mal Sayım Artığı";
                VAL_MATTOT_COUNT51 = "Mal Sayım Əksiyi";

                VAL_MATTOT_PRCH_TOT = "Mat Alışı";
                VAL_MATTOT_SLS_TOT = "Mal Satışı";
                VAL_MATTOT_SLS_COST = "Satılan Malın Maliyyəti";
                VAL_MATTOT_SLS_COST_BY_PRCH = "Satılan Malın Maliyyəti Alış Əsasında";
                VAL_MATTOT_PROFIT_BY_PRCH = "Qazanc Alış Əsasında";


                VAL_ACC411 = "İnvestisiyalar";
                VAL_ACC521 = "Tedarükculərə Ödəmələr";
                VAL_ACC534 = "Sahibkəra Odəmə";
                VAL_ACC642 = "Arenda Qəlirləri";
                VAL_ACC655 = "Şirkətlərdən Əlavə Qəlirlər";
                VAL_ACC728 = "Diqər Hərclər";
                VAL_ACC733 = "Maaş Hərcləri";
                VAL_ACC735 = "Verqi Hərcləri";
                VAL_ACC738 = "Ofis Hərcləri";
                VAL_ACC742 = "Arenda Hərcləri";
                VAL_ACC747 = "İtkilər";


                VAL_EXPENSE_TOT = "Hərclər Сəmi";
                VAL_PROFIT_GROSS = "Mənfəət Gros";
                VAL_PROFIT_NET = "Mənfəət Təmiz";
            }



            public void lang_ru()
            {

                BALANCE = "Финансовое Состояние";
                FIN_STATUS = "Финансовое Состояние Упрощенное";

                VAL_CASHBEG = "Касса Начальная";
                VAL_CASHIN = "Касса Пришло";
                VAL_CASHOUT = "Касса Ушло";
                VAL_CASHEND = "Касса Конечная";

                VAL_BANKBEG = "Банк в Начале";
                VAL_BANKIN = "Банк Пришло";
                VAL_BANKOUT = "Банк Ушло";
                VAL_BANKEND = "Банк в Конце";

                VAL_CLVDEBIT = "Долг Поставщиков";
                VAL_CLVCREDIT = "Долг Поставщикам";
                VAL_CLVIN = "Долг Поставщикам Спустился";
                VAL_CLVOUT = "Долг Поставщикам Вырос";

                VAL_CLCDEBIT = "Долг Покупателей";
                VAL_CLCCREDIT = "Долг Покупателям";
                VAL_CLCIN = "Долг Покупателей Вырос";
                VAL_CLCOUT = "Долг Покупателям Спустился";

                VAL_MATREM_SLSPRC = "ТМЗ по Отпускной Цене";
                VAL_MATREM_PRCHPRC = "ТМЗ по Закупочной Цене";
                VAL_MATREM_PROFIT = "Ожидаемый доход в ТМЗ";

                VAL_MATTOT_PRCH = "Закупки Брутто";
                VAL_MATTOT_PRCHRET = "Возврат с Закупок";
                VAL_MATTOT_PRCHNET = "Закупки Нетто";

                VAL_MATTOT_SLS = "Продажи Брутто";
                VAL_MATTOT_SLSRET = "Возврат с Продаж";
                VAL_MATTOT_SLSNET = "Продажи Нетто";

                VAL_MATTOT_COUNT50 = "Инвентаризация Прибавила";
                VAL_MATTOT_COUNT51 = "Инвентаризация Убавила";

                VAL_MATTOT_PRCH_TOT = "Закупки Нетто";
                VAL_MATTOT_SLS_TOT = "Продажи Нетто";

                VAL_MATTOT_SLS_COST = "Себестоимость Продажи";
                VAL_MATTOT_SLS_COST_BY_PRCH = "Себестоимость Продажи на Основе Закупок";
                VAL_MATTOT_PROFIT_BY_PRCH = "Прибыль с Продаж на Основе Закупок";

                VAL_ACC411 = "Инвестиции";
                VAL_ACC521 = "Расчеты с поставщиками";
                VAL_ACC534 = "Расчеты с учредителями";
                VAL_ACC642 = "Доходы от текущей аренды";
                VAL_ACC655 = "Проценты полученные";
                VAL_ACC728 = "Прочие коммерческие расходы";
                VAL_ACC733 = "Зарплата персонала";
                VAL_ACC735 = "Налоги, сборы и платежи";
                VAL_ACC738 = "Офисные расходы";
                VAL_ACC742 = "Расходы по аренде";
                VAL_ACC747 = "Недостачи и потери";

                VAL_EXPENSE_TOT = "Cумма Расходов";
                VAL_PROFIT_GROSS = "Прибыль Брутто";
                VAL_PROFIT_NET = "Прибыль Нетто";
            }

            public string BALANCE;
            public string FIN_STATUS;

            public string VAL_CASHBEG;
            public string VAL_CASHIN;
            public string VAL_CASHOUT;
            public string VAL_CASHEND;

            public string VAL_BANKBEG;
            public string VAL_BANKIN;
            public string VAL_BANKOUT;
            public string VAL_BANKEND;


            public string VAL_CLVDEBIT;
            public string VAL_CLVCREDIT;
            public string VAL_CLVIN;
            public string VAL_CLVOUT;

            public string VAL_CLCDEBIT;
            public string VAL_CLCCREDIT;
            public string VAL_CLCIN;
            public string VAL_CLCOUT;

            public string VAL_MATREM_SLSPRC;
            public string VAL_MATREM_PRCHPRC;
            public string VAL_MATREM_PROFIT;

            public string VAL_MATTOT_PRCH;
            public string VAL_MATTOT_PRCHRET;
            public string VAL_MATTOT_PRCHNET;

            public string VAL_MATTOT_SLS;
            public string VAL_MATTOT_SLSRET;
            public string VAL_MATTOT_SLSNET;

            public string VAL_MATTOT_COUNT50;
            public string VAL_MATTOT_COUNT51;

            public string VAL_MATTOT_PRCH_TOT;
            public string VAL_MATTOT_SLS_TOT;
            public string VAL_MATTOT_SLS_COST;

            public string VAL_MATTOT_SLS_COST_BY_PRCH;
            public string VAL_MATTOT_PROFIT_BY_PRCH;


            public string VAL_ACC411;
            public string VAL_ACC521;
            public string VAL_ACC534;
            public string VAL_ACC642;
            public string VAL_ACC655;
            public string VAL_ACC728;
            public string VAL_ACC733;
            public string VAL_ACC735;
            public string VAL_ACC738;
            public string VAL_ACC742;
            public string VAL_ACC747;


            public string VAL_EXPENSE_TOT;
            public string VAL_PROFIT_GROSS;
            public string VAL_PROFIT_NET;

        }



        class TOTALS
        {

            public static DataTable GET_CASH_OPER_BY_SPECODE(_PLUGIN PLUGIN, DateTime pDf, DateTime pDt)
            {


                var res = PLUGIN.SQL(
@"
--@df P1
--@dt P2
--@cashCode P3





SELECT
sum(COALESCE((
				CASE WHEN TRCODE IN (11,41)
						AND SPECODE LIKE '642%' THEN AMOUNT ELSE 0 END
				), 0)) VAL_ACC642,
sum(COALESCE((
				CASE WHEN TRCODE IN (11,41)
						AND SPECODE LIKE '655%' THEN AMOUNT ELSE 0 END
				), 0)) VAL_ACC655,
sum(COALESCE((
				CASE WHEN TRCODE IN (11,41)
						AND SPECODE LIKE '411%' THEN AMOUNT ELSE 0 END
				), 0)) VAL_ACC411,
sum(COALESCE((
				CASE WHEN TRCODE IN (12,42)
						AND SPECODE LIKE '521%' THEN AMOUNT ELSE 0 END
				), 0)) VAL_ACC521,
sum(COALESCE((
				CASE WHEN TRCODE IN (12,42)
						AND SPECODE LIKE '534%' THEN AMOUNT ELSE 0 END
				), 0)) VAL_ACC534,
sum(COALESCE((
				CASE WHEN TRCODE IN (12,42)
						AND SPECODE LIKE '728%' THEN AMOUNT ELSE 0 END
				), 0)) VAL_ACC728,
sum(COALESCE((
				CASE WHEN TRCODE IN (12,42)
						AND SPECODE LIKE '733%' THEN AMOUNT ELSE 0 END
				), 0)) VAL_ACC733,
sum(COALESCE((
				CASE WHEN TRCODE IN (12,42)
						AND SPECODE LIKE '735%' THEN AMOUNT ELSE 0 END
				), 0)) VAL_ACC735,
sum(COALESCE((
				CASE WHEN TRCODE IN (12,42)
						AND SPECODE LIKE '738%' THEN AMOUNT ELSE 0 END
				), 0)) VAL_ACC738,
sum(COALESCE((
				CASE WHEN TRCODE IN (12,42)
						AND SPECODE LIKE '742%' THEN AMOUNT ELSE 0 END
				), 0)) VAL_ACC742,
sum(COALESCE((
				CASE WHEN TRCODE IN (12,42)
						AND SPECODE LIKE '747%' THEN AMOUNT ELSE 0 END
				), 0)) VAL_ACC747
FROM LG_$FIRM$_$PERIOD$_KSLINES 
--$MS$--WITH(NOLOCK)
WHERE CANCELLED = 0
	AND DATE_ BETWEEN @P1
		AND @P2
	AND CANCELLED = 0
	AND TRCODE IN (11, 12, 41,42)


"
, new object[] { pDf, pDt, "%" });

                res.TableName = "CASH_BY_TYPE";

                return res;
            }

            public static DataTable GET_CASH_IO(_PLUGIN PLUGIN, DateTime pDf, DateTime pDt)
            {


                var res = PLUGIN.SQL(
@"
--@df P1
--@dt P2
--@cashCode P3
SELECT 
 sum((CASE WHEN CH.DATE_ < @P1 THEN 1 ELSE 0 END) * (CH.DEBIT - CH.CREDIT)) VAL_CASHBEG,

 sum((
			CASE WHEN CH.DATE_ BETWEEN @P1
						AND @P2 THEN 1 ELSE 0 END
			) * (CH.DEBIT)) VAL_CASHIN,

 sum((
			CASE WHEN CH.DATE_ BETWEEN @P1
						AND @P2 THEN 1 ELSE 0 END
			) * (CH.CREDIT)) VAL_CASHOUT,

 sum((CASE WHEN CH.DATE_ <= @P2 THEN 1 ELSE 0 END) * (CH.DEBIT - CH.CREDIT)) VAL_CASHEND

FROM LG_$FIRM$_$PERIOD$_CSHTOTS CH 
--$MS$--WITH(NOLOCK)
LEFT JOIN LG_$FIRM$_KSCARD KS 
--$MS$--WITH(NOLOCK) 
ON CH.CARDREF = KS.LOGICALREF
WHERE CH.TOTTYPE = 1
	AND KS.CODE LIKE @P3
	AND CH.DATE_ <= @P2


"
, new object[] { pDf, pDt, "%" });

                res.TableName = "CASH_IO";

                return res;
            }



            public static DataTable GET_BANK_IO_LOC(_PLUGIN PLUGIN, DateTime pDf, DateTime pDt)
            {
                var data = GET_BANK_IO_EXT(PLUGIN, pDf, pDt);
                //l_dailyexchanges
                var cols = new string[] { "VAL_BANKBEG", "VAL_BANKIN", "VAL_BANKOUT", "VAL_BANKEND" };

                var tabRes = new DataTable();
                foreach (var c in cols)
                    TAB_ADDCOL(tabRes, c, typeof(double));

                tabRes.Rows.Add(tabRes.NewRow());
                TAB_FILLNULL(tabRes);

                //


                foreach (DataRow row in data.Rows)
                {
                    var curr = CASTASSHORT(TAB_GETROW(row, "CURRENCY"));
                    var exch = GET_EXCHANGE(PLUGIN, curr, pDt);

                    foreach (var c in cols)
                    {
                        var v1 = CASTASDOUBLE(TAB_GETROW(row, c));

                        v1 = v1 * exch;

                        TAB_SETROW(row, c, v1);
                    }

                }

                MY_SUM_TABLE(tabRes, data);

                return tabRes;

            }

            public static double GET_EXCHANGE(_PLUGIN PLUGIN, short pCurr, DateTime pDt)
            {
                if (pCurr == 0 || pCurr == PLUGIN.GETSYSPRM_CURRENCY())
                    return 1.0;

                var rate = CASTASDOUBLE(
                     PLUGIN.SQLSCALAR("SELECT RATES1 FROM L_DAILYEXCHANGES WHERE CRTYPE = @P1 AND DATE_ <= @P2 ",
                     new object[] { pCurr, GETDATETODATEINT(pDt) }));


                return rate;
            }
            public static DataTable GET_BANK_IO_EXT(_PLUGIN PLUGIN, DateTime pDf, DateTime pDt)
            {


                var res = PLUGIN.SQL(
@"
--@df P1
--@dt P2
--@accCode P3
 

SELECT 
BANKACC.CODE,
BANKACC.CARDTYPE, 
BANKACC.CURRENCY,
SUM(
(CASE WHEN  BNFLINE.DATE_ < @P1 THEN + 1 ELSE 0 END)*
(CASE WHEN BNFLINE.SIGN = 0 THEN + 1 ELSE -1 END)*
(CASE WHEN  BANKACC.CARDTYPE IN (3,4) THEN BNFLINE.TRNET ELSE BNFLINE.AMOUNT END)
) VAL_BANKBEG,
SUM(
(CASE WHEN  (BNFLINE.DATE_ BETWEEN  @P1 AND @P2) THEN + 1 ELSE 0 END)*
(CASE WHEN BNFLINE.SIGN = 0 THEN + 1 ELSE 0 END)*
(CASE WHEN  BANKACC.CARDTYPE IN (3,4) THEN BNFLINE.TRNET ELSE BNFLINE.AMOUNT END)
) VAL_BANKIN,
SUM(
(CASE WHEN  (BNFLINE.DATE_ BETWEEN  @P1 AND @P2) THEN + 1 ELSE 0 END)*
(CASE WHEN BNFLINE.SIGN = 1 THEN + 1 ELSE 0 END)*
(CASE WHEN  BANKACC.CARDTYPE IN (3,4) THEN BNFLINE.TRNET ELSE BNFLINE.AMOUNT END)
) VAL_BANKOUT,
SUM(
(CASE WHEN  BNFLINE.DATE_ <=  @P2 THEN + 1 ELSE 0 END)*
(CASE WHEN BNFLINE.SIGN = 0 THEN + 1 ELSE -1 END)*
(CASE WHEN  BANKACC.CARDTYPE IN (3,4) THEN BNFLINE.TRNET ELSE BNFLINE.AMOUNT END)
) VAL_BANKEND

FROM LG_$FIRM$_$PERIOD$_BNFLINE BNFLINE 
--$MS$--WITH(NOLOCK) 
 INNER JOIN LG_$FIRM$_BANKACC BANKACC 
--$MS$--WITH(NOLOCK) 
 ON BNFLINE.BNACCREF = BANKACC.LOGICALREF

 WHERE 
 BNFLINE.TRANSTYPE > 0 AND 
 BNFLINE.CANCELLED = 0 AND 
 BNFLINE.TRNSTATE = 0 AND 
 BNFLINE.OPSTAT IN (0,2) AND
 --AND BANKACC.CODE LIKE @P3
 BNFLINE.DATE_ <= @P2
 GROUP BY BANKACC.CODE, BANKACC.CARDTYPE, BANKACC.CURRENCY


"
, new object[] { pDf, pDt, "%" });

                res.TableName = "BANK_IO";

                return res;
            }


            public static DataTable GET_BANK_OPER_BY_SPECODE(_PLUGIN PLUGIN, DateTime pDf, DateTime pDt)
            {


                var res = PLUGIN.SQL(
@"
--@df P1
--@dt P2
--@cashCode P3





SELECT
sum(COALESCE((
				CASE WHEN TRCODE = 3
						AND SPECODE LIKE '642%' THEN AMOUNT ELSE 0 END
				), 0)) VAL_ACC642,
sum(COALESCE((
				CASE WHEN TRCODE = 3
						AND SPECODE LIKE '655%' THEN AMOUNT ELSE 0 END
				), 0)) VAL_ACC655,
sum(COALESCE((
				CASE WHEN TRCODE = 3
						AND SPECODE LIKE '411%' THEN AMOUNT ELSE 0 END
				), 0)) VAL_ACC411,
sum(COALESCE((
				CASE WHEN TRCODE = 4
						AND SPECODE LIKE '521%' THEN AMOUNT ELSE 0 END
				), 0)) VAL_ACC521,
sum(COALESCE((
				CASE WHEN TRCODE = 4
						AND SPECODE LIKE '534%' THEN AMOUNT ELSE 0 END
				), 0)) VAL_ACC534,
sum(COALESCE((
				CASE WHEN TRCODE = 4
						AND SPECODE LIKE '728%' THEN AMOUNT ELSE 0 END
				), 0)) VAL_ACC728,
sum(COALESCE((
				CASE WHEN TRCODE = 4
						AND SPECODE LIKE '733%' THEN AMOUNT ELSE 0 END
				), 0)) VAL_ACC733,
sum(COALESCE((
				CASE WHEN TRCODE = 4
						AND SPECODE LIKE '735%' THEN AMOUNT ELSE 0 END
				), 0)) VAL_ACC735,
sum(COALESCE((
				CASE WHEN TRCODE = 4
						AND SPECODE LIKE '738%' THEN AMOUNT ELSE 0 END
				), 0)) VAL_ACC738,
sum(COALESCE((
				CASE WHEN TRCODE = 4
						AND SPECODE LIKE '742%' THEN AMOUNT ELSE 0 END
				), 0)) VAL_ACC742,
sum(COALESCE((
				CASE WHEN TRCODE = 4
						AND SPECODE LIKE '747%' THEN AMOUNT ELSE 0 END
				), 0)) VAL_ACC747
FROM LG_$FIRM$_$PERIOD$_BNFLINE 
--$MS$--WITH(NOLOCK)
WHERE CANCELLED = 0
	AND DATE_ BETWEEN @P1
		AND @P2
	AND CANCELLED = 0
    AND MODULENR = 7
	AND TRCODE IN (3, 4)


"
, new object[] { pDf, pDt, "%" });

                res.TableName = "BANK_BY_TYPE";

                return res;
            }


            public static DataTable GET_FIN_OPER_BY_SPECODE(_PLUGIN PLUGIN, DateTime pDf, DateTime pDt)
            {
                var cash = GET_CASH_OPER_BY_SPECODE(PLUGIN, pDf, pDt);
                var bank = GET_BANK_OPER_BY_SPECODE(PLUGIN, pDf, pDt);

                MY_SUM_TABLE(cash, bank);

                return cash;
            }

            public static DataTable GET_CLIENT_IO(_PLUGIN PLUGIN, DateTime pDf, DateTime pDt)
            {


                var res = PLUGIN.SQL(
@"
--@df P1
--@dt P2
 
 
WITH CL_BALANCE AS (

SELECT 
T.LOGICALREF,
(CASE 
WHEN T.CARDTYPE = 3 AND T.BALANCE > 0 THEN 1 
WHEN T.CARDTYPE = 3 AND T.BALANCE < 0.01 THEN 2 
ELSE T.CARDTYPE END 
) CARDTYPE,
T.BALANCE,T.IN_,T.OUT_ 
FROM (

    SELECT C.LOGICALREF,C.CARDTYPE,
	    sum(COALESCE(
(CASE WHEN CLFLINE.SIGN = 0 THEN 1 WHEN CLFLINE.SIGN = 1 THEN - 1 ELSE 0 END) * 
(CLFLINE.AMOUNT), 0)
) BALANCE,
				
	    sum(COALESCE((
				    CASE WHEN ( CLFLINE.DATE_ ) BETWEEN @P1 AND @P2 THEN 1 ELSE 0 END
				    ) * (CASE WHEN CLFLINE.SIGN = 0 THEN 1 ELSE 0 END) * (CLFLINE.AMOUNT), 0)) IN_,
	    sum(COALESCE((
				    CASE WHEN ( CLFLINE.DATE_ ) BETWEEN @P1 AND @P2 THEN 1 ELSE 0 END
				    ) * (CASE WHEN CLFLINE.SIGN = 1 THEN 1 ELSE 0 END) * (CLFLINE.AMOUNT), 0)) OUT_			
	
 
    FROM LG_$FIRM$_CLCARD C 
    --$MS$--WITH(NOLOCK)
    INNER JOIN LG_$FIRM$_$PERIOD$_CLFLINE CLFLINE 
    --$MS$--WITH(NOLOCK) 
    ON CLFLINE.CLIENTREF = C.LOGICALREF
    WHERE (CLFLINE.DATE_ <= @P2)
	    AND (CLFLINE.CANCELLED = 0)
	    AND C.CARDTYPE IN (1,2,3)
    GROUP BY C.LOGICALREF,C.CARDTYPE

    ) T
)

SELECT  

COALESCE(sum((CASE WHEN B.CARDTYPE = 1 THEN 1 ELSE 0 END)*(CASE WHEN B.BALANCE > 0 THEN B.BALANCE ELSE 0 END)),0) VAL_CLCDEBIT,
COALESCE(sum((CASE WHEN B.CARDTYPE = 1 THEN 1 ELSE 0 END)*(CASE WHEN B.BALANCE < 0 THEN abs(B.BALANCE) ELSE 0 END)),0) VAL_CLCCREDIT,
COALESCE(sum((CASE WHEN B.CARDTYPE = 1 THEN 1 ELSE 0 END)*(B.IN_)),0) VAL_CLCIN,
COALESCE(sum((CASE WHEN B.CARDTYPE = 1 THEN 1 ELSE 0 END)*(B.OUT_)),0) VAL_CLCOUT,
 
COALESCE(sum((CASE WHEN B.CARDTYPE = 2 THEN 1 ELSE 0 END)*(CASE WHEN B.BALANCE > 0 THEN B.BALANCE ELSE 0 END)),0) VAL_CLVDEBIT,
COALESCE(sum((CASE WHEN B.CARDTYPE = 2 THEN 1 ELSE 0 END)*(CASE WHEN B.BALANCE < 0 THEN abs(B.BALANCE) ELSE 0 END)),0) VAL_CLVCREDIT,
COALESCE(sum((CASE WHEN B.CARDTYPE = 2 THEN 1 ELSE 0 END)*(B.IN_)),0) VAL_CLVIN,
COALESCE(sum((CASE WHEN B.CARDTYPE = 2 THEN 1 ELSE 0 END)*(B.OUT_)),0) VAL_CLVOUT
 
FROM CL_BALANCE B
 

"
, new object[] { pDf, pDt });

                res.TableName = "CL_BALANCE";

                return res;
            }

            public static DataTable GET_WH_OPER(_PLUGIN PLUGIN, DateTime pDf, DateTime pDt)
            {


                var res = PLUGIN.SQL(
           @"
--@df P1
--@dt P2
 

SELECT 
	 sum((CASE WHEN F.TRCODE IN (50) THEN 1 ELSE 0 END) * F.NETTOTAL) VAL_MATTOT_COUNT50,
	 sum((CASE WHEN F.TRCODE IN (51) THEN 1 ELSE 0 END) * F.NETTOTAL) VAL_MATTOT_COUNT51
FROM LG_$FIRM$_$PERIOD$_STFICHE F
--$MS$--WITH(NOLOCK) 
WHERE F.DATE_ BETWEEN @P1
		AND @P2
	AND F.CANCELLED = 0
	--AND F.SOURCEINDEX = 0

"
           , new object[] { pDf, pDt });

                res.TableName = "WH_OPER";

                return res;
            }

            public static DataTable GET_PRCH_SLS_OPER_ALL(_PLUGIN PLUGIN, DateTime pDf, DateTime pDt)
            {


                var res = PLUGIN.SQL(
           @"
--@df P1
--@dt P2
 


 
SELECT
     sum((CASE WHEN F.TRCODE IN (1) THEN 1 ELSE 0 END) * F.NETTOTAL) VAL_MATTOT_PRCH,
	 sum((CASE WHEN F.TRCODE IN (6) THEN 1 ELSE 0 END) * F.NETTOTAL) VAL_MATTOT_PRCHRET,
	 sum((CASE WHEN F.TRCODE IN (8) THEN 1 ELSE 0 END) * F.NETTOTAL) VAL_MATTOT_SLS, --sales mat and service
	 sum((CASE WHEN F.TRCODE IN (3) THEN 1 ELSE 0 END) * F.NETTOTAL) VAL_MATTOT_SLSRET
FROM LG_$FIRM$_$PERIOD$_INVOICE F
--$MS$--WITH(NOLOCK) 
WHERE F.DATE_ BETWEEN @P1
		AND @P2
	AND F.CANCELLED = 0
	--AND F.SOURCEINDEX = 0

"
           , new object[] { pDf, pDt });

                res.TableName = "WH_OPER";

                return res;


            }

            public static DataTable GET_MAT_REM(_PLUGIN PLUGIN, DateTime pDf, DateTime pDt)
            {


                var res = PLUGIN.SQL(
           @"
--@df P1
--@dt P2
 

WITH PRCLIST AS (

SELECT 
LOGICALREF,
COALESCE(PRICE_PURCHASE,0) PRICE_PURCHASE,
COALESCE(PRICE_SALE,PRICE_SALE_LIST,PRICE_PURCHASE,0) PRICE_SALE --for wh cost
FROM (
        SELECT LOGICALREF,

                        (
				        SELECT 
                        --$MS$--TOP(1) 
                        PRICE
				        FROM LG_$FIRM$_PRCLIST PRC 
                        --$MS$--WITH(NOLOCK)
				        WHERE PRC.CARDREF = I.LOGICALREF
					        AND PRC.PTYPE = 2
					        AND PRC.PAYPLANREF = 0
				        ORDER BY ENDDATE DESC
                        --$PG$--LIMIT 1
                        ) PRICE_SALE_LIST,
       
				
         (

                  SELECT SUM(TOTAL)/SUM(AMOUNT) FROM
		                    (
		                    SELECT 
		                    --$MS$--TOP 3
		                    AMOUNT AMOUNT,
		                    (VATMATRAH+VATAMNT+DISTEXP) TOTAL
		                    FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK)
		                    WHERE (
				                    STOCKREF = I.LOGICALREF AND --@P1
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
		 )
        
        PRICE_PURCHASE,
		        
        (

             SELECT SUM(TOTAL)/SUM(AMOUNT) FROM
		            (
		            SELECT 
		            --$MS$--TOP 3
		            AMOUNT AMOUNT,
		            (VATMATRAH+VATAMNT+DISTEXP) TOTAL
		            FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK)
		            WHERE (
				            STOCKREF = I.LOGICALREF AND --@P1
				            VARIANTREF = 0 AND DATE_ <= @P2 AND FTIME < 987654321 AND IOCODE = 4 AND  
				            SOURCEINDEX >= 0
				            ) AND (
				            CANCELLED = 0 AND TRCODE IN ( 8,7) AND (AMOUNT > 0) AND LINETYPE IN (0)
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
    
		 )
   
        PRICE_SALE

        FROM LG_$FIRM$_ITEMS I


) Z

)

SELECT
COALESCE((sum(ONHAND * PRICE_SALE)),0) VAL_MATREM_SLSPRC,
COALESCE((sum(ONHAND * PRICE_PURCHASE)),0) VAL_MATREM_PRCHPRC
FROM (
	SELECT T.STOCKREF,
		SUM(ONHAND) ONHAND,
		COALESCE((
				SELECT PRICE_PURCHASE
				FROM PRCLIST PRC
				WHERE PRC.LOGICALREF = T.STOCKREF
				), 0) PRICE_PURCHASE,
		COALESCE(
                (
				SELECT PRICE_SALE
				FROM PRCLIST PRC
				WHERE PRC.LOGICALREF = T.STOCKREF
				),0) PRICE_SALE

	FROM LG_$FIRM$_$PERIOD$_STINVTOT T 
--$MS$--WITH(NOLOCK)
	WHERE T.INVENNO = -1
		AND T.DATE_ <= @P2
	GROUP BY T.STOCKREF
	) D
WHERE D.ONHAND > 0



"
           , new object[] { pDf, pDt });

                res.TableName = "MAT_REM";

                return res;
            }

            public static DataTable GET_PRCH_SLS_OPER_MAT(_PLUGIN PLUGIN, DateTime pDf, DateTime pDt)
            {


                var res = PLUGIN.SQL(
           @"
--@df P1
--@dt P2
 
 
SELECT 

COALESCE(SUM(COALESCE(
(CASE WHEN STLINE.LINETYPE IN (0) THEN 1 ELSE 0 END) * 
(CASE WHEN STLINE.TRCODE IN (1) THEN + 1 WHEN STLINE.TRCODE IN (6) THEN - 1 ELSE 0 END) * 
(STLINE.VATMATRAH + STLINE.VATAMNT + STLINE.DISTEXP),0)),0) VAL_MATTOT_PRCH_TOT, --mat prch not service
 
COALESCE(SUM(COALESCE(
(CASE WHEN STLINE.LINETYPE IN (0) THEN 1 ELSE 0 END) * 
(CASE WHEN STLINE.TRCODE IN (7, 8) THEN + 1 WHEN STLINE.TRCODE IN (2, 3) THEN - 1 ELSE 0 END) * 
(STLINE.VATMATRAH + STLINE.VATAMNT + STLINE.DISTEXP),0)),0) VAL_MATTOT_SLS_TOT  --mat sales
 
--

FROM LG_$FIRM$_$PERIOD$_STLINE STLINE 
--$MS$--WITH(NOLOCK,INDEX = I$FIRM$_$PERIOD$_STLINE_I19) 
 
WHERE 
	 STLINE.DATE_ BETWEEN @P1 AND @P2
	AND STLINE.FTIME >= 0 
	AND STLINE.TRCODE IN (7, 8, 2, 3, 1, 6)
	AND STLINE.CANCELLED = 0
	AND STLINE.LINETYPE IN (0,1)


"
           , new object[] { pDf, pDt });

                res.TableName = "PRCH_SLS_OPER_MAT";

                return res;
            }

            public static DataTable GET_MAT_PROFIT(_PLUGIN PLUGIN, DateTime pDf, DateTime pDt)
            {

                if (pDf > pDt)
                    throw new Exception("Incorrect date range");

                var d1 = pDf;

                var list = new List<DataTable>();

                while (d1 <= pDt)
                {

                    list.Add(_GET_MAT_PROFIT(PLUGIN, d1, d1));

                    d1 = d1.AddDays(+1);
                }


                var first = list[0];

                list.RemoveAt(0);
                foreach (var tab in list)
                    MY_SUM_TABLE(first, tab);


                return first;

            }

            static DataTable _GET_MAT_PROFIT(_PLUGIN PLUGIN, DateTime pDf, DateTime pDt)
            {


                var res = PLUGIN.SQL(
           @"
--@df P1
--@dt P2
 
 
 
SELECT 
 
COALESCE(SUM(COALESCE(
(CASE WHEN STLINE.LINETYPE IN (0, 1) THEN 1 ELSE 0 END) * 
(CASE WHEN STLINE.TRCODE IN (7, 8) THEN + 1 WHEN STLINE.TRCODE IN (2, 3) THEN - 1 ELSE 0 END) * 
(((STLINE.AMOUNT * 1.0 /*STLINE.UINFO2 / STLINE.UINFO1*/) * STLINE.OUTCOST)),0)),0) VAL_MATTOT_SLS_COST,
 
COALESCE(SUM(COALESCE(
(CASE WHEN STLINE.LINETYPE IN (0) THEN 1 ELSE 0 END) * 
(CASE WHEN STLINE.TRCODE IN (7, 8) THEN + 1 WHEN STLINE.TRCODE IN (2, 3) THEN - 1 ELSE 0 END) * 

((STLINE.AMOUNT * 1.0 /*STLINE.UINFO2 / STLINE.UINFO1*/) * (

/*COST*/

		SELECT SUM(TOTAL)/SUM(AMOUNT) FROM
		(
		SELECT 
		--$MS$--TOP 3
		AMOUNT AMOUNT,
		(VATMATRAH+VATAMNT+DISTEXP) TOTAL
		FROM LG_$FIRM$_$PERIOD$_STLINE WITH(NOLOCK)
		WHERE (
				STOCKREF = STLINE.STOCKREF AND --@P1
				VARIANTREF = 0 AND DATE_ <= STLINE.DATE_ AND FTIME < 987654321 AND IOCODE = 1 AND --IOCODE IN (3,4) AND --@P2
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

/*COST*/
))
 

,0)),0) VAL_MATTOT_SLS_COST_BY_PRCH

--

FROM LG_$FIRM$_$PERIOD$_STLINE STLINE WITH(NOLOCK)

WHERE 
	 STLINE.DATE_ BETWEEN @P1 AND @P2
	AND STLINE.FTIME >= 0 
	AND STLINE.TRCODE IN (7, 8, 2, 3) --, 1, 6
	AND STLINE.CANCELLED = 0
	AND STLINE.LINETYPE IN (0,1)


"
           , new object[] { pDf, pDt });

                res.TableName = "MAT_PROFIT";

                return res;
            }



            static void MY_SUM_TABLE(DataTable pMainTab, DataTable pValuesTab)
            {

                foreach (DataColumn c in pMainTab.Columns)
                {

                    var v1 = CASTASDOUBLE(TAB_GETROW(pMainTab, c.ColumnName));
                    var sum = v1;

                    foreach (DataRow r in pValuesTab.Rows)
                    {

                        var v2 = CASTASDOUBLE(TAB_GETROW(r, c.ColumnName));
                        sum += v2;

                    }


                    TAB_SETROW(pMainTab, c.ColumnName, sum);
                }

            }
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

        static bool MY_ASKDATETIME(_PLUGIN pPLUGIN, string pMsg, ref DateTime pDf, ref DateTime pDt, bool pShowTime = false)
        {

            DataRow[] rows_ = pPLUGIN.REF("ref.gen.daterange showtime::" + FORMAT(pShowTime) + " desc::" + _PLUGIN.STRENCODE(pMsg)
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

        static bool MY_ASKDATETIME(_PLUGIN pPLUGIN, string pMsg, ref DateTime pDt)
        {

            DataRow[] rows_ = pPLUGIN.REF("ref.gen.date showtime::0 desc::" + _PLUGIN.STRENCODE(pMsg)
                + " filter::"
                + "filter_VALUE," + _PLUGIN.FORMATSERIALIZE(pDt)

            )
                ;

            if (rows_ != null && rows_.Length > 0)
            {

                pDt = CASTASDATE(rows_[0]["DATETIME"]);


                return true;
            }

            return false;

        }


        static DateTime MY_MONTH_BEG(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        static DateTime MY_MONTH_END(DateTime dt)
        {
            return MY_MONTH_BEG(dt).AddMonths(+1).AddDays(-1);
        }

        #endregion

        #endregion