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
    
        public class ThreadingCSLStatsMod : ThreadingExtensionBase
        {
            bool m_initialized = false;
            
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

            static IManagers man;
            public override void OnCreated(IThreading threading)
            {
                if (man == null)
                { man = threading.managers; } //store for later use
                if (man.loading.loadingComplete)
                { init(); }//initialize
                base.OnCreated(threading);
            }
            

            static int numberofcalls = 0; //track approximate fps
            //called about ~60 times per second. (or max frame rate user is getting atm imagine)
            public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
            {
                if (man.loading.loadingComplete && !m_initialized) init();
                //update about once every 3 seconds at 60fps. 
                numberofcalls++;
                if (numberofcalls < 60 * 3) return;
                numberofcalls = 0;
                if (man.loading.loadingComplete & m_initialized)
                {
                    StatusWindowInterface.updateText();
                }
                else if (man.loading.loadingComplete)
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
                /*
                System.IO.StreamWriter wr;
                if (System.IO.File.Exists("cslstatslog.txt") && !keeponlylastmessage)
                {
                    wr = System.IO.File.AppendText("cslstatslog.txt");
                }
                else wr = System.IO.File.CreateText("cslstatslog.txt");
                wr.WriteLine(logtext);
                wr.Close();
                */
                //ModTools.Log.Message(logtext);
            }
        }


}

