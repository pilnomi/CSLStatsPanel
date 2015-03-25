using ColossalFramework;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSLStatsPanel
{

    public class StatisticsCategoryWrapper
    {
        public List<StatisticsClassWrapper> m_scwlist = new List<StatisticsClassWrapper>();
        public string m_category = "default";
        public string m_resourceusedfield = "";
        public string m_resourcecapacityfield = "";
        public float m_resourcecapacityliteral = -1f, m_resourceusedliteral = -1f;
        public string m_sprite = "GenericPanel";
        private int m_resourceusedindex = -1, m_resourcecapacityindex = -1;

        public StatisticsCategoryWrapper(string category, List<StatisticsClassWrapper> scwlist,
            string resourceusedfield, string resourcecapacityfield, string sprite = "GenericPanel")
        {
            m_scwlist = scwlist;
            m_category = category;
            m_resourcecapacityfield = resourcecapacityfield;
            m_resourceusedfield = resourceusedfield;
            m_sprite = sprite;

            for (int i = 0; i < m_scwlist.Count(); i++)
            {
                if (m_scwlist[i].m_desc == m_resourceusedfield && !string.IsNullOrEmpty(m_resourceusedfield)) m_resourceusedindex = i;
                if (m_scwlist[i].m_desc == m_resourcecapacityfield && !string.IsNullOrEmpty(m_resourcecapacityfield)) m_resourcecapacityindex = i;
                if (m_resourcecapacityindex > -1 && m_resourceusedindex > -1) break;
            }

            if (m_resourcecapacityindex == -1)
            {
                float t = -1;
                if (float.TryParse(m_resourcecapacityfield, out t))
                    m_resourcecapacityliteral = t;
            }
            if (m_resourceusedindex == -1)
            {
                float t = -1;
                if (float.TryParse(m_resourceusedfield, out t))
                    m_resourceusedliteral = t;
            }
        }

        public bool isDeficient
        {
            get
            {

                if ((m_resourceusedindex == -1 && m_resourceusedliteral == -1) ||
                    (m_resourcecapacityindex == -1 && m_resourcecapacityliteral == -1)) return false;
                float usedvalue = (m_resourceusedliteral == -1) ? m_scwlist[m_resourceusedindex].m_value : m_resourceusedliteral;
                float capacityvalue = (m_resourcecapacityliteral == -1) ? m_scwlist[m_resourcecapacityindex].m_value : m_resourcecapacityliteral;
                if (usedvalue > capacityvalue) return true;
                return false;
            }
        }

        public float capacityUsage
        {
            get
            {
                if ((m_resourceusedindex == -1 && m_resourceusedliteral == -1) ||
                    (m_resourcecapacityindex == -1 && m_resourcecapacityliteral == -1)) return -1;
                float usedvalue = (m_resourceusedliteral == -1) ? m_scwlist[m_resourceusedindex].m_value : m_resourceusedliteral;
                float capacityvalue = (m_resourcecapacityliteral == -1) ? m_scwlist[m_resourcecapacityindex].m_value : m_resourcecapacityliteral;
                //statlog.log("capacity usage " + m_category + " " + usedvalue.ToString() + " / " + capacityvalue.ToString() +
                //    " " + (usedvalue / capacityvalue).ToString());
                return (usedvalue / capacityvalue);
            }
        }

        public List<StatisticsClassWrapper> activeStats
        {
            get
            {
                List<StatisticsClassWrapper> l = new List<StatisticsClassWrapper>();
                for (int i = 0; i < m_scwlist.Count(); i++)
                {
                    if (CSLStatsPanelConfigSettings.isStatActive(m_scwlist[i].category, m_scwlist[i].m_desc))
                    {
                        l.Add(m_scwlist[i]);
                    }
                }
                return l;
            }
        }
    }

    public class StatisticsClassWrapper
    {
        public StatisticType m_st;
        public ImmaterialResourceManager.Resource m_st2;
        public string m_desc;
        public decimal m_scale, m_multiplier;
        public string m_scaledesc;
        public string statstring;
        public string category;
        public float m_value;

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

        public StatisticsClassWrapper(string category, string description, ImmaterialResourceManager.Resource st, decimal scale, decimal multiplier, string scaledescription, int precision = 2)
        {
            this.category = category;
            //m_st = st;
            m_desc = description;
            m_scale = scale;
            m_st2 = st;
            m_multiplier = multiplier;
            m_scaledesc = scaledescription;
            statstring = getstatstring(m_desc, st, m_multiplier, m_scale, m_scaledesc, precision);
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
            m_value = (float)value;
            statstring = description + ": " + Math.Round(value, precision).ToString() + suffix;

        }

        string getstatstring(string desc, ImmaterialResourceManager.Resource st,
decimal multiplier = 16, decimal scale = 1000, string scalestring = "M", int precision = 2)
        {
            //statlog.log(st);
            try
            {
                ImmaterialResourceManager im = Singleton<ImmaterialResourceManager>.instance;
                int outint = 0;
                float total = 0f;

                im.CheckTotalResource(st, out outint);
                total = outint;
                m_value = total;
                string strtotal = total.ToString();
                if (total > (float)scale)
                {
                    total /= (float)scale;
                    strtotal = Math.Round(total, precision).ToString() + scalestring;
                }

                return desc + ": " + strtotal + " ";
            }
            catch
            {
                return desc + ": -1 ";
            }
        }

        string getstatstring(string desc, StatisticType st,
            decimal multiplier = 16, decimal scale = 1000, string scalestring = "M", int precision = 2)
        {
            statlog.log(st);
            try
            {
                StatisticsManager sm = Singleton<StatisticsManager>.instance;
                StatisticBase sb = sm.Get(st);
                if (sb != null)
                {
                    float total = (float)Math.Round(sb.GetLatestFloat() * (float)multiplier, precision);
                    m_value = total;

                    string strtotal = total.ToString();
                    if (total > (float)scale)
                    {
                        total /= (float)scale;
                        strtotal = Math.Round(total, precision).ToString() + scalestring;

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

    public class buildingStats
    {
        public int onfire = 0, buildingcount = 0;
        public buildingStats()
        {
            BuildingManager bm = Singleton<BuildingManager>.instance;
            for (int i = 0; i < bm.m_buildings.m_buffer.Count(); i++)
            {
                if (!bm.m_buildings.m_buffer[i].m_flags.IsFlagSet(Building.Flags.Created)) continue;
                if (bm.m_buildings.m_buffer[i].m_flags.IsFlagSet(Building.Flags.Original)) continue;
                if (bm.m_buildings.m_buffer[i].m_flags.IsFlagSet(Building.Flags.None)) continue;
                if (bm.m_buildings.m_buffer[i].m_flags.IsFlagSet(Building.Flags.Untouchable)) continue;

                buildingcount++;
                if (bm.m_buildings.m_buffer[i].m_fireIntensity > 0) onfire++;
            }


        }


    }

    public class districtStats
    {
        public double waterbuffer = 0, sewagebuffer = 0, watercapacity = 0, sewagecapacity = 0, garbage = 0;
        public int dmcount = 0, dmusage = 0, dmproduction = 0;
        public int finalcrimerate = 0, citizencount = 0, sickcount = 0, groundpollution = 0;
        public int dmincome = 0;
        public districtStats()
        {
            DistrictManager dm = Singleton<DistrictManager>.instance;
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
                groundpollution += dm.m_districts.m_buffer[i].GetGroundPollution();
                dmcount++;
                citizencount += localcitizencount;
                dmusage += dm.m_districts.m_buffer[i].GetElectricityConsumption();
                dmproduction += dm.m_districts.m_buffer[i].GetElectricityCapacity();
                waterbuffer += dm.m_districts.m_buffer[i].GetWaterConsumption();
                sewagebuffer += dm.m_districts.m_buffer[i].GetSewageAccumulation();
                sewagecapacity += dm.m_districts.m_buffer[i].GetSewageCapacity();
                watercapacity += dm.m_districts.m_buffer[i].GetWaterCapacity();
                garbage += dm.m_districts.m_buffer[i].GetGarbageAccumulation();
                sickcount += dm.m_districts.m_buffer[i].GetSickCount();
                finalcrimerate += (int)dm.m_districts.m_buffer[i].m_finalCrimeRate;
                dmincome += (int)dm.m_districts.m_buffer[i].GetIncomeAccumulation();
                
            }

        }
    }

    public static class MasterStatsWrapper
    {
        public static List<StatisticsCategoryWrapper> getstats3()
        {
            List<StatisticsCategoryWrapper> catstopull = new List<StatisticsCategoryWrapper>();
            List<StatisticsClassWrapper> statstopull = new List<StatisticsClassWrapper>();

            districtStats ds = new districtStats();
            buildingStats bs = new buildingStats();

            string cat = "Power";
            //if (CSLStatsPanelConfigSettings.isCatActive(cat))
            {
                statstopull.Add(new StatisticsClassWrapper(cat, "Used", ds.dmusage / 1000, 2, "MW"));
                statstopull.Add(new StatisticsClassWrapper(cat, "Capacity", StatisticType.ElectricityCapacity, 1000, 16, "MW"));
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, ds.dmusage.ToString(), "Capacity", "ToolbarIconElectricity"));
                statstopull = new List<StatisticsClassWrapper>();
            }

            cat = "Water";
            //if (CSLStatsPanelConfigSettings.isCatActive(cat))
            {
                statstopull.Add(new StatisticsClassWrapper(cat, "Used", ds.waterbuffer, 2, "m³"));
                statstopull.Add(new StatisticsClassWrapper(cat, "Capacity", StatisticType.WaterCapacity, 1, 16, "m³"));
                statstopull.Add(new StatisticsClassWrapper(cat, "Pollution", StatisticType.WaterPollution, 1, 1, "%"));
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "Used", "Capacity"));
                statstopull = new List<StatisticsClassWrapper>();
            }

            cat = "Sewage";
            //if (CSLStatsPanelConfigSettings.isCatActive(cat))
            {
                statstopull.Add(new StatisticsClassWrapper(cat, "Used", ds.sewagebuffer, 2, "m³"));
                statstopull.Add(new StatisticsClassWrapper(cat, "Capacity", StatisticType.SewageCapacity, 1, 16, "m³"));
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "Used", "Capacity"));
                statstopull = new List<StatisticsClassWrapper>();
            }

            cat = "Garbage";
            //if (CSLStatsPanelConfigSettings.isCatActive(cat))
            {
                statstopull.Add(new StatisticsClassWrapper(cat, "Amount", StatisticType.GarbageAmount, 1000, 1, "K")); ;
                int garbageamount = (int)statstopull.Last().m_value;
                statstopull.Add(new StatisticsClassWrapper(cat, "Accumulation", ds.garbage));
                garbageamount += (int)statstopull.Last().m_value;
                statstopull.Add(new StatisticsClassWrapper(cat, "Total Inflow", garbageamount));
                statstopull.Add(new StatisticsClassWrapper(cat, "Capacity", StatisticType.GarbageCapacity, 1000, 1, "K")); ;
                int totalcapacity = (int)statstopull.Last().m_value;
                statstopull.Add(new StatisticsClassWrapper(cat, "Incinerate Capacity", StatisticType.IncinerationCapacity, 1000, 16, "M")); ;
                totalcapacity += (int)statstopull.Last().m_value;
                statstopull.Add(new StatisticsClassWrapper(cat, "Total Capacity", totalcapacity));
                statstopull.Add(new StatisticsClassWrapper(cat, "Piles", StatisticType.GarbagePiles, 1000, 1, "M"));
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "Total Inflow", "Total Capacity"));
                statstopull = new List<StatisticsClassWrapper>();
            }

            cat = "Health Services";
            //if (CSLStatsPanelConfigSettings.isCatActive(cat))
            {
                statstopull.Add(new StatisticsClassWrapper(cat, "Health", ImmaterialResourceManager.Resource.Health, 1, 1, "%"));
                StatisticsClassWrapper tempscw = statstopull[statstopull.Count() - 1];
                int health = (int)tempscw.m_value;
                statstopull.Add(new StatisticsClassWrapper(cat, "Well Being", ImmaterialResourceManager.Resource.Wellbeing, 1, 1, "%"));
                statstopull.Add(new StatisticsClassWrapper(cat, "Sick", ds.sickcount));
                statstopull.Add(new StatisticsClassWrapper(cat, "Capacity", StatisticType.HealCapacity, 1, 1, ""));
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, (100 - health).ToString(), "30"));
                statstopull = new List<StatisticsClassWrapper>();
            }
            
            cat = "Death Services";
            //if (CSLStatsPanelConfigSettings.isCatActive(cat))
            {

                statstopull.Add(new StatisticsClassWrapper(cat, "Amount", StatisticType.DeadAmount, 1, 1, ""));
                statstopull.Add(new StatisticsClassWrapper(cat, "Capacity", StatisticType.DeadCapacity, 1, 1, ""));
                statstopull.Add(new StatisticsClassWrapper(cat, "Cremate Capacity", StatisticType.CremateCapacity, 1000, 1, "K"));
                if (statstopull.Last().m_value > 0)
                    catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "Amount", "Cremate Capacity"));
                else catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "Amount", "Capacity"));
                statstopull = new List<StatisticsClassWrapper>();
            }

            cat = "Buildings";
            //if (CSLStatsPanelConfigSettings.isCatActive(cat))
            {
                statstopull.Add(new StatisticsClassWrapper(cat, "Count", bs.buildingcount, 2, ""));
                statstopull.Add(new StatisticsClassWrapper(cat, "Abandoned", StatisticType.AbandonedBuildings, 1, 1, ""));
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "Abandoned", (bs.buildingcount * .25).ToString()));
                statstopull = new List<StatisticsClassWrapper>();
            }

            cat = "Fire";
            //if (CSLStatsPanelConfigSettings.isCatActive(cat))
            {
                statstopull.Add(new StatisticsClassWrapper(cat, "Buildings Burning", bs.onfire, 2, ""));
                statstopull.Add(new StatisticsClassWrapper(cat, "Hazard", ImmaterialResourceManager.Resource.FireHazard, 1, 1, "%"));
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "Hazard", "50"));
                statstopull = new List<StatisticsClassWrapper>();
            }
            //statstopull.Add(new StatisticsClassWrapper("Health Services", "Heath Care", ImmaterialResourceManager.Resource.HealthCare, 1, 1, "%"));
            //statstopull.Add(new StatisticsClassWrapper("Health Services", "Health", ImmaterialResourceManager.Resource.Health, 1, 1, "%"));

            cat = "Economy";
            //if (CSLStatsPanelConfigSettings.isCatActive(cat))
            {
                statstopull.Add(new StatisticsClassWrapper(cat, "Entertainment", ImmaterialResourceManager.Resource.Entertainment, 1, 1, ""));
                statstopull.Add(new StatisticsClassWrapper(cat, "Attractiveness", ImmaterialResourceManager.Resource.Attractiveness, 1, 1, ""));
                statstopull.Add(new StatisticsClassWrapper(cat, "Cargo Transport", ImmaterialResourceManager.Resource.CargoTransport, 1, 1, ""));
                //statstopull.Add(new StatisticsClassWrapper("Misc", "Coverage?", ImmaterialResourceManager.Resource.Coverage, 1, 1, ""));
                statstopull.Add(new StatisticsClassWrapper(cat, "Density", ImmaterialResourceManager.Resource.Density, 1, 1, ""));
                statstopull.Add(new StatisticsClassWrapper(cat, "Land Value", ImmaterialResourceManager.Resource.LandValue, 1, 1, "₡/m²"));
                statstopull.Add(new StatisticsClassWrapper(cat, "ImmaterialResource", StatisticType.ImmaterialResource, 1000, 1, "M"));
                statstopull.Add(new StatisticsClassWrapper(cat, "Goods Produced", StatisticType.GoodsProduced, 1000, 1, "K"));
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "", ""));
                statstopull = new List<StatisticsClassWrapper>();
            }

            cat = "Citizens";
            //if (CSLStatsPanelConfigSettings.isCatActive(cat))
            {

                statstopull.Add(new StatisticsClassWrapper(cat, "Count", ds.citizencount, 2, ""));
                statstopull.Add(new StatisticsClassWrapper(cat, "Move Rate", StatisticType.MoveRate, 1, 1, ""));
                statstopull.Add(new StatisticsClassWrapper(cat, "Birth Rate", StatisticType.BirthRate, 1, 1, ""));
                statstopull.Add(new StatisticsClassWrapper(cat, "Death Rate", StatisticType.DeathRate, 1, 1, ""));
                statstopull.Add(new StatisticsClassWrapper(cat, "Eligible Workers", StatisticType.EligibleWorkers, 1, 1, ""));
                statstopull.Add(new StatisticsClassWrapper(cat, "Unemployed", StatisticType.Unemployed, 1, 1, ""));
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "Death Rate", "Birth Rate"));
                statstopull = new List<StatisticsClassWrapper>();
            }

            cat = "Education";
            //if (CSLStatsPanelConfigSettings.isCatActive(cat))
            {

                statstopull.Add(new StatisticsClassWrapper(cat, "Educated", StatisticType.EducatedCount, 1, 1, ""));
                statstopull.Add(new StatisticsClassWrapper(cat, "Students", StatisticType.StudentCount, 1, 1, ""));
                statstopull.Add(new StatisticsClassWrapper(cat, "Max Students", StatisticType.EducationCapacity, 1, 1, ""));
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "Students", "Max Students"));
                statstopull = new List<StatisticsClassWrapper>();
            }

            cat = "Crime";
            //if (CSLStatsPanelConfigSettings.isCatActive(cat))
            {

                statstopull.Add(new StatisticsClassWrapper(cat, "Crimes", StatisticType.CrimeRate, 1, 1, ""));
                statstopull.Add(new StatisticsClassWrapper(cat, "Crime Rate", ds.finalcrimerate, 2, "%"));
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "Crime Rate", "20"));
                statstopull = new List<StatisticsClassWrapper>();
            }

            cat = "Tourists";
            //if (CSLStatsPanelConfigSettings.isCatActive(cat))
            {

                statstopull.Add(new StatisticsClassWrapper(cat, "Visits", StatisticType.TouristVisits, 1, 1, ""));
                statstopull.Add(new StatisticsClassWrapper(cat, "Incoming", StatisticType.IncomingTourists, 1, 1, ""));
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "", ""));
                statstopull = new List<StatisticsClassWrapper>();
            }

            cat = "Public Transit";
            //if (CSLStatsPanelConfigSettings.isCatActive(cat))
            {

                statstopull.Add(new StatisticsClassWrapper(cat, "Avg Passengers", StatisticType.AveragePassengers, 1, 1, ""));
                statstopull.Add(new StatisticsClassWrapper(cat, "Availability", ImmaterialResourceManager.Resource.PublicTransport, 1, 1, "%"));
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "80", "Availability"));
                statstopull = new List<StatisticsClassWrapper>();
            }

            cat = "Pollution";
            //if (CSLStatsPanelConfigSettings.isCatActive(cat))
            {
                statstopull.Add(new StatisticsClassWrapper("Pollution", "Noise", ImmaterialResourceManager.Resource.NoisePollution, 1, 1, "%"));
                statstopull.Add(new StatisticsClassWrapper("Pollution", "Ground", ds.groundpollution, 2, "%"));
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "", ""));
                statstopull = new List<StatisticsClassWrapper>();
            }

            return catstopull;
        }
    }
}
