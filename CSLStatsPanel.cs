using ColossalFramework;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * CSL Stats Panel, a UI mod for Cities: Skylines
 * Created and maintained by Operation40
 * Github: https://github.com/pilnomi/CSLStatsPanel
 */
namespace CSLStatsPanel 
{

    public class CSLStatsPanelMod : IUserMod
    {



        public string Name 
        {
            get { return "CSL Stats Panel"; }
        }

        public string Description 
        {
            get { return "All your city service stats in one place"; }
        }
        
        
    }

    public class LoadingCSLStatsMod : LoadingExtensionBase
    {
        //handles loading the threading mod
        //seems to avoid the "load game" problem while in-game
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
                return;
            StatusWindowInterface.destroy();
            if (ThreadingCSLStatsMod.instance != null)
            {
                ThreadingCSLStatsMod.instance.refreshtimer.Stop();
                ThreadingCSLStatsMod.instance.m_initialized = false;
            }
        }

        public override void OnLevelUnloading()
        {
            if (ThreadingCSLStatsMod.instance != null) ThreadingCSLStatsMod.instance.refreshtimer.Stop();
            StatusWindowInterface.destroy();
            base.OnLevelUnloading();
        }

        public override void OnReleased()
        {
            if (ThreadingCSLStatsMod.instance != null) ThreadingCSLStatsMod.instance.refreshtimer.Stop();
            StatusWindowInterface.destroy();
            base.OnReleased();
        }
    }

    
    public class ThreadingCSLStatsMod : ThreadingExtensionBase
    {
        public static ThreadingCSLStatsMod instance = null;
        public bool m_initialized = false;

        ~ThreadingCSLStatsMod()
        {
            refreshtimer.Stop();
            m_initialized = false;
            StatusWindowInterface.destroy();
        }

        public override void OnReleased()
        {
            refreshtimer.Stop();
            m_initialized = false;
            StatusWindowInterface.destroy();
            base.OnReleased();
        }

        public static string defaultXMLConfig = "";
        public bool init()
        {
            if (m_initialized == true) return true;
            StatusWindowInterface.init();
            m_initialized = true;
            settimer();
            loadConfigFile();

            
            
            return true;
        }

        public static string loadedXMLConfigVersion = "0", defaultXMLConfigVersion = "0";
        public static string getXMLConfigVersion(string xmlconfigstring)
        {
            string r = "";
            try
            {
                System.Xml.XmlDocument xdoc = new System.Xml.XmlDocument();
                xdoc.LoadXml(xmlconfigstring);
                System.Xml.XmlNodeList nodes = xdoc.GetElementsByTagName("CONFIGVERSION");
                if (nodes.Count > 0)
                {
                    try
                    {
                        r = nodes[0].Attributes["value"].Value;
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                statlog.log("loadConfigfile, parse config: " + ex.Message);
            }
            return r;
        }

        public void loadConfigSettings(string xmlconfigstring)
        {
            System.Xml.XmlDocument xdoc = new System.Xml.XmlDocument();
            xdoc.LoadXml(xmlconfigstring);
            System.Xml.XmlNodeList nodes = xdoc.GetElementsByTagName("CONFIGSETTINGS");
            if (nodes.Count > 0)
            {
                System.Xml.XmlNodeList n = xdoc.GetElementsByTagName("LOGGING");
                if (n.Count > 0) 
                {
                    bool.TryParse(XMLHelper.safeAttributes(n[0], "enabled"), out statlog.enablelogging);
                    bool.TryParse(XMLHelper.safeAttributes(n[0], "tofile"), out statlog.enablelogtofile);
                    bool.TryParse(XMLHelper.safeAttributes(n[0], "tof7screen"), out statlog.enablelogtoscreen);
                    if (statlog.enablelogging) statlog.log("logging enabled");
                }

                n = xdoc.GetElementsByTagName("COLORS");
                if (n.Count > 0)
                {
                    string color = XMLHelper.safeAttributes(n[0], "defaultPanelColor");
                    if (color.Split(',').Length == 4)
                        CSLStatsPanelConfigSettings.DefaultPanelColor = parseColor(color);
                    color = XMLHelper.safeAttributes(n[0], "transparentPanelColor");
                    if (color.Split(',').Length == 4)
                        CSLStatsPanelConfigSettings.TransparentPanelColor = parseColor(color);

                    color = XMLHelper.safeAttributes(n[0], "defaultPanelColor_NormalStatus");
                    statlog.log(color + " " + color.Split(',').Length.ToString());

                    if (color.Split(',').Length == 4)
                        CSLStatsPanelConfigSettings.DefaultPanelColor_NormalStatus = parseColor(color);
                    color = XMLHelper.safeAttributes(n[0], "defaultPanelColor_WarningStatus");
                    if (color.Split(',').Length == 4)
                        CSLStatsPanelConfigSettings.DefaultPanelColor_WarningStatus = parseColor(color);
                    color = XMLHelper.safeAttributes(n[0], "defaultPanelColor_CriticalStatus");
                    if (color.Split(',').Length == 4)
                        CSLStatsPanelConfigSettings.DefaultPanelColor_CriticalStatus = parseColor(color);
                }
            }
        }

        UnityEngine.Color32 parseColor(string color)
        {
            byte r = 0, g = 0, b = 0, a = 255;
            byte.TryParse(color.Split(',')[0].Trim(), out r);
            byte.TryParse(color.Split(',')[1].Trim(), out g);
            byte.TryParse(color.Split(',')[2].Trim(), out b);
            byte.TryParse(color.Split(',')[3].Trim(), out a);
            return new UnityEngine.Color32(r, g, b, a);
        }

        public void loadConfigFile()
        {
            try
            {
                defaultXMLConfig = GetResourceTextFile("CSLStatsPanelConfig.xml");
                defaultXMLConfigVersion = getXMLConfigVersion(defaultXMLConfig);

                string mydocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (System.IO.File.Exists(mydocs + "\\CSLStatsPanel\\CSLStatsPanelConfig.xml"))
                {
                    defaultXMLConfig = System.IO.File.ReadAllText(mydocs + "\\CSLStatsPanel\\CSLStatsPanelConfig.xml");
                    loadedXMLConfigVersion = getXMLConfigVersion(defaultXMLConfig);
                }
                else loadedXMLConfigVersion = defaultXMLConfigVersion;

                loadConfigSettings(defaultXMLConfig);
            }
            catch (Exception ex)
            {
                statlog.log("loadConfigfile: " + ex.Message);
                loadedXMLConfigVersion = defaultXMLConfigVersion;
            }
            finally
            {
            }
        }

        public override void OnCreated(IThreading threading)
        {
            instance = this;
            base.OnCreated(threading);
        }
        

        public static double framespersecond = 0.0f;
        private static double m_realTimeDelta = 0.0f;
        private int minimumFramesBetweenCalls = 20;
        static int numberofcalls = 0; //track approximate fps
        public System.Timers.Timer refreshtimer = new System.Timers.Timer();

        public void settimer()
        {
            refreshtimer = new System.Timers.Timer(1000);
            refreshtimer.Elapsed += new System.Timers.ElapsedEventHandler(refreshtimer_Elapsed);
            refreshtimer.Enabled = true;
            refreshtimer.Start();
        }

        void refreshtimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            refreshtimer.Stop();
            try
            {
                StatusWindowInterface.cacheddata = CSLStatsPanelConfigSettings.Categories(true);
            }
            catch(Exception ex)
            {
                statlog.log("refreshtimer_elapsed: " + ex.Message);
            }
            finally
            {
                int myrefreshrate = CSLStatsPanelConfigSettings.PanelRefreshRate;
                refreshtimer.Interval = myrefreshrate * 1000 - 10;
                if (refreshtimer.Interval < 990) refreshtimer.Interval = 990;
                refreshtimer.Start();
            }
            
        }


        //called about ~60 times per second. (or max frame rate user is getting atm imagine)
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            if (!m_initialized) init();
            numberofcalls++;
            m_realTimeDelta += realTimeDelta;
            framespersecond = numberofcalls / m_realTimeDelta;

            int myrefreshrate = CSLStatsPanelConfigSettings.PanelRefreshRate,
                myminframes = minimumFramesBetweenCalls;
            if (CSLStatsPanelConfigSettings.m_ConfigChanged.value)
            {
                StatusWindowInterface.cacheddata = null;
                CSLStatsPanelConfigSettings.m_ConfigChanged.value = false;
                loadConfigFile();
                StatusWindowInterface.resetstatswindow();
                StatusWindowInterface.doReset = true;
                myrefreshrate = 0; myminframes = 2;
            }
            //if (StatusWindowInterface.configChanged) myrefreshrate = 1; // temporarily change update rate to 1sec after config change
            //if (StatusWindowInterface.doReset) myrefreshrate = 1;
            if (StatusWindowInterface.running) return;

            if (m_realTimeDelta < myrefreshrate || numberofcalls < myminframes) return;
            numberofcalls = 0;
            m_realTimeDelta = 0.0f;
            try
            {
                StatusWindowInterface.updateText();
            }
            catch (Exception ex) { statlog.log("statuswindowinterface.updatetext: " + ex.Message); }
        }

        public string GetResourceTextFile(string filename)
        {
            string result = string.Empty;
            //foreach (string s in this.GetType().Assembly.GetManifestResourceNames())
            //    statlog.log(s);

            using (System.IO.Stream stream = this.GetType().Assembly.
                       GetManifestResourceStream("CSLStatsPanel." + filename))
            {
                if (stream == null) statlog.log("default config stream null");
                using (System.IO.StreamReader sr = new System.IO.StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }

    }

    
    public static class statlog
        {
            public static bool enablelogging = true, enablelogtofile = true, enablelogtoscreen = false;
            public static void log(string logtext)
            {
                if (!enablelogging) return;
                //UnityEngine.Debug.Log(logtext);

                if (enablelogtofile) System.IO.File.AppendAllText(@"cslstatspanelog.txt", System.DateTime.Now.ToString() + logtext + Environment.NewLine);
                if (enablelogtoscreen) DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Message, "CSLStatsPanel: " + logtext);
            }
        }

    public static class XMLHelper
    {
        public static string safeAttributes(System.Xml.XmlNode n, string attributename)
        {
            string r = "";
            try
            {
                r = n.Attributes[attributename].Value;
            }
            catch { }
            return r;
        }
    }

    public static class DataHelper
    {

        public static bool isnumeric(object o)
        {
            decimal d = 0;
            return decimal.TryParse(o.ToString(), out d);
        }

    }
}

