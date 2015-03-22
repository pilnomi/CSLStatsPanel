using ICities;
using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CSLStatsPanel
{

    class StatusWindowInterface
    {
        public static bool initialized = false;
        //public static UIView uiView;
        public static CSLStatsMasterWindow myStatsWindowPanel;
        public static bool running = false;

        public static void init()
        {
            if (initialized) return;
            if (running) return;
            running = true;

            UIView uiView = GameObject.FindObjectOfType<UIView>();
            if (uiView == null) return;

            myStatsWindowPanel = (CSLStatsMasterWindow)UIView.GetAView().AddUIComponent(typeof(CSLStatsMasterWindow));
            myStatsWindowPanel.name = "CSLStatsMasterPanel";

            initialized = true;
            running = false;

        }
        public static void reset()
        {
            myStatsWindowPanel = null;
            initialized = false;
            running = false;
        }

        public static void updateText(List<string> TextFields)
        {
            if (!initialized) return;
            if (running) return;
            running = true;
            myStatsWindowPanel.updateText(TextFields);
            running = false;
        }
        public static void updateText()
        {
            if (!initialized) return;
            if (running) return;
            running = true;
            myStatsWindowPanel.getstats2();
            running = false;
        }
    }

    public class CSLStatsMasterWindow : UIPanel
    {
        CSLStatusWindowPanel myStatsWindowPanel;
        UIResizeHandle myresizepanel;
        UIPanel headerpanel;
        UILabel resizelabel, headertext;
        bool firstrun = true;
        bool dragging = false, resizing = false;
        public SavedFloat
            windowx = new SavedFloat("ModCSLStatsPanelWindowPosX", Settings.gameSettingsFile, 0, true),
            windowy = new SavedFloat("ModCSLStatsPanelWindowPosY", Settings.gameSettingsFile, 0, true),
            windoww = new SavedFloat("ModCSLStatsPanelWindowPosW", Settings.gameSettingsFile, 700, true),
            windowh = new SavedFloat("ModCSLStatsPanelWindowPosH", Settings.gameSettingsFile, 400, true);

        public CSLStatsMasterWindow()
        {
            init();
        }

        public void init()
        {
            this.color = new Color32(0, 0, 255, 200);
            this.backgroundSprite = "GenericPanel";
            this.autoLayoutDirection = LayoutDirection.Vertical;
            this.autoLayoutStart = LayoutStart.TopLeft;
            this.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
            this.autoLayout = true;

            headerpanel = (UIPanel)this.AddUIComponent(typeof(UIPanel));
            headerpanel.height = 20;
            headerpanel.backgroundSprite = "GenericPanel";
            headerpanel.color = new Color32(0, 0, 100, 100);
            
            headertext = headerpanel.AddUIComponent<UILabel>();
            headertext.text = "CSL Stats Panel";
            headertext.CenterToParent();

            myStatsWindowPanel = (CSLStatusWindowPanel)this.AddUIComponent(typeof(CSLStatusWindowPanel));
            myStatsWindowPanel.name = "CSLStatsPanel";
            myStatsWindowPanel.color = new Color32(0, 0, 255, 200);
            myresizepanel = (UIResizeHandle)this.AddUIComponent(typeof(UIResizeHandle));
            myresizepanel.name = "CSLStatsResizePanel";
            myresizepanel.height = 20;
            myresizepanel.color = new Color32(0, 0, 100, 100);
            myresizepanel.backgroundSprite = "GenericPanel";
            //myresizepanel.anchor = UIAnchorStyle.Bottom;
            //myresizepanel.anchor = UIAnchorStyle.Right;
            resizelabel = myresizepanel.AddUIComponent<UILabel>();

            setdefaultpos();
        }

        void setdefaultpos()
        {
            this.position = new Vector3(windowx.value, windowy.value, this.position.z);
            this.width = windoww.value;
            this.height = windowh.value;
            if (this.width == 0) this.width = 700;
            if (this.height == 0) this.height = 400;
            OnSizeChanged();
        }

        public void updateText(List<string> s) { myStatsWindowPanel.updateText(s);}

        protected override void OnSizeChanged()
        {
            
            base.OnSizeChanged();
            headerpanel.width = this.width;
            myresizepanel.width = this.width;
            myStatsWindowPanel.width = this.width;
            myStatsWindowPanel.height = this.height - headerpanel.height - myresizepanel.height;
            resizelabel.CenterToParent();
            headertext.CenterToParent();

            if (!firstrun)
            {
                windowx.value = this.position.x; windowy.value = this.position.y;
                windoww.value = this.width; windowh.value = this.height;
            }
            //myresizepanel.anchor = UIAnchorStyle.Right;
            //myresizepanel.anchor = UIAnchorStyle.Bottom;
            //this.FitChildrenVertically();
            //this.FitChildrenHorizontally();

        }

        public void getstats2()
        {
            myStatsWindowPanel.getstats2();
            if (firstrun)
            {
                setdefaultpos();
                OnSizeChanged();
            }
            firstrun = false;
        }

        bool checkresizebox(UIMouseEventParameter p)
        {
            
            if (p.position.x >= this.relativePosition.x + this.width - 10
                && p.position.y >= this.relativePosition.y + this.height - 10)
                return true;
            return false;
        }
        protected override void OnMouseDown(UIMouseEventParameter p)
        {

            if (checkresizebox(p)) resizing = true;
            else if (!resizing) dragging = true;
        }
        protected override void OnMouseUp(UIMouseEventParameter p)
        {
            dragging = false; resizing = false;
        }
        protected override void OnMouseMove(UIMouseEventParameter p)
        {
            //resizelabel.text = string.Format("x{0} y{1} w{2} h{3} px{4} py{5} r{6}", this.relativePosition.x,
            //    this.relativePosition.y, this.width, this.height, p.position.x, p.position.y,
            //    checkresizebox(p));
            if (dragging) this.position = new Vector3(this.position.x + p.moveDelta.x,
             this.position.y + p.moveDelta.y,
             this.position.z);

            if (resizing)
            {
                this.width += p.moveDelta.x;
                this.height -= p.moveDelta.y;
                myStatsWindowPanel.width = this.width;
                myStatsWindowPanel.height = this.height - 20;
                myresizepanel.width = this.width;
                myresizepanel.height = 20;
            }
        }
    }

    public class CSLStatusWindowSubPanel : CSLStatusWindowPanel
    {
        public CSLStatusWindowSubPanel()
        {
            m_issubpanel = true;
            base.m_issubpanel = true;
            this.autoSize = true;
            this.color = new Color32(225, 225, 225, 200);

        }

        public override void Start()
        {
            statlog.log("sub panel started");
            m_issubpanel = true;
            base.m_issubpanel = true;
            base.Start();
        }
    }

    public class CSLStatusWindowPanel : UIScrollablePanel
    {
        public bool initialized = false, running = false;
        public int mycount = 0;
        public bool m_issubpanel = false;
        public List<string> m_stringbuilder = new List<string>();
        public Dictionary<string, CSLStatusWindowSubPanel> m_categories = new Dictionary<string, CSLStatusWindowSubPanel>();
        public List<CSLStatsPanelLabel> m_textfields = new List<CSLStatsPanelLabel>();
        bool firstrun = true;

        public CSLStatusWindowPanel()
        {
            this.backgroundSprite = "GenericPanel";
            this.autoLayoutDirection = LayoutDirection.Vertical;
            this.autoLayoutStart = LayoutStart.TopLeft;
            this.autoLayoutPadding = new RectOffset(1, 1, 1, 1);
            this.autoLayout = true;

            m_textfields = new List<CSLStatsPanelLabel>();
        }

        public void init()
        {
            if (initialized) return;
            statlog.log("CLSStatusWindowPanel init - subpanel=" + m_issubpanel.ToString());
            if (!this.m_issubpanel)
            {
                this.freeScroll = true;
                this.clipChildren = true;
                this.width = 300;
                this.height = 400;
                this.wrapLayout = true;
            }
            initialized = true;
        }

        public override void Start()
        {
            init();
        }

        public void updateText(List<StatisticsClassWrapper> TextFields)
        {
            statlog.log("update text scw initialized=" + initialized.ToString() + " running=" + running.ToString());
            if (!initialized) return;
            if (running) return;
            running = true;

            statlog.log("reseting stringbuilders");
            foreach (KeyValuePair<string, CSLStatusWindowSubPanel> p in m_categories)
                p.Value.m_stringbuilder = new List<string>();

            statlog.log("looping stats");
            for (int i = 0; i < TextFields.Count; i++)
            {
                string currentcat = TextFields[i].category;
                if (string.IsNullOrEmpty(currentcat)) currentcat = "default";

                if (!m_categories.Keys.Contains(currentcat))
                {
                    statlog.log("adding category " + currentcat);
                    m_categories.Add(currentcat, (CSLStatusWindowSubPanel)this.AddUIComponent(typeof(CSLStatusWindowSubPanel)));
                }

                if (m_categories[currentcat].m_stringbuilder.Count()==0)
                    m_categories[currentcat].m_stringbuilder.Add(currentcat);

                m_categories[currentcat].m_stringbuilder.Add(TextFields[i].statstring);
            }

            foreach (KeyValuePair<string, CSLStatusWindowSubPanel> p in m_categories)
            {
                statlog.log("calling updatetext on subpanel " + p.Key);
                p.Value.updateText(p.Value.m_stringbuilder);
                p.Value.m_stringbuilder = new List<string>();
            }
            if (firstrun && !m_issubpanel)
            {
                //UIResizeHandle rh = (UIResizeHandle)this.AddUIComponent(typeof(UIResizeHandle));
                //UILabel resizelabel = rh.AddUIComponent<UILabel>();
                //resizelabel.text = "resize me";
                //resizelabel.autoSize = true;
                this.FitChildrenVertically();
                this.FitChildrenHorizontally();
                this.FitToContents();
            }
            firstrun = false;
            running = false;
        }

        public void updateText(List<string> TextFields)
        {
            statlog.log("update text initialized=" + initialized.ToString() + " running=" + running.ToString());
            if (!initialized) init();
            if (running) return;
            running = true;
            string s = ""; bool usesinglefield = false; // <-- work around uses a single label
            bool labelsadded = false;
            for (int i = 0;  i < TextFields.Count; i++)
            {
                if (i >= m_textfields.Count && (!usesinglefield || i == 0))
                {
                    labelsadded = true;
                    m_textfields.Add(this.AddUIComponent<CSLStatsPanelLabel>());
                    m_textfields[i].name = "CSLStatsLabel_" + i.ToString();
                    statlog.log("creating label " + m_textfields[i].name);
                }
                else if (usesinglefield) s += TextFields[i] + "\n";
                    
                if (!usesinglefield)
                {
                    statlog.log("setting name: " + m_textfields[i].name + " text:  " + TextFields[i]);
                    m_textfields[i].text = TextFields[i];
                    
                }

            }

            if (usesinglefield) m_textfields[0].text = s;
            if (this.m_issubpanel ) //&& labelsadded)
            {
                this.FitChildrenVertically();
                //this.FitChildrenHorizontally();
                this.FitToContents();
                //this.FitChildren();
            }
            running = false;
        }

        ~CSLStatusWindowPanel()
        {

            m_textfields = new List<CSLStatsPanelLabel>();
            initialized = false;
            running = false;
        }

        public void getstats2()
        {
            if (!initialized) init();
            SimulationManager simManager = Singleton<SimulationManager>.instance;
            if (simManager == null) return;

            List<string> mystrings = new List<string>();

            mystrings.Add("CSL Stats Panel " + DateTime.Now.ToString("MM/dd/yy H:mm:ss"));

            NetManager nm = Singleton<NetManager>.instance;
            int firecoverage = 0; 
            for (int i = 0; i < nm.m_segments.m_buffer.Count(); i++)
            {
                if (!nm.m_segments.m_buffer[i].m_flags.IsFlagSet(NetSegment.Flags.Created)) continue;
                firecoverage += nm.m_segments.m_buffer[i].m_fireCoverage;
                
            }
            
            BuildingManager bm = Singleton<BuildingManager>.instance;
            int electricbuffer = 0, onfire=0,firehazard=0, crimebuffer=0, garbagebuffer=0;
            for (int i = 0; i < bm.m_buildings.m_buffer.Count(); i++)
            {
                if (!bm.m_buildings.m_buffer[i].m_flags.IsFlagSet(Building.Flags.Active)) continue;
                if (!bm.m_buildings.m_buffer[i].m_flags.IsFlagSet(Building.Flags.Created)) continue;

                electricbuffer += bm.m_buildings.m_buffer[i].m_electricityBuffer;
                if (bm.m_buildings.m_buffer[i].m_fireIntensity > 0) onfire++;
                firehazard += (int)bm.m_buildings.m_buffer[i].m_fireHazard;
                crimebuffer += bm.m_buildings.m_buffer[i].m_crimeBuffer;
                garbagebuffer += bm.m_buildings.m_buffer[i].m_garbageBuffer;
            }
            double waterbuffer = 0, sewagebuffer = 0, watercapacity = 0, sewagecapacity = 0,
                garbage=0;

            List<StatisticsClassWrapper> statstopull = new List<StatisticsClassWrapper>();
            
            DistrictManager dm = Singleton<DistrictManager>.instance;
            int dmcount = 0;
            int dmusage = 0, dmproduction = 0;
            int finalcrimerate = 0, citizencount=0;
            for (int i = 0; i < 1; i++)
            {
                //if (!dm.m_districts.m_buffer[i].m_flags.IsFlagSet(District.Flags.Created)) continue;
                //if (dm.m_districts.m_buffer[i].m_flags.IsFlagSet(District.Flags.CustomName)) continue;
                //citizencount += (int)dm.m_districts.m_buffer[i].m_populationData.m_finalCount;
                    
                int localcitizencount = (int)(dm.m_districts.m_buffer[i].m_adultData.m_finalCount
                    + dm.m_districts.m_buffer[i].m_childData.m_finalCount
                    + dm.m_districts.m_buffer[i].m_youngData.m_finalCount
                    + dm.m_districts.m_buffer[i].m_teenData.m_finalCount
                    + dm.m_districts.m_buffer[i].m_seniorData.m_finalCount);
                    
                    dmcount++;
                    citizencount += localcitizencount;
                    dmusage += dm.m_districts.m_buffer[i].GetElectricityConsumption();
                    dmproduction += dm.m_districts.m_buffer[i].GetElectricityCapacity();
                    waterbuffer += dm.m_districts.m_buffer[i].GetWaterConsumption();
                    sewagebuffer += dm.m_districts.m_buffer[i].GetSewageAccumulation();
                    sewagecapacity += dm.m_districts.m_buffer[i].GetSewageCapacity();
                    watercapacity += dm.m_districts.m_buffer[i].GetWaterCapacity();
                    garbage += dm.m_districts.m_buffer[i].GetGarbageAccumulation();
                    finalcrimerate += (int)dm.m_districts.m_buffer[i].m_finalCrimeRate;
            }

            statstopull.Add(new StatisticsClassWrapper("Power", "Used", dmusage / 1000, 2, "MW"));
            statstopull.Add(new StatisticsClassWrapper("Power", "Capacity", StatisticType.ElectricityCapacity, 1000, 16, "MW"));
            
            statstopull.Add(new StatisticsClassWrapper("Water", "Used", waterbuffer, 2, "m³"));
            statstopull.Add(new StatisticsClassWrapper("Water", "Capacity", StatisticType.WaterCapacity, 1, 16, "m³"));
            statstopull.Add(new StatisticsClassWrapper("Water", "Pollution", StatisticType.WaterPollution, 1, 1, "%"));
            statstopull.Add(new StatisticsClassWrapper("Water", "Sewage", sewagebuffer, 2, "m³"));
            statstopull.Add(new StatisticsClassWrapper("Water", "Capacity", StatisticType.SewageCapacity, 1, 16, "m³"));

            //statstopull.Add(new StatisticsClassWrapper(""));
            statstopull.Add(new StatisticsClassWrapper("Garbage", "Amount", StatisticType.GarbageAmount, 1000, 1, "K")); ;
            statstopull.Add(new StatisticsClassWrapper("Garbage", "Accumulation", garbage));
            statstopull.Add(new StatisticsClassWrapper("Garbage", "Capacity", StatisticType.GarbageCapacity, 1000, 1, "K")); ;
            statstopull.Add(new StatisticsClassWrapper("Garbage", "Incinerate Capacity", StatisticType.IncinerationCapacity, 1000, 16, "M")); ;
            statstopull.Add(new StatisticsClassWrapper("Garbage", "Piles", StatisticType.GarbagePiles, 1000, 1, "M")); ;
            //statstopull.Add(new StatisticsClassWrapper("Garbage", "...buffer", garbagebuffer));

            statstopull.Add(new StatisticsClassWrapper("Health Services", "Health", StatisticType.CitizenHealth, 1, 1, ""));
            statstopull.Add(new StatisticsClassWrapper("Health Services", "Capacity", StatisticType.HealCapacity, 1, 1, ""));
            
            statstopull.Add(new StatisticsClassWrapper("Death Services", "Amount", StatisticType.DeadAmount, 1, 1, ""));
            statstopull.Add(new StatisticsClassWrapper("Death Services", "Capacity", StatisticType.DeadCapacity, 1, 1, ""));
            statstopull.Add(new StatisticsClassWrapper("Death Services", "Cremate Capacity", StatisticType.CremateCapacity, 1000, 1, "K"));

            statstopull.Add(new StatisticsClassWrapper("Buildings", "Count", bm.m_buildingCount, 2, ""));
            //statstopull.Add(new StatisticsClassWrapper("Buildings", "Districts", dmcount, 2, ""));
            statstopull.Add(new StatisticsClassWrapper("Fire", "Buildings Burning", onfire, 2, ""));
            statstopull.Add(new StatisticsClassWrapper("Fire", "Coverage", firecoverage, 2, ""));
            statstopull.Add(new StatisticsClassWrapper("Fire", "Hazard", firehazard, 2, ""));
            statstopull.Add(new StatisticsClassWrapper("Buildings", "Abandoned", StatisticType.AbandonedBuildings, 1, 1, ""));

            statstopull.Add(new StatisticsClassWrapper("Citizens", "Count", citizencount, 2, ""));
            statstopull.Add(new StatisticsClassWrapper("Citizens", "Move Rate", StatisticType.MoveRate, 1, 1, ""));
            statstopull.Add(new StatisticsClassWrapper("Citizens", "Birth Rate", StatisticType.BirthRate, 1, 1, ""));
            statstopull.Add(new StatisticsClassWrapper("Citizens", "Death Rate", StatisticType.DeathRate, 1, 1, ""));
            statstopull.Add(new StatisticsClassWrapper("Citizens", "Eligible Workers", StatisticType.EligibleWorkers, 1, 1, ""));
            statstopull.Add(new StatisticsClassWrapper("Citizens", "Unemployed", StatisticType.Unemployed, 1, 1, ""));
           
            statstopull.Add(new StatisticsClassWrapper("Education", "Educated", StatisticType.EducatedCount, 1, 1, ""));
            statstopull.Add(new StatisticsClassWrapper("Education", "Students", StatisticType.StudentCount, 1, 1, ""));
            statstopull.Add(new StatisticsClassWrapper("Education", "Max Students", StatisticType.EducationCapacity, 1, 1, ""));
            
            statstopull.Add(new StatisticsClassWrapper("Crime", "Crimes?", StatisticType.CrimeRate, 1, 1, ""));
            statstopull.Add(new StatisticsClassWrapper("Crime", "Crime Rate", finalcrimerate, 2, "%"));

            statstopull.Add(new StatisticsClassWrapper("Tourists", "Visits", StatisticType.TouristVisits, 1, 1, ""));
            statstopull.Add(new StatisticsClassWrapper("Tourists", "Incoming", StatisticType.IncomingTourists, 1, 1, ""));
            statstopull.Add(new StatisticsClassWrapper("Public Transit", "Avg Passengers", StatisticType.AveragePassengers, 1, 1, ""));

            statstopull.Add(new StatisticsClassWrapper("Misc", "ImmaterialResource", StatisticType.ImmaterialResource, 1000, 1, "M"));
            statstopull.Add(new StatisticsClassWrapper("Misc", "Goods Produced", StatisticType.GoodsProduced, 1000, 1, "K"));
            
            //int buildingcount = buildManager.m_buildingCount;
            //mystrings.Add("build count: " + buildingcount);

            try
            {
                updateText(statstopull);
            }
            catch (Exception ex)
            {
                statlog.log(ex.Message);
                running = false;
            }
            /*
                        for (int i = 0; i < statstopull.Count; i++)
                            mystrings.Add(statstopull[i].statstring);


                        try
                        {
                            updateText(mystrings);
                        }
                        catch { running = false; }
                        */

            //running = false;

        }
    }

    public class CSLStatsPanelLabel : UILabel
    {
        public override void Start()
        {
            this.backgroundSprite = "GenericLabel";
            this.autoSize = true;
            this.useDropShadow = false;
            this.color = new Color32(255, 255, 255, 200);

        }


    }
}
