 #line 2
 
 
        public void SYS_BEGIN()
        {
 
            if (!MSGUSERASK("T_MSG_COMMIT_BEGIN (Barkod vermək) ?"))
                throw new Exception("T_MSG_OPERATION_STOPPING");

//EXEADPCMD(string[] pCmds, DoWorkEventHandler[] pEvents)

			List<string> listCmd_ = new List<string>();
			List<DoWorkEventHandler> listEvent_ = new List<DoWorkEventHandler>();
            
			foreach(DataRow row_ in MY_GET_NOBARCODE_MATS().Rows)
			{
			listCmd_.Add(MY_GET_MAT_CMD(MY_GET_MAT_INFO(row_["LOGICALREF"])));
			listEvent_.Add(new DoWorkEventHandler(this.DoWorkEventHandler));
			
			}
			EXEADPCMD(listCmd_.ToArray(), listEvent_.ToArray());
			
			
			 if(countAffected > 0)  MSGUSERINFO("Mallarin sayı " + countAffected.ToString());
              MSGUSERINFO("T_MSG_OPERATION_FINISHED");
        }
       

 
        DataTable MY_GET_NOBARCODE_MATS()
        {
            return SQL("select distinct I.LOGICALREF from LG_$FIRM$_ITEMS I inner join LG_$FIRM$_UNITSETL U on I.UNITSETREF  =  U.UNITSETREF where I.CARDTYPE in (1)  and not exists(select 1 from LG_$FIRM$_UNITBARCODE B where B.TYP = 0 and B.ITEMREF = I.LOGICALREF and  B.UNITLINEREF = U.LOGICALREF) order by I.LOGICALREF", new object[] {  }); 
        }
		
        DataRow MY_GET_MAT_INFO(object pLRef)
        {
            return TAB_GETLASTROW(SQL("select * from LG_$FIRM$_ITEMS I where I.LOGICALREF = @P1", new object[] { pLRef })); 
        }
		
		DataTable MY_GET_MAT_UNIT_INFO(object pLRef)
        {
            return  SQL("select U.LOGICALREF from LG_$FIRM$_ITEMS I inner join LG_$FIRM$_UNITSETL U on I.UNITSETREF  =  U.UNITSETREF where I.LOGICALREF = @P1", new object[] { pLRef }) ; 
        }
		
		string MY_GET_MAT_CMD(DataRow pRow)
		{
		 if(pRow == null) 
		 return "";
		 
		 return string.Format("adp.mm.rec.mat/{0} cmd::edit lref::{1}",FORMAT(pRow["CARDTYPE"]),FORMAT(pRow["LOGICALREF"]));
		
		}
		

		int countAffected =0;
		void DoWorkEventHandler(object sender, DoWorkEventArgs e)
		{
		++countAffected;
		//e.Result = true; //for send changes
		e.Result = false;
		DataSet ds_ = e.Argument as DataSet;
		 
		
		e.Result = MY_FILLMATWITHBARCODE(  ds_); 
		 
		 
		}
		
		
		public bool MY_FILLMATWITHBARCODE(DataSet pDs){
		DataSet ds_ = pDs;
		
		
		if(ds_ != null)
		{
		DataTable barcodes_ = TAB_GETTAB(ds_ ,"UNITBARCODE");
		object matLRef = TAB_GETROW(TAB_GETTAB(ds_ ,"ITEMS"), "LOGICALREF");
		if(!ISNULL(matLRef))
		{
		
			foreach(DataRow row_ in MY_GET_MAT_UNIT_INFO(matLRef).Rows)
			{
				DataRow rowTarget_ = TAB_SEARCH(barcodes_, "UNITLINEREF", row_["LOGICALREF"]);
				if(rowTarget_ == null) {rowTarget_ = TAB_ADDROW(barcodes_); rowTarget_["UNITLINEREF"] = row_["LOGICALREF"];   }
				MY_FILL_BARCODE(rowTarget_);
			}
		
		return true; 
		}
		}
		
		return false;
		}
		
		
		
		public void MY_FILL_BARCODE(DataRow pRow)
		{
			string barcode = pRow["BARCODE"].ToString();
			if(barcode == "")
			{
			barcode = MY_GET_BARCODE();
			pRow["BARCODE"] = barcode;
			}
		
		}
 
 
 		public string MY_GET_BARCODE()
		{
			
			while(true)
			{
			int id_ = CASTASINT(ISNULL(GETSEQFIRM(),0));
			string barcode_ = MY_WRAP_TO_BARCODE(id_);
			if(!MY_HAS_BARCODE(  barcode_))
				return barcode_;
			}
			throw new Exception("T_MSG_ERROR_NUMERATION (T_BARCODE)");
		}
		
		string MY_WRAP_TO_BARCODE(int pId)
		{
		 return  BARCODEADDCHECKSUM( "21"+ FORMAT(pId).PadLeft(10, '0'));
		}
		
		bool MY_HAS_BARCODE(string pBarcode)
		{
			return ( "OK" == ISNULL(SQLSCALAR("select top 1 'OK' RES from LG_$FIRM$_UNITBARCODE where BARCODE = @P1",new object[]{pBarcode}),"").ToString());
			
		}
		