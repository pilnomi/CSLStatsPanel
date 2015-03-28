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


            var statButton = (UIButton)UIView.GetAView().AddUIComponent(typeof(UIButton));
            statButton.width = 125;
            statButton.height = 30;
            statButton.normalBgSprite = "ButtonMenu";
            statButton.hoveredBgSprite = "ButtonMenuHovered";
            statButton.focusedBgSprite = "ButtonMenuFocused";
            statButton.pressedBgSprite = "ButtonMenuPressed";
            statButton.textColor = new Color32(186, 217, 238, 0);
            statButton.disabledTextColor = new Color32(7, 7, 7, 255);
            statButton.hoveredTextColor = new Color32(7, 132, 255, 255);
            statButton.focusedTextColor = new Color32(255, 255, 255, 255);
            statButton.pressedTextColor = new Color32(30, 30, 44, 255);
            statButton.transformPosition = new Vector3(1.2f, -0.93f);
            statButton.BringToFront();
            statButton.text = "CSL Stats Panel";
            statButton.eventClick += new MouseEventHandler(statButton_eventClick);
            
            initialized = true;
            running = false;
            if (CSLStatsPanelConfigSettings.m_DisplayPanel.value) statButton_eventClick(null, null);

        }

        public static bool configChanged
        {
            get
            {
                if (!initialized) return false;
                if (myStatsWindowPanel == null) return false;
                return myStatsWindowPanel.configChanged;
            }
            set
            {
                myStatsWindowPanel.configChanged = value;
            }
        }
        
        static void statButton_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (myStatsWindowPanel == null)
            {
                CSLStatsPanelConfigSettings.m_DisplayPanel.value = true;
                UIView uiView = GameObject.FindObjectOfType<UIView>();
                if (uiView == null) return;
                myStatsWindowPanel = (CSLStatsMasterWindow)UIView.GetAView().AddUIComponent(typeof(CSLStatsMasterWindow));
                myStatsWindowPanel.name = "CSLStatsMasterPanel";
                myStatsWindowPanel.eventStatsConfigReset += new CSLStatsMasterWindow.eventStatsConfigResetHandler(myStatsWindowPanel_eventStatsConfigReset);
                updateText();
                updateText();
            }
            else
            {
                CSLStatsPanelConfigSettings.m_DisplayPanel.value = false;
                GameObject.Destroy(myStatsWindowPanel);
                myStatsWindowPanel = null;
            }
        }

        static void myStatsWindowPanel_eventStatsConfigReset(object sender, EventArgs e)
        {
            GameObject.Destroy(myStatsWindowPanel);
            myStatsWindowPanel = null;           
        }


        public static void reset()
        {
            GameObject.Destroy(myStatsWindowPanel);
            myStatsWindowPanel = null;
            initialized = false;
            running = false;
        }

        public static void updateText()
        {
            if (myStatsWindowPanel == null) return;
            if (!initialized) return;
            if (running) return;
            running = true;
            if (configChanged) myStatsWindowPanel.updateText(CSLStatsPanelConfigSettings.Categories(true));
            configChanged = false;
            //myStatsWindowPanel.getstats2();
            myStatsWindowPanel.updateText(CSLStatsPanelConfigSettings.Categories(true));
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
        bool dragging = false;

        int minfontsize = -15, maxfontsize = 20;
        float fontincr = .05f;

        public CSLStatsMasterWindow()
        {
            init();
        }

        bool isresetting = false;
        public void resetStatsWindow()
        {
            if (isresetting) return;
            if (myStatsWindowPanel == null) return;
            isresetting = true;
            myStatsWindowPanel.reset();
            firstrun = true;
            isresetting = false;
            
        }
        public void addStatsWindowPanel()
        {
            myStatsWindowPanel = (CSLStatusWindowPanel)this.AddUIComponent(typeof(CSLStatusWindowPanel));
            myStatsWindowPanel.name = "CSLStatsPanel";
            myStatsWindowPanel.color = CSLStatsPanelConfigSettings.DefaultPanelColor;
        }

        UIButton configButton = null;
        public void addConfigureButton()
        {
            if (configButton != null) this.RemoveUIComponent(configButton);
            
            UIScrollablePanel p = myresizepanel.AddUIComponent<UIScrollablePanel>();
            p.autoLayout = true;
            p.autoSize = true;
            p.autoLayoutDirection = LayoutDirection.Horizontal;
            p.autoLayoutStart = LayoutStart.TopLeft;
            
            configButton = (UIButton)p.AddUIComponent(typeof(UIButton));
            configButton.width = 125;
            configButton.height = 20;
            configButton.normalBgSprite = "ButtonMenu";
            configButton.hoveredBgSprite = "ButtonMenuHovered";
            configButton.focusedBgSprite = "ButtonMenuFocused";
            configButton.pressedBgSprite = "ButtonMenuPressed";
            configButton.textColor = new Color32(186, 217, 238, 0);
            configButton.disabledTextColor = new Color32(7, 7, 7, 255);
            configButton.hoveredTextColor = new Color32(7, 132, 255, 255);
            configButton.focusedTextColor = new Color32(255, 255, 255, 255);
            configButton.pressedTextColor = new Color32(30, 30, 44, 255);
            configButton.color = new Color32(configButton.color.r, configButton.color.g, configButton.color.b, 255);
            //configButton.transformPosition = new Vector3(1.2f, -0.93f);
            configButton.BringToFront();
            configButton.text = "Configure";
            configButton.eventClick += new MouseEventHandler(configButton_eventClick);

            UIButton modetoggle = (UIButton)p.AddUIComponent(typeof(UIButton));
            modetoggle.width = 125;
            modetoggle.height = 20;
            modetoggle.normalBgSprite = "ButtonMenu";
            modetoggle.hoveredBgSprite = "ButtonMenuHovered";
            modetoggle.focusedBgSprite = "ButtonMenuFocused";
            modetoggle.pressedBgSprite = "ButtonMenuPressed";
            modetoggle.textColor = new Color32(186, 217, 238, 0);
            modetoggle.disabledTextColor = new Color32(7, 7, 7, 255);
            modetoggle.hoveredTextColor = new Color32(7, 132, 255, 255);
            modetoggle.focusedTextColor = new Color32(255, 255, 255, 255);
            modetoggle.pressedTextColor = new Color32(30, 30, 44, 255);
            modetoggle.color = new Color32(configButton.color.r, configButton.color.g, configButton.color.b, 255);
            //configButton.transformPosition = new Vector3(1.2f, -0.93f);
            modetoggle.BringToFront();
            modetoggle.text = (CSLStatsPanelConfigSettings.m_MiniMode.value) ? "Expand" : "Mini";
            modetoggle.eventClick += new MouseEventHandler(modetoggle_eventClick);

            modetoggle.spritePadding = new RectOffset(125/2, 0, 0, 0);
            //p.FitChildrenHorizontally();
            p.FitChildrenVertically();
            //p.FitToContents();
            //myresizepanel.FitChildrenVertically();
            myresizepanel.FitChildrenVertically();
        }

        void modetoggle_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            CSLStatsPanelConfigSettings.m_MiniMode.value = !CSLStatsPanelConfigSettings.m_MiniMode.value;
            ((UIButton)component).text = (CSLStatsPanelConfigSettings.m_MiniMode.value) ? "Expand" : "Mini";
            component.parent.Focus();
            myconfigwindow_eventModeConfigChanged(this, EventArgs.Empty);

        }


        public void init()
        {
            this.color = CSLStatsPanelConfigSettings.DefaultPanelColor;
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

            addStatsWindowPanel();

            myresizepanel = (UIResizeHandle)this.AddUIComponent(typeof(UIResizeHandle));
            myresizepanel.name = "CSLStatsResizePanel";
            myresizepanel.height = 20;
            myresizepanel.color = new Color32(0, 0, 100, 100);
            myresizepanel.backgroundSprite = "GenericPanel";
            //myresizepanel.anchor = UIAnchorStyle.Bottom;
            //myresizepanel.anchor = UIAnchorStyle.Right;
            resizelabel = myresizepanel.AddUIComponent<UILabel>();

            addConfigureButton();
            setdefaultpos();
        }


        public override void Start()
        {
            base.Start();
 
        }
        public override void OnDestroy()
        {
            if (myconfigwindow != null) GameObject.Destroy(myconfigwindow);
            base.OnDestroy();
        }

        ConfigWindow myconfigwindow = null;
        void configButton_eventClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (myconfigwindow == null)
            {
                myconfigwindow = (ConfigWindow)UIView.GetAView().AddUIComponent(typeof(ConfigWindow));
                myconfigwindow.eventStatsConfigChanged += new ConfigWindow.eventStatsConfigChangedHandler(myconfigwindow_eventStatsConfigChanged);
                myconfigwindow.eventModeConfigChanged += new ConfigWindow.eventConfigModeChangedHandler(myconfigwindow_eventModeConfigChanged);
                myconfigwindow.eventStatsConfigReset += new ConfigWindow.eventStatsConfigResetHandler(myconfigwindow_eventStatsConfigReset);
            }
            else
            {
                GameObject.Destroy(myconfigwindow);
                myconfigwindow = null;
            }
            component.parent.Focus();
        }

        void myconfigwindow_eventStatsConfigReset(object sender, EventArgs e)
        {
            eventStatsConfigReset(sender, e);
        }
        public delegate void eventStatsConfigResetHandler(object sender, EventArgs e);
        public event eventStatsConfigResetHandler eventStatsConfigReset;


        void myconfigwindow_eventModeConfigChanged(object sender, EventArgs e)
        {
            configChanged = true;
            resetStatsWindow();
            setdefaultpos();            
        }

        public bool configChanged = false;
        void myconfigwindow_eventStatsConfigChanged(object sender, EventArgs e)
        {
            configChanged = true;
            resetStatsWindow();
        }

        void setdefaultpos()
        {
            
            CSLStatsPanelConfigSettings.mySavedFloat windowx = (!CSLStatsPanelConfigSettings.m_MiniMode.value) ? CSLStatsPanelConfigSettings.windowx : CSLStatsPanelConfigSettings.miniwindowx,
                windowy = (!CSLStatsPanelConfigSettings.m_MiniMode.value) ? CSLStatsPanelConfigSettings.windowy : CSLStatsPanelConfigSettings.miniwindowy,
                windoww = (!CSLStatsPanelConfigSettings.m_MiniMode.value) ? CSLStatsPanelConfigSettings.windoww : CSLStatsPanelConfigSettings.miniwindoww,
                windowh = (!CSLStatsPanelConfigSettings.m_MiniMode.value) ? CSLStatsPanelConfigSettings.windowh : CSLStatsPanelConfigSettings.miniwindowh;
            CSLStatsPanelConfigSettings.mySavedInt fontchange = (!CSLStatsPanelConfigSettings.m_MiniMode.value) ? CSLStatsPanelConfigSettings.fontchange : CSLStatsPanelConfigSettings.minifontchange;


            this.position = new Vector3(windowx.value, windowy.value, this.position.z);
            this.width = windoww.value;
            this.height = windowh.value;
            if (this.width == 0) this.width = 700;
            if (this.height == 0) this.height = 400;

            myStatsWindowPanel.width = this.width;
            myStatsWindowPanel.height = this.height - headerpanel.height - myresizepanel.height;
            
            if (fontchange.value < minfontsize) fontchange.value = minfontsize;
            if (fontchange.value > maxfontsize) fontchange.value = maxfontsize;
            foreach (KeyValuePair<string, CSLStatusWindowSubPanel> subpanel in myStatsWindowPanel.m_categories)
            {
                foreach (CSLStatsPanelLabel l in subpanel.Value.m_textfields)
                {
                    l.textScale += fontincr * fontchange.value;
                }
                if (subpanel.Value.spritepanel != null)
                {
                    UIScrollablePanel sp = subpanel.Value.GetComponentInChildren<UIScrollablePanel>();
                    UISprite s = subpanel.Value.GetComponentInChildren<UISprite>();
                    UILabel l = subpanel.Value.GetComponentInChildren<UILabel>();
                    l.padding = new RectOffset(l.padding.left + (1 * fontchange.value), l.padding.right, l.padding.top, l.padding.bottom);
                    s.width += 1 * fontchange.value;
                    s.height += 1 * fontchange.value;
                    //sp.FitChildrenHorizontally();
                    //sp.FitToContents();
                }
                
                subpanel.Value.FitToContents();
                
            }
            OnSizeChanged();
        }

        public void updateText(List<string> s) { myStatsWindowPanel.updateText(s);}
        public void updateText(List<StatisticsCategoryWrapper> l) 
        {
            if (isresetting) return;
            myStatsWindowPanel.updateText(l);
            if (firstrun) setdefaultpos();
            firstrun = false;
        }
        protected override void OnSizeChanged()
        {
            CSLStatsPanelConfigSettings.mySavedFloat windowx = (!CSLStatsPanelConfigSettings.m_MiniMode.value) ? CSLStatsPanelConfigSettings.windowx : CSLStatsPanelConfigSettings.miniwindowx,
                windowy = (!CSLStatsPanelConfigSettings.m_MiniMode.value) ? CSLStatsPanelConfigSettings.windowy : CSLStatsPanelConfigSettings.miniwindowy,
                windoww = (!CSLStatsPanelConfigSettings.m_MiniMode.value) ? CSLStatsPanelConfigSettings.windoww : CSLStatsPanelConfigSettings.miniwindoww,
                windowh = (!CSLStatsPanelConfigSettings.m_MiniMode.value) ? CSLStatsPanelConfigSettings.windowh : CSLStatsPanelConfigSettings.miniwindowh;

            
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
       
        protected override void OnMouseDown(UIMouseEventParameter p)
        {
            dragging = true;
        }
        protected override void OnMouseUp(UIMouseEventParameter p)
        {
            if (dragging)
            {
                CSLStatsPanelConfigSettings.mySavedFloat windowx = (!CSLStatsPanelConfigSettings.m_MiniMode.value) ? CSLStatsPanelConfigSettings.windowx : CSLStatsPanelConfigSettings.miniwindowx,
                    windowy = (!CSLStatsPanelConfigSettings.m_MiniMode.value) ? CSLStatsPanelConfigSettings.windowy : CSLStatsPanelConfigSettings.miniwindowy,
                    windoww = (!CSLStatsPanelConfigSettings.m_MiniMode.value) ? CSLStatsPanelConfigSettings.windoww : CSLStatsPanelConfigSettings.miniwindoww,
                    windowh = (!CSLStatsPanelConfigSettings.m_MiniMode.value) ? CSLStatsPanelConfigSettings.windowh : CSLStatsPanelConfigSettings.miniwindowh;

                
                windowx.value = this.position.x; windowy.value = this.position.y;
                windoww.value = this.width; windowh.value = this.height;
            }
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

        bool childrenareclipped
        {
            get
            {
                foreach (KeyValuePair<string, CSLStatusWindowSubPanel> subpanel in myStatsWindowPanel.m_categories)
                {
                    if (subpanel.Value.IsClippedFromParent())
                    {
                        return true;
                        
                    }

                }
                return false;
            }
        }
        bool zooming = false;
        protected override void OnMouseWheel(UIMouseEventParameter p)
        {
            CSLStatsPanelConfigSettings.mySavedInt fontchange = (!CSLStatsPanelConfigSettings.m_MiniMode.value) ? CSLStatsPanelConfigSettings.fontchange : CSLStatsPanelConfigSettings.minifontchange;
            if (!zooming)
            {
                float wd = p.wheelDelta;
                if (wd < 0)
                {
                    if (fontchange.value <= minfontsize) return;
                    fontchange.value--;
                }
                if (wd > 0)
                {
                    if (fontchange.value >= maxfontsize) return;
                    fontchange.value++;
                }
                zooming = true;
                foreach (KeyValuePair<string, CSLStatusWindowSubPanel> subpanel in myStatsWindowPanel.m_categories)
                {
                    if (subpanel.Value.spritepanel != null)
                    {
                        UISprite s = subpanel.Value.spritepanel.GetComponentInChildren<UISprite>();
                        s.width += (wd > 0) ? 1 : -1;
                        s.height += (wd > 0) ? 1 : -1;
                        UILabel l = subpanel.Value.GetComponentInChildren<UILabel>();
                        l.padding = new RectOffset(l.padding.left + ((wd > 0) ? 1 : -1), l.padding.right, l.padding.top, l.padding.bottom);
                    
                    }
                    foreach (CSLStatsPanelLabel l in subpanel.Value.m_textfields)
                    {
                        if (wd < 0)
                        {
                            l.textScale -= fontincr;
                        }
                        else
                        {
                            l.textScale += fontincr;
                        }

                    }
                    subpanel.Value.FitToContents();
                }

            }
            zooming = false;
            base.OnMouseWheel(p);
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
        public UIScrollablePanel spritepanel = null;
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

        public void reset()
        {
            foreach (KeyValuePair<string, CSLStatusWindowSubPanel> p in m_categories)
            {
                this.RemoveUIComponent(p.Value);
                UnityEngine.Object.Destroy(p.Value);
                // GameObject.Destroy(p.Value);
            }
            m_categories = new Dictionary<string, CSLStatusWindowSubPanel>();
            m_textfields = new List<CSLStatsPanelLabel>();
            firstrun = true;
            mycount = 0;
            running = false;
            
        }

        public void init()
        {
            if (initialized) return;
            statlog.log("CSLStatusWindowPanel init - subpanel=" + m_issubpanel.ToString());
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
            base.Start();
        }

        public void updateText(List<StatisticsCategoryWrapper> categorydata)
        {
            statlog.log("update text catwrapper initialized=" + initialized.ToString() + " running=" + running.ToString());
            if (!initialized) init();
            if (!initialized) return;
            if (running) return;
            running = true;

            statlog.log("reseting stringbuilders");
            foreach (KeyValuePair<string, CSLStatusWindowSubPanel> p in m_categories)
                p.Value.m_stringbuilder = new List<string>();

            statlog.log("looping categories" + categorydata.Count().ToString());
            for (int i = 0; i < categorydata.Count(); i++)
            {
                if (categorydata[i].m_scwlist.Count() == 0) continue;
                string currentcat = categorydata[i].m_category;
                if (string.IsNullOrEmpty(currentcat)) currentcat = "default";

                if (!m_categories.Keys.Contains(currentcat))
                {
                    statlog.log("adding category " + currentcat);
                    m_categories.Add(currentcat, (CSLStatusWindowSubPanel)this.AddUIComponent(typeof(CSLStatusWindowSubPanel)));
                    if (!string.IsNullOrEmpty(categorydata[i].m_sprite))
                    {
                        UIScrollablePanel spritepanel = m_categories[currentcat].AddUIComponent<UIScrollablePanel>();

                        UISprite p = spritepanel.AddUIComponent<UISprite>();
                        spritepanel.autoSize = true;
                        p.color = new Color32(255, 255, 255, 255);
                        p.spriteName = categorydata[i].m_sprite; 
                        p.opacity = 1;

                        p.width = 20; p.height = 20;
                  
                        spritepanel.autoLayoutDirection = LayoutDirection.Horizontal;
                        //spritepanel.autoLayoutPadding = new RectOffset(1, 1, 1, 1);
                        CSLStatsPanelLabel l = (CSLStatsPanelLabel)spritepanel.AddUIComponent(typeof(CSLStatsPanelLabel));
                        m_categories[currentcat].m_textfields.Add(l);
                        m_categories[currentcat].spritepanel = spritepanel;
                        
                        l.padding = new RectOffset(20, 1, 1, 1);
                        
                    }

                }

                if (categorydata[i].capacityUsage > -1 && CSLStatsPanelConfigSettings.m_EnablePanelColors.value)
                {
                    if (categorydata[i].capacityUsage > .95)
                        m_categories[currentcat].color = new Color32(255, 0, 0, 200); //red
                    else if (categorydata[i].capacityUsage > .75)
                        m_categories[currentcat].color = new Color32(255, 255, 0, 200); //yellow
                    else m_categories[currentcat].color = new Color32(0, 255, 0, 200); //green
                }

                List<StatisticsClassWrapper> myscwlist = categorydata[i].activeStats;
                for (int c = 0; c < myscwlist.Count(); c++)
                {
                    if (!CSLStatsPanelConfigSettings.m_MiniMode.value)
                    {
                        if (m_categories[currentcat].m_stringbuilder.Count() == 0)
                            m_categories[currentcat].m_stringbuilder.Add(currentcat
                                + ((CSLStatsPanelConfigSettings.m_EnablePanelSummaries.value && categorydata[i].m_showstatsummary && categorydata[i].capacityUsage > -1) ? " - " + Math.Round(categorydata[i].capacityUsage * 100, 0).ToString() + "%" : ""));


                        m_categories[currentcat].m_stringbuilder.Add(myscwlist[c].statstring);
                    }
                    else
                    {
                        if (m_categories[currentcat].m_stringbuilder.Count() == 0)
                        {
                            if (CSLStatsPanelConfigSettings.m_MiniMode.value 
                                && !CSLStatsPanelConfigSettings.m_ShowLabelsInMiniMode.value)
                            {
                                if (categorydata[i].m_showstatsummary && categorydata[i].capacityUsage > -1)
                                {
                                    m_categories[currentcat].m_stringbuilder.Add(
                                        (
                                        (categorydata[i].m_showstatsummary && categorydata[i].capacityUsage > -1) ? Math.Round(categorydata[i].capacityUsage * 100, 0).ToString() + "%" : ""));
                                }
                                else if (myscwlist.Count() > 0)
                                {
                                    m_categories[currentcat].m_stringbuilder.Add(
                                        Math.Round(myscwlist[0].m_value, 0) + myscwlist[0].m_scaledesc);

                                }

                            }
                            else
                            {

                                if (categorydata[i].m_showstatsummary && categorydata[i].capacityUsage > -1)
                                {
                                    m_categories[currentcat].m_stringbuilder.Add(currentcat
                                        + ((categorydata[i].m_showstatsummary && categorydata[i].capacityUsage > -1) ? " - " + Math.Round(categorydata[i].capacityUsage * 100, 0).ToString() + "%" : ""));
                                }
                                else if (myscwlist.Count() > 0)
                                {
                                    m_categories[currentcat].m_stringbuilder.Add(currentcat
                                        + " - " + Math.Round(myscwlist[0].m_value, 0) + myscwlist[0].m_scaledesc);

                                }
                            }
                        }
                        
                    }
                }
            }

            foreach (KeyValuePair<string, CSLStatusWindowSubPanel> p in m_categories)
            {
                statlog.log("calling updatetext on subpanel " + p.Key);
                p.Value.updateText(p.Value.m_stringbuilder);
                p.Value.m_stringbuilder = new List<string>();
            }
            if (firstrun && !m_issubpanel)
            {
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
                if (spritepanel != null)
                {
                    if (firstrun) spritepanel.FitChildrenHorizontally();
                    spritepanel.FitToContents();
                }
                this.FitChildrenVertically();
                this.FitToContents();
            }
            running = false;
        }

        ~CSLStatusWindowPanel()
        {

            m_textfields = new List<CSLStatsPanelLabel>();
            initialized = false;
            running = false;
        }

        
    }

    public class CSLStatsPanelLabel : UILabel
    {
        public CSLStatsPanelLabel()
        {
            this.color = new Color32(1, 1, 1, 255);
        }
        public override void Start()
        {
            this.backgroundSprite = "GenericLabel";
            this.autoSize = true;
            this.useDropShadow = false;
            this.color = new Color32(1, 1, 1, 255);

        }


    }
}
