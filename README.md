# ModifiableValue

## 简介
惰性数值修改。当需要数据的时候，根据数据初始值 + 修改器计算出新值。
- `IValueModifier<T>` 将对数值的修改行为抽象为一个对象，该对象内定义修改的计算逻辑。
- `IModifiable<T>` 定义了被修改的数值。它包含初始值、增添修改器，以及每次计算之后的缓存值。

## 依赖
- 使用了 `OdinSerialize` 与 `OdinInspector`。
- [扩展、帮助函数](https://github.com/VodByte/UnityCSharpHelpers)

## 用例
```CSarp
// 攻击力
public ModifiableValue<float> atkPoint = new(12);

/// 某个增强攻击力的 buff
public class PowerBuff
{
    // 初始化一个数值修改器
    private sealed class PowerBufferModifier : IValueModifier<falot>
    {
        // 优先级一般
        public int Priority => 0;
        public void Modify(ref float r_initVal)
        {
            /// 自定义对攻击力的修改逻辑：
            /// 如 当前游戏时长 * 玩家等级
            r_initVal += GameManager.PlayTime * Player.CurrentLevel;
        }
    }
    var m_modifier = new PowerBufferModifier();

    // 获得某 buff 增强攻击力
    atkPoint.AddModifier(m_modifier);
}

// 当获取攻击力的时候，会运行 PowerBufferModifier 中定义的计算逻辑
float currentAtkPoint = atkPoint.ApplyModifier();
```

## 其他
如果你了解函数式编程，你一定更能了解我想做什么。
