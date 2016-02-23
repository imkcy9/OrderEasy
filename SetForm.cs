using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OrderEasy.common;

namespace OrderEasy
{
    public partial class SetForm : Form
    {
        private SourceGrid.Cells.Views.ColumnHeader headerView = new SourceGrid.Cells.Views.ColumnHeader();
        private SourceGrid.Cells.Views.Cell cellView = new SourceGrid.Cells.Views.Cell();
        private SourceGrid.Cells.Views.CheckBox checkView = new SourceGrid.Cells.Views.CheckBox();
        private int _W = 230;
        private int _H = 25;
        private Common comon;
        private OrderEasy frmMain;
        public SetForm(OrderEasy _frmMain)
        {
            InitializeComponent();
            this.frmMain = _frmMain;
        }
        private void initView()
        {

            DevAge.Drawing.RectangleBorder border = new DevAge.Drawing.RectangleBorder(new DevAge.Drawing.BorderLine(Color.DarkGreen), new DevAge.Drawing.BorderLine(Color.DarkGreen));

            DevAge.Drawing.VisualElements.ColumnHeader flatHeader = new DevAge.Drawing.VisualElements.ColumnHeader();
            flatHeader.Border = border;
            flatHeader.BackColor = Color.LightBlue;
            flatHeader.BackgroundColorStyle = DevAge.Drawing.BackgroundColorStyle.Solid;
            headerView.Font = new Font(grid_addr.Font, FontStyle.Bold);
            headerView.Background = flatHeader;
            headerView.ForeColor = Color.Black;
            headerView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;


            cellView.Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(Color.White, Color.White, 500);
            cellView.Border = border;

            checkView = new SourceGrid.Cells.Views.CheckBox();
            checkView.Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(Color.White, Color.White, 45);
            checkView.Border = border;
        }
        private void SetGrid(SourceGrid.Grid grid, Dictionary<string, bool> dic, string c1, string c2)
        {

            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.Redim(dic.Count + 1, 2);

            AddOneRow(grid, 0, c1, c2, true);
            int i = 0;
            foreach (KeyValuePair<string, bool> pair in dic)
            {
                string showState = pair.Value ? "on" : "off";
                AddOneRow(grid, i++ + 1, pair.Key, showState, false);
            }

            grid.Selection.SelectRow(1, true);
            grid.Selection.EnableMultiSelection = false;
        }

        private void SetAddrGrid()
        {
            SetGrid(grid_addr, comon.addrDic, "地址", "状态");
            grid_addr.Rows[0].Height = _H;
            grid_addr.Columns[0].Width = 215;
        }
        private void SetFutureGrid()
        {
            grid_account.BorderStyle = BorderStyle.FixedSingle;
            grid_account.Redim(comon.futureDic.Count + 1, 5);
            AddOneRowFutureHead(grid_account, true);
            int i = 0;
            foreach (KeyValuePair<string, Future> pair in comon.futureDic)
            {
                Future f = pair.Value;
                AddOneRowFuture(grid_account, i++ + 1, pair.Key, f.isActive, f.tick.ToString(),f.point,f.symbols);
            }

        }
        private void SetStockGrid()
        {
            grid_category.BorderStyle = BorderStyle.FixedSingle;
            grid_category.Redim(comon.stockDic.Count + 1, 4);
            AddOneRowFutureHead(grid_category, false);
            int i = 0;
            foreach (KeyValuePair<string, Future> pair in comon.stockDic)
            {
                Future f = pair.Value;
                AddOneRowFuture(grid_category, i++ + 1, pair.Key, f.isActive, f.tick.ToString(), f.point, "");
            }
        }
        private void init() {

            initView();
            comon = Common.Instance();
            SetAddrGrid();
            SetFutureGrid();
            SetStockGrid();
        }

        private void SetForm_Load(object sender, EventArgs e)
        {
            init();
        }

        private void AddOneRowFutureHead(SourceGrid.Grid grid, bool isFutrue)
        {
            try
            {
                string fristName = isFutrue ? "品种" : "合约";
                grid[0, 0] = new SourceGrid.Cells.Cell(fristName, typeof(string));
                grid[0, 0].View = headerView;
                grid[0, 1] = new SourceGrid.Cells.Cell("状态", typeof(string));
                grid[0, 1].View = headerView;
                grid[0, 2] = new SourceGrid.Cells.Cell("TICK", typeof(string));
                grid[0, 2].View = headerView;
                grid[0, 3] = new SourceGrid.Cells.Cell("小数点", typeof(string));
                grid[0, 3].View = headerView;
                if (isFutrue)
                {
                    grid[0, 4] = new SourceGrid.Cells.Cell("合约", typeof(string));
                    grid[0, 4].View = headerView;
                }
            }
            catch (System.Exception ex)
            {
                Program.log.Error(ex, ex);
            }

        }
        private void AddOneRowFuture(SourceGrid.Grid grid, int r, string product, bool isCheck, string tick,int point, string symbol)
        {
            try
            {
                grid[r, 0] = new SourceGrid.Cells.Cell(product, typeof(string));
                grid[r, 0].View = cellView;
                grid[r, 1] = new SourceGrid.Cells.CheckBox(null, isCheck);
                grid[r, 1].View = checkView;

                grid[r, 2] = new SourceGrid.Cells.Cell(tick, typeof(string));
                grid[r, 2].View = cellView;
                grid[r, 3] = new SourceGrid.Cells.Cell(point, typeof(int));
                grid[r, 3].View = cellView;
                if (!string.IsNullOrEmpty(symbol))
                {
                    grid[r, 4] = new SourceGrid.Cells.Cell(symbol, typeof(string));
                    grid[r, 4].View = cellView;
                }
            }
            catch (System.Exception ex)
            {
                Program.log.Error(ex, ex);
            }

        }
        private void AddOneRow(SourceGrid.Grid grid, int r, string c1, string c2, bool isHead)
        {
            try
            {
                //int rowCount = grid.RowsCount;
                //grid.Redim(rowCount + 1, 2);

                grid[r, 0] = new SourceGrid.Cells.Cell(c1, typeof(string));
                grid[r, 0].View = isHead ? headerView : cellView;
                if (isHead)
                {
                    grid[r, 1] = new SourceGrid.Cells.Cell(c2, typeof(string));
                    grid[r, 1].View = headerView;
                }
                else
                {
                    bool isCheck = c2 == "on" ? true : false;
                    grid[r, 1] = new SourceGrid.Cells.CheckBox(null, isCheck);
                    grid[r, 1].View = checkView;
                }
                //grid.Refresh();
            }
            catch (System.Exception ex)
            {
                Program.log.Error(ex, ex);
            }

        }
        private void btnAdd_Click(object sender, EventArgs e)
        {

            string name = tabControl1.SelectedTab.Name;
            if ("tab_addr_m".Equals(name))
            {
                //AddOneRow(grid_addr, comon.addrList.Count + 1, "", "显示", false);
                int rowCount = grid_addr.RowsCount;
                grid_addr.Redim(rowCount + 1, 2);
                AddOneRow(grid_addr, rowCount, "tcp://192.168.0.10:6006", "on", false);
                grid_addr.Refresh();
            }
            else if ("tab_account_m".Equals(name))
            {
                int rowCount = grid_account.RowsCount;
                grid_account.Redim(rowCount + 1, 2);
                AddOneRow(grid_account, rowCount, "0000", "on", false);
                grid_account.Refresh();
            }
            else
            {
                int rowCount = grid_category.RowsCount;
                grid_category.Redim(rowCount + 1, 2);
                AddOneRow(grid_category, rowCount, "XX", "on", false);
                grid_category.Refresh();
            }
        }
        private int DeleteRow(SourceGrid.Grid grid){

            for (int i =0;i<grid.Rows.Count;i++)
            {
                if (grid.Selection.IsSelectedRow(i))
                {
                    grid.Rows.Remove(i);
                    grid.Refresh();
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="rowMove">-1:down 1:up</param>
        /// <returns></returns>
        private int changeIndex(SourceGrid.Grid grid,int rowMove)
        {

            for (int i = 0; i < grid.Rows.Count; i++)
            {
                if (grid.Selection.IsSelectedRow(i))
                {
                    if (i + rowMove > grid.Rows.Count - 1)
                    {
                        MessageBox.Show("已到最底!");
                        return -1;
                    }
                    else if (i + rowMove <0)
                    {
                        MessageBox.Show("已到最顶!");
                        return -1;
                    }

                    grid.Refresh();
                    return i;
                }
            }
            return -1;
        }
        private void btnSub_Click(object sender, EventArgs e)
        {
            string name = tabControl1.SelectedTab.Name;
            if ("tab_addr_m".Equals(name))
            {
                int deleteIndex = DeleteRow(grid_addr);
                ZMQControl.Instance().DisConnect(grid_addr[deleteIndex,0].Value + "");
                
                return;
            }
            else if ("tab_account_m".Equals(name))
            {
                DeleteRow(grid_account);
                //return;
            }
            else
            {
                DeleteRow(grid_category);
            }

        }

        private void btnUp_Click(object sender, EventArgs e)
        {

            //string name = tabControl1.SelectedTab.Name;
            //if ("tab_addr_m".Equals(name))
            //{
            //    return;
            //}
            //else if ("tab_account_m".Equals(name))
            //{
            //    changeIndex(grid_account);
            //    //return;
            //}
            //else
            //{
            //    changeIndex(grid_category);
            //}
        }

        private void btnDown_Click(object sender, EventArgs e)
        {

        }

        private void Rest(Dictionary<string, bool> dic, SourceGrid.Grid grid)
        {

            //list.Clear();
            dic.Clear();
            for (int i = 1; i < grid.RowsCount; i++)
            {
                bool isChecked = (bool)grid[i, 1].Value;
                string key = grid[i, 0].Value + "";
                if (!dic.ContainsKey(key))
                {
                    dic.Add(key, isChecked);
                }
                
            }
        }
        private void AddrConHandle()
        {

            foreach (KeyValuePair<string, bool> pair in comon.addrDic)
            {
                if (pair.Value)
                {
                    ZMQControl.Instance().Connect(pair.Key);
                }
                else
                {
                    ZMQControl.Instance().DisConnect(pair.Key);
                }

            }
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            Rest(comon.addrDic, grid_addr);
            //Rest(comon.futureDic, grid_account);
            //Rest(comon.productDic, grid_category);
            frmMain.updateGridHandle();
            AddrConHandle();
            comon.SaveConfig();

            this.Close();
        }

    }
}
