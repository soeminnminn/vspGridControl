using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.SqlServer.Management.UI.Grid;

namespace VspGridControlApp
{
    public partial class MainFrom : Form
    {
        #region Variables
        private List<GridDataRow> mGridData;
        #endregion

        #region Constructor
        public MainFrom()
        {
            InitializeComponent();
            this.Initialize();
        }
        #endregion

        #region Methods
        private void Initialize()
        {
            this.BuildData();

            this.gridControl.AddColumn(new GridColumnInfo()
            {
                ColumnType = GridColumnType.Checkbox,
                ColumnWidth = 28,
                WidthType = GridColumnWidthType.InPixels,
                ColumnAlignment = HorizontalAlignment.Center,
                IsUserResizable = false,
                HeaderType = GridColumnHeaderType.CheckBox
            });
            this.gridControl.AddColumn(new GridColumnInfo() { ColumnType = GridColumnType.Bitmap, ColumnWidth = 6, IsUserResizable = false });
            this.gridControl.AddColumn(new GridColumnInfo());
            this.gridControl.AddColumn(new GridColumnInfo());
            this.gridControl.AddColumn(new GridColumnInfo() { ColumnWidth = 24 });
            this.gridControl.AddColumn(new GridColumnInfo());
            this.gridControl.AddColumn(new GridColumnInfo() { ColumnType = GridColumnType.Hyperlink });
            this.gridControl.AddColumn(new GridColumnInfo() { ColumnType = GridColumnType.Button, ColumnAlignment = HorizontalAlignment.Center });

            this.gridControl.SetHeaderInfo(0, "", GridCheckBoxState.Unchecked);
            this.gridControl.SetHeaderInfo(1, "", null);
            this.gridControl.SetHeaderInfo(2, "Text", null);
            this.gridControl.SetHeaderInfo(3, "Dropdown", null);
            this.gridControl.SetHeaderInfo(4, "DropdownList", null);
            this.gridControl.SetHeaderInfo(5, "Spin", null);
            this.gridControl.SetHeaderInfo(6, "Hyperlink", null);
            this.gridControl.SetHeaderInfo(7, "Button", null);

            this.gridControl.SelectionType = GridSelectionType.SingleRow;
            this.gridControl.FirstScrollableColumn = 2;

            this.gridControl.GridStorage = new GridStorage(this.mGridData);

            this.gridControl.MouseButtonClicked += GridControl_MouseButtonClicked;
            this.gridControl.SelectionChanged += GridControl_SelectionChanged;
            this.gridControl.GridSpecialEvent += GridControl_GridSpecialEvent;
        }

        private void BuildData()
        {
            this.mGridData = new List<GridDataRow>();

            for(int i=0; i<100; i++)
            {
                this.mGridData.Add(new GridDataRow() 
                {
                    Image = Properties.Resources.favicon,
                    Text = string.Format("Text: {0}", i),
                    DropDownText = string.Format("DropDown: {0}", i),
                    DropDownListText = string.Format("DropDownList: {0}", i),
                    SpinValue = i,
                    HyperLinkText = string.Format("HyperLink: {0}", i),
                    ButtonText = string.Format("Button: {0}", i)
                });
            }
        }
        #endregion

        #region Grid Events
        private void GridControl_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
        }

        private void GridControl_MouseButtonClicked(object sender, MouseButtonClickedEventArgs args)
        {
            if (args.ColumnIndex == 7)
            {
                MessageBox.Show("Button Clicked");
            }
        }

        private void GridControl_GridSpecialEvent(object sender, GridSpecialEventArgs sea)
        {
            if (sea.ColumnIndex == 6)
            {
                MessageBox.Show("Hyperlink Clicked");
            }
        }
        #endregion

        #region File Menu
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Edit Menu
        #endregion

        #region Action Menu
        #endregion

        #region Help Menu
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //
        }
        #endregion

        #region Nested Types
        public class GridStorage : IGridStorage
        {
            #region Variables
            private List<GridDataRow> mData;
            #endregion

            #region Constructor
            public GridStorage(List<GridDataRow> list)
            {
                this.mData = list;
            }
            #endregion

            #region Methods
            public long EnsureRowsInBuf(long FirstRowIndex, long LastRowIndex)
            {
                return 0L;
            }

            public void FillControlWithData(long nRowIndex, int nColIndex, IGridEmbeddedControl control)
            {
                GridDataRow row = this.mData[(int)nRowIndex];
                switch (nColIndex)
                {
                    case 2:
                        {
                            EmbeddedTextBox textBox = control as EmbeddedTextBox;
                            textBox.Text = row.Text;
                        }
                        break;
                    case 3:
                        {
                            EmbeddedComboBox comboBox = control as EmbeddedComboBox;
                            for (int i = 0; i < (int)this.NumRows(); i++)
                            {
                                comboBox.AddDataAsString(string.Format("Dropdown: {0}", i));
                            }
                            comboBox.Text = row.DropDownText;
                        }
                        break;
                    case 4:
                        {
                            EmbeddedComboBox comboBox = control as EmbeddedComboBox;
                            for(int i=0; i<(int)this.NumRows(); i++)
                            {
                                comboBox.AddDataAsString(string.Format("DropDownList: {0}", i));
                            }

                            comboBox.SelectedItem = row.DropDownListText;
                        }
                        break;
                    case 5:
                        {
                            EmbeddedSpinBox spinBox = control as EmbeddedSpinBox;
                            spinBox.Value = row.SpinValue;
                        }
                        break;
                    default:
                        break;
                }
            }

            public Bitmap GetCellDataAsBitmap(long nRowIndex, int nColIndex)
            {
                GridDataRow row = this.mData[(int)nRowIndex];
                return row.Image;
            }

            public string GetCellDataAsString(long nRowIndex, int nColIndex)
            {
                GridDataRow row = this.mData[(int)nRowIndex];

                switch (nColIndex)
                {
                    case 2:
                        return row.Text;
                    case 3:
                        return row.DropDownText;
                    case 4:
                        return row.DropDownListText;
                    case 5:
                        return string.Format("{0}", row.SpinValue);
                    case 6:
                        return row.HyperLinkText;
                    case 7:
                        return row.ButtonText;
                    default:
                        return string.Format("Col: {0}, Row: {1}", nColIndex, nRowIndex);
                }                
            }

            public void GetCellDataForButton(long nRowIndex, int nColIndex, out ButtonCellState state, out Bitmap image, out string buttonLabel)
            {
                GridDataRow row = this.mData[(int)nRowIndex];

                state = ButtonCellState.Normal;
                image = null;
                buttonLabel = row.ButtonText;
            }

            public GridCheckBoxState GetCellDataForCheckBox(long nRowIndex, int nColIndex)
            {
                GridDataRow row = this.mData[(int)nRowIndex];
                return row.IsChecked ? GridCheckBoxState.Checked : GridCheckBoxState.Unchecked;
            }

            public int IsCellEditable(long nRowIndex, int nColIndex)
            {
                switch(nColIndex)
                {
                    case 2:
                        return 1; // Text box
                    case 3:
                        return 2; // Dropdown
                    case 4:
                        return 3; // Dropdown List
                    case 5:
                        return 4; // spin
                    default:
                        return 0;
                }
            }

            public long NumRows()
            {
                return this.mData.Count;
            }

            public bool SetCellDataFromControl(long nRowIndex, int nColIndex, IGridEmbeddedControl control)
            {
                switch (nColIndex)
                {
                    case 2:
                        {
                            EmbeddedTextBox textBox = control as EmbeddedTextBox;
                            this.mData[(int)nRowIndex].Text = textBox.Text;
                        }
                        return true;
                    case 3:
                        {
                            EmbeddedComboBox comboBox = control as EmbeddedComboBox;
                            this.mData[(int)nRowIndex].DropDownText = comboBox.Text;
                        }
                        return true;
                    case 4:
                        {
                            EmbeddedComboBox comboBox = control as EmbeddedComboBox;
                            this.mData[(int)nRowIndex].DropDownListText = comboBox.SelectedItem.ToString();
                        }
                        return true;
                    case 5:
                        {
                            EmbeddedSpinBox spinBox = control as EmbeddedSpinBox;
                            this.mData[(int)nRowIndex].SpinValue = (int)spinBox.Value;
                        }
                        return true;
                    default:
                        break;
                }
                return false;
            }
            #endregion
        }
        #endregion
    }
}
