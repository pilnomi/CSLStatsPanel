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
    class ConfigWindow : UIPanel
    {
        UIResizeHandle myresizepanel;
        UIPanel headerpanel;
        UILabel headertext, resizelabel;
        ConfigSettingsWindow myConfigWindowPanel;

        public ConfigWindow()
        {

            //this.color = new Color32(0, 0, 100, 200);
            this.backgroundSprite = "GenericPanel";
            this.autoLayoutDirection = LayoutDirection.Vertical;
            this.autoLayoutStart = LayoutStart.TopLeft;
            this.autoLayout = true;
            this.autoLayoutPadding = new RectOffset(0, 0, 0, 0);
            this.width = 1000;
            this.height = 710;
            this.name = "CLSStatsPanelConfigurationWindow";

        }

        public override void Start()
        {

            headerpanel = (UIPanel)this.AddUIComponent(typeof(UIPanel));
            headerpanel.height = 20;
            headerpanel.backgroundSprite = "GenericPanel";
            headerpanel.color = new Color32(0, 0, 100, 200);

            headertext = headerpanel.AddUIComponent<UILabel>();
            headertext.text = "CSL Stats Panel - Configuration";
            headertext.CenterToParent();

            myConfigWindowPanel = (ConfigSettingsWindow)this.AddUIComponent(typeof(ConfigSettingsWindow));
            myConfigWindowPanel.width = this.width;
            myConfigWindowPanel.name = "CSLStatsConfigurationPanel";
            myConfigWindowPanel.color = new Color32(0, 0, 0, 255);
            //myConfigWindowPanel.eventStatsConfigChanged += new ConfigSettingsWindow.eventStatsConfigChangedHandler(myConfigWindowPanel_eventStatsConfigChanged);
            //myConfigWindowPanel.eventModeConfigChanged += new ConfigSettingsWindow.eventConfigModeChangedHandler(myConfigWindowPanel_eventModeConfigChanged);
            //myConfigWindowPanel.eventConfigReset += new ConfigSettingsWindow.eventConfigResetHandler(myConfigWindowPanel_eventConfigReset);
            //myConfigWindowPanel.eventConfigTransparencyChanged += new ConfigSettingsWindow.eventConfigTransparencyChangeHandler(myConfigWindowPanel_eventConfigTransparencyChanged);
            myresizepanel = (UIResizeHandle)this.AddUIComponent(typeof(UIResizeHandle));
            myresizepanel.name = "CSLStatsConfigurationResizePanel";
            myresizepanel.height = 20;
            myresizepanel.color = new Color32(0, 0, 100, 200);
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

            myresizepanel.FitChildrenVertically();
            this.CenterToParent();

            base.Start();
            OnSizeChanged();
            this.Update();
        }

        /*
        void myConfigWindowPanel_eventConfigTransparencyChanged(object sender, EventArgs e)
        {
            eventConfigTransparencyChanged(sender, e);
            CloseButton_eventClick(null, null);
        }

        void myConfigWindowPanel_eventConfigReset(object sender, EventArgs e)
        {
            eventStatsConfigReset(sender, e);
            CloseButton_eventClick(null, null);
        }

        void myConfigWindowPanel_eventModeConfigChanged(object sender, EventArgs e)
        {
            this.eventModeConfigChanged(sender, e);
        }

        public void myConfigWindowPanel_eventStatsConfigChanged(object sender, EventArgs e)
        {
            this.eventStatsConfigChanged(sender, e);
        }
        public delegate void eventStatsConfigResetHandler(object sender, EventArgs e);
        public event eventStatsConfigResetHandler eventStatsConfigReset;

        public delegate void eventStatsConfigChangedHandler(object sender, EventArgs e);
        public event eventStatsConfigChangedHandler eventStatsConfigChanged;

        public delegate void eventConfigModeChangedHandler(object sender, EventArgs e);
        public event eventConfigModeChangedHandler eventModeConfigChanged;


        public delegate void eventConfigTransparencyChangeHandler(object sender, EventArgs e);
        public event eventConfigTransparencyChangeHandler eventConfigTransparencyChanged;
        */

        void CloseButton_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            try
            {
                myresizepanel.enabled = false;
                UIView.DestroyImmediate(this);
            }
            catch { }
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
        protected override void OnClick(UIMouseEventParameter p)
        {
            this.BringToFront();
            base.OnClick(p);
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

            UIPanel[] mypanels = myConfigWindowPanel.catpanels.ToArray();
            for (int i = 0; i < mypanels.Count(); i++) 
            {
                mypanels[i].width = myConfigWindowPanel.width - 15;
               // mypanels[i].FitToContents();
                mypanels[i].FitChildrenVertically();
            
            }

        

        }

    }

    class ConfigSettingsWindow : UIPanel
    {
        UITextField refreshInterval;
        public List<UIPanel> catpanels = new List<UIPanel>();
        public override void Start()
        {
            base.Start();
            //this.color = new Color32(0, 0, 100, 200);
            this.backgroundSprite = "GenericPanel";
            this.autoLayoutDirection = LayoutDirection.Vertical;
            this.autoLayoutStart = LayoutStart.TopLeft;
            this.autoLayoutPadding = new RectOffset(5, 5, 5,5);
            this.autoLayout = true;
            this.clipChildren = true;



            drawstatsconfig();
            //this.FitChildrenVertically();
            //this.FitChildrenHorizontally();
            //this.FitToContents();
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
            //p.width = this.width;
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

            UIButton decrementLabel = p.AddUIComponent<UIButton>();
            setcommonbuttonprops(decrementLabel);
            decrementLabel.width = 5;
            decrementLabel.text = "-";
            decrementLabel.eventClick += new MouseEventHandler(decrementLabel_eventClick);
            UIButton incrementLabel = p.AddUIComponent<UIButton>();
            setcommonbuttonprops(incrementLabel);
            incrementLabel.width = 5;
            incrementLabel.text = "+";
            incrementLabel.eventClick += new MouseEventHandler(incrementLabel_eventClick);


            p.FitChildrenHorizontally();
            p.FitToContents();

            p = this.AddUIComponent<UIScrollablePanel>();
            //p.width = this.width;
            //p.height = 40;
            p.autoLayoutPadding = new RectOffset(5, 5, 5, 5);
            p.autoLayoutDirection = LayoutDirection.Horizontal;
            p.autoLayout = true;
            //p.wrapLayout = true;
            useColors = p.AddUIComponent<UIButton>();
            setcommonbuttonprops(useColors);
            useColors.text = "Use Panel Colors";
            useColors.tooltip = "Think colors are over-rated?  Toggle them on/off";
            useColors.eventClick += new MouseEventHandler(useColors_eventClick);
            useColors.textColor = (CSLStatsPanelConfigSettings.m_EnablePanelColors.value) ? selectedcolor : deselectedcolor;

            UIButton displaySummaries = p.AddUIComponent<UIButton>();
            setcommonbuttonprops(displaySummaries);
            displaySummaries.text = "% Summaries";
            displaySummaries.tooltip = "Toggle display of % summaries in expanded mode.";
            displaySummaries.textColor = (CSLStatsPanelConfigSettings.m_EnablePanelSummaries.value) ? selectedcolor : deselectedcolor;
            displaySummaries.eventClick += new MouseEventHandler(displaySummaries_eventClick);

            UIButton miniMode = p.AddUIComponent<UIButton>();
            setcommonbuttonprops(miniMode);
            miniMode.text = "Mini-Mode";
            miniMode.textColor = (CSLStatsPanelConfigSettings.m_MiniMode.value) ? selectedcolor : deselectedcolor;
            miniMode.eventClick += new MouseEventHandler(miniMode_eventClick);

            UIButton showLabelsInMiniMode = p.AddUIComponent<UIButton>();
            setcommonbuttonprops(showLabelsInMiniMode);
            showLabelsInMiniMode.text = "Labels in Mini-Mode";
            showLabelsInMiniMode.tooltip = "Show service description labels while in mini-mode, turn this off to see only the icons";
            showLabelsInMiniMode.autoSize = true;
            showLabelsInMiniMode.textColor = (CSLStatsPanelConfigSettings.m_ShowLabelsInMiniMode.value) ? selectedcolor : deselectedcolor;
            showLabelsInMiniMode.eventClick += new MouseEventHandler(showLabelsInMiniMode_eventClick);

            UIButton enableTransparency = p.AddUIComponent<UIButton>();
            setcommonbuttonprops(enableTransparency);
            enableTransparency.text = "Transparency";
            enableTransparency.textColor = (CSLStatsPanelConfigSettings.m_EnableTransparency.value) ? selectedcolor : deselectedcolor;
            enableTransparency.eventClick += new MouseEventHandler(enableTransparency_eventClick);

            UIButton useVehicleStatsInSummaries = p.AddUIComponent<UIButton>();
            setcommonbuttonprops(useVehicleStatsInSummaries);
            useVehicleStatsInSummaries.text = "Vehicle Usage";
            useVehicleStatsInSummaries.tooltip = "Use vehicle inuse/total on Summary % when it is higher than default used/capacity (excludes police)";
            useVehicleStatsInSummaries.textColor = (CSLStatsPanelConfigSettings.m_UseVechileStatsForSummaries.value) ? selectedcolor : deselectedcolor;
            useVehicleStatsInSummaries.eventClick += new MouseEventHandler(useVehicleStatsInSummaries_eventClick);

            UIButton resetConfig = p.AddUIComponent<UIButton>();
            setcommonbuttonprops(resetConfig);
            resetConfig.text = "Reset Config";
            resetConfig.textColor = resetConfig.focusedTextColor;
            resetConfig.eventClick += new MouseEventHandler(resetConfig_eventClick);


            p.FitChildrenHorizontally();
            p.FitToContents();


            List<StatisticsCategoryWrapper> scw = CSLStatsPanelConfigSettings.Categories(false);
            for (int i = 0; i < scw.Count(); i++)
            {
                drawstatsconfigpanel(scw[i]);
            }
        }

        void useVehicleStatsInSummaries_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            CSLStatsPanelConfigSettings.m_UseVechileStatsForSummaries.value = !CSLStatsPanelConfigSettings.m_UseVechileStatsForSummaries.value;
            //CSLStatsPanelConfigSettings.m_ConfigChanged.value = true;
            ((UIButton)component).textColor = (CSLStatsPanelConfigSettings.m_UseVechileStatsForSummaries.value) ? selectedcolor : deselectedcolor;
            ((UIButton)component).focusedColor = ((UIButton)component).textColor;
            component.parent.Focus();
        }

        void enableTransparency_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            CSLStatsPanelConfigSettings.m_EnableTransparency.value = !CSLStatsPanelConfigSettings.m_EnableTransparency.value;
            CSLStatsPanelConfigSettings.m_ConfigChanged.value = true;
            ((UIButton)component).textColor = (CSLStatsPanelConfigSettings.m_EnableTransparency.value) ? selectedcolor : deselectedcolor;
            //component.parent.Focus();
            //eventConfigTransparencyChanged(this, EventArgs.Empty);
        }

        //public delegate void eventConfigTransparencyChangeHandler(object sender, EventArgs e);
        //public event eventConfigTransparencyChangeHandler eventConfigTransparencyChanged;

        void resetConfig_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            CSLStatsPanelConfigSettings.resetConfig();
            CSLStatsPanelConfigSettings.m_ConfigChanged.value = true;
            UIView.DestroyImmediate(this.parent);
            //eventConfigReset(this, EventArgs.Empty);
            
        }

        void showLabelsInMiniMode_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            CSLStatsPanelConfigSettings.m_ShowLabelsInMiniMode.value = !CSLStatsPanelConfigSettings.m_ShowLabelsInMiniMode.value;
            CSLStatsPanelConfigSettings.m_ConfigChanged.value = true;
            ((UIButton)component).textColor = (CSLStatsPanelConfigSettings.m_ShowLabelsInMiniMode.value) ? selectedcolor : deselectedcolor;
            component.parent.Focus();
            //eventStatsConfigChanged(this, EventArgs.Empty);
        }

        private void setcommonbuttonprops(UIButton b)
        {
            b.normalFgSprite = "ButtonMenu";
            b.width = 125;
            b.height = 20;
            b.normalBgSprite = "ButtonMenu";
            b.hoveredBgSprite = "ButtonMenuHovered";
            b.focusedBgSprite = "ButtonMenuFocused";
            b.pressedBgSprite = "ButtonMenuPressed";
            b.disabledTextColor = new Color32(7, 7, 7, 255);
            b.hoveredTextColor = new Color32(7, 132, 255, 255);
        }

        void miniMode_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            CSLStatsPanelConfigSettings.m_MiniMode.value = !CSLStatsPanelConfigSettings.m_MiniMode.value;
            CSLStatsPanelConfigSettings.m_ConfigChanged.value = true;
            UIButton b = (UIButton)component;
            b.textColor = (CSLStatsPanelConfigSettings.m_MiniMode.value) ? selectedcolor : deselectedcolor;
            b.focusedColor = (CSLStatsPanelConfigSettings.m_MiniMode.value) ? selectedcolor : deselectedcolor;
            b.parent.Focus();
            //eventModeConfigChanged(this, EventArgs.Empty);
        }

        void displaySummaries_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            CSLStatsPanelConfigSettings.m_EnablePanelSummaries.value = !CSLStatsPanelConfigSettings.m_EnablePanelSummaries.value;
            CSLStatsPanelConfigSettings.m_ConfigChanged.value = true;
            UIButton b = (UIButton)component;
            b.textColor = (CSLStatsPanelConfigSettings.m_EnablePanelSummaries.value) ? selectedcolor : deselectedcolor;
            b.focusedColor = (CSLStatsPanelConfigSettings.m_EnablePanelSummaries.value) ? selectedcolor : deselectedcolor;
            b.parent.Focus();
           // eventStatsConfigChanged(this, EventArgs.Empty);
        }
        UIButton useColors;
        void useColors_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            CSLStatsPanelConfigSettings.m_EnablePanelColors.value = !CSLStatsPanelConfigSettings.m_EnablePanelColors.value;
            CSLStatsPanelConfigSettings.m_ConfigChanged.value = true;
            useColors.textColor = (CSLStatsPanelConfigSettings.m_EnablePanelColors.value) ? selectedcolor : deselectedcolor;
            useColors.focusedColor = (CSLStatsPanelConfigSettings.m_EnablePanelColors.value) ? selectedcolor : deselectedcolor;
            useColors.parent.Focus();
            //eventStatsConfigChanged(this, EventArgs.Empty);
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
            //UIScrollablePanel p = this.AddUIComponent<UIScrollablePanel>();
            //p.backgroundSprite = "GenericPanel";
            
            //p.width = this.width;
            //p.height = 40;
            //p.autoLayoutDirection = LayoutDirection.Vertical;
            //p.autoLayout = true;



            UIPanel catsubpanel = this.AddUIComponent<UIPanel>();
            catpanels.Add(catsubpanel);
            catsubpanel.autoLayout = true;
            catsubpanel.wrapLayout = true;
            catsubpanel.autoLayoutDirection = LayoutDirection.Horizontal;
            catsubpanel.width = this.width - 5;
            catsubpanel.backgroundSprite = "GenericPanel";
            //catsubpanel.height = 15;
            catsubpanel.autoLayoutPadding = new RectOffset(3, 3, 3, 3);
            
            bool catisChecked = CSLStatsPanelConfigSettings.isCatActive(scw.m_category);
            UIButton catname = catsubpanel.AddUIComponent<UIButton>();
            catname.textColor = (catisChecked) ? selectedcolor : deselectedcolor;
            setcommonbuttonprops(catname);
            catname.autoSize = true;
            catname.text = scw.m_category;
            catname.name = scw.m_category;
            catsubpanel.color = (catisChecked) ? selectedcolor : deselectedcolor;
                
            catsubpanel.autoSize = true;
            catname.eventClick += new MouseEventHandler(catsubpanel_eventClick);

            //mypanels = new Dictionary<string, UIScrollablePanel>();
            for (int x = 0; x < scw.m_scwlist.Count(); x++)
            {
                //UIScrollablePanel statsubpanel = catsubpanel.AddUIComponent<UIScrollablePanel>();
                //statsubpanel.backgroundSprite = "GenericPanel";
                //statsubpanel.autoLayout = true;
                //statsubpanel.autoSize = true;
                //statsubpanel.autoLayoutDirection = LayoutDirection.Horizontal;
                //statsubpanel.width = p.width;
                bool isStatActive = CSLStatsPanelConfigSettings.isStatActive(scw.m_category, scw.m_scwlist[x].m_desc);
                UIButton statname = catsubpanel.AddUIComponent<UIButton>();
                statname.name = scw.m_category + "|" + scw.m_scwlist[x].m_desc;

                statname.color = (isStatActive) ? selectedcolor : deselectedcolor;
                statname.textColor = (isStatActive) ? selectedcolor : deselectedcolor;
                setcommonbuttonprops(statname);
                statname.text = scw.m_scwlist[x].m_desc;
                statname.eventClick += new MouseEventHandler(statsubpanel_eventClick);
                //mypanels.Add(scw.m_scwlist[x].m_desc, statsubpanel);

                //statsubpanel.FitToContents();
                //statsubpanel.FitChildrenHorizontally();
            }
            catsubpanel.FitChildrenVertically();
            //catsubpanel.FitToContents();
            
            //p.FitChildrenVertically();

            
            return catsubpanel;
        }

        void catsubpanel_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            bool isCatActive = !CSLStatsPanelConfigSettings.isCatActive(component.name);
            CSLStatsPanelConfigSettings.setCatActive(component.name, isCatActive);
            CSLStatsPanelConfigSettings.m_ConfigChanged.value = true;

            UIButton b = (UIButton)component;
            b.textColor = (isCatActive) ? selectedcolor : deselectedcolor;
            b.focusedColor = (isCatActive) ? selectedcolor : deselectedcolor;
            b.parent.Focus();

            component.color = (isCatActive) ? selectedcolor : deselectedcolor;
            component.parent.color = component.color;

            //eventStatsConfigChanged(this, EventArgs.Empty);
        }
        /*
        public delegate void eventConfigResetHandler(object sender, EventArgs e);
        public event eventConfigResetHandler eventConfigReset;
        public delegate void eventConfigModeChangedHandler(object sender, EventArgs e);
        public event eventConfigModeChangedHandler eventModeConfigChanged;
        public delegate void eventStatsConfigChangedHandler(object sender, EventArgs e);
        public event eventStatsConfigChangedHandler eventStatsConfigChanged;
        */
        Color32 selectedcolor = new Color32(0, 200, 0, 255),
            deselectedcolor = new Color32(200, 0, 0, 255);

        void statsubpanel_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            string[] mystring = component.name.Split('|');
            bool isStatActive = !CSLStatsPanelConfigSettings.isStatActive(mystring[0], mystring[1]);
            CSLStatsPanelConfigSettings.setStatActive(mystring[0], mystring[1], isStatActive);
            CSLStatsPanelConfigSettings.m_ConfigChanged.value = true;

            UIButton b = (UIButton)component;
            b.textColor = (isStatActive) ? selectedcolor : deselectedcolor;
            b.focusedColor = b.textColor;
            b.focusedTextColor = b.textColor;
            b.parent.Focus();

            component.color = (isStatActive) ? selectedcolor : deselectedcolor;
            //component.parent.color = component.color;
            //eventStatsConfigChanged(this, EventArgs.Empty);

        }





    }

    public static class CSLStatsPanelConfigSettings
    {
        public const string m_settingsprefix = "MODCSLStatsPanelConfig_";
        public const int m_minRefreshRate = 1, m_maxRefreshRate = 255;
        public static List<SavedValue> configurationsettings = new List<SavedValue>();
        
        public class customColor
        {
            private string m_name = "defaultPanelColor";
            private SavedInt m_r, m_g, m_b, m_a;

            public customColor(string name, int default_r=0, int default_g=0, int default_b=255, int default_a=200)
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

            private void initVars(int default_r = 0, int default_g = 0, int default_b = 255, int default_a = 200)
            {
                m_r = new SavedInt(m_settingsprefix + "SavedColor_" + m_name + "R", Settings.gameSettingsFile, default_r, true);
                m_g = new SavedInt(m_settingsprefix + "SavedColor_" + m_name + "G", Settings.gameSettingsFile, default_g, true);
                m_b = new SavedInt(m_settingsprefix + "SavedColor_" + m_name + "B", Settings.gameSettingsFile, default_b, true);
                m_a = new SavedInt(m_settingsprefix + "SavedColor_" + m_name + "A", Settings.gameSettingsFile, default_a, true);

                if (!CSLStatsPanelConfigSettings.configurationsettings.Contains(m_r))
                    CSLStatsPanelConfigSettings.configurationsettings.Add(m_r);
                if (!CSLStatsPanelConfigSettings.configurationsettings.Contains(m_g))
                    CSLStatsPanelConfigSettings.configurationsettings.Add(m_g);
                if (!CSLStatsPanelConfigSettings.configurationsettings.Contains(m_b))
                    CSLStatsPanelConfigSettings.configurationsettings.Add(m_b);
                if (!CSLStatsPanelConfigSettings.configurationsettings.Contains(m_a))
                    CSLStatsPanelConfigSettings.configurationsettings.Add(m_a);
            }
            /*
            public customColor(string name, int r, int g, int b, int a)
            {
                m_name = name;
                initVars();
                m_r.value = r; m_g.value = g; m_b.value = b; m_a.value = a;
            }
             */ 
        }
        public class mySavedFloat
        {
            SavedFloat f = null;
            public mySavedFloat(string name, float defaultvalue)
            {
                f = new SavedFloat(name, Settings.gameSettingsFile, defaultvalue, true);
                if (!CSLStatsPanelConfigSettings.configurationsettings.Contains(f))
                    CSLStatsPanelConfigSettings.configurationsettings.Add(f);
            }

            public float value
            {
                get { return f.value; }
                set{f.value = value;}
            }

        }
        public class mySavedInt
        {
            SavedInt f = null;
            public mySavedInt(string name, int defaultvalue)
            {
                f = new SavedInt(name, Settings.gameSettingsFile, defaultvalue, true);
                if (!CSLStatsPanelConfigSettings.configurationsettings.Contains(f))
                    CSLStatsPanelConfigSettings.configurationsettings.Add(f);
            }

            public int value
            {
                get { return f.value; }
                set { f.value = value; }
            }

        }
        public class mySavedBool
        {
            SavedBool f = null;
            public mySavedBool(string name, bool defaultvalue)
            {
                f = new SavedBool(name, Settings.gameSettingsFile, defaultvalue, true);
                if (!CSLStatsPanelConfigSettings.configurationsettings.Contains(f))
                    CSLStatsPanelConfigSettings.configurationsettings.Add(f);

            }

            public bool value
            {
                get { return f.value; }
                set { f.value = value; }
            }

        }

        public static mySavedBool m_DisplayPanel = new mySavedBool(m_settingsprefix + "DisplayPanel", true);
        public static mySavedFloat
            windowx = new mySavedFloat("ModCSLStatsPanelWindowPosX", 0),
            windowy = new mySavedFloat("ModCSLStatsPanelWindowPosY", 0),
            windoww = new mySavedFloat("ModCSLStatsPanelWindowPosW", 700),
            windowh = new mySavedFloat("ModCSLStatsPanelWindowPosH", 400);

        public static mySavedFloat
            miniwindowx = new mySavedFloat("ModCSLStatsPanelMiniWindowPosX", 0),
            miniwindowy = new mySavedFloat("ModCSLStatsPanelMiniWindowPosY", 0),
            miniwindoww = new mySavedFloat("ModCSLStatsPanelMiniWindowPosW", 400),
            miniwindowh = new mySavedFloat("ModCSLStatsPanelMiniWindowPosH", 200);

        public static mySavedInt
            fontchange = new mySavedInt("CSLStatsPanelTextScaleDelta", 0),
            minifontchange = new mySavedInt("CSLStatsMiniPanelTextScaleDelta", 0);

        public static mySavedBool m_ConfigChanged = new mySavedBool(m_settingsprefix + "ConfigChanged", false);

        public static mySavedBool m_ShowLabelsInMiniMode = new mySavedBool(m_settingsprefix + "ShowLabelsInMiniMode", true);

        public static mySavedBool m_MiniMode = new mySavedBool(m_settingsprefix + "EnableMiniMode", false);
        public static mySavedBool m_UseVechileStatsForSummaries = new mySavedBool(m_settingsprefix + "UseVehicleStatsForSummaries", true);
        public static mySavedBool m_EnablePanelColors = new mySavedBool(m_settingsprefix + "EnablePanelColors", true);
        public static mySavedBool m_EnablePanelSummaries = new mySavedBool(m_settingsprefix + "EnablePanelSummaries", true);
        public static mySavedBool m_EnableTransparency = new mySavedBool(m_settingsprefix + "EnableTransparency", true);
        private static mySavedInt m_PanelRefreshRate = new mySavedInt(m_settingsprefix + "RefreshRate", 3);
        public static int PanelRefreshRate
        {
            get { return m_PanelRefreshRate.value; }
            set {
                if (value < 1) value = 1;
                if (value > 255) value = 255;
                m_PanelRefreshRate.value = value; 
            }
        }

        private static customColor m_defaultPanelColor = new customColor("defaultPanelColor", 0, 0, 255, 255);
        private static customColor m_transparentPanelColor = new customColor("transparentPanelColor", 0, 0, 0, 0);
        public static Color32 DefaultPanelColor
        {
            get { return m_defaultPanelColor.value; }
            set { m_defaultPanelColor.value = value; }
        }
        public static Color32 TransparentPanelColor
        {
            get { return m_transparentPanelColor.value; }
            set { m_transparentPanelColor.value = value; }
        }

        private static List<StatisticsCategoryWrapper> m_categories(bool onlyactive = true)
        {
            return MasterStatsWrapper.getstats3(onlyactive);
        }

        public static List<StatisticsCategoryWrapper> Categories(bool onlyactive=true)
        {
            if (!onlyactive ) return m_categories(onlyactive);
            List<StatisticsCategoryWrapper> t = new List<StatisticsCategoryWrapper>();
            List<StatisticsCategoryWrapper> x = m_categories(onlyactive);
            for (int i = 0; i < x.Count(); i++)
            {
                bool isactive = isCatActive(x[i].m_category);
                if (isactive) t.Add(x[i]);
            }
            return t;
        }

        public static bool isCatActive(string catname)
        {
            string savedstatname = m_settingsprefix + "Cat_" + catname + "_Active";
            SavedBool isactive = new SavedBool(savedstatname, Settings.gameSettingsFile, true, true);
            if (!configurationsettings.Contains(isactive)) configurationsettings.Add(isactive);
            return isactive.value;
        }

        public static void setCatActive(string catname, bool active)
        {
            SavedBool isactive = new SavedBool(m_settingsprefix + "Cat_" + catname + "_Active", Settings.gameSettingsFile, true, true);
            isactive.value = active;
        }

        public static bool isStatActive(string catname, string statname)
        {
            string savedstatname = m_settingsprefix + "Stat_" + catname + "_" + statname + "_Active";
            SavedBool isactive = new SavedBool(savedstatname, Settings.gameSettingsFile, true, true);
            if (!configurationsettings.Contains(isactive)) configurationsettings.Add(isactive);
            return isactive.value;
        }

        public static void setStatActive(string catname, string statname, bool active)
        {
            string savedstatname = m_settingsprefix + "Stat_" + catname + "_" + statname + "_Active";
            SavedBool isactive = new SavedBool(savedstatname, Settings.gameSettingsFile, true, true);
            isactive.value = active;
        }

        public static void resetConfig()
        {
            for (int i = 0; i < configurationsettings.Count(); i++)
            {
                try
                {
                    ((SavedValue)configurationsettings[i]).Delete();
                }
                catch { }
            }

            windowx.value = 0; windowy.value = 0;
            windoww.value = 700; windowh.value = 500;
            miniwindowx.value = 0; miniwindowy.value = 0;
            miniwindoww.value = 400; miniwindowh.value = 200;

            fontchange.value = 0;
            minifontchange.value = 0;
            m_MiniMode.value = false;
            m_PanelRefreshRate.value = 3;
            m_ShowLabelsInMiniMode.value = false;
        }
    }
}
