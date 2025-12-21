using System.Collections.Generic;

namespace MoonBrookRidge.Characters.NPCs;

/// <summary>
/// Dialogue tree for branching conversations
/// </summary>
public class DialogueTree
{
    private DialogueNode _rootNode;
    private DialogueNode _currentNode;
    
    public DialogueTree(DialogueNode rootNode)
    {
        _rootNode = rootNode;
        _currentNode = rootNode;
    }
    
    public void Reset()
    {
        _currentNode = _rootNode;
    }
    
    public DialogueNode GetCurrentNode()
    {
        return _currentNode;
    }
    
    public void SelectOption(int optionIndex)
    {
        if (_currentNode != null && optionIndex < _currentNode.Options.Count)
        {
            _currentNode = _currentNode.Options[optionIndex].NextNode;
        }
    }
    
    public bool IsComplete()
    {
        return _currentNode == null || _currentNode.Options.Count == 0;
    }
}

/// <summary>
/// Single node in a dialogue tree
/// </summary>
public class DialogueNode
{
    public string Text { get; set; }
    public string SpeakerName { get; set; }
    public List<DialogueOption> Options { get; set; }
    public string EmotionSprite { get; set; } // Optional emotion/expression
    
    public DialogueNode(string text, string speakerName = "")
    {
        Text = text;
        SpeakerName = speakerName;
        Options = new List<DialogueOption>();
    }
    
    public void AddOption(string optionText, DialogueNode nextNode, int friendshipRequirement = 0)
    {
        Options.Add(new DialogueOption
        {
            Text = optionText,
            NextNode = nextNode,
            FriendshipRequirement = friendshipRequirement
        });
    }
}

/// <summary>
/// A dialogue option in the radial wheel
/// </summary>
public class DialogueOption
{
    public string Text { get; set; }
    public DialogueNode NextNode { get; set; }
    public int FriendshipRequirement { get; set; }
}
