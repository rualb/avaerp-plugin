//Env. arguments
//ADPSTR - adapter string
//ADPCODE - adapter code


//
//COSTDISTPEG containts a copy of material trans. for each COSTDISTLN record (COSTDISTPEG.COSTDISTLNREF <-> COSTDISTLN.LOGICALREF)
//
//
        
        DataRow[] MY_SELECT(DataRow ROW, DataSet DATASET)
        {
            object lref = TAB_GETROW(ROW, "LOGICALREF");
            List<DataRow> listTmp = new List<DataRow>();
            foreach (DataRow rowTmp in DATASET.Tables["COSTDISTPEG"].Rows)
                if (rowTmp.RowState != DataRowState.Deleted)
                    if (COMPARE(TAB_GETROW(rowTmp, "COSTDISTLNREF"), lref))
                        listTmp.Add(rowTmp);
            return listTmp.ToArray();
        }
        double MY_GETVALUE(DataRow ROW, short DISTTYPE)
        {
            if (COMPARE(TAB_GETROW(ROW, "ISDISTRIBUTED"), 0))
                return 0;

            switch (DISTTYPE)
            {
                case 0: //do nothing
                    return 0;
                case 1: //price*quantity
                    return CASTASDOUBLE(TAB_GETROW(ROW, "TOTALAMNT")) * CASTASDOUBLE(TAB_GETROW(ROW, "UNITPRICE"));
                case 2: //quantity
                    return CASTASDOUBLE(TAB_GETROW(ROW, "TOTALAMNT"));
                case 3: //weight
                    return CASTASDOUBLE(TAB_GETROW(ROW, "TOTALAMNT")) * CASTASDOUBLE(ISNULL(SQLSCALAR("select WEIGHT from LG_$FIRM$_ITMUNITA where ITEMREF  = @P1 and LINENR = 1", new object[] { TAB_GETROW(ROW, "ITEMREF") }), 0));
                case 4: //volume
                    return CASTASDOUBLE(TAB_GETROW(ROW, "TOTALAMNT")) * CASTASDOUBLE(ISNULL(SQLSCALAR("select VOLUME_ from LG_$FIRM$_ITMUNITA where ITEMREF  = @P1 and LINENR = 1", new object[] { TAB_GETROW(ROW, "ITEMREF") }), 0));
                case 5: //rate
                    return CASTASDOUBLE(TAB_GETROW(ROW, "DISTRATE"));
                case 6: //price
                    return CASTASDOUBLE(TAB_GETROW(ROW, "UNITPRICE"));
            }

            throw new Exception("T_MSG_INVALID_PARAMETER [" + DISTTYPE.ToString() + "]");
        }
        public void SYS_BEGIN(DataSet DATASET)
        {


            //search records
            foreach (DataRow rowTmp in DATASET.Tables["COSTDISTLN"].Rows)
                if (rowTmp.RowState != DataRowState.Deleted)
                {


                    short distType = CASTASSHORT(TAB_GETROW(rowTmp, "SRVDISTTYPE"));

                    DataRow[] records = MY_SELECT(rowTmp, DATASET);
                    double[] data = new double[records.Length];

                    for (int indx = 0; indx < records.Length; ++indx)
                    {
                        data[indx] = MY_GETVALUE(records[indx], distType);
                    }


                    //sum
                    double sum = 0;
                    foreach (double val in data)
                        sum += val;
                    //set record rate (%)
                    for (int indx = 0; indx < records.Length; ++indx)
                    {
                        double rate_ = MULT(data[indx], DIV(100.0, sum));
                        TAB_SETROW(records[indx], "DISTRATE", rate_);
                    }

                }

        }



		public void SYS_END()
		{

		}
