using System;

namespace VodByte.ModifiableValue
{
    public interface IModifiable
    {
        object Initial { get; }

        ///<summary> 缓存的修改后的数值，每次使用 <see cref="ApplyModifier"/> 都会进行更新 </summary>
        object Cached { get; }

        ///<summary> 使用修改器后，会得到的数值 </summary>
        object ApplyModifier();

        ///<summary> 添加修改器 </summary>
        void AddModifier(IValueModifier<object> _modifier);

        ///<summary> 移除修改器 </summary>
        bool RemoveModifier(IValueModifier<object> _modifier);

        void ClearModifiers();

        event Action OnAddedModifier;
        event Action OnRemovedModifier;
    }

    ///---------------------------------------------------------------
    public interface IModifiable<T> : IModifiable
    {
        new T Initial { get; }

        ///<summary> 缓存的修改后的数值，每次使用 <see cref="ApplyModifier"/> 都会进行更新 </summary>
        new T Cached { get; }

        ///<summary> 使用修改器后，会得到的数值 </summary>
        new T ApplyModifier();

        ///<summary> 添加修改器 </summary>
        void AddModifier(IValueModifier<T> _modifier);

        ///<summary> 移除修改器 </summary>
        bool RemoveModifier(IValueModifier<T> _modifier);
    }
}