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
            //statlog.log("LoadingCSLStatsMod.OnLevelLoaded");
            StatusWindowInterface.destroy();
            if (ThreadingCSLStatsMod.instance != null)
            {
                ThreadingCSLStatsMod.instance.refreshtimer.Stop();
                ThreadingCSLStatsMod.instance.m_initialized = false;
                statlog.log("reset ThreadingCSLStatsMod");
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

        public bool init()
        {
            if (m_initialized == true) return true;
            StatusWindowInterface.init();
            m_initialized = true;
            settimer();
            return true;
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
                statlog.log(ex.Message);
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
            catch (Exception ex) { statlog.log(ex.Message); }
        }
    }

    
    public static class statlog
        {
            public static bool enablelogging = false;
            public static void log(string logtext)
            {
                if (!enablelogging) return;
                //UnityEngine.Debug.Log(logtext);
                
                DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Message, logtext);
            }
        }


}

