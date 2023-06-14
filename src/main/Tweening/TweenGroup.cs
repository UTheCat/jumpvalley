using Godot;
using System.Collections.Generic;

/// <summary>
/// A class capable of overseeing multiple tweens as a "group".
/// <br/>
/// It offers support for storing tweens in a key-value map (also called a <see cref="Dictionary{TKey, TValue}"/>) and doing various operations with all of the tweens in the map at once.
/// <br/>
/// In addition, it can be used to make Godot tweens work with objects that aren't in the scene tree, particularly, through the use of <see cref="Tween.TweenMethod"/>
/// <br/>
/// Please note: In order for this to work, an instance of <see cref="TweenGroup{TKey}"/> must be parented to a Godot node of some sort (preferably one that will be in the scene tree until the application exits). This is due to the Godot limitation that Tweens must be in the scene tree in order to be processed.
/// </summary>
/// <typeparam name="TKey">The type of keys used in the tween map</typeparam>
public partial class TweenGroup<TKey>: Node
{
    private Dictionary<TKey, Tween> tweens = new Dictionary<TKey, Tween>();

    public TweenGroup()
    {

    }

    public void Add(TKey key, Tween t)
    {
        if (tweens[key] == null)
        {
            tweens[key] = t;
        }
    }
}