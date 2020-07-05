using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.SqlServer.Management.UI.Grid;
using S16.Utils;

namespace VspGridControlApp
{
    public partial class MainFrom : Form
    {
        #region Variables
        private Node<GridDataRow> mNode;
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
            this.gridControl.AddColumn(new GridColumnInfo() { ColumnType = GridColumnType.Bitmap, ColumnWidth = 5, IsUserResizable = false });
            this.gridControl.AddColumn(new GridColumnInfo() { ColumnType = GridColumnType.Tree, ColumnWidth = 50 });
            this.gridControl.AddColumn(new GridColumnInfo());
            this.gridControl.AddColumn(new GridColumnInfo());
            this.gridControl.AddColumn(new GridColumnInfo());
            this.gridControl.AddColumn(new GridColumnInfo());
            this.gridControl.AddColumn(new GridColumnInfo() { ColumnType = GridColumnType.Hyperlink });
            this.gridControl.AddColumn(new GridColumnInfo() { ColumnType = GridColumnType.Button, ColumnAlignment = HorizontalAlignment.Center });

            this.gridControl.SetHeaderInfo(0, "", GridCheckBoxState.Unchecked);
            this.gridControl.SetHeaderInfo(1, "", null);
            this.gridControl.SetHeaderInfo(2, "Tree", null);
            this.gridControl.SetHeaderInfo(3, "Text", null);
            this.gridControl.SetHeaderInfo(4, "Dropdown", null);
            this.gridControl.SetHeaderInfo(5, "DropdownList", null);
            this.gridControl.SetHeaderInfo(6, "Spin", null);
            this.gridControl.SetHeaderInfo(7, "Hyperlink", null);
            this.gridControl.SetHeaderInfo(8, "Button", null);

            this.gridControl.SelectionType = GridSelectionType.SingleRow;
            this.gridControl.FirstScrollableColumn = 3;

            this.gridControl.GridStorage = new GridStorage(this.mNode);

            this.gridControl.MouseButtonClicked += GridControl_MouseButtonClicked;
            this.gridControl.SelectionChanged += GridControl_SelectionChanged;
            this.gridControl.GridSpecialEvent += GridControl_GridSpecialEvent;
        }

        private void BuildData()
        {
            this.mNode = new Node<GridDataRow>(new GridDataRow());

            for(int i=0; i<10; i++)
            {
                Node<GridDataRow> node = this.mNode.Nodes.Add(new GridDataRow() 
                {
                    Image = Properties.Resources.favicon,
                    TreeText = string.Format("Tree: {0}", i),
                    Text = string.Format("Text: {0}", i),
                    DropDownText = string.Format("DropDown: {0}", i),
                    DropDownListText = string.Format("DropDownList: {0}", i),
                    SpinValue = i,
                    HyperLinkText = string.Format("HyperLink: {0}", i),
                    ButtonText = string.Format("Button: {0}", i)
                });
                this.AddChildren(node);
            }
        }

        private void AddChildren(Node<GridDataRow> pNode)
        {
            GridDataRow row = pNode.Value;
            for (int i = 0; i < 5; i++)
            {
                Node<GridDataRow> node = pNode.Nodes.Add(new GridDataRow()
                {
                    TreeText = string.Format("{0}, {1}", row.TreeText, i),
                    Text = string.Format("{0}, {1}", row.Text, i),
                    DropDownText = string.Format("{0}, {1}", row.DropDownText, i),
                    DropDownListText = string.Format("{0}, {1}", row.DropDownListText, i),
                    SpinValue = i,
                    HyperLinkText = string.Format("{0}, {1}", row.HyperLinkText, i),
                    ButtonText = string.Format("{0}, {1}", row.ButtonText, i)
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
            if (args.ColumnIndex == 8)
            {
                MessageBox.Show("Button Clicked");
            }
        }

        private void GridControl_GridSpecialEvent(object sender, GridSpecialEventArgs sea)
        {
            if (sea.ColumnIndex == 7)
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
            private Node<GridDataRow>[] mNodes;
            #endregion

            #region Constructor
            public GridStorage(Node<GridDataRow> node)
            {
                this.mNodes = node.GetAllNodes(true);
            }
            #endregion

            public long EnsureRowsInBuf(long FirstRowIndex, long LastRowIndex)
            {
                return 0L;
            }

            public void FillControlWithData(long nRowIndex, int nColIndex, IGridEmbeddedControl control)
            {
                GridDataRow row = this.mNodes[nRowIndex];
                switch (nColIndex)
                {
                    case 3:
                        {
                            EmbeddedTextBox textBox = control as EmbeddedTextBox;
                            textBox.Text = row.Text;
                        }
                        break;
                    case 4:
                        {
                            EmbeddedComboBox comboBox = control as EmbeddedComboBox;
                            for (int i = 0; i < (int)this.NumRows(); i++)
                            {
                                comboBox.AddDataAsString(string.Format("Dropdown: {0}", i));
                            }
                            comboBox.Text = row.DropDownText;
                        }
                        break;
                    case 5:
                        {
                            EmbeddedComboBox comboBox = control as EmbeddedComboBox;
                            for (int i = 0; i < (int)this.NumRows(); i++)
                            {
                                comboBox.AddDataAsString(string.Format("DropDownList: {0}", i));
                            }

                            comboBox.SelectedItem = row.DropDownListText;
                        }
                        break;
                    case 6:
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
                GridDataRow row = this.mNodes[nRowIndex];
                return row.Image;
            }

            public string GetCellDataAsString(long nRowIndex, int nColIndex)
            {
                GridDataRow row = this.mNodes[nRowIndex];

                switch (nColIndex)
                {
                    case 3:
                        return row.Text;
                    case 4:
                        return row.DropDownText;
                    case 5:
                        return row.DropDownListText;
                    case 6:
                        return string.Format("{0}", row.SpinValue);
                    case 7:
                        return row.HyperLinkText;
                    case 8:
                        return row.ButtonText;
                    default:
                        return string.Format("Col: {0}, Row: {1}", nColIndex, nRowIndex);
                }                
            }

            public void GetCellDataForButton(long nRowIndex, int nColIndex, out ButtonCellState state, out Bitmap image, out string buttonLabel)
            {
                GridDataRow row = this.mNodes[nRowIndex];

                state = ButtonCellState.Normal;
                image = null;
                buttonLabel = row.ButtonText;
            }

            public GridCheckBoxState GetCellDataForCheckBox(long nRowIndex, int nColIndex)
            {
                GridDataRow row = this.mNodes[nRowIndex];
                return row.IsChecked ? GridCheckBoxState.Checked : GridCheckBoxState.Unchecked;
            }

            public void GetCellDataForTree(long nRowIndex, int nColIndex, out int level, out bool expanded, out bool hasChildren, out string label)
            {
                Node<GridDataRow> row = this.mNodes[nRowIndex];

                level = row.Level - 1;
                expanded = true;
                hasChildren = row.HasChildren;
                label = row.Value.TreeText;
            }

            public int IsCellEditable(long nRowIndex, int nColIndex)
            {
                switch(nColIndex)
                {
                    case 3:
                        return 1; // Text box
                    case 4:
                        return 2; // Dropdown
                    case 5:
                        return 3; // Dropdown List
                    case 6:
                        return 4; // spin
                    default:
                        return 0;
                }
            }

            public long NumRows()
            {
                return this.mNodes.Length;
            }

            public bool SetCellDataFromControl(long nRowIndex, int nColIndex, IGridEmbeddedControl control)
            {
                switch (nColIndex)
                {
                    case 3:
                        {
                            EmbeddedTextBox textBox = control as EmbeddedTextBox;
                            this.mNodes[(int)nRowIndex].Value.Text = textBox.Text;
                        }
                        return true;
                    case 4:
                        {
                            EmbeddedComboBox comboBox = control as EmbeddedComboBox;
                            this.mNodes[(int)nRowIndex].Value.DropDownText = comboBox.Text;
                        }
                        return true;
                    case 5:
                        {
                            EmbeddedComboBox comboBox = control as EmbeddedComboBox;
                            this.mNodes[(int)nRowIndex].Value.DropDownListText = comboBox.SelectedItem.ToString();
                        }
                        return true;
                    case 6:
                        {
                            EmbeddedSpinBox spinBox = control as EmbeddedSpinBox;
                            this.mNodes[(int)nRowIndex].Value.SpinValue = (int)spinBox.Value;
                        }
                        return true;
                    default:
                        break;
                }
                return false;
            }
        }
        #endregion
    }
}
