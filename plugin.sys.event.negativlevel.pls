#line 2

const int VERSION = 4;
 
 
 
public void SYS_BEGIN(string EVENTCODE, object[] ARGS) // adapter data reading (opening)
{
  
object arg1 = ARGS.Length > 0 ? ARGS[0]: null;
object arg2 = ARGS.Length > 1 ? ARGS[1]: null;
object arg3 = ARGS.Length > 2 ? ARGS[2]: null;

 string[] list_ = EXPLODELISTPATH(EVENTCODE);
 
 switch( list_.Length >0 ? list_[0]:""){
  
case SysEvent.SYS_SYSEVENT:
 MY_SYS_SYSEVENT(EVENTCODE,ARGS) ;
 break;
 
 
 }
 

 
}


public void MY_SYS_SYSEVENT(string EVENTCODE, object[] ARGS) //adapter start
{
IDictionary<string,object> dic = ARGS.Length > 0 ? ARGS[0] as IDictionary<string,object>: null;
 if(dic == null)
 return;
 		
 string[] list_ = EXPLODELISTPATH(EVENTCODE);
  
 switch( list_.Length >1 ? list_[1].ToUpperInvariant():""){
 
  case UserEventType.SYS_STOCKLEVEL:
	{
	
	//dic["STOCKREF"];
	//dic["SOURCEINDEX"];
	//dic["AMOUNT"];
	//dic["LEVELNEW"];
	//dic["LEVELALLOWED"];
	//dic["LEVELTYPE"];
	//dic["DATASET"];
			
	Form FORM =	Form.ActiveForm;
	if(FORM == null)
	return;
	DataGridView GRID = CONTROL_SEARCH(FORM,"cGrid") as DataGridView;
	if(GRID == null)
	return;
	DataSet DATASET = GETDATASETFROMADPFORM(FORM);
	DataSet DATASET_TMP = DICGET(dic,"DATASET") as DataSet;
	
	if(DATASET == null || DATASET_TMP == null || !object.ReferenceEquals(DATASET,DATASET_TMP))
	return;
	
	var STLINE = DATASET.Tables["STLINE"];
	if(STLINE == null)
	return;
	
	foreach(DataRow ROW in TAB_SEARCHALL(STLINE, "STOCKREF", DICGET(dic,"STOCKREF"), "SOURCEINDEX", DICGET(dic,"SOURCEINDEX")))
	{
		DataGridViewRow gridRow_ = TOOL_GRID.GET_GRID_ROW(GRID, ROW); //for doc 25
		if(gridRow_ !=null){
			
			double lineAmount_ = CASTASDOUBLE(ISNULL(TAB_GETROW(ROW,"AMOUNT"),0));
			if(!ISNUMZERO(lineAmount_)){ //for ignore emty line
				short lineType_ = CASTASSHORT(ISNULL(TAB_GETROW(ROW,"LINETYPE"),0));
				if(lineType_ == 0 || lineType_ == 1) //mat or promo line
				{
					FORM.ActiveControl = GRID;
					TOOL_GRID.SET_GRID_POSITION(GRID,ROW,"AMOUNT");
				}
			}
		
		}	
	}
	
	
	}
 break;
 }
}
 
  
  
  class GRID_TOOLS{
  
  
  
//grid operations static

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
public static DataGridViewRow MY_GET_GRID_ROW(DataGridView pGrid, DataRow pRow)
{
	foreach(DataGridViewRow row in pGrid.Rows) 
		if(object.ReferenceEquals(pRow,MY_GET_GRID_ROW_DATA( row)))
			return row;
	
	return null;
	
}

public static void MY_SET_GRID_POSITION(DataGridView pGrid, DataRow pRow,string pColumnCode)
{
	if(pRow == null)
	return;

	try
	{
		 foreach(DataGridViewRow rowGrid_  in pGrid.Rows){
		 
		 DataRow rowData_ = MY_GET_GRID_ROW_DATA(rowGrid_);
		 
		 if(rowData_ != null && Object.ReferenceEquals(rowData_, pRow)){
		 
			DataGridViewCell cellCurr_ = null;
			
			if(cellCurr_ == null)
			if(pColumnCode != null && pColumnCode != ""){
				foreach(DataGridViewColumn col_  in pGrid.Columns)
					if(col_.DataPropertyName == pColumnCode)
					{
					cellCurr_ = rowGrid_.Cells[col_.Name]; //Index
					break;
					}
			}
			
			if(cellCurr_ == null){
			 int currCol_ = MY_GET_GRID_CELL(pGrid) != null ? MY_GET_GRID_CELL(pGrid).ColumnIndex : 0;
			  cellCurr_ = rowGrid_.Cells[currCol_];
			  }
			  
			 pGrid.CurrentCell = cellCurr_;
			 
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
/////

  
  }
 
 