﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotSpatialMap.Presenters;
using DotSpatialMap.Models;
using DotSpatial.Symbology;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using DotSpatial.Data;
using System.Threading;

namespace DotSpatialMap.Views
{
    public partial class MainView : Form,IMainView,DrawingToolsBox,IGeoCoordinatesLabel
    {

        private MainViewPresenter Presenter;
        private string SelectedLayerName;
        
        public MainView()
        {
            
            InitializeComponent();
            //this.WindowState = FormWindowState.Maximized;




            Map map = new Map(Map);
            Presenter = new MainViewPresenter(this, map);
            Presenter.addPresenter(new DrawingToolsBoxPresenter(this));
            Presenter.addPresenter(new GeoCoordinatesPresenter(this));

            Presenter.setModel(map);


            Map.Layers.LayerSelected += handle_Layer_Selected_Changed;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            InvokeInitialize(new EventArgs());

            
        }
      

        #region IMainView

        string IMainView.SelelectedLayerName
        {
            get
            {
                return Map.Layers.SelectedLayer.LegendText;
            }
            set
            {
                //SelectedLayerName = Map.Layers.SelectedLayer.LegendText;
            }
        }

        public event EventHandler Initialize;
        public event EventHandler AddEmptyLayer;
        public event EventHandler ExecuteTopologicalQuery;

        private void addEmptyLayerBtn_Click(object sender, EventArgs e)
        {

            var handler = AddEmptyLayer;
            if (handler != null)
            {
                handler(this, e);
            }
        }


        #endregion IMainView


        #region DrawingToolBox
        public bool PolygonDrawingToolEnabled
        {

            get
            {
                return drawPolygonBtn.Enabled;
            }
            set
            {
                drawPolygonBtn.Enabled = value;
            }
        }
        public bool LineDrawingToolEnabled { get => drawLineBtn.Enabled; set => drawLineBtn.Enabled = value; }
        public bool PointDrawingToolEnabled { get => drawPointBtn.Enabled; set => drawPointBtn.Enabled = value; }

        public bool PolygonDrawingToolChecked { get => drawPolygonBtn.Checked; set => drawPolygonBtn.Checked = value; }
        public bool PointDrawingToolChecked { get => drawPointBtn.Checked; set => drawPointBtn.Checked = value; }
        public bool LineDrawingToolChecked { get => drawLineBtn.Checked; set => drawLineBtn.Checked = value; }
        private string itemClicked;
        public string ItemClicked { get => itemClicked; }


        public Graphics graphics => Graphics.FromHwnd(Map.Handle);

        public event EventHandler<LayerSelectedEventArgs> SelectedLayerChanged;
        public event EventHandler Draw;
        public event EventHandler AddPoint;
        public event EventHandler StopDrawing;
        public event EventHandler CancelDrawing;
        public event EventHandler DrawPolygon;
        public event EventHandler DrawLine;
        public event EventHandler DrawPoint;
        public event EventHandler StopDrawingShapes;
        public event EventHandler RemoveLastPoint;



        #endregion DrawingToolBox


        #region IGeoCoordinateLabel
        public string GeoLocation { set => curruntCoordinates.Text = value; }
        public Cursor MapCursor { set => Map.Cursor = value; }

        public event EventHandler CoordinateChanged;
        #endregion IGeoCoordinateLabel


        #region Handle Methodes
        public void InvokeInitialize(EventArgs e)
        {
            EventHandler handler = Initialize;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private void handle_Layer_Selected_Changed(object sender, LayerSelectedEventArgs e)
        {
            var handler = SelectedLayerChanged;

            //  MessageBox.Show(Map.Layers.SelectedLayer.ToString());
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private void Map_DoubleClick(object sender, EventArgs e)
        {
            var handler = StopDrawing;
            if (handler == null)
            {
                return;
            }

            handler(this, e);
        }
        private void handle_Geo_Mosue_Move(object sender, DotSpatial.Controls.GeoMouseArgs e)
        {
            
            var handler = Draw;
            Map.Invalidate();
            Map.Update();
            handler += CoordinateChanged;
             // Refresh();
            
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private void draw_Polygon_Btn_Checked(object sender, EventArgs e)
        {
            var handler = DrawPolygon;
            if (handler == null)
                return;
            handler(this, e);
        }
        private void draw_Line_Btn_Checked(object sender, EventArgs e)
        {
            var handler = DrawLine;
            if (handler == null)
                return;
            handler(this, e);
        }
        private void draw_Point_Btn_Checked(object sender, EventArgs e)
        {
            var handler = DrawPoint;
            if (handler == null)
                return;
            handler(this, e);

        }
        private void Map_Click(object sender, EventArgs e)
        {
            EventHandler handler;
            MouseEventArgs args = (MouseEventArgs)e;
            if(args.Button == MouseButtons.Left)
            {
                handler = AddPoint;
                if (handler == null)
                {
                    return;
                }
            }
            else
            {
                handler = RemoveLastPoint;
                if(handler == null)
                {
                    return;
                }
            }

            handler(this, e);
        }
        #endregion Handle Methodes

        private void spatialToolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //            MessageBox.Show(e.GetHashCode+" ");


            // IFeatureSet fs = FeatureSet.Open("C:\\Temp\\roads.shp");

            
        }

        private void App_MapChanged(object sender, DotSpatial.Controls.MapChangedEventArgs e)
        {
            MessageBox.Show("salam");
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Map.Layers.LayerSelected += handle_Layer_Selected_Changed;

        }

        private void spatialToolStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            
            setItemClicked(e);
            var handler = CancelDrawing;     
            if (handler == null)
            {
                return;
            }
            handler(this, e);
        }

        private void setItemClicked(ToolStripItemClickedEventArgs e)
        {
            var clicked = e.ClickedItem.Name;
            if (clicked == "drawPolygonBtn") { itemClicked = "POLYGON"; }
            else if  (clicked == "drawLineBtn") {itemClicked = "LINE"; }
            else if (clicked == "drawPointBtn") { itemClicked = "Point"; }
            else
            {
                itemClicked = clicked;
            }
               
        }

        private void Map_Paint(object sender, PaintEventArgs e)
        {
            
            //Map.MapFrame.Draw(e);
            
        }

        private void Map_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            var handler = ExecuteTopologicalQuery;
            if (handler == null) return;
            handler(this, e);
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Map.AddLayer();

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to close this application ?", "Admin", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                this.Close();
            }
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Map.ZoomIn();

        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Map.ZoomOut();

        }

        private void zoomToExtentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Map.ZoomToMaxExtent();

        }

        private void createShapeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var handler = AddEmptyLayer;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }

    
}
