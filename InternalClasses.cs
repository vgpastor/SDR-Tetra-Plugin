namespace SDRSharp.Tetra
{
    public enum LCType
    {
        BSCH,
        BNCH,
        AACH,
        BLCH,
        CLCH,
        SCH_F,
        SCH_HD,
        SCH_HU,
        TCH,
        STCH,
    }

    public unsafe class LogicChannel
    {
        public byte* Ptr;
        public int Length;
        public bool CrcIsOk;
        public int TimeSlot;
        public int Frame;
    }

    public enum RulesType
    {
        Direct = 0,
        Options_bit,
        Presence_bit,
        More_bit,
        Switch,
        SwitchNot,
        Reserved,
        Jamp,
        JampNot
    }

    public struct Calls
    {
        public int CallIdent;
        public int TXer;
        public int SSI1;
        public int SSI2;
        public bool IsEncripted;
        public bool Duplex;
        public int AssignedSlot;
    }

    public struct Rules
    {
        public GlobalNames GlobalName;
        public int Length;
        public RulesType Type;
        public int Ext1;
        public int Ext2;
        public int Ext3;

        public Rules(GlobalNames globalName, int length, RulesType type = RulesType.Direct, int ext1 = 0, int ext2 = 0, int ext3 = 0)
        {
            GlobalName = globalName;
            Length = length;
            Type = type;
            Ext1 = ext1;
            Ext2 = ext2;
            Ext3 = ext3;
        }
    }
}
