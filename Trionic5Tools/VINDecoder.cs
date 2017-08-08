using System;
using System.Collections.Generic;
using System.Text;

namespace Trionic5Tools
{
    public enum VINCarModel : int
    {
        Unknown = 0,
        Saab900 = 1,
        Saab9000 = 2,
        Saab900SE = 3,
        Saab93 = 4,
        Saab95 = 5,
        Saab99 = 6
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
        B204S
    }

    public enum VINTurboModel : int
    {
        None,
        GarretT25,
        GarretTB2529,
        GarretTB2531,
        MitsuTD04,
        GarrettT17
    }

    public class VINDecoder
    {
        public VINCarInfo DecodeVINNumber(string VINNumber)
        {
            VINCarInfo _carInfo = new VINCarInfo();
            if (VINNumber.StartsWith("YK1")) // finland?
            {
                _carInfo.Makeyear = DecodeMakeyear(VINNumber);
                _carInfo.CarModel = DecodeCarModel(VINNumber);
                _carInfo.EngineType = DecodeEngineType(VINNumber);
                _carInfo.PlantInfo = DecodePlantInfo(VINNumber);
                _carInfo.Series = DecodeSeries(VINNumber);
                _carInfo.Body = DecodeBodyType(VINNumber);
                _carInfo.TurboModel = DecodeTurboModel(_carInfo.EngineType, _carInfo.CarModel);
                _carInfo.Valid = true;
            }
            else if (VINNumber.StartsWith("YS3")) // sweden?
            {
                _carInfo.Makeyear = DecodeMakeyear(VINNumber);
                _carInfo.CarModel = DecodeCarModel(VINNumber);
                _carInfo.EngineType = DecodeEngineType(VINNumber);
                _carInfo.PlantInfo = DecodePlantInfo(VINNumber);
                _carInfo.Series = DecodeSeries(VINNumber);
                _carInfo.Body = DecodeBodyType(VINNumber);
                _carInfo.TurboModel = DecodeTurboModel(_carInfo.EngineType, _carInfo.CarModel);
                _carInfo.Valid = true;
            }
            return _carInfo;
        }

        private VINTurboModel DecodeTurboModel(VINEngineType vINEngineType, VINCarModel carModel)
        {
            // depending on enginetype and vehicletype, determine turbo type
            switch (vINEngineType)
            {
                case VINEngineType.B204E:
                case VINEngineType.B204L:
                    return VINTurboModel.GarretT25;
                case VINEngineType.B204R:
                    return VINTurboModel.GarretT25;
                case VINEngineType.B205E:
                case VINEngineType.B205L:
                    return VINTurboModel.GarretT25;
                case VINEngineType.B205R:
                    return VINTurboModel.MitsuTD04;
                case VINEngineType.B234E:
                case VINEngineType.B234L:
                    return VINTurboModel.GarretT25;
                case VINEngineType.B234R:
                    return VINTurboModel.MitsuTD04;
                case VINEngineType.B235E:
                case VINEngineType.B235L:
                    return VINTurboModel.GarrettT17;
                case VINEngineType.B235R:
                    return VINTurboModel.MitsuTD04;
            }
            return VINTurboModel.None;
        }

        private string DecodeTransmissionType(string VINNumber)
        {
            if (VINNumber.Length < 7) return string.Empty;
            else if (VINNumber[6] == '4') return "4 speed manual";
            else if (VINNumber[6] == '5') return "5-speed manual / front wheel drive";
            else if (VINNumber[6] == '6') return "3 speed automatic";
            else if (VINNumber[6] == '8') return "4-speed automatic";
            else if (VINNumber[6] == '9') return "5 speed automatic";
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

        private string DecodeSeries(string VINNumber)
        {
            if (VINNumber.Length < 5) return string.Empty;
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
            else return string.Empty;
            /*
            B = i 
            C = i16 
            D = Turbo 
             * */
        }

        private string DecodePlantInfo(string VINNumber)
        {
            if (VINNumber.Length < 11) return string.Empty;
            else if (VINNumber[10] == '1') return "Trollh�ttan line A";
            else if (VINNumber[10] == '2') return "Trollh�ttan line B (900 / 9-3)";
            else if (VINNumber[10] == '3') return "Trollh�ttan line A";
            else if (VINNumber[10] == '5') return "Malm�, Sweden";
            else if (VINNumber[10] == '6') return "Nystad, Finland";
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
            else if (VINNumber[3] == 'D') return VINCarModel.Saab93;
            else if (VINNumber[3] == 'E') return VINCarModel.Saab95; // 9-5
            else if (VINNumber[3] == 'F') return VINCarModel.Saab93;
            else return VINCarModel.Unknown;
        }

        private VINEngineType DecodeEngineType(string VINNumber)
        {
            if (VINNumber.Length < 8) return VINEngineType.Unknown;
            else if (VINNumber[7] == 'A') return VINEngineType.B206I;
            else if (VINNumber[7] == 'B') return VINEngineType.B234I;
            else if (VINNumber[7] == 'C') return VINEngineType.B205E;
            // A = B206I 4-inline, Fuel injection, PETROL, I (B206 I) 
            // B = B234I 4-inline, Fuel injection, PETROL, I (B234 I) 
            // C = B205E 4-inline, Turbo, PETROL, TURBO (B205 E)
            // D = D223L 4-in line, Turbo Diesel, DIESEL, TURBO (D223 L) 
            // E = B235E 4-inline, Turbo, PETROL, TURBO (B235 E) 
            // G = B235R 4-inline, Turbo, PETROL, TURBO (B235 R) 
            // H = B205L 4-inline, Turbo, PETROL, TURBO (B205 L)
            // J = B204I 4-inline, Fuel injection, PETROL, I (B204 I)
            // K = B205R 4-inline, Turbo, PETROL, TURBO (B205 R)
            // M = B234L 4-inline, Turbo, PETROL, TURBO (B234 L) 
            // N = B204L 4-inline, Turbo, PETROL, TURBO (B204 L)
            // P = B204S 4-inline, Turbo, PETROL, TURBO (B204 S)
            // R = B234R 4-inline, Turbo, PETROL, TURBO (B234 R)
            // U = B234E 4-inline, Turbo, PETROL, TURBO (B234 E) 
            // V = B258I V6, Fuel injection, PETROL, I (B258 I)
            // W = B308I V6, Fuel injection, PETROL, I (B308 I) 
            else if (VINNumber[7] == 'D') return VINEngineType.B202I;
            else if (VINNumber[7] == 'E') return VINEngineType.B235E;
            else if (VINNumber[7] == 'G') return VINEngineType.B235R;
            else if (VINNumber[7] == 'H') return VINEngineType.B205L;
            else if (VINNumber[7] == 'J') return VINEngineType.B204I;
            else if (VINNumber[7] == 'K') return VINEngineType.B205R;
            else if (VINNumber[7] == 'L') return VINEngineType.B202L;
            else if (VINNumber[7] == 'M') return VINEngineType.B234L;
            else if (VINNumber[7] == 'N') return VINEngineType.B204L;
            else if (VINNumber[7] == 'P') return VINEngineType.B204S;
            else if (VINNumber[7] == 'R') return VINEngineType.B234R;
            else if (VINNumber[7] == 'S') return VINEngineType.B202E;
            else if (VINNumber[7] == 'U') return VINEngineType.B234E;
            else if (VINNumber[7] == 'V') return VINEngineType.B258I;
            else if (VINNumber[7] == 'W') return VINEngineType.B308I;
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
            else if (VINNumber[9] == 'A') return 1980;
            else if (VINNumber[9] == 'B') return 1981;
            else if (VINNumber[9] == 'C') return 1982;
            else if (VINNumber[9] == 'D') return 1983;
            else if (VINNumber[9] == 'E') return 1984;
            else if (VINNumber[9] == 'F') return 1985;
            else if (VINNumber[9] == 'G') return 1986;
            else if (VINNumber[9] == 'H') return 1987;
            else if (VINNumber[9] == 'J') return 1988;
            else if (VINNumber[9] == 'K') return 1989;
            else if (VINNumber[9] == 'L') return 1990;
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
            return 0;
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

    }
}
