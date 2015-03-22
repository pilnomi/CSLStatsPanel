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
                m_initialized = true;
                StatusWindowInterface.init();
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
                //update about once every 3 seconds at 60fps. Definately gotta be a better way to do this just a sample.
                //but the point is don't call your update code on every single frame\tick unless
                //you truely have a need for that.
                numberofcalls++;
                if (numberofcalls < 60 * 3) return;
                numberofcalls = 0;
                if (man.loading.loadingComplete & m_initialized)
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

        public class StatisticsClassWrapper
        {
            public StatisticType m_st;
            public string m_desc;
            public decimal m_scale, m_multiplier;
            public string m_scaledesc;
            public string statstring;
            public string category;

            public StatisticsClassWrapper(string category, string description, StatisticType st, decimal scale, decimal multiplier, string scaledescription, int precision = 2)
            {
                this.category = category;
                m_st = st;
                m_desc = description;
                m_scale = scale;
                m_multiplier = multiplier;
                m_scaledesc = scaledescription;
                statstring = getstatstring(m_desc, m_st, m_multiplier, m_scale, m_scaledesc, precision);
            }
            public StatisticsClassWrapper(string category, string description)
            {
                this.category = category;
                statstring = description;
            }
            public StatisticsClassWrapper(string category, string description, double value, int precision = 2, string suffix = "")
            {
                this.category = category;
                m_desc = description;
                statstring = description + ": " + Math.Round(value, precision).ToString() + suffix;

            }

            string getstatstring(string desc, StatisticType st,
                decimal multiplier = 16, decimal scale = 1000, string scalestring = "M", int precision=2)
            {
                statlog.log(st);
                try
                {
                    StatisticsManager sm = Singleton<StatisticsManager>.instance;
                    while (sm.IsInvoking()) System.Threading.Thread.Sleep(1);
                    StatisticBase sb = sm.Get(st);
                    if (sb != null)
                    {
                        float total = (float)Math.Round(sb.GetLatestFloat() * (float)multiplier,precision);
                        string strtotal = total.ToString();
                        if (total > (float)scale)
                        {
                            total /= (float)scale;
                            strtotal = Math.Round(total,precision).ToString() + scalestring;

                        }
                        return desc + ": " + strtotal + " ";
                    }
                    else return desc + ": N/A";
                }
                catch
                {
                    return desc + ": -1 ";
                }
            }


        }
    
        public static class statlog
        {
            public static void log(StatisticType st)
            {
                log(st.ToString());
            }

            static bool enablelogging = false;
            static bool keeponlylastmessage = false;
            public static void log(string logtext)
            {
                if (!enablelogging) return;
                
                System.IO.StreamWriter wr;
                if (System.IO.File.Exists("cslstatslog.txt") && !keeponlylastmessage)
                {
                    wr = System.IO.File.AppendText("cslstatslog.txt");
                }
                else wr = System.IO.File.CreateText("cslstatslog.txt");
                wr.WriteLine(logtext);
                wr.Close();
                
            }
        }
}

