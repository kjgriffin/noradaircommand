using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Norad_AirCommand
{
    enum MouseMode
    {
        Normal,
        DestinationSelection,
        PlaneSelection,
        hittest,
        GuardPoint,

    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Show();
            gfx = CreateGraphics();
        }

        MouseMode mousemode = MouseMode.Normal;
        Universe u = new Universe();
        Plane selectedplane;
        Plane selectedPlane
        {
            get { return selectedplane; }
            set
            {
                try { selectedplane.isSelected = false; }
                catch (Exception) { }
                selectedplane = value;
                selectedplane.isSelected = true;
                updateUI(selectedplane);
            }
        }


        //Handles the graphics functions
        #region Graphics
        //variables
        Graphics gfx;
        Graphics bbg;
        static Size canvasSize = new Size(2000, 2000);
        static Size displaySize = new Size(100, 100);
        Bitmap bb = new Bitmap(canvasSize.Width, canvasSize.Height);
        //Camera Location
        Rectangle Camera = new Rectangle(0, 0, 200, 200);
        //display settings
        UniverseDisplaySettings us = new UniverseDisplaySettings();

        private void Form1_Load(object sender, EventArgs e)
        {
            Camera.Width = vScrollBar1.Location.X - 4;
            Camera.Height = hScrollBar1.Location.Y - 4;
        }


        /// <summary>
        /// Drawing Logic Here
        /// </summary>
        public void Draw()
        {
            //draw logic here
            //use gfx.draw


            u.Draw(gfx, us);

            //graphics handoff
            gfx = Graphics.FromImage(bb);
            bbg = CreateGraphics();
            bbg.DrawImage(bb, new Rectangle(0, 0, Camera.Width, Camera.Height), Camera, GraphicsUnit.Pixel);
            gfx.Clear(Color.Black);
        }



        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            Draw();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Plane p = new Plane(new PointF(20, 20), 4.3f, 90, new PointF(400, 350), Color.Blue, "Blue");
            u.Entities.Add(p);
            selectedPlane = p;
            Draw();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            //scale mouse to camera location
            PointF logicalpos = new PointF(e.X + Camera.X, e.Y + Camera.Y);

            switch (mousemode)
            {
                case MouseMode.Normal:
                    break;
                case MouseMode.GuardPoint:
                    selectedPlane.AITarget.GuardPoint = logicalpos;
                    Draw();
                    mousemode = MouseMode.Normal;
                    break;
                case MouseMode.PlaneSelection:
                    selectedPlane = u.find_ClosestPlane(logicalpos);
                    Draw();
                    mousemode = MouseMode.Normal;
                    break;
                case MouseMode.hittest:
                    //see if point is inside planes triangel
                    if (selectedPlane.Location.CreateHitTriangle(30, selectedPlane.Orientation, 20).IsPointInPolygon(logicalpos))
                        MessageBox.Show("hit");
                    mousemode = MouseMode.Normal;
                    break;
                case MouseMode.DestinationSelection:
                    //set the destination of the plane
                    selectedPlane.Destination = logicalpos;
                    Draw();
                    mousemode = MouseMode.Normal;
                    break;
                default:
                    break;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            mousemode = MouseMode.DestinationSelection;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            mousemode = MouseMode.hittest;
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            Camera.Y = (canvasSize.Height - Camera.Height) / 100 * vScrollBar1.Value;
            Draw();
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            Camera.X = (canvasSize.Width - Camera.Width) / 100 * hScrollBar1.Value;
            Draw();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            u.Update();
            Draw();
            updateUI(selectedPlane);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //change displayres
            Camera.Width = vScrollBar1.Location.X - 4;
            Camera.Height = hScrollBar1.Location.Y - 4;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Camera.Width += 5;
            Camera.Height += 5;
            Draw();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Camera.Width -= 5;
            Camera.Height -= 5;
            Draw();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            mousemode = MouseMode.PlaneSelection;
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            Plane p = new Plane(new PointF(20, 20), 4.3f, 90, new PointF(400, 350), Color.Red, "Red");
            u.Entities.Add(p);
            selectedPlane = p;
            Draw();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            selectedPlane.AITarget.minchasedistance = float.Parse(tbMaxChaseDistance.Text, CultureInfo.InvariantCulture.NumberFormat);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            selectedPlane.AITarget.maxfueluse = float.Parse(tbMaxFuelConsumption.Text, CultureInfo.InvariantCulture.NumberFormat);
        }

        private void cbGTFOmode_CheckedChanged(object sender, EventArgs e)
        {
            selectedPlane.AITarget.GTFOmode = cbGTFOmode.Checked;
        }

        private void cbDisengagetoConserveFuel_CheckedChanged(object sender, EventArgs e)
        {
            selectedPlane.AITarget.ConserveFuelmode = cbDisengagetoConserveFuel.Checked;
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            mousemode = MouseMode.GuardPoint;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            selectedPlane.AITarget.GuardAreaRadius = float.Parse(tbGuardAreaRadius.Text);
        }

        #region AutoTargetMode

        private void set_AutoTargetMode()
        {

            if (cbEnableAutoTarget.Checked)
                selectedPlane.AITarget.Enabled = true;
            else
                selectedPlane.AITarget.Enabled = false;
            if (rbGMClosest.Checked)
                selectedPlane.AITarget.Mode = TargetMode.Closest;
            else if (rbGMFurthest.Checked)
                selectedPlane.AITarget.Mode = TargetMode.Furthest;
            else if (rbGMGuard.Checked)
                selectedPlane.AITarget.Mode = TargetMode.Guard;
            else if (rbGMGuardtoPursue.Checked)
                selectedPlane.AITarget.Mode = TargetMode.GuardToPursue;
            else if (rbGMReturn.Checked)
                selectedPlane.AITarget.Mode = TargetMode.Return;

        }

        private void cbEnableAutoTarget_Click(object sender, EventArgs e)
        {
            set_AutoTargetMode();
        }

        private void rbGMReturn_Click(object sender, EventArgs e)
        {
            set_AutoTargetMode();
        }

        private void rbGMFurthest_Click(object sender, EventArgs e)
        {
            set_AutoTargetMode();
        }

        private void rbGMGuard_Click(object sender, EventArgs e)
        {
            set_AutoTargetMode();
        }

        private void rbGMGuardtoPursue_Click(object sender, EventArgs e)
        {
            set_AutoTargetMode();
        }

        private void rbGMClosest_Click(object sender, EventArgs e)
        {
            set_AutoTargetMode();
        }

        private void cbGMClosest_Click(object sender, EventArgs e)
        {
            selectedPlane.AITarget.GuardClosest = cbGMClosest.Checked;
        }

        private void cbGMFurthest_Click(object sender, EventArgs e)
        {
            selectedPlane.AITarget.GuardClosest = cbGMClosest.Checked;
        }

        private void cbReturnToGuard_Click(object sender, EventArgs e)
        {
            selectedPlane.AITarget.ReturnToGuard = cbReturnToGuard.Checked;
        }

        #endregion

        #region updateUI

        void updateUI(Plane sp)
        {
            //update target modes
            cbEnableAutoTarget.Checked = sp.AITarget.Enabled;
            switch (sp.AITarget.Mode)
            {
                case TargetMode.Closest:
                    rbGMClosest.Checked = true;
                    break;
                case TargetMode.Furthest:
                    rbGMFurthest.Checked = true;
                    break;
                case TargetMode.Guard:
                    rbGMGuard.Checked = true;
                    break;
                case TargetMode.GuardToPursue:
                    rbGMGuardtoPursue.Checked = true;
                    break;
                case TargetMode.Return:
                    rbGMReturn.Checked = true;
                    break;
                default:
                    break;
            }
            //update chase parameters
            cbDisengagetoConserveFuel.Checked = sp.AITarget.ConserveFuelmode;
            cbReturnToGuard.Checked = sp.AITarget.ReturnToGuard;
            cbGTFOmode.Checked = sp.AITarget.GTFOmode;
            tbMaxChaseDistance.Text = sp.AITarget.minchasedistance.ToString();
            tbMaxFuelConsumption.Text = sp.AITarget.maxfueluse.ToString();

            //guard mode
            tbGuardAreaRadius.Text = sp.AITarget.GuardAreaRadius.ToString();
            cbGMClosest.Checked = sp.AITarget.GuardClosest;
            cbGMFurthest.Checked = !sp.AITarget.GuardClosest;

            //airplane weapons
            cbAutoEngageEnabled.Checked = sp.AIEngage.Enabled;
            cbAutoTargetPreferShortRange.Checked = sp.AIEngage.PreferShortRange;
            if (sp.AIEngage.SRIRlock)
                pbSRIR.BackColor = Color.Green;
            else
                pbSRIR.BackColor = Color.Red;
            if (sp.AIEngage.SRRlock)
                pbSRR.BackColor = Color.Green;
            else
                pbSRR.BackColor = Color.Red;
            if (sp.AIEngage.MRRlock)
                pbMRR.BackColor = Color.Green;
            else
                pbMRR.BackColor = Color.Red;

            if (sp.AIEngage.Enabled)
                pbAutoEngageSignalStrength.Value = (int)(sp.AITarget.targetPassiveSignalStrength * 10);
            else
                pbAutoEngageSignalStrength.Value = 0;

            switch (sp.AIEngage.Mode)
            {
                case AutoEngageMode.AutoSelect:
                    rbAutoEngageAutoSelect.Checked = true;
                    break;
                case AutoEngageMode.Infared:
                    rbAutoEngageInfared.Checked = true;
                    break;
                case AutoEngageMode.Radar:
                    rbAutoEngageRadar.Checked = true;
                    break;
                default:
                    break;
            }

            switch (sp.AIEngage.Status)
            {
                case AutoEngageStatus.NoTarget:
                    lbAutoEngageStatus.Text = "no target";
                    lbAutoEngageStatus.ForeColor = Color.Red;
                    break;
                case AutoEngageStatus.Scanning:
                    lbAutoEngageStatus.Text = "Scanning";
                    lbAutoEngageStatus.ForeColor = Color.Yellow;
                    break;
                case AutoEngageStatus.Tracking:
                    lbAutoEngageStatus.Text = "Tracking";
                    lbAutoEngageStatus.ForeColor = Color.LimeGreen;
                    break;
                case AutoEngageStatus.Resolving:
                    lbAutoEngageStatus.Text = "Resolving";
                    lbAutoEngageStatus.ForeColor = Color.Orange;
                    break;
                case AutoEngageStatus.Locked:
                    lbAutoEngageStatus.Text = "Locked";
                    lbAutoEngageStatus.ForeColor = Color.Green;
                    break;
                default:
                    break;
            }

            //weapons display
            clbWeapons.Items.Clear();
            foreach (Missile m in selectedPlane.Missiles)
            {
                clbWeapons.Items.Add(m.Type, m.Active);
            }

            //general aircraft stats
            pbPlaneFuel.Value = (int)(sp.Fuel / sp.MaxFuel * 100);
            lbPlaneFuelActual.Text = sp.Fuel.ToString();
            lbPlaneTeam.Text = "Team: " + sp.Team;

        }

        #endregion

        #region UniverseDisplaySettings

        private void cbHighlightSelection_CheckedChanged(object sender, EventArgs e)
        {
            us.HighightSelectedPlane = cbHighlightSelection.Checked;
        }

        private void cbTrails_CheckedChanged(object sender, EventArgs e)
        {
            us.AllTrail = cbTrails.Checked;
        }

        private void cbDestination_CheckedChanged(object sender, EventArgs e)
        {
            us.AllDestination = cbDestination.Checked;
        }

        private void cbLRRadar_CheckedChanged(object sender, EventArgs e)
        {
            us.AllLRRadar = cbLRRadar.Checked;
        }

        private void cbMRRadar_CheckedChanged(object sender, EventArgs e)
        {
            us.AllMRRadar = cbMRRadar.Checked;
        }

        private void cbSRRadar_CheckedChanged(object sender, EventArgs e)
        {
            us.AllSRRadar = cbSRRadar.Checked;
        }

        private void cbIRRadar_CheckedChanged(object sender, EventArgs e)
        {
            us.AlllIRRadar = cbIRRadar.Checked;
        }

        private void cbGunSight_CheckedChanged(object sender, EventArgs e)
        {
            us.AllGunSight = cbGunSight.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            us.AllGuardZone = cbGuardZones.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            us.DebugHeatSignatures = checkBox3.Checked;
        }


        #endregion

        private void button13_Click(object sender, EventArgs e)
        {
            //adds a missile to the current plane
            //debug only
            Missile m = new Missile(MissileType.srir);
            //default missile specs

            selectedplane.Missiles.Add(m);
        }




        #region AutoEngageMode







        #endregion

        private void cbAutoEngageEnabled_CheckedChanged(object sender, EventArgs e)
        {
            selectedplane.AIEngage.Enabled = cbAutoEngageEnabled.Checked;
        }

        private void clbWeapons_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (Missile m  in selectedPlane.Missiles)
            {
                m.Active = false;
            }
            for (int i = 0; i < clbWeapons.CheckedIndices.Count; i++)
            {
                selectedplane.Missiles[clbWeapons.CheckedIndices[i]].Active = true;
            }
        }

        private void clbWeapons_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void clbWeapons_Click(object sender, EventArgs e)
        {

        }

        private void clbWeapons_ItemCheck(object sender, ItemCheckEventArgs e)
        {

        }
    }
}
