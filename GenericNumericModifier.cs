using UnityEngine;
using System;
using Sirenix.OdinInspector;

/// 泛用的数值修改器，都是常见的数值类型，与常见的操作（加减乘除）
namespace VodByte.ModifiableValue
{
#region Int类型
    public interface IIntValueModifier : IValueModifier<int> { }

    /////////////////////////////////////////////////////////
    [Serializable]
    [InlineProperty]
    public sealed class IntAdditionModifier : IIntValueModifier
    {
        [field: SerializeField]
        public int Addend { get; set; }
        public int Priority => 0;

        public void Modify(ref int r_initialValue)
        {
            r_initialValue += Addend;
        }

        void IValueModifier.Modify(ref object ref_initialValue) => throw new NotImplementedException();
    }

    /////////////////////////////////////////////////////////
    [Serializable]
    [InlineProperty]
    public sealed class IntMultiplicationModifier : IValueModifier<int>
    {
        [field: SerializeField]
        public float Multiplier { get; set; } = 1;

        public int Priority => 1;

        public IntMultiplicationModifier(float _multiplier) => Multiplier = _multiplier;

        public void Modify(ref int r_initialValue)
        {
            float temp = (float)r_initialValue * Multiplier;
            r_initialValue = (int)temp;
        }

        void IValueModifier.Modify(ref object ref_initialValue) => throw new System.NotImplementedException();
    }
#endregion

#region Float类型
    public interface IFloatValueModifier : IValueModifier<float> { }

    /////////////////////////////////////////////////////////
    [Serializable]
    [InlineProperty]
    public sealed class FloatAdditionModifier : IFloatValueModifier
    {
        [field: SerializeField]
        public float Addend { get; set; }

        public int Priority => 0;

        public FloatAdditionModifier(float _addend) => Addend = _addend;

        public void Modify(ref float r_initialValue)
        {
            r_initialValue += Addend;
        }

        void IValueModifier.Modify(ref object ref_initialValue) => throw new System.NotImplementedException();
    }

    /////////////////////////////////////////////////////////
    [Serializable]
    [InlineProperty]
    public sealed class FloatMultiplicationModifier : IValueModifier<float>
    {
        [field: SerializeField]
        public float Multiplier { get; set; } = 1f;

        public int Priority => 1;

        public FloatMultiplicationModifier(float _multiplier) => Multiplier = _multiplier;

        public void Modify(ref float r_initialValue)
        {
            r_initialValue *= Multiplier;
        }

        void IValueModifier.Modify(ref object ref_initialValue) => throw new System.NotImplementedException();
    }
#endregion
}