using System;
using System.Collections.Generic;
using System.Linq;

using Sirenix.OdinInspector;
using Sirenix.Serialization;

using UnityEngine;
using UnityEngine.Assertions;

namespace VodByte.ModifiableValue
{
    /// <summary>
    /// 可通过添加修改器，动态修正数值的类。<see cref="IValueModifier{T}"/>
    /// </summary>
    /// <typeparam name="T"> 数值类型 </typeparam>
    [Serializable]
    public class ModifiableValue<T> : IModifiable<T>
    {
        ///=============================================
        /// ███████╗██╗   ██╗███████╗███╗   ██╗████████╗
        /// ██╔════╝██║   ██║██╔════╝████╗  ██║╚══██╔══╝
        /// █████╗  ██║   ██║█████╗  ██╔██╗ ██║   ██║   
        /// ██╔══╝  ╚██╗ ██╔╝██╔══╝  ██║╚██╗██║   ██║   
        /// ███████╗ ╚████╔╝ ███████╗██║ ╚████║   ██║   
        /// ╚══════╝  ╚═══╝  ╚══════╝╚═╝  ╚═══╝   ╚═╝   
        ///=============================================
        public event Action OnAddedModifier;
        public event Action OnRemovedModifier;

        ///======================================================================
        /// ██████╗ ██████╗  ██████╗ ██████╗ ███████╗██████╗ ████████╗██╗███████╗
        /// ██╔══██╗██╔══██╗██╔═══██╗██╔══██╗██╔════╝██╔══██╗╚══██╔══╝██║██╔════╝
        /// ██████╔╝██████╔╝██║   ██║██████╔╝█████╗  ██████╔╝   ██║   ██║█████╗  
        /// ██╔═══╝ ██╔══██╗██║   ██║██╔═══╝ ██╔══╝  ██╔══██╗   ██║   ██║██╔══╝  
        /// ██║     ██║  ██║╚██████╔╝██║     ███████╗██║  ██║   ██║   ██║███████╗
        /// ╚═╝     ╚═╝  ╚═╝ ╚═════╝ ╚═╝     ╚══════╝╚═╝  ╚═╝   ╚═╝   ╚═╝╚══════╝
        ///======================================================================
        public T Initial => m_initialValue;
        public T Cached => m_cached;

        ///====================================
        /// ███████╗██╗███████╗██╗     ██████╗ 
        /// ██╔════╝██║██╔════╝██║     ██╔══██╗
        /// █████╗  ██║█████╗  ██║     ██║  ██║
        /// ██╔══╝  ██║██╔══╝  ██║     ██║  ██║
        /// ██║     ██║███████╗███████╗██████╔╝
        /// ╚═╝     ╚═╝╚══════╝╚══════╝╚═════╝ 
        ///====================================
        /// <summary>
        /// runtime被添加的修改器。
        /// 由于该类常被序列化使用，而该变量不会被序列化，runtime 中直接调用的话为 null，
        /// 所以要访问此变量时，通过 <see cref="GetModifiers"/>
        /// </summary>
        [HideInEditorMode, PropertyOrder(999), LabelText("修改器"), ReadOnly]
        SortedDictionary<int, List<IValueModifier<T>>> _modifiers;

        ///---------------------------------------------------------------
        [OdinSerialize]
        [LabelText("初始值"), PropertyOrder(-1), ShowInInspector]
        protected T m_initialValue;

        protected T m_cached;

        ///===================================
        ///  ██████╗████████╗ ██████╗ ██████╗ 
        /// ██╔════╝╚══██╔══╝██╔═══██╗██╔══██╗
        /// ██║        ██║   ██║   ██║██████╔╝
        /// ██║        ██║   ██║   ██║██╔══██╗
        /// ╚██████╗   ██║   ╚██████╔╝██║  ██║
        ///  ╚═════╝   ╚═╝    ╚═════╝ ╚═╝  ╚═╝
        ///===================================
        public ModifiableValue()
        {
            m_initialValue  = default;
            m_cached = m_initialValue;
        }

        ///---------------------------------------------------------------
        public ModifiableValue(T _presetVal)
        {
            m_initialValue = _presetVal;
            m_cached = _presetVal;
        }

        ///=============================================
        ///  ██████╗ ██╗   ██╗████████╗███████╗██████╗ 
        /// ██╔═══██╗██║   ██║╚══██╔══╝██╔════╝██╔══██╗
        /// ██║   ██║██║   ██║   ██║   █████╗  ██████╔╝
        /// ██║   ██║██║   ██║   ██║   ██╔══╝  ██╔══██╗
        /// ╚██████╔╝╚██████╔╝   ██║   ███████╗██║  ██║
        ///  ╚═════╝  ╚═════╝    ╚═╝   ╚══════╝╚═╝  ╚═╝
        ///=============================================
        /// <summary>
        /// 给属性值添加修改器
        /// </summary>
        /// <param name="_modifier"> 修改器实例 </param>ifier _modifier)
        public virtual void AddModifier(IValueModifier<T> _modifier)
        {
            Assert.IsNotNull(_modifier);
            GetModifiers().AddOrUpdate(_modifier.Priority, _modifier);
            /// 不应该出现空值
            Assert.IsFalse(_modifiers.Values.Any(_list => _list.Any(n => n == null)));
            OnAddedModifier?.Invoke();
        }

        ///---------------------------------------------------------------
        public virtual bool RemoveModifier(IValueModifier<T> _modifier)
        {
            bool result = GetModifiers().Remove(_modifier.Priority, _modifier);
            if (result) OnRemovedModifier?.Invoke();
            return result;
        }

        ///---------------------------------------------------------------
        /// <summary>
        /// 使用所有保存的修改器，计算出一个新的数值（不会在该类的实例的缓存）。
        /// 如果没有修改器，会直接返回初始值
        /// </summary>
        public virtual T ApplyModifier()
        {
            if (GetModifiers().IsEmpty())
            {
                m_cached = m_initialValue;
                return m_initialValue;
            }

            var temp = m_initialValue;
            foreach (var list in _modifiers.Values)
            {
                foreach (var modifier in list)
                {
                    if (modifier == null)
                    {
                        Debug.LogError($"{nameof(ModifiableValue<T>)}: 出现空引用修改器");
                        continue;
                    }
                    modifier.Modify(ref temp);
                }
            }
            m_cached = temp;
            return temp;
        }

        ///=========================================
        /// ██╗███╗   ██╗███╗   ██╗███████╗██████╗ 
        /// ██║████╗  ██║████╗  ██║██╔════╝██╔══██╗
        /// ██║██╔██╗ ██║██╔██╗ ██║█████╗  ██████╔╝
        /// ██║██║╚██╗██║██║╚██╗██║██╔══╝  ██╔══██╗
        /// ██║██║ ╚████║██║ ╚████║███████╗██║  ██║
        /// ╚═╝╚═╝  ╚═══╝╚═╝  ╚═══╝╚══════╝╚═╝  ╚═╝
        ///=========================================
        private SortedDictionary<int, List<IValueModifier<T>>> GetModifiers()
        {
            _modifiers ??= new();
            return _modifiers;
        }

        ///---------------------------------------------------------------
        /// <summary> 清除所有的修改器 </summary>
        /// <returns> 清除掉的修改器数量 </returns>
        public virtual void ClearModifiers()
        {
            foreach (var list in GetModifiers().Values)
            {
                list.Clear();
                list.TrimExcess();
            }
        }

        ///---------------------------------------------------------------
        /// <summary>
        /// 设置新的初始值。
        /// 不把它放在 <see cref="IModifiable{T}"/> 接口里是有意的，实例化本类之后，
        /// 仅将接口暴露给外界，外界只能通过添加 <see cref="IValueModifier{T}"/> 修改数值，
        /// 只有实例的拥有者有能力修改初始值，保证安全性
        /// </summary>
        /// <param name="_newInitialVal"> 新值 </param>
        /// <returns> 设置之后的值 </returns>
        public virtual T SetInitial(T _newInitialVal)
        {
            m_initialValue = _newInitialVal;
            return m_initialValue;
        }

        ///=======================================================================
        /// ██╗███╗   ██╗████████╗███████╗██████╗ ███████╗ █████╗  ██████╗███████╗
        /// ██║████╗  ██║╚══██╔══╝██╔════╝██╔══██╗██╔════╝██╔══██╗██╔════╝██╔════╝
        /// ██║██╔██╗ ██║   ██║   █████╗  ██████╔╝█████╗  ███████║██║     █████╗  
        /// ██║██║╚██╗██║   ██║   ██╔══╝  ██╔══██╗██╔══╝  ██╔══██║██║     ██╔══╝  
        /// ██║██║ ╚████║   ██║   ███████╗██║  ██║██║     ██║  ██║╚██████╗███████╗
        /// ╚═╝╚═╝  ╚═══╝   ╚═╝   ╚══════╝╚═╝  ╚═╝╚═╝     ╚═╝  ╚═╝ ╚═════╝╚══════╝
        ///=======================================================================
        object IModifiable.Initial => Initial;
        object IModifiable.Cached => Cached;

        object IModifiable.ApplyModifier() => ApplyModifier();
        void IModifiable.AddModifier(IValueModifier<object> _modifier) => AddModifier((IValueModifier<T>)_modifier);
        bool IModifiable.RemoveModifier(IValueModifier<object> _modifier) => RemoveModifier((IValueModifier<T>)_modifier);
    }
}
