using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using MoonBrookRidge.Core.Systems;
using MoonBrookRidge.UI.Dialogue;

namespace MoonBrookRidge.Characters.NPCs;

/// <summary>
/// Manages all NPCs in the game world
/// </summary>
public class NPCManager
{
    private List<NPCCharacter> _npcs;
    private Dictionary<string, Texture2D> _npcSprites;
    private ChatBubble _chatBubble;
    private RadialDialogueWheel _dialogueWheel;
    private NPCCharacter _interactingNPC;
    
    // Interaction settings
    private const float INTERACTION_DISTANCE = 64f; // Distance at which player can interact with NPC
    private const float CHAT_BUBBLE_DISTANCE = 80f; // Distance at which chat bubbles appear
    
    public NPCManager()
    {
        _npcs = new List<NPCCharacter>();
        _npcSprites = new Dictionary<string, Texture2D>();
        _chatBubble = new ChatBubble();
        _dialogueWheel = new RadialDialogueWheel();
        _interactingNPC = null;
        
        // Wire up dialogue wheel events
        _dialogueWheel.OnOptionSelected += OnDialogueOptionSelected;
    }
    
    public void LoadContent(Dictionary<string, Texture2D> npcSprites)
    {
        _npcSprites = npcSprites;
    }
    
    /// <summary>
    /// Add an NPC to the world
    /// </summary>
    public void AddNPC(NPCCharacter npc)
    {
        _npcs.Add(npc);
    }
    
    /// <summary>
    /// Remove an NPC from the world
    /// </summary>
    public void RemoveNPC(NPCCharacter npc)
    {
        _npcs.Remove(npc);
    }
    
    /// <summary>
    /// Get NPC by name
    /// </summary>
    public NPCCharacter GetNPC(string name)
    {
        return _npcs.Find(npc => npc.Name == name);
    }
    
    /// <summary>
    /// Get nearest NPC within a given distance
    /// </summary>
    public NPCCharacter GetNearbyNPC(Vector2 position, float maxDistance = 64f)
    {
        NPCCharacter nearestNPC = null;
        float nearestDistance = float.MaxValue;
        
        foreach (var npc in _npcs)
        {
            float distance = Vector2.Distance(npc.Position, position);
            if (distance < maxDistance && distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestNPC = npc;
            }
        }
        
        return nearestNPC;
    }
    
    public void Update(GameTime gameTime, TimeSystem timeSystem, Vector2 playerPosition, bool interactPressed)
    {
        // Update all NPCs (always simulated, even when off-screen - Phase 7.4)
        // This ensures NPCs follow their daily schedules and can be found by players
        foreach (var npc in _npcs)
        {
            npc.Update(gameTime, timeSystem);
            
            // Check distance to player for chat bubble display
            float distance = Vector2.Distance(npc.Position, playerPosition);
            
            // Show chat bubble if player is near
            if (distance < CHAT_BUBBLE_DISTANCE && !_dialogueWheel.IsActive)
            {
                // Show a greeting or status message
                if (!_chatBubble.IsVisible)
                {
                    _chatBubble.Show(npc.Position, GetGreetingText(npc));
                }
            }
            
            // Check for interaction
            if (distance < INTERACTION_DISTANCE && interactPressed && !_dialogueWheel.IsActive)
            {
                StartInteraction(npc);
            }
        }
        
        // Update UI systems
        _chatBubble.Update(gameTime);
        _dialogueWheel.Update(gameTime);
    }
    
    private string GetGreetingText(NPCCharacter npc)
    {
        // Get appropriate greeting based on time of day or friendship level
        int heartLevel = npc.GetHeartLevel();
        
        if (heartLevel >= 8)
        {
            return $"Hey there, friend!";
        }
        else if (heartLevel >= 4)
        {
            return $"Hello!";
        }
        else
        {
            return $"Hi.";
        }
    }
    
    private void StartInteraction(NPCCharacter npc)
    {
        _interactingNPC = npc;
        _chatBubble.Hide();
        
        // Get dialogue tree for this NPC
        var dialogueTree = npc.GetDialogue("greeting");
        if (dialogueTree != null)
        {
            var currentNode = dialogueTree.GetCurrentNode();
            if (currentNode != null)
            {
                // Show dialogue wheel with options
                var options = currentNode.Options;
                _dialogueWheel.Show(npc.Position, options);
            }
        }
    }
    
    private void OnDialogueOptionSelected(int optionIndex)
    {
        if (_interactingNPC != null)
        {
            var dialogueTree = _interactingNPC.GetDialogue("greeting");
            if (dialogueTree != null)
            {
                var currentNode = dialogueTree.GetCurrentNode();
                if (currentNode != null && optionIndex < currentNode.Options.Count)
                {
                    var selectedOption = currentNode.Options[optionIndex];
                    
                    // Progress dialogue tree
                    dialogueTree.SelectOption(optionIndex);
                    
                    var nextNode = dialogueTree.GetCurrentNode();
                    if (nextNode != null && nextNode.Options.Count > 0)
                    {
                        // Show next set of options
                        _dialogueWheel.Show(_interactingNPC.Position, nextNode.Options);
                    }
                    else
                    {
                        // End of dialogue
                        EndInteraction();
                    }
                }
            }
        }
    }
    
    private void EndInteraction()
    {
        _dialogueWheel.Hide();
        _interactingNPC = null;
    }
    
    public void Draw(SpriteBatch spriteBatch, SpriteFont font, Rectangle? visibleBounds = null)
    {
        // Draw all NPCs (with optional frustum culling - Phase 7.4)
        // NOTE: NPCs are ALWAYS updated in Update() regardless of visibility
        // Culling here only affects rendering, not simulation/schedules
        foreach (var npc in _npcs)
        {
            // If viewport bounds provided, check if NPC is visible
            if (visibleBounds.HasValue)
            {
                // Simple visibility check - NPC position within viewport bounds
                // Add buffer around viewport to prevent pop-in
                Rectangle npcBounds = new Rectangle(
                    (int)npc.Position.X - 32,
                    (int)npc.Position.Y - 32,
                    64, 64
                );
                
                if (!visibleBounds.Value.Intersects(npcBounds))
                {
                    continue; // Skip NPCs outside visible area
                }
            }
            
            npc.Draw(spriteBatch);
        }
        
        // Draw chat bubble (always visible when active)
        _chatBubble.Draw(spriteBatch, font);
    }
    
    public void DrawUI(SpriteBatch spriteBatch, SpriteFont font)
    {
        // Draw dialogue wheel (this should be drawn after world transform)
        _dialogueWheel.Draw(spriteBatch, font);
    }
    
    public bool IsDialogueActive => _dialogueWheel.IsActive;
    
    public int NPCCount => _npcs.Count;
}
