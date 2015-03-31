using ColossalFramework;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


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
    public static class ThreadTracker
    {
        public static ThreadingCSLStatsMod instance = null;

    }
    public class LoadingCSLStatsMod : LoadingExtensionBase
    {
        //handles loading the threading mod
        //seems to avoid the "load game" problem while in-game
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
                return;
            //DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Message, "LoadingCSLStatsMod.OnLevelLoaded");
            //UnityEngine.Debug.Log("LoadingCSLStatsMod.OnLevelLoaded");
            if (ThreadTracker.instance == null) ThreadTracker.instance = new ThreadingCSLStatsMod();
            ThreadTracker.instance.m_initialized = false;
            StatusWindowInterface.reset();
            ThreadTracker.instance.init();
            
            base.OnLevelLoaded(mode);
        }

        public override void OnLevelUnloading()
        {
            StatusWindowInterface.reset();
            ThreadTracker.instance = null;
            base.OnLevelUnloading();
        }

        public override void OnReleased()
        {
            StatusWindowInterface.reset();
            ThreadTracker.instance = null;
            base.OnReleased();
        }
    }
    public class ThreadingCSLStatsMod : ThreadingExtensionBase
    {
        public bool m_initialized = false;

        ~ThreadingCSLStatsMod()
        {
            m_initialized = false;
            StatusWindowInterface.reset();
        }

        public override void OnReleased()
        {
            m_initialized = false;
            StatusWindowInterface.reset();
            base.OnReleased();
        }

        public bool init()
        {
            if (m_initialized == true) return true;
            StatusWindowInterface.init();
            m_initialized = true;
            return true;
        }


        //DateTime Time = DateTime.Now;
        public static double framespersecond = 0.0f;
        private static double m_realTimeDelta = 0.0f;
        private int minimumFramesBetweenCalls = 20;
        static int numberofcalls = 0; //track approximate fps

        //called about ~60 times per second. (or max frame rate user is getting atm imagine)
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            numberofcalls++;
            m_realTimeDelta += realTimeDelta;
            int myrefreshrate = CSLStatsPanelConfigSettings.PanelRefreshRate;
            
            if (StatusWindowInterface.configChanged) myrefreshrate = 1; // temporarily change update rate to 1sec after config change
            if (StatusWindowInterface.doReset) myrefreshrate = 1;
            if (StatusWindowInterface.running) return;
            if (m_realTimeDelta < myrefreshrate || numberofcalls < minimumFramesBetweenCalls) return;
            framespersecond = numberofcalls / m_realTimeDelta;
                
            //Time = DateTime.Now;
            numberofcalls = 0;
            m_realTimeDelta = 0.0f;
            if (m_initialized)
            {
                try
                {
                    StatusWindowInterface.updateText();
                }
                catch (Exception ex) { UnityEngine.Debug.Log(ex.Message); }
            }
            else
            {
                init();  //were we not able to init previously? if so try now.
                if (m_initialized) StatusWindowInterface.updateText(); ;
            }

            base.OnUpdate(realTimeDelta, simulationTimeDelta);
        }
    }

    
    public static class statlog
        {
            public static void log(StatisticType st)
            {
                log(st.ToString());
            }

            public static bool enablelogging = false;
            //static bool keeponlylastmessage = false;
            public static void log(string logtext)
            {
                if (!enablelogging) return;
                UnityEngine.Debug.Log(logtext);
            }
        }


}

