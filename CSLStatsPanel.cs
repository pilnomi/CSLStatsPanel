﻿using ColossalFramework;
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
            //UnityEngine.Debug.Log("LoadingCSLStatsMod.OnLevelLoaded");
            if (ThreadTracker.instance == null) ThreadTracker.instance = new ThreadingCSLStatsMod();
            ThreadTracker.instance.m_initialized = false;
            StatusWindowInterface.reset();
            ThreadTracker.instance.init();
            
            base.OnLevelLoaded(mode);
        }
    }
    public class ThreadingCSLStatsMod : ThreadingExtensionBase
    {
        public bool m_initialized = false;
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
        static int numberofcalls = 0; //track approximate fps
        //called about ~60 times per second. (or max frame rate user is getting atm imagine)
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            //update about once every 3 seconds at 60fps. 
            numberofcalls++;
            m_realTimeDelta += realTimeDelta;
            int myrefreshrate = CSLStatsPanelConfigSettings.PanelRefreshRate;
            if (StatusWindowInterface.configChanged) myrefreshrate = 1; // temporarily change update rate to 1sec after config change
            if (StatusWindowInterface.doReset) myrefreshrate = 1;
            if (StatusWindowInterface.running) return;
            //if (numberofcalls < 60 * myrefreshrate) return;
            if (m_realTimeDelta < myrefreshrate) return ;
            //UnityEngine.Debug.Log("ThreadingCSLStatsMod.OnUpdate " + framespersecond.ToString());    
            framespersecond = numberofcalls / m_realTimeDelta;
                
            //Time = DateTime.Now;
            numberofcalls = 0;
            m_realTimeDelta = 0.0f;
            if (m_initialized)
            {
                StatusWindowInterface.updateText();
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

