using System.Globalization;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Management;

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
        public bool m_showstatsummary = false;
        public float m_targetred = .95f, m_targetyellow = .75f;

        public StatisticsCategoryWrapper(string category, List<StatisticsClassWrapper> scwlist,
            string resourceusedfield, string resourcecapacityfield, string sprite = "GenericPanel", bool showstatsummary = false)
        {
            m_showstatsummary = showstatsummary;
            m_scwlist = scwlist;
            m_category = category;
            m_resourcecapacityfield = resourcecapacityfield;
            m_resourceusedfield = resourceusedfield;
            m_sprite = sprite;

            for (int i = 0; i < m_scwlist.Count(); i++)
            {
                if (m_scwlist[i].m_desc.ToUpper() == m_resourceusedfield.ToUpper() && !string.IsNullOrEmpty(m_resourceusedfield)) m_resourceusedindex = i;
                if (m_scwlist[i].m_desc.ToUpper() == m_resourcecapacityfield.ToUpper() && !string.IsNullOrEmpty(m_resourcecapacityfield)) m_resourcecapacityindex = i;
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
                //float usedvalue = (m_resourceusedliteral == -1) ? m_scwlist[m_resourceusedindex].m_value : m_resourceusedliteral;
                //float capacityvalue = (m_resourcecapacityliteral == -1) ? m_scwlist[m_resourcecapacityindex].m_value : m_resourcecapacityliteral;
                float usedvalue = (m_resourceusedliteral == -1) ? m_scwlist[m_resourceusedindex].m_finalvalue : m_resourceusedliteral;
                float capacityvalue = (m_resourcecapacityliteral == -1) ? m_scwlist[m_resourcecapacityindex].m_finalvalue : m_resourcecapacityliteral;
                if (usedvalue > 0 && capacityvalue <= 0) return 1;
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
        public string m_scaledesc = "";
        public string statstring;
        public string category;
        public float m_value, m_finalvalue;

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
        public StatisticsClassWrapper(string category, string description, double value, int precision = 2, string suffix = "", decimal scale = 1, bool alwaysaddsuffix = true)
        {
            bool showsuffix = alwaysaddsuffix;
            this.category = category;
            m_desc = description;
            m_value = (float)value;
            if (value > (float)scale)
            {
                value /= (float)scale;
                m_scaledesc = suffix;
                showsuffix = true;
            }
            m_finalvalue = (float)value;
            statstring = description + ": " + Math.Round(value, precision).ToString() + ((showsuffix) ? suffix : "");

        }

        string getstatstring(string desc, ImmaterialResourceManager.Resource st,
decimal multiplier = 16, decimal scale = 1000, string scalestring = "M", int precision = 2)
        {
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

                m_finalvalue = (float)total;
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
                    m_finalvalue = (float)total;

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
        public int onfire = 0, buildingcount = 0,
            garbagetrucks = 0, firetrucks = 0, hearse = 0, policecars = 0, healthcarevehicles = 0,
            workplacecount0, workplacecount1, workplacecount2, workplacecount3,
            maintenancetrucks = 0, snowtrucks=0;
        public buildingStats()
        {
            BuildingManager bm = Singleton<BuildingManager>.instance;
            
            //buildingcount = mybuilding.Count();
            //onfire = mybuilding.Where(p =>p.m_fireIntensity > 0).Count();


            for (int i = 0; i < bm.m_buildings.m_buffer.Count(); i++)
            {
                if (!bm.m_buildings.m_buffer[i].m_flags.IsFlagSet(Building.Flags.Created)) continue;
                if (bm.m_buildings.m_buffer[i].m_flags.IsFlagSet(Building.Flags.Deleted)) continue;
                //if (bm.m_buildings.m_buffer[i].m_flags.IsFlagSet(Building.Flags.Untouchable)) continue;

                buildingcount++;

                bool isEmptying = false, isFull = false;
                //if (bm.m_buildings.m_buffer[i].m_flags.IsFlagSet(Building.Flags.Downgrading)) isEmptying = true;
                if ((bm.m_buildings.m_buffer[i].m_flags & Building.Flags.Downgrading) != Building.Flags.None) isEmptying = true;
                if (bm.m_buildings.m_buffer[i].Info.m_buildingAI.IsFull((ushort)i, ref bm.m_buildings.m_buffer[i])) isFull = true;
                

                BuildingAI bi = bm.m_buildings.m_buffer[i].Info.m_buildingAI;
                int budget = Singleton<EconomyManager>.instance.GetBudget(bi.m_info.m_class);
                int productionRate = PlayerBuildingAI.GetProductionRate(100, budget);
                Type t = bi.GetType();

                
                if (t == typeof(LandfillSiteAI))                    // incinerators seem to be landfillai's as well
                {
                    
                    if (!isEmptying && !isFull) garbagetrucks += (productionRate * ((LandfillSiteAI)bi).m_garbageTruckCount + 99) / 100;
                }
                else if (t == typeof(PoliceStationAI))
                    policecars += (productionRate * ((PoliceStationAI)bi).m_policeCarCount + 99) / 100;
                else if (t == typeof(FireStationAI))
                    firetrucks += (productionRate * ((FireStationAI)bi).m_fireTruckCount + 99) / 100;
                else if (t == typeof(HospitalAI))
                    healthcarevehicles += (productionRate * ((HospitalAI)bi).m_ambulanceCount + 99) / 100;
                else if (t == typeof(CemeteryAI))
                    if (!isEmptying && !isFull) hearse += (productionRate * ((CemeteryAI)bi).m_hearseCount + 99) / 100;
                else if (t == typeof(MaintenanceDepotAI))
                        maintenancetrucks += (productionRate * ((MaintenanceDepotAI)bi).m_maintenanceTruckCount + 99) / 100;
                else if (t==typeof(SnowDumpAI))
                        snowtrucks += (productionRate * ((SnowDumpAI)bi).m_snowTruckCount + 99) / 100;
                if (bm.m_buildings.m_buffer[i].m_fireIntensity > 0) onfire++;
            }



        }


    }

    public class districtStats
    {
        public double waterbuffer = 0, sewagebuffer = 0, watercapacity = 0, sewagecapacity = 0, garbage = 0;
        public int dmcount = 0, dmusage = 0, dmproduction = 0, deadamount=0;
        public int finalcrimerate = 0, citizencount = 0, sickcount = 0, groundpollution = 0;
        public int dmincome = 0, education1rate = 0, education2rate = 0, education3rate = 0, 
            educated0=0,educated1=0,educated2=0,educated3=0;
        public List<DistrictEducationData> educationData = new List<DistrictEducationData>();

        public districtStats()
        {
            DistrictManager dm = Singleton<DistrictManager>.instance;
            int i = 0;
            //for (int i = 0; i < 1; i++)
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
                //deadamount += (int)dm.m_districts.m_buffer[i].GetDeadAmount();
                deadamount += (int)dm.m_districts.m_buffer[i].GetDeadCount();
                education1rate = (int)dm.m_districts.m_buffer[i].GetEducation1Rate();
                education2rate = (int)dm.m_districts.m_buffer[i].GetEducation2Rate();
                education3rate = (int)dm.m_districts.m_buffer[i].GetEducation3Rate();
                educated0 = (int)dm.m_districts.m_buffer[i].m_educated0Data.m_finalCount;
                educated1 = (int)dm.m_districts.m_buffer[i].m_educated1Data.m_finalCount;
                educated2 = (int)dm.m_districts.m_buffer[i].m_educated2Data.m_finalCount;
                educated3 = (int)dm.m_districts.m_buffer[i].m_educated3Data.m_finalCount;
                educationData.Add(dm.m_districts.m_buffer[i].m_educated0Data);
                educationData.Add(dm.m_districts.m_buffer[i].m_educated1Data);
                educationData.Add(dm.m_districts.m_buffer[i].m_educated2Data);
                educationData.Add(dm.m_districts.m_buffer[i].m_educated3Data);
                
            }

        }
    }

    public class vehiclestats
    {
        public double activevehicles = 0,
            garbagetrucksinuse = 0, firetrucksinuse = 0, hearseinuse = 0, policecarsinuse = 0, healthcarevehiclesinuse = 0,
            maintenancetrucksinuse = 0, snowtrucksinuse=0,
            passengerbusses = 0, passengerships = 0, passengertrains = 0, passengerplanes = 0, passengermetro = 0, passengertram = 0,
            cargocars = 0, cargobusses = 0, cargoships = 0, cargotrains = 0, cargoplanes = 0, cargometro = 0, cargotrams = 0,
            commercialcars = 0, commercialbusses = 0, commercialships = 0, commercialtrains = 0, commercialplanes = 0, commercialmetro = 0, commercialtrams = 0,
            officecars = 0, officebusses = 0, officeships = 0, officetrains = 0, officeplanes = 0, officemetro = 0, officetrams = 0,
            waitcounter=0,blockcounter=0, highestwait=0, highestblock=0, highestdelay=0, delay=0, bodiesintransit=0,
            cargoexports =0, cargoimports=0,
            carexports = 0, carimports = 0, trainexports = 0, trainimports = 0, tramimports=0,
            metroexports = 0, metroimports = 0, shipexports = 0, shipimports = 0, tramexports=0,
            planeexports = 0, planeimports = 0, 
            industryimports=0, industryexports=0, commercialimports=0, commercialexports=0, officeimports=0, officeexports=0,
            intracitytransports=0;


        public bool checkbuilding(Building b, ushort buildingIndex)
        {
            bool isEmptying = false;
            if ((b.m_flags & Building.Flags.Downgrading) != Building.Flags.None) isEmptying = true;

            return b.m_flags.IsFlagSet(Building.Flags.Created)
                && !b.m_flags.IsFlagSet(Building.Flags.Deleted)
                && !b.m_flags.IsFlagSet(Building.Flags.Untouchable)
                && !b.Info.m_buildingAI.IsFull(buildingIndex, ref b) // is full
                && !isEmptying; // is emptying


        }

        public vehiclestats()
        {
            VehicleManager vm = Singleton<VehicleManager>.instance;
            BuildingManager bm = Singleton<BuildingManager>.instance;


            for (int i = 0; i < vm.m_vehicles.m_buffer.Count(); i++)
            {
                Vehicle myv = vm.m_vehicles.m_buffer[i];
                if (!myv.m_flags.IsFlagSet(Vehicle.Flags.Created)) continue;
                if (myv.m_flags.IsFlagSet(Vehicle.Flags.Deleted)) continue;
                
                if (myv.Info.m_vehicleType == VehicleInfo.VehicleType.Car)
                {
                    activevehicles++;
                    waitcounter += myv.m_waitCounter;
                    if (myv.m_waitCounter > highestwait) highestwait = myv.m_waitCounter;
                    blockcounter += myv.m_blockCounter;
                    if (myv.m_blockCounter > highestblock) highestblock = myv.m_blockCounter;
                    delay += myv.m_waitCounter + myv.m_blockCounter;
                    if (myv.m_waitCounter + myv.m_blockCounter > highestdelay) highestdelay = myv.m_waitCounter + myv.m_blockCounter;
                }
                Building b = bm.m_buildings.m_buffer[myv.m_sourceBuilding];
                bool buildingisvalid = checkbuilding(b, myv.m_sourceBuilding);
                bool sourcebuildingisplayer = (bm.m_buildings.m_buffer[myv.m_sourceBuilding].m_flags.IsFlagSet(Building.Flags.Untouchable)) ? false : true;
                bool targetbuildingisplayer = (bm.m_buildings.m_buffer[myv.m_targetBuilding].m_flags.IsFlagSet(Building.Flags.Untouchable)) ? false : true;
                uint transfersize = myv.m_transferSize;
                TransferManager.TransferReason transfertype = (TransferManager.TransferReason)myv.m_transferType;
                
                switch (myv.Info.m_class.m_service)
                {
                    case ItemClass.Service.Government:
                    case ItemClass.Service.Road:
                        if (myv.Info.m_vehicleAI.GetType() == typeof(MaintenanceTruckAI))
                            maintenancetrucksinuse++;
                        if (myv.Info.m_vehicleAI.GetType() == typeof(SnowTruckAI))
                            snowtrucksinuse++;
                        break;
                    case ItemClass.Service.Garbage:
                        if (buildingisvalid) garbagetrucksinuse++;
                        break;
                    case ItemClass.Service.FireDepartment:
                        firetrucksinuse++;
                        break;
                    case ItemClass.Service.PoliceDepartment:
                        policecarsinuse++;
                        break;
                    case ItemClass.Service.HealthCare:
                        if (myv.Info.m_vehicleAI.GetType() == typeof(AmbulanceAI))
                            healthcarevehiclesinuse++;
                        else if (myv.Info.m_vehicleAI.GetType() == typeof(HearseAI))
                            if (buildingisvalid)
                            {
                                hearseinuse++;
                                bodiesintransit += transfersize;
                            }
                        
                        break;
                    case ItemClass.Service.PublicTransport:
                        if (sourcebuildingisplayer && !targetbuildingisplayer) cargoexports += transfersize;
                        if (targetbuildingisplayer && !sourcebuildingisplayer) cargoimports += transfersize;
                        if (targetbuildingisplayer && sourcebuildingisplayer) intracitytransports += transfersize;

                        switch (myv.Info.m_class.m_subService)
                        {
                            case ItemClass.SubService.PublicTransportPlane:
                                if (buildingisvalid) 
                                    passengerplanes++;
                                if (sourcebuildingisplayer && !targetbuildingisplayer) planeexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) planeimports += transfersize;
                                break;
                            case ItemClass.SubService.PublicTransportTrain:
                                if (buildingisvalid)
                                    passengertrains++;
                                if (sourcebuildingisplayer && !targetbuildingisplayer) trainexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) trainimports += transfersize;
                                break;
                            case ItemClass.SubService.PublicTransportTram:
                                if (buildingisvalid)
                                    passengertram++;
                                if (sourcebuildingisplayer && !targetbuildingisplayer) tramexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) tramimports += transfersize;
                                break;
                            case ItemClass.SubService.PublicTransportShip:
                                if (buildingisvalid)
                                    passengerships++;
                                if (sourcebuildingisplayer && !targetbuildingisplayer) shipexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) shipimports += transfersize;
                                break;
                            case ItemClass.SubService.PublicTransportBus:
                                passengerbusses++;
                                break;
                            case ItemClass.SubService.PublicTransportMetro:
                                passengermetro++;
                                if (sourcebuildingisplayer && !targetbuildingisplayer) metroexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) metroimports += transfersize;
                                break;
                        }
                        break;
                     case ItemClass.Service.Commercial:
                        if (sourcebuildingisplayer && !targetbuildingisplayer) cargoexports += transfersize;
                        if (targetbuildingisplayer && !sourcebuildingisplayer) cargoimports += transfersize;
                        if (sourcebuildingisplayer && !targetbuildingisplayer) commercialexports += transfersize;
                        if (targetbuildingisplayer && !sourcebuildingisplayer) commercialimports += transfersize;
                        if (targetbuildingisplayer && sourcebuildingisplayer) intracitytransports += transfersize;

                        switch (myv.Info.m_vehicleType)
                        {
                            case VehicleInfo.VehicleType.Car:
                                commercialcars++;
                                if (sourcebuildingisplayer && !targetbuildingisplayer) carexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) carimports += transfersize;

                                break;
                            case VehicleInfo.VehicleType.Metro:
                                commercialmetro++;
                                if (sourcebuildingisplayer && !targetbuildingisplayer) metroexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) metroimports += transfersize;
                                break;
                            case VehicleInfo.VehicleType.Plane:
                                if (sourcebuildingisplayer && !targetbuildingisplayer) planeexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) planeimports += transfersize;
                                commercialplanes++;
                                break;
                            case VehicleInfo.VehicleType.Ship:
                                if (sourcebuildingisplayer && !targetbuildingisplayer) shipexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) shipimports += transfersize;
                                commercialships++;
                                break;
                            case VehicleInfo.VehicleType.Train:
                                
                                commercialtrains++;
                                if (sourcebuildingisplayer && !targetbuildingisplayer) trainexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) trainimports += transfersize;
                                break;
                            case VehicleInfo.VehicleType.Tram:
                                commercialtrams++;
                                if (sourcebuildingisplayer && !targetbuildingisplayer) tramexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) tramimports += transfersize;
                                break;
                        }
                        break;
                    case ItemClass.Service.Office:
                        if (sourcebuildingisplayer && !targetbuildingisplayer) cargoexports += transfersize;
                        if (targetbuildingisplayer && !sourcebuildingisplayer) cargoimports += transfersize;
                        if (sourcebuildingisplayer && !targetbuildingisplayer) officeexports += transfersize;
                        if (targetbuildingisplayer && !sourcebuildingisplayer) officeimports += transfersize;
                        if (targetbuildingisplayer && sourcebuildingisplayer) intracitytransports += transfersize;
                        switch (myv.Info.m_vehicleType)
                        {
                            case VehicleInfo.VehicleType.Car:
                                if (sourcebuildingisplayer && !targetbuildingisplayer) carexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) carimports += transfersize;
                                officecars++;
                                break;
                            case VehicleInfo.VehicleType.Metro:
                                officemetro++;
                                if (sourcebuildingisplayer && !targetbuildingisplayer) metroexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) metroimports += transfersize;
                                break;
                            case VehicleInfo.VehicleType.Plane:
                                officeplanes++;
                                if (sourcebuildingisplayer && !targetbuildingisplayer) planeexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) planeimports += transfersize;
                                break;
                            case VehicleInfo.VehicleType.Ship:
                                officeships++;
                                if (sourcebuildingisplayer && !targetbuildingisplayer) shipexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) shipimports += transfersize;
                                break;
                            case VehicleInfo.VehicleType.Train:
                                officetrains++;
                                if (sourcebuildingisplayer && !targetbuildingisplayer) trainexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) trainimports += transfersize;
                                break;
                            case VehicleInfo.VehicleType.Tram:
                                officetrams++;
                                if (sourcebuildingisplayer && !targetbuildingisplayer) tramexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) tramimports += transfersize;
                                break;
                        }
                        break;
                    case ItemClass.Service.Industrial:
                        if (sourcebuildingisplayer && !targetbuildingisplayer) cargoexports += transfersize;
                        if (targetbuildingisplayer && !sourcebuildingisplayer) cargoimports += transfersize;
                        if (sourcebuildingisplayer && !targetbuildingisplayer) industryexports += transfersize;
                        if (targetbuildingisplayer && !sourcebuildingisplayer) industryimports += transfersize;
                        if (targetbuildingisplayer && sourcebuildingisplayer) intracitytransports += transfersize;
                        switch (myv.Info.m_vehicleType)
                        {
                            case VehicleInfo.VehicleType.Car:
                                cargocars++;
                                if (sourcebuildingisplayer && !targetbuildingisplayer) carexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) carimports += transfersize;

                                break;
                            case VehicleInfo.VehicleType.Metro:
                                cargometro++;
                                if (sourcebuildingisplayer && !targetbuildingisplayer) metroexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) metroimports += transfersize;
                                break;
                            case VehicleInfo.VehicleType.Plane:
                                cargoplanes++;
                                if (sourcebuildingisplayer && !targetbuildingisplayer) planeexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) planeimports += transfersize;

                                break;
                            case VehicleInfo.VehicleType.Ship:
                                cargoships++;
                                if (sourcebuildingisplayer && !targetbuildingisplayer) shipexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) shipimports += transfersize;
                                break;
                            case VehicleInfo.VehicleType.Train:
                                if (sourcebuildingisplayer && !targetbuildingisplayer) trainexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) trainimports += transfersize;
                                cargotrains++;
                                break;
                            case VehicleInfo.VehicleType.Tram:
                                if (sourcebuildingisplayer && !targetbuildingisplayer) tramexports += transfersize;
                                if (targetbuildingisplayer && !sourcebuildingisplayer) tramimports += transfersize;
                                cargotrams++;
                                break;
                        }
                        break;
                }
                
            }


        }
    }


    public static class MasterStatsWrapper
    {
        public class AveragedStat : List<float>
        {
            public int m_numbertohold = 5;
            public AveragedStat(int numbertohold = 5)
            {
                m_numbertohold = numbertohold;
            }

            public float AddStat(float stattoadd)
            {
                while (this.Count > m_numbertohold) this.RemoveAt(0);
                this.Add(stattoadd);
                return this.Average();
            }

            public float Value
            {
                get { return this.Average(); }
            }
        }

        public static AveragedStat TrafficWaitAverage = new AveragedStat(10);

        public static float GetStat(StatisticType st)
        {
            try
            {
                StatisticsManager sm = Singleton<StatisticsManager>.instance;
                StatisticBase sb = sm.Get(st);
                if (sb != null)
                {
                    return sb.GetLatestFloat();
                }
            }catch{}
            return 0f;
        }
        public static float GetStat(ImmaterialResourceManager.Resource st)
        {
            try
            {
                ImmaterialResourceManager im = Singleton<ImmaterialResourceManager>.instance;
                int outint = 0;
                im.CheckTotalResource(st, out outint);
                return outint;
            }
            catch { }
            return 0f;
        }

        public static void StatAdd(ref List<StatisticsClassWrapper> scwlist, StatisticsClassWrapper scwtoadd, bool onlyenabled)
        {
            if (CSLStatsPanelConfigSettings.isStatActive(scwtoadd.category, scwtoadd.m_desc) || !onlyenabled)
            {
                scwlist.Add(scwtoadd);
            }
        }


        public static string customStatReplacer(districtStats ds, vehiclestats vs, buildingStats bs, string stringtoanalyze)
        {

            if (string.IsNullOrEmpty(stringtoanalyze)) return stringtoanalyze;
            stringtoanalyze = stringtoanalyze.ToLower().Trim();

            if (stringtoanalyze.EndsWith(".totalgarbageamount"))
            {
                decimal customGarbageAmount = (decimal)MasterStatsWrapper.GetStat(StatisticType.GarbageAmount) + (decimal)ds.garbage;
                return customGarbageAmount.ToString();
            }
            else if (stringtoanalyze.EndsWith(".totalgarbagecapacity"))
            {
                decimal customGarbageCapacity = (decimal)MasterStatsWrapper.GetStat(StatisticType.GarbageCapacity) + (decimal)MasterStatsWrapper.GetStat(StatisticType.IncinerationCapacity);
                return customGarbageCapacity.ToString();

            }
            else if (stringtoanalyze.EndsWith(".invertedhealth"))
            {
                decimal customInvertedHealth = 100 - (decimal)MasterStatsWrapper.GetStat(ImmaterialResourceManager.Resource.Health);
                return customInvertedHealth.ToString();
            }
            else if (stringtoanalyze.EndsWith(".totaldeadamount"))
            {
                return (MasterStatsWrapper.GetStat(StatisticType.DeadAmount) + ds.deadamount).ToString();
            }
            else if (stringtoanalyze.EndsWith(".totaldeadcapacity"))
            {
                return (MasterStatsWrapper.GetStat(StatisticType.DeadCapacity) + MasterStatsWrapper.GetStat(StatisticType.CremateCapacity)).ToString();
            }
            else if (stringtoanalyze.EndsWith(".abandonedbuildingtarget"))
            {
                return (bs.buildingcount * .25).ToString();
            }

            else if (stringtoanalyze.EndsWith(".budget"))
            {
                return (servicesincome - servicesexpenses).ToString();
            }
            else if (stringtoanalyze.EndsWith(".income"))
            {
                return (servicesincome).ToString();
            }
            else if (stringtoanalyze.EndsWith(".expenses"))
            {
                return (servicesexpenses).ToString();
            }

            else if (stringtoanalyze.EndsWith(".homeless"))
            {
                return (ds.educationData[0].m_finalHomeless
                    + ds.educationData[1].m_finalHomeless
                    + ds.educationData[2].m_finalHomeless
                    + ds.educationData[3].m_finalHomeless).ToString();
            }
            else if (stringtoanalyze.EndsWith(".eligibleworkers"))
            {
                return (ds.educationData[0].m_finalEligibleWorkers
                    + ds.educationData[1].m_finalEligibleWorkers
                    + ds.educationData[2].m_finalEligibleWorkers
                    + ds.educationData[3].m_finalEligibleWorkers).ToString();
            }
            else if (stringtoanalyze.EndsWith(".educatedcount"))
            {
                return (ds.educated1 + ds.educated2 + ds.educated3).ToString();
            }
            else if (stringtoanalyze.Substring(0, stringtoanalyze.Length - 1).EndsWith(".educatedlevel"))
            {

                string[] educatedlevels = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    educatedlevels[i] =
                         (ds.educationData[i].m_finalEligibleWorkers == 0) ? string.Format("L{0}: 0.00% - 0", i) :
                         string.Format("L{2}: {0:0.00%} - {1}",
                         ds.educationData[i].m_finalCount / (double)ds.citizencount,
                         ds.educationData[i].m_finalCount,
                         i);
                }


                if (stringtoanalyze.EndsWith(".educatedlevel0"))
                {
                    return educatedlevels[0];
                }
                else if (stringtoanalyze.EndsWith(".educatedlevel1"))
                {
                    return educatedlevels[1];
                }
                else if (stringtoanalyze.EndsWith(".educatedlevel2"))
                {
                    return educatedlevels[2];
                }
                else if (stringtoanalyze.EndsWith(".educatedlevel3"))
                {
                    return educatedlevels[3];
                }
            }
            else if (stringtoanalyze.Substring(0, stringtoanalyze.Length - 1).EndsWith(".unemployedlevel"))
            {
                string[] unemployedlevels = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    unemployedlevels[i] =
                        (ds.educationData[i].m_finalEligibleWorkers == 0) ? string.Format("L{0}: 0.00% - 0", i) : string.Format("L{2}: {1:0.00%} - {0}",
                        ds.educationData[i].m_finalUnemployed,
                        ds.educationData[i].m_finalUnemployed / (double)ds.educationData[i].m_finalEligibleWorkers,
                        i);
                }
                if (stringtoanalyze.EndsWith(".unemployedlevel0"))
                {
                    return unemployedlevels[0];
                }
                else if (stringtoanalyze.EndsWith(".unemployedlevel1"))
                {
                    return unemployedlevels[1];
                }
                else if (stringtoanalyze.EndsWith(".unemployedlevel2"))
                {
                    return unemployedlevels[2];
                }
                else if (stringtoanalyze.EndsWith(".unemployedlevel3"))
                {
                    return unemployedlevels[3];
                }
            }
            else if (stringtoanalyze.EndsWith(".trafficavgdelay"))
            {
                return (vs.delay / vs.activevehicles).ToString();
            }
            else if (stringtoanalyze.EndsWith(".averagedstattrafficdelay"))
            {
                double avgstat = MasterStatsWrapper.TrafficWaitAverage.AddStat((float)(vs.delay / vs.activevehicles));
                return avgstat.ToString();
            }
            else if (stringtoanalyze.EndsWith(".fps"))
            {
                return ThreadingCSLStatsMod.framespersecond.ToString();
            }
            return stringtoanalyze;
        }

        
        public static double myincome = 0, myexpenses = 0;
        public static int servicesexpenses=0, servicesincome=0;
        public static void refreshservicesandexpenses()
        {

            if (!pollsinitialized) InitializePolls();

                myexpenses = 0;myincome = 0;
                servicesexpenses = 0; servicesincome = 0;

                basicIncomePolls[0].Poll(Settings.moneyFormat, LocaleManager.cultureInfo);
                myincome += basicIncomePolls[0].income;
                
                for (int j = 0; j < budgetExpensesPolls.Length; j++)
                {
                    budgetExpensesPolls[j].Poll(Settings.moneyFormat, LocaleManager.cultureInfo);
                    myexpenses += budgetExpensesPolls[j].expenses;
                }
                myexpenses += expensesPoliciesTotal;
                servicesexpenses = (int)myexpenses / 100; // tempscw.m_value / 100;
                servicesincome = (int)myincome / 100; // tempscw.m_value / 100;

        }

        public static List<StatisticsCategoryWrapper> getstats3(bool onlyenabled, string xmlconfig)
        {
            onlyenabled = false;
            List<StatisticsCategoryWrapper> catstopull = new List<StatisticsCategoryWrapper>();
            List<StatisticsClassWrapper> statstopull = new List<StatisticsClassWrapper>();
            districtStats ds = new districtStats();
            buildingStats bs = new buildingStats();
            vehiclestats vs = new vehiclestats();
            refreshservicesandexpenses();

            System.Xml.XmlDocument xmld = new System.Xml.XmlDocument();
            xmld.LoadXml(xmlconfig);
            
            System.Xml.XmlNode root = xmld.ChildNodes[0];
            for (int i = 0; i < root.ChildNodes.Count; i++)
            {
                System.Xml.XmlNode catnode = root.ChildNodes[i];
                if (catnode.Name.ToUpper().Trim() == "CATEGORY")
                {
                    string cat = XMLHelper.safeAttributes(catnode, "name");
                    bool active = bool.Parse(XMLHelper.safeAttributes(catnode, "active"));
                    string icon = XMLHelper.safeAttributes(catnode, "icon");
                    bool showstatssummary = bool.Parse(XMLHelper.safeAttributes(catnode, "showstatsummary"));
                    string summaryusedstat = customStatReplacer(ds, vs, bs, XMLHelper.safeAttributes(catnode, "summaryusedstat"));
                    string summarycapacitystat = customStatReplacer(ds, vs, bs, XMLHelper.safeAttributes(catnode, "summarycapacitystat"));
                    string summarykeystat = "";
                    string vehiclesummaryusedstat = "", vehiclesummarycapacitystat = "";
                    if (XMLHelper.safeAttributes(catnode, "vehiclesummaryusedstat") != "" && XMLHelper.safeAttributes(catnode, "vehiclesummarycapacitystat") != "")
                    {
                        vehiclesummaryusedstat = customStatReplacer(ds, vs, bs, XMLHelper.safeAttributes(catnode, "vehiclesummaryusedstat"));
                        vehiclesummarycapacitystat = customStatReplacer(ds, vs, bs, XMLHelper.safeAttributes(catnode, "vehiclesummarycapacitystat"));
                    }

                    if ((CSLStatsPanelConfigSettings.isCatActive(cat) && active) || !onlyenabled)
                    {
                        for (int c = 0; c < catnode.ChildNodes.Count; c++ )
                        {
                            System.Xml.XmlNode statnode = catnode.ChildNodes[c];
                            string statname = XMLHelper.safeAttributes(statnode, "name");
                            string value = XMLHelper.safeAttributes(statnode, "value");
                            decimal divisor = decimal.Parse(XMLHelper.safeAttributes(statnode, "divisor"));
                            string divisorsuffix = XMLHelper.safeAttributes(statnode, "divisorsuffix");
                            decimal multiplier = decimal.Parse(XMLHelper.safeAttributes(statnode, "multiplier"));
                            int precision = int.Parse(XMLHelper.safeAttributes(statnode, "precision"));
                            if (bool.Parse(XMLHelper.safeAttributes(statnode, "keystat")))
                                summarykeystat = statname;
                            bool alwaysaddsuffix = bool.Parse(XMLHelper.safeAttributes(statnode, "alwaysaddsuffix"));
                            
                            if (value.StartsWith("CustomStats."))
                            {
                                string valuestring = customStatReplacer(ds, vs, bs, value);
                                if (DataHelper.isnumeric(valuestring))
                                {
                                    decimal valueliteral = decimal.Parse(valuestring);
                                    StatAdd(ref statstopull, new StatisticsClassWrapper(cat, statname, double.Parse((valueliteral / divisor).ToString()), precision, divisorsuffix, multiplier, alwaysaddsuffix), onlyenabled);
                                }
                                else
                                {
                                    StatAdd(ref statstopull, new StatisticsClassWrapper(cat, statname, 0), onlyenabled);
                                    statstopull.Last().statstring = valuestring;
                                }
                            }
                            else if (value.StartsWith("DistrictStats.") 
                                || value.StartsWith("VehicleStats.")
                                || value.StartsWith("BuildingStats."))
                            {
                                try
                                {
                                    object queryobject = ds;
                                if (value.StartsWith("VehicleStats.")) queryobject = vs;
                                if (value.StartsWith("BuildingStats")) queryobject = bs;

                                string valuestring = "0";
                                decimal valueliteral = 0;
                                System.Reflection.FieldInfo[] mi = queryobject.GetType().GetFields();
                                
                                foreach (System.Reflection.FieldInfo m in mi)
                                {
                                    
                                    if (value.ToUpper().Trim().EndsWith("." + m.Name.ToUpper().Trim()))
                                    {
                                        object fieldvalue =  m.GetValue(queryobject);
                                        if (fieldvalue != null)
                                        {
                                            if (DataHelper.isnumeric(fieldvalue))
                                            {
                                                if (decimal.TryParse(fieldvalue.ToString(), out valueliteral))
                                                    valuestring = valueliteral.ToString();

                                            }
                                        }
                                        break;
                                    }
                                }
                                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, statname, double.Parse((valueliteral / divisor).ToString()), precision, divisorsuffix, multiplier, alwaysaddsuffix), onlyenabled);
                                }
                                catch (Exception ex)
                                {
                                    statlog.log("cat: " + cat + " stat:" + statname + " error:" + ex.Message);
                                }
                            }
                            else if (value.StartsWith("StatisticType."))
                            {
                                foreach (StatisticType t in Enum.GetValues(typeof(StatisticType)))
                                {
                                    if (value.ToUpper().EndsWith("." + t.ToString().ToUpper()))
                                    {
                                        try
                                        {
                                            StatAdd(ref statstopull, new StatisticsClassWrapper(cat, statname, t, divisor, multiplier, divisorsuffix, precision), onlyenabled);
                                        }
                                        catch (Exception ex)
                                        {
                                            statlog.log("cat: " + cat + " stat:" + statname + " error:" + ex.Message);
                                        }

                                        break;
                                    }

                                }
                            }
                            else if (value.StartsWith("ImmaterialResourceManager.Resource."))
                            {
                                
                                foreach (ImmaterialResourceManager.Resource t in Enum.GetValues(typeof(ImmaterialResourceManager.Resource)))
                                {
                                    
                                    //statlog.log(value + " " + t.ToString());
                                    if (value.ToUpper().EndsWith("." + t.ToString().ToUpper()))
                                    {
                                        try
                                        {
                                            StatAdd(ref statstopull, new StatisticsClassWrapper(cat, statname, t, divisor, multiplier, divisorsuffix, precision), onlyenabled);
                                        }
                                        catch (Exception ex)
                                        {
                                            statlog.log("cat: " + cat + " stat:" + statname + " error:" + ex.Message);
                                        }

                                        break;
                                    }

                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(vehiclesummarycapacitystat))
                    {
                        double used1=0, used2=0, cap1=0, cap2=0;
                        if (DataHelper.isnumeric(summarycapacitystat)) cap1 = double.Parse(summarycapacitystat);
                        if (DataHelper.isnumeric(summaryusedstat)) used1 = double.Parse(summaryusedstat);
                        if (DataHelper.isnumeric(vehiclesummarycapacitystat)) cap2 = double.Parse(vehiclesummarycapacitystat);
                        if (DataHelper.isnumeric(vehiclesummaryusedstat)) used2 = double.Parse(vehiclesummaryusedstat);
                        
                        foreach (StatisticsClassWrapper scw in statstopull)
                        {
                            if (scw.m_desc.ToLower().Trim() == summaryusedstat.ToLower().Trim()) used1 = scw.m_finalvalue;
                            if (scw.m_desc.ToLower().Trim() == summarycapacitystat.ToLower().Trim()) cap1 = scw.m_finalvalue;
                            if (scw.m_desc.ToLower().Trim() == vehiclesummaryusedstat.ToLower().Trim()) used2 = scw.m_finalvalue;
                            if (scw.m_desc.ToLower().Trim() == vehiclesummarycapacitystat.ToLower().Trim()) cap2 = scw.m_finalvalue;
                        }
                        if (cap1 > 0 && cap2 > 0)
                        {
                            double d1 = (double)used1 / (double)cap1;
                            double d2 = (double)used2 / (double)cap2;
                            if (d2 > d1
                                && CSLStatsPanelConfigSettings.m_UseVechileStatsForSummaries.value)
                            {
                                summaryusedstat = vehiclesummaryusedstat;
                                summarycapacitystat = vehiclesummarycapacitystat;
                                showstatssummary = true;
                            }

                        }
                    }
                    else if (!string.IsNullOrEmpty(summarykeystat))
                    {
                        summaryusedstat = summarykeystat;
                        summarycapacitystat = "";
                        
                    }
                    catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, summaryusedstat, summarycapacitystat, icon, showstatssummary));
                    if (XMLHelper.safeAttributes(catnode, "summarystattargetred") != "")
                        catstopull.Last().m_targetred = float.Parse(XMLHelper.safeAttributes(catnode, "summarystattargetred"));
                    if (XMLHelper.safeAttributes(catnode, "summarystattargetyellow") != "")
                        catstopull.Last().m_targetyellow = float.Parse(XMLHelper.safeAttributes(catnode, "summarystattargetyellow"));

                    statstopull = new List<StatisticsClassWrapper>();
                }
            }
            return catstopull;
        }

        public static List<StatisticsCategoryWrapper> getstats3(bool onlyenabled = true)
        {
            
            return getstats3(onlyenabled, ThreadingCSLStatsMod.defaultXMLConfig);
            /*
             * old stat configuration below
             */

            onlyenabled = false;
            List<StatisticsCategoryWrapper> catstopull = new List<StatisticsCategoryWrapper>();
            List<StatisticsClassWrapper> statstopull = new List<StatisticsClassWrapper>();

            districtStats ds = new districtStats();
            buildingStats bs = new buildingStats();

            string cat = "Power";
            if (CSLStatsPanelConfigSettings.isCatActive(cat) || !onlyenabled)
            {
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Used", ds.dmusage / 1000, 2, "MW"), false);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Capacity", StatisticType.ElectricityCapacity, 1000, 16, "MW"), false);
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, ds.dmusage.ToString(), "Capacity", "InfoIconElectricity", true));
                statstopull = new List<StatisticsClassWrapper>();
            }
            
            cat = "Water";
            if (CSLStatsPanelConfigSettings.isCatActive(cat) || !onlyenabled)
            {
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Used", ds.waterbuffer, 2, "m³"), false);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Capacity", StatisticType.WaterCapacity, 1, 16, "m³"), false);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Pollution", StatisticType.WaterPollution, 1, 1, "%"), onlyenabled);
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "Used", "Capacity", "InfoIconWater", true));
                statstopull = new List<StatisticsClassWrapper>();
            }

            cat = "Sewage";
            if (CSLStatsPanelConfigSettings.isCatActive(cat) || !onlyenabled)
            {
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Used", ds.sewagebuffer, 2, "m³"),false);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Capacity", StatisticType.SewageCapacity, 1, 16, "m³"),false);
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "Used", "Capacity", "InfoIconWaterPressed", true));
                statstopull = new List<StatisticsClassWrapper>();
            }

            vehiclestats vs = new vehiclestats();
                
            cat = "Garbage";
            if (CSLStatsPanelConfigSettings.isCatActive(cat) || !onlyenabled)
            {
                string suffix = "K";
                if (ds.garbage < 1000) suffix = "";
                StatisticsClassWrapper tempscw = new StatisticsClassWrapper(cat, "Production", ds.garbage, 2, suffix, 1000);
                StatAdd(ref statstopull, tempscw,onlyenabled); // production

                tempscw = new StatisticsClassWrapper(cat, "Stored", StatisticType.GarbageAmount, 1000, 1, "K");
                int garbageamount = (int)ds.garbage + (int)tempscw.m_value;
                StatAdd(ref statstopull, tempscw, onlyenabled); ;


                if (garbageamount < 1000) suffix = ""; else suffix = "K";
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Total", garbageamount, 2, suffix, 1000), false);

                tempscw = new StatisticsClassWrapper(cat, "Storage", StatisticType.GarbageCapacity, 1000, 1, "K");
                int totalcapacity = (int)tempscw.m_value;
                StatAdd(ref statstopull, tempscw, onlyenabled);

                tempscw = new StatisticsClassWrapper(cat, "Incinerators", StatisticType.IncinerationCapacity, 1000, 16, "K");
                totalcapacity += (int)tempscw.m_value;
                StatAdd(ref statstopull, tempscw, onlyenabled);

                if (totalcapacity < 1000) suffix = ""; else suffix = "K";
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Capacity", totalcapacity, 2, suffix, 1000),false);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Piles", StatisticType.GarbagePiles, 1000, 1, "M"),onlyenabled);

                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Vehicles Used", vs.garbagetrucksinuse),onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Vehicles Total", bs.garbagetrucks),onlyenabled);
                double d1 = (double)garbageamount / (double)totalcapacity;
                double d2 = (double)vs.garbagetrucksinuse / (double)bs.garbagetrucks;
                if ( d1 > d2
                    || !CSLStatsPanelConfigSettings.m_UseVechileStatsForSummaries.value)
                    catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, garbageamount.ToString(), totalcapacity.ToString(), "InfoIconGarbage", true));
                else catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, vs.garbagetrucksinuse.ToString(), bs.garbagetrucks.ToString(), "InfoIconGarbage", true));
                statstopull = new List<StatisticsClassWrapper>();
            }

            cat = "Health Services";
            if (CSLStatsPanelConfigSettings.isCatActive(cat) || !onlyenabled)
            {
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Health", ImmaterialResourceManager.Resource.Health, 1, 1, "%"),false);
                StatisticsClassWrapper tempscw = statstopull[statstopull.Count() - 1];
                int health = (int)tempscw.m_value;
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Well Being", ImmaterialResourceManager.Resource.Wellbeing, 1, 1, "%"),onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Sick", ds.sickcount),onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Capacity", StatisticType.HealCapacity, 1, 1, ""),onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Vehicles Used", vs.healthcarevehiclesinuse),onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Vehicles Total", bs.healthcarevehicles),onlyenabled);
                double d1 = (100 - health) / 30d;
                double d2 = vs.healthcarevehiclesinuse / (double)bs.healthcarevehicles;
                if (d1 > d2 || !CSLStatsPanelConfigSettings.m_UseVechileStatsForSummaries.value)
                    catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, (100 - health).ToString(), "30", "InfoIconHealth"));
                else catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, vs.healthcarevehiclesinuse.ToString(), bs.healthcarevehicles.ToString(), "InfoIconHealth", true));
                statstopull = new List<StatisticsClassWrapper>();
            }
            
            cat = "Death Services";
            if (CSLStatsPanelConfigSettings.isCatActive(cat) || !onlyenabled)
            {

                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Amount", StatisticType.DeadAmount, 1, 1, ""),false);
                double deadamount = statstopull.Last().m_value + ds.deadamount;
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Waiting", ds.deadamount), false);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "In Transit", vs.bodiesintransit), false);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Capacity", StatisticType.DeadCapacity, 1, 1, ""), false);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Cremate Capacity", StatisticType.CremateCapacity, 1000, 1, "K"),false);
                float totaldeathcap = statstopull[statstopull.Count() - 2].m_value + statstopull[statstopull.Count - 1].m_value;
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Vehicles Used", vs.hearseinuse),onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Vehicles Total", bs.hearse),onlyenabled);
                double d1 = deadamount / totaldeathcap;
                double d2 = vs.hearseinuse / (double)bs.hearse;
                if (d1 > d2 || !CSLStatsPanelConfigSettings.m_UseVechileStatsForSummaries.value)
                    catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, deadamount.ToString(), totaldeathcap.ToString(), "InfoIconHealthPressed", true));
                else catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, vs.hearseinuse.ToString(), bs.hearse.ToString(), "InfoIconHealthPressed", true));
                statstopull = new List<StatisticsClassWrapper>();
            }

            cat = "Buildings";
            if (CSLStatsPanelConfigSettings.isCatActive(cat) || !onlyenabled)
            {
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Count", bs.buildingcount, 2, ""),false);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Abandoned", StatisticType.AbandonedBuildings, 1, 1, ""),false);
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "Abandoned", (bs.buildingcount * .25).ToString(), "ToolbarIconZoning"));
                statstopull = new List<StatisticsClassWrapper>();
            }
            
            cat = "Fire";
            if (CSLStatsPanelConfigSettings.isCatActive(cat) || !onlyenabled)
            {
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Hazard", ImmaterialResourceManager.Resource.FireHazard, 1, 1, "%"),onlyenabled);
                float hazard = statstopull.Last().m_value;
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Buildings Burning", bs.onfire, 2, ""),onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Vehicles Used", vs.firetrucksinuse),onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Vehicles Total", bs.firetrucks),onlyenabled);

                double d1 = hazard / 50d;
                double d2 = vs.firetrucksinuse / (double)bs.firetrucks;
                if (d1 > d2 || !CSLStatsPanelConfigSettings.m_UseVechileStatsForSummaries.value)
                    catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, hazard.ToString(), "50", "InfoIconFireSafety"));
                else catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, vs.firetrucksinuse.ToString(), bs.firetrucks.ToString(), "InfoIconFireSafety", true));
                statstopull = new List<StatisticsClassWrapper>();
            }
            //statstopull.Add(new StatisticsClassWrapper("Health Services", "Heath Care", ImmaterialResourceManager.Resource.HealthCare, 1, 1, "%"));
            //statstopull.Add(new StatisticsClassWrapper("Health Services", "Health", ImmaterialResourceManager.Resource.Health, 1, 1, "%"));

            cat = "Economy";

            if (CSLStatsPanelConfigSettings.isCatActive(cat) || !onlyenabled)
            {
                if (!pollsinitialized) InitializePolls();
                //UpdateIncomeExpenses();
                double myincome = 0, myexpenses = 0;
                basicIncomePolls[0].Poll(Settings.moneyFormat, LocaleManager.cultureInfo);
                myincome += basicIncomePolls[0].income;

                for (int j = 0; j < budgetExpensesPolls.Length; j++)
                {
                    budgetExpensesPolls[j].Poll(Settings.moneyFormat, LocaleManager.cultureInfo);
                    myexpenses += budgetExpensesPolls[j].expenses;
                }
                myexpenses += expensesPoliciesTotal;
                //StatisticsClassWrapper tempscw = new StatisticsClassWrapper(cat, "Service Expenses", StatisticType.ServiceExpenses, 1, 1, "");
                int servicesexpenses = (int)myexpenses / 100 ; // tempscw.m_value / 100;
                int servicesincome = (int)myincome / 100; // tempscw.m_value / 100;
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Budget", servicesincome - servicesexpenses),onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Income", servicesincome), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Expenses", servicesexpenses), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Entertainment", ImmaterialResourceManager.Resource.Entertainment, 1, 1, ""), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Attractiveness", ImmaterialResourceManager.Resource.Attractiveness, 1, 1, ""), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Cargo Transport", ImmaterialResourceManager.Resource.CargoTransport, 1, 1, ""), onlyenabled);
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Imports", vs.cargoimports, 2, "K", 1000, false), onlyenabled);
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Exports", vs.cargoexports, 2, "K", 1000, false), onlyenabled);
                //statstopull.Add(new StatisticsClassWrapper("Misc", "Coverage?", ImmaterialResourceManager.Resource.Coverage, 1, 1, ""));
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Density", ImmaterialResourceManager.Resource.Density, 1, 1, ""), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Land Value", ImmaterialResourceManager.Resource.LandValue, 1, 1, "₡/m²"), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "ImmaterialResource", StatisticType.ImmaterialResource, 1000, 1, "M"), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Goods Produced", StatisticType.GoodsProduced, 1000, 1, "K"), onlyenabled);
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, servicesexpenses.ToString(), servicesincome.ToString(), "InfoIconLandValue"));
                statstopull = new List<StatisticsClassWrapper>();
            }

            cat = "Trade";
            if (CSLStatsPanelConfigSettings.isCatActive(cat) || !onlyenabled)
            {
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Imports", vs.cargoimports, 2, "K", 1000, false), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Exports", vs.cargoexports, 2, "K", 1000, false), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Intracity", vs.intracitytransports, 2, "K", 1000, false), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Imports Car", vs.carimports, 2, "K", 1000, false), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Exports Car", vs.carexports, 2, "K", 1000, false), onlyenabled);
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Imports Metro", vs.metroimports, 2, "K", 1000), onlyenabled);
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Exports Metro", vs.metroexports, 2, "K", 1000), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Imports Train", vs.trainimports, 2, "K", 1000, false), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Exports Train", vs.trainexports, 2, "K", 1000, false), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Imports Ship", vs.shipimports, 2, "K", 1000, false), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Exports Ship", vs.shipexports, 2, "K", 1000, false), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Imports Tram", vs.tramimports, 2, "K", 1000, false), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Exports Tram", vs.tramexports, 2, "K", 1000, false), onlyenabled);
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Imports Plane", vs.planeimports, 2, "K", 1000), onlyenabled);
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Exports Plane", vs.planeexports, 2, "K", 1000), onlyenabled);
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Ind Imports", vs.industryimports, 2, "K", 1000, false), onlyenabled);
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Ind Exports", vs.industryexports, 2, "K", 1000, false), onlyenabled);
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Com Imports", vs.commercialimports, 2, "K", 1000, false), onlyenabled);
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Com Exports", vs.commercialexports, 2, "K", 1000, false), onlyenabled);
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Off Imports", vs.officeimports, 2, "K", 1000, false), onlyenabled);
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Off Exports", vs.officeexports, 2, "K", 1000, false), onlyenabled);
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "", "", "InfoIconOutsideConnections"));
                statstopull = new List<StatisticsClassWrapper>();
            }

            cat = "Citizens";
            if (CSLStatsPanelConfigSettings.isCatActive(cat) || !onlyenabled)
            {
                
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Count", ds.citizencount, 2, ""),onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Move Rate", StatisticType.MoveRate, 1, 1, ""), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Birth Rate", StatisticType.BirthRate, 1, 1, ""), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Death Rate", StatisticType.DeathRate, 1, 1, ""), onlyenabled);

                uint homeless = ds.educationData[0].m_finalHomeless
                    + ds.educationData[1].m_finalHomeless
                    + ds.educationData[2].m_finalHomeless
                    + ds.educationData[3].m_finalHomeless;

                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Homeless", homeless), onlyenabled);

                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "Death Rate", "Birth Rate", "InfoIconPopulation"));
                statstopull = new List<StatisticsClassWrapper>();
            }

            float eligibleworkers = ds.educationData[0].m_finalEligibleWorkers
                    + ds.educationData[1].m_finalEligibleWorkers
                    + ds.educationData[2].m_finalEligibleWorkers
                    + ds.educationData[3].m_finalEligibleWorkers;

            cat = "Education";
            if (CSLStatsPanelConfigSettings.isCatActive(cat) || !onlyenabled)
            {
                int educatedcount = ds.educated1 + ds.educated2 + ds.educated3;
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Educated", educatedcount),onlyenabled);

                for (int i = 0; i < 4; i++)
                {
                    StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Level " + i.ToString(), ds.educated0), onlyenabled);
                    statstopull.Last().statstring = 
                        (ds.educationData[i].m_finalEligibleWorkers == 0) ? string.Format("L{0}: 0.00% - 0", i) :
                        string.Format("L{2}: {0:0.00%} - {1}",
                        ds.educationData[i].m_finalCount / (double)ds.citizencount,
                        ds.educationData[i].m_finalCount,
                        i);
                }
                
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Students", StatisticType.StudentCount, 1, 1, ""),onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Max Students", StatisticType.EducationCapacity, 1, 1, ""),onlyenabled);
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "Students", "Max Students", "InfoIconEducation", true));
                statstopull = new List<StatisticsClassWrapper>();
            }

            cat = "Unemployment";
            if (CSLStatsPanelConfigSettings.isCatActive(cat) || !onlyenabled)
            {
                statstopull.Add(new StatisticsClassWrapper(cat, "Unemployed", StatisticType.Unemployed, 1, 1, ""));
                for (int i = 0; i < 4; i++)
                {
                    StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Level " + i.ToString(), ds.educated0), onlyenabled);
                    statstopull.Last().statstring = 
                        (ds.educationData[i].m_finalEligibleWorkers == 0) ? string.Format("L{0}: 0.00% - 0", i) : string.Format("L{2}: {1:0.00%} - {0}",
                        ds.educationData[i].m_finalUnemployed,
                        ds.educationData[i].m_finalUnemployed / (double)ds.educationData[i].m_finalEligibleWorkers,
                        i);
                }

                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Eligible Workers", StatisticType.EligibleWorkers, 1, 1, ""), onlyenabled);
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "Unemployed", eligibleworkers.ToString(), "InfoIconPopulationPressed", true));
                catstopull.Last().m_targetred = .25f;
                catstopull.Last().m_targetyellow = .15f;
                statstopull = new List<StatisticsClassWrapper>();

            }

            cat = "Crime";
            if (CSLStatsPanelConfigSettings.isCatActive(cat) || !onlyenabled)
            {

                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Crime Rate", ds.finalcrimerate, 2, "%"),false);
                float crimerate = statstopull.Last().m_value;
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Crimes", StatisticType.CrimeRate, 1, 1, ""),onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Vehicles Used", vs.policecarsinuse),onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Vehicles Total", bs.policecars),onlyenabled);

                double d1 = crimerate / 20d;
                double d2 = vs.policecarsinuse / (double)bs.policecars;
                //if (d1 > d2 || !CSLStatsPanelConfigSettings.m_UseVechileStatsForSummaries.value)
                    catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, crimerate.ToString(), "20", "InfoIconCrime"));
                //else catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, vs.policecarsinuse.ToString(), bs.policecars.ToString(), "InfoIconCrime", true));
                statstopull = new List<StatisticsClassWrapper>();
            }

            cat = "Tourists";
            if (CSLStatsPanelConfigSettings.isCatActive(cat) || !onlyenabled)
            {

                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Visits", StatisticType.TouristVisits, 1, 1, ""),onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Incoming", StatisticType.IncomingTourists, 1, 1, ""),onlyenabled);
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "", "", "InfoIconPublicTransportPressed"));
                statstopull = new List<StatisticsClassWrapper>();
            }

            cat = "Public Transit";
            if (CSLStatsPanelConfigSettings.isCatActive(cat) || !onlyenabled)
            {

                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Availability", ImmaterialResourceManager.Resource.PublicTransport, 1, 1, "%"),onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Avg Passengers", StatisticType.AveragePassengers, 1, 1, ""), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Busses", vs.passengerbusses), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Metro", vs.passengermetro), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Tram", vs.passengertram), onlyenabled);
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Trains", vs.passengertrains), onlyenabled);
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Ships", vs.passengerships), onlyenabled);
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Planes", vs.passengerplanes), onlyenabled);
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "80", "Availability", "InfoIconPublicTransport"));
                statstopull = new List<StatisticsClassWrapper>();
            }

            cat = "Pollution";
            if (CSLStatsPanelConfigSettings.isCatActive(cat) || !onlyenabled)
            {
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Noise", ImmaterialResourceManager.Resource.NoisePollution, 1, 1, "%"),onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Ground", ds.groundpollution, 2, "%"),onlyenabled);

                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "", "", "InfoIconPollution"));
                statstopull = new List<StatisticsClassWrapper>();
            }

            cat = "Traffic";
            if (CSLStatsPanelConfigSettings.isCatActive(cat) || !onlyenabled)
            {
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Vehicles", vs.activevehicles, 2, ""), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Avg Delay", vs.delay / vs.activevehicles, 2, ""), onlyenabled);
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Max Delay", vs.highestdelay, 2, ""), onlyenabled);
                
                double avgstat = MasterStatsWrapper.TrafficWaitAverage.AddStat((float)(vs.delay / vs.activevehicles));
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, avgstat.ToString(), "1", "InfoIconTrafficCongestion", true));
                statstopull = new List<StatisticsClassWrapper>();
            }
            cat = "System Stats";
            if (CSLStatsPanelConfigSettings.isCatActive(cat) || !onlyenabled)
            {
                StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "FPS", ThreadingCSLStatsMod.framespersecond),onlyenabled);
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "CPU", UnityEngine.SystemInfo.systemMemorySize, 2, "%"), onlyenabled);
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Cities", systemstats.GetCitiesValue(), 2, "%"), onlyenabled);
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Unity Alloc RAM", UnityEngine.Profiler.GetTotalAllocatedMemory() / 1024 / 1024, 2, "MB"), onlyenabled);
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Unity Res RAM", UnityEngine.Profiler.GetTotalReservedMemory() / 1024 / 1024, 2, "MB"), onlyenabled);
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "Unity Free RAM", UnityEngine.Profiler.GetTotalUnusedReservedMemory() / 1024 / 1024, 2, "MB"), onlyenabled);
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "SYS RAM", UnityEngine.SystemInfo.systemMemorySize, 2, "MB"), onlyenabled);
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "GPU RAM", UnityEngine.SystemInfo.graphicsMemorySize, 2, "MB"), onlyenabled);
                
                //StatAdd(ref statstopull, new StatisticsClassWrapper(cat, "RAM", ramCounter.NextValue(), 2, "%"), onlyenabled);
                catstopull.Add(new StatisticsCategoryWrapper(cat, statstopull, "", "", "ToolbarIconHelp"));
                statstopull = new List<StatisticsClassWrapper>();
            }
            return catstopull;
        }


        private static IncomeExpensesPoll[] basicExpensesPolls;
        private static IncomeExpensesPoll[] basicIncomePolls;
        private static IncomeExpensesPoll[] publicTransportDetailExpensesPolls;
        private static IncomeExpensesPoll[] budgetExpensesPolls;

        private static void InitializePolls()
        {
            basicExpensesPolls = new IncomeExpensesPoll[]
		    {
			    new IncomeExpensesPoll(ItemClass.Service.Road, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.Electricity, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.Water, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.Garbage, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.HealthCare, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.FireDepartment, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.PoliceDepartment, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.Education, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.Monument, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.Beautification, null, null)
		    }; 
            basicIncomePolls = new IncomeExpensesPoll[]
		    {
			    new IncomeExpensesPoll(ItemClass.Service.None, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.Residential, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.Commercial, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.Industrial, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.Office, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.PublicTransport, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.Residential, ItemClass.SubService.ResidentialLow, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.Residential, ItemClass.SubService.ResidentialHigh, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.Commercial, ItemClass.SubService.CommercialLow, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.Commercial, ItemClass.SubService.CommercialHigh, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.Tourism, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.Citizen, null, null)
		    };
            publicTransportDetailExpensesPolls = new IncomeExpensesPoll[]
		    {
			    new IncomeExpensesPoll(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportBus, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportMetro, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrain, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportShip, null, null),
			    new IncomeExpensesPoll(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportPlane, null, null)
		    };
            budgetExpensesPolls = new IncomeExpensesPoll[]
		    {
			    basicExpensesPolls[0],
			    basicExpensesPolls[1],
			    basicExpensesPolls[2],
			    basicExpensesPolls[3],
			    basicExpensesPolls[4],
			    basicExpensesPolls[5],
			    basicExpensesPolls[6],
			    basicExpensesPolls[7],
			    basicExpensesPolls[8],
			    basicExpensesPolls[9],
			    publicTransportDetailExpensesPolls[0],
			    publicTransportDetailExpensesPolls[1],
			    publicTransportDetailExpensesPolls[2],
			    publicTransportDetailExpensesPolls[3],
			    publicTransportDetailExpensesPolls[4]
		    };
            pollsinitialized = true;
        }

        private static bool pollsinitialized = false;

        private sealed class IncomeExpensesPoll
        {
            private ItemClass.Service m_Service;
            private ItemClass.SubService m_SubService;
            private ItemClass.Level m_Level;
            private long m_Income = 9223372036854775807L;
            private long m_Expenses = 9223372036854775807L;
            private string m_IncomeString = "N/A";
            private string m_ExpensesString = "N/A";
            private bool m_Bound;
            private string m_IncomeFieldName;
            private string m_ExpensesFieldName;
            private UITextComponent m_IncomeField;
            private UITextComponent m_ExpensesField;
            public string incomeString
            {
                get
                {
                    return this.m_IncomeString;
                }
            }
            public string expensesString
            {
                get
                {
                    return this.m_ExpensesString;
                }
            }
            public double income
            {
                get
                {
                    return (double)this.m_Income;
                }
            }
            public double expenses
            {
                get
                {
                    return (double)this.m_Expenses;
                }
            }
            public IncomeExpensesPoll(ItemClass.Service service, string incomeFieldName, string expensesFieldName)
            {
                this.m_Service = service;
                this.m_SubService = ItemClass.SubService.None;
                this.m_Level = ItemClass.Level.None;
                this.m_IncomeFieldName = incomeFieldName;
                this.m_ExpensesFieldName = expensesFieldName;
            }
            public IncomeExpensesPoll(ItemClass.Service service, ItemClass.SubService subService, string incomeFieldName, string expensesFieldName)
            {
                this.m_Service = service;
                this.m_SubService = subService;
                this.m_Level = ItemClass.Level.None;
                this.m_IncomeFieldName = incomeFieldName;
                this.m_ExpensesFieldName = expensesFieldName;
            }
            public IncomeExpensesPoll(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, string incomeFieldName, string expensesFieldName)
            {
                this.m_Service = service;
                this.m_SubService = subService;
                this.m_Level = level;
                this.m_IncomeFieldName = incomeFieldName;
                this.m_ExpensesFieldName = expensesFieldName;
            }
            public void Poll(string moneyFormat, CultureInfo moneyLocale)
            {
                long num = 0L;
                long num2 = 0L;
                if (Singleton<EconomyManager>.exists)
                {
                    Singleton<EconomyManager>.instance.GetIncomeAndExpenses(this.m_Service, this.m_SubService, this.m_Level, out num, out num2);
                }
                if (num != this.m_Income)
                {
                    this.m_Income = num;
                    this.m_IncomeString = ((double)this.m_Income / 100.0).ToString(moneyFormat, moneyLocale);
                }
                if (num2 != this.m_Expenses)
                {
                    this.m_Expenses = num2;
                    this.m_ExpensesString = ((double)this.m_Expenses / 100.0).ToString(moneyFormat, moneyLocale);
                }
            }
            public void Update(UIComponent root)
            {
                if (!this.m_Bound)
                {
                    this.m_Bound = true;
                    this.m_IncomeField = root.Find<UITextComponent>(this.m_IncomeFieldName);
                    this.m_ExpensesField = root.Find<UITextComponent>(this.m_ExpensesFieldName);
                }
                else
                {
                    if (this.m_IncomeField != null)
                    {
                        this.m_IncomeField.text = this.incomeString;
                    }
                    if (this.m_ExpensesField != null)
                    {
                        this.m_ExpensesField.text = this.expensesString;
                    }
                }
            }
        }
  
        public static long expensesPoliciesTotal
        {
            get
            {
                long policyexpense = 0;
                if (Singleton<EconomyManager>.exists)
                {
                    policyexpense = Singleton<EconomyManager>.instance.GetPolicyExpenses();
                }
                return policyexpense;
            }
        }

    }

}

