namespace VodByte.ModifiableValue
{
    /// <summary>
    /// 属性值修改器基类
    /// </summary>
    public interface IValueModifier
    {
        // 值越大，越优先使用。例如，加减法为 0 优先权，乘除法为 1
        int Priority { get; }
        void Modify(ref object ref_initialValue);
    }

    /////////////////////////////////////////////////////////
    /// <summary>
    /// 属性值修改器基类
    /// </summary>
    public interface IValueModifier<T> : IValueModifier
    {
        // 值越大，越优先使用。例如，加减法为 0 优先权，乘除法为 1
        new int Priority { get; }
        void Modify(ref T ref_initialValue);
    }
}
