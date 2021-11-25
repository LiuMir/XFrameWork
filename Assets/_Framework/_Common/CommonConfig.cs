// UI层级枚举
public enum UIWindowLayer 
{
    WindowLayer = 1,
    PopLayer = 2,
    MsgLayer = 3,
    OnThingLayer = 4,
}

// 全局变量
public class GlobalVariable
{
    public static readonly int MinWindowLayerOrder = 2100; // WindowLayer 最低在canvas order in layer  -> popLayer 层级与当前依附的windowlayer层级一致
    public static readonly int MinMsgLayerOrder = 6100; // MsgLayer 最低在canvas order in layer
}




