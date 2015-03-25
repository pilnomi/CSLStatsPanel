using UnityEngine;
using ICities;
using ColossalFramework;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSLStatsPanel 
{
    class ConfigWindow : UIScrollablePanel
    {
        UIResizeHandle myresizepanel;
        UIPanel headerpanel;
        UILabel headertext, resizelabel;
        ConfigSettingsWindow myConfigWindowPanel;

        public ConfigWindow()
        {

   

        }

        public override void Start()
        {
            base.Start();

            this.color = new Color32(0, 0, 255, 200);
            this.backgroundSprite = "GenericPanel";
            this.autoLayoutDirection = LayoutDirection.Vertical;
            this.autoLayoutStart = LayoutStart.TopLeft;
            this.autoLayout = true;
            this.autoLayoutPadding = new RectOffset(0, 0, 0, 0);


            //this.CenterToParent();

            headerpanel = (UIPanel)this.AddUIComponent(typeof(UIPanel));
            headerpanel.height = 20;
            headerpanel.backgroundSprite = "GenericPanel";
            headerpanel.color = new Color32(0, 0, 100, 100);

            headertext = headerpanel.AddUIComponent<UILabel>();
            headertext.text = "CSL Stats Panel - Configuration";
            headertext.CenterToParent();

            myConfigWindowPanel = (ConfigSettingsWindow)this.AddUIComponent(typeof(ConfigSettingsWindow));
            myConfigWindowPanel.name = "CSLStatsConfigurationPanel";
            myConfigWindowPanel.color = new Color32(0, 0, 255, 200);
            myConfigWindowPanel.eventStatsConfigChanged += new ConfigSettingsWindow.eventStatsConfigChangedHandler(myConfigWindowPanel_eventStatsConfigChanged);
            myresizepanel = (UIResizeHandle)this.AddUIComponent(typeof(UIResizeHandle));
            myresizepanel.name = "CSLStatsConfigurationResizePanel";
            myresizepanel.height = 20;
            myresizepanel.color = new Color32(0, 0, 100, 100);
            myresizepanel.backgroundSprite = "GenericPanel";
            //myresizepanel.anchor = UIAnchorStyle.Bottom;
            //myresizepanel.anchor = UIAnchorStyle.Right;
            //resizelabel = myresizepanel.AddUIComponent<UILabel>();
            



            var CloseButton = (UIButton)myresizepanel.AddUIComponent(typeof(UIButton));
            CloseButton.width = 125;
            CloseButton.height = 20;
            CloseButton.normalBgSprite = "ButtonMenu";
            CloseButton.hoveredBgSprite = "ButtonMenuHovered";
            CloseButton.focusedBgSprite = "ButtonMenuFocused";
            CloseButton.pressedBgSprite = "ButtonMenuPressed";
            CloseButton.textColor = new Color32(186, 217, 238, 0);
            CloseButton.disabledTextColor = new Color32(7, 7, 7, 255);
            CloseButton.hoveredTextColor = new Color32(7, 132, 255, 255);
            CloseButton.focusedTextColor = new Color32(255, 255, 255, 255);
            CloseButton.pressedTextColor = new Color32(30, 30, 44, 255);
            CloseButton.color = new Color32(CloseButton.color.r, CloseButton.color.g, CloseButton.color.b, 255);
            //CloseButton.transformPosition = new Vector3(1.2f, -0.93f);
            CloseButton.BringToFront();
            CloseButton.text = "Close";
            CloseButton.eventClick += new MouseEventHandler(CloseButton_eventClick);

            //this.width = 700;
            //this.height = 400;
            this.autoSize = true;
            this.FitToContents();
            this.CenterToParent();
        }

        public void myConfigWindowPanel_eventStatsConfigChanged(object sender, EventArgs e)
        {
            this.eventStatsConfigChanged(sender, e);
        }
        public delegate void eventStatsConfigChangedHandler(object sender, EventArgs e);
        public event eventStatsConfigChangedHandler eventStatsConfigChanged;


        void CloseButton_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            GameObject.Destroy(this);
        }

        protected override void OnMouseDown(UIMouseEventParameter p)
        {

            dragging = true;
        }
        bool dragging = false; 
        protected override void OnMouseUp(UIMouseEventParameter p)
        {
            dragging = false; 
        }
        protected override void OnMouseMove(UIMouseEventParameter p)
        {
            //resizelabel.text = string.Format("x{0} y{1} w{2} h{3} px{4} py{5} r{6}", this.relativePosition.x,
            //    this.relativePosition.y, this.width, this.height, p.position.x, p.position.y,
            //    checkresizebox(p));
            if (dragging) this.position = new Vector3(this.position.x + p.moveDelta.x,
             this.position.y + p.moveDelta.y,
             this.position.z);
        }
        protected override void OnSizeChanged()
        {

            base.OnSizeChanged();
            if (myConfigWindowPanel == null) return;
            headerpanel.width = this.width;
            myresizepanel.width = this.width;
            myConfigWindowPanel.width = this.width;
            myConfigWindowPanel.height = this.height - headerpanel.height - myresizepanel.height;
            //resizelabel.CenterToParent();
            headertext.CenterToParent();

            
        }

    }

    class ConfigSettingsWindow : UIScrollablePanel
    {
        UITextField refreshInterval;
        public ConfigSettingsWindow()
        {
            this.color = new Color32(0, 0, 255, 200);
            this.backgroundSprite = "GenericPanel";
            this.autoLayoutDirection = LayoutDirection.Vertical;
            this.autoLayoutStart = LayoutStart.TopLeft;
            this.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
            this.autoLayout = true;
            this.clipChildren = true;

      

            drawstatsconfig();
            this.FitChildrenVertically();
            this.FitToContents();
        }

        void refreshInterval_eventLeaveFocus(UIComponent component, UIFocusEventParameter eventParam)
        {
            int t = 1;
            if (int.TryParse(refreshInterval.text, out t))
            {
                CSLStatsPanelConfigSettings.PanelRefreshRate = t;

            }
            if (CSLStatsPanelConfigSettings.PanelRefreshRate != t)
                refreshInterval.text = CSLStatsPanelConfigSettings.PanelRefreshRate.ToString();
        }
        UILabel refreshIntervalLabel;
        void drawstatsconfig()
        {


            UIScrollablePanel p = this.AddUIComponent<UIScrollablePanel>();
            p.width = this.width;
            //p.height = 40;
            p.autoLayoutDirection = LayoutDirection.Horizontal;
            p.autoLayout = true;
            p.backgroundSprite = "GenericPanel";
            
            refreshIntervalLabel = p.AddUIComponent<UILabel>();
            refreshIntervalLabel.text = "Refresh Interval (sec): ";
            refreshInterval = p.AddUIComponent<UITextField>();
            refreshInterval.Enable();
            refreshInterval.text = CSLStatsPanelConfigSettings.PanelRefreshRate.ToString();
            refreshInterval.numericalOnly = true;
            refreshInterval.eventLeaveFocus += new FocusEventHandler(refreshInterval_eventLeaveFocus);

            UILabel decrementLabel = p.AddUIComponent<UILabel>();
            decrementLabel.text = "-";
            decrementLabel.eventClick += new MouseEventHandler(decrementLabel_eventClick);
            UILabel incrementLabel = p.AddUIComponent<UILabel>();
            incrementLabel.text = "+";
            incrementLabel.eventClick += new MouseEventHandler(incrementLabel_eventClick);
            p.FitChildrenHorizontally();
            p.FitToContents();

            List<StatisticsCategoryWrapper> scw = CSLStatsPanelConfigSettings.Categories(false);
            for (int i = 0; i < scw.Count(); i++)
            {
                drawstatsconfigpanel(scw[i]);
            }
        }

        void incrementLabel_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            CSLStatsPanelConfigSettings.PanelRefreshRate++;
            refreshInterval.text = CSLStatsPanelConfigSettings.PanelRefreshRate.ToString();
        }

        void decrementLabel_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            CSLStatsPanelConfigSettings.PanelRefreshRate--;
            refreshInterval.text = CSLStatsPanelConfigSettings.PanelRefreshRate.ToString();
            
        }

        UIPanel drawstatsconfigpanel(StatisticsCategoryWrapper scw)
        {
            UIPanel p = this.AddUIComponent<UIPanel>();
            p.backgroundSprite = "GenericPanel";
            
            p.width = this.width;
            //p.height = 40;
            p.autoLayoutDirection = LayoutDirection.Vertical;
            p.autoLayout = true;



            UIScrollablePanel catsubpanel = p.AddUIComponent<UIScrollablePanel>();
            catsubpanel.autoLayout = true;
            catsubpanel.autoLayoutDirection = LayoutDirection.Horizontal;
            catsubpanel.width = p.width;
            catsubpanel.backgroundSprite = "GenericPanel";
            catsubpanel.height = 15;
            catsubpanel.autoLayoutPadding = new RectOffset(5, 5, 5, 5);
            UILabel catname = catsubpanel.AddUIComponent<UILabel>();
            catname.autoSize = true;
            catname.text = scw.m_category;
            catname.name = scw.m_category;
            bool catisChecked = CSLStatsPanelConfigSettings.isCatActive(scw.m_category);
            catsubpanel.color = (catisChecked) ? selectedcolor : deselectedcolor;
                
            catsubpanel.FitChildrenHorizontally();
            catsubpanel.autoSize = true;
            catname.eventClick += new MouseEventHandler(catsubpanel_eventClick);

            //mypanels = new Dictionary<string, UIScrollablePanel>();
            for (int x = 0; x < scw.m_scwlist.Count(); x++)
            {
                UIScrollablePanel statsubpanel = catsubpanel.AddUIComponent<UIScrollablePanel>();
                statsubpanel.backgroundSprite = "GenericPanel";
                //statsubpanel.height = 15;
                statsubpanel.autoLayout = true;
                statsubpanel.autoSize = true;
                statsubpanel.autoLayoutDirection = LayoutDirection.Horizontal;
                statsubpanel.width = p.width;
                bool isStatActive = CSLStatsPanelConfigSettings.isStatActive(scw.m_category, scw.m_scwlist[x].m_desc);
                statsubpanel.name = scw.m_category + "|" + scw.m_scwlist[x].m_desc;
                
                statsubpanel.color = (isStatActive) ? selectedcolor : deselectedcolor;
                statsubpanel.eventClick += new MouseEventHandler(statsubpanel_eventClick);
                UILabel statname = statsubpanel.AddUIComponent<UILabel>();
                statname.text = scw.m_scwlist[x].m_desc;
                //mypanels.Add(scw.m_scwlist[x].m_desc, statsubpanel);
                
                statsubpanel.FitChildrenHorizontally();
                statsubpanel.FitToContents();
            }
            catsubpanel.FitChildrenHorizontally();
            catsubpanel.FitToContents();
            p.FitChildrenVertically();

            
            return p;
        }

        void catsubpanel_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            bool isCatActive = !CSLStatsPanelConfigSettings.isCatActive(component.name);
            CSLStatsPanelConfigSettings.setCatActive(component.name, isCatActive);
            component.color = (isCatActive) ? selectedcolor : deselectedcolor;
            component.parent.color = component.color;
            eventStatsConfigChanged(this, EventArgs.Empty);
        }
        public delegate void eventStatsConfigChangedHandler(object sender, EventArgs e);
        public event eventStatsConfigChangedHandler eventStatsConfigChanged;

        Color32 selectedcolor = new Color32(0, 255, 0, 255),
            deselectedcolor = new Color32(255, 0, 0, 255);

        Dictionary<string, UIPanel> mypanels = new Dictionary<string, UIPanel>();
        void statsubpanel_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            string[] mystring = component.name.Split('|');
            bool isStatActive = !CSLStatsPanelConfigSettings.isStatActive(mystring[0], mystring[1]);
            CSLStatsPanelConfigSettings.setStatActive(mystring[0], mystring[1], isStatActive);
            component.color = (isStatActive) ? selectedcolor : deselectedcolor;
            eventStatsConfigChanged(this, EventArgs.Empty);

        }





    }

    public static class CSLStatsPanelConfigSettings
    {
        class customColor
        {
            private string m_name = "defaultPanelColor";
            private SavedInt m_r, m_g, m_b, m_a;

            public customColor(string name)
            {
                m_name = name;
                initVars();
            }

            public Color32 value
            {
                get { return new Color32((byte)m_r.value, (byte)m_g.value, (byte)m_b.value, (byte)m_a.value); }
                set
                {
                    m_r.value = value.r;
                    m_g.value = value.g;
                    m_b.value = value.b;
                    m_a.value = value.a;
                }
            }

            private void initVars()
            {
                m_r = new SavedInt(m_settingsprefix + "SavedColor_" + m_name + "R", Settings.gameSettingsFile, 0, true);
                m_g = new SavedInt(m_settingsprefix + "SavedColor_" + m_name + "G", Settings.gameSettingsFile, 0, true);
                m_b = new SavedInt(m_settingsprefix + "SavedColor_" + m_name + "B", Settings.gameSettingsFile, 255, true);
                m_a = new SavedInt(m_settingsprefix + "SavedColor_" + m_name + "A", Settings.gameSettingsFile, 200, true);
            }

            public customColor(string name, int r, int g, int b, int a)
            {
                m_name = name;
                initVars();
                m_r.value = r; m_g.value = g; m_b.value = b; m_a.value = a;
            }
        }
        public const string m_settingsprefix = "MODCSLStatsPanelConfig_";
        public const int m_minRefreshRate = 1, m_maxRefreshRate=255;

        private static SavedInt m_PanelRefreshRate = new SavedInt(m_settingsprefix + "RefreshRate", Settings.gameSettingsFile, 3, true);
        public static int PanelRefreshRate
        {
            get { return m_PanelRefreshRate; }
            set {
                if (value < 1) value = 1;
                if (value > 255) value = 255;
                m_PanelRefreshRate.value = value; 
            }
        }

        private static customColor m_defaultPanelColor = new customColor("defaultPanelColor");
        public static Color32 DefaultPanelColor
        {
            get { return m_defaultPanelColor.value; }
            set { m_defaultPanelColor.value = value; }
        }

        private static List<StatisticsCategoryWrapper> m_categories
        {
            get{ return MasterStatsWrapper.getstats3(); }
        }

        public static List<StatisticsCategoryWrapper> Categories(bool onlyactive=true)
        {
            if (!onlyactive ) return m_categories;
            List<StatisticsCategoryWrapper> t = new List<StatisticsCategoryWrapper>();
            List<StatisticsCategoryWrapper> x = m_categories;
            for (int i = 0; i < x.Count(); i++)
            {
                bool isactive = isCatActive(x[i].m_category);
                statlog.log(x[i].m_category + " is active: " + isactive.ToString());
                if (isactive) t.Add(x[i]);
            }
            return t;
        }

        public static bool isCatActive(string catname)
        {
            SavedBool isactive = new SavedBool(m_settingsprefix + "Cat_" + catname + "_Active", Settings.gameSettingsFile, true, true);
            return isactive.value;
        }

        public static void setCatActive(string catname, bool active)
        {
            SavedBool isactive = new SavedBool(m_settingsprefix + "Cat_" + catname + "_Active", Settings.gameSettingsFile, true, true);
            isactive.value = active;
        }

        public static bool isStatActive(string catname, string statname)
        {
            SavedBool isactive = new SavedBool(m_settingsprefix + "Stat_" + catname + "_" + statname + "_Active", Settings.gameSettingsFile, true, true);
            return isactive.value;
        }

        public static void setStatActive(string catname, string statname, bool active)
        {
            SavedBool isactive = new SavedBool(m_settingsprefix + "Stat_" + catname + "_" + statname + "_Active", Settings.gameSettingsFile, true, true);
            isactive.value = active;
        }
    }
}
