using System;
using System.Collections.Generic;

namespace CommonSuite
{
    public enum VINCarModel : int
    {
        Unknown = 0,
        Saab900 = 1,
        Saab9000 = 2,
        Saab900SE = 3,
        Saab93 = 4,
        Saab93new = 5,
        Saab95 = 6,
        Saab95new = 7,
        Saab99 = 8,
        OpelSignum = 9,
        OpelVectra = 10,
        CadillacBTS = 11
    }

    public enum VINEngineType : int
    {
        Unknown,
        B204,
        B204E,
        B204L,
        B204R,
        B205,
        B205E,
        B205L,
        B205R,
        B234,
        B234I,
        B234E,
        B234L,
        B234R,
        B235,
        B235E,
        B235L,
        B235R,
        B202I,
        B202L,
        B202E,
        B212,
        B201,
        B308E,
        B206I,
        B258I,
        B308I,
        B204I,
        B204S,
        B207F,
        B207G,
        B207H,
        B207R, /* 2.0T */
        B207S,
        B207L, /* 2.0t */
        B207E, /* 1.8t */
        B207M,
        Z20NET,
        D223L,
        D308L,
        Z18XE, /* 1.8i */
        Z19DT, /* 1.9TiD, 8-valve */
        Z19DTH, /* 1.9TiD, 16-valve */
        Z19DTR,
        B284L, /* 2.8t */
        B284E,
        B284R,
        Z19DTR_130HP,
        Z19DTR_160HP,
        Z19DTR_180HP,
        A28NET_LBW,
        A28NER_LAU,
        A20DTH_LBS,
        A20DTR_LBY,
        A20NHT_LDK,
        A20NFT_LHU, /* Turbo4 */
        A20NFT_LHU_BP,
        A16LET_LLU,
    }

    public enum VINTurboModel : int
    {
        Unknown,
        None,
        GarretT25, 

        GarretTB2531,
        MitsubishiTD04,
        GarrettT17,

        GarretTB2529, //B2x4E_L 9000
        MitsubishiTD04HL_15T_6, //B234R

        GarrettTB2569, //B204_E_L_R NG900_OG9-3

        GarrettGT1752, //B205E_L
        MitsubishiTD04HL_15T_5, //B205R B235L_R

        GarrettGT2052, //B207E_L 2003-2006
        MitsubishiTD04L_14T, //B207R
        MitsubishiTD04L_11TK, //B207E_L 2007->
        MitsubishiTD04HL_15TK, //B284L_R
        MitsubishiTD04HL_19TK3, //A28NET_LBW and A28NER_LAU

        MitsubishiTD04L6_04H_19TK3S, //Some A20NHT_and A20NFT
        BorgWarnerK04_2277DCB, //A20NHT_LDK A20NFT_LHU
        
    }

    public class VINDecoder
    {
        public VINCarInfo DecodeVINNumber(string VINNumber)
        {
            // Pos 1-3 World Manufacturer Identifier
            // Pos 1 Geographical Area: Y=Northern Europe
            // Pos 2 Country: S=Sweden K=Finland
            // Pos 3 Car Manuacturer: 3=Saab Automobile AB
            // 
            VINCarInfo _carInfo = new VINCarInfo();
            if (VINNumber.StartsWith("YK1") || // finland
                VINNumber.StartsWith("YS3"))  // sweden
            {
                _carInfo.Makeyear = DecodeMakeyear(VINNumber);
                _carInfo.CarModel = DecodeCarModel(VINNumber);
                _carInfo.EngineType = DecodeEngineType(VINNumber, _carInfo.CarModel, _carInfo.Makeyear);
                _carInfo.PlantInfo = DecodePlantInfo(VINNumber);
                _carInfo.Series = DecodeSeries(VINNumber, _carInfo.CarModel, _carInfo.Makeyear);
                _carInfo.Body = DecodeBodyType(VINNumber);
                _carInfo.TurboModel = DecodeTurboModel(_carInfo.EngineType, _carInfo.CarModel, _carInfo.Makeyear);
                _carInfo.GearboxDescription = DecodeTransmissionType(VINNumber);
                _carInfo.Valid = true;
            }
            else if (VINNumber.StartsWith("YSC")) // Cadillac
            {
                _carInfo.Makeyear = DecodeMakeyear(VINNumber);
                _carInfo.CarModel = VINCarModel.CadillacBTS;
                _carInfo.EngineType = DecodeEngineType(VINNumber, _carInfo.CarModel, _carInfo.Makeyear);
                _carInfo.PlantInfo = DecodePlantInfo(VINNumber);
                _carInfo.Series = DecodeSeries(VINNumber, _carInfo.CarModel, _carInfo.Makeyear);
                _carInfo.Body = DecodeBodyType(VINNumber);
                _carInfo.TurboModel  = VINTurboModel.Unknown;
                _carInfo.GearboxDescription = DecodeTransmissionType(VINNumber);
                _carInfo.Valid = true;
                /*
                Engines:
                1.9 L Fiat turbodiesel I4 16v, 150 hp (110 kW)
                1.9 L Fiat turbodiesel I4 16v, 180 hp (132 kW) (2007-)
                2.0 L Ecotec LK9 I4, mid-pressure turbo, 175 hp (129 kW)
                2.0 L Ecotec LK9 I4, high-pressure turbo, 210 hp (154 kW)
                2.0 T FlexPower 200 hp (147 kW)
                2.8 L HFV6 V6, turbo, 250 hp (184 kW)
                */
            }

            else if (VINNumber.StartsWith("W0L")) // Opel
            {
                _carInfo.Makeyear = DecodeMakeyear(VINNumber);
                _carInfo.CarModel = DecodeCarModelOpel(VINNumber);
                _carInfo.EngineType = VINEngineType.Z20NET;
                _carInfo.PlantInfo = "";
                _carInfo.Series = DecodeSeries(VINNumber, _carInfo.CarModel, _carInfo.Makeyear);
                _carInfo.Body = DecodeBodyType(VINNumber);
                _carInfo.TurboModel = DecodeTurboModel(_carInfo.EngineType, _carInfo.CarModel, _carInfo.Makeyear);
                _carInfo.GearboxDescription = DecodeTransmissionType(VINNumber);
                _carInfo.Valid = true;
            }

            _carInfo.CalculatedChecksum = CalculateChecksum(VINNumber);

            return _carInfo;
        }

        private VINCarModel DecodeCarModelOpel(string VINNumber)
        {
            if (VINNumber.Length < 8) return VINCarModel.Unknown;
            else if (VINNumber[7] == '3') return VINCarModel.OpelVectra;
            else if (VINNumber[7] == '4') return VINCarModel.OpelSignum;
            else return VINCarModel.Unknown;
        }

        private VINTurboModel DecodeTurboModel(VINEngineType vINEngineType, VINCarModel carModel, int makeYear)
        {
            // depending on enginetype and vehicletype, determine turbo type
            switch (vINEngineType)
            {
                case VINEngineType.B204E:
                case VINEngineType.B204L:
                case VINEngineType.B204R:
                    return VINTurboModel.GarretT25; //Need to diffrentiate NG900 and OG9-3 from 9000
                case VINEngineType.B205E:
                case VINEngineType.B205L:
                    return VINTurboModel.GarrettT17;
                case VINEngineType.B205R:
                    return VINTurboModel.MitsubishiTD04HL_15T_5;
                case VINEngineType.B234E:
                case VINEngineType.B234L:
                    return VINTurboModel.GarretTB2529;
                case VINEngineType.B234R:
                    return VINTurboModel.MitsubishiTD04HL_15T_6;
                case VINEngineType.B235E:
                    return VINTurboModel.GarrettGT1752;
                case VINEngineType.B235L:
                case VINEngineType.B235R:
                    return VINTurboModel.MitsubishiTD04HL_15T_5;
                case VINEngineType.B207E:
                case VINEngineType.B207L:
                    if (makeYear > 2005)
                        return VINTurboModel.MitsubishiTD04L_11TK;
                    else
                        return VINTurboModel.GarrettGT2052;
                case VINEngineType.B207R:
                    return VINTurboModel.MitsubishiTD04L_14T;
                case VINEngineType.B284R:
                    return VINTurboModel.MitsubishiTD04HL_15TK;
                case VINEngineType.A28NER_LAU:
                    return VINTurboModel.MitsubishiTD04HL_19TK3;
                case VINEngineType.B207H:
                    return VINTurboModel.MitsubishiTD04L_14T;
                case VINEngineType.B207G:
                    return VINTurboModel.MitsubishiTD04L_14T;
                case VINEngineType.B207S:
                    return VINTurboModel.MitsubishiTD04L_14T;
                case VINEngineType.B207M:
                    return VINTurboModel.MitsubishiTD04L_11TK;
                case VINEngineType.B207F:
                    return VINTurboModel.MitsubishiTD04L_11TK;
                case VINEngineType.A20NFT_LHU:
                    return VINTurboModel.BorgWarnerK04_2277DCB;
                case VINEngineType.A20NHT_LDK:
                    return VINTurboModel.BorgWarnerK04_2277DCB;
                case VINEngineType.A16LET_LLU:
                    return VINTurboModel.Unknown;
                case VINEngineType.A20NFT_LHU_BP:
                    return VINTurboModel.BorgWarnerK04_2277DCB;
                case VINEngineType.Z20NET:
                    if (makeYear > 2006)
                        return VINTurboModel.MitsubishiTD04L_11TK;
                    else
                        return VINTurboModel.GarrettGT2052;
            }
            return VINTurboModel.None;
        }

        private string DecodeTransmissionType(string VINNumber)
        {
            if (VINNumber.Length < 7) return string.Empty;
            else if (VINNumber[6] == '1') return "6 speed automatic / front wheel drive";
            else if (VINNumber[6] == '2') return "6 speed automatic / all wheel drive";
            else if (VINNumber[6] == '4') return "4 speed manual";
            else if (VINNumber[6] == '5') return "5 speed manual / front wheel drive";
            else if (VINNumber[6] == '6') return "6 speed manual / front wheel drive";
            else if (VINNumber[6] == '7') return "6 speed manual / all wheel drive";
            else if (VINNumber[6] == '8') return "4 speed automatic";
            else if (VINNumber[6] == '9') return "5 speed automatic / front wheel drive";
            else if (VINNumber[6] == 'A') return "6 speed automatic / front wheel drive";
            else if (VINNumber[6] == 'B') return "6 speed automatic / all wheel drive";
            else if (VINNumber[6] == 'C') return "5 speed automatic / front wheel drive";
            else if (VINNumber[6] == 'M') return "6 speed manual / front wheel drive";
            else if (VINNumber[6] == 'N') return "6 speed manual / all wheel drive";
            else if (VINNumber[6] == 'P') return "5 speed manual / front wheel drive";
            return string.Empty;
            /*
4 = 4-speed manual 
5 = 5-speed manual 
6 = 3-speed Automatic 
8 = 4-speed Automatic 
             * */
        }

        private string DecodeBodyType(string VINNumber)
        {
            if (VINNumber.Length < 6) return string.Empty;
            else if (VINNumber[5] == '2') return "2 door sedan";
            else if (VINNumber[5] == '3') return "3 door combi coupe (CK)";
            else if (VINNumber[5] == '4') return "4 door sedan (SN)";
            else if (VINNumber[5] == '5') return "5 door combi coupe";
            else if (VINNumber[5] == '6') return "5-door 9000 CS (5CS)";
            else if (VINNumber[5] == '7') return "2 door convertible (CV)";
            else return string.Empty;
            /*
2 = 2-door sedan 
3 = 3-door Combi Coupe (CK) 
4 = 4-door Sedan (SN) 
5 = 5-door Combi Coupe 
6 = 4-door Sedan, Extended Length (CD) 
7 = 2-door Convertible (CV) 

             * */
        }

        private string DecodeSeries(string VINNumber, VINCarModel carModel, int makeYear)
        {
            if (VINNumber.Length < 5) return string.Empty;
            else if (carModel == VINCarModel.Saab93new)
            {
                if (makeYear == 2008)
                {
                    if (VINNumber[4] == 'M') return "Saab 9-3 TurboX";
                }
                else if (makeYear > 2009)
                {
                    if (VINNumber[4] == 'A') return "Saab 9-3 Linear";
                    else if (VINNumber[4] == 'B') return "Saab 9-3 Vector";
                    else if (VINNumber[4] == 'C') return "Saab 9-3 Aero";
                    else if (VINNumber[4] == 'D') return "Saab 9-3 X";
                    else if (VINNumber[4] == 'E') return "Saab 9-3 Linear Convertible";
                    else if (VINNumber[4] == 'F') return "Saab 9-3 Vector Convertible";
                    else if (VINNumber[4] == 'G') return "Saab 9-3 Aero Convertible";
                }
                else
                {
                    if (VINNumber[4] == 'B') return "Saab 9-3 Linear";
                    else if (VINNumber[4] == 'D') return "Saab 9-3 Arc";
                    else if (VINNumber[4] == 'F') return "Saab 9-3 Vector";
                    else if (VINNumber[4] == 'H') return "Saab 9-3 Aero";
                }
            }
            else if (carModel == VINCarModel.Saab95new)
            {
                if (VINNumber[4] == 'N') return "Saab 9-5 Linear";
                else if (VINNumber[4] == 'P') return "Saab 9-5 Vector";
                else if (VINNumber[4] == 'R') return "Saab 9-5 Aero";
                else return string.Empty;
            }
            else if (VINNumber[4] == 'A') return "Model series I, Driver airbag";
            else if (VINNumber[4] == 'B') return "Model series I, Driver and passenger airbags";
            else if (VINNumber[4] == 'C') return "Model series II, Driver airbag";
            else if (VINNumber[4] == 'D') return "Model series II, Driver and passenger airbags";
            else if (VINNumber[4] == 'E') return "Model series III, Driver airbag";
            else if (VINNumber[4] == 'F') return "Model series III, Driver and passenger airbags";
            else if (VINNumber[4] == 'G') return "Model series IV, Driver airbag";
            else if (VINNumber[4] == 'H') return "Model series IV, Driver and passenger airbags";
            else if (VINNumber[4] == 'K') return "Model series V, Driver airbag";
            else if (VINNumber[4] == 'M') return "Model series V, Driver and passenger airbags";
            else if (VINNumber[4] == 'N') return "Model series VI, Driver airbag";
            else if (VINNumber[4] == 'P') return "Model series VI, Driver and passenger airbags";
            else if (VINNumber[4] == 'Y') return "All cars without passive restraint system";
            else if (VINNumber[4] == 'K') return "Linear, Driver and passenger airbags";
            else if (VINNumber[4] == 'L') return "Vector, Driver and passenger airbags";

            return string.Empty;
            /*
            B = i 
            C = i16 
            D = Turbo 
             * */
        }

        private string DecodePlantInfo(string VINNumber)
        {
            if (VINNumber.Length < 11) return string.Empty;
            else if (VINNumber[10] == '1') return "Trollh�ttan line A (9-3)";
            else if (VINNumber[10] == '2') return "Trollh�ttan line B (900 / 9-3)";
            else if (VINNumber[10] == '3') return "Trollh�ttan line A (9-5)";
            else if (VINNumber[10] == '4') return "Trollh�ttan (9-5)";
            else if (VINNumber[10] == '5') return "Malm�, Sweden";
            else if (VINNumber[10] == '6') return "Graz, Austria (9-3 convertible)";
            else if (VINNumber[10] == '7') return "Nystad, Finland (900 / 9-3)";
            else if (VINNumber[10] == '8') return "Nystad, Finland (9000)";
            else if (VINNumber[10] == '9') return "Trollh�ttan, Sweden, Pre-production workshop";
            /*
1 = Trollh�ttan line A 
2 = Trollh�ttan line B 
3 = Arl�v, Sweden 
5 = Malm�, Sweden 
6 = Nystad, Finland 
7 = Nystad, Finland 
8 = Nystad, Finland (9000) 
9 = Trollh�ttan Line C 
             * */
            return string.Empty;
        }

        private VINCarModel DecodeCarModel(string VINNumber)
        {
            if (VINNumber.Length < 4) return VINCarModel.Unknown;
            else if (VINNumber[3] == 'A') return VINCarModel.Saab900;
            else if (VINNumber[3] == 'B') return VINCarModel.Saab99;
            else if (VINNumber[3] == 'C') return VINCarModel.Saab9000;
            else if (VINNumber[3] == 'D') return VINCarModel.Saab93; // code=9400
            else if (VINNumber[3] == 'E') return VINCarModel.Saab95; // code=9600
            else if (VINNumber[3] == 'F') return VINCarModel.Saab93new; // code=9440
            else if (VINNumber[3] == 'G') return VINCarModel.Saab95new; // code=9650
            else return VINCarModel.Unknown;
        }

        private VINEngineType DecodeEngineType(string VINNumber, VINCarModel carModel, int makeYear)
        {
            if (VINNumber.Length < 8) return VINEngineType.Unknown;
            // else if (makeYear > 2010 && carModel == VINCarModel.Saab93new)
          //  {
                //For 9-3 model after 2010 we use special decoder as engine number is reused
                /*             
        A = Diesel biturbo (Z19DTR) 130 HP
        B = Diesel biturbo (Z19DTR) 160 HP
        D = Diesel biturbo (Z19DTR) 180 HP
        F = 1.8t (B207E)
        N = 2.0 T/163/BP (B207H)
        M = 2.0 T/163 (B207G)
        S = 2.0t (B207L)
        T = 2.0 TS/BP (B207S)
        U = 2.0 T/BP (B207M)
        X = 2.0 LPT/BP (B207F)
        Y = 2.0 Turbo (B207R)
                     */
              //  if (VINNumber[7] == 'A') return VINEngineType.Z19DTR_130HP;
              //  else if (VINNumber[7] == 'B') return VINEngineType.Z19DTR_160HP;
              //  else if (VINNumber[7] == 'D') return VINEngineType.Z19DTR_180HP;
              //  else if (VINNumber[7] == 'F') return VINEngineType.B207E;
              //  else if (VINNumber[7] == 'N') return VINEngineType.B207H;
              //  else if (VINNumber[7] == 'M') return VINEngineType.B207G;
              //  else if (VINNumber[7] == 'S') return VINEngineType.B207L;
              //  else if (VINNumber[7] == 'T') return VINEngineType.B207S;
              //  else if (VINNumber[7] == 'U') return VINEngineType.B207M;
              //  else if (VINNumber[7] == 'X') return VINEngineType.B207F;
              //  else if (VINNumber[7] == 'Y') return VINEngineType.B207R;
              //  else return VINEngineType.Unknown;
          //  }
            else if (makeYear == 2010)
            {
                if (VINNumber[7] == 'A') return VINEngineType.Z19DTR_130HP;
                else if (VINNumber[7] == 'B') return VINEngineType.Z19DTR_160HP;
                else if (VINNumber[7] == 'C') return VINEngineType.B205E;
                else if (VINNumber[7] == 'D') return VINEngineType.Z19DTR_180HP;
                else if (VINNumber[7] == 'E') return VINEngineType.B235E;
                else if (VINNumber[7] == 'F') return VINEngineType.B207E;
                else if (VINNumber[7] == 'G') return VINEngineType.B235R;
                else if (VINNumber[7] == 'H') return VINEngineType.A28NET_LBW;
                else if (VINNumber[7] == 'J') return VINEngineType.A28NER_LAU;
                else if (VINNumber[7] == 'L') return VINEngineType.A20DTR_LBY;
                else if (VINNumber[7] == 'N') return VINEngineType.B207H;
                else if (VINNumber[7] == 'M') return VINEngineType.B207G;
                else if (VINNumber[7] == 'P') return VINEngineType.Z19DTR;
                else if (VINNumber[7] == 'R') return VINEngineType.B284R;
                else if (VINNumber[7] == 'S') return VINEngineType.B207L;
                else if (VINNumber[7] == 'T') return VINEngineType.B207S;
                else if (VINNumber[7] == 'U') return VINEngineType.B207M;
                else if (VINNumber[7] == 'V') return VINEngineType.Z19DT;
                else if (VINNumber[7] == 'W') return VINEngineType.Z19DTH;
                else if (VINNumber[7] == 'X') return VINEngineType.B207F;
                else if (VINNumber[7] == 'Y') return VINEngineType.B207R;
                else if (VINNumber[7] == 'Z') return VINEngineType.A20NFT_LHU;
                
            }
            else if (makeYear == 2011)
            {
                if (VINNumber[7] == 'A') return VINEngineType.Z19DTR_130HP;
                else if (VINNumber[7] == 'B') return VINEngineType.Z19DTR_160HP;
                else if (VINNumber[7] == 'C') return VINEngineType.A20NFT_LHU;
                else if (VINNumber[7] == 'D') return VINEngineType.Z19DTR_180HP;
                else if (VINNumber[7] == 'F') return VINEngineType.B207E;
                else if (VINNumber[7] == 'G') return VINEngineType.A16LET_LLU;
                else if (VINNumber[7] == 'H') return VINEngineType.A28NET_LBW;
                else if (VINNumber[7] == 'J') return VINEngineType.A28NER_LAU;
                else if (VINNumber[7] == 'L') return VINEngineType.A20DTH_LBS;
                else if (VINNumber[7] == 'L') return VINEngineType.A20DTR_LBY;
                else if (VINNumber[7] == 'M') return VINEngineType.B207G;
                else if (VINNumber[7] == 'N') return VINEngineType.B207H;
                else if (VINNumber[7] == 'R') return VINEngineType.A20NFT_LHU_BP;
                else if (VINNumber[7] == 'S') return VINEngineType.B207L;
                else if (VINNumber[7] == 'T') return VINEngineType.B207S;
                else if (VINNumber[7] == 'U') return VINEngineType.B207M;
                else if (VINNumber[7] == 'X') return VINEngineType.B207F;
                else if (VINNumber[7] == 'Y') return VINEngineType.B207R;
                else if (VINNumber[7] == 'Z') return VINEngineType.A20NFT_LHU;
            }
            else if (makeYear == 2012)
            {
                if (VINNumber[7] == 'A') return VINEngineType.Z19DTR_130HP;
                else if (VINNumber[7] == 'B') return VINEngineType.Z19DTR_160HP;
                else if (VINNumber[7] == 'D') return VINEngineType.Z19DTR_180HP;
                else if (VINNumber[7] == 'Z') return VINEngineType.A20NFT_LHU;
            }
            

            // For all models and 9-3 before MY2011
            if (VINNumber[7] == 'A') return VINEngineType.B235L;
            else if (VINNumber[7] == 'B') return VINEngineType.Z18XE;
            else if (VINNumber[7] == 'C') return VINEngineType.B205E;
            else if (VINNumber[7] == 'D') return VINEngineType.D223L;
            else if (VINNumber[7] == 'E') return VINEngineType.B235E;
            else if (VINNumber[7] == 'F') return VINEngineType.B207E;
            else if (VINNumber[7] == 'G') return VINEngineType.B235R;
            else if (VINNumber[7] == 'H') return VINEngineType.B205L;
            else if (VINNumber[7] == 'J') return VINEngineType.B204I;
            else if (VINNumber[7] == 'K') return VINEngineType.B205R;
            else if (VINNumber[7] == 'L') return VINEngineType.D308L;
            else if (VINNumber[7] == 'M') return VINEngineType.B284E;
            else if (VINNumber[7] == 'N') return VINEngineType.B204L;
            else if (VINNumber[7] == 'P') return VINEngineType.Z19DTR;
            else if (VINNumber[7] == 'R') return VINEngineType.B284R;
            else if (VINNumber[7] == 'S') return VINEngineType.B207L;
            else if (VINNumber[7] == 'T') return VINEngineType.B204E;
            else if (VINNumber[7] == 'U') return VINEngineType.B284L;
            else if (VINNumber[7] == 'V') return VINEngineType.Z19DT;
            else if (VINNumber[7] == 'W') return VINEngineType.Z19DTH;
            else if (VINNumber[7] == 'Y') return VINEngineType.B207R;
            else if (VINNumber[7] == 'Z') return VINEngineType.B308E;

        /*
B = Fuel Injection, B234 (2.3 liter 16v) 
D = Fuel Injection, B202 (16-v) 
E = Fuel Injection, B212 (2.1 liter 16v) 
J = Fuel Injection, B201 (8-v) 
L = Turbo, B202 & intercooler (16-valve) 
M = Turbo, B234 
N = Turbo, B204 & intercooler (2.0 liter 16v with balance shafts) 
S = Turbo, B201 (8-valve) 
S = Turbo, B202 Low-pressure turbo (16-valve) 
V = 2.5 liter V-6 
         * */
            return VINEngineType.Unknown;
        }

        private int DecodeMakeyear(string VINNumber)
        {
            if (VINNumber.Length < 10) return 0;
            else if (VINNumber[9] == 'M') return 1991;
            else if (VINNumber[9] == 'N') return 1992;
            else if (VINNumber[9] == 'P') return 1993;
            else if (VINNumber[9] == 'R') return 1994;
            else if (VINNumber[9] == 'S') return 1995;
            else if (VINNumber[9] == 'T') return 1996;
            else if (VINNumber[9] == 'V') return 1997;
            else if (VINNumber[9] == 'W') return 1998;
            else if (VINNumber[9] == 'X') return 1999;
            else if (VINNumber[9] == 'Y') return 2000;
            else if (VINNumber[9] == '1') return 2001;
            else if (VINNumber[9] == '2') return 2002;
            else if (VINNumber[9] == '3') return 2003;
            else if (VINNumber[9] == '4') return 2004;
            else if (VINNumber[9] == '5') return 2005;
            else if (VINNumber[9] == '6') return 2006;
            else if (VINNumber[9] == '7') return 2007;
            else if (VINNumber[9] == '8') return 2008;
            else if (VINNumber[9] == '9') return 2009;
            else if (VINNumber[9] == 'A') return 2010;
            else if (VINNumber[9] == 'B') return 2011;
            else if (VINNumber[9] == 'C') return 2012;
            else if (VINNumber[9] == 'D') return 2013;
            else if (VINNumber[9] == 'E') return 2014;
            return 0;
        }

        private int TranslateVINdigit(char letter)
        {
            if (letter >= '0' && letter <= '9') return letter - '0';
            if (letter >= 'A' && letter <= 'Z')
            {
                if (letter == 'A' || letter == 'J') return 1;
                if (letter == 'B' || letter == 'K' || letter == 'S') return 2;
                if (letter == 'C' || letter == 'L' || letter == 'T') return 3;
                if (letter == 'D' || letter == 'M' || letter == 'U') return 4;
                if (letter == 'E' || letter == 'V' || letter == 'N') return 5;
                if (letter == 'F' || letter == 'W') return 6;
                if (letter == 'G' || letter == 'P' || letter == 'X') return 7;
                if (letter == 'H' || letter == 'Y') return 8;
                if (letter == 'R' || letter == 'Z') return 9;
            }
            return -1;
        }

        // VIN checksum, for more details: https://en.wikibooks.org/wiki/Vehicle_Identification_Numbers_(VIN_codes)/Check_digit
        public char CalculateChecksum(string VINnumber)
        {
            char checksum = '*'; // * means not possible to calculate checksum
            if (VINnumber.Length != 17) return checksum;
            int[] weights = new int[17] { 8, 7, 6, 5, 4, 3, 2, 10, 0, 9, 8, 7, 6, 5, 4, 3, 2 };
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                if (i == 8) continue; // Skip checksum digit
                int value = TranslateVINdigit(VINnumber[i]);
                if (value < 0) return checksum;
                sum += value * weights[i];
            }
            int checksumValue = sum % 11;
            if (checksumValue >= 0 && checksumValue <= 9) checksum = checksumValue.ToString()[0];
            else if (checksumValue == 10) checksum = 'X';
            return checksum;
        }

    }

    public class VINCarInfo
    {
        private string _body = string.Empty;

        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        private string _series = string.Empty;

        public string Series
        {
            get { return _series; }
            set { _series = value; }
        }

        private string _plantInfo = string.Empty;

        public string PlantInfo
        {
            get { return _plantInfo; }
            set { _plantInfo = value; }
        }

        private Int32 _makeyear = 0;

        public Int32 Makeyear
        {
            get { return _makeyear; }
            set { _makeyear = value; }
        }


        private bool _valid = false;

        public bool Valid
        {
            get { return _valid; }
            set { _valid = value; }
        }
        private VINCarModel _carModel = VINCarModel.Unknown;

        public VINCarModel CarModel
        {
            get { return _carModel; }
            set { _carModel = value; }
        }
        private VINEngineType _engineType = VINEngineType.Unknown;

        public VINEngineType EngineType
        {
            get { return _engineType; }
            set { _engineType = value; }
        }
        private VINTurboModel _turboModel = VINTurboModel.None;

        public VINTurboModel TurboModel
        {
            get { return _turboModel; }
            set { _turboModel = value; }
        }

        private string _gearboxDescription = string.Empty;

        public string GearboxDescription
        {
            get { return _gearboxDescription; }
            set { _gearboxDescription = value; }
        }

        private char _calculatedChecksum = '*';

        public char CalculatedChecksum
        {
            get { return _calculatedChecksum; }
            set { _calculatedChecksum = value; }
        }
    }
}
