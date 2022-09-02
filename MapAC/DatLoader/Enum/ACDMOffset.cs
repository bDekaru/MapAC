namespace MapAC.DatLoader.Enum
{
    public enum ACDMOffset : uint
    {
        /// 
        /// CELL
        /// 
        // misc in Cell
        EnvCell = 0x10000,


        /// 
        /// PORTAL
        /// 

        // 0x01 in Portal, max 0x4E59
        GfxObj = 0x5000, 
        // 0x02 in Portal, max 0x1C56
        Setup = 0x2000,
        // 0x03 in Portal, max 0x0E24
        Animation = 0x1000,
        // 0x04, max 0x2059
        Palette = 0x3000,
        // 0x05, max is 0x3358
        SurfaceTexture = 0x4000,
        // 0x06, max is 0x7576
        Texture = 0x8000,
        // 0x08, max is 0x194D
        Surface = 0x2000,
        // 0x09, max is 0x0231
        MotionTable = 0x1000,
        // 0x0A, max is 0x05B5
        Sound = 0x1000,
        Wave = 0x1000, // alt name
        // 0x0D, max is 0x063F
        Environment = 0x1000,
        // 0x0F, max is 0x0B6B
        PaletteSet = 0x1000,
        // 0x10, max is 0x086C
        ClothingTable = 0x1000,
        // 0x11, max is 0x10BF
        DIDDegrade = 0x2000,
        // 0x12, max is 0x02C6
        //Scene = 0x1000,
        Scene = 0x0000,

        // 0x20, max is 0x00DA
        SoundTable = 0x0100,

        // 0x30, max is 0x004D
        CombatTable = 0x0100,
        // 0x31, max is 0x0025
        String = 0x0100,
        // 0x32, max is 0x0A83
        EmitterInfo = 0x1000,
        // 0x33, max is 0x139F
        PhysicsScript = 0x2000,
        // 0x34, max is 0x00D7
        PhysicsScriptTable = 0x0100,

    }
}
